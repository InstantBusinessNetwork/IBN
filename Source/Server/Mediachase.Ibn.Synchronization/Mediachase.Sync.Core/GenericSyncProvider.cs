using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Synchronization;
using Microsoft.Synchronization.MetadataStorage;
using Mediachase.Sync.Core.ErrorManagment;

namespace Mediachase.Sync.Core
{
	public abstract class GenericSyncProvider<T> : KnowledgeSyncProvider, IChangeDataRetriever, INotifyingChangeApplierTarget
													where T : class 
	{

		protected MetadataStore _metaDataStore;
		protected ReplicaMetadata _metaData;
		protected string ProviderName { get; set; }
		protected SyncSessionContext _currentSessionContext = null;

		protected uint RemainingSessionWorkEstimate = 0;
		private bool _fullEnumFirstCall = false;


		#region GenericSyncProviderMethods
		protected abstract void BeginTransaction();
		protected abstract void CommitTransaction();

		protected abstract void SaveDataStore();
		protected abstract void ReadDataStore();

		protected abstract void CreateDataItem(ItemChange change, ItemMetadata item,  T data);
		protected abstract void UpdateDataItem(ItemChange change, ItemMetadata item, T data);
		protected abstract bool DeleteDataItem(ItemChange change, ItemMetadata item,  T data);
		protected abstract T MergeDataItem(T dataItem, T otherDataItem);
		protected abstract T GetDataItem(ItemMetadata item);

		protected abstract void InitializeMetaDataStore();
		protected abstract void CloseMetaDataStore();

		protected abstract void UpdateMetadataStoreWithLocalChanges();

		protected abstract T ConvertFromTransferData(object data);
		protected abstract object ConvertToTransferData(T data);

		protected abstract void SaveItemMetadata(ItemMetadata item);

		protected abstract uint SyncBatchSize { get; }

		#endregion

		#region KnowledgeSyncProvider methods
		public override void BeginSession(SyncProviderPosition position, SyncSessionContext syncSessionContext)
		{
			InitializeMetaDataStore();
			ReadDataStore();
			if (_metaDataStore == null || _metaData == null)
			{
				throw new ArgumentNullException("metadata not initialized");
			}
			//Make sure the metadata store is updated to reflect the state of the data before each sync operation.
			BeginTransaction();
			UpdateMetadataStoreWithLocalChanges();
			CommitTransaction();

			_currentSessionContext = syncSessionContext;
		}

		public override void EndSession(SyncSessionContext syncSessionContext)
		{
			SaveDataStore();
			CloseMetaDataStore();
		}

		public override ChangeBatch GetChangeBatch(uint batchSize, SyncKnowledge destinationKnowledge, out object changeDataRetriever)
		{
			ChangeBatch batch = _metaData.GetChangeBatch(batchSize, destinationKnowledge);
			changeDataRetriever = this; //this is where the transfer mechanism/protocol would go. For an in memory provider, this is sufficient

			//Calculate estimate work
			batch.RemainingSessionWorkEstimate = RemainingSessionWorkEstimate;
			batch.BatchWorkEstimate = batch.IsLastBatch ? (uint)batch.Count() : batchSize;
			RemainingSessionWorkEstimate = batch.IsLastBatch ? 0 : RemainingSessionWorkEstimate - batchSize;

			return batch;
		}

		public override FullEnumerationChangeBatch GetFullEnumerationChangeBatch(uint batchSize, SyncId lowerEnumerationBound, SyncKnowledge knowledgeForDataRetrieval, out object changeDataRetriever)
		{
			FullEnumerationChangeBatch batch = _metaData.GetFullEnumerationChangeBatch(batchSize, lowerEnumerationBound, knowledgeForDataRetrieval);
			changeDataRetriever = this; //this is where the transfer mechanism/protocol would go. For an in memory provider, this is sufficient
			//get total item for calculating estimated work
			if (!_fullEnumFirstCall)
			{
				uint allDeletedItems;
				_metaData.GetItemCount(out RemainingSessionWorkEstimate, out allDeletedItems);
				_fullEnumFirstCall = true;
			}

			//Calculate estimate work
			batch.RemainingSessionWorkEstimate = RemainingSessionWorkEstimate;
			batch.BatchWorkEstimate = batch.IsLastBatch ? (uint)batch.Count() : batchSize;
			RemainingSessionWorkEstimate = batch.IsLastBatch ? 0 : RemainingSessionWorkEstimate - batchSize;

			return batch;
		}

		public override void GetSyncBatchParameters(out uint batchSize, out SyncKnowledge knowledge)
		{
			batchSize = this.SyncBatchSize;
			knowledge = _metaData.GetKnowledge();
			
		}

		/// <summary>
		/// When overridden in a derived class, gets the ID format schema of the provider.
		/// </summary>
		/// <value></value>
		/// <returns>The ID format schema of the provider.</returns>
		/// <remarks>Must be override in derivered class</remarks>
		public override SyncIdFormatGroup IdFormats
		{
			get { throw new NotImplementedException(); }
		}

		public override void ProcessChangeBatch(ConflictResolutionPolicy resolutionPolicy, ChangeBatch sourceChanges, object changeDataRetriever, SyncCallbacks syncCallbacks, SyncSessionStatistics sessionStatistics)
		{
			BeginTransaction();

			//Get all my local change versions from the metadata store
			IEnumerable<ItemChange> localChanges = _metaData.GetLocalVersions(sourceChanges);

			//Create a changeapplier object to make change application easier (make the engine call me 
			//when it needs data and when I should save data)
			NotifyingChangeApplier changeApplier = new NotifyingChangeApplier(IdFormats);

			//ForgottenKnowledge destinationForgottenKnowledge = new ForgottenKnowledge(IdFormats, );

			changeApplier.ApplyChanges(resolutionPolicy, sourceChanges, changeDataRetriever as IChangeDataRetriever, localChanges, _metaData.GetKnowledge(),
				_metaData.GetForgottenKnowledge(), this, _currentSessionContext, syncCallbacks);

			CommitTransaction();
		}

		//If full enumeration is needed because  this provider is out of date due to tombstone cleanup, then this method will be called by the engine.
		public override void ProcessFullEnumerationChangeBatch(ConflictResolutionPolicy resolutionPolicy, FullEnumerationChangeBatch sourceChanges, object changeDataRetriever, SyncCallbacks syncCallbacks, SyncSessionStatistics sessionStatistics)
		{
			BeginTransaction();

			//Get all my local change versions from the metadata store
			IEnumerable<ItemChange> localChanges = _metaData.GetFullEnumerationLocalVersions(sourceChanges);

			//Create a changeapplier object to make change application easier (make the engine call me 
			//when it needs data and when I should save data)
			NotifyingChangeApplier changeApplier = new NotifyingChangeApplier(this.IdFormats);
			changeApplier.ApplyFullEnumerationChanges(resolutionPolicy, sourceChanges, changeDataRetriever as IChangeDataRetriever, localChanges, _metaData.GetKnowledge(),
				_metaData.GetForgottenKnowledge(), this, _currentSessionContext, syncCallbacks);

			CommitTransaction();
		} 
		#endregion

		#region IChangeDataRetriever Members

		public object LoadChangeData(LoadChangeContext loadChangeContext)
		{
			object retVal = null;
			ItemMetadata item = _metaData.FindItemMetadataById(loadChangeContext.ItemChange.ItemId);
			if (item != null)
			{
				//return a copy of the data.
				retVal =  ConvertToTransferData(GetDataItem(item));
			}

			return retVal;
		}

		#endregion

		#region INotifyingChangeApplierTarget Members

		public IChangeDataRetriever GetDataRetriever()
		{
			return this;
		}

		public ulong GetNextTickCount()
		{
			return _metaData.GetNextTickCount();
		}

		public void SaveChangeWithChangeUnits(ItemChange change, SaveChangeWithChangeUnitsContext context)
		{
			throw new NotImplementedException();
		}

		public void SaveConflict(ItemChange conflictingChange, object conflictingChangeData, SyncKnowledge conflictingChangeKnowledge)
		{
			throw new NotImplementedException();
		}

		public void SaveItemChange(SaveChangeAction saveChangeAction, ItemChange change, SaveChangeContext context)
		{
			ItemMetadata item = null;
			T data = default(T);
			try
			{
				switch (saveChangeAction)
				{
					case SaveChangeAction.Create:
						try
						{
							//Do duplicate detection here
							item = _metaData.FindItemMetadataById(change.ItemId);
							if (item != null)
							{
								throw new SyncProviderException("SaveItemChange must not have Create action for existing items.");
							}
							item = _metaData.CreateItemMetadata(change.ItemId, change.CreationVersion);
							item.ChangeVersion = change.ChangeVersion;
							data = ConvertFromTransferData(context.ChangeData);
							CreateDataItem(change, item, data);
							SaveItemMetadata(item);
						}
						catch(Exception e)
						{
							throw new SyncProviderException(e, SaveChangeAction.Create);
						}
						break;
					case SaveChangeAction.UpdateVersionAndData:
					case SaveChangeAction.UpdateVersionOnly:
						try
						{
							item = _metaData.FindItemMetadataById(change.ItemId);
							if (null == item)
							{
								throw new SyncProviderException("Item Not Found in Store!?");
							}

							item.ChangeVersion = change.ChangeVersion;
							if (saveChangeAction == SaveChangeAction.UpdateVersionOnly)
							{
								SaveItemMetadata(item);
							}
							else  //Also update the data.
							{
								data = ConvertFromTransferData(context.ChangeData);
								UpdateDataItem(change, item, data);
								SaveItemMetadata(item);
							}
						}
						catch (Exception e)
						{
							throw new SyncProviderException(e, SaveChangeAction.UpdateVersionAndData);
						}
						break;

					case SaveChangeAction.DeleteAndStoreTombstone:
						try
						{
							item = _metaData.FindItemMetadataById(change.ItemId);
							if (null == item)
							{
								item = _metaData.CreateItemMetadata(change.ItemId, change.CreationVersion);
							}
							if (change.ChangeKind == ChangeKind.Deleted)
							{
								item.MarkAsDeleted(change.ChangeVersion);
							}
							else
							{
								// This should never happen in Sync Framework V1.0
								throw new SyncProviderException("Invalid ChangeType");
							}

							item.ChangeVersion = change.ChangeVersion;
							SaveItemMetadata(item);
							DeleteDataItem(change, item, data);//item.GlobalId.GetGuidId());
						}
						catch (Exception e)
						{
							throw new SyncProviderException(e, SaveChangeAction.DeleteAndStoreTombstone);
						}
						break;

					//Merge the changes! (Take the data from the local item + the remote item),noting to update the tick count to propagate the resolution!
					case SaveChangeAction.UpdateVersionAndMergeData:
						try
						{
							item = _metaData.FindItemMetadataById(change.ItemId);
							if (null == item)
							{
								throw new SyncProviderException("Item Not Found in Store!?");
							}
							if (item.IsDeleted != true)
							{
								//Note - you must update the change version to propagate the resolution!
								item.ChangeVersion = new SyncVersion(0, _metaData.GetNextTickCount());

								//Combine the conflicting data...
								T dataItem = GetDataItem(item);
								if (dataItem == null)
								{
									throw new SyncProviderException("data item not found for merge");
								}
								data = ConvertFromTransferData(context.ChangeData);
								data = MergeDataItem(dataItem, data);
								UpdateDataItem(change, item, data);
								SaveItemMetadata(item);
							}
						}
						catch (Exception e)
						{
							throw new SyncProviderException(e, SaveChangeAction.UpdateVersionAndMergeData);
						}
						break;

					case SaveChangeAction.DeleteAndRemoveTombstone:
						try
						{
							item = _metaData.FindItemMetadataById(change.ItemId);
							if (item != null)
							{
								_metaData.RemoveItemMetadata(new SyncId[] { item.GlobalId });
							}
							data = ConvertFromTransferData(context.ChangeData);
							DeleteDataItem(change, item, data);
						}
						catch (Exception e)
						{
							throw new SyncProviderException(e, SaveChangeAction.DeleteAndRemoveTombstone);
						}
						break;
				}
			}
			catch (SyncProviderException e)
			{
				Exception exception = e.InnerException != null ? e.InnerException : e;
				string itemDescr = e.SaveChangeAction.ToString();
				string errDescr =  e.Message + "[" + e.InnerException != null ? e.InnerException.ToString() 
																			  : string.Empty + "]";
				RecoverableErrorData recoverableError = new RecoverableErrorData(exception, itemDescr, errDescr);
				context.RecordRecoverableErrorForItem(recoverableError);
			}
		}

		public void StoreKnowledgeForScope(SyncKnowledge knowledge, ForgottenKnowledge forgottenKnowledge)
		{
			_metaData.SetKnowledge(knowledge);
            _metaData.SetForgottenKnowledge(forgottenKnowledge);
            _metaData.SaveReplicaMetadata();
		}

		public bool TryGetDestinationVersion(ItemChange sourceChange, out ItemChange destinationVersion)
		{
			ItemMetadata item = _metaData.FindItemMetadataById(sourceChange.ItemId);

			if (item == null)
			{
				destinationVersion = null;
				return false;
			}
			else
			{
				destinationVersion = new ItemChange(this.IdFormats, _metaData.ReplicaId, sourceChange.ItemId,
						item.IsDeleted ? ChangeKind.Deleted : ChangeKind.Update,
						item.CreationVersion, item.ChangeVersion);
				return true;
			}
		}

		#endregion
	}
}
