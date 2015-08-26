using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OutlookAddin.OutlookUI;

namespace OutlookAddin
{
	public partial class FormSyncOptions : Form, ISettingView
	{
		delegate void VoidFunc<T>(T param1);
		private int _minHeight = 0;
		private VistaMenuControl _vistaMenuCtrl;
		private static Outlook.OlItemType[] AvailSettingTypes = { Outlook.OlItemType.olAppointmentItem, Outlook.OlItemType.olContactItem,
																	Outlook.OlItemType.olTaskItem };

		private Dictionary<VistaMenuItem, Control> _settingPagesMapping;
		public event EventHandler ApplySettingEvent;

		/// <summary>
		/// Initializes a new instance of the <see cref="FormSyncOptions"/> class.
		/// </summary>
		public FormSyncOptions()
		{
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer
					  | ControlStyles.DoubleBuffer
					  | ControlStyles.AllPaintingInWmPaint, true);


			InitializeComponent();

			this.SuspendLayout();

			InitializeSettingView();

			this.ResumeLayout(false);
		}

	
		protected virtual void OnApplySetting()
		{
			EventHandler tmp = ApplySettingEvent;
			if (tmp != null)
			{
				tmp(this, new EventArgs());
			}
		}

		#region Private members
		private void InitializeSettingView()
		{
			_vistaMenuCtrl = new VistaMenuControl();

			this.panelLeft.Controls.Add(_vistaMenuCtrl);
			_vistaMenuCtrl.MaximumSize = new Size(0, 0);
			_vistaMenuCtrl.MinimumSize = new Size(panelLeft.Width - (panelLeft.Padding.Left + panelLeft.Padding.Right), 
												  panelLeft.Height - (panelLeft.Padding.Top + panelLeft.Padding.Bottom));
			_vistaMenuCtrl.Dock = DockStyle.Fill;

			_vistaMenuCtrl.SideBar = false;
			_vistaMenuCtrl.SideBarCaption = Resources.FormSettingMenuSidebar_Caption;
			_vistaMenuCtrl.SideBarEndGradient = Color.FromArgb(81, 69, 59);
			_vistaMenuCtrl.SideBarStartGradient = Color.DarkOrange;
			_vistaMenuCtrl.SideBarFontColor = Color.Black;

			_vistaMenuCtrl.MenuStartColor = Color.FromArgb(239, 239, 239);
			_vistaMenuCtrl.MenuEndColor = Color.FromArgb(202, 202, 202);

			_vistaMenuCtrl.MenuInnerBorderColor = Color.FromArgb(167, 162, 158);
			_vistaMenuCtrl.MenuOuterBorderColor = Color.FromArgb(47, 37, 28);

			_vistaMenuCtrl.RenderSeparators = false;
			_vistaMenuCtrl.CheckOnClick = true;
			_vistaMenuCtrl.ItemHeight = 48;
			_vistaMenuCtrl.ItemWidth = 48;
			_vistaMenuCtrl.BackImageAlign = System.Drawing.ContentAlignment.TopRight;
			_vistaMenuCtrl.MenuOrientation = VistaMenuControl.VistaMenuOrientation.Vertical;
			_vistaMenuCtrl.SelectedItem = -1;
			_vistaMenuCtrl.SideBarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			_vistaMenuCtrl.SideBarFontColor = System.Drawing.Color.White;

			VistaMenuItem menuItem = CreateSettingMenuItem(eMenuSettings_Icon.Connection);
			CtrlListItemContainer settingPage = CreateSettingCtrlPage(menuItem);
			settingPage.BorderStyle = BorderStyle.None;
			settingPage.Dock = DockStyle.Fill;
			_vistaMenuCtrl.Items.Add(menuItem);
			this.panelRight.Controls.Add(settingPage);

			menuItem = CreateSettingMenuItem(eMenuSettings_Icon.SyncItems);
			settingPage = CreateSettingCtrlPage(menuItem);
			settingPage.BorderStyle = BorderStyle.None;
			settingPage.Dock = DockStyle.Fill;
			_vistaMenuCtrl.Items.Add(menuItem);
			this.panelRight.Controls.Add(settingPage);

			_vistaMenuCtrl.VistaMenuItemClick += vistaMenuCtrl_itemclick;

			this._minHeight = this.panelLeft.Height;
		}
	
