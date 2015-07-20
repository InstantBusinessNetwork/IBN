using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.UI.Web.Util;
using Mediachase.IBN.Business.WebDAV.Common;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.FileStorage
{
	/// <summary>
	/// Summary description for FileInfoView.
	/// </summary>
	public partial class FileInfoView : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strViewDocument", typeof(FileInfoView).Assembly);
		
		private int _fileId = -1;
		private string _containerKey = "";
		private string _containerName = "";

		#region Request Variables
		private int FileId
		{
			get
			{
				if(Request["FileId"]!=null)
					return int.Parse(Request["FileId"]);
				else
					return _fileId;
			}
			set
			{
				_fileId = value;
			}
		}

		private string ContainerKey
		{
			get
			{
				if(Request["ContainerKey"]!=null)
					return Request["ContainerKey"];
				else
					return _containerKey;
			}
			set
			{
				_containerKey = value;
			}
		}

		private string ContainerName
		{
			get
			{
				if(Request["ContainerName"]!=null)
					return Request["ContainerName"];
				else
					return _containerName;
			}
			set
			{
				_containerName = value;
			}
		}
		#endregion

		BaseIbnContainer bic;
		Mediachase.IBN.Business.ControlSystem.FileStorage fs;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			ApplyValues();
			if(!Page.IsPostBack)
				BindValues();
		}

		#region ApplyValues
		private void ApplyValues()
		{
			if (Request["PrimaryKeyId"] != null)
			{
				string[] elem = Request["PrimaryKeyId"].Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
				FileId = int.Parse(elem[1]);
				ContainerName = elem[2];
				ContainerKey = elem[3];
			}
			bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
			fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			FileInfo fi = fs.GetFile(FileId);
			string sLink = "";
			if (fi.FileBinaryContentType.ToLower().IndexOf("url") >= 0)
				sLink = Util.CommonHelper.GetLinkText(fs, fi);
			if (sLink == "")
				sLink = Util.CommonHelper.GetAbsoluteDownloadFilePath(fi.Id, fi.Name, ContainerName, ContainerKey);

			string sNameLocked = CommonHelper.GetLockerText(sLink);

			lblName.Text = String.Format(CultureInfo.InvariantCulture,
				"<img src='{3}' width='16' height='16' border='0' align='absmiddle'/>&nbsp;<a href=\"{0}\" title='{4}'{2}>{1}</a> {5}",
				sLink,
				Util.CommonHelper.GetShortFileName(fi.Name, 40),
				Mediachase.IBN.Business.Common.OpenInNewWindow(fi.FileBinaryContentType) ? " target='_blank'" : "",
				ResolveUrl("~/Common/ContentIcon.aspx?IconID=" + fi.FileBinaryContentTypeId),
				fi.Name,
				sNameLocked);

			string ParentLink = "";
			string ParentName = "";
			if (fi.ContainerKey.StartsWith("Workspace"))
			{
				lblPath.Text = BuildPath(fi.ParentDirectoryId);
			}
			else
			{
				Util.CommonHelper.GetParentContainer(fi.ContainerKey, fi.ParentDirectory, out ParentName, out ParentLink);
				lblPath.Text = ParentName + " \\ " + BuildPath(fi.ParentDirectoryId);
			}

			lblDescription.Text = fi.Description;
			lblCreator.Text = Util.CommonHelper.GetUserStatusPureName(fi.CreatorId);
			lblCreated.Text = fi.Created.ToShortDateString() + " " + fi.Created.ToShortTimeString();
			lblModifier.Text = Util.CommonHelper.GetUserStatusPureName(fi.ModifierId);
			lblModified.Text = fi.Modified.ToShortDateString() + " " + fi.Modified.ToShortTimeString();
			lblKeepHistory.Text = String.Format(CultureInfo.InvariantCulture,
				"<img align='absmiddle' border='0' src='{0}' />&nbsp;{1}",
				(fi.AllowHistory) ? ResolveUrl("~/layouts/images/accept.gif") :
				ResolveUrl("~/layouts/images/deny.gif"),
				LocRM.GetString("tKeepHistory"));
			lblViewCount.Text = fi.DownloadCount.ToString();
			if (fi.AllowHistory)
			{
				dgFiles.Visible = true;
				trManage.Visible = true;
				lblNoHistory.Visible = false;

				dgFiles.Columns[1].HeaderText = LocRM.GetString("tName");
				dgFiles.Columns[2].HeaderText = LocRM.GetString("tUpdatedBy");
				dgFiles.Columns[3].HeaderText = LocRM.GetString("tUpdDate");
				dgFiles.Columns[4].HeaderText = LocRM.GetString("tSize");

				FileHistoryInfo[] _fhi = fi.GetHistory();
				DataTable dt = new DataTable();
				dt.Columns.Add(new DataColumn("Id", typeof(int)));
				dt.Columns.Add(new DataColumn("Weight", typeof(int)));
				dt.Columns.Add(new DataColumn("Name", typeof(string)));
				dt.Columns.Add(new DataColumn("Modified", typeof(DateTime)));
				dt.Columns.Add(new DataColumn("Modifier", typeof(string)));
				dt.Columns.Add(new DataColumn("Size", typeof(string)));
				DataRow dr;
				foreach (FileHistoryInfo fhi in _fhi)
				{
					dr = dt.NewRow();
					dr["Id"] = fhi.Id;
					dr["Weight"] = 1;

					string sLinkH = "";
					if (fhi.FileBinaryContentType.ToLower().IndexOf("url") >= 0)
						sLinkH = CommonHelper.GetLinkText(fs, fhi);
					if (sLinkH == "")
						sLinkH = CommonHelper.GetAbsoluteDownloadFilePath(fi.Id, fi.Name, fhi.Id, ContainerName, ContainerKey);

					string sNameLockedH = CommonHelper.GetLockerText(sLinkH);

					dr["Name"] = String.Format(CultureInfo.InvariantCulture,
						"<a href=\"{0}\" title='{3}'{2}>{1}</a> {4}",
						sLinkH,
						CommonHelper.GetShortFileName(fhi.Name, 40),
						Mediachase.IBN.Business.Common.OpenInNewWindow(fhi.FileBinaryContentType) ? " target='_blank'" : "",
						fhi.Name,
						sNameLockedH
						);
					
					dr["Modified"] = fhi.Modified;
					dr["Modifier"] = Util.CommonHelper.GetUserStatusPureName(fhi.ModifierId);
					dr["Size"] = FormatBytes((long)fhi.Length);
					dt.Rows.Add(dr);
				}
				dr = dt.NewRow();
				dr["Id"] = fi.Id;
				dr["Weight"] = 0;
				dr["Name"] = String.Format(CultureInfo.InvariantCulture,
					"<a href='{0}' title='{3}'{2}>{1}</a>",
					sLink,
					Util.CommonHelper.GetShortFileName(fi.Name, 40),
					Mediachase.IBN.Business.Common.OpenInNewWindow(fi.FileBinaryContentType) ? " target='_blank'" : "",
					fi.Name
					);
				dr["Modified"] = fi.Modified;
				dr["Modifier"] = Util.CommonHelper.GetUserStatusPureName(fi.ModifierId);
				dr["Size"] = FormatBytes((long)fi.Length);
				dt.Rows.Add(dr);

				DataView dv = dt.DefaultView;
				dv.Sort = "Weight, Modified DESC";

				dgFiles.DataSource = dv;
				dgFiles.DataBind();
			}
			else
			{
				dgFiles.Visible = false;
				trManage.Visible = false;
				lblNoHistory.Visible = true;
				lblNoHistory.Text = LocRM.GetString("tHistoryIsNot");
			}
		}
		#endregion

		#region BuildPath
		private string BuildPath(int FolderId)
		{
			string sPath = "";
			
			int iFolder = FolderId;
			while (true)
			{
				DirectoryInfo di = fs.GetDirectory(iFolder);
				string sFullFolderName = di.Name;
				if(iFolder==fs.Root.Id)
					sFullFolderName = LocRM.GetString("tRoot");
				string sFolderName = sFullFolderName;
				if (sFolderName.Length > 13)
					sFolderName = sFolderName.Substring(0,10) + "...";
				if (sPath == "")
					sPath = sFolderName; 
				else 
					sPath = String.Format("{0} \\ {1}", sFolderName, sPath);
				if(iFolder==fs.Root.Id)
					break;
				iFolder = di.ParentDirectoryId;
			}
			return sPath;
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion

		#region Help Strings
		static string FormatBytes(long bytes)
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

		static string ThreeNonZeroDigits(double value)
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
