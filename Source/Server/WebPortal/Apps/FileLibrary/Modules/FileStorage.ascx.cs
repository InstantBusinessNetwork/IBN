using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.UI.Web.Util;
using System.Reflection;
using System.Resources;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business.WebDAV.Common;

namespace Mediachase.Ibn.Web.UI.FileLibrary.Modules
{
	public partial class FileStorage : System.Web.UI.UserControl
	{
		private const string _className = "FileLibrary";
		private string _viewName = "ListView";
		private const string _placeName = "FileStorage";
		public const string _folderIdKey = "FileStorage_FolderId";
		public const string _containerKeyKey = "FileStorage_ContainerKey";
		public const string _containerNameKey = "FileStorage_ContainerName";
		protected UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strLibraryAdvanced", Assembly.GetExecutingAssembly());
		protected int _folderId = -1;
		protected int _rootId = -1;
		private Mediachase.IBN.Business.ControlSystem.FileStorage _fs;

		#region _containerKey, _containerName
		protected string _containerKey
		{
			get
			{
				if (Request["ProjectId"] != null)
					return "ProjectId_" + Request["ProjectId"];
				else if (Request["IncidentId"] != null)
					return "IncidentId_" + Request["IncidentId"];
				else if (Request["TaskId"] != null)
					return "TaskId_" + Request["TaskId"];
				else if (Request["DocumentId"] != null)
					return "DocumentId_" + Request["DocumentId"];
				else if (Request["EventId"] != null)
					return "EventId_" + Request["EventId"];
				else if (Request["ToDoId"] != null)
				{
					using (IDataReader reader = Mediachase.IBN.Business.ToDo.GetToDo(int.Parse(Request["ToDoId"]), false))
					{
						if (reader.Read())
						{
							if (reader["TaskId"] != DBNull.Value)
								return "TaskId_" + reader["TaskId"].ToString();
							else if (reader["DocumentId"] != DBNull.Value)
								return "DocumentId_" + reader["DocumentId"].ToString();
							else
								return "ToDoId_" + Request["ToDoId"];
						}
						else
							return "ToDoId_" + Request["ToDoId"];
					}
				}
				else
					return "Workspace";
			}
		}

		protected string _containerName
		{
			get
			{
				return "FileLibrary";
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			GetCurrentFolder();
			CHelper.AddToContext(_folderIdKey, _folderId.ToString());
			CHelper.AddToContext(_containerKeyKey, _containerKey);
			CHelper.AddToContext(_containerNameKey, _containerName);

			if (!Page.IsPostBack)
			{
				BindBlockHeader();

				if (_pc["fs_ViewStyle"] == null)
					_pc["fs_ViewStyle"] = "ListView";	
			}
			_viewName = _pc["fs_ViewStyle"];

			grdMain.ClassName = _className;
			grdMain.ViewName = _viewName;
			grdMain.PlaceName = _placeName;

			MainMetaToolbar.ClassName = _className;
			MainMetaToolbar.ViewName = _viewName;
			MainMetaToolbar.PlaceName = _placeName;

			CommandManager cm = CommandManager.GetCurrent(this.Page);

			CommandParameters cp = new CommandParameters("FL_ChangeFolderTree");
			cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
			cp.AddCommandArgument("ContainerKey", _containerKey);
			cp.AddCommandArgument("FolderId", _rootId.ToString());
			string cmd = cm.AddCommand(_className, _viewName, _placeName, cp);
			cmd = cmd.Replace("\"", "&quot;");

			jsTreeExt.FolderId = _folderId.ToString();
			jsTreeExt.IconUrl = ResolveClientUrl("~/layouts/images/folder.gif");
			jsTreeExt.RootHrefCommand = String.Format("javascript:{0}", cmd);
			jsTreeExt.RootId = _rootId.ToString();
			jsTreeExt.RootNodeText = LocRM.GetString("tRoot");
			jsTreeExt.TreeSourceUrl = ResolveClientUrl("~/Apps/FileLibrary/Pages/FileLibraryTreeSource.aspx?ContainerName=" + _containerName + "&ContainerKey=" + _containerKey + "&FolderId=" + _folderId);

			BindDataGrid(!Page.IsPostBack);

			cm.AddCommand(_className, _viewName, _placeName, "FL_Selected_MoveToFolder");
		}

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			if (CHelper.NeedToBindGrid())
			{
				_folderId = int.Parse(_pc["fs_FolderId_" + _containerKey]);

				CHelper.AddToContext(_containerKeyKey, _containerKey);
				CHelper.AddToContext(_containerNameKey, _containerName);
				CHelper.AddToContext(_folderIdKey, _folderId.ToString());
				
				jsTreeExt.TreeSourceUrl = ResolveClientUrl("~/Apps/FileLibrary/Pages/FileLibraryTreeSource.aspx?ContainerName=" + _containerName + "&ContainerKey=" + _containerKey + "&FolderId=" + _folderId);
				jsTreeExt.FolderId = _folderId.ToString();

				BindDataGrid(true);
			}
			BuildPath();

			base.OnPreRender(e);
		}
		#endregion

