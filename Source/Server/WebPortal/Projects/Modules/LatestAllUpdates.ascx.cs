using System.Web.UI.WebControls;
using Mediachase.UI.Web.Modules;

namespace Mediachase.UI.Web.Projects.Modules
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
	///		Summary description for LatestUpdates.
	/// </summary>
	public partial  class LatestAllUpdates : System.Web.UI.UserControl
	{
		#region ProjectID
		private int ProjectID
		{
			get
			{
				try
				{
					return int.Parse(Request["ProjectID"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		private UserLightPropertyCollection pc =  Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strLatestUpdates", typeof(LatestAllUpdates).Assembly);
		protected System.Web.UI.WebControls.DataGrid dgPrj;
		protected Panel ProjectPanel;
		int PageSize = 5;
		int NumOfDays = 10;
		bool collapse = true;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (pc["ProjectUSetup_RecsPerPage"] != null)
				PageSize = int.Parse(pc["ProjectUSetup_RecsPerPage"]);

			if (pc["ProjectUSetup_UpdatesLastDays"] != null)
				NumOfDays = int.Parse(pc["ProjectUSetup_UpdatesLastDays"]);

			if (pc["ProjectUSetup_CollapseBlocks"]!=null)
				collapse = bool.Parse(pc["ProjectUSetup_CollapseBlocks"]);

			if (!IsPostBack)
			{
				BindSavedValues();

				ViewState["dgIncidents"] = 0;
				if(!Configuration.HelpDeskEnabled)
				{
					sep2.Visible = false;
					Pan2.Visible = false;
				}
				else
					BinddgIncidents();

				ViewState["dgDocuments"] = 0;
				BinddgDocuments();

				ViewState["dgEvents"] = 0;
				BinddgEvents();

				ViewState["dgTasks"] = 0;
				BinddgTasks();

			}
			else
				BindDataGrids();
		}


		#region BinddgIncidents()
		private void BinddgIncidents()
		{
			DataTable dt = Incident.GetListIncidentsUpdatedForUserDataTable(NumOfDays, ProjectID);
			ViewState["IncidentsRowCount"] = dt.Rows.Count;
			
			if (dt.Rows.Count == 0 || 
				(pc["ProjectUSetup_ShowIncidents"]!=null && pc["ProjectUSetup_ShowIncidents"]=="False"))
			{
				sep2.Visible = false;
				Pan2.Visible = false;
			}
			else if (PageSize>=dt.Rows.Count) 
				dgIncidents.PagerStyle.Visible = false;

			dgIncidents.PageSize = PageSize;
			dgIncidents.Columns[0].HeaderText = LocRM.GetString("Title");
			dgIncidents.Columns[1].HeaderText = LocRM.GetString("UpdatedBy");
			dgIncidents.Columns[2].HeaderText = LocRM.GetString("Modify");
			dgIncidents.CurrentPageIndex = (int)ViewState["dgIncidents"];
			dgIncidents.DataSource = dt.DefaultView;
			dgIncidents.DataBind();
		}
		#endregion

		#region BinddgDocuments
		private void BinddgDocuments()
		{
			DataTable dt = Document.GetListDocumentsUpdatedForUserDataTable(NumOfDays, ProjectID);
			///		DocumentId, Title, CreatorId, ManagerId, LastSavedDate, ProjectId, ProjectName, StateId
			ViewState["DocumentsRowCount"] = dt.Rows.Count;
			
			if (dt.Rows.Count == 0 || 
				(pc["ProjectUSetup_ShowDocuments"]!=null && pc["ProjectUSetup_ShowDocuments"]=="False"))
			{
				sep3.Visible = false;
				Pan3.Visible = false;
			}
			else if (PageSize>=dt.Rows.Count) 
				dgDocuments.PagerStyle.Visible = false;

			dgDocuments.PageSize = PageSize;
			dgDocuments.Columns[0].HeaderText = LocRM.GetString("Title");
			dgDocuments.Columns[1].HeaderText = LocRM.GetString("Modify");
			dgDocuments.CurrentPageIndex = (int)ViewState["dgDocuments"];
			dgDocuments.DataSource = dt.DefaultView;
			dgDocuments.DataBind();
		}
		#endregion

		#region BinddgEvents()
		private void BinddgEvents()
		{
			DataTable dt = CalendarEntry.GetListEventsUpdatedForUserDataTable(NumOfDays, ProjectID);
			ViewState["EventsRowCount"] = dt.Rows.Count;
			
			if (dt.Rows.Count == 0 || 
				(pc["ProjectUSetup_ShowEvents"]!=null && pc["ProjectUSetup_ShowEvents"]=="False"))
			{
				sep4.Visible = false;
				Pan4.Visible = false;
			}
			else if (PageSize>=dt.Rows.Count) 
				dgEvents.PagerStyle.Visible = false;

			dgEvents.PageSize = PageSize;
			dgEvents.Columns[0].HeaderText = LocRM.GetString("Title");
			dgEvents.Columns[1].HeaderText = LocRM.GetString("UpdatedBy");
			dgEvents.Columns[2].HeaderText = LocRM.GetString("Modify");
			dgEvents.CurrentPageIndex = (int)ViewState["dgEvents"];
			dgEvents.DataSource = dt.DefaultView;
			dgEvents.DataBind();
		}
		#endregion

		#region BinddgTasks()
		private void BinddgTasks()
		{
			DataTable dt = ToDo.GetListToDoAndTasksUpdatedForUserDataTable(NumOfDays, ProjectID);
			ViewState["TasksRowCount"] = dt.Rows.Count;
			
			if (dt.Rows.Count == 0 || 
				(pc["ProjectUSetup_ShowTasks"]!=null && pc["ProjectUSetup_ShowTasks"]=="False"))
			{
				sep5.Visible = false;
				Pan5.Visible = false;
			}
			else if (PageSize>=dt.Rows.Count) 
				dgTasks.PagerStyle.Visible = false;

			dgTasks.PageSize = PageSize;
			dgTasks.Columns[0].HeaderText = LocRM.GetString("Title");
			dgTasks.Columns[1].HeaderText = LocRM.GetString("UpdatedBy");
			dgTasks.Columns[2].HeaderText = LocRM.GetString("Modify");
			dgTasks.CurrentPageIndex = (int)ViewState["dgTasks"];
			dgTasks.DataSource = dt.DefaultView;
			dgTasks.DataBind();
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindDataGrids();
			BindToolbar();
		} 
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			tbHeader.Title = LocRM.GetString("tbTitle");

			if (sep2.Visible)
			{
				sep2.Title = String.Format(CultureInfo.InvariantCulture,
					"<img width='16px' height='16px' src='{0}' align='absmiddle' border='0'>&nbsp;{1} <span style='color:red'>({2})</span>",
					ResolveClientUrl("~/layouts/images/icons/Incident.gif"),
					LocRM.GetString("Issues"),
					ViewState["IncidentsRowCount"].ToString());
				sep2.pan = Pan2;
			}

			sep3.Title = String.Format(CultureInfo.InvariantCulture,
				"<img width='16px' height='16px' src='{0}' align='absmiddle' border='0'>&nbsp;{1} <span style='color:red'>({2})</span>",
				ResolveClientUrl("~/layouts/images/filetypes/common.gif"),
				LocRM.GetString("Documents"),
				ViewState["DocumentsRowCount"].ToString());
			sep3.pan = Pan3;

			sep4.Title = String.Format(CultureInfo.InvariantCulture,
				"<img width='16px' height='16px' src='{0}' align='absmiddle' border='0'>&nbsp;{1} <span style='color:red'>({2})</span>",
				ResolveClientUrl("~/layouts/images/icons/event.gif"),
				LocRM.GetString("Events"),
				ViewState["EventsRowCount"].ToString());
			sep4.pan = Pan4;

			sep5.Title = String.Format(CultureInfo.InvariantCulture,
				"<img alt='' width='16' height='16' src='{0}' align='absmiddle' border='0'/>&nbsp;{1} <span style='color:red'>({2})</span>",
				ResolveClientUrl("~/layouts/images/icons/task.gif"),
				LocRM.GetString("Tasks"),
				ViewState["TasksRowCount"].ToString());
			sep5.pan = Pan5;
		} 
		#endregion


		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();

			sep2.SeparatorChanged +=new Mediachase.UI.Web.Modules.SeparatorChangeEventHandler(Separator_OnChange);
			sep3.SeparatorChanged +=new Mediachase.UI.Web.Modules.SeparatorChangeEventHandler(Separator_OnChange);
			sep4.SeparatorChanged +=new Mediachase.UI.Web.Modules.SeparatorChangeEventHandler(Separator_OnChange);
			sep5.SeparatorChanged +=new Mediachase.UI.Web.Modules.SeparatorChangeEventHandler(Separator_OnChange);

			base.OnInit(e);
		}
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgIncidents.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgIncidents_PageChange);
			this.dgDocuments.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgDocuments_PageChange);
			this.dgEvents.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgEvents_PageChange);
			this.dgTasks.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgTasks_PageChange);

		}
		#endregion

		#region Separator_OnChange
		private void Separator_OnChange(Separator1 sender)
		{
			if (sender.IsMinimized == false && collapse)
			{
				if (sender != (object)sep2) sep2.IsMinimized = true;
				if (sender != (object)sep3) sep3.IsMinimized = true;
				if (sender != (object)sep4) sep4.IsMinimized = true;
				if (sender != (object)sep5) sep5.IsMinimized = true;
			}
			SaveSeparatorState();
		} 
		#endregion

		#region SaveSeparatorState
		private void SaveSeparatorState()
		{
			pc["ProjectLatestUpdates_sep2"] = sep2.IsMinimized.ToString();
			pc["ProjectLatestUpdates_sep3"] = sep3.IsMinimized.ToString();
			pc["ProjectLatestUpdates_sep4"] = sep4.IsMinimized.ToString();
			pc["ProjectLatestUpdates_sep5"] = sep5.IsMinimized.ToString();
		} 
		#endregion

		#region BindDataGrids
		private void BindDataGrids()
		{
			if (!sep2.IsMinimized) BinddgIncidents();
			if (!sep3.IsMinimized) BinddgDocuments();
			if (!sep4.IsMinimized) BinddgEvents();
			if (!sep5.IsMinimized) BinddgTasks();
		} 
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			if (pc["ProjectLatestUpdates_sep2"] != null) sep2.IsMinimized = bool.Parse(pc["ProjectLatestUpdates_sep2"]);
			if (pc["ProjectLatestUpdates_sep3"] != null) sep3.IsMinimized = bool.Parse(pc["ProjectLatestUpdates_sep3"]);
			if (pc["ProjectLatestUpdates_sep4"] != null) sep4.IsMinimized = bool.Parse(pc["ProjectLatestUpdates_sep4"]);
			if (pc["ProjectLatestUpdates_sep5"] != null) sep5.IsMinimized = bool.Parse(pc["ProjectLatestUpdates_sep5"]);
		} 
		#endregion

		#region PageChange
		private void dgPrj_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			ViewState["DGProjects"] = e.NewPageIndex;
			//BindDGProjects();
		}

		private void dgIncidents_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			ViewState["dgIncidents"] = e.NewPageIndex;
		}

		private void dgDocuments_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			ViewState["dgDocuments"] = e.NewPageIndex;
		}

		private void dgEvents_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			ViewState["dgEvents"] = e.NewPageIndex;
		}

		private void dgTasks_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			ViewState["dgTasks"] = e.NewPageIndex;
		} 
		#endregion
	}
}
