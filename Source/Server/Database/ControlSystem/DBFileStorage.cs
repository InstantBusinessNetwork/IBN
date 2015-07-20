using System;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database.ControlSystem
{
	/// <summary>
	/// Summary description for DBFileStorage.
	/// </summary>
	public class DBFileStorage
	{
		#region -- CanUserRunAction --
		public static bool CanUserRunAction(int UserId, string ContainerKey, int DirectoryId, string Action)
		{
			return (DbHelper2.RunSpInteger("fsc_CanUserRunAction",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey),
				DbHelper2.mp("@DirectoryId", SqlDbType.Int, DirectoryId),
				DbHelper2.mp("@Action", SqlDbType.NVarChar, 50, Action))==1);
		}
		#endregion

		#region FilesSearch
		// ContainerKey, FileId, [Name], DirectoryId, FileBinaryId, CreatorId, Created, ModifierId, Modified, Length, ContentTypeString, AllowHistory
		public static IDataReader Search(
			int UserId,
			object ProjectId,
			object ObjectTypeId,
			object ObjectId,
			object strKeyword,
			object intContentType,
			object dtModifiedFrom, object dtModifiedTo,
			object intLenghtFrom,  object intLenghtTo)
		{
			return DbHelper2.RunSpDataReader("WorkSpaceFilesSearch",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),

				DbHelper2.mp("@ProjectId", SqlDbType.Int, SqlHelper.Null2DBNull(ProjectId)),
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, SqlHelper.Null2DBNull(ObjectTypeId)),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, SqlHelper.Null2DBNull(ObjectId)),

				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 255, SqlHelper.Null2DBNull(strKeyword)),
				DbHelper2.mp("@ContentType", SqlDbType.Int, SqlHelper.Null2DBNull(intContentType)),
				DbHelper2.mp("@ModifiedFrom", SqlDbType.DateTime, SqlHelper.Null2DBNull(dtModifiedFrom)),
				DbHelper2.mp("@ModifiedTo", SqlDbType.DateTime, SqlHelper.Null2DBNull(dtModifiedTo)),
				DbHelper2.mp("@LengthFrom", SqlDbType.Int, SqlHelper.Null2DBNull(intLenghtFrom)),
				DbHelper2.mp("@LengthTo", SqlDbType.Int, SqlHelper.Null2DBNull(intLenghtTo)));
		}
		#endregion	
	}
}
