namespace Mediachase.UI.Web.ToDo.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
	using System.Resources;

	using ComponentArt.Web.UI;
	using Mediachase.Ibn;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using Mediachase.Ibn.Web.UI;
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for ToDoView2.
	/// </summary>
	public partial class ToDoView2 : System.Web.UI.UserControl, ITopTabs
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strToDoView", typeof(ToDoView2).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(ToDoView2).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strPageTitles", typeof(ToDoView2).Assembly);
		protected ResourceManager LocRM4 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strPredecessors", typeof(ToDoView2).Assembly);
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

		#region ToDoID
		private int ToDoID
		{
			get
			{
				try
				{
					return int.Parse(Request["ToDoID"]);
				}
				catch
				{
					throw new Exception("Invalid ToDo ID");
				}
			}
		}
		#endregion

		#region SharedId
		private int SharedId
		{
			get
			{
				int result;
				if (!int.TryParse(Request["SharedID"], out result))
					result = -1;
				return result;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (SharedId > 0)
			{
				apShared.Visible = true;
				lblEntryOwner.Text = Util.CommonHelper.GetUserStatus(SharedId);
			}
			else
				apShared.Visible = false;

			BindToolbar();
			BindTabs();
			if (!IsPostBack)
			{
				ViewState["CurrentTab"] = pc["ToDoView_CurrentTab"];
				Mediachase.IBN.Business.ToDo.AddHistory(ToDoID, Mediachase.IBN.Business.ToDo.GetToDoTitle(ToDoID));
			}

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			cm.AddCommand("ToDo", "", "ToDoView", "MC_PM_ToDoRedirect");
			cm.AddCommand("ToDo", "", "ToDoView", "MC_ToDo_AddSucc");
			cm.AddCommand("ToDo", "", "ToDoView", "MC_ToDo_AddSuccHandler");
			cm.AddCommand("ToDo", "", "ToDoView", "MC_ToDo_AddPred");
			cm.AddCommand("ToDo", "", "ToDoView", "MC_ToDo_AddPredHandler");
		}

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

		#region Implementation of ITopTabs
		public Mediachase.UI.Web.Modules.TopTabs GetTopTabs()
		{
			return ctrlTopTab;
		}
		#endregion

		#region BindToolbar()
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tTitle");

			bool canUpdate = Mediachase.IBN.Business.ToDo.CanUpdate(ToDoID);
			bool canViewFinances = Mediachase.IBN.Business.ToDo.CanViewFinances(ToDoID);
			bool canDelete = Mediachase.IBN.Business.ToDo.CanDelete(ToDoID);
			bool isExternal = Security.CurrentUser.IsExternal;

			ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = /*"<img border='0' src='../Layouts/Images/downbtn.gif' width='9px' height='5px' align='absmiddle'/>&nbsp;" + */LocRM.GetString("Actions");
			topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			ComponentArt.Web.UI.MenuItem subItem;

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = null;
			string cmd = String.Empty;

			#region Create
			if (canUpdate)
			{
				#region Create: Predecessor, Successor
				ComponentArt.Web.UI.MenuItem createItem = new ComponentArt.Web.UI.MenuItem();
				createItem.Text = LocRM.GetString("Create");
				createItem.Look.RightIconUrl = "../Layouts/Images/arrow_right.gif";
				createItem.Look.HoverRightIconUrl = "../Layouts/Images/arrow_right_hover.gif";
				createItem.Look.RightIconWidth = Unit.Pixel(15);
				createItem.Look.RightIconHeight = Unit.Pixel(10);

				#region Create Predecessor
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task_create.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "../ToDo/ToDoEdit.aspx?SuccID=" + ToDoID;
				subItem.Text = LocRM.GetString("Predecessor");
				createItem.Items.Add(subItem);
				#endregion

				#region Create Successor
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task_create.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "../ToDo/ToDoEdit.aspx?PredID=" + ToDoID;
				subItem.Text = LocRM.GetString("Successor");
				createItem.Items.Add(subItem);
				#endregion

				topMenuItem.Items.Add(createItem);
				#endregion
			}
			#endregion

			#region Edit
			if (canUpdate)
			{
				#region Edit: ToDo, Categories
				ComponentArt.Web.UI.MenuItem editItem = new ComponentArt.Web.UI.MenuItem();
				editItem.Text = LocRM.GetString("Edit");
				editItem.Look.RightIconUrl = "../Layouts/Images/arrow_right.gif";
				editItem.Look.HoverRightIconUrl = "../Layouts/Images/arrow_right_hover.gif";
				editItem.Look.RightIconWidth = Unit.Pixel(15);
				editItem.Look.RightIconHeight = Unit.Pixel(10);

				#region Edit ToDo
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task_edit.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "../ToDo/ToDoEdit.aspx?ToDoID=" + ToDoID;
				subItem.Text = LocRM.GetString("ToDo");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit Categories
				if (PortalConfig.CommonToDoAllowEditGeneralCategoriesField)
				{
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.ClientSideCommand = "javascript:ShowWizard('../ToDo/EditCategories.aspx?ToDoID=" + ToDoID + "', 300, 250);";
					subItem.Text = LocRM.GetString("EditCategories");
					editItem.Items.Add(subItem);
				}
				#endregion

				topMenuItem.Items.Add(editItem);
				#endregion
			}
			#endregion

			#region Add Resources
			if (canUpdate && !isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/newgroup.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				cp = new CommandParameters("MC_PM_ToDoResEdit");
				cmd = cm.AddCommand("ToDo", "", "ToDoView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				subItem.ClientSideCommand = "javascript:" + cmd;
				//subItem.ClientSideCommand = "javascript:ModifyResources2(" + ToDoID + ")";
				subItem.Text = LocRM.GetString("AddResources");
				topMenuItem.Items.Add(subItem);
				if (!IsPostBack && Request["Assign"] == "1")
				{
					ClientScript.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
					 "function Assign(){" + cmd + "} setTimeout('Assign()', 400);", true);
				}
			}
			#endregion

			#region Delete
			if (canDelete)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task_delete.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:DeleteToDoModal2()";
				subItem.Text = LocRM.GetString("Delete");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region --- Seperator ---
			if (canUpdate || canDelete)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.LookId = "BreakItem";
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Add Predecessors and Successors
			if (canUpdate)
			{
				#region Add Predecessors
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task_predecessors.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);

				cp = new CommandParameters("MC_ToDo_AddPred");
				cmd = cm.AddCommand("ToDo", "", "ToDoView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				subItem.ClientSideCommand = "javascript:" + cmd;

				subItem.Text = LocRM.GetString("AddPredecessors");
				topMenuItem.Items.Add(subItem);
				#endregion

				#region Add Successors
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task_sucessors.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);

				cp = new CommandParameters("MC_ToDo_AddSucc");
				cmd = cm.AddCommand("ToDo", "", "ToDoView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				subItem.ClientSideCommand = "javascript:" + cmd;

				subItem.Text = LocRM.GetString("AddSuccessors");
				topMenuItem.Items.Add(subItem);
				#endregion

/*				using (IDataReader rdr = ToDo.GetListVacantPredecessors(ToDoID))
				{
					if (rdr.Read())
					{
						subItem = new ComponentArt.Web.UI.MenuItem();
						subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task_predecessors.gif";
						subItem.Look.LeftIconWidth = Unit.Pixel(16);
						subItem.Look.LeftIconHeight = Unit.Pixel(16);
						cp = new CommandParameters("MC_PM_AddPredTodo");
						cmd = cm.AddCommand("ToDo", "", "ToDoView", cp);
						cmd = cmd.Replace("\"", "&quot;");
						subItem.ClientSideCommand = "javascript:" + cmd;
						subItem.Text = LocRM4.GetString("Add");
						topMenuItem.Items.Add(subItem);
					}
				}

				using (IDataReader rdr = ToDo.GetListVacantSuccessors(ToDoID))
				{
					if (rdr.Read())
					{
						subItem = new ComponentArt.Web.UI.MenuItem();
						subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task_sucessors.gif";
						subItem.Look.LeftIconWidth = Unit.Pixel(16);
						subItem.Look.LeftIconHeight = Unit.Pixel(16);
						cp = new CommandParameters("MC_PM_AddSuccTodo");
						cmd = cm.AddCommand("ToDo", "", "ToDoView", cp);
						cmd = cmd.Replace("\"", "&quot;");
						subItem.ClientSideCommand = "javascript:" + cmd;
						subItem.Text = LocRM4.GetString("AddSuc");
						topMenuItem.Items.Add(subItem);
					}
				}
 * */
			}
			#endregion

			#region Add Comments
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/comments.gif";
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			string commentaddlink = "../Common/CommentAdd.aspx?";
			if (isExternal)
				commentaddlink = "../External/ExternalCommentAdd.aspx?";
			subItem.ClientSideCommand = String.Format("javascript:OpenWindow('{0}ToDoID={1}',{2},false);",
		commentaddlink, ToDoID, (Security.CurrentUser.IsExternal) ? "800,600" : "520,270");
			subItem.Text = LocRM.GetString("CreateComment");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region Timesheet
			// OR [2007-08-23]: We should use IbnNext TimeTracking
			/*
			if (Configuration.ProjectManagementEnabled)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/timesheet.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				string sPath = (Security.CurrentUser.IsExternal) ? "../External/ExternalTimeTracking.aspx" : "../TimeTracking/TimeTrackingWeek.aspx";
				subItem.ClientSideCommand = String.Format("javascript:ShowWizard('{0}?ToDoID={1}', {2});",
				  sPath, ToDoID, (Security.CurrentUser.IsExternal) ? "800,600" : "450, 200");
				subItem.Text = LocRM.GetString("AddTimeSheet");
				topMenuItem.Items.Add(subItem);
			}
			 */
			#endregion

			#region Update History
			if (!isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = String.Format("javascript:ShowWizard('../Common/SystemEventsByObject.aspx?ObjectId={0}&ObjectTypeId={1}', 750, 466);", ToDoID, (int)ObjectTypes.ToDo);
				subItem.Text = LocRM3.GetString("UpdateHistory");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Latest Visitors
			if (!isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = String.Format("javascript:ShowWizard('../Common/LatestVisitors.aspx?ObjectId={0}&ObjectTypeId={1}', 450, 266);", ToDoID, (int)ObjectTypes.ToDo);
				subItem.Text = LocRM3.GetString("LatestVisitors");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region System Notifications
			if (!isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = String.Format("../Directory/SystemNotificationForObject.aspx?ObjectId={0}&ObjectTypeId={1}", ToDoID, ((int)ObjectTypes.ToDo).ToString());
				subItem.Text = LocRM2.GetString("SystemNotifications");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region System Reminder
			if (!isExternal)
			{
				Mediachase.IBN.Business.ToDo.ToDoSecurity sec = Mediachase.IBN.Business.ToDo.GetSecurity(ToDoID);
				if (sec.IsManager || sec.IsResource || sec.IsCreator)
				{
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.Look.LeftIconUrl = "~/Layouts/Images/reminder.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.ClientSideCommand = String.Format("javascript:ShowWizard('../Directory/SystemRemindersForObject.aspx?ObjectTypeId={0}&ObjectId={1}', 420, 150)", (int)ObjectTypes.ToDo, ToDoID);
					subItem.Text = LocRM2.GetString("EditReminder");
					topMenuItem.Items.Add(subItem);
				}
			}
			#endregion

			#region Favorites
			if (!Mediachase.IBN.Business.ToDo.CheckFavorites(ToDoID) && !isExternal)
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

			secHeader.ActionsMenu.Items.Add(topMenuItem);

		}
		#endregion

		#region BindTabs
		private void BindTabs()
		{
			ctrlTopTab.IsLeftLine = false;
			
			if (Tab != null && (Tab == "General" || Tab == "FileLibrary" || Tab == "Finance" || Tab == "Discussions" || Tab == "Customization"))
				pc["ToDoView_CurrentTab"] = Tab;
			else if (ViewState["CurrentTab"] != null)
				pc["ToDoView_CurrentTab"] = ViewState["CurrentTab"].ToString();
			else if (pc["ToDoView_CurrentTab"] == null)
				pc["ToDoView_CurrentTab"] = "General";

			ctrlTopTab.AddTab(LocRM.GetString("tabGeneral"), "General");
			ctrlTopTab.AddTab(LocRM.GetString("tabMetaData"), "Customization");
			ctrlTopTab.AddTab(LocRM.GetString("tabLibrary"), "FileLibrary");
			int projectId = Mediachase.UI.Web.Util.CommonHelper.GetProjectIdByObjectIdObjectType(this.ToDoID, (int)ObjectTypes.ToDo);

			if (ToDo.CanViewFinances(ToDoID) && !Security.CurrentUser.IsExternal && Mediachase.IBN.Business.SpreadSheet.ProjectSpreadSheet.IsActive(projectId))
				ctrlTopTab.AddTab(LocRM.GetString("tabFinance"), "Finance");
			else
			{
				if (pc["ToDoView_CurrentTab"] == "Finance")
					pc["ToDoView_CurrentTab"] = "General";
			}
			ctrlTopTab.AddTab(LocRM.GetString("tabDiscussions"), "Discussions");
			ctrlTopTab.SelectItem(pc["ToDoView_CurrentTab"]);

			string controlName = "";
			try
			{
				using (IDataReader rdr = ToDo.GetToDo(ToDoID))
				{
					if (rdr.Read())
					{
						if (rdr["IncidentId"] != DBNull.Value)
							controlName = "ToDoGeneralIncidentLayout.ascx";
						else if (rdr["TaskId"] != DBNull.Value)
							controlName = "ToDoGeneralTaskLayout.ascx";
						else if (rdr["DocumentId"] != DBNull.Value)
							controlName = "ToDoGeneralDocumentLayout.ascx";
						else controlName = "ToDoGeneral2.ascx";
					}
					else
						Response.Redirect("../Common/NotExistingID.aspx?ToDoID=1");
				}
			}
			catch (AccessDeniedException)
			{
				Response.Redirect("../Common/NotExistingID.aspx?AD=1");
			}

			switch (pc["ToDoView_CurrentTab"])
			{
				case "FileLibrary":
					controlName = "FileLibrary.ascx";
					break;
				case "Finance":
					controlName = "Finance.ascx";
					break;
				case "Discussions":
					controlName = "Discussions.ascx";
					break;
				case "Customization":
					controlName = "MetaDataView.ascx";
					break;
			}

			System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
			phItems.Controls.Add(control);
		}
		#endregion

		#region lbDeleteToDoAll_Click
		protected void lbDeleteToDoAll_Click(object sender, System.EventArgs e)
		{
			string link = GetLink();

			ToDo.Delete(ToDoID);

			Response.Redirect(link, true);
		}
		#endregion

		#region GetLink()
		private string GetLink()
		{
			int task_id = -1;
			int doc_id = -1;
			int issue_id = -1;
			int ProjectId = -1;
			using (IDataReader reader = Mediachase.IBN.Business.ToDo.GetToDo(ToDoID, false))
			{
				if (reader.Read())
				{
					if (reader["ProjectId"] != DBNull.Value)
						ProjectId = (int)reader["ProjectId"];
					if (reader["TaskId"] != DBNull.Value)
						task_id = (int)reader["TaskId"];
					if (reader["DocumentId"] != DBNull.Value)
						doc_id = (int)reader["DocumentId"];
					if (reader["IncidentId"] != DBNull.Value)
						issue_id = (int)reader["IncidentId"];
				}
			}

			string link = "";
			if (task_id > 0 && Task.CanRead(task_id))
				link = String.Format("../Tasks/TaskView.aspx?TaskId={0}", task_id);
			else if (doc_id > 0 && Document.CanRead(doc_id))
				link = String.Format("../Documents/DocumentView.aspx?DocumentId={0}", doc_id);
			else if (issue_id > 0 && Incident.CanRead(issue_id))
				link = String.Format("../Incidents/IncidentView.aspx?IncidentId={0}", issue_id);
			else if (ProjectId > 0 && Project.CanRead(ProjectId))
				link = String.Format("../Projects/ProjectView.aspx?ProjectId={0}", ProjectId);
			else
				link = "../Workspace/default.aspx?Btab=Workspace";

			return link;
		}
		#endregion

		#region btnAddToFavorites_Click
		private void btnAddToFavorites_Click(object sender, EventArgs e)
		{
			Mediachase.IBN.Business.ToDo.AddFavorites(ToDoID);
			Util.CommonHelper.ReloadTopFrame("Favorites.ascx", "../ToDo/ToDoView.aspx?ToDoID=" + ToDoID, Response);
		}
		#endregion

	}
}
