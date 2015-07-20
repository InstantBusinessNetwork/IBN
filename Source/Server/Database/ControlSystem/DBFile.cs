using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Mediachase.IBN.Database.ControlSystem
{
	/// <summary>
	/// Summary description for DBFile.
	/// </summary>
	public class DBFile
	{
		#region Create
		/// <summary>
		/// Reader returns fields:
		///		FileId, Name, DirectoryId, FileBinaryId, CreatorId, Created, ModifierId, Modified, AllowHistory, Description
		/// </summary>
		public static IDataReader Create(int TimeZoneId, string Name, int DirectoryId, int CreatorId, DateTime Created, string Description)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_FileCreate", 
				DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, Name),
				DbHelper2.mp("@DirectoryId", SqlDbType.Int, DirectoryId),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@Created", SqlDbType.DateTime, Created),
				DbHelper2.mp("@Description", SqlDbType.NText, Description));
		}

		public static int CreateBinary(int FileId, int ContentTypeId)
		{
			return DbHelper2.RunSpInteger("fsc_FileBinaryCreate",
				DbHelper2.mp("@FileId", SqlDbType.Int, FileId),
				DbHelper2.mp("@ContentTypeId", SqlDbType.Int, ContentTypeId));
		}
		#endregion

		#region Get

		/// <summary>
		/// Reader returns fields:
		///		FileId, Name, DirectoryId, FileBinaryId, CreatorId, Created, ModifierId, Modified, Length, AllowHistory, Description
		/// </summary>
		public static IDataReader GetById(int TimeZoneId, int FileId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_FileGetById",
				DbHelper2.mp("@FileId", SqlDbType.Int, FileId));
		}
		/// <summary>
		/// Reader returns fields:
		///		FileId, Name, DirectoryId, FileBinaryId, CreatorId, Created, ModifierId, Modified, Length, AllowHistory, Description
		/// </summary>
		public static IDataReader GetByName(int TimeZoneId, string Name, int DirectoryId, string ContainerKey)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_FileGetByName",
				DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, Name),
				DbHelper2.mp("@DirectoryId", SqlDbType.Int, DirectoryId),
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 255, ContainerKey));
		}
		#endregion

		#region GetViewCount
		public static int GetDownloadCount(int FileBinaryId)
		{
			return DbHelper2.RunSpInteger("fsc_FileGetDownloadCount",
				DbHelper2.mp("@FileBinaryId", SqlDbType.Int, FileBinaryId));
		}
		#endregion

		#region OpenRead
		public static Stream OpenRead(int FileBinaryId)
		{
			SqlTransaction sqlTran = DbContext.Current.Transaction;
			if (sqlTran != null)
			{
				return new SqlBlobStream(sqlTran, "fsc_FileBinaries", "Data", SqlBlobAccess.Read
					, new SqlParameter("@FileBinaryId", FileBinaryId));
			}
			else
				throw new Exception("Transaction is not started");
		}
		#endregion

		#region OpenWrite
		public static Stream OpenWrite(int FileBinaryId)
		{
			SqlTransaction sqlTran = DbContext.Current.Transaction;
			if (sqlTran != null)
			{
				return new SqlBlobStream(sqlTran, "fsc_FileBinaries", "Data", SqlBlobAccess.Write
					, new SqlParameter("@FileBinaryId", FileBinaryId));
			}
			else
				throw new Exception("Transaction is not started");
		}
		#endregion

		#region Rename
		/// <summary>
		/// Reader returns fields:
		///		FileId, Name, DirectoryId, FileBinaryId, CreatorId, Created, ModifierId, Modified, Length, AllowHistory, Description
		/// </summary>
		public static IDataReader Rename(int TimeZoneId, int FileId, string NewName, int ModifierId, DateTime Modified)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_FileRename",
				DbHelper2.mp("@FileId", SqlDbType.Int, FileId),
				DbHelper2.mp("@NewName", SqlDbType.NVarChar, 255, NewName),
				DbHelper2.mp("@ModifierId", SqlDbType.Int, ModifierId),
				DbHelper2.mp("@Modified", SqlDbType.DateTime, Modified));
		}
		#endregion

		#region Modify
		/// <summary>
		/// Reader returns fields:
		///		FileId, Name, DirectoryId, FileBinaryId, CreatorId, Created, ModifierId, Modified, Length, AllowHistory, Description
		/// </summary>
		public static IDataReader Modify(int TimeZoneId, int FileId, int ModifierId, DateTime Modified)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_FileModify",
				DbHelper2.mp("@FileId", SqlDbType.Int, FileId),
				DbHelper2.mp("@ModifierId", SqlDbType.Int, ModifierId),
				DbHelper2.mp("@Modified", SqlDbType.DateTime, Modified));
		}
		#endregion		

		#region FileBinaryModifyCounter
		public static void FileBinaryModifyCounter(int FileBinaryId)
		{
			DbHelper2.RunSp("fsc_FileBinaryModifyCounter",
				DbHelper2.mp("@FileBinaryId", SqlDbType.Int, FileBinaryId));
		}
		#endregion

		#region Move
		/// Reader returns fields:
		///		FileId, Name, DirectoryId, FileBinaryId, CreatorId, Created, ModifierId, Modified, Length, AllowHistory, Description
		public static IDataReader Move(int TimeZoneId, int FileId, int DestDirectoryId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_FileMove",
				DbHelper2.mp("@FileId", SqlDbType.Int, FileId),
				DbHelper2.mp("@DestDirectoryId", SqlDbType.Int, DestDirectoryId));
		}
		#endregion

		#region Copy
		public static int Copy(int FileId, int DestDirectoryId, bool OverwriteExisting)
		{
			return DbHelper2.RunSpInteger("fsc_FileCopy",
				DbHelper2.mp("@FileId", SqlDbType.Int, FileId),
				DbHelper2.mp("@DestDirectoryId", SqlDbType.Int, DestDirectoryId),
                DbHelper2.mp("@OverwriteExisting", SqlDbType.Bit, OverwriteExisting));
		}
		#endregion

		#region AllowHistory
		/// Reader returns fields:
		///		FileId, Name, DirectoryId, FileBinaryId, CreatorId, Created, ModifierId, Modified, Length, AllowHistory, Description
		public static IDataReader AllowHistory(int TimeZoneId, int FileId, bool AllowHistory)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_FileAllowHistory",
				DbHelper2.mp("@FileId", SqlDbType.Int, FileId),
				DbHelper2.mp("@AllowHistory", SqlDbType.Bit, AllowHistory));
		}
		#endregion

		#region Delete
		public static void Delete(int FileId)
		{
			DbHelper2.RunSp("fsc_FileDelete",
				DbHelper2.mp("@FileId", SqlDbType.Int, FileId));
		}
		#endregion

		#region Search
		// ContainerKey, FileId, [Name], DirectoryId, FileBinaryId, CreatorId, Created, ModifierId, Modified, Length, ContentTypeString, AllowHistory, Description
		public static IDataReader Search(
			int TimeZoneId,
			int UserId,
			object strContainerKey,
			object intDirectoryId, object boolDeep, 
			object strKeyword,
			object intContentType,
			object dtModifiedFrom, object dtModifiedTo,
			object intLenghtFrom,  object intLenghtTo)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_FilesSearch",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, SqlHelper.Null2DBNull(strContainerKey)),
				DbHelper2.mp("@DirectoryId", SqlDbType.Int, SqlHelper.Null2DBNull(intDirectoryId)),
				DbHelper2.mp("@Deep", SqlDbType.Bit, SqlHelper.Null2DBNull(boolDeep)),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 255, SqlHelper.Null2DBNull(strKeyword)),
				DbHelper2.mp("@ContentType", SqlDbType.Int, SqlHelper.Null2DBNull(intContentType)),
				DbHelper2.mp("@ModifiedFrom", SqlDbType.DateTime, SqlHelper.Null2DBNull(dtModifiedFrom)),
				DbHelper2.mp("@ModifiedTo", SqlDbType.DateTime, SqlHelper.Null2DBNull(dtModifiedTo)),
				DbHelper2.mp("@LengthFrom", SqlDbType.Int, SqlHelper.Null2DBNull(intLenghtFrom)),
				DbHelper2.mp("@LengthTo", SqlDbType.Int, SqlHelper.Null2DBNull(intLenghtTo)));
		}
		#endregion

		#region GetHistory
		/// <summary>
		/// Reader returns fields:
		///		FileId, Name, FileBinaryId, ModifierId, Modified, Length, ContentTypeString, ContentTypeId
		/// </summary>
		public static IDataReader GetHistory(int TimeZoneId, int FileId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Modified"},
				"fsc_FileGetHistory",
				DbHelper2.mp("@FileId", SqlDbType.Int, FileId));
		}
		#endregion

		#region Update Description
		/// <summary>
		/// Updates the description.
		/// </summary>
		/// <param name="fileId">The file id.</param>
		/// <param name="description">The description.</param>
		public static void UpdateDescription(int fileId, string description)
		{
			if (description == null)
				throw new ArgumentNullException("description");

			DbHelper2.RunSp("fsc_FileUpdateDescription",
							DbHelper2.mp("@FileId", SqlDbType.Int, fileId),
							DbHelper2.mp("@Description", SqlDbType.NText, description));
		}
		#endregion

		#region RecalculateFileSize
		/// <summary>
		/// Recalculates the size of the file.
		/// </summary>
		/// <param name="fileBinaryId">The file binary id.</param>
		public static void RecalculateFileSize(int fileBinaryId)
		{
			DbHelper2.RunSp("fsc_FileBinaryRecalculateFileSize",
				DbHelper2.mp("@FileBinaryId", SqlDbType.Int, fileBinaryId));
		}
		#endregion
	}
}
