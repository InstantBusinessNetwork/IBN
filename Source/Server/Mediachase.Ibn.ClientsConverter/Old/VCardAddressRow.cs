using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.Database;


namespace Mediachase.Ibn.Database.VCard
{
	internal class VCardAddressRow
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

		int _VCardAddressId;
		int _VCardId;
		int _VCardAddressTypeId;
		bool _IsPrefered;
		string _ExtraAddress;
		string _Street;
		string _Locality;
		string _Region;
		string _PostalCode;
		string _Country;


		public VCardAddressRow()
		{
			_state = DataRowState.Added;


		}

		public VCardAddressRow(int VCardAddressId)
		{
			using (IDataReader reader = DBHelper2.DBHelper.RunSPDataReader("mc_VCardAddressSelect", DBHelper.MP("@VCardAddressId", SqlDbType.Int, VCardAddressId)))
			{
				if (reader.Read())
				{
					Load(reader);
				}
				else throw new VCardRowException("VCardAddress", "VCardAddressId", VCardAddressId);
			}
		}

		public VCardAddressRow(IDataReader reader)
		{
			Load(reader);
		}

		protected void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;


			_VCardAddressId = (int)DBNull2Null(reader["VCardAddressId"]);

			_VCardId = (int)DBNull2Null(reader["VCardId"]);

			_VCardAddressTypeId = (int)DBNull2Null(reader["VCardAddressTypeId"]);

			_IsPrefered = (bool)DBNull2Null(reader["IsPrefered"]);

			_ExtraAddress = (string)DBNull2Null(reader["ExtraAddress"]);

			_Street = (string)DBNull2Null(reader["Street"]);

			_Locality = (string)DBNull2Null(reader["Locality"]);

			_Region = (string)DBNull2Null(reader["Region"]);

			_PostalCode = (string)DBNull2Null(reader["PostalCode"]);

			_Country = (string)DBNull2Null(reader["Country"]);

		}

		#region Public Properties

		public virtual int PrimaryKeyId
		{
			get { return _VCardAddressId; }
		}


		public virtual int VCardAddressId
		{
			get
			{
				return _VCardAddressId;
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

		public virtual int VCardAddressTypeId
		{
			get
			{
				return _VCardAddressTypeId;
			}

			set
			{
				_VCardAddressTypeId = value;
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

		public virtual string ExtraAddress
		{
			get
			{
				return _ExtraAddress;
			}

			set
			{
				_ExtraAddress = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual string Street
		{
			get
			{
				return _Street;
			}

			set
			{
				_Street = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual string Locality
		{
			get
			{
				return _Locality;
			}

			set
			{
				_Locality = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual string Region
		{
			get
			{
				return _Region;
			}

			set
			{
				_Region = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual string PostalCode
		{
			get
			{
				return _PostalCode;
			}

			set
			{
				_PostalCode = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual string Country
		{
			get
			{
				return _Country;
			}

			set
			{
				_Country = value;
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
				DBHelper2.DBHelper.RunSP("mc_VCardAddressUpdate"

					, DBHelper.MP("@VCardAddressId", SqlDbType.Int, _VCardAddressId)

					, DBHelper.MP("@VCardId", SqlDbType.Int, _VCardId)

					, DBHelper.MP("@VCardAddressTypeId", SqlDbType.Int, _VCardAddressTypeId)

					, DBHelper.MP("@IsPrefered", SqlDbType.Bit, _IsPrefered)

					, DBHelper.MP("@ExtraAddress", SqlDbType.NText, _ExtraAddress)

					, DBHelper.MP("@Street", SqlDbType.NText, _Street)

					, DBHelper.MP("@Locality", SqlDbType.NText, _Locality)

					, DBHelper.MP("@Region", SqlDbType.NVarChar, 255, _Region)

					, DBHelper.MP("@PostalCode", SqlDbType.NVarChar, 50, _PostalCode)

					, DBHelper.MP("@Country", SqlDbType.NVarChar, 50, _Country)

				);
			}
			else if (_state == DataRowState.Added)
			{
				SqlParameter outPutKey = new SqlParameter("@VCardAddressId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DBHelper2.DBHelper.RunSP("mc_VCardAddressInsert", outPutKey

					, DBHelper.MP("@VCardId", SqlDbType.Int, _VCardId)

					, DBHelper.MP("@VCardAddressTypeId", SqlDbType.Int, _VCardAddressTypeId)

					, DBHelper.MP("@IsPrefered", SqlDbType.Bit, _IsPrefered)

					, DBHelper.MP("@ExtraAddress", SqlDbType.NText, _ExtraAddress)

					, DBHelper.MP("@Street", SqlDbType.NText, _Street)

					, DBHelper.MP("@Locality", SqlDbType.NText, _Locality)

					, DBHelper.MP("@Region", SqlDbType.NVarChar, 255, _Region)

					, DBHelper.MP("@PostalCode", SqlDbType.NVarChar, 50, _PostalCode)

					, DBHelper.MP("@Country", SqlDbType.NVarChar, 50, _Country)

				);

				_VCardAddressId = (int)outPutKey.Value;
			}
		}

		public virtual void Delete()
		{
			_state = DataRowState.Deleted;

			Delete(_VCardAddressId);
		}

		public static void Delete(int VCardAddressId)
		{
			DBHelper2.DBHelper.RunSP("mc_VCardAddressDelete", DBHelper.MP("@VCardAddressId", SqlDbType.Int, VCardAddressId));
		}

		public static VCardAddressRow[] List(int VCardId)
		{
			ArrayList retVal = new ArrayList();

			using (IDataReader reader = DBHelper2.DBHelper.RunSPDataReader("mc_VCardAddressListByVCardId", DBHelper.MP("@VCardId", SqlDbType.Int, VCardId)))
			{
				while (reader.Read())
				{
					retVal.Add(new VCardAddressRow(reader));
				}
			}

			return (VCardAddressRow[])retVal.ToArray(typeof(VCardAddressRow));
		}
	}
}
