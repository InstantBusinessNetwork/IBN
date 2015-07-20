namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Reflection;
	using System.Resources;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	
	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Data;
	

	/// <summary>
	///		Summary description for ProjectActivities2.
	/// </summary>
	public partial class ProjectActivities2 : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Calendar.Resources.strCalendar", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Documents.Resources.strDocuments", typeof(ProjectActivities2).Assembly);

		private string pcKey = "TasksProjectActivities_CurrentTab";

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

		#region SubTab
		private string subTab;
		protected string SubTab
		{
			get
			{
				if (subTab == null)
				{
					subTab = "";
					if (Request["SubTab"] != null)
						subTab = Request["SubTab"];
				}
				return subTab;
			}
		}
		#endregion

		#region _canEdit
		private bool _canEdit
		{
			get
			{
				return ((Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)) ||
					(Security.CurrentUser.UserID == Project.GetProjectManager(ProjectId)));
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Add Tabs
			string link;
			string text;

			DataTable dt;

			int iIncidents = 0;
			if (Configuration.HelpDeskEnabled)
			{
				dt = Incident.GetListIncidentsByFilterDataTable(ProjectId, 0, 0, 0, 0, PrimaryKeyId.Empty, PrimaryKeyId.Empty, 0, -1, 0, 0, 0, "", 0, 0);
				iIncidents = dt.Rows.Count;
			}

			dt = Document.GetListDocumentsByFilterDataTable(ProjectId, 0, 0, -1, 0, "", 0, PrimaryKeyId.Empty, PrimaryKeyId.Empty);
			int iDocuments = dt.Rows.Count;

///			if (_canEdit)
//			{
				link = String.Format("ProjectView.aspx?ProjectId={0}&Tab=6&SubTab=ResourceView", ProjectId);
				text = LocRM.GetString("ResourceView");
				blockControl.AddTab("ResourceView", text, link, "../Projects/Modules/WorksForManagers.ascx");
//			}

			link = String.Format("ProjectView.aspx?ProjectId={0}&Tab=6&SubTab=GanttChart2", ProjectId);
			text = LocRM.GetString("GanttChart");
			blockControl.AddTab("GanttChart2", text, link, "../Projects/Modules/GanttView2.ascx");

			if (Configuration.HelpDeskEnabled)
			{
				link = String.Format("ProjectView.aspx?ProjectId={0}&Tab=6&SubTab=AllIncidents", ProjectId);
				text = String.Format("{0} <span class='ibn-number'>({1})</span>", LocRM.GetString("tIssues"), iIncidents);
				blockControl.AddTab("AllIncidents", text, link, "../Incidents/Modules/IncidentsList.ascx");
			}

			link = String.Format("ProjectView.aspx?ProjectId={0}&Tab=6&SubTab=AllDocuments", ProjectId);
			text = String.Format("{0} <span class='ibn-number'>({1})</span>", LocRM2.GetString("tDocuments"), iDocuments);
			blockControl.AddTab("AllDocuments", text, link, "../Documents/Modules/DocumentList.ascx");

			// Select Tab
			UserLightPropertyCollection pc = Security.CurrentUser.Properties;

			if ((SubTab == "ResourceView" || SubTab == "GanttChart2" || SubTab == "AllIncidents" || SubTab == "AllDocuments"))
				pc[pcKey] = SubTab;
			else if (pc[pcKey] == null)
				pc[pcKey] = "ResourceView";

			if (!Configuration.HelpDeskEnabled && pc[pcKey] == "AllIncidents")
				pc[pcKey] = "ResourceView";

			blockControl.SelectTab(pc[pcKey]);
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
		}
		#endregion
	}
}
