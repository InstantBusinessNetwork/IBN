using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Resources;

using Mediachase.Ibn;
using Mediachase.IBN.Database;
using Mediachase.Ibn.Assignments;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Business.ControlSystem;

namespace Mediachase.IBN.Business
{
	#region enum SystemEventTypes 
	public enum SystemEventTypes
	{
		// Диапазон для ToDo: 0-99
		Todo_Created = 1,
		Todo_Deleted = 2,
		//		Todo_Updated = 3,
		Todo_Updated_GeneralInfo = 4,
		Todo_Updated_Priority = 5,
		Todo_Updated_ConfigurationInfo = 6,
		Todo_Updated_State = 8,
		Todo_Updated_Percent = 14,
		Todo_Updated_MetaFields = 15,
		Todo_Updated_Timeline = 16,
		//		Todo_Updated_ResourceList = 17,
		Todo_Updated_ResourceList_AssignmentAdded = 18,
		Todo_Updated_ResourceList_AssignmentDeleted = 19,
		Todo_Updated_ResourceList_RequestAdded = 20,
		Todo_Updated_ResourceList_RequestAccepted = 21,
		Todo_Updated_ResourceList_RequestDenied = 22,
		//		Todo_Updated_FileList = 24,
		Todo_Updated_FileList_FileAdded = 25,
		Todo_Updated_FileList_FileDeleted = 26,
		Todo_Updated_FileList_FileUpdated = 27,
		//		Todo_Updated_CommentList = 28,
		Todo_Updated_CommentList_CommentAdded = 29,
		Todo_Updated_CommentList_CommentDeleted = 30,
		Todo_Updated_CommentList_CommentUpdated = 31,
		//Todo_Updated_Manager = 32,
		Todo_Updated_Manager_ManagerAdded = 33,
		Todo_Updated_Manager_ManagerDeleted = 34,
		Todo_Updated_GeneralCategories = 35,
		Todo_StartDate_Arrived = 80,
		Todo_FinishDate_Arrived = 81,

		// Диапазон для Tasks: 100-199
		Task_Created = 101,
		Task_Deleted = 102,
		//		Task_Updated = 103,
		Task_Updated_GeneralInfo = 104,
		Task_Updated_Priority = 105,
		Task_Updated_ConfigurationInfo = 106,
		Task_Updated_State = 108,
		Task_Updated_Percent = 114,
		Task_Updated_MetaFields = 115,
		Task_Updated_Timeline = 116,
		//		Task_Updated_ResourceList = 117,
		Task_Updated_ResourceList_AssignmentAdded = 118,
		Task_Updated_ResourceList_AssignmentDeleted = 119,
		Task_Updated_ResourceList_RequestAdded = 120,
		Task_Updated_ResourceList_RequestAccepted = 121,
		Task_Updated_ResourceList_RequestDenied = 122,
		//		Task_Updated_FileList = 124,
		Task_Updated_FileList_FileAdded = 125,
		Task_Updated_FileList_FileDeleted = 126,
		Task_Updated_FileList_FileUpdated = 127,
		//		Task_Updated_CommentList = 128,
		Task_Updated_CommentList_CommentAdded = 129,
		Task_Updated_CommentList_CommentDeleted = 130,
		Task_Updated_CommentList_CommentUpdated = 131,
		//		Task_Updated_TodoList = 132,
		Task_Updated_TodoList_TodoAdded = 133,
		Task_Updated_TodoList_TodoDeleted = 134,
		//		Task_Updated_PredecessorList = 136,
		Task_Updated_PredecessorList_PredecessorAdded = 137,
		Task_Updated_PredecessorList_PredecessorDeleted = 138,
		Task_Updated_PredecessorList_PredecessorLagUpdated = 139,
		//		Task_Updated_SuccessorList = 140,
		Task_Updated_SuccessorList_SuccessorAdded = 141,
		Task_Updated_SuccessorList_SuccessorDeleted = 142,
		Task_Updated_SuccessorList_SuccessorLagUpdated = 143,
		Task_Updated_GeneralCategories = 144,
		Task_StartDate_Arrived = 180,
		Task_FinishDate_Arrived = 181,

		// Диапазон для CalendarEntries: 200-299
		CalendarEntry_Created = 201,
		CalendarEntry_Deleted = 202,
		//		CalendarEntry_Updated = 203,
		CalendarEntry_Updated_GeneralInfo = 204,
		CalendarEntry_Updated_Priority = 205,
		CalendarEntry_Updated_State = 208,
		CalendarEntry_Updated_MetaFields = 215,
		CalendarEntry_Updated_Timeline = 216,
		//		CalendarEntry_Updated_ResourceList = 217,
		CalendarEntry_Updated_ResourceList_AssignmentAdded = 218,
		CalendarEntry_Updated_ResourceList_AssignmentDeleted = 219,
		CalendarEntry_Updated_ResourceList_RequestAdded = 220,
		CalendarEntry_Updated_ResourceList_RequestAccepted = 221,
		CalendarEntry_Updated_ResourceList_RequestDenied = 222,
		//		CalendarEntry_Updated_FileList = 224,
		CalendarEntry_Updated_FileList_FileAdded = 225,
		CalendarEntry_Updated_FileList_FileDeleted = 226,
		CalendarEntry_Updated_FileList_FileUpdated = 227,
		//		CalendarEntry_Updated_CommentList = 228,
		CalendarEntry_Updated_CommentList_CommentAdded = 229,
		CalendarEntry_Updated_CommentList_CommentDeleted = 230,
		CalendarEntry_Updated_CommentList_CommentUpdated = 231,
		//CalendarEntry_Updated_Manager = 232,
		CalendarEntry_Updated_Manager_ManagerAdded = 233,
		CalendarEntry_Updated_Manager_ManagerDeleted = 234,
		CalendarEntry_Updated_GeneralCategories = 235,
		CalendarEntry_StartDate_Arrived = 280,
		CalendarEntry_FinishDate_Arrived = 281,

