using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Synchronization;
using System.Threading;
using Mediachase.ClientOutlook.Configuration;
using Mediachase.Sync.Core;
using Mediachase.ClientOutlook.WebService;
using Mediachase.ClientOutlook.TestProvider;
using OutlookAddin;
using System.Runtime.InteropServices;
using OutlookAddin.OutlookItemAdaptors;
using System.Web.Services.Protocols;
using Mediachase.Sync.Core.ErrorManagement;

namespace Mediachase.ClientOutlook
{

	public class SyncProcessEventArgs : EventArgs
	{
		public SyncProcessEventArgs()
		{
		}
	}

	/// <summary>
	/// Представляет класс приложения синхронизации
	/// </summary>
	public class SyncApplication
	{
		private object _lockObject = new object();
		private bool _syncInProcess = false;
		private Outlook.OlItemType? _curProcessedSyncType = null;
		OutlookApplication _outlookApplication;

		SyncOrchestrator _syncAgent;
		SyncStatistics _syncStats;

		UserProfileSetting _settings;

		private ThreadedWorkManager<Outlook.OlItemType> _workManager;
		private static SyncApplication _instance;

		private Dictionary<Outlook.OlItemType, KnowledgeSyncProvider> _syncProviderCache = 
														new Dictionary<Outlook.OlItemType, KnowledgeSyncProvider>();

		private List<ItemChange> _skippedItems = new List<ItemChange>();
		public bool LastSyncErrorOccur { get; set; }
		public string LastSyncErrorDescr { get; set; }
		public IEnumerable<ItemChange> SkippedItems 
		 {
			 get { return _skippedItems; }
		 }
		#region Events
		#region Custom event
		//custom events
		public event EventHandler<SyncProcessEventArgs> SyncProcessBeginEvent;
		public event EventHandler<SyncProcessEventArgs> SyncProcessEndEvent;

		protected virtual void OnSyncProcessBegin(SyncProcessEventArgs args)
		{
			EventHandler<SyncProcessEventArgs> tmp = SyncProcessBeginEvent;
			if (tmp != null)
			{
				tmp(this, args);
			}
		}

		protected virtual void OnSyncProcessEnd(SyncProcessEventArgs args)
		{
			EventHandler<SyncProcessEventArgs> tmp = SyncProcessEndEvent;
			if (tmp != null)
			{
				tmp(this, args);
			}
		}
		#endregion
		#region Sync session event
		//Sync session events
		public event EventHandler<SyncStagedProgressEventArgs> SessionProgress;
		public event EventHandler<SyncOrchestratorStateChangedEventArgs> SessionStateChanged;
		protected virtual void OnSessionProgress(object sender, SyncStagedProgressEventArgs args)
		{
			EventHandler<SyncStagedProgressEventArgs> tmp = SessionProgress;
			if (tmp != null)
			{
				tmp(this, args);
			}
		}

		protected virtual void OnSessionStateChanged(object sender, SyncOrchestratorStateChangedEventArgs args)
		{
			EventHandler<SyncOrchestratorStateChangedEventArgs> tmp = SessionStateChanged;
			if (tmp != null)
			{
				tmp(this, args);
			}
		}
		#endregion
		#region Sync destination provider event
		//Sync destination provider events
		//     Occurs when the forgotten knowledge from the source provider is not contained
		//     in the current knowledge of the destination provider.
		public event EventHandler<FullEnumerationNeededEventArgs> FullEnumerationNeeded;

		//     Occurs when a synchronization provider sets a recoverable error while an
		//     item is being loaded or saved.
		public event EventHandler<ItemChangeSkippedEventArgs> ItemChangeSkipped;

		//     Occurs before a change is applied.
		public event EventHandler<ItemChangingEventArgs> ItemChanging;

		//     Occurs when a conflict is detected when the conflict resolution policy is
		//     set to Microsoft.Synchronization.ConflictResolutionPolicy.ApplicationDefined.
		public event EventHandler<ItemConflictingEventArgs> ItemConflicting;

		//     Occurs periodically during the synchronization session to report progress.
		public event EventHandler<SyncStagedProgressEventArgs> ProgressChanged;

