using System;
using System.Data;
using System.Data.SqlClient;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBEvent.
	/// </summary>
	public class DBEvent
	{
		#region Create
		public static int Create(
			object ProjectId, int CreatorId, int ManagerId, 
			string Title, string Description,	string Location,
			DateTime CreationDate, DateTime StartDate, DateTime FinishDate,
			int PriorityId, int TypeId, int TaskTime,
			object contactUid,
			object orgUid)
		{
			return DbHelper2.RunSpInteger("EventCreate",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, Title),
				DbHelper2.mp("@Description", SqlDbType.NText, Description),
				DbHelper2.mp("@Location", SqlDbType.NVarChar, 1000, Location),
				DbHelper2.mp("@CreationDate", SqlDbType.DateTime, CreationDate),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId),
				DbHelper2.mp("@TypeId", SqlDbType.Int, TypeId),
				DbHelper2.mp("@TaskTime", SqlDbType.Int, TaskTime),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion

		#region Delete
		public static void Delete(int EventId)
		{
			DbHelper2.RunSp("EventDelete", 
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId));
		}
		#endregion

		#region GetEvent
		/// <summary>
		/// Reader returns fields:
		///  EventId, ProjectId, ProjectTitle, CreatorId, ManagerId,
		///  Title, Description, Location, CreationDate, StartDate, 
		///  FinishDate, PriorityId, PriorityName, TypeId, TypeName, ReminderInterval, 
		///  StateId, HasRecurrence, ContactUid, OrgUid, ClientName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetEvent(int EventId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate", "StartDate", "FinishDate"},
				"EventGet",
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetEventInUTC
		/// <summary>
		/// Reader returns fields:
		///  EventId, ProjectId, ProjectTitle, CreatorId, ManagerId, 
		///  Title, Description, Location, CreationDate, StartDate, 
		///  FinishDate, PriorityId, PriorityName, TypeId, TypeName, ReminderInterval, 
		///  StateId, TaskTime, HasRecurrence
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetEventInUTC(int EventId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader("EventGetInUTC",
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetEventDates
		/// <summary>
		/// Reader returns fields:
		///  EventId, StartDate, FinishDate
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetEventDates(int EventId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"EventGetDates",
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId));
		}
		#endregion

		#region GetManager
		public static int GetManager(int eventId)
		{
			return DbHelper2.RunSpInteger("EventManagerGet",
				DbHelper2.mp("@EventId", SqlDbType.Int, eventId));
		}
		#endregion

		#region GetListEventTypes
		/// <summary>
		/// Reader returns fields:
		///  TypeId, TypeName 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventTypes(int LanguageId)
		{
			return DbHelper2.RunSpDataReader("EventTypesGet",
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListResources
		/// <summary>
		/// Reader returns fields:
		///  ResourceId, EventId, PrincipalId, IsGroup, MustBeConfirmed, ResponsePending, 
		///  IsConfirmed, IsExternal
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListResources(int EventId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"LastSavedDate"},
				"EventResourcesGet", 
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId));
		}
		#endregion

		#region GetListEventResourcesDataTable
		/// <summary>
		///  ResourceId, EventId, PrincipalId, IsGroup, MustBeConfirmed, ResponsePending, 
		///  IsConfirmed, IsExternal
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEventResourcesDataTable(int EventId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"LastSavedDate"},
				"EventResourcesGet", 
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId));
		}
		#endregion

		#region AddResource
		public static void AddResource(int EventId, int PrincipalId, bool MustBeConfirmed)
		{
			DbHelper2.RunSp("EventResourceAdd",
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, MustBeConfirmed));
		}
		#endregion

		#region DeleteResource
		public static void DeleteResource(int EventId, int PrincipalId)
		{
			DbHelper2.RunSp("EventResourceDelete",
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId));
		}
		#endregion

		#region GetListEventsByProject
		/// <summary>
		/// Reader returns fields:
		///  EventId, ProjectId, CreatorId, ManagerId, LastEditorId, 
		///  Title, Description, Location, CreationDate, LastSavedDate, StartDate, 
		///  FinishDate, PriorityId, PriorityName, TypeId, TypeName, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventsByProject(int ProjectId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate", "StartDate", "FinishDate"},
				"EventsGetByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListEventsByProjectDataTable
		/// <summary>
		/// Reader returns fields:
		///  EventId, ProjectId, CreatorId, ManagerId, LastEditorId, 
		///  Title, Description, Location, CreationDate, LastSavedDate, StartDate, 
		///  FinishDate, PriorityId, PriorityName, TypeId, TypeName, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEventsByProjectDataTable(int ProjectId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate", "StartDate", "FinishDate"},
				"EventsGetByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetSecurityForUser
		/// <summary>
		/// Reader return fields:
		///  EventId, PrincipalId, IsResource, IsManager
		/// </summary>
		/// <param name="EventId"></param>
		/// <param name="UserId"></param>
		/// <returns></returns>
		public static IDataReader GetSecurityForUser(int EventId, int UserId)
		{
			return DbHelper2.RunSpDataReader("EventGetSecurityForUser",
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListEventsByFilter
		/// <summary>
		///  Reader returns fields:
		///		EventId, TypeId, Title, Description, StartDate, FinishDate, CanEdit, CanDelete, 
		///		ManagerId, PriorityId, Interval,Location, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventsByFilter(int ProjectId, int UserId, 
			int ResourceId, int TimeZoneId, DateTime StartDate, DateTime FinishDate, 
			bool GetAssigned, bool GetManaged, bool GetCreated, string Keyword,
			object contactUid, object orgUid)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"EventsGetByFilter",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ResourceId", SqlDbType.Int, ResourceId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate), 
				DbHelper2.mp("@GetAssigned", SqlDbType.Bit, GetAssigned),
				DbHelper2.mp("@GetManaged", SqlDbType.Bit, GetManaged),
				DbHelper2.mp("@GetCreated", SqlDbType.Bit, GetCreated),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, Keyword),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@orgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion

		#region GetListEventsSimple
		/// <summary>
		///  Reader returns fields:
		///		EventId, Title, TypeId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventsSimple(int ProjectId, 
			int UserId, bool GetAssigned, bool GetManaged)
		{
			return DbHelper2.RunSpDataReader("EventsGetSimple",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@GetAssigned", SqlDbType.Bit, GetAssigned),
				DbHelper2.mp("@GetManaged", SqlDbType.Bit, GetManaged));
		}
		#endregion

		#region GetProject
		public static int GetProject(int EventId)
		{
			return DbHelper2.RunSpInteger("EventGetProject", 
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId));
		}
		#endregion

		#region GetSharingLevel
		public static int GetSharingLevel(int UserId, int EventId)
		{
			return DbHelper2.RunSpInteger("SharingGetLevelForEvent", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId));
		}
		#endregion

		#region GetResourceByUser
		/// <summary>
		/// Reader returns fields:
		///	 MustBeConfirmed, ResponsePending, IsConfirmed
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetResourceByUser(int EventId, int UserId)
		{
			return DbHelper2.RunSpDataReader("EventResourceGetByUser",
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListPendingEvents
		/// <summary>
		///		EventId, Title, Location, PriorityId, PriorityName, TypeId, ManagerId, 
		///		StartDate, FinishDate, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListPendingEvents(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"EventsGetPending",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListPendingEventsDataTable
		/// <summary>
		///		EventId, Title, Location, PriorityId, PriorityName, TypeId, ManagerId, StartDate, FinishDate
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListPendingEventsDataTable(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"EventsGetPending",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListEventResourcesNotPending
		/// <summary>
		/// Reader returns fields:
		///  EventId, Title, PrincipalId, IsConfirmed, LastSavedDate, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventResourcesNotPending(int UserId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"LastSavedDate", "StartDate", "FinishDate"},
				"EventResourcesGetNotPending", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId)	);
		}
		#endregion

		#region GetListEventsForUser
		/// <summary>
		///  Reader returns fields:
		///		EventId, Title, Location, TypeId, StartDate, FinishDate, 
		///		Description, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventsForUser(int UserId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"EventsGetForUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListEventsForScheduling
		/// <summary>
		///  Reader returns fields:
		///		UserId, EventId, TypeId, StartDate, FinishDate, StateId, HasRecurrence
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventsForScheduling(int UserId, 
			int TimeZoneId, DateTime StartDate, DateTime FinishDate)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"EventsGetForScheduling",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate));
		}
		#endregion

		#region GetListEventsForEventScheduling
		/// <summary>
		///  Reader returns fields:
		///		UserId, EventId, TypeId, StartDate, FinishDate, StateId, HasRecurrence
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventsForEventScheduling(int EventId, 
			int TimeZoneId, DateTime StartDate, DateTime FinishDate)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"EventsGetForEventScheduling",
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate));
		}
		#endregion

		#region DeleteByProject
		public static void DeleteByProject(int ProjectId)
		{
			DbHelper2.RunSp("EventDeleteByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListUsersForEvent
		/// <summary>
		/// Reader returns fields:
		///  UserId, FirstName, LastName, Email, Login
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListUsersForEvent(int EventId)
		{
			return DbHelper2.RunSpDataReader("EventSecurityGetByEvent",
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId));
		}
		#endregion

		#region GetListUsersForEventDataTable
		/// <summary>
		///  UserId, FirstName, LastName, Email, Login
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListUsersForEventDataTable(int EventId)
		{
			return DbHelper2.RunSpDataTable("EventSecurityGetByEvent",
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId));
		}
		#endregion

		#region GetListEventsUpdatedByUser
		/// <summary>
		///		EventId, Title, Location, TypeId, StartDate, FinishDate, LastSavedDate, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventsUpdatedByUser(int ProjectId, int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "LastSavedDate"},
				"EventsGetUpdatedByUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListEventsUpdatedByUserDataTable
		/// <summary>
		///		EventId, Title, Location, TypeId, StartDate, FinishDate, LastSavedDate
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEventsUpdatedByUserDataTable(int ProjectId, int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "LastSavedDate"},
				"EventsGetUpdatedByUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListEventsUpdatedForUser
		/// <summary>
		///		EventId, Title, Location, TypeId, StartDate, FinishDate, 
		///		LastEditorId, LastSavedDate, ProjectId, ProjectName, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventsUpdatedForUser(int ProjectId, int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "LastSavedDate"},
				"EventsGetUpdatedForUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListEventsUpdatedForUserDataTable
		/// <summary>
		///		EventId, Title, Location, TypeId, StartDate, FinishDate, 
		///		LastEditorId, LastSavedDate, ProjectId, ProjectName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEventsUpdatedForUserDataTable(int ProjectId, int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "LastSavedDate"},
				"EventsGetUpdatedForUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListEventsByKeyword
		/// <summary>
		///  Reader returns fields:
		///		EventId, Title, Description, Location, TypeId, StartDate, FinishDate, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventsByKeyword(int UserId, int TimeZoneId, string Keyword)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"EventsGetByKeyword",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, Keyword));
		}
		#endregion

		#region CheckForChangeableRoles
		public static int CheckForChangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("EventsCheckForChangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region CheckForUnchangeableRoles
		public static int CheckForUnchangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("EventsCheckForUnchangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListEventsForChangeableRoles
		/// <summary>
		/// Reader returns fields:
		///  EventId, Title, IsManager, IsResource, CanView, CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventsForChangeableRoles(int UserId, int CurrentUserId)
		{
			return DbHelper2.RunSpDataReader("EventsGetForChangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@CurrentUserId", SqlDbType.Int, CurrentUserId));
		}
		#endregion

		#region ReplaceUnchangeableUserToManager
		public static void ReplaceUnchangeableUserToManager(int UserId)
		{
			DbHelper2.RunSp("EventsReplaceUnchangeableUserToManager", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ReplaceUnchangeableUser
		public static void ReplaceUnchangeableUser(int FromUserId, int ToUserId)
		{
			DbHelper2.RunSp("EventsReplaceUnchangeableUser", 
				DbHelper2.mp("@FromUserId", SqlDbType.Int, FromUserId),
				DbHelper2.mp("@ToUserId", SqlDbType.Int, ToUserId));
		}
		#endregion

		#region GetListEventManagers
		/// <summary>
		/// UserId, FirstName, LastName, FullName, FullName2
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventManagers()
		{
			return DbHelper2.RunSpDataReader("EventGetManagers");
		}
		#endregion

		#region GetListEventManagersDataTable
		/// <summary>
		/// UserId, FirstName, LastName, FullName, FullName2
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEventManagersDataTable()
		{
			return DbHelper2.RunSpDataTable("EventGetManagers");
		}
		#endregion

		#region GetListEventCreators
		/// <summary>
		/// UserId, FirstName, LastName, FullName, FullName2
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventCreators()
		{
			return DbHelper2.RunSpDataReader("EventGetCreators");
		}
		#endregion

		#region GetListEventCreatorsDataTable
		/// <summary>
		/// UserId, FirstName, LastName, FullName, FullName2
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEventCreatorsDataTable()
		{
			return DbHelper2.RunSpDataTable("EventGetCreators");
		}
		#endregion

		#region GetTotalCountWithType()
		/// <summary>
		/// Reader returns fields:
		///		Type, Count.
		/// </summary>
		public static IDataReader GetTotalCountWithType(int UserId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader("EventsGetTotalCountWithType",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListEventsForManagerViewDataTable
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate, 
		///	PercentCompleted, IsCompleted, ManagerId, ReasonId, ProjectId, ProjectTitle,
		///	StateId, CompletionTypeId,
		///	TaskTime, TotalMinutes, TotalApproved, HasRecurrence, ContactUid, OrgUid, ClientName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEventsForManagerViewDataTable(int PrincipalId,
			int TimeZoneId, int LanguageId, int ManagerId, int ProjectId, int categoryId, 
			bool ShowActive,
			DateTime dtCompleted1, DateTime dtCompleted2, 
			DateTime dtUpcoming1, DateTime dtUpcoming2,
			DateTime dtCreated1, DateTime dtCreated2, 
			PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			object obj_dtCompleted1 = (dtCompleted1 <= DateTime.MinValue.AddDays(1))? null: (object)dtCompleted1;
			object obj_dtCompleted2 = (dtCompleted2 <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCompleted2;
			object obj_dtUpcoming1 = (dtUpcoming1 >= DateTime.MaxValue.AddDays(-1))? null: (object)dtUpcoming1;
			object obj_dtUpcoming2 = (dtUpcoming2 >= DateTime.MaxValue.AddDays(-1)) ? null : (object)dtUpcoming2;
			object obj_dtCreated1 = (dtCreated1 <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCreated1;
			object obj_dtCreated2 = (dtCreated2 <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCreated2;
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate", "StartDate", "FinishDate"},
				"EventsGetForManagerView",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@CategoryId", SqlDbType.Int, categoryId),
				DbHelper2.mp("@ShowActive", SqlDbType.Bit, ShowActive),
				DbHelper2.mp("@CompletedDate1", SqlDbType.DateTime, obj_dtCompleted1),
				DbHelper2.mp("@CompletedDate2", SqlDbType.DateTime, obj_dtCompleted2),
				DbHelper2.mp("@StartDate1", SqlDbType.DateTime, obj_dtUpcoming1),
				DbHelper2.mp("@StartDate2", SqlDbType.DateTime, obj_dtUpcoming2),
				DbHelper2.mp("@CreationDate1", SqlDbType.DateTime, obj_dtCreated1),
				DbHelper2.mp("@CreationDate2", SqlDbType.DateTime, obj_dtCreated2),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid));
		}
		#endregion

		#region GetListEventsForManagerViewWithCategories
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate, 
		///	PercentCompleted, IsCompleted, ManagerId, ReasonId, ProjectId, ProjectTitle,
		///	StateId, CompletionTypeId,
		///	TaskTime, TotalMinutes, TotalApproved, HasRecurrence, ContactUid, OrgUid, ClientName,
		///	CategoryId, CategoryName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEventsForManagerViewWithCategories(int PrincipalId,
			int TimeZoneId, int LanguageId, int ManagerId, int ProjectId, int categoryId,
			bool ShowActive,
			DateTime dtCompleted1, DateTime dtCompleted2,
			DateTime dtUpcoming1, DateTime dtUpcoming2,
			DateTime dtCreated1, DateTime dtCreated2,
			PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			object obj_dtCompleted1 = (dtCompleted1 <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCompleted1;
			object obj_dtCompleted2 = (dtCompleted2 <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCompleted2;
			object obj_dtUpcoming1 = (dtUpcoming1 >= DateTime.MaxValue.AddDays(-1)) ? null : (object)dtUpcoming1;
			object obj_dtUpcoming2 = (dtUpcoming2 >= DateTime.MaxValue.AddDays(-1)) ? null : (object)dtUpcoming2;
			object obj_dtCreated1 = (dtCreated1 <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCreated1;
			object obj_dtCreated2 = (dtCreated2 <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCreated2;
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "CreationDate", "StartDate", "FinishDate" },
				"EventsGetForManagerViewWithCategories",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@CategoryId", SqlDbType.Int, categoryId),
				DbHelper2.mp("@ShowActive", SqlDbType.Bit, ShowActive),
				DbHelper2.mp("@CompletedDate1", SqlDbType.DateTime, obj_dtCompleted1),
				DbHelper2.mp("@CompletedDate2", SqlDbType.DateTime, obj_dtCompleted2),
				DbHelper2.mp("@StartDate1", SqlDbType.DateTime, obj_dtUpcoming1),
				DbHelper2.mp("@StartDate2", SqlDbType.DateTime, obj_dtUpcoming2),
				DbHelper2.mp("@CreationDate1", SqlDbType.DateTime, obj_dtCreated1),
				DbHelper2.mp("@CreationDate2", SqlDbType.DateTime, obj_dtCreated2),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid));
		}
		#endregion

		#region GetListEventsForResourceViewDataTable
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate, 
		/// TaskTime, TotalMinutes, TotalApproved,
		///	PercentCompleted, IsCompleted, ManagerId, ReasonId, ProjectId, StateId, CompletionTypeId,
		///	HasRecurrence, ContactUid, OrgUid, ClientName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEventsForResourceViewDataTable(int userId,
			int timeZoneId, int languageId, int managerId, int projectId, int categoryId,
			bool showActive, DateTime dtCompleted, DateTime dtUpcoming, 
			PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			object obj_dtCompleted = (dtCompleted <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCompleted;
			object obj_dtUpcoming = (dtUpcoming >= DateTime.MaxValue.AddDays(-1)) ? null : (object)dtUpcoming;
			return DbHelper2.RunSpDataTable(
				timeZoneId, new string[] { "CreationDate", "StartDate", "FinishDate" },
				"EventsGetForResourceView",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, languageId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, managerId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@CategoryId", SqlDbType.Int, categoryId),
				DbHelper2.mp("@ShowActive", SqlDbType.Bit, showActive),
				DbHelper2.mp("@CompletedDate", SqlDbType.DateTime, obj_dtCompleted),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, obj_dtUpcoming),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid));
		}
		#endregion

		#region GetListEventsForResourceViewWithCategories
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate, 
		/// TaskTime, TotalMinutes, TotalApproved,
		///	PercentCompleted, IsCompleted, ManagerId, ReasonId, ProjectId, StateId, CompletionTypeId,
		///	HasRecurrence, ContactUid, OrgUid, ClientName, CategoryId, CategoryName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEventsForResourceViewWithCategories(int userId,
			int timeZoneId, int languageId, int managerId, int projectId, int categoryId,
			bool showActive, DateTime dtCompleted, DateTime dtUpcoming,
			PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			object obj_dtCompleted = (dtCompleted <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCompleted;
			object obj_dtUpcoming = (dtUpcoming >= DateTime.MaxValue.AddDays(-1)) ? null : (object)dtUpcoming;
			return DbHelper2.RunSpDataTable(
				timeZoneId, new string[] { "CreationDate", "StartDate", "FinishDate" },
				"EventsGetForResourceViewWithCategories",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, languageId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, managerId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@CategoryId", SqlDbType.Int, categoryId),
				DbHelper2.mp("@ShowActive", SqlDbType.Bit, showActive),
				DbHelper2.mp("@CompletedDate", SqlDbType.DateTime, obj_dtCompleted),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, obj_dtUpcoming),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid));
		}
		#endregion

		#region RecalculateState
		/// <summary>
		/// OldState, NewState
		/// </summary>
		/// <returns></returns>
		public static IDataReader RecalculateState(int EventId, DateTime DT)
		{
			return DbHelper2.RunSpDataReader("EventRecalculateState",
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId),
				DbHelper2.mp("@DateNow", SqlDbType.DateTime, DT)
				);
		}
		#endregion

		#region GetState(...)
		public static int GetState(int ObjectId)
		{
			return DBCommon.NullToInt32(
				DbHelper2.RunSpScalar("EventGetState"
				, DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId)
				));
		}
		#endregion
	}
}
