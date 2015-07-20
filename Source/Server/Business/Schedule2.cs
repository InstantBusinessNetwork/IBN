using System;
using System.Collections;
using System.Data;
using System.Globalization;

using Mediachase.Ibn;
using Mediachase.IBN.Database;
using Mediachase.Ibn.Assignments;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using System.Collections.Generic;

namespace Mediachase.IBN.Business
{
	#region enum DateTypes
	public enum DateTypes
	{
		Undefined = 0,
		Project_StartDate = 1,
		Project_FinishDate = 2,
		CalendarEntry_StartDate = 3,
		CalendarEntry_FinishDate = 4,
		Task_StartDate = 5,
		Task_FinishDate = 6,
		Todo_StartDate = 7,
		Todo_FinishDate = 8,
		BatchAlert = 9,
		LdapSynchronization = 10,
		AssignmentFinishDate = 11,
	}
	#endregion

	#region enum DateTypeHandlers
	public enum DateTypeHandlers
	{
		SendReminder = 1,
		RaiseSystemEvent = 2,
		BatchAlert = 3,
		LdapSync = 4
	}
	#endregion

	#region enum SubscriptionTypes
	public enum SubscriptionTypes
	{
		UNDEFINED = -1,
		Global = 1,
		Personal = 2,
		PersonalForObject = 3
	}
	#endregion

	public class Schedule
	{
		#region AddDateTypeValue
		public static int AddDateTypeValue(DateTypes DateType, int ObjectId, DateTime DateValue)
		{
			return AddDateTypeValue((int)DateType, ObjectId, DateValue);
		}
		public static int AddDateTypeValue(int DateTypeId, int ObjectId, DateTime DateValue)
		{
			// OR: Данный метод используется только для Batch Alerts [4/18/2006]
			return DbSchedule2.AddDateTypeValue(DateTypeId, ObjectId, DateValue);
		}
		#endregion

		#region UpdateDateTypeValue
		public static void UpdateDateTypeValue(DateTypes dateType, int objectId, DateTime dateValue)
		{
			UpdateDateTypeValue((int)dateType, objectId, null, dateValue);
		}
		public static void UpdateDateTypeValue(DateTypes dateType, Guid objectUid, DateTime dateValue)
		{
			UpdateDateTypeValue((int)dateType, null, objectUid, dateValue);
		}
		public static void UpdateDateTypeValue(int dateTypeId, int? objectId, Guid? objectUid, DateTime dateValue)
		{
			// OR: Если дата в прошлом - она нам не нужна [4/18/2006]
			if (dateValue < DateTime.UtcNow)
				DbSchedule2.DeleteDateTypeValue(dateTypeId, objectId, objectUid);
			else
				DbSchedule2.UpdateDateTypeValue(dateTypeId, objectId, objectUid, dateValue);
		}
		#endregion

		#region DeleteDateTypeValue
		public static void DeleteDateTypeValue(DateTypes dateType, int objectId)
		{
			DeleteDateTypeValue((int)dateType, objectId, null);
		}
		public static void DeleteDateTypeValue(DateTypes dateType, Guid objectUid)
		{
			DeleteDateTypeValue((int)dateType, null, objectUid);
		}
		public static void DeleteDateTypeValue(int dateTypeId, int? objectId, Guid? objectUid)
		{
			DbSchedule2.DeleteDateTypeValue(dateTypeId, objectId, objectUid);
		}
		#endregion

		#region GetReminderSubscriptionGlobal
		/// <summary>
		/// Reader returns fields:
		///  SubscriptionId, DateTypeId, UserId, ObjectId, ObjectUid, Lag, IsActive, HookId 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetReminderSubscriptionGlobal(DateTypes DateType)
		{
			return DbSchedule2.GetReminderSubscriptionGlobal((int)DateType);
		}
		#endregion

