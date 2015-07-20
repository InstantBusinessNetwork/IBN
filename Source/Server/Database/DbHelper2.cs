using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;

using Mediachase.Ibn;
using Mediachase.Ibn.Data;


namespace Mediachase.IBN.Database
{
	#region class DbHelper2
	public class DbHelper2
	{
		private const string _initialCatalog = "initial catalog=";
		private const string _database = "database=";
		private const string _retval = "@retval";

		#region Init*()
		public static void Init()
		{
			int iBegin, iEnd;
			Init(out iBegin, out iEnd);
		}

		public static void Init2()
		{
			int iBegin, iEnd;
			Init(out iBegin, out iEnd);

			string portalConnectionString = ConnectionString;

			DbContext.Current.PortalConnectionString = portalConnectionString;
			Mediachase.MetaDataPlus.MetaDataContext.Current.ConnectionString = portalConnectionString;
			IbnWebSaltatorySqlSettingsStorage.Init(portalConnectionString);
			DataContext.Current = new DataContext(portalConnectionString);
		}

		private static void Init(out int valueStart, out int valueEnd)
		{
			string connectionString = ConnectionString;

			string database = _initialCatalog;
			valueStart = connectionString.IndexOf(database, StringComparison.OrdinalIgnoreCase);
			if (valueStart < 0)
			{
				database = _database;
				valueStart = connectionString.IndexOf(database, StringComparison.OrdinalIgnoreCase);
			}
			if (valueStart < 0)
				throw new Exception("Invalid connection string.");
			valueStart += database.Length;

			valueEnd = connectionString.IndexOf(";", valueStart, StringComparison.OrdinalIgnoreCase);
			if (valueEnd < 0)
				valueEnd = connectionString.Length;

			DbContext.Current.Init(connectionString.Substring(valueStart, valueEnd - valueStart));
		}
		#endregion

		#region ConnectionString, Database
		public static string ConnectionString
		{
			get
			{
				return ConfigurationManager.AppSettings["ConnectionString"];
			}
		}
		#endregion


		#region mp - Make Parameter
		public static SqlParameter mp(string PName, SqlDbType PType, int PSize, object PValue)
		{
			if (PType == SqlDbType.VarChar || PType == SqlDbType.NVarChar || PType == SqlDbType.NChar)
			{
				if (PValue == null)
					PValue = DBNull.Value;
				else
				{
					string s = PValue.ToString();
					if (s.Length > PSize)
						PValue = s.Substring(0, PSize);
				}
			}

			// 2006-11-22 OZ Addon
			if (PValue == null)
				PValue = DBNull.Value;

			SqlParameter prm = new SqlParameter(PName, PType, PSize);
			prm.Direction = ParameterDirection.Input;
			prm.Value = PValue;
			return prm;
		}

		public static SqlParameter mp(string PName, SqlDbType PType, object PValue)
		{
			if (PType == SqlDbType.DateTime && PValue != DBNull.Value && (PValue == null || (DateTime)PValue == DateTime.MinValue))
				PValue = DBNull.Value;

			// 2006-11-22 OZ Addon
			if (PValue == null)
				PValue = DBNull.Value;

			SqlParameter prm = new SqlParameter(PName, PType);
			prm.Direction = ParameterDirection.Input;
			prm.Value = PValue;
			return prm;
		}

		public static SqlParameter mp(string PName, SqlDbType PType, object PValue, bool TranslateHtmlChars)
		{
			if (PValue != null && TranslateHtmlChars && (PType == SqlDbType.Text || PType == SqlDbType.NText))
				PValue = PValue.ToString().Replace("<", "&lt;").Replace(">", "&gt;");

			SqlParameter prm = new SqlParameter(PName, PType);
			prm.Direction = ParameterDirection.Input;
			prm.Value = PValue;
			return prm;
		}

		public static SqlParameter mp(string PName, SqlDbType PType, int PSize, object PValue, bool TranslateHtmlChars)
		{
			if (PValue != null && (PType == SqlDbType.VarChar || PType == SqlDbType.NVarChar || PType == SqlDbType.NChar || PType == SqlDbType.Char))
			{
				if (TranslateHtmlChars)
					PValue = PValue.ToString().Replace("<", "&lt;").Replace(">", "&gt;");

				if (PValue.ToString().Length > PSize)
					PValue = PValue.ToString().Substring(0, PSize);
			}

			SqlParameter prm = new SqlParameter(PName, PType, PSize);
			prm.Direction = ParameterDirection.Input;
			prm.Value = PValue;
			return prm;
		}
		#endregion

		#region MakeCommand()
		public static SqlCommand MakeCommand(CommandType type, string cmdText, params object[] paramList)
		{
			SqlCommand cmd = new SqlCommand(cmdText);
			cmd.CommandType = type;

			foreach (SqlParameter param in paramList)
				cmd.Parameters.Add(param);

			return cmd;
		}
		#endregion

		#region SetupConnection()
		private static void SetupConnection(SqlCommand cmd)
		{
			if (cmd == null)
				throw new ArgumentNullException("cmd");

			DbContext context = DbContext.Current;
			cmd.CommandTimeout = context.CommandTimeout;
			string database = context.Database;

			SqlTransaction sqlTran = context.Transaction;

			SqlConnection con;
			if (sqlTran != null && sqlTran.Connection != null)
			{
				con = sqlTran.Connection;
				cmd.Transaction = sqlTran;
			}
			else
			{
				con = new SqlConnection(ConnectionString);
				con.Open();
			}

			if (database != null && con.Database != database)
				con.ChangeDatabase(database);

			cmd.Connection = con;
		}
		#endregion