		// Диапазон для Projects: 300-399
		Project_Created = 301,
		Project_Deleted = 302,
		//		Project_Updated = 303,
		Project_Updated_GeneralInfo = 304,
		Project_Updated_Priority = 305,
		Project_Updated_ConfigurationInfo = 306,
		Project_Updated_Phase = 307,
		Project_Updated_Status = 308,
		Project_Updated_RiskLevel = 309,
		Project_Updated_GeneralCategories = 310,
		Project_Updated_ProjectCategories = 311,
		Project_Updated_ActualStartDate = 312,
		Project_Updated_ActualFinishDate = 313,
		Project_Updated_Percent = 314,
		Project_Updated_MetaFields = 315,
		Project_Updated_TargetTimeline = 316,
		//		Project_Updated_TeamMemberList = 317,
		Project_Updated_TeamMemberList_TeamMemberAdded = 318,
		Project_Updated_TeamMemberList_TeamMemberDeleted = 319,
		Project_Updated_TeamMemberList_TeamMemberUpdated = 320,
		Project_Updated_Client = 321,
		Project_Updated_ProjectGroups = 323,
		//		Project_Updated_FileList = 324,
		Project_Updated_FileList_FileAdded = 325,
		Project_Updated_FileList_FileDeleted = 326,
		Project_Updated_FileList_FileUpdated = 327,
		//		Project_Updated_CommentList = 328,
		Project_Updated_CommentList_CommentAdded = 329,
		Project_Updated_CommentList_CommentDeleted = 330,
		Project_Updated_CommentList_CommentUpdated = 331,
		//		Project_Updated_TodoList = 332,
		Project_Updated_TodoList_TodoAdded = 333,
		Project_Updated_TodoList_TodoDeleted = 334,
		//		Project_Updated_Manager = 335,
		Project_Updated_Manager_ManagerAdded = 336,
		Project_Updated_Manager_ManagerDeleted = 337,
		//		Project_Updated_ExecutiveManager = 346,
		Project_Updated_ExecutiveManager_ExecutiveManagerAdded = 347,
		Project_Updated_ExecutiveManager_ExecutiveManagerDeleted = 348,
		//		Project_Updated_SponsorList = 349,
		Project_Updated_SponsorList_SponsorAdded = 350,
		Project_Updated_SponsorList_SponsorDeleted = 351,
		//		Project_Updated_StakeholderList = 352,
		Project_Updated_StakeholderList_StakeholderAdded = 353,
		Project_Updated_StakeholderList_StakeholderDeleted = 354,
		//		Project_Updated_TaskList = 355,
		Project_Updated_TaskList_TaskAdded = 356,
		Project_Updated_TaskList_TaskDeleted = 357,
		//		Project_Updated_CalendarEntryList = 358,
		Project_Updated_CalendarEntryList_CalendarEntryAdded = 359,
		Project_Updated_CalendarEntryList_CalendarEntryDeleted = 360,
		//		Project_Updated_DocumentList = 361,
		Project_Updated_DocumentList_DocumentAdded = 362,
		Project_Updated_DocumentList_DocumentDeleted = 363,
		//		Project_Updated_IssueList = 364,
		Project_Updated_IssueList_IssueAdded = 365,
		Project_Updated_IssueList_IssueDeleted = 366,
		//		Project_Updated_ListList = 367,
		Project_Updated_ListList_ListAdded = 368,
		Project_Updated_ListList_ListDeleted = 369,
		//		Project_Updated_RelatedProjectList = 370,
		Project_Updated_RelatedProjectList_RelatedProjectAdded = 371,
		Project_Updated_RelatedProjectList_RelatedProjectDeleted = 372,
		Project_TargetStartDate_Arrived = 380,
		Project_TargetFinishDate_Arrived = 381,

		// Диапазон для Issues: 400-499
		Issue_Created = 401, // Info, Personal
		Issue_Deleted = 402,
		//Issue_Updated = 403,
		Issue_Updated_GeneralInfo = 404,
		Issue_Updated_Priority = 405,
		//Issue_Updated_ResolutionInfo = 406,
		Issue_Updated_Status = 408,
		Issue_Updated_GeneralCategories = 410,
		Issue_Updated_IssueCategories = 411,
		Issue_Updated_MetaFields = 415,
		//Issue_Updated_ResourceList = 417,
		Issue_Updated_ResourceList_AssignmentAdded = 418,
		Issue_Updated_ResourceList_AssignmentDeleted = 419,
		Issue_Updated_ResourceList_RequestAdded = 420,
		Issue_Updated_ResourceList_RequestAccepted = 421,
		Issue_Updated_ResourceList_RequestDenied = 422,
		//Issue_Updated_FileList = 424,
		Issue_Updated_FileList_FileAdded = 425,
		Issue_Updated_FileList_FileDeleted = 426,
		Issue_Updated_FileList_FileUpdated = 427,
		//Issue_Updated_CommentList = 428,
		Issue_Updated_CommentList_CommentAdded = 429,
		Issue_Updated_CommentList_CommentDeleted = 430,
		Issue_Updated_CommentList_CommentUpdated = 431,
		//Issue_Updated_TodoList = 432,
		Issue_Updated_TodoList_TodoAdded = 433,
		Issue_Updated_TodoList_TodoDeleted = 434,
		//Issue_Updated_Manager = 435,
		//Issue_Updated_Forum = 450,
		//Issue_Updated_RelatedIssueList = 470,
		Issue_Updated_RelatedIssueList_RelatedIssueAdded = 471,
		Issue_Updated_RelatedIssueList_RelatedIssueDeleted = 472,
		Issue_Updated_Manager_ManagerAdded = 473, // Personal
		Issue_Updated_Manager_ManagerDeleted = 474, // Personal
		Issue_Updated_Forum_MessageAdded = 475, // Info

