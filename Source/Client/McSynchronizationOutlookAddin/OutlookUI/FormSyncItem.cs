using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mediachase.ClientOutlook;
using Microsoft.Synchronization;
using Mediachase.Sync.Core;

namespace OutlookAddin.OutlookUI
{
	public partial class FormSyncItem : Form
	{
		private VistaMenuControl _vistaMenuCtrl;

		private int _onMouseHoverAnimateDuration = 5; //0.5 sec

		public SyncStagedProgressEventArgs CurrentSyncStagedProgress { get; set; }
		public SyncOrchestratorStateChangedEventArgs CurrentSyncSessionStage { get; set; }

		#region Events
		public event EventHandler<SyncItemEventArgs> ProcessSyncEvent;

		protected virtual void OnProcessSync(SyncItemEventArgs args)
		{
			EventHandler<SyncItemEventArgs> tmp = ProcessSyncEvent;
			if (tmp != null)
			{
				tmp(this, args);
			}
		}
		#endregion

		public FormSyncItem()
			: base()
		{
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer 
						  | ControlStyles.DoubleBuffer 
						  | ControlStyles.AllPaintingInWmPaint, true);

			InitializeComponent();
			
			this.SuspendLayout();
			_vistaMenuCtrl = new VistaMenuControl();
			_vistaMenuCtrl.BackImageAlign = System.Drawing.ContentAlignment.TopRight;
			_vistaMenuCtrl.BackMenuImage = null;
			_vistaMenuCtrl.CheckOnClick = false;
			_vistaMenuCtrl.FlatSeparators = false;
			_vistaMenuCtrl.FlatSeparatorsColor = System.Drawing.Color.Silver;
			_vistaMenuCtrl.ItemHeight = 48;
			_vistaMenuCtrl.ItemWidth = 150;
			_vistaMenuCtrl.MenuOrientation = VistaMenuControl.VistaMenuOrientation.Vertical;
			_vistaMenuCtrl.MenuStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
			_vistaMenuCtrl.MinimumSize = new System.Drawing.Size(300, 46);
			_vistaMenuCtrl.Name = "_vistaMenuCtrl";
			_vistaMenuCtrl.RenderSeparators = true;
			_vistaMenuCtrl.SelectedItem = -1;
			_vistaMenuCtrl.SideBarBitmap = null;
			_vistaMenuCtrl.SideBarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			_vistaMenuCtrl.SideBarFontColor = System.Drawing.Color.White;
			_vistaMenuCtrl.MenuStartColor = Color.FromArgb(239, 239, 239);
			_vistaMenuCtrl.MenuEndColor = Color.FromArgb(202, 202, 202);
			_vistaMenuCtrl.MenuInnerBorderColor = Color.FromArgb(254, 254, 254);
			_vistaMenuCtrl.MenuOuterBorderColor = Color.FromArgb(192, 192, 192);

			_vistaMenuCtrl.SideBar = true;
			_vistaMenuCtrl.SideBarCaption = Resources.FromSyncMenuSidebar_Caption;
			_vistaMenuCtrl.SideBarEndGradient = Color.FromArgb(202, 202, 202);
			_vistaMenuCtrl.SideBarStartGradient = Color.FromArgb(202, 202, 202);
			_vistaMenuCtrl.SideBarFontColor = Color.Black;
			
			_vistaMenuCtrl.Dock = DockStyle.Fill;
			_vistaMenuCtrl.VistaMenuItemClick += vistaMenuCtrl_itemclick;
			_vistaMenuCtrl.MouseMove += vistaMenuCtrl_mouseMove;
			_vistaMenuCtrl.MouseLeave += vistaMenuCtrl_mouseLeave;

			this.panel1.Controls.Add(_vistaMenuCtrl);

			this.ResumeLayout(false);
		}


		protected override void OnLoad(EventArgs e)
		{
 			base.OnLoad(e);
		}


		protected override void OnClosing(CancelEventArgs e)
		{
			this.Hide();
			e.Cancel = true;
		}

		#region Event handlers
		protected virtual void vistaMenuCtrl_mouseLeave(object sender, EventArgs args)
		{
			foreach (SyncMenuItem menuItem in this._vistaMenuCtrl.Items)
			{
				//DebugAssistant.Log("Mouse leave " + menuItem.Hovering + " item status " + menuItem.CurrentSyncStatus.ToString());
				if (menuItem.CurrentSyncStatus != eSyncStatus.InProgress 
					&& menuItem.CurrentSyncStatus == eSyncStatus.ReadyProgress)
				{
					menuItem.AnimateStatusImg = false;
					menuItem.CurrentSyncStatus = menuItem.PrevState;
				}
			}
		}

