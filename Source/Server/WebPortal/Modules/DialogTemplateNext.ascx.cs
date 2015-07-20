using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Modules
{
	public partial class DialogTemplateNext : System.Web.UI.UserControl
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
		public string Title
		{
			set
			{
				ViewState["Title"] = value;
			}
			get
			{
				string retval = String.Empty;
				if (ViewState["Title"] != null)
					retval = ViewState["Title"].ToString();
				return retval;
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

		#region CurrentControl
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
			RegisterScrips();

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
			if (String.IsNullOrEmpty(Page.Title) || Page.Title.ToLower() == "untitled page")
				Page.Title = CHelper.GetFullPageTitle(Title);
			else
				Page.Title = CHelper.GetFullPageTitle(Page.Title);
		}
		#endregion

		private void RegisterScrips()
		{
			// Styles
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ibn.css");

			// Scripts
			UtilHelper.RegisterScript(Page, "~/Scripts/IbnFramework/main.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/IbnFramework/common.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
		}
	}
}