		Issue_Updated_Box = 476, // Info
		//Issue_Updated_Responsible = 477,
		Issue_Updated_Responsible_Changed = 478, // Info
		Issue_Updated_Responsible_Requested = 479, // Personal
		Issue_Updated_Responsible_Assigned = 480, // Personal
		Issue_Updated_Responsible_Resigned = 481, // Personal
		Issue_Updated_Responsible_Rejected = 482, // Info
		//Issue_Updated_Controller = 483,
		Issue_Updated_Controller_Assigned = 484, // Personal
		Issue_Updated_Controller_Resigned = 485, // Personal
		Issue_Closed = 486, // External Recipient

		// Диапазон для Documents: 500-599
		Document_Created = 501,
		Document_Deleted = 502,
		//		Document_Updated = 503,
		Document_Updated_GeneralInfo = 504,
		Document_Updated_Priority = 505,
		Document_Updated_State = 507,
		Document_Updated_Status = 508,
		Document_Updated_GeneralCategories = 510,
		Document_Updated_MetaFields = 515,
		//		Document_Updated_ResourceList = 517,
		Document_Updated_ResourceList_AssignmentAdded = 518,
		Document_Updated_ResourceList_AssignmentDeleted = 519,
		Document_Updated_ResourceList_RequestAdded = 520,
		Document_Updated_ResourceList_RequestAccepted = 521,
		Document_Updated_ResourceList_RequestDenied = 522,
		//		Document_Updated_FileList = 524,
		Document_Updated_FileList_FileAdded = 525,
		Document_Updated_FileList_FileDeleted = 526,
		Document_Updated_FileList_FileUpdated = 527,
		//		Document_Updated_CommentList = 528,
		Document_Updated_CommentList_CommentAdded = 529,
		Document_Updated_CommentList_CommentDeleted = 530,
		Document_Updated_CommentList_CommentUpdated = 531,
		//		Document_Updated_TodoList = 532,
		Document_Updated_TodoList_TodoAdded = 533,
		Document_Updated_TodoList_TodoDeleted = 534,
		//		Document_Updated_VersionList = 573,
		Document_Updated_VersionList_VersionAdded = 574,

		// Диапазон для Lists: 600-699
		List_Created = 601,
		List_Deleted = 602,
		//		List_Updated = 603,
		List_Updated_Data = 604,
		List_Updated_Structure = 606,
		List_Updated_GeneralInfo = 607,
		List_Updated_Status = 608,
		List_Updated_GeneralCategories = 609,

		// Диапазон для IssueRequest: 700-799
		IssueRequest_Created = 701,
		IssueRequest_Deleted = 702,
		IssueRequest_Updated = 703,
		IssueRequest_Approved = 704, // Info

		// Диапазон для User: 800-899
		User_Activated = 807, // Info, Personal
		User_Created = 801, // Info, Personal
		User_Created_External = 802, // Info, Personal
		User_Created_Partner = 803, // Info, Personal
		User_Created_Request = 804, // Info
		User_Deleted = 805, // Info, Personal
		User_Deactivated = 806, // Info, Personal


		// Диапазон для Assignments: 900-999
		Assignment_Created = 901,
		Assignment_Deleted = 902,
		Assignment_Updated_GeneralInfo = 904,
		Assignment_Updated_Priority = 905,
		Assignment_Updated_Status = 908,
		Assignment_Updated_FinishDate = 916,
		Assignment_Updated_Participant_Assigned = 933, // Personal, Info
		Assignment_Updated_Participant_Resigned = 934, // Personal, Info
		Assignment_FinishDate_Arrived = 981
	}
	#endregion

	#region enum MessageTypes
	public enum MessageTypes
	{
		Info = 1,
		//Manager = 2,
		Resource = 3
	}
	#endregion
	
	#region enum DeliveryType
	public enum DeliveryType
	{
		Email = 1,
		IBN = 2
	}
	#endregion

	
	#region class SystemEvents
	public class SystemEvents
	{
		[ThreadStatic]
		private static bool _hasNewEvent;

		static SystemEvents()
		{
			DbTransaction.Committed += new EventHandler<DbTransactionEventArgs>(DbTransactionCommitted);
		}

		#region AddSystemEvents
		public static void AddSystemEvents(SystemEventTypes eventType, int objectId, Dictionary<string, string> additionalValues)
		{
			AddSystemEvents(eventType, objectId, null, -1, null, null, null, additionalValues);
		}

