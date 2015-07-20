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
	///		Summary description for ProjectGroups.
	/// </summary>
	public partial class ProjectGroups : System.Web.UI.UserControl
	{

    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ProjectGroups).Assembly);
		private bool bCanUpdate = ProjectGroup.CanUpdate();
		private bool bCanAdd = ProjectGroup.CanAdd();
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				if (pc["PrjGrpSort"]==null)
					pc["PrjGrpSort"] = "Title";
				if (pc["PrjGrpPageSize"]==null)
					pc["PrjGrpPageSize"] = "10";
				if (pc["PrjGrpPage"]==null)
					pc["PrjGrpPage"] = "1";
			}
		}
		
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindDataGrid();
			BindToolbar();
		}

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tPortfolios");
			if(bCanAdd)
				secHeader.AddLink("<img alt='' src='../Layouts/Images/newItem.gif'/> " + LocRM.GetString("tNewPortfolio"),"../Projects/ProjectGroupEdit.aspx");
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid()
		{
			int i = 1;
			dgProjectGroups.Columns[i++].HeaderText = LocRM.GetString("Title");
			dgProjectGroups.Columns[i++].HeaderText = LocRM.GetString("tCreationDate");
			dgProjectGroups.Columns[i++].HeaderText = LocRM.GetString("tCreator");
			foreach(DataGridColumn dgc in dgProjectGroups.Columns)
			{
				if(dgc.SortExpression == pc["PrjGrpSort"].ToString())
					dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='../layouts/images/upbtnF.jpg'/>";
				else if(dgc.SortExpression + " DESC" == pc["PrjGrpSort"].ToString())
					dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='../layouts/images/downbtnF.jpg'/>";
			}

			dgProjectGroups.Columns[dgProjectGroups.Columns.Count-1].Visible = bCanUpdate;

			DataTable dt = ProjectGroup.GetProjectGroupsDT();
			DataView dv = dt.DefaultView;
			
			try
			{
				dv.Sort = pc["PrjGrpSort"].ToString();
			}
			catch
			{
				pc["PrjGrpSort"] = "Title";
				dv.Sort = pc["PrjGrpSort"].ToString();
			}

			dgProjectGroups.DataSource = dt.DefaultView;
			if (pc["PrjGrpPageSize"]!=null)
				dgProjectGroups.PageSize = int.Parse(pc["PrjGrpPageSize"]);
			if (pc["PrjGrpPage"]!=null)
			{
				int pageindex = int.Parse(pc["PrjGrpPage"]);
				int ppi = dv.Count / dgProjectGroups.PageSize;
				if  (dv.Count % dgProjectGroups.PageSize == 0)
					ppi = ppi - 1;

				if (pageindex <= ppi)
					dgProjectGroups.CurrentPageIndex = pageindex;
				else dgProjectGroups.CurrentPageIndex = 0;
			}
			dgProjectGroups.DataBind();
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
			this.dgProjectGroups.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_PageChange);
			this.dgProjectGroups.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.dg_Sort);
			this.dgProjectGroups.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dg_PageSizeChange);			
		}
		#endregion

		#region DataGrid Events
		protected void Delete_Click(object sender, System.EventArgs e)
		{
			int DelPrjGroupId = int.Parse(hdnProjectGroupId.Value);
			ProjectGroup.Delete(DelPrjGroupId);
			Response.Redirect("../Projects/ProjectGroups.aspx");
		}

		private void dg_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			pc["PrjGrpPage"] = e.NewPageIndex.ToString();
		}

		private void dg_PageSizeChange(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["PrjGrpPageSize"] = e.NewPageSize.ToString();
		}

		private void dg_Sort(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if(pc["PrjGrpSort"] !=null && pc["PrjGrpSort"].ToString() == (string)e.SortExpression)
				pc["PrjGrpSort"] = (string)e.SortExpression + " DESC";
			else
				pc["PrjGrpSort"] = (string)e.SortExpression;
		}
		#endregion
	}
}
