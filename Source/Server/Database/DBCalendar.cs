using System;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBCalendar.
	/// </summary>
	public class DBCalendar
	{
		#region Create
		public static int Create(string CalendarName, int TimeZoneId)
		{
			return DbHelper2.RunSpInteger("CalendarCreate",
				DbHelper2.mp("@CalendarName", SqlDbType.NVarChar, 250, CalendarName),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId));
		}

		public static int Create(string CalendarName, int TimeZoneId, int ProjectId)
		{
			return DbHelper2.RunSpInteger("CalendarCreateWithProject",
				DbHelper2.mp("@CalendarName", SqlDbType.NVarChar, 250, CalendarName),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region Update
		public static void Update(int CalendarId, string CalendarName, int TimeZoneId)
		{
			DbHelper2.RunSp("CalendarUpdate",
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId),
				DbHelper2.mp("@CalendarName", SqlDbType.NVarChar, 250, CalendarName), 
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId));
		}
		#endregion

		#region Delete
		public static void Delete(int CalendarId)
		{
			DbHelper2.RunSp("CalendarDelete",
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId));
		}
		#endregion

		#region Copy
		public static void Copy(int FromCalendarId, int ToCalendarId)
		{
			DbHelper2.RunSp("CalendarCopy",
				DbHelper2.mp("@FromCalendarId", SqlDbType.Int, FromCalendarId),
				DbHelper2.mp("@ToCalendarId", SqlDbType.Int, ToCalendarId));
		}
		#endregion

		#region GetCalendar
		/// <summary>
		/// Reader returns fields:
		///		CalendarId, ProjectId, ProjectName, CalendarName, TimeZoneId
		/// </summary>
		public static IDataReader GetCalendar(int CalendarId)
		{
			return DbHelper2.RunSpDataReader("CalendarsGet",
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId));
		}
		#endregion

		#region GetListCalendars
		/// <summary>
		/// Reader returns fields:
		///		CalendarId, ProjectId, ProjectName, CalendarName, TimeZoneId, CanDelete
		/// </summary>
		public static IDataReader GetListCalendars()
		{
			return DbHelper2.RunSpDataReader("CalendarsGet",
				DbHelper2.mp("@CalendarId", SqlDbType.Int, 0));
		}
		#endregion

		#region GetListCalendarsDataTable
		/// <summary>
		/// Reader returns fields:
		///		CalendarId, ProjectId, ProjectName, CalendarName, TimeZoneId, CanDelete
		/// </summary>
		public static DataTable GetListCalendarsDataTable()
		{
			return DbHelper2.RunSpDataTable("CalendarsGet",
				DbHelper2.mp("@CalendarId", SqlDbType.Int, 0));
		}
		#endregion

		#region GetListCalendarsForProject
		/// <summary>
		/// Reader returns fields:
		///		CalendarId, ProjectId, CalendarName, TimeZoneId
		/// </summary>
		public static IDataReader GetListCalendarsForProject(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("CalendarsGetForProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region AddWeekdayHours
		public static void AddWeekdayHours(
			int CalendarId, byte DayOfWeek, int FromTime, int ToTime)
		{
			DbHelper2.RunSp("CalWeekdayAddHours",
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId),
				DbHelper2.mp("@DayOfWeek", SqlDbType.TinyInt, DayOfWeek),
				DbHelper2.mp("@FromTime", SqlDbType.Int, FromTime),
				DbHelper2.mp("@ToTime", SqlDbType.Int, ToTime));
		}
		#endregion

		#region DeleteWeekday
		public static void DeleteWeekday(int CalendarId, byte DayOfWeek)
		{
			DbHelper2.RunSp("CalWeekdayDelete",
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId),
				DbHelper2.mp("@DayOfWeek", SqlDbType.TinyInt, DayOfWeek));
		}
		#endregion

		#region GetListWeekdayHours
		/// <summary>
		/// Reader return fields:
		///  CalendarId, DayOfWeek, FromTime, ToTime 
		/// </summary>
		/// <param name="CalendarId"></param>
		/// <param name="DayOfWeek"></param>
		/// <returns></returns>
		public static IDataReader GetListWeekdayHours(int CalendarId, byte DayOfWeek)
		{
			return DbHelper2.RunSpDataReader("CalWeekdayGetHours",
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId),
				DbHelper2.mp("@DayOfWeek", SqlDbType.TinyInt, DayOfWeek));
		}
		#endregion

		#region AddException
		public static int AddException(int CalendarId, DateTime FromDate, DateTime ToDate)
		{
			return DbHelper2.RunSpInteger("CalExceptionAdd",
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId),
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, FromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, ToDate));
		}
		#endregion

		#region UpdateException
		public static void UpdateException(int ExceptionId, DateTime FromDate, DateTime ToDate)
		{
			DbHelper2.RunSp("CalExceptionUpdate",
				DbHelper2.mp("@ExceptionId", SqlDbType.Int, ExceptionId),
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, FromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, ToDate));
		}
		#endregion

		#region DeleteException
		public static void DeleteException(int ExceptionId)
		{
			DbHelper2.RunSp("CalExceptionDelete",
				DbHelper2.mp("@ExceptionId", SqlDbType.Int, ExceptionId));
		}
		#endregion

		#region GetException
		/// <summary>
		/// Reader return fields:
		///  ExceptionId, CalendarId, FromDate, ToDate 
		/// </summary>
		/// <param name="ExceptionId"></param>
		/// <returns></returns>
		public static IDataReader GetException(int ExceptionId)
		{
			return DbHelper2.RunSpDataReader("CalExceptionGet",
				DbHelper2.mp("@ExceptionId", SqlDbType.Int, ExceptionId));
		}
		#endregion

		#region GetListExceptions
		/// <summary>
		/// Reader return fields:
		///  ExceptionId, CalendarId, FromDate, ToDate 
		/// </summary>
		/// <param name="CalendarId"></param>
		/// <returns></returns>
		public static IDataReader GetListExceptions(int CalendarId)
		{
			return DbHelper2.RunSpDataReader("CalExceptionsGet",
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId));
		}
		#endregion

		#region AddExceptionHours
		public static void AddExceptionHours(
			int ExceptionId, int FromTime, int ToTime)
		{
			DbHelper2.RunSp("CalExceptionHoursAdd",
				DbHelper2.mp("@ExceptionId", SqlDbType.Int, ExceptionId),
				DbHelper2.mp("@FromTime", SqlDbType.Int, FromTime),
				DbHelper2.mp("@ToTime", SqlDbType.Int, ToTime));
		}
		#endregion

		#region DeleteExceptionHours
		public static void DeleteExceptionHours(int ExceptionId)
		{
			DbHelper2.RunSp("CalExceptionHoursDelete",
				DbHelper2.mp("@ExceptionId", SqlDbType.Int, ExceptionId));
		}
		#endregion

		#region GetListExceptionHours
		/// <summary>
		/// Reader return fields:
		///  ExceptionId, FromTime, ToTime 
		/// </summary>
		/// <param name="ExceptionId"></param>
		/// <returns></returns>
		public static IDataReader GetListExceptionHours(int ExceptionId)
		{
			return DbHelper2.RunSpDataReader("CalExceptionHoursGet",
				DbHelper2.mp("@ExceptionId", SqlDbType.Int, ExceptionId));
		}
		#endregion

		#region GetListExceptionHoursByCalendar
		/// <summary>
		/// Reader return fields:
		///  ExceptionId, CalendarId, FromDate, ToDate, FromTime, ToTime
		/// </summary>
		/// <param name="CalendarId"></param>
		/// <returns></returns>
		public static IDataReader GetListExceptionHoursByCalendar(int CalendarId)
		{
			return DbHelper2.RunSpDataReader("CalExceptionHoursGetByCalendar",
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId));
		}
		#endregion

		#region GetCalendarByException
		public static int GetCalendarByException(int ExceptionId)
		{
			return DbHelper2.RunSpInteger("CalExceptionGetCalendar",
				DbHelper2.mp("@ExceptionId", SqlDbType.Int, ExceptionId));
		}
		#endregion

		#region GetDurationByFinishDate
		public static int GetDurationByFinishDate(int CalendarId, DateTime StartDate, DateTime FinishDate)
		{
			return DbHelper2.RunSpInteger("Cal_GetDurationByFinishDate",
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@EndDate", SqlDbType.DateTime, FinishDate));
		}
		#endregion

		#region GetFinishDateByDuration
		public static DateTime GetFinishDateByDuration(int CalendarId, DateTime StartDate, int Duration)
		{
			return DbHelper2.RunSpDateTime("Cal_GetFinishDateByDuration",
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@Duration", SqlDbType.Int, Duration));
		}
		#endregion

		#region GetStartDateByDuration
		public static DateTime GetStartDateByDuration(int CalendarId, DateTime FinishDate, int Duration)
		{
			return DbHelper2.RunSpDateTime("Cal_GetStartDateByDuration",
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate),
				DbHelper2.mp("@Duration", SqlDbType.Int, Duration));
		}
		#endregion

		#region GetCalendarByProject
		public static int GetCalendarByProject(int ProjectId)
		{
			return DbHelper2.RunSpInteger("CalendarGetByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region EmptyCalendar
		public static void EmptyCalendar(int CalendarId)
		{
			DbHelper2.RunSp("CalendarEmpty",
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId));
		}
		#endregion

		#region GetOverlapedExceptions
		public static IDataReader GetOverlapedExceptions(int ExceptionId, DateTime FromDate, DateTime ToDate)
		{
			return DbHelper2.RunSpDataReader("CalExceptionsGetOverlaped",
				DbHelper2.mp("@ExceptionId", SqlDbType.Int, ExceptionId),
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, FromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, ToDate));
		}
		#endregion

		#region CopyExceptionHours
		public static void CopyExceptionHours(int FromExceptionId, int ToExceptionId)
		{
			DbHelper2.RunSp("CalExceptionHoursCopy",
				DbHelper2.mp("@FromExceptionId", SqlDbType.Int, FromExceptionId),
				DbHelper2.mp("@ToExceptionId", SqlDbType.Int, ToExceptionId));
		}
		#endregion

		#region GetUserCalendarByUser
		/// <summary>
		/// Gets the user calendar by user.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <returns>UserCalendarId, CalendarId, TimeZoneId</returns>
		public static IDataReader GetUserCalendarByUser(int userId)
		{
			return DbHelper2.RunSpDataReader("UserCalendarGetByUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId));
		}
		#endregion

		#region GetListUserCalendarExceptions
		/// <summary>
		/// Reader return fields:
		///  ExceptionId, UserCalendarId, FromDate, ToDate 
		/// </summary>
		/// <param name="CalendarId"></param>
		/// <returns></returns>
		public static IDataReader GetListUserCalendarExceptions(int userCalendarId)
		{
			return DbHelper2.RunSpDataReader("UserCalendarExceptionsGet",
				DbHelper2.mp("@UserCalendarId", SqlDbType.Int, userCalendarId));
		}
		#endregion

		#region GetListUserCalendarExceptionHours
		/// <summary>
		/// Reader return fields:
		///  ExceptionId, FromTime, ToTime 
		/// </summary>
		/// <param name="ExceptionId"></param>
		/// <returns></returns>
		public static IDataReader GetListUserCalendarExceptionHours(int exceptionId)
		{
			return DbHelper2.RunSpDataReader("UserCalendarExceptionHoursGet",
				DbHelper2.mp("@ExceptionId", SqlDbType.Int, exceptionId));
		}
		#endregion

		#region GetListObjectsInPriorityOrderUtc
		/// <summary>
		/// Gets the list objects in priority order in UTC time.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <param name="getTasks">if set to <c>true</c> [get tasks].</param>
		/// <param name="getTodo">if set to <c>true</c> [get todo].</param>
		/// <param name="getIncidents">if set to <c>true</c> [get incidents].</param>
		/// <returns>
		/// ObjectId, ObjectTypeId, ObjectName, PriorityId, StateId, 
		/// StartDate (UTC), FinishDate (UTC), CreationDate (UTC), 
		/// TaskTime, PercentCompleted, TaskTimeLeft, FinishDateLeft (UTC), 
		/// IsOverdue, IsNewMessage, AssignmentId, AssignmentName, ProjectId, ProjectTitle
		/// </returns>
		public static DataTable GetListObjectsInPriorityOrderUtc(int userId, DateTime fromDate,
			DateTime toDate, bool getTasks, bool getTodo, bool getIncidents, bool getDocuments)
		{
			return DbHelper2.RunSpDataTable("ObjectsGetInPriorityOrder",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, toDate),
				DbHelper2.mp("@GetTasks", SqlDbType.Bit, getTasks),
				DbHelper2.mp("@GetTodo", SqlDbType.Bit, getTodo),
				DbHelper2.mp("@GetIncidents", SqlDbType.Bit, getIncidents),
				DbHelper2.mp("@GetDocuments", SqlDbType.Bit, getDocuments));
		}
		#endregion

		#region GetListObjectsInPriorityOrder
		/// <summary>
		/// Gets the list objects in priority order.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <param name="getTasks">if set to <c>true</c> [get tasks].</param>
		/// <param name="getTodo">if set to <c>true</c> [get todo].</param>
		/// <param name="getIncidents">if set to <c>true</c> [get incidents].</param>
		/// <param name="timeZoneId">The time zone id.</param>
		/// <returns>ObjectId, ObjectTypeId, ObjectName, PriorityId, StateId, StartDate (local), FinishDate (local), CreationDate (local), TaskTime, PercentCompleted, TaskTimeLeft, FinishDateLeft (local)</returns>
		public static DataTable GetListObjectsInPriorityOrder(int userId, DateTime fromDate,
			DateTime toDate, bool getTasks, bool getTodo, bool getIncidents, int timeZoneId)
		{
			return DbHelper2.RunSpDataTable(timeZoneId,
				new string[] { "StartDate", "FinishDate", "CreationDate", "FinishDateLeft" },
				"ObjectsGetInPriorityOrder",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, toDate),
				DbHelper2.mp("@GetTasks", SqlDbType.Bit, getTasks),
				DbHelper2.mp("@GetTodo", SqlDbType.Bit, getTodo),
				DbHelper2.mp("@GetIncidents", SqlDbType.Bit, getIncidents));
		}
		#endregion

		#region AddUserCalendar
		public static void AddUserCalendar(
			int CalendarId, int UserId)
		{
			DbHelper2.RunSp("UserCalendarAdd",
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region DeleteUserException
		public static void DeleteUserException(int exceptionId)
		{
			DbHelper2.RunSp("UserCalendarExceptionDelete",
				DbHelper2.mp("@ExceptionId", SqlDbType.Int, exceptionId));
		}
		#endregion

		#region GetUserException
		/// <summary>
		/// Reader return fields:
		///  ExceptionId, UserCalendarId, FromDate, ToDate 
		/// </summary>
		/// <param name="ExceptionId"></param>
		/// <returns></returns>
		public static IDataReader GetUserException(int exceptionId)
		{
			return DbHelper2.RunSpDataReader("UserCalendarExceptionGet",
				DbHelper2.mp("@ExceptionId", SqlDbType.Int, exceptionId));
		}
		#endregion

		#region UpdateUserException
		public static void UpdateUserException(int ExceptionId, DateTime FromDate, DateTime ToDate)
		{
			DbHelper2.RunSp("UserCalendarExceptionUpdate",
				DbHelper2.mp("@ExceptionId", SqlDbType.Int, ExceptionId),
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, FromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, ToDate));
		}
		#endregion

		#region DeleteUserExceptionHours
		public static void DeleteUserExceptionHours(int ExceptionId)
		{
			DbHelper2.RunSp("UserCalendarExceptionHoursDelete",
				DbHelper2.mp("@ExceptionId", SqlDbType.Int, ExceptionId));
		}
		#endregion

		#region AddUserExceptionHours
		public static void AddUserExceptionHours(
			int ExceptionId, int FromTime, int ToTime)
		{
			DbHelper2.RunSp("UserCalendarExceptionHoursAdd",
				DbHelper2.mp("@ExceptionId", SqlDbType.Int, ExceptionId),
				DbHelper2.mp("@FromTime", SqlDbType.Int, FromTime),
				DbHelper2.mp("@ToTime", SqlDbType.Int, ToTime));
		}
		#endregion

		#region GetUserCalendarByException
		public static int GetUserCalendarByException(int ExceptionId)
		{
			return DbHelper2.RunSpInteger("UserCalendarByExceptionGet",
				DbHelper2.mp("@ExceptionId", SqlDbType.Int, ExceptionId));
		}
		#endregion

		#region GetOverlapedUserExceptions
		public static IDataReader GetOverlapedUserExceptions(int ExceptionId, DateTime FromDate, DateTime ToDate)
		{
			return DbHelper2.RunSpDataReader("UserCalendarExceptionsGetOverlaped",
				DbHelper2.mp("@ExceptionId", SqlDbType.Int, ExceptionId),
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, FromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, ToDate));
		}
		#endregion

		#region AddUserException
		public static int AddUserException(int userCalendarId, DateTime FromDate, DateTime ToDate)
		{
			return DbHelper2.RunSpInteger("UserCalendarExceptionAdd",
				DbHelper2.mp("@UserCalendarId", SqlDbType.Int, userCalendarId),
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, FromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, ToDate));
		}
		#endregion

		#region CopyUserExceptionHours
		public static void CopyUserExceptionHours(int FromExceptionId, int ToExceptionId)
		{
			DbHelper2.RunSp("UserCalendarExceptionHoursCopy",
				DbHelper2.mp("@FromExceptionId", SqlDbType.Int, FromExceptionId),
				DbHelper2.mp("@ToExceptionId", SqlDbType.Int, ToExceptionId));
		}
		#endregion

		#region === StickedObjects===
		#region DeleteStickedObject
		public static void DeleteStickedObject(int objectTypeId, int objectId, int userId)
		{
			DeleteStickedObject(objectTypeId, objectId, null, userId);
		}

		public static void DeleteStickedObject(int objectTypeId, int objectId, Guid? assignmentId, int userId)
		{
			DbHelper2.RunSp("StickedObjectDelete",
				DbHelper2.mp("@userId", SqlDbType.Int, userId),
				DbHelper2.mp("@objectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@objectTypeId", SqlDbType.Int, objectTypeId),
				DbHelper2.mp("@assignmentId", SqlDbType.UniqueIdentifier, assignmentId));
		}
		#endregion

		#region DeleteStickedObjectForAllUsers
		public static void DeleteStickedObjectForAllUsers(int objectTypeId, int objectId)
		{
			DbHelper2.RunSp("StickedObjectDeleteForAllUsers",
				DbHelper2.mp("@objectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@objectTypeId", SqlDbType.Int, objectTypeId));
		}
		#endregion

		#region GetStickedObjectsCount
		public static int GetStickedObjectsCount(int userId)
		{
			return DbHelper2.RunSpInteger("StickedObjectsGetCount",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId));
		}
		#endregion

		#region GetListStickedObjectsUtc
		public static DataTable GetListStickedObjectsUtc(int userId,
			bool getTasks, bool getTodo, bool getIncidents, bool getDocuments)
		{
			return DbHelper2.RunSpDataTable("StickedObjectsGet",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@GetTasks", SqlDbType.Bit, getTasks),
				DbHelper2.mp("@GetTodo", SqlDbType.Bit, getTodo),
				DbHelper2.mp("@GetIncidents", SqlDbType.Bit, getIncidents),
				DbHelper2.mp("@getDocuments", SqlDbType.Bit, getDocuments));
		}
		#endregion

		#region CheckStickedObject
		public static bool CheckStickedObject(int userId, int objectId, int objectTypeId, Guid? assignmentId)
		{
			int retval = DbHelper2.RunSpInteger("StickedObjectCheck",
				DbHelper2.mp("@userId", SqlDbType.Int, userId),
				DbHelper2.mp("@objectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@objectTypeId", SqlDbType.Int, objectTypeId),
				DbHelper2.mp("@assignmentId", SqlDbType.UniqueIdentifier, assignmentId));

			return (retval > 0) ? true : false;
		}
		#endregion

		#region AddStickedObject
		public static void AddStickedObject(int userId, int objectId, int objectTypeId, Guid? assignmentId, int position)
		{
			DbHelper2.RunSp("StickedObjectAdd",
				DbHelper2.mp("@userId", SqlDbType.Int, userId),
				DbHelper2.mp("@objectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@objectTypeId", SqlDbType.Int, objectTypeId),
				DbHelper2.mp("@assignmentId", SqlDbType.UniqueIdentifier, assignmentId),
				DbHelper2.mp("@position", SqlDbType.Int, position));
		}
		#endregion
		#endregion
	}
}