		protected virtual void vistaMenuCtrl_mouseMove(object sender, MouseEventArgs args)
		{
			foreach (SyncMenuItem menuItem in this._vistaMenuCtrl.Items)
			{
				
				if (menuItem.CurrentSyncStatus == eSyncStatus.InProgress)
					continue;
				if (menuItem.Hovering && menuItem.CurrentSyncStatus == eSyncStatus.ReadyProgress)
					continue;
				if (!menuItem.Hovering && menuItem.CurrentSyncStatus != eSyncStatus.ReadyProgress)
					continue;

				//DebugAssistant.Log("Mouse Move " + args.ToString() + " Hovering" + menuItem.Hovering + " item status " + menuItem.CurrentSyncStatus.ToString());
				if (menuItem.CurrentSyncStatus != eSyncStatus.Unknow)
				{
					if (menuItem.Hovering)
					{
						menuItem.PrevState = menuItem.CurrentSyncStatus;
						menuItem.CurrentSyncStatus = eSyncStatus.ReadyProgress;
						menuItem.AnimateDuration = _onMouseHoverAnimateDuration;
						menuItem.AnimateStatusImg = true;

					}
					else
					{
						menuItem.AnimateDuration = -1;
						menuItem.CurrentSyncStatus = menuItem.PrevState;
						menuItem.AnimateStatusImg = false;
					}
				}
			}
		}

		protected virtual void vistaMenuCtrl_itemclick(OutlookAddin.OutlookUI.VistaMenuControl.VistaMenuItemClickArgs args)
		{
			Outlook.OlItemType oItemType = (Outlook.OlItemType)args.Item.ItemTag;
			SyncMenuItem menuItem = FindSyncMenuItem(oItemType);
			//Избегаем повторного нажатия
			if (menuItem != null && menuItem.CurrentSyncStatus != eSyncStatus.InProgress)
			{
				SyncItemEventArgs syncItemArgs = new SyncItemEventArgs(oItemType);
				OnProcessSync(syncItemArgs);
			}
		}

		#endregion
	
		private IEnumerable<SyncMenuItem> MenuItems
		{
			get
			{
				foreach (SyncMenuItem item in _vistaMenuCtrl.Items)
				{
					yield return item;
				}
			}
		}

	
		private SyncMenuItem FindSyncMenuItem(Outlook.OlItemType oItemType)
		{
			return MenuItems.FirstOrDefault(x=>(Outlook.OlItemType)x.ItemTag == oItemType);
		}

		/// <summary>
		/// Adds the sync menu item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void AddSyncMenuItem(Outlook.OlItemType oItemType)
		{
			SyncMenuItem menuItem = CreateSyncMenuItem(oItemType);
			if (menuItem != null)
			{
				_vistaMenuCtrl.Items.Add(menuItem);
				_vistaMenuCtrl.Invalidate();
			}
		}

