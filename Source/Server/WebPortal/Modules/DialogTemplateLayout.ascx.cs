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
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Modules
{
	public partial class DialogTemplateLayout : System.Web.UI.UserControl
	{
		private Hashtable controlProperties = new Hashtable();

		#region ControlName
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
		#endregion

		#region Title
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
		#endregion

		#region Enctype
		private string enctype = "application/x-www-form-urlencoded";
		public string Enctype
		{
			set { enctype = value; }
			get { return enctype; }
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			iconIBN.Href = Page.ResolveUrl("~/portal.ico");
			RegisterScripts();

			Response.Cache.SetNoStore();
			frmMain.Enctype = this.Enctype;

			if (controlName != "")
			{
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
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (Page.Title.ToLower() == "untitled page")
				Page.Title = CHelper.GetFullPageTitle(Title);
			else
				Page.Title = CHelper.GetFullPageTitle(Page.Title);
		}
		#endregion

		private void RegisterScripts()
		{
			// Styles
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ibn.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ext-all2.css");

			// Scripts
			UtilHelper.RegisterScript(Page, "~/Scripts/IbnFramework/main.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/IbnFramework/common.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");

			CHelper.LoadExtJSGridScriptsToHead(Page);
		}
	}
}