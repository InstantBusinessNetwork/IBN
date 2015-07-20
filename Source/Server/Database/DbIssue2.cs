using System;
using System.Data;

namespace Mediachase.IBN.Database
{
	public class DbIssue2
	{
		#region UpdateGeneralInfo()
		public static void UpdateGeneralInfo(int issueId,
			string title,
			string description,
			int typeId,
			int severityId)
		{
			DbHelper2.RunSp("Act_IssueUpdateGeneralInfo",
				DbHelper2.mp("@IssueId", SqlDbType.Int, issueId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, title),
				DbHelper2.mp("@Description", SqlDbType.NText, description),
				DbHelper2.mp("@TypeId", SqlDbType.Int, typeId),
				DbHelper2.mp("@SeverityId", SqlDbType.Int, severityId));
		}
		#endregion

		#region UpdatePriority()
		public static int UpdatePriority(int issueId, int valueId)
		{
			return DbHelper2.RunSpInteger("Act_IssueUpdatePriority",
				DbHelper2.mp("@IssueId", SqlDbType.Int, issueId),
				DbHelper2.mp("@ValueId", SqlDbType.Int, valueId));
		}
		#endregion

		#region UpdateResolutionInfo()
		public static void UpdateResolutionInfo(int issueId, string resolution, string workaround)
		{
			DbHelper2.RunSp("Act_IssueUpdateResolutionInfo",
				DbHelper2.mp("@IssueId", SqlDbType.Int, issueId),
				DbHelper2.mp("@Resolution", SqlDbType.NText, resolution),
				DbHelper2.mp("@Workaround", SqlDbType.NText, workaround));
		}
		#endregion

		#region UpdateStat()
		public static int UpdateState(int issueId, int valueId)
		{
			return DbHelper2.RunSpInteger("Act_IssueUpdateState",
				DbHelper2.mp("@IssueId", SqlDbType.Int, issueId),
				DbHelper2.mp("@ValueId", SqlDbType.Int, valueId));
		}
		#endregion

