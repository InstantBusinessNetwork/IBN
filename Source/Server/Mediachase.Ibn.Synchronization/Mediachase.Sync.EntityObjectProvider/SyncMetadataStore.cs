using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Sync.Core;
using Microsoft.Synchronization;
using Microsoft.Synchronization.MetadataStorage;
using Mediachase.Ibn.Data;
using Mediachase.Sync.EntityObjectProvider.Sql;

namespace Mediachase.Sync.EntityObjectProvider
{
	public class SyncMetadataStore : MetadataStore, IDisposable
	{
		private bool _disposed = false;
		private TransactionScope _tran;

		private static object _lock = new object();

		private ReplicaMetadataAdaptor _replicaMetaData;

		/// <summary>
		/// Кеш метаданных реплики. В многопотоковой среде вся работа будет вестись с одним экземпляром метаданых реплики.
		/// </summary>
		private static Dictionary<SyncId, ReplicaMetadataAdaptor> _replicaCache = new Dictionary<SyncId, ReplicaMetadataAdaptor>();

		private SyncMetadataStore(ReplicaMetadataAdaptor replica)
		{
			_replicaMetaData = replica;
		}

		public static SyncIdFormatGroup StaticIdFormats
		{
			get
			{
				SyncIdFormatGroup retVal = new SyncIdFormatGroup();
				// 1 byte change unit id 

				retVal.ChangeUnitIdFormat.IsVariableLength = false;
				retVal.ChangeUnitIdFormat.Length = 1;

				// Guid replica id

				retVal.ReplicaIdFormat.IsVariableLength = false;
				retVal.ReplicaIdFormat.Length = 16;

				// Sync id for item ids

				retVal.ItemIdFormat.IsVariableLength = false;
				retVal.ItemIdFormat.Length = 24;

				return retVal;
			}
		}

		/// <summary>
		/// Создает хранилище метаданных реплики
		/// </summary>
		/// <param name="replicaId">The replica id.</param>
		/// <param name="idFormats">The id formats.</param>
		/// <returns></returns>
		public static SyncMetadataStore CreateStore(SyncId replicaId, int principalId)
		{
			SyncMetadataStore retVal = null;
			ReplicaMetadataAdaptor replica = ReplicaMetadataAdaptor.CreateInstance(replicaId, StaticIdFormats);
			if (replica != null)
			{
				replica.PrincipalId = principalId;
				replica.Save();
			}
			retVal = OpenStore(replicaId);
			return retVal;
		}

		/// <summary>
		/// Возвращает id реплики для определенного пользователя
		/// </summary>
		/// <param name="principalId">The principal id.</param>
		/// <returns></returns>
		public static SyncId FindReplicaIdByPrincipalId(int principalId)
		{
			SyncId retVal = null;
			ReplicaMetadataAdaptor replicaMetaData = ReplicaMetadataAdaptor.CreateInstance(principalId);
			if (replicaMetaData != null)
			{
				retVal = replicaMetaData.ReplicaId;
			}

			return retVal;
		}

		/// <summary>
		/// Возвращает только кешированные хранилища метаданнх для определенной реплики 
		/// </summary>
		/// <param name="replicaId">The replica id.</param>
		/// <returns></returns>
		public static SyncMetadataStore OpenStore(SyncId replicaId)
		{
			SyncMetadataStore retVal = null;
			try
			{
				lock (_lock)
				{
					ReplicaMetadataAdaptor replica;
					if (!_replicaCache.TryGetValue(replicaId, out replica))
					{
						replica = ReplicaMetadataAdaptor.CreateInstance(replicaId);
						if (replica != null)
						{
							_replicaCache.Add(replicaId, replica);
						}
					}

					retVal = new SyncMetadataStore(replica);
				}
			}
			catch (ArgumentException)
			{
			}
			return retVal;
		}

		public override ReplicaMetadata GetReplicaMetadata(SyncIdFormatGroup idFormats, SyncId replicaId)
		{
			SyncReplicaMetadata retVal = new SyncReplicaMetadata(idFormats, replicaId, _replicaMetaData);
			
			return retVal;
		}

		public override ReplicaMetadata InitializeReplicaMetadata(SyncIdFormatGroup idFormats, SyncId replicaId, IEnumerable<FieldSchema> customItemFieldSchemas,
															  IEnumerable<IndexSchema> customIndexedFieldSchemas)
		{
			if (_replicaMetaData == null)
			{
				throw new NullReferenceException("not initialized");
			}

			return GetReplicaMetadata(idFormats, replicaId);

		}

		public void BeginTransaction()
		{
			BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
		}

		public override void BeginTransaction(System.Data.IsolationLevel isolationLevel)
		{
			_tran = DataContext.Current.BeginTransaction();
		}

		public override void CommitTransaction()
		{
			_tran.Commit();
		}
		public override void RollbackTransaction()
		{
			_tran.Dispose();
			_tran = null;
		}

		

	
		public override bool IsTransactionActive(out System.Data.IsolationLevel isolationLevel)
		{
			isolationLevel = System.Data.IsolationLevel.ReadCommitted;
			return _tran != null;
		}


		#region IDisposable Members
		// Dispose(bool disposing) executes in two distinct scenarios.
		// If disposing equals true, the method has been called directly
		// or indirectly by a user's code. Managed and unmanaged resources
		// can be disposed.
		// If disposing equals false, the method has been called by the
		// runtime from inside the finalizer and you should not reference
		// other objects. Only unmanaged resources can be disposed.
		protected void Dispose(bool disposing)
		{
			// Check to see if Dispose has already been called.
			if (!_disposed)
			{
				// If disposing equals true, dispose all managed
				// and unmanaged resources.
				if (disposing)
				{
					// Dispose managed resources.
					if (_tran != null)
					{
						_tran.Dispose();
						_tran = null;
					}
				}

				// Call the appropriate methods to clean up
				// unmanaged resources here.
				// If disposing is false,
				// only the following code is executed.
			
				// Note disposing has been done.
				_disposed = true;

			}
		}

		public void Dispose()
		{
			Dispose(true);
			// This object will be cleaned up by the Dispose method.
			// Therefore, you should call GC.SupressFinalize to
			// take this object off the finalization queue
			// and prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);

		}


		#endregion

		public override ReplicaMetadata GetSingleReplicaMetadata()
		{
			throw new NotImplementedException();
		}

		public override void RemoveReplicaMetadata(SyncIdFormatGroup idFormats, SyncId replicaId)
		{
			throw new NotImplementedException();
		}
	}
}
