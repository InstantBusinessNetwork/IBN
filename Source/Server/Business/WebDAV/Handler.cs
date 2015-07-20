using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;

using Mediachase.IBN.Business.WebDAV.Common;
using Mediachase.IBN.Business.WebDAV.Definition;
using Mediachase.Net.WebDavServer;
using Mediachase.Net.WebDavServer.WebDavTrace;

namespace Mediachase.Ibn.WebDavServer
{
	/// <summary>
	/// Summary description for Handler.
	/// </summary>
	public class Handler : IHttpHandler
	{
		public const string ConstEnableTrace = "EnableWebDavTrace";

		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContext context)
		{
			//try
			//{
			//проверяем есть ли в ticket-е идентификатор сессии
			WebDavTicket ticket = WebDavUrlBuilder.GetWebDavTicket(context.Request.Url.ToString());
			if (ticket == null)
				throw new HttpException((int)HttpStatusCode.BadRequest, "Incorrect URI.");

			if (!WebDavAuthHelper.WebDavAuthentificate(context, ticket))
			{
				//Используем стандартный механизм аутентификации
				System.Web.Security.FormsAuthentication.RedirectToLoginPage();
				return;
			}
			//throw new HttpException((int)HttpStatusCode.Unauthorized, "Unauthorized");
			WebDavAbstractFactory factory = new WebDavAbstractFactory();
			WebDavElementStorageProvider storageProvider =
									factory.Create<WebDavElementStorageProvider>(ticket.AbsolutePath.StorageType);
			if (storageProvider == null)
				throw new HttpException((int)HttpStatusCode.BadRequest, "Incorrect URI.");


			//Запускаем механизм обработки web dav запроса
			WebDavTicketRequest request = new WebDavTicketRequest(context.Request, ePluginToken.webdav);
			WebDavTicketResponse response = new WebDavTicketResponse(context.Response);
			WebDavApplication.DefaultProvider = storageProvider;

			try
			{
				bool enableTrace = Convert.ToBoolean(ConfigurationManager.AppSettings[ConstEnableTrace]);
				if (enableTrace)
				{
					WebDavTracer.TraceBinaryBody = false;
					WebDavTracer.EnableTrace = true;
				}
			}
			catch (FormatException)
			{
			}


			if (request.HttpMethod == WebDavHttpMethod.GET)
			{
				//для get запросов выключаем буферизацию для работы с большими файлами
				context.Response.BufferOutput = false;
				//Некоторые браузеры кешируют результаты GET запроса при редактировании на сервере.
				//Установка expires равную текущей дате позволят указать браузеру что кеширование не требуется
				HttpCachePolicy cache = HttpContext.Current.Response.Cache;
				cache.SetExpires(DateTime.Now);

				//Если для запрошенного файла стоит флаг ForceDownload и не включен фдаг WebDav
				//то вернуть как attachment для выбора пользователем приложения для работы с документом
				if (!string.IsNullOrEmpty(ticket.AbsolutePath.FileName))
				{
					string fileExtension = Path.GetExtension(ticket.AbsolutePath.FileName);
					if (!ContentTypeResolver.IsWebDAVSupportedExtension(fileExtension) &&
						ContentTypeResolver.IsAllowForceDownload(fileExtension))
					{
						context.Response.AddHeader("Content-Disposition", "attachment;" + "filename=" + ticket.AbsolutePath.FileName);
					}
				}
			}

			WebDavApplication.ProcessRequest(request, response);

			//}
			//catch (HttpException ex)
			//{
			//    // Http Exception
			//    context.Response.Clear();

			//    context.Response.ContentEncoding = System.Text.Encoding.UTF8;
			//    context.Response.StatusCode = ex.GetHttpCode();
			//    context.Response.StatusDescription = ex.Message;
			//}
			//catch (Exception ex)
			//{
			//    // Global Exception
			//    context.Response.Clear();

			//    context.Response.ContentEncoding = System.Text.Encoding.UTF8;
			//    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			//    context.Response.StatusDescription = ex.Message;

			//    byte[] errorBuffer = System.Text.Encoding.UTF8.GetBytes(ex.ToString());
			//    context.Response.OutputStream.Write(errorBuffer, 0, errorBuffer.Length);
			//}

			//context.Response.Flush();
		}

