using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

using Mediachase.Ibn.Data.Meta;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Business.EMail;
using Mediachase.IBN.Business.WebDAV.Definition;
using Mediachase.IBN.Business.WebDAV.ElementStorageProvider;
using Mediachase.MetaDataPlus;
using Mediachase.Net.Wdom;
using Mediachase.Net.WebDavServer;

namespace Mediachase.IBN.Business.WebDAV.Common
{
	/// <summary>
	/// Предназначен для формирования url доступа к файлу и последующей публикации
	/// </summary>
	/// TODO: Добавить время жизни аутентификационной сессии.
	public static class WebDavUrlBuilder
	{
		public const string AUTH_REDIRECT_PAGE = "/FileStorage/DownloadLink.aspx";
		public const string AUTH_TICKET_PARAM_NAME = "id";
		private const string AUTH_TOKEN_CACHE_NAME = "cached_token";
		private static Regex RegExpWebDavTicketFromUri = new Regex(@"(?<ticket>/?[^/]+/[^/]+/[^\\/:*?<>]*$)", RegexOptions.Compiled);

		/// <summary>
		/// Gets the web dav URL by WebDavTicket.
		/// </summary>
		/// <param name="ticket">The ticket.</param>
		/// <returns></returns>
		public static string GetWebDavUrl(string ticket)
		{
			string retVal = string.Empty;
			WebDavTicket webDavTicket = WebDavTicket.Parse(ticket);

			//Попытаемся определить имя файла 
			if (String.IsNullOrEmpty(webDavTicket.AbsolutePath.FileName))
			{
				if (webDavTicket.AbsolutePath.StorageType == ObjectTypes.File_FileStorage)
				{
					FileStorageAbsolutePath fsAbsPath = webDavTicket.AbsolutePath as FileStorageAbsolutePath;
					if (fsAbsPath != null)
					{
						FileStorage fs = new FileStorage();
						Mediachase.IBN.Business.ControlSystem.FileInfo fileInfo = fs.GetFile(fsAbsPath.UniqueId);
						if (fileInfo != null)
						{
							webDavTicket.AbsolutePath.FileName = fileInfo.Name;
						}
					}

				}
				else if (webDavTicket.AbsolutePath.StorageType == ObjectTypes.File_MetaData)
				{
					MetaDataAbsolutePath mdAbsPath = webDavTicket.AbsolutePath as MetaDataAbsolutePath;
					if (mdAbsPath != null)
					{
						Mediachase.Ibn.Data.Meta.FileInfo fileInfo = new Mediachase.Ibn.Data.Meta.FileInfo(mdAbsPath.FileUID);
						webDavTicket.AbsolutePath.FileName = fileInfo.Name;
					}
				}
				else if (webDavTicket.AbsolutePath.StorageType == ObjectTypes.File_MetaDataPlus)
				{
					MetaDataPlusAbsolutePath mdpAbsPath = webDavTicket.AbsolutePath as MetaDataPlusAbsolutePath;
					if (mdpAbsPath != null)
					{
						Mediachase.MetaDataPlus.MetaObject obj = MetaDataWrapper.LoadMetaObject(mdpAbsPath.MetaObjectId, mdpAbsPath.MetaObjectType);
						MetaFile mf = obj[mdpAbsPath.MetaFieldName] as MetaFile;
						if (mf != null)
						{
							webDavTicket.AbsolutePath.FileName = mf.Name;
						}
					}

				}
				else if (webDavTicket.AbsolutePath.StorageType == ObjectTypes.File_Incident)
				{
					EmailStorageAbsolutePath emlAbsPath = webDavTicket.AbsolutePath as EmailStorageAbsolutePath;
					if (emlAbsPath != null)
					{
						EMailMessageInfo emlInfo = EMailMessageInfo.Load(emlAbsPath.EmailMsgId);
						AttachmentInfo attachInfo = emlInfo.Attachments[emlAbsPath.EmailAttachmentIndex];
						webDavTicket.AbsolutePath.FileName = attachInfo.FileName;
					}
				}
			}

			retVal = GetWebDavUrl(webDavTicket.AbsolutePath, true);


			return retVal;
		}

		#region FileStorage url maker
		/// <summary>
		/// Gets the file storage web dav URL for alerts.
		/// </summary>
		/// <param name="fileId">The file id.</param>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="withAuthToken">if set to <c>true</c> [with auth token].</param>
		/// <returns></returns>
		public static string GetFileStorageWebDavUrlForAlerts(int fileId, string fileName, bool withAuthToken)
		{
			return GetFileStorageWebDavUrl(fileId, -1, fileName, withAuthToken, new Uri(Configuration.PortalLink));
		}

