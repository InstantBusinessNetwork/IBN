using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Pages.Public
{
	public partial class ListTimeTrackingWeek : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			pT.Title = CHelper.GetResFileString("{IbnFramework.Global:_mc_TimeManagement}");
		}
	}
}
