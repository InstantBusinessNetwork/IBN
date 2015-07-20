using System;
using System.Data;
using System.Data.SqlClient;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBGroup.
	/// </summary>
	public class DBGroup
	{

		#region Create
		public static int Create(
			int			ParentGroupId,
			string	GroupName)
		{
			return DbHelper2.RunSpInteger("GroupCreate",
				DbHelper2.mp("@ParentPrincipalId",	SqlDbType.Int,					ParentGroupId	),
				DbHelper2.mp("@GroupName",					SqlDbType.NVarChar, 50,	GroupName			));
		}
		#endregion

		#region Delete
		public static void Delete(int GroupId)
		{
			DbHelper2.RunSp("GroupDelete",
				DbHelper2.mp("@GroupId",	SqlDbType.Int, GroupId));
		}
		#endregion

		#region Update
		public static void Update(
			int			GroupId,
			string	GroupName)
		{
			DbHelper2.RunSp("GroupUpdate",
				DbHelper2.mp("@PrincipalId",		SqlDbType.Int,			GroupId),
				DbHelper2.mp("@GroupName",	SqlDbType.NVarChar, 50,	GroupName));
		}
		#endregion

		#region ChangeParent
		public static void ChangeParent(
			int	GroupId,
			int	ParentGroupId)
		{
			DbHelper2.RunSp("GroupChangeParent",
				DbHelper2.mp("@PrincipalId",				SqlDbType.Int, GroupId),
				DbHelper2.mp("@ParentPrincipalId",	SqlDbType.Int, ParentGroupId));
		}
		#endregion

		#region GetGroup
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, IMGroupId, ContactUid, OrgUid
		/// </summary>
		public static IDataReader GetGroup(int GroupId)
		{
			return DbHelper2.RunSpDataReader("GroupGet", 
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetParentGroup
		public static int GetParentGroup(int GroupId)
		{
			return DbHelper2.RunSpInteger("GroupGetParent",
				DbHelper2.mp("@PrincipalId",	SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetListUsersInGroup
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, Activity, IMGroupId, CreatedBy, IsExternal
		/// </summary>
		public static IDataReader GetListUsersInGroup(int GroupId)
		{
			return DbHelper2.RunSpDataReader("UsersGetByGroup", 
				DbHelper2.mp("@GroupId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetListActiveUsersInGroup
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IMGroupId, CreatedBy
		/// </summary>
		public static IDataReader GetListActiveUsersInGroup(int GroupId)
		{
			return DbHelper2.RunSpDataReader("UsersGetActiveByGroup", 
				DbHelper2.mp("@GroupId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetListActiveUsersInGroupDataTable
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IMGroupId, CreatedBy
		/// </summary>
		public static DataTable GetListActiveUsersInGroupDataTable(int GroupId)
		{
			return DbHelper2.RunSpDataTable("UsersGetActiveByGroup",
				DbHelper2.mp("@GroupId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetListAllUsersInGroup
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IMGroupId, CreatedBy, Activity, IsExternal
		/// </summary>
		public static IDataReader GetListAllUsersInGroup(int GroupId)
		{
			return DbHelper2.RunSpDataReader("UsersGetByGroupAll", 
				DbHelper2.mp("@GroupId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetListAllUsersIn2Group
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IMGroupId, CreatedBy, Activity, IsExternal
		/// </summary>
		public static IDataReader GetListAllUsersIn2Group(int Group1Id, int Group2Id)
		{
			return DbHelper2.RunSpDataReader("UsersGetBy2GroupAll",
				DbHelper2.mp("@Group1Id", SqlDbType.Int, Group1Id),
				DbHelper2.mp("@Group2Id", SqlDbType.Int, Group2Id));
		}
		#endregion

		#region GetListAllActiveUsersInGroup
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IMGroupId, CreatedBy, Activity, IsExternal
		/// </summary>
		public static IDataReader GetListAllActiveUsersInGroup(int GroupId)
		{
			return DbHelper2.RunSpDataReader("UsersGetActiveByGroupAll", 
				DbHelper2.mp("@GroupId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetListChildGroups
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, HasChildren
		/// </summary>
		public static IDataReader GetListChildGroups(int GroupId)
		{
			return DbHelper2.RunSpDataReader("GroupsGetByParent", 
				DbHelper2.mp("@GroupId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetListGroups
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, IMGroupId, ContactUid, OrgUid
		/// </summary>
		public static IDataReader GetListGroups()
		{
			return DbHelper2.RunSpDataReader("GroupGet", 
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, 0));
		}
		#endregion

		#region GetListAvailableGroups
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName
		/// </summary>
		public static IDataReader GetListAvailableGroups()
		{
			return DbHelper2.RunSpDataReader("GroupsGetAvailable");
		}
		#endregion

		#region GetListGroupsAsTree
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, Level
		/// </summary>
		public static IDataReader GetListGroupsAsTree()
		{
			return DbHelper2.RunSpDataReader("GroupsGetAsTree");
		}
		#endregion

		#region GetListGroupsForMove
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName
		/// </summary>
		public static IDataReader GetListGroupsForMove(int GroupId)
		{
			return DbHelper2.RunSpDataReader("GroupsGetForMove", 
				DbHelper2.mp("@GroupId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetListGroupsBySubstring
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, HasChildren
		/// </summary>
		public static IDataReader GetListGroupsBySubstring(string SubString)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			SubString = DBCommon.ReplaceSqlWildcard(SubString);
			//

			return DbHelper2.RunSpDataReader("GroupsSearch", 
				DbHelper2.mp("@SubString", SqlDbType.NVarChar, 50, @SubString));
		}
		#endregion

		#region CheckChildren
		public static int CheckChildren(int GroupId)
		{
			return DbHelper2.RunSpInteger("GroupCheckChildren",
				DbHelper2.mp("@GroupId",	SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetListGroupsAsTreeForIBN
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, Level
		/// </summary>
		public static IDataReader GetListGroupsAsTreeForIBN()
		{
			return DbHelper2.RunSpDataReader("GroupsGetAsTree2");
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
			return DbHelper2.RunSpDataReader("GroupsGet", 
				DbHelper2.mp("@IncludeEveryone", SqlDbType.Bit, IncludeEveryone),
				DbHelper2.mp("@IncludeAdmin", SqlDbType.Bit, IncludeAdmin),
				DbHelper2.mp("@IncludePM", SqlDbType.Bit, IncludePM),
				DbHelper2.mp("@IncludePPM", SqlDbType.Bit, IncludePPM),
				DbHelper2.mp("@IncludeHDM", SqlDbType.Bit, IncludeHDM),
				DbHelper2.mp("@IncludePartner", SqlDbType.Bit, IncludePartner),
				DbHelper2.mp("@IncludeSubPartners", SqlDbType.Bit, IncludeSubPartners),
				DbHelper2.mp("@IncludeExecutive", SqlDbType.Bit, IncludeExecutive),
				DbHelper2.mp("@IncludeRoles", SqlDbType.Bit, IncludeRoles),
				DbHelper2.mp("@IncludeIntranet", SqlDbType.Bit, IncludeIntranet),
				DbHelper2.mp("@IncludeTM", SqlDbType.Bit, IncludeTM));
		}
		#endregion


		#region AddPartnerGroup
		public static void AddPartnerGroup(int PartnerId, int GroupId)
		{
			DbHelper2.RunSp("PartnerGroupAdd",
				DbHelper2.mp("@PartnerId",	SqlDbType.Int, PartnerId),
				DbHelper2.mp("@GroupId",	SqlDbType.Int, GroupId));
		}
		#endregion

		#region DeletePartnerGroup
		public static void DeletePartnerGroup(int PartnerId, int GroupId)
		{
			DbHelper2.RunSp("PartnerGroupDelete",
				DbHelper2.mp("@PartnerId",	SqlDbType.Int, PartnerId),
				DbHelper2.mp("@GroupId",	SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetListGroupsByPartner
		/// <summary>
		/// Reader returns fields:
		///		PartnerId, GroupId, GroupName, Level (=1)
		/// </summary>
		public static IDataReader GetListGroupsByPartner(int PartnerId, bool IncludeCurrent, bool IncludeEveryone)
		{
			return DbHelper2.RunSpDataReader("PartnerGroupGetListByPartner", 
				DbHelper2.mp("@PartnerId", SqlDbType.Int, PartnerId),
				DbHelper2.mp("@IncludeCurrent", SqlDbType.Bit, IncludeCurrent),
				DbHelper2.mp("@IncludeEveryone", SqlDbType.Bit, IncludeEveryone));
		}
		#endregion

		#region GetListGroupsByPartnerDataTable
		/// <summary>
		/// Reader returns fields:
		///		PartnerId, GroupId, GroupName, Level (=1)
		/// </summary>
		public static DataTable GetListGroupsByPartnerDataTable(int PartnerId, bool IncludeCurrent, bool IncludeEveryone)
		{
			return DbHelper2.RunSpDataTable("PartnerGroupGetListByPartner", 
				DbHelper2.mp("@PartnerId", SqlDbType.Int, PartnerId),
				DbHelper2.mp("@IncludeCurrent", SqlDbType.Bit, IncludeCurrent),
				DbHelper2.mp("@IncludeEveryone", SqlDbType.Bit, IncludeEveryone));
		}
		#endregion

		#region UpdateIMGroupId
		public static void UpdateIMGroupId(int GroupId, int IMGroupId)
		{
			DbHelper2.RunSp("GroupUpdateIMGroupId",
				DbHelper2.mp("@PrincipalId",	SqlDbType.Int, GroupId),
				DbHelper2.mp("@IMGroupId",	SqlDbType.Int, IMGroupId));
		}
		#endregion

		#region CheckUserVisibilityForPartnerGroup
		public static bool CheckUserVisibilityForPartnerGroup(int PartnerId, int UserId)
		{
			if (DbHelper2.RunSpInteger("PartnerGroupCheckUserVisibility",
				DbHelper2.mp("@PartnerId",	SqlDbType.Int, PartnerId),
				DbHelper2.mp("@UserId",	SqlDbType.Int, UserId)) == 1)
				return true;
			else
				return false;
		}
		#endregion

		#region CheckGroupVisibilityForPartnerGroup
		public static bool CheckGroupVisibilityForPartnerGroup(int PartnerId, int GroupId)
		{
			if (DbHelper2.RunSpInteger("PartnerGroupCheckGroupVisibility",
				DbHelper2.mp("@PartnerId",	SqlDbType.Int, PartnerId),
				DbHelper2.mp("@GroupId",	SqlDbType.Int, GroupId)) == 1)
				return true;
			else
				return false;
		}
		#endregion

		#region GetGroupForPartnerUser
		public static int GetGroupForPartnerUser(int UserId)
		{
			return DbHelper2.RunSpInteger("GroupGetForPartnerUser", 
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListAvailableGroupsForPartner
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName
		/// </summary>
		public static IDataReader GetListAvailableGroupsForPartner(int UserId)
		{
			return DbHelper2.RunSpDataReader("GroupsGetAvailableForPartner",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListGroupsBySubstringForPartner
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, HasChildren
		/// </summary>
		public static IDataReader GetListGroupsBySubstringForPartner(string SubString, int UserId)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			SubString = DBCommon.ReplaceSqlWildcard(SubString);
			//

			return DbHelper2.RunSpDataReader("GroupsSearchForPartner", 
				DbHelper2.mp("@SubString", SqlDbType.NVarChar, 50, @SubString),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, UserId));
		}
		#endregion

		#region UpdateClient
		public static void UpdateClient(int GroupId, PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			DbHelper2.RunSp("GroupUpdateClient",
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, GroupId),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion
	}
}