		#region GetReminderSubscriptionPersonal
		/// <summary>
		/// Reader returns fields:
		///  SubscriptionId, DateTypeId, UserId, ObjectId, ObjectUid, Lag, IsActive, HookId, SubscriptionType 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetReminderSubscriptionPersonal(DateTypes DateType)
		{
			return DbSchedule2.GetReminderSubscriptionPersonal((int)DateType, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetReminderSubscriptionPersonalForObject
		/// <summary>
		/// Reader returns fields:
		///  SubscriptionId, DateTypeId, UserId, ObjectId, Lag, IsActive, HookId, SubscriptionType 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetReminderSubscriptionPersonalForObject(DateTypes DateType, int ObjectId)
		{
			return DbSchedule2.GetReminderSubscriptionPersonalForObject((int)DateType, Security.CurrentUser.UserID, ObjectId);
		}
		#endregion


		#region UpdateReminderSubscriptionGlobal
		public static void UpdateReminderSubscriptionGlobal(
			int ProjectStartDateLag, int ProjectFinishDateLag,
			int CalendarEntryStartDateLag, int CalendarEntryFinishDateLag,
			int TaskStartDateLag, int TaskFinishDateLag,
			int TodoStartDateLag, int TodoFinishDateLag,
			int assignmentFinishDateLag)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				UpdateReminderSubscriptionGlobal(DateTypes.Project_StartDate, ProjectStartDateLag >= 0 ? ProjectStartDateLag : 0, ProjectStartDateLag >= 0);
				UpdateReminderSubscriptionGlobal(DateTypes.Project_FinishDate, ProjectFinishDateLag >= 0 ? ProjectFinishDateLag : 0, ProjectFinishDateLag >= 0);

				UpdateReminderSubscriptionGlobal(DateTypes.CalendarEntry_StartDate, CalendarEntryStartDateLag >= 0 ? CalendarEntryStartDateLag : 0, CalendarEntryStartDateLag >= 0);
				UpdateReminderSubscriptionGlobal(DateTypes.CalendarEntry_FinishDate, CalendarEntryFinishDateLag >= 0 ? CalendarEntryFinishDateLag : 0, CalendarEntryFinishDateLag >= 0);

				UpdateReminderSubscriptionGlobal(DateTypes.Task_StartDate, TaskStartDateLag >= 0 ? TaskStartDateLag : 0, TaskStartDateLag >= 0);
				UpdateReminderSubscriptionGlobal(DateTypes.Task_FinishDate, TaskFinishDateLag >= 0 ? TaskFinishDateLag : 0, TaskFinishDateLag >= 0);

				UpdateReminderSubscriptionGlobal(DateTypes.Todo_StartDate, TodoStartDateLag >= 0 ? TodoStartDateLag : 0, TodoStartDateLag >= 0);
				UpdateReminderSubscriptionGlobal(DateTypes.Todo_FinishDate, TodoFinishDateLag >= 0 ? TodoFinishDateLag : 0, TodoFinishDateLag >= 0);

				UpdateReminderSubscriptionGlobal(DateTypes.AssignmentFinishDate, assignmentFinishDateLag >= 0 ? assignmentFinishDateLag : 0, assignmentFinishDateLag >= 0);

				tran.Commit();
			}
		}

		private static void UpdateReminderSubscriptionGlobal(DateTypes dateType, int Lag, bool IsActive)
		{
			bool WasActive = false;
			int HookId = -1;
			int OldLag = -1;
			using (IDataReader reader = DbSchedule2.GetReminderSubscriptionGlobal((int)dateType))
			{
				if (reader.Read())
				{
					WasActive = (bool)reader["IsActive"];
					if (reader["HookId"] != DBNull.Value)
						HookId = (int)reader["HookId"];
					OldLag = (int)reader["Lag"];
				}
			}

			int SubscriptionId = DbSchedule2.AddReminderSubscriptionGlobal((int)dateType, Lag, IsActive);

			if (IsActive && !WasActive)	// Activation
			{
				HookId = DbSchedule2.AddHook((int)dateType, -1, Lag, (int)DateTypeHandlers.SendReminder, ((int)SubscriptionTypes.Global).ToString());
				DbSchedule2.UpdateReminderSubscriptionHookId(SubscriptionId, HookId);
			}
			else if (IsActive && WasActive && Lag != OldLag)	// Lag changed
			{
				DbSchedule2.UpdateHook(HookId, Lag);
			}
			else if (!IsActive && WasActive)	// Deactivation
			{
				DbSchedule2.UpdateReminderSubscriptionHookId(SubscriptionId, -1);
				DbSchedule2.DeleteHook(HookId);
			}
		}
		#endregion

