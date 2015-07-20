using System;
using System.Collections;
using System.Data;

using Mediachase.IBN.Database.ControlSystem;


namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Summary description for UserRole.
	/// </summary>
	public enum UserRoleTypeEnum
	{
		Multivalue,
		SingleValue
	}

	public class UserRole
	{
		private UserRole()
		{
		}

		#region -- Add --
		public static int Add(string role, int principalId)
		{
			return Add(role, principalId, UserRoleTypeEnum.Multivalue);
		}

		public static int Add(string role, int principalId, UserRoleTypeEnum type)
		{
			if (type == UserRoleTypeEnum.SingleValue)
			{
				DBUserRole.DeleteByRole(role);
			}
			return DBUserRole.Add(principalId, role);
		}

		public static int Add(string containerKey, string role, int principalId)
		{
			return Add(containerKey, role, principalId, UserRoleTypeEnum.Multivalue);
		}

		public static int Add(string containerKey, string role, int principalId, UserRoleTypeEnum type)
		{
			if (type == UserRoleTypeEnum.SingleValue)
			{
				DBUserRole.DeleteByRole(containerKey, role);
			}
			return DBUserRole.Add(principalId, containerKey, role);
		}
		#endregion

		#region -- Delete --
		public static void DeleteAll(string containerKey)
		{
			DBUserRole.DeleteByContainerKey(containerKey);
		}

		public static void Delete(string containerKey, string role)
		{
			DBUserRole.DeleteByRole(containerKey, role);
		}

		public static void Delete(string role)
		{
			DBUserRole.DeleteByRole(role);
		}

		public static void Delete(string role, int principalId)
		{
			DBUserRole.DeleteByUser(principalId, role);
		}

		public static void Delete(string containerKey, string role, int principalId)
		{
			DBUserRole.DeleteByUser(principalId, containerKey, role);
		}
		#endregion

		#region -- List --
		public static int[] List(string role)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DBUserRole.ListByRole(role))
			{
				while (reader.Read())
				{
					list.Add(reader["PrincipalId"]);
				}
			}
			return (int[])list.ToArray(typeof(int));
		}

		public static int[] List(string containerKey, string role)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DBUserRole.ListByRole(containerKey, role))
			{
				while (reader.Read())
				{
					list.Add(reader["PrincipalId"]);
				}
			}
			return (int[])list.ToArray(typeof(int));
		}
		#endregion
	}

	public class ForeignContainerKey
	{
		private ForeignContainerKey()
		{
		}

		public static void Add(string containerKey, string foreignContainerKey)
		{
			DBForeignContainerKey.Add(containerKey, foreignContainerKey);
		}

		public static void Delete(string containerKey)
		{
			DBForeignContainerKey.Delete(containerKey);
		}

		public static void Delete(string containerKey, string foreignContainerKey)
		{
			DBForeignContainerKey.Delete(containerKey, foreignContainerKey);
		}

	}
}
