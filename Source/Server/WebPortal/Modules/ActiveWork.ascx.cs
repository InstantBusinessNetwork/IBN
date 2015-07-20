namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.EMail;
	using Mediachase.UI.Web.Util;
	using System.Resources;
	using System.Globalization;

	/// <summary>
	///		Summary description for ActiveWork.
	/// </summary>
	public partial class ActiveWork : System.Web.UI.UserControl
	{

    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strResAsts", typeof(ActiveWork).Assembly);
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

		#region BindVisibility
		private void BindVisibility()
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Administrator))
			{
				Sep1.Visible = false;
				Pan1.Visible = false;
			}

			if (!Configuration.HelpDeskEnabled ||
				!(Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) ||
				Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager))
				)
			{
				Sep2.Visible = false;
				Pan2.Visible = false;
			}

//			if (!(Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
//				Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) ||
//				Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager))
//				)
//			{
				Sep3.Visible = false;
				Pan3.Visible = false;
//			}

			if (!Configuration.HelpDeskEnabled)
			{
				Sep6.Visible = false;
				Pan6.Visible = false;
			}
		}
		#endregion

		#region BindSepAndPanels
		private void BindSepAndPanels()
		{
			Sep1.ControlledPanel = Pan1;
			Sep2.ControlledPanel = Pan2;
			Sep3.ControlledPanel = Pan3;
			Sep4.ControlledPanel = Pan4;
			Sep5.ControlledPanel = Pan5;
			Sep6.ControlledPanel = Pan6;
			Sep7.ControlledPanel = Pan7;
			Sep8.ControlledPanel = Pan8;
			Sep10.ControlledPanel = Pan10;
			Sep11.ControlledPanel = Pan11;
			Sep12.ControlledPanel = Pan12;

			Sep1.PCValue = "ActiveWork_sep1";
			Sep2.PCValue = "ActiveWork_sep2";
			Sep3.PCValue = "ActiveWork_sep3";
			Sep4.PCValue = "ActiveWork_sep4";
			Sep5.PCValue = "ActiveWork_sep5";
			Sep6.PCValue = "ActiveWork_sep6";
			Sep7.PCValue = "ActiveWork_sep7";
			Sep8.PCValue = "ActiveWork_sep8";
			Sep10.PCValue = "ActiveWork_sep10";
			Sep11.PCValue = "ActiveWork_sep11";
			Sep12.PCValue = "ActiveWork_sep12";
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			if (Sep1.Visible)
				BindDGPending();
			if (Sep2.Visible)
				BindDGIncidents();
			if (Sep3.Visible)
				BindDGMailIncidents();
			if (Sep4.Visible)
				BindDGNotApprovedTaskToDo();
			if (Sep5.Visible)
				BindDGAssignments();
			if (Sep6.Visible)
				BindDGPendingIncidents();
			if (Sep7.Visible)
				BindDGPendingEvents();
			if (Sep8.Visible)
				BindDGNotAssigned();
			if (Sep10.Visible)
				BindDGNotAssignedDocuments();
			if (Sep11.Visible)
				BindDGPendingDocuments();
			if (Sep12.Visible)
				BindDGDeclinedRequests();
		}
		#endregion

		#region BindDGPending
		private void BindDGPending()
		{
			dgPendingUsers.DataSource = User.GetListPendingUsers();
			dgPendingUsers.DataBind();

			int RowCount = dgPendingUsers.Items.Count;
			if (RowCount == 0)
			{
				Sep1.Visible = false;
				Pan1.Visible = false;
			}
			else
			{
				Sep1.Title = String.Format("{0} ({1})", LocRM.GetString("US"), RowCount);
			}
		}
		#endregion

		#region BindDGIncidents
		private void BindDGIncidents()
		{
			dgIncidents.DataSource = Incident.GetListNotAssignedIncidentsDataTable(0);
			dgIncidents.DataBind();

			int RowCount = dgIncidents.Items.Count;
			if (RowCount == 0)
			{
				Sep2.Visible = false;
				Pan2.Visible = false;
			}
			else
			{
				Sep2.Title = String.Format("{0} ({1})", LocRM.GetString("NI"), RowCount);
			}
		}
		#endregion

		#region BindDGMailIncidents
		private void BindDGMailIncidents()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("PendingMessageId", typeof(int)));
