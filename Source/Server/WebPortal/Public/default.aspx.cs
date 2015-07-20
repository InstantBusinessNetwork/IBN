using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using Mediachase.Ibn;

namespace Mediachase.UI.Web.Public
{
	/// <summary>
	/// Summary description for _default.
	/// </summary>
	public partial class _default : System.Web.UI.Page
	{

		#region External Object Types
		public ObjectTypes ExternalObjectType
		{
			get
			{
				if (Request["ToDoId"] != null)
					return ObjectTypes.ToDo;
				else if (Request["TaskId"] != null)
					return ObjectTypes.Task;
				else if (Request["EventId"] != null)
					return ObjectTypes.CalendarEntry;
				else if (Request["IncidentId"] != null)
					return ObjectTypes.Issue;
				else if (Request["DocumentId"] != null)
					return ObjectTypes.Document;
				else throw new AccessDeniedException();
			}
		}

		public int ExternalObjectID
		{
			get
			{
				try
				{
					if (Request["ToDoId"] != null)
						return int.Parse(Request["ToDoId"]);
					else if (Request["TaskId"] != null)
						return int.Parse(Request["TaskId"]);
					else if (Request["EventId"] != null)
						return int.Parse(Request["EventId"]);
					else if (Request["IncidentId"] != null)
						return int.Parse(Request["IncidentId"]);
					else if (Request["DocumentId"] != null)
						return int.Parse(Request["DocumentId"]);
					else throw new AccessDeniedException();
				}
				catch
				{
					throw new AccessDeniedException();
				}
			}
		}
		#endregion

		#region External Query Properties
		public string ExternalLogin
		{
			get
			{
				return Request["ExternalLogin"];
			}
		}

		public string ExternalID
		{
			get
			{
				return Request["Guid"];
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			CheckExternal();

			HttpBrowserCapabilities br = Request.Browser;
			IFormatProvider culture = CultureInfo.InvariantCulture;
			int ver = br.MajorVersion;

			// O.R. [2008-12-05]: don't use winlogin if ticket was passed
			if (String.IsNullOrEmpty(Request["ticket"])	
				&& ((br.Browser.IndexOf("IE") >= 0 && ver >= 5) || br.Browser.IndexOf("Gecko") >= 0) 
				&& Security.CanLoginAD(Request.UserHostAddress))
			{
				if (Request.QueryString.Keys.Count > 0)
					Response.Redirect("~/Public/WinLogin.aspx?" + Request.QueryString.ToString());
				else
					Response.Redirect("~/Public/WinLogin.aspx");
			}
			else
			{
				string sDefUser = ConfigurationManager.AppSettings["defaultUser"];
				int iUserId = Mediachase.IBN.Business.User.GetUserByLogin(sDefUser);
				if(iUserId>0)
					LoginAsDefaultUser(iUserId, sDefUser);
				else
				{
					if (Request.QueryString.Keys.Count > 0)
						Response.Redirect("~/Public/WebLogin.aspx?" + Request.QueryString.ToString());
					else
						Response.Redirect("~/Public/WebLogin.aspx");
				}
			}
		}

		#region CheckExternal()
		private void CheckExternal()
		{
			if (ExternalLogin != null && ExternalID != null)
			{
				try
				{
					if (Mediachase.IBN.Business.Configuration.LicenseExpired)
						throw new Mediachase.Ibn.LicenseExpiredException();

					int EUserID = Security.UserLoginByExternalGate((int)ExternalObjectType, ExternalObjectID, ExternalID, ExternalLogin);
					string path = HttpRuntime.AppDomainAppVirtualPath;
					if (path == "/") path = "";
					SetAuthCookie(path + "/External", ExternalLogin, false, EUserID);
					HttpContext.Current.Items.Remove("userlight");
					UserLight userLight = UserLight.Load(EUserID);
					UserLightPropertyCollection pc = userLight.Properties;
					HttpContext context = HttpContext.Current;
					context.Items.Add("userlight", userLight);

					//Response.Redirect(Server.UrlDecode(Request["ReturnUrl"]));
					// O.R. [2008-04-15]
					Response.Redirect(Request["ReturnUrl"], true);
				}
				catch
				{
				}
			}
		}
		#endregion

		#region LoginAsDefaultUser
		private void LoginAsDefaultUser(int iUserId, string sDefUser)
		{
			SetAuthCookie(HttpRuntime.AppDomainAppVirtualPath,sDefUser,false, iUserId);
					
			UserLight userLight = UserLight.Load(iUserId);
			UserLightPropertyCollection pc =  userLight.Properties;
			HttpContext context = HttpContext.Current;
			if(context != null && context.Items.Contains("userlight"))
				context.Items.Remove("userlight");
			context.Items.Add("userlight", userLight);
					
			if (Request["ReturnUrl"] == null)
			{
				// not redirect 
				if (Security.IsUserInGroup(InternalSecureGroups.Administrator))
				{
					if (PortalConfig.PortalShowAdminWizard)
					{
						Response.Redirect("~/Workspace/default.aspx?BTab=Workspace&AdminWizard=1", true);
						return;
					}
				}

				Response.Redirect("~/Apps/Shell/Pages/default.aspx");
				return;
			}
			else
			{
				//Response.Redirect(Server.UrlDecode(Request["ReturnUrl"]));
				// O.R. [2008-04-15]
				Response.Redirect(Request["ReturnUrl"]);
				return;
			}
		}
		#endregion

		#region SetAuthCookie
		private void SetAuthCookie(string path, string login, bool IsPersistent, int iUserId)
		{
			FormsAuthenticationTicket tkt = new FormsAuthenticationTicket(1, login, DateTime.Now, DateTime.Now.AddDays(3), IsPersistent, "", path);
			HttpCookie ck = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(tkt));
			if (IsPersistent)
				ck.Expires = tkt.Expiration;

			ck.Path = tkt.CookiePath;
			Response.Cookies.Add(ck);
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
