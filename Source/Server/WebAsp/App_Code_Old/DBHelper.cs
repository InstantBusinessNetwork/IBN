using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

using Mediachase.Ibn.Configuration;

namespace Mediachase.Ibn.WebAsp
{
	#region class AfterCommitArgs
	public class AfterCommitArgs : EventArgs
	{
		public int tran_id;
		public AfterCommitArgs(int tran_id)
		{
			this.tran_id = tran_id;
		}
	}
	#endregion

	/// <summary>
	/// Summary description for DBHelper.
	/// </summary>
	public class DBHelper
	{
		#region class McTransaction
		protected class McTransaction
		{
			SqlTransaction m_sqlTran;
			public event EventHandler evAfterCommit;

			public McTransaction(SqlTransaction sqlTran)
			{
				m_sqlTran = sqlTran;
			}

			public SqlTransaction SqlTransaction{ get{ return m_sqlTran; } }

			public void RaiseAfterCommit(int tran_id)
			{
				if(evAfterCommit != null)
				{
					evAfterCommit(this, new AfterCommitArgs(tran_id));
					evAfterCommit = null;
				}
			}
		}
		#endregion

		#region ConnectionString, Database
		private const string sSQLDatabase = "IBN" + Mediachase.Ibn.IbnConst.VersionMajorMinor + "_ASP_SQLDatabase";

		#region ConnectionString
		public static string ConnectionString 
		{
			get
			{
				return ConfigurationManager.AppSettings["ConnectionString"];
			}
		}
		#endregion

		#region AspDatabase
		private static string AspDatabase
		{
			get 
			{
				string retval = string.Empty;
				string[] blocks = ConnectionString.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string block in blocks)
				{
					if (block.IndexOf("=") > 0)
					{
						string[] pair = block.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
						if (pair.Length == 2 && pair[0].Trim().ToLowerInvariant() == ("initial catalog"))
						{
							retval = pair[1].Trim();
							break;
						}
					}
				}

				return retval;
			}
		}
		#endregion

		#region GetDatabase
		private static string GetDatabase()
		{
			string db = (string)Thread.GetData(Thread.GetNamedDataSlot(sSQLDatabase));
			if (db == null)
			{
				db = AspDatabase;
				SetDatabase(db);
			}
			return db;
		}
		#endregion

		#region SetDatabase
		private static void SetDatabase(string DatabaseName)
		{
			Thread.SetData(Thread.GetNamedDataSlot(sSQLDatabase), DatabaseName);
		}
		#endregion

		#region SwitchToPortalDatabase, SwitchToAspDatabase
		/// Sample of Using:
		/// DBHelper.SwitchToPortalDatabase(PortalDatabase);
		/// try
		/// {
		///   [return] DBHelper.RunSomeMethod();
		/// }
		/// finally
		/// {
		///		DBHelper.SwitchToAspDatabase();
		/// }
		/// 
		public static void SwitchToPortalDatabase(string PortalDatabase)
		{
			SetDatabase(PortalDatabase);
		}

		public static void SwitchToAspDatabase()
		{
			SetDatabase(AspDatabase);
		}
		#endregion
		#endregion

		#region RunSP
		public static IDataReader RunSPReturnDataReader(string strSP, params object[] paramList)
		{
			SqlCommand cmd = new SqlCommand(strSP);
			cmd.CommandType = CommandType.StoredProcedure;

			foreach(SqlParameter param in paramList)
				cmd.Parameters.Add(param);

			SqlTransaction tran;
			SetupConnection(cmd, out tran);

			return (tran != null) ? cmd.ExecuteReader() : cmd.ExecuteReader(CommandBehavior.CloseConnection);
		}


		public static DataTable RunSPReturnDataTable(string strSP, params object[] paramList)
		{
			SqlTransaction tran = null;
			DataSet ds = new DataSet();

			SqlCommand cmd = new SqlCommand(strSP);
			cmd.CommandType = CommandType.StoredProcedure;

			foreach (SqlParameter param in paramList)
				cmd.Parameters.Add(param);

			try
			{
				SetupConnection(cmd, out tran);

				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.Fill(ds);
			}
			finally
			{
				if (tran == null && cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed)
					cmd.Connection.Close();
			}

			return ds.Tables[0];
		}

