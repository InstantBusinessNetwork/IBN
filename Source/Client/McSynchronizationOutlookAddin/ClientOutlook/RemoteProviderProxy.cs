using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Synchronization;
using Mediachase.Sync.Core;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using OutlookAddin.SyncService;
using Mediachase.Sync.Core.Common;
using System.Web.Services.Protocols;

namespace Mediachase.ClientOutlook.WebService
{
	/// <summary>
	/// Представляет тип заместитель для работы с удаленным провайдером синхронизации посредством вызова методов web сервиса
	/// </summary>
	public class RemoteProviderProxy : KnowledgeSyncProvider
	{
		
		private SynchronizationService _syncService;
		private SyncSessionContext _syncSessionContext;
		private SyncIdFormatGroup _idFormats;

		private RemoteProviderProxy(SynchronizationService syncService)
		{
			_syncService = syncService;
		}

		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <param name="appSetting">The app setting.</param>
		/// <param name="oItemType">Type of the o item.</param>
		/// <returns></returns>
		public static RemoteProviderProxy CreateInstance(syncAppSetting appSetting, Outlook.OlItemType oItemType)
		{
			RemoteProviderProxy retVal = null;
			NetworkCredential cred = new NetworkCredential();
			cred.UserName = appSetting.ibnPortalLogin;
			cred.Password = appSetting.ibnPortalPassword;
			SynchronizationService syncService = new SynchronizationService();
			//enable session
			syncService.CookieContainer = new System.Net.CookieContainer();

			string syncServiceUrl = appSetting.ibnPortalUrl;
			if (!syncServiceUrl.Contains(OutlookAddin.Resources.System_IbnSyncWebServicePath))
			{
				Uri url = new Uri(syncServiceUrl);
				UriBuilder uriBuilder = new UriBuilder();
				uriBuilder.Scheme = Uri.UriSchemeHttp;
				uriBuilder.Port = url.Port;
				uriBuilder.Host = url.Host;
				uriBuilder.Path = url.AbsolutePath + OutlookAddin.Resources.System_IbnSyncWebServicePath;
				syncServiceUrl = uriBuilder.ToString();
			}

			syncService.Url = syncServiceUrl;
			//Authentificate
			syncService.SetCredentials(cred);
			//Set sync provider
			syncService.SetProviderTypeForSyncSession((OutlookAddin.SyncService.eSyncProviderType)oItemType);
			retVal = new RemoteProviderProxy(syncService);


			return retVal;
		}

		/// <summary>
		/// When overridden in a derived class, notifies the provider that it is joining a synchronization session.
		/// </summary>
		/// <param name="position">The position of this provider, relative to the other provider in the session.</param>
		/// <param name="syncSessionContext">The current status of the corresponding session.</param>
		public override void BeginSession(SyncProviderPosition position, SyncSessionContext syncSessionContext)
		{
			if (_syncService == null)
				throw new Exception("Not initalized");
			
			_syncSessionContext = syncSessionContext;
			_syncService.BeginSession();
		}

		/// <summary>
		/// When overridden in a derived class, notifies the provider that a synchronization session to which it was enlisted has completed.
		/// </summary>
		/// <param name="syncSessionContext">The current status of the corresponding session.</param>
		public override void EndSession(SyncSessionContext syncSessionContext)
		{
			_syncSessionContext = null;
			_syncService.EndSession();
		}

