using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.IBN.Business;
using Mediachase.UI.Web.Modules;
using CS = Mediachase.IBN.Business.ControlSystem;
using System.Resources;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Incidents.Modules
{
	public partial class AddPortalAttach : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strAddDoc", typeof(AddPortalAttach).Assembly);

		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		private int FolderId = -1;
		private int RootId = -1;
		CS.BaseIbnContainer bic;
		CS.FileStorage fs;

		#region ContainerKey, ContainerName
		private string _containerKey = "Workspace";
		private string ContainerKey
		{
			get
			{
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
				return "FileLibrary";
			}
		}
		#endregion

		#region guid
		protected string guid
		{
			get
			{
				if (Request["guid"] != null)
					return Request["guid"];
				else
					throw new Exception("Session guid must be defined!");
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			RegisterLinks();
			if (!Page.IsPostBack)
			{
				if (Request["ContainerKey"] != null && Request["ContainerKey"] != String.Empty)
				{
					ContainerKey = Request["ContainerKey"];
					if (ContainerKey.Contains("ProjectId_"))
					{
						string s = ContainerKey.Substring(10);
						int i = -1;
						if (int.TryParse(s, out i))
						{
							ucProject.ObjectId = i;
							ucProject.ObjectTypeId = (int)ObjectTypes.Project;
						}
					}
				}
				ViewState["ContainerKey"] = ContainerKey;
			}
			GetCurrentFolder();
			lblPathTitle.Text = LocRM.GetString("tPath");
			lblProject.Text = LocRM.GetString("Project");
			if (!Page.IsPostBack)
			{
				if (pc["fs_Sort1_" + ContainerKey] == null)
					pc["fs_Sort1_" + ContainerKey] = "sortName";
			}
			else
				BindStorage();
		}

		private void Page_PreRender(object sender, EventArgs e)
		{
			BindStorage();
			BuildPath();
		}

		private void RegisterLinks()
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/common.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/emailsend.js");
		}

		#region GetCurrentFolder
		private void GetCurrentFolder()
		{
			ContainerKey = ViewState["ContainerKey"].ToString();
			bic = CS.BaseIbnContainer.Create(ContainerName, ContainerKey);
			fs = (CS.FileStorage)bic.LoadControl("FileStorage");
			if (!Page.IsPostBack && Request["FolderId"] != null && Request["FolderId"] != String.Empty)
				FolderId = int.Parse(Request["FolderId"]);
			else if (FolderId < 0 && pc["fs_FolderId_" + ContainerKey] != null)
			{
				int iFolder = int.Parse(pc["fs_FolderId_" + ContainerKey]);
				CS.DirectoryInfo di = fs.GetDirectory(iFolder);
				if (di != null)
					FolderId = iFolder;
				else
					FolderId = fs.Root.Id;
			}
			else
				FolderId = fs.Root.Id;
			RootId = fs.Root.Id;
			pc["fs_FolderId_" + ContainerKey] = FolderId.ToString();
		}
		#endregion

		#region BindStorage
		private void BindStorage()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("Weight", typeof(int)));	//0- Root, 1- Folder, 2-File
			dt.Columns.Add(new DataColumn("Icon", typeof(string)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("sortName", typeof(string)));
			DataRow dr;
			CS.DirectoryInfo diCur = fs.GetDirectory(FolderId);
			if (diCur.ParentDirectoryId > 0)
			{
				dr = dt.NewRow();
				dr["Id"] = diCur.Id;
				dr["Weight"] = 0;
				dr["Icon"] = ResolveUrl("~/layouts/images/blank.gif");
				dr["Name"] = String.Format("<a href='{0}'>{1}</a>", GetCurrentLink(diCur.ParentDirectoryId), "[..]");
				dr["sortName"] = "";
				dt.Rows.Add(dr);
			}
			if (fs.CanUserRead(diCur.Id))
			{
				CS.DirectoryInfo[] _di = fs.GetDirectories(FolderId);
				bool CanWrite = fs.CanUserWrite(diCur.Id);
				foreach (CS.DirectoryInfo di in _di)
				{
					if (!fs.CanUserRead(di.Id))
						continue;
					dr = dt.NewRow();
					dr["Id"] = di.Id;
					dr["Weight"] = 1;
					if (Mediachase.IBN.Business.Common.IsPop3Folder(di.Id))
						dr["Icon"] = ResolveUrl("~/layouts/images/folder_mailbox.gif");
					else
						dr["Icon"] = ResolveUrl("~/layouts/images/FileTypes/Folder.gif");
					dr["Name"] = String.Format("<a href='{0}' title='{2}'>{1}</a>", GetCurrentLink(di.Id),
						(di.Name.Length > 40) ? di.Name.Substring(0, 37) + "..." : di.Name, di.Name);
					dr["sortName"] = di.Name;
					dt.Rows.Add(dr);
				}

				CS.FileInfo[] _fi = fs.GetFiles(FolderId);
				foreach (CS.FileInfo fi in _fi)
				{
					dr = dt.NewRow();
					dr["Id"] = fi.Id;
					dr["Weight"] = 2;
					dr["Icon"] = ResolveUrl("~/Common/ContentIcon.aspx?IconID=" + fi.FileBinaryContentTypeId);
					dr["Name"] = Util.CommonHelper.GetShortFileName(fi.Name, 40);
					dr["sortName"] = fi.Name;
					dt.Rows.Add(dr);
				}
			}
			DataView dv = dt.DefaultView;
			dv.Sort = "Weight, " + pc["fs_Sort1_" + ContainerKey].ToString();

			int i = 4;
			grdMain.Columns[i++].HeaderText = LocRM.GetString("DocTitle");

			if (pc["fs_PageSize1_" + ContainerKey] == null)
				pc["fs_PageSize1_" + ContainerKey] = "10";

			grdMain.PageSize = int.Parse(pc["fs_PageSize1_" + ContainerKey].ToString());

			if (pc["fs_Page1_" + ContainerKey] == null)
				pc["fs_Page1_" + ContainerKey] = "0";
			int PageIndex = int.Parse(pc["fs_Page1_" + ContainerKey].ToString());
			int ppi = dv.Count / grdMain.PageSize;
			if (dv.Count % grdMain.PageSize == 0)
				ppi = ppi - 1;
			if (PageIndex <= ppi)
			{
				grdMain.CurrentPageIndex = PageIndex;
			}
			else
			{
				grdMain.CurrentPageIndex = 0;
				pc["fs_Page1_" + ContainerKey] = "0";
			}

			grdMain.DataSource = dv;
			grdMain.DataBind();
			/*foreach (DataGridItem dgi in grdMain.Items)
			{
			}*/
		}
		#endregion

		#region BuildPath
		private void BuildPath()
		{
			if (FolderId <= 0)
				return;
			string sPath = "";

			int iFolder = FolderId;
			while (true)
			{
				CS.DirectoryInfo di = fs.GetDirectory(iFolder);
				string sFullFolderName = di.Name;
				if (iFolder == RootId)
					sFullFolderName = LocRM.GetString("tRoot");
				string sFolderName = sFullFolderName;
				if (sFolderName.Length > 13)
					sFolderName = sFolderName.Substring(0, 10) + "...";
				if (sPath == "")
					sPath = String.Format("<a href='{0}' title=\"{2}\">{1}</a>", GetCurrentLink(iFolder), sFolderName, sFullFolderName);
				else
					sPath = String.Format("<a href='{0}' title=\"{2}\">{1}</a> \\ {3}", GetCurrentLink(iFolder), sFolderName, sFullFolderName, sPath);
				if (iFolder == RootId)
					break;
				iFolder = di.ParentDirectoryId;
			}
			lblPath.Text = sPath;
		}

		#endregion

		#region GetCurrentLink
		private string GetCurrentLink(int iFolderId)
		{
			string sPath = HttpContext.Current.Request.Url.LocalPath;
			sPath += String.Format("?guid={1}&amp;Tab=0&amp;FolderId={0}&amp;ContainerKey={2}", iFolderId, guid, ContainerKey);
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.grdMain.SortCommand += new DataGridSortCommandEventHandler(grdMain_SortCommand);
			this.grdMain.PageIndexChanged += new DataGridPageChangedEventHandler(grdMain_PageIndexChanged);
			this.grdMain.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(grdMain_PageSizeChanged);
			this.grdMain.ItemCommand += new DataGridCommandEventHandler(grdMain_ItemCommand);
			this.lbSelectChecked.Click += new EventHandler(lbSelectChecked_Click);
		}
		#endregion

		#region Events
		private void grdMain_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (pc["fs_Sort1_" + ContainerKey].ToString() == (string)e.SortExpression)
				pc["fs_Sort1_" + ContainerKey] = (string)e.SortExpression + " DESC";
			else
				pc["fs_Sort1_" + ContainerKey] = (string)e.SortExpression;
		}

		private void grdMain_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			pc["fs_Page1_" + ContainerKey] = e.NewPageIndex.ToString();
		}

		private void grdMain_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["fs_PageSize1_" + ContainerKey] = e.NewPageSize.ToString();
		}

		protected void lbChangeProject_Click(object sender, EventArgs e)
		{
			FolderId = -1;
			if (ucProject.ObjectId > 0)
				ContainerKey = "ProjectId_" + ucProject.ObjectId;
			else
				ContainerKey = "Workspace";
			ViewState["ContainerKey"] = ContainerKey;
			GetCurrentFolder();
			if (pc["fs_Sort1_" + ContainerKey] == null)
				pc["fs_Sort1_" + ContainerKey] = "sortName";
		}

		void grdMain_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Select")
			{
				int selId = int.Parse(e.Item.Cells[0].Text);
				string containerKey = "EMailAttach";
				CS.BaseIbnContainer bic2 = CS.BaseIbnContainer.Create(ContainerName, containerKey);
				CS.FileStorage fs2 = (CS.FileStorage)bic2.LoadControl("FileStorage");
				CS.DirectoryInfo di = fs2.GetDirectory(fs2.Root.Id, guid, true);
				try
				{
					fs.CopyFile(selId, di.Id);
				}
				catch { }
				string sFiles = "";
				CS.FileInfo[] _fi = fs2.GetFiles(di);
				foreach (CS.FileInfo fi in _fi)
				{
					sFiles += String.Format("<div style='padding-bottom:1px;'><img align='absmiddle' src='{0}' width='16' height='16'/>&nbsp;{1}&nbsp;&nbsp;<img src='{2}' align='absmiddle' width='16' height='16' style='cursor:pointer;' onclick='_deleteFile({3})' title='{4}' /></div>",
					  ResolveUrl("~/Common/ContentIcon.aspx?IconID=" + fi.FileBinaryContentTypeId),
					  Util.CommonHelper.GetShortFileName(fi.Name, 40),
					  ResolveUrl("~/Layouts/Images/delete.gif"),
					  fi.Id, LocRM.GetString("tDelete"));
				}
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), String.Format("CloseAll(\"{0}\");", sFiles), true);
			}
		}

		void lbSelectChecked_Click(object sender, EventArgs e)
		{
			string sIds = hidForSelect.Value;
			ArrayList alIds = new ArrayList();
			while (sIds.Length > 0)
			{
				alIds.Add(sIds.Substring(0, sIds.IndexOf(",")));
				sIds = sIds.Remove(0, sIds.IndexOf(",") + 1);
			}
			string _containerKey = "EMailAttach";
			CS.BaseIbnContainer bic2 = CS.BaseIbnContainer.Create(ContainerName, _containerKey);
			CS.FileStorage fs2 = (CS.FileStorage)bic2.LoadControl("FileStorage");
			CS.DirectoryInfo di = fs2.GetDirectory(fs2.Root.Id, guid, true);
			foreach (string scId in alIds)
			{
				int assetID = int.Parse(scId);
				try
				{
					fs.CopyFile(assetID, di.Id);
				}
				catch { }
			}
			string sFiles = "";
			CS.FileInfo[] _fi = fs2.GetFiles(di);
			foreach (CS.FileInfo fi in _fi)
			{
				sFiles += String.Format("<div style='padding-bottom:1px;'><img align='absmiddle' src='{0}' width='16' height='16'/>&nbsp;{1}&nbsp;&nbsp;<img src='{2}' align='absmiddle' width='16' height='16' style='cursor:pointer;' onclick='_deleteFile({3})' title='{4}' /></div>",
				  ResolveUrl("~/Common/ContentIcon.aspx?IconID=" + fi.FileBinaryContentTypeId),
				  Util.CommonHelper.GetShortFileName(fi.Name, 40),
				  ResolveUrl("~/Layouts/Images/delete.gif"),
				  fi.Id, LocRM.GetString("tDelete"));
			}
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), String.Format("CloseAll(\"{0}\");", sFiles), true);
		}
		#endregion
	}
}