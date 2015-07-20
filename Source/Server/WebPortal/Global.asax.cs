using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.SessionState;

using Mediachase.Ibn;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Business.Configuration;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Data.Meta;

using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Modules;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Assignments;

namespace Mediachase.IBN.Web
{
	public class Global : System.Web.HttpApplication
	{
		public Global()
		{
		}

		#region protected void Application_Start(Object sender, EventArgs e)
		protected void Application_Start(Object sender, EventArgs e)
		{
			InitializeGlobalContext();

			// Subscribe to transaction events
			DatabaseTransactionBridge.Init();
			Mediachase.Ibn.Lists.AlertManager.Init();

			// Code that runs on application startup
			Application["ComponentArtWebUI_AppKey"] = "This edition of ComponentArt Web.UI is licensed for Instant Business Network application only.";

			//InitializeDatabase();

			ControlPathResolver current = new ControlPathResolver();
			current.Init(new string[] {
				"~/Apps/MetaDataBase/Primitives/"
				, "~/Apps/MetaUI/Primitives/"
				, "~/Apps/MetaUIEntity/Primitives/"
				, "~/Apps/Security/Primitives/"
				, "~/Apps/BusinessProcess/Primitives/"
				, "~/Apps/TimeTracking/Primitives/"
				, "~/Apps/ListApp/Primitives/"
				, "~/Apps/ClientManagement/Primitives/"
				, "~/Apps/DocumentManagement/Primitives/"
				, "~/Apps/ReportManagement/Primitives/"
				, "~/Apps/IbnDirectory/Primitives/"
				, "~/Apps/Calendar/Primitives/"
				, "~/Apps/Administration/Primitives/"
				, "~/Apps/WidgetEngine/Primitives/"
			});
			ControlPathResolver.Current = current;

			McScriptLoader mcLoader = new McScriptLoader();
			mcLoader.Init();
			McScriptLoader.Current = mcLoader;


			Mediachase.Ibn.Data.Services.Security.PrincipalGroupsResolving = Mediachase.IBN.Business.User.GetListSecureGroupAllArray;

			Mediachase.IbnNext.TimeTracking.TimeTrackingManager.Init();

			ListViewProfile.Init();

			FormController.Init();
		}
		#endregion

