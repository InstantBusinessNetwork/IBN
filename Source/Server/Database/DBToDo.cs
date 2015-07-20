using System;
using System.Data;
using System.Data.SqlClient;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBToDo.
	/// </summary>
	public class DBToDo
	{
		#region Create
		public static int Create(
			object ProjectId, int CreatorId, int ManagerId,
			string Title, string Description, 
			DateTime CreationDate, 
			object StartDate, object FinishDate, 
			int PriorityId, int ActivationTypeId, int CompletionTypeId,
			bool MustBeConfirmed, int TaskTime, 
			object contactUid,
			object orgUid)
		{
			return DbHelper2.RunSpInteger("ToDoCreate",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, Title),
				DbHelper2.mp("@Description", SqlDbType.NText, Description),
				DbHelper2.mp("@CreationDate", SqlDbType.DateTime, CreationDate),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId),
				DbHelper2.mp("@ActivationTypeId", SqlDbType.Int, ActivationTypeId),
				DbHelper2.mp("@CompletionTypeId", SqlDbType.Int, CompletionTypeId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, MustBeConfirmed),
				DbHelper2.mp("@TaskTime", SqlDbType.Int, TaskTime),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid)
				);
		}
		#endregion

		#region Delete
		public static void Delete(int ToDoId)
		{
			DbHelper2.RunSp("ToDoDelete",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId));
		}
		#endregion

		#region GetSecurityForUser
		/// <summary>
		/// Reader return fields:
		///  ToDoId, PrincipalId, IsResource, IsManager
		/// </summary>
		/// <param name="ToDoId"></param>
		/// <param name="UserId"></param>
		/// <returns></returns>
		public static IDataReader GetSecurityForUser(int ToDoId, int UserId)
		{
			return DbHelper2.RunSpDataReader("ToDoGetSecurityForUser",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
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
		///  CompletionTypeId, CompletionTypeName, IsCompleted, MustBeConfirmed, 
		///  ReasonId, TaskId, CompleteTask, TaskTitle
		///  ContactUid, OrgUid, ClientName, TotalMinutes, TotalApproved, ProjectCode
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetToDo(int ToDoId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate","StartDate", "FinishDate", "ActualFinishDate"},
				"ToDoGet",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetToDoInUTC
		/// <summary>
		/// Reader returns fields:
		///  ToDoId, ProjectId, ProjectTitle, IncidentId, IncidentTitle, StatusId,
		///  CreatorId, ManagerId, CompletedBy,
		///  Title, Description, CreationDate, StartDate, FinishDate, 
		///  ActualFinishDate, PriorityId, PriorityName, PercentCompleted, IsActual, 
		///  CompletionTypeId, CompletionTypeName, IsCompleted, MustBeConfirmed, 
		///  ReasonId, AssetId, TaskId, CompleteTask, TaskTitle, StateId, 
		///  TotalMinutes, TotalApproved, ProjectCode
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetToDoInUTC(int ToDoId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader("ToDoGetInUTC",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListCompletionTypes
		/// <summary>
		/// Reader returns fields:
		///  CompletionTypeId, CompletionTypeName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListCompletionTypes(int LanguageId)
		{
			return DbHelper2.RunSpDataReader("CompletionTypesGet",
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListResources
		/// <summary>
		/// Reader returns fields:
		///	 ResourceId, ToDoId, UserId, MustBeConfirmed, ResponsePending, IsConfirmed, 
		///	 PercentCompleted, ActualFinishDate, LastSavedDate, IsGroup, IsExternal, ResourceName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListResources(int ToDoId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"ActualFinishDate", "LastSavedDate"},
				"ToDoResourcesGet",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId));
		}
		#endregion

		#region GetListResourcesDataTable
		/// <summary>
		/// DataTable returns fields:
		///	 ResourceId, ToDoId, UserId, MustBeConfirmed, ResponsePending, IsConfirmed, 
		///	 PercentCompleted, ActualFinishDate, LastSavedDate, IsGroup, IsExternal
		/// </summary>
		/// <param name="ToDoId"></param>
		/// <returns></returns>
		public static DataTable GetListResourcesDataTable(int ToDoId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"ActualFinishDate", "LastSavedDate"},
				"ToDoResourcesGet",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId));
		}
		#endregion

		#region AddResource
		public static void AddResource(int ToDoId, int UserId, bool MustBeConfirmed)
		{
			DbHelper2.RunSp("ToDoResourceAdd",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, MustBeConfirmed));
		}
		#endregion

		#region DeleteResource
		public static void DeleteResource(int ToDoId, int UserId)
		{
			DbHelper2.RunSp("ToDoResourceDelete",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetResourceByUser
		/// <summary>
		/// Reader returns fields:
		///	 MustBeConfirmed, ResponsePending, IsConfirmed, PercentCompleted
		/// </summary>
		/// <param name="ToDoId"></param>
		/// <param name="UserId"></param>
		/// <returns></returns>
		public static IDataReader GetResourceByUser(int ToDoId, int UserId)
		{
			return DbHelper2.RunSpDataReader("ToDoResourceGetByUser",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region UpdateResourcePercent
		public static void UpdateResourcePercent(int ToDoId, int UserId, int PercentCompleted, DateTime LastSavedDate)
		{
			DbHelper2.RunSp("ToDoResourceUpdatePercent",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@PercentCompleted", SqlDbType.Int, PercentCompleted),
				DbHelper2.mp("@LastSavedDate", SqlDbType.DateTime, LastSavedDate));
		}
		#endregion

		#region UpdatePercent
		public static void UpdatePercent(int ToDoId, int PercentCompleted)
		{
			DbHelper2.RunSp("ToDoUpdatePercent",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId),
				DbHelper2.mp("@PercentCompleted", SqlDbType.Int, PercentCompleted));
		}
		#endregion

		#region UpdateCompletion
		public static void UpdateCompletion(int ToDoId, bool IsCompleted, int ReasonId)
		{
			DbHelper2.RunSp("ToDoUpdateCompletion",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId),
				DbHelper2.mp("@IsCompleted", SqlDbType.Bit, IsCompleted),
				DbHelper2.mp("@ReasonId", SqlDbType.Int, ReasonId));
		}
		#endregion

		#region AddIncidentToDo
		public static void AddIncidentToDo(int IncidentId, int ToDoId, int StateId)
		{
			DbHelper2.RunSp("IncidentToDoAdd",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId),
				DbHelper2.mp("@StateId", SqlDbType.Int, StateId));
		}
		#endregion

		#region AddDocumentToDo
		public static void AddDocumentToDo(int DocumentId, int ToDoId, bool CompleteDocument)
		{
			DbHelper2.RunSp("DocumentToDoAdd",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId),
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId),
				DbHelper2.mp("@CompleteDocument", SqlDbType.Bit, CompleteDocument));
		}
		#endregion

		#region GetListToDoByFilter
		/// <summary>
		///  Reader returns fields:
		///		ToDoId, Title, Description, IsCompleted, StartDate, FinishDate, CreationDate,
		///		CanEdit, CanDelete, PriorityId, PercentCompleted, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoByFilter(int ProjectId, int UserId, int LanguageId, 
			int ResourceId, int TimeZoneId, DateTime StartDate, DateTime FinishDate, 
			bool GetAssigned, bool GetManaged, bool GetCreated, string Keyword, 
			object contactUid, object orgUid)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"ToDoGetByFilter",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@ResourceId", SqlDbType.Int, ResourceId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate), 
				DbHelper2.mp("@GetAssigned", SqlDbType.Bit, GetAssigned),
				DbHelper2.mp("@GetManaged", SqlDbType.Bit, GetManaged),
				DbHelper2.mp("@GetCreated", SqlDbType.Bit, GetCreated),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, Keyword),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion

		#region GetListToDoByFilterDataTable
		/// <summary>
		///  Reader returns fields:
		///		ToDoId, Title, Description, IsCompleted, StartDate, FinishDate, CreationDate,
		///		CanEdit, CanDelete, PriorityId, PriorityName, PercentCompleted, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoByFilterDataTable(int ProjectId, int UserId, int LanguageId,
			int ResourceId, int TimeZoneId, DateTime StartDate, DateTime FinishDate,
			bool GetAssigned, bool GetManaged, bool GetCreated, string Keyword,
			object contactUid, object orgUid)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "StartDate", "FinishDate" },
				"ToDoGetByFilter",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@ResourceId", SqlDbType.Int, ResourceId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate),
				DbHelper2.mp("@GetAssigned", SqlDbType.Bit, GetAssigned),
				DbHelper2.mp("@GetManaged", SqlDbType.Bit, GetManaged),
				DbHelper2.mp("@GetCreated", SqlDbType.Bit, GetCreated),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, Keyword),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion

		#region GetListToDoSimple
		/// <summary>
		///  Reader returns fields:
		///		ToDoId, Title
		/// </summary>
		/// <param name="ProjectId"></param>
		/// <param name="UserId"></param>
		/// <param name="GetAssigned"></param>
		/// <param name="GetManaged"></param>
		/// <returns></returns>
		public static IDataReader GetListToDoSimple(int ProjectId, 
			int UserId, bool GetAssigned, bool GetManaged)
		{
			return DbHelper2.RunSpDataReader("ToDoGetSimple",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@GetAssigned", SqlDbType.Bit, GetAssigned),
				DbHelper2.mp("@GetManaged", SqlDbType.Bit, GetManaged));
		}
		#endregion

		#region GetProject
		public static int GetProject(int ToDoId)
		{
			return DbHelper2.RunSpInteger("ToDoGetProject", 
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId));
		}
		#endregion

		#region GetListToDoForUserByProject
		/// <summary>
		/// DataTable contains fields:
		///  ToDoId, Title, Description, PriorityId, PriorityName, StartDate, FinishDate, 
		///  PercentCompleted, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoForUserByProject(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"ToDoGetForUserByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetIsCompleted
		public static int GetIsCompleted(int ToDoId)
		{
			return DbHelper2.RunSpInteger("ToDoGetIsCompleted",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId));
		}
		#endregion

		#region GetListActiveToDoAndTasks
		/// <summary>
		/// ItemId, Title, PriorityId, PriorityName, ManagerId, IsToDo, IsCompleted, 
		/// CompletionTypeId, StartDate, FinishDate, CreationDate, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListActiveToDoAndTasks(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate", "StartDate", "FinishDate"},
				"ToDoAndTasksGetActive",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListNotApprovedToDoAndTasks
		/// <summary>
		/// ItemId, Title, PriorityId, PriorityName, IsToDo, IsCompleted, 
		/// CompletionTypeId, StartDate, FinishDate, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListNotApprovedToDoAndTasks(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"ToDoAndTasksGetNotApproved",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListPendingToDoAndTasks
		/// <summary>
		/// ItemId, Title, PriorityId, PriorityName, ManagerId, IsToDo, 
		/// IsCompleted, CompletionTypeId, StartDate, FinishDate, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListPendingToDoAndTasks(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"ToDoAndTasksGetPending",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListToDoAndTasksWithoutResources
		/// <summary>
		/// ItemId, Title, PriorityId, PriorityName, IsToDo,
		/// IsCompleted, CompletionTypeId, StartDate, FinishDate, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksWithoutResources(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"ToDoAndTasksGetWithoutResources",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region UpdateProjectAndManager
		public static void UpdateProjectAndManager(int ToDoId, int ProjectId, int ManagerId)
		{
			DbHelper2.RunSp("ToDoUpdateProjectAndManager",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId));
		}
		#endregion

		#region GetListToDoByProject
		/// <summary>
		/// Reader returns fields
		///  ToDoId, Title, Description, StartDate, FinishDate, IsCompleted, PriorityId, 
    ///  PriorityName, StateId, ReasonId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoByProject(int ProjectId, int LanguageId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"ToDoGetByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetSharingLevel
		public static int GetSharingLevel(int UserId, int ToDoId)
		{
			return DbHelper2.RunSpInteger("SharingGetLevelForToDo", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId));
		}
		#endregion

		#region GetListToDoAndTaskResourcesNotPending
		/// <summary>
		/// ItemId, Title, PrincipalId, IsConfirmed, IsToDo, IsCompleted, CompletionTypeId, 
		/// LastSavedDate, StartDate, FinishDate, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTaskResourcesNotPending(int UserId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"LastSavedDate", "StartDate", "FinishDate"},
				"ToDoAndTaskResourcesGetNotPending",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListToDoAndTaskResourcesNotPendingDataTable
		/// <summary>
		/// ItemId, Title, PrincipalId, IsConfirmed, IsToDo, IsCompleted, CompletionTypeId, 
		/// LastSavedDate, StartDate, FinishDate, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTaskResourcesNotPendingDataTable(int UserId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"LastSavedDate", "StartDate", "FinishDate"},
				"ToDoAndTaskResourcesGetNotPending",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListNotCompletedToDoAndTasks
		/// <summary>
		/// ItemId, Title, PriorityId, PriorityName, IsToDo, ManagerId, ProjectId, ProjectTitle, 
		///  StartDate, FinishDate, CanEdit, CanDelete,
		///  IsCompleted, CompletionTypeId, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListNotCompletedToDoAndTasks(int UserId, 
			int TimeZoneId, int LanguageId, bool GetAssigned, bool GetManaged, bool GetCreated)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"ToDoAndTasksGetNotCompleted",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@GetAssigned", SqlDbType.Bit, GetAssigned),
				DbHelper2.mp("@GetManaged", SqlDbType.Bit, GetManaged),
				DbHelper2.mp("@GetCreated", SqlDbType.Bit, GetCreated));
		}
		#endregion

		#region GetListToDoAndTasksForUser
		/// <summary>
		///  Reader returns fields:
		///		ItemId, Title, CreatorId, IsToDo, StartDate, FinishDate, CreationDate,
		///		IsCompleted, CompletionTypeId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTasksForUser(int UserId, int TimeZoneId,
			bool GetAssigned, bool GetManaged, bool GetCreated)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "CreationDate"},
				"ToDoAndTasksGetForUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@GetAssigned", SqlDbType.Bit, GetAssigned),
				DbHelper2.mp("@GetManaged", SqlDbType.Bit, GetManaged),
				DbHelper2.mp("@GetCreated", SqlDbType.Bit, GetCreated));
		}
		#endregion

		#region GetListToDoAndTasksForUserDataTable
		/// <summary>
		///		ItemId, Title, CreatorId, IsToDo, StartDate, FinishDate, CreationDate,
		///		IsCompleted, CompletionTypeId, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksForUserDataTable(int UserId, int TimeZoneId,
			bool GetAssigned, bool GetManaged, bool GetCreated)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "CreationDate"},
				"ToDoAndTasksGetForUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@GetAssigned", SqlDbType.Bit, GetAssigned),
				DbHelper2.mp("@GetManaged", SqlDbType.Bit, GetManaged),
				DbHelper2.mp("@GetCreated", SqlDbType.Bit, GetCreated));
		}
		#endregion

		#region GetListToDoAndTasksByFilterDataTable
		/// <summary>
		///	ItemId, Title, Description, IsCompleted, IsToDo, ProjectId, ProjectTitle, ReasonId
		///	ManagerId, CompletionTypeId, StartDate, FinishDate, CanEdit, CanDelete, PriorityId, 
		///	PercentCompleted, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksByFilterDataTable(int ProjectId, 
			int UserId, int ResourceId, int TimeZoneId, 
			int StartDateCondition, DateTime StartDate, 
			int FinishDateCondition, DateTime FinishDate, string SearchString,
			bool GetAssigned, bool GetManaged, bool GetCreated, int CategoryType)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			SearchString = DBCommon.ReplaceSqlWildcard(SearchString);
			//

			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"ToDoAndTasksGetByFilter",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ResourceId", SqlDbType.Int, ResourceId),
				DbHelper2.mp("@StartDateCondition", SqlDbType.Int, StartDateCondition),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDateCondition", SqlDbType.Int, FinishDateCondition),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate), 
				DbHelper2.mp("@SearchString", SqlDbType.NVarChar, 100, SearchString), 
				DbHelper2.mp("@GetAssigned", SqlDbType.Bit, GetAssigned),
				DbHelper2.mp("@GetManaged", SqlDbType.Bit, GetManaged),
				DbHelper2.mp("@GetCreated", SqlDbType.Bit, GetCreated),
				DbHelper2.mp("@CategoryType", SqlDbType.Int, CategoryType));
		}
		#endregion

		#region GetListToDoForScheduling
		/// <summary>
		///  Reader returns fields:
		///		UserId, ToDoId, StartDate, FinishDate, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoForScheduling(int UserId, 
			int TimeZoneId, DateTime StartDate, DateTime FinishDate)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"TodoGetForScheduling",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate));
		}
		#endregion

		#region GetListToDoForEventScheduling
		/// <summary>
		///  Reader returns fields:
		///		UserId, ToDoId, StartDate, FinishDate, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoForEventScheduling(int EventId, 
			int TimeZoneId, DateTime StartDate, DateTime FinishDate)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"TodoGetForEventScheduling",
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate));
		}
		#endregion

		#region DeleteByProject
		public static void DeleteByProject(int ProjectId)
		{
			DbHelper2.RunSp("ToDoDeleteByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListToDoAndTasksUpdatedByUser
		/// <summary>
		///		ItemId, Title, IsToDo, IsCompleted, CompletionTypeId, 
		///		StartDate, FinishDate, LastSavedDate
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTasksUpdatedByUser(int ProjectId, int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "LastSavedDate"},
				"ToDoAndTasksGetUpdatedByUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListToDoAndTasksUpdatedByUserDataTable
		/// <summary>
		///		ItemId, Title, IsToDo, IsCompleted, CompletionTypeId, 
		///		StartDate, FinishDate, LastSavedDate, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksUpdatedByUserDataTable(int ProjectId, int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "LastSavedDate"},
				"ToDoAndTasksGetUpdatedByUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListToDoAndTasksUpdatedForUser
		/// <summary>
		///		ItemId, Title, LastEditorId, IsToDo, StartDate, FinishDate, LastSavedDate, 
		///		IsCompleted, CompletionTypeId, ProjectId, ProjectName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTasksUpdatedForUser(int ProjectId, int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "LastSavedDate"},
				"ToDoAndTasksGetUpdatedForUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListToDoAndTasksUpdatedForUserDataTable
		/// <summary>
		///		ItemId, Title, LastEditorId, IsToDo, StartDate, FinishDate, LastSavedDate, 
		///		IsCompleted, CompletionTypeId, ProjectId, ProjectName, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksUpdatedForUserDataTable(int ProjectId, int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "LastSavedDate"},
				"ToDoAndTasksGetUpdatedForUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListToDoAndTasksByKeyword
		/// <summary>
		///  Reader returns fields:
		///		ItemId, Title, Description, ManagerId, IsCompleted, IsToDo, CompletionTypeId,
		///		StartDate, FinishDate, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTasksByKeyword(
			int UserId, int TimeZoneId, string Keyword)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"ToDoAndTasksGetByKeyword",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, Keyword));
		}
		#endregion

		#region GetListActiveToDoAndTasksByUser
		/// <summary>
		/// ObjectId, Title, IsCompleted, IsToDo, ProjectId, ProjectTitle, CategoryId, 
		/// IsIncidentTodo, IsTaskTodo, IsDocumentTodo, ActualFinishDate, 
		/// ManagerId, ManagerName, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListActiveToDoAndTasksByUser(
			DateTime FromDate, DateTime ToDate, int UserId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"ActualFinishDate"},
				"ToDoAndTasksGetActiveByUser",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, FromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, ToDate),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListActiveToDoAndTasksByProject
		/// <summary>
		/// ObjectId, Title, IsCompleted, IsToDo, CategoryId, 
		/// IsIncidentTodo, IsTaskToDo, IsDocumentTodo, ActualFinishDate, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListActiveToDoAndTasksByProject(
			DateTime FromDate, DateTime ToDate, int ProjectId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"ActualFinishDate"},
				"ToDoAndTasksGetActiveByProject",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, FromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, ToDate),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region CheckForChangeableRoles
		public static int CheckForChangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("ToDoCheckForChangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region CheckForUnchangeableRoles
		public static int CheckForUnchangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("ToDoCheckForUnchangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListToDoForChangeableRoles
		/// <summary>
		/// Reader returns fields:
		///  ToDoId, Title, IsCompleted, CompletionTypeId, StartDate, FinishDate,
		///  IsManager, IsResource, CanView, CanEdit, CanDelete, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoForChangeableRoles(int UserId, int CurrentUserId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"TodoGetForChangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@CurrentUserId", SqlDbType.Int, CurrentUserId));
		}
		#endregion

		#region ReplaceUnchangeableUserToManager
		public static void ReplaceUnchangeableUserToManager(int UserId)
		{
			DbHelper2.RunSp("ToDoReplaceUnchangeableUserToManager", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ReplaceUnchangeableUser
		public static void ReplaceUnchangeableUser(int FromUserId, int ToUserId)
		{
			DbHelper2.RunSp("ToDoReplaceUnchangeableUser", 
				DbHelper2.mp("@FromUserId", SqlDbType.Int, FromUserId),
				DbHelper2.mp("@ToUserId", SqlDbType.Int, ToUserId));
		}
		#endregion

		#region GetListToDoAndTaskManagers
		/// <summary>
		/// UserId, FirstName, LastName, FullName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTaskManagers()
		{
			return DbHelper2.RunSpDataReader("ToDoAndTaskGetManagers");
		}
		#endregion

		#region GetListToDoAndTaskManagersDataTable
		/// <summary>
		/// UserId, FirstName, LastName, FullName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTaskManagersDataTable()
		{
			return DbHelper2.RunSpDataTable("ToDoAndTaskGetManagers");
		}
		#endregion

		#region GetListToDoAndTaskCreators
		/// <summary>
		/// UserId, FirstName, LastName, FullName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTaskCreators()
		{
			return DbHelper2.RunSpDataReader("ToDoAndTaskGetCreators");
		}
		#endregion

		#region GetListToDoAndTaskCreatorsDataTable
		/// <summary>
		/// UserId, FirstName, LastName, FullName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTaskCreatorsDataTable()
		{
			return DbHelper2.RunSpDataTable("ToDoAndTaskGetCreators");
		}
		#endregion

		#region GetListCompletedToDoAndTasks
		/// <summary>
		/// ItemId, Title, Description, IsCompleted, IsToDo, ProjectId, ProjectTitle, 
		/// ManagerId, StartDate, FinishDate, CanEdit, CanDelete, CompletionTypeId, 
		/// ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListCompletedToDoAndTasks(int UserId, 
			int TimeZoneId, bool GetAssigned, bool GetManaged, bool GetCreated)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"ToDoAndTasksGetCompleted",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@GetAssigned", SqlDbType.Bit, GetAssigned),
				DbHelper2.mp("@GetManaged", SqlDbType.Bit, GetManaged),
				DbHelper2.mp("@GetCreated", SqlDbType.Bit, GetCreated));
		}
		#endregion

		#region GetListManagedToDoAndTasksByResource
		/// <summary>
		/// ItemId, Title, PriorityId, IsToDo, ManagerId, CompletionTypeId, ProjectId, 
		/// ProjectTitle, IsCompleted, StartDate, FinishDate, ActualFinishDate, StateId,
		/// CanEdit, CanDelete, ReasonId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListManagedToDoAndTasksByResource(
			int UserId, int ResourceId, int ProjectId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "ActualFinishDate"},
				"ToDoAndTasksGetManagedByResource",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ResourceId", SqlDbType.Int, ResourceId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListToDoAndTaskResourcesByManager
		/// <summary>
		/// UserId, FirstName, LastName, FullName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTaskResourcesByManager(int ManagerId)
		{
			return DbHelper2.RunSpDataReader("ToDoAndTaskResourcesGetByManager",
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId));
		}
		#endregion

		#region GetListToDoAndTaskPastDueByProject
		/// <summary>
		/// ItemId, Title, Description, IsToDo, FinishDate
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTaskPastDueByProject(int ProjectId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"FinishDate"},
				"ToDoAndTasksGetPastDueByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListToDoAndTaskPastDueByProjectDataTable
		/// <summary>
		/// ItemId, Title, Description, IsToDo, FinishDate
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTaskPastDueByProjectDataTable(int ProjectId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"FinishDate"},
				"ToDoAndTasksGetPastDueByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListToDoAndTaskResourcesCount
		/// <summary>
		/// UserId, TaskTodoCount, FirstName, LastName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoAndTaskResourcesCount(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("ToDoAndTaskResourcesGetCount",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListToDoAndTaskResourcesCountDataTable
		/// <summary>
		/// UserId, TaskTodoCount, FirstName, LastName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTaskResourcesCountDataTable(int ProjectId)
		{
			return DbHelper2.RunSpDataTable("ToDoAndTaskResourcesGetCount",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListCompletedToDoAndTasks
		/// <summary>
		/// ItemId, Title, PriorityId, PriorityName, IsToDo, CreationDate, ActualFinishDate
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListCompletedToDoAndTasks(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate", "ActualFinishDate"},
				"ToDoAndTasksGetCompletedByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListProjectActivitiesCreators
		/// <summary>
		/// UserId, FirstName, LastName, FullName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListProjectActivitiesCreators(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("ProjectActivitiesGetCreators",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListProjectActivitiesManagers
		/// <summary>
		/// UserId, FirstName, LastName, FullName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListProjectActivitiesManagers(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("ProjectActivitiesGetManagers",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region AddTaskToDo
		public static void AddTaskToDo(int TaskId, int ToDoId, bool CompleteTask)
		{
			DbHelper2.RunSp("TaskToDoAdd",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId),
				DbHelper2.mp("@CompleteTask", SqlDbType.Bit, CompleteTask));
		}
		#endregion

		#region GetContainerManagers
		/// <summary>
		/// Returns fields: UserId
		/// </summary>
		public static IDataReader GetContainerManagers(int ToDoId)
		{
			return DbHelper2.RunSpDataReader("ToDoGetContainerManagers",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId));
		}
		#endregion

		#region GetListActiveToDoAndTasksByUserOnlyDataTable
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, IsToDo, CreationDate, StartDate, FinishDate, 
		///	PercentCompleted, IsResource, IsManager, StateId, ManagerId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListActiveToDoAndTasksByUserOnlyDataTable(int UserId, 
			int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate", "StartDate", "FinishDate"},
				"ToDoAndTasksGetActiveByUserOnly",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListToDoAndTasksForManagerViewDataTable
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate, 
		/// ActualStartDate, ActualFinishDate, PercentCompleted, 
		/// TaskTime, TotalMinutes, TotalApproved, IsCompleted, ManagerId, ReasonId, 
		/// ProjectId, ProjectTitle,
		/// StateId, CompletionTypeId, IsOverdue, ContactUid, OrgUid, ClientName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksForManagerViewDataTable(int PrincipalId,
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
				TimeZoneId, new string[] { "CreationDate", "StartDate", "FinishDate", "ActualStartDate", "ActualFinishDate" },
				"ToDoAndTasksGetForManagerView",
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

		#region GetListToDoAndTasksForManagerViewWithChildTodo
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate, 
		/// ActualStartDate, ActualFinishDate, PercentCompleted, 
		/// TaskTime, TotalMinutes, TotalApproved, IsCompleted, ManagerId, ReasonId, 
		/// ProjectId, ProjectTitle,
		/// StateId, CompletionTypeId, T.TaskTime, TotalMinutes, TotalApproved, 
		/// ContainerName, ContainerType, IsChildToDo, IsOverdue,
		/// IsOverdue, ContactUid, OrgUid, ClientName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksForManagerViewWithChildTodo(int PrincipalId,
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
				TimeZoneId, new string[] { "CreationDate", "StartDate", "FinishDate", "ActualStartDate", "ActualFinishDate" },
				"ToDoAndTasksGetForManagerViewWithChildToDo",
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

		#region GetListToDoAndTasksForManagerViewWithCategories
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate, 
		/// ActualStartDate, ActualFinishDate, PercentCompleted, 
		/// TaskTime, TotalMinutes, TotalApproved, IsCompleted, ManagerId, ReasonId, 
		/// ProjectId, ProjectTitle,
		/// StateId, CompletionTypeId, IsOverdue, ContactUid, OrgUid, ClientName,
		/// CategoryId, CategoryName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksForManagerViewWithCategories(int PrincipalId,
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
				TimeZoneId, new string[] { "CreationDate", "StartDate", "FinishDate", "ActualStartDate", "ActualFinishDate" },
				"ToDoAndTasksGetForManagerViewWithCategories",
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

		#region GetListToDoAndTasksForManagerViewWithChildTodoWithCategories
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate, 
		/// ActualStartDate, ActualFinishDate, PercentCompleted, 
		/// TaskTime, TotalMinutes, TotalApproved, IsCompleted, ManagerId, ReasonId, 
		/// ProjectId, ProjectTitle,
		/// StateId, CompletionTypeId, T.TaskTime, TotalMinutes, TotalApproved, 
		/// ContainerName, ContainerType, IsChildToDo, IsOverdue,
		/// IsOverdue, ContactUid, OrgUid, ClientName, CategoryId, CategoryName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksForManagerViewWithChildTodoWithCategories(int PrincipalId,
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
				TimeZoneId, new string[] { "CreationDate", "StartDate", "FinishDate", "ActualStartDate", "ActualFinishDate" },
				"ToDoAndTasksGetForManagerViewWithChildToDoWithCategories",
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


		#region GetListToDoAndTasksForManagerViewGroupedByUser
		/// <summary>
		///	PrincipalId, FirstName, LastName, ItemId, Title, PriorityId, PriorityName, ItemType, 
		///	CreationDate, StartDate, FinishDate, ActualStartDate, ActualFinishDate,
		/// TaskTime, TotalMinutes, TotalApproved,
		/// PercentCompleted, IsCompleted, ManagerId, ReasonId, 
		///	ProjectId, ProjectTitle, StateId, CompletionTypeId, HasRecurrence,
		/// TaskTime, TotalMinutes, TotalApproved, IsOverdue, IsNewMessage, 
		/// ContactUid, OrgUid, ClientName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksForManagerViewGroupedByUser(int PrincipalId,
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
				TimeZoneId, new string[] { "CreationDate", "StartDate", "FinishDate", "ActualStartDate", "ActualFinishDate" },
				"ObjectsGetForManagerViewGroupedByUser",
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

		#region GetListToDoAndTasksForManagerViewGroupedByUserWithChildTodo
		/// <summary>
		///	PrincipalId, FirstName, LastName, ItemId, Title, PriorityId, PriorityName, ItemType, 
		///	CreationDate, StartDate, FinishDate, ActualStartDate, ActualFinishDate,
		/// TaskTime, TotalMinutes, TotalApproved,
		/// PercentCompleted, IsCompleted, ManagerId, ReasonId, 
		///	ProjectId, ProjectTitle,StateId, CompletionTypeId, HasRecurrence,
		/// TaskTime, TotalMinutes, TotalApproved, ContainerName, ContainerType, 
		/// IsChildToDo, IsOverdue, IsNewMessage, ContactUid, OrgUid, ClientName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksForManagerViewGroupedByUserWithChildTodo(int PrincipalId,
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
				TimeZoneId, new string[] { "CreationDate", "StartDate", "FinishDate", "ActualStartDate", "ActualFinishDate" },
				"ObjectsGetForManagerViewGroupedByUserWithChildToDo",
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

		#region GetListToDoAndTasksForManagerViewGroupedByCategory
		/// <summary>
		///	CategoryId, CategoryName, ItemId, Title, PriorityId, PriorityName, ItemType, 
		///	CreationDate, StartDate, FinishDate, PercentCompleted, IsCompleted, ManagerId, ReasonId, 
		///	ProjectId, StateId, CompletionTypeId, HasRecurrence, IsOverdue, IsNewMessage, ItemCode
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksForManagerViewGroupedByCategory(int PrincipalId, 
			int TimeZoneId, int LanguageId, int ManagerId, int ProjectId, bool ShowActive,
			DateTime dtCompleted, DateTime dtUpcoming)
		{
			object obj_dtCompleted = (dtCompleted <= DateTime.MinValue.AddDays(1))? null: (object)dtCompleted;
			object obj_dtUpcoming = (dtUpcoming >= DateTime.MaxValue.AddDays(-1))? null: (object)dtUpcoming;
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate", "StartDate", "FinishDate"},
				"ObjectsGetForManagerViewGroupedByCategory",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@ShowActive", SqlDbType.Bit, ShowActive),
				DbHelper2.mp("@CompletedDate", SqlDbType.DateTime, obj_dtCompleted),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, obj_dtUpcoming));
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
			int resourceId, int priorityId, string keyword, int userId,
			int timeZoneId, int languageId, int genCategoryType, 
			PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			keyword = DBCommon.ReplaceSqlWildcard(keyword);
			//

			return DbHelper2.RunSpDataTable(
				timeZoneId, new string[] { "CreationDate" },
				"ToDoGetByFilterGroupedByClient",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, managerId),
				DbHelper2.mp("@ResourceId", SqlDbType.Int, resourceId),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, priorityId),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, keyword),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, languageId),
				DbHelper2.mp("@CategoryType", SqlDbType.Int, genCategoryType),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid));
		}
		#endregion

		#region CollapseByClient
		public static void CollapseByClient(int UserId, PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			DbHelper2.RunSp("CollapsedToDoByClientAdd",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion

		#region ExpandByClient
		public static void ExpandByClient(int UserId, PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			DbHelper2.RunSp("CollapsedToDoByClientDelete",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion
			
		#region GetListToDoAndTasksForResourceViewDataTable
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate, 
		/// ActualStartDate, ActualFinishDate, PercentCompleted, 
		/// TaskTime, TotalMinutes, TotalApproved, 
		/// IsCompleted, ManagerId, ReasonId, ProjectId, StateId, CompletionTypeId, IsOverdue,
		/// ContactUid, OrgUid, ClientName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksForResourceViewDataTable(int userId,
			int timeZoneId, int languageId, int managerId, int projectId, int categoryId,
			bool showActive, DateTime dtCompleted, DateTime dtUpcoming, 
			PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			object obj_dtCompleted = (dtCompleted <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCompleted;
			object obj_dtUpcoming = (dtUpcoming >= DateTime.MaxValue.AddDays(-1)) ? null : (object)dtUpcoming;
			return DbHelper2.RunSpDataTable(
				timeZoneId, new string[] { "CreationDate", "StartDate", "FinishDate", "ActualStartDate", "ActualFinishDate" },
				"ToDoAndTasksGetForResourceView",
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

		#region GetListToDoAndTasksForResourceViewWithCategories
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate, 
		/// ActualStartDate, ActualFinishDate, PercentCompleted, 
		/// TaskTime, TotalMinutes, TotalApproved, 
		/// IsCompleted, ManagerId, ReasonId, ProjectId, StateId, CompletionTypeId, IsOverdue,
		/// ContactUid, OrgUid, ClientName, CategoryId, CategoryName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoAndTasksForResourceViewWithCategories(int userId,
			int timeZoneId, int languageId, int managerId, int projectId, int categoryId,
			bool showActive, DateTime dtCompleted, DateTime dtUpcoming,
			PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			object obj_dtCompleted = (dtCompleted <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCompleted;
			object obj_dtUpcoming = (dtUpcoming >= DateTime.MaxValue.AddDays(-1)) ? null : (object)dtUpcoming;
			return DbHelper2.RunSpDataTable(
				timeZoneId, new string[] { "CreationDate", "StartDate", "FinishDate", "ActualStartDate", "ActualFinishDate" },
				"ToDoAndTasksGetForResourceViewWithCategories",
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

		#region GetToDoSecurity
		/// <summary>
		/// UserId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetToDoSecurity(int ToDoId)
		{
			return DbHelper2.RunSpDataReader("ToDoSecurityGet",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId));
		}
		#endregion

		#region GetManager
		public static int GetManager(int todoId)
		{
			return DbHelper2.RunSpInteger("TodoManagerGet",
				DbHelper2.mp("@TodoId", SqlDbType.Int, todoId));
		}
		#endregion

		#region ToDoRecalculateState
		/// <summary>
		/// OldState, NewState
		/// </summary>
		/// <returns></returns>
		public static IDataReader ToDoRecalculateState(int ToDoId, DateTime DT)
		{
			return DbHelper2.RunSpDataReader("ToDoRecalculateState",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId),
				DbHelper2.mp("@DateNow", SqlDbType.DateTime, DT)
				);
		}
		#endregion

		#region GetManagers
		/// <summary>
		/// </summary>
		/// <returns>PrincipalId, FirstName, LastName, FullName, DisplayName</returns>
		public static IDataReader GetManagers()
		{
			return DbHelper2.RunSpDataReader("ManagersGet");
		}
		#endregion

		#region UpdateCompletedBy
		public static void UpdateCompletedBy(int ToDoId, int UserId)
		{
			DbHelper2.RunSp("ToDoUpdateCompletedBy",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ResetCompletedBy
		public static void ResetCompletedBy(int ToDoId)
		{
			DbHelper2.RunSp("ToDoResetCompletedBy",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId));
		}
		#endregion

		#region UpdateActivation
		public static void UpdateActivation(int TodoId)
		{
			DbHelper2.RunSp("ToDoUpdateActivation",
				DbHelper2.mp("@TodoId", SqlDbType.Int, TodoId));
		}
		#endregion

		#region GetState(...)
		public static int GetState(int ObjectId)
		{
			return DBCommon.NullToInt32(
				DbHelper2.RunSpScalar("ToDoGetState"
				, DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId)
				));
		}
		#endregion

		#region GetListSuccessors
		/// <summary>
		/// Reader returns fields:
		///  ToDoId, StateId
		/// </summary>
		/// <param name="ToDoId"></param>
		/// <returns></returns>
		public static IDataReader GetListSuccessors(int ToDoId)
		{
			return DbHelper2.RunSpDataReader("ToDoGetSuccessors",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId));
		}
		#endregion

		#region UpdateActualStart
		public static void UpdateActualStart(int ToDoId)
		{
			DbHelper2.RunSp("ToDoUpdateActualStart",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, ToDoId));
		}
		#endregion

		#region GetListToDoAndTasksAssignedByUserDataTable
		/// <summary>
		///	ItemId, Title, IsCompleted, CompletionTypeId, ReasonId, IsToDo, StateId, 
		///	CreationDate, StartDate, FinishDate, ActualStartDate, ActualFinishDate, 
		/// SortFinishDate, PercentCompleted, PriorityId, ProjectId, ProjectTitle
		/// </summary>
		/// <returns>DataTable</returns>
		public static DataTable GetListToDoAndTasksAssignedByUserDataTable(
			int userId, bool showActive, DateTime fromDate, DateTime toDate, int timeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				timeZoneId,
				new string[] { "CreationDate", "StartDate", "FinishDate", "ActualStartDate", "ActualFinishDate" },
				"ToDoAndTasksGetAssignedByUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@ShowActive", SqlDbType.Bit, showActive),
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, toDate));
		}
		#endregion

		#region GetListToDoAndTasksAssignedToUserDataTable
		/// <summary>
		///	ItemId, Title, IsCompleted, CompletionTypeId, ReasonId, IsToDo, StateId, ManagerId,
		///	CreationDate, StartDate, FinishDate, ActualStartDate, ActualFinishDate, 
		/// SortFinishDate, PercentCompleted, PriorityId, PriorityName, ProjectId, ProjectTitle
		/// </summary>
		/// <returns>DataTable</returns>
		public static DataTable GetListToDoAndTasksAssignedToUserDataTable(
			int userId, bool showActive, DateTime fromDate, DateTime toDate, 
			int timeZoneId, int languageId)
		{
			return DbHelper2.RunSpDataTable(
				timeZoneId,
				new string[] { "CreationDate", "StartDate", "FinishDate", "ActualStartDate", "ActualFinishDate" },
				"ToDoAndTasksGetAssignedToUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@ShowActive", SqlDbType.Bit, showActive),
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, toDate),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, languageId));
		}
		#endregion

		#region GetListPredecessorsDetails
		/// <summary>
		/// Gets the list predecessors details.
		/// </summary>
		/// <param name="todoId">The todo id.</param>
		/// <param name="timeZoneId">The time zone id.</param>
		/// <returns>LinkId, ToDoId, Title, IsCompleted, CompletionTypeId, ReasonId, StartDate, FinishDate, PriorityId, PercentCompleted, StateId</returns>
		public static IDataReader GetListPredecessorsDetails(int todoId, int timeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				timeZoneId, new string[] { "StartDate", "FinishDate" },
				"ToDoGetPredecessorsDetails",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, todoId));
		}
		#endregion

		#region GetListSuccessorsDetails
		/// <summary>
		/// Gets the list successors details.
		/// </summary>
		/// <param name="todoId">The todo id.</param>
		/// <param name="timeZoneId">The time zone id.</param>
		/// <returns>LinkId, ToDoId, Title, CompletionTypeId, ReasonId, StartDate, FinishDate, PriorityId, PercentCompleted, IsCompleted, StateId</returns>
		public static IDataReader GetListSuccessorsDetails(int todoId, int timeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				timeZoneId, new string[] { "StartDate", "FinishDate" },
				"ToDoGetSuccessorsDetails",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, todoId));
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
			return DbHelper2.RunSpDataReader("ToDoGetVacantPredecessors",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, todoId));
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
			return DbHelper2.RunSpDataReader("ToDoGetVacantSuccessors",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, todoId));
		}
		#endregion

		#region CreateToDoLink
		/// <summary>
		/// Creates to do link.
		/// </summary>
		/// <param name="predecessorId">The predecessor id.</param>
		/// <param name="successorId">The successor id.</param>
		/// <returns>LinkId</returns>
		public static int CreateToDoLink(int predecessorId, int successorId)
		{
			return DbHelper2.RunSpInteger("ToDoLinkCreate",
				DbHelper2.mp("@PredId", SqlDbType.Int, predecessorId),
				DbHelper2.mp("@SuccId", SqlDbType.Int, successorId));
		}
		#endregion

		#region DeleteToDoLink
		/// <summary>
		/// Deletes todo link.
		/// </summary>
		/// <param name="linkId">The link id.</param>
		/// <returns></returns>
		public static int DeleteToDoLink(int linkId)
		{
			return DbHelper2.RunSpInteger("ToDoLinkDelete",
				DbHelper2.mp("@LinkId", SqlDbType.Int, linkId));
		}
		#endregion

		#region GetToDoLink
		/// <summary>
		/// Gets to do link.
		/// </summary>
		/// <param name="linkId">The link id.</param>
		/// <returns>LinkId, PredId, SuccId</returns>
		public static IDataReader GetToDoLink(int linkId)
		{
			return DbHelper2.RunSpDataReader("ToDoLinkGet",
				DbHelper2.mp("@LinkId", SqlDbType.Int, linkId));
		}
		#endregion

		#region GetListManagedToDoDataTable
		/// <summary>
		/// Datatable returns fields
		///  ToDoId, Title, ProjectId, ProjectTitle, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListManagedToDoDataTable(int userId, bool getActive, string keyword)
		{
			keyword = DBCommon.ReplaceSqlWildcard(keyword);

			return DbHelper2.RunSpDataTable(
				"ToDoGetManaged",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@GetActive", SqlDbType.Bit, getActive),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, keyword));
		}
		#endregion
	}
}
