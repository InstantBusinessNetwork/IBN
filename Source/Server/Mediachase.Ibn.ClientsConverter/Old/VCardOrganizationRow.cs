using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.Database;


namespace Mediachase.Ibn.Database.VCard
{
	internal class VCardOrganizationRow
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

		int _VCardOrganizationId;
		int _VCardId;
		string _Name;
		string _Unit;


		public VCardOrganizationRow()
		{
			_state = DataRowState.Added;


		}

		public VCardOrganizationRow(int VCardOrganizationId)
		{
			using (IDataReader reader = DBHelper2.DBHelper.RunSPDataReader("mc_VCardOrganizationSelect", DBHelper.MP("@VCardOrganizationId", SqlDbType.Int, VCardOrganizationId)))
			{
				if (reader.Read())
				{
					Load(reader);
				}
				else throw new VCardRowException("VCardOrganization", "VCardOrganizationId", VCardOrganizationId);
			}
		}

		public VCardOrganizationRow(IDataReader reader)
		{
			Load(reader);
		}

		protected void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;


			_VCardOrganizationId = (int)DBNull2Null(reader["VCardOrganizationId"]);

			_VCardId = (int)DBNull2Null(reader["VCardId"]);

			_Name = (string)DBNull2Null(reader["Name"]);

			_Unit = (string)DBNull2Null(reader["Unit"]);

		}

		#region Public Properties

		public virtual int PrimaryKeyId
		{
			get { return _VCardOrganizationId; }
		}


		public virtual int VCardOrganizationId
		{
			get
			{
				return _VCardOrganizationId;
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

		public virtual string Unit
		{
			get
			{
				return _Unit;
			}

			set
			{
				_Unit = value;
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
				DBHelper2.DBHelper.RunSP("mc_VCardOrganizationUpdate"

					, DBHelper.MP("@VCardOrganizationId", SqlDbType.Int, _VCardOrganizationId)

					, DBHelper.MP("@VCardId", SqlDbType.Int, _VCardId)

					, DBHelper.MP("@Name", SqlDbType.NText, _Name)

					, DBHelper.MP("@Unit", SqlDbType.NText, _Unit)

				);
			}
			else if (_state == DataRowState.Added)
			{
				SqlParameter outPutKey = new SqlParameter("@VCardOrganizationId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DBHelper2.DBHelper.RunSP("mc_VCardOrganizationInsert", outPutKey

					, DBHelper.MP("@VCardId", SqlDbType.Int, _VCardId)

					, DBHelper.MP("@Name", SqlDbType.NText, _Name)

					, DBHelper.MP("@Unit", SqlDbType.NText, _Unit)

				);

				_VCardOrganizationId = (int)outPutKey.Value;
			}
		}

		public virtual void Delete()
		{
			_state = DataRowState.Deleted;

			Delete(_VCardOrganizationId);
		}

		public static void Delete(int VCardOrganizationId)
		{
			DBHelper2.DBHelper.RunSP("mc_VCardOrganizationDelete", DBHelper.MP("@VCardOrganizationId", SqlDbType.Int, VCardOrganizationId));
		}

		public static VCardOrganizationRow[] List(int VCardId)
		{
			ArrayList retVal = new ArrayList();

			using (IDataReader reader = DBHelper2.DBHelper.RunSPDataReader("mc_VCardOrganizationListByVCardId", DBHelper.MP("@VCardId", SqlDbType.Int, VCardId)))
			{
				while (reader.Read())
				{
					retVal.Add(new VCardOrganizationRow(reader));
				}
			}

			return (VCardOrganizationRow[])retVal.ToArray(typeof(VCardOrganizationRow));
		}
	}
}
