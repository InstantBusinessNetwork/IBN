using System;
using System.Collections;
using System.Data;

using Mediachase.Ibn;
using Mediachase.IBN.Database;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Business
{
	public class CalendarEntry2
	{
		// Private
		private const ObjectTypes OBJECT_TYPE = ObjectTypes.CalendarEntry;
		#region VerifyCanUpdate()
		private static void VerifyCanUpdate(int objectId)
		{
			if(!CalendarEntry.CanUpdate(objectId))
				throw new AccessDeniedException();
		}
		#endregion
		#region UpdateProjectAndManager()
		private static void UpdateProjectAndManager(int objectId, int projectId, int managerId)
		{
			int oldProjectId = DBEvent.GetProject(objectId);
			int oldManagerId = DBEvent.GetManager(objectId);

			if (managerId == 0) // Don't change manager
				managerId = oldManagerId;

			if(projectId == 0) // Don't change project
				projectId = oldProjectId;

			if(projectId != oldProjectId || managerId != oldManagerId)
			{
				using(DbTransaction tran = DbTransaction.Begin())
				{
					DbCalendarEntry2.UpdateProjectAndManager(objectId, projectId, managerId);

					// OZ: User Role Addon
					if(managerId != oldManagerId)
					{
						UserRoleHelper.DeleteEventManagerRole(objectId, oldManagerId);
						UserRoleHelper.AddEventManagerRole(objectId, managerId);
					}

					if(projectId != oldProjectId)
					{
						ForeignContainerKey.Delete(UserRoleHelper.CreateEventContainerKey(objectId),UserRoleHelper.CreateProjectContainerKey(oldProjectId));
						if(projectId > 0)
							ForeignContainerKey.Add(UserRoleHelper.CreateEventContainerKey(objectId),UserRoleHelper.CreateProjectContainerKey(projectId));
					}
					// end OZ

					if(projectId != oldProjectId)
					{
						if(projectId > 0)
							SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_CalendarEntryList_CalendarEntryAdded, projectId, objectId);
						else
							SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_CalendarEntryList_CalendarEntryDeleted, oldProjectId, objectId);
					}

					if(managerId != oldManagerId)
					{
						SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_Manager_ManagerDeleted, objectId, oldManagerId);
						SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_Manager_ManagerAdded, objectId, managerId);
					}

					tran.Commit();
				}
			}
		}
		#endregion
		#region UpdateListResources()
		private static void UpdateListResources(int objectId, DataTable items, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(objectId);

			ArrayList oldItems = new ArrayList();
			using(IDataReader reader = DBEvent.GetListResources(objectId, Security.CurrentUser.TimeZoneId))
			{
				Common.LoadPrincipals(reader, oldItems);
			}

			ArrayList add = new ArrayList();
			ArrayList del = new ArrayList();
			foreach(DataRow row in items.Rows)
			{
				int id = (int)row["PrincipalId"];
				if(oldItems.Contains(id))
					oldItems.Remove(id);
				else
					add.Add(id);
			}

			del.AddRange(oldItems);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				foreach(int id in del)
				{
					DBEvent.DeleteResource(objectId, id);

					// OZ: User Role Addon
					UserRoleHelper.DeleteEventResourceRole(objectId, id);
					//

					SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_ResourceList_AssignmentDeleted, objectId, id);
				}

				foreach(DataRow row in items.Rows)
				{
					int id = (int)row["PrincipalId"];
					bool mustBeConfirmed = (bool)row["MustBeConfirmed"];
					bool updated = true;
					
					if(add.Contains(id))
					{
						DbCalendarEntry2.ResourceAdd(objectId, id, mustBeConfirmed);
						if(User.IsExternal(id))
							DBCommon.AddGate((int)OBJECT_TYPE, objectId, id);
					}
					else
						updated = (0 < DbCalendarEntry2.ResourceUpdate(objectId, id, mustBeConfirmed));

					// OZ: User Role Addon
					UserRoleHelper.AddEventResourceRole(objectId, id);
					// end
					
					if(updated)
					{
						if(mustBeConfirmed)
							SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_ResourceList_RequestAdded, objectId, id);
						else
							SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_ResourceList_AssignmentAdded, objectId, id);
					}
				}

				tran.Commit();
			}
		}
		#endregion


		// Public
		#region UpdateGeneralInfo()
		public static void UpdateGeneralInfo(int eventId, int typeId, string title, string description, string location)
		{
			UpdateGeneralInfo(eventId, typeId, title, description, location, true);
		}

		internal static void UpdateGeneralInfo(int eventId, int typeId, string title, string description, string location, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(eventId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DbCalendarEntry2.UpdateGeneralInfo(eventId, typeId, title, description, location);
				SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_GeneralInfo, eventId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateTimeline()
		public static void UpdateTimeline(int eventId, DateTime startDate, DateTime finishDate)
		{
			UpdateTimeline(eventId, startDate, finishDate, true);
		}

		internal static void UpdateTimeline(int eventId, DateTime startDate, DateTime finishDate, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(eventId);

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			startDate = DBCommon.GetUTCDate(TimeZoneId, startDate);
			finishDate = DBCommon.GetUTCDate(TimeZoneId, finishDate);

			int projectId = -1;
			bool hasRecurrence = false;
			using (IDataReader reader = CalendarEntry.GetEvent(eventId, false))
			{
				if (reader.Read())
				{
					if (reader["ProjectId"] != DBNull.Value)
						projectId = (int)reader["ProjectId"];
					hasRecurrence = ((int)reader["HasRecurrence"] == 1);
				}
			}

			// O.R. [2009-12-18] Dates for recurrent events should be chnaged in recurrence editing.
			if (hasRecurrence)
				return;

			TimeSpan ts = finishDate.Subtract(startDate);
			int taskTime = (int)ts.TotalMinutes;

			using(DbTransaction tran = DbTransaction.Begin())
			{
				int retVal = DbCalendarEntry2.UpdateTimeline(eventId, startDate, finishDate, taskTime);
				if(retVal > 0)
				{
					SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_Timeline, eventId);

					if (!hasRecurrence)
					{
						if (retVal == 1 || retVal == 3)
							Schedule.UpdateDateTypeValue(DateTypes.CalendarEntry_StartDate, eventId, startDate);
						if (retVal == 2 || retVal == 3)
							Schedule.UpdateDateTypeValue(DateTypes.CalendarEntry_FinishDate, eventId, finishDate);
					}
					else
					{
						// O.R.[2009-09-29]: Recalculate events dates for scheduling
						Schedule.DeleteDateTypeValue(DateTypes.CalendarEntry_StartDate, eventId);
						Schedule.DeleteDateTypeValue(DateTypes.CalendarEntry_FinishDate, eventId);

						int ReminderInterval;
						Hashtable ret = CalendarEntry.GetEventInstances(eventId, out ReminderInterval);
						foreach (DateTime dtStart in ret.Keys)
						{
							DateTime dtEnd = (DateTime)ret[dtStart];
							if (dtStart > DateTime.UtcNow && dtEnd > DateTime.UtcNow)
							{
								DbSchedule2.AddNewDateTypeValue((int)DateTypes.CalendarEntry_StartDate, eventId, dtStart);
								DbSchedule2.AddNewDateTypeValue((int)DateTypes.CalendarEntry_FinishDate, eventId, dtEnd);
							}
						}
					}

					// O.R: Recalculating project TaskTime
					if (projectId > 0)
						TimeTracking.RecalculateProjectTaskTime(projectId);
				}

				CalendarEntry.RecalculateState(eventId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdatePriority()
		public static void UpdatePriority(int eventId, int priorityId)
		{
			UpdatePriority(eventId, priorityId, true);
		}

		internal static void UpdatePriority(int eventId, int priorityId, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(eventId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if(0 < DbCalendarEntry2.UpdatePriority(eventId, priorityId))
					SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_Priority, eventId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateManager()
		public static void UpdateManager(int eventId, int userId)
		{
			UpdateManager(eventId, userId, true);
		}

		internal static void UpdateManager(int eventId, int userId, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(eventId);

			UpdateProjectAndManager(eventId, 0, userId);
		}
		#endregion

		#region UpdateProject()
		public static void UpdateProject(int eventId, int projectId)
		{
			UpdateProject(eventId, projectId, true);
		}

		internal static void UpdateProject(int eventId, int projectId, bool checkAccess)
		{
			bool update = true;
			if (checkAccess)
			{
				int oldProjectId = DBEvent.GetProject(eventId);
				if (oldProjectId != projectId && !CalendarEntry.CanChangeProject(eventId))
					throw new AccessDeniedException();

				if (oldProjectId == projectId)
					update = false;
			}

			if (update)
				UpdateProjectAndManager(eventId, projectId, 0);
		}
		#endregion

		#region UpdateResources()
		public static void UpdateResources(int eventId, DataTable resources)
		{
			UpdateListResources(eventId, resources, true);
		}
		#endregion

		#region AcceptResource
		public static void AcceptResource(int eventId)
		{
			int userId = Security.CurrentUser.UserID;

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DbCalendarEntry2.ResourceReply(eventId, userId, true);
				
				SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_ResourceList_RequestAccepted, eventId, userId);

				tran.Commit();
			}
		}
		#endregion
		#region DeclineResource
		public static void DeclineResource(int eventId)
		{
			int userId = Security.CurrentUser.UserID;

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DbCalendarEntry2.ResourceReply(eventId, userId, false);

				SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_ResourceList_RequestDenied, eventId, userId);

				tran.Commit();
			}
		}
		#endregion

		// Categories
		#region UpdateCategories()
		public static void UpdateCategories(ListAction action, int eventId, ArrayList categories)
		{
			UpdateCategories(action, eventId, categories, true);
		}

		internal static void UpdateCategories(ListAction action, int eventId, ArrayList categories, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(eventId);

			ArrayList oldCategories = new ArrayList();
			using(IDataReader reader = CalendarEntry.GetListCategories(eventId))
			{
				Common.LoadCategories(reader, oldCategories);
			}
			Common.UpdateList(action, oldCategories, categories, OBJECT_TYPE, eventId, SystemEventTypes.CalendarEntry_Updated_GeneralCategories, new UpdateListDelegate(Common.ListUpdate), null);
		}
		#endregion

		#region AddGeneralCategories()
		public static void AddGeneralCategories(int eventId, ArrayList categories)
		{
			UpdateCategories(ListAction.Add, eventId, categories);
		}
		#endregion
		#region RemoveGeneralCategories()
		public static void RemoveGeneralCategories(int eventId, ArrayList categories)
		{
			UpdateCategories(ListAction.Remove, eventId, categories);
		}
		#endregion
		#region SetGeneralCategories()
		public static void SetGeneralCategories(int eventId, ArrayList categories)
		{
			UpdateCategories(ListAction.Set, eventId, categories);
		}
		#endregion

		// Batch
		#region Update
		public static void Update(
			int eventId,
			string title,
			string description,
			string location,
			int projectId,
			int managerId,
			int priorityId,
			int typeId,
			DateTime startDate, 
			DateTime finishDate,
			ArrayList categories,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			VerifyCanUpdate(eventId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				UpdateGeneralInfo(eventId, typeId, title, description, location, false);
				UpdateProjectAndManager(eventId, projectId, managerId);
				UpdatePriority(eventId, priorityId, false);
				UpdateTimeline(eventId, startDate, finishDate, false);
				UpdateCategories(ListAction.Set, eventId, categories, false);
				UpdateClient(eventId, contactUid, orgUid, false);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateClient()
		public static void UpdateClient(int eventId, PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			UpdateClient(eventId, contactUid, orgUid, true);
		}

		internal static void UpdateClient(int eventId, PrimaryKeyId contactUid, PrimaryKeyId orgUid, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(eventId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if (0 < DbCalendarEntry2.UpdateClient(eventId, contactUid == PrimaryKeyId.Empty ? null : (object)contactUid, orgUid == PrimaryKeyId.Empty ? null : (object)orgUid))
				{
					// TODO:
					//SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_Client, projectId);
				}

				tran.Commit();
			}
		}
		#endregion
	}
}