		protected virtual void OnFullEnumerationNeeded(object sender, FullEnumerationNeededEventArgs args)
		{
			EventHandler<FullEnumerationNeededEventArgs> tmp = FullEnumerationNeeded;
			if (tmp != null)
			{
				tmp(this, args);
			}
		}

		protected virtual void OnItemChangeSkipped(object sender, ItemChangeSkippedEventArgs args)
		{
			EventHandler<ItemChangeSkippedEventArgs> tmp = ItemChangeSkipped;
			if (tmp != null)
			{
				_skippedItems.Add(args.ItemChange);
				tmp(this, args);
			}
		}

		protected virtual void OnItemChanging(object sender, ItemChangingEventArgs args)
		{
			EventHandler<ItemChangingEventArgs> tmp = ItemChanging;
			if (tmp != null)
			{
				tmp(this, args);
			}
		}

		protected virtual void OnItemConflicting(object sender, ItemConflictingEventArgs args)
		{
			EventHandler<ItemConflictingEventArgs> tmp = ItemConflicting;
			if (tmp != null)
			{
				tmp(this, args);
			}

		}

		protected virtual void OnProgressChanged(object sender, SyncStagedProgressEventArgs args)
		{
			EventHandler<SyncStagedProgressEventArgs> tmp = ProgressChanged;
			if (tmp != null)
			{
				tmp(this, args);
			}

		}

		#endregion
		#endregion

		private SyncApplication(OutlookApplication outlookApplication, UserProfileSetting settings)
		{
			_outlookApplication = outlookApplication;
			_settings = settings;
			_workManager = new ThreadedWorkManager<Outlook.OlItemType>(this.DoSync);
		}

		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <param name="oApp">The o app.</param>
		/// <returns></returns>
		public static SyncApplication CreateInstance(OutlookApplication outlookApplication)
		{
			SyncApplication retVal = _instance;
			if (retVal == null)
			{
				UserProfileSetting settings = UserProfileSetting.LoadActiveProfile();

				if (settings != null)
				{
					retVal = _instance = new SyncApplication(outlookApplication, settings);
				}
			}
			return retVal;
		}

		/// <summary>
		/// Adds the syncronize task.
		/// </summary>
		/// <param name="oSyncItem">The o sync item.</param>
		public void SheduleSyncronizeTask(Outlook.OlItemType oSyncItem)
		{
			this._workManager.SheduleTask(oSyncItem);
			//DoSync(oSyncItem);
		}

		public bool SyncInProgress
		{
			get	{ return _syncInProcess; }
		}

		public Outlook.OlItemType? CurrentProcessedSyncType
		{
			get	{	return _curProcessedSyncType;	}
			private set { _curProcessedSyncType = value; }
		}

