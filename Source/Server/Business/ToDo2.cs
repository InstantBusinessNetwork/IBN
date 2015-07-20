using System;
using System.Collections;
using System.Data;

using Mediachase.Ibn;
using Mediachase.IBN.Database;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Business
{
	public class ToDo2
	{
		// Private
		private const ObjectTypes OBJECT_TYPE = ObjectTypes.ToDo;
		#region VerifyCanUpdate()
		private static void VerifyCanUpdate(int todoId)
		{
			if(!ToDo.CanUpdate(todoId))
				throw new AccessDeniedException();
		}
		#endregion

		#region UpdateProjectAndManager()
		private static void UpdateProjectAndManager(int todoId, int projectId, int managerId)
		{
			int oldProjectId = DBToDo.GetProject(todoId);
			int oldManagerId = DBToDo.GetManager(todoId);

			if(projectId == 0) // leave old project
			{
				projectId = oldProjectId;
			}

			if (managerId == 0)	// Leave old manager
			{
				managerId = oldManagerId; 
			}

			if (projectId != oldProjectId && projectId > 0 && !ToDo.CanCreate(projectId))
				throw new AccessDeniedException();

			if(projectId != oldProjectId || managerId != oldManagerId)
			{
				using(DbTransaction tran = DbTransaction.Begin())
				{
					DBToDo.UpdateProjectAndManager(todoId, projectId, managerId);

					// OZ: User Role Addon
					if(managerId != oldManagerId)
					{
						UserRoleHelper.DeleteTodoManagerRole(todoId, oldManagerId);
						UserRoleHelper.AddTodoManagerRole(todoId, managerId);
					}

					if(projectId != oldProjectId)
					{
						ForeignContainerKey.Delete(UserRoleHelper.CreateTodoContainerKey(todoId),UserRoleHelper.CreateProjectContainerKey(oldProjectId));
						if(projectId > 0)
							ForeignContainerKey.Add(UserRoleHelper.CreateTodoContainerKey(todoId),UserRoleHelper.CreateProjectContainerKey(projectId));
					}
					// end OZ

					if(projectId != oldProjectId)
					{
						if(projectId > 0)
							SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_TodoList_TodoAdded, projectId, todoId);
						else
							SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_TodoList_TodoDeleted, oldProjectId, todoId);

						// O.R. [2008-07-30]
						if (oldProjectId > 0)
							TimeTracking.RecalculateProjectTaskTime(oldProjectId);
						if (projectId > 0)
							TimeTracking.RecalculateProjectTaskTime(projectId);
					}

					if(managerId != oldManagerId)
					{
						SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_Manager_ManagerDeleted, todoId, oldManagerId);
						SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_Manager_ManagerAdded, todoId, managerId);
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

			int todoCompletionTypeId;
			bool todoManagerConfirmed;
			bool todoIsCompleted;
			int todoReasonId;
			int todoIncidentId = -1;
			int todoDocumentId = -1;
			int todoTaskId = -1;
			int todoPercentCompleted = 0;
			bool CompleteIncident = false;
			bool todoCompleteTask = false;
			bool todoCompleteDocument = false;
			string todoTitle = "";
			using(IDataReader reader = DBToDo.GetToDo(objectId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				reader.Read();

				todoCompletionTypeId = (int)reader["CompletionTypeId"];
				todoManagerConfirmed = (bool)reader["MustBeConfirmed"];
				todoIsCompleted = (bool)reader["IsCompleted"];
				todoReasonId = (int)reader["ReasonId"];
				if(reader["IncidentId"] != DBNull.Value)
					todoIncidentId = (int)reader["IncidentId"];
				if(reader["DocumentId"] != DBNull.Value)
					todoDocumentId = (int)reader["DocumentId"];
				if(reader["TaskId"] != DBNull.Value)
					todoTaskId = (int)reader["TaskId"];
				if (reader["CompleteIncident"] != DBNull.Value)
					CompleteIncident = (bool)reader["CompleteIncident"];
				if(reader["CompleteTask"] != DBNull.Value)
					todoCompleteTask = (bool)reader["CompleteTask"];
				if(reader["CompleteDocument"] != DBNull.Value)
					todoCompleteDocument = (bool)reader["CompleteDocument"];
				todoPercentCompleted = (int)reader["PercentCompleted"];
				todoTitle = reader["Title"].ToString();
			}

			ArrayList oldItems = new ArrayList();
			using(IDataReader reader = DBToDo.GetListResources(objectId, Security.CurrentUser.TimeZoneId))
			{
				Common.LoadItems(reader, "UserId", oldItems);
			}

			ArrayList add = new ArrayList();
			ArrayList del = new ArrayList();
			foreach(DataRow row in items.Rows)
			{
				int id = (int)row["UserId"];
				if(oldItems.Contains(id))
					oldItems.Remove(id);
				else
					add.Add(id);
			}

			del.AddRange(oldItems);

			int cuid = Security.CurrentUser.UserID;

			using(DbTransaction tran = DbTransaction.Begin())
			{
				foreach(int id in del)
				{
					DBCommon.DeleteGate((int)OBJECT_TYPE, objectId, id);

					// O.R. [2009-02-12]
					DBCalendar.DeleteStickedObject((int)OBJECT_TYPE, objectId, id);

					DBToDo.DeleteResource(objectId, id);

					// OZ: User Role Addon
					//if(todoIncidentId!=-1)
					//{
					//    UserRoleHelper.DeleteIssueTodoResourceRole(todoIncidentId, id);
					//}
					//else 
					if(todoDocumentId!=-1)
					{
						UserRoleHelper.DeleteDocumentTodoResourceRole(todoDocumentId, id);
					}
					else if(todoTaskId!=-1)
					{
						UserRoleHelper.DeleteTaskTodoResourceRole(todoTaskId, id);
					}
					else
						UserRoleHelper.DeleteTodoResourceRole(objectId, id);
					//

					SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_ResourceList_AssignmentDeleted, objectId, id);
				}

				foreach(DataRow row in items.Rows)
				{
					int id = (int)row["UserId"];
					bool mustBeConfirmed = (bool)row["MustBeConfirmed"];
					bool updated = true;
					
					if(add.Contains(id))
					{
						DbTodo2.AddResource(objectId, id, mustBeConfirmed);
						if(User.IsExternal(id))
							DBCommon.AddGate((int)OBJECT_TYPE, objectId, id);

						// OZ: User Role Addon
						//if(todoIncidentId!=-1)
						//{
						//    UserRoleHelper.AddIssueTodoResourceRole(todoIncidentId, id);
						//}
						//else 
						if(todoDocumentId!=-1)
						{
							UserRoleHelper.AddDocumentTodoResourceRole(todoDocumentId, id);
						}
						else if(todoTaskId!=-1)
						{
							UserRoleHelper.AddTaskTodoResourceRole(todoTaskId, id);
						}
						else
							UserRoleHelper.AddTodoResourceRole(objectId, id);
						//
					}
					else
						updated = (0 < DbTodo2.UpdateResource(objectId, id, mustBeConfirmed));
					
					if(updated)
					{
						if(mustBeConfirmed)
							SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_ResourceList_RequestAdded, objectId, id);
						else
							SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_ResourceList_AssignmentAdded, objectId, id);
					}
				}

				if(todoCompletionTypeId == (int)CompletionType.All)
				{
					int overallPercent = ToDo.RecalculateOverallPercent(objectId);
					if(todoPercentCompleted != overallPercent)
					{
						DBToDo.UpdatePercent(objectId, overallPercent);
						SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_Percent, objectId);
					}

					// Если поручение было незавершённым, то при удалении людей, процент завершения может
					// увеличиться и достигнуть 100%. Если при этом не требуется подтверждения менеджера,
					// то произойдёт завершение todo
					if(!todoIsCompleted && !todoManagerConfirmed && overallPercent == 100)
					{
						DBToDo.UpdateCompletion(objectId, true, (int)CompletionReason.CompletedAutomatically);

						if(todoTaskId > 0 && todoCompleteTask)
							ToDo.UpdateTaskCompletion(todoTaskId);

						if(todoDocumentId > 0 && todoCompleteDocument)
							ToDo.UpdateDocumentCompletion(todoDocumentId);

						if (todoIncidentId > 0 && CompleteIncident)
							ToDo.CompleteIncidentIfNeed(todoIncidentId);

						ToDo.RecalculateState(objectId);
					}
				}

				tran.Commit();
			}
		}
		#endregion

		// Public
		#region UpdateGeneralInfo()
		public static void UpdateGeneralInfo(int todoId, string title, string description)
		{
			UpdateGeneralInfo(todoId, title, description, true);
		}

		internal static void UpdateGeneralInfo(int todoId, string title, string description, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(todoId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DbTodo2.UpdateGeneralInfo(todoId, title, description);
				SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_GeneralInfo, todoId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateTimeline()
		public static void UpdateTimeline(int todoId, DateTime startDate, DateTime finishDate, int taskTime)
		{
			UpdateTimeline(todoId, startDate, finishDate, taskTime, true);
		}

		internal static void UpdateTimeline(int todoId, DateTime startDate, DateTime finishDate, int taskTime, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(todoId);

			object oStartDate = null;
			object oFinishDate = null;
			DateTime MinValue1 = DateTime.MinValue.AddDays(1);
			int TimeZoneId = Security.CurrentUser.TimeZoneId;

			if (startDate > MinValue1 && finishDate > MinValue1 && startDate > finishDate)
				throw new Exception("wrong dates");

			if (startDate > MinValue1)
				oStartDate = DBCommon.GetUTCDate(TimeZoneId, startDate);
			if (finishDate > MinValue1)
				oFinishDate = DBCommon.GetUTCDate(TimeZoneId, finishDate);

			int projectId = ToDo.GetProject(todoId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				int retVal = DbTodo2.UpdateTimeline(todoId, oStartDate, oFinishDate, taskTime);
				if(retVal > 0)
				{
					SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_Timeline, todoId);

					// O.R: Scheduling
					if (retVal == 1 || retVal == 3)	// StartDate Changed
					{
						if (oStartDate != null)
							Schedule.UpdateDateTypeValue(DateTypes.Todo_StartDate, todoId, (DateTime)oStartDate);
						else
							Schedule.DeleteDateTypeValue(DateTypes.Todo_StartDate, todoId);
					}
					if (retVal == 2 || retVal == 3)	// FinishDate Changed
					{
						if (oFinishDate != null)
							Schedule.UpdateDateTypeValue(DateTypes.Todo_FinishDate, todoId, (DateTime)oFinishDate);
						else
							Schedule.DeleteDateTypeValue(DateTypes.Todo_FinishDate, todoId);
					}

					// O.R: Recalculating project TaskTime
					if (projectId > 0)
						TimeTracking.RecalculateProjectTaskTime(projectId);

					ToDo.RecalculateState(todoId);
				}

				tran.Commit();
			}
		}
		#endregion

		#region UpdatePriority()
		public static void UpdatePriority(int todoId, int priorityId)
		{
			UpdatePriority(todoId, priorityId, true);
		}

		internal static void UpdatePriority(int todoId, int priorityId, bool checkAccess)
		{
			if(checkAccess && !ToDo.CanUpdate(todoId))
				throw new AccessDeniedException();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if(0 < DbTodo2.UpdatePriority(todoId, priorityId))
					SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_Priority, todoId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateManager()
		public static void UpdateManager(int todoId, int userId)
		{
			UpdateManager(todoId, userId, true);
		}

		internal static void UpdateManager(int todoId, int userId, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(todoId);

			UpdateProjectAndManager(todoId, 0, userId);
		}
		#endregion

		#region UpdateProject()
		public static void UpdateProject(int todoId, int projectId)
		{
			UpdateProject(todoId, projectId, true);
		}

		internal static void UpdateProject(int todoId, int projectId, bool checkAccess)
		{
			if(checkAccess && !ToDo.CanChangeProject(todoId))
				throw new AccessDeniedException();

			UpdateProjectAndManager(todoId, projectId, 0);
		}
		#endregion

		#region UpdateResources()
		public static void UpdateResources(int todoId, DataTable resources)
		{
			UpdateListResources(todoId, resources, true);
		}
		#endregion

		#region UpdateConfigurationInfo()
		public static void UpdateConfigurationInfo(int todoId, int activationTypeId, int completionTypeId, bool mustBeConfirmed)
		{
			UpdateConfigurationInfo(todoId, activationTypeId, completionTypeId, mustBeConfirmed, true);
		}

		internal static void UpdateConfigurationInfo(int todoId, int activationTypeId, int completionTypeId, bool mustBeConfirmed, bool checkAccess)
		{
			if(checkAccess && !ToDo.CanUpdate(todoId))
				throw new AccessDeniedException();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if(0 < DbTodo2.UpdateConfigurationInfo(todoId, activationTypeId, completionTypeId, mustBeConfirmed))
					SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_ConfigurationInfo, todoId);

				tran.Commit();
			}
		}
		#endregion

		#region AcceptResource
		public static void AcceptResource(int todoId)
		{
			int userId = Security.CurrentUser.UserID;

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DbTodo2.ResourceReply(todoId, userId, true);
				
				SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_ResourceList_RequestAccepted, todoId, userId);

				tran.Commit();
			}
		}
		#endregion

		#region DeclineResource
		public static void DeclineResource(int todoId)
		{
			UserLight cu = Security.CurrentUser;
			int UserId = cu.UserID;
			DateTime utc_now = DateTime.UtcNow;

			int CompletionTypeId;
			bool IsManagerConfirmed;
			bool IsCompleted;
			int ReasonId;
			int IncidentId = -1;
			int DocumentId = -1;
			int TaskId = -1;
			bool CompleteIncident = false;
			bool CompleteTask = false;
			bool CompleteDocument = false;
			int oldPercentCompleted = 0;
			string title = "";
			using(IDataReader reader = DBToDo.GetToDo(todoId, cu.TimeZoneId, cu.LanguageId))
			{
				reader.Read();
				CompletionTypeId = (int)reader["CompletionTypeId"];
				IsManagerConfirmed = (bool)reader["MustBeConfirmed"];
				IsCompleted = (bool)reader["IsCompleted"];
				ReasonId = (int)reader["ReasonId"];
				if (reader["IncidentId"] != DBNull.Value)
					IncidentId = (int)reader["IncidentId"];
				if (reader["DocumentId"] != DBNull.Value)
					DocumentId = (int)reader["DocumentId"];
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

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DbTodo2.ResourceReply(todoId, UserId, false);

				// O.R. [2009-02-12]
				DBCalendar.DeleteStickedObject((int)OBJECT_TYPE, todoId, UserId);

				SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_ResourceList_RequestDenied, todoId, UserId);

				if (CompletionTypeId == (int)CompletionType.All)
				{
					int OverallPercent = ToDo.RecalculateOverallPercent(todoId);	

					if (oldPercentCompleted != OverallPercent)
					{
						DBToDo.UpdatePercent(todoId, OverallPercent);
						SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_Percent, todoId);
					}

					if (!IsCompleted && !IsManagerConfirmed && OverallPercent == 100)
					{
						DBToDo.UpdateCompletion(todoId, true, (int)CompletionReason.CompletedAutomatically);

						if (TaskId > 0 && CompleteTask)
							ToDo.UpdateTaskCompletion(TaskId);

						if (DocumentId > 0 && CompleteDocument)
							ToDo.UpdateDocumentCompletion(DocumentId);

						if (IncidentId > 0 && CompleteIncident)
							ToDo.CompleteIncidentIfNeed(IncidentId);

						ToDo.RecalculateState(todoId);
					}
				}

				tran.Commit();
			}
		}
		#endregion

		// Categories
		#region UpdateCategories()
		public static void UpdateCategories(ListAction action, int todoId, ArrayList categories)
		{
			UpdateCategories(action, todoId, categories, true);
		}

		internal static void UpdateCategories(ListAction action, int todoId, ArrayList categories, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(todoId);

			ArrayList oldCategories = new ArrayList();
			using(IDataReader reader = ToDo.GetListCategories(todoId))
			{
				Common.LoadCategories(reader, oldCategories);
			}
			Common.UpdateList(action, oldCategories, categories, OBJECT_TYPE, todoId, SystemEventTypes.Todo_Updated_GeneralCategories, new UpdateListDelegate(Common.ListUpdate), null);
		}
		#endregion

		#region AddGeneralCategories()
		public static void AddGeneralCategories(int todoId, ArrayList categories)
		{
			UpdateCategories(ListAction.Add, todoId, categories);
		}
		#endregion
		#region RemoveGeneralCategories()
		public static void RemoveGeneralCategories(int todoId, ArrayList categories)
		{
			UpdateCategories(ListAction.Remove, todoId, categories);
		}
		#endregion
		#region SetGeneralCategories()
		public static void SetGeneralCategories(int todoId, ArrayList categories)
		{
			UpdateCategories(ListAction.Set, todoId, categories);
		}
		#endregion

		// Batch
		#region UpdateWithProject()
		public static void UpdateWithProject(int todoId, 
			int projectId,
			int managerId,
			string title, 
			string description,	
			DateTime startDate,
			DateTime finishDate,
			int priorityId, 
			int activationTypeId,
			int completionTypeId,
			bool mustBeConfirmed,
			int taskTime,
			ArrayList categories,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			VerifyCanUpdate(todoId);

			int oldProjectId = DBToDo.GetProject(todoId);
			if (oldProjectId != projectId && !ToDo.CanChangeProject(todoId))
				throw new AccessDeniedException();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				UpdateGeneralInfo(todoId, title, description, false);
				UpdateProjectAndManager(todoId, projectId, managerId);
				UpdateTimeline(todoId, startDate, finishDate, taskTime);
				UpdatePriority(todoId, priorityId, false);
				UpdateCategories(ListAction.Set, todoId, categories, false);
				UpdateConfigurationInfo(todoId, activationTypeId, completionTypeId, mustBeConfirmed, false);
				UpdateClient(todoId, contactUid, orgUid, false);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateIssueToDo()
		public static void UpdateIssueToDo(int todoId, 
			int issueId,
			int managerId,
			string title, 
			string description,	
			DateTime startDate,
			DateTime finishDate,
			int priorityId, 
			int activationTypeId,
			int completionTypeId,
			bool mustBeConfirmed,
			bool completeIssue,
			int taskTime,
			ArrayList categories,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			if(!ToDo.CanUpdateIncidentTodo(todoId, issueId))
				throw new AccessDeniedException();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				UpdateGeneralInfo(todoId, title, description, false);
				UpdateTimeline(todoId, startDate, finishDate, taskTime);
				UpdatePriority(todoId, priorityId, false);
				UpdateCategories(ListAction.Set, todoId, categories, false);
				UpdateConfigurationInfo(todoId, activationTypeId, completionTypeId, mustBeConfirmed, false);
				UpdateClient(todoId, contactUid, orgUid, false);
				UpdateManager(todoId, managerId, false);

				int issueState = completeIssue ? (int)ObjectStates.Completed : -1;
				DBToDo.AddIncidentToDo(issueId, todoId, issueState);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateTaskToDo()
		public static void UpdateTaskToDo(int todoId, 
			int taskId,
			int managerId,
			string title, 
			string description,	
			DateTime startDate,
			DateTime finishDate,
			int priorityId, 
			int activationTypeId,
			int completionTypeId,
			bool mustBeConfirmed,
			bool completeTask,
			int taskTime,
			ArrayList categories,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			if(!ToDo.CanUpdate(todoId, taskId))
				throw new AccessDeniedException();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				UpdateGeneralInfo(todoId, title, description, false);
				UpdateTimeline(todoId, startDate, finishDate, taskTime, false);
				UpdatePriority(todoId, priorityId, false);
				UpdateCategories(ListAction.Set, todoId, categories, false);
				UpdateConfigurationInfo(todoId, activationTypeId, completionTypeId, mustBeConfirmed, false);
				UpdateClient(todoId, contactUid, orgUid, false);
				UpdateManager(todoId, managerId, false);

				DBToDo.AddTaskToDo(taskId, todoId, completeTask);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateDocumentToDo()
		public static void UpdateDocumentToDo(int todoId, 
			int documentId,
			int managerId,
			string title, 
			string description,	
			DateTime startDate,
			DateTime finishDate,
			int priorityId, 
			int activationTypeId,
			int completionTypeId,
			bool mustBeConfirmed,
			bool completeDocument,
			int taskTime,
			ArrayList categories,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid)
		{
			if(!ToDo.CanUpdateDocumentTodo(todoId, documentId))
				throw new AccessDeniedException();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				UpdateGeneralInfo(todoId, title, description, false);
				UpdateTimeline(todoId, startDate, finishDate, taskTime);
				UpdatePriority(todoId, priorityId, false);
				UpdateCategories(ListAction.Set, todoId, categories, false);
				UpdateConfigurationInfo(todoId, activationTypeId, completionTypeId, mustBeConfirmed, false);
				UpdateClient(todoId, contactUid, orgUid, false);
				UpdateManager(todoId, managerId, false);

				DBToDo.AddDocumentToDo(documentId, todoId, completeDocument);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateClient()
		public static void UpdateClient(int todoId, PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			UpdateClient(todoId, contactUid, orgUid, true);
		}

		internal static void UpdateClient(int todoId, PrimaryKeyId contactUid, PrimaryKeyId orgUid, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(todoId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if (0 < DbTodo2.UpdateClient(todoId, contactUid == PrimaryKeyId.Empty ? null : (object)contactUid, orgUid == PrimaryKeyId.Empty ? null : (object)orgUid))
				{
					// TODO: 
					//SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_Client, projectId);
				}

				tran.Commit();
			}
		}
		#endregion
	}
}
