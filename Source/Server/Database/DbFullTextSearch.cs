using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DbFullTextSearch.
	/// </summary>
	public class DbFullTextSearch
	{
		private DbFullTextSearch()
		{
		}

		public static IDataReader IsInstalled()
		{
			return DbHelper2.RunSpDataReader("fsc_FullTextSearchIsInstalled");
		}

		public static void Activate()
		{
			DbHelper2.RunSp("fsc_FullTextSearchActivate");
		}

		public static void Deactivate()
		{
			DbHelper2.RunSp("fsc_FullTextSearchDeactivate");
		}

		public static IDataReader GetActive()
		{
			return DbHelper2.RunSpDataReader("fsc_FullTextSearchEnabled");
		}

		public static IDataReader GetInfo()
		{
			return DbHelper2.RunSpDataReader("fsc_FullTextSearchGetInfo");
		}

	}
}
