using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading;

namespace Mediachase.Ibn.WebTrial
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

		private const string sSQLDatabase = "SQLDatabase";

		public static string ConnectionString 
		{
			get 
			{
				return Settings.ConnectionString;
			} 
		}
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

		public static bool BeginTransaction(bool newTran)
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
			if(tran != null)
			{
				cmd.Connection = tran.Connection;
				cmd.Transaction = tran;
			}
			else
			{
				SqlConnection cnn = new SqlConnection(ConnectionString);
				cnn.Open();
				cmd.Connection = cnn;
			}
		}
		#endregion
	}
}
