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

namespace Mediachase.Ibn.Apps.ClioSoft.Modules
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

		protected void Page_Load(object sender, EventArgs e)
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

		#region RegisterScripts
		private void RegisterScripts()
		{
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), Guid.NewGuid().ToString(), ResolveClientUrl("~/Scripts/browser.js"));
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), Guid.NewGuid().ToString(), ResolveClientUrl("~/Scripts/common.js"));
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), Guid.NewGuid().ToString(), ResolveClientUrl("~/Scripts/toptabsnew.js"));
			Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), Guid.NewGuid().ToString(),
				String.Format("<link type='text/css' rel='stylesheet' href='{0}' />", ResolveClientUrl("~/styles/IbnFramework/windows.css")));
			Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), Guid.NewGuid().ToString(),
				String.Format("<link type='text/css' rel='stylesheet' href='{0}' />", ResolveClientUrl("~/styles/IbnFramework/Theme.css")));
			Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), Guid.NewGuid().ToString(),
				String.Format("<link type='text/css' rel='stylesheet' href='{0}' />", ResolveClientUrl("~/styles/IbnFramework/mcCalendClient.css")));
		}
		#endregion
	}
}