using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for SqlDataReadetWithTimeOffset.
	/// </summary>
	internal class SqlDataReaderWithTimeOffset: MarshalByRefObject, IEnumerable, IDataReader, IDisposable, IDataRecord
	{
		private	SqlDataReader	_innerReader	=	null;
		private int				_timeZoneId		=	0;
		private	McTimeZone		_timeZone		=	null;

		private Hashtable		_hash			=	new Hashtable();

		public SqlDataReaderWithTimeOffset(SqlDataReader	reader, int TimeZoneId, string[] ShiftTimeOffsetCollumns)
		{
			_innerReader	=	reader;
			_timeZoneId		=	TimeZoneId;
			_timeZone		=	McTimeZone.Load(_timeZoneId);

			if(ShiftTimeOffsetCollumns!=null)
			{
				foreach(string columnName in ShiftTimeOffsetCollumns)
					_hash.Add(columnName,string.Empty);
			}
		}

		public McTimeZone	TimeZone
		{
			get
			{
				return _timeZone;
			}
		}

		protected int	CalculateTimeOffset(DateTime value)
		{
			return 0;
		}

		protected virtual object GetLocalDate(string name, object Value)
		{
			if(Value is DateTime && _hash.ContainsKey(name))
			{
				return this.TimeZone.GetLocalDate((DateTime)Value);
			}
 
			return Value;
		}

		protected virtual object GetLocalDate(int i, object Value)
		{
			return GetLocalDate(this.InnerReader.GetName(i),Value);
		}

		public virtual SqlDataReader	InnerReader
		{
			get
			{
				return _innerReader;
			}
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable)this.InnerReader).GetEnumerator();
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
			// TODO:  Add SqlDataReadetWithTimeOffset.GetSchemaTable implementation
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
				return GetLocalDate(name,((IDataRecord)this.InnerReader)[name]);
			}
		}

		object System.Data.IDataRecord.this[int i]
		{
			get
			{
				return GetLocalDate(i,((IDataRecord)this.InnerReader)[i]);
			}
		}

		public object GetValue(int i)
		{
			return GetLocalDate(i,((IDataRecord)this.InnerReader).GetValue(i));
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
			return ((IDataRecord)this.InnerReader).GetValues(values);
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
			return  ((IDataRecord)this.InnerReader).GetInt64(i);
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
			return (DateTime)GetLocalDate(i,((IDataRecord)this.InnerReader).GetDateTime(i));
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
}
