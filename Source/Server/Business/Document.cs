using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Security;

using Mediachase.Ibn;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Database;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.MetaDataPlus;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for Document.
	/// </summary>
	public class Document
	{
		#region DocumentSecurity	
		public class DocumentSecurity
		{
			public bool IsManager = false;
			public bool IsResource = false;
			public bool IsRealDocumentResource = false;
			public bool IsCreator = false;

			public DocumentSecurity(int document_id, int user_id)
			{
				using(IDataReader reader = DBDocument.GetSecurityForUser(document_id, user_id))
				{
					if (reader.Read())
					{
						IsManager = ((int)reader["IsManager"] > 0)?true:false;
						IsResource = ((int)reader["IsResource"] > 0)?true:false;
						IsRealDocumentResource = ((int)reader["IsRealDocumentResource"] > 0)?true:false;
						IsCreator = ((int)reader["IsCreator"] > 0) ? true : false;
					}
				}
			}
		}
		#endregion

		#region DOCUMENT_TYPE
		internal static ObjectTypes DOCUMENT_TYPE
		{
			get {return ObjectTypes.Document;}
		}
		#endregion

		#region Tracking
		public struct Tracking
		{
			public bool ShowAcceptDeny;
			public bool ShowComplete;
			public bool ShowSuspend;
			public bool ShowUncomplete;
			public bool ShowResume;
			public bool ShowTimeTracking;
			public bool ShowActivate;
			public bool ShowStatus;
			public bool ShowStatusAndTimeTracking;
			public bool ShowAddNewVersion;

			public Tracking(
				bool showAcceptDeny, 
				bool showComplete, 
				bool showSuspend,
				bool showUncomplete, 
				bool showResume,
				bool showTimeTracking,
				bool showActivate,
				bool showStatus,
				bool showStatusAndTimeTracking,
				bool showAddNewVersion)
			{
				ShowAcceptDeny = showAcceptDeny;
				ShowComplete = showComplete;
				ShowSuspend = showSuspend;
				ShowUncomplete = showUncomplete;
				ShowResume = showResume;
				ShowTimeTracking = showTimeTracking;
				ShowActivate = showActivate;
				ShowStatus = showStatus;
				ShowStatusAndTimeTracking = showStatusAndTimeTracking;
				ShowAddNewVersion = showAddNewVersion;
			}
		}
		#endregion

		#region CanCreate
		public static bool CanCreate(int projectId)
		{
			bool RetVal = false;

			using(IDataReader reader = DBUser.GetUserInfo(Security.CurrentUser.UserID))
			{
				if (reader.Read())
					RetVal = (bool)reader["IsActive"];
			}

			if (RetVal)
			{
				if (projectId > 0)
				{
					Project.ProjectSecurity ps = Project.GetSecurity(projectId);
					Dictionary<string, string> prop = Project.GetProperties(projectId);

					RetVal = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) 
						|| ps.IsManager
						|| (ps.IsTeamMember && prop.ContainsKey(Project.CanProjectTeamCreateDocument))
						|| (ps.IsSponsor && prop.ContainsKey(Project.CanProjectSponsorCreateDocument))
						|| (ps.IsStakeHolder && prop.ContainsKey(Project.CanProjectStakeholderCreateDocument))
						|| (ps.IsExecutiveManager && prop.ContainsKey(Project.CanProjectExecutiveCreateDocument));
				}
			}

			return RetVal;
		}
		#endregion

		#region CanUpdate
		public static bool CanUpdate(int document_id)
		{
			bool retval = false;
			DocumentSecurity docs = GetSecurity(document_id);
			retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				docs.IsManager || docs.IsCreator;

			if (!retval)	// check by project
			{
				int projectId = DBDocument.GetProject(document_id);
				if (projectId > 0)
				{
					Project.ProjectSecurity ps = Project.GetSecurity(projectId);
					retval = ps.IsManager; 
				}
			}

			return retval;
		}
		#endregion

		#region CanDelete
		public static bool CanDelete(int document_id)
		{
			return CanUpdate(document_id);
		}
		#endregion

		#region CanUpdateStatus
		public static bool CanUpdateStatus(int document_id)
		{
			// Изменять статус могут: PPM, Document Manager, Document Resources, ProjectManager
			bool retval = false;
			DocumentSecurity docs = GetSecurity(document_id);
			retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)
				|| docs.IsManager || docs.IsRealDocumentResource || docs.IsCreator;

			if (!retval)	// check by project
			{
				int projectId = DBDocument.GetProject(document_id);
				if (projectId > 0)
				{
					Project.ProjectSecurity ps = Project.GetSecurity(projectId);
					retval = ps.IsManager;
				}
			}

			return retval;
		}
		#endregion

		#region CanAddVersion
		public static bool CanAddVersion(int document_id)
		{
			bool retval = false;
			using (IDataReader reader = GetDocument(document_id, false))
			{
				if (reader.Read())
				{
					if ((int)reader["StateId"] == (int)ObjectStates.Active || (int)reader["StateId"] == (int)ObjectStates.Overdue)
						retval = true;
				}
			}

			if (retval)
			{
				DocumentSecurity docs = GetSecurity(document_id);
				retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) 
					|| docs.IsManager || docs.IsRealDocumentResource || docs.IsCreator;

				if (!retval)	// check by project
				{
					int projectId = DBDocument.GetProject(document_id);
					if (projectId > 0)
					{
						Project.ProjectSecurity ps = Project.GetSecurity(projectId);
						retval = ps.IsManager;
					}
				}
			}

			return retval;
		}
		#endregion

		#region CanRead(int document_id)
		public static bool CanRead(int document_id)
		{
			bool retval = false;
			int UserId = Security.CurrentUser.UserID;

			DocumentSecurity docs = GetSecurity(document_id);
			int ProjectId = DBDocument.GetProject(document_id);
			if (ProjectId > 0)
			{
				Project.ProjectSecurity ps = Project.GetSecurity(ProjectId);
				
				retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) 
					|| Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager)
					|| docs.IsManager || docs.IsResource || docs.IsCreator
					|| ps.IsManager || ps.IsTeamMember || ps.IsExecutiveManager || ps.IsSponsor || ps.IsStakeHolder 
					|| Security.CurrentUser.IsAlertService
					|| (DBProject.GetSharingLevel(UserId, ProjectId) >= 0);
			}
			else
			{
				retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) 
					|| Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) 
					|| docs.IsManager || docs.IsResource || docs.IsCreator
					|| Security.CurrentUser.IsAlertService
					|| (DBDocument.GetSharingLevel(UserId, document_id) >= 0);
			}

			return retval;
		}
		#endregion

		#region CanRead(int document_id, int user_id)
		public static bool CanRead(int document_id, int user_id)
		{
			bool retval = false;

			DocumentSecurity docs = GetSecurity(document_id, user_id);
			int ProjectId = DBDocument.GetProject(document_id);
			if (ProjectId > 0)
			{
				Project.ProjectSecurity ps = Project.GetSecurity(ProjectId, user_id);
				ArrayList secure_groups = User.GetListSecureGroupAll(user_id);
				
				retval = secure_groups.Contains((int)InternalSecureGroups.PowerProjectManager)
					|| secure_groups.Contains((int)InternalSecureGroups.ExecutiveManager)
					|| docs.IsManager || docs.IsResource || docs.IsCreator
					|| ps.IsManager || ps.IsTeamMember || ps.IsExecutiveManager || ps.IsSponsor || ps.IsStakeHolder 
					|| Security.CurrentUser.IsAlertService
					|| (DBProject.GetSharingLevel(user_id, ProjectId) >= 0);
			}
			else
			{
				retval = docs.IsManager || docs.IsResource || docs.IsCreator
					|| Security.CurrentUser.IsAlertService
					|| (DBDocument.GetSharingLevel(user_id, document_id) >= 0);
			}

			return retval;
		}
		#endregion

		#region CanViewToDoList
		public static bool CanViewToDoList(int document_id)
		{
			// Видеть ToDo List могут: PPM, Exec, Document Manager, Document Creator, Document Resources,
			//	участники проекта (Project!=null) 

			bool retval = false;

			DocumentSecurity sec = GetSecurity(document_id);
			retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) ||
				sec.IsManager || sec.IsResource || sec.IsCreator;

			if (!retval)
			{
				int ProjectId = DBDocument.GetProject(document_id);
				if (ProjectId > 0)
				{
					Project.ProjectSecurity ps = Project.GetSecurity(ProjectId);
					retval = ps.IsTeamMember || ps.IsExecutiveManager || ps.IsSponsor || ps.IsStakeHolder;
				}
			}
			return retval;
		}
		#endregion

		#region CanAddToDo
		public static bool CanAddToDo(int document_id)
		{
			bool retval = false;

			DocumentSecurity sec = GetSecurity(document_id);
			retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				sec.IsManager;

			if (!retval)	// check by project
			{
				int projectId = DBDocument.GetProject(document_id);
				if (projectId > 0)
				{
					Project.ProjectSecurity ps = Project.GetSecurity(projectId);
					retval = ps.IsManager;
				}
			}

			return retval;
		}
		#endregion

		#region CanDeleteToDo
		public static bool CanDeleteToDo(int document_id)
		{
			return CanAddToDo(document_id);
		}
		#endregion

		#region CanModifyResources
		public static bool CanModifyResources(int document_id)
		{
			return CanAddToDo(document_id);
		}
		#endregion

		#region CanViewFinances
		public static bool CanViewFinances(int document_id)
		{
			bool retval = false;

			int projectId = DBDocument.GetProject(document_id);
			if (projectId > 0)
			{
				retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)
					|| Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager)
					|| GetSecurity(document_id).IsManager;

				if (!retval)	// check by project
				{
					Project.ProjectSecurity ps = Project.GetSecurity(projectId);
					retval = ps.IsManager;
				}
			}

			return retval;
		}
		#endregion

		#region GetSecurity
		public static DocumentSecurity GetSecurity(int document_id)
		{
			return GetSecurity(document_id, Security.CurrentUser.UserID);
		}

		public static DocumentSecurity GetSecurity(int document_id, int user_id)
		{
			return new DocumentSecurity(document_id, user_id);
		}
		#endregion

		#region Create
		public static int Create(
			string title,
			string description,
			int project_id,
			int priority,
			int managerId,
			int status_id,
			int task_time,
			PrimaryKeyId ContactUid,
			PrimaryKeyId OrgUid)
		{
			ArrayList alCategories = Common.StringToArrayList(PortalConfig.DocumentDefaultValueGeneralCategoriesField);
			return Create(title, description, project_id, priority, managerId, status_id, task_time, alCategories, null, null, ContactUid, OrgUid);
		}

		public static int Create(
			string title,
			string description,
			int project_id,
			int priority,
			int managerId,
			int status_id,
			int task_time,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			PrimaryKeyId ContactUid,
			PrimaryKeyId OrgUid)
		{
			return Create(title, description, project_id, priority, managerId, status_id, task_time, categories, FileName, _inputStream, DateTime.UtcNow, ContactUid, OrgUid);
		}

		public static int Create(
			string title,
			string description,
			int project_id,
			int priority,
			int managerId,
			int status_id,
			int task_time,
			ArrayList categories,
			string FileName,
			System.IO.Stream _inputStream,
			DateTime creation_date,
			PrimaryKeyId ContactUid,
			PrimaryKeyId OrgUid)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (!CanCreate(project_id))
				throw new AccessDeniedException();

			// DateTime creation_date = DateTime.UtcNow;

			object oProjectId = null;
			if (project_id > 0)
			{
				oProjectId = project_id;
			}

			int UserId = Security.CurrentUser.UserID;
			int DocumentId = -1;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				DocumentId = DBDocument.Create(oProjectId, UserId, managerId, title, description, creation_date, priority, status_id, task_time,
					ContactUid == PrimaryKeyId.Empty ? null : (object)ContactUid,
					OrgUid == PrimaryKeyId.Empty ? null : (object)OrgUid);

				SystemEvents.AddSystemEvents(SystemEventTypes.Document_Created, DocumentId);
				if (project_id > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_DocumentList_DocumentAdded, project_id, DocumentId);

				// OZ: User Role Addon
				UserRoleHelper.AddDocumentCreatorRole(DocumentId, UserId);
				UserRoleHelper.AddDocumentManagerRole(DocumentId, managerId);
				if(project_id > 0)
				{
					ForeignContainerKey.Add(UserRoleHelper.CreateDocumentContainerKey(DocumentId),UserRoleHelper.CreateProjectContainerKey(project_id));
				}
				//

				// Categories
				foreach(int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)DOCUMENT_TYPE, DocumentId, CategoryId);
				}

				if(FileName!=null)
				{
					string ContainerName = "FileLibrary";
					string ContainerKey = UserRoleHelper.CreateDocumentContainerKey(DocumentId);

					BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
					FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
					fs.SaveFile(fs.Root.Id, FileName, _inputStream);
				}

				// O.R: Recalculating project TaskTime
				if (project_id > 0 && task_time > 0)
					TimeTracking.RecalculateProjectTaskTime(project_id);

				RecalculateState(DocumentId);

				tran.Commit();
			}

			return DocumentId;
		}
		#endregion

		#region Delete
		public static void Delete(int document_id)
		{
			Delete(document_id, true);
		}

		internal static void Delete(int document_id, bool checkAccess)
		{
			if(checkAccess && !CanDelete(document_id))
				throw new AccessDeniedException();

			ArrayList allRemovedToDo	=	new ArrayList();
			using(IDataReader todoReader = Document.GetListToDo(document_id))
			{
				while(todoReader.Read())
				{
					int ToDoId = (int)todoReader["ToDoId"];
					allRemovedToDo.Add(ToDoId);
				}
			}

			int projectId = DBDocument.GetProject(document_id);

			using(DbTransaction tran = DbTransaction.Begin())
			{
                // OZ: DocumentVers_
                BaseIbnContainer containerDV = BaseIbnContainer.Create("FileLibrary", string.Format("DocumentVers_{0}",document_id));
                FileStorage fsDV = (FileStorage)containerDV.LoadControl("FileStorage");
                fsDV.DeleteAll();
                //

				// OZ: File Storage
				BaseIbnContainer container = BaseIbnContainer.Create("FileLibrary", UserRoleHelper.CreateDocumentContainerKey(document_id));
				FileStorage fs = (FileStorage)container.LoadControl("FileStorage");
				fs.DeleteAll();
				//

				// OZ: User Roles
				UserRoleHelper.DeleteDocumentRoles(document_id);

				// Delete Todo and don't send todo alerts
				foreach(int ToDoId in allRemovedToDo)
				{
					ToDo.DeleteSimple(ToDoId);
				}

				MetaObject.Delete(document_id, "DocumentsEx");

				SystemEvents.AddSystemEvents(SystemEventTypes.Document_Deleted, document_id);
				if (projectId > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_DocumentList_DocumentDeleted, projectId, document_id);

				DBDocument.Delete(document_id);

				// O.R: Recalculating project TaskTime
				if (projectId > 0)
					TimeTracking.RecalculateProjectTaskTime(projectId);

				tran.Commit();
			}
		}
		#endregion

		#region GetDocument
		/// <summary>
		///  DocumentId, ProjectId, ProjectTitle, CreatorId, ManagerId, 
		///  Title, Description, CreationDate, PriorityId, PriorityName, 
		///  StatusId, StatusName, IsCompleted, ReasonId, StateId, StateName,
		///  ContactUid, OrgUid, ClientName,
		///  TaskTime, TotalMinutes, TotalApproved, ProjectCode
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetDocument(int document_id)
		{
			return GetDocument(document_id, true);
		}

		public static IDataReader GetDocument(int document_id, bool checkAccess)
		{
			if(checkAccess && !CanRead(document_id))
				throw new AccessDeniedException();

			return DBDocument.GetDocument(document_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
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
		public static IDataReader GetListCategories(int document_id)
		{
			return DBCommon.GetListCategoriesByObject((int)DOCUMENT_TYPE, document_id);
		}
		#endregion

		#region GetListDiscussions
		/// <summary>
		/// Reader returns fields:
		///		DiscussionId, ObjectTypeId, ObjectId, CreatorId, CreationDate, Text, CreatorName
		/// </summary>
		public static IDataReader GetListDiscussions(int document_id)
		{
			return Common.GetListDiscussions(DOCUMENT_TYPE, document_id, Security.CurrentUser.TimeZoneId);
		}

		public static DataTable GetListDiscussionsDataTable(int document_id)
		{
			return Common.GetListDiscussionsDataTable(DOCUMENT_TYPE, document_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region AddDiscussion
		public static void AddDiscussion(int document_id, string text)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			DateTime creation_date = DateTime.UtcNow;
			int UserId = Security.CurrentUser.UserID;

			using(DbTransaction tran = DbTransaction.Begin())
			{
				int CommentId = DBCommon.AddDiscussion((int)DOCUMENT_TYPE, document_id, UserId, creation_date, text);
				
				SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_CommentList_CommentAdded, document_id, CommentId);

				tran.Commit();
			}
		}
		#endregion

		#region GetListDocumentsByProject
		/// <summary>
		///  DocumentId, ProjectId, ProjectTitle, CreatorId, CreatorName, 
		///  ManagerId, ManagerName, Title, CreationDate, PriorityId, PriorityName, 
		///  StatusId, StatusName, IsCompleted, StateId, CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListDocumentsByProject(int project_id)
		{
			return DBDocument.GetListDocumentsByProject(project_id, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}

		public static DataTable GetListDocumentsByProjectDataTable(int project_id)
		{
			return DBDocument.GetListDocumentsByProjectDataTable(project_id, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListToDo
		/// <summary>
		///	 ToDoId, Title, Description, StartDate, FinishDate, ActualFinishDate, PercentCompleted, 
		///	 IsCompleted, CompletedBy, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDo(int document_id)
		{
			return DBDocument.GetListToDo(document_id, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}

		public static DataTable GetListToDoDataTable(int document_id)
		{
			return DBDocument.GetListToDoDataTable(document_id, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListDocumentStatus
		/// <summary>
		/// Reader returns fields:
		///  StatusId, StatusName 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListDocumentStatus()
		{
			return DBDocument.GetListDocumentStatus();
		}
		#endregion

		#region GetTitle
		public static string GetTitle(int document_id)
		{
			string documentTitle = DBDocument.GetTitle(document_id);
			if (documentTitle == null)
				documentTitle = "";
			return documentTitle;
		}
		#endregion

		#region GetListNotAssignedDocumentsDataTable
		/// <summary>
		///  DocumentId, Title, PriorityId, PriorityName, StatusId, StatusName, 
		///  CreationDate, CreatorId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListNotAssignedDocumentsDataTable(int ProjectId)
		{
			return DBDocument.GetListNotAssignedDocumentsDataTable(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
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
			int ResourceId, int PriorityId, int StatusId, string Keyword, int CategoryType, 
			PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			return DBDocument.GetListDocumentsByFilterDataTable(ProjectId, ManagerId, 
				ResourceId, PriorityId, StatusId, Keyword, 
				Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, 
				Security.CurrentUser.LanguageId, CategoryType,
				contactUid == PrimaryKeyId.Empty ? null : (object)contactUid,
				orgUid == PrimaryKeyId.Empty ? null : (object)orgUid);
		}
		#endregion

		#region GetListDocumentsByFilterGroupedByProject
		/// <summary>
		/// DocumentId, ProjectId, ProjectTitle, CreatorId, ManagerId, 
		/// Title, CreationDate, PriorityId, PriorityName, StatusId, 
		/// StatusName, IsCompleted, StateId, CanEdit, CanDelete, IsProject, IsCollapsed
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListDocumentsByFilterGroupedByProject(int projectId, int managerId,
			int resourceId, int priorityId, int statusId, string keyword,
			int categoryType, PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			return DBDocument.GetListDocumentsByFilterGroupedByProject(projectId, managerId,
				resourceId, priorityId, statusId, keyword,
				Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId,
				Security.CurrentUser.LanguageId, categoryType, orgUid, contactUid);
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
		public static DataTable GetListDocumentsByFilterGroupedByClient(int projectId, int managerId,
			int resourceId, int priorityId, int statusId, string keyword,
			int categoryType, PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			return DBDocument.GetListDocumentsByFilterGroupedByClient(projectId, managerId,
				resourceId, priorityId, statusId, keyword,
				Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId,
				Security.CurrentUser.LanguageId, categoryType, orgUid, contactUid);
		}
		#endregion

		#region GetListDocumentsUpdatedByUser
		/// <summary>
		///		DocumentId, Title, CreatorId, ManagerId, LastSavedDate, ProjectId, ProjectName, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListDocumentsUpdatedByUser(int Days, int ProjectId)
		{
			return DBDocument.GetListDocumentsUpdatedByUser(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}

		public static DataTable GetListDocumentsUpdatedByUserDataTable(int Days, int ProjectId)
		{
			return DBDocument.GetListDocumentsUpdatedByUserDataTable(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}
		#endregion

		#region GetListDocumentsUpdatedForUser
		/// <summary>
		///		DocumentId, Title, CreatorId, ManagerId, LastEditorId, LastSavedDate, 
		///		ProjectId, ProjectName, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListDocumentsUpdatedForUser(int Days, int ProjectId)
		{
			return DBDocument.GetListDocumentsUpdatedForUser(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}

		public static DataTable GetListDocumentsUpdatedForUserDataTable(int Days, int ProjectId)
		{
			return DBDocument.GetListDocumentsUpdatedForUserDataTable(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}
		#endregion

		#region GetListDocumentsByKeyword
		/// <summary>
		/// DocumentId, CreatorId, ManagerId, Title, Description, PriorityId, PriorityName, 
		/// StatusId, StatusName, CreationDate, IsCompleted, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListDocumentsByKeyword(string Keyword)
		{
			return DBDocument.GetListDocumentsByKeyword(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, Keyword);
		}
		#endregion

		#region UploadFile
		public static void UploadFile(int document_id, string FileName, System.IO.Stream _inputStream)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				if(FileName!=null)
				{
					string ContainerName = "FileLibrary";
					string ContainerKey = UserRoleHelper.CreateDocumentContainerKey(document_id);

					BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
					FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
					fs.SaveFile(fs.Root.Id, FileName, _inputStream);
				//	Asset.Create(document_id,ObjectTypes.Document, FileName,"", FileName, _inputStream, true);
				}

				tran.Commit();
			}
		}
		#endregion

		#region GetListDocumentsForChangeableRoles
		/// <summary>
		/// DocumentId, Title, IsCompleted, ReasonId, StateId, IsManager, IsResource, 
		///  CanView, CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListDocumentsForChangeableRoles(int UserId)
		{
			return DBDocument.GetListDocumentsForChangeableRoles(UserId, Security.CurrentUser.UserID);
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

		#region SaveGeneralCategories
		public static void SaveGeneralCategories(ArrayList general_categories)
		{
			Project.SaveGeneralCategories(general_categories);
		}
		#endregion

		#region UpdateDiscussion
		public static void UpdateDiscussion(int DiscussionId, string Text)
		{
			Project.UpdateDiscussion(DiscussionId, Text);
		}
		#endregion

		#region AcceptResource
		[Obsolete]
		public static void AcceptResource(int document_id)
		{
			int UserId = Security.CurrentUser.UserID;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBDocument.ReplyResource(document_id, UserId, true, DateTime.UtcNow);

				// ToDo [9/27/2005]
				//SendAlert(AlertEventType.Document_ResourceAccepted, document_id);

				tran.Commit();
			}
		}
		#endregion

		#region DeclineResource
		[Obsolete]
		public static void DeclineResource(int document_id)
		{
			int UserId = Security.CurrentUser.UserID;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBDocument.ReplyResource(document_id, UserId, false, DateTime.UtcNow);

				// ToDo [9/27/2005]
				//SendAlert(AlertEventType.Document_ResourceDeclined, document_id);

				tran.Commit();
			}
		}
		#endregion

		#region GetListResources
		/// <summary>
		///  ResourceId, DocumentId, PrincipalId, IsGroup, MustBeConfirmed, ResponsePending, 
		///  IsConfirmed, IsExternal, CanManage
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListResources(int DocumentId)
		{
			return DBDocument.GetListResources(DocumentId, Security.CurrentUser.TimeZoneId);
		}

		public static DataTable GetListDocumentResourcesDataTable(int DocumentId)
		{
			return DBDocument.GetListDocumentResourcesDataTable(DocumentId, Security.CurrentUser.TimeZoneId);
		}
		#endregion	

		#region Collapse
		public static void Collapse(int ProjectId)
		{
			DBDocument.Collapse(Security.CurrentUser.UserID, ProjectId);
		}
		#endregion

		#region Expand
		public static void Expand(int ProjectId)
		{
			DBDocument.Expand(Security.CurrentUser.UserID, ProjectId);
		}
		#endregion

		#region CollapseByClient
		public static void CollapseByClient(PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			DBDocument.CollapseByClient(Security.CurrentUser.UserID, contactUid, orgUid);
		}
		#endregion

		#region ExpandByClient
		public static void ExpandByClient(PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			DBDocument.ExpandByClient(Security.CurrentUser.UserID, contactUid, orgUid);
		}
		#endregion

		#region GetListActiveDocumentsByUserOnlyDataTable
		/// <summary>
		/// DocumentId, Title, CreationDate, PriorityId, PriorityName, IsManager, IsResource
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListActiveDocumentsByUserOnlyDataTable()
		{
			return DBDocument.GetListActiveDocumentsByUserOnlyDataTable(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}

		public static DataTable GetListActiveDocumentsByUserOnlyDataTable(int UserId)
		{
			return DBDocument.GetListActiveDocumentsByUserOnlyDataTable(UserId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region AddFavorites
		public static void AddFavorites(int DocumentId)
		{
			DBCommon.AddFavorites((int)DOCUMENT_TYPE, DocumentId, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListFavoritesDT
		/// <summary>
		///		FavoriteId, ObjectTypeId, ObjectId, UserId, Title 
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListFavoritesDT()
		{
			return DBCommon.GetListFavoritesDT((int)DOCUMENT_TYPE, Security.CurrentUser.UserID);
		}
		#endregion

		#region CheckFavorites
		public static bool CheckFavorites(int DocumentId)
		{
			return DBCommon.CheckFavorites((int)DOCUMENT_TYPE, DocumentId, Security.CurrentUser.UserID);
		}
		#endregion

		#region DeleteFavorites
		public static void DeleteFavorites(int DocumentId)
		{
			DBCommon.DeleteFavorites((int)DOCUMENT_TYPE, DocumentId, Security.CurrentUser.UserID);
		}
		#endregion

		#region AddHistory
		public static void AddHistory(int DocumentId)
		{
			string Title = GetTitle(DocumentId);
			if (Title == null || Title == String.Empty)
				return;

			AddHistory(DocumentId, Title);
		}

		public static void AddHistory(int DocumentId, string Title)
		{
			DBCommon.AddHistory((int)DOCUMENT_TYPE, DocumentId, Title, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListDocumentManagers
		/// <summary>
		///  UserId, UserName, UserName2
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListDocumentManagers()
		{
			return DBDocument.GetListDocumentManagers();
		}
		#endregion

		#region GetIsCompleted
		public static bool GetIsCompleted(int DocumentId)
		{
			bool retval = false;
			if (DBDocument.GetIsCompleted(DocumentId) == 1)
				retval = true;
			return retval;
		}
		#endregion

		#region SuspendToDo
		public static void SuspendToDo(int DocumentId)
		{
			ArrayList TodoList = new ArrayList();
			using(IDataReader reader = DBDocument.GetListToDo(DocumentId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
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
		public static void CompleteToDo(int DocumentId)
		{
			ArrayList TodoList = new ArrayList();
			using(IDataReader reader = DBDocument.GetListToDo(DocumentId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				while (reader.Read())
					if (!(bool)reader["IsCompleted"])
						TodoList.Add((int)reader["ToDoId"]);
			}

			foreach (int ToDoId in TodoList)
				ToDo.CompleteToDo(ToDoId, false);
		}
		#endregion

		#region ResumeToDo
		public static void ResumeToDo(int DocumentId)
		{
			ArrayList TodoList = new ArrayList();
			using(IDataReader reader = DBDocument.GetListToDo(DocumentId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				while (reader.Read())
					if ((bool)reader["IsCompleted"] && (int)reader["ReasonId"] == (int)CompletionReason.SuspendedAutomatically)
						TodoList.Add((int)reader["ToDoId"]);
			}

			foreach (int ToDoId in TodoList)
				ToDo.ResumeToDo(ToDoId, false);
		}
		#endregion

		#region UncompleteToDo
		public static void UncompleteToDo(int DocumentId)
		{
			ArrayList TodoList = new ArrayList();
			using(IDataReader reader = DBDocument.GetListToDo(DocumentId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				while (reader.Read())
					if ((bool)reader["IsCompleted"] && (int)reader["ReasonId"] == (int)CompletionReason.CompletedAutomatically)
						TodoList.Add((int)reader["ToDoId"]);
			}

			foreach (int ToDoId in TodoList)
				ToDo.UncompleteToDo(ToDoId, false);
		}
		#endregion

		#region CompleteDocument
		public static void CompleteDocument(int document_id)
		{
			CompleteDocument(document_id, true);
		}

		public static void CompleteDocument(int document_id, bool CheckAccess)
		{
			if (CheckAccess && !CanUpdate(document_id))
				throw new AccessDeniedException();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBDocument.UpdateCompletion(document_id, true, (int)CompletionReason.CompletedManually);

				CompleteToDo(document_id);

				RecalculateState(document_id);

				tran.Commit();
			}
		}
		#endregion

		#region UncompleteDocument
		public static void UncompleteDocument(int document_id)
		{
			if (!CanUpdate(document_id))
				throw new AccessDeniedException();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBDocument.UpdateCompletion(document_id, false, (int)CompletionReason.NotCompleted);

				UncompleteToDo(document_id);

				RecalculateState(document_id);

				tran.Commit();
			}
		}
		#endregion

		#region SuspendDocument
		public static void SuspendDocument(int document_id)
		{
			if (!CanUpdate(document_id))
				throw new AccessDeniedException();

			int UserId = Security.CurrentUser.UserID;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBDocument.UpdateCompletion(document_id, true, (int)CompletionReason.SuspendedManually);

				SuspendToDo(document_id);

				RecalculateState(document_id);

				tran.Commit();
			}
		}
		#endregion

		#region ResumeDocument
		public static void ResumeDocument(int document_id)
		{
			if (!CanUpdate(document_id))
				throw new AccessDeniedException();

			int UserId = Security.CurrentUser.UserID;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBDocument.UpdateCompletion(document_id, false, (int)CompletionReason.NotCompleted);
	
				ResumeToDo(document_id);

				RecalculateState(document_id);

				tran.Commit();
			}
		}
		#endregion

		#region GetTrackingInfo
		public static Tracking GetTrackingInfo(int document_id)
		{
			DocumentSecurity sec = GetSecurity(document_id);
			bool IsResource = sec.IsRealDocumentResource;
			bool IsManager = sec.IsManager || Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager);

			bool Document_IsCompleted;
			int Document_ReasonId;
			int Document_StateId;

			using(IDataReader reader = DBDocument.GetDocument(document_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				reader.Read();
				Document_IsCompleted = (bool)reader["IsCompleted"];
				Document_ReasonId = (int)reader["ReasonId"];
				Document_StateId = (int)reader["StateId"];
			}

			bool Resource_MustBeConfirmed = false;
			bool Resource_IsResponsePending = false;
			bool Resource_IsConfirmed = true;

			if (IsResource)
			{
				using(IDataReader reader = DBDocument.GetResourceByUser(document_id, Security.CurrentUser.UserID))
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
				&& !Document_IsCompleted;

			// Complete
			bool ShowComplete = IsManager 
				&& (Document_StateId == (int)ObjectStates.Upcoming 
						|| Document_StateId == (int)ObjectStates.Active
						|| Document_StateId == (int)ObjectStates.Overdue
					);

			// Suspend
			bool ShowSuspend = IsManager 
				&& (Document_StateId == (int)ObjectStates.Active
						|| Document_StateId == (int)ObjectStates.Overdue);

			// Uncomplete
			bool ShowUncomplete = IsManager && Document_StateId == (int)ObjectStates.Completed;

			// Resume
			bool ShowResume = IsManager && Document_StateId == (int)ObjectStates.Suspended;

			// Activate
			bool ShowActivate = IsManager && Document_StateId == (int)ObjectStates.Upcoming;

			// Time Tracking (without status)
			bool ShowTimeTracking = Configuration.TimeTrackingModule && IsResource 
				&& (!Resource_MustBeConfirmed || (Resource_MustBeConfirmed && !Resource_IsResponsePending && Resource_IsConfirmed))
				&& Document_StateId != (int)ObjectStates.Active 
				&& Document_StateId != (int)ObjectStates.Overdue;

			// Show Status
			bool ShowStatus = IsManager && !IsResource
				&& (Document_StateId == (int)ObjectStates.Active || Document_StateId == (int)ObjectStates.Overdue);
			if (!ShowStatus)
			{
				ShowStatus = !Configuration.TimeTrackingModule && IsResource
					&& (!Resource_MustBeConfirmed || (Resource_MustBeConfirmed && !Resource_IsResponsePending && Resource_IsConfirmed))
					&& (Document_StateId == (int)ObjectStates.Active || Document_StateId == (int)ObjectStates.Overdue);
			}

			// Show Status and TimeTracking
			bool ShowStatusAndTimeTracking = Configuration.TimeTrackingModule && IsResource
				&& (!Resource_MustBeConfirmed || (Resource_MustBeConfirmed && !Resource_IsResponsePending && Resource_IsConfirmed))
				&& (Document_StateId == (int)ObjectStates.Active || Document_StateId == (int)ObjectStates.Overdue);

			// Show Add New Version
			bool ShowAddNewVersion = CanAddVersion(document_id);

			return new Tracking(ShowAcceptDeny, ShowComplete, ShowSuspend, ShowUncomplete, ShowResume, ShowTimeTracking, ShowActivate, ShowStatus, ShowStatusAndTimeTracking, ShowAddNewVersion);
		}
		#endregion

		#region GetProject
		public static int GetProject(int DocumentId)
		{
			return DBDocument.GetProject(DocumentId);
		}
		#endregion

		#region GetListPendingDocumentsDataTable
		/// <summary>
		///		DocumentId, Title, Description, PriorityId, PriorityName, ManagerId, CreationDate,
		///		StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListPendingDocumentsDataTable(int ProjectId)
		{
			return DBDocument.GetListPendingDocumentsDataTable(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region Exists
		public static bool Exists(int DocumentId)
		{
			bool retval = false;
			using (IDataReader reader = DBDocument.GetDocument(DocumentId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (reader.Read())
					retval = true;
			}
			return retval;
		}
		#endregion

		#region ActivateDocument
		public static void ActivateDocument(int document_id)
		{
			if (!CanUpdate(document_id))
				throw new AccessDeniedException();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBDocument.UpdateActivation(document_id);

				RecalculateState(document_id);

				tran.Commit();
			}
		}
		#endregion

		#region RecalculateState
		public static void RecalculateState(int document_id)
		{
			/// OldState, NewState
			int OldStateId = -1;
			int NewStateId = -1;

			using (IDataReader reader = DBDocument.DocumentRecalculateState(document_id))
			{
				if (reader.Read())
				{
					if (reader["OldStateId"] != DBNull.Value)
						OldStateId = (int)reader["OldStateId"];
					NewStateId = (int)reader["NewStateId"];
				}
			}

			int UserId = Security.CurrentUser.UserID;
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			int LanguageId = Security.CurrentUser.LanguageId;

			ArrayList TodoList = new ArrayList();
			if (OldStateId != NewStateId)
			{
				SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_State, document_id);

				// Формируем список ToDo
				using(IDataReader reader = DBDocument.GetListToDo(document_id, UserId, TimeZoneId, LanguageId))
				{
					while (reader.Read())
						if (!(bool)reader["IsCompleted"])
							TodoList.Add((int)reader["ToDoId"]);
				}
			}

			foreach (int TodoId in TodoList)
				ToDo.RecalculateState(TodoId);
		}
		#endregion

        #region RebuildDocumentVersionAcl
        public static void RebuildDocumentVersionAcl()
        {
            List<string> containerKeys = new List<string>();
            using (DbTransaction tran = DbTransaction.Begin())
            {
                using (IDataReader reader = DbHelper2.RunTextDataReader(
                    "SELECT DISTINCT ContainerKey FROM fsc_Directories WHERE ContainerKey like 'DocumentVers_%'"))
                {
                    while (reader.Read())
                    {
                        containerKeys.Add((string)reader["ContainerKey"]);
                    }
                }
                foreach (string containerKey in containerKeys)
                {
                    BaseIbnContainer bic = BaseIbnContainer.Create("FileLibrary", containerKey);
                    FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");

                    AccessControlList rootAcl = AccessControlList.GetACL(fs.Root.Id);
                    rootAcl.Clear();

                    foreach (AccessControlEntry ace in fs.Info.DefaultAccessControlList.GetACL(containerKey))
                        rootAcl.Add(ace);

                    AccessControlList.SetACL(fs, rootAcl, false);
                }
                tran.Commit();
            }
        }
        #endregion

		#region AddTimeSheet
		public static void AddTimeSheet(int documentId, int minutes, DateTime dt)
		{
			string title = GetTitle(documentId);
			int projectId = GetProject(documentId);

			if (minutes > 0)
				Mediachase.IbnNext.TimeTracking.TimeTrackingManager.AddHoursForEntryByObject(documentId, (int)DOCUMENT_TYPE, title, projectId, Security.CurrentUser.UserID, TimeTracking.GetWeekStart(dt), TimeTracking.GetDayNum(dt), minutes, true);
		}
		#endregion

		#region ReadOnly
		/// <summary>
		/// Sets the read only.
		/// </summary>
		/// <param name="documentId">The document id.</param>
		/// <param name="readOnly">if set to <c>true</c> [read only].</param>
		public static void SetReadOnly(int documentId, bool readOnly)
		{
			DBDocument.SetReadOnly(documentId, readOnly);
		}

		/// <summary>
		/// Determines whether [is read only] [the specified document id].
		/// </summary>
		/// <param name="documentId">The document id.</param>
		/// <returns>
		/// 	<c>true</c> if [is read only] [the specified document id]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsReadOnly(int documentId)
		{
			return DBDocument.IsReadOnly(documentId);
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
			return DBDocument.GetListDocumentsByProjectAll(projectId);
		}
		#endregion
	}
}
