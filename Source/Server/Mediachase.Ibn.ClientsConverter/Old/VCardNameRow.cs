using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.Database;


namespace Mediachase.Ibn.Database.VCard
{
	internal class VCardNameRow
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

		int _VCardNameId;
		int _VCardId;
		string _Family;
		string _Given;
		string _Middle;


		public VCardNameRow()
		{
			_state = DataRowState.Added;


		}

		public VCardNameRow(int VCardNameId)
		{
			using (IDataReader reader = DBHelper2.DBHelper.RunSPDataReader("mc_VCardNameSelect", DBHelper.MP("@VCardNameId", SqlDbType.Int, VCardNameId)))
			{
				if (reader.Read())
				{
					Load(reader);
				}
				else throw new VCardRowException("VCardName", "VCardNameId", VCardNameId);
			}
		}

		public VCardNameRow(IDataReader reader)
		{
			Load(reader);
		}

		protected void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;


			_VCardNameId = (int)DBNull2Null(reader["VCardNameId"]);

			_VCardId = (int)DBNull2Null(reader["VCardId"]);

			_Family = (string)DBNull2Null(reader["Family"]);

			_Given = (string)DBNull2Null(reader["Given"]);

			_Middle = (string)DBNull2Null(reader["Middle"]);

		}

		#region Public Properties

		public virtual int PrimaryKeyId
		{
			get { return _VCardNameId; }
		}


		public virtual int VCardNameId
		{
			get
			{
				return _VCardNameId;
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

		public virtual string Family
		{
			get
			{
				return _Family;
			}

			set
			{
				_Family = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual string Given
		{
			get
			{
				return _Given;
			}

			set
			{
				_Given = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual string Middle
		{
			get
			{
				return _Middle;
			}

			set
			{
				_Middle = value;
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
				DBHelper2.DBHelper.RunSP("mc_VCardNameUpdate"

					, DBHelper.MP("@VCardNameId", SqlDbType.Int, _VCardNameId)

					, DBHelper.MP("@VCardId", SqlDbType.Int, _VCardId)

					, DBHelper.MP("@Family", SqlDbType.NVarChar, 255, _Family)

					, DBHelper.MP("@Given", SqlDbType.NVarChar, 255, _Given)

					, DBHelper.MP("@Middle", SqlDbType.NVarChar, 255, _Middle)

				);
			}
			else if (_state == DataRowState.Added)
			{
				SqlParameter outPutKey = new SqlParameter("@VCardNameId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DBHelper2.DBHelper.RunSP("mc_VCardNameInsert", outPutKey

					, DBHelper.MP("@VCardId", SqlDbType.Int, _VCardId)

					, DBHelper.MP("@Family", SqlDbType.NVarChar, 255, _Family)

					, DBHelper.MP("@Given", SqlDbType.NVarChar, 255, _Given)

					, DBHelper.MP("@Middle", SqlDbType.NVarChar, 255, _Middle)

				);

				_VCardNameId = (int)outPutKey.Value;
			}
		}

		public virtual void Delete()
		{
			_state = DataRowState.Deleted;

			Delete(_VCardNameId);
		}

		public static void Delete(int VCardNameId)
		{
			DBHelper2.DBHelper.RunSP("mc_VCardNameDelete", DBHelper.MP("@VCardNameId", SqlDbType.Int, VCardNameId));
		}

		public static VCardNameRow[] List(int VCardId)
		{
			ArrayList retVal = new ArrayList();

			using (IDataReader reader = DBHelper2.DBHelper.RunSPDataReader("mc_VCardNameListByVCardId", DBHelper.MP("@VCardId", SqlDbType.Int, VCardId)))
			{
				while (reader.Read())
				{
					retVal.Add(new VCardNameRow(reader));
				}
			}

			return (VCardNameRow[])retVal.ToArray(typeof(VCardNameRow));
		}
	}
}
