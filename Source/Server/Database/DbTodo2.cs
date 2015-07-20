using System;
using System.Data;

namespace Mediachase.IBN.Database
{
	public class DbTodo2
	{
		#region UpdateGeneralInfo()
		public static void UpdateGeneralInfo(int todoId, string title, string description)
		{
			DbHelper2.RunSp("Act_TodoUpdateGeneralInfo",
				DbHelper2.mp("@TodoId", SqlDbType.Int, todoId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, title),
				DbHelper2.mp("@Description", SqlDbType.NText, description));
		}
		#endregion

		#region UpdateTimeline()
		/// <summary>
		/// </summary>
		/// <param name="todoId"></param>
		/// <param name="startDate"></param>
		/// <param name="finishDate"></param>
		/// <returns>
		///		0 if no dates were changed
		///		1 if only startDate was changed
		///		2 if only finishDate was changed
		///		3 if both startDate and finishDate were changed
		/// </returns>
		public static int UpdateTimeline(int todoId, Object startDate, Object finishDate, int taskTime)
		{
			return DbHelper2.RunSpInteger("Act_TodoUpdateTimeline",
				DbHelper2.mp("@TodoId", SqlDbType.Int, todoId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, startDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, finishDate),
				DbHelper2.mp("@taskTime", SqlDbType.Int, taskTime));
		}
		#endregion

		#region UpdatePriority()
		public static int UpdatePriority(int todoId, int valueId)
		{
			return DbHelper2.RunSpInteger("Act_TodoUpdatePriority",
				DbHelper2.mp("@TodoId", SqlDbType.Int, todoId),
				DbHelper2.mp("@ValueId", SqlDbType.Int, valueId));
		}
		#endregion

		#region AddResource
		public static void AddResource(int objectId, int principalId, bool mustBeConfirmed)
		{
			DbHelper2.RunSp("Act_TodoResourceAdd",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, principalId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, mustBeConfirmed));
		}
		#endregion

		#region UpdateResource
		public static int UpdateResource(int objectId, int principalId, bool mustBeConfirmed)
		{
			return DbHelper2.RunSpInteger("Act_TodoResourceUpdate",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, principalId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, mustBeConfirmed));
		}
		#endregion

		#region UpdateConfigurationInfo()
		public static int UpdateConfigurationInfo(int todoId, int activationTypeId, int completionTypeId, bool mustBeConfirmed)
		{
			return DbHelper2.RunSpInteger("Act_TodoUpdateConfigurationInfo",
				DbHelper2.mp("@TodoId", SqlDbType.Int, todoId),
				DbHelper2.mp("@activationTypeId", SqlDbType.Int, activationTypeId),
				DbHelper2.mp("@completionTypeId", SqlDbType.Int, completionTypeId),
				DbHelper2.mp("@mustBeConfirmed", SqlDbType.Bit, mustBeConfirmed));
		}
		#endregion

		#region ResourceReply
		public static int ResourceReply(int objectId, int userId, bool isConfirmed)
		{
			return DbHelper2.RunSpInteger("Act_TodoResourceReply",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@IsConfirmed", SqlDbType.Bit, isConfirmed));
		}
		#endregion
	
		#region UpdateClient()
		public static int UpdateClient(int todoId, object contactUid, object orgUid)
		{
			return DbHelper2.RunSpInteger("Act_ToDoUpdateClient",
				DbHelper2.mp("@ToDoId", SqlDbType.Int, todoId),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion
	}
}
