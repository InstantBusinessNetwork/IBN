using System;
using System.Data;
using System.Collections;
using System.Resources;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Pages.Public
{
	public partial class ListTimeTrackingReport : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strWorkspace", Assembly.GetExecutingAssembly());
			if(Request["ReportType"]!= null && Request["ReportType"] == "Admin")
				pT.Title = LocRM.GetString("tTTReportTitleAdmin");
			else
				pT.Title = LocRM.GetString("tTTReportTitleAdminMy");
		}
	}

	public class ListTimeTrackingReportInfo : Mediachase.IBN.Business.UserReport.IUserReportInfo
	{
		#region IUserReportInfo Members
		ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strWorkspace", Assembly.GetExecutingAssembly());
		public string Description
		{
			get
			{
				return string.Empty;
			}
		}

		public string ShowName
		{
			get
			{
				return LocRM.GetString("tTTReportTitleAdmin");
			}
		}

		#endregion

	}

	public class ListTimeTrackingReportMyInfo : Mediachase.IBN.Business.UserReport.IUserReportInfo
	{
		#region IUserReportInfo Members
		ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strWorkspace", Assembly.GetExecutingAssembly());
		public string Description
		{
			get
			{
				return string.Empty;
			}
		}

		public string ShowName
		{
			get
			{
				return LocRM.GetString("tTTReportTitleAdminMy");
			}
		}

		#endregion

	}
}