		#region GetCurrentFolder
		private void GetCurrentFolder()
		{
			_fs = Mediachase.IBN.Business.ControlSystem.FileStorage.Create(_containerName, _containerKey);
			if (!IsPostBack && Request["FolderId"] != null && Request["FolderId"] != String.Empty)
				_folderId = int.Parse(Request["FolderId"]);
			else if (_folderId < 0 && _pc["fs_FolderId_" + _containerKey] != null)
			{
				int iFolder = int.Parse(_pc["fs_FolderId_" + _containerKey]);
				DirectoryInfo di = _fs.GetDirectory(iFolder);
				if (di != null)
					_folderId = iFolder;
				else
					_folderId = _fs.Root.Id;
			}
			else
				_folderId = _fs.Root.Id;
			_rootId = _fs.Root.Id;
			_pc["fs_FolderId_" + _containerKey] = _folderId.ToString();
		}
		#endregion

		#region BindBlockHeader
		/// <summary>
		/// Binds the tool bar.
		/// </summary>
		private void BindBlockHeader()
		{
			BlockHeaderMain.Title = GetGlobalResourceObject("IbnFramework.FileLibrary", "FileLibraryTitle").ToString();

			lblPathTitle.Text = GetGlobalResourceObject("IbnFramework.FileLibrary", "Path").ToString();
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid(bool dataBind)
		{
			DataTable dt = BindStorage();

			grdMain.DataSource = dt.DefaultView;

			if (dataBind)
				grdMain.DataBind();
		}
		#endregion

		#region BindStorage
		private DataTable BindStorage()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("PrimaryKeyId", typeof(string)));
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("Weight", typeof(int)));	//0- Root, 1- Folder, 2-File
			dt.Columns.Add(new DataColumn("Icon", typeof(string)));
			dt.Columns.Add(new DataColumn("BigIcon", typeof(string)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("sortName", typeof(string)));
			dt.Columns.Add(new DataColumn("Description", typeof(string)));
			dt.Columns.Add(new DataColumn("ModifiedDate", typeof(string)));
			dt.Columns.Add(new DataColumn("sortModified", typeof(DateTime)));
			dt.Columns.Add(new DataColumn("Modifier", typeof(string)));
			dt.Columns.Add(new DataColumn("sortModifier", typeof(string)));
			dt.Columns.Add(new DataColumn("Size", typeof(string)));
			dt.Columns.Add(new DataColumn("sortSize", typeof(int)));
			DataRow dr;
			DirectoryInfo diCur = _fs.GetDirectory(_folderId);
			if (diCur.ParentDirectoryId > 0)
			{
				#region Parent Directory
				dr = dt.NewRow();
				dr["Id"] = diCur.Id;
				dr["Weight"] = 0;
				dr["PrimaryKeyId"] = GetUniqueKey(dr["Weight"].ToString(), dr["Id"].ToString());
				dr["Icon"] = String.Format("<img alt='' src='{0}' width='16' height='16' />", ResolveClientUrl("~/layouts/images/blank.gif"));
				dr["BigIcon"] = "";
				if (_pc["fs_ViewStyle"] == "Thumbnails")
				{
					dr["BigIcon"] = String.Format("<a href=\"{0}\"><img src='{1}' align='absmiddle' border='0' /></a>",
						GetCurrentLink(diCur.ParentDirectoryId),
						ResolveClientUrl("/layouts/images/back-up.gif"));
				}
				dr["Name"] = String.Format("<a href=\"{0}\">{1}</a>", GetCurrentLink(diCur.ParentDirectoryId), "[..]");
				dr["sortName"] = "";
				dr["sortSize"] = 0;
				dt.Rows.Add(dr);
				#endregion
			}
			bool isExternal = Mediachase.IBN.Business.Security.CurrentUser.IsExternal;
			if (_fs.CanUserRead(diCur.Id))
			{
				DirectoryInfo[] dMas = _fs.GetDirectories(_folderId);
				bool canWrite = _fs.CanUserWrite(diCur.Id);
				foreach (DirectoryInfo di in dMas)
				{
					#region Children Directories
					if (!_fs.CanUserRead(di.Id))
						continue;
					dr = dt.NewRow();
					dr["Id"] = di.Id;
					dr["Weight"] = 1;
					dr["PrimaryKeyId"] = GetUniqueKey(dr["Weight"].ToString(), dr["Id"].ToString());
					if (Mediachase.IBN.Business.Common.IsPop3Folder(di.Id))
						dr["Icon"] = String.Format("<img alt='' src='{0}' width='16' height='16' />", ResolveClientUrl("~/layouts/images/folder_mailbox.gif"));
					else
						dr["Icon"] = String.Format("<img alt='' src='{0}' width='16' height='16' />", ResolveClientUrl("~/layouts/images/FileTypes/Folder.gif"));
					dr["BigIcon"] = "";
					if (_pc["fs_ViewStyle"] == "Thumbnails")
					{
						dr["BigIcon"] = String.Format("<a href=\"{0}\"><img src='{1}' align='absmiddle' border='0' /></a>",
							GetCurrentLink(di.Id),
							(Mediachase.IBN.Business.Common.IsPop3Folder(di.Id)) ?
							ResolveClientUrl("~/layouts/images/mailfolder.gif") :
							ResolveClientUrl("~/layouts/images/folder1.gif"));
						dr["Name"] = String.Format("<a href=\"{0}\" title='{2}'>{1}</a>",
						  GetCurrentLink(di.Id),
						  (di.Name.Length > 27) ? di.Name.Substring(0, 24) + "..." : di.Name,
						  di.Name
						  );
					}
					else
						dr["Name"] = String.Format("<a href=\"{0}\" title='{2}'>{1}</a>", GetCurrentLink(di.Id),
						  (di.Name.Length > 40) ? di.Name.Substring(0, 37) + "..." : di.Name, di.Name);
					dr["sortName"] = di.Name;
					dr["ModifiedDate"] = di.Modified.ToShortDateString();
					dr["sortModified"] = di.Modified;
					dr["Modifier"] = CommonHelper.GetUserStatus(di.ModifierId);
					dr["sortModifier"] = CommonHelper.GetUserStatusPureName(di.ModifierId);
					dr["sortSize"] = 0;
					dt.Rows.Add(dr);
					#endregion
				}

				FileInfo[] fMas = _fs.GetFiles(_folderId);
				foreach (FileInfo fi in fMas)
				{
					#region Children Files
					dr = dt.NewRow();
					dr["Id"] = fi.Id;
					dr["Weight"] = 2;
					dr["PrimaryKeyId"] = GetUniqueKey(dr["Weight"].ToString(), dr["Id"].ToString());
					dr["Icon"] = String.Format("<img alt='' src='{0}' width='16' height='16' />", ResolveClientUrl("~/Common/ContentIcon.aspx?IconID=" + fi.FileBinaryContentTypeId));
					dr["BigIcon"] = "";
					string sLink = "";
					if (fi.FileBinaryContentType.ToLower().IndexOf("url") >= 0)
						sLink = CommonHelper.GetLinkText(_fs, fi);
					if (sLink == "")
						sLink = CommonHelper.GetAbsoluteDownloadFilePath(fi.Id, fi.Name, _containerName, _containerKey);

					string sNameLocked = CommonHelper.GetLockerText(sLink);

					if (_pc["fs_ViewStyle"] == "Thumbnails")
					{
						dr["BigIcon"] = String.Format("<a href=\"{0}\"{2}>{1}</a>",
							sLink,
							String.Format("<img src='{0}' align='absmiddle' border='0' />",
							String.Format(ResolveClientUrl("~/Common/ContentIcon.aspx?Big=1&IconID={0}&ContainerKey={1}&FileId={2}"), fi.FileBinaryContentTypeId, _containerKey, fi.Id)),
							Mediachase.IBN.Business.Common.OpenInNewWindow(fi.FileBinaryContentType) ? " target='_blank'" : ""
							);
						dr["Name"] = String.Format("<a href=\"{0}\" title='{3}'{2}>{1}</a> {4}",
						  sLink,
						  CommonHelper.GetShortFileName(fi.Name, 27),
						  Mediachase.IBN.Business.Common.OpenInNewWindow(fi.FileBinaryContentType) ? " target='_blank'" : "",
						  fi.Name,
						  sNameLocked
						  );
					}
					else
						dr["Name"] = String.Format("<a href='{0}' title='{3}'{2}>{1}</a> {4}",
						  sLink,
						  CommonHelper.GetShortFileName(fi.Name, 40),
						  Mediachase.IBN.Business.Common.OpenInNewWindow(fi.FileBinaryContentType) ? " target='_blank'" : "",
						  fi.Name,
						  sNameLocked
						  );
					dr["sortName"] = fi.Name;
					dr["Description"] = fi.Description;
					dr["ModifiedDate"] = fi.Modified.ToShortDateString();
					dr["sortModified"] = fi.Modified;
					dr["Modifier"] = CommonHelper.GetUserStatus(fi.ModifierId);
					dr["sortModifier"] = CommonHelper.GetUserStatusPureName(fi.ModifierId);
					dr["Size"] = FormatBytes((long)fi.Length);
					dr["sortSize"] = fi.Length;
					dt.Rows.Add(dr);
					#endregion
				}
			}

			return dt;
		}
		#endregion

