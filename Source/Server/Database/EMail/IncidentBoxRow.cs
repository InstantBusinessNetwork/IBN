
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;
using Mediachase.Ibn;

namespace Mediachase.IBN.Database.EMail
{
	public class IncidentBoxRow
	{
		#region Const
		public const string IncidentBoxCategoryName = "IncidentBox";
		#endregion

		private enum DataRowState
		{
			Unchanged,
			Added,
			Modified,
			Deleted
		}

		private DataRowState _state = DataRowState.Unchanged;

		#region Util
		static object DBNull2Null(object Value)
		{
			if (Value == DBNull.Value)
				return null;
			return Value;
		}
		static object DBNull2Null(object Value, object NullValue)
		{
			if (Value == DBNull.Value)
				return NullValue;
			return Value;
		}
		#endregion

		int _IncidentBoxId;
		string _Name;
		string _IdentifierMask;
		bool _IsDefault;
		int _ManagerId;

		int _ControllerId;
		string _Document = string.Empty;

		int _CalendarId;
		int _ExpectedDuration;
		int _ExpectedResponseTime;
		int _ExpectedAssignTime;
		int _TaskTime;

		int _Index = -1;

		#region constructor
		public IncidentBoxRow()
		{
			_state = DataRowState.Added;
			_IsDefault = false;
			_ControllerId = -1;
			_ManagerId = -1;
		}

