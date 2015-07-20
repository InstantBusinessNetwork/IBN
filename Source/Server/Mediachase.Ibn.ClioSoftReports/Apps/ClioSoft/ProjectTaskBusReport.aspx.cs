using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Resources;

namespace Mediachase.Ibn.Apps.ClioSoft
{
	public partial class ProjectTaskBusReport : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.ClioSoftReports.App_GlobalResources.Report", typeof(ProjectTaskBusReport).Assembly);
			pT.Title = LocRM.GetString("ManagerReportName");
		}
	}
}
