using System;
using System.Data;

namespace Mediachase.IBN.Database.ControlSystem
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
		public static IDataReader Create(int TimeZoneId, string ContainerKey, string Name, int ParentDirectoryId, int CreatorId, DateTime Created)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_DirectoryCreate",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 255, ContainerKey),
				DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, Name),
				DbHelper2.mp("@ParentDirectoryId", SqlDbType.Int, ParentDirectoryId),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@Created", SqlDbType.DateTime, Created));
		}
		public static IDataReader CreateRoot(int TimeZoneId, string ContainerKey, string Name, int CreatorId, DateTime Created)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_DirectoryCreateRoot",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 255, ContainerKey),
				DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, Name),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@Created", SqlDbType.DateTime, Created));
		}
		#endregion

		#region Get
		/// <summary>
		/// Reader returns fields:
		///		DirectoryId, Name,  Path, CreatorId, Created, ModifierId, Modified, ParentDirectoryId, ContainerKey
		/// </summary>
		public static IDataReader GetById(int TimeZoneId, int DirectoryId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_DirectoryGetById",
				DbHelper2.mp("@DirectoryId", SqlDbType.Int, DirectoryId));
		}

		/// <summary>
		/// Reader returns fields:
		///		DirectoryId, Name,  Path, CreatorId, Created, ModifierId, Modified, ParentDirectoryId
		/// </summary>
		public static IDataReader GetByName(int TimeZoneId, string Name, int ParentDirectoryId, string ContainerKey)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_DirectoryGetByName",
				DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, Name),
				DbHelper2.mp("@ParentDirectoryId", SqlDbType.Int, ParentDirectoryId),
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 255, ContainerKey));
		}

		/// <summary>
		/// Reader returns fields:
		///		DirectoryId, Name,  Path, CreatorId, Created, ModifierId, Modified, ParentDirectoryId
		/// </summary>
		public static IDataReader GetRoot(int TimeZoneId, string ContainerKey)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_DirectoryGetRoot",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 255, ContainerKey));
		}
		#endregion

		#region Rename
		/// <summary>
		/// Reader returns fields:
		///		DirectoryId, Name,  Path, CreatorId, Created, ModifierId, Modified, ParentDirectoryId
		/// </summary>
		public static IDataReader Rename(int TimeZoneId, int DirectoryId, string NewName, int ModifierId, DateTime Modified)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_DirectoryRename",
				DbHelper2.mp("@DirectoryId", SqlDbType.Int, DirectoryId),
				DbHelper2.mp("@NewName", SqlDbType.NVarChar, 255, NewName),
				DbHelper2.mp("@ModifierId", SqlDbType.Int, ModifierId),
				DbHelper2.mp("@Modified", SqlDbType.DateTime, Modified));
		}
		#endregion

		#region Modify
		/// <summary>
		/// Reader returns fields:
		///		DirectoryId, Name,  Path, CreatorId, Created, ModifierId, Modified, ParentDirectoryId
		/// </summary>
		public static IDataReader Modify(int TimeZoneId, int DirectoryId, int ModifierId, DateTime Modified)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_DirectoryModify",
				DbHelper2.mp("@DirectoryId", SqlDbType.Int, DirectoryId),
				DbHelper2.mp("@ModifierId", SqlDbType.Int, ModifierId),
				DbHelper2.mp("@Modified", SqlDbType.DateTime, Modified));
		}
		#endregion

		#region GetChildDirectoriesByParent
		/// <summary>
		/// Reader returns fields:
		///		DirectoryId, Name,  Path, CreatorId, Created, ModifierId, Modified, ParentDirectoryId
		/// </summary>
		public static IDataReader GetChildDirectoriesByParent(int TimeZoneId, int DirectoryId, string ContainerKey)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_DirectoriesGetByParent",
				DbHelper2.mp("@ParentDirectoryId", SqlDbType.Int, DirectoryId),
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 255, ContainerKey));
		}
		#endregion

		#region GetChildFilesByDirectoryId
		/// <summary>
		/// Reader returns fields:
		///		FileId, Name, DirectoryId, FileBinaryId, CreatorId, Created, ModifierId, Modified, Length
		/// </summary>
		public static IDataReader GetChildFilesByDirectoryId(int TimeZoneId, int DirectoryId, string ContainerKey)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_FilesGetByDirectoryId",
				DbHelper2.mp("@DirectoryId", SqlDbType.Int, DirectoryId),
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 255, ContainerKey));
		}
		#endregion

		#region GetParentById
		/// <summary>
		/// Reader returns fields:
		///		DirectoryId, Name,  Path, CreatorId, Created, ModifierId, Modified, ParentDirectoryId
		/// </summary>
		public static IDataReader GetParentById(int TimeZoneId, int DirectoryId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_DirectoryGetParentById",
				DbHelper2.mp("@DirectoryId", SqlDbType.Int, DirectoryId));
		}
		#endregion

		#region Move
		public static IDataReader Move(int TimeZoneId, int DirectoryId, int NewParentDirectoryId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Created", "Modified"},
				"fsc_DirectoryMove",
				DbHelper2.mp("@DirectoryId", SqlDbType.Int, DirectoryId),
				DbHelper2.mp("@NewParentDirectoryId", SqlDbType.Int, NewParentDirectoryId));
		}
		#endregion

		#region Delete
		public static void Delete(int DirectoryId)
		{
			DbHelper2.RunSp("fsc_DirectoryDelete",
				DbHelper2.mp("@DirectoryId", SqlDbType.Int, DirectoryId));
		}
		#endregion

		#region GetContainerKey
		public static string GetContainerKey(int DirectoryId)
		{
			return (string)DbHelper2.RunSpScalar("fsc_ContainerKeyByDirectoryId", 
				DbHelper2.mp("@DirectoryId", SqlDbType.Int, DirectoryId));
		}
		#endregion
	}
}