		#region UpdateReminderSubscriptionPersonal
		public static void UpdateReminderSubscriptionPersonal(DateTypes dateType, int Lag, bool IsActive)
		{
			bool WasActive = false;
			int HookId = -1;
			int OldLag = -1;
			using (IDataReader reader = DbSchedule2.GetReminderSubscriptionPersonal((int)dateType, Security.CurrentUser.UserID))
			{
				///  SubscriptionId, DateTypeId, UserId, ObjectId, Lag, IsActive, HookId, SubscriptionType 
				if (reader.Read())
				{
					if ((int)reader["SubscriptionType"] == (int)SubscriptionTypes.Personal)
					{
						WasActive = (bool)reader["IsActive"];
						if (reader["HookId"] != DBNull.Value)
							HookId = (int)reader["HookId"];
						OldLag = (int)reader["Lag"];
					}
				}
			}

			int SubscriptionId = DbSchedule2.AddReminderSubscriptionPersonal((int)dateType, Security.CurrentUser.UserID, Lag, IsActive);

			if (IsActive && !WasActive)	// Activation
			{
				HookId = DbSchedule2.AddHook((int)dateType, -1, Lag, (int)DateTypeHandlers.SendReminder, ((int)SubscriptionTypes.Personal).ToString());
				DbSchedule2.UpdateReminderSubscriptionHookId(SubscriptionId, HookId);
			}
			else if (IsActive && WasActive && Lag != OldLag)	// Lag changed
			{
				DbSchedule2.UpdateHook(HookId, Lag);
			}
			else if (!IsActive && WasActive)	// Deactivation
			{
				DbSchedule2.UpdateReminderSubscriptionHookId(SubscriptionId, -1);
				DbSchedule2.DeleteHook(HookId);
			}
		}
		#endregion

		#region UpdateReminderSubscriptionPersonalForObject
		public static void UpdateReminderSubscriptionPersonalForObject(DateTypes dateType, int ObjectId, int Lag, bool IsActive)
		{
			bool WasActive = false;
			int HookId = -1;
			int OldLag = -1;
			using (IDataReader reader = DbSchedule2.GetReminderSubscriptionPersonalForObject((int)dateType, Security.CurrentUser.UserID, ObjectId))
			{
				///  SubscriptionId, DateTypeId, UserId, ObjectId, Lag, IsActive, HookId, SubscriptionType 
				if (reader.Read())
				{
					if ((int)reader["SubscriptionType"] == (int)SubscriptionTypes.PersonalForObject)
					{
						WasActive = (bool)reader["IsActive"];
						if (reader["HookId"] != DBNull.Value)
							HookId = (int)reader["HookId"];
						OldLag = (int)reader["Lag"];
					}
				}
			}

			int SubscriptionId = DbSchedule2.AddReminderSubscriptionPersonalForObject((int)dateType, Security.CurrentUser.UserID, ObjectId, Lag, IsActive);

			if (IsActive && !WasActive)	// Activation
			{
				HookId = DbSchedule2.AddHook((int)dateType, ObjectId, Lag, (int)DateTypeHandlers.SendReminder, ((int)SubscriptionTypes.PersonalForObject).ToString());
				DbSchedule2.UpdateReminderSubscriptionHookId(SubscriptionId, HookId);
			}
			else if (IsActive && WasActive && Lag != OldLag)	// Lag changed
			{
				DbSchedule2.UpdateHook(HookId, Lag);
			}
			else if (!IsActive && WasActive)	// Deactivation
			{
				DbSchedule2.UpdateReminderSubscriptionHookId(SubscriptionId, -1);
				DbSchedule2.DeleteHook(HookId);
			}
		}
		#endregion


