using System;
using System.Globalization;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web;

using Mediachase.Ibn;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business
{
	public class ListInfoBus
	{
		#region LIST_TYPE
		private static ObjectTypes LIST_TYPE
		{
			get { return ObjectTypes.List; }
		}
		#endregion

		#region enum ListAccess
		public enum ListAccess
		{
			NoAccess = 0,
			Read = 1,
			Write = 2,
			Admin = 3
		}
		#endregion

		#region enum ListProjectRole
		public enum ListProjectRole
		{
			Manager = -1,
			TeamMembers = -2,
			Sponsors = -3,
			Stakeholders = -4,
			ExecutiveManager = -5
		}
		#endregion

		#region CanRead(int ListId)
		public static bool CanRead(int ListId)
		{
			bool retval = false;

			int Level = GetSecurityForUser(ListId);
			if (Level == (int)ListAccess.Admin
				|| Level == (int)ListAccess.Write
				|| Level == (int)ListAccess.Read)
				retval = true;

			return retval;
		}
		#endregion

		#region CanRead(int ListId, int UserId)
		public static bool CanRead(int ListId, int UserId)
		{
			bool retval = false;

			int Level = GetSecurityForUser(ListId, UserId);
			if (Level == (int)ListAccess.Admin
				|| Level == (int)ListAccess.Write
				|| Level == (int)ListAccess.Read)
				retval = true;

			return retval;
		}
		#endregion

		#region CanWrite
		public static bool CanWrite(int ListId)
		{
			bool retval = false;

			int Level = GetSecurityForUser(ListId);
			if (Level == (int)ListAccess.Admin
				|| Level == (int)ListAccess.Write)
				retval = true;

			return retval;
		}
		#endregion

		#region CanAdmin
		public static bool CanAdmin(int ListId)
		{
			bool retval = false;

			int Level = GetSecurityForUser(ListId);
			if (Level == (int)ListAccess.Admin)
				retval = true;

			return retval;
		}
		#endregion



		#region GetSecurityForUser
		/// <summary>
		/// Gets the security for user.
		/// </summary>
		/// <param name="ListId">The list id.</param>
		/// <returns></returns>
		public static int GetSecurityForUser(int ListId)
		{
			return GetSecurityForUser(ListId, Security.CurrentUser.UserID);
		}

		/// <summary>
		/// Gets the security for user.
		/// </summary>
		/// <param name="ListId">The list id.</param>
		/// <param name="UserId">The user id.</param>
		/// <returns></returns>
		public static int GetSecurityForUser(int listId, int userId)
		{
			int retVal = 0;
			string key = string.Format(CultureInfo.InvariantCulture, 
				"_list_{0}_usr_{1}",
				listId, userId);

			if (HttpContext.Current == null || HttpContext.Current.Items[key] == null)
			{
				retVal = DBListInfo.GetSecurityForUser(listId, userId);

				if (HttpContext.Current != null)
					HttpContext.Current.Items[key] = retVal;
			}
			else
			{
				retVal = (int)HttpContext.Current.Items[key];
			}

			return retVal;
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
			return DBListInfo.GetListAccess(ListAccessId);
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
			if (!CanAdmin(ListId))
				throw new AccessDeniedException();

			return DBListInfo.GetListAccesses(ListId);
		}

		/// <summary>
		/// Gets the list accesses as DataTable.
		/// </summary>
		/// <param name="ListId">The list id.</param>
		/// <returns>ListAccessId, PrincipalId, AllowLevel</returns>
		public static DataTable GetListAccessesDT(int ListId)
		{
			if (!CanAdmin(ListId))
				throw new AccessDeniedException();

			return DBListInfo.GetListAccessesDT(ListId);
		}
		#endregion

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
			return DBListInfo.CreateListAccess(ListId, PrincipalId, AllowLevel);
		}
		#endregion

		#region UpdateListAccess
		/// <summary>
		/// Updates the list access.
		/// </summary>
		/// <param name="ListId">The list id.</param>
		/// <param name="ListAccess">The list access.</param>
		public static void UpdateListAccess(int ListId, DataTable ListAccess)
		{
			if (!CanAdmin(ListId))
				throw new AccessDeniedException();

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBListInfo.DeleteListAccessByList(ListId);

				foreach (DataRow row in ListAccess.Rows)
				{
					int PrincipalId = (int)row["PrincipalId"];
					byte AllowLevel = (byte)row["AllowLevel"];
					if (AllowLevel < 1 || AllowLevel > 3)
						throw new ArgumentOutOfRangeException("AllowLevel", AllowLevel, "should be > 0 and < 3");
					DBListInfo.CreateListAccess(ListId, PrincipalId, AllowLevel);
				}

				tran.Commit();
			}
		}
		#endregion

		#region DeleteListAccessByList
		/// <summary>
		/// Deletes the list access by list.
		/// </summary>
		/// <param name="ListId">The list id.</param>
		public static void DeleteListAccessByList(int ListId)
		{
			DBListInfo.DeleteListAccessByList(ListId);
		}
		#endregion

		#region DeleteListAccess
		/// <summary>
		/// Deletes the list access by PrincipalId.
		/// </summary>
		/// <param name="listId">The list id.</param>
		/// <param name="principalId">The principal id.</param>
		public static void DeleteListAccess(int listId, int principalId)
		{
			DBListInfo.DeleteListAccess(listId, principalId);
		}
		#endregion


		#region AddFavorites
		/// <summary>
		/// Adds the List to the favorites.
		/// </summary>
		/// <param name="ListId">The list id.</param>
		public static void AddFavorites(int ListId)
		{
			DBCommon.AddFavorites((int)LIST_TYPE, ListId, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListFavorites
		/// <summary>
		/// Gets the list favorites as DataTable.
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListFavoritesDT()
		{
			return DBCommon.GetListFavoritesDT((int)LIST_TYPE, Security.CurrentUser.UserID);
		}
		#endregion

		#region CheckFavorites
		/// <summary>
		/// Checks the favorites.
		/// </summary>
		/// <param name="ListId">The list id.</param>
		/// <returns></returns>
		public static bool CheckFavorites(int ListId)
		{
			return DBCommon.CheckFavorites((int)LIST_TYPE, ListId, Security.CurrentUser.UserID);
		}
		#endregion

		#region DeleteFavorites
		/// <summary>
		/// Deletes the List from favorites.
		/// </summary>
		/// <param name="ListId">The list id.</param>
		public static void DeleteFavorites(int ListId)
		{
			DBCommon.DeleteFavorites((int)LIST_TYPE, ListId, Security.CurrentUser.UserID);
		}
		#endregion
	}
}