		/// <summary>
		/// Sets the sync item status.
		/// </summary>
		/// <param name="oItemType">Type of the o item.</param>
		/// <param name="status">The status.</param>
		public bool ThrSetSyncItemStatus(Outlook.OlItemType oItemType, SyncItemInfo syncInfo)
		{

			if (this.InvokeRequired)
			{
				Func<Outlook.OlItemType, SyncItemInfo, bool> func = this.ThrSetSyncItemStatus;
				this.Invoke(func, oItemType, syncInfo);
			}
			else
			{
				SyncMenuItem item = FindSyncMenuItem(oItemType);
				if (item != null)
				{
					//Если предыдущий статус был InProgress, снимаем disable c других пунктов
					if (item.CurrentSyncStatus == eSyncStatus.InProgress && syncInfo.Status != eSyncStatus.InProgress)
					{
						EnableAllSyncItems(x=>x.CurrentSyncStatus != eSyncStatus.Unknow);
					}

					if (syncInfo.Status == eSyncStatus.InProgress)
					{
						//Делаем неактивнми остальные пункты меню синхронизации
						DisableAllSyncItems(x=>(Outlook.OlItemType)x.ItemTag != oItemType );
						item.Description = Resources.FormSyncMenuItem_ProcessStatus;
					}
					else if (syncInfo.Status == eSyncStatus.Ready || syncInfo.Status == eSyncStatus.Ok)
					{
						if (syncInfo.LastSyncDate != DateTime.MinValue)
						{
							item.Description = FormatingStatus(Resources.FormSyncMenuItem_ReadyStatus_Pattern, new object[] { syncInfo.LastSyncDate });
						}
						else
						{
							item.Description = Resources.FormSyncMenuItem_NotSyncStatus_Pattern;
						}
					}
					else if (syncInfo.Status == eSyncStatus.Failed)
					{
						item.Description = FormatingStatus(Resources.FormSyncmenuItem_FailStatus_Pattern, new object[] { syncInfo.ErrorDescr });
					}
					else if (syncInfo.Status == eSyncStatus.SkipedChangesDetected)
					{
						item.Description = FormatingStatus(Resources.FormSyncMenuItem_SkippedItemPattern, new object[] { syncInfo.SkippedCount });
					}
					else if (syncInfo.Status == eSyncStatus.Unknow)
					{
						//Disabling unknow status
						item.Description = Resources.FormSyncMenuItem_NotSyncStatus_Pattern;
						DisableSyncItem(oItemType);
					}
					//Устанавливаем новый статус
					item.CurrentSyncStatus = syncInfo.Status;
					item.AnimateStatusImg = false;
					switch (item.CurrentSyncStatus)
					{
						case eSyncStatus.InProgress:
							item.AnimateStatusImg = true;
							item.AnimateDuration = -1;
							break;
						case eSyncStatus.ReadyProgress:
							item.AnimateDuration = _onMouseHoverAnimateDuration;
							item.AnimateStatusImg = true;
							break;
					}

				}
			}
			return true;

		}

