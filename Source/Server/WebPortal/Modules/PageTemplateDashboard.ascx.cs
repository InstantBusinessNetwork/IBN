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
using System.Resources;
using Mediachase.IBN.Business;
using System.Reflection;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn;
using Mediachase.Ibn.Web.Interfaces;
using System.Text;
using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.Modules
{
	public partial class PageTemplateDashboard : System.Web.UI.UserControl
	{
		private Hashtable controlProperties = new Hashtable();

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", Assembly.GetExecutingAssembly());
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		protected string sTitle = String.Format(" | {0} {1}", IbnConst.ProductFamily, IbnConst.VersionMajorDotMinor);

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

		public UserControl CurrentControl
		{
			get
			{
				return (phMain.Controls.Count > 0) ? (UserControl)phMain.Controls[0] : null;
			}
		}
		#endregion


		protected void Page_Load(object sender, EventArgs e)
		{
			iconIBN.Href = Page.ResolveUrl("~/portal.ico");
			RegisterScripts();

			Response.Cache.SetNoStore();
			if (controlName != "")
			{
				frmMain.Enctype = this.enctype;

				System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
				foreach (DictionaryEntry de in controlProperties)
				{
					control.GetType().BaseType.GetProperty(de.Key.ToString()).SetValue(control, de.Value, null);
				}

				phMain.Controls.Add(control);
			}
		}

		#region SetControlProperties
		public void SetControlProperties(string key, object value)
		{
			controlProperties.Add(key, value);
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			if (phMain.Controls[0] is IPageTemplateTitle)
				this.Title = ((IPageTemplateTitle)phMain.Controls[0]).Modify(this.Title);
		}
		#endregion

		private void RegisterScripts()
		{
			// Styles
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ibn.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Grid.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcBlockMenu.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/menuStyle.css");

			// Scripts
			UtilHelper.RegisterScript(Page, "~/Scripts/IbnFramework/JsDebug.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/common.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/toptabsnew.js");

			CHelper.LoadExtJSGridScriptsToHead(Page);

			CommonHelper.RegisterLocationScript(Page, sTitle);
		}
	}
}