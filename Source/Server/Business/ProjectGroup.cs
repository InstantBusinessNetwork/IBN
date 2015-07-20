using System;
using System.Collections;
using System.Data;

using Mediachase.Ibn;
using Mediachase.IBN.Database;


namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for ProjectGroup.
	/// </summary>
	public class ProjectGroup
	{
		#region CanDelete
		public static bool CanDelete()
		{
			return Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)
				|| Security.IsUserInGroup(InternalSecureGroups.ProjectManager);
		}
		#endregion

		#region CanUpdate
		public static bool CanUpdate()
		{
			return Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)
				|| Security.IsUserInGroup(InternalSecureGroups.ProjectManager);
		}
		#endregion

		#region CanAdd
		public static bool CanAdd()
		{
			bool IsEnabled = false;
			if (License.PortfolioCount >= 0)
			{
				DataTable dt = DBProjectGroup.GetProjectGroupsDT(Security.CurrentUser.TimeZoneId);
				IsEnabled = (dt.Rows.Count < License.PortfolioCount);
			}
			else
			{
				IsEnabled = true;
			}

			return CanUpdate() && IsEnabled;
		}
		#endregion


		#region Create
		public static int Create(string Title, string Description)
		{
			if (!CanAdd())
				throw new AccessDeniedException();

			int id;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				id = DBProjectGroup.Create(Title, Description, Security.CurrentUser.UserID, DateTime.UtcNow);

				//SendAlert(AlertEventType.Portfolio_New, id);

				tran.Commit();
			}
			return id;
		}
		#endregion

		#region Update

		public static void Update(int ProjectGroupId, string Title, string Description)
		{
			if (!CanUpdate())
				throw new AccessDeniedException();

			DBProjectGroup.Update(ProjectGroupId, Title, Description);
		}
		#endregion

		#region Delete
		public static void Delete(int ProjectGroupId)
		{
			if (!CanDelete())
				throw new AccessDeniedException();

			using (DbTransaction tran = DbTransaction.Begin())
			{
				//SendAlert(AlertEventType.Portfolio_Deleted, ProjectGroupId);
				DBProjectGroup.Delete(ProjectGroupId);

				tran.Commit();
			}
		}

		#endregion

		#region GetProjectGroups
		/*
			[ProjectGroupId], [Title], [Description], [CreationDate], [CreatorId]
		*/

		public static IDataReader GetProjectGroups()
		{
			return DBProjectGroup.GetProjectGroups(User.DefaultTimeZoneId);
		}

		public static DataTable GetProjectGroupsDT()
		{
			return DBProjectGroup.GetProjectGroupsDT(User.DefaultTimeZoneId);
		}
		#endregion

		#region GetProjectGroup
		/*
			[ProjectGroupId], [Title], [Description], [CreationDate], [CreatorId]
		*/

		public static IDataReader GetProjectGroups(int ProjectGroupId)
		{
			return DBProjectGroup.GetProjectGroup(ProjectGroupId, User.DefaultTimeZoneId);
		}
		#endregion

		#region ProjectGroupsGetByProject
		//A.[ProjectGroupId], B.[Title] 
		public static IDataReader ProjectGroupsGetByProject(int ProjectId)
		{
			return DBProjectGroup.ProjectGroupsGetByProject(ProjectId);
		}

		#endregion

		#region AssignProjectToProjectGroup
		public static void AssignProjectGroup(int ProjectId, int ProjectGroupId)
		{
			if (!CanUpdate())
				throw new AccessDeniedException();

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBProjectGroup.AssignProjectGroup(ProjectId, ProjectGroupId);
				//SendAlert(AlertEventType.Portfolio_ProjectNew, ProjectGroupId, ProjectId);

				tran.Commit();
			}
		}
		#endregion

		#region RemoveProjectFromProjectGroup
		public static void RemoveProjectFromProjectGroup(int ProjectId, int ProjectGroupId)
		{
			if (!CanUpdate())
				throw new AccessDeniedException();

			using (DbTransaction tran = DbTransaction.Begin())
			{
				//SendAlert(AlertEventType.Portfolio_ProjectDeleted, ProjectGroupId, ProjectId);
				DBProjectGroup.RemoveProjectFromProjectGroup(ProjectId, ProjectGroupId);

				tran.Commit();
			}
		}
		#endregion

		#region GetProjectGroupTitle
		public static string GetProjectGroupTitle(int ProjectGroupId)
		{
			string ProjectGroupTitle = "";
			using (IDataReader reader = DBProjectGroup.GetProjectGroup(ProjectGroupId, Security.CurrentUser.TimeZoneId))
			{
				if (reader.Read())
					ProjectGroupTitle = reader["Title"].ToString();
			}

			return ProjectGroupTitle;
		}
		#endregion
	}
}
