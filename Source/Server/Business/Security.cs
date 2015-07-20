using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Security.Principal;
using System.Web;

using Mediachase.IBN.Database;
using Mediachase.Ibn;

namespace Mediachase.IBN.Business
{
	#region enum InternalSecureGroups
	public enum InternalSecureGroups
	{
		Everyone = 1,
		Administrator = 2,
		ProjectManager = 3,
		PowerProjectManager = 4,
		HelpDeskManager = 5,
		Partner = 6,
		ExecutiveManager = 7,
		Roles = 8,
		Intranet = 9,
		TimeManager = -10
	};
	#endregion
	
	#region class UserLight
	public class UserLight
	{
		private UserLightPropertyCollection	m_Properties = null;
		
		internal int m_UserID;
		internal string m_Login;
		internal string m_FirstName;
		internal string m_LastName;
		internal string m_Email;
		internal int m_TimeZoneId;
		internal string m_Culture;
		internal int m_LanguageId;
		internal bool m_IsAlertService;
		internal bool m_IsPending;
		internal bool m_IsExternal;

		private TimeZone _currentTimeZone = null;

		public int UserID
		{
			get
			{
				return m_UserID;
			}
		}

		public string Login
		{
			get
			{
				return m_Login;
			}
		}

		public string FirstName
		{
			get
			{
				return m_FirstName;
			}
		}

		public string LastName
		{
			get
			{
				return m_LastName;
			}
		}

		public string Email
		{
			get
			{
				return m_Email;
			}
		}

		public string Culture
		{
			get
			{
				return m_Culture;
			}
		}

		public int TimeZoneId
		{
			get
			{
				return m_TimeZoneId;
			}
		}

		/// <summary>
		/// Gets the current time zone.
		/// </summary>
		/// <returns></returns>
		public TimeZone CurrentTimeZone
		{
			get
			{
				if (_currentTimeZone == null)
				{
					lock (this)
					{
						if (_currentTimeZone == null)
						{
							McTimeZone timeZoneInfo = McTimeZone.Load(this.TimeZoneId);

							if (timeZoneInfo == null)
								throw new ArgumentException("Unknown TimeZoneId = '" + this.TimeZoneId.ToString() + "'.");

							_currentTimeZone = new Mediachase.Ibn.Data.UserTimeZone("Ibn User Time Zone",
								timeZoneInfo.Bias,
								timeZoneInfo.DaylightBias,
								timeZoneInfo.DaylightMonth,
								timeZoneInfo.DaylightDayOfWeek,
								timeZoneInfo.DaylightWeek,
								timeZoneInfo.DaylightHour,
								timeZoneInfo.StandardBias,
								timeZoneInfo.StandardMonth,
								timeZoneInfo.StandardDayOfWeek,
								timeZoneInfo.StandardWeek,
								timeZoneInfo.StandardHour);
						}
					}
				}

				return _currentTimeZone;
			}
		}

		public int LanguageId
		{
			get
			{
				return m_LanguageId;
			}
		}

		public bool IsAlertService
		{
			get
			{
				return m_IsAlertService;
			}
		}

		public bool IsExternal
		{
			get
			{
				return m_IsExternal;
			}
		}

		public bool IsPending
		{
			get
			{
				return m_IsPending;
			}
		}

		public string DisplayName
		{
			get
			{
				return string.Format("{0} {1}", m_LastName, m_FirstName);
			}
		}

		public UserLightPropertyCollection Properties
		{
			get
			{
				if(null == m_Properties)
					m_Properties = UserLightPropertyCollection.Load(this.m_UserID);
				return m_Properties;
			}
		}

		#region * Load *
		public static UserLight Load(int UserId)
		{
			using(IDataReader r = DBUser.GetUserInfo(UserId))
			{
				return Load(r);
			}
		}

		public static UserLight Load(string Login)
		{
			using(IDataReader r = DBUser.GetUserInfoByLogin(Login))
			{
				return Load(r);
			}
		}

