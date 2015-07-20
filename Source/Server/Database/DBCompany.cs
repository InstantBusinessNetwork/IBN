using System;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBCompany.
	/// </summary>
	public class DBCompany
	{
		#region GetDatabaseSize
		public static long GetDatabaseSize()
		{
			return DbHelper2.RunSpLong("DatabaseGetSize");
		}
		#endregion
	}
}
