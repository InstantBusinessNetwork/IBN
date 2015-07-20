using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business.WidgetEngine;

namespace Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Pages
{
	public partial class CustomPageView : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack && !String.IsNullOrEmpty(Request["PageUid"]))
			{
				CustomPageEntity page = CustomPageManager.GetCustomPage(new Guid(Request["PageUid"]), null, Mediachase.IBN.Business.Security.CurrentUser.UserID);
				if (page != null)
					pT.Title = CHelper.GetResFileString(page.Title);
			}
		}
	}
}
