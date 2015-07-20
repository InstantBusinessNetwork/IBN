namespace Mediachase.UI.Web.FileStorage.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Text;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using ComponentArt.Web.UI;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.ControlSystem;
	using Mediachase.UI.Web.Modules;
	using Mediachase.IBN.Business.WebDAV.Common;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for FileStorageControl.
	/// </summary>
	public partial class FileStorageControl : System.Web.UI.UserControl
	{

		private UserLightPropertyCollection _pc = Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strLibraryAdvanced", typeof(FileStorageControl).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strLibraryList", typeof(FileStorageControl).Assembly);

		private int _folderId = -1;
		private int _rootId = -1;
		private BaseIbnContainer _bic;
		private Mediachase.IBN.Business.ControlSystem.FileStorage _fs;

		#region _containerKey, _containerName
		private string _containerKey
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

		private string _containerName
		{
			get
			{
				return "FileLibrary";
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			GetCurrentFolder();
			ApplyLocalization();
			if (!Page.IsPostBack)
			{
				if (_pc["fs_Sort_" + _containerKey] == null)
					_pc["fs_Sort_" + _containerKey] = "sortName";
				if (_pc["fs_ViewStyle"] == null)
					_pc["fs_ViewStyle"] = "ListView";
			}
			else
				BindStorage();
			BindToolbar();
			
		}

		private void Page_PreRender(object sender, EventArgs e)
		{
			BindStorage();
			BuildPath();
		}

		#region GetCurrentFolder
		private void GetCurrentFolder()
		{
			_bic = BaseIbnContainer.Create(_containerName, _containerKey);
			_fs = (FileStorage)_bic.LoadControl("FileStorage");
			if (Request["FolderId"] != null && Request["FolderId"] != String.Empty)
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

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			lblPathTitle.Text = LocRM.GetString("Path");
		}
		#endregion

		#region BindToolbar - Menu
		private void BindToolbar()
		{
			if (this.Parent.Parent is IToolbarLight || this.Parent is IToolbarLight)
			{
				DirectoryInfo diCur = _fs.GetDirectory(_folderId);
				if (diCur == null)
					throw new Mediachase.Ibn.AccessDeniedException();

				BlockHeaderLightWithMenu secHeader;
				if (this.Parent.Parent is IToolbarLight)
					secHeader = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent.Parent).GetToolBar();
				else
					secHeader = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent).GetToolBar();

				if (_containerKey.ToLower().StartsWith("workspace"))
					secHeader.AddText(LocRM.GetString("tGlobal"));
				secHeader.ActionsMenu.Items.Clear();
				secHeader.ClearRightItems();

				ComponentArt.Web.UI.MenuItem subItem;

				#region MenuItems
				bool isExternal = Security.CurrentUser.IsExternal;
				bool canWrite = _fs.CanUserWrite(diCur.Id);
				bool canAdmin = _fs.CanUserAdmin(diCur.Id);

				if (canWrite)
				{
					#region New File
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.LookId = "TopItemLook";
					string commandLink = (Security.CurrentUser.IsExternal) ? "~/External/FileUpload.aspx" : "~/FileStorage/FileUpload.aspx";
					subItem.ClientSideCommand = String.Format("javascript:ShowWizard('{0}?ParentFolderId={1}&ContainerKey={2}&ContainerName={3}{4}', 470, 270);",
					  ResolveUrl(commandLink), _folderId, _containerKey, _containerName,
					  (Security.CurrentUser.IsExternal) ? ("&ExternalId=" + Security.CurrentUser.UserID) : "");
					subItem.Text = /*"<img border='0' src='../Layouts/Images/icons/newfile.gif' width='16px' height='16px' align='absmiddle'/> " + */LocRM.GetString("tAddD");
					subItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/icons/newfile.gif");
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					secHeader.ActionsMenu.Items.Add(subItem);
					#endregion

					if (!isExternal)
					{
						#region New Folder
						subItem = new ComponentArt.Web.UI.MenuItem();
						subItem.LookId = "TopItemLook";
						subItem.ClientSideCommand = "javascript:ShowWizard('" + ResolveUrl("~/FileStorage/DirectoryEdit.aspx") + "?ParentFolderId=" + _folderId + "&ContainerKey=" + _containerKey + "&ContainerName=" + _containerName + "', 500, 130);";
						subItem.Text = /*"<img border='0' src='../Layouts/Images/nfolder.gif' width='16px' height='16px' align='absmiddle'/> " + */LocRM.GetString("tAdd");
						subItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/nfolder.gif");
						subItem.Look.LeftIconHeight = Unit.Pixel(16);
						subItem.Look.LeftIconWidth = Unit.Pixel(16);
						secHeader.ActionsMenu.Items.Add(subItem);
						#endregion
					}
				}

				ComponentArt.Web.UI.MenuItem topMenuItem;

				#region Table-Details-Thumbnails
				topMenuItem = new ComponentArt.Web.UI.MenuItem();
				topMenuItem.Text = LocRM.GetString("tView");
				topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
				topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
				topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
				topMenuItem.LookId = "TopItemLook";

				string sCurrentView = _pc["fs_ViewStyle"];

				subItem = new ComponentArt.Web.UI.MenuItem();
				if (sCurrentView == "ListView")
				{
					subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
				}
				subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeViewTable, "");
				subItem.Text = LocRM.GetString("tListView");
				topMenuItem.Items.Add(subItem);

				subItem = new ComponentArt.Web.UI.MenuItem();
				if (sCurrentView == "DetailsView")
				{
					subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
				}
				subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeViewDet, "");
				subItem.Text = LocRM.GetString("tDetailsView");
				topMenuItem.Items.Add(subItem);

				subItem = new ComponentArt.Web.UI.MenuItem();
				if (sCurrentView == "Thumbnails")
				{
					subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
				}
				subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeViewThumb, "");
				subItem.Text = LocRM.GetString("tThumbnails");
				topMenuItem.Items.Add(subItem);
				#endregion

				secHeader.ActionsMenu.Items.Add(topMenuItem);

				topMenuItem = new ComponentArt.Web.UI.MenuItem();
				topMenuItem.Text = LocRM.GetString("tActions");
				topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
				topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
				topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
				topMenuItem.LookId = "TopItemLook";

				if (canWrite)
				{
					#region New File2
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/newfile.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					string commandLink = (Security.CurrentUser.IsExternal) ? "~/External/FileUpload.aspx" : "~/FileStorage/FileUpload.aspx";
					subItem.ClientSideCommand = String.Format("javascript:ShowWizard('{0}?ParentFolderId={1}&ContainerKey={2}&ContainerName={3}{4}', 470, 270);",
					  ResolveUrl(commandLink), _folderId, _containerKey, _containerName,
					  (Security.CurrentUser.IsExternal) ? ("&ExternalId=" + Security.CurrentUser.UserID) : "");
					subItem.Text = LocRM.GetString("tAddD");
					topMenuItem.Items.Add(subItem);
					#endregion

					if (!isExternal)
					{
						#region New Link
						subItem = new ComponentArt.Web.UI.MenuItem();
						subItem.Look.LeftIconUrl = "~/Layouts/Images/FileTypes/link.gif";
						subItem.Look.LeftIconWidth = Unit.Pixel(16);
						subItem.Look.LeftIconHeight = Unit.Pixel(16);
						subItem.ClientSideCommand = "javascript:ShowWizard('" + ResolveUrl("~/FileStorage/FileEdit.aspx") + "?LinkId=0&ParentFolderId=" + _folderId + "&ContainerKey=" + _containerKey + "&ContainerName=" + _containerName + "', 470, 230);";
						subItem.Text = LocRM.GetString("tAddL");
						topMenuItem.Items.Add(subItem);
						#endregion

						#region New Folder2
						subItem = new ComponentArt.Web.UI.MenuItem();
						subItem.Look.LeftIconUrl = "~/Layouts/Images/nfolder.gif";
						subItem.Look.LeftIconWidth = Unit.Pixel(16);
						subItem.Look.LeftIconHeight = Unit.Pixel(16);
						subItem.ClientSideCommand = "javascript:ShowWizard('" + ResolveUrl("~/FileStorage/DirectoryEdit.aspx") + "?ParentFolderId=" + _folderId + "&ContainerKey=" + _containerKey + "&ContainerName=" + _containerName + "', 500, 130);";
						subItem.Text = LocRM.GetString("tAdd");
						topMenuItem.Items.Add(subItem);
						#endregion
					}

					if (!isExternal)
					{
						subItem = new ComponentArt.Web.UI.MenuItem();
						subItem.LookId = "BreakItem";
						topMenuItem.Items.Add(subItem);

						#region Move Selected
						subItem = new ComponentArt.Web.UI.MenuItem();
						subItem.Look.LeftIconUrl = "~/Layouts/Images/moveto.gif";
						subItem.Look.LeftIconWidth = Unit.Pixel(16);
						subItem.Look.LeftIconHeight = Unit.Pixel(16);
						subItem.ClientSideCommand = "javascript:MoveChecked('" + _containerName + "','" + _containerKey + "')";
						subItem.Text = LocRM.GetString("MoveCheck");
						topMenuItem.Items.Add(subItem);
						#endregion

						#region Delete Selected
						subItem = new ComponentArt.Web.UI.MenuItem();
						subItem.Look.LeftIconUrl = "~/Layouts/Images/delete.gif";
						subItem.Look.LeftIconWidth = Unit.Pixel(16);
						subItem.Look.LeftIconHeight = Unit.Pixel(16);
						subItem.ClientSideCommand = "javascript:DeleteChecked()";
						subItem.Text = LocRM.GetString("DelCheck");
						topMenuItem.Items.Add(subItem);
						#endregion
					}
				}

				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.LookId = "BreakItem";
				topMenuItem.Items.Add(subItem);

				if (!isExternal)
				{
					#region Send Selected
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.Look.LeftIconUrl = "~/Layouts/Images/mail.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.ClientSideCommand = "javascript:SendChecked('" + _containerName + "','" + _containerKey + "')";
					subItem.Text = LocRM.GetString("tSendFiles");
					topMenuItem.Items.Add(subItem);
					#endregion

					#region Copy Selected
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.Look.LeftIconUrl = "~/Layouts/Images/xp-copy.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.ClientSideCommand = "javascript:CopyChecked()";
					subItem.Text = LocRM.GetString("CopyToClipboard");
					topMenuItem.Items.Add(subItem);
					#endregion

					if (canWrite)
					{
						#region Paste From Clip
						subItem = new ComponentArt.Web.UI.MenuItem();
						subItem.Look.LeftIconUrl = "~/Layouts/Images/xp-paste.gif";
						subItem.Look.LeftIconWidth = Unit.Pixel(16);
						subItem.Look.LeftIconHeight = Unit.Pixel(16);
						subItem.ClientSideCommand = "javascript:PasteFromClip('" + _containerName + "','" + _containerKey + "'," + _folderId + ")";
						subItem.Text = LocRM.GetString("tPasteFromClip");
						topMenuItem.Items.Add(subItem);
						#endregion
					}
				}

				if (canAdmin && !isExternal)
				{
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.LookId = "BreakItem";
					topMenuItem.Items.Add(subItem);

					#region Security
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.Look.LeftIconUrl = "~/Layouts/Images/icon-key.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.ClientSideCommand = "javascript:FolderSecurity('" + _containerName + "','" + _containerKey + "'," + _folderId + ")";
					subItem.Text = LocRM.GetString("tFolderSec");
					topMenuItem.Items.Add(subItem);
					#endregion
				}
				#endregion

				secHeader.ActionsMenu.Items.Add(topMenuItem);
			}
		}
		#endregion

		#region BindStorage
		private void BindStorage()
		{
			DataTable dt = new DataTable();
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
			dt.Columns.Add(new DataColumn("ActionVS", typeof(string)));
			dt.Columns.Add(new DataColumn("ActionEdit", typeof(string)));
			dt.Columns.Add(new DataColumn("CanDelete", typeof(bool)));
			dt.Columns.Add(new DataColumn("Size", typeof(string)));
			dt.Columns.Add(new DataColumn("sortSize", typeof(int)));
			dt.Columns.Add(new DataColumn("CopyLink", typeof(string)));
			DataRow dr;
			DirectoryInfo diCur = _fs.GetDirectory(_folderId);
			if (diCur.ParentDirectoryId > 0)
			{
				#region Parent Directory
				dr = dt.NewRow();
				dr["Id"] = diCur.Id;
				dr["Weight"] = 0;
				dr["Icon"] = ResolveUrl("~/layouts/images/blank.gif");
				dr["BigIcon"] = "";
				if (_pc["fs_ViewStyle"] == "Thumbnails")
				{
					dr["BigIcon"] = String.Format("<a href='{0}'><img src='{1}' align='absmiddle' border='0' /></a>",
						GetCurrentLink(diCur.ParentDirectoryId),
						ResolveUrl("~/layouts/images/back-up.gif"));
				}
				dr["Name"] = String.Format("<a href='{0}'>{1}</a>", GetCurrentLink(diCur.ParentDirectoryId), "[..]");
				dr["sortName"] = "";
				dr["CopyLink"] = "";
				dr["ActionVS"] = "";
				dr["ActionEdit"] = "";
				dr["CanDelete"] = false;
				dr["sortSize"] = 0;
				dt.Rows.Add(dr); 
				#endregion
			}
			bool isExternal = Security.CurrentUser.IsExternal;
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
					if (Mediachase.IBN.Business.Common.IsPop3Folder(di.Id))
						dr["Icon"] = ResolveUrl("~/layouts/images/folder_mailbox.gif");
					else
						dr["Icon"] = ResolveUrl("~/layouts/images/FileTypes/Folder.gif");
					dr["BigIcon"] = "";
					if (_pc["fs_ViewStyle"] == "Thumbnails")
					{
						dr["BigIcon"] = String.Format("<a href='{0}'><img src='{1}' align='absmiddle' border='0' /></a>",
							GetCurrentLink(di.Id),
							(Mediachase.IBN.Business.Common.IsPop3Folder(di.Id)) ?
							ResolveUrl("~/layouts/images/mailfolder.gif") :
							ResolveUrl("~/layouts/images/folder1.gif"));
						dr["Name"] = String.Format("<a href='{0}' title='{2}'>{1}</a>",
						  GetCurrentLink(di.Id),
						  (di.Name.Length > 27) ? di.Name.Substring(0, 24) + "..." : di.Name,
						  di.Name
						  );
					}
					else
						dr["Name"] = String.Format("<a href='{0}' title='{2}'>{1}</a>", GetCurrentLink(di.Id),
						  (di.Name.Length > 40) ? di.Name.Substring(0, 37) + "..." : di.Name, di.Name);
					dr["sortName"] = di.Name;
					dr["ModifiedDate"] = di.Modified.ToShortDateString();
					dr["sortModified"] = di.Modified;
					dr["Modifier"] = CommonHelper.GetUserStatus(di.ModifierId);
					dr["sortModifier"] = CommonHelper.GetUserStatusPureName(di.ModifierId);
					dr["sortSize"] = 0;
					if (canWrite && !isExternal)
						dr["ActionEdit"] = String.Format("<a href=\"{0}\"><img width='16' height='16' align='absmiddle' border='0' src='{1}' title='{2}'{3}/></a>",
							String.Format("javascript:ShowWizard('{0}?FolderId={1}&ContainerKey={2}&ContainerName={3}', 500, 130);", ResolveUrl("~/FileStorage/DirectoryEdit.aspx"), di.Id, _containerKey, _containerName),
							ResolveUrl("~/layouts/images/edit.gif"),
							LocRM.GetString("tEditF"),
							(_pc["fs_ViewStyle"] == "Thumbnails") ? " style='margin-top:5;margin-bottom:5'" : "");
					else
						dr["ActionEdit"] = "";
					if (_fs.CanUserAdmin(di.Id) && !isExternal)
						dr["ActionVS"] = String.Format("<a href=\"{0}\"><img width='16' height='16' align='absmiddle' border='0' src='{1}' title='{2}'/></a>",
							String.Format("javascript:FolderSecurity('{0}','{1}',{2});", _containerName, _containerKey, di.Id),
							ResolveUrl("~/layouts/images/icon-key.gif"),
							LocRM.GetString("tFolderSec"));
					else
						dr["ActionVS"] = "";
					dr["CopyLink"] = "";
					dr["CanDelete"] = canWrite && !isExternal;
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
					dr["Icon"] = ResolveUrl("~/Common/ContentIcon.aspx?IconID=" + fi.FileBinaryContentTypeId);
					dr["BigIcon"] = "";
					string sLink = "";
					string sPublicLink = "";
					if (fi.FileBinaryContentType.ToLower().IndexOf("url") >= 0)
					{
						sLink = CommonHelper.GetLinkText(_fs, fi);
						sPublicLink = sLink;
					}
					if (sLink == "")
					{
						sLink = CommonHelper.GetAbsoluteDownloadFilePath(fi.Id, fi.Name, _containerName, _containerKey);
						sPublicLink = CommonHelper.GetAbsolutePublicDownloadFilePath(fi.Id, fi.Name, _containerName, _containerKey);
					}

					string sNameLocked = CommonHelper.GetLockerText(sLink);
					if (String.IsNullOrEmpty(sNameLocked) &&
						!Mediachase.IBN.Business.ControlSystem.FileStorage.CanUserWrite(Mediachase.IBN.Business.Security.CurrentUser.UserID, _containerKey, diCur.Id))
						sNameLocked = String.Concat("<span style='color:#858585;font-size:7pt;'>[", LocRM2.GetString("ReadOnly"), "]</span>");

					if (_pc["fs_ViewStyle"] == "Thumbnails")
					{
						dr["BigIcon"] = String.Format("<a href=\"{0}\"{2}>{1}</a>",
							sLink,
							String.Format("<img src='{0}' align='absmiddle' border='0' />",
							String.Format(ResolveUrl("~/Common/ContentIcon.aspx?Big=1&IconID={0}&ContainerKey={1}&FileId={2}"), fi.FileBinaryContentTypeId, _containerKey, fi.Id)),
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
					if (canWrite && !isExternal)
						dr["ActionEdit"] = String.Format("<a href=\"{0}\"><img width='16' height='16' align='absmiddle' border='0' src='{1}' title='{2}'{3}/></a>",
							String.Format("javascript:ShowWizard('{0}?FileId={1}&ContainerKey={2}&ContainerName={3}', 470, 200);", ResolveUrl("~/FileStorage/FileEdit.aspx"), fi.Id, _containerKey, _containerName),
							ResolveUrl("~/layouts/images/edit.gif"),
							LocRM.GetString("tEditDoc"),
							(_pc["fs_ViewStyle"] == "Thumbnails") ? " style='margin-top:5;margin-bottom:5'" : "");
					else
						dr["ActionEdit"] = "";
					if (!isExternal)
						dr["ActionVS"] = String.Format("<a href=\"{0}\"><img width='16' height='16' align='absmiddle' border='0' src='{1}' title='{2}'/></a>",
							String.Format("javascript:ViewFile('{0}','{1}',{2});", _containerName, _containerKey, fi.Id),
							ResolveUrl("~/layouts/images/icon-search.gif"),
							LocRM.GetString("tViewDetails"));
					dr["CopyLink"] = String.Format("<a href=\"javascript:copy('{0}')\"><img width='16' height='16' align='absmiddle' border='0' src='{1}' title='{2}'/></a>",
						sPublicLink, ResolveClientUrl("~/layouts/images/copyLink.png"), LocRM.GetString("tCopyLink"));
					dr["CanDelete"] = canWrite && !isExternal;
					dr["Size"] = FormatBytes((long)fi.Length);
					dr["sortSize"] = fi.Length;
					dt.Rows.Add(dr); 
					#endregion
				}
			}
			DataView dv = dt.DefaultView;
			dv.Sort = "Weight, " + _pc["fs_Sort_" + _containerKey].ToString();

			if (_pc["fs_ViewStyle"] == "ListView")
			{
				#region ListView
				grdMain.Visible = true;
				grdDetails.Visible = false;
				repThumbs.Visible = false;

				int i = 4;
				grdMain.Columns[i++].HeaderText = LocRM.GetString("tName");
				grdMain.Columns[i++].HeaderText = LocRM.GetString("UpdatedBy");
				grdMain.Columns[i++].HeaderText = LocRM.GetString("UpdatedDate");
				grdMain.Columns[i++].HeaderText = LocRM.GetString("Size");

				foreach (DataGridColumn dgc in grdMain.Columns)
				{
					if (dgc.SortExpression == _pc["fs_Sort_" + _containerKey].ToString())
						dgc.HeaderText += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>",
							ResolveUrl("~/layouts/images/upbtnF.jpg"));
					else if (dgc.SortExpression + " DESC" == _pc["fs_Sort_" + _containerKey].ToString())
						dgc.HeaderText += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>",
							ResolveUrl("~/layouts/images/downbtnF.jpg"));
				}

				if (_pc["fs_PageSize_" + _containerKey] == null)
					_pc["fs_PageSize_" + _containerKey] = "10";

				grdMain.PageSize = int.Parse(_pc["fs_PageSize_" + _containerKey].ToString());

				if (_pc["fs_Page_" + _containerKey] == null)
					_pc["fs_Page_" + _containerKey] = "0";
				int pageIndex = int.Parse(_pc["fs_Page_" + _containerKey].ToString());
				int ppi = dv.Count / grdMain.PageSize;
				if (dv.Count % grdMain.PageSize == 0)
					ppi = ppi - 1;
				if (pageIndex <= ppi)
				{
					grdMain.CurrentPageIndex = pageIndex;
				}
				else
				{
					grdMain.CurrentPageIndex = 0;
					_pc["fs_Page_" + _containerKey] = "0";
				}

				grdMain.DataSource = dv;
				grdMain.DataBind();
				foreach (DataGridItem dgi in grdMain.Items)
				{
					ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
					if (ib != null)
					{
						if (dgi.Cells[1].Text == "2")
						{
							ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "')");
							ib.ToolTip = LocRM.GetString("tDelDoc");
						}
						else
						{
							ib.ToolTip = LocRM.GetString("tDeleteF");
							if (Mediachase.IBN.Business.Common.IsPop3Folder(int.Parse(dgi.Cells[0].Text)))
								ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("MesDeletePop3F") + "')");
							else
								ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("MesDeleteF") + "')");
						}
					}
				}
				#endregion
			}
			else if (_pc["fs_ViewStyle"] == "DetailsView")
			{
				#region DetailsView
				grdMain.Visible = false;
				grdDetails.Visible = true;
				repThumbs.Visible = false;

				if (_pc["fs_PageSize_" + _containerKey] == null)
					_pc["fs_PageSize_" + _containerKey] = "10";

				grdDetails.PageSize = int.Parse(_pc["fs_PageSize_" + _containerKey].ToString());

				if (_pc["fs_Page_" + _containerKey] == null)
					_pc["fs_Page_" + _containerKey] = "0";
				int pageIndex = int.Parse(_pc["fs_Page_" + _containerKey].ToString());
				int ppi = dv.Count / grdDetails.PageSize;
				if (dv.Count % grdDetails.PageSize == 0)
					ppi = ppi - 1;
				if (pageIndex <= ppi)
				{
					grdDetails.CurrentPageIndex = pageIndex;
				}
				else
				{
					grdDetails.CurrentPageIndex = 0;
					_pc["fs_Page_" + _containerKey] = "0";
				}

				grdDetails.DataSource = dv;
				grdDetails.DataBind();
				foreach (DataGridItem dgi in grdDetails.Items)
				{
					ImageButton ib = (ImageButton)dgi.FindControl("ibDelete2");
					if (ib != null)
					{
						if (dgi.Cells[1].Text == "2")
						{
							ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "')");
							ib.ToolTip = LocRM.GetString("tDelDoc");
						}
						else
						{
							ib.ToolTip = LocRM.GetString("tDeleteF");
							if (Mediachase.IBN.Business.Common.IsPop3Folder(int.Parse(dgi.Cells[0].Text)))
								ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("MesDeletePop3F") + "')");
							else
								ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("MesDeleteF") + "')");
						}
					}
				}
				#endregion
			}
			else if (_pc["fs_ViewStyle"] == "Thumbnails")
			{
				#region Thumbnails
				grdMain.Visible = false;
				grdDetails.Visible = false;
				repThumbs.Visible = true;

				repThumbs.DataSource = dv;
				repThumbs.DataBind();
				foreach (RepeaterItem repItem in repThumbs.Items)
				{
					ImageButton ib = (ImageButton)repItem.FindControl("ibDelete1");
					if (ib != null)
					{
						string sIds = ib.CommandArgument;
						string sWeight = sIds.Substring(0, sIds.IndexOf("_"));
						string sId = sIds.Substring(sIds.IndexOf("_") + 1);
						if (sWeight == "2")
						{
							ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "')");
							ib.ToolTip = LocRM.GetString("tDelDoc");
						}
						else
						{
							ib.ToolTip = LocRM.GetString("tDeleteF");
							if (Mediachase.IBN.Business.Common.IsPop3Folder(int.Parse(sId)))
								ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("MesDeletePop3F") + "')");
							else
								ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("MesDeleteF") + "')");
						}
					}
				}
				#endregion
			}
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
					sPath = String.Format("<a href='{0}' title=\"{2}\">{1}</a>", GetCurrentLink(iFolder), sFolderName, sFullFolderName);
				else
					sPath = String.Format("<a href='{0}' title=\"{2}\">{1}</a> \\ {3}", GetCurrentLink(iFolder), sFolderName, sFullFolderName, sPath);
				if (iFolder == _rootId)
					break;
				iFolder = di.ParentDirectoryId;
			}
			lblPath.Text = sPath;
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
			this.lbDeleteChecked.Click += new EventHandler(lbDeleteChecked_Click);
			this.lbChangeViewTable.Click += new EventHandler(lbChangeViewTable_Click);
			this.lbChangeViewThumb.Click += new EventHandler(lbChangeViewThumb_Click);
			this.lbChangeViewDet.Click += new EventHandler(lbChangeViewDet_Click);
			this.grdMain.SortCommand += new DataGridSortCommandEventHandler(grdMain_SortCommand);
			this.grdMain.DeleteCommand += new DataGridCommandEventHandler(grdMain_DeleteCommand);
			this.grdMain.PageIndexChanged += new DataGridPageChangedEventHandler(grdMain_PageIndexChanged);
			this.grdMain.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(grdMain_PageSizeChanged);
			this.grdDetails.SortCommand += new DataGridSortCommandEventHandler(grdMain_SortCommand);
			this.grdDetails.DeleteCommand += new DataGridCommandEventHandler(grdMain_DeleteCommand);
			this.grdDetails.PageIndexChanged += new DataGridPageChangedEventHandler(grdMain_PageIndexChanged);
			this.grdDetails.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(grdMain_PageSizeChanged);
			this.grdDetails.ItemDataBound += new DataGridItemEventHandler(grdDetails_ItemDataBound);
			this.repThumbs.ItemCommand += new RepeaterCommandEventHandler(repThumbs_ItemCommand);
		}
		#endregion

		#region DataGrid Events
		private void grdMain_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (_pc["fs_Sort_" + _containerKey].ToString() == (string)e.SortExpression)
				_pc["fs_Sort_" + _containerKey] = (string)e.SortExpression + " DESC";
			else
				_pc["fs_Sort_" + _containerKey] = (string)e.SortExpression;
		}

		private void grdMain_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			_pc["fs_Page_" + _containerKey] = e.NewPageIndex.ToString();
		}

		private void grdMain_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			_pc["fs_PageSize_" + _containerKey] = e.NewPageSize.ToString();
		}

		private void grdMain_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int delType = int.Parse(e.Item.Cells[1].Text);
			int delId = int.Parse(e.Item.Cells[0].Text);
			try
			{
				switch (delType)
				{
					case 1:
						_fs.DeleteFolder(delId);
						break;
					case 2:
						_fs.DeleteFile(delId);
						break;
				}
			}
			catch { }
			Response.Redirect(GetCurrentLink(_folderId));
		}

		void grdDetails_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Header)
			{
				foreach (Control c in e.Item.Controls[4].Controls)
				{
					if (c is LinkButton)
					{
						LinkButton lb = (LinkButton)c;
						switch (lb.CommandArgument)
						{
							case "sortName":
								lb.Text = LocRM.GetString("tName");
								break;
							case "sortModifier":
								lb.Text = LocRM.GetString("UpdatedBy");
								break;
							case "sortModified":
								lb.Text = LocRM.GetString("UpdatedDate");
								break;
							case "sortSize":
								lb.Text = LocRM.GetString("Size");
								break;
							default:
								break;
						}
						if (lb.CommandArgument == _pc["fs_Sort_" + _containerKey].ToString())
							lb.Text += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>",
								ResolveUrl("~/layouts/images/upbtnF.jpg"));
						else if (lb.CommandArgument + " DESC" == _pc["fs_Sort_" + _containerKey].ToString())
							lb.Text += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>",
								ResolveUrl("~/layouts/images/downbtnF.jpg"));
					}
				}
			}
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

		#region Delete Event
		private void lbDeleteChecked_Click(object sender, EventArgs e)
		{
			string sIds = hidForDelete.Value;
			ArrayList alIds = new ArrayList();
			while (sIds.Length > 0)
			{
				alIds.Add(sIds.Substring(0, sIds.IndexOf(",")));
				sIds = sIds.Remove(0, sIds.IndexOf(",") + 1);
			}
			foreach (string scId in alIds)
			{
				string sWeight = scId.Substring(0, scId.IndexOf("_"));
				string sId = scId.Substring(scId.IndexOf("_") + 1);
				if (sWeight == "1")
				{
					int id = int.Parse(sId);
					_fs.DeleteFolder(id);
				}
				if (sWeight == "2")
				{
					int assetID = int.Parse(sId);
					_fs.DeleteFile(assetID);
				}
			}
			Response.Redirect(GetCurrentLink(_folderId));
		}
		#endregion

		#region Change View
		private void lbChangeViewTable_Click(object sender, EventArgs e)
		{
			_pc["fs_ViewStyle"] = "ListView";
			Response.Redirect(GetCurrentLink(_folderId));
		}

		private void lbChangeViewThumb_Click(object sender, EventArgs e)
		{
			_pc["fs_ViewStyle"] = "Thumbnails";
			Response.Redirect(GetCurrentLink(_folderId));
		}

		private void lbChangeViewDet_Click(object sender, EventArgs e)
		{
			_pc["fs_ViewStyle"] = "DetailsView";
			Response.Redirect(GetCurrentLink(_folderId));
		}
		#endregion

		#region Repeater Delete
		private void repThumbs_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandName == "Delete")
			{
				string sIds = e.CommandArgument.ToString();
				string sWeight = sIds.Substring(0, sIds.IndexOf("_"));
				string sId = sIds.Substring(sIds.IndexOf("_") + 1);
				try
				{
					switch (sWeight)
					{
						case "1":
							_fs.DeleteFolder(int.Parse(sId));
							break;
						case "2":
							_fs.DeleteFile(int.Parse(sId));
							break;
					}
				}
				catch { }
				Response.Redirect(GetCurrentLink(_folderId));
			}
		}
		#endregion

		#region GetCurrentLink
		private string GetCurrentLink(int iFolderId)
		{
			string sPath = HttpContext.Current.Request.Url.LocalPath;
			if (Request["ProjectId"] != null)
				sPath += String.Format("?ProjectId={0}&FolderId={1}&SubTab=0", Request["ProjectId"], iFolderId);
			else if (Request["IncidentId"] != null)
				sPath += String.Format("?IncidentId={0}&FolderId={1}&SubTab=0", Request["IncidentId"], iFolderId);
			else if (Request["TaskId"] != null)
				sPath += String.Format("?TaskId={0}&FolderId={1}&SubTab=0", Request["TaskId"], iFolderId);
			else if (Request["ToDoId"] != null)
				sPath += String.Format("?ToDoId={0}&FolderId={1}&SubTab=0", Request["ToDoId"], iFolderId);
			else if (Request["EventId"] != null)
				sPath += String.Format("?EventId={0}&FolderId={1}&SubTab=0", Request["EventId"], iFolderId);
			else if (Request["DocumentId"] != null)
				sPath += String.Format("?DocumentId={0}&FolderId={1}&SubTab=0", Request["DocumentId"], iFolderId);
			else
				sPath += String.Format("?FolderId={0}&Tab=0", iFolderId);
			return sPath;
		}
		#endregion
	}
}