		public static void AddSystemEvents(SystemEventTypes eventType, int objectId)
		{
			AddSystemEvents(eventType, objectId, null, -1, null, null, null, null);
		}

		public static void AddSystemEvents(SystemEventTypes eventType, Guid objectUid)
		{
			AddSystemEvents(eventType, null, objectUid, -1, null, null, null, null);
		}

		public static void AddSystemEvents(SystemEventTypes eventType, int objectId, int relObjectId)
		{
			AddSystemEvents(eventType, objectId, null, relObjectId, null, null, null, null);
		}

		public static void AddSystemEvents(SystemEventTypes eventType, Guid objectUid, int relObjectId)
		{
			AddSystemEvents(eventType, null, objectUid, relObjectId, null, null, null, null);
		}

		public static void AddSystemEvents(SystemEventTypes eventType, int objectId, int relObjectId, ArrayList excludeUsers)
		{
			AddSystemEvents(eventType, objectId, null, relObjectId, null, null, excludeUsers, null);
		}

		public static void AddSystemEvents(SystemEventTypes eventType, int objectId, string emailTo)
		{
			AddSystemEvents(eventType, objectId, null, -1, emailTo, null, null, null);
		}

		public static void AddSystemEvents(SystemEventTypes eventType, int objectId, string emailTo, string emailFrom)
		{
			AddSystemEvents(eventType, objectId, null, -1, emailTo, emailFrom, null, null);
		}

		public static void AddSystemEvents(SystemEventTypes eventType, int? objectId, Guid? objectUid, int relObjectId, string emailTo, string emailFrom, ArrayList excludeUsers, Dictionary<string, string> additionalValues)
		{
			// User can be unregistered
			int CurrentUserId = -1;
			if(Security.CurrentUser != null)
				CurrentUserId = Security.CurrentUser.UserID;

			ObjectTypes objectType, relObjectType;
			string ObjectTitle = string.Empty;
			string RelObjectTitle = string.Empty;
			string XMLData = Alerts2.GetXmlData(eventType, objectId, objectUid, relObjectId, out objectType, out relObjectType, ref ObjectTitle, ref RelObjectTitle, additionalValues);

			// Здесь будем хранить обработанных пользователей и пользователей, которых не надо обрабатывать
			ArrayList processedUsers = new ArrayList();

			if (CurrentUserId > 0 && !Alerts2.SendToCurrentUser)
				processedUsers.Add(CurrentUserId);

			// Получатели info-сообщений
			ArrayList infoRecipients = new ArrayList();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				int eventId = DBSystemEvents.AddSystemEvents((int)eventType, objectId, objectUid, ObjectTitle, relObjectId, 
					RelObjectTitle, CurrentUserId, XMLData, DbContext.Current.TransactionId);

				#region Персональные сообщения
				if (
					eventType == SystemEventTypes.CalendarEntry_Updated_Manager_ManagerAdded
					|| eventType == SystemEventTypes.CalendarEntry_Updated_Manager_ManagerDeleted
					|| eventType == SystemEventTypes.CalendarEntry_Updated_ResourceList_AssignmentAdded
					|| eventType == SystemEventTypes.CalendarEntry_Updated_ResourceList_AssignmentDeleted
					|| eventType == SystemEventTypes.CalendarEntry_Updated_ResourceList_RequestAdded
					|| eventType == SystemEventTypes.Document_Updated_ResourceList_AssignmentAdded
					|| eventType == SystemEventTypes.Document_Updated_ResourceList_AssignmentDeleted
					|| eventType == SystemEventTypes.Document_Updated_ResourceList_RequestAdded
					|| eventType == SystemEventTypes.Issue_Created
					|| eventType == SystemEventTypes.Issue_Updated_Forum_MessageAdded
					|| eventType == SystemEventTypes.Issue_Updated_Manager_ManagerAdded
					|| eventType == SystemEventTypes.Issue_Updated_Manager_ManagerDeleted
					|| eventType == SystemEventTypes.Issue_Updated_ResourceList_AssignmentAdded
					|| eventType == SystemEventTypes.Issue_Updated_ResourceList_AssignmentDeleted
					|| eventType == SystemEventTypes.Issue_Updated_ResourceList_RequestAdded
					|| eventType == SystemEventTypes.Issue_Updated_Responsible_Requested
					|| eventType == SystemEventTypes.Issue_Updated_Responsible_Assigned
					|| eventType == SystemEventTypes.Issue_Updated_Responsible_Resigned
					|| eventType == SystemEventTypes.Issue_Updated_Controller_Assigned
					|| eventType == SystemEventTypes.Issue_Updated_Controller_Resigned
					|| eventType == SystemEventTypes.Project_Updated_ExecutiveManager_ExecutiveManagerAdded
					|| eventType == SystemEventTypes.Project_Updated_ExecutiveManager_ExecutiveManagerDeleted
					|| eventType == SystemEventTypes.Project_Updated_Manager_ManagerAdded
					|| eventType == SystemEventTypes.Project_Updated_Manager_ManagerDeleted
					|| eventType == SystemEventTypes.Project_Updated_SponsorList_SponsorAdded
					|| eventType == SystemEventTypes.Project_Updated_SponsorList_SponsorDeleted
					|| eventType == SystemEventTypes.Project_Updated_StakeholderList_StakeholderAdded
					|| eventType == SystemEventTypes.Project_Updated_StakeholderList_StakeholderDeleted
					|| eventType == SystemEventTypes.Project_Updated_TeamMemberList_TeamMemberAdded
					|| eventType == SystemEventTypes.Project_Updated_TeamMemberList_TeamMemberDeleted
					|| eventType == SystemEventTypes.Todo_Updated_Manager_ManagerAdded
					|| eventType == SystemEventTypes.Todo_Updated_Manager_ManagerDeleted
					|| eventType == SystemEventTypes.Todo_Updated_ResourceList_AssignmentAdded
					|| eventType == SystemEventTypes.Todo_Updated_ResourceList_AssignmentDeleted
					|| eventType == SystemEventTypes.Todo_Updated_ResourceList_RequestAdded
					|| eventType == SystemEventTypes.Task_Updated_ResourceList_AssignmentAdded
					|| eventType == SystemEventTypes.Task_Updated_ResourceList_AssignmentDeleted
					|| eventType == SystemEventTypes.Task_Updated_ResourceList_RequestAdded
					|| eventType == SystemEventTypes.User_Activated
					|| eventType == SystemEventTypes.User_Created
					|| eventType == SystemEventTypes.User_Created_External
					|| eventType == SystemEventTypes.User_Created_Partner
					|| eventType == SystemEventTypes.User_Deleted
					|| eventType == SystemEventTypes.User_Deactivated
					|| eventType == SystemEventTypes.Assignment_Updated_Participant_Assigned
					|| eventType == SystemEventTypes.Assignment_Updated_Participant_Resigned
					)
				{
					ArrayList users = new ArrayList();

					if(objectType == ObjectTypes.User)
						users.Add(objectId);

					if(relObjectType == ObjectTypes.User)
					{
						if(User.IsGroup(relObjectId))
						{
							// this method doesn't return inactive users
							using (IDataReader reader = SecureGroup.GetListAllUsersInGroup(relObjectId, false))
							{
								while (reader.Read())
									users.Add((int)reader["UserId"]);
							}
						}
						else
						{
							if (User.GetUserActivity(relObjectId) == User.UserActivity.Active)
								users.Add(relObjectId);
						}
					}

					foreach(int userId in users)
					{
						if(!processedUsers.Contains(userId))
						{
							AddSystemEventRecipient(eventId, userId, MessageTypes.Resource);
							processedUsers.Add(userId);
						}
					}
					if(emailTo != null)
						AddSystemEventRecipient(eventId, MessageTypes.Resource, emailTo, emailFrom);
				}
				#endregion

				CollectAllObjectSubscribers(eventType, objectType, objectId, objectUid, processedUsers, infoRecipients);

				foreach(int userId in infoRecipients)
				{
					if ((excludeUsers == null || !excludeUsers.Contains(userId)) && CheckRights(eventType, objectType, relObjectType, objectId, relObjectId,objectUid, userId))
						AddSystemEventRecipient(eventId, userId, MessageTypes.Info);
				}
				
				_hasNewEvent = true;

				tran.Commit();
			}
		}
		#endregion

