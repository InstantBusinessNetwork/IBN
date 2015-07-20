using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using Mediachase.IBN.Business.WebDAV.Definition;
using System.Net;
using Mediachase.IBN.Database;
using System.Configuration;
using System.Web.Configuration;

namespace Mediachase.IBN.Business.WebDAV.Common
{
    /// <summary>
    /// Обеспечивает интерфейс доступа к сессиям аутентификации
    /// Использует таблицу EXTERNAL_GATE
    /// </summary>
    public static class WebDavAuthHelper
    {
        public const Char HTTP_CREDENTIAL_SEPARATOR = ':'; // HTTP1.1 Credential username and password separator
        public const String HTTP_AUTH_HEADER = "Authorization";  // HTTP1.1 Authorization header
        public const String HTTP_WWW_AUTH_HEADER = "WWW-Authenticate"; // HTTP1.1 Basic Challenge Scheme Name
        public const String COOCKIE_NAME = "sessionId";
        public const int COOCKIE_EXPIRES = 1;
        //Minutes
        public const string SETTING_SESSION_LIFE_TIME = "WebDavAuthSessionLifeTime";
    

        #region Database session methods
        /// <summary>
        /// Makes the auth session.
        /// </summary>
        /// <param name="forceCleanup">if set to <c>true</c> [force cleanup].</param>
        /// <param name="elStorageType">Type of the el storage.</param>
        /// <param name="objectId">The object id.</param>
        /// <returns></returns>
        public static Guid MakeAuthSession(bool forceCleanup, ObjectTypes elStorageType, int objectId)
        {
            Guid retVal = Guid.Empty;

            //Cleanup expiated sessions
            if (forceCleanup)
            {
                CleanupAuthSession(elStorageType);
            }

            UserLight currentUser = Security.CurrentUser;
            if (currentUser == null)
                throw new Exception("CurrentUser");

            return DBCommon.AddGate((int)elStorageType, objectId, currentUser.UserID);
        }


        private static void RefreshAuthSession(Guid sessionId)
        {
            DBCommon.RefreshGate(sessionId);
        }

        private static void CleanupAuthSession(ObjectTypes storageType)
        {
            //Default value
            int lifeTime = 60;
            try
            {
                lifeTime = lifeTime = Convert.ToInt32(PortalConfig.WebDavSessionLifeTime);
                //Convert.ToInt32(ConfigurationManager.AppSettings[SETTING_SESSION_LIFE_TIME]);
            }
            catch (Exception)
            {
            }
            DBCommon.RemoveGateExpired((int)storageType, lifeTime);
        }

        private static int GetUserIdByAuthToken(Guid authToken)
        {
            int retVal = -1;

            using (IDataReader reader = Mediachase.IBN.Business.User.GetGateByGuid(authToken.ToString()))
            {
                if (reader.Read())
                {
                    retVal = (int)reader["UserId"];
                }
            }
            return retVal;
        }
        #endregion

        /// <summary>
        /// Webs the dav authenticates.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="ticket">The ticket.</param>
        /// <returns></returns>
        internal static bool WebDavAuthentificate(HttpContext context, WebDavTicket ticket)
        {
            int userId = -1;
            if (ticket == null)
                throw new Exception("ticket");

			System.Web.Configuration.AuthenticationSection section = (AuthenticationSection)WebConfigurationManager.GetSection("system.web/authentication");

            //Если есть сессионный id в тикете
            if (ticket.SessionId != Guid.Empty)
            {
                CleanupAuthSession(ticket.AbsolutePath.StorageType);
                //Пытаемся получить id пользователя из тикета
                userId = TrySessionAuthentificate(ticket.SessionId);
            }
			//Пытемся аутентифицироваться из сессии
			if (userId == -1)
			{
				UserLight userLight = Security.CurrentUser;
				if(userLight != null)
				{
					userId = userLight.UserID;
				}
			
			}
			////Используем стандартный механизм аутентификации
			//if (userId == -1 && section.Mode == AuthenticationMode.Forms)
			//{
			//    System.Web.Security.FormsAuthentication.RedirectToLoginPage();
			//    return false;
			//}
			////Используем Basic аутентификацию
			//else if (userId == -1)
			//{
			//    userId = BasicAuthentificate(context, ticket);
			//}
        
            return userId != -1;
        }

