using System;
using System.Data;
using System.Data.SqlClient;
using Mediachase.IBN.Database.ControlSystem;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBProject.
	/// </summary>
	public class DBProject
	{
		#region Create
		public static int Create(
			string Title, string Description, 
			string Goals, string Scope, string Deliverables,
			int CreatorId, int ManagerId, object ExecutiveManagerId,
			DateTime CreationDate, DateTime TargetStartDate, DateTime TargetFinishDate, 
			object ActualStartDate, object ActualFinishDate,
			int StatusId, int TypeId, object CalendarId, object contactUid, object orgUid, 
			int CurrencyId, int PriorityId, int InitialPhaseId, int PhaseId,
			int PercentCompleted, int RiskLevelId, int BlockTypeId
			)
		{
			return DbHelper2.RunSpInteger("ProjectCreate",
				DbHelper2.mp("@TypeId", SqlDbType.Int, TypeId),
				DbHelper2.mp("@StatusId", SqlDbType.Int, StatusId),
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId),
				DbHelper2.mp("@ExecutiveManagerId", SqlDbType.Int, ExecutiveManagerId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, Title),
				DbHelper2.mp("@Description", SqlDbType.NText, Description),
				DbHelper2.mp("@CreationDate", SqlDbType.DateTime, CreationDate),
				DbHelper2.mp("@TargetStartDate", SqlDbType.DateTime, TargetStartDate),
				DbHelper2.mp("@TargetFinishDate", SqlDbType.DateTime, TargetFinishDate),
				DbHelper2.mp("@ActualStartDate", SqlDbType.DateTime, ActualStartDate),
				DbHelper2.mp("@ActualFinishDate", SqlDbType.DateTime, ActualFinishDate),
				DbHelper2.mp("@Goals", SqlDbType.NText, Goals),
				DbHelper2.mp("@Scope", SqlDbType.NText, Scope),
				DbHelper2.mp("@Deliverables", SqlDbType.NText, Deliverables),
				DbHelper2.mp("@CurrencyId", SqlDbType.Int, CurrencyId),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId),
				DbHelper2.mp("@InitialPhaseId", SqlDbType.Int, InitialPhaseId),
				DbHelper2.mp("@PhaseId", SqlDbType.Int, PhaseId),
				DbHelper2.mp("@PercentCompleted", SqlDbType.Int, PercentCompleted),
				DbHelper2.mp("@RiskLevelId", SqlDbType.Int, RiskLevelId),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid),
				DbHelper2.mp("@BlockTypeId", SqlDbType.Int, BlockTypeId));
		}
		#endregion

		#region Delete
		public static void Delete(int ProjectId)
		{
			DbHelper2.RunSp("ProjectDelete", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetProjectManager
		public static int GetProjectManager(int ProjectId)
		{
			return DbHelper2.RunSpInteger("ProjectManagerGet",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetProject
		/// <summary>
		///		ProjectId, TypeId, TypeName, CalendarId, CalendarName, CreatorId, 
		///		ManagerId, ExecutiveManagerId, Title, Description, CreationDate, StartDate, FinishDate, 
		///		TargetStartDate, TargetFinishDate, ActualStartDate, ActualFinishDate, FixedHours, 
		///		FixedCost, Goals, Scope, Deliverables, StatusId, IsActive, StatusName, 
		///		ContactUid, OrgUid, ClientName, XMLFileId, CurrencyId, CurrencySymbol,
		///		PriorityId, PriorityName, PercentCompleted, PhaseId, PhaseName,
		///		RiskLevelId, RiskLevelName, RiskLevelWeight, InitialPhaseId, InitialPhaseName, IsMSProject,
		///		TaskTime, TotalMinutes, TotalApproved, BlockTypeId, ProjectCode
		/// </summary>
		public static IDataReader GetProject(int ProjectId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate", "StartDate", "FinishDate", "TargetStartDate", "TargetFinishDate", "ActualStartDate", "ActualFinishDate"},
				"ProjectGet", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetCalendar
		public static int GetCalendar(int ProjectId)
		{
			return DbHelper2.RunSpInteger("ProjectGetCalendar", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetTitle
		public static string GetTitle(int ProjectId)
		{
			return (string)DbHelper2.RunSpScalar("ProjectGetTitle", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
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
		public static IDataReader GetListProjects(int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate", "StartDate", "FinishDate", "TargetStartDate", "TargetFinishDate", "ActualStartDate", "ActualFinishDate"},
				"ProjectsGet", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}

		public static DataTable GetListProjectsDataTable(int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate", "StartDate", "FinishDate", "TargetStartDate", "TargetFinishDate", "ActualStartDate", "ActualFinishDate"},
				"ProjectsGet", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListProjectsForPMAndPPM
		/// <summary>
		/// ProjectId, TypeId, CalendarId, CreatorId, ManagerId, ManagerName, ExecutiveManagerId,
		/// Title, Description, PercentCompleted, CreationDate, StartDate, FinishDate, 
		/// TargetStartDate, TargetFinishDate, ActualStartDate, ActualFinishDate, 
		/// FixedHours, FixedCost, Goals, Scope, 
		/// Deliverables, StatusId, StatusName, IsActive, CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListProjectsForPMAndPPM(int UserId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate", "StartDate", "FinishDate", "TargetStartDate", "TargetFinishDate", "ActualStartDate", "ActualFinishDate"},
				"ProjectsGetForPMAndPPM", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListProjectsSimple
		/// <summary>
		/// Reader returns fields:
		///		ProjectId, Title
		/// </summary>
		public static IDataReader GetListProjectsSimple(int UserId)
		{
			return DbHelper2.RunSpDataReader("ProjectsGetSimple", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
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
			int UserId, int TimeZoneId,	string SearchString, 
			int State, int StatusId, int TypeId, int PriorityId, object contactUid, object orgUid,
			int StartDateCondition, object StartDate,
			int FinishDateCondition, object FinishDate,
			int ManagerId, int ExecutiveManagerId, int LanguageId, 
			int CategoryType, int ProjectCategoryType, int ProjectGroupType,
			int PhaseId, bool OnlyForUser)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			SearchString = DBCommon.ReplaceSqlWildcard(SearchString);
			//

			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate", "StartDate", "FinishDate", "TargetStartDate", "TargetFinishDate", "ActualStartDate", "ActualFinishDate"},
				"ProjectsGetByFilter", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@SearchString", SqlDbType.NVarChar, 100, SearchString),
				DbHelper2.mp("@State", SqlDbType.Int, State),
				DbHelper2.mp("@StatusId", SqlDbType.Int, StatusId),
				DbHelper2.mp("@FormId", SqlDbType.Int, TypeId),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId),

				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid),

				DbHelper2.mp("@StartDateCondition", SqlDbType.Int, StartDateCondition),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDateCondition", SqlDbType.Int, FinishDateCondition),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId),
				DbHelper2.mp("@ExecutiveManagerId", SqlDbType.Int, ExecutiveManagerId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@CategoryType", SqlDbType.Int, CategoryType),
				DbHelper2.mp("@ProjectCategoryType", SqlDbType.Int, ProjectCategoryType),
				DbHelper2.mp("@ProjectGroupType", SqlDbType.Int, ProjectGroupType),
				DbHelper2.mp("@PhaseId", SqlDbType.Int, PhaseId),
				DbHelper2.mp("@OnlyForUser", SqlDbType.Bit, OnlyForUser));
		}
		#endregion

		#region GetListActiveProjects
		/// <summary>
		/// Reader returns fields:
		///		ProjectId, Title
		/// </summary>
		public static IDataReader GetListActiveProjects()
		{
			return DbHelper2.RunSpDataReader("ProjectsGetActive");
		}

		public static DataTable GetListActiveProjectsDataTable()
		{
			return DbHelper2.RunSpDataTable("ProjectsGetActive");
		}
		#endregion


		#region GetListProjectsByManager
		/// <summary>
		/// Reader returns fields:
		///		ProjectId, Title
		/// </summary>
		public static IDataReader GetListProjectsByManager(int UserId)
		{
			return DbHelper2.RunSpDataReader("ProjectsGetByManager", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListActiveProjectsByUser
		/// <summary>
		/// Reader returns columns:
		///		ProjectId, Title, ManagerId, CreationDate, TargetFinishDate
		/// </summary>
		public static IDataReader GetListActiveProjectsByUser(int UserId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate", "TargetFinishDate"},
				"ProjectsGetActiveByUser", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListActiveProjectsByUserDataTable
		/// <summary>
		/// DataTable contains columns:
		///		ProjectId, Title, ManagerId, CreationDate, TargetFinishDate
		/// </summary>
		public static DataTable GetListActiveProjectsByUserDataTable(int UserId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate", "TargetFinishDate"},
				"ProjectsGetActiveByUser", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListActiveProjectsByUserOnlyDataTable
		/// <summary>
		///		ProjectId, Title, ManagerId, StatusId, PercentCompleted, TargetStartDate, 
		///		TargetFinishDate, PriorityId, PriorityName, ProjectCode,
		///		IsManager, IsExecutiveManager, IsTeamMember, IsSponsor, IsStakeHolder
		/// </summary>
		public static DataTable GetListActiveProjectsByUserOnlyDataTable(int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"TargetFinishDate"},
				"ProjectsGetActiveByUserOnly", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListProjectTypes
		/// <summary>
		/// Reader returns fields:
		///		TypeId, TypeName 
		/// </summary>
		public static IDataReader GetListProjectTypes()
		{
			return DbHelper2.RunSpDataReader("ProjectTypesGet");
		}
		#endregion

		#region GetListProjectTypesForDictionaries
		/// <summary>
		///		ItemId, ItemName, CanDelete
		/// </summary>
		public static DataTable GetListProjectTypesForDictionaries()
		{
			return DbHelper2.RunSpDataTable("ProjectTypesGetForDictionaries");
		}
		#endregion

		#region GetListProjectStatus
		/// <summary>
		/// Reader returns fields:
		///		StatusId, StatusName, IsActive 
		/// </summary>
		public static IDataReader GetListProjectStatus(int LanguageId)
		{
			return DbHelper2.RunSpDataReader("ProjectStatusGet",
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetSecurityForUser
		/// <summary>
		/// Reader return fields:
		///  ProjectId, PrincipalId, IsTeamMember, IsSponsor, IsStakeHolder, IsManager, IsExecutiveManager
		/// </summary>
		/// <param name="ProjectId"></param>
		/// <param name="UserId"></param>
		/// <returns></returns>
		public static IDataReader GetSecurityForUser(int ProjectId, int UserId)
		{
			return DbHelper2.RunSpDataReader("ProjectGetSecurityForUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListFirstLevelTasks
		/// <summary>
		/// Reader return fields:
		///  TaskId,TaskNum
		/// </summary>
		/// <param name="ProjectId"></param>
		/// <returns></returns>
		public static IDataReader GetListFirstLevelTasks(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("ProjectGetFirstLevelTasks",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListProjectsByCalendar
		/// <summary>
		/// Reader returns fields:
		///		ProjectId
		/// </summary>
		public static IDataReader GetListProjectsByCalendar(int CalendarId)
		{
			return DbHelper2.RunSpDataReader("ProjectsGetByCalendar", 
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId));
		}
		#endregion

		#region ----- TeamMembers, Stakeholders, Sponsors -----
		#region GetListMembers
		/// <summary>
		/// Reader returns fields:
		///		PrincipalId, IsTeamMember, IsSponsor, IsStakeholder
		/// </summary>
		public static IDataReader GetListMembers(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("ProjectGetMembers", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListTeamMembers
		/// <summary>
		/// Reader returns fields:
		///		MemberId, ProjectId, UserId, Code, Rate
		/// </summary>
		public static IDataReader GetListTeamMembers(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("ProjectGetTeamMembers", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListTeamMembersDataTable
		/// <summary>
		/// Reader returns fields:
		///		MemberId, ProjectId, UserId, Code, Rate 
		/// </summary>
		public static DataTable GetListTeamMembersDataTable(int ProjectId)
		{
			return DbHelper2.RunSpDataTable("ProjectGetTeamMembers", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListTeamMemberNames
		/// <summary>
		/// Reader returns fields:
		///		UserId, FirstName, LastName, Activity
		/// </summary>
		public static IDataReader GetListTeamMemberNames(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("ProjectGetTeamMemberNames", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListTeamMemberNamesDataTable
		/// <summary>
		///		UserId, FirstName, LastName
		/// </summary>
		public static DataTable GetListTeamMemberNamesDataTable(int ProjectId)
		{
			return DbHelper2.RunSpDataTable("ProjectGetTeamMemberNames", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListTeamMemberNamesWithManager
		/// <summary>
		///		UserId, FirstName, LastName, IsExternal
		/// </summary>
		public static IDataReader GetListTeamMemberNamesWithManager(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("ProjectGetTeamMemberNamesWithManager", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListTeamMemberNamesWithManagerDataTable
		/// <summary>
		///		UserId, FirstName, LastName, IsExternal
		/// </summary>
		public static DataTable GetListTeamMemberNamesWithManagerDataTable(int ProjectId)
		{
			return DbHelper2.RunSpDataTable("ProjectGetTeamMemberNamesWithManager", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListSponsors
		/// <summary>
		/// Reader returns fields:
		///		MemberId, ProjectId, PrincipalId, IsGroup
		/// </summary>
		public static IDataReader GetListSponsors(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("ProjectGetSponsors", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListSponsorsDataTable
		/// <summary>
		/// Reader returns fields:
		///		MemberId, ProjectId, PrincipalId, IsGroup
		/// </summary>
		public static DataTable GetListSponsorsDataTable(int ProjectId)
		{
			return DbHelper2.RunSpDataTable("ProjectGetSponsors", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListStakeholders
		/// <summary>
		/// Reader returns fields:
		///		MemberId, ProjectId, PrincipalId, IsGroup
		/// </summary>
		public static IDataReader GetListStakeholders(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("ProjectGetStakeholders", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListStakeholdersDataTable
		/// <summary>
		/// Reader returns fields:
		///		MemberId, ProjectId, PrincipalId, IsGroup
		/// </summary>
		public static DataTable GetListStakeholdersDataTable(int ProjectId)
		{
			return DbHelper2.RunSpDataTable("ProjectGetStakeholders", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region AddTeamMember
		public static void AddTeamMember(int ProjectId, int PrincipalId, string Code, decimal Rate)
		{
			DbHelper2.RunSp("ProjectMemberAddMember",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId),
				DbHelper2.mp("@Code", SqlDbType.NChar, 2, Code),
				DbHelper2.mp("@Rate", SqlDbType.Money, Rate));
		}
		#endregion

		#region AddSponsor
		public static void AddSponsor(int ProjectId, int PrincipalId)
		{
			DbHelper2.RunSp("ProjectMemberAddSponsor",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId));
		}
		#endregion

		#region AddStakeholder
		public static void AddStakeholder(int ProjectId, int PrincipalId)
		{
			DbHelper2.RunSp("ProjectMemberAddStakeholder",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId));
		}
		#endregion

		#region DeleteTeamMember
		public static void DeleteTeamMember(int ProjectId, int PrincipalId)
		{
			DbHelper2.RunSp("ProjectMemberDeleteMember",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId));
		}
		#endregion

		#region DeleteSponsor
		public static void DeleteSponsor(int ProjectId, int PrincipalId)
		{
			DbHelper2.RunSp("ProjectMemberDeleteSponsor",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId));
		}
		#endregion

		#region DeleteStakeholder
		public static void DeleteStakeholder(int ProjectId, int PrincipalId)
		{
			DbHelper2.RunSp("ProjectMemberDeleteStakeholder",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId));
		}
		#endregion
		#endregion

		#region GetProjectDateRange
		/// <summary>
		/// Reader returns fields:
		///  StartDate, FinishDate 
		/// </summary>
		/// <param name="ProjectId"></param>
		/// <returns></returns>
		public static IDataReader GetProjectDateRange(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("ProjectGetDateRange",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region UpdateTargetDates
		public static void UpdateTargetDates(int ProjectId, DateTime StartTargetDate, DateTime FinishTargetDate)
		{
			DbHelper2.RunSp("ProjectUpdateTargetDates",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@TargetStartDate", SqlDbType.DateTime, StartTargetDate),
				DbHelper2.mp("@TargetFinishDate", SqlDbType.DateTime, FinishTargetDate));
		}
		#endregion

		#region GetProjectMemberByCode
		public static int GetProjectMemberByCode(int ProjectId, string RName, string Code)
		{
			return DbHelper2.RunSpInteger("ProjectMemberGetByCode",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@RName", SqlDbType.NVarChar, 250, RName),
				DbHelper2.mp("@Code", SqlDbType.NVarChar, 250, Code));
		}
		#endregion

		#region UpdateManagerAtAssociatedObjects
		public static void UpdateManagerAtAssociatedObjects(int ProjectId, int ManagerId)
		{
			DbHelper2.RunSp("ProjectUpdateManagerAtAssociatedObjects",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId));
		}
		#endregion

		#region AddProjectType
		public static int AddProjectType(string TypeName)
		{
			return DbHelper2.RunSpInteger("ProjectTypeAdd", 
				DbHelper2.mp("@TypeName", SqlDbType.NVarChar, 50, TypeName));
		}
		#endregion

		#region UpdateProjectType
		public static void UpdateProjectType(int TypeId, string TypeName)
		{
			DbHelper2.RunSp("ProjectTypeUpdate", 
				DbHelper2.mp("@TypeId", SqlDbType.Int, TypeId),
				DbHelper2.mp("@TypeName", SqlDbType.NVarChar, 50, TypeName));
		}
		#endregion

		#region DeleteProjectType
		public static void DeleteProjectType(int TypeId)
		{
			DbHelper2.RunSp("ProjectTypeDelete", 
				DbHelper2.mp("@TypeId", SqlDbType.Int, TypeId));
		}
		#endregion

		#region GetSharingLevel
		public static int GetSharingLevel(int UserId, int ProjectId)
		{
			return DbHelper2.RunSpInteger("SharingGetLevelForProject", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListProjectsUpdatedByUser
		/// <summary>
		///		ProjectId, Title, LastSavedDate
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListProjectsUpdatedByUser(int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"LastSavedDate"},
				"ProjectsGetUpdatedByUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListProjectsUpdatedByUserDataTable
		/// <summary>
		///		ProjectId, Title, StatusId, LastSavedDate
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectsUpdatedByUserDataTable(int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"LastSavedDate"},
				"ProjectsGetUpdatedByUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListProjectsUpdatedForUser
		/// <summary>
		///		ProjectId, Title, LastEditorId, LastSavedDate, StatusId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListProjectsUpdatedForUser(int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"LastSavedDate"},
				"ProjectsGetUpdatedForUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListProjectsUpdatedForUserDataTable
		/// <summary>
		///		ProjectId, Title, StatusId, LastEditorId, LastSavedDate
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectsUpdatedForUserDataTable(int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"LastSavedDate"},
				"ProjectsGetUpdatedForUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListProjectsByKeyword
		/// <summary>
		/// Reader returns fields:
		///	 ProjectId, ManagerId, Title, Description, StatusId, PercentCompleted, 
		///	 StartDate, FinishDate, TargetStartDate, TargetFinishDate, 
		///	 ActualStartDate, ActualFinishDate, ProjectCode
		/// </summary>
		public static IDataReader GetListProjectsByKeyword(
			int UserId, int TimeZoneId, string Keyword)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "TargetStartDate", "TargetFinishDate", "ActualStartDate", "ActualFinishDate"},
				"ProjectsSearch", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, Keyword));
		}

		public static DataTable GetListProjectsByKeywordDataTable(
			int UserId, int TimeZoneId, string Keyword)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "TargetStartDate", "TargetFinishDate", "ActualStartDate", "ActualFinishDate"},
				"ProjectsSearch", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, Keyword));
		}
		#endregion

		#region GetListProjectCategoriesForDictionaries
		/// <summary>
		///  ItemId, ItemName, CanDelete
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectCategoriesForDictionaries()
		{
			return DbHelper2.RunSpDataTable("ProjectCategoriesGetForDictionaries");
		}
		#endregion

		#region AddProjectCategory
		public static void AddProjectCategory(string CategoryName)
		{
			DbHelper2.RunSp("ProjectCategoryAdd", 
				DbHelper2.mp("@CategoryName", SqlDbType.NVarChar, 100, CategoryName));
		}
		#endregion

		#region UpdateProjectCategory
		public static void UpdateProjectCategory(int CategoryId, string CategoryName)
		{
			DbHelper2.RunSp("ProjectCategoryUpdate", 
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId),
				DbHelper2.mp("@CategoryName", SqlDbType.NVarChar, 100, CategoryName));
		}
		#endregion

		#region DeleteProjectCategory
		public static void DeleteProjectCategory(int CategoryId)
		{
			DbHelper2.RunSp("ProjectCategoryDelete", 
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId));
		}
		#endregion

		#region GetListProjectCategories
		/// <summary>
		/// Reader returns fields:
		///  CategoryId, CategoryName 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListProjectCategories(int CategoryId)
		{
			return DbHelper2.RunSpDataReader("ProjectCategoriesGet",
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId));
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
			return DbHelper2.RunSpDataReader("ProjectCategoriesGetByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region AssignProjectCategory
		public static void AssignProjectCategory(int ProjectId, int CategoryId)
		{
			DbHelper2.RunSp("ProjectCategoryAssign", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId));
		}
		#endregion

		#region RemoveCategoryFromProject
		public static void RemoveCategoryFromProject(int ProjectId, int CategoryId)
		{
			DbHelper2.RunSp("ProjectCategoryRemove", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId));
		}
		#endregion

		#region CheckForChangeableRoles
		public static int CheckForChangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("ProjectsCheckForChangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region CheckForUnchangeableRoles
		public static int CheckForUnchangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("ProjectsCheckForUnchangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListProjectsForChangeableRoles
		/// <summary>
		/// Reader returns fields:
		///  ProjectId, Title, StatusId, IsManager, IsExecutiveManagerId, IsTeamMember, IsSponsor, IsStakeHolder, CanView, CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListProjectsForChangeableRoles(int UserId, int CurrentUserId)
		{
			return DbHelper2.RunSpDataReader("ProjectsGetForChangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@CurrentUserId", SqlDbType.Int, CurrentUserId));
		}
		#endregion

		#region ReplaceUnchangeableUserToManager
		public static void ReplaceUnchangeableUserToManager(int UserId)
		{
			DbHelper2.RunSp("ProjectsReplaceUnchangeableUserToManager", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ReplaceUnchangeableUser
		public static void ReplaceUnchangeableUser(int FromUserId, int ToUserId)
		{
			DbHelper2.RunSp("ProjectsReplaceUnchangeableUser", 
				DbHelper2.mp("@FromUserId", SqlDbType.Int, FromUserId),
				DbHelper2.mp("@ToUserId", SqlDbType.Int, ToUserId));
		}
		#endregion

		#region UpdateXMLFileId
		public static void UpdateXMLFileId(int ProjectId, int XMLFileId)
		{
			DbHelper2.RunSp("ProjecUpdateXMLFileId",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@XMLFileId", SqlDbType.Int, XMLFileId));
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
			return DbHelper2.RunSpDataReader("ProjectsGetStatistic");
		}
		#endregion

		#region GetProjectStatisticByStatusDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByStatusDataTable(int LanguageId)
		{
			return DbHelper2.RunSpDataTable("ProjectsGetStatisticByStatus",
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetProjectStatisticByManagerDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByManagerDataTable()
		{
			return DbHelper2.RunSpDataTable("ProjectsGetStatisticByManager");
		}
		#endregion

		#region GetProjectStatisticByTypeDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByTypeDataTable()
		{
			return DbHelper2.RunSpDataTable("ProjectsGetStatisticByType");
		}
		#endregion

		#region GetProjectStatisticByProjectCategoryDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByProjectCategoryDataTable()
		{
			return DbHelper2.RunSpDataTable("ProjectsGetStatisticByProjectCategory");
		}
		#endregion

		#region GetProjectStatisticByGeneralCategoryDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByGeneralCategoryDataTable()
		{
			return DbHelper2.RunSpDataTable("ProjectsGetStatisticByGeneralCategory");
		}
		#endregion

		#region GetProjectStatisticByProjectGroupDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByProjectGroupDataTable()
		{
			return DbHelper2.RunSpDataTable("ProjectsGetStatisticByProjectGroup");
		}
		#endregion

		#region GetProjectStatisticByPhaseDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByPhaseDataTable()
		{
			return DbHelper2.RunSpDataTable("ProjectsGetStatisticByPhase");
		}
		#endregion

		#region GetProjectStatisticByPriorityDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByPriorityDataTable(int LanguageId)
		{
			return DbHelper2.RunSpDataTable("ProjectsGetStatisticByPriority",
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetProjectStatisticByClientDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectStatisticByClientDataTable()
		{
			return DbHelper2.RunSpDataTable("ProjectsGetStatisticByClient");
		}
		#endregion

		#region GetCurrencySymbol
		public static string GetCurrencySymbol(int ProjectId)
		{
			return (string)DbHelper2.RunSpScalar("ProjectGetCurrencySymbol", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region AddProjectCategoryUser
		public static void AddProjectCategoryUser(int CategoryId, int UserId)
		{
			DbHelper2.RunSp("ProjectCategoryUserAdd", 
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region DeleteProjectCategoryUser
		public static void DeleteProjectCategoryUser(int CategoryId, int UserId)
		{
			DbHelper2.RunSp("ProjectCategoryUserDelete", 
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListProjectCategoriesByUser
		/// <summary>
		/// Reader returns fields:
		///		CategoryId, UserId
		/// </summary>
		public static IDataReader GetListProjectCategoriesByUser(int UserId)
		{
			return DbHelper2.RunSpDataReader("ProjectCategoryUserGetList",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListCategoriesStatisticForPMAndPPMDataTable
		/// <summary>
		///		CategoryId, CategoryName, Count
		/// </summary>
		public static DataTable GetListCategoriesStatisticForPMAndPPMDataTable(int UserId)
		{
			return DbHelper2.RunSpDataTable("CategoriesGetProjectStatisticForPMAndPPM",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListProjectCategoriesStatisticForPMAndPPMDataTable
		/// <summary>
		///		CategoryId, CategoryName, Count
		/// </summary>
		public static DataTable GetListProjectCategoriesStatisticForPMAndPPMDataTable(int UserId)
		{
			return DbHelper2.RunSpDataTable("ProjectCategoriesGetProjectStatisticForPMAndPPM",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListTaskResources
		/// <summary>
		///	 ResourceId, UserId, FirstName, LastName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListTaskResources(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("TaskResourcesGetByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListTaskResourcesDataTable
		/// <summary>
		///	 ResourceId, UserId, FirstName, LastName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListTaskResourcesDataTable(int ProjectId)
		{
			return DbHelper2.RunSpDataTable("TaskResourcesGetByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion


		#region AddProjectRelation
		public static void AddProjectRelation(int ProjectId, int RelProjectId)
		{
			DbHelper2.RunSp("ProjectRelatonAdd",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@RelProjectId", SqlDbType.Int,	RelProjectId));
		}
		#endregion

		#region DeleteProjectRelation
		public static void DeleteProjectRelation(int ProjectId, int RelProjectId)
		{
			DbHelper2.RunSp("ProjectRelationDelete",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@RelProjectId", SqlDbType.Int,	RelProjectId));
		}
		#endregion

		#region GetListProjectRelationsDataTable
		/// <summary>
		///	 ProjectId, Title, ManagerId, CanView, CanViewSnapshot
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectRelationsDataTable(int ProjectId, int UserId)
		{
			return DbHelper2.RunSpDataTable("ProjectRelatonsGet",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion


		#region AddProjectGroupUser
		public static void AddProjectGroupUser(int ProjectGroupId, int UserId)
		{
			DbHelper2.RunSp("ProjectGroupUserAdd", 
				DbHelper2.mp("@ProjectGroupId", SqlDbType.Int, ProjectGroupId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region DeleteProjectGroupUser
		public static void DeleteProjectGroupUser(int ProjectGroupId, int UserId)
		{
			DbHelper2.RunSp("ProjectGroupUserDelete", 
				DbHelper2.mp("@ProjectGroupId", SqlDbType.Int, ProjectGroupId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListProjectGroupsByUser
		/// <summary>
		/// Reader returns fields:
		///		ProjectGroupId, UserId
		/// </summary>
		public static IDataReader GetListProjectGroupsByUser(int UserId)
		{
			return DbHelper2.RunSpDataReader("ProjectGroupUserGetList",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListProjectPhasesForDictionaries
		/// <summary>
		///  ItemId, ItemName, Weight, CanDelete
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectPhasesForDictionaries()
		{
			return DbHelper2.RunSpDataTable("ProjectPhasesGetForDictionaries");
		}
		#endregion

		#region AddProjectPhase
		public static void AddProjectPhase(string PhaseName, int Weight)
		{
			DbHelper2.RunSp("ProjectPhaseAdd", 
				DbHelper2.mp("@PhaseName", SqlDbType.NVarChar, 50, PhaseName),
				DbHelper2.mp("@Weight", SqlDbType.Int, Weight));
		}
		#endregion

		#region UpdateProjectPhase
		public static void UpdateProjectPhase(int PhaseId, string PhaseName, int Weight)
		{
			DbHelper2.RunSp("ProjectPhaseUpdate", 
				DbHelper2.mp("@PhaseId", SqlDbType.Int, PhaseId),
				DbHelper2.mp("@PhaseName", SqlDbType.NVarChar, 50, PhaseName),
				DbHelper2.mp("@Weight", SqlDbType.Int, Weight));
		}
		#endregion

		#region DeleteProjectPhase
		public static void DeleteProjectPhase(int PhaseId)
		{
			DbHelper2.RunSp("ProjectPhaseDelete", 
				DbHelper2.mp("@PhaseId", SqlDbType.Int, PhaseId));
		}
		#endregion

		#region GetListProjectPhases
		/// <summary>
		///  PhaseId, PhaseName, Weight
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListProjectPhases(int PhaseId)
		{
			return DbHelper2.RunSpDataReader("ProjectPhasesGet",
				DbHelper2.mp("@PhaseId", SqlDbType.Int, PhaseId));
		}
		#endregion

		#region AddProjectSnapshot
		public static void AddProjectSnapshot(int ProjectId, int CreatorId)
		{
			DbHelper2.RunSp("ProjectSnapshotAdd", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId));
		}
		#endregion

		#region GetListProjectSnapshots
		/// <summary>
		///  SnapshotId, ProjectId, StatusId, StatusName, TargetBudget, EstimatedBudget, 
		///  ActualBudget, StartDate, FinishDate, TargetFinishDate, 
		///  CreatorId, CreationDate, PhaseId, PhaseName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectSnapshots(int ProjectId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "TargetFinishDate", "CreationDate"},
				"ProjectSnapshotGetList",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region DeleteProjectSnapshot
		public static void DeleteProjectSnapshot(int ProjectId)
		{
			DbHelper2.RunSp("ProjectSnapshotDelete", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListProjectsGroupedByPortfolio
		/// <summary>
		///  PortfolioId, PortfolioName, ProjectId, ProjectName, StatusName, 
		///  Resources, TasksTodos, Issues, PhaseName, 
		///  ProjectsCount, ActiveProjectsCount, IsHeader
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectsGroupedByPortfolio(int PortfolioId, int PhaseId, int StatusId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable("ProjectsGetGroupedByPortfolio",
				DbHelper2.mp("@PortfolioId", SqlDbType.Int, PortfolioId),
				DbHelper2.mp("@PhaseId", SqlDbType.Int, PhaseId),
				DbHelper2.mp("@StatusId", SqlDbType.Int, StatusId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListProjectsGroupedByPhase
		/// <summary>
		///  PhaseId, PhaseName, Weight, ProjectId, ProjectName, StatusName, 
		///  Resources, TasksTodos, Issues, ProjectsCount, ActiveProjectsCount, IsHeader
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectsGroupedByPhase(int PortfolioId, int PhaseId, int StatusId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable("ProjectsGetGroupedByPhase",
				DbHelper2.mp("@PortfolioId", SqlDbType.Int, PortfolioId),
				DbHelper2.mp("@PhaseId", SqlDbType.Int, PhaseId),
				DbHelper2.mp("@StatusId", SqlDbType.Int, StatusId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListProjectsGroupedByManager
		/// <summary>
		///  ManagerId, ManagerName, ProjectId, ProjectName, StatusName, 
		///  OpenTasks, CompletedTasks, Issues, IsHeader
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectsGroupedByManager(int PortfolioId, int PhaseId, int StatusId, int ManagerId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable("ProjectsGetGroupedByManager",
				DbHelper2.mp("@PortfolioId", SqlDbType.Int, PortfolioId),
				DbHelper2.mp("@PhaseId", SqlDbType.Int, PhaseId),
				DbHelper2.mp("@StatusId", SqlDbType.Int, StatusId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListProjectsGroupedByClient
		/// <summary>
		///  ContactUid, OrgUid, IsClient, ClientName, ProjectId, ProjectName, StatusName, 
		///  OpenTasks, CompletedTasks, Issues, IsCollapsed
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectsGroupedByClient(int portfolioId,
			int phaseId, int statusId, int managerId, int userId,
			int languageId, PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			return DbHelper2.RunSpDataTable("ProjectsGetGroupedByClient",
				DbHelper2.mp("@PortfolioId", SqlDbType.Int, portfolioId),
				DbHelper2.mp("@PhaseId", SqlDbType.Int, phaseId),
				DbHelper2.mp("@StatusId", SqlDbType.Int, statusId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, managerId),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, languageId),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid));
		}
		#endregion

		#region CollapseByClient
		public static void CollapseByClient(int UserId, PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			DbHelper2.RunSp("CollapsedProjectByClientAdd",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion

		#region ExpandByClient
		public static void ExpandByClient(int UserId, PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			DbHelper2.RunSp("CollapsedProjectByClientDelete",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion

		#region GetListProjectsByUsersInGroup
		/// <summary>
		///		UserId, UserName, ProjectId, Title, PriorityId, PriorityName, 
		///		StatusId, StatusName, PhaseId, PhaseName, IsTeamMember,
		///		IsSponsor, IsStakeHolder, IsManager, IsExecutiveManager, IsGroup
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListProjectsByUsersInGroup(int GroupId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable("ProjectsGetByUsersInGroup",
				DbHelper2.mp("@GroupId", SqlDbType.Int, GroupId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
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
			return DbHelper2.RunSpDataReader("ProjectMetricsGet");
		}
		#endregion

		#region GetProjectSecurity
		/// <summary>
		/// UserId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetProjectSecurity(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("ProjectSecurityGet",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetFileCountByProject
		public static int GetFileCountByProject(int ProjectId)
		{
			return DbHelper2.RunSpInteger("fsc_FileGetCountByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetFileCountByProjectAll
		// с учётом фалов в ToDo, Tasks, Documents, Incidents, Events
		public static int GetFileCountByProjectAll(int ProjectId)
		{
			return DbHelper2.RunSpInteger("fsc_FileGetCountByProjectAll",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListActiveProjectsByManager
		/// <summary>
		/// ProjectId, Title
		/// </summary>
		public static IDataReader GetListActiveProjectsByManager(int UserId)
		{
			return DbHelper2.RunSpDataReader("ProjectsGetActiveByManager", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region AddRiskLevel
		public static void AddRiskLevel(string RiskLevelName, int Weight)
		{
			DbHelper2.RunSp("RiskLevelAdd", 
				DbHelper2.mp("@RiskLevelName", SqlDbType.NVarChar, 50, RiskLevelName),
				DbHelper2.mp("@Weight", SqlDbType.Int, Weight));
		}
		#endregion

		#region UpdateRiskLevel
		public static void UpdateRiskLevel(int RiskLevelId, string RiskLevelName, int Weight)
		{
			DbHelper2.RunSp("RiskLevelUpdate", 
				DbHelper2.mp("@RiskLevelId", SqlDbType.Int, RiskLevelId),
				DbHelper2.mp("@RiskLevelName", SqlDbType.NVarChar, 50, RiskLevelName),
				DbHelper2.mp("@Weight", SqlDbType.Int, Weight));
		}
		#endregion

		#region DeleteRiskLevel
		public static void DeleteRiskLevel(int RiskLevelId)
		{
			DbHelper2.RunSp("RiskLevelDelete", 
				DbHelper2.mp("@RiskLevelId", SqlDbType.Int, RiskLevelId));
		}
		#endregion

		#region GetListRiskLevelsForDictionaries
		/// <summary>
		///  ItemId, ItemName, Weight, CanDelete
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListRiskLevelsForDictionaries()
		{
			return DbHelper2.RunSpDataTable("RiskLevelsGetForDictionaries");
		}
		#endregion

		#region UpdateDates
		public static void UpdateDates(int ProjectId, object StartDate, object FinishDate)
		{
			DbHelper2.RunSp("ProjectUpdateDates",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate));
		}
		#endregion

		#region GetListProjectManagers
		/// <summary>
		/// Reader returns fields:
		///		ManagerId, UserName
		/// </summary>
		public static IDataReader GetListProjectManagers()
		{
			return DbHelper2.RunSpDataReader("ProjectManagersGet");
		}
		#endregion

		#region GetListExecutiveManagers
		/// <summary>
		/// Reader returns fields:
		///		ExecutiveManagerId, UserName
		/// </summary>
		public static IDataReader GetListExecutiveManagers()
		{
			return DbHelper2.RunSpDataReader("ProjectExecutiveManagersGet");
		}
		#endregion

		#region GetStatus(...)
		public static int GetStatus(int ObjectId)
		{
			return DBCommon.NullToInt32(
				DbHelper2.RunSpScalar("ProjectGetStatus"
				, DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId)
				));
		}
		#endregion

		#region UpdateIsMSProject
		public static void UpdateIsMSProject(int ProjectId, bool IsMSProject)
		{
			DbHelper2.RunSp("ProjecUpdateIsMSProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@IsMSProject", SqlDbType.Bit, IsMSProject));
		}
		#endregion

		#region GetMSProjectXMLStream
		public static SqlBlobStream GetMSProjectXMLStream(int projectId, SqlBlobAccess sqlBlobAccess)
		{
			SqlTransaction sqlTran = DbContext.Current.Transaction;
			if (sqlTran != null)
			{
				return new SqlBlobStream(sqlTran, "Projects", "MsProjectXml", sqlBlobAccess
					, new SqlParameter("@ProjectId", projectId));
			}
			else
				throw new Exception("Transaction is not started");
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the properties PropertyId, ProjectId, Key, Value.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <returns></returns>
		public static IDataReader GetProperties(int projectId)
		{
			return DbHelper2.RunSpDataReader("ProjectPropertyGet",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId));
		}

		/// <summary>
		/// Sets the property.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <param name="Key">The key.</param>
		/// <param name="Value">The value.</param>
		public static void SetProperty(int projectId, string key, string value)
		{
			DbHelper2.RunSp("ProjectPropertySet",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@Key", SqlDbType.NVarChar, 100, key),
				DbHelper2.mp("@Value", SqlDbType.NText, value, false));
		}

		/// <summary>
		/// Removes the property.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <param name="key">The key.</param>
		public static void RemoveProperty(int projectId, string key)
		{
			DbHelper2.RunSp("ProjectPropertyRemove",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@Key", SqlDbType.NVarChar, 100, key));
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
			object obj = DbHelper2.RunSpScalar("ProjectGetTeamMemberRate",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId));

			decimal retval = 0;
			if (obj != null && obj != DBNull.Value)
				retval = (decimal)obj;

			return retval;
		}
		#endregion

		#region GetListContactsForProjects
		/// <summary>
		/// Reader returns fields:
		///		ObjectId, ObjectTypeId, ClientName, ProjectCount
		/// </summary>
		public static IDataReader GetListContactsForProjects(int userId)
		{
			return DbHelper2.RunSpDataReader("ClientsGetForProjects",
				DbHelper2.mp("@userId", SqlDbType.Int, userId));
		}

		public static DataTable GetListContactsForProjectsDataTable(int userId)
		{
			return DbHelper2.RunSpDataTable("ClientsGetForProjects",
				DbHelper2.mp("@userId", SqlDbType.Int, userId));
		}
		#endregion
	}
}
