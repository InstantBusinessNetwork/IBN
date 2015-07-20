using System;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBUser.
	/// </summary>
	public class DBUser
	{

		#region Create
		public static int Create(
			string Login,
			string WindowsLogin,
			string Password, 
			string FirstName, 
			string LastName, 
			string Email,
			string salt,
			string hash,
			int Activity,
			bool IsExternal,
			int CreatedBy,
			string InviteText)
		{
			return DbHelper2.RunSpInteger("UserCreate",
				DbHelper2.mp("@Login", SqlDbType.NVarChar, 250, Login),
				DbHelper2.mp("@WindowsLogin", SqlDbType.NVarChar, 250, WindowsLogin),
				DbHelper2.mp("@Password", SqlDbType.NVarChar, 50, Password, false),
				DbHelper2.mp("@FirstName", SqlDbType.NVarChar, 50, FirstName),
				DbHelper2.mp("@LastName", SqlDbType.NVarChar, 50, LastName),
				DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, Email),
				DbHelper2.mp("@salt", SqlDbType.VarChar, 50, salt),
				DbHelper2.mp("@hash", SqlDbType.VarChar, 50, hash),
				DbHelper2.mp("@Activity", SqlDbType.TinyInt, Activity),
				DbHelper2.mp("@IsExternal",SqlDbType.Bit, IsExternal),
				DbHelper2.mp("@CreatedBy", SqlDbType.Int, CreatedBy),
				DbHelper2.mp("@InviteText",SqlDbType.NText, InviteText));
		}
		#endregion

		#region AddToMain
		public static int AddToMain(
			int UserId,
			string Login, 
			string Password, 
			string FirstName, 
			string LastName, 
			string Email, 
			string salt,
			string hash,
			bool IsActive,
			int IMGroupId)
		{
			return DbHelper2.RunSpInteger("ASP_ADD_USER",
				DbHelper2.mp("@user_id", SqlDbType.Int, UserId),
				DbHelper2.mp("@login", SqlDbType.NVarChar, 250, Login),
				DbHelper2.mp("@password", SqlDbType.NVarChar, 50, Password, false),
				DbHelper2.mp("@first_name",SqlDbType.NVarChar, 50, FirstName),
				DbHelper2.mp("@last_name", SqlDbType.NVarChar, 50, LastName),
				DbHelper2.mp("@email", SqlDbType.NVarChar, 250, Email),
				DbHelper2.mp("@salt", SqlDbType.VarChar, 50, salt),
				DbHelper2.mp("@hash", SqlDbType.VarChar, 50, hash),
				DbHelper2.mp("@imgroup_id", SqlDbType.Int, IMGroupId),
				DbHelper2.mp("@is_active", SqlDbType.Bit, IsActive));
		}
		#endregion

		#region UpdateMain
		public static void UpdateMain(
			int UserId,
			string FirstName,
			string LastName,
			string Email)
		{
			DbHelper2.RunSp("ASP_UPDATE_USER",
				DbHelper2.mp("@user_id", SqlDbType.Int, UserId),
				DbHelper2.mp("@first_name",SqlDbType.NVarChar, 50, FirstName),
				DbHelper2.mp("@last_name", SqlDbType.NVarChar, 50, LastName),
				DbHelper2.mp("@email", SqlDbType.NVarChar, 250, Email));
		}
		#endregion

		#region UpdateUserInfo
		/// <summary>
		/// If password == "", then the old password remains
		/// </summary>
		public static void UpdateUserInfo(
			int UserId, 
			string Password, 
			string FirstName, 
			string LastName, 
			string Email,
			string salt,
			string hash,
			int Activity,
			bool IsExternal)
		{
			DbHelper2.RunSp("UserUpdate",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, UserId),
				DbHelper2.mp("@Password", SqlDbType.NVarChar, 50, Password, false),
				DbHelper2.mp("@FirstName", SqlDbType.NVarChar, 50, FirstName),
				DbHelper2.mp("@LastName", SqlDbType.NVarChar, 50, LastName),
				DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, Email),
				DbHelper2.mp("@salt", SqlDbType.VarChar, 50, salt),
				DbHelper2.mp("@hash", SqlDbType.VarChar, 50, hash),
				DbHelper2.mp("@Activity", SqlDbType.TinyInt, Activity),
				DbHelper2.mp("@IsExternal", SqlDbType.Bit, IsExternal));
		}
		#endregion

		#region UpdateUserInfo
		/// <summary>
		/// If password == "", then the old password remains
		/// </summary>
		public static void UpdateUserInfo(
			int UserId, 
			string Password, 
			string FirstName, 
			string LastName, 
			string Email,
			string salt,
			string hash)
		{
			DbHelper2.RunSp("UserUpdateSimple",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, UserId),
				DbHelper2.mp("@Password", SqlDbType.NVarChar, 50, Password, false),
				DbHelper2.mp("@FirstName", SqlDbType.NVarChar, 50, FirstName),
				DbHelper2.mp("@LastName", SqlDbType.NVarChar, 50, LastName),
				DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, Email),
				DbHelper2.mp("@salt", SqlDbType.VarChar, 50, salt),
				DbHelper2.mp("@hash", SqlDbType.VarChar, 50, hash));
		}
		#endregion

		#region UpdateExternalUserInfo
		/// <summary>
		/// </summary>
		public static void UpdateExternalUserInfo(
			int UserId, 
			string FirstName, 
			string LastName, 
			string Email, 
			int Activity)
		{
			DbHelper2.RunSp("UserUpdateExternal",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, UserId),
				DbHelper2.mp("@FirstName", SqlDbType.NVarChar, 50, FirstName),
				DbHelper2.mp("@LastName", SqlDbType.NVarChar, 50, LastName),
				DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, Email),
				DbHelper2.mp("@Activity", SqlDbType.TinyInt, Activity));
		}
		#endregion

		#region UpdatePendingUserInfo
		/// <summary>
		/// </summary>
		public static void UpdatePendingUserInfo(
			int UserId, 
			string Login,
			string Password,
			string FirstName, 
			string LastName, 
			string Email,
			string salt,
			string hash,
			int Activity,
			int CreatedBy)
		{
			DbHelper2.RunSp("UserUpdatePending",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, UserId),
				DbHelper2.mp("@Login", SqlDbType.NVarChar, 250, Login),
				DbHelper2.mp("@Password", SqlDbType.NVarChar, 50, Password, false),
				DbHelper2.mp("@FirstName", SqlDbType.NVarChar, 50, FirstName),
				DbHelper2.mp("@LastName", SqlDbType.NVarChar, 50, LastName),
				DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, Email),
				DbHelper2.mp("@salt", SqlDbType.VarChar, 50, salt),
				DbHelper2.mp("@hash", SqlDbType.VarChar, 50, hash),
				DbHelper2.mp("@Activity", SqlDbType.TinyInt, Activity),
				DbHelper2.mp("@CreatedBy", SqlDbType.Int, CreatedBy));
		}
		#endregion

		#region ConvertFromExternal
		/// <summary>
		/// </summary>
		public static void ConvertFromExternal(
			int UserId, 
			string Login,
			string Password,
			string FirstName, 
			string LastName, 
			string Email,
			string salt,
			string hash,
			int Activity,
			int CreatedBy)
		{
			DbHelper2.RunSp("UserConvertFromExternal",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, UserId),
				DbHelper2.mp("@Login", SqlDbType.NVarChar, 250, Login),
				DbHelper2.mp("@Password", SqlDbType.NVarChar, 50, Password, false),
				DbHelper2.mp("@FirstName", SqlDbType.NVarChar, 50, FirstName),
				DbHelper2.mp("@LastName", SqlDbType.NVarChar, 50, LastName),
				DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, Email),
				DbHelper2.mp("@salt", SqlDbType.VarChar, 50, salt),
				DbHelper2.mp("@hash", SqlDbType.VarChar, 50, hash),
				DbHelper2.mp("@Activity", SqlDbType.TinyInt, Activity),
				DbHelper2.mp("@CreatedBy", SqlDbType.Int, CreatedBy));
		}
		#endregion

		#region UpdateProfile
		public static void UpdateProfile(
			int UserId, 
			string phone, 
			string fax, 
			string mobile,
			string position, 
			string department, 
			string company, 
			string location)
		{
			DbHelper2.RunSp("UserDetailsCreateUpdate",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@phone", SqlDbType.NVarChar, 100, phone),
				DbHelper2.mp("@fax", SqlDbType.NVarChar, 100, fax),
				DbHelper2.mp("@mobile", SqlDbType.NVarChar, 100, mobile),
				DbHelper2.mp("@position", SqlDbType.NVarChar, 100, position),
				DbHelper2.mp("@department", SqlDbType.NVarChar, 100, department),
				DbHelper2.mp("@company", SqlDbType.NVarChar, 100, company),
				DbHelper2.mp("@location", SqlDbType.NVarChar, 100, location));
		}

		public static void UpdateProfile(
			int UserId, 
			string phone, 
			string mobile)
		{
			DbHelper2.RunSp("UserDetailsUpdate",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@phone", SqlDbType.NVarChar, 100, phone),
				DbHelper2.mp("@mobile", SqlDbType.NVarChar, 100, mobile));
		}

		public static void UpdateProfile(
			int UserId, 
			string position, 
			string department, 
			string company, 
			string location)
		{
			DbHelper2.RunSp("UserDetailsUpdate2",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@position", SqlDbType.NVarChar, 100, position),
				DbHelper2.mp("@department", SqlDbType.NVarChar, 100, department),
				DbHelper2.mp("@company", SqlDbType.NVarChar, 100, company),
				DbHelper2.mp("@location", SqlDbType.NVarChar, 100, location));
		}
		#endregion

		#region UpdateUserPhoto
		public static void UpdateUserPhoto(int UserId, string PictureUrl)
		{
			DbHelper2.RunSp("UserDetailsUpdatePhoto",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@PictureUrl", SqlDbType.NVarChar, 1024, PictureUrl));
		}
		#endregion

		#region UpdateLanguage
		public static void UpdateLanguage(int UserId, int LanguageId)
		{
			DbHelper2.RunSp("UserPreferencesUpdateLanguage",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region UpdateTimeZoneId
		public static void UpdateTimeZoneId(int UserId, int TimeZoneId)
		{
			DbHelper2.RunSp("UserPreferencesUpdateTimeZoneId",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId));
		}
		#endregion

		#region UpdateTimeOffsetLatest
		public static void UpdateTimeOffsetLatest(int UserId, int TimeOffset)
		{
			DbHelper2.RunSp("UserPreferencesUpdateTimeOffsetLatest",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, TimeOffset));
		}
		#endregion

		#region UpdatePreferences
		public static void UpdatePreferences(
			int UserId,
			bool IsNotified,
			bool IsNotifiedByEmail,
			bool IsNotifiedByIBN,
			bool IsBatchNotifications,
			int Period,
			int From,
			int Till,
			int TimeZoneId,
			int LanguageId,
			int ReminderType)
		{
			DbHelper2.RunSp("UserPreferencesCreateUpdate",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@IsNotified", SqlDbType.Bit, IsNotified),
				DbHelper2.mp("@IsNotifiedByEmail", SqlDbType.Bit, IsNotifiedByEmail),
				DbHelper2.mp("@IsNotifiedByIBN", SqlDbType.Bit, IsNotifiedByIBN),
				DbHelper2.mp("@IsBatchNotifications", SqlDbType.Bit, IsBatchNotifications),
				DbHelper2.mp("@Period", SqlDbType.Int, Period),
				DbHelper2.mp("@From", SqlDbType.Int, From),
				DbHelper2.mp("@Till", SqlDbType.Int, Till),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@ReminderType", SqlDbType.Int, ReminderType));
		}
		#endregion

		#region AssignIMGroup
		public static void AssignIMGroup(int UserId, int IMGroupId)
		{
			DbHelper2.RunSp("UserIMGroupAssign",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@IMGroupId", SqlDbType.Int, IMGroupId));
		}
		#endregion

		#region AssignOriginalId
		public static void AssignOriginalId(int UserId, int OriginalId)
		{
			DbHelper2.RunSp("UserAssignOriginalId",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@OriginalId", SqlDbType.Int, OriginalId));
		}
		#endregion
 
		#region AddUserToGroup
		public static void AddUserToGroup(int UserId, int GroupId)
		{
			DbHelper2.RunSp("UserAddToGroup",
				DbHelper2.mp("@UserID", SqlDbType.Int, UserId),
				DbHelper2.mp("@GroupID", SqlDbType.Int, GroupId));
			// OZ: User Role Addon:
			switch(GroupId)
			{
					//Administrator = 2,
				case 2:
					Mediachase.IBN.Database.ControlSystem.DBUserRole.Add(UserId,"Admin");
					break;
					//ProjectManager = 3,
				case 3:
					Mediachase.IBN.Database.ControlSystem.DBUserRole.Add(UserId,"GlobalPM");
					break;
					//PowerProjectManager = 4,
				case 4:
					Mediachase.IBN.Database.ControlSystem.DBUserRole.Add(UserId,"PPM");
					break;
					//HelpDeskManager = 5,
				case 5:
					Mediachase.IBN.Database.ControlSystem.DBUserRole.Add(UserId,"HDM");
					break;
					//ExecutiveManager = 7,
				case 7:
					Mediachase.IBN.Database.ControlSystem.DBUserRole.Add(UserId,"GlobalEM");
					break;
				case -10:
					Mediachase.IBN.Database.ControlSystem.DBUserRole.Add(UserId, "TM");
					break;
			}
			//
		}
		#endregion

		#region DeleteUserFromGroup
		public static void DeleteUserFromGroup(int UserId, int GroupId)
		{
			DbHelper2.RunSp("UserDeleteFromGroup",
				DbHelper2.mp("@UserID", SqlDbType.Int, UserId),
				DbHelper2.mp("@GroupID", SqlDbType.Int, GroupId));
			// OZ: User Role Addon:
			switch(GroupId)
			{
					//Administrator = 2,
				case 2:
					Mediachase.IBN.Database.ControlSystem.DBUserRole.DeleteByUser(UserId,"Admin");
					break;
					//ProjectManager = 3,
				case 3:
					Mediachase.IBN.Database.ControlSystem.DBUserRole.DeleteByUser(UserId,"GlobalPM");
					break;
					//PowerProjectManager = 4,
				case 4:
					Mediachase.IBN.Database.ControlSystem.DBUserRole.DeleteByUser(UserId,"PPM");
					break;
					//HelpDeskManager = 5,
				case 5:
					Mediachase.IBN.Database.ControlSystem.DBUserRole.DeleteByUser(UserId,"HDM");
					break;
					//ExecutiveManager = 7,
				case 7:
					Mediachase.IBN.Database.ControlSystem.DBUserRole.DeleteByUser(UserId,"GlobalEM");
					break;
				case -10:
					Mediachase.IBN.Database.ControlSystem.DBUserRole.DeleteByUser(UserId, "TM");
					break;
			}
			//
		}
		#endregion

		#region GetUserInfo
		/// <summary>
		/// Reader returns fields:
		/// UserId, Password, Login, FirstName, LastName, Email, Activity, IsActive, IMGroupId, CreatedBy, Comments, IsExternal, IsPending, OriginalId, WindowsLogin, LdapUid
		/// </summary>
		public static IDataReader GetUserInfo(int UserId)
		{
			return DbHelper2.RunSpDataReader("UserGet", 
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetUserInfoByOriginalId
		/// <summary>
		/// Reader returns fields:
		/// UserId, Login, FirstName, LastName, Email, Activity, IMGroupId
		/// </summary>
		public static IDataReader GetUserInfoByOriginalId(int OriginalId)
		{
			return DbHelper2.RunSpDataReader("UserGetByOriginalId", 
				DbHelper2.mp("@OriginalId", SqlDbType.Int, OriginalId));
		}
		#endregion

		#region GetUserInfoByLogin
		/// <summary>
		/// Reader returns fields:
		/// UserId, Login, FirstName, LastName, Email, IsActive, IMGroupId, IsExternal, IsPending, 
		/// salt, hash, password
		/// </summary>
		public static IDataReader GetUserInfoByLogin(string Login)
		{
			return DbHelper2.RunSpDataReader("UserGetInfoByLogin", 
				DbHelper2.mp("@Login", SqlDbType.NVarChar, 250, Login));
		}
		#endregion

		#region GetUserPreferences
		/// <summary>
		/// UserId, IsNotified, IsNotifiedByEmail, IsNotifiedByIBN, IsBatchNotifications, 
		/// Period, From, Till, TimeZoneId, TimeOffsetLatest, LanguageId, Locale, 
		/// LanguageName, ReminderType, BatchLastSent, BatchNextSend
		/// </summary>
		public static IDataReader GetUserPreferences(int TimeZoneId, int UserId)
		{
			return DbHelper2.RunSpDataReader(TimeZoneId,
				new string[]{"BatchLastSent", "BatchNextSend"},
				"UserPreferencesGet", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		public static IDataReader GetUserPreferences(int UserId)
		{
			return DbHelper2.RunSpDataReader("UserPreferencesGet", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetUserProfile
		/// <summary>
		/// Reader returns fields:
		/// UserId, phone, fax, mobile, position, department, company, location, PictureUrl 
		/// </summary>
		public static IDataReader GetUserProfile(int UserId)
		{
			return DbHelper2.RunSpDataReader("UserDetailsGet", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListSecureGroupExplicit
		/// <summary>
		/// Reader returns fields:
		/// GroupId, GroupName
		/// </summary>
		public static IDataReader GetListSecureGroupExplicit(int UserId)
		{
			return DbHelper2.RunSpDataReader("GroupsGetByUser", 
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListSecureGroupAll
		/// <summary>
		/// Reader returns fields:
		/// GroupId
		/// </summary>
		public static IDataReader GetListSecureGroupAll(int UserId)
		{
			return DbHelper2.RunSpDataReader("GroupsGetByUser_All", 
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, UserId));
		}
		#endregion

		#region UserLogin
		public static int UserLogin(string Login, string Password)
		{
			return DbHelper2.RunSpInteger("UserLogin",
				DbHelper2.mp("@Login", SqlDbType.NVarChar, 250, Login),
				DbHelper2.mp("@Password", SqlDbType.NVarChar, 50, Password, false));
		}
		#endregion

		#region UserLoginByTicket
		public static int UserLoginByTicket(string login, Guid ticket)
		{
			return DbHelper2.RunSpInteger("UserLoginByTicket",
				DbHelper2.mp("@Login", SqlDbType.NVarChar, 250, login),
				DbHelper2.mp("@Ticket", SqlDbType.UniqueIdentifier, ticket));
		}
		#endregion

		#region UserLoginByWindows()
		public static int UserLoginByWindows(string WindowsLogin)
		{
			return DbHelper2.RunSpInteger("UserLoginByWindows",
				DbHelper2.mp("@WindowsLogin", SqlDbType.NVarChar, 250, WindowsLogin));
		}
		#endregion

		#region UserLoginByEmail
		public static int UserLoginByEmail(string Email)
		{
			return DbHelper2.RunSpInteger("UserLoginByEmail",
				DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, Email));
		}
		#endregion

		#region Properties
		public static IDataReader GetProperties(int UserId)
		{
			return DbHelper2.RunSpDataReader("UserPropertyGet", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}

		public static int SetProperty(int UserId, string Key, string Value)
		{
			return DbHelper2.RunSpInteger("UserPropertySet", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@Key", SqlDbType.NVarChar, 100, Key),
				DbHelper2.mp("@Value", SqlDbType.NText, Value, false));
		}

		public static void RemoveProperty(int UserId, string Key)
		{
			DbHelper2.RunSp("UserPropertyRemove", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@Key", SqlDbType.NVarChar, 100, Key));
		}

		//dvs 2009-05-07
		//deletes all properties with [key] like %Key%
		public static void RemovePropertyLike(int UserId, string Key)
		{
			DbHelper2.RunSp("UserPropertyRemoveLike",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@Key", SqlDbType.NVarChar, 100, Key));
		}
		#endregion

		#region GetSharingLevel
		public static int GetSharingLevel(int UserId, int ProUserId)
		{
			return DbHelper2.RunSpInteger("SharingGetLevel",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ProUserId", SqlDbType.Int, ProUserId));
		}
		#endregion

		#region DeleteSharing
		public static void DeleteSharing(int UserId, int ProUserId)
		{
			DbHelper2.RunSp("SharingDelete",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ProUserId", SqlDbType.Int, ProUserId));
		}
		#endregion

		#region AddSharing
		public static void AddSharing(int UserId, int ProUserId, int Level)
		{
			DbHelper2.RunSp("SharingAdd",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ProUserId", SqlDbType.Int, ProUserId),
				DbHelper2.mp("@Level", SqlDbType.Int, Level));
		}
		#endregion

		#region GetListSharingByUser
		/// <summary>
		/// Reader returns fields:
		/// UserId, FirstName, LastName, Email, Login, Level
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListSharingByUser(int UserId)
		{
			return DbHelper2.RunSpDataReader("SharingGetByUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListSharingByUserDataTable
		/// <summary>
		/// DataTable returns fields:
		/// UserId, FirstName, LastName, Email, Login, Level
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListSharingByUserDataTable(int UserId)
		{
			return DbHelper2.RunSpDataTable("SharingGetByUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListUsersBySubstring
		/// <summary>
		/// Reader returns fields:
		/// UserId, FirstName, LastName, Email, Login, IsExternal, IsPending
		/// </summary>
		public static IDataReader GetListUsersBySubstring(string SubString)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			SubString = DBCommon.ReplaceSqlWildcard(SubString);
			//

			return DbHelper2.RunSpDataReader("UsersSearch", 
				DbHelper2.mp("@SubString", SqlDbType.NVarChar, 50, @SubString));
		}
		#endregion

		#region GetListUsersBySubstringDataTable
		/// <summary>
		/// Reader returns fields:
		/// UserId, FirstName, LastName, Email, Login, IsExternal, IsPending
		/// </summary>
		public static DataTable GetListUsersBySubstringDataTable(string SubString)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			SubString = DBCommon.ReplaceSqlWildcard(SubString);
			//

			return DbHelper2.RunSpDataTable("UsersSearch", 
				DbHelper2.mp("@SubString", SqlDbType.NVarChar, 50, @SubString));
		}
		#endregion

		#region GetOriginalId
		public static int GetOriginalId(int UserId)
		{
			return DbHelper2.RunSpInteger("UserGetOriginalId",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion


		#region GetPrinciplaId
		public static int GetPrincipalId(int OriginalId)
		{
			return DbHelper2.RunSpInteger("UserGetPrincipalId",
				DbHelper2.mp("@OriginalId", SqlDbType.Int, OriginalId));
		}
		#endregion

		#region GetPassword
		public static string GetPassword(int UserId)
		{
			return (string)DbHelper2.RunSpScalar("UserGetPassword", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetUserStatus
		public static int GetUserStatus(int UserId)
		{
			return DbHelper2.RunSpInteger("ASP_GET_USER_STATUS", 
				DbHelper2.mp("@user_id", SqlDbType.Int, UserId));
		}
		#endregion

		#region AddScheduledUser
		public static void AddScheduledUser(int UserId, int ContUserId)
		{
			DbHelper2.RunSp("ScheduleAdd",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ContUserId", SqlDbType.Int, ContUserId));
		}
		#endregion

		#region DeleteScheduledUser
		public static void DeleteScheduledUser(int UserId, int ContUserId)
		{
			DbHelper2.RunSp("ScheduleDelete",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ContUserId", SqlDbType.Int, ContUserId));
		}
		#endregion

		#region GetListScheduledUsers
		/// <summary>
		/// Reader returns fields:
		/// UserId, FirstName, LastName, Email, Login, IsActive
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListScheduledUsers(int UserId)
		{
			return DbHelper2.RunSpDataReader("ScheduleGetByUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListScheduledUsersDataTable
		/// <summary>
		/// DataTable returns fields:
		/// UserId, FirstName, LastName, Email, Login
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListScheduledUsersDataTable(int UserId)
		{
			return DbHelper2.RunSpDataTable("ScheduleGetByUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetUserByEmail
		public static int GetUserByEmail(string Email, bool OnlyExternal)
		{
			return DbHelper2.RunSpInteger("UserGetByEmail", 
				DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, Email),
				DbHelper2.mp("@OnlyExternal", SqlDbType.Bit, OnlyExternal));
		}
		#endregion

		#region GetUserByLogin
		public static int GetUserByLogin(string Login)
		{
			return DbHelper2.RunSpInteger("UserGetByLogin", 
				DbHelper2.mp("@Login", SqlDbType.NVarChar, 250, Login));
		}
		#endregion

		#region GetUserByWindowsLogin()
		public static int GetUserByWindowsLogin(string WindowsLogin)
		{
			return DbHelper2.RunSpInteger("UserGetByWindowsLogin", 
				DbHelper2.mp("@WindowsLogin", SqlDbType.NVarChar, 250, WindowsLogin));
		}
		#endregion

		#region UpdateActivity
		public static void UpdateActivity(int UserId, bool IsActive)
		{
			DbHelper2.RunSp("UserUpdateActivity"
				, DbHelper2.mp("@UserId", SqlDbType.Int, UserId)
				, DbHelper2.mp("@IsActive", SqlDbType.Bit, IsActive)
				);
		}
		#endregion

		#region UpdateActivityInMain
		public static void UpdateActivityInMain(int UserId, bool IsActive)
		{
			DbHelper2.RunSp("ASP_UserUpdateActivity"
				, DbHelper2.mp("@UserId", SqlDbType.Int, UserId)
				, DbHelper2.mp("@IsActive", SqlDbType.Bit, IsActive)
				);
		}
		#endregion

		#region Delete
		public static void Delete(int UserId)
		{
			DbHelper2.RunSp("UserDelete",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region DeleteFromMain
		public static void DeleteFromMain(int UserId)
		{
			DbHelper2.RunSp("ASP_DELETE_USER",
				DbHelper2.mp("@USER_ID", SqlDbType.Int, UserId));
		}
		#endregion

		#region IncreaseStubsVersion
		public static void IncreaseStubsVersion(int UserId)
		{
			DbHelper2.RunSp("ASP_INCREASE_STUBS_VERSION",
				DbHelper2.mp("@user_id", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetUserStatistic
		/// <summary>
		/// TotalUserCount, ActiveUserCount, InactiveUserCount, 
		/// ExternalCount, ExternalActiveCount, ExternalInactiveCount,
		/// PendingCount, SecureGroupCount, AvgCountUserInGroup, PartnerGroupCount,
		/// PartnerUserCount, RegularUserCount, PortalLoginsCount,
		/// ActiveUserTotalCount, InactiveUserTotalCount
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetUserStatistic()
		{
			return DbHelper2.RunSpDataReader("UsersGetStatistic");
		}
		#endregion

		#region GetIMUserStatistics
		/// <summary>
		/// IMGroupCount, AvgCountUserInIMGroup
		/// </summary>
		/// <returns></returns>
		public static DataTable GetIMUserStatistics()
		{
			return DbHelper2.RunSpDataTable("ASP_REP_GET_USER_STATISTIC");
		}
		#endregion

		#region GetListUsersBySubstringForPartnerUser
		/// <summary>
		/// Reader returns fields:
		/// UserId, FirstName, LastName, Email, Login, IsExternal, IsPending
		/// </summary>
		public static IDataReader GetListUsersBySubstringForPartnerUser(int UserId, string SubString)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			SubString = DBCommon.ReplaceSqlWildcard(SubString);
			//

			return DbHelper2.RunSpDataReader("UsersSearchForPartnerUser", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@SubString", SqlDbType.NVarChar, 50, @SubString));
		}
		#endregion

		#region GetListUsersBySubstringForPartnerUserDataTable
		/// <summary>
		/// Reader returns fields:
		/// UserId, FirstName, LastName, Email, Login, IsExternal, IsPending
		/// </summary>
		public static DataTable GetListUsersBySubstringForPartnerUserDataTable(int UserId, string SubString)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			SubString = DBCommon.ReplaceSqlWildcard(SubString);
			//

			return DbHelper2.RunSpDataTable("UsersSearchForPartnerUser", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@SubString", SqlDbType.NVarChar, 50, @SubString));
		}
		#endregion

		#region CheckUserGroup
		public static int CheckUserGroup(int UserId, int GroupId)
		{
			return DbHelper2.RunSpInteger("UserGroupCheck",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@GroupId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetActiveUsersCount
		public static int GetActiveUsersCount(bool isExternal)
		{
			int Counter = 0;
			string cmdText = "SELECT COUNT(*) AS Counter FROM USERS WHERE Activity=3 AND IsExternal = " + (isExternal ? "1" : "0");
			using(IDataReader reader = DbHelper2.RunTextDataReader(cmdText))
			{
				if(reader.Read())
					Counter = (int)reader["Counter"];
			}
			return Counter;
		}
		#endregion

		#region GetListPendingUsers
		/// <summary>
		/// Reader returns fields:
		/// PrincipalId, Login, FirstName, LastName, Email, IMGroupId, OriginalId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListPendingUsers()
		{
			return DbHelper2.RunSpDataReader("UsersGetPending");
		}
		#endregion
 
		#region AddEmail
		public static void AddEmail(int UserId, string Email)
		{
			DbHelper2.RunSp("EmailAdd",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, Email));
		}
		#endregion

		#region DeleteEmail
		public static void DeleteEmail(int EmailId)
		{
			DbHelper2.RunSp("EmailDelete",
				DbHelper2.mp("@EmailId", SqlDbType.Int, EmailId));
		}
		#endregion

		#region GetListEmails
		/// <summary>
		/// Reader returns fields:
		/// EmailId, Email
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEmails(int UserId)
		{
			return DbHelper2.RunSpDataReader("EmailsGet",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListEmailsDataTable
		/// <summary>
		/// Reader returns fields:
		/// EmailId, Email
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEmailsDataTable(int UserId)
		{
			return DbHelper2.RunSpDataTable("EmailsGet",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region EmailSetPrimary
		public static void EmailSetPrimary(int UserId, string Email)
		{
			DbHelper2.RunSp("EmailSetPrimary",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, Email));
		}
		#endregion

		#region UpdateEmail
		public static void UpdateEmail(int EmailId, string Email)
		{
			DbHelper2.RunSp("EmailUpdate",
				DbHelper2.mp("@EmailId", SqlDbType.Int, EmailId),
				DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, Email));
		}
		#endregion

		#region GetEmail
		/// <summary>
		/// Reader returns fields:
		/// EmailId, UserId, Email
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetEmail(int EmailId)
		{
			return DbHelper2.RunSpDataReader("EmailGet",
				DbHelper2.mp("@EmailId", SqlDbType.Int, EmailId));
		}
		#endregion

		#region CheckEmail
		public static int CheckEmail(string Email)
		{
			return DbHelper2.RunSpInteger("EmailCheck",
				DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, Email));
		}
		#endregion

		#region GetListUsersGroupedByRole
		/// <summary>
		/// RoleId, RoleName, UserId, UserName, OpenTasks, CompletedTasks, Issues, IsHeader
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListUsersGroupedByRole()
		{
			return DbHelper2.RunSpDataTable("UsersGetGroupedByRole");
		}
		#endregion

		#region GetListActions
		/// <summary>
		/// ActionType, Counter
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListActions(int UserId)
		{
			return DbHelper2.RunSpDataReader("ActionsGetList",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ReplaceUserInMetaData
		public static void ReplaceUserInMetaData(int OldUserId, int NewUserId)
		{
			DbHelper2.RunSp("mdpsp_sys_ReplaceUser",
				DbHelper2.mp("@OldUserId", SqlDbType.Int, OldUserId),
				DbHelper2.mp("@NewUserId", SqlDbType.Int, NewUserId));
		}
		#endregion

		#region CheckForUnchangeableRoles
		public static int CheckForUnchangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("mdpsp_sys_CheckReplaceUser",
				DbHelper2.mp("@OldUserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region UpdateMenuInAlerts
		public static void UpdateMenuInAlerts(int UserId, bool bShow)
		{
			DbHelper2.RunSp("UserPreferencesUpdateMenuInAlerts",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@MenuInAlerts", SqlDbType.Bit, bShow));
		}
		#endregion

		#region GetMenuInAlerts
		public static bool GetMenuInAlerts(int UserId)
		{
			return (bool)DbHelper2.RunSpScalar("UserPreferenceMenuInAlertsGet",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region UpdateBatchLastSent()
		public static void UpdateBatchLastSent(int userId, DateTime dt)
		{
			DbHelper2.RunSp("UserUpdateBatchLastSent",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@Dt", SqlDbType.DateTime, dt));
		}
		#endregion
		#region UpdateBatchNextSend()
		public static void UpdateBatchNextSend(int userId, DateTime dt)
		{
			DbHelper2.RunSp("UserUpdateBatchNextSend",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@Dt", SqlDbType.DateTime, dt));
		}
		#endregion

		#region GetListAll()
		/// <summary>
		/// Reader returns fields:
		/// UserId, FirstName, LastName, Email, Login, IsExternal, Activity, IsPending
		/// </summary>
		public static IDataReader GetListAll()
		{
			return DbHelper2.RunSpDataReader("UsersGetAll");
		}
		#endregion

		#region GetListActive()
		/// <summary>
		/// Reader returns fields:
		/// PrincipalId, Login, FirstName, LastName, Email, IMGroupId, OriginalId, 
		/// DisplayName, Department
		/// </summary>
		public static IDataReader GetListActive(string sKeyword)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			sKeyword = DBCommon.ReplaceSqlWildcard(sKeyword);
			//

			return DbHelper2.RunSpDataReader("UsersGetActive",
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, sKeyword));
		}

		public static DataTable GetListActiveDataTable(string sKeyword)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			sKeyword = DBCommon.ReplaceSqlWildcard(sKeyword);
			//

			return DbHelper2.RunSpDataTable("UsersGetActive",
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, sKeyword));
		}
		#endregion

		#region GetListActiveManagers()
		/// <summary>
		/// Reader returns fields:
		/// UserId, Login, FirstName, LastName, Email, IMGroupId, OriginalId, CreatedBy, Activity, IsExternal, DisplayName
		/// </summary>
		public static IDataReader GetListActiveManagers()
		{
			return DbHelper2.RunSpDataReader("UsersGetActiveManagers");
		}
		#endregion

	
		#region UpdateWindowsLogin()
		public static void UpdateWindowsLogin(int UserId, string WindowsLogin)
		{
			DbHelper2.RunSp("UserUpdateWindowsLogin",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@WindowsLogin", SqlDbType.NVarChar, 255, WindowsLogin));
		}
		#endregion

		#region GetLdap()
		public static IDataReader GetLdap()
		{
			return DbHelper2.RunSpDataReader("UsersGetLdap");
		}
		#endregion

		#region UpdateMain2
		public static void UpdateMain2(
			int UserId
			, string Login
			, string FirstName
			, string LastName
			, string Email
			)
		{
			DbHelper2.RunSp("ASP_UserUpdate",
				DbHelper2.mp("@user_id", SqlDbType.Int, UserId),
				DbHelper2.mp("@login",SqlDbType.NVarChar, 250, Login),
				DbHelper2.mp("@first_name",SqlDbType.NVarChar, 50, FirstName),
				DbHelper2.mp("@last_name", SqlDbType.NVarChar, 50, LastName),
				DbHelper2.mp("@email", SqlDbType.NVarChar, 250, Email));
		}
		#endregion

		#region Update()
		public static void Update(
			int UserId,
			string Login,
			string FirstName,
			string LastName,
			string Email,
			string WindowsLogin,
			string LdapUid
			)
		{
			DbHelper2.RunSp("UserUpdateMain"
				, DbHelper2.mp("@UserId", SqlDbType.Int, UserId)
				, DbHelper2.mp("@Login", SqlDbType.NVarChar, 250, Login)
				, DbHelper2.mp("@FirstName", SqlDbType.NVarChar, 50, FirstName)
				, DbHelper2.mp("@LastName", SqlDbType.NVarChar, 50, LastName)
				, DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, Email)
				, DbHelper2.mp("@WindowsLogin", SqlDbType.NVarChar, 255, WindowsLogin)
				, DbHelper2.mp("@LdapUid", SqlDbType.NVarChar, 255, LdapUid)
				);
		}
		#endregion

		#region GetListActiveUsersForPartnerUser
		/// <summary>
		/// Gets the list active users for partner user.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <returns>UserId, Login, FirstName, LastName, Email, IsExternal, IsPending</returns>
		public static IDataReader GetListActiveUsersForPartnerUser(int UserId)
		{
			return DbHelper2.RunSpDataReader("UsersGetActiveForPartnerUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListActiveForPartner()
		/// <summary>
		/// Reader returns fields:
		/// PrincipalId, Login, [Password], FirstName, LastName, Email, IMGroupId, OriginalId, DisplayName
		/// </summary>
		public static IDataReader GetListActiveForPartner(int userId)
		{
			return DbHelper2.RunSpDataReader("UsersGetActiveForPartner",
				DbHelper2.mp("@Userid", SqlDbType.Int, userId));
		}

		public static DataTable GetListActiveForPartnerDataTable(int userId)
		{
			return DbHelper2.RunSpDataTable("UsersGetActiveForPartner",
				DbHelper2.mp("@Userid", SqlDbType.Int, userId));
		}
		#endregion

		#region CreateUserTicket
		/// <summary>
		/// Reader returns fields:
		/// PrincipalId, Login, Email, Ticket
		/// </summary>
		public static IDataReader CreateUserTicket(int userId)
		{
			return DbHelper2.RunSpDataReader("UserCreateTicket",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId));
		}
		#endregion

		#region ResetPassword
		/// <summary>
		/// Resetes the user password.
		/// </summary>
		/// <param name="userId">The user id.</param>
		public static void ResetPassword(int userId)
		{
			DbHelper2.RunSp("UserResetPassword",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId));
		}
		#endregion

		#region ResetPasswordInMain
		/// <summary>
		/// Resetes the user password in Main database.
		/// </summary>
		/// <param name="userId">The user id.</param>
		public static void ResetPasswordInMain(int userId)
		{
			DbHelper2.RunSp("ASP_UserResetPassword",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId));
		}
		#endregion

		#region GetUserInfoByRssTicket()
		/// <summary>
		/// Reader returns fields:
		/// UserId, [Login], FirstName, LastName, Email, Activity, IMGroupId, OriginalId, IsExternal,	IsPending,	salt, [hash], [password]
		/// </summary>
		public static IDataReader GetUserInfoByRssKey(Guid rssKey)
		{
			return DbHelper2.RunSpDataReader("UserGetInfoByRssKey",
				DbHelper2.mp("@RssKey", SqlDbType.UniqueIdentifier, rssKey));
		}

		public static DataTable GetUserInfoDataTableByRssKey(Guid rssKey)
		{
			return DbHelper2.RunSpDataTable("UserGetInfoByRssKey",
				DbHelper2.mp("@RssKey", SqlDbType.UniqueIdentifier, rssKey));
		}
		#endregion

		#region GetRssKeyByUserId()
		/// <summary>
		/// Reader returns fields:
		/// RssKey
		/// </summary>
		public static IDataReader GetRssKeyByUserId(int userId)
		{
			return DbHelper2.RunSpDataReader("UserGetRssKey",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId));
		}
		#endregion
	}
}
