using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

using Mediachase.Ibn;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Database;
using Mediachase.IBN.Database.ControlSystem;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using ProjectBusinessUtil.Assignment;
using ProjectBusinessUtil.Assignment.Contour;
using ProjectBusinessUtil.Calendar;
using ProjectBusinessUtil.Common;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// 
	/// </summary>
	/// 
	public class Task
	{
		#region TASK_TYPE
		private static ObjectTypes TASK_TYPE
		{
			get { return ObjectTypes.Task; }
		}
		#endregion

		public const int MaxTaskTime = 59880; // 998 hours
		public Task()
		{
		}

		//TODO: testing
		//TODO: Lock Last
		#region Create
		public static int Create(int project_id,
			string title,
			string description,
			DateTime StartDate,
			DateTime FinishDate,
			int priority_id,
			bool is_milestone,
			int activation_type,
			int completion_type,
			bool must_be_confirmed_by_manager,
			ArrayList categories)
		{
			return Create(project_id, title, description, StartDate, FinishDate, priority_id, is_milestone, activation_type, completion_type, must_be_confirmed_by_manager, categories, null, null, null, false, -1, -1);
		}

		public static int CreateUseUniversalTime(int project_id,
			string title,
			string description,
			DateTime StartDate,
			DateTime FinishDate,
			int priority_id,
			bool is_milestone,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			ArrayList categories,
			ArrayList resourceList)
		{
			if (!CanCreate(project_id))
				throw new AccessDeniedException();

			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (StartDate > FinishDate)
				throw new Exception("wrong dates");

			DateTime NewStartDate = StartDate;
			DateTime NewFinishDate = FinishDate;

			DateTime CurrentDate = DateTime.UtcNow;

			DateTime ProjectTargetStartDate;
			DateTime ProjectTargetFinishDate;
			int CalendarId;
			int ManagerId = -1;

			#region get project
			using (IDataReader ProjectReader = DBProject.GetProject(project_id, User.DefaultTimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (ProjectReader.Read())
				{
					ProjectTargetStartDate = (DateTime)ProjectReader["TargetStartDate"];
					ProjectTargetFinishDate = (DateTime)ProjectReader["TargetFinishDate"];
					CalendarId = (int)ProjectReader["CalendarId"];
					ManagerId = (int)ProjectReader["ManagerId"];
				}
				else
					throw new Exception("Project not found");
			}
			#endregion


			#region define dates
			//Вычисление длительностей с учетом начала проекта и календаря
			int NewDuration;
			int DirtyDuration;

			DirtyDuration = (int)((TimeSpan)(NewFinishDate - NewStartDate)).TotalMinutes;

			ProjectTargetStartDate = DBCalendar.GetFinishDateByDuration(CalendarId, ProjectTargetStartDate, 0);
			NewStartDate = DBCalendar.GetFinishDateByDuration(CalendarId, NewStartDate, 0);

			NewStartDate = MaxDate(ProjectTargetStartDate, NewStartDate);

			NewFinishDate = DBCalendar.GetStartDateByDuration(CalendarId, NewFinishDate, 0);

			if (NewStartDate > NewFinishDate)
				NewDuration = 0;
			else
				NewDuration = DBCalendar.GetDurationByFinishDate(CalendarId, NewStartDate, NewFinishDate);

			if (NewDuration == 0)
			{
				NewDuration = DBCalendar.GetDurationByFinishDate(CalendarId, NewStartDate, NewStartDate.AddMinutes(DirtyDuration));
				NewFinishDate = DBCalendar.GetFinishDateByDuration(CalendarId, NewStartDate, NewDuration);
			}

			if (NewDuration == 0)
				is_milestone = true;

			if (is_milestone)
			{
				NewFinishDate = NewStartDate;
				NewDuration = 0;
			}

			#endregion

			#region create

			int UserId = Security.CurrentUser.UserID;
			//создаем задачу
			int TaskId = -1;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				TaskId = DBTask.Create(project_id, UserId, title, description, CurrentDate, NewStartDate, NewFinishDate,
					NewDuration, priority_id, is_milestone, (int)ConstraintTypes.StartNotEarlierThan, NewStartDate,
					activation_type, completion_type, must_be_confirmed, -1, NewDuration);

				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Created, TaskId);
				SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_TaskList_TaskAdded, project_id, TaskId);

				// OZ: User Role Addon
				UserRoleHelper.AddTaskManagerRole(TaskId, ManagerId);
				if (ManagerId != UserId)
					UserRoleHelper.AddTaskCreatorRole(TaskId, UserId);
				ForeignContainerKey.Add(UserRoleHelper.CreateTaskContainerKey(TaskId), UserRoleHelper.CreateProjectContainerKey(project_id));
				//

				// Categories
				foreach (int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)TASK_TYPE, TaskId, CategoryId);
				}

				//Big E [9/02/06] - don't use correction in this place
				/*
				//correction of project dates
				DateTime RangeFinishDate;
				using(IDataReader RangeReader = DBProject.GetProjectDateRange(project_id))
				{
					if(RangeReader.Read())
						RangeFinishDate = (DateTime)RangeReader["FinishDate"];
					else
						throw new Exception("project not found");
				}
					
				if(RangeFinishDate != ProjectFinishDate)
					DBProject.UpdateDates(project_id,ProjectStartDate,RangeFinishDate);
				*/
				// O.R: Scheduling
				Schedule.UpdateDateTypeValue(DateTypes.Task_StartDate, TaskId, NewStartDate);
				Schedule.UpdateDateTypeValue(DateTypes.Task_FinishDate, TaskId, NewFinishDate);

				// Assign Resource [2/2/2004]
				if (resourceList != null)
				{
					ArrayList resAdd = new ArrayList();
					ArrayList resReq = new ArrayList();

					foreach (int User_Id in resourceList)
					{
						int RealPrincipalId = User_Id;

						bool bMustBeConfirmed = false;

						if (RealPrincipalId < 0)
						{
							RealPrincipalId *= -1;
							bMustBeConfirmed = true;
						}

						if (bMustBeConfirmed)
							resReq.Add(RealPrincipalId);
						else
							resAdd.Add(RealPrincipalId);

						bool CanManage = false;
						if (RealPrincipalId == ManagerId)
							CanManage = true;
						DBTask.AddResource(TaskId, RealPrincipalId, bMustBeConfirmed, CanManage);

						if (User.IsExternal(User_Id))
							DBCommon.AddGate((int)TASK_TYPE, TaskId, RealPrincipalId);

						if (CanManage)
						{
							if (RealPrincipalId != ManagerId)
								UserRoleHelper.AddTaskManagerRole(TaskId, RealPrincipalId);
						}
						else
							UserRoleHelper.AddTaskResourceRole(TaskId, RealPrincipalId);
					}

					// O.R [2/2/2006]
					/*
					if(resAdd.Count > 0)
						SendAlert(AlertEventType.Task_ResourceAdded, TaskId, (int[])resAdd.ToArray(typeof(int)));
					if(resReq.Count > 0)
						SendAlert(AlertEventType.Task_ResourceRequest, TaskId, (int[])resReq.ToArray(typeof(int)));
						*/

					RecalculateAllStates(project_id);
				}

				tran.Commit();
			}
			#endregion
			return TaskId;
		}

		public static int Create(int project_id,
			string title,
			string description,
			DateTime StartDate,
			DateTime FinishDate,
			int priority_id,
			bool is_milestone,
			int activation_type,
			int completion_type,
			bool must_be_confirmed_by_manager,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream)
		{
			return Create(project_id, title, description, StartDate, FinishDate, priority_id, is_milestone, activation_type, completion_type, must_be_confirmed_by_manager, categories, FileName, _inputStream, null, false, -1, -1);
		}

		public static int Create(int project_id,
			string title,
			string description,
			DateTime StartDate,
			DateTime FinishDate,
			int priority_id,
			bool is_milestone,
			int activation_type,
			int completion_type,
			bool must_be_confirmed_by_manager,
			ArrayList categories,
			ArrayList resourceList,
			bool must_be_confirmed_by_resource,
			int phase_id,
			int task_time)
		{
			return Create(project_id, title, description, StartDate, FinishDate, priority_id, is_milestone, activation_type, completion_type, must_be_confirmed_by_manager, categories, null, null, resourceList, must_be_confirmed_by_resource, phase_id, task_time);
		}

		public static int Create(int project_id,
			string title,
			string description,
			DateTime StartDate,
			DateTime FinishDate,
			int priority_id,
			bool is_milestone,
			int activation_type,
			int completion_type,
			bool must_be_confirmed_by_manager,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			ArrayList resourceList,
			bool must_be_confirmed_by_user,
			int phase_id,
			int task_time)
		{
			if (!CanCreate(project_id))
				throw new AccessDeniedException();

			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (StartDate > FinishDate)
				throw new Exception("wrong dates");

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime NewStartDate = DBCommon.GetUTCDate(TimeZoneId, StartDate);
			DateTime NewFinishDate = DBCommon.GetUTCDate(TimeZoneId, FinishDate);

			DateTime CurrentDate = DateTime.UtcNow;

			DateTime ProjectTargetStartDate;
			DateTime ProjectTargetFinishDate;
			int CalendarId;
			int ManagerId = -1;

			#region get project
			using (IDataReader ProjectReader = DBProject.GetProject(project_id, User.DefaultTimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (ProjectReader.Read())
				{
					ProjectTargetStartDate = (DateTime)ProjectReader["TargetStartDate"];
					ProjectTargetFinishDate = (DateTime)ProjectReader["TargetFinishDate"];
					CalendarId = (int)ProjectReader["CalendarId"];
					ManagerId = (int)ProjectReader["ManagerId"];
				}
				else
					throw new Exception("Project not found");
			}
			#endregion


			#region define dates
			//Вычисление длительностей с учетом начала проекта и календаря
			int NewDuration;
			int DirtyDuration;

			DirtyDuration = (int)((TimeSpan)(NewFinishDate - NewStartDate)).TotalMinutes;

			ProjectTargetStartDate = DBCalendar.GetFinishDateByDuration(CalendarId, ProjectTargetStartDate, 0);
			NewStartDate = DBCalendar.GetFinishDateByDuration(CalendarId, NewStartDate, 0);
			NewStartDate = MaxDate(ProjectTargetStartDate, NewStartDate);
			NewFinishDate = DBCalendar.GetStartDateByDuration(CalendarId, NewFinishDate, 0);

			if (NewStartDate > NewFinishDate)
				NewDuration = 0;
			else
				NewDuration = DBCalendar.GetDurationByFinishDate(CalendarId, NewStartDate, NewFinishDate);

			if (NewDuration == 0)
			{
				NewDuration = DBCalendar.GetDurationByFinishDate(CalendarId, NewStartDate, NewStartDate.AddMinutes(DirtyDuration));
				NewFinishDate = DBCalendar.GetFinishDateByDuration(CalendarId, NewStartDate, NewDuration);
			}

			if (NewDuration == 0)
				is_milestone = true;

			if (is_milestone)
			{
				NewFinishDate = NewStartDate;
				NewDuration = 0;
			}
			else
			{
				phase_id = -1;
			}

			if (task_time < 0)
				task_time = NewDuration;

			#endregion

			#region create

			int UserId = Security.CurrentUser.UserID;
			//создаем задачу
			int TaskId = -1;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				TaskId = DBTask.Create(project_id, UserId, title, description, CurrentDate, NewStartDate, NewFinishDate,
					NewDuration, priority_id, is_milestone, (int)ConstraintTypes.StartNotEarlierThan, NewStartDate,
					activation_type, completion_type, must_be_confirmed_by_manager, phase_id, task_time);

				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Created, TaskId);
				SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_TaskList_TaskAdded, project_id, TaskId);

				// OZ: User Role Addon
				UserRoleHelper.AddTaskManagerRole(TaskId, ManagerId);
				if (ManagerId != UserId)
					UserRoleHelper.AddTaskCreatorRole(TaskId, UserId);
				ForeignContainerKey.Add(UserRoleHelper.CreateTaskContainerKey(TaskId), UserRoleHelper.CreateProjectContainerKey(project_id));
				//

				// Categories
				foreach (int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)TASK_TYPE, TaskId, CategoryId);
				}

				//Big E [9/02/06] - don't use correction in this place
				/*
				//correction of project dates
				DateTime RangeFinishDate;
				using(IDataReader RangeReader = DBProject.GetProjectDateRange(project_id))
				{
					if(RangeReader.Read())
						RangeFinishDate = (DateTime)RangeReader["FinishDate"];
					else
						throw new Exception("project not found");
				}
					
				if(RangeFinishDate != ProjectFinishDate)
					DBProject.UpdateDates(project_id,ProjectStartDate,RangeFinishDate);
				*/

				// O.R: Scheduling
				Schedule.UpdateDateTypeValue(DateTypes.Task_StartDate, TaskId, NewStartDate);
				Schedule.UpdateDateTypeValue(DateTypes.Task_FinishDate, TaskId, NewFinishDate);

				// Assign Resource [2/2/2004]
				if (resourceList != null)
				{
					foreach (int User_Id in resourceList)
					{
						bool CanManage = false;
						if (User_Id == ManagerId)
							CanManage = true;

						DBTask.AddResource(TaskId, User_Id, must_be_confirmed_by_user, CanManage);

						// OZ: User Role Addon
						if (CanManage)
						{
							if (User_Id != ManagerId)
								UserRoleHelper.AddTaskManagerRole(TaskId, User_Id);
						}
						else
							UserRoleHelper.AddTaskResourceRole(TaskId, User_Id);
						//

						if (User.IsExternal(User_Id))
							DBCommon.AddGate((int)TASK_TYPE, TaskId, User_Id);

						if (must_be_confirmed_by_user)
							SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_ResourceList_RequestAdded, TaskId, User_Id);
						else
							SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_ResourceList_AssignmentAdded, TaskId, User_Id);
					}
				}

				// Al: FileStorage
				if (FileName != null)
				{
					string ContainerName = "FileLibrary";
					string ContainerKey = UserRoleHelper.CreateTaskContainerKey(TaskId);

					BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
					FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
					fs.SaveFile(fs.Root.Id, FileName, _inputStream);
				}

				// O.R: Recalculating project TaskTime
				if (task_time > 0)
					TimeTracking.RecalculateProjectTaskTime(project_id);

				RecalculateAllStates(project_id);

				tran.Commit();
			}
			#endregion
			return TaskId;
		}

		#endregion

		//TODO: Implement
		#region Delete

		public static void Delete(int task_id)
		{
			if (!CanDelete(task_id))
				throw new AccessDeniedException();

			ArrayList allRemovedToDo = new ArrayList();
			using (IDataReader todoReader = GetListToDo(task_id))
			{
				while (todoReader.Read())
				{
					int ToDoId = (int)todoReader["ToDoId"];
					allRemovedToDo.Add(ToDoId);
				}
			}

			ArrayList Tasks = new ArrayList();
			using (IDataReader reader = DBTask.GetListAllChild(task_id))
			{
				while (reader.Read())
					Tasks.Add((int)reader["TaskId"]);
			}

			int project_id = GetProject(task_id);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				// OZ: File Storage
				BaseIbnContainer container = BaseIbnContainer.Create("FileLibrary", UserRoleHelper.CreateTaskContainerKey(task_id));
				FileStorage fs = (FileStorage)container.LoadControl("FileStorage");
				fs.DeleteAll();
				//

				// OZ: User Roles
				UserRoleHelper.DeleteTaskRoles(task_id);

				// Delete Todo and don't send todo alerts
				foreach (int ToDoId in allRemovedToDo)
				{
					ToDo.DeleteSimple(ToDoId);
				}


				// 2. Transform Timesheet [2004-02-06]
				TimeTracking.ResetObjectId((int)TASK_TYPE, task_id);
				foreach (int TaskId in Tasks)
					TimeTracking.ResetObjectId((int)TASK_TYPE, TaskId);

				string update_id = Guid.NewGuid().ToString();
				int parentId = DBTask.GetParent(task_id);
				if (parentId != -1)
					DBTask.TaskUpdateAddPossibleTask(update_id, parentId, true);

				MetaObject.Delete(task_id, "TaskEx");

				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Deleted, task_id);
				SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_TaskList_TaskDeleted, project_id, task_id);

				DBTask.Delete(task_id, update_id);

				Process(update_id);

				if (parentId != -1)
				{
					bool ParentIsSummary = true;
					bool ParentWasCompleted = false;
					int ParentCompletionTypeId = (int)CompletionType.All;
					using (IDataReader reader = DBTask.GetTask(parentId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
					{
						if (reader.Read())
						{
							ParentCompletionTypeId = (int)reader["CompletionTypeId"];
							ParentIsSummary = (bool)reader["IsSummary"];
							ParentWasCompleted = (bool)reader["IsCompleted"];
						}
					}

					// если parent task перестал быть summary и тип завершения у него - all, то 
					// пресчитаем его процент на основании среднего значения % завершения ресурсов
					if (!ParentIsSummary && ParentCompletionTypeId == (int)CompletionType.All)
					{
						int OverallPercent = RecalculateOverallPercent(parentId);
						DBTask.UpdatePercent(parentId, OverallPercent);

						if (!ParentWasCompleted && OverallPercent == 100)
							DBTask.UpdateCompletion(parentId, true, (int)CompletionReason.CompletedAutomatically);
					}

					// если parent перестал быть summary и если у него CompletionType = any, то 
					// оставляем его процент без изменений

					DBTask.RecalculateSummaryPercent(parentId);
				}

				// O.R: Recalculating project TaskTime
				TimeTracking.RecalculateProjectTaskTime(project_id);

				DBTask.RecalculateSummaryPercent(task_id);

				RecalculateAllStates(project_id);

				tran.Commit();
			}

			//TODO: пересчитать проект
		}
		#endregion
		//TODO: testing
		//TODO; Lock

		#region Insert

		public static int Insert(int project_id,
			int befor_task_id,
			string title,
			string description,
			DateTime StartDate,
			DateTime FinishDate,
			int priority_id,
			bool is_milestone,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			int phase_id,
			int task_time)
		{
			return Insert(project_id, befor_task_id, title, description, StartDate, FinishDate, priority_id, is_milestone, activation_type, completion_type, must_be_confirmed, categories, FileName, _inputStream, null, false, phase_id, task_time);
		}

		public static int Insert(int project_id,
			int befor_task_id,
			string title,
			string description,
			DateTime StartDate,
			DateTime FinishDate,
			int priority_id,
			bool is_milestone,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			ArrayList resourceList,
			bool must_be_confirmed_by_user,
			int phase_id,
			int task_time)
		{
			if (!CanCreate(project_id))
				throw new AccessDeniedException();

			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (StartDate > FinishDate)
				throw new Exception("wrong dates");

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime NewStartDate = DBCommon.GetUTCDate(TimeZoneId, StartDate);
			DateTime NewFinishDate = DBCommon.GetUTCDate(TimeZoneId, FinishDate);

			DateTime CurrentDate = DateTime.UtcNow;


			#region get project
			DateTime ProjectTargetStartDate;
			DateTime ProjectTargetFinishDate;
			int CalendarId;
			int ManagerId = -1;

			//Get project's fields
			using (IDataReader ProjectReader = DBProject.GetProject(project_id, User.DefaultTimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (ProjectReader.Read())
				{
					ProjectTargetStartDate = (DateTime)ProjectReader["TargetStartDate"];
					ProjectTargetFinishDate = (DateTime)ProjectReader["TargetFinishDate"];
					CalendarId = (int)ProjectReader["CalendarId"];
					ManagerId = (int)ProjectReader["ManagerId"];
				}
				else
					throw new Exception("Project not found");
			}
			#endregion

			#region Define dates

			//Вычисление длительностей с учетом начала проекта и календаря
			int NewDuration;
			int DirtyDuration;

			DirtyDuration = (int)((TimeSpan)(NewFinishDate - NewStartDate)).TotalMinutes;

			ProjectTargetStartDate = DBCalendar.GetFinishDateByDuration(CalendarId, ProjectTargetStartDate, 0);
			NewStartDate = DBCalendar.GetFinishDateByDuration(CalendarId, NewStartDate, 0);
			NewStartDate = MaxDate(ProjectTargetStartDate, NewStartDate);
			NewFinishDate = DBCalendar.GetStartDateByDuration(CalendarId, NewFinishDate, 0);

			if (NewStartDate > NewFinishDate)
				NewDuration = 0;
			else
				NewDuration = DBCalendar.GetDurationByFinishDate(CalendarId, NewStartDate, NewFinishDate);

			if (NewDuration == 0)
			{
				NewDuration = DBCalendar.GetDurationByFinishDate(CalendarId, NewStartDate, NewStartDate.AddMinutes(DirtyDuration));
				NewFinishDate = DBCalendar.GetFinishDateByDuration(CalendarId, NewStartDate, NewDuration);
			}

			if (NewDuration == 0)
				is_milestone = true;

			if (is_milestone)
			{
				NewFinishDate = NewStartDate;
				NewDuration = 0;
			}
			else
			{
				phase_id = -1;
			}

			if (task_time < 0)
				task_time = NewDuration;
			#endregion

			#region создаем задачу

			int UserId = Security.CurrentUser.UserID;
			int TaskId = -1;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				TaskId = DBTask.Insert(project_id, befor_task_id, UserId, title, description, CurrentDate, NewStartDate, NewFinishDate,
					NewDuration, priority_id, is_milestone, (int)ConstraintTypes.StartNotEarlierThan, NewStartDate,
					activation_type, completion_type, must_be_confirmed, phase_id, task_time);

				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Created, TaskId);
				SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_TaskList_TaskAdded, project_id, TaskId);

				// OZ: User Role Addon
				UserRoleHelper.AddTaskManagerRole(TaskId, ManagerId);
				if (ManagerId != UserId)
					UserRoleHelper.AddTaskCreatorRole(TaskId, UserId);
				ForeignContainerKey.Add(UserRoleHelper.CreateTaskContainerKey(TaskId), UserRoleHelper.CreateProjectContainerKey(project_id));
				//

				// Categories
				foreach (int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)TASK_TYPE, TaskId, CategoryId);
				}

				if (FileName != null)
				{
					string ContainerName = "FileLibrary";
					string ContainerKey = UserRoleHelper.CreateTaskContainerKey(TaskId);

					BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
					FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
					fs.SaveFile(fs.Root.Id, FileName, _inputStream);
					//Asset.Create(TaskId,ObjectTypes.Task, FileName,"", FileName, _inputStream, false);
				}

				// O.R: Scheduling
				Schedule.UpdateDateTypeValue(DateTypes.Task_StartDate, TaskId, NewStartDate);
				Schedule.UpdateDateTypeValue(DateTypes.Todo_FinishDate, TaskId, NewFinishDate);

				string update_id = Guid.NewGuid().ToString();

				DBTask.TaskUpdateAddPossibleTask(update_id, TaskId, true);

				int parentId = DBTask.GetParent(TaskId);
				if (parentId != -1)
					DBTask.TaskUpdateAddPossibleTask(update_id, parentId, false);

				Process(update_id);

				//Big E [9/2/06] don't use correction in this place
				/*
				//correction of project dates
				DateTime RangeFinishDate;
				using(IDataReader RangeReader = DBProject.GetProjectDateRange(project_id))
				{
					if(RangeReader.Read())
						RangeFinishDate = (DateTime)RangeReader["FinishDate"];
					else
						throw new Exception("project not found");
				}
					
				if(RangeFinishDate != ProjectFinishDate)
					DBProject.UpdateDates(project_id,ProjectStartDate,RangeFinishDate);
				*/
				// Resources
				if (resourceList != null)
				{
					foreach (int User_Id in resourceList)
					{

						bool CanManage = false;
						if (User_Id == ManagerId)
							CanManage = true;
						DBTask.AddResource(TaskId, User_Id, must_be_confirmed_by_user, CanManage);

						if (CanManage)
						{
							if (User_Id != ManagerId)
								UserRoleHelper.AddTaskManagerRole(TaskId, User_Id);
						}
						else
							UserRoleHelper.AddTaskResourceRole(TaskId, User_Id);

						if (User.IsExternal(User_Id))
							DBCommon.AddGate((int)TASK_TYPE, TaskId, User_Id);
					}
				}

				// O.R: Recalculating project TaskTime
				if (task_time > 0)
					TimeTracking.RecalculateProjectTaskTime(project_id);

				DBTask.RecalculateSummaryPercent(TaskId);

				RecalculateAllStates(project_id);

				tran.Commit();
			}
			#endregion
			return TaskId;
		}



		#endregion

		#region InsertTo

		public static int InsertTo(int project_id,
			int from_task_id,
			PlaceTypes place,
			string title,
			string description,
			DateTime StartDate,
			DateTime FinishDate,
			int priority_id,
			bool is_milestone,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			int phase_id,
			int task_time)
		{
			return InsertTo(project_id, from_task_id, place, title, description, StartDate, FinishDate, priority_id, is_milestone, activation_type, completion_type, must_be_confirmed, categories, FileName, _inputStream, null, false, phase_id, task_time);
		}

		public static int InsertTo(int project_id,
			int from_task_id,
			PlaceTypes place,
			string title,
			string description,
			DateTime StartDate,
			DateTime FinishDate,
			int priority_id,
			bool is_milestone,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			ArrayList resourceList,
			bool must_be_confirmed_by_user,
			int phase_id,
			int task_time)
		{
			if (!CanCreate(project_id))
				throw new AccessDeniedException();

			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (StartDate > FinishDate)
				throw new Exception("wrong dates");

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime NewStartDate = DBCommon.GetUTCDate(TimeZoneId, StartDate);
			DateTime NewFinishDate = DBCommon.GetUTCDate(TimeZoneId, FinishDate);

			DateTime CurrentDate = DateTime.UtcNow;


			#region get project
			DateTime ProjectTargetStartDate;
			DateTime ProjectTargetFinishDate;
			int CalendarId;
			int ManagerId = -1;

			//Get project's fields
			using (IDataReader ProjectReader = DBProject.GetProject(project_id, User.DefaultTimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (ProjectReader.Read())
				{
					ProjectTargetStartDate = (DateTime)ProjectReader["TargetStartDate"];
					ProjectTargetFinishDate = (DateTime)ProjectReader["TargetFinishDate"];
					CalendarId = (int)ProjectReader["CalendarId"];
					ManagerId = (int)ProjectReader["ManagerId"];
				}
				else
					throw new Exception("Project not found");
			}
			#endregion

			#region Define dates

			//Вычисление длительностей с учетом начала проекта и календаря
			int NewDuration;
			int DirtyDuration;

			DirtyDuration = (int)((TimeSpan)(NewFinishDate - NewStartDate)).TotalMinutes;

			ProjectTargetStartDate = DBCalendar.GetFinishDateByDuration(CalendarId, ProjectTargetStartDate, 0);
			NewStartDate = DBCalendar.GetFinishDateByDuration(CalendarId, NewStartDate, 0);
			NewStartDate = MaxDate(ProjectTargetStartDate, NewStartDate);
			NewFinishDate = DBCalendar.GetStartDateByDuration(CalendarId, NewFinishDate, 0);

			if (NewStartDate > NewFinishDate)
				NewDuration = 0;
			else
				NewDuration = DBCalendar.GetDurationByFinishDate(CalendarId, NewStartDate, NewFinishDate);

			if (NewDuration == 0)
			{
				NewDuration = DBCalendar.GetDurationByFinishDate(CalendarId, NewStartDate, NewStartDate.AddMinutes(DirtyDuration));
				NewFinishDate = DBCalendar.GetFinishDateByDuration(CalendarId, NewStartDate, NewDuration);
			}

			if (NewDuration == 0)
				is_milestone = true;

			if (is_milestone)
			{
				NewFinishDate = NewStartDate;
				NewDuration = 0;
			}
			else
			{
				phase_id = -1;
			}

			if (task_time < 0)
				task_time = NewDuration;
			#endregion

			#region создаем задачу

			int UserId = Security.CurrentUser.UserID;
			int TaskId = -1;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				TaskId = DBTask.InsertTo(project_id, from_task_id, (byte)place, UserId, title, description, CurrentDate, NewStartDate, NewFinishDate,
					NewDuration, priority_id, is_milestone, (int)ConstraintTypes.StartNotEarlierThan, NewStartDate,
					activation_type, completion_type, must_be_confirmed, phase_id, task_time);

				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Created, TaskId);
				SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_TaskList_TaskAdded, project_id, TaskId);

				// OZ: User Role Addon
				UserRoleHelper.AddTaskManagerRole(TaskId, ManagerId);
				if (ManagerId != UserId)
					UserRoleHelper.AddTaskCreatorRole(TaskId, UserId);
				ForeignContainerKey.Add(UserRoleHelper.CreateTaskContainerKey(TaskId), UserRoleHelper.CreateProjectContainerKey(project_id));
				//

				// Categories
				foreach (int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)TASK_TYPE, TaskId, CategoryId);
				}

				if (FileName != null)
				{
					string ContainerName = "FileLibrary";
					string ContainerKey = UserRoleHelper.CreateTaskContainerKey(TaskId);

					BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
					FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
					fs.SaveFile(fs.Root.Id, FileName, _inputStream);
					//Asset.Create(TaskId,ObjectTypes.Task, FileName,"", FileName, _inputStream, false);
				}

				// O.R: Scheduling
				Schedule.UpdateDateTypeValue(DateTypes.Task_StartDate, TaskId, NewStartDate);
				Schedule.UpdateDateTypeValue(DateTypes.Todo_FinishDate, TaskId, NewFinishDate);

				string update_id = Guid.NewGuid().ToString();

				DBTask.TaskUpdateAddPossibleTask(update_id, TaskId, true);

				int parentId = DBTask.GetParent(TaskId);
				if (parentId != -1)
					DBTask.TaskUpdateAddPossibleTask(update_id, parentId, false);

				Process(update_id);

				//Big E [9/2/06] don't use correction in this place
				//correction of project dates
				/*
				DateTime RangeFinishDate;
				using(IDataReader RangeReader = DBProject.GetProjectDateRange(project_id))
				{
					if(RangeReader.Read())
						RangeFinishDate = (DateTime)RangeReader["FinishDate"];
					else
						throw new Exception("project not found");
				}
					
				if(RangeFinishDate != ProjectFinishDate)
					DBProject.UpdateDates(project_id,ProjectStartDate,RangeFinishDate);
				*/

				// Resources
				if (resourceList != null)
				{
					foreach (int User_Id in resourceList)
					{

						bool CanManage = false;
						if (User_Id == ManagerId)
							CanManage = true;
						DBTask.AddResource(TaskId, User_Id, must_be_confirmed_by_user, CanManage);

						if (CanManage)
						{
							if (User_Id != ManagerId)
								UserRoleHelper.AddTaskManagerRole(TaskId, User_Id);
						}
						else
							UserRoleHelper.AddTaskResourceRole(TaskId, User_Id);

						if (User.IsExternal(User_Id))
							DBCommon.AddGate((int)TASK_TYPE, TaskId, User_Id);
					}
				}

				// O.R: Recalculating project TaskTime
				if (task_time > 0)
					TimeTracking.RecalculateProjectTaskTime(project_id);

				DBTask.RecalculateSummaryPercent(TaskId);

				RecalculateAllStates(project_id);

				tran.Commit();
			}
			#endregion
			return TaskId;
		}



		#endregion
		#region PROCESS
		public static void process()
		{
			XmlDocument doc = new XmlDocument();

			//doc.
			doc.Load("C:\\Exelon IT Master Schedule 11-2003 Final.xml");
			//doc.Load("C:\\Office Move.xml");
			//Task.TasksImport(doc.DocumentElement,63);

		}
		#endregion

		#region Update

		public static void Update(int task_id,
			string title,
			string description,
			DateTime StartDate,
			DateTime FinishDate,
			int priority_id,
			bool is_milestone,
			int activation_type,
			int completion_type,
			bool must_be_confirmed,
			ArrayList categories,
			int phase_id,
			int task_time)
		{
			if (!CanUpdate(task_id))
				throw new AccessDeniedException();

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime NewStartDate = DBCommon.GetUTCDate(TimeZoneId, StartDate);
			DateTime NewFinishDate = DBCommon.GetUTCDate(TimeZoneId, FinishDate);
			DateTime CurrentDate = DateTime.UtcNow;

			DateTime ProjectTargetStartDate;
			int CalendarId;

			//Task's fields
			int ProjectId;
			string OldTitle;
			string OldDescription;
			int OldPriorityId;
			int OldCompletionType;
			bool OldIsMilestone;
			bool OldIsSummary;
			DateTime OldStartDate;
			DateTime OldFinishDate;
			int OldDuration;
			int OldConstraintType;
			DateTime OldConstraintDate;
			int OldPercentCompleted;
			bool OldIsCompleted;
			int OldActivationType;
			bool OldMustBeConfirmed;
			int OldPhase = -1;
			int OldTaskTime;
			//get project
			//#region get task

			using (IDataReader TaskReader = DBTask.GetTask(task_id, User.DefaultTimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (TaskReader.Read())
				{
					ProjectId = (int)TaskReader["ProjectId"];
					OldFinishDate = (DateTime)TaskReader["FinishDate"];
					OldStartDate = (DateTime)TaskReader["StartDate"];
					OldDuration = (int)TaskReader["Duration"];
					OldConstraintType = (int)TaskReader["ConstraintTypeId"];
					OldConstraintDate = TaskReader["ConstraintDate"].GetType() == typeof(DBNull) ? DateTime.MinValue : (DateTime)TaskReader["ConstraintDate"];
					OldIsMilestone = (bool)TaskReader["IsMilestone"];
					OldIsSummary = (bool)TaskReader["IsSummary"];

					OldTitle = (string)TaskReader["Title"];
					OldDescription = TaskReader["Description"].GetType() == typeof(DBNull) ? null : (string)TaskReader["Description"];
					OldPriorityId = (int)TaskReader["PriorityId"];
					OldCompletionType = (int)TaskReader["CompletionTypeId"];
					OldActivationType = (int)TaskReader["ActivationTypeId"];

					OldPercentCompleted = (int)TaskReader["PercentCompleted"];
					OldIsCompleted = (bool)TaskReader["IsCompleted"];
					OldMustBeConfirmed = (bool)TaskReader["MustBeConfirmed"];
					if (TaskReader["PhaseId"] != DBNull.Value)
						OldPhase = (int)TaskReader["PhaseId"];
					OldTaskTime = (int)TaskReader["TaskTime"];
				}
				else
					throw new Exception("Task not found");
			}

			// O.R. [2009-09-29]: Don't update dates and IsMileStone flag for MSProject Task
			if (Project.GetIsMSProject(ProjectId))
			{
				Task2.UpdateSimple(task_id, title, description, priority_id, activation_type, completion_type, must_be_confirmed, task_time, phase_id, categories);
				return;
			}

			//Get project's fields
			using (IDataReader ProjectReader = DBProject.GetProject(ProjectId, User.DefaultTimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (ProjectReader.Read())
				{
					ProjectTargetStartDate = (DateTime)ProjectReader["TargetStartDate"];
					CalendarId = (int)ProjectReader["CalendarId"];
				}
				else
					throw new Exception("Project not found");
			}
			//#endregion

			//#region Dates

			int NewDuration;
			int DirtyDuration;
			DateTime NewConstraintDate = OldConstraintDate;
			int NewConstraintType = OldConstraintType;

			DirtyDuration = (int)((TimeSpan)(NewFinishDate - NewStartDate)).TotalMinutes;

			ProjectTargetStartDate = DBCalendar.GetFinishDateByDuration(CalendarId, ProjectTargetStartDate, 0);

			// Fix: 2006-12-14 Milestone's problem
			if (!OldIsSummary)
				NewDuration = DBCalendar.GetDurationByFinishDate(CalendarId, NewStartDate, NewFinishDate);
			else
				NewDuration = OldDuration;

			// Fix: 2006-12-14 Milestone's problem
			if (OldIsSummary)
				is_milestone = false;

			//
			if (is_milestone)
			{
				if (OldIsSummary)
					throw new Exception("Unable mark summary as mileston");

				if (NewStartDate != OldStartDate)
				{
					NewStartDate = DBCalendar.GetFinishDateByDuration(CalendarId, NewStartDate, 0);
					NewStartDate = MaxDate(ProjectTargetStartDate, NewStartDate);
					OldConstraintDate = NewStartDate;

				}
				NewDuration = 0;
				NewFinishDate = NewStartDate;
				NewConstraintDate = NewStartDate;
				NewConstraintType = (int)ConstraintTypes.StartNotEarlierThan;
			}
			else
			{
				if (NewStartDate != OldStartDate || NewFinishDate != OldFinishDate)
				{
					if (!OldIsSummary)
					{
						NewStartDate = DBCalendar.GetFinishDateByDuration(CalendarId, NewStartDate, 0);

						NewStartDate = MaxDate(ProjectTargetStartDate, NewStartDate);
						NewFinishDate = DBCalendar.GetStartDateByDuration(CalendarId, NewFinishDate, 0);

						NewDuration = DBCalendar.GetDurationByFinishDate(CalendarId, NewStartDate, NewFinishDate);

						if (NewDuration == 0)
						{
							NewDuration = DBCalendar.GetDurationByFinishDate(CalendarId, NewStartDate, NewStartDate.AddMinutes(DirtyDuration));
							NewFinishDate = DBCalendar.GetFinishDateByDuration(CalendarId, NewStartDate, NewDuration);
						}
						NewConstraintDate = NewStartDate;
						NewConstraintType = (int)ConstraintTypes.StartNotEarlierThan;
					}
					else
					{
						NewStartDate = OldStartDate;
						NewFinishDate = OldFinishDate;
						NewDuration = OldDuration;
					}
				}

				if (NewDuration == 0 && !OldIsSummary)
					is_milestone = true;
			}

			if (!is_milestone)
				phase_id = -1;

			//#endregion
			//Categories
			//#region Categories
			ArrayList NewCategories = new ArrayList(categories);
			ArrayList DeletedCategories = new ArrayList();

			using (IDataReader reader = DBCommon.GetListCategoriesByObject((int)TASK_TYPE, task_id))
			{
				while (reader.Read())
				{
					int CategoryId = (int)reader["CategoryId"];
					if (NewCategories.Contains(CategoryId))
						NewCategories.Remove(CategoryId);
					else
						DeletedCategories.Add(CategoryId);
				}
			}
			//#endregion

			int UserId = Security.CurrentUser.UserID;

			//TODO: Update Task !!!!!!!!
			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBTask.Update(task_id, title, description, NewStartDate, NewFinishDate,
					NewDuration, priority_id, is_milestone, NewConstraintType, NewConstraintDate,
					activation_type, completion_type, must_be_confirmed, phase_id, task_time);

				if (OldTitle != title || OldDescription != description)
					SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_GeneralInfo, task_id);
				if (OldStartDate != NewStartDate
					|| OldFinishDate != NewFinishDate
					|| OldTaskTime != task_time)
					SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_Timeline, task_id);
				if (OldPriorityId != priority_id)
					SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_Priority, task_id);
				if (OldConstraintType != NewConstraintType
					|| OldActivationType != activation_type
					|| OldCompletionType != completion_type
					|| OldMustBeConfirmed != must_be_confirmed
					|| OldPhase != phase_id)
					SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_ConfigurationInfo, task_id);

				// Remove Category
				foreach (int CategoryId in DeletedCategories)
					DBCommon.RemoveCategoryFromObject((int)TASK_TYPE, task_id, CategoryId);

				// Add Category
				foreach (int CategoryId in NewCategories)
					DBCommon.AssignCategoryToObject((int)TASK_TYPE, task_id, CategoryId);

				if (DeletedCategories.Count > 0 || NewCategories.Count > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_GeneralCategories, task_id);

				// O.R: Scheduling
				if (OldStartDate != NewStartDate)
					Schedule.UpdateDateTypeValue(DateTypes.Task_StartDate, task_id, NewStartDate);
				if (OldFinishDate != NewFinishDate)
				{
					Schedule.UpdateDateTypeValue(DateTypes.Task_FinishDate, task_id, NewFinishDate);

					// O.R. [2008-10-07]: TaskDates
					DBTask.AddTaskDates(ProjectId, task_id, Security.CurrentUser.UserID, OldFinishDate, NewFinishDate);
				}

				//InternalUpdate(task_id);
				string update_id = Guid.NewGuid().ToString();

				int parentId = DBTask.GetParent(task_id);
				if (parentId != -1)
					DBTask.TaskUpdateAddPossibleTask(update_id, parentId, false);

				DBTask.TaskUpdateAddPossibleTask(update_id, task_id, false);

				DBTask.TaskUpdateAddPossibleSuccessor(update_id, task_id);

				// O.R: Recalculating project TaskTime
				if (OldTaskTime != task_time)
					TimeTracking.RecalculateProjectTaskTime(ProjectId);

				Process(update_id);

				RecalculateAllStates(ProjectId);

				tran.Commit();
			}
		}



		#endregion
		//TODO: Testing
		//TODO: percent completed update
		#region MoveRight
		public static void MoveRight(int task_id)
		{
			if (!CanUpdateConfigurationInfo(task_id))
				throw new AccessDeniedException();

			using (DbTransaction tran = DbTransaction.Begin())
			{
				int summaryId = DBTask.MoveRight(task_id);
				if (summaryId == -2)
					throw new UnableMoveTask();

				int ProjectId = DBTask.GetProject(task_id);

				if (!DBTask.ProjectTaskCheckConsistency(ProjectId))
					throw new UnableMoveTask();

				string update_id = Guid.NewGuid().ToString();

				if (summaryId != -1)
					DBTask.TaskUpdateAddPossibleTask(update_id, summaryId, true);

				int ParentId = DBTask.GetParent(task_id);
				if (ParentId != -1)
					DBTask.TaskUpdateAddPossibleTask(update_id, ParentId, true);

				DBTask.TaskUpdateAddPossibleTask(update_id, task_id, false);

				Process(update_id);

				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_ConfigurationInfo, task_id);

				DBTask.RecalculateSummaryPercent(task_id);

				RecalculateAllStates(ProjectId);

				tran.Commit();
			}
		}
		#endregion
		//TODO: Testing
		//TODO: percent completed update
		#region MoveLeft
		public static void MoveLeft(int task_id)
		{
			if (!CanUpdateConfigurationInfo(task_id))
				throw new AccessDeniedException();

			int project_id = GetProject(task_id);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				int ParentId = DBTask.GetParent(task_id);
				int summaryId = DBTask.MoveLeft(task_id);

				int ProjectId = DBTask.GetProject(task_id);
				if (!DBTask.ProjectTaskCheckConsistency(ProjectId))
					throw new UnableMoveTask();

				string update_id = Guid.NewGuid().ToString();

				if (summaryId != -1)
					DBTask.TaskUpdateAddPossibleTask(update_id, summaryId, false);

				if (ParentId != -1)
					DBTask.TaskUpdateAddPossibleTask(update_id, ParentId, false);


				DBTask.TaskUpdateAddPossibleTask(update_id, task_id, false);

				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_ConfigurationInfo, task_id);

				Process(update_id);

				if (ParentId != -1)
				{
					bool ParentIsSummary = true;
					bool ParentWasCompleted = false;
					int ParentCompletionTypeId = (int)CompletionType.All;
					using (IDataReader reader = DBTask.GetTask(ParentId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
					{
						if (reader.Read())
						{
							ParentCompletionTypeId = (int)reader["CompletionTypeId"];
							ParentIsSummary = (bool)reader["IsSummary"];
							ParentWasCompleted = (bool)reader["IsCompleted"];
						}
					}
					// если parent task перестал быть summary и тип завершения у него - all, то 
					// пресчитаем его процент на основании среднего значения % завершения ресурсов
					if (!ParentIsSummary && ParentCompletionTypeId == (int)CompletionType.All)
					{
						int OverallPercent = RecalculateOverallPercent(ParentId);
						DBTask.UpdatePercent(ParentId, OverallPercent);

						if (!ParentWasCompleted && OverallPercent == 100)
						{
							DBTask.UpdateCompletion(ParentId, true, (int)CompletionReason.CompletedAutomatically);
						}
					}

					// если parent перестал быть summary и если у него CompletionType = any, то 
					// оставляем его процент без изменений

					DBTask.RecalculateSummaryPercent(ParentId);
				}

				DBTask.RecalculateSummaryPercent(task_id);

				RecalculateAllStates(project_id);

				tran.Commit();
			}
		}
		#endregion
		//TODO: Testing
		//TODO: percent completed update
		#region MoveTo

		public static void MoveTo(int task_id, int after_task_num)
		{
			if (!CanUpdateConfigurationInfo(task_id))
				throw new AccessDeniedException();

			int project_id = GetProject(task_id);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				int ParentId = DBTask.GetParent(task_id);
				int summaryId = DBTask.MoveTo(task_id, after_task_num);

				int ProjectId = DBTask.GetProject(task_id);
				if (!DBTask.ProjectTaskCheckConsistency(ProjectId))
					throw new UnableMoveTask();

				string update_id = Guid.NewGuid().ToString();

				if (summaryId != -1)
					DBTask.TaskUpdateAddPossibleTask(update_id, summaryId, false);

				if (ParentId != -1)
					DBTask.TaskUpdateAddPossibleTask(update_id, ParentId, true);

				int ParentId1 = DBTask.GetParent(task_id);
				if (ParentId1 != -1)
					DBTask.TaskUpdateAddPossibleTask(update_id, ParentId1, true);

				DBTask.TaskUpdateAddPossibleTask(update_id, task_id, false);

				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_ConfigurationInfo, task_id);

				Process(update_id);


				if (ParentId != -1)
				{
					bool ParentIsSummary = true;
					bool ParentWasCompleted = false;
					int ParentCompletionTypeId = (int)CompletionType.All;
					using (IDataReader reader = DBTask.GetTask(ParentId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
					{
						if (reader.Read())
						{
							ParentCompletionTypeId = (int)reader["CompletionTypeId"];
							ParentIsSummary = (bool)reader["IsSummary"];
							ParentWasCompleted = (bool)reader["IsCompleted"];
						}
					}

					// если parent task перестал быть summary и тип завершения у него - all, то 
					// пресчитаем его процент на основании среднего значения % завершения ресурсов
					if (!ParentIsSummary && ParentCompletionTypeId == (int)CompletionType.All)
					{
						int OverallPercent = RecalculateOverallPercent(ParentId);
						DBTask.UpdatePercent(ParentId, OverallPercent);

						if (!ParentWasCompleted && OverallPercent == 100)
							DBTask.UpdateCompletion(ParentId, true, (int)CompletionReason.CompletedAutomatically);
					}

					// если parent перестал быть summary и если у него CompletionType = any, то 
					// оставляем его процент без изменений
					DBTask.RecalculateSummaryPercent(ParentId);
				}

				DBTask.RecalculateSummaryPercent(task_id);

				RecalculateAllStates(project_id);

				tran.Commit();
			}
		}

		#endregion
		//TODO: testing
		//TODO: Locking
		#region Task Import

		#region TaskImport2

		private interface IMsProjectSync
		{
			bool Validate();
			void Recalculate(TaskSync origMsProjTask);
		}


		private class ProjectSyncContext
		{
			private Dictionary<int, ResourceSync> _resourcesBox;
			private Dictionary<int, TaskSync> _taskBox;
			private readonly DataTable _resourceMapping;
			private List<MsProjectCalendarSync> _msProjectCalendarList;

			public ProjectSyncContext()
				: this(null)
			{

			}
			public ProjectSyncContext(DataTable resMapping)
			{
				_resourceMapping = resMapping;
				_resourcesBox = new Dictionary<int, ResourceSync>();
				_taskBox = new Dictionary<int, TaskSync>();
				_msProjectCalendarList = new List<MsProjectCalendarSync>();
			}

			public List<MsProjectCalendarSync> MsProjectCalendarList
			{
				get { return _msProjectCalendarList; }
			}

			public List<TaskSync> GetChildList(TaskSync parent)
			{
				return GetChildTaskList(_taskBox.Values, parent);
			}

			public ResourceSync GetResByMsProjMapVal(int msProjMapVal)
			{
				ResourceSync retVal = null;
				try
				{
					_resourcesBox.TryGetValue(msProjMapVal, out retVal);
				}
				catch (ArgumentNullException)
				{
					return null;
				}

				return retVal;
			}

			public TaskSync GetTaskByMsProjMapVal(int msProjMapVal)
			{
				TaskSync retVal = null;
				try
				{
					_taskBox.TryGetValue(msProjMapVal, out retVal);
				}
				catch (ArgumentNullException)
				{
					return null;
				}

				return retVal;

			}

			public void AddResource(ResourceSync resource)
			{
				//If need mapping
				if (resource.IbnMapVal == -1)
				{
					resource.IbnMapVal =
							ResourceSync.GetMapIbnValue(_resourceMapping,
														resource.MsProjectMapVal);
				}

				_resourcesBox.Add(resource.MsProjectMapVal, resource);
			}

			public void AddTask(TaskSync task)
			{
				_taskBox.Add(task.MsProjectMapValue, task);
			}

			/// <summary>
			/// Equalities the project comparer.
			/// return difference in percent
			/// </summary>
			/// <param name="first">The first.</param>
			/// <param name="second">The second.</param>
			/// <returns></returns>
			public static int EqualityProjectComparer(ProjectSyncContext first,
													  ProjectSyncContext second)
			{

				//Empty are equals
				if ((first._taskBox.Count == 0) && (second._taskBox.Count == 0))
					return 0;

				//swap
				if (first._taskBox.Count == 0)
				{
					ProjectSyncContext dummy = second;
					second = first;
					first = dummy;
				}


				int allDifference = 0;

				foreach (int key in first._taskBox.Keys)
				{
					TaskSync firstTask = first._taskBox[key];
					TaskSync secondTask;

					if (!second._taskBox.TryGetValue(key, out secondTask))
					{
						allDifference++;
						continue;
					}

					if (string.Compare(firstTask.Name, secondTask.Name) != 0)
						allDifference++;
				}

				return allDifference / first._taskBox.Count * 100;
			}

		};

		private class TimePhasedDataType
		{
			public int Type;
			public int UID;
			public DateTime Start;
			public DateTime Finish;
			public TimeSpan Value;
			public int Unit;
		};

		private class ResourceSync : IMsProjectSync
		{

			#region Fields
			#region Recalculate fields
			private int _percentWorkComplete;
			private TimeSpan _actualWork;
			private TimeSpan _remainingWork;
			private TimeSpan _work;
			private MsProjectCalendarSync _resourceCalendar;
			#endregion

			#region IBN<->MsProject field mapping
			//MsProject<UID> <-> IBN<PrincipalID>
			private int _msProjectMapVal = -1;
			private int _ibnMapVal = -1;
			#endregion

			private string _resName;
			private string _resCode;
			#endregion
			#region Properties
			public MsProjectCalendarSync ResourceCalendar
			{
				get { return _resourceCalendar; }
				set { _resourceCalendar = value; }
			}

			public string Name
			{
				get { return _resName; }
				set { _resName = value; }
			}

			public string Code
			{
				get { return _resCode; }
				set { _resCode = value; }
			}

			public int MsProjectMapVal
			{
				get { return _msProjectMapVal; }
				set { _msProjectMapVal = value; }
			}
			public int IbnMapVal
			{
				get { return _ibnMapVal; }
				set { _ibnMapVal = value; }
			}

			/// <summary>
			/// Recalculate 
			/// </summary>
			public int PercentWorkComplete
			{
				get { return _percentWorkComplete; }
				set { _percentWorkComplete = value; }
			}
			public TimeSpan ActualWork
			{
				get { return _actualWork; }
				set { _actualWork = value; }
			}
			public TimeSpan RemainingWork
			{
				get { return _remainingWork; }
				set { _remainingWork = value; }
			}
			public TimeSpan Work
			{
				get { return _work; }
				set { _work = value; }
			}


			#endregion
			#region Methods

			protected ResourceSync()
			{
				_work = TimeSpan.Zero;
				_actualWork = TimeSpan.Zero;
				_remainingWork = TimeSpan.Zero;

			}

			/// <summary>
			/// Maps the resources.
			/// </summary>
			/// <remarks>
			/// mapTable(MsProject resource [UID] <-> IBN resource [PrincipalID])
			/// </remarks>
			/// <param name="mapTable">The map table.</param>
			public static int GetMapIbnValue(DataTable mapTable, int msProjMapValue)
			{
				//Not using
				if (mapTable == null)
					return 0;

				DataRow row = mapTable.Rows.Find(new object[] { msProjMapValue });

				if (row != null)
					return Convert.ToInt32(row["PrincipalId"]);
				//Not found
				return -1;
			}

			public static ResourceSync CreateFromXml(XmlNode node, ProjectSyncContext projectSyncContext,
													 XmlNamespaceManager nsXmlMmngr, int timeZoneId)
			{
				if (node == null)
					return null;

				int msProjResUID = (Int32)GetXmlValue(node["UID"], typeof(Int32), false);

				ResourceSync retVal = projectSyncContext != null
									  ? projectSyncContext.GetResByMsProjMapVal(msProjResUID)
									  : null;

				if (retVal != null)
					return retVal;

				retVal = new ResourceSync();
				retVal.MsProjectMapVal = msProjResUID;

				retVal.Name = (String)GetXmlValue(node["Name"], typeof(String),
												  true, String.Empty);
				retVal.Code = (String)GetXmlValue(node["Code"], typeof(String),
												  true, String.Empty);
				//Recalculate fields
				retVal.Work = (TimeSpan)GetXmlValue(node["Work"], typeof(TimeSpan), true,
													TimeSpan.Zero);
				if (projectSyncContext != null)
				{
					int calendarUID = (Int32)GetXmlValue(node["CalendarUID"], typeof(Int32), false);
					//try to find corresponding resource calendar 
					retVal.ResourceCalendar =
						projectSyncContext.MsProjectCalendarList.Find(delegate(MsProjectCalendarSync calendar) { return calendar.UID == calendarUID; });

				}

				if (retVal.Validate())
				{
					if (projectSyncContext != null)
						projectSyncContext.AddResource(retVal);
					return retVal;
				}

				return null;
			}

			public static ResourceSync CreateFromDataRow(DataRow row, ProjectSyncContext projSandBox,
														 int timeZoneId)
			{
				if (row == null)
					return null;

				int msProjResUID = 0;
				if (row["MSProjectResourceId"] != DBNull.Value)
					msProjResUID =
								Convert.ToInt32(row["MSProjectResourceId"]);

				ResourceSync retVal = projSandBox != null
									  ? projSandBox.GetResByMsProjMapVal(msProjResUID)
									  : null;
				if (retVal != null)
				{
					return retVal;
				}
				retVal = new ResourceSync();

				retVal.MsProjectMapVal = msProjResUID;
				retVal.IbnMapVal = Convert.ToInt32(row["UserId"]);

				if (retVal.Validate())
				{
					if (projSandBox != null)
						projSandBox.AddResource(retVal);
					return retVal;
				}

				return null;

			}



			#region IMsProjectSync Members

			public bool Validate()
			{
				return (this.MsProjectMapVal != 0);
			}

			public void Recalculate(TaskSync origMsProjTask)
			{
				double totalWork = this.Work.TotalMinutes;
				double totalActualWork = this.ActualWork.TotalMinutes;

				this.PercentWorkComplete = (int)Math.Round((100 / totalWork * totalActualWork));
			}

			#endregion

			#endregion
		}

		private class AssignmentSync : IMsProjectSync
		{
			#region Fields

			#region Recalculate fields
			private int _percentWorkComplete;
			private int _percentComplete;
			private TimeSpan _actualWork;
			private TimeSpan _remainingWork;
			// it is the total amount of work scheduled to be performed by a
			// resource on a task
			private TimeSpan _work;
			private DateTime _stop;

			#endregion


			//May be null
			private DateTime? _actualFinish;
			private DateTime? _actualStart;

			private DateTime _start = DateTime.MinValue;
			private DateTime _finish = DateTime.MaxValue;
			private ResourceSync _resource;
			private List<TimePhasedDataType> _timePhaseDataTypeList;
			private int _workContour;
			private double _units;
			private TimeSpan _duration;
			private int _uid;
			//IBNcalendarId
			#endregion

			#region Properties
			public int UID
			{
				get { return _uid; }
				set { _uid = value; }
			}
			public int PercentComplete
			{
				get { return _percentComplete; }
				set { _percentComplete = value; }
			}
			public DateTime Stop
			{
				get { return _stop; }
				set { _stop = value; }
			}
			public TimeSpan Duration
			{
				get { return _duration; }
				set { _duration = value; }
			}
			public int WorkContour
			{
				get { return _workContour; }
				set { _workContour = value; }
			}
			public double Units
			{
				get { return _units; }
				set { _units = value; }
			}
			public List<TimePhasedDataType> TimePhaseDataType
			{
				get { return _timePhaseDataTypeList; }
			}
			public ResourceSync Resource
			{
				get { return _resource; }
				set { _resource = value; }
			}
			public TimeSpan Work
			{
				get { return _work; }
				set { _work = value; }
			}

			public DateTime? ActualFinish
			{
				get { return _actualFinish; }
				set { _actualFinish = value; }
			}
			public DateTime? ActualStart
			{
				get { return _actualStart; }
				set { _actualStart = value; }
			}
			public DateTime Start
			{
				get { return _start; }
				set { _start = value; }
			}
			public DateTime Finish
			{
				get { return _finish; }
				set { _finish = value; }
			}

			/// <summary>
			/// Recalculate 
			/// </summary>
			public int PercentWorkComplete
			{
				get { return _percentWorkComplete; }
				set { _percentWorkComplete = value; }
			}
			public TimeSpan ActualWork
			{
				get { return _actualWork; }
				set { _actualWork = value; }
			}
			public TimeSpan RemainingWork
			{
				get { return _remainingWork; }
				set { _remainingWork = value; }
			}
			#endregion

			#region Methods
			public AssignmentSync()
			{
				_work = TimeSpan.Zero;
				_actualWork = TimeSpan.Zero;
				_remainingWork = TimeSpan.Zero;
				_timePhaseDataTypeList = new List<TimePhasedDataType>();

			}

			public static AssignmentSync CreateFromXML(XmlNode node,
													   ProjectSyncContext projSandBox,
													   XmlNamespaceManager nsXmlMmngr,
													   int timeZoneId)
			{
				if (node == null)
					return null;

				AssignmentSync retVal = new AssignmentSync();

				retVal.UID = (int)GetXmlValue(node["UID"], typeof(int), false);
				retVal.Start = (DateTime)GetXmlValue(node["Start"], typeof(DateTime), true, DateTime.MinValue);
				retVal.Finish = (DateTime)GetXmlValue(node["Finish"], typeof(DateTime), true, DateTime.MaxValue);

				retVal.ActualWork = (TimeSpan)GetXmlValue(node["ActualWork"], typeof(TimeSpan),
														  true, TimeSpan.Zero);
				retVal.PercentWorkComplete = (Int32)GetXmlValue(node["PercentWorkComplete"],
																typeof(Int32), true, 0);
				retVal.RemainingWork = (TimeSpan)GetXmlValue(node["RemainingWork"],
															typeof(TimeSpan), true,
															TimeSpan.Zero);
				retVal.Work = (TimeSpan)GetXmlValue(node["Work"], typeof(TimeSpan), true,
													TimeSpan.Zero);
				retVal.WorkContour = (int)GetXmlValue(node["WorkContour"], typeof(int), true, 0);
				retVal.Units = (double)GetXmlValue(node["Units"], typeof(double), true, 1.0);

				int resourseUID = (Int32)GetXmlValue(node["ResourceUID"], typeof(Int32));

				string resXPath = string.Format("ns:Resources/ns:Resource[ns:UID = '{0}']",
												 resourseUID);

				XmlNode resNode = node.OwnerDocument.DocumentElement.SelectSingleNode(resXPath,
																	  nsXmlMmngr);


				//Link assignments and resource
				if (resNode != null)
				{
					retVal.Resource =
								ResourceSync.CreateFromXml(resNode, projSandBox,
														   nsXmlMmngr, timeZoneId);
				}

				//Retrieve all TimePhaseData elements
				XmlNodeList timePhaseNodeList =
							node.SelectNodes("ns:TimephasedData", nsXmlMmngr);
				foreach (XmlNode timePhaseNode in timePhaseNodeList)
				{
					TimePhasedDataType newTimePhase = new TimePhasedDataType();
					newTimePhase.UID = (Int32)GetXmlValue(timePhaseNode["UID"], typeof(Int32),
												true, -1);
					newTimePhase.Unit = (Int32)GetXmlValue(timePhaseNode["Unit"], typeof(Int32),
												true, 2);

					newTimePhase.Type = (Int32)GetXmlValue(timePhaseNode["Type"], typeof(Int32),
													true, -1);
					newTimePhase.Start = (DateTime)GetXmlValue(timePhaseNode["Start"], typeof(DateTime),
																true, DateTime.MinValue);
					newTimePhase.Finish = (DateTime)GetXmlValue(timePhaseNode["Finish"], typeof(DateTime),
																true, DateTime.MinValue);
					newTimePhase.Value = (TimeSpan)GetXmlValue(timePhaseNode["Value"], typeof(TimeSpan),
																true, TimeSpan.Zero);

					retVal.TimePhaseDataType.Add(newTimePhase);
				}

				if (retVal.Validate())
					return retVal;

				return null;
			}

			public static AssignmentSync CreateFromDataRow(DataRow row,
														  ProjectSyncContext projSandBox,
														  int timeZoneId)
			{
				AssignmentSync retVal = new AssignmentSync();


				retVal.PercentWorkComplete =
							Convert.ToInt32(row["PercentCompleted"]);

				retVal.ActualFinish = null;
				if (row["ActualFinishDate"] != DBNull.Value)
					retVal.ActualFinish =
								Convert.ToDateTime(row["ActualFinishDate"]);


				//Link assignments and resource
				retVal.Resource =
								ResourceSync.CreateFromDataRow(row, projSandBox,
															   timeZoneId);

				if (retVal.Validate())
					return retVal;

				return null;
			}
			#endregion

			#region IMsProjectSync Members

			public bool Validate()
			{
				//WorkPercent not set but ReaminingWork is assigned
				//OpenProject format
				if (this.PercentWorkComplete == 0
				   && this.RemainingWork != this.Work)
				{
					this.ActualWork = this.Work - this.RemainingWork;
					if (this.Work != TimeSpan.Zero)
					{
						this.PercentWorkComplete = (int)(this.ActualWork.TotalMinutes
												   / this.Work.TotalMinutes * 100);

					}
				}
				return ((this.Resource != null) && (this.Resource.IbnMapVal != -1));
			}

			public void Recalculate(TaskSync origMsProjTask)
			{
				//Find this assign in orig task
				AssignmentSync origAssign = origMsProjTask.FindAssignment(this);

				if (origAssign == null)
					throw new Exception("C'not find corresponding assignments.");


				ProjectBusinessUtil.Assignment.Assignment extAssign =
										CreateMsProjectAssignment(origMsProjTask, origAssign);

				//Используем значение work из оригинального назначения так как msProject иногда формирует не корректные интервалы
				// не актуальные на несколько минут или секунд из-за этого значение work (расчетное) получается немного меньше
				//long work = extAssign.GetWork(extAssign.Start, extAssign.End);
				long work = CalendarHelper.Tick2Milis(origAssign.Work.Ticks);
				//Устанавливаем процент завершения по трудозатратам
				extAssign.SetActualWork(work * (this.PercentWorkComplete * 0.01));
				long actualWork = extAssign.GetWork(extAssign.Start, extAssign.Stop);
				long remainingWork = work - actualWork;

				//Process percent work completion
				this.Duration = new TimeSpan(CalendarHelper.Milis2Tick(extAssign.Duration));
				this.Work = new TimeSpan(CalendarHelper.Milis2Tick(work));
				this.ActualWork = new TimeSpan(CalendarHelper.Milis2Tick(actualWork));
				this.RemainingWork = new TimeSpan(CalendarHelper.Milis2Tick(remainingWork));
				this.PercentWorkComplete = (int)Math.Round(((double)actualWork / work * 100));
				this.PercentComplete = (int)Math.Round(extAssign.PercentComplete * 100);
				this.Stop = new DateTime(CalendarHelper.Milis2Tick(extAssign.Stop));

				//Get time phased data
				this.TimePhaseDataType.AddRange(GetTimePhases(extAssign, origAssign.UID, ProjectBusinessUtil.Services.TimePhasedDataType.TimePhaseType.AssignmentRemainingWork));
				this.TimePhaseDataType.AddRange(GetTimePhases(extAssign, origAssign.UID, ProjectBusinessUtil.Services.TimePhasedDataType.TimePhaseType.AssignmentActualWork));

				//Recalculate resources
				this.Resource.Work += this.Work;
				this.Resource.ActualWork += this.ActualWork;
				this.Resource.RemainingWork += this.RemainingWork;
				this.Resource.Recalculate(origMsProjTask);

			}
			#endregion
		}

		private class PredecessorSync : IMsProjectSync
		{
			private TaskSync _predecessorLink;
			#region Predecessor fields
			private int _linkLag = 0;
			private int _type = -1;
			#endregion

			public TaskSync TaskRef
			{
				get { return _predecessorLink; }
				set { _predecessorLink = value; }
			}
			public int LinkLag
			{
				get { return _linkLag; }
				set { _linkLag = value; }
			}

			public int Type
			{
				get { return _type; }
				set { _type = value; }
			}

			#region IMsProjectSync Members

			public bool Validate()
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public void Recalculate(TaskSync origMsProjTask)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			#endregion
		}

		private class TaskSync : IMsProjectSync
		{
			#region Fields
			#region Recalculate fields
			private int _percentWorkComplete;
			private TimeSpan _actualWork;
			private TimeSpan _remainingWork;
			private TimeSpan _actualDuration;
			private TimeSpan _remainingDuration;
			private DateTime _stop;
			#endregion
			#region Field using in recalculate no but must be CONST
			private CompletionType _completionType = CompletionType.All;
			// it is the total amount of work scheduled to be performed by a
			// resource on a task
			private int _percentComplete;
			private TimeSpan _duration;
			private TimeSpan _work;
			private DateTime? _actualStart;
			private DateTime? _actualFinish;
			#endregion


			#region IBN<->MsProject field mapping
			//MsProject<UID> <-> IBN<ID>
			private int _msProjectMapValue;
			private int _ibnMapValue;
			#endregion

			private bool _isRecalculate = false;
			private bool _isMilestone;
			private bool _isSummary = false;
			private bool _isNull;

			private int _taskId;
			private int _taskNum;
			private int _outlineLevel;
			private int _intPriority;
			private int _durationFormat;
			private int _constraintType;
			private int _calendarId;

			private string _name;
			private string _outlineNumber;
			private string _notes;

			private DateTime _start;
			private DateTime _finish;

			//private DateTime _constraintDate;

			private List<PredecessorSync> _predecessorsList;
			private List<AssignmentSync> _assignmentsList;
			//Indexed values by key resource MsProjMapVal
			//for quick search in Import2;
			private Dictionary<int, AssignmentSync> _assignDict;
			private ProjectSyncContext _sandBox;

			#endregion

			#region Properties
			public CompletionType ComplType
			{
				get { return _completionType; }
				set { _completionType = value; }
			}
			public DateTime Stop
			{
				get { return _stop; }
				set { _stop = value; }
			}
			public int CalendarId
			{
				get { return _calendarId; }
				set { _calendarId = value; }
			}

			public List<PredecessorSync> Predecessors
			{
				get { return _predecessorsList; }
			}
			public List<AssignmentSync> Assignments
			{
				get { return _assignmentsList; }
			}

			public int MsProjectMapValue
			{
				get { return _msProjectMapValue; }
				set { _msProjectMapValue = value; }
			}

			public int IbnMapValue
			{
				get { return _ibnMapValue; }
				set { _ibnMapValue = value; }
			}

			public bool IsRecalculate
			{
				get { return _isRecalculate; }
				set { _isRecalculate = value; }
			}
			public bool IsNull
			{
				get { return _isNull; }
				set { _isNull = value; }
			}
			public bool IsSummary
			{
				get { return _isSummary; }
				set { _isSummary = value; }
			}
			public bool IsMilestone
			{
				get { return _isMilestone; }
				set { _isMilestone = value; }
			}
			/// <summary>
			/// Integer
			/// </summary>
			public int TaskId
			{
				get { return _taskId; }
				set { _taskId = value; }
			}
			public int TaskNum
			{
				get { return _taskNum; }
				set { _taskNum = value; }
			}
			public int OutlineLevel
			{
				get { return _outlineLevel; }
				set { _outlineLevel = value; }
			}
			public int IntPriority
			{
				get { return _intPriority; }
				set { _intPriority = value; }
			}
			public int DurationFormat
			{
				get { return _durationFormat; }
				set { _durationFormat = value; }
			}
			public int ConstraintType
			{
				get { return _constraintType; }
				set { _constraintType = value; }
			}

			/// <summary>
			/// Recalculate 
			/// </summary>
			public int PercentWorkComplete
			{
				get { return _percentWorkComplete; }
				set { _percentWorkComplete = value; }
			}
			public TimeSpan ActualDuration
			{
				get { return _actualDuration; }
				set { _actualDuration = value; }
			}
			public TimeSpan RemainingDuration
			{
				get { return _remainingDuration; }
				set { _remainingDuration = value; }
			}
			public DateTime? ActualStart
			{
				get { return _actualStart; }
				set { _actualStart = value; }
			}
			public DateTime? ActualFinish
			{
				get { return _actualFinish; }
				set { _actualFinish = value; }
			}

			public TimeSpan RemainingWork
			{
				get { return _remainingWork; }
				set { _remainingWork = value; /*Recalculate ?*/}
			}
			public TimeSpan ActualWork
			{
				get { return _actualWork; }
				set { _actualWork = value; /*Recalculate ?*/}
			}

			public int PercentComplete
			{
				get { return _percentComplete; }
				set { _percentComplete = value; }
			}
			/// <summary>
			/// String
			/// </summary>
			/// 
			public string Name
			{
				get { return _name; }
				set { _name = value; }
			}
			public string OutlineNumber
			{
				get { return _outlineNumber; }
				set { _outlineNumber = value; }
			}
			public string Notes
			{
				get { return _notes; }
				set { _notes = value; }
			}
			/// <summary>
			/// DateTime
			/// </summary>

			public DateTime Start
			{
				get { return _start; }
				set { _start = value; }
			}
			public DateTime Finish
			{
				get { return _finish; }
				set { _finish = value; }
			}
			/// <summary>
			/// TimeSpan
			///</summary>
			///
			public TimeSpan Duration
			{
				get { return _duration; }
				set { _duration = value; }
			}
			public TimeSpan Work
			{
				get { return _work; }
				set { _work = value; }
			}
			#endregion

			private TaskSync()
			{

				_predecessorsList = new List<PredecessorSync>();
				_assignmentsList = new List<AssignmentSync>();
				_assignDict = new Dictionary<int, AssignmentSync>();

				_actualDuration = TimeSpan.Zero;
				_actualWork = TimeSpan.Zero;
				_remainingWork = TimeSpan.Zero;
				_duration = TimeSpan.Zero;
				_work = TimeSpan.Zero;

			}


			/// <summary>
			/// Mses the project2 ibn priority.
			/// </summary>
			/// <param name="priority">The priority.</param>
			/// <returns></returns>
			public static Priority MsProject2IbnPriority(int priority)
			{
				Priority retVal = Priority.Normal;
				foreach (int eachVal in Enum.GetValues(typeof(Priority)))
				{
					if (priority >= eachVal)
						retVal = (Priority)eachVal;
				}
				return retVal;
			}

			/// <summary>
			/// Mses the type of the project2 ibn constraint.
			/// </summary>
			/// <param name="constraint">The constraint.</param>
			/// <returns></returns>
			public static ConstraintTypes MsProject2IbnConstraintType(int constraint)
			{
				if (Enum.IsDefined(typeof(ConstraintTypes), constraint))
					return (ConstraintTypes)Enum.Parse(typeof(ConstraintTypes),
													   constraint.ToString());

				return ConstraintTypes.AsSoonAsPossible;

			}

			public static TaskSync CreateFromXml(XmlNode node,
												 ProjectSyncContext projSandBox,
												 XmlNamespaceManager nsXmlMmngr,
												 int timeZoneId)
			{
				if (node == null)
					return null;

				#region Begin read Xml values

				int msProjTaskUID = (Int32)GetXmlValue(node["UID"], typeof(Int32), false);

				TaskSync retVal = projSandBox != null
									? projSandBox.GetTaskByMsProjMapVal(msProjTaskUID)
									: null;

				if (retVal != null)
					return retVal;

				retVal = new TaskSync();

				retVal.IsNull = (Boolean)GetXmlValue(node["IsNull"], typeof(Boolean));
				retVal.IsMilestone = (Boolean)GetXmlValue(node["Milestone"], typeof(Boolean));
				retVal.IsSummary = (Boolean)GetXmlValue(node["Summary"], typeof(Boolean));


				retVal.MsProjectMapValue = msProjTaskUID;
				retVal.TaskId = msProjTaskUID;
				retVal.TaskNum = (Int32)GetXmlValue(node["ID"], typeof(Int32));


				retVal.OutlineLevel = (Int32)GetXmlValue(node["OutlineLevel"],
														  typeof(Int32));
				retVal.IntPriority = (Int32)GetXmlValue(node["Priority"], typeof(Int32));
				retVal.ConstraintType = (Int32)GetXmlValue(node["ConstraintType"], typeof(Int32));

				// Calculate fields
				retVal.PercentComplete = (Int32)GetXmlValue(node["PercentComplete"],
															typeof(Int32), true, 0);
				retVal.ActualWork = (TimeSpan)GetXmlValue(node["ActualWork"],
															typeof(TimeSpan), true,
															TimeSpan.Zero);
				retVal.RemainingWork = (TimeSpan)GetXmlValue(node["RemainingWork"],
															typeof(TimeSpan), true,
															TimeSpan.Zero);
				retVal.Work = (TimeSpan)GetXmlValue(node["Work"], typeof(TimeSpan), true,
													TimeSpan.Zero);
				//End calculate fields

				retVal.Name = (String)GetXmlValue(node["Name"], typeof(String),
												  true, @"_________");
				retVal.Notes = (String)GetXmlValue(node["Notes"], typeof(String),
												   true, String.Empty);
				retVal.OutlineNumber = (string)GetXmlValue(node["OutlineNumber"],
														  typeof(String));


				retVal.ActualStart = (DateTime?)GetXmlValue(node["ActualStart"],
																  typeof(DateTime),
																  true, null);
				if (retVal.ActualStart != null)
					retVal.ActualStart = DBCommon.GetUTCDate(timeZoneId,
															 retVal.ActualStart.Value);

				retVal.ActualFinish = (DateTime?)GetXmlValue(node["ActualFinish"],
															 typeof(DateTime),
															 true, null);
				if (retVal.ActualFinish != null)
					retVal.ActualFinish = DBCommon.GetUTCDate(timeZoneId,
															  retVal.ActualFinish.Value);

				//TODO: Ask what set if not exist in xml
				retVal.Start = (DateTime)GetXmlValue(node["Start"],
															typeof(DateTime),
															true, DateTime.MinValue);
				retVal.Start = DBCommon.GetUTCDate(timeZoneId, retVal.Start);

				retVal.Finish = (DateTime)GetXmlValue(node["Finish"],
															 typeof(DateTime),
															 true, DateTime.MaxValue);
				retVal.Finish = DBCommon.GetUTCDate(timeZoneId, retVal.Finish);


				retVal.Duration = (TimeSpan)GetXmlValue(node["Duration"],
														typeof(TimeSpan), true,
														TimeSpan.Zero);
				retVal.ActualDuration = (TimeSpan)GetXmlValue(node["ActualDuration"],
															  typeof(TimeSpan), true,
															  TimeSpan.Zero);
				retVal.RemainingDuration = (TimeSpan)GetXmlValue(node["RemainingDuration"],
															  typeof(TimeSpan), true,
															  TimeSpan.Zero);
				#endregion


				#region Create Assignments from XML

				string assignXPath = string.Format("ns:Assignments/ns:Assignment[ns:TaskUID = '{0}']",
													msProjTaskUID);

				XmlNodeList assignNodeList = node.OwnerDocument.DocumentElement.SelectNodes(assignXPath, nsXmlMmngr);
				foreach (XmlNode assignNode in assignNodeList)
				{
					AssignmentSync assignment =
							AssignmentSync.CreateFromXML(assignNode, projSandBox,
														 nsXmlMmngr, timeZoneId);
					if (assignment != null)
					{
						retVal.Assignments.Add(assignment);
						retVal._assignDict.Add(assignment.Resource.MsProjectMapVal,
												assignment);
					}
				}
				#endregion

				#region Create Predecessors from XML
				XmlNodeList predNodeList = node.SelectNodes("ns:PredecessorLink", nsXmlMmngr);
				foreach (XmlNode predNode in predNodeList)
				{
					int predTaskUID = (Int32)GetXmlValue(predNode["PredecessorUID"], typeof(Int32), false);

					string predXPath = string.Format("ns:Tasks/ns:Task[ns:UID = '{0}']",
													 predTaskUID);
					XmlNode predTaskNode =
						node.OwnerDocument.DocumentElement.SelectSingleNode(predXPath,
																			nsXmlMmngr);
					if (predTaskNode != null)
					{
						PredecessorSync predecessor = new PredecessorSync();
						predecessor.TaskRef =
								TaskSync.CreateFromXml(predTaskNode, projSandBox,
													   nsXmlMmngr, timeZoneId);

						predecessor.Type = (Int32)GetXmlValue(predNode["Type"],
															  typeof(Int32),
															  true, 0/*Finish-Finish*/);
						predecessor.LinkLag = (Int32)GetXmlValue(predNode["LinkLag"],
																 typeof(Int32), true, 0);
						retVal.Predecessors.Add(predecessor);
					}

				}
				#endregion

				if (retVal.Validate())
				{
					if (projSandBox != null)
					{
						projSandBox.AddTask(retVal);
						retVal._sandBox = projSandBox;
					}
					return retVal;
				}

				return null;

			}

			/// <summary>
			/// Creates from data row.
			/// </summary>
			/// <param name="reader">The reader.</param>
			/// <param name="timeZoneId">The time zone id.</param>
			/// <returns></returns>
			public static TaskSync CreateFromDataRow(DataRow row, ProjectSyncContext projSandBox,
													 int timeZoneId)
			{
				if (row == null)
					return null;

				int msProjTaskUID = -1;
				if (row["MSProjectUID"] != DBNull.Value)
					msProjTaskUID = Convert.ToInt32(row["MSProjectUID"]);

				TaskSync retVal = projSandBox != null
									? projSandBox.GetTaskByMsProjMapVal(msProjTaskUID)
									: null;

				if (retVal != null)
					return retVal;

				retVal = new TaskSync();

				retVal.MsProjectMapValue = msProjTaskUID;
				retVal.IbnMapValue = Convert.ToInt32(row["TaskId"]);

				retVal.ActualStart = null;
				if (row["ActualStartDate"] != DBNull.Value)
					retVal.ActualStart = Convert.ToDateTime(row["ActualStartDate"]);

				retVal.ActualFinish = null;
				if (row["ActualFinishDate"] != DBNull.Value)
					retVal.ActualFinish = Convert.ToDateTime(row["ActualFinishDate"]);

				retVal.Notes = String.Empty;
				if (row["Description"] != DBNull.Value)
					retVal.Notes = Convert.ToString(row["Description"]);

				retVal.TaskId = retVal.IbnMapValue;
				retVal.TaskNum = Convert.ToInt32(row["TaskNum"]);
				retVal.Name = Convert.ToString(row["Title"]);

				retVal.Start = Convert.ToDateTime(row["StartDate"]);
				retVal.Finish = Convert.ToDateTime(row["FinishDate"]);
				retVal.Duration = new TimeSpan(Convert.ToInt32(row["Duration"]) * TimeSpan.TicksPerMinute);

				retVal.ConstraintType = Convert.ToInt32(row["ConstraintTypeId"]);
				retVal.IntPriority = Convert.ToInt32(row["PriorityId"]);
				retVal.PercentComplete = Convert.ToInt32(row["PercentCompleted"]);
				retVal.OutlineNumber = Convert.ToString(row["OutlineNumber"]);
				retVal.OutlineLevel = Convert.ToInt32(row["OutlineLevel"]);
				retVal.IsSummary = Convert.ToBoolean(row["IsSummary"]);
				retVal.IsMilestone = Convert.ToBoolean(row["IsMilestone"]);
				retVal.ComplType = (CompletionType)row["CompletionTypeId"];

				//retrieving assignments resources
				using (DataTable assignTable =
							DBTask.GetListResourcesDataTable(retVal.TaskId, timeZoneId))
				{
					foreach (DataRow assignRow in assignTable.Rows)
					{
						AssignmentSync assign =
								AssignmentSync.CreateFromDataRow(assignRow, projSandBox,
																 timeZoneId);

						if (assign != null)
						{
							//Для завершенных задач утсанавливаем процент завершения для назначений. 
							//Или при условии что completionType - Any
							if ((bool)row["IsCompleted"] && ((int)row["ReasonId"] == 1 || (int)row["ReasonId"] == 2))
							{
								assign.PercentWorkComplete = 100;
							}

							retVal._assignDict.Add(assign.Resource.MsProjectMapVal,
												   assign);

							retVal.Assignments.Add(assign);
						}
					}
				}
				//Retrieving predecessors from db
				using (DataTable predTable =
								DBTask.GetListPredecessorsDetailsDataTable(retVal.TaskId,
																		   timeZoneId))
				{
					foreach (DataRow predRow in predTable.Rows)
					{
						int predId = Convert.ToInt32(predRow["TaskId"]);
						DataTable predTaskTable = DBTask.GetTaskDataTable(predId, timeZoneId,
																		  Security.CurrentUser.LanguageId);
						if (predTaskTable.Rows == null)
							throw new Exception("Task structure invalid.");

						DataRow predTaskRow = predTaskTable.Rows[0];

						PredecessorSync predecessor = new PredecessorSync();
						predecessor.TaskRef =
									TaskSync.CreateFromDataRow(predTaskRow, projSandBox,
															   timeZoneId);

						predecessor.Type = Convert.ToInt32(predRow["CompletionTypeId"]);
						predecessor.LinkLag = Convert.ToInt32(predRow["Lag"]);

						retVal.Predecessors.Add(predecessor);

					}

				}
				if (retVal.Validate())
				{
					if ((projSandBox != null) && (msProjTaskUID != -1))
					{
						projSandBox.AddTask(retVal);
						retVal._sandBox = projSandBox;
					}
					return retVal;
				}

				return null;
			}

			/// <summary>
			/// Finds the assignment.
			/// By IbnMapValue
			/// </summary>
			/// <param name="otherAssign">The other assign.</param>
			/// <returns></returns>
			public AssignmentSync FindAssignment(AssignmentSync otherAssign)
			{
				AssignmentSync retVal = null;

				if (otherAssign == null)
					return null;

				_assignDict.TryGetValue(otherAssign.Resource.MsProjectMapVal,
										out retVal);

				return retVal;
			}


			#region IMsProjectSync Members

			public bool Validate()
			{
				//PercentComplete not set but Duration and RemainingDuration
				//is assigned and not equals
				//OpenProject format
				//if (this.PercentComplete == 0
				//   && this.Duration != this.RemainingDuration)
				//{
				//    this.ActualDuration = this.Duration - this.RemainingDuration;
				//    if (this.Duration != TimeSpan.Zero)
				//    {
				//        this.PercentComplete = (int)(this.ActualDuration.TotalMinutes
				//                                   / this.Duration.TotalMinutes * 100);
				//    }
				//}

				return (this.TaskId != 0);
			}

			public void Recalculate(TaskSync origMsProjTask)
			{
				if (IsRecalculate)
					return;

				//check const invariant
				//if (this.Duration != origMsProjTask.Duration)
				//    throw new Exception("Change const DURATION");
				this.Duration = origMsProjTask.Duration;

				this.Stop = this.Finish;
				double origWork = origMsProjTask.Work.TotalMinutes;
				long sumChildDuration = 0;
				long sumChildActualDuration = 0;

				if (this.IsSummary)
				{
					if (_sandBox == null)
						throw new Exception("missing reference to project items");
					List<TaskSync> childList = _sandBox.GetChildList(this);
					//В суммарных задачах Duration, ActualDuration являются пройденными (elapsed), соответсвенно
					// расчет процента завершения суммарной задачи:
					//PercentComplete = SUM(childTask.ActualDuration)/SUM(childTaskDuration)
					//А Stop time берется как самый ранний стоп из дочерних задач

					foreach (TaskSync ibnChildTask in childList)
					{
						TaskSync msProjChildTask =
								origMsProjTask._sandBox.GetTaskByMsProjMapVal(ibnChildTask.MsProjectMapValue);
						if (msProjChildTask == null)
							throw new Exception("invalid structure");

						ibnChildTask.Recalculate(msProjChildTask);
						this.Work += ibnChildTask.Work;
						this.ActualWork += ibnChildTask.ActualWork;
						sumChildActualDuration += ibnChildTask.ActualDuration.Ticks;
						sumChildDuration += ibnChildTask.Duration.Ticks;
						this.Stop = new DateTime(Math.Min(this.Stop.Ticks, ibnChildTask.Stop.Ticks));
					}
				}
				else
				{
					//Recalculate assignments (mapped and no mapped)
					foreach (AssignmentSync msProjAssign in origMsProjTask.Assignments)
					{
						//try find corresponding Ibn assign
						AssignmentSync ibnAssign =
												this.FindAssignment(msProjAssign);

						//Для типа завершения "All" завершаем по трудозатратам
						if (ibnAssign != null && this.ComplType == CompletionType.All)
						{
							//Завершения по трудозатратам
							ibnAssign.Recalculate(origMsProjTask);
						}
						else
						{
							//Завершение по времени для всех назначений из msProject не имеющих сопоставленных назначений в ibn 
							// или для задач имеющий тип завершения "Any".
							//Eсли в IBN нет соотв назначений, то изменяем  MsProject.Assignment 
							// данными необходимыми для получения корректных значений ActualWork и Stop
							//путем изменения ActualDuration по формуле actualDuration = assignment.Duration * task.PercentComplete
							ProjectBusinessUtil.Assignment.Assignment extAssign =
									 CreateMsProjectAssignment(this, msProjAssign);
							extAssign.ActualDuration = (long)(this.PercentComplete * 0.01 * CalendarHelper.Tick2Milis(this.Duration.Ticks));

							//изменяем существующий msProject assignment
							msProjAssign.Duration = origMsProjTask.Duration;
							msProjAssign.ActualWork = new TimeSpan(CalendarHelper.Milis2Tick(extAssign.GetWork(extAssign.Start, extAssign.Stop)));
							msProjAssign.RemainingWork = msProjAssign.Work - msProjAssign.ActualWork;
							msProjAssign.Stop = new DateTime(CalendarHelper.Milis2Tick(extAssign.Stop));
							msProjAssign.PercentComplete = (int)(extAssign.PercentComplete * 100);
							msProjAssign.PercentWorkComplete = (int)Math.Round((double)msProjAssign.ActualWork.Ticks / msProjAssign.Work.Ticks * 100);

							//Создаем timePhases
							msProjAssign.TimePhaseDataType.Clear();
							msProjAssign.TimePhaseDataType.AddRange(GetTimePhases(extAssign, msProjAssign.UID, ProjectBusinessUtil.Services.TimePhasedDataType.TimePhaseType.AssignmentRemainingWork));
							msProjAssign.TimePhaseDataType.AddRange(GetTimePhases(extAssign, msProjAssign.UID, ProjectBusinessUtil.Services.TimePhasedDataType.TimePhaseType.AssignmentActualWork));

							//recalculate resources
							msProjAssign.Resource.ActualWork += msProjAssign.ActualWork;
							msProjAssign.Resource.RemainingWork += msProjAssign.RemainingWork;
							msProjAssign.Resource.Recalculate(origMsProjTask);

							//добавляем измененный msProject.assignment в набор, для экспорта иначе msProject Assignment в файле останется неизмененный.
							this.Assignments.Add(msProjAssign);
							ibnAssign = msProjAssign;
						}

						this.ActualWork += ibnAssign.ActualWork;
						this.Work += ibnAssign.Work;
						sumChildActualDuration += (long)(ibnAssign.Duration.Ticks * ibnAssign.PercentComplete * 0.01);
						sumChildDuration += ibnAssign.Duration.Ticks;
						this.Stop = new DateTime(Math.Min(this.Stop.Ticks, ibnAssign.Stop.Ticks));
					}
				}


				if (sumChildDuration != 0)
				{
					this.PercentComplete = (int)Math.Round((double)sumChildActualDuration / sumChildDuration * 100);
				}

				this.RemainingWork = this.Work - this.ActualWork;
				this.ActualDuration = new TimeSpan((long)(this.Duration.Ticks * this.PercentComplete * 0.01));
				this.PercentWorkComplete = (int)Math.Round((double)this.ActualWork.Ticks / this.Work.Ticks * 100);

				this.RemainingDuration = this.Duration - this.ActualDuration;
				//Turn on flag Recalculate
				this.IsRecalculate = true;

			}

			#endregion
		}

		#region CalendarSync

		private class MsProjectCalendarSync : ProjectBusinessUtil.XmlSerialization.SerializableCalendar
		{
			public MsProjectCalendarSync(string xmlCalendar)
				: base(xmlCalendar)
			{
			}
			public int UID
			{
				get { return Convert.ToInt32(base.InnerCalendar.UID); }
			}

		};

		private class IBNCalendarSync : ProjectBusinessUtil.Calendar.WorkCalendar
		{
			public IBNCalendarSync(int IBNcalId)
			{
				LoadCalendarFromIBN(IBNcalId);
			}

			private void LoadCalendarFromIBN(int ibnCalId)
			{
				//1. Set work week

				for (byte dayOfWeek = 1; dayOfWeek < CalendarSettings.DayInWeek + 1; dayOfWeek++)
				{
					WorkDay workDay = new WorkDay();
					using (IDataReader reader = Calendar.GetListWeekdayHours(ibnCalId, dayOfWeek))
					{
						int intervalNumber = 0;
						while (reader.Read())
						{
							workDay.WorkingHours.AddInterval(intervalNumber++,
															CalendarHelper.MilisPerMinute() * (int)reader["FromTime"],
															CalendarHelper.MilisPerMinute() * (int)reader["ToTime"]);
						}
					}
					this.WorkingWeek.WeekDays[dayOfWeek == CalendarSettings.DayInWeek ? 0 : (int)dayOfWeek] = workDay;
				}

				Dictionary<int, WorkDay> exceptionDays = new Dictionary<int, WorkDay>();
				//2. Set Exceptions
				using (IDataReader reader = Calendar.GetListExceptions(ibnCalId))
				{
					while (reader.Read())
					{
						//Get begin day date
						long exceptionStartDate = CalendarHelper.DayOf(CalendarHelper.Tick2Milis(((DateTime)reader["FromDate"]).Ticks));
						//Whole day only
						WorkDay exceptionDay = new WorkDay(exceptionStartDate);
						exceptionDays.Add((int)reader["ExceptionId"], exceptionDay);
					}
				}
				foreach (KeyValuePair<int, WorkDay> exception in exceptionDays)
				{
					using (IDataReader reader = Calendar.GetListExceptionHours(exception.Key))
					{
						int intervalNumber = 0;
						while (reader.Read())
						{
							exception.Value.WorkingHours.AddInterval(intervalNumber++,
															CalendarHelper.MilisPerMinute() * (int)reader["FromTime"],
															CalendarHelper.MilisPerMinute() * (int)reader["ToTime"]);
						}
					}
				}
				//Populate calendar day exception
				this.DayException.AddRange(exceptionDays.Values);
			}

		};

		private class CalendarSyncFactory : WorkCalendarFactory, IFactoryMethod<IBNCalendarSync>, IFactoryMethod<MsProjectCalendarSync>
		{
			#region IFactoryMethod<CalendarSync> Members

			IBNCalendarSync IFactoryMethod<IBNCalendarSync>.Create(object obj)
			{
				int ibnCalendarId = (int)obj;
				IBNCalendarSync retVal = new IBNCalendarSync(ibnCalendarId);

				return retVal;
			}

			#endregion

			#region IFactoryMethod<MsProjectCalendarSync> Members

			public MsProjectCalendarSync Create(object obj)
			{
				XmlElement xmlEl = (XmlElement)obj;
				MsProjectCalendarSync retVal = new MsProjectCalendarSync(xmlEl.OuterXml);
				return retVal;
			}

			#endregion
		};
		#endregion

		private static TimePhasedDataType[] GetTimePhases(ProjectBusinessUtil.Assignment.Assignment assignment, int uid,
														  ProjectBusinessUtil.Services.TimePhasedDataType.TimePhaseType type)
		{

			List<TimePhasedDataType> retVal = new List<TimePhasedDataType>();
			foreach (ProjectBusinessUtil.Services.TimePhasedDataType extTimePhase in
								ProjectBusinessUtil.Services.TimePhasedService.GetTimePhaseData(assignment, type))
			{
				TimePhasedDataType timePhase = new TimePhasedDataType();
				timePhase.UID = uid;
				timePhase.Type = extTimePhase.Type;
				timePhase.Unit = (int)extTimePhase.Unit;
				timePhase.Value = new TimeSpan(CalendarHelper.Milis2Tick(extTimePhase.Value));
				timePhase.Start = new DateTime(CalendarHelper.Milis2Tick(extTimePhase.Start));
				timePhase.Finish = new DateTime(CalendarHelper.Milis2Tick(extTimePhase.Finish));
				retVal.Add(timePhase);
			}

			return retVal.ToArray();
		}

		/// <summary>
		/// Creates the ms project assignment from task and assignment
		/// </summary>
		/// <param name="origMsProjTask">The orig ms proj task.</param>
		/// <param name="origAssign">The orig assign.</param>
		/// <returns></returns>
		private static ProjectBusinessUtil.Assignment.Assignment CreateMsProjectAssignment(TaskSync origMsProjTask, AssignmentSync origAssign)
		{
			ProjectBusinessUtil.Assignment.Assignment retVal = null;
			//Start external assignment
			//Create calendar default calendar
			CalendarSyncFactory extFactory = new CalendarSyncFactory();
			WorkCalendar calendarResource = extFactory.Create<DefaultCalendar>(null);
			if (origAssign.Resource.ResourceCalendar != null)
			{
				calendarResource = origAssign.Resource.ResourceCalendar;
			}
			//Create workContour
			ContourTypes workContour = (ContourTypes)origAssign.WorkContour;
			ContourFactory contourFactory = new ContourFactory();
			AbstractContour contour = new PersonalContour();
			double units = origAssign.Units;
			ProjectBusinessUtil.Task.Task extTask = new ProjectBusinessUtil.Task.Task();
			extTask.Start = CalendarHelper.Tick2Milis(origMsProjTask.Start.Ticks);
			retVal = new Assignment(extTask, calendarResource, contour, units, 0);

			//if personal contour or in assignment not set finish date
			//if (workContour == ContourTypes.Contoured || origAssign.Finish == DateTime.MaxValue) // || (workContour != ContourTypes.Flat && origAssign.PercentWorkComplete != 0))
			//{
			if (origAssign.TimePhaseDataType.Count != 0)
			{
				List<ProjectBusinessUtil.Services.TimePhasedDataType> extTimePhases =
												new List<ProjectBusinessUtil.Services.TimePhasedDataType>();
				//create personal contour
				foreach (TimePhasedDataType timePhase in origAssign.TimePhaseDataType)
				{
					ProjectBusinessUtil.Services.TimePhasedDataType extTimephase = new ProjectBusinessUtil.Services.TimePhasedDataType();
					extTimephase.UID = timePhase.UID;
					extTimephase.Unit = (ProjectBusinessUtil.Services.TimePhasedDataType.TimePhaseUnit)timePhase.Unit;
					extTimephase.Value = CalendarHelper.Tick2Milis(timePhase.Value.Ticks);
					extTimephase.Start = CalendarHelper.Tick2Milis(timePhase.Start.Ticks);
					extTimephase.Finish = CalendarHelper.Tick2Milis(timePhase.Finish.Ticks);
					extTimePhases.Add(extTimephase);
				}

				contour = contourFactory.Create<PersonalContour>
														(new KeyValuePair<Assignment,
														 ProjectBusinessUtil.Services.TimePhasedDataType[]>(retVal, extTimePhases.ToArray()));
			}
			else
			{
				contour = contourFactory.Create<StandardContour>(workContour);
			}

			retVal = new Assignment(extTask, calendarResource, contour, units, 0);
			if (origAssign.Finish == DateTime.MaxValue)
			{
				//Используем длительность для назначений  взятую из рабочего контура(Personal)
				retVal.Duration = contour.CalcSumBucketDuration(0);

			}
			else
			{
				//косвенно устанавливаем длительность путем установки даты завершения назначения
				retVal.End = CalendarHelper.Tick2Milis(origAssign.Finish.Ticks);
			}


			return retVal;
		}


		#region Utility
		/// <summary>
		/// Gets the child task list.
		/// </summary>
		/// <param name="taskSyncList">The task sync list.</param>
		/// <param name="parent">The parent.</param>
		/// <returns></returns>
		static private List<TaskSync> GetChildTaskList(ICollection<TaskSync> taskSyncList, TaskSync parent)
		{
			List<TaskSync> retVal = new List<TaskSync>();
			string matchCond = String.Format("{0}.", parent.OutlineNumber);
			foreach (TaskSync task in taskSyncList)
			{
				if ((task.OutlineLevel - parent.OutlineLevel) == 1)
				{
					if (task.OutlineNumber.StartsWith(matchCond))
						retVal.Add(task);
				}
			}

			return retVal;
		}
		/// <summary>
		/// Adds the XML value.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="name">The name.</param>
		/// <param name="valType">Type of the val.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>

		static private XmlNode AddXmlValue(XmlNode node, string name, Type valType,
												 object value)
		{
			if (node == null)
				return null;

			XmlNode newChild = node.OwnerDocument.CreateElement(name);
			SetXmlValue(newChild, valType, value);
			if (newChild != null)
				node.AppendChild(newChild);

			return newChild;
		}

		/// <summary>
		/// Sets the XML value.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="valType">Type of the val.</param>
		/// <param name="value">The value.</param>
		static private void SetXmlValue(XmlNode node, Type valType, object value)
		{
			if (node == null)
				return;
			try
			{
				if (valType == typeof(TimeSpan))
				{
					TimeSpan timeSpan = (TimeSpan)value;
					long tick = timeSpan.Ticks;
					double second = (double)(tick % TimeSpan.TicksPerMinute) / TimeSpan.TicksPerSecond;
					node.InnerText = String.Format("PT{0}H{1}M{2}S", tick / TimeSpan.TicksPerHour,
																	 (tick % TimeSpan.TicksPerHour) / TimeSpan.TicksPerMinute,
																	  second - (int)second == 0 ? ((int)second).ToString()
																								: second.ToString(CultureInfo.InvariantCulture));

				}
				else if ((valType == typeof(DateTime)) || (valType == typeof(DateTime?)))
				{
					if (valType != null)
					{
						DateTime dateTime = Convert.ToDateTime(value);
						node.InnerText = dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
					}
				}
				else if (valType == typeof(String))
				{
					node.InnerText = Convert.ToString(value);
				}
				else if (valType == typeof(Int32))
				{
					int intVal = Convert.ToInt32(value);
					node.InnerText = intVal.ToString();
				}
				else if (valType == typeof(Boolean))
				{
					Boolean bolVal = Convert.ToBoolean(value);
					node.InnerText = bolVal.ToString();
				}
				else if (valType == typeof(double))
				{
					double doubleVal = Convert.ToDouble(value);
					node.InnerText = doubleVal.ToString();
				}
				else
				{
					throw new ArgumentException("unsupported type");
				}
			}
			catch (InvalidCastException)
			{
				throw;
			}
		}
		/// <summary>
		/// Gets the XML value.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="valType">Type of the val.</param>
		/// <returns></returns>
		static private object GetXmlValue(XmlNode node, Type valType)
		{
			return GetXmlValue(node, valType, true, null);
		}

		/// <summary>
		/// Gets the XML value.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="valType">Type of the val.</param>
		/// <param name="optional">if set to <c>true</c> [optional].</param>
		/// <returns></returns>
		static private object GetXmlValue(XmlNode node, Type valType, bool optional)
		{
			return GetXmlValue(node, valType, optional, null);
		}

		/// <summary>
		/// Gets the XML value.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="valType">Type of the val.</param>
		/// <param name="optional">if set to <c>true</c> [optional].</param>
		/// <param name="defaultVal">The default val.</param>
		/// <returns></returns>
		static private object GetXmlValue(XmlNode node, Type valType,
									bool optional, object defaultVal)
		{
			object retVal = defaultVal;

			if (node == null)
			{
				if (optional)
					return retVal;

				throw new ArgumentException("must presents");
			}

			if (valType == typeof(TimeSpan))
			{

				Regex pattern = new Regex(@"^PT(?<H>[0-9]+)H(?<M>[0-9]+)"
										 + @"M(?<S>(?:[0-9]*\.)?[0-9]+)S");
				Match match = pattern.Match(node.InnerText);

				if (match.Success)
				{
					int hours = Int32.Parse(match.Groups["H"].Value);
					int min = Int32.Parse(match.Groups["M"].Value);
					double second = Double.Parse(match.Groups["S"].Value, CultureInfo.InvariantCulture);
					int sec = (Int32)second;
					int milis = (Int32)((second - sec) * 1000);
					retVal = new TimeSpan(0, hours, min, sec, milis);
				}

			}
			else if (valType == typeof(DateTime))
			{
				retVal = XmlConvert.ToDateTime(node.InnerText,
											   XmlDateTimeSerializationMode.Utc);
			}
			else if (valType == typeof(String))
			{
				retVal = node.InnerText;
			}
			else if (valType == typeof(Int32))
			{
				retVal = XmlConvert.ToInt32(node.InnerText);
			}
			else if (valType == typeof(Boolean))
			{
				retVal = XmlConvert.ToBoolean(node.InnerText);
			}
			else if (valType == typeof(double))
			{
				retVal = XmlConvert.ToDouble(node.InnerText);
			}
			else
			{
				throw new ArgumentException("unsupported type");
			}

			return retVal;
		}
		#endregion

		#region FileStorage utility
		/// <summary>
		/// Gets the ms project file id.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <returns></returns>
		[Obsolete]
		static private int GetMsProjectFileId(int projectId)
		{
			int retVal = -1;
			using (IDataReader reader = DBProject.GetProject(projectId,
															User.DefaultTimeZoneId,
															Security.CurrentUser.LanguageId))
			{
				if (reader.Read())
				{
					if (reader["XMLFileId"] != null)
						retVal = Convert.ToInt32(reader["XMLFileId"]);
				}
			}
			return retVal;
		}
		/// <summary>
		/// Gets the file storage.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <returns></returns>
		static private FileStorage GetFileStorage(int projectId)
		{
			System.IO.MemoryStream memStream = new System.IO.MemoryStream();
			string containerName = "TemporaryStorage";
			string containerKey = "TmpProjectId_" + projectId.ToString();

			BaseIbnContainer bic = BaseIbnContainer.Create(containerName, containerKey);

			return (FileStorage)bic.LoadControl("FileStorage");
		}


		/// <summary>
		/// Deletes the ms project from file storage.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <param name="dataFileId">The data file id.</param>
		static private void DeleteTmpProjectFile(int projectId, int dataFileId)
		{
			FileStorage fs = GetFileStorage(projectId);

			fs.DeleteFile(dataFileId);

			//DBProject.UpdateXMLFileId(projectId, -1);
		}

		/// <summary>
		/// Gets the ms project file stream.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <returns></returns>
		static private MemoryStream GetMsProjectFileStream(int projectId, int dataFileId)
		{
			FileStorage fs = GetFileStorage(projectId);
			//int msProjectFileId = GetMsProjectFileId(projectId);
			MemoryStream memStream = new MemoryStream();

			if (dataFileId != -1)
			{
				fs.LoadFile(dataFileId, memStream);

				memStream.Seek(0, SeekOrigin.Begin);
			}

			return memStream;
		}


		#endregion

		#region GetCalendarsFromXml
		static private Hashtable GetProjectCalendarFromXml(XmlElement msprojXmlDoc)
		{
			XmlNamespaceManager mngr =
				   new XmlNamespaceManager(msprojXmlDoc.OwnerDocument.NameTable);
			mngr.AddNamespace("ns", "http://schemas.microsoft.com/project");

			Hashtable calList = new Hashtable();
			XmlNode calNode = null;

			int calUID = (int)GetXmlValue(msprojXmlDoc["CalendarUID"], typeof(Int32), true, -1);

			while (calUID != -1)
			{
				string xPath =
						   String.Format("ns:Calendars/ns:Calendar[ns:UID = '{0}']", calUID);

				calNode = msprojXmlDoc.SelectSingleNode(xPath, mngr);

				if (calNode == null)
					return null;

				string calName = (string)GetXmlValue(calNode["Name"], typeof(String), true, "unnamed");

				calList.Add(calName, calUID);

				if ((bool)GetXmlValue(calNode["IsBaseCalendar"], typeof(Boolean), true, true))
					break;

				calUID = (int)GetXmlValue(calNode["BaseCalendarUID"], typeof(Int32), true, -1);
			}

			return calList;
		}
		#endregion

		#region UpdateTaskInIBN
		private static void UpdateTaskInIBN(int ibnTaskId, TaskSync task,
											CompletionType complType)
		{


			DBTask.Update(ibnTaskId, task.Name, task.Notes, task.Start,
						 task.Finish, task.ActualStart, task.ActualFinish, (int)task.Duration.TotalMinutes,
						 (int)TaskSync.MsProject2IbnPriority(task.IntPriority),
						 (int)TaskSync.MsProject2IbnConstraintType(task.ConstraintType),
						 (int)ActivationTypes.AutoWithCheck, (int)complType, task.TaskNum, task.IsMilestone,
						 task.IsSummary, task.OutlineNumber, task.OutlineLevel,
						 task.PercentComplete, (task.PercentComplete == 100)
									? (int)CompletionReason.CompletedManually
									: (int)CompletionReason.NotCompleted, task.MsProjectMapValue);
		}
		#endregion
		#region AddTaskToIBN
		private static int AddTaskToIBN(TaskSync task, int ibnProjectId, int userId,
										 DateTime curDate, CompletionType complType)
		{

			return DBTask.CreateFromExport2(ibnProjectId, task.Name, task.Notes, userId,
												curDate, task.Start, task.Finish, task.ActualStart,
												task.ActualFinish, (int)task.Duration.TotalMinutes,
												(int)TaskSync.MsProject2IbnPriority(task.IntPriority),
												(int)TaskSync.MsProject2IbnConstraintType(task.ConstraintType),
												(int)ActivationTypes.AutoWithCheck,
												(int)complType, task.TaskNum, task.IsMilestone,
												task.IsSummary, task.OutlineNumber, task.OutlineLevel,
												task.PercentComplete, (task.PercentComplete == 100)
														   ? (int)CompletionReason.CompletedManually
														   : (int)CompletionReason.NotCompleted, task.MsProjectMapValue);

		}

		#endregion
		#region DeleteTaskFromIBN
		private static void DeleteTaskFromIBN(TaskSync task)
		{
			DeleteSimple(task.TaskId);
		}
		private static void DeleteAllTaskInProject(int IBNProjectID)
		{

			ArrayList toDelete = new ArrayList();
			using (IDataReader oldTaskReader =
								DBTask.GetListTasksByProject(IBNProjectID, Security.CurrentUser.LanguageId,
															 Security.CurrentUser.TimeZoneId))
			{
				while (oldTaskReader.Read())
				{
					toDelete.Add(Convert.ToInt32(oldTaskReader["taskId"]));
				}
			}

			foreach (int taskId in toDelete)
			{
				DeleteSimple(taskId);
			}

		}
		#endregion

		#region CreateCalendar

		static private int CreateCalendar(int ibnProjectId, XmlElement msprojXmlDoc)
		{
			Hashtable calList = GetProjectCalendarFromXml(msprojXmlDoc);
			XmlNode calNode = null;

			//TODO: remove further
			string calName = "default";

			foreach (string key in calList.Keys)
				calName = key;

			XmlNamespaceManager mngr =
						 new XmlNamespaceManager(msprojXmlDoc.OwnerDocument.NameTable);
			mngr.AddNamespace("ns", @"http://schemas.microsoft.com/project");

			int calendarID = Calendar.Create(ibnProjectId, calName);
			DbProject2.UpdateCalendar(ibnProjectId, calendarID);

			foreach (int calUID in calList.Values)
			{
				calNode = msprojXmlDoc.SelectSingleNode(string.Format("ns:Calendars/ns:Calendar[ns:UID = '{0}']", calUID), mngr);
				XmlNodeList WeekDays = calNode.SelectNodes("ns:WeekDays/ns:WeekDay", mngr);
				//If calendar not valid (without WeekDays) creation 24 hours / 7 day work week
				if (WeekDays.Count == 0)
				{
					int i = 1;
					while (i < 8)
					{
						Calendar.UpdateWeekdayHoursInternal(calendarID, (byte)i, 0, 0, -1, -1, -1, -1, -1, -1, -1, -1);
						i++;
					}

				}
				else
					foreach (XmlNode WeekDay in WeekDays)
					{
						XmlNode DayTypeNode = WeekDay.SelectSingleNode("ns:DayType", mngr);
						XmlNode DayWorkingNode = WeekDay.SelectSingleNode("ns:DayWorking", mngr);

						if (DayTypeNode == null || DayWorkingNode == null)
							throw new Exception("wrong xml");

						int DayType = (int)GetXmlValue(DayTypeNode, typeof(Int32));
						int DayWorking = (int)GetXmlValue(DayWorkingNode, typeof(Int32));

						if (DayType >= 1 && DayType <= 7)
						{
							DayType = DayType > 1 ? DayType - 1 : 7;

							if (DayWorking == 0)
							{
								Calendar.UpdateWeekdayHoursInternal(calendarID, (byte)DayType, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1);
							}
							else
								if (DayWorking == 1)
								{
									ArrayList list = new ArrayList();

									/*
										<WorkingTimes>
											<WorkingTime>
												<FromTime />
												<ToTime />
											</WorkingTime>
										</WorkingTimes>
										*/

									XmlNodeList WorkingTimeNodes = WeekDay.SelectNodes("ns:WorkingTimes/ns:WorkingTime", mngr);
									int counter = 0;
									foreach (XmlNode WorkingTimeNode in WorkingTimeNodes)
									{
										XmlNode FromNode = WorkingTimeNode["FromTime"];
										XmlNode ToNode = WorkingTimeNode["ToTime"];

										if (FromNode == null || ToNode == null)
											throw new Exception("wrong xml");

										DateTime FromTime = (DateTime)GetXmlValue(FromNode, typeof(DateTime));

										DateTime ToTime = (DateTime)GetXmlValue(ToNode, typeof(DateTime));
										//If end time is 0, is midnight the next day
										// 13.11.2008 et: fix 
										int earlyTime = (int)FromTime.TimeOfDay.TotalMinutes;
										int laterTime = ToTime.TimeOfDay == TimeSpan.Zero ? 60 * 24 : (int)ToTime.TimeOfDay.TotalMinutes;
										list.Add(earlyTime);
										list.Add(laterTime);

										counter += 2;
									}

									if (counter > 10)
										throw new Exception("wrong xml");

									for (int k = counter; k < 10; k++)
										list.Add(-1);

									Calendar.UpdateWeekdayHoursInternal(calendarID, (byte)DayType, (int)list[0], (int)list[1], (int)list[2], (int)list[3], (int)list[4],
										(int)list[5], (int)list[6], (int)list[7], (int)list[8], (int)list[9]);
								}
								else
									throw new Exception("wrong xml");
						}
						else
							if (DayType == 0)
							{
								/*
										<TimePeriod>
											<FromDate />
											<ToDate />
										</TimePeriod>
									*/
								XmlNode FromDateNode = WeekDay.SelectSingleNode("ns:TimePeriod/ns:FromDate", mngr);
								XmlNode ToDateNode = WeekDay.SelectSingleNode("ns:TimePeriod/ns:ToDate", mngr);

								if (FromDateNode == null || ToDateNode == null)
									throw new Exception("wrong xml");

								DateTime FromDate = (DateTime)GetXmlValue(FromDateNode, typeof(DateTime));

								DateTime ToDate = (DateTime)GetXmlValue(ToDateNode, typeof(DateTime));

								if (FromDate > ToDate)
									throw new Exception("wrong xml");

								if (DayWorking == 0)
								{
									Calendar.CreateExceptionInternal(calendarID, FromDate, ToDate, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1);
								}
								else
									if (DayWorking == 1)
									{
										ArrayList list = new ArrayList();

										/*
													<WorkingTimes>
														<WorkingTime>
															<FromTime />
															<ToTime />
														</WorkingTime>
													</WorkingTimes>
													*/

										XmlNodeList WorkingTimeNodes = WeekDay.SelectNodes("ns:WorkingTime", mngr);
										int counter = 0;
										foreach (XmlNode WorkingTimeNode in WorkingTimeNodes)
										{
											XmlNode FromNode = WorkingTimeNode["FromTime"];
											XmlNode ToNode = WorkingTimeNode["ToTime"];

											if (FromNode == null || ToNode == null)
												throw new Exception("wrong xml");

											DateTime FromTime = (DateTime)GetXmlValue(FromNode, typeof(DateTime));

											DateTime ToTime = (DateTime)GetXmlValue(ToNode, typeof(DateTime));

											//If end time is 0, is midnight the next day
											// 13.11.2008 et: fix 
											int earlyTime = (int)FromTime.TimeOfDay.TotalMinutes;
											int laterTime = ToTime.TimeOfDay == TimeSpan.Zero ? 60 * 24 : (int)ToTime.TimeOfDay.TotalMinutes;
											list.Add(earlyTime);
											list.Add(laterTime);
											counter += 2;
										}

										if (counter > 10)
											throw new Exception("wrong xml");

										for (int k = counter; k < 10; k++)
											list.Add(-1);

										Calendar.CreateExceptionInternal(calendarID, FromDate, ToDate, (int)list[0], (int)list[1], (int)list[2], (int)list[3], (int)list[4],
											(int)list[5], (int)list[6], (int)list[7], (int)list[8], (int)list[9]);
									}
									else
										throw new Exception("wrong xml");
							}
							else
								throw new Exception("wrong xml");
					}
			}

			return calendarID;

		}
		#endregion


		public static void TasksImport2(DataTable resMapping, int project_id,
										 int dataFileId, CompletionType complType,
										 bool ignoreDiffProject)
		{
			//Licensing support
			if (!Mediachase.Ibn.License.MSProjectSynchronization)
			{
				throw new FeatureNotAvailable();
			}

			DateTime currDate = DateTime.UtcNow;
			int managerId = DBProject.GetProjectManager(project_id);
			int userId = Security.CurrentUser.UserID;
			int timeZoneId = Security.CurrentUser.TimeZoneId;
			int languageId = Security.CurrentUser.LanguageId;

			//Flag define the first import 
			bool alreadySync = false;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				using (IDataReader reader = DBProject.GetProject(project_id, User.DefaultTimeZoneId,
																Security.CurrentUser.LanguageId))
				{
					if (reader.Read())
					{
						alreadySync = (bool)reader["IsMsProject"];
					}
				}

				XmlDocument msprjXmlDoc = new XmlDocument();

				using (MemoryStream memStream = GetMsProjectFileStream(project_id, dataFileId))
				{
					msprjXmlDoc.Load(memStream);
				}

				XmlNamespaceManager nsXmlMmngr =
						 new XmlNamespaceManager(msprjXmlDoc.NameTable);

				nsXmlMmngr.AddNamespace("ns", @"http://schemas.microsoft.com/project");


				#region Create Calendar
				CreateCalendar(project_id, msprjXmlDoc.DocumentElement);
				#endregion

				#region Read Tasks from MsProject XML
				ProjectSyncContext msProjSandBox =
								new ProjectSyncContext(resMapping);
				Dictionary<int, TaskSync> msProjTasks =
											new Dictionary<int, TaskSync>();
				XmlNodeList taskNodeList =
							msprjXmlDoc.DocumentElement.SelectNodes("ns:Tasks/ns:Task",
																	nsXmlMmngr);
				foreach (XmlNode taskNode in taskNodeList)
				{
					TaskSync task = TaskSync.CreateFromXml(taskNode, msProjSandBox,
														   nsXmlMmngr, timeZoneId);
					if (task != null)
						msProjTasks.Add(task.MsProjectMapValue, task);
				}

				//OpenProj fix. OpenProj always set Summery is 0. 
				foreach (TaskSync fixTask in msProjTasks.Values)
				{
					if (fixTask.IsSummary == false)
						fixTask.IsSummary = GetChildTaskList(msProjTasks.Values, fixTask).Count != 0;
				}

				#endregion

				#region Read Tasks from IBN
				ProjectSyncContext ibnProjsandBox =
										new ProjectSyncContext();
				Dictionary<int, TaskSync> ibnTasks =
												 new Dictionary<int, TaskSync>();
				using (DataTable ibnTaskTable =
								DBTask.GetListTasksByProjectDataTable(project_id,
																	  languageId, timeZoneId))
				{
					foreach (DataRow row in ibnTaskTable.Rows)
					{
						TaskSync ibnTask =
									TaskSync.CreateFromDataRow(row, ibnProjsandBox,
															   timeZoneId);

						if ((ibnTask != null))
						{
							//If already sync then not need mapping keys
							ibnTasks.Add(alreadySync ? ibnTask.MsProjectMapValue
													   : ibnTask.IbnMapValue, ibnTask);
						}
					}

				}

				#endregion

				//Check difference imported project and already exist sync project
				if (alreadySync == true)
				{
					int projDifference = ProjectSyncContext.EqualityProjectComparer(msProjSandBox,
																				ibnProjsandBox);
					if (projDifference > 80) //difference great that 80%
					{
						if (!ignoreDiffProject)
							throw new FoundDifferenceSynchronizedProjectException();

					}
				}

				foreach (TaskSync msProjTask in msProjTasks.Values)
				{
					//Skip Null tasks
					if (msProjTask.IsNull == true)
						continue;

					TaskSync ibnTask = null;
					if (alreadySync && ibnTasks.TryGetValue(msProjTask.TaskId, out ibnTask))
					{
						//Mapping task IBN <-> MsProject UID
						msProjTask.IbnMapValue = ibnTask.TaskId;
						UpdateTaskInIBN(msProjTask.IbnMapValue, msProjTask, complType);
						//To remove from the container intended for removal 
						ibnTasks.Remove(msProjTask.TaskId);
					}
					else
					{
						msProjTask.IbnMapValue = AddTaskToIBN(msProjTask, project_id,
															 userId, currDate, complType);
					}

					#region Sync assignments (resources)

					foreach (AssignmentSync msProjAssign in msProjTask.Assignments)
					{
						bool canManage = (msProjAssign.Resource.IbnMapVal == managerId);

						//calculate percent for each resource from total task percentComplete value
						int percentComplete = msProjTask.PercentWorkComplete;
						if (complType == CompletionType.All)
						{
							percentComplete = msProjAssign.PercentWorkComplete;
						}

						AssignmentSync ibnAssign = null;
						//try to find corresponding Ibn assignment
						if (ibnTask != null)
						{
							ibnAssign = ibnTask.FindAssignment(msProjAssign);
						}

						if (ibnAssign != null)
						{
							DBTask.UpdateResourceForMSProject(msProjTask.IbnMapValue,
															  msProjAssign.Resource.IbnMapVal,
															  msProjAssign.Resource.MsProjectMapVal,
															  percentComplete,
															  msProjAssign.ActualFinish);
							//Exclude item from removal queue
							ibnTask.Assignments.Remove(ibnAssign);
						}
						else
						{
							//Add or change Assignment in IBN
							DBTask.AddResourceForMSProject(msProjTask.IbnMapValue,
														   msProjAssign.Resource.IbnMapVal,
														   msProjAssign.Resource.MsProjectMapVal,
														   percentComplete, msProjAssign.ActualFinish);

							if (User.IsExternal(msProjAssign.Resource.IbnMapVal))
								DBCommon.AddGate((int)TASK_TYPE, msProjTask.IbnMapValue,
												  msProjAssign.Resource.IbnMapVal);
						}


					}
					//remove resources not assigned to task
					if (ibnTask != null)
					{
						foreach (AssignmentSync toRemove in ibnTask.Assignments)
						{
							DBTask.DeleteResource(msProjTask.IbnMapValue, toRemove.Resource.IbnMapVal);
						}
					}

					#endregion

					// ET 14.04.2008
					//Apply security settings for new task
					ApplySecurityOnTaskImported(project_id, msProjTask.IbnMapValue);
				}


				//remove ibnTask not contains to MsProject xml file
				foreach (TaskSync toRemove in ibnTasks.Values)
				{
					DeleteTaskFromIBN(toRemove);
				}

				#region Sync predecessors
				//Remove all task link
				DBTask.DeleteTaskLinksByProject(project_id);
				//Create new Task link
				foreach (TaskSync successor in msProjTasks.Values)
					foreach (PredecessorSync predecessor in successor.Predecessors)
					{
						if (predecessor.Type == 1)
						{
							if (DBTask.TaskInAllDependOnTasks(successor.IbnMapValue,
															  predecessor.TaskRef.IbnMapValue))
								throw new Exception("from task to dependent on task");

							DBTask.CreateTaskLink(predecessor.TaskRef.IbnMapValue,
												  successor.IbnMapValue,
												  predecessor.LinkLag);
						}

					}
				#endregion

				//Save orig msProject xml file to IBN project
				using (SqlBlobStream dstStream =
						DBProject.GetMSProjectXMLStream(project_id, SqlBlobAccess.Write))
				{
					using (MemoryStream memStream = new MemoryStream())
					{
						msprjXmlDoc.Save(memStream);
						byte[] buffer = memStream.GetBuffer();
						dstStream.Write(buffer, 0, buffer.Length);
					}

				}
				//Remove XML File from tmp file storage
				DeleteTmpProjectFile(project_id, dataFileId);

				//Turn on flag, this project now is MsProject synchronized
				DBProject.UpdateIsMSProject(project_id, true);


				RecalculateAllStates2(project_id);

				// Recalculating project TaskTime;
				TimeTracking.RecalculateProjectTaskTime(project_id);


				//Update resource mapping (save user resource mapping)
				foreach (DataRow row in resMapping.Rows)
				{
					int msProjectResourceId = (int)row["ResourceId"];
					int ibnPrincipalId = (int)row["PrincipalId"];
					DbProject2.UpdateMsProjectResourceId(project_id, ibnPrincipalId, msProjectResourceId);
				}


				tran.Commit();
			}
		}


		public static XmlDocument TaskExport2(int projectId)
		{
			//Licensing support
			if (!Mediachase.Ibn.License.MSProjectSynchronization)
			{
				throw new FeatureNotAvailable();
			}

			int languageId = Security.CurrentUser.LanguageId;
			int timeZoneId = User.DefaultTimeZoneId;

			XmlDocument msprjXmlDoc = new XmlDocument();
			ProjectSyncContext projectSyncContext = new ProjectSyncContext();

			using (DbTransaction tran = DbTransaction.Begin())
			{
				//Try load MsProject xml file attached to IBN Project
				using (IDataReader reader = DBProject.GetProject(projectId, timeZoneId,
																 Security.CurrentUser.LanguageId))
				{
					if (reader.Read())
					{
						if (!(bool)reader["IsMsProject"])
							throw new Exception("Is not MsProject sync");

					}
				}

				using (SqlBlobStream sqlStream =
							   DBProject.GetMSProjectXMLStream(projectId, SqlBlobAccess.Read))
				{
					msprjXmlDoc.Load(sqlStream);
				}

				XmlNamespaceManager nsXmlMmngr =
							 new XmlNamespaceManager(msprjXmlDoc.NameTable);
				nsXmlMmngr.AddNamespace("ns", @"http://schemas.microsoft.com/project");

				int calendarId = DBProject.GetCalendar(projectId);

				using (IDataReader reader = DBCalendar.GetCalendar(calendarId))
				{
					if (reader.Read())
					{
						timeZoneId = (int)reader["TimeZoneId"];
					}
				}

				#region Read Calendars from original MsProject Xml
				XmlNodeList calNodeList =
						   msprjXmlDoc.DocumentElement.SelectNodes("ns:Calendars/ns:Calendar",
																   nsXmlMmngr);
				CalendarSyncFactory extFactory = new CalendarSyncFactory();

				//Create first base calendar
				foreach (XmlNode calendarNode in calNodeList)
				{
					bool isBaseCalendar = (bool)GetXmlValue(calendarNode["IsBaseCalendar"], typeof(bool), true, false);
					if (isBaseCalendar)
					{
						MsProjectCalendarSync extCalendar = extFactory.Create<MsProjectCalendarSync>(calendarNode);
						projectSyncContext.MsProjectCalendarList.Add(extCalendar);
					}
				}
				//Create inherits calendars
				foreach (XmlNode calendarNode in calNodeList)
				{
					bool isBaseCalendar = (bool)GetXmlValue(calendarNode["IsBaseCalendar"], typeof(bool), true, false);

					if (isBaseCalendar == false)
					{
						int baseCalendarUID = (int)GetXmlValue(calendarNode["BaseCalendarUID"], typeof(int), false, -1);
						XmlNode baseCalNode = msprjXmlDoc.DocumentElement.SelectSingleNode(string.Format("ns:Calendars/ns:Calendar[ns:UID = '{0}']",
																			  baseCalendarUID), nsXmlMmngr);
						if (baseCalNode == null)
							throw new XmlException("Invalid calendar XML definition");

						MsProjectCalendarSync baseCalendar = extFactory.Create<MsProjectCalendarSync>(baseCalNode);
						MsProjectCalendarSync extCalendar = extFactory.Create<MsProjectCalendarSync>(calendarNode);

						if (baseCalendar != null)
							extCalendar.BaseCalendar = baseCalendar;

						projectSyncContext.MsProjectCalendarList.Add(extCalendar);
					}
				}
				#endregion

				#region Read Tasks from original MsProject XML
				Dictionary<int, TaskSync> msProjTasks = new Dictionary<int, TaskSync>();
				XmlNodeList taskNodeList =
							msprjXmlDoc.DocumentElement.SelectNodes("ns:Tasks/ns:Task",
																	nsXmlMmngr);
				foreach (XmlNode taskNode in taskNodeList)
				{
					TaskSync task = TaskSync.CreateFromXml(taskNode, projectSyncContext,
														   nsXmlMmngr, timeZoneId);

					if (task != null)
					{
						task.CalendarId = calendarId;
						msProjTasks.Add(task.MsProjectMapValue, task);
					}
				}
				#endregion

				#region Read tasks from IBN and recalculate changed fields

				ProjectSyncContext ibnProjSandBox = new ProjectSyncContext();
				Dictionary<int, TaskSync> ibnTasks =
											  new Dictionary<int, TaskSync>();

				using (DataTable ibnTaskTable =
								DBTask.GetListTasksByProjectDataTable(projectId,
																	  languageId, timeZoneId))
				{
					foreach (DataRow row in ibnTaskTable.Rows)
					{
						TaskSync ibnTask = TaskSync.CreateFromDataRow(row, ibnProjSandBox,
																	  timeZoneId);

						if (ibnTask != null)
						{
							ibnTask.CalendarId = calendarId;
							ibnTasks.Add(ibnTask.MsProjectMapValue, ibnTask);
						}
					}
				}
				#endregion
				#region Recalculate ibnTask
				foreach (TaskSync ibnTask in ibnTasks.Values)
				{
					TaskSync origMsProjTask;
					if (!msProjTasks.TryGetValue(ibnTask.MsProjectMapValue,
												out origMsProjTask))
						throw new Exception("Missing corresponding task in orig msProj xml file");

					ibnTask.Recalculate(origMsProjTask);
					if (!ibnTask.Validate())
					{
						ibnTasks.Remove(ibnTask.MsProjectMapValue);
					}
				}

				#endregion

				foreach (TaskSync task in ibnTasks.Values)
				{
					string taskXPath = string.Format(@"ns:Tasks/ns:Task[ns:UID = '{0}']",
													 task.MsProjectMapValue);
					XmlNode taskNode =
								msprjXmlDoc.DocumentElement.SelectSingleNode(taskXPath, nsXmlMmngr);

					if (taskNode == null)
						throw new Exception("msProject XML not valid: task not found");

					SetXmlValue(taskNode["Name"], typeof(String), task.Name);
					SetXmlValue(taskNode["Notes"], typeof(String), task.Notes);
					SetXmlValue(taskNode["Priority"], typeof(Int32), task.IntPriority);
					SetXmlValue(taskNode["PercentComplete"], typeof(Int32), task.PercentComplete);
					SetXmlValue(taskNode["PercentWorkComplete"], typeof(Int32), task.PercentWorkComplete);
					SetXmlValue(taskNode["ActualDuration"], typeof(TimeSpan), task.ActualDuration);
					SetXmlValue(taskNode["RemainingDuration"], typeof(TimeSpan), task.RemainingDuration);
					SetXmlValue(taskNode["ActualWork"], typeof(TimeSpan), task.ActualWork);
					SetXmlValue(taskNode["RemainingWork"], typeof(TimeSpan), task.RemainingWork);
					SetXmlValue(taskNode["Stop"], typeof(DateTime), task.Stop);

					foreach (AssignmentSync assign in task.Assignments)
					{
						string assignXPath = string.Format("ns:Assignments/ns:Assignment[ns:ResourceUID = '{0}'"
																						+ "and ns:TaskUID = '{1}']",
															assign.Resource.MsProjectMapVal, task.MsProjectMapValue);
						string resXPath = string.Format("ns:Resources/ns:Resource[ns:UID = '{0}']",
													   assign.Resource.MsProjectMapVal);
						XmlNode assignNode =
								msprjXmlDoc.DocumentElement.SelectSingleNode(assignXPath, nsXmlMmngr);
						XmlNode resNode =
							   msprjXmlDoc.DocumentElement.SelectSingleNode(resXPath, nsXmlMmngr);

						if ((resNode == null) || (assignNode == null))
							throw new Exception("msProject XML not valid: resource or assignments not found");
						//Assignments
						SetXmlValue(assignNode["PercentWorkComplete"], typeof(Int32),
									assign.PercentWorkComplete);
						SetXmlValue(assignNode["ActualWork"], typeof(TimeSpan),
									assign.ActualWork);
						SetXmlValue(assignNode["RemainingWork"], typeof(TimeSpan),
									assign.RemainingWork);
						SetXmlValue(assignNode["Stop"], typeof(DateTime), assign.Stop);
						//TimePhaseData
						//Remove TimephasedData nodes
						string timePhaseXPath = "ns:TimephasedData";
						XmlNodeList timePhaseNodes =
										assignNode.SelectNodes(timePhaseXPath, nsXmlMmngr);
						foreach (XmlNode timePhaseNode in timePhaseNodes)
						{
							assignNode.RemoveChild(timePhaseNode);
						}

						foreach (TimePhasedDataType ibnTimePhase in assign.TimePhaseDataType)
						{
							XmlNode timePhaseNode =
									assignNode.OwnerDocument.CreateElement(timePhaseXPath);
							//ibnTimePhase.Start =
							//            DBCommon.GetLocalDate(timeZoneId, ibnTimePhase.Start);
							//ibnTimePhase.Finish =
							//            DBCommon.GetLocalDate(timeZoneId, ibnTimePhase.Finish);
							AddXmlValue(timePhaseNode, "Type", typeof(Int32),
										ibnTimePhase.Type);
							AddXmlValue(timePhaseNode, "UID", typeof(Int32),
										ibnTimePhase.UID);
							AddXmlValue(timePhaseNode, "Start", typeof(DateTime),
										ibnTimePhase.Start);
							AddXmlValue(timePhaseNode, "Finish", typeof(DateTime),
										ibnTimePhase.Finish);
							AddXmlValue(timePhaseNode, "Unit", typeof(Int32),
										ibnTimePhase.Unit);
							AddXmlValue(timePhaseNode, "Value", typeof(TimeSpan),
										ibnTimePhase.Value);

							assignNode.AppendChild(timePhaseNode);
						}

						//Resources
						SetXmlValue(resNode["PercentWorkComplete"], typeof(Int32),
									assign.Resource.PercentWorkComplete);
						SetXmlValue(resNode["ActualWork"], typeof(TimeSpan),
									assign.Resource.ActualWork);
						SetXmlValue(resNode["RemainingWork"], typeof(TimeSpan),
									assign.Resource.RemainingWork);




					}

				}


				tran.Commit();
			}

			return msprjXmlDoc;
		}

		/// <summary>
		/// Recalculates all states.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		private static void RecalculateAllStates2(int projectId)
		{
			ArrayList changedTasks = new ArrayList();
			ArrayList tasks = new ArrayList();
			using (IDataReader reader = DBTask.GetListTasksInRecalculateOrder(projectId))
			{
				while (reader.Read())
				{
					int taskId = (int)reader["TaskId"];
					tasks.Add(taskId);
				}
			}

			DateTime calculatedStartDate = DateTime.MaxValue;
			DateTime calculatedFinishDate = DateTime.MinValue;

			foreach (int taskId in tasks)
			{
				int newStateId, oldStateId;
				DateTime startDate, finishDate;
				using (IDataReader reader = DBTask.TaskRecalculateState(taskId, DateTime.UtcNow))
				{
					reader.Read();
					newStateId = (int)reader["NewStateId"];
					oldStateId = (int)reader["OldStateId"];
					startDate = (DateTime)reader["StartDate"];
					finishDate = (DateTime)reader["FinishDate"];
				}

				if (startDate < calculatedStartDate)
					calculatedStartDate = startDate;
				if (finishDate > calculatedFinishDate)
					calculatedFinishDate = finishDate;

				if (newStateId == (int)ObjectStates.Completed || newStateId == (int)ObjectStates.Suspended)
				{
					startDate = DateTime.MinValue;
					finishDate = DateTime.MinValue;
				}

				Schedule.UpdateDateTypeValue(DateTypes.Task_StartDate, taskId, startDate);
				Schedule.UpdateDateTypeValue(DateTypes.Task_FinishDate, taskId, finishDate);
			}

			// O.R. [2009-06-15]: Incident dates
			if (PortalConfig.UseIncidentDatesForProject)
			{
				using (IDataReader reader = DBIncident.GetIncidentDatesForProject(projectId))
				{
					if (reader.Read() && reader["StartDate"] != DBNull.Value && reader["FinishDate"] != DBNull.Value)
					{
						DateTime startDate = (DateTime)reader["StartDate"];
						DateTime finishDate = (DateTime)reader["FinishDate"];

						if (startDate < calculatedStartDate)
							calculatedStartDate = startDate;
						if (finishDate > calculatedFinishDate)
							calculatedFinishDate = finishDate;
					}
				}
			}

			if (calculatedStartDate <= calculatedFinishDate)
				DBProject.UpdateDates(projectId, calculatedStartDate, calculatedFinishDate);
			else
				DBProject.UpdateDates(projectId, null, null);

			int userId = Security.CurrentUser.UserID;
			int timeZoneId = Security.CurrentUser.TimeZoneId;
			int languageId = Security.CurrentUser.LanguageId;

			ArrayList todoList = new ArrayList();
			foreach (int taskId in changedTasks)
			{
				// O.R [2/3/2006]: Событие об изменении состояния задачи генерим здесь
				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_State, taskId);

				// O.R [2/7/2006]: Формируем список ToDo
				using (IDataReader reader = DBTask.GetListToDo(taskId, userId, timeZoneId, languageId))
				{
					while (reader.Read())
					{
						if (!(bool)reader["IsCompleted"])
							todoList.Add((int)reader["ToDoId"]);
					}
				}
			}

			foreach (int todoId in todoList)
				ToDo.RecalculateState(todoId);
		}

		#endregion

		public static DataTable TaskImportAssignments(int project_id, int dataFileId)
		{
			if (!License.MsProjectIntegration)
				throw new FeatureNotAvailable();

			int timeZoneId = Security.CurrentUser.TimeZoneId;
			DataTable table = new DataTable();
			DataColumn Column = table.Columns.Add("ResourceId", typeof(int));
			table.PrimaryKey = new DataColumn[] { Column };
			table.Columns.Add("ResourceName", typeof(string));
			table.Columns.Add("PrincipalId", typeof(int));

			using (DbTransaction tran = DbTransaction.Begin())
			{
				XmlDocument msProjXmlDoc = new XmlDocument();
				using (MemoryStream memStream = GetMsProjectFileStream(project_id, dataFileId))
				{
					msProjXmlDoc.Load(memStream);
				}
				XmlNamespaceManager nsXmlMmngr =
							new XmlNamespaceManager(msProjXmlDoc.NameTable);
				nsXmlMmngr.AddNamespace("ns", @"http://schemas.microsoft.com/project");


				#region Read Tasks from MsProject XML
				XmlNodeList resNodeList =
							msProjXmlDoc.DocumentElement.SelectNodes("ns:Resources/ns:Resource",
																	nsXmlMmngr);
				foreach (XmlNode resNode in resNodeList)
				{
					ResourceSync resource = ResourceSync.CreateFromXml(resNode, null, nsXmlMmngr,
																	   timeZoneId);
					if (resource == null)
						continue;
					//Try to find in resource mapped value
					int principleId =
						  DBTask.GetResourceByMSProjectResourceId(project_id,
																  resource.MsProjectMapVal);

					//otherwise try to find by Res NAME and CODE in to Project members
					//if (principleId == -1)
					//    principleId = DBProject.GetProjectMemberByCode(project_id, resource.Name,
					//                                                   resource.Code);

					table.Rows.Add(new object[] { resource.MsProjectMapVal, resource.Name, 
                                                  principleId });

				}
				#endregion


				tran.Commit();
			}
			return table;
		}

		public static void TasksImport(DataTable assignmets, int project_id, int update_id)
		{
			if (!License.MsProjectIntegration)
				throw new FeatureNotAvailable();

			if (Project.GetIsMSProject(project_id))
				throw new AccessDeniedException();

			//int datafileid = -1;
			XmlNode main_node;
			int calendar_id;
			DateTime CurrentDate = DateTime.UtcNow;

			int ManagerId = DBProject.GetProjectManager(project_id);

			ArrayList OldTasks = new ArrayList();
			using (IDataReader OldTaskReader = DBTask.GetListTasksByProject(project_id, Security.CurrentUser.LanguageId, Security.CurrentUser.TimeZoneId))
			{
				while (OldTaskReader.Read())
				{
					OldTasks.Add((int)OldTaskReader["TaskId"]);
				}
			}


			//TODO: transaction
			using (DbTransaction tran = DbTransaction.Begin())
			{
				//    using (IDataReader reader = DBProject.GetProject(project_id, User.DefaultTimeZoneId, Security.CurrentUser.LanguageId))
				//    {
				//        if (reader.Read())
				//        {
				//            if (reader["XMLFileId"] == null)
				//                throw new Exception("data not found");
				//            else
				//                datafileid = Convert.ToInt32(reader["XMLFileId"]);
				//        }
				//    }

				//if (datafileid != update_id)
				//    throw new Exception("you lost");

				//XmlDocument datafile = new XmlDocument();
				//System.IO.MemoryStream MemStream = new System.IO.MemoryStream();

				//string ContainerName = "TemporaryStorage";
				//string ContainerKey = "ProjectId_" + project_id.ToString();

				//BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
				//Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");

				//fs.LoadFile(datafileid, MemStream);

				//MemStream.Seek(0, System.IO.SeekOrigin.Begin);
				//datafile.Load(MemStream);

				XmlDocument datafile = new XmlDocument();
				using (MemoryStream memStream = GetMsProjectFileStream(project_id, update_id))
				{
					datafile.Load(memStream);
				}

				main_node = datafile.DocumentElement;
				#region Udalit' vse taski

				foreach (int TaskId in OldTasks)
				{
					TimeTracking.ResetObjectId((int)TASK_TYPE, TaskId);

					MetaObject.Delete(TaskId, "TaskEx");

					// OZ: 2008-07-16 Clear Task File Library and remove
					// OZ: File Storage
					BaseIbnContainer container = BaseIbnContainer.Create("FileLibrary", UserRoleHelper.CreateTaskContainerKey(TaskId));
					FileStorage taskFs = (FileStorage)container.LoadControl("FileStorage");
					taskFs.DeleteAll(); // DeleteAll call ForeignContainerKey.Delete(UserRoleHelper.CreateTaskContainerKey(TaskId));

					// OZ: User Roles
					UserRoleHelper.DeleteTaskRoles(TaskId);
				}
				DBTask.DeleteAll(project_id);
				#endregion

				#region  создание календаря


				ArrayList CalendarList = new ArrayList();
				XmlNode calendar_node = null;
				string CalendarName = null;

				int CalendarUID = XmlConvert.ToInt32(main_node["CalendarUID"].InnerText);

				XmlNamespaceManager mngr = new XmlNamespaceManager(main_node.OwnerDocument.NameTable);
				mngr.AddNamespace("ns", "http://schemas.microsoft.com/project");

				while (CalendarUID != -1)
				{
					string filter = string.Format("ns:Calendars/ns:Calendar[ns:UID = '{0}']", CalendarUID);
					//string filter = "ns:Calendars/ns:Calendar";

					calendar_node = main_node.SelectSingleNode(filter, mngr);//["Calendars"]["Calendar"];//

					if (calendar_node != null)
					{
						if (CalendarName == null)
						{
							if (calendar_node["Name"] != null)
								CalendarName = calendar_node["Name"].InnerText;
						}

						if (CalendarList.Contains(CalendarUID))
							throw new Exception("wrong xml");

						CalendarList.Add(CalendarUID);
						XmlNode node = calendar_node["IsBaseCalendar"];
						int IsBaseCalendar = node != null ? XmlConvert.ToInt32(node.InnerText) : 0;
						node = calendar_node["BaseCalendarUID"];
						int BaseCalendarUID = node != null ? XmlConvert.ToInt32(node.InnerText) : -1;
						if (IsBaseCalendar == 1)
							break;
						else
							CalendarUID = BaseCalendarUID;
					}
					else
						throw new Exception("wrong xml");
				}


				calendar_id = Calendar.Create(project_id, CalendarName == null ? "" : CalendarName);// TODO: vstavit' nazavanie
				DbProject2.UpdateCalendar(project_id, calendar_id);

				for (int i = CalendarList.Count - 1; i >= 0; i--)
				{
					int Calendar_UID = (int)CalendarList[i];

					calendar_node = main_node.SelectSingleNode(string.Format("ns:Calendars/ns:Calendar[ns:UID = '{0}']", Calendar_UID), mngr);
					XmlNodeList WeekDays = calendar_node.SelectNodes("ns:WeekDays/ns:WeekDay", mngr);

					foreach (XmlNode WeekDay in WeekDays)
					{
						XmlNode DayTypeNode = WeekDay.SelectSingleNode("ns:DayType", mngr);
						XmlNode DayWorkingNode = WeekDay.SelectSingleNode("ns:DayWorking", mngr);

						if (DayTypeNode == null || DayWorkingNode == null)
							throw new Exception("wrong xml");

						int DayType = XmlConvert.ToInt32(DayTypeNode.InnerText);
						int DayWorking = XmlConvert.ToInt32(DayWorkingNode.InnerText);

						if (DayType >= 1 && DayType <= 7)
						{
							DayType = DayType > 1 ? DayType - 1 : 7;

							if (DayWorking == 0)
							{
								Calendar.UpdateWeekdayHoursInternal(calendar_id, (byte)DayType, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1);
							}
							else
								if (DayWorking == 1)
								{
									ArrayList list = new ArrayList();

									/*
										<WorkingTimes>
											<WorkingTime>
												<FromTime />
												<ToTime />
											</WorkingTime>
										</WorkingTimes>
										*/

									XmlNodeList WorkingTimeNodes = WeekDay.SelectNodes("ns:WorkingTimes/ns:WorkingTime", mngr);
									int counter = 0;
									foreach (XmlNode WorkingTimeNode in WorkingTimeNodes)
									{
										XmlNode FromNode = WorkingTimeNode["FromTime"];
										XmlNode ToNode = WorkingTimeNode["ToTime"];

										if (FromNode == null || ToNode == null)
											throw new Exception("wrong xml");

										DateTime FromTime = XmlConvert.ToDateTime(FromNode.InnerText,
											XmlDateTimeSerializationMode.Utc);
										DateTime ToTime = XmlConvert.ToDateTime(ToNode.InnerText,
											XmlDateTimeSerializationMode.Utc);

										//If end time is 0, is midnight the next day
										// 13.11.2008 et: fix 
										int earlyTime = (int)FromTime.TimeOfDay.TotalMinutes;
										int laterTime = ToTime.TimeOfDay == TimeSpan.Zero ? 60 * 24 : (int)ToTime.TimeOfDay.TotalMinutes;
										list.Add(earlyTime);
										list.Add(laterTime);
										counter += 2;
									}

									if (counter > 10)
										throw new Exception("wrong xml");

									for (int k = counter; k < 10; k++)
										list.Add(-1);

									Calendar.UpdateWeekdayHoursInternal(calendar_id, (byte)DayType, (int)list[0], (int)list[1], (int)list[2], (int)list[3], (int)list[4],
										(int)list[5], (int)list[6], (int)list[7], (int)list[8], (int)list[9]);
								}
								else
									throw new Exception("wrong xml");
						}
						else
							if (DayType == 0)
							{
								/*
										<TimePeriod>
											<FromDate />
											<ToDate />
										</TimePeriod>
									*/
								XmlNode FromDateNode = WeekDay.SelectSingleNode("ns:TimePeriod/ns:FromDate", mngr);
								XmlNode ToDateNode = WeekDay.SelectSingleNode("ns:TimePeriod/ns:ToDate", mngr);

								if (FromDateNode == null || ToDateNode == null)
									throw new Exception("wrong xml");

								DateTime FromDate = XmlConvert.ToDateTime(FromDateNode.InnerText,
									XmlDateTimeSerializationMode.Utc);
								DateTime ToDate = XmlConvert.ToDateTime(ToDateNode.InnerText,
									XmlDateTimeSerializationMode.Utc);

								if (FromDate > ToDate)
									throw new Exception("wrong xml");

								if (DayWorking == 0)
								{
									Calendar.CreateExceptionInternal(calendar_id, FromDate, ToDate, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1);
								}
								else
									if (DayWorking == 1)
									{
										ArrayList list = new ArrayList();

										/*
													<WorkingTimes>
														<WorkingTime>
															<FromTime />
															<ToTime />
														</WorkingTime>
													</WorkingTimes>
													*/

										XmlNodeList WorkingTimeNodes = WeekDay.SelectNodes("ns:WorkingTime", mngr);
										int counter = 0;
										foreach (XmlNode WorkingTimeNode in WorkingTimeNodes)
										{
											XmlNode FromNode = WorkingTimeNode["FromTime"];
											XmlNode ToNode = WorkingTimeNode["ToTime"];

											if (FromNode == null || ToNode == null)
												throw new Exception("wrong xml");

											DateTime FromTime = XmlConvert.ToDateTime(FromNode.InnerText,
												XmlDateTimeSerializationMode.Utc);
											DateTime ToTime = XmlConvert.ToDateTime(ToNode.InnerText,
												XmlDateTimeSerializationMode.Utc);

											//If end time is 0, is midnight the next day
											// 13.11.2008 et: fix 
											int earlyTime = (int)FromTime.TimeOfDay.TotalMinutes;
											int laterTime = ToTime.TimeOfDay == TimeSpan.Zero ? 60 * 24 : (int)ToTime.TimeOfDay.TotalMinutes;
											list.Add(earlyTime);
											list.Add(laterTime);

											counter += 2;
										}

										if (counter > 10)
											throw new Exception("wrong xml");

										for (int k = counter; k < 10; k++)
											list.Add(-1);

										Calendar.CreateExceptionInternal(calendar_id, FromDate, ToDate, (int)list[0], (int)list[1], (int)list[2], (int)list[3], (int)list[4],
											(int)list[5], (int)list[6], (int)list[7], (int)list[8], (int)list[9]);
									}
									else
										throw new Exception("wrong xml");
							}
							else
								throw new Exception("wrong xml");
					}
				}

				#endregion

				#region Resources

				Hashtable ResourcesHash = new Hashtable();
				string RName;
				string Code;
				//int LagFormat;
				XmlNodeList XmlResourceList = main_node.SelectNodes("ns:Resources/ns:Resource", mngr);

				foreach (XmlNode Resource_node in XmlResourceList)
				{
					int resUID = XmlConvert.ToInt32(Resource_node["UID"].InnerText);
					RName = Resource_node["Name"] != null ? Resource_node["Name"].InnerText : null;
					Code = Resource_node["Code"] != null ? Resource_node["Code"].InnerText : null;

					if (RName == null && Code == null)
						continue;

					int PrincipleId = DBProject.GetProjectMemberByCode(project_id, RName == null ? "" : RName, Code);

					if (PrincipleId != -1)
						ResourcesHash.Add(resUID, PrincipleId);
				}

				#endregion

				#region Tasks;  построение структуры

				//				int UID;
				int ID;
				string Name;
				int IsNull;
				string OutlineNumber;
				int OutlineLevel;
				int intPriority;
				DateTime Start;
				DateTime Finish;
				TimeSpan Duration;
				int DurationFormat;
				bool Milestone;
				bool Summary;
				int PercentComplete;
				DateTime ActualFinish;
				int ConstraintType;
				DateTime ConstraintDate;
				String Notes;

				int TaskNum = 0;
				int PrevOutlineLevel = 1;
				string PrevOutlineNumber = "0";
				bool PrevIsSummary = false;

				XmlNodeList XmlTasksList = main_node.SelectNodes("ns:Tasks/ns:Task", mngr);


				foreach (XmlNode task_node in XmlTasksList)
				{
					IsNull = XmlConvert.ToInt32(task_node["IsNull"].InnerText);
					if (IsNull == 0)
					{
						//required fields
						ID = XmlConvert.ToInt32(task_node["ID"].InnerText);
						Name = task_node["Name"] == null ? "_________" : task_node["Name"].InnerText;
						OutlineNumber = task_node["OutlineNumber"].InnerText;
						OutlineLevel = XmlConvert.ToInt32(task_node["OutlineLevel"].InnerText);
						intPriority = XmlConvert.ToInt32(task_node["Priority"].InnerText);
						Milestone = XmlConvert.ToInt32(task_node["Milestone"].InnerText) == 1 ? true : false;
						Summary = XmlConvert.ToInt32(task_node["Summary"].InnerText) == 1 ? true : false;
						PercentComplete = task_node["PercentComplete"] != null ? XmlConvert.ToInt32(task_node["PercentComplete"].InnerText) : 0;
						ActualFinish = task_node["ActualFinish"] != null ? XmlConvert.ToDateTime(task_node["ActualFinish"].InnerText,
							XmlDateTimeSerializationMode.Utc) : DateTime.MinValue;

						if (task_node["Notes"] != null)
							Notes = task_node["Notes"].InnerText;
						else
							Notes = "";

						if (OutlineLevel < 1)
							continue; //throw new Exception("wrong xml");

						if (PrevOutlineLevel < OutlineLevel)
						{
							if (PrevOutlineLevel + 1 == OutlineLevel)
							{
								if (!PrevIsSummary)
									throw new Exception("wrong xml");

								OutlineNumber = PrevOutlineNumber + ".1";
							}
							else
								throw new Exception("wrong xml");
						}
						else
							if (PrevOutlineLevel >= OutlineLevel)
							{
								if (PrevIsSummary)
									throw new Exception("wrong xml");

								if (PrevOutlineLevel > OutlineLevel)
								{
									for (int i = 0; i < PrevOutlineLevel - OutlineLevel; i++)
									{
										int ind = PrevOutlineNumber.LastIndexOf(".");
										PrevOutlineNumber = PrevOutlineNumber.Substring(0, ind);
									}
								}

								string LeftPart;
								int val;
								int index = PrevOutlineNumber.LastIndexOf(".");

								if (index == -1)
								{
									LeftPart = "";
									val = Convert.ToInt32(PrevOutlineNumber);
								}
								else
								{
									LeftPart = PrevOutlineNumber.Substring(0, index + 1);
									val = Convert.ToInt32(PrevOutlineNumber.Substring(index + 1));
								}
								val++;
								OutlineNumber = LeftPart + val.ToString();
							}


						if (TaskNum == 1 && PrevOutlineLevel != 1)
							throw new Exception("wrong xml");


						TaskNum++;
						PrevOutlineLevel = OutlineLevel;
						PrevOutlineNumber = OutlineNumber;
						PrevIsSummary = Summary;


						if (intPriority < 400)
							intPriority = (int)Priority.Low;
						else
							if (intPriority >= 400 && intPriority < 600)
								intPriority = (int)Priority.Normal;
							else
								if (intPriority >= 600 && intPriority < 700)
									intPriority = (int)Priority.High;
								else
									if (intPriority >= 700 && intPriority < 900)
										intPriority = (int)Priority.VeryHigh;
									else
										if (intPriority >= 900 && intPriority <= 1000)
											intPriority = (int)Priority.Urgent;
										else
											intPriority = (int)Priority.Normal;

						int UserId = Security.CurrentUser.UserID;
						//current date as we change it after
						// - Start,Finish,Duration
						// + Description, PercentComplete, may Resone,ActualFinish
						int TaskId = DBTask.Create(project_id, Name, Notes, UserId, CurrentDate, CurrentDate, CurrentDate,
							PercentComplete == 100 ? ActualFinish : DateTime.MinValue, 0, intPriority,
							(int)ConstraintTypes.AsSoonAsPossible, (int)ActivationTypes.AutoWithCheck,
							(int)CompletionType.Any, TaskNum, Milestone, Summary,
							OutlineNumber, OutlineLevel, PercentComplete, PercentComplete == 100 ? 1 : -1);

						XmlNode XmlTaskId = task_node.OwnerDocument.CreateNode(XmlNodeType.Element, "TaskId", null);
						XmlTaskId.InnerText = TaskId.ToString();

						task_node.AppendChild(XmlTaskId);
					}
				}

				#endregion

				#region Predecessors; создание
				//связать ресурсы
				/*
				 *	0 ОО (окончание-окончание) 
					1 ОН (окончание-начало) 
					2 НО (начало-окончание) 
					3 НН (начало-начало) 
				 */
				int Type;
				int LinkLag;
				//int LagFormat;
				XmlNodeList XmlPredecessorLinkList = main_node.SelectNodes("ns:Tasks/ns:Task/ns:PredecessorLink", mngr);

				foreach (XmlNode predlink_node in XmlPredecessorLinkList)
				{
					int SuccId = XmlConvert.ToInt32(predlink_node.ParentNode["TaskId"].InnerText);
					string PredUID = predlink_node["PredecessorUID"].InnerText;
					XmlNode PredNode = main_node.SelectSingleNode(string.Format("ns:Tasks/ns:Task[ns:UID = {0}]", PredUID), mngr);
					if (PredNode == null)
						continue;
					int PredId = XmlConvert.ToInt32(PredNode["TaskId"].InnerText);
					Type = XmlConvert.ToInt32(predlink_node["Type"].InnerText);
					LinkLag = XmlConvert.ToInt32(predlink_node["LinkLag"].InnerText) / 10;
					//LagFormat = XmlConvert.ToInt32(predlink_node["LagFormat"].Value);

					if (Type == 1)
					{
						if (DBTask.TaskInAllDependOnTasks(SuccId, PredId))
							throw new Exception("from task to dependent on task");

						DBTask.CreateTaskLink(PredId, SuccId, LinkLag);
					}

				}
				#endregion

				#region  TODO: пересчет тасков и линков


				//				bool resFound = false;
				ArrayList TaskList = new ArrayList();
				using (IDataReader TaskReader = DBTask.GetListTasksInRecalculateOrder(project_id))
				{
					while (TaskReader.Read())
						TaskList.Add(Convert.ToInt32(TaskReader["TaskId"]));
				}

				foreach (int TaskId in TaskList)
				{
					XmlNode task_node = main_node.SelectSingleNode(string.Format("ns:Tasks/ns:Task[TaskId = '{0}']", TaskId), mngr);

					int TimeZoneId = Security.CurrentUser.TimeZoneId;
					int TaskUID = XmlConvert.ToInt32(task_node["UID"].InnerText);
					Start = DBCommon.GetUTCDate(TimeZoneId, XmlConvert.ToDateTime(task_node["Start"].InnerText,
						XmlDateTimeSerializationMode.Utc));
					Finish = DBCommon.GetUTCDate(TimeZoneId, XmlConvert.ToDateTime(task_node["Finish"].InnerText,
						XmlDateTimeSerializationMode.Utc));
					// [11.05.06] A.Rakov, Hot Fix
					if (task_node["Duration"] == null || task_node["Duration"].InnerText == "")
					{
						Duration = TimeSpan.Zero;
						DurationFormat = 7;
					}
					else
					{
						Duration = XmlConvert.ToTimeSpan(task_node["Duration"].InnerText);
						// [29.09.06] A.Rakov, Hot Fix
						if (task_node["DurationFormat"] != null && task_node["DurationFormat"].InnerText == "")
						{
							DurationFormat = XmlConvert.ToInt32(task_node["DurationFormat"].InnerText);
						}
						else
						{
							Duration = TimeSpan.Zero;
							DurationFormat = 7;
						}
					}

					ConstraintType = XmlConvert.ToInt32(task_node["ConstraintType"].InnerText);

					// [23.03.07] FIXED                        
					if (task_node["ConstraintDate"] != null && task_node["ConstraintDate"].InnerText != string.Empty)
						ConstraintDate = DBCommon.GetUTCDate(TimeZoneId, XmlConvert.ToDateTime(task_node["ConstraintDate"].InnerText,
							XmlDateTimeSerializationMode.Utc));
					else
						ConstraintDate = DateTime.MinValue;

					Start = DBCalendar.GetFinishDateByDuration(calendar_id, Start, 0);
					Finish = DBCalendar.GetStartDateByDuration(calendar_id, Finish, 0);

					DataTable PredDates = DBTask.GetListTaskPredecessorDates(TaskId, calendar_id);

					bool UsePred = true;
					DateTime PredFinishDate;
					DateTime PossibleStartDate;
					int LinkId;
					int Lag;

					//LinkId, PredId, FinishDate, Lag, DBO.GetFinishDateByDuration(@CalendarId,FinishDate,Lag) As PossibleStartDate
					foreach (DataRow row in PredDates.Rows)
					{
						LinkId = (int)row["LinkId"];
						PredFinishDate = (DateTime)row["FinishDate"];
						PossibleStartDate = (DateTime)row["PossibleStartDate"];
						Lag = (int)row["Lag"];

						if (PossibleStartDate > Start)
						{
							//вычислиьт новвый лэг
							Lag = 0;
							if (PredFinishDate > Start)
							{
								Lag = DBCalendar.GetDurationByFinishDate(calendar_id, Start, PredFinishDate);
							}
							else
								if (PredFinishDate < Start)
								{
									Lag = -DBCalendar.GetDurationByFinishDate(calendar_id, PredFinishDate, Start);
								}

							//обновить линк
							DBTask.UpdateTaskLink(LinkId, Lag);


							UsePred = true;

						}
						else
							if (PossibleStartDate == Start)
							{
								UsePred = true;
							}
					}
					PredDates.Dispose();



					/*
					 *	0 Как можно раньше 
						1 Как можно позже 
						2 Фактическое начало 
						3 Фактическое окончание 
						4 Начало не ранее 
						5 Начало не позднее 
						6 Окончание не ранее 
						7 Окончание не позднее 

					 */
					if (UsePred)
					{
						if (ConstraintType != 0)
							ConstraintType = 4;

						if (ConstraintDate >= Start)
							ConstraintDate = Start;
					}
					else
					{
						//TODO; project start
						ConstraintType = 4;
						ConstraintDate = Start;
					}

					if (Finish < Start)
						Finish = Start;

					int iDuration = DBCalendar.GetDurationByFinishDate(calendar_id, Start, Finish);
					DBTask.Update(TaskId, Start, Finish, iDuration, ConstraintType, ConstraintDate);

					// [2007-07-25], by O.Rylin
					if (iDuration > MaxTaskTime)
						iDuration = MaxTaskTime;
					DBTask.UpdateTaskTime(TaskId, iDuration);

					#region Naznachenie resursov
					//					resFound = false;
					int intUID = XmlConvert.ToInt32(task_node["UID"].InnerText);

					/*
					 *	<Assignments>
							<Assignment>
								<TaskUID>6</TaskUID>
								<ResourceUID></ResourceUID> 

					 */
					string fl1 = string.Format("ns:Assignments/ns:Assignment[ns:TaskUID = '{0}']", TaskUID);
					XmlNodeList AssignNodeList = main_node.SelectNodes(fl1, mngr);

					foreach (XmlNode ass_node in AssignNodeList)
					{
						int PrincipleId = -1;

						int resUID = XmlConvert.ToInt32(ass_node["ResourceUID"].InnerText);

						DataRow row = assignmets.Rows.Find(new object[] { resUID });
						if (row != null)
							PrincipleId = Convert.ToInt32(row["PrincipalId"]);


						//if(ResourcesHash[resUID] != null)
						if (PrincipleId != -1)
						{
							//PrincipleId = (int)ResourcesHash[resUID];
							bool CanManage = false;
							if (PrincipleId == ManagerId)
								CanManage = true;
							DBTask.AddResource(TaskId, PrincipleId, false, CanManage);

							if (User.IsExternal(PrincipleId))
								DBCommon.AddGate((int)TASK_TYPE, TaskId, PrincipleId);

							//							resFound = true;
						}

					}

					//if(!resFound)
					//	DBTask.AddResource(TaskId,Security.UserID,false);

					// ET 14.04.2008
					//Apply security settings for new task
					ApplySecurityOnTaskImported(project_id, TaskId);

				}
					#endregion

				#region granici projecta
				DateTime ProjectTargetStartDate;
				DateTime ProjectTargetFinishDate;
				DateTime ProjectStartDateRange;
				DateTime ProjectFinishDateRange;

				using (IDataReader ProjectReader = DBProject.GetProject(project_id, User.DefaultTimeZoneId, Security.CurrentUser.LanguageId))
				{
					if (ProjectReader.Read())
					{
						ProjectTargetStartDate = (DateTime)ProjectReader["TargetStartDate"];
						ProjectTargetFinishDate = (DateTime)ProjectReader["TargetFinishDate"];
					}
					else
						throw new Exception("Project not found");
				}

				using (IDataReader ProjectReader = DBProject.GetProjectDateRange(project_id))
				{
					if (ProjectReader.Read())
					{
						object rowStartDate = ProjectReader["StartDate"];
						object rowFinishDate = ProjectReader["FinishDate"];
						ProjectStartDateRange = rowStartDate != DBNull.Value ? (DateTime)rowStartDate : DateTime.MinValue;
						ProjectFinishDateRange = rowFinishDate != DBNull.Value ? (DateTime)rowFinishDate : DateTime.MaxValue;
					}
					else
						throw new Exception("Project not found");
				}

				if (ProjectStartDateRange < ProjectTargetStartDate)
					ProjectTargetStartDate = ProjectStartDateRange;
				if (ProjectFinishDateRange > ProjectTargetFinishDate)
					ProjectTargetFinishDate = ProjectFinishDateRange;

				DBProject.UpdateTargetDates(project_id, ProjectTargetStartDate, ProjectTargetFinishDate);

				#endregion

				#endregion

				RecalculateAllStates(project_id);

				// TODO: Remove XML File
				DeleteTmpProjectFile(project_id, update_id);

				//if (datafileid > 0)
				//{
				//    Mediachase.IBN.Business.ControlSystem.FileInfo fi = fs.GetFile(datafileid);
				//    if (fi != null)
				//        fs.DeleteFile(datafileid);
				//}

				DBProject.UpdateXMLFileId(project_id, -1);

				// O.R: Recalculating project TaskTime
				TimeTracking.RecalculateProjectTaskTime(project_id);

				tran.Commit();
			}
		}

		/// <summary>
		/// Applies the security on task imported.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <param name="taskId">The task id.</param>
		/// <remarks> Call only in transaction scope </remarks>
		private static void ApplySecurityOnTaskImported(int projectId, int taskId)
		{
			if (DbContext.Current.Transaction == null)
				throw new ArgumentException("Call only in transaction scope");

			int timeZoneId = Security.CurrentUser.TimeZoneId;
			int userId = Security.CurrentUser.UserID;
			int managerId = -1;

			DateTime startDate = DateTime.MinValue;
			DateTime finishDate = DateTime.MaxValue;

			List<int> taskResources = new List<int>();

			#region get project manager
			using (IDataReader projectReader = DBProject.GetProject(projectId, User.DefaultTimeZoneId,
																	Security.CurrentUser.LanguageId))
			{
				if (projectReader.Read())
				{
					managerId = (int)projectReader["ManagerId"];
				}
				else
					throw new Exception("Project not found");
			}
			#endregion

			#region get time borders
			using (IDataReader resourceReader = DBTask.GetTask(taskId, timeZoneId,
															   Security.CurrentUser.LanguageId))
			{
				if (resourceReader.Read())
				{
					startDate = (DateTime)resourceReader["StartDate"];
					finishDate = (DateTime)resourceReader["FinishDate"];
				}
			}

			startDate = DBCommon.GetUTCDate(timeZoneId, startDate);
			finishDate = DBCommon.GetUTCDate(timeZoneId, finishDate);
			#endregion

			#region get task resources
			using (IDataReader resourceReader = DBTask.GetListResources(taskId, timeZoneId))
			{
				while (resourceReader.Read())
				{
					taskResources.Add((int)resourceReader["UserId"]);
				}
			}


			#endregion

			//Add manager role
			UserRoleHelper.AddTaskManagerRole(taskId, managerId);

			if (managerId != userId)
				UserRoleHelper.AddTaskCreatorRole(taskId, userId);

			ForeignContainerKey.Add(UserRoleHelper.CreateTaskContainerKey(taskId),
									UserRoleHelper.CreateProjectContainerKey(projectId));

			Schedule.UpdateDateTypeValue(DateTypes.Task_StartDate, taskId, startDate);
			Schedule.UpdateDateTypeValue(DateTypes.Task_FinishDate, taskId, finishDate);

			foreach (int resourceId in taskResources)
			{
				bool canManage = (bool)(resourceId == managerId);
				//manager role already add, skip him
				if (!canManage)
					UserRoleHelper.AddTaskResourceRole(taskId, resourceId);
			}


		}

		#endregion

		//TODO: IMPLEMENTATION
		#region Task Expoort
		public static void TasksExport()
		{
			if (!License.MsProjectIntegration)
				throw new FeatureNotAvailable();
		}

		#endregion

		#region Templates


		public static DataTable MakeTemplateAssignments(int project_id)
		{
			DataTable table = new DataTable();
			DataColumn Column = table.Columns.Add("PrincipalId", typeof(int));
			table.PrimaryKey = new DataColumn[] { Column };
			table.Columns.Add("PrincipalName", typeof(string));
			table.Columns.Add("RoleName", typeof(string));
			table.Columns.Add("RoleId", typeof(int));

			//R.ResourceId, R.PrincipalId AS UserId, U.FirstName, U.LastName
			using (IDataReader resoursList = Project.GetListTaskResources(project_id))
				while (resoursList.Read())
				{
					string name = (String)resoursList["FirstName"] + " " + (String)resoursList["LastName"];
					int PrincipalId = (int)resoursList["UserId"];
					table.Rows.Add(new object[] { PrincipalId, name, name, 0 });
				}

			return table;
		}

		public static void MakeTemplateFromProject2(int project_id, DataTable assignmets, string template_name, bool IsPrivate, TemplateMakeInfo template_info)
		{
			XmlDocument TempDoc = new XmlDocument();
			XmlNode project_node = TempDoc.AppendChild(TempDoc.CreateElement("Project"));
			XmlNode tasks_node = project_node.AppendChild(TempDoc.CreateElement("Tasks"));
			XmlNode resources_node = project_node.AppendChild(TempDoc.CreateElement("Resources"));
			XmlNode assignments_node = project_node.AppendChild(TempDoc.CreateElement("Assignments"));

			//DV: 26.10.2006
			XmlNode projinfo_node = project_node.AppendChild(TempDoc.CreateElement("ProjectInfo"));
			XmlNode roles_node = project_node.AppendChild(TempDoc.CreateElement("Roles"));
			XmlNode projcategory_node = projinfo_node.AppendChild(TempDoc.CreateElement("ProjectCategories"));
			XmlNode generalcategory_node = projinfo_node.AppendChild(TempDoc.CreateElement("GeneralCategories"));
			XmlNode metafield_node = projinfo_node.AppendChild(TempDoc.CreateElement("MetaFields"));

			#region Fill: ProjectInfo node
			/*
			 <ProjectInfo>
				<Manager>
				<ExecutiveManager>
				<Goals>
				<Description>
				<Deliverables>
				<Scope>
				<Priority>
				<ProjectCategories>
					<Category>
						<UID>
						<Name>
				<GeneralCategories>
					<Category>
						<UID>
						<Name>
			</ProjectInfo>
			*/

			using (IDataReader reader = Project.GetProject(project_id))
			{
				if (reader.Read())
				{
					//Meta Fields Import
					int projType = (int)reader["TypeId"];

					#region --- Import MetaFields ---
					if (template_info.MetaFields)
					{
						MetaObject obj = MetaDataWrapper.LoadMetaObject(project_id, String.Format("ProjectsEx_{0}", projType));
						MetaClass proj = MetaClass.Load(String.Format("ProjectsEx_{0}", projType));
						foreach (MetaField mf in proj.UserMetaFields)
						{
							XmlNode mf_node = metafield_node.AppendChild(TempDoc.CreateElement("MetaField"));
							mf_node.AppendChild(TempDoc.CreateElement("Name")).InnerText = mf.Name;
							if (obj != null && obj[mf.Name] != null)
							{
								if (obj[mf.Name] is Mediachase.MetaDataPlus.Configurator.MetaDictionaryItem)
								{
									mf_node.AppendChild(TempDoc.CreateElement("Value")).InnerText = XmlConvert.EncodeName(((Mediachase.MetaDataPlus.Configurator.MetaDictionaryItem)obj[mf.Name]).Id.ToString());
								}
								else if (obj[mf.Name] is Mediachase.MetaDataPlus.Configurator.MetaDictionaryItem[])
								{
									string multiValue = string.Empty;
									foreach (MetaDictionaryItem item in (Mediachase.MetaDataPlus.Configurator.MetaDictionaryItem[])obj[mf.Name])
									{
										multiValue += item.Id + ",";
									}
									if (multiValue.Length > 0)
										multiValue = multiValue.Remove(multiValue.Length - 1);
									mf_node.AppendChild(TempDoc.CreateElement("Value")).InnerText = XmlConvert.EncodeName(multiValue);
								}
								else
								{
									mf_node.AppendChild(TempDoc.CreateElement("Value")).InnerText = XmlConvert.EncodeName(obj[mf.Name].ToString());
								}
							}
						}
					}
					#endregion

					#region --- Import System Fields ---
					if (template_info.SystemFields)
					{

						projinfo_node.AppendChild(TempDoc.CreateElement("Goals")).InnerText = XmlConvert.EncodeName((string)reader["Goals"]);
						projinfo_node.AppendChild(TempDoc.CreateElement("Description")).InnerText = XmlConvert.EncodeName((string)reader["Description"]);
						projinfo_node.AppendChild(TempDoc.CreateElement("Deliverables")).InnerText = XmlConvert.EncodeName((string)reader["Deliverables"]);
						projinfo_node.AppendChild(TempDoc.CreateElement("Scope")).InnerText = XmlConvert.EncodeName((string)reader["Scope"]);
						projinfo_node.AppendChild(TempDoc.CreateElement("Priority")).InnerText = ((int)reader["PriorityId"]).ToString();
						projinfo_node.AppendChild(TempDoc.CreateElement("ProjectTypeId")).InnerText = projType.ToString();
						projinfo_node.AppendChild(TempDoc.CreateElement("CalendarId")).InnerText = ((int)reader["CalendarId"]).ToString();
						projinfo_node.AppendChild(TempDoc.CreateElement("CurrencyId")).InnerText = ((int)reader["CurrencyId"]).ToString();
						projinfo_node.AppendChild(TempDoc.CreateElement("TypeId")).InnerText = ((int)reader["TypeId"]).ToString();
						projinfo_node.AppendChild(TempDoc.CreateElement("Title")).InnerText = XmlConvert.EncodeName((string)reader["Title"]);

						//Fill GeneralCategories
						using (IDataReader reader2 = Project.GetListCategories(project_id))
						{
							while (reader2.Read())
							{
								XmlNode category_node = generalcategory_node.AppendChild(TempDoc.CreateElement("Category"));
								category_node.AppendChild(TempDoc.CreateElement("UID")).InnerText = ((int)reader2["CategoryId"]).ToString();
								category_node.AppendChild(TempDoc.CreateElement("Name")).InnerText = (string)reader2["CategoryName"];
							}
						}

						//Fill ProjectCategories
						using (IDataReader reader2 = Project.GetListProjectCategoriesByProject(project_id))
						{
							while (reader2.Read())
							{
								XmlNode category_node = projcategory_node.AppendChild(TempDoc.CreateElement("Category"));
								category_node.AppendChild(TempDoc.CreateElement("UID")).InnerText = ((int)reader2["CategoryId"]).ToString();
								category_node.AppendChild(TempDoc.CreateElement("Name")).InnerText = (string)reader2["CategoryName"];
							}
						}
					}
					#endregion

					#region --- Import Manager ---
					if (template_info.Manager)
					{
						projinfo_node.AppendChild(TempDoc.CreateElement("ManagerId")).InnerText = reader["ManagerId"].ToString();
						/*using (IDataReader reader2 = User.GetUserInfo((int)reader["ManagerId"]))
						{
							if (reader2.Read())
								projinfo_node.AppendChild(TempDoc.CreateElement("Manager")).InnerText = String.Format("{0} {1}",(string)reader2["LastName"], (string)reader2["FirstName"]);
						}*/
					}
					#endregion

					#region --- Import Executive Managaer ---
					if (template_info.Roles && template_info.ExecManager)
					{
						if (reader["ExecutiveManagerId"] != DBNull.Value)
							projinfo_node.AppendChild(TempDoc.CreateElement("ExecutiveManagerId")).InnerText = reader["ExecutiveManagerId"].ToString();
						/*if (reader["ExecutiveManagerId"] != DBNull.Value)
						{
							using (IDataReader reader2 = User.GetUserInfo((int)reader["ExecutiveManagerId"]))
							{
								if (reader2.Read())
									projinfo_node.AppendChild(TempDoc.CreateElement("ExecutiveManager")).InnerText = String.Format("{0} {1}",(string)reader2["LastName"], (string)reader2["FirstName"]);
							}
						}
						else
						{
							projinfo_node.AppendChild(TempDoc.CreateElement("ExecutiveManager")).InnerText = DBNull.Value.ToString();
						}*/
					}
					#endregion
				}
			}
			#endregion

			#region Fill: Roles node
			/*
			 <Roles>
				<Team>
					<TeamMember>
						<Name>
						<UID>
					</TeamMember>
					...
				</Team>
				<Stake>
					<StackMember>
						<IsGroup>
						<Name>
						<UID> PrincipalId
					</StackMember>
					...
				</Stake>
				<Sponsor>
					<SponsorMember>
						<IsGroup>
						<Name>
						<UID> PrincipalId
					</SponsorMember>
					...
				</Sponsor>
			 */

			XmlNode team_node = roles_node.AppendChild(TempDoc.CreateElement("Team"));
			XmlNode stake_node = roles_node.AppendChild(TempDoc.CreateElement("Stake"));
			XmlNode sponsor_node = roles_node.AppendChild(TempDoc.CreateElement("Sponsor"));

			#region --- Team Import ---
			if (template_info.Team)
			{
				using (IDataReader reader = Project.GetListTeamMembers(project_id))
				{
					while (reader.Read())
					{
						XmlNode teammember_node = team_node.AppendChild(TempDoc.CreateElement("TeamMember"));
						//if (User.IsGroup((int)reader["UserId"]))
						//	teammember_node.AppendChild(TempDoc.CreateElement("Name")).InnerText = String.Format("{0}", reader["FirstName"]);
						teammember_node.AppendChild(TempDoc.CreateElement("UID")).InnerText = ((int)reader["UserId"]).ToString();
						teammember_node.AppendChild(TempDoc.CreateElement("Code")).InnerText = reader["Code"].ToString();
						teammember_node.AppendChild(TempDoc.CreateElement("Rate")).InnerText = ((decimal)reader["Rate"]).ToString();

					}
				}
			}
			#endregion

			#region --- Stackholders Import ---
			if (template_info.Stackhoders)
			{
				using (IDataReader reader = Project.GetListStakeholders(project_id))
				{
					while (reader.Read())
					{
						XmlNode stakemember_node = stake_node.AppendChild(TempDoc.CreateElement("StakeMember"));
						stakemember_node.AppendChild(TempDoc.CreateElement("IsGroup")).InnerText = ((bool)reader["IsGroup"]).ToString();
						if ((bool)reader["IsGroup"])
						{
							using (IDataReader reader2 = SecureGroup.GetGroup((int)reader["PrincipalId"]))
							{
								if (reader2.Read())
								{
									stakemember_node.AppendChild(TempDoc.CreateElement("Name")).InnerText = Common.GetWebResourceString((string)reader2["GroupName"]);
								}
							}
						}
						else
						{
							stakemember_node.AppendChild(TempDoc.CreateElement("Name")).InnerText = User.GetUserName((int)reader["PrincipalId"]); ;
						}

						stakemember_node.AppendChild(TempDoc.CreateElement("UID")).InnerText = ((int)reader["PrincipalId"]).ToString();
					}
				}
			}
			#endregion

			#region --- Sponsor Import ---
			if (template_info.Sponsor)
			{
				using (IDataReader reader = Project.GetListSponsors(project_id))
				{
					while (reader.Read())
					{
						XmlNode sponsormember_node = sponsor_node.AppendChild(TempDoc.CreateElement("SponsorMember"));
						sponsormember_node.AppendChild(TempDoc.CreateElement("IsGroup")).InnerText = ((bool)reader["IsGroup"]).ToString();

						if ((bool)reader["IsGroup"])
						{
							using (IDataReader reader2 = SecureGroup.GetGroup((int)reader["PrincipalId"]))
							{
								if (reader2.Read())
								{
									sponsormember_node.AppendChild(TempDoc.CreateElement("Name")).InnerText = Common.GetWebResourceString((string)reader2["GroupName"]);
								}
							}
						}
						else
						{
							sponsormember_node.AppendChild(TempDoc.CreateElement("Name")).InnerText = User.GetUserName((int)reader["PrincipalId"]); ;
						}

						sponsormember_node.AppendChild(TempDoc.CreateElement("Name")).InnerText = User.GetUserName((int)reader["MemberId"]); ;
						sponsormember_node.AppendChild(TempDoc.CreateElement("UID")).InnerText = ((int)reader["PrincipalId"]).ToString();
					}
				}
			}
			#endregion
			#endregion


			/*
			 <Resources>
				<Resource>
					<UID>3</UID>
					<Name>
			PrincipalId
			PrincipalName
			RoleName
			RoleId
			*/

			int i = 1;
			for (int k = 0; k < assignmets.Rows.Count; k++)
				assignmets.Rows[k]["RoleId"] = 0;

			for (int k = 0; k < assignmets.Rows.Count; k++)
			{
				if ((int)assignmets.Rows[k]["RoleId"] == 0)
				{
					assignmets.Rows[k]["RoleId"] = i;

					XmlNode resource_node = resources_node.AppendChild(TempDoc.CreateElement("Resource"));
					resource_node.AppendChild(TempDoc.CreateElement("UID")).InnerText = i.ToString();
					resource_node.AppendChild(TempDoc.CreateElement("Name")).InnerText = assignmets.Rows[k]["RoleName"].ToString();

					for (int j = k + 1; j < assignmets.Rows.Count; j++)
						if (assignmets.Rows[k]["RoleName"].ToString().ToUpper() == assignmets.Rows[j]["RoleName"].ToString().ToUpper())
						{
							assignmets.Rows[j]["RoleName"] = assignmets.Rows[k]["RoleName"];
							assignmets.Rows[j]["RoleId"] = i;
						}

					i++;
				}
			}

			/*
			<Tasks>
				<Task>
					<ID>3</ID>
					<UID>3</UID>
					<Name>
					<Description>
					<OutlineNumber>1.1.1</OutlineNumber> 
					<OutlineLevel>3</OutlineLevel> 
					<Priority>500</Priority>
					<Duration>PT40H0M0S</Duration>
					<Milestone>0</Milestone> 
					<Summary>0</Summary> 
					 <PredecessorLink>
						<PredecessorUID>4</PredecessorUID> 
						<LinkLag>0</LinkLag> 
					</PredecessorLink>							
			*/

			#region --- Task Structure ---
			if (template_info.TaskInfo != TemplateTaskInfo.NoTask)
			{
				using (IDataReader TaskList = Task.GetListTasksByProject(project_id))
					while (TaskList.Read())
					{
						int iTaskId = (int)TaskList["TaskId"];
						XmlNode task_node = tasks_node.AppendChild(TempDoc.CreateElement("Task"));

						using (IDataReader reader = Task.GetTask(iTaskId))
						{
							if (reader.Read())
							{

								task_node.AppendChild(TempDoc.CreateElement("UID")).InnerText = iTaskId.ToString();
								task_node.AppendChild(TempDoc.CreateElement("ID")).InnerText = ((int)reader["TaskNum"]).ToString();
								task_node.AppendChild(TempDoc.CreateElement("Name")).InnerText = XmlConvert.EncodeName(reader["Title"].ToString());
								task_node.AppendChild(TempDoc.CreateElement("Description")).InnerText = XmlConvert.EncodeName(reader["Description"].ToString());
								task_node.AppendChild(TempDoc.CreateElement("OutlineNumber")).InnerText = reader["OutlineNumber"].ToString();
								task_node.AppendChild(TempDoc.CreateElement("OutlineLevel")).InnerText = ((int)reader["OutlineLevel"]).ToString();
								task_node.AppendChild(TempDoc.CreateElement("Priority")).InnerText = ((int)reader["PriorityId"]).ToString();
								task_node.AppendChild(TempDoc.CreateElement("Duration")).InnerText = reader["Duration"].ToString();
								task_node.AppendChild(TempDoc.CreateElement("Milestone")).InnerText = (bool)reader["IsMilestone"] ? "1" : "0";
								task_node.AppendChild(TempDoc.CreateElement("Summary")).InnerText = (bool)reader["IsSummary"] ? "1" : "0";
								//DV 01.10.2006
								task_node.AppendChild(TempDoc.CreateElement("CompletionTypeId")).InnerText = ((int)reader["CompletionTypeId"]).ToString();

								//DV 24.09.2007
								if ((bool)reader["IsMilestone"] && reader["PhaseId"] != DBNull.Value)
								{
									task_node.AppendChild(TempDoc.CreateElement("PhaseId")).InnerText = ((int)reader["PhaseId"]).ToString();
								}
								task_node.AppendChild(TempDoc.CreateElement("MustBeConfirmed")).InnerText = (bool)reader["MustBeConfirmed"] ? "1" : "0";

							}

						}

						XmlNode pred_list = null;
						/// LinkId, PredId, SuccId, Lag 
						/// </summary>
						/// <returns></returns>
						using (IDataReader linkList = DBTask.GetListPredecessorsDetails(iTaskId, 0))
							while (linkList.Read())
							{
								//if(pred_list == null) //
								pred_list = task_node.AppendChild(TempDoc.CreateElement("PredecessorLink"));

								pred_list.AppendChild(TempDoc.CreateElement("PredecessorUID")).InnerText = ((int)linkList["TaskId"]).ToString();
								pred_list.AppendChild(TempDoc.CreateElement("LinkLag")).InnerText = ((int)linkList["Lag"]).ToString();
							}

						/*
						<Assignments>
							<Assignment>
							<UID></UID> 
							<TaskUID></TaskUID> 
							<ResourceUID></ResourceUID> 
							<MustBeConfirmed></MustBeConfirmed>
						 */
						ArrayList roleidlist = new ArrayList();
						/// Reader returns fields:
						///	 ResourceId, TaskId, UserId, MustBeConfirmed, ResponsePending, IsConfirmed, 
						///	 PercentCompleted, ActualFinishDate, LastSavedDate, IsGroup, IsExternal
						using (IDataReader resList = DBTask.GetListResources(iTaskId, 0))
							while (resList.Read())
							{
								int resId = (int)resList["UserId"];
								DataRow[] foundRows;
								foundRows = assignmets.Select(string.Format("PrincipalId = {0}", resId));
								if (foundRows.Length > 0)
								{
									int roleId = (int)foundRows[0]["RoleId"];
									if (!roleidlist.Contains(roleId))
									{
										XmlNode assignment_node = assignments_node.AppendChild(TempDoc.CreateElement("Assignment"));
										assignment_node.AppendChild(TempDoc.CreateElement("TaskUID")).InnerText = iTaskId.ToString();
										assignment_node.AppendChild(TempDoc.CreateElement("ResourceUID")).InnerText = roleId.ToString();
										//DV 29.12.2007
										assignment_node.AppendChild(TempDoc.CreateElement("MustBeConfirmed")).InnerText = (bool)resList["MustBeConfirmed"] ? "1" : "0";
									}
								}
							}
					}
			}
			#endregion

			DBProjectTemplate.AddProjectTemplate(template_name, Security.CurrentUser.UserID, DateTime.UtcNow, TempDoc.OuterXml);
		}

		public static void MakeTemplateFromProject(int project_id, DataTable assignmets, string template_name, bool IsPrivate)
		{
			//MakeTemplateFromProject2(project_id, assignmets, template_name, false);
			//return;
			XmlDocument TempDoc = new XmlDocument();
			XmlNode project_node = TempDoc.AppendChild(TempDoc.CreateElement("Project"));
			XmlNode tasks_node = project_node.AppendChild(TempDoc.CreateElement("Tasks"));
			XmlNode resources_node = project_node.AppendChild(TempDoc.CreateElement("Resources"));
			XmlNode assignments_node = project_node.AppendChild(TempDoc.CreateElement("Assignments"));


			/*
			 <Resources>
				<Resource>
					<UID>3</UID>
					<Name>
			PrincipalId
			PrincipalName
			RoleName
			RoleId
			*/

			int i = 1;
			for (int k = 0; k < assignmets.Rows.Count; k++)
				assignmets.Rows[k]["RoleId"] = 0;

			for (int k = 0; k < assignmets.Rows.Count; k++)
			{
				if ((int)assignmets.Rows[k]["RoleId"] == 0)
				{
					assignmets.Rows[k]["RoleId"] = i;

					XmlNode resource_node = resources_node.AppendChild(TempDoc.CreateElement("Resource"));
					resource_node.AppendChild(TempDoc.CreateElement("UID")).InnerText = i.ToString();
					resource_node.AppendChild(TempDoc.CreateElement("Name")).InnerText = assignmets.Rows[k]["RoleName"].ToString();

					for (int j = k + 1; j < assignmets.Rows.Count; j++)
						if (assignmets.Rows[k]["RoleName"].ToString().ToUpper() == assignmets.Rows[j]["RoleName"].ToString().ToUpper())
						{
							assignmets.Rows[j]["RoleName"] = assignmets.Rows[k]["RoleName"];
							assignmets.Rows[j]["RoleId"] = i;
						}

					i++;
				}
			}

			/*
			<Tasks>
				<Task>
					<ID>3</ID>
					<UID>3</UID>
					<Name>
					<Description>
					<OutlineNumber>1.1.1</OutlineNumber> 
					<OutlineLevel>3</OutlineLevel> 
					<Priority>500</Priority>
					<Duration>PT40H0M0S</Duration>
					<Milestone>0</Milestone> 
					<Summary>0</Summary> 
					 <PredecessorLink>
						<PredecessorUID>4</PredecessorUID> 
						<LinkLag>0</LinkLag> 
					</PredecessorLink>
					<ActivationTypeId>[int value]</ActivationTypeId>
					<CompletionTypeId>[int value]</CompletionTypeId>
					<PhaseId>[int value]</PhaseId>
					<MustBeConfirmed>1 or 0</MustBeConfirmed>
			*/

			using (IDataReader TaskList = Task.GetListTasksByProject(project_id))
				while (TaskList.Read())
				{
					int iTaskId = (int)TaskList["TaskId"];
					XmlNode task_node = tasks_node.AppendChild(TempDoc.CreateElement("Task"));

					using (IDataReader reader = Task.GetTask(iTaskId))
					{
						if (reader.Read())
						{
							task_node.AppendChild(TempDoc.CreateElement("UID")).InnerText = iTaskId.ToString();
							task_node.AppendChild(TempDoc.CreateElement("ID")).InnerText = ((int)reader["TaskNum"]).ToString();
							task_node.AppendChild(TempDoc.CreateElement("Name")).InnerText = reader["Title"].ToString();
							task_node.AppendChild(TempDoc.CreateElement("Description")).InnerText = reader["Description"].ToString();
							task_node.AppendChild(TempDoc.CreateElement("OutlineNumber")).InnerText = reader["OutlineNumber"].ToString();
							task_node.AppendChild(TempDoc.CreateElement("OutlineLevel")).InnerText = ((int)reader["OutlineLevel"]).ToString();
							task_node.AppendChild(TempDoc.CreateElement("Priority")).InnerText = ((int)reader["PriorityId"]).ToString();
							task_node.AppendChild(TempDoc.CreateElement("Duration")).InnerText = reader["Duration"].ToString();
							task_node.AppendChild(TempDoc.CreateElement("Milestone")).InnerText = (bool)reader["IsMilestone"] ? "1" : "0";
							task_node.AppendChild(TempDoc.CreateElement("Summary")).InnerText = (bool)reader["IsSummary"] ? "1" : "0";
							task_node.AppendChild(TempDoc.CreateElement("ActivationTypeId")).InnerText = ((int)reader["ActivationTypeId"]).ToString();
							//DV 01.10.2006
							task_node.AppendChild(TempDoc.CreateElement("CompletionTypeId")).InnerText = ((int)reader["CompletionTypeId"]).ToString();

							//DV 24.09.2007
							if ((bool)reader["IsMilestone"] && reader["PhaseId"] != DBNull.Value)
							{
								task_node.AppendChild(TempDoc.CreateElement("PhaseId")).InnerText = ((int)reader["PhaseId"]).ToString();
							}
							task_node.AppendChild(TempDoc.CreateElement("MustBeConfirmed")).InnerText = (bool)reader["MustBeConfirmed"] ? "1" : "0";
							task_node.AppendChild(TempDoc.CreateElement("ActivationTypeId")).InnerText = ((int)reader["ActivationTypeId"]).ToString();

						}

					}

					XmlNode pred_list = null;
					/// LinkId, PredId, SuccId, Lag 
					/// </summary>
					/// <returns></returns>
					using (IDataReader linkList = DBTask.GetListPredecessorsDetails(iTaskId, 0))
						while (linkList.Read())
						{
							//if(pred_list == null) //
							pred_list = task_node.AppendChild(TempDoc.CreateElement("PredecessorLink"));

							pred_list.AppendChild(TempDoc.CreateElement("PredecessorUID")).InnerText = ((int)linkList["TaskId"]).ToString();
							pred_list.AppendChild(TempDoc.CreateElement("LinkLag")).InnerText = ((int)linkList["Lag"]).ToString();
						}

					/*
					<Assignments>
						<Assignment>
						<UID></UID> 
						<TaskUID></TaskUID> 
						<ResourceUID></ResourceUID> 
					 */
					ArrayList roleidlist = new ArrayList();
					/// Reader returns fields:
					///	 ResourceId, TaskId, UserId, MustBeConfirmed, ResponsePending, IsConfirmed, 
					///	 PercentCompleted, ActualFinishDate, LastSavedDate, IsGroup, IsExternal
					using (IDataReader resList = DBTask.GetListResources(iTaskId, 0))
						while (resList.Read())
						{
							int resId = (int)resList["UserId"];
							DataRow[] foundRows;
							foundRows = assignmets.Select(string.Format("PrincipalId = {0}", resId));
							if (foundRows.Length > 0)
							{
								int roleId = (int)foundRows[0]["RoleId"];
								if (!roleidlist.Contains(roleId))
								{
									XmlNode assignment_node = assignments_node.AppendChild(TempDoc.CreateElement("Assignment"));
									assignment_node.AppendChild(TempDoc.CreateElement("TaskUID")).InnerText = iTaskId.ToString();
									assignment_node.AppendChild(TempDoc.CreateElement("ResourceUID")).InnerText = roleId.ToString();
									//DV 29.12.2007
									assignment_node.AppendChild(TempDoc.CreateElement("MustBeConfirmed")).InnerText = (bool)resList["MustBeConfirmed"] ? "1" : "0";
								}
							}
						}
				}

			DBProjectTemplate.AddProjectTemplate(template_name, Security.CurrentUser.UserID, DateTime.UtcNow, TempDoc.OuterXml);
		}

		public static DataTable MakeProjectAssignments(int project_template_id)
		{
			DataTable table = new DataTable();
			DataColumn Column = table.Columns.Add("RoleId", typeof(int));
			//table.PrimaryKey = new DataColumn[] {Column};
			table.Columns.Add("RoleName", typeof(string));
			table.Columns.Add("PrincipalId", typeof(int));
			table.Columns.Add("PrincipalName", typeof(string));

			using (IDataReader reader = DBProjectTemplate.GetProjectTemplate(project_template_id, 0))
			{
				XmlDocument doc = new XmlDocument();
				if (reader.Read())
					doc.LoadXml(System.Web.HttpUtility.HtmlDecode(reader["TemplateData"].ToString()));
				XmlNodeList reslist = doc.SelectNodes("Project/Resources/Resource");
				foreach (XmlNode node in reslist)
				{
					string name = node.SelectSingleNode("Name").InnerText;
					int roleId = Convert.ToInt32(node.SelectSingleNode("UID").InnerText);
					table.Rows.Add(new object[] { roleId, name, 0, "" });
				}
			}

			return table;
		}



		//DV 31.10.2006
		public static void MakeProjectMetaRolesFromTemplate(int project_template_id, int project_id, int type_id)
		{
			using (IDataReader reader = DBProjectTemplate.GetProjectTemplate(project_template_id, 0))
			{
				XmlDocument doc = new XmlDocument();

				if (reader.Read())
					doc.LoadXml(System.Web.HttpUtility.HtmlDecode(reader["TemplateData"].ToString()));

				#region --- Add Team to project ---
				DataTable team = new DataTable();
				team.Columns.Add(new DataColumn("ProjectId", typeof(int)));
				team.Columns.Add(new DataColumn("UserId", typeof(int)));
				team.Columns.Add(new DataColumn("Code", typeof(string)));
				team.Columns.Add(new DataColumn("Rate", typeof(decimal)));
				foreach (XmlNode node in doc.SelectNodes("Project/Roles/Team/TeamMember"))
				{
					int uid = int.Parse(node.SelectSingleNode("UID").InnerText);
					if (User.GetUserPreferences(uid).Read())
					{
						team.Rows.Add(new object[] { project_id, uid, node.SelectSingleNode("Code").InnerText, decimal.Parse(node.SelectSingleNode("Rate").InnerText) });
					}
				}

				Project2.UpdateTeamMembers(project_id, team);
				#endregion

				#region --- Add Stakeholders to project ---
				ArrayList Stake = new ArrayList();
				foreach (XmlNode node in doc.SelectNodes("Project/Roles/Stake/StakeMember"))
				{
					int uid = int.Parse(node.SelectSingleNode("UID").InnerText);
					if (SecureGroup.GetGroup(uid).Read() || User.GetUserPreferences(uid).Read())
					{
						Stake.Capacity++;
						Stake.Add(uid);
					}
				}
				if (Stake.Count > 0)
					Project2.UpdateStakeholders(project_id, Stake);
				#endregion

				#region --- Add Sponsors to project ---
				ArrayList Sponsor = new ArrayList();
				foreach (XmlNode node in doc.SelectNodes("Project/Roles/Sponsor/SponsorMember"))
				{
					int uid = int.Parse(node.SelectSingleNode("UID").InnerText);
					if (SecureGroup.GetGroup(uid).Read() || User.GetUserPreferences(uid).Read())
					{
						Sponsor.Capacity++;
						Sponsor.Add(uid);
					}
				}
				if (Sponsor.Count > 0)
					Project2.UpdateSponsors(project_id, Sponsor);
				#endregion

				#region --- Import Meta Fields ---
				if (doc.SelectSingleNode("Project/ProjectInfo/TypeId") != null)
				{
					int oldTypeId = int.Parse(doc.SelectSingleNode("Project/ProjectInfo/TypeId").InnerText);

					if (oldTypeId == type_id)
					{
						//MetaObject obj = MetaDataWrapper.LoadMetaObject(project_id, String.Format("ProjectsEx_{0}", projType));
						MetaClass proj = MetaClass.Load(String.Format("ProjectsEx_{0}", type_id));
						MetaObject obj = MetaDataWrapper.NewMetaObject(project_id, String.Format("ProjectsEx_{0}", type_id));

						//Import Meta Fields
						foreach (XmlNode node in doc.SelectNodes("Project/ProjectInfo/MetaFields/MetaField"))
						{
							string metaFieldName = node.SelectSingleNode("Name").InnerText;
							if (node.SelectSingleNode("Value") != null)
							{
								string stringValue = string.Empty; //XmlConvert.DecodeName(node.SelectSingleNode("Value").InnerText);
								object realValue = null;

								//DV 29.12.2007 fix (meta feld for this project type was deleted
								if (proj.MetaFields[metaFieldName] == null)
								{
									//if meta field was deleted
									continue;
								}
								if (node.SelectSingleNode("Value") != null)
								{
									stringValue = XmlConvert.DecodeName(node.SelectSingleNode("Value").InnerText);
								}
								else
								{
									switch (proj.MetaFields[metaFieldName].DataType)
									{
										case MetaDataType.Bit:
										case MetaDataType.Boolean:
											stringValue = "false";
											break;
										case MetaDataType.BigInt:
										case MetaDataType.Numeric:
										case MetaDataType.Int:
										case MetaDataType.Integer:
										case MetaDataType.Float:
										case MetaDataType.Real:
										case MetaDataType.TinyInt:
										case MetaDataType.Decimal:
										case MetaDataType.DictionarySingleValue:
										case MetaDataType.DictionaryMultivalue:
										case MetaDataType.SmallMoney:
										case MetaDataType.Money:
											stringValue = "0";
											break;
										case MetaDataType.SmallDateTime:
										case MetaDataType.DateTime:
										case MetaDataType.Date:
											stringValue = DateTime.Now.ToString();
											break;
										default:
											stringValue = string.Empty;
											break;
									}
								}

								switch (proj.MetaFields[metaFieldName].DataType)
								{
									case MetaDataType.BigInt:
										if (stringValue != "")
											realValue = Int64.Parse(stringValue);
										break;
									case MetaDataType.Bit:
									case MetaDataType.Boolean:
										if (stringValue != "")
											realValue = bool.Parse(stringValue);
										break;
									case MetaDataType.NChar:
									case MetaDataType.Char:
										if (stringValue != "")
											realValue = char.Parse(stringValue);
										break;
									case MetaDataType.SmallDateTime:
									case MetaDataType.DateTime:
									case MetaDataType.Date:
										if (stringValue != "")
											realValue = DateTime.Parse(stringValue);
										break;
									case MetaDataType.Decimal:
										if (stringValue != "")
											realValue = decimal.Parse(stringValue);
										break;
									case MetaDataType.Float:
										if (stringValue != "")
											realValue = double.Parse(stringValue);
										break;
									case MetaDataType.Numeric:
									case MetaDataType.Int:
									case MetaDataType.Integer:
										if (stringValue != "")
											realValue = int.Parse(stringValue);
										break;
									case MetaDataType.SmallMoney:
									case MetaDataType.Money:
										if (stringValue != "")
											realValue = decimal.Parse(stringValue);
										break;
									case MetaDataType.NVarChar:
									case MetaDataType.NText:
									case MetaDataType.Text:
									case MetaDataType.VarChar:
									case MetaDataType.Sysname:
									case MetaDataType.Email:
									case MetaDataType.Url:
									case MetaDataType.ShortString:
									case MetaDataType.LongString:
									case MetaDataType.LongHtmlString:
										realValue = stringValue;
										break;
									case MetaDataType.Real:
										realValue = Single.Parse(stringValue);
										break;
									case MetaDataType.UniqueIdentifier:
										realValue = new Guid(stringValue);
										break;
									case MetaDataType.SmallInt:
										realValue = Int16.Parse(stringValue);
										break;
									case MetaDataType.Timestamp:
										break;
									case MetaDataType.TinyInt:
										realValue = byte.Parse(stringValue);
										break;

									case MetaDataType.Image:
									case MetaDataType.VarBinary:
									case MetaDataType.Binary:
									case MetaDataType.Variant:
									case MetaDataType.DictionarySingleValue:
									case MetaDataType.DictionaryMultivalue:
									case MetaDataType.EnumSingleValue:
									case MetaDataType.EnumMultivalue:
									case MetaDataType.StringDictionary:
									case MetaDataType.File:
									case MetaDataType.ImageFile:
									case MetaDataType.MetaObject:
										// Nothing Do
										break;
								}

								if (proj.MetaFields[metaFieldName].DataType == MetaDataType.DictionarySingleValue)
								{
									try
									{
										MetaDictionaryItem item = proj.MetaFields[metaFieldName].Dictionary.GetItem(int.Parse(stringValue));
										obj[metaFieldName] = item;
									}
									catch (FormatException)
									{
									}

								}
								else if (proj.MetaFields[metaFieldName].DataType == MetaDataType.DictionaryMultivalue)
								{
									try
									{
										ArrayList items = new ArrayList();
										foreach (string id in stringValue.Split(','))
										{
											MetaDictionaryItem item = proj.MetaFields[metaFieldName].Dictionary.GetItem(int.Parse(id));
											items.Add(item);
										}
										obj[metaFieldName] = items.ToArray(typeof(MetaDictionaryItem));
									}
									catch (FormatException)
									{
									}
								}
								else
								{
									if (realValue != null)
									{
										obj[metaFieldName] = realValue;// XmlConvert.DecodeName(node.SelectSingleNode("Value").InnerText);	
									}
								}
							}
						}

						obj.AcceptChanges();
					}
				}
				#endregion
			}
		}

		//DV 31.10.2006
		public static void MakeProjectSysFieldsFromTemplate(int project_template_id, DataTable SysFields, ArrayList GeneralCategories, ArrayList ProjectCategories)
		{
			using (IDataReader reader = DBProjectTemplate.GetProjectTemplate(project_template_id, 0))
			{
				XmlDocument doc = new XmlDocument();

				if (reader.Read())
					doc.LoadXml(System.Web.HttpUtility.HtmlDecode(reader["TemplateData"].ToString()));

				XmlNodeList sys_nodes = doc.SelectNodes("Project/ProjectInfo");

				if (SysFields == null)
					SysFields = new DataTable();
				//SysFields.Columns.Add("Title", typeof(string));
				SysFields.Columns.Add("Goals", typeof(string));
				SysFields.Columns.Add("Description", typeof(string));
				SysFields.Columns.Add("Deliverables", typeof(string));
				SysFields.Columns.Add("Scope", typeof(string));
				SysFields.Columns.Add("Priority", typeof(int));
				SysFields.Columns.Add("ProjectTypeId", typeof(int));
				SysFields.Columns.Add("CalendarId", typeof(int));
				SysFields.Columns.Add("CurrencyId", typeof(int));
				SysFields.Columns.Add("TypeId", typeof(int));
				SysFields.Columns.Add("ManagerId", typeof(int));
				SysFields.Columns.Add("ExecutiveManagerId", typeof(int));



				foreach (XmlNode node in sys_nodes)
				{
					//string title = node.SelectSingleNode("Title").InnerText;

					//DV FIX [2007-06-18] Proverka byli li sohraneny system fields
					if (node.SelectSingleNode("Goals") == null)
						continue;

					string goals = XmlConvert.DecodeName(node.SelectSingleNode("Goals").InnerText);
					string desription = XmlConvert.DecodeName(node.SelectSingleNode("Description").InnerText);
					string deliverables = XmlConvert.DecodeName(node.SelectSingleNode("Deliverables").InnerText);
					string scope = XmlConvert.DecodeName(node.SelectSingleNode("Scope").InnerText);
					int priority = int.Parse(node.SelectSingleNode("Priority").InnerText);
					int projtypeid = int.Parse(node.SelectSingleNode("ProjectTypeId").InnerText);

					int calendarid = int.Parse(node.SelectSingleNode("CalendarId").InnerText);
					int currencyid = int.Parse(node.SelectSingleNode("CurrencyId").InnerText);
					int typeid = int.Parse(node.SelectSingleNode("TypeId").InnerText);

					int managerid = -1;
					if (node.SelectSingleNode("ManagerId") != null)
						managerid = int.Parse(node.SelectSingleNode("ManagerId").InnerText);
					else
						managerid = Security.CurrentUser.UserID;

					int executiveid = -1;
					if (node.SelectSingleNode("ExecutiveManagerId") != null)
						executiveid = int.Parse(node.SelectSingleNode("ExecutiveManagerId").InnerText);

					SysFields.Rows.Add(new object[] { goals, desription, deliverables, scope, priority, projtypeid, calendarid, currencyid, typeid, managerid, executiveid });

					foreach (XmlNode node2 in node.SelectNodes("GeneralCategories/Category"))
					{
						GeneralCategories.Capacity++;
						GeneralCategories.Add(int.Parse(node2.SelectSingleNode("UID").InnerText));
					}
					foreach (XmlNode node2 in node.SelectNodes("ProjectCategories/Category"))
					{
						ProjectCategories.Capacity++;
						ProjectCategories.Add(int.Parse(node2.SelectSingleNode("UID").InnerText));
					}
				}

			}

		}

		public static void MakeProjectFromTemplate(int project_template_id, int project_id, DataTable assignmets)
		{

			XmlDocument TempXml = new XmlDocument();
			#region Load xml

			using (IDataReader reader = DBProjectTemplate.GetProjectTemplate(project_template_id, 0))
			{
				if (reader.Read())
				{
					string ss = System.Web.HttpUtility.HtmlDecode(reader["TemplateData"].ToString());
					TempXml.LoadXml(ss);
				}
			}
			#endregion

			int ManagerId = -1;
			using (IDataReader reader = DBProject.GetProject(project_id, User.DefaultTimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (reader.Read())
				{
					ManagerId = Convert.ToInt32(reader["ManagerId"]);
				}
			}

			/*            //TODO !!!
						int ratePerUser = 0;
						DataTable table = Company.GetCompanyInfo();
						if (table.Rows.Count > 0)
							ratePerUser = (int)table.Rows[0]["rate_per_user"];
			*/
			//TODO: transaction
			using (DbTransaction tran = DbTransaction.Begin())
			{

				#region Udalit' vse taski

				ArrayList OldTasks = new ArrayList();
				using (IDataReader OldTaskReader = DBTask.GetListTasksByProject(project_id, Security.CurrentUser.LanguageId, Security.CurrentUser.TimeZoneId))
				{
					while (OldTaskReader.Read())
					{
						OldTasks.Add((int)OldTaskReader["TaskId"]);
					}
				}

				foreach (int TaskId in OldTasks)
				{
					TimeTracking.ResetObjectId((int)TASK_TYPE, TaskId);

					MetaObject.Delete(TaskId, "TaskEx");

					// OZ: 2008-07-16 Clear Task File Library and remove
					// OZ: File Storage
					BaseIbnContainer container = BaseIbnContainer.Create("FileLibrary", UserRoleHelper.CreateTaskContainerKey(TaskId));
					FileStorage taskFs = (FileStorage)container.LoadControl("FileStorage");
					taskFs.DeleteAll(); // DeleteAll call ForeignContainerKey.Delete(UserRoleHelper.CreateTaskContainerKey(TaskId));

					// OZ: User Roles
					UserRoleHelper.DeleteTaskRoles(TaskId);
				}
				DBTask.DeleteAll(project_id);
				#endregion


				#region Tasks;  построение структуры

				int UID;
				int ID;
				string Name;
				string OutlineNumber;
				int OutlineLevel;
				int intPriority;
				bool Milestone;
				bool Summary;
				int Duration;
				int CompletionTypeId = (int)CompletionType.Any;
				int ActivationTypeId = (int)ActivationTypes.AutoWithCheck;
				String Notes;
				int PhaseId = -1;
				bool MustBeConfirmed = false;

				//DateTime Start;
				//DateTime Finish;


				int TaskNum = 0;
				int PrevOutlineLevel = 1;
				string PrevOutlineNumber = "0";
				bool PrevIsSummary = false;
				DateTime CurrentDate = DateTime.UtcNow;

				XmlNodeList XmlTasksList = TempXml.SelectNodes("Project/Tasks/Task");


				foreach (XmlNode task_node in XmlTasksList)
				{
					//required fields
					ID = XmlConvert.ToInt32(task_node["ID"].InnerText);
					UID = XmlConvert.ToInt32(task_node["UID"].InnerText);
					Name = task_node["Name"] == null ? "_________" : XmlConvert.DecodeName(task_node["Name"].InnerText);
					OutlineNumber = task_node["OutlineNumber"].InnerText;
					OutlineLevel = XmlConvert.ToInt32(task_node["OutlineLevel"].InnerText);
					intPriority = XmlConvert.ToInt32(task_node["Priority"].InnerText);
					Milestone = XmlConvert.ToInt32(task_node["Milestone"].InnerText) == 1 ? true : false;
					Summary = XmlConvert.ToInt32(task_node["Summary"].InnerText) == 1 ? true : false;
					Duration = XmlConvert.ToInt32(task_node["Duration"].InnerText);
					Notes = XmlConvert.DecodeName(task_node["Description"].InnerText);

					//DV: 24.09.2007
					if (Milestone && task_node["PhaseId"] != null)
						PhaseId = XmlConvert.ToInt32(task_node["PhaseId"].InnerText);

					if (task_node["MustBeConfirmed"] != null)
						MustBeConfirmed = XmlConvert.ToInt32(task_node["MustBeConfirmed"].InnerText) == 1 ? true : false;

					//DV: 01.10.2006
					if (task_node["CompletionTypeId"] != null)
						CompletionTypeId = XmlConvert.ToInt32(task_node["CompletionTypeId"].InnerText);

					//DV: 15.05.2008
					if (task_node["ActivationTypeId"] != null)
						ActivationTypeId = XmlConvert.ToInt32(task_node["ActivationTypeId"].InnerText);

					if (OutlineLevel < 1)
						continue; //throw new Exception("wrong xml");

					if (PrevOutlineLevel < OutlineLevel)
					{
						if (PrevOutlineLevel + 1 == OutlineLevel)
						{
							if (!PrevIsSummary)
								throw new Exception("wrong xml");

							OutlineNumber = PrevOutlineNumber + ".1";
						}
						else
							throw new Exception("wrong xml");
					}
					else
						if (PrevOutlineLevel >= OutlineLevel)
						{
							if (PrevIsSummary)
								throw new Exception("wrong xml");

							if (PrevOutlineLevel > OutlineLevel)
							{
								for (int i = 0; i < PrevOutlineLevel - OutlineLevel; i++)
								{
									int ind = PrevOutlineNumber.LastIndexOf(".");
									PrevOutlineNumber = PrevOutlineNumber.Substring(0, ind);
								}
							}

							string LeftPart;
							int val;
							int index = PrevOutlineNumber.LastIndexOf(".");

							if (index == -1)
							{
								LeftPart = "";
								val = Convert.ToInt32(PrevOutlineNumber);
							}
							else
							{
								LeftPart = PrevOutlineNumber.Substring(0, index + 1);
								val = Convert.ToInt32(PrevOutlineNumber.Substring(index + 1));
							}
							val++;
							OutlineNumber = LeftPart + val.ToString();
						}


					if (TaskNum == 1 && PrevOutlineLevel != 1)
						throw new Exception("wrong xml");


					TaskNum++;
					PrevOutlineLevel = OutlineLevel;
					PrevOutlineNumber = OutlineNumber;
					PrevIsSummary = Summary;


					if (intPriority < 400)
						intPriority = (int)Priority.Low;
					else
						if (intPriority >= 400 && intPriority < 600)
							intPriority = (int)Priority.Normal;
						else
							if (intPriority >= 600 && intPriority < 700)
								intPriority = (int)Priority.High;
							else
								if (intPriority >= 700 && intPriority < 900)
									intPriority = (int)Priority.VeryHigh;
								else
									if (intPriority >= 900 && intPriority <= 1000)
										intPriority = (int)Priority.Urgent;
									else
										intPriority = (int)Priority.Normal;

					int UserId = Security.CurrentUser.UserID;
					//current date as we change it after
					// - Start,Finish,Duration
					// + Description, PercentComplete, may Resone,ActualFinish

					int TaskId = DBTask.Create(project_id, Name, Notes, UserId, CurrentDate, CurrentDate, CurrentDate,
						CurrentDate.AddMinutes(Duration), Duration, intPriority, (int)ConstraintTypes.AsSoonAsPossible,
						ActivationTypeId, CompletionTypeId, TaskNum, Milestone, Summary,
						OutlineNumber, OutlineLevel, 0, -1);
					/*DBTask.Create(project_id, UserId, Name, Notes, CurrentDate, CurrentDate, CurrentDate, Duration, intPriority, Milestone, (int)ConstraintTypes.AsSoonAsPossible,
						CurrentDate, (int)ActivationTypes.AutoWithCheck, CompletionTypeId, MustBeConfirmed, PhaseId, 0);*/

					//update MustBeConfirmed
					DBTask.Update(TaskId, Name, Notes, CurrentDate, CurrentDate.AddMinutes(Duration), Duration, intPriority, Milestone, (int)ConstraintTypes.AsSoonAsPossible,
						CurrentDate, ActivationTypeId, CompletionTypeId, MustBeConfirmed, PhaseId, 0);

					//DV: 24.09.2007
					if (Milestone && PhaseId != -1)
					{
						DBTask.Update(TaskId, Name, Notes, CurrentDate, CurrentDate, 0, intPriority, Milestone,
							(int)ConstraintTypes.AsSoonAsPossible, CurrentDate, (int)ActivationTypes.AutoWithCheck,
							(int)CompletionType.Any, MustBeConfirmed, PhaseId, 0);
					}


					// OZ: User Role Addon
					UserRoleHelper.AddTaskManagerRole(TaskId, ManagerId);
					if (ManagerId != UserId)
						UserRoleHelper.AddTaskCreatorRole(TaskId, UserId);
					ForeignContainerKey.Add(UserRoleHelper.CreateTaskContainerKey(TaskId), UserRoleHelper.CreateProjectContainerKey(project_id));
					//

					XmlNode XmlTaskId = task_node.OwnerDocument.CreateNode(XmlNodeType.Element, "TaskId", null);
					XmlTaskId.InnerText = TaskId.ToString();

					task_node.AppendChild(XmlTaskId);

					#region Resources
					/*
					<Assignments>
						 <Assignment>
							  <UID></UID> 
							  <TaskUID></TaskUID> 
							  <ResourceUID></ResourceUID> 
							  <MustBeConfirmed></MustBeConfirmed>
					*/


					ArrayList PrincipalsList = new ArrayList();
					XmlNodeList XmlAssignmentsList = TempXml.SelectNodes(string.Format("Project/Assignments/Assignment[TaskUID = \"{0}\"]", UID));

					foreach (XmlNode assingment in XmlAssignmentsList)
					{
						int ResourceUID = XmlConvert.ToInt32(assingment["ResourceUID"].InnerText);
						int ResMustBeConfirmed = 0;

						if (assingment["MustBeConfirmed"] != null)
							ResMustBeConfirmed = XmlConvert.ToInt32(assingment["MustBeConfirmed"].InnerText);

						foreach (DataRow row in assignmets.Rows)
						{
							if (((int)row["RoleId"]) == ResourceUID)
							{
								if (row["PrincipalId"] != null)
								{
									int PrincipalId = ((int)row["PrincipalId"]);
									if (!PrincipalsList.Contains(PrincipalId))
										PrincipalsList.Add(new int[] { PrincipalId, ResMustBeConfirmed });

								}

							}
						}
					}
					foreach (int[] PrincipalID in PrincipalsList)
					{
						if (PrincipalID[0] != -1)
						{
							bool CanManage = false;
							if (PrincipalID[0] == ManagerId)
								CanManage = true;

							bool _mustBeConfirmed = (PrincipalID[1] == 1);

							DBTask.AddResource(TaskId, PrincipalID[0], _mustBeConfirmed, CanManage);

							if (User.IsExternal(PrincipalID[0]))
								DBCommon.AddGate((int)TASK_TYPE, TaskId, PrincipalID[0]);

							// OZ: User Role Addon
							if (CanManage)
							{
								if (PrincipalID[0] != ManagerId)
									UserRoleHelper.AddTaskManagerRole(TaskId, PrincipalID[0]);
							}
							else
								UserRoleHelper.AddTaskResourceRole(TaskId, PrincipalID[0]);
							// 

						}
					}
					#endregion

				}

				#endregion


				#region Predecessors; создание
				//связать ресурсы
				/*
						 *	0 ОО (окончание-окончание) 
							1 ОН (окончание-начало) 
							2 НО (начало-окончание) 
							3 НН (начало-начало) 
						 */
				//int Type;
				int LinkLag;
				//int LagFormat;
				XmlNodeList XmlPredecessorLinkList = TempXml.SelectNodes("Project/Tasks/Task/PredecessorLink");

				foreach (XmlNode predlink_node in XmlPredecessorLinkList)
				{
					int SuccId = XmlConvert.ToInt32(predlink_node.ParentNode["TaskId"].InnerText);
					string PredUID = predlink_node["PredecessorUID"].InnerText;
					XmlNode PredNode = TempXml.SelectSingleNode(string.Format("Project/Tasks/Task[UID = {0}]", PredUID));
					if (PredNode == null)
						continue;
					int PredId = XmlConvert.ToInt32(PredNode["TaskId"].InnerText);
					//Type = XmlConvert.ToInt32(predlink_node["Type"].InnerText);
					LinkLag = XmlConvert.ToInt32(predlink_node["LinkLag"].InnerText);
					//LagFormat = XmlConvert.ToInt32(predlink_node["LagFormat"].Value);

					if (DBTask.TaskInAllDependOnTasks(SuccId, PredId))
						throw new Exception("from task to dependent on task");

					DBTask.CreateTaskLink(PredId, SuccId, LinkLag);
				}
				#endregion

				Mediachase.Ibn.Data.Meta.Management.MetaClass mc = Project.GetProjectMetaClass();

				#region Project Team


				ArrayList ProjectTeam = new ArrayList();
				DataTable team = Project.GetListTeamMembersDataTable(project_id);
				foreach (DataRow row in assignmets.Rows)
				{
					if (row["PrincipalId"] != null)
					{
						int PrincipalId = ((int)row["PrincipalId"]);
						if (!ProjectTeam.Contains(PrincipalId))
						{
							ProjectTeam.Add(PrincipalId);

							string Code = "";
							decimal rate = 0;
							using (IDataReader reader = User.GetUserInfo(PrincipalId, false))
							{
								if (reader.Read())
								{
									if (reader["FirstName"] != DBNull.Value)
										Code += reader["FirstName"].ToString().Substring(0, 1);
									if (reader["LastName"] != DBNull.Value)
										Code += reader["LastName"].ToString().Substring(0, 1);

									team.DefaultView.RowFilter = "UserId = " + PrincipalId.ToString();
									if (team.DefaultView.Count == 1)
										rate = Convert.ToDecimal(team.DefaultView[0]["Rate"].ToString().Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator), CultureInfo.InvariantCulture);
								}
							}

							//TODO
							DBProject.AddTeamMember(project_id, PrincipalId, Code, rate);

							// OZ: FileStorage Addon (2007-08-16)
							UserRoleHelper.AddProjectTeamRole(project_id, PrincipalId);

							// IbnNext
							Mediachase.Ibn.Data.Services.RoleManager.AddPrincipalToObjectRole(mc, project_id, (int)Project.ProjectRole.TeamMember, PrincipalId);
						}
					}
				}

				//Expand Team members with resources from tasks
				DataTable taskres = Project.GetListTaskResourcesDataTable(project_id);

				foreach (DataRow dr in taskres.Rows)
				{
					team.DefaultView.RowFilter = "UserId = " + dr["UserId"].ToString();
					if (team.DefaultView.Count == 0)
					{
						string Code = string.Empty;
						decimal rate = 0;

						if (dr["FirstName"].ToString().Length > 0)
							Code += dr["FirstName"].ToString();
						if (dr["LastName"].ToString().Length > 0)
							Code += dr["LastName"].ToString();

						if (dr["Rate"].ToString().Length > 0)
							rate = Convert.ToDecimal(dr["Rate"].ToString().Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator), CultureInfo.InvariantCulture);

						int PrincipalId = (int)dr["UserId"];

						DBProject.AddTeamMember(project_id, PrincipalId, Code, rate);

						// OZ: FileStorage Addon (2007-08-16)
						UserRoleHelper.AddProjectTeamRole(project_id, PrincipalId);

						// IbnNext
						Mediachase.Ibn.Data.Services.RoleManager.AddPrincipalToObjectRole(mc, project_id, (int)Project.ProjectRole.TeamMember, PrincipalId);
					}
				}
				#endregion

				Project.RecalculateAll(project_id);

				tran.Commit();
			}
		}

		#endregion

		#region CreatePredecessor
		public static int CreatePredecessor(int from_task_id, int to_task_id, int lag)
		{
			if (!CanUpdateConfigurationInfo(from_task_id))
				throw new AccessDeniedException();

			//TODO: lock all tasks
			int LinkID;

			//нельзя допустить зацикливания
			if (DBTask.TaskInAllDependOnTasks(to_task_id, from_task_id))
				throw new Exception("from task to dependent on task");

			int project_id = GetProject(from_task_id);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				LinkID = DBTask.CreateTaskLink(from_task_id, to_task_id, lag);

				string update_id = Guid.NewGuid().ToString();
				DBTask.TaskUpdateAddPossibleTask(update_id, to_task_id, false);

				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_PredecessorList_PredecessorAdded, to_task_id, from_task_id);
				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_SuccessorList_SuccessorAdded, from_task_id, to_task_id);

				Process(update_id);

				RecalculateAllStates(project_id);

				tran.Commit();
			}

			return LinkID;
		}
		#endregion

		#region UpdatePredecessor

		public static void UpdatePredecessor(int link_id, int lag)
		{
			int pred_id = -1;
			int succ_id = -1;
			using (IDataReader reader = DBTask.GetTaskLink(link_id))
			{
				if (reader.Read())
				{
					pred_id = (int)reader["PredId"];
					succ_id = (int)reader["SuccId"];
				}
			}

			if (!CanUpdateConfigurationInfo(pred_id))
				throw new AccessDeniedException();

			int project_id = GetProject(pred_id);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				int SuccId = DBTask.UpdateTaskLink(link_id, lag);

				//InternalUpdate(SuccId);

				string update_id = Guid.NewGuid().ToString();
				DBTask.TaskUpdateAddPossibleTask(update_id, SuccId, false);

				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_PredecessorList_PredecessorLagUpdated, pred_id, succ_id);
				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_SuccessorList_SuccessorLagUpdated, succ_id, pred_id);

				Process(update_id);

				RecalculateAllStates(project_id);

				tran.Commit();
			}
		}
		#endregion

		#region DeletePredecessor

		public static void DeletePredecessor(int link_id)
		{
			int pred_id = -1;
			int succ_id = -1;
			using (IDataReader reader = DBTask.GetTaskLink(link_id))
			{
				if (reader.Read())
				{
					pred_id = (int)reader["PredId"];
					succ_id = (int)reader["SuccId"];
				}
			}

			if (!CanUpdateConfigurationInfo(pred_id))
				throw new AccessDeniedException();

			int ProjectId = GetProject(pred_id);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				int SuccId = DBTask.DeleteTaskLink(link_id);


				string update_id = Guid.NewGuid().ToString();
				DBTask.TaskUpdateAddPossibleTask(update_id, SuccId, false);

				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_PredecessorList_PredecessorDeleted, succ_id, pred_id);
				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_SuccessorList_SuccessorDeleted, pred_id, succ_id);

				Process(update_id);

				RecalculateAllStates(ProjectId);

				tran.Commit();
			}
		}
		#endregion

		#region SUPPORT FUNCTIONS
		protected static DateTime MinDate(DateTime date1, DateTime date2)
		{
			if (date1 < date2)
				return date1;
			else
				return date2;
		}

		protected static DateTime MaxDate(DateTime date1, DateTime date2)
		{
			if (date1 > date2)
				return date1;
			else
				return date2;
		}

		protected static DateTime CalculateStartDate(int CalebdarId, DateTime ProjectTargetStartDate, DateTime MinPossibleStartDate, ConstraintTypes ConstrainType, DateTime ConstrainDate)
		{
			ProjectTargetStartDate = DBCalendar.GetFinishDateByDuration(CalebdarId, ProjectTargetStartDate, 0);

			DateTime date = MaxDate(ProjectTargetStartDate, MinPossibleStartDate);

			switch (ConstrainType)
			{
				case ConstraintTypes.AsSoonAsPossible:
					return date;
				case ConstraintTypes.StartNotEarlierThan:
					return MaxDate(date, ConstrainDate);
				case ConstraintTypes.StartNotLaterThan:
					//TODO:
					return DateTime.MinValue;
				default:
					return DateTime.MinValue;
			}
		}


		protected static DateTime CalculateFinishDate(int CalendarId, DateTime StartDate, int Duration)
		{
			return DBCalendar.GetFinishDateByDuration(CalendarId, StartDate, Duration);
		}
		#endregion

		#region InternalUpdate2

		internal static void InternalUpdate2(int task_id)
		{
			bool IsMilestone;
			bool IsSummary;
			DateTime OldStartDate;
			DateTime OldFinishDate;
			int OldDuration;
			DateTime ProjectTargetStartDate;
			int ProjectId;
			int CalendarId;
			int Constraintype;
			DateTime ConstrainDate;

			DateTime CurrentDate = DateTime.UtcNow;

			//Get task fields
			using (IDataReader TaskReader = DBTask.GetTask(task_id, User.DefaultTimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (TaskReader.Read())
				{
					ProjectId = (int)TaskReader["ProjectId"];
					OldStartDate = (DateTime)TaskReader["StartDate"];
					OldFinishDate = (DateTime)TaskReader["FinishDate"];
					OldDuration = (int)TaskReader["Duration"];
					Constraintype = (int)TaskReader["ConstraintTypeId"];
					ConstrainDate = TaskReader["ConstraintDate"].GetType() == typeof(DBNull) ? DateTime.MinValue : (DateTime)TaskReader["ConstraintDate"];
					IsMilestone = (bool)TaskReader["IsMilestone"];
					IsSummary = (bool)TaskReader["IsSummary"];
				}
				else
					throw new Exception("Task not found");
			}

			//Get project's fields
			using (IDataReader ProjectReader = DBProject.GetProject(ProjectId, User.DefaultTimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (ProjectReader.Read())
				{
					ProjectTargetStartDate = (DateTime)ProjectReader["TargetStartDate"];
					CalendarId = (int)ProjectReader["CalendarId"];
				}
				else
					throw new Exception("Project not found");
			}

			//Get Min possible date for start Date
			DateTime MinPossibleStartDate = DBTask.GetMinPossibleStartDate(task_id, CalendarId);

			DateTime NewStartDate = OldStartDate;
			DateTime NewFinishDate = OldFinishDate;
			int NewDuration = OldDuration;
			//			DateTime MinDateTime;

			if (IsSummary)
			{
				//FinishDate of pred + lag and Finish date of pred of summaries + lag
				NewStartDate = DBTask.GetMinStartDateOfChild(task_id);
				NewFinishDate = DBTask.GetMaxFinishDateOfChild(task_id);
				NewStartDate = MaxDate(NewStartDate, MinPossibleStartDate);
				//NewDuration = DBCalendar.GetDurationByFinishDate(CalendarId,NewStartDate,NewFinishDate); 
			}
			else
			{
				NewStartDate = CalculateStartDate(CalendarId, ProjectTargetStartDate, MinPossibleStartDate, (ConstraintTypes)Constraintype, ConstrainDate);
				NewStartDate = DBCalendar.GetFinishDateByDuration(CalendarId, NewStartDate, 0);
				// 2007-04-28 Oleg Rylin: Fix for Calendar changing
				//				if(NewStartDate != OldStartDate)
				//				{
				if (IsMilestone)
					NewFinishDate = NewStartDate;
				else
					NewFinishDate = DBCalendar.GetFinishDateByDuration(CalendarId, NewStartDate, OldDuration);
				//				}
			}

			//TODO: Update Task !!!!!!!!
			DBTask.Update(task_id, NewStartDate, NewFinishDate, NewDuration, Constraintype, ConstrainDate);
		}

		#endregion

		#region InternalUpdate3
		internal static void InternalUpdate3(int task_id, string update_id)
		{
			//Task's fields
			bool IsMilestone;
			bool IsSummary;
			DateTime OldStartDate;
			DateTime OldFinishDate;
			int OldDuration;
			DateTime ProjectTargetStartDate;
			int ProjectId;
			int CalendarId;
			int Constraintype;
			DateTime ConstrainDate;

			DateTime CurrentDate = DateTime.UtcNow;

			//Get task fields
			using (IDataReader TaskReader = DBTask.GetTask(task_id, User.DefaultTimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (TaskReader.Read())
				{
					ProjectId = (int)TaskReader["ProjectId"];
					OldStartDate = (DateTime)TaskReader["StartDate"];
					OldFinishDate = (DateTime)TaskReader["FinishDate"];
					OldDuration = (int)TaskReader["Duration"];
					Constraintype = (int)TaskReader["ConstraintTypeId"];
					ConstrainDate = TaskReader["ConstraintDate"].GetType() == typeof(DBNull) ? DateTime.MinValue : (DateTime)TaskReader["ConstraintDate"];
					IsMilestone = (bool)TaskReader["IsMilestone"];
					IsSummary = (bool)TaskReader["IsSummary"];
				}
				else
					throw new Exception("Task not found");
			}

			//Get project's fields
			using (IDataReader ProjectReader = DBProject.GetProject(ProjectId, User.DefaultTimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (ProjectReader.Read())
				{
					ProjectTargetStartDate = (DateTime)ProjectReader["TargetStartDate"];
					CalendarId = (int)ProjectReader["CalendarId"];
				}
				else
					throw new Exception("Project not found");
			}

			//Get Min possible date for start Date
			DateTime MinPossibleStartDate = DBTask.GetMinPossibleStartDate(task_id, CalendarId);

			DateTime NewStartDate = OldStartDate;
			DateTime NewFinishDate = OldFinishDate;
			int NewDuration = OldDuration;
			//			DateTime MinDateTime;

			if (IsSummary)
			{
				//FinishDate of pred + lag and Finish date of pred of summaries + lag
				NewStartDate = DBTask.GetMinStartDateOfChild(task_id);
				NewFinishDate = DBTask.GetMaxFinishDateOfChild(task_id);
				NewStartDate = MaxDate(NewStartDate, MinPossibleStartDate);
				//NewDuration = DBCalendar.GetDurationByFinishDate(CalendarId,NewStartDate,NewFinishDate); 
			}
			else
			{
				NewStartDate = CalculateStartDate(CalendarId, ProjectTargetStartDate, MinPossibleStartDate, (ConstraintTypes)Constraintype, ConstrainDate);
				NewStartDate = DBCalendar.GetFinishDateByDuration(CalendarId, NewStartDate, 0);
				//if(NewStartDate != OldStartDate)
				//{
				if (IsMilestone)
					NewFinishDate = NewStartDate;
				else
					NewFinishDate = DBCalendar.GetFinishDateByDuration(CalendarId, NewStartDate, OldDuration);
				//}
			}


			DBTask.Update(task_id, NewStartDate, NewFinishDate, NewDuration, Constraintype, ConstrainDate);

			DBTask.TaskUpdateDeleteTask(update_id, task_id);

			if (NewFinishDate != OldFinishDate || NewStartDate != OldStartDate)
			{
				int ParentId = DBTask.GetParent(task_id);
				if (ParentId != -1)
					DBTask.TaskUpdateAddPossibleTask(update_id, ParentId, true);
			}

			if (NewFinishDate != OldFinishDate)
			{
				DBTask.TaskUpdateAddPossibleSuccessor(update_id, task_id);

				// O.R. [2008-08-11]: TaskDates
				DBTask.AddTaskDates(ProjectId, task_id, Security.CurrentUser.UserID, OldFinishDate, NewFinishDate);
			}
		}
		#endregion

		#region Process

		internal static void Process(string update_id)
		{
			int task_id = 0;
			while (task_id != -1)
			{
				task_id = DBTask.TaskUpdateGetTask(update_id);
				if (task_id != -1)
					InternalUpdate3(task_id, update_id);
			}
			DBTask.TaskUpdateDeleteAll(update_id);

		}
		#endregion

		#region RecalculateAllStates
		public static void RecalculateAllStates(int projectId)
		{
			ArrayList changedTasks = new ArrayList();
			ArrayList tasks = new ArrayList();
			using (IDataReader reader = DBTask.GetListTasksInRecalculateOrder(projectId))
			{
				while (reader.Read())
				{
					int taskId = (int)reader["TaskId"];
					tasks.Add(taskId);
				}
			}

			DateTime calculatedStartDate = DateTime.MaxValue;
			DateTime calculatedFinishDate = DateTime.MinValue;

			foreach (int taskId in tasks)
			{
				int newStateId, oldStateId;
				DateTime startDate, finishDate;
				using (IDataReader reader = DBTask.TaskRecalculateState(taskId, DateTime.UtcNow))
				{
					reader.Read();
					newStateId = (int)reader["NewStateId"];
					oldStateId = (int)reader["OldStateId"];
					startDate = (DateTime)reader["StartDate"];
					finishDate = (DateTime)reader["FinishDate"];
				}

				if (startDate < calculatedStartDate)
					calculatedStartDate = startDate;
				if (finishDate > calculatedFinishDate)
					calculatedFinishDate = finishDate;

				if (newStateId != oldStateId)
				{
					// O.R [2/3/2006]
					changedTasks.Add(taskId);

					//:todo
					//activation, close date

					if ((newStateId == (int)ObjectStates.Completed || newStateId == (int)ObjectStates.Suspended)
						&& (oldStateId == (int)ObjectStates.Upcoming || oldStateId == (int)ObjectStates.Active || oldStateId == (int)ObjectStates.Overdue))
						DBTask.UpdateCompletedBy(taskId, Security.CurrentUser.UserID);
					else if ((oldStateId == (int)ObjectStates.Completed || oldStateId == (int)ObjectStates.Suspended)
						&& (newStateId == (int)ObjectStates.Upcoming || newStateId == (int)ObjectStates.Active || newStateId == (int)ObjectStates.Overdue))
						DBTask.ResetCompletedBy(taskId);
					else if (newStateId == (int)ObjectStates.Active && oldStateId == (int)ObjectStates.Upcoming)
						DBTask.UpdateActualStart(taskId);
				}

				if (newStateId == (int)ObjectStates.Completed || newStateId == (int)ObjectStates.Suspended)
				{
					startDate = DateTime.MinValue;
					finishDate = DateTime.MinValue;
				}

				Schedule.UpdateDateTypeValue(DateTypes.Task_StartDate, taskId, startDate);
				Schedule.UpdateDateTypeValue(DateTypes.Task_FinishDate, taskId, finishDate);
			}

			// O.R. [2009-06-15]: Incident dates
			if (PortalConfig.UseIncidentDatesForProject)
			{
				using (IDataReader reader = DBIncident.GetIncidentDatesForProject(projectId))
				{
					if (reader.Read() && reader["StartDate"] != DBNull.Value && reader["FinishDate"] != DBNull.Value)
					{
						DateTime startDate = (DateTime)reader["StartDate"];
						DateTime finishDate = (DateTime)reader["FinishDate"];

						if (startDate < calculatedStartDate)
							calculatedStartDate = startDate;
						if (finishDate > calculatedFinishDate)
							calculatedFinishDate = finishDate;
					}
				}
			}

			if (calculatedStartDate <= calculatedFinishDate)
				DBProject.UpdateDates(projectId, calculatedStartDate, calculatedFinishDate);
			else
				DBProject.UpdateDates(projectId, null, null);

			int userId = Security.CurrentUser.UserID;
			int timeZoneId = Security.CurrentUser.TimeZoneId;
			int languageId = Security.CurrentUser.LanguageId;

			ArrayList todoList = new ArrayList();
			foreach (int taskId in changedTasks)
			{
				// O.R [2/3/2006]: Событие об изменении состояния задачи генерим здесь
				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_State, taskId);

				// O.R [2/7/2006]: Формируем список ToDo
				using (IDataReader reader = DBTask.GetListToDo(taskId, userId, timeZoneId, languageId))
				{
					while (reader.Read())
					{
						if (!(bool)reader["IsCompleted"])
							todoList.Add((int)reader["ToDoId"]);
					}
				}
			}

			foreach (int todoId in todoList)
				ToDo.RecalculateState(todoId);
		}
		#endregion

		#region TasksGetMilestonesCount
		public static int TasksGetMilestonesCount(int ProjectId)
		{
			return DBTask.TaskGetMilestonesCount(ProjectId);
		}

		#endregion


		#region ============== Rylin's Block ============

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

		#region TaskSecurity
		public class TaskSecurity
		{
			public bool IsResource = false;
			public bool IsManager = false;
			public bool IsRealTaskResource = false;
			public bool IsCreator = false;

			public TaskSecurity(int task_id, int user_id)
			{
				using (IDataReader reader = DBTask.GetSecurityForUser(task_id, user_id))
				{
					if (reader.Read())
					{
						IsResource = ((int)reader["IsResource"] > 0) ? true : false;
						IsManager = ((int)reader["IsManager"] > 0) ? true : false;
						IsRealTaskResource = ((int)reader["IsRealTaskResource"] > 0) ? true : false;
						IsCreator = ((int)reader["IsCreator"] > 0) ? true : false;
					}
				}
			}
		}
		#endregion

		#region ConstraintType
		public enum ConstraintTypes
		{
			AsSoonAsPossible = 0,
			StartNotEarlierThan = 4,
			StartNotLaterThan = 5
		}
		#endregion

		#region GetSecurity
		public static TaskSecurity GetSecurity(int task_id)
		{
			return GetSecurity(task_id, Security.CurrentUser.UserID);
		}

		public static TaskSecurity GetSecurity(int task_id, int user_id)
		{
			return new TaskSecurity(task_id, user_id);
		}
		#endregion

		#region GetTrackingInfo
		public static Tracking GetTrackingInfo(int task_id)
		{
			TaskSecurity ts = GetSecurity(task_id);
			bool IsResource = ts.IsRealTaskResource;
			bool IsManager = ts.IsManager || Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager);

			int Task_OverallPercent;
			bool Task_IsCompleted;
			int Task_ReasonId;
			int Task_CompletionTypeId;
			bool Task_IsSummary;
			bool Task_IsMilestone;
			int Task_ActivationTypeId;
			int Task_StateId;
			int projectId;
			DateTime startDate;

			using (IDataReader reader = DBTask.GetTask(task_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (!reader.Read())
					throw new NotExistingIdException();
				Task_OverallPercent = (int)reader["PercentCompleted"];
				Task_IsCompleted = (bool)reader["IsCompleted"];
				Task_ReasonId = (int)reader["ReasonId"];
				Task_CompletionTypeId = (int)reader["CompletionTypeId"];
				Task_IsSummary = (bool)reader["IsSummary"];
				Task_IsMilestone = (bool)reader["IsMilestone"];
				Task_ActivationTypeId = (int)reader["ActivationTypeId"];
				Task_StateId = (int)reader["StateId"];
				projectId = (int)reader["ProjectId"];
				startDate = (DateTime)reader["StartDate"];
			}

			startDate = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, startDate);

			bool isMSProject = Project.GetIsMSProject(projectId);

			bool Resource_MustBeConfirmed = false;
			bool Resource_IsResponsePending = false;
			bool Resource_IsConfirmed = true;

			if (IsResource)
			{
				using (IDataReader reader = DBTask.GetResourceByUser(task_id, Security.CurrentUser.UserID))
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
				&& !Task_IsCompleted
				&& !Task_IsSummary
				&& !Task_IsMilestone;

			// Personal Status with TimeTracking
			bool ShowPersonalStatus = Configuration.TimeTrackingModule && IsResource
				&& (!Resource_MustBeConfirmed || (Resource_MustBeConfirmed && !Resource_IsResponsePending && Resource_IsConfirmed))
				&& Task_CompletionTypeId == (int)CompletionType.All
				&& !Task_IsCompleted
				&& !Task_IsSummary
				&& !Task_IsMilestone;

			// Personal Status only
			bool ShowPersonalStatusOnly = !Configuration.TimeTrackingModule && IsResource
				&& (!Resource_MustBeConfirmed || (Resource_MustBeConfirmed && !Resource_IsResponsePending && Resource_IsConfirmed))
				&& Task_CompletionTypeId == (int)CompletionType.All
				&& !Task_IsCompleted
				&& !Task_IsSummary
				&& !Task_IsMilestone;

			// Overall Status with TimeTracking
			bool ShowOverallStatus = Configuration.TimeTrackingModule && (IsResource || IsManager)
				&& (!Resource_MustBeConfirmed || (Resource_MustBeConfirmed && !Resource_IsResponsePending && Resource_IsConfirmed))
				&& Task_CompletionTypeId == (int)CompletionType.Any
				&& !Task_IsCompleted
				&& !Task_IsSummary
				&& !Task_IsMilestone;

			// Overall Status only
			bool ShowOverallStatusOnly = !Configuration.TimeTrackingModule && (IsResource || IsManager)
				&& (!Resource_MustBeConfirmed || (Resource_MustBeConfirmed && !Resource_IsResponsePending && Resource_IsConfirmed))
				&& Task_CompletionTypeId == (int)CompletionType.Any
				&& !Task_IsCompleted
				&& !Task_IsSummary
				&& !Task_IsMilestone;

			// Complete
			bool ShowComplete = IsManager
				&& (Task_StateId == (int)ObjectStates.Upcoming
						|| Task_StateId == (int)ObjectStates.Active
						|| Task_StateId == (int)ObjectStates.Overdue
						|| (Task_StateId == (int)ObjectStates.Suspended && Task_IsMilestone)
					);

			// Suspend
			bool ShowSuspend = IsManager && !Task_IsMilestone
				&& (Task_StateId == (int)ObjectStates.Active
						|| Task_StateId == (int)ObjectStates.Overdue);

			// Uncomplete
			bool ShowUncomplete = IsManager && Task_StateId == (int)ObjectStates.Completed;
			//				&& Task_IsCompleted && !Task_IsSummary
			//				&& (Task_ReasonId == (int)CompletionReason.CompletedByManager || Task_ReasonId == (int)CompletionReason.CompletedByResource);

			// Resume
			bool ShowResume = IsManager && !Task_IsMilestone
				&& Task_StateId == (int)ObjectStates.Suspended;
			//				&& Task_IsCompleted && !Task_IsSummary
			//				&& (Task_ReasonId == (int)CompletionReason.Suspended || Task_ReasonId == (int)CompletionReason.SuspendedAutomatically);

			// Activate
			bool ShowActivate = IsManager && !Task_IsSummary && !Task_IsMilestone
				&& Task_StateId == (int)ObjectStates.Upcoming
				&& (!isMSProject || startDate <= DateTime.UtcNow);

			// Time Tracking (without percent)
			bool ShowTimeTracking = Configuration.TimeTrackingModule && IsResource
				&& (!Resource_MustBeConfirmed || (Resource_MustBeConfirmed && !Resource_IsResponsePending && Resource_IsConfirmed))
				&& Task_IsCompleted
				&& !Task_IsSummary
				&& !Task_IsMilestone;

			return new Tracking(ShowAcceptDeny,
				ShowPersonalStatus, ShowPersonalStatusOnly,
				ShowOverallStatus, ShowOverallStatusOnly,
				ShowActivate, ShowComplete, ShowSuspend, ShowUncomplete, ShowResume, ShowTimeTracking);
		}
		#endregion

		#region CanCreate
		public static bool CanCreate(int project_id)
		{
			Project.ProjectSecurity ps = Project.GetSecurity(project_id);
			Dictionary<string, string> prop = Project.GetProperties(project_id);
			return 
				(
					(
						Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) 
						|| ps.IsManager
						|| (ps.IsTeamMember && prop.ContainsKey(Project.CanProjectTeamCreateTask))
						|| (ps.IsSponsor && prop.ContainsKey(Project.CanProjectSponsorCreateTask))
						|| (ps.IsStakeHolder && prop.ContainsKey(Project.CanProjectStakeholderCreateTask))
						|| (ps.IsExecutiveManager && prop.ContainsKey(Project.CanProjectExecutiveCreateTask))
					)
					&& !Project.GetIsMSProject(project_id)
				);
		}
		#endregion

		#region CanAddToDo
		public static bool CanAddToDo(int task_id)
		{
			// Добавлять ToDo можно для NotCompleted Tasks
			// Добавлять могут: PPM, Project Manager, Task Resources с флагом CanManage
			bool retval = false;

			int StatusId = DBTask.GetIsCompleted(task_id);
			if (StatusId == 0)	// Not Completed
			{
				retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager);
				if (!retval)
				{
					TaskSecurity ts = GetSecurity(task_id);
					retval = ts.IsManager;
				}
			}
			return retval;
		}
		#endregion

		#region CanDeleteToDo
		public static bool CanDeleteToDo(int task_id)
		{
			// Удалять могут: PPM, Project Manager,  Task Resources с флагом CanManage
			bool retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager);
			if (!retval)
			{
				TaskSecurity ts = GetSecurity(task_id);
				retval = ts.IsManager;
			}
			return retval;
		}
		#endregion

		#region CanUpdate
		public static bool CanUpdate(int task_id)
		{
			TaskSecurity ts = GetSecurity(task_id);
			return (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) || ts.IsManager || ts.IsCreator);
		}
		#endregion

		#region CanUpdateConfigurationInfo
		public static bool CanUpdateConfigurationInfo(int task_id)
		{
			int projectId = DBTask.GetProject(task_id);
			TaskSecurity ts = GetSecurity(task_id);

			return Project.CanUpdate(projectId) && !Project.GetIsMSProject(projectId) &&
				(Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) || ts.IsManager || ts.IsCreator);
		}
		#endregion

		#region CanViewFinances
		public static bool CanViewFinances(int task_id)
		{
			return (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)
				|| Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager)
				|| GetSecurity(task_id).IsManager);
		}
		#endregion

		#region CanDelete
		public static bool CanDelete(int task_id)
		{
			return CanUpdateConfigurationInfo(task_id);
		}
		#endregion

		#region CanModifyResources
		public static bool CanModifyResources(int task_id)
		{
			bool isMilestone = false;
			bool isSummary = false;
			int projectId = -1;

			using (IDataReader reader = Task.GetTask(task_id))
			{
				if (reader.Read())
				{
					isMilestone = (bool)reader["IsMilestone"];
					isSummary = (bool)reader["IsSummary"];
					projectId = (int)reader["ProjectId"];
				}
			}

			bool isSyncMode = Project.GetIsMSProject(projectId);

			TaskSecurity ts = GetSecurity(task_id);

			return !isMilestone && !isSummary && !isSyncMode &&
				(Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) || ts.IsManager || ts.IsCreator);
		}
		#endregion

		#region CanRead(int task_id)
		public static bool CanRead(int task_id)
		{
			int UserId = Security.CurrentUser.UserID;

			bool retval = false;
			if (!Security.CurrentUser.IsExternal)
			{
				// Check by AlertService
				retval = Security.CurrentUser.IsAlertService;

				if (!retval)
					retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)
						|| Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager);

				// Check by Task
				if (!retval)
				{
					TaskSecurity ts = GetSecurity(task_id);
					retval = ts.IsManager || ts.IsResource || ts.IsCreator;
				}

				// Check by Project
				if (!retval)
				{
					int ProjectId = DBTask.GetProject(task_id);
					Project.ProjectSecurity ps = Project.GetSecurity(ProjectId);
					retval = ps.IsManager || ps.IsTeamMember || ps.IsExecutiveManager || ps.IsSponsor || ps.IsStakeHolder;
				}

				// Check by Sharing
				if (!retval)
				{
					if (DBTask.GetSharingLevel(UserId, task_id) >= 0)
						retval = true;
				}
			}
			else		// External User
			{
				if (DBTask.GetIsCompleted(task_id) == 0)
				{
					TaskSecurity ts = GetSecurity(task_id);
					retval = ts.IsResource;
				}
			}
			return retval;
		}
		#endregion

		#region CanRead(int task_id, int user_id)
		public static bool CanRead(int task_id, int user_id)
		{
			bool retval = false;
			if (!User.IsExternal(user_id))
			{
				ArrayList secure_groups = User.GetListSecureGroupAll(user_id);
				retval = secure_groups.Contains((int)InternalSecureGroups.PowerProjectManager)
					|| secure_groups.Contains((int)InternalSecureGroups.ExecutiveManager);

				// Check by Task
				if (!retval)
				{
					TaskSecurity ts = GetSecurity(task_id, user_id);
					retval = ts.IsManager || ts.IsResource || ts.IsCreator;
				}

				// Check by Project
				if (!retval)
				{
					int ProjectId = DBTask.GetProject(task_id);
					Project.ProjectSecurity ps = Project.GetSecurity(ProjectId, user_id);
					retval = ps.IsManager || ps.IsTeamMember || ps.IsExecutiveManager || ps.IsSponsor || ps.IsStakeHolder;
				}

				// Check by Sharing
				if (!retval)
				{
					if (DBTask.GetSharingLevel(user_id, task_id) >= 0)
						retval = true;
				}
			}
			else		// External User
			{
				if (DBTask.GetIsCompleted(task_id) == 0)
				{
					TaskSecurity ts = GetSecurity(task_id, user_id);
					retval = ts.IsResource;
				}
			}
			return retval;
		}
		#endregion

		#region GetTask
		/// <summary>
		/// Reader returns fields:
		///  TaskId, TaskNum, ProjectId, ProjectTitle, ManagerId, CreatorId, CompletedBy, Title, 
		///  Description,	CreationDate, StartDate, FinishDate, 	Duration, 
		///  ActualStartDate, ActualFinishDate, PriorityId, PriorityName, PercentCompleted, OutlineNumber, 
		///  OutlineLevel, 	IsSummary, IsMilestone, ConstraintTypeId, ConstraintTypeName, 
		///  ConstraintDate, CompletionTypeId, CompletionTypeName, IsCompleted, 
		///  MustBeConfirmed, ReasonId, ActivationTypeId, StateId, ActivatedManually,
		///  ActivationTypeName, StateName, PhaseId, PhaseName, 
		///  TaskTime, MSProjectUID, TotalMinutes, TotalApproved, ProjectCode
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetTask(int task_id)
		{
			return GetTask(task_id, true, false);
		}

		public static IDataReader GetTask(int task_id, bool checkAccess)
		{
			return GetTask(task_id, checkAccess, false);
		}

		private static IDataReader GetTask(int task_id, bool checkAccess, bool utc_dates)
		{
			if (checkAccess && !CanRead(task_id))
				throw new AccessDeniedException();

			if (utc_dates)
				return DBTask.GetTaskInUTC(task_id, Security.CurrentUser.LanguageId);
			else
				return DBTask.GetTask(task_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
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
			return DBTask.GetListCompletionTypes(Security.CurrentUser.LanguageId);
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
		public static IDataReader GetListCategories(int task_id)
		{
			return DBCommon.GetListCategoriesByObject((int)TASK_TYPE, task_id);
		}
		#endregion

		#region GetListDiscussions
		/// <summary>
		/// Reader returns fields:
		///		DiscussionId, ObjectTypeId, ObjectId, CreatorId, CreationDate, Text, CreatorName
		/// </summary>
		public static IDataReader GetListDiscussions(int task_id)
		{
			return Common.GetListDiscussions(TASK_TYPE, task_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListDiscussionsDataTable
		/// <summary>
		/// Reader returns fields:
		///		DiscussionId, ObjectTypeId, ObjectId, CreatorId, CreationDate, Text, CreatorName
		/// </summary>
		public static DataTable GetListDiscussionsDataTable(int task_id)
		{
			return Common.GetListDiscussionsDataTable(TASK_TYPE, task_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region AddDiscussion
		public static void AddDiscussion(int task_id, string text)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			DateTime creation_date = DateTime.UtcNow;
			int UserId = Security.CurrentUser.UserID;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				int CommentId = DBCommon.AddDiscussion((int)TASK_TYPE, task_id, UserId, creation_date, text);

				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_CommentList_CommentAdded, task_id, CommentId);

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

		#region GetListSuccessorsDetails
		/// <summary>
		/// Reader returns fields:
		///  LinkId, TaskId, TaskNum, Title, StartDate, FinishDate, Duration, PriorityId, 
		///	 PercentCompleted, OutlineNumber, OutlineLevel, IsCompleted, Lag, CompletionTypeId, 
		///	 ReasonId, StateId
		/// </summary>
		/// <param name="task_id"></param>
		/// <returns></returns>
		public static IDataReader GetListSuccessorsDetails(int task_id)
		{
			return DBTask.GetListSuccessorsDetails(task_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListPredecessorsDetails
		/// <summary>
		/// Reader returns fields:
		///  LinkId, TaskId, TaskNum, Title, StartDate, FinishDate, Duration, PriorityId, 
		///	 PercentCompleted, OutlineNumber, OutlineLevel, IsCompleted, Lag,
		///	 IsCompleted, CompletionTypeId, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListPredecessorsDetails(int task_id)
		{
			return DBTask.GetListPredecessorsDetails(task_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetProjectManager
		public static int GetProjectManager(int project_id)
		{
			return DBProject.GetProjectManager(project_id);
		}
		#endregion

		#region GetListTasksByProject
		/// <summary>
		/// TaskId, TaskNum, ProjectId, CreatorId, LastEditorId, Title, Description, 	
		/// CreationDate, LastSavedDate, 	StartDate, FinishDate, 	Duration, 
		/// ActualFinishDate, PriorityId, PercentCompleted, OutlineNumber, OutlineLevel, 	
		/// IsSummary, IsMilestone, ConstraintTypeId, ConstraintDate, CompletionTypeId, 
		/// IsCompleted, MustBeConfirmed, ReasonId, PriorityName, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListTasksByProject(int project_id)
		{
			return DBTask.GetListTasksByProject(project_id, Security.CurrentUser.LanguageId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListTasksByProjectDataTable
		/// <summary>
		/// TaskId, TaskNum, ProjectId, CreatorId, LastEditorId, Title, Description, 	
		/// CreationDate, LastSavedDate, 	StartDate, FinishDate, 	Duration, 
		/// ActualFinishDate, PriorityId, PercentCompleted, OutlineNumber, OutlineLevel, 	
		/// IsSummary, IsMilestone, ConstraintTypeId, ConstraintDate, CompletionTypeId, 
		/// IsCompleted, MustBeConfirmed, ReasonId, PriorityName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListTasksByProjectDataTable(int project_id)
		{
			return DBTask.GetListTasksByProjectDataTable(project_id, Security.CurrentUser.LanguageId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListTasksByProjectCollapsedDataTable
		/// <summary>
		/// Datatable contains fields:
		///  TaskId, TaskNum, OutlineNumber, OutlineLevel, IsSummary, IsMilestone, Title, 
		///  PercentCompleted, CompletionTypeId, IsCompleted, IsCollapsed, StartDate, FinishDate, Duration, StateId
		/// </summary>
		/// <param name="project_id"></param>
		/// <returns></returns>
		public static DataTable GetListTasksByProjectCollapsedDataTable(int project_id)
		{
			DataTable dt = DBTask.GetListTasksByProjectCollapsedDataTable(project_id, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId);

			int OldOutlineLevel = 1;
			bool OldIsCollapsed = false;
			foreach (DataRow dr in dt.Rows)
			{
				string TaskNum = dr["TaskNum"].ToString();
				int OutlineLevel = (int)dr["OutlineLevel"];
				bool IsCollapsed = ((int)dr["IsCollapsed"] > 0) ? true : false;

				if (OldIsCollapsed && OldOutlineLevel < OutlineLevel)
				{
					dr.Delete();
				}
				else
				{
					OldOutlineLevel = OutlineLevel;
					OldIsCollapsed = IsCollapsed;
				}
			}

			return dt;
		}
		#endregion

		#region Collapse
		public static void Collapse(int task_id)
		{
			DBTask.AddCollapsedTask(Security.CurrentUser.UserID, task_id);
		}
		#endregion

		#region Expand
		public static void Expand(int task_id)
		{
			DBTask.DeleteCollapsedTask(Security.CurrentUser.UserID, task_id);
		}
		#endregion

		#region GetListTasksForMove
		/// <summary>
		/// Reader returns fields:
		///	 TaskNum, Title
		/// </summary>
		/// <param name="task_id"></param>
		/// <returns></returns>
		public static IDataReader GetListTasksForMove(int task_id)
		{
			return DBTask.GetListTasksForMove(task_id);
		}
		#endregion

		#region GetProject
		public static int GetProject(int TaskId)
		{
			return DBTask.GetProject(TaskId);
		}
		#endregion

		#region GetListVacantPredecessors
		/// <summary>
		/// Reader returns fields:
		///	 TaskId, TaskNum, Title
		/// </summary>
		/// <param name="task_id"></param>
		/// <returns></returns>
		public static IDataReader GetListVacantPredecessors(int task_id)
		{
			return DBTask.GetListVacantPredecessors(task_id);
		}
		#endregion

		#region GetListVacantSuccessors
		/// <summary>
		/// Reader returns fields:
		///	 TaskId, TaskNum, Title
		/// </summary>
		/// <param name="task_id"></param>
		/// <returns></returns>
		public static IDataReader GetListVacantSuccessors(int task_id)
		{
			return DBTask.GetListVacantSuccessors(task_id);
		}
		#endregion

		#region GetListResources
		/// <summary>
		/// Reader returns fields:
		///	 ResourceId, TaskId, UserId, MustBeConfirmed, ResponsePending, IsConfirmed, 
		///	 PercentCompleted, ActualFinishDate, LastSavedDate, IsGroup, IsExternal, ResourceName
		/// </summary>
		/// <param name="task_id"></param>
		/// <returns></returns>
		public static IDataReader GetListResources(int task_id)
		{
			return DBTask.GetListResources(task_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListResourcesDataTable
		/// <summary>
		/// Reader returns fields:
		///	 ResourceId, TaskId, UserId, MustBeConfirmed, ResponsePending, IsConfirmed, 
		///	 PercentCompleted, ActualFinishDate, LastSavedDate, IsGroup, IsExternal, ResourceName
		/// </summary>
		/// <param name="task_id"></param>
		/// <returns></returns>
		public static DataTable GetListResourcesDataTable(int task_id)
		{
			return DBTask.GetListResourcesDataTable(task_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetResourceInfo
		/// <summary>
		/// Reader returns fields:
		///	 MustBeConfirmed, ResponsePending, IsConfirmed, PercentCompleted
		/// </summary>
		/// <param name="task_id"></param>
		/// <returns></returns>
		public static IDataReader GetResourceInfo(int task_id)
		{
			return DBTask.GetResourceByUser(task_id, Security.CurrentUser.UserID);
		}
		#endregion

		#region AcceptResource
		public static void AcceptResource(int task_id)
		{
			int UserId = Security.CurrentUser.UserID;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBTask.ReplyResource(task_id, UserId, true, DateTime.UtcNow);

				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_ResourceList_RequestAccepted, task_id, UserId);

				tran.Commit();
			}
		}
		#endregion

		#region DeclineResource
		public static void DeclineResource(int task_id)
		{
			UserLight cu = Security.CurrentUser;
			int UserId = cu.UserID;
			DateTime utc_now = DateTime.UtcNow;

			int CompletionTypeId;
			bool IsManagerConfirmed;
			bool IsCompleted;
			int ReasonId;
			int project_id;
			using (IDataReader reader = DBTask.GetTask(task_id, cu.TimeZoneId, cu.LanguageId))
			{
				reader.Read();
				CompletionTypeId = (int)reader["CompletionTypeId"];
				IsManagerConfirmed = (bool)reader["MustBeConfirmed"];
				IsCompleted = (bool)reader["IsCompleted"];
				ReasonId = (int)reader["ReasonId"];
				project_id = (int)reader["ProjectId"];
			}

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBTask.ReplyResource(task_id, UserId, false, utc_now);

				// O.R. [2009-02-12]
				DBCalendar.DeleteStickedObject((int)TASK_TYPE, task_id, UserId);

				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_ResourceList_RequestDenied, task_id, UserId);

				if (CompletionTypeId == (int)CompletionType.All)
				{
					int OverallPercent = RecalculateOverallPercent(task_id);
					DBTask.UpdatePercent(task_id, OverallPercent);

					if (!IsCompleted && !IsManagerConfirmed && OverallPercent == 100)
					{
						DBTask.UpdateCompletion(task_id, true, (int)CompletionReason.CompletedAutomatically);
						CompleteToDo(task_id);
						RecalculateAllStates(project_id);
					}

					DBTask.RecalculateSummaryPercent(task_id);
				}

				tran.Commit();
			}
		}
		#endregion

		#region UpdateResourcePercent (TODO: сделать уведомление менеджера при 100% завершении)
		public static void UpdateResourcePercent(int task_id, int percent_completed, int minutes, DateTime Dt)
		{
			UpdateResourcePercent(task_id, Security.CurrentUser.UserID, percent_completed, minutes, Dt, true);
		}

		public static void UpdateResourcePercent(int task_id, int percent_completed)
		{
			UpdateResourcePercent(task_id, Security.CurrentUser.UserID, percent_completed, 0, DateTime.Now, true);
		}

		public static void UpdateResourcePercent(int task_id, int userId, int percent_completed)
		{
			UpdateResourcePercent(task_id, userId, percent_completed, 0, DateTime.Now, true);
		}

		public static void UpdateResourcePercent(int task_id, int UserId, int percent_completed, int minutes, DateTime Dt, bool summarize)
		{
			if (!CanRead(task_id))
				throw new AccessDeniedException();

			UserLight cu = Security.CurrentUser;

			bool IsManagerConfirmed;
			int ProjectId;
			int previousPercent = -1;
			bool IsCompleted = false;
			string title = "";
			Dt = Dt.Date;

			using (IDataReader reader = DBTask.GetTask(task_id, cu.TimeZoneId, cu.LanguageId))
			{
				reader.Read();
				IsManagerConfirmed = (bool)reader["MustBeConfirmed"];
				ProjectId = (int)reader["ProjectId"];
				IsCompleted = (bool)reader["IsCompleted"];
				title = (string)reader["Title"];
			}

			if (IsCompleted && !CanUpdate(task_id))
				throw new AccessDeniedException();

			// Get current percent completed
			using (IDataReader reader = DBTask.GetResourceByUser(task_id, UserId))
			{
				if (reader.Read())
					previousPercent = (int)reader["PercentCompleted"];
			}

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBTask.UpdateResourcePercent(task_id, UserId, percent_completed, DateTime.UtcNow);

				// O.R. [2009-05-15]
				if (percent_completed == 100)
					DBCalendar.DeleteStickedObject((int)TASK_TYPE, task_id, UserId);

				if (!IsCompleted)
				{
					int OverallPercent = RecalculateOverallPercent(task_id);
					DBTask.UpdatePercent(task_id, OverallPercent);

					SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_Percent, task_id, OverallPercent);

					if (OverallPercent == 100)
					{
						// O.R. [2009-02-12]
						DBCalendar.DeleteStickedObjectForAllUsers((int)TASK_TYPE, task_id);

						if (!IsManagerConfirmed)
						{
							// O.R. [2008-07-24]. If CompletedAutomatically then the task will be reopened during suspend/resume parent
							//DBTask.UpdateCompletion(task_id, true, (int)CompletionReason.CompletedAutomatically);
							DBTask.UpdateCompletion(task_id, true, (int)CompletionReason.CompletedManually);
							CompleteToDo(task_id);
							RecalculateAllStates(ProjectId);
						}
					}

					DBTask.RecalculateSummaryPercent(task_id);
				}

				if (!summarize || minutes > 0)
					Mediachase.IbnNext.TimeTracking.TimeTrackingManager.AddHoursForEntryByObject(task_id, (int)TASK_TYPE, title, ProjectId, UserId, TimeTracking.GetWeekStart(Dt), TimeTracking.GetDayNum(Dt), minutes, summarize);

				tran.Commit();
			}
		}
		#endregion

		#region UpdatePercent
		public static void UpdatePercent(int task_id, int percent_completed, int minutes, DateTime Dt)
		{
			UpdatePercent(task_id, percent_completed, minutes, Dt, true);
		}

		public static void UpdatePercent(int task_id, int percent_completed)
		{
			UpdatePercent(task_id, percent_completed, 0, DateTime.Now, true);
		}

		public static void UpdatePercent(int task_id, int percent_completed, int minutes, DateTime Dt, bool summarize)
		{
			if (!CanRead(task_id))
				throw new AccessDeniedException();

			Dt = Dt.Date;
			bool IsManagerConfirmed;
			int ProjectId;
			bool IsCompleted = false;
			string title = "";
			using (IDataReader reader = DBTask.GetTask(task_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				reader.Read();
				IsManagerConfirmed = (bool)reader["MustBeConfirmed"];
				ProjectId = (int)reader["ProjectId"];
				IsCompleted = (bool)reader["IsCompleted"];
				title = (string)reader["Title"];
			}

			if (IsCompleted)
				throw new AccessDeniedException();

			int UserId = Security.CurrentUser.UserID;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBTask.UpdatePercent(task_id, percent_completed);

				SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_Percent, task_id, percent_completed);

				if (percent_completed == 100)
				{
					// O.R. [2009-02-12]
					DBCalendar.DeleteStickedObjectForAllUsers((int)TASK_TYPE, task_id);

					if (!IsManagerConfirmed)
					{
						// O.R. [2008-07-24]. If CompletedAutomatically then the task will be reopened during suspend/resume parent
						// DBTask.UpdateCompletion(task_id, true, (int)CompletionReason.CompletedAutomatically);
						DBTask.UpdateCompletion(task_id, true, (int)CompletionReason.CompletedManually);
						CompleteToDo(task_id);
						RecalculateAllStates(ProjectId);
					}
				}

				DBTask.RecalculateSummaryPercent(task_id);

				if (!summarize || minutes > 0)
					Mediachase.IbnNext.TimeTracking.TimeTrackingManager.AddHoursForEntryByObject(task_id, (int)TASK_TYPE, title, ProjectId, UserId, TimeTracking.GetWeekStart(Dt), TimeTracking.GetDayNum(Dt), minutes, summarize);

				tran.Commit();
			}
		}
		#endregion

		#region RecalculateOverallPercent
		internal static int RecalculateOverallPercent(int task_id)	// Completion Type = All
		{
			int ResourceCount = 0;
			int TotalPercent = 0;

			using (IDataReader reader = DBTask.GetListResources(task_id, Security.CurrentUser.TimeZoneId))
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

		#region ActivateTask
		public static void ActivateTask(int task_id)
		{
			if (!CanUpdate(task_id))
				throw new AccessDeniedException();

			//--- We can't activate MSProject tasks
			int projectId;
			DateTime startDate;
			using (IDataReader reader = DBTask.GetTaskInUTC(task_id, Security.CurrentUser.LanguageId))
			{
				if (!reader.Read())
					throw new NotExistingIdException();
				projectId = (int)reader["ProjectId"];
				startDate = (DateTime)reader["StartDate"];
			}

			bool isMSProject = Project.GetIsMSProject(projectId);

			if (isMSProject && startDate > DateTime.UtcNow)
				throw new AccessDeniedException();
			//---

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBTask.UpdateActivation(task_id, true);

				RecalculateAllStates(projectId);
				tran.Commit();
			}
		}
		#endregion

		#region DeactivateTask
		public static void DeactivateTask(int task_id)
		{
			if (!CanUpdate(task_id))
				throw new AccessDeniedException();

			//--- We can't deactivate MSProject tasks
			int projectId;
			using (IDataReader reader = DBTask.GetTaskInUTC(task_id, Security.CurrentUser.LanguageId))
			{
				if (!reader.Read())
					throw new NotExistingIdException();
				projectId = (int)reader["ProjectId"];
			}

			bool isMSProject = Project.GetIsMSProject(projectId);

			if (isMSProject)
				throw new AccessDeniedException();
			//---

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBTask.UpdateActivation(task_id, false);

				RecalculateAllStates(projectId);
				tran.Commit();
			}
		}
		#endregion

		#region CompleteTask
		public static void CompleteTask(int taskId)
		{
			CompleteTask(taskId, true);
		}

		public static void CompleteTask(int taskId, bool manually)
		{
			if (manually && !CanUpdate(taskId))
				throw new AccessDeniedException();

			int completionTypeId;

			int phaseId = -1;
			int projectId = -1;
			using (IDataReader reader = DBTask.GetTask(taskId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				reader.Read();
				completionTypeId = (int)reader["CompletionTypeId"];
				projectId = (int)reader["ProjectId"];
				if ((bool)reader["IsMileStone"] && reader["PhaseId"] != DBNull.Value)
					phaseId = (int)reader["PhaseId"];
			}

			int userId = Security.CurrentUser.UserID;
			DateTime dt = DateTime.UtcNow;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				// O.R. [2009-02-12]
				DBCalendar.DeleteStickedObjectForAllUsers((int)TASK_TYPE, taskId);

				if (completionTypeId == (int)CompletionType.All)
					DBTask.UpdateResourcePercent(taskId, userId, 100, dt);

				DBTask.UpdatePercent(taskId, 100);
				if (manually)
					DBTask.UpdateCompletion(taskId, true, (int)CompletionReason.CompletedManually);
				else
					DBTask.UpdateCompletion(taskId, true, (int)CompletionReason.CompletedAutomatically);

				// O.R [2/2/2006]
				//SendAlert(AlertEventType.Task_Completed, task_id);
				CompleteToDo(taskId);
				CompleteChildren(taskId);

				if (phaseId > 0)
					Project2.UpdatePhase(projectId, phaseId, false);

				RecalculateAllStates(projectId);

				DBTask.RecalculateSummaryPercent(taskId);

				tran.Commit();
			}
		}
		#endregion

		#region UncompleteTask
		public static void UncompleteTask(int task_id)
		{
			if (!CanUpdate(task_id))
				throw new AccessDeniedException();

			int CompletionTypeId;
			int ProjectId;
			using (IDataReader reader = DBTask.GetTask(task_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				reader.Read();
				CompletionTypeId = (int)reader["CompletionTypeId"];
				ProjectId = (int)reader["ProjectId"];
			}

			int UserId = Security.CurrentUser.UserID;
			DateTime dt = DateTime.UtcNow;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBTask.UpdateCompletion(task_id, false, (int)CompletionReason.NotCompleted);

				int OverallPercent = 50;
				if (CompletionTypeId == (int)CompletionType.All)
				{
					DBTask.ResetResourcePercent(task_id, 50, dt);

					OverallPercent = RecalculateOverallPercent(task_id);
				}
				// O.R. [2008-07-24]
				DBTask.UpdatePercent(task_id, OverallPercent);

				// O.R [2/2/2006]
				//SendAlert(AlertEventType.Task_Updated, task_id);
				UncompleteToDo(task_id);
				UncompleteChildren(task_id);

				RecalculateAllStates(ProjectId);

				DBTask.RecalculateSummaryPercent(task_id);

				tran.Commit();
			}
		}
		#endregion

		#region SuspendTask
		public static void SuspendTask(int task_id)
		{
			if (!CanUpdate(task_id))
				throw new AccessDeniedException();

			int ProjectId = GetProject(task_id);
			int UserId = Security.CurrentUser.UserID;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				// O.R. [2009-02-12]
				DBCalendar.DeleteStickedObjectForAllUsers((int)TASK_TYPE, task_id);

				DBTask.UpdateCompletion(task_id, true, (int)CompletionReason.SuspendedManually);
				// O.R [2/2/2006]
				//SendAlert(AlertEventType.Task_Completed, task_id);
				SuspendToDo(task_id);
				SuspendChildren(task_id);

				RecalculateAllStates(ProjectId);

				tran.Commit();
			}
		}
		#endregion

		#region ResumeTask
		public static void ResumeTask(int task_id)
		{
			if (!CanUpdate(task_id))
				throw new AccessDeniedException();

			int ProjectId = GetProject(task_id);
			int UserId = Security.CurrentUser.UserID;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBTask.UpdateCompletion(task_id, false, (int)CompletionReason.NotCompleted);

				// O.R [2/2/2006]
				//SendAlert(AlertEventType.Task_Updated, task_id);
				ResumeToDo(task_id);
				ResumeChildren(task_id);

				RecalculateAllStates(ProjectId);

				tran.Commit();
			}
		}
		#endregion

		#region GetListTasksForUserByProject
		/// <summary>
		/// DataTable contains fields:
		///  TaskId, Title, Description, PriorityId, PriorityName, StartDate, FinishDate, 
		///  PercentCompleted, StateId
		/// </summary>
		/// <param name="ProjectId"></param>
		/// <returns></returns>
		public static DataTable GetListTasksForUserByProject(int project_id)
		{
			int user_id = Security.CurrentUser.UserID;
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			return DBTask.GetListTasksForUserByProject(project_id, user_id, TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListTasksForChangeableRoles
		/// <summary>
		/// Reader returns fields:
		///  TaskId, Title, IsCompleted, CompletionTypeId, StartDate, FinishDate,
		///  IsResource, CanView, CanEdit, CanDelete, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListTasksForChangeableRoles(int UserId)
		{
			return DBTask.GetListTasksForChangeableRoles(UserId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId);
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

		#region GetListMilestones
		/// <summary>
		///  Reader returns fields:
		///		TaskId, Title, Description, IsCompleted, StartDate, FinishDate, StateId,
		///		CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListMilestones(int ProjectId,
			DateTime StartDate, DateTime FinishDate)
		{
			return GetListMilestones(ProjectId, StartDate, FinishDate, "");
		}

		public static IDataReader GetListMilestones(int ProjectId,
			DateTime StartDate, DateTime FinishDate, string Keyword)
		{
			return DBTask.GetListMilestones(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, StartDate, FinishDate, Keyword);
		}
		#endregion

		#region GetListMilestonesDataTable
		/// <summary>
		///  Reader returns fields:
		///		TaskId, Title, Description, IsCompleted, StartDate, FinishDate,
		///		CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListMilestonesDataTable(int ProjectId,
			DateTime StartDate, DateTime FinishDate)
		{
			return GetListMilestonesDataTable(ProjectId, StartDate, FinishDate, "");
		}

		public static DataTable GetListMilestonesDataTable(int ProjectId,
			DateTime StartDate, DateTime FinishDate, string Keyword)
		{
			return DBTask.GetListMilestonesDataTable(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, StartDate, FinishDate, Keyword);
		}
		#endregion

		#region UpdateDiscussion
		public static void UpdateDiscussion(
			int DiscussionId, string Text)
		{
			Project.UpdateDiscussion(DiscussionId, Text);
		}
		#endregion

		#endregion

		#region GetListToDo
		/// <summary>
		/// ToDoId, Title, Description, StartDate, FinishDate, ActualFinishDate, PercentCompleted, 
		///	 IsCompleted, CompletedBy, ReasonId, ReasonName, PriorityId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDo(int TaskId)
		{
			return DBTask.GetListToDo(TaskId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListToDoDataTable
		/// <summary>
		/// ToDoId, Title, Description, StartDate, FinishDate, ActualFinishDate, PercentCompleted, 
		///	 IsCompleted, CompletedBy, ReasonId, ReasonName, PriorityId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoDataTable(int TaskId)
		{
			return DBTask.GetListToDoDataTable(TaskId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetIsCompleted
		public static bool GetIsCompleted(int TaskId)
		{
			bool retval = false;
			if (DBTask.GetIsCompleted(TaskId) == 1)
				retval = true;
			return retval;
		}
		#endregion

		#region SuspendToDo
		public static void SuspendToDo(int TaskId)
		{
			ArrayList TodoList = new ArrayList();
			using (IDataReader reader = DBTask.GetListToDo(TaskId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				while (reader.Read())
					if (!(bool)reader["IsCompleted"])
						TodoList.Add((int)reader["ToDoId"]);
			}

			foreach (int ToDoId in TodoList)
				ToDo.SuspendToDo(ToDoId, false);
		}
		#endregion

		#region CompleteToDo
		public static void CompleteToDo(int TaskId)
		{
			ArrayList TodoList = new ArrayList();
			using (IDataReader reader = DBTask.GetListToDo(TaskId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				while (reader.Read())
					if (!(bool)reader["IsCompleted"])
						TodoList.Add((int)reader["ToDoId"]);
			}

			foreach (int ToDoId in TodoList)
				ToDo.CompleteToDo(ToDoId, false);
		}
		#endregion

		#region UncompleteToDo
		public static void UncompleteToDo(int TaskId)
		{
			ArrayList TodoList = new ArrayList();
			using (IDataReader reader = DBTask.GetListToDo(TaskId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				while (reader.Read())
					if ((bool)reader["IsCompleted"] && (int)reader["ReasonId"] == (int)CompletionReason.CompletedAutomatically)
						TodoList.Add((int)reader["ToDoId"]);
			}

			foreach (int ToDoId in TodoList)
				ToDo.UncompleteToDo(ToDoId, false);
		}
		#endregion

		#region ResumeToDo
		public static void ResumeToDo(int TaskId)
		{
			ArrayList TodoList = new ArrayList();
			using (IDataReader reader = DBTask.GetListToDo(TaskId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				while (reader.Read())
					if ((bool)reader["IsCompleted"] && (int)reader["ReasonId"] == (int)CompletionReason.SuspendedAutomatically)
						TodoList.Add((int)reader["ToDoId"]);
			}

			foreach (int ToDoId in TodoList)
				ToDo.ResumeToDo(ToDoId, false);
		}
		#endregion


		#region GetTaskTitle
		public static string GetTaskTitle(int TaskId)
		{
			string TaskTitle = "";
			using (IDataReader reader = GetTask(TaskId, false))
			{
				if (reader.Read())
					TaskTitle = reader["Title"].ToString();
			}
			return TaskTitle;
		}
		#endregion

		#region AddFavorites
		public static void AddFavorites(int TaskId)
		{
			DBCommon.AddFavorites((int)TASK_TYPE, TaskId, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListFavorites
		/// <summary>
		///		FavoriteId, ObjectTypeId, ObjectId, UserId, Title 
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListFavoritesDT()
		{
			return DBCommon.GetListFavoritesDT((int)TASK_TYPE, Security.CurrentUser.UserID);
		}
		#endregion

		#region CheckFavorites
		public static bool CheckFavorites(int TaskId)
		{
			return DBCommon.CheckFavorites((int)TASK_TYPE, TaskId, Security.CurrentUser.UserID);
		}
		#endregion

		#region DeleteFavorites
		public static void DeleteFavorites(int TaskId)
		{
			DBCommon.DeleteFavorites((int)TASK_TYPE, TaskId, Security.CurrentUser.UserID);
		}
		#endregion

		#region AddHistory
		public static void AddHistory(int TaskId, string Title)
		{
			DBCommon.AddHistory((int)TASK_TYPE, TaskId, Title, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListActivationTypes
		/// <summary>
		/// Reader returns fields:
		///  ActivationTypeId, ActivationTypeName 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListActivationTypes()
		{
			return DBTask.GetListActivationTypes(Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListChildrenInfo
		/// <summary>
		///  TaskId, CreatorId, CompletedBy, Title, Description, 
		///  CreationDate, StartDate, FinishDate, Duration, 
		///  ActualFinishDate, PercentCompleted, IsSummary, IsMilestone, IsCompleted, 
		///  ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListChildrenInfo(int TaskId)
		{
			return DBTask.GetListChildrenInfo(TaskId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetCalendarXml
		public static void GetCalendarXml(XmlDocument xmlDocument, DateTime dateFrom, DateTime dateTo, bool bLoadTask, bool bLoadToDo)
		{
			XmlNode taskRoot = xmlDocument.SelectSingleNode("ibnCalendar/tasks");
			XmlNode todoRoot = xmlDocument.SelectSingleNode("ibnCalendar/todos");

			using (DataTable table = ToDo.GetListToDoAndTasksByFilterDataTable(0, 0, 1, dateFrom, 2, dateTo, string.Empty, true, false, false, 0))
			{
				XmlDocumentFragment fragmentToDoItem = xmlDocument.CreateDocumentFragment();
				fragmentToDoItem.InnerXml = "<todo><id/><title/><description/><priority/><startDate/><dueDate/><percentComplete/></todo>";

				XmlDocumentFragment fragmentTaskItem = xmlDocument.CreateDocumentFragment();
				fragmentTaskItem.InnerXml = "<task><id/><title/><description/><priority/><startDate/><dueDate/><percentComplete/></task>";

				foreach (DataRow row in table.Rows)
				{
					if (!((bool)row["IsCompleted"]))
					{
						string Id = row["ItemId"].ToString();
						string Title = row["Title"].ToString();
						string Description = row["Description"].ToString();
						string Priority = row["PriorityId"].ToString();
						string StartDate = row["StartDate"] == DBNull.Value ? string.Empty : ((DateTime)row["StartDate"]).ToString("s");
						string FinishDate = row["FinishDate"] == DBNull.Value ? string.Empty : ((DateTime)row["FinishDate"]).ToString("s");
						string PercentCompleted = row["PercentCompleted"].ToString();

						XmlNode currentItemNode = null;

						if (((int)row["IsToDo"]) == 1)
						{
							currentItemNode = fragmentToDoItem.SelectSingleNode("todo");
						}
						else
						{
							currentItemNode = fragmentTaskItem.SelectSingleNode("task");
						}

						currentItemNode.SelectSingleNode("id").InnerText = Id;
						currentItemNode.SelectSingleNode("title").InnerText = Title;
						currentItemNode.SelectSingleNode("description").InnerText = Description;
						currentItemNode.SelectSingleNode("priority").InnerText = Priority;
						currentItemNode.SelectSingleNode("percentComplete").InnerText = PercentCompleted;
						currentItemNode.SelectSingleNode("startDate").InnerText = StartDate;
						currentItemNode.SelectSingleNode("dueDate").InnerText = FinishDate;

						if (((int)row["IsToDo"]) == 1)
							if (bLoadToDo)
								todoRoot.AppendChild(currentItemNode.CloneNode(true));
							else
								if (bLoadTask)
									taskRoot.AppendChild(currentItemNode.CloneNode(true));
					}
				}
			}
		}
		#endregion

		#region SuspendChildren
		private static void SuspendChildren(int task_id)
		{
			ArrayList taskList = new ArrayList();
			using (IDataReader reader = DBTask.GetListAllChild(task_id))
			{
				while (reader.Read())
				{
					if (!(bool)reader["IsCompleted"])
						taskList.Add((int)reader["TaskId"]);
				}
			}

			foreach (int childTaskId in taskList)
			{
				// O.R. [2009-02-12]
				DBCalendar.DeleteStickedObjectForAllUsers((int)TASK_TYPE, childTaskId);

				DBTask.UpdateCompletion(childTaskId, true, (int)CompletionReason.SuspendedAutomatically);
				SuspendToDo(childTaskId);
			}
		}
		#endregion

		#region CompleteChildren
		private static void CompleteChildren(int task_id)
		{
			ArrayList taskList = new ArrayList();
			using (IDataReader reader = DBTask.GetListAllChild(task_id))
			{
				while (reader.Read())
				{
					if (!(bool)reader["IsCompleted"])
						taskList.Add((int)reader["TaskId"]);
				}
			}

			foreach (int childTaskId in taskList)
			{
				// O.R. [2009-02-12]
				DBCalendar.DeleteStickedObjectForAllUsers((int)TASK_TYPE, childTaskId);

				DBTask.UpdateCompletion(childTaskId, true, (int)CompletionReason.CompletedAutomatically);
				CompleteToDo(childTaskId);
			}
		}
		#endregion

		#region ResumeChildren
		private static void ResumeChildren(int task_id)
		{
			ArrayList taskList = new ArrayList();
			using (IDataReader reader = DBTask.GetListAllChild(task_id))
			{
				while (reader.Read())
				{
					if ((int)reader["ReasonId"] == (int)CompletionReason.SuspendedAutomatically)
						taskList.Add((int)reader["TaskId"]);
				}
			}

			foreach (int childTaskId in taskList)
			{
				DBTask.UpdateCompletion(childTaskId, false, (int)CompletionReason.NotCompleted);
				ResumeToDo(childTaskId);
			}
		}
		#endregion

		#region UncompleteChildren
		private static void UncompleteChildren(int task_id)
		{
			ArrayList taskList = new ArrayList();
			using (IDataReader reader = DBTask.GetListAllChild(task_id))
			{
				while (reader.Read())
				{
					if ((int)reader["ReasonId"] == (int)CompletionReason.CompletedAutomatically)
						taskList.Add((int)reader["TaskId"]);
				}
			}

			foreach (int childTaskId in taskList)
			{
				DBTask.UpdateCompletion(childTaskId, false, (int)CompletionReason.NotCompleted);
				UncompleteToDo(childTaskId);
			}
		}
		#endregion

		#region ConfirmReminder(...)
		internal static bool ConfirmReminder(DateTypes dateType, int objectId, bool hasRecurrence)
		{
			int stateId = DBTask.GetState(objectId);
			return Common.ConfirmReminder(
				hasRecurrence
				, (int)dateType
				, stateId
				, (int)DateTypes.Task_StartDate
				, (int)DateTypes.Task_FinishDate
				, (int)ObjectStates.Upcoming
				, (int)ObjectStates.Completed
				, (int)ObjectStates.Suspended
				);
		}
		#endregion

		#region GetParent
		public static int GetParent(int TaskId)
		{
			return DBTask.GetParent(TaskId);
		}
		#endregion

		#region AddTimeSheet
		public static void AddTimeSheet(int taskId, int minutes, DateTime dt)
		{
			string title = GetTaskTitle(taskId);
			int projectId = GetProject(taskId);

			if (minutes > 0)
				Mediachase.IbnNext.TimeTracking.TimeTrackingManager.AddHoursForEntryByObject(taskId, (int)TASK_TYPE, title, projectId, Security.CurrentUser.UserID, TimeTracking.GetWeekStart(dt), TimeTracking.GetDayNum(dt), minutes, true);
		}
		#endregion

		#region DeleteSimple
		/// <summary>
		/// This method should be called only during import from MSProject.
		/// </summary>
		/// <param name="taskId"></param>
		public static void DeleteSimple(int task_id)
		{
			ArrayList allRemovedToDo = new ArrayList();
			using (IDataReader todoReader = GetListToDo(task_id))
			{
				while (todoReader.Read())
				{
					int ToDoId = (int)todoReader["ToDoId"];
					allRemovedToDo.Add(ToDoId);
				}
			}

			using (DbTransaction tran = DbTransaction.Begin())
			{
				// OZ: File Storage
				BaseIbnContainer container = BaseIbnContainer.Create("FileLibrary", UserRoleHelper.CreateTaskContainerKey(task_id));
				FileStorage fs = (FileStorage)container.LoadControl("FileStorage");
				fs.DeleteAll();
				//

				// OZ: User Roles
				UserRoleHelper.DeleteTaskRoles(task_id);

				// Delete Todo and don't send todo alerts
				foreach (int ToDoId in allRemovedToDo)
				{
					ToDo.DeleteSimple(ToDoId);
				}

				// Transform Timesheet
				TimeTracking.ResetObjectId((int)TASK_TYPE, task_id);

				// MetaObject
				MetaObject.Delete(task_id, "TaskEx");

				// Delete
				DBTask.DeleteSimple(task_id);

				tran.Commit();
			}
		}
		#endregion

		#region GetMinStartDate
		public static DateTime GetMinStartDate(int projectId)
		{
			DateTime retval = DBTask.GetMinStartDate(projectId);
			if (retval == DateTime.MinValue)
				retval = DateTime.UtcNow;
			return DBCommon.GetLocalDate(Security.CurrentUser.TimeZoneId, retval);
		}
		#endregion
		#region GetMaxFinishDate
		public static DateTime GetMaxFinishDate(int projectId)
		{
			DateTime retval = DBTask.GetMaxFinishDate(projectId);
			if (retval == DateTime.MinValue)
				retval = DateTime.UtcNow;
			return DBCommon.GetLocalDate(Security.CurrentUser.TimeZoneId, retval);
		}
		#endregion

		#region GetTaskDates
		/// <summary>
		/// Gets the task dates.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <returns>ProjectId, ProjectName, TaskId, TaskName,  UserId, UserName, OldDate, NewDate, Created</returns>
		public static IDataReader GetTaskDates(int projectId, DateTime? fromDate, DateTime? toDate)
		{
			int timeZoneId = Security.CurrentUser.TimeZoneId;
			if (fromDate.HasValue)
				fromDate = Security.CurrentUser.CurrentTimeZone.ToUniversalTime(fromDate.Value);
			if (toDate.HasValue)
				toDate = Security.CurrentUser.CurrentTimeZone.ToUniversalTime(toDate.Value);

			return DBTask.GetTaskDates(projectId, fromDate, toDate, Security.CurrentUser.UserID, timeZoneId);
		}
		#endregion
	}

	#region === DV BLOCK ===

	public enum TemplateTaskInfo { NoTask = 1, Task, TaskWithRoleDefine };

	#region class TemplateMakeInfo
	public class TemplateMakeInfo
	{
		public bool SystemFields;
		public bool MetaFields;
		public bool Roles;
		public bool Team;
		public bool Stackhoders;
		public bool Sponsor;
		public bool ExecManager;
		public bool Manager;
		//public TemplateTaskInfo {NoTask=1, Task, TaskWithRoleDefine};
		public TemplateTaskInfo TaskInfo;

		#region ctor: TemplateMakeInfo
		public TemplateMakeInfo(bool systemFields, bool metaFields, bool roles, bool team, bool stackholders, bool sponsor,
			bool manager, bool execManager, TemplateTaskInfo taskInfo)
		{
			this.SystemFields = systemFields;
			this.MetaFields = metaFields;
			this.Roles = roles;
			this.Team = team && roles;
			this.Stackhoders = stackholders && roles;
			this.Sponsor = sponsor && roles;
			this.ExecManager = execManager && roles;
			this.Manager = manager && roles;
			this.TaskInfo = taskInfo;
		}
		#endregion


	}
	#endregion

	#endregion
}
