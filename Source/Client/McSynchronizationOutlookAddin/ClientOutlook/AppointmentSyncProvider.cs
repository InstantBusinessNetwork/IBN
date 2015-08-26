using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Sync.Core;
using Microsoft.Synchronization.MetadataStorage;
using Microsoft.Synchronization;
using System.IO;
using Mediachase.ClientOutlook.SyncTransferedData;
using Mediachase.Sync.Core.TransferDataType;
using Mediachase.ClientOutlook.SyncTransferedData.Appointment;
using Mediachase.ClientOutlook.Configuration;
using OutlookAddin;
using System.Runtime.InteropServices;
using OutlookAddin.OutlookItemAdaptors;
using Mediachase.Sync.Core.ErrorManagment;

namespace Mediachase.ClientOutlook
{
	public class AppointmentSyncProvider : GenericSyncProvider<SyncTransferData>, ISyncProviderSetting
	{
		// The name of the metadata store custom column that is used to save a timestamp of last change on an 
		// item in the metadata store so we can do change detection.
		const string TIMESTAMP_COLUMNNAME = "timestamp";
		const string URI_COLUMNNAME = "uri";

		private OutlookApplication _outlookApplication;
		private OutlookFolder _outlookFolder;
		private syncAppointmentSetting _activeSyncSetting;
		private SyncId _replicaId;
		private SyncIdFormatGroup _idFormats;
		
		private List<SyncTransferData> _localItems = new List<SyncTransferData>();

		private AppointmentSyncProvider(syncAppointmentSetting settings, OutlookApplication outlookApplication)
		{
			_outlookApplication = outlookApplication;
			_activeSyncSetting = settings;
		}

		public static AppointmentSyncProvider CreateInstance(syncAppointmentSetting settings, OutlookApplication outlookApplication)
		{
			return new AppointmentSyncProvider(settings, outlookApplication);
		}

		public SyncId ReplicaId
		{
			get
			{
				if (_replicaId == null)
				{
					_replicaId = new SyncId(new Guid(_activeSyncSetting.replicaId));
				}

				return _replicaId;
			}

		}

		public List<SyncTransferData> LocalItems
		{
			get
			{
				return _localItems;
			}
		}

		private OutlookFolder OutlookMapiFolder
		{
			get
			{
				DebugAssistant.Log("OutlookMapiFolder : get access");
				OutlookFolder retVal = _outlookFolder;
				if (retVal == null)
				{
					if (_activeSyncSetting == null)
						throw new NullReferenceException("appointment sync settings is empty");
					if (_outlookApplication == null)
						throw new NullReferenceException("_outlookListener");

					retVal = _outlookApplication.GetOutlookFolderFromPath(_activeSyncSetting.localFolder);

					if (retVal == null)
					{
						DebugAssistant.Log(DebugSeverity.MessageBox | DebugSeverity.Error, "Outlook folder {0} not found", _activeSyncSetting.localFolder);
						throw new NullReferenceException("retVal");
						//retVal = _outlookNS.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar);
					}
					_outlookFolder = retVal;
				}

				return _outlookFolder;
			}

			set
			{
				_outlookFolder = value;
			}
		}
		protected override void BeginTransaction()
		{
			if (_metaDataStore == null)
			{
				throw new ArgumentNullException("metadata store not initialized");
			}
			SqlMetadataStore metaDataStore = _metaDataStore as SqlMetadataStore;
			metaDataStore.BeginTransaction();
			
		}

		protected override void CommitTransaction()
		{
			if (_metaDataStore == null)
			{
				throw new ArgumentNullException("metadata store not initialized");
			}
			SqlMetadataStore metaDataStore = _metaDataStore as SqlMetadataStore;
			metaDataStore.CommitTransaction();
		}
		/// <summary>
		/// Initializes the meta data store.
		/// </summary>
		protected override void InitializeMetaDataStore()
		{
			// Values for adding a custom field to the metadata store
			List<FieldSchema> fields = new List<FieldSchema>();
			SyncId id = ReplicaId;
			string replicaMetadataFile = _activeSyncSetting.replicaStoreFileName;
			// Create or open the metadata store, initializing it with the id formats we'll use to reference our items and endpoints
			if (!File.Exists(replicaMetadataFile))
			{
				fields.Add(new FieldSchema(TIMESTAMP_COLUMNNAME, typeof(System.UInt64)));
				fields.Add(new FieldSchema(URI_COLUMNNAME, typeof(string), 1024));
				_metaDataStore = SqlMetadataStore.CreateStore(replicaMetadataFile);
				_metaDataStore.InitializeReplicaMetadata(IdFormats, ReplicaId, fields, new IndexSchema[] { new IndexSchema(URI_COLUMNNAME, true) });

			}
			else
			{
				_metaDataStore = SqlMetadataStore.OpenStore(replicaMetadataFile);
				
			}

			_metaData = _metaDataStore.GetReplicaMetadata(IdFormats, ReplicaId);
			
			//Если ReplicaId изменилась то необходимо пересоздать хранилище
			if (_metaData.ReplicaId != ReplicaId)
			{
				CloseMetaDataStore();
				File.Delete(replicaMetadataFile);
				InitializeMetaDataStore();
			}
			
		}

