using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Synchronization;
using Mediachase.ClientOutlook;
using Mediachase.Sync.Core;
using System.Windows.Forms;
using OutlookAddin.OutlookItemAdaptors;
using Mediachase.ClientOutlook.Configuration;

namespace OutlookAddin.OutlookUI
{
	public delegate void Func();

	public class UIController
	{
		private FormSyncItem _syncItemForm;
		private FormSyncOptions _syncOptionsForm;
		private FormSyncConflictResolution _syncConflictResForm;

		private SyncApplication _syncApp;

		private AddinModule _addinModule;
		private OutlookListener _outlookListener;

		private static UIController _instance;


		private UIController(AddinModule addinModule, OutlookListener outlookListener, SyncApplication app, 
							FormSyncItem syncForm, FormSyncOptions syncOprionsForm,	 FormSyncConflictResolution syncConflictResForm)
		{
			_addinModule = addinModule;
			_outlookListener = outlookListener;
			_syncApp = app;
			_syncItemForm = syncForm;
			_syncOptionsForm = syncOprionsForm;
			_syncConflictResForm = syncConflictResForm;
		}

		public static UIController GetInstance()
		{
			return _instance;
		}

		//this method must call in Addin thread
		public static UIController CreateInstance(AddinModule addinModule)
		{
			if (addinModule == null)
				throw new ArgumentNullException("addinModule");

			if (_instance != null)
				return _instance;
			
			//Set LogFilePath 
			DebugAssistant.LogFilePath = ApplicationConfig.LogPathFile;

			FormSyncOptions syncOptionsForm = new FormSyncOptions();
			FormSyncItem syncForm= new FormSyncItem();
			FormSyncConflictResolution syncConflictResForm = new FormSyncConflictResolution();
			OutlookListener listener = new OutlookListener(addinModule);
			IntPtr handle = listener.Handle;
			OutlookApplication outlookApplication = OutlookApplication.CreateInstance(listener);
			SyncApplication syncApp = SyncApplication.CreateInstance(outlookApplication);

			if (syncApp == null)
				throw new NullReferenceException("syncApp");

			UIController retVal = new UIController(addinModule, listener, syncApp, syncForm, 
												   syncOptionsForm, syncConflictResForm);

			if (syncConflictResForm != null)
			{
				//Force create control
				handle = syncConflictResForm.Handle;
			}

			if (syncForm != null)
			{
				//Subscribe events SyncApp(Model)
				retVal.HookEvents(syncApp);
				//Sunscribe events syncForm(View)
				retVal.HookEvents(syncForm);
				//Force create control
				handle = syncForm.Handle;

				//TODO: Нужно прочитать историю последней синхронизации и установить статус прошлой синхронизации
				syncForm.AddSyncMenuItem(Outlook.OlItemType.olAppointmentItem);
				syncForm.AddSyncMenuItem(Outlook.OlItemType.olContactItem);
				syncForm.AddSyncMenuItem(Outlook.OlItemType.olTaskItem);
				syncForm.AddSyncMenuItem(Outlook.OlItemType.olNoteItem);
				//Appointment
				SyncItemInfo status = new SyncItemInfo();
				syncAppointmentSetting apppointSetting = syncApp.CurrentSettings.CurrentSyncAppointentSetting;
				if(apppointSetting != null)
				{
					status.Status = (eSyncStatus)apppointSetting.lastSyncStatus;
					if (status.Status != eSyncStatus.Unknow)
					{
						status.LastSyncDate = new DateTime(TimeSpan.TicksPerSecond * apppointSetting.lastSyncDate);
					}
					
					if(status.Status == eSyncStatus.Ok || status.Status == eSyncStatus.Unknow)
					{
						status.Status = eSyncStatus.Ready;
					}
				}

				syncForm.ThrSetSyncItemStatus(Outlook.OlItemType.olAppointmentItem, status);
				//Contact
				//Task
				status = new SyncItemInfo();
				status.Status = eSyncStatus.Ready;
				syncForm.ThrSetSyncItemStatus(Outlook.OlItemType.olTaskItem, status);
				//Note
				status = new SyncItemInfo();
				status.Status = eSyncStatus.Unknow;
				syncForm.ThrSetSyncItemStatus(Outlook.OlItemType.olContactItem, status);
				
				syncForm.ThrSetSyncItemStatus(Outlook.OlItemType.olNoteItem, status);


				retVal._syncItemForm = syncForm;

			}

			if (syncOptionsForm != null)
			{
				//Force create control
				handle = syncOptionsForm.Handle;
				//Sunscribe events syncOptionsForm(View)
				retVal.HookEvents(syncOptionsForm);
			}

			_instance = retVal;

			return retVal;
		}

