using System;
using System.Collections;
using System.Data;

using Mediachase.Ibn;
using Mediachase.IBN.Database;
using Mediachase.Ibn.Data;
using System.Collections.Generic;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// 
	/// </summary>
	public class SecureGroup
	{
		public enum SecureGroups
		{

		}

		public SecureGroup()
		{
		
		}

		#region CanCreate
		public static bool CanCreate(int ParentGroupId)
		{
			return Security.IsUserInGroup(InternalSecureGroups.Administrator)
				&& (ParentGroupId != (int)InternalSecureGroups.Administrator)
				&& (ParentGroupId != (int)InternalSecureGroups.ProjectManager)
				&& (ParentGroupId != (int)InternalSecureGroups.PowerProjectManager)
				&& (ParentGroupId != (int)InternalSecureGroups.HelpDeskManager)
				&& (ParentGroupId != (int)InternalSecureGroups.ExecutiveManager)
				&& (ParentGroupId != (int)InternalSecureGroups.TimeManager)
				&& (ParentGroupId != (int)InternalSecureGroups.Roles)
				&& (ParentGroupId != (int)InternalSecureGroups.Everyone)
				&& (GetParentGroup(ParentGroupId) != (int)InternalSecureGroups.Partner);
		}
		#endregion

		#region CanMove
		public static bool CanMove(int GroupId)
		{
			return Security.IsUserInGroup(InternalSecureGroups.Administrator)
				&& (GroupId != (int)InternalSecureGroups.Administrator)
				&& (GroupId != (int)InternalSecureGroups.ProjectManager)
				&& (GroupId != (int)InternalSecureGroups.PowerProjectManager)
				&& (GroupId != (int)InternalSecureGroups.HelpDeskManager)
				&& (GroupId != (int)InternalSecureGroups.ExecutiveManager)
				&& (GroupId != (int)InternalSecureGroups.TimeManager)
				&& (GroupId != (int)InternalSecureGroups.Roles)
				&& (GroupId != (int)InternalSecureGroups.Everyone)
				&& (GroupId != (int)InternalSecureGroups.Partner)
				&& (GroupId != (int)InternalSecureGroups.Intranet)
				&& (GetParentGroup(GroupId) != (int)InternalSecureGroups.Partner);
		}
		#endregion

		#region CanRead
		public static bool CanRead(int GroupId)
		{
			bool RetVal = false;
			if (Security.IsUserInGroup(InternalSecureGroups.Partner))
			{
				int PartnerGroupId = DBGroup.GetGroupForPartnerUser(Security.CurrentUser.UserID);

				if (PartnerGroupId > 0)
				{
					do
					{
						RetVal = DBGroup.CheckGroupVisibilityForPartnerGroup(PartnerGroupId, GroupId);
						GroupId = GetParentGroup(GroupId);
					}
					while (!(GroupId <= 0 || RetVal));
				}
			}
			else
			{
				RetVal = true;
			}

			return RetVal;
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

		public static bool CanDelete(int group_id)
		{
			return Security.IsUserInGroup(InternalSecureGroups.Administrator)
				&& !CheckChildren(group_id)
				&& (group_id != (int)InternalSecureGroups.Administrator)
				&& (group_id != (int)InternalSecureGroups.ProjectManager)
				&& (group_id != (int)InternalSecureGroups.PowerProjectManager)
				&& (group_id != (int)InternalSecureGroups.HelpDeskManager)
				&& (group_id != (int)InternalSecureGroups.ExecutiveManager)
				&& (group_id != (int)InternalSecureGroups.TimeManager)
				&& (group_id != (int)InternalSecureGroups.Roles)
				&& (group_id != (int)InternalSecureGroups.Partner)
				&& (group_id != (int)InternalSecureGroups.Intranet)
				&& (group_id != (int)InternalSecureGroups.Everyone);
		}
		#endregion

		#region Create
		public static int Create(int parent_group_id, string name)
		{
			if(!CanCreate(parent_group_id))
				throw new AccessDeniedException();

			return DBGroup.Create(parent_group_id,name);
		}
		#endregion
		
		#region Delete
		public static void Delete(int group_id)
		{
			if(!CanDelete(group_id))
				throw new AccessDeniedException();

			int IMGroupId = -1;
			using(IDataReader reader = DBGroup.GetGroup(group_id))
			{
				if (reader.Read())
				{
					if (reader["IMGroupId"] != DBNull.Value)
						IMGroupId = (int)reader["IMGroupId"];
				}
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBGroup.Delete(group_id);

				if(IMGroupId > 0)
					IMGroup.Delete(IMGroupId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateGroup
		public static void UpdateGroup(int group_id,string name)
		{
			if(!CanUpdate() || group_id <= 9)
				throw new AccessDeniedException();

			DBGroup.Update(group_id,name);
		}

		public static void UpdateGroup(int group_id, int parent_group_id)
		{
			if(!CanMove(group_id))
				throw new AccessDeniedException();

			DBGroup.ChangeParent(group_id,parent_group_id);
		}
		#endregion

		#region GetGroup
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, IMGroupId, ContactUid, OrgUid
		/// </summary>
		public static IDataReader GetGroup(int group_id)
		{
			return DBGroup.GetGroup(group_id);
		}
		#endregion
		
		#region GetParentGroup
		public static int GetParentGroup(int group_id)
		{
			return DBGroup.GetParentGroup(group_id);
		}
		#endregion

		#region GetListUsersInGroup
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, Activity, IMGroupId, CreatedBy, IsExternal
		/// </summary>
		public static IDataReader GetListUsersInGroup(int group_id)
		{
			if (!CanRead(group_id))
				throw new AccessDeniedException();

			return DBGroup.GetListUsersInGroup(group_id);
		}
		#endregion

		#region GetListActiveUsersInGroup
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IMGroupId, CreatedBy
		/// </summary>
		public static IDataReader GetListActiveUsersInGroup(int group_id)
		{
			return GetListActiveUsersInGroup(group_id, true);
		}

		public static IDataReader GetListActiveUsersInGroup(int group_id, bool check_security)
		{
			if (check_security)
				if (!CanRead(group_id))
					throw new AccessDeniedException();

			return DBGroup.GetListActiveUsersInGroup(group_id);
		}
		#endregion

		#region GetListActiveUsersInGroupDataTable
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IMGroupId, CreatedBy
		/// </summary>
		public static DataTable GetListActiveUsersInGroupDataTable(int group_id)
		{
			return GetListActiveUsersInGroupDataTable(group_id, true);
		}

		public static DataTable GetListActiveUsersInGroupDataTable(int group_id, bool check_security)
		{
			if (check_security)
				if (!CanRead(group_id))
					throw new AccessDeniedException();

			return DBGroup.GetListActiveUsersInGroupDataTable(group_id);
		}
		#endregion

		#region GetListAllUsersInGroup
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IMGroupId, CreatedBy, Activity, IsExternal
		/// </summary>
		public static IDataReader GetListAllUsersInGroup(int group_id)
		{
			return GetListAllUsersInGroup(group_id, true);
		}

		public static IDataReader GetListAllUsersInGroup(int group_id, bool check_security)
		{
			if (check_security)
				if (!CanRead(group_id))
					throw new AccessDeniedException();

			return DBGroup.GetListAllUsersInGroup(group_id);
		}
		#endregion

		#region GetListAllUsersIn2Group
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IMGroupId, CreatedBy, Activity, IsExternal
		/// </summary>
		public static IDataReader GetListAllUsersIn2Group(int Group1Id, int Group2Id)
		{
			return DBGroup.GetListAllUsersIn2Group(Group1Id, Group2Id);
		}
		#endregion

		#region GetListAllActiveUsersInGroup
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IMGroupId, CreatedBy, Activity, IsExternal
		/// </summary>
		public static IDataReader GetListAllActiveUsersInGroup(int group_id)
		{
			return GetListAllUsersInGroup(group_id, true);
		}

		public static IDataReader GetListAllActiveUsersInGroup(int group_id, bool check_security)
		{
			if (check_security)
				if (!CanRead(group_id))
					throw new AccessDeniedException();

			return DBGroup.GetListAllActiveUsersInGroup(group_id);
		}
		#endregion

		#region GetListChildGroups
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, HasChildren
		/// </summary>
		public static IDataReader GetListChildGroups(int group_id)
		{ 
			if (!CanRead(group_id))
				throw new AccessDeniedException();

			return DBGroup.GetListChildGroups(group_id);
		}
		#endregion

		#region GetListGroups
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, IMGroupId, ContactUid, OrgUid
		/// </summary>
		public static IDataReader GetListGroups()
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
			{
				return DBGroup.GetListGroups();
			}
			else
			{
				int PartnerGroupId = DBGroup.GetGroupForPartnerUser(Security.CurrentUser.UserID);

				return DBGroup.GetListGroupsByPartner(PartnerGroupId, true, true);
			}
		}
		#endregion

		#region GetListAvailableGroups
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName
		/// </summary>
		public static IDataReader GetListAvailableGroups()
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
				return DBGroup.GetListAvailableGroups();
			else
				return DBGroup.GetListAvailableGroupsForPartner(Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListGroupsAsTreeDataTable
		/// <summary>
		///		GroupId, GroupName
		/// </summary>
		public static DataTable GetListGroupsAsTreeDataTable()
		{
			DataTable dt;

			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
			{
				dt = new DataTable();
				dt.Columns.Add("GroupId", typeof(int));
				dt.Columns.Add("GroupName", typeof(string));
				GenerateTree(1, 0, ref dt);
			}
			else
			{
				int PartnerGroupId = DBGroup.GetGroupForPartnerUser(Security.CurrentUser.UserID);
				dt = DBGroup.GetListGroupsByPartnerDataTable(PartnerGroupId, true, false);
			}
			
			return dt;
		}

		private static void GenerateTree(int ParentGroupId, int CurrentLevel, ref DataTable dt)
		{
			List<GroupInfo> groups = new List<GroupInfo>();

			using (IDataReader reader = DBGroup.GetListChildGroups(ParentGroupId))
			{
				while (reader.Read())
				{
					GroupInfo gi = new GroupInfo((int)reader["GroupId"], Common.GetWebResourceString(reader["GroupName"].ToString()), (bool)reader["HasChildren"]);
					groups.Add(gi);
				}
			}

			// sorting
			groups.Sort(delegate(GroupInfo x, GroupInfo y) { return x.GroupName.CompareTo(y.GroupName); });

			string Prefix = "";
			for (int i=0; i < CurrentLevel; i++)
				Prefix += "  ";

			CurrentLevel++;

			foreach (GroupInfo group in groups)
			{
				DataRow dr = dt.NewRow();
				dr["GroupId"] = group.GroupId;
				dr["GroupName"] = Prefix + group.GroupName;
				dt.Rows.Add(dr);

				if (group.HasChildren)
					GenerateTree(group.GroupId, CurrentLevel, ref dt);
			}
		}

		private class GroupInfo
		{
			public int GroupId;
			public string GroupName;
			public bool HasChildren;
			public GroupInfo(int groupId, string groupName, bool hasChildren)
			{
				GroupId = groupId;
				GroupName = groupName;
				HasChildren = hasChildren;
			}
		}
		#endregion

		#region GetListGroupsAsTree
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, Level
		/// </summary>
		public static IDataReader GetListGroupsAsTree()
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
				return DBGroup.GetListGroupsAsTree();
			else
			{
				int PartnerGroupId = DBGroup.GetGroupForPartnerUser(Security.CurrentUser.UserID);
				return DBGroup.GetListGroupsByPartner(PartnerGroupId, true, false);
			}
		}
		#endregion

		#region GetListGroupsForMove
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName
		/// </summary>
		public static IDataReader GetListGroupsForMove(int group_id)
		{
			return DBGroup.GetListGroupsForMove(group_id);
		}
		#endregion

		#region GetListGroupsBySubstring
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, HasChildren
		/// </summary>
		public static IDataReader GetListGroupsBySubstring(string SubString)
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
				return DBGroup.GetListGroupsBySubstring(SubString);
			else
				return DBGroup.GetListGroupsBySubstringForPartner(SubString, Security.CurrentUser.UserID);
		}
		#endregion

		#region CheckChildren
		public static bool CheckChildren(int group_id)
		{
			return (DBGroup.CheckChildren(group_id) == 1)?true:false;
		}
		#endregion

		#region GetListGroupsAsTreeForIBN
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, Level
		/// </summary>
		public static IDataReader GetListGroupsAsTreeForIBN()
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
				return DBGroup.GetListGroupsAsTreeForIBN();
			else
			{
				int PartnerGroupId = DBGroup.GetGroupForPartnerUser(Security.CurrentUser.UserID);
				return DBGroup.GetListGroupsByPartner(PartnerGroupId, true, false);
			}
		}
		#endregion

		#region GetListGroupsWithParameters
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName
		/// </summary>
		public static IDataReader GetListGroupsWithParameters(bool IncludeEveryone,
			bool IncludeAdmin, bool IncludePM, bool IncludePPM, bool IncludeHDM,
			bool IncludePartner, bool IncludeSubPartners, bool IncludeExecutive, 
			bool IncludeRoles, bool IncludeIntranet, bool IncludeTM)
		{
			return DBGroup.GetListGroupsWithParameters(IncludeEveryone, IncludeAdmin, IncludePM, IncludePPM, IncludeHDM, IncludePartner, IncludeSubPartners, IncludeExecutive, IncludeRoles, IncludeIntranet, IncludeTM);
		}
		#endregion

		#region IsPartner
		public static bool IsPartner(int GroupId)
		{
			return (GetParentGroup(GroupId) == (int)InternalSecureGroups.Partner);
		}
		#endregion

		#region CreatePartner
		public static int CreatePartner(string name, PrimaryKeyId contactUid, PrimaryKeyId orgUid, ArrayList VisibleGroups, byte[] IMGroupLogo)
		{
			if(!Security.IsUserInGroup(InternalSecureGroups.Administrator))
				throw new AccessDeniedException();

			int PartnerId = -1;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				PartnerId = DBGroup.Create((int)InternalSecureGroups.Partner, name);
				foreach (int GroupId in VisibleGroups)
					DBGroup.AddPartnerGroup(PartnerId, GroupId);

				int IMGroupId = IMGroup.Create(name, "2B6087", true, IMGroupLogo, new ArrayList(), new ArrayList());

				DBGroup.UpdateIMGroupId(PartnerId, IMGroupId);
				DBGroup.UpdateClient(PartnerId, contactUid, orgUid);

				tran.Commit();
			}
			
			return PartnerId;
		}
		#endregion

		#region UpdatePartner
		public static void UpdatePartner(int PartnerId, PrimaryKeyId contactUid, PrimaryKeyId orgUid, string Name, ArrayList VisibleGroups)
		{
			if(!Security.IsUserInGroup(InternalSecureGroups.Administrator))
				throw new AccessDeniedException();

			// VisibleGroups
			ArrayList NewGroups = new ArrayList(VisibleGroups);
			ArrayList DeletedGroups = new ArrayList();
			using(IDataReader reader = DBGroup.GetListGroupsByPartner(PartnerId, false, true))
			{
				while(reader.Read())
				{
					int GroupId = (int)reader["GroupId"];
					if (NewGroups.Contains(GroupId))
						NewGroups.Remove(GroupId);
					else
						DeletedGroups.Add(GroupId);
				}
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBGroup.Update(PartnerId, Name);

				// Remove Groups
				foreach(int GroupId in DeletedGroups)
					DBGroup.DeletePartnerGroup(PartnerId, GroupId);
					
				// Add Groups
				foreach(int GroupId in NewGroups)
					DBGroup.AddPartnerGroup(PartnerId, GroupId);

				DBGroup.UpdateClient(PartnerId, contactUid, orgUid);

				tran.Commit();
			}
		}
		#endregion

		#region GetListGroupsByPartner
		/// <summary>
		/// Reader returns fields:
		///		PartnerId, GroupId, GroupName
		/// </summary>
		public static IDataReader GetListGroupsByPartner(int PartnerId)
		{
			return DBGroup.GetListGroupsByPartner(PartnerId, false, true);
		}
		#endregion

		#region GetGroupName
		public static string GetGroupName(int group_id)
		{
			string GroupName = "";
			using (IDataReader reader = DBGroup.GetGroup(group_id))
			{
				if (reader.Read())
					GroupName = reader["GroupName"].ToString();
			}

			return GroupName;
		}
		#endregion
	}
}
