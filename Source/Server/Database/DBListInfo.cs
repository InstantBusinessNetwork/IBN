using System;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database
{
	public class DBListInfo
	{
		#region CreateListAccess
		/// <summary>
		/// Creates the list access.
		/// </summary>
		/// <param name="ListId">The list id.</param>
		/// <param name="PrincipalId">The principal id.</param>
		/// <param name="AllowLevel">The allow level.</param>
		/// <returns></returns>
		public static int CreateListAccess(int ListId, int PrincipalId, byte AllowLevel)
		{
			return DbHelper2.RunSpInteger("ListInfoAccessCreate",
				DbHelper2.mp("@ListId", SqlDbType.Int, ListId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId),
				DbHelper2.mp("@AllowLevel", SqlDbType.Int, AllowLevel));
		}
		#endregion

		#region DeleteListAccess
		/// <summary>
		/// Deletes the list access by PrincipalId.
		/// </summary>
		/// <param name="ListId">The list id.</param>
		/// <param name="PrincipalId">The principal id.</param>
		public static void DeleteListAccess(int ListId, int PrincipalId)
		{
			DbHelper2.RunSp("ListInfoAccessDelete",
				DbHelper2.mp("@ListId", SqlDbType.Int, ListId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, PrincipalId));
		}
		#endregion

		#region DeleteListAccessByList
		/// <summary>
		/// Deletes the list access by list.
		/// </summary>
		/// <param name="ListId">The list id.</param>
		public static void DeleteListAccessByList(int ListId)
		{
			DbHelper2.RunSp("ListInfoAccessDeleteByList",
				DbHelper2.mp("@ListId", SqlDbType.Int, ListId));
		}
		#endregion

		#region GetListAccess
		/// <summary>
		/// Gets the list access.
		/// </summary>
		/// <param name="ListAccessId">The list access id.</param>
		/// <returns>ListId, PrincipalId, AllowLevel</returns>
		public static IDataReader GetListAccess(int ListAccessId)
		{
			return DbHelper2.RunSpDataReader("ListInfoAccessGet",
				DbHelper2.mp("@ListAccessId", SqlDbType.Int, ListAccessId));
		}
		#endregion

		#region GetListAccesses
		/// <summary>
		/// Gets the list accesses as IDataReader.
		/// </summary>
		/// <param name="ListId">The list id.</param>
		/// <returns>ListAccessId, PrincipalId, AllowLevel</returns>
		public static IDataReader GetListAccesses(int ListId)
		{
			return DbHelper2.RunSpDataReader("ListInfoAccessesGet",
				DbHelper2.mp("@ListId", SqlDbType.Int, ListId));
		}

		/// <summary>
		/// Gets the list accesses as DataTable.
		/// </summary>
		/// <param name="ListId">The list id.</param>
		/// <returns>ListAccessId, PrincipalId, AllowLevel</returns>
		public static DataTable GetListAccessesDT(int ListId)
		{
			return DbHelper2.RunSpDataTable("ListInfoAccessesGet",
				DbHelper2.mp("@ListId", SqlDbType.Int, ListId));
		}
		#endregion

		#region GetSecurityForUser
		/// <summary>
		/// Gets the security for user.
		/// </summary>
		/// <param name="ListId">The list id.</param>
		/// <param name="UserId">The user id.</param>
		/// <returns></returns>
		public static int GetSecurityForUser(int ListId, int UserId)
		{
			return DbHelper2.RunSpInteger("ListInfoGetSecurityForUser",
				DbHelper2.mp("@ListId", SqlDbType.Int, ListId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion
	}
}