		static UserLight Load(IDataReader reader)
		{
			UserLight u = null;

			if(reader.Read())
			{
				u = new UserLight();

				u.m_UserID = (int)reader["UserId"];
				u.m_Login = reader["Login"].ToString();
				u.m_FirstName = reader["FirstName"].ToString();
				u.m_LastName = reader["LastName"].ToString();
				u.m_Email = reader["Email"].ToString();
				u.m_IsExternal = (bool)reader["IsExternal"];
				u.m_IsPending = (bool)reader["IsPending"];
				u.m_IsAlertService = (u.m_Login.ToLower() == "alert" || u.m_Login.ToLower().StartsWith("alert@"));

				reader.Close();

				using(IDataReader prefReader = DBUser.GetUserPreferences(u.m_UserID))
				{
					if(prefReader.Read())
					{
						u.m_TimeZoneId = (int)prefReader["TimeZoneId"];
						u.m_Culture = (string)prefReader["Locale"];
						u.m_LanguageId = (int)prefReader["LanguageId"];
					}
					else
					{
						u.m_TimeZoneId = User.GetTimeZoneByBias(-TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours);
					}
				}
			}
			return u;
		}
		#endregion
	}

	#endregion

	public class Security
	{
		private const string sUserLight = "userlight";
		private const string sGroupsForUser = "groupsforuser";

		[ThreadStatic]
		private static UserLight m_CurrentUser = null;

		#region UserLogin(string login, string password)
		static public int UserLogin(string login, string password)
		{
			// TODO: for testing stage only
			if (login.IndexOf("@") >= 0)
				login = login.Substring(0, login.IndexOf("@"));

/*			int userId = DBUser.UserLogin(login, password);
			switch(userId)
			{
				case -1:
					throw new InvalidAccountException();
				case -2:
					throw new InvalidPasswordException();
				case -3:
					throw new NotActiveAccountException();
				case -4:
					throw new ExternalOrPendingAccountException();
			}
 */ 

			// O.R. [2008-12-09]
			//-----------------------------------------------------
			int userId = -1;
			string salt = string.Empty;
			string hash = string.Empty;
			bool isExternal = true;
			bool isPending = true;
			byte activity = 1;
			int originalId = -1;
			bool emptyPassword = false;
			using (IDataReader reader = DBUser.GetUserInfoByLogin(login))
			{
				/// UserId, Login, FirstName, LastName, Email, Activity, IMGroupId, OriginalId, IsExternal,
				/// IsPending, salt, hash
				if (reader.Read())
				{
					userId = (int)reader["UserId"];
					salt = (string)reader["salt"];
					hash = (string)reader["hash"];
					isExternal = (bool)reader["IsExternal"];
					isPending = (bool)reader["IsPending"];
					activity = (byte)reader["Activity"];
					if (reader["OriginalId"] != DBNull.Value)
						originalId = (int)reader["OriginalId"];
					if ((string)reader["password"] == string.Empty)
						emptyPassword = true;
				}
			}

			// Audit
			if (userId == -1 || userId == -2)
			{
				if (PortalConfig.AuditWebLogin)
				{
					HttpRequest request = HttpContext.Current.Request;
					string referrer = "";
					if (request.UrlReferrer != null)
						referrer = String.Concat(request.UrlReferrer.Host, request.UrlReferrer.PathAndQuery);
					string message = String.Format(CultureInfo.InvariantCulture,
						"Failed IBN portal login.\r\n\r\nLogin: {0}\r\nIP: {1}\r\nReferrer: {2}",
						login,
						request.UserHostAddress,
						referrer);
					Log.WriteEntry(message, System.Diagnostics.EventLogEntryType.FailureAudit);
				}
			}
			//

			if (userId <= 0)
			{
				throw new InvalidAccountException();
			}
			else if (activity != 3)
			{
				throw new NotActiveAccountException();
			}
			else if (isExternal || isPending)
			{
				throw new ExternalOrPendingAccountException();
			}
			else if (!PasswordUtil.Check(password, salt, hash))
			{
				throw new InvalidPasswordException();
			}

			// reset password if necessary
			if (!emptyPassword)
			{
				using (DbTransaction tran = DbTransaction.Begin())
				{
					DBUser.ResetPassword(userId);
					if (originalId > 0)
						DBUser.ResetPasswordInMain(originalId);

					tran.Commit();
				}
			}
 			//-----------------------------------------------------

			return userId;
		}
		#endregion

