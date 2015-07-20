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
	public partial  class WorkspacePTTI : System.Web.UI.UserControl
	{
		#region HTML Vars
		protected System.Web.UI.WebControls.Panel Pan4;
		#endregion
		private UserLightPropertyCollection pc =  Security.CurrentUser.Properties;

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

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPTTI", typeof(WorkspacePTTI).Assembly);
		int PageSize = 5;
		bool collapse = true;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (pc["ProjectUSetup_RecsPerPage"] != null)
				PageSize = int.Parse(pc["ProjectUSetup_RecsPerPage"]);

			if (pc["ProjectUSetup_CollapseBlocks"]!=null)
				collapse = bool.Parse(pc["ProjectUSetup_CollapseBlocks"]);

			if (!IsPostBack)
			{
				BindSavedValues();


				ViewState["DGToDo"] = 0;
				BindDGToDo();

			}
			else
				BindDataGrids();
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindDataGrids();
			BindToolbar();
		}
		#endregion


		#region BindDGToDo
		private void BindDGToDo()
		{
			DataTable dt = ToDo.GetListActiveToDoAndTasks(ProjectID);
			ViewState["ToDoRowCount"] = dt.Rows.Count;

			if (dt.Rows.Count == 0 || pc["ProjectUSetup_ShowTasksToDosInvolved"] == "False")
			{
				sep2.Visible = false;
				Pan2.Visible = false;
			}
			else if (PageSize>=dt.Rows.Count) 
				dgToDo.PagerStyle.Visible = false;

			dgToDo.PageSize = PageSize;

			dgToDo.Columns[1].HeaderText = LocRM.GetString("Title");
			dgToDo.Columns[2].HeaderText = LocRM.GetString("Manager");
			dgToDo.Columns[3].HeaderText = LocRM.GetString("FinishDate");

			DataView dv = new DataView(dt);
			dv.Sort = "StateId DESC,PriorityId DESC,FinishDate";
			dgToDo.CurrentPageIndex = (int)ViewState["DGToDo"];
			dgToDo.DataSource = dv;
			dgToDo.DataBind();
		}
		#endregion

		#region GetProjectStatus
		protected string GetProjectStatus(DateTime TFD)
		{
			string text = LocRM.GetString("Status") + ":" + LocRM.GetString("OnTarget");
			string color = "flaggreen.gif";
			if (TFD<UserDateTime.UserNow) 
			{
				color = "flagred.gif";
				text = LocRM.GetString("Status") + ":" + LocRM.GetString("AtRisk");
			}

			return String.Format(CultureInfo.InvariantCulture,
				"<img width='16px' height='16px' src='{0}' title='{1}'>",
				ResolveClientUrl("~/layouts/images/" + color),
				text);
		}
		#endregion

		#region GetTaskToDoStatus
		protected string GetTaskToDoStatus(int PID,string Name)
		{
			Name = LocRM.GetString("Priority") + Name;
			string color = "PriorityNormal.gif";
			if (PID < 100) color = "PriorityLow.gif";
			if (PID > 500 && PID < 800) color = "PriorityHigh.gif";
			if (PID >= 800 && PID < 1000) color = "PriorityVeryHigh.gif";
			if (PID >= 1000) color = "PriorityUrgent.gif";
			return String.Format(CultureInfo.InvariantCulture,
				"<img width='16' height='16' src='{0}' alt='{1}' title='{1}'/>",
				ResolveClientUrl("~/layouts/images/icons/" + color),
				Name);
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			tbHeader.Title = LocRM.GetString("tbTitle");

			sep2.Title = LocRM.GetString("TT")+ " <span style='color:red'>("+ViewState["ToDoRowCount"].ToString()+")</span>";
			sep2.pan = Pan2;
//			sep2.AddLinkItem("<img border=0 width='16' height='16' src='../layouts/images/icon-search.gif' align='absmiddle'>&nbsp;" + LocRM.GetString("ViewAll"),"#");
			
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

			this.dgToDo.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgToDo_PageChange);

			
			sep2.SeparatorChanged +=new Mediachase.UI.Web.Modules.SeparatorChangeEventHandler(Max_Sep);

		}

		private void Max_Sep(Mediachase.UI.Web.Modules.Separator1 sender)
		{
			if (sender.IsMinimized == false && collapse)
			{
				if (sender!=(object)sep2) sep2.IsMinimized = true;
			}
			SaveSeparatorState();
		}
		#endregion

		#region SaveSeparatorState
		private void SaveSeparatorState()
		{
			pc["ProjectWSptti_sep2"] = sep2.IsMinimized.ToString();
		}
		#endregion

		#region BindDataGrids
		private void BindDataGrids()
		{
			if (!sep2.IsMinimized) BindDGToDo();
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			if (pc["ProjectWSptti_sep2"]!=null) sep2.IsMinimized = bool.Parse(pc["ProjectWSptti_sep2"]);
		}
		#endregion

		#region dgPrj_PageChange
		private void dgPrj_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			ViewState["DGProjects"] = e.NewPageIndex;
			//BindDGProjects();
		}
		#endregion

		#region dgToDo_PageChange
		private void dgToDo_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{	
			ViewState["DGToDo"] = e.NewPageIndex;
			//BindDGToDo();
		}
		#endregion

	}
}