		/// <summary>
		/// Does the sync. Выполняется в другом потоке отличном от AddinModule
		/// </summary>
		/// <param name="oItemType">Type of the o item.</param>
		private void DoSync(Outlook.OlItemType oItemType)
		{
			//reset last error 
			LastSyncErrorDescr = string.Empty;
			LastSyncErrorOccur = false;
			//reset skipped items
			_skippedItems.Clear();

			KnowledgeSyncProvider localProvider = null;
			KnowledgeSyncProvider remoteProvider = null;
			CurrentProcessedSyncType = oItemType;
			try
			{
				remoteProvider = GetRemoteSyncProvidersBySyncType(oItemType);
				localProvider = GetLocalSyncProviderBySyncType(oItemType);

				if (localProvider != null)
				{
					//Create sync session
					if (_syncAgent == null)
					{
						_syncAgent = new SyncOrchestrator();
						//Subscribe sync framework events
						SubscribeEvents(_syncAgent);
					}

					//ISyncProviderSetting providerSetting = localProvider.ProviderSetting;
					ISyncProviderSetting providerSetting = localProvider as ISyncProviderSetting;
					if (providerSetting != null)
					{

						SyncDirectionOrder direction = providerSetting.SyncDirectionOrderSetting;
						ConflictResolutionPolicy conflictResolution = providerSetting.ConflictResolutionPolicySetting;

						remoteProvider.Configuration.ConflictResolutionPolicy = conflictResolution;
						localProvider.Configuration.ConflictResolutionPolicy = conflictResolution;

						_syncAgent.Direction = direction;
						_syncAgent.LocalProvider = localProvider;
						_syncAgent.RemoteProvider = remoteProvider;

						//Subscribe to knowledege provider events
						SubscribeEvents(localProvider);
						SubscribeEvents(remoteProvider);
						//raise sync process begin event
						OnSyncProcessBegin(new SyncProcessEventArgs());

						SyncOperationStatistics syncStats = _syncAgent.Synchronize();
						CollectStatistics(syncStats);
					}
				}
			}
			catch (UriFormatException e)
			{
				DebugAssistant.Log(DebugSeverity.Error, e.Message);
				LastSyncErrorOccur = true;
				LastSyncErrorDescr = OutlookAddin.Resources.ERR_SYNC_SERVICE_INVALID_URL;
				//DebugAssistant.Log(DebugSeverity.Error | DebugSeverity.MessageBox,
				//					OutlookAddin.Resources.ERR_SYNC_SERVICE_INVALID_URL);
			}
			catch (SoapException e)
			{
				LastSyncErrorOccur = true;
				SyncronizationServiceError syncError = SoapErrorHandler.HandleError(e);

				string msg = OutlookAddin.Resources.ERR_SYNC_SERVICE_UKNOW;
				if (syncError != null)
				{
					DebugAssistant.Log(DebugSeverity.Error, syncError.errorType.ToString() + " "
										+ syncError.message + " " + syncError.stackTrace);

					switch (syncError.errorType)
					{
						case SyncronizationServiceError.eServiceErrorType.AuthFailed:
							msg = Resources.ERR_SYNC_SERVICE_AUTH_FAILED;
							break;
						case SyncronizationServiceError.eServiceErrorType.NotAuthRequest:
							msg = Resources.ERR_SYNC_SERVICE_NOT_AUTH;
							break;
						case SyncronizationServiceError.eServiceErrorType.ProviderNotSpecified:
							msg = Resources.ERR_SYNC_SERVICE_INVALID_PROVIDER;
							break;
						case SyncronizationServiceError.eServiceErrorType.SyncFramework:
							msg = Resources.ERR_SYNC_SERVICE_FRAMEWORK;
							break;
						case SyncronizationServiceError.eServiceErrorType.SyncProvider:
							msg = Resources.ERR_SYNC_SERVICE_PROVIDER;
							break;
						case SyncronizationServiceError.eServiceErrorType.ServerError:
							msg = Resources.ERR_SYNC_SERVICE_SERVER;
							break;
						case SyncronizationServiceError.eServiceErrorType.Undef:
							msg = Resources.ERR_SYNC_SERVICE_UKNOW;
							break;
					}
				}

				LastSyncErrorDescr = msg;

				//DebugAssistant.Log(DebugSeverity.Error | DebugSeverity.MessageBox, msg);
			}
			catch (System.Net.WebException e)
			{
				LastSyncErrorOccur = true;
				LastSyncErrorDescr = Resources.ERR_SYNC_CONNECTION;
				DebugAssistant.Log(DebugSeverity.Error, e.Message);
			}
			catch (Exception e)
			{
				LastSyncErrorOccur = true;
				LastSyncErrorDescr = OutlookAddin.Resources.ERR_ADDIN_UNKNOW;
				DebugAssistant.Log(DebugSeverity.Error, e.Message);
				//DebugAssistant.Log(DebugSeverity.Error | DebugSeverity.MessageBox, OutlookAddin.Resources.ERR_ADDIN_UNKNOW);
			}
			finally
			{
				if (localProvider != null)
				{
					localProvider.EndSession(null);
				}
				if (remoteProvider != null)
				{
					remoteProvider.EndSession(null);
				}
				OnSyncProcessEnd(new SyncProcessEventArgs());
				CurrentProcessedSyncType = null;

			}
		}

		public void SyncCancel()
		{
			if (_syncAgent == null)
				throw new NullReferenceException("Sync session not started");
			if (SyncInProgress)
			{
				_syncAgent.Cancel();
			}
		}