		public Outlook._Application OutlookApp
		{
			get
			{
				return _addinModule.OutlookApp;
			}
		}

		/// <summary>
		/// Shows the sync setting form.
		/// </summary>
		/// <param name="show">if set to <c>true</c> [show].</param>
		public void ShowSyncSettingForm(bool show)
		{
			if (_syncOptionsForm != null)
			{
				Func formMethod = _syncOptionsForm.Hide;
				if (show)
				{
					object syncAccountSetting = (object)_syncApp.CurrentSettings.CurrentSyncAppSetting;
					object syncAppointmentSetting = (object)_syncApp.CurrentSettings.CurrentSyncAppointentSetting;
					object syncContactSetting = (object)_syncApp.CurrentSettings.CurrentContactSetting;
					object syncTaskSetting = (object)_syncApp.CurrentSettings.CurrentTaskSetting;

					_syncOptionsForm.SetSetting(syncAccountSetting);
					_syncOptionsForm.SetSetting(syncAppointmentSetting);
					_syncOptionsForm.SetSetting(syncContactSetting);
					_syncOptionsForm.SetSetting(syncTaskSetting);

					formMethod = _syncOptionsForm.Show;
				}
				_syncOptionsForm.Invoke(formMethod);
			}
		}

		/// <summary>
		/// Shows the sync form.
		/// </summary>
		/// <param name="show">if set to <c>true</c> [show].</param>
		public void ShowSyncForm(bool show)
		{
			try
			{
				if (_syncItemForm != null)
				{
					Func formMethod = _syncItemForm.Hide;
					
					if (show)
					{
						formMethod = _syncItemForm.Show;
					}

					_syncItemForm.Invoke(formMethod);

				}
			}
			catch (Exception exception)
			{

				DebugAssistant.Log(exception.Message);
				throw;
			}
		}

		private ConflictResolutionAction GetConflictResolutionAction(ItemConflictingEventArgs args)
		{
			ConflictResolutionAction retVal = ConflictResolutionAction.SkipChange;
			if (_syncConflictResForm != null)
			{
				_syncConflictResForm.CurrentConflicting = args;

				Func<IWin32Window, DialogResult> functor = _syncConflictResForm.ShowDialog;
				DialogResult result = (DialogResult)_syncConflictResForm.Invoke(functor, _syncItemForm);
				if (result != DialogResult.Cancel)
				{
					retVal = _syncConflictResForm.SelectedAction;
				}

			}
			return retVal;
		}
		#region Sync application event handlers
		/// <summary>
		/// Called when [sync begin].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected virtual void OnSyncBegin(object sender, EventArgs args)
		{
			if (_syncApp.CurrentProcessedSyncType == null)
			{
				throw new NullReferenceException("CurrentProcessedSyncType");
			}
			//Устанавливаем новый статус
			SyncItemInfo status = new SyncItemInfo();
			status.Status = eSyncStatus.InProgress;
			//Вызываем в потоке формы функцию установки статуса
			_syncItemForm.ThrSetSyncItemStatus(_syncApp.CurrentProcessedSyncType.Value, status);
			_syncItemForm.ThrUpdateSyncStatisticStatus(_syncApp.SyncStatistics);
		
		}

