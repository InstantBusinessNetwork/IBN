using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Synchronization;
using Mediachase.ClientOutlook.Configuration;
using System.Drawing.Drawing2D;

namespace OutlookAddin.OutlookUI
{
	public partial class CtrlSyncItemSetting : CtrlExtendedListItemBase, ISettingView
	{
		public Outlook.OlItemType SyncItemType { get; private set; }

		private int _deltaSize = 0;
		
		private Color _clrMenuPanelStart = Color.FromArgb(239, 239, 239);
		private Color _clrMenuPanelEnd = Color.FromArgb(202, 202, 202);

		protected CtrlHeaderListItem _header;

		public CtrlSyncItemSetting(ImageList imgList, Outlook.OlItemType oItemType)
		{
			InitializeComponent();

			SyncItemType = oItemType;
			_header = new CtrlHeaderListItem(imgList);
			_header.Dock = DockStyle.Fill;
			this.pnlTop.Controls.Add(_header);

			this.imageList1 = imgList;
			AdditionalInitialize();
			_deltaSize = this.pnlBottom.Height;
			this.Height = this.pnlTop.Height;

			this.pnlBottom.Paint += new PaintEventHandler(OnPaintExtendedPanel);
		}

		protected virtual void AdditionalInitialize()
		{
			//Sync direction
			KeyValuePair<string, SyncDirectionOrder>[] directionList = {
				new KeyValuePair<string, SyncDirectionOrder>(Resources.FormSetting_Sync_Direction_Download, SyncDirectionOrder.Download),
				new KeyValuePair<string, SyncDirectionOrder>(Resources.FormSetting_Sync_Direction_DownloadAndUpload, SyncDirectionOrder.DownloadAndUpload),
				new KeyValuePair<string, SyncDirectionOrder>(Resources.FormSetting_Sync_Direction_Upload, SyncDirectionOrder.Upload),
				new KeyValuePair<string, SyncDirectionOrder>(Resources.FormSetting_Sync_Direction_UploadAndDownload, SyncDirectionOrder.UploadAndDownload)
																	   };
			this.cbDirection.DataSource = directionList;
			this.cbDirection.DisplayMember = "Key";

			//Sync conflict resolution
			KeyValuePair<string, ConflictResolutionPolicy>[] conflictPolicyList = { 
				new KeyValuePair<string, ConflictResolutionPolicy>(Resources.FormSetting_Sync_ConflictPolicy_LocalWins, ConflictResolutionPolicy.SourceWins),
				new KeyValuePair<string, ConflictResolutionPolicy>(Resources.FormSetting_Sync_ConflictPolicy_RemoteWins, ConflictResolutionPolicy.DestinationWins),
				new KeyValuePair<string, ConflictResolutionPolicy>(Resources.FromSetting_Sync_ConflictPolicy_Custom, ConflictResolutionPolicy.ApplicationDefined)
																				  };
			this.cbConflictPolicy.DataSource = conflictPolicyList;
			this.cbConflictPolicy.DisplayMember = "Key";

			//Local folder

			//Remote folder
			RemoteSyncTarget = Resources.FormSetting_Sync_RemoteFolder_NotSet;
			
		}

		public CtrlHeaderListItem Header
		{
			get { return _header; }
		}
	
		#region Overrides
		public override void SelectedChanged(bool selected)
		{
			if (selected != IsSelected)
			{
				base.Height += _deltaSize * (selected ? 1 : -1);
				_header.CurrentItemState = selected ? eSettingItem_State.Selected : eSettingItem_State.Normal;
				this.Invalidate();
			}

			base.SelectedChanged(selected);
		}
		#endregion

		private string RemoteSyncTarget
		{
			get
			{
				return cbRemoteSyncTarget.SelectedText;
			}

			set
			{

				bool found = false;
				foreach (object item in cbRemoteSyncTarget.Items)
				{
					if (item.ToString() == value)
					{
						cbRemoteSyncTarget.SelectedItem = item;
						found = true;
						break;
					}
				}

				if (!found)
				{
					cbRemoteSyncTarget.SelectedIndex = cbRemoteSyncTarget.Items.Add(value);
				}
			}
		}

