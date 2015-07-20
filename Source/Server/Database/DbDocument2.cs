using System;
using System.Data;


namespace Mediachase.IBN.Database
{
	public class DbDocument2
	{
		#region UpdateGeneralInfo()
		public static void UpdateGeneralInfo(int documentId, string title, string description)
		{
			DbHelper2.RunSp("Act_DocumentUpdateGeneralInfo",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, documentId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, title),
				DbHelper2.mp("@Description", SqlDbType.NText, description));
		}
		#endregion

		#region UpdatePriority()
		public static int UpdatePriority(int documentId, int valueId)
		{
			return DbHelper2.RunSpInteger("Act_DocumentUpdatePriority",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, documentId),
				DbHelper2.mp("@ValueId", SqlDbType.Int, valueId));
		}
		#endregion

		#region UpdateStatus()
		public static int UpdateStatus(int documentId, int valueId)
		{
			return DbHelper2.RunSpInteger("Act_DocumentUpdateStatus",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, documentId),
				DbHelper2.mp("@ValueId", SqlDbType.Int, valueId));
		}
		#endregion

		#region ResourceAdd
		public static void ResourceAdd(int objectId, int principalId, bool mustBeConfirmed, bool canManage)
		{
			DbHelper2.RunSp("Act_DocumentResourceAdd",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, principalId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, mustBeConfirmed),
				DbHelper2.mp("@CanManage", SqlDbType.Bit, canManage));
		}
		#endregion

		#region ResourceUpdate
		public static int ResourceUpdate(int objectId, int principalId, bool mustBeConfirmed, bool canManage)
		{
			return DbHelper2.RunSpInteger("Act_DocumentResourceUpdate",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, principalId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, mustBeConfirmed),
				DbHelper2.mp("@CanManage", SqlDbType.Bit, canManage));
		}
		#endregion

		#region ResourceReply
		public static int ResourceReply(int objectId, int userId, bool isConfirmed)
		{
			return DbHelper2.RunSpInteger("Act_DocumentResourceReply",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@IsConfirmed", SqlDbType.Bit, isConfirmed));
		}
		#endregion

		#region UpdateTimeline()
		public static int UpdateTimeline(int documentId, int taskTime)
		{
			return DbHelper2.RunSpInteger("Act_DocumentUpdateTimeline",
				DbHelper2.mp("@documentId", SqlDbType.Int, documentId),
				DbHelper2.mp("@taskTime", SqlDbType.Int, taskTime));
		}
		#endregion

		#region UpdateClient()
		public static int UpdateClient(int DocumentId, object ContactUid, object OrgUid)
		{
			return DbHelper2.RunSpInteger("Act_DocumentUpdateClient",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, DocumentId),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, ContactUid),
				DbHelper2.mp("@OrgUid", SqlDbType.UniqueIdentifier, OrgUid));
		}
		#endregion

		#region UpdateManager()
		public static int UpdateManager(int documentId, int valueId)
		{
			return DbHelper2.RunSpInteger("Act_DocumentUpdateManager",
				DbHelper2.mp("@DocumentId", SqlDbType.Int, documentId),
				DbHelper2.mp("@ValueId", SqlDbType.Int, valueId));
		}
		#endregion

	}
}