		#region UserLoginByTicket
		static public void UserLoginByTicket(EMail.IncidentUserTicket ticket)
		{
			UserLight user = UserLight.Load(ticket.UserId);

			SetCurrentUser(user);
		}
		#endregion

		#region UserLoginByTicket(string login, guid ticket)
		static public int UserLoginByTicket(string login, Guid ticket)
		{
			int retval = DBUser.UserLoginByTicket(login, ticket);

			// Audit
			if (retval == -1 || retval == -2)
			{
				if (PortalConfig.AuditWebLogin)
				{
					HttpRequest request = HttpContext.Current.Request;
					string referrer = "";
					if (request.UrlReferrer != null)
						referrer = String.Concat(request.UrlReferrer.Host, request.UrlReferrer.PathAndQuery);
					string message = String.Format(CultureInfo.InvariantCulture,
						"Failed IBN portal login by ticket.\r\n\r\nLogin: {0}\r\nTicket:{1}\r\nIP: {2}\r\nReferrer: {3}",
						login,
						ticket.ToString(),
						request.UserHostAddress,
						referrer);
					Log.WriteEntry(message, System.Diagnostics.EventLogEntryType.FailureAudit);
				}
			}
			//

			switch (retval)
			{
				case -1:
					throw new InvalidAccountException();
				case -2:
					throw new InvalidTicketException();
				case -3:
					throw new NotActiveAccountException();
				case -4:
					throw new ExternalOrPendingAccountException();
			}

			return retval;
		}
		#endregion

		#region UserLoginByExternalGate
		public static int UserLoginByExternalGate(int ObjectTypeId, int ObjectId, string GUID, string Email)
		{
			int UserId = DBUser.GetUserByEmail(Email, true);
			string DBGUID = DBCommon.GetGateGuid(ObjectTypeId, ObjectId, UserId);
			if (0 != string.Compare(DBGUID, GUID, true))
			{
				// Audit
				if (PortalConfig.AuditWebLogin)
				{
					HttpRequest request = HttpContext.Current.Request;
					string referrer = "";
					if (request.UrlReferrer != null)
						referrer = String.Concat(request.UrlReferrer.Host, request.UrlReferrer.PathAndQuery);
					string message = String.Format(CultureInfo.InvariantCulture,
						"Failed IBN portal external login.\r\n\r\nEmail: {0}\r\nGate:{1}\r\nIP: {2}\r\nReferrer: {3}",
						Email,
						GUID,
						request.UserHostAddress,
						referrer);
					Log.WriteEntry(message, System.Diagnostics.EventLogEntryType.FailureAudit);
				}
				//

				throw new AccessDeniedException();
			}

			return UserId;
		}
		#endregion

		#region UserLoginWindows(string windowsLogin)
		static public int UserLoginWindows(string windowsLogin)
		{
			int userId = DBUser.UserLoginByWindows(windowsLogin);

			// Audit
			if (userId == -1 || userId == -2)
			{
				if (PortalConfig.AuditWebLogin)
				{
					HttpRequest request = HttpContext.Current.Request;
					string referrer = "";
					if (request.UrlReferrer != null)
						referrer = String.Concat(request.UrlReferrer.Host, request.UrlReferrer.PathAndQuery);
					string message = String.Format(CultureInfo.InvariantCulture,
						"Failed IBN portal login.\r\n\r\nWindows login: {0}\r\nIP: {1}\r\nReferrer: {2}",
						windowsLogin,
						request.UserHostAddress,
						referrer);
					Log.WriteEntry(message, System.Diagnostics.EventLogEntryType.FailureAudit);
				}
			}
			//
			
			/*switch(userId)
			{
				case -1:
					throw new InvalidAccountException();
				case -3:
					throw new NotActiveAccountException();
				case -4:
					throw new ExternalOrPendingAccountException();
			}*/

			return userId;
		}
		#endregion

		#region UserID
		public static int UserID
		{
			get
			{
				return CurrentUser.m_UserID;
			}
		}
		#endregion

		#region GetUser
		public static UserLight GetUser(string login, string password)
		{
			// TODO: for testing stage only
			if (login.IndexOf("@") >= 0)
				login = login.Substring(0, login.IndexOf("@"));

			int UserId = Security.UserLogin(login, password);
			return UserLight.Load(UserId);
		}
		#endregion

