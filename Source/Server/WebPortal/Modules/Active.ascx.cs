namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Resources;
	using System.Globalization;

	/// <summary>
	///		Summary description for Active.
	/// </summary>
	public partial class Active : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strActive", typeof(Active).Assembly);
		private UserLightPropertyCollection pc =  Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindSepAndPanels();
			if (!IsPostBack)
			{
				BindVisibility();
			}
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

		#region BindSepAndPanels
		private void BindSepAndPanels()
		{
			Sep1.ControlledPanel = Pan1;
			Sep2.ControlledPanel = Pan2;
			Sep3.ControlledPanel = Pan3;
			Sep4.ControlledPanel = Pan4;

			Sep1.PCValue = "Active_sep1";
			Sep2.PCValue = "Active_sep2";
			Sep3.PCValue = "Active_sep3";
			Sep4.PCValue = "Active_sep4";
		}
		#endregion

		#region BindVisibility
		private void BindVisibility()
		{
			// TODO: вытащить видимость блоков из pc
			if(!Configuration.ProjectManagementEnabled)
				Sep1.Visible = false;

			if(!Configuration.HelpDeskEnabled)
				Sep3.Visible = false;
		}
		#endregion

		#region Page_PreRender
    private void Page_PreRender(object sender, EventArgs e)
		{
			BindDG();
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			if (Sep1.Visible)
				BindDGActiveProjects();

			if (Sep2.Visible)
				BindDGActiveToDoAndTasks();

			if (Sep3.Visible)
				BindDGActiveIncidents();

			if (Sep4.Visible)
				BindDGActiveDocuments();
		}
		#endregion

		#region BindDGActiveProjects
		private void BindDGActiveProjects()
		{
			///		ProjectId, Title, ManagerId, StatusId, PercentCompleted, TargetStartDate, TargetFinishDate,
			///		IsManager, IsExecutiveManager, IsTeamMember, IsSponsor, IsStakeHolder, ProjectCode
			DataTable dt = Project.GetListActiveProjectsByUserOnlyDataTable();
			DataView dv = dt.DefaultView;

			// TODO: Вытащить значения из pc и сформировать строку фильтра
			// dv.RowFilter = "IsManager=1";

			dgActiveProjects.DataSource = dv;
			dgActiveProjects.DataBind();

			int RowCount = dgActiveProjects.Items.Count;
			if (RowCount == 0)
			{
				Sep1.Visible = false;
				Pan1.Visible = false;
			}
			else
			{
				Sep1.Title = String.Format("{0} ({1})", LocRM.GetString("ActiveProjects"), RowCount);
			}
		}
		#endregion

		#region BindDGActiveToDoAndTasks
		private void BindDGActiveToDoAndTasks()
		{
			DataTable dt = ToDo.GetListActiveToDoAndTasksByUserOnlyDataTable();
			DataView dv = dt.DefaultView;

			if(!Configuration.ProjectManagementEnabled)
				dv.RowFilter = "IsToDo=1";

			// TODO: Вытащить значения из pc и сформировать строку фильтра
			// dv.RowFilter = "IsManager=1";

			dgActiveTaskToDo.DataSource = dv;
			dv.Sort = "FinishDate DESC, CreationDate DESC";
			dgActiveTaskToDo.DataBind();

			int RowCount = dgActiveTaskToDo.Items.Count;
			if (RowCount == 0)
			{
				Sep2.Visible = false;
				Pan2.Visible = false;
			}
			else
			{
				Sep2.Title = String.Format("{0} ({1})", LocRM.GetString("ActiveTasksToDo"), RowCount);
			}
		}
		#endregion

		#region BindDGActiveIncidents
		private void BindDGActiveIncidents()
		{
			DataTable dt = Incident.GetListOpenIncidentsByUserOnlyDataTable();
			DataView dv = dt.DefaultView;
			dv.Sort = "CreationDate DESC";

			// TODO: Вытащить значения из pc и сформировать строку фильтра
			// dv.RowFilter = "IsManager=1";

			dgActiveIncidents.DataSource = dv;
			dgActiveIncidents.DataBind();

			int RowCount = dgActiveIncidents.Items.Count;
			if (RowCount == 0)
			{
				Sep3.Visible = false;
				Pan3.Visible = false;
			}
			else
			{
				Sep3.Title = String.Format("{0} ({1})", LocRM.GetString("ActiveIncidents"), RowCount);
			}
			
		}
		#endregion

		#region BindDGActiveDocuments
		private void BindDGActiveDocuments()
		{
			DataTable dt = Document.GetListActiveDocumentsByUserOnlyDataTable();
			DataView dv = dt.DefaultView;

			// TODO: Вытащить значения из pc и сформировать строку фильтра
			// dv.RowFilter = "IsManager=1";

			dgActiveDocuments.DataSource = dv;
			dgActiveDocuments.DataBind();

			int RowCount = dgActiveDocuments.Items.Count;
			if (RowCount == 0)
			{
				Sep4.Visible = false;
				Pan4.Visible = false;
			}
			else
			{
				Sep4.Title = String.Format("{0} ({1})", LocRM.GetString("ActiveDocuments"), RowCount);
			}
			
		}
		#endregion

		#region GetPriorityIcon
		protected string GetPriorityIcon(int PID,string Name)
		{
			string color = "PriorityNormal.gif";
			if (PID < 100) color = "PriorityLow.gif";
			if (PID > 500 && PID < 800) color = "PriorityHigh.gif";
			if (PID >= 800 && PID < 1000) color = "PriorityVeryHigh.gif";
			if (PID >= 1000) color = "PriorityUrgent.gif";
			Name = LocRM.GetString("Priority") + Name;
			return String.Format(CultureInfo.InvariantCulture,
				"<img width='16' height='16' src='{0}' alt='{1}' title='{1}'/>",
				ResolveClientUrl("~/layouts/images/icons/" + color),
				Name);
		}
		#endregion

		#region GetInterval
		protected string GetInterval(object oStart, object oFinish)
		{
			string retval = "&nbsp;";
			if (oStart != DBNull.Value && oFinish != DBNull.Value)
				retval = String.Format("{0} {1} {2} {3}", LocRM.GetString("from"), ((DateTime)oStart).ToShortDateString(), LocRM.GetString("till"), ((DateTime)oFinish).ToShortDateString());
			else if (oStart != DBNull.Value)
				retval = String.Format("{0} {1}", LocRM.GetString("from"), ((DateTime)oStart).ToShortDateString());
			else if (oFinish != DBNull.Value)
				retval = String.Format("{0} {1}", LocRM.GetString("till"), ((DateTime)oFinish).ToShortDateString());
			return retval;
		}
		#endregion
	}
}
