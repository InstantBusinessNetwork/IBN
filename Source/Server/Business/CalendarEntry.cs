using System;
using System.Collections;
using System.Data;
using System.Xml;

using Mediachase.Ibn;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Business.SpreadSheet;
using Mediachase.IBN.Database;
using Mediachase.MetaDataPlus;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// 
	/// </summary>
	public class CalendarEntry
	{
		#region CALENDAR_ENTRY_TYPE
		private static ObjectTypes CALENDAR_ENTRY_TYPE
		{
			get { return ObjectTypes.CalendarEntry; }
		}
		#endregion

		#region EventType
		public enum EventType
		{
			Event = 1,
			Meeting = 2,
			Appointment = 3
		};
		#endregion

		#region EventSecurity
		public class EventSecurity
		{
			public bool IsResource = false;
			public bool IsManager = false;

			public EventSecurity(int event_id, int user_id)
			{
				using (IDataReader reader = DBEvent.GetSecurityForUser(event_id, user_id))
				{
					if (reader.Read())
					{
						IsResource = ((int)reader["IsResource"] > 0) ? true : false;
						IsManager = ((int)reader["IsManager"] > 0) ? true : false;
					}
				}
			}
		}
		#endregion

		#region CanCreate
		public static bool CanCreate()
		{
			return true;
		}
		public static bool CanCreate(int project_id)
		{
			return true;
		}
		#endregion

		#region CanUpdate
		public static bool CanUpdate(int event_id)
		{
			bool retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)
				|| GetSecurity(event_id).IsManager;

			// Check by Project
			if (!retval)
			{
				int ProjectId = DBEvent.GetProject(event_id);
				if (ProjectId > 0)
					retval = Project.GetSecurity(ProjectId).IsManager;
			}

			// Check by Sharing
			if (!retval)
			{
				if (DBEvent.GetSharingLevel(Security.CurrentUser.UserID, event_id) > 0)
					retval = true;
			}

			return retval;
		}
		#endregion

		#region CanDelete
		public static bool CanDelete(int event_id)
		{
			return CanUpdate(event_id);
		}
		#endregion

		#region CanRead(int event_id)
		public static bool CanRead(int event_id)
		{
			bool retval = false;

			int projectId = -1;
			using (IDataReader reader = GetEvent(event_id, false))
			{
				if (reader.Read())
				{
					if (reader["ProjectId"] != DBNull.Value)
						projectId = (int)reader["ProjectId"];
				}
			}

			// Check by AlertService
			retval = Security.CurrentUser.IsAlertService;

			if (!retval /*&& projectId > 0 */)
			{
				retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)
					|| Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager);
			}

			// Check by Event
			if (!retval)
			{
				EventSecurity es = GetSecurity(event_id);
				retval = es.IsManager || es.IsResource;
			}

			// Check by Project
			if (!retval && projectId > 0)
			{
				Project.ProjectSecurity ps = Project.GetSecurity(projectId);
				retval = ps.IsManager || ps.IsTeamMember || ps.IsExecutiveManager || ps.IsSponsor || ps.IsStakeHolder;
			}

			// Check by Sharing
			if (!retval)
			{
				if (DBEvent.GetSharingLevel(Security.CurrentUser.UserID, event_id) >= 0)
					retval = true;
			}

			return retval;
		}
		#endregion

		#region CanRead(int event_id, int user_id)
		public static bool CanRead(int event_id, int user_id)
		{
			bool retval = false;

			int projectId = -1;
			using (IDataReader reader = GetEvent(event_id, false))
			{
				if (reader.Read())
				{
					if (reader["ProjectId"] != DBNull.Value)
						projectId = (int)reader["ProjectId"];
				}
			}

			if (!retval && projectId > 0)
			{
				ArrayList secure_groups = User.GetListSecureGroupAll(user_id);
				retval = secure_groups.Contains((int)InternalSecureGroups.PowerProjectManager)
					|| secure_groups.Contains((int)InternalSecureGroups.ExecutiveManager);
			}

			// Check by Event
			if (!retval)
			{
				EventSecurity es = GetSecurity(event_id, user_id);
				retval = es.IsManager || es.IsResource;
			}

			// Check by Project
			if (!retval && projectId > 0)
			{
				Project.ProjectSecurity ps = Project.GetSecurity(projectId, user_id);
				retval = ps.IsManager || ps.IsTeamMember || ps.IsExecutiveManager || ps.IsSponsor || ps.IsStakeHolder;
			}

			return retval;
		}
		#endregion

		#region CanViewFinances
		public static bool CanViewFinances(int event_id)
		{
			return (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)
				|| Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager)
				|| GetSecurity(event_id).IsManager)
				&& DBEvent.GetProject(event_id) > 0;
		}
		#endregion

		#region GetSecurity
		public static EventSecurity GetSecurity(int event_id)
		{
			return GetSecurity(event_id, Security.CurrentUser.UserID);
		}

		public static EventSecurity GetSecurity(int event_id, int user_id)
		{
			return new EventSecurity(event_id, user_id);
		}
		#endregion

		#region CreateUseUniversalTime
		public static int CreateUseUniversalTime(
			string title,
			string description,
			string location,
			int project_id,
			int manager_id,
			int priority,
			int type_id,
			DateTime start_date,
			DateTime finish_date,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			ArrayList resourceList,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (!CanCreate(project_id))
				throw new AccessDeniedException();

			if (start_date > finish_date)
				throw new Exception("wrong dates");

			int task_time = 0;
			if (project_id > 0)
			{
				int CalendarId = DBProject.GetCalendar(project_id);
				task_time = DBCalendar.GetDurationByFinishDate(CalendarId, start_date, finish_date);
			}
			else
			{
				TimeSpan ts = (finish_date).Subtract(start_date);
				task_time = (int)ts.TotalMinutes;
			}

			DateTime creation_date = DateTime.UtcNow;

			object oProjectId = null;
			if (project_id > 0)
				oProjectId = project_id;

			int UserId = Security.CurrentUser.UserID;
			int EventId = -1;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				EventId = DBEvent.Create(oProjectId, UserId,
					manager_id, title, description, location, creation_date,
					start_date, finish_date, priority, type_id, task_time,
					contactUid == PrimaryKeyId.Empty ? null : (object)contactUid,
					orgUid == PrimaryKeyId.Empty ? null : (object)orgUid);

				// Categories
				foreach (int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)CALENDAR_ENTRY_TYPE, EventId, CategoryId);
				}

				// O.R: Scheduling
				Schedule.UpdateDateTypeValue(DateTypes.CalendarEntry_StartDate, EventId, start_date);
				Schedule.UpdateDateTypeValue(DateTypes.CalendarEntry_FinishDate, EventId, finish_date);

				SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Created, EventId);
				if (project_id > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_CalendarEntryList_CalendarEntryAdded, project_id, EventId);

				if (FileName != null)
				{
					string ContainerName = "FileLibrary";
					string ContainerKey = UserRoleHelper.CreateEventContainerKey(EventId);

					BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
					FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
					fs.SaveFile(fs.Root.Id, FileName, _inputStream);

					//Asset.Create(EventId,ObjectTypes.CalendarEntry, FileName,"", FileName, _inputStream, false);			
				}

				// OZ: User Role Addon
				UserRoleHelper.AddEventCreatorRole(EventId, UserId);
				UserRoleHelper.AddEventManagerRole(EventId, manager_id);
				if (project_id > 0)
				{
					ForeignContainerKey.Add(UserRoleHelper.CreateEventContainerKey(EventId), UserRoleHelper.CreateProjectContainerKey(project_id));
				}
				//

				// Oleg Zhuk Addon: Add Resource
				if (resourceList != null)
				{
					foreach (int PrincipalId in resourceList)
					{
						int RealPrincipalId = PrincipalId;

						bool bMustBeConfirmed = false;

						if (RealPrincipalId < 0)
						{
							RealPrincipalId *= -1;
							bMustBeConfirmed = true;
						}

						DBEvent.AddResource(EventId, RealPrincipalId, bMustBeConfirmed);

						if (bMustBeConfirmed)
							SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_ResourceList_RequestAdded, EventId, RealPrincipalId);
						else
							SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_ResourceList_AssignmentAdded, EventId, RealPrincipalId);

						UserRoleHelper.AddEventResourceRole(EventId, manager_id);

						if (User.IsExternal(RealPrincipalId))
							DBCommon.AddGate((int)CALENDAR_ENTRY_TYPE, EventId, RealPrincipalId);
					}
				}

				// O.R: Recalculating project TaskTime
				if (project_id > 0 && task_time > 0)
					TimeTracking.RecalculateProjectTaskTime(project_id);

				RecalculateState(EventId);

				tran.Commit();
			}

			return EventId;
		}
		#endregion

		#region CreateWithResource
		public static int Create(
			string title,
			string description,
			string location,
			int project_id,
			int manager_id,
			int priority,
			int type_id,
			DateTime start_date,
			DateTime finish_date,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			ArrayList resourceList,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (!CanCreate(project_id))
				throw new AccessDeniedException();

			if (start_date > finish_date)
				throw new Exception("wrong dates");

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime creation_date = DateTime.UtcNow;
			start_date = DBCommon.GetUTCDate(TimeZoneId, start_date);
			finish_date = DBCommon.GetUTCDate(TimeZoneId, finish_date);

			int task_time = 0;
			if (project_id > 0)
			{
				int CalendarId = DBProject.GetCalendar(project_id);
				task_time = DBCalendar.GetDurationByFinishDate(CalendarId, start_date, finish_date);
			}
			else
			{
				TimeSpan ts = (finish_date).Subtract(start_date);
				task_time = (int)ts.TotalMinutes;
			}

			object oProjectId = null;
			if (project_id > 0)
				oProjectId = project_id;

			int UserId = Security.CurrentUser.UserID;
			int EventId = -1;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				EventId = DBEvent.Create(oProjectId, UserId,
					manager_id, title, description, location, creation_date,
					start_date, finish_date, priority, type_id, task_time,
					contactUid == PrimaryKeyId.Empty ? null : (object)contactUid,
					orgUid == PrimaryKeyId.Empty ? null : (object)orgUid);

				// Categories
				foreach (int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)CALENDAR_ENTRY_TYPE, EventId, CategoryId);
				}

				// O.R: Scheduling
				Schedule.UpdateDateTypeValue(DateTypes.CalendarEntry_StartDate, EventId, start_date);
				Schedule.UpdateDateTypeValue(DateTypes.CalendarEntry_FinishDate, EventId, finish_date);

				SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Created, EventId);
				if (project_id > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_CalendarEntryList_CalendarEntryAdded, project_id, EventId);

				// OZ: User Role Addon
				UserRoleHelper.AddEventCreatorRole(EventId, UserId);
				UserRoleHelper.AddEventManagerRole(EventId, manager_id);
				if (project_id > 0)
				{
					ForeignContainerKey.Add(UserRoleHelper.CreateEventContainerKey(EventId), UserRoleHelper.CreateProjectContainerKey(project_id));
				}

				// Al: FileStorage
				if (FileName != null)
				{
					string ContainerName = "FileLibrary";
					string ContainerKey = UserRoleHelper.CreateEventContainerKey(EventId);

					BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
					FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
					fs.SaveFile(fs.Root.Id, FileName, _inputStream);

					//Asset.Create(EventId,ObjectTypes.CalendarEntry, FileName,"", FileName, _inputStream, false);			
				}

				// Oleg Zhuk Addon: Add Resource
				if (resourceList != null)
				{
					foreach (int PrincipalId in resourceList)
					{
						DBEvent.AddResource(EventId, PrincipalId, false);

						// OZ: User Role Addon
						UserRoleHelper.AddEventResourceRole(EventId, PrincipalId);
						//

						if (User.IsExternal(PrincipalId))
							DBCommon.AddGate((int)CALENDAR_ENTRY_TYPE, EventId, PrincipalId);

						SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_ResourceList_AssignmentAdded, EventId, PrincipalId);
					}
				}

				// O.R: Recalculating project TaskTime
				if (project_id > 0 && task_time > 0)
					TimeTracking.RecalculateProjectTaskTime(project_id);

				RecalculateState(EventId);

				tran.Commit();
			}

			return EventId;
		}
		#endregion

		#region Create
		public static int Create(
			string title,
			string description,
			string location,
			int project_id,
			int manager_id,
			int priority,
			int type_id,
			DateTime start_date,
			DateTime finish_date,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			bool use_scheduled_users,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			ArrayList ResourceList = new ArrayList();
			if (use_scheduled_users)
			{
				using (IDataReader reader = CalendarView.GetListScheduledUsers())
				{
					while (reader.Read())
					{
						if ((byte)reader["Activity"] != (byte)User.UserActivity.Inactive)
							ResourceList.Add((int)reader["UserId"]);
					}
				}
			}

			if (!ResourceList.Contains(manager_id))
				ResourceList.Add(manager_id);

			return CalendarEntry.Create(title, description, location, project_id,
				manager_id, priority, type_id, start_date, finish_date, categories, FileName, _inputStream, ResourceList, contactUid, orgUid);
		}
		#endregion

		#region CreateFromWizard
		public static int CreateFromWizard(
			string title,
			string description,
			string location,
			int project_id,
			int priority,
			int type_id,
			DateTime start_date,
			DateTime finish_date,
			DataTable resourceList,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (!CanCreate(project_id))
				throw new AccessDeniedException();

			if (start_date > finish_date)
				throw new Exception("wrong dates");

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime creation_date = DateTime.UtcNow;
			start_date = DBCommon.GetUTCDate(TimeZoneId, start_date);
			finish_date = DBCommon.GetUTCDate(TimeZoneId, finish_date);

			int task_time = 0;
			if (project_id > 0)
			{
				int CalendarId = DBProject.GetCalendar(project_id);
				task_time = DBCalendar.GetDurationByFinishDate(CalendarId, start_date, finish_date);
			}
			else
			{
				TimeSpan ts = (finish_date).Subtract(start_date);
				task_time = (int)ts.TotalMinutes;
			}

			object oProjectId = null;
			if (project_id > 0)
				oProjectId = project_id;

			int UserId = Security.CurrentUser.UserID;
			int EventId = -1;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				EventId = DBEvent.Create(oProjectId, UserId,
					UserId, title, description, location, creation_date,
					start_date, finish_date, priority, type_id, task_time,
					contactUid == PrimaryKeyId.Empty ? null : (object)contactUid,
					orgUid == PrimaryKeyId.Empty ? null : (object)orgUid);

				// O.R: Scheduling
				Schedule.UpdateDateTypeValue(DateTypes.CalendarEntry_StartDate, EventId, start_date);
				Schedule.UpdateDateTypeValue(DateTypes.CalendarEntry_FinishDate, EventId, finish_date);

				SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Created, EventId);
				if (project_id > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_CalendarEntryList_CalendarEntryAdded, project_id, EventId);

				//AK: Categories
				// Categories
				ArrayList categories = Common.StringToArrayList(PortalConfig.CEntryDefaultValueGeneralCategoriesField);
				foreach (int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)CALENDAR_ENTRY_TYPE, EventId, CategoryId);
				}

				// OZ: User Role Addon
				UserRoleHelper.AddEventCreatorRole(EventId, UserId);
				UserRoleHelper.AddEventManagerRole(EventId, UserId);
				if (project_id > 0)
				{
					ForeignContainerKey.Add(UserRoleHelper.CreateEventContainerKey(EventId), UserRoleHelper.CreateProjectContainerKey(project_id));
				}
				//

				///
				DBEvent.AddResource(EventId, UserId, false);
				UserRoleHelper.AddEventResourceRole(EventId, UserId);
				///

				// Add new resources
				ArrayList resAdd = new ArrayList();
				ArrayList resReq = new ArrayList();
				foreach (DataRow dr in resourceList.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						int uId = (int)dr["UserId"];
						bool MustBeConfirmed = (bool)dr["MustBeConfirmed"];
						DBEvent.AddResource(EventId, uId, MustBeConfirmed);

						if (MustBeConfirmed)
							SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_ResourceList_RequestAdded, EventId, uId);
						else
							SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_ResourceList_AssignmentAdded, EventId, uId);

						// OZ: User Role Addon
						UserRoleHelper.AddEventResourceRole(EventId, uId);
						//

						if (User.IsExternal(uId))
							DBCommon.AddGate((int)CALENDAR_ENTRY_TYPE, EventId, uId);
					}
				}

				// O.R: Recalculating project TaskTime
				if (project_id > 0 && task_time > 0)
					TimeTracking.RecalculateProjectTaskTime(project_id);

				RecalculateState(EventId);

				tran.Commit();
			}

			return EventId;
		}
		#endregion

		#region Delete
		public static void Delete(int event_id)
		{
			if (!CanDelete(event_id))
				throw new AccessDeniedException();

			int projectId = DBEvent.GetProject(event_id);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				// OZ: File Storage
				BaseIbnContainer container = BaseIbnContainer.Create("FileLibrary", UserRoleHelper.CreateEventContainerKey(event_id));
				FileStorage fs = (FileStorage)container.LoadControl("FileStorage");
				fs.DeleteAll();
				//

				// OZ: User Roles
				UserRoleHelper.DeleteEventRoles(event_id);

				// Step 2. Transform Timesheet [2004-02-04]
				TimeTracking.ResetObjectId((int)CALENDAR_ENTRY_TYPE, event_id);

				MetaObject.Delete(event_id, "EventsEx");

				SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Deleted, event_id);
				if (projectId > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_CalendarEntryList_CalendarEntryDeleted, projectId, event_id);

				// Step 3. Delete 
				DBEvent.Delete(event_id);

				// O.R: Recalculating project TaskTime
				if (projectId > 0)
					TimeTracking.RecalculateProjectTaskTime(projectId);

				tran.Commit();
			}
		}
		#endregion

		#region GetEvent
		/// <summary>
		/// Reader returns fields:
		///  EventId, ProjectId, ProjectTitle, CreatorId, ManagerId, 
		///  Title, Description, Location, CreationDate, StartDate, 
		///  FinishDate, PriorityId, PriorityName, TypeId, TypeName, ReminderInterval, 
		///  StateId, HasRecurrence
		/// </summary>
		/// <param name="event_id"></param>
		/// <returns></returns>
		public static IDataReader GetEvent(int event_id)
		{
			return GetEvent(event_id, true);
		}

		public static IDataReader GetEvent(int event_id, bool check_access)
		{
			return GetEvent(event_id, check_access, false);
		}

		private static IDataReader GetEvent(int event_id, bool check_access, bool utc_dates)
		{
			if (check_access && !CanRead(event_id))
				throw new AccessDeniedException();

			if (utc_dates)
				return DBEvent.GetEventInUTC(event_id, Security.CurrentUser.LanguageId);
			else
				return DBEvent.GetEvent(event_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetEventDates
		/// <summary>
		/// Reader returns fields:
		///  EventId, StartDate, FinishDate
		/// </summary>
		/// <param name="event_id"></param>
		/// <returns></returns>
		public static IDataReader GetEventDates(int event_id)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;

			using (IDataReader reader = DBCommon.GetRecurrence((int)CALENDAR_ENTRY_TYPE, event_id))
			{
				if (reader.Read())	// recurrence exists
					TimeZoneId = (int)reader["TimeZoneId"];
			}

			return DBEvent.GetEventDates(event_id, TimeZoneId);
		}
		#endregion

		#region GetListProjects
		/// <summary>
		/// Reader returns fields:
		///		ProjectId, Title
		/// </summary>
		public static IDataReader GetListProjects()
		{
			return DBProject.GetListProjectsByManager(Security.CurrentUser.UserID);
		}

		public static IDataReader GetListProjects(int UserId)
		{
			return DBProject.GetListActiveProjectsByUser(UserId, Security.CurrentUser.TimeZoneId);
		}

		public static DataTable GetListProjectsDataTable(int UserId)
		{
			return DBProject.GetListActiveProjectsByUserDataTable(UserId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetProjectManager
		public static int GetProjectManager(int project_id)
		{
			return DBProject.GetProjectManager(project_id);
		}
		#endregion

		#region GetListPriorities
		/// <summary>
		/// Reader returns fields:
		///		PriorityId, PriorityName 
		/// </summary>
		public static IDataReader GetListPriorities()
		{
			return DBCommon.GetListPriorities(Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListEventTypes
		/// <summary>
		/// Reader returns fields:
		///  TypeId, TypeName 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventTypes()
		{
			return DBEvent.GetListEventTypes(Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListCategoriesAll
		/// <summary>
		/// Reader returns fields:
		///		CategoryId, CategoryName 
		/// </summary>
		public static IDataReader GetListCategoriesAll()
		{
			return DBCommon.GetListCategories();
		}
		#endregion

		#region GetListCategories
		/// <summary>
		/// Reader returns fields:
		///		CategoryId, CategoryName 
		/// </summary>
		public static IDataReader GetListCategories(int event_id)
		{
			return DBCommon.GetListCategoriesByObject((int)CALENDAR_ENTRY_TYPE, event_id);
		}
		#endregion

		#region GetListDiscussions
		/// <summary>
		/// Reader returns fields:
		///		DiscussionId, ObjectTypeId, ObjectId, CreatorId, CreationDate, Text, CreatorName
		/// </summary>
		public static IDataReader GetListDiscussions(int event_id)
		{
			return Common.GetListDiscussions(CALENDAR_ENTRY_TYPE, event_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListDiscussionsDataTable
		/// <summary>
		/// Reader returns fields:
		///		DiscussionId, ObjectTypeId, ObjectId, CreatorId, CreationDate, Text, CreatorName
		/// </summary>
		public static DataTable GetListDiscussionsDataTable(int event_id)
		{
			return Common.GetListDiscussionsDataTable(CALENDAR_ENTRY_TYPE, event_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region AddDiscussion
		public static void AddDiscussion(int event_id, string text)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			DateTime creation_date = DateTime.UtcNow;
			int UserId = Security.CurrentUser.UserID;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				int CommentId = DBCommon.AddDiscussion((int)CALENDAR_ENTRY_TYPE, event_id, UserId, creation_date, text);

				SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_CommentList_CommentAdded, event_id, CommentId);

				tran.Commit();
			}
		}
		#endregion

		#region GetListResources
		/// <summary>
		/// Reader returns fields:
		///  ResourceId, EventId, PrincipalId, IsGroup, MustBeConfirmed, ResponsePending, 
		///  IsConfirmed, IsExternal
		/// </summary>
		/// <param name="event_id"></param>
		/// <returns></returns>
		public static IDataReader GetListResources(int event_id)
		{
			return DBEvent.GetListResources(event_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListEventResourcesDataTable
		/// <summary>
		/// ResourceId, EventId, PrincipalId, IsGroup, MustBeConfirmed, ResponsePending, 
		///  IsConfirmed, IsExternal
		/// </summary>
		/// <param name="event_id"></param>
		/// <returns></returns>
		public static DataTable GetListEventResourcesDataTable(int event_id)
		{
			return DBEvent.GetListEventResourcesDataTable(event_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListEventsByProject
		/// <summary>
		/// Reader returns fields:
		///  EventId, ProjectId, CreatorId, ManagerId, LastEditorId, 
		///  Title, Description, Location, CreationDate, LastSavedDate, StartDate, 
		///  FinishDate, PriorityId, PriorityName, TypeId, TypeName, StateId
		/// </summary>
		/// <param name="project_id"></param>
		/// <returns></returns>
		public static IDataReader GetListEventsByProject(int project_id)
		{
			return DBEvent.GetListEventsByProject(project_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListEventsByProjectDataTable
		/// <summary>
		/// Reader returns fields:
		///  EventId, ProjectId, CreatorId, ManagerId, LastEditorId, 
		///  Title, Description, Location, CreationDate, LastSavedDate, StartDate, 
		///  FinishDate, PriorityId, PriorityName, TypeId, TypeName, StateId
		/// </summary>
		/// <param name="project_id"></param>
		/// <returns></returns>
		public static DataTable GetListEventsByProjectDataTable(int project_id)
		{
			return DBEvent.GetListEventsByProjectDataTable(project_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region AddRecurrence
		public static void AddRecurrence(
			int EventId,
			int StartTime, int EndTime,
			byte Pattern, byte SubPattern,
			byte Frequency, byte Weekdays,
			byte DayOfMonth, byte weekNumber,
			byte MonthNumber, int EndAfter,
			DateTime StartDate, DateTime FinishDate,
			int TimeZoneId)
		{
			if (!CanUpdate(EventId))
				throw new AccessDeniedException();

			if (Pattern < 4 && Frequency <= 0)
				throw new WrongDataException();

			DateTime eventStartDate = DateTime.UtcNow;
			using (IDataReader reader = GetEvent(EventId, false, true))
			{
				if (reader.Read())
					eventStartDate = (DateTime)reader["StartDate"];
			}

			DateTime RecStartDate = StartDate;
			DateTime RecFinishDate = FinishDate;
			StartDate = DBCommon.GetUTCDate(TimeZoneId, StartDate);		// to UTC
			FinishDate = DBCommon.GetUTCDate(TimeZoneId, FinishDate);	// to UTC

			if (EndAfter > 0)		// end after N occurrences
			{
				// Recalculating FinishDate
				Recurrence recurrence = new Recurrence(Pattern, SubPattern, Frequency, Weekdays, DayOfMonth, weekNumber, MonthNumber, EndAfter, RecStartDate, RecFinishDate, TimeZoneId);
				ArrayList dates = GetRecurDates(StartDate, StartDate.AddYears(100), StartTime, eventStartDate, recurrence, 1, Disposition.Last);
				if (dates.Count > 0)
					FinishDate = (DateTime)dates[0];		// Finish is in UTC format
			}

			StartDate = StartDate.AddMinutes(StartTime);
			FinishDate = FinishDate.AddMinutes(EndTime);

			int UserId = Security.CurrentUser.UserID;

			int ReminderInterval;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBCommon.AddRecurrence((int)CALENDAR_ENTRY_TYPE, EventId, StartTime, EndTime, Pattern, SubPattern, Frequency, Weekdays, DayOfMonth, weekNumber, MonthNumber, EndAfter, TimeZoneId);
				DbCalendarEntry2.UpdateTimeline(EventId, StartDate, FinishDate, EndTime - StartTime);

				Schedule.DeleteDateTypeValue(DateTypes.CalendarEntry_StartDate, EventId);
				Schedule.DeleteDateTypeValue(DateTypes.CalendarEntry_FinishDate, EventId);

				Hashtable ret = GetEventInstances(EventId, out ReminderInterval);
				foreach (DateTime dtStart in ret.Keys)
				{
					DateTime dtEnd = (DateTime)ret[dtStart];
					if (dtStart > DateTime.UtcNow && dtEnd > DateTime.UtcNow)
					{
						DbSchedule2.AddNewDateTypeValue((int)DateTypes.CalendarEntry_StartDate, EventId, dtStart);
						DbSchedule2.AddNewDateTypeValue((int)DateTypes.CalendarEntry_FinishDate, EventId, dtEnd);
					}
				}

				// O.R. [2009-12-18] Timeline = duration * count
				DbCalendarEntry2.UpdateTimeline(EventId, StartDate, FinishDate, (EndTime - StartTime) * ret.Count);

				RecalculateState(EventId);

				tran.Commit();
			}
		}
		#endregion

		#region DeleteRecurrence
		public static void DeleteRecurrence(int eventId)
		{
			if (!CanUpdate(eventId))
				throw new AccessDeniedException();

			int projectId = GetProject(eventId);

			using (DbTransaction tran = DbTransaction.Begin())
			{

				DBCommon.DeleteRecurrence((int)CALENDAR_ENTRY_TYPE, eventId);
				DateTime dtStartDate;
				DateTime dtFinishDate;
				using (IDataReader reader = DBEvent.GetEventInUTC(eventId, Security.CurrentUser.LanguageId))
				{
					reader.Read();
					dtStartDate = (DateTime)reader["StartDate"];
					dtFinishDate = (DateTime)reader["FinishDate"];
				}

				dtFinishDate = (dtStartDate.Date).AddMinutes(dtFinishDate.TimeOfDay.TotalMinutes);

				if (dtFinishDate <= dtStartDate)
					dtFinishDate = dtStartDate.AddHours(1);

				TimeSpan ts = dtFinishDate.Subtract(dtStartDate);
				int taskTime = (int)ts.TotalMinutes;

				DbCalendarEntry2.UpdateTimeline(eventId, dtStartDate, dtFinishDate, taskTime);

				Schedule.DeleteDateTypeValue(DateTypes.CalendarEntry_StartDate, eventId);
				Schedule.DeleteDateTypeValue(DateTypes.CalendarEntry_FinishDate, eventId);
				Schedule.UpdateDateTypeValue(DateTypes.CalendarEntry_StartDate, eventId, dtStartDate);
				Schedule.UpdateDateTypeValue(DateTypes.CalendarEntry_FinishDate, eventId, dtFinishDate);

				// O.R: Recalculating project TaskTime
				if (projectId > 0)
					TimeTracking.RecalculateProjectTaskTime(projectId);

				RecalculateState(eventId);

				tran.Commit();
			}
		}
		#endregion

		#region GetRecurrence
		/// <summary>
		/// Reader returns fields:
		///		RecurrenceId, ObjectTypeId, ObjectId, StartTime, EndTime, Pattern, SubPattern, 
		///		Frequency, Weekdays, DayOfMonth, WeekNumber, MonthNumber, EndAfter, TimeZoneId
		/// </summary>
		public static IDataReader GetRecurrence(int EventId)
		{
			return DBCommon.GetRecurrence((int)CALENDAR_ENTRY_TYPE, EventId);
		}
		#endregion

		#region ==== Recurrence ======
		#region Recurrence
		public struct Recurrence
		{
			public RecurPattern Pattern;
			public RecurSubPattern SubPattern;
			public byte Frequency;
			public byte WeekDays;
			public byte DayOfMonth;
			public WeekNumber RecurWeekNumber;
			public byte MonthNumber;
			public int EndAfter;
			public DateTime StartDate;
			public DateTime FinishDate;
			public int TimeZoneId;

			public Recurrence(byte pattern, byte subPattern,
				byte frequency, byte weekDays, byte dayOfMonth,
				byte weekNumber, byte monthNumber, int endAfter,
				DateTime startDate, DateTime finishDate, int timeZoneId)
			{
				Pattern = (RecurPattern)pattern;
				SubPattern = (RecurSubPattern)subPattern;
				Frequency = frequency;
				WeekDays = weekDays;
				DayOfMonth = dayOfMonth;
				RecurWeekNumber = (WeekNumber)weekNumber;
				MonthNumber = monthNumber;
				EndAfter = endAfter;
				StartDate = startDate.Date;
				FinishDate = finishDate.Date;
				TimeZoneId = timeZoneId;
			}

			public override string ToString()
			{
				string RetVal = "";

				string sFinishDate = FinishDate.ToString("yyyyMMdd");
				if (EndAfter <= 0)
				{
					int TimeZoneBias = -DBCommon.GetTimeZoneBias(TimeZoneId);
					int HH = TimeZoneBias / 60;
					string sHH = HH.ToString();
					if (HH < 10 && HH > -10)
						sHH = String.Format("0{0}", sHH);
					int MM = TimeZoneBias - HH * 60;
					string sMM = MM.ToString();
					if (MM < 10 && MM > -10)
						sMM = String.Format("0{0}", sMM);
					sFinishDate = String.Format("{0}T{1}{2}00Z", sFinishDate, sHH, sMM);
				}

				switch (Pattern)
				{
					case RecurPattern.Daily:
						RetVal = String.Format("D{0}", Frequency);
						break;
					case RecurPattern.Weekly:
						RetVal = String.Format("W{0}", Frequency);
						if ((WeekDays & (byte)BitDayOfWeek.Sunday) == (byte)BitDayOfWeek.Sunday)
							RetVal = String.Format("{0} SU", RetVal);
						if ((WeekDays & (byte)BitDayOfWeek.Monday) == (byte)BitDayOfWeek.Monday)
							RetVal = String.Format("{0} MO", RetVal);
						if ((WeekDays & (byte)BitDayOfWeek.Tuesday) == (byte)BitDayOfWeek.Tuesday)
							RetVal = String.Format("{0} TU", RetVal);
						if ((WeekDays & (byte)BitDayOfWeek.Wednesday) == (byte)BitDayOfWeek.Wednesday)
							RetVal = String.Format("{0} WE", RetVal);
						if ((WeekDays & (byte)BitDayOfWeek.Thursday) == (byte)BitDayOfWeek.Thursday)
							RetVal = String.Format("{0} TH", RetVal);
						if ((WeekDays & (byte)BitDayOfWeek.Friday) == (byte)BitDayOfWeek.Friday)
							RetVal = String.Format("{0} FR", RetVal);
						if ((WeekDays & (byte)BitDayOfWeek.Saturday) == (byte)BitDayOfWeek.Saturday)
							RetVal = String.Format("{0} SA", RetVal);
						break;
					case RecurPattern.Monthly:
						if (SubPattern == RecurSubPattern.SubPattern1)
							RetVal = String.Format("MD{0} {1}", Frequency, DayOfMonth);
						else
						{
							RetVal = String.Format("MP{0}", Frequency);
							if (RecurWeekNumber == WeekNumber.Last)
								RetVal = String.Format("{0} 1-", RetVal);
							else
								RetVal = String.Format("{0} {1}+", RetVal, (int)RecurWeekNumber);

							if ((WeekDays & (byte)BitDayOfWeek.Sunday) == (byte)BitDayOfWeek.Sunday)
								RetVal = String.Format("{0} SU", RetVal);
							else if ((WeekDays & (byte)BitDayOfWeek.Monday) == (byte)BitDayOfWeek.Monday)
								RetVal = String.Format("{0} MO", RetVal);
							else if ((WeekDays & (byte)BitDayOfWeek.Tuesday) == (byte)BitDayOfWeek.Tuesday)
								RetVal = String.Format("{0} TU", RetVal);
							else if ((WeekDays & (byte)BitDayOfWeek.Wednesday) == (byte)BitDayOfWeek.Wednesday)
								RetVal = String.Format("{0} WE", RetVal);
							else if ((WeekDays & (byte)BitDayOfWeek.Thursday) == (byte)BitDayOfWeek.Thursday)
								RetVal = String.Format("{0} TH", RetVal);
							else if ((WeekDays & (byte)BitDayOfWeek.Friday) == (byte)BitDayOfWeek.Friday)
								RetVal = String.Format("{0} FR", RetVal);
							else
								RetVal = String.Format("{0} SA", RetVal);
						}
						break;
					default:
						RetVal = String.Format("YM{0} {1}", Frequency, MonthNumber);
						break;
				}
				if (EndAfter > 0)
					RetVal = String.Format("{0} #{1}", RetVal, EndAfter);
				else
					RetVal = String.Format("{0} {1}", RetVal, sFinishDate);

				if (Pattern == RecurPattern.Yearly)
				{
					if (SubPattern == RecurSubPattern.SubPattern1)
						RetVal = String.Format("{0} MD1 {1}", RetVal, DayOfMonth);
					else
					{
						RetVal = String.Format("{0} MP1", RetVal);
						if (RecurWeekNumber == WeekNumber.Last)
							RetVal = String.Format("{0} 1-", RetVal);
						else
							RetVal = String.Format("{0} {1}+", RetVal, (int)RecurWeekNumber);

						if ((WeekDays & (byte)BitDayOfWeek.Sunday) == (byte)BitDayOfWeek.Sunday)
							RetVal = String.Format("{0} SU", RetVal);
						else if ((WeekDays & (byte)BitDayOfWeek.Monday) == (byte)BitDayOfWeek.Monday)
							RetVal = String.Format("{0} MO", RetVal);
						else if ((WeekDays & (byte)BitDayOfWeek.Tuesday) == (byte)BitDayOfWeek.Tuesday)
							RetVal = String.Format("{0} TU", RetVal);
						else if ((WeekDays & (byte)BitDayOfWeek.Wednesday) == (byte)BitDayOfWeek.Wednesday)
							RetVal = String.Format("{0} WE", RetVal);
						else if ((WeekDays & (byte)BitDayOfWeek.Thursday) == (byte)BitDayOfWeek.Thursday)
							RetVal = String.Format("{0} TH", RetVal);
						else if ((WeekDays & (byte)BitDayOfWeek.Friday) == (byte)BitDayOfWeek.Friday)
							RetVal = String.Format("{0} FR", RetVal);
						else
							RetVal = String.Format("{0} SA", RetVal);
					}
				}

				return RetVal;
			}
		}
		#endregion

		#region enum RecurPattern
		public enum RecurPattern
		{
			Daily = 1,
			Weekly = 2,
			Monthly = 3,
			Yearly = 4
		};
		#endregion

		#region enum RecurSubPattern
		public enum RecurSubPattern
		{
			SubPattern1 = 1,
			SubPattern2 = 2
		};
		#endregion

		#region enum BitDayOfWeek
		[Flags]
		public enum BitDayOfWeek
		{
			Unknown = 0x00,
			Monday = 0x01,
			Tuesday = 0x02,
			Wednesday = 0x04,
			Thursday = 0x08,
			Friday = 0x10,
			Saturday = 0x20,
			Sunday = 0x40,
			Alldays = Monday | Tuesday | Wednesday | Thursday | Friday | Saturday | Sunday,
			Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday
		}
		#endregion

		#region enum WeekNumber
		public enum WeekNumber
		{
			Unknown = 0,
			First = 1,
			Second = 2,
			Third = 3,
			Fourth = 4,
			Fifth = 5,
			Last = 8
		};
		#endregion

		#region enum Disposition
		public enum Disposition
		{
			First,
			Last
		}
		#endregion

		#region GetRecurDates
		/// <summary>
		/// Returns ArrayList of DateTime objects 
		/// (fromDate, toDate, eventStartDate - in UTC format)
		/// (return dates in UTC too, but without time part. 
		///		To get the real Start DateTime you need to add recurrence StartTime to the returned dates)
		/// </summary>
		public static ArrayList GetRecurDates(DateTime fromDate, DateTime toDate, int startTime, DateTime eventStartDate, Recurrence recurrence, int maxCount, Disposition disp)
		{
			ArrayList list = new ArrayList();
			DateTime addDate;
			bool add;
			bool WeekDaysCalculated = false;
			int count = 0;
			// Переводим fromDate и toDate из UTC во временную зону рекурсии
			fromDate = DBCommon.GetLocalDate(recurrence.TimeZoneId, fromDate);
			toDate = DBCommon.GetLocalDate(recurrence.TimeZoneId, toDate);
			eventStartDate = DBCommon.GetLocalDate(recurrence.TimeZoneId, eventStartDate);

			DateTime startDate = recurrence.StartDate.Date;
			DateTime finishDate = recurrence.FinishDate.Date;
			DateTime curDate = startDate;

			if (recurrence.EndAfter == 0 && finishDate < toDate)
				toDate = finishDate;

			while (curDate <= toDate.AddDays(1))	// O.R.: добавляем один день, чтобы не потерять данные за счёт разницы временных зон рекурсии и пользователя
			{
				// O.R.: if curDate is not appropriate - skip
				if (curDate.AddMinutes(startTime) < eventStartDate)
				{
					curDate = curDate.AddDays(1);
					continue;
				}

				add = false;
				addDate = curDate;

				if (recurrence.Pattern == RecurPattern.Daily && recurrence.SubPattern == RecurSubPattern.SubPattern1)
				{
					// Repeat on every Nth day.
					TimeSpan span = curDate - startDate;
					if (span.Days % recurrence.Frequency == 0)
					{
						add = true;
						curDate = curDate.AddDays(recurrence.Frequency);
					}
				}
				else if (recurrence.Pattern == RecurPattern.Daily && recurrence.SubPattern == RecurSubPattern.SubPattern2)
				{
					// Repeat on every Nth weekday.
					if (WeekDaysCalculated)
					{
						add = true;
						curDate = AddWeekDays(curDate, recurrence.Frequency);
					}
					else
					{
						int weekDaysElapsed = GetElapsedWeekDays(startDate, curDate);
						if (weekDaysElapsed % recurrence.Frequency == 0 && GetFirstWeekDay(startDate) <= curDate)
						{
							WeekDaysCalculated = true;
							add = true;
							curDate = AddWeekDays(curDate, recurrence.Frequency);
						}
					}
				}
				else if (recurrence.Pattern == RecurPattern.Weekly)
				{
					// Repeat on every selected day of Nth week.
					BitDayOfWeek bdow = GetBitDayOfWeek(curDate.DayOfWeek);
					if ((byte)BitDayOfWeek.Unknown != ((byte)bdow & recurrence.WeekDays))
					{
						int week = GetRelativeWeekNumber(startDate, curDate);
						if ((double)week % recurrence.Frequency == 0)
						{
							add = true;
							curDate = curDate.AddDays(1);
						}
					}
				}
				else if (recurrence.Pattern == RecurPattern.Monthly && recurrence.SubPattern == RecurSubPattern.SubPattern1)
				{
					// Repeat on selected day of every Nth month.
					if (curDate.Day == recurrence.DayOfMonth)
					{
						int month = GetRelativeMonthNumber(startDate, curDate);
						if ((double)month % recurrence.Frequency == 0)
						{
							add = true;
							curDate = curDate.AddMonths(recurrence.Frequency);
						}
					}
				}
				else if (recurrence.Pattern == RecurPattern.Monthly && recurrence.SubPattern == RecurSubPattern.SubPattern2)
				{
					// Repeat on selected week and day of week of every Nth month.
					if (curDate.DayOfWeek == GetDayOfWeekByBit((BitDayOfWeek)recurrence.WeekDays))
					{
						WeekNumber week = GetWeekNumber(curDate);
						if ((week & ~WeekNumber.Last) == recurrence.RecurWeekNumber || (week & WeekNumber.Last) == recurrence.RecurWeekNumber)
						{
							int month = GetRelativeMonthNumber(startDate, curDate);
							if ((double)month % recurrence.Frequency == 0)
							{
								add = true;
								curDate = curDate.AddMonths(recurrence.Frequency).Subtract(new TimeSpan(7, 0, 0, 0));
							}
						}
					}
				}
				else if (recurrence.Pattern == RecurPattern.Yearly && recurrence.SubPattern == RecurSubPattern.SubPattern1)
				{
					// Repeat on selected day and month of every year.
					if (curDate.Month == recurrence.MonthNumber && curDate.Day == recurrence.DayOfMonth)
					{
						add = true;
						curDate = curDate.AddYears(1);
					}
				}
				else if (recurrence.Pattern == RecurPattern.Yearly && recurrence.SubPattern == RecurSubPattern.SubPattern2)
				{
					// Repeat on selected month, week and day of week of every year.
					if (curDate.Month == recurrence.MonthNumber && curDate.DayOfWeek == GetDayOfWeekByBit((BitDayOfWeek)recurrence.WeekDays))
					{
						WeekNumber week = GetWeekNumber(curDate);
						if ((week & ~WeekNumber.Last) == recurrence.RecurWeekNumber || (week & WeekNumber.Last) == recurrence.RecurWeekNumber)
						{
							add = true;
							curDate = curDate.AddYears(1).Subtract(new TimeSpan(7, 0, 0, 0));
						}
					}
				}

				if (add && addDate >= fromDate.Date && addDate <= toDate.Date.AddDays(1))
				{
					list.Add(DBCommon.GetUTCDate(recurrence.TimeZoneId, addDate));	// to UTC

					if (maxCount > 0 && disp == Disposition.First && list.Count >= maxCount)
						break;

					if (maxCount > 0 && disp == Disposition.Last && list.Count > maxCount)
						list.RemoveAt(0);
				}

				if (add)
				{
					count++;
					if (recurrence.EndAfter > 0 && count >= recurrence.EndAfter)
						break;
				}
				else
					curDate = curDate.AddDays(1);
			}

			return list;
		}

		// fromDate, toDate, eventStartDate - in UTC format
		public static ArrayList GetRecurDates(DateTime fromDate, DateTime toDate, int startTime, DateTime eventStartDate, Recurrence recurrence)
		{
			return GetRecurDates(fromDate, toDate, startTime, eventStartDate, recurrence, 0, Disposition.First);
		}
		#endregion

		#region GetElapsedWeekDays
		// Вычисляет количество рабочих дней между двумя датами
		private static int GetElapsedWeekDays(DateTime fromDate, DateTime toDate)
		{
			// TODO: оптимизировать
			DateTime curDate = fromDate;
			int daysCount = 0;
			while (curDate < toDate)
			{
				if (!(curDate.DayOfWeek == DayOfWeek.Saturday || curDate.DayOfWeek == DayOfWeek.Sunday))
					daysCount++;
				curDate = curDate.AddDays(1);
			}
			return daysCount;
		}
		#endregion

		#region GetFirstWeekDay
		private static DateTime GetFirstWeekDay(DateTime fromDate)
		{
			while (fromDate.DayOfWeek == DayOfWeek.Saturday || fromDate.DayOfWeek == DayOfWeek.Sunday)
			{
				fromDate = fromDate.AddDays(1);
			}
			return fromDate;
		}
		#endregion

		#region AddWeekDays
		private static DateTime AddWeekDays(DateTime curDate, int interval)
		{
			if (interval == 0)
				throw new Exception("Zero frequency");

			int counter = 0;

			while (counter < interval)
			{
				curDate = curDate.AddDays(1);

				if (!(curDate.DayOfWeek == DayOfWeek.Saturday || curDate.DayOfWeek == DayOfWeek.Sunday))
					counter++;
			}
			return curDate;
		}
		#endregion

		#region GetBitDayOfWeek
		public static BitDayOfWeek GetBitDayOfWeek(DayOfWeek day)
		{
			switch (day)
			{
				case DayOfWeek.Monday:
					return BitDayOfWeek.Monday;
				case DayOfWeek.Tuesday:
					return BitDayOfWeek.Tuesday;
				case DayOfWeek.Wednesday:
					return BitDayOfWeek.Wednesday;
				case DayOfWeek.Thursday:
					return BitDayOfWeek.Thursday;
				case DayOfWeek.Friday:
					return BitDayOfWeek.Friday;
				case DayOfWeek.Saturday:
					return BitDayOfWeek.Saturday;
				case DayOfWeek.Sunday:
					return BitDayOfWeek.Sunday;
			}
			return BitDayOfWeek.Unknown;
		}
		#endregion

		#region GetDayOfWeekByBit
		public static DayOfWeek GetDayOfWeekByBit(BitDayOfWeek bitDayOfWeek)
		{
			switch (bitDayOfWeek)
			{
				case BitDayOfWeek.Monday:
					return DayOfWeek.Monday;
				case BitDayOfWeek.Tuesday:
					return DayOfWeek.Tuesday;
				case BitDayOfWeek.Wednesday:
					return DayOfWeek.Wednesday;
				case BitDayOfWeek.Thursday:
					return DayOfWeek.Thursday;
				case BitDayOfWeek.Friday:
					return DayOfWeek.Friday;
				case BitDayOfWeek.Saturday:
					return DayOfWeek.Saturday;
				case BitDayOfWeek.Sunday:
					return DayOfWeek.Sunday;
			}
			throw new Exception("Illegal day of week.");
		}
		#endregion

		#region GetRelativeWeekNumber
		private static int GetRelativeWeekNumber(DateTime day1, DateTime day2)
		{
			int week = 0;
			DateTime curDate = day1;
			TimeSpan span = day2 - day1;
			if (span.Days >= 7)
			{
				while (curDate.DayOfWeek != DayOfWeek.Monday)
				{
					curDate = curDate.AddDays(1);
					span = span.Subtract(new TimeSpan(1, 0, 0, 0));
				}
				if (curDate != day1)
					week++;
				week += span.Days / 7;
			}
			else
			{
				while (curDate.DayOfWeek != DayOfWeek.Monday && curDate < day2)
				{
					curDate = curDate.AddDays(1);
				}
				if (curDate != day2 || (curDate != day1 && curDate.DayOfWeek == DayOfWeek.Monday))
					week++;
			}

			return week;
		}
		#endregion

		#region GetRelativeMonthNumber
		private static int GetRelativeMonthNumber(DateTime day1, DateTime day2)
		{
			int month = 0;
			DateTime curDate = day1;
			TimeSpan span = day2 - day1;
			while (curDate < day2)
			{
				int daysLeft = DateTime.DaysInMonth(curDate.Year, curDate.Month) - curDate.Day;
				if (span.Days > daysLeft)
				{
					month++;
					curDate = curDate.AddDays(daysLeft + 1);
					span = span.Subtract(new TimeSpan(daysLeft + 1, 0, 0, 0));
				}
				else
					break;
			}
			return month;
		}
		#endregion

		#region GetWeekNumber
		private static WeekNumber GetWeekNumber(DateTime day)
		{
			WeekNumber ret;
			int i = 1;
			int d = day.Day - 7;
			while (d > 0)
			{
				d -= 7;
				i++;
			}
			ret = (WeekNumber)Enum.Parse(typeof(WeekNumber), i.ToString());
			int n = DateTime.DaysInMonth(day.Year, day.Month);

			if (n - day.Day < 7)
				ret |= WeekNumber.Last;
			return ret;
		}
		#endregion
		#endregion

		#region GetEventInstances
		internal static Hashtable GetEventInstances(int object_id, out int reminder_interval)
		{
			Hashtable ret = new Hashtable();

			reminder_interval = -1;

			DateTime StartDate;
			DateTime FinishDate;
			int HasRecurrence = 0;

			// Get info
			using (IDataReader reader = GetEvent(object_id, false, true))
			{
				reader.Read();

				StartDate = (DateTime)reader["StartDate"];
				FinishDate = (DateTime)reader["FinishDate"];
				HasRecurrence = (int)reader["HasRecurrence"];
				reminder_interval = (int)reader["ReminderInterval"];
			}

			if (HasRecurrence == 0)
			{
				ret[StartDate] = FinishDate;
			}
			else	// Recurrence
			{
				int StartTime;
				int EndTime;
				Recurrence recurrence;

				using (IDataReader reader = DBCommon.GetRecurrence((int)ObjectTypes.CalendarEntry, object_id))
				{
					reader.Read();
					recurrence = new Recurrence(
						(byte)reader["Pattern"],
						(byte)reader["SubPattern"],
						(byte)reader["Frequency"],
						(byte)reader["Weekdays"],
						(byte)reader["DayOfMonth"],
						(byte)reader["WeekNumber"],
						(byte)reader["MonthNumber"],
						(int)reader["EndAfter"],
						StartDate,
						FinishDate,
						(int)reader["TimeZoneId"]);
					StartTime = (int)reader["StartTime"];
					EndTime = (int)reader["EndTime"];
				}

				// Get new StartDate and FinishDate for recurrence TimeOffset (not UserTimeOffset)
				using (IDataReader reader = DBEvent.GetEventDates(object_id, recurrence.TimeZoneId))
				{
					reader.Read();
					recurrence.StartDate = ((DateTime)reader["StartDate"]).Date;
					recurrence.FinishDate = ((DateTime)reader["FinishDate"]).Date;
				}

				// from_date, to_date, StartDate - in UTC
				ArrayList dates = GetRecurDates(DBCommon.GetUTCDate(recurrence.TimeZoneId, recurrence.StartDate), DBCommon.GetUTCDate(recurrence.TimeZoneId, recurrence.FinishDate), StartTime, StartDate, recurrence);
				foreach (DateTime dt in dates)	// Dates in UTC
				{
					ret[dt.AddMinutes(StartTime)] = dt.AddMinutes(EndTime);
				}
			}

			return ret;
		}
		#endregion

		#region ShowAcceptDeny
		public static bool ShowAcceptDeny(int event_id)
		{
			bool retval = false;

			EventSecurity es = GetSecurity(event_id);
			retval = es.IsResource;

			if (retval)
			{
				bool Resource_MustBeConfirmed = false;
				bool Resource_IsResponsePending = false;

				using (IDataReader reader = DBEvent.GetResourceByUser(event_id, Security.CurrentUser.UserID))
				{
					if (reader.Read())
					{
						Resource_MustBeConfirmed = (bool)reader["MustBeConfirmed"];
						Resource_IsResponsePending = (bool)reader["ResponsePending"];
					}
				}
				retval = Resource_MustBeConfirmed && Resource_IsResponsePending;
			}
			return retval;
		}
		#endregion

		#region GetListPendingEvents
		/// <summary>
		///		EventId, Title, Location, PriorityId, PriorityName, TypeId, ManagerId, 
		///		StartDate, FinishDate, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListPendingEvents(int ProjectID)
		{
			return GetListPendingEvents(ProjectID, Security.CurrentUser.UserID);
		}

		public static DataTable GetListPendingEventsDataTable(int ProjectID)
		{
			return GetListPendingEventsDataTable(ProjectID, Security.CurrentUser.UserID);
		}

		public static IDataReader GetListPendingEvents(int ProjectID, int UserId)
		{
			return DBEvent.GetListPendingEvents(ProjectID, UserId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}

		public static DataTable GetListPendingEventsDataTable(int ProjectId, int UserId)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime UserDate = DBCommon.GetLocalDate(TimeZoneId, DateTime.UtcNow);	// current User's datetime
			DataRow row;

			DataTable table = new DataTable();
			table.Columns.Add("EventId", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("Location", typeof(string));
			table.Columns.Add("PriorityId", typeof(int));
			table.Columns.Add("PriorityName", typeof(string));
			table.Columns.Add("TypeId", typeof(int));
			table.Columns.Add("ManagerId", typeof(int));
			table.Columns.Add("StartDate", typeof(DateTime));
			table.Columns.Add("FinishDate", typeof(DateTime));
			table.Columns.Add("StateId", typeof(int));

			using (IDataReader reader = DBEvent.GetListPendingEvents(ProjectId, UserId, TimeZoneId, Security.CurrentUser.LanguageId))
			{
				while (reader.Read())
				{
					DateTime StartDate = (DateTime)reader["StartDate"];
					DateTime FinishDate = (DateTime)reader["FinishDate"];

					row = table.NewRow();
					row["EventId"] = (int)reader["EventId"];
					row["Title"] = reader["Title"].ToString();
					row["Location"] = reader["Location"].ToString();
					row["PriorityId"] = (int)reader["PriorityId"];
					row["PriorityName"] = reader["PriorityName"].ToString();
					row["TypeId"] = (int)reader["TypeId"];
					row["ManagerId"] = (int)reader["ManagerId"];
					row["StartDate"] = StartDate;
					row["FinishDate"] = FinishDate;
					row["StateId"] = (int)reader["StateId"];

					table.Rows.Add(row);
				}
			}

			return table;
		}
		#endregion

		#region GetListEventResourcesNotPending
		/// <summary>
		///  EventId, Title, PrincipalId, IsConfirmed, LastSavedDate, StartDate, FinishDate
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventResourcesNotPending()
		{
			return GetListEventResourcesNotPending(Security.CurrentUser.UserID);
		}

		/// <summary>
		///  EventId, Title, PrincipalId, IsConfirmed, LastSavedDate, StartDate, FinishDate, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEventResourcesNotPendingDataTable()
		{
			return GetListEventResourcesNotPendingDataTable(Security.CurrentUser.UserID);
		}

		/// <summary>
		///  EventId, Title, PrincipalId, IsConfirmed, LastSavedDate, StartDate, FinishDate
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventResourcesNotPending(int UserId)
		{
			return DBEvent.GetListEventResourcesNotPending(UserId, Security.CurrentUser.TimeZoneId);
		}

		/// <summary>
		///  EventId, Title, PrincipalId, IsConfirmed, LastSavedDate, StartDate, FinishDate, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEventResourcesNotPendingDataTable(int UserId)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime UserDate = DBCommon.GetLocalDate(TimeZoneId, DateTime.UtcNow);	// current User's datetime
			DataRow row;

			DataTable table = new DataTable();
			table.Columns.Add("EventId", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("PrincipalId", typeof(int));
			table.Columns.Add("IsConfirmed", typeof(bool));
			table.Columns.Add("LastSavedDate", typeof(DateTime));
			table.Columns.Add("StartDate", typeof(DateTime));
			table.Columns.Add("FinishDate", typeof(DateTime));
			table.Columns.Add("StateId", typeof(int));

			using (IDataReader reader = DBEvent.GetListEventResourcesNotPending(UserId, Security.CurrentUser.TimeZoneId))
			{
				while (reader.Read())
				{
					DateTime StartDate = (DateTime)reader["StartDate"];
					DateTime FinishDate = (DateTime)reader["FinishDate"];

					row = table.NewRow();
					row["EventId"] = (int)reader["EventId"];
					row["Title"] = reader["Title"].ToString();
					row["PrincipalId"] = (int)reader["PrincipalId"];
					row["IsConfirmed"] = (bool)reader["IsConfirmed"];
					row["LastSavedDate"] = (DateTime)reader["LastSavedDate"];
					row["StartDate"] = StartDate;
					row["FinishDate"] = FinishDate;
					row["StateId"] = (int)reader["StateId"];

					table.Rows.Add(row);
				}
			}
			return table;
		}
		#endregion

		#region GetListEventsForUser
		/// <summary>
		///		EventId, Title, Location, TypeId, StartDate, FinishDate, 
		///		ManagerId, PriorityId, Interval, Description, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventsForUser()
		{
			return DBEvent.GetListEventsForUser(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListEventsUpdatedByUser
		/// <summary>
		///		EventId, Title, Location, TypeId, StartDate, FinishDate, LastSavedDate, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventsUpdatedByUser(int Days, int ProjectId)
		{
			return DBEvent.GetListEventsUpdatedByUser(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}

		public static DataTable GetListEventsUpdatedByUserDataTable(int Days, int ProjectId)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime UserDate = DBCommon.GetLocalDate(TimeZoneId, DateTime.UtcNow);	// current User's datetime
			DataRow row;

			DataTable table = new DataTable();
			table.Columns.Add("EventId", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("Location", typeof(string));
			table.Columns.Add("TypeId", typeof(int));
			table.Columns.Add("StartDate", typeof(DateTime));
			table.Columns.Add("FinishDate", typeof(DateTime));
			table.Columns.Add("LastSavedDate", typeof(DateTime));
			table.Columns.Add("StateId", typeof(int));

			using (IDataReader reader = DBEvent.GetListEventsUpdatedByUser(ProjectId, Security.CurrentUser.UserID, TimeZoneId, Days))
			{
				while (reader.Read())
				{
					DateTime StartDate = (DateTime)reader["StartDate"];
					DateTime FinishDate = (DateTime)reader["FinishDate"];

					row = table.NewRow();
					row["EventId"] = (int)reader["EventId"];
					row["Title"] = reader["Title"].ToString();
					row["Location"] = reader["Location"].ToString();
					row["TypeId"] = (int)reader["TypeId"];
					row["StartDate"] = StartDate;
					row["FinishDate"] = FinishDate;
					row["LastSavedDate"] = (DateTime)reader["LastSavedDate"];
					row["StateId"] = (int)reader["StateId"];

					table.Rows.Add(row);
				}
			}
			return table;
		}
		#endregion

		#region GetListEventsUpdatedForUser
		/// <summary>
		///		EventId, Title, Location, TypeId, StartDate, FinishDate, 
		///		LastEditorId, LastSavedDate, ProjectId, ProjectName, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventsUpdatedForUser(int Days, int ProjectId)
		{
			return DBEvent.GetListEventsUpdatedForUser(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}

		public static DataTable GetListEventsUpdatedForUserDataTable(int Days, int ProjectId)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime UserDate = DBCommon.GetLocalDate(TimeZoneId, DateTime.UtcNow);	// current User's datetime
			DataRow row;

			DataTable table = new DataTable();
			table.Columns.Add("EventId", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("Location", typeof(string));
			table.Columns.Add("TypeId", typeof(int));
			table.Columns.Add("StartDate", typeof(DateTime));
			table.Columns.Add("FinishDate", typeof(DateTime));
			table.Columns.Add("LastEditorId", typeof(int));
			table.Columns.Add("LastSavedDate", typeof(DateTime));
			table.Columns.Add("StateId", typeof(int));

			using (IDataReader reader = DBEvent.GetListEventsUpdatedForUser(ProjectId, Security.CurrentUser.UserID, TimeZoneId, Days))
			{
				while (reader.Read())
				{
					DateTime StartDate = (DateTime)reader["StartDate"];
					DateTime FinishDate = (DateTime)reader["FinishDate"];

					row = table.NewRow();
					row["EventId"] = (int)reader["EventId"];
					row["Title"] = reader["Title"].ToString();
					row["Location"] = reader["Location"].ToString();
					row["TypeId"] = (int)reader["TypeId"];
					row["StartDate"] = StartDate;
					row["FinishDate"] = FinishDate;
					row["LastEditorId"] = (int)reader["LastEditorId"];
					row["LastSavedDate"] = (DateTime)reader["LastSavedDate"];
					row["StateId"] = (int)reader["StateId"];

					table.Rows.Add(row);
				}
			}
			return table;
		}
		#endregion

		#region GetListEventsByKeyword
		/// <summary>
		///  Reader returns fields:
		///		EventId, Title, Description, Location, TypeId, StartDate, FinishDate, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventsByKeyword(string Keyword)
		{
			return DBEvent.GetListEventsByKeyword(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Keyword);
		}
		#endregion

		#region UploadFile
		public static void UploadFile(int event_id, string FileName, System.IO.Stream _inputStream)
		{
			if (FileName != null)
			{
				string ContainerName = "FileLibrary";
				string ContainerKey = UserRoleHelper.CreateEventContainerKey(event_id);

				BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
				FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
				fs.SaveFile(fs.Root.Id, FileName, _inputStream);

				//Asset.Create(event_id,ObjectTypes.CalendarEntry, FileName,"", FileName, _inputStream, true);
			}
		}
		#endregion

		#region GetListEventsForChangeableRoles
		/// <summary>
		/// Reader returns fields:
		///  EventId, Title, IsManager, IsResource, CanView, CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventsForChangeableRoles(int UserId)
		{
			return DBEvent.GetListEventsForChangeableRoles(UserId, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListEventManagers
		/// <summary>
		/// UserId, FirstName, LastName, FullName, FullName2
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventManagers()
		{
			return DBEvent.GetListEventManagers();
		}
		#endregion

		#region GetListEventManagersDataTable
		/// <summary>
		/// UserId, FirstName, LastName, FullName, FullName2
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEventManagersDataTable()
		{
			return DBEvent.GetListEventManagersDataTable();
		}
		#endregion

		#region GetListEventCreators
		/// <summary>
		/// UserId, FirstName, LastName, FullName, FullName2
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEventCreators()
		{
			return DBEvent.GetListEventCreators();
		}
		#endregion

		#region GetListEventCreatorsDataTable
		/// <summary>
		/// UserId, FirstName, LastName, FullName, FullName2
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEventCreatorsDataTable()
		{
			return DBEvent.GetListEventCreatorsDataTable();
		}
		#endregion

		#region GetListCategoriesByUser
		/// <summary>
		/// Reader returns fields:
		///		CategoryId, UserId
		/// </summary>
		public static IDataReader GetListCategoriesByUser()
		{
			return DBCommon.GetListCategoriesByUser(Security.CurrentUser.UserID);
		}
		#endregion

		#region UpdateDiscussion
		public static void UpdateDiscussion(
			int DiscussionId, string Text)
		{
			Project.UpdateDiscussion(DiscussionId, Text);
		}
		#endregion

		#region GetTotalCountWithTypes
		/// <summary>
		/// Reader returns fields:
		///		Type, Count.
		/// </summary>
		public static IDataReader GetTotalCountWithTypes()
		{
			return DBEvent.GetTotalCountWithType(Security.CurrentUser.UserID, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetCalendarXml
		//TODO: When this method is called from IBNProject webservices dateTime and dateTo should be in UTC
		public static void GetCalendarXml(XmlDocument retValDoc, DateTime dateFrom, DateTime dateTo)
		{
			XmlNode eventRoot = retValDoc.SelectSingleNode("ibnCalendar/events");

			using (IDataReader reader = CalendarEntry.GetListEventsForUser())
			{
				while (reader.Read())
				{
					XmlDocumentFragment EventItem = retValDoc.CreateDocumentFragment();
					EventItem.InnerXml = "<event><id/><title/><description/><location/><startDate/><finishDate/><priority/><reminderInterval/><resources/></event>";

					XmlDocumentFragment ResourceItem = retValDoc.CreateDocumentFragment();
					ResourceItem.InnerXml = "<resource><login/><type/><name/><address/></resource>";

					int EventId = (int)reader["EventId"];
					int ManagerId = (int)reader["ManagerId"];

					DateTime StartDate = (DateTime)reader["StartDate"];
					DateTime FinishDate = (DateTime)reader["FinishDate"];

					Recurrence recurrence = new Recurrence(0, 0, 0, 0, 0, 0, 0, 0, DateTime.Now, DateTime.Now, 0);
					int StartTime = 0;
					int EndTime = 0;
					bool HasRecurrence = false;

					bool bAddEvent = false;

					using (IDataReader rr_reader = DBCommon.GetRecurrence((int)CALENDAR_ENTRY_TYPE, EventId))
					{
						if (rr_reader.Read())	// recurrence exists
						{

							recurrence = new Recurrence(
								(byte)rr_reader["Pattern"],
								(byte)rr_reader["SubPattern"],
								(byte)rr_reader["Frequency"],
								(byte)rr_reader["Weekdays"],
								(byte)rr_reader["DayOfMonth"],
								(byte)rr_reader["WeekNumber"],
								(byte)rr_reader["MonthNumber"],
								(int)rr_reader["EndAfter"],
								StartDate,
								FinishDate,
								(int)rr_reader["TimeZoneId"]);

							StartTime = (int)rr_reader["StartTime"];
							EndTime = (int)rr_reader["EndTime"];

							HasRecurrence = true;
						}
					}

					ArrayList RecurDatesList = null;

					if (HasRecurrence)
					{
						RecurDatesList = CalendarEntry.GetRecurDates(dateFrom, dateTo, StartTime, Security.CurrentUser.CurrentTimeZone.ToUniversalTime(StartDate), recurrence);
						bAddEvent = (RecurDatesList.Count > 0);
					}
					else
					{
						if (StartDate >= dateFrom && StartDate <= dateTo)
							bAddEvent = true;
					}

					if (bAddEvent)
					{
						ArrayList addedUserId = new ArrayList();

						EventItem.SelectSingleNode("event/id").InnerText = ((int)reader["EventId"]).ToString();
						EventItem.SelectSingleNode("event/title").InnerText = (string)reader["Title"];
						EventItem.SelectSingleNode("event/description").InnerText = (string)reader["Description"];
						EventItem.SelectSingleNode("event/location").InnerText = (string)reader["Location"];
						EventItem.SelectSingleNode("event/startDate").InnerText = ((DateTime)reader["StartDate"]).ToString("s");
						EventItem.SelectSingleNode("event/finishDate").InnerText = ((DateTime)reader["FinishDate"]).ToString("s");
						EventItem.SelectSingleNode("event/priority").InnerText = ((int)reader["PriorityId"]).ToString();
						EventItem.SelectSingleNode("event/reminderInterval").InnerText = ((int)reader["Interval"]).ToString();

						XmlNode resourceRoot = EventItem.SelectSingleNode("event/resources");

						using (IDataReader r_reader = User.GetUserInfo(ManagerId))
						{
							if (r_reader.Read())
							{
								ResourceItem.SelectSingleNode("resource/login").InnerText = (string)r_reader["Login"];
								ResourceItem.SelectSingleNode("resource/type").InnerText = "manager";
								ResourceItem.SelectSingleNode("resource/name").InnerText = string.Format("{0} {1}", r_reader["FirstName"], r_reader["LastName"]);
								ResourceItem.SelectSingleNode("resource/address").InnerText = (string)r_reader["Email"];

								resourceRoot.AppendChild(ResourceItem.CloneNode(true));
								addedUserId.Add(ManagerId);
							}
						}


						using (IDataReader r_reader = CalendarEntry.GetListResources(EventId))
						{
							while (r_reader.Read())
							{
								int PrincipalId = (int)r_reader["PrincipalId"];
								bool IsGroup = (bool)r_reader["IsGroup"];
								bool MustBeConfirmed = (bool)r_reader["MustBeConfirmed"];
								bool ResponsePending = (bool)r_reader["ResponsePending"];
								bool IsConfirmed = (bool)r_reader["IsConfirmed"];

								if (IsGroup)
								{
									using (IDataReader g_reader = SecureGroup.GetGroup(PrincipalId))
									{
										if (g_reader.Read())
										{
											ResourceItem.SelectSingleNode("resource/login").InnerText = "";
											ResourceItem.SelectSingleNode("resource/type").InnerText = "group";
											ResourceItem.SelectSingleNode("resource/name").InnerText = Common.GetWebResourceString(g_reader["GroupName"].ToString());
											ResourceItem.SelectSingleNode("resource/address").InnerText = "";

											resourceRoot.AppendChild(ResourceItem.CloneNode(true));
										}
									}
								}
								else if (!MustBeConfirmed ||
									MustBeConfirmed && ResponsePending ||
									MustBeConfirmed && !ResponsePending && IsConfirmed)
								{
									if (!addedUserId.Contains(PrincipalId))
									{
										using (IDataReader u_reader = User.GetUserInfo(PrincipalId))
										{
											if (u_reader.Read())
											{
												ResourceItem.SelectSingleNode("resource/login").InnerText = (string)u_reader["Login"];
												ResourceItem.SelectSingleNode("resource/type").InnerText = MustBeConfirmed ? "optional" : "required";
												ResourceItem.SelectSingleNode("resource/name").InnerText = string.Format("{0} {1}", u_reader["FirstName"], u_reader["LastName"]);
												ResourceItem.SelectSingleNode("resource/address").InnerText = (string)u_reader["Email"];

												resourceRoot.AppendChild(ResourceItem.CloneNode(true));
												addedUserId.Add(ManagerId);
											}
										}
									}
								}
							}
						}

						if (HasRecurrence)	// recurrence exists
						{
							XmlDocumentFragment RecurrenceItem = retValDoc.CreateDocumentFragment();
							RecurrenceItem.InnerXml = "<recurrence><recurrenceType></recurrenceType><interval/><dayOfWeek/><dayOfMonth/><monthOfYear/><instance/><startTime/><endTime/><occurrences/></recurrence>";

							// Common Information [2/9/2005]
							RecurrenceItem.SelectSingleNode("recurrence/recurrenceType").InnerText = "";
							RecurrenceItem.SelectSingleNode("recurrence/startTime").InnerText = string.Format("{0:d2}:{1:d2}:00", StartTime / 60, StartTime % 60);
							RecurrenceItem.SelectSingleNode("recurrence/endTime").InnerText = string.Format("{0:d2}:{1:d2}:00", EndTime / 60, EndTime % 60);
							RecurrenceItem.SelectSingleNode("recurrence/occurrences").InnerText = recurrence.EndAfter.ToString();

							//								RecurrenceItem.SelectSingleNode("recurrence/interval").InnerText = rr_reader["Frequency"].ToString();
							//								RecurrenceItem.SelectSingleNode("recurrence/dayOfWeek").InnerText = rr_reader["Weekdays"].ToString();
							//								RecurrenceItem.SelectSingleNode("recurrence/dayOfMonth").InnerText = rr_reader["DayOfMonth"].ToString();
							//								RecurrenceItem.SelectSingleNode("recurrence/monthOfYear").InnerText = rr_reader["MonthNumber"].ToString();
							//								RecurrenceItem.SelectSingleNode("recurrence/instance").InnerText = rr_reader["WeekNumber"].ToString(); // ??? MonthNumber

							// 1-Daily 2-Weekly 3-Monthly 4-Yearly
							switch ((int)recurrence.Pattern)
							{
								case 1:
									if (recurrence.SubPattern == RecurSubPattern.SubPattern1)
									{
										RecurrenceItem.SelectSingleNode("recurrence/recurrenceType").InnerText = "daily";
										RecurrenceItem.SelectSingleNode("recurrence/interval").InnerText = recurrence.Frequency.ToString();
										RecurrenceItem.SelectSingleNode("recurrence/dayOfWeek").InnerText = "0";
									}
									else // SubPattern 2
									{
										if (recurrence.Frequency == 1)
										{
											RecurrenceItem.SelectSingleNode("recurrence/recurrenceType").InnerText = "weekly";
											RecurrenceItem.SelectSingleNode("recurrence/interval").InnerText = "1";// [2/11/2005] recurrence.Frequency.ToString();
											RecurrenceItem.SelectSingleNode("recurrence/dayOfWeek").InnerText = ((int)BitDayOfWeek.Weekdays).ToString();
										}
										else
										{
											//HasRecurrence = false;
											// Unpack Recurring calendar Item [2/11/2005]
											foreach (DateTime dt in RecurDatesList)
											{
												DateTime UserDt = DBCommon.GetLocalDate(Security.CurrentUser.TimeZoneId, dt);	// from UTC to User's time
												DateTime _StartDate = UserDt.AddMinutes(StartTime);
												DateTime _FinishDate = UserDt.AddMinutes(EndTime);

												XmlNode xmlRecEvent = EventItem.CloneNode(true);

												xmlRecEvent.SelectSingleNode("event/startDate").InnerText = _StartDate.ToString("s");
												xmlRecEvent.SelectSingleNode("event/finishDate").InnerText = _FinishDate.ToString("s");

												eventRoot.AppendChild(xmlRecEvent);
											}

											continue;
										}
									}
									break;
								case 2:
									RecurrenceItem.SelectSingleNode("recurrence/recurrenceType").InnerText = "weekly";
									RecurrenceItem.SelectSingleNode("recurrence/interval").InnerText = recurrence.Frequency.ToString();
									RecurrenceItem.SelectSingleNode("recurrence/dayOfWeek").InnerText = recurrence.WeekDays.ToString();
									break;
								case 3:
									if (recurrence.SubPattern == RecurSubPattern.SubPattern1)
									{
										RecurrenceItem.SelectSingleNode("recurrence/recurrenceType").InnerText = "monthly";
										RecurrenceItem.SelectSingleNode("recurrence/interval").InnerText = recurrence.Frequency.ToString();
										RecurrenceItem.SelectSingleNode("recurrence/dayOfMonth").InnerText = recurrence.DayOfMonth.ToString();

									}
									else // SubPattern 2
									{
										RecurrenceItem.SelectSingleNode("recurrence/recurrenceType").InnerText = "monthNth";
										RecurrenceItem.SelectSingleNode("recurrence/interval").InnerText = recurrence.Frequency.ToString();
										RecurrenceItem.SelectSingleNode("recurrence/dayOfWeek").InnerText = recurrence.WeekDays.ToString();
										RecurrenceItem.SelectSingleNode("recurrence/instance").InnerText = recurrence.RecurWeekNumber.ToString();
									}
									break;
								case 4:
									if (recurrence.SubPattern == RecurSubPattern.SubPattern1)
									{
										RecurrenceItem.SelectSingleNode("recurrence/recurrenceType").InnerText = "yearly";
										RecurrenceItem.SelectSingleNode("recurrence/dayOfMonth").InnerText = recurrence.DayOfMonth.ToString();
										RecurrenceItem.SelectSingleNode("recurrence/monthOfYear").InnerText = recurrence.MonthNumber.ToString();
									}
									else // SubPattern 2
									{
										RecurrenceItem.SelectSingleNode("recurrence/recurrenceType").InnerText = "yearNth";
										RecurrenceItem.SelectSingleNode("recurrence/monthOfYear").InnerText = recurrence.MonthNumber.ToString();
										RecurrenceItem.SelectSingleNode("recurrence/dayOfWeek").InnerText = recurrence.WeekDays.ToString();
										RecurrenceItem.SelectSingleNode("recurrence/instance").InnerText = recurrence.RecurWeekNumber.ToString();

									}
									break;
							}


							//
							EventItem.SelectSingleNode("event").AppendChild(RecurrenceItem);
						}


						eventRoot.AppendChild(EventItem);
					}
				}
			}
		}
		#endregion

		#region AddFavorites
		public static void AddFavorites(int EventId)
		{
			DBCommon.AddFavorites((int)CALENDAR_ENTRY_TYPE, EventId, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListFavorites
		/// <summary>
		///		FavoriteId, ObjectTypeId, ObjectId, UserId, Title 
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListFavoritesDT()
		{
			return DBCommon.GetListFavoritesDT((int)CALENDAR_ENTRY_TYPE, Security.CurrentUser.UserID);
		}
		#endregion

		#region CheckFavorites
		public static bool CheckFavorites(int EventId)
		{
			return DBCommon.CheckFavorites((int)CALENDAR_ENTRY_TYPE, EventId, Security.CurrentUser.UserID);
		}
		#endregion

		#region DeleteFavorites
		public static void DeleteFavorites(int EventId)
		{
			DBCommon.DeleteFavorites((int)CALENDAR_ENTRY_TYPE, EventId, Security.CurrentUser.UserID);
		}
		#endregion

		#region AddHistory
		public static void AddHistory(int EventId, string Title)
		{
			DBCommon.AddHistory((int)CALENDAR_ENTRY_TYPE, EventId, Title, Security.CurrentUser.UserID);
		}
		#endregion

		#region CanChangeProject
		public static bool CanChangeProject(int eventId)
		{
			// O.R. [2008-07-30] Check that there are no finances
			bool retval = true;
			if (ActualFinances.List(eventId, ObjectTypes.CalendarEntry).Length > 0)
				retval = false;

			return retval;
		}
		#endregion

		#region RecalculateState
		public static void RecalculateState(int event_id)
		{
			/// OldState, NewState
			int OldStateId = -1;
			int NewStateId = -1;

			using (IDataReader reader = DBEvent.RecalculateState(event_id, DateTime.UtcNow))
			{
				if (reader.Read())
				{
					if (reader["OldStateId"] != DBNull.Value)
						OldStateId = (int)reader["OldStateId"];
					NewStateId = (int)reader["NewStateId"];
				}
			}

			if (OldStateId != NewStateId)
				SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_State, event_id);
		}
		#endregion

		#region GetEventTitle
		public static string GetEventTitle(int EventId)
		{
			string EventTitle = "";
			using (IDataReader reader = GetEvent(EventId, false))
			{
				if (reader.Read())
					EventTitle = reader["Title"].ToString();
			}
			return EventTitle;
		}
		#endregion

		#region GetProject
		public static int GetProject(int EventId)
		{
			return DBEvent.GetProject(EventId);
		}
		#endregion

		#region ConfirmReminder(...)
		internal static bool ConfirmReminder(DateTypes dateType, int objectId, bool hasRecurrence)
		{
			int stateId = DBEvent.GetState(objectId);
			return Common.ConfirmReminder(
				hasRecurrence
				, (int)dateType
				, stateId
				, (int)DateTypes.CalendarEntry_StartDate
				, (int)DateTypes.CalendarEntry_FinishDate
				, (int)ObjectStates.Upcoming
				, (int)ObjectStates.Completed
				, (int)ObjectStates.Suspended
				);
		}
		#endregion
	}
}
