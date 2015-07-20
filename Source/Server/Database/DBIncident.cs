using System;
using System.Data;
using System.Data.SqlClient;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBIncident.
	/// </summary>
	public class DBIncident
	{
		#region Create
		public static int Create(
			object ProjectId, int CreatorId,
			string Title, string Description,
			DateTime CreationDate, int TypeId, int PriorityId,
			int StateId, int SeverityId, bool IsEmail, int TaskTime,
			int IncidentBoxId, int ResponsibleId, int IsResponsibleGroup, 
			PrimaryKeyId contactUid, PrimaryKeyId orgUid, 
			int ExpectedDuration, int ExpectedResponseTime, int ExpectedAssignTime,
			DateTime ExpectedResolveDate, bool UseDuration)
		{
			return DbHelper2.RunSpInteger("IncidentCreate",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, Title),
				DbHelper2.mp("@Description", SqlDbType.NText, Description),
				DbHelper2.mp("@CreationDate", SqlDbType.DateTime, CreationDate),
				DbHelper2.mp("@TypeId", SqlDbType.Int, TypeId),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId),
				DbHelper2.mp("@StateId", SqlDbType.Int, StateId),
				DbHelper2.mp("@SeverityId", SqlDbType.Int, SeverityId),
				DbHelper2.mp("@IsEmail", SqlDbType.Bit, IsEmail),
				DbHelper2.mp("@TaskTime", SqlDbType.Int, TaskTime),
				DbHelper2.mp("@IncidentBoxId", SqlDbType.Int, IncidentBoxId),
				DbHelper2.mp("@ResponsibleId", SqlDbType.Int, ResponsibleId),
				DbHelper2.mp("@IsResponsibleGroup", SqlDbType.Int, IsResponsibleGroup),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid),
				DbHelper2.mp("@ExpectedDuration", SqlDbType.Int, ExpectedDuration),
				DbHelper2.mp("@ExpectedResponseTime", SqlDbType.Int, ExpectedResponseTime),
				DbHelper2.mp("@ExpectedAssignTime", SqlDbType.Int, ExpectedAssignTime),
				DbHelper2.mp("@ExpectedResolveDate", SqlDbType.DateTime, ExpectedResolveDate),
				DbHelper2.mp("@UseDuration", SqlDbType.Bit, UseDuration));
		}
		#endregion

		#region Delete
		public static void Delete(int IncidentId)
		{
			DbHelper2.RunSp("IncidentDelete",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId));
		}
		#endregion

		#region GetIncident
		/// <summary>
		/// Reader returns fields:
		///  IncidentId, ProjectId, ProjectTitle, CreatorId, 
		///  Title, Description, Resolution, Workaround, CreationDate, 
		///  TypeId, TypeName, PriorityId, PriorityName, 
		///  SeverityId, SeverityName, IsEmail, MailSenderEmail, StateId, TaskTime, 
		///  IncidentBoxId, OrgUid, ContactUid, ClientName, ExpectedDuration,
		///  ExpectedResponseTime, ActualFinishDate, Identifier,
		///  ModifiedDate, ExpectedResponseDate, ExpectedResolveDate, ActualOpenDate,
		///  ControllerId, ResponsibleGroupState, TotalMinutes, TotalApproved, IsOverdue, 
		///  IsNewMessage, ExpectedAssignTime, ExpectedAssignDate, CurrentResponsibleId,
		///  UseDuration, ProjectCode
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetIncident(int IncidentId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[] { "CreationDate", "ActualFinishDate", "ModifiedDate", "ExpectedResponseDate", "ExpectedResolveDate", "ActualOpenDate", "ExpectedAssignDate" },
				"IncidentGet",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetIncidentInUTC
		/// <summary>
		/// Reader returns fields:
		///  IncidentId, ProjectId, ProjectTitle, CreatorId, 
		///  Title, Description, Resolution, Workaround, CreationDate, 
		///  TypeId, TypeName, PriorityId, PriorityName, 
		///  SeverityId, SeverityName, IsEmail, MailSenderEmail, StateId, TaskTime, 
		///  IncidentBoxId, OrgUid, ContactUid, ClientName, ExpectedDuration,
		///  ExpectedResponseTime, ActualFinishDate, Identifier,
		///  ModifiedDate, ExpectedResponseDate, ExpectedResolveDate, ActualOpenDate,
		///  ControllerId, ResponsibleGroupState, TotalMinutes, TotalApproved, IsOverdue, 
		///  IsNewMessage, ExpectedAssignTime, ExpectedAssignDate, CurrentResponsibleId,
		///  UseDuration
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetIncidentInUTC(int incidentId, int languageId)
		{
			return DbHelper2.RunSpDataReader("IncidentGet",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, incidentId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, languageId));
		}
		#endregion

		#region GetIncidentTrackingState
		/// <summary>
		/// Reader returns fields:
		///  IncidentId, StateId, StateName, 
		///  ResponsibleId, IsResponsibleGroup, ResponsibleGroupState, PriorityId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetIncidentTrackingState(int IncidentId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[] { "CreationDate", "ActualFinishDate" },
				"IncidentTrackingStateGet",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion



		#region GetSecurityForUser
		/// <summary>
		/// Reader return fields:
		///  IncidentId, PrincipalId, IsManager, IsCreator, IsResource
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetSecurityForUser(int IncidentId, int UserId)
		{
			return DbHelper2.RunSpDataReader("IncidentGetSecurityForUser",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetSecurityForUser2
		/// <summary>
		/// Reader return fields:
		///  IncidentId, PrincipalId, IsManager, IsCreator, IsResource
		///  IsPendingResource, IsPendingResponsible, IsResponsible, IsControler
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetSecurityForUser2(int IncidentId, int UserId)
		{
			return DbHelper2.RunSpDataReader("IncidentGetSecurityForUser2",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListIncidentTypes
		/// <summary>
		/// Reader returns fields:
		///  TypeId, TypeName 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentTypes()
		{
			return DbHelper2.RunSpDataReader("IncidentTypesGet");
		}
		#endregion

		#region GetListIncidentTypesForDictionaries
		/// <summary>
		///  ItemId, ItemName, CanDelete
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentTypesForDictionaries()
		{
			return DbHelper2.RunSpDataTable("IncidentTypesGetForDictionaries");
		}
		#endregion

		#region GetListIncidentSeverity
		/// <summary>
		/// Reader returns fields:
		///  SeverityId, SeverityName 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentSeverity()
		{
			return DbHelper2.RunSpDataReader("IncidentSeverityGet");
		}
		#endregion

		#region GetListIncidentSeverityForDictionaries
		/// <summary>
		///  ItemId, ItemName, CanDelete
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentSeverityForDictionaries()
		{
			return DbHelper2.RunSpDataTable("IncidentSeverityGetForDictionaries");
		}
		#endregion

		#region GetListIncidentsByProject
		/// <summary>
		/// Reader returns fields:
		///  IncidentId, ProjectId, ProjectTitle, CreatorId, CreatorName, 
		///  ManagerId, ManagerName,  Title, CreationDate, 
		///  TypeId, TypeName, PriorityId, PriorityName, 
		///  SeverityId, SeverityName, StateId, CanEdit, CanDelete, Identifier
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentsByProject(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[] { "CreationDate" },
				"IncidentsGetByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListIncidentsByProjectDataTable
		/// <summary>
		/// Reader returns fields:
		///  IncidentId, ProjectId, ProjectTitle, CreatorId, CreatorName, 
		///  ManagerId, ManagerName, Title, CreationDate, 
		///  TypeId, TypeName, PriorityId, PriorityName, 
		///  SeverityId, SeverityName, CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsByProjectDataTable(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "CreationDate" },
				"IncidentsGetByProject",
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
		public static IDataReader GetListToDo(int IncidentId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[] { "StartDate", "FinishDate", "ActualFinishDate" },
				"ToDoGetByIncident",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}

		public static DataTable GetListToDoDataTable(int IncidentId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "StartDate", "FinishDate", "ActualFinishDate" },
				"ToDoGetByIncident",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetProject
		public static int GetProject(int IncidentId)
		{
			return DbHelper2.RunSpInteger("IncidentGetProject",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId));
		}
		#endregion

		#region GetState
		public static int GetState(int IncidentId)
		{
			return DbHelper2.RunSpInteger("IncidentGetState",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId));
		}
		#endregion

		#region GetTitle
		public static string GetTitle(int IncidentId)
		{
			return (string)DbHelper2.RunSpScalar("IncidentGetTitle",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId));
		}
		#endregion

		#region AddIncidentSeverity
		public static void AddIncidentSeverity(string SeverityName)
		{
			DbHelper2.RunSp("IncidentSeverityAdd",
				DbHelper2.mp("@SeverityName", SqlDbType.NVarChar, 50, SeverityName));
		}
		#endregion

		#region UpdateIncidentSeverity
		public static void UpdateIncidentSeverity(int SeverityId, string SeverityName)
		{
			DbHelper2.RunSp("IncidentSeverityUpdate",
				DbHelper2.mp("@SeverityId", SqlDbType.Int, SeverityId),
				DbHelper2.mp("@SeverityName", SqlDbType.NVarChar, 50, SeverityName));
		}
		#endregion

		#region DeleteIncidentSeverity
		public static void DeleteIncidentSeverity(int SeverityId)
		{
			DbHelper2.RunSp("IncidentSeverityDelete",
				DbHelper2.mp("@SeverityId", SqlDbType.Int, SeverityId));
		}
		#endregion

		#region AddIncidentType
		public static void AddIncidentType(string TypeName)
		{
			DbHelper2.RunSp("IncidentTypeAdd",
				DbHelper2.mp("@TypeName", SqlDbType.NVarChar, 50, TypeName));
		}
		#endregion

		#region UpdateIncidentType
		public static void UpdateIncidentType(int TypeId, string TypeName)
		{
			DbHelper2.RunSp("IncidentTypeUpdate",
				DbHelper2.mp("@TypeId", SqlDbType.Int, TypeId),
				DbHelper2.mp("@TypeName", SqlDbType.NVarChar, 50, TypeName));
		}
		#endregion

		#region DeleteIncidentType
		public static void DeleteIncidentType(int TypeId)
		{
			DbHelper2.RunSp("IncidentTypeDelete",
				DbHelper2.mp("@TypeId", SqlDbType.Int, TypeId));
		}
		#endregion

		#region GetListNotAssignedIncidents
		/// <summary>
		/// Reader returns fields:
		///  IncidentId, Title, PriorityId, PriorityName, 
		///  CreationDate, CreatorId, StateId, Identifier, IsOverdue
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListNotAssignedIncidents(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[] { "CreationDate" },
				"IncidentsGetNotAssigned",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		public static DataTable GetListNotAssignedIncidentsDataTable(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "CreationDate" },
				"IncidentsGetNotAssigned",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetSharingLevel
		public static int GetSharingLevel(int UserId, int IncidentId)
		{
			return DbHelper2.RunSpInteger("SharingGetLevelForIncident",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId));
		}
		#endregion

		#region AddPOP3Incident
		public static int AddPOP3Incident(int MailboxId, string From, string FirstName, string LastName,
			string Title, string Description, int PriorityId)
		{
			return DbHelper2.RunSpInteger("POP3IncidentAdd",
				DbHelper2.mp("@MailboxId", SqlDbType.Int, MailboxId),
				DbHelper2.mp("@From", SqlDbType.NVarChar, 255, From),
				DbHelper2.mp("@FirstName", SqlDbType.NVarChar, 50, FirstName),
				DbHelper2.mp("@LastName", SqlDbType.NVarChar, 50, LastName),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, Title),
				DbHelper2.mp("@Description", SqlDbType.NText, Description),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId));
		}
		#endregion

		#region AddMHTFilePOP3Incident
		public static void AddMHTFilePOP3Incident(int Pop3Incident, int MHTFileId)
		{
			DbHelper2.RunSp("POP3IncidentAddMHTFile",
				DbHelper2.mp("@Pop3IncidentId", SqlDbType.Int, Pop3Incident),
				DbHelper2.mp("@MHTFileId", SqlDbType.Int, MHTFileId));
		}
		#endregion

		#region DeletePOP3Incident
		public static void DeletePOP3Incident(int IncidentId)
		{
			DbHelper2.RunSp("POP3IncidentDelete",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId));
		}
		#endregion

		#region GetListPOP3Incidents
		/// <summary>
		/// Reader returns fields:
		///  IncidentId, MailboxId, Sender, FirstName, LastName, Title, Description, PriorityId, PriorityName, CreationDate, SenderType, MHTFileId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListPOP3Incidents(int IncidentId, int MailboxId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[] { "CreationDate" },
				"POP3IncidentsGet",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@MailboxId", SqlDbType.Int, MailboxId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListPOP3IncidentsDataTable
		/// <summary>
		/// DataTable returns fields:
		///  IncidentId, From, Title, Description, PriorityId, PriorityName, CreationDate, SenderType, MHTFileId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListPOP3IncidentsDataTable(int IncidentId, int MailboxId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "CreationDate" },
				"POP3IncidentsGet",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@MailboxId", SqlDbType.Int, MailboxId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListIncidentCreators
		/// <summary>
		///  UserId, UserName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentCreators()
		{
			return DbHelper2.RunSpDataReader("IncidentCreatorsGet");
		}
		#endregion

		#region GetListIncidentManagers
		/// <summary>
		///  UserId, UserName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentManagers()
		{
			return DbHelper2.RunSpDataReader("IncidentManagersGet");
		}
		#endregion

		#region GetListIncidentsByFilterDataTable
		/// <summary>
		/// IncidentId, ProjectId, ProjectTitle, IncidentBoxId, IncidentBoxName, ClientName,
		/// CreatorId, CreatorName, ControllerId, ControllerName, ManagerId, ManagerName,
		/// ResponsibleId, ResponsibleName, IsResponsibleGroup, ResponsibleGroupState,
		/// ContactUid, OrgUid, Title, CreationDate, ModifiedDate, ActualOpenDate, ActualFinishDate, 
		/// ExpectedResponseDate, ExpectedResolveDate, ExpectedAssignDate,
		/// TypeId, TypeName, PriorityId, PriorityName, 
		/// StateId, StateName, SeverityId, Identifier, IsOverdue, IsNewMessage, CurrentResponsibleId, 
		/// CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsByFilterDataTable(int ProjectId, int ManagerId,
			int CreatorId, int ResourceId, int ResponsibleId, PrimaryKeyId orgUid, PrimaryKeyId contactUid, 
			int IncidentBoxId, int PriorityId, int TypeId, int StateId,
			int SeverityId, string Keyword, int UserId, int TimeZoneId, int LanguageId,
			int CategoryType, int IncidentCategoryType)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "CreationDate", "ModifiedDate", "ActualOpenDate", "ActualFinishDate", "ExpectedResponseDate", "ExpectedResolveDate", "ExpectedAssignDate" },
				"IncidentsGetByFilter",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@ResourceId", SqlDbType.Int, ResourceId),
				DbHelper2.mp("@ResponsibleId", SqlDbType.Int, ResponsibleId),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@IncidentBoxId", SqlDbType.Int, IncidentBoxId),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId),
				DbHelper2.mp("@TypeId", SqlDbType.Int, TypeId),
				DbHelper2.mp("@StateId", SqlDbType.Int, StateId),
				DbHelper2.mp("@SeverityId", SqlDbType.Int, SeverityId),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, Keyword),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@CategoryType", SqlDbType.Int, CategoryType),
				DbHelper2.mp("@IncidentCategoryType", SqlDbType.Int, IncidentCategoryType));


		}
		#endregion

		#region GetListIncidentsByFilterGroupedByProject
		/// <summary>
		///ContactUid, OrgUid, IsClient, ClientName, ProjectId, ProjectTitle, 
		///IncidentId, Title, CreatorId, CreatorName, ControllerId, ControllerName, ManagerId, 
		///ManagerName,ResponsibleId , ResponsibleName , IsResponsibleGroup, ResponsibleGroupState ,
		///CreationDate, TypeId, TypeName, PriorityId, PriorityName,
		///StateId, StateName, SeverityId, CanEdit, CanDelete, IsCollapsed, IsOverdue, IsNewMessage,
		/// CurrentResponsibleId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsByFilterGroupedByProject(int ProjectId, int ManagerId,
			int CreatorId, int ResourceId, int ResponsibleId, PrimaryKeyId orgUid, PrimaryKeyId contactUid, int IncidentBoxId,
			int PriorityId, int TypeId, int StateId,
			int SeverityId, string Keyword, int UserId, int TimeZoneId, int LanguageId,
			int CategoryType, int IncidentCategoryType)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "CreationDate" },
				"IncidentsGetByFilterGroupedByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@ResourceId", SqlDbType.Int, ResourceId),
				DbHelper2.mp("@ResponsibleId", SqlDbType.Int, ResponsibleId),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@IncidentBoxId", SqlDbType.Int, IncidentBoxId),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId),
				DbHelper2.mp("@TypeId", SqlDbType.Int, TypeId),
				DbHelper2.mp("@StateId", SqlDbType.Int, StateId),
				DbHelper2.mp("@SeverityId", SqlDbType.Int, SeverityId),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, Keyword),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@CategoryType", SqlDbType.Int, CategoryType),
				DbHelper2.mp("@IncidentCategoryType", SqlDbType.Int, IncidentCategoryType));
		}
		#endregion

		#region GetListIncidentsByFilterGroupedByClient
		/// <summary>
		/// IncidentId, ProjectId, ProjectTitle, CreatorId, ManagerId, 
		/// ControllerId,ControllerName, ResponsibleId, ResponsibleName,  
		/// IsResponsibleGroup, ResponsibleGroupState, ContactUid, OrgUid,
		/// Title, CreationDate, 
		/// TypeId, TypeName, PriorityId, PriorityName, 
		/// SeverityId, StateId, CanEdit, CanDelete, IsProject, 
		/// IsCollapsed, IsOverdue, IsNewMessage, CurrentResponsibleId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsByFilterGroupedByClient(int ProjectId, int ManagerId,
			int CreatorId, int ResourceId, int ResponsibleId, PrimaryKeyId orgUid, PrimaryKeyId contactUid, int IncidentBoxId,
			int PriorityId, int TypeId, int StateId,
			int SeverityId, string Keyword, int UserId, int TimeZoneId, int LanguageId,
			int CategoryType, int IncidentCategoryType)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "CreationDate" },
				"IncidentsGetByFilterGroupedByClient",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@ResourceId", SqlDbType.Int, ResourceId),
				DbHelper2.mp("@ResponsibleId", SqlDbType.Int, ResponsibleId),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@IncidentBoxId", SqlDbType.Int, IncidentBoxId),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId),
				DbHelper2.mp("@TypeId", SqlDbType.Int, TypeId),
				DbHelper2.mp("@StateId", SqlDbType.Int, StateId),
				DbHelper2.mp("@SeverityId", SqlDbType.Int, SeverityId),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, Keyword),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@CategoryType", SqlDbType.Int, CategoryType),
				DbHelper2.mp("@IncidentCategoryType", SqlDbType.Int, IncidentCategoryType));
		}
		#endregion
		#region DeleteByProject
		public static void DeleteByProject(int ProjectId)
		{
			DbHelper2.RunSp("IncidentsDeleteByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListIncidentsUpdatedByUser
		/// <summary>
		///		IncidentId, Title, StateId, LastSavedDate
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentsUpdatedByUser(int ProjectId, int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[] { "LastSavedDate" },
				"IncidentsGetUpdatedByUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListIncidentsUpdatedByUserDataTable
		/// <summary>
		///		IncidentId, Title, StateId, LastSavedDate
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsUpdatedByUserDataTable(int ProjectId, int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "LastSavedDate" },
				"IncidentsGetUpdatedByUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListIncidentsUpdatedForUser
		/// <summary>
		///		IncidentId, Title, CreatorId, ManagerId, LastEditorId, LastSavedDate,
		///		ProjectId, ProjectName, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentsUpdatedForUser(int ProjectId, int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[] { "LastSavedDate" },
				"IncidentsGetUpdatedForUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListIncidentsUpdatedForUserDataTable
		/// <summary>
		///		IncidentId, Title, CreatorId, ManagerId, LastEditorId, LastSavedDate,
		///		ProjectId, ProjectName, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsUpdatedForUserDataTable(int ProjectId, int UserId, int TimeZoneId, int Days)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "LastSavedDate" },
				"IncidentsGetUpdatedForUser",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Days", SqlDbType.Int, Days));
		}
		#endregion

		#region GetListIncidentsByKeyword
		/// <summary>
		/// IncidentId, CreatorId, ManagerId, Title, Description, PriorityId, PriorityName, 
		/// CreationDate, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentsByKeyword(
			int UserId, int TimeZoneId, int LanguageId, string Keyword)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[] { "CreationDate" },
				"IncidentsGetByKeyword",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, Keyword));
		}
		#endregion

		#region GetListIncidentsByKeywordDataTable
		/// <summary>
		/// IncidentId, CreatorId, ManagerId, Title, Description, PriorityId, PriorityName, 
		/// CreationDate, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsByKeywordDataTable(
			int UserId, int TimeZoneId, int LanguageId, string Keyword)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "CreationDate" },
				"IncidentsGetByKeyword",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, Keyword));
		}
		#endregion

		#region GetListIncidentCategoriesForDictionaries
		/// <summary>
		///  ItemId, ItemName, CanDelete
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentCategoriesForDictionaries()
		{
			return DbHelper2.RunSpDataTable("IncidentCategoriesGetForDictionaries");
		}
		#endregion

		#region AddIncidentCategory
		public static void AddIncidentCategory(string CategoryName)
		{
			DbHelper2.RunSp("IncidentCategoryAdd",
				DbHelper2.mp("@CategoryName", SqlDbType.NVarChar, 100, CategoryName));
		}
		#endregion

		#region UpdateIncidentCategory
		public static void UpdateIncidentCategory(int CategoryId, string CategoryName)
		{
			DbHelper2.RunSp("IncidentCategoryUpdate",
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId),
				DbHelper2.mp("@CategoryName", SqlDbType.NVarChar, 100, CategoryName));
		}
		#endregion

		#region DeleteIncidentCategory
		public static void DeleteIncidentCategory(int CategoryId)
		{
			DbHelper2.RunSp("IncidentCategoryDelete",
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId));
		}
		#endregion

		#region GetListIncidentCategories
		/// <summary>
		/// Reader returns fields:
		///  CategoryId, CategoryName 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentCategories(int CategoryId)
		{
			return DbHelper2.RunSpDataReader("IncidentCategoriesGet",
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId));
		}
		#endregion

		#region GetListIncidentCategoriesByIncident
		/// <summary>
		/// Reader returns fields:
		///  CategoryId, CategoryName 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentCategoriesByIncident(int IncidentId)
		{
			return DbHelper2.RunSpDataReader("IncidentCategoriesGetByIncident",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId));
		}
		#endregion

		#region AssignIncidentCategory
		public static void AssignIncidentCategory(int IncidentId, int CategoryId)
		{
			DbHelper2.RunSp("IncidentCategoryAssign",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId));
		}
		#endregion

		#region RemoveCategoryFromIncident
		public static void RemoveCategoryFromIncident(int IncidentId, int CategoryId)
		{
			DbHelper2.RunSp("IncidentCategoryRemove",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId));
		}
		#endregion

		#region CheckForChangeableRoles
		public static int CheckForChangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("IncidentsCheckForChangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region CheckForUnchangeableRoles
		public static int CheckForUnchangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("IncidentsCheckForUnchangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListIncidentsForChangeableRoles
		/// <summary>
		/// Reader returns fields:
		///  IncidentId, Title, IsManager, CanView, CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentsForChangeableRoles(int UserId, int CurrentUserId)
		{
			return DbHelper2.RunSpDataReader("IncidentsGetForChangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@CurrentUserId", SqlDbType.Int, CurrentUserId));
		}
		#endregion

		#region ReplaceUnchangeableUserToManager
		public static void ReplaceUnchangeableUserToManager(int UserId)
		{
			DbHelper2.RunSp("IncidentsReplaceUnchangeableUserToManager",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ReplaceUnchangeableUser
		public static void ReplaceUnchangeableUser(int FromUserId, int ToUserId)
		{
			DbHelper2.RunSp("IncidentsReplaceUnchangeableUser",
				DbHelper2.mp("@FromUserId", SqlDbType.Int, FromUserId),
				DbHelper2.mp("@ToUserId", SqlDbType.Int, ToUserId));
		}
		#endregion

		#region GetIncidentStatistic
		/// <summary>
		/// Reader returns fields:
		///  IncidentCount, Pop3IncidentCount, NewIncidentCount, ActiveIncidentCount, 
		///  ClosedIncidentCount, AvgTimeInNewState, AvgTimeInActiveState, 
		///  AvgTimeForResolveClosed, AvgTimeForResolveAll,
		///  OnCheckIncidentCount, ReOpenIncidentCount, SuspendedIncidentCount
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetIncidentStatistic(int ProjectId, int ManagerId)
		{
			return DbHelper2.RunSpDataReader("IncidentsGetStatistic",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId));
		}
		#endregion

		#region GetListIncidentStatisticByTypeDataTable
		/// <summary>
		/// Reader returns fields:
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentStatisticByTypeDataTable(int ProjectId, int ManagerId)
		{
			return DbHelper2.RunSpDataTable("IncidentsGetStatisticByType",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId));
		}
		#endregion


		#region GetListIncidentStatisticByGeneralCategoryDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentStatisticByGeneralCategoryDataTable(int ProjectId, int ManagerId)
		{
			return DbHelper2.RunSpDataTable("IncidentsGetStatisticByGeneralCategory",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId));
		}
		#endregion

		#region GetListIncidentStatisticByIncidentCategoryDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentStatisticByIncidentCategoryDataTable(int ProjectId, int ManagerId)
		{
			return DbHelper2.RunSpDataTable("IncidentsGetStatisticByIncidentCategory",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId));
		}
		#endregion

		#region GetListProjectsForIncidents
		/// <summary>
		/// Reader returns fields:
		///		ProjectId, Title, StatusId, IncidentCount
		/// </summary>
		public static IDataReader GetListProjectsForIncidents(int userId)
		{
			return DbHelper2.RunSpDataReader("ProjectsGetForIncidents",
				DbHelper2.mp("@userId", SqlDbType.Int, userId));
		}
		#endregion

		#region GetListContactsForIncidents
		/// <summary>
		/// Reader returns fields:
		///		ObjectId, ObjectTypeId, ClientName, IncidentCount
		/// </summary>
		public static IDataReader GetListContactsForIncidents(int userId)
		{
			return DbHelper2.RunSpDataReader("ClientsGetForIncidents",
				DbHelper2.mp("@userId", SqlDbType.Int, userId));
		}

		public static DataTable GetListContactsForIncidentsDataTable(int userId)
		{
			return DbHelper2.RunSpDataTable("ClientsGetForIncidents",
				DbHelper2.mp("@userId", SqlDbType.Int, userId));
		}
		#endregion

		#region GetListIncidentStatisticByProjectDataTable
		/// <summary>
		/// Reader returns fields:
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentStatisticByProjectDataTable(int ManagerId)
		{
			return DbHelper2.RunSpDataTable("IncidentsGetStatisticByProject",
				DbHelper2.mp("@ManagerId", SqlDbType.Int, ManagerId));
		}
		#endregion

		#region GetListIncidentStatisticByStatusDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentStatisticByStatusDataTable(int LanguageId)
		{
			return DbHelper2.RunSpDataTable("IncidentsGetStatisticByStatus",
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListIncidentStatisticByIncidentBoxDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentStatisticByIncidentBoxDataTable(int LanguageId)
		{
			return DbHelper2.RunSpDataTable("IncidentsGetStatisticByIncidentBox",
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListIncidentStatisticByPriorityDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentStatisticByPriorityDataTable(int LanguageId)
		{
			return DbHelper2.RunSpDataTable("IncidentsGetStatisticByPriority",
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListIncidentStatisticBySeverityDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentStatisticBySeverityDataTable()
		{
			return DbHelper2.RunSpDataTable("IncidentsGetStatisticBySeverity");
		}
		#endregion

		#region AddIncidentCategoryUser
		public static void AddIncidentCategoryUser(int CategoryId, int UserId)
		{
			DbHelper2.RunSp("IncidentCategoryUserAdd",
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region DeleteIncidentCategoryUser
		public static void DeleteIncidentCategoryUser(int CategoryId, int UserId)
		{
			DbHelper2.RunSp("IncidentCategoryUserDelete",
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListIncidentCategoriesByUser
		/// <summary>
		/// Reader returns fields:
		///		CategoryId, UserId
		/// </summary>
		public static IDataReader GetListIncidentCategoriesByUser(int UserId)
		{
			return DbHelper2.RunSpDataReader("IncidentCategoryUserGetList",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListOpenIncidentsByProject
		/// <summary>
		/// IncidentId, Title, TypeId, TypeName, StateId, ManagerId, ManagerName, Identifier
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListOpenIncidentsByProject(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("IncidentsGetOpenByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListOpenIncidentsByProjectDataTable
		/// <summary>
		/// IncidentId, Title, TypeId, TypeName, StateId, ManagerId, ManagerName, Identifier
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListOpenIncidentsByProjectDataTable(int ProjectId)
		{
			return DbHelper2.RunSpDataTable("IncidentsGetOpenByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListHighPriorityIncidents
		/// <summary>
		/// IncidentId, Title, ProjectId, ProjectTitle, 
		/// PriorityId, PriorityName, CreationDate, StateId, Identifier
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListHighPriorityIncidents(int LanguageId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[] { "CreationDate" },
				"IncidentsGetHighPriority",
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListHighPriorityIncidentsDataTable
		/// <summary>
		/// IncidentId, Title, ProjectId, ProjectTitle, 
		/// PriorityId, PriorityName, CreationDate, StateId, Identifier
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListHighPriorityIncidentsDataTable(int LanguageId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "CreationDate" },
				"IncidentsGetHighPriority",
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListIncidentsByState
		/// <summary>
		/// IncidentId, Title, ProjectId, ProjectName, ManagerId, PriorityId, PriorityName, 
		/// TypeId, TypeName, ManagerName, CreationDate, Identifier
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsByState(int ProjectId, int StateId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "CreationDate" },
				"IncidentsGetByState",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@StateId", SqlDbType.Int, StateId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListIncidentsByCategory
		/// <summary>
		/// IncidentId, Title, ProjectId, ProjectName, PriorityId, PriorityName, 
		/// StateId, Identifier
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsByCategory(int CategoryId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable("IncidentsGetByCategory",
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListIncidentsByType
		/// <summary>
		/// IncidentId, Title, ProjectId, ProjectName, ManagerId, PriorityId, PriorityName, 
		/// StateId, ManagerName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsByType(int TypeId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable("IncidentsGetByType",
				DbHelper2.mp("@TypeId", SqlDbType.Int, TypeId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region AddResource
		public static void AddResource(int IncidentId, int PrincipalId, bool MustBeConfirmed, bool CanManage)
		{
			DbHelper2.RunSp("IncidentResourceAdd",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, MustBeConfirmed),
				DbHelper2.mp("@CanManage", SqlDbType.Bit, CanManage));
		}
		#endregion

		#region DeleteResource
		public static void DeleteResource(int IncidentId, int PrincipalId)
		{
			DbHelper2.RunSp("IncidentResourceDelete",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId));
		}
		#endregion

		#region GetResourceByUser
		/// <summary>
		/// Reader returns fields:
		///	 MustBeConfirmed, ResponsePending, IsConfirmed
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetResourceByUser(int IncidentId, int UserId)
		{
			return DbHelper2.RunSpDataReader("IncidentResourceGetByUser",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ReplyResource
		[Obsolete]
		public static void ReplyResource(int IncidentId, int UserId, bool IsConfirmed, DateTime LastSavedDate)
		{
			DbHelper2.RunSp("IncidentResourceReply",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@IsConfirmed", SqlDbType.Bit, IsConfirmed),
				DbHelper2.mp("@LastSavedDate", SqlDbType.DateTime, LastSavedDate));
		}
		#endregion

		#region GetListResources
		/// <summary>
		/// Reader returns fields:
		///  ResourceId, IncidentId, PrincipalId, IsGroup, MustBeConfirmed, ResponsePending, 
		///  IsConfirmed, IsExternal, CanManage
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListResources(int IncidentId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[] { "LastSavedDate" },
				"IncidentResourcesGet",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId));
		}
		#endregion

		#region GetListIncidentResourcesDataTable
		/// <summary>
		///  ResourceId, IncidentId, PrincipalId, IsGroup, MustBeConfirmed, ResponsePending, 
		///  IsConfirmed, IsExternal, CanManage
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentResourcesDataTable(int IncidentId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "LastSavedDate" },
				"IncidentResourcesGet",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId));
		}
		#endregion

		#region GetListResourcesWithResponsible
		/// <summary>
		/// Reader returns fields:
		///  IncidentId, PrincipalId, IsGroup, IsResponsible, FirstName, LastName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListResourcesWithResponsible(int IncidentId)
		{
			return DbHelper2.RunSpDataReader("IncidentResourcesGetWithResponsible",
			  DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId));
		}
		#endregion

		#region GetListPendingIncidents
		/// <summary>
		///		IncidentId, Title, Description, PriorityId, PriorityName, ManagerId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListPendingIncidents(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader("IncidentsGetPending",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListPendingIncidentsDataTable
		/// <summary>
		///		IncidentId, Title, Description, PriorityId, PriorityName, ManagerId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListPendingIncidentsDataTable(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable("IncidentsGetPending",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region AddIncidentRelation
		public static void AddIncidentRelation(int IncidentId, int RelIncidentId)
		{
			DbHelper2.RunSp("IncidentRelatonAdd",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@RelIncidentId", SqlDbType.Int, RelIncidentId));
		}
		#endregion

		#region DeleteIncidentRelation
		public static void DeleteIncidentRelation(int IncidentId, int RelIncidentId)
		{
			DbHelper2.RunSp("IncidentRelationDelete",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@RelIncidentId", SqlDbType.Int, RelIncidentId));
		}
		#endregion

		#region GetListIncidentRelationsDataTable
		/// <summary>
		///	 IncidentId, Title, ManagerId, StateId, CanView
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentRelationsDataTable(int IncidentId, int UserId)
		{
			return DbHelper2.RunSpDataTable("IncidentRelatonsGet",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region Collapse
		public static void Collapse(int UserId, int ProjectId)
		{
			DbHelper2.RunSp("CollapsedIncidentAdd",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region Expand
		public static void Expand(int UserId, int ProjectId)
		{
			DbHelper2.RunSp("CollapsedIncidentDelete",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region CollapseByClient
		public static void CollapseByClient(int UserId, PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			DbHelper2.RunSp("CollapsedIncidentByClientAdd",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion

		#region ExpandByClient
		public static void ExpandByClient(int UserId, PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			DbHelper2.RunSp("CollapsedIncidentByClientDelete",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion

		#region GetListOpenIncidentsByUserOnlyDataTable
		/// <summary>
		/// IncidentId, Title, CreationDate, PriorityId, PriorityName, IsManager, IsCreator, 
		/// IsResource, StateId, Identifier, IsOverdue, IsNewMessage
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListOpenIncidentsByUserOnlyDataTable(int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "CreationDate" },
				"IncidentsGetOpenByUserOnly",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListIncidentsSimple
		/// <summary>
		///  Reader returns fields:
		///		IncidentId, Title, Identifier
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentsSimple(int ProjectId, int UserId)
		{
			return DbHelper2.RunSpDataReader("IncidentsGetSimple",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListIncidentsForManagerViewDataTable
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, 
		/// StartDate, FinishDate, ActualFinishDate, ActualStartDate, 
		/// PercentCompleted, TaskTime, TotalMinutes, TotalApproved, 
		///	IsCompleted, ManagerId, ReasonId, ProjectId, ProjectTitle,
		///	StateId, CompletionTypeId, 
		/// Identifier, IsOverdue, IsNewMessage, ContactUid, OrgUid, ClientName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsForManagerViewDataTable(int PrincipalId,
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
				TimeZoneId, new string[] { "CreationDate", "StartDate", "ActualFinishDate", "ActualStartDate" },
				"IncidentsGetForManagerView",
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

		#region GetListIncidentsForManagerViewWithChildTodo
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, 
		/// StartDate, FinishDate, ActualFinishDate, ActualStartDate, 
		/// PercentCompleted, TaskTime, TotalMinutes, TotalApproved, 
		///	IsCompleted, ManagerId, ReasonId, ProjectId, ProjectTitle,
		///	StateId, CompletionTypeId, Identifier,
		/// ContainerName, IsOverdue, IsNewMessage, ContactUid, OrgUid, ClientName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsForManagerViewWithChildTodo(int PrincipalId,
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
				TimeZoneId, new string[] { "CreationDate", "StartDate", "ActualFinishDate", "ActualStartDate" },
				"IncidentsGetForManagerViewWithChildToDo",
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

		#region GetListIncidentsForManagerViewWithCategories
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, 
		/// StartDate, FinishDate, ActualFinishDate, ActualStartDate, 
		/// PercentCompleted, TaskTime, TotalMinutes, TotalApproved, 
		///	IsCompleted, ManagerId, ReasonId, ProjectId, ProjectTitle,
		///	StateId, CompletionTypeId, 
		/// Identifier, IsOverdue, IsNewMessage, ContactUid, OrgUid, ClientName,
		/// CategoryId, CategoryName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsForManagerViewWithCategories(int PrincipalId,
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
				TimeZoneId, new string[] { "CreationDate", "StartDate", "ActualFinishDate", "ActualStartDate" },
				"IncidentsGetForManagerViewWithCategories",
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

		#region GetListIncidentsForManagerViewWithChildTodoWithCategories
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, 
		/// StartDate, FinishDate, ActualFinishDate, ActualStartDate, 
		/// PercentCompleted, TaskTime, TotalMinutes, TotalApproved, 
		///	IsCompleted, ManagerId, ReasonId, ProjectId, ProjectTitle,
		///	StateId, CompletionTypeId, Identifier,
		/// ContainerName, IsOverdue, IsNewMessage, ContactUid, OrgUid, ClientName,
		/// CategoryId, CategoryName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsForManagerViewWithChildTodoWithCategories(int PrincipalId,
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
				TimeZoneId, new string[] { "CreationDate", "StartDate", "ActualFinishDate", "ActualStartDate" },
				"IncidentsGetForManagerViewWithChildToDoWithCategories",
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

		#region GetListIncidentsForResourceViewDataTable
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate,
		/// ActualFinishDate, ActualStartDate, PercentCompleted, 
		/// TaskTime, TotalMinutes, TotalApproved, 
		/// IsCompleted, ManagerId, ReasonId, ProjectId, StateId, CompletionTypeId,
		///	Identifier, IsOverdue, IsNewMessage, ContactUid, OrgUid, ClientName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsForResourceViewDataTable(int userId,
			int timeZoneId, int languageId, int managerId, int projectId, int categoryId,
			bool showActive, DateTime dtCompleted, DateTime dtUpcoming, 
			PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			object obj_dtCompleted = (dtCompleted <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCompleted;
			object obj_dtUpcoming = (dtUpcoming >= DateTime.MaxValue.AddDays(-1)) ? null : (object)dtUpcoming;
			return DbHelper2.RunSpDataTable(
				timeZoneId, new string[] { "CreationDate", "StartDate", "ActualFinishDate", "ActualStartDate" },
				"IncidentsGetForResourceView",
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

		#region GetListIncidentsForResourceViewWithCategories
		/// <summary>
		///	ItemId, Title, PriorityId, PriorityName, ItemType, CreationDate, StartDate, FinishDate,
		/// ActualFinishDate, ActualStartDate, PercentCompleted, 
		/// TaskTime, TotalMinutes, TotalApproved, 
		/// IsCompleted, ManagerId, ReasonId, ProjectId, StateId, CompletionTypeId,
		///	Identifier, IsOverdue, IsNewMessage, ContactUid, OrgUid, ClientName, CategoryId, CategoryName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsForResourceViewWithCategories(int userId,
			int timeZoneId, int languageId, int managerId, int projectId, int categoryId,
			bool showActive, DateTime dtCompleted, DateTime dtUpcoming,
			PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			object obj_dtCompleted = (dtCompleted <= DateTime.MinValue.AddDays(1)) ? null : (object)dtCompleted;
			object obj_dtUpcoming = (dtUpcoming >= DateTime.MaxValue.AddDays(-1)) ? null : (object)dtUpcoming;
			return DbHelper2.RunSpDataTable(
				timeZoneId, new string[] { "CreationDate", "StartDate", "ActualFinishDate", "ActualStartDate" },
				"IncidentsGetForResourceViewWithCategories",
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


		#region AddResponsible
		public static void AddResponsible(int IncidentId, int PrincipalId)
		{
			DbHelper2.RunSp("IncidentResponsibleAdd",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId));
		}
		#endregion

		#region DeleteResponsible
		public static void DeleteResponsible(int IncidentId, int PrincipalId)
		{
			DbHelper2.RunSp("IncidentResponsibleDelete",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId));
		}
		#endregion

		#region GetResponsibleByUser
		/// <summary>
		/// Reader returns fields:
		///	 MustBeConfirmed, ResponsePending
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetResponsibleByUser(int IncidentId, int UserId)
		{
			return DbHelper2.RunSpDataReader("IncidentResponsibleGetByUser",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ReplyResponsible
		[Obsolete]
		public static void ReplyResponsible(int IncidentId, int UserId, DateTime LastSavedDate)
		{
			DbHelper2.RunSp("IncidentResponsibleReply",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LastSavedDate", SqlDbType.DateTime, LastSavedDate));
		}
		#endregion

		#region GetResponsibleGroup
		/// <summary>
		/// Reader returns fields:
		///  ResourceId, IncidentId, PrincipalId, IsGroup, MustBeConfirmed, ResponsePending, 
		///  IsConfirmed, IsExternal
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetResponsibleGroup(int IncidentId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[] { "LastSavedDate" },
				"IncidentResponsibleGroupGet",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId));
		}
		#endregion

		#region GetResponsibleGroupDataTable
		/// <summary>
		///  ResourceId, IncidentId, PrincipalId, IsGroup, MustBeConfirmed, ResponsePending, 
		///  IsConfirmed, IsExternal,
		/// </summary>
		/// <returns></returns>
		public static DataTable GetResponsibleGroupDataTable(int IncidentId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "LastSavedDate" },
				"IncidentResponsibleGroupGet",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId));
		}
		#endregion

		#region GetListIncidentStates
		/// <summary>
		/// Reader returns fields:
		///		StateId, StateName 
		/// </summary>
		public static IDataReader GetListIncidentStates(int StateId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader("IncidentStatesGet",
				DbHelper2.mp("@StateId", SqlDbType.Int, StateId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListIncidentStatesDataTable
		/// <summary>
		/// Reader returns fields:
		///		StateId, StateName 
		/// </summary>
		public static DataTable GetListIncidentStatesDataTable(int StateId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable("IncidentStatesGet",
				DbHelper2.mp("@StateId", SqlDbType.Int, StateId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListIncidentsByIncidentBox
		/// <summary>
		/// Reader returns fields:
		///		IncidentId 
		/// </summary>
		public static IDataReader GetListIncidentsByIncidentBox(int IncidentBoxId)
		{
			return DbHelper2.RunSpDataReader("IncidentsGetByIncidentBox",
				DbHelper2.mp("@IncidentBoxId", SqlDbType.Int, IncidentBoxId));
		}
		#endregion

		#region UpdateIdentifier
		public static void UpdateIdentifier(int IncidentId, string Identifier)
		{
			DbHelper2.RunSp("IncidentUpdateIdentifier",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, (int)IncidentId),
				DbHelper2.mp("@Identifier", SqlDbType.NVarChar, 300, Identifier));
		}
		#endregion

		#region GetListIncidentResponsibles
		/// <summary>
		///  UserId, UserName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentResponsibles()
		{
			return DbHelper2.RunSpDataReader("IncidentResponsiblesGetAll");
		}

		/// <summary>
		///  UserId, UserName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentResponsiblesDT()
		{
			return DbHelper2.RunSpDataTable("IncidentResponsiblesGetAll");
		}
		#endregion

		#region GetListIncidentResponsiblesForPartner
		/// <summary>
		///  UserId, UserName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentResponsiblesForPartner(int userId)
		{
			return DbHelper2.RunSpDataReader("IncidentResponsiblesGetAllForPartner",
				DbHelper2.mp("@Userid", SqlDbType.Int, userId));
		}

		/// <summary>
		///  UserId, UserName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentResponsiblesForPartnerDT(int userId)
		{
			return DbHelper2.RunSpDataTable("IncidentResponsiblesGetAllForPartner",
				DbHelper2.mp("@Userid", SqlDbType.Int, userId));
		}
		#endregion

		#region GetListIncidentCreatorsForPartner
		/// <summary>
		///  UserId, UserName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentCreatorsForPartner(int userId)
		{
			return DbHelper2.RunSpDataReader("IncidentCreatorsGetForPartner",
				DbHelper2.mp("@Userid", SqlDbType.Int, userId));
		}
		#endregion

		#region GetListIncidentManagersForPartner
		/// <summary>
		///  UserId, UserName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentManagersForPartner(int userId)
		{
			return DbHelper2.RunSpDataReader("IncidentManagersGetForPartner",
				DbHelper2.mp("@Userid", SqlDbType.Int, userId));
		}
		#endregion

		#region RecalculateExpectedResponseDate
		public static void RecalculateExpectedResponseDate(int incidentId)
		{
			DbHelper2.RunSp("IncidentRecalculateExpectedResponseDate",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, incidentId));
		}
		#endregion

		#region RecalculateExpectedAssignDate
		public static void RecalculateExpectedAssignDate(int incidentId)
		{
			DbHelper2.RunSp("IncidentRecalculateExpectedAssignDate",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, incidentId));
		}
		#endregion

		#region RecalculateCurrentResponsible
		/// <summary>
		/// Recalculates the current responsible.
		/// </summary>
		/// <param name="incidentId">The incident id.</param>
		public static void RecalculateCurrentResponsible(int incidentId)
		{
			DbHelper2.RunSp("IncidentRecalculateCurrentResponsible",
				DbHelper2.mp("@incidentId", SqlDbType.Int, incidentId));
		}
		#endregion

		#region RecalculateCurrentResponsibleByIncidentBox
		/// <summary>
		/// Recalculates the current responsible by incident box.
		/// </summary>
		/// <param name="incidentBoxId">The incident box id.</param>
		public static void RecalculateCurrentResponsibleByIncidentBox(int incidentBoxId)
		{
			DbHelper2.RunSp("IncidentsRecalculateCurrentResponsibleByIncidentBox",
				DbHelper2.mp("@incidentBoxId", SqlDbType.Int, incidentBoxId));
		}
		#endregion

		#region GetIncidentDatesForProject
		/// <summary>
		/// Reader returns fields:
		///  StartDate, FinishDate
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetIncidentDatesForProject(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("IncidentDatesGetForProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion
	}
}
