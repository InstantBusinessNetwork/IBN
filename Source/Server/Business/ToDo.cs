using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Resources;

using Mediachase.Ibn;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Business.SpreadSheet;
using Mediachase.IBN.Database;
using Mediachase.MetaDataPlus;

namespace Mediachase.IBN.Business
{
	#region CompletionReason
	public enum CompletionReason
	{
		NotCompleted = -1,
		CompletedManually = 1,
		CompletedAutomatically = 2,
		SuspendedManually = 3,
		SuspendedAutomatically = 4
	}
	#endregion

	/// <summary>
	/// 
	/// </summary>
	public class ToDo
	{
		#region TODO_TYPE
		private static ObjectTypes TODO_TYPE
		{
			get { return ObjectTypes.ToDo; }
		}
		#endregion

		#region Tracking
		public struct Tracking
		{
			public bool ShowAcceptDeny;
			public bool ShowPersonalStatus;
			public bool ShowPersonalStatusOnly;
			public bool ShowOverallStatus;
			public bool ShowOverallStatusOnly;
			public bool ShowActivate;
			public bool ShowComplete;
			public bool ShowSuspend;
			public bool ShowUncomplete;
			public bool ShowResume;
			public bool ShowTimeTracking;

			public Tracking(
				bool showAcceptDeny,
				bool showPersonalStatus,
				bool showPersonalStatusOnly,
				bool showOverallStatus,
				bool showOverallStatusOnly,
				bool showActivate,
				bool showComplete,
				bool showSuspend,
				bool showUncomplete,
				bool showResume,
				bool showTimeTracking)
			{
				ShowAcceptDeny = showAcceptDeny;
				ShowPersonalStatus = showPersonalStatus;
				ShowPersonalStatusOnly = showPersonalStatusOnly;
				ShowOverallStatus = showOverallStatus;
				ShowOverallStatusOnly = showOverallStatusOnly;
				ShowActivate = showActivate;
				ShowComplete = showComplete;
				ShowSuspend = showSuspend;
				ShowUncomplete = showUncomplete;
				ShowResume = showResume;
				ShowTimeTracking = showTimeTracking;
			}
		}
		#endregion

		#region ToDoSecurity
		public class ToDoSecurity
		{
			public bool IsResource = false;
			public bool IsManager = false;
			public bool IsCreator = false;

			public ToDoSecurity(int todo_id, int user_id)
			{
				using (IDataReader reader = DBToDo.GetSecurityForUser(todo_id, user_id))
				{
					if (reader.Read())
					{
						IsResource = ((int)reader["IsResource"] > 0) ? true : false;
						IsManager = ((int)reader["IsManager"] > 0) ? true : false;
						IsCreator = ((int)reader["IsCreator"] > 0) ? true : false;
					}
				}
			}
		}
		#endregion

		#region CanCreate
		public static bool CanCreate(int projectId)
		{
			bool retVal = false;
			if (projectId > 0)
			{
				if (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager))
				{
					retVal = true;
				}

				if (!retVal)
				{
					Project.ProjectSecurity ps = Project.GetSecurity(projectId);
					Dictionary<string, string> prop = Project.GetProperties(projectId);
					retVal = ps.IsManager
						|| (ps.IsTeamMember && prop.ContainsKey(Project.CanProjectTeamCreateTodo))
						|| (ps.IsSponsor && prop.ContainsKey(Project.CanProjectSponsorCreateTodo))
						|| (ps.IsStakeHolder && prop.ContainsKey(Project.CanProjectStakeholderCreateTodo))
						|| (ps.IsExecutiveManager && prop.ContainsKey(Project.CanProjectExecutiveCreateTodo));
				}
			}
			else
			{
				retVal = true;
			}

			if (retVal)
			{
				using (IDataReader reader = DBUser.GetUserInfo(Security.CurrentUser.UserID))
				{
					if (reader.Read())
						retVal = (bool)reader["IsActive"];
				}
			}

			return retVal;
		}
		#endregion

		#region CanUpdate
		public static bool CanUpdate(int todo_id)
		{
			int task_id = -1;
			int doc_id = -1;
			int issue_id = -1;
			int project_id = -1;
			using (IDataReader reader = GetToDo(todo_id, false))
			{
				if (reader.Read())
				{
					if (reader["TaskId"] != DBNull.Value)
						task_id = (int)reader["TaskId"];
					if (reader["DocumentId"] != DBNull.Value)
						doc_id = (int)reader["DocumentId"];
					if (reader["IncidentId"] != DBNull.Value)
						issue_id = (int)reader["IncidentId"];
					if (reader["ProjectId"] != DBNull.Value)
						project_id = (int)reader["ProjectId"];
				}
			}

			bool retval = false;

			if (project_id > 0)
			{
				Mediachase.IBN.Business.Project.ProjectSecurity psec = Project.GetSecurity(project_id);
				retval = psec.IsManager;
			}

			if (!retval)
			{
				if (task_id > 0)
				{
					retval = CanUpdate(todo_id, task_id);
				}
				else if (doc_id > 0)
				{
					retval = CanUpdateDocumentTodo(todo_id, doc_id);
				}
				else if (issue_id > 0)
				{
					retval = CanUpdateIncidentTodo(todo_id, issue_id);
				}
				else
				{
					ToDoSecurity sec = GetSecurity(todo_id);
					retval = sec.IsManager || sec.IsCreator || Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager);
				}
			}

