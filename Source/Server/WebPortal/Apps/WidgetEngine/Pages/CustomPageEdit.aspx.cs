using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Pages
{
	public partial class CustomPageEdit : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(Request["Id"]))
				pT.Title = GetGlobalResourceObject("IbnFramework.Profile", "NewPage").ToString();
			else
				pT.Title = GetGlobalResourceObject("IbnFramework.Profile", "PageEditing").ToString();
		}
	}
}
