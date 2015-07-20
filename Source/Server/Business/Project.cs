using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Globalization;
using Mediachase.Ibn;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Business.EMail;
using Mediachase.IBN.Database;
using Mediachase.MetaDataPlus;

namespace Mediachase.IBN.Business
{
	public class Project
	{
		#region FilterKeys
		public const string StartDateFilterKey = "ProjectStartDate";
		public const string EndDateFilterKey = "ProjectEndDate";
		public const string StatusFilterKey = "ProjectStatusId";
		public const string PhaseFilterKey = "ProjectPhaseId";
		public const string TypeFilterKey = "ProjectTypeId";
		public const string ManagerFilterKey = "ProjectManagerId";
		public const string ExecManagerFilterKey = "ProjectExecManagerId";
		public const string PriorityFilterKey = "ProjectPriorityId";
		public const string ClientFilterKey = "ProjectClient";
		public const string PortfolioTypeFilterKey = "ProjectPortfolioType";
		public const string PortfoliosFilterKey = "ProjectPortfolios";
		public const string GenCategoryTypeFilterKey = "ProjectGeneralCategoryType";
		public const string GenCategoriesFilterKey = "ProjectGeneralCategories";
		public const string ProjectCategoryTypeFilterKey = "ProjectCategoryType";
		public const string ProjectCategoriesFilterKey = "ProjectCategories";
		public const string MyParticipationOnlyFilterKey = "ProjectMyParticipationOnly";
		public const string KeywordFilterKey = "ProjectKeyword";
		#endregion

		public enum ProjectFieldSet
		{
			ProjectsDefault,
			ProjectsLight
		}

		public enum AvailableGroupField
		{
			NotSet,
			Portfolio,
			Client
		}

		#region Costants
		public const string CanProjectExecutiveCreateTodo = "CanProjectExecutiveCreateTodo";
		public const string CanProjectTeamCreateTodo = "CanProjectTeamCreateTodo";
		public const string CanProjectSponsorCreateTodo = "CanProjectSponsorCreateTodo";
		public const string CanProjectStakeholderCreateTodo = "CanProjectStakeholderCreateTodo";

		public const string CanProjectExecutiveCreateDocument = "CanProjectExecutiveCreateDocument";
		public const string CanProjectTeamCreateDocument = "CanProjectTeamCreateDocument";
		public const string CanProjectSponsorCreateDocument = "CanProjectSponsorCreateDocument";
		public const string CanProjectStakeholderCreateDocument = "CanProjectStakeholderCreateDocument";

		public const string CanProjectExecutiveCreateTask = "CanProjectExecutiveCreateTask";
		public const string CanProjectTeamCreateTask = "CanProjectTeamCreateTask";
		public const string CanProjectSponsorCreateTask = "CanProjectSponsorCreateTask";
		public const string CanProjectStakeholderCreateTask = "CanProjectStakeholderCreateTask";

		public const string HideManagementForProjectTeam = "HideManagementForProjectTeam";
		#endregion

		#region ProjectRole
		public enum ProjectRole
		{
			Manager = 1,
			ExecutiveManager = 2,
			Sponsor = 3,
			Stakeholder = 4,
			TeamMember = 5
		}
		#endregion

		#region PROJECT_TYPE
		private static ObjectTypes PROJECT_TYPE
		{
			get { return ObjectTypes.Project; }
		}
		#endregion

		#region ProjectSecurity
		public class ProjectSecurity
		{
			public bool IsTeamMember = false;
			public bool IsSponsor = false;
			public bool IsStakeHolder = false;
			public bool IsManager = false;
			public bool IsExecutiveManager = false;

			public ProjectSecurity(int project_id, int user_id)
			{
				using (IDataReader reader = DBProject.GetSecurityForUser(project_id, user_id))
				{
					if (reader.Read())
					{
						IsTeamMember = ((int)reader["IsTeamMember"] > 0) ? true : false;
						IsSponsor = ((int)reader["IsSponsor"] > 0) ? true : false;
						IsStakeHolder = ((int)reader["IsStakeHolder"] > 0) ? true : false;
						IsManager = ((int)reader["IsManager"] > 0) ? true : false;
						IsExecutiveManager = ((int)reader["IsExecutiveManager"] > 0) ? true : false;
					}
				}
			}
		}
		#endregion

		#region ProjectStatus
		public enum ProjectStatus
		{
			Active = 1,
			OnHold = 2,
			Completed = 3,
			Pending = 4,
			AtRisk = 5,
			Cancelled = 6
		}
		#endregion

		#region GetSecurity
		public static ProjectSecurity GetSecurity(int project_id)
		{
			return GetSecurity(project_id, Security.CurrentUser.UserID);
		}

		public static ProjectSecurity GetSecurity(int project_id, int user_id)
		{
			return new ProjectSecurity(project_id, user_id);
		}
		#endregion

		#region CanCreate
		public static bool CanCreate()
		{
			return Security.IsUserInGroup(InternalSecureGroups.ProjectManager) ||
				   Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager);
		}
		#endregion

		#region CanUpdate
		public static bool CanUpdate(int project_id)
		{
			return Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				GetSecurity(project_id).IsManager;
		}
		#endregion

		#region CanDelete
		public static bool CanDelete(int project_id)
		{
			return CanUpdate(project_id);
		}
		#endregion

		#region CanRead(int project_id)
		public static bool CanRead(int project_id)
		{
			bool retval = false;

			if (!Security.CurrentUser.IsExternal)
			{
				// Check by AlertService
				retval = Security.CurrentUser.IsAlertService;
				if (!retval)
				{
					ProjectSecurity ps = GetSecurity(project_id);
					retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
						Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) ||
						ps.IsExecutiveManager || ps.IsManager || ps.IsSponsor || ps.IsStakeHolder || ps.IsTeamMember;
				}

				// Check by Sharing
				if (!retval)
				{
					if (DBProject.GetSharingLevel(Security.CurrentUser.UserID, project_id) >= 0)
						retval = true;
				}
			}
			else		// External User
			{
				retval = false;
			}

