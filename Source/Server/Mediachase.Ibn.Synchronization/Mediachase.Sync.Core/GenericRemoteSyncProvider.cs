using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Synchronization;

namespace Mediachase.Sync.Core
{
	public abstract class GenericRemoteSyncProvider<T> : GenericSyncProvider<T> where T : class 
	{
		#region Remote provider methods

        public virtual byte[] ProcessRemoteChangeBatch(
            ConflictResolutionPolicy resolutionPolicy, 
            ChangeBatch sourceChanges,
            object changeDataRetriever, 
            byte[] changeApplierInfo)
        {
			BeginTransaction();

			//Get all my local change versions from the metadata store
			IEnumerable<ItemChange> localChanges = _metaData.GetLocalVersions(sourceChanges);

			//Create a changeapplier object to make change application easier (make the engine call me 
			//when it needs data and when I should save data)
			NotifyingChangeApplier changeApplier = new NotifyingChangeApplier(IdFormats);

			// The following step is required because we are remote change application
			changeApplier.LoadChangeApplierInfo(changeApplierInfo);

			changeApplier.ApplyChanges(
				 resolutionPolicy,
				 sourceChanges,
				 changeDataRetriever as IChangeDataRetriever,
				 localChanges,
				 base._metaData.GetKnowledge(),
				 base._metaData.GetForgottenKnowledge(),
				 this,
				 null,                     // Note that we do not pass a sync session context
				 new SyncCallbacks());                    

			CommitTransaction();

			// Return the ChangeApplierInfo
			return changeApplier.GetChangeApplierInfo();
      
        }

        //If full enumeration is needed because  this provider is out of date due to tombstone cleanup, then this method will be called by the engine.
        public virtual byte[] ProcessRemoteFullEnumerationChangeBatch(
            ConflictResolutionPolicy resolutionPolicy, 
            FullEnumerationChangeBatch sourceChanges,
            object changeDataRetriever,
            byte[] changeApplierInfo)
        {

			BeginTransaction();

			//Get all my local change versions from the metadata store
			IEnumerable<ItemChange> localChanges = _metaData.GetFullEnumerationLocalVersions(sourceChanges);

			//Create a changeapplier object to make change application easier (make the engine call me 
			//when it needs data and when I should save data)
			NotifyingChangeApplier changeApplier = new NotifyingChangeApplier(this.IdFormats);

			// The following step is required because we are remote change application
			changeApplier.LoadChangeApplierInfo(changeApplierInfo);

			changeApplier.ApplyFullEnumerationChanges(
				 resolutionPolicy,
				sourceChanges,
				 changeDataRetriever as IChangeDataRetriever,
				 localChanges,
				 _metaData.GetKnowledge(),
				 _metaData.GetForgottenKnowledge(),
				 this,
				 null,                   // Note that we do not pass a sync session context
				 new SyncCallbacks());

			CommitTransaction();
			// Return the ChangeApplierInfo
			return changeApplier.GetChangeApplierInfo();
        }
        
		#endregion
	}
}