		/// <summary>
		/// Преобразует Outlook appointment  в иерархию объектов transferData
		/// </summary>
		/// <param name="appItem">The app item.</param>
		/// <returns></returns>
		private SyncTransferData Appointment2TransferData(OutlookAppointment appItem)
		{
			SyncTransferData retVal = null;
			ITransferDataSerializable serializer = new AppointmentSerializer(appItem);
			retVal = serializer.Serialize();
			if (appItem.IsRecurring)
			{
				OutlookRecurrencePattern rPattern = appItem.GetRecurrencePattern();
				serializer = new RecurrencePatternSerializer(rPattern);
				retVal.Childrens.Add(serializer.Serialize());
				serializer = null;
				foreach (OutlookException exception in rPattern.Exceptions)
				{
					AppointmentTransferData transferException = new AppointmentTransferData();
					if (!exception.Deleted)
					{
						OutlookAppointment appointmentException = exception.AppointmentItem;
						serializer = new AppointmentSerializer(appointmentException);
						transferException = (AppointmentTransferData)serializer.Serialize();
						//exception recipient
						foreach (OutlookRecipient exceptionRecipient in appointmentException.Recipients)
						{
							serializer = new RecipientSerializer(exceptionRecipient);
							transferException.Childrens.Add(serializer.Serialize());
						}
					}
					transferException.DeletedException = exception.Deleted;
					transferException.RecurrenceId = exception.OriginalDate;

					retVal.Childrens.Add(transferException);

				}
			}
			//appointment recipient
			foreach (OutlookRecipient recipient in appItem.Recipients)
			{
				serializer = new RecipientSerializer(recipient);
				retVal.Childrens.Add(serializer.Serialize());
			}
			
			//Инициализируем дополнительные поля (LastModified и Uri)
			retVal.LastModified = (ulong)appItem.LastModificationTime.ToUniversalTime().Ticks;
			retVal.Uri = appItem.EntryID;

			return retVal;

		}

		/// <summary>
		/// Преобразует иерархию объектов SyncTranferData в Outlook Appointment
		/// </summary>
		/// <param name="transferData">The transfer data.</param>
		/// <param name="appItem">The app item.</param>
		/// <returns></returns>
		private OutlookAppointment TransferData2AppointmentItem(SyncTransferData transferData,  OutlookAppointment appItem)
		{
			ITransferDataSerializable serializer = new AppointmentSerializer(appItem);
			serializer.Deserialize(transferData);
			foreach (SyncTransferData child in transferData.Childrens)
			{
				if (child.SyncDataName == RecurrencePatternTransferData.DataName)
				{
					OutlookRecurrencePattern rPattern = appItem.GetRecurrencePattern();
					serializer = new RecurrencePatternSerializer(rPattern);
					if (serializer != null)
					{
						serializer.Deserialize(child);
					}
				}
				else if (child.SyncDataName == RecipientTransferData.DataName)
				{
					serializer = new RecipientSerializer();
					string recipientName = (string)serializer.Deserialize(child);
					if (string.IsNullOrEmpty(recipientName))
					{
						OutlookRecipient recipient = appItem.AddRecipient(recipientName);
					}
				}
			}

			//Сохранем outlook appointment
			appItem.Save();
			//Переносим exception
			if (appItem.IsRecurring)
			{
				OutlookRecurrencePattern rPattern = appItem.GetRecurrencePattern();
				foreach (SyncTransferData child in transferData.Childrens)
				{
					AppointmentTransferData exception = child as AppointmentTransferData;
					if (exception != null)
					{
						OutlookAppointment appException = rPattern.GetOccurrence(exception.RecurrenceId);
						TransferData2AppointmentItem(child, appException);
						if (exception.DeletedException)
						{
							appException.Delete();
						}
						
					}
				}
			}
			return appItem;
		}
		/// <summary>
		/// Closes the meta data store.
		/// </summary>
		protected override void CloseMetaDataStore()
		{
			if (_metaDataStore != null)
			{
				((SqlMetadataStore)_metaDataStore).Dispose();
			}
			_metaDataStore = null;
		}