		#region DeleteReminderSubscriptionPersonal
		public static void DeleteReminderSubscriptionPersonal(DateTypes DateType)
		{
			DbSchedule2.DeleteReminderSubscriptionPersonal((int)DateType, Security.CurrentUser.UserID);
		}
		#endregion

		#region DeleteReminderSubscriptionPersonalForObject
		public static void DeleteReminderSubscriptionPersonalForObject(DateTypes DateType, int ObjectId)
		{
			DbSchedule2.DeleteReminderSubscriptionPersonalForObject((int)DateType, Security.CurrentUser.UserID, ObjectId);
		}
		#endregion


		#region ProcessHandler
		public static void ProcessHandler(int handlerId, string argument, int hookId, int? objectId, Guid? objectUid, DateTime dateValue)
		{
			ObjectTypes objectType = ObjectTypes.UNDEFINED;
			DateTypes dateType = DateTypes.Undefined;

			if (handlerId == (int)DateTypeHandlers.SendReminder)
			{
				// Напоминать только о тех датах, которые еще не наступили.
				if (dateValue >= DateTime.UtcNow)
				{
					bool sendReminder = false;

					int userId = -1;
					using (IDataReader reader = DbSchedule2.GetReminderSubscriptionByHookId(hookId))
					{
						if (reader.Read())
						{
							sendReminder = true;

							if (reader["UserId"] != DBNull.Value)
							{
								userId = (int)reader["UserId"];

								// O.R. [2010-04-01]: Don't process inactive user
								if (User.GetUserActivity(userId) != User.UserActivity.Active)
									sendReminder = false;
							}
							objectType = (ObjectTypes)reader["ObjectTypeId"];
							dateType = (DateTypes)reader["DateTypeId"];
						}
					}

					// Не напоминать о досрочно запущенных или досрочно завершённых объектах [2007-02-16]
					if (sendReminder)
						sendReminder = ConfirmNotification(objectType, dateType, objectId, objectUid);

					if (sendReminder)
					{
						SubscriptionTypes subscriptionType = (SubscriptionTypes)int.Parse(argument, CultureInfo.InvariantCulture);

						switch (subscriptionType)
						{
							case SubscriptionTypes.Global:
								ProcessGlobalSubscription(hookId, dateType, objectType, objectId, objectUid);
								break;
							case SubscriptionTypes.Personal:
								ProcessPersonalSubscription(hookId, dateType, userId, objectType, objectId, objectUid);
								break;
							case SubscriptionTypes.PersonalForObject:
								ProcessPersonalSubscriptionForObject(hookId, dateType, userId, objectType, objectId, objectUid);
								break;
						}
					}
				}
			}
			else if (handlerId == (int)DateTypeHandlers.RaiseSystemEvent)
			{
				bool raiseEvent = false;

				using (IDataReader reader = DbSchedule2.GetHook(hookId))
				{
					if (reader.Read())
					{
						raiseEvent = true;

						objectType = (ObjectTypes)reader["ObjectTypeId"];
						dateType = (DateTypes)reader["DateTypeId"];
					}
				}

				if (raiseEvent)
				{
					// Не уведомлять о досрочно запущенных или досрочно завершённых объектах [2008-07-29]
					raiseEvent = ConfirmNotification(objectType, dateType, objectId, objectUid);

					if (raiseEvent)
					{
						SystemEventTypes EventType = (SystemEventTypes)int.Parse(argument);
						if (objectId.HasValue)
							SystemEvents.AddSystemEvents(EventType, objectId.Value);
						else if (objectUid.HasValue)
							SystemEvents.AddSystemEvents(EventType, objectUid.Value);
					}

					if (objectType == ObjectTypes.Task)
						Task.RecalculateAllStates(Task.GetProject(objectId.Value));
					else if (objectType == ObjectTypes.ToDo)
						ToDo.RecalculateState(objectId.Value);
					else if (objectType == ObjectTypes.CalendarEntry)
						CalendarEntry.RecalculateState(objectId.Value);
				}
			}
			else if (handlerId == (int)DateTypeHandlers.BatchAlert)
			{
				if (objectId.HasValue)
					Alerts2.SendBatch(objectId.Value, dateValue);
			}
			else if (handlerId == (int)DateTypeHandlers.LdapSync)
			{
				if (objectId.HasValue)
					Ldap.Synchronize(objectId.Value, dateValue);
			}
		}
		#endregion

