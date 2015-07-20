using System;
using System.Data;
using System.Data.SqlClient;

using Mediachase.MetaDataPlus.Common;


namespace Mediachase.MetaDataPlus.Configurator
{
	/// <summary>
	/// Summary description for MetaType.
	/// </summary>
	public class MetaType
	{
		private int _id;
		private string _name = string.Empty;
		private string _friendlyName = string.Empty;
		private string _description = string.Empty;
		private int _length;
		private string _sqlName = string.Empty;
		private bool _allowNulls = true;
		private bool _variable = true;
		private bool _isSqlCommonType = true;

		protected MetaType()
		{
		}

		protected static MetaType Load(IDataRecord reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			MetaType retVal = new MetaType();

			retVal._id = (int)reader["DataTypeId"];
			retVal._name = (string)reader["Name"];
			retVal._friendlyName = (string)reader["FriendlyName"];

			if (reader["Description"] != DBNull.Value)
				retVal._description = (string)reader["Description"];

			retVal._length = (int)reader["Length"];
			retVal._sqlName = (string)reader["SqlName"];
			retVal._allowNulls = (bool)reader["AllowNulls"];
			retVal._variable = (bool)reader["Variable"];
			retVal._isSqlCommonType = (bool)reader["IsSQLCommonType"];

			return retVal;
		}

		public static MetaType Load(int metaTypeId)
		{
			MetaType retVal = null;
			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaType"), new SqlParameter("@MetaTypeId", metaTypeId)))
			{
				if (reader.Read())
				{
					retVal = MetaType.Load(reader);
				}
				reader.Close();
			}

			return retVal;

		}

		public static MetaType Load(MetaDataType type)
		{
			return MetaType.Load((int)type);
		}

		public static MetaTypeCollection GetMetaTypes()
		{
			MetaTypeCollection retVal = new MetaTypeCollection();
			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaTypeList")))
			{
				while (reader.Read())
				{
					MetaType newType = MetaType.Load(reader);
					retVal.Add(newType);
				}
				reader.Close();
			}

			return retVal;

		}

		public static MetaTypeCollection GetSqlMetaTypes()
		{
			MetaTypeCollection retVal = new MetaTypeCollection();
			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaTypeList")))
			{
				while (reader.Read())
				{
					MetaType newType = MetaType.Load(reader);
					if (newType.IsSqlCommonType)
						retVal.Add(newType);
				}
				reader.Close();
			}

			return retVal;
		}

		public static MetaTypeCollection GetMetaDataMetaTypes()
		{
			MetaTypeCollection retVal = new MetaTypeCollection();
			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaTypeList")))
			{
				while (reader.Read())
				{
					MetaType newType = MetaType.Load(reader);
					if (!newType.IsSqlCommonType)
						retVal.Add(newType);
				}
				reader.Close();
			}

			return retVal;
		}


		//		public static MetaTypeCollection	GetCommonMetaTypes()
		//		{
		//			MetaTypeCollection	retVal = new MetaTypeCollection();
		//
		//			MetaTypeCollection	fullList	=	MetaType.GetMetaTypes();
		//
		//			foreach(MetaType item in fullList)
		//			{
		//				if(!item.IsSQLCommonType||
		//					String.Compare(item.Name,"Boolean",true)==0||
		//					String.Compare(item.Name,"Money",true)==0 ||
		//					String.Compare(item.Name,"DateTime",true)==0)
		//					retVal.Add(item);
		//			}
		//
		//			return retVal;
		//		}

		public int Id
		{
			get
			{
				return _id;
			}
		}

		public MetaDataContext Context
		{
			get
			{
				return MetaDataContext.Current;
			}
		}

		public MetaDataType MetaDataType
		{
			get
			{
				return (MetaDataType)_id;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public string FriendlyName
		{
			get
			{
				return _friendlyName;
			}
		}


		public string Description
		{
			get
			{
				return _description;
			}
		}

		public int Length
		{
			get
			{
				return _length;
			}
		}

		public string SqlName
		{
			get
			{
				return _sqlName;
			}
		}

		public bool AllowNulls
		{
			get
			{
				return _allowNulls;
			}
		}

		public bool Variable
		{
			get
			{
				return _variable;
			}
		}

		public bool IsSqlCommonType
		{
			get
			{
				return _isSqlCommonType;
			}
		}

	}
}
