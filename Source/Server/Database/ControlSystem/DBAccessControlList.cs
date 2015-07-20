using System;
using System.Data;

namespace Mediachase.IBN.Database.ControlSystem
{
	/// <summary>
	/// Represents data base access to access control list.
	/// </summary>
	public static class DBAccessControlList
	{
		/// <summary>
		/// Gets the acl.
		/// </summary>
		/// <param name="directoryId">The directory id.</param>
		/// <returns>
		/// Reader returns fields:
		/// AclId, IsInherited as AclIsInherited, FolderId,
		/// AceId, IsInherited, Role, PrincipalId, Action, Allow, OwnerKey
		/// </returns>
		public static IDataReader	GetAcl(int directoryId)
		{
			return DbHelper2.RunSpDataReader("fsc_AccessControlEntriesGet",
				DbHelper2.mp("@DirectoryId", SqlDbType.Int, directoryId));
		}

		/// <summary>
		/// Clears the specified acl id.
		/// </summary>
		/// <param name="aclId">The acl id.</param>
		public static void Clear(int aclId)
		{
			DbHelper2.RunSp("fsc_AccessControlEntriesClear",
				DbHelper2.mp("@AclId", SqlDbType.Int, aclId));
		}

		/// <summary>
		/// Adds the ace.
		/// </summary>
		/// <param name="aclId">The acl id.</param>
		/// <param name="role">The role.</param>
		/// <param name="principalId">The principal id.</param>
		/// <param name="action">The action.</param>
		/// <param name="allow">if set to <c>true</c> [allow].</param>
		/// <param name="isInternal">if set to <c>true</c> [is internal].</param>
		/// <param name="ownerKey">The owner key.</param>
		/// <returns></returns>
		public  static int AddAce(int aclId, 
			string role, 
			int principalId,
			string action,
			bool allow,
			bool isInternal,
			Guid ownerKey)
		{
			return DbHelper2.RunSpInteger("fsc_AccessControlEntryAdd",
				DbHelper2.mp("@AclId", SqlDbType.Int, aclId),
				DbHelper2.mp("@Role", SqlDbType.NVarChar,50, SqlHelper.Null2DBNull(role)),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, SqlHelper.Null2DBNull(principalId<=0?null:(object)principalId)),
				DbHelper2.mp("@Action", SqlDbType.NVarChar,50, action),
				DbHelper2.mp("@Allow", SqlDbType.Bit, allow),
				DbHelper2.mp("@IsInternal", SqlDbType.Bit, isInternal),
				DbHelper2.mp("@OwnerKey", SqlDbType.UniqueIdentifier, SqlHelper.Null2DBNull(ownerKey==Guid.Empty?null:(object)ownerKey)));
		}

		/// <summary>
		/// Turns the off is inherited.
		/// </summary>
		/// <param name="aclId">The acl id.</param>
		/// <param name="copy">if set to <c>true</c> [copy].</param>
		public static void TurnOffIsInherited(int aclId, bool copy)
		{
			DbHelper2.RunSp("fsc_AccessControlListTurnOffInheritFromParent",
				DbHelper2.mp("@AclId", SqlDbType.Int, aclId),
				DbHelper2.mp("@Copy", SqlDbType.Bit, copy));
		}

		/// <summary>
		/// Turns the on is inherited.
		/// </summary>
		/// <param name="AclId">The acl id.</param>
		public static void TurnOnIsInherited(int aclId)
		{
			DbHelper2.RunSp("fsc_AccessControlListTurnOnInheritFromParent",
				DbHelper2.mp("@AclId", SqlDbType.Int, aclId));
		}

		/// <summary>
		/// Refreshes the inherited ACL.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		public static void RefreshInheritedACL(int directoryId)
		{
			DbHelper2.RunSp("fsc_DirectoryRefreshInheritedACL",
				DbHelper2.mp("@DirectoryId", SqlDbType.Int, directoryId));
		}

		/// <summary>
		/// Gets the acl id by directory id.
		/// </summary>
		/// <param name="directoryId">The directory id.</param>
		/// <returns></returns>
		public static int GetAclIdByDirectoryId(int directoryId)
		{
			return DbHelper2.RunSpInteger("fsc_AccessControlEntriesGetAclIdByDirectoryId",
				DbHelper2.mp("@DirectoryId", SqlDbType.Int, directoryId));
		}
	}
}
