namespace Mediachase.UI.Web.UserReports.GlobalModules
{
	using System;
	using System.Data;
	using System.Collections;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using System.Resources;
	using Mediachase.UI.Web.UserReports.GlobalModules.PageTemplateExtension;
	using Mediachase.Ibn;
	using System.Text;
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for PageTemplateNew.
	/// </summary>

	public partial class PageTemplateNew : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.GlobalModules.Resources.strTemplate", typeof(PageTemplateNew).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		protected string sTitle = String.Format(" | {0} {1}", _productName, IbnConst.VersionMajorDotMinor);
		private static string _productName
		{
			get
			{
				return IbnConst.ProductFamily;
			}
		}

		#region Properties
		private string title = "";
		public string Title
		{
			set
			{
				title = value;
			}
			get
			{
				return title;
			}
		}

		private string controlName = "";
		public string ControlName
		{
			set
			{
				controlName = value;
			}
			get
			{
				return controlName;
			}
		}

		private string enctype = "application/x-www-form-urlencoded";
		public string Enctype
		{
			set { enctype = value; }
			get { return enctype; }
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			bodyTag.Attributes.Add("onload", "OnRedirect();");
			Response.Cache.SetNoStore();

			if (controlName != "")
			{
				frmMain.Enctype = this.enctype;

				System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
				phMain.Controls.Add(control);
			}
			RegisterScripts();
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			if (phMain.Controls[0] is IPageTemplateTitle)
				this.Title = ((IPageTemplateTitle)phMain.Controls[0]).Modify(this.Title);
		}
		#endregion

		#region RegisterScripts
		private void RegisterScripts()
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/common.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/toptabsnew.js");

			RegisterLocationScript(Page, sTitle);
		}
		#endregion

		#region public static void RegisterLocationScript(Page page, string title)
		public static void RegisterLocationScript(Page page, string title)
		{
			string pageUrl = page.ResolveUrl("~/Apps/Shell/Pages/default.aspx");

			StringBuilder builder = new StringBuilder();

			builder.AppendLine("	//<![CDATA[");
			builder.AppendLine("	if (parent == window) {");
			builder.AppendLine("		if (location.replace)");
			builder.AppendLine("			location.replace('" + pageUrl + "#right=' + escapeWithAmp(location.href));");
			builder.AppendLine("		else");
			builder.AppendLine("			location.href = '" + pageUrl + "#right=' + escapeWithAmp(location.href);");
			builder.AppendLine("	}");
			builder.AppendLine("	else {");
			builder.AppendLine("		if (parent && parent.document) {");
			builder.AppendLine("			var td = parent.document.getElementById(\"onetidPageTitle\");");
			builder.AppendLine("			if (td)");
			builder.AppendLine("				td.innerHTML = self.document.title;");
			builder.AppendLine("		}");
			builder.AppendLine("		top.document.title = self.document.title + '" + title + "';");
			builder.AppendLine("	}");
			builder.AppendLine("	function escapeWithAmp(str) {");
			builder.AppendLine("		var re = /&/gi;");
			builder.AppendLine("		var ampEncoded = \"%26\";");
			builder.AppendLine("		return escape(str).replace(re, ampEncoded);");
			builder.AppendLine("	}");
			builder.AppendLine("	//]]>");

			UtilHelper.RegisterScriptBlock(page, builder.ToString(), true);
		}
		#endregion
	}
}
