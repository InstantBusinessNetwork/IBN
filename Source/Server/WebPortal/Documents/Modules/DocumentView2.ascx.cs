namespace Mediachase.UI.Web.Documents.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using ComponentArt.Web.UI;
	using Mediachase.Ibn.Assignments;
	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Web.UI.WebControls;
	using Mediachase.UI.Web.Modules;

	/// <summary>
	///		Summary description for DocumentView2.
	/// </summary>
	public partial class DocumentView2 : System.Web.UI.UserControl, ITopTabs
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Documents.Resources.strDocuments", typeof(DocumentView2).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(DocumentView2).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strPageTitles", typeof(DocumentView2).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		#region Tab
		private string Tab
		{
			get
			{
				return Request["Tab"];
			}
		}
		#endregion

		#region DocumentId
		private int DocumentId
		{
			get
			{
				try
				{
					return int.Parse(Request["DocumentId"]);
				}
				catch
				{
					throw new Exception("Invalid Document ID");
				}
			}
		}
		#endregion

		#region SharedID
		private int SharedID
		{
			get
			{
				try
				{
					return int.Parse(Request["SharedID"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Document.Exists(DocumentId))
				Response.Redirect("../Common/NotExistingID.aspx?DocumentID=1", true);

			if (SharedID > 0)
			{
				apShared.Visible = true;
				lblEntryOwner.Text = Util.CommonHelper.GetUserStatus(SharedID);
			}
			else
				apShared.Visible = false;

			BindToolBar();
			BindTabs();

			if (!IsPostBack)
			{
				ViewState["CurrentTab"] = pc["DocumentView_CurrentTab"];
				Document.AddHistory(DocumentId);
			}
		}

		#region BindToolbar
		private void BindToolBar()
		{
			secHeader.Title = LocRM.GetString("tbView");
			BindMenu();
		}
		#endregion

		#region BindMenu
		private void BindMenu()
		{
			bool _CanUpdate = Document.CanUpdate(DocumentId);
			bool _CanAddToDo_ModifyRes = Document.CanAddToDo(DocumentId);
			bool _CanDelete = Document.CanDelete(DocumentId);
			bool _IsExternal = Security.CurrentUser.IsExternal;

			ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = /*"<img border='0' src='../Layouts/Images/downbtn.gif' width='9px' height='5px' align='absmiddle'/>&nbsp;" + */LocRM.GetString("tActions");
			topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			ComponentArt.Web.UI.MenuItem subItem;

			#region AddToDo
			if (_CanAddToDo_ModifyRes && !_IsExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task_create.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "../ToDo/ToDoEdit.aspx?DocumentId=" + DocumentId;
				subItem.Text = LocRM.GetString("tbAdd");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Edit
			if (_CanUpdate && !_IsExternal)
			{
				#region Edit: Document, Categories
				ComponentArt.Web.UI.MenuItem editItem = new ComponentArt.Web.UI.MenuItem();
				editItem.Text = LocRM.GetString("tbViewEdit");
				editItem.Look.RightIconUrl = "../Layouts/Images/arrow_right.gif";
				editItem.Look.HoverRightIconUrl = "../Layouts/Images/arrow_right_hover.gif";
				editItem.Look.RightIconWidth = Unit.Pixel(15);
				editItem.Look.RightIconHeight = Unit.Pixel(10);

				#region Edit Document
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/document_edit.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "../Documents/DocumentEdit.aspx?DocumentId=" + DocumentId + "&Back=document";
				subItem.Text = LocRM.GetString("Document");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit Categories
				if (PortalConfig.CommonDocumentAllowEditGeneralCategoriesField)
				{
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.ClientSideCommand = "javascript:ShowWizard('../Documents/EditCategories.aspx?DocumentId=" + DocumentId + "', 300, 250);";
					subItem.Text = LocRM.GetString("EditCategories");
					editItem.Items.Add(subItem);
				}
				#endregion

				topMenuItem.Items.Add(editItem);
				#endregion
			}
			#endregion

			#region Add Resources
			if (_CanAddToDo_ModifyRes && !_IsExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/newgroup.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				CommandManager cm = CommandManager.GetCurrent(this.Page);
				CommandParameters cp = new CommandParameters("MC_DM_DocRes");
				string cmd = cm.AddCommand("Document", "", "DocumentView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				subItem.ClientSideCommand = "javascript:" + cmd;
				//subItem.ClientSideCommand = "javascript:ModifyResources2(" + DocumentId + ")";
				subItem.Text = LocRM.GetString("AddResources");
				topMenuItem.Items.Add(subItem);

				frManageResources.Attributes.Add("src", "ResourcesEditor.aspx?DocumentID=" + DocumentId + "&FrameId=" + frManageResources.ClientID);
				if (!IsPostBack)
				{
					if (Request["Assign"] == "1")
						ClientScript.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
					 "function Assign(){" + cmd + "} setTimeout('Assign()', 400);", true);
//                        Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
//                                        @"OpenAssignWizard(" + DocumentId + @");
//					      function OpenAssignWizard(DocumentId)
//					      {
//						      var obj = document.getElementById('" + frManageResources.ClientID + @"');
//						      if(obj!=null)
//						      {
//							      obj.style.display = '';
//						      }
//					      }",
//                        true);
				}
			}
			#endregion

			#region Delete
			if (_CanDelete && !_IsExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/document_delete.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				string sUrl = "DeleteDocument2()";
				subItem.ClientSideCommand = sUrl;
				subItem.Text = LocRM.GetString("Delete");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region --- Seperator ---
			if (_CanAddToDo_ModifyRes || _CanDelete || _CanUpdate)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.LookId = "BreakItem";
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Add Comments
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/comments.gif";
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			string commentaddlink = "../Common/CommentAdd.aspx?";
			if (_IsExternal)
				commentaddlink = "../External/ExternalCommentAdd.aspx?";
			subItem.ClientSideCommand = String.Format("javascript:OpenWindow('{0}DocumentId={1}',{2},false);",
		commentaddlink, DocumentId, _IsExternal ? "800,600" : "520,270");
			subItem.Text = LocRM.GetString("tbAddCom");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region Timesheet
			// OR [2007-08-23]: We should use IbnNext TimeTracking
			/*
			if(Configuration.ProjectManagementEnabled)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/timesheet.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				string sPath = (Security.CurrentUser.IsExternal) ? "../External/ExternalTimeTracking.aspx" : "../TimeTracking/TimeTrackingWeek.aspx";
				subItem.ClientSideCommand = String.Format("javascript:ShowWizard('{0}?DocumentId={1}', {2});",
					sPath, DocumentId, (Security.CurrentUser.IsExternal) ? "800,600" : "450, 200");
				subItem.Text = LocRM.GetString("tbAddTimeSheet");
				topMenuItem.Items.Add(subItem);
			}*/
			#endregion

			#region UpdateHistory
			if (!_IsExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = String.Format("javascript:ShowWizard('../Common/SystemEventsByObject.aspx?ObjectId={0}&ObjectTypeId={1}', 750, 466);", DocumentId, (int)ObjectTypes.Document);
				subItem.Text = LocRM3.GetString("UpdateHistory");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Latest Visitors
			if (!_IsExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = String.Format("javascript:ShowWizard('../Common/LatestVisitors.aspx?ObjectId={0}&ObjectTypeId={1}', 450, 266);", DocumentId, (int)ObjectTypes.Document);
				subItem.Text = LocRM3.GetString("LatestVisitors");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region System Notifications
			if (!_IsExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = String.Format("../Directory/SystemNotificationForObject.aspx?ObjectId={0}&ObjectTypeId={1}", DocumentId, ((int)ObjectTypes.Document).ToString());
				subItem.Text = LocRM2.GetString("SystemNotifications");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Favorites
			if (!Document.CheckFavorites(DocumentId) && !_IsExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/Favorites.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(btnAddToFavorites, "");
				subItem.Text = LocRM.GetString("AddToFavorites");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			/*
			#region DocumentBack
			if (!_IsExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/cancel.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "../Documents/default.aspx";
				subItem.Text = LocRM.GetString("tbDocumentBack");
				topMenuItem.Items.Add(subItem);
			}
			#endregion
			*/

			secHeader.ActionsMenu.Items.Add(topMenuItem);
		}
		#endregion

		#region BindTabs
		private void BindTabs()
		{
			bool CanViewFinances = Document.CanViewFinances(DocumentId);

			// O.R. [2009-04-06]: Workflow
			if (Tab != null && (Tab == "General" || Tab == "FileLibrary" || Tab == "Finance" || Tab == "Discussions" || Tab == "Customization" || Tab == "Workflow"))
				pc["DocumentView_CurrentTab"] = Tab;
			else if (ViewState["CurrentTab"] != null)
				pc["DocumentView_CurrentTab"] = ViewState["CurrentTab"].ToString();
			else if (pc["DocumentView_CurrentTab"] == null)
				pc["DocumentView_CurrentTab"] = "General";

			if (!CanViewFinances && Tab == "Finance")
				pc["DocumentView_CurrentTab"] = "General";

			// O.R. [2009-10-28]: Workflow. Check Schemas
			Mediachase.Ibn.Assignments.Schemas.SchemaMaster[] list = Mediachase.Ibn.Assignments.Schemas.SchemaManager.GetAvailableShemaMasters();
			if (Tab == "Workflow" && !(Configuration.WorkflowModule && WorkflowActivityWrapper.IsFramework35Installed() && list != null && list.Length > 0))
				pc["DocumentView_CurrentTab"] = "General";

			ctrlTopTab.AddTab(LocRM.GetString("tabGeneral"), "General");
			ctrlTopTab.AddTab(LocRM.GetString("tbDetails"), "Customization");
			ctrlTopTab.AddTab(LocRM.GetString("tabLibrary"), "FileLibrary");
			if (CanViewFinances && !Security.CurrentUser.IsExternal)
				ctrlTopTab.AddTab(LocRM.GetString("tabFinance"), "Finance");
			else
			{
				if (pc["DocumentView_CurrentTab"] == "Finance")
					pc["DocumentView_CurrentTab"] = "General";
			}
			ctrlTopTab.AddTab(LocRM.GetString("tabDiscussions"), "Discussions");

			// O.R. [2009-07-28]: Check license and NET Framework version
			if (Configuration.WorkflowModule && WorkflowActivityWrapper.IsFramework35Installed() && list != null && list.Length > 0)
				ctrlTopTab.AddTab(GetGlobalResourceObject("IbnFramework.BusinessProcess", "Workflows").ToString(), "Workflow");

			ctrlTopTab.SelectItem(pc["DocumentView_CurrentTab"]);

			string controlName = "DocumentGeneral2.ascx";
			switch (pc["DocumentView_CurrentTab"])
			{
				case "General":
					controlName = "DocumentGeneral2.ascx";
					break;
				case "Customization":
					controlName = "MetaDataView.ascx";
					break;
				case "Finance":
					if (CanViewFinances)
						controlName = "Finance.ascx";
					break;
				case "FileLibrary":
					controlName = "FileLibrary.ascx";
					break;
				case "Discussions":
					controlName = "Discussions.ascx";
					break;
				case "Workflow":	// O.R. [2009-04-06]: Workflow hack
					controlName = "~/Apps/BusinessProcess/Modules/WorkflowListByObject.ascx";
					break;
			}

			System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
			phItems.Controls.Add(control);
		}
		#endregion

		#region Implementation of ITopTabs
		public Mediachase.UI.Web.Modules.TopTabs GetTopTabs()
		{
			return ctrlTopTab;
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
			this.btnAddToFavorites.Click += new EventHandler(btnAddToFavorites_Click);
		}
		#endregion

		#region lbDeleteDocumentAll_Click
		protected void lbDeleteDocumentAll_Click(object sender, System.EventArgs e)
		{
			int ProjectId = Document.GetProject(DocumentId);
			string link;
			if (ProjectId > 0 && Project.CanRead(ProjectId))
				link = String.Format("../Projects/ProjectView.aspx?ProjectId={0}&Tab=Documents", ProjectId);
			else
				link = "../Workspace/default.aspx?Btab=Workspace";

			Document.Delete(DocumentId);

			Response.Redirect(link, true);
		}
		#endregion

		#region btnAddToFavorites_Click
		private void btnAddToFavorites_Click(object sender, EventArgs e)
		{
			Document.AddFavorites(DocumentId);
			Util.CommonHelper.ReloadTopFrame("Favorites.ascx", "../Documents/DocumentView.aspx?DocumentId=" + DocumentId, Response);
		}
		#endregion
	}
}
