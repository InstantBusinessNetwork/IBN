using System;
using System.Data;

namespace Mediachase.IBN.Database
{
	public class DbCalendarEntry2
	{
		#region UpdateProjectAndManager
		public static void UpdateProjectAndManager(int eventId, int projectId, int managerId)
		{
			DbHelper2.RunSp("Act_EventUpdateProjectAndManager",
				DbHelper2.mp("@EventId", SqlDbType.Int, eventId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@ManagerId", SqlDbType.Int, managerId));
		}
		#endregion

		#region UpdateGeneralInfo()
		public static void UpdateGeneralInfo(int eventId, int typeId, string title, string description, string location)
		{
			DbHelper2.RunSp("Act_EventUpdateGeneralInfo",
				DbHelper2.mp("@EventId", SqlDbType.Int, eventId),
				DbHelper2.mp("@TypeId", SqlDbType.Int, typeId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, title),
				DbHelper2.mp("@Description", SqlDbType.NText, description),
				DbHelper2.mp("@Location", SqlDbType.NVarChar, 1000, location));
		}
		#endregion

		#region UpdateTimeline()
		/// <summary>
		/// </summary>
		/// <param name="eventId"></param>
		/// <param name="startDate"></param>
		/// <param name="finishDate"></param>
		/// <returns>
		///		0 if no dates were changed
		///		1 if only startDate was changed
		///		2 if only finishDate was changed
		///		3 if both startDate and finishDate were changed
		/// </returns>
		public static int UpdateTimeline(int eventId, DateTime startDate, DateTime finishDate, int taskTime)
		{
			return DbHelper2.RunSpInteger("Act_EventUpdateTimeline",
				DbHelper2.mp("@EventId", SqlDbType.Int, eventId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, startDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, finishDate),
				DbHelper2.mp("@TaskTime", SqlDbType.Int, taskTime));
		}
		#endregion

		#region UpdatePriority()
		public static int UpdatePriority(int eventId, int valueId)
		{
			return DbHelper2.RunSpInteger("Act_EventUpdatePriority",
				DbHelper2.mp("@EventId", SqlDbType.Int, eventId),
				DbHelper2.mp("@ValueId", SqlDbType.Int, valueId));
		}
		#endregion

		#region ResourceAdd
		public static void ResourceAdd(int objectId, int principalId, bool mustBeConfirmed)
		{
			DbHelper2.RunSp("Act_EventResourceAdd",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, principalId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, mustBeConfirmed));
		}
		#endregion

		#region ResourceUpdate
		public static int ResourceUpdate(int objectId, int principalId, bool mustBeConfirmed)
		{
			return DbHelper2.RunSpInteger("Act_EventResourceUpdate",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, principalId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, mustBeConfirmed));
		}
		#endregion

		#region ResourceReply
		public static int ResourceReply(int objectId, int userId, bool isConfirmed)
		{
			return DbHelper2.RunSpInteger("Act_EventResourceReply",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@IsConfirmed", SqlDbType.Bit, isConfirmed));
		}
		#endregion

		#region UpdateClient()
		public static int UpdateClient(int eventId, object contactUid, object orgUid)
		{
			return DbHelper2.RunSpInteger("Act_EventUpdateClient",
				DbHelper2.mp("@EventId", SqlDbType.Int, eventId),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion
	}
}