		private static void CloseConnection(SqlCommand cmd)
		{
			if (cmd != null)
			{
				if (
					DbContext.Current.Transaction == null
					&& cmd.Connection != null
					&& cmd.Connection.State != ConnectionState.Closed
					)
					cmd.Connection.Close();
			}
		}

		#region ProcessDateTimeValue()
		protected static object ProcessDateTimeValue(int timezoneId, object value)
		{
			if (value is DateTime)
			{
				McTimeZone tz = McTimeZone.Load(timezoneId);
				return tz.GetLocalDate((DateTime)value);
			}
			return value;
		}
		#endregion


		// SQL Command
		#region RunCmd()
		public static void RunCmd(SqlCommand cmd)
		{
			try
			{
				SetupConnection(cmd);
				cmd.ExecuteNonQuery();
			}
			finally
			{
				CloseConnection(cmd);
			}
		}
		#endregion

		#region RunCmdDataReader()
		public static IDataReader RunCmdDataReader(SqlCommand cmd)
		{
			return RunCmdDataReader(cmd, CommandBehavior.Default);
		}

		public static IDataReader RunCmdDataReader(SqlCommand cmd, CommandBehavior behavior)
		{
			SetupConnection(cmd);
			if (DbContext.Current.Transaction == null)
				return cmd.ExecuteReader(behavior | CommandBehavior.CloseConnection);
			else
				return cmd.ExecuteReader(behavior);
		}
		#endregion

		#region RunCmdDataReaderBlob()
		public static IDataReader RunCmdDataReaderBlob(SqlCommand cmd)
		{
			return RunCmdDataReader(cmd, CommandBehavior.SequentialAccess | CommandBehavior.SingleRow);
		}
		#endregion

		#region RunCmdDataTable()
		public static DataTable RunCmdDataTable(SqlCommand cmd)
		{
			DataSet ds = new DataSet();
			try
			{
				SetupConnection(cmd);
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.Fill(ds);
			}
			finally
			{
				CloseConnection(cmd);
			}

			return ds.Tables[0];
		}

		public static DataTable RunCmdDataTable(SqlCommand cmd, int timezoneId, string[] shiftTimeCollumns)
		{
			DataTable table = RunCmdDataTable(cmd);

			if (shiftTimeCollumns != null && shiftTimeCollumns.Length > 0)
			{
				foreach (DataRow row in table.Rows)
				{
					foreach (string column in shiftTimeCollumns)
						row[column] = ProcessDateTimeValue(timezoneId, row[column]);
				}
			}
			return table;
		}
		#endregion

		#region RunCmdDateTime()
		public static DateTime RunCmdDateTime(SqlCommand cmd)
		{
			DateTime ret = DateTime.MinValue;
			object o = RunCmdRetval(cmd, SqlDbType.DateTime);
			if (o != null)
				ret = (DateTime)o;
			return ret;
		}
		#endregion

		#region RunCmdInteger()
		public static int RunCmdInteger(SqlCommand cmd)
		{
			int ret = -1;
			object o = RunCmdRetval(cmd, SqlDbType.Int);
			if (o != null)
				ret = (int)o;
			return ret;
		}
		#endregion

		#region RunCmdLong()
		public static long RunCmdLong(SqlCommand cmd)
		{
			long ret = -1;
			object o = RunCmdRetval(cmd, SqlDbType.BigInt);
			if (o != null)
				ret = (long)o;
			return ret;
		}
		#endregion

		#region RunCmdUniqueIdentifier()
		public static Guid RunCmdUniqueIdentifier(SqlCommand cmd)
		{
			Guid ret = Guid.Empty;
			object o = RunCmdRetval(cmd, SqlDbType.UniqueIdentifier);
			if (o != null)
				ret = (Guid)o;
			return ret;
		}
		#endregion

		#region RunCmdRetval()
		public static object RunCmdRetval(SqlCommand cmd, SqlDbType type)
		{
			object ret = null;
			try
			{
				AddRetVal(cmd, type);
				SetupConnection(cmd);
				cmd.ExecuteNonQuery();
			}
			finally
			{
				CloseConnection(cmd);
			}

			object o = cmd.Parameters[_retval].Value;
			if (o != DBNull.Value)
				ret = o;

			return ret;
		}
		#endregion

		#region RunCmdScalar()
		public static object RunCmdScalar(SqlCommand cmd)
		{
			object ret = null;
			try
			{
				SetupConnection(cmd);
				ret = cmd.ExecuteScalar();
			}
			finally
			{
				CloseConnection(cmd);
			}
			return ret;
		}
		#endregion

		#region RunCmdXmlDocument()
		public static XmlDocument RunCmdXmlDocument(SqlCommand cmd)
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
				CloseConnection(cmd);
			}

