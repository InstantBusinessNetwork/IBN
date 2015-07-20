using System;
using System.Collections;
using System.Data;

using Mediachase.Ibn;
using Mediachase.IBN.Database;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Business.EMail;
using Mediachase.IBN.Database.EMail;
using Mediachase.Net.Mail;
using Mediachase.Ibn.Data;
using System.Collections.Generic;

namespace Mediachase.IBN.Business
{
	public class Issue2
	{
		// Private:
		private const ObjectTypes OBJECT_TYPE = ObjectTypes.Issue;

		#region VerifyCanUpdate()
		private static void VerifyCanUpdate(int objectId)
		{
			if(!Incident.CanUpdate(objectId))
				throw new AccessDeniedException();
		}
		#endregion

		#region VerifyCanModifyResources()
		private static void VerifyCanModifyResources(int objectId)
		{
			if (!Incident.CanModifyResources(objectId))
				throw new AccessDeniedException();
		}
		#endregion

		#region LoadGeneralCategories()
		private static ArrayList LoadGeneralCategories(int issueId)
		{
			ArrayList ret = new ArrayList();
			using(IDataReader reader = Incident.GetListCategories(issueId))
			{
				Common.LoadCategories(reader, ret);
			}
			return ret;
		}
		#endregion
		#region LoadIssueCategories()
		public static ArrayList LoadIssueCategories(int issueId)
		{
			ArrayList ret = new ArrayList();
			using(IDataReader reader = Incident.GetListIncidentCategoriesByIncident(issueId))
			{
				Common.LoadCategories(reader, ret);
			}
			return ret;
		}
		#endregion

		#region UpdateProject()
		public static void UpdateProject(int issueId, int projectId)
		{
			UpdateProject(issueId, projectId, true);
		}