		#region ProcessGlobalSubscription
		private static void ProcessGlobalSubscription(int hookId, DateTypes dateType, ObjectTypes objectType, int? objectId, Guid? objectUid)
		{
			// Формируем список пользователей, связанных с объектом
			List<int> users = new List<int>();

			if (objectType == ObjectTypes.Project)
			{
				using (IDataReader reader = DBProject.GetProjectSecurity(objectId.Value))
				{
					while (reader.Read())
						users.Add((int)reader["UserId"]);
				}
			}
			else if (objectType == ObjectTypes.Task)
			{
				using (IDataReader reader = DBTask.GetTaskSecurity(objectId.Value))
				{
					while (reader.Read())
					{
						if ((bool)reader["IsRealTaskResource"] || (bool)reader["IsRealTaskManager"] || (bool)reader["IsCreator"])
							users.Add((int)reader["UserId"]);
					}
				}
			}
			else if (objectType == ObjectTypes.ToDo)
			{
				using (IDataReader reader = DBToDo.GetToDoSecurity(objectId.Value))
				{
					while (reader.Read())
						users.Add((int)reader["UserId"]);
				}
			}
			else if (objectType == ObjectTypes.CalendarEntry)
			{
				using (IDataReader reader = DBEvent.GetListUsersForEvent(objectId.Value))
				{
					while (reader.Read())
						users.Add((int)reader["UserId"]);
				}
			}
			else if (objectType == ObjectTypes.Document)
			{
				using (IDataReader reader = DBDocument.GetDocumentSecurity(objectId.Value))
				{
					while (reader.Read())
						users.Add((int)reader["UserId"]);
				}
			}
			else if (objectType == ObjectTypes.Assignment)
			{
				AssignmentEntity entity = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, (PrimaryKeyId)objectUid.Value);
				if (entity != null && entity.OwnerDocumentId.HasValue)
				{
					using (IDataReader reader = DBDocument.GetDocumentSecurity(entity.OwnerDocumentId.Value))
					{
						while (reader.Read())
							users.Add((int)reader["UserId"]);
					}
				}
			}

			// Проверим отсутствие индивидуальной подписки
			for (int i = 0; i < users.Count; i++)
			{
				int userId = users[i];

				// O.R. [2010-04-01]: Don't process inactive users
				if (User.GetUserActivity(userId) != User.UserActivity.Active)
				{
					users.RemoveAt(i);
					i--;
				}
				else if (objectId.HasValue)
				{
					if (GetSubscriptionType(dateType, userId, objectId.Value) != SubscriptionTypes.Global)
					{
						users.RemoveAt(i);
						i--;
					}
				}
			}

			// Send Reminder
			if (users.Count > 0)
				SendReminder(dateType, objectType, objectId, objectUid, users);
		}
		#endregion

		#region ProcessPersonalSubscription
		private static void ProcessPersonalSubscription(int hookId, DateTypes dateType, int userId, ObjectTypes objectType, int? objectId, Guid? objectUid)
		{
			// Проверим отсутствие индивидуальной подписки
			if (objectId.HasValue && GetSubscriptionType(dateType, userId, objectId.Value) != SubscriptionTypes.Personal)
				return;

			// Check Security
			if (objectId.HasValue && !CheckSecurityForObject(objectType, objectId.Value, userId))
				return;
			if (objectUid.HasValue && !CheckSecurityForObject(objectType, objectUid.Value, userId))
				return;

			// Send Reminder
			List<int> users = new List<int>();
			users.Add(userId);
			SendReminder(dateType, objectType, objectId, objectUid, users);
		}
		#endregion

