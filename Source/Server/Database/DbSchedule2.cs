using System;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database
{
	public class DbSchedule2
	{
		#region == Old Methods ==
		/*

		#region AddUserToDateType()
		public void AddUserToDateType(int userId, int dateTypeId, int lag)
		{
			DbHelper2.RunSp("Schedule2AddUserToDateType",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@DateTypeId", SqlDbType.Int, dateTypeId),
				DbHelper2.mp("@Lag", SqlDbType.Int, lag));
		}
		#endregion

		#region AddUserToObjectDate()
		public void AddUserToObjectDate(int userId, int dateTypeId, int lag, int objectId, DateTime dt)
		{
			DbHelper2.RunSp("Schedule2AddUserToObjectDate",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@DateTypeId", SqlDbType.Int, dateTypeId),
				DbHelper2.mp("@Lag", SqlDbType.Int, lag),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@Dt", SqlDbType.DateTime, dt));
		}
		#endregion

		#region AddObjectDate()
		public void AddObjectDate(int dateTypeId, int objectId, DateTime dt)
		{
			DbHelper2.RunSp("Schedule2AddObjectDate",
				DbHelper2.mp("@DateTypeId", SqlDbType.Int, dateTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@Dt", SqlDbType.DateTime, dt));
		}
		#endregion

		#region DeleteObject()
		public void DeleteObject(int objectTypeId, int objectId)
		{
			DbHelper2.RunSp("Schedule2DeleteObject",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, objectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId));
		}
		#endregion

		#region DeleteUserFromDateType()
		public void DeleteUserFromDateType(int userId, int dateTypeId)
		{
			DbHelper2.RunSp("Schedule2DeleteUserFromDateType",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@DateTypeId", SqlDbType.Int, dateTypeId));
		}
		#endregion

		#region DeleteUserFromObjectDate()
		public void DeleteUserFromObjectDate(int userId, int dateTypeId, int objectId)
		{
			DbHelper2.RunSp("Schedule2DeleteUserFromObjectDate",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@DateTypeId", SqlDbType.Int, dateTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId));
		}
		#endregion

		#region Get()
		public IDataReader Get(DateTime dt)
		{
			return DbHelper2.RunSpDataReader("Schedule2Get",
				DbHelper2.mp("@Dt", SqlDbType.DateTime, dt));
		}
		#endregion
		*/
		#endregion

		#region GetDateTypesByObjectType
		/// <summary>
		/// Reader returns fields:
		///  DateTypeId, ObjectTypeId, DateTypeName 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetDateTypesByObjectType(int ObjectTypeId)
		{
			return DbHelper2.RunSpDataReader("DateTypesGetByObjectType",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId));
		}
		#endregion


		#region AddHook
		public static int AddHook(int DateTypeId, int ObjectId, int Lag, int HandlerId, string Params)
		{
			if (ObjectId > 0)
				return DbHelper2.RunSpInteger("DateTypeHookAdd",
					DbHelper2.mp("@DateTypeId", SqlDbType.Int, DateTypeId),
					DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
					DbHelper2.mp("@Lag", SqlDbType.Int, Lag),
					DbHelper2.mp("@HandlerId", SqlDbType.Int, HandlerId),
					DbHelper2.mp("@Params", SqlDbType.NVarChar, 1024, Params));
			else
				return DbHelper2.RunSpInteger("DateTypeHookAdd",
					DbHelper2.mp("@DateTypeId", SqlDbType.Int, DateTypeId),
					DbHelper2.mp("@Lag", SqlDbType.Int, Lag),
					DbHelper2.mp("@HandlerId", SqlDbType.Int, HandlerId),
					DbHelper2.mp("@Params", SqlDbType.NVarChar, 1024, Params));
		}
		#endregion

		#region UpdateHook
		public static void UpdateHook(int HookId, int Lag)
		{
			DbHelper2.RunSp("DateTypeHookUpdate",
				DbHelper2.mp("@HookId", SqlDbType.Int, HookId),
				DbHelper2.mp("@Lag", SqlDbType.Int, Lag));
		}
		#endregion

		#region DeleteHook
		public static void DeleteHook(int HookId)
		{
			DbHelper2.RunSp("DateTypeHookDelete",
				DbHelper2.mp("@HookId", SqlDbType.Int, HookId));
		}
		#endregion

		#region GetHook
		/// <summary>
		/// Reader returns fields:
		///  HookId, DateTypeId, ObjectId, ObjectUid, Lag, HandlerId, Params, ObjectTypeId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetHook(int HookId)
		{
			return DbHelper2.RunSpDataReader("DateTypeHookGet",
				DbHelper2.mp("@HookId", SqlDbType.Int, HookId));
		}
		#endregion


		#region AddDateTypeValue
		public static int AddDateTypeValue(int DateTypeId, int ObjectId, DateTime DateValue)
		{
			return DbHelper2.RunSpInteger("DateTypeValueAdd",
				DbHelper2.mp("@DateTypeId", SqlDbType.Int, DateTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@DateValue", SqlDbType.DateTime, DateValue));
		}
		#endregion
		#region UpdateDateTypeValue()
		public static int UpdateDateTypeValue(int dateTypeId, int? objectId, Guid? objectUid, DateTime dateValue)
		{
			return DbHelper2.RunSpInteger("DateTypeValueUpdate",
				DbHelper2.mp("@DateTypeId", SqlDbType.Int, dateTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@ObjectUid", SqlDbType.UniqueIdentifier, objectUid),
				DbHelper2.mp("@DateValue", SqlDbType.DateTime, dateValue));
		}
		#endregion

		#region AddNewDateTypeValue
		public static void AddNewDateTypeValue(int DateTypeId, int ObjectId, DateTime DateValue)
		{
			DbHelper2.RunSp("DateTypeValueAddNew",
				DbHelper2.mp("@DateTypeId", SqlDbType.Int, DateTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@DateValue", SqlDbType.DateTime, DateValue));
		}
		#endregion

		#region DeleteDateTypeValue
		public static void DeleteDateTypeValue(int dateTypeId, int? objectId, Guid? objectUid)
		{
			DbHelper2.RunSp("DateTypeValueDelete",
				DbHelper2.mp("@DateTypeId", SqlDbType.Int, dateTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@objectUid", SqlDbType.UniqueIdentifier, objectUid));
		}
		#endregion

		#region AddReminderSubscriptionGlobal
		public static int AddReminderSubscriptionGlobal(int DateTypeId, int Lag, bool IsActive)
		{
			return DbHelper2.RunSpInteger("ReminderSubscribtionAddGlobal",
				DbHelper2.mp("@DateTypeId", SqlDbType.Int, DateTypeId),
				DbHelper2.mp("@Lag", SqlDbType.Int, Lag),
				DbHelper2.mp("@IsActive", SqlDbType.Bit, IsActive));
		}
		#endregion

		#region AddReminderSubscriptionPersonal
		public static int AddReminderSubscriptionPersonal(int DateTypeId, int UserId, int Lag, bool IsActive)
		{
			return DbHelper2.RunSpInteger("ReminderSubscribtionAddPersonal",
				DbHelper2.mp("@DateTypeId", SqlDbType.Int, DateTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@Lag", SqlDbType.Int, Lag),
				DbHelper2.mp("@IsActive", SqlDbType.Bit, IsActive));
		}
		#endregion

		#region AddReminderSubscriptionPersonalForObject
		public static int AddReminderSubscriptionPersonalForObject(int DateTypeId, int UserId, 
			int ObjectId, int Lag, bool IsActive)
		{
			return DbHelper2.RunSpInteger("ReminderSubscribtionAddPersonalForObject",
				DbHelper2.mp("@DateTypeId", SqlDbType.Int, DateTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@Lag", SqlDbType.Int, Lag),
				DbHelper2.mp("@IsActive", SqlDbType.Bit, IsActive));
		}
		#endregion


		#region UpdateReminderSubscriptionHookId
		public static void UpdateReminderSubscriptionHookId(int SubscriptionId, int HookId)
		{
			if (HookId > 0)
				DbHelper2.RunSp("ReminderSubscribtionUpdateHookId",
					DbHelper2.mp("@SubscriptionId", SqlDbType.Int, SubscriptionId),
					DbHelper2.mp("@HookId", SqlDbType.Int, HookId));
			else
				DbHelper2.RunSp("ReminderSubscribtionUpdateHookId",
					DbHelper2.mp("@SubscriptionId", SqlDbType.Int, SubscriptionId));
		}
		#endregion


		#region GetReminderSubscriptionGlobal
		/// <summary>
		/// Reader returns fields:
		///  SubscriptionId, DateTypeId, UserId, ObjectId, ObjectUid, Lag, IsActive, HookId 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetReminderSubscriptionGlobal(int DateTypeId)
		{
			return DbHelper2.RunSpDataReader("ReminderSubscribtionGetGlobal",
				DbHelper2.mp("@DateTypeId", SqlDbType.Int, DateTypeId));
		}
		#endregion

		#region GetReminderSubscriptionPersonal
		/// <summary>
		/// Reader returns fields:
		///  SubscriptionId, DateTypeId, UserId, ObjectId, ObjectUid, Lag, IsActive, HookId, SubscriptionType 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetReminderSubscriptionPersonal(int DateTypeId, int UserId)
		{
			return DbHelper2.RunSpDataReader("ReminderSubscribtionGetPersonal",
				DbHelper2.mp("@DateTypeId", SqlDbType.Int, DateTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetReminderSubscriptionPersonalForObject
		/// <summary>
		/// Reader returns fields:
		///  SubscriptionId, DateTypeId, UserId, ObjectId, Lag, IsActive, HookId, SubscriptionType 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetReminderSubscriptionPersonalForObject(int DateTypeId, int UserId, int ObjectId)
		{
			return DbHelper2.RunSpDataReader("ReminderSubscribtionGetPersonalForObject",
				DbHelper2.mp("@DateTypeId", SqlDbType.Int, DateTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion


		#region DeleteReminderSubscriptionPersonal
		public static void DeleteReminderSubscriptionPersonal(int DateTypeId, int UserId)
		{
			DbHelper2.RunSp("ReminderSubscribtionDeletePersonal",
				DbHelper2.mp("@DateTypeId", SqlDbType.Int, DateTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region DeleteReminderSubscriptionPersonalForObject
		public static void DeleteReminderSubscriptionPersonalForObject(int DateTypeId, int UserId, int ObjectId)
		{
			DbHelper2.RunSp("ReminderSubscribtionDeletePersonalForObject",
				DbHelper2.mp("@DateTypeId", SqlDbType.Int, DateTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion


		#region GetReminderSubscriptionByHookId
		/// <summary>
		/// Reader returns fields:
		///  SubscriptionId, DateTypeId, UserId, ObjectId, ObjectUid, Lag, IsActive, HookId, ObjectTypeId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetReminderSubscriptionByHookId(int HookId)
		{
			return DbHelper2.RunSpDataReader("ReminderSubscribtionGetByHookId",
				DbHelper2.mp("@HookId", SqlDbType.Int, HookId));
		}
		#endregion

		#region FillScheduleProcessHandlerTempItems 
		/// <summary>
		/// Fills the schedule process handler temp items.
		/// </summary>
		/// <returns>Reader returns fields: ValueId, DateTypeName, ObjectTypeId, ObjectId, ObjectUid,
		/// Dt, HookId, HandlerId, Params, DateValue, Lag</returns>
		public static IDataReader FillScheduleProcessHandlerTempItems()
		{
			return DbHelper2.RunSpDataReader("ScheduleProcessHandlerTempItemsFill");
		}
		#endregion

		#region ProcessScheduleProcessHandlerTempItem 
		/// <summary>
		/// Processes the schedule process handler temp item.
		/// </summary>
		/// <param name="Id">The id.</param>
		public static void ProcessScheduleProcessHandlerTempItem(int Id)
		{
			DbHelper2.RunSpDataReader("ScheduleProcessHandlerTempItemProcess",
				DbHelper2.mp("@id", SqlDbType.Int, Id));
		}
		#endregion

		#region AddProcessedDateType
		public static void AddProcessedDateType(int ValueId, int HookId, DateTime DateValue, int Lag)
		{
			DbHelper2.RunSp("DateTypeProcessedAdd",
				DbHelper2.mp("@ValueId", SqlDbType.Int, ValueId),
				DbHelper2.mp("@HookId", SqlDbType.Int, HookId),
				DbHelper2.mp("@DateValue", SqlDbType.DateTime, DateValue),
				DbHelper2.mp("@Lag", SqlDbType.Int, Lag));
		}
		#endregion
	}
}
