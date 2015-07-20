using System;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database
{

	public class DBProjectGroup
	{
		#region Create
		public static int Create(
			string Title, string Description, 
			int CreatorId,DateTime CreationDate)
		{
			return DbHelper2.RunSpInteger("ProjectGroupCreate",
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 100, Title),
				DbHelper2.mp("@Description", SqlDbType.NText, Description),
				DbHelper2.mp("@CreationDate", SqlDbType.DateTime, CreationDate),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId));
		}
		#endregion
		
		#region Update
			/*
			ProjectGroupUpdate 
			@ProjectGroupId int,
			@Title nvarchar(100),
			@Description ntext
			*/

		
		public static void Update(
			int GroupId,
			string Title, string Description)
		{
				DbHelper2.RunSp("ProjectGroupUpdate",
				DbHelper2.mp("@ProjectGroupId", SqlDbType.Int, GroupId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 100, Title),
				DbHelper2.mp("@Description", SqlDbType.NText, Description));
		}
		
		#endregion

		#region Delete
		public static void Delete(int ProjectGroupId)
		{
			DbHelper2.RunSp("ProjectGroupDelete", 
				DbHelper2.mp("@ProjectGroupId", SqlDbType.Int, ProjectGroupId));
		}
		#endregion
		
		#region GetProjectGroup
		/*
			ProjectGroupsGet 
			[ProjectGroupId], [Title], [Description], [CreationDate], [CreatorId]
		*/
		public static IDataReader GetProjectGroup(int ProjectGroupId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate"},
				"ProjectGroupGet",
				DbHelper2.mp("@ProjectGroupId", SqlDbType.Int, ProjectGroupId));
		}
		
		
		#endregion

		#region GetProjectGroups
		/*
			ProjectGroupsGet 
			[ProjectGroupId], [Title], [Description], [CreationDate], [CreatorId]
		*/
		public static IDataReader GetProjectGroups(int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate"},
				"ProjectGroupsGet");
		}
		#endregion

		#region GetProjectGroupsDT
		/*
			ProjectGroupsGet 
			[ProjectGroupId], [Title], [Description], [CreationDate], [CreatorId]
		*/
		public static DataTable GetProjectGroupsDT(int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate"},
				"ProjectGroupsGet");
		}
		#endregion

		#region ProjectGroupsGetByProject
		//A.[ProjectGroupId], B.[Title] 
		public static IDataReader ProjectGroupsGetByProject(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("ProjectGroupsGetByProject",
				   DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}

		#endregion	 

		#region AssignProjectToProjectGroup
		public static void AssignProjectGroup(int ProjectId, int ProjectGroupId)
		{
			DbHelper2.RunSp("ProjectGroupAssign", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@ProjectGroupId", SqlDbType.Int, ProjectGroupId));
		}
		#endregion

		#region RemoveProjectFromProjectGroup
		public static void RemoveProjectFromProjectGroup(int ProjectId, int ProjectGroupId)
		{
			DbHelper2.RunSp("ProjectGroupRemove", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@ProjectGroupId", SqlDbType.Int, ProjectGroupId));
		}
		#endregion

		#region CheckForUnchangeableRoles
		public static int CheckForUnchangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("ProjectGroupsCheckForUnchangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ReplaceUnchangeableUser
		public static void ReplaceUnchangeableUser(int FromUserId, int ToUserId)
		{
			DbHelper2.RunSp("ProjectGroupsReplaceUnchangeableUser", 
				DbHelper2.mp("@FromUserId", SqlDbType.Int, FromUserId),
				DbHelper2.mp("@ToUserId", SqlDbType.Int, ToUserId));
		}
		#endregion
	}
}
