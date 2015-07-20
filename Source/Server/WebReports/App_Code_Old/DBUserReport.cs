using System;
using System.Data;
//using Mediachase.IBN.Database;

using DbHelper2 = Mediachase.IBN.Database.DbHelper2;

namespace Mediachase.IBN.DefaultUserReports
{
	/// <summary>
	/// Summary description for DBUserReport.
	/// </summary>
	public class DBUserReport
	{
		#region GetCountIMMessages
		public static int GetCountIMMessages(DateTime StartDate, DateTime EndDate, bool getInternal, int DefaultBias)
		{
			int ret = 0;
			using (IDataReader reader = DbHelper2.RunSpDataReader("ASP_REP_GET_IM_MESSAGES",
					  DbHelper2.mp("@startdate", SqlDbType.DateTime, StartDate),
					  DbHelper2.mp("@enddate", SqlDbType.DateTime, EndDate),
					  DbHelper2.mp("@use_dates", SqlDbType.Bit, 1),
					  DbHelper2.mp("@internal", SqlDbType.Bit, getInternal ? 1 : 0),
					  DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias * 60)))
			{
				if (reader.Read())
					ret = (int)reader["MessagesSent"];
			}
			return ret;
		}
		#endregion

		#region GetQuickSnapshotReport
		public static IDataReader GetQuickSnapshotReport(DateTime StartDate, DateTime EndDate, int CreatorId)
		{
			return DbHelper2.RunSpDataReader("ReportQuickSnapshot",
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@EndDate", SqlDbType.DateTime, EndDate),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId));
		}
		#endregion

		#region GetUsersByCompany
		public static IDataReader GetUsersByCompany()
		{
			return DbHelper2.RunSpDataReader("ASP_GET_USERS_BY_COMPANY");
		}
		#endregion

		#region GetMostActiveGroupsByPortalLoginsReport
		/// <summary>
		/// DisplayName, Count
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetMostActiveGroupsByPortalLoginsReport(DateTime FromDate, DateTime ToDate)
		{
			return DbHelper2.RunSpDataReader("ReportMostActiveGroupsByPortalLogins",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, FromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, ToDate));
		}
		#endregion

