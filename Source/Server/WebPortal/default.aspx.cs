using System;
using System.Data;
using System.Web;
using System.Web.Security;

using Mediachase.Ibn;
using Mediachase.Ibn.Web.UI;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.EMail;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.IBN.Web
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public partial class Default : System.Web.UI.Page
	{
		protected string sTitle = String.Format("{0} {1}", IbnConst.ProductFamily, IbnConst.VersionMajorDotMinor);
		protected void Page_Load(object sender, System.EventArgs e)
		{
			iconIBN.Attributes.Add("href", ResolveUrl("~/portal.ico"));

			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			if (!PortalConfig.SystemIsActive)
			{
				lblText.InnerHtml = String.Format(CHelper.GetResFileString(GlobalResourceManager.Strings["BottomsTextResourceKey"]), Mediachase.Ibn.IbnConst.FullVersion);
			}
			else
			{
				if (!IsPostBack)
				{
					if (Request["sid"] != null)
						ProcessSid();
					else if (Request["outlookticketid"] != null)
						ProcessOutlook();
					else if (Request["login"] != null && Request["password"] != null)
						ProcessLogin();
					else if (Request.IsAuthenticated)
						ProcessRedirect();
				}

				Response.Redirect("Public/default.aspx", true);
			}
		}

		#region ProcessSid
		private void ProcessSid()
		{
			try
			{
				string sSID = Request["sid"];
				int iUserId = Root.CheckActiveUser(sSID);
				if (iUserId <= 0)
					throw new Exception();

				string sUser = string.Empty;

				using (IDataReader reader = Root.GetUserInfo(iUserId, 1))
				{
					if (reader.Read())
						sUser = reader["login"].ToString();
				}

				int realId = 0;
				using (IDataReader reader = Mediachase.IBN.Business.User.GetUserInfoByOriginalId(iUserId))
				{
					if (reader.Read())
						realId = (int)reader["UserId"];
				}
				SetAuthCookie(sUser, HttpRuntime.AppDomainAppVirtualPath, false);
				Report.LogPortalLogin(realId, Request.UserHostAddress);

				ProcessRedirect();
			}
			catch { }
		}
		#endregion

		private void ProcessRedirect()
		{
			string sRedirectString = String.Empty;
			string sTempString = "";
			string _right_eq = "right=";
			if (Request["redirect"] != null)
			{
				sTempString = Request["redirect"].ToString();
				string last = "";
				string first = sTempString;
				if (sTempString.IndexOf(_right_eq) >= 0)
				{
					last = sTempString.Substring(sTempString.IndexOf(_right_eq) + _right_eq.Length);
					last = Server.UrlEncode(last);
					first = sTempString.Substring(0, sTempString.IndexOf(_right_eq) + _right_eq.Length);
				}
				sRedirectString += "/" + first + last;
			}

			if (String.IsNullOrEmpty(sRedirectString))
				sRedirectString = "Apps/Shell/Pages/default.aspx";

			Response.Redirect(sRedirectString, true);
		}

		#region ProcessOutlook
		private void ProcessOutlook()
		{
			try
			{
				string ticketUid = Request["outlookticketid"].ToString();

				IncidentUserTicket iut = IncidentUserTicket.Load(new Guid(ticketUid));
				int iUserId = iut.UserId;
				int iIncidentId = iut.IncidentId;

				if (iUserId <= 0 || iIncidentId <= 0)
					throw new Exception();

				string sUser = string.Empty;

				using (IDataReader reader = Mediachase.IBN.Business.User.GetUserInfo(iUserId))
				{
					if (reader.Read())
						sUser = reader["login"].ToString();
				}

				SetAuthCookie(sUser, HttpRuntime.AppDomainAppVirtualPath, false);
				Report.LogPortalLogin(iUserId, Request.UserHostAddress);

				string sRedirectString = String.Empty;
				if (Request["type"] != null && Request["type"] == "Incident")
					sRedirectString += "/Incidents/IncidentView.aspx?IncidentId=" + iIncidentId;
				else if (Request["type"] != null && Request["type"] == "Client")
					sRedirectString += "/Incidents/IncidentView.aspx?IncidentId=" + iIncidentId + "&Tab=General&View=Client";
				Response.Redirect(sRedirectString, true);
			}
			catch { }
		}
		#endregion

		#region ProcessLogin
		private void ProcessLogin()
		{
			string password = Request["password"];
			string fulllogin = Server.UrlDecode(Request["login"]);
			string login = String.Empty;

			if (fulllogin.IndexOf("@") > 0)
			{
				int index = fulllogin.IndexOf("@");
				login = fulllogin.Substring(0, index);
				string path = string.Format("/Public/WebLogin.aspx?login={0}&password={1}&redirect={2}",
					login, password, Request["redirect"]);
				Response.Redirect(path);
			}
		}
		#endregion


		#region SetAuthCookie
		private void SetAuthCookie(string sUser, string path, bool IsPersistent)
		{
			FormsAuthenticationTicket tkt = new FormsAuthenticationTicket(1, sUser, DateTime.Now, DateTime.Now.AddDays(3), IsPersistent, "", path);
			HttpCookie ck = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(tkt));
			if (IsPersistent)
				ck.Expires = tkt.Expiration;
			ck.Name = "IBNAUTH";
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