		/// <summary>
		/// Gets the web dav URL by FileInfo.
		/// </summary>
		/// <param name="fileInfo">The file info.</param>
		/// <param name="withAuthToken">if set to <c>true</c> [with auth token].</param>
		/// <returns></returns>
		public static string GetFileStorageWebDavUrl(Mediachase.IBN.Business.ControlSystem.FileInfo fileInfo, bool withAuthToken)
		{
			if (fileInfo == null)
				throw new ArgumentNullException("fielInfo");

			return GetFileStorageWebDavUrl(fileInfo.Id, fileInfo.Name, withAuthToken);
		}
		/// <summary>
		/// Gets the web dav URL.
		/// </summary>
		/// <param name="storageType">Type of the storage.</param>
		/// <param name="fileId">The file id.</param>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="withAuthToken">if set to <c>true</c> [with auth token].</param>
		/// <returns></returns>
		public static string GetFileStorageWebDavUrl(int fileId, string fileName, bool withAuthToken)
		{
			return GetFileStorageWebDavUrl(fileId, -1, fileName, withAuthToken);
		}
		/// <summary>
		/// Gets the web dav URL.
		/// </summary>
		/// <param name="fileId">The file id.</param>
		/// <param name="historyId">The history id.</param>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="withAuthToken">if set to <c>true</c> [with auth token].</param>
		/// <returns></returns>
		public static string GetFileStorageWebDavUrl(int fileId, int historyId, string fileName, bool withAuthToken)
		{
			return GetFileStorageWebDavUrl(fileId, historyId, fileName, withAuthToken, HttpContext.Current.Request.Url);
		}

		public static string GetFileStorageWebDavUrl(int fileId, int historyId, string fileName, bool withAuthToken, Uri baseUri)
		{
			FileStorageAbsolutePath absPath = (FileStorageAbsolutePath)WebDavAbsolutePath.CreateInstance(ObjectTypes.File_FileStorage);
			absPath.UniqueId = fileId;
			absPath.FileName = fileName;
			absPath.HistoryId = historyId;
			//Do not allow server edit for history file versions
			return GetWebDavUrl(absPath, withAuthToken, historyId == -1, baseUri);
		}
	
			
		#endregion

		#region MetaData url maker
		/// <summary>
		/// Gets the web dav URL.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns></returns>
		public static string GetMetaDataWebDavUrl(Mediachase.Ibn.Data.Meta.MetaObject mo, string fieldName, bool withAuthToken)
		{
			if (mo == null)
				throw new ArgumentNullException("mo");

			return GetMetaDataWebDavUrl((MetaObjectProperty)mo.Properties[fieldName], withAuthToken);
		}
		/// <summary>
		/// Gets the web dav URL.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns></returns>
		public static string GetMetaDataWebDavUrl(MetaObjectProperty property, bool withAuthToken)
		{
			if (property == null)
				throw new ArgumentNullException("property");

			Mediachase.Ibn.Data.Meta.FileInfo fileInfo = property.Value as Mediachase.Ibn.Data.Meta.FileInfo;
			if (fileInfo == null)
				throw new ArgumentException("metaproperty " + property.Name + " is not a FileInfo type");

			return GetMetaDataWebDavUrl(fileInfo, withAuthToken);
		}
		/// <summary>
		/// Gets the web dav URL.
		/// </summary>
		/// <param name="fileUID">The file UID.</param>
		/// <param name="withAuthToken">if set to <c>true</c> [with auth token].</param>
		/// <returns></returns>
		public static string GetMetaDataWebDavUrl(Guid fileUID, bool withAuthToken)
		{
			Mediachase.Ibn.Data.Meta.FileInfo fileInfo = new Mediachase.Ibn.Data.Meta.FileInfo(fileUID);

			return GetMetaDataWebDavUrl(fileInfo, withAuthToken);
		}

