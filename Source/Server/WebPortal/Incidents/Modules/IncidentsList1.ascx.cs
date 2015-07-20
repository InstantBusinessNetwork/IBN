namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Data;
	using System.Collections;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Globalization;

	/// <summary>
	///		Summary description for IncidentsList1.
	/// </summary>
	public partial  class IncidentsList1 : System.Web.UI.UserControl
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentsList", typeof(IncidentsList1).Assembly);
		protected UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		
		private string strPref = "Inc";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(pc["IncidentsList1_Sort"]==null)
				pc["IncidentsList1_Sort"] = "PriorityName";
			ApplyLocalization();
			if(!Page.IsPostBack)
				BindDataGrid();
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid()
		{
			dgIncidents.Columns[2].HeaderText = LocRM.GetString("Title");
			dgIncidents.Columns[3].HeaderText = LocRM.GetString("CreatedBy");
			dgIncidents.Columns[4].HeaderText = LocRM.GetString("Status");
			dgIncidents.Columns[5].HeaderText = LocRM.GetString("Priority");
			dgIncidents.Columns[6].HeaderText = LocRM.GetString("ModifiedDate");
			foreach(DataGridColumn dgc in dgIncidents.Columns)
			{
				if(dgc.SortExpression == pc["IncidentsList1_Sort"].ToString())
					dgc.HeaderText += String.Format(CultureInfo.InvariantCulture,
						"&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>",
						ResolveClientUrl("~/layouts/images/upbtnF.jpg"));
				else if(dgc.SortExpression + " DESC" == pc["IncidentsList1_Sort"].ToString())
					dgc.HeaderText += String.Format(CultureInfo.InvariantCulture,
						"&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>",
						ResolveClientUrl("~/layouts/images/downbtnF.jpg"));
			}

			DataTable dt = Incident.GetListNotAssignedIncidentsDataTable(0);
			DataView dv = dt.DefaultView;
			dv.Sort = pc["IncidentsList1_Sort"].ToString();
			dgIncidents.DataSource = dv;

			if (pc[strPref+"IncidentList_PageSize"]!=null)
				dgIncidents.PageSize = int.Parse(pc[strPref+"IncidentList_PageSize"]);

			if (pc[strPref+"IncidentList_Page"]!=null)
			{
				int iPageIndex = int.Parse(pc[strPref+"IncidentList_Page"]);
				int ppi = dt.Rows.Count / dgIncidents.PageSize;
				if  (dt.Rows.Count % dgIncidents.PageSize == 0)
					ppi = ppi - 1;
				if (iPageIndex <= ppi)
					dgIncidents.CurrentPageIndex = iPageIndex;
				else dgIncidents.CurrentPageIndex = 0;
			}
			dgIncidents.DataBind();
			foreach (DataGridItem dgi in dgIncidents.Items)
			{
				ImageButton ib=(ImageButton)dgi.FindControl("ibDelete");
				if (ib!=null)
					ib.Attributes.Add("onclick","return confirm('"+LocRM.GetString("tWarning")+"')");
			}
		}
		#endregion

		#region Protected Strings
		protected string GetCreator(int CreatorId)
		{
			return CommonHelper.GetUserStatus(CreatorId);
		}

		protected string GetTaskToDoStatus(int PID,string Name)
		{
			string color = "PriorityNormal.gif";
			if (PID < 100) color = "PriorityLow.gif";
			if (PID > 500 && PID < 800) color = "PriorityHigh.gif";
			if (PID >= 800 && PID < 1000) color = "PriorityVeryHigh.gif";
			if (PID >= 1000) color = "PriorityUrgent.gif";
			Name = LocRM.GetString("Priority")+ ":"+ Name;
			return String.Format(CultureInfo.InvariantCulture,
				"<img width='16' height='16' src='{0}' alt='{1}' title='{1}'/>",
				ResolveClientUrl("~/layouts/images/icons/" + color),
				Name); 
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
			this.dgIncidents.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgIncidents_PageIndexChanged);
			this.dgIncidents.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dgIncidents_PageSizeChange);
			this.dgIncidents.DeleteCommand += new DataGridCommandEventHandler(dgIncidents_DeleteCommand);
			this.dgIncidents.SortCommand += new DataGridSortCommandEventHandler(dgIncidents_SortCommand);
		}
		#endregion

		private void dgIncidents_PageSizeChange(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc[strPref+"IncidentList_PageSize"] = e.NewPageSize.ToString();
			BindDataGrid();
		}

		private void dgIncidents_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			pc[strPref+"IncidentList_Page"] = e.NewPageIndex.ToString();
			BindDataGrid();
		}

		private void dgIncidents_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int sid = int.Parse(e.Item.Cells[0].Text);
			Incident.Delete(sid);
			Response.Redirect("~/Apps/HelpDeskManagement/Pages/IncidentListNew.aspx");
		}

		private void dgIncidents_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if((pc["IncidentsList1_Sort"] != null) && (pc["IncidentsList1_Sort"].ToString() == (string)e.SortExpression))
				pc["IncidentsList1_Sort"] = (string)e.SortExpression + " DESC";
			else
				pc["IncidentsList1_Sort"] = (string)e.SortExpression;
			BindDataGrid();
		}
	}
}
