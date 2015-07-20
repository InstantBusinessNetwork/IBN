namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using System.Globalization;
	using System.Resources;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for ProjectTemplates.
	/// </summary>
	public partial class ProjectTemplates : System.Web.UI.UserControl
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ProjectTemplates).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				if (pc["PrjTempSort"]==null)
					pc["PrjTempSort"] = "TemplateName";
				if (pc["PrjTempPageSize"]==null)
					pc["PrjTempPageSize"] = "10";
				if (pc["PrjTempPage"]==null)
					pc["PrjTempPage"] = "1";
			}
		}

    private void Page_PreRender(object sender, EventArgs e)
		{
			BindDataGrid();
			BindToolbar();
		}

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("PrjTemps");
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid()
		{
			dgProjectTemps.Columns[1].HeaderText = LocRM.GetString("Title");
			dgProjectTemps.Columns[2].HeaderText = LocRM.GetString("tCreationDate");
			dgProjectTemps.Columns[3].HeaderText = LocRM.GetString("tCreator");
			dgProjectTemps.Columns[4].HeaderText = LocRM.GetString("tLastSavedDate");
			dgProjectTemps.Columns[5].HeaderText = LocRM.GetString("tLastEditor");
			
			foreach(DataGridColumn dgc in dgProjectTemps.Columns)
			{
				if(dgc.SortExpression == pc["PrjTempSort"].ToString())
					dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='../layouts/images/upbtnF.jpg'/>";
				else if(dgc.SortExpression + " DESC" == pc["PrjTempSort"].ToString())
					dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='../layouts/images/downbtnF.jpg'/>";
			}
			//dgProjectTemps.Columns[6].Visible = bCanUpdate;

			DataTable dt = ProjectTemplate.GetListProjectTemplateDataTable();
			DataView dv = dt.DefaultView;
			
			dv.Sort = pc["PrjTempSort"].ToString();

			dgProjectTemps.DataSource = dt.DefaultView;
			if (pc["PrjTempPageSize"]!=null)
				dgProjectTemps.PageSize = int.Parse(pc["PrjTempPageSize"]);
			if (pc["PrjTempPage"]!=null)
			{
				int pageindex = int.Parse(pc["PrjTempPage"]);
				int ppi = dv.Count / dgProjectTemps.PageSize;
				if  (dv.Count % dgProjectTemps.PageSize == 0)
					ppi = ppi - 1;

				if (pageindex <= ppi)
					dgProjectTemps.CurrentPageIndex = pageindex;
				else dgProjectTemps.CurrentPageIndex = 0;
			}
			dgProjectTemps.DataBind();
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
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgProjectTemps.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_PageChange);
			this.dgProjectTemps.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.dg_Sort);
			this.dgProjectTemps.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dg_PageSizeChange);			
			this.lbDeleteProjectTemp.Click += new EventHandler(lbDeleteProjectTemp_Click);
		}
		#endregion

		private void lbDeleteProjectTemp_Click(object sender, EventArgs e)
		{
			int DelPrjTempId = int.Parse(hdnProjectTempId.Value);
			ProjectTemplate.DeleteProjectTemplate(DelPrjTempId);
			Response.Redirect("../Projects/ProjectTemplates.aspx");
		}

		#region DataGrid Events
		private void dg_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			pc["PrjTempPage"] = e.NewPageIndex.ToString();
		}

		private void dg_PageSizeChange(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["PrjTempPageSize"] = e.NewPageSize.ToString();
		}

		private void dg_Sort(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if(pc["PrjTempSort"] !=null && pc["PrjTempSort"].ToString() == (string)e.SortExpression)
				pc["PrjTempSort"] = (string)e.SortExpression + " DESC";
			else
				pc["PrjTempSort"] = (string)e.SortExpression;
		}
		#endregion

	}
}
