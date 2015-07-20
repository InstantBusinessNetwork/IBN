using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.XPath;

namespace Mediachase.Database
{
	#region class DBHelper
	public class DBHelper
	{
		#region * Fields *
		
		private string _connectionString;
		private int _commandTimeout = 60;
		private string _database;
		private DBTransaction _dbTransaction;
		
		#endregion

		#region * Properties *

		#region ConnectionString
		public string ConnectionString
		{
			get 
			{
				return _connectionString;
			}
		}
		#endregion

		#region CommandTimeout
		public int CommandTimeout
		{
			get 
			{
				return _commandTimeout;
			} 
			set 
			{
				_commandTimeout = value;
			} 
		}
		#endregion

		#region Database
		public string Database
		{
			get 
			{
				return _database;
			} 
			set 
			{
				_database = value;
			} 
		}
		#endregion

		#endregion

		// Static
		#region MP() - Make Parameter
		public static SqlParameter MP(string name, SqlDbType type, int size, object value)
		{
			if(type == SqlDbType.VarChar || type == SqlDbType.NVarChar || type == SqlDbType.NChar)
			{
				if(value == null)
					value = DBNull.Value;
				else
				{
					string s = value.ToString();
					if(s.Length > size)
						value = s.Substring(0, size);
				}
			}

			SqlParameter prm = new SqlParameter(name, type, size);
			prm.Direction = ParameterDirection.Input;
			prm.Value = value;
			return prm;
		}

		public static SqlParameter MP(string name, SqlDbType type, object value)
		{
			if(type == SqlDbType.DateTime && value != DBNull.Value && (value == null || (DateTime)value == DateTime.MinValue))
				value = DBNull.Value;

			SqlParameter prm = new SqlParameter(name, type);
			prm.Direction = ParameterDirection.Input;
			prm.Value = value;
			return prm;
		}

		public static SqlParameter MP(string name, SqlDbType type, object value, bool translateHtmlChars)
		{
			if (value != null)
			{
				if (translateHtmlChars && (type == SqlDbType.Text || type == SqlDbType.NText))
					value = value.ToString().Replace("<", "&lt;").Replace(">", "&gt;");
			}

			SqlParameter prm = new SqlParameter(name, type);
			prm.Direction = ParameterDirection.Input;
			prm.Value = value;
			return prm;
		}

		public static SqlParameter MP(string name, SqlDbType type, int size, object value, bool translateHtmlChars)
		{
			if(value != null)
			{
				if (type == SqlDbType.VarChar || type == SqlDbType.NVarChar || type == SqlDbType.NChar || type == SqlDbType.Char)
				{
					if (translateHtmlChars)
						value = value.ToString().Replace("<", "&lt;").Replace(">", "&gt;");

					if (value.ToString().Length > size)
						value = value.ToString().Substring(0, size);
				}
			}

			SqlParameter prm = new SqlParameter(name, type, size);
			prm.Direction = ParameterDirection.Input;
			prm.Value = value;
			return prm;
		}
		#endregion
		#region MakeCommand()
		public static SqlCommand MakeCommand(CommandType type, string cmdText, params SqlParameter[] paramList)
		{
			SqlCommand cmd = new SqlCommand(cmdText);
			cmd.CommandType = type;

			if (paramList != null)
			{
				foreach (SqlParameter param in paramList)
					cmd.Parameters.Add(param);
			}

			return cmd;
		}
		#endregion


		public DBHelper(string connectionString)
		{
			_connectionString = connectionString;
		}

		#region BeginTransaction()
		public DBTransaction BeginTransaction()
		{
			SqlTransaction sqlTran = null;
			if(_dbTransaction == null)
			{
				SqlConnection con = new SqlConnection(ConnectionString);
				con.Open();

				if (_database != null && con.Database != _database)
					con.ChangeDatabase(_database);

				sqlTran = con.BeginTransaction();
			}
			DBTransaction ret = new DBTransaction(sqlTran, _dbTransaction);
			if(_dbTransaction == null)
			{
				_dbTransaction = ret;
				_dbTransaction.CommitCompletedEvent += new EventHandler(OnTransactionCompleted);
				_dbTransaction.RollbackCompletedEvent += new EventHandler(OnTransactionCompleted);
			}
			return ret;
		}
		#endregion

