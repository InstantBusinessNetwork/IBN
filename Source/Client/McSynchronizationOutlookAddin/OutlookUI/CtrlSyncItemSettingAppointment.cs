using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mediachase.ClientOutlook.Configuration;

namespace OutlookAddin.OutlookUI
{
	public partial class CtrlSyncItemSettingAppointment : OutlookAddin.OutlookUI.CtrlSyncItemSetting
	{
		public CtrlSyncItemSettingAppointment(ImageList imgList)
			:base(imgList, Outlook.OlItemType.olAppointmentItem)
		{
			InitializeComponent();
		}

		public override void SetSetting(object setting)
		{
			syncAppointmentSetting appointmentSetting = setting as syncAppointmentSetting;
			if (appointmentSetting != null)
			{
				base.SetSetting(setting);
			}
		}

		public override void HarvestSetting(ref object setting)
		{
			syncAppointmentSetting appointmentSetting = setting as syncAppointmentSetting;
			if (appointmentSetting != null)
			{
				base.HarvestSetting(ref setting);
			}
		}
	}
}
