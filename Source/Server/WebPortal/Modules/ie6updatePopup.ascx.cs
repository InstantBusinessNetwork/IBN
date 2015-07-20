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

namespace Mediachase.UI.Web.Modules
{
	public partial class ie6updatePopup : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request.Browser.Browser.Contains("IE") && Request.Browser.MajorVersion < 7)
			{
				divPopupIe.Style.Add("display", "block");
				divPopupIe.Visible = true;
				this.Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "hideAllDropDown();", true);
			}
			else
			{
				divPopupIe.Visible = false;
			}
		}
	}
}