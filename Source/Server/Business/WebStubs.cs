using System;
using System.Collections;
using System.Data;

using Mediachase.Ibn;
using Mediachase.IBN.Database;


namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for WebStubs.
	/// </summary>
	public class WebStubs
	{
		#region CreateGroupStub
		public static int CreateGroupStub(
			string abbreviation
			, string toolTip
			, string url
			, bool openInBrowser
			, int width
			, int height
			, byte[] icon
			, ArrayList groups)
		{
			int stubId = -1;

			ArrayList users = new ArrayList();
			foreach (int groupId in groups)
			{
				using(IDataReader reader = SecureGroup.GetListAllUsersInGroup(groupId))
				{
					while (reader.Read())
					{
						if(reader["OriginalId"]!=DBNull.Value)
						{
							int userId = (int)reader["OriginalId"];
							if (!users.Contains(userId))
								users.Add(userId);
						}
					}
				}
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				stubId = DBWebStubs.Create(null, abbreviation, toolTip, url, openInBrowser, width, height);

				if (icon != null)
					DBWebStubs.UpdateStubIcon(stubId, icon);

				foreach (int groupId in groups)
					DBWebStubs.AddStubGroup(stubId, groupId);

				foreach (int userId in users)
					User.IncreaseStubsVersion(userId);

				try
				{
					foreach (int userId in users)
						IMManager.UpdateUserWebStub(userId);
				}
				catch (Exception)
				{
				}

				tran.Commit();
			}
			return stubId;
		}
		#endregion

		#region CreateUserStub
		public static int CreateUserStub(
			string abbreviation
			, string toolTip
			, string url
			, bool openInBrowser
			, int width
			, int height
			, byte[] icon)
		{
			int stubId = -1;
			int userId = Security.CurrentUser.UserID;
			int imUserId = DBUser.GetOriginalId(userId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				stubId = DBWebStubs.Create(userId, abbreviation, toolTip, url, openInBrowser, width, height);

				User.IncreaseStubsVersion(imUserId);

				if (icon != null)
					DBWebStubs.UpdateStubIcon(stubId, icon);

				try
				{
					IMManager.UpdateUserWebStub(imUserId);
				}
				catch (Exception)
				{
				}

				tran.Commit();
			}

			return stubId;
		}
		#endregion

		#region UpdateGroupStub
		public static void UpdateGroupStub(
			int stubId
			, string abbreviation
			, string toolTip
			, string url
			, bool openInBrowser
			, int width
			, int height
			, byte[] icon
			, ArrayList groups
			, bool deleteIcon)
		{
			// Groups
			ArrayList newGroups = new ArrayList(groups);
			ArrayList deletedGroups = new ArrayList();

			using(IDataReader reader = DBWebStubs.GetListGroupsByStub(stubId))
			{
				while(reader.Read())
				{
					int groupId = (int)reader["GroupId"];
					if (newGroups.Contains(groupId))
						newGroups.Remove(groupId);
					else
						deletedGroups.Add(groupId);
				}
			}

			// Users
			ArrayList users = new ArrayList();
			foreach (int groupId in groups)
			{
				using(IDataReader reader = SecureGroup.GetListAllUsersInGroup(groupId))
				{
					while (reader.Read())
					{
						if(reader["OriginalId"]!=DBNull.Value)
						{
							int userId = (int)reader["OriginalId"];
							if (!users.Contains(userId))
								users.Add(userId);
						}
					}
				}
			}
			foreach (int groupId in deletedGroups)
			{
				using(IDataReader reader = SecureGroup.GetListAllUsersInGroup(groupId))
				{
					while (reader.Read())
					{
						if(reader["OriginalId"]!=DBNull.Value)
						{
							int userId = (int)reader["OriginalId"];
							if (!users.Contains(userId))
								users.Add(userId);
						}
					}
				}
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBWebStubs.Update(stubId, abbreviation, toolTip, url, openInBrowser, width, height);

				if (deleteIcon)
					DBWebStubs.UpdateStubIcon(stubId, null);
				else if (icon != null)
					DBWebStubs.UpdateStubIcon(stubId, icon);

				// Remove Group
				foreach(int groupId in deletedGroups)
					DBWebStubs.DeleteStubGroup(stubId, groupId);
					
				// Add Group
				foreach(int groupId in newGroups)
					DBWebStubs.AddStubGroup(stubId, groupId);

				// Stubs Version
				foreach (int userId in users)
					User.IncreaseStubsVersion(userId);

				try
				{
					foreach (int userId in users)
						IMManager.UpdateUserWebStub(userId);
				}
				catch (Exception)
				{
				}

				tran.Commit();
			}
		}
		#endregion

		#region UpdateUserStub
		public static void UpdateUserStub(
			int stubId
			, string abbreviation
			, string toolTip
			, string url
			, bool openInBrowser
			, int width
			, int height
			, byte[] icon
			, bool deleteIcon)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBWebStubs.Update(stubId, abbreviation, toolTip, url, openInBrowser, width, height);

				int imUserId = DBUser.GetOriginalId(Security.CurrentUser.UserID);
				User.IncreaseStubsVersion(imUserId);

				if (deleteIcon)
					DBWebStubs.UpdateStubIcon(stubId, null);
				else if (icon != null)
					DBWebStubs.UpdateStubIcon(stubId, icon);

				try
				{
					IMManager.UpdateUserWebStub(imUserId);
				}
				catch (Exception)
				{
				}

				tran.Commit();
			}
		}
		#endregion

		#region Delete
		public static void Delete(int stubId)
		{
			int userId = -1;
			ArrayList users = new ArrayList();
			ArrayList groups = new ArrayList();
			using(IDataReader reader = DBWebStubs.GetStub(stubId))
			{
				if (!reader.Read())
					return;
				if (reader["UserId"] != DBNull.Value)
					userId = DBUser.GetOriginalId((int)reader["UserId"]);
			}

			if (userId == -1)
			{
				using(IDataReader reader = DBWebStubs.GetListGroupsByStub(stubId))
				{
					while (reader.Read())
						groups.Add((int)reader["GroupId"]);
				}

				foreach (int groupId in groups)
				{
					using(IDataReader reader = SecureGroup.GetListAllUsersInGroup(groupId))
					{
						while (reader.Read())
						{
							if(reader["OriginalId"]!=DBNull.Value)
							{
								int imUserId = (int)reader["OriginalId"];
								if (!users.Contains(imUserId))
									users.Add(imUserId);
							}
						}
					}
				}
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBWebStubs.Delete(stubId);

				if (userId > 0)	// UserStub
				{
					User.IncreaseStubsVersion(userId);

					try
					{
						IMManager.UpdateUserWebStub(userId);
					}
					catch (Exception)
					{
					}
				}
				else
				{
					foreach (int imUserId in users)
						User.IncreaseStubsVersion(imUserId);

					try
					{
						foreach (int imUserId in users)
							IMManager.UpdateUserWebStub(imUserId);
					}
					catch (Exception)
					{
					}
				}

				tran.Commit();
			}
		}
		#endregion

		#region GetListGroupsByStub
		/// <summary>
		/// GroupId, GroupName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListGroupsByStub(int StubId)
		{
			return DBWebStubs.GetListGroupsByStub(StubId);
		}
		#endregion

		#region GetStubIcon
		/// <summary>
		/// Reader returns fields:
		///		Icon
		/// </summary>
		public static IDataReader GetStubIcon(int StubId)
		{
			return DBWebStubs.GetStubIcon(StubId);
		}
		#endregion

		#region Hide
		public static void Hide(int stubId)
		{
			int userId = Security.CurrentUser.UserID;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBWebStubs.Hide(stubId, userId);
				int imUserId = DBUser.GetOriginalId(userId);
				User.IncreaseStubsVersion(imUserId);

				try
				{
					IMManager.UpdateUserWebStub(imUserId);
				}
				catch (IMHelperException)
				{
				}

				tran.Commit();
			}
		}
		#endregion

		#region Show
		public static void Show(int stubId)
		{
			int userId = Security.CurrentUser.UserID;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBWebStubs.Show(stubId, userId);

				int imUserId = DBUser.GetOriginalId(userId);
				User.IncreaseStubsVersion(imUserId);

				try
				{
					IMManager.UpdateUserWebStub(imUserId);
				}
				catch (IMHelperException)
				{
				}

				tran.Commit();
			}
		}
		#endregion

		#region GetListStubsForAdmin
		/// <summary>
		/// StubId, Abbreviation, ToolTip, Url, OpenInBrowser, Width, Height, IsInternal, HasIcon 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListStubsForAdmin()
		{
			return DBWebStubs.GetListStubsForAdmin();
		}
		#endregion

		#region GetListStubsForUser
		/// <summary>
		/// StubId, Abbreviation, ToolTip, Url, OpenInBrowser, Width, Height, IsInternal, HasIcon 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListStubsForUser()
		{
			return DBWebStubs.GetListStubsForUser(Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListGroupStubsForUser
		/// <summary>
		/// StubId, Abbreviation, ToolTip, Url, OpenInBrowser, Width, Height, IsInternal, IsVisible, HasIcon 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListGroupStubsForUser()
		{
			return DBWebStubs.GetListGroupStubsForUser(Security.CurrentUser.UserID);
		}
		#endregion

		#region GetStub
		/// <summary>
		/// StubId, Abbreviation, ToolTip, Url, OpenInBrowser, Width, Height, IsInternal, HasIcon 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetStub(int StubId)
		{
			return DBWebStubs.GetStub(StubId);
		}
		#endregion

		#region GetListVisibleStubsForUser
		/// <summary>
		/// StubId, Abbreviation, ToolTip, Url, OpenInBrowser, Width, Height, IsInternal, HasIcon
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListVisibleStubsForUser()
		{
			return DBWebStubs.GetListVisibleStubsForUser(Security.CurrentUser.UserID);
		}
		#endregion
	}
}
