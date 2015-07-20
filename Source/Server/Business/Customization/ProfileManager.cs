using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.XmlTools;

namespace Mediachase.Ibn.Business.Customization
{
	public sealed class ProfileManager
	{
		private ProfileManager()
		{
		}

		#region CheckPersonalization
		/// <summary>
		/// Checks whether the current user to personalize workspace
		/// </summary>
		/// <returns></returns>
		public static bool CheckPersonalization()
		{
			int profileId = GetProfileIdByUser();

			CustomizationProfileEntity profile = (CustomizationProfileEntity)BusinessManager.Load(CustomizationProfileEntity.ClassName, PrimaryKeyId.Parse(profileId.ToString()));
			return profile.WorkspacePersonalization;
		} 
		#endregion

		#region GetProfileIdByUser
		/// <summary>
		/// Gets the profile id by user.
		/// </summary>
		/// <returns></returns>
		public static int GetProfileIdByUser()
		{
			return GetProfileIdByUser(Security.CurrentUser.UserID);
		}

		/// <summary>
		/// Gets the profile id by user.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public static int GetProfileIdByUser(int userId)
		{
			int profileId = -1;
			EntityObject[] profileUserList = BusinessManager.List(CustomizationProfileUserEntity.ClassName,
				new FilterElement[] { FilterElement.EqualElement(CustomizationProfileUserEntity.FieldPrincipalId, userId) });

			if (profileUserList.Length > 0)
				profileId = (int)((CustomizationProfileUserEntity)profileUserList[0]).ProfileId;

			return profileId;
		}
		#endregion

		#region ClearCacheForProfileUsers
		internal static void ClearCacheForProfileUsers(int profileId)
		{
			// [2009-03-11] O.R.: check default profile
			if (profileId > 0)
			{
				FilterElement[] filters = new FilterElement[] { FilterElement.EqualElement("ProfileId", profileId) };
				EntityObject[] entityList = BusinessManager.List(CustomizationProfileUserEntity.ClassName, filters);

				foreach (CustomizationProfileUserEntity entity in entityList)
				{
					DataCache.RemoveByUser(entity.PrincipalId.ToString());
				}
			}
			else
			{
				// 1. Get list all active users 
				List<int> activeUsers = new List<int>();
				using (IDataReader reader = Mediachase.IBN.Business.User.GetListActive())
				{
					while (reader.Read())
						activeUsers.Add((int)reader["PrincipalId"]);
				}

				// 2. Exclude users with profile
				EntityObject[] entityList = BusinessManager.List(CustomizationProfileUserEntity.ClassName, (new FilterElementCollection()).ToArray());
				foreach (CustomizationProfileUserEntity entity in entityList)
				{
					activeUsers.Remove((int)entity.PrincipalId);
				}

				// 3. Clear cache for users without profile (with default profile)
				foreach (int userId in activeUsers)
					DataCache.RemoveByUser(userId.ToString());
			}
		}
		#endregion
	}
}
