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
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Pages
{
	public partial class PropertyPageContainer : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			RegisterScriptTags();
		}

		#region RegisterScriptTags
		/// <summary>
		/// Registers the script tags.
		/// </summary>
		private void RegisterScriptTags()
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ibn.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/grid.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ext-all2.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/IbnFramework/main.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/IbnFramework/common.js");

			CHelper.LoadExtJSGridScriptsToHead(this.Page);
		}
		#endregion
	}
}