		public SyncStatistics SyncStatistics
		{
			get
			{
				if (_syncStats == null)
				{
					_syncStats = new SyncStatistics();
				}
				return _syncStats;
			}
		}

		public UserProfileSetting CurrentSettings
		{
			get
			{
				return _settings;
			}
		}

		/// <summary>
		/// Возвращает объект настройки для соотв типа синхронизации
		/// </summary>
		/// <param name="oItemType">Type of the o item.</param>
		/// <returns></returns>
		public syncProviderSetting GetSyncProviderSettingByType(Outlook.OlItemType oItemType)
		{
			syncProviderSetting retVal = null;
			if (CurrentSettings == null)
				throw new NullReferenceException("CurrentSetting");

			switch (oItemType)
			{
				case Outlook.OlItemType.olAppointmentItem:
					retVal = CurrentSettings.CurrentSyncAppointentSetting;
					break;
				case Outlook.OlItemType.olContactItem:
					retVal = CurrentSettings.CurrentContactSetting;
					break;
				case Outlook.OlItemType.olNoteItem:
					retVal = null;
					break;
				case Outlook.OlItemType.olTaskItem:
					retVal = CurrentSettings.CurrentTaskSetting;
					break;
			}

			return retVal;
			
		}
		private KnowledgeSyncProvider GetRemoteSyncProvidersBySyncType(Outlook.OlItemType oItemType)
		{
			return  RemoteProviderProxy.CreateInstance(_settings.CurrentSyncAppSetting, oItemType);
			//return  ClientOutlook.TestProvider.MyProviderFactory.CreateBProvider() ;
		}

		private KnowledgeSyncProvider GetLocalSyncProviderBySyncType(Outlook.OlItemType oItemType)
		{
			//return new KnowledgeSyncProvider[] { ClientOutlook.TestProvider.MyProviderFactory.CreateAProvider() };
			KnowledgeSyncProvider retVal = null;
			lock (_lockObject)
			{
				if (!_syncProviderCache.TryGetValue(oItemType, out retVal))
				{
					retVal = AppointmentSyncProvider.CreateInstance(_settings.CurrentSyncAppointentSetting, _outlookApplication);
					_syncProviderCache.Add(oItemType, retVal);
				}
			}
			//TODO: Add other types
			return retVal;
		}

		#region Event handlers
	
		#region Sync destination provider event handler
		#endregion

		#endregion

		private void CollectStatistics(SyncOperationStatistics stats)
		{
			_syncStats = new SyncStatistics(stats);
		}

		private void SubscribeEvents(SyncOrchestrator syncAgent)
		{
			if (syncAgent != null)
			{
				syncAgent.SessionProgress -= OnSessionProgress;
				syncAgent.StateChanged -= OnSessionStateChanged;

				syncAgent.SessionProgress += OnSessionProgress;
				syncAgent.StateChanged += OnSessionStateChanged;
			}
		}

		private void SubscribeEvents(KnowledgeSyncProvider syncProvider)
		{
			if (syncProvider != null)
			{
				syncProvider.DestinationCallbacks.FullEnumerationNeeded -= OnFullEnumerationNeeded;
				syncProvider.DestinationCallbacks.ItemChangeSkipped -= OnItemChangeSkipped;
				syncProvider.DestinationCallbacks.ItemChanging -= OnItemChanging;
				syncProvider.DestinationCallbacks.ItemConflicting -= OnItemConflicting;
				syncProvider.DestinationCallbacks.ProgressChanged -= OnProgressChanged;

				syncProvider.DestinationCallbacks.FullEnumerationNeeded += OnFullEnumerationNeeded;
				syncProvider.DestinationCallbacks.ItemChangeSkipped += OnItemChangeSkipped;
				syncProvider.DestinationCallbacks.ItemChanging += OnItemChanging;
				syncProvider.DestinationCallbacks.ItemConflicting += OnItemConflicting;
				syncProvider.DestinationCallbacks.ProgressChanged += OnProgressChanged;

			}
		}
	}
}
