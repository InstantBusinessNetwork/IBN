using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	///		Summary description for LoginUser.
	/// </summary>
	public partial  class LoginUser : System.Web.UI.UserControl
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				BindDefaultValues();
				BindDGUsers();
			}
		}

		private void BindDefaultValues()
		{
			using (IDataReader rdr = CManage.GetCompany(Guid.Empty))
			{
				while (rdr.Read())
				{
					ddCompany.Items.Add(new ListItem(rdr["domain"].ToString() + " (" + rdr["company_name"].ToString() + ") ", rdr["company_uid"].ToString()));
				}
			}

			SetViewState();
		}

		private void SetViewState()
		{
			if (ddCompany.SelectedItem!=null)
				ViewState["CompanyUid"] = new Guid(ddCompany.SelectedItem.Value);
		}

		private void BindDGUsers()
		{
			if (ViewState["CompanyUid"]!=null)
			{
				DataView dv = null;
				Guid companyUid = (Guid)ViewState["CompanyUid"];
				DataTable dt = CManage.GetUsersDataTable(companyUid);
				if (dt != null)
				{
					dv = dt.DefaultView;
					dv.Sort = "LastName, FirstName, Email";
				}
				dgCompanyUser.DataSource = dv;
				dgCompanyUser.DataBind();
			}
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
			this.dgCompanyUser.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgCompanyUser_PageChange);

		}
		#endregion

		protected void btnApply_Click(object sender, System.EventArgs e)
		{
			SetViewState();
			BindDGUsers();
		}

		private void dgCompanyUser_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			dgCompanyUser.CurrentPageIndex = e.NewPageIndex;
			BindDGUsers();
		}
	}
}