		#region OnTransactionCompleted()
		private void OnTransactionCompleted(object sender, EventArgs e)
		{
			_dbTransaction = null;
		}
		#endregion

		#region DemandTransaction()
		public void DemandTransaction()
		{
			if(null == _dbTransaction)
				throw new DBHelperException("This operation requires active transaction.");
		}
		#endregion

		#region SetupConnection()
		private void SetupConnection(SqlCommand cmd)
		{
			cmd.CommandTimeout = CommandTimeout;
			SqlConnection con;

			if(_dbTransaction != null)
			{
				con = _dbTransaction.SqlTran.Connection;

				if(_database != null && con.Database != _database)
					con.ChangeDatabase(_database);

				cmd.Connection = _dbTransaction.SqlTran.Connection;
				cmd.Transaction = _dbTransaction.SqlTran;
			}
			else
			{
				con = new SqlConnection(ConnectionString);
				con.Open();

				if(_database != null && con.Database != _database)
					con.ChangeDatabase(_database);

				cmd.Connection = con;
			}
		}
		#endregion


		// SQL Command
		#region RunCmd()
		public void RunCmd(SqlCommand cmd)
		{
			try
			{
				SetupConnection(cmd);
				cmd.ExecuteNonQuery();
			}
			finally
			{
				if(_dbTransaction == null && cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed)
					cmd.Connection.Close();
			}
		}
		#endregion

		#region RunCmdDataReader()
		public IDataReader RunCmdDataReader(SqlCommand cmd)
		{
			return RunCmdDataReader(cmd, CommandBehavior.Default);
		}

		public IDataReader RunCmdDataReader(SqlCommand cmd, CommandBehavior behavior)
		{
			SetupConnection(cmd);
			if(_dbTransaction == null)
				return cmd.ExecuteReader(behavior | CommandBehavior.CloseConnection);
			else
				return cmd.ExecuteReader(behavior);
		}
		#endregion

		#region RunCmdDataReaderBlob()
		public IDataReader RunCmdDataReaderBlob(SqlCommand cmd)
		{
			return RunCmdDataReader(cmd, CommandBehavior.SequentialAccess | CommandBehavior.SingleRow);
		}
		#endregion

		#region RunCmdDataTable()
		public DataTable RunCmdDataTable(SqlCommand cmd)
		{
			DataSet ds = new DataSet();
			ds.Locale = CultureInfo.InvariantCulture;
			try
			{
				SetupConnection(cmd);
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.Fill(ds);
			}
			finally
			{
				if(_dbTransaction == null && cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed)
					cmd.Connection.Close();
			}

			return ds.Tables[0];
		}
		#endregion

		#region RunCmdDateTime()
		public DateTime RunCmdDateTime(SqlCommand cmd)
		{
			DateTime ret = DateTime.MinValue;
			object o = RunCmdRetval(cmd, SqlDbType.DateTime);
			if(o != null)
				ret = (DateTime)o;
			return ret;
		}
		#endregion

		#region RunCmdInteger()
		public int RunCmdInteger(SqlCommand cmd)
		{
			int ret = -1;
			object o = RunCmdRetval(cmd, SqlDbType.Int);
			if(o != null)
				ret = (int)o;
			return ret;
		}
		#endregion

		#region RunCmdLong()
		public long RunCmdLong(SqlCommand cmd)
		{
			long ret = -1;
			object o = RunCmdRetval(cmd, SqlDbType.BigInt);
			if(o != null)
				ret = (long)o;
			return ret;
		}
		#endregion

