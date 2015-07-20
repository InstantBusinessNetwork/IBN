using System;
using System.Data;

namespace Mediachase.IBN.Database.ControlSystem
{
	/// <summary>
	/// Summary description for DBUserRole.
	/// </summary>
	public class DBUserRole
	{
		#region -- Create --
		public static int Add(int PrincipalId, string Role)
		{
			return DbHelper2.RunSpInteger("fsc_UserRoleAdd",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId),
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, null),
				DbHelper2.mp("@Role", SqlDbType.NVarChar, 50, Role));
		}

		public static int Add(int PrincipalId, string ContainerKey, string Role)
		{
			return DbHelper2.RunSpInteger("fsc_UserRoleAdd",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId),
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey),
				DbHelper2.mp("@Role", SqlDbType.NVarChar, 50, Role));
		}
		#endregion

		#region -- Delete --
		public static void DeleteByUser(int PrincipalId, string ContainerKey, string Role)
		{
			DbHelper2.RunSp("fsc_UserRoleDeleteByUser",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId),
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey),
				DbHelper2.mp("@Role", SqlDbType.NVarChar, 50, Role));
		}

		public static void DeleteByUser(int PrincipalId, string Role)
		{
			DbHelper2.RunSp("fsc_UserRoleDeleteByUser",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId),
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, null),
				DbHelper2.mp("@Role", SqlDbType.NVarChar, 50, Role));
		}

		public static void DeleteByContainerKey(string ContainerKey)
		{
			DbHelper2.RunSp("fsc_UserRoleDeleteByContainerKey",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey));
		}

		public static void DeleteByRole(string ContainerKey, string Role)
		{
			DbHelper2.RunSp("fsc_UserRoleDeleteByRole",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey),
				DbHelper2.mp("@Role", SqlDbType.NVarChar, 50, Role));
		}

		public static void DeleteByRole(string Role)
		{
			DbHelper2.RunSp("fsc_UserRoleDeleteByRole",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, null),
				DbHelper2.mp("@Role", SqlDbType.NVarChar, 50, Role));
		}
		#endregion

		#region -- List --
		public static IDataReader ListByRole(string Role)
		{
			return DbHelper2.RunSpDataReader("fsc_UserRoleListByRole",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, null),
				DbHelper2.mp("@Role", SqlDbType.NVarChar, 50, Role));
		}

		public static IDataReader ListByRole(string ContainerKey, string Role)
		{
			return DbHelper2.RunSpDataReader("fsc_UserRoleListByRole",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey),
				DbHelper2.mp("@Role", SqlDbType.NVarChar, 50, Role));
		}
		#endregion
	}

	public class DBForeignContainerKey
	{
		private DBForeignContainerKey()
		{
		}

		public static void Add(string ContainerKey, string ForeignContainerKey)
		{
			DbHelper2.RunSp("fsc_ForeignContainerKeyAdd",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey),
				DbHelper2.mp("@ForeignContainerKey", SqlDbType.NVarChar, 50, ForeignContainerKey));
		}

		public static void Delete(string ContainerKey)
		{
			DbHelper2.RunSp("fsc_ForeignContainerKeyDelete",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey));
		}

		public static void Delete(string ContainerKey, string ForeignContainerKey)
		{
			DbHelper2.RunSp("fsc_ForeignContainerKeyDelete",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey),
				DbHelper2.mp("@ForeignContainerKey", SqlDbType.NVarChar, 50, ForeignContainerKey));
		}

	}
}
