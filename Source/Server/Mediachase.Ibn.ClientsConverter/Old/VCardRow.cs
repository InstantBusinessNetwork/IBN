using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.Database;


namespace Mediachase.Ibn.Database.VCard
{
	internal class VCardRow
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

		static object DBNull2Null(object Value, object NullValue)
		{
			if (Value == DBNull.Value)
				return NullValue;
			return Value;
		}
		#endregion

		int _VCardId;
		string _FullName;
		string _NickName = string.Empty;
		string _Url = string.Empty;

		DateTime _Birthday;
		string _Title = string.Empty;
		string _Role = string.Empty;
		string _Description = string.Empty;
		string _Gender = string.Empty;

		int _OrganizationId;


		public VCardRow()
		{
			_state = DataRowState.Added;

			_Birthday = DateTime.MinValue;
			_OrganizationId = -1;
		}

		public VCardRow(int VCardId)
		{
			using (IDataReader reader = DBHelper2.DBHelper.RunSPDataReader("mc_VCardSelect", DBHelper.MP("@VCardId", SqlDbType.Int, VCardId)))
			{
				if (reader.Read())
				{
					Load(reader);
				}
				else throw new VCardRowException("VCard", "VCardId", VCardId);
			}
		}

		public VCardRow(IDataReader reader)
		{
			Load(reader);
		}

		protected void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;


			_VCardId = (int)DBNull2Null(reader["VCardId"]);

			_FullName = (string)DBNull2Null(reader["FullName"]);

			_NickName = (string)DBNull2Null(reader["NickName"]);

			_Url = (string)DBNull2Null(reader["Url"]);

			_Birthday = (DateTime)DBNull2Null(reader["Birthday"], DateTime.MinValue);

			_Title = (string)DBNull2Null(reader["Title"]);

			_Role = (string)DBNull2Null(reader["Role"]);

			_Description = (string)DBNull2Null(reader["Description"]);

			_Gender = (string)DBNull2Null(reader["Gender"]);

			_OrganizationId = (int)DBNull2Null(reader["OrganizationId"], -1);

		}

		#region Public Properties

		public virtual int PrimaryKeyId
		{
			get { return _VCardId; }
		}


		public virtual int VCardId
		{
			get
			{
				return _VCardId;
			}

		}

		public virtual string FullName
		{
			get
			{
				return _FullName;
			}

			set
			{
				_FullName = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual string NickName
		{
			get
			{
				return _NickName;
			}

			set
			{
				_NickName = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual string Url
		{
			get
			{
				return _Url;
			}

			set
			{
				_Url = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual DateTime Birthday
		{
			get
			{
				return _Birthday;
			}

			set
			{
				_Birthday = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual string Title
		{
			get
			{
				return _Title;
			}

			set
			{
				_Title = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual string Role
		{
			get
			{
				return _Role;
			}

			set
			{
				_Role = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual string Description
		{
			get
			{
				return _Description;
			}

			set
			{
				_Description = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual string Gender
		{
			get
			{
				return _Gender;
			}

			set
			{
				_Gender = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}

		}

		public virtual int OrganizationId
		{
			get
			{
				return _OrganizationId;
			}

			set
			{
				_OrganizationId = value;
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
				DBHelper2.DBHelper.RunSP("mc_VCardUpdate"

					, DBHelper.MP("@VCardId", SqlDbType.Int, _VCardId)

					, DBHelper.MP("@FullName", SqlDbType.NVarChar, 50, _FullName)

					, DBHelper.MP("@NickName", SqlDbType.NVarChar, 50, _NickName)

					, DBHelper.MP("@Url", SqlDbType.NVarChar, 255, _Url)

					, DBHelper.MP("@Birthday", SqlDbType.DateTime, _Birthday == DateTime.MinValue ? DBNull.Value : (object)_Birthday)

					, DBHelper.MP("@Title", SqlDbType.NText, _Title)

					, DBHelper.MP("@Role", SqlDbType.NText, _Role)

					, DBHelper.MP("@Description", SqlDbType.NText, _Description)

					, DBHelper.MP("@Gender", SqlDbType.NVarChar, 15, _Gender)

					, DBHelper.MP("@OrganizationId", SqlDbType.Int, _OrganizationId < 0 ? DBNull.Value : (object)_OrganizationId)

				);
			}
			else if (_state == DataRowState.Added)
			{
				SqlParameter outPutKey = new SqlParameter("@VCardId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DBHelper2.DBHelper.RunSP("mc_VCardInsert", outPutKey

					, DBHelper.MP("@FullName", SqlDbType.NVarChar, 50, _FullName)

					, DBHelper.MP("@NickName", SqlDbType.NVarChar, 50, _NickName)

					, DBHelper.MP("@Url", SqlDbType.NVarChar, 255, _Url)

					, DBHelper.MP("@Birthday", SqlDbType.DateTime, _Birthday == DateTime.MinValue ? DBNull.Value : (object)_Birthday)

					, DBHelper.MP("@Title", SqlDbType.NText, _Title)

					, DBHelper.MP("@Role", SqlDbType.NText, _Role)

					, DBHelper.MP("@Description", SqlDbType.NText, _Description)

					, DBHelper.MP("@Gender", SqlDbType.NVarChar, 15, _Gender)

					, DBHelper.MP("@OrganizationId", SqlDbType.Int, _OrganizationId < 0 ? DBNull.Value : (object)_OrganizationId)
				);

				_VCardId = (int)outPutKey.Value;
			}
		}

		public virtual void Delete()
		{
			_state = DataRowState.Deleted;

			Delete(_VCardId);
		}

		public static void Delete(int VCardId)
		{
			DBHelper2.DBHelper.RunSP("mc_VCardDelete", DBHelper.MP("@VCardId", SqlDbType.Int, VCardId));
		}

		public static VCardRow[] List()
		{
			ArrayList retVal = new ArrayList();

			using (IDataReader reader = DBHelper2.DBHelper.RunSPDataReader("mc_VCardList"))
			{
				while (reader.Read())
				{
					retVal.Add(new VCardRow(reader));
				}
			}

			return (VCardRow[])retVal.ToArray(typeof(VCardRow));

		}

		public static VCardRow[] List(int OrganizationId)
		{
			ArrayList retVal = new ArrayList();

			using (IDataReader reader = DBHelper2.DBHelper.RunSPDataReader("mc_VCardListByOrganization",
					  DBHelper.MP("@OrganizationId", SqlDbType.Int, OrganizationId)))
			{
				while (reader.Read())
				{
					retVal.Add(new VCardRow(reader));
				}
			}

			return (VCardRow[])retVal.ToArray(typeof(VCardRow));

		}

		public static VCardRow[] List(string Substring)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Substring = DBCommon.ReplaceSqlWildcard(Substring);
			//

			ArrayList retVal = new ArrayList();

			using (IDataReader reader = DBHelper2.DBHelper.RunSPDataReader("mc_VCardListBySubstring",
					  DBHelper.MP("@SearchStr", SqlDbType.NVarChar, 100, Substring)))
			{
				while (reader.Read())
				{
					retVal.Add(new VCardRow(reader));
				}
			}

			return (VCardRow[])retVal.ToArray(typeof(VCardRow));
		}
	}
}
