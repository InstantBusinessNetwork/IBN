using System;
using System.Data;
using System.Web;
using System.Web.Security;

using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Public
{
	public partial class WinLogin : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			string redirectUrl = "~/Public/WebLogin.aspx?" + Request.QueryString.ToString();

			string windowsUserName = Request.ServerVariables["LOGON_USER"];
			if (!string.IsNullOrEmpty(windowsUserName)) // Don't search for empty user name.
			{
				int userId = Security.UserLoginWindows(windowsUserName);
				if (userId > 0)
				{
					string ibnUserName = GetIbnUserName(userId);
					SetAuthCookie(HttpRuntime.AppDomainAppVirtualPath, ibnUserName);

					if (Request["ReturnUrl"] == null)
					{
						UserLight userLight = UserLight.Load(userId);
						UserLightPropertyCollection pc = userLight.Properties;
						HttpContext context = HttpContext.Current;
						if (context != null && context.Items.Contains("userlight"))
							context.Items.Remove("userlight");

						context.Items.Add("userlight", userLight);

						if (Security.IsUserInGroup(InternalSecureGroups.Administrator) && PortalConfig.PortalShowAdminWizard)
							redirectUrl = "~/Workspace/default.aspx?BTab=Workspace&AdminWizard=1";
						else
							redirectUrl = "~/Apps/Shell/Pages/default.aspx";
					}
					else
						redirectUrl = Request["ReturnUrl"];
				}
			}

			Response.Redirect(redirectUrl, true);
		}

		#region private void SetAuthCookie(string path, string ibnUserName)
		private void SetAuthCookie(string path, string ibnUserName)
		{
			FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, ibnUserName, DateTime.Now, DateTime.Now.AddDays(3), false, string.Empty, path);
			HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
			cookie.Path = ticket.CookiePath;
			Response.Cookies.Add(cookie);
		}
		#endregion

		#region private string GetIbnUserName(int userId)
		private string GetIbnUserName(int userId)
		{
			string result = string.Empty;

			using (IDataReader reader = Mediachase.IBN.Business.User.GetUserInfo(userId))
			{
				if (reader.Read())
					result = reader["Login"].ToString();
			}

			return result;
		}
		#endregion

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
