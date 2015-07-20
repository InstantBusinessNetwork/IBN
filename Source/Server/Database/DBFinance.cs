using System;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database
{
	public class DBFinance
	{
		#region CheckForUnchangeableRoles
		public static int CheckForUnchangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("FinancesCheckForUnchangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ReplaceUnchangeableUser
		public static void ReplaceUnchangeableUser(int FromUserId, int ToUserId)
		{
			DbHelper2.RunSp("FinancesReplaceUnchangeableUser", 
				DbHelper2.mp("@FromUserId", SqlDbType.Int, FromUserId),
				DbHelper2.mp("@ToUserId", SqlDbType.Int, ToUserId));
		}
		#endregion

		#region ReplaceUnchangeableUserToManager
		public static void ReplaceUnchangeableUserToManager(int UserId)
		{
			DbHelper2.RunSp("FinancesReplaceUnchangeableUserToManager", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region CreateRootAccount
		public static int CreateRootAccount(int ProjectId, string Title, int LastEditorId)
		{
			return DbHelper2.RunSpInteger("AccountCreateRoot",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, Title),
				DbHelper2.mp("@LastEditorId", SqlDbType.Int, LastEditorId));
		}
		#endregion

		#region GetListAccounts
		/// <summary>
		/// Reader returns fields:
		///		AccountId, ProjectId, Title, OutlineLevel, OutlineNumber, IsSummary
		/// </summary>
		public static IDataReader GetListAccounts(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("AccountGetList", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}

		public static DataTable GetListAccountsDataTable(int ProjectId)
		{
			return DbHelper2.RunSpDataTable("AccountGetList", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region AddActualFinances
		public static int AddActualFinances(int AccountId, int ObjectTypeId, int ObjectId,
			DateTime ActualDate, string Description, decimal AValue, int LastEditorId)
		{
			return DbHelper2.RunSpInteger("ActualFinancesAdd",
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId),
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@ActualDate", SqlDbType.DateTime, ActualDate),
				DbHelper2.mp("@Description", SqlDbType.NVarChar, 255, Description),
				DbHelper2.mp("@AValue", SqlDbType.Money, AValue),
				DbHelper2.mp("@LastEditorId", SqlDbType.Int, LastEditorId));
		}
		#endregion

		#region GetActualFinances
		/// <summary>
		/// Reader returns fields:
		///		ActualId, AccountId, Description, AValue, LastEditorId, ObjectTypeId, ObjectId,
		///		IsTimeSheet
		/// </summary>
		public static IDataReader GetActualFinances(int ActualId)
		{
			return DbHelper2.RunSpDataReader("ActualFinancesGet", 
				DbHelper2.mp("@ActualId", SqlDbType.Int, ActualId));
		}
		#endregion

		#region RecalculateACurAccount
		public static void RecalculateACurAccount(int AccountId, int LastEditorId)
		{
			DbHelper2.RunSp("AccountRecalculateACur", 
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId),
				DbHelper2.mp("@LastEditorId", SqlDbType.Int, LastEditorId));
		}
		#endregion

		#region RecalculateParentAccount
		public static void RecalculateParentAccount(int AccountId, int LastEditorId)
		{
			DbHelper2.RunSp("AccountRecalculateSub", 
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId),
				DbHelper2.mp("@LastEditorId", SqlDbType.Int, LastEditorId));
		}
		#endregion

		#region RecalculateParentAccountWithPreserving
		public static void RecalculateParentAccountWithPreserving(int AccountId, int LastEditorId)
		{
			DbHelper2.RunSp("AccountRecalculateSubWithPreserving", 
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId),
				DbHelper2.mp("@LastEditorId", SqlDbType.Int, LastEditorId));
		}
		#endregion

		#region GetListParentAccounts
		/// <summary>
		/// Reader returns fields:
		///		AccountId, ProjectId, Title, OutlineLevel, OutlineNumber, IsSummary, 
		///		TCur, TSub, ECur, ESub, ACur, ASub
		/// </summary>
		public static IDataReader GetListParentAccounts(int AccountId)
		{
			return DbHelper2.RunSpDataReader("AccountGetListParents", 
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId));
		}
		#endregion

		#region AddCollapsedAccount
		public static void AddCollapsedAccount(int UserId, int AccountId)
		{
			DbHelper2.RunSp("CollapsedAccountAdd",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId));
		}
		#endregion

		#region DeleteCollapsedAccount
		public static void DeleteCollapsedAccount(int UserId, int AccountId)
		{
			DbHelper2.RunSp("CollapsedAccountDelete",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId));
		}
		#endregion

		#region GetListAccountsByProjectCollapsed
		/// <summary>
		/// AccountId, ProjectId, Title, OutlineLevel, OutlineNumber, IsSummary, IsCollapsed,
		/// TTotal, TCur, TSub,
		/// ETotal, ECur, ESub,
		/// ATotal, ACur, ASub,
		/// TParent, EParent
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListAccountsByProjectCollapsed(int ProjectId, int UserId)
		{
			return DbHelper2.RunSpDataTable("AccountsGetByProjectCollapsed",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region AddChildAccount
		public static int AddChildAccount(int ParentId, string Title, int LastEditorId)
		{
			return DbHelper2.RunSpInteger("AccountAddChild",
				DbHelper2.mp("@ParentId", SqlDbType.Int, ParentId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, Title),
				DbHelper2.mp("@LastEditorId", SqlDbType.Int, LastEditorId));
		}
		#endregion

		#region GetAccount
		/// <summary>
		/// Reader returns fields:
		///		AccountId, ProjectId, Title, OutlineLevel, OutlineNumber, IsSummary,
		///		TTotal, TCur, TSub, 
		///		ETotal, ECur, ESub, 
		///		ATotal, ACur, ASub
		/// </summary>
		public static IDataReader GetAccount(int AccountId)
		{
			return DbHelper2.RunSpDataReader("AccountGet", 
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId));
		}
		#endregion

		#region GetListAccountsForMove
		/// <summary>
		/// Reader returns fields:
		///		AccountId, Title, OutlineLevel, OutlineNumber, IsSummary
		/// </summary>
		public static IDataReader GetListAccountsForMove(int AccountId)
		{
			return DbHelper2.RunSpDataReader("AccountGetListForMove", 
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId));
		}

		public static DataTable GetListAccountsForMoveDataTable(int AccountId)
		{
			return DbHelper2.RunSpDataTable("AccountGetListForMove", 
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId));
		}
		#endregion

		#region GetListActualFinancesByProject
		/// <summary>
		///		ActualId, AccountId, Title, OutlineLevel, ActualDate, Description, AValue, 
		///		LastEditorId, LastSavedDate, ObjectTypeId, ObjectId, ObjectTitle
		/// </summary>
		public static DataTable GetListActualFinancesByProject(int ProjectId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"ActualDate", "LastSavedDate"},
				"ActualFinancesGetListByProject", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region UpdateActualFinancesValue
		public static void UpdateActualFinancesValue(int ActualId, decimal AValue, int LastEditorId)
		{
			DbHelper2.RunSp("ActualFinancesUpdateValue",
				DbHelper2.mp("@ActualId", SqlDbType.Int, ActualId),
				DbHelper2.mp("@AValue", SqlDbType.Money, AValue),
				DbHelper2.mp("@LastEditorId", SqlDbType.Int, LastEditorId));
		}
		#endregion

		#region UpdateAccountTCur
		public static void UpdateAccountTCur(int AccountId, decimal Value, int LastEditorId)
		{
			DbHelper2.RunSp("AccountUpdateTCur",
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId),
				DbHelper2.mp("@Value", SqlDbType.Money, Value),
				DbHelper2.mp("@LastEditorId", SqlDbType.Int, LastEditorId));
		}
		#endregion

		#region UpdateAccountECur
		public static void UpdateAccountECur(int AccountId, decimal Value, int LastEditorId)
		{
			DbHelper2.RunSp("AccountUpdateECur",
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId),
				DbHelper2.mp("@Value", SqlDbType.Money, Value),
				DbHelper2.mp("@LastEditorId", SqlDbType.Int, LastEditorId));
		}
		#endregion

		#region GetParentAccountId
		public static int GetParentAccountId(int AccountId)
		{
			return DbHelper2.RunSpInteger("AccountGetParentId",
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId));
		}
		#endregion

		#region RenameAccount
		public static void RenameAccount(int AccountId, string Title, int LastEditorId)
		{
			DbHelper2.RunSp("AccountRename",
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, Title),
				DbHelper2.mp("@LastEditorId", SqlDbType.Int, LastEditorId));
		}
		#endregion

		#region DeleteAccount
		public static void DeleteAccount(int AccountId)
		{
			DbHelper2.RunSp("AccountDelete",
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId));
		}
		#endregion

		#region GetListChildrenAccounts
		/// <summary>
		/// Reader returns fields:
		///		AccountId, Title, OutlineNumber
		/// </summary>
		public static IDataReader GetListChildrenAccounts(int AccountId)
		{
			return DbHelper2.RunSpDataReader("AccountGetListChildren", 
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId));
		}
		#endregion

		#region UpdateOutlineLevelAndNumber
		public static void UpdateOutlineLevelAndNumber(int AccountId, int OutlineLevel, string OutlineNumber)
		{
			DbHelper2.RunSp("AccountUpdateOutlineLevelAndNumber",
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId),
				DbHelper2.mp("@OutlineLevel", SqlDbType.Int, OutlineLevel),
				DbHelper2.mp("@OutlineNumber", SqlDbType.VarChar, 255, OutlineNumber));
		}
		#endregion

		#region UpdateIsSummary
		public static void UpdateIsSummary(int AccountId, bool IsSummary)
		{
			DbHelper2.RunSp("AccountUpdateIsSummary",
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId),
				DbHelper2.mp("@IsSummary", SqlDbType.Bit, IsSummary));
		}
		#endregion

		#region UpdateActualFinancesValueAndDescription
		public static void UpdateActualFinancesValueAndDescription(int ActualId, 
			decimal AValue, string Description, int LastEditorId)
		{
			DbHelper2.RunSp("ActualFinancesUpdateValueAndDescription",
				DbHelper2.mp("@ActualId", SqlDbType.Int, ActualId),
				DbHelper2.mp("@AValue", SqlDbType.Money, AValue),
				DbHelper2.mp("@Description", SqlDbType.NVarChar, 255, Description),
				DbHelper2.mp("@LastEditorId", SqlDbType.Int, LastEditorId));
		}
		#endregion

		#region DeleteActualFinances
		public static void DeleteActualFinances(int ActualId)
		{
			DbHelper2.RunSp("ActualFinancesDelete", 
				DbHelper2.mp("@ActualId", SqlDbType.Int, ActualId));
		}
		#endregion

		#region MoveAccount
		public static void MoveAccount(int AccountId, int ParentId)
		{
			DbHelper2.RunSp("AccountMove", 
				DbHelper2.mp("@AccountId", SqlDbType.Int, AccountId),
				DbHelper2.mp("@ParentId", SqlDbType.Int, ParentId));
		}
		#endregion

		#region GetListActualFinancesByObject
		/// <summary>
		///		ActualId, AccountId, Title, OutlineLevel, ActualDate, Description, AValue, 
		///		LastEditorId, LastSavedDate
		/// </summary>
		public static DataTable GetListActualFinancesByObject(int ObjectTypeId, int ObjectId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"ActualDate", "LastSavedDate"},
				"ActualFinancesGetListByObject", 
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion

		#region GetListTopLevelAccounts
		/// <summary>
		///		AccountId, OutlineLevel, Title, Target, Estimated, Actual
		/// </summary>
		public static IDataReader GetListTopLevelAccounts(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("AccountGetListTopLevel", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}

		public static DataTable GetListTopLevelAccountsDataTable(int ProjectId)
		{
			return DbHelper2.RunSpDataTable("AccountGetListTopLevel", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion
	}
}