			return retval;
		}
		#endregion

		#region CanRead(int project_id, int user_id)
		public static bool CanRead(int project_id, int user_id)
		{
			bool retval = false;

			if (!User.IsExternal(user_id))
			{
				ArrayList secure_groups = User.GetListSecureGroupAll(user_id);
				retval = secure_groups.Contains((int)InternalSecureGroups.PowerProjectManager)
					|| secure_groups.Contains((int)InternalSecureGroups.ExecutiveManager);

				if (!retval)
				{
					ProjectSecurity ps = GetSecurity(project_id, user_id);
					retval = ps.IsExecutiveManager || ps.IsManager || ps.IsSponsor || ps.IsStakeHolder || ps.IsTeamMember;
				}

				// Check by Sharing
				if (!retval)
				{
					if (DBProject.GetSharingLevel(user_id, project_id) >= 0)
						retval = true;
				}
			}

			return retval;
		}
		#endregion

		#region CanViewFinances
		public static bool CanViewFinances(int project_id)
		{
			ProjectSecurity ps = GetSecurity(project_id);
			return Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) ||
				ps.IsManager || ps.IsExecutiveManager;
		}
		#endregion

		#region CanEditFinances
		public static bool CanEditFinances(int project_id)
		{
			ProjectSecurity ps = GetSecurity(project_id);
			return Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				ps.IsManager || ps.IsExecutiveManager;
		}
		#endregion

		#region CanViewManagement
		public static bool CanViewManagement(int projectId)
		{
			bool retVal = false;
			if (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager))
			{
				retVal = true;
			}

			if (!retVal)
			{
				Project.ProjectSecurity ps = Project.GetSecurity(projectId);
				Dictionary<string, string> prop = Project.GetProperties(projectId);
				retVal = ps.IsManager || ps.IsSponsor || ps.IsStakeHolder || ps.IsExecutiveManager
					|| (ps.IsTeamMember && !prop.ContainsKey(Project.HideManagementForProjectTeam)
					);
			}

			return retVal;
		}
		#endregion

		#region CanViewReports
		public static bool CanViewReports(int projectId)
		{
			bool retVal = false;
			if (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager))
			{
				retVal = true;
			}

			if (!retVal)
			{
				Project.ProjectSecurity ps = Project.GetSecurity(projectId);
				Dictionary<string, string> prop = Project.GetProperties(projectId);
				retVal = ps.IsManager || ps.IsSponsor || ps.IsStakeHolder || ps.IsExecutiveManager;
			}

			return retVal;
		}
		#endregion

		#region Create
		public static int Create(
			string title,
			string description,
			string goals,
			string scope,
			string deliverable,
			int manager_id,
			int exmanager_id,
			DateTime target_start_date,
			DateTime target_finish_date,
			DateTime actual_start_date,
			DateTime actual_finish_date,
			int status_id,
			int type_id,
			int calendar_id,
			//int client_id,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid,
			int currency_id,
			int priority_id,
			int initial_phase_id,
			int phase_id,
			int percent_completed,
			int risk_level_id,
			int blocType_id,
			ArrayList categories,
			ArrayList project_categories,
			ArrayList project_Groups)
		{
			return Create(title, description, goals, scope, deliverable, manager_id, exmanager_id,
				target_start_date, target_finish_date, actual_start_date, actual_finish_date,
				status_id, type_id, calendar_id, contactUid, orgUid, currency_id, priority_id, initial_phase_id,
				phase_id, percent_completed, risk_level_id, blocType_id, categories, project_categories,
				project_Groups, (DataTable)null, false);
		}

		public static int CreateUseUniversalTime(
			string title,
			string description,
			string goals,
			string scope,
			string deliverable,
			int manager_id,
			int exmanager_id,
			DateTime target_start_date,
			DateTime target_finish_date,
			DateTime actual_start_date,
			DateTime actual_finish_date,
			int status_id,
			int type_id,
			int calendar_id,
			//int client_id,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid,
			int currency_id,
			int initial_phase_id,
			int phase_id,
			int percent_completed,
			int risk_level_id,
			int blocType_id,
			ArrayList categories,
			ArrayList project_categories,
			ArrayList project_Groups,
			ArrayList TeamMembers,
			bool SendAlerts
			)
		{
			return CreateUseUniversalTime(title, description, goals, scope, deliverable, manager_id,
				exmanager_id, target_start_date, target_finish_date, actual_start_date, actual_finish_date,
				status_id, type_id, calendar_id, contactUid, orgUid, currency_id, (int)Priority.Normal,
				initial_phase_id, phase_id, percent_completed, risk_level_id, blocType_id,
				categories, project_categories, project_Groups, TeamMembers, SendAlerts);
		}

		public static int CreateUseUniversalTime(
			string title,
			string description,
			string goals,
			string scope,
			string deliverable,
			int manager_id,
			int exmanager_id,
			DateTime target_start_date,
			DateTime target_finish_date,
			DateTime actual_start_date,
			DateTime actual_finish_date,
			int status_id,
			int type_id,
			int calendar_id,
			//int client_id,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid,
			int currency_id,
			int priority_id,
			int initial_phase_id,
			int phase_id,
			int percent_completed,
			int risk_level_id,
			int blocType_id,
			ArrayList categories,
			ArrayList project_categories,
			ArrayList project_Groups,
			ArrayList TeamMembers,
			bool SendAlerts
			)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (!CanCreate())
				throw new AccessDeniedException();

			DateTime creation_date = DateTime.UtcNow;

			object oActualFinishDate = null;
			if (actual_finish_date != DateTime.MinValue)
				oActualFinishDate = actual_finish_date;

			object oActualStartDate = null;
			if (actual_start_date != DateTime.MinValue)
				oActualStartDate = actual_start_date;

			object oExecutiveManagerId = null;
			if (exmanager_id > 0)
				oExecutiveManagerId = exmanager_id;

			int UserId = Security.CurrentUser.UserID;

			// Team Members
			//			ArrayList TeamMembers = new ArrayList();
			//			if (dt != null)
			//			{
			//				foreach (DataRow dr in dt.Rows)
			//					TeamMembers.Add((int)dr["UserId"]);
			//			}

			int ProjectId = -1;
			Mediachase.Ibn.Data.Meta.Management.MetaClass mc = GetProjectMetaClass();
			using (DbTransaction tran = DbTransaction.Begin())
			{
				// Add Project to Database
				ProjectId = DBProject.Create(title, description, goals, scope, deliverable,
					UserId, manager_id, oExecutiveManagerId, creation_date,
					target_start_date, target_finish_date, oActualStartDate, oActualFinishDate,
					status_id, type_id, calendar_id,
					contactUid == PrimaryKeyId.Empty ? null : (object)contactUid,
					orgUid == PrimaryKeyId.Empty ? null : (object)orgUid,
					currency_id, priority_id,
					initial_phase_id, phase_id, percent_completed, risk_level_id, blocType_id);

				// OZ: FileStorage Addon
				UserRoleHelper.AddProjectManagerRole(ProjectId, manager_id);
				if (exmanager_id > 0)
					UserRoleHelper.AddExecutiveManagerRole(ProjectId, exmanager_id);
				// OZ: End FileStorage Addon

				// Categories
				foreach (int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)PROJECT_TYPE, ProjectId, CategoryId);
				}

				// Project Categories
				foreach (int CategoryId in project_categories)
				{
					DBProject.AssignProjectCategory(ProjectId, CategoryId);
				}

				foreach (int ProjectGroupId in project_Groups)
				{
					DBProjectGroup.AssignProjectGroup(ProjectId, ProjectGroupId);
				}

				// Finances
				DBFinance.CreateRootAccount(ProjectId, Finance.ROOT_ACCOUNT, UserId);

				// O.R: Scheduling
				Schedule.UpdateDateTypeValue(DateTypes.Project_StartDate, ProjectId, target_start_date);
				Schedule.UpdateDateTypeValue(DateTypes.Project_FinishDate, ProjectId, target_finish_date);

				// IbnNext
				Mediachase.Ibn.Data.Services.RoleManager.AddPrincipalToObjectRole(mc, ProjectId, (int)Project.ProjectRole.Manager, manager_id);
				if (exmanager_id > 0)
					Mediachase.Ibn.Data.Services.RoleManager.AddPrincipalToObjectRole(mc, ProjectId, (int)Project.ProjectRole.ExecutiveManager, exmanager_id);

				// Team Members
				if (TeamMembers != null)
				{
					// Add new Team Members 
					foreach (int MemberId in TeamMembers)
					{
						UserLight Principal = UserLight.Load(MemberId);

						int PrincipalId = MemberId;

						string Code = string.Format("{0}{1}", Principal.FirstName.ToUpper()[0], Principal.LastName.ToUpper()[0]);

						DBProject.AddTeamMember(ProjectId, Principal.UserID, Code, 0);

						// OZ: FileStorage Addon
						UserRoleHelper.AddProjectTeamRole(ProjectId, MemberId);
						// OZ: End FileStorage Addon

						// IbnNext
						Mediachase.Ibn.Data.Services.RoleManager.AddPrincipalToObjectRole(mc, ProjectId, (int)Project.ProjectRole.TeamMember, PrincipalId);
					}
				}

				if (SendAlerts)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Created, ProjectId);

				tran.Commit();
			}

			return ProjectId;
		}

		public static int Create(
			string title,
			string description,
			string goals,
			string scope,
			string deliverable,
			int manager_id,
			int exmanager_id,
			DateTime target_start_date,
			DateTime target_finish_date,
			DateTime actual_start_date,
			DateTime actual_finish_date,
			int status_id,
			int type_id,
			int calendar_id,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid,
			int currency_id,
			int priority_id,
			int initial_phase_id,
			int phase_id,
			int percent_completed,
			int risk_level_id,
			int blocType_id,
			ArrayList categories,
			ArrayList project_categories,
			ArrayList project_Groups,
			ArrayList TeamMembers,
			bool SendAlerts
			)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (!CanCreate())
				throw new AccessDeniedException();

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime creation_date = DateTime.UtcNow;
			target_start_date = DBCommon.GetUTCDate(TimeZoneId, target_start_date);
			target_finish_date = DBCommon.GetUTCDate(TimeZoneId, target_finish_date);

			object oActualFinishDate = null;
			if (actual_finish_date != DateTime.MinValue)
				oActualFinishDate = DBCommon.GetUTCDate(TimeZoneId, actual_finish_date);

			object oActualStartDate = null;
			if (actual_start_date != DateTime.MinValue)
				oActualStartDate = DBCommon.GetUTCDate(TimeZoneId, actual_start_date);

			object oExecutiveManagerId = null;
			if (exmanager_id > 0)
				oExecutiveManagerId = exmanager_id;

			int UserId = Security.CurrentUser.UserID;

			// Team Members
			//			ArrayList TeamMembers = new ArrayList();
			//			if (dt != null)
			//			{
			//				foreach (DataRow dr in dt.Rows)
			//					TeamMembers.Add((int)dr["UserId"]);
			//			}

			int ProjectId = -1;
			Mediachase.Ibn.Data.Meta.Management.MetaClass mc = GetProjectMetaClass();
			using (DbTransaction tran = DbTransaction.Begin())
			{
				// Add Project to Database
				ProjectId = DBProject.Create(title, description, goals, scope, deliverable,
					UserId, manager_id, oExecutiveManagerId, creation_date,
					target_start_date, target_finish_date, oActualStartDate, oActualFinishDate,
					status_id, type_id, calendar_id,
					contactUid == PrimaryKeyId.Empty ? null : (object)contactUid,
					orgUid == PrimaryKeyId.Empty ? null : (object)orgUid,
					currency_id, priority_id,
					initial_phase_id, phase_id, percent_completed, risk_level_id, blocType_id);

				// OZ: FileStorage Addon
				UserRoleHelper.AddProjectManagerRole(ProjectId, manager_id);
				if (exmanager_id > 0)
					UserRoleHelper.AddExecutiveManagerRole(ProjectId, exmanager_id);
				// OZ: End FileStorage Addon

				// Categories
				foreach (int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)PROJECT_TYPE, ProjectId, CategoryId);
				}

				// Project Categories
				foreach (int CategoryId in project_categories)
				{
					DBProject.AssignProjectCategory(ProjectId, CategoryId);
				}

				foreach (int ProjectGroupId in project_Groups)
				{
					DBProjectGroup.AssignProjectGroup(ProjectId, ProjectGroupId);
				}

				// Finances
				DBFinance.CreateRootAccount(ProjectId, Finance.ROOT_ACCOUNT, UserId);

				// O.R: Scheduling
				Schedule.UpdateDateTypeValue(DateTypes.Project_StartDate, ProjectId, target_start_date);
				Schedule.UpdateDateTypeValue(DateTypes.Project_FinishDate, ProjectId, target_finish_date);

				// IbnNext
				Mediachase.Ibn.Data.Services.RoleManager.AddPrincipalToObjectRole(mc, ProjectId, (int)Project.ProjectRole.Manager, manager_id);
				if (exmanager_id > 0)
					Mediachase.Ibn.Data.Services.RoleManager.AddPrincipalToObjectRole(mc, ProjectId, (int)Project.ProjectRole.ExecutiveManager, exmanager_id);

				// Team Members
				if (TeamMembers != null)
				{

					// Add new Team Members 
					foreach (int MemberId in TeamMembers)
					{
						UserLight Principal = UserLight.Load(MemberId);

						int PrincipalId = MemberId;

						string Code = string.Format("{0}{1}", Principal.FirstName.ToUpper()[0], Principal.LastName.ToUpper()[0]);

						DBProject.AddTeamMember(ProjectId, Principal.UserID, Code, 0);

						// OZ: FileStorage Addon
						UserRoleHelper.AddProjectTeamRole(ProjectId, MemberId);
						// OZ: End FileStorage Addon

						// IbnNext
						Mediachase.Ibn.Data.Services.RoleManager.AddPrincipalToObjectRole(mc, ProjectId, (int)Project.ProjectRole.TeamMember, PrincipalId);
					}
				}

				if (SendAlerts)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Created, ProjectId);

				tran.Commit();
			}

			return ProjectId;
		}
		#endregion

		#region Create
		public static int Create(
								string title,
								string description,
								string goals,
								string scope,
								string deliverable,
								int manager_id,
								int exmanager_id,
								DateTime target_start_date,
								DateTime target_finish_date,
								DateTime actual_start_date,
								DateTime actual_finish_date,
								int status_id,
								int type_id,
								int calendar_id,
			//int client_id,
								PrimaryKeyId contactUid,
								PrimaryKeyId orgUid,
								int currency_id,
								int priority_id,
								int initial_phase_id,
								int phase_id,
								int percent_completed,
								int risk_level_id,
								int blocType_id,
								ArrayList categories,
								ArrayList project_categories,
								ArrayList project_Groups,
								DataTable dt,
								bool SendAlerts
				)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (!CanCreate())
				throw new AccessDeniedException();

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			DateTime creation_date = DateTime.UtcNow;
			target_start_date = DBCommon.GetUTCDate(TimeZoneId, target_start_date);
			target_finish_date = DBCommon.GetUTCDate(TimeZoneId, target_finish_date);

			object oActualStartDate = null;
			if (actual_start_date != DateTime.MinValue)
				oActualStartDate = DBCommon.GetUTCDate(TimeZoneId, actual_start_date);

			object oActualFinishDate = null;
			if (actual_finish_date != DateTime.MinValue)
				oActualFinishDate = DBCommon.GetUTCDate(TimeZoneId, actual_finish_date);

			object oExecutiveManagerId = null;
			if (exmanager_id > 0)
				oExecutiveManagerId = exmanager_id;

			int UserId = Security.CurrentUser.UserID;

			// Team Members
			ArrayList TeamMembers = new ArrayList();
			if (dt != null)
			{
				foreach (DataRow dr in dt.Rows)
					TeamMembers.Add((int)dr["UserId"]);
			}

			int ProjectId = -1;
			Mediachase.Ibn.Data.Meta.Management.MetaClass mc = Project.GetProjectMetaClass();
			using (DbTransaction tran = DbTransaction.Begin())
			{
				// Add Project to Database
				ProjectId = DBProject.Create(title, description, goals, scope, deliverable,
					UserId, manager_id, oExecutiveManagerId, creation_date,
					target_start_date, target_finish_date, oActualStartDate, oActualFinishDate,
					status_id, type_id, calendar_id,
					contactUid == PrimaryKeyId.Empty ? null : (object)contactUid,
					orgUid == PrimaryKeyId.Empty ? null : (object)orgUid,
					currency_id, priority_id,
					initial_phase_id, phase_id, percent_completed, risk_level_id, blocType_id);

				// OZ: FileStorage Addon
				UserRoleHelper.AddProjectManagerRole(ProjectId, manager_id);
				if (exmanager_id > 0)
					UserRoleHelper.AddExecutiveManagerRole(ProjectId, exmanager_id);
				// OZ: End FileStorage Addon

				// Categories
				foreach (int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)PROJECT_TYPE, ProjectId, CategoryId);
				}

				// Project Categories
				foreach (int CategoryId in project_categories)
				{
					DBProject.AssignProjectCategory(ProjectId, CategoryId);
				}

				foreach (int ProjectGroupId in project_Groups)
				{
					DBProjectGroup.AssignProjectGroup(ProjectId, ProjectGroupId);
				}

				// Finances
				DBFinance.CreateRootAccount(ProjectId, Finance.ROOT_ACCOUNT, UserId);

				// O.R: Scheduling
				Schedule.UpdateDateTypeValue(DateTypes.Project_StartDate, ProjectId, target_start_date);
				Schedule.UpdateDateTypeValue(DateTypes.Project_FinishDate, ProjectId, target_finish_date);

				// IbnNext
				Mediachase.Ibn.Data.Services.RoleManager.AddPrincipalToObjectRole(mc, ProjectId, (int)Project.ProjectRole.Manager, manager_id);
				if (exmanager_id > 0)
					Mediachase.Ibn.Data.Services.RoleManager.AddPrincipalToObjectRole(mc, ProjectId, (int)Project.ProjectRole.ExecutiveManager, exmanager_id);

				// Team Members
				if (dt != null)
				{
					// Add new Team Members 
					foreach (DataRow dr in dt.Rows)
					{
						int PrincipalId = (int)dr["UserId"];
						string Code = dr["Code"].ToString();
						decimal Rate = (decimal)dr["Rate"];

						DBProject.AddTeamMember(ProjectId, PrincipalId, Code, Rate);

						// OZ: FileStorage Addon
						UserRoleHelper.AddProjectTeamRole(ProjectId, PrincipalId);
						// OZ: End FileStorage Addon

						// IbnNext
						Mediachase.Ibn.Data.Services.RoleManager.AddPrincipalToObjectRole(mc, ProjectId, (int)Project.ProjectRole.TeamMember, PrincipalId);
					}
				}

				if (SendAlerts)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Created, ProjectId);

				tran.Commit();
			}

			return ProjectId;
		}
		#endregion

		#region Delete
		public static void Delete(int project_id)
		{
			if (!CanDelete(project_id))
				throw new AccessDeniedException();

			int TypeId = -1;
			using (IDataReader reader = DBProject.GetProject(project_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (reader.Read())
					TypeId = (int)reader["TypeId"];
			}

			// ToDo 
			ArrayList ToDoList = new ArrayList();
			using (IDataReader reader = ToDo.GetListToDoByProject(project_id))
			{
				while (reader.Read())
					ToDoList.Add((int)reader["ToDoId"]);
			}

			// Tasks 
			ArrayList TaskList = new ArrayList();
			using (IDataReader reader = Task.GetListTasksByProject(project_id))
			{
				while (reader.Read())
					TaskList.Add((int)reader["TaskId"]);
			}

			// Events 
			ArrayList EventList = new ArrayList();
			using (IDataReader reader = CalendarEntry.GetListEventsByProject(project_id))
			{
				while (reader.Read())
					EventList.Add((int)reader["EventId"]);
			}

			// Incidents 
			ArrayList IncidentList = new ArrayList();
			using (IDataReader reader = Incident.GetListIncidentsByProject(project_id))
			{
				while (reader.Read())
					IncidentList.Add((int)reader["IncidentId"]);
			}

			// Documents 
			ArrayList DocumentList = new ArrayList();
			using (IDataReader reader = Document.GetListDocumentsByProject(project_id))
			{
				while (reader.Read())
					DocumentList.Add((int)reader["DocumentId"]);
			}

			using (DbTransaction tran = DbTransaction.Begin())
			{
				// OZ: File Storage
				BaseIbnContainer container = BaseIbnContainer.Create("FileLibrary", UserRoleHelper.CreateProjectContainerKey(project_id));
				FileStorage fs = (FileStorage)container.LoadControl("FileStorage");
				fs.DeleteAll();
				//

				// OZ: User Roles
				UserRoleHelper.DeleteProjectRoles(project_id);

				// DV: Actual finances
				SpreadSheet.ProjectSpreadSheet.Deactivate(project_id, false);

				// ToDo
				foreach (int ToDoId in ToDoList)
				{
					// OZ: File Storage
					BaseIbnContainer container2 = BaseIbnContainer.Create("FileLibrary", UserRoleHelper.CreateTodoContainerKey(ToDoId));
					FileStorage fs2 = (FileStorage)container2.LoadControl("FileStorage");
					fs2.DeleteAll();
					//

					MetaObject.Delete(ToDoId, "TodoEx");

					// OZ: User Roles
					UserRoleHelper.DeleteTodoRoles(ToDoId);
				}
				DBToDo.DeleteByProject(project_id);

				// Tasks
				foreach (int TaskId in TaskList)
				{
					// OZ: File Storage
					BaseIbnContainer container2 = BaseIbnContainer.Create("FileLibrary", UserRoleHelper.CreateTaskContainerKey(TaskId));
					FileStorage fs2 = (FileStorage)container2.LoadControl("FileStorage");
					fs2.DeleteAll();
					//

					MetaObject.Delete(TaskId, "TaskEx");

					// OZ: User Roles
					UserRoleHelper.DeleteTaskRoles(TaskId);
				}
				DBTask.DeleteAll(project_id);

				// Events
				foreach (int EventId in EventList)
				{
					// OZ: File Storage
					BaseIbnContainer container2 = BaseIbnContainer.Create("FileLibrary", UserRoleHelper.CreateEventContainerKey(EventId));
					FileStorage fs2 = (FileStorage)container2.LoadControl("FileStorage");
					fs2.DeleteAll();
					//

					MetaObject.Delete(EventId, "EventsEx");

					// OZ: User Roles
					UserRoleHelper.DeleteEventRoles(EventId);
				}
				DBEvent.DeleteByProject(project_id);

				// OZ: 2008-12-04 Remove Project Id From POP3 Box Mapping
				foreach (EMailRouterPop3Box box in EMailRouterPop3Box.ListExternal())
				{
					if (box.Settings.DefaultEMailIncidentMappingBlock.ProjectId == project_id)
					{
						box.Settings.DefaultEMailIncidentMappingBlock.ProjectId = -1;

						EMailRouterPop3BoxSettings.Save(box.Settings);
					}
				}

				// Incidents
				foreach (int IncidentId in IncidentList)
				{
					// OZ: File Storage
					BaseIbnContainer container2 = BaseIbnContainer.Create("FileLibrary", UserRoleHelper.CreateIssueContainerKey(IncidentId));
					FileStorage fs2 = (FileStorage)container2.LoadControl("FileStorage");
					fs2.DeleteAll();
					//

					BaseIbnContainer FOcontainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", IncidentId));
					ForumStorage forumStorage = (ForumStorage)FOcontainer.LoadControl("ForumStorage");

					// Delete Forum Files
					foreach (ForumThreadNodeInfo node in forumStorage.GetForumThreadNodes())
					{
						BaseIbnContainer FScontainer = BaseIbnContainer.Create("FileLibrary", string.Format("ForumNodeId_{0}", node.Id));
						FileStorage FSfileStorage = (FileStorage)FScontainer.LoadControl("FileStorage");
						FSfileStorage.DeleteAll();
					}

					// Delete Forum
					forumStorage.Delete();

					MetaObject.Delete(IncidentId, "IncidentsEx");

					// OZ: User Roles
					UserRoleHelper.DeleteIssueRoles(IncidentId);
				}
				DBIncident.DeleteByProject(project_id);

				// Documentss
				foreach (int DocumentId in DocumentList)
				{
					// OZ: File Storage
					BaseIbnContainer container2 = BaseIbnContainer.Create("FileLibrary", UserRoleHelper.CreateDocumentContainerKey(DocumentId));
					FileStorage fs2 = (FileStorage)container2.LoadControl("FileStorage");
					fs2.DeleteAll();
					//

					MetaObject.Delete(DocumentId, "DocumentsEx");

					// OZ: User Roles
					UserRoleHelper.DeleteDocumentRoles(DocumentId);
				}
				DBDocument.DeleteByProject(project_id);

				string MetaClassName = String.Format("ProjectsEx_{0}", TypeId);
				MetaObject.Delete(project_id, MetaClassName);

				SystemEvents.AddSystemEvents(SystemEventTypes.Project_Deleted, project_id);

				DBProject.Delete(project_id);

				tran.Commit();
			}
		}
		#endregion

		#region GetProject
		/// <summary>
		///		ProjectId, TypeId, TypeName, CalendarId, CalendarName, CreatorId, 
		///		ManagerId, ExecutiveManagerId, Title, Description, CreationDate, 
		///		StartDate, FinishDate, TargetStartDate, TargetFinishDate, ActualStartDate, ActualFinishDate, FixedHours, 
		///		FixedCost, Goals, Scope, Deliverables, StatusId, IsActive, StatusName, 
		///		ContactUid, OrgUid, ClientName, XMLFileId, CurrencyId, CurrencySymbol,
		///		PriorityId, PriorityName, PercentCompleted, PhaseId, PhaseName,
		///		RiskLevelId, RiskLevelName, RiskLevelWeight, InitialPhaseId, InitialPhaseName, IsMSProject,
		///		TaskTime, TotalMinutes, TotalApproved, BlockTypeId, ProjectCode
		/// </summary>
		public static IDataReader GetProject(int project_id)
		{
			return GetProject(project_id, true);
		}

		public static IDataReader GetProject(int project_id, bool checkAccess)
		{
			if (checkAccess && !CanRead(project_id))
				throw new AccessDeniedException();

			return DBProject.GetProject(project_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListProjects
		/// <summary>
		///		ProjectId, TypeId, TypeName, CalendarId, CreatorId, ManagerId, ManagerName, 
		///		ExecutiveManagerId, Title, Description, PriorityId, PriorityName, PercentCompleted,
		///		CreationDate, StartDate, FinishDate, TargetStartDate, TargetFinishDate, 
		///		ActualStartDate, ActualFinishDate, FixedHours, FixedCost, Goals, Scope, 
		///		Deliverables, StatusId, StatusName,PhaseId, PhaseName, 
		///		RiskLevelId, RiskLevelName, RiskLevelWeight, CanEdit, CanDelete
		/// </summary>
		public static IDataReader GetListProjects()
		{
			return DBProject.GetListProjects(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}

		public static DataTable GetListProjectsDataTable()
		{
			return DBProject.GetListProjectsDataTable(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListProjectsForPMAndPPM
		/// <summary>
		/// ProjectId, TypeId, CalendarId, CreatorId, ManagerId, ManagerName, ExecutiveManagerId,
		/// Title, Description, PercentCompleted, CreationDate, StartDate, FinishDate, 
		/// TargetStartDate, TargetFinishDate, ActualStartDate, ActualFinishDate, 
		/// FixedHours, FixedCost, Goals, Scope, 
		/// Deliverables, StatusId, StatusName, CanEdit, CanDelete
		/// </summary>
		public static IDataReader GetListProjectsForPMAndPPM()
		{
			return DBProject.GetListProjectsForPMAndPPM(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListProjectsSimple
		/// <summary>
		/// Reader returns fields:
		///		ProjectId, Title
		/// </summary>
		public static IDataReader GetListProjectsSimple()
		{
			return DBProject.GetListProjectsSimple(Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListProjectsByFilterDataTable
		/// <summary>
		///	 ProjectId, TypeId, CalendarId, CreatorId, ManagerId, ManagerName,
		///	 ExecutiveManagerId, Title, Description, PriorityId, PriorityName, PercentCompleted,
		///	 CreationDate, StartDate, FinishDate, TargetStartDate, TargetFinishDate, 
		///	 ActualStartDate, ActualFinishDate, StatusId, StatusName, PhaseId, PhaseName, 
		///	 PhaseWeight, RiskLevelId, RiskLevelName, RiskLevelWeight, ProjectCode,
		///	 CanEdit, CanDelete, CanViewSnapShot
		/// </summary>
		public static DataTable GetListProjectsByFilterDataTable(
			string SearchString,
			int state,
			int status_id,
			int type_id,
			int priority_id,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid,
			int start_date_condition,
			DateTime start_date,
			int finis_date_condition,
			DateTime finish_date,
			int manager_id,
			int executive_manager_id,
			int CategoryType,
			int ProjectCategoryType,
			int ProjectGroupType,
			int PhaseId,
			bool OnlyForUser)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			int UserId = Security.CurrentUser.UserID;
			start_date = DBCommon.GetUTCDate(TimeZoneId, start_date);
			finish_date = DBCommon.GetUTCDate(TimeZoneId, finish_date);

			return DBProject.GetListProjectsByFilterDataTable(UserId, TimeZoneId,
				SearchString, state, status_id, type_id, priority_id,
				contactUid == PrimaryKeyId.Empty ? null : (object)contactUid,
				orgUid == PrimaryKeyId.Empty ? null : (object)orgUid,
				start_date_condition, start_date, finis_date_condition, finish_date,
				manager_id, executive_manager_id, Security.CurrentUser.LanguageId,
				CategoryType, ProjectCategoryType, ProjectGroupType, PhaseId, OnlyForUser);
		}

		public static DataTable GetListProjectsByFilterDataTable(string keyword, FilterElementCollection fec,
			FilterElementCollection fecAbove)
		{
			#region Variables
			int state = 0; int status_id = 0; int phase_id = 0; int type_id = 0; int priority_id = -1;
			int sdc = 0; DateTime startdate = DateTime.Now; int fdc = 0; DateTime finishdate = DateTime.Now;
			int manager_id = 0; int exemanager_id = 0; int prjGroup_type = 0; int genCategory_type = 0;
			int prjCategory_type = 0;
			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			bool isParticipationOnly = false;
			#endregion

			foreach (FilterElement fe in fec)
			{
				string source = fe.Source;
				string value = fe.Value.ToString();
				if (fe.ValueIsTemplate)
					value = TemplateResolver.ResolveAll(value);
				#region BindVariables
				switch (source)
				{
					case Project.StatusFilterKey:
						int val = int.Parse(value);
						if (val > 0)
							status_id = int.Parse(value);
						else if (val < 0)
							state = Math.Abs(val);
						break;
					case Project.PhaseFilterKey:
						phase_id = int.Parse(value);
						break;
					case Project.TypeFilterKey:
						type_id = int.Parse(value);
						break;
					case Project.PriorityFilterKey:
						priority_id = int.Parse(value);
						break;
					case Project.StartDateFilterKey:
						switch (fe.Type)
						{
							case FilterElementType.GreaterOrEqual:
								sdc = 1;
								startdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							case FilterElementType.LessOrEqual:
								sdc = 2;
								startdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							case FilterElementType.Equal:
								sdc = 3;
								startdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							default:
								break;
						}
						break;
					case Project.EndDateFilterKey:
						switch (fe.Type)
						{
							case FilterElementType.GreaterOrEqual:
								fdc = 1;
								finishdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							case FilterElementType.LessOrEqual:
								fdc = 2;
								finishdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							case FilterElementType.Equal:
								fdc = 3;
								finishdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							default:
								break;
						}
						break;
					case Project.ManagerFilterKey:
						manager_id = int.Parse(value);
						break;
					case Project.ExecManagerFilterKey:
						exemanager_id = int.Parse(value);
						break;
					case Project.ClientFilterKey:
						Incident.GetContactId(value, out orgUid, out contactUid);
						break;
					case Project.GenCategoryTypeFilterKey:
						genCategory_type = int.Parse(value);
						break;
					case Project.GenCategoriesFilterKey:
						ArrayList alGenCats = new ArrayList();
						string[] mas = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in mas)
						{
							if (!alGenCats.Contains(int.Parse(s)))
								alGenCats.Add(int.Parse(s));
						}
						Project.SaveGeneralCategories(alGenCats);
						break;
					case Project.ProjectCategoryTypeFilterKey:
						prjCategory_type = int.Parse(value);
						break;
					case Project.ProjectCategoriesFilterKey:
						ArrayList alPrjCats = new ArrayList();
						string[] masPrj = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in masPrj)
						{
							if (!alPrjCats.Contains(int.Parse(s)))
								alPrjCats.Add(int.Parse(s));
						}
						Project.SaveProjectCategories(alPrjCats);
						break;
					case Project.PortfolioTypeFilterKey:
						prjGroup_type = int.Parse(value);
						break;
					case Project.PortfoliosFilterKey:
						ArrayList alPrjGroups = new ArrayList();
						string[] masPrjGrp = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in masPrjGrp)
						{
							if (!alPrjGroups.Contains(int.Parse(s)))
								alPrjGroups.Add(int.Parse(s));
						}
						Project.SaveProjectGroups(alPrjGroups);
						break;
					case Project.MyParticipationOnlyFilterKey:
						if (bool.Parse(value))
							isParticipationOnly = true;
						break;
					default:
						break;
				}
				#endregion
			}

			foreach (FilterElement fe in fecAbove)
			{
				string source = fe.Source;
				string value = fe.Value.ToString();
				if (fe.ValueIsTemplate)
					value = TemplateResolver.ResolveAll(value);
				#region BindVariables
				switch (source)
				{
					case Project.StatusFilterKey:
						int val = int.Parse(value);
						if (val > 0)
							status_id = int.Parse(value);
						else if (val < 0)
							state = Math.Abs(val);
						break;
					case Project.PhaseFilterKey:
						phase_id = int.Parse(value);
						break;
					case Project.TypeFilterKey:
						type_id = int.Parse(value);
						break;
					case Project.PriorityFilterKey:
						priority_id = int.Parse(value);
						break;
					case Project.StartDateFilterKey:
						switch (fe.Type)
						{
							case FilterElementType.GreaterOrEqual:
								sdc = 1;
								startdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							case FilterElementType.LessOrEqual:
								sdc = 2;
								startdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							case FilterElementType.Equal:
								sdc = 3;
								startdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							default:
								break;
						}
						break;
					case Project.EndDateFilterKey:
						switch (fe.Type)
						{
							case FilterElementType.GreaterOrEqual:
								fdc = 1;
								finishdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							case FilterElementType.LessOrEqual:
								fdc = 2;
								finishdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							case FilterElementType.Equal:
								fdc = 3;
								finishdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							default:
								break;
						}
						break;
					case Project.ManagerFilterKey:
						manager_id = int.Parse(value);
						break;
					case Project.ExecManagerFilterKey:
						exemanager_id = int.Parse(value);
						break;
					case Project.ClientFilterKey:
						Incident.GetContactId(value, out orgUid, out contactUid);
						break;
					case Project.GenCategoryTypeFilterKey:
						genCategory_type = int.Parse(value);
						break;
					case Project.GenCategoriesFilterKey:
						ArrayList alGenCats = new ArrayList();
						string[] mas = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in mas)
						{
							if (!alGenCats.Contains(int.Parse(s)))
								alGenCats.Add(int.Parse(s));
						}
						Project.SaveGeneralCategories(alGenCats);
						break;
					case Project.ProjectCategoryTypeFilterKey:
						prjCategory_type = int.Parse(value);
						break;
					case Project.ProjectCategoriesFilterKey:
						ArrayList alPrjCats = new ArrayList();
						string[] masPrj = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in masPrj)
						{
							if (!alPrjCats.Contains(int.Parse(s)))
								alPrjCats.Add(int.Parse(s));
						}
						Project.SaveProjectCategories(alPrjCats);
						break;
					case Project.PortfolioTypeFilterKey:
						prjGroup_type = int.Parse(value);
						break;
					case Project.PortfoliosFilterKey:
						ArrayList alPrjGroups = new ArrayList();
						string[] masPrjGrp = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in masPrjGrp)
						{
							if (!alPrjGroups.Contains(int.Parse(s)))
								alPrjGroups.Add(int.Parse(s));
						}
						Project.SaveProjectGroups(alPrjGroups);
						break;
					case Project.MyParticipationOnlyFilterKey:
						if (bool.Parse(value))
							isParticipationOnly = true;
						break;
					default:
						break;
				}
				#endregion
			}

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			int UserId = Security.CurrentUser.UserID;
			startdate = DBCommon.GetUTCDate(TimeZoneId, startdate);
			finishdate = DBCommon.GetUTCDate(TimeZoneId, finishdate);

			return DBProject.GetListProjectsByFilterDataTable(UserId, TimeZoneId,
				keyword, state, status_id, type_id, priority_id,
				contactUid == PrimaryKeyId.Empty ? null : (object)contactUid,
				orgUid == PrimaryKeyId.Empty ? null : (object)orgUid,
				sdc, startdate, fdc, finishdate,
				manager_id, exemanager_id, Security.CurrentUser.LanguageId,
				genCategory_type, prjCategory_type, prjGroup_type, phase_id, isParticipationOnly);
		}
		#endregion

		#region GetListActiveProjectsByUserDataTable
		/// <summary>
		/// DataTable contains columns:
		///		ProjectId, Title, ManagerId, CreationDate, TargetFinishDate
		/// </summary>
		public static DataTable GetListActiveProjectsByUserDataTable()
		{
			return DBProject.GetListActiveProjectsByUserDataTable(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListActiveProjectsByUserOnlyDataTable
		/// <summary>
		///		ProjectId, Title, ManagerId, StatusId, PercentCompleted, TargetStartDate, 
		///		TargetFinishDate, PriorityId, PriorityName, ProjectCode,
		///		IsManager, IsExecutiveManager, IsTeamMember, IsSponsor, IsStakeHolder
		/// </summary>
		public static DataTable GetListActiveProjectsByUserOnlyDataTable()
		{
			return DBProject.GetListActiveProjectsByUserOnlyDataTable(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}

		public static DataTable GetListActiveProjectsByUserOnlyDataTable(int UserId)
		{
			return DBProject.GetListActiveProjectsByUserOnlyDataTable(UserId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListProjectTypes
		/// <summary>
		/// Reader returns fields:
		///		TypeId, TypeName 
		/// </summary>
		public static IDataReader GetListProjectTypes()
		{
			return DBProject.GetListProjectTypes();
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
		public static IDataReader GetListCategories(int project_id)
		{
			return DBCommon.GetListCategoriesByObject((int)PROJECT_TYPE, project_id);
		}
		#endregion

		#region GetListCalendars
		/// <summary>
		/// Reader returns fields:
		///		CalendarId, ProjectId, CalendarName, TimeZoneId
		/// </summary>
		public static IDataReader GetListCalendars(int project_id)
		{
			return DBCalendar.GetListCalendarsForProject(project_id);
		}
		#endregion

		#region GetListProjectStatus
		/// <summary>
		/// Reader returns fields:
		///		StatusId, StatusName, IsActive 
		/// </summary>
		public static IDataReader GetListProjectStatus()
		{
			return DBProject.GetListProjectStatus(Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListTeamMembers
		/// <summary>
		/// Reader returns fields:
		///		MemberId, ProjectId, UserId, Code, Rate
		/// </summary>
		public static IDataReader GetListTeamMembers(int project_id)
		{
			return DBProject.GetListTeamMembers(project_id);
		}
		#endregion

		#region GetListTeamMembersDataTable
		/// <summary>
		/// Reader returns fields:
		///		MemberId, ProjectId, UserId, Code, Rate
		/// </summary>
		public static DataTable GetListTeamMembersDataTable(int project_id)
		{
			return DBProject.GetListTeamMembersDataTable(project_id);
		}
		#endregion

		#region GetListSponsors
		/// <summary>
		/// Reader returns fields:
		///		MemberId, ProjectId, PrincipalId, IsGroup
		/// </summary>
		public static IDataReader GetListSponsors(int project_id)
		{
			return DBProject.GetListSponsors(project_id);
		}
		#endregion

		#region GetListSponsorsDataTable
		/// <summary>
		/// Reader returns fields:
		///		MemberId, ProjectId, PrincipalId, IsGroup
		/// </summary>
		public static DataTable GetListSponsorsDataTable(int project_id)
		{
			return DBProject.GetListSponsorsDataTable(project_id);
		}
		#endregion

		#region GetListStakeholders
		/// <summary>
		/// Reader returns fields:
		///		MemberId, ProjectId, PrincipalId, IsGroup
		/// </summary>
		public static IDataReader GetListStakeholders(int project_id)
		{
			return DBProject.GetListStakeholders(project_id);
		}
		#endregion

		#region GetListStakeholdersDataTable
		/// <summary>
		/// Reader returns fields:
		///		MemberId, ProjectId, PrincipalId, IsGroup
		/// </summary>
		public static DataTable GetListStakeholdersDataTable(int project_id)
		{
			return DBProject.GetListStakeholdersDataTable(project_id);
		}
		#endregion

		#region GetListDiscussions
		/// <summary>
		/// Reader returns fields:
		///		DiscussionId, ObjectTypeId, ObjectId, CreatorId, CreationDate, Text, CreatorName
		/// </summary>
		public static IDataReader GetListDiscussions(int project_id)
		{
			return Common.GetListDiscussions(PROJECT_TYPE, project_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListDiscussionsDataTable
		/// <summary>
		/// Reader returns fields:
		///		DiscussionId, ObjectTypeId, ObjectId, CreatorId, CreationDate, Text, CreatorName
		/// </summary>
		public static DataTable GetListDiscussionsDataTable(int project_id)
		{
			return Common.GetListDiscussionsDataTable(PROJECT_TYPE, project_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region AddDiscussion
		public static void AddDiscussion(int project_id, string text)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			DateTime creation_date = DateTime.UtcNow;
			int UserId = Security.CurrentUser.UserID;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				int CommentId = DBCommon.AddDiscussion((int)PROJECT_TYPE, project_id, UserId, creation_date, text);

				SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_CommentList_CommentAdded, project_id, CommentId);

				tran.Commit();
			}
		}
		#endregion

		#region GetListTaskLinskByProject
		/// <summary>
		/// Reader returns fields:
		/// LinkId, PredId, SuccId, Lag 
		/// </summary>
		/// <param name="ProjectId"></param>
		/// <returns></returns>
		public static IDataReader GetListTaskLinksByProject(int project_id)
		{
			return DBTask.GetListTaskLinksByProject(project_id);
		}
		#endregion

		#region PROCESS
		public static void PROCESS()
		{
			//RecalculateAll(41);
			Task.process();
		}
		#endregion

		#region RecalculateAll
		public static void RecalculateAll(int project_id)
		{
			ArrayList Tasks = new ArrayList();
			using (IDataReader TasksReader = DBTask.GetListTasksInRecalculateOrder(project_id))
			{
				while (TasksReader.Read())
				{
					int TaskId = (int)TasksReader["TaskId"];
					Tasks.Add(TaskId);
				}
			}

			foreach (int TaskId in Tasks)
				Task.InternalUpdate2(TaskId);

			Task.RecalculateAllStates(project_id);
		}
		#endregion

		#region ShowManagedCalendar
		public static bool ShowManagedCalendar(int project_id)
		{
			return Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) ||
				GetSecurity(project_id).IsManager;
		}
		#endregion

		#region GetListTeamMemberNames
		/// <summary>
		/// Reader returns fields:
		///  UserId, FirstName, LastName, Activity
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListTeamMemberNames(int ProjectId)
		{
			return DBProject.GetListTeamMemberNames(ProjectId);
		}
		#endregion

		#region GetListTeamMemberNamesDataTable
		/// <summary>
		///		UserId, FirstName, LastName
		/// </summary>
		public static DataTable GetListTeamMemberNamesDataTable(int ProjectId)
		{
			return DBProject.GetListTeamMemberNamesDataTable(ProjectId);
		}
		#endregion

		#region GetListTeamMemberNamesWithManager
		/// <summary>
		///		UserId, FirstName, LastName, IsExternal
		/// </summary>
		public static IDataReader GetListTeamMemberNamesWithManager(int ProjectId)
		{
			return DBProject.GetListTeamMemberNamesWithManager(ProjectId);
		}
		#endregion

		#region GetListTeamMemberNamesWithManagerDataTable
		/// <summary>
		///		UserId, FirstName, LastName, IsExternal
		/// </summary>
		public static DataTable GetListTeamMemberNamesWithManagerDataTable(int ProjectId)
		{
			return DBProject.GetListTeamMemberNamesWithManagerDataTable(ProjectId);
		}
		#endregion

		#region GetProjectManager
		public static int GetProjectManager(int ProjectId)
		{
			return DBProject.GetProjectManager(ProjectId);
		}
		#endregion

		#region UpdateClient
		//		public static void UpdateClient(int ClientId, string ClientName)
		//		{
		//			DBProject.UpdateClient(ClientId, ClientName);
		//		}
		#endregion

		#region DeleteClient
		//		public static void DeleteClient(int ClientId)
		//		{
		//			DBProject.DeleteClient(ClientId);
		//		}
		#endregion

		#region GetListClients
		//		/// <summary>
		//		/// Reader returns fields:
		//		///  ClientId, ClientName, IsInternal, Created 
		//		/// </summary>
		//		/// <returns></returns>
		//		public static IDataReader GetListClients()
		//		{
		//			return GetListClients(0);;
		//		}
		//		public static IDataReader GetListClients(int ClientId)
		//		{
		//			return DBProject.GetListClients(ClientId, Security.CurrentUser.TimeZoneId);
		//		}
		//
		//		public static DataTable GetListClientsDT()
		//		{
		//			return GetListClientsDT(0);
		//		}
		//		public static DataTable GetListClientsDT(int ClientId)
		//		{
		//			return DBProject.GetListClientsDT(ClientId, Security.CurrentUser.TimeZoneId);
		//		}
		#endregion

		#region GetListProjectsUpdatedByUser
		/// <summary>
		///		ProjectId, Title, LastSavedDate
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListProjectsUpdatedByUser(int Days)
		{
			return DBProject.GetListProjectsUpdatedByUser(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}
		#endregion

		#region GetListProjectsUpdatedByUserDataTable
		/// <summary>
		///		ProjectId, Title, StatusId, LastSavedDate
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectsUpdatedByUserDataTable(int Days)
		{
			return DBProject.GetListProjectsUpdatedByUserDataTable(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}
		#endregion

		#region GetListProjectsUpdatedForUser
		/// <summary>
		///		ProjectId, Title, LastEditorId, LastSavedDate
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListProjectsUpdatedForUser(int Days)
		{
			return DBProject.GetListProjectsUpdatedForUser(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}
		#endregion

		#region GetListProjectsUpdatedForUserDataTable
		/// <summary>
		///		ProjectId, Title, StatusId, LastEditorId, LastSavedDate
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectsUpdatedForUserDataTable(int Days)
		{
			return DBProject.GetListProjectsUpdatedForUserDataTable(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}
		#endregion

		#region GetListProjectsByKeyword
		/// <summary>
		/// Reader returns fields:
		///	 ProjectId, ManagerId, Title, Description, StatusId, PercentCompleted, 
		///	 StartDate, FinishDate, TargetStartDate, TargetFinishDate, 
		///	 ActualStartDate, ActualFinishDate, ProjectCode
		/// </summary>
		public static IDataReader GetListProjectsByKeyword(string Keyword)
		{
			return DBProject.GetListProjectsByKeyword(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Keyword);
		}

		public static DataTable GetListProjectsByKeywordDataTable(string Keyword)
		{
			return DBProject.GetListProjectsByKeywordDataTable(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Keyword);
		}
		#endregion

		#region GetListProjectCategories
		/// <summary>
		/// Reader returns fields:
		///  CategoryId, CategoryName 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListProjectCategories()
		{
			return DBProject.GetListProjectCategories(0);
		}
		#endregion

		#region GetListProjectCategoriesByProject
		/// <summary>
		/// Reader returns fields:
		///  CategoryId, CategoryName 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListProjectCategoriesByProject(int ProjectId)
		{
			return DBProject.GetListProjectCategoriesByProject(ProjectId);
		}
		#endregion

		#region GetListProjectsForChangeableRoles
		/// <summary>
		/// Reader returns fields:
		///  ProjectId, Title, StatusId, IsManager, IsExecutiveManagerId, IsTeamMember, IsSponsor, IsStakeHolder, CanView, CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListProjectsForChangeableRoles(int UserId)
		{
			return DBProject.GetListProjectsForChangeableRoles(UserId, Security.CurrentUser.UserID);
		}
		#endregion

		#region UploadImportXML
		public static int UploadImportXML(int ProjectId, string FileName,
										  System.IO.Stream _inputStream)
		{
			int OldXMLFileId = -1;
			int XMLFileId = -1;
			using (IDataReader reader = DBProject.GetProject(ProjectId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (reader.Read())
					OldXMLFileId = (int)reader["XMLFileId"];
			}

			using (DbTransaction tran = DbTransaction.Begin())
			{
				string ContainerName = "TemporaryStorage";
				string ContainerKey = "TmpProjectId_" + ProjectId.ToString();

				BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
				Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");

				if (OldXMLFileId > 0)
				{
					FileInfo fi = fs.GetFile(OldXMLFileId);
					if (fi != null)
						fs.DeleteFile(OldXMLFileId);
				}

				FileInfo file = fs.SaveFile(FileName, _inputStream);
				XMLFileId = file.Id;

				//DBProject.UpdateXMLFileId(ProjectId, XMLFileId);

				tran.Commit();
			}
			return XMLFileId;

		}
		#endregion

		#region Class Prj
		internal class Prj
		{
			public int ProjectId;
			public string Title;
			public int ManagerId;
			public string ManagerName;
			public int StatusId;
			public string StatusName;
			public int TypeId;
			public string TypeName;
			public int Budget;
			public Hashtable GeneralCategories;
			public Hashtable ProjectCategories;

			internal Prj()
			{
			}
		}
		#endregion

		#region GetListProjectsByListId
		/// <summary>
		/// </summary>
		/// <returns></returns>
		public static ArrayList GetListProjectsByListId(ArrayList alPrjs)
		{
			ArrayList _List = new ArrayList();
			foreach (int iPrjId in alPrjs)
			{
				Prj _p = new Prj();
				using (IDataReader reader = Project.GetProject(iPrjId))
				{
					if (reader.Read())
					{
						_p.ProjectId = iPrjId;
						_p.Title = reader["Title"].ToString();
						_p.ManagerId = (int)reader["ManagerId"];
						_p.StatusId = (int)reader["StatusId"];
						_p.StatusName = reader["StatusName"].ToString();
						_p.TypeId = (int)reader["TypeId"];
						_p.TypeName = reader["TypeName"].ToString();
						_p.Budget = 0;
					}
				}
				using (IDataReader reader = User.GetUserInfo(_p.ManagerId))
				{
					if (reader.Read())
						_p.ManagerName = reader["FirstName"].ToString() + " " + reader["LastName"].ToString();
				}
				Hashtable htGenCats = new Hashtable();
				using (IDataReader reader = Project.GetListCategories(iPrjId))
				{
					while (reader.Read())
						htGenCats.Add((int)reader["CategoryId"], reader["CategoryName"].ToString());
				}
				_p.GeneralCategories = htGenCats;

				Hashtable htPrjCats = new Hashtable();
				using (IDataReader reader = Project.GetListProjectCategoriesByProject(iPrjId))
				{
					while (reader.Read())
						htPrjCats.Add((int)reader["CategoryId"], reader["CategoryName"].ToString());
				}
				_p.ProjectCategories = htPrjCats;
				_List.Add(_p);
			}
			return _List;
		}
		#endregion

		#region GetProjectStatistic
		/// <summary>
		/// Reader returns fields:
		///  ProjectCount
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetProjectStatistic()
		{
			return DBProject.GetProjectStatistic();
		}
		#endregion

		#region GetProjectStatisticByStatusDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByStatusDataTable()
		{
			return DBProject.GetProjectStatisticByStatusDataTable(Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetProjectStatisticByStatusDataTable - By Group of Projects
		/// <summary>
		///  StatusId, StatusName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByStatusDataTable(ArrayList alPrjs)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("StatusId", typeof(int));
			dt.Columns.Add("StatusName", typeof(string));
			dt.Columns.Add("Count", typeof(int));
			Hashtable htStatusCount = new Hashtable();
			Hashtable htStatusName = new Hashtable();
			foreach (Prj _p in alPrjs)
			{
				int iStatusId = _p.StatusId;
				string sStatusName = _p.StatusName;
				if (!htStatusCount.Contains(iStatusId))
					htStatusCount.Add(iStatusId, 1);
				else
					htStatusCount[iStatusId] = (int)htStatusCount[iStatusId] + 1;
				if (!htStatusName.Contains(iStatusId))
					htStatusName.Add(iStatusId, sStatusName);

			}
			foreach (int iStatusId in htStatusName.Keys)
			{
				DataRow dr;
				dr = dt.NewRow();
				dr["StatusId"] = iStatusId;
				dr["StatusName"] = htStatusName[iStatusId].ToString();
				dr["Count"] = (int)htStatusCount[iStatusId];
				dt.Rows.Add(dr);
			}
			return dt;
		}
		#endregion

		#region GetProjectStatisticByManagerDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByManagerDataTable()
		{
			return DBProject.GetProjectStatisticByManagerDataTable();
		}
		#endregion

		#region GetProjectStatisticByManagerDataTable - By Group of Projects
		/// <summary>
		///  ManagerId, ManagerName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByManagerDataTable(ArrayList alPrjs)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("ManagerId", typeof(int));
			dt.Columns.Add("ManagerName", typeof(string));
			dt.Columns.Add("Count", typeof(int));
			Hashtable htManagerCount = new Hashtable();
			Hashtable htManagerName = new Hashtable();
			foreach (Prj _p in alPrjs)
			{
				int iManagerId = _p.ManagerId;
				string sManagerName = _p.ManagerName;
				if (!htManagerCount.Contains(iManagerId))
					htManagerCount.Add(iManagerId, 1);
				else
					htManagerCount[iManagerId] = (int)htManagerCount[iManagerId] + 1;
				if (!htManagerName.Contains(iManagerId))
					htManagerName.Add(iManagerId, sManagerName);
			}
			foreach (int iManagerId in htManagerName.Keys)
			{
				DataRow dr;
				dr = dt.NewRow();
				dr["ManagerId"] = iManagerId;
				dr["ManagerName"] = htManagerName[iManagerId].ToString();
				dr["Count"] = (int)htManagerCount[iManagerId];
				dt.Rows.Add(dr);
			}
			return dt;
		}
		#endregion

		#region GetProjectStatisticByTypeDataTable - By Group of Projects
		/// <summary>
		///  TypeId, TypeName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByTypeDataTable(ArrayList alPrjs)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("TypeId", typeof(int));
			dt.Columns.Add("TypeName", typeof(string));
			dt.Columns.Add("Count", typeof(int));
			Hashtable htTypeCount = new Hashtable();
			Hashtable htTypeName = new Hashtable();
			foreach (Prj _p in alPrjs)
			{
				int iTypeId = _p.TypeId;
				string sTypeName = _p.TypeName;
				if (!htTypeCount.Contains(iTypeId))
					htTypeCount.Add(iTypeId, 1);
				else
					htTypeCount[iTypeId] = (int)htTypeCount[iTypeId] + 1;
				if (!htTypeName.Contains(iTypeId))
					htTypeName.Add(iTypeId, sTypeName);

			}
			foreach (int iTypeId in htTypeName.Keys)
			{
				DataRow dr;
				dr = dt.NewRow();
				dr["TypeId"] = iTypeId;
				dr["TypeName"] = htTypeName[iTypeId].ToString();
				dr["Count"] = (int)htTypeCount[iTypeId];
				dt.Rows.Add(dr);
			}
			return dt;
		}
		#endregion

		#region GetProjectStatisticByTypeDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByTypeDataTable()
		{
			return DBProject.GetProjectStatisticByTypeDataTable();
		}
		#endregion

		#region GetProjectStatisticByProjectCategoryDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByProjectCategoryDataTable()
		{
			return DBProject.GetProjectStatisticByProjectCategoryDataTable();
		}
		#endregion

		#region GetProjectStatisticByProjectCategoryDataTable - By Group of Projects
		/// <summary>
		///  CategoryId, CategoryName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByProjectCategoryDataTable(ArrayList alPrjs)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("CategoryId", typeof(int));
			dt.Columns.Add("CategoryName", typeof(string));
			dt.Columns.Add("Count", typeof(int));
			Hashtable htCategoryCount = new Hashtable();
			Hashtable htCategoryName = new Hashtable();
			foreach (Prj _p in alPrjs)
			{
				Hashtable htPrjCats = _p.ProjectCategories;
				foreach (int iCategoryId in htPrjCats.Keys)
				{
					string sCategoryName = htPrjCats[iCategoryId].ToString();
					if (!htCategoryCount.Contains(iCategoryId))
						htCategoryCount.Add(iCategoryId, 1);
					else
						htCategoryCount[iCategoryId] = (int)htCategoryCount[iCategoryId] + 1;
					if (!htCategoryName.Contains(iCategoryId))
						htCategoryName.Add(iCategoryId, sCategoryName);
				}
			}
			foreach (int iCategoryId in htCategoryName.Keys)
			{
				DataRow dr;
				dr = dt.NewRow();
				dr["CategoryId"] = iCategoryId;
				dr["CategoryName"] = htCategoryName[iCategoryId].ToString();
				dr["Count"] = (int)htCategoryCount[iCategoryId];
				dt.Rows.Add(dr);
			}
			return dt;
		}
		#endregion

		#region GetProjectStatisticByGeneralCategoryDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByGeneralCategoryDataTable()
		{
			return DBProject.GetProjectStatisticByGeneralCategoryDataTable();
		}
		#endregion

		#region GetProjectStatisticByGeneralCategoryDataTable - By Group of Projects
		/// <summary>
		///  CategoryId, CategoryName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByGeneralCategoryDataTable(ArrayList alPrjs)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("CategoryId", typeof(int));
			dt.Columns.Add("CategoryName", typeof(string));
			dt.Columns.Add("Count", typeof(int));
			Hashtable htCategoryCount = new Hashtable();
			Hashtable htCategoryName = new Hashtable();
			foreach (Prj _p in alPrjs)
			{
				Hashtable htGenCats = _p.GeneralCategories;
				foreach (int iCategoryId in htGenCats.Keys)
				{
					string sCategoryName = htGenCats[iCategoryId].ToString();
					if (!htCategoryCount.Contains(iCategoryId))
						htCategoryCount.Add(iCategoryId, 1);
					else
						htCategoryCount[iCategoryId] = (int)htCategoryCount[iCategoryId] + 1;
					if (!htCategoryName.Contains(iCategoryId))
						htCategoryName.Add(iCategoryId, sCategoryName);
				}
			}
			foreach (int iCategoryId in htCategoryName.Keys)
			{
				DataRow dr;
				dr = dt.NewRow();
				dr["CategoryId"] = iCategoryId;
				dr["CategoryName"] = htCategoryName[iCategoryId].ToString();
				dr["Count"] = (int)htCategoryCount[iCategoryId];
				dt.Rows.Add(dr);
			}
			return dt;
		}
		#endregion

		#region GetProjectStatisticByProjectGroupDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByProjectGroupDataTable()
		{
			return DBProject.GetProjectStatisticByProjectGroupDataTable();
		}
		#endregion

		#region GetProjectStatisticByPhaseDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByPhaseDataTable()
		{
			return DBProject.GetProjectStatisticByPhaseDataTable();
		}
		#endregion

		#region GetProjectStatisticByPriorityDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByPriorityDataTable()
		{
			return DBProject.GetProjectStatisticByPriorityDataTable(Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetProjectStatisticByClientDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByClientDataTable()
		{
			return DBProject.GetProjectStatisticByClientDataTable();
		}
		#endregion


		#region GetListCurrency
		/// <summary>
		/// Reader returns fields:
		///		CurrencyId, CurrencySymbol
		/// </summary>
		public static IDataReader GetListCurrency()
		{
			return DBCommon.GetListCurrency(0);
		}
		#endregion

		#region GetCurrencySymbol
		public static string GetCurrencySymbol(int ProjectId)
		{
			return DBProject.GetCurrencySymbol(ProjectId);
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

		#region GetListProjectCategoriesByUser
		/// <summary>
		/// Reader returns fields:
		///		CategoryId, UserId
		/// </summary>
		public static IDataReader GetListProjectCategoriesByUser()
		{
			return DBProject.GetListProjectCategoriesByUser(Security.CurrentUser.UserID);
		}
		#endregion

		#region SaveGeneralCategories
		public static void SaveGeneralCategories(ArrayList general_categories)
		{
			if (general_categories == null)
				general_categories = new ArrayList();

			int UserId = Security.CurrentUser.UserID;

			// General Categories
			ArrayList NewCategories = new ArrayList(general_categories);
			ArrayList DeletedCategories = new ArrayList();
			using (IDataReader reader = DBCommon.GetListCategoriesByUser(UserId))
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

			using (DbTransaction tran = DbTransaction.Begin())
			{
				foreach (int CategoryId in DeletedCategories)
					DBCommon.DeleteCategoryUser(CategoryId, UserId);
				foreach (int CategoryId in NewCategories)
					DBCommon.AddCategoryUser(CategoryId, UserId);

				tran.Commit();
			}
		}
		#endregion

		#region SaveProjectCategories
		public static void SaveProjectCategories(ArrayList project_categories)
		{
			if (project_categories == null)
				project_categories = new ArrayList();

			int UserId = Security.CurrentUser.UserID;

			// Project Categories
			ArrayList NewProjectCategories = new ArrayList(project_categories);
			ArrayList DeletedProjectCategories = new ArrayList();
			using (IDataReader reader = DBProject.GetListProjectCategoriesByUser(UserId))
			{
				while (reader.Read())
				{
					int CategoryId = (int)reader["CategoryId"];
					if (NewProjectCategories.Contains(CategoryId))
						NewProjectCategories.Remove(CategoryId);
					else
						DeletedProjectCategories.Add(CategoryId);
				}
			}

			using (DbTransaction tran = DbTransaction.Begin())
			{
				foreach (int CategoryId in DeletedProjectCategories)
					DBProject.DeleteProjectCategoryUser(CategoryId, UserId);
				foreach (int CategoryId in NewProjectCategories)
					DBProject.AddProjectCategoryUser(CategoryId, UserId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateDiscussion
		public static void UpdateDiscussion(int DiscussionId, string Text)
		{
			// DiscussionId, ObjectTypeId, ObjectId, CreatorId, CreationDate, Text, CreatorName
			int ObjectTypeId = (int)ObjectTypes.UNDEFINED;
			int ObjectId = -1;
			using (IDataReader reader = DBCommon.GetDiscussion(DiscussionId, Security.CurrentUser.TimeZoneId))
			{
				if (reader.Read())
				{
					if ((int)reader["CreatorId"] != Security.CurrentUser.UserID)
						throw new AccessDeniedException();

					ObjectTypeId = (int)reader["ObjectTypeId"];
					ObjectId = (int)reader["ObjectId"];
				}
			}

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBCommon.UpdateDiscussion(DiscussionId, Text);

				if (ObjectTypeId == (int)ObjectTypes.Project)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_CommentList_CommentUpdated, ObjectId, DiscussionId);
				else if (ObjectTypeId == (int)ObjectTypes.Task)
					SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_CommentList_CommentUpdated, ObjectId, DiscussionId);
				else if (ObjectTypeId == (int)ObjectTypes.ToDo)
					SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_CommentList_CommentUpdated, ObjectId, DiscussionId);
				else if (ObjectTypeId == (int)ObjectTypes.Document)
					SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_CommentList_CommentUpdated, ObjectId, DiscussionId);
				else if (ObjectTypeId == (int)ObjectTypes.Issue)
					SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_CommentList_CommentUpdated, ObjectId, DiscussionId);
				else if (ObjectTypeId == (int)ObjectTypes.CalendarEntry)
					SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_CommentList_CommentUpdated, ObjectId, DiscussionId);

				tran.Commit();
			}
		}
		#endregion

		#region DeleteDiscussion
		public static void DeleteDiscussion(int discussionId)
		{
			int objectTypeId = (int)ObjectTypes.UNDEFINED;
			int objectId = -1;
			using (IDataReader reader = DBCommon.GetDiscussion(discussionId, Security.CurrentUser.TimeZoneId))
			{
				if (reader.Read())
				{
					if ((int)reader["CreatorId"] != Security.CurrentUser.UserID)
						throw new AccessDeniedException();

					objectTypeId = (int)reader["ObjectTypeId"];
					objectId = (int)reader["ObjectId"];
				}
			}
			using (DbTransaction tran = DbTransaction.Begin())
			{
				if (objectTypeId == (int)ObjectTypes.Project)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_CommentList_CommentDeleted, objectId, discussionId);
				else if (objectTypeId == (int)ObjectTypes.Task)
					SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_CommentList_CommentDeleted, objectId, discussionId);
				else if (objectTypeId == (int)ObjectTypes.ToDo)
					SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_CommentList_CommentDeleted, objectId, discussionId);
				else if (objectTypeId == (int)ObjectTypes.Document)
					SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_CommentList_CommentDeleted, objectId, discussionId);
				else if (objectTypeId == (int)ObjectTypes.Issue)
					SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_CommentList_CommentDeleted, objectId, discussionId);
				else if (objectTypeId == (int)ObjectTypes.CalendarEntry)
					SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_CommentList_CommentDeleted, objectId, discussionId);

				DBCommon.DeleteDiscussion(discussionId);

				tran.Commit();
			}
		}
		#endregion

		#region GetDiscussion
		/// <summary>
		/// Reader returns fields:
		///		DiscussionId, ObjectTypeId, ObjectId, CreatorId, CreationDate, Text, CreatorName
		/// </summary>
		public static IDataReader GetDiscussion(int DiscussionId)
		{
			return DBCommon.GetDiscussion(DiscussionId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListCategoriesStatisticForPMAndPPMDataTable
		/// <summary>
		///		CategoryId, CategoryName, Count
		/// </summary>
		public static DataTable GetListCategoriesStatisticForPMAndPPMDataTable()
		{
			return DBProject.GetListCategoriesStatisticForPMAndPPMDataTable(Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListProjectCategoriesStatisticForPMAndPPMDataTable
		/// <summary>
		///		CategoryId, CategoryName, Count
		/// </summary>
		public static DataTable GetListProjectCategoriesStatisticForPMAndPPMDataTable()
		{
			return DBProject.GetListProjectCategoriesStatisticForPMAndPPMDataTable(Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListTaskResources
		/// <summary>
		///	 ResourceId, UserId, FirstName, LastName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListTaskResources(int ProjectId)
		{
			return DBProject.GetListTaskResources(ProjectId);
		}
		#endregion

		#region GetListTaskResourcesDataTable
		/// <summary>
		///	 ResourceId, UserId, FirstName, LastName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListTaskResourcesDataTable(int ProjectId)
		{
			return DBProject.GetListTaskResourcesDataTable(ProjectId);
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

		#region GetListProjectRelationsDataTable
		/// <summary>
		///	 ProjectId, Title, ManagerId, CanView, CanViewSnapshot
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectRelationsDataTable(int ProjectId)
		{
			return DBProject.GetListProjectRelationsDataTable(ProjectId, Security.CurrentUser.UserID);
		}
		#endregion


		#region SaveProjectGroups
		public static void SaveProjectGroups(ArrayList project_groups)
		{
			if (project_groups == null)
				project_groups = new ArrayList();

			int UserId = Security.CurrentUser.UserID;

			// Project Categories
			ArrayList NewProjectGroups = new ArrayList(project_groups);
			ArrayList DeletedProjectGroups = new ArrayList();
			using (IDataReader reader = DBProject.GetListProjectGroupsByUser(UserId))
			{
				while (reader.Read())
				{
					int ProjectGroupId = (int)reader["ProjectGroupId"];
					if (NewProjectGroups.Contains(ProjectGroupId))
						NewProjectGroups.Remove(ProjectGroupId);
					else
						DeletedProjectGroups.Add(ProjectGroupId);
				}
			}

			using (DbTransaction tran = DbTransaction.Begin())
			{
				foreach (int ProjectGroupId in DeletedProjectGroups)
					DBProject.DeleteProjectGroupUser(ProjectGroupId, UserId);
				foreach (int ProjectGroupId in NewProjectGroups)
					DBProject.AddProjectGroupUser(ProjectGroupId, UserId);

				tran.Commit();
			}
		}
		#endregion

		#region GetListProjectGroupsByUser
		/// <summary>
		/// Reader returns fields:
		///		ProjectGroupId, UserId
		/// </summary>
		public static IDataReader GetListProjectGroupsByUser()
		{
			return DBProject.GetListProjectGroupsByUser(Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListProjectPhases
		/// <summary>
		///  PhaseId, PhaseName, Weight
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListProjectPhases()
		{
			return DBProject.GetListProjectPhases(0);
		}
		#endregion

		#region AddProjectSnapshot
		public static void AddProjectSnapshot(int ProjectId)
		{
			if (!CanViewFinances(ProjectId))
				throw new AccessDeniedException();

			DBProject.AddProjectSnapshot(ProjectId, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListProjectSnapshots
		/// <summary>
		///  SnapshotId, ProjectId, StatusId, StatusName, TargetBudget, EstimatedBudget, 
		///  ActualBudget, StartDate, FinishDate, TargetFinishDate, 
		///  CreatorId, CreationDate, PhaseId, PhaseName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectSnapshots(int ProjectId)
		{
			return DBProject.GetListProjectSnapshots(ProjectId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region DeleteProjectSnapshot
		public static void DeleteProjectSnapshot(int ProjectId)
		{
			if (!CanViewFinances(ProjectId))
				throw new AccessDeniedException();

			DBProject.DeleteProjectSnapshot(ProjectId);
		}
		#endregion

		#region GetListProjectGroupedByPortfolio
		/// <summary>
		///  PortfolioId, PortfolioName, ProjectId, ProjectName, StatusName, 
		///  Resources, TasksTodos, Issues, PhaseName, 
		///  ProjectsCount, ActiveProjectsCount, IsHeader
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectGroupedByPortfolio(int PortfolioId, int PhaseId, int StatusId)
		{
			return DBProject.GetListProjectsGroupedByPortfolio(PortfolioId, PhaseId, StatusId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListProjectsGroupedByPhase
		/// <summary>
		///  PhaseId, PhaseName, Weight, ProjectId, ProjectName, StatusName, 
		///  Resources, TasksTodos, Issues, ProjectsCount, ActiveProjectsCount, IsHeader
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectsGroupedByPhase(int PortfolioId, int PhaseId, int StatusId)
		{
			return DBProject.GetListProjectsGroupedByPhase(PortfolioId, PhaseId, StatusId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListProjectsGroupedByManager
		/// <summary>
		///  ManagerId, ManagerName, ProjectId, ProjectName, StatusName, 
		///  OpenTasks, CompletedTasks, Issues, IsHeader
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectsGroupedByManager(int PortfolioId, int PhaseId, int StatusId, int ManagerId)
		{
			return DBProject.GetListProjectsGroupedByManager(PortfolioId, PhaseId, StatusId, ManagerId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListProjectsGroupedByClient
		/// <summary>
		///  ContactUid, OrgUid, IsClient, ClientName, ProjectId, ProjectName, StatusName, 
		///  OpenTasks, CompletedTasks, Issues, IsCollapsed
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectsGroupedByClient(int portfolioId,
			int phaseId, int statusId, int managerId, PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			return DBProject.GetListProjectsGroupedByClient(portfolioId, phaseId,
				statusId, managerId, Security.CurrentUser.UserID,
				Security.CurrentUser.LanguageId, orgUid, contactUid);
		}
		#endregion

		#region CollapseByClient
		public static void CollapseByClient(PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			DBProject.CollapseByClient(Security.CurrentUser.UserID, contactUid, orgUid);
		}
		#endregion

		#region ExpandByClient
		public static void ExpandByClient(PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			DBProject.ExpandByClient(Security.CurrentUser.UserID, contactUid, orgUid);
		}
		#endregion

		#region GetListProjectsByUsersInGroup
		/// <summary>
		///		UserId, UserName, ProjectId, Title, PriorityId, PriorityName, 
		///		StatusId, StatusName, PhaseId, PhaseName, IsTeamMember,
		///		IsSponsor, IsStakeHolder, IsManager, IsExecutiveManager, IsGroup
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectsByUsersInGroup(int GroupId)
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) &&
				!Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager))
				throw new AccessDeniedException();

			return DBProject.GetListProjectsByUsersInGroup(GroupId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetProjectMetrics
		/// <summary>
		/// ActiveProjects, OnHoldProjects, CompletedProjects,ProjectIncidents, 
		/// TargetBudget, EstimatedBudget, 
		/// ActualBudget, ActiveTasks, OpenIssues
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetProjectMetrics()
		{
			if (!(Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) || Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager)))
				throw new AccessDeniedException();

			return DBProject.GetProjectMetrics();
		}
		#endregion

		#region IsMSProjectIntegrationEnabled
		public static bool IsMSProjectIntegrationEnabled()
		{
			return License.MsProjectIntegration;
		}
		#endregion

		#region IsMSProjectSynchronizationEnabled
		public static bool IsMSProjectSynchronizationEnabled()
		{
			return License.MSProjectSynchronization;
		}
		#endregion

		#region IsWebGanttChartEnabled
		public static bool IsWebGanttChartEnabled()
		{
			return License.WebGanttChart;
		}
		#endregion

		#region GetProjectTitle
		public static string GetProjectTitle(int ProjectId)
		{
			string ProjectTitle = "";
			using (IDataReader reader = GetProject(ProjectId, false))
			{
				if (reader.Read())
					ProjectTitle = reader["Title"].ToString();
			}
			return ProjectTitle;
		}
		#endregion

		#region AddFavorites
		public static void AddFavorites(int ProjectId)
		{
			DBCommon.AddFavorites((int)PROJECT_TYPE, ProjectId, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListFavorites
		/// <summary>
		///		FavoriteId, ObjectTypeId, ObjectId, UserId, Title 
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListFavoritesDT()
		{
			return DBCommon.GetListFavoritesDT((int)PROJECT_TYPE, Security.CurrentUser.UserID);
		}
		#endregion

		#region CheckFavorites
		public static bool CheckFavorites(int ProjectId)
		{
			return DBCommon.CheckFavorites((int)PROJECT_TYPE, ProjectId, Security.CurrentUser.UserID);
		}
		#endregion

		#region DeleteFavorites
		public static void DeleteFavorites(int ProjectId)
		{
			DBCommon.DeleteFavorites((int)PROJECT_TYPE, ProjectId, Security.CurrentUser.UserID);
		}
		#endregion

		#region AddHistory
		public static void AddHistory(int ProjectId, string Title)
		{
			DBCommon.AddHistory((int)PROJECT_TYPE, ProjectId, Title, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListRiskLevels
		/// <summary>
		///		RiskLevelId, RiskLevelName, Weight
		/// </summary>
		public static IDataReader GetListRiskLevels()
		{
			return DBCommon.GetListRiskLevels();
		}
		#endregion

		#region GetFileCountByProject
		public static int GetFileCountByProject(int ProjectId)
		{
			return DBProject.GetFileCountByProject(ProjectId);
		}
		#endregion

		#region GetFileCountByProjectAll
		// с учётом фалов в ToDo, Tasks, Documents, Incidents, Events
		public static int GetFileCountByProjectAll(int ProjectId)
		{
			return DBProject.GetFileCountByProjectAll(ProjectId);
		}
		#endregion

		#region GetListActiveProjectsByManager
		/// <summary>
		/// ProjectId, Title
		/// </summary>
		public static IDataReader GetListActiveProjectsByManager()
		{
			return DBProject.GetListActiveProjectsByManager(Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListProjectManagers
		/// <summary>
		/// Reader returns fields:
		///		ManagerId, UserName
		/// </summary>
		public static IDataReader GetListProjectManagers()
		{
			return DBProject.GetListProjectManagers();
		}
		#endregion

		#region GetListExecutiveManagers
		/// <summary>
		/// Reader returns fields:
		///		ExecutiveManagerId, UserName
		/// </summary>
		public static IDataReader GetListExecutiveManagers()
		{
			return DBProject.GetListExecutiveManagers();
		}
		#endregion

		#region ConfirmReminder(...)
		internal static bool ConfirmReminder(DateTypes dateType, int objectId, bool hasRecurrence)
		{
			int stateId = DBProject.GetStatus(objectId);
			bool retval = Common.ConfirmReminder(
				hasRecurrence
				, (int)dateType
				, stateId
				, (int)DateTypes.Project_StartDate
				, (int)DateTypes.Project_FinishDate
				, (int)ProjectStatus.Pending
				, (int)ProjectStatus.Completed
				, (int)ProjectStatus.OnHold
				);

			// O.R. [2009-12-14]: Check Cancelled State
			if (retval)
				retval = Common.ConfirmReminder(
				hasRecurrence
				, (int)dateType
				, stateId
				, (int)DateTypes.Project_StartDate
				, (int)DateTypes.Project_FinishDate
				, (int)ProjectStatus.Pending
				, (int)ProjectStatus.Cancelled
				, (int)ProjectStatus.OnHold
				);

			return retval;
		}
		#endregion

		#region GetProjectMetaClass
		public static Mediachase.Ibn.Data.Meta.Management.MetaClass GetProjectMetaClass()
		{
			return Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName("Project");
		}
		#endregion

		#region GetIsMSProject
		public static bool GetIsMSProject(int projectId)
		{
			bool isMSProject = false;
			using (IDataReader reader = GetProject(projectId, false))
			{
				if (reader.Read())
					isMSProject = (bool)reader["IsMSProject"];
			}
			return isMSProject;
		}
		#endregion

		#region UpdateIsMSProject
		public static void UpdateIsMSProject(int projectId)
		{
			DBProject.UpdateIsMSProject(projectId, true);
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the properties.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <returns>Dictionary<string, string>.</returns>
		public static Dictionary<string, string> GetProperties(int projectId)
		{
			Dictionary<string, string> properties = new Dictionary<string, string>();
			using (IDataReader reader = DBProject.GetProperties(projectId))
			{
				while (reader.Read())
					properties.Add(reader["Key"].ToString(), reader["Value"].ToString());
			}
			return properties;
		}

		/// <summary>
		/// Sets the property.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <param name="Key">The key.</param>
		/// <param name="Value">The value.</param>
		public static void SetProperty(int projectId, string key, string value)
		{
			DBProject.SetProperty(projectId, key, value);
		}

		/// <summary>
		/// Removes the property.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <param name="key">The key.</param>
		public static void RemoveProperty(int projectId, string key)
		{
			DBProject.RemoveProperty(projectId, key);
		}

		/// <summary>
		/// Sets the security properties.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <param name="properties">The properties.</param>
		public static void SetSecurityProperties(int projectId, Dictionary<string, bool> properties)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				foreach (KeyValuePair<string, bool> kvp in properties)
				{
					if (kvp.Value)
						SetProperty(projectId, kvp.Key, "1");
					else
						RemoveProperty(projectId, kvp.Key);
				}

				tran.Commit();
			}
		}
		#endregion

		#region GetTeamMemberRate
		/// <summary>
		/// Gets the team member rate.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public static decimal GetTeamMemberRate(int projectId, int userId)
		{
			return DBProject.GetTeamMemberRate(projectId, userId);
		}
		#endregion

		#region GetListContactsForProjects
		/// <summary>
		/// Reader returns fields:
		///		ObjectId, ObjectTypeId, ClientName, ProjectCount
		/// </summary>
		public static IDataReader GetListContactsForProjects()
		{
			return DBProject.GetListContactsForProjects(Security.CurrentUser.UserID);
		}

		public static DataTable GetListContactsForProjectsDataTable()
		{
			return DBProject.GetListContactsForProjectsDataTable(Security.CurrentUser.UserID);
		}

		public static DataTable GetListContactsForProjectsDataTable(string keyword, int count)
		{
			DataTable dt = DBProject.GetListContactsForProjectsDataTable(Security.CurrentUser.UserID);
			DataTable dtClone = dt.Clone();
			DataView dv = dt.DefaultView;
			if (!String.IsNullOrEmpty(keyword))
				dv.RowFilter = String.Format("ClientName LIKE '%{0}%'", keyword);
			dv.Sort = "ProjectCount DESC";
			int index = 0;
			DataRow dr;
			foreach (DataRowView drv in dv)
			{
				dr = dtClone.NewRow();
				dr.ItemArray = drv.Row.ItemArray;
				dtClone.Rows.Add(dr);
				index++;
				if (count > 0 && index >= count)
					break;
			}
			return dtClone;
		}
		#endregion

		#region GetProjectStatusName
		/// <summary>
		/// Reader returns fields:
		///		StatusName
		/// </summary>
		public static string GetProjectStatusName(int StateId)
		{
			string retVal = String.Empty;
			using (IDataReader reader = DBProject.GetListProjectStatus(Security.CurrentUser.LanguageId))
			{
				while (reader.Read())
					if ((int)reader["StatusId"] == StateId)
					{
						retVal = reader["StatusName"].ToString();
						break;
					}
			}
			return retVal;
		}
		#endregion

		#region GetProjectPhaseName
		/// <summary>
		/// Reader returns fields:
		///		PhaseName 
		/// </summary>
		public static string GetProjectPhaseName(int PhaseId)
		{
			string retVal = String.Empty;
			using (IDataReader reader = DBProject.GetListProjectPhases(PhaseId))
			{
				if (reader.Read())
					retVal = reader["PhaseName"].ToString();
			}
			return retVal;
		}
		#endregion

		#region GetProjectTypeName
		/// <summary>
		/// Reader returns fields:
		///		TypeName 
		/// </summary>
		public static string GetProjectTypeName(int TypeId)
		{
			string retVal = String.Empty;
			using (IDataReader reader = DBProject.GetListProjectTypes())
			{
				while (reader.Read())
					if ((int)reader["TypeId"] == TypeId)
					{
						retVal = reader["TypeName"].ToString();
						break;
					}
			}
			return retVal;
		}
		#endregion

		#region RecalculateDates
		/// <summary>
		/// Recalculates the calculated project dates.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		public static void RecalculateDates(int projectId)
		{
			DateTime calculatedStartDate = DateTime.MaxValue;
			DateTime calculatedFinishDate = DateTime.MinValue;

			// Tasks
			DateTime tasksStartDate = DBTask.GetMinStartDate(projectId);
			if (tasksStartDate != DateTime.MinValue)
				calculatedStartDate = tasksStartDate;

			DateTime tasksFinishDate = DBTask.GetMaxFinishDate(projectId);
			if (tasksFinishDate != DateTime.MinValue)
				calculatedFinishDate = tasksStartDate;

			// Incidents
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
		}
		#endregion
	}
}