		#region Handlers
		protected void OnPaintExtendedPanel(object sender, PaintEventArgs e)
		{
			VistaMenuControl.DrawGradientBckground(e.Graphics, this.pnlBottom.ClientRectangle, _clrMenuPanelStart, _clrMenuPanelEnd);
			VistaMenuControl.DrawInnerBorder(e.Graphics, this.pnlBottom.ClientRectangle, _header.BorderColorInner);
			VistaMenuControl.DrawOuterBorder(e.Graphics, this.pnlBottom.ClientRectangle, _header.BorderColorOuter);

		}

		private void btnChangeFolder_Click(object sender, EventArgs e)
		{
			this.tbSyncFolder.Text = OutlookAddin.OutlookItemAdaptors.OutlookApplication.PickOutlookFolderPath(UIController.GetInstance().OutlookApp, 
																										  SyncItemType);
		}

		private void btnFillRemoteTargets_Click(object sender, EventArgs e)
		{
			//TODO: Получения списка доступных IBN таргетов

			var remoteTargets = new string[] { "Personal ibn calendar", "Project calendar" };
			foreach (string item in remoteTargets)
			{
				RemoteSyncTarget = item;
			}
		}

	
		#endregion

		#region ISettingView Members

		/// <summary>
		/// Sets the setting.
		/// </summary>
		/// <param name="setting">The setting.</param>
		public virtual void SetSetting(object setting)
		{
			syncAppointmentSetting appointmentSetting = setting as syncAppointmentSetting;
			if (appointmentSetting != null)
			{
				cbDirection.SelectedItem =
					((IEnumerable<KeyValuePair<string, SyncDirectionOrder>>)cbDirection.DataSource).First(x => x.Value == (SyncDirectionOrder)appointmentSetting.syncDirection);
				cbConflictPolicy.SelectedItem =
					((IEnumerable<KeyValuePair<string, ConflictResolutionPolicy>>)cbConflictPolicy.DataSource).First(x => x.Value == (ConflictResolutionPolicy)appointmentSetting.syncConflictResolution);
				tbSyncFolder.Text = appointmentSetting.localFolder;
				chkboxIncludeSubfolder.Checked = appointmentSetting.recursive;
				cbRemoteSyncTarget.SelectedItem = appointmentSetting.remoteFolder;
			}
		}

		/// <summary>
		/// Harvests the setting.
		/// </summary>
		/// <param name="setting">The setting.</param>
		public virtual void HarvestSetting(ref object setting)
		{
			syncAppointmentSetting appSetting = setting as syncAppointmentSetting;
			if (appSetting != null)
			{
				appSetting.localFolder = tbSyncFolder.Text;
				appSetting.recursive = chkboxIncludeSubfolder.Checked;
				object selectedItem = cbRemoteSyncTarget.SelectedItem;
				if (selectedItem != null)
				{
					appSetting.remoteFolder = selectedItem.ToString();
				}
				appSetting.syncDirection = (int)((KeyValuePair<string, SyncDirectionOrder>)cbDirection.SelectedItem).Value;
				appSetting.syncConflictResolution = (int)((KeyValuePair<string, ConflictResolutionPolicy>)cbConflictPolicy.SelectedItem).Value;
			}
		}

		#endregion

		#region Overrides
		public override bool IsHovering
		{
			get
			{
				return base.IsHovering;
			}
			set
			{
				base.IsHovering = value;
				if (this.Header.CurrentItemState != eSettingItem_State.Selected)
				{
					this.Header.CurrentItemState = base.IsHovering ? eSettingItem_State.Hover : eSettingItem_State.Normal;
				}
			}
		} 
		#endregion
	}
}