		#region GetSystemEvents
		/// <summary>
		///  SystemEventId, EventTypeId, ObjectId, ObjectUid, ObjectTitle, RelObjectId, RelObjectTitle, UserId, 
		///  Dt, ObjectTypeId, RelObjectTypeId, SystemEventTitle, ObjectCode
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetSystemEvents(DateTime StartDate, DateTime EndDate)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			StartDate = DBCommon.GetUTCDate(TimeZoneId, StartDate);
			EndDate = DBCommon.GetUTCDate(TimeZoneId, EndDate);
			return DBSystemEvents.GetSystemEvents(Security.CurrentUser.UserID, StartDate, EndDate, TimeZoneId);
		}

		public static DataTable GetSystemEventsDT(DateTime StartDate, DateTime EndDate)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			StartDate = DBCommon.GetUTCDate(TimeZoneId, StartDate);
			EndDate = DBCommon.GetUTCDate(TimeZoneId, EndDate);
			return DBSystemEvents.GetSystemEventsDT(Security.CurrentUser.UserID, StartDate, EndDate, TimeZoneId);
		}
		#endregion

		#region CheckToDoSubscribtion
		public static bool CheckToDoSubscribtion(SystemEventTypes EventTypeId, int ObjectId, int UserId, 
			bool CheckManagerSubscription, bool CheckResourcesSubscription, bool CheckGlobalSubscription)
		{
			return DBSystemEvents.CheckToDoSubscribtion((int)EventTypeId, ObjectId, UserId, CheckManagerSubscription, CheckResourcesSubscription, CheckGlobalSubscription);
		}
		#endregion

		#region AddSystemEventRecipient
		public static void AddSystemEventRecipient(int SystemEventId, int UserId, MessageTypes MessageTypeId)
		{
			AddSystemEventRecipient(SystemEventId, UserId, MessageTypeId, null, null);
		}
		public static void AddSystemEventRecipient(int SystemEventId, MessageTypes MessageTypeId, string emailTo, string emailFrom)
		{
			AddSystemEventRecipient(SystemEventId, -1, MessageTypeId, emailTo, emailFrom);
		}
		public static void AddSystemEventRecipient(
			int SystemEventId,
			int UserId,
			MessageTypes MessageTypeId,
			string emailTo,
			string emailFrom)
		{
			DBSystemEvents.AddSystemEventRecipient(SystemEventId, UserId, (int)MessageTypeId, emailTo, emailFrom);
		}
		#endregion

		#region CollectAllObjectSubscribers
		private static void CollectAllObjectSubscribers(SystemEventTypes eventType, ObjectTypes objectType, int? objectId, Guid? objectUid, ArrayList processedUsers, ArrayList infoRecipients)
		{
			using (IDataReader reader = GetAllObjectSubscribersReader(eventType, objectType, objectId, objectUid))
			{
				if(reader != null)
				{
					while(reader.Read())
					{
						int userId = (int)reader["UserId"];
						if(!processedUsers.Contains(userId))
						{
							processedUsers.Add(userId);
							infoRecipients.Add(userId);
						}
					}
				}
			}
		}
		#endregion

		#region GetAllObjectSubscribersReader
		/// <summary>
		///  UserId, Email
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetAllObjectSubscribersReader(SystemEventTypes eventTypeId, ObjectTypes objectType, int? objectId, Guid? objectUid)
		{
			switch(objectType)
			{
				case ObjectTypes.ToDo:
					return DBSystemEvents.GetSubscribersForToDo((int)eventTypeId, objectId.Value, true, true, true);
				case ObjectTypes.Document:
					return DBSystemEvents.GetSubscribersForDocument((int)eventTypeId, objectId.Value, true, true, true);
				case ObjectTypes.Issue:
					return DBSystemEvents.GetSubscribersForIssue((int)eventTypeId, objectId.Value, true, true, true, true, true, true);
				case ObjectTypes.List:
					return DBSystemEvents.GetSubscribersForList((int)eventTypeId, objectId.Value);
				case ObjectTypes.Project:
					return DBSystemEvents.GetSubscribersForProject((int)eventTypeId, objectId.Value, true, true, true, true, true, true);
				case ObjectTypes.Task:
					return DBSystemEvents.GetSubscribersForTask((int)eventTypeId, objectId.Value, true, true, true);
				case ObjectTypes.CalendarEntry:
					return DBSystemEvents.GetSubscribersForCalendarEntry((int)eventTypeId, objectId.Value, true, true, true);
				case ObjectTypes.IssueRequest:
					return DBSystemEvents.GetSubscribersForPop3MailRequests((int)eventTypeId, objectId.Value);
				case ObjectTypes.User:
					return DBSystemEvents.GetSubscribersForUser((int)eventTypeId, objectId.Value);
				case ObjectTypes.Assignment:
					return DBSystemEvents.GetSubscribersForAssignment((int)eventTypeId, objectUid.Value, true, true, true);
				default:
					return null;
			}
		}
		#endregion

		#region CheckRights
		private static bool CheckRights(SystemEventTypes eventType, ObjectTypes objectType, ObjectTypes relObjectType, int? objectId, int? relObjectId, Guid? objectUid, int userId)
		{
			// если нужна дополнительная проверка в зависимости от типа события, то её нужно делать здесь
			bool retval = false;

			if (relObjectType == ObjectTypes.File_FileStorage && relObjectId != null)
			{
				FileInfo fileInfo = null;
				//Получаем оригинальный файл
				using (IDataReader reader = Mediachase.IBN.Database.ControlSystem.DBFile.GetById(0, relObjectId.Value))
				{
					if (reader.Read())
					{
						fileInfo = new Mediachase.IBN.Business.ControlSystem.FileInfo(reader);
					}
				
				}

				if (fileInfo != null)
				{
					return CheckFileStorageRight(fileInfo, "Read", userId);
				}
			}

			switch(objectType) 
			{
				case ObjectTypes.ToDo:
					retval = ToDo.CanRead(objectId.Value, userId);
					break;
				case ObjectTypes.CalendarEntry:
					retval = CalendarEntry.CanRead(objectId.Value, userId);
					break;
				case ObjectTypes.Document:
					retval = Document.CanRead(objectId.Value, userId);
					break;
				case ObjectTypes.Issue:
					retval = Incident.CanRead(objectId.Value, userId);
					break;
				case ObjectTypes.List:
					retval = ListInfoBus.CanRead(objectId.Value, userId);
					break;
				case ObjectTypes.Project:
					retval = Project.CanRead(objectId.Value, userId);
					break;
				case ObjectTypes.Task:
					retval = Task.CanRead(objectId.Value, userId);
					break;
				case ObjectTypes.IssueRequest:
					retval = IssueRequest.CanUse(userId);
					break;
				case ObjectTypes.User:
					retval = Security.IsUserInGroup(userId, InternalSecureGroups.Administrator);
					break;
				case ObjectTypes.Assignment:
					AssignmentEntity entity = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, (PrimaryKeyId)objectUid);
					if (entity != null && entity.OwnerDocumentId.HasValue)
						retval = Document.CanRead(entity.OwnerDocumentId.Value, userId);
					break;
				default:
					// Для остальных временно разрешаем любой доступ
					retval = true;
					break;
			}

			return retval;
		}
		#endregion

		#region GetGlobalSubscriptions
		/// <summary>
		///  EventTypeId, ObjectTypeId, Title, Role_*
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetGlobalSubscriptions(int ObjectTypeId)
		{
			return DBSystemEvents.GetGlobalSubscriptions(ObjectTypeId);
		}

		public static DataTable GetGlobalSubscriptionsDT(int ObjectTypeId)
		{
			return DBSystemEvents.GetGlobalSubscriptionsDT(ObjectTypeId);
		}
		#endregion

		#region AddGlobalSubscriptions
		// SelectedItems - это массив строк вида EventTypeId_ObjectRoleId
		public static void AddGlobalSubscriptions(int ObjectTypeId, ArrayList SelectedItems)
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Administrator))
				throw new AccessDeniedException();

			ArrayList add = new ArrayList(SelectedItems);
			ArrayList del = new ArrayList();
			using (IDataReader reader = DBSystemEvents.GetGlobalSubscriptionsAsList(ObjectTypeId))
			{
				while (reader.Read())
				{
					string itemString = String.Format("{0}_{1}", reader["EventTypeId"].ToString(), reader["ObjectRoleId"].ToString());
					if (add.Contains(itemString))
						add.Remove(itemString);
					else
						del.Add(itemString);
				}
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				foreach(string itemString in del)
				{
					String[] items = itemString.Split('_');
					int EventTypeId = int.Parse(items[0]);
					int ObjectRoleId = int.Parse(items[1]);

					DBSystemEvents.DeleteGlobalSubscription(EventTypeId, ObjectRoleId);
				}

				foreach(string itemString in add)
				{
					String[] items = itemString.Split('_');
					int EventTypeId = int.Parse(items[0]);
					int ObjectRoleId = int.Parse(items[1]);

					DBSystemEvents.AddGlobalSubscription(EventTypeId, ObjectRoleId);
				}

				tran.Commit();
			}
		}
		#endregion

		#region GetPersonalSubscriptions
		/// <summary>
		///  EventTypeId, ObjectTypeId, Title, Role_*, IsDefault
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetPersonalSubscriptions(int ObjectTypeId)
		{
			return DBSystemEvents.GetPersonalSubscriptions(ObjectTypeId, Security.CurrentUser.UserID);
		}

		public static DataTable GetPersonalSubscriptionsDT(int ObjectTypeId)
		{
			return DBSystemEvents.GetPersonalSubscriptionsDT(ObjectTypeId, Security.CurrentUser.UserID);
		}
		#endregion

		#region AddPersonalSubscription
		public static void AddPersonalSubscription(int EventTypeId, ArrayList SelectedItems)
		{
			int UserId = Security.CurrentUser.UserID;

			ArrayList add = new ArrayList(SelectedItems);
			ArrayList del = new ArrayList();
			using (IDataReader reader = DBSystemEvents.GetPersonalSubscriptionDetails(EventTypeId, UserId))
			{
				while (reader.Read())
				{
					int ObjectRoleId = (int)reader["ObjectRoleId"];

					if (add.Contains(ObjectRoleId))
						add.Remove(ObjectRoleId);
					else
						del.Add(ObjectRoleId);
				}
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				int SubscriptionId = DBSystemEvents.AddPersonalSubscription(EventTypeId, UserId);

				foreach(int ObjectRoleId in del)
					DBSystemEvents.DeleteSubscriptionDetails(SubscriptionId, ObjectRoleId);

				foreach(int ObjectRoleId  in add)
					DBSystemEvents.AddSubscriptionDetails(SubscriptionId, ObjectRoleId);

				tran.Commit();
			}
		}
		#endregion

		#region DeletePersonalSubscription
		public static void DeletePersonalSubscription(int EventTypeId)
		{
			DBSystemEvents.DeletePersonalSubscription(EventTypeId, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetPersonalSubscriptionsForObject
		/// <summary>
		///  EventTypeId, ObjectTypeId, Title, Role_*, IsDefault
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetPersonalSubscriptionsForObject(int ObjectTypeId, int ObjectId)
		{
			return DBSystemEvents.GetPersonalSubscriptionsForObject(ObjectTypeId, Security.CurrentUser.UserID, ObjectId);
		}

		public static DataTable GetPersonalSubscriptionsForObjectDT(int ObjectTypeId, int ObjectId)
		{
			return DBSystemEvents.GetPersonalSubscriptionsForObjectDT(ObjectTypeId, Security.CurrentUser.UserID, ObjectId);
		}
		#endregion

		#region AddPersonalSubscriptionForObject
		public static void AddPersonalSubscriptionForObject(int EventTypeId, int ObjectId, ArrayList SelectedItems)
		{
			int UserId = Security.CurrentUser.UserID;

			ArrayList add = new ArrayList(SelectedItems);
			ArrayList del = new ArrayList();
			using (IDataReader reader = DBSystemEvents.GetPersonalSubscriptionDetailsForObject(EventTypeId, UserId, ObjectId))
			{
				while (reader.Read())
				{
					int ObjectRoleId = (int)reader["ObjectRoleId"];

					if (add.Contains(ObjectRoleId))
						add.Remove(ObjectRoleId);
					else
						del.Add(ObjectRoleId);
				}
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				int SubscriptionId = DBSystemEvents.AddPersonalSubscriptionForObject(EventTypeId, UserId, ObjectId);

				foreach(int ObjectRoleId in del)
					DBSystemEvents.DeleteSubscriptionDetails(SubscriptionId, ObjectRoleId);

				foreach(int ObjectRoleId  in add)
					DBSystemEvents.AddSubscriptionDetails(SubscriptionId, ObjectRoleId);

				tran.Commit();
			}
		}
		#endregion

		#region DeletePersonalSubscriptionForObject
		public static void DeletePersonalSubscriptionForObject(int EventTypeId, int ObjectId)
		{
			DBSystemEvents.DeletePersonalSubscriptionForObject(EventTypeId, Security.CurrentUser.UserID, ObjectId);
		}
		#endregion


		#region private static void DbTransactionCommitted(object sender, DbTransactionEventArgs e)
		private static void DbTransactionCommitted(object sender, DbTransactionEventArgs e)
		{
			if (_hasNewEvent)
			{
				_hasNewEvent = false;
				Alerts2.Send(DbContext.Current.TransactionId);
			}
		}
		#endregion

		#region GetMessageTypeName, GetSystemEventName
		private static ResourceManager rmMessageTypes = new ResourceManager("Mediachase.IBN.Business.Resources.MessageTypes", typeof(MessageTypes).Assembly);
		private static ResourceManager rmSystemEvents = new ResourceManager("Mediachase.IBN.Business.Resources.SystemEventTypes", typeof(SystemEvents).Assembly);

		public static string GetMessageTypeName(string type)
		{
			return rmMessageTypes.GetString(type);
		}

		public static string GetSystemEventName(string type)
		{
			return rmSystemEvents.GetString(type);
		}

		public static string GetSystemEventName(string key, string locale)
		{
			CultureInfo culture = new CultureInfo(locale);
			string ret = rmSystemEvents.GetString(key, culture);
			if(ret == null || ret.Length == 0)
				ret = string.Format("{0}|{1}", culture.Name, key);
			return ret;
		}
		#endregion

		#region GetListSystemEventsByObject
		/// <summary>
		///  SystemEventId, EventTypeId, ObjectId, ObjectTitle, RelObjectTitle, UserId, Dt, 
		///  ObjectTypeId, SystemEventTitle, RelObjectTypeId, RelObjectId 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListSystemEventsByObject(int ObjectId, int ObjectTypeId)
		{
			return DBSystemEvents.GetListSystemEventsByObject(ObjectId, ObjectTypeId, Security.CurrentUser.TimeZoneId);
		}

		public static DataTable GetListSystemEventsByObjectDT(int ObjectId, int ObjectTypeId)
		{
			return DBSystemEvents.GetListSystemEventsByObjectDT(ObjectId, ObjectTypeId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region UnsubscribePersonal
		public static void UnsubscribePersonal(int EventTypeId)
		{
			DBSystemEvents.UnsubscribePersonal(EventTypeId, Security.CurrentUser.UserID);
		}
		#endregion

		#region UnsubscribePersonalForObject
		public static void UnsubscribePersonalForObject(int EventTypeId, int ObjectId)
		{
			DBSystemEvents.UnsubscribePersonalForObject(EventTypeId, Security.CurrentUser.UserID, ObjectId);
		}
		#endregion

		#region GetSystemEventType
		/// <summary>
		///  EventTypeId, ParentId, ObjectTypeId, RelObjectTypeId, Title, IsActive
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetSystemEventType(int EventTypeId)
		{
			return DBSystemEvents.GetSystemEventType(EventTypeId);
		}
		#endregion

		#region GetSystemEventObjectTypes()
		internal static void GetSystemEventObjectTypes(SystemEventTypes eventType, out ObjectTypes objectType, out ObjectTypes relObjectType)
		{
			objectType = (ObjectTypes)DBSystemEvents.GetObjectType((int)eventType);
			relObjectType = (ObjectTypes)DBSystemEvents.GetRelObjectType((int)eventType);
		}
		#endregion

		#region CheckSystemEvents
		public static bool CheckSystemEvents(DateTime startDate, DateTime endDate)
		{
			startDate = Security.CurrentUser.CurrentTimeZone.ToUniversalTime(startDate);
			endDate = Security.CurrentUser.CurrentTimeZone.ToUniversalTime(endDate);
			return DBSystemEvents.CheckSystemEvents(Security.CurrentUser.UserID, startDate, endDate);
		}
		#endregion


		private static bool CheckFileStorageRight(FileInfo fileInfo, string action, int userId)
		{
			bool isActionAllowed = false;

			if (fileInfo.ContainerKey.StartsWith("ForumNodeId_"))
			{
				// Extract forumNodeId
				int forumNodeId = int.Parse(fileInfo.ContainerKey.Split('_')[1]);

				// Find incidentId by ForumNodeId
				string forumContainerKey = ForumThreadNodeInfo.GetOwnerContainerKey(forumNodeId);
				int incidentId = int.Parse(forumContainerKey.Split('_')[1]);

				// Check Security
				switch (action)
				{
					case "Read":
						isActionAllowed = Incident.CanRead(incidentId);
						break;
					case "Write":
						isActionAllowed = Incident.CanUpdate(incidentId);
						break;
				}
			}
			else if (fileInfo.ContainerKey.StartsWith("DocumentVers_"))
			{
				// Extract documentVersionId
				int documentId = int.Parse(fileInfo.ContainerKey.Split('_')[1]);

				// Check Security
				switch (action)
				{
					case "Read":
						isActionAllowed = Document.CanRead(documentId);
						break;
					case "Write":
						isActionAllowed = Document.CanAddVersion(documentId);
						break;
				}
			}
			else
			{
				isActionAllowed = FileStorage.CanUserRunAction(userId, fileInfo.ContainerKey, fileInfo.ParentDirectoryId, action);
			}

			return isActionAllowed;
		}
	}
	#endregion
}
