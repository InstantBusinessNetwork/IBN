using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

using Mediachase.Ibn;
using Mediachase.Ibn.Clients;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business
{
	#region * Enums *
	public enum ListAction
	{
		Add,
		Remove,
		Set
	}

	public enum ListType
	{
		GeneralCategories,
		IssueCategories,
		ProjectCategories,
		ProjectGroups,
		ProjectSponsors,
		ProjectStakeholders,
		ProjectTeamMembers
	}

	public enum ObjectTypes
	{
		UNDEFINED = -1,
		User = 1,
		UserGroup = 2,
		Project = 3,
		CalendarEntry = 4,
		Task = 5,
		ToDo = 6,
		Issue = 7,
		File_FileStorage = 8,
		Folder = 10,
		TimesheetTodo = 11,
		IssueRequest = 12,
		ProjectGroup = 13,
		Timesheet = 14,
		List = 15,
		Document = 16,
		Comment = 17,
		Client = 18,
		IssueBox = 19,
		KnowledgeBase = 20,
		Organization = 21,
		Contact = 22,
		IbnNext_Principal = 23,
		IbnNext_BlockTypeInstance = 24,
		Report = 25,
		File_MetaDataPlus = 26,
		File_MetaData = 27,
		File_Incident = 28,
		CalendarEvent = 29,
		Assignment = 30
	}

	public enum CompletionType
	{
		All = 1,
		Any = 2
	}

	public enum Priority
	{
		Low = 0,
		Normal = 500,
		High = 750,
		VeryHigh = 900,
		Urgent = 1000
	}


	public enum ConstraintTypes
	{
		AsSoonAsPossible = 0,
		StartNotEarlierThan = 4,
		StartNotLaterThan = 5
	}

	public enum BudgetTypes
	{
		Handly = 1,
		CalculatedUponChildren = 2,
		FromTable = 3
	}

	public enum PlaceTypes
	{
		Before_this = 1,
		After_this = 2,
		First_child = 3,
		Last_child = 4
	}

	public enum ObjectStates
	{
		Upcoming = 1,
		Active = 2,
		Overdue = 3,
		Suspended = 4,
		Completed = 5,
		ReOpen = 6,
		OnCheck = 7
	}

	public enum ActivationTypes
	{
		AutoWithCheck = 1,
		AutoNoCheck = 2,
		Manually = 3
	}
	#endregion

	public delegate void UpdateListDelegate(bool add, ObjectTypes objectType, int objectId, int itemId, object context);

	#region public class Client
	public class Client
	{
		private bool isContact;
		private PrimaryKeyId id;
		private string name;

		public bool IsContact
		{
			get
			{
				return isContact;
			}
		}

		public PrimaryKeyId Id
		{
			get
			{
				return id;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public Client(bool _is, PrimaryKeyId _id, string _name)
		{
			isContact = _is;
			id = _id;
			name = _name;
		}
	}
	#endregion

	/// <summary>
	/// Represents common.
	/// </summary>
	public static class Common
	{
		#region LoadCategories()
		internal static void LoadCategories(IDataReader reader, ArrayList list)
		{
			while (reader.Read())
			{
				list.Add((int)reader["CategoryId"]);
			}
		}
		#endregion

		#region LoadPrincipals()
		internal static void LoadPrincipals(IDataReader reader, ArrayList list)
		{
			LoadItems(reader, "PrincipalId", list);
		}
		#endregion
		#region LoadItems()
		internal static void LoadItems(IDataReader reader, string fieldName, ArrayList list)
		{
			while (reader.Read())
			{
				list.Add((int)reader[fieldName]);
			}
		}
		#endregion

		#region GetTabsByObjectType
		/// <summary>
		///	TabId, TemplateId, IsDefault, RowNum, PosNum, Title
		/// </summary>
		public static IDataReader GetTabsByObjectType(int ObjectTypeId)
		{
			return DBCommon.GetTabsByObjectType(ObjectTypeId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetPageViewByObjectType
		/// <summary>
		///	ViewPageId, MenuId, ShortInfoPath, Title, MenuXML
		/// </summary>
		public static IDataReader GetPageViewByObjectType(int ObjectTypeId)
		{
			return DBCommon.GetPageViewByObjectType(ObjectTypeId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region UpdateList()
		internal static void UpdateList(ListAction action, ArrayList oldItems, ArrayList newItems, ObjectTypes objectType, int objectId, SystemEventTypes eventType, UpdateListDelegate fnUpdate, object context)
		{
			ArrayList add = new ArrayList();
			ArrayList del = new ArrayList();
			foreach (int id in newItems)
			{
				switch (action)
				{
					case ListAction.Add:
						if (!oldItems.Contains(id))
							add.Add(id);
						break;
					case ListAction.Remove:
						if (oldItems.Contains(id))
							del.Add(id);
						break;
					case ListAction.Set:
						if (oldItems.Contains(id))
							oldItems.Remove(id);
						else
							add.Add(id);
						break;
				}
			}

			if (action == ListAction.Set)
				del.AddRange(oldItems);


			using (DbTransaction tran = DbTransaction.Begin())
			{
				bool updated = false;

				foreach (int id in add)
				{
					fnUpdate(true, objectType, objectId, id, context);
					updated = true;
				}

				foreach (int id in del)
				{
					fnUpdate(false, objectType, objectId, id, context);
					updated = true;
				}

				if (updated)
					SystemEvents.AddSystemEvents(eventType, objectId);

				tran.Commit();
			}
		}
		#endregion

		#region ListUpdate()
		public static void ListUpdate(bool add, ObjectTypes objectType, int objectId, int itemId, object context)
		{
			if (add)
				DBCommon.AssignCategoryToObject((int)objectType, objectId, itemId);
			else
				DBCommon.RemoveCategoryFromObject((int)objectType, objectId, itemId);
		}
		#endregion

		#region GetListDiscussions
		/// <summary>
		/// Reader returns fields:
		///		DiscussionId, ObjectTypeId, ObjectId, CreatorId, CreationDate, Text, CreatorName
		/// </summary>
		public static IDataReader GetListDiscussions(ObjectTypes ObjectTypeId, int ObjectId, int TimeZoneId)
		{
			if (ObjectTypeId == ObjectTypes.ToDo)	// вернуть с учётом родительского Task
				return DbHelper2.RunSpDataReader(
					TimeZoneId, new string[] { "CreationDate" },
					"DiscussionsGetByToDo",
					DbHelper2.mp("@ToDoId", SqlDbType.Int, ObjectId));
			else if (ObjectTypeId == ObjectTypes.Task)	// вернуть с учётом дочерних todo
				return DbHelper2.RunSpDataReader(
					TimeZoneId, new string[] { "CreationDate" },
					"DiscussionsGetByTask",
					DbHelper2.mp("@TaskId", SqlDbType.Int, ObjectId));
			else
				return DbHelper2.RunSpDataReader(
					TimeZoneId, new string[] { "CreationDate" },
					"DiscussionsGet",
					DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, (int)ObjectTypeId),
					DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion

		#region GetListDiscussionsDataTable
		/// <summary>
		/// Reader returns fields:
		///		DiscussionId, ObjectTypeId, ObjectId, CreatorId, CreationDate, Text, CreatorName
		/// </summary>
		public static DataTable GetListDiscussionsDataTable(ObjectTypes ObjectTypeId, int ObjectId, int TimeZoneId)
		{
			if (ObjectTypeId == ObjectTypes.ToDo)	// вернуть с учётом родительского Task
				return DbHelper2.RunSpDataTable(
					TimeZoneId, new string[] { "CreationDate" },
					"DiscussionsGetByToDo",
					DbHelper2.mp("@ToDoId", SqlDbType.Int, ObjectId));
			else if (ObjectTypeId == ObjectTypes.Task)	// вернуть с учётом дочерних todo
				return DbHelper2.RunSpDataTable(
					TimeZoneId, new string[] { "CreationDate" },
					"DiscussionsGetByTask",
					DbHelper2.mp("@TaskId", SqlDbType.Int, ObjectId));
			else
				return DbHelper2.RunSpDataTable(
					TimeZoneId, new string[] { "CreationDate" },
					"DiscussionsGet",
					DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, (int)ObjectTypeId),
					DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion

		#region GetListLanguages
		/// <summary>
		/// Reader returns fields:
		///		LanguageId, Locale, FriendlyName 
		/// </summary>
		public static IDataReader GetListLanguages()
		{
			return DBCommon.GetListLanguages();
		}
		#endregion

		#region GetListDeclinedRequests
		/// <summary>
		///		ObjectId, ObjectTypeId, Title, PriorityId, PriorityName, LastSavedDate, UserId, FirstName, LastName
		/// </summary>
		public static IDataReader GetListDeclinedRequests(int ProjectId)
		{
			return DBCommon.GetListDeclinedRequests(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.LanguageId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region IsPop3Folder
		public static bool IsPop3Folder(int DirectoryId)
		{
			return DBCommon.IsPop3Folder(DirectoryId);
		}
		#endregion

		#region GetListAllFavoritesDT
		public static DataTable GetListAllFavoritesDT()
		{
			return DBCommon.GetListFavoritesDT(-1, Security.CurrentUser.UserID);
		}
		#endregion

		#region AddBroadCastMessage()
		public static void AddBroadCastMessage(string text, DateTime expirationDate, ArrayList recipients)
		{
			expirationDate = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, expirationDate);

			int messageId;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				messageId = DbAlert2.AddBroadCastMessage(text, expirationDate, Security.CurrentUser.UserID);

				foreach (int groupId in recipients)
					DbAlert2.AddBroadCastRecipient(messageId, groupId);

				tran.Commit();
			}

			Alerts2.SendBroadcastMessage(messageId, text);
		}
		#endregion

		#region GetBroadCastMessages()
		/// <summary>
		/// MessageId, CreationDate, ExpirationDate, Text, CreatorId
		/// </summary>
		/// <param name="onlyActive"></param>
		/// <returns></returns>
		public static IDataReader GetBroadCastMessages(bool onlyActive)
		{
			return DbAlert2.GetBroadCastMessages(Security.CurrentUser.UserID, onlyActive, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region SendFiles()
		//public static void SendFiles(string containerKey, ArrayList files, ArrayList users)
		//{
		//    Alerts2.SendFiles(containerKey, files, users);
		//}
		#endregion

		#region GetListObjectStates
		/// <summary>
		/// Reader returns fields:
		///		StateId, StateName 
		/// </summary>
		public static IDataReader GetListObjectStates()
		{
			return DBCommon.GetListObjectStates(Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetIncidentType
		/// <summary>
		/// </summary>
		public static string GetIncidentType(int TypeId)
		{
			string retVal = "";
			using (IDataReader reader = Incident.GetListIncidentTypes())
			{
				while (reader.Read())
					if ((int)reader["TypeId"] == TypeId)
					{
						retVal = reader["TypeName"].ToString();
						break;
					}
			}
			return retVal;
		}
		#endregion

		#region GetDefaultIncidentType
		/// <summary>
		/// </summary>
		public static int GetDefaultIncidentType()
		{
			int retVal = -1;
			using (IDataReader reader = Incident.GetListIncidentTypes())
			{
				if (reader.Read())
					retVal = (int)reader["TypeId"];
			}
			return retVal;
		}
		#endregion

		#region GetIncidentSeverity
		/// <summary>
		/// </summary>
		public static string GetIncidentSeverity(int SeverityId)
		{
			string retVal = "";
			using (IDataReader reader = Incident.GetListIncidentSeverity())
			{
				while (reader.Read())
					if ((int)reader["SeverityId"] == SeverityId)
					{
						retVal = reader["SeverityName"].ToString();
						break;
					}
			}
			return retVal;
		}
		#endregion

		#region GetDefaultIncidentSeverity
		/// <summary>
		/// </summary>
		public static int GetDefaultIncidentSeverity()
		{
			int retVal = -1;
			using (IDataReader reader = Incident.GetListIncidentSeverity())
			{
				if (reader.Read())
					retVal = (int)reader["SeverityId"];
			}
			return retVal;
		}
		#endregion

		#region GetPriority
		/// <summary>
		/// </summary>
		public static string GetPriority(int PriorityId)
		{
			string retVal = "";
			using (IDataReader reader = Incident.GetListPriorities())
			{
				while (reader.Read())
					if ((int)reader["PriorityId"] == PriorityId)
					{
						retVal = reader["PriorityName"].ToString();
						break;
					}
			}
			return retVal;
		}
		#endregion

		#region GetActivationType
		/// <summary>
		/// </summary>
		public static string GetActivationType(int activationTypeId)
		{
			string retVal = "";
			using (IDataReader reader = Task.GetListActivationTypes())
			{
				while (reader.Read())
					if ((int)reader["ActivationTypeId"] == activationTypeId)
					{
						retVal = reader["ActivationTypeName"].ToString();
						break;
					}
			}
			return retVal;
		}
		#endregion

		#region GetCompletionType
		/// <summary>
		/// </summary>
		public static string GetCompletionType(int completionTypeId)
		{
			string retVal = "";
			using (IDataReader reader = Task.GetListCompletionTypes())
			{
				while (reader.Read())
					if ((int)reader["CompletionTypeId"] == completionTypeId)
					{
						retVal = reader["CompletionTypeName"].ToString();
						break;
					}
			}
			return retVal;
		}
		#endregion

		#region GetGeneralCategory
		/// <summary>
		/// </summary>
		public static string GetGeneralCategory(int CategoryId)
		{
			string retVal = "";
			using (IDataReader reader = Incident.GetListCategoriesAll())
			{
				while (reader.Read())
					if ((int)reader["CategoryId"] == CategoryId)
					{
						retVal = reader["CategoryName"].ToString();
						break;
					}
			}
			return retVal;
		}
		#endregion

		#region GetIncidentCategory
		/// <summary>
		/// </summary>
		public static string GetIncidentCategory(int CategoryId)
		{
			string retVal = "";
			using (IDataReader reader = Incident.GetListIncidentCategories())
			{
				while (reader.Read())
					if ((int)reader["CategoryId"] == CategoryId)
					{
						retVal = reader["CategoryName"].ToString();
						break;
					}
			}
			return retVal;
		}
		#endregion

		#region GetProjectCategory
		public static string GetProjectCategory(int CategoryId)
		{
			string retVal = "";
			using (IDataReader reader = Project.GetListProjectCategories())
			{
				while (reader.Read())
					if ((int)reader["CategoryId"] == CategoryId)
					{
						retVal = reader["CategoryName"].ToString();
						break;
					}
			}
			return retVal;
		}
		#endregion

		#region GetContact
		/// <summary>
		/// </summary>
		public static Client GetClient(string senderEmail)
		{
			if (string.IsNullOrEmpty(senderEmail))
				return null;

			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			string name = String.Empty;

			EntityObject[] entityList;

			FilterElement[] filters = CreateFiltersByEmail(OrganizationEntity.GetAssignedMetaClassName(), senderEmail);

			entityList = BusinessManager.List(OrganizationEntity.GetAssignedMetaClassName(), filters);
			if (entityList.Length > 0)
			{
				orgUid = entityList[0].PrimaryKeyId.Value;
				name = ((OrganizationEntity)entityList[0]).Name;
			}

			if (orgUid == PrimaryKeyId.Empty)
			{
				filters = CreateFiltersByEmail(ContactEntity.GetAssignedMetaClassName(), senderEmail);

				entityList = BusinessManager.List(ContactEntity.GetAssignedMetaClassName(), filters);

				if (entityList.Length > 0)
				{
					contactUid = entityList[0].PrimaryKeyId.Value;
					name = ((ContactEntity)entityList[0]).FullName;
				}
			}

			if (orgUid != PrimaryKeyId.Empty)
				return new Client(false, orgUid, name);
			else if (contactUid != PrimaryKeyId.Empty)
				return new Client(true, contactUid, name);
			else
				return null;
		}

		public static FilterElement[] CreateFiltersByEmail(string metaClassName, string senderEmail)
		{
			if (metaClassName == null)
				throw new ArgumentNullException("metaClassName");
			if (senderEmail == null)
				throw new ArgumentNullException("senderEmail");

			OrBlockFilterElement orBlock = new OrBlockFilterElement();

			Mediachase.Ibn.Data.Meta.Management.MetaFieldType emailType = DataContext.Current.GetMetaFieldType("EMail");

			foreach (Mediachase.Ibn.Data.Meta.Management.MetaField metaField in DataContext.Current.GetMetaClass(metaClassName).Fields)
			{
				if (metaField.GetMetaType() == emailType)
				{
					orBlock.ChildElements.Add(FilterElement.EqualElement(metaField.Name, senderEmail));
				}
			}

			return new FilterElement[] { orBlock };
		}
		#endregion

		#region internal static bool ConfirmReminder(bool hasRecurrence, int dateTypeId, int stateId, int startDateTypeId, int finishDateTypeId, int upcomingStateId, int completedStateId, int suspendedStateId)
		internal static bool ConfirmReminder(bool hasRecurrence, int dateTypeId, int stateId, int startDateTypeId, int finishDateTypeId, int upcomingStateId, int completedStateId, int suspendedStateId)
		{
			bool result = false;

			if (stateId > 0)
			{
				// For start date:
				// confirm if state is upcoming or (has recurrence and state is active)
				if (dateTypeId == startDateTypeId && (stateId == upcomingStateId || hasRecurrence && stateId != completedStateId && stateId != suspendedStateId))
					result = true;

				// For finish date:
				// confirm if state is active
				if (dateTypeId == finishDateTypeId && stateId != completedStateId && stateId != suspendedStateId)
					result = true;
			}

			return result;
		}
		#endregion

		#region GetListArticles
		/// <summary>
		///	ArticleId, Question, Answer, AnswerHTML, Tags, Created,
		/// NoveltyRate, CounterRate, TotalRate
		/// </summary>
		public static DataTable GetListArticles(string sSearch)
		{
			// days to obsolescence
			// TODO: брать извне!!!!
			int period = 30;

			// difference in minutes between the start date and current date
			int diff = period * 24 * 60;

			// start date for novelty rate
			DateTime startDate = DateTime.UtcNow.AddDays(-period);

			// Coefficients
			// TODO: брать извне!!!!
			decimal noveltyRateCoef = 1.0m;
			decimal counterRateCoef = 1.0m;
			decimal questionRateCoef = 10.0m;
			decimal answerRateCoef = 1.0m;
			decimal tagRateCoef = 5.0m;

			// parameters
			ArrayList sqlParams = new ArrayList();
			sqlParams.Add(DbHelper2.mp("@StartDate", SqlDbType.DateTime, startDate));
			sqlParams.Add(DbHelper2.mp("@Diff", SqlDbType.Decimal, 9, diff));
			sqlParams.Add(DbHelper2.mp("@NoveltyRateCoef", SqlDbType.Decimal, 9, noveltyRateCoef));
			sqlParams.Add(DbHelper2.mp("@CounterRateCoef", SqlDbType.Decimal, 9, counterRateCoef));
			sqlParams.Add(DbHelper2.mp("@QuestionRateCoef", SqlDbType.Decimal, 9, questionRateCoef));
			sqlParams.Add(DbHelper2.mp("@AnswerRateCoef", SqlDbType.Decimal, 9, answerRateCoef));
			sqlParams.Add(DbHelper2.mp("@TagRateCoef", SqlDbType.Decimal, 9, tagRateCoef));

			string sql =
			  "DECLARE @maxCounter decimal(9) \r\n" +
			  "SELECT @maxCounter = MAX(Counter) FROM Articles \r\n";

			if (sSearch != "")
			{
				string[] words = sSearch.Split(' ');
				string whereFilter = "";
				string questionCols = "";
				string answerCols = "";
				string tagCols = "";
				string questionRate = " + (";
				string answerRate = " + (";
				string tagRate = " + (";

				for (int i = 0; i < words.Length; i++)
				{
					if (i > 0)
					{
						whereFilter += " AND ";
						questionRate += " + ";
						answerRate += " + ";
						tagRate += " + ";
					}

					whereFilter += String.Format("(A.Tags LIKE '%' + @Word{0} + '%' OR A.Question LIKE '%' + @Word{0} + '%' OR A.Answer LIKE '%' + @Word{0} + '%')", i);

					questionCols += String.Format(", CASE WHEN A.Question LIKE '%' + @Word{0} + '%' THEN 1.0 ELSE 0.0 END AS Q{0}", i);
					answerCols += String.Format(", CASE WHEN A.Answer LIKE '%' + @Word{0} + '%' THEN 1.0 ELSE 0.0 END AS A{0}", i);
					tagCols += String.Format(", CASE WHEN A.Tags LIKE '%' + @Word{0} + '%' THEN 1.0 ELSE 0.0 END AS T{0}", i);

					questionRate += String.Format("Q{0}", i);
					answerRate += String.Format("A{0}", i);
					tagRate += String.Format("T{0}", i);

					sqlParams.Add(DbHelper2.mp("@Word" + i, SqlDbType.NVarChar, 50, words[i]));
				}

				questionRate += String.Format(") / {0} * @QuestionRateCoef", words.Length);
				answerRate += String.Format(") / {0} * @AnswerRateCoef", words.Length);
				tagRate += String.Format(") / {0} * @TagRateCoef", words.Length);

				sql +=
				  "SELECT ArticleId, Question, Answer, AnswerHTML, Tags, Created, NoveltyRate, CounterRate," +
				  " (NoveltyRate * @NoveltyRateCoef + CounterRate * @CounterRateCoef" +
				  questionRate + answerRate + tagRate + ") AS TotalRate" +
				  " FROM (" +
				  "SELECT A.* " +
				  " , CASE WHEN Created < @StartDate THEN 0.0 ELSE DATEDIFF(mi, @StartDate, Created) / @Diff END AS NoveltyRate" +
				  " , CASE WHEN @maxCounter = 0 THEN 0.0 ELSE A.Counter / @maxCounter END AS CounterRate" +
				  questionCols + answerCols + tagCols +
				  " FROM Articles A" +
				  " WHERE " + whereFilter +
				  ") AA " +
				  "ORDER BY TotalRate DESC";

			}
			else  // if search string is empty
			{
				sql +=
				  "SELECT ArticleId, Question, Answer, AnswerHTML, Tags, Created, NoveltyRate, CounterRate," +
				  " (NoveltyRate * @NoveltyRateCoef + CounterRate * @CounterRateCoef) AS TotalRate" +
				  " FROM (" +
				  "SELECT ArticleId, Question, Answer, AnswerHTML, Tags, Created" +
				  " , CASE WHEN Created < @StartDate THEN 0.0 ELSE DATEDIFF(mi, @StartDate, Created) / @Diff END AS NoveltyRate" +
				  " , CASE WHEN @maxCounter = 0 THEN 0.0 ELSE Counter / @maxCounter END AS CounterRate" +
				  " FROM Articles" +
				  ") AA " +
				  "ORDER BY TotalRate DESC"; ;
			}
			return DbHelper2.RunTextDataTable(
			  Security.CurrentUser.TimeZoneId,
			  new string[] { "Created" },
			  sql,
			  sqlParams.ToArray());
		}
		#endregion

		#region GetListArticlesUsedByUser
		/// <summary>
		///		ArticleId, Question, Answer, AnswerHTML
		/// </summary>
		public static DataTable GetListArticlesUsedByUser()
		{
			return DBCommon.GetListArticlesUsedByUser(Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListArticlesByTag
		/// <summary>
		///	ArticleId, Question, Answer, AnswerHTML, Tags, Created, 
		/// NoveltyRate, CounterRate, TotalRate
		/// </summary>
		public static DataTable GetListArticlesByTag(string Tag)
		{
			// days to obsolescence
			// TODO: брать извне!!!!
			int period = 30;

			// difference in minutes between the start date and current date
			int diff = period * 24 * 60;

			// start date for novelty rate
			DateTime startDate = DateTime.UtcNow.AddDays(-period);

			// Coefficients
			// TODO: брать извне!!!!
			decimal noveltyRateCoef = 1.0m;
			decimal counterRateCoef = 1.0m;
			decimal questionRateCoef = 10.0m;
			decimal answerRateCoef = 1.0m;

			// parameters
			ArrayList sqlParams = new ArrayList();
			sqlParams.Add(DbHelper2.mp("@StartDate", SqlDbType.DateTime, startDate));
			sqlParams.Add(DbHelper2.mp("@Diff", SqlDbType.Decimal, 9, diff));
			sqlParams.Add(DbHelper2.mp("@NoveltyRateCoef", SqlDbType.Decimal, 9, noveltyRateCoef));
			sqlParams.Add(DbHelper2.mp("@CounterRateCoef", SqlDbType.Decimal, 9, counterRateCoef));
			sqlParams.Add(DbHelper2.mp("@QuestionRateCoef", SqlDbType.Decimal, 9, questionRateCoef));
			sqlParams.Add(DbHelper2.mp("@AnswerRateCoef", SqlDbType.Decimal, 9, answerRateCoef));
			sqlParams.Add(DbHelper2.mp("@Tag", SqlDbType.NVarChar, 50, Tag));

			string sql =
			  "DECLARE @maxCounter decimal(9) \r\n" +
			  "SELECT @maxCounter = MAX(Counter) FROM Articles \r\n";

			string questionCols = ", CASE WHEN A.Question LIKE '%' + @Tag + '%' THEN 1.0 ELSE 0.0 END AS Q";
			string answerCols = ", CASE WHEN A.Answer LIKE '%' + @Tag + '%' THEN 1.0 ELSE 0.0 END AS A";

			string questionRate = " + Q * @QuestionRateCoef";
			string answerRate = " + A * @AnswerRateCoef";

			sql +=
			  "SELECT ArticleId, Question, Answer, AnswerHTML, Tags, Created, NoveltyRate, CounterRate," +
			  " (NoveltyRate * @NoveltyRateCoef + CounterRate * @CounterRateCoef" +
			  questionRate + answerRate + ") AS TotalRate" +
			  " FROM (" +
			  "SELECT A.* " +
			  " , CASE WHEN Created < @StartDate THEN 0.0 ELSE DATEDIFF(mi, @StartDate, Created) / @Diff END AS NoveltyRate" +
			  " , CASE WHEN @maxCounter = 0 THEN 0.0 ELSE A.Counter / @maxCounter END AS CounterRate" +
			  questionCols + answerCols +
			  " FROM Articles A" +
			  " WHERE ArticleId IN (SELECT ObjectId FROM Tags WHERE ObjectTypeId = " + (int)ObjectTypes.KnowledgeBase + " AND Tag = @Tag)" +
			  ") AA " +
			  "ORDER BY TotalRate DESC";

			return DbHelper2.RunTextDataTable(
			  Security.CurrentUser.TimeZoneId,
			  new string[] { "Created" },
			  sql,
			  sqlParams.ToArray());
		}
		#endregion

		#region GetListTagsForCloud
		/// <summary>
		/// Gets the list tags for cloud.
		/// </summary>
		/// <param name="ObjectTypeId">The object type id. Typically 20 - Articles</param>
		/// <param name="TagSizeCount">Quantity steps for tag size</param>
		/// <param name="TagCount">Max record count to return</param>
		/// <returns>DataTable with columns: Tag, TagSize</returns>
		public static DataTable GetListTagsForCloud(int ObjectTypeId, int TagSizeCount, int TagCount)
		{
			return DBCommon.GetListTagsForCloud(ObjectTypeId, TagSizeCount, TagCount);
		}
		#endregion

		#region GetListTagsForCloudByTag
		/// <summary>
		/// Gets the list of tags for cloud by tag.
		/// </summary>
		/// <param name="ObjectTypeId">The object type id. Typically 20 - Articles</param>
		/// <param name="TagSizeCount">Quantity steps for tag size</param>
		/// <param name="TagCount">Max record count to return</param>
		/// <param name="Tag">Related tag</param>
		/// <returns>DataTable with columns: Tag, TagSize</returns>
		public static DataTable GetListTagsForCloudByTag(int ObjectTypeId, int TagSizeCount, int TagCount, string Tag)
		{
			return DBCommon.GetListTagsForCloudByTag(ObjectTypeId, TagSizeCount, TagCount, Tag);
		}
		#endregion

		#region GetArticle
		/// <summary>
		///		ArticleId, Question, Answer, AnswerHTML, Tags, Created, Counter, Delimiter
		/// </summary>
		public static IDataReader GetArticle(int ArticleId)
		{
			return DBCommon.GetArticle(ArticleId, Security.CurrentUser.TimeZoneId);
		}

		public static DataTable GetArticleDT(int ArticleId)
		{
			return DBCommon.GetArticleDT(ArticleId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region AddArticle
		public static int AddArticle(string Question, string AnswerHTML, string Tags, string Delimiter)
		{
			int ArticleId = -1;

			string[] tagArray = Tags.Split(new string[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				ArticleId = DBCommon.AddArticle(Question, RemoveHTMLTags(AnswerHTML), AnswerHTML, Tags, Delimiter);

				foreach (string tag in tagArray)
				{
					DBCommon.AddTag((int)ObjectTypes.KnowledgeBase, ArticleId, tag.Trim());
				}

				tran.Commit();
			}

			return ArticleId;
		}
		#endregion

		#region UpdateArticle
		public static void UpdateArticle(int ArticleId, string Question, string AnswerHTML, string Tags, string Delimiter)
		{
			string[] tagArray = Tags.Split(new string[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBCommon.UpdateArticle(ArticleId, Question, RemoveHTMLTags(AnswerHTML), AnswerHTML, Tags, Delimiter);

				DBCommon.DeleteTagsByObject((int)ObjectTypes.KnowledgeBase, ArticleId);

				foreach (string tag in tagArray)
				{
					DBCommon.AddTag((int)ObjectTypes.KnowledgeBase, ArticleId, tag.Trim());
				}

				tran.Commit();
			}
		}
		#endregion

		#region DeleteArticle
		public static void DeleteArticle(int ArticleId)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				// OZ: File Storage
				BaseIbnContainer container = BaseIbnContainer.Create("FileLibrary", UserRoleHelper.CreateArticleContainerKey(ArticleId));
				FileStorage fs = (FileStorage)container.LoadControl("FileStorage");
				fs.DeleteAll();
				//

				DBCommon.DeleteArticle(ArticleId);

				tran.Commit();
			}
		}
		#endregion

		#region RemoveHTMLTags
		private static string RemoveHTMLTags(string HtmlText)
		{
			string noTagText = HtmlText;

			string regex = "</?[a-z][a-z0-9]*[^<>]*>";
			RegexOptions options = (RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase);
			System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(regex, options);

			foreach (Match m in reg.Matches(HtmlText))
			{
				noTagText = noTagText.Replace(m.Value, string.Empty);
			}

			return noTagText;
		}
		#endregion

		#region OpenInNewWindow
		public static bool OpenInNewWindow(string contentType)
		{
			bool retval = false;
			using (IDataReader reader = ContentType.GetContentTypeByString(contentType))
			{
				if (reader.Read())
				{
					if ((bool)reader["AllowNewWindow"])
						retval = true;
				}
			}
			return retval;
		}
		#endregion

		#region OpenInNewWindow
		public static bool OpenFileInNewWindow(string fileName)
		{
			bool retval = false;
			string ext = Path.GetExtension(fileName);
			if (ext.StartsWith(".") && ext.Length > 1)
				ext = ext.Substring(1);
			using (IDataReader reader = ContentType.GetContentTypeByExtension(ext))
			{
				if (reader.Read())
				{
					if ((bool)reader["AllowNewWindow"])
						retval = true;
				}
			}
			return retval;
		}
		#endregion

		#region AddHistory
		public static void AddHistory(ObjectTypes ObjectType, int ObjectId, string Title)
		{
			DBCommon.AddHistory((int)ObjectType, ObjectId, Title, Security.CurrentUser.UserID);
		}
		#endregion

		#region DeleteHistory
		public static void DeleteHistory(ObjectTypes objectType, int objectId)
		{
			DeleteHistory((int)objectType, objectId);
		}
		public static void DeleteHistory(int objectTypeId, int objectId)
		{
			DBCommon.DeleteHistory(objectTypeId, objectId);
		}
		#endregion

		#region AddUsedHistory
		public static void AddUsedHistory(int ObjectTypeId, int ObjectId, string Title)
		{
			DBCommon.AddUsedHistory(ObjectTypeId, ObjectId, Title, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetArticleTitle
		public static string GetArticleTitle(int ArticleId)
		{
			string retval = "";
			using (IDataReader reader = DBCommon.GetArticle(ArticleId, Security.CurrentUser.TimeZoneId))
			{
				if (reader.Read())
					retval = reader["Question"].ToString();
			}
			return retval;
		}
		#endregion

		#region IncreaseArticleCounter
		public static void IncreaseArticleCounter(int ArticleId)
		{
			DBCommon.IncreaseArticleCounter(ArticleId);
		}
		#endregion

		#region GetListHistoryDT
		/// <summary>
		///		HistoryId, ObjectTypeId, ObjectId, ObjectTitle, Dt, ClassName, ObjectUid, ObjectCode
		/// </summary>
		public static DataTable GetListHistoryDT()
		{
			return DBCommon.GetListHistoryDT(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListHistoryByObjectDT
		/// <summary>
		///		UserId, Dt
		/// </summary>
		public static DataTable GetListHistoryByObjectDT(int ObjectId, int ObjectTypeId)
		{
			return DBCommon.GetListHistoryByObjectDT(ObjectId, ObjectTypeId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListHistoryFull
		/// <summary>
		/// HistoryId, ObjectTypeId, ObjectId, ObjectTitle, Dt, IsView
		/// </summary>
		/// <returns>DataTable</returns>
		public static DataTable GetListHistoryFull()
		{
			return DBCommon.GetListHistoryFull(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetWebResourceString
		public static string GetWebResourceString(string template)
		{
			return GetWebResourceString(template, Thread.CurrentThread.CurrentCulture);
		}
		#endregion

		#region public static string GetWebResourceString(string template, CultureInfo culture)
		public static string GetWebResourceString(string template, CultureInfo culture)
		{
			string retVal = template;

			if (!string.IsNullOrEmpty(template))
			{
				int begin = template.IndexOf("{");
				int end = template.IndexOf("}");
				if (begin >= 0 && end >= begin)
				{
					string oldValue = template.Substring(begin, end - begin + 1);

					string classKey = "Global";
					string resourceKey = oldValue.Substring(1, oldValue.Length - 2);
					int separator = resourceKey.IndexOf(":");
					if (separator >= 0)
					{
						classKey = resourceKey.Substring(0, separator);
						resourceKey = resourceKey.Substring(separator + 1);
					}

					try
					{
						string resourceValue = HttpContext.GetGlobalResourceObject(classKey, resourceKey, culture) as string;
						if (resourceValue != null)
							retVal = template.Replace(oldValue, resourceValue);
					}
					catch (MissingManifestResourceException)
					{
					}
				}
			}

			return retVal;
		}
		#endregion

		#region AddFavorites
		public static void AddFavorites(int objectId, ObjectTypes objectType)
		{
			DBCommon.AddFavorites((int)objectType, objectId, Security.CurrentUser.UserID);
		}
		#endregion

		#region AddFavoritesByUid
		public static void AddFavoritesByUid(Guid objectUid, ObjectTypes objectType)
		{
			DBCommon.AddFavoritesByUid((int)objectType, objectUid, Security.CurrentUser.UserID);
		}
		#endregion

		#region CheckFavorites
		public static bool CheckFavorites(int objectId, ObjectTypes objectType)
		{
			return DBCommon.CheckFavorites((int)objectType, objectId, Security.CurrentUser.UserID);
		}
		#endregion

		#region CheckFavoritesByUid
		public static bool CheckFavoritesByUid(Guid objectUid, ObjectTypes objectType)
		{
			return DBCommon.CheckFavoritesByUid((int)objectType, objectUid, Security.CurrentUser.UserID);
		}
		#endregion

		#region DeleteFavorites
		public static void DeleteFavorites(int objectId, ObjectTypes objectType)
		{
			DBCommon.DeleteFavorites((int)objectType, objectId, Security.CurrentUser.UserID);
		}
		#endregion

		#region DeleteFavoritesByUid
		public static void DeleteFavoritesByUid(Guid objectUid, ObjectTypes objectType)
		{
			DBCommon.DeleteFavoritesByUid((int)objectType, objectUid, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListFavoritesDT
		/// <summary>
		///		FavoriteId, ObjectTypeId, ObjectId, ObjectUid, UserId, Title 
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListFavoritesDT(ObjectTypes objectType)
		{
			return DBCommon.GetListFavoritesDT((int)objectType, Security.CurrentUser.UserID);
		}
		#endregion

		#region AddGate
		/// <summary>
		/// Adds the gate.
		/// </summary>
		/// <param name="objectTypeId">The object type id.</param>
		/// <param name="objectId">The object id.</param>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public static Guid AddGate(int objectTypeId, int objectId, int userId)
		{
			return DBCommon.AddGate(objectTypeId, objectId, userId);
		}

		/// <summary>
		/// Adds the gate.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="objectId">The object id.</param>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public static Guid AddGate(ObjectTypes objectType, int objectId, int userId)
		{
			return DBCommon.AddGate((int)objectType, objectId, userId);
		}
		#endregion

		#region AddEntityHistory
		public static void AddEntityHistory(string className, PrimaryKeyId objectId)
		{
			try
			{
				Mediachase.Ibn.Data.Meta.Management.MetaClass mc = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(className);
				string titleFieldName = mc.TitleFieldName;

				EntityObject obj = BusinessManager.Load(className, objectId);
				if (obj != null)
					DBCommon.AddEntityHistory(className, objectId, obj.Properties[titleFieldName].Value.ToString(), Security.CurrentUser.UserID, true);
			}
			catch
			{
			}
		}

		public static void AddEntityHistory(string className, PrimaryKeyId objectId, string title)
		{
			DBCommon.AddEntityHistory(className, objectId, title, Security.CurrentUser.UserID, true);
		}
		#endregion

		#region AddUsedEntityHistory
		public static void AddUsedEntityHistory(string className, PrimaryKeyId objectId, string title)
		{
			DBCommon.AddEntityHistory(className, objectId, title, Security.CurrentUser.UserID, false);
		}
		#endregion

		#region GetListEntityHistoryFull
		/// <summary>
		/// HistoryId, ClassName, ObjectId, ObjectTitle, Dt, IsView
		/// </summary>
		/// <returns>DataTable</returns>
		public static DataTable GetListEntityHistoryFull()
		{
			return DBCommon.GetListEntityHistoryFull(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region DeleteEntityHistory
		public static void DeleteEntityHistory(string className, PrimaryKeyId objectId)
		{
			DBCommon.DeleteEntityHistory(className, objectId);
		}
		#endregion

		#region WriteExceptionToEventLog
		/// <summary>
		/// Writes the exception to event log.
		/// </summary>
		/// <param name="ex">The ex.</param>
		public static void WriteExceptionToEventLog(Exception ex)
		{
			Log.WriteException(ex);
		}
		#endregion

		#region GetDefaultClient
		public static void GetDefaultClient(string defaultString,
			out PrimaryKeyId contact_uid, out PrimaryKeyId org_uid)
		{
			org_uid = PrimaryKeyId.Empty;
			contact_uid = PrimaryKeyId.Empty;
			if (!String.IsNullOrEmpty(defaultString) && defaultString != "_")
			{
				string className = defaultString.Substring(0, defaultString.IndexOf("_"));
				string uid = defaultString.Substring(defaultString.IndexOf("_") + 1);
				if (className.ToLower() == OrganizationEntity.GetAssignedMetaClassName().ToLower())
					org_uid = PrimaryKeyId.Parse(uid);
				else
					contact_uid = PrimaryKeyId.Parse(uid);
			}
		}
		#endregion

		#region GetDefaultIncidentCategories
		public static ArrayList GetDefaultIncidentCategories()
		{
			return StringToArrayList(PortalConfig.IncidentDefaultValueIncidentCategoriesField);
		}
		public static ArrayList GetDefaultIncidentCategories(string value)
		{
			return StringToArrayList(value);
		}
		#endregion

		#region StringToArrayList
		public static ArrayList StringToArrayList(string defaultValue)
		{
			ArrayList alCategories = new ArrayList();
			string[] mas = defaultValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string s in mas)
				alCategories.Add(int.Parse(s));
			return alCategories;
		}
		#endregion

		#region GetListCategoriesByObject
		/// <summary>
		/// Gets the list categories.by objects
		/// </summary>
		/// <param name="objectTypeId">The object type id.</param>
		/// <param name="objectId">The object id.</param>
		/// <returns>CategoryId, CategoryName </returns>
		public static IDataReader GetListCategoriesByObject(int objectTypeId, int objectId)
		{
			return DBCommon.GetListCategoriesByObject(objectTypeId, objectId);
		}
		#endregion

		#region GetDefaultGeneralCategories
		public static ArrayList GetDefaultGeneralCategories(string defaultString)
		{
			ArrayList alCategories = new ArrayList();
			string[] mas = defaultString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string s in mas)
				alCategories.Add(int.Parse(s));
			return alCategories;
		}
		#endregion

		#region GetCurrency
		/// <summary>
		/// </summary>
		public static string GetCurrency(int currencyId)
		{
			string retVal = "";
			using (IDataReader reader = Project.GetListCurrency())
			{
				while (reader.Read())
					if ((int)reader["CurrencyId"] == currencyId)
					{
						retVal = reader["CurrencySymbol"].ToString();
						break;
					}
			}
			return retVal;
		}
		#endregion
	}
}
