namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Text;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using ComponentArt.Web.UI;
	using Mediachase.Ibn;
	using Mediachase.Ibn.Lists;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Web.UI.WebControls;
	using Mediachase.IBN.Business.SpreadSheet;
	using System.IO;

	/// <summary>
	///		Summary description for ProjectView2.
	/// </summary>
	public partial class ProjectView2 : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(ProjectView2).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(ProjectView2).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strPageTitles", typeof(ProjectView2).Assembly);
		protected ResourceManager LocRM4 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectFinances", typeof(ProjectView2).Assembly);
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

		#region ProjectId
		private int ProjectId
		{
			get
			{
				try
				{
					return int.Parse(Request["ProjectId"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		private bool isMSProject = false;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Configuration.ProjectManagementEnabled)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			try
			{
				using (IDataReader rdr = Project.GetProject(ProjectId))
				{
					if (!rdr.Read())
						Response.Redirect("~/Common/notexistingid.aspx?ProjectId=" + ProjectId);
					isMSProject = (bool)rdr["IsMSProject"];
				}
			}
			catch (AccessDeniedException)
			{
				Response.Redirect("../Common/NotExistingId.aspx?AD=1");
			}

			string path = PortalConfig.ProjectInfoControlDefaultValue;
			if (File.Exists(Server.MapPath(PortalConfig.ProjectInfoControl)))
				path = PortalConfig.ProjectInfoControl;
			System.Web.UI.Control control = LoadControl(path);
			InfoPlaceHolder.Controls.Add(control);

			BindToolbar();
			BindTabs();

			if (!IsPostBack)
			{
				ViewState["CurrentTab"] = pc["ProjectView_CurrentTab"];
				Mediachase.IBN.Business.Project.AddHistory(ProjectId, Mediachase.IBN.Business.Project.GetProjectTitle(ProjectId));
			}

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			cm.AddCommand("Project", "", "ProjectView", "MC_PM_RelatedPrjAdd");
			cm.AddCommand("Project", "", "ProjectView", "MC_PM_Redirect");
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
			this.btnSaveSnapshot.Click += new EventHandler(btnSaveSnapshot_Click);
			this.btnAddToFavorites.Click += new EventHandler(btnAddToFavorites_Click);
			this.btnAddRelatedPrj.Click += new EventHandler(btnAddRelatedPrj_Click);
			this.btnDeactivateFinance.Click += new EventHandler(btnDeactivateFinance_Click);
		}
		#endregion

		#region BindToolbar()
		private void BindToolbar()
		{
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			
			secHeader.Title = LocRM.GetString("QuickTools");

			bool canUpdate = Project.CanUpdate(ProjectId);
			bool canViewFinances = Project.CanViewFinances(ProjectId);
			bool canDelete = Project.CanDelete(ProjectId);
			bool isExternal = Security.CurrentUser.IsExternal;

			ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = /*"<img border='0' src='../Layouts/Images/downbtn.gif' width='9px' height='5px' align='absmiddle'/>&nbsp;" + */LocRM.GetString("Actions");
			topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			ComponentArt.Web.UI.MenuItem subItem;

			#region TTBlock
			/*
			ComponentArt.Web.UI.MenuItem ttItem = new ComponentArt.Web.UI.MenuItem();

			ttItem.Text = CommonHelper.GetResFileString("{IbnFramework.Global:_mc_TimeManagement}");
			ttItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
			ttItem.Look.LeftIconHeight = Unit.Pixel(5);
			ttItem.Look.LeftIconWidth = Unit.Pixel(16);
			ttItem.LookId = "TopItemLook";

            //DV: 2007-10-19
            subItem = new ComponentArt.Web.UI.MenuItem();
            subItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/icons/timesheet.gif");
            subItem.Look.LeftIconWidth = Unit.Pixel(16);
            subItem.Look.LeftIconHeight = Unit.Pixel(16);
            subItem.NavigateUrl = "~/IbnNext/TimeTracking/ListTimeTrackingPopupEdit.aspx?ViewName=TT_MyGroupByWeek&ProjectId=" + ProjectId;
            subItem.Text = CommonHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_TT_MyGroupByWeek}");// +" new";
            ttItem.Items.Add(subItem);

            subItem = new ComponentArt.Web.UI.MenuItem();
            subItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/icons/timesheet.gif");
            subItem.Look.LeftIconWidth = Unit.Pixel(16);
            subItem.Look.LeftIconHeight = Unit.Pixel(16);
            subItem.NavigateUrl = "~/IbnNext/TimeTracking/ListTimeTrackingNew.aspx?ViewName=TT_MyGroupByWeek&ProjectId=" + ProjectId;
            subItem.Text = CommonHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_TT_MyGroupByWeek}") + " new";
            ttItem.Items.Add(subItem);

			if (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
						Security.IsUserInGroup(InternalSecureGroups.ProjectManager) ||
						Security.IsUserInGroup(InternalSecureGroups.TimeManager))
			{
                //DV: 2007-10-19
                subItem = new ComponentArt.Web.UI.MenuItem();
                subItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/icons/timesheet.gif");
                subItem.Look.LeftIconWidth = Unit.Pixel(16);
                subItem.Look.LeftIconHeight = Unit.Pixel(16);
                subItem.NavigateUrl = "~/IbnNext/TimeTracking/ListTimeTrackingPopupEdit.aspx?ViewName=TT_CurrentProjectGroupByWeekUser&ProjectId=" + ProjectId;
                subItem.Text = subItem.Text = CommonHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_TT_CurrentProjectGroupByWeekUser}");// +" new";
                ttItem.Items.Add(subItem);

                subItem = new ComponentArt.Web.UI.MenuItem();
                subItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/icons/timesheet.gif");
                subItem.Look.LeftIconWidth = Unit.Pixel(16);
                subItem.Look.LeftIconHeight = Unit.Pixel(16);
                subItem.NavigateUrl = "~/IbnNext/TimeTracking/ListTimeTrackingNew.aspx?ViewName=TT_CurrentProjectGroupByWeekUser&ProjectId=" + ProjectId;
                subItem.Text = subItem.Text = CommonHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_TT_CurrentProjectGroupByWeekUser}") + " new";// +" new";
                ttItem.Items.Add(subItem);
			}

			secHeader.ActionsMenu.Items.Add(ttItem);
			 */
			#endregion

			#region Quick Add Tasks / To-dos
			if (Mediachase.IBN.Business.ToDo.CanCreate(ProjectId) || Task.CanCreate(ProjectId))
			{
				ComponentArt.Web.UI.MenuItem listToDo = new ComponentArt.Web.UI.MenuItem();

				listToDo.Look.LeftIconUrl = "~/Layouts/Images/rulesnew.gif";
				listToDo.Look.LeftIconWidth = Unit.Pixel(16);
				listToDo.Look.LeftIconHeight = Unit.Pixel(16);
				CommandParameters cpQT = new CommandParameters("MC_MetaUI_CreateTaskTodoGrid");
				string cmdQT = cm.AddCommand("Project", "", "ProjectView", cpQT);
				cmdQT = cmdQT.Replace("\"", "&quot;");
				listToDo.ClientSideCommand = "javascript:" + cmdQT;
				listToDo.Text = GetGlobalResourceObject("IbnFramework.Project", "CreateTaskTodoGrid").ToString();
				secHeader.ActionsMenu.Items.Add(listToDo);
			}

			if (Mediachase.IBN.Business.ToDo.CanCreate(ProjectId))
			{
				ComponentArt.Web.UI.MenuItem quickToDo = new ComponentArt.Web.UI.MenuItem();

				quickToDo.Look.LeftIconUrl = "~/Layouts/Images/icons/task_create.gif";
				quickToDo.Look.LeftIconWidth = Unit.Pixel(16);
				quickToDo.Look.LeftIconHeight = Unit.Pixel(16);
				CommandParameters cpQT = new CommandParameters("MC_MetaUI_CreateTodoQuick");
				string cmdQT = cm.AddCommand("Project", "", "ProjectView", cpQT);
				cmdQT = cmdQT.Replace("\"", "&quot;");
				quickToDo.ClientSideCommand = "javascript:" + cmdQT;
				quickToDo.Text = LocRM.GetString("CreateToDo");
				secHeader.ActionsMenu.Items.Add(quickToDo);
			}
			#endregion

			#region Create: Task, CalendarEntry, ToDo, Incident, Document
			ComponentArt.Web.UI.MenuItem createItem = new ComponentArt.Web.UI.MenuItem();

			createItem.Text = /*"<img border='0' src='../Layouts/Images/downbtn.gif' width='9px' height='5px' align='absmiddle'/>&nbsp;" + */LocRM.GetString("Create");
			createItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
			createItem.Look.LeftIconHeight = Unit.Pixel(5);
			createItem.Look.LeftIconWidth = Unit.Pixel(16);
			createItem.LookId = "TopItemLook";
			/*			createItem.Text = LocRM.GetString("Create");
						createItem.Look.RightIconUrl = "../Layouts/Images/arrow_right.gif";
						createItem.Look.HoverRightIconUrl = "../Layouts/Images/arrow_right_hover.gif";
						createItem.Look.RightIconWidth = Unit.Pixel(15);
						createItem.Look.RightIconHeight = Unit.Pixel(10);
			*/

			#region Create Task
			if (!isMSProject && Task.CanCreate(ProjectId))
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task1_create.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "~/Tasks/TaskEdit.aspx?ProjectId=" + ProjectId;
				subItem.Text = LocRM.GetString("CreateTask");
				createItem.Items.Add(subItem);
			}
			#endregion

			#region Create ToDo
			if (Mediachase.IBN.Business.ToDo.CanCreate(ProjectId))
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task_create.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "~/ToDo/ToDoEdit.aspx?ProjectId=" + ProjectId;
				subItem.Text = LocRM.GetString("CreateToDo");
				createItem.Items.Add(subItem);
			}
			#endregion

			#region Create CalendarEntry
			if (CalendarEntry.CanCreate(ProjectId))
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/event_create.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "~/Events/EventEdit.aspx?ProjectId=" + ProjectId;
				subItem.Text = LocRM.GetString("CreateEvent");
				createItem.Items.Add(subItem);
			}
			#endregion

			#region Create Incident
			if (Configuration.HelpDeskEnabled && Incident.CanCreate(ProjectId))
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/incident_create.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "~/Incidents/IncidentEdit.aspx?ProjectId=" + ProjectId;
				subItem.Text = LocRM.GetString("CreateIncident");
				createItem.Items.Add(subItem);
			}
			#endregion

			#region Create Document
			if (Document.CanCreate(ProjectId))
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/document_create.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "~/Documents/DocumentEdit.aspx?ProjectId=" + ProjectId;
				subItem.Text = LocRM.GetString("CreateDocument");
				createItem.Items.Add(subItem);
			}
			#endregion

			if (createItem.Items.Count > 0)
				secHeader.ActionsMenu.Items.Add(createItem);

			//			topMenuItem.Items.Add(createItem);
			#endregion

			if (canUpdate)
			{
				#region Edit: Project, Timeline, GeneralInfo, ConfigurationInfo, Categories, Managers, Client
				ComponentArt.Web.UI.MenuItem editItem = new ComponentArt.Web.UI.MenuItem();
				editItem.Text = /*"<img border='0' src='../Layouts/Images/downbtn.gif' width='9px' height='5px' align='absmiddle'/>&nbsp;" + */LocRM.GetString("Edit");
				editItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
				editItem.Look.LeftIconHeight = Unit.Pixel(5);
				editItem.Look.LeftIconWidth = Unit.Pixel(16);
				editItem.LookId = "TopItemLook";
				/*				editItem.Text = LocRM.GetString("Edit");
								editItem.Look.RightIconUrl = "../Layouts/Images/arrow_right.gif";
								editItem.Look.HoverRightIconUrl = "../Layouts/Images/arrow_right_hover.gif";
								editItem.Look.RightIconWidth = Unit.Pixel(15);
								editItem.Look.RightIconHeight = Unit.Pixel(10);
				*/

				#region Edit Project
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/project_edit.gif";
				//subItem.Look.LeftIconWidth = Unit.Pixel(16);
				//subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "../Projects/ProjectEdit.aspx?ProjectId=" + ProjectId + "&Back=project";
				subItem.Text = LocRM.GetString("Project");
				editItem.Items.Add(subItem);
				#endregion

				#region Participants: Team, Sponsors, Stakeholders, Managers
				ComponentArt.Web.UI.MenuItem participantsItem = new ComponentArt.Web.UI.MenuItem();
				participantsItem.Text = LocRM.GetString("tParticipants");
				participantsItem.Look.RightIconUrl = "../Layouts/Images/arrow_right.gif";
				//participantsItem.Look.HoverRightIconUrl = "../Layouts/Images/arrow_right_hover.gif";
				//participantsItem.Look.RightIconWidth = Unit.Pixel(15);
				//participantsItem.Look.RightIconHeight = Unit.Pixel(10);

				#region Modify Team
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/newgroup.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				CommandParameters cpTeam = new CommandParameters("MC_PM_TeamEdit");
				string cmdTeam = cm.AddCommand("Project", "", "ProjectView", cpTeam);
				subItem.ClientSideCommand = "javascript:" + cmdTeam;
				subItem.Text = LocRM.GetString("ModifyTeam");
				participantsItem.Items.Add(subItem);
				#endregion

				#region Modify Sponsors
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/sponsors.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				CommandParameters cpSpons = new CommandParameters("MC_PM_SponsorsEdit");
				string cmdSpons = cm.AddCommand("Project", "", "ProjectView", cpSpons);
				subItem.ClientSideCommand = "javascript:" + cmdSpons;
				subItem.Text = LocRM.GetString("ModifySponsors");
				participantsItem.Items.Add(subItem);
				#endregion

				#region Modify Stakeholders
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/stakeholders.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				CommandParameters cpSt = new CommandParameters("MC_PM_StakesEdit");
				string cmdSt = cm.AddCommand("Project", "", "ProjectView", cpSt);
				subItem.ClientSideCommand = "javascript:" + cmdSt;
				subItem.Text = LocRM.GetString("ModifyStakeholders");
				participantsItem.Items.Add(subItem);
				#endregion

				editItem.Items.Add(participantsItem);
				#endregion

				#region Edit Target Timileme
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = "javascript:ShowWizard('EditTargetTimeline.aspx?ProjectId=" + ProjectId + "', 400, 250);";
				subItem.Text = LocRM.GetString("EditTargetTimeline");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit Actual Timileme
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = "javascript:ShowWizard('EditActualTimeline.aspx?ProjectId=" + ProjectId + "', 400, 250);";
				subItem.Text = LocRM.GetString("EditActualTimeline");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit State Info
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = "javascript:ShowWizard('EditStateInfo.aspx?ProjectId=" + ProjectId + "', 350, 265);";
				subItem.Text = LocRM.GetString("EditStateInfo");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit General Info
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = "javascript:ShowWizard('EditGeneralInfo.aspx?ProjectId=" + ProjectId + "', 550, 450);";
				subItem.Text = LocRM.GetString("EditGeneralInfo");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit Configuration Info
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = "javascript:ShowWizard('EditConfigurationInfo.aspx?ProjectId=" + ProjectId + "', 380, 270);";
				subItem.Text = LocRM.GetString("EditConfigurationInfo");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit Categories
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = "javascript:ShowWizard('EditCategories.aspx?ProjectId=" + ProjectId + "', 300, 450);";
				subItem.Text = LocRM.GetString("EditCategories");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit Managers
				subItem = new ComponentArt.Web.UI.MenuItem();
				CommandParameters cpMan = new CommandParameters("MC_PM_Managers");
				string cmdMan = cm.AddCommand("Project", "", "ProjectView", cpMan);
				subItem.ClientSideCommand = "javascript:" + cmdMan;
				subItem.Text = LocRM.GetString("EditManagers");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit Security
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = "javascript:ShowWizard('EditSecurity.aspx?ProjectId=" + ProjectId + "', 650, 280);";
				subItem.Text = LocRM2.GetString("SecuritySettings");
				editItem.Items.Add(subItem);
				#endregion

				secHeader.ActionsMenu.Items.Add(editItem);
				//				topMenuItem.Items.Add(editItem);
				#endregion
			}

			#region Copy to Clipboard
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Look.LeftIconUrl = "~/Layouts/Images/xp-copy.gif";
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			subItem.ClientSideCommand = "javascript:try{_XMLReqForClip('AddClip=Prj&ProjectId=" + ProjectId.ToString() + "', '" + LocRM.GetString("tXMLError") + "')}catch(e){}";
			subItem.Text = LocRM.GetString("tCopyPrjToClipboard");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region Add from Clipboard
			if (canUpdate)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/xp-paste.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				CommandManager cm1 = CommandManager.GetCurrent(this.Page);
				CommandParameters cp1 = new CommandParameters("MC_PM_RelatedPrjClipboard");
				string cmd1 = cm1.AddCommand("Project", "", "ProjectView", cp1);
				cmd1 = cmd1.Replace("\"", "&quot;");
				subItem.ClientSideCommand = "javascript:" + cmd1;
				subItem.Text = LocRM.GetString("tPastePrjFromClipboard");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Clear Clipboard
			if (canUpdate)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/clearbuffer.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:try{_XMLReqForClip('ClearClip=Prj', '" + LocRM.GetString("tXMLError") + "')}catch(e){}";
				subItem.Text = LocRM.GetString("tClearClipboard");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region --- Separator ---
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.LookId = "BreakItem";
			topMenuItem.Items.Add(subItem);
			#endregion

			#region Add Related Project
			if (canUpdate)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/relprojects.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				CommandManager cm1 = CommandManager.GetCurrent(this.Page);
				CommandParameters cp1 = new CommandParameters("MC_PM_RelatedPrj");
				string cmd1 = cm1.AddCommand("Project", "", "ProjectView", cp1);
				cmd1 = cmd1.Replace("\"", "&quot;");
				subItem.ClientSideCommand = "javascript:" + cmd1;
				subItem.Text = LocRM.GetString("AddRelated");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Add Comments
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/comments.gif";
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			subItem.ClientSideCommand = "javascript:OpenWindow('../Common/CommentAdd.aspx?ProjectId=" + ProjectId + "',520,270,false);";
			subItem.Text = LocRM.GetString("CreateComment");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region Timesheet
			// OR [2007-08-23]: We should use IbnNext TimeTracking
			/*
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/timesheet.gif";
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			subItem.NavigateUrl = "~/TimeTracking/AddTimesheet.aspx?ProjectId=" + ProjectId;
			subItem.Text = LocRM.GetString("AddTimeSheet");
			topMenuItem.Items.Add(subItem);
			 */
			#endregion

			#region Delete
			if (canDelete)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/project_delete.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:DeleteProject()";
				subItem.Text = LocRM2.GetString("Delete");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region --- Separator ---
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.LookId = "BreakItem";
			topMenuItem.Items.Add(subItem);
			#endregion

			#region Snapshot
			if (canViewFinances)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/report.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:ShowWizard2('../Reports/OverallProjectSnapshot.aspx?ProjectId=" + ProjectId + "', 750, 466, true);";
				subItem.Text = LocRM.GetString("Snapshot");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region SaveToHistory - deleted
			if (canViewFinances)
			{
				/*
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/saveitem.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(btnSaveSnapshot, "");
				subItem.Text = LocRM.GetString("tSaveSnapshot");
				topMenuItem.Items.Add(subItem);
				*/
			}
			#endregion

			#region CreateTemplate OLD From 4.1 //DV
			/*if (canUpdate)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/newtemplate.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:ShowWizard('EditProjectTemplate.aspx?ProjectId=" + ProjectId + "', 640, 480);";
				subItem.Text = LocRM.GetString("CreateTemplate");
				topMenuItem.Items.Add(subItem);
			}*/
			#endregion

			#region CreateTemplate2 //DV
			if (canUpdate && !isMSProject)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/newtemplate.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:ShowWizard('EditProjectTemplate2.aspx?ProjectId=" + ProjectId + "', 640, 600);";
				subItem.Text = LocRM.GetString("CreateTemplate");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region SaveBasePlan
			if (canUpdate)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/SAVEITEM.GIF";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:ShowWizard('SaveBasePlanPopUp.aspx?ProjectId=" + ProjectId + "', 380, 100);";
				subItem.Text = LocRM.GetString("SaveBasePlan");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			if (canUpdate && ProjectSpreadSheet.IsActive(ProjectId))
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/card-delete.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);

				subItem.ClientSideCommand = string.Format("if (confirm('{0}')) {{ {1} }}", LocRM4.GetString("ReactivateMsg"), this.Page.ClientScript.GetPostBackEventReference(btnDeactivateFinance, string.Empty));
				subItem.Text = LocRM4.GetString("ReactivateText");
				topMenuItem.Items.Add(subItem);
			}

			#region Export/Import
			if (canUpdate && Project.IsMSProjectSynchronizationEnabled() && !Project.GetIsMSProject(ProjectId))
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/Synch.png";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				//StringBuilder sb = new StringBuilder();
				//sb.AppendFormat("javascript:{{if(confirm('{0}')) {1};}}",
				//    LocRM.GetString("ToMSProjSyncWarning"), Page.ClientScript.GetPostBackEventReference(btnToMSPrjSynch, ""));
				//subItem.ClientSideCommand = sb.ToString();
				subItem.ClientSideCommand = "OpenWindow('../Projects/ProjectExportImportNew.aspx?ToMSPrj=1&ProjectId=" + ProjectId.ToString() + "',600,400);";
				subItem.Text = LocRM.GetString("ToMSProjSync");
				topMenuItem.Items.Add(subItem);
			}
			if (canUpdate && 
					(	
						(Project.IsMSProjectIntegrationEnabled() && !Project.GetIsMSProject(ProjectId))
						||
						(Project.IsMSProjectSynchronizationEnabled() && Project.GetIsMSProject(ProjectId))
					)
				)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/export.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "OpenWindow('../Projects/ProjectExportImportNew.aspx?ProjectId=" + ProjectId.ToString() + "',600,410);";
				subItem.Text = LocRM.GetString("MSProjectExchange");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region --- Separator ---
			if (canViewFinances || canUpdate)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.LookId = "BreakItem";
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region UpdateHistory
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.ClientSideCommand = String.Format("javascript:ShowWizard('../Common/SystemEventsByObject.aspx?ObjectId={0}&ObjectTypeId={1}', 750, 466);", ProjectId, (int)ObjectTypes.Project);
			subItem.Text = LocRM3.GetString("UpdateHistory");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region Latest Visitors
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.ClientSideCommand = String.Format("javascript:ShowWizard('../Common/LatestVisitors.aspx?ObjectId={0}&ObjectTypeId={1}', 450, 266);", ProjectId, (int)ObjectTypes.Project);
			subItem.Text = LocRM3.GetString("LatestVisitors");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region System Notifications
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			subItem.NavigateUrl = String.Format("../Directory/SystemNotificationForObject.aspx?ObjectId={0}&ObjectTypeId={1}", ProjectId, ((int)ObjectTypes.Project).ToString());
			subItem.Text = LocRM.GetString("SystemNotifications");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region System Reminder
			Project.ProjectSecurity ps = Project.GetSecurity(ProjectId);
			if (ps.IsManager || ps.IsManager || ps.IsTeamMember || ps.IsSponsor || ps.IsStakeHolder)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/reminder.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = String.Format("javascript:ShowWizard('../Directory/SystemRemindersForObject.aspx?ObjectTypeId={0}&ObjectId={1}', 420, 150)", (int)ObjectTypes.Project, ProjectId);
				subItem.Text = LocRM.GetString("EditReminder");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Favorites
			if (!Project.CheckFavorites(ProjectId))
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

		#region Add Related
		protected void btnAddRelatedPrj_Click(object sender, EventArgs e)
		{
			string param = Request["__EVENTARGUMENT"];
			if (!String.IsNullOrEmpty(param))
			{
				string[] mas = param.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				if (mas.Length < 2 || !mas[0].Equals("3"))
					return;
				int iRelId = int.Parse(mas[1]);
				Project2.AddRelation(ProjectId, iRelId);
				Response.Redirect("~/Projects/ProjectView.aspx?ProjectId=" + ProjectId + "&Tab=1", true);
			}
		}
		#endregion

		#region BindTabs
		private void BindTabs()
		{
			DataTable dt;
			ctrlTopTab.IsLeftLine = false;

			if (Tab != null && (
				Tab == "Workspace" || Tab == "1" || Tab == "FileLibrary" || Tab == "Discussions"
				|| Tab == "4" || Tab == "6" || Tab == "Setup" || Tab == "Reports"
				|| Tab == "tabRes" || Tab == "Calendar" || Tab == "Lists"
				|| Tab == "My"
				))
				pc["ProjectView_CurrentTab"] = Tab;
			else if (ViewState["CurrentTab"] != null)
				pc["ProjectView_CurrentTab"] = ViewState["CurrentTab"].ToString();
			else if (pc["ProjectView_CurrentTab"] == null)
				pc["ProjectView_CurrentTab"] = "Workspace";

			ctrlTopTab.AddTab(LocRM.GetString("tabWorkspace"), "Workspace");
			ctrlTopTab.AddTab(LocRM.GetString("tabGeneral"), "1");
			ctrlTopTab.AddTab(LocRM.GetString("tParticipants"), "tabRes");

			// Management
			if (Project.CanViewManagement(ProjectId))
				ctrlTopTab.AddTab(LocRM.GetString("tManagement"), "6");
			else
			{
				if (pc["ProjectView_CurrentTab"] == "6")
					pc["ProjectView_CurrentTab"] = "Workspace";
			}

			// Calendar
			ctrlTopTab.AddTab(LocRM.GetString("tabCalendar"), "Calendar");

			/*			// Documents
						dt = Document.GetListDocumentsByProjectDataTable(ProjectId);
						ctrlTopTab.AddTab(LocRM.GetString("tabDocuments") + " <span class='ibn-number'>(" + dt.Rows.Count + ")</span>", "Documents");
			*/

			ctrlTopTab.AddTab(LocRM.GetString("tMy"), "My", 2);

			// Finances
			if (Project.CanViewFinances(ProjectId))
				ctrlTopTab.AddTab(LocRM.GetString("tabFinances"), "4", 2);
			else
			{
				if (pc["ProjectView_CurrentTab"] == "4")
					pc["ProjectView_CurrentTab"] = "Workspace";
			}

			// Lists
			//int ListsCount = Mediachase.IBN.Business.List.GetListsCountByProject(ProjectId);
			int ListsCount = Mediachase.Ibn.Lists.ListManager.GetListsCountByProject(ProjectId);
			ctrlTopTab.AddTab(LocRM.GetString("tabLists") + " <span class='ibn-number'>(" + ListsCount + ")</span>", "Lists", 2);

			// Library
			int FilesCount = Project.GetFileCountByProject(ProjectId);
			int FilesCountAll = Project.GetFileCountByProjectAll(ProjectId);
			if (FilesCount != FilesCountAll)
				ctrlTopTab.AddTab(LocRM.GetString("tabLibrary") + " <span class='ibn-number'>(" + FilesCount + "/" + FilesCountAll + ")</span>", "FileLibrary", 2);
			else
				ctrlTopTab.AddTab(LocRM.GetString("tabLibrary") + " <span class='ibn-number'>(" + FilesCount + ")</span>", "FileLibrary", 2);

			// Discussions
			dt = Project.GetListDiscussionsDataTable(ProjectId);
			ctrlTopTab.AddTab(LocRM.GetString("tabDiscussions") + " <span class='ibn-number'>(" + dt.Rows.Count + ")</span>", "Discussions", 2);

			if (pc["ProjectView_CurrentTab"] == "ExportImport")
				pc["ProjectView_CurrentTab"] = "Workspace";

			// Reports
			if (Project.CanViewReports(ProjectId))
				ctrlTopTab.AddTab(LocRM.GetString("tabReports"), "Reports", 2);
			else
				if (pc["ProjectView_CurrentTab"] == "Reports")
					pc["ProjectView_CurrentTab"] = "Workspace";

			ctrlTopTab.TabWidth = "120";
			ctrlTopTab.SelectItem(pc["ProjectView_CurrentTab"]);

			string controlName = "";
			switch (pc["ProjectView_CurrentTab"])
			{
				case "1":
					controlName = "ProjectGeneral2.ascx";
					break;
				case "FileLibrary":
					controlName = "FileLibrary.ascx";
					break;
				case "Lists":
					controlName = "ProjectLists.ascx";
					break;
				case "Discussions":
					controlName = "Discussions.ascx";
					break;
				case "4":
					controlName = "ProjectFinances3.ascx";
					break;
				case "6":
					controlName = "ProjectActivities2.ascx";
					break;
				case "Calendar":
					controlName = "ProjectCalendar.ascx";
					break;
				case "tabRes":
					controlName = "Resources.ascx";
					break;
				case "Workspace":
					controlName = "Workspaceview.ascx";
					break;
				/*				case "Documents":
									controlName = "ProjectDocuments.ascx";
									break;
				*/
				case "Setup":
					controlName = "Customize.ascx";
					break;
				case "Reports":
					controlName = "ProjectReports.ascx";
					break;
				case "My":
					controlName = "MyWork.ascx";
					break;
				default:
					controlName = "Workspaceview.ascx";
					break;
			}

			System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
			phItems.Controls.Add(control);
		}
		#endregion

		#region btnSaveSnapshot_Click
		private void btnSaveSnapshot_Click(object sender, EventArgs e)
		{
			Project.AddProjectSnapshot(ProjectId);
		}
		#endregion

		#region btnAddToFavorites_Click
		private void btnAddToFavorites_Click(object sender, EventArgs e)
		{
			Project.AddFavorites(ProjectId);
			Util.CommonHelper.ReloadTopFrame("Favorites.ascx", "../Projects/ProjectView.aspx?ProjectId=" + ProjectId, Response);
		}
		#endregion

		#region lblDeleteProjectAll_Click
		protected void lblDeleteProjectAll_Click(object sender, System.EventArgs e)
		{
			ListManager.DeleteProjectRoot(ProjectId);
			Project.Delete(ProjectId);
			Response.Redirect("~/Apps/ProjectManagement/Pages/ProjectList.aspx");
		}
		#endregion

		#region btnToMSPrjSynch_Click
		protected void btnToMSPrjSynch_Click(object sender, System.EventArgs e)
		{
			Project.UpdateIsMSProject(ProjectId);
			Response.Redirect(Request.RawUrl);
		}
		#endregion

		#region btnDeactivateFinance_Click
		/// <summary>
		/// Handles the Click event of the btnDeactivateFinance control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void btnDeactivateFinance_Click(object sender, EventArgs e)
		{
			ProjectSpreadSheet.Deactivate(ProjectId);
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), "<script language=JavaScript> window.location.href=window.location.href; </script>");
		} 
		#endregion

	}
}
