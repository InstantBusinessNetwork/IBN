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
	///		Summary description for WorkspaceThisWeek.
	/// </summary>
	public partial  class ResourceAssignments : System.Web.UI.UserControl
	{

		#region HTML Vars

		

		#endregion

		private UserLightPropertyCollection pc =  Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strResAsts", typeof(ResourceAssignments).Assembly);
		int PageSize = 5;
		bool collapse = true;

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

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (pc["ProjectUSetup_RecsPerPage"] != null)
				PageSize = int.Parse(pc["ProjectUSetup_RecsPerPage"]);

			if (pc["ProjectUSetup_CollapseBlocks"]!=null)
				collapse = bool.Parse(pc["ProjectUSetup_CollapseBlocks"]);

			if (!IsPostBack)
			{
				BindSavedValues();

				if (!(Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
					Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) ||
					Security.IsUserInGroup(InternalSecureGroups.ProjectManager) ||
					Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager))
					)
				{
					sep2.Visible = false;
					Pan2.Visible = false;
				}

				ViewState["DGIncidents_page"] = 0;
				ViewState["DGAcceptIncidents_page"] = 0;
				if(!Configuration.HelpDeskEnabled)
				{
					sep2.Visible = false;
					Pan2.Visible = false;

					sep8.Visible = false;
					Pan8.Visible = false;
				}
				else
				{
					BindDGIncidents();
					BindDGAcceptIncidents();
				}

				ViewState["DGTaskToDo_page"] = 0;
				BindDGTaskToDo();

				ViewState["DGAssignments_page"] = 0;
				BindDGAssignments();

				ViewState["DGNotAssigned_page"] = 0;
				BindDGNotAssigned();

				ViewState["DGPACalEntry_page"] = 0;
				BindDGPACalEntry();
			}
			else
				BindDataGrids();
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{		
			BindDataGrids();
			BindToolbar();
		}

		#region BindDataGrids
		private void BindDataGrids()
		{
			if (!sep2.IsMinimized) BindDGIncidents();
			if (!sep3.IsMinimized) BindDGTaskToDo();
			if (!sep4.IsMinimized) BindDGAssignments();
			if (!sep5.IsMinimized) BindDGNotAssigned();
			if (!sep7.IsMinimized) BindDGPACalEntry();
			if (!sep8.IsMinimized) BindDGAcceptIncidents();
		}
		#endregion

		#region BindDGAcceptIncidents
		private void BindDGAcceptIncidents()
		{
			dgIncPending.Columns[1].HeaderText =  LocRM.GetString("Title");
			dgIncPending.Columns[2].HeaderText =  LocRM.GetString("Manager");
			//dgIncPending.Columns[3].HeaderText =  LocRM.GetString("Activation");

			DataTable dt =  Mediachase.IBN.Business.Incident.GetListPendingIncidentsDataTable(ProjectID);

			ViewState["dgPI"] = dt.Rows.Count;
			if (dt.Rows.Count == 0)
			{
				sep8.Visible = false;
				Pan8.Visible = false;
			}
			else if (PageSize>=dt.Rows.Count) 
				dgIncPending.PagerStyle.Visible = false;

			dgIncPending.PageSize = PageSize;

			DataView dv = new DataView(dt);
			dv.Sort = "PriorityId Desc";
			dgIncPending.CurrentPageIndex = (int)ViewState["DGAcceptIncidents_page"];
			dgIncPending.DataSource = dv;
			dgIncPending.DataBind();
		}
		#endregion

		#region BindDGPACalEntry
		private void BindDGPACalEntry()
		{

			dgPendingAssignments.Columns[1].HeaderText =  LocRM.GetString("Title");
			dgPendingAssignments.Columns[2].HeaderText =  LocRM.GetString("Organizer");
			dgPendingAssignments.Columns[3].HeaderText =  LocRM.GetString("From");
			dgPendingAssignments.Columns[4].HeaderText =  LocRM.GetString("To");

			DataTable dt =  Mediachase.IBN.Business.CalendarEntry.GetListPendingEventsDataTable(ProjectID);

			ViewState["dgPA"] = dt.Rows.Count;
			if (dt.Rows.Count == 0)
			{
				sep7.Visible = false;
				Pan7.Visible = false;
			}
			else if (PageSize>=dt.Rows.Count) 
				dgPendingAssignments.PagerStyle.Visible = false;

			dgPendingAssignments.PageSize = PageSize;

			DataView dv = new DataView(dt);
			dv.Sort = "PriorityId Desc, StartDate Desc";
			dgPendingAssignments.CurrentPageIndex = (int)ViewState["DGPACalEntry_page"];
			dgPendingAssignments.DataSource = dv;
			dgPendingAssignments.DataBind();

		}
		#endregion

		#region BindDGAssignments
		private void BindDGAssignments()
		{
			DataTable dt = ToDo.GetListPendingToDoAndTasks(ProjectID);
			ViewState["dgAssignments"] = dt.Rows.Count;
			
			if (dt.Rows.Count == 0)
			{
				sep4.Visible = false;
				Pan4.Visible = false;
			}
			else if (PageSize>=dt.Rows.Count) 
				dgAssignments.PagerStyle.Visible = false;

			dgAssignments.PageSize = PageSize;

			dgAssignments.Columns[2].HeaderText = LocRM.GetString("Title");
			dgAssignments.Columns[3].HeaderText = LocRM.GetString("Manager");
			dgAssignments.Columns[4].HeaderText = LocRM.GetString("FinishDate");

			DataView dv = new DataView(dt);
			dv.Sort = "PriorityId Desc, FinishDate";
			dgAssignments.CurrentPageIndex = (int)ViewState["DGAssignments_page"];
			dgAssignments.DataSource = dv;
			dgAssignments.DataBind();
		}
		#endregion

		#region BindDGTaskToDo
		private void BindDGTaskToDo()
		{
			DataTable dt = ToDo.GetListNotApprovedToDoAndTasks(ProjectID);
			ViewState["dgTaskToDo"] = dt.Rows.Count;
			if (dt.Rows.Count == 0)
			{
				sep3.Visible = false; 
				Pan3.Visible = false;
			}
			else if (PageSize>=dt.Rows.Count) 
				dgTaskToDo.PagerStyle.Visible = false;

			dgTaskToDo.PageSize = PageSize;

			dgTaskToDo.Columns[2].HeaderText = LocRM.GetString("Title");
			dgTaskToDo.Columns[3].HeaderText = LocRM.GetString("FinishDate");

			DataView dv = new DataView(dt);
			dv.Sort = "PriorityId Desc, FinishDate";
			dgTaskToDo.CurrentPageIndex = (int)ViewState["DGTaskToDo_page"];
			dgTaskToDo.DataSource = dv;
			dgTaskToDo.DataBind();
		}
		#endregion

		#region BindDGNotAssigned
		private void BindDGNotAssigned()
		{
			DataTable dt = ToDo.GetListToDoAndTasksWithoutResources(ProjectID);
			ViewState["dgNotAssigned"] = dt.Rows.Count;
			if (dt.Rows.Count == 0)
			{
				sep5.Visible = false;
				Pan5.Visible = false;
			}
			else if (PageSize>=dt.Rows.Count) 
				dgNotAssigned.PagerStyle.Visible = false;

			dgNotAssigned.PageSize = PageSize;

			dgNotAssigned.Columns[2].HeaderText = LocRM.GetString("Title");
			dgNotAssigned.Columns[3].HeaderText = LocRM.GetString("FinishDate");

			DataView dv = new DataView(dt);
			dv.Sort = "PriorityId Desc, FinishDate";
			dgNotAssigned.CurrentPageIndex = (int)ViewState["DGNotAssigned_page"];
			dgNotAssigned.DataSource = dv;
			dgNotAssigned.DataBind();
		}
		#endregion
		
		#region BindDGIncidents
		private void BindDGIncidents()
		{
			if (!(Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager))
				)
				return;

			DataTable dt = Incident.GetListNotAssignedIncidentsDataTable(ProjectID);
			ViewState["dgIncident"] = dt.Rows.Count;
			
			if (dt.Rows.Count == 0)
			{
				sep2.Visible = false;
				Pan2.Visible = false;
			}
			else if (PageSize>=dt.Rows.Count) 
				dgIncident.PagerStyle.Visible = false;

			dgIncident.PageSize = PageSize;

			dgIncident.Columns[1].HeaderText = LocRM.GetString("Title");
			dgIncident.Columns[2].HeaderText = LocRM.GetString("CreatedBy");
			dgIncident.Columns[3].HeaderText = LocRM.GetString("Created");
			dgIncident.Columns[4].HeaderText = LocRM.GetString("Status");

			DataView dv = new DataView(dt);
			dv.Sort = "PriorityId Desc, CreationDate Desc";
			dgIncident.CurrentPageIndex = (int)ViewState["DGIncidents_page"];
			dgIncident.DataSource = dv;
			dgIncident.DataBind();
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			tbHeader.Title = LocRM.GetString("tbTitle");

			//sep1.AddLinkItem(LocRM.GetString("AssignInc"),"#");
			if (ViewState["dgIncident"]!=null && sep2.Visible)
				sep2.Title = String.Format(CultureInfo.InvariantCulture,
					"<img width='16px' height='16px' src='{0}' align='absmiddle' border='0'>&nbsp;{1} <span style='color:red'>({2})</span>",
					ResolveClientUrl("~/layouts/images/newincidents.gif"),
					LocRM.GetString("NI"),
					ViewState["dgIncident"].ToString());
			sep2.pan = Pan2;

			if (ViewState["dgTaskToDo"]!=null)
				sep3.Title = String.Format(CultureInfo.InvariantCulture,
					"<img width='16px' height='16px' src='{0}' align='absmiddle' border='0'>&nbsp;{1} <span style='color:red'>({2})</span>",
					ResolveClientUrl("~/layouts/images/approvetasks.gif"),
					LocRM.GetString("AC"),
					ViewState["dgTaskToDo"].ToString());
			sep3.pan = Pan3;

			if (ViewState["dgAssignments"]!=null)
				sep4.Title = String.Format(CultureInfo.InvariantCulture,
					"<img width='16px' height='16px' src='{0}' align='absmiddle' border='0'>&nbsp;{1} <span style='color:red'>({2})</span>",
					ResolveClientUrl("~/layouts/images/pendingass.gif"),
					LocRM.GetString("PA"),
					ViewState["dgAssignments"].ToString());
			sep4.pan = Pan4;

			if (ViewState["dgNotAssigned"]!=null)
				sep5.Title = String.Format(CultureInfo.InvariantCulture,
					"<img width='16px' height='16px' src='{0}' align='absmiddle' border='0'>&nbsp;{1} <span style='color:red'>({2})</span>",
					ResolveClientUrl("~/layouts/images/notass.gif"),
					LocRM.GetString("NA"),
					ViewState["dgNotAssigned"].ToString());
			sep5.pan = Pan5;

			if (ViewState["dgPA"]!=null)
				sep7.Title = String.Format(CultureInfo.InvariantCulture,
					"<img width='16px' height='16px' src='{0}' align='absmiddle' border='0'>&nbsp;{1} <span style='color:red'>({2})</span>",
					ResolveClientUrl("~/layouts/images/icons/Event.gif"),
					LocRM.GetString("CA"),
					ViewState["dgPA"].ToString());
			sep7.pan = Pan7;

			if (ViewState["dgPI"]!=null && sep8.Visible)
				sep8.Title = String.Format(CultureInfo.InvariantCulture,
					"<img width='16px' height='16px' src='{0}' align='absmiddle' border='0'>&nbsp;{1} <span style='color:red'>({2})</span>",
					ResolveClientUrl("~/layouts/images/icons/incident.gif"),
					LocRM.GetString("IA"),
					ViewState["dgPI"].ToString());
			sep8.pan = Pan8;
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			if (pc["ProjectWSRAwra_sep2"]!=null) sep2.IsMinimized = bool.Parse(pc["ProjectWSRAwra_sep2"]);
			if (pc["ProjectWSRAwra_sep3"]!=null) sep3.IsMinimized = bool.Parse(pc["ProjectWSRAwra_sep3"]);
			if (pc["ProjectWSRAwra_sep4"]!=null) sep4.IsMinimized = bool.Parse(pc["ProjectWSRAwra_sep4"]);
			if (pc["ProjectWSRAwra_sep5"]!=null) sep5.IsMinimized = bool.Parse(pc["ProjectWSRAwra_sep5"]);
			if (pc["ProjectWSRAwra_sep7"]!=null) sep7.IsMinimized = bool.Parse(pc["ProjectWSRAwra_sep7"]);
			if (pc["ProjectWSRAwra_sep8"]!=null) sep8.IsMinimized = bool.Parse(pc["ProjectWSRAwra_sep8"]);
		}
		#endregion

		#region SaveSeparatorState
		private void SaveSeparatorState()
		{
			pc["ProjectWSRAwra_sep2"] = sep2.IsMinimized.ToString();
			pc["ProjectWSRAwra_sep3"] = sep3.IsMinimized.ToString();
			pc["ProjectWSRAwra_sep4"] = sep4.IsMinimized.ToString();
			pc["ProjectWSRAwra_sep5"] = sep5.IsMinimized.ToString();
			pc["ProjectWSRAwra_sep7"] = sep7.IsMinimized.ToString();
			pc["ProjectWSRAwra_sep8"] = sep8.IsMinimized.ToString();
		}
		#endregion

		#region DataGrid Strings
		protected string GetTaskToDoStatus(int PID,string Name)
		{
			string color = "PriorityNormal.gif";
			if (PID < 100) color = "PriorityLow.gif";
			if (PID > 500 && PID < 800) color = "PriorityHigh.gif";
			if (PID >= 800 && PID < 1000) color = "PriorityVeryHigh.gif";
			if (PID >= 1000) color = "PriorityUrgent.gif";
			Name = LocRM.GetString("Priority") + Name;
			return String.Format(CultureInfo.InvariantCulture,
				"<img width='16' height='16' src='{0}' alt='{1}' title='{1}'/>",
				ResolveClientUrl(String.Concat("~/layouts/images/icons/", color)),
				Name);
		}

		protected string GetSender(string email)
		{
			int UserId = User.GetUserByEmail(email);
			if (UserId==-1)
			{
				return String.Format("<a href='mailto:{0}'>{0}</a>",email);
			}
			else
			{
				return CommonHelper.GetUserStatus(UserId);
			}
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

			sep2.SeparatorChanged +=new Mediachase.UI.Web.Modules.SeparatorChangeEventHandler(Max_Sep);
			sep3.SeparatorChanged +=new Mediachase.UI.Web.Modules.SeparatorChangeEventHandler(Max_Sep);
			sep4.SeparatorChanged +=new Mediachase.UI.Web.Modules.SeparatorChangeEventHandler(Max_Sep);
			sep5.SeparatorChanged +=new Mediachase.UI.Web.Modules.SeparatorChangeEventHandler(Max_Sep);
			sep7.SeparatorChanged +=new Mediachase.UI.Web.Modules.SeparatorChangeEventHandler(Max_Sep);
			sep8.SeparatorChanged +=new Mediachase.UI.Web.Modules.SeparatorChangeEventHandler(Max_Sep);
		}


		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgIncident.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgIncident_PageIndexChange);
			this.dgTaskToDo.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgTaskToDo_PageChange);
			this.dgAssignments.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgAssignments_PageChange);
			this.dgNotAssigned.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgNotAssigned_PageChange);
			this.dgPendingAssignments.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgPendingAssignments_PageChange);
			this.dgIncPending.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgIncPending_PageChange);
		}
		#endregion

		private void Max_Sep(Mediachase.UI.Web.Modules.Separator1 sender)
		{
			if (sender.IsMinimized == false && collapse)
			{
				if (sender!=(object)sep2) {sep2.IsMinimized = true;}
				if (sender!=(object)sep3) {sep3.IsMinimized = true;}
				if (sender!=(object)sep4) {sep4.IsMinimized = true;}
				if (sender!=(object)sep5) {sep5.IsMinimized = true;}
				if (sender!=(object)sep7) {sep7.IsMinimized = true;}
				if (sender!=(object)sep8) {sep8.IsMinimized = true;}
			}
			SaveSeparatorState();
		}

		#region DataGrids Events
		private void dgIncident_PageIndexChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			ViewState["DGIncidents_page"] = e.NewPageIndex;
		}

		private void dgPendingUsers_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			ViewState["DGPending_page"] = e.NewPageIndex;
		}

		private void dgTaskToDo_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			ViewState["DGTaskToDo_page"] = e.NewPageIndex;
		}

		private void dgAssignments_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			ViewState["DGAssignments_page"] = e.NewPageIndex;
		}

		private void dgNotAssigned_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			ViewState["DGNotAssigned_page"] = e.NewPageIndex;
		}

		private void dgMailIncidents_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			ViewState["DGMailIncidents_page"] = e.NewPageIndex;
		}

		private void dgPendingAssignments_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			ViewState["DGPACalEntry_page"] = e.NewPageIndex;
		}

		private void dgIncPending_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			ViewState["DGAcceptIncidents_page"] = e.NewPageIndex;
		}
		#endregion
		
	}
}
