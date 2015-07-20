using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBReport.
	/// </summary>
	public class DBReport
	{
		#region SearchHistory
		public static IDataReader SearchHistory(DateTime fromDate, DateTime toDate, 
			int userID, int contUserID, int msgType, string keyword, bool displayLastEventsFirst, int DefaultBias)
		{
			//if userId=contUserId=0 -> all with all
			int origUserId=0;
			int origContUserId=0;
			if(userID!=0)
			{
				origUserId=DbHelper2.RunSpInteger("UserGetOriginalId",
					DbHelper2.mp("@UserId", SqlDbType.Int, userID));
			}	
			if(contUserID!=0)
			{
				origContUserId=DbHelper2.RunSpInteger("UserGetOriginalId",
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
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias*60));
		}
		#endregion

		#region GetChats
		public static IDataReader GetChats(int iUserId)
		{
			int origUserId=0;
			if(iUserId!=0)
			{
				origUserId=DbHelper2.RunSpInteger("UserGetOriginalId",
					DbHelper2.mp("@UserId", SqlDbType.Int, iUserId));
				if (origUserId==-1) origUserId=0;
			}	

			return DbHelper2.RunSpDataReader("ASP_GET_CHATS", 
				DbHelper2.mp("@USER_ID", SqlDbType.Int, origUserId));
		}
		#endregion

		#region GetUsersByCompany
		public static IDataReader GetUsersByCompany()
		{
			return DbHelper2.RunSpDataReader("ASP_GET_USERS_BY_COMPANY");
		}
		#endregion

		#region GetSessions
		public static IDataReader GetSessions(DateTime StartDate, DateTime EndDate, int DefaultBias)
		{
			return DbHelper2.RunSpDataReader("ASP_REP_GET_SESSIONS",  
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate), 
				DbHelper2.mp("@EndDate", SqlDbType.DateTime, EndDate),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias*60));
		}
		#endregion

		#region GetGroupsCount
		public static int GetGroupsCount()
		{
			return DbHelper2.RunSpInteger("ASP_REP_GET_TOTAL_GROUPS");
		}
		#endregion

		#region GetUsersCount
		public static int GetUsersCount()
		{
			return DbHelper2.RunSpInteger("ASP_REP_GET_TOTAL_USERS");
		}
		#endregion

		#region GetActiveUsersCount
		public static int GetActiveUsersCount()
		{
			return DbHelper2.RunSpInteger("ASP_REP_GET_TOTAL_ACTIVE_USERS");
		}
		#endregion

		#region GetInActiveUsersCount
		public static int GetInActiveUsersCount()
		{
			return DbHelper2.RunSpInteger("ASP_REP_GET_TOTAL_INACTIVE_USERS");
		}
		#endregion
	
		#region GetCountIMMessages
		public static int GetCountIMMessages(DateTime StartDate, DateTime EndDate, bool getInternal, int DefaultBias)
		{
			int ret = 0;
			using(IDataReader reader = DbHelper2.RunSpDataReader("ASP_REP_GET_IM_MESSAGES",
				DbHelper2.mp("@startdate", SqlDbType.DateTime, StartDate), 
				DbHelper2.mp("@enddate", SqlDbType.DateTime, EndDate),
				DbHelper2.mp("@use_dates",SqlDbType.Bit, 1),
				DbHelper2.mp("@internal",SqlDbType.Bit, getInternal ? 1 : 0),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias*60)))
			{
				if(reader.Read())
					ret = (int)reader["MessagesSent"];
			}
			return ret;
		}
		#endregion

		#region GetCountChatMessages
		public static int GetCountChatMessages(DateTime StartDate,DateTime EndDate, int DefaultBias)
		{
			return DbHelper2.RunSpInteger("ASP_REP_GET_CHAT_MESSAGES",
				DbHelper2.mp("@startdate", SqlDbType.DateTime, StartDate), 
				DbHelper2.mp("@enddate", SqlDbType.DateTime, EndDate),
				DbHelper2.mp("@total",SqlDbType.Int,0),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias*60));
		}
		#endregion

		#region GetFilesTransferred
		public static int GetFilesTransferred(DateTime StartDate,DateTime EndDate, int DefaultBias)
		{
			return DbHelper2.RunSpInteger("ASP_REP_GET_FILES_TRANSFERRED",
				DbHelper2.mp("@startdate", SqlDbType.DateTime, StartDate), 
				DbHelper2.mp("@enddate", SqlDbType.DateTime, EndDate),
				DbHelper2.mp("@total",SqlDbType.Int,0),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias*60));
		}
		#endregion

		#region GetAuthenticatedUsers
		public static int GetAuthenticatedUsers(DateTime StartDate, DateTime EndDate, int DefaultBias)
		{
			return DbHelper2.RunSpInteger("ASP_REP_GET_AUTH_USERS",
				DbHelper2.mp("@startdate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@enddate", SqlDbType.DateTime, EndDate),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias*60));
		}
		#endregion

		#region GetConferencesCreated
		public static int GetConferencesCreated(DateTime StartDate, int DefaultBias)
		{
			return DbHelper2.RunSpInteger("ASP_REP_GET_CONFERENCES_CREATED",
				DbHelper2.mp("@startdate", SqlDbType.DateTime, StartDate), 
				DbHelper2.mp("@enddate", SqlDbType.DateTime, DateTime.Now),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias*60));
		}
		#endregion

		#region GetTop10Users
		public static IDataReader GetTop10Users(DateTime StartDate,DateTime EndDate, int DefaultBias)
		{
			return DbHelper2.RunSpDataReader("ASP_REP_GET_TOP10_WRITERS",
				DbHelper2.mp("@startdate", SqlDbType.DateTime, StartDate), 
				DbHelper2.mp("@enddate", SqlDbType.DateTime, EndDate),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias*60));
		}
		#endregion

		#region GetIMGroupStats
		/// <summary>
		/// Returns fields: AccActive, AccInactive, MsgIM, MsgConf,
		/// TransfFiles, TransfBytes, Confs, Projects, FilesPub, CalEntries.
		/// </summary>
		public static IDataReader GetGroupStats(int IMGroupId, DateTime fromDate, DateTime toDate, int DefaultBias)
		{
			return DbHelper2.RunSpDataReader("ASP_REP_GET_GROUP_STATS",
				DbHelper2.mp("@imgroup_id", SqlDbType.Int, IMGroupId),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias*60),
				DbHelper2.mp("@fromdate", SqlDbType.DateTime, fromDate), 
				DbHelper2.mp("@todate", SqlDbType.DateTime, toDate));
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
			int origUserId=0;
			if(UserID!=0)
			{
				origUserId=DbHelper2.RunSpInteger("UserGetOriginalId",
					DbHelper2.mp("@UserId", SqlDbType.Int, UserID));
				if (origUserId==-1) origUserId=0;
			}	
			return DbHelper2.RunSpDataReader("ASP_REP_GET_USER_STATS",
				DbHelper2.mp("@user_id", SqlDbType.Int, origUserId),
				DbHelper2.mp("@fromdate", SqlDbType.DateTime, fromDate), 
				DbHelper2.mp("@todate", SqlDbType.DateTime, toDate),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias*60));
		}
		#endregion

		#region GetSecureGroupStats
		public static IDataReader GetSecGroupStats(int GroupID, DateTime fromDate, DateTime toDate, bool ShowAll)
		{  
			return DbHelper2.RunSpDataReader("ReportGroupStatistics",
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@EndDate", SqlDbType.DateTime, toDate),
				DbHelper2.mp("@GroupId", SqlDbType.Int, GroupID),
				DbHelper2.mp("@ShowAll", SqlDbType.Bit, ShowAll));
		}
		#endregion

		#region SearchChat
		public static IDataReader SearchChat(int iUserId, int iChatId, DateTime dtFrom, DateTime dtTo, 
			string sKeyWord, int iOrder, int DefaultBias)
		{
			int origUserId=0;
			if(iUserId!=0)
			{
				origUserId=DbHelper2.RunSpInteger("UserGetOriginalId",
					DbHelper2.mp("@UserId", SqlDbType.Int, iUserId));
				if (origUserId==-1) origUserId=0;
			}	
			return DbHelper2.RunSpDataReader("ASP_SEARCH_CHAT", 
				DbHelper2.mp("@User_id", SqlDbType.Int, origUserId), 
				DbHelper2.mp("@Chat_id", SqlDbType.Int, iChatId), 
				DbHelper2.mp("@DT_Begin", SqlDbType.DateTime, dtFrom), 
				DbHelper2.mp("@DT_End", SqlDbType.DateTime, dtTo),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 50, sKeyWord), 
				DbHelper2.mp("@Order", SqlDbType.Int, iOrder), 
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, DefaultBias*60));
		}
		#endregion
	
		#region GetListAllTasksForUser
		/// <summary>
		/// Reader contains fields:
		///  TaskId, Title, ProjectId, ProjectTitle, ManagerName, StartDate, FinishDate, 
		///  PercentCompleted, IsCompleted, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListAllTasksForUser(int UserId, int ProjectId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"TasksGetAllForUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListAllToDoForUserByProject
		/// <summary>
		/// DataTable contains fields:
		///  ToDoId, Title, Description, PriorityId, PriorityName, StartDate, FinishDate, PercentCompleted
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListAllToDoForUserByProject(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"ToDoGetAllForUserByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
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
		
		#region GetListAllEventsForUserByProject
		/// <summary>
		/// Reader returns fields:
		///  EventId, Title, Description, PriorityId, PriorityName, TypeId, StartDate, FinishDate, 
		///  StateId, HasRecurrence
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListAllEventsForUserByProject(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"EventsGetAllForUserByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetXMLReport
		public static XmlDocument GetXMLReport(string sql)
		{
			return DbHelper2.RunTextXmlDocument(sql);
		}
		#endregion

		#region GetQuickUsageStatsReport
		/// <summary>
		/// ActiveProjects, CalendarEntries, TasksCompleted, IssuesResolved
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetQuickUsageStatsReport(int UserId)
		{
			return DbHelper2.RunSpDataReader("ReportQuickUsageStats",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetQuickUsageStatsReport2
		/// <summary>
		/// MessSent, MessReceived
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetQuickUsageStatsReport2(int OriginalUserId)
		{
			return DbHelper2.RunSpDataReader("ASP_REP_GET_QUICK_USAGE_STATS",
				DbHelper2.mp("@user_id", SqlDbType.Int, OriginalUserId));
		}
		#endregion

		#region GetToDoAndTaskTrackingReport
		/// <summary>
		/// Total, Completed, PastDue, Active, Upcoming
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetToDoAndTaskTrackingReport(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("ReportToDoAndTaskTracking",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetProjectStatisticReport
		/// <summary>
		/// Discussions, Events, Tasks, ToDo, Incidents, Resources
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetProjectStatisticReport(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("ReportProjectStatistic",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetProjectTeamBreakdownReport
		/// <summary>
		/// ProjectId, UserId, FirstName, LastName, Code, Rate, Minutes
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetProjectTeamBreakdownReport(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("ReportProjectTeamBreakdown",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetProjectTeamBreakdownReportDataTable
		/// <summary>
		/// ProjectId, UserId, FirstName, LastName, Code, Rate, Minutes, TotalMinutes
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectTeamBreakdownReportDataTable(int ProjectId)
		{
			return DbHelper2.RunSpDataTable("ReportProjectTeamBreakdown",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetProjectStatisticByUserReport
		/// <summary>
		/// IncidentsCreated, IncidentsModified, IncidentsClosed, 
		/// Events, Discussions
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetProjectStatisticByUserReport(int ProjectId, int UserId)
		{
			return DbHelper2.RunSpDataReader("ReportProjectStatisticByUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetHelpDeskIssuesSnapshotReport
		/// <summary>
		/// HDM, TotalIssues, ProjectIssues, GeneralIssues
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetHelpDeskIssuesSnapshotReport()
		{
			return DbHelper2.RunSpDataReader("ReportHelpDeskIssuesSnapshot");
		}
		#endregion

		#region GetUsageStats
		/// <summary>
		/// IMLogins, MessSent, MessReceived, IMGroups
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetUsageStats()
		{
			return DbHelper2.RunSpDataReader("ASP_REP_GET_USAGE_STATS");
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

		#region LogPortalLogin
		public static void LogPortalLogin(int UserId, string IP)
		{
			DbHelper2.RunSp("PortalLoginAdd",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LoginTime", SqlDbType.DateTime, DateTime.UtcNow),
				DbHelper2.mp("@IP", SqlDbType.VarChar, 15, IP));
		}
		#endregion

		#region GetListReportsCreators
		/// <summary>
		/// Reader contains fields:
		/// UserId, FirstName, LastName, FullName, FullName2
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListReportsCreators()
		{
			return DbHelper2.RunSpDataReader("ReportGetAuthors");
		}
		#endregion

		#region GetListReportsModifiers
		/// <summary>
		/// Reader contains fields:
		/// UserId, FirstName, LastName, FullName, FullName2
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListReportsModifiers()
		{
			return DbHelper2.RunSpDataReader("ReportGetModifiers");
		}
		#endregion

		#region CreateReportTemplate
		public static int CreateReportTemplate(string Name, int CreatorId, 
			DateTime dtCreated, string RepTemplate, bool IsGlobal, bool IsTemporary)
		{
			return DbHelper2.RunSpInteger("ReportTemplateAdd",
				DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, Name),
				DbHelper2.mp("@CreationDate", SqlDbType.DateTime, dtCreated),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@IsGlobal", SqlDbType.Bit, IsGlobal),
				DbHelper2.mp("@IsTemporary", SqlDbType.Bit, IsTemporary),
				DbHelper2.mp("@ReportTemplate", SqlDbType.NText, RepTemplate));
		}
		#endregion

		#region UpdateReportTemplate
		public static void UpdateReportTemplate(int TemplateId, string Name, int EditorId, 
			DateTime dtUpdated, string RepTemplate, bool IsGlobal, bool IsTemporary)
		{
			DbHelper2.RunSp("ReportTemplateUpdate",
				DbHelper2.mp("@TemplateId", SqlDbType.Int, TemplateId),
				DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, Name),
				DbHelper2.mp("@LastSavedDate", SqlDbType.DateTime, dtUpdated),
				DbHelper2.mp("@LastEditorId", SqlDbType.Int, EditorId),
				DbHelper2.mp("@IsGlobal", SqlDbType.Bit, IsGlobal),
				DbHelper2.mp("@IsTemporary", SqlDbType.Bit, IsTemporary),
				DbHelper2.mp("@ReportTemplate", SqlDbType.NText, RepTemplate));
		}
		#endregion

		#region CreateReportByTemplate
		public static int CreateReportByTemplate(int ReportId, int CreatorId, DateTime dtCreated, byte[] RepXML)
		{
			return DbHelper2.RunSpInteger("ReportHistoryAdd",
				DbHelper2.mp("@ReportId", SqlDbType.Int, ReportId),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@CreationDate", SqlDbType.DateTime, dtCreated),
				DbHelper2.mp("@ResultXML", SqlDbType.Image, RepXML));
		}
		#endregion

		#region GetReport
		/// <summary>
		/// DataReader contains fields:
		///  TemplateId, ReportCreated, ReportCreator, Name, IsGlobal, 
		///  TemplateCreated, TemplateModified, TemplateCreatorId, TemplateModifierId, 
		///  TemplateXML
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetReport(int ReportItemId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"ReportCreated", "TemplateCreated", "TemplateModified"},
				"ReportGet",
				DbHelper2.mp("@ReportItemId", SqlDbType.Int, ReportItemId));
		}
		#endregion

		#region GetReportBinaryData
		public static IDataReader GetReportBinaryData(int ReportItemId)
		{
			return DbHelper2.RunSpDataReaderBlob("ReportGetBinaryData",
				DbHelper2.mp("@ReportItemId", SqlDbType.Int, ReportItemId));
		}
		#endregion
		
		#region GetReportTemplatesByFilter
		/// <summary>
		/// DataReader contains fields:
		///  TemplateId, TemplateName, TemplateCreated, TemplateModified, TemplateCreatorId, TemplateModifierId, TemplateXML
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetReportTemplatesByFilter(int CreatorId, 
			int TimeZoneId, DateTime StartDate, DateTime FinishDate, int LastEditorId,
			int UserId)
		{
			object oStartDate = (StartDate == DateTime.MinValue) ? (object)DBNull.Value : StartDate;
			object oFinishDate = (FinishDate == DateTime.MaxValue) ? (object)DBNull.Value : FinishDate;
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"TemplateCreated", "TemplateModified"},
				"ReportTemplatesGetByFilter",
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@LastEditorId", SqlDbType.Int, LastEditorId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, oStartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, oFinishDate));
		}
		#endregion

		#region GetReportTemplatesByFilterDataTable
		/// <summary>
		/// DataReader contains fields:
		///  TemplateId, TemplateName, TemplateCreated, TemplateModified, TemplateCreatorId, TemplateModifierId, TemplateXML
		/// </summary>
		/// <returns></returns>
		public static DataTable GetReportTemplatesByFilterDataTable(int CreatorId,
			int TimeZoneId, DateTime StartDate, DateTime FinishDate, int LastEditorId,
			int UserId)
		{
			object oStartDate = (StartDate == DateTime.MinValue) ? (object)DBNull.Value : StartDate;
			object oFinishDate = (FinishDate == DateTime.MaxValue) ? (object)DBNull.Value : FinishDate;
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"TemplateCreated", "TemplateModified"},
				"ReportTemplatesGetByFilter",
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@LastEditorId", SqlDbType.Int, LastEditorId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, oStartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, oFinishDate));
		}
		#endregion

		#region GetReportTemplate
		/// <summary></summary>
		/// <returns>
		/// DataReader contains fields:
		///  TemplateId, Name, TemplateCreated, TemplateModified, TemplateCreatorId, TemplateModifierId, TemplateXML
		/// </returns>
		public static IDataReader GetReportTemplate(int ReportTemplateId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"TemplateCreated", "TemplateModified"},
				"ReportTemplateGet",
				DbHelper2.mp("@ReportTemplateId", SqlDbType.Int, ReportTemplateId));
		}
		#endregion

		#region GetReportsByTemplateId
		/// <summary></summary>
		/// <returns>
		/// DataReader contains fields:
		///  ReportItemId, ReportId, CreationDate, CreatorId
		/// </returns>
		public static DataTable GetReportsByTemplateId(int TemplateId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate"},
				"ReportsGetByTemplateId",
				DbHelper2.mp("@TemplateId", SqlDbType.Int, TemplateId));
		}
		#endregion

		#region CheckForUnchangeableRoles
		public static int CheckForUnchangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("ReportsCheckForUnchangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ReplaceUnchangeableUser
		public static void ReplaceUnchangeableUser(int FromUserId, int ToUserId)
		{
			DbHelper2.RunSp("ReportsReplaceUnchangeableUser", 
				DbHelper2.mp("@FromUserId", SqlDbType.Int, FromUserId),
				DbHelper2.mp("@ToUserId", SqlDbType.Int, ToUserId));
		}
		#endregion

		#region GetQDictionary
		public static IDataReader GetQDictionary(string query)
		{
			return DbHelper2.RunTextDataReader(query);
		}
		#endregion

		#region DeleteReportTemplate
		/// <summary>
		/// </summary>
		/// <returns></returns>
		public static void DeleteReportTemplate(int TemplateId)
		{
			DbHelper2.RunSp("ReportTemplateDelete",
				DbHelper2.mp("@TemplateId", SqlDbType.Int, TemplateId));
		}
		#endregion

		#region DeleteReportItem
		/// <summary>
		/// </summary>
		/// <returns></returns>
		public static void DeleteReportItem(int ReportItemId)
		{
			DbHelper2.RunSp("ReportHistoryDelete",
				DbHelper2.mp("@ReportItemId", SqlDbType.Int, ReportItemId));
		}
		#endregion

		#region DeleteTemporaryTemplates
		/// <summary>
		/// </summary>
		/// <returns></returns>
		public static void DeleteTemporaryTemplates()
		{
			DbHelper2.RunSp("ReportTemplateDeleteTemporary");
		}
		#endregion

		#region GetOnlineUsersCount
		public static int GetOnlineUsersCount()
		{  
			return DbHelper2.RunSpInteger("ASP_REP_GET_ONLINE_USERS_COUNT");
		}
		#endregion

		#region GetUserLoginsClient
		/// <summary>
		/// </summary>
		public static int GetUserLoginsClient(int UserID)
		{  
			int origUserId=0;
			if(UserID!=0)
			{
				origUserId=DbHelper2.RunSpInteger("UserGetOriginalId",
					DbHelper2.mp("@UserId", SqlDbType.Int, UserID));
				if (origUserId==-1) origUserId=0;
			}
			return DbHelper2.RunSpInteger("ASP_REP_GET_USER_LOGIN_CLIENTS",
				DbHelper2.mp("@user_id", SqlDbType.Int, origUserId));
		}
		#endregion

		#region GetStatusLog
		/// <summary>
		/// Gets the Status Log
		/// </summary>
		/// <param name="imGroupId"></param>
		/// <param name="userId"></param>
		/// <param name="fromDate"></param>
		/// <param name="toDate"></param>
		/// <param name="timeZoneId"></param>
		/// <returns>user_id, dt, status, first_name, last_name</returns>
		public static IDataReader GetStatusLog(int imGroupId, int userId, DateTime fromDate, DateTime toDate, int timeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				timeZoneId, new string[] { "dt" },
				"ASP_REP_GET_STATUS_LOG",
				DbHelper2.mp("@imGroupId", SqlDbType.Int, imGroupId),
				DbHelper2.mp("@userId", SqlDbType.Int, userId),
				DbHelper2.mp("@fromDate", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@toDate", SqlDbType.DateTime, toDate));
		}
		#endregion
	}
}