		private static int BasicAuthentificate(HttpContext context, WebDavTicket ticket)
		{
			int retVal = -1;

			//Пытаемся получить id пользователя из куков
			HttpCookie userCookie = context.Request.Cookies[COOCKIE_NAME];
			if (userCookie != null)
			{
				try
				{
					Guid sessionId = new Guid(userCookie.Value);
					retVal = TrySessionAuthentificate(sessionId);
				}
				catch (System.Exception)
				{
				}
			}

			if (retVal == -1)
			{
				//Пытаемся аутентифицироваться
				retVal = TryBasicAuthentificate(context.Request.Headers[HTTP_AUTH_HEADER]);
				if (retVal != -1)
				{
					//Создаем сессию
					Guid sessionId = MakeAuthSession(true, ticket.AbsolutePath.StorageType, ticket.AbsolutePath.UniqueId);
					//Пишем в куки id сессии
					context.Response.Cookies.Add(GetAuthSessionCookie(sessionId));
				}
			}

			if (retVal == -1)
			{
				//запрос аутентификации (тип)
				foreach (KeyValuePair<string, string> header in GetNeedAuthHeader(AuthenticationSchemes.Basic))
				{
					//context.Response.AddHeader(header.Key, header.Value);
					context.Response.ContentEncoding = System.Text.Encoding.UTF8;
					context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
					context.Response.Flush();
				}
			}

			return retVal;
		}

		private static int TryBasicAuthentificate(string authorizationHeader)
		{
			int retVal = -1;
			String userName = String.Empty;
			String password = String.Empty;
			try
			{
				ExtractBasicCredentials(authorizationHeader, ref userName, ref password);
				retVal = Security.UserLogin(userName, password);
				UserLight user = UserLight.Load(retVal);
				Security.SetCurrentUser(user);

			}
			catch (System.Exception)
			{
			}

			return retVal;
		}

        private static HttpCookie GetAuthSessionCookie(Guid sessionId)
        {
            HttpCookie retVal = new HttpCookie(COOCKIE_NAME);
            DateTime now = DateTime.Now;

            retVal.Value = sessionId.ToString();
            retVal.Expires = now.AddHours(COOCKIE_EXPIRES);

            return retVal;
        }

        private static Dictionary<string, string> GetNeedAuthHeader(AuthenticationSchemes authSheme)
        {
            Dictionary<string, string> retVal = new Dictionary<string,string>();

            switch (authSheme)
            {
                case AuthenticationSchemes.Basic:
                    retVal.Add("WWW-Authenticate", "BASIC realm=\"\"");
                    break;
                case AuthenticationSchemes.Digest:
                    retVal.Add("WWW-Authenticate", "Digest realm=\"\", stale=false, nonce=\"ec2cc00f21f71acd35ab9be057970609\", qop=\"auth\", algorithm=\"MD5\"");
                    break;
                case AuthenticationSchemes.IntegratedWindowsAuthentication:
                    throw new NotSupportedException();
                default:
                    break;
            }
            return retVal;
        }

        private static int TrySessionAuthentificate(Guid sessionId)
        {
            //По умолчанию не используем утентификацию из сессии если пользователь аутентифицирован
            int retVal =  Security.CurrentUser != null ? Security.CurrentUser.UserID : -1;
            if (retVal == -1)
            {
                //Пытаемся получить id пользователя из сессии 
                retVal = WebDavAuthHelper.GetUserIdByAuthToken(sessionId);
                if (retVal != -1)
                {
                    //аутентифицируем пользователя из сессии
                    UserLight user = UserLight.Load(retVal);
                    Security.SetCurrentUser(user);
                    //Refresh auth session
                    RefreshAuthSession(sessionId);
                }
            }
            return retVal;
        }

     
        private static void ExtractBasicCredentials(String authorizationHeader, ref String username, ref String password)
        {
            if (String.IsNullOrEmpty(authorizationHeader))
                throw new ArgumentException("authorizationHeader");

            String verifiedAuthorizationHeader = authorizationHeader.Trim();
            if (verifiedAuthorizationHeader.IndexOf(AuthenticationSchemes.Basic.ToString(), StringComparison.CurrentCultureIgnoreCase) != 0)
                throw new NotSupportedException("auth method " + verifiedAuthorizationHeader + " not supported");


            // get the credential payload
            verifiedAuthorizationHeader = verifiedAuthorizationHeader.Substring(AuthenticationSchemes.Basic.ToString().Length,
                                                                                verifiedAuthorizationHeader.Length - AuthenticationSchemes.Basic.ToString().Length).Trim();
            // decode the base 64 encoded credential payload
            byte[] credentialBase64DecodedArray = Convert.FromBase64String(verifiedAuthorizationHeader);
            UTF8Encoding encoding = new UTF8Encoding();
            String decodedAuthorizationHeader = encoding.GetString(credentialBase64DecodedArray, 0, credentialBase64DecodedArray.Length);

            // get the username, password, and realm
            int separatorPosition = decodedAuthorizationHeader.IndexOf(HTTP_CREDENTIAL_SEPARATOR);

            username = decodedAuthorizationHeader.Substring(0, separatorPosition).Trim();
            password = decodedAuthorizationHeader.Substring(separatorPosition + 1, (decodedAuthorizationHeader.Length - separatorPosition - 1)).Trim();
        }

    }
}