		protected override void SaveDataStore()
		{
			//Nothing todo
		}

		protected override void ReadDataStore()
		{
			DebugAssistant.Log("Entering ReadDataStore()");

			_localItems.Clear();

			foreach (OutlookItem outlookItem in OutlookMapiFolder.Items)
			{
				OutlookAppointment appItem = outlookItem as OutlookAppointment;
				if (appItem != null)
				{
					try
					{
						SyncTransferData transferData = Appointment2TransferData(appItem);
						_localItems.Add(transferData);
					}
					catch (Exception e)
					{
						DebugAssistant.Log(DebugSeverity.Error | DebugSeverity.MessageBox, e.Message);
						throw;
					}
				}
			}

			DebugAssistant.Log("Leaving ReadDataStore(), localLastModificationTimesUtc.Count = " + _localItems.Count);
        
		}

		protected override void UpdateMetadataStoreWithLocalChanges()
		{
			SyncVersion newVersion = new SyncVersion(0, _metaData.GetNextTickCount());
			_metaData.DeleteDetector.MarkAllItemsUnreported();
			foreach (SyncTransferData data in _localItems)
			{
				//Look up an item's metadata by its ID... 
				ItemMetadata item = _metaData.FindItemMetadataByUniqueIndexedField(URI_COLUMNNAME, data.Uri);
				
				if (item == null)
				{
					//New item, must have been created since that last time the metadata was updated.
					//Create the item metadata required for sync (giving it a SyncID and a version, defined to be a DWORD and a ULONGLONG
					//For creates, simply provide the relative replica ID (0) and the tick count for the provider (ever increasing)
					SyncGlobalId globalId = new SyncGlobalId(0, Guid.NewGuid());
					item = _metaData.CreateItemMetadata(new SyncId(globalId), newVersion);
					item.ChangeVersion = newVersion;
					item.SetCustomField(URI_COLUMNNAME, data.Uri);
					SaveItemMetadata(item);
					//Increment sync item counter need for reporting progress info
					base.RemainingSessionWorkEstimate++;
				}
				else
				{
					if (data.LastModified > item.GetUInt64Field(TIMESTAMP_COLUMNNAME)) // the item has changed since the last sync operation.
					{
						//Changed Item, this item has changed since the last time the metadata was updated.
						//Assign a new version by simply stating "who" modified this item (0 = local/me) and "when" (tick count for the store)
						item.ChangeVersion = newVersion;
						SaveItemMetadata(item);
						//Increment sync item counter need for reporting progress info
						base.RemainingSessionWorkEstimate++;
					}
					else
					{
						//Unchanged item, nothing has changes so just mark it as live so that the metadata knows it has not been deleted.
						_metaData.DeleteDetector.ReportLiveItemById(item.GlobalId);
					}
				}
			}

			//Now go back through the items that are no longer in the store and mark them as deleted in the metadata.  
			//This sets the item as a tombstone.
			foreach (ItemMetadata item in _metaData.DeleteDetector.FindUnreportedItems())
			{
				item.MarkAsDeleted(newVersion);
				SaveItemMetadata(item); 
			}
		}

		protected override void CreateDataItem(ItemChange change, ItemMetadata item, SyncTransferData data)
		{
			OutlookAppointment appItem = OutlookMapiFolder.AddItem(Outlook.OlItemType.olAppointmentItem) as OutlookAppointment;
			if (appItem == null)
				throw new InvalidCastException("OutlookAppointment");

			//Сохраним чтобы у appointment появился globalUID
			appItem = TransferData2AppointmentItem(data, appItem);
			item.SetCustomField(URI_COLUMNNAME, appItem.EntryID);

		}

		protected override void UpdateDataItem(ItemChange change, ItemMetadata item, SyncTransferData data)
		{
			OutlookAppointment appItem = FindAppointment(item);
			if (appItem == null)
			{
				throw new System.Exception("Appointment for update not found");
			}
			//Удаляем рекурсию
			appItem.ClearRecurrencePattern();
			//Удаляем всех участников
			while (appItem.Recipients.Count() != 0)
			{
				appItem.RemoveRecipient(1);
			}

			appItem = TransferData2AppointmentItem(data, appItem);
		}

