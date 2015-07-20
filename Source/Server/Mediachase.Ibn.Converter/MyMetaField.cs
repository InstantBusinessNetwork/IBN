using System;
using System.Collections;
using System.Globalization;
using System.Data;

using Mediachase.Database;


namespace Mediachase.Ibn.Converter
{
	internal class MyMetaField
	{
		public string Name;
		public string SqlName;
		public string DefaultValue;
		public int Length;
		public bool AllowNulls;
		public bool Variable;

		#region LoadList()
		public static void LoadList(DBHelper source, int metaClassId, ArrayList fields)
		{
			using (IDataReader reader = source.RunTextDataReader(string.Format(CultureInfo.InvariantCulture, "SELECT F.Name, F.Length, T.SqlName, F.AllowNulls, T.Variable, T.DefaultValue FROM MetaClassMetaFieldRelation R JOIN MetaField F ON F.MetaFieldId = R.MetaFieldId JOIN MetaDataType T ON T.DataTypeId = F.DataTypeId WHERE R.MetaClassId = {0} AND F.SystemMetaClassId = 0 ORDER BY F.MetaFieldId", metaClassId)))
			{
				while (reader.Read())
				{
					MyMetaField item = Load(reader);
					fields.Add(item);
				}
			}
		}
		#endregion

		#region Load()
		public static MyMetaField Load(IDataRecord reader)
		{
			MyMetaField ret = new MyMetaField();

			ret.Name = reader["Name"].ToString();
			ret.SqlName = reader["SqlName"].ToString();
			ret.DefaultValue = reader["DefaultValue"].ToString();
			ret.Length = (int)reader["Length"];
			ret.AllowNulls = (bool)reader["AllowNulls"];
			ret.Variable = (bool)reader["Variable"];

			return ret;
		}
		#endregion

		private MyMetaField()
		{
		}
	}
}