		/// <summary>
		/// Creates the sync menu item.
		/// </summary>
		/// <param name="oItemType">Type of the o item.</param>
		/// <returns></returns>
		private SyncMenuItem CreateSyncMenuItem(Outlook.OlItemType oItemType)
		{

			SyncMenuItem retVal = new SyncMenuItem(_vistaMenuCtrl, this.imageList1);
			switch (oItemType)
			{
				case Outlook.OlItemType.olAppointmentItem:
					retVal.RegisterStatusImages(eSyncStatus.InProgress, (int)eSyncMenuItem_Icon.Calendar_sync, (int)eSyncMenuItem_Icon.Calendar_sync_1,
																   (int)eSyncMenuItem_Icon.Calendar_sync_2, (int)eSyncMenuItem_Icon.Calendar_sync_3);
					retVal.RegisterStatusImages(eSyncStatus.Failed, (int)eSyncMenuItem_Icon.Calendar_failed);
					retVal.RegisterStatusImages(eSyncStatus.Canceled, (int)eSyncMenuItem_Icon.Calendar_canceled);
					retVal.RegisterStatusImages(eSyncStatus.ReadyProgress, (int)eSyncMenuItem_Icon.Calendar_sync, (int)eSyncMenuItem_Icon.Calendar_sync_1,
																   (int)eSyncMenuItem_Icon.Calendar_sync_2, (int)eSyncMenuItem_Icon.Calendar_sync_3);
					retVal.RegisterStatusImages(eSyncStatus.Ready, (int)eSyncMenuItem_Icon.Calendar_ready);
					retVal.RegisterStatusImages(eSyncStatus.Ok, (int)eSyncMenuItem_Icon.Calendar_ok);
					retVal.RegisterStatusImages(eSyncStatus.Unknow, (int)eSyncMenuItem_Icon.Calendar_unknow);
					retVal.RegisterStatusImages(eSyncStatus.SkipedChangesDetected, (int)eSyncMenuItem_Icon.Calendar_canceled);
					retVal.ItemTag = oItemType;
					retVal.Text = Resources.FormSyncMenuItem_TextCalendar;
					break;
				case Outlook.OlItemType.olContactItem:
					retVal.RegisterStatusImages(eSyncStatus.InProgress, (int)eSyncMenuItem_Icon.Contact_sync, (int)eSyncMenuItem_Icon.Contact_sync_1,
															   (int)eSyncMenuItem_Icon.Contact_sync_2, (int)eSyncMenuItem_Icon.Contact_sync_3);
					retVal.RegisterStatusImages(eSyncStatus.Failed, (int)eSyncMenuItem_Icon.Contact_failed);
					retVal.RegisterStatusImages(eSyncStatus.Canceled, (int)eSyncMenuItem_Icon.Contact_canceled);
					retVal.RegisterStatusImages(eSyncStatus.ReadyProgress, (int)eSyncMenuItem_Icon.Contact_sync, (int)eSyncMenuItem_Icon.Contact_sync_1,
																   (int)eSyncMenuItem_Icon.Contact_sync_2, (int)eSyncMenuItem_Icon.Contact_sync_3);
					retVal.RegisterStatusImages(eSyncStatus.Ready, (int)eSyncMenuItem_Icon.Contact_ok);
					retVal.RegisterStatusImages(eSyncStatus.Unknow, (int)eSyncMenuItem_Icon.Contact_unknow);
					retVal.ItemTag = oItemType;
					retVal.Text = Resources.FormSyncMenuItem_TextContact;
					break;
				case Outlook.OlItemType.olTaskItem:
					retVal.RegisterStatusImages(eSyncStatus.InProgress, (int)eSyncMenuItem_Icon.Task_sync, (int)eSyncMenuItem_Icon.Task_sync_1,
															   (int)eSyncMenuItem_Icon.Task_sync_2, (int)eSyncMenuItem_Icon.Task_sync_3);
					retVal.RegisterStatusImages(eSyncStatus.Failed, (int)eSyncMenuItem_Icon.Task_failed);
					retVal.RegisterStatusImages(eSyncStatus.Canceled, (int)eSyncMenuItem_Icon.Task_canceled);
					retVal.RegisterStatusImages(eSyncStatus.ReadyProgress, (int)eSyncMenuItem_Icon.Task_sync, (int)eSyncMenuItem_Icon.Task_sync_1,
																   (int)eSyncMenuItem_Icon.Task_sync_2, (int)eSyncMenuItem_Icon.Task_sync_3);
					retVal.RegisterStatusImages(eSyncStatus.Ready, (int)eSyncMenuItem_Icon.Task_ready);
					retVal.RegisterStatusImages(eSyncStatus.Unknow, (int)eSyncMenuItem_Icon.Task_unknow);
					retVal.ItemTag = oItemType;
					retVal.Text = Resources.FormSyncMenuItem_TextTask;
					break;
				case Outlook.OlItemType.olNoteItem:
					retVal.RegisterStatusImages(eSyncStatus.InProgress, (int)eSyncMenuItem_Icon.Note_sync, (int)eSyncMenuItem_Icon.Note_sync_1,
															   (int)eSyncMenuItem_Icon.Note_sync_2, (int)eSyncMenuItem_Icon.Note_sync_3);
					retVal.RegisterStatusImages(eSyncStatus.Failed, (int)eSyncMenuItem_Icon.Note_failed);
					retVal.RegisterStatusImages(eSyncStatus.Canceled, (int)eSyncMenuItem_Icon.Note_canceled);
					retVal.RegisterStatusImages(eSyncStatus.ReadyProgress, (int)eSyncMenuItem_Icon.Note_sync, (int)eSyncMenuItem_Icon.Note_sync_1,
																   (int)eSyncMenuItem_Icon.Note_sync_2, (int)eSyncMenuItem_Icon.Note_sync_3);
					retVal.RegisterStatusImages(eSyncStatus.Ready, (int)eSyncMenuItem_Icon.Note_ok);
					retVal.RegisterStatusImages(eSyncStatus.Unknow, (int)eSyncMenuItem_Icon.Note_unknow);
					retVal.ItemTag = oItemType;
					retVal.Text = Resources.FormSyncMenuItem_TextNote;
					break;
			}

			if (retVal != null)
			{
				retVal.SelectionStartColor = Color.FromArgb(152, 193, 233);
				retVal.SelectionEndColor = Color.FromArgb(134, 186, 237);
				retVal.SelectionStartColorStart = Color.FromArgb(104, 169, 234);
				retVal.SelectionEndColorEnd = Color.FromArgb(169, 232, 255);
				retVal.InnerBorder = Color.FromArgb(254, 254, 254);
				retVal.OuterBorder = Color.FromArgb(231, 231, 231);
				retVal.CaptionFont = new Font("Tahoma", 10, FontStyle.Bold);
				retVal.ContentFont = new Font("Tahoma", 7);
				retVal.CaptionColor = Color.Black;
				retVal.ContentColor = Color.Black;
			}
			return retVal;
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			//Set new size
			this.Width = _vistaMenuCtrl.Width + 10;
			this.Height = _vistaMenuCtrl.Height + this.statusStrip1.Height + 38;
		}

