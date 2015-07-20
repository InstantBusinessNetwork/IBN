using System;

using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Shell.Pages
{
	public partial class _default : System.Web.UI.Page
	{
		//protected string defaultLink = CHelper.GetAbsolutePath("/Workspace/default.aspx?BTab=Workspace");
		protected string defaultLink = String.Empty;
		protected string sTitle = String.Format("{0} {1}", IbnConst.ProductFamily, IbnConst.VersionMajorDotMinor);

		protected void Page_Load(object sender, EventArgs e)
		{
			iconIBN.Attributes["href"] = ResolveUrl("~/portal.ico");

			Response.Cache.SetNoStore();

			if (Mediachase.IBN.Business.Configuration.LicenseExpired)
			{
				Response.Redirect("~/Logoff.aspx");
				return;
			}

			GetDefaultLink();
			RegisterScripts();
		}

		#region GetDefaultLink
		private void GetDefaultLink()
		{
			string s = System.Configuration.ConfigurationManager.AppSettings["ShellFirstPageUrl"];
			if (s.StartsWith("http"))
			{
				defaultLink = s;
				return;
			}
			else if (s.StartsWith("~"))
			{
				//s = s.Substring(1);
				//s = CHelper.GetAbsolutePath(s);
				s = this.Page.ResolveUrl(s);
				defaultLink = s;
			}
			else
				//defaultLink = CHelper.GetAbsolutePath(s);
				defaultLink = this.Page.ResolveUrl("~/Workspace/default.aspx?BTab=Workspace");
		} 
		#endregion

		#region RegisterScripts
		private void RegisterScripts()
		{
			// Styles
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ext-all2.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ibn.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcBlockMenu.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Grid.css");

			// Scripts
			UtilHelper.RegisterScript(Page, "~/Scripts/IbnFramework/JsDebug.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/IbnFramework/yahoo-min.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/IbnFramework/ext-all.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/Shell/mainLayout.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/Shell/mainHistory.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/common.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/toptabsnew.js");

			UtilHelper.RegisterScript(Page, "~/Scripts/IbnFramework/dom-min.js", true);
			UtilHelper.RegisterScript(Page, "~/Scripts/IbnFramework/event-min.js", true);
			UtilHelper.RegisterScript(Page, "~/Scripts/IbnFramework/animation-min.js", true);
		}
		#endregion
	}
}
