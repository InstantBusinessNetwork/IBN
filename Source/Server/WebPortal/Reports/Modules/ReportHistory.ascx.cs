namespace Mediachase.UI.Web.Reports.Modules
{
	using System;
	using System.Xml;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.IBN.Business.Reports;

	/// <summary>
	///		Summary description for ReportHistory.
	/// </summary>
	public partial class ReportHistory : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strXMLReport", typeof(ReportHistory).Assembly);		
		protected string grdMain_SortColumn = "CreationDate DESC";
		private UserLightPropertyCollection pc=Security.CurrentUser.Properties;

		#region TemplateId
		protected int TemplateId
		{
			get
			{
				try{
					return int.Parse(Request["TemplateId"].ToString());
				}
				catch{
					return -1;
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				BindData();
				ApplyLocalization();
				ViewState["grdMain_SortColumn"] = grdMain_SortColumn;
				BindDG();
			}
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolBar();
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			grdMain.Columns[1].HeaderText = LocRM.GetString("tCreateDate");
			grdMain.Columns[2].HeaderText = LocRM.GetString("tCreator");
		}
		#endregion

		#region BindData
		private void BindData()
		{
			using(IDataReader reader = Report.GetReportTemplate(TemplateId))
			{
				if(reader.Read())
				{
					lblTempTitle.Text = reader["Name"].ToString();
					lblCreatorName.Text = CommonHelper.GetUserStatus((int)reader["TemplateCreatorId"]);
					lblModifierName.Text = CommonHelper.GetUserStatus((int)reader["TemplateModifierId"]);
				}
			}
		}
		#endregion

		#region BindToolBar
		private void BindToolBar()
		{
			secHeader.Title = LocRM.GetString("tReportHistory");
			secHeader.AddLink("<img src='../Layouts/Images/report.gif' border='0' width='16px' height='16px' align='absmiddle' > "+LocRM.GetString("tGenerateNew"),"../Reports/TemplateEdit.aspx?Generate=1&TemplateId="+TemplateId.ToString());
			if (!Common.CheckFavorites(TemplateId, ObjectTypes.Report))
				secHeader.AddLink("<img src='../Layouts/Images/Favorites.gif' border='0' width='16px' height='16px' align='absmiddle' > " + GetGlobalResourceObject("IbnFramework.ListInfo", "AddToFavorites"), Page.ClientScript.GetPostBackClientHyperlink(btnAddToFavorites, ""));
			secHeader.AddLink("<img src='../Layouts/Images/cancel.gif' border='0' width='16px' height='16px' align='absmiddle' > "+LocRM.GetString("tCustomReports"),this.ResolveUrl("~/Apps/ReportManagement/Pages/UserReport.aspx"));
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			DataTable dt = Report.GetReportsByTemplateId(TemplateId);
			DataView dv = dt.DefaultView;
			dv.Sort = (string)ViewState["grdMain_SortColumn"];

			if (pc["RepHist_PageSize"]!=null)
				grdMain.PageSize = int.Parse(pc["RepHist_PageSize"]);

			int pageindex = grdMain.CurrentPageIndex;
			int ppi = dt.Rows.Count / grdMain.PageSize;
			if  (dt.Rows.Count % grdMain.PageSize == 0)
				ppi = ppi - 1;

			if (pageindex > ppi)
				grdMain.CurrentPageIndex = 0;
			
			grdMain.DataSource = dv;
			grdMain.DataBind();
			foreach (DataGridItem dgi in grdMain.Items)
			{
				ImageButton ib=(ImageButton)dgi.FindControl("ibView");
				if (ib!=null)
					ib.ToolTip = LocRM.GetString("tViewReport");
				ImageButton ib1=(ImageButton)dgi.FindControl("ibDelete");
				if (ib1!=null)
				{
					ib1.ToolTip = LocRM.GetString("tDelete");
					ib1.Attributes.Add("onclick","return confirm('"+ LocRM.GetString("tWarning2") +"')");
				}
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
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.grdMain.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_PageIndexChanged);
			this.grdMain.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.grdMain_Sort);
			this.grdMain.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dg_PageSizeChanged);			
			this.grdMain.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_view);
			this.grdMain.DeleteCommand += new DataGridCommandEventHandler(grdMain_DeleteCommand);
		}
		#endregion

		#region grdMain_Sort
		private void grdMain_Sort(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if(ViewState["grdMain_SortColumn"].ToString() == (string)e.SortExpression)
				grdMain_SortColumn = (string)e.SortExpression + " DESC";
			else
				grdMain_SortColumn = (string)e.SortExpression;

			ViewState["grdMain_SortColumn"] = grdMain_SortColumn;
			BindDG();
		}
		#endregion

		#region dg_PageSizeChanged
		private void dg_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["RepHist_PageSize"] = e.NewPageSize.ToString();
			BindDG();
		}
		#endregion

		#region dg_PageIndexChanged
		private void dg_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			grdMain.CurrentPageIndex = e.NewPageIndex;
			BindDG();
		}
		#endregion

		#region dg_view
		private void dg_view(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName=="View")
			{
				int ReportId = int.Parse(e.Item.Cells[0].Text);
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					"OpenWindow('../Reports/XMLReportOutput.aspx?ReportId="+
					ReportId.ToString()+"',screen.width,screen.height,true);", true);
			}
		}
		#endregion

		#region grdMain_DeleteCommand
		private void grdMain_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int iReportId = int.Parse(e.Item.Cells[0].Text);
			Report.DeleteReportItem(iReportId);
			BindDG();
		}
		#endregion

		#region btnAddToFavorites_Click
		protected void btnAddToFavorites_Click(object sender, EventArgs e)
		{
			Common.AddFavorites(TemplateId, ObjectTypes.Report);
		}
		#endregion
	}
}
