using System;
using System.Data;

namespace Mediachase.IBN.Database
{
	public class DbAlert2
	{
		private DbAlert2()
		{
		}

		#region TemplateGet()
		public static IDataReader TemplateGet(string key)
		{
			return DbHelper2.RunSpDataReader("Alert2TemplateGet",
				DbHelper2.mp("@Key", SqlDbType.NVarChar, 100, key));
		}
		#endregion
		#region TemplateDelete()
		public static void TemplateDelete(string key)
		{
			DbHelper2.RunSp("Alert2TemplateDelete",
				DbHelper2.mp("@Key", SqlDbType.NVarChar, 100, key));
		}
		#endregion
		#region TemplateUpdate()
		public static void TemplateUpdate(string key, string subject, string body)
		{
			DbHelper2.RunSp("Alert2TemplateUpdate",
				DbHelper2.mp("@Key", SqlDbType.NVarChar, 100, key),
				DbHelper2.mp("@Subject", SqlDbType.NVarChar, 1000, subject),
				DbHelper2.mp("@Body", SqlDbType.NText, body));
		}
		#endregion

		#region GetUser()
		public static IDataReader GetUser(int userId)
		{
			return DbHelper2.RunSpDataReader("Alert2UserGet",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId));
		}
		#endregion

		#region GetVariablesCalendarEntry()
		public static IDataReader GetVariablesCalendarEntry(int objectId)
		{
			return DbHelper2.RunSpDataReader("Alert2VariablesGetCalendarEntry",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId));
		}
		#endregion
		#region GetVariablesComment()
		public static IDataReader GetVariablesComment(int objectId)
		{
			return DbHelper2.RunSpDataReader("Alert2VariablesGetComment",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId));
		}
		#endregion
		#region GetVariablesDocument()
		public static IDataReader GetVariablesDocument(int objectId)
		{
			return DbHelper2.RunSpDataReader("Alert2VariablesGetDocument",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId));
		}
		#endregion
		#region GetVariablesFile()
		public static IDataReader GetVariablesFile(int objectId)
		{
			return DbHelper2.RunSpDataReader("Alert2VariablesGetFile",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId));
		}
		#endregion
		#region GetVariablesIssue()
		public static IDataReader GetVariablesIssue(int objectId)
		{
			return DbHelper2.RunSpDataReader("Alert2VariablesGetIssue",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId));
		}
		#endregion
		#region GetVariablesIssueBox()
		public static IDataReader GetVariablesIssueBox(int objectId)
		{
			return DbHelper2.RunSpDataReader("Alert2VariablesGetIssueBox",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId));
		}
		#endregion
		#region GetVariablesIssueRequest()
		public static IDataReader GetVariablesIssueRequest(int objectId)
		{
			return DbHelper2.RunSpDataReader("Alert2VariablesGetIssueRequest",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId));
		}
		#endregion
		#region GetVariablesList()
		public static IDataReader GetVariablesList(int objectId)
		{
			return DbHelper2.RunSpDataReader("Alert2VariablesGetList",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId));
		}
		#endregion
		#region GetVariablesProject()
		public static IDataReader GetVariablesProject(int objectId)
		{
			return DbHelper2.RunSpDataReader("Alert2VariablesGetProject",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId));
		}
		#endregion
		#region GetVariablesTask()
		public static IDataReader GetVariablesTask(int objectId)
		{
			return DbHelper2.RunSpDataReader("Alert2VariablesGetTask",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId));
		}
		#endregion
		#region GetVariablesTodo()
		public static IDataReader GetVariablesTodo(int objectId)
		{
			return DbHelper2.RunSpDataReader("Alert2VariablesGetTodo",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId));
		}
		#endregion
		#region GetVariablesUser()
		public static IDataReader GetVariablesUser(int objectId)
		{
			return DbHelper2.RunSpDataReader("Alert2VariablesGetUser",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId));
		}
		#endregion
		#region GetVariablesAssignment()
		public static IDataReader GetVariablesAssignment(Guid objectUid)
		{
			return DbHelper2.RunSpDataReader("Alert2VariablesGetAssignment",
				DbHelper2.mp("@ObjectUid", SqlDbType.UniqueIdentifier, objectUid));
		}
		#endregion

		#region GetEventTypeNames()
		/// <summary>
		/// Reader returns fields:
		///		LanguageId, Name 
		/// </summary>
		public static IDataReader GetEventTypeNames(int typeId)
		{
			return DbHelper2.RunSpDataReader("EventTypeGetNames",
				DbHelper2.mp("@TypeId", SqlDbType.Int, typeId));
		}
		#endregion
		#region GetIssueStateNames()
		/// <summary>
		/// Reader returns fields:
		///		LanguageId, Name 
		/// </summary>
		public static IDataReader GetIssueStateNames(int StateId)
		{
			return DbHelper2.RunSpDataReader("IncidentStateGetNames",
				DbHelper2.mp("@StateId", SqlDbType.Int, StateId));
		}
		#endregion
		#region GetObjectStateNames()
		/// <summary>
		/// Reader returns fields:
		///		LanguageId, Name 
		/// </summary>
		public static IDataReader GetObjectStateNames(int StateId)
		{
			return DbHelper2.RunSpDataReader("ObjectStateGetNames",
				DbHelper2.mp("@StateId", SqlDbType.Int, StateId));
		}
		#endregion
		#region GetPriorityNames()
		/// <summary>
		/// Reader returns fields:
		///		LanguageId, Name 
		/// </summary>
		public static IDataReader GetPriorityNames(int priorityId)
		{
			return DbHelper2.RunSpDataReader("PriorityGetNames",
				DbHelper2.mp("@PriorityId", SqlDbType.Int, priorityId));
		}
		#endregion
		#region GetProjectStatusNames()
		/// <summary>
		/// Reader returns fields:
		///		LanguageId, Name 
		/// </summary>
		public static IDataReader GetProjectStatusNames(int statusId)
		{
			return DbHelper2.RunSpDataReader("ProjectStatusGetNames",
				DbHelper2.mp("@StatusId", SqlDbType.Int, statusId));
		}
		#endregion

		#region MessageLogAdd()
		public static int MessageLogAdd(string subject, string body)
		{
			return DbHelper2.RunSpInteger("Alert2MessageLogAdd",
				DbHelper2.mp("@Subject", SqlDbType.NVarChar, 1000, subject),
				DbHelper2.mp("@Body", SqlDbType.NText, body));
		}
		#endregion

		#region Message_GetByUserIdAndTimeFilter
		public static IDataReader Message_GetByUserIdAndTimeFilter(int userId, DateTime startDate, DateTime endDate)
		{
			return DbHelper2.RunSpDataReader("Message_GetByUserIdAndTimeFilter",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, startDate),
				DbHelper2.mp("@EndDate", SqlDbType.DateTime, endDate));
		}
		#endregion


		#region AddBroadCastMessage()
		public static int AddBroadCastMessage(string text, DateTime expirationDate, int creatorId)
		{
			return DbHelper2.RunSpInteger("BroadCastMessageAdd",
				DbHelper2.mp("@Text", SqlDbType.NText, text),
				DbHelper2.mp("@ExpirationDate", SqlDbType.DateTime, expirationDate),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, creatorId));
		}
		#endregion
		#region AddBroadCastRecipient()
		public static void AddBroadCastRecipient(int messageId, int groupId)
		{
			DbHelper2.RunSp("BroadCastRecipientAdd",
				DbHelper2.mp("@MessageId", SqlDbType.Int, messageId),
				DbHelper2.mp("@GroupId", SqlDbType.Int, groupId));
		}
		#endregion
		#region GetBroadCastMessages()
		/// <summary>
		/// MessageId, CreationDate, ExpirationDate, Text, CreatorId
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="onlyActive"></param>
		/// <param name="TimeZoneId"></param>
		/// <returns></returns>
		public static IDataReader GetBroadCastMessages(int userId, bool onlyActive, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[] { "CreationDate", "ExpirationDate" },
				"BroadCastMessagesGet",
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@OnlyActive", SqlDbType.Bit, onlyActive));
		}
		#endregion
		#region GetBroadcastRecipients()
		public static IDataReader GetBroadcastRecipients(int messageId)
		{
			return DbHelper2.RunSpDataReader("BroadcastRecipientGetUsers",
				DbHelper2.mp("@MessageId", SqlDbType.Int, messageId));
		}
		#endregion
	}
}