		#region BuildPath
		private void BuildPath()
		{
			if (_folderId <= 0)
				return;
			string sPath = "";

			int iFolder = _folderId;
			while (true)
			{
				DirectoryInfo di = _fs.GetDirectory(iFolder);
				string sFullFolderName = di.Name;
				if (iFolder == _rootId)
					sFullFolderName = LocRM.GetString("tRoot");
				string sFolderName = sFullFolderName;
				if (sFolderName.Length > 13)
					sFolderName = sFolderName.Substring(0, 10) + "...";
				if (sPath == "")
					sPath = String.Format("<a href=\"{0}\" title=\"{2}\">{1}</a>", GetCurrentLink(iFolder), sFolderName, sFullFolderName);
				else
					sPath = String.Format("<a href=\"{0}\" title=\"{2}\">{1}</a> \\ {3}", GetCurrentLink(iFolder), sFolderName, sFullFolderName, sPath);
				if (iFolder == _rootId)
					break;
				iFolder = di.ParentDirectoryId;
			}
			lblPath.Text = sPath;
		}

		#endregion

		#region GetCurrentLink
		private string GetCurrentLink(int iFolderId)
		{
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = new CommandParameters("FL_ChangeFolder");
			cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
			cp.AddCommandArgument("ContainerKey", _containerKey);
			cp.AddCommandArgument("FolderId", iFolderId.ToString());
			string retVal = "javascript:" + cm.AddCommand(_className, _viewName, _placeName, cp);
			retVal = retVal.Replace("\"", "&quot;");
			return retVal;

			//string sPath = HttpContext.Current.Request.Url.LocalPath;
			//if (Request["ProjectId"] != null)
			//    sPath += String.Format("?ProjectId={0}&FolderId={1}&SubTab=0", Request["ProjectId"], iFolderId);
			//else if (Request["IncidentId"] != null)
			//    sPath += String.Format("?IncidentId={0}&FolderId={1}&SubTab=0", Request["IncidentId"], iFolderId);
			//else if (Request["TaskId"] != null)
			//    sPath += String.Format("?TaskId={0}&FolderId={1}&SubTab=0", Request["TaskId"], iFolderId);
			//else if (Request["ToDoId"] != null)
			//    sPath += String.Format("?ToDoId={0}&FolderId={1}&SubTab=0", Request["ToDoId"], iFolderId);
			//else if (Request["EventId"] != null)
			//    sPath += String.Format("?EventId={0}&FolderId={1}&SubTab=0", Request["EventId"], iFolderId);
			//else if (Request["DocumentId"] != null)
			//    sPath += String.Format("?DocumentId={0}&FolderId={1}&SubTab=0", Request["DocumentId"], iFolderId);
			//else
			//    sPath += String.Format("?FolderId={0}&Tab=0", iFolderId);
			//return sPath;
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

		private string GetUniqueKey(string type, string id)
		{
			return String.Format("{0}::{1}::{2}::{3}", type, id, _containerName, _containerKey);
		}
	}
}