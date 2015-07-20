namespace Mediachase.UI.Web.Workspace.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Globalization;

	/// <summary>
	///		Summary description for Customizer.
	/// </summary>
	public partial  class Customizer : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.RequiredFieldValidator rfvMyDays;
		protected System.Web.UI.WebControls.RequiredFieldValidator rfvDays;
		protected System.Web.UI.WebControls.TextBox tbRecsPerPage;
		protected System.Web.UI.WebControls.RequiredFieldValidator rvfRPP;
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strCustomizer", typeof(Customizer).Assembly);
		protected System.Web.UI.WebControls.RangeValidator rvMyDays;
		protected System.Web.UI.WebControls.RangeValidator rvDays;
		protected System.Web.UI.WebControls.RangeValidator rvRecsPerPage;
		private UserLightPropertyCollection pc =  Security.CurrentUser.Properties;



		protected void Page_Load(object sender, System.EventArgs e)
		{
			cbShowMyList.Style.Add("display","none");
			Page.EnableViewState = false;
			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");
			btnCancel.Attributes.Add("onclick","DisableButtons(this);");
			btnSave.Attributes.Add("onclick","DisableButtons(this);");
			FillddLists();
			if (!IsPostBack)
			{
				BindSavedData();
				BindVisibility();
			}

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "SetMyUpdatesEnabled();SetUpdatesEnabled();SetShowActivities();SetShowActiveProjects();SetShowThisWeek();", true);
			DataBind();

			tbHeader.Title = LocRM.GetString("tbTitle");
		}

		private void BindVisibility()
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Administrator))
				cbShowNewPending.Visible = false;

			if (!(Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager))
				)
				cbShowIssuesYouNeed.Visible = false;

			if (!Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager))
				cbShowExternalEmail.Visible = false;

		}

		private void FillddLists()
		{
			ddDays.Items.Add(new ListItem("1"));
			ddDays.Items.Add(new ListItem("3"));
			ddDays.Items.Add(new ListItem("10"));
			ddDays.Items.Add(new ListItem("30"));

			ddMyDays.Items.Add(new ListItem("1"));
			ddMyDays.Items.Add(new ListItem("3"));
			ddMyDays.Items.Add(new ListItem("10"));
			ddMyDays.Items.Add(new ListItem("30"));

			ddRecsPerPage.Items.Add(new ListItem("3"));
			ddRecsPerPage.Items.Add(new ListItem("5"));
			ddRecsPerPage.Items.Add(new ListItem("10"));
			ddRecsPerPage.Items.Add(new ListItem("20"));

			ddTWDaysBefore.Items.Add(new ListItem(LocRM.GetString("TWToday"),"0"));
			ddTWDaysBefore.Items.Add(new ListItem(LocRM.GetString("TW1Day"),"1"));
			ddTWDaysBefore.Items.Add(new ListItem(LocRM.GetString("TW2Day"),"2"));
		}

		private void BindSavedData()
		{

			#region My Updates
			if (pc["USetup_ShowMyUpdates"]==null)
				cbShowMyUpdates.Checked = true;
			else
				cbShowMyUpdates.Checked = bool.Parse(pc["USetup_ShowMyUpdates"]);

			if (pc["USetup_ShowMyProjects"]==null)
				cbShowMyProjects.Checked=true;
			else
				cbShowMyProjects.Checked = bool.Parse(pc["USetup_ShowMyProjects"]);

			if (pc["USetup_ShowMyIncidents"]==null)
				cbShowMyIncidents.Checked=true;
			else
				cbShowMyIncidents.Checked = bool.Parse(pc["USetup_ShowMyIncidents"]);

			if (pc["USetup_ShowMyDocuments"]==null)
				cbShowMyDocument.Checked=true;
			else
				cbShowMyDocument.Checked = bool.Parse(pc["USetup_ShowMyDocuments"]);

			if (pc["USetup_ShowMyEvents"]==null)
				cbShowMyCalendarEntries.Checked=true;
			else
				cbShowMyCalendarEntries.Checked = bool.Parse(pc["USetup_ShowMyEvents"]);

			if (pc["USetup_ShowMyTasks"]==null)
				cbShowMyTaskToDo.Checked=true;
			else
				cbShowMyTaskToDo.Checked = bool.Parse(pc["USetup_ShowMyTasks"]);

			if (pc["USetup_ShowMyLists"]==null)
				cbShowMyList.Checked=true;
			else
				cbShowMyList.Checked = bool.Parse(pc["USetup_ShowMyLists"]);

			if (pc["USetup_MyUpdatesLastDays"]==null || pc["USetup_MyUpdatesLastDays"]==String.Empty)
				CommonHelper.SafeSelect(ddMyDays,"10");
			else
				CommonHelper.SafeSelect(ddMyDays,pc["USetup_MyUpdatesLastDays"]);
#endregion

			#region All Updates
			if (pc["USetup_ShowUpdates"]==null)
				cbShowAllUpdates.Checked = true;
			else
				cbShowAllUpdates.Checked = bool.Parse(pc["USetup_ShowUpdates"]);

			if (pc["USetup_ShowProjects"]==null)
				cbShowProjects.Checked=true;
			else
				cbShowProjects.Checked = bool.Parse(pc["USetup_ShowProjects"]);

			if (pc["USetup_ShowIncidents"]==null)
				cbShowIncidents.Checked=true;
			else
				cbShowIncidents.Checked = bool.Parse(pc["USetup_ShowIncidents"]);

			if (pc["USetup_ShowDocuments"]==null)
				cbShowDocument.Checked=true;
			else
				cbShowDocument.Checked = bool.Parse(pc["USetup_ShowDocuments"]);

			if (pc["USetup_ShowEvents"]==null)
				cbShowCalendarEntries.Checked=true;
			else
				cbShowCalendarEntries.Checked = bool.Parse(pc["USetup_ShowEvents"]);

			if (pc["USetup_ShowTasks"]==null)
				cbShowTaskToDo.Checked=true;
			else
				cbShowTaskToDo.Checked = bool.Parse(pc["USetup_ShowTasks"]);

			if (pc["USetup_ShowLists"]==null)
				cbShowList.Checked=true;
			else
				cbShowList.Checked = bool.Parse(pc["USetup_ShowLists"]);
			
			if (pc["USetup_UpdatesLastDays"]==null || pc["USetup_UpdatesLastDays"]==String.Empty)
				CommonHelper.SafeSelect(ddDays,"10");
			else
				CommonHelper.SafeSelect(ddDays,pc["USetup_UpdatesLastDays"]);
#endregion

			#region ShowActivities
			if (pc["USetup_ShowActivities"]==null)
				cbShowActivities.Checked = true;
			else
				cbShowActivities.Checked = bool.Parse(pc["USetup_ShowActivities"]);

			if (pc["USetup_ShowNewPending"]==null)
				cbShowNewPending.Checked = true;
			else
				cbShowNewPending.Checked = bool.Parse(pc["USetup_ShowNewPending"]);
			
			if (pc["USetup_ShowIssuesYouNeed"]==null)
				cbShowIssuesYouNeed.Checked = true;
			else
				cbShowIssuesYouNeed.Checked = bool.Parse(pc["USetup_ShowIssuesYouNeed"]);
			
			if (pc["USetup_ShowExternalEmail"]==null)
				cbShowExternalEmail.Checked = true;
			else
				cbShowExternalEmail.Checked = bool.Parse(pc["USetup_ShowExternalEmail"]);

			if (pc["USetup_ShowCompletedTasksToDos"]==null)
				cbShowCompletedTasksToDos.Checked = true;
			else
				cbShowCompletedTasksToDos.Checked = bool.Parse(pc["USetup_ShowCompletedTasksToDos"]);

			if (pc["USetup_ShowTasksToDosAwaiting"]==null)
				cbShowTasksToDosAwaiting.Checked = true;
			else
				cbShowTasksToDosAwaiting.Checked = bool.Parse(pc["USetup_ShowTasksToDosAwaiting"]);
			
			if (pc["USetup_ShowEventsAssignments"]==null)
				cbShowEventsAssignments.Checked = true;
			else
				cbShowEventsAssignments.Checked = bool.Parse(pc["USetup_ShowEventsAssignments"]);

			if (pc["USetup_ShowTasksToDosAssign"]==null)
				cbShowTasksToDosAssign.Checked = true;
			else
				cbShowTasksToDosAssign.Checked = bool.Parse(pc["USetup_ShowTasksToDosAssign"]);

			if (pc["USetup_ShowIncidentsAssignments"]==null)
				cbShowIssuesAssignments.Checked = true;
			else
				cbShowIssuesAssignments.Checked = bool.Parse(pc["USetup_ShowIncidentsAssignments"]);

			if (pc["USetup_ShowPendingTimeSheets"]==null)
				cbShowPendingTimesheets.Checked = true;
			else
				cbShowPendingTimesheets.Checked = bool.Parse(pc["USetup_ShowPendingTimeSheets"]);

			#endregion

			#region ShowActiveProjects
			if (pc["USetup_ShowActiveProjects"]==null)
				cbShowActiveProjects.Checked=true;
			else
				cbShowActiveProjects.Checked = bool.Parse(pc["USetup_ShowActiveProjects"]);

			if (pc["USetup_ShowActiveProjectsInvolved"]==null)
				cbShowActiveProjectsInvolved.Checked=true;
			else
				cbShowActiveProjectsInvolved.Checked = bool.Parse(pc["USetup_ShowActiveProjectsInvolved"]);

			if (pc["USetup_ShowTasksToDosInvolved"]==null)
				cbShowTasksToDosInvolved.Checked=true;
			else
				cbShowTasksToDosInvolved.Checked = bool.Parse(pc["USetup_ShowTasksToDosInvolved"]);

			if (pc["USetup_ShowIssuesResponsible"]==null)
				cbShowIssuesResponsible.Checked=true;
			else
				cbShowIssuesResponsible.Checked = bool.Parse(pc["USetup_ShowIssuesResponsible"]);

			if (pc["USetup_ShowDocumentsTasks"]==null)
				cbShowDocumentsTasks.Checked = true;
			else
				cbShowDocumentsTasks.Checked = bool.Parse(pc["USetup_ShowDocumentsTasks"]);			
			#endregion

			#region ShowThisWeek
			if (pc["USetup_ShowThisWeek"]==null)
				cbShowThisWeek.Checked=true;
			else
				cbShowThisWeek.Checked = bool.Parse(pc["USetup_ShowThisWeek"]);

			if (pc["USetup_ThisWeekDaysBefore"]!=null)
				CommonHelper.SafeSelect(ddTWDaysBefore,pc["USetup_ThisWeekDaysBefore"]);
			else
				CommonHelper.SafeSelect(ddTWDaysBefore,"0");
			#endregion

			if (pc["USetup_ShowMostRecentTimesheet"]==null)
				cbShowMostRecentTimesheet.Checked=true;
			else
				cbShowMostRecentTimesheet.Checked = bool.Parse(pc["USetup_ShowMostRecentTimesheet"]);

			if (pc["USetup_CollapseBlocks"]==null)
				cbCollapseBlocks.Checked=true;
			else
				cbCollapseBlocks.Checked = bool.Parse(pc["USetup_CollapseBlocks"]);

			if (pc["USetup_ShowGettingStarted"]==null)
				cbShowGettingStarted.Checked = true;
			else
				cbShowGettingStarted.Checked = bool.Parse(pc["USetup_ShowGettingStarted"]);			

			if (pc["USetup_RecsPerPage"]==null || pc["USetup_RecsPerPage"]==String.Empty)
				CommonHelper.SafeSelect(ddRecsPerPage,"5");
			else
				CommonHelper.SafeSelect(ddRecsPerPage,pc["USetup_RecsPerPage"]);

			cbShowMyUpdates.Attributes.Add("onclick","SetMyUpdatesEnabled()");
			cbShowAllUpdates.Attributes.Add("onclick","SetUpdatesEnabled()");
			cbShowActivities.Attributes.Add("onclick","SetShowActivities()");
			cbShowActiveProjects.Attributes.Add("onclick","SetShowActiveProjects()");
			cbShowThisWeek.Attributes.Add("onclick","SetShowThisWeek()");
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			pc["USetup_ShowMyUpdates"] = cbShowMyUpdates.Checked.ToString();
			if (cbShowMyUpdates.Checked)
			{
				pc["USetup_ShowMyProjects"] = cbShowMyProjects.Checked.ToString();
				pc["USetup_ShowMyIncidents"] = cbShowMyIncidents.Checked.ToString();
				pc["USetup_ShowMyDocuments"] = cbShowMyDocument.Checked.ToString();
				pc["USetup_ShowMyEvents"] = cbShowMyCalendarEntries.Checked.ToString();
				pc["USetup_ShowMyTasks"] = cbShowMyTaskToDo.Checked.ToString();
				pc["USetup_ShowMyLists"] = cbShowMyList.Checked.ToString();
				pc["USetup_MyUpdatesLastDays"] = ddMyDays.SelectedItem.Value;
			}

			pc["USetup_ShowUpdates"] = cbShowAllUpdates.Checked.ToString();
			if (cbShowAllUpdates.Checked)
			{
				pc["USetup_ShowProjects"] = cbShowProjects.Checked.ToString();
				pc["USetup_ShowIncidents"] = cbShowIncidents.Checked.ToString();
				pc["USetup_ShowDocuments"] = cbShowDocument.Checked.ToString();
				pc["USetup_ShowEvents"] = cbShowCalendarEntries.Checked.ToString();
				pc["USetup_ShowTasks"] = cbShowTaskToDo.Checked.ToString();
				pc["USetup_ShowLists"] = cbShowList.Checked.ToString();
				pc["USetup_UpdatesLastDays"] = ddDays.SelectedItem.Value;
			}

			pc["USetup_ShowActivities"] = cbShowActivities.Checked.ToString();
			if (cbShowActivities.Checked)
			{
				pc["USetup_ShowNewPending"] = cbShowNewPending.Checked.ToString();
				pc["USetup_ShowIssuesYouNeed"] = cbShowIssuesYouNeed.Checked.ToString();
				pc["USetup_ShowExternalEmail"] = cbShowExternalEmail.Checked.ToString();
				pc["USetup_ShowCompletedTasksToDos"] = cbShowCompletedTasksToDos.Checked.ToString();
				pc["USetup_ShowTasksToDosAwaiting"] = cbShowTasksToDosAwaiting.Checked.ToString();
				pc["USetup_ShowEventsAssignments"] = cbShowEventsAssignments.Checked.ToString();
				pc["USetup_ShowTasksToDosAssign"] = cbShowTasksToDosAssign.Checked.ToString();
				pc["USetup_ShowIncidentsAssignments"] = cbShowIssuesAssignments.Checked.ToString();
				pc["USetup_ShowPendingTimeSheets"] = cbShowPendingTimesheets.Checked.ToString();
			}

			pc["USetup_ShowActiveProjects"] = cbShowActiveProjects.Checked.ToString();
			if (cbShowActiveProjects.Checked)
			{
				pc["USetup_ShowActiveProjectsInvolved"] = cbShowActiveProjectsInvolved.Checked.ToString();
				pc["USetup_ShowTasksToDosInvolved"] = cbShowTasksToDosInvolved.Checked.ToString();
				pc["USetup_ShowIssuesResponsible"] = cbShowIssuesResponsible.Checked.ToString();
				pc["USetup_ShowDocumentsTasks"] = cbShowDocumentsTasks.Checked.ToString();
			}

			pc["USetup_ShowThisWeek"] = cbShowThisWeek.Checked.ToString();
			if (cbShowThisWeek.Checked)
			{
				pc["USetup_ThisWeekDaysBefore"] = ddTWDaysBefore.SelectedItem.Value;
			}
			pc["USetup_ShowMostRecentTimesheet"] = cbShowMostRecentTimesheet.Checked.ToString();

			pc["USetup_RecsPerPage"] = ddRecsPerPage.SelectedItem.Value;
			pc["USetup_CollapseBlocks"] = cbCollapseBlocks.Checked.ToString();
			pc["USetup_ShowGettingStarted"] = cbShowGettingStarted.Checked.ToString();

			Response.Redirect("../Workspace/default.aspx?BTab=Workspace");
		}

		protected void btnCancelClick(object sender, System.EventArgs e)
		{
			Response.Redirect("../Workspace/default.aspx?BTab=Workspace");
		}
	}
}