		#region RunCmdRetval()
		public object RunCmdRetval(SqlCommand cmd, SqlDbType type)
		{
			if (cmd == null)
				throw new ArgumentNullException("cmd");

			object ret = null;
			try
			{
				if(cmd.Parameters.Contains("@retval"))
					cmd.Parameters.Remove(cmd.Parameters["@retval"]);
				cmd.Parameters.Add(new SqlParameter("@retval", type));
				cmd.Parameters["@retval"].Direction = ParameterDirection.Output;

				SetupConnection(cmd);
				cmd.ExecuteNonQuery();
			}
			finally
			{
				if(_dbTransaction == null && cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed)
					cmd.Connection.Close();
			}

			object o = cmd.Parameters["@retval"].Value;
			if(o != DBNull.Value)
				ret = o;

			return ret;
		}
		#endregion

		#region RunCmdScalar()
		public object RunCmdScalar(SqlCommand cmd)
		{
			object ret = null;
			try
			{
				SetupConnection(cmd);
				ret = cmd.ExecuteScalar();
			}
			finally
			{
				if(_dbTransaction == null && cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed)
					cmd.Connection.Close();
			}
			return ret;
		}
		#endregion

		#region RunCmdXmlDocument()
		public IXPathNavigable RunCmdXmlDocument(SqlCommand cmd)
		{
			XmlDocument ret;
			try
			{
				SetupConnection(cmd);
				XmlReader reader = cmd.ExecuteXmlReader();
				ret = new XmlDocument();
				ret.Load(reader);
				reader.Close();
			}
			finally
			{
				if(_dbTransaction == null && cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed)
					cmd.Connection.Close();
			}

			return ret;
		}
		#endregion


		// Stored Procedure
		#region RunSP()
		public void RunSP(string cmdText, params SqlParameter[] paramList)
		{
			RunCmd(MakeCommand(CommandType.StoredProcedure, cmdText, paramList));
		}

		public SqlCommand RunSP(SqlCommand cmd, string cmdText, params SqlParameter[] paramList)
		{
			DemandTransaction();

			if(null == cmd)
				cmd = MakeCommand(CommandType.StoredProcedure, cmdText, paramList);

			RunCmd(cmd);

			return cmd;
		}
		#endregion

		#region RunSPDataReader()
		public IDataReader RunSPDataReader(string cmdText, params SqlParameter[] paramList)
		{
			return RunCmdDataReader(MakeCommand(CommandType.StoredProcedure, cmdText, paramList));
		}
		#endregion

		#region RunSPDataReaderBlob()
		public IDataReader RunSPDataReaderBlob(string cmdText, params SqlParameter[] paramList)
		{
			return RunCmdDataReaderBlob(MakeCommand(CommandType.StoredProcedure, cmdText, paramList));
		}
		#endregion

		#region RunSPDataTable()
		public DataTable RunSPDataTable(string cmdText, params SqlParameter[] paramList)
		{
			return RunCmdDataTable(MakeCommand(CommandType.StoredProcedure, cmdText, paramList));
		}
		#endregion

		#region RunSPDateTime()
		public DateTime RunSPDateTime(string cmdText, params SqlParameter[] paramList)
		{
			return RunCmdDateTime(MakeCommand(CommandType.StoredProcedure, cmdText, paramList));
		}
		#endregion

		#region RunSPInteger()
		public int RunSPInteger(string cmdText, params SqlParameter[] paramList)
		{
			return RunCmdInteger(MakeCommand(CommandType.StoredProcedure, cmdText, paramList));
		}
		#endregion

		#region RunSPLong()
		public long RunSPLong(string cmdText, params SqlParameter[] paramList)
		{
			return RunCmdLong(MakeCommand(CommandType.StoredProcedure, cmdText, paramList));
		}
		#endregion