		internal static void UpdateProject(int issueId, int projectId, bool checkAccess)
		{
			if (projectId == 0)
				return;

			int oldProjectId = DBIncident.GetProject(issueId);

			if (checkAccess && projectId != oldProjectId && !Incident.CanChangeProject(issueId))
				throw new AccessDeniedException();

			if(projectId != oldProjectId)
			{
				List<int> todoList = new List<int>();
				using (IDataReader reader = Incident.GetListToDo(issueId))
				{
					while (reader.Read())
						todoList.Add((int)reader["ToDoId"]);
				}

				int managerId;
				if (projectId > 0)
					managerId = Project.GetProjectManager(projectId);
				else
					managerId = Security.CurrentUser.UserID;

				using(DbTransaction tran = DbTransaction.Begin())
				{
					DbIssue2.UpdateProject(issueId, projectId);

					if(projectId > 0)
						SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_IssueList_IssueAdded, projectId, issueId);
					else
						SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_IssueList_IssueDeleted, oldProjectId, issueId);


					// O.R.[2009-09-29] Incident todo's
					foreach (int todoId in todoList)
					{
						int oldToDoManagerId = DBToDo.GetManager(todoId);

						DBToDo.UpdateProjectAndManager(todoId, projectId, oldToDoManagerId);

						ForeignContainerKey.Delete(UserRoleHelper.CreateTodoContainerKey(todoId), UserRoleHelper.CreateProjectContainerKey(oldProjectId));
						if (projectId > 0)
							ForeignContainerKey.Add(UserRoleHelper.CreateTodoContainerKey(todoId), UserRoleHelper.CreateProjectContainerKey(projectId));
					}

					if (oldProjectId > 0)
						TimeTracking.RecalculateProjectTaskTime(oldProjectId);
					if (projectId > 0)
						TimeTracking.RecalculateProjectTaskTime(projectId);

					tran.Commit();
				}
			}
		}
		#endregion
		
		#region UpdateResponsibleGroup()
		internal static void UpdateResponsibleGroup(int objectId, DataTable items)
		{
			UserLight cu = Security.CurrentUser;
			int cuid = cu.UserID;
			int timeZoneId = cu.TimeZoneId;
			DateTime now = DateTime.UtcNow;

			int stateId;
			using(IDataReader reader = DBIncident.GetIncident(objectId, timeZoneId, cu.LanguageId))
			{
				reader.Read();
				stateId = (int)reader["StateId"];
			}

			ArrayList oldItems = new ArrayList();
			using(IDataReader reader = DBIncident.GetResponsibleGroup(objectId, timeZoneId))
			{
				Common.LoadPrincipals(reader, oldItems);
			}

			ArrayList add = new ArrayList();
			ArrayList del = new ArrayList();
			foreach(DataRow row in items.Rows)
			{
				int id = (int)row["PrincipalId"];
				if(oldItems.Contains(id))
					oldItems.Remove(id);
				else
					add.Add(id);
			}

			del.AddRange(oldItems);

			foreach(int id in del)
			{
				DBIncident.DeleteResponsible(objectId, id);
				SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Responsible_Resigned, objectId, id);
			}

			foreach(DataRow row in items.Rows)
			{
				int id = (int)row["PrincipalId"];
				bool IsNew = (bool)row["IsNew"];
				bool ResponsePending = (bool)row["ResponsePending"];
					
				bool updated = true;
				if(add.Contains(id))
					DbIssue2.ResponsibleGroupAddUser(objectId, id);
				else
					updated = (IsNew && DbIssue2.ResponsibleGroupUserUpdate(objectId, id, ResponsePending) > 0);

				if(updated)
					SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Responsible_Requested, objectId, id);
			}
		}
		#endregion
		
		#region UpdateListResources()
		internal static void UpdateListResources(int objectId, DataTable items, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanModifyResources(objectId);

			UserLight cu = Security.CurrentUser;
			int cuid = cu.UserID;
			int timeZoneId = cu.TimeZoneId;
			DateTime now = DateTime.UtcNow;

			int stateId;
			using(IDataReader reader = DBIncident.GetIncident(objectId, timeZoneId, cu.LanguageId))
			{
				reader.Read();
				stateId = (int)reader["StateId"];
			}

			ArrayList oldItems = new ArrayList();
			using(IDataReader reader = DBIncident.GetListResources(objectId, timeZoneId))
			{
				Common.LoadPrincipals(reader, oldItems);
			}

			ArrayList add = new ArrayList();
			ArrayList del = new ArrayList();
			foreach(DataRow row in items.Rows)
			{
				int id = (int)row["PrincipalId"];
				if(oldItems.Contains(id))
					oldItems.Remove(id);
				else
					add.Add(id);
			}

			del.AddRange(oldItems);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if(stateId == (int)ObjectStates.Upcoming)
					DbIssue2.UpdateState(objectId, (int)ObjectStates.Active);

				foreach(int id in del)
				{
					DBCommon.DeleteGate((int)OBJECT_TYPE, objectId, id);
					DBIncident.DeleteResource(objectId, id);

					SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_ResourceList_AssignmentDeleted, objectId, id);
				}

				foreach(DataRow row in items.Rows)
				{
					int id = (int)row["PrincipalId"];
					bool mustBeConfirmed = (bool)row["MustBeConfirmed"];
					bool canManage = (bool)row["CanManage"];
					bool updated = true;
					
					if(add.Contains(id))
					{
						DbIssue2.ResourceAdd(objectId, id, mustBeConfirmed, canManage);
						if (User.IsExternal(id))
							DBCommon.AddGate((int)OBJECT_TYPE, objectId, id);
					}
					else
						updated = (0 < DbIssue2.ResourceUpdate(objectId, id, mustBeConfirmed, canManage));

					if(updated)
					{
						if(mustBeConfirmed)
							SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_ResourceList_RequestAdded, objectId, id);
						else
							SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_ResourceList_AssignmentAdded, objectId, id);
					}
				}

				// O.R.[2008-12-16]: Recalculate Current Responsible
				DBIncident.RecalculateCurrentResponsible(objectId);

				tran.Commit();
			}
		}
		#endregion

		#region ListUpdate()
		private static void ListUpdate(bool add, ObjectTypes objectType, int objectId, int itemId, object context)
		{
			if(add)
				DBIncident.AssignIncidentCategory(objectId, itemId);
			else
				DBIncident.RemoveCategoryFromIncident(objectId, itemId);
		}
		#endregion

		// Public:
		#region UpdateGeneralInfo()
		public static void UpdateGeneralInfo(int issueId, string title, string description, int typeId, int severityId)
		{
			UpdateGeneralInfo(issueId, title, description, typeId, severityId, true);
		}

		internal static void UpdateGeneralInfo(int issueId, string title, string description, int typeId, int severityId, bool checkAccess)
		{
			if(checkAccess && !Incident.CanUpdate(issueId))
				throw new AccessDeniedException();

			title = title.Replace("<", "&lt;").Replace(">", "&gt;");

			string oldTitle = "";
			string oldDescription = "";
			int oldTypeId = -1;
			int oldSeverityId = -1;

			using (IDataReader reader = DBIncident.GetIncident(issueId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (reader.Read())
				{
					oldTitle = reader["Title"].ToString();
					oldDescription = reader["Description"].ToString();
					oldTypeId = (int)reader["TypeId"];
					oldSeverityId = (int)reader["SeverityId"];
				}
			}

			if (title == oldTitle && description == oldDescription && oldTypeId == typeId && oldSeverityId == severityId)
				return;

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DbIssue2.UpdateGeneralInfo(issueId, title, description, typeId, severityId);
				SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_GeneralInfo, issueId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdatePriority()
		public static void UpdatePriority(int issueId, int priorityId)
		{
			UpdatePriority(issueId, priorityId, true);
		}

		internal static void UpdatePriority(int issueId, int priorityId, bool checkAccess)
		{
			if(checkAccess && !Incident.CanUpdate(issueId))
				throw new AccessDeniedException();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if(0 < DbIssue2.UpdatePriority(issueId, priorityId))
					SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Priority, issueId);

				tran.Commit();
			}
		}
		#endregion

		#region UpPriority()
		public static void UpPriority(int issueId)
		{
			UpPriority(issueId, true);
		}

		public static void UpPriority(int issueId, bool checkAccess)
		{
			if (checkAccess && !Incident.CanUpdate(issueId))
				throw new AccessDeniedException();

			int priorityId = -1;
			using (IDataReader reader = Incident.GetIncident(issueId))
			{
				if (reader.Read())
				{
					priorityId = (int)reader["PriorityId"];
				}
			}

			Priority priority = (Priority)priorityId;
			bool needChange = true;
			switch (priority)
			{
				case Priority.Low:
					priorityId = (int)Priority.Normal;
					break;
				case Priority.Normal:
					priorityId = (int)Priority.High;
					break;
				case Priority.High:
					priorityId = (int)Priority.VeryHigh;
					break;
				case Priority.VeryHigh:
					priorityId = (int)Priority.Urgent;
					break;
				case Priority.Urgent:
					needChange = false;
					break;
				default:
					needChange = false;
					break;
			}

			if (needChange)
			{
				UpdatePriority(issueId, priorityId, false);
			}
		}
		#endregion

		#region DownPriority()
		public static void DownPriority(int issueId)
		{
			DownPriority(issueId, true);
		}

		internal static void DownPriority(int issueId, bool checkAccess)
		{
			if (checkAccess && !Incident.CanUpdate(issueId))
				throw new AccessDeniedException();

			int priorityId = -1;
			using (IDataReader reader = Incident.GetIncident(issueId))
			{
				if (reader.Read())
				{
					priorityId = (int)reader["PriorityId"];
				}
			}

			Priority priority = (Priority)priorityId;
			bool needChange = true;
			switch (priority)
			{
				case Priority.Low:
					needChange = false;
					break;
				case Priority.Normal:
					priorityId = (int)Priority.Low;
					break;
				case Priority.High:
					priorityId = (int)Priority.Normal;
					break;
				case Priority.VeryHigh:
					priorityId = (int)Priority.High;
					break;
				case Priority.Urgent:
					priorityId = (int)Priority.VeryHigh;
					break;
				default:
					needChange = false;
					break;
			}

			if (needChange)
			{
				UpdatePriority(issueId, priorityId, false);
			}
		}
		#endregion
	
		#region UpdateState()
		public static void UpdateState(int issueId, int stateId)
		{
			UpdateState(issueId, stateId, true);
		}

		internal static void UpdateState(int issueId, int stateId, bool checkAccess)
		{
			int oldStateId = -1;
			using(IDataReader reader = Incident.GetIncident(issueId, false))
			{
				if(reader.Read())
					oldStateId = (int)reader["StateId"];
			}
			
			if(oldStateId == stateId)
				return;

			if(checkAccess && !Incident.IsTransitionAllowed(issueId,(ObjectStates)oldStateId,(ObjectStates)stateId))
				throw new AccessDeniedException();

			UpdateStateAndNotifyController(issueId, stateId, -1);
		}
		#endregion

		#region UpdateStateAndNotifyController()
		internal static bool UpdateStateAndNotifyController(int issueId, int stateId, int forumNodeId)
		{
			bool updated = false;

			int controllerId, incidentBoxId;
			UserLight cu = Security.CurrentUser;
			using(IDataReader reader = DBIncident.GetIncident(issueId, cu.TimeZoneId, cu.LanguageId))
			{
				reader.Read();
				controllerId = DBCommon.NullToInt32(reader["ControllerId"]);
				incidentBoxId = DBCommon.NullToInt32(reader["IncidentBoxId"]);
			}
			IncidentBoxDocument IncDoc = IncidentBoxDocument.Load(incidentBoxId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				updated = (DbIssue2.UpdateState(issueId, stateId) > 0);

				if(stateId == (int)ObjectStates.Completed || stateId == (int)ObjectStates.Suspended)
				{
					// 2007-02-15: OZ: New Messagew Addon
					Issue2.SetNewMessage(issueId, false);
					//
				}

				if(updated)
				{
					SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Status, issueId);
					IncidentForum.AddForumMesageWithStateChange(issueId, forumNodeId, stateId);

					if(stateId == (int)ObjectStates.OnCheck)
					{
						// Notify controller
						if(controllerId > 0)
							SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Controller_Assigned, issueId, controllerId);
					}

					if(stateId != (int)ObjectStates.Active && stateId != (int)ObjectStates.ReOpen && stateId != (int)ObjectStates.Upcoming)
					{
						if(IncDoc.GeneralBlock.ReassignResponsibileOnReOpen)
						{
							if(IncDoc.GeneralBlock.ResponsibleAssignType == ResponsibleAssignType.Pool)
								DbIssue2.UpdateResponsibility(issueId, -1, true);
							if(IncDoc.GeneralBlock.ResponsibleAssignType == ResponsibleAssignType.Manual)
								DbIssue2.UpdateResponsibility(issueId, -1, false);
							if(IncDoc.GeneralBlock.ResponsibleAssignType == ResponsibleAssignType.CustomUser)
								DbIssue2.UpdateResponsibility(issueId, IncDoc.GeneralBlock.Responsible, false);
						}
					}


					// 2008-08-11 Send informational message to external recipient about Incident Closed
					if (stateId == (int)ObjectStates.Completed && IncDoc.EMailRouterBlock.SendAutoIncidentClosed)
					{
						string[] toList = EMailIssueExternalRecipient.StringList(issueId);

						if (toList.Length > 0)
						{
							string emailFrom = EMail.EMailRouterOutputMessage.FindEMailRouterPublicEmail(issueId);
							EMailRouterOutputMessage.SendAutoIncidentClosed(issueId, emailFrom, toList);
						}
					}

					// O.R. [2009-02-12]
					if (stateId == (int)ObjectStates.OnCheck || stateId == (int)ObjectStates.Suspended || stateId == (int)ObjectStates.Completed)
						DBCalendar.DeleteStickedObjectForAllUsers((int)OBJECT_TYPE, issueId);

					// O.R.[2008-12-16]: Recalculate Current Responsible
					DBIncident.RecalculateCurrentResponsible(issueId);
				}

				tran.Commit();
			}
			return updated;
		}
		#endregion

		#region UpdateResources()
		public static void UpdateResources(int issueId, DataTable resources)
		{
			UpdateListResources(issueId, resources, true);
		}
		#endregion

		#region AddRelation
		public static void AddRelation(int issueId, int relIssueId)
		{
			VerifyCanUpdate(issueId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBIncident.AddIncidentRelation(issueId, relIssueId);

				SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_RelatedIssueList_RelatedIssueAdded, issueId, relIssueId);
				SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_RelatedIssueList_RelatedIssueAdded, relIssueId, issueId);

				tran.Commit();
			}
		}
		#endregion
		
		#region DeleteRelation
		public static void DeleteRelation(int issueId, int relIssueId)
		{
			VerifyCanUpdate(issueId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBIncident.DeleteIncidentRelation(issueId, relIssueId);

				SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_RelatedIssueList_RelatedIssueDeleted, issueId, relIssueId);
				SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_RelatedIssueList_RelatedIssueDeleted, relIssueId, issueId);

				tran.Commit();
			}
		}
		#endregion

		#region AcceptResource
		public static void AcceptResource(int issueId)
		{
			int userId = Security.CurrentUser.UserID;

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DbIssue2.ResourceReply(issueId, userId, true);
				
				SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_ResourceList_RequestAccepted, issueId, userId);

				tran.Commit();
			}
		}
		#endregion
		
		#region DeclineResource
		public static void DeclineResource(int issueId)
		{
			int userId = Security.CurrentUser.UserID;

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DbIssue2.ResourceReply(issueId, userId, false);

				SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_ResourceList_RequestDenied, issueId, userId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateMailSenderEmail()
		/// <summary>
		/// Updates the mail sender email.
		/// </summary>
		/// <param name="issueId">The issue id.</param>
		/// <param name="email">The mail sender email.</param>
		public static void UpdateMailSenderEmail(int issueId, string email)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				DbIssue2.UpdateMailSenderEmail(issueId, email);
				DBCommon.AddGate((int)ObjectTypes.Issue, issueId, email);

//				SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Forum_MessageAdded, issueId, email);
				
				tran.Commit();
			}
		}
		#endregion

		#region DeleteForumMessage()
		public static void DeleteForumMessage(int issueId, int nodeId)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				BaseIbnContainer destContainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", issueId));
				ForumStorage forumStorage = (ForumStorage)destContainer.LoadControl("ForumStorage");

                // Delete Emails
                ForumThreadNodeInfo threadNode = forumStorage.GetForumThreadNode(nodeId);
                if (threadNode.ContentType == ForumStorage.NodeContentType.EMail)
                {
                    EMailMessage.Delete(threadNode.EMailMessageId);
                }

                // CleanUp Attached Files
                BaseIbnContainer attachedFilesContainer = BaseIbnContainer.Create("FileLibrary", string.Format("ForumNodeId_{0}", nodeId));
                FileStorage attachedFilesStorage = (FileStorage)attachedFilesContainer.LoadControl("FileStorage");
                attachedFilesStorage.DeleteAll();
                // end

                forumStorage.DeleteForumThreadNode(nodeId);
       
				tran.Commit();
			}
		}
		#endregion

		#region UpdateIncidentBox()
		public static void UpdateIncidentBox(int issueId, int boxId)
		{
			UpdateIncidentBox(issueId, boxId, true);
		}

		internal static void UpdateIncidentBox(int issueId, int boxId, bool checkAccess)
		{
			// Do not change anything for 0
			if (boxId == 0)
				return;

			int oldBoxId = Incident.GetIncidentBox(issueId);
			if (oldBoxId == boxId)	
				return;

			if(checkAccess && !Incident.CanUpdate(issueId))
				throw new AccessDeniedException();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				IncidentBoxDocument.RaiseModifications(issueId, oldBoxId, boxId);
				DbIssue2.UpdateIncidentBox(issueId, boxId);

				// O.R.[2008-12-16]: Recalculate Current Responsible
				DBIncident.RecalculateCurrentResponsible(issueId);

				SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Box, issueId, boxId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateClient()
		public static void UpdateClient(int issueId, PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			UpdateClient(issueId, contactUid, orgUid, true);
		}

		internal static void UpdateClient(int issueId, PrimaryKeyId contactUid, PrimaryKeyId orgUid, bool checkAccess)
		{
			if(checkAccess && !Incident.CanUpdate(issueId))
				throw new AccessDeniedException();

			DbIssue2.UpdateClient(issueId, contactUid == PrimaryKeyId.Empty ? null : (object)contactUid, orgUid == PrimaryKeyId.Empty ? null : (object)orgUid);
		}
		#endregion

		#region UpdateTimeline()
		public static void UpdateTimeline(int issueId, int taskTime)
		{
			UpdateTimeline(issueId, taskTime, true);
		}

		internal static void UpdateTimeline(int issueId, int taskTime, bool checkAccess)
		{
			if(checkAccess && !Incident.CanUpdate(issueId))
				throw new AccessDeniedException();

			int projectId = Incident.GetProject(issueId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if (DbIssue2.UpdateTimeline(issueId, taskTime) > 0)
				{
					// O.R: Recalculating project TaskTime
					if (projectId > 0)
						TimeTracking.RecalculateProjectTaskTime(projectId);
				}

				tran.Commit();
			}
		}
		#endregion

		#region UpdateExpectedValues()
		public static void UpdateExpectedValues(int issueId, int expectedResponseTime, 
			int expectedDuration, int expectedAssignTime,
			DateTime expectedResolveDate, bool useDuration)
		{
			UpdateExpectedValues(issueId, expectedResponseTime, expectedDuration, expectedAssignTime, expectedResolveDate, useDuration, true);
		}

		internal static void UpdateExpectedValues(int issueId, int expectedResponseTime, 
			int expectedDuration, int expectedAssignTime, 
			DateTime expectedResolveDate, bool useDuration,
			bool checkAccess)
		{
			if (checkAccess && !Incident.CanUpdate(issueId))
				throw new AccessDeniedException();

			// O.R. [2009-06-15]: Recalculate project dates
			bool recalculate = false;
			int projectId = -1;
			if (PortalConfig.UseIncidentDatesForProject)
			{
				bool oldUseDuration;
				DateTime oldExpectedResolveDate = DateTime.MinValue;
				using (IDataReader reader = DBIncident.GetIncidentInUTC(issueId, Security.CurrentUser.LanguageId))
				{
					reader.Read();
					oldUseDuration = (bool)reader["UseDuration"];
					if (reader["ExpectedResolveDate"] != DBNull.Value)
						oldExpectedResolveDate = (DateTime)reader["ExpectedResolveDate"];
					if (reader["Projectid"] != DBNull.Value)
						projectId = (int)reader["Projectid"];
				}

				if (projectId > 0 && (useDuration != oldUseDuration || expectedResolveDate != oldExpectedResolveDate))
					recalculate = true;
			}

			DbIssue2.UpdateExpectedValues(issueId, expectedResponseTime, expectedDuration, expectedAssignTime, expectedResolveDate, useDuration);

			if (recalculate)
			{
				Project.RecalculateDates(projectId);
			}
		}
		#endregion
		
		// Categories
		#region UpdateCategories()
		public static void UpdateCategories(ListAction action, ListType type, int issueId, ArrayList categories)
		{
			UpdateCategories(action, type, issueId, categories, true);
		}

		internal static void UpdateCategories(ListAction action, ListType type, int issueId, ArrayList categories, bool checkAccess)
		{
			if(checkAccess && !Incident.CanUpdate(issueId))
				throw new AccessDeniedException();

			ArrayList oldItems;
			SystemEventTypes eventType;
			UpdateListDelegate fnUpdate;

			switch(type)
			{
				case ListType.GeneralCategories:
					eventType = SystemEventTypes.Issue_Updated_GeneralCategories;
					oldItems = LoadGeneralCategories(issueId);
					fnUpdate = new UpdateListDelegate(Common.ListUpdate);
					break;
				case ListType.IssueCategories:
					eventType = SystemEventTypes.Issue_Updated_IssueCategories;
					oldItems = LoadIssueCategories(issueId);
					fnUpdate = new UpdateListDelegate(ListUpdate);
					break;
				default:
					throw new Exception("Unknown category type.");
			}
			Common.UpdateList(action, oldItems, categories, OBJECT_TYPE, issueId, eventType, fnUpdate, type);
		}
		#endregion

		#region AddGeneralCategories()
		public static void AddGeneralCategories(int issueId, ArrayList categories)
		{
			UpdateCategories(ListAction.Add, ListType.GeneralCategories, issueId, categories);
		}
		#endregion
		#region RemoveGeneralCategories()
		public static void RemoveGeneralCategories(int issueId, ArrayList categories)
		{
			UpdateCategories(ListAction.Remove, ListType.GeneralCategories, issueId, categories);
		}
		#endregion
		#region SetGeneralCategories()
		public static void SetGeneralCategories(int issueId, ArrayList categories)
		{
			UpdateCategories(ListAction.Set, ListType.GeneralCategories, issueId, categories);
		}
		#endregion

		#region AddIssueCategories()
		public static void AddIssueCategories(int issueId, ArrayList categories)
		{
			UpdateCategories(ListAction.Add, ListType.IssueCategories, issueId, categories);
		}
		#endregion
		#region RemoveIssueCategories()
		public static void RemoveIssueCategories(int issueId, ArrayList categories)
		{
			UpdateCategories(ListAction.Remove, ListType.IssueCategories, issueId, categories);
		}
		#endregion
		#region SetIssueCategories()
		public static void SetIssueCategories(int issueId, ArrayList categories)
		{
			UpdateCategories(ListAction.Set, ListType.IssueCategories, issueId, categories);
		}
		#endregion

		// Batch
		#region UpdateCategories()
		public static void UpdateCategories(int issueId, ArrayList generalCategories, ArrayList issueCategories)
		{
			if (!Incident.CanUpdate(issueId))
				throw new AccessDeniedException();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				UpdateCategories(ListAction.Set, ListType.GeneralCategories, issueId, generalCategories, false);
				UpdateCategories(ListAction.Set, ListType.IssueCategories, issueId, issueCategories, false);

				tran.Commit();
			}
		}
		#endregion

		#region Update
		public static void Update(
			int issueId,
			string title,
			string description,
			int projectId,
			int typeId,
			int priorityId,
			int severityId,
			int taskTime,
			int boxId,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid,
			int expectedResponseTime,
			int expectedDuration,
			int expectedAssignTime,
			ArrayList categories,
			ArrayList issueCategories)
		{
			if (!Incident.CanUpdate(issueId))
				throw new AccessDeniedException();

			IncidentBox box = IncidentBox.Load(boxId);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				UpdateGeneralInfo(issueId, title, description, typeId, severityId, false);
				UpdatePriority(issueId, priorityId, false);
				UpdateTimeline(issueId, taskTime, false);
				UpdateProject(issueId, projectId);
				UpdateCategories(ListAction.Set, ListType.GeneralCategories, issueId, categories, false);
				UpdateCategories(ListAction.Set, ListType.IssueCategories, issueId, issueCategories, false);
				UpdateIncidentBox(issueId, boxId, false);
				UpdateClient(issueId, contactUid, orgUid);
				UpdateExpectedValues(issueId, expectedResponseTime, expectedDuration, expectedAssignTime, DateTime.Now, true, false);

				tran.Commit();
			}
		}

		public static void Update(
			int issueId,
			string title,
			string description,
			int projectId,
			int typeId,
			int priorityId,
			int severityId,
			int taskTime,
			int boxId,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid,
			int expectedResponseTime,
			int expectedDuration,
			int expectedAssignTime,
			ArrayList categories,
			ArrayList issueCategories,
			DateTime expResolveDate,
			bool useDuration)
		{
			if (!Incident.CanUpdate(issueId))
				throw new AccessDeniedException();

			IncidentBox box = IncidentBox.Load(boxId);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				UpdateGeneralInfo(issueId, title, description, typeId, severityId, false);
				UpdatePriority(issueId, priorityId, false);
				UpdateTimeline(issueId, taskTime, false);
				UpdateProject(issueId, projectId);
				UpdateCategories(ListAction.Set, ListType.GeneralCategories, issueId, categories, false);
				UpdateCategories(ListAction.Set, ListType.IssueCategories, issueId, issueCategories, false);
				UpdateIncidentBox(issueId, boxId, false);
				UpdateClient(issueId, contactUid, orgUid);
				expResolveDate = User.GetUTCDate(expResolveDate);
				UpdateExpectedValues(issueId, expectedResponseTime, expectedDuration, expectedAssignTime, expResolveDate, useDuration, false);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateTrackingState
		
		//TODO: Rg system events
		//TODO: Manage forum
		//TODO: Manage Alerts

		public static void UpdateTrackingState(
			int IssueId
			, int ForumNodeId
			, int StateId
			, int PriorityId
			, int ResponsibleId
			, bool IsResponsibleGroup
			, DataTable ResponsibleGroup
			, bool Decline
			)
		{
			if(ResponsibleId > 0 && IsResponsibleGroup)
				throw new ArgumentException("ResponsibleId > 0 && IsResponsibleGroup");

			UserLight cu = Security.CurrentUser;
			
			//get current Tracking state
			int oldStateId = -1;
			int oldResponsibleId  = -1;
			bool oldIsResponsibleGroup  = false;
			int oldPriorityId = -1;

			using(IDataReader reader = DBIncident.GetIncidentTrackingState(IssueId, cu.TimeZoneId, cu.LanguageId))
			{
				if(reader.Read())
				{
					oldStateId = (int)reader["StateId"];
					oldResponsibleId = (int)reader["ResponsibleId"];
					oldIsResponsibleGroup = (bool)reader["IsResponsibleGroup"];
					oldPriorityId = (int)reader["PriorityId"];
				}
			}

			if(oldStateId != StateId)
			{
				if(!Incident.IsTransitionAllowed(IssueId, (ObjectStates)oldStateId, (ObjectStates)StateId))
					throw new AccessDeniedException();
			}

			//Getting IncidentBox Document
			int incidentBoxId = -1;
			int CreatorId = -1;
			int controllerId = -1;
			using(IDataReader reader = DBIncident.GetIncident(IssueId, cu.TimeZoneId, cu.LanguageId))
			{
				if(reader.Read())
				{
					incidentBoxId = (int)reader["IncidentBoxId"];
					CreatorId = (int)reader["CreatorId"];
					controllerId = DBCommon.NullToInt32(reader["ControllerId"]);
				}
			}
			IncidentBoxDocument IncDoc = IncidentBoxDocument.Load(incidentBoxId);

			// OZ: Fix Second Approve
			bool canSetUser = (IncDoc.GeneralBlock.AllowToReassignResponsibility || 
				Incident.CanDoAction(IssueId, "AssignResponsible", cu.UserID));
			if(!IsResponsibleGroup && !oldIsResponsibleGroup && 
				oldResponsibleId!=ResponsibleId && 
				!canSetUser)
			{
				throw new AccessDeniedException();
			}
			//


			Incident.Tracking tracking = Incident.GetTrackingInfo(IssueId);

			//set responsible
			int newResponsibleId = oldResponsibleId;
			bool newIsResponsibleGroup = oldIsResponsibleGroup;
			int forumRespId = oldResponsibleId;

			if(StateId == (int)ObjectStates.Active || StateId == (int)ObjectStates.ReOpen || StateId == (int)ObjectStates.Upcoming)
			{
				if(ResponsibleId != oldResponsibleId || IsResponsibleGroup != oldIsResponsibleGroup)
				{
					if(ResponsibleId > 0 )
					{
						//check security
						if(!tracking.CanSetUser)
						{
							if(!((tracking.ShowAccept || tracking.ShowAcceptDecline) && ResponsibleId == cu.UserID))
								throw new AccessDeniedException();
						}

						if(ResponsibleId != oldResponsibleId)
						{
							newResponsibleId = ResponsibleId;
							newIsResponsibleGroup = false;
							forumRespId = ResponsibleId;

							//Auto activate
							if(StateId == (int)ObjectStates.Upcoming)
								StateId = (int)ObjectStates.Active;
						}
					}
					else if(ResponsibleId < 0 && !IsResponsibleGroup)
					{
						//check security
						if(!tracking.CanSetNoUser)
							throw new AccessDeniedException();

						newResponsibleId = -1;
						newIsResponsibleGroup = false;
						forumRespId = -2;
					}
					else if(IsResponsibleGroup && !oldIsResponsibleGroup)
					{
						//check security
						if(!tracking.CanSetGroup)
							throw new AccessDeniedException();

						newResponsibleId = -1;
						newIsResponsibleGroup = true;
						forumRespId = -1;
					}
				}

				if(Decline)
				{
					//check security
					if(!(tracking.ShowDecline || tracking.ShowAcceptDecline))
						throw new AccessDeniedException();

					if(newResponsibleId != oldResponsibleId || newIsResponsibleGroup != oldIsResponsibleGroup)
					{
						Decline = false;
					}
					else if(cu.UserID == oldResponsibleId)
					{
					
						if(IncDoc.GeneralBlock.ResponsibleAssignType == ResponsibleAssignType.Pool)
						{
							newResponsibleId = -1;
							newIsResponsibleGroup = true;
						}
						else if(IncDoc.GeneralBlock.ResponsibleAssignType == ResponsibleAssignType.CustomUser)
						{
							newIsResponsibleGroup = false;

							if (IncDoc.GeneralBlock.Responsible != cu.UserID)
							{
								// O.R. [2008-08-06] Fix: take new responsible from box
								newResponsibleId = IncDoc.GeneralBlock.Responsible;
								// newResponsibleId = cu.UserID;
							}
							else
							{
								newResponsibleId = -1;
							}
						}
						else
						{
							newResponsibleId = -1;
							newIsResponsibleGroup = false;
						}
					}
				}
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				//Update Priority
				if (PriorityId >= 0 && oldPriorityId != PriorityId)
					Issue2.UpdatePriority(IssueId, PriorityId);

				// Update state
				if (StateId != oldStateId)
					UpdateStateAndNotifyController(IssueId, StateId, ForumNodeId);

				// Update responsible
				if(StateId == (int)ObjectStates.Active || StateId == (int)ObjectStates.ReOpen || StateId == (int)ObjectStates.Upcoming)
				{
					UpdateResponsibleGroup(IssueId, ResponsibleGroup); 

					if(Decline)
					{
						DbIssue2.ResponsibleReply(IssueId, cu.UserID, false);
							
						SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Responsible_Rejected, IssueId, cu.UserID);
						ForumNodeId = IncidentForum.AddForumMesageWithDeclining(IssueId, ForumNodeId);
					}

					if(newResponsibleId != oldResponsibleId || newIsResponsibleGroup != oldIsResponsibleGroup)
					{
						// O.R. [2009-02-12]
						DBCalendar.DeleteStickedObjectForAllUsers((int)OBJECT_TYPE, IssueId);

						if(DbIssue2.UpdateResponsibility(IssueId, newResponsibleId, newIsResponsibleGroup) > 0)
						{
							SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Responsible_Changed, IssueId, newResponsibleId);

							if(oldResponsibleId > 0)
								SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Responsible_Resigned, IssueId, oldResponsibleId);
							if(newResponsibleId > 0)
								SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Responsible_Assigned, IssueId, newResponsibleId);
							
							IncidentForum.AddForumMesageWithResponsibleChange(IssueId, ForumNodeId, newResponsibleId);
						}
					}
					
				}

				// O.R.[2008-12-16]: Recalculate Current Responsible
				DBIncident.RecalculateCurrentResponsible(IssueId);

				tran.Commit();
			}
		}

		#endregion
		
		#region UpdateQuickTracking
		public static void UpdateQuickTracking(int issueId, string message, int stateId, int priorityId, int responsibleId, bool isResponsibleGroup, DataTable responsibleGroup, bool decline)
		{
			UpdateQuickTracking(issueId, message, stateId, priorityId, null, null, responsibleId, isResponsibleGroup, responsibleGroup, decline, false);
		}

		/// <summary>
		/// Updates the quick tracking.
		/// </summary>
		/// <param name="issueId">The issue id.</param>
		/// <param name="message">The message.</param>
		/// <param name="stateId">The state id.</param>
		/// <param name="priorityId">The priority id.</param>
		/// <param name="issueBoxId">The issue box id.</param>
		/// <param name="projectId">The project id.</param>
		/// <param name="responsibleId">The responsible id.</param>
		/// <param name="isResponsibleGroup">if set to <c>true</c> [is responsible group].</param>
		/// <param name="responsibleGroup">The responsible group.</param>
		/// <param name="decline">if set to <c>true</c> [decline].</param>
		/// <param name="useNewResponsible">if set to <c>true</c> [use new responsible from Issue Box].</param>
		public static void UpdateQuickTracking(int issueId, string message, int stateId, int priorityId, int? issueBoxId, int? projectId, int responsibleId, bool isResponsibleGroup, DataTable responsibleGroup, bool decline, bool useNewResponsible)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				int nodeId = -1;
				if (message != null && message.Trim().Length > 0)
				{
					BaseIbnContainer destContainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", issueId));
					ForumStorage forumStorage = (ForumStorage)destContainer.LoadControl("ForumStorage");
					ForumThreadNodeInfo info = forumStorage.CreateForumThreadNode(message, Security.UserID, 1);

					ForumThreadNodeSettingCollection settings1 = new ForumThreadNodeSettingCollection(info.Id);
					settings1.Add(ForumThreadNodeSetting.Internal, "1");
					nodeId = info.Id;

					SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Forum_MessageAdded, issueId);
				}

				if (issueBoxId.HasValue && projectId.HasValue && Incident.CanChangeProject(issueId))
				{
					UpdateProject(issueId, projectId.Value, false);
					UpdateIncidentBox(issueId, issueBoxId.Value, false);

					if (useNewResponsible)
					{
						decline = false;
						responsibleGroup.Rows.Clear();

						IncidentBox box = IncidentBox.Load(issueBoxId.Value);

						ResponsibleAssignType responsibleAssignType = box.Document.GeneralBlock.ResponsibleAssignType;
						if (responsibleAssignType == ResponsibleAssignType.Manual)
						{
							responsibleId = -1;
							isResponsibleGroup = false;
						}
						else if (responsibleAssignType == ResponsibleAssignType.CustomUser)
						{
							responsibleId = box.Document.GeneralBlock.Responsible;
							isResponsibleGroup = false;
						}
						else // Pool
						{
							responsibleId = -1;
							isResponsibleGroup = true;

							List<int> responsibleUsers = new List<int>();
							foreach (int principalId in box.Document.GeneralBlock.ResponsiblePool)
							{
								if (User.IsGroup(principalId))
								{
									using (IDataReader reader = DBGroup.GetListUsersInGroup(principalId))
									{
										while (reader.Read())
										{
											if (!responsibleUsers.Contains((int)reader["UserId"]) && (int)reader["Activity"] == 3)
												responsibleUsers.Add((int)reader["UserId"]);
										}
									}
								}
								else
								{
									if (!responsibleUsers.Contains(principalId) && User.GetUserActivity(principalId) == User.UserActivity.Active)
										responsibleUsers.Add(principalId);
								}
							}

							// responsibleGroup
							foreach (int principalId in responsibleUsers)
							{
								DataRow row = responsibleGroup.NewRow();
								row["PrincipalId"] = principalId;
								row["IsNew"] = true;
								row["ResponsePending"] = true;

								responsibleGroup.Rows.Add(row);
							}
						}
						
					}
				}

				UpdateTrackingState(issueId, nodeId, stateId, priorityId, responsibleId, isResponsibleGroup, responsibleGroup, decline);

				tran.Commit();
			}
		}
		#endregion


		#region UpdateQuickTracking
		public static void UpdateQuickTracking(int IssueId, string Message, int StateId)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				int NodeId = -1;
				if (Message != null && Message.Trim().Length > 0)
				{
					BaseIbnContainer destContainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", IssueId));
					ForumStorage forumStorage = (ForumStorage)destContainer.LoadControl("ForumStorage");
					ForumThreadNodeInfo info = forumStorage.CreateForumThreadNode(Message, Security.UserID, 1);

					ForumThreadNodeSettingCollection settings1 = new ForumThreadNodeSettingCollection(info.Id);
					settings1.Add(ForumThreadNodeSetting.Internal, "1");
					NodeId = info.Id;

					SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Forum_MessageAdded, IssueId);
				}
				UpdateTrackingState(IssueId, NodeId, StateId);

				tran.Commit();
			}
		}

		#endregion

		#region UpdateTrackingState
		public static void UpdateTrackingState(
			int IssueId
			, int ForumNodeId
			, int StateId
			)
		{
			UserLight cu = Security.CurrentUser;

			//get current Tracking state
			int oldStateId = -1;
			
			using (IDataReader reader = DBIncident.GetIncidentTrackingState(IssueId, cu.TimeZoneId, cu.LanguageId))
			{
				if (reader.Read())
					oldStateId = (int)reader["StateId"];
			}

			if (oldStateId != StateId)
			{
				if (!Incident.IsTransitionAllowed(IssueId, (ObjectStates)oldStateId, (ObjectStates)StateId))
				{
					if ((ObjectStates)StateId == ObjectStates.Active)
					{
						StateId = (int)ObjectStates.ReOpen;
						if (!Incident.IsTransitionAllowed(IssueId, (ObjectStates)oldStateId, (ObjectStates)StateId))
							throw new AccessDeniedException();
					}
					else if ((ObjectStates)StateId == ObjectStates.ReOpen)
					{
						StateId = (int)ObjectStates.Active;
						if (!Incident.IsTransitionAllowed(IssueId, (ObjectStates)oldStateId, (ObjectStates)StateId))
							throw new AccessDeniedException();
					}
					else if ((ObjectStates)StateId == ObjectStates.OnCheck)
					{
						StateId = (int)ObjectStates.Completed;
						if (!Incident.IsTransitionAllowed(IssueId, (ObjectStates)oldStateId, (ObjectStates)StateId))
							throw new AccessDeniedException();
					}
					else if ((ObjectStates)StateId == ObjectStates.Completed)
					{
						StateId = (int)ObjectStates.OnCheck;
						if (!Incident.IsTransitionAllowed(IssueId, (ObjectStates)oldStateId, (ObjectStates)StateId))
							throw new AccessDeniedException();
					}
					else
						throw new AccessDeniedException();
				}
			}

			using (DbTransaction tran = DbTransaction.Begin())
			{
				// Update state
				if (StateId != oldStateId)
					UpdateStateAndNotifyController(IssueId, StateId, ForumNodeId);

				tran.Commit();
			}
		}

		#endregion


		#region UpdateQuickTracking
		public static void UpdateQuickTracking(int IssueId, string Message, int ResponsibleId, bool IsResponsibleGroup, DataTable ResponsibleGroup)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				int NodeId = -1;
				if (Message != null && Message.Trim().Length > 0)
				{
					BaseIbnContainer destContainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", IssueId));
					ForumStorage forumStorage = (ForumStorage)destContainer.LoadControl("ForumStorage");
					ForumThreadNodeInfo info = forumStorage.CreateForumThreadNode(Message, Security.UserID, 1);

					ForumThreadNodeSettingCollection settings1 = new ForumThreadNodeSettingCollection(info.Id);
					settings1.Add(ForumThreadNodeSetting.Internal, "1");
					NodeId = info.Id;

					SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Forum_MessageAdded, IssueId);
				}
				UpdateTrackingState(IssueId, NodeId, ResponsibleId, IsResponsibleGroup, ResponsibleGroup);

				tran.Commit();
			}
		}

		#endregion

		#region UpdateTrackingState

		//TODO: Rg system events
		//TODO: Manage forum
		//TODO: Manage Alerts

		public static void UpdateTrackingState(
			int IssueId
			, int ForumNodeId
			, int ResponsibleId
			, bool IsResponsibleGroup
			, DataTable ResponsibleGroup
			)
		{
			if (ResponsibleId > 0 && IsResponsibleGroup)
				throw new ArgumentException("ResponsibleId > 0 && IsResponsibleGroup");

			UserLight cu = Security.CurrentUser;

			//get current Tracking state
			int oldStateId = -1;
			int oldResponsibleId = -1;
			bool oldIsResponsibleGroup = false;

			using (IDataReader reader = DBIncident.GetIncidentTrackingState(IssueId, cu.TimeZoneId, cu.LanguageId))
			{
				if (reader.Read())
				{
					oldStateId = (int)reader["StateId"];
					oldResponsibleId = (int)reader["ResponsibleId"];
					oldIsResponsibleGroup = (bool)reader["IsResponsibleGroup"];
				}
			}

			int newStateId = oldStateId;

			//Getting IncidentBox Document
			int incidentBoxId = -1;
			int CreatorId = -1;
			int controllerId = -1;
			using (IDataReader reader = DBIncident.GetIncident(IssueId, cu.TimeZoneId, cu.LanguageId))
			{
				if (reader.Read())
				{
					incidentBoxId = (int)reader["IncidentBoxId"];
					CreatorId = (int)reader["CreatorId"];
					controllerId = DBCommon.NullToInt32(reader["ControllerId"]);
				}
			}
			IncidentBoxDocument IncDoc = IncidentBoxDocument.Load(incidentBoxId);

			// OZ: Fix Second Approve
			bool canSetUser = (IncDoc.GeneralBlock.AllowToReassignResponsibility ||
				Incident.CanDoAction(IssueId, "AssignResponsible", cu.UserID));
			if (!IsResponsibleGroup && !oldIsResponsibleGroup &&
				oldResponsibleId != ResponsibleId &&
				!canSetUser)
			{
				throw new AccessDeniedException();
			}
			//

			Incident.Tracking tracking = Incident.GetTrackingInfo(IssueId);

			//set responsible
			int newResponsibleId = oldResponsibleId;
			bool newIsResponsibleGroup = oldIsResponsibleGroup;
			int forumRespId = oldResponsibleId;

			if (oldStateId == (int)ObjectStates.Active || oldStateId == (int)ObjectStates.ReOpen || oldStateId == (int)ObjectStates.Upcoming)
			{
				if (ResponsibleId != oldResponsibleId || IsResponsibleGroup != oldIsResponsibleGroup)
				{
					if (ResponsibleId > 0)
					{
						//check security
						if (!tracking.CanSetUser)
						{
							if (!((tracking.ShowAccept || tracking.ShowAcceptDecline) && ResponsibleId == cu.UserID))
								throw new AccessDeniedException();
						}

						if (ResponsibleId != oldResponsibleId)
						{
							newResponsibleId = ResponsibleId;
							newIsResponsibleGroup = false;
							forumRespId = ResponsibleId;

							//Auto activate
							if (oldStateId == (int)ObjectStates.Upcoming)
								newStateId = (int)ObjectStates.Active;
						}
					}
					else if (ResponsibleId < 0 && !IsResponsibleGroup)
					{
						//check security
						if (!tracking.CanSetNoUser)
							throw new AccessDeniedException();

						newResponsibleId = -1;
						newIsResponsibleGroup = false;
						forumRespId = -2;
					}
					else if (IsResponsibleGroup && !oldIsResponsibleGroup)
					{
						//check security
						if (!tracking.CanSetGroup)
							throw new AccessDeniedException();

						newResponsibleId = -1;
						newIsResponsibleGroup = true;
						forumRespId = -1;
					}
				}
			}

			using (DbTransaction tran = DbTransaction.Begin())
			{
				// Update state
				if (newStateId != oldStateId)
					UpdateStateAndNotifyController(IssueId, newStateId, ForumNodeId);

				// Update responsible
				if (newStateId == (int)ObjectStates.Active || newStateId == (int)ObjectStates.ReOpen || newStateId == (int)ObjectStates.Upcoming)
				{
					UpdateResponsibleGroup(IssueId, ResponsibleGroup);

					if (newResponsibleId != oldResponsibleId || newIsResponsibleGroup != oldIsResponsibleGroup)
					{
						// O.R. [2009-02-12]
						DBCalendar.DeleteStickedObjectForAllUsers((int)OBJECT_TYPE, IssueId);

						if (DbIssue2.UpdateResponsibility(IssueId, newResponsibleId, newIsResponsibleGroup) > 0)
						{
							SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Responsible_Changed, IssueId, newResponsibleId);

							if (oldResponsibleId > 0)
								SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Responsible_Resigned, IssueId, oldResponsibleId);
							if (newResponsibleId > 0)
								SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Responsible_Assigned, IssueId, newResponsibleId);

							IncidentForum.AddForumMesageWithResponsibleChange(IssueId, ForumNodeId, newResponsibleId);
						}
					}

					// O.R.[2008-12-16]: Recalculate Current Responsible
					DBIncident.RecalculateCurrentResponsible(IssueId);
				}

				tran.Commit();
			}
		}

		#endregion

		//in work
			

		#region //AddForumMessage()
		/*
		public static void AddForumMessage(int issueId, string messText, 
			string FileName1, System.IO.Stream inputStream1,
			string FileName2, System.IO.Stream inputStream2,
			string FileName3, System.IO.Stream inputStream3)
		{

			ForumStorage.NodeContentType contentType = ((FileName1 != String.Empty && inputStream1 != null)||
				(FileName2 != String.Empty && inputStream2 != null)||
				(FileName3 != String.Empty && inputStream3 != null)) ? 
				ForumStorage.NodeContentType.TextWithFiles:
				ForumStorage.NodeContentType.Text;
			AddForumMessage2(issueId, messText, (int)contentType, "0",  -1, FileName1, inputStream1, FileName2, inputStream2, FileName3, inputStream3);
		}*/
		#endregion
		#region AddForumMessage2()
		public static void AddForumMessage2(int issueId, string messText, 
			int nodeType, ArrayList nodeAttribute, bool toSend, int stateId,
			string FileName1, System.IO.Stream inputStream1,
			string FileName2, System.IO.Stream inputStream2,
			string FileName3, System.IO.Stream inputStream3)
		{
			int oldState;
			using(IDataReader reader = Incident.GetIncident(issueId, false))
			{
				reader.Read();
				oldState = (int)reader["StateId"];
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				ForumThreadNodeInfo info = null;
				if (messText.Trim() != String.Empty || FileName1 != String.Empty || FileName2 != String.Empty || FileName3 != String.Empty)
				{
					BaseIbnContainer destContainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", issueId));
					ForumStorage forumStorage = (ForumStorage)destContainer.LoadControl("ForumStorage");

					info = forumStorage.CreateForumThreadNode(messText, Security.CurrentUser.UserID, nodeType);

					if (FileName1 != String.Empty && inputStream1 != null)
					{
						BaseIbnContainer forumContainer = BaseIbnContainer.Create("FileLibrary", string.Format("ForumNodeId_{0}", info.Id));
						FileStorage fs = (FileStorage)forumContainer.LoadControl("FileStorage");
						fs.SaveFile(FileName1, inputStream1);
					}

					if (FileName2 != String.Empty && inputStream2 != null)
					{
						BaseIbnContainer forumContainer = BaseIbnContainer.Create("FileLibrary", string.Format("ForumNodeId_{0}", info.Id));
						FileStorage fs = (FileStorage)forumContainer.LoadControl("FileStorage");
						fs.SaveFile(FileName2, inputStream2);
					}

					if (FileName3 != String.Empty && inputStream3 != null)
					{
						BaseIbnContainer forumContainer = BaseIbnContainer.Create("FileLibrary", string.Format("ForumNodeId_{0}", info.Id));
						FileStorage fs = (FileStorage)forumContainer.LoadControl("FileStorage");
						fs.SaveFile(FileName3, inputStream3);
					}

					ForumThreadNodeSettingCollection settings = new ForumThreadNodeSettingCollection(info.Id);
                    //if(toSend)
                    //{
                    //    settings.Add(ForumThreadNodeSetting.Outgoing, "1");
                    //    //EMailRouterOutputMessage.SendOutgoing(issueId, info.Id);
                    //}
                    //else
					settings.Add(ForumThreadNodeSetting.Internal, "1");

					foreach(string sAttr in nodeAttribute)
						settings.Add(sAttr, "1");

					SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Forum_MessageAdded, issueId);
				}

				if(stateId > 0 && stateId != oldState)
					UpdateStateAndNotifyController(issueId, stateId, info.Id);

				tran.Commit();
			}
		}
		#endregion
		#region //AddForumMessageFromClient()
		//		public static void AddForumMessageFromClient(int issueId, string messText, 
		//			string externalId,
		//			string FileName1, System.IO.Stream inputStream1,
		//			string FileName2, System.IO.Stream inputStream2,
		//			string FileName3, System.IO.Stream inputStream3)
		//		{
		//			using(DbTransaction tran = DbTransaction.Begin())
		//			{
		//				ForumStorage.NodeContentType contentType = ((FileName1 != String.Empty && inputStream1 != null)||
		//					(FileName2 != String.Empty && inputStream2 != null)||
		//					(FileName3 != String.Empty && inputStream3 != null)) ? 
		//					ForumStorage.NodeContentType.TextWithFiles:
		//					ForumStorage.NodeContentType.Text;
		//				AddForumMessageFromClient2(issueId, messText, externalId, (int)contentType, 
		//					ForumThreadNodeSetting.Incoming, -1, 
		//					FileName1, inputStream1, FileName2, inputStream2, FileName3, inputStream3);
		//
		//				tran.Commit();
		//			}
		//		}
		//		#endregion
		//		#region AddForumMessageFromClient2()
		//		public static int AddForumMessageFromClient2(int issueId, string messText, 
		//			string externalId, int nodeType, string nodeAttribute, int stateId,
		//			string FileName1, System.IO.Stream inputStream1,
		//			string FileName2, System.IO.Stream inputStream2,
		//			string FileName3, System.IO.Stream inputStream3)
		//		{
		//			int retVal = -1;
		//			string email = String.Empty;
		//			using(IDataReader reader = User.GetGateByGuid(externalId))
		//			{
		//				if(reader.Read())
		//					email = reader["Email"].ToString();
		//			}
		//			if(email.Length == 0)
		//				throw new AccessDeniedException();
		//
		//			int oldState;
		//			using(IDataReader reader = Incident.GetIncident(issueId))
		//			{
		//				reader.Read();
		//				oldState = (int)reader["StateId"];
		//			}
		//
		//			using(DbTransaction tran = DbTransaction.Begin())
		//			{
		//				BaseIbnContainer destContainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", issueId));
		//				ForumStorage forumStorage = (ForumStorage)destContainer.LoadControl("ForumStorage");
		//				ForumThreadNodeInfo info = forumStorage.CreateForumThreadNode(messText, email, email, nodeType);
		//				retVal = info.Id;
		//				if (FileName1 != String.Empty && inputStream1 != null)
		//				{
		//					BaseIbnContainer forumContainer = BaseIbnContainer.Create("FileLibrary", string.Format("ForumNodeId_{0}", info.Id));
		//					FileStorage fs = (FileStorage)forumContainer.LoadControl("FileStorage");
		//					fs.SaveFile(FileName1, -1, DateTime.UtcNow, inputStream1);
		//				}
		//
		//				if (FileName2 != String.Empty && inputStream2 != null)
		//				{
		//					BaseIbnContainer forumContainer = BaseIbnContainer.Create("FileLibrary", string.Format("ForumNodeId_{0}", info.Id));
		//					FileStorage fs = (FileStorage)forumContainer.LoadControl("FileStorage");
		//					fs.SaveFile(FileName2, -1, DateTime.UtcNow, inputStream2);
		//				}
		//
		//				if (FileName3 != String.Empty && inputStream3 != null)
		//				{
		//					BaseIbnContainer forumContainer = BaseIbnContainer.Create("FileLibrary", string.Format("ForumNodeId_{0}", info.Id));
		//					FileStorage fs = (FileStorage)forumContainer.LoadControl("FileStorage");
		//					fs.SaveFile(FileName2, -1, DateTime.UtcNow, inputStream2);
		//				}
		//
		//				if(nodeAttribute.Length>0 && nodeAttribute!="0")
		//				{
		//					ForumThreadNodeSettingCollection settings = new ForumThreadNodeSettingCollection(info.Id);
		//					settings.Add(nodeAttribute, "1");
		//				}
		//
		//				SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Forum_MessageAdded, issueId);
		//
		//				if (stateId > 0 && stateId != oldState)
		//					UpdateState(issueId, stateId);
		//
		//				tran.Commit();
		//			}
		//			return retVal;
		//		}
		#endregion

		#region SetStateByEmail
		public static void SetStateByEmail(ForumStorage.NodeType nodeType, IncidentBoxDocument incidentBoxDocument, int IncidentId, int ThreadNodeId, ObjectStates state)
		{
			//if(ForumStorage.NodeType.Outgoing == nodeType || ForumStorage.NodeType.Internal == nodeType)
			{
				int stateId = (int)state;
				int oldStateId = -1;
				int responsibleId = -1;
				bool oldIsGroupResponsobility = false;
				using(IDataReader reader = Incident.GetIncidentTrackingState(IncidentId, false))
				{
					if(reader.Read())
					{
						oldStateId = (int)reader["StateId"];
						responsibleId = (int)reader["ResponsibleId"];
						oldIsGroupResponsobility = (bool)reader["IsResponsibleGroup"];
					}
				}

				if(stateId==(int)ObjectStates.ReOpen && oldStateId==(int)ObjectStates.Active)
					return;

				using(DbTransaction tran = DbTransaction.Begin())
				{
					if(stateId != oldStateId)
						UpdateStateAndNotifyController(IncidentId, stateId, ThreadNodeId);

					if(stateId == (int)ObjectStates.Active || stateId == (int)ObjectStates.ReOpen || stateId == (int)ObjectStates.Upcoming)
					{
						//set responsible if it's incoming and resp not set
						if(responsibleId <= 0)
						{
							DbIssue2.UpdateResponsibility(IncidentId, Security.UserID, false);

							// O.R.[2008-12-16]: Recalculate Current Responsible
							DBIncident.RecalculateCurrentResponsible(IncidentId);

							SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Responsible_Assigned, IncidentId, Security.UserID);
							IncidentForum.AddForumMesageWithResponsibleChange(IncidentId, ThreadNodeId, Security.UserID);
						}
					}
					tran.Commit();
				}
			}
		}
		#endregion

		#region GetIssueBoxSettings()
		internal static void GetIssueBoxSettings(int issueBoxId, out int stateId, out int managerId, out int responsibleId, out bool isResposibleGroup, ArrayList responsibleUsers)
		{
			responsibleId = -1;
			isResposibleGroup = false;
			stateId = (int)ObjectStates.Upcoming;

			IncidentBoxDocument document = IncidentBoxDocument.Load(issueBoxId);

			GeneralIncidentBoxBlock generalBlock = document.GeneralBlock;
			managerId = generalBlock.Manager;
			
			switch(generalBlock.ResponsibleAssignType)
			{
				case Business.EMail.ResponsibleAssignType.CustomUser:
					responsibleId = generalBlock.Responsible;
					if(responsibleId > 0)
						stateId = (int)ObjectStates.Active;
					break;

				case Business.EMail.ResponsibleAssignType.Manual:
					break;
				case Business.EMail.ResponsibleAssignType.Pool:
					isResposibleGroup = true;
					stateId = (int)ObjectStates.Active;
					foreach(int principalId in generalBlock.ResponsiblePool)
					{
						if(User.IsGroup(principalId))
						{
							using(IDataReader reader = DBGroup.GetListUsersInGroup(principalId))
							{
								while(reader.Read())
									responsibleUsers.Add((int)reader["UserId"]);
							}
						}
						else
						{
							responsibleUsers.Add(principalId);
						}
					}
					break;

			}
		}
		#endregion

		#region SendAlertsForNewIssue()
		internal static void SendAlertsForNewIssue(int issueId, int managerId, int responsibleId, ArrayList responsibleUsers, ArrayList excludeUsers)
		{
			if(responsibleId > 0)
				SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Responsible_Assigned, issueId, responsibleId);
			else if(responsibleUsers != null && responsibleUsers.Count > 0)
			{
				foreach(int userId in responsibleUsers)
				{
					if(excludeUsers == null || !excludeUsers.Contains(userId))
						SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Responsible_Requested, issueId, userId);
				}
			}
			SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Manager_ManagerAdded, issueId, managerId);
		}
		#endregion

		#region SetNewMessage
		public static void SetNewMessage(int IncidentId, bool NewMessage)
		{
			DbIssue2.NewMessageSet(IncidentId, NewMessage);
		}
		#endregion

		#region MarkAsSpam
		/// <summary>
		/// Marks as spam.
		/// </summary>
		/// <param name="issueId">The issue id.</param>
		/// <param name="deleteIssue">if set to <c>true</c> [delete issue].</param>
		public static void MarkAsSpam(int issueId, bool deleteIssue)
		{
			string[] emailItems = EMailIssueExternalRecipient.StringList(issueId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				foreach (string senderEmail in emailItems)
				{
					// Remove From White List
					foreach (WhiteListItem whiteItem in WhiteListItem.List(senderEmail))
					{
						WhiteListItem.Delete(whiteItem.Id);
					}

					// Add To Black List
					if (!string.IsNullOrEmpty(senderEmail) && !BlackListItem.Contains(senderEmail))
						BlackListItem.Create(senderEmail);
				}

				// Check deleteIssue
				if (deleteIssue)
				{
					// Delete
					Incident.Delete(issueId);
				}
				else
				{
					// Set Suspend State
					Issue2.UpdateState(issueId, (int)ObjectStates.Completed, false);
				}

				tran.Commit();
			}
		}
		#endregion
	}
}
