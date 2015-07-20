using System;
using System.Data.SqlClient;

using Mediachase.IBN.Database;
using Mediachase.Ibn.Data.Sql;


namespace Mediachase.IBN.Business
{
	/// <summary>
	/// 
	/// </summary>
	public static class DatabaseTransactionBridge
	{
		private static bool _isInitialized = false;

		static SqlTransaction DataLevelTransaction
		{
			get { return DbContext.Current.Transaction; }
			set { DbContext.Current.Transaction = value; }
		}

		static SqlTransaction NewMetaDataTransaction
		{
			get { return SqlContext.Current.Transaction; }
			set { SqlContext.Current.Transaction = value; }
		}

		public static void Init()
		{
			if (!_isInitialized)
			{
				DbTransaction.Created += new EventHandler<DbTransactionEventArgs>(DbTransactionCreated);
				DbTransaction.Committed += new EventHandler<DbTransactionEventArgs>(DbTransactionCommitted);
				DbTransaction.RolledBack += new EventHandler<DbTransactionEventArgs>(DbTransactionRolledBack);

				SqlContext.TransactionCreated += new EventHandler(SqlContext_TransactionCreated);
				SqlContext.TransactionCommited += new EventHandler(SqlContext_TransactionCommited);
				SqlContext.TransactionRollbacked += new EventHandler(SqlContext_TransactionRollbacked);

				_isInitialized = true;
			}
		}

		public static void Uninit()
		{
			SqlContext.TransactionRollbacked -= new EventHandler(SqlContext_TransactionRollbacked);
			SqlContext.TransactionCommited -= new EventHandler(SqlContext_TransactionCommited);
			SqlContext.TransactionCreated -= new EventHandler(SqlContext_TransactionCreated);

			DbTransaction.RolledBack -= new EventHandler<DbTransactionEventArgs>(DbTransactionRolledBack);
			DbTransaction.Committed -= new EventHandler<DbTransactionEventArgs>(DbTransactionCommitted);
			DbTransaction.Created -= new EventHandler<DbTransactionEventArgs>(DbTransactionCreated);
		}

		static DatabaseTransactionBridge()
		{
		}

		#region Ibn 4.7 -> Mediachase.Ibn.Data

		static void DbTransactionCreated(object sender, DbTransactionEventArgs e)
		{
			if (e.SqlTransacted)
				NewMetaDataTransaction = DataLevelTransaction;
		}

		static void DbTransactionCommitted(object sender, DbTransactionEventArgs e)
		{
			if (e.SqlTransacted && e.Transaction != null)
			{
				NewMetaDataTransaction = DataLevelTransaction;
				Mediachase.Ibn.Data.Meta.Management.TriggerManager.CallTriggers(new Guid(DbContext.Current.TransactionId));
			}
		}

		static void DbTransactionRolledBack(object sender, DbTransactionEventArgs e)
		{
			if (e.SqlTransacted)
				NewMetaDataTransaction = DataLevelTransaction;
		}

		#endregion

		#region Mediachase.Ibn.Data -> Ibn 4.7

		static void SqlContext_TransactionCreated(object sender, EventArgs e)
		{
			DataLevelTransaction = NewMetaDataTransaction;
		}

		static void SqlContext_TransactionCommited(object sender, EventArgs e)
		{
			DataLevelTransaction = NewMetaDataTransaction;
			DbTransaction.OnExternalTransactionCommitted();
		}

		static void SqlContext_TransactionRollbacked(object sender, EventArgs e)
		{
			DataLevelTransaction = NewMetaDataTransaction;
		}

		#endregion
	}
}