			return retval;
		}

		public static bool CanUpdate(int todo_id, int task_id)
		{
			bool retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager);

			if (!retval)
			{
				ToDoSecurity sec = GetSecurity(todo_id);
				retval = sec.IsManager || sec.IsCreator;
			}

			if (!retval)
				retval = Task.GetSecurity(task_id).IsManager;

			return retval;
		}

		public static bool CanUpdateIncidentTodo(int todo_id, int incident_id)
		{
			bool retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager);

			if (!retval)
			{
				ToDoSecurity sec = GetSecurity(todo_id);
				retval = sec.IsManager || sec.IsCreator;
			}

			if (!retval)
				retval = Incident.GetSecurity(incident_id).IsManager;

			return retval;
		}

		public static bool CanUpdateDocumentTodo(int todo_id, int document_id)
		{
			bool retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager);

			if (!retval)
			{
				ToDoSecurity sec = GetSecurity(todo_id);
				retval = sec.IsManager || sec.IsCreator;
			}

			if (!retval)
				retval = Document.GetSecurity(document_id).IsManager;

			return retval;
		}
		#endregion

		#region CanDelete
		public static bool CanDelete(int todo_id)
		{
			return CanUpdate(todo_id);
		}

		public static bool CanDelete(int todo_id, int task_id)
		{
			return CanUpdate(todo_id, task_id);
		}

		public static bool CanDeleteIncidentTodo(int todo_id, int incident_id)
		{
			return CanUpdateIncidentTodo(todo_id, incident_id);
		}

		public static bool CanDeleteDocumentTodo(int todo_id, int document_id)
		{
			return CanUpdateDocumentTodo(todo_id, document_id);
		}
		#endregion

		#region CanRead(int todo_id)
		public static bool CanRead(int todo_id)
		{
			bool retval = false;

			int incidentId = -1;
			int documentId = -1;
			int taskId = -1;
			int projectId = -1;
			bool isCompleted = false;
			using (IDataReader reader = ToDo.GetToDo(todo_id, false))
			{
				if (reader.Read())
				{
					if (reader["IncidentId"] != DBNull.Value)
						incidentId = (int)reader["IncidentId"];
					if (reader["DocumentId"] != DBNull.Value)
						documentId = (int)reader["DocumentId"];
					if (reader["TaskId"] != DBNull.Value)
						taskId = (int)reader["TaskId"];
					if (reader["ProjectId"] != DBNull.Value)
						projectId = (int)reader["ProjectId"];
					isCompleted = (bool)reader["IsCompleted"];
				}
			}

			if (!Security.CurrentUser.IsExternal)
			{
				// Check by AlertService
				retval = Security.CurrentUser.IsAlertService;

				if (!retval /*&& projectId > 0 */)
				{
					retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)
						|| Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager);
				}

				// Check by ToDo
				if (!retval)
				{
					ToDoSecurity ts = GetSecurity(todo_id);
					retval = ts.IsManager || ts.IsResource || ts.IsCreator;
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
					if (DBToDo.GetSharingLevel(Security.CurrentUser.UserID, todo_id) >= 0)
						retval = true;
				}

				// Check by Incident
				if (!retval && incidentId > 0)
				{
					Incident.IncidentSecurity incS = Incident.GetSecurity(incidentId);
					retval = incS.IsManager;
				}

				// Check by Document
				if (!retval && documentId > 0)
				{
					Document.DocumentSecurity docS = Document.GetSecurity(documentId);
					retval = docS.IsManager;
				}

				// Check by Task
				if (!retval && taskId > 0)
				{
					Task.TaskSecurity taskS = Task.GetSecurity(taskId);
					retval = taskS.IsManager;
				}
			}
			else		// External User
			{
				if (!isCompleted)
				{
					ToDoSecurity ts = GetSecurity(todo_id);
					retval = ts.IsResource;
				}
			}
			return retval;
		}
		#endregion

		#region CanRead(int todo_id, int user_id)
		public static bool CanRead(int todo_id, int user_id)
		{
			bool retval = false;

			int incidentId = -1;
			int documentId = -1;
			int taskId = -1;
			int projectId = -1;
			bool isCompleted = false;
			using (IDataReader reader = ToDo.GetToDo(todo_id, false))
			{
				if (reader.Read())
				{
					if (reader["IncidentId"] != DBNull.Value)
						incidentId = (int)reader["IncidentId"];
					if (reader["DocumentId"] != DBNull.Value)
						documentId = (int)reader["DocumentId"];
					if (reader["TaskId"] != DBNull.Value)
						taskId = (int)reader["TaskId"];
					if (reader["ProjectId"] != DBNull.Value)
						projectId = (int)reader["ProjectId"];
					isCompleted = (bool)reader["IsCompleted"];
				}
			}

			if (!User.IsExternal(user_id))
			{
				if (projectId > 0)
				{
					ArrayList secure_groups = User.GetListSecureGroupAll(user_id);
					retval = secure_groups.Contains((int)InternalSecureGroups.PowerProjectManager)
						|| secure_groups.Contains((int)InternalSecureGroups.ExecutiveManager);
				}

				// Check by ToDo
				if (!retval)
				{
					ToDoSecurity ts = GetSecurity(todo_id, user_id);
					retval = ts.IsManager || ts.IsResource || ts.IsCreator;
				}

				// Check by Project
				if (!retval && projectId > 0)
				{
					Project.ProjectSecurity ps = Project.GetSecurity(projectId, user_id);
					retval = ps.IsManager || ps.IsTeamMember || ps.IsExecutiveManager || ps.IsSponsor || ps.IsStakeHolder;
				}

				// Check by Incident
				if (!retval && incidentId > 0)
				{
					Incident.IncidentSecurity incS = Incident.GetSecurity(incidentId, user_id);
					retval = incS.IsManager;
				}

				// Check by Document
				if (!retval && documentId > 0)
				{
					Document.DocumentSecurity docS = Document.GetSecurity(documentId, user_id);
					retval = docS.IsManager;
				}

				// Check by Task
				if (!retval && taskId > 0)
				{
					Task.TaskSecurity taskS = Task.GetSecurity(taskId, user_id);
					retval = taskS.IsManager;
				}
			}
			else		// External User
			{
				if (!isCompleted)
				{
					ToDoSecurity ts = GetSecurity(todo_id, user_id);
					retval = ts.IsResource;
				}
			}
			return retval;
		}
		#endregion


		#region CanViewFinances
		public static bool CanViewFinances(int todo_id)
		{
			if (DBToDo.GetProject(todo_id) < 0)
				return false;

			bool retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)
				|| Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager)
				|| GetSecurity(todo_id).IsManager;

			if (!retval)	// Проверим родительский объект
			{
				int task_id = -1;
				int doc_id = -1;
				int issue_id = -1;
				using (IDataReader reader = GetToDo(todo_id, false))
				{
					if (reader.Read())
					{
						if (reader["TaskId"] != DBNull.Value)
							task_id = (int)reader["TaskId"];
						if (reader["DocumentId"] != DBNull.Value)
							doc_id = (int)reader["DocumentId"];
						if (reader["IncidentId"] != DBNull.Value)
							issue_id = (int)reader["IncidentId"];
					}
				}

				if (task_id > 0)
					retval = Task.GetSecurity(task_id).IsManager;
				else if (doc_id > 0)
					retval = Document.GetSecurity(doc_id).IsManager;
				else if (issue_id > 0)
					retval = Incident.GetSecurity(issue_id).IsManager;
			}

			return retval;
		}
		#endregion

		#region GetSecurity
		public static ToDoSecurity GetSecurity(int todo_id)
		{
			return GetSecurity(todo_id, Security.CurrentUser.UserID);
		}

		public static ToDoSecurity GetSecurity(int todo_id, int user_id)
		{
			return new ToDoSecurity(todo_id, user_id);
		}
		#endregion

		#region Create
		public static int Create(
			int project_id,
			int manager_id,
			string title,
			string description,
			DateTime start_date,
			DateTime finish_date,
			int priority_id,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			int task_time,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			ArrayList resourceList,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			return Create(project_id, manager_id, title, description, start_date, finish_date, priority_id, activation_type,
				completion_type, must_be_confirmed, task_time, categories, FileName, _inputStream, resourceList, contactUid, orgUid, -1, -1);
		}

		public static int Create(
			int project_id,
			int manager_id,
			string title,
			string description,
			DateTime start_date,
			DateTime finish_date,
			int priority_id,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			int task_time,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			ArrayList resourceList,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid,
			int predId,
			int succId)
		{
			// Check for security and wrong parameters
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (!CanCreate(project_id))
				throw new AccessDeniedException();

			object oStartDate = null;
			object oFinishDate = null;
			DateTime MinValue1 = DateTime.MinValue.AddDays(1);
			int TimeZoneId = Security.CurrentUser.TimeZoneId;

			if (start_date > MinValue1 && finish_date > MinValue1)
			{
				if (start_date > finish_date)
					throw new Exception("wrong dates");
			}

			//			if (project_id > 0 && start_date > MinValue1)
			//			{
			//				DateTime MinStartDate = Project.GetStartDate(project_id);
			//				if (start_date < MinStartDate)
			//				{
			//					throw new Exception("Wrong Start Date");
			//				}
			//			}

			// Some variables
			if (start_date > MinValue1)
				oStartDate = DBCommon.GetUTCDate(TimeZoneId, start_date);
			if (finish_date > MinValue1)
				oFinishDate = DBCommon.GetUTCDate(TimeZoneId, finish_date);

			if (task_time < 0)
			{
				task_time = int.Parse(PortalConfig.ToDoDefaultValueTaskTimeField);
				if (oStartDate != null && oFinishDate != null)
				{
					if (project_id > 0)
					{
						int CalendarId = DBProject.GetCalendar(project_id);
						task_time = DBCalendar.GetDurationByFinishDate(CalendarId, (DateTime)oStartDate, (DateTime)oFinishDate);
					}
					else
					{
						TimeSpan ts = ((DateTime)oFinishDate).Subtract((DateTime)oStartDate);
						task_time = (int)ts.TotalMinutes;
					}
				}
			}

			object oProjectId = null;
			if (project_id > 0)
				oProjectId = project_id;

			DateTime creation_date = DateTime.UtcNow;
			int UserId = Security.CurrentUser.UserID;
			int ToDoId = -1;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				ToDoId = DBToDo.Create(oProjectId, UserId, manager_id,
					title, description, creation_date, oStartDate, oFinishDate, priority_id, activation_type, completion_type, must_be_confirmed, task_time,
					contactUid == PrimaryKeyId.Empty ? null : (object)contactUid,
					orgUid == PrimaryKeyId.Empty ? null : (object)orgUid);

				// OZ: User Role Addon
				UserRoleHelper.AddTodoCreatorRole(ToDoId, UserId);
				UserRoleHelper.AddTodoManagerRole(ToDoId, manager_id);
				if (project_id > 0)
				{
					ForeignContainerKey.Add(UserRoleHelper.CreateTodoContainerKey(ToDoId), UserRoleHelper.CreateProjectContainerKey(project_id));
				}
				//

				// Categories
				foreach (int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)TODO_TYPE, ToDoId, CategoryId);
				}

				// O.R: Scheduling
				if (oStartDate != null)
					Schedule.UpdateDateTypeValue(DateTypes.Todo_StartDate, ToDoId, (DateTime)oStartDate);
				if (oFinishDate != null)
					Schedule.UpdateDateTypeValue(DateTypes.Todo_FinishDate, ToDoId, (DateTime)oFinishDate);

				SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Created, ToDoId);
				if (project_id > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_TodoList_TodoAdded, project_id, ToDoId);

				if (resourceList != null)
				{
					foreach (int userId in resourceList)
					{
						DBToDo.AddResource(ToDoId, userId, false);

						// OZ: User Role Addon
						UserRoleHelper.AddTodoResourceRole(ToDoId, userId);
						//

						if (User.IsExternal(userId))
							DBCommon.AddGate((int)TODO_TYPE, ToDoId, userId);

						SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_ResourceList_AssignmentAdded, ToDoId, userId);
					}
				}

				if (FileName != null)
				{
					string ContainerName = "FileLibrary";
					string ContainerKey = UserRoleHelper.CreateTodoContainerKey(ToDoId);

					BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
					FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
					fs.SaveFile(fs.Root.Id, FileName, _inputStream);
					//int assetId = Asset.Create(ToDoId,ObjectTypes.ToDo, FileName,"", FileName, _inputStream, false);
					//SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_FileList_FileAdded, ToDoId, assetId);
				}

				// O.R: Recalculating project TaskTime
				if (project_id > 0 && task_time > 0)
					TimeTracking.RecalculateProjectTaskTime(project_id);

				if (predId > 0)
					DBToDo.CreateToDoLink(predId, ToDoId);
				if (succId > 0)
					DBToDo.CreateToDoLink(ToDoId, succId);

				RecalculateState(ToDoId);

				tran.Commit();
			}

			return ToDoId;
		}

		public static int CreateWithLink(
			int project_id,
			int manager_id,
			string title,
			string description,
			DateTime start_date,
			DateTime finish_date,
			int priority_id,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			int task_time,
			ArrayList categories,
			string link_title,
			string link_url,
			ArrayList resourceList,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			// Check for security and wrong parameters
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (!CanCreate(project_id))
				throw new AccessDeniedException();

			object oStartDate = null;
			object oFinishDate = null;
			DateTime MinValue1 = DateTime.MinValue.AddDays(1);
			int TimeZoneId = Security.CurrentUser.TimeZoneId;

			if (start_date > MinValue1 && finish_date > MinValue1)
			{
				if (start_date > finish_date)
					throw new Exception("wrong dates");
			}

			// Some variables
			if (start_date > MinValue1)
				oStartDate = DBCommon.GetUTCDate(TimeZoneId, start_date);
			if (finish_date > MinValue1)
				oFinishDate = DBCommon.GetUTCDate(TimeZoneId, finish_date);

			if (task_time < 0)
			{
				task_time = int.Parse(PortalConfig.ToDoDefaultValueTaskTimeField); ;
				if (oStartDate != null && oFinishDate != null)
				{
					if (project_id > 0)
					{
						int CalendarId = DBProject.GetCalendar(project_id);
						task_time = DBCalendar.GetDurationByFinishDate(CalendarId, (DateTime)oStartDate, (DateTime)oFinishDate);
					}
					else
					{
						TimeSpan ts = ((DateTime)oFinishDate).Subtract((DateTime)oStartDate);
						task_time = (int)ts.TotalMinutes;
					}
				}
			}

			object oProjectId = null;
			if (project_id > 0)
				oProjectId = project_id;

			DateTime creation_date = DateTime.UtcNow;
			int UserId = Security.CurrentUser.UserID;
			int ToDoId = -1;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				ToDoId = DBToDo.Create(oProjectId, UserId, manager_id,
					title, description, creation_date, oStartDate, oFinishDate, priority_id,
					activation_type, completion_type, must_be_confirmed, task_time,
					contactUid == PrimaryKeyId.Empty ? null : (object)contactUid,
					orgUid == PrimaryKeyId.Empty ? null : (object)orgUid);

				// OZ: User Role Addon
				UserRoleHelper.AddTodoCreatorRole(ToDoId, UserId);
				UserRoleHelper.AddTodoManagerRole(ToDoId, manager_id);
				if (project_id > 0)
				{
					ForeignContainerKey.Add(UserRoleHelper.CreateTodoContainerKey(ToDoId), UserRoleHelper.CreateProjectContainerKey(project_id));
				}
				//

				// Categories
				foreach (int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)TODO_TYPE, ToDoId, CategoryId);
				}

				// O.R: Scheduling
				if (oStartDate != null)
					Schedule.UpdateDateTypeValue(DateTypes.Todo_StartDate, ToDoId, (DateTime)oStartDate);
				if (oFinishDate != null)
					Schedule.UpdateDateTypeValue(DateTypes.Todo_FinishDate, ToDoId, (DateTime)oFinishDate);

				SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Created, ToDoId);
				if (project_id > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_TodoList_TodoAdded, project_id, ToDoId);

				if (resourceList != null)
				{
					foreach (int userId in resourceList)
					{
						DBToDo.AddResource(ToDoId, userId, false);

						// OZ: User Role Addon
						UserRoleHelper.AddTodoResourceRole(ToDoId, userId);
						//

						bool IsExternal = false;
						using (IDataReader reader = DBUser.GetUserInfo(userId))
						{
							if (reader.Read())
							{
								IsExternal = (bool)reader["IsExternal"];
							}
						}

						if (IsExternal)
							DBCommon.AddGate((int)TODO_TYPE, ToDoId, userId);

						SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_ResourceList_AssignmentAdded, ToDoId, userId);
					}
				}

				// O.R: Recalculating project TaskTime
				if (project_id > 0 && task_time > 0)
					TimeTracking.RecalculateProjectTaskTime(project_id);

				RecalculateState(ToDoId);

				tran.Commit();
			}

			return ToDoId;
		}

		public static int CreateUseUniversalTime(
			int project_id,
			int manager_id,
			string title,
			string description,
			DateTime start_date,
			DateTime finish_date,
			int priority_id,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			int task_time,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			ArrayList resourceList,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			// Check for security and wrong parameters
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (!CanCreate(project_id))
				throw new AccessDeniedException();

			object oStartDate = null;
			object oFinishDate = null;
			DateTime MinValue1 = DateTime.MinValue.AddDays(1);

			if (start_date > MinValue1 && finish_date > MinValue1)
			{
				if (start_date > finish_date)
					throw new Exception("wrong dates");
			}

			// Some variables
			if (start_date > MinValue1)
				oStartDate = start_date;
			if (finish_date > MinValue1)
				oFinishDate = finish_date;

			if (task_time < 0)
			{
				task_time = int.Parse(PortalConfig.ToDoDefaultValueTaskTimeField);
				if (oStartDate != null && oFinishDate != null)
				{
					if (project_id > 0)
					{
						int CalendarId = DBProject.GetCalendar(project_id);
						task_time = DBCalendar.GetDurationByFinishDate(CalendarId, (DateTime)oStartDate, (DateTime)oFinishDate);
					}
					else
					{
						TimeSpan ts = ((DateTime)oFinishDate).Subtract((DateTime)oStartDate);
						task_time = (int)ts.TotalMinutes;
					}
				}
			}

			object oProjectId = null;
			if (project_id > 0)
				oProjectId = project_id;

			DateTime creation_date = DateTime.UtcNow;
			int UserId = Security.CurrentUser.UserID;
			int ToDoId = -1;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				ToDoId = DBToDo.Create(oProjectId, UserId, manager_id,
					title, description, creation_date, oStartDate, oFinishDate, priority_id,
					activation_type, completion_type, must_be_confirmed, task_time,
					contactUid == PrimaryKeyId.Empty ? null : (object)contactUid,
					orgUid == PrimaryKeyId.Empty ? null : (object)orgUid);

				// OZ: User Role Addon
				UserRoleHelper.AddTodoCreatorRole(ToDoId, UserId);
				UserRoleHelper.AddTodoManagerRole(ToDoId, manager_id);
				if (project_id > 0)
				{
					ForeignContainerKey.Add(UserRoleHelper.CreateTodoContainerKey(ToDoId), UserRoleHelper.CreateProjectContainerKey(project_id));
				}
				//

				// Categories
				foreach (int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)TODO_TYPE, ToDoId, CategoryId);
				}

				// O.R: Scheduling
				if (oStartDate != null)
					Schedule.UpdateDateTypeValue(DateTypes.Todo_StartDate, ToDoId, (DateTime)oStartDate);
				if (oFinishDate != null)
					Schedule.UpdateDateTypeValue(DateTypes.Todo_FinishDate, ToDoId, (DateTime)oFinishDate);

				SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Created, ToDoId);
				if (project_id > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_TodoList_TodoAdded, project_id, ToDoId);

				if (resourceList != null)
				{
					ArrayList resAdd = new ArrayList();
					ArrayList resReq = new ArrayList();

					foreach (int userId in resourceList)
					{
						int RealPrincipalId = userId;

						bool bMustBeConfirmed = false;

						if (RealPrincipalId < 0)
						{
							RealPrincipalId *= -1;
							bMustBeConfirmed = true;
						}

						bool IsExternal = false;
						using (IDataReader reader = DBUser.GetUserInfo(RealPrincipalId))
						{
							if (reader.Read())
							{
								IsExternal = (bool)reader["IsExternal"];
							}
						}

						if (bMustBeConfirmed)
						{
							resReq.Add(RealPrincipalId);
							SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_ResourceList_RequestAdded, ToDoId, RealPrincipalId);
						}
						else
						{
							resAdd.Add(RealPrincipalId);
							SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_ResourceList_AssignmentAdded, ToDoId, RealPrincipalId);
						}

						DBToDo.AddResource(ToDoId, RealPrincipalId, bMustBeConfirmed);

						// OZ: User Role Addon
						UserRoleHelper.AddTodoResourceRole(ToDoId, RealPrincipalId);
						//

						if (IsExternal)
							DBCommon.AddGate((int)TODO_TYPE, ToDoId, RealPrincipalId);
					}
				}

				if (FileName != null)
				{
					string ContainerName = "FileLibrary";
					string ContainerKey = UserRoleHelper.CreateTodoContainerKey(ToDoId);

					BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
					FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
					fs.SaveFile(fs.Root.Id, FileName, _inputStream);
				}

				// O.R: Recalculating project TaskTime
				if (project_id > 0 && task_time > 0)
					TimeTracking.RecalculateProjectTaskTime(project_id);

				RecalculateState(ToDoId);

				tran.Commit();
			}

			return ToDoId;
		}


		public static int Create(
			int project_id,
			int manager_id,
			string title,
			string description,
			DateTime start_date,
			DateTime finish_date,
			int priority_id,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			int task_time,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			return ToDo.Create(project_id, manager_id, title, description, start_date, finish_date,
				priority_id, activation_type, completion_type, must_be_confirmed, task_time, categories, FileName, _inputStream, null,
				contactUid, orgUid, -1, -1);
		}

		public static int CreateFromWizard(
			int project_id,
			int manager_id,
			string title,
			string description,
			DateTime start_date,
			DateTime finish_date,
			int priority_id,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			int task_time,
			DataTable resourceList,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			// Check for security and wrong parameters
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (!CanCreate(project_id))
				throw new AccessDeniedException();

			object oStartDate = null;
			object oFinishDate = null;
			DateTime MinValue1 = DateTime.MinValue.AddDays(1);
			int TimeZoneId = Security.CurrentUser.TimeZoneId;

			if (start_date > MinValue1 && finish_date > MinValue1)
			{
				if (start_date > finish_date)
					throw new Exception("wrong dates");
			}

			//			if (project_id > 0 && start_date > MinValue1)
			//			{
			//				DateTime MinStartDate = Project.GetStartDate(project_id);
			//				if (start_date < MinStartDate)
			//				{
			//					throw new Exception("Wrong Start Date");
			//				}
			//			}

			// Some variables
			if (start_date > MinValue1)
				oStartDate = DBCommon.GetUTCDate(TimeZoneId, start_date);
			if (finish_date > MinValue1)
				oFinishDate = DBCommon.GetUTCDate(TimeZoneId, finish_date);

			if (task_time < 0)
			{
				task_time = int.Parse(PortalConfig.ToDoDefaultValueTaskTimeField);
				if (oStartDate != null && oFinishDate != null)
				{
					if (project_id > 0)
					{
						int CalendarId = DBProject.GetCalendar(project_id);
						task_time = DBCalendar.GetDurationByFinishDate(CalendarId, (DateTime)oStartDate, (DateTime)oFinishDate);
					}
					else
					{
						TimeSpan ts = ((DateTime)oFinishDate).Subtract((DateTime)oStartDate);
						task_time = (int)ts.TotalMinutes;
					}
				}
			}

			object oProjectId = null;
			if (project_id > 0)
				oProjectId = project_id;

			DateTime creation_date = DateTime.UtcNow;
			int UserId = Security.CurrentUser.UserID;
			int ToDoId = -1;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				ToDoId = DBToDo.Create(oProjectId, UserId, manager_id,
					title, description, creation_date, oStartDate, oFinishDate, priority_id, activation_type, completion_type, must_be_confirmed, task_time,
					contactUid == PrimaryKeyId.Empty ? null : (object)contactUid,
					orgUid == PrimaryKeyId.Empty ? null : (object)orgUid);

				// OZ: User Role Addon
				UserRoleHelper.AddTodoCreatorRole(ToDoId, UserId);
				UserRoleHelper.AddTodoManagerRole(ToDoId, manager_id);
				if (project_id > 0)
				{
					ForeignContainerKey.Add(UserRoleHelper.CreateTodoContainerKey(ToDoId), UserRoleHelper.CreateProjectContainerKey(project_id));
				}
				//

				// O.R: Scheduling
				if (oStartDate != null)
					Schedule.UpdateDateTypeValue(DateTypes.Todo_StartDate, ToDoId, (DateTime)oStartDate);
				if (oFinishDate != null)
					Schedule.UpdateDateTypeValue(DateTypes.Todo_FinishDate, ToDoId, (DateTime)oFinishDate);

				SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Created, ToDoId);
				if (project_id > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_TodoList_TodoAdded, project_id, ToDoId);

				// Categories
				ArrayList categories = Common.StringToArrayList(PortalConfig.ToDoDefaultValueGeneralCategoriesField);
				foreach (int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)TODO_TYPE, ToDoId, CategoryId);
				}

				// Add new resources
				ArrayList resAdd = new ArrayList();
				ArrayList resReq = new ArrayList();
				foreach (DataRow dr in resourceList.Rows)
				{
					if (dr.RowState != DataRowState.Deleted)
					{
						int uId = (int)dr["UserId"];
						bool MustBeConfirmed = (bool)dr["MustBeConfirmed"];
						DBToDo.AddResource(ToDoId, uId, MustBeConfirmed);

						// OZ: User Role Addon
						UserRoleHelper.AddTodoResourceRole(ToDoId, uId);
						//

						bool IsExternal = false;
						using (IDataReader reader = DBUser.GetUserInfo(uId))
						{
							if (reader.Read())
							{
								IsExternal = (bool)reader["IsExternal"];
							}
						}

						if (IsExternal)
							DBCommon.AddGate((int)TODO_TYPE, ToDoId, uId);

						if (MustBeConfirmed)
						{
							resReq.Add(uId);
							SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_ResourceList_RequestAdded, ToDoId, uId);
						}
						else
						{
							resAdd.Add(uId);
							SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_ResourceList_AssignmentAdded, ToDoId, uId);
						}
					}
				}

				// O.R: Recalculating project TaskTime
				if (project_id > 0 && task_time > 0)
					TimeTracking.RecalculateProjectTaskTime(project_id);

				RecalculateState(ToDoId);

				tran.Commit();
			}

			return ToDoId;
		}


		public static int Create(
			int incident_id,
			int managerId,
			string title,
			string description,
			DateTime start_date,
			DateTime finish_date,
			int priority_id,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			bool complete_incident,
			int task_time,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			return Create(incident_id, managerId, title, description, start_date, finish_date,
				priority_id, activation_type, completion_type, must_be_confirmed, complete_incident,
				task_time, categories, FileName, _inputStream, null, contactUid, orgUid, -1, -1);
		}

		public static int Create(
			int incident_id,
			int managerId,
			string title,
			string description,
			DateTime start_date,
			DateTime finish_date,
			int priority_id,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			bool complete_incident,
			int task_time,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			ArrayList resourceList,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			return Create(incident_id, managerId, title, description, start_date, finish_date,
				priority_id, activation_type, completion_type, must_be_confirmed, complete_incident,
				task_time, categories, FileName, _inputStream, resourceList, contactUid, orgUid, -1, -1);
		}

		public static int Create(
			int incident_id,
			int managerId,
			string title,
			string description,
			DateTime start_date,
			DateTime finish_date,
			int priority_id,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			bool complete_incident,
			int task_time,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			ArrayList resourceList,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid,
			int predId,
			int succId)
		{
			// Check for security and wrong parameters
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (!Incident.CanAddToDo(incident_id))
				throw new AccessDeniedException();

			int project_id = DBIncident.GetProject(incident_id);

			//			if (!CanCreate(project_id))
			//				throw new AccessDeniedException();

			object oStartDate = null;
			object oFinishDate = null;
			DateTime MinValue1 = DateTime.MinValue.AddDays(1);
			int TimeZoneId = Security.CurrentUser.TimeZoneId;

			if (start_date > MinValue1 && finish_date > MinValue1)
			{
				if (start_date > finish_date)
					throw new Exception("wrong dates");
			}

			//			if (project_id > 0 && start_date > MinValue1)
			//			{
			//				DateTime MinStartDate = Project.GetStartDate(project_id);
			//				if (start_date < MinStartDate)
			//				{
			//					throw new Exception("Wrong Start Date");
			//				}
			//			}

			// Some variables
			if (start_date > MinValue1)
				oStartDate = DBCommon.GetUTCDate(TimeZoneId, start_date);
			if (finish_date > MinValue1)
				oFinishDate = DBCommon.GetUTCDate(TimeZoneId, finish_date);

			if (task_time < 0)
			{
				task_time = int.Parse(PortalConfig.ToDoDefaultValueTaskTimeField);
				if (oStartDate != null && oFinishDate != null)
				{
					if (project_id > 0)
					{
						int CalendarId = DBProject.GetCalendar(project_id);
						task_time = DBCalendar.GetDurationByFinishDate(CalendarId, (DateTime)oStartDate, (DateTime)oFinishDate);
					}
					else
					{
						TimeSpan ts = ((DateTime)oFinishDate).Subtract((DateTime)oStartDate);
						task_time = (int)ts.TotalMinutes;
					}
				}
			}

			object oProjectId = null;
			if (project_id > 0)
				oProjectId = project_id;

			DateTime current_date = DateTime.UtcNow;

			int CurrentIncidentState;
			string IncidentTitle = "";
			using (IDataReader reader = DBIncident.GetIncident(incident_id, TimeZoneId, Security.CurrentUser.LanguageId))
			{
				reader.Read();
				CurrentIncidentState = (int)reader["StateId"];
				IncidentTitle = reader["Title"].ToString();
			}

			int ToDoId = -1;

			int UserId = Security.CurrentUser.UserID;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				if (CurrentIncidentState == (int)ObjectStates.Upcoming)
					Issue2.UpdateStateAndNotifyController(incident_id, (int)ObjectStates.Active, -1);

				ToDoId = DBToDo.Create(oProjectId, UserId, managerId,
					title, description, current_date, oStartDate, oFinishDate, priority_id,
					activation_type, completion_type, must_be_confirmed, task_time,
					contactUid == PrimaryKeyId.Empty ? null : (object)contactUid,
					orgUid == PrimaryKeyId.Empty ? null : (object)orgUid);

				// OZ: User Role Addon
				UserRoleHelper.AddTodoCreatorRole(ToDoId, UserId);
				UserRoleHelper.AddTodoManagerRole(ToDoId, managerId);
				if (project_id > 0)
				{
					ForeignContainerKey.Add(UserRoleHelper.CreateTodoContainerKey(ToDoId), UserRoleHelper.CreateProjectContainerKey(project_id));
				}
				//

				// O.R: Scheduling
				if (oStartDate != null)
					Schedule.UpdateDateTypeValue(DateTypes.Todo_StartDate, ToDoId, (DateTime)oStartDate);
				if (oFinishDate != null)
					Schedule.UpdateDateTypeValue(DateTypes.Todo_FinishDate, ToDoId, (DateTime)oFinishDate);

				SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Created, ToDoId);

				if (complete_incident)
					DBToDo.AddIncidentToDo(incident_id, ToDoId, (int)ObjectStates.Completed);
				else
					DBToDo.AddIncidentToDo(incident_id, ToDoId, -1);

				SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_TodoList_TodoAdded, incident_id, ToDoId);

				if (FileName != null)
				{
					string ContainerName = "FileLibrary";
					string ContainerKey = UserRoleHelper.CreateTodoContainerKey(ToDoId);

					BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
					FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
					string sFileName = FileName;
					int iRoot = fs.Root.Id;
					int i = 1;
					while (fs.FileExist(sFileName, iRoot))
					{
						string sStart = FileName;
						string sEnd = "";
						if (FileName.LastIndexOf(".") >= 0)
						{
							sEnd = FileName.Substring(FileName.LastIndexOf("."));
							sStart = FileName.Substring(0, FileName.LastIndexOf("."));
						}
						sFileName = sStart + (i++).ToString() + sEnd;
					}
					fs.SaveFile(iRoot, sFileName, _inputStream);

					//int assetId = Asset.Create(ToDoId,ObjectTypes.ToDo, FileName,"", FileName, _inputStream, false);
					//SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_FileList_FileAdded, ToDoId, assetId);
				}

				// Categories
				foreach (int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)TODO_TYPE, ToDoId, CategoryId);
				}

				if (resourceList != null)
				{
					foreach (int userId in resourceList)
					{
						DBToDo.AddResource(ToDoId, userId, false);

						// OZ: User Role Addon
						UserRoleHelper.AddTodoResourceRole(ToDoId, userId);
						//

						bool IsExternal = false;
						using (IDataReader reader = DBUser.GetUserInfo(userId))
						{
							if (reader.Read())
							{
								IsExternal = (bool)reader["IsExternal"];
							}
						}

						if (IsExternal)
							DBCommon.AddGate((int)TODO_TYPE, ToDoId, userId);

						SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_ResourceList_AssignmentAdded, ToDoId, userId);
					}
				}

				// O.R: Recalculating project TaskTime
				if (project_id > 0 && task_time > 0)
					TimeTracking.RecalculateProjectTaskTime(project_id);

				if (predId > 0)
					DBToDo.CreateToDoLink(predId, ToDoId);
				if (succId > 0)
					DBToDo.CreateToDoLink(ToDoId, succId);

				RecalculateState(ToDoId);

				tran.Commit();
			}

			return ToDoId;
		}
		#endregion

		#region Delete
		public static void Delete(int todo_id)
		{
			Delete(todo_id, true);
		}

		internal static void Delete(int todo_id, bool checkAccess)
		{
			int incidentId = -1;
			int documentId = -1;
			int taskId = -1;
			int projectId = -1;
			int stateId = (int)ObjectStates.Completed;
			string title = "";
			string incidentTitle = "";
			string documentTitle = "";
			string taskTitle = "";
			string projectTitle = "";
			using (IDataReader reader = ToDo.GetToDo(todo_id, false))
			{
				if (reader.Read())
				{
					if (reader["IncidentId"] != DBNull.Value)
					{
						incidentId = (int)reader["IncidentId"];
						incidentTitle = reader["IncidentTitle"].ToString();
					}
					if (reader["DocumentId"] != DBNull.Value)
					{
						documentId = (int)reader["DocumentId"];
						documentTitle = reader["DocumentTitle"].ToString();
					}
					if (reader["TaskId"] != DBNull.Value)
					{
						taskId = (int)reader["TaskId"];
						taskTitle = reader["TaskTitle"].ToString();
					}
					if (reader["ProjectId"] != DBNull.Value)
					{
						projectId = (int)reader["ProjectId"];
						projectTitle = reader["ProjectTitle"].ToString();
					}
					title = reader["Title"].ToString();
					stateId = (int)reader["StateId"];
				}
			}

			if (checkAccess)
			{
				if (!((incidentId > 0 && CanDeleteIncidentTodo(todo_id, incidentId))
					|| (documentId > 0 && CanDeleteDocumentTodo(todo_id, documentId))
					|| (taskId > 0 && CanDelete(todo_id, taskId))
					|| CanDelete(todo_id)))
					throw new AccessDeniedException();
			}

			using (DbTransaction tran = DbTransaction.Begin())
			{
				// OZ: File Storage
				BaseIbnContainer container = BaseIbnContainer.Create("FileLibrary", UserRoleHelper.CreateTodoContainerKey(todo_id));
				FileStorage fs = (FileStorage)container.LoadControl("FileStorage");
				fs.DeleteAll();
				//

				// OZ: User Roles
				UserRoleHelper.DeleteTodoRoles(todo_id);

				// Step 2. Transform Timesheet [2004-02-04]
				TimeTracking.ResetObjectId((int)TODO_TYPE, todo_id);

				MetaObject.Delete(todo_id, "TodoEx");

				SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Deleted, todo_id);
				if (incidentId > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_TodoList_TodoDeleted, incidentId, todo_id);
				if (documentId > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_TodoList_TodoDeleted, documentId, todo_id);
				if (taskId > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_TodoList_TodoDeleted, taskId, todo_id);
				if (incidentId < 0 && taskId < 0 && documentId < 0 && projectId > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_TodoList_TodoDeleted, projectId, todo_id);

				// OR: [2007-08-30] Формируем список зависимых ToDo, находящихся в состоянии Upcoming
				ArrayList todoList = new ArrayList();
				if (stateId != (int)ObjectStates.Completed)
				{
					using (IDataReader reader = DBToDo.GetListSuccessors(todo_id))
					{
						while (reader.Read())
						{
							if ((int)reader["StateId"] == (int)ObjectStates.Upcoming)
								todoList.Add((int)reader["ToDoId"]);
						}
					}
				}

				// Step 3. Delete Todo[1/6/2004]
				DBToDo.Delete(todo_id);

				// O.R: Recalculating project TaskTime
				if (projectId > 0)
					TimeTracking.RecalculateProjectTaskTime(projectId);

				// O.R: [2007-08-30] Recalculate state of the dependent ToDo
				foreach (int todoId in todoList)
					RecalculateState(todoId);

				tran.Commit();
			}
		}
		#endregion

		#region GetListCompletionTypes
		/// <summary>
		/// Reader returns fields:
		///  CompletionTypeId, CompletionTypeName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListCompletionTypes()
		{
			return DBToDo.GetListCompletionTypes(Security.CurrentUser.LanguageId);
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

		#region GetListDiscussions
		/// <summary>
		/// Reader returns fields:
		///		DiscussionId, ObjectTypeId, ObjectId, CreatorId, CreationDate, Text, CreatorName
		/// </summary>
		public static IDataReader GetListDiscussions(int todo_id)
		{
			return Common.GetListDiscussions(TODO_TYPE, todo_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListDiscussionsDataTable
		/// <summary>
		/// Reader returns fields:
		///		DiscussionId, ObjectTypeId, ObjectId, CreatorId, CreationDate, Text, CreatorName
		/// </summary>
		public static DataTable GetListDiscussionsDataTable(int todo_id)
		{
			return Common.GetListDiscussionsDataTable(TODO_TYPE, todo_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region AddDiscussion
		public static void AddDiscussion(int todo_id, string text)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			DateTime creation_date = DateTime.UtcNow;
			int UserId = Security.CurrentUser.UserID;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				int CommentId = DBCommon.AddDiscussion((int)TODO_TYPE, todo_id, UserId, creation_date, text);

				SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_CommentList_CommentAdded, todo_id, CommentId);

				tran.Commit();
			}
		}
		#endregion

		#region GetProjectTitle
		public static string GetProjectTitle(int project_id)
		{
			return DBProject.GetTitle(project_id);
		}
		#endregion

		#region GetProjectManager
		public static int GetProjectManager(int project_id)
		{
			return DBProject.GetProjectManager(project_id);
		}
		#endregion

		#region GetToDo
		/// <summary>
		/// Reader returns fields:
		///  ToDoId, ProjectId, ProjectTitle, 
		///  IncidentId, IncidentTitle, CompleteIncident,
		///  DocumentId, DocumentTitle, CompleteDocument, 
		///  CreatorId, ManagerId, CompletedBy,
		///  Title, Description, CreationDate, StartDate, FinishDate, 
		///  ActualFinishDate, PriorityId, PriorityName, PercentCompleted, IsActual, 
		///  CompletionTypeId, IsCompleted, CompletionTypeName, MustBeConfirmed,
		///  ReasonId, TaskId, CompleteTask, TaskTitle, StateId,
		///  ClientName, TotalMinutes, TotalApproved, ProjectCode
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetToDo(int todo_id)
		{
			return GetToDo(todo_id, true);
		}

		public static IDataReader GetToDo(int todo_id, bool checkAccess)
		{
			return GetToDo(todo_id, checkAccess, false);
		}

		private static IDataReader GetToDo(int todo_id, bool checkAccess, bool utc_dates)
		{
			if (checkAccess && !CanRead(todo_id))
				throw new AccessDeniedException();

			if (utc_dates)
				return DBToDo.GetToDoInUTC(todo_id, Security.CurrentUser.LanguageId);
			else
				return DBToDo.GetToDo(todo_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
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
		#endregion

		#region GetListResources
		/// <summary>
		/// Reader returns fields:
		///	 ResourceId, ToDoId, UserId, MustBeConfirmed, ResponsePending, IsConfirmed, 
		///	 PercentCompleted, ActualFinishDate, LastSavedDate, IsGroup, IsExternal, ResourceName
		/// </summary>
		/// <param name="todo_id"></param>
		/// <returns></returns>
		public static IDataReader GetListResources(int todo_id)
		{
			return DBToDo.GetListResources(todo_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListResourcesDataTable
		/// <summary>
		/// DataTable returns fields:
		///	 ResourceId, ToDoId, UserId, MustBeConfirmed, ResponsePending, IsConfirmed, 
		///	 PercentCompleted, ActualFinishDate, LastSavedDate, IsGroup, IsExternal
		/// </summary>
		/// <param name="todo_id"></param>
		/// <returns></returns>
		public static DataTable GetListResourcesDataTable(int todo_id)
		{
			return DBToDo.GetListResourcesDataTable(todo_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetResourceInfo
		/// <summary>
		/// Reader returns fields:
		///	 MustBeConfirmed, ResponsePending, IsConfirmed, PercentCompleted
		/// </summary>
		/// <param name="todo_id"></param>
		/// <returns></returns>
		public static IDataReader GetResourceInfo(int todo_id)
		{
			return DBToDo.GetResourceByUser(todo_id, Security.CurrentUser.UserID);
		}
		#endregion

		#region UpdateResourcePercent
		public static void UpdateResourcePercent(int todo_id, int percent_completed, int minutes, DateTime Dt)
		{
			UpdateResourcePercent(todo_id, Security.CurrentUser.UserID, percent_completed, minutes, Dt, true);
		}

		public static void UpdateResourcePercent(int todo_id, int percent_completed)
		{
			UpdateResourcePercent(todo_id, Security.CurrentUser.UserID, percent_completed, 0, DateTime.Now, true);
		}

		public static void UpdateResourcePercent(int todo_id, int userId, int percent_completed)
		{
			UpdateResourcePercent(todo_id, userId, percent_completed, 0, DateTime.Now, true);
		}

		public static void UpdateResourcePercent(int todo_id, int userId, int percent_completed, int minutes, DateTime Dt, bool summarize)
		{
			if (!CanRead(todo_id))
				throw new AccessDeniedException();

			Dt = Dt.Date;
			bool IsManagerConfirmed;
			int IncidentId = -1;
			int DocumentId = -1;
			int ProjectId = -1;
			int previousPercent = -1;
			bool IsCompleted = false;
			int TaskId = -1;
			bool CompleteTask = false;
			bool CompleteDocument = false;
			bool CompleteIncident = false;
			int oldPercentCompleted = 0;
			string title = "";
			using (IDataReader reader = DBToDo.GetToDo(todo_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				reader.Read();
				IsManagerConfirmed = (bool)reader["MustBeConfirmed"];
				if (reader["IncidentId"] != DBNull.Value)
					IncidentId = (int)reader["IncidentId"];
				if (reader["DocumentId"] != DBNull.Value)
					DocumentId = (int)reader["DocumentId"];
				if (reader["ProjectId"] != DBNull.Value)
					ProjectId = (int)reader["ProjectId"];
				IsCompleted = (bool)reader["IsCompleted"];
				if (reader["TaskId"] != DBNull.Value)
					TaskId = (int)reader["TaskId"];
				if (reader["CompleteIncident"] != DBNull.Value)
					CompleteIncident = (bool)reader["CompleteIncident"];
				if (reader["CompleteTask"] != DBNull.Value)
					CompleteTask = (bool)reader["CompleteTask"];
				if (reader["CompleteDocument"] != DBNull.Value)
					CompleteDocument = (bool)reader["CompleteDocument"];
				oldPercentCompleted = (int)reader["PercentCompleted"];
				title = reader["Title"].ToString();
			}

			if (IsCompleted)
				throw new AccessDeniedException();

			// Get current percent completed
			using (IDataReader reader = DBToDo.GetResourceByUser(todo_id, userId))
			{
				if (reader.Read())
					previousPercent = (int)reader["PercentCompleted"];
			}

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBToDo.UpdateResourcePercent(todo_id, userId, percent_completed, DateTime.UtcNow);

				// O.R. [2009-05-15]
				if (percent_completed == 100)
					DBCalendar.DeleteStickedObject((int)TODO_TYPE, todo_id, userId);

				int OverallPercent = RecalculateOverallPercent(todo_id);
				if (oldPercentCompleted != OverallPercent)
				{
					DBToDo.UpdatePercent(todo_id, OverallPercent);
					SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_Percent, todo_id);
				}

				if (OverallPercent == 100)
				{
					// O.R. [2009-02-12]
					DBCalendar.DeleteStickedObjectForAllUsers((int)TODO_TYPE, todo_id);

					if (!IsManagerConfirmed)
					{
						DBToDo.UpdateCompletion(todo_id, true, (int)CompletionReason.CompletedManually);

						if (IncidentId > 0 && CompleteIncident)
							CompleteIncidentIfNeed(IncidentId);

						if (TaskId > 0 && CompleteTask)
							UpdateTaskCompletion(TaskId);

						if (DocumentId > 0 && CompleteDocument)
							UpdateDocumentCompletion(DocumentId);

						RecalculateState(todo_id);
					}
				}

				if (!summarize || minutes > 0)
					Mediachase.IbnNext.TimeTracking.TimeTrackingManager.AddHoursForEntryByObject(todo_id, (int)TODO_TYPE, title, ProjectId, userId, TimeTracking.GetWeekStart(Dt), TimeTracking.GetDayNum(Dt), minutes, summarize);

				tran.Commit();
			}
		}
		#endregion

		#region UpdatePercent
		public static void UpdatePercent(int todo_id, int percent_completed, int minutes, DateTime Dt)
		{
			UpdatePercent(todo_id, percent_completed, minutes, Dt, true);
		}

		public static void UpdatePercent(int todo_id, int percent_completed)
		{
			UpdatePercent(todo_id, percent_completed, 0, DateTime.Now, true);
		}

		public static void UpdatePercent(int todo_id, int percent_completed, int minutes, DateTime Dt, bool summarize)
		{
			if (!CanRead(todo_id))
				throw new AccessDeniedException();

			bool IsManagerConfirmed;
			int IncidentId = -1;
			int DocumentId = -1;
			int ProjectId = -1;
			bool IsCompleted = false;
			int TaskId = -1;
			bool CompleteIncident = false;
			bool CompleteTask = false;
			bool CompleteDocument = false;
			int oldPercentCompleted = 0;
			string title = "";
			using (IDataReader reader = DBToDo.GetToDo(todo_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				reader.Read();
				IsManagerConfirmed = (bool)reader["MustBeConfirmed"];
				if (reader["IncidentId"] != DBNull.Value)
					IncidentId = (int)reader["IncidentId"];
				if (reader["DocumentId"] != DBNull.Value)
					DocumentId = (int)reader["DocumentId"];
				if (reader["ProjectId"] != DBNull.Value)
					ProjectId = (int)reader["ProjectId"];
				IsCompleted = (bool)reader["IsCompleted"];
				if (reader["TaskId"] != DBNull.Value)
					TaskId = (int)reader["TaskId"];
				if (reader["CompleteIncident"] != DBNull.Value)
					CompleteIncident = (bool)reader["CompleteIncident"];
				if (reader["CompleteTask"] != DBNull.Value)
					CompleteTask = (bool)reader["CompleteTask"];
				if (reader["CompleteDocument"] != DBNull.Value)
					CompleteDocument = (bool)reader["CompleteDocument"];
				oldPercentCompleted = (int)reader["PercentCompleted"];
				title = reader["Title"].ToString();
			}

			if (IsCompleted)
				throw new AccessDeniedException();

			int UserId = Security.CurrentUser.UserID;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				if (oldPercentCompleted != percent_completed)
				{
					DBToDo.UpdatePercent(todo_id, percent_completed);
					SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_Percent, todo_id);
				}


				if (!summarize || minutes > 0)
					Mediachase.IbnNext.TimeTracking.TimeTrackingManager.AddHoursForEntryByObject(todo_id, (int)TODO_TYPE, title, ProjectId, UserId, TimeTracking.GetWeekStart(Dt), TimeTracking.GetDayNum(Dt), minutes, summarize);

				if (percent_completed == 100)
				{
					// O.R. [2009-02-12]
					DBCalendar.DeleteStickedObjectForAllUsers((int)TODO_TYPE, todo_id);

					if (!IsManagerConfirmed)
					{
						DBToDo.UpdateCompletion(todo_id, true, (int)CompletionReason.CompletedManually);

						if (IncidentId > 0 && CompleteIncident)
							CompleteIncidentIfNeed(IncidentId);

						if (TaskId > 0 && CompleteTask)
							UpdateTaskCompletion(TaskId);

						if (DocumentId > 0 && CompleteDocument)
							UpdateDocumentCompletion(DocumentId);

						RecalculateState(todo_id);
					}
				}

				tran.Commit();
			}
		}
		#endregion

		#region RecalculateOverallPercent
		internal static int RecalculateOverallPercent(int todo_id)	// Completion Type = All
		{
			int ResourceCount = 0;
			int TotalPercent = 0;

			using (IDataReader reader = DBToDo.GetListResources(todo_id, Security.CurrentUser.TimeZoneId))
			{
				while (reader.Read())
				{
					int PercentCompleted = (int)reader["PercentCompleted"];
					bool MustBeConfirmed = (bool)reader["MustBeConfirmed"];
					bool IsResponsePending = (bool)reader["ResponsePending"];
					bool IsConfirmed = (bool)reader["IsConfirmed"];

					// Don't calculate deny users
					if (!(MustBeConfirmed && !IsResponsePending && !IsConfirmed))
					{
						ResourceCount++;
						TotalPercent += PercentCompleted;
					}
				}
			}

			int OverallPercent = 0;
			if (ResourceCount > 0)
				OverallPercent = (int)(TotalPercent / ResourceCount);
			return OverallPercent;
		}
		#endregion

		#region CompleteToDo
		public static void CompleteToDo(int todo_id)
		{
			CompleteToDo(todo_id, true);
		}

		public static void CompleteToDo(int todo_id, bool manually)
		{
			if (manually && !CanUpdate(todo_id))
				throw new AccessDeniedException();

			int IncidentId = -1;
			int DocumentId = -1;
			int CompletionTypeId;
			int TaskId = -1;
			bool CompleteIncident = false;
			bool CompleteTask = false;
			bool CompleteDocument = false;
			string title = "";
			using (IDataReader reader = DBToDo.GetToDo(todo_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				reader.Read();
				if (reader["IncidentId"] != DBNull.Value)
					IncidentId = (int)reader["IncidentId"];
				if (reader["DocumentId"] != DBNull.Value)
					DocumentId = (int)reader["DocumentId"];
				CompletionTypeId = (int)reader["CompletionTypeId"];
				if (reader["TaskId"] != DBNull.Value)
					TaskId = (int)reader["TaskId"];
				if (reader["CompleteIncident"] != DBNull.Value)
					CompleteIncident = (bool)reader["CompleteIncident"];
				if (reader["CompleteTask"] != DBNull.Value)
					CompleteTask = (bool)reader["CompleteTask"];
				if (reader["CompleteDocument"] != DBNull.Value)
					CompleteDocument = (bool)reader["CompleteDocument"];
				title = reader["Title"].ToString();
			}

			int UserId = Security.CurrentUser.UserID;
			DateTime dt = DateTime.UtcNow;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				// O.R. [2009-02-12]
				DBCalendar.DeleteStickedObjectForAllUsers((int)TODO_TYPE, todo_id);

				if (CompletionTypeId == (int)CompletionType.All)
					DBToDo.UpdateResourcePercent(todo_id, UserId, 100, dt);

				DBToDo.UpdatePercent(todo_id, 100);

				if (manually)
					DBToDo.UpdateCompletion(todo_id, true, (int)CompletionReason.CompletedManually);
				else
					DBToDo.UpdateCompletion(todo_id, true, (int)CompletionReason.CompletedAutomatically);

				if (IncidentId > 0 && CompleteIncident)
					CompleteIncidentIfNeed(IncidentId);

				if (TaskId > 0 && CompleteTask)
					UpdateTaskCompletion(TaskId);

				if (DocumentId > 0 && CompleteDocument)
					UpdateDocumentCompletion(DocumentId);

				RecalculateState(todo_id);

				tran.Commit();
			}
		}
		#endregion

		#region UncompleteToDo
		public static void UncompleteToDo(int todo_id)
		{
			UncompleteToDo(todo_id, true);
		}
		public static void UncompleteToDo(int todo_id, bool manually)
		{
			if (manually && !CanUpdate(todo_id))
				throw new AccessDeniedException();

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBToDo.UpdateCompletion(todo_id, false, (int)CompletionReason.NotCompleted);

				int OverallPercent = RecalculateOverallPercent(todo_id);
				DBToDo.UpdatePercent(todo_id, OverallPercent);

				RecalculateState(todo_id);

				tran.Commit();
			}
		}
		#endregion

		#region SuspendToDo
		public static void SuspendToDo(int todo_id)
		{
			SuspendToDo(todo_id, true);
		}

		public static void SuspendToDo(int todo_id, bool manually)
		{
			if (manually && !CanUpdate(todo_id))
				throw new AccessDeniedException();

			int UserId = Security.CurrentUser.UserID;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				// O.R. [2009-02-12]
				DBCalendar.DeleteStickedObjectForAllUsers((int)TODO_TYPE, todo_id);

				if (manually)
					DBToDo.UpdateCompletion(todo_id, true, (int)CompletionReason.SuspendedManually);
				else
					DBToDo.UpdateCompletion(todo_id, true, (int)CompletionReason.SuspendedAutomatically);

				RecalculateState(todo_id);

				tran.Commit();
			}
		}
		#endregion

		#region ResumeToDo
		public static void ResumeToDo(int todo_id)
		{
			ResumeToDo(todo_id, true);
		}

		public static void ResumeToDo(int todo_id, bool manually)
		{
			if (manually && !CanUpdate(todo_id))
				throw new AccessDeniedException();

			int UserId = Security.CurrentUser.UserID;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBToDo.UpdateCompletion(todo_id, false, (int)CompletionReason.NotCompleted);

				RecalculateState(todo_id);

				tran.Commit();
			}
		}
		#endregion

		#region GetTrackingInfo
		public static Tracking GetTrackingInfo(int todo_id)
		{
			int ToDo_OverallPercent;
			bool ToDo_IsCompleted;
			int ToDo_ReasonId;
			int ToDo_CompletionTypeId;
			int ToDo_ActivationTypeId;
			int ToDo_StateId;
			int Task_Id = -1;
			int Doc_Id = -1;
			int Issue_Id = -1;

			using (IDataReader reader = DBToDo.GetToDo(todo_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				reader.Read();
				ToDo_OverallPercent = (int)reader["PercentCompleted"];
				ToDo_IsCompleted = (bool)reader["IsCompleted"];
				ToDo_ReasonId = (int)reader["ReasonId"];
				ToDo_CompletionTypeId = (int)reader["CompletionTypeId"];
				ToDo_ActivationTypeId = (int)reader["ActivationTypeId"];
				ToDo_StateId = (int)reader["StateId"];

				if (reader["TaskId"] != DBNull.Value)
					Task_Id = (int)reader["TaskId"];
				if (reader["DocumentId"] != DBNull.Value)
					Doc_Id = (int)reader["DocumentId"];
				if (reader["IncidentId"] != DBNull.Value)
					Issue_Id = (int)reader["IncidentId"];
			}

			ToDoSecurity ts = GetSecurity(todo_id);
			bool IsResource = ts.IsResource;
			bool IsManager = ts.IsManager || ts.IsCreator || Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager);
			if (Task_Id > 0 && !IsManager)
				IsManager = Task.GetSecurity(Task_Id).IsManager;
			if (Doc_Id > 0 && !IsManager)
				IsManager = Document.GetSecurity(Doc_Id).IsManager;
			if (Issue_Id > 0 && !IsManager)
				IsManager = Incident.GetSecurity(Issue_Id).IsManager;

			bool Resource_MustBeConfirmed = false;
			bool Resource_IsResponsePending = false;
			bool Resource_IsConfirmed = true;

			if (IsResource)
			{
				using (IDataReader reader = DBToDo.GetResourceByUser(todo_id, Security.CurrentUser.UserID))
				{
					if (reader.Read())
					{
						Resource_MustBeConfirmed = (bool)reader["MustBeConfirmed"];
						Resource_IsResponsePending = (bool)reader["ResponsePending"];
						Resource_IsConfirmed = (bool)reader["IsConfirmed"];
					}
				}
			}

			// Accept/Deny
			bool ShowAcceptDeny = IsResource
				&& Resource_MustBeConfirmed
				&& Resource_IsResponsePending
				&& !ToDo_IsCompleted;

			// Personal Status with TimeTracking
			bool ShowPersonalStatus = Configuration.TimeTrackingModule && IsResource
				&& (!Resource_MustBeConfirmed || (Resource_MustBeConfirmed && !Resource_IsResponsePending && Resource_IsConfirmed))
				&& ToDo_CompletionTypeId == (int)CompletionType.All
				&& !ToDo_IsCompleted;

			// Personal Status only
			bool ShowPersonalStatusOnly = !Configuration.TimeTrackingModule && IsResource
				&& (!Resource_MustBeConfirmed || (Resource_MustBeConfirmed && !Resource_IsResponsePending && Resource_IsConfirmed))
				&& ToDo_CompletionTypeId == (int)CompletionType.All
				&& !ToDo_IsCompleted;

			// Overall Status with TimeTracking
			bool ShowOverallStatus = Configuration.TimeTrackingModule && (IsResource || IsManager)
				&& (!Resource_MustBeConfirmed || (Resource_MustBeConfirmed && !Resource_IsResponsePending && Resource_IsConfirmed))
				&& ToDo_CompletionTypeId == (int)CompletionType.Any
				&& !ToDo_IsCompleted;

			// Overall Status only
			bool ShowOverallStatusOnly = !Configuration.TimeTrackingModule && (IsResource || IsManager)
				&& (!Resource_MustBeConfirmed || (Resource_MustBeConfirmed && !Resource_IsResponsePending && Resource_IsConfirmed))
				&& ToDo_CompletionTypeId == (int)CompletionType.Any
				&& !ToDo_IsCompleted;

			// Complete
			bool ShowComplete = IsManager
				&& (ToDo_StateId == (int)ObjectStates.Upcoming
						|| ToDo_StateId == (int)ObjectStates.Active
						|| ToDo_StateId == (int)ObjectStates.Overdue
					);

			// Suspend
			bool ShowSuspend = IsManager
				&& (ToDo_StateId == (int)ObjectStates.Active
						|| ToDo_StateId == (int)ObjectStates.Overdue);

			// Uncomplete
			bool ShowUncomplete = IsManager && ToDo_StateId == (int)ObjectStates.Completed;

			// Resume
			bool ShowResume = IsManager && ToDo_StateId == (int)ObjectStates.Suspended;

			// Activate
			bool ShowActivate = IsManager && ToDo_StateId == (int)ObjectStates.Upcoming;

			// Time Tracking (without percent)
			bool ShowTimeTracking = Configuration.TimeTrackingModule && IsResource
				&& (!Resource_MustBeConfirmed || (Resource_MustBeConfirmed && !Resource_IsResponsePending && Resource_IsConfirmed))
				&& ToDo_IsCompleted;

			return new Tracking(ShowAcceptDeny,
				ShowPersonalStatus, ShowPersonalStatusOnly,
				ShowOverallStatus, ShowOverallStatusOnly,
				ShowActivate, ShowComplete, ShowSuspend, ShowUncomplete, ShowResume, ShowTimeTracking);
		}
		#endregion

		#region GetListToDoForUserByProject
		/// <summary>
		/// DataTable contains fields:
		///  ToDoId, Title, Description, PriorityId, PriorityName, StartDate, FinishDate, 
		///  PercentCompleted, StateId
		/// </summary>
		/// <param name="ProjectId"></param>
		/// <returns></returns>
		public static DataTable GetListToDoForUserByProject(int project_id)
		{
			int user_id = Security.CurrentUser.UserID;
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			return DBToDo.GetListToDoForUserByProject(project_id, user_id, TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListActiveToDoAndTasks
		/// <summary>
		/// ItemId, Title, PriorityId, PriorityName, ManagerId, IsToDo, IsCompleted, 
		/// CompletionTypeId, StartDate, FinishDate, CreationDate, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListActiveToDoAndTasks(int ProjectId)
		{
			return GetListActiveToDoAndTasks(ProjectId, Security.CurrentUser.UserID);
		}

		public static DataTable GetListActiveToDoAndTasks(int ProjectId, int UserId)
		{
			return DBToDo.GetListActiveToDoAndTasks(ProjectId, UserId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListNotApprovedToDoAndTasks
		/// <summary>
		/// ItemId, Title, PriorityId, PriorityName, IsToDo, IsCompleted, 
		/// CompletionTypeId, StartDate, FinishDate, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListNotApprovedToDoAndTasks(int ProjectId)
		{
			return DBToDo.GetListNotApprovedToDoAndTasks(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListPendingToDoAndTasks
		/// <summary>
		/// ItemId, Title, PriorityId, PriorityName, ManagerId, IsToDo, 
		/// IsCompleted, CompletionTypeId, StartDate, FinishDate, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListPendingToDoAndTasks(int ProjectId)
		{
			return DBToDo.GetListPendingToDoAndTasks(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListToDoAndTasksWithoutResources
		/// <summary>
		/// ItemId, Title, PriorityId, PriorityName, IsToDo,
		/// IsCompleted, CompletionTypeId, StartDate, FinishDate, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksWithoutResources(int ProjectId)
		{
			return DBToDo.GetListToDoAndTasksWithoutResources(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListToDoByProject
		/// <summary>
		/// Reader returns fields
		///  ToDoId, Title, Description, StartDate, FinishDate, IsCompleted, PriorityId, 
		///  PriorityName, StateId, ReasonId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoByProject(int ProjectId)
		{
			return DBToDo.GetListToDoByProject(ProjectId, Security.CurrentUser.LanguageId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListToDoAndTaskResourcesNotPending
		/// <summary>
		/// ItemId, Title, PrincipalId, IsConfirmed, IsToDo, IsCompleted, CompletionTypeId, 
		/// LastSavedDate, StartDate, FinishDate, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTaskResourcesNotPending()
		{
			return DBToDo.GetListToDoAndTaskResourcesNotPending(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListToDoAndTaskResourcesNotPendingDataTable
		/// <summary>
		/// ItemId, Title, PrincipalId, IsConfirmed, IsToDo, IsCompleted, CompletionTypeId, 
		/// LastSavedDate, StartDate, FinishDate, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTaskResourcesNotPendingDataTable()
		{
			return DBToDo.GetListToDoAndTaskResourcesNotPendingDataTable(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListNotCompletedToDoAndTasks
		/// <summary>
		/// ItemId, Title, PriorityId, PriorityName, IsToDo, ManagerId, ProjectId, ProjectTitle, 
		///  StartDate, FinishDate, CanEdit, CanDelete,
		///  IsCompleted, CompletionTypeId, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListNotCompletedToDoAndTasks(bool GetAssigned, bool GetManaged, bool GetCreated)
		{
			return DBToDo.GetListNotCompletedToDoAndTasks(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, GetAssigned, GetManaged, GetCreated);
		}
		#endregion

		#region GetListActiveToDoAndTasksWithoutUpcoming
		/// <summary>
		/// ItemId, Title, PriorityId, PriorityName, IsToDo, ManagerId, ProjectId, ProjectTitle, 
		///  StartDate, FinishDate, ShowStartDate, 
		///  ShowFinishDate, TypeId, ManagerId, CanEdit, CanDelete, IsCompleted, CompletionTypeId,
		///  ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListActiveToDoAndTasksWithoutUpcoming(
			bool GetAssigned, bool GetManaged, bool GetCreated)
		{
			DataRow row;

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime UserDate = DBCommon.GetLocalDate(TimeZoneId, DateTime.UtcNow);	// current User's datetime
			DateTime UserStartDate = UserDate.Date;
			DateTime UserFinishDate = UserDate.Date.AddDays(1);

			DataTable table = new DataTable();
			table.Columns.Add("ItemId", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("PriorityId", typeof(int));
			table.Columns.Add("PriorityName", typeof(string));
			table.Columns.Add("IsToDo", typeof(int));
			table.Columns.Add("ProjectId", typeof(int));
			table.Columns.Add("ProjectTitle", typeof(string));
			table.Columns.Add("StartDate", typeof(DateTime));
			table.Columns.Add("FinishDate", typeof(DateTime));
			table.Columns.Add("ShowStartDate", typeof(bool));
			table.Columns.Add("ShowFinishDate", typeof(bool));
			table.Columns.Add("ManagerId", typeof(int));
			table.Columns.Add("CanEdit", typeof(int));
			table.Columns.Add("CanDelete", typeof(int));
			table.Columns.Add("IsCompleted", typeof(bool));
			table.Columns.Add("CompletionTypeId", typeof(int));
			table.Columns.Add("ReasonId", typeof(int));
			table.Columns.Add("StateId", typeof(int));

			using (IDataReader reader = DBToDo.GetListNotCompletedToDoAndTasks(Security.CurrentUser.UserID, TimeZoneId, Security.CurrentUser.LanguageId, GetAssigned, GetManaged, GetCreated))
			{
				while (reader.Read())
				{
					// Upcoming
					if (reader["StartDate"] != DBNull.Value && (DateTime)reader["StartDate"] > UserDate)
						continue;

					row = table.NewRow();

					row["ItemId"] = (int)reader["ItemId"];
					row["Title"] = (string)reader["Title"];
					row["PriorityId"] = (int)reader["PriorityId"];
					row["PriorityName"] = (string)reader["PriorityName"];
					row["IsToDo"] = (int)reader["IsToDo"];

					if (reader["ProjectId"] != DBNull.Value)
						row["ProjectId"] = (int)reader["ProjectId"];
					else
						row["ProjectId"] = -1;

					if (reader["ProjectTitle"] != DBNull.Value)
						row["ProjectTitle"] = (string)reader["ProjectTitle"];
					else
						row["ProjectTitle"] = "";

					if (reader["StartDate"] != DBNull.Value)
					{
						row["StartDate"] = (DateTime)reader["StartDate"];
						row["ShowStartDate"] = true;
					}
					else
					{
						row["StartDate"] = UserStartDate;
						row["ShowStartDate"] = false;
					}

					if (reader["FinishDate"] != DBNull.Value)
					{
						row["FinishDate"] = (DateTime)reader["FinishDate"];
						row["ShowFinishDate"] = true;
					}
					else
					{
						row["FinishDate"] = UserFinishDate;
						row["ShowFinishDate"] = false;
					}

					row["ManagerId"] = (int)reader["ManagerId"];
					row["CanEdit"] = reader["CanEdit"];
					row["CanDelete"] = reader["CanDelete"];

					row["IsCompleted"] = (bool)reader["IsCompleted"];
					row["CompletionTypeId"] = (int)reader["CompletionTypeId"];

					row["ReasonId"] = (int)reader["ReasonId"];
					row["StateId"] = (int)reader["StateId"];

					table.Rows.Add(row);
				}
			}
			return table;
		}
		#endregion

		#region GetListActiveToDoAndTasksWithoutUpcoming
		/// <summary>
		/// ItemId, Title, PriorityId, PriorityName, IsToDo, ManagerId, ProjectId, ProjectTitle, 
		///  StartDate, FinishDate, ShowStartDate, 
		///  ShowFinishDate, TypeId, ManagerId, CanEdit, CanDelete, IsCompleted, CompletionTypeId,
		///  ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListActiveToDoAndTasksWithoutUpcoming(int UserId,
			bool GetAssigned, bool GetManaged, bool GetCreated)
		{
			DataRow row;

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime UserDate = DBCommon.GetLocalDate(TimeZoneId, DateTime.UtcNow);	// current User's datetime
			DateTime UserStartDate = UserDate.Date;
			DateTime UserFinishDate = UserDate.Date.AddDays(1);

			DataTable table = new DataTable();
			table.Columns.Add("ItemId", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("PriorityId", typeof(int));
			table.Columns.Add("PriorityName", typeof(string));
			table.Columns.Add("IsToDo", typeof(int));
			table.Columns.Add("ProjectId", typeof(int));
			table.Columns.Add("ProjectTitle", typeof(string));
			table.Columns.Add("StartDate", typeof(DateTime));
			table.Columns.Add("FinishDate", typeof(DateTime));
			table.Columns.Add("ShowStartDate", typeof(bool));
			table.Columns.Add("ShowFinishDate", typeof(bool));
			table.Columns.Add("ManagerId", typeof(int));
			table.Columns.Add("CanEdit", typeof(int));
			table.Columns.Add("CanDelete", typeof(int));
			table.Columns.Add("IsCompleted", typeof(bool));
			table.Columns.Add("CompletionTypeId", typeof(int));
			table.Columns.Add("ReasonId", typeof(int));
			table.Columns.Add("StateId", typeof(int));

			using (IDataReader reader = DBToDo.GetListNotCompletedToDoAndTasks(UserId, TimeZoneId,
							Security.CurrentUser.LanguageId, GetAssigned, GetManaged, GetCreated))
			{
				while (reader.Read())
				{
					// Upcoming
					if (reader["StartDate"] != DBNull.Value && (DateTime)reader["StartDate"] > UserDate)
						continue;

					row = table.NewRow();

					row["ItemId"] = (int)reader["ItemId"];
					row["Title"] = (string)reader["Title"];
					row["PriorityId"] = (int)reader["PriorityId"];
					row["PriorityName"] = (string)reader["PriorityName"];
					row["IsToDo"] = (int)reader["IsToDo"];

					if (reader["ProjectId"] != DBNull.Value)
						row["ProjectId"] = (int)reader["ProjectId"];
					else
						row["ProjectId"] = -1;

					if (reader["ProjectTitle"] != DBNull.Value)
						row["ProjectTitle"] = (string)reader["ProjectTitle"];
					else
						row["ProjectTitle"] = "";

					if (reader["StartDate"] != DBNull.Value)
					{
						row["StartDate"] = (DateTime)reader["StartDate"];
						row["ShowStartDate"] = true;
					}
					else
					{
						row["StartDate"] = UserStartDate;
						row["ShowStartDate"] = false;
					}

					if (reader["FinishDate"] != DBNull.Value)
					{
						row["FinishDate"] = (DateTime)reader["FinishDate"];
						row["ShowFinishDate"] = true;
					}
					else
					{
						row["FinishDate"] = UserFinishDate;
						row["ShowFinishDate"] = false;
					}

					row["ManagerId"] = (int)reader["ManagerId"];
					row["CanEdit"] = reader["CanEdit"];
					row["CanDelete"] = reader["CanDelete"];

					row["IsCompleted"] = (bool)reader["IsCompleted"];
					row["CompletionTypeId"] = (int)reader["CompletionTypeId"];

					row["ReasonId"] = (int)reader["ReasonId"];
					row["StateId"] = (int)reader["StateId"];

					table.Rows.Add(row);
				}
			}
			return table;
		}
		#endregion

		#region GetListOverdueToDoAndTasks
		/// <summary>
		/// ItemId, Title, PriorityId, PriorityName, IsToDo, ManagerId, ProjectId, ProjectTitle, 
		///  StartDate, FinishDate, ShowStartDate, 
		///  ShowFinishDate, TypeId, ManagerId, CanEdit, CanDelete, IsCompleted, CompletionTypeId,
		///  ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListOverdueToDoAndTasks(
			bool GetAssigned, bool GetManaged, bool GetCreated)
		{
			DataRow row;

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime UserDate = DBCommon.GetLocalDate(TimeZoneId, DateTime.UtcNow);	// current User's datetime
			DateTime UserStartDate = UserDate.Date;
			DateTime UserFinishDate = UserDate.Date.AddDays(1);

			DataTable table = new DataTable();
			table.Columns.Add("ItemId", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("PriorityId", typeof(int));
			table.Columns.Add("PriorityName", typeof(string));
			table.Columns.Add("IsToDo", typeof(int));
			table.Columns.Add("ProjectId", typeof(int));
			table.Columns.Add("ProjectTitle", typeof(string));
			table.Columns.Add("StartDate", typeof(DateTime));
			table.Columns.Add("FinishDate", typeof(DateTime));
			table.Columns.Add("ShowStartDate", typeof(bool));
			table.Columns.Add("ShowFinishDate", typeof(bool));
			table.Columns.Add("ManagerId", typeof(int));
			table.Columns.Add("CanEdit", typeof(int));
			table.Columns.Add("CanDelete", typeof(int));
			table.Columns.Add("IsCompleted", typeof(bool));
			table.Columns.Add("CompletionTypeId", typeof(int));
			table.Columns.Add("ReasonId", typeof(int));
			table.Columns.Add("StateId", typeof(int));

			using (IDataReader reader = DBToDo.GetListNotCompletedToDoAndTasks(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, GetAssigned, GetManaged, GetCreated))
			{
				while (reader.Read())
				{

					if (reader["FinishDate"] == DBNull.Value)
						continue;
					if (reader["FinishDate"] != DBNull.Value && (DateTime)reader["FinishDate"] >= UserDate)
						continue;

					row = table.NewRow();

					row["ItemId"] = (int)reader["ItemId"];
					row["Title"] = (string)reader["Title"];
					row["PriorityId"] = (int)reader["PriorityId"];
					row["PriorityName"] = (string)reader["PriorityName"];
					row["IsToDo"] = (int)reader["IsToDo"];

					if (reader["ProjectId"] != DBNull.Value)
						row["ProjectId"] = (int)reader["ProjectId"];
					else
						row["ProjectId"] = -1;

					if (reader["ProjectTitle"] != DBNull.Value)
						row["ProjectTitle"] = (string)reader["ProjectTitle"];
					else
						row["ProjectTitle"] = "";

					if (reader["StartDate"] != DBNull.Value)
					{
						row["StartDate"] = (DateTime)reader["StartDate"];
						row["ShowStartDate"] = true;
					}
					else
					{
						row["StartDate"] = UserStartDate;
						row["ShowStartDate"] = false;
					}

					row["FinishDate"] = (DateTime)reader["FinishDate"];
					row["ShowFinishDate"] = true;
					row["ManagerId"] = (int)reader["ManagerId"];
					row["CanEdit"] = reader["CanEdit"];
					row["CanDelete"] = reader["CanDelete"];

					row["IsCompleted"] = (bool)reader["IsCompleted"];
					row["CompletionTypeId"] = (int)reader["CompletionTypeId"];
					row["ReasonId"] = (int)reader["ReasonId"];
					row["StateId"] = (int)reader["StateId"];

					table.Rows.Add(row);
				}
			}
			return table;
		}
		#endregion

		#region GetListUpcomingToDoAndTasks
		/// <summary>
		/// ItemId, Title, PriorityId, PriorityName, IsToDo, ManagerId, ProjectId, ProjectTitle, 
		///  StartDate, FinishDate, ShowStartDate, 
		///  ShowFinishDate, TypeId, ManagerId, CanEdit, CanDelete, IsCompleted, CompletionTypeId,
		///  ReasonId, StateID
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListUpcomingToDoAndTasks(bool GetAssigned, bool GetManaged, bool GetCreated)
		{
			DataRow row;

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime UserDate = DBCommon.GetLocalDate(TimeZoneId, DateTime.UtcNow);	// current User's datetime
			DateTime UserStartDate = UserDate.Date;
			DateTime UserFinishDate = UserDate.Date.AddDays(1);

			DataTable table = new DataTable();
			table.Columns.Add("ItemId", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("PriorityId", typeof(int));
			table.Columns.Add("PriorityName", typeof(string));
			table.Columns.Add("IsToDo", typeof(int));
			table.Columns.Add("ProjectId", typeof(int));
			table.Columns.Add("ProjectTitle", typeof(string));
			table.Columns.Add("StartDate", typeof(DateTime));
			table.Columns.Add("FinishDate", typeof(DateTime));
			table.Columns.Add("ShowStartDate", typeof(bool));
			table.Columns.Add("ShowFinishDate", typeof(bool));
			table.Columns.Add("ManagerId", typeof(int));
			table.Columns.Add("CanEdit", typeof(int));
			table.Columns.Add("CanDelete", typeof(int));
			table.Columns.Add("IsCompleted", typeof(bool));
			table.Columns.Add("CompletionTypeId", typeof(int));
			table.Columns.Add("ReasonId", typeof(int));
			table.Columns.Add("StateId", typeof(int));

			using (IDataReader reader = DBToDo.GetListNotCompletedToDoAndTasks(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, GetAssigned, GetManaged, GetCreated))
			{
				while (reader.Read())
				{
					if (reader["StartDate"] != DBNull.Value
						&& (DateTime)reader["StartDate"] > UserDate)
					{
						row = table.NewRow();

						row["ItemId"] = (int)reader["ItemId"];
						row["Title"] = (string)reader["Title"];
						row["PriorityId"] = (int)reader["PriorityId"];
						row["PriorityName"] = (string)reader["PriorityName"];
						row["IsToDo"] = (int)reader["IsToDo"];

						if (reader["ProjectId"] != DBNull.Value)
							row["ProjectId"] = (int)reader["ProjectId"];
						else
							row["ProjectId"] = -1;

						if (reader["ProjectTitle"] != DBNull.Value)
							row["ProjectTitle"] = (string)reader["ProjectTitle"];
						else
							row["ProjectTitle"] = "";

						row["StartDate"] = (DateTime)reader["StartDate"];
						row["ShowStartDate"] = true;
						if (reader["FinishDate"] != DBNull.Value)
						{
							row["FinishDate"] = (DateTime)reader["FinishDate"];
							row["ShowFinishDate"] = true;
						}
						else
						{
							row["FinishDate"] = UserFinishDate;
							row["ShowFinishDate"] = false;
						}
						row["ManagerId"] = (int)reader["ManagerId"];
						row["CanEdit"] = reader["CanEdit"];
						row["CanDelete"] = reader["CanDelete"];

						row["IsCompleted"] = (bool)reader["IsCompleted"];
						row["CompletionTypeId"] = (int)reader["CompletionTypeId"];

						row["ReasonId"] = (int)reader["ReasonId"];
						row["StateId"] = (int)reader["StateId"];

						table.Rows.Add(row);
					}
				}
			}
			return table;
		}
		#endregion

		#region GetListToDoAndTasksForUser
		/// <summary>
		///  Reader returns fields:
		///		ItemId, Title, CreatorId, IsToDo, StartDate, FinishDate, CreationDate,
		///		IsCompleted, CompletionTypeId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTasksForUser(bool GetAssigned, bool GetManaged, bool GetCreated)
		{
			return DBToDo.GetListToDoAndTasksForUser(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, GetAssigned, GetManaged, GetCreated);
		}
		#endregion

		#region GetListToDoAndTasksForUserDataTable
		/// <summary>
		///		ItemId, Title, CreatorId, IsToDo, StartDate, FinishDate, CreationDate,
		///		IsCompleted, CompletionTypeId, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksForUserDataTable(bool GetAssigned, bool GetManaged, bool GetCreated)
		{
			return DBToDo.GetListToDoAndTasksForUserDataTable(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, GetAssigned, GetManaged, GetCreated);
		}
		#endregion

		#region GetListToDoAndTasksByFilterDataTable
		/// <summary>
		///  ItemId, Title, Description, IsCompleted, IsToDo, ProjectId, ProjectTitle, ReasonId
		///		ManagerId, CompletionTypeId, StartDate, FinishDate, CanEdit, CanDelete, PriorityId, 
		///		PercentCompleted, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksByFilterDataTable(
			int ProjectId,
			int ResourceId,
			int StartDateCondition, DateTime StartDate,
			int FinishDateCondition, DateTime FinishDate, string SearchString,
			bool GetAssigned, bool GetManaged, bool GetCreated, int CategoryType)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			int UserId = Security.CurrentUser.UserID;
			StartDate = DBCommon.GetUTCDate(TimeZoneId, StartDate);
			FinishDate = DBCommon.GetUTCDate(TimeZoneId, FinishDate);

			return DBToDo.GetListToDoAndTasksByFilterDataTable(ProjectId, UserId, ResourceId,
				TimeZoneId, StartDateCondition, StartDate, FinishDateCondition, FinishDate,
				SearchString, GetAssigned, GetManaged, GetCreated, CategoryType);
		}
		#endregion

		#region SaveGeneralCategories
		public static void SaveGeneralCategories(ArrayList general_categories)
		{
			Project.SaveGeneralCategories(general_categories);
		}
		#endregion

		#region GetListCategories
		/// <summary>
		/// Reader returns fields:
		///		CategoryId, CategoryName 
		/// </summary>
		public static IDataReader GetListCategories(int todo_id)
		{
			return DBCommon.GetListCategoriesByObject((int)TODO_TYPE, todo_id);
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

		#region GetListToDoAndTasksUpdatedByUser
		/// <summary>
		///		ItemId, Title, IsToDo, IsCompleted, CompletionTypeId, 
		///		StartDate, FinishDate, LastSavedDate
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTasksUpdatedByUser(int Days, int ProjectId)
		{
			return DBToDo.GetListToDoAndTasksUpdatedByUser(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}
		#endregion

		#region GetListToDoAndTasksUpdatedByUserDataTable
		/// <summary>
		///		ItemId, Title, IsToDo, IsCompleted, CompletionTypeId, 
		///		StartDate, FinishDate, LastSavedDate, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksUpdatedByUserDataTable(int Days, int ProjectId)
		{
			return DBToDo.GetListToDoAndTasksUpdatedByUserDataTable(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}
		#endregion

		#region GetListToDoAndTasksUpdatedForUser
		/// <summary>
		///		ItemId, Title, LastEditorId, IsToDo, StartDate, FinishDate, LastSavedDate, 
		///		IsCompleted, CompletionTypeId, ProjectId, ProjectName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTasksUpdatedForUser(int Days, int ProjectId)
		{
			return DBToDo.GetListToDoAndTasksUpdatedForUser(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}
		#endregion

		#region GetListToDoAndTasksUpdatedForUserDataTable
		/// <summary>
		///		ItemId, Title, LastEditorId, IsToDo, StartDate, FinishDate, LastSavedDate, 
		///		IsCompleted, CompletionTypeId, ProjectId, ProjectName, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksUpdatedForUserDataTable(int Days, int ProjectId)
		{
			return DBToDo.GetListToDoAndTasksUpdatedForUserDataTable(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}
		#endregion

		#region GetListToDoAndTasksByKeyword
		/// <summary>
		///  Reader returns fields:
		///		ItemId, Title, Description, ManagerId, IsCompleted, IsToDo, CompletionTypeId,
		///		StartDate, FinishDate, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTasksByKeyword(string Keyword)
		{
			return DBToDo.GetListToDoAndTasksByKeyword(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Keyword);
		}
		#endregion

		#region UploadFile
		public static void UploadFile(int todo_id, string FileName, System.IO.Stream _inputStream)
		{
			if (FileName != null)
			{
				string ContainerName = "FileLibrary";
				string ContainerKey = UserRoleHelper.CreateTodoContainerKey(todo_id);

				BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
				FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
				fs.SaveFile(fs.Root.Id, FileName, _inputStream);

				//				int assetId = Asset.Create(todo_id,ObjectTypes.ToDo, FileName,"", FileName, _inputStream, true);
				//				SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_FileList_FileAdded, todo_id, assetId);
			}
		}
		#endregion

		#region GetListToDoForChangeableRoles
		/// <summary>
		/// Reader returns fields:
		///  ToDoId, Title, IsCompleted, CompletionTypeId, StartDate, FinishDate,
		///  IsManager, IsResource, CanView, CanEdit, CanDelete, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoForChangeableRoles(int UserId)
		{
			return DBToDo.GetListToDoForChangeableRoles(UserId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListToDoAndTaskManagers
		/// <summary>
		/// UserId, FirstName, LastName, FullName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTaskManagers()
		{
			return DBToDo.GetListToDoAndTaskManagers();
		}
		#endregion

		#region GetListToDoAndTaskManagersDataTable
		/// <summary>
		/// UserId, FirstName, LastName, FullName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTaskManagersDataTable()
		{
			return DBToDo.GetListToDoAndTaskManagersDataTable();
		}
		#endregion

		#region GetListToDoAndTaskCreators
		/// <summary>
		/// UserId, FirstName, LastName, FullName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTaskCreators()
		{
			return DBToDo.GetListToDoAndTaskCreators();
		}
		#endregion

		#region GetListToDoAndTaskCreatorsDataTable
		/// <summary>
		/// UserId, FirstName, LastName, FullName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTaskCreatorsDataTable()
		{
			return DBToDo.GetListToDoAndTaskCreatorsDataTable();
		}
		#endregion

		#region GetListCompletedToDoAndTasks
		/// <summary>
		/// ItemId, Title, IsToDo, ProjectId, ProjectTitle, IsCompleted, CompletionTypeId,
		///	ManagerId, StartDate, FinishDate, ShowStartDate, ShowFinishDate, CanEdit, CanDelete, 
		///	ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListCompletedToDoAndTasks(bool GetAssigned, bool GetManaged, bool GetCreated)
		{
			DataRow row;

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime UserDate = DBCommon.GetUTCDate(TimeZoneId, DateTime.UtcNow);	// current User's datetime
			DateTime UserStartDate = UserDate.Date;
			DateTime UserFinishDate = UserDate.Date.AddDays(1);

			DataTable table = new DataTable();
			table.Columns.Add("ItemId", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("IsToDo", typeof(int));
			table.Columns.Add("ProjectId", typeof(int));
			table.Columns.Add("ProjectTitle", typeof(string));
			table.Columns.Add("ManagerId", typeof(int));
			table.Columns.Add("StartDate", typeof(DateTime));
			table.Columns.Add("FinishDate", typeof(DateTime));
			table.Columns.Add("ShowStartDate", typeof(bool));
			table.Columns.Add("ShowFinishDate", typeof(bool));
			table.Columns.Add("CanEdit", typeof(int));
			table.Columns.Add("CanDelete", typeof(int));
			table.Columns.Add("IsCompleted", typeof(bool));
			table.Columns.Add("CompletionTypeId", typeof(int));
			table.Columns.Add("ReasonId", typeof(int));
			table.Columns.Add("StateId", typeof(int));

			using (IDataReader reader = DBToDo.GetListCompletedToDoAndTasks(Security.CurrentUser.UserID,
					  TimeZoneId, GetAssigned, GetManaged, GetCreated))
			{
				while (reader.Read())
				{
					if (!(bool)reader["IsCompleted"])
						continue;

					row = table.NewRow();

					row["ItemId"] = (int)reader["ItemId"];
					row["Title"] = (string)reader["Title"];
					row["IsToDo"] = (int)reader["IsToDo"];

					if (reader["ProjectId"] != DBNull.Value)
						row["ProjectId"] = (int)reader["ProjectId"];
					else
						row["ProjectId"] = -1;

					if (reader["ProjectTitle"] != DBNull.Value)
						row["ProjectTitle"] = (string)reader["ProjectTitle"];
					else
						row["ProjectTitle"] = "";

					if (reader["StartDate"] != DBNull.Value)
					{
						row["StartDate"] = (DateTime)reader["StartDate"];
						row["ShowStartDate"] = true;
					}
					else
					{
						row["StartDate"] = UserStartDate;
						row["ShowStartDate"] = false;
					}

					if (reader["FinishDate"] != DBNull.Value)
					{
						row["FinishDate"] = (DateTime)reader["FinishDate"];
						row["ShowFinishDate"] = true;
					}
					else
					{
						row["FinishDate"] = UserFinishDate;
						row["ShowFinishDate"] = false;
					}

					row["ManagerId"] = (int)reader["ManagerId"];

					row["CanEdit"] = reader["CanEdit"];
					row["CanDelete"] = reader["CanDelete"];

					row["IsCompleted"] = (bool)reader["IsCompleted"];
					row["CompletionTypeId"] = (int)reader["CompletionTypeId"];
					row["ReasonId"] = (int)reader["ReasonId"];
					row["StateId"] = (int)reader["StateId"];

					table.Rows.Add(row);
				}
			}
			return table;
		}
		#endregion

		#region GetListAllAssignmentsByResource
		/// <summary>
		/// ItemId, Title, PriorityId, IsToDo, ManagerId, ProjectId, ProjectTitle, 
		///  StartDate, FinishDate, ShowStartDate, ShowFinishDate, CanEdit, CanDelete,
		///  CompletionTypeId, IsCompleted
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListAllAssignmentsByResource(int ResourceId,
			int ProjectId)
		{
			DataRow row;

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime UserDate = DBCommon.GetUTCDate(TimeZoneId, DateTime.UtcNow);	// current User's datetime
			DateTime UserStartDate = UserDate.Date;
			DateTime UserFinishDate = UserDate.Date.AddDays(1);

			DataTable table = new DataTable();
			table.Columns.Add("ItemId", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("PriorityId", typeof(int));
			table.Columns.Add("IsToDo", typeof(int));
			table.Columns.Add("ManagerId", typeof(int));
			table.Columns.Add("ProjectId", typeof(int));
			table.Columns.Add("ProjectTitle", typeof(string));
			table.Columns.Add("StartDate", typeof(DateTime));
			table.Columns.Add("FinishDate", typeof(DateTime));
			table.Columns.Add("ShowStartDate", typeof(bool));
			table.Columns.Add("ShowFinishDate", typeof(bool));
			table.Columns.Add("CanEdit", typeof(int));
			table.Columns.Add("CanDelete", typeof(int));
			table.Columns.Add("CompletionTypeId", typeof(int));
			table.Columns.Add("IsCompleted", typeof(bool));

			using (IDataReader reader = DBToDo.GetListManagedToDoAndTasksByResource(
					  Security.CurrentUser.UserID, ResourceId, ProjectId, TimeZoneId))
			{
				while (reader.Read())
				{
					row = table.NewRow();

					row["ItemId"] = (int)reader["ItemId"];
					row["Title"] = (string)reader["Title"];
					row["PriorityId"] = (int)reader["PriorityId"];
					row["IsToDo"] = (int)reader["IsToDo"];

					if (reader["ProjectId"] != DBNull.Value)
						row["ProjectId"] = (int)reader["ProjectId"];
					else
						row["ProjectId"] = -1;

					if (reader["ProjectTitle"] != DBNull.Value)
						row["ProjectTitle"] = (string)reader["ProjectTitle"];
					else
						row["ProjectTitle"] = "";

					if (reader["StartDate"] != DBNull.Value)
					{
						row["StartDate"] = (DateTime)reader["StartDate"];
						row["ShowStartDate"] = true;
					}
					else
					{
						row["StartDate"] = UserStartDate;
						row["ShowStartDate"] = false;
					}

					if (reader["FinishDate"] != DBNull.Value)
					{
						row["FinishDate"] = (DateTime)reader["FinishDate"];
						row["ShowFinishDate"] = true;
					}
					else
					{
						row["FinishDate"] = UserFinishDate;
						row["ShowFinishDate"] = false;
					}

					row["ManagerId"] = (int)reader["ManagerId"];
					row["CanEdit"] = reader["CanEdit"];
					row["CanDelete"] = reader["CanDelete"];

					row["CompletionTypeId"] = (int)reader["CompletionTypeId"];
					row["IsCompleted"] = (bool)reader["IsCompleted"];

					table.Rows.Add(row);
				}
			}
			return table;
		}
		#endregion

		#region GetListActiveAssignmentsByResource
		/// <summary>
		/// ItemId, Title, PriorityId, IsToDo, ManagerId, ProjectId, ProjectTitle, 
		///  StartDate, FinishDate, ShowStartDate, ShowFinishDate, CanEdit, CanDelete,
		///  CompletionTypeId, IsCompleted, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListActiveAssignmentsByResource(int ResourceId,
			int ProjectId)
		{
			DataRow row;

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime UserDate = DBCommon.GetUTCDate(TimeZoneId, DateTime.UtcNow);	// current User's datetime
			DateTime UserStartDate = UserDate.Date;
			DateTime UserFinishDate = UserDate.Date.AddDays(1);

			DataTable table = new DataTable();
			table.Columns.Add("ItemId", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("PriorityId", typeof(int));
			table.Columns.Add("IsToDo", typeof(int));
			table.Columns.Add("ManagerId", typeof(int));
			table.Columns.Add("ProjectId", typeof(int));
			table.Columns.Add("ProjectTitle", typeof(string));
			table.Columns.Add("StartDate", typeof(DateTime));
			table.Columns.Add("FinishDate", typeof(DateTime));
			table.Columns.Add("ShowStartDate", typeof(bool));
			table.Columns.Add("ShowFinishDate", typeof(bool));
			table.Columns.Add("CanEdit", typeof(int));
			table.Columns.Add("CanDelete", typeof(int));
			table.Columns.Add("ReasonId", typeof(int));
			table.Columns.Add("CompletionTypeId", typeof(int));
			table.Columns.Add("IsCompleted", typeof(bool));
			table.Columns.Add("StateId", typeof(int));

			using (IDataReader reader = DBToDo.GetListManagedToDoAndTasksByResource(
					  Security.CurrentUser.UserID, ResourceId, ProjectId, TimeZoneId))
			{
				while (reader.Read())
				{
					// Exclude Upcoming
					if (reader["StartDate"] != DBNull.Value && (DateTime)reader["StartDate"] > UserDate)
						continue;

					// Exclude Completed
					if ((bool)reader["IsCompleted"])
						continue;

					row = table.NewRow();

					row["ItemId"] = (int)reader["ItemId"];
					row["Title"] = (string)reader["Title"];
					row["PriorityId"] = (int)reader["PriorityId"];
					row["IsToDo"] = (int)reader["IsToDo"];

					if (reader["ProjectId"] != DBNull.Value)
						row["ProjectId"] = (int)reader["ProjectId"];
					else
						row["ProjectId"] = -1;

					if (reader["ProjectTitle"] != DBNull.Value)
						row["ProjectTitle"] = (string)reader["ProjectTitle"];
					else
						row["ProjectTitle"] = "";

					if (reader["StartDate"] != DBNull.Value)
					{
						row["StartDate"] = (DateTime)reader["StartDate"];
						row["ShowStartDate"] = true;
					}
					else
					{
						row["StartDate"] = UserStartDate;
						row["ShowStartDate"] = false;
					}

					if (reader["FinishDate"] != DBNull.Value)
					{
						row["FinishDate"] = (DateTime)reader["FinishDate"];
						row["ShowFinishDate"] = true;
					}
					else
					{
						row["FinishDate"] = UserFinishDate;
						row["ShowFinishDate"] = false;
					}

					row["ManagerId"] = (int)reader["ManagerId"];
					row["CanEdit"] = reader["CanEdit"];
					row["CanDelete"] = reader["CanDelete"];

					row["ReasonId"] = (int)reader["ReasonId"];
					row["CompletionTypeId"] = (int)reader["CompletionTypeId"];
					row["IsCompleted"] = (bool)reader["IsCompleted"];
					row["StateId"] = (int)reader["StateId"];

					table.Rows.Add(row);
				}
			}
			return table;
		}
		#endregion

		#region GetListCompletedAssignmentsByResource
		/// <summary>
		/// ItemId, Title, PriorityId, IsToDo, ManagerId, ProjectId, ProjectTitle, 
		///  StartDate, FinishDate, ShowStartDate, ShowFinishDate, CanEdit, CanDelete,
		///  CompletionTypeId, IsCompleted, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListCompletedAssignmentsByResource(int ResourceId,
			int ProjectId)
		{
			DataRow row;

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime UserDate = DBCommon.GetUTCDate(TimeZoneId, DateTime.UtcNow);	// current User's datetime
			DateTime UserStartDate = UserDate.Date;
			DateTime UserFinishDate = UserDate.Date.AddDays(1);

			DataTable table = new DataTable();
			table.Columns.Add("ItemId", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("PriorityId", typeof(int));
			table.Columns.Add("IsToDo", typeof(int));
			table.Columns.Add("ManagerId", typeof(int));
			table.Columns.Add("ProjectId", typeof(int));
			table.Columns.Add("ProjectTitle", typeof(string));
			table.Columns.Add("StartDate", typeof(DateTime));
			table.Columns.Add("FinishDate", typeof(DateTime));
			table.Columns.Add("ShowStartDate", typeof(bool));
			table.Columns.Add("ShowFinishDate", typeof(bool));
			table.Columns.Add("CanEdit", typeof(int));
			table.Columns.Add("CanDelete", typeof(int));
			table.Columns.Add("ReasonId", typeof(int));
			table.Columns.Add("CompletionTypeId", typeof(int));
			table.Columns.Add("IsCompleted", typeof(bool));
			table.Columns.Add("StateId", typeof(int));

			using (IDataReader reader = DBToDo.GetListManagedToDoAndTasksByResource(
					  Security.CurrentUser.UserID, ResourceId, ProjectId, TimeZoneId))
			{
				while (reader.Read())
				{
					// Exclude Not Completed
					if (!(bool)reader["IsCompleted"])
						continue;

					row = table.NewRow();

					row["ItemId"] = (int)reader["ItemId"];
					row["Title"] = (string)reader["Title"];
					row["PriorityId"] = (int)reader["PriorityId"];
					row["IsToDo"] = (int)reader["IsToDo"];

					if (reader["ProjectId"] != DBNull.Value)
						row["ProjectId"] = (int)reader["ProjectId"];
					else
						row["ProjectId"] = -1;

					if (reader["ProjectTitle"] != DBNull.Value)
						row["ProjectTitle"] = (string)reader["ProjectTitle"];
					else
						row["ProjectTitle"] = "";

					if (reader["StartDate"] != DBNull.Value)
					{
						row["StartDate"] = (DateTime)reader["StartDate"];
						row["ShowStartDate"] = true;
					}
					else
					{
						row["StartDate"] = UserStartDate;
						row["ShowStartDate"] = false;
					}

					if (reader["ActualFinishDate"] != DBNull.Value)
					{
						row["FinishDate"] = (DateTime)reader["ActualFinishDate"];
						row["ShowFinishDate"] = true;
					}
					else
					{
						row["FinishDate"] = UserFinishDate;
						row["ShowFinishDate"] = false;
					}

					row["ManagerId"] = (int)reader["ManagerId"];
					row["CanEdit"] = reader["CanEdit"];
					row["CanDelete"] = reader["CanDelete"];

					row["ReasonId"] = (int)reader["ReasonId"];
					row["CompletionTypeId"] = (int)reader["CompletionTypeId"];
					row["IsCompleted"] = (bool)reader["IsCompleted"];
					row["StateId"] = (int)reader["StateId"];

					table.Rows.Add(row);
				}
			}
			return table;
		}
		#endregion

		#region GetListOverdueAssignmentsByResource
		/// <summary>
		/// ItemId, Title, PriorityId, IsToDo, ManagerId, ProjectId, ProjectTitle, 
		///  StartDate, FinishDate, ShowStartDate, ShowFinishDate, CanEdit, CanDelete,
		///  CompletionTypeId, IsCompleted, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListOverdueAssignmentsByResource(int ResourceId,
			int ProjectId)
		{
			DataRow row;

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime UserDate = DBCommon.GetUTCDate(TimeZoneId, DateTime.UtcNow);	// current User's datetime
			DateTime UserStartDate = UserDate.Date;
			DateTime UserFinishDate = UserDate.Date.AddDays(1);

			DataTable table = new DataTable();
			table.Columns.Add("ItemId", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("PriorityId", typeof(int));
			table.Columns.Add("IsToDo", typeof(int));
			table.Columns.Add("ManagerId", typeof(int));
			table.Columns.Add("ProjectId", typeof(int));
			table.Columns.Add("ProjectTitle", typeof(string));
			table.Columns.Add("StartDate", typeof(DateTime));
			table.Columns.Add("FinishDate", typeof(DateTime));
			table.Columns.Add("ShowStartDate", typeof(bool));
			table.Columns.Add("ShowFinishDate", typeof(bool));
			table.Columns.Add("CanEdit", typeof(int));
			table.Columns.Add("CanDelete", typeof(int));
			table.Columns.Add("CompletionTypeId", typeof(int));
			table.Columns.Add("IsCompleted", typeof(bool));
			table.Columns.Add("ReasonId", typeof(int));
			table.Columns.Add("StateId", typeof(int));

			using (IDataReader reader = DBToDo.GetListManagedToDoAndTasksByResource(
					  Security.CurrentUser.UserID, ResourceId, ProjectId, TimeZoneId))
			{
				while (reader.Read())
				{
					// Exclude without FinishDate
					if (reader["FinishDate"] == DBNull.Value)
						continue;
					// Exclude Not Overdue
					if (reader["FinishDate"] != DBNull.Value && (DateTime)reader["FinishDate"] >= UserDate)
						continue;
					// Exclude Completed
					if ((bool)reader["IsCompleted"])
						continue;

					row = table.NewRow();

					row["ItemId"] = (int)reader["ItemId"];
					row["Title"] = (string)reader["Title"];
					row["PriorityId"] = (int)reader["PriorityId"];
					row["IsToDo"] = (int)reader["IsToDo"];

					if (reader["ProjectId"] != DBNull.Value)
						row["ProjectId"] = (int)reader["ProjectId"];
					else
						row["ProjectId"] = -1;

					if (reader["ProjectTitle"] != DBNull.Value)
						row["ProjectTitle"] = (string)reader["ProjectTitle"];
					else
						row["ProjectTitle"] = "";

					if (reader["StartDate"] != DBNull.Value)
					{
						row["StartDate"] = (DateTime)reader["StartDate"];
						row["ShowStartDate"] = true;
					}
					else
					{
						row["StartDate"] = UserStartDate;
						row["ShowStartDate"] = false;
					}

					if (reader["FinishDate"] != DBNull.Value)
					{
						row["FinishDate"] = (DateTime)reader["FinishDate"];
						row["ShowFinishDate"] = true;
					}
					else
					{
						row["FinishDate"] = UserFinishDate;
						row["ShowFinishDate"] = false;
					}

					row["ManagerId"] = (int)reader["ManagerId"];
					row["CanEdit"] = reader["CanEdit"];
					row["CanDelete"] = reader["CanDelete"];

					row["ReasonId"] = (int)reader["ReasonId"];
					row["CompletionTypeId"] = (int)reader["CompletionTypeId"];
					row["IsCompleted"] = (bool)reader["IsCompleted"];
					row["StateId"] = (int)reader["StateId"];

					table.Rows.Add(row);
				}
			}
			return table;
		}
		#endregion

		#region GetListUpcomingAssignmentsByResource
		/// <summary>
		/// ItemId, Title, PriorityId, IsToDo, ManagerId, ProjectId, ProjectTitle, 
		///  StartDate, FinishDate, ShowStartDate, ShowFinishDate, CanEdit, CanDelete,
		///  CompletionTypeId, IsCompleted, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListUpcomingAssignmentsByResource(int ResourceId,
			int ProjectId)
		{
			DataRow row;

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime UserDate = DBCommon.GetUTCDate(TimeZoneId, DateTime.UtcNow);	// current User's datetime
			DateTime UserStartDate = UserDate.Date;
			DateTime UserFinishDate = UserDate.Date.AddDays(1);

			DataTable table = new DataTable();
			table.Columns.Add("ItemId", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("PriorityId", typeof(int));
			table.Columns.Add("IsToDo", typeof(int));
			table.Columns.Add("ManagerId", typeof(int));
			table.Columns.Add("ProjectId", typeof(int));
			table.Columns.Add("ProjectTitle", typeof(string));
			table.Columns.Add("StartDate", typeof(DateTime));
			table.Columns.Add("FinishDate", typeof(DateTime));
			table.Columns.Add("ShowStartDate", typeof(bool));
			table.Columns.Add("ShowFinishDate", typeof(bool));
			table.Columns.Add("CanEdit", typeof(int));
			table.Columns.Add("CanDelete", typeof(int));
			table.Columns.Add("ReasonId", typeof(int));
			table.Columns.Add("CompletionTypeId", typeof(int));
			table.Columns.Add("IsCompleted", typeof(bool));
			table.Columns.Add("StateId", typeof(int));

			using (IDataReader reader = DBToDo.GetListManagedToDoAndTasksByResource(
					  Security.CurrentUser.UserID, ResourceId, ProjectId, TimeZoneId))
			{
				while (reader.Read())
				{
					// Exclude Completed
					if ((bool)reader["IsCompleted"])
						continue;

					// Exclude Without StartDate
					if (reader["StartDate"] == DBNull.Value)
						continue;

					// Exclude Started
					if ((DateTime)reader["StartDate"] < UserDate)
						continue;

					row = table.NewRow();

					row["ItemId"] = (int)reader["ItemId"];
					row["Title"] = (string)reader["Title"];
					row["PriorityId"] = (int)reader["PriorityId"];
					row["IsToDo"] = (int)reader["IsToDo"];

					if (reader["ProjectId"] != DBNull.Value)
						row["ProjectId"] = (int)reader["ProjectId"];
					else
						row["ProjectId"] = -1;

					if (reader["ProjectTitle"] != DBNull.Value)
						row["ProjectTitle"] = (string)reader["ProjectTitle"];
					else
						row["ProjectTitle"] = "";

					if (reader["StartDate"] != DBNull.Value)
					{
						row["StartDate"] = (DateTime)reader["StartDate"];
						row["ShowStartDate"] = true;
					}
					else
					{
						row["StartDate"] = UserStartDate;
						row["ShowStartDate"] = false;
					}

					if (reader["FinishDate"] != DBNull.Value)
					{
						row["FinishDate"] = (DateTime)reader["FinishDate"];
						row["ShowFinishDate"] = true;
					}
					else
					{
						row["FinishDate"] = UserFinishDate;
						row["ShowFinishDate"] = false;
					}

					row["ManagerId"] = (int)reader["ManagerId"];
					row["CanEdit"] = reader["CanEdit"];
					row["CanDelete"] = reader["CanDelete"];

					row["ReasonId"] = (int)reader["ReasonId"];
					row["CompletionTypeId"] = (int)reader["CompletionTypeId"];
					row["IsCompleted"] = (bool)reader["IsCompleted"];
					row["StateId"] = (int)reader["StateId"];

					table.Rows.Add(row);
				}
			}
			return table;
		}
		#endregion

		#region GetListToDoAndTasksResourcesByManager
		/// <summary>
		/// UserId, FirstName, LastName, FullName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTasksResourcesByManager()
		{
			return DBToDo.GetListToDoAndTaskResourcesByManager(Security.CurrentUser.UserID);
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

		#region GetListToDoAndTasksResourcesAll
		/// <summary>
		/// UserId, FirstName, LastName, FullName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTasksResourcesAll()
		{
			return DBToDo.GetListToDoAndTaskResourcesByManager(0);
		}
		#endregion

		#region GetListToDoAndTaskPastDueByProject
		/// <summary>
		/// ItemId, Title, Description, IsToDo, FinishDate
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTaskPastDueByProject(int ProjectId)
		{
			return DBToDo.GetListToDoAndTaskPastDueByProject(ProjectId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListToDoAndTaskPastDueByProjectDataTable
		/// <summary>
		/// ItemId, Title, Description, IsToDo, FinishDate
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTaskPastDueByProjectDataTable(int ProjectId)
		{
			return DBToDo.GetListToDoAndTaskPastDueByProjectDataTable(ProjectId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListToDoAndTaskResourcesCount
		/// <summary>
		/// UserId, TaskTodoCount, FirstName, LastName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTaskResourcesCount(int ProjectId)
		{
			return DBToDo.GetListToDoAndTaskResourcesCount(ProjectId);
		}
		#endregion

		#region GetListToDoAndTaskResourcesCountDataTable
		/// <summary>
		/// UserId, TaskTodoCount, FirstName, LastName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTaskResourcesCountDataTable(int ProjectId)
		{
			return DBToDo.GetListToDoAndTaskResourcesCountDataTable(ProjectId);
		}
		#endregion

		#region GetListCompletedToDoAndTasks
		/// <summary>
		/// ItemId, Title, Description, IsCompleted, IsToDo, ProjectId, ProjectTitle, 
		/// ManagerId, StartDate, FinishDate, CanEdit, CanDelete, CompletionTypeId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListCompletedToDoAndTasks(int ProjectId, int UserId)
		{
			return DBToDo.GetListCompletedToDoAndTasks(ProjectId, UserId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListProjectActivitiesCreators
		/// <summary>
		/// UserId, FirstName, LastName, FullName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListProjectActivitiesCreators(int ProjectId)
		{
			return DBToDo.GetListProjectActivitiesCreators(ProjectId);
		}
		#endregion

		#region GetListProjectActivitiesManagers
		/// <summary>
		/// UserId, FirstName, LastName, FullName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListProjectActivitiesManagers(int ProjectId)
		{
			return DBToDo.GetListProjectActivitiesManagers(ProjectId);
		}
		#endregion

		#region CreateToDoForTask
		public static int CreateToDoForTask(
			int task_id,
			int managerId,
			string title,
			string description,
			DateTime start_date,
			DateTime finish_date,
			int priority_id,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			bool complete_task,
			int task_time,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			return CreateToDoForTask(task_id, managerId, title, description, start_date, finish_date,
				priority_id, activation_type, completion_type, must_be_confirmed, complete_task,
				task_time, categories, FileName, _inputStream, null, contactUid, orgUid, -1, -1);
		}

		public static int CreateToDoForTask(
			int task_id,
			int managerId,
			string title,
			string description,
			DateTime start_date,
			DateTime finish_date,
			int priority_id,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			bool complete_task,
			int task_time,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			ArrayList resourceList,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			return CreateToDoForTask(task_id, managerId, title, description, start_date, finish_date,
				priority_id, activation_type, completion_type, must_be_confirmed, complete_task,
				task_time, categories, FileName, _inputStream, resourceList, contactUid, orgUid, -1, -1);
		}

		public static int CreateToDoForTask(
			int task_id,
			int managerId,
			string title,
			string description,
			DateTime start_date,
			DateTime finish_date,
			int priority_id,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			bool complete_task,
			int task_time,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			ArrayList resourceList,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid,
			int predId,
			int succId)
		{
			// Check for security and wrong parameters
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (!Task.CanAddToDo(task_id))
				throw new AccessDeniedException();

			int project_id = DBTask.GetProject(task_id);

			object oStartDate = null;
			object oFinishDate = null;
			DateTime MinValue1 = DateTime.MinValue.AddDays(1);
			int TimeZoneId = Security.CurrentUser.TimeZoneId;

			if (start_date > MinValue1 && finish_date > MinValue1)
			{
				if (start_date > finish_date)
					throw new Exception("wrong dates");
			}

			// Some variables
			if (start_date > MinValue1)
				oStartDate = DBCommon.GetUTCDate(TimeZoneId, start_date);
			if (finish_date > MinValue1)
				oFinishDate = DBCommon.GetUTCDate(TimeZoneId, finish_date);

			if (task_time < 0)
			{
				task_time = int.Parse(PortalConfig.ToDoDefaultValueTaskTimeField);
				if (oStartDate != null && oFinishDate != null)
				{
					int CalendarId = DBProject.GetCalendar(project_id);
					task_time = DBCalendar.GetDurationByFinishDate(CalendarId, (DateTime)oStartDate, (DateTime)oFinishDate);
				}
			}

			DateTime current_date = DateTime.UtcNow;


			int ToDoId = -1;

			// find all containers asset for task
			//			ArrayList	linkAssetArray	=	new ArrayList();
			//			using(IDataReader assetReader = Asset.GetAssetList(task_id, ObjectTypes.Task,false,true))
			//			{
			//				while(assetReader.Read())
			//				{
			//					linkAssetArray.Add((int)assetReader["AssetId"]);
			//				}
			//			}

			int UserId = Security.CurrentUser.UserID;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				ToDoId = DBToDo.Create(project_id, UserId, managerId,
					title, description, current_date, oStartDate, oFinishDate, priority_id,
					activation_type, completion_type, must_be_confirmed, task_time,
					contactUid == PrimaryKeyId.Empty ? null : (object)contactUid,
					orgUid == PrimaryKeyId.Empty ? null : (object)orgUid);

				// OZ: User Role Addon
				UserRoleHelper.AddTodoCreatorRole(ToDoId, UserId);
				UserRoleHelper.AddTodoManagerRole(ToDoId, managerId);
				if (project_id > 0)
				{
					ForeignContainerKey.Add(UserRoleHelper.CreateTodoContainerKey(ToDoId), UserRoleHelper.CreateProjectContainerKey(project_id));
				}
				//

				// O.R: Scheduling
				if (oStartDate != null)
					Schedule.UpdateDateTypeValue(DateTypes.Todo_StartDate, ToDoId, (DateTime)oStartDate);
				if (oFinishDate != null)
					Schedule.UpdateDateTypeValue(DateTypes.Todo_FinishDate, ToDoId, (DateTime)oFinishDate);

				SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Created, ToDoId);

				DBToDo.AddTaskToDo(task_id, ToDoId, complete_task);
				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_TodoList_TodoAdded, task_id, ToDoId);

				if (FileName != null)
				{
					string ContainerName = "FileLibrary";
					string ContainerKey = UserRoleHelper.CreateTaskContainerKey(task_id);

					BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
					FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
					string sFileName = FileName;
					int iRoot = fs.Root.Id;
					int i = 1;
					while (fs.FileExist(sFileName, iRoot))
					{
						string sStart = FileName;
						string sEnd = "";
						if (FileName.LastIndexOf(".") >= 0)
						{
							sEnd = FileName.Substring(FileName.LastIndexOf("."));
							sStart = FileName.Substring(0, FileName.LastIndexOf("."));
						}
						sFileName = sStart + (i++).ToString() + sEnd;
					}
					fs.SaveFile(iRoot, sFileName, _inputStream);

					//					int assetId = Asset.Create(ToDoId, ObjectTypes.ToDo, FileName,"", FileName, _inputStream, false);
					//					SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_FileList_FileAdded, ToDoId, assetId);
				}

				// Categories
				foreach (int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)TODO_TYPE, ToDoId, CategoryId);
				}

				if (resourceList != null)
				{
					foreach (int userId in resourceList)
					{
						DBToDo.AddResource(ToDoId, userId, false);

						// OZ: User Role Addon
						UserRoleHelper.AddTodoResourceRole(ToDoId, userId);
						//

						bool IsExternal = false;
						using (IDataReader reader = DBUser.GetUserInfo(userId))
						{
							if (reader.Read())
							{
								IsExternal = (bool)reader["IsExternal"];
							}
						}

						if (IsExternal)
							DBCommon.AddGate((int)TODO_TYPE, ToDoId, userId);

						SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_ResourceList_AssignmentAdded, ToDoId, userId);
					}
				}

				// O.R: Recalculating project TaskTime
				if (project_id > 0 && task_time > 0)
					TimeTracking.RecalculateProjectTaskTime(project_id);

				if (predId > 0)
					DBToDo.CreateToDoLink(predId, ToDoId);
				if (succId > 0)
					DBToDo.CreateToDoLink(ToDoId, succId);

				RecalculateState(ToDoId);

				tran.Commit();
			}

			return ToDoId;
		}
		#endregion

		#region UpdateTaskCompletion
		public static void UpdateTaskCompletion(int task_id)
		{
			bool WasCompleted = Task.GetIsCompleted(task_id);

			if (!WasCompleted)
			{
				// O.R. [2009-02-12]
				DBCalendar.DeleteStickedObjectForAllUsers((int)ObjectTypes.Task, task_id);

				DBTask.UpdatePercent(task_id, 100);
				DBTask.UpdateCompletion(task_id, true, (int)CompletionReason.CompletedAutomatically);
				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_State, task_id);

				DBTask.RecalculateSummaryPercent(task_id);

				Task.CompleteToDo(task_id);

				Task.RecalculateAllStates(Task.GetProject(task_id));
			}
		}
		#endregion

		#region UpdateDocumentCompletion
		public static void UpdateDocumentCompletion(int document_id)
		{
			bool WasCompleted = Document.GetIsCompleted(document_id);

			if (!WasCompleted)
			{
				DBDocument.UpdateCompletion(document_id, true, (int)CompletionReason.CompletedAutomatically);
				SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_State, document_id);

				Document.CompleteToDo(document_id);

				Document.RecalculateState(document_id);
			}
		}
		#endregion

		#region CompleteIncidentIfNeed
		public static void CompleteIncidentIfNeed(int IncidentId)
		{
			int curState = Incident.GetState(IncidentId);
			if (curState == (int)ObjectStates.Completed)
				return;

			int StateId = (int)ObjectStates.Completed;
			if (Incident.HasControl(IncidentId))
				StateId = (int)ObjectStates.OnCheck;

			Issue2.UpdateState(IncidentId, StateId, false);
		}
		#endregion

		#region GetListActiveToDoAndTasksByUserOnlyDataTable
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, IsToDo, CreationDate, StartDate, FinishDate, 
		///	PercentCompleted, IsResource, IsManager, StateId, ManagerId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListActiveToDoAndTasksByUserOnlyDataTable()
		{
			return DBToDo.GetListActiveToDoAndTasksByUserOnlyDataTable(Security.CurrentUser.UserID,
				Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}

		public static DataTable GetListActiveToDoAndTasksByUserOnlyDataTable(int UserId)
		{
			return DBToDo.GetListActiveToDoAndTasksByUserOnlyDataTable(UserId,
				Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetToDoTitle
		public static string GetToDoTitle(int ToDoId)
		{
			string ToDoTitle = "";
			using (IDataReader reader = GetToDo(ToDoId, false))
			{
				if (reader.Read())
					ToDoTitle = reader["Title"].ToString();
			}
			return ToDoTitle;
		}
		#endregion

		#region AddFavorites
		public static void AddFavorites(int TodoId)
		{
			DBCommon.AddFavorites((int)TODO_TYPE, TodoId, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListFavorites
		/// <summary>
		///		FavoriteId, ObjectTypeId, ObjectId, UserId, Title 
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListFavoritesDT()
		{
			return DBCommon.GetListFavoritesDT((int)TODO_TYPE, Security.CurrentUser.UserID);
		}
		#endregion

		#region CheckFavorites
		public static bool CheckFavorites(int ToDoId)
		{
			return DBCommon.CheckFavorites((int)TODO_TYPE, ToDoId, Security.CurrentUser.UserID);
		}
		#endregion

		#region DeleteFavorites
		public static void DeleteFavorites(int ToDoId)
		{
			DBCommon.DeleteFavorites((int)TODO_TYPE, ToDoId, Security.CurrentUser.UserID);
		}
		#endregion

		#region AddHistory
		public static void AddHistory(int TodoId, string Title)
		{
			DBCommon.AddHistory((int)TODO_TYPE, TodoId, Title, Security.CurrentUser.UserID);
		}
		#endregion

		#region CreateDocumentTodo
		public static int CreateDocumentTodo(
			int document_id,
			int managerId,
			string title,
			string description,
			DateTime start_date,
			DateTime finish_date,
			int priority_id,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			bool complete_document,
			int task_time,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			return CreateDocumentTodo(document_id, managerId, title, description, start_date, finish_date,
				priority_id, activation_type, completion_type, must_be_confirmed, complete_document,
				task_time, categories, FileName, _inputStream, null, contactUid, orgUid, -1, -1);
		}

		public static int CreateDocumentTodo(
			int document_id,
			int managerId,
			string title,
			string description,
			DateTime start_date,
			DateTime finish_date,
			int priority_id,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			bool complete_document,
			int task_time,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			ArrayList resourceList,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			return CreateDocumentTodo(document_id, managerId, title, description, start_date, finish_date,
				priority_id, activation_type, completion_type, must_be_confirmed, complete_document,
				task_time, categories, FileName, _inputStream, resourceList, contactUid, orgUid, -1, -1);
		}

		public static int CreateDocumentTodo(
			int document_id,
			int managerId,
			string title,
			string description,
			DateTime start_date,
			DateTime finish_date,
			int priority_id,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			bool complete_document,
			int task_time,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			ArrayList resourceList,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid,
			int predId,
			int succId)
		{
			// Check for security and wrong parameters
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (!Document.CanAddToDo(document_id))
				throw new AccessDeniedException();

			int project_id = DBDocument.GetProject(document_id);

			object oStartDate = null;
			object oFinishDate = null;
			DateTime MinValue1 = DateTime.MinValue.AddDays(1);
			int TimeZoneId = Security.CurrentUser.TimeZoneId;

			if (start_date > MinValue1 && finish_date > MinValue1)
			{
				if (start_date > finish_date)
					throw new Exception("wrong dates");
			}

			// Some variables
			if (start_date > MinValue1)
				oStartDate = DBCommon.GetUTCDate(TimeZoneId, start_date);
			if (finish_date > MinValue1)
				oFinishDate = DBCommon.GetUTCDate(TimeZoneId, finish_date);

			if (task_time < 0)
			{
				task_time = int.Parse(PortalConfig.ToDoDefaultValueTaskTimeField);
				if (oStartDate != null && oFinishDate != null)
				{
					if (project_id > 0)
					{
						int CalendarId = DBProject.GetCalendar(project_id);
						task_time = DBCalendar.GetDurationByFinishDate(CalendarId, (DateTime)oStartDate, (DateTime)oFinishDate);
					}
					else
					{
						TimeSpan ts = ((DateTime)oFinishDate).Subtract((DateTime)oStartDate);
						task_time = (int)ts.TotalMinutes;
					}
				}
			}

			object oProjectId = null;
			if (project_id > 0)
				oProjectId = project_id;

			DateTime current_date = DateTime.UtcNow;

			string DocumentTitle = DBDocument.GetTitle(document_id);
			int ToDoId = -1;

			// find all containers asset for document 
			//			ArrayList	linkAssetArray	=	new ArrayList();
			//			using(IDataReader assetReader = Asset.GetAssetList(document_id, ObjectTypes.Document,false,true))
			//			{
			//				while(assetReader.Read())
			//				{
			//					linkAssetArray.Add((int)assetReader["AssetId"]);
			//				}
			//			}

			int UserId = Security.CurrentUser.UserID;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				ToDoId = DBToDo.Create(oProjectId, UserId, managerId,
					title, description, current_date, oStartDate, oFinishDate, priority_id,
					activation_type, completion_type, must_be_confirmed, task_time,
					contactUid == PrimaryKeyId.Empty ? null : (object)contactUid,
					orgUid == PrimaryKeyId.Empty ? null : (object)orgUid);

				// OZ: User Role Addon
				UserRoleHelper.AddTodoCreatorRole(ToDoId, UserId);
				UserRoleHelper.AddTodoManagerRole(ToDoId, managerId);
				if (project_id > 0)
				{
					ForeignContainerKey.Add(UserRoleHelper.CreateTodoContainerKey(ToDoId), UserRoleHelper.CreateProjectContainerKey(project_id));
				}
				//

				// O.R: Scheduling
				if (oStartDate != null)
					Schedule.UpdateDateTypeValue(DateTypes.Todo_StartDate, ToDoId, (DateTime)oStartDate);
				if (oFinishDate != null)
					Schedule.UpdateDateTypeValue(DateTypes.Todo_FinishDate, ToDoId, (DateTime)oFinishDate);

				SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Created, ToDoId);

				DBToDo.AddDocumentToDo(document_id, ToDoId, complete_document);

				SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_TodoList_TodoAdded, document_id, ToDoId);

				if (FileName != null)
				{
					string ContainerName = "FileLibrary";
					string ContainerKey = UserRoleHelper.CreateDocumentContainerKey(document_id);

					BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
					FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
					string sFileName = FileName;
					int iRoot = fs.Root.Id;
					int i = 1;
					while (fs.FileExist(sFileName, iRoot))
					{
						string sStart = FileName;
						string sEnd = "";
						if (FileName.LastIndexOf(".") >= 0)
						{
							sEnd = FileName.Substring(FileName.LastIndexOf("."));
							sStart = FileName.Substring(0, FileName.LastIndexOf("."));
						}
						sFileName = sStart + (i++).ToString() + sEnd;
					}
					fs.SaveFile(iRoot, sFileName, _inputStream);
					//					int assetId = Asset.Create(ToDoId,ObjectTypes.ToDo, FileName,"", FileName, _inputStream, false);
					//					SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_FileList_FileAdded, ToDoId, assetId);
				}

				// Categories
				foreach (int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)TODO_TYPE, ToDoId, CategoryId);
				}

				if (resourceList != null)
				{
					foreach (int userId in resourceList)
					{
						DBToDo.AddResource(ToDoId, userId, false);

						// OZ: User Role Addon
						UserRoleHelper.AddTodoResourceRole(ToDoId, userId);
						//

						bool IsExternal = false;
						using (IDataReader reader = DBUser.GetUserInfo(userId))
						{
							if (reader.Read())
							{
								IsExternal = (bool)reader["IsExternal"];
							}
						}

						if (IsExternal)
							DBCommon.AddGate((int)TODO_TYPE, ToDoId, userId);

						SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_ResourceList_AssignmentAdded, ToDoId, userId);
					}
				}

				// O.R: Recalculating project TaskTime
				if (project_id > 0 && task_time > 0)
					TimeTracking.RecalculateProjectTaskTime(project_id);

				if (predId > 0)
					DBToDo.CreateToDoLink(predId, ToDoId);
				if (succId > 0)
					DBToDo.CreateToDoLink(ToDoId, succId);

				RecalculateState(ToDoId);

				tran.Commit();
			}

			return ToDoId;
		}
		#endregion

		#region CanChangeProject
		public static bool CanChangeProject(int todoId)
		{
			bool retval = true;

			// O.R. [2008-07-30] Check that there are no finances
			if (ActualFinances.List(todoId, ObjectTypes.ToDo).Length > 0)
				retval = false;

			return retval;
		}
		#endregion

		#region GetGroupedItemsForManagerViewDataTable
		/// <summary>
		///	GroupId, GroupName, ItemId, Title, ItemType, ManagerId, ProjectId, StateId, 
		/// StartDate, FinishDate, CompletionTypeId, PercentCompleted, PriorityId, PriorityName, 
		/// ActualStartDate, ActualFinishDate, TaskTime, TotalMinutes, TotalApproved,
		/// ContainerName, ContainerType, IsChildToDo, IsOverdue, IsNewMessage
		/// </summary>
		/// <returns></returns>
		
		public static DataTable GetGroupedItemsForManagerViewDataTable(int GroupBy,
			int PrincipalId, int ManagerId, int ProjectId, int categoryId,
			bool ShowActive, ArrayList alTypes,
			DateTime dtCompleted1, DateTime dtCompleted2,
			DateTime dtUpcoming1, DateTime dtUpcoming2,
			PrimaryKeyId orgUid, PrimaryKeyId contactUid, bool showChildTodo)
		{
			return GetGroupedItemsForManagerViewDataTable(GroupBy, PrincipalId, ManagerId, ProjectId, categoryId, ShowActive,
				alTypes, dtCompleted1, dtCompleted2, dtUpcoming1, dtUpcoming2, DateTime.MinValue, DateTime.MinValue,
				orgUid, contactUid, showChildTodo);
		}
		public static DataTable GetGroupedItemsForManagerViewDataTable(int GroupBy,
			int PrincipalId, int ManagerId, int ProjectId, int categoryId,
			bool ShowActive, ArrayList alTypes,
			DateTime dtCompleted1, DateTime dtCompleted2, 
			DateTime dtUpcoming1, DateTime dtUpcoming2,
			DateTime dtCreated1, DateTime dtCreated2,
			PrimaryKeyId orgUid, PrimaryKeyId contactUid, bool showChildTodo)
		{
			DataTable result = new DataTable();
			result.Columns.Add(new DataColumn("GroupId", typeof(string)));
			result.Columns.Add(new DataColumn("GroupName", typeof(string)));
			result.Columns.Add(new DataColumn("ItemId", typeof(int)));
			result.Columns.Add(new DataColumn("Title", typeof(string)));
			result.Columns.Add(new DataColumn("ItemType", typeof(int)));
			result.Columns.Add(new DataColumn("ManagerId", typeof(int)));
			result.Columns.Add(new DataColumn("ProjectId", typeof(int)));
			result.Columns.Add(new DataColumn("ProjectTitle", typeof(string)));
			result.Columns.Add(new DataColumn("StateId", typeof(int)));
			result.Columns.Add(new DataColumn("FinishDate", typeof(DateTime)));
			result.Columns.Add(new DataColumn("StartDate", typeof(DateTime)));
			result.Columns.Add(new DataColumn("CompletionTypeId", typeof(int)));
			result.Columns.Add(new DataColumn("PercentCompleted", typeof(int)));
			result.Columns.Add(new DataColumn("PriorityId", typeof(int)));
			result.Columns.Add(new DataColumn("PriorityName", typeof(string)));
			result.Columns.Add(new DataColumn("ActualStartDate", typeof(DateTime)));
			result.Columns.Add(new DataColumn("ActualFinishDate", typeof(DateTime)));
			result.Columns.Add(new DataColumn("CreationDate", typeof(DateTime)));
			result.Columns.Add(new DataColumn("TaskTime", typeof(int)));
			result.Columns.Add(new DataColumn("TotalMinutes", typeof(int)));
			result.Columns.Add(new DataColumn("TotalApproved", typeof(int)));
			result.Columns.Add(new DataColumn("ContainerName", typeof(string)));
			result.Columns.Add(new DataColumn("ContainerType", typeof(int)));
			result.Columns.Add(new DataColumn("IsChildToDo", typeof(bool)));
			result.Columns.Add(new DataColumn("IsOverdue", typeof(bool)));
			result.Columns.Add(new DataColumn("IsNewMessage", typeof(bool)));
			result.Columns.Add(new DataColumn("ContactUid", typeof(Guid)));
			result.Columns.Add(new DataColumn("OrgUid", typeof(Guid)));
			result.Columns.Add(new DataColumn("ClientUid", typeof(Guid)));
			result.Columns.Add(new DataColumn("ClientName", typeof(string)));
			result.Columns.Add(new DataColumn("CategoryId", typeof(int)));
			result.Columns.Add(new DataColumn("CategoryName", typeof(string)));
			DataRow dr;
			SortedList<object, string> alGroup = new SortedList<object, string>();
			Hashtable htPrincipals = new Hashtable();

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime UserDate = DBCommon.GetLocalDate(TimeZoneId, DateTime.UtcNow);		// current User's datetime

			DateTime dtUpcoming1User = dtUpcoming1;
			DateTime dtUpcoming2User = dtUpcoming2;
			DateTime dtCompleted1User = dtCompleted1;
			DateTime dtCompleted2User = dtCompleted2;
			DateTime dtCreated1User = dtCreated1;
			DateTime dtCreated2User = dtCreated2;

			dtCompleted1 = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, dtCompleted1);
			dtCompleted2 = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, dtCompleted2);
			dtUpcoming1 = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, dtUpcoming1);
			dtUpcoming2 = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, dtUpcoming2);
			dtCreated1 = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, dtCreated1);
			dtCreated2 = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, dtCreated2);

			if (GroupBy != 1)
			{
				#region ToDo & Tasks
				if (alTypes.Contains((int)ObjectTypes.Task) || alTypes.Contains((int)ObjectTypes.ToDo))
				{
					DataTable dt;
					if (showChildTodo)
					{
						if (GroupBy == 5)
							dt = DBToDo.GetListToDoAndTasksForManagerViewWithChildTodoWithCategories(PrincipalId,
								Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId,
								ManagerId, ProjectId, categoryId, ShowActive,
								dtCompleted1, dtCompleted2,
								dtUpcoming1, dtUpcoming2,
								dtCreated1, dtCreated2,
								orgUid, contactUid);
						else
							dt = DBToDo.GetListToDoAndTasksForManagerViewWithChildTodo(PrincipalId,
								Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId,
								ManagerId, ProjectId, categoryId, ShowActive,
								dtCompleted1, dtCompleted2,
								dtUpcoming1, dtUpcoming2,
								dtCreated1, dtCreated2,
								orgUid, contactUid);
					}
					else
					{
						if (GroupBy == 5)
							dt = DBToDo.GetListToDoAndTasksForManagerViewWithCategories(PrincipalId,
								Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId,
								ManagerId, ProjectId, categoryId, ShowActive,
								dtCompleted1, dtCompleted2,
								dtUpcoming1, dtUpcoming2,
								dtCreated1, dtCreated2,
								orgUid, contactUid);
						else
							dt = DBToDo.GetListToDoAndTasksForManagerViewDataTable(PrincipalId,
								Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId,
								ManagerId, ProjectId, categoryId, ShowActive,
								dtCompleted1, dtCompleted2,
								dtUpcoming1, dtUpcoming2,
								dtCreated1, dtCreated2,
								orgUid, contactUid);
					}
					foreach (DataRow _dr in dt.Rows)
						if (alTypes.Contains((int)_dr["ItemType"]))
						{
							// Exclude child todo if container is not visible
							if (showChildTodo && !alTypes.Contains((int)_dr["ContainerType"]))
							{
								continue;
							}

							dr = result.NewRow();
							dr["GroupId"] = "0";
							dr["GroupName"] = "";
							dr["ItemId"] = (int)_dr["ItemId"];
							dr["Title"] = _dr["Title"].ToString();
							dr["ItemType"] = (int)_dr["ItemType"];
							dr["ManagerId"] = (int)_dr["ManagerId"];
							if (_dr["ProjectId"] != DBNull.Value)
								dr["ProjectId"] = (int)_dr["ProjectId"];
							else
								dr["ProjectId"] = 0;
							if (_dr["ProjectTitle"] != DBNull.Value)
								dr["ProjectTitle"] = _dr["ProjectTitle"].ToString();
							else
								dr["ProjectTitle"] = "";
							dr["StateId"] = (int)_dr["StateId"];
							dr["IsOverdue"] = (bool)_dr["IsOverdue"]; ;

							if (_dr["StartDate"] != DBNull.Value)
								dr["StartDate"] = (DateTime)_dr["StartDate"];
							if (_dr["FinishDate"] != DBNull.Value)
								dr["FinishDate"] = (DateTime)_dr["FinishDate"];

							if (_dr["ActualStartDate"] != DBNull.Value)
								dr["ActualStartDate"] = (DateTime)_dr["ActualStartDate"];
							if (_dr["ActualFinishDate"] != DBNull.Value)
								dr["ActualFinishDate"] = (DateTime)_dr["ActualFinishDate"];
							dr["CreationDate"] = (DateTime)_dr["CreationDate"];

							dr["TaskTime"] = (int)_dr["TaskTime"];
							dr["TotalMinutes"] = (int)_dr["TotalMinutes"];
							dr["TotalApproved"] = (int)_dr["TotalApproved"];

							dr["CompletionTypeId"] = (int)_dr["CompletionTypeId"];
							dr["PercentCompleted"] = (int)_dr["PercentCompleted"];
							dr["PriorityId"] = (int)_dr["PriorityId"];
							dr["PriorityName"] = _dr["PriorityName"].ToString();
							if (showChildTodo)
							{
								dr["ContainerName"] = _dr["ContainerName"].ToString();
								dr["ContainerType"] = (int)_dr["ContainerType"];
								dr["IsChildToDo"] = (bool)_dr["IsChildToDo"];
							}
							else
							{
								dr["ContainerName"] = "";
								dr["ContainerType"] = 0;
								dr["IsChildToDo"] = false;
							}
							dr["IsNewMessage"] = false;

							Guid clientUid = Guid.Empty;
							if (_dr["ContactUid"] != DBNull.Value)
								clientUid = (Guid)_dr["ContactUid"];
							if (_dr["OrgUid"] != DBNull.Value)
								clientUid = (Guid)_dr["OrgUid"];
							dr["ClientUid"] = clientUid;
							dr["ContactUid"] = _dr["ContactUid"];
							dr["OrgUid"] = _dr["OrgUid"];
							dr["ClientName"] = _dr["ClientName"];

							if (GroupBy == 5)
							{
								if (_dr["CategoryId"] != DBNull.Value)
									dr["CategoryId"] = _dr["CategoryId"];
								else
									dr["CategoryId"] = 0;
								dr["CategoryName"] = _dr["CategoryName"];
							}

							result.Rows.Add(dr);

							if (GroupBy == 2 && !alGroup.Keys.Contains((int)dr["ManagerId"]))
							{
								int managerId = (int)dr["ManagerId"];
								alGroup.Add(managerId, User.GetUserName(managerId));
							}
							if (GroupBy == 3 && !alGroup.Keys.Contains((int)dr["ProjectId"]))
							{
								alGroup.Add((int)dr["ProjectId"], dr["ProjectTitle"].ToString());
							}
							if (GroupBy == 4 && !alGroup.Keys.Contains(clientUid))
							{
								alGroup.Add(clientUid, dr["ClientName"].ToString());
							}
							if (GroupBy == 5 && !alGroup.Keys.Contains((int)dr["CategoryId"]))
							{
								alGroup.Add((int)dr["CategoryId"], dr["CategoryName"].ToString());
							}
						}
				}
				#endregion
				#region Documents
				if (alTypes.Contains((int)ObjectTypes.Document))
				{
					DataTable dt;
					if (showChildTodo)
					{
						if (GroupBy == 5)
							dt = DBDocument.GetListDocumentsForManagerViewWithChildTodoWithCategories(PrincipalId,
								Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
								ProjectId, categoryId, ShowActive,
								dtCompleted1, dtCompleted2,
								dtUpcoming1, dtUpcoming2,
								dtCreated1, dtCreated2,
								orgUid, contactUid);
						else
							dt = DBDocument.GetListDocumentsForManagerViewWithChildTodo(PrincipalId,
								Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
								ProjectId, categoryId, ShowActive,
								dtCompleted1, dtCompleted2,
								dtUpcoming1, dtUpcoming2,
								dtCreated1, dtCreated2,
								orgUid, contactUid);
					}
					else
					{
						if (GroupBy == 5)
							dt = DBDocument.GetListDocumentsForManagerViewWithCategories(PrincipalId,
								Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
								ProjectId, categoryId, ShowActive,
								dtCompleted1, dtCompleted2,
								dtCreated1, dtCreated2,
								orgUid, contactUid);
						else
							dt = DBDocument.GetListDocumentsForManagerViewDataTable(PrincipalId,
								Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
								ProjectId, categoryId, ShowActive,
								dtCompleted1, dtCompleted2,
								dtCreated1, dtCreated2,
								orgUid, contactUid);
					}
					foreach (DataRow _dr in dt.Rows)
					{
						dr = result.NewRow();
						dr["GroupId"] = "0";
						dr["GroupName"] = "";
						dr["ItemId"] = (int)_dr["ItemId"];
						dr["Title"] = _dr["Title"].ToString();
						dr["ItemType"] = (int)_dr["ItemType"];
						dr["ManagerId"] = (int)_dr["ManagerId"];
						if (_dr["ProjectId"] != DBNull.Value)
							dr["ProjectId"] = (int)_dr["ProjectId"];
						else
							dr["ProjectId"] = 0;
						if (_dr["ProjectTitle"] != DBNull.Value)
							dr["ProjectTitle"] = _dr["ProjectTitle"].ToString();
						else
							dr["ProjectTitle"] = "";
						dr["StateId"] = (int)_dr["StateId"];
						dr["IsOverdue"] = false;
						if (_dr["StartDate"] != DBNull.Value)
							dr["StartDate"] = (DateTime)_dr["StartDate"];
						if (_dr["FinishDate"] != DBNull.Value)
							dr["ActualFinishDate"] = (DateTime)_dr["FinishDate"];
						dr["CreationDate"] = (DateTime)_dr["CreationDate"];

						dr["TaskTime"] = (int)_dr["TaskTime"];
						dr["TotalMinutes"] = (int)_dr["TotalMinutes"];
						dr["TotalApproved"] = (int)_dr["TotalApproved"];

						dr["CompletionTypeId"] = (int)_dr["CompletionTypeId"];
						dr["PercentCompleted"] = (int)_dr["PercentCompleted"];
						dr["PriorityId"] = (int)_dr["PriorityId"];
						dr["PriorityName"] = _dr["PriorityName"].ToString();
						if (showChildTodo)
						{
							dr["ContainerName"] = _dr["ContainerName"].ToString();
							dr["ContainerType"] = (int)_dr["ItemType"];
						}
						else
						{
							dr["ContainerName"] = "";
							dr["ContainerType"] = 0;
						}
						dr["IsChildToDo"] = false;
						dr["IsNewMessage"] = false;

						Guid clientUid = Guid.Empty;
						if (_dr["ContactUid"] != DBNull.Value)
							clientUid = (Guid)_dr["ContactUid"];
						if (_dr["OrgUid"] != DBNull.Value)
							clientUid = (Guid)_dr["OrgUid"];
						dr["ClientUid"] = clientUid;
						dr["ContactUid"] = _dr["ContactUid"];
						dr["OrgUid"] = _dr["OrgUid"];
						dr["ClientName"] = _dr["ClientName"];

						if (GroupBy == 5)
						{
							if (_dr["CategoryId"] != DBNull.Value)
								dr["CategoryId"] = _dr["CategoryId"];
							else
								dr["CategoryId"] = 0;
							dr["CategoryName"] = _dr["CategoryName"];
						}
						result.Rows.Add(dr);

						if (GroupBy == 2 && !alGroup.Keys.Contains((int)dr["ManagerId"]))
						{
							int managerId = (int)dr["ManagerId"];
							alGroup.Add(managerId, User.GetUserName(managerId));
						}
						if (GroupBy == 3 && !alGroup.Keys.Contains((int)dr["ProjectId"]))
						{
							alGroup.Add((int)dr["ProjectId"], dr["ProjectTitle"].ToString());
						}
						if (GroupBy == 4 && !alGroup.Keys.Contains(clientUid))
						{
							alGroup.Add(clientUid, dr["ClientName"].ToString());
						}
						if (GroupBy == 5 && !alGroup.Keys.Contains((int)dr["CategoryId"]))
						{
							alGroup.Add((int)dr["CategoryId"], dr["CategoryName"].ToString());
						}
					}
				}
				#endregion
				#region Issues
				if (alTypes.Contains((int)ObjectTypes.Issue))
				{
					DataTable dt;
					if (showChildTodo)
					{
						if (GroupBy == 5)
							dt = DBIncident.GetListIncidentsForManagerViewWithChildTodoWithCategories(PrincipalId,
								Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
								ProjectId, categoryId, ShowActive,
								dtCompleted1, dtCompleted2,
								dtUpcoming1, dtUpcoming2,
								dtCreated1, dtCreated2,
								orgUid, contactUid);
						else
							dt = DBIncident.GetListIncidentsForManagerViewWithChildTodo(PrincipalId,
								Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
								ProjectId, categoryId, ShowActive,
								dtCompleted1, dtCompleted2,
								dtUpcoming1, dtUpcoming2,
								dtCreated1, dtCreated2,
								orgUid, contactUid);
					}
					else
					{
						if (GroupBy == 5)
							dt = DBIncident.GetListIncidentsForManagerViewWithCategories(PrincipalId,
								Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
								ProjectId, categoryId, ShowActive,
								dtCompleted1, dtCompleted2,
								dtCreated1, dtCreated2,
								orgUid, contactUid);
						else
							dt = DBIncident.GetListIncidentsForManagerViewDataTable(PrincipalId,
								Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
								ProjectId, categoryId, ShowActive,
								dtCompleted1, dtCompleted2,
								dtCreated1, dtCreated2,
								orgUid, contactUid);
					}
					foreach (DataRow _dr in dt.Rows)
					{
						dr = result.NewRow();
						dr["GroupId"] = "0";
						dr["GroupName"] = "";
						dr["ItemId"] = (int)_dr["ItemId"];
						dr["Title"] = _dr["Title"].ToString();
						dr["ItemType"] = (int)_dr["ItemType"];
						dr["ManagerId"] = (int)_dr["ManagerId"];
						if (_dr["ProjectId"] != DBNull.Value)
							dr["ProjectId"] = (int)_dr["ProjectId"];
						else
							dr["ProjectId"] = 0;
						if (_dr["ProjectTitle"] != DBNull.Value)
							dr["ProjectTitle"] = _dr["ProjectTitle"].ToString();
						else
							dr["ProjectTitle"] = "";
						dr["StateId"] = (int)_dr["StateId"];
						dr["IsOverdue"] = (bool)_dr["IsOverdue"];
						if (_dr["StartDate"] != DBNull.Value)
							dr["StartDate"] = (DateTime)_dr["StartDate"];
						if (_dr["FinishDate"] != DBNull.Value)
							dr["FinishDate"] = (DateTime)_dr["FinishDate"];
						if (_dr["ActualFinishDate"] != DBNull.Value)
							dr["ActualFinishDate"] = (DateTime)_dr["ActualFinishDate"];
						if (_dr["ActualStartDate"] != DBNull.Value)
							dr["ActualStartDate"] = (DateTime)_dr["ActualStartDate"];
						dr["CreationDate"] = (DateTime)_dr["CreationDate"];

						dr["TaskTime"] = (int)_dr["TaskTime"];
						dr["TotalMinutes"] = (int)_dr["TotalMinutes"];
						dr["TotalApproved"] = (int)_dr["TotalApproved"];

						dr["CompletionTypeId"] = (int)_dr["CompletionTypeId"];
						dr["PercentCompleted"] = (int)_dr["PercentCompleted"];
						dr["PriorityId"] = (int)_dr["PriorityId"];
						dr["PriorityName"] = _dr["PriorityName"].ToString();
						if (showChildTodo)
						{
							dr["ContainerName"] = _dr["ContainerName"].ToString();
							dr["ContainerType"] = (int)_dr["ItemType"];
						}
						else
						{
							dr["ContainerName"] = "";
							dr["ContainerType"] = 0;
						}
						dr["IsChildToDo"] = false;
						dr["IsNewMessage"] = (bool)_dr["IsNewMessage"];

						Guid clientUid = Guid.Empty;
						if (_dr["ContactUid"] != DBNull.Value)
							clientUid = (Guid)_dr["ContactUid"];
						if (_dr["OrgUid"] != DBNull.Value)
							clientUid = (Guid)_dr["OrgUid"];
						dr["ClientUid"] = clientUid;
						dr["ContactUid"] = _dr["ContactUid"];
						dr["OrgUid"] = _dr["OrgUid"];
						dr["ClientName"] = _dr["ClientName"];

						if (GroupBy == 5)
						{
							if (_dr["CategoryId"] != DBNull.Value)
								dr["CategoryId"] = _dr["CategoryId"];
							else
								dr["CategoryId"] = 0;
							dr["CategoryName"] = _dr["CategoryName"];
						}
						result.Rows.Add(dr);

						if (GroupBy == 2 && !alGroup.Keys.Contains((int)dr["ManagerId"]))
						{
							int managerId = (int)dr["ManagerId"];
							alGroup.Add(managerId, User.GetUserName(managerId));
						}
						if (GroupBy == 3 && !alGroup.Keys.Contains((int)dr["ProjectId"]))
						{
							alGroup.Add((int)dr["ProjectId"], dr["ProjectTitle"].ToString());
						}
						if (GroupBy == 4 && !alGroup.Keys.Contains(clientUid))
						{
							alGroup.Add(clientUid, dr["ClientName"].ToString());
						}
						if (GroupBy == 5 && !alGroup.Keys.Contains((int)dr["CategoryId"]))
						{
							alGroup.Add((int)dr["CategoryId"], dr["CategoryName"].ToString());
						}
					}
				}
				#endregion
				#region Events
				if (alTypes.Contains((int)ObjectTypes.CalendarEntry))
				{
					DataTable dt;
					if (GroupBy == 5)
						dt = DBEvent.GetListEventsForManagerViewWithCategories(PrincipalId,
							Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
							ProjectId, categoryId, ShowActive, 
							dtCompleted1, dtCompleted2, 
							dtUpcoming1, dtUpcoming2,
							dtCreated1, dtCreated2,
							orgUid, contactUid);
					else
						dt = DBEvent.GetListEventsForManagerViewDataTable(PrincipalId,
							Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
							ProjectId, categoryId, ShowActive,
							dtCompleted1, dtCompleted2,
							dtUpcoming1, dtUpcoming2,
							dtCreated1, dtCreated2,
							orgUid, contactUid);
					foreach (DataRow _dr in dt.Rows)
					{
						if (!(bool)_dr["HasRecurrence"])
						{
							dr = result.NewRow();
							dr["GroupId"] = "0";
							dr["GroupName"] = "";
							dr["ItemId"] = (int)_dr["ItemId"];
							dr["Title"] = _dr["Title"].ToString();
							dr["ItemType"] = (int)_dr["ItemType"];
							dr["ManagerId"] = (int)_dr["ManagerId"];
							if (_dr["ProjectId"] != DBNull.Value)
								dr["ProjectId"] = (int)_dr["ProjectId"];
							else
								dr["ProjectId"] = 0;
							if (_dr["ProjectTitle"] != DBNull.Value)
								dr["ProjectTitle"] = _dr["ProjectTitle"].ToString();
							else
								dr["ProjectTitle"] = "";
							dr["StateId"] = (int)_dr["StateId"];
							dr["IsOverdue"] = false;
							if (_dr["StartDate"] != DBNull.Value)
								dr["StartDate"] = (DateTime)_dr["StartDate"];
							if (_dr["FinishDate"] != DBNull.Value)
								dr["FinishDate"] = (DateTime)_dr["FinishDate"];
							dr["CreationDate"] = (DateTime)_dr["CreationDate"];

							dr["TaskTime"] = (int)_dr["TaskTime"];
							dr["TotalMinutes"] = (int)_dr["TotalMinutes"];
							dr["TotalApproved"] = (int)_dr["TotalApproved"];

							dr["CompletionTypeId"] = (int)_dr["CompletionTypeId"];
							dr["PercentCompleted"] = (int)_dr["PercentCompleted"];
							dr["PriorityId"] = (int)_dr["PriorityId"];
							dr["PriorityName"] = _dr["PriorityName"].ToString();
							if (showChildTodo)
							{
								dr["ContainerName"] = _dr["Title"].ToString();
								dr["ContainerType"] = (int)_dr["ItemType"];
							}
							else
							{
								dr["ContainerName"] = "";
								dr["ContainerType"] = 0;
							}
							dr["IsChildToDo"] = false;
							dr["IsNewMessage"] = false;

							Guid clientUid = Guid.Empty;
							if (_dr["ContactUid"] != DBNull.Value)
								clientUid = (Guid)_dr["ContactUid"];
							if (_dr["OrgUid"] != DBNull.Value)
								clientUid = (Guid)_dr["OrgUid"];
							dr["ClientUid"] = clientUid;
							dr["ContactUid"] = _dr["ContactUid"];
							dr["OrgUid"] = _dr["OrgUid"];
							dr["ClientName"] = _dr["ClientName"];

							if (GroupBy == 5)
							{
								if (_dr["CategoryId"] != DBNull.Value)
									dr["CategoryId"] = _dr["CategoryId"];
								else
									dr["CategoryId"] = 0;
								dr["CategoryName"] = _dr["CategoryName"];
							}
							result.Rows.Add(dr);

							if (GroupBy == 2 && !alGroup.Keys.Contains((int)dr["ManagerId"]))
							{
								int managerId = (int)dr["ManagerId"];
								alGroup.Add(managerId, User.GetUserName(managerId));
							}
							if (GroupBy == 3 && !alGroup.Keys.Contains((int)dr["ProjectId"]))
							{
								alGroup.Add((int)dr["ProjectId"], dr["ProjectTitle"].ToString());
							}
							if (GroupBy == 4 && !alGroup.Keys.Contains(clientUid))
							{
								alGroup.Add(clientUid, dr["ClientName"].ToString());
							}
							if (GroupBy == 5 && !alGroup.Keys.Contains((int)dr["CategoryId"]))
							{
								alGroup.Add((int)dr["CategoryId"], dr["CategoryName"].ToString());
							}
						}
						else	// Recurrence
						{
							int StartTime;
							int EndTime;
							CalendarEntry.Recurrence recurrence;
							using (IDataReader r_reader = DBCommon.GetRecurrence((int)ObjectTypes.CalendarEntry, (int)_dr["ItemId"]))
							{
								r_reader.Read();
								recurrence = new CalendarEntry.Recurrence(
									(byte)r_reader["Pattern"],
									(byte)r_reader["SubPattern"],
									(byte)r_reader["Frequency"],
									(byte)r_reader["Weekdays"],
									(byte)r_reader["DayOfMonth"],
									(byte)r_reader["WeekNumber"],
									(byte)r_reader["MonthNumber"],
									(int)r_reader["EndAfter"],
									(DateTime)_dr["StartDate"],
									(DateTime)_dr["FinishDate"],
									(int)r_reader["TimeZoneId"]);
								StartTime = (int)r_reader["StartTime"];
								EndTime = (int)r_reader["EndTime"];
							}

							// Get new StartDate and FinishDate for recurrence TimeZone (not UserTimeOffset)
							DateTime eventStartDate = DateTime.UtcNow;
							using (IDataReader r_reader = DBEvent.GetEventDates((int)_dr["ItemId"], recurrence.TimeZoneId))
							{
								r_reader.Read();
								recurrence.StartDate = ((DateTime)r_reader["StartDate"]).Date;
								recurrence.FinishDate = ((DateTime)r_reader["FinishDate"]).Date;
								eventStartDate = (DateTime)r_reader["StartDate"];
							}
							eventStartDate = DBCommon.GetUTCDate(recurrence.TimeZoneId, eventStartDate);

							// from_date, to_date - in UTC
							DateTime from_date = DateTime.UtcNow.Date.AddDays(-1);
							DateTime to_date = DateTime.UtcNow.Date.AddDays(2);
							if (dtUpcoming2 < DateTime.MaxValue.AddDays(-1))
								to_date = dtUpcoming2;
							if (dtCompleted1 > DateTime.MinValue.AddDays(1))
								from_date = dtCompleted1;
							ArrayList dates = CalendarEntry.GetRecurDates(from_date, to_date, StartTime, eventStartDate, recurrence);
							foreach (DateTime d in dates)	// Dates in UTC (но в предположении, что событие начинается в 00:00. Поэтому надо еще добавить StartTime)
							{
								DateTime UserDt = DBCommon.GetLocalDate(TimeZoneId, d);	// from UTC to User's time
								DateTime _StartDate = UserDt.AddMinutes(StartTime);	// Start Date in User's time
								DateTime _FinishDate = UserDt.AddMinutes(EndTime);	// Finish Date in User's time

								dr = result.NewRow();

								if (_StartDate > UserDate)
								{
									if (dtUpcoming1 >= DateTime.MaxValue.AddDays(-1))	// if we don't need upcoming
										continue;

									if (_StartDate < dtUpcoming1User || _StartDate > dtUpcoming2User)	// out of range
										continue;

									dr["StateId"] = (int)ObjectStates.Upcoming;
								}
								else if (_FinishDate < UserDate)
								{
									if (dtCompleted2 <= DateTime.MinValue.AddDays(1))	// if we don't need completed
										continue;

									if (_FinishDate < dtCompleted1User || _FinishDate > dtCompleted2User)	// out of range
										continue;

									dr["StateId"] = (int)ObjectStates.Completed;
								}
								else
								{
									if (!ShowActive)
										continue;
									dr["StateId"] = (int)ObjectStates.Active;
								}
								dr["IsOverdue"] = false;

								dr["GroupId"] = "0";
								dr["GroupName"] = "";
								dr["ItemId"] = (int)_dr["ItemId"];
								dr["Title"] = _dr["Title"].ToString();
								dr["ItemType"] = (int)_dr["ItemType"];
								dr["ManagerId"] = (int)_dr["ManagerId"];
								if (_dr["ProjectId"] != DBNull.Value)
									dr["ProjectId"] = (int)_dr["ProjectId"];
								else
									dr["ProjectId"] = 0;
								if (_dr["ProjectTitle"] != DBNull.Value)
									dr["ProjectTitle"] = _dr["ProjectTitle"].ToString();
								else
									dr["ProjectTitle"] = "";

								dr["StartDate"] = _StartDate;
								dr["FinishDate"] = _FinishDate;
								dr["CreationDate"] = (DateTime)_dr["CreationDate"];

								dr["TaskTime"] = (int)_dr["TaskTime"];
								dr["TotalMinutes"] = (int)_dr["TotalMinutes"];
								dr["TotalApproved"] = (int)_dr["TotalApproved"];

								dr["CompletionTypeId"] = (int)_dr["CompletionTypeId"];
								dr["PercentCompleted"] = (int)_dr["PercentCompleted"];
								dr["PriorityId"] = (int)_dr["PriorityId"];
								dr["PriorityName"] = _dr["PriorityName"].ToString();
								if (showChildTodo)
									dr["ContainerName"] = _dr["Title"].ToString();
								else
									dr["ContainerName"] = "";
								dr["IsChildToDo"] = false;
								dr["IsNewMessage"] = false;

								Guid clientUid = Guid.Empty;
								if (_dr["ContactUid"] != DBNull.Value)
									clientUid = (Guid)_dr["ContactUid"];
								if (_dr["OrgUid"] != DBNull.Value)
									clientUid = (Guid)_dr["OrgUid"];
								dr["ClientUid"] = clientUid;
								dr["ContactUid"] = _dr["ContactUid"];
								dr["OrgUid"] = _dr["OrgUid"];
								dr["ClientName"] = _dr["ClientName"];

								if (GroupBy == 5)
								{
									if (_dr["CategoryId"] != DBNull.Value)
										dr["CategoryId"] = _dr["CategoryId"];
									else
										dr["CategoryId"] = 0;
									dr["CategoryName"] = _dr["CategoryName"];
								}
								result.Rows.Add(dr);

								if (GroupBy == 2 && !alGroup.Keys.Contains((int)dr["ManagerId"]))
								{
									int managerId = (int)dr["ManagerId"];
									alGroup.Add(managerId, User.GetUserName(managerId));
								}
								if (GroupBy == 3 && !alGroup.Keys.Contains((int)dr["ProjectId"]))
								{
									alGroup.Add((int)dr["ProjectId"], dr["ProjectTitle"].ToString());
								}
								if (GroupBy == 4 && !alGroup.Keys.Contains(clientUid))
								{
									alGroup.Add(clientUid, dr["ClientName"].ToString());
								}
								if (GroupBy == 5 && !alGroup.Keys.Contains((int)dr["CategoryId"]))
								{
									alGroup.Add((int)dr["CategoryId"], dr["CategoryName"].ToString());
								}
							}
						}
					}
				}
				#endregion
			}
			else
			{
				#region Objects
				DataTable dt;
				if (showChildTodo)
					dt = DBToDo.GetListToDoAndTasksForManagerViewGroupedByUserWithChildTodo(PrincipalId,
						Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
						ProjectId, categoryId, ShowActive, 
						dtCompleted1, dtCompleted2, 
						dtUpcoming1, dtUpcoming2,
						dtCreated1, dtCreated2,
						orgUid, contactUid);
				else
					dt = DBToDo.GetListToDoAndTasksForManagerViewGroupedByUser(PrincipalId,
						Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
						ProjectId, categoryId, ShowActive, 
						dtCompleted1, dtCompleted2, 
						dtUpcoming1, dtUpcoming2,
						dtCreated1, dtCreated2,
						orgUid, contactUid);
				foreach (DataRow _dr in dt.Rows)
				{
					if (alTypes.Contains((int)_dr["ItemType"]))
					{
						// Exclude child todo if container is not visible
						if (showChildTodo && !alTypes.Contains((int)_dr["ContainerType"]))
						{
							continue;
						}

						if (!(bool)_dr["HasRecurrence"])
						{
							dr = result.NewRow();
							dr["GroupId"] = _dr["PrincipalId"].ToString();
							dr["GroupName"] = _dr["LastName"].ToString() + " " + _dr["FirstName"].ToString();
							dr["ItemId"] = (int)_dr["ItemId"];
							dr["Title"] = _dr["Title"].ToString();
							dr["ItemType"] = (int)_dr["ItemType"];
							dr["ManagerId"] = (int)_dr["ManagerId"];
							if (_dr["ProjectId"] != DBNull.Value)
								dr["ProjectId"] = (int)_dr["ProjectId"];
							else
								dr["ProjectId"] = 0;
							if (_dr["ProjectTitle"] != DBNull.Value)
								dr["ProjectTitle"] = _dr["ProjectTitle"].ToString();
							else
								dr["ProjectTitle"] = "";
							dr["StateId"] = (int)_dr["StateId"];
							dr["IsOverdue"] = (bool)_dr["IsOverdue"];
							if (_dr["StartDate"] != DBNull.Value)
								dr["StartDate"] = (DateTime)_dr["StartDate"];
							if (_dr["FinishDate"] != DBNull.Value)
								dr["FinishDate"] = (DateTime)_dr["FinishDate"];

							if (_dr["ActualStartDate"] != DBNull.Value)
								dr["ActualStartDate"] = (DateTime)_dr["ActualStartDate"];
							if (_dr["ActualFinishDate"] != DBNull.Value)
								dr["ActualFinishDate"] = (DateTime)_dr["ActualFinishDate"];

							dr["CreationDate"] = (DateTime)_dr["CreationDate"];

							dr["TaskTime"] = (int)_dr["TaskTime"];
							dr["TotalMinutes"] = (int)_dr["TotalMinutes"];
							dr["TotalApproved"] = (int)_dr["TotalApproved"];

							dr["CompletionTypeId"] = (int)_dr["CompletionTypeId"];
							dr["PercentCompleted"] = (int)_dr["PercentCompleted"];
							dr["PriorityId"] = (int)_dr["PriorityId"];
							dr["PriorityName"] = _dr["PriorityName"].ToString();
							if (showChildTodo)
							{
								dr["ContainerName"] = _dr["ContainerName"].ToString();
								dr["ContainerType"] = (int)_dr["ContainerType"];
								dr["IsChildToDo"] = (bool)_dr["IsChildToDo"];
							}
							else
							{
								dr["ContainerName"] = "";
								dr["ContainerType"] = 0;
								dr["IsChildToDo"] = false;
							}
							dr["IsNewMessage"] = (bool)_dr["IsNewMessage"];

							Guid clientUid = Guid.Empty;
							if (_dr["ContactUid"] != DBNull.Value)
								clientUid = (Guid)_dr["ContactUid"];
							if (_dr["OrgUid"] != DBNull.Value)
								clientUid = (Guid)_dr["OrgUid"];
							dr["ClientUid"] = clientUid;
							dr["ContactUid"] = _dr["ContactUid"];
							dr["OrgUid"] = _dr["OrgUid"];
							dr["ClientName"] = _dr["ClientName"];
							result.Rows.Add(dr);

							if (!htPrincipals.ContainsKey(int.Parse(dr["GroupId"].ToString())))
								htPrincipals.Add(int.Parse(dr["GroupId"].ToString()), dr["GroupName"]);
						}
						else	// Recurrence
						{
							int StartTime;
							int EndTime;
							CalendarEntry.Recurrence recurrence;
							using (IDataReader r_reader = DBCommon.GetRecurrence((int)ObjectTypes.CalendarEntry, (int)_dr["ItemId"]))
							{
								r_reader.Read();
								recurrence = new CalendarEntry.Recurrence(
									(byte)r_reader["Pattern"],
									(byte)r_reader["SubPattern"],
									(byte)r_reader["Frequency"],
									(byte)r_reader["Weekdays"],
									(byte)r_reader["DayOfMonth"],
									(byte)r_reader["WeekNumber"],
									(byte)r_reader["MonthNumber"],
									(int)r_reader["EndAfter"],
									(DateTime)_dr["StartDate"],
									(DateTime)_dr["FinishDate"],
									(int)r_reader["TimeZoneId"]);
								StartTime = (int)r_reader["StartTime"];
								EndTime = (int)r_reader["EndTime"];
							}

							// Get new StartDate and FinishDate for recurrence TimeZone (not UserTimeOffset)
							DateTime eventStartDate = DateTime.UtcNow;
							using (IDataReader r_reader = DBEvent.GetEventDates((int)_dr["ItemId"], recurrence.TimeZoneId))
							{
								r_reader.Read();
								recurrence.StartDate = ((DateTime)r_reader["StartDate"]).Date;
								recurrence.FinishDate = ((DateTime)r_reader["FinishDate"]).Date;
								eventStartDate = (DateTime)r_reader["StartDate"];
							}
							eventStartDate = DBCommon.GetUTCDate(recurrence.TimeZoneId, eventStartDate);

							// from_date, to_date - in UTC
							DateTime from_date = DateTime.UtcNow.Date.AddDays(-1);
							DateTime to_date = DateTime.UtcNow.Date.AddDays(2);
							if (dtUpcoming2 < DateTime.MaxValue.AddDays(-1))
								to_date = dtUpcoming2;
							if (dtCompleted1 > DateTime.MinValue.AddDays(1))
								from_date = dtCompleted1;
							ArrayList dates = CalendarEntry.GetRecurDates(from_date, to_date, StartTime, eventStartDate, recurrence);
							foreach (DateTime d in dates)	// Dates in UTC (но в предположении, что событие начинается в 00:00. Поэтому надо еще добавить StartTime)
							{
								DateTime UserDt = DBCommon.GetLocalDate(TimeZoneId, d);	// from UTC to User's time
								DateTime _StartDate = UserDt.AddMinutes(StartTime);	// Start Date in User's time
								DateTime _FinishDate = UserDt.AddMinutes(EndTime);	// Finish Date in User's time

								dr = result.NewRow();

								if (_StartDate > UserDate)
								{
									if (dtUpcoming1 >= DateTime.MaxValue.AddDays(-1))	// if we don't need upcoming
										continue;

									if (_StartDate < dtUpcoming1User || _StartDate > dtUpcoming2User)	// out of range
										continue;

									dr["StateId"] = (int)ObjectStates.Upcoming;
								}
								else if (_FinishDate < UserDate)
								{
									if (dtCompleted2 <= DateTime.MinValue.AddDays(1))	// if we don't need completed
										continue;

									if (_FinishDate < dtCompleted1User || _FinishDate > dtCompleted2User)	// out of range
										continue;

									dr["StateId"] = (int)ObjectStates.Completed;
								}
								else
								{
									if (!ShowActive)
										continue;
									dr["StateId"] = (int)ObjectStates.Active;
								}
								dr["IsOverdue"] = false;

								dr["GroupId"] = _dr["PrincipalId"].ToString();
								dr["GroupName"] = _dr["LastName"].ToString() + " " + _dr["FirstName"].ToString();
								dr["ItemId"] = (int)_dr["ItemId"];
								dr["Title"] = _dr["Title"].ToString();
								dr["ItemType"] = (int)_dr["ItemType"];
								dr["ManagerId"] = (int)_dr["ManagerId"];
								if (_dr["ProjectId"] != DBNull.Value)
									dr["ProjectId"] = (int)_dr["ProjectId"];
								else
									dr["ProjectId"] = 0;
								if (_dr["ProjectTitle"] != DBNull.Value)
									dr["ProjectTitle"] = _dr["ProjectTitle"].ToString();
								else
									dr["ProjectTitle"] = "";

								dr["StartDate"] = _StartDate;
								dr["FinishDate"] = _FinishDate;

								if (_dr["ActualStartDate"] != DBNull.Value)
									dr["ActualStartDate"] = (DateTime)_dr["ActualStartDate"];
								if (_dr["ActualFinishDate"] != DBNull.Value)
									dr["ActualFinishDate"] = (DateTime)_dr["ActualFinishDate"];

								dr["CreationDate"] = (DateTime)_dr["CreationDate"];

								dr["TaskTime"] = (int)_dr["TaskTime"];
								dr["TotalMinutes"] = (int)_dr["TotalMinutes"];
								dr["TotalApproved"] = (int)_dr["TotalApproved"];

								dr["CompletionTypeId"] = (int)_dr["CompletionTypeId"];
								dr["PercentCompleted"] = (int)_dr["PercentCompleted"];
								dr["PriorityId"] = (int)_dr["PriorityId"];
								dr["PriorityName"] = _dr["PriorityName"].ToString();
								if (showChildTodo)
								{
									dr["ContainerName"] = _dr["ContainerName"].ToString();
									dr["ContainerType"] = (int)_dr["ContainerType"];
									dr["IsChildToDo"] = (bool)_dr["IsChildToDo"];
								}
								else
								{
									dr["ContainerName"] = "";
									dr["ContainerType"] = 0;
									dr["IsChildToDo"] = false;
								}
								dr["IsNewMessage"] = false;

								Guid clientUid = Guid.Empty;
								if (_dr["ContactUid"] != DBNull.Value)
									clientUid = (Guid)_dr["ContactUid"];
								if (_dr["OrgUid"] != DBNull.Value)
									clientUid = (Guid)_dr["OrgUid"];
								dr["ClientUid"] = clientUid;
								dr["ContactUid"] = _dr["ContactUid"];
								dr["OrgUid"] = _dr["OrgUid"];
								dr["ClientName"] = _dr["ClientName"];
								result.Rows.Add(dr);

								if (!htPrincipals.ContainsKey(int.Parse(dr["GroupId"].ToString())))
									htPrincipals.Add(int.Parse(dr["GroupId"].ToString()), dr["GroupName"]);
							}
						}
					}
				}
				#endregion
			}

			if (GroupBy > 1)
			{
				#region Grouping
				DataTable dt_clone = result.Clone();
				string str_grouping = "";

				if (GroupBy == 2)
					str_grouping = "ManagerId";
				else if (GroupBy == 3)
					str_grouping = "ProjectId";
				else if (GroupBy == 4)
					str_grouping = "ClientUid";
				else if (GroupBy == 5)
					str_grouping = "CategoryId";

				foreach (KeyValuePair<object, string> kvp in alGroup)
				{
					string group_name = kvp.Value;
					if (GroupBy == 3 && kvp.Key.ToString() == "0")
						group_name = Common.GetWebResourceString("{IbnFramework.Project:NoProject}");
					if (GroupBy == 4 && kvp.Key.ToString() == Guid.Empty.ToString())
						group_name = Common.GetWebResourceString("{IbnFramework.Project:NoClient}");
					if (GroupBy == 5 && kvp.Key.ToString() == "0")
						group_name = Common.GetWebResourceString("{IbnFramework.Project:NoCategory}");

					dr = dt_clone.NewRow();
					dr["GroupId"] = kvp.Key.ToString();
					dr["GroupName"] = group_name;
					dr["ItemId"] = 0;
					dr["Title"] = "";
					dr["ItemType"] = 0;
					dr["ManagerId"] = 0;
					dr["ProjectId"] = 0;
					dr["ProjectTitle"] = "";
					dr["StateId"] = 0;
					dr["CompletionTypeId"] = 0;
					dr["PercentCompleted"] = 0;
					dr["PriorityId"] = 0;
					dr["PriorityName"] = "";
					dr["ContainerName"] = "";
					dr["ContainerType"] = 0;
					dr["IsChildToDo"] = false;
					dr["IsOverdue"] = false;
					dr["IsNewMessage"] = false;
					dt_clone.Rows.Add(dr);

					// if showChildTodo is true, then we can get the duplicating rows for incidents
					// so we should eliminate such rows
					int prevItemId = 0;
					int prevItemType = 0;
					DataRow[] dr_items = result.Select(str_grouping + "='" + kvp.Key.ToString() + "'", "ContainerName, ContainerType, IsChildToDo, Title");
					foreach (DataRow dr1 in dr_items)
					{
						// eliminate duplicating rows
						if (showChildTodo)
						{
							if (prevItemId == (int)dr1["ItemId"] && prevItemType == (int)dr1["ItemType"])
								continue;
							prevItemId = (int)dr1["ItemId"];
							prevItemType = (int)dr1["ItemType"];
						}

						DataRow _dr = dt_clone.NewRow();
						_dr.ItemArray = (Object[])dr1.ItemArray.Clone();
						_dr["GroupId"] = kvp.Key.ToString();;
						_dr["GroupName"] = group_name;
						dt_clone.Rows.Add(_dr);
					}
				}
				#endregion

				return dt_clone;
			}
			else if (GroupBy == 1) //by user
			{
				#region Grouping
				DataTable dt_clone = result.Clone();
				foreach (int Id in htPrincipals.Keys)
				{
					string group_name = htPrincipals[Id].ToString();

					dr = dt_clone.NewRow();
					dr["GroupId"] = Id.ToString();
					dr["GroupName"] = group_name;
					dr["ItemId"] = 0;
					dr["Title"] = "";
					dr["ItemType"] = 0;
					dr["ManagerId"] = 0;
					dr["ProjectId"] = 0;
					dr["ProjectTitle"] = "";
					dr["StateId"] = 0;
					dr["CompletionTypeId"] = 0;
					dr["PercentCompleted"] = 0;
					dr["PriorityId"] = 0;
					dr["PriorityName"] = "";
					dr["ContainerName"] = "";
					dr["ContainerType"] = 0;
					dr["IsChildToDo"] = false;
					dr["IsOverdue"] = false;
					dr["IsNewMessage"] = false;
					dt_clone.Rows.Add(dr);

					// Sorting is defined in SQL
					DataRow[] dr_items = result.Select("GroupId='" + Id + "'");
					foreach (DataRow dr1 in dr_items)
					{
						DataRow _dr = dt_clone.NewRow();
						_dr.ItemArray = (Object[])dr1.ItemArray.Clone();
						dt_clone.Rows.Add(_dr);
					}
				}
				#endregion

				return dt_clone;
			}
			else
			{
				// if showChildTodo is true, then we can get the duplicating rows for incidents
				// so we should eliminate such rows
				if (showChildTodo)
				{
					#region eliminate duplicating rows
					DataTable dt_clone = result.Clone();
					int prevItemId = 0;
					int prevItemType = 0;
					DataRow[] dr_items = result.Select("1=1", "ContainerName, ContainerType, IsChildToDo, Title");
					foreach (DataRow dr1 in dr_items)
					{
						if (prevItemId == (int)dr1["ItemId"] && prevItemType == (int)dr1["ItemType"])
							continue;
						prevItemId = (int)dr1["ItemId"];
						prevItemType = (int)dr1["ItemType"];

						DataRow _dr = dt_clone.NewRow();
						_dr.ItemArray = (Object[])dr1.ItemArray.Clone();
						dt_clone.Rows.Add(_dr);
					}
					#endregion

					return dt_clone; ;
				}
				else
				{
					return result;
				}
			}
		}
		#endregion

		#region GetGroupedItemsForResourceViewDataTable
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate, 
		///	PercentCompleted, IsCompleted, ManagerId, ReasonId, ProjectId, ProjectTitle, StateId, 
		/// CompletionTypeId, ActualStartDate, ActualFinishDate, TaskTime, TotalMinutes, TotalApproved,
		/// IsOverdue, IsNewMessage, ContactUid, OrgUid, ClientName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetGroupedItemsForResourceViewDataTable(int GroupBy, int ResId,
			int ManagerId, int ProjectId, int categoryId,
			bool ShowActive, ArrayList alTypes,
			DateTime dtCompleted, DateTime dtUpcoming, PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			DataTable result = new DataTable();
			result.Columns.Add(new DataColumn("GroupId", typeof(string)));
			result.Columns.Add(new DataColumn("GroupName", typeof(string)));
			result.Columns.Add(new DataColumn("ItemId", typeof(int)));
			result.Columns.Add(new DataColumn("Title", typeof(string)));
			result.Columns.Add(new DataColumn("ItemType", typeof(int)));
			result.Columns.Add(new DataColumn("ManagerId", typeof(int)));
			result.Columns.Add(new DataColumn("ProjectId", typeof(int)));
			result.Columns.Add(new DataColumn("ProjectTitle", typeof(string)));
			result.Columns.Add(new DataColumn("StateId", typeof(int)));
			result.Columns.Add(new DataColumn("StartDate", typeof(DateTime)));
			result.Columns.Add(new DataColumn("FinishDate", typeof(DateTime)));
			result.Columns.Add(new DataColumn("CompletionTypeId", typeof(int)));
			result.Columns.Add(new DataColumn("PercentCompleted", typeof(int)));
			result.Columns.Add(new DataColumn("PriorityId", typeof(int)));
			result.Columns.Add(new DataColumn("PriorityName", typeof(string)));
			result.Columns.Add(new DataColumn("ActualStartDate", typeof(DateTime)));
			result.Columns.Add(new DataColumn("ActualFinishDate", typeof(DateTime)));
			result.Columns.Add(new DataColumn("TaskTime", typeof(int)));
			result.Columns.Add(new DataColumn("TotalMinutes", typeof(int)));
			result.Columns.Add(new DataColumn("TotalApproved", typeof(int)));
			result.Columns.Add(new DataColumn("IsOverdue", typeof(bool)));
			result.Columns.Add(new DataColumn("IsNewMessage", typeof(bool)));
			result.Columns.Add(new DataColumn("ContactUid", typeof(Guid)));
			result.Columns.Add(new DataColumn("OrgUid", typeof(Guid)));
			result.Columns.Add(new DataColumn("ClientUid", typeof(Guid)));
			result.Columns.Add(new DataColumn("ClientName", typeof(string)));
			result.Columns.Add(new DataColumn("CategoryId", typeof(int)));
			result.Columns.Add(new DataColumn("CategoryName", typeof(string)));
			DataRow dr;
			SortedList<object, string> alGroup = new SortedList<object, string>();

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime UserDate = DBCommon.GetLocalDate(TimeZoneId, DateTime.UtcNow);		// current User's datetime

			DateTime dtUpcomingUser = dtUpcoming;
			DateTime dtCompletedUser = dtCompleted;

			dtCompleted = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, dtCompleted);
			dtUpcoming = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, dtUpcoming);

			#region ToDo & Tasks
			if (alTypes.Contains((int)ObjectTypes.Task) || alTypes.Contains((int)ObjectTypes.ToDo))
			{
				DataTable dt;

				if (GroupBy == 5)
					dt = DBToDo.GetListToDoAndTasksForResourceViewWithCategories(ResId,
					Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
					ProjectId, categoryId, ShowActive, dtCompleted, dtUpcoming, orgUid, contactUid);
				else
					dt = DBToDo.GetListToDoAndTasksForResourceViewDataTable(ResId,
						Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
						ProjectId, categoryId, ShowActive, dtCompleted, dtUpcoming, orgUid, contactUid);
				foreach (DataRow _dr in dt.Rows)
					if (alTypes.Contains((int)_dr["ItemType"]))
					{
						dr = result.NewRow();
						dr["GroupId"] = "";
						dr["GroupName"] = "";
						dr["ItemId"] = (int)_dr["ItemId"];
						dr["Title"] = _dr["Title"].ToString();
						dr["ItemType"] = (int)_dr["ItemType"];
						dr["ManagerId"] = (int)_dr["ManagerId"];
						if (_dr["ProjectId"] != DBNull.Value)
							dr["ProjectId"] = (int)_dr["ProjectId"];
						else
							dr["ProjectId"] = 0;
						if (_dr["ProjectTitle"] != DBNull.Value)
							dr["ProjectTitle"] = _dr["ProjectTitle"].ToString();
						else
							dr["ProjectTitle"] = "";
						if (_dr["StartDate"] != DBNull.Value)
							dr["StartDate"] = (DateTime)_dr["StartDate"];
						if (_dr["FinishDate"] != DBNull.Value)
							dr["FinishDate"] = (DateTime)_dr["FinishDate"];

						if (_dr["ActualStartDate"] != DBNull.Value)
							dr["ActualStartDate"] = (DateTime)_dr["ActualStartDate"];
						if (_dr["ActualFinishDate"] != DBNull.Value)
							dr["ActualFinishDate"] = (DateTime)_dr["ActualFinishDate"];

						dr["TaskTime"] = (int)_dr["TaskTime"];
						dr["TotalMinutes"] = (int)_dr["TotalMinutes"];
						dr["TotalApproved"] = (int)_dr["TotalApproved"];

						dr["CompletionTypeId"] = (int)_dr["CompletionTypeId"];
						int iPercCompleted = (int)_dr["PercentCompleted"];
						dr["PercentCompleted"] = iPercCompleted;
						dr["StateId"] = (iPercCompleted == 100) ? 5 : (int)_dr["StateId"];
						dr["IsOverdue"] = (iPercCompleted == 100) ? false : (bool)_dr["IsOverdue"];
						dr["PriorityId"] = (int)_dr["PriorityId"];
						dr["PriorityName"] = _dr["PriorityName"].ToString();
						dr["IsNewMessage"] = false;

						Guid clientUid = Guid.Empty;
						if (_dr["ContactUid"] != DBNull.Value)
							clientUid = (Guid)_dr["ContactUid"];
						if (_dr["OrgUid"] != DBNull.Value)
							clientUid = (Guid)_dr["OrgUid"];
						dr["ClientUid"] = clientUid;
						dr["ContactUid"] = _dr["ContactUid"];
						dr["OrgUid"] = _dr["OrgUid"];
						dr["ClientName"] = _dr["ClientName"];

						if (GroupBy == 5)
						{
							if (_dr["CategoryId"] != DBNull.Value)
								dr["CategoryId"] = _dr["CategoryId"];
							else
								dr["CategoryId"] = 0;
							dr["CategoryName"] = _dr["CategoryName"];
						}

						result.Rows.Add(dr);

						if (GroupBy == 2 && !alGroup.Keys.Contains((int)dr["ManagerId"]))
						{
							int managerId = (int)dr["ManagerId"];
							alGroup.Add(managerId, User.GetUserName(managerId));
						}
						if (GroupBy == 3 && !alGroup.Keys.Contains((int)dr["ProjectId"]))
						{
							alGroup.Add((int)dr["ProjectId"], dr["ProjectTitle"].ToString());
						}
						if (GroupBy == 4 && !alGroup.Keys.Contains(clientUid))
						{
							alGroup.Add(clientUid, dr["ClientName"].ToString());
						}
						if (GroupBy == 5 && !alGroup.Keys.Contains((int)dr["CategoryId"]))
						{
							alGroup.Add((int)dr["CategoryId"], dr["CategoryName"].ToString());
						}
					}
			}
			#endregion
			#region Documents
			if (alTypes.Contains((int)ObjectTypes.Document))
			{
				DataTable dt;
				if (GroupBy == 5)
					dt = DBDocument.GetListDocumentsForResourceViewWithCategories(ResId,
					 Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
					 ProjectId, categoryId, ShowActive, dtCompleted, dtUpcoming, orgUid, contactUid);
				else
					dt = DBDocument.GetListDocumentsForResourceViewDataTable(ResId,
						 Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
						 ProjectId, categoryId, ShowActive, dtCompleted, dtUpcoming, orgUid, contactUid);
				foreach (DataRow _dr in dt.Rows)
				{
					dr = result.NewRow();
					dr["GroupId"] = "";
					dr["GroupName"] = "";
					dr["ItemId"] = (int)_dr["ItemId"];
					dr["Title"] = _dr["Title"].ToString();
					dr["ItemType"] = (int)_dr["ItemType"];
					dr["ManagerId"] = (int)_dr["ManagerId"];
					if (_dr["ProjectId"] != DBNull.Value)
						dr["ProjectId"] = (int)_dr["ProjectId"];
					else
						dr["ProjectId"] = 0;
					if (_dr["ProjectTitle"] != DBNull.Value)
						dr["ProjectTitle"] = _dr["ProjectTitle"].ToString();
					else
						dr["ProjectTitle"] = "";
					dr["StateId"] = (int)_dr["StateId"];
					dr["IsOverdue"] = false;
					if (_dr["StartDate"] != DBNull.Value)
						dr["StartDate"] = (DateTime)_dr["StartDate"];
					if (_dr["FinishDate"] != DBNull.Value)
						dr["ActualFinishDate"] = (DateTime)_dr["FinishDate"];

					dr["TaskTime"] = (int)_dr["TaskTime"];
					dr["TotalMinutes"] = (int)_dr["TotalMinutes"];
					dr["TotalApproved"] = (int)_dr["TotalApproved"];

					dr["CompletionTypeId"] = (int)_dr["CompletionTypeId"];
					dr["PercentCompleted"] = (int)_dr["PercentCompleted"];
					dr["PriorityId"] = (int)_dr["PriorityId"];
					dr["PriorityName"] = _dr["PriorityName"].ToString();
					dr["IsNewMessage"] = false;

					Guid clientUid = Guid.Empty;
					if (_dr["ContactUid"] != DBNull.Value)
						clientUid = (Guid)_dr["ContactUid"];
					if (_dr["OrgUid"] != DBNull.Value)
						clientUid = (Guid)_dr["OrgUid"];
					dr["ClientUid"] = clientUid;
					dr["ContactUid"] = _dr["ContactUid"];
					dr["OrgUid"] = _dr["OrgUid"];
					dr["ClientName"] = _dr["ClientName"];

					if (GroupBy == 5)
					{
						if (_dr["CategoryId"] != DBNull.Value)
							dr["CategoryId"] = _dr["CategoryId"];
						else
							dr["CategoryId"] = 0;
						dr["CategoryName"] = _dr["CategoryName"];
					}

					result.Rows.Add(dr);

					if (GroupBy == 2 && !alGroup.Keys.Contains((int)dr["ManagerId"]))
					{
						int managerId = (int)dr["ManagerId"];
						alGroup.Add(managerId, User.GetUserName(managerId));
					}
					if (GroupBy == 3 && !alGroup.Keys.Contains((int)dr["ProjectId"]))
					{
						alGroup.Add((int)dr["ProjectId"], dr["ProjectTitle"].ToString());
					}
					if (GroupBy == 4 && !alGroup.Keys.Contains(clientUid))
					{
						alGroup.Add(clientUid, dr["ClientName"].ToString());
					}
					if (GroupBy == 5 && !alGroup.Keys.Contains((int)dr["CategoryId"]))
					{
						alGroup.Add((int)dr["CategoryId"], dr["CategoryName"].ToString());
					}
				}
			}
			#endregion
			#region Issues
			if (alTypes.Contains((int)ObjectTypes.Issue))
			{
				DataTable dt;
				if (GroupBy == 5)
					dt = DBIncident.GetListIncidentsForResourceViewWithCategories(ResId,
					 Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
					 ProjectId, categoryId, ShowActive, dtCompleted, dtUpcoming, orgUid, contactUid);
				else
					dt = DBIncident.GetListIncidentsForResourceViewDataTable(ResId,
						 Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
						 ProjectId, categoryId, ShowActive, dtCompleted, dtUpcoming, orgUid, contactUid);
				foreach (DataRow _dr in dt.Rows)
				{
					dr = result.NewRow();
					dr["GroupId"] = "";
					dr["GroupName"] = "";
					dr["ItemId"] = (int)_dr["ItemId"];
					dr["Title"] = _dr["Title"].ToString();
					dr["ItemType"] = (int)_dr["ItemType"];
					dr["ManagerId"] = (int)_dr["ManagerId"];
					if (_dr["ProjectId"] != DBNull.Value)
						dr["ProjectId"] = (int)_dr["ProjectId"];
					else
						dr["ProjectId"] = 0;
					if (_dr["ProjectTitle"] != DBNull.Value)
						dr["ProjectTitle"] = _dr["ProjectTitle"].ToString();
					else
						dr["ProjectTitle"] = "";
					dr["StateId"] = (int)_dr["StateId"];
					dr["IsOverdue"] = (bool)_dr["IsOverdue"];
					if (_dr["StartDate"] != DBNull.Value)
						dr["StartDate"] = (DateTime)_dr["StartDate"];
					if (_dr["FinishDate"] != DBNull.Value)
						dr["FinishDate"] = (DateTime)_dr["FinishDate"];
					if (_dr["ActualFinishDate"] != DBNull.Value)
						dr["ActualFinishDate"] = (DateTime)_dr["ActualFinishDate"];
					if (_dr["ActualStartDate"] != DBNull.Value)
						dr["ActualStartDate"] = (DateTime)_dr["ActualStartDate"];

					dr["TaskTime"] = (int)_dr["TaskTime"];
					dr["TotalMinutes"] = (int)_dr["TotalMinutes"];
					dr["TotalApproved"] = (int)_dr["TotalApproved"];

					dr["CompletionTypeId"] = (int)_dr["CompletionTypeId"];
					dr["PercentCompleted"] = (int)_dr["PercentCompleted"];
					dr["PriorityId"] = (int)_dr["PriorityId"];
					dr["PriorityName"] = _dr["PriorityName"].ToString();
					dr["IsNewMessage"] = (bool)_dr["IsNewMessage"];

					Guid clientUid = Guid.Empty;
					if (_dr["ContactUid"] != DBNull.Value)
						clientUid = (Guid)_dr["ContactUid"];
					if (_dr["OrgUid"] != DBNull.Value)
						clientUid = (Guid)_dr["OrgUid"];
					dr["ClientUid"] = clientUid;
					dr["ContactUid"] = _dr["ContactUid"];
					dr["OrgUid"] = _dr["OrgUid"];
					dr["ClientName"] = _dr["ClientName"];

					if (GroupBy == 5)
					{
						if (_dr["CategoryId"] != DBNull.Value)
							dr["CategoryId"] = _dr["CategoryId"];
						else
							dr["CategoryId"] = 0;
						dr["CategoryName"] = _dr["CategoryName"];
					}
					result.Rows.Add(dr);

					if (GroupBy == 2 && !alGroup.Keys.Contains((int)dr["ManagerId"]))
					{
						int managerId = (int)dr["ManagerId"];
						alGroup.Add(managerId, User.GetUserName(managerId));
					}
					if (GroupBy == 3 && !alGroup.Keys.Contains((int)dr["ProjectId"]))
					{
						alGroup.Add((int)dr["ProjectId"], dr["ProjectTitle"].ToString());
					}
					if (GroupBy == 4 && !alGroup.Keys.Contains(clientUid))
					{
						alGroup.Add(clientUid, dr["ClientName"].ToString());
					}
					if (GroupBy == 5 && !alGroup.Keys.Contains((int)dr["CategoryId"]))
					{
						alGroup.Add((int)dr["CategoryId"], dr["CategoryName"].ToString());
					}
				}
			}
			#endregion
			#region Events
			if (alTypes.Contains((int)ObjectTypes.CalendarEntry))
			{
				DataTable dt;
				if (GroupBy == 5)
					dt = DBEvent.GetListEventsForResourceViewWithCategories(ResId,
					 Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
					 ProjectId, categoryId, ShowActive, dtCompleted, dtUpcoming, orgUid, contactUid);
				else
					dt = DBEvent.GetListEventsForResourceViewDataTable(ResId,
						 Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
						 ProjectId, categoryId, ShowActive, dtCompleted, dtUpcoming, orgUid, contactUid);
				foreach (DataRow _dr in dt.Rows)
				{
					if (!(bool)_dr["HasRecurrence"])
					{
						dr = result.NewRow();
						dr["GroupId"] = "";
						dr["GroupName"] = "";
						dr["ItemId"] = (int)_dr["ItemId"];
						dr["Title"] = _dr["Title"].ToString();
						dr["ItemType"] = (int)_dr["ItemType"];
						dr["ManagerId"] = (int)_dr["ManagerId"];
						if (_dr["ProjectId"] != DBNull.Value)
							dr["ProjectId"] = (int)_dr["ProjectId"];
						else
							dr["ProjectId"] = 0;
						if (_dr["ProjectTitle"] != DBNull.Value)
							dr["ProjectTitle"] = _dr["ProjectTitle"].ToString();
						else
							dr["ProjectTitle"] = "";
						dr["StateId"] = (int)_dr["StateId"];
						dr["IsOverdue"] = false;
						if (_dr["StartDate"] != DBNull.Value)
							dr["StartDate"] = (DateTime)_dr["StartDate"];
						if (_dr["FinishDate"] != DBNull.Value)
							dr["FinishDate"] = (DateTime)_dr["FinishDate"];

						dr["TaskTime"] = (int)_dr["TaskTime"];
						dr["TotalMinutes"] = (int)_dr["TotalMinutes"];
						dr["TotalApproved"] = (int)_dr["TotalApproved"];

						dr["CompletionTypeId"] = (int)_dr["CompletionTypeId"];
						dr["PercentCompleted"] = (int)_dr["PercentCompleted"];
						dr["PriorityId"] = (int)_dr["PriorityId"];
						dr["PriorityName"] = _dr["PriorityName"].ToString();
						dr["IsNewMessage"] = false;

						Guid clientUid = Guid.Empty;
						if (_dr["ContactUid"] != DBNull.Value)
							clientUid = (Guid)_dr["ContactUid"];
						if (_dr["OrgUid"] != DBNull.Value)
							clientUid = (Guid)_dr["OrgUid"];
						dr["ClientUid"] = clientUid;
						dr["ContactUid"] = _dr["ContactUid"];
						dr["OrgUid"] = _dr["OrgUid"];
						dr["ClientName"] = _dr["ClientName"];

						if (GroupBy == 5)
						{
							if (_dr["CategoryId"] != DBNull.Value)
								dr["CategoryId"] = _dr["CategoryId"];
							else
								dr["CategoryId"] = 0;
							dr["CategoryName"] = _dr["CategoryName"];
						}
						result.Rows.Add(dr);

						if (GroupBy == 2 && !alGroup.Keys.Contains((int)dr["ManagerId"]))
						{
							int managerId = (int)dr["ManagerId"];
							alGroup.Add(managerId, User.GetUserName(managerId));
						}
						if (GroupBy == 3 && !alGroup.Keys.Contains((int)dr["ProjectId"]))
						{
							alGroup.Add((int)dr["ProjectId"], dr["ProjectTitle"].ToString());
						}
						if (GroupBy == 4 && !alGroup.Keys.Contains(clientUid))
						{
							alGroup.Add(clientUid, dr["ClientName"].ToString());
						}
						if (GroupBy == 5 && !alGroup.Keys.Contains((int)dr["CategoryId"]))
						{
							alGroup.Add((int)dr["CategoryId"], dr["CategoryName"].ToString());
						}
					}
					else	// Recurrence
					{
						int StartTime;
						int EndTime;
						CalendarEntry.Recurrence recurrence;
						using (IDataReader r_reader = DBCommon.GetRecurrence((int)ObjectTypes.CalendarEntry, (int)_dr["ItemId"]))
						{
							r_reader.Read();
							recurrence = new CalendarEntry.Recurrence(
								(byte)r_reader["Pattern"],
								(byte)r_reader["SubPattern"],
								(byte)r_reader["Frequency"],
								(byte)r_reader["Weekdays"],
								(byte)r_reader["DayOfMonth"],
								(byte)r_reader["WeekNumber"],
								(byte)r_reader["MonthNumber"],
								(int)r_reader["EndAfter"],
								(DateTime)_dr["StartDate"],
								(DateTime)_dr["FinishDate"],
								(int)r_reader["TimeZoneId"]);
							StartTime = (int)r_reader["StartTime"];
							EndTime = (int)r_reader["EndTime"];
						}

						// Get new StartDate and FinishDate for recurrence TimeZone (not UserTimeOffset)
						DateTime eventStartDate = DateTime.UtcNow;
						using (IDataReader r_reader = DBEvent.GetEventDates((int)_dr["ItemId"], recurrence.TimeZoneId))
						{
							r_reader.Read();
							recurrence.StartDate = ((DateTime)r_reader["StartDate"]).Date;
							recurrence.FinishDate = ((DateTime)r_reader["FinishDate"]).Date;
							eventStartDate = (DateTime)r_reader["StartDate"];
						}
						eventStartDate = DBCommon.GetUTCDate(recurrence.TimeZoneId, eventStartDate);

						// from_date, to_date - in UTC
						DateTime from_date = DateTime.UtcNow.Date.AddDays(-1);
						DateTime to_date = DateTime.UtcNow.Date.AddDays(2);
						if (dtUpcoming < DateTime.MaxValue.AddDays(-1))
							to_date = dtUpcoming;
						if (dtCompleted > DateTime.MinValue.AddDays(1))
							from_date = dtCompleted;
						ArrayList dates = CalendarEntry.GetRecurDates(from_date, to_date, StartTime, eventStartDate, recurrence);
						foreach (DateTime d in dates)	// Dates in UTC (но в предположении, что событие начинается в 00:00. Поэтому надо еще добавить StartTime)
						{
							DateTime UserDt = DBCommon.GetLocalDate(TimeZoneId, d);	// from UTC to User's time
							DateTime _StartDate = UserDt.AddMinutes(StartTime);
							DateTime _FinishDate = UserDt.AddMinutes(EndTime);

							dr = result.NewRow();

							if (_StartDate > UserDate)
							{
								if (_StartDate > dtUpcomingUser)	// if we don't need upcoming
									continue;
								dr["StateId"] = (int)ObjectStates.Upcoming;
							}
							else if (_FinishDate < UserDate)
							{
								if (_FinishDate < dtCompletedUser)	// if we don't need completed
									continue;
								dr["StateId"] = (int)ObjectStates.Completed;
							}
							else
							{
								if (!ShowActive)
									continue;
								dr["StateId"] = (int)ObjectStates.Active;
							}
							dr["IsOverdue"] = false;

							dr["GroupId"] = "";
							dr["GroupName"] = "";
							dr["ItemId"] = (int)_dr["ItemId"];
							dr["Title"] = _dr["Title"].ToString();
							dr["ItemType"] = (int)_dr["ItemType"];
							dr["ManagerId"] = (int)_dr["ManagerId"];
							if (_dr["ProjectId"] != DBNull.Value)
								dr["ProjectId"] = (int)_dr["ProjectId"];
							else
								dr["ProjectId"] = 0;
							if (_dr["ProjectTitle"] != DBNull.Value)
								dr["ProjectTitle"] = _dr["ProjectTitle"].ToString();
							else
								dr["ProjectTitle"] = "";

							dr["StartDate"] = _StartDate;
							dr["FinishDate"] = _FinishDate;

							dr["TaskTime"] = (int)_dr["TaskTime"];
							dr["TotalMinutes"] = (int)_dr["TotalMinutes"];
							dr["TotalApproved"] = (int)_dr["TotalApproved"];

							dr["CompletionTypeId"] = (int)_dr["CompletionTypeId"];
							dr["PercentCompleted"] = (int)_dr["PercentCompleted"];
							dr["PriorityId"] = (int)_dr["PriorityId"];
							dr["PriorityName"] = _dr["PriorityName"].ToString();
							dr["IsNewMessage"] = false;

							Guid clientUid = Guid.Empty;
							if (_dr["ContactUid"] != DBNull.Value)
								clientUid = (Guid)_dr["ContactUid"];
							if (_dr["OrgUid"] != DBNull.Value)
								clientUid = (Guid)_dr["OrgUid"];
							dr["ClientUid"] = clientUid;
							dr["ContactUid"] = _dr["ContactUid"];
							dr["OrgUid"] = _dr["OrgUid"];
							dr["ClientName"] = _dr["ClientName"];

							if (GroupBy == 5)
							{
								if (_dr["CategoryId"] != DBNull.Value)
									dr["CategoryId"] = _dr["CategoryId"];
								else
									dr["CategoryId"] = 0;
								dr["CategoryName"] = _dr["CategoryName"];
							}
							result.Rows.Add(dr);

							if (GroupBy == 2 && !alGroup.Keys.Contains((int)dr["ManagerId"]))
							{
								int managerId = (int)dr["ManagerId"];
								alGroup.Add(managerId, User.GetUserName(managerId));
							}
							if (GroupBy == 3 && !alGroup.Keys.Contains((int)dr["ProjectId"]))
							{
								alGroup.Add((int)dr["ProjectId"], dr["ProjectTitle"].ToString());
							}
							if (GroupBy == 4 && !alGroup.Keys.Contains(clientUid))
							{
								alGroup.Add(clientUid, dr["ClientName"].ToString());
							}
							if (GroupBy == 5 && !alGroup.Keys.Contains((int)dr["CategoryId"]))
							{
								alGroup.Add((int)dr["CategoryId"], dr["CategoryName"].ToString());
							}
						}
					}
				}
			}
			#endregion

			if (GroupBy > 1)
			{
				#region Grouping
				DataTable dt_clone = result.Clone();
				string str_grouping = "";

				if (GroupBy == 2)
					str_grouping = "ManagerId";
				else if (GroupBy == 3)
					str_grouping = "ProjectId";
				else if (GroupBy == 4)
					str_grouping = "ClientUid";
				else if (GroupBy == 5)
					str_grouping = "CategoryId";

				foreach (KeyValuePair<object, string> kvp in alGroup)
				{
					string group_name = kvp.Value;
					if (GroupBy == 3 && kvp.Key.ToString() == "0")
						group_name = Common.GetWebResourceString("{IbnFramework.Project:NoProject}");
					if (GroupBy == 4 && kvp.Key.ToString() == Guid.Empty.ToString())
						group_name = Common.GetWebResourceString("{IbnFramework.Project:NoClient}");
					if (GroupBy == 5 && kvp.Key.ToString() == "0")
						group_name = Common.GetWebResourceString("{IbnFramework.Project:NoCategory}");

					dr = dt_clone.NewRow();
					dr["GroupId"] = kvp.Key.ToString();
					dr["GroupName"] = group_name;
					dr["ItemId"] = 0;
					dr["Title"] = "";
					dr["ItemType"] = 0;
					dr["ManagerId"] = 0;
					if (GroupBy == 3)	// By Project
						dr["ProjectId"] = (int)kvp.Key;
					else
						dr["ProjectId"] = 0;
					dr["ProjectTitle"] = "";
					dr["StateId"] = 0;
					dr["CompletionTypeId"] = 0;
					dr["PercentCompleted"] = 0;
					dr["PriorityId"] = 0;
					dr["PriorityName"] = "";
					dr["IsOverdue"] = false;
					dr["IsNewMessage"] = false;
					dt_clone.Rows.Add(dr);

					DataRow[] dr_items = result.Select(str_grouping + "='" + kvp.Key.ToString() + "'");
					foreach (DataRow dr1 in dr_items)
					{
						DataRow _dr = dt_clone.NewRow();
						_dr.ItemArray = (Object[])dr1.ItemArray.Clone();
						_dr["GroupId"] = kvp.Key.ToString();
						_dr["GroupName"] = group_name;
						dt_clone.Rows.Add(_dr);
					}
				}
				#endregion

				return dt_clone;
			}
			else
			{
				return result;
			}
		}
		#endregion

		#region GetGroupedItemsByCategoryDataTable
		/// <summary>
		///	GroupId, GroupName, ItemId, Title, ItemType, ManagerId, ProjectId, StateId, 
		/// StartDate, FinishDate, CompletionTypeId, PercentCompleted, 
		/// PriorityId, PriorityName, IsOverdue, IsNewMessage, ItemCode
		/// </summary>
		/// <returns></returns>
		public static DataTable GetGroupedItemsByCategoryDataTable(int PrincipalId,
			int ManagerId, int ProjectId, bool ShowActive, ArrayList alTypes,
			DateTime dtCompleted, DateTime dtUpcoming)
		{
			DataTable result = new DataTable();
			result.Columns.Add(new DataColumn("GroupId", typeof(int)));
			result.Columns.Add(new DataColumn("GroupName", typeof(string)));
			result.Columns.Add(new DataColumn("ItemId", typeof(int)));
			result.Columns.Add(new DataColumn("Title", typeof(string)));
			result.Columns.Add(new DataColumn("ItemType", typeof(int)));
			result.Columns.Add(new DataColumn("ManagerId", typeof(int)));
			result.Columns.Add(new DataColumn("ProjectId", typeof(int)));
			result.Columns.Add(new DataColumn("StateId", typeof(int)));
			result.Columns.Add(new DataColumn("FinishDate", typeof(DateTime)));
			result.Columns.Add(new DataColumn("StartDate", typeof(DateTime)));
			result.Columns.Add(new DataColumn("CompletionTypeId", typeof(int)));
			result.Columns.Add(new DataColumn("PercentCompleted", typeof(int)));
			result.Columns.Add(new DataColumn("PriorityId", typeof(int)));
			result.Columns.Add(new DataColumn("PriorityName", typeof(string)));
			result.Columns.Add(new DataColumn("IsOverdue", typeof(bool)));
			result.Columns.Add(new DataColumn("IsNewMessage", typeof(bool)));
			result.Columns.Add(new DataColumn("ItemCode", typeof(string)));
			DataRow dr;
			Hashtable htCategories = new Hashtable();

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime UserDate = DBCommon.GetLocalDate(TimeZoneId, DateTime.UtcNow);		// current User's datetime

			dtCompleted = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, dtCompleted);
			dtUpcoming = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, dtUpcoming);

			DataTable dt = DBToDo.GetListToDoAndTasksForManagerViewGroupedByCategory(PrincipalId,
				Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, ManagerId,
				ProjectId, ShowActive, dtCompleted, dtUpcoming);
			foreach (DataRow _dr in dt.Rows)
			{
				if (alTypes.Contains((int)_dr["ItemType"]))
				{
					if (!(bool)_dr["HasRecurrence"])
					{
						dr = result.NewRow();
						dr["GroupId"] = (int)_dr["CategoryId"];
						dr["GroupName"] = _dr["CategoryName"].ToString();
						dr["ItemId"] = (int)_dr["ItemId"];
						dr["Title"] = _dr["Title"].ToString();
						dr["ItemType"] = (int)_dr["ItemType"];
						dr["ManagerId"] = (int)_dr["ManagerId"];
						if (_dr["ProjectId"] != DBNull.Value)
							dr["ProjectId"] = (int)_dr["ProjectId"];
						else
							dr["ProjectId"] = 0;
						dr["StateId"] = (int)_dr["StateId"];
						dr["IsOverdue"] = (bool)_dr["IsOverdue"];
						if (_dr["StartDate"] != DBNull.Value)
							dr["StartDate"] = (DateTime)_dr["StartDate"];
						if (_dr["FinishDate"] != DBNull.Value)
							dr["FinishDate"] = (DateTime)_dr["FinishDate"];
						dr["CompletionTypeId"] = (int)_dr["CompletionTypeId"];
						dr["PercentCompleted"] = (int)_dr["PercentCompleted"];
						dr["PriorityId"] = (int)_dr["PriorityId"];
						dr["PriorityName"] = _dr["PriorityName"].ToString();
						dr["IsNewMessage"] = (bool)_dr["IsNewMessage"];
						dr["ItemCode"] = (string)_dr["ItemCode"];
						result.Rows.Add(dr);

						if (!htCategories.ContainsKey((int)dr["GroupId"]))
							htCategories.Add((int)dr["GroupId"], dr["GroupName"]);
					}
					else	// Recurrence
					{
						int StartTime;
						int EndTime;
						CalendarEntry.Recurrence recurrence;
						using (IDataReader r_reader = DBCommon.GetRecurrence((int)ObjectTypes.CalendarEntry, (int)_dr["ItemId"]))
						{
							r_reader.Read();
							recurrence = new CalendarEntry.Recurrence(
								(byte)r_reader["Pattern"],
								(byte)r_reader["SubPattern"],
								(byte)r_reader["Frequency"],
								(byte)r_reader["Weekdays"],
								(byte)r_reader["DayOfMonth"],
								(byte)r_reader["WeekNumber"],
								(byte)r_reader["MonthNumber"],
								(int)r_reader["EndAfter"],
								(DateTime)_dr["StartDate"],
								(DateTime)_dr["FinishDate"],
								(int)r_reader["TimeZoneId"]);
							StartTime = (int)r_reader["StartTime"];
							EndTime = (int)r_reader["EndTime"];
						}

						// Get new StartDate and FinishDate for recurrence TimeZone (not UserTimeOffset)
						DateTime eventStartDate = DateTime.UtcNow;
						using (IDataReader r_reader = DBEvent.GetEventDates((int)_dr["ItemId"], recurrence.TimeZoneId))
						{
							r_reader.Read();
							recurrence.StartDate = ((DateTime)r_reader["StartDate"]).Date;
							recurrence.FinishDate = ((DateTime)r_reader["FinishDate"]).Date;
							eventStartDate = (DateTime)r_reader["StartDate"];
						}
						eventStartDate = DBCommon.GetUTCDate(recurrence.TimeZoneId, eventStartDate);

						// from_date, to_date - in UTC
						DateTime from_date = DateTime.UtcNow.Date.AddDays(-1);
						DateTime to_date = DateTime.UtcNow.Date.AddDays(2);
						if (dtUpcoming < DateTime.MaxValue.AddDays(-1))
							to_date = dtUpcoming;
						if (dtCompleted > DateTime.MinValue.AddDays(1))
							from_date = dtCompleted;
						ArrayList dates = CalendarEntry.GetRecurDates(from_date, to_date, StartTime, eventStartDate, recurrence);
						foreach (DateTime d in dates)	// Dates in UTC (но в предположении, что событие начинается в 00:00. Поэтому надо еще добавить StartTime)
						{
							DateTime UserDt = DBCommon.GetLocalDate(TimeZoneId, d);	// from UTC to User's time
							DateTime _StartDate = UserDt.AddMinutes(StartTime);
							DateTime _FinishDate = UserDt.AddMinutes(EndTime);

							dr = result.NewRow();

							if (_StartDate > UserDate)
							{
								if (dtUpcoming >= DateTime.MaxValue.AddDays(-1))	// if we don't need upcoming
									continue;
								dr["StateId"] = (int)ObjectStates.Upcoming;
							}
							else if (_FinishDate < UserDate)
							{
								if (dtCompleted <= DateTime.MinValue.AddDays(1))	// if we don't need completed
									continue;
								dr["StateId"] = (int)ObjectStates.Completed;
							}
							else
							{
								if (!ShowActive)
									continue;
								dr["StateId"] = (int)ObjectStates.Active;
							}
							dr["IsOverdue"] = false;

							dr["GroupId"] = (int)_dr["CategoryId"];
							dr["GroupName"] = _dr["CategoryName"].ToString();
							dr["ItemId"] = (int)_dr["ItemId"];
							dr["Title"] = _dr["Title"].ToString();
							dr["ItemType"] = (int)_dr["ItemType"];
							dr["ManagerId"] = (int)_dr["ManagerId"];
							if (_dr["ProjectId"] != DBNull.Value)
								dr["ProjectId"] = (int)_dr["ProjectId"];
							else
								dr["ProjectId"] = 0;

							dr["StartDate"] = _StartDate;
							dr["FinishDate"] = _FinishDate;
							dr["CompletionTypeId"] = (int)_dr["CompletionTypeId"];
							dr["PercentCompleted"] = (int)_dr["PercentCompleted"];
							dr["PriorityId"] = (int)_dr["PriorityId"];
							dr["PriorityName"] = _dr["PriorityName"].ToString();
							dr["IsNewMessage"] = false;
							dr["ItemCode"] = String.Empty;
							result.Rows.Add(dr);

							if (!htCategories.ContainsKey((int)dr["GroupId"]))
								htCategories.Add((int)dr["GroupId"], dr["GroupName"]);
						}
					}

				}
			}

			#region Grouping
			DataTable dt_clone = result.Clone();
			ArrayList alCats = new ArrayList();
			foreach (int Id in htCategories.Keys)
				alCats.Add(Id);
			alCats.Sort();
			foreach (int Id in alCats)
			{
				string group_name = htCategories[Id].ToString();

				dr = dt_clone.NewRow();
				dr["GroupId"] = Id;
				dr["GroupName"] = group_name;
				dr["ItemId"] = 0;
				dr["Title"] = "";
				dr["ItemType"] = 0;
				dr["ManagerId"] = Id;
				dr["ProjectId"] = 0;
				dr["StateId"] = 0;
				dr["CompletionTypeId"] = 0;
				dr["PercentCompleted"] = 0;
				dr["PriorityId"] = 0;
				dr["PriorityName"] = "";
				dr["IsOverdue"] = false;
				dr["IsNewMessage"] = false;
				dr["ItemCode"] = String.Empty;
				dt_clone.Rows.Add(dr);

				DataRow[] dr_items = result.Select("GroupId=" + Id);
				foreach (DataRow dr1 in dr_items)
				{
					DataRow _dr = dt_clone.NewRow();
					_dr.ItemArray = (Object[])dr1.ItemArray.Clone();
					dt_clone.Rows.Add(_dr);
				}
			}
			#endregion

			return dt_clone;
		}
		#endregion

		#region GetListTodoByFilterGroupedByClient
		/// <summary>
		/// ContactUid, OrgUid, IsClient, ClientName, ProjectId, ProjectTitle, 
		/// ToDoId, Title, CreatorId, CreatorName, ManagerId, ManagerName,
		/// CreationDate, PriorityId, PriorityName,
		/// IsCompleted, CanEdit, CanDelete, IsCollapsed
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListTodoByFilterGroupedByClient(int projectId, int managerId,
			int resourceId, int priorityId, string keyword, int genCategoryType,
			PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			return DBToDo.GetListTodoByFilterGroupedByClient(projectId, managerId,
				resourceId, priorityId, keyword,
				Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId,
				Security.CurrentUser.LanguageId, genCategoryType, orgUid, contactUid);
		}
		#endregion

		#region CollapseByClient
		public static void CollapseByClient(PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			DBToDo.CollapseByClient(Security.CurrentUser.UserID, contactUid, orgUid);
		}
		#endregion

		#region ExpandByClient
		public static void ExpandByClient(PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			DBToDo.ExpandByClient(Security.CurrentUser.UserID, contactUid, orgUid);
		}
		#endregion

		#region RecalculateState
		public static void RecalculateState(int todo_id)
		{
			/// OldState, NewState
			int OldStateId = -1;
			int NewStateId = -1;

			using (IDataReader reader = DBToDo.ToDoRecalculateState(todo_id, DateTime.UtcNow))
			{
				if (reader.Read())
				{
					if (reader["OldStateId"] != DBNull.Value)
						OldStateId = (int)reader["OldStateId"];
					NewStateId = (int)reader["NewStateId"];
				}
			}

			if (OldStateId != NewStateId)
			{
				SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_State, todo_id);

				if ((NewStateId == (int)ObjectStates.Completed || NewStateId == (int)ObjectStates.Suspended)
					&& (OldStateId == (int)ObjectStates.Upcoming || OldStateId == (int)ObjectStates.Active || OldStateId == (int)ObjectStates.Overdue))
					DBToDo.UpdateCompletedBy(todo_id, Security.CurrentUser.UserID);
				else if ((OldStateId == (int)ObjectStates.Completed || OldStateId == (int)ObjectStates.Suspended)
					&& (NewStateId == (int)ObjectStates.Upcoming || NewStateId == (int)ObjectStates.Active || NewStateId == (int)ObjectStates.Overdue))
					DBToDo.ResetCompletedBy(todo_id);
				else if (NewStateId == (int)ObjectStates.Active && OldStateId == (int)ObjectStates.Upcoming)
					DBToDo.UpdateActualStart(todo_id);

				// OR: [2007-08-30] Формируем список зависимых ToDo, находящихся в состоянии Upcoming
				if (NewStateId == (int)ObjectStates.Completed)
				{
					ArrayList todoList = new ArrayList();
					using (IDataReader reader = DBToDo.GetListSuccessors(todo_id))
					{
						while (reader.Read())
						{
							if ((int)reader["StateId"] == (int)ObjectStates.Upcoming)
								todoList.Add((int)reader["ToDoId"]);
						}
					}

					foreach (int todoId in todoList)
						RecalculateState(todoId);
				}
			}
		}
		#endregion

		#region GetManagers
		/// <summary>
		/// </summary>
		/// <returns>PrincipalId, FirstName, LastName, FullName, DisplayName</returns>
		public static IDataReader GetManagers()
		{
			return DBToDo.GetManagers();
		}
		#endregion

		#region ActivateTodo
		public static void ActivateTodo(int todo_id)
		{
			if (!CanUpdate(todo_id))
				throw new AccessDeniedException();

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBToDo.UpdateActivation(todo_id);

				RecalculateState(todo_id);
				tran.Commit();
			}
		}
		#endregion

		#region GetProject
		public static int GetProject(int ToDoId)
		{
			return DBToDo.GetProject(ToDoId);
		}
		#endregion

		#region ConfirmReminder(...)
		internal static bool ConfirmReminder(DateTypes dateType, int objectId, bool hasRecurrence)
		{
			int stateId = DBToDo.GetState(objectId);
			return Common.ConfirmReminder(
				hasRecurrence
				, (int)dateType
				, stateId
				, (int)DateTypes.Todo_StartDate
				, (int)DateTypes.Todo_FinishDate
				, (int)ObjectStates.Upcoming
				, (int)ObjectStates.Completed
				, (int)ObjectStates.Suspended
				);
		}
		#endregion

		#region AddTimeSheet
		public static void AddTimeSheet(int todoId, int minutes, DateTime dt)
		{
			string title = GetToDoTitle(todoId);
			int projectId = GetProject(todoId);

			if (minutes > 0)
				Mediachase.IbnNext.TimeTracking.TimeTrackingManager.AddHoursForEntryByObject(todoId, (int)TODO_TYPE, title, projectId, Security.CurrentUser.UserID, TimeTracking.GetWeekStart(dt), TimeTracking.GetDayNum(dt), minutes, true);
		}
		#endregion

		#region DeleteSimple
		/// <summary>
		/// This method should be called only during Task/Incident/Document deleting.
		/// </summary>
		/// <param name="todo_id"></param>
		internal static void DeleteSimple(int todo_id)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				// OZ: File Storage
				BaseIbnContainer container = BaseIbnContainer.Create("FileLibrary", UserRoleHelper.CreateTodoContainerKey(todo_id));
				FileStorage fs = (FileStorage)container.LoadControl("FileStorage");
				fs.DeleteAll();
				//

				// OZ: User Roles
				UserRoleHelper.DeleteTodoRoles(todo_id);

				// Transform Timesheet
				TimeTracking.ResetObjectId((int)TODO_TYPE, todo_id);

				// MetaObject
				MetaObject.Delete(todo_id, "TodoEx");

				// Delete Todo
				DBToDo.Delete(todo_id);

				tran.Commit();
			}
		}
		#endregion

		#region GetListToDoAndTasksAssignedByUserDataTable
		/// <summary>
		///	ItemId, Title, IsCompleted, CompletionTypeId, ReasonId, IsToDo, StateId, 
		///	CreationDate, StartDate, FinishDate, ActualStartDate, ActualFinishDate, 
		/// SortFinishDate, PercentCompleted, PriorityId, ProjectId, ProjectTitle
		/// </summary>
		/// <returns>DataTable</returns>
		public static DataTable GetListToDoAndTasksAssignedByUserDataTable(bool showActive, DateTime fromDate, DateTime toDate)
		{
			int timeZoneId = Security.CurrentUser.TimeZoneId;
			fromDate = DBCommon.GetUTCDate(timeZoneId, fromDate);
			toDate = DBCommon.GetUTCDate(timeZoneId, toDate);

			return DBToDo.GetListToDoAndTasksAssignedByUserDataTable(Security.CurrentUser.UserID, showActive, fromDate, toDate, timeZoneId);
		}
		#endregion

		#region GetListToDoAndTasksAssignedToUserDataTable
		/// <summary>
		///	ItemId, Title, IsCompleted, CompletionTypeId, ReasonId, IsToDo, StateId, ManagerId,
		///	CreationDate, StartDate, FinishDate, ActualStartDate, ActualFinishDate, 
		/// SortFinishDate, PercentCompleted, PriorityId, PriorityName, ProjectId, ProjectTitle
		/// </summary>
		/// <returns>DataTable</returns>
		public static DataTable GetListToDoAndTasksAssignedToUserDataTable(bool showActive, DateTime fromDate, DateTime toDate)
		{
			int timeZoneId = Security.CurrentUser.TimeZoneId;
			fromDate = DBCommon.GetUTCDate(timeZoneId, fromDate);
			toDate = DBCommon.GetUTCDate(timeZoneId, toDate);

			return DBToDo.GetListToDoAndTasksAssignedToUserDataTable(Security.CurrentUser.UserID, showActive, fromDate, toDate, timeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetManager
		public static int GetManager(int todoId)
		{
			int managerId = -1;
			using (IDataReader reader = DBToDo.GetToDo(todoId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (reader.Read())
					managerId = (int)reader["ManagerId"];
			}
			return managerId;
		}
		#endregion

		#region GetListToDoByFilterDataTable
		/// <summary>
		/// ToDoId, Title, [Description], IsCompleted, StartDate, FinishDate, 
		/// CreationDate, CanEdit, CanDelete,
		/// PriorityId, PriorityName, PercentCompleted, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoByFilterDataTable(int projectId,
			int resourceId, string keyword,
			DateTime StartDate, DateTime FinishDate,
			PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			StartDate = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, StartDate);
			FinishDate = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, FinishDate);

			object o_orgUid = null;
			if (orgUid != PrimaryKeyId.Empty)
				o_orgUid = orgUid;
			object o_contactUid = null;
			if (contactUid != PrimaryKeyId.Empty)
				o_contactUid = contactUid;

			return DBToDo.GetListToDoByFilterDataTable(projectId,
				Security.CurrentUser.UserID, Security.CurrentUser.LanguageId,
				resourceId, Security.CurrentUser.TimeZoneId,
				StartDate, FinishDate, true,
				true, true, keyword, o_contactUid, o_orgUid);
		}
		#endregion

		#region GetListPredecessorsDetails
		/// <summary>
		/// Gets the list predecessors details.
		/// </summary>
		/// <param name="todoId">The todo id.</param>
		/// <returns>LinkId, ToDoId, Title, IsCompleted, CompletionTypeId, ReasonId, StartDate, FinishDate, PriorityId, PercentCompleted, StateId</returns>
		public static IDataReader GetListPredecessorsDetails(int todoId)
		{
			return DBToDo.GetListPredecessorsDetails(todoId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListSuccessorsDetails
		/// <summary>
		/// Gets the list successors details.
		/// </summary>
		/// <param name="todoId">The todo id.</param>
		/// <returns>LinkId, ToDoId, Title, CompletionTypeId, ReasonId, StartDate, FinishDate, PriorityId, PercentCompleted, IsCompleted, StateId</returns>
		public static IDataReader GetListSuccessorsDetails(int todoId)
		{
			return DBToDo.GetListSuccessorsDetails(todoId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListVacantPredecessors
		/// <summary>
		/// Gets the list vacant predecessors.
		/// </summary>
		/// <param name="todoId">The todo id.</param>
		/// <returns>ToDoId, Title </returns>
		public static IDataReader GetListVacantPredecessors(int todoId)
		{
			return DBToDo.GetListVacantPredecessors(todoId);
		}
		#endregion

		#region GetListVacantSuccessors
		/// <summary>
		/// Gets the list vacant successors.
		/// </summary>
		/// <param name="todoId">The todo id.</param>
		/// <returns>ToDoId, Title </returns>
		public static IDataReader GetListVacantSuccessors(int todoId)
		{
			return DBToDo.GetListVacantSuccessors(todoId);
		}
		#endregion

		#region CreateToDoLink
		/// <summary>
		/// Creates the predecessor.
		/// </summary>
		/// <param name="fromToDoId">From todo id.</param>
		/// <param name="toToDoId">To todo id.</param>
		/// <returns></returns>
		public static int CreateToDoLink(int fromToDoId, int toToDoId)
		{
			if (!CanUpdate(fromToDoId))
				throw new AccessDeniedException();
			if (!CanUpdate(toToDoId))
				throw new AccessDeniedException();

			int linkId;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				// [O.R.] Создание нового линка не требует пересчётов,
				// т.к. если поручение-последователь ещё не было активировано,
				// то оно тем более не активируется и при наличии линка,
				// а если поручение-последователь уже активно,
				// то мы всё равно не будем переводить его в состояние Upcoming
				linkId = DBToDo.CreateToDoLink(fromToDoId, toToDoId);

				tran.Commit();
			}

			return linkId;
		}
		#endregion

		#region DeleteToDoLink
		/// <summary>
		/// Deletes todo link.
		/// </summary>
		/// <param name="linkId">The link id.</param>
		public static void DeleteToDoLink(int linkId)
		{
			int predId = -1;
			int succId = -1;
			using (IDataReader reader = DBToDo.GetToDoLink(linkId))
			{
				if (reader.Read())
				{
					predId = (int)reader["PredId"];
					succId = (int)reader["SuccId"];
				}
			}

			if (!CanUpdate(predId))
				throw new AccessDeniedException();
			if (!CanUpdate(succId))
				throw new AccessDeniedException();

			using (DbTransaction tran = DbTransaction.Begin())
			{
				int todoId = DBToDo.DeleteToDoLink(linkId);

				RecalculateState(todoId);

				tran.Commit();
			}
		}
		#endregion

		#region GetListManagedToDoDataTable
		/// <summary>
		/// Datatable returns fields
		///  ToDoId, Title, ProjectId, ProjectTitle, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListManagedToDoDataTable(bool getActive, string keyword)
		{
			return DBToDo.GetListManagedToDoDataTable(Security.CurrentUser.UserID, getActive, keyword);
		}
		#endregion
	}
}