		#region GetMostActiveGroupsByProjectReport
		/// <summary>
		/// DisplayName, Count
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetMostActiveGroupsByProjectReport(DateTime FromDate, DateTime ToDate)
		{
			return DbHelper2.RunSpDataReader("ReportMostActiveGroupsByProject",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, FromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, ToDate));
		}
		#endregion

		#region GetMostActiveGroupsByIncidentReport
		/// <summary>
		/// DisplayName, Count
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetMostActiveGroupsByIncidentReport(DateTime FromDate, DateTime ToDate)
		{
			return DbHelper2.RunSpDataReader("ReportMostActiveGroupsByIncident",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, FromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, ToDate));
		}
		#endregion

		#region GetMostActiveGroupsByEventReport
		/// <summary>
		/// DisplayName, Count
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetMostActiveGroupsByEventReport(DateTime FromDate, DateTime ToDate)
		{
			return DbHelper2.RunSpDataReader("ReportMostActiveGroupsByEvent",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, FromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, ToDate));
		}
		#endregion

		#region GetMostActiveGroupsByTaskReport
		/// <summary>
		/// DisplayName, Count
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetMostActiveGroupsByTaskReport(DateTime FromDate, DateTime ToDate)
		{
			return DbHelper2.RunSpDataReader("ReportMostActiveGroupsByTask",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, FromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, ToDate));
		}
		#endregion

		#region GetMostActiveGroupsByToDoReport
		/// <summary>
		/// DisplayName, Count
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetMostActiveGroupsByToDoReport(DateTime FromDate, DateTime ToDate)
		{
			return DbHelper2.RunSpDataReader("ReportMostActiveGroupsByToDo",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, FromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, ToDate));
		}
		#endregion

		#region GetMostActiveGroupsByAssetReport
		/// <summary>
		/// DisplayName, Count
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetMostActiveGroupsByAssetReport(DateTime FromDate, DateTime ToDate)
		{
			return DbHelper2.RunSpDataReader("ReportMostActiveGroupsByAsset",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, FromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, ToDate));
		}
		#endregion

		#region GetMostActiveGroupsByAssetVersionReport
		/// <summary>
		/// DisplayName, Count
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetMostActiveGroupsByAssetVersionReport(DateTime FromDate, DateTime ToDate)
		{
			return DbHelper2.RunSpDataReader("ReportMostActiveGroupsByAssetVersion",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, FromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, ToDate));
		}
		#endregion

		#region GetCountChatMessages
		public static int GetCountChatMessages(DateTime StartDate, DateTime EndDate, int DefaultBias)
		{
			return DbHelper2.RunSpInteger("ASP_REP_GET_CHAT_MESSAGES",
				DbHelper2.mp("@startdate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@enddate", SqlDbType.DateTime, EndDate),
				DbHelper2.mp("@total", SqlDbType.Int, 0),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias * 60));
		}
		#endregion

		#region GetFilesTransferred
		public static int GetFilesTransferred(DateTime StartDate, DateTime EndDate, int DefaultBias)
		{
			return DbHelper2.RunSpInteger("ASP_REP_GET_FILES_TRANSFERRED",
				DbHelper2.mp("@startdate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@enddate", SqlDbType.DateTime, EndDate),
				DbHelper2.mp("@total", SqlDbType.Int, 0),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias * 60));
		}
		#endregion

		#region GetAuthenticatedUsers
		public static int GetAuthenticatedUsers(DateTime StartDate, DateTime EndDate, int DefaultBias)
		{
			return DbHelper2.RunSpInteger("ASP_REP_GET_AUTH_USERS",
				DbHelper2.mp("@startdate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@enddate", SqlDbType.DateTime, EndDate),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias * 60));
		}
		#endregion

		#region GetUserStats
		/// <summary>
		/// LoginsClient, MsgSent, MsgReceived, ConfCreated,
		/// ConfMsgSent, FilesSent, FSBytes, FilesReceived, FRBytes,
		/// FilesPublished, FPBytes, CalEntries, Projects, CreateDate.
		/// </summary>
		public static IDataReader GetUserStats(int UserID, DateTime fromDate, DateTime toDate, int DefaultBias)
		{
			int origUserId = 0;
			if (UserID != 0)
			{
				origUserId = DbHelper2.RunSpInteger("UserGetOriginalId",
					DbHelper2.mp("@UserId", SqlDbType.Int, UserID));
				if (origUserId == -1) origUserId = 0;
			}
			return DbHelper2.RunSpDataReader("ASP_REP_GET_USER_STATS",
				DbHelper2.mp("@user_id", SqlDbType.Int, origUserId),
				DbHelper2.mp("@fromdate", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@todate", SqlDbType.DateTime, toDate),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias * 60));
		}
		#endregion

		#region GetTop10Users
		public static IDataReader GetTop10Users(DateTime StartDate, DateTime EndDate, int DefaultBias)
		{
			return DbHelper2.RunSpDataReader("ASP_REP_GET_TOP10_WRITERS",
				DbHelper2.mp("@startdate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@enddate", SqlDbType.DateTime, EndDate),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias * 60));
		}
		#endregion

		#region GetListUsers
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IsActive, IMGroupId, OriginalId
		/// </summary>
		public static IDataReader GetListUsers(int GroupId)
		{
			return DbHelper2.RunSpDataReader("UsersGetByIMGroup",
				DbHelper2.mp("@IMGroupId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetIMGroup
		/// <summary>
		/// Reader returns fields:
		///		IMGroupId, IMGroupName, color, logo_version, is_partner
		/// </summary>
		public static IDataReader GetIMGroup(int IMGroupId, bool IncludeInternal)
		{
			return DbHelper2.RunSpDataReader("ASP_GET_IMGROUPS",
				DbHelper2.mp("@imgroup_id", SqlDbType.Int, IMGroupId),
				DbHelper2.mp("@include_internal", SqlDbType.Bit, IncludeInternal));
		}
		#endregion

		#region GetListIMSessionsByUserAndDate
		public static IDataReader GetListIMSessionsByUserAndDate(int UserId, DateTime StartDate, DateTime EndDate, int Bias)
		{
			return DbHelper2.RunSpDataReader("ASP_REP_GET_USER_IM_SESSIONS",
				DbHelper2.mp("@user_id", SqlDbType.Int, UserId),
				DbHelper2.mp("@fromdate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@todate", SqlDbType.DateTime, EndDate),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, Bias * 60));
		}
		#endregion

		#region GetGroup
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, IMGroupId
		/// </summary>
		public static IDataReader GetGroup(int GroupId)
		{
			return DbHelper2.RunSpDataReader("GroupGet",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetParentGroup
		public static int GetParentGroup(int GroupId)
		{
			return DbHelper2.RunSpInteger("GroupGetParent",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetListChildGroups
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, HasChildren
		/// </summary>
		public static IDataReader GetListChildGroups(int GroupId)
		{
			return DbHelper2.RunSpDataReader("GroupsGetByParent",
				DbHelper2.mp("@GroupId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetListSecureGroupExplicit
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName
		/// </summary>
		public static IDataReader GetListSecureGroupExplicit(int UserId)
		{
			return DbHelper2.RunSpDataReader("GroupsGetByUser",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListActiveUsersInGroup
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IMGroupId, CreatedBy
		/// </summary>
		public static IDataReader GetListActiveUsersInGroup(int GroupId)
		{
			return DbHelper2.RunSpDataReader("UsersGetActiveByGroup",
				DbHelper2.mp("@GroupId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetUserInfo
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IsActive, IMGroupId, CreatedBy, Comments, IsExternal, IsPending, OriginalId
		/// </summary>
		public static IDataReader GetUserInfo(int UserId)
		{
			return DbHelper2.RunSpDataReader("UserGet",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetSecureGroupStats
		/// <summary>
		/// Gets the sec group stats.
		/// </summary>
		/// <param name="GroupID">The group ID.</param>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <param name="ShowAll">if set to <c>true</c> [show all].</param>
		/// <returns>ActiveAccounts, InactiveAccounts, NewProjectsCount, NewEventsCount, NewIncidentsCount, NewToDosCount, NewTasksCount, PortalLogins, ReOpenIncidentsCount</returns>
		public static IDataReader GetSecGroupStats(int GroupID, DateTime fromDate, DateTime toDate, bool ShowAll)
		{
			return DbHelper2.RunSpDataReader("ReportGroupStatistics",
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@EndDate", SqlDbType.DateTime, toDate),
				DbHelper2.mp("@GroupId", SqlDbType.Int, GroupID),
				DbHelper2.mp("@ShowAll", SqlDbType.Bit, ShowAll));
		}
		#endregion

		#region GetChats
		public static IDataReader GetChats(int iUserId)
		{
			int origUserId = 0;
			if (iUserId != 0)
			{
				origUserId = DbHelper2.RunSpInteger("UserGetOriginalId",
					DbHelper2.mp("@UserId", SqlDbType.Int, iUserId));
				if (origUserId == -1) origUserId = 0;
			}

			return DbHelper2.RunSpDataReader("ASP_GET_CHATS",
				DbHelper2.mp("@USER_ID", SqlDbType.Int, origUserId));
		}
		#endregion

		#region SearchChat
		public static IDataReader SearchChat(int iUserId, int iChatId, DateTime dtFrom, DateTime dtTo,
			string sKeyWord, int iOrder, int DefaultBias)
		{
			int origUserId = 0;
			if (iUserId != 0)
			{
				origUserId = DbHelper2.RunSpInteger("UserGetOriginalId",
					DbHelper2.mp("@UserId", SqlDbType.Int, iUserId));
				if (origUserId == -1) origUserId = 0;
			}
			return DbHelper2.RunSpDataReader("ASP_SEARCH_CHAT",
				DbHelper2.mp("@User_id", SqlDbType.Int, origUserId),
				DbHelper2.mp("@Chat_id", SqlDbType.Int, iChatId),
				DbHelper2.mp("@DT_Begin", SqlDbType.DateTime, dtFrom),
				DbHelper2.mp("@DT_End", SqlDbType.DateTime, dtTo),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 50, sKeyWord),
				DbHelper2.mp("@Order", SqlDbType.Int, iOrder),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias * 60));
		}
		#endregion

		#region GetListAllUsersInGroup
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IMGroupId, CreatedBy, Activity, IsExternal
		/// </summary>
		public static IDataReader GetListAllUsersInGroup(int GroupId)
		{
			return DbHelper2.RunSpDataReader("UsersGetByGroupAll",
				DbHelper2.mp("@GroupId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetListGroupsByPartner
		/// <summary>
		/// Reader returns fields:
		///		PartnerId, GroupId, GroupName, Level (=1)
		/// </summary>
		public static IDataReader GetListGroupsByPartner(int PartnerId, bool IncludeCurrent, bool IncludeEveryone)
		{
			return DbHelper2.RunSpDataReader("PartnerGroupGetListByPartner",
				DbHelper2.mp("@PartnerId", SqlDbType.Int, PartnerId),
				DbHelper2.mp("@IncludeCurrent", SqlDbType.Bit, IncludeCurrent),
				DbHelper2.mp("@IncludeEveryone", SqlDbType.Bit, IncludeEveryone));
		}
		#endregion

		#region GetListGroupsAsTree
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, Level
		/// </summary>
		public static IDataReader GetListGroupsAsTree()
		{
			return DbHelper2.RunSpDataReader("GroupsGetAsTree2");
		}
		#endregion

		#region GetGroupForPartnerUser
		public static int GetGroupForPartnerUser(int UserId)
		{
			return DbHelper2.RunSpInteger("GroupGetForPartnerUser",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListGroups
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName
		/// </summary>
		public static IDataReader GetListGroups()
		{
			return DbHelper2.RunSpDataReader("GroupGet",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, 0));
		}
		#endregion

		#region SearchHistory
		public static IDataReader SearchHistory(
			DateTime fromDate,
			DateTime toDate,
			int userID,
			int contUserID,
			int msgType,
			string keyword,
			bool displayLastEventsFirst,
			int DefaultBias)
		{
			//if userId=contUserId=0 -> all with all
			int origUserId = 0;
			int origContUserId = 0;
			if (userID != 0)
			{
				origUserId = DbHelper2.RunSpInteger("UserGetOriginalId",
					DbHelper2.mp("@UserId", SqlDbType.Int, userID));
			}
			if (contUserID != 0)
			{
				origContUserId = DbHelper2.RunSpInteger("UserGetOriginalId",
					DbHelper2.mp("@UserId", SqlDbType.Int, contUserID));
			}

			return DbHelper2.RunSpDataReader("ASP_SEARCH_HISTORY",
				DbHelper2.mp("@User_id", SqlDbType.Int, origUserId),
				DbHelper2.mp("@Cont_user_id", SqlDbType.Int, origContUserId),
				DbHelper2.mp("@DT_Begin", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@DT_End", SqlDbType.DateTime, toDate),
				DbHelper2.mp("@Mess_Type", SqlDbType.Int, msgType),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 50, keyword),
				DbHelper2.mp("@Order", SqlDbType.Int, displayLastEventsFirst ? 1 : 0),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias * 60));
		}
		#endregion

		#region GetListAvailableGroups
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName
		/// </summary>
		public static IDataReader GetListAvailableGroups()
		{
			return DbHelper2.RunSpDataReader("GroupsGetAvailable");
		}
		#endregion

		#region GetListAvailableGroupsForPartner
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName
		/// </summary>
		public static IDataReader GetListAvailableGroupsForPartner(int UserId)
		{
			return DbHelper2.RunSpDataReader("GroupsGetAvailableForPartner",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetUserInfoByOriginalId
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, Activity, IMGroupId
		/// </summary>
		public static IDataReader GetUserInfoByOriginalId(int OriginalId)
		{
			return DbHelper2.RunSpDataReader("UserGetByOriginalId",
				DbHelper2.mp("@OriginalId", SqlDbType.Int, OriginalId));
		}
		#endregion

		#region GetMenuInAlerts
		public static bool GetMenuInAlerts(int UserId)
		{
			return (bool)DbHelper2.RunSpScalar("UserPreferenceMenuInAlertsGet",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region Message_GetByUserIdAndTimeFilter
		public static IDataReader Message_GetByUserIdAndTimeFilter(int userId, DateTime startDate, DateTime endDate, int timeZoneId)
		{
			return DbHelper2.RunSpDataReader(timeZoneId, new string[] { "Sent" }, "Message_GetByUserIdAndTimeFilter",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, startDate),
				DbHelper2.mp("@EndDate", SqlDbType.DateTime, endDate));
		}
		#endregion
	}
}