		/// <summary>
		/// Gets the web dav URL.
		/// </summary>
		/// <param name="fileInfo">The file info.</param>
		/// <param name="withAuthToken">if set to <c>true</c> [with auth token].</param>
		/// <returns></returns>
		public static string GetMetaDataWebDavUrl(Mediachase.Ibn.Data.Meta.FileInfo fileInfo, bool withAuthToken)
		{
			if (fileInfo == null)
				throw new ArgumentNullException("fileInfo");

			MetaDataAbsolutePath absPath = (MetaDataAbsolutePath)WebDavAbsolutePath.CreateInstance(ObjectTypes.File_MetaData);
			absPath.FileUID = fileInfo.FileUID;
			absPath.FileName = fileInfo.Name;

			return GetWebDavUrl(absPath, withAuthToken);
		}
		#endregion

		#region MetaDataPlus Url Maker
		/// <summary>
		/// Gets the web dav URL.
		/// </summary>
		/// <param name="metaObjectId">The meta object id.</param>
		/// <param name="metaClassName">Name of the meta class.</param>
		/// <param name="metaFieldName">Name of the meta field.</param>
		/// <param name="withAuthToken">if set to <c>true</c> [with auth token].</param>
		/// <returns></returns>
		public static string GetMetaDataPlusWebDavUrl(Mediachase.MetaDataPlus.MetaObject mo, string metaField, bool withAuthToken)
		{
			MetaFile mf = mo[metaField] as MetaFile;
			if (mf == null)
				throw new ArgumentException(metaField + " is not a MetaFile");

			MetaDataPlusAbsolutePath absPath = (MetaDataPlusAbsolutePath)WebDavAbsolutePath.CreateInstance(ObjectTypes.File_MetaDataPlus);
			absPath.MetaFieldName = metaField;
			absPath.MetaObjectId = mo.Id;
			absPath.MetaObjectType = mo.MetaClass.Name;
			absPath.UniqueId = mo.Id;
			absPath.FileName = mf.Name;

			return GetWebDavUrl(absPath, withAuthToken);

		}

		/// <summary>
		/// Gets the web dav URL.
		/// </summary>
		/// <param name="storageType">Type of the storage.</param>
		/// <param name="objectId">The object id.</param>
		/// <param name="objectName">Name of the object.</param>
		/// <returns></returns>
		public static string GetMetaDataPlusWebDavUrl(int objectId, string objectType, string fieldName, bool withAuthToken)
		{
			string retVal = string.Empty;

			Mediachase.MetaDataPlus.MetaObject obj = MetaDataWrapper.LoadMetaObject(objectId, objectType);
			retVal = GetMetaDataPlusWebDavUrl(obj, fieldName, withAuthToken);

			return retVal;
		}
		#endregion


		#region Email attachment Url maker
		/// <summary>
		/// Gets the web dav URL.
		/// </summary>
		/// <param name="emailId">The email id.</param>
		/// <param name="emailAttachIndex">Index of the email attach.</param>
		/// <param name="withAuthToken">if set to <c>true</c> [with auth token].</param>
		/// <returns></returns>
		public static string GetEmailAtachWebDavUrl(int emailId, int emailAttachIndex, bool withAuthToken)
		{
			EMailMessageInfo emlInfo = EMailMessageInfo.Load(emailId);

			if (emlInfo.Attachments.Length <= emailAttachIndex)
				throw new ArgumentException("emailAttachIndex");

			AttachmentInfo attachInfo = emlInfo.Attachments[emailAttachIndex];

			EmailStorageAbsolutePath absPath = (EmailStorageAbsolutePath)WebDavAbsolutePath.CreateInstance(ObjectTypes.File_Incident);
			absPath.EmailMsgId = emailId;
			absPath.EmailAttachmentIndex = emailAttachIndex;
			absPath.FileName = attachInfo.FileName;

			return GetWebDavUrl(absPath, withAuthToken);
		}
		#endregion

		internal static string GetWebDavUrl(WebDavAbsolutePath absPath, bool withAuthToken)
		{
			return GetWebDavUrl(absPath, withAuthToken, true);
		}

		internal static string GetWebDavUrl(WebDavAbsolutePath absPath, bool withAuthToken, bool detectServerEdit)
		{
			return GetWebDavUrl(absPath, withAuthToken, true, HttpContext.Current.Request.Url);
		}

