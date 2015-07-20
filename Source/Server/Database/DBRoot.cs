using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBRoot.
	/// </summary>
	public class DBRoot
	{
		#region CheckActiveUser
		public static int CheckActiveUser(string sid)
		{
			return DbHelper2.RunSpInteger("OM_CHECK_ACTIVE_USER",
				DbHelper2.mp("@sid", SqlDbType.Char, 36, sid));
		}
		#endregion

		#region GetUserInfo
		/// <summary>
		/// Reader returns fields:
		///		@INFO_TYPE=1: U.[user_id], U.login, U.first_name, U.last_name, U.eMail, @STATUS AS [status], 
		///		U.imgroup_id, R.imgroup_name, U.status_time as [time]
		/// </summary>
		public static IDataReader GetUserInfo(int iUserId, int iInfoType)
		{
			return DbHelper2.RunSpDataReader("OM_GET_USER_INFO",
				DbHelper2.mp("@USER_ID", SqlDbType.Int, iUserId),
				DbHelper2.mp("@INFO_TYPE", SqlDbType.Int, iInfoType),
				DbHelper2.mp("@TIME", SqlDbType.Int, 0));
		} 
		#endregion

		#region SearchUsers
		/// <summary>
		/// Reader returns fields:
		///		[user_id], login, first_name, last_name, email 
		/// </summary>
		public static IDataReader SearchUsers(
			string sLogin,
			string sFirstName,
			string sLastName,
			string sEmail)
		{
			return DbHelper2.RunSpDataReader("OM_SEARCH_USERS",
				DbHelper2.mp("@lg", SqlDbType.NVarChar, 50, sLogin),
				DbHelper2.mp("@fn", SqlDbType.NVarChar, 50, sFirstName),
				DbHelper2.mp("@ln", SqlDbType.NVarChar, 50, sLastName),
				DbHelper2.mp("@em", SqlDbType.NVarChar, 50, sEmail));
		} 
		#endregion

		#region GetStubsVersion
		public static int GetStubsVersion(int currentUserId)
		{
			return DbHelper2.RunSpInteger("ASP_GET_STUBS_VERSION",
				DbHelper2.mp("@user_id", SqlDbType.Int, currentUserId));
		} 
		#endregion

		#region GetGroupByUser
		public static int GetGroupByUser(int iUserId)
		{
			return DbHelper2.RunSpInteger("ASP_GET_IMGROUP_BY_USER",
				DbHelper2.mp("@USER_ID", SqlDbType.Int, iUserId));
		} 
		#endregion

		#region GetAllStubsForUser
		public static IDataReader GetAllStubsForUser(int origUserId)
		{
			int iUserId = 0;
			using (IDataReader reader = DbHelper2.RunSpDataReader("UserGetByOriginalId",
					DbHelper2.mp("@OriginalId", SqlDbType.Int, origUserId)))
			{
				if (reader.Read())
					iUserId = (int)reader["UserId"];
			}
			return DbHelper2.RunSpDataReader("StubsGetVisibleForUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, iUserId));
		} 
		#endregion

		#region GetBinaryStubIcon
		public static IDataReader GetBinaryStubIcon(int iStubId)
		{
			return DbHelper2.RunSpDataReaderBlob("StubGetIcon",
				DbHelper2.mp("@StubId", SqlDbType.Int, iStubId));
		} 
		#endregion

		#region GetLogoByGroup
		public static IDataReader GetLogoByGroup(int iGroupId)
		{
			return DbHelper2.RunSpDataReader("ASP_GET_LOGO_BY_IMGROUP",
				DbHelper2.mp("@imgroup_id", SqlDbType.Int, iGroupId));
		} 
		#endregion

		#region GetBinaryClientLogo
		public static IDataReader GetBinaryClientLogo(int iGroupId)
		{
			return DbHelper2.RunSpDataReaderBlob("ASP_GET_BINARY_CLIENT_LOGO",
				DbHelper2.mp("@imgroup_id", SqlDbType.Int, iGroupId));
		} 
		#endregion

		#region GetUserDetails
		public static IDataReader GetUserDetails(int origUserId)
		{
			int iUserId = 0;
			using (IDataReader reader = DbHelper2.RunSpDataReader("UserGetByOriginalId",
					DbHelper2.mp("@OriginalId", SqlDbType.Int, origUserId)))
			{
				if (reader.Read())
					iUserId = (int)reader["UserId"];
			}
			return DbHelper2.RunSpDataReader("UserDetailsGet",
				DbHelper2.mp("@userid", SqlDbType.Int, iUserId));
		} 
		#endregion

		#region GetUserGroups
		public static IDataReader GetUserGroups(int origUserId)
		{
			int iUserId = 0;
			using (IDataReader reader = DbHelper2.RunSpDataReader("UserGetByOriginalId",
					DbHelper2.mp("@OriginalId", SqlDbType.Int, origUserId)))
			{
				if (reader.Read())
					iUserId = (int)reader["UserId"];
			}
			return DbHelper2.RunSpDataReader("GroupsGetByUser",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, iUserId));
		} 
		#endregion

		#region GetChatIdByCUID
		public static int GetChatIdByCUID(string sChatId)
		{
			return DbHelper2.RunSpInteger("ASP_GET_CHAT_ID_BY_CUID",
				DbHelper2.mp("@cuid", SqlDbType.Char, 36, sChatId));
		} 
		#endregion

		#region GetChatDetails
		/// <summary>
		/// Reader returns fields:
		///		chat_id, [name], [desc], begin_time, owner_id, first_name, last_name, mess_count
		/// </summary>
		public static IDataReader GetChatDetails(int iChatId)
		{
			return DbHelper2.RunSpDataReader("ASP_GET_CHAT_DETAILS",
				DbHelper2.mp("@CHAT_ID", SqlDbType.Int, iChatId));
		} 
		#endregion

		#region GetChatUsers
		/// <summary>
		/// Reader returns fields:
		///		[user_id], accepted, exited, user_status, first_name, last_name
		/// </summary>
		public static IDataReader GetChatUsers(int iChatId)
		{
			return DbHelper2.RunSpDataReader("ASP_GET_CHAT_USERS",
				DbHelper2.mp("@CHAT_ID", SqlDbType.Int, iChatId));
		} 
		#endregion
	}
}
