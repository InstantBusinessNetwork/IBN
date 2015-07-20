using System;
using System.Data;

namespace Mediachase.IBN.Database
{
	public class DbTask2
	{
		#region UpdateGeneralInfo()
		public static void UpdateGeneralInfo(int objectId, string title, string description)
		{
			DbHelper2.RunSp("Act_TaskUpdateGeneralInfo",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, title),
				DbHelper2.mp("@Description", SqlDbType.NText, description));
		}
		#endregion

		#region UpdatePriority()
		public static int UpdatePriority(int objectId, int valueId)
		{
			return DbHelper2.RunSpInteger("Act_TaskUpdatePriority",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@ValueId", SqlDbType.Int, valueId));
		}
		#endregion

		#region AddResource
		public static void AddResource(int objectId, int principalId, bool mustBeConfirmed, bool canManage)
		{
			DbHelper2.RunSp("Act_TaskResourceAdd",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, principalId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, mustBeConfirmed),
				DbHelper2.mp("@CanManage", SqlDbType.Bit, canManage));
		}
		#endregion

		#region UpdateResource
		public static int UpdateResource(int objectId, int principalId, bool mustBeConfirmed, bool canManage)
		{
			return DbHelper2.RunSpInteger("Act_TaskResourceUpdate",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, principalId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, mustBeConfirmed),
				DbHelper2.mp("@CanManage", SqlDbType.Bit, canManage));
		}
		#endregion

		#region UpdateTaskTime()
		public static int UpdateTaskTime(int objectId, int value)
		{
			return DbHelper2.RunSpInteger("Act_TaskUpdateTasktime",
				DbHelper2.mp("@objectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@value", SqlDbType.Int, value));
		}
		#endregion

		#region UpdateConfigurationInfo()
		public static int UpdateConfigurationInfo(int taskId, int activationTypeId, int completionTypeId, bool mustBeConfirmed)
		{
			return DbHelper2.RunSpInteger("Act_TaskUpdateConfigurationInfo",
				DbHelper2.mp("@TaskId", SqlDbType.Int, taskId),
				DbHelper2.mp("@activationTypeId", SqlDbType.Int, activationTypeId),
				DbHelper2.mp("@completionTypeId", SqlDbType.Int, completionTypeId),
				DbHelper2.mp("@mustBeConfirmed", SqlDbType.Bit, mustBeConfirmed));
		}
		#endregion

		#region UpdatePhase()
		public static int UpdatePhase(int objectId, int valueId)
		{
			return DbHelper2.RunSpInteger("Act_TaskUpdatePhase",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@ValueId", SqlDbType.Int, valueId));
		}
		#endregion
	}
}