//			dt.Columns.Add(new DataColumn("From", typeof(string)));
//			dt.Columns.Add(new DataColumn("To", typeof(string)));
			dt.Columns.Add(new DataColumn("Subject", typeof(string)));
			dt.Columns.Add(new DataColumn("Created", typeof(DateTime)));
			DataRow dr;
			int[] pendList = EMailMessage.ListPendigEMailMessageIds();
			foreach(int id in pendList)
			{
				dr = dt.NewRow();
				dr["PendingMessageId"] = id;
				EMailMessageInfo emi = EMailMessageInfo.Load(id);
//				dr["From"] = GetAddress(emi.From);
//				dr["To"] = GetAddress(emi.To);
				dr["Subject"] = emi.Subject;
				dr["Created"] = emi.Created;
				dt.Rows.Add(dr);
			}
			DataView dv = dt.DefaultView;
			dv.Sort = "Created DESC";
			
			dgMailIncidents.DataSource = dv;
			dgMailIncidents.DataBind();

			int RowCount = dgMailIncidents.Items.Count;
			if (RowCount == 0)
			{
				Sep3.Visible = false;
				Pan3.Visible = false;
			}
			else
			{
				Sep3.Title = String.Format("{0} ({1})", LocRM.GetString("MI"), RowCount);
			}
		}
		#endregion

		#region BindDGNotApprovedTaskToDo
		private void BindDGNotApprovedTaskToDo()
		{
			DataView dv = ToDo.GetListNotApprovedToDoAndTasks(0).DefaultView;
			if(!Configuration.ProjectManagementEnabled)
				dv.RowFilter = "IsToDo=1";
			dgNotApprovedTaskToDo.DataSource = dv;
			dgNotApprovedTaskToDo.DataBind();

			int RowCount = dgNotApprovedTaskToDo.Items.Count;
			if (RowCount == 0)
			{
				Sep4.Visible = false;
				Pan4.Visible = false;
			}
			else
			{
				Sep4.Title = String.Format("{0} ({1})", LocRM.GetString("AC"), RowCount);
			}
		}
		#endregion

		#region BindDGAssignments
		private void BindDGAssignments()
		{
			DataView dv = ToDo.GetListPendingToDoAndTasks(0).DefaultView;
			if(!Configuration.ProjectManagementEnabled)
				dv.RowFilter = "IsToDo=1";

			dgAssignments.DataSource = dv;
			dgAssignments.DataBind();

			int RowCount = dgAssignments.Items.Count;
			if (RowCount == 0)
			{
				Sep5.Visible = false;
				Pan5.Visible = false;
			}
			else
			{
				Sep5.Title = String.Format("{0} ({1})", LocRM.GetString("PA"), RowCount);
			}
		}
		#endregion

		#region BindDGPendingIncidents
		private void BindDGPendingIncidents()
		{
			dgIncPending.DataSource = Incident.GetListPendingIncidentsDataTable(0);
			dgIncPending.DataBind();

			int RowCount = dgIncPending.Items.Count;
			if (RowCount == 0)
			{
				Sep6.Visible = false;
				Pan6.Visible = false;
			}
			else
			{
				Sep6.Title = String.Format("{0} ({1})", LocRM.GetString("IA"), RowCount);
			}
		}
		#endregion

		#region BindDGPendingEvents
		private void BindDGPendingEvents()
		{
			dgPendingEvents.DataSource = CalendarEntry.GetListPendingEventsDataTable(0);
			dgPendingEvents.DataBind();

			int RowCount = dgPendingEvents.Items.Count;
			if (RowCount == 0)
			{
				Sep7.Visible = false;
				Pan7.Visible = false;
			}
			else
			{
				Sep7.Title = String.Format("{0} ({1})", LocRM.GetString("CA"), RowCount);
			}
		}
		#endregion

		#region BindDGNotAssigned
		private void BindDGNotAssigned()
		{
			DataView dv = ToDo.GetListToDoAndTasksWithoutResources(0).DefaultView;
			if(!Configuration.ProjectManagementEnabled)
				dv.RowFilter = "IsToDo=1";

			dgNotAssigned.DataSource = dv;
			dgNotAssigned.DataBind();

			int RowCount = dgNotAssigned.Items.Count;
			if (RowCount == 0)
			{
				Sep8.Visible = false;
				Pan8.Visible = false;
			}
			else
			{
				Sep8.Title = String.Format("{0} ({1})", LocRM.GetString("NA"), RowCount);
			}
		}
		#endregion

		#region BindDGNotAssignedDocuments
		private void BindDGNotAssignedDocuments()
		{
			dgDocuments.DataSource = Document.GetListNotAssignedDocumentsDataTable(0);
			dgDocuments.DataBind();

			int RowCount = dgDocuments.Items.Count;
			if (RowCount == 0)
			{
				Sep10.Visible = false;
				Pan10.Visible = false;
			}
			else
			{
				Sep10.Title = String.Format("{0} ({1})", LocRM.GetString("ND"), RowCount);
			}
		}
		#endregion

		#region BindDGPendingDocuments
		private void BindDGPendingDocuments()
		{
			dgPendingDocuments.DataSource = Document.GetListPendingDocumentsDataTable(0);
			dgPendingDocuments.DataBind();

			int RowCount = dgPendingDocuments.Items.Count;
			if (RowCount == 0)
			{
				Sep11.Visible = false;
				Pan11.Visible = false;
			}
			else
			{
				Sep11.Title = String.Format("{0} ({1})", LocRM.GetString("DA"), RowCount);
			}
		}
		#endregion

		#region BindDGDeclinedRequests
		private void BindDGDeclinedRequests()
		{
			dgDeclinedRequests.DataSource = Mediachase.IBN.Business.Common.GetListDeclinedRequests(0);
			dgDeclinedRequests.DataBind();

			int RowCount = dgDeclinedRequests.Items.Count;
			if (RowCount == 0)
			{
				Sep12.Visible = false;
				Pan12.Visible = false;
			}
			else
			{
				Sep12.Title = String.Format("{0} ({1})", LocRM.GetString("DR"), RowCount);
			}
		}
		#endregion

    #region Page_PreRender
    private void Page_PreRender(object sender, EventArgs e)
		{
			BindDG();
		}
		#endregion
	}
}
