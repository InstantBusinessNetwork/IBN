using System;
using System.Data;

using DbHelper2 = Mediachase.IBN.Database.DbHelper2;

namespace Mediachase.Ibn.Apps.ClioSoft
{
	public class DbReport
	{
		#region GetProjectManagers
		/// <summary>
		/// Gets the project managers.
		/// </summary>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <returns>ManagerId, ManagerName</returns>
		public static IDataReader GetProjectManagers(DateTime fromDate, DateTime toDate)
		{
			return DbHelper2.RunSpDataReader("custom_ClioSoft_ProjectManagersGet",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, toDate));
		}
		#endregion

		#region GetClients
		/// <summary>
		/// Gets the clients.
		/// </summary>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <param name="managers">The managers.</param>
		/// <returns>ClientId, ClientName</returns>
		public static IDataReader GetClients(DateTime fromDate, DateTime toDate, string managers)
		{
			return DbHelper2.RunSpDataReader("custom_ClioSoft_ClientsGet",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, toDate),
				DbHelper2.mp("@Managers", SqlDbType.VarChar, 8000, managers));
		}
		#endregion

		#region GetProjectGroups
		/// <summary>
		/// Gets the project groups.
		/// </summary>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <param name="managers">The managers.</param>
		/// <param name="clients">The clients.</param>
		/// <returns>ProjectGroupId, ProjectGroupName</returns>
		public static IDataReader GetProjectGroups(DateTime fromDate, DateTime toDate, string managers, string clients)
		{
			return DbHelper2.RunSpDataReader("custom_ClioSoft_ProjectGroupsGet",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, toDate),
				DbHelper2.mp("@Managers", SqlDbType.VarChar, 8000, managers),
				DbHelper2.mp("@Clients", SqlDbType.VarChar, 8000, clients));
		}
		#endregion

		#region GetProjects
		/// <summary>
		/// Gets the projects.
		/// </summary>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <param name="managers">The managers.</param>
		/// <param name="clients">The clients.</param>
		/// <param name="projectGroups">The project groups.</param>
		/// <param name="completion">The completion.</param>
		/// <param name="completionFromDate">The completion from date.</param>
		/// <param name="completionToDate">The completion to date.</param>
		/// <returns></returns>
		public static IDataReader GetProjects(DateTime fromDate, DateTime toDate, string managers, string clients, string projectGroups, int completion, DateTime completionFromDate, DateTime completionToDate)
		{
			return DbHelper2.RunSpDataReader("custom_ClioSoft_ProjectsGet",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, toDate),
				DbHelper2.mp("@Managers", SqlDbType.VarChar, 8000, managers),
				DbHelper2.mp("@Clients", SqlDbType.VarChar, 8000, clients),
				DbHelper2.mp("@ProjectGroups", SqlDbType.VarChar, 8000, projectGroups),
				DbHelper2.mp("@Completion", SqlDbType.Int, completion),
				DbHelper2.mp("@CompletionFromDate", SqlDbType.DateTime, completionFromDate),
				DbHelper2.mp("@CompletionToDate", SqlDbType.DateTime, completionToDate));
		}
		#endregion

		#region GetUserReportInfo
		/// <summary>
		/// Gets the user report info.
		/// </summary>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <param name="ownerId">The owner id.</param>
		/// <param name="projectId">The project id.</param>
		/// <param name="completion">The completion.</param>
		/// <param name="completionFromDate">The completion from date.</param>
		/// <param name="completionToDate">The completion to date.</param>
		/// <returns></returns>
		public static IDataReader GetUserReportInfo(DateTime fromDate, DateTime toDate, int ownerId, int projectId, int completion, DateTime completionFromDate, DateTime completionToDate)
		{
			return DbHelper2.RunSpDataReader("custom_ClioSoft_UserReportGetInfo",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, toDate),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, ownerId),
				DbHelper2.mp("@Completion", SqlDbType.Int, completion),
				DbHelper2.mp("@CompletionFromDate", SqlDbType.DateTime, completionFromDate),
				DbHelper2.mp("@CompletionToDate", SqlDbType.DateTime, completionToDate));
		}
		#endregion

		#region GetUsers
		/// <summary>
		/// Gets the users.
		/// </summary>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <returns>UserId, UserName</returns>
		public static IDataReader GetUsers(DateTime fromDate, DateTime toDate)
		{
			return DbHelper2.RunSpDataReader("custom_ClioSoft_UsersGet",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, toDate));
		}
		#endregion

		#region GetClientsByUser
		/// <summary>
		/// Gets the clients by user.
		/// </summary>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <param name="users">The users.</param>
		/// <returns>ClientId, ClientName</returns>
		public static IDataReader GetClientsByUser(DateTime fromDate, DateTime toDate, string users)
		{
			return DbHelper2.RunSpDataReader("custom_ClioSoft_ClientsGetByUser",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, toDate),
				DbHelper2.mp("@Users", SqlDbType.VarChar, 8000, users));
		}
		#endregion

		#region GetProjectGroupsByUser
		/// <summary>
		/// Gets the project groups by user.
		/// </summary>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <param name="users">The users.</param>
		/// <param name="clients">The clients.</param>
		/// <returns>ProjectGroupId, ProjectGroupName</returns>
		public static IDataReader GetProjectGroupsByUser(DateTime fromDate, DateTime toDate, string users, string clients)
		{
			return DbHelper2.RunSpDataReader("custom_ClioSoft_ProjectGroupsGetByUser",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, toDate),
				DbHelper2.mp("@Users", SqlDbType.VarChar, 8000, users),
				DbHelper2.mp("@Clients", SqlDbType.VarChar, 8000, clients));
		}
		#endregion

		#region GetProjectsByUser
		/// <summary>
		/// Gets the projects by user.
		/// </summary>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <param name="users">The users.</param>
		/// <param name="clients">The clients.</param>
		/// <param name="projectGroups">The project groups.</param>
		/// <param name="completion">The completion.</param>
		/// <param name="completionFromDate">The completion from date.</param>
		/// <param name="completionToDate">The completion to date.</param>
		/// <returns></returns>
		public static IDataReader GetProjectsByUser(DateTime fromDate, DateTime toDate, string users, string clients, string projectGroups, int completion, DateTime completionFromDate, DateTime completionToDate)
		{
			return DbHelper2.RunSpDataReader("custom_ClioSoft_ProjectsGetByUser",
				DbHelper2.mp("@FromDate", SqlDbType.DateTime, fromDate),
				DbHelper2.mp("@ToDate", SqlDbType.DateTime, toDate),
				DbHelper2.mp("@Users", SqlDbType.VarChar, 8000, users),
				DbHelper2.mp("@Clients", SqlDbType.VarChar, 8000, clients),
				DbHelper2.mp("@ProjectGroups", SqlDbType.VarChar, 8000, projectGroups),
				DbHelper2.mp("@Completion", SqlDbType.Int, completion),
				DbHelper2.mp("@CompletionFromDate", SqlDbType.DateTime, completionFromDate),
				DbHelper2.mp("@CompletionToDate", SqlDbType.DateTime, completionToDate));
		}
		#endregion

		#region GetTasks
		/// <summary>
		/// Gets the tasks.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <param name="completion">The completion.</param>
		/// <param name="completionFromDate">The completion from date.</param>
		/// <param name="completionToDate">The completion to date.</param>
		/// <returns></returns>
		public static IDataReader GetTasks(int projectId, int completion, DateTime completionFromDate, DateTime completionToDate)
		{
			return DbHelper2.RunSpDataReader("custom_ClioSoft_TasksGet",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@Completion", SqlDbType.Int, completion),
				DbHelper2.mp("@CompletionFromDate", SqlDbType.DateTime, completionFromDate),
				DbHelper2.mp("@CompletionToDate", SqlDbType.DateTime, completionToDate));
		}
		#endregion
	}
}