		/// <summary>
		/// When overridden in a derived class, gets a change batch that contains item metadata for items that are not contained in the specified knowledge from the destination provider.
		/// </summary>
		/// <param name="batchSize">The number of changes to include in the change batch.</param>
		/// <param name="destinationKnowledge">The knowledge from the destination provider. This knowledge must be mapped by calling <see cref="M:Microsoft.Synchronization.SyncKnowledge.MapRemoteKnowledgeToLocal(Microsoft.Synchronization.SyncKnowledge)"/> on the source knowledge before it can be used for change enumeration.</param>
		/// <param name="changeDataRetriever">Returns an object that can be used to retrieve change data. It can be an <see cref="T:Microsoft.Synchronization.IChangeDataRetriever"/> object or a provider-specific object.</param>
		/// <returns>
		/// A change batch that contains item metadata for items that are not contained in the specified knowledge from the destination provider. Cannot be a null.
		/// </returns>
		public override ChangeBatch GetChangeBatch(uint batchSize, SyncKnowledge destinationKnowledge, out object changeDataRetriever)
		{
			byte[] rawDestinationKnowledge = destinationKnowledge.Serialize();
			byte[] rawChangeDataRetriever;
			byte[] rawChangeBatch = _syncService.GetChangeBatch(batchSize, rawDestinationKnowledge,
																out rawChangeDataRetriever);
			CachedChangeDataRetriever cachedRetriever = 
							SerializerHelper.BinaryDeserialize<CachedChangeDataRetriever>(rawChangeDataRetriever);
			changeDataRetriever = cachedRetriever;

			return ChangeBatch.Deserialize(IdFormats, rawChangeBatch);
		}

		/// <summary>
		/// When overridden in a derived class, gets a change batch that contains item metadata for items that have IDs greater than the specified lower bound, as part of a full enumeration.
		/// </summary>
		/// <param name="batchSize">The number of changes to include in the change batch.</param>
		/// <param name="lowerEnumerationBound">The lower bound for item IDs. This method returns changes that have IDs greater than or equal to this ID value.</param>
		/// <param name="knowledgeForDataRetrieval">If an item change is contained in this knowledge object, data for that item already exists on the destination replica.</param>
		/// <param name="changeDataRetriever">Returns an object that can be used to retrieve change data. It can be an <see cref="T:Microsoft.Synchronization.IChangeDataRetriever"/> object or a be provider-specific object.</param>
		/// <returns>
		/// A change batch that contains item metadata for items that have IDs greater than the specified lower bound, as part of a full enumeration.
		/// </returns>
		public override FullEnumerationChangeBatch GetFullEnumerationChangeBatch(uint batchSize, SyncId lowerEnumerationBound, SyncKnowledge knowledgeForDataRetrieval, out object changeDataRetriever)
		{
			byte[] rawLowerEnumBound = lowerEnumerationBound.RawId;
			byte[] rawKnowledgeForDataRetrieval = knowledgeForDataRetrieval.Serialize();
			byte[] rawChangeDataRetriever;
			byte[] rawFullEnumerationChangeBatch = 
									_syncService.GetFullEnumerationChangeBatch(batchSize,
																			   rawLowerEnumBound,
																			   rawKnowledgeForDataRetrieval,
																			   out rawChangeDataRetriever);
			CachedChangeDataRetriever cachedRetriever =
						SerializerHelper.BinaryDeserialize<CachedChangeDataRetriever>(rawChangeDataRetriever);
			changeDataRetriever = cachedRetriever;
			return FullEnumerationChangeBatch.Deserialize(IdFormats, rawFullEnumerationChangeBatch);
		}

		/// <summary>
		/// When overridden in a derived class, gets the number of item changes that will be included in change batches, and the current knowledge for the synchronization scope.
		/// </summary>
		/// <param name="batchSize">The number of item changes that will be included in change batches returned by this object.</param>
		/// <param name="knowledge">The current knowledge for the synchronization scope, or a newly created knowledge object if no current knowledge exists.</param>
		public override void GetSyncBatchParameters(out uint batchSize, out SyncKnowledge knowledge)
		{
			byte[] rawKnowledge;
			batchSize = _syncService.GetSyncBatchParameters(out rawKnowledge);
			knowledge = SyncKnowledge.Deserialize(IdFormats, rawKnowledge);
		}

