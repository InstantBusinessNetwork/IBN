namespace Mediachase.UI.Web.Tasks.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using System.Resources;
	using ComponentArt.Web.UI;
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for TaskView2.
	/// </summary>
	public partial class TaskView2 : System.Web.UI.UserControl, ITopTabs
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strTaskGeneral", typeof(TaskView2).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(TaskView2).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strTaskView", typeof(TaskView2).Assembly);
		protected ResourceManager LocRM4 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strPredecessors", typeof(TaskView2).Assembly);
		protected ResourceManager LocRM5 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strFileLibrary", typeof(TaskView2).Assembly);
		protected ResourceManager LocRM6 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strPageTitles", typeof(TaskView2).Assembly);
		private bool canViewFinances = false;
		private bool isMSProject = false;
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

		#region TaskId
		private int TaskId
		{
			get
			{
				return Util.CommonHelper.GetRequestInteger(Request, "TaskId", -1);
			}
		}
		#endregion

		#region SharedID
		private int SharedID
		{
			get
			{
				if (Request["SharedID"] != null)
					return int.Parse(Request["SharedID"]);
				else
					return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Configuration.ProjectManagementEnabled)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			if (SharedID > 0)
			{
				apShared.Visible = true;
				lblEntryOwner.Text = Util.CommonHelper.GetUserStatus(SharedID);
			}
			else
				apShared.Visible = false;

			isMSProject = Project.GetIsMSProject(Task.GetProject(TaskId));

			BindToolbar();
			BindTabs();
			if (!IsPostBack)
			{
				ViewState["CurrentTab"] = pc["TaskView_CurrentTab"];
				Task.AddHistory(TaskId, Task.GetTaskTitle(TaskId));
			}

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			cm.AddCommand("Task", "", "TaskView", "MC_PM_TaskRedirect");
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
			this.lbDeleteTaskAll.Click += new EventHandler(lbDeleteTaskAll_Click);
		}
		#endregion

		#region BindToolbar()
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tTitle");

			bool canUpdateConfigurationInfo = Task.CanUpdateConfigurationInfo(TaskId);
			bool canUpdate = Task.CanUpdate(TaskId);
			bool canAddTodo = Task.CanAddToDo(TaskId);
			bool canDelete = Task.CanDelete(TaskId);
			bool canModifyResources = Task.CanModifyResources(TaskId);
			canViewFinances = Task.CanViewFinances(TaskId);

			ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = /*"<img border='0' src='../Layouts/Images/downbtn.gif' width='9px' height='5px' align='absmiddle'/>&nbsp;" + */LocRM2.GetString("Actions");
			topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			ComponentArt.Web.UI.MenuItem subItem;

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = new CommandParameters();
			string cmd = String.Empty;

			#region Add ToDo
			if (canAddTodo && !Security.CurrentUser.IsExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task_create.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "~/ToDo/ToDoEdit.aspx?TaskId=" + TaskId;
				subItem.Text = LocRM.GetString("tbAdd");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Edit
			if (canUpdate && !Security.CurrentUser.IsExternal)
			{
				ComponentArt.Web.UI.MenuItem editItem = new ComponentArt.Web.UI.MenuItem();
				editItem.Text = LocRM.GetString("Edit");
				editItem.Look.RightIconUrl = "../Layouts/Images/arrow_right.gif";
				editItem.Look.HoverRightIconUrl = "../Layouts/Images/arrow_right_hover.gif";
				editItem.Look.RightIconWidth = Unit.Pixel(15);
				editItem.Look.RightIconHeight = Unit.Pixel(10);

				#region Edit Task
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task1_edit.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "../Tasks/TaskEdit.aspx?TaskId=" + TaskId;
				subItem.Text = LocRM.GetString("EditTask");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit Categories
				if (PortalConfig.CommonTaskAllowViewGeneralCategoriesField)
				{
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.ClientSideCommand = "javascript:ShowWizard('../Tasks/EditCategories.aspx?TaskId=" + TaskId + "', 300, 250);";
					subItem.Text = LocRM2.GetString("EditCategories");
					editItem.Items.Add(subItem);
				}
				#endregion

				/*
				#region Edit General Info
								subItem = new ComponentArt.Web.UI.MenuItem();
								subItem.ClientSideCommand = "javascript:ShowWizard('EditGeneralInfo.aspx?IncidentId=" + IncidentId + "', 500, 400);";
								subItem.Text = LocRM2.GetString("EditGeneralInfo");
								editItem.Items.Add(subItem);
				#endregion
				*/
				topMenuItem.Items.Add(editItem);
			}
			#endregion

			#region Modyfy Resources
			if (canModifyResources && !Security.CurrentUser.IsExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/editgroup.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				cp = new CommandParameters("MC_PM_TaskResEdit");
				cmd = cm.AddCommand("Task", "", "TaskView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				subItem.ClientSideCommand = "javascript:" + cmd;
				//subItem.ClientSideCommand = "javascript:ShowWizard('AddResources.aspx?TaskId=" + TaskId + "', 650, 350);";
				subItem.Text = LocRM.GetString("AssignWizard");
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
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task1_delete.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:DeleteTask()";
				subItem.Text = LocRM.GetString("Delete");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region --- Seperator ---
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.LookId = "BreakItem";
			topMenuItem.Items.Add(subItem);
			#endregion

			#region Add Predecessors
			if (canUpdateConfigurationInfo && !isMSProject)
			{
				using (IDataReader rdr = Task.GetListVacantPredecessors(TaskId))
				{
					if (rdr.Read())
					{
						subItem = new ComponentArt.Web.UI.MenuItem();
						subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task_predecessors.gif";
						subItem.Look.LeftIconWidth = Unit.Pixel(16);
						subItem.Look.LeftIconHeight = Unit.Pixel(16);
						cp = new CommandParameters("MC_PM_AddPredTask");
						cmd = cm.AddCommand("Task", "", "TaskView", cp);
						cmd = cmd.Replace("\"", "&quot;");
						subItem.ClientSideCommand = "javascript:" + cmd;
						//subItem.NavigateUrl = "~/Tasks/AddPredecessor.aspx?BaseTaskID=" + TaskId;
						subItem.Text = LocRM4.GetString("Add");
						topMenuItem.Items.Add(subItem);
					}
				}
			}
			#endregion

			#region Add Successors
			if (canUpdateConfigurationInfo && !isMSProject)
			{
				using (IDataReader rdr = Task.GetListVacantSuccessors(TaskId))
				{
					if (rdr.Read())
					{
						subItem = new ComponentArt.Web.UI.MenuItem();
						subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task_sucessors.gif";
						subItem.Look.LeftIconWidth = Unit.Pixel(16);
						subItem.Look.LeftIconHeight = Unit.Pixel(16);
						cp = new CommandParameters("MC_PM_AddSuccTask");
						cmd = cm.AddCommand("Task", "", "TaskView", cp);
						cmd = cmd.Replace("\"", "&quot;");
						subItem.ClientSideCommand = "javascript:" + cmd;
						//subItem.NavigateUrl = "~/Tasks/AddSuccessor.aspx?BaseTaskID=" + TaskId;
						subItem.Text = LocRM4.GetString("AddSuc");
						topMenuItem.Items.Add(subItem);
					}
				}
			}
			#endregion

			#region Add Comments
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/comments.gif";
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			string sPath = (Security.CurrentUser.IsExternal) ? "../External/ExternalCommentAdd.aspx" : "../Common/CommentAdd.aspx";
			subItem.ClientSideCommand = String.Format("javascript:OpenWindow('{0}?TaskId={1}',{2},false);",
		sPath, TaskId, (Security.CurrentUser.IsExternal) ? "800,600" : "520,270");
			subItem.Text = LocRM2.GetString("CreateComment");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region Timesheet
			// OR [2007-08-23]: We should use IbnNext TimeTracking
			/*
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/timesheet.gif";
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			sPath = (Security.CurrentUser.IsExternal) ? "../External/ExternalTimeTracking.aspx" : "../TimeTracking/TimeTrackingWeek.aspx";
			subItem.ClientSideCommand = String.Format("javascript:ShowWizard('{0}?TaskId={1}', {2});", 
				sPath, TaskId, (Security.CurrentUser.IsExternal)? "800,600" : "450, 200");
			subItem.Text = LocRM2.GetString("AddTimeSheet");
			topMenuItem.Items.Add(subItem);
			 */
			#endregion

			#region UpdateHistory
			if (!Security.CurrentUser.IsExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = String.Format("javascript:ShowWizard('../Common/SystemEventsByObject.aspx?ObjectId={0}&ObjectTypeId={1}', 750, 466);", TaskId, (int)ObjectTypes.Task);
				subItem.Text = LocRM6.GetString("UpdateHistory");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Latest Visitors
			if (!Security.CurrentUser.IsExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = String.Format("javascript:ShowWizard('../Common/LatestVisitors.aspx?ObjectId={0}&ObjectTypeId={1}', 450, 266);", TaskId, (int)ObjectTypes.Task);
				subItem.Text = LocRM6.GetString("LatestVisitors");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region System Notifications
			if (!Security.CurrentUser.IsExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = String.Format("../Directory/SystemNotificationForObject.aspx?ObjectId={0}&ObjectTypeId={1}", TaskId, ((int)ObjectTypes.Task).ToString());
				subItem.Text = LocRM2.GetString("SystemNotifications");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region System Reminder
			if (!Security.CurrentUser.IsExternal)
			{
				Task.TaskSecurity sec = Task.GetSecurity(TaskId);
				if (sec.IsManager || sec.IsRealTaskResource)
				{
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.Look.LeftIconUrl = "~/Layouts/Images/reminder.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.ClientSideCommand = String.Format("javascript:ShowWizard('../Directory/SystemRemindersForObject.aspx?ObjectTypeId={0}&ObjectId={1}', 420, 150)", (int)ObjectTypes.Task, TaskId);
					subItem.Text = LocRM2.GetString("EditReminder");
					topMenuItem.Items.Add(subItem);
				}
			}
			#endregion

			#region Favorites
			if (!Task.CheckFavorites(TaskId) && !Security.CurrentUser.IsExternal)
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
			if (Tab != null && (Tab == "General" || Tab == "FileLibrary" || Tab == "Finance" || Tab == "Discussions" || Tab == "Customization"))
				pc["TaskView_CurrentTab"] = Tab;
			else if (ViewState["CurrentTab"] != null)
				pc["TaskView_CurrentTab"] = ViewState["CurrentTab"].ToString();
			else if (pc["TaskView_CurrentTab"] == null)
				pc["TaskView_CurrentTab"] = "General";

			ctrlTopTab.AddTab(LocRM3.GetString("tabGeneral"), "General");
			ctrlTopTab.AddTab(LocRM3.GetString("tabMetaData"), "Customization");
			ctrlTopTab.AddTab(LocRM3.GetString("tabLibrary"), "FileLibrary");

			int projectId = Mediachase.UI.Web.Util.CommonHelper.GetProjectIdByObjectIdObjectType(this.TaskId, (int)ObjectTypes.Task);

			if (canViewFinances && !Security.CurrentUser.IsExternal && Mediachase.IBN.Business.SpreadSheet.ProjectSpreadSheet.IsActive(projectId))
				ctrlTopTab.AddTab(LocRM3.GetString("tabFinance"), "Finance");
			else
			{
				if (pc["TaskView_CurrentTab"] == "Finance")
					pc["TaskView_CurrentTab"] = "General";
			}
			ctrlTopTab.AddTab(LocRM3.GetString("tabDiscussions"), "Discussions");

			ctrlTopTab.SelectItem(pc["TaskView_CurrentTab"]);

			string controlName = "TaskGeneral.ascx";
			switch (pc["TaskView_CurrentTab"])
			{
				case "General":
					controlName = "TaskGeneral.ascx";
					break;
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

		#region btnAddToFavorites_Click
		private void btnAddToFavorites_Click(object sender, EventArgs e)
		{
			Task.AddFavorites(TaskId);
			Util.CommonHelper.ReloadTopFrame("Favorites.ascx", "../Tasks/TaskView.aspx?TaskId=" + TaskId, Response);
		}
		#endregion

		#region lbDeleteTaskAll_Click
		private void lbDeleteTaskAll_Click(object sender, EventArgs e)
		{
			int ProjectId = Task.GetProject(TaskId);
			string link = String.Format("../Projects/ProjectView.aspx?ProjectId={0}", ProjectId);

			Task.Delete(TaskId);

			Response.Redirect(link, true);
		}
		#endregion

		#region ITopTabs Members
		public Mediachase.UI.Web.Modules.TopTabs GetTopTabs()
		{
			return ctrlTopTab;
		}
		#endregion
	}
}
