using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Mediachase.ClientOutlook;
using OutlookAddin.OutlookUI;

namespace OutlookAddin
{
    /// <summary>
    ///   Add-in Express Add-in Module
    /// </summary>
    [GuidAttribute("B7A91DF7-B172-4BE7-9948-03789036A75B"), ProgId("OutlookAddin.AddinModule")]
    public class AddinModule : AddinExpress.MSO.ADXAddinModule
    {
		private static UIController _controller;
        public AddinModule()
        {
            InitializeComponent();
		}
		private AddinExpress.MSO.ADXOlExplorerCommandBar adxOlExplorerCommandBar1;
		private AddinExpress.MSO.ADXCommandBarButton adxCmdBarSyncMenu;
		private AddinExpress.MSO.ADXCommandBarButton adxCmdBarBtnSyncSetting;
 
        #region Component Designer generated code
        /// <summary>
        /// Required by designer
        /// </summary>
        private System.ComponentModel.IContainer components;
 
        /// <summary>
        /// Required by designer support - do not modify
        /// the following method
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.adxOlExplorerCommandBar1 = new AddinExpress.MSO.ADXOlExplorerCommandBar(this.components);
			this.adxCmdBarSyncMenu = new AddinExpress.MSO.ADXCommandBarButton(this.components);
			this.adxCmdBarBtnSyncSetting = new AddinExpress.MSO.ADXCommandBarButton(this.components);
			// 
			// adxOlExplorerCommandBar1
			// 
			this.adxOlExplorerCommandBar1.CommandBarName = "adxOlExplorerCommandBar1";
			this.adxOlExplorerCommandBar1.CommandBarTag = "a21630d3-55db-4de4-a3d4-9f0ac20683bf";
			this.adxOlExplorerCommandBar1.Controls.Add(this.adxCmdBarSyncMenu);
			this.adxOlExplorerCommandBar1.Controls.Add(this.adxCmdBarBtnSyncSetting);
			this.adxOlExplorerCommandBar1.ItemTypes = ((AddinExpress.MSO.ADXOlExplorerItemTypes)(((((AddinExpress.MSO.ADXOlExplorerItemTypes.olMailItem | AddinExpress.MSO.ADXOlExplorerItemTypes.olAppointmentItem)
						| AddinExpress.MSO.ADXOlExplorerItemTypes.olContactItem)
						| AddinExpress.MSO.ADXOlExplorerItemTypes.olTaskItem)
						| AddinExpress.MSO.ADXOlExplorerItemTypes.olNoteItem)));
			this.adxOlExplorerCommandBar1.Temporary = true;
			this.adxOlExplorerCommandBar1.UpdateCounter = 5;
			// 
			// adxCmdBarSyncMenu
			// 
			this.adxCmdBarSyncMenu.Caption = "SyncMenu";
			this.adxCmdBarSyncMenu.ControlTag = "c1baa06d-9040-401b-a9f7-e51bfd9e0f76";
			this.adxCmdBarSyncMenu.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.adxCmdBarSyncMenu.Temporary = true;
			this.adxCmdBarSyncMenu.UpdateCounter = 4;
			this.adxCmdBarSyncMenu.Click += new AddinExpress.MSO.ADXClick_EventHandler(this.adxCommandBarButton2_Click);
			// 
			// adxCmdBarBtnSyncSetting
			// 
			this.adxCmdBarBtnSyncSetting.Caption = "SyncSetting";
			this.adxCmdBarBtnSyncSetting.ControlTag = "fafd40fe-e532-4df9-b23a-5953c21d62c2";
			this.adxCmdBarBtnSyncSetting.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.adxCmdBarBtnSyncSetting.Temporary = true;
			this.adxCmdBarBtnSyncSetting.UpdateCounter = 4;
			this.adxCmdBarBtnSyncSetting.Click += new AddinExpress.MSO.ADXClick_EventHandler(this.adxCommandBarButton1_Click);
			// 
			// AddinModule
			// 
			this.AddinName = "OutlookAddin";
			this.SupportedApps = AddinExpress.MSO.ADXOfficeHostApp.ohaOutlook;
			this.AddinInitialize += new AddinExpress.MSO.ADXEvents_EventHandler(this.AddinModule_AddinInitialize);

        }
        #endregion
 
        #region Add-in Express automatic code
 
        // Required by Add-in Express - do not modify
        // the methods within this region
 
        public override System.ComponentModel.IContainer GetContainer()
        {
            if (components == null)
                components = new System.ComponentModel.Container();
            return components;
        }
 
        [ComRegisterFunctionAttribute]
        public static void AddinRegister(Type t)
        {
            AddinExpress.MSO.ADXAddinModule.ADXRegister(t);
        }
 
        [ComUnregisterFunctionAttribute]
        public static void AddinUnregister(Type t)
        {
            AddinExpress.MSO.ADXAddinModule.ADXUnregister(t);
        }
 
        public override void UninstallControls()
        {
            base.UninstallControls();
        }

        #endregion

        public Outlook._Application OutlookApp
        {
            get
            {
                return (HostApplication as Outlook._Application);
            }
        }

		private void adxCommandBarButton2_Click(object sender)
		{
			Controller.ShowSyncForm(true);

		}

		private void adxCommandBarButton1_Click(object sender)
		{
			Controller.ShowSyncSettingForm(true);

		}

		private void AddinModule_AddinInitialize(object sender, EventArgs e)
		{
			
		}

		private UIController Controller
		{
			get
			{
				UIController retVal = _controller;
				if (retVal == null)
				{
					retVal = UIController.CreateInstance(this);
					_controller = retVal;
				}

				return retVal;
			}

		}


    }
}

