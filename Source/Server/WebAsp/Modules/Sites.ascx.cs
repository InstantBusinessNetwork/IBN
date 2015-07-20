using System;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;

using Mediachase.Ibn.Configuration;


namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	///		Summary description for Library.
	/// </summary>
	public partial class Sites : System.Web.UI.UserControl, IPageTemplateTitle
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebAsp.App_GlobalResources.Resources.Sites", typeof(Sites).Assembly);
		protected string SortColumn = "creation_date DESC";
		protected string SortColumn_key = "SiteList_SortColumn";
		protected string PageSize_key = "SiteList_PageSize";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (Session[SortColumn_key] == null)
				Session[SortColumn_key] = SortColumn;

			if (Session[PageSize_key] == null)
				Session[PageSize_key] = "25";

			BindToolbar();
			if (!this.IsPostBack)
			{
				BindLists();
				BindData();
			}
		}

		#region BindToolbar
		private void BindToolbar()
		{
			secH.Title = LocRM.GetString("tbTitle");
			secH.AddLink("<img alt='' src='../Layouts/Images/newitem.gif'/> " + LocRM.GetString("CreateSite"), "../Pages/SiteCreate.aspx");
			secH.AddSeparator();
			secH.AddLink("<img alt='' src='../Layouts/Images/newitem.gif'/> " + LocRM.GetString("CreateTrial"), "../Pages/SiteCreate.aspx?Trial=1");
			secH.AddSeparator();
			secH.AddLink("<img alt='' src='../Layouts/Images/xlsexport.gif'/> Export to Excel", Page.ClientScript.GetPostBackClientHyperlink(lbExport, ""));
			secH.AddSeparator();
			secH.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> Default Page", "../Pages/ASPHome.aspx");
		}
		#endregion

		#region BindLists
		private void BindLists()
		{
			ddType.Items.Add(new ListItem("All", "0"));
			ddType.Items.Add(new ListItem("Billable", "1"));
			ddType.Items.Add(new ListItem("Trial", "2"));

			ddActivity.Items.Add(new ListItem("All", "0"));
			ddActivity.Items.Add(new ListItem("Active", "1"));
			ddActivity.Items.Add(new ListItem("Inactive", "-1"));

			ListItem li = PageSizeList.Items.FindByValue(Session[PageSize_key].ToString());
			if (li != null)
				li.Selected = true;
		}
		#endregion

		#region BindData
		private void BindData()
		{
			SitesList.PageSize = int.Parse(PageSizeList.SelectedValue);

			SitesList.DataSource = GetDataView();
			SitesList.DataBind();
		}
		#endregion

		#region GetCompanyType
		protected string GetCompanyType(byte type)
		{
			if (type == 1)
				return LocRM.GetString("Billable");
			else
				return LocRM.GetString("Trial");
		}
		#endregion

		#region GetDate
		protected string GetDate(object dt, byte type)
		{
			if (dt != DBNull.Value)
				return ((DateTime)dt).ToString("d", CultureInfo.CurrentCulture);
			else
				return "";
		}
		#endregion

		#region GetActivity
		protected string GetActivity(bool fl)
		{
			if (fl)
				return "<img alt='' src='../Layouts/Images/ontrack.gif'/>";
			else
				return "<img alt='' src='../Layouts/Images/atrisk.gif'/>";
		}
		#endregion

		#region GetDeletePath
		protected string GetDeletePath(string siteId, string company, string domain)
		{
			string script = string.Format(CultureInfo.InvariantCulture, "javascript:DeleteSite(\"{0}\",\"{1}\",\"{2}\")", ScriptEncode(siteId), ScriptEncode(company), ScriptEncode(domain));
			return String.Format("<a href=\"{0}\" title=\"Delete {1}\"><img alt=\"\" src=\"{2}\" /></a>", HttpUtility.HtmlAttributeEncode(script), HttpUtility.HtmlAttributeEncode(company), Page.ResolveUrl("~/Layouts/Images/delete.gif"));
		}
		#endregion

		#region GetEditPath
		protected string GetEditPath(string siteId)
		{
			return String.Concat("<a href=\"../pages/SiteEdit.aspx?id=", siteId, "\" title=\"Edit\"> <img alt='' src='../Layouts/Images/edit.gif' /></a>");
		}
		#endregion

		#region GetErrorLogPath
		protected string GetErrorLogPath(string siteId)
		{
			return String.Concat("<a href=\"../pages/SiteErrorLog.aspx?id=", siteId, "\" title='Error Log'> <img alt='' src='../Layouts/Images/errorlog.gif' /></a>");
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
			this.SitesList.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgT_ItemCreated);
			this.SitesList.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.SitesList_PageChange);
			this.SitesList.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.SitesList_Sort);

		}
		#endregion

		#region lbDelete_Click
		protected void lbDelete_Click(object sender, System.EventArgs e)
		{
			CManage.DeleteCompany(new Guid(txtID.Value));
			BindData();
		}
		#endregion

		#region SitesList_Sort
		private void SitesList_Sort(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if (Session[SortColumn_key].ToString() == (string)e.SortExpression)
				SortColumn = (string)e.SortExpression + " DESC";
			else
				SortColumn = (string)e.SortExpression;

			Session[SortColumn_key] = SortColumn;
			BindData();
		}
		#endregion

		#region Implementation of IPageTemplateTitle
		public string Modify(string oldValue)
		{
			return LocRM.GetString("ManageSites");
		}
		#endregion

		#region SitesList_PageChange
		private void SitesList_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			SitesList.CurrentPageIndex = e.NewPageIndex;
			BindData();
		}
		#endregion 

		#region dgT_ItemCreated
		private void dgT_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{

			ListItemType elemType = e.Item.ItemType;
			if (elemType == ListItemType.Pager)
			{
				TableCell pager = (TableCell)e.Item.Controls[0];
				Label lab = new Label();
				lab.Text = "Page: ";

				pager.Controls.AddAt(0, lab);

				for (int i = 0; i < pager.Controls.Count; i++)
				{
					Object o = pager.Controls[i];
					if (o is LinkButton)
					{
						LinkButton h = (LinkButton)o;
						h.Text = "[" + h.Text + "]";
					}
					else if (o is Label && o != lab)
					{
						Label l = (Label)o;
						l.Text = "[" + l.Text + "]";
					}
				}
			}
		}
		#endregion

		#region lbExport_Click
		protected void lbExport_Click(object sender, System.EventArgs e)
		{
			exportPanel.Visible = true;

			dgExport.DataSource = GetDataView();
			dgExport.DataBind();
			CManage.ExportExcel(exportPanel, "SitesExport.xls");

			exportPanel.Visible = false;
		}
		#endregion

		#region btnApply_Click
		protected void btnApply_Click(object sender, System.EventArgs e)
		{
			SitesList.CurrentPageIndex = 0;
			BindData();
		}
		#endregion

		#region GetDataView
		private DataView GetDataView()
		{
/*			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("company_uid", typeof(string));
			dt.Columns.Add("company_name", typeof(string));
			dt.Columns.Add("domain", typeof(string));
			dt.Columns.Add("company_type", typeof(byte));
			dt.Columns.Add("is_active", typeof(bool));
			dt.Columns.Add("creation_date", typeof(DateTime));
			dt.Columns.Add("start_date", typeof(DateTime));
			dt.Columns.Add("end_date", typeof(DateTime));

			ICompanyInfo[] companies = Configurator.Create().ListCompanies();
			foreach (ICompanyInfo company in companies)
			{
				DataRow row = dt.NewRow();
				row["company_uid"] = company.Id;
				row["company_name"] = company.;

				dt.Rows.Add(row);
			}
*/

			DataView view = CManage.GetCompaniesDataTable().DefaultView;

			view.Sort = Session[SortColumn_key].ToString();

			string filter = string.Empty;

			if (ddType.SelectedItem.Value != "0")
				filter = "company_type='" + ddType.SelectedItem.Value + "'";

			if (ddActivity.SelectedItem.Value == "1")
			{
				if (filter.Length > 0)
					filter += " AND ";
				filter += "Is_Active='True'";
			}

			if (ddActivity.SelectedItem.Value == "-1")
			{
				if (filter.Length > 0)
					filter += " AND ";
				filter += "Is_Active='False'";
			}

			if (filter.Length > 0)
				view.RowFilter = filter;

			return view;
		}
		#endregion

		#region PageSizeList_SelectedIndexChanged
		protected void PageSizeList_SelectedIndexChanged(object sender, EventArgs e)
		{
			Session[PageSize_key] = PageSizeList.SelectedValue;
			SitesList.CurrentPageIndex = 0;

			BindData();
		}
		#endregion

		private static string ScriptEncode(string value)
		{
			string result = value;

			if (!string.IsNullOrEmpty(result))
			{
				result = result.Replace("\\", "\\\\");
				result = result.Replace("\"", "\\\"");
			}

			return result;
		}
	}
}