		/// <summary>
		/// Called when [sync end].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected virtual void OnSyncEnd(object sender, EventArgs args)
		{
			if (_syncApp.CurrentProcessedSyncType == null)
			{
				throw new NullReferenceException("CurrentProcessedSyncType");
			}
			SyncApplication syncApp = sender as SyncApplication;
			if (syncApp == null)
				throw new NullReferenceException("syncApp");

			//Устанавливаем новый статус
			SyncItemInfo status = new SyncItemInfo();
			status.Status = syncApp.LastSyncErrorOccur ? eSyncStatus.Failed : eSyncStatus.Ok;
			status.Status = syncApp.SkippedItems.Count() != 0 ? eSyncStatus.SkipedChangesDetected : status.Status;
			status.LastSyncDate = DateTime.Now;
			status.ErrorDescr = syncApp.LastSyncErrorDescr;
			status.SkippedCount = syncApp.SkippedItems.Count();
			//Вызываем в потоке формы функцию установки статуса
			_syncItemForm.ThrSetSyncItemStatus(_syncApp.CurrentProcessedSyncType.Value, status);
			_syncItemForm.ThrUpdateSyncStatisticStatus(_syncApp.SyncStatistics);

			//Запоминаем статус и дату последней синхронизации
			syncProviderSetting provSetting = _syncApp.GetSyncProviderSettingByType(_syncApp.CurrentProcessedSyncType.Value);
			if (provSetting != null)
			{
				provSetting.lastSyncStatus = (int)status.Status;
				provSetting.lastSyncDate = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
				_syncApp.CurrentSettings.SaveActiveProfile();
			}
		}


		/// <summary>
		/// Called when [sync process change].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="Microsoft.Synchronization.SyncStagedProgressEventArgs"/> instance containing the event data.</param>
		protected virtual void OnSyncProcessChange(object sender, SyncStagedProgressEventArgs args)
		{
			if (_syncApp.CurrentProcessedSyncType == null)
			{
				throw new NullReferenceException("CurrentProcessedSyncType");
			}
			_syncItemForm.CurrentSyncStagedProgress = args;
	
			string debugStr = string.Format("Progress changed: provider - {0}, stage - {1}, work - {2} of {3}", 
											 args.ReportingProvider.ToString(), args.Stage.ToString(), args.CompletedWork, args.TotalWork); 
			DebugAssistant.Log(DebugSeverity.Debug, debugStr);

			_syncItemForm.ThrUpdateSyncItemStatus(_syncApp.CurrentProcessedSyncType.Value);
			
		}

		/// <summary>
		/// Called when [sync session stage process change].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="Microsoft.Synchronization.SyncOrchestratorStateChangedEventArgs"/> instance containing the event data.</param>
		protected virtual void OnSyncSessionStageProcessChange(object sender, SyncOrchestratorStateChangedEventArgs args)
		{
			if (_syncApp.CurrentProcessedSyncType == null)
			{
				throw new NullReferenceException("CurrentProcessedSyncType");
			}
			_syncItemForm.CurrentSyncSessionStage = args;

			string debugStr = string.Format("Session stage changed: {0} -> {1}", args.OldState.ToString(), args.NewState.ToString());
			DebugAssistant.Log(DebugSeverity.Debug, debugStr);
			
			_syncItemForm.ThrUpdateSyncItemStatus(_syncApp.CurrentProcessedSyncType.Value);

		}

		/// <summary>
		/// Called when [sync item conflicting].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="Microsoft.Synchronization.ItemConflictingEventArgs"/> instance containing the event data.</param>
		protected virtual void OnSyncItemConflicting(object sender, ItemConflictingEventArgs e)
		{
			if (_syncApp.CurrentProcessedSyncType == null)
			{
				throw new NullReferenceException("CurrentProcessedSyncType");
			}

			string debugStr = string.Format("Item conflict detected: source data - {0}, destination data - {1}",
											e.SourceChangeData != null ? e.SourceChangeData.ToString() : "unknow",
											e.DestinationChangeData != null ? e.DestinationChangeData.ToString() : "unknow");
			DebugAssistant.Log(debugStr);

			ConflictResolutionAction resolution = GetConflictResolutionAction(e);
			e.SetResolutionAction(resolution);
		}