		#region ResponsibleGroupAddUser
		public static void ResponsibleGroupAddUser(int objectId, int principalId)
		{
			DbHelper2.RunSp("Act_IssueResponsibleGroupAddUser",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, principalId));
		}
		#endregion

		
		#region ResponsibleGroupUserUpdate
		public static int ResponsibleGroupUserUpdate(int objectId, int principalId, bool ResponsePending)
		{
			return DbHelper2.RunSpInteger("Act_IssueResponsibleGroupUserUpdate",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, principalId),
				DbHelper2.mp("@ResponsePending", SqlDbType.Bit, ResponsePending));
		}
		#endregion

		#region ResourceAdd
		public static void ResourceAdd(int objectId, int principalId, bool mustBeConfirmed, bool canManage)
		{
			DbHelper2.RunSp("Act_IssueResourceAdd",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, principalId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, mustBeConfirmed),
				DbHelper2.mp("@CanManage", SqlDbType.Bit, canManage));
		}
		#endregion

		#region ResourceUpdate
		public static int ResourceUpdate(int objectId, int principalId, bool mustBeConfirmed, bool canManage)
		{
			return DbHelper2.RunSpInteger("Act_IssueResourceUpdate",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, principalId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, mustBeConfirmed),
				DbHelper2.mp("@CanManage", SqlDbType.Bit, canManage));
		}
		#endregion

		#region ResourceReply
		public static int ResourceReply(int objectId, int userId, bool isConfirmed)
		{
			return DbHelper2.RunSpInteger("Act_IssueResourceReply",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@IsConfirmed", SqlDbType.Bit, isConfirmed));
		}
		#endregion

		#region ResponsibleReply
		public static int ResponsibleReply(int objectId, int userId, bool isConfirmed)
		{
			return DbHelper2.RunSpInteger("Act_IssueResponsibleReply",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@IsConfirmed", SqlDbType.Bit, isConfirmed));
		}
		#endregion

		#region UpdateMailSenderEmail
		/// <summary>
		/// Updates the mail sender email.
		/// </summary>
		/// <param name="issueId">The issue id.</param>
		/// <param name="email">The mail sender email.</param>
		public static void UpdateMailSenderEmail(int issueId, string email)
		{
			DbHelper2.RunSp("Act_IssueUpdateMailSenderEmail", 
				DbHelper2.mp("@IssueId", SqlDbType.Int, issueId),
				DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, email));
		}
		#endregion

		#region UpdateTimeline()
		public static int UpdateTimeline(int issueId, int taskTime)
		{
			return DbHelper2.RunSpInteger("Act_IssueUpdateTimeline",
				DbHelper2.mp("@IssueId", SqlDbType.Int, issueId),
				DbHelper2.mp("@TaskTime", SqlDbType.Int, taskTime));
		}
		#endregion

		#region UpdateIncidentBox()
		public static int UpdateIncidentBox(int issueId, int valueId)
		{
			return DbHelper2.RunSpInteger("Act_IssueUpdateIncidentBox",
				DbHelper2.mp("@IssueId", SqlDbType.Int, issueId),
				DbHelper2.mp("@ValueId", SqlDbType.Int, valueId));
		}
		#endregion

		#region UpdateClient()
		public static int UpdateClient(int issueId, object contactUid, object orgUid)
		{
			return DbHelper2.RunSpInteger("Act_IssueUpdateClient",
				DbHelper2.mp("@IssueId", SqlDbType.Int, issueId),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@orgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion

		#region UpdateProject()
		public static int UpdateProject(int issueId, int projectId)
		{
			return DbHelper2.RunSpInteger("Act_IssueUpdateProject",
				DbHelper2.mp("@IssueId", SqlDbType.Int, issueId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId));
		}
		#endregion

		#region UpdateResponsibility()
		public static int UpdateResponsibility(int issueId, int ResponsibleId, bool IsGroupResponsibility)
		{
			return DbHelper2.RunSpInteger("Act_IssueUpdateResponsible",
				DbHelper2.mp("@IssueId", SqlDbType.Int, issueId),
				DbHelper2.mp("@ResponsibleId", SqlDbType.Int, ResponsibleId),
				DbHelper2.mp("@IsGroupResponsibility", SqlDbType.Bit, IsGroupResponsibility));
		}
		#endregion

		#region UpdateOnCheck()
		public static void UpdateOnCheck(int issueId)
		{
			 DbHelper2.RunSp("Act_IssueUpdateOnCheck",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, issueId));
		}
		#endregion
		
		#region UpdateOnCheckByBox()
		public static void UpdateOnCheckByBox(int incidentBoxId)
		{
			DbHelper2.RunSp("Act_IssueUpdateOnCheckByBox",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, incidentBoxId));
		}
		#endregion

		#region NewMessageSet
		public static void NewMessageSet(int IncidentId, bool NewMessage)
		{
			DbHelper2.RunSp("IncidentNewMessageSet",
				DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId),
				DbHelper2.mp("@NewMessage", SqlDbType.Bit, NewMessage));
		}
		#endregion

		#region UpdateExpectedValues()
		public static int UpdateExpectedValues(int issueId, int expectedResponseTime, 
			int expectedDuration, int expectedAssignTime,
			DateTime expectedResolveDate, bool useDuration)
		{
			return DbHelper2.RunSpInteger("Act_IssueUpdateExpectedValues",
				DbHelper2.mp("@IssueId", SqlDbType.Int, issueId),
				DbHelper2.mp("@ExpectedResponseTime", SqlDbType.Int, expectedResponseTime),
				DbHelper2.mp("@ExpectedDuration", SqlDbType.Int, expectedDuration),
				DbHelper2.mp("@ExpectedAssignTime", SqlDbType.Int, expectedAssignTime),
				DbHelper2.mp("@ExpectedResolveDate", SqlDbType.DateTime, expectedResolveDate),
				DbHelper2.mp("@UseDuration", SqlDbType.Bit, useDuration));
		}
		#endregion
	}
}
