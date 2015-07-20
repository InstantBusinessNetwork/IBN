using System;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBPrincipal.
	/// </summary>
	public class DBPrincipal
	{
		public static IDataReader GetPrincipal(int PrincipalId)
		{
			return DbHelper2.RunSpDataReader("PrincipalGet",
				DbHelper2.mp("@PrincipalId",SqlDbType.Int,PrincipalId));
		}

		public static bool IsGroup(int PrincipalId)
		{
			return (DbHelper2.RunSpInteger("PrincipalGetIsGroup",
				DbHelper2.mp("@PrincipalId",SqlDbType.Int,PrincipalId))==0?false:true);
		}
	}
}
