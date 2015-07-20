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

namespace Mediachase.Ibn.Web.UI.ListApp.Pages
{
	public partial class HistoryDataView : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			pT.Title = CHelper.GetResFileString("{IbnFramework.ListInfo:History}");
		}
	}
}
