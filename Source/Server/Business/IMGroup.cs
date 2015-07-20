using System;
using System.Collections;
using System.Data;

using Mediachase.Ibn;
using Mediachase.IBN.Database;


namespace Mediachase.IBN.Business
{
	/// <summary>
	/// 
	/// </summary>
	public class IMGroup
	{
		#region CanCreate
		public static bool CanCreate()
		{
			return Security.IsUserInGroup(InternalSecureGroups.Administrator);
		}
		#endregion

		#region CanUpdate
		public static bool CanUpdate()
		{
			return Security.IsUserInGroup(InternalSecureGroups.Administrator);
		}
		#endregion

		#region CanDelete
		public static bool CanDelete()
		{
			return Security.IsUserInGroup(InternalSecureGroups.Administrator);
		}
		#endregion

		#region CanDelete
		public static bool CanDelete(int IMGroupId)
		{
			bool retval = Security.IsUserInGroup(InternalSecureGroups.Administrator);
			if (retval)		// Check for users
			{
				using (IDataReader reader = DBIMGroup.GetListUsers(IMGroupId))
				{
					if (reader.Read())
						retval = false;
				}
			}

			if (retval)		// Check for partner
			{
				DataTable table = DBIMGroup.GetIMGroup(IMGroupId, true);
				if (table.Rows.Count > 0)
				{
					if ((bool)table.Rows[0]["is_partner"])
						retval = false;
				}
			}
			return retval;
		}
		#endregion

		#region Create
		public static int Create(string IMGroupName, string Color, 
			byte[] IMGroupLogo, ArrayList YouCanSeeGroups, ArrayList CanSeeYouGroups)
		{
			return Create(IMGroupName, Color, false, IMGroupLogo, YouCanSeeGroups, CanSeeYouGroups);
		}

		public static int Create(string IMGroupName, string Color, bool IsPartner, byte[] IMGroupLogo, ArrayList YouCanSeeGroups, ArrayList CanSeeYouGroups)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (!CanCreate())
				throw new AccessDeniedException();

			int IMGroupId = -1;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				IMGroupId = DBIMGroup.CreateUpdate(-1, IMGroupName, Color, IsPartner);

				if (IMGroupLogo != null)
					DBIMGroup.UpdateIMGroupLogo(IMGroupId, IMGroupLogo);

				// You Can See Groups
				foreach (int GroupId in YouCanSeeGroups)
					DBIMGroup.AddDependences(IMGroupId, GroupId);

				// Can See You Groups
				foreach (int GroupId in CanSeeYouGroups)
					DBIMGroup.AddDependences(GroupId, IMGroupId);
				tran.Commit();
			}