		private VistaMenuItem CreateSettingMenuItem(eMenuSettings_Icon settingImgIndex)
		{
			VistaMenuItem retVal = new VistaMenuItem(_vistaMenuCtrl);
			retVal.SelectionStartColor = Color.FromArgb(152, 193, 233);
			retVal.SelectionEndColor = Color.FromArgb(134, 186, 237);
			retVal.SelectionStartColorStart = Color.FromArgb(104, 169, 234);
			retVal.SelectionEndColorEnd = Color.FromArgb(169, 232, 255);

			retVal.CheckedStartColor = Color.FromArgb(152, 193, 233);
			retVal.CheckedEndColor = Color.FromArgb(134, 186, 237);
			retVal.CheckedStartColorStart = Color.FromArgb(104, 169, 234);
			retVal.CheckedEndColorEnd = Color.FromArgb(169, 232, 255);

			retVal.InnerBorder = Color.FromArgb(254, 254, 254);
			retVal.OuterBorder = Color.FromArgb(231, 231, 231);
			retVal.CaptionFont = new Font("Tahoma", 10, FontStyle.Bold);
			retVal.ContentFont = new Font("Tahoma", 7);
			retVal.CaptionColor = Color.Black;
			retVal.ContentColor = Color.Black;

			retVal.Image = this.imageList48.Images[(int)settingImgIndex];
			retVal.ItemTag = settingImgIndex;
			return retVal;
		}

		private CtrlListItemContainer CreateSettingCtrlPage(VistaMenuItem menuItem)
		{
			CtrlListItemContainer retVal = new CtrlListItemContainer();
			switch ((eMenuSettings_Icon)menuItem.ItemTag)
			{
				case eMenuSettings_Icon.Connection:
					CtrlSyncConnection ctrSettingConnection = new CtrlSyncConnection(imageList32);
					ctrSettingConnection.Header.Caption = Resources.FormSetting_Connection_Caption;
					retVal.AddItem(ctrSettingConnection);
					break;
				case eMenuSettings_Icon.SyncItems:
					foreach (Outlook.OlItemType itemType in AvailSettingTypes)
					{
						CtrlExtendedListItemBase listItem = CreateSyncItemSetting(itemType);
						if (listItem != null)
						{
							retVal.AddItem(listItem);
							retVal.SelectedItem = listItem;
						}
					}
					break;
			}

			if (retVal != null)
			{
				if (_settingPagesMapping == null)
				{
					_settingPagesMapping = new Dictionary<VistaMenuItem, Control>();
				}
				_settingPagesMapping.Add(menuItem, retVal);
			}

			return retVal;
		}

