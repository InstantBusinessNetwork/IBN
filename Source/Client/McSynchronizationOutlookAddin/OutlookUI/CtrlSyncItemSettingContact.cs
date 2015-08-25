using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mediachase.ClientOutlook.Configuration;

namespace OutlookAddin.OutlookUI
{
	public partial class CtrlSyncItemSettingContact : OutlookAddin.OutlookUI.CtrlSyncItemSetting
	{
		public CtrlSyncItemSettingContact(ImageList imgList)
			:base(imgList, Outlook.OlItemType.olContactItem)
		{
			InitializeComponent();
		}

		public override void SetSetting(object setting)
		{
			syncContactSetting contactSetting = setting as syncContactSetting;
			if (contactSetting != null)
			{
				base.SetSetting(setting);
			}
		}

		public override void HarvestSetting(ref object setting)
		{
			syncContactSetting contactSetting = setting as syncContactSetting;
			if (contactSetting != null)
			{
				base.HarvestSetting(ref setting);
			}
		}
	}
}
