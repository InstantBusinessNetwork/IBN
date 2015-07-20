using System;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Database.ControlSystem;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for UserRoleHelper.
	/// </summary>
	public class UserRoleHelper
	{
		#region Constants 
		//<!-- Glolbal Role -->
		public const string AdminRoleName = "Admin";
		public const string PowerProjectManagerRoleName = "PPM";
		public const string HelpDeskManagerRoleName = "HDM";
		public const string GlobalProjectManagerRoleName = "GlobalPM";
		public const string GlobalExecutiveManagerRoleName = "GlobalEM";
		public const string TimeManagerRoleName = "TM";

		// <!-- Project Role -->
		public const string ProjectManagerRoleName = "PM";
		public const string ExecutiveManagerRoleName = "EM";
		public const string ProjectTeamRoleName = "PTeam";
		public const string ProjectSponsorRoleName = "PSponsor";
		public const string ProjectStakeHolderRoleName = "PStakeholder";

		// <!-- Issue Role -->
		public const string IssueCreatorRoleName = "ICreator";
		public const string IssueManagerRoleName = "IManager";
		public const string IssueResourceRoleName = "IResource";
		public const string IssueTodoResource = "ITodoResource";

		// <!-- Task Role -->
		public const string TaskCreatorRoleName = "TCreator";
		public const string TaskManagerRoleName = "TManager";
		public const string TaskResourceRoleName = "TResource";
		public const string TaskTodoResource = "TTodoResource";

		// <!-- Document Role -->
		public const string DocumentCreatorRoleName = "DCreator";
		public const string DocumentManagerRoleName = "DManager";
		public const string DocumentResourceRoleName = "DResource";
		public const string DocumentTodoResource = "DTodoResource";

		// <!-- Event Role -->
		public const string EventCreatorRoleName = "ECreator";
		public const string EventManagerRoleName = "EManager";
		public const string EventResourceRoleName = "EResource";

		// <!-- ToDo Role -->
		public const string TodoCreatorRoleName = "TodoCreator";
		public const string TodoManagerRoleName = "TodoManager";
		public const string TodoResourceRoleName = "TodoResource";

		#endregion

		private UserRoleHelper()
		{
		}

		#region -- CreateXXXContainerKey --
		public static string CreateProjectContainerKey(int projectId)
		{
			return string.Format("ProjectId_{0}",projectId);
		}

		public static string CreateIssueContainerKey(int issueId)
		{
			return string.Format("IncidentId_{0}",issueId);
		}

		public static string CreateTaskContainerKey(int taskId)
		{
			return string.Format("TaskId_{0}",taskId);
		}

		public static string CreateDocumentContainerKey(int taskId)
		{
			return string.Format("DocumentId_{0}",taskId);
		}

		public static string CreateTodoContainerKey(int todoId)
		{
			return string.Format("ToDoId_{0}",todoId);
		}

		public static string CreateEventContainerKey(int eventId)
		{
			return string.Format("EventId_{0}",eventId);
		}

    public static string CreateArticleContainerKey(int articleId)
    {
      return string.Format("ArticleId_{0}", articleId);
    }
		#endregion

		#region !!! Global Role !!!

		#region -- PPM --
		public static void AddPowerProjectManagerRole(int UserId)
		{
			UserRole.Add(PowerProjectManagerRoleName,UserId);
		}

		public static void DeletePowerProjectManagerRole(int UserId)
		{
			UserRole.Delete(PowerProjectManagerRoleName,UserId);
		}

		#endregion

		#region -- Admin --
		public static void AddAdminRole(int UserId)
		{
			UserRole.Add(AdminRoleName,UserId);
		}

		public static void DeleteAdminRole(int UserId)
		{
			UserRole.Delete(AdminRoleName,UserId);
		}

		#endregion
		#endregion

		#region !!! Project Roles !!!
		#region -- PM --
		public static void AddProjectManagerRole(int ProjectId, int UserId)
		{
			UserRole.Add(CreateProjectContainerKey(ProjectId), ProjectManagerRoleName,UserId, UserRoleTypeEnum.SingleValue);
		}

		public static void DeleteProjectManagerRole(int ProjectId, int UserId)
		{
			UserRole.Delete(CreateProjectContainerKey(ProjectId), ProjectManagerRoleName,UserId);
		}
		#endregion

		#region -- EM --
		public static void AddExecutiveManagerRole(int ProjectId, int UserId)
		{
			UserRole.Add(CreateProjectContainerKey(ProjectId), ExecutiveManagerRoleName,UserId, UserRoleTypeEnum.SingleValue);
		}

		public static void DeleteExecutiveManagerRole(int ProjectId, int UserId)
		{
			UserRole.Delete(CreateProjectContainerKey(ProjectId), ExecutiveManagerRoleName,UserId);
		}
		#endregion

		#region -- PTeam --
		public static void AddProjectTeamRole(int ProjectId, int UserId)
		{
			UserRole.Add(CreateProjectContainerKey(ProjectId), ProjectTeamRoleName,UserId);
		}

		public static void DeleteProjectTeamRole(int ProjectId, int UserId)
		{
			UserRole.Delete(CreateProjectContainerKey(ProjectId), ProjectTeamRoleName,UserId);
		}
		#endregion

		#region -- PSponsor --
		public static void AddProjectSponsorRole(int ProjectId, int UserId)
		{
			UserRole.Add(CreateProjectContainerKey(ProjectId), ProjectSponsorRoleName,UserId);
		}

		public static void DeleteProjectSponsorRole(int ProjectId, int UserId)
		{
			UserRole.Delete(CreateProjectContainerKey(ProjectId), ProjectSponsorRoleName,UserId);
		}
		#endregion

		#region -- PStakeHolder --
		public static void AddProjectStakeHolderRole(int ProjectId, int UserId)
		{
			UserRole.Add(CreateProjectContainerKey(ProjectId), ProjectStakeHolderRoleName,UserId);
		}

		public static void DeleteProjectStakeHolderRole(int ProjectId, int UserId)
		{
			UserRole.Delete(CreateProjectContainerKey(ProjectId), ProjectStakeHolderRoleName,UserId);
		}
		#endregion

		#region -- Delete All Project Roles
		public static void DeleteProjectRoles(int ProjectId)
		{
			UserRole.DeleteAll(CreateProjectContainerKey(ProjectId));
		}
		#endregion

		#endregion

		#region !!! Issue Roles !!!
		#region -- ICreator --
		public static void AddIssueCreatorRole(int IssueId, int UserId)
		{
			UserRole.Add(CreateIssueContainerKey(IssueId), IssueCreatorRoleName,UserId, UserRoleTypeEnum.SingleValue);
		}

		public static void DeleteIssueCreatorRole(int IssueId, int UserId)
		{
			UserRole.Delete(CreateIssueContainerKey(IssueId), IssueCreatorRoleName,UserId);
		}
		#endregion

		#region -- IManager --
		public static void AddIssueManagerRole(int IssueId, int UserId)
		{
			UserRole.Add(CreateIssueContainerKey(IssueId), IssueManagerRoleName,UserId);
		}

		public static void DeleteIssueManagerRole(int IssueId, int UserId)
		{
			UserRole.Delete(CreateIssueContainerKey(IssueId), IssueManagerRoleName,UserId);
		}
		#endregion

		#region -- IResource --
		public static void AddIssueResourceRole(int IssueId, int UserId)
		{
			UserRole.Add(CreateIssueContainerKey(IssueId), IssueResourceRoleName,UserId);
		}

		public static void DeleteIssueResourceRole(int IssueId, int UserId)
		{
			UserRole.Delete(CreateIssueContainerKey(IssueId), IssueResourceRoleName,UserId);
		}
		#endregion

		#region -- ITodoResource --
		public static void AddIssueTodoResourceRole(int IssueId, int UserId)
		{
			UserRole.Add(CreateIssueContainerKey(IssueId), IssueTodoResource,UserId);
		}

		public static void DeleteIssueTodoResourceRole(int IssueId, int UserId)
		{
			UserRole.Delete(CreateIssueContainerKey(IssueId), IssueTodoResource,UserId);
		}
		#endregion

		#region -- Delete All Issue Roles
		public static void DeleteIssueRoles(int IssueId)
		{
			UserRole.DeleteAll(CreateIssueContainerKey(IssueId));
		}
		#endregion
		#endregion

		#region !!! Task Roles !!!

		#region -- TCreator --
		public static void AddTaskCreatorRole(int TaskId, int UserId)
		{
			UserRole.Add(CreateTaskContainerKey(TaskId), TaskCreatorRoleName,UserId, UserRoleTypeEnum.SingleValue);
		}

		public static void DeleteTaskCreatorRole(int TaskId, int UserId)
		{
			UserRole.Delete(CreateTaskContainerKey(TaskId), TaskCreatorRoleName,UserId);
		}
		#endregion

		#region -- TManager --
		public static void AddTaskManagerRole(int TaskId, int UserId)
		{
			UserRole.Add(CreateTaskContainerKey(TaskId), TaskManagerRoleName,UserId);
		}

		public static void DeleteTaskManagerRole(int TaskId, int UserId)
		{
			UserRole.Delete(CreateTaskContainerKey(TaskId), TaskManagerRoleName,UserId);
		}
		#endregion

		#region -- TResource --
		public static void AddTaskResourceRole(int TaskId, int UserId)
		{
			UserRole.Add(CreateTaskContainerKey(TaskId), TaskResourceRoleName,UserId);
		}

		public static void DeleteTaskResourceRole(int TaskId, int UserId)
		{
			UserRole.Delete(CreateTaskContainerKey(TaskId), TaskResourceRoleName,UserId);
		}
		#endregion

		#region -- TTodoResource --
		public static void AddTaskTodoResourceRole(int TaskId, int UserId)
		{
			UserRole.Add(CreateTaskContainerKey(TaskId), TaskTodoResource,UserId);
		}

		public static void DeleteTaskTodoResourceRole(int TaskId, int UserId)
		{
			UserRole.Delete(CreateTaskContainerKey(TaskId), TaskTodoResource,UserId);
		}
		#endregion

		#region -- Delete All Task Roles
		public static void DeleteTaskRoles(int TaskId)
		{
			UserRole.DeleteAll(CreateTaskContainerKey(TaskId));
		}
		#endregion

		#endregion

		#region !!! Document Roles !!!

		#region -- DCreator --
		public static void AddDocumentCreatorRole(int DocumentId, int UserId)
		{
			UserRole.Add(CreateDocumentContainerKey(DocumentId), DocumentCreatorRoleName,UserId, UserRoleTypeEnum.SingleValue);
		}

		public static void DeleteDocumentCreatorRole(int DocumentId, int UserId)
		{
			UserRole.Delete(CreateDocumentContainerKey(DocumentId), DocumentCreatorRoleName,UserId);
		}
		#endregion

		#region -- DManager --
		public static void AddDocumentManagerRole(int DocumentId, int UserId)
		{
			UserRole.Add(CreateDocumentContainerKey(DocumentId), DocumentManagerRoleName,UserId);
		}

		public static void DeleteDocumentManagerRole(int DocumentId, int UserId)
		{
			UserRole.Delete(CreateDocumentContainerKey(DocumentId), DocumentManagerRoleName,UserId);
		}
		#endregion

		#region -- DResource --
		public static void AddDocumentResourceRole(int DocumentId, int UserId)
		{
			UserRole.Add(CreateDocumentContainerKey(DocumentId), DocumentResourceRoleName,UserId);
		}

		public static void DeleteDocumentResourceRole(int DocumentId, int UserId)
		{
			UserRole.Delete(CreateDocumentContainerKey(DocumentId), DocumentResourceRoleName,UserId);
		}
		#endregion

		#region -- DTodoResource --
		public static void AddDocumentTodoResourceRole(int DocumentId, int UserId)
		{
			UserRole.Add(CreateDocumentContainerKey(DocumentId), DocumentTodoResource,UserId);
		}

		public static void DeleteDocumentTodoResourceRole(int DocumentId, int UserId)
		{
			UserRole.Delete(CreateDocumentContainerKey(DocumentId), DocumentTodoResource,UserId);
		}
		#endregion

		#region -- Delete All Document Roles
		public static void DeleteDocumentRoles(int DocumentId)
		{
			UserRole.DeleteAll(CreateDocumentContainerKey(DocumentId));
		}
		#endregion
		#endregion

		#region !!! Event Roles !!!

		#region -- ECreator --
		public static void AddEventCreatorRole(int EventId, int UserId)
		{
			UserRole.Add(CreateEventContainerKey(EventId), EventCreatorRoleName,UserId, UserRoleTypeEnum.SingleValue);
		}

		public static void DeleteEventCreatorRole(int EventId, int UserId)
		{
			UserRole.Delete(CreateEventContainerKey(EventId), EventCreatorRoleName,UserId);
		}
		#endregion

		#region -- EManager --
		public static void AddEventManagerRole(int EventId, int UserId)
		{
			UserRole.Add(CreateEventContainerKey(EventId), EventManagerRoleName,UserId, UserRoleTypeEnum.SingleValue);
		}

		public static void DeleteEventManagerRole(int EventId, int UserId)
		{
			UserRole.Delete(CreateEventContainerKey(EventId), EventManagerRoleName,UserId);
		}
		#endregion

		#region --EResource --
		public static void AddEventResourceRole(int EventId, int UserId)
		{
			UserRole.Add(CreateEventContainerKey(EventId), EventResourceRoleName,UserId);
		}

		public static void DeleteEventResourceRole(int EventId, int UserId)
		{
			UserRole.Delete(CreateEventContainerKey(EventId), EventResourceRoleName,UserId);
		}
		#endregion

		#region -- Delete All Event Roles
		public static void DeleteEventRoles(int EventId)
		{
			UserRole.DeleteAll(CreateEventContainerKey(EventId));
		}
		#endregion

		#endregion

		#region !!! Todo Roles !!!

		#region -- TodoCreator --
		public static void AddTodoCreatorRole(int TodoId, int UserId)
		{
			UserRole.Add(CreateTodoContainerKey(TodoId), TodoCreatorRoleName,UserId, UserRoleTypeEnum.SingleValue);
		}

		public static void DeleteTodoCreatorRole(int TodoId, int UserId)
		{
			UserRole.Delete(CreateTodoContainerKey(TodoId), TodoCreatorRoleName,UserId);
		}
		#endregion

		#region -- TodoManager --
		public static void AddTodoManagerRole(int TodoId, int UserId)
		{
			UserRole.Add(CreateTodoContainerKey(TodoId), TodoManagerRoleName,UserId, UserRoleTypeEnum.SingleValue);
		}

		public static void DeleteTodoManagerRole(int TodoId, int UserId)
		{
			UserRole.Delete(CreateTodoContainerKey(TodoId), TodoManagerRoleName,UserId);
		}
		#endregion

		#region --TodoResource --
		public static void AddTodoResourceRole(int TodoId, int UserId)
		{
			UserRole.Add(CreateTodoContainerKey(TodoId), TodoResourceRoleName,UserId);
		}

		public static void DeleteTodoResourceRole(int TodoId, int UserId)
		{
			UserRole.Delete(CreateTodoContainerKey(TodoId), TodoResourceRoleName,UserId);
		}
		#endregion

		#region -- Delete All Todo Roles
		public static void DeleteTodoRoles(int TodoId)
		{
			UserRole.DeleteAll(CreateTodoContainerKey(TodoId));
		}
		#endregion

		#endregion
	}
}
