using System;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Web;
using System.Web.UI;

using Mediachase.Ibn;
using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.Modules
{
	public partial class upTemplate1 : System.Web.UI.UserControl
	{
		protected UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", Assembly.GetExecutingAssembly());

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Thread.CurrentThread.CurrentUICulture.DateTimeFormat.PMDesignator == String.Empty)
				Is24Hours.Value = "1";

			int offset = Mediachase.IBN.Business.User.GetCurrentBias();
			TimeOffSet.Value = offset.ToString();

			DateTime dtExpired = Company.TrialEndDate;
			if (dtExpired > DateTime.MinValue && dtExpired < DateTime.MaxValue)
			{
				lblExpirationDate.Text = String.Format("{0}&nbsp;<b>{1}</b>",
					LocRM.GetString("tPeriodWillExpired"),
					dtExpired.ToShortDateString());
				if (GlobalResourceManager.Options["BuyLinkVisible"])
					lblExpirationDate.Text += String.Format("&nbsp;&nbsp;<span class='ibn-propertysheet' style='font-size:.9em;font-weight:bold;'><a href='{1}' target='_blank'>{0}</a></span>",
						LocRM.GetString("tBuy"),
						GlobalResourceManager.Strings["BuyNowLink"]);
				//(Security.CurrentUser != null && Security.CurrentUser.Culture.ToLower().IndexOf("ru") >= 0) ? "http://pmbox.ru/price6.aspx" : "http://www.mediachase.com/ibn/overview.aspx");
			}
			if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated != false)
			{
				lblUser.Text = String.Format("<a href='{4}?UserID={0}' title='{1}' target='right'>[ {2} ]</a>&nbsp;<a href='{5}?Back=View&amp;UserID={0}' title='{1}' target='right'><img alt='' src='{6}' width='16' height='16' style='vertical-align:middle' border='0' title='{3}'/></a>",
					Security.CurrentUser.UserID,
					HttpUtility.HtmlEncode(LocRM.GetString("ViewProfile")),
					HttpUtility.HtmlEncode(Security.CurrentUser.LastName + " " + Security.CurrentUser.FirstName),
					HttpUtility.HtmlEncode(LocRM.GetString("EditPreferences")),
					HttpUtility.HtmlEncode(ResolveClientUrl("~/Directory/UserView.aspx")),
					HttpUtility.HtmlEncode(ResolveClientUrl("~/Directory/PrefsEdit.aspx")),
					HttpUtility.HtmlEncode(ResolveClientUrl("~/Layouts/Images/icons/user_Edit.GIF")));
				lblTime.Text = ", " + CommonHelper.GetStringTimeZoneOffset(Security.CurrentUser.TimeZoneId);
			}
			lblUser.EnableViewState = false;

			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), Guid.NewGuid().ToString(), ResolveClientUrl("~/Scripts/upTemplate.js"));

		}
	}
}