using System;
using System.Collections.Generic;
using System.Data;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business.SpreadSheet;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Lists;
using System.Globalization;
using System.IO;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.Modules
{
	public partial class ViewProject : System.Web.UI.UserControl
	{
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

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(ViewProject).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(ViewProject).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strPageTitles", typeof(ViewProject).Assembly);
		protected ResourceManager LocRM4 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectFinances", typeof(ViewProject).Assembly);

		private bool isMSProject = false;

		protected void Page_Load(object sender, EventArgs e)
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
				Response.Redirect("~/Common/NotExistingId.aspx?AD=1");
			}

			string path = PortalConfig.ProjectInfoControlDefaultValue;
			if (File.Exists(Server.MapPath(PortalConfig.ProjectInfoControl)))
				path = PortalConfig.ProjectInfoControl;
			System.Web.UI.Control control = LoadControl(path);
			InfoPlaceHolder.Controls.Add(control);

			BindToolbar();

			if (!IsPostBack)
			{
				Mediachase.IBN.Business.Project.AddHistory(ProjectId, Mediachase.IBN.Business.Project.GetProjectTitle(ProjectId));
			}

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			cm.AddCommand("Project", "", "ProjectView", "MC_PM_RelatedPrjAdd");
			cm.AddCommand("Project", "", "ProjectView", "MC_PM_Redirect");
		}

		#region BindToolbar()
		private void BindToolbar()
		{
			CommandManager cm = CommandManager.GetCurrent(this.Page);

			secHeader.Title = LocRM.GetString("QuickTools");

			bool canUpdate = Project.CanUpdate(ProjectId);
			bool canViewFinances = Project.CanViewFinances(ProjectId);
			bool canDelete = Project.CanDelete(ProjectId);
			bool isExternal = Mediachase.IBN.Business.Security.CurrentUser.IsExternal;

			ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = LocRM.GetString("Actions");
			topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			ComponentArt.Web.UI.MenuItem subItem;

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

			createItem.Text = LocRM.GetString("Create");
			createItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
			createItem.Look.LeftIconHeight = Unit.Pixel(5);
			createItem.Look.LeftIconWidth = Unit.Pixel(16);
			createItem.LookId = "TopItemLook";

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
				editItem.Text = LocRM.GetString("Edit");
				editItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
				editItem.Look.LeftIconHeight = Unit.Pixel(5);
				editItem.Look.LeftIconWidth = Unit.Pixel(16);
				editItem.LookId = "TopItemLook";

				#region Edit Project
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/project_edit.gif";
				subItem.NavigateUrl = "~/Projects/ProjectEdit.aspx?ProjectId=" + ProjectId + "&Back=project";
				subItem.Text = LocRM.GetString("Project");
				editItem.Items.Add(subItem);
				#endregion

				#region Participants: Team, Sponsors, Stakeholders, Managers
				ComponentArt.Web.UI.MenuItem participantsItem = new ComponentArt.Web.UI.MenuItem();
				participantsItem.Text = LocRM.GetString("tParticipants");
				participantsItem.Look.RightIconUrl = "~/Layouts/Images/arrow_right.gif";

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
				subItem.ClientSideCommand = String.Format(CultureInfo.InvariantCulture,
					"javascript:ShowWizard('{0}?ProjectId={1}', 400, 250);",
					ResolveClientUrl("~/Projects/EditTargetTimeline.aspx"),
					ProjectId);
				subItem.Text = LocRM.GetString("EditTargetTimeline");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit Actual Timileme
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = String.Format(CultureInfo.InvariantCulture,
					"javascript:ShowWizard('{0}?ProjectId={1}', 400, 250);",
					ResolveClientUrl("~/Projects/EditActualTimeline.aspx"),
					ProjectId);
				subItem.Text = LocRM.GetString("EditActualTimeline");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit State Info
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = String.Format(CultureInfo.InvariantCulture,
					"javascript:ShowWizard('{0}?ProjectId={1}', 350, 265);",
					ResolveClientUrl("~/Projects/EditStateInfo.aspx"),
					ProjectId);
				subItem.Text = LocRM.GetString("EditStateInfo");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit General Info
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = String.Format(CultureInfo.InvariantCulture,
					"javascript:ShowWizard('{0}?ProjectId={1}', 550, 450);",
					ResolveClientUrl("~/Projects/EditGeneralInfo.aspx"),
					ProjectId);
				subItem.Text = LocRM.GetString("EditGeneralInfo");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit Configuration Info
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = String.Format(CultureInfo.InvariantCulture,
					"javascript:ShowWizard('{0}?ProjectId={1}', 380, 270);",
					ResolveClientUrl("~/Projects/EditConfigurationInfo.aspx"),
					ProjectId);
				subItem.Text = LocRM.GetString("EditConfigurationInfo");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit Categories
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = String.Format(CultureInfo.InvariantCulture,
					"javascript:ShowWizard('{0}?ProjectId={1}', 300, 450);",
					ResolveClientUrl("~/Projects/EditCategories.aspx"),
					ProjectId);
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
				subItem.ClientSideCommand = String.Format(CultureInfo.InvariantCulture,
					"javascript:ShowWizard('{0}?ProjectId={1}', 650, 280);",
					ResolveClientUrl("~/Projects/EditSecurity.aspx"),
					ProjectId);
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
			subItem.ClientSideCommand = String.Format(CultureInfo.InvariantCulture,
				"javascript:OpenWindow('{0}?ProjectId={1}',520,270,false);",
				ResolveClientUrl("~/Common/CommentAdd.aspx"),
				ProjectId);
			subItem.Text = LocRM.GetString("CreateComment");
			topMenuItem.Items.Add(subItem);
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
				subItem.ClientSideCommand = String.Format(CultureInfo.InvariantCulture,
					"javascript:ShowWizard2('{0}?ProjectId={1}', 750, 466, true);",
					ResolveClientUrl("~/Reports/OverallProjectSnapshot.aspx"),
					ProjectId);
				subItem.Text = LocRM.GetString("Snapshot");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region CreateTemplate2 //DV
			if (canUpdate && !isMSProject)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/newtemplate.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = String.Format(CultureInfo.InvariantCulture,
					"javascript:ShowWizard('{0}?ProjectId={1}', 640, 600);",
					ResolveClientUrl("~/Projects/EditProjectTemplate2.aspx"),
					ProjectId);
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
				subItem.ClientSideCommand = String.Format(CultureInfo.InvariantCulture,
					"javascript:ShowWizard('{0}?ProjectId={1}', 380, 100);",
					ResolveClientUrl("~/Projects/SaveBasePlanPopUp.aspx"),
					ProjectId);
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
				subItem.ClientSideCommand = String.Format(CultureInfo.InvariantCulture,
					"OpenWindow('{0}?ToMSPrj=1&ProjectId={1}',600,400);",
					ResolveClientUrl("~/Projects/ProjectExportImportNew.aspx"),
					ProjectId);
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
				subItem.ClientSideCommand = String.Format(CultureInfo.InvariantCulture,
					"OpenWindow('{0}?ProjectId={1}',600,410);",
					ResolveClientUrl("~/Projects/ProjectExportImportNew.aspx"),
					ProjectId);
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
			subItem.ClientSideCommand = String.Format(CultureInfo.InvariantCulture,
				"javascript:ShowWizard('{0}?ObjectId={1}&ObjectTypeId={1}', 750, 466);",
				ResolveClientUrl("~/Common/SystemEventsByObject.aspx"),
				ProjectId, 
				(int)ObjectTypes.Project);
			subItem.Text = LocRM3.GetString("UpdateHistory");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region Latest Visitors
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.ClientSideCommand = String.Format(CultureInfo.InvariantCulture,
				"javascript:ShowWizard('{0}?ObjectId={1}&ObjectTypeId={2}', 450, 266);",
				ResolveClientUrl("~/Common/LatestVisitors.aspx"),
				ProjectId, 
				(int)ObjectTypes.Project);
			subItem.Text = LocRM3.GetString("LatestVisitors");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region System Notifications
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			subItem.NavigateUrl = String.Format(CultureInfo.InvariantCulture,
				"~/Directory/SystemNotificationForObject.aspx?ObjectId={0}&ObjectTypeId={1}", 
				ProjectId, 
				((int)ObjectTypes.Project).ToString());
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
				subItem.ClientSideCommand = String.Format(CultureInfo.InvariantCulture,
					"javascript:ShowWizard('{0}?ObjectTypeId={1}&ObjectId={2}', 420, 150)",
					ResolveClientUrl("~/Directory/SystemRemindersForObject.aspx"),
					(int)ObjectTypes.Project, ProjectId);
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