		/// <summary>
		/// When overridden in a derived class, gets the ID format schema of the provider.
		/// </summary>
		/// <value></value>
		/// <returns>The ID format schema of the provider.</returns>
		public override SyncIdFormatGroup IdFormats
		{
			get 
			{
				if (_idFormats == null)
				{
					MemoryStream rawIdFormatStream = new MemoryStream(_syncService.GetIdFormats());
					BinaryFormatter formatter = new BinaryFormatter();
					_idFormats =  (SyncIdFormatGroup)formatter.Deserialize(rawIdFormatStream);
				}

				return _idFormats;
			}
		}

		/// <summary>
		/// When overridden in a derived class, processes a set of changes by detecting conflicts and applying changes to the item store.
		/// </summary>
		/// <param name="resolutionPolicy">The conflict resolution policy to use when this method applies changes.</param>
		/// <param name="sourceChanges">A batch of changes from the source provider to be applied locally.</param>
		/// <param name="changeDataRetriever">An object that can be used to retrieve change data. It can be an <see cref="T:Microsoft.Synchronization.IChangeDataRetriever"/> object or a provider-specific object.</param>
		/// <param name="syncCallbacks">An object that receives event notifications during change application.</param>
		/// <param name="sessionStatistics">Tracks change statistics. For a provider that uses custom change application, this object must be updated with the results of the change application.</param>
		public override void ProcessChangeBatch(ConflictResolutionPolicy resolutionPolicy, ChangeBatch sourceChanges, object changeDataRetriever, SyncCallbacks syncCallbacks, SyncSessionStatistics sessionStatistics)
		{
			CachedChangeDataRetriever cachedChangeDataRetriever = new CachedChangeDataRetriever(
				   changeDataRetriever as IChangeDataRetriever,
				   sourceChanges);
			byte[] rawSourceChanges = sourceChanges.Serialize();
			byte[] rawCachedChangeDataRetriever = SerializerHelper.BinarySerialize(cachedChangeDataRetriever);
			byte[] newChangeApplierInfo = _syncService.ProcessChangeBatch(
				(int)resolutionPolicy,
				rawSourceChanges,
				rawCachedChangeDataRetriever,
				_syncSessionContext.ChangeApplierInfo);

			_syncSessionContext.ChangeApplierInfo = newChangeApplierInfo;
		}

		/// <summary>
		/// When overridden in a derived class, processes a set of changes for a full enumeration by applying changes to the item store.
		/// </summary>
		/// <param name="resolutionPolicy">The conflict resolution policy to use when this method applies changes.</param>
		/// <param name="sourceChanges">A batch of changes from the source provider to be applied locally.</param>
		/// <param name="changeDataRetriever">An object that can be used to retrieve change data. It can be an <see cref="T:Microsoft.Synchronization.IChangeDataRetriever"/> object or a provider-specific object.</param>
		/// <param name="syncCallbacks">An object that receives event notifications during change application.</param>
		/// <param name="sessionStatistics">Tracks change statistics. For a provider that uses custom change application, this object must be updated with the results of the change application.</param>
		public override void ProcessFullEnumerationChangeBatch(ConflictResolutionPolicy resolutionPolicy, FullEnumerationChangeBatch sourceChanges, object changeDataRetriever, SyncCallbacks syncCallbacks, SyncSessionStatistics sessionStatistics)
		{
			CachedChangeDataRetriever cachedChangeDataRetriever = new CachedChangeDataRetriever(
			  changeDataRetriever as IChangeDataRetriever,
			  sourceChanges);

			byte[] rawSourceChanges = sourceChanges.Serialize();
			byte[] rawCachedChangeDataRetriever = SerializerHelper.BinarySerialize(cachedChangeDataRetriever);
			byte[] newChangeApplierInfo = _syncService.ProcessFullEnumerationChangeBatch(
				(int)resolutionPolicy,
				rawSourceChanges,
				rawCachedChangeDataRetriever,
				_syncSessionContext.ChangeApplierInfo);

			_syncSessionContext.ChangeApplierInfo = newChangeApplierInfo;
		}
	}
}
