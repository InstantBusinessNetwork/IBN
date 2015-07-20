using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBDocument.
	/// </summary>
	public class DBDocument
	{
		#region Create
		public static int Create(
			object ProjectId, int CreatorId, int ManagerId, 
			string Title, string Description,	
			DateTime CreationDate, int PriorityId, int StatusId, int TaskTime,
			object ContactUid, object OrgUid)
		{
			return DbHelper2.RunSpInteger("DocumentCreate",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, Title),
				DbHelper2.mp("@Description", SqlDbType.NText, Description),
				DbHelper2.mp("@CreationDate", SqlDbType.DateTime, CreationDate),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId),
				DbHelper2.mp("@StatusId", SqlDbType.Int, StatusId),
				DbHelper2.mp("@TaskTime", SqlDbType.Int, TaskTime),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, ContactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, OrgUid));
		}
		#endregion

		#region Delete
		public static void Delete(int DocumentId)
		{
			DbHelper2.RunSp("DocumentDelete", 
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId));
		}
		#endregion

		#region GetDocument
		/// <summary>
		///  DocumentId, ProjectId, ProjectTitle, CreatorId, ManagerId, 
		///  Title, Description, CreationDate, PriorityId, PriorityName, 
		///  StatusId, StatusName, IsCompleted, ReasonId, StateId, StateName,
		///  ContactUid, OrgUid, ClientName, TotalMinutes, TotalApproved, ProjectCode
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetDocument(int DocumentId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate"},
				"DocumentGet",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListDocumentStatus
		/// <summary>
		/// Reader returns fields:
		///  StatusId, StatusName, StateId 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListDocumentStatus()
		{
			return DbHelper2.RunSpDataReader("DocumentStatusGet");
		}
		#endregion

		#region GetListDocumentStatusForDictionaries
		/// <summary>
		///  ItemId, ItemName, StateId, StateName, CanDelete
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListDocumentStatusForDictionaries(int LanguageId)
		{
			return DbHelper2.RunSpDataTable("DocumentStatusGetForDictionaries",
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region AddDocumentStatus
		public static void AddDocumentStatus(string StatusName, int StateId)
		{
			DbHelper2.RunSp("DocumentStatusAdd", 
				DbHelper2.mp("@StatusName", SqlDbType.NVarChar, 50, StatusName),
				DbHelper2.mp("@StateId", SqlDbType.Int, StateId));
		}
		#endregion

		#region DeleteDocumentStatus
		public static void DeleteDocumentStatus(int StatusId)
		{
			DbHelper2.RunSp("DocumentStatusDelete", 
				DbHelper2.mp("@StatusId", SqlDbType.Int, StatusId));
		}
		#endregion

		#region UpdateDocumentStatus
		public static void UpdateDocumentStatus(int StatusId, string StatusName, int StateId)
		{
			DbHelper2.RunSp("DocumentStatusUpdate", 
				DbHelper2.mp("@StatusId", SqlDbType.Int, StatusId),
				DbHelper2.mp("@StatusName", SqlDbType.NVarChar, 50, StatusName),
				DbHelper2.mp("@StateId", SqlDbType.Int, StateId));
		}
		#endregion

		#region GetListDocumentsByProject
		/// <summary>
		///  DocumentId, ProjectId, ProjectTitle, CreatorId, CreatorName, 
		///  ManagerId, ManagerName, Title, CreationDate, PriorityId, PriorityName, 
		///  StatusId, StatusName, IsCompleted, StateId, CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListDocumentsByProject(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate"},
				"DocumentsGetByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}

		public static DataTable GetListDocumentsByProjectDataTable(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate"},
				"DocumentsGetByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListToDo
		/// <summary>
		///	 ToDoId, Title, Description, StartDate, FinishDate, ActualFinishDate, PercentCompleted, 
		///	 IsCompleted, CompletedBy, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDo(int DocumentId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "ActualFinishDate"},
				"ToDoGetByDocument",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}

		public static DataTable GetListToDoDataTable(int DocumentId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "ActualFinishDate"},
				"ToDoGetByDocument",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetProject
		public static int GetProject(int DocumentId)
		{
			return DbHelper2.RunSpInteger("DocumentGetProject", 
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId));
		}
		#endregion

		#region UpdateProjectAndManager
		public static void UpdateProjectAndManager(int DocumentId, int ProjectId, int ManagerId)
		{
			DbHelper2.RunSp("DocumentUpdateProjectAndManager",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId));
		}
		#endregion

		#region GetTitle
		public static string GetTitle(int DocumentId)
		{
			return (string)DbHelper2.RunSpScalar("DocumentGetTitle", 
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId));
		}
		#endregion

		#region GetListNotAssignedDocumentsDataTable
		/// <summary>
		///  DocumentId, Title, PriorityId, PriorityName, StatusId, StatusName, 
		///  CreationDate, CreatorId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListNotAssignedDocumentsDataTable(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate"},
				"DocumentsGetNotAssigned",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetSharingLevel
		public static int GetSharingLevel(int UserId, int DocumentId)
		{
			return DbHelper2.RunSpInteger("SharingGetLevelForDocument", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId));
		}
		#endregion

		#region GetListDocumentsByFilterDataTable
		/// <summary>
		/// DocumentId, ProjectId, ProjectTitle, CreatorId, CreatorName, ManagerId, 
		/// ManagerName, Title, CreationDate, PriorityId, PriorityName, StatusId, StatusName, 
		/// IsCompleted, StateId, CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListDocumentsByFilterDataTable(int ProjectId, int ManagerId,
			int ResourceId, int PriorityId, int StatusId, string Keyword, int UserId, 
			int TimeZoneId, int LanguageId,	int CategoryType, object contactUid, object orgUid)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate"},
				"DocumentsGetByFilter",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId),
				DbHelper2.mp("@ResourceId", SqlDbType.Int, ResourceId),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId),
				DbHelper2.mp("@StatusId", SqlDbType.Int, StatusId),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, Keyword),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@CategoryType", SqlDbType.Int, CategoryType),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion

		#region GetListDocumentsByFilterGroupedByProject
		/// <summary>
		/// DocumentId, ProjectId, ProjectTitle, CreatorId, ManagerId, 
		/// Title, CreationDate, PriorityId, PriorityName, StatusId, 
		/// StatusName, IsCompleted, StateId, CanEdit, CanDelete, IsProject, IsCollapsed
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListDocumentsByFilterGroupedByProject(int projectId,
			int managerId, int resourceId, int priorityId, int statusId,
			string keyword, int userId, int timeZoneId, int languageId,
			int categoryType, PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			keyword = DBCommon.ReplaceSqlWildcard(keyword);
			//

			return DbHelper2.RunSpDataTable(
				timeZoneId, new string[] { "CreationDate" },
				"DocumentsGetByFilterGroupedByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, managerId),
				DbHelper2.mp("@ResourceId", SqlDbType.Int, resourceId),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, priorityId),
				DbHelper2.mp("@StatusId", SqlDbType.Int, statusId),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, keyword),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, languageId),
				DbHelper2.mp("@CategoryType", SqlDbType.Int, categoryType),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid));
		}
		#endregion

		#region GetListDocumentsByFilterGroupedByClient
		/// <summary>
		/// ContactUid, OrgUid, IsClient, ClientName, ProjectId, ProjectTitle, 
		/// DocumentId, Title, CreatorId, CreatorName, ManagerId, ManagerName,
		/// CreationDate, PriorityId, PriorityName, StatusId, StatusName, 
		/// StateId, IsCompleted, CanEdit, CanDelete, IsCollapsed
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListDocumentsByFilterGroupedByClient(int projectId,
			int managerId, int resourceId, int priorityId, int statusId,
			string keyword, int userId, int timeZoneId, int languageId,
			int categoryType, PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			keyword = DBCommon.ReplaceSqlWildcard(keyword);
			//

			return DbHelper2.RunSpDataTable(
				timeZoneId, new string[] { "CreationDate" },
				"DocumentsGetByFilterGroupedByClient",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, managerId),
				DbHelper2.mp("@ResourceId", SqlDbType.Int, resourceId),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, priorityId),
				DbHelper2.mp("@StatusId", SqlDbType.Int, statusId),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, keyword),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, languageId),
				DbHelper2.mp("@CategoryType", SqlDbType.Int, categoryType),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid));
		}
		#endregion

		#region DeleteByProject
		public static void DeleteByProject(int ProjectId)
		{
			DbHelper2.RunSp("DocumentsDeleteByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListDocumentsUpdatedByUser
		/// <summary>
		///		DocumentId, Title, StateId, LastSavedDate
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListDocumentsUpdatedByUser(int ProjectId, int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"LastSavedDate"},
				"DocumentsGetUpdatedByUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}

		public static DataTable GetListDocumentsUpdatedByUserDataTable(int ProjectId, int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"LastSavedDate"},
				"DocumentsGetUpdatedByUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListDocumentsUpdatedForUser
		/// <summary>
		///		DocumentId, Title, CreatorId, ManagerId, LastEditorId, LastSavedDate, 
		///		ProjectId, ProjectName, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListDocumentsUpdatedForUser(int ProjectId, int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"LastSavedDate"},
				"DocumentsGetUpdatedForUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}

		public static DataTable GetListDocumentsUpdatedForUserDataTable(int ProjectId, int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"LastSavedDate"},
				"DocumentsGetUpdatedForUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListDocumentsByKeyword
		/// <summary>
		/// DocumentId, CreatorId, ManagerId, Title, Description, PriorityId, PriorityName, 
		/// StatusId, StatusName, CreationDate, IsCompleted, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListDocumentsByKeyword(
			int UserId, int TimeZoneId, int LanguageId, string Keyword)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate"},
				"DocumentsGetByKeyword",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, Keyword));
		}
		#endregion

		#region CheckForChangeableRoles
		public static int CheckForChangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("DocumentsCheckForChangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region CheckForUnchangeableRoles
		public static int CheckForUnchangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("DocumentsCheckForUnchangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListDocumentsForChangeableRoles
		/// <summary>
		///  DocumentId, Title, IsCompleted, ReasonId, StateId, IsManager, IsResource, 
		///  CanView, CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListDocumentsForChangeableRoles(int UserId, int CurrentUserId)
		{
			return DbHelper2.RunSpDataReader("DocumentsGetForChangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@CurrentUserId", SqlDbType.Int, CurrentUserId));
		}
		#endregion

		#region ReplaceUnchangeableUserToManager
		public static void ReplaceUnchangeableUserToManager(int UserId)
		{
			DbHelper2.RunSp("DocumentsReplaceUnchangeableUserToManager", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ReplaceUnchangeableUser
		public static void ReplaceUnchangeableUser(int FromUserId, int ToUserId)
		{
			DbHelper2.RunSp("DocumentsReplaceUnchangeableUser", 
				DbHelper2.mp("@FromUserId", SqlDbType.Int, FromUserId),
				DbHelper2.mp("@ToUserId", SqlDbType.Int, ToUserId));
		}
		#endregion

		#region AddResource
		public static void AddResource(int DocumentId, int PrincipalId, bool MustBeConfirmed, bool CanManage)
		{
			DbHelper2.RunSp("DocumentResourceAdd",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, MustBeConfirmed),
				DbHelper2.mp("@CanManage", SqlDbType.Bit, CanManage));
		}
		#endregion

		#region DeleteResource
		public static void DeleteResource(int DocumentId, int PrincipalId)
		{
			DbHelper2.RunSp("DocumentResourceDelete",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId));
		}
		#endregion

		#region GetResourceByUser
		/// <summary>
		///	 MustBeConfirmed, ResponsePending, IsConfirmed
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetResourceByUser(int DocumentId, int UserId)
		{
			return DbHelper2.RunSpDataReader("DocumentResourceGetByUser",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ReplyResource
		public static void ReplyResource(int DocumentId, int UserId, bool IsConfirmed, DateTime LastSavedDate)
		{
			DbHelper2.RunSp("DocumentResourceReply",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@IsConfirmed", SqlDbType.Bit, IsConfirmed),
				DbHelper2.mp("@LastSavedDate", SqlDbType.DateTime, LastSavedDate));
		}
		#endregion

		#region GetListResources
		/// <summary>
		///  ResourceId, DocumentId, PrincipalId, IsGroup, MustBeConfirmed, ResponsePending, 
		///  IsConfirmed, IsExternal, CanManage
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListResources(int DocumentId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"LastSavedDate"},
				"DocumentResourcesGet", 
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId));
		}

		public static DataTable GetListDocumentResourcesDataTable(int DocumentId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"LastSavedDate"},
				"DocumentResourcesGet", 
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId));
		}
		#endregion

		#region Collapse
		public static void Collapse(int UserId, int ProjectId)
		{
			DbHelper2.RunSp("CollapsedDocumentAdd",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region Expand
		public static void Expand(int UserId, int ProjectId)
		{
			DbHelper2.RunSp("CollapsedDocumentDelete",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region CollapseByClient
		public static void CollapseByClient(int UserId, PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			DbHelper2.RunSp("CollapsedDocumentByClientAdd",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion

		#region ExpandByClient
		public static void ExpandByClient(int UserId, PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			DbHelper2.RunSp("CollapsedDocumentByClientDelete",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion

		#region GetSecurityForUser
		/// <summary>
		///  DocumentId, PrincipalId, IsManager, IsCreator, IsResource, IsRealDocumentResource, IsCreator
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetSecurityForUser(int DocumentId, int UserId)
		{
			return DbHelper2.RunSpDataReader("DocumentGetSecurityForUser",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListActiveDocumentsByUserOnlyDataTable
		/// <summary>
		/// DocumentId, Title, CreationDate, PriorityId, PriorityName, IsManager, IsResource
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListActiveDocumentsByUserOnlyDataTable(int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate"},
				"DocumentsGetActiveByUserOnly",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListDocumentManagers
		/// <summary>
		///  UserId, UserName, UserName2
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListDocumentManagers()
		{
			return DbHelper2.RunSpDataReader("DocumentManagersGet");
		}
		#endregion

		#region GetManager
		public static int GetManager(int DocumentId)
		{
			return DbHelper2.RunSpInteger("DocumentManagerGet",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId));
		}
		#endregion

		#region GetIsCompleted
		public static int GetIsCompleted(int DocumentId)
		{
			return DbHelper2.RunSpInteger("DocumentGetIsCompleted",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId));
		}
		#endregion

		#region UpdateCompletion
		public static void UpdateCompletion(int DocumentId, bool IsCompleted, int ReasonId)
		{
			DbHelper2.RunSp("DocumentUpdateCompletion",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId),
				DbHelper2.mp("@IsCompleted", SqlDbType.Bit, IsCompleted),
				DbHelper2.mp("@ReasonId", SqlDbType.Int, ReasonId));
		}
		#endregion

		#region GetListDocumentsSimple
		/// <summary>
		///  Reader returns fields:
		///		DocumentId, Title
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListDocumentsSimple(int ProjectId, 
			int UserId, bool GetAssigned, bool GetManaged)
		{
			return DbHelper2.RunSpDataReader("DocumentsGetSimple",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@GetAssigned", SqlDbType.Bit, GetAssigned),
				DbHelper2.mp("@GetManaged", SqlDbType.Bit, GetManaged));
		}
		#endregion

		#region GetListPendingDocumentsDataTable
		/// <summary>
		///		DocumentId, Title, Description, PriorityId, PriorityName, ManagerId, CreationDate,
		///		StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListPendingDocumentsDataTable(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate"},
				"DocumentsGetPending",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListDocumentsForManagerViewDataTable
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate, 
		/// TaskTime, TotalMinutes, TotalApproved,
		///	PercentCompleted, IsCompleted, ManagerId, ReasonId, ProjectId, ProjectTitle,
		///	StateId, CompletionTypeId, ContactUid, OrgUid, ClientName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListDocumentsForManagerViewDataTable(int PrincipalId,
			int TimeZoneId, int LanguageId, int ManagerId, int ProjectId, int categoryId, 
			bool ShowActive,
			DateTime dtCompleted1, DateTime dtCompleted2,
			DateTime dtCreated1, DateTime dtCreated2, 
			PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			object obj_dtCompleted1 = (dtCompleted1 <= DateTime.MinValue.AddDays(1))? null: (object)dtCompleted1;
			object obj_dtCompleted2 = (dtCompleted2 <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCompleted2;
			object obj_dtCreated1 = (dtCreated1 <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCreated1;
			object obj_dtCreated2 = (dtCreated2 <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCreated2;
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate", "StartDate", "FinishDate"},
				"DocumentsGetForManagerView",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@CategoryId", SqlDbType.Int, categoryId),
				DbHelper2.mp("@ShowActive", SqlDbType.Bit, ShowActive),
				DbHelper2.mp("@CompletedDate1", SqlDbType.DateTime, obj_dtCompleted1),
				DbHelper2.mp("@CompletedDate2", SqlDbType.DateTime, obj_dtCompleted2),
				DbHelper2.mp("@CreationDate1", SqlDbType.DateTime, obj_dtCreated1),
				DbHelper2.mp("@CreationDate2", SqlDbType.DateTime, obj_dtCreated2),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid));
		}
		#endregion

		#region GetListDocumentsForManagerViewWithChildTodo
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate, 
		/// TaskTime, TotalMinutes, TotalApproved,
		///	PercentCompleted, IsCompleted, ManagerId, ReasonId, ProjectId, ProjectTitle,
		///	StateId, CompletionTypeId, ContainerName, ContactUid, OrgUid, ClientName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListDocumentsForManagerViewWithChildTodo(int PrincipalId,
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
				TimeZoneId, new string[] { "CreationDate", "StartDate", "FinishDate" },
				"DocumentsGetForManagerViewWithChildToDo",
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

		#region GetListDocumentsForManagerViewWithCategories
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate, 
		/// TaskTime, TotalMinutes, TotalApproved,
		///	PercentCompleted, IsCompleted, ManagerId, ReasonId, ProjectId, ProjectTitle,
		///	StateId, CompletionTypeId, ContactUid, OrgUid, ClientName, CategoryId, CategoryName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListDocumentsForManagerViewWithCategories(int PrincipalId,
			int TimeZoneId, int LanguageId, int ManagerId, int ProjectId, int categoryId,
			bool ShowActive,
			DateTime dtCompleted1, DateTime dtCompleted2,
			DateTime dtCreated1, DateTime dtCreated2,
			PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			object obj_dtCompleted1 = (dtCompleted1 <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCompleted1;
			object obj_dtCompleted2 = (dtCompleted2 <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCompleted2;
			object obj_dtCreated1 = (dtCreated1 <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCreated1;
			object obj_dtCreated2 = (dtCreated2 <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCreated2;
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "CreationDate", "StartDate", "FinishDate" },
				"DocumentsGetForManagerViewWithCategories",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@CategoryId", SqlDbType.Int, categoryId),
				DbHelper2.mp("@ShowActive", SqlDbType.Bit, ShowActive),
				DbHelper2.mp("@CompletedDate1", SqlDbType.DateTime, obj_dtCompleted1),
				DbHelper2.mp("@CompletedDate2", SqlDbType.DateTime, obj_dtCompleted2),
				DbHelper2.mp("@CreationDate1", SqlDbType.DateTime, obj_dtCreated1),
				DbHelper2.mp("@CreationDate2", SqlDbType.DateTime, obj_dtCreated2),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid));
		}
		#endregion

		#region GetListDocumentsForManagerViewWithChildTodoWithCategories
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate, 
		/// TaskTime, TotalMinutes, TotalApproved,
		///	PercentCompleted, IsCompleted, ManagerId, ReasonId, ProjectId, ProjectTitle,
		///	StateId, CompletionTypeId, ContainerName, ContactUid, OrgUid, ClientName, 
		///	CategoryId, CategoryName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListDocumentsForManagerViewWithChildTodoWithCategories(int PrincipalId,
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
				TimeZoneId, new string[] { "CreationDate", "StartDate", "FinishDate" },
				"DocumentsGetForManagerViewWithChildToDoWithCategories",
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


		#region GetListDocumentsForResourceViewDataTable
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate, 
		/// TaskTime, TotalMinutes, TotalApproved,
		///	PercentCompleted, IsCompleted, ManagerId, ReasonId, ProjectId, StateId, CompletionTypeId,
		///	ContactUid, OrgUid, ClientName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListDocumentsForResourceViewDataTable(int userId,
			int timeZoneId, int languageId, int managerId, int projectId, int categoryId,
			bool showActive, DateTime dtCompleted, DateTime dtUpcoming, 
			PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			object obj_dtCompleted = (dtCompleted <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCompleted;
			object obj_dtUpcoming = (dtUpcoming >= DateTime.MaxValue.AddDays(-1)) ? null : (object)dtUpcoming;
			return DbHelper2.RunSpDataTable(
				timeZoneId, new string[] { "CreationDate", "StartDate", "FinishDate" },
				"DocumentsGetForResourceView",
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

		#region GetListDocumentsForResourceViewWithCategories
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate, 
		/// TaskTime, TotalMinutes, TotalApproved,
		///	PercentCompleted, IsCompleted, ManagerId, ReasonId, ProjectId, StateId, CompletionTypeId,
		///	ContactUid, OrgUid, ClientName, CategoryId, CategoryName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListDocumentsForResourceViewWithCategories(int userId,
			int timeZoneId, int languageId, int managerId, int projectId, int categoryId,
			bool showActive, DateTime dtCompleted, DateTime dtUpcoming,
			PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			object obj_dtCompleted = (dtCompleted <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCompleted;
			object obj_dtUpcoming = (dtUpcoming >= DateTime.MaxValue.AddDays(-1)) ? null : (object)dtUpcoming;
			return DbHelper2.RunSpDataTable(
				timeZoneId, new string[] { "CreationDate", "StartDate", "FinishDate" },
				"DocumentsGetForResourceViewWithCategories",
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

		#region GetDocumentStateByStatus
		public static int GetDocumentStateByStatus(int StatusId)
		{
			return DbHelper2.RunSpInteger("DocumentStateGetByStatus",
				DbHelper2.mp("@StatusId", SqlDbType.Int, StatusId));
		}
		#endregion

		#region UpdateActivation
		public static void UpdateActivation(int DocumentId)
		{
			DbHelper2.RunSp("DocumentUpdateActivation",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId));
		}
		#endregion

		#region DocumentRecalculateState
		/// <summary>
		/// OldState, NewState
		/// </summary>
		/// <returns></returns>
		public static IDataReader DocumentRecalculateState(int DocumentId)
		{
			return DbHelper2.RunSpDataReader("DocumentRecalculateState",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId));
		}

		/// <summary>
		/// OldState, NewState
		/// </summary>
		/// <param name="DocumentId">The document id.</param>
		/// <returns></returns>
		public static DataTable DocumentRecalculateStateDataTable(int DocumentId)
		{
			return DbHelper2.RunSpDataTable("DocumentRecalculateState",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId));
		}
		#endregion

		#region ReadOnly
		/// <summary>
		/// Determines whether [is read only] [the specified document id].
		/// </summary>
		/// <param name="documentId">The document id.</param>
		/// <returns>
		/// 	<c>true</c> if [is read only] [the specified document id]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsReadOnly(int documentId)
		{
			return DbHelper2.RunSpInteger("DocumentIsReadOnly",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, documentId))!=0;
		}

		/// <summary>
		/// Sets the read only.
		/// </summary>
		/// <param name="documentId">The document id.</param>
		/// <param name="readOnly">if set to <c>true</c> [read only].</param>
		public static void SetReadOnly(int documentId, bool readOnly)
		{
			DbHelper2.RunSp("DocumentSetReadOnly",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, documentId),
				DbHelper2.mp("@ReadOnly", SqlDbType.Bit, readOnly));
		}
		#endregion

		#region GetDocumentSecurity
		/// <summary>
		/// UserId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetDocumentSecurity(int documentId)
		{
			return DbHelper2.RunSpDataReader("DocumentSecurityGet",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, documentId));
		}
		#endregion

		#region GetListDocumentsByProjectAll
		/// <summary>
		/// Gets the list documents by project all.
		/// DocumentId, CreatorId, ManagerId, Title, CreationDate, PriorityId, StatusId, IsCompleted, StateId, ReasonId
		/// </summary>
		/// <param name="ProjectId">The project id.</param>
		/// <returns></returns>
		public static IDataReader GetListDocumentsByProjectAll(int projectId)
		{
			return DbHelper2.RunSpDataReader("DocumentsGetByProjectAll",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId));
		} 
		#endregion
	}
}