		/// <summary>
		/// Called when [sync item change skiped].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="Microsoft.Synchronization.ItemChangeSkippedEventArgs"/> instance containing the event data.</param>
		protected virtual void OnSyncItemChangeSkiped(object sender, ItemChangeSkippedEventArgs args)
		{
			if (_syncApp.CurrentProcessedSyncType == null)
			{
				throw new NullReferenceException("CurrentProcessedSyncType");
			}

			SaveChangeAction changeAction = (SaveChangeAction)Enum.Parse(typeof(SaveChangeAction), 
																		args.ItemRecoverableErrorData.ItemDisplayName);

			string debugStr = string.Format("Item change skiped: {0} in stage - {1} provider position - {2} for action {3} error detail {4}",
											 args.ItemRecoverableErrorData.ItemDisplayName, args.Stage.ToString(),
											 args.ReportingProvider.ToString(), changeAction.ToString(),
											 args.ItemRecoverableErrorData.ErrorDescription);
			DebugAssistant.Log(debugStr);


		}

		#endregion
		#region SyncItem event handlers
		
		#endregion
		#region Sync settings event handlers
		protected virtual void OnSettingApply(object sender, EventArgs args)
		{
			object syncAccountSetting = (object)_syncApp.CurrentSettings.CurrentSyncAppSetting;
			object syncAppointmentSetting = (object)_syncApp.CurrentSettings.CurrentSyncAppointentSetting;
			object syncContactSetting = (object)_syncApp.CurrentSettings.CurrentContactSetting;
			object syncTaskSetting = (object)_syncApp.CurrentSettings.CurrentTaskSetting;
			try
			{
				this._syncOptionsForm.HarvestSetting(ref syncAccountSetting);
				this._syncOptionsForm.HarvestSetting(ref syncAppointmentSetting);
				this._syncOptionsForm.HarvestSetting(ref syncContactSetting);
				this._syncOptionsForm.HarvestSetting(ref syncTaskSetting);
			}
			catch (System.Exception exception)
			{
				DebugAssistant.Log(DebugSeverity.MessageBox | DebugSeverity.Error, exception.Message);
				return;
			}

			_syncApp.CurrentSettings.SaveActiveProfile();
		}
		#endregion

		#region SyncForm event handler
		protected virtual void OnProcessSync(object sender, SyncItemEventArgs args)
		{
			//Устанавливаем новый статус
			SyncItemInfo status = new SyncItemInfo();
			status.Status = eSyncStatus.InProgress;
			_syncItemForm.ThrSetSyncItemStatus(args.oItemType, status);
			//Планируем задание синхронизации
			_syncApp.SheduleSyncronizeTask(args.oItemType);
		}
		#endregion

		private void HookEvents(SyncApplication syncApp)
		{
			syncApp.SyncProcessBeginEvent -= OnSyncBegin;
			syncApp.SyncProcessBeginEvent += OnSyncBegin;

			syncApp.SyncProcessEndEvent -= OnSyncEnd;
			syncApp.SyncProcessEndEvent += OnSyncEnd;

			syncApp.ProgressChanged -= OnSyncProcessChange;
			syncApp.ProgressChanged += OnSyncProcessChange;

			syncApp.SessionStateChanged -= OnSyncSessionStageProcessChange;
			syncApp.SessionStateChanged += OnSyncSessionStageProcessChange;

			syncApp.ItemConflicting -= OnSyncItemConflicting;
			syncApp.ItemConflicting += OnSyncItemConflicting;

			syncApp.ItemChangeSkipped -= OnSyncItemChangeSkiped;
			syncApp.ItemChangeSkipped += OnSyncItemChangeSkiped;

		}

		private void HookEvents(FormSyncItem syncForm)
		{
			syncForm.ProcessSyncEvent -= OnProcessSync;
			syncForm.ProcessSyncEvent += OnProcessSync;
		}

		private void HookEvents(FormSyncOptions syncOptionsForm)
		{
			syncOptionsForm.ApplySettingEvent -= OnSettingApply;
			syncOptionsForm.ApplySettingEvent += OnSettingApply;
		}
	}
}
