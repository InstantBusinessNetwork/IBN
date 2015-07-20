using System;
using System.Collections;
using System.Data;

using Mediachase.Ibn;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business
{
	public class Task2
	{
		// Private
		private const ObjectTypes OBJECT_TYPE = ObjectTypes.Task;

		#region VerifyCanUpdate()
		private static void VerifyCanUpdate(int taskId)
		{
			if(!Task.CanUpdate(taskId))
				throw new AccessDeniedException();
		}
		#endregion

		#region VerifyCanModifyResources()
		private static void VerifyCanModifyResources(int taskId)
		{
			if(!Task.CanModifyResources(taskId))
				throw new AccessDeniedException();
		}
		#endregion

		#region UpdateListResources()
		private static void UpdateListResources(int objectId, DataTable items, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanModifyResources(objectId);

			int taskCompletionTypeId;
			bool taskMustBeConfirmed;
			bool taskIsCompleted;
			int taskReasonId;
			int taskProjectId;
			int taskPercentCompleted;
			using(IDataReader reader = DBTask.GetTask(objectId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				reader.Read();

				taskCompletionTypeId = (int)reader["CompletionTypeId"];
				taskMustBeConfirmed = (bool)reader["MustBeConfirmed"];
				taskIsCompleted = (bool)reader["IsCompleted"];
				taskReasonId = (int)reader["ReasonId"];
				taskProjectId = (int)reader["ProjectId"];
				taskPercentCompleted = (int)reader["PercentCompleted"];
			}
			int managerId = DBProject.GetProjectManager(taskProjectId);

			ArrayList oldItems = new ArrayList();
			using(IDataReader reader = DBTask.GetListResources(objectId, Security.CurrentUser.TimeZoneId))
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

					DBTask.DeleteResource(objectId, id);

					// OZ: User Role Addon
					UserRoleHelper.DeleteTaskResourceRole(objectId, id);
					if(id!=managerId)
						UserRoleHelper.DeleteTaskManagerRole(objectId, id);
					//
					SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_ResourceList_AssignmentDeleted, objectId, id);
				}

				foreach(DataRow row in items.Rows)
				{
					int id = (int)row["UserId"];
					bool mustBeConfirmed = (bool)row["MustBeConfirmed"];
					bool canManage = (bool)row["CanManage"];
					if(id == managerId)
						canManage = true;

					bool updated = true;
					
					if(add.Contains(id))
					{
						DbTask2.AddResource(objectId, id, mustBeConfirmed, canManage);
						if(User.IsExternal(id))
							DBCommon.AddGate((int)OBJECT_TYPE, objectId, id);
					}
					else
						updated = (0 < DbTask2.UpdateResource(objectId, id, mustBeConfirmed, canManage));

					// OZ: User Role Addon
					if(id!=managerId)
						UserRoleHelper.DeleteTaskManagerRole(objectId, id);
					UserRoleHelper.DeleteTaskResourceRole(objectId, id);

					if(canManage)
					{
						if(id!=managerId)
							UserRoleHelper.AddTaskManagerRole(objectId, id);
					}
					else
						UserRoleHelper.AddTaskResourceRole(objectId, id);
					//
					
					if(updated)
					{
						if(mustBeConfirmed)
							SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_ResourceList_RequestAdded, objectId, id);
						else
							SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_ResourceList_AssignmentAdded, objectId, id);
					}
				}

				if(taskCompletionTypeId == (int)CompletionType.All)
				{
					int overallPercent = Task.RecalculateOverallPercent(objectId);
					if(taskPercentCompleted != overallPercent)
					{
						DBTask.UpdatePercent(objectId, overallPercent);
						SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_Percent, objectId);
					}

					// Если задача была незавершённой, то при удалении людей, процент завершения может
					// увеличиться и достигнуть 100%. Если при этом не требуется подтверждения менеджера,
					// то произойдёт завершение задачи
					if(!taskIsCompleted && !taskMustBeConfirmed && overallPercent == 100)
					{
						DBTask.UpdateCompletion(objectId, true, (int)CompletionReason.CompletedAutomatically);
						Task.CompleteToDo(objectId);
						Task.RecalculateAllStates(taskProjectId);
					}

					DBTask.RecalculateSummaryPercent(objectId);
				}

				tran.Commit();
			}
		}
		#endregion

		// Public
		#region UpdateGeneralInfo()
		public static void UpdateGeneralInfo(int taskId, string title, string description)
		{
			UpdateGeneralInfo(taskId, title, description, true);
		}

		internal static void UpdateGeneralInfo(int taskId, string title, string description, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(taskId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DbTask2.UpdateGeneralInfo(taskId, title, description);
				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_GeneralInfo, taskId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdatePriority()
		public static void UpdatePriority(int taskId, int priorityId)
		{
			UpdatePriority(taskId, priorityId, true);
		}

		internal static void UpdatePriority(int taskId, int priorityId, bool checkAccess)
		{
			if (checkAccess)
				VerifyCanUpdate(taskId);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				if (0 < DbTask2.UpdatePriority(taskId, priorityId))
					SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_Priority, taskId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateResources()
		public static void UpdateResources(int taskId, DataTable resources)
		{
			UpdateListResources(taskId, resources, true);
		}
		#endregion

		// Categories
		#region UpdateCategories()
		public static void UpdateCategories(ListAction action, int taskId, ArrayList categories)
		{
			UpdateCategories(action, taskId, categories, true);
		}

		internal static void UpdateCategories(ListAction action, int taskId, ArrayList categories, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(taskId);

			ArrayList oldCategories = new ArrayList();
			using(IDataReader reader = Task.GetListCategories(taskId))
			{
				Common.LoadCategories(reader, oldCategories);
			}
			Common.UpdateList(action, oldCategories, categories, OBJECT_TYPE, taskId, SystemEventTypes.Task_Updated_GeneralCategories, new UpdateListDelegate(Common.ListUpdate), null);
		}
		#endregion

		#region AddGeneralCategories()
		public static void AddGeneralCategories(int taskId, ArrayList categories)
		{
			UpdateCategories(ListAction.Add, taskId, categories);
		}
		#endregion
		#region RemoveGeneralCategories()
		public static void RemoveGeneralCategories(int taskId, ArrayList categories)
		{
			UpdateCategories(ListAction.Remove, taskId, categories);
		}
		#endregion
		#region SetGeneralCategories()
		public static void SetGeneralCategories(int taskId, ArrayList categories)
		{
			UpdateCategories(ListAction.Set, taskId, categories);
		}
		#endregion

		#region UpdateTimeline()
		public static void UpdateTimeline(int taskId, DateTime startDate, DateTime finishDate)
		{
			string title;
			string description;
			int priorityId;
			bool isMilestone;
			int activationType;
			int completionType;
			bool mustBeConfirmed;
			int phaseId;
			int taskTime;

			using(IDataReader TaskReader = Task.GetTask(taskId))
			{
				if(TaskReader.Read())
				{
					title = (string)TaskReader["Title"];
					description = TaskReader["Description"].GetType() == typeof(DBNull) ? null: (string)TaskReader["Description"];
					isMilestone   = (bool)TaskReader["IsMilestone"];
					priorityId = (int)TaskReader["PriorityId"];
					completionType = (int)TaskReader["CompletionTypeId"];
					activationType = (int)TaskReader["ActivationTypeId"];
					mustBeConfirmed = (bool)TaskReader["MustBeConfirmed"];
					phaseId = DBCommon.NullToInt32(TaskReader["PhaseId"]);
					taskTime = (int)TaskReader["TaskTime"];
				}
				else
					throw new Exception("Task not found");
			}

			ArrayList categories = new ArrayList();
			using(IDataReader reader = DBCommon.GetListCategoriesByObject((int)OBJECT_TYPE, taskId))
			{
				while(reader.Read())
				{
					int categoryId = (int)reader["CategoryId"];
					categories.Add(categoryId);
				}
			}

			Task.Update(
				taskId
				, title
				, description
				, startDate
				, finishDate
				, priorityId
				, isMilestone
				, activationType
				, completionType
				, mustBeConfirmed
				, categories
				, phaseId
				, taskTime
				);
		}
		#endregion

		#region UpdateTaskTime()
		public static void UpdateTaskTime(int taskId, int taskTime)
		{
			UpdateTaskTime(taskId, taskTime, true);
		}

		internal static void UpdateTaskTime(int taskId, int taskTime, bool checkAccess)
		{
			if (checkAccess)
				VerifyCanUpdate(taskId);

			int projectId = Task.GetProject(taskId);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				int retVal = DbTask2.UpdateTaskTime(taskId, taskTime);
				if (retVal > 0)
				{
					// OR: we do not have the appropriate event
					// SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_Timeline, taskId);

					// O.R: Recalculating project TaskTime
					TimeTracking.RecalculateProjectTaskTime(projectId);
				}

				tran.Commit();
			}
		}
		#endregion

		#region UpdateConfigurationInfo()
		public static void UpdateConfigurationInfo(int taskId, int activationTypeId, int completionTypeId, bool mustBeConfirmed)
		{
			UpdateConfigurationInfo(taskId, activationTypeId, completionTypeId, mustBeConfirmed, true);
		}

		internal static void UpdateConfigurationInfo(int taskId, int activationTypeId, int completionTypeId, bool mustBeConfirmed, bool checkAccess)
		{
			if (checkAccess)
				VerifyCanUpdate(taskId);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				if (0 < DbTask2.UpdateConfigurationInfo(taskId, activationTypeId, completionTypeId, mustBeConfirmed))
				{
					SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_ConfigurationInfo, taskId);

					Task.RecalculateAllStates(DBTask.GetProject(taskId));
				}

				tran.Commit();
			}
		}
		#endregion

		#region UpdatePhase()
		public static void UpdatePhase(int taskId, int phaseId)
		{
			UpdatePhase(taskId, phaseId, true);
		}

		internal static void UpdatePhase(int taskId, int phaseId, bool checkAccess)
		{
			if (checkAccess)
				VerifyCanUpdate(taskId);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				if (0 < DbTask2.UpdatePhase(taskId, phaseId))
				{
					// OR: we do not have the appropriate event
					// SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_ConfigurationInfo, taskId);
				}

				tran.Commit();
			}
		}
		#endregion

		#region UpdateSimple
		public static void UpdateSimple(int taskId, string title, string description, int priorityId, int activationTypeId, int completionTypeId, bool mustBeConfirmed, int taskTime, int phaseId, ArrayList categories)
		{
			VerifyCanUpdate(taskId);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				UpdateGeneralInfo(taskId, title, description, false);
				UpdatePriority(taskId, priorityId, false);
				UpdateCategories(ListAction.Set, taskId, categories, false);
				UpdateTaskTime(taskId, taskTime, false);
				UpdateConfigurationInfo(taskId, activationTypeId, completionTypeId, mustBeConfirmed, false);
				UpdatePhase(taskId, phaseId, false);

				tran.Commit();
			}
		}
		#endregion
	}
}
