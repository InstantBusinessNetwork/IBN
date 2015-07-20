using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business
{
	public class Root
	{
		#region CheckActiveUser
		public static int CheckActiveUser(string sid)
		{
			return DBRoot.CheckActiveUser(sid);
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
			return DBRoot.GetUserInfo(iUserId, iInfoType);
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
			return DBRoot.SearchUsers(sLogin, sFirstName, sLastName, sEmail);
		} 
		#endregion

		#region GetStubsVersion
		public static int GetStubsVersion(int currentUserId)
		{
			return DBRoot.GetStubsVersion(currentUserId);
		} 
		#endregion

		#region GetGroupByUser
		public static int GetGroupByUser(int iUserId)
		{
			return DBRoot.GetGroupByUser(iUserId);
		} 
		#endregion

		#region GetAllStubsForUser
		public static IDataReader GetAllStubsForUser(int origUserId)
		{
			return DBRoot.GetAllStubsForUser(origUserId);
		} 
		#endregion

		#region GetBinaryStubIcon
		public static IDataReader GetBinaryStubIcon(int iStubId)
		{
			return DBRoot.GetBinaryStubIcon(iStubId);
		} 
		#endregion

		#region GetLogoByGroup
		public static IDataReader GetLogoByGroup(int iGroupId)
		{
			return DBRoot.GetLogoByGroup(iGroupId);
		} 
		#endregion

		#region GetBinaryClientLogo
		public static IDataReader GetBinaryClientLogo(int iGroupId)
		{
			return DBRoot.GetBinaryClientLogo(iGroupId);
		} 
		#endregion

		#region GetUserDetails
		public static IDataReader GetUserDetails(int origUserId)
		{
			return DBRoot.GetUserDetails(origUserId);
		} 
		#endregion

		#region GetUserGroups
		public static IDataReader GetUserGroups(int origUserId)
		{
			return DBRoot.GetUserGroups(origUserId);
		} 
		#endregion

		#region GetChatIdByCUID
		public static int GetChatIdByCUID(string sChatId)
		{
			return DBRoot.GetChatIdByCUID(sChatId);
		} 
		#endregion

		#region GetChatDetails
		/// <summary>
		/// Reader returns fields:
		///		chat_id, [name], [desc], begin_time, owner_id, first_name, last_name, mess_count
		/// </summary>
		public static IDataReader GetChatDetails(int iChatId)
		{
			return DBRoot.GetChatDetails(iChatId);
		} 
		#endregion

		#region GetChatUsers
		/// <summary>
		/// Reader returns fields:
		///		[user_id], accepted, exited, user_status, first_name, last_name
		/// </summary>
		public static IDataReader GetChatUsers(int iChatId)
		{
			return DBRoot.GetChatUsers(iChatId);
		} 
		#endregion
	}
}