		#region protected void Application_BeginRequest(Object sender, EventArgs e)
		protected void Application_BeginRequest(Object sender, EventArgs e)
		{
			GetCultureFromRequest();
			InitializeGlobalContext();
			GlobalResourceManager.Initialize(HostingEnvironment.MapPath("~/App_GlobalResources/GlobalResources.xml"));

			string path = Request.Path;

			#region Remove /portals/ from query
			string constOldUrl = "/portals/";
			if (path.IndexOf(constOldUrl, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				string fullPath = Request.RawUrl;
				int index = fullPath.IndexOf(constOldUrl, StringComparison.OrdinalIgnoreCase);
				string begin = fullPath.Substring(0, index);
				string end = fullPath.Substring(index + constOldUrl.Length);
				int endOfDomain = end.IndexOf('/');
				if (endOfDomain >= 0)
					end = end.Substring(endOfDomain + 1);
				path = begin + '/' + end;

				//OZ: RewritePath чтобы работали старые клиентские инструменты
				//AK: 2009-01-26 - exclude css
				if (path.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
					Response.Redirect(path, true);
				else
					HttpContext.Current.RewritePath(path);
			}
			#endregion

			bool pathContainsFiles = (path.IndexOf("/files/", StringComparison.OrdinalIgnoreCase) >= 0);
			bool pathContainsWebDav = (path.IndexOf("/webdav/", StringComparison.OrdinalIgnoreCase) >= 0);

			if (!pathContainsFiles && !pathContainsWebDav
					&& (path.EndsWith("error.aspx", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith(".css", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith(".html", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith("webresource.axd", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith("scriptresource.axd", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith("licenseexpired.aspx", StringComparison.OrdinalIgnoreCase)
					)
				)
				return;


			//Обработка файлов которые подвергаются кэшированию
			// TODO: перенести список строк и время жизни кеша в web.config
			if (!pathContainsFiles && !pathContainsWebDav
				&& !path.EndsWith("Reserved.ReportViewerWebControl.axd", StringComparison.OrdinalIgnoreCase)
				&& (path.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
					|| path.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)
					|| path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
					|| path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
					|| path.EndsWith(".axd", StringComparison.OrdinalIgnoreCase)
					)
				)
			{
				HttpCachePolicy cache = HttpContext.Current.Response.Cache;
				//HttpContext.Current.Response.AddFileDependency(Server.MapPath(path));
				bool _hanldeFlag = true;

				//Вид кэширования (включает возможность кэширования на прокси)
				cache.SetCacheability(HttpCacheability.Public);

				//кэширование по параметрам в QueryString (d, versionUid)
				//все запросы включающие любой из этих параметров будут кешироваться по значению параметра
				cache.VaryByParams["d"] = true;
				cache.VaryByParams["versionUid"] = true;
				cache.SetOmitVaryStar(true);

				//устанавливаем срок годности закэшированого файла
				//Можно сделать для разных типов фалов - разные сроки хранения
				double cacheExpires = 1;
				if (System.Configuration.ConfigurationManager.AppSettings["ClientCache"] != null)
					cacheExpires = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["ClientCache"], CultureInfo.InvariantCulture);
				cache.SetExpires(DateTime.Now + TimeSpan.FromMinutes(cacheExpires));
				//cache.SetMaxAge(TimeSpan.FromSeconds(259200));


				//разрешаем хранить кэш на диске
				cache.SetAllowResponseInBrowserHistory(true);
				cache.SetValidUntilExpires(true);

#if (DEBUG)
				cache.SetExpires(DateTime.Now);
				cache.SetAllowResponseInBrowserHistory(false);
#endif

				DateTime dtRequest = DateTime.MinValue;

#if (!DEBUG)
				//проверка даты модификации файла
				if (File.Exists(Server.MapPath(path)))
				{
					cache.SetLastModified(File.GetLastWriteTime(Server.MapPath(path)).ToUniversalTime());

					//Не удалять(!) Включает режим более строгово кеширования
					//Кэшеирует файлы даже после рестарта IIS, вернусь после отпуска протестирую и включу (dvs)

					if (HttpContext.Current.Request.Headers["If-Modified-Since"] != null)
					{
						try
						{
							dtRequest = Convert.ToDateTime(HttpContext.Current.Request.Headers["If-Modified-Since"], CultureInfo.InvariantCulture);
						}
						catch
						{
						}

						//если файл существует и его дата модификации совпадает с версией на клиенте то возвращаем 304, в противном случае
						//обрабатывать данный запрос будет дефолтный хэндлер ASP.NET (подробнее см. System.Web.Cachig.OutputCacheModule)						
						if (File.GetLastWriteTime(Server.MapPath(path)).ToUniversalTime().ToString("r") == dtRequest.ToUniversalTime().ToString("r"))
						{
							//Если отладка загрузки скриптов включена, то не кэшируем их
							if ((path.EndsWith(".js", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".axd", StringComparison.OrdinalIgnoreCase)) && Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogSriptLoading"], CultureInfo.InvariantCulture))
							{
								cache.SetExpires(DateTime.Now);
							}
							else
							{
								Response.ClearContent();
								Response.StatusCode = 304;
								_hanldeFlag = false;
							}
						}
					}
				}
#endif

				if (_hanldeFlag)
					return;
			}
			else
			{
				//25.02.2009 et: Не выполнять проверку для WebDav запросов
				if (!pathContainsFiles && !pathContainsWebDav)
				{
					if (path.IndexOf('\\') >= 0 || System.IO.Path.GetFullPath(Request.PhysicalPath) != Request.PhysicalPath)
						throw new HttpException(404, "not found");
				}

				InitializeDatabase(); // Terminates request if error occurs.

				//AK 2009-01-16
				if (!PortalConfig.SystemIsActive)
				{
					if (Request.AppRelativeCurrentExecutionFilePath.Equals("~/default.aspx", StringComparison.OrdinalIgnoreCase))
						return;
					else
						Response.Redirect("~/default.aspx", true);
				}

				//Init TemplateResolver
				TemplateResolver.Current = new TemplateResolver();

				TemplateResolver.Current.AddSource("QueryString", new TemplateSource(HttpContext.Current.Request.QueryString));

				if (HttpContext.Current.Session != null)
					TemplateResolver.Current.AddSource("Session", new TemplateSource(HttpContext.Current.Session));

				TemplateResolver.Current.AddSource("HttpContext", new TemplateSource(HttpContext.Current.Items));
				TemplateResolver.Current.AddSource("DataContext", new TemplateSource(DataContext.Current.Attributes));

				TemplateResolver.Current.AddSource("DateTime", new DateTimeTemplateSource());
				TemplateResolver.Current.AddSource("Security", new Mediachase.Ibn.Data.Services.SecurityTemplateSource());

				TemplateResolver.Current.AddSource("TimeTrackingSecurity", new Mediachase.IbnNext.TimeTracking.TimeTrackingSecurityTemplateSource());

				//Init PathTemplateResolver
				PathTemplateResolver.Current = new PathTemplateResolver();

				PathTemplateResolver.Current.AddSource("QueryString", new PathTemplateSource(HttpContext.Current.Request.QueryString));

				if (HttpContext.Current.Session != null)
					PathTemplateResolver.Current.AddSource("Session", new PathTemplateSource(HttpContext.Current.Session));

				PathTemplateResolver.Current.AddSource("HttpContext", new PathTemplateSource(HttpContext.Current.Items));
				PathTemplateResolver.Current.AddSource("DataContext", new PathTemplateSource(DataContext.Current.Attributes));

				PathTemplateResolver.Current.AddSource("DateTime", new Mediachase.Ibn.Web.UI.Controls.Util.DateTimePathTemplateSource());
				PathTemplateResolver.Current.AddSource("Security", new Mediachase.Ibn.Web.UI.Controls.Util.SecurityPathTemplateSource());

				//PathTemplateResolver.Current.AddSource("TimeTrackingSecurity", new Mediachase.IbnNext.TimeTracking.TimeTrackingSecurityTemplateSource());

				// O.R. [2009-07-28]: Check license and .NET Framework version
				if (Configuration.WorkflowModule && WorkflowActivityWrapper.IsFramework35Installed())
					GlobalWorkflowRuntime.StartRuntime(DataContext.Current.SqlContext.ConnectionString);
			}
		}
		#endregion

		#region protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{
			GetCultureFromRequest();
			InitializeGlobalContext();

			string path = Request.Path;
			if (path.IndexOf("/files/", StringComparison.OrdinalIgnoreCase) < 0 && path.IndexOf("/webdav/", StringComparison.OrdinalIgnoreCase) < 0
					&& (path.EndsWith("error.aspx", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith("userstatusimage.aspx", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith("contenticon.aspx", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith("companylogo.aspx", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith("grouplogo.aspx", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith(".css", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith(".html", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith("webresource.axd", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith("scriptresource.axd", StringComparison.OrdinalIgnoreCase)
						|| path.EndsWith("licenseexpired.aspx", StringComparison.OrdinalIgnoreCase)
					)
				)
				return;

			InitializeDatabase(); // Terminates request if error occurs.

			if (Request.IsAuthenticated)
			{
				BindRoles();

				UserLight usr = null;
				try
				{
					usr = Mediachase.IBN.Business.Security.CurrentUser;
				}
				catch
				{
				}

				if (usr != null)
				{
					string cultureName = Mediachase.IBN.Business.Security.CurrentUser.Culture;
					SetCurrentCulture(cultureName);
					SaveCultureToCookie(cultureName);

					// Init Security Context
					Mediachase.Ibn.Data.Services.Security.CurrentUserId = Mediachase.IBN.Business.Security.CurrentUser.UserID;

					// Init  Current Time Zone
					DataContext.Current.CurrentUserTimeZone = Mediachase.IBN.Business.Security.CurrentUser.CurrentTimeZone;

					// Init GlobalContext CurrentUser
					GlobalContext.Current.CurrentUser = new Mediachase.Ibn.UserInfo(usr.UserID,
						usr.Login, usr.FirstName, usr.LastName, usr.DisplayName, usr.Email, usr.LanguageId,
						"",
						usr.TimeZoneId, null, usr.IsAlertService, usr.IsExternal, usr.IsPending);

					//Mediachase.Ibn.Data.Services.Security.CurrentUserId = Mediachase.IBN.Business.Security.CurrentUser.UserID;
				}
			}
		}
		#endregion

		#region protected void Application_Error(Object sender, EventArgs e)
		protected void Application_Error(Object sender, EventArgs e)
		{
			Exception ex = Server.GetLastError().GetBaseException();
			if (ex == null)
				ex = Server.GetLastError();

			if (ex.Message.IndexOf("unable to load viewstate file:", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				Server.ClearError();
				Response.Redirect(Request.RawUrl);
			}

			// 2007-10-19 Oleg Zhuk: Ignore WebDav Folder Error 
			#region More Info MC_SUPPORT-002453-17558
			/*
				Какой-то MS Word пытается прочитать настройки папки, чтобы потом запросить её содержимое.
			2007-10-18 07:03:02 W3SVC1914035998 217.107.47.23 OPTIONS /portals/project_dialog-it_ru/webdav/NzMyOTY2LzI1LzMzNy81MDczNA== - 80 - 84.237.124.10

				Мы это не поддерживаем, поэтому настроили ASP.NET так чтобы для папки он выкидывал ошибку и 
			MS Word больше не пытался работать с папкой.
				У нас встроен наш перехватчки необработанных исключений, который перехватывает исключение и 
			пишет эту "ошибку" в лог.

				Поэтому, то что мы в MS Word возвращаем ошибку это правильно, скорее всего в перехватчике 
			исключений надо будет сделать игнорирование подобных ошибок, чтобы они не путались под ногами. 
			 */
			#endregion
			if (Request.HttpMethod == "OPTIONS")
				return;

			string redirectLink = null;
			switch (ex.GetType().ToString())
			{
				case "Mediachase.Ibn.LicenseExpiredException":
					redirectLink = "~/Public/LicenseExpired.aspx";
					break;
				case "Mediachase.Ibn.AccessDeniedException":
				case "Mediachase.Ibn.Core.AccessDeniedException":
					// TODO: Save exception to log.
					redirectLink = "~/Common/notexistingId.aspx?AD=1";
					break;
				case "System.Net.Sockets.SocketException":
					redirectLink = "~/Common/notexistingId.aspx?SMTP=Socket";
					break;
				case "Mediachase.IBN.Business.EMail.SmtpClientException":
					CHelper.GenerateErrorReport(ex);
					redirectLink = String.Format(CultureInfo.InvariantCulture
						, "~/Common/notexistingId.aspx?SMTP=Client&mess={0}"
						, HttpUtility.UrlEncode(ex.Message));
					break;
				case "System.NullReferenceException":
				default:
					if (ex is HttpException && ((HttpException)ex).ErrorCode == 403) // Forbidden
						redirectLink = "~/Common/notexistingId.aspx?AD=1";

					break;
			}

			if (!string.IsNullOrEmpty(redirectLink))
			{
				Server.ClearError();
				Response.Redirect(redirectLink, true);
				return;
			}

			string errorid = CHelper.GenerateErrorReport(ex);

#if (!DEBUG)
			Server.ClearError();
			HttpContext.Current.Response.Redirect("~/Public/Error.aspx?ErrorId="+ errorid);
#endif
		}
		#endregion

		#region protected void Application_End(Object sender, EventArgs e)
		protected void Application_End(Object sender, EventArgs e)
		{
			// O.R. [2009-07-28]: Check license and NET Framework version
			if (Configuration.WorkflowModule && WorkflowActivityWrapper.IsFramework35Installed())
				GlobalWorkflowRuntime.StopRuntime();

			Configuration.Uninitialize();
		}
		#endregion


		#region private void InitializeGlobalContext()
		private void InitializeGlobalContext()
		{
			string modulesVirtualPath = "~/Apps/";
			string modulesDirectoryPath = HostingEnvironment.MapPath(modulesVirtualPath);

			GlobalContext context = GlobalContext.Current;
			if (context == null)
			{
				context = new GlobalContext(null);
				GlobalContext.Current = context;
			}

			context.ModulesDirectoryPath = modulesDirectoryPath;
			context.ModulesVirtualPath = modulesVirtualPath;
		}
		#endregion

		#region private void BindRoles()
		private void BindRoles()
		{
			try
			{
				IPrincipal iprincipal = HttpContext.Current.User;
				String[] role = new string[1];
				if (Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.Administrator))
					role[0] = "Administrator";
				HttpContext.Current.User = new GenericPrincipal(iprincipal.Identity, role);
			}
			catch
			{
			}
		}
		#endregion


		#region private void GetCultureFromRequest()
		private void GetCultureFromRequest()
		{
			string cultureName = null;

			// Get culture from cookie
			HttpCookie cookie = Request.Cookies["Culture"];
			if (cookie != null)
				cultureName = cookie.Value;

			// Get default company culture
			if (cultureName == null)
				cultureName = Configuration.DefaultLocale; // Can be null if database is not initialized

			// Get user preferred culture
			if (cultureName == null)
			{
				string[] userLanguages = Request.UserLanguages;
				if (userLanguages != null && userLanguages.Length > 0)
					cultureName = userLanguages[0];
			}

			if (cultureName == null)
				cultureName = IbnConst.NeutralResourcesLanguage;

			SetCurrentCulture(cultureName);
		}
		#endregion
		#region private void SetCurrentCulture(string cultureName)
		private void SetCurrentCulture(string cultureName)
		{
			if (cultureName != null)
			{
				try
				{
					CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(cultureName);
					Thread.CurrentThread.CurrentCulture = cultureInfo;
					Thread.CurrentThread.CurrentUICulture = cultureInfo;
				}
				catch (ArgumentException)
				{
				}
			}
		}
		#endregion
		#region private void SaveCultureToCookie(string cultureName)
		private void SaveCultureToCookie(string cultureName)
		{
			HttpCookie cookie = new HttpCookie("Culture", cultureName);
			cookie.Path = HttpRuntime.AppDomainAppVirtualPath;
			cookie.Expires = DateTime.Today.AddMonths(1);
			Response.Cookies.Add(cookie);
		}
		#endregion

		#region private DatabaseState InitializeDatabase()
		private void InitializeDatabase()
		{
			try
			{
				Configuration.Init();
			}
			catch (UnsupportedSqlServerVersionException)
			{
				ReturnUnsupportedSqlServerVersionResponse();
			}
			catch (DatabaseStateException ex)
			{
				DatabaseState state = ex.State;

				if (state == DatabaseState.Initialize)
					new Thread(new ThreadStart(InitializeDatabaseAsynchronous)).Start();

				if (state != DatabaseState.Ready)
					ReturnDatabaseIsNotReadyResponse(state);
			}
		}
		#endregion
		#region private void InitializeDatabaseAsynchronous()
		private void InitializeDatabaseAsynchronous()
		{
			try
			{
				string applicationPhysicalPath = HostingEnvironment.ApplicationPhysicalPath;
				DatabaseConfigurator configurator = new DatabaseConfigurator(applicationPhysicalPath);
				configurator.InitializeDatabase();
			}
			catch (Exception ex)
			{
				CHelper.GenerateErrorReport(ex);
			}
		}
		#endregion

		#region private void ReturnUnsupportedSqlServerVersionResponse()
		private void ReturnUnsupportedSqlServerVersionResponse()
		{
			ResourceManager resourceManager = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Public.Resources.Global", typeof(Global).Assembly);
			StringBuilder builder = new StringBuilder();

			builder.Append("<?xml version='1.0' encoding='utf-8'?>");
			builder.Append("<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.1//EN' 'http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd'>");
			builder.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");

			builder.Append("<head>");

			builder.Append("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
			builder.Append("<title>");
			builder.Append(HttpUtility.HtmlEncode(resourceManager.GetString("UnsupportedSqlServerVersionTitle")));
			builder.Append("</title>");

			builder.Append("<style type='text/css'>");
			builder.Append("div.errorReport { font-family: Verdana, Arial, Helvetica, Sans-Serif; font-size: 10pt; text-align: center; background-color: rgb(255, 255, 225); border: 1px solid rgb(187, 187, 187); }");
			builder.Append("</style>");

			builder.Append("</head>");

			builder.Append("<body>");

			builder.Append("<div class='errorReport'>");
			builder.Append("<p>");
			builder.Append(HttpUtility.HtmlEncode(resourceManager.GetString("UnsupportedSqlServerVersion1")));
			builder.Append("</p>");
			builder.Append("<p>");
			builder.Append(HttpUtility.HtmlEncode(resourceManager.GetString("UnsupportedSqlServerVersion2")));
			builder.Append("</p>");
			builder.Append("</div>");

			builder.Append("</body></html>");

			Response.Clear();
			Response.ContentType = "text/html";
			Response.Write(builder.ToString());
			Response.End();
		}
		#endregion

		#region private void ReturnDatabaseIsNotReadyResponse(DatabaseState state)
		private void ReturnDatabaseIsNotReadyResponse(DatabaseState state)
		{
			ResourceManager resourceManager = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Public.Resources.Global", typeof(Global).Assembly);
			StringBuilder builder = new StringBuilder();

			builder.Append("<?xml version='1.0' encoding='utf-8'?>");
			builder.Append("<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.1//EN' 'http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd'>");
			builder.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");

			builder.Append("<head>");

			builder.Append("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
			builder.Append("<title>");
			builder.Append(HttpUtility.HtmlEncode(resourceManager.GetString("DatabaseIsNotReadyTitle")));
			builder.Append("</title>");

			builder.Append("<style type='text/css'>");
			builder.Append("div.errorReport { font-family: Verdana, Arial, Helvetica, Sans-Serif; font-size: 10pt; text-align: center; background-color: rgb(255, 255, 225); border: 1px solid rgb(187, 187, 187); }");
			builder.Append("</style>");

			builder.Append("<script type='text/javascript'>");
			builder.Append("window.setTimeout('RefreshValue();', 1000);");
			builder.Append("function RefreshValue() {");
			builder.Append("var spanTimeLeft = document.getElementById('TimeLeft');");
			builder.Append("if (spanTimeLeft) {");
			builder.Append("var seconds = spanTimeLeft.innerHTML;");
			builder.Append("seconds--;");
			builder.Append("if (seconds == 0)");
			builder.Append("window.location.href = window.location.href;");
			builder.Append("else {");
			builder.Append("spanTimeLeft.innerHTML = seconds;");
			builder.Append("window.setTimeout('RefreshValue();', 1000);");
			builder.Append("}");
			builder.Append("}");
			builder.Append("}");
			builder.Append("</script>");

			builder.Append("</head>");

			builder.Append("<body>");

			builder.Append("<div class='errorReport'>");
			builder.Append("<p>");
			builder.Append(HttpUtility.HtmlEncode(resourceManager.GetString("DatabaseIsNotReadyWait")));
			builder.Append("</p>");
			builder.Append("<p>");
			builder.Append(HttpUtility.HtmlEncode(resourceManager.GetString("DatabaseIsNotReadyReload1")));
			builder.Append("<span id='TimeLeft'>20</span>");
			builder.Append(HttpUtility.HtmlEncode(resourceManager.GetString("DatabaseIsNotReadyReload2")));
			builder.Append("</p>");
			builder.Append("</div>");

			builder.Append("</body></html>");

			Response.Clear();
			Response.ContentType = "text/html";
			Response.Write(builder.ToString());
			Response.End();
		}
		#endregion
	}
}
