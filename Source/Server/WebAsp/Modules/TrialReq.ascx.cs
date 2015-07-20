using System;
using System.Data;
using System.Resources;
using System.Web.UI.WebControls;

namespace Mediachase.Ibn.WebAsp.Modules
{
	public partial class TrialReq : System.Web.UI.UserControl
	{
		private const string ConstRequestSortingKey = "RequestSortingKey";
		private const string ConstFailedRequestSortingKey = "FailedRequestSortingKey";

		private string _requestsSorting = "CreationDate DESC";
		private string _failedRequestsSorting = "CreationDate DESC";

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebAsp.App_GlobalResources.Resources.Sites", typeof(TrialReq).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbar();
			if (!IsPostBack)
			{
				BindLists();
				ViewState[ConstRequestSortingKey] = _requestsSorting;
				ViewState[ConstFailedRequestSortingKey] = _failedRequestsSorting;
			}
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindTable();
			BindFailed();
		}

		private void BindLists()
		{
			ddReseller.DataSource = CManage.ResellerGet(0);
			ddReseller.DataTextField = "Title";
			ddReseller.DataValueField = "ResellerId";
			ddReseller.DataBind();
			ListItem liItem = new ListItem("All", "0");
			liItem.Selected = true;
			ddReseller.Items.Insert(0, liItem);

			ddStatus.Items.Add(new ListItem("All", "0"));
			ddStatus.Items.Add(new ListItem("Active", "1"));
			ddStatus.Items.Add(new ListItem("Pending", "-1"));
		}

		private void BindToolbar()
		{
			secH.Title = LocRM.GetString("tbTitle2");
			secH.AddLink("<img alt='' src='../Layouts/Images/xlsexport.gif'/> Export to Excel", "javascript:ExcelExport();");
			secH.AddSeparator();
			secH.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> Default Page", "../Pages/ASPHome.aspx");
		}

		private void BindFailed()
		{
			DataView dv = DBTrialRequestFailed.GetDataTable().DefaultView;
			dv.Sort = ViewState[ConstFailedRequestSortingKey].ToString();
			dgFailed.DataSource = dv;
			dgFailed.DataKeyField = "RequestID";
			dgFailed.DataBind();

			foreach (DataGridItem itm in dgFailed.Items)
			{
				ImageButton ib = (ImageButton)itm.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('Do you really want to delete this failed request?')");
			}
		}

		protected string GetError(int Error)
		{
			return ((Mediachase.Ibn.WebAsp.WebServices.TrialResult)Error).ToString();
		}

		private void BindTable()
		{
			SitesList.Columns[0].HeaderText = LocRM.GetString("Title");
			SitesList.Columns[1].HeaderText = LocRM.GetString("Domain");
			SitesList.Columns[2].HeaderText = LocRM.GetString("CName");
			SitesList.Columns[3].HeaderText = LocRM.GetString("eMail");
			SitesList.Columns[4].HeaderText = LocRM.GetString("Phone");
			SitesList.Columns[5].HeaderText = "Reseller";
			DataTable dt = DBTrialRequest.GetDataTable();
			DataView dv = dt.DefaultView;
			dv.Sort = ViewState[ConstRequestSortingKey].ToString();
			string sFilter = "";
			if (ddReseller.SelectedItem.Value != "0")
				sFilter = "ResellerTitle='" + ddReseller.SelectedItem.Text + "'";
			if (ddStatus.SelectedItem.Value == "1")
			{
				if (sFilter.Length > 0)
					sFilter += "AND IsActive='True'";
				else
					sFilter = "IsActive='True'";
			}
			if (ddStatus.SelectedItem.Value == "-1")
			{
				if (sFilter.Length > 0)
					sFilter += "AND IsActive='False'";
				else
					sFilter = "IsActive='False'";
			}
			if (sFilter.Length > 0)
				dv.RowFilter = sFilter;
			SitesList.DataSource = dv;
			SitesList.DataBind();
		}

		protected string GetName(string _first, string _last)
		{
			return (_first + " " + _last);
		}

		protected string GetEditPath(int ReqId)
		{
			return String.Format("<a href=\"../pages/TrialReqEdit.aspx?id=" + ReqId.ToString() + "\" title=\"{0}\"> <img alt='' src='../Layouts/Images/edit.gif' /></a>", "edit");
		}

		protected string GetDeletePath(int ReqId)
		{
			return String.Format("<a href=\"javascript:DeleteRequest({0})\" title=\"{1}\"><img alt='' src='../layouts/images/delete.gif' /></a>", ReqId, "delete");
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgFailed.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgFailed_Delete);
			this.SitesList.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.SitesList_Sort);
			this.dgFailed.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.dgFailed_Sort);
		}
		#endregion

		protected void lbDelete_Click(object sender, System.EventArgs e)
		{
			DBTrialRequest.Delete(int.Parse(txtID.Value));
		}

		protected void btnApply_Click(object sender, System.EventArgs e)
		{

		}

		protected void lbExport_Click(object sender, System.EventArgs e)
		{
			exportPanel.Visible = true;
			DataView dv = DBTrialRequest.GetDataTable().DefaultView;
			dv.Sort = ViewState[ConstRequestSortingKey].ToString();
			string sFilter = "";
			if (ddReseller.SelectedItem.Value != "0")
				sFilter = "ResellerTitle='" + ddReseller.SelectedItem.Text + "'";
			if (ddStatus.SelectedItem.Value == "1")
			{
				if (sFilter.Length > 0)
					sFilter += "AND IsActive='True'";
				else
					sFilter = "IsActive='True'";
			}
			if (ddStatus.SelectedItem.Value == "-1")
			{
				if (sFilter.Length > 0)
					sFilter += "AND IsActive='False'";
				else
					sFilter = "IsActive='False'";
			}
			if (sFilter.Length > 0)
				dv.RowFilter = sFilter;
			dgExport.DataSource = dv;
			dgExport.DataBind();
			CManage.ExportExcel(exportPanel, "RequestsExport.xls");
			exportPanel.Visible = false;
		}

		private void dgFailed_Delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int RequestID = (int)dgFailed.DataKeys[e.Item.ItemIndex];
			DBTrialRequestFailed.Delete(RequestID);
		}

		private void SitesList_Sort(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if (ViewState[ConstRequestSortingKey].ToString() == (string)e.SortExpression)
				_requestsSorting = (string)e.SortExpression + " DESC";
			else
				_requestsSorting = (string)e.SortExpression;

			ViewState[ConstRequestSortingKey] = _requestsSorting;
		}

		private void dgFailed_Sort(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if (ViewState[ConstFailedRequestSortingKey].ToString() == (string)e.SortExpression)
				_failedRequestsSorting = (string)e.SortExpression + " DESC";
			else
				_failedRequestsSorting = (string)e.SortExpression;

			ViewState[ConstFailedRequestSortingKey] = _failedRequestsSorting;
		}
	}
}
