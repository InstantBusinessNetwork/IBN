using System;
using System.Data;

namespace Mediachase.IBN.Database.ControlSystem
{
	/// <summary>
	/// Summary description for DBReportAccessControList.
	/// </summary>
	public class DBReportAccessControList
	{
		private DBReportAccessControList()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="FolderId"></param>
		/// <returns>Reader returns fields: 
		/// AclId, ReportId, AceId, Role, PrincipalId, Action, Allow
		/// </returns>
		public static IDataReader	GetAcl(int ReportId)
		{
			return DbHelper2.RunSpDataReader("fsc_ReportAccessControlEntriesGet",
				DbHelper2.mp("@ReportId", SqlDbType.Int, ReportId));
		}

		public static void Clear(int AclId)
		{
			DbHelper2.RunSp("fsc_ReportAccessControlEntriesClear",
				DbHelper2.mp("@AclId", SqlDbType.Int, AclId));
		}

		public  static int AddAce(int AclId, 
			string Role, 
			int PrincipalId,
			string Action,
			bool Allow,
			bool IsInternal)
		{
			return DbHelper2.RunSpInteger("fsc_ReportAccessControlEntryAdd",
				DbHelper2.mp("@AclId", SqlDbType.Int, AclId),
				DbHelper2.mp("@Role", SqlDbType.NVarChar,50, SqlHelper.Null2DBNull(Role)),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, SqlHelper.Null2DBNull(PrincipalId<=0?null:(object)PrincipalId)),
				DbHelper2.mp("@Action", SqlDbType.NVarChar,50, Action),
				DbHelper2.mp("@Allow", SqlDbType.Bit, Allow),
				DbHelper2.mp("@IsInternal", SqlDbType.Bit, IsInternal));
		}

		public static int GetAclIdByReportId(int ReportId)
		{
			return DbHelper2.RunSpInteger("fsc_ReportAccessControlEntriesGetAclIdByReportId",
				DbHelper2.mp("@ReportId", SqlDbType.Int, ReportId));
		}
	}
}
