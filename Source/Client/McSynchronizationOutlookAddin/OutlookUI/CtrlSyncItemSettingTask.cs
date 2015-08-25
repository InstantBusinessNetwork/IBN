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
	public partial class CtrlSyncItemSettingTask : OutlookAddin.OutlookUI.CtrlSyncItemSetting
	{
		public CtrlSyncItemSettingTask(ImageList imgList)
			:base(imgList, Outlook.OlItemType.olTaskItem)
		{
			InitializeComponent();
		}

		public override void SetSetting(object setting)
		{
			syncTaskSetting taskSetting = setting as syncTaskSetting;
			if (taskSetting != null)
			{
				base.SetSetting(setting);
			}
		}

		public override void HarvestSetting(ref object setting)
		{
			syncTaskSetting taskSetting = setting as syncTaskSetting;
			if (taskSetting != null)
			{
				base.HarvestSetting(ref setting);
			}
		}
	}
}