		protected override bool DeleteDataItem(ItemChange change, ItemMetadata item, SyncTransferData data)
		{
			OutlookAppointment appItem = FindAppointment(item);
			if (appItem == null)
			{
				throw new System.Exception("Appointment for delete not found");
			}
			appItem.Delete();
			return true;
		}

		protected override SyncTransferData MergeDataItem(SyncTransferData dataItem, SyncTransferData otherDataItem)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Возвращает результат поиска в кеше или в папке outlook объект SyncTransferData имеющий соответственный uri.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		protected override SyncTransferData GetDataItem(ItemMetadata item)
		{
			SyncTransferData retVal = null;
			string uri = item.GetStringField(URI_COLUMNNAME);
			foreach(SyncTransferData data in _localItems)
			{
				if (data.Uri == uri)
				{
					retVal = data;
					break;
				}
			}

			if (retVal == null)
			{
				OutlookAppointment appItem = FindAppointment(item);
				if (appItem == null)
				{
					throw new System.Exception("appointment not found");
				}
				retVal = Appointment2TransferData(appItem);
			}
			return retVal;
		}

		protected override uint SyncBatchSize
		{
			get { return 50; }
		}


		/// <summary>
		/// Converts from transfer data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		protected override SyncTransferData ConvertFromTransferData(object data)
		{
			//Данный провайдер работает с SyncTransferData значит никаких преобразований не требуется
			SyncTransferData retVal = data as SyncTransferData;
			return retVal;
		}

		/// <summary>
		/// Converts to transfer data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		protected override object ConvertToTransferData(SyncTransferData data)
		{
			//Данный провайдер работает с SyncTransferData значит никаких преобразований не требуется
			return data;
		}

		public override SyncIdFormatGroup IdFormats
		{
			get
			{
				if (_idFormats == null)
				{
					_idFormats = new SyncIdFormatGroup();

					// 1 byte change unit id 

					_idFormats.ChangeUnitIdFormat.IsVariableLength = false;
					_idFormats.ChangeUnitIdFormat.Length = 1;

					// Guid replica id

					_idFormats.ReplicaIdFormat.IsVariableLength = false;
					_idFormats.ReplicaIdFormat.Length = 16;

					// Sync id for item ids

					_idFormats.ItemIdFormat.IsVariableLength = false;
					_idFormats.ItemIdFormat.Length = 24;
				}

				return _idFormats;
			}
		}

		/// <summary>
		/// Сохраняет метаданные
		/// </summary>
		/// <param name="item">The item.</param>
		protected override void SaveItemMetadata(ItemMetadata item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
					
			if (item.IsDeleted)
			{
				// set timestamp to 0 for tombstones
				item.SetCustomField(TIMESTAMP_COLUMNNAME, (ulong)0);
			}
			else
			{
				SyncTransferData data = GetDataItem(item);
				if (data == null)
					throw new Exception("data object for itemMetadata not found");

				item.SetCustomField(TIMESTAMP_COLUMNNAME, data.LastModified);
			}

			//Сохраняем  в хранилище метаданных
			_metaData.SaveItemMetadata(item);

		}

		/// <summary>
		/// Finds the appointment.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		private OutlookAppointment FindAppointment(ItemMetadata item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			string uri = item.GetStringField(URI_COLUMNNAME);
			if (string.IsNullOrEmpty(uri))
			{
				throw new System.Exception("uri not specified in metadata item");
			}
			return FindAppointment(uri);
		}

		/// <summary>
		/// Finds the appointment.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <returns></returns>
		public OutlookAppointment FindAppointment(string uri)
		{
			OutlookAppointment retVal = null;
			foreach(OutlookItem outlookItem in OutlookMapiFolder.Items)
			{
				OutlookAppointment appItem = outlookItem as OutlookAppointment;
				if (appItem != null)
				{
					if (appItem.EntryID == uri)
					{
						retVal = appItem;
						break;
					}
				}
			}
			return retVal;
		}

		#region ISyncProviderSetting Members

		public ConflictResolutionPolicy ConflictResolutionPolicySetting
		{
			get { return (ConflictResolutionPolicy)_activeSyncSetting.syncConflictResolution; }
		}

		public SyncDirectionOrder SyncDirectionOrderSetting
		{
			get { return (SyncDirectionOrder)_activeSyncSetting.syncDirection; }
		}

		#endregion
	}
}