		internal static string GetWebDavUrl(WebDavAbsolutePath absPath, bool withAuthToken, 
											bool detectServerEdit, Uri baseUrl)
		{
			if (absPath == null)
			{
				throw new ArgumentNullException("absPath");
			}
			if (baseUrl == null)
			{
				throw new ArgumentNullException("baseUrl");
			}

			Guid? authToken = null;
			bool? bWebDavTurnOn = PortalConfig.UseWebDav;
			string applicationPath = baseUrl.AbsolutePath;
			if (HttpContext.Current != null)
			{
				//try get cached token
				authToken = (Guid?)HttpContext.Current.Items[AUTH_TOKEN_CACHE_NAME];
				applicationPath = HttpContext.Current.Request.ApplicationPath;
				applicationPath = applicationPath.TrimEnd('/');
			}

			ePluginToken pluginToken = ePluginToken.files;
			//Determine server editing
			if (detectServerEdit)
			{
				if (bWebDavTurnOn.HasValue && bWebDavTurnOn.Value && ContentTypeResolver.IsWebDAVSupportedExtension(Path.GetExtension(absPath.FileName)))
				{
					pluginToken = ePluginToken.webdav;
				}
			}
			//Формировать authToken only for webdav resources
			if (pluginToken == ePluginToken.webdav)
			{
				//Использовать из кеша если нет то сгенерировать новый
				if (authToken == null)
				{
					//authToken = withAuthToken ? WebDavAuthHelper.MakeAuthSession(true, absPath.StorageType, absPath.UniqueId) : Guid.Empty;
					authToken = withAuthToken ? WebDavAuthHelper.MakeAuthSession(true, ObjectTypes.File_FileStorage, 0) : Guid.Empty;
					if (HttpContext.Current != null && authToken != Guid.Empty)
					{
						//add to cache auth token
						HttpContext.Current.Items[AUTH_TOKEN_CACHE_NAME] = authToken;
					}
				}
			}
			else if (pluginToken == ePluginToken.files)
			{
				//never add auth token to file plugin token resources
				authToken = Guid.Empty;
			}
			
			WebDavTicket ticket = WebDavTicket.CreateInstance(pluginToken, authToken.Value, absPath);
			UriBuilder uriBuilder = new UriBuilder();
			uriBuilder.Scheme = baseUrl.Scheme;
			uriBuilder.Port = baseUrl.Port;
			uriBuilder.Host = baseUrl.Host;


			uriBuilder.Path = applicationPath + ticket.ToString();

			//Outer url to redirect page only for webdav access type
			if (ticket.PluginToken == ePluginToken.webdav && !withAuthToken)
			{
				uriBuilder.Path = applicationPath + AUTH_REDIRECT_PAGE;
				string webDavTicket = ticket.ToString("A");
				//Remove file name from ticket
				uriBuilder.Query = AUTH_TICKET_PARAM_NAME + String.Format("={0}", webDavTicket);
			}

			////FileName = System.Web.HttpUtility.UrlPathEncode(FileName);

			return Uri.EscapeUriString(uriBuilder.Uri.ToString());
		}

		/// <summary>
		/// Gets the web dav ticket.
		/// <remarks> Получает web dav ticket is URI формата shema://PortalHost:PortalPort/[App Portal Path]?/Ticket
		/// т.е абсолютный путь состоит только из авбсолютного пути портала http://ibn.mediachase.ru/portals/ + ticket </remarks>
		/// <remarks>Абсолютный путь портала может быть взят как из конфига  в случае вызова без httpContexta так и из веб контекста 
		/// при вызове из веба</remarks>
		/// </summary>
		/// <exception cref="Exception">Incorrect URI</exception>
		/// <param name="uriString">The URI string.</param>
		/// <returns></returns>
		internal static WebDavTicket GetWebDavTicket(string uriString)
		{
			WebDavTicket retVal = null;

			////Берем за базовый урл портала так как получить доступ к http request нет возможности
			//String appPath = Configuration.PortalLink;
			//if (HttpContext.Current != null)
			//{
			//    HttpRequest request = HttpContext.Current.Request;
			//    //Используем HttpContext
			//    appPath = request.ApplicationPath;
			//    appPath = appPath.TrimEnd('/');
			//}

			//int index = uriString.IndexOf(appPath, StringComparison.InvariantCultureIgnoreCase);
			//if (index != -1)
			//{
			//    //oтбрасывем общую часть, оставляем только ticket
			//    uriString = uriString.Substring(index + appPath.Length);
			//}
			Match match = RegExpWebDavTicketFromUri.Match(uriString.Trim());
			try
			{
				retVal = WebDavTicket.Parse(match.Groups["ticket"].Value);
			}
			catch (System.Exception)
			{
				throw new Exception("Incorrect URI.");
			}

			return retVal;
		}
	}
}