		#region RunSPScalar()
		public object RunSPScalar(string cmdText, params SqlParameter[] paramList)
		{
			return RunCmdScalar(MakeCommand(CommandType.StoredProcedure, cmdText, paramList));
		}
		#endregion


		// Command Text
		#region RunText()
		public void RunText(string cmdText, params SqlParameter[] paramList)
		{
			RunCmd(MakeCommand(CommandType.Text, cmdText, paramList));
		}
		#endregion

		#region RunTextDataReader()
		public IDataReader RunTextDataReader(string cmdText, params SqlParameter[] paramList)
		{
			return RunCmdDataReader(MakeCommand(CommandType.Text, cmdText, paramList));
		}
		#endregion

		#region RunTextDataReaderSchemaOnly()
		public IDataReader RunTextDataReaderSchemaOnly(string cmdText, params SqlParameter[] paramList)
		{
			return RunCmdDataReader(MakeCommand(CommandType.Text, cmdText, paramList), CommandBehavior.SchemaOnly);
		}
		#endregion

		#region RunTextDataReaderBlob()
		public IDataReader RunTextDataReaderBlob(string cmdText, params SqlParameter[] paramList)
		{
			return RunCmdDataReaderBlob(MakeCommand(CommandType.Text, cmdText, paramList));
		}
		#endregion

		#region RunTextInteger()
		public int RunTextInteger(string cmdText, params SqlParameter[] paramList)
		{
			return RunCmdInteger(MakeCommand(CommandType.Text, cmdText, paramList));
		}
		#endregion

		#region RunTextScalar()
		public object RunTextScalar(string cmdText, params SqlParameter[] paramList)
		{
			return RunCmdScalar(MakeCommand(CommandType.Text, cmdText, paramList));
		}
		#endregion

		#region RunTextXmlDocument()
		public IXPathNavigable RunTextXmlDocument(string cmdText, params SqlParameter[] paramList)
		{
			return RunCmdXmlDocument(MakeCommand(CommandType.Text, cmdText, paramList));
		}
		#endregion


