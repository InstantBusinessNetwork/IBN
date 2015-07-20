using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Mediachase.Ibn.WebDavServer;
using Mediachase.IBN.Business.WebDAV.Definition;
using Mediachase.Net.WebDavServer;
using System.Text.RegularExpressions;

namespace Mediachase.IBN.Business.WebDAV
{
	/// <summary>
	/// Данный модуль обеспечивает корректные WebDav ответы для корня портала (как коллекции)
	/// <remarks>
	/// Для запросов PROPFIND и OPTIONS относительно / должны быть валидные ответы 
	/// А так как в портале включена Form Authentification то возвращался 302 код ошибки
	/// </remarks>
	/// </summary>
	public class RootFolderModule : IHttpModule
	{
		private static Regex WebDavRootRegExp = new Regex(@"/webdav/[0-9a-zA-Z\+=_-]+/?$");
	
		#region IHttpModule Members

		public void Dispose()
		{
		}

		// In the Init function, register for HttpApplication 
		// events by adding your handlers.
		public void Init(HttpApplication application)
		{
			application.BeginRequest +=
				(new EventHandler(this.Application_BeginRequest));
		}

	
		#endregion

		private void Application_BeginRequest(Object source, EventArgs e)
		{
			// Create HttpApplication and HttpContext objects to access
			// request and response properties.
			HttpApplication application = (HttpApplication)source;
			HttpContext context = application.Context;
			if (context.Request.HttpMethod == "PROPFIND" || context.Request.HttpMethod == "OPTIONS")
			{
				Uri url = context.Request.Url;
				if (IsWebDavRootFolderRequest(context.Request))
				{
					WebDavAbstractFactory factory = new WebDavAbstractFactory();
					WebDavElementStorageProvider storageProvider = factory.Create<WebDavElementStorageProvider>(ObjectTypes.Folder);
					if (storageProvider != null)
					{
						WebDavApplication.DefaultProvider = storageProvider;
						WebDavTicketRequest request = new WebDavTicketRequest(context.Request, ePluginToken.webdav);
						WebDavTicketResponse response = new WebDavTicketResponse(context.Response);
						WebDavApplication.ProcessRequest(request, response);
						context.Response.End();
					}
					
				}
			}
		}

		private static bool IsWebDavRootFolderRequest(HttpRequest request)
		{
			bool retVal = false;
			Uri url = request.Url;

			retVal = url.AbsolutePath == "/" || url.AbsolutePath == "/webdav";
			//allow /portal_name/webdav/ || /portal_name
			if (!retVal)
			{
				retVal = url.AbsolutePath == request.ApplicationPath
						 || url.AbsolutePath == request.ApplicationPath + "/webdav";
			}
			//allow /portal_name/webdav/webdav_ticket
			if (!retVal)
			{
				Match match = WebDavRootRegExp.Match(url.AbsolutePath.Trim());
				retVal = match.Success;
			}
			return retVal;
		}
	}
}