		protected void EnableAllSyncItems(Func<SyncMenuItem, bool> predicate)
		{
			DisableEnableAllSyncItems(false, predicate);
		}

		protected void EnableSyncItem(Outlook.OlItemType oItemType)
		{
			DisableEnableSyncItem(false, oItemType);
		}

		protected void DisableAllSyncItems(Func<SyncMenuItem, bool> predicate)
		{
			DisableEnableAllSyncItems(true, predicate);
		}

		protected void DisableSyncItem(Outlook.OlItemType oItemType)
		{
			DisableEnableSyncItem(true, oItemType);
		}

		protected void DisableEnableAllSyncItems(bool disableEnable, Func<SyncMenuItem, bool> predicate)
		{
			foreach (SyncMenuItem menuItem in MenuItems.Where(predicate))
			{
				DisableEnableSyncItem(disableEnable, menuItem);
			}
		}

		protected virtual void DisableEnableSyncItem(bool disableEnable, Outlook.OlItemType oItemType)
		{
			foreach (SyncMenuItem menuItem in MenuItems)
			{
				if (((Outlook.OlItemType)menuItem.ItemTag) == oItemType)
				{
					DisableEnableSyncItem(disableEnable, menuItem);
					break;
				}
			}
		}

		protected virtual void DisableEnableSyncItem(bool disableEnable, SyncMenuItem menuItem)
		{
			if (menuItem == null)
				throw new ArgumentNullException("menuItem");

			if (menuItem.Disabled != disableEnable)
			{
				menuItem.Disabled = disableEnable;
			}
		}

		public bool ThrUpdateSyncStatisticStatus(SyncStatistics syncStats)
		{
			bool retVal = true;
			if (this.InvokeRequired)
			{
				Func<SyncStatistics, bool> func = this.ThrUpdateSyncStatisticStatus;
				retVal = (bool)this.Invoke(func, syncStats);
			}
			else
			{
				if (syncStats != null)
				{
					this.statusStrip1.Text = String.Format(Resources.FormSyncMenuStatusStrip_Pattern,
															syncStats.UploadChangesApplied,
															syncStats.UploadChangesFailed,
															syncStats.UploadChangesTotal,
															syncStats.DownloadChangesApplied,
															syncStats.DownloadChangesFailed,
															syncStats.DownloadChangesTotal);
				}
			}

			return retVal;
		}
		/// <summary>
		/// THRs the update sync item status.
		/// </summary>
		/// <param name="oItemType">Type of the o item.</param>
		/// <param name="args">The <see cref="Microsoft.Synchronization.SyncStagedProgressEventArgs"/> instance containing the event data.</param>
		/// <returns></returns>
		public bool ThrUpdateSyncItemStatus(Outlook.OlItemType oItemType)
		{
			bool retVal = true;
			if (this.InvokeRequired)
			{
				Func<Outlook.OlItemType, bool> func = this.ThrUpdateSyncItemStatus;
				retVal = (bool)this.Invoke(func, oItemType);
			}
			else
			{
				SyncMenuItem item = FindSyncMenuItem(oItemType);
				if (item == null)
				{
					throw new Exception("Unable to find menuItem");
				}
				int percent = 0;
				string syncState = "unknow";
				string reportingProvider = "unknow";
				uint totalWork = 0;
				uint completedWork = 0;

				if (CurrentSyncStagedProgress != null)
				{
					totalWork = CurrentSyncStagedProgress.TotalWork;
					completedWork = CurrentSyncStagedProgress.CompletedWork;

					percent = totalWork == 0 ? 0 : (int)((double)completedWork / (double)totalWork * 100);
					reportingProvider = CurrentSyncStagedProgress.ReportingProvider.ToString();
				}

				if (CurrentSyncSessionStage != null)
				{
					syncState = CurrentSyncSessionStage.NewState.ToString();
				}

				//item.Description = FormatingStatus(Resources.SyncMenuItem_ProcessStatus_Pattern1,
				//                                    new object[] { syncState, reportingProvider, percent });

				item.Description = FormatingStatus(Resources.FormSyncMenuItem_ProcessStatus_Pattern2,
													new object[] { syncState, reportingProvider, completedWork, totalWork });
			}
			return retVal;
		}

		private string FormatingStatus(string pattern, object[] replacements)
		{
			return String.Format(pattern, replacements);
		}

	}
}
