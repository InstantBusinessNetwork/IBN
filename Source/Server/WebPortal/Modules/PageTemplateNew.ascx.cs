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
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.Interfaces;
using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.Modules
{
	public partial class PageTemplateNew : System.Web.UI.UserControl
	{

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

		protected string sTitle = String.Format(" | {0} {1}", IbnConst.ProductFamily, IbnConst.VersionMajorDotMinor);
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(PageTemplateNew).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, EventArgs e)
		{
			iconIBN.Href = Page.ResolveUrl("~/portal.ico");
			RegisterScripts();

			Response.Cache.SetNoStore();

			frmMain.Enctype = this.enctype;
			if (controlName != "")
			{
				System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
				phMain.Controls.Add(control);
			}
		}

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
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcBlockMenu.css");

			// Scripts
			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/common.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/AjaxFU.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/toptabsnew.js");

			CommonHelper.RegisterLocationScript(Page, sTitle);
		}
	}
}