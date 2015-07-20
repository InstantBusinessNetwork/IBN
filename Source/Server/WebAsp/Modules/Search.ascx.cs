using System;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Configuration;	
using System.Resources;

namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	///		Summary description for Search.
	/// </summary>
	public partial  class Search : System.Web.UI.UserControl
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebAsp.App_GlobalResources.Resources.Sites", typeof(Search).Assembly);

		private string Keyword
		{
			get
			{
				return HttpUtility.UrlDecode(Request["keyword"]);
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindCompanyData();
			BindTrialData();
			BindResellers();
		}

		private void BindResellers()
		{
			dgResellers.DataSource = CManage.GetResellerByKeyword(Keyword);
			dgResellers.DataBind();
		}

		private void BindTrialData()
		{
			dgTrialRequests.Columns[0].HeaderText = LocRM.GetString("Title");
			dgTrialRequests.Columns[1].HeaderText = LocRM.GetString("Domain");
			dgTrialRequests.Columns[2].HeaderText = LocRM.GetString("CName");
			dgTrialRequests.Columns[3].HeaderText = LocRM.GetString("eMail");
			dgTrialRequests.Columns[4].HeaderText = LocRM.GetString("Phone");
			dgTrialRequests.Columns[5].HeaderText = "Reseller";

			dgTrialRequests.DataSource = CManage.GetTrialRequestByKeyword(Keyword);
			dgTrialRequests.DataBind();
		}

		private void BindCompanyData() 
		{
			dgCompanies.Columns[0].HeaderText = LocRM.GetString("Title");
			dgCompanies.Columns[1].HeaderText = LocRM.GetString("Domain");
			dgCompanies.Columns[2].HeaderText = LocRM.GetString("Type");
			dgCompanies.Columns[3].HeaderText = LocRM.GetString("Activity");
			dgCompanies.Columns[4].HeaderText = "Creation Date";
			dgCompanies.Columns[5].HeaderText = LocRM.GetString("StartDate");
			dgCompanies.Columns[6].HeaderText = LocRM.GetString("EndDate");

			dgCompanies.DataSource = CManage.GetCompanyByKeyword(Keyword);
			dgCompanies.DataBind();
		}

		protected string GetName(string _first, string _last)
		{
			return (_first+" "+_last);
		}

		protected string GetCompanyType(byte type)
		{
			if (type==1)
				return LocRM.GetString("Billable");
			else
				return LocRM.GetString("Trial");
		}

		protected string GetDate(object dt, byte type)
		{
			if (type==1)
				return "";
			else
			{
				if	(dt!=DBNull.Value)
					return ((DateTime)dt).ToShortDateString();
				else
					return "";
			}
		}

		protected string GetActivity(bool fl)
		{
			if(fl)
				return "<img alt='' src='../Layouts/Images/ontrack.gif'/>";
			else
				return "<img alt='' src='../Layouts/Images/atrisk.gif'/>";
		}

		protected string GetPercentage(int _val)
		{
			return _val.ToString()+"%";
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

		}
		#endregion
	}
}
