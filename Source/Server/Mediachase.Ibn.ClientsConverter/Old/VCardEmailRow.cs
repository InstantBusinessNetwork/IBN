using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.Database;


namespace Mediachase.Ibn.Database.VCard
{
	internal class VCardEmailRow
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

		int _VCardEMailId;
		int _VCardId;
		string _UserId;
		int _EmailTypeId;
		bool _IsPrefered;


		public VCardEmailRow()
		{
			_state = DataRowState.Added;


		}

		public VCardEmailRow(int VCardEMailId)
		{
			using (IDataReader reader = DBHelper2.DBHelper.RunSPDataReader("mc_VCardEMailSelect", DBHelper.MP("@VCardEMailId", SqlDbType.Int, VCardEMailId)))
			{
				if (reader.Read())
				{
					Load(reader);
				}
				else throw new VCardRowException("VCardEmail", "VCardEMailId", VCardEMailId);
			}
		}

		public VCardEmailRow(IDataReader reader)
		{
			Load(reader);
		}

		protected void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;


			_VCardEMailId = (int)DBNull2Null(reader["VCardEMailId"]);

			_VCardId = (int)DBNull2Null(reader["VCardId"]);

			_UserId = (string)DBNull2Null(reader["UserId"]);

			_EmailTypeId = (int)DBNull2Null(reader["EmailTypeId"]);

			_IsPrefered = (bool)DBNull2Null(reader["IsPrefered"]);

		}

		#region Public Properties

		public virtual int PrimaryKeyId
		{
			get { return _VCardEMailId; }
		}


		public virtual int VCardEMailId
		{
			get
			{
				return _VCardEMailId;
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

		public virtual string UserId
		{
			get
			{
				return _UserId;
			}

			set
			{
				_UserId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual int EmailTypeId
		{
			get
			{
				return _EmailTypeId;
			}

			set
			{
				_EmailTypeId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual bool IsPrefered
		{
			get
			{
				return _IsPrefered;
			}

			set
			{
				_IsPrefered = value;
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
				DBHelper2.DBHelper.RunSP("mc_VCardEMailUpdate"

					, DBHelper.MP("@VCardEMailId", SqlDbType.Int, _VCardEMailId)

					, DBHelper.MP("@VCardId", SqlDbType.Int, _VCardId)

					, DBHelper.MP("@UserId", SqlDbType.NVarChar, 255, _UserId)

					, DBHelper.MP("@EmailTypeId", SqlDbType.Int, _EmailTypeId)

					, DBHelper.MP("@IsPrefered", SqlDbType.Bit, _IsPrefered)

				);
			}
			else if (_state == DataRowState.Added)
			{
				SqlParameter outPutKey = new SqlParameter("@VCardEMailId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DBHelper2.DBHelper.RunSP("mc_VCardEMailInsert", outPutKey

					, DBHelper.MP("@VCardId", SqlDbType.Int, _VCardId)

					, DBHelper.MP("@UserId", SqlDbType.NVarChar, 255, _UserId)

					, DBHelper.MP("@EmailTypeId", SqlDbType.Int, _EmailTypeId)

					, DBHelper.MP("@IsPrefered", SqlDbType.Bit, _IsPrefered)

				);

				_VCardEMailId = (int)outPutKey.Value;
			}
		}

		public virtual void Delete()
		{
			_state = DataRowState.Deleted;

			Delete(_VCardEMailId);
		}

		public static void Delete(int VCardEMailId)
		{
			DBHelper2.DBHelper.RunSP("mc_VCardEMailDelete", DBHelper.MP("@VCardEMailId", SqlDbType.Int, VCardEMailId));
		}

		public static VCardEmailRow[] List(int VCardId)
		{
			ArrayList retVal = new ArrayList();

			using (IDataReader reader = DBHelper2.DBHelper.RunSPDataReader("mc_VCardEMailListByVCardId", DBHelper.MP("@VCardId", SqlDbType.Int, VCardId)))
			{
				while (reader.Read())
				{
					retVal.Add(new VCardEmailRow(reader));
				}
			}

			return (VCardEmailRow[])retVal.ToArray(typeof(VCardEmailRow));
		}
	}
}