		#region SignOut
		public static void SignOut()
		{
			SetCurrentUser(null);
		}
		#endregion

		#region SetCurrentUser(UserLight user)
		/// <summary>
		/// Returns previous user.
		/// </summary>
		internal static UserLight SetCurrentUser(UserLight user)
		{
			UserLight ret = null;

			HttpContext context = HttpContext.Current;
			if(context != null)
			{
				if(context.Items.Contains(sUserLight))
				{
					ret = (UserLight)context.Items[sUserLight];
					context.Items.Remove(sUserLight);
				}
				context.Items.Add(sUserLight, user);
				context.User = user == null ? null : new GenericPrincipal(new GenericIdentity(user.Login), null);
			}
			else
			{
				ret = m_CurrentUser;
				m_CurrentUser = user;
			}

			return ret;
		}
		#endregion

		#region SetCurrentUser(string login, out UserLight currentUser)
		/// <summary>
		/// Returns previous user.
		/// </summary>
		internal static UserLight SetCurrentUser(string login, out UserLight currentUser)
		{
			// TODO: for testing stage only
			if (login.IndexOf("@") >= 0)
				login = login.Substring(0, login.IndexOf("@"));

			UserLight ret = null;
			currentUser = null;

			UserLight u = UserLight.Load(login);

			HttpContext context = HttpContext.Current;
			if(context != null)
			{
				if(context.Items.Contains(sUserLight))
				{
					ret = (UserLight)context.Items[sUserLight];
					currentUser = ret;
				}

				if(u != null)
				{
					context.Items[sUserLight] = u;
					context.User = new GenericPrincipal(new GenericIdentity(login), null);
					currentUser = u;
				}
			}
			else
			{
				ret = m_CurrentUser;
				currentUser = u;
				m_CurrentUser = currentUser;
			}
			return ret;
		}
		#endregion

		#region CurrentUser
		public static UserLight CurrentUser
		{
			get
			{
				UserLight ret = null;
				HttpContext context = HttpContext.Current;
				if(context != null)
				{
					if(context.Items.Contains(sUserLight))
					{
						return (UserLight)context.Items[sUserLight];
					}
					else
					{
						ret = UserLight.Load(context.User.Identity.Name);
						if(ret != null)
							context.Items.Add(sUserLight, ret);
					}
				}
				else
				{
					ret = m_CurrentUser;
				}

				return ret;
			}
		}
		#endregion

		#region IsUserInGroup
		public static bool IsUserInGroup(InternalSecureGroups groupId)
		{
			UserLight cu = CurrentUser;
			int cuid = (cu != null ? cu.UserID : 0);
			return IsUserInGroup(groupId, cuid, cuid);
		}

		public static bool IsUserInGroup(int userId, InternalSecureGroups groupId)
		{
			UserLight cu = CurrentUser;
			int cuid = (cu != null ? cu.UserID : 0);
			return IsUserInGroup(groupId, userId, cuid);
		}

		private static bool IsUserInGroup(InternalSecureGroups groupId, int userId, int currentUserId)
		{
			ArrayList groups = null;
			HttpContext context = HttpContext.Current;
			if(userId == currentUserId && context != null && context.Items.Contains(sGroupsForUser))
			{
				groups = (ArrayList)context.Items[sGroupsForUser];
			}
			else
			{
				groups = User.GetListSecureGroupAll(userId);

				if(userId == currentUserId && context != null)
					context.Items.Add(sGroupsForUser, groups);
			}
			return (groups != null && groups.Contains((int)groupId));
		}
		#endregion

		#region IsManager	
		public static bool IsManager()
		{
			return IsUserInGroup(InternalSecureGroups.PowerProjectManager)
				|| IsUserInGroup(InternalSecureGroups.ProjectManager)
				|| IsUserInGroup(InternalSecureGroups.HelpDeskManager)
				|| IsUserInGroup(InternalSecureGroups.ExecutiveManager)
				|| IsUserInGroup(InternalSecureGroups.TimeManager);
		}
		#endregion

		#region CanLoginAD()
		public static bool CanLoginAD(string address)
		{
			return (PortalConfig.UseWinLogin && ActiveDirectory.IsLocalAddress(address));
		}
		#endregion
	}
}