			return IMGroupId;
		}
		#endregion

		#region Clone
		public static int Clone(int FromIMGroupId, string IMGroupName, string Color, byte[] IMGroupLogo, ArrayList YouCanSeeGroups, ArrayList CanSeeYouGroups)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (!CanCreate())
				throw new AccessDeniedException();

			int IMGroupId = -1;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				IMGroupId = DBIMGroup.CreateUpdate(-1, IMGroupName, Color, false);

				if (IMGroupLogo != null)
					DBIMGroup.UpdateIMGroupLogo(IMGroupId, IMGroupLogo);
				else
					DBIMGroup.CloneIMGroupLogo(FromIMGroupId, IMGroupId);

				// You Can See Groups
				foreach (int GroupId in YouCanSeeGroups)
					DBIMGroup.AddDependences(IMGroupId, GroupId);

				// Can See You Groups
				foreach (int GroupId in CanSeeYouGroups)
					DBIMGroup.AddDependences(GroupId, IMGroupId);
				tran.Commit();
			}

			return IMGroupId;
		}
		#endregion

		#region Update
		public static void Update(
			int imGroupId
			, string imGroupName
			, string color
			, byte[] imGroupLogo
			, ArrayList youCanSeeGroups
			, ArrayList canSeeYouGroups)
		{
			if (!CanUpdate())
				throw new AccessDeniedException();

			// YouCanSeeGroups 
			ArrayList newYouCanSeeGroups = new ArrayList(youCanSeeGroups);
			ArrayList deletedYouCanSeeGroups = new ArrayList();
			SeparateIems(newYouCanSeeGroups, deletedYouCanSeeGroups, DBIMGroup.GetListIMGroupsYouCanSee(imGroupId, false), "IMGroupId");

			// CanSeeYouGroups 
			ArrayList newCanSeeYouGroups = new ArrayList(canSeeYouGroups);
			ArrayList deletedCanSeeYouGroups = new ArrayList();
			SeparateIems(newCanSeeYouGroups, deletedCanSeeYouGroups, DBIMGroup.GetListIMGroupsCanSeeYou(imGroupId), "IMGroupId");

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBIMGroup.CreateUpdate(imGroupId, imGroupName, color, false);

				if (imGroupLogo != null)
					DBIMGroup.UpdateIMGroupLogo(imGroupId, imGroupLogo);

				// You Can See Groups
				foreach (int groupId in deletedYouCanSeeGroups)
					DBIMGroup.DeleteDependences(imGroupId, groupId);
				foreach (int groupId in newYouCanSeeGroups)
					DBIMGroup.AddDependences(imGroupId, groupId);

				// Can See You Groups
				foreach (int groupId in deletedCanSeeYouGroups)
					DBIMGroup.DeleteDependences(groupId, imGroupId);
				foreach (int groupId in newCanSeeYouGroups)
					DBIMGroup.AddDependences(groupId, imGroupId);

				// IBN
				try
				{
					IMManager.UpdateGroup(imGroupId);
					foreach (int groupId in deletedCanSeeYouGroups)
						IMManager.UpdateGroup(groupId);
					foreach (int groupId in newCanSeeYouGroups)
						IMManager.UpdateGroup(groupId);
				}
				catch (Exception)
				{
				}
				tran.Commit();
			}
		}
		#endregion

		#region Delete
		public static void Delete(int IMGroupId)
		{
			if (!CanDelete())
				throw new AccessDeniedException();
			DBIMGroup.Delete(IMGroupId);
		}
		#endregion

		#region GetGroup
		/// <summary>
		/// DataTable contains columns:
		///		IMGroupId, IMGroupName, color, logo_version, is_partner
		/// </summary>
		public static DataTable GetGroup(int imGroupId)
		{
			return DBIMGroup.GetIMGroup(imGroupId, true);
		}
		#endregion

		#region GetListIMGroup
		/// <summary>
		/// DataTable contains columns:
		///		IMGroupId, IMGroupName, color, logo_version, is_partner
		/// </summary>
		public static DataTable GetListIMGroup()
		{
			return DBIMGroup.GetIMGroup(0, false);
		}
		#endregion

		#region GetListUsers
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IsActive, IMGroupId, OriginalId
		/// </summary>
		public static IDataReader GetListUsers(int group_id)
		{
			return DBIMGroup.GetListUsers(group_id);
		}
		#endregion

		#region GetListIMGroupsYouCanSee
		/// <summary>
		/// DataTable contains columns:
		///  IMGroupId, IMGroupName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIMGroupsYouCanSee(int IMGroupId)
		{
			return DBIMGroup.GetListIMGroupsYouCanSee(IMGroupId, false);
		}
		#endregion

		#region GetListIMGroupsCanSeeYou
		/// <summary>
		/// DataTable contains columns:
		///  IMGroupId, IMGroupName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIMGroupsCanSeeYou(int imGroupId)
		{
			return DBIMGroup.GetListIMGroupsCanSeeYou(imGroupId);
		}
		#endregion

		#region GetBinaryClientLogo
		/// <summary>
		/// Reader returns fields:
		///		client_logo
		/// </summary>
		public static IDataReader GetBinaryClientLogo(int IMGroupId)
		{
			return DBIMGroup.GetBinaryClientLogo(IMGroupId);
		}
		#endregion

		#region GetListIMGroupsWithoutPartners
		/// <summary>
		/// DataTable contains columns:
		///		IMGroupId, IMGroupName, color, logo_version, is_partner
		/// </summary>
		public static DataTable GetListIMGroupsWithoutPartners()
		{
			return DBIMGroup.GetListIMGroupsWithoutPartners();
		}
		#endregion

		#region GetListIMSessionsByUserAndDate
		/// <summary>
		/// DataTable contains columns:
		///		User_Id, SessionBegin, SessionEnd
		/// </summary>
		public static DataTable GetListIMSessionsByUserAndDate(int userId, DateTime startDate, DateTime endDate)
		{
			int defaultBias = User.GetCurrentBias();
			return DBIMGroup.GetListIMSessionsByUserAndDate(userId, startDate, endDate, defaultBias);
		}
		#endregion

		#region public static string GetIMGroupName(int imGroupId, string defaultValue)
		public static string GetIMGroupName(int imGroupId, string defaultValue)
		{
			string retVal = defaultValue;

			using (DataTable table = GetGroup(imGroupId))
			{
				if (table.Rows.Count > 0)
					retVal = table.Rows[0]["IMGroupName"].ToString();
			}

			return retVal;
		}
		#endregion


		#region private

		private static void SeparateIems(ArrayList newItems, ArrayList deletedItems, DataTable table, string fieldName)
		{
			foreach (DataRow row in table.Rows)
			{
				int item = (int)row[fieldName];

				if (newItems.Contains(item))
					newItems.Remove(item);
				else
					deletedItems.Add(item);
			}
		}

		#endregion
	}
}