			return ret;
		}
		#endregion


		// Stored Procedure
		#region RunSp()
		public static void RunSp(string cmdText, params object[] paramList)
		{
			RunCmd(MakeCommand(CommandType.StoredProcedure, cmdText, paramList));
		}

		public static SqlCommand RunSp(SqlCommand cmd, string cmdText, params object[] paramList)
		{
			DbTransaction.Demand();

			if (null == cmd)
				cmd = MakeCommand(CommandType.StoredProcedure, cmdText, paramList);

			RunCmd(cmd);

			return cmd;
		}
		#endregion

		#region RunSpDataReader()
		public static IDataReader RunSpDataReader(string cmdText, params object[] paramList)
		{
			return RunCmdDataReader(MakeCommand(CommandType.StoredProcedure, cmdText, paramList));
		}

		public static IDataReader RunSpDataReader(int TimeZoneId, string[] ShiftTimeOffsetCollumns, string strSP, params SqlParameter[] paramList)
		{
			IDataReader reader = RunSpDataReader(strSP, paramList);

			if (reader != null && ShiftTimeOffsetCollumns != null && ShiftTimeOffsetCollumns.Length > 0)
				reader = (IDataReader)(new DataReaderWithTimeOffset2(reader, TimeZoneId, ShiftTimeOffsetCollumns));
			return reader;
		}
		#endregion

		#region RunSpDataReaderBlob()
		public static IDataReader RunSpDataReaderBlob(string cmdText, params object[] paramList)
		{
			return RunCmdDataReaderBlob(MakeCommand(CommandType.StoredProcedure, cmdText, paramList));
		}
		#endregion

		#region RunSpDataTable()
		public static DataTable RunSpDataTable(string cmdText, params object[] paramList)
		{
			return RunCmdDataTable(MakeCommand(CommandType.StoredProcedure, cmdText, paramList));
		}

		public static DataTable RunSpDataTable(int timezoneId, string[] shiftTimeCollumns, string cmdText, params object[] paramList)
		{
			return RunCmdDataTable(MakeCommand(CommandType.StoredProcedure, cmdText, paramList), timezoneId, shiftTimeCollumns);
		}
		#endregion

		#region RunSpDateTime()
		public static DateTime RunSpDateTime(string cmdText, params object[] paramList)
		{
			return RunCmdDateTime(MakeCommand(CommandType.StoredProcedure, cmdText, paramList));
		}
		#endregion

		#region RunSpInteger()
		public static int RunSpInteger(string cmdText, params object[] paramList)
		{
			return RunCmdInteger(MakeCommand(CommandType.StoredProcedure, cmdText, paramList));
		}
		#endregion

		#region RunSpLong()
		public static long RunSpLong(string cmdText, params object[] paramList)
		{
			return RunCmdLong(MakeCommand(CommandType.StoredProcedure, cmdText, paramList));
		}
		#endregion

		#region RunSpUniqueIdentifier()
		public static Guid RunSpUniqueIdentifier(string cmdText, params object[] paramList)
		{
			return RunCmdUniqueIdentifier(MakeCommand(CommandType.StoredProcedure, cmdText, paramList));
		}
		#endregion

		#region RunSpScalar()
		public static object RunSpScalar(string cmdText, params object[] paramList)
		{
			return RunCmdScalar(MakeCommand(CommandType.StoredProcedure, cmdText, paramList));
		}
		#endregion


		// Command Text
		#region RunText()
		public static void RunText(string cmdText, params object[] paramList)
		{
			RunCmd(MakeCommand(CommandType.Text, cmdText, paramList));
		}
		#endregion

		#region RunTextDataReader()
		public static IDataReader RunTextDataReader(string cmdText, params object[] paramList)
		{
			return RunCmdDataReader(MakeCommand(CommandType.Text, cmdText, paramList));
		}
		#endregion

		#region RunTextInteger()
		public static int RunTextInteger(string cmdText, params object[] paramList)
		{
			return RunCmdInteger(MakeCommand(CommandType.Text, cmdText, paramList));
		}
		#endregion

		#region RunTextDataTable()
		public static DataTable RunTextDataTable(string cmdText, params object[] paramList)
		{
			return RunCmdDataTable(MakeCommand(CommandType.Text, cmdText, paramList));
		}
		public static DataTable RunTextDataTable(int timezoneId, string[] shiftTimeCollumns, string cmdText, params object[] paramList)
		{
			return RunCmdDataTable(MakeCommand(CommandType.Text, cmdText, paramList), timezoneId, shiftTimeCollumns);
		}
		#endregion

		#region RunTextScalar()
		public static object RunTextScalar(string cmdText, params object[] paramList)
		{
			return RunCmdScalar(MakeCommand(CommandType.Text, cmdText, paramList));
		}
		#endregion

		#region RunTextXmlDocument()
		public static XmlDocument RunTextXmlDocument(string cmdText, params object[] paramList)
		{
			return RunCmdXmlDocument(MakeCommand(CommandType.Text, cmdText, paramList));
		}
		#endregion


		// SQL Script
		#region RunScript
		public static void RunScript(StreamReader Script, NameValueCollection Replace)
		{
			SqlCommand cmd = new SqlCommand();
			try
			{
				cmd.CommandType = CommandType.Text;
				SetupConnection(cmd);

				RunScript(cmd, Script, Replace);
			}
			finally
			{
				CloseConnection(cmd);
			}
		}

		public static void RunScript(SqlCommand cmd, StreamReader Script, NameValueCollection Replace)
		{
			StringBuilder sb = new StringBuilder();
			string line;
			bool eof, append;
			char[] crap = "\r\n\t ".ToCharArray();

			eof = false;
			sb = new StringBuilder();

			while (!eof)
			{
				append = false;
				while ((line = Script.ReadLine()) != null)
				{
					if (0 == string.Compare(line, "GO", true))
						break;
					if (append || line.Length != 0)
					{
						append = true;
						sb.AppendFormat("{0}\r\n", line);
					}
				}
				if (Replace != null)
				{
					foreach (string oldValue in Replace.AllKeys)
						sb = sb.Replace(oldValue, Replace[oldValue]);
				}
				cmd.CommandText = sb.ToString().Trim(crap);
				if (cmd.CommandText.Length != 0)
					cmd.ExecuteNonQuery();

				sb.Length = 0;
				eof = (line == null);
			}
		}
		#endregion


		#region RunSpDataTableAndInteger(...)
		private static DataTable RunSpDataTableAndInteger(ref int retVal, string cmdText, params object[] paramList)
		{
			return RunCmdDataTableAndInteger(MakeCommand(CommandType.StoredProcedure, cmdText, paramList), ref retVal);
		}
		private static DataTable RunCmdDataTableAndInteger(SqlCommand cmd, ref int retVal)
		{
			object o = null;
			DataTable ret = RunCmdDataTableAndRetval(cmd, SqlDbType.Int, ref o);
			if (o != null)
				retVal = (int)o;
			return ret;
		}
		private static DataTable RunCmdDataTableAndRetval(SqlCommand cmd, SqlDbType type, ref object retVal)
		{
			DataTable ret = null;

			DataSet ds = new DataSet();
			try
			{
				AddRetVal(cmd, type);
				SetupConnection(cmd);

				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.Fill(ds);
			}
			finally
			{
				CloseConnection(cmd);
			}

			object o = cmd.Parameters[_retval].Value;
			if (o != DBNull.Value)
				retVal = o;

			if (ds.Tables.Count > 0)
				ret = ds.Tables[0];

			return ret;
		}
		#endregion

		#region RunSPReturnXMLReader
		private static XmlReader RunSPReturnXMLReader(string strSP, params object[] paramList)
		{
			SqlCommand cmd = new SqlCommand(strSP);
			cmd.CommandType = CommandType.StoredProcedure;

			foreach (SqlParameter param in paramList)
				cmd.Parameters.Add(param);

			SetupConnection(cmd);

			return cmd.ExecuteXmlReader();
		}
		#endregion

		#region AddRetVal(...)
		private static void AddRetVal(SqlCommand cmd, SqlDbType type)
		{
			if (cmd.Parameters.Contains(_retval))
				cmd.Parameters.Remove(cmd.Parameters[_retval]);
			cmd.Parameters.Add(new SqlParameter(_retval, type));
			cmd.Parameters[_retval].Direction = ParameterDirection.Output;
		}
		#endregion
	}
	#endregion

	#region class DbTransactionEventArgs
	public class DbTransactionEventArgs : EventArgs
	{
		private DbTransaction _dbTran;
		private bool _sqlTransacted;

		public DbTransaction Transaction { get { return _dbTran; } }
		public bool SqlTransacted { get { return _sqlTransacted; } }

		internal DbTransactionEventArgs(DbTransaction dbTran, bool sqlTransacted)
		{
			_dbTran = dbTran;
			_sqlTransacted = sqlTransacted;
		}
	}
	#endregion

	#region class DbTransaction
	public class DbTransaction : IDisposable
	{
		//Private

		[ThreadStatic]
		private static int _transactionCount;

		private bool _sqlTransactionCreated;
		private bool _commitCalled;

		public static event EventHandler<DbTransactionEventArgs> Created;
		public static event EventHandler<DbTransactionEventArgs> Committed;
		public static event EventHandler<DbTransactionEventArgs> RolledBack;

		private DbTransaction(bool sqlTransactionCreated)
		{
			_sqlTransactionCreated = sqlTransactionCreated;
		}

		// Public

		//public bool SqlTransactionCreated { get { return _sqlTransactionCreated; } }

		#region Begin()
		public static DbTransaction Begin()
		{
			SqlTransaction sqlTran = null;
			if (_transactionCount == 0 && DbContext.Current.Transaction == null)
			{
				string database = DbContext.Current.Database;
				SqlConnection con = new SqlConnection(DbHelper2.ConnectionString);
				con.Open();

				if (con.Database != database)
					con.ChangeDatabase(database);

				sqlTran = con.BeginTransaction();
				DbContext.Current.Transaction = sqlTran;
			}

			DbTransaction retVal = new DbTransaction(sqlTran != null);

			if (_transactionCount == 0)
			{
				if (Created != null)
					Created(null, new DbTransactionEventArgs(retVal, retVal._sqlTransactionCreated));
			}

			_transactionCount++;

			return retVal;
		}
		#endregion

		#region Commit()
		public void Commit()
		{
			_commitCalled = true;

			_transactionCount--;
			if (_transactionCount == 0)
			{
				if (_sqlTransactionCreated)
				{
					SqlTransaction sqlTran = DbContext.Current.Transaction;
					SqlConnection con = sqlTran.Connection;
					try
					{
						sqlTran.Commit();
					}
					catch
					{
						try
						{
							sqlTran.Rollback();
						}
						catch { }
						throw;
					}
					finally
					{
						if (con != null)
							con.Close();
					}

					DbContext.Current.Transaction = null;
					sqlTran.Dispose();
				}

				if (Committed != null)
					Committed(this, new DbTransactionEventArgs(this, _sqlTransactionCreated));
			}
		}
		#endregion

		#region Rollback()
		public void Rollback()
		{
			_transactionCount--;
			if (_transactionCount == 0)
			{
				if (_sqlTransactionCreated)
				{
					SqlTransaction sqlTran = DbContext.Current.Transaction;
					SqlConnection con = sqlTran.Connection;
					try
					{
						sqlTran.Rollback();
					}
					catch
					{
					}

					if (con != null)
						con.Close();

					DbContext.Current.Transaction = null;
					sqlTran.Dispose();
				}

				if (RolledBack != null)
					RolledBack(this, new DbTransactionEventArgs(this, _sqlTransactionCreated));
			}
		}
		#endregion

		#region Demand()
		public static void Demand()
		{
			if (_transactionCount == 0)
				throw new Exception("This operation requires active transaction.");
		}
		#endregion


		#region IDisposable Members
		public void Dispose()
		{
			if (!_commitCalled)
				Rollback();
		}
		#endregion

		public static void OnExternalTransactionCommitted()
		{
			if (Committed != null)
				Committed(null, new DbTransactionEventArgs(null, true));
		}
	}
	#endregion

	#region class DataReaderWithTimeOffset
	internal class DataReaderWithTimeOffset2 : MarshalByRefObject, IEnumerable, IDataReader, IDisposable, IDataRecord
	{
		private IDataReader m_innerReader = null;
		private int m_timeZoneId = 0;
		private McTimeZone2 m_timeZone = null;

		private Hashtable _hash = new Hashtable();

		public DataReaderWithTimeOffset2(IDataReader reader, int TimeZoneId, string[] ShiftTimeOffsetCollumns)
		{
			m_innerReader = reader;
			m_timeZoneId = TimeZoneId;
			m_timeZone = McTimeZone2.Load(m_timeZoneId);

			if (ShiftTimeOffsetCollumns != null)
			{
				foreach (string columnName in ShiftTimeOffsetCollumns)
					_hash.Add(columnName, string.Empty);
			}
		}

		public McTimeZone2 TimeZone
		{
			get
			{
				return m_timeZone;
			}
		}

		protected virtual object GetLocalDate(string name, object Value)
		{
			if (Value is DateTime && _hash.ContainsKey(name))
			{
				return this.TimeZone.GetLocalDate((DateTime)Value);
			}
			return Value;
		}

		protected virtual object GetLocalDate(int i, object Value)
		{
			return GetLocalDate(this.InnerReader.GetName(i), Value);
		}

		public virtual IDataReader InnerReader
		{
			get
			{
				return m_innerReader;
			}
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new System.Data.Common.DbEnumerator(this, DbContext.Current.Transaction == null);
		}

		#endregion

		#region IDataReader Members

		public int RecordsAffected
		{
			get
			{
				return ((IDataReader)this.InnerReader).RecordsAffected;
			}
		}

		public bool IsClosed
		{
			get
			{
				return ((IDataReader)this.InnerReader).IsClosed;
			}
		}

		public bool NextResult()
		{
			return ((IDataReader)this.InnerReader).NextResult();
		}

		public void Close()
		{
			((IDataReader)this.InnerReader).Close();
		}

		public bool Read()
		{
			return ((IDataReader)this.InnerReader).Read();
		}

		public int Depth
		{
			get
			{
				return ((IDataReader)this.InnerReader).Depth;
			}
		}

		public DataTable GetSchemaTable()
		{
			// TODO: Add IDataReader.GetSchemaTable implementation
			return null;
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			((IDisposable)this.InnerReader).Dispose();
		}

		#endregion

		#region IDataRecord Members

		public int GetInt32(int i)
		{
			return ((IDataRecord)this.InnerReader).GetInt32(i);
		}

		public object this[string name]
		{
			get
			{
				return GetLocalDate(name, ((IDataRecord)this.InnerReader)[name]);
			}
		}

		object System.Data.IDataRecord.this[int i]
		{
			get
			{
				return GetLocalDate(i, ((IDataRecord)this.InnerReader)[i]);
			}
		}

		public object GetValue(int i)
		{
			return GetLocalDate(i, ((IDataRecord)this.InnerReader).GetValue(i));
		}

		public bool IsDBNull(int i)
		{
			return ((IDataRecord)this.InnerReader).IsDBNull(i);
		}

		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return ((IDataRecord)this.InnerReader).GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}

		public byte GetByte(int i)
		{
			return ((IDataRecord)this.InnerReader).GetByte(i);
		}

		public Type GetFieldType(int i)
		{
			return ((IDataRecord)this.InnerReader).GetFieldType(i);
		}

		public decimal GetDecimal(int i)
		{
			return ((IDataRecord)this.InnerReader).GetDecimal(i);
		}

		public int GetValues(object[] values)
		{
			int retVal = ((IDataRecord)this.InnerReader).GetValues(values);

			for (int index = 0; index < values.Length; index++)
			{
				values[index] = GetLocalDate(index, values[index]);
			}

			return retVal;
		}

		public string GetName(int i)
		{
			return ((IDataRecord)this.InnerReader).GetName(i);
		}

		public int FieldCount
		{
			get
			{
				return ((IDataRecord)this.InnerReader).FieldCount;
			}
		}

		public long GetInt64(int i)
		{
			return ((IDataRecord)this.InnerReader).GetInt64(i);
		}

		public double GetDouble(int i)
		{
			return ((IDataRecord)this.InnerReader).GetDouble(i);
		}

		public bool GetBoolean(int i)
		{
			return ((IDataRecord)this.InnerReader).GetBoolean(i);
		}

		public Guid GetGuid(int i)
		{
			return ((IDataRecord)this.InnerReader).GetGuid(i);
		}

		public DateTime GetDateTime(int i)
		{
			return (DateTime)GetLocalDate(i, ((IDataRecord)this.InnerReader).GetDateTime(i));
		}

		public int GetOrdinal(string name)
		{
			return ((IDataRecord)this.InnerReader).GetOrdinal(name);
		}

		public string GetDataTypeName(int i)
		{
			return ((IDataRecord)this.InnerReader).GetDataTypeName(i);
		}

		public float GetFloat(int i)
		{
			return ((IDataRecord)this.InnerReader).GetFloat(i);
		}

		public IDataReader GetData(int i)
		{
			return ((IDataRecord)this.InnerReader).GetData(i);
		}

		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return ((IDataRecord)this.InnerReader).GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		public string GetString(int i)
		{
			return ((IDataRecord)this.InnerReader).GetString(i);
		}

		public char GetChar(int i)
		{
			return ((IDataRecord)this.InnerReader).GetChar(i);
		}

		public short GetInt16(int i)
		{
			return ((IDataRecord)this.InnerReader).GetInt16(i);
		}

		#endregion
	}
	#endregion

	#region class McTimeZone2
	public class McTimeZone2
	{
		private static Hashtable _hash = new Hashtable(255);

		private int _TimeZoneId;
		private int _Bias;
		private int _StandardBias;
		private int _DaylightBias;
		private int _DaylightMonth;
		private int _DaylightDayOfWeek;
		private int _DaylightWeek;
		private int _DaylightHour;
		private int _StandardMonth;
		private int _StandardDayOfWeek;
		private int _StandardWeek;
		private int _StandardHour;
		private Hashtable _cachedDaylightChanges = new Hashtable(16);

		static McTimeZone2()
		{
			_hash.Add(0, new McTimeZone2(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(1, new McTimeZone2(1, -270, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(2, new McTimeZone2(2, 540, 0, -60, 4, 0, 1, 2, 10, 0, 5, 2));
			_hash.Add(3, new McTimeZone2(3, -180, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(4, new McTimeZone2(4, -240, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(5, new McTimeZone2(5, -180, 0, -60, 4, 0, 1, 3, 10, 0, 1, 4));
			_hash.Add(6, new McTimeZone2(6, 240, 0, -60, 4, 0, 1, 2, 10, 0, 5, 2));
			_hash.Add(7, new McTimeZone2(7, -570, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(8, new McTimeZone2(8, -600, 0, -60, 10, 0, 5, 2, 3, 0, 5, 3));
			_hash.Add(9, new McTimeZone2(9, 60, 0, -60, 3, 0, 5, 2, 10, 0, 5, 3));
			_hash.Add(10, new McTimeZone2(10, 360, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(11, new McTimeZone2(11, 60, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(12, new McTimeZone2(12, -240, 0, -60, 3, 0, 5, 2, 10, 0, 5, 3));
			_hash.Add(13, new McTimeZone2(13, -570, 0, -60, 10, 0, 5, 2, 3, 0, 5, 3));
			_hash.Add(14, new McTimeZone2(14, 360, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(15, new McTimeZone2(15, -360, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(16, new McTimeZone2(16, -60, 0, -60, 3, 0, 5, 2, 10, 0, 5, 3));
			_hash.Add(17, new McTimeZone2(17, -60, 0, -60, 3, 0, 5, 2, 10, 0, 5, 3));
			_hash.Add(18, new McTimeZone2(18, -660, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(19, new McTimeZone2(19, 360, 0, -60, 4, 0, 1, 2, 10, 0, 5, 2));
			_hash.Add(20, new McTimeZone2(20, -480, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(21, new McTimeZone2(21, 720, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(22, new McTimeZone2(22, -180, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(23, new McTimeZone2(23, -600, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(24, new McTimeZone2(24, -120, 0, -60, 3, 0, 5, 0, 10, 0, 5, 1));
			_hash.Add(25, new McTimeZone2(25, 180, 0, -60, 10, 0, 3, 2, 2, 0, 2, 2));
			_hash.Add(26, new McTimeZone2(26, 300, 0, -60, 4, 0, 1, 2, 10, 0, 5, 2));
			_hash.Add(27, new McTimeZone2(27, -120, 0, -60, 5, 5, 1, 2, 9, 3, 5, 2));
			_hash.Add(28, new McTimeZone2(28, -300, 0, -60, 3, 0, 5, 2, 10, 0, 5, 3));
			_hash.Add(29, new McTimeZone2(29, -720, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(30, new McTimeZone2(30, -120, 0, -60, 3, 0, 5, 3, 10, 0, 5, 4));
			_hash.Add(31, new McTimeZone2(31, 0, 0, -60, 3, 0, 5, 1, 10, 0, 5, 2));
			_hash.Add(32, new McTimeZone2(32, 180, 0, -60, 4, 0, 1, 2, 10, 0, 5, 2));
			_hash.Add(33, new McTimeZone2(33, 0, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(34, new McTimeZone2(34, -120, 0, -60, 3, 0, 5, 2, 10, 0, 5, 3));
			_hash.Add(35, new McTimeZone2(35, 600, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(36, new McTimeZone2(36, -330, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(37, new McTimeZone2(37, -210, 0, -60, 3, 0, 1, 2, 9, 2, 4, 2));
			_hash.Add(38, new McTimeZone2(38, -120, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(39, new McTimeZone2(39, -540, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(40, new McTimeZone2(40, 360, 0, -60, 4, 0, 1, 2, 10, 0, 5, 2));
			_hash.Add(41, new McTimeZone2(41, 420, 0, -60, 4, 0, 1, 2, 10, 0, 5, 2));
			_hash.Add(42, new McTimeZone2(42, 120, 0, -60, 3, 0, 5, 2, 9, 0, 5, 2));
			_hash.Add(43, new McTimeZone2(43, 420, 0, -60, 4, 0, 1, 2, 10, 0, 5, 2));
			_hash.Add(44, new McTimeZone2(44, -390, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(45, new McTimeZone2(45, -360, 0, -60, 3, 0, 5, 2, 10, 0, 5, 3));
			_hash.Add(46, new McTimeZone2(46, -345, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(47, new McTimeZone2(47, -720, 0, -60, 10, 0, 1, 2, 3, 0, 3, 2));
			_hash.Add(48, new McTimeZone2(48, 210, 0, -60, 4, 0, 1, 2, 10, 0, 5, 2));
			_hash.Add(49, new McTimeZone2(49, -480, 0, -60, 3, 0, 5, 2, 10, 0, 5, 3));
			_hash.Add(50, new McTimeZone2(50, -420, 0, -60, 3, 0, 5, 2, 10, 0, 5, 3));
			_hash.Add(51, new McTimeZone2(51, 240, 0, -60, 10, 6, 2, 0, 3, 6, 2, 0));
			_hash.Add(52, new McTimeZone2(52, 480, 0, -60, 4, 0, 1, 2, 10, 0, 5, 2));
			_hash.Add(53, new McTimeZone2(53, -60, 0, -60, 3, 0, 5, 2, 10, 0, 5, 3));
			_hash.Add(54, new McTimeZone2(54, -180, 0, -60, 3, 0, 5, 2, 10, 0, 5, 3));
			_hash.Add(55, new McTimeZone2(55, 180, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(56, new McTimeZone2(56, 300, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(57, new McTimeZone2(57, 240, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(58, new McTimeZone2(58, 660, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(59, new McTimeZone2(59, -420, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(60, new McTimeZone2(60, -480, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(61, new McTimeZone2(61, -120, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(62, new McTimeZone2(62, -360, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(63, new McTimeZone2(63, -480, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(64, new McTimeZone2(64, -600, 0, -60, 10, 0, 1, 2, 3, 0, 5, 3));
			_hash.Add(65, new McTimeZone2(65, -540, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(66, new McTimeZone2(66, -780, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(67, new McTimeZone2(67, 300, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(68, new McTimeZone2(68, 420, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(69, new McTimeZone2(69, -600, 0, -60, 3, 0, 5, 2, 10, 0, 5, 3));
			_hash.Add(70, new McTimeZone2(70, -480, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(71, new McTimeZone2(71, -60, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(72, new McTimeZone2(72, -60, 0, -60, 3, 0, 5, 2, 10, 0, 5, 3));
			_hash.Add(73, new McTimeZone2(73, -300, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(74, new McTimeZone2(74, -600, 0, -60, 0, 0, 0, 0, 0, 0, 0, 0));
			_hash.Add(75, new McTimeZone2(75, -540, 0, -60, 3, 0, 5, 2, 10, 0, 5, 3));
		}

		public static McTimeZone2 Load(int TimeZoneId)
		{
			return (McTimeZone2)_hash[TimeZoneId];
		}

		protected McTimeZone2()
		{
		}

		protected McTimeZone2(int TimeZoneId,
			int Bias,
			int StandardBias,
			int DaylightBias,
			int DaylightMonth,
			int DaylightDayOfWeek,
			int DaylightWeek,
			int DaylightHour,
			int StandardMonth,
			int StandardDayOfWeek,
			int StandardWeek,
			int StandardHour)
		{
			_TimeZoneId = TimeZoneId;
			_Bias = Bias;
			_StandardBias = StandardBias;
			_DaylightBias = DaylightBias;
			_DaylightMonth = DaylightMonth;
			_DaylightDayOfWeek = DaylightDayOfWeek;
			_DaylightWeek = DaylightWeek;
			_DaylightHour = DaylightHour;
			_StandardMonth = StandardMonth;
			_StandardDayOfWeek = StandardDayOfWeek;
			_StandardWeek = StandardWeek;
			_StandardHour = StandardHour;
		}

		public int TimeZoneId { get { return TimeZoneId; } }

		public int Bias { get { return _Bias; } }
		public int StandardBias { get { return _StandardBias; } }
		public int DaylightBias { get { return _DaylightBias; } }
		public int DaylightMonth { get { return _DaylightMonth; } }
		public int DaylightDayOfWeek { get { return _DaylightDayOfWeek; } }
		public int DaylightWeek { get { return _DaylightWeek; } }
		public int DaylightHour { get { return _DaylightHour; } }
		public int StandardMonth { get { return _StandardMonth; } }
		public int StandardDayOfWeek { get { return _StandardDayOfWeek; } }
		public int StandardWeek { get { return _StandardWeek; } }
		public int StandardHour { get { return _StandardHour; } }

		public DateTime GetLocalDate(DateTime time)
		{
			return new DateTime(time.Ticks + this.GetUtcOffsetFromUniversalTime(time));
		}

		private long GetUtcOffsetFromUniversalTime(DateTime time)
		{
			object Year = time.Year;

			if (!_cachedDaylightChanges.ContainsKey(Year))
			{
				lock (this)
				{
					if (!_cachedDaylightChanges.ContainsKey(Year))
					{
						if (this.DaylightMonth == 0 || this.StandardMonth == 0)
						{
							_cachedDaylightChanges.Add(Year, new DaylightTime(DateTime.MinValue, DateTime.MaxValue, new TimeSpan(0, 0, -(this.Bias), 0, 0)));
						}
						else
							if (this.DaylightMonth < this.StandardMonth)
							{
								DateTime startDaylight = new DateTime(time.Year, this.DaylightMonth, 1, this.DaylightHour/*-(this.StandardBias+this.DaylightBias)/60*/, 0, 0, 0);
								DateTime endDaylight = new DateTime(time.Year, this.StandardMonth, 1, this.StandardHour, 0, 0, 0);

								// Calculate Real Day  [6/18/2004]
								startDaylight = TransformDate(startDaylight, this.DaylightWeek, (DayOfWeek)this.DaylightDayOfWeek);
								endDaylight = TransformDate(endDaylight, this.StandardWeek, (DayOfWeek)this.StandardDayOfWeek);

								startDaylight = startDaylight.AddMinutes((this.Bias + this.StandardBias));
								endDaylight = endDaylight.AddMinutes((this.Bias + this.DaylightBias));

								_cachedDaylightChanges.Add(Year, new DaylightTime(startDaylight, endDaylight, new TimeSpan(0, 0, -(this.Bias + this.DaylightBias), 0, 0)));
							}
							else
							{
								DateTime startStandard = new DateTime(time.Year, this.StandardMonth, 1, this.StandardHour, 0, 0, 0);
								DateTime endStandard = new DateTime(time.Year, this.DaylightMonth, 1, this.DaylightHour/*-(this.StandardBias+this.DaylightBias)/60*/, 0, 0, 0);

								// Calculate Real Day  [6/18/2004]
								startStandard = TransformDate(startStandard, this.StandardWeek, (DayOfWeek)this.StandardDayOfWeek);
								endStandard = TransformDate(endStandard, this.DaylightWeek, (DayOfWeek)this.DaylightDayOfWeek);

								startStandard = startStandard.AddMinutes((this.Bias + this.DaylightBias));
								endStandard = endStandard.AddMinutes((this.Bias + this.StandardBias));

								_cachedDaylightChanges.Add(Year, new DaylightTime(endStandard, startStandard, new TimeSpan(0, 0, -(this.Bias + this.DaylightBias), 0, 0)));
							}
					}
				}
			}

			DaylightTime daylightTime = (DaylightTime)_cachedDaylightChanges[Year];

			if (daylightTime.Start < daylightTime.End)
			{
				if (time < daylightTime.Start || time > daylightTime.End)
				{
					// Standard Time
					return (long)-1 * (long)this.Bias * (long)600000000;
				}
				else
				{
					// Daylight Time
					return daylightTime.Delta.Ticks;
				}
			}
			else
			{
				if (time < daylightTime.End || time > daylightTime.Start)
				{
					// Standard Time
					return daylightTime.Delta.Ticks;
				}
				else
				{
					// Daylight Time
					return (long)-1 * (long)this.Bias * (long)600000000;
				}
			}
		}

		internal static DateTime TransformDate(DateTime date, int Week, DayOfWeek DayOfWeek)
		{
			DateTime retVal = date;
			int tmpMonth = date.Month;
			while (Week > 0 && date.Month == tmpMonth)
			{
				if (date.DayOfWeek == DayOfWeek)
				{
					retVal = date;
					Week--;
				}
				date = date.AddDays(1);
			}
			return retVal;
		}


	}
	#endregion

	#region class DbContext
	public class DbContext
	{
		private string _database;
		private string _portalDatabase;
		private int _commandTimeout = 30;
		private string _portalConnectionString;
		private SqlTransaction _sqlTransaction;
		private string _transactionId;

		[ThreadStatic]
		private static DbContext _current;
		private static string _typeName = typeof(DbContext).FullName;
		#region Current
		public static DbContext Current
		{
			get
			{
				DbContext ret;

				if (HttpContext.Current != null)
					ret = HttpContext.Current.Items[_typeName] as DbContext;
				else
					ret = _current;

				if (ret == null)
				{
					ret = new DbContext();
					Current = ret;
				}

				return ret;
			}
			set
			{
				if (HttpContext.Current != null)
					HttpContext.Current.Items[_typeName] = value;
				else
					_current = value;
			}
		}
		#endregion

		public void Init(string databaseName)
		{
			_database = _portalDatabase = databaseName;
		}

		#region Database
		public string Database
		{
			get
			{
				if (_database != null)
					return _database;
				else
					return _portalDatabase;
			}
			set
			{
				_database = value;
			}
		}
		#endregion

		#region PortalDatabase
		public string PortalDatabase
		{
			get
			{
				return _portalDatabase;
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

		#region PortalConnectionString
		public string PortalConnectionString
		{
			get
			{
				return _portalConnectionString;
			}
			internal set
			{
				_portalConnectionString = value;
			}
		}
		#endregion

		#region public SqlTransaction Transaction
		public SqlTransaction Transaction
		{
			get { return _sqlTransaction; }
			set
			{
				if(value != null && value != _sqlTransaction)
					_transactionId = Guid.NewGuid().ToString();

				_sqlTransaction = value;
			}
		}
		#endregion

		#region public string TransactionId
		public string TransactionId
		{
			get { return _transactionId; }
		}
		#endregion
	}
	#endregion
}
