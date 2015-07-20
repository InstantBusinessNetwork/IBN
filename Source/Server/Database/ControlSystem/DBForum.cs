using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.ControlSystem
{
	/// <summary>
	/// Summary description for DBForum.
	/// </summary>
	public class DBForum
	{
		private DBForum()
		{
		}

		// [ForumId], [ContainerKey], [Created], [Name], [Description] 
		public static IDataReader GetForumsByContainerKey(string ContainerKey)
		{
			return DbHelper2.RunSpDataReader("fsc_ForumsGetByContainerKey",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey));
		}

		// [ForumId], [ContainerKey], [Created], [Name], [Description] 
		public static IDataReader GetForum(int ForumId)
		{
			return DbHelper2.RunSpDataReader("fsc_ForumGet",
				DbHelper2.mp("@ForumId", SqlDbType.Int, ForumId));
		}

		// [ThreadId], [ForumId], [Created], [Name]
		public static IDataReader GetForumThreadsByForumId(int ForumId)
		{
			return DbHelper2.RunSpDataReader("fsc_ForumTheradsGetByForumId",
				DbHelper2.mp("@ForumId", SqlDbType.Int,  ForumId));
		}

		// [ThreadId], [ForumId], [Created], [Name]
		public static IDataReader GetForumThread(int ThreadId)
		{
			return DbHelper2.RunSpDataReader("fsc_ForumThreadGet",
				DbHelper2.mp("@ThreadId", SqlDbType.Int, ThreadId));
		}


		// [ThreadId], [ForumId], [Created], [Name]
		public static IDataReader GetForumThreadsByContainerKey(string ContainerKey)
		{
			return DbHelper2.RunSpDataReader("fsc_ForumTheradsGetByContainerKey",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey));
		}

		// NodeId, ThreadId, Text, Created, CreatorId, CreatorName, CreatorEmail, EMailMessageId, NodeType  
		public static IDataReader GetForumThreadNodesByThreadId(int TimeZoneId, int ThreadId)
		{
			return DbHelper2.RunSpDataReader(TimeZoneId, new string[]{"Created"}, 
				"fsc_ForumTheradNodesGetByThreadId",
				DbHelper2.mp("@ThreadId", SqlDbType.Int,  ThreadId));
		}

		// NodeId, ThreadId, Text, Created, CreatorId, CreatorName, CreatorEmail, EMailMessageId, NodeType  
		public static IDataReader GetForumThreadNode(int TimeZoneId, int NodeId)
		{
			return DbHelper2.RunSpDataReader(TimeZoneId, new string[]{"Created"},
				"fsc_ForumThreadNodeGet",
				DbHelper2.mp("@NodeId", SqlDbType.Int, NodeId));
		}


		// NodeId, ThreadId, Text, Created, CreatorId, CreatorName, CreatorEmail, EMailMessageId, NodeType 
		public static IDataReader GetForumThreadNodesByContainerKey(int TimeZoneId, string ContainerKey)
		{
			return DbHelper2.RunSpDataReader(TimeZoneId, new string[]{"Created"},
				"fsc_ForumTheradNodesGetByContainerKey",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey));
		}

		// NodeId, ThreadId, Text, Created, CreatorId, CreatorName, CreatorEmail, EMailMessageId, NodeType  
		public static IDataReader GetForumThreadNodesByForumId(int TimeZoneId, int ForumId)
		{
			return DbHelper2.RunSpDataReader(TimeZoneId, new string[]{"Created"},
				"fsc_ForumTheradNodesGetByForumId",
				DbHelper2.mp("@ForumId", SqlDbType.Int, ForumId));
		}

		public static int CreateForum(string ContainerKey, string Name,  string Description, DateTime Created)
		{
			return DbHelper2.RunSpInteger("fsc_ForumCreate",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey),
				DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, Name),
				DbHelper2.mp("@Description", SqlDbType.NText, Description),
				DbHelper2.mp("@Created", SqlDbType.DateTime, Created));
		}

		public static int CreateForumThread(int ForumId, string Name, DateTime Created)
		{
			return DbHelper2.RunSpInteger("fsc_ForumThreadCreate",
				DbHelper2.mp("@ForumId", SqlDbType.Int, ForumId),
				DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, Name),
				DbHelper2.mp("@Created", SqlDbType.DateTime, Created));
		}

		public static int CreateSimpleForumThread(string ContainerKey, string Name, DateTime Created)
		{
			return DbHelper2.RunSpInteger("fsc_ForumThreadSimpleCreate",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey),
				DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, Name),
				DbHelper2.mp("@Created", SqlDbType.DateTime, Created));
		}

		public static int CreateForumThreadNode(int ThreadId, string Text, DateTime Created, 
			object CreatorId,
			object CreatorName,
			object CreatorEmail,
			object EMailMessageId,
			int NodeType)
		{
			return DbHelper2.RunSpInteger("fsc_ForumThreadNodeCreate",
				DbHelper2.mp("@ThreadId", SqlDbType.Int, ThreadId),
				DbHelper2.mp("@Text", SqlDbType.NText, Text),
				DbHelper2.mp("@Created", SqlDbType.DateTime, Created),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, SqlHelper.Null2DBNull(CreatorId)),
				DbHelper2.mp("@CreatorName", SqlDbType.NVarChar, 255, SqlHelper.Null2DBNull(CreatorName)),
				DbHelper2.mp("@CreatorEmail", SqlDbType.NVarChar, 50, SqlHelper.Null2DBNull(CreatorEmail)),
				DbHelper2.mp("@EMailMessageId", SqlDbType.Int, SqlHelper.Null2DBNull(EMailMessageId)),
				DbHelper2.mp("@NodeType", SqlDbType.Int, NodeType)
				);
		}

		public static int CreateSimpleForumThreadNode(string ContainerKey, string Text, DateTime Created, 
			object CreatorId,
			object CreatorName,
			object CreatorEmail,
			object EMailMessageId,
			int NodeType)
		{
			return DbHelper2.RunSpInteger("fsc_ForumThreadNodeSimpleCreate",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey),
				DbHelper2.mp("@Text", SqlDbType.NText, Text),
				DbHelper2.mp("@Created", SqlDbType.DateTime, Created),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, SqlHelper.Null2DBNull(CreatorId)),
				DbHelper2.mp("@CreatorName", SqlDbType.NVarChar, 255, SqlHelper.Null2DBNull(CreatorName)),
				DbHelper2.mp("@CreatorEmail", SqlDbType.NVarChar, 50, SqlHelper.Null2DBNull(CreatorEmail)),
				DbHelper2.mp("@EMailMessageId", SqlDbType.Int, SqlHelper.Null2DBNull(EMailMessageId)),
				DbHelper2.mp("@NodeType", SqlDbType.Int, NodeType)
				);
		}

