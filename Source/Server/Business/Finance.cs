using System;
using System.Data;
using System.Collections;

using Mediachase.Ibn;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for Finance.
	/// </summary>
	public class Finance
	{
		public const string ROOT_ACCOUNT = "Root";
		public const string TIMESHEET_ACCOUNT_DESCRIPTION = "TimeSheet";

		#region CanView
		public static bool CanView(int AccountId)
		{
			int ProjectId = -1;
			using (IDataReader reader = DBFinance.GetAccount(AccountId))
			{
				reader.Read();
				ProjectId = (int)reader["ProjectId"];
			}

			return Project.CanViewFinances(ProjectId);
		}
		#endregion

		#region CanWork
		public static bool CanWork(int AccountId)
		{
			int ProjectId = -1;
			using (IDataReader reader = DBFinance.GetAccount(AccountId))
			{
				reader.Read();
				ProjectId = (int)reader["ProjectId"];
			}

			return Project.CanEditFinances(ProjectId);
		}

		public static bool CanWork(int ObjectTypeId, int ObjectId)
		{
			if (ObjectTypeId == (int)ObjectTypes.Task)
				return Task.CanViewFinances(ObjectId);
			else if (ObjectTypeId == (int)ObjectTypes.CalendarEntry)
				return CalendarEntry.CanViewFinances(ObjectId);
			else if (ObjectTypeId == (int)ObjectTypes.Document)
				return Document.CanViewFinances(ObjectId);
			else if (ObjectTypeId == (int)ObjectTypes.Issue)
				return Incident.CanViewFinances(ObjectId);
			else if (ObjectTypeId == (int)ObjectTypes.ToDo)
				return ToDo.CanViewFinances(ObjectId);
			else if (ObjectTypeId == (int)ObjectTypes.Project)
				return Project.CanEditFinances(ObjectId);
			else 
				return false;
		}
		#endregion

		#region GetListAccounts
		/// <summary>
		/// Reader returns fields:
		///		AccountId, ProjectId, Title, AccountNum, OutlineLevel, OutlineNumber, IsSummary
		/// </summary>
		public static IDataReader GetListAccounts(int ProjectId)
		{
			return DBFinance.GetListAccounts(ProjectId);
		}

		public static DataTable GetListAccountsDataTable(int ProjectId)
		{
			return DBFinance.GetListAccountsDataTable(ProjectId);
		}
		#endregion

		#region RecalculateActualAccounts
		internal static void RecalculateActualAccounts(int AccountId)
		{
			// Пересчитаем текущий счёт
			DBFinance.RecalculateACurAccount(AccountId, Security.CurrentUser.UserID);

			RecalculateParentAccounts(AccountId, false);
		}
		#endregion

		#region RecalculateParentAccounts
		private static void RecalculateParentAccounts(int AccountId, bool IncludeCurrent)
		{
			ArrayList parentAccounts = new ArrayList();
			if (IncludeCurrent)
				parentAccounts.Add(AccountId);

			// Найдём всех родителей
			using (IDataReader reader = DBFinance.GetListParentAccounts(AccountId))
			{
				while (reader.Read())
					parentAccounts.Add(reader["AccountId"]);
			}

			// Пересчитаем родителей
			foreach (int parentAccountId in parentAccounts)
				DBFinance.RecalculateParentAccount(parentAccountId, Security.CurrentUser.UserID);
		}
		#endregion

		#region RecalculateParentAccountWithPreserving
		private static void RecalculateParentAccountWithPreserving(int AccountId)
		{
			int ParentId = DBFinance.GetParentAccountId(AccountId);

			if (ParentId > 0)
				DBFinance.RecalculateParentAccountWithPreserving(ParentId, Security.CurrentUser.UserID);
		}
		#endregion

		#region CollapseAccount
		public static void CollapseAccount(int AccountId)
		{
			DBFinance.AddCollapsedAccount(Security.CurrentUser.UserID, AccountId);
		}
		#endregion

		#region ExpandAccount
		public static void ExpandAccount(int AccountId)
		{
			DBFinance.DeleteCollapsedAccount(Security.CurrentUser.UserID, AccountId);
		}
		#endregion

		#region GetListAccountsByProjectCollapsed
		/// <summary>
		/// AccountId, ProjectId, Title, OutlineLevel, OutlineNumber, IsSummary, IsCollapsed,
		/// TTotal, TCur, TSub,
		/// ETotal, ECur, ESub,
		/// ATotal, ACur, ASub
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListAccountsByProjectCollapsed(int project_id)
		{
			DataTable dt = DBFinance.GetListAccountsByProjectCollapsed(project_id, Security.CurrentUser.UserID);

			// Удалим невидимые строки
			int OldOutlineLevel = 1;
			bool OldIsCollapsed = false;
			Hashtable htT = new Hashtable();
			Hashtable htE = new Hashtable();
			foreach (DataRow dr in dt.Rows) 
			{
				int OutlineLevel = (int)dr["OutlineLevel"];
				bool IsCollapsed = (bool)dr["IsCollapsed"];

				if (OldIsCollapsed && OldOutlineLevel < OutlineLevel)
				{
					dr.Delete();
				}
				else
				{
					OldOutlineLevel = OutlineLevel;
					OldIsCollapsed = IsCollapsed;

					// Выставим значения для TParent и EParent
					htT[OutlineLevel] = (decimal)dr["TCur"];
					htE[OutlineLevel] = (decimal)dr["ECur"];

					if (OutlineLevel == 1)
					{
						dr["TParent"] = decimal.MaxValue;
						dr["EParent"] = decimal.MaxValue;
					}
					else
					{
						dr["TParent"] = (decimal)htT[OutlineLevel - 1] + (decimal)dr["TTotal"];
						dr["EParent"] = (decimal)htE[OutlineLevel - 1] + (decimal)dr["ETotal"];
					}
				}
			}
			return dt;
		}
		#endregion

		#region AddChildAccount
		public static int AddChildAccount(int ParentId, string Title)
		{
			if (!CanWork(ParentId))
				throw new AccessDeniedException();

			return DBFinance.AddChildAccount(ParentId, Title, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListAccountsForMove
		/// <summary>
		/// Reader returns fields:
		///		AccountId, Title, OutlineLevel, OutlineNumber, IsSummary
		/// </summary>
		public static IDataReader GetListAccountsForMove(int AccountId)
		{
			return DBFinance.GetListAccountsForMove(AccountId);
		}

		public static DataTable GetListAccountsForMoveDataTable(int AccountId)
		{
			return DBFinance.GetListAccountsForMoveDataTable(AccountId);
		}
		#endregion

		#region UpdateTargetAccount
		public static void UpdateTargetAccount(int AccountId, decimal TotalValue, bool PreserveParentValue)
		{
			if (!CanWork(AccountId))
				throw new AccessDeniedException();

			decimal TSub = 0;
			decimal TTotal = 0;
			using (IDataReader reader = DBFinance.GetAccount(AccountId))
			{
				reader.Read();
				TSub = (decimal)reader["TSub"];		// сумма дочерних 
				TTotal = (decimal)reader["TTotal"];		// старый общий итог
			}

			// Новый итог не может быть меньше суммы старых дочерних значений
			if (TotalValue < TSub)
				throw new WrongDataException();

			// Oleg Rylin [5/23/2006]
			// Проверка на то, чтобы не вылезти из родительского диапазона
			// Т.е. разница между тем, что было (TTotal) и тем, что стало (TotalValue), 
			// не должна быть больше TCur родителя (parentTCur)
			if (PreserveParentValue)
			{
				decimal parentTCur = 0;
				int ParentId = DBFinance.GetParentAccountId(AccountId);
				if (ParentId > 0)
				{
					using (IDataReader reader = DBFinance.GetAccount(ParentId))
					{
						reader.Read();
						parentTCur = (decimal)reader["TCur"];		// настолько можно изменить значение
					}
					if (TotalValue - TTotal > parentTCur)
						throw new WrongDataException();
				}
			}

			decimal TCur = TotalValue - TSub;

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBFinance.UpdateAccountTCur(AccountId, TCur, Security.CurrentUser.UserID);

				if (PreserveParentValue)
					RecalculateParentAccountWithPreserving(AccountId);
				else
					RecalculateParentAccounts(AccountId, false);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateEstimatedAccount
		public static void UpdateEstimatedAccount(int AccountId, decimal TotalValue, bool PreserveParentValue)
		{
			if (!CanWork(AccountId))
				throw new AccessDeniedException();

			decimal ESub = 0;
			decimal ETotal = 0;
			using (IDataReader reader = DBFinance.GetAccount(AccountId))
			{
				reader.Read();
				ESub = (decimal)reader["ESub"];		// сумма дочерних 
				ETotal = (decimal)reader["ETotal"];		// старый общий итог
			}

			// Новый итог не может быть меньше суммы старых дочерних значений
			if (TotalValue < ESub)
				throw new WrongDataException();

			// Oleg Rylin [5/23/2006]
			// Проверка на то, чтобы не вылезти из родительского диапазона
			// Т.е. разница между тем, что было (ETotal) и тем, что стало (TotalValue), 
			// не должна быть больше ECur родителя (parentECur)
			if (PreserveParentValue)
			{
				decimal parentECur = 0;
				int ParentId = DBFinance.GetParentAccountId(AccountId);
				if (ParentId > 0)
				{
					using (IDataReader reader = DBFinance.GetAccount(ParentId))
					{
						reader.Read();
						parentECur = (decimal)reader["ECur"];		// настолько можно изменить значение
					}
					if (TotalValue - ETotal > parentECur)
						throw new WrongDataException();
				}
			}

			decimal ECur = TotalValue - ESub;

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBFinance.UpdateAccountECur(AccountId, ECur, Security.CurrentUser.UserID);

				if (PreserveParentValue)
					RecalculateParentAccountWithPreserving(AccountId);
				else
					RecalculateParentAccounts(AccountId, false);

				tran.Commit();
			}
		}
		#endregion

		#region RenameAccount
		public static void RenameAccount(int AccountId, string Title)
		{
			if (!CanWork(AccountId))
				throw new AccessDeniedException();

			DBFinance.RenameAccount(AccountId, Title, Security.CurrentUser.UserID);
		}
		#endregion

		#region DeleteAccount
		public static void DeleteAccount(int AccountId)
		{
			if (!CanWork(AccountId))
				throw new AccessDeniedException();

			int ParentId = DBFinance.GetParentAccountId(AccountId);
			if (ParentId <= 0)
				throw new AccessDeniedException();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				// Улалим счёт и всех его детей
				DBFinance.DeleteAccount(AccountId);

				// Найдём всех сестёр
				ArrayList siblings = new ArrayList();
				using (IDataReader reader = DBFinance.GetListChildrenAccounts(ParentId))
				{
					while (reader.Read())
						siblings.Add((int)reader["AccountId"]);
				}

				if (siblings.Count == 0)
					DBFinance.UpdateIsSummary(ParentId, false);
				else
					ProcessRenumber(ParentId, siblings);

				// Пересчёт вверх по иерархии
				RecalculateParentAccounts(AccountId, false);

				tran.Commit();
			}
		}
		#endregion

		#region ProcessRenumber
		// Перенумерация OutlineNumber и OutlineLevel
		private static void ProcessRenumber(int AccountId, ArrayList children)
		{
			string OutlineNumber;
			int OutlineLevel;
			using (IDataReader reader = DBFinance.GetAccount(AccountId))
			{
				reader.Read();
				OutlineNumber = reader["OutlineNumber"].ToString();
				OutlineLevel = (int)reader["OutlineLevel"];
			}

			int pos = 0;
			foreach (int childId in children)
			{
				// Найдём всех внуков
				ArrayList grandChildren = new ArrayList();
				using (IDataReader reader = DBFinance.GetListChildrenAccounts(childId))
				{
					while (reader.Read())
						grandChildren.Add((int)reader["AccountId"]);
				}

				// Обновим OutlineNumber
				pos++;
				string newNumber = String.Format("{0}.{1}", OutlineNumber, pos);
				DBFinance.UpdateOutlineLevelAndNumber(childId, OutlineLevel + 1, newNumber);

				// Рекурсия
				if (grandChildren.Count > 0)
					ProcessRenumber(childId, grandChildren);
			}
		}
		#endregion
		
		#region AddActualFinances
		private static int AddActualFinances(int AccountId, int ObjectTypeId, int ObjectId, string Description, decimal Value)
		{
			if (!CanWork(ObjectTypeId, ObjectId))
				throw new AccessDeniedException();

			int retval = -1;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				retval = DBFinance.AddActualFinances(AccountId, ObjectTypeId, ObjectId, DateTime.UtcNow, Description, Value, Security.CurrentUser.UserID);

				RecalculateActualAccounts(AccountId);

				tran.Commit();
			}

			return retval;
		}

		public static int AddActualFinancesForProject(int AccountId, int ProjectId, string Description, decimal Value)
		{
			return AddActualFinances(AccountId, (int)ObjectTypes.Project, ProjectId, Description, Value);
		}

		public static int AddActualFinancesForTask(int AccountId, int TaskId, string Description, decimal Value)
		{
			return AddActualFinances(AccountId, (int)ObjectTypes.Task, TaskId, Description, Value);
		}
		
		public static int AddActualFinancesForToDo(int AccountId, int ToDoId, string Description, decimal Value)
		{
			return AddActualFinances(AccountId, (int)ObjectTypes.ToDo, ToDoId, Description, Value);
		}

		public static int AddActualFinancesForEvent(int AccountId, int EventId, string Description, decimal Value)
		{
			return AddActualFinances(AccountId, (int)ObjectTypes.CalendarEntry, EventId, Description, Value);
		}

		public static int AddActualFinancesForIncident(int AccountId, int IncidentId, string Description, decimal Value)
		{
			return AddActualFinances(AccountId, (int)ObjectTypes.Issue, IncidentId, Description, Value);
		}

		public static int AddActualFinancesForDocument(int AccountId, int DocumentId, string Description, decimal Value)
		{
			return AddActualFinances(AccountId, (int)ObjectTypes.Document, DocumentId, Description, Value);
		}
		#endregion

		#region UpdateActualFinancesValueAndDescription
		public static void UpdateActualFinancesValueAndDescription(int ActualId, 
			decimal Value, string Description)
		{
			int AccountId;
			int ObjectTypeId, ObjectId;
			using (IDataReader reader = DBFinance.GetActualFinances(ActualId))
			{
				reader.Read();
				AccountId = (int)reader["AccountId"];
				ObjectTypeId = (int)reader["ObjectTypeId"];
				ObjectId = (int)reader["ObjectId"];
			}

			if (!CanWork(ObjectTypeId, ObjectId))
				throw new AccessDeniedException();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBFinance.UpdateActualFinancesValueAndDescription(ActualId, Value, Description, Security.CurrentUser.UserID);

				RecalculateActualAccounts(AccountId);

				tran.Commit();
			}
		}
		#endregion

		#region DeleteActualFinances
		public static void DeleteActualFinances(int ActualId)
		{
			int AccountId;
			int ObjectTypeId, ObjectId;
			using (IDataReader reader = DBFinance.GetActualFinances(ActualId))
			{
				reader.Read();
				AccountId = (int)reader["AccountId"];
				ObjectTypeId = (int)reader["ObjectTypeId"];
				ObjectId = (int)reader["ObjectId"];
			}

			if (!CanWork(ObjectTypeId, ObjectId))
				throw new AccessDeniedException();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBFinance.DeleteActualFinances(ActualId);
				RecalculateActualAccounts(AccountId);

				tran.Commit();
			}
		}
		#endregion

		#region MoveAccount
		public static void MoveAccount(int AccountId, int NewParentId)
		{
			// Проверка на соответствие проектов:
			int oldProjectId = -1;
			int newProjectId = -1;
			using (IDataReader reader = DBFinance.GetAccount(AccountId))
			{
				reader.Read();
				oldProjectId = (int)reader["ProjectId"];
			}
			using (IDataReader reader = DBFinance.GetAccount(NewParentId))
			{
				reader.Read();
				newProjectId = (int)reader["ProjectId"];
			}
			if (oldProjectId != newProjectId)
				throw new AccessDeniedException();

			// Проверка на права
			if (!Project.CanEditFinances(newProjectId))
				throw new AccessDeniedException();

			// Проверка на дочерние элементы
			bool CanMove = false;
			using (IDataReader reader  = DBFinance.GetListAccountsForMove(AccountId))
			{
				while (reader.Read())
				{
					if ((int)reader["AccountId"] == NewParentId)
					{
						CanMove = true;
						break;
					}
				}
			}
			if (!CanMove)
				throw new AccessDeniedException();

			// Предварительная работа
			int oldParentId = DBFinance.GetParentAccountId(AccountId);

			// Найдём всех детей (для перенумерации)
			ArrayList children = new ArrayList();
			using (IDataReader reader = DBFinance.GetListChildrenAccounts(AccountId))
			{
				while (reader.Read())
					children.Add((int)reader["AccountId"]);
			}

			// Обработка
			using(DbTransaction tran = DbTransaction.Begin())
			{
				// Move
				DBFinance.MoveAccount(AccountId, NewParentId);

				// Найдём всех бывших сестёр (для перенумерации)
				ArrayList siblings = new ArrayList();
				using (IDataReader reader = DBFinance.GetListChildrenAccounts(oldParentId))
				{
					while (reader.Read())
						siblings.Add((int)reader["AccountId"]);
				}

				// Перенумеруем новую ветку
				ProcessRenumber(AccountId, children);

				// Обработаем старую ветку
				if (siblings.Count == 0)
					DBFinance.UpdateIsSummary(oldParentId, false);
				else
					ProcessRenumber(oldParentId, siblings);

				// Пересчитаем новую ветку
				RecalculateParentAccounts(AccountId, false);

				// Пересчитаем старую ветку
				RecalculateParentAccounts(oldParentId, true);

				tran.Commit();
			}
		}
		#endregion

		#region GetListActualFinancesByObject
		/// <summary>
		///		ActualId, AccountId, Title, OutlineLevel, ActualDate, Description, AValue, 
		///		LastEditorId, LastSavedDate
		/// </summary>
		private static DataTable GetListActualFinancesByObject(int ObjectTypeId, int ObjectId)
		{
			return DBFinance.GetListActualFinancesByObject(ObjectTypeId, ObjectId, Security.CurrentUser.TimeZoneId);
		}

		public static DataTable GetListActualFinancesByTask(int TaskId)
		{
			if (!Task.CanViewFinances(TaskId))
				throw new AccessDeniedException();

			return GetListActualFinancesByObject((int)ObjectTypes.Task, TaskId);
		}

		public static DataTable GetListActualFinancesByToDo(int ToDoId)
		{
			if (!ToDo.CanViewFinances(ToDoId))
				throw new AccessDeniedException();

			return GetListActualFinancesByObject((int)ObjectTypes.ToDo, ToDoId);
		}

		public static DataTable GetListActualFinancesByEvent(int EventId)
		{
			if (!CalendarEntry.CanViewFinances(EventId))
				throw new AccessDeniedException();

			return GetListActualFinancesByObject((int)ObjectTypes.CalendarEntry, EventId);
		}

		public static DataTable GetListActualFinancesByIncident(int IncidentId)
		{
			if (!Incident.CanViewFinances(IncidentId))
				throw new AccessDeniedException();

			return GetListActualFinancesByObject((int)ObjectTypes.Issue, IncidentId);
		}

		public static DataTable GetListActualFinancesByDocument(int DocumentId)
		{
			if (!Document.CanViewFinances(DocumentId))
				throw new AccessDeniedException();

			return GetListActualFinancesByObject((int)ObjectTypes.Document, DocumentId);
		}
		#endregion

		#region GetListActualFinancesByProject
		/// <summary>
		/// Reader returns fields:
		///		ActualId, AccountId, Title, OutlineLevel, ActualDate, Description, AValue, 
		///		LastEditorId, LastSavedDate, ObjectTypeId, ObjectId, ObjectTitle, IsTimeSheet
		/// </summary>
		public static DataTable GetListActualFinancesByProject(int ProjectId)
		{
			return GetListActualFinancesByProject(ProjectId, true);
		}

		public static DataTable GetListActualFinancesByProject(int ProjectId, bool checkAccess)
		{
			if (checkAccess && !Project.CanViewFinances(ProjectId))
				throw new AccessDeniedException();

			return DBFinance.GetListActualFinancesByProject(ProjectId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListTopLevelAccounts
		/// <summary>
		///		AccountId, OutlineLevel, Title, Target, Estimated, Actual
		/// </summary>
		public static IDataReader GetListTopLevelAccounts(int ProjectId)
		{
			return DBFinance.GetListTopLevelAccounts(ProjectId);
		}

		public static DataTable GetListTopLevelAccountsDataTable(int ProjectId)
		{
			return DBFinance.GetListTopLevelAccountsDataTable(ProjectId);
		}
		#endregion
	}
}
