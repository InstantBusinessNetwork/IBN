using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Configuration;
using System.Globalization;

namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	///		Summary description for Reports.
	/// </summary>
	public partial  class Reports : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Header2.Title = "Recent Error Events";
			Header3.Title = "Statistics";
		}

		private void Page_PreRender(object sender, EventArgs e)
		{
			BindData();
		}

		private void BindData()
		{
			DataTable dtError = new DataTable();
			dtError.Columns.Add("Domain", typeof(string));
			dtError.Columns.Add("Error", typeof(string));
			dtError.Columns.Add("CreationTime", typeof(DateTime));

			IConfigurator config = Configurator.Create();
			string installPath = config.InstallPath;

			foreach (ICompanyInfo company in config.ListCompanies(false))
			{
				string companyCodePath = Path.Combine(installPath, company.CodePath);
				string errorsPath = Path.Combine(companyCodePath, @"Web\Portal\Admin\Log\Error");

				DirectoryInfo dir = new DirectoryInfo(errorsPath);
				foreach (FileInfo fileinfo in dir.GetFiles())
				{
					string ErrorLink = fileinfo.Name;

					string ext = "";
					if (ErrorLink.IndexOf(".aspx") >= 0)
						ext = ".aspx";
					else if (ErrorLink.IndexOf(".html") >= 0)
						ext = ".html";
					else
						continue;

					string PureName = ErrorLink.Substring(0, ErrorLink.IndexOf(ext));
					string ErrorID = PureName.Substring(PureName.LastIndexOf("_") + 1);
					string ErrorPortal = ErrorLink.Substring(0, PureName.LastIndexOf("_"));

					if (fileinfo.Length == 0)
						continue;

					DataRow dr = dtError.NewRow();
					dr["Domain"] = company.Host;
					dr["Error"] = String.Format(CultureInfo.InvariantCulture,
						"<a href='../Pages/SiteErrorLog.aspx?fileName={0}&amp;id={1}&amp;back=reports'>{2}</a>",
						PureName,
						company.Id,
						ErrorID);
					dr["CreationTime"] = fileinfo.CreationTime;
					dtError.Rows.Add(dr);
				}
			}
			
			DataView dv = dtError.DefaultView;
			dv.Sort = "CreationTime DESC";

			dgErrors.DataSource = dv;
			if(dv.Count <= 25)
				dgErrors.PagerStyle.Visible = false;
			dgErrors.DataBind();

			using(IDataReader reader = CManage.GetTotalStatistic())
			{
				if(reader.Read())
				{
					BillableCountValue.Text = reader["BillableCount"].ToString();
					ActiveBillableCountValue.Text = reader["ActiveBillableCount"].ToString();
					InactiveBillableCountValue.Text = ((int)reader["BillableCount"] - (int)reader["ActiveBillableCount"]).ToString();

					TrialCountValue.Text = reader["TrialCount"].ToString();
					ActiveTrialCountValue.Text = reader["ActiveTrialCount"].ToString();
					InactiveTrialCountValue.Text = ((int)reader["TrialCount"] - (int)reader["ActiveTrialCount"]).ToString();
					
					CompaniesCountValue.Text = reader["CompaniesCount"].ToString();
					ActiveCountValue.Text = reader["ActiveCount"].ToString();
					InactiveCountValue.Text = ((int)reader["CompaniesCount"] - (int)reader["ActiveCount"]).ToString();

					PendingTrialsCountValue.Text = reader["PendingTrialsCount"].ToString();
				}
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
			this.dgErrors.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_PageChange);
		}
		#endregion

		private void dg_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			dgErrors.CurrentPageIndex = e.NewPageIndex;
			//BindData();
		}

	}
}