//		public static int CreateFile(int NodeId, string FileName, int ContentTypeId)
//		{
//			return DbHelper2.RunSpInteger("fsc_ForumFileCreate",
//				DbHelper2.mp("@NodeId", SqlDbType.Int, NodeId),
//				DbHelper2.mp("@FileName", SqlDbType.NVarChar, 255, FileName),
//				DbHelper2.mp("@ContentTypeId", SqlDbType.Int, ContentTypeId)
//				);
//		}

		// [FileId], [NodeId], [FileName], [FileBinaryId], Length, DownloadCount
//		public static IDataReader GetForumFilesByNodeId(int NodeId)
//		{
//			return DbHelper2.RunSpDataReader("fsc_ForumFilesGetByNodeId",
//				DbHelper2.mp("@NodeId", SqlDbType.Int, NodeId));
//		}

		// [FileId], [NodeId], [FileName], [FileBinaryId], Length, DownloadCount
//		public static IDataReader GetForumFilesByThreadId(int ThreadId)
//		{
//			return DbHelper2.RunSpDataReader("fsc_ForumFilesGetByThreadId",
//				DbHelper2.mp("@ThreadId", SqlDbType.Int, ThreadId));
//		}

		// [FileId], [NodeId], [FileName], [FileBinaryId], Length, DownloadCount, ThreadId
//		public static IDataReader GetForumFilesByForumId(int ForumId)
//		{
//			return DbHelper2.RunSpDataReader("fsc_ForumFilesGetByForumId",
//				DbHelper2.mp("@ForumId", SqlDbType.Int, ForumId));
//		}

		// [FileId], [NodeId], [FileName], [FileBinaryId], Length, DownloadCount
//		public static IDataReader GetForumFile(int FileId)
//		{
//			return DbHelper2.RunSpDataReader("fsc_ForumFileGet",
//				DbHelper2.mp("@FileId", SqlDbType.Int, FileId));
//		}