		//private string GetPath(string appPath, Uri url)
		//{
		//    appPath = appPath.TrimEnd(new char[] { '/' });

		//    string path = HttpUtility.UrlDecode(url.AbsolutePath + url.Fragment);
		//    if (appPath != "")
		//    {
		//        path = path.Substring(appPath.Length);
		//    }
		//    return path;
		//}

		//private void LoadFile(string path, Stream output)
		//{
		//    try
		//    {
		//        byte[] tmpBuffer = new byte[Resource.BufferSize];
		//        using (Stream input = new FileStream(path, FileMode.Open, FileAccess.Read))
		//        {
		//            long count = input.Length;
		//            int curcount;
		//            do
		//            {
		//                curcount = input.Read(tmpBuffer, 0, tmpBuffer.Length < (int)count ? tmpBuffer.Length : (int)count);
		//                output.Write(tmpBuffer, 0, curcount);

		//                count -= curcount;
		//            }
		//            while (count > 0);
		//        }
		//    }
		//    catch (Exception)
		//    {
		//        throw new ServerException(404, "Not found");
		//    }
		//}

		//public void ProcessRequest(HttpContext context)
		//{
		//    try
		//    {
		//        Command command = null;
		//        String path = GetPath(context.Request.ApplicationPath, context.Request.Url);

		//        if (!path.ToLower().StartsWith("/webdav/") && !path.ToLower().StartsWith("/files/") && path != "/")
		//        {
		//            if (context.Request.HttpMethod=="GET")
		//            {
		//                LoadFile(HttpUtility.UrlDecode(
		//                    context.Request.Url.AbsolutePath + context.Request.Url.Fragment),
		//                    context.Response.OutputStream);

		//                //context.Response.OutputStream.Flush();

		//                context.Response.StatusCode = 200;
		//                context.Response.StatusDescription = "OK";
		//                return;	
		//            }
		//            else throw new ServerException(404, "Not found");
		//        }
		//        switch (context.Request.HttpMethod)
		//        {
		//            case "OPTIONS":
		//                command = new OptionCommand(context.Request, context.Response);
		//                // TODO: alex added here
		//                command.Run(null);
		//                break;
		//            case "PROPFIND":
		//                command = new PropFindCommand(context.Request, context.Response);
		//                break;
		//            case "PROPPATCH":
		//                command = new PropPatchCommand(context.Request, context.Response);
		//                break;
		//            case "GET":
		//                command = new GetCommand(context.Request, context.Response);
		//                break;
		//            case "HEAD":
		//                command = new HeadCommand(context.Request, context.Response);
		//                break;
		//            case "PUT":
		//                command = new PutCommand(context.Request, context.Response);
		//                break;
		//            case "LOCK":
		//                command = new LockCommand(context.Request, context.Response);
		//                break;
		//            case "UNLOCK":
		//                command = new UnLockCommand(context.Request, context.Response);
		//                break;
		//            default:
		//                throw new ServerException(0x195, "Method Not Allowed");
		//        }
		//        command.Run(new Resource(path));
		//    }
		//    catch (System.Threading.ThreadAbortException)
		//    {
		//        throw;
		//    }
		//    catch (ServerException exception)
		//    {
		//        context.Response.StatusCode = exception.Code;
		//        context.Response.StatusDescription = exception.Description;
		//        if (exception.Code==0xcc)				
		//        {
		//            context.Response.ClearContent();
		//            context.Response.AddHeader("Content-Length", "0");
		//        }
		//    }
		//    //catch (Exception ex)
		//    //{
		//    //    context.Response.StatusCode = 500;
		//    //    context.Response.StatusDescription = "Internal server error";
		//    //    context.Response.Write(ex.ToString());
		//    //}
		//}
	}
}