		#region ProcessPersonalSubscriptionForObject
		private static void ProcessPersonalSubscriptionForObject(int hookId, DateTypes dateType, int userId, ObjectTypes objectType, int? objectId, Guid? objectUid)
		{
			// Check Security
			if (objectId.HasValue && !CheckSecurityForObject(objectType, objectId.Value, userId))
				return;
			if (objectUid.HasValue && !CheckSecurityForObject(objectType, objectUid.Value, userId))
				return;

			// Send Reminder
			List<int> users = new List<int>();
			users.Add(userId);
			SendReminder(dateType, objectType, objectId, objectUid, users);
		}
		#endregion

		#region CheckSecurityForObject
		public static bool CheckSecurityForObject(ObjectTypes objectType, int ObjectId, int UserId)
		{
			bool isValid = false;
			if (objectType == ObjectTypes.Project)
			{
				Project.ProjectSecurity sec = Project.GetSecurity(ObjectId, UserId);
				isValid = sec.IsManager || sec.IsExecutiveManager || sec.IsTeamMember || sec.IsSponsor || sec.IsStakeHolder;
			}
			else if (objectType == ObjectTypes.Task)
			{
				Task.TaskSecurity sec = Task.GetSecurity(ObjectId, UserId);
				isValid = sec.IsManager || sec.IsRealTaskResource;
			}
			else if (objectType == ObjectTypes.ToDo)
			{
				ToDo.ToDoSecurity sec = ToDo.GetSecurity(ObjectId, UserId);
				isValid = sec.IsManager || sec.IsResource || sec.IsCreator;
			}
			else if (objectType == ObjectTypes.CalendarEntry)
			{
				CalendarEntry.EventSecurity sec = CalendarEntry.GetSecurity(ObjectId, UserId);
				isValid = sec.IsManager || sec.IsResource;
			}
			else if (objectType == ObjectTypes.Document)
			{
				Document.DocumentSecurity sec = Document.GetSecurity(ObjectId, UserId);
				isValid = sec.IsManager || sec.IsResource || sec.IsCreator;
			}

			return isValid;
		}

