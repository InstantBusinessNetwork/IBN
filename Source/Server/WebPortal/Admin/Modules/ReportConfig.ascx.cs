namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.ControlSystem;
	using Mediachase.UI.Web.Modules;
	using System.Reflection;

	/// <summary>
	///		Summary description for ReportConfig.
	/// </summary>
	public partial class ReportConfig : System.Web.UI.UserControl
	{


		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		BaseIbnContainer bic;
		Mediachase.IBN.Business.ControlSystem.ReportStorage rs;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			bic = BaseIbnContainer.Create("Reports", "GlobalReports");
			rs = (ReportStorage)bic.LoadControl("ReportStorage");
			if (!Page.IsPostBack)
			{
				if (pc["rep_Config_Sort"] == null)
					pc["rep_Config_Sort"] = "sortName";
				BindDG();
			}
			BindToolBar();
		}

		#region BindToolBar
		private void BindToolBar()
		{
			//tbLightRS.ActionsMenu.Items.Clear();
			//tbLightRS.ClearRightItems();
			//tbLightRS.AddText(LocRM.GetString("tRepConfig"));
			tbLightRS.Title = LocRM.GetString("tRepConfig");
			tbLightRS.AddLink("<img alt='' src='" + ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tCommonSettings"), ResolveClientUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin3"));
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			int i = 2;
			grdMain.Columns[i++].HeaderText = LocRM.GetString("FieldName");
			grdMain.Columns[i++].HeaderText = LocRM.GetString("tIsPersonal");
			grdMain.Columns[i++].HeaderText = LocRM.GetString("tCategory");

			foreach (DataGridColumn dgc in grdMain.Columns)
			{
				if (dgc.SortExpression == pc["rep_Config_Sort"].ToString())
					dgc.HeaderText += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>",
						ResolveUrl("~/layouts/images/upbtnF.jpg"));
				else if (dgc.SortExpression + " DESC" == pc["rep_Config_Sort"].ToString())
					dgc.HeaderText += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>",
						ResolveUrl("~/layouts/images/downbtnF.jpg"));
			}

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("sortName", typeof(string)));
			dt.Columns.Add(new DataColumn("Description", typeof(string)));
			dt.Columns.Add(new DataColumn("Category", typeof(string)));
			dt.Columns.Add(new DataColumn("CategoryId", typeof(int)));
			dt.Columns.Add(new DataColumn("ActionVS", typeof(string)));
			dt.Columns.Add(new DataColumn("Type", typeof(string)));
			DataRow dr;

			ReportInfo[] _ri = rs.GetReports();
			foreach (ReportInfo ri in _ri)
			{
				if (ri.Name == "ProjectTime" && !Configuration.ProjectManagementEnabled)
					continue;
				dr = dt.NewRow();
				dr["Id"] = ri.Id;
				dr["Name"] = String.Format("<a href='{0}'>{1}</a>", ResolveUrl("~" + ri.Url), ri.ShowName);
				dr["sortName"] = ri.ShowName;
				dr["Description"] = ri.Description;
				dr["Category"] = ((ri.Category == null) || (ri.Category.Id == 0)) ? "" : ri.Category.Name;
				dr["CategoryId"] = (ri.Category == null) ? 0 : ri.Category.Id;
				dr["ActionVS"] = String.Format("<a href=\"{0}\"><img width='16' height='16' align='absmiddle' border='0' src='{1}' title='{2}'/></a>",
						String.Format("javascript:FolderSecurity({0});", ri.Id),
						ResolveUrl("~/layouts/images/icon-key.gif"),
						LocRM.GetString("tSecurity"));
				switch (ri.Type)
				{
					case Mediachase.IBN.Business.UserReport.UserReportType.Global:
						dr["Type"] = LocRM.GetString("tGlobalType");
						break;
					case Mediachase.IBN.Business.UserReport.UserReportType.Personal:
						dr["Type"] = LocRM.GetString("tPersonalType");
						break;
					case Mediachase.IBN.Business.UserReport.UserReportType.Project:
						dr["Type"] = LocRM.GetString("tProjectType");
						break;
					default:
						break;
				}
				dt.Rows.Add(dr);
			}

			DataView dv = dt.DefaultView;
			dv.Sort = pc["rep_Config_Sort"].ToString();

			if (pc["rep_Config_PageSize"] == null)
				pc["rep_Config_PageSize"] = "10";
			grdMain.PageSize = int.Parse(pc["rep_Config_PageSize"].ToString());

			if (pc["rep_Config_Page"] == null)
				pc["rep_Config_Page"] = "0";
			int PageIndex = int.Parse(pc["rep_Config_Page"].ToString());
			int ppi = dv.Count / grdMain.PageSize;
			if (dv.Count % grdMain.PageSize == 0)
				ppi = ppi - 1;
			if (PageIndex <= ppi)
			{
				grdMain.CurrentPageIndex = PageIndex;
			}
			else
			{
				grdMain.CurrentPageIndex = 0;
				pc["rep_Config_Page"] = "0";
			}

			grdMain.DataSource = dv;
			grdMain.DataBind();

			foreach (DataGridItem dgi in grdMain.Items)
			{
				DropDownList ddl = (DropDownList)dgi.FindControl("ddCategory");
				if (ddl != null)
				{
					ddl.Items.Add(new ListItem(LocRM.GetString("tNotSet"), "0"));
					ReportCategoryInfo[] _ci = ReportStorage.GetCategories();
					foreach (ReportCategoryInfo ci in _ci)
						ddl.Items.Add(new ListItem(ci.Name, ci.Id.ToString()));
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
			this.grdMain.SortCommand += new DataGridSortCommandEventHandler(grdMain_SortCommand);
			this.grdMain.PageIndexChanged += new DataGridPageChangedEventHandler(grdMain_PageIndexChanged);
			this.grdMain.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(grdMain_PageSizeChanged);
			this.grdMain.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_edit);
			this.grdMain.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_cancel);
			this.grdMain.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_update);
		}
		#endregion

		#region DataGridCommands
		private void dg_edit(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			grdMain.EditItemIndex = e.Item.ItemIndex;
			grdMain.DataKeyField = "Id";
			BindDG();
			foreach (DataGridItem dgi in grdMain.Items)
			{
				DropDownList ddl = (DropDownList)dgi.FindControl("ddCategory");
				if (ddl != null)
					ddl.SelectedValue = e.Item.Cells[1].Text;
			}
		}

		private void dg_cancel(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			Response.Redirect("~/Admin/ReportConfig.aspx");
		}

		private void dg_update(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			string sReportId = grdMain.DataKeys[e.Item.ItemIndex].ToString();
			DropDownList ddl = (DropDownList)e.Item.FindControl("ddCategory");
			if (ddl != null)
				rs.SetReportCategory(int.Parse(sReportId), int.Parse(ddl.SelectedValue));
			Response.Redirect("~/Admin/ReportConfig.aspx");
		}
		#endregion

		#region DataGrid Events
		private void grdMain_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (pc["rep_Config_Sort"].ToString() == (string)e.SortExpression)
				pc["rep_Config_Sort"] = (string)e.SortExpression + " DESC";
			else
				pc["rep_Config_Sort"] = (string)e.SortExpression;

			BindDG();
		}

		private void grdMain_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			pc["rep_Config_Page"] = e.NewPageIndex.ToString();
			BindDG();
		}

		private void grdMain_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["rep_Config_PageSize"] = e.NewPageSize.ToString();
			BindDG();
		}
		#endregion

	}
}
