namespace Mediachase.UI.Web.Public.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Web.Security;
	using Mediachase.IBN.Business;
	using System.Resources;
	using Mediachase.Ibn;

	/// <summary>
	///		Summary description for Login.
	/// </summary>
	public partial class Login : System.Web.UI.UserControl
	{

		public ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Public.Resources.strDownload", typeof(Login).Assembly);


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

		#region Regular Query Properties
		public string RegularLogin
		{
			get
			{
				return Request["login"];
			}
		}

		public string RegularPassword
		{
			get
			{
				return Request["password"];
			}
		}

		public string RegularTicket
		{
			get
			{
				return Request["ticket"];
			}
		}

		public string RegularRedirect
		{
			get
			{
				return Request["redirect"];
			}
		}
		#endregion

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

		#region Page_Load(object sender, System.EventArgs e)
		protected void Page_Load(object sender, System.EventArgs e)
		{
			bool isActive = PortalConfig.SystemIsActive;
			string domain = PortalConfig.SystemHost;

			if (!isActive)
			{
				string schema = (Request.IsSecureConnection) ? "https://" : "http://";
				Response.Redirect(schema + domain);
				return;
			}

			lblPortal.Text = "@" + domain;
			HttpContext.Current.Items.Remove("userlight");
			ApplyLocalization();

			CheckExternal();
			CheckTicketLogin();
			CheckQueryLogin();
			/*if (!Page.IsPostBack)
			{
				Page.RegisterStartupScript("maximize","<script language='javascript'>try {"+
					"var offset=(navigator.userAgent.indexOf(\"Mac\")!=-1 || navigator.userAgent.indexOf(\"Gecko\")!=-1 ||"+
					"navigator.appName.indexOf(\"Netscape\")!=-1)? 0 : 4;"+
					"window.moveTo(-offset,-offset);window.resizeTo(screen.availWidth+(2*offset),screen.availHeight+(2*offset));}catch (e){}</script>");
			}*/
			if (!IsPostBack)
				BindData();

			Page.ClientScript.RegisterOnSubmitStatement(this.GetType(), "GetDate", "var date = new Date();" +
				"document.forms[0]." + hdnOffset.ClientID + ".value = date.getTimezoneOffset();");

			lblTitle1.Text = PortalConfig.PortalHomepageTitle1;
			lblTitle2.Text = PortalConfig.PortalHomepageTitle2;
			lblHPText1.Text = HttpUtility.HtmlDecode(PortalConfig.PortalHomepageText1);
			lblHPText2.Text = HttpUtility.HtmlDecode(PortalConfig.PortalHomepageText2);

			//cvInvalidLogin.ErrorMessage = LocRM.GetString("InvalidLogin");
			tbLogin.Attributes.Add("onkeydown", "mcInnerValidationClear();");
			tbPassword.Attributes.Add("onkeydown", "mcInnerValidationClear();");
		}
		#endregion

		#region ApplyLocalization()
		private void ApplyLocalization()
		{
			lblScreenText.Text = String.Format(LocRM.GetString("DownloadTextSC"), IbnConst.ProductFamilyShort);
			hlDownload.Text = String.Format(LocRM.GetString("tDownloadIBNClient"), IbnConst.ProductFamilyShort);
			hlToolBox.Text = String.Format(LocRM.GetString("tDownloadToolBox"), IbnConst.ProductFamilyShort);
			lbForgotPass.Text = LocRM.GetString("tForgotPassword");
			hlScreenCapture.Text = LocRM.GetString("tDownloadScreenCapture");
			hlPluginIE.Text = LocRM.GetString("tDownloadPlugIE");
			hlPluginFF.Text = LocRM.GetString("tDownloadPlugFF");
			hlPluginOut.Text = LocRM.GetString("tDownloadPlugOut");
			Hyperlink1.Text = LocRM.GetString("tDownloadNow");
			cbRemember.Text = LocRM.GetString("tRemember");
			btnLogin.Text = LocRM.GetString("tLogin");
		}
		#endregion

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

					UserLight userLight = UserLight.Load(EUserID);
					UserLightPropertyCollection pc = userLight.Properties;
					HttpContext context = HttpContext.Current;
					context.Items.Add("userlight", userLight);

					//Response.Redirect(Server.UrlDecode(Request["ReturnUrl"]));
					// O.R. [2008-04-15]
					Response.Redirect(Request["ReturnUrl"]);
				}
				catch
				{
				}
			}
		}
		#endregion

		#region CheckQueryLogin()
		private void CheckQueryLogin()
		{
			if (RegularLogin != null && RegularPassword != null && RegularRedirect != null)
			{
				try
				{
					int iUserId = Security.UserLogin(RegularLogin, RegularPassword);

					if (Mediachase.IBN.Business.Configuration.LicenseExpired)
						throw new Mediachase.Ibn.LicenseExpiredException();

					// !! Authorisation cookie
					SetAuthCookie(HttpRuntime.AppDomainAppVirtualPath, RegularLogin, false, iUserId);
					// not rederect 
					UserLight userLight = UserLight.Load(iUserId);
					UserLightPropertyCollection pc = userLight.Properties;
					HttpContext context = HttpContext.Current;
					context.Items.Add("userlight", userLight);

					//Response.Redirect("~/" + Server.UrlDecode(RegularRedirect));
					// O.R. [2008-04-15]
					Response.Redirect("~/" + RegularRedirect);
				}
				catch (InvalidAccountException)
				{
					Response.Redirect("~/Public/default.aspx");
				}
				catch (InvalidPasswordException)
				{
					Response.Redirect("~/Public/default.aspx");
				}
				catch (NotActiveAccountException)
				{
					Response.Redirect("~/Public/default.aspx");
				}
				catch (ExternalOrPendingAccountException)
				{
					Response.Redirect("~/Public/default.aspx");
				}
				catch (Mediachase.Ibn.LicenseExpiredException)
				{
					Response.Redirect("~/Public/default.aspx");
				}
			}
		}
		#endregion

		#region CheckTicketLogin()
		private void CheckTicketLogin()
		{
			if (!String.IsNullOrEmpty(RegularLogin) && !String.IsNullOrEmpty(RegularTicket) && !String.IsNullOrEmpty(RegularRedirect))
			{
				try
				{
					int iUserId = Security.UserLoginByTicket(RegularLogin, new Guid(RegularTicket));

					if (Mediachase.IBN.Business.Configuration.LicenseExpired)
						throw new Mediachase.Ibn.LicenseExpiredException();

					// !! Authorisation cookie
					SetAuthCookie(HttpRuntime.AppDomainAppVirtualPath, RegularLogin, false, iUserId);
					// not rederect 
					UserLight userLight = UserLight.Load(iUserId);
					UserLightPropertyCollection pc = userLight.Properties;
					HttpContext context = HttpContext.Current;
					context.Items.Add("userlight", userLight);

					Response.Redirect("~/" + RegularRedirect);
				}
				catch (InvalidAccountException)
				{
					Response.Redirect("~/Public/default.aspx");
				}
				catch (InvalidTicketException)
				{
					Response.Redirect("~/Public/default.aspx");
				}
				catch (NotActiveAccountException)
				{
					Response.Redirect("~/Public/default.aspx");
				}
				catch (ExternalOrPendingAccountException)
				{
					Response.Redirect("~/Public/default.aspx");
				}
				catch (Mediachase.Ibn.LicenseExpiredException)
				{
					Response.Redirect("~/Public/default.aspx");
				}
			}
		}
		#endregion

		#region Data Binding
		private void BindData()
		{
			BindCookie();
			BindTitles();
		}

		private void BindTitles()
		{
			lblTitle1.Text = "Title 1";
			lblTitle2.Text = "Title 2";
			lblHPText1.Text = "Text 1";
			lblHPText2.Text = "Text 2";
		}


		private void BindCookie()
		{
			tbLogin.Text = "";
			cbRemember.Checked = false;
			tbPassword.Text = "";
			if (Request.Cookies["login"] != null && Request.Cookies["Remember"] != null && bool.Parse(Request.Cookies["Remember"].Value))
				tbLogin.Text = Request.Cookies["login"].Value;

			if (Request.Cookies["Remember"] != null)
				cbRemember.Checked = bool.Parse(Request.Cookies["Remember"].Value);
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region Form Login
		protected void btnLogin_Click(object sender, System.EventArgs e)
		{
			string LoginString = tbLogin.Text;
			string PasswordString = tbPassword.Text;
			int ClientTimeZoneOffset = int.Parse(hdnOffset.Value);

			if (LoginString.IndexOf("@") >= 0)
				LoginString = LoginString.Substring(0, LoginString.IndexOf("@"));

			try
			{
				if (Mediachase.IBN.Business.Configuration.LicenseExpired)
					throw new Mediachase.Ibn.LicenseExpiredException();
				int iUserId = Security.UserLogin(LoginString, PasswordString);
				if (cbRemember.Checked)
				{
					HttpCookie loginCookie = new HttpCookie("login", tbLogin.Text);
					loginCookie.Expires = DateTime.Now.AddMonths(1);
					loginCookie.Path = HttpRuntime.AppDomainAppVirtualPath;
					Response.Cookies.Add(loginCookie);
				}
				else
					if (Response.Cookies["login"] != null)
					{
						HttpCookie cookie = Response.Cookies["login"];
						cookie.Expires = DateTime.Now.AddYears(-10);
					}

				HttpCookie cbCookie = new HttpCookie("Remember", cbRemember.Checked.ToString());
				cbCookie.Path = HttpRuntime.AppDomainAppVirtualPath;
				cbCookie.Expires = DateTime.Now.AddMonths(1);
				Response.Cookies.Add(cbCookie);

				int TimeZoneOffset = 0;
				int TimeZoneOffsetLatest = 0;

				using (IDataReader rdr = User.GetUserPreferences(iUserId))
				{
					if (rdr.Read())
					{
						TimeZoneOffset = User.GetCurrentBias((int)rdr["TimeZoneId"]);
						TimeZoneOffsetLatest = (int)rdr["TimeOffsetLatest"];
					}
				}

				// !! Authorisation cookie
				SetAuthCookie(HttpRuntime.AppDomainAppVirtualPath, LoginString, false, iUserId);

				if (Request["ReturnUrl"] == null 
					|| Request["ReturnUrl"] == "/" 
					|| Request["ReturnUrl"].ToLower() == HttpRuntime.AppDomainAppVirtualPath.ToLower()
					|| Request["ReturnUrl"].ToLower() == HttpRuntime.AppDomainAppVirtualPath.ToLower() + "/")
				{
					// not rederect 
					UserLight userLight = UserLight.Load(iUserId);
					UserLightPropertyCollection pc = userLight.Properties;
					HttpContext context = HttpContext.Current;
					context.Items.Add("userlight", userLight);

					if (Security.IsUserInGroup(InternalSecureGroups.Administrator) && PortalConfig.PortalShowAdminWizard)
					{
						Response.Redirect("~/Workspace/default.aspx?BTab=Workspace&AdminWizard=1", true);
						return;
					}

					// check for startup wizard
					/*if (pc["USetup_ShowStartupWizard"]==null || pc["USetup_ShowStartupWizard"]=="True")
					{
						// Show Startup Wizard
						if (!Security.IsUserInGroup(InternalSecureGroups.Administrator))
							Response.Redirect("~/Workspace/Default.aspx?BTab=Workspace&BTab=Workspace&wizard=1", true);
					}*/

					// check for time zone					
					if (TimeZoneOffset != ClientTimeZoneOffset && TimeZoneOffsetLatest != ClientTimeZoneOffset)
					{
						//Response.Redirect("~/Directory/ChangeZone.aspx?TimeOffset=" + ClientTimeZoneOffset + "&ReturnUrl=" + Server.UrlEncode("~/Workspace/default.aspx?BTab=Workspace"), true);
						// O.R. [2008-04-15]
						Response.Redirect("~/Directory/ChangeZone.aspx?TimeOffset=" + ClientTimeZoneOffset, true);
						return;
					}

					Response.Redirect("~/Apps/Shell/Pages/default.aspx");
					return;
				}
				else
				{
					if (TimeZoneOffset != ClientTimeZoneOffset && TimeZoneOffsetLatest != ClientTimeZoneOffset)
					{
						Response.Redirect("~/Directory/ChangeZone.aspx?TimeOffset=" + ClientTimeZoneOffset + "&ReturnUrl=" + Server.UrlEncode(Request["ReturnUrl"]), true);
					}
					else
					{
						//Response.Redirect(Server.UrlDecode(Request["ReturnUrl"]));
						// O.R. [2008-04-15]
						Response.Redirect(Request["ReturnUrl"]);
					}
					return;
				}
			}
			catch (Exception ex)
			{
				if (ex is InvalidAccountException
					|| ex is InvalidPasswordException
					|| ex is NotActiveAccountException
					|| ex is ExternalOrPendingAccountException)
					lblPassword.Text = LocRM.GetString("InvalidLoginPassword");
				else if (ex is Mediachase.Ibn.LicenseExpiredException)
					lblPassword.Text = LocRM.GetString("tLicExpired");
				else throw ex;

				//cvInvalidLogin.IsValid = false;
				//if (ex is InvalidAccountException
				//    || ex is InvalidPasswordException
				//    || ex is NotActiveAccountException
				//    || ex is ExternalOrPendingAccountException)
				//    cvInvalidLogin.ErrorMessage = LocRM.GetString("InvalidLoginPassword");
				//else if (ex is Mediachase.Ibn.LicenseExpiredException)
				//    cvInvalidLogin.ErrorMessage = LocRM.GetString("tLicExpired");
				//else throw ex;
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
			Report.LogPortalLogin(iUserId, Request.UserHostAddress);
		}
		#endregion

		private void Page_PreRender(object sender, EventArgs e)
		{
			if (tbLogin.Text == "")
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
							"<script type=\"text/javascript\">" +
							"setTimeout(\"LoginFocusElement('" + tbLogin.ClientID + "')\", 0);</script>");
			else
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
							"<script type=\"text/javascript\">" +
							"setTimeout(\"LoginFocusElement('" + tbPassword.ClientID + "')\", 0);</script>");
		}
	}
}