		// SQL Script
		#region RunScript
		public void RunScript(TextReader script, NameValueCollection replace, bool catchSqlExceptions)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				try
				{
					cmd.CommandType = CommandType.Text;
					SetupConnection(cmd);

					RunScript(cmd, script, replace, catchSqlExceptions);
				}
				finally
				{
					if (_dbTransaction == null && cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed)
						cmd.Connection.Close();
				}
			}
		}

		public static void RunScript(DbCommand cmd, TextReader script, NameValueCollection replace, bool catchSqlExceptions)
		{
			if (cmd == null)
				throw new ArgumentNullException("cmd");
			if (script == null)
				throw new ArgumentNullException("script");

			StringBuilder sb = new StringBuilder();
			string line;
			bool eof, append;
			char[] crap = "\r\n\t ".ToCharArray();

			eof = false;
			sb = new StringBuilder();

			while(!eof)
			{
				append = false;
				while((line = script.ReadLine()) != null)
				{
					if(0 == string.Compare(line, "GO", true, CultureInfo.InvariantCulture))
						break;
					if(append || line.Length != 0)
					{
						append = true;
						sb.AppendFormat("{0}\r\n", line);
					}
				}
				if(replace != null)
				{
					foreach(string oldValue in replace.AllKeys)
						sb = sb.Replace(oldValue, replace[oldValue]);
				}
				cmd.CommandText = sb.ToString().Trim(crap);
				if(cmd.CommandText.Length != 0)
				{
					try
					{
						cmd.ExecuteNonQuery();
					}
					catch(SqlException)
					{
						if(!catchSqlExceptions)
							throw;
					}
				}
					
				sb.Length = 0;
				eof = (line == null);
			}
		}
		#endregion
	}
	#endregion

	#region class TransactionCompletedEventArgs
	public class TransactionCompletedEventArgs : EventArgs
	{
		private string m_tranId;
		
		public string TranId{ get{ return m_tranId; } }
		internal TransactionCompletedEventArgs(string tranId)
		{
			m_tranId = tranId;
		}
	}
	#endregion

	#region class DBTransaction
	public class DBTransaction : IDisposable
	{
		//Private
		private DBTransaction _rootTransaction;
		private SqlTransaction _sqlTransaction;
		private bool _commitCalled;
		private string _guid = (Guid.NewGuid().ToString());
		
		private event EventHandler _commitCompletedEvent;
		private event EventHandler _rollbackCompletedEvent;

		internal DBTransaction(SqlTransaction sqlTransaction, DBTransaction rootTransaction)
		{
			_sqlTransaction = sqlTransaction;
			_rootTransaction = rootTransaction;
		}

		private DBTransaction Root{ get{ return _rootTransaction != null ? _rootTransaction : this; } }

		// Public
		public SqlTransaction SqlTran
		{
			get
			{
				if (_rootTransaction != null)
					return _rootTransaction.SqlTran;
				else
					return _sqlTransaction;
			}
		}
		public string Id{ get{ return _guid; } }

		#region event CommitCompletedEvent
		public event EventHandler CommitCompletedEvent
		{
			add
			{
				Root._commitCompletedEvent += value;
			}
			remove
			{
				Root._commitCompletedEvent -= value;
			}
		}
		#endregion
		#region event RollbackCompletedEvent
		public event EventHandler RollbackCompletedEvent
		{
			add
			{
				Root._rollbackCompletedEvent += value;
			}
			remove
			{
				Root._rollbackCompletedEvent -= value;
			}
		}
		#endregion

		#region Commit()
		public void Commit()
		{
			_commitCalled = true;
			if(_sqlTransaction != null)
			{
				SqlConnection con = _sqlTransaction.Connection;
				try
				{
					_sqlTransaction.Commit();
				}
				catch
				{
					try
					{
						_sqlTransaction.Rollback();
					}
					catch (InvalidOperationException)
					{
					}
					throw;
				}
				finally
				{
					if(con != null)
						con.Close();
				}
				_sqlTransaction.Dispose();
				_sqlTransaction = null;

				if(_commitCompletedEvent != null)
					_commitCompletedEvent(this, new TransactionCompletedEventArgs(_guid));
			}
		}
		#endregion

		#region Rollback()
		public void Rollback()
		{
			if(_sqlTransaction != null)
			{
				SqlConnection con = _sqlTransaction.Connection;
				try
				{
					_sqlTransaction.Rollback();
				}
				catch (InvalidOperationException)
				{
				}

				if(con != null)
					con.Close();
				
				_sqlTransaction.Dispose();
				_sqlTransaction = null;

				if(_rollbackCompletedEvent != null)
					_rollbackCompletedEvent(this, new TransactionCompletedEventArgs(_guid));
			}
		}
		#endregion

		#region IDisposable Members
		public void Dispose()
		{
			Dispose(true);
		}
		#endregion

		protected virtual void Dispose(bool freeManagedResources)
		{
			if (freeManagedResources)
			{
				// free managed resources
				if (!_commitCalled)
					Rollback();
				if (_sqlTransaction != null)
					_sqlTransaction.Dispose();
			}

			// free native resources if there are any.
		}
	}
	#endregion

	#region class DBHelperException
	[Serializable]
	public class DBHelperException : Exception
	{
		public DBHelperException()
			: base()
		{
		}

		public DBHelperException(string message)
			: base(message)
		{
		}

		public DBHelperException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected DBHelperException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
	#endregion

	#region class DBHelper2
	public sealed class DBHelper2
	{
		[ThreadStatic]
		private static DBHelper _dbHelper;
		
		public static void Init(DBHelper dbHelper)
		{
			_dbHelper = dbHelper;
		}

		public static DBHelper DBHelper
		{
			get
			{
				return _dbHelper;
			}
		}

		private DBHelper2()
		{
		}
	}
	#endregion
}
