using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.WebAsp
{
	public class UtilHelper
	{
		#region public static void RegisterCssStyleSheet(Page page, string link)
		public static void RegisterCssStyleSheet(Page page, string link)
		{
			if (page == null)
				throw new ArgumentNullException("page");
			if (link == null)
				throw new ArgumentNullException("link");
			if (page.Header == null)
				throw new NullReferenceException("page.Header returned null. Add runat=\"server\" to the <head> element.");

			HtmlLink htmlLink = new HtmlLink();

			htmlLink.Attributes["type"] = "text/css";
			htmlLink.Attributes["rel"] = "stylesheet";
			htmlLink.Href = page.ResolveUrl(link);

			page.Header.Controls.Add(htmlLink);
		}
		#endregion

		#region public static void RegisterScript(Page page, string link)
		public static void RegisterScript(Page page, string link)
		{
			RegisterScript(page, link, false);
		}
		#endregion
		#region public static void RegisterScript(Page page, string link, bool defer)
		public static void RegisterScript(Page page, string link, bool defer)
		{
			if (page == null)
				throw new ArgumentNullException("page");
			if (link == null)
				throw new ArgumentNullException("link");
			if (page.Header == null)
				throw new NullReferenceException("page.Header returned null. Add runat=\"server\" to the <head> element.");

			HtmlGenericControl child = new HtmlGenericControl();

			child.TagName = "script";
			child.Attributes["type"] = "text/javascript";

			if (defer)
				child.Attributes["defer"] = "defer";

			child.Attributes["src"] = page.ResolveUrl(link);

			page.Header.Controls.Add(child);
		}
		#endregion

		#region public static void RegisterScriptBlock(Page page, string script)
		public static void RegisterScriptBlock(Page page, string script)
		{
			RegisterScriptBlock(page, script, false);
		}
		#endregion
		#region public static void RegisterScriptBlock(Page page, string script, bool defer)
		public static void RegisterScriptBlock(Page page, string script, bool defer)
		{
			if (page == null)
				throw new ArgumentNullException("page");
			if (script == null)
				throw new ArgumentNullException("script");
			if (page.Header == null)
				throw new NullReferenceException("page.Header returned null. Add runat=\"server\" to the <head> element.");

			HtmlGenericControl child = new HtmlGenericControl();

			child.TagName = "script";
			child.Attributes["type"] = "text/javascript";

			if (defer)
				child.Attributes["defer"] = "defer";

			child.InnerHtml = script;

			page.Header.Controls.Add(child);
		}
		#endregion
	}
}
