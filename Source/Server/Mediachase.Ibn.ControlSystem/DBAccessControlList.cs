using System;
using System.Data;

using Mediachase.Database;


namespace Mediachase.Ibn.ControlSystem
{
	/// <summary>
	/// Summary description for DBAccessControlList.
	/// </summary>
	public class DBAccessControlList
	{
		private DBAccessControlList()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="FolderId"></param>
		/// <returns>Reader returns fields: 
		/// AclId, IsInherited as AclIsInherited, FolderId, 
		/// AceId, IsInherited, Role, PrincipalId, Action, Allow
		/// </returns>
		public static IDataReader	GetAcl(int DirectoryId)
		{
			return DBHelper2.DBHelper.RunSPDataReader("fsc_AccessControlEntriesGet",
				DBHelper.MP("@DirectoryId", SqlDbType.Int, DirectoryId));
		}

		public static void Clear(int AclId)
		{
			DBHelper2.DBHelper.RunSP("fsc_AccessControlEntriesClear",
				DBHelper.MP("@AclId", SqlDbType.Int, AclId));
		}

		public static int AddAce(int AclId, 
			string Role, 
			int PrincipalId,
			string Action,
			bool Allow,
			bool IsInternal)
		{
			return DBHelper2.DBHelper.RunSPInteger("fsc_AccessControlEntryAdd",
				DBHelper.MP("@AclId", SqlDbType.Int, AclId),
				DBHelper.MP("@Role", SqlDbType.NVarChar, 50, SqlHelper.Null2DBNull(Role)),
				DBHelper.MP("@PrincipalId", SqlDbType.Int, SqlHelper.Null2DBNull(PrincipalId <= 0 ? null : (object)PrincipalId)),
				DBHelper.MP("@Action", SqlDbType.NVarChar, 50, Action),
				DBHelper.MP("@Allow", SqlDbType.Bit, Allow),
				DBHelper.MP("@IsInternal", SqlDbType.Bit, IsInternal));
		}

		public static void TurnOffIsInherited(int AclId, bool Copy)
		{
			DBHelper2.DBHelper.RunSP("fsc_AccessControlListTurnOffInheritFromParent",
				DBHelper.MP("@AclId", SqlDbType.Int, AclId),
				DBHelper.MP("@Copy", SqlDbType.Bit, Copy));
		}

		public static void TurnOnIsInherited(int AclId)
		{
			DBHelper2.DBHelper.RunSP("fsc_AccessControlListTurnOnInheritFromParent",
				DBHelper.MP("@AclId", SqlDbType.Int, AclId));
		}

		public static void RefreshInheritedACL(int DirectoryId)
		{
			DBHelper2.DBHelper.RunSP("fsc_DirectoryRefreshInheritedACL",
				DBHelper.MP("@DirectoryId", SqlDbType.Int, DirectoryId));
		}

		public static int GetAclIdByDirectoryId(int DirectoryId)
		{
			return DBHelper2.DBHelper.RunSPInteger("fsc_AccessControlEntriesGetAclIdByDirectoryId",
				DBHelper.MP("@DirectoryId", SqlDbType.Int, DirectoryId));
		}
	}
}
