using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Lists;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;


namespace Mediachase.Ibn.Web.UI.ListApp.Modules.ManageControls
{
	public partial class ListInfoList : System.Web.UI.UserControl
	{
		private UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
		private int _folderId = 0;
		private const int _dialogWidth = 550;
		private const int _dialogHeight = 470;

		#region ListFolderId
		private int ListFolderId
		{
			get
			{
				int value;
				if (int.TryParse(Request["ListFolderId"], NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
					return value;
				else
					return -1;
			}
		}
		#endregion

		#region ProjectId
		private int ProjectId
		{
			get
			{
				int value;
				if (int.TryParse(Request["ProjectId"], NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
					return value;
				else
					return -1;
			}
		}
		#endregion

		#region IsProject
		private bool _isProject = false;
		public bool IsProject
		{
			get { return _isProject; }
			set { _isProject = value; }
		} 
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			DefineFolderId();

			if (!Page.IsPostBack)
			{
				if (!ListManager.CanReadFolder(_folderId))
					throw new AccessDeniedException();

				if (pc["Lists_FV_Sort"] == null)
					pc["Lists_FV_Sort"] = "sortName";

				if (Request["Assign"] != null && Request["Assign"] == "1")
				{
					string listClass = "List_" + Request["ListId"];
					Dictionary<string, string> dic = new Dictionary<string,string>();
					dic.Add("ClassName", listClass);
					CommandParameters cp = new CommandParameters("MC_ListApp_Security", dic);
					CommandManager cm = CommandManager.GetCurrent(this.Page);
					ClientScript.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString("N"),
						cm.AddCommand("", "", "ListInfoList", cp), true);
				}
			}
			BindToolBar();

			ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(btnRefresh);
		}

		#region DefineFolderId
		private void DefineFolderId()
		{
			_folderId = ListFolderId;

			if (Request["IsProject"] != null && Request["IsProject"] == "1")
				_folderId = ListManager.GetProjectRoot(ListFolderId).PrimaryKeyId.Value;

			if (IsProject)
			{
				if (_folderId < 0 && ProjectId > 0)
					_folderId = ListManager.GetProjectRoot(ProjectId).PrimaryKeyId.Value;
			}
		} 
		#endregion

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			if (IsProject)
			{
				secHeader.Visible = false;
				if(mainTable.Attributes["class"] != null)
					mainTable.Attributes.Remove("class");
			}
			BindList();
		}

		#region BindList
		private void BindList()
		{
			int i = 3;
			grdMain.Columns[i++].HeaderText = CHelper.GetResFileString("{IbnFramework.ListInfo:tName}");
			grdMain.Columns[i++].HeaderText = CHelper.GetResFileString("{IbnFramework.ListInfo:Type}");
			grdMain.Columns[i++].HeaderText = CHelper.GetResFileString("{IbnFramework.ListInfo:Status}");
			grdMain.Columns[i++].HeaderText = CHelper.GetResFileString("{IbnFramework.ListInfo:CreatedBy}");
			grdMain.Columns[i++].HeaderText = CHelper.GetResFileString("{IbnFramework.ListInfo:Created}");

			foreach (DataGridColumn dgc in grdMain.Columns)
			{
				if (dgc.SortExpression == pc["Lists_FV_Sort"].ToString())
					dgc.HeaderText += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>", ResolveUrl("~/layouts/images/upbtnF.jpg"));
				else if (dgc.SortExpression + " DESC" == pc["Lists_FV_Sort"].ToString())
					dgc.HeaderText += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>", ResolveUrl("~/layouts/images/downbtnF.jpg"));
			}

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("ObjectId", typeof(int)));
			dt.Columns.Add(new DataColumn("Type", typeof(string)));
			dt.Columns.Add(new DataColumn("Icon", typeof(string)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("TypeName", typeof(string)));
			dt.Columns.Add(new DataColumn("StatusName", typeof(string)));
			dt.Columns.Add(new DataColumn("CreatorName", typeof(string)));
			dt.Columns.Add(new DataColumn("CreationDate", typeof(DateTime)));

			dt.Columns.Add(new DataColumn("ActionMove", typeof(string)));
			dt.Columns.Add(new DataColumn("ActionSecurity", typeof(string)));
			dt.Columns.Add(new DataColumn("ActionEdit", typeof(string)));
			dt.Columns.Add(new DataColumn("ActionDelete", typeof(string)));

			dt.Columns.Add(new DataColumn("sortName", typeof(string)));
			dt.Columns.Add(new DataColumn("sortCreator", typeof(string)));

			string link = "";
			string sPath = ResolveUrl("~/Apps/ListApp/Pages/ListInfoList.aspx?Tab=0");
			if (ProjectId > 0)
			{
				link = "ProjectId=" + ProjectId.ToString();
				sPath = ResolveUrl(String.Format("~/Projects/ProjectView.aspx?{0}", link));
			}

			DataRow dr;
			DataRow dr_bas = dt.NewRow();
			bool fl_UseBas = false;
			int iParentFolderId = 0;
			bool IsPrjFolder = false;
			ListFolder folder = null;
			
			if (_folderId == -1)
				IsPrjFolder = true;
			else
			{
				folder = new ListFolder(_folderId);
				iParentFolderId = (folder.ParentId.HasValue)? (int)folder.ParentId.Value : 0;
				IsPrjFolder = (folder.FolderType == ListFolderType.Project);
			}
			if ((iParentFolderId > 0 || (IsPrjFolder && _folderId != -1)) &&
				(ProjectId < 0 || (ProjectId > 0 && _folderId != ListManager.GetProjectRoot(ProjectId).PrimaryKeyId.Value)))
			{
				if (IsPrjFolder && iParentFolderId == 0)
					iParentFolderId = -1;
				fl_UseBas = true;
				dr_bas["ObjectId"] = iParentFolderId;
				dr_bas["Icon"] = ResolveUrl("~/layouts/images/blank.gif");
				dr_bas["Name"] = String.Format("<a href='{1}&ListFolderId={0}'>[..]</a>", iParentFolderId.ToString(), sPath);
			}

			if (folder == null)
			{
				using (IDataReader reader = Project.GetListProjects())
				{
					while (reader.Read())
					{
						AddFolderItem(dt, link, sPath, (int)reader["ProjectId"], reader["Title"].ToString(),
							(int)reader["CreatorId"], (DateTime)reader["CreationDate"], true);
					}
				}
			}
			else
			{
				foreach (Mediachase.Ibn.Data.Services.TreeNode tN in Mediachase.Ibn.Data.Services.TreeManager.GetChildNodes(folder))
				{
					MetaObject mo = tN.InnerObject;
					AddFolderItem(dt, link, sPath, mo.PrimaryKeyId.Value, tN.Title, (int)mo.Properties["CreatorId"].Value, (DateTime)mo.Properties["Created"].Value, false);
				}
			}

			string sFilter = pc["Lists_FV_Sort"].ToString();
			if (!(sFilter == "sortName") || (sFilter == "sortName DESC"))
				sFilter += ",sortName";
			DataRow[] drFolders = dt.Select("", sFilter);
			DataTable dtLists = dt.Clone();
			foreach (ListInfo li in ListManager.GetLists(_folderId))
			{
				int iListId = li.PrimaryKeyId.Value;
				dr = dtLists.NewRow();
				dr["ObjectId"] = iListId;
				dr["Type"] = "List";
				dr["Icon"] = ResolveUrl("~/layouts/images/lists.gif");
				dr["Name"] = String.Format("<a href='{3}?ClassName={0}{1}&ListFolderId={4}'>{2}</a>",
					ListManager.GetListMetaClassName(iListId), (String.IsNullOrEmpty(link) ? link : "&" + link), li.Title,
					ResolveUrl("~/Apps/MetaUIEntity/Pages/EntityList.aspx"),
					_folderId);
				dr["sortName"] = li.Title;
				dr["sortCreator"] = Mediachase.UI.Web.Util.CommonHelper.GetUserStatusPureName(li.CreatorId);
				dr["TypeName"] = (li.Properties["ListType"].Value != null) ? CHelper.GetResFileString(MetaEnum.GetFriendlyName(DataContext.Current.MetaModel.RegisteredTypes[ListManager.ListTypeEnumName], (int)li.Properties["ListType"].Value)) : "";
				dr["StatusName"] = (li.Properties["Status"].Value != null) ? CHelper.GetResFileString(MetaEnum.GetFriendlyName(DataContext.Current.MetaModel.RegisteredTypes[ListManager.ListStatusEnumName], (int)li.Properties["Status"].Value)) : "";
				dr["CreatorName"] = Mediachase.UI.Web.Util.CommonHelper.GetUserStatus(li.CreatorId);
				dr["CreationDate"] = li.Created;

				if(ListInfoBus.CanAdmin(iListId))
				{
					if (folder.FolderType != ListFolderType.Private)
					{
						string sPrjId = "";
						if (ProjectId > 0)
							sPrjId = "," + ProjectId.ToString();
					}


					string className = ListManager.GetListMetaClassName(iListId);
					string moveLink = String.Format("javascript:OpenSizableWindow(\"{3}?ListId={0}{2}&btn={1}\", {4}, {5});",
						iListId, btnRefresh.UniqueID, (String.IsNullOrEmpty(link) ? link : "&" + link),
						ResolveClientUrl("~/Apps/ListApp/Pages/ListInfoMove.aspx"), _dialogWidth, _dialogHeight);

					dr["ActionMove"] = String.Format("<a href='{0}'><img alt='' src='{2}' width='12' height='12' border='0' align='absmiddle' title='{1}'></a>",
						moveLink, CHelper.GetResFileString("{IbnFramework.ListInfo:MoveTo}"),
						ResolveUrl("~/layouts/images/MoveTo.gif"));

					Dictionary<string, string> prms = new Dictionary<string, string>();
					prms.Add("ClassName", className);
					string securityLink = String.Format("javascript:{{{0}}}", CommandManager.GetCurrent(this.Page).AddCommand("", "", "ListInfoList", "MC_ListApp_Security", prms).Replace("\"", "&quot;"));
					dr["ActionSecurity"] = String.Format("<a href=\"{0}\"><img alt='' src='{1}' width='16' height='16' border='0' align='absmiddle' title='{2}'></a>",
						securityLink,
						ResolveUrl("~/Layouts/Images/icon-key.gif"),
						CHelper.GetResFileString("{IbnFramework.ListInfo:Security}"));

					dr["ActionEdit"] = String.Format("<a href='{3}?ListFolderId={0}&class={5}{2}'><img alt='' src='{4}' width='16' height='16' border='0'  align='absmiddle' title='{1}'></a>",
						folder.PrimaryKeyId.Value.ToString(),
						CHelper.GetResFileString("{IbnFramework.ListInfo:ListManagement}"), (String.IsNullOrEmpty(link) ? link : "&" + link),
						ResolveUrl(CHelper.ListAdminPage),
						ResolveUrl("~/layouts/images/customize.gif"),
						className);
					//dr["ActionDelete"] = "<a href='javascript:DeleteList(" + iListId.ToString() + ")'><img src='../layouts/images/delete.gif' width='16' height='16' border=0  align='absmiddle' title='" + CHelper.GetResFileString("{IbnFramework.ListInfo:Delete}") + "' ></a>";
					//dr["ActionDelete"] = String.Format("<img alt='' src='{1}' width='16' height='16' border='0' align='absmiddle' title='{0}'>",
					//        CHelper.GetResFileString("{IbnFramework.ListInfo:Delete}"),
					//        ResolveUrl("~/layouts/images/delete.gif"));

					dr["ActionDelete"] = String.Format("<a href='javascript:DeleteList({2})'><img alt='' src='{1}' width='16' height='16' border='0' align='absmiddle' title='{0}'></a>",
						CHelper.GetResFileString("{IbnFramework.ListInfo:Delete}"),
						ResolveUrl("~/layouts/images/delete.gif"), iListId.ToString());

				}
				dtLists.Rows.Add(dr);
			}
			
			DataRow[] drLists = dtLists.Select("", sFilter);

			DataTable dtResult = dt.Clone();

			if (fl_UseBas)
			{
				DataRow _dr = dtResult.NewRow();
				_dr.ItemArray = (Object[])dr_bas.ItemArray.Clone();
				dtResult.Rows.Add(_dr);
			}
			foreach (DataRow dr1 in drFolders)
			{
				DataRow _dr = dtResult.NewRow();
				_dr.ItemArray = dr1.ItemArray;
				dtResult.Rows.Add(_dr);
			}
			foreach (DataRow dr2 in drLists)
			{
				DataRow _dr = dtResult.NewRow();
				_dr.ItemArray = dr2.ItemArray;
				dtResult.Rows.Add(_dr);
			}

			DataView dv = dtResult.DefaultView;
			
			if (pc["Lists_FV_PageSize"] != null)
				grdMain.PageSize = int.Parse(pc["Lists_FV_PageSize"]);

			int iPageIndex = 0;
			if (pc["Lists_FV_Page"] != null)
			{
				iPageIndex = int.Parse(pc["Lists_FV_Page"]);
			}

			int ppi = dtResult.Rows.Count / grdMain.PageSize;
			if (dtResult.Rows.Count % grdMain.PageSize == 0)
				ppi = ppi - 1;
			if (iPageIndex <= ppi)
				grdMain.CurrentPageIndex = iPageIndex;
			else grdMain.CurrentPageIndex = 0;

			grdMain.DataSource = dv;
			grdMain.DataBind();
		}
		#endregion

		#region AddFolderItem
		private void AddFolderItem(DataTable dt, string link, string sPath,
			int iFolderId, string folderTitle, int creatorId, DateTime created, bool isProject)
		{
			DataRow dr = dt.NewRow();
			dr["ObjectId"] = iFolderId;
			dr["Type"] = "Folder";
			dr["Icon"] = ResolveUrl("~/layouts/images/Folder.gif");
			string sName = folderTitle;
			if (sName.Length >= 30)
				sName = sName.Substring(0, 27) + "...";

			dr["Name"] = String.Format("<a href='{0}{3}&ListFolderId={1}'>{2}</a>", 
				sPath, iFolderId.ToString(), sName, isProject ? "&IsProject=1" : "");
			dr["sortName"] = folderTitle;
			dr["sortCreator"] = Mediachase.UI.Web.Util.CommonHelper.GetUserStatusPureName(creatorId);
			dr["CreatorName"] = Mediachase.UI.Web.Util.CommonHelper.GetUserStatus(creatorId);
			dr["CreationDate"] = created;
			dr["ActionMove"] = "";
			dr["ActionSecurity"] = "";
			if (!isProject && ListManager.CanUpdateFolder(iFolderId))
			{
				dr["ActionEdit"] = String.Format("<a href='{3}?ListFolderId={0}{2}'><img alt='' src='{4}' width='16' height='16' border='0'  align='absmiddle' title='{1}'></a>",
					iFolderId.ToString(), CHelper.GetResFileString("{IbnFramework.ListInfo:Edit}"), (String.IsNullOrEmpty(link) ? link : "&" + link),
					ResolveUrl("~/Apps/ListApp/Pages/ListFolderEdit.aspx"),
					ResolveUrl("~/layouts/images/edit.gif"));
			}
			if (!isProject && ListManager.CanDeleteFolder(iFolderId))
			{
				dr["ActionDelete"] = String.Format("<a href='javascript:DeleteFolder({2})'><img alt='' src='{1}' width='16' height='16' border='0' align='absmiddle' title='{0}'></a>",
					CHelper.GetResFileString("{IbnFramework.ListInfo:Delete}"), 
					ResolveUrl("~/layouts/images/delete.gif"), iFolderId.ToString());
			}
			dt.Rows.Add(dr);
		}
		#endregion

		#region BindToolBar
		private void BindToolBar()
		{
			string sTitle = CHelper.GetResFileString("{IbnFramework.ListInfo:tLists}");
			if (ListFolderId == -1)
			{
				sTitle = CHelper.GetResFileString("{IbnFramework.ListInfo:tPrjLists}");
			}
			else if (ProjectId <= 0)
			{
				ListFolder folder = new ListFolder(_folderId);
				int iParentFolderId = (folder.ParentId.HasValue) ? (int)folder.ParentId.Value : 0;
				if (iParentFolderId == 0 && folder.FolderType != ListFolderType.Project)
				{
					if (folder.FolderType == ListFolderType.Private)
						sTitle = CHelper.GetResFileString("{IbnFramework.ListInfo:tPrivateLists}");
					else
						sTitle = CHelper.GetResFileString("{IbnFramework.ListInfo:tPublicLists}");
				}
				else if (iParentFolderId == 0)
				{
					int projectId = ListFolderId;
					if (folder.ProjectId.HasValue)
						projectId = folder.ProjectId.Value;
					sTitle = Task.GetProjectTitle(projectId);
				}
			}

			string link1 = "~/Apps/ListApp/Pages/ListFolderEdit.aspx?parentFolderId=" + _folderId.ToString();
			if (ProjectId > 0)
				link1 += "&ProjectId=" + ProjectId.ToString();

			string link2 = "~/Apps/ListApp/Pages/ListInfoCreate.aspx?ListFolderId=" + _folderId.ToString();
			if (ProjectId > 0)
				link2 += "&ProjectId=" + ProjectId.ToString();

			if (this.Parent is Mediachase.UI.Web.Modules.IToolbarLight)
			{
				Mediachase.UI.Web.Modules.BlockHeaderLightWithMenu parentHeaderTrue = ((Mediachase.UI.Web.Modules.IToolbarLight)this.Parent).GetToolBar();
//				parentHeaderTrue.AddText(CHelper.GetResFileString("{IbnFramework.ListInfo:tLists}"));
				BindMenu(parentHeaderTrue.ActionsMenu, link1, link2);
			}
			else
			{
				secHeader.Title = sTitle;

				if (ListFolderId != -1)
					BindMenu(secHeader.ActionsMenu, link1, link2);
			}
		}
		#endregion

		#region BindMenu
		private void BindMenu(ComponentArt.Web.UI.Menu menu, string link1, string link2)
		{
			ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:tActions}");
			topMenuItem.DefaultSubGroupExpandDirection = ComponentArt.Web.UI.GroupExpandDirection.BelowRight;
			topMenuItem.Look.LeftIconUrl = "~/Layouts/Images/downbtn1.gif";
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			ComponentArt.Web.UI.MenuItem subItem;

			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Look.LeftIconUrl = "~/Layouts/Images/nfolder.gif";
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			subItem.NavigateUrl = link1;
			subItem.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:tAddFolder}");
			topMenuItem.Items.Add(subItem);

			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Look.LeftIconUrl = "~/Layouts/Images/listsnew.gif";
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			subItem.NavigateUrl = link2;
			subItem.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:tAddList}");
			topMenuItem.Items.Add(subItem);

			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Look.LeftIconUrl = "~/Layouts/Images/import.gif";
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = new CommandParameters("MC_ListApp_ImportWizard");
			cp.CommandArguments = new Dictionary<string, string>();
			cp.AddCommandArgument("ListFolderId", _folderId.ToString());
			subItem.ClientSideCommand = cm.AddCommand("", "", "ListInfoList", cp);
			subItem.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:ImportListMenu}");
			topMenuItem.Items.Add(subItem);

			menu.Items.Add(topMenuItem);
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
			this.grdMain.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_PageIndexChanged);
			this.grdMain.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.grdMain_Sort);
			this.grdMain.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dg_PageSizeChanged);
			this.lbDelFolder.Click += new EventHandler(lbDelFolder_Click);
			this.lbDelList.Click += new EventHandler(lbDelList_Click);
		}
		#endregion

		#region btnRefresh_Click
		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			string param = Request.Params.Get("__EVENTARGUMENT");
			if (String.IsNullOrEmpty(param))
				return;
		}
		#endregion

		#region DataGrid_Events
		private void grdMain_Sort(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if (pc["Lists_FV_Sort"].ToString() == (string)e.SortExpression)
				pc["Lists_FV_Sort"] = (string)e.SortExpression + " DESC";
			else
				pc["Lists_FV_Sort"] = (string)e.SortExpression;
		}

		private void dg_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["Lists_FV_PageSize"] = e.NewPageSize.ToString();
		}

		private void dg_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			pc["Lists_FV_Page"] = e.NewPageIndex.ToString();
		}
		#endregion

		#region lbDelFolder_Click
		private void lbDelFolder_Click(object sender, System.EventArgs e)
		{
			//try
			//{
				ListManager.DeleteFolder(int.Parse(deletedId.Value));
			//}
			//catch(Exception ex)
			//{
			//    System.Diagnostics.Trace.WriteLine(ex);
			//}
			if (ProjectId < 0)
				Response.Redirect("~/Apps/ListApp/Pages/ListInfoList.aspx?Tab=0&ListFolderId=" + _folderId.ToString());
			else
				Response.Redirect("~/Projects/ProjectView.aspx?Tab=Lists&ProjectId=" + ProjectId.ToString() + "&ListFolderId=" + ListFolderId.ToString());
		}
		#endregion

		#region lbDelList_Click
		void lbDelList_Click(object sender, EventArgs e)
		{
			try
			{
				ListInfo li = new ListInfo(int.Parse(deletedId.Value));
				li.Delete();

				if (ProjectId < 0)
					Response.Redirect("~/Apps/ListApp/Pages/ListInfoList.aspx?Tab=0&ListFolderId=" + _folderId.ToString());
				else
					Response.Redirect("~/Projects/ProjectView.aspx?Tab=Lists&ProjectId=" + ProjectId.ToString() + "&ListFolderId=" + ListFolderId.ToString());
			}
			catch (MetaFieldReferencedException ex)
			{	
				ClientScript.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString("N"),
					String.Format("alert('{0}');", String.Format(CHelper.GetResFileString("{IbnFramework.ListInfo:RefException}"), CHelper.GetResFileString(ListManager.GetListInfoByMetaClassName(ex.MetaClassName).Title))), true);
			}			
		} 
		#endregion
	}
}
