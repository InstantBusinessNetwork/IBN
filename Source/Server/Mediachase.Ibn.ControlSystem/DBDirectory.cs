using System;
using System.Data;

using Mediachase.Database;


namespace Mediachase.Ibn.ControlSystem
{
	/// <summary>
	/// Summary description for DBFolder.
	/// </summary>
	public class DBDirectory
	{
		#region Create
		/// <summary>
		/// Reader returns fields:
		///		DirectoryId, Name,  Path, CreatorId, Created, ModifierId, Modified, ParentDirectoryId
		/// </summary>
		public static IDataReader Create(string ContainerKey, string Name, int ParentDirectoryId, int CreatorId, DateTime Created)
		{
			return DBHelper2.DBHelper.RunSPDataReader("fsc_DirectoryCreate",
				DBHelper.MP("@ContainerKey", SqlDbType.NVarChar, 255, ContainerKey),
				DBHelper.MP("@Name", SqlDbType.NVarChar, 255, Name),
				DBHelper.MP("@ParentDirectoryId", SqlDbType.Int, ParentDirectoryId),
				DBHelper.MP("@CreatorId", SqlDbType.Int, CreatorId),
				DBHelper.MP("@Created", SqlDbType.DateTime, Created));
		}
		public static IDataReader CreateRoot(string ContainerKey, string Name, int CreatorId, DateTime Created)
		{
			return DBHelper2.DBHelper.RunSPDataReader("fsc_DirectoryCreateRoot",
				DBHelper.MP("@ContainerKey", SqlDbType.NVarChar, 255, ContainerKey),
				DBHelper.MP("@Name", SqlDbType.NVarChar, 255, Name),
				DBHelper.MP("@CreatorId", SqlDbType.Int, CreatorId),
				DBHelper.MP("@Created", SqlDbType.DateTime, Created));
		}
		#endregion

		#region Get
		/// <summary>
		/// Reader returns fields:
		///		DirectoryId, Name,  Path, CreatorId, Created, ModifierId, Modified, ParentDirectoryId
		/// </summary>
		public static IDataReader GetById(int DirectoryId)
		{
			return DBHelper2.DBHelper.RunSPDataReader("fsc_DirectoryGetById",
				DBHelper.MP("@DirectoryId", SqlDbType.Int, DirectoryId));
		}

		/// <summary>
		/// Reader returns fields:
		///		DirectoryId, Name,  Path, CreatorId, Created, ModifierId, Modified, ParentDirectoryId
		/// </summary>
		public static IDataReader GetByName(string Name, int ParentDirectoryId, string ContainerKey)
		{
			return DBHelper2.DBHelper.RunSPDataReader("fsc_DirectoryGetByName",
				DBHelper.MP("@Name", SqlDbType.NVarChar, 255, Name),
				DBHelper.MP("@ParentDirectoryId", SqlDbType.Int, ParentDirectoryId),
				DBHelper.MP("@ContainerKey", SqlDbType.NVarChar, 255, ContainerKey));
		}

		/// <summary>
		/// Reader returns fields:
		///		DirectoryId, Name,  Path, CreatorId, Created, ModifierId, Modified, ParentDirectoryId
		/// </summary>
		public static IDataReader GetRoot(string ContainerKey)
		{
			return DBHelper2.DBHelper.RunSPDataReader("fsc_DirectoryGetRoot",
				DBHelper.MP("@ContainerKey", SqlDbType.NVarChar, 255, ContainerKey));
		}
		#endregion

		#region Rename
		/// <summary>
		/// Reader returns fields:
		///		DirectoryId, Name,  Path, CreatorId, Created, ModifierId, Modified, ParentDirectoryId
		/// </summary>
		public static IDataReader Rename(int DirectoryId, string NewName, int ModifierId, DateTime Modified)
		{
			return DBHelper2.DBHelper.RunSPDataReader("fsc_DirectoryRename",
				DBHelper.MP("@DirectoryId", SqlDbType.Int, DirectoryId),
				DBHelper.MP("@NewName", SqlDbType.NVarChar, 255, NewName),
				DBHelper.MP("@ModifierId", SqlDbType.Int, ModifierId),
				DBHelper.MP("@Modified", SqlDbType.DateTime, Modified));
		}
		#endregion

		#region Modify
		/// <summary>
		/// Reader returns fields:
		///		DirectoryId, Name,  Path, CreatorId, Created, ModifierId, Modified, ParentDirectoryId
		/// </summary>
		public static IDataReader Modify(int DirectoryId, int ModifierId, DateTime Modified)
		{
			return DBHelper2.DBHelper.RunSPDataReader("fsc_DirectoryModify",
				DBHelper.MP("@DirectoryId", SqlDbType.Int, DirectoryId),
				DBHelper.MP("@ModifierId", SqlDbType.Int, ModifierId),
				DBHelper.MP("@Modified", SqlDbType.DateTime, Modified));
		}
		#endregion

		#region GetChildDirectoriesByParent
		/// <summary>
		/// Reader returns fields:
		///		DirectoryId, Name,  Path, CreatorId, Created, ModifierId, Modified, ParentDirectoryId
		/// </summary>
		public static IDataReader GetChildDirectoriesByParent(int DirectoryId, string ContainerKey)
		{
			return DBHelper2.DBHelper.RunSPDataReader("fsc_DirectoriesGetByParent",
				DBHelper.MP("@ParentDirectoryId", SqlDbType.Int, DirectoryId),
				DBHelper.MP("@ContainerKey", SqlDbType.NVarChar, 255, ContainerKey));
		}
		#endregion

		#region GetChildFilesByDirectoryId
		/// <summary>
		/// Reader returns fields:
		///		FileId, Name, DirectoryId, FileBinaryId, CreatorId, Created, ModifierId, Modified, Length
		/// </summary>
		public static IDataReader GetChildFilesByDirectoryId(int DirectoryId, string ContainerKey)
		{
			return DBHelper2.DBHelper.RunSPDataReader("fsc_FilesGetByDirectoryId",
				DBHelper.MP("@DirectoryId", SqlDbType.Int, DirectoryId),
				DBHelper.MP("@ContainerKey", SqlDbType.NVarChar, 255, ContainerKey));
		}
		#endregion

		#region GetParentById
		/// <summary>
		/// Reader returns fields:
		///		DirectoryId, Name,  Path, CreatorId, Created, ModifierId, Modified, ParentDirectoryId
		/// </summary>
		public static IDataReader GetParentById(int DirectoryId)
		{
			return DBHelper2.DBHelper.RunSPDataReader("fsc_DirectoryGetParentById",
				DBHelper.MP("@DirectoryId", SqlDbType.Int, DirectoryId));
		}
		#endregion

		#region Move
		public static IDataReader Move(int DirectoryId, int NewParentDirectoryId)
		{
			return DBHelper2.DBHelper.RunSPDataReader("fsc_DirectoryMove",
				DBHelper.MP("@DirectoryId", SqlDbType.Int, DirectoryId),
				DBHelper.MP("@NewParentDirectoryId", SqlDbType.Int, NewParentDirectoryId));
		}
		#endregion

		#region Delete
		public static void Delete(int DirectoryId)
		{
			DBHelper2.DBHelper.RunSP("fsc_DirectoryDelete",
				DBHelper.MP("@DirectoryId", SqlDbType.Int, DirectoryId));
		}
		#endregion
	}
}
