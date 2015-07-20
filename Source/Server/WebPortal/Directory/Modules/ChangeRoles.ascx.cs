namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
	using System.Text;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;

	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Lists;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for ChangeRoles.
	/// </summary>
	public partial  class ChangeRoles : System.Web.UI.UserControl
	{
		#region HTML Vars
		#endregion

		private UserLightPropertyCollection pc =  Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strChangeRoles", typeof(ChangeRoles).Assembly);

		#region UserID
		private int UserID
		{
			get
			{
				try
				{
					return int.Parse(Request["UserID"]);
				}
				catch 
				{
					return 0;
				}
			}
		}
		#endregion

		#region UserName
		private string UserName
		{
			get
			{
				if (ViewState["UserName"] == null)
				{
					String UserName = String.Empty;
					using (IDataReader rdr = User.GetUserInfo(UserID))
					{
						if (rdr.Read())
							UserName = String.Concat(rdr["LastName"], " ", rdr["FirstName"]);
					}
					ViewState["UserName"] = UserName;
					return UserName;
				}
				else 
					return (string)ViewState["UserName"];
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindDGProjects();
			BindDGToDo();
			BindDGTask();
			BindDGEvent();
			BindDGIncident();
			BindDGDocument();
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolbar();

		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			tbHeader.Title = String.Concat(LocRM.GetString("tbTitle")," ",UserName);

			sep1.Title = String.Concat("<img alt='' width='16' height='16' src='../layouts/images/icons/project.gif' align='absmiddle' border='0'/>&nbsp;<b>", LocRM.GetString("Projects"), "</b>");
			sep2.Title = String.Concat("<img alt='' width='16' height='16' src='../layouts/images/icons/task.gif' align='absmiddle' border='0'/>&nbsp;<b>", LocRM.GetString("ToDos"), "</b>");
			sep3.Title = String.Concat("<img alt='' width='16' height='16' src='../layouts/images/icons/task1.gif' align='absmiddle' border='0'/>&nbsp;<b>", LocRM.GetString("Tasks"), "</b>");
			sep4.Title = String.Concat("<img alt='' width='16' height='16' src='../layouts/images/icons/event.gif' align='absmiddle' border='0'/>&nbsp;<b>", LocRM.GetString("Events"), "</b>");
			sep5.Title = String.Concat("<img alt='' width='16' height='16' src='../layouts/images/icons/Incident.gif' align='absmiddle' border='0'/>&nbsp;<b>", LocRM.GetString("Incidents"), "</b>");
			sep6.Title = String.Concat("<img alt='' width='16' height='16' src='../layouts/images/icons/Document.gif' align='absmiddle' border='0'/>&nbsp;<b>", LocRM.GetString("Documents"), "</b>");
		}
		#endregion

		
		#region GetBool
		protected bool GetBool(int val)
		{
			if (val == 1) 
				return true;
			else
				return false;
		}
		#endregion

		#region GetEventLink
		protected string GetEventLink(int EventId, string Title,int CanView)
		{
			string img =  "<img src='../layouts/images/icons/event.gif' width='16' height='16' align='absmiddle' border=0>&nbsp;";
			if (CanView == 1)
				return String.Concat(img,"<a href='../events/eventview.aspx?EventID="+ EventId+"'>",Title,"</a>");
			else
				return String.Concat(img,Title);
		}
		#endregion

		#region GetProjectRole(int IsM, int IsEM, int IsTM,int IsSP, int IsSH)
		protected string GetProjectRole(int IsM, int IsEM, int IsTM,int IsSP, int IsSH)
		{
			bool showdot = false;
			String dotpattern = ", ";
			StringBuilder role = new StringBuilder();
			if (IsM == 1)
			{
				role.Append(LocRM.GetString("Manager"));
				showdot = true;
			}

			if (IsEM == 1)
			{
				if (showdot) role.Append(dotpattern);
				else showdot = true;
				role.Append(LocRM.GetString("ExecutiveManager"));
			}

			if (IsTM == 1)
			{
				if (showdot) role.Append(dotpattern);
				else showdot = true;
				role.Append(LocRM.GetString("TeamMemeber"));
			}

			if (IsSP == 1)
			{
				if (showdot) role.Append(dotpattern);
				else showdot = true;
				role.Append(LocRM.GetString("Sponsor"));
			}

			if (IsSH == 1)
			{
				if (showdot) role.Append(dotpattern);
				role.Append(LocRM.GetString("Stakeholder"));
			}
			return role.ToString();
			
		}
		#endregion

		#region GetToDoRole(int IsM, int IsR)
		protected string GetToDoRole(int IsM, int IsR)
		{
			bool showdot = false;
			String dotpattern = ", ";
			StringBuilder role = new StringBuilder();
			if (IsM == 1)
			{
				role.Append(LocRM.GetString("Manager"));
				showdot = true;
			}

			if (IsR == 1)
			{
				if (showdot) role.Append(dotpattern);
				role.Append(LocRM.GetString("Resource"));
			}

			return role.ToString();
		}
		#endregion

		#region BindDGProjects
		private void BindDGProjects()
		{
			dgPrj.Columns[0].HeaderText = LocRM.GetString("Title");
			dgPrj.Columns[1].HeaderText = LocRM.GetString("Roles");
			dgPrj.Columns[2].HeaderText = LocRM.GetString("Options");
			dgPrj.DataSource = Project.GetListProjectsForChangeableRoles(UserID);
			dgPrj.DataBind();
		}
		#endregion

		#region BindDGIncident
		private void BindDGIncident()
		{
			dgIncidents.Columns[0].HeaderText = LocRM.GetString("Title");
			dgIncidents.Columns[1].HeaderText = LocRM.GetString("Roles");
			dgIncidents.Columns[2].HeaderText = LocRM.GetString("Options");
			dgIncidents.DataSource = Incident.GetListIncidentsForChangeableRoles(UserID);
			dgIncidents.DataBind();
		}
		#endregion

		#region BindDGDocument
		private void BindDGDocument()
		{
			dgDocuments.Columns[0].HeaderText = LocRM.GetString("Title");
			dgDocuments.Columns[1].HeaderText = LocRM.GetString("Roles");
			dgDocuments.Columns[2].HeaderText = LocRM.GetString("Options");
			dgDocuments.DataSource = Document.GetListDocumentsForChangeableRoles(UserID);
			dgDocuments.DataBind();
		}
		#endregion


		#region BindDGTask
		private void BindDGTask()
		{
			dgTasks.Columns[0].HeaderText = LocRM.GetString("Title");
			dgTasks.Columns[1].HeaderText = LocRM.GetString("Roles");
			dgTasks.Columns[2].HeaderText = LocRM.GetString("Options");
			dgTasks.DataSource = Task.GetListTasksForChangeableRoles(UserID);
			dgTasks.DataBind();
		}
		#endregion

		#region BindDGToDo
		private void BindDGToDo()
		{
			dgToDo.Columns[0].HeaderText = LocRM.GetString("Title");
			dgToDo.Columns[1].HeaderText = LocRM.GetString("Roles");
			dgToDo.Columns[2].HeaderText = LocRM.GetString("Options");
			dgToDo.DataSource = Mediachase.IBN.Business.ToDo.GetListToDoForChangeableRoles(UserID);
			dgToDo.DataBind();
		}
		#endregion

		#region BindDGEvent
		private void BindDGEvent()
		{
			dgEvent.Columns[0].HeaderText = LocRM.GetString("Title");
			dgEvent.Columns[1].HeaderText = LocRM.GetString("Roles");
			dgEvent.Columns[2].HeaderText = LocRM.GetString("Options");
			dgEvent.DataSource = CalendarEntry.GetListEventsForChangeableRoles(UserID);
			dgEvent.DataBind();
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region lblDeleteProjectAll_Click
		protected void lblDeleteProjectAll_Click(object sender, System.EventArgs e)
		{
			int projectId = int.Parse(hdnProjectId.Value);
			ListManager.DeleteProjectRoot(projectId);
			Project.Delete(projectId);
			BindDGProjects();
		}
		#endregion

		#region lbDeleteToDoAll_Click
		protected void lbDeleteToDoAll_Click(object sender, System.EventArgs e)
		{
			int ToDoId = int.Parse(hdnToDoId.Value);
			ToDo.Delete(ToDoId);
			BindDGToDo();
		}
		#endregion

		#region lblDeleteTaskAll_Click
		protected void lblDeleteTaskAll_Click(object sender, System.EventArgs e)
		{
			int TaskId = int.Parse(hdnTaskId.Value);
			Task.Delete(TaskId);
			BindDGTask();
		}
		#endregion

		#region lbEventDeleteAll_Click
		protected void lbEventDeleteAll_Click(object sender, System.EventArgs e)
		{
			int EventId = int.Parse(hdnEventId.Value);
			CalendarEntry.Delete(EventId);
			BindDGEvent();
		}
		#endregion

		#region lbIncidentDeleteAll_Click
		protected void lbIncidentDeleteAll_Click(object sender, System.EventArgs e)
		{
			int IncidentId = int.Parse(hdnIncidentId.Value);
			Incident.Delete(IncidentId);
			BindDGIncident();
		}
		#endregion

		#region lbDocumentDeleteAll_Click
		protected void lbDocumentDeleteAll_Click(object sender, EventArgs e)
		{
			int DocumentId = int.Parse(hdnDocumentId.Value);
			Document.Delete(DocumentId);
			BindDGDocument();
		}
		#endregion
	}
}
