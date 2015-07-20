using System;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBSystemEvents.
	/// </summary>
	public class DBSystemEvents
	{
		#region AddSystemEvents
		public static int AddSystemEvents(int EventTypeId, int? ObjectId, Guid? ObjectUid, string ObjectTitle,
			int RelObjectId, string RelObjectTitle, int UserId, string XMLData,	string TransactionId)
		{
			object xml = XMLData == null ? DBNull.Value : (object)XMLData;
			object oUserId = UserId <= 0 ? DBNull.Value : (object)UserId;
			return DbHelper2.RunSpInteger("SystemEventsAdd", 
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@ObjectUid", SqlDbType.UniqueIdentifier, ObjectUid),
				DbHelper2.mp("@ObjectTitle", SqlDbType.NVarChar, 250, ObjectTitle),
				DbHelper2.mp("@RelObjectId", SqlDbType.Int, RelObjectId),
				DbHelper2.mp("@RelObjectTitle", SqlDbType.NVarChar, 250, RelObjectTitle),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@XMLData", SqlDbType.NText, xml, false),
				DbHelper2.mp("@TranId", SqlDbType.Char, 36, TransactionId));
		}
		#endregion

		#region GetSystemEvents
		/// <summary>
		///  SystemEventId, EventTypeId, ObjectId, ObjectUid, ObjectTitle, RelObjectId, RelObjectTitle, UserId, 
		///  Dt, ObjectTypeId, RelObjectTypeId, SystemEventTitle, ObjectCode
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetSystemEvents(int UserId, DateTime StartDate, 
			DateTime EndDate, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Dt"},
				"SystemEventsGet",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@EndDate", SqlDbType.DateTime, EndDate));
		}

		public static DataTable GetSystemEventsDT(int UserId, DateTime StartDate, 
			DateTime EndDate, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"Dt"},
				"SystemEventsGet",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@EndDate", SqlDbType.DateTime, EndDate));
		}
		#endregion

		#region GetSubscribersForToDo
		/// <summary>
		///  UserId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetSubscribersForToDo(int EventTypeId, int ObjectId,
			bool GetManagers, bool GetResources, bool GetAll)
		{
			return DbHelper2.RunSpDataReader("SubscribersGetForToDo",
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@GetManagers", SqlDbType.Bit, GetManagers),
				DbHelper2.mp("@GetResources", SqlDbType.Bit, GetResources),
				DbHelper2.mp("@GetAll", SqlDbType.Bit, GetAll));
		}
		#endregion

		#region GetSubscribersForTask
		/// <summary>
		///  UserId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetSubscribersForTask(int EventTypeId, int ObjectId,
			bool GetManagers, bool GetResources, bool GetAll)
		{
			return DbHelper2.RunSpDataReader("SubscribersGetForTask",
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@GetManagers", SqlDbType.Bit, GetManagers),
				DbHelper2.mp("@GetResources", SqlDbType.Bit, GetResources),
				DbHelper2.mp("@GetAll", SqlDbType.Bit, GetAll));
		}
		#endregion

		#region GetSubscribersForDocument
		/// <summary>
		///  UserId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetSubscribersForDocument(int EventTypeId, int ObjectId,
			bool GetManagers, bool GetResources, bool GetAll)
		{
			return DbHelper2.RunSpDataReader("SubscribersGetForDocument",
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@GetManagers", SqlDbType.Bit, GetManagers),
				DbHelper2.mp("@GetResources", SqlDbType.Bit, GetResources),
				DbHelper2.mp("@GetAll", SqlDbType.Bit, GetAll));
		}
		#endregion

		#region GetSubscribersForIssue
		/// <summary>
		///  UserId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetSubscribersForIssue(
			int EventTypeId
			, int ObjectId
			, bool GetCreators
			, bool GetManagers
			, bool GetResources
			, bool GetResponsible
			, bool GetControllers
			, bool GetAll
			)
		{
			return DbHelper2.RunSpDataReader("SubscribersGetForIssue",
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@GetCreators", SqlDbType.Bit, GetCreators),
				DbHelper2.mp("@GetManagers", SqlDbType.Bit, GetManagers),
				DbHelper2.mp("@GetResources", SqlDbType.Bit, GetResources),
				DbHelper2.mp("@GetResponsible", SqlDbType.Bit, GetResponsible),
				DbHelper2.mp("@GetControllers", SqlDbType.Bit, GetControllers),
				DbHelper2.mp("@GetAll", SqlDbType.Bit, GetAll));
		}
		#endregion

		#region GetSubscribersForList
		/// <summary>
		///  UserId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetSubscribersForList(int EventTypeId, int ObjectId)
		{
			return DbHelper2.RunSpDataReader("SubscribersGetForList",
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion

		#region GetSubscribersForProject
		/// <summary>
		///  UserId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetSubscribersForProject(int EventTypeId, int ObjectId,
			bool GetManagers, bool GetExecutiveManagers, bool GetTeamMembers, bool GetSponsors, 
			bool GetStakeholders, bool GetAll)
		{
			return DbHelper2.RunSpDataReader("SubscribersGetForProject",
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@GetManagers", SqlDbType.Bit, GetManagers),
				DbHelper2.mp("@GetExecutiveManagers", SqlDbType.Bit, GetExecutiveManagers),
				DbHelper2.mp("@GetTeamMembers", SqlDbType.Bit, GetTeamMembers),
				DbHelper2.mp("@GetSponsors", SqlDbType.Bit, GetSponsors),
				DbHelper2.mp("@GetStakeholders", SqlDbType.Bit, GetStakeholders),
				DbHelper2.mp("@GetAll", SqlDbType.Bit, GetAll));
		}
		#endregion

		#region GetSubscribersForCalendarEntry
		/// <summary>
		///  UserId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetSubscribersForCalendarEntry(int EventTypeId, int ObjectId,
			bool GetManagers, bool GetResources, bool GetAll)
		{
			return DbHelper2.RunSpDataReader("SubscribersGetForCalendarEntry",
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@GetManagers", SqlDbType.Bit, GetManagers),
				DbHelper2.mp("@GetResources", SqlDbType.Bit, GetResources),
				DbHelper2.mp("@GetAll", SqlDbType.Bit, GetAll));
		}
		#endregion

		#region GetSubscribersForPop3MailRequests
		/// <summary>
		///  UserId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetSubscribersForPop3MailRequests(int EventTypeId, int ObjectId)
		{
			return DbHelper2.RunSpDataReader("SubscribersGetForPop3MailRequests",
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion

		#region GetSubscribersForUser
		/// <summary>
		///  UserId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetSubscribersForUser(int EventTypeId, int ObjectId)
		{
			return DbHelper2.RunSpDataReader("SubscribersGetForUser",
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion

		#region GetSubscribersForAssignment
		/// <summary>
		///  UserId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetSubscribersForAssignment(int EventTypeId, Guid ObjectUid,
			bool GetManagers, bool GetParticipants, bool GetAll)
		{
			return DbHelper2.RunSpDataReader("SubscribersGetForAssignment",
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@ObjectUid", SqlDbType.UniqueIdentifier, ObjectUid),
				DbHelper2.mp("@GetManagers", SqlDbType.Bit, GetManagers),
				DbHelper2.mp("@GetParticipants", SqlDbType.Bit, GetParticipants),
				DbHelper2.mp("@GetAll", SqlDbType.Bit, GetAll));
		}
		#endregion


		#region CheckToDoSubscribtion
		public static bool CheckToDoSubscribtion(int EventTypeId, int ObjectId, int UserId, 
			bool CheckManagerSubscription, bool CheckResourcesSubscription, bool CheckGlobalSubscription)
		{
			return DbHelper2.RunSpInteger("SubscribtionCheckForToDo", 
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@CheckManagerSubscription", SqlDbType.Bit, CheckManagerSubscription),
				DbHelper2.mp("@CheckResourcesSubscription", SqlDbType.Bit, CheckResourcesSubscription),
				DbHelper2.mp("@CheckGlobalSubscription", SqlDbType.Bit, CheckGlobalSubscription)) == 1;
		}
		#endregion

		#region AddSystemEventRecipient
		public static void AddSystemEventRecipient(
			int SystemEventId
			, int UserId
			, int MessageTypeId
			, string emailTo
			, string emailFrom
			)
		{
			DbHelper2.RunSp("SystemEventRecipientAdd"
				, DbHelper2.mp("@SystemEventId", SqlDbType.Int, SystemEventId)
				, DbHelper2.mp("@UserId", SqlDbType.Int, UserId)
				, DbHelper2.mp("@MessageTypeId", SqlDbType.Int, MessageTypeId)
				, DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, emailTo)
				, DbHelper2.mp("@EmailFrom", SqlDbType.NVarChar, 250, emailFrom)
				);
		}
		#endregion

		#region GetObjectType
		public static int GetObjectType(int EventTypeId)
		{
			return DbHelper2.RunSpInteger("SystemEventTypeGetObjectType", 
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId));
		}
		#endregion
		#region GetRelObjectType
		public static int GetRelObjectType(int EventTypeId)
		{
			return DbHelper2.RunSpInteger("SystemEventTypeGetRelObjectType", 
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId));
		}
		#endregion

		#region RecipientsGetBatch()
		public static IDataReader RecipientsGetBatch(int userId, DateTime dt)
		{
			return DbHelper2.RunSpDataReader("SystemEventRecipientsGetForBatch",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@Dt", SqlDbType.DateTime, dt));
		}
		#endregion
		#region RecipientsGetTran()
		public static IDataReader RecipientsGetTran(string tranId)
		{
			return DbHelper2.RunSpDataReader("SystemEventRecipientsGetByTran",
				DbHelper2.mp("@TranId", SqlDbType.Char, 36, tranId));
		}
		#endregion

		#region GetEvent
		/// <summary>
		///  SystemEventId, EventTypeId, ObjectId, ObjectUid, ObjectTitle, RelObjectId, RelObjectTitle,
		///  Dt, UserId, XmlData, SystemEventTitle, ObjectTypeId, RelObjectTypeId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetEvent(int SystemEventId)
		{
			return DbHelper2.RunSpDataReader("SystemEventGet",
				DbHelper2.mp("@SystemEventId", SqlDbType.Int, SystemEventId));
		}
		#endregion

		#region GetGlobalSubscriptions
		/// <summary>
		///  EventTypeId, ObjectTypeId, Title, Role_*
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetGlobalSubscriptions(int ObjectTypeId)
		{
			return DbHelper2.RunSpDataReader("SubscriptionsGetGlobal",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId));
		}

		public static DataTable GetGlobalSubscriptionsDT(int ObjectTypeId)
		{
			return DbHelper2.RunSpDataTable("SubscriptionsGetGlobal",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId));
		}
		#endregion

		#region GetGlobalSubscriptionsAsList
		/// <summary>
		///  EventTypeId, ObjectRoleId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetGlobalSubscriptionsAsList(int ObjectTypeId)
		{
			return DbHelper2.RunSpDataReader("SubscriptionsGetGlobalAsList",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId));
		}
		#endregion

		#region AddGlobalSubscription
		public static void AddGlobalSubscription(int EventTypeId, int ObjectRoleId)
		{
			DbHelper2.RunSp("SubscribtionAddGlobal", 
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@ObjectRoleId", SqlDbType.Int, ObjectRoleId));
		}
		#endregion

		#region DeleteGlobalSubscription
		public static void DeleteGlobalSubscription(int EventTypeId, int ObjectRoleId)
		{
			DbHelper2.RunSp("SubscribtionDeleteGlobal", 
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@ObjectRoleId", SqlDbType.Int, ObjectRoleId));
		}
		#endregion

		#region GetPersonalSubscriptions
		/// <summary>
		///  EventTypeId, ObjectTypeId, Title, Role_*, IsDefault
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetPersonalSubscriptions(int ObjectTypeId, int UserId)
		{
			return DbHelper2.RunSpDataReader("SubscriptionsGetPersonal",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}

		public static DataTable GetPersonalSubscriptionsDT(int ObjectTypeId, int UserId)
		{
			return DbHelper2.RunSpDataTable("SubscriptionsGetPersonal",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetPersonalSubscriptionsForObject
		/// <summary>
		///  EventTypeId, ObjectTypeId, Title, Role_*, IsDefault
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetPersonalSubscriptionsForObject(int ObjectTypeId, int UserId, int ObjectId)
		{
			return DbHelper2.RunSpDataReader("SubscriptionsGetPersonalForObject",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}

		public static DataTable GetPersonalSubscriptionsForObjectDT(int ObjectTypeId, int UserId, int ObjectId)
		{
			return DbHelper2.RunSpDataTable("SubscriptionsGetPersonalForObject",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion

		#region AddPersonalSubscription
		public static int AddPersonalSubscription(int EventTypeId, int UserId)
		{
			return DbHelper2.RunSpInteger("SubscribtionAddPersonal", 
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region AddPersonalSubscriptionForObject
		public static int AddPersonalSubscriptionForObject(int EventTypeId, int UserId, int ObjectId)
		{
			return DbHelper2.RunSpInteger("SubscribtionAddPersonalForObject", 
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion

		#region DeletePersonalSubscription
		public static void DeletePersonalSubscription(int EventTypeId, int UserId)
		{
			DbHelper2.RunSp("SubscribtionDeletePersonal", 
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region DeletePersonalSubscriptionForObject
		public static void DeletePersonalSubscriptionForObject(int EventTypeId, int UserId, int ObjectId)
		{
			DbHelper2.RunSp("SubscribtionDeletePersonalForObject", 
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion

		#region AddSubscriptionDetails
		public static void AddSubscriptionDetails(int SubscriptionId, int ObjectRoleId)
		{
			DbHelper2.RunSp("SubscribtionDetailsAdd", 
				DbHelper2.mp("@SubscriptionId", SqlDbType.Int, SubscriptionId),
				DbHelper2.mp("@ObjectRoleId", SqlDbType.Int, ObjectRoleId));
		}
		#endregion

		#region DeleteSubscriptionDetails
		public static void DeleteSubscriptionDetails(int SubscriptionId, int ObjectRoleId)
		{
			DbHelper2.RunSp("SubscribtionDetailsDelete", 
				DbHelper2.mp("@SubscriptionId", SqlDbType.Int, SubscriptionId),
				DbHelper2.mp("@ObjectRoleId", SqlDbType.Int, ObjectRoleId));
		}
		#endregion

		#region GetPersonalSubscriptionDetails
		/// <summary>
		///  ObjectRoleId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetPersonalSubscriptionDetails(int EventTypeId, int UserId)
		{
			return DbHelper2.RunSpDataReader("SubscriptionDetailsGetPersonal",
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetPersonalSubscriptionDetailsForObject
		/// <summary>
		///  ObjectRoleId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetPersonalSubscriptionDetailsForObject(int EventTypeId, int UserId, int ObjectId)
		{
			return DbHelper2.RunSpDataReader("SubscriptionDetailsGetPersonalForObject",
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion

		#region RecipientUpdateSend()
		public static void RecipientUpdateSend(int recipientId, bool sendEmail, bool sendIbn)
		{
			RecipientUpdateSend(recipientId, sendEmail, sendIbn, null);
		}
		public static void RecipientUpdateSend(int recipientId, bool sendEmail, bool sendIbn, object logId)
		{
			DbHelper2.RunSp("SystemEventRecipientUpdateSend",
				DbHelper2.mp("@RecipientId", SqlDbType.Int, recipientId),
				DbHelper2.mp("@SendEmail", SqlDbType.Bit, sendEmail),
				DbHelper2.mp("@SendIbn", SqlDbType.Bit, sendIbn),
				DbHelper2.mp("@LogId", SqlDbType.Int, logId));
		}
		#endregion
		#region RecipientUpdateSent()
		public static void RecipientUpdateSent(int recipientId, int deliveryTypeId, bool sent)
		{
			DbHelper2.RunSp("SystemEventRecipientUpdateSent",
				DbHelper2.mp("@RecipientId", SqlDbType.Int, recipientId),
				DbHelper2.mp("@DeliveryTypeId", SqlDbType.Int, deliveryTypeId),
				DbHelper2.mp("@Sent", SqlDbType.Bit, sent));
		}
		#endregion

		#region RecipientUpdateLogId()
		public static void RecipientUpdateLogId(int recipientId, int logId)
		{
			DbHelper2.RunSp("SystemEventRecipientUpdateLogId",
				DbHelper2.mp("@RecipientId", SqlDbType.Int, recipientId),
				DbHelper2.mp("@LogId", SqlDbType.Int, logId));
		}
		#endregion

		#region GetListSystemEventsByObject
		/// <summary>
		///  SystemEventId, EventTypeId, ObjectId, ObjectTitle, RelObjectTitle, UserId, Dt, 
		///  ObjectTypeId, SystemEventTitle, RelObjectTypeId, RelObjectId 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListSystemEventsByObject(int ObjectId, int ObjectTypeId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Dt"},
				"SystemEventsGetByObject",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId));
		}

		public static DataTable GetListSystemEventsByObjectDT(int ObjectId, int ObjectTypeId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"Dt"},
				"SystemEventsGetByObject",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId));
		}
		#endregion

		#region UnsubscribePersonal
		public static void UnsubscribePersonal(int EventTypeId, int UserId)
		{
			DbHelper2.RunSp("SubscribtionUnsubscribePersonal", 
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region UnsubscribePersonalForObject
		public static void UnsubscribePersonalForObject(int EventTypeId, int UserId, int ObjectId)
		{
			DbHelper2.RunSp("SubscribtionUnsubscribePersonalForObject", 
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion

		#region GetSystemEventType
		/// <summary>
		///  EventTypeId, ParentId, ObjectTypeId, RelObjectTypeId, Title, IsActive
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetSystemEventType(int EventTypeId)
		{
			return DbHelper2.RunSpDataReader("SystemEventTypeGet",
				DbHelper2.mp("@EventTypeId", SqlDbType.Int, EventTypeId));
		}
		#endregion

		#region CheckSystemEvents
		public static bool CheckSystemEvents(int UserId, DateTime StartDate, DateTime EndDate)
		{
			int retval = DbHelper2.RunSpInteger("SystemEventsCheck",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@EndDate", SqlDbType.DateTime, EndDate));

			return (retval > 0);
		}
		#endregion
	}
}
