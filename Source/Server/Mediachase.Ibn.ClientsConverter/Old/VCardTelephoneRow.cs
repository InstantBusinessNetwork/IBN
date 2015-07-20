using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.Database;


namespace Mediachase.Ibn.Database.VCard
{
	internal class VCardTelephoneRow
	{
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
		#endregion

		int _VCardTelephoneId;
		int _VCardId;
		string _Number;
		int _TelephoneTypeId;


		public VCardTelephoneRow()
		{
			_state = DataRowState.Added;


		}

		public VCardTelephoneRow(int VCardTelephoneId)
		{
			using (IDataReader reader = DBHelper2.DBHelper.RunSPDataReader("mc_VCardTelephoneSelect", DBHelper.MP("@VCardTelephoneId", SqlDbType.Int, VCardTelephoneId)))
			{
				if (reader.Read())
				{
					Load(reader);
				}
				else throw new VCardRowException("VCardTelephone", "VCardTelephoneId", VCardTelephoneId);
			}
		}

		public VCardTelephoneRow(IDataReader reader)
		{
			Load(reader);
		}

		protected void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;


			_VCardTelephoneId = (int)DBNull2Null(reader["VCardTelephoneId"]);

			_VCardId = (int)DBNull2Null(reader["VCardId"]);

			_Number = (string)DBNull2Null(reader["Number"]);

			_TelephoneTypeId = (int)DBNull2Null(reader["TelephoneTypeId"]);

		}

		#region Public Properties

		public virtual int PrimaryKeyId
		{
			get { return _VCardTelephoneId; }
		}


		public virtual int VCardTelephoneId
		{
			get
			{
				return _VCardTelephoneId;
			}

		}

		public virtual int VCardId
		{
			get
			{
				return _VCardId;
			}

			set
			{
				_VCardId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual string Number
		{
			get
			{
				return _Number;
			}

			set
			{
				_Number = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual int TelephoneTypeId
		{
			get
			{
				return _TelephoneTypeId;
			}

			set
			{
				_TelephoneTypeId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		#endregion

		public virtual void Update()
		{
			if (_state == DataRowState.Modified)
			{
				DBHelper2.DBHelper.RunSP("mc_VCardTelephoneUpdate"

					, DBHelper.MP("@VCardTelephoneId", SqlDbType.Int, _VCardTelephoneId)

					, DBHelper.MP("@VCardId", SqlDbType.Int, _VCardId)

					, DBHelper.MP("@Number", SqlDbType.NVarChar, 50, _Number)

					, DBHelper.MP("@TelephoneTypeId", SqlDbType.Int, _TelephoneTypeId)

				);
			}
			else if (_state == DataRowState.Added)
			{
				SqlParameter outPutKey = new SqlParameter("@VCardTelephoneId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DBHelper2.DBHelper.RunSP("mc_VCardTelephoneInsert", outPutKey

					, DBHelper.MP("@VCardId", SqlDbType.Int, _VCardId)

					, DBHelper.MP("@Number", SqlDbType.NVarChar, 50, _Number)

					, DBHelper.MP("@TelephoneTypeId", SqlDbType.Int, _TelephoneTypeId)

				);

				_VCardTelephoneId = (int)outPutKey.Value;
			}
		}

		public virtual void Delete()
		{
			_state = DataRowState.Deleted;

			Delete(_VCardTelephoneId);
		}

		public static void Delete(int VCardTelephoneId)
		{
			DBHelper2.DBHelper.RunSP("mc_VCardTelephoneDelete", DBHelper.MP("@VCardTelephoneId", SqlDbType.Int, VCardTelephoneId));
		}

		public static VCardTelephoneRow[] List(int VCardId)
		{
			ArrayList retVal = new ArrayList();

			using (IDataReader reader = DBHelper2.DBHelper.RunSPDataReader("mc_VCardTelephoneListByVCardId", DBHelper.MP("@VCardId", SqlDbType.Int, VCardId)))
			{
				while (reader.Read())
				{
					retVal.Add(new VCardTelephoneRow(reader));
				}
			}

			return (VCardTelephoneRow[])retVal.ToArray(typeof(VCardTelephoneRow));
		}
	}
}
