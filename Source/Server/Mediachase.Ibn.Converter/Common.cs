using System;
using System.Collections.Generic;
using System.Data;

using Mediachase.Database;


namespace Mediachase.Ibn.Converter
{
	public sealed class Helper
	{
		#region NullToObject()
		public static object NullToObject(object value, object defaultValue)
		{
			if (value != null && value != DBNull.Value)
				return value;
			else
				return defaultValue;
		}
		#endregion
		#region NullToDateTime()
		public static DateTime NullToDateTime(object value)
		{
			return (DateTime)NullToObject(value, DateTime.MinValue);
		}
		#endregion
		#region NullToInt32()
		public static Int32 NullToInt32(object value)
		{
			return (Int32)NullToObject(value, -1);
		}
		#endregion
		#region NullToNulableInt32()
		public static Int32? NullToNulableInt32(object value)
		{
			return (Int32?)NullToObject(value, null);
		}
		#endregion

		private Helper()
		{
		}

		#region public static void SaveRolePrincipals(DBHelper target, string tableName, int objectId, int roleId, IEnumerable<int> principals)
		public static void SaveRolePrincipals(DBHelper target, string tableName, int objectId, int roleId, IEnumerable<int> principals)
		{
			foreach (int principalId in principals)
			{
				target.RunText(string.Concat("INSERT INTO [", tableName, "] ([ObjectId],[PrincipalId],[RoleId]) VALUES (@p1,@p2,@p3)")
					, DBHelper.MP("@p1", SqlDbType.Int, objectId)
					, DBHelper.MP("@p2", SqlDbType.Int, principalId)
					, DBHelper.MP("@p3", SqlDbType.Int, roleId)
					);
			}
		}
		#endregion

		#region public static void SaveRolePrincipals(DBHelper target, string tableName, int objectId, int roleId, int principalId)
		public static void SaveRolePrincipals(DBHelper target, string tableName, int objectId, int roleId, int principalId)
		{
			SaveRolePrincipals(target, tableName, objectId, roleId, new int[] { principalId });
		}
		#endregion
	}
}