		protected IncidentBoxRow(int IncidentBoxId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_IncidentBoxSelect", DbHelper2.mp("@IncidentBoxId", SqlDbType.Int, IncidentBoxId)))
			{
				if (reader.Read())
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "IncidentBox", "IncidentBoxId", IncidentBoxId));
			}
		}

		protected IncidentBoxRow(IDataReader reader)
		{
			Load(reader);
		}
		#endregion

		#region Static Load Method
		public static IncidentBoxRow Load(int IncidentBoxId)
		{
			IncidentBoxRow retVal;

			// OZ 2008-12-08 Improve Performance
			if (!DataCache.TryGetValue<IncidentBoxRow>(IncidentBoxCategoryName, DataCache.EmptyUser, IncidentBoxId.ToString(), out retVal))
			{
				retVal = new IncidentBoxRow(IncidentBoxId);

				// OZ 2008-12-08 Improve Performance
				DataCache.Add(IncidentBoxCategoryName, DataCache.EmptyUser, IncidentBoxId.ToString(), retVal);
			}

			return retVal;
		}

		#endregion

		#region Load
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;


			_IncidentBoxId = (int)DBNull2Null(reader["IncidentBoxId"]);

			_Name = (string)DBNull2Null(reader["Name"]);

			_IdentifierMask = (string)DBNull2Null(reader["IdentifierMask"]);

			_IsDefault = (bool)DBNull2Null(reader["IsDefault"]);

			_ManagerId = (int)DBNull2Null(reader["ManagerId"], (int)-1);

			_ControllerId = (int)DBNull2Null(reader["ControllerId"], (int)-1);

			_Document = (string)DBNull2Null(reader["Document"]);

			_CalendarId = (int)DBNull2Null(reader["CalendarId"]);

			_ExpectedDuration = (int)DBNull2Null(reader["ExpectedDuration"]);

			_ExpectedResponseTime = (int)DBNull2Null(reader["ExpectedResponseTime"]);

			_ExpectedAssignTime = (int)DBNull2Null(reader["ExpectedAssignTime"]);

			_TaskTime = (int)DBNull2Null(reader["TaskTime"]);

			_Index = (int)DBNull2Null(reader["Index"], -1);

		}
		#endregion

		#region Public Properties

		public virtual int PrimaryKeyId
		{
			get { return _IncidentBoxId; }
		}


		public virtual int IncidentBoxId
		{
			get
			{
				return _IncidentBoxId;
			}

		}

		public virtual string Name
		{
			get
			{
				return _Name;
			}

			set
			{
				_Name = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual string IdentifierMask
		{
			get
			{
				return _IdentifierMask;
			}

			set
			{
				_IdentifierMask = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual bool IsDefault
		{
			get
			{
				return _IsDefault;
			}

			set
			{
				_IsDefault = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual int ManagerId
		{
			get
			{
				return _ManagerId;
			}

			set
			{
				_ManagerId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual int ControllerId
		{
			get
			{
				return _ControllerId;
			}
			set
			{
				_ControllerId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual string Document
		{
			get
			{
				return _Document;
			}

			set
			{
				_Document = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual int CalendarId
		{
			get
			{
				return _CalendarId;
			}
			set
			{
				_CalendarId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual int ExpectedDuration
		{
			get
			{
				return _ExpectedDuration;
			}
			set
			{
				_ExpectedDuration = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual int ExpectedResponseTime
		{
			get
			{
				return _ExpectedResponseTime;
			}
			set
			{
				_ExpectedResponseTime = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}
		}

		public virtual int ExpectedAssignTime
		{
			get
			{
				return _ExpectedAssignTime;
			}
			set
			{
				_ExpectedAssignTime = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}
		}

		public virtual int TaskTime
		{
			get
			{
				return _TaskTime;
			}
			set
			{
				_TaskTime = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}
		}

		public virtual int Index
		{
			get
			{
				return _Index;
			}
			set
			{
				_Index = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}
		#endregion

		#region Update
		public virtual void Update()
		{
			if (_state == DataRowState.Modified)
			{
				DbHelper2.RunSp("mc_IncidentBoxUpdate"
					, DbHelper2.mp("@IncidentBoxId", SqlDbType.Int, _IncidentBoxId)
					, DbHelper2.mp("@Name", SqlDbType.NVarChar, 100, _Name)
					, DbHelper2.mp("@IdentifierMask", SqlDbType.NVarChar, 255, _IdentifierMask)
					, DbHelper2.mp("@IsDefault", SqlDbType.Bit, _IsDefault)
					, DbHelper2.mp("@ManagerId", SqlDbType.Int, _ManagerId <= 0 ? DBNull.Value : (object)_ManagerId)
					, DbHelper2.mp("@ControllerId", SqlDbType.Int, _ControllerId < 0 ? DBNull.Value : (object)_ControllerId)
					, DbHelper2.mp("@Document", SqlDbType.NText, _Document)
					, DbHelper2.mp("@CalendarId", SqlDbType.Int, _CalendarId)
					, DbHelper2.mp("@ExpectedDuration", SqlDbType.Int, _ExpectedDuration)
					, DbHelper2.mp("@ExpectedResponseTime", SqlDbType.Int, _ExpectedResponseTime)
					, DbHelper2.mp("@ExpectedAssignTime", SqlDbType.Int, _ExpectedAssignTime)
					, DbHelper2.mp("@TaskTime", SqlDbType.Int, _TaskTime)
					, DbHelper2.mp("@Index", SqlDbType.Int, _Index >= 0 ? (object)_Index : DBNull.Value)
				);
			}
			else if (_state == DataRowState.Added)
			{
				SqlParameter outPutKey = new SqlParameter("@IncidentBoxId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_IncidentBoxInsert", outPutKey
					, DbHelper2.mp("@Name", SqlDbType.NVarChar, 100, _Name)
					, DbHelper2.mp("@IdentifierMask", SqlDbType.NVarChar, 255, _IdentifierMask)
					, DbHelper2.mp("@IsDefault", SqlDbType.Bit, _IsDefault)
					, DbHelper2.mp("@ManagerId", SqlDbType.Int, _ManagerId <= 0 ? DBNull.Value : (object)_ManagerId)
					, DbHelper2.mp("@ControllerId", SqlDbType.Int, _ControllerId < 0 ? DBNull.Value : (object)_ControllerId)
					, DbHelper2.mp("@Document", SqlDbType.NText, _Document)
					, DbHelper2.mp("@CalendarId", SqlDbType.Int, _CalendarId)
					, DbHelper2.mp("@ExpectedDuration", SqlDbType.Int, _ExpectedDuration)
					, DbHelper2.mp("@ExpectedResponseTime", SqlDbType.Int, _ExpectedResponseTime)
					, DbHelper2.mp("@ExpectedAssignTime", SqlDbType.Int, _ExpectedAssignTime)
					, DbHelper2.mp("@TaskTime", SqlDbType.Int, _TaskTime)
					, DbHelper2.mp("@Index", SqlDbType.Int, _Index >= 0 ? (object)_Index : DBNull.Value)
				);
				_IncidentBoxId = (int)outPutKey.Value;
			}

			// OZ 2008-12-08 Improve Performance
			DataCache.Remove(IncidentBoxCategoryName, DataCache.EmptyUser, IncidentBoxId.ToString());
		}
		#endregion

		#region Delete
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;

			Delete(_IncidentBoxId);
		}
		#endregion

		#region Delete
		public static void Delete(int IncidentBoxId)
		{
			DbHelper2.RunSp("mc_IncidentBoxDelete", DbHelper2.mp("@IncidentBoxId", SqlDbType.Int, IncidentBoxId));

			// OZ 2008-12-08 Improve Performance
			DataCache.Remove(IncidentBoxCategoryName, DataCache.EmptyUser, IncidentBoxId.ToString());
		}
		#endregion

		#region List
		public static IncidentBoxRow[] List()
		{
			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_IncidentBoxList"))
			{
				while (reader.Read())
				{
					retVal.Add(new IncidentBoxRow(reader));
				}
			}
			return (IncidentBoxRow[])retVal.ToArray(typeof(IncidentBoxRow));
		}
		#endregion

		#region ListWithRules
		public static IncidentBoxRow[] ListWithRules()
		{
			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_IncidentBoxListWithRules"))
			{
				while (reader.Read())
				{
					retVal.Add(new IncidentBoxRow(reader));
				}
			}
			return (IncidentBoxRow[])retVal.ToArray(typeof(IncidentBoxRow));
		}
		#endregion

		#region ListWithoutRules
		public static IncidentBoxRow[] ListWithoutRules()
		{
			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_IncidentBoxListWithoutRules"))
			{
				while (reader.Read())
				{
					retVal.Add(new IncidentBoxRow(reader));
				}
			}
			return (IncidentBoxRow[])retVal.ToArray(typeof(IncidentBoxRow));
		}
		#endregion

		#region CanDelete
		public static bool CanDelete(int IncidentBoxId)
		{
			return DbHelper2.RunSpInteger("mc_IncidentBoxCanDelete",
				DbHelper2.mp("@IncindentBoxId", SqlDbType.Int, IncidentBoxId)) != 0;
		}
		#endregion
	}
}