		public static bool CheckSecurityForObject(ObjectTypes objectType, Guid objectUid, int userId)
		{
			bool isValid = false;
			if (objectType == ObjectTypes.Assignment)
			{
				AssignmentEntity entity = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, (PrimaryKeyId)objectUid);
				if (entity != null && entity.OwnerDocumentId.HasValue)
				{
					Document.DocumentSecurity sec = Document.GetSecurity(entity.OwnerDocumentId.Value, userId);
					isValid = sec.IsManager || sec.IsResource || sec.IsCreator;
				}
			}
			return isValid;
		}
		#endregion

		#region GetSubscriptionType
		public static SubscriptionTypes GetSubscriptionType(DateTypes dateType, int UserId, int ObjectId)
		{
			SubscriptionTypes SubscriptionType = SubscriptionTypes.UNDEFINED;
			using (IDataReader reader = DbSchedule2.GetReminderSubscriptionPersonalForObject((int)dateType, UserId, ObjectId))
			{
				if (reader.Read())
					SubscriptionType = (SubscriptionTypes)((int)reader["SubscriptionType"]);
			}
			return SubscriptionType;
		}
		#endregion

		#region SendReminder
		private static void SendReminder(DateTypes dateType, ObjectTypes objecType, int? objectId, Guid? objectUid, List<int> users)
		{
			// TODO: Step 0. Check Security (Not implemented yet)

			// Step 1. Calculate variables
			foreach (int userId in users)
			{
				int reminderType = 0;

				using (IDataReader reader = User.GetUserPreferences(userId))
				{
					if (reader.Read())
						reminderType = (int)reader["ReminderType"];
				}

				if (reminderType != 0)
				{
					// Step 2. Get Message Template
					ReminderTemplate tmpl = Reminder.GetMessageTemplate(dateType, User.GetUserLanguage(userId));

					// Step 2.1. Calculate Variables
					ArrayList vars = new ArrayList();

					Alerts2.GetObjectVariables(objecType, objectId, objectUid, false, Reminder.GetVariables(dateType), vars);

					// Step 3. Replace variables and GetMessage
					Alerts2.Message msg = Reminder.GetMessage(tmpl, objectId, objectUid, objecType, userId, (VariableInfo[])vars.ToArray(typeof(VariableInfo)));

					// Step 4. Save to log
					using (DbTransaction tran = DbTransaction.Begin())
					{
						int logId = DbAlert2.MessageLogAdd(msg.Subject, msg.Body);
						DBSystemEvents.RecipientUpdateSend(userId, (reminderType & 1) != 0, PortalConfig.UseIM && (reminderType & 2) != 0, logId); // IsNotifiedByEmail, IsNotifiedByIBN

						tran.Commit();
					}

					#region -- Send via Email --
					// Step 5. Send via Email
					try
					{
						if ((reminderType & 1) != 0)//IsNotifiedByEmail
						{
							string body = "<html><head><meta http-equiv=Content-Type content=\"text/html; charset=utf-8\" /></head><body>" + msg.Body + "</body></html>";

							using (DbTransaction tran = DbTransaction.Begin())
							{
								Alerts2.SendMessage(DeliveryType.Email, Reminder.GetAddress(DeliveryType.Email, userId), body, msg.Subject);

								DBSystemEvents.RecipientUpdateSent(userId, (int)DeliveryType.Email, true);

								tran.Commit();
							}
						}
					}
					catch (Exception ex)
					{
						Log.WriteError(ex.ToString());
					}
					#endregion

					#region -- Send via IM --
					// Step 6. Send via IM
					try
					{
						if ((reminderType & 2) != 0)//uInfo != null && uInfo.IsNotifiedByIBN)
						{
							using (DbTransaction tran = DbTransaction.Begin())
							{
								Alerts2.SendMessage(DeliveryType.IBN, Reminder.GetAddress(DeliveryType.IBN, userId), msg.Body, msg.Subject);

								DBSystemEvents.RecipientUpdateSent(userId, (int)DeliveryType.IBN, true);

								tran.Commit();
							}
						}
					}
					catch (Exception ex)
					{
						Log.WriteError(ex.ToString());
					}
					#endregion
				}
			}
		}
		#endregion

		#region private static bool ConfirmNotification(ObjectTypes objectType, DateTypes dateType, int? objectId, Guid? objectUid)
		private static bool ConfirmNotification(ObjectTypes objectType, DateTypes dateType, int? objectId, Guid? objectUid)
		{
			bool notify = false;

			bool hasRecurrence = false;
			if (objectId.HasValue)
			{
				using (IDataReader reader = DBCommon.GetRecurrence((int)objectType, objectId.Value))
				{
					hasRecurrence = reader.Read();
				}
			}

			switch (objectType)
			{
				case ObjectTypes.CalendarEntry:
					notify = CalendarEntry.ConfirmReminder(dateType, objectId.Value, hasRecurrence);
					break;
				case ObjectTypes.Project:
					notify = Project.ConfirmReminder(dateType, objectId.Value, hasRecurrence);
					break;
				case ObjectTypes.Task:
					notify = Task.ConfirmReminder(dateType, objectId.Value, hasRecurrence);
					break;
				case ObjectTypes.ToDo:
					notify = ToDo.ConfirmReminder(dateType, objectId.Value, hasRecurrence);
					break;
				case ObjectTypes.Assignment:
					AssignmentEntity entity = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, (PrimaryKeyId)objectUid.Value);
					if (entity != null && entity.State == (int)AssignmentState.Active)
						notify = true;
					break;

			}

			return notify;
		}
		#endregion
	}
}
