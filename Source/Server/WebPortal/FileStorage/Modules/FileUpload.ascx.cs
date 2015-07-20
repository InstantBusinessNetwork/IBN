using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.FileUploader;
using Mediachase.FileUploader.Web;
using System.Reflection;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.FileStorage.Modules
{
	public partial class FileUpload : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strAddDoc", Assembly.GetExecutingAssembly());

		#region Request Variables
		protected int ParentFolderId
		{
			get
			{
				if (Request["ParentFolderId"] != null)
					return int.Parse(Request["ParentFolderId"]);
				else
					return -1;
			}
		}

		protected string ContainerKey
		{
			get
			{
				if (Request["ContainerKey"] != null)
					return Request["ContainerKey"];
				else
					return "";
			}
		}

		protected string ContainerName
		{
			get
			{
				if (Request["ContainerName"] != null)
					return Request["ContainerName"];
				else
					return "";
			}
		}

		protected string ExternalId
		{
			get
			{
				if (Request["ExternalId"] != null)
					return Request["ExternalId"];
				else
					return "";
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			UtilHelper.RegisterScript(Page, "~/Scripts/AjaxFU.js");

			Ajax.Utility.RegisterTypeForAjax(typeof(FileUpload));
			if (ParentFolderId >= 0)
				pGlobalTree.Visible = false;
		}

		#region Ajax
		[Ajax.AjaxMethod()]
		public ArrayList GetProgressInfo(string FormId)
		{
			ArrayList values = new ArrayList();
			Guid progressUid = new Guid(FormId);
			UploadProgressInfo upi = UploadProgress.Provider.GetInfo(progressUid);
			if (upi == null)
			{
				values.Add("-1");
				values.Add(LocRM.GetString("tWaitForUploading"));
			}
			else
			{
				if (upi.Result == UploadResult.Succeeded)
				{
					if (upi.BytesTotal != upi.BytesReceived)
					{
						values.Add("-2");
						values.Add(LocRM.GetString("tUploadFailed"));
					}
					else
					{
						values.Add("-3");
						values.Add(LocRM.GetString("tUploadSuccess"));
					}
				}
				else
				{
					// 0
					values.Add(FormatBytes(upi.BytesReceived));
					// 1
					values.Add(FormatBytes(upi.BytesTotal));
					// 2
					values.Add(upi.EstimatedTime.ToString().Substring(0, 8));
					// 3
					values.Add((upi.TimeRemaining.ToString().Substring(0, 8)).StartsWith("-") ? "00:00:00" : upi.TimeRemaining.ToString().Substring(0, 8));

					// 4
					int percents = (int)((float)upi.BytesReceived / (float)upi.BytesTotal * 100);
					values.Add(percents.ToString());

					// 5
					string sFName = upi.CurrentFileName;
					if (sFName.LastIndexOf("\\") >= 0)
						sFName = sFName.Substring(sFName.LastIndexOf("\\") + 1);
					values.Add(LocRM.GetString("tInProgress") + " " + sFName);
				}
			}

			return values;
		}

		[Ajax.AjaxMethod()]
		public ArrayList GetFileExistence(string fName, string ContainerName, string ContainerKey, string ParentFolderId)
		{
			ArrayList values = new ArrayList();

			string sFName = fName;
			if (sFName.LastIndexOf("\\") >= 0)
				sFName = sFName.Substring(sFName.LastIndexOf("\\") + 1);

			Mediachase.IBN.Business.ControlSystem.BaseIbnContainer bic = Mediachase.IBN.Business.ControlSystem.BaseIbnContainer.Create(ContainerName, ContainerKey);
			Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");

			if (fs.FileExist(sFName, int.Parse(ParentFolderId)))
			{
				if (fs.GetFile(int.Parse(ParentFolderId), sFName).AllowHistory)
					values.Add(2);
				else
					values.Add(1);
			}
			else
				values.Add(0);

			return values;
		}
		#endregion

		#region Help Strings
		string FormatBytes(long bytes)
		{
			const double ONE_KB = 1024;
			const double ONE_MB = ONE_KB * 1024;
			const double ONE_GB = ONE_MB * 1024;
			const double ONE_TB = ONE_GB * 1024;
			const double ONE_PB = ONE_TB * 1024;
			const double ONE_EB = ONE_PB * 1024;
			const double ONE_ZB = ONE_EB * 1024;
			const double ONE_YB = ONE_ZB * 1024;

			if ((double)bytes <= 999)
				return bytes.ToString() + " bytes";
			else if ((double)bytes <= ONE_KB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_KB) + " KB";
			else if ((double)bytes <= ONE_MB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_MB) + " MB";
			else if ((double)bytes <= ONE_GB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_GB) + " GB";
			else if ((double)bytes <= ONE_TB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_TB) + " TB";
			else if ((double)bytes <= ONE_PB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_PB) + " PB";
			else if ((double)bytes <= ONE_EB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_EB) + " EB";
			else if ((double)bytes <= ONE_ZB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_ZB) + " ZB";
			else
				return ThreeNonZeroDigits((double)bytes / ONE_YB) + " YB";
		}

		string ThreeNonZeroDigits(double value)
		{
			if (value >= 100)
				return ((int)value).ToString();
			else if (value >= 10)
				return value.ToString("0.0");
			else
				return value.ToString("0.00");
		}
		#endregion
	}
}