		public static IDataReader RunSPReturnDataReaderBLOB(string strSP, params object[] paramList)
		{
			SqlCommand cmd = new SqlCommand(strSP);
			cmd.CommandType = CommandType.StoredProcedure;

			foreach(SqlParameter param in paramList)
				cmd.Parameters.Add(param);

			SqlTransaction tran;
			SetupConnection(cmd, out tran);

			return (tran != null) ? cmd.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleRow) : cmd.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleRow);
		}


		public static int RunSPReturnInteger(string strSP, params object[] paramList)
		{
			int retVal;
			SqlTransaction tran = null;

			SqlCommand cmd = new SqlCommand(strSP);
			cmd.CommandType = CommandType.StoredProcedure;

			foreach (SqlParameter param in paramList)
				cmd.Parameters.Add(param);

			cmd.Parameters.Add(new SqlParameter("@retval", SqlDbType.Int, 4));
			cmd.Parameters["@retval"].Direction = ParameterDirection.Output;

			try
			{
				SetupConnection(cmd, out tran);
				cmd.ExecuteNonQuery();
			}
			finally
			{
				if (tran == null && cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed)
					cmd.Connection.Close();
			}

			if (cmd.Parameters["@retval"].Value != DBNull.Value)
				retVal = (int)cmd.Parameters["@retval"].Value;
			else
				retVal = -1;

			return retVal;
		}

		public static object RunSPReturnObject(string strSP, params object[] paramList)
		{
			object retVal = null;
			SqlTransaction tran = null;

			SqlCommand cmd = new SqlCommand(strSP);
			cmd.CommandType = CommandType.StoredProcedure;

			foreach (SqlParameter param in paramList)
				cmd.Parameters.Add(param);

			try
			{
				SetupConnection(cmd, out tran);
				retVal = cmd.ExecuteScalar();
			}
			finally
			{
				if (tran == null && cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed)
					cmd.Connection.Close();
			}

			return retVal;
		}

		public static DateTime RunSPReturnDateTime(string strSP, params object[] paramList)
		{
			DateTime retVal;
			SqlTransaction tran = null;

			SqlCommand cmd = new SqlCommand(strSP);
			cmd.CommandType = CommandType.StoredProcedure;

			foreach (SqlParameter param in paramList)
				cmd.Parameters.Add(param);

			cmd.Parameters.Add(new SqlParameter("@retval", SqlDbType.DateTime));
			cmd.Parameters["@retval"].Direction = ParameterDirection.Output;

			try
			{
				SetupConnection(cmd, out tran);
				cmd.ExecuteNonQuery();
			}
			finally
			{
				if (tran == null && cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed)
					cmd.Connection.Close();
			}
			
			if (cmd.Parameters["@retval"].Value != DBNull.Value)
				retVal = (DateTime)cmd.Parameters["@retval"].Value;
			else
				retVal = DateTime.MinValue;

			return retVal;
		}

		public static void RunSP(string strSP, params object[] paramList)
		{
			SqlTransaction tran = null;

			SqlCommand cmd = new SqlCommand(strSP);
			cmd.CommandType = CommandType.StoredProcedure;

			foreach (SqlParameter param in paramList)
				cmd.Parameters.Add(param);

			try
			{
				SetupConnection(cmd, out tran);
				cmd.ExecuteNonQuery();
			}
			finally
			{
				if (tran == null && cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed)
					cmd.Connection.Close();
			}
		}

		public static SqlCommand RunSP(SqlCommand cmd, string strSP, params object[] paramList)
		{
			if(null == cmd)
			{
				cmd = new SqlCommand(strSP);
				cmd.CommandType = CommandType.StoredProcedure;

				foreach(SqlParameter param in paramList)
					cmd.Parameters.Add(param);
			}

			DemandTransaction();
			SqlTransaction tran;
			
			SetupConnection(cmd, out tran);

			cmd.ExecuteNonQuery();

			return cmd;
		}


		public static IDataReader RunSQLReturnDataReader(string sql)
		{
			SqlCommand cmd = new SqlCommand();
			cmd.CommandType = CommandType.Text;
			cmd.CommandText=sql;;

			SqlTransaction tran;
			SetupConnection(cmd, out tran);

			return (tran != null) ? cmd.ExecuteReader() : cmd.ExecuteReader(CommandBehavior.CloseConnection);
		}
		#endregion

		#region Transactions
		private const string sSQLTransaction = "SQLTransaction";

		/// <summary>
		/// Always creates new transaction.
		/// </summary>
		public static bool BeginTransaction()
		{
			return BeginTransaction(false);
		}

		private static bool BeginTransaction(bool newTran)
		{
			McTransaction tran = null;
			if(!newTran)
				tran = GetMcTransaction();

			if(null == tran)
			{
				newTran = true;
				SqlConnection cnn = new SqlConnection(ConnectionString);
				cnn.Open();
				tran = new McTransaction(cnn.BeginTransaction());
				SetTransaction(tran);
			}
			return newTran;
		}

		public static void Commit(bool newTran)
		{
			if(newTran)
			{
				McTransaction tran = GetMcTransaction();
				if(tran != null)
				{
					SqlConnection cnn = null;
					if (tran.SqlTransaction.Connection != null)
						cnn = tran.SqlTransaction.Connection;

					try
					{
						tran.SqlTransaction.Commit();
					}
					catch
					{
						try
						{
							tran.SqlTransaction.Rollback();
						}
						catch{}
						throw;
					}
					finally
					{
						if(cnn != null)
							cnn.Close();
					}
				}
				SetTransaction(null);
				tran.RaiseAfterCommit(tran.SqlTransaction.GetHashCode());
			}
		}

		public static void Rollback(bool newTran)
		{
			if(newTran)
			{
				McTransaction tran = GetMcTransaction();
				if(tran != null)
				{
					SqlConnection cnn = null;
					if (tran.SqlTransaction.Connection != null)
						cnn = tran.SqlTransaction.Connection;

					try 
					{
						tran.SqlTransaction.Rollback();
					}
					catch{}
					
					if(cnn != null)
						cnn.Close();
				}
				SetTransaction(null);
			}
		}

		private static McTransaction GetMcTransaction()
		{
			return (McTransaction)Thread.GetData(Thread.GetNamedDataSlot(sSQLTransaction));
		}

		private static SqlTransaction GetTransaction()
		{
			SqlTransaction ret = null;
			McTransaction tran = GetMcTransaction();
			if(tran != null)
				ret = tran.SqlTransaction;
			return ret;
		}

		private static void SetTransaction(McTransaction tran)
		{
			Thread.SetData(Thread.GetNamedDataSlot(sSQLTransaction), tran);
		}

		public static void DemandTransaction()
		{
			if(null == GetTransaction())
				throw new Exception("This operation requires active transaction.");
		}

		public static int GetTransactionHashCode()
		{
			int ret = -1;
			SqlTransaction tran = GetTransaction();
			if(tran != null)
				ret = tran.GetHashCode();
			return ret;
		}
		#endregion

		#region mp - Make Parameter
		public static SqlParameter mp(string PName, SqlDbType PType, int PSize, object PValue)
		{
			if (PType == SqlDbType.VarChar || PType == SqlDbType.NVarChar || PType == SqlDbType.NChar)
				if (PValue != null && PValue.ToString().Length > PSize)
					PValue = PValue.ToString().Substring(0, PSize);

			SqlParameter prm = new SqlParameter(PName, PType, PSize);
			prm.Direction = ParameterDirection.Input;
			prm.Value = PValue;
			return prm;
		}

		public static SqlParameter mp(string PName, SqlDbType PType, object PValue)
		{
			SqlParameter prm = new SqlParameter(PName, PType);
			prm.Direction = ParameterDirection.Input;
			prm.Value = PValue;
			return prm;
		}
		#endregion

		#region AddOnCommited
		public static void AddOnCommited(EventHandler handler)
		{
			DemandTransaction();
			GetMcTransaction().evAfterCommit += handler;
		}
		#endregion
		
		#region ExecuteScalar
		public static object ExecuteScalar(SqlCommand cmd)
		{
			cmd.Connection = new SqlConnection(ConnectionString);
			try
			{
				cmd.Connection.Open();
				return cmd.ExecuteScalar();
			}
			finally
			{
				cmd.Connection.Close();
			}
		}
		#endregion

		#region SetupConnection
		private static void SetupConnection(SqlCommand cmd, out SqlTransaction tran)
		{
			tran = GetTransaction();
			string currentDB = GetDatabase();
			if(tran != null)
			{
				if(tran.Connection.Database != currentDB)
					tran.Connection.ChangeDatabase(currentDB);
				cmd.Connection = tran.Connection;
				cmd.Transaction = tran;
			}
			else
			{
				SqlConnection cnn = new SqlConnection(ConnectionString);
				cnn.Open();
				if(cnn.Database != currentDB)
					cnn.ChangeDatabase(currentDB);
				cmd.Connection = cnn;
			}
		}
		#endregion
	}
}
