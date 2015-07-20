using System;
using System.Collections;
using System.Data;

using Mediachase.Ibn;
using Mediachase.IBN.Database;
using Mediachase.MetaDataPlus;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Business
{
	public class Project2
	{
		// Private:
		private const ObjectTypes OBJECT_TYPE = ObjectTypes.Project;
		#region VerifyCanUpdate()
		private static void VerifyCanUpdate(int projectId)
		{
			if(!Project.CanUpdate(projectId))
				throw new AccessDeniedException();
		}
		#endregion

		#region LoadGeneralCategories()
		private static ArrayList LoadGeneralCategories(int projectId)
		{
			ArrayList ret = new ArrayList();
			using(IDataReader reader = Project.GetListCategories(projectId))
			{
				Common.LoadCategories(reader, ret);
			}
			return ret;
		}
		#endregion
		#region LoadProjectCategories()
		public static ArrayList LoadProjectCategories(int projectId)
		{
			ArrayList ret = new ArrayList();
			using(IDataReader reader = Project.GetListProjectCategoriesByProject(projectId))
			{
				Common.LoadCategories(reader, ret);
			}
			return ret;
		}
		#endregion
		#region LoadProjectGroups()
		private static ArrayList LoadProjectGroups(int projectId)
		{
			ArrayList ret = new ArrayList();
			using(IDataReader reader = ProjectGroup.ProjectGroupsGetByProject(projectId))
			{
				while(reader.Read())
				{
					ret.Add((int)reader["ProjectGroupId"]);
				}
			}
			return ret;
		}
		#endregion
		#region LoadSponsors()
		private static ArrayList LoadSponsors(int projectId)
		{
			ArrayList ret = new ArrayList();
			using(IDataReader reader = Project.GetListSponsors(projectId))
			{
				Common.LoadPrincipals(reader, ret);
			}
			return ret;
		}
		#endregion
		#region LoadStakeholders()
		private static ArrayList LoadStakeholders(int projectId)
		{
			ArrayList ret = new ArrayList();
			using(IDataReader reader = Project.GetListStakeholders(projectId))
			{
				Common.LoadPrincipals(reader, ret);
			}
			return ret;
		}
		#endregion

		#region UpdateList()
		private static void UpdateList(ListAction action, ListType type, int projectId, ArrayList items)
		{
			UpdateList(action, type, projectId, items, true);
		}

		private static void UpdateList(ListAction action, ListType type, int projectId, ArrayList items, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(projectId);

			ArrayList oldItems;
			SystemEventTypes eventType;
			UpdateListDelegate fnUpdate;

			switch(type)
			{
				case ListType.GeneralCategories:
					eventType = SystemEventTypes.Project_Updated_GeneralCategories;
					oldItems = LoadGeneralCategories(projectId);
					fnUpdate = new UpdateListDelegate(Common.ListUpdate);
					break;
				case ListType.ProjectCategories:
					eventType = SystemEventTypes.Project_Updated_ProjectCategories;
					oldItems = LoadProjectCategories(projectId);
					fnUpdate = new UpdateListDelegate(ListUpdate);
					break;
				case ListType.ProjectGroups:
					eventType = SystemEventTypes.Project_Updated_ProjectGroups;
					oldItems = LoadProjectGroups(projectId);
					fnUpdate = new UpdateListDelegate(ListUpdate);
					break;
				default:
					throw new Exception("Unknown list type.");
			}
			Common.UpdateList(action, oldItems, items, OBJECT_TYPE, projectId, eventType, fnUpdate, type);
		}
		#endregion
		#region UpdateListMembers()
		private static void UpdateListMembers(ListType type, int projectId, ArrayList items, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(projectId);

			ArrayList oldItems;
			switch(type)
			{
				case ListType.ProjectSponsors:
					oldItems = LoadSponsors(projectId);
					break;
				case ListType.ProjectStakeholders:
					oldItems = LoadStakeholders(projectId);
					break;
				default:
					throw new Exception("Unknown category type.");
			}

			ArrayList add = new ArrayList();
			ArrayList del = new ArrayList();
			foreach(int id in items)
			{
				if(oldItems.Contains(id))
					oldItems.Remove(id);
				else
					add.Add(id);
			}

			del.AddRange(oldItems);

			Mediachase.Ibn.Data.Meta.Management.MetaClass mc = Project.GetProjectMetaClass();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				foreach(int id in del)
				{
					switch(type)
					{
						case ListType.ProjectSponsors:
							DBProject.DeleteSponsor(projectId, id);

							// OZ: FileStorage Addon
							UserRoleHelper.DeleteProjectSponsorRole(projectId,id);

							// IbnNext
							Mediachase.Ibn.Data.Services.RoleManager.RemovePrincipalFromObjectRole(mc, projectId, (int)Project.ProjectRole.Sponsor, id);

							// Alert
							SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_SponsorList_SponsorDeleted, projectId, id);
							break;
						case ListType.ProjectStakeholders:
							DBProject.DeleteStakeholder(projectId, id);

							// OZ: FileStorage Addon
							UserRoleHelper.DeleteProjectStakeHolderRole(projectId,id);

							// IbnNext
							Mediachase.Ibn.Data.Services.RoleManager.RemovePrincipalFromObjectRole(mc, projectId, (int)Project.ProjectRole.Stakeholder, id);

							// Alert
							SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_StakeholderList_StakeholderDeleted, projectId, id);
							break;
						default:
							throw new Exception("Unknown member type.");
					}
				}

				foreach(int id in add)
				{
					switch(type)
					{
						case ListType.ProjectSponsors:
							DBProject.AddSponsor(projectId, id);

							// OZ: FileStorage Addon
							UserRoleHelper.AddProjectSponsorRole(projectId,id);

							// IbnNext
							if (!User.IsGroup(id) || id < 6 || id == 7)		// only users, everyone and roles
								Mediachase.Ibn.Data.Services.RoleManager.AddPrincipalToObjectRole(mc, projectId, (int)Project.ProjectRole.Sponsor, id);

							// Alert
							SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_SponsorList_SponsorAdded, projectId, id);
							break;
						case ListType.ProjectStakeholders:
							DBProject.AddStakeholder(projectId, id);

							// OZ: FileStorage Addon
							UserRoleHelper.AddProjectStakeHolderRole(projectId,id);

							// IbnNext
							if (!User.IsGroup(id) || id < 6 || id == 7)		// only users, everyone and roles
								Mediachase.Ibn.Data.Services.RoleManager.AddPrincipalToObjectRole(mc, projectId, (int)Project.ProjectRole.Stakeholder, id);

							// Alert
							SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_StakeholderList_StakeholderAdded, projectId, id);
							break;
						default:
							throw new Exception("Unknown member type.");
					}
				}

				tran.Commit();
			}
		}
		#endregion
		#region UpdateListTeamMembers()
		private static void UpdateListTeamMembers(int projectId, DataTable items, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(projectId);

			ArrayList oldItems = new ArrayList();
			using(IDataReader reader = Project.GetListTeamMembers(projectId))
			{
				while(reader.Read())
				{
					oldItems.Add((int)reader["UserId"]);
				}
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

			Mediachase.Ibn.Data.Meta.Management.MetaClass mc = Project.GetProjectMetaClass();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				foreach(int id in del)
				{
					DBProject.DeleteTeamMember(projectId, id);

					// OZ: FileStorage Addon
					UserRoleHelper.DeleteProjectTeamRole(projectId, id);

					// IbnNext
					Mediachase.Ibn.Data.Services.RoleManager.RemovePrincipalFromObjectRole(mc, projectId, (int)Project.ProjectRole.TeamMember, id);

					// Alert
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_TeamMemberList_TeamMemberDeleted, projectId, id);
				}

				foreach(DataRow row in items.Rows)
				{
					int id = (int)row["UserId"];
					string code = row["Code"].ToString();
					decimal rate = (decimal)row["Rate"];
					
					if(add.Contains(id))
					{
						DbProject2.AddTeamMember(projectId, id, code, rate);

						// OZ: FileStorage Addon
						UserRoleHelper.AddProjectTeamRole(projectId,id);

						// IbnNext
						Mediachase.Ibn.Data.Services.RoleManager.AddPrincipalToObjectRole(mc, projectId, (int)Project.ProjectRole.TeamMember, id);

						// Alert
						SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_TeamMemberList_TeamMemberAdded, projectId, id);
					}
					else
					{
						if(0 < DbProject2.UpdateTeamMember(projectId, id, code, rate))
							SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_TeamMemberList_TeamMemberUpdated, projectId, id);
					}
				}

				tran.Commit();
			}
		}
		#endregion

		#region ListUpdate()
		private static void ListUpdate(bool add, ObjectTypes objectType, int objectId, int itemId, object context)
		{
			switch((ListType)context)
			{
				case ListType.ProjectCategories:
					if(add)
						DBProject.AssignProjectCategory(objectId, itemId);
					else
						DBProject.RemoveCategoryFromProject(objectId, itemId);
					break;
				case ListType.ProjectGroups:
					if(add)
						DBProjectGroup.AssignProjectGroup(objectId, itemId);
					else
						DBProjectGroup.RemoveProjectFromProjectGroup(objectId, itemId);
					break;
			}
		}
		#endregion


		// Public:
		#region UpdateActualFinishDate()
		public static void UpdateActualFinishDate(int projectId, DateTime date)
		{
			UpdateActualFinishDate(projectId, date, true);
		}

		internal static void UpdateActualFinishDate(int projectId, DateTime date, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(projectId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if(0 < DbProject2.UpdateActualFinishDate(projectId, date))
				{
					// TODO: Call Project.RecalculateAll() if needed.
				
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_ActualFinishDate, projectId);
				}

				tran.Commit();
			}
		}
		#endregion

		#region UpdateActualStartDate()
		public static void UpdateActualStartDate(int projectId, DateTime date)
		{
			UpdateActualStartDate(projectId, date, true);
		}

		internal static void UpdateActualStartDate(int projectId, DateTime date, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(projectId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if(0 < DbProject2.UpdateActualStartDate(projectId, date))
				{
					// TODO: Call Project.RecalculateAll() if needed.
				
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_ActualStartDate, projectId);
				}
				
				tran.Commit();
			}
		}
		#endregion

		#region UpdateClient()
		public static void UpdateClient(int projectId, PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			UpdateClient(projectId, contactUid, orgUid, true);
		}

		internal static void UpdateClient(int projectId, PrimaryKeyId contactUid, PrimaryKeyId orgUid, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(projectId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if (0 < DbProject2.UpdateClient(projectId, contactUid == PrimaryKeyId.Empty ? null : (object)contactUid, orgUid == PrimaryKeyId.Empty ? null : (object)orgUid))
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_Client, projectId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateConfigurationInfo()
		public static void UpdateConfigurationInfo(int projectId, int calendarId, int currencyId, int typeId, int initialPhaseId, int blockTypeId)
		{
			UpdateConfigurationInfo(projectId, calendarId, currencyId, typeId, initialPhaseId, blockTypeId, true);
		}

		internal static void UpdateConfigurationInfo(int projectId, int calendarId, int currencyId, int typeId, int initialPhaseId, int blockTypeId, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(projectId);

			int oldTypeId;
			int oldCalendarId;
			using(IDataReader reader = Project.GetProject(projectId))
			{
				reader.Read();
				oldTypeId = (int)reader["TypeId"];
				oldCalendarId = (int)reader["CalendarId"];
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if (0 < DbProject2.UpdateConfigurationInfo(projectId, calendarId, currencyId, typeId, initialPhaseId, blockTypeId))
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_ConfigurationInfo, projectId);

				if (oldTypeId != typeId)
				{
					string MetaClassName = String.Format("ProjectsEx_{0}", oldTypeId);
					MetaObject.Delete(projectId, MetaClassName);
				}

				if (oldCalendarId != calendarId)
					Project.RecalculateAll(projectId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateExecutiveManager()
		public static void UpdateExecutiveManager(int projectId, int userId)
		{
			UpdateExecutiveManager(projectId, userId, true);
		}

		internal static void UpdateExecutiveManager(int projectId, int userId, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(projectId);

			int prevUserId = -1;
			using(IDataReader reader = Project.GetProject(projectId))
			{
				reader.Read();
				object o = reader["ExecutiveManagerId"];
				if(o != DBNull.Value)
					prevUserId = (int)o;
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if(userId != prevUserId)
				{
					DbProject2.UpdateExecutiveManager(projectId, userId);

					// OZ: FileStorage Addon
					UserRoleHelper.AddExecutiveManagerRole(projectId,userId);

					// IbnNext
					Mediachase.Ibn.Data.Meta.Management.MetaClass mc = Project.GetProjectMetaClass();
					if (prevUserId > 0)
						Mediachase.Ibn.Data.Services.RoleManager.RemovePrincipalFromObjectRole(mc, projectId, (int)Project.ProjectRole.ExecutiveManager, prevUserId);
					if (userId > 0)
						Mediachase.Ibn.Data.Services.RoleManager.AddPrincipalToObjectRole(mc, projectId, (int)Project.ProjectRole.ExecutiveManager, userId);

					// Alerts
					if(prevUserId > 0)
						SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_ExecutiveManager_ExecutiveManagerDeleted, projectId, prevUserId);
					if(userId > 0)
						SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_ExecutiveManager_ExecutiveManagerAdded, projectId, userId);
				}
				tran.Commit();
			}
		}
		#endregion

		#region UpdateGeneralInfo()
		public static void UpdateGeneralInfo(int projectId, string title, string description, string goals, string scope, string deliverables)
		{
			UpdateGeneralInfo(projectId, title, description, goals, scope, deliverables, true);
		}

		internal static void UpdateGeneralInfo(int projectId, string title, string description, string goals, string scope, string deliverables, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(projectId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DbProject2.UpdateGeneralInfo(projectId, title, description, goals, scope, deliverables);

				SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_GeneralInfo, projectId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateManager()
		public static void UpdateManager(int projectId, int userId)
		{
			UpdateManager(projectId, userId, true);
		}

		internal static void UpdateManager(int projectId, int userId, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(projectId);

			int prevUserId = Project.GetProjectManager(projectId);
			if(userId != prevUserId)
			{
				using(DbTransaction tran = DbTransaction.Begin())
				{
					DbProject2.UpdateManager(projectId, userId);

					// [2009-03-16] O.R.
					//DBProject.UpdateManagerAtAssociatedObjects(projectId, userId);

					// OZ: FS Addon
					UserRoleHelper.AddProjectManagerRole(projectId,userId);

					// IbnNext
					Mediachase.Ibn.Data.Meta.Management.MetaClass mc = Project.GetProjectMetaClass();
					Mediachase.Ibn.Data.Services.RoleManager.UpdatePrincipalInObjectRole(mc, projectId, prevUserId, userId, (int)Project.ProjectRole.Manager);

					// Alerts
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_Manager_ManagerDeleted, projectId, prevUserId);
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_Manager_ManagerAdded, projectId, userId);

					tran.Commit();
				}
			}
		}
		#endregion

		#region UpdatePhase()
		public static void UpdatePhase(int projectId, int phaseId)
		{
			UpdatePhase(projectId, phaseId, true);
		}

		internal static void UpdatePhase(int projectId, int phaseId, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(projectId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if(0 < DbProject2.UpdatePhase(projectId, phaseId))
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_Phase, projectId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdatePriority()
		public static void UpdatePriority(int projectId, int priorityId)
		{
			UpdatePriority(projectId, priorityId, true);
		}

		internal static void UpdatePriority(int projectId, int priorityId, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(projectId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if(0 < DbProject2.UpdatePriority(projectId, priorityId))
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_Priority, projectId);

				tran.Commit();
			}
		}
		#endregion

		#region UpPriority()
		public static void UpPriority(int projectId)
		{
			UpPriority(projectId, true);
		}

		internal static void UpPriority(int projectId, bool checkAccess)
		{
			if (checkAccess && !Project.CanUpdate(projectId))
				throw new AccessDeniedException();

			int priorityId = -1;
			using (IDataReader reader = Project.GetProject(projectId))
			{
				if (reader.Read())
					priorityId = (int)reader["PriorityId"];
			}

			Priority priority = (Priority)priorityId;
			bool needChange = true;
			switch (priority)
			{
				case Priority.Low:
					priorityId = (int)Priority.Normal;
					break;
				case Priority.Normal:
					priorityId = (int)Priority.High;
					break;
				case Priority.High:
					priorityId = (int)Priority.VeryHigh;
					break;
				case Priority.VeryHigh:
					priorityId = (int)Priority.Urgent;
					break;
				case Priority.Urgent:
					needChange = false;
					break;
			}

			if (needChange)
				using (DbTransaction tran = DbTransaction.Begin())
				{
					if (0 < DbProject2.UpdatePriority(projectId, priorityId))
						SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_Priority, projectId);

					tran.Commit();
				}
		}
		#endregion

		#region DownPriority()
		public static void DownPriority(int projectId)
		{
			DownPriority(projectId, true);
		}

		internal static void DownPriority(int projectId, bool checkAccess)
		{
			if (checkAccess && !Project.CanUpdate(projectId))
				throw new AccessDeniedException();

			int priorityId = -1;
			using (IDataReader reader = Project.GetProject(projectId))
			{
				if (reader.Read())
					priorityId = (int)reader["PriorityId"];
			}

			Priority priority = (Priority)priorityId;
			bool needChange = true;
			switch (priority)
			{
				case Priority.Low:
					needChange = false;
					break;
				case Priority.Normal:
					priorityId = (int)Priority.Low;
					break;
				case Priority.High:
					priorityId = (int)Priority.Normal;
					break;
				case Priority.VeryHigh:
					priorityId = (int)Priority.High;
					break;
				case Priority.Urgent:
					priorityId = (int)Priority.VeryHigh;
					break;
			}

			if (needChange)
				using (DbTransaction tran = DbTransaction.Begin())
				{
					if (0 < DbProject2.UpdatePriority(projectId, priorityId))
						SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_Priority, projectId);

					tran.Commit();
				}
		}
		#endregion

		#region UpdateProgress()
		public static void UpdateProgress(int projectId, int percent)
		{
			UpdateProgress(projectId, percent, true);
		}

		internal static void UpdateProgress(int projectId, int percent, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(projectId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if(0 < DbProject2.UpdateProgress(projectId, percent))
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_Percent, projectId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateStatus()
		public static void UpdateStatus(int projectId, int statusId)
		{
			UpdateStatus(projectId, statusId, true);
		}

		internal static void UpdateStatus(int projectId, int statusId, bool checkAccess)
		{
			if (checkAccess)
				VerifyCanUpdate(projectId);

			int oldStatusId = DBProject.GetStatus(projectId);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				if (0 < DbProject2.UpdateStatus(projectId, statusId))
				{
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_Status, projectId);

					#region === Activation ===
					if (statusId == (int)Project.ProjectStatus.Active &&
						(
							oldStatusId == (int)Project.ProjectStatus.Completed ||
							oldStatusId == (int)Project.ProjectStatus.OnHold ||
							oldStatusId == (int)Project.ProjectStatus.Pending ||
							oldStatusId == (int)Project.ProjectStatus.Cancelled
						)
					)
					{
						#region Tasks
						ArrayList taskList = new ArrayList();
						using (IDataReader reader = Task.GetListTasksByProject(projectId))
						{
							while (reader.Read())
							{
								if ((bool)reader["IsCompleted"] &&
									(
										(int)reader["ReasonId"] == (int)CompletionReason.SuspendedAutomatically || 
										(int)reader["ReasonId"] == (int)CompletionReason.CompletedAutomatically)
									)

									taskList.Add((int)reader["TaskId"]);
							}
						}

						foreach (int task_id in taskList)
							DBTask.UpdateCompletion(task_id, false, (int)CompletionReason.NotCompleted);

						Task.RecalculateAllStates(projectId);
						#endregion

						#region ToDo
						ArrayList todoList = new ArrayList();
						using (IDataReader reader = ToDo.GetListToDoByProject(projectId))
						{
							while (reader.Read())
							{
								if ((bool)reader["IsCompleted"] &&
									((int)reader["ReasonId"] == (int)CompletionReason.SuspendedAutomatically || (int)reader["ReasonId"] == (int)CompletionReason.CompletedAutomatically))
									todoList.Add((int)reader["ToDoId"]);
							}
						}

						foreach (int todo_id in todoList)
						{
							// OR: We don't recalculate percent

							DBToDo.UpdateCompletion(todo_id, false, (int)CompletionReason.NotCompleted);
							ToDo.RecalculateState(todo_id);
						}

						#endregion

						#region Documents
						ArrayList docList = new ArrayList();
						using (IDataReader reader = Document.GetListDocumentsByProjectAll(projectId))
						{
							while (reader.Read())
							{
								if ((bool)reader["IsCompleted"] &&
									((int)reader["ReasonId"] == (int)CompletionReason.SuspendedAutomatically || (int)reader["ReasonId"] == (int)CompletionReason.CompletedAutomatically))
									docList.Add((int)reader["DocumentId"]);
							}
						}

						foreach (int docId in docList)
						{
							DBDocument.UpdateCompletion(docId, false, (int)CompletionReason.NotCompleted);
							DBDocument.DocumentRecalculateStateDataTable(docId);
						}

						#endregion
					}
					#endregion

					#region === Suspending ===
					else if ((statusId == (int)Project.ProjectStatus.OnHold || statusId == (int)Project.ProjectStatus.Pending) &&
					  (oldStatusId == (int)Project.ProjectStatus.Active || oldStatusId == (int)Project.ProjectStatus.AtRisk))
					{
						#region Tasks
						ArrayList taskList = new ArrayList();
						using (IDataReader reader = Task.GetListTasksByProject(projectId))
						{
							while (reader.Read())
							{
								if (!(bool)reader["IsCompleted"])
									taskList.Add((int)reader["TaskId"]);
							}
						}

						foreach (int task_id in taskList)
						{
							// O.R. [2009-02-12]
							DBCalendar.DeleteStickedObjectForAllUsers((int)ObjectTypes.Task, task_id);

							DBTask.UpdateCompletion(task_id, true, (int)CompletionReason.SuspendedAutomatically);
						}

						Task.RecalculateAllStates(projectId);
						#endregion

						#region ToDo
						ArrayList todoList = new ArrayList();
						using (IDataReader reader = ToDo.GetListToDoByProject(projectId))
						{
							while (reader.Read())
							{
								if (!(bool)reader["IsCompleted"])
									todoList.Add((int)reader["ToDoId"]);
							}
						}

						foreach (int todo_id in todoList)
						{
							DBToDo.UpdateCompletion(todo_id, true, (int)CompletionReason.SuspendedAutomatically);

							// O.R. [2009-02-12]
							DBCalendar.DeleteStickedObjectForAllUsers((int)ObjectTypes.ToDo, todo_id);

							ToDo.RecalculateState(todo_id);
						}

						#endregion

						#region Document
						ArrayList docList = new ArrayList();
						using (IDataReader reader = Document.GetListDocumentsByProjectAll(projectId))
						{
							while (reader.Read())
							{
								if (!(bool)reader["IsCompleted"])
									docList.Add((int)reader["DocumentId"]);
							}
						}

						foreach (int docId in docList)
						{
							DBDocument.UpdateCompletion(docId, true, (int)CompletionReason.SuspendedAutomatically);
							DBDocument.DocumentRecalculateStateDataTable(docId);
						}

						#endregion
					}
					#endregion

					#region == Completion ===
					else if (statusId == (int)Project.ProjectStatus.Completed || statusId == (int)Project.ProjectStatus.Cancelled)
					{
						#region Tasks
						ArrayList taskList = new ArrayList();
						using (IDataReader reader = Task.GetListTasksByProject(projectId))
						{
							while (reader.Read())
							{
								if (!(bool)reader["IsCompleted"] ||
									((int)reader["StateId"] == (int)ObjectStates.Suspended && (int)reader["ReasonId"] == (int)CompletionReason.SuspendedAutomatically))
									taskList.Add((int)reader["TaskId"]);
							}
						}

						foreach (int task_id in taskList)
						{
							// O.R. [2009-02-12]
							DBCalendar.DeleteStickedObjectForAllUsers((int)ObjectTypes.Task, task_id);

							DBTask.UpdateCompletion(task_id, true, (int)CompletionReason.CompletedAutomatically);
						}

						Task.RecalculateAllStates(projectId);
						#endregion

						#region ToDo
						ArrayList todoList = new ArrayList();
						using (IDataReader reader = ToDo.GetListToDoByProject(projectId))
						{
							while (reader.Read())
							{
								if (!(bool)reader["IsCompleted"] ||
									((int)reader["StateId"] == (int)ObjectStates.Suspended && (int)reader["ReasonId"] == (int)CompletionReason.SuspendedAutomatically))
									todoList.Add((int)reader["ToDoId"]);
							}
						}

						foreach (int todo_id in todoList)
						{
							// OR: We don't recalculate percent

							DBToDo.UpdateCompletion(todo_id, true, (int)CompletionReason.CompletedAutomatically);

							// O.R. [2009-02-12]
							DBCalendar.DeleteStickedObjectForAllUsers((int)ObjectTypes.ToDo, todo_id);

							ToDo.RecalculateState(todo_id);
						}

						#endregion

						#region Documents
						ArrayList docList = new ArrayList();
						using (IDataReader reader = Document.GetListDocumentsByProjectAll(projectId))
						{
							while (reader.Read())
							{
								if (!(bool)reader["IsCompleted"] ||
									((int)reader["StateId"] == (int)ObjectStates.Suspended && (int)reader["ReasonId"] == (int)CompletionReason.SuspendedAutomatically))
									docList.Add((int)reader["DocumentId"]);
							}
						}

						foreach (int docId in docList)
						{
							DBDocument.UpdateCompletion(docId, true, (int)CompletionReason.CompletedAutomatically);
							DBDocument.DocumentRecalculateStateDataTable(docId);
						}

						#endregion
					}
					#endregion
				}

				tran.Commit();
			}
		}
		#endregion

		#region UpdateRiskLevel()
		public static void UpdateRiskLevel(int projectId, int riskLevelId)
		{
			UpdateRiskLevel(projectId, riskLevelId, true);
		}

		internal static void UpdateRiskLevel(int projectId, int riskLevelId, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(projectId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if(0 < DbProject2.UpdateRiskLevel(projectId, riskLevelId))
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_RiskLevel, projectId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateTargetDates()
		public static void UpdateTargetDates(int projectId, DateTime startDate, DateTime finishDate)
		{
			UpdateTargetDates(projectId, startDate, finishDate, true);
		}

		internal static void UpdateTargetDates(int projectId, DateTime startDate, DateTime finishDate, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(projectId);

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			startDate = DBCommon.GetUTCDate(TimeZoneId, startDate);
			finishDate = DBCommon.GetUTCDate(TimeZoneId, finishDate);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				int retVal = DbProject2.UpdateTargetDates(projectId, startDate, finishDate);
				if(retVal > 0)
				{
					// TODO: Call Project.RecalculateAll() if needed.
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_TargetTimeline, projectId);

					// O.R: Scheduling
					if (retVal == 1 || retVal == 3)
						Schedule.UpdateDateTypeValue(DateTypes.Project_StartDate, projectId, startDate);
					if (retVal == 2 || retVal == 3)
						Schedule.UpdateDateTypeValue(DateTypes.Project_FinishDate, projectId, finishDate);
				}

				tran.Commit();
			}
		}
		#endregion

		#region UpdateSponsors()
		public static void UpdateSponsors(int projectId, ArrayList members)
		{
			UpdateListMembers(ListType.ProjectSponsors, projectId, members, true);
		}
		#endregion
		#region UpdateStakeholders()
		public static void UpdateStakeholders(int projectId, ArrayList members)
		{
			UpdateListMembers(ListType.ProjectStakeholders, projectId, members, true);
		}
		#endregion
		#region UpdateTeamMembers()
		public static void UpdateTeamMembers(int projectId, DataTable members)
		{
			UpdateListTeamMembers(projectId, members, true);
		}
		#endregion

		#region AddRelation
		public static void AddRelation(int projectId, int relProjectId)
		{
			VerifyCanUpdate(projectId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBProject.AddProjectRelation(projectId, relProjectId);

				SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_RelatedProjectList_RelatedProjectAdded, projectId, relProjectId);
				SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_RelatedProjectList_RelatedProjectAdded, relProjectId, projectId);

				tran.Commit();
			}
		}
		#endregion
		#region DeleteRelation
		public static void DeleteRelation(int projectId, int relProjectId)
		{
			VerifyCanUpdate(projectId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBProject.DeleteProjectRelation(projectId, relProjectId);

				SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_RelatedProjectList_RelatedProjectDeleted, projectId, relProjectId);
				SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_RelatedProjectList_RelatedProjectDeleted, relProjectId, projectId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateProjectCode()
		public static void UpdateProjectCode(int projectId, string projectCode)
		{
			UpdateProjectCode(projectId, projectCode, true);
		}

		internal static void UpdateProjectCode(int projectId, string projectCode, bool checkAccess)
		{
			if (checkAccess)
				VerifyCanUpdate(projectId);

			DbProject2.UpdateProjectCode(projectId, projectCode);
		}
		#endregion

		// Categories
		#region AddGeneralCategories()
		public static void AddGeneralCategories(int projectId, ArrayList categories)
		{
			UpdateList(ListAction.Add, ListType.GeneralCategories, projectId, categories);
		}
		#endregion
		#region RemoveGeneralCategories()
		public static void RemoveGeneralCategories(int projectId, ArrayList categories)
		{
			UpdateList(ListAction.Remove, ListType.GeneralCategories, projectId, categories);
		}
		#endregion
		#region SetGeneralCategories()
		public static void SetGeneralCategories(int projectId, ArrayList categories)
		{
			UpdateList(ListAction.Set, ListType.GeneralCategories, projectId, categories);
		}
		#endregion

		#region AddProjectCategories()
		public static void AddProjectCategories(int projectId, ArrayList categories)
		{
			UpdateList(ListAction.Add, ListType.ProjectCategories, projectId, categories);
		}
		#endregion
		#region RemoveProjectCategories()
		public static void RemoveProjectCategories(int projectId, ArrayList categories)
		{
			UpdateList(ListAction.Remove, ListType.ProjectCategories, projectId, categories);
		}
		#endregion
		#region SetProjectCategories()
		public static void SetProjectCategories(int projectId, ArrayList categories)
		{
			UpdateList(ListAction.Set, ListType.ProjectCategories, projectId, categories);
		}
		#endregion

		// Project Groups
		#region AddProjectGroups()
		public static void AddProjectGroups(int projectId, ArrayList projectGroups)
		{
			UpdateList(ListAction.Add, ListType.ProjectGroups, projectId, projectGroups);
		}
		#endregion
		#region RemoveProjectGroups()
		public static void RemoveProjectGroups(int projectId, ArrayList projectGroups)
		{
			UpdateList(ListAction.Remove, ListType.ProjectGroups, projectId, projectGroups);
		}
		#endregion
		#region SetProjectGroups()
		public static void SetProjectGroups(int projectId, ArrayList projectGroups)
		{
			UpdateList(ListAction.Set, ListType.ProjectGroups, projectId, projectGroups);
		}
		#endregion

		// Batch
		#region UpdateActualDates()
		public static void UpdateActualDates(int projectId, DateTime startDate, DateTime finishDate)
		{
			VerifyCanUpdate(projectId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				UpdateActualStartDate(projectId, startDate, false);
				UpdateActualFinishDate(projectId, finishDate, false);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateStateInfo()
		public static void UpdateStateInfo(int projectId, int statusId, int phaseId, int riskLevelId, int percentCompleted, int priorityId)
		{
			VerifyCanUpdate(projectId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				UpdateStatus(projectId, statusId, false);
				UpdatePhase(projectId, phaseId, false);
				UpdateRiskLevel(projectId, riskLevelId, false);
				UpdateProgress(projectId, percentCompleted, false);
				UpdatePriority(projectId, priorityId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateCategories()
		public static void UpdateCategories(int projectId, ArrayList generalCategories, ArrayList projectCategories, ArrayList projectGroups)
		{
			VerifyCanUpdate(projectId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				UpdateList(ListAction.Set, ListType.GeneralCategories, projectId, generalCategories, false);
				UpdateList(ListAction.Set, ListType.ProjectCategories, projectId, projectCategories, false);
				UpdateList(ListAction.Set, ListType.ProjectGroups, projectId, projectGroups, false);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateManagers()
		public static void UpdateManagers(int projectId, int managerId, int execManagerId)
		{
			VerifyCanUpdate(projectId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				UpdateManager(projectId, managerId, false);
				UpdateExecutiveManager(projectId, execManagerId, false);

				tran.Commit();
			}
		}
		#endregion

		#region Update
		public static void Update(
			int projectId,	
			string title,
			string description,
			string goals,
			string scope,
			string deliverables,
			int managerId,	
			int execManagerId,
			DateTime targetStartDate,
			DateTime targetFinishDate,
			DateTime actualStartDate,
			DateTime actualFinishDate,
			int statusId,		
			int typeId,
			int calendarId,
			//int clientId,	
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid,
			int currencyId,
			int priorityId,	//-
			int initialPhaseId,
			int phaseId,
			int percentCompleted,
			int riskLevelId,
			int blockTypeId,
			ArrayList categories,
			ArrayList projectCategories,
			ArrayList projectGroups)
		{
			VerifyCanUpdate(projectId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				UpdateGeneralInfo(projectId, title, description, goals, scope, deliverables, false);
				UpdateManager(projectId, managerId, false);
				UpdateExecutiveManager(projectId, execManagerId, false);
				UpdateTargetDates(projectId, targetStartDate, targetFinishDate, false);
				UpdateActualStartDate(projectId, actualStartDate, false);
				UpdateActualFinishDate(projectId, actualFinishDate, false);
				UpdateStatus(projectId, statusId, false);
				UpdateConfigurationInfo(projectId, calendarId, currencyId, typeId, initialPhaseId, blockTypeId, false);
				UpdateClient(projectId, contactUid, orgUid, false);
				UpdatePriority(projectId, priorityId, false);
				UpdatePhase(projectId, phaseId, false);
				UpdateProgress(projectId, percentCompleted, false);
				UpdateRiskLevel(projectId, riskLevelId, false);
				UpdateList(ListAction.Set, ListType.GeneralCategories, projectId, categories, false);
				UpdateList(ListAction.Set, ListType.ProjectCategories, projectId, projectCategories, false);
				UpdateList(ListAction.Set, ListType.ProjectGroups, projectId, projectGroups, false);

				tran.Commit();
			}
		}
		#endregion
	}
}
