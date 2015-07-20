using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;

using Mediachase.IBN.Business;

namespace Mediachase.UI.Calendar.Web
{
	/// <summary>
	/// Summary description for Logoff.
	/// </summary>
	public partial class Logoff : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{

			string path = HttpRuntime.AppDomainAppVirtualPath;

			// Check Securty Zones
			if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
				path = Request.Cookies[FormsAuthentication.FormsCookieName].Path;
			
			// Remove Old Auth Cookie
			if (Response.Cookies[FormsAuthentication.FormsCookieName]!=null)
					Response.Cookies.Remove(FormsAuthentication.FormsCookieName);

			HttpCookie ck = new HttpCookie(FormsAuthentication.FormsCookieName,String.Empty);
			ck.Expires = DateTime.Now.AddMonths(-1);
			ck.Path = path;
			Response.Cookies.Add(ck);

			Security.SignOut();
			// Log off ASP Session
			Response.Redirect("Public/WebLogin.aspx");
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}