		private CtrlExtendedListItemBase CreateSyncItemSetting(Outlook.OlItemType oItemType)
		{
			CtrlSyncItemSetting retVal = null;

			switch(oItemType)
			{
				case Outlook.OlItemType.olAppointmentItem:
					retVal = new CtrlSyncItemSettingAppointment(this.imageList32);
					retVal.Header.RegisterStateImage(eSettingItem_State.Normal, (int)eMenuSyncItemSettings_Icon.Calendar);
					retVal.Header.RegisterStateImage(eSettingItem_State.Disabled, (int)eMenuSyncItemSettings_Icon.Calendar);
					retVal.Header.RegisterStateImage(eSettingItem_State.Hover, (int)eMenuSyncItemSettings_Icon.Calendar);
					retVal.Header.RegisterStateImage(eSettingItem_State.Selected, (int)eMenuSyncItemSettings_Icon.Calendar);
					retVal.Header.Caption = Resources.FormSyncMenuItem_TextCalendar;
					break;
				case Outlook.OlItemType.olContactItem:
					retVal = new CtrlSyncItemSettingContact(this.imageList32);
					retVal.Header.RegisterStateImage(eSettingItem_State.Normal, (int)eMenuSyncItemSettings_Icon.Contact);
					retVal.Header.RegisterStateImage(eSettingItem_State.Disabled, (int)eMenuSyncItemSettings_Icon.Contact);
					retVal.Header.RegisterStateImage(eSettingItem_State.Hover, (int)eMenuSyncItemSettings_Icon.Contact);
					retVal.Header.RegisterStateImage(eSettingItem_State.Selected, (int)eMenuSyncItemSettings_Icon.Contact);
					retVal.Header.Caption = Resources.FormSyncMenuItem_TextContact;
					break;
				case Outlook.OlItemType.olTaskItem:
					retVal = new CtrlSyncItemSettingTask(this.imageList32);
					retVal.Header.RegisterStateImage(eSettingItem_State.Normal, (int)eMenuSyncItemSettings_Icon.Task);
					retVal.Header.RegisterStateImage(eSettingItem_State.Disabled, (int)eMenuSyncItemSettings_Icon.Task);
					retVal.Header.RegisterStateImage(eSettingItem_State.Hover, (int)eMenuSyncItemSettings_Icon.Task);
					retVal.Header.RegisterStateImage(eSettingItem_State.Selected, (int)eMenuSyncItemSettings_Icon.Task);
					retVal.Header.Caption = Resources.FormSyncMenuItem_TextTask;
					break;
			}

			return retVal;
		}

		private void SetCurrentSettingPage(VistaMenuItem menuItem)
		{
			if (_settingPagesMapping == null)
				throw new ArgumentNullException("not initialized");

			foreach (KeyValuePair<VistaMenuItem, Control> pageSettingMap in _settingPagesMapping)
			{
				bool selected = pageSettingMap.Key == menuItem;
				//Меняем высоту в соотв с содержимым
				if (pageSettingMap.Key == menuItem)
				{
					CtrlListItemContainer container = pageSettingMap.Value as CtrlListItemContainer;
					if (container != null)
					{
						int newHeight = Math.Max(this.panel3.Height + container.ItemsHeight, _minHeight);
						int delta = newHeight - this.panel1.Height;
						this.Height += delta;
					}
				}
				pageSettingMap.Value.Visible = selected;
			}

		}
		#endregion

		#region ISettingView Members

		/// <summary>
		/// Mediator set setting to all owned setting pages
		/// </summary>
		/// <param name="setting">The setting.</param>
		public void SetSetting(object setting)
		{
			if (setting == null)
				return;

			if (this.InvokeRequired)
			{
				VoidFunc<object> functor = this.SetSetting;
				this.Invoke(functor, setting);
			}
			else
			{
				if (_settingPagesMapping == null)
					throw new Exception("Not initialized");

				foreach (Control ctrl in _settingPagesMapping.Values)
				{
					ISettingView settingCtrl = ctrl as ISettingView;
					if (settingCtrl != null)
					{
						settingCtrl.SetSetting(setting);
					}
				}
			}
		}

		/// <summary>
		/// Mediator collect setting from all owned setting pages
		/// </summary>
		/// <param name="setting">The setting.</param>
		public void HarvestSetting(ref object setting)
		{
			if (_settingPagesMapping == null)
				throw new Exception("Not initialized");

			foreach (Control ctrl in _settingPagesMapping.Values)
			{
				ISettingView settingCtrl = ctrl as ISettingView;
				if (settingCtrl != null)
				{
					settingCtrl.HarvestSetting(ref setting);
				}
			}
			
		}

		#endregion

		#region Event handlers
		private void btCancel_Click(object sender, EventArgs e)
		{
			this.Hide();
			this.Visible = false;
		}

		private void btApply_Click(object sender, EventArgs e)
		{
			OnApplySetting();
			this.Visible = false;
		}

		protected virtual void vistaMenuCtrl_itemclick(OutlookAddin.OutlookUI.VistaMenuControl.VistaMenuItemClickArgs args)
		{
			SetCurrentSettingPage(args.Item);
		}
		#endregion

		#region Overrides
		protected override void OnClosing(CancelEventArgs e)
		{
			this.Hide();
			e.Cancel = true;
		}
		#endregion
	}
}