//		public static Stream OpenRead(int FileBinaryId)
//		{
//			if (DbTransaction.Current != null)
//			{
//				return new SqlBlobStream(DbTransaction.Current.SqlTran, "fsc_ForumFilesBinaries",
//					"Data", SqlBlobAccess.Read, 
//					new SqlParameter("@FileBinaryId", FileBinaryId));
//			}
//			else 
//				throw new Exception("Transaction is not started");
//		}
//
//		public static Stream OpenWrite(int FileBinaryId)
//		{
//			if (DbTransaction.Current != null)
//			{
//				return new SqlBlobStream(DbTransaction.Current.SqlTran, "fsc_ForumFilesBinaries",
//					"Data", SqlBlobAccess.Write, 
//					new SqlParameter("@FileBinaryId", FileBinaryId));
//			}
//			else 
//				throw new Exception("Transaction is not started");
//		}

		public static void RenameForum(int ForumId, string newName, string newDescription)
		{
			DbHelper2.RunSp("fsc_ForumRename",
				DbHelper2.mp("@ForumId", SqlDbType.Int, ForumId),
				DbHelper2.mp("@NewName", SqlDbType.NVarChar, 255, newName),
				DbHelper2.mp("@NewDescription", SqlDbType.NText, newDescription));
		}

		public static void RenameForumThread(int ThreadId, string newName)
		{
			DbHelper2.RunSp("fsc_ForumThreadRename",
				DbHelper2.mp("@ThreadId", SqlDbType.Int, ThreadId),
				DbHelper2.mp("@NewName", SqlDbType.NVarChar, 255, newName));
		}

		public static void SetThreadNodeText(int NodeId, string Text)
		{
			DbHelper2.RunSp("fsc_ForumThreadNodeSetText",
				DbHelper2.mp("@NodeId", SqlDbType.Int, NodeId),
				DbHelper2.mp("@Text", SqlDbType.NText, Text));
		}

		public static void DeleteForumByContainerKey(string  ContainerKey)
		{
			DbHelper2.RunSp("fsc_ForumDeleteByContainerKey",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey));
		}

		public static void DeleteForumById(int ForumId)
		{
			DbHelper2.RunSp("fsc_ForumDeleteById",
				DbHelper2.mp("@ForumId", SqlDbType.Int, ForumId));
		}

		public static void DeleteForumThreadByForumId(int ForumId)
		{
			DbHelper2.RunSp("fsc_ForumThreadDeleteByForumId",
				DbHelper2.mp("@ForumId", SqlDbType.Int, ForumId));
		}

		public static void DeleteForumThreadById(int ThreadId)
		{
			DbHelper2.RunSp("fsc_ForumThreadDeleteById",
				DbHelper2.mp("@ThreadId", SqlDbType.Int, ThreadId));
		}

		public static void DeleteForumThreadNodeByThreadId(int ThreadId)
		{
			DbHelper2.RunSp("fsc_ForumThreadNodeDeleteByThreadId",
				DbHelper2.mp("@ThreadId", SqlDbType.Int, ThreadId));
		}

		public static void DeleteForumThreadNodeById(int NodeId)
		{
			DbHelper2.RunSp("fsc_ForumThreadNodeDeleteById",
				DbHelper2.mp("@NodeId", SqlDbType.Int, NodeId));
		}

		public static void DeleteForumsByContainerKey(string ContainerKey)
		{
			DbHelper2.RunSp("fsc_ForumsDeleteByContainerKey",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey));
		}

//		public static void DeleteFile(int FileId)
//		{
//			DbHelper2.RunSp("fsc_ForumFileDelete",
//				DbHelper2.mp("@FileId", SqlDbType.Int, FileId));
//		}

		#region Settings
		// SettingId, NodeId, [Key], Value
		public static IDataReader GetSettings(int NodeId)
		{
			return DbHelper2.RunSpDataReader("fsc_ForumThreadNodeSettingsGet", 
				DbHelper2.mp("@NodeId", SqlDbType.Int, NodeId));
		}

		public static int SetSetting(int NodeId, string Key, string Value)
		{
			return DbHelper2.RunSpInteger("fsc_ForumThreadNodeSettingSet", 
				DbHelper2.mp("@NodeId", SqlDbType.Int, NodeId),
				DbHelper2.mp("@Key", SqlDbType.NVarChar, 100, Key),
				DbHelper2.mp("@Value", SqlDbType.NText, Value, false));
		}

		public static void RemoveSetting(int NodeId, string Key)
		{
			DbHelper2.RunSp("fsc_ForumThreadNodeSettingRemove", 
				DbHelper2.mp("@NodeId", SqlDbType.Int, NodeId),
				DbHelper2.mp("@Key", SqlDbType.NVarChar, 100, Key));
		}
		#endregion

        //[ContainerKey]
        public static IDataReader GetOwnerContainerKey(int NodeId)
        {
            return DbHelper2.RunSpDataReader("fsc_ForumContainerKeyByNodeId",
                DbHelper2.mp("@NodeId", SqlDbType.Int, NodeId));
        }
    }
}
