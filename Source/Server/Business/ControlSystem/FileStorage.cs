using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;

using Mediachase.Ibn;
using Mediachase.IBN.Database.ControlSystem;


namespace Mediachase.IBN.Business.ControlSystem
{
	public class FileUserTicket
	{
		public int UserId;
		public int FileId;
	}

	/// <summary>
	/// Summary description for FileStorage.
	/// </summary>
	public class FileStorage : IIbnControl
	{
		//private NameValueCollection	_params = new NameValueCollection();
		//private string				_ownerContainer.Key = string.Empty;
		private IIbnContainer _ownerContainer = null;
		private IbnControlInfo _info = null;
		private DirectoryInfo _root = null;
		private const int BufferSize = 655360;

		#region Create
		public static FileStorage Create(string containerName, string ñontainerKey)
		{
			if (containerName == null)
				throw new ArgumentNullException("containerName");
			if (ñontainerKey == null)
				throw new ArgumentNullException("ñontainerKey");

			string key = "FileStorage_" + containerName + "_" + ñontainerKey;

			if (HttpContext.Current != null && HttpContext.Current.Items.Contains(key))
				return (FileStorage)HttpContext.Current.Items[key];

			BaseIbnContainer bic = BaseIbnContainer.Create(containerName, ñontainerKey);
			FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");

			if (HttpContext.Current != null)
				HttpContext.Current.Items.Add(key, fs);

			return fs;
		}
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="FileStorage"/> class.
		/// </summary>
		public FileStorage()
		{
		}

		#region IIbnControl Members

		/// <summary>
		/// Inits the specified owner.
		/// </summary>
		/// <param name="owner">The owner.</param>
		/// <param name="controlInfo">The control info.</param>
		public void Init(IIbnContainer owner, IbnControlInfo controlInfo)
		{
			_ownerContainer = owner;

			_info = controlInfo;

			using (IDataReader reader = DBDirectory.GetRoot(CurrentTimeZoneId, owner.Key))
			{
				if (reader.Read())
					_root = new DirectoryInfo(this, reader);
			}

			if (_root == null)
			{
				using (IDataReader reader = DBDirectory.CreateRoot(CurrentTimeZoneId, owner.Key, "root", this.CurrentUserId, DateTime.Now))
				{
					if (reader.Read())
					{
						_root = new DirectoryInfo(this, reader);
					}
				}

				AccessControlList rootAcl = AccessControlList.GetACL(_root.Id);

				foreach (AccessControlEntry ace in _info.DefaultAccessControlList.GetACL(_ownerContainer.Key))
					rootAcl.Add(ace);

				if (rootAcl.Count > 0)
					AccessControlList.SetACL(this, rootAcl, false);
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get
			{
				return "FileStorage";
			}
		}

		/// <summary>
		/// Gets the owner container.
		/// </summary>
		/// <value>The owner container.</value>
		public IIbnContainer OwnerContainer
		{
			get
			{
				return _ownerContainer;
			}
		}

		/// <summary>
		/// Gets the actions.
		/// </summary>
		/// <value>The actions.</value>
		public string[] Actions
		{
			get
			{
				return new string[] { "Read", "Write", "Admin" };
			}
		}

		/// <summary>
		/// Gets the base actions.
		/// </summary>
		/// <param name="Action">The action.</param>
		/// <returns></returns>
		public string[] GetBaseActions(string Action)
		{
			switch (Action.ToLower())
			{
				case "read":
					return new string[] { };
				case "write":
					return new string[] { "Read" };
				case "admin":
					return new string[] { "Read", "Write" };
			}

			return new string[] { };
		}

		/// <summary>
		/// Gets the derived actions.
		/// </summary>
		/// <param name="Action">The action.</param>
		/// <returns></returns>
		public string[] GetDerivedActions(string Action)
		{
			switch (Action.ToLower())
			{
				case "read":
					return new string[] { "Write", "Admin" };
				case "write":
					return new string[] { "Admin" };
				case "admin":
					return new string[] { };
			}

			return new string[] { };
		}

		/// <summary>
		/// Gets the parameters.
		/// </summary>
		/// <value>The parameters.</value>
		public NameValueCollection Parameters
		{
			get
			{
				return _info.Parameters;
			}
		}

		#endregion

		/// <summary>
		/// Gets the info.
		/// </summary>
		/// <value>The info.</value>
		public IbnControlInfo Info
		{
			get
			{
				return _info;
			}
		}

		/// <summary>
		/// Gets a value indicating whether [allow create folder].
		/// </summary>
		/// <value><c>true</c> if [allow create folder]; otherwise, <c>false</c>.</value>
		public bool AllowCreateFolder
		{
			get
			{
				try
				{
					return bool.Parse(this.Parameters["AllowCreateFolder"]);
				}
				catch (Exception)
				{
					return true;
				}
			}
		}

		/// <summary>
		/// Gets the root.
		/// </summary>
		/// <value>The root.</value>
		public DirectoryInfo Root
		{
			get
			{
				return _root;
			}
		}

		/// <summary>
		/// Gets the current user id.
		/// </summary>
		/// <value>The current user id.</value>
		protected int CurrentUserId
		{
			get
			{
				if (Security.CurrentUser != null)
					return Security.CurrentUser.UserID;
				return -1;
			}
		}

		protected static int CurrentTimeZoneId
		{
			get
			{
				if (Security.CurrentUser != null)
					return Security.CurrentUser.TimeZoneId;
				return 0;
			}
		}


		#region -- SaveFile --
		/// <summary>
		/// Saves the file.
		/// </summary>
		/// <param name="FileName">Name of the file.</param>
		/// <param name="inputStream">The input stream.</param>
		/// <returns></returns>
		public FileInfo SaveFile(string FileName, Stream inputStream)
		{
			return SaveFile(_root.Id, FileName, string.Empty, this.CurrentUserId, DateTime.UtcNow, inputStream);
		}

		/// <summary>
		/// Saves the file.
		/// </summary>
		/// <param name="FileName">Name of the file.</param>
		/// <param name="Description">The description.</param>
		/// <param name="inputStream">The input stream.</param>
		/// <returns></returns>
		public FileInfo SaveFile(string FileName, string Description, Stream inputStream)
		{
			return SaveFile(_root.Id, FileName, Description, this.CurrentUserId, DateTime.UtcNow, inputStream);
		}

		/// <summary>
		/// Saves the file.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="FileName">Name of the file.</param>
		/// <param name="inputStream">The input stream.</param>
		/// <returns></returns>
		public FileInfo SaveFile(DirectoryInfo parent, string FileName, Stream inputStream)
		{
			return SaveFile(parent.Id, FileName, string.Empty, this.CurrentUserId, DateTime.UtcNow, inputStream);
		}

		/// <summary>
		/// Saves the file.
		/// </summary>
		/// <param name="parentFolderId">The parent folder id.</param>
		/// <param name="FileName">Name of the file.</param>
		/// <param name="inputStream">The input stream.</param>
		/// <returns></returns>
		public FileInfo SaveFile(int parentFolderId, string FileName, Stream inputStream)
		{
			return SaveFile(parentFolderId, FileName, string.Empty, this.CurrentUserId, DateTime.UtcNow, inputStream);
		}

		/// <summary>
		/// Saves the file.
		/// </summary>
		/// <param name="parentFolderId">The parent folder id.</param>
		/// <param name="FileName">Name of the file.</param>
		/// <param name="Description">The description.</param>
		/// <param name="inputStream">The input stream.</param>
		/// <returns></returns>
		public FileInfo SaveFile(int parentFolderId, string FileName, string Description, Stream inputStream)
		{
			return SaveFile(parentFolderId, FileName, Description, this.CurrentUserId, DateTime.UtcNow, inputStream);
		}


		/// <summary>
		/// Saves the file.
		/// </summary>
		/// <param name="FileName">Name of the file.</param>
		/// <param name="CreatorId">The creator id.</param>
		/// <param name="Created">The created.</param>
		/// <param name="inputStream">The input stream.</param>
		/// <returns></returns>
		public FileInfo SaveFile(string FileName, int CreatorId, DateTime Created, Stream inputStream)
		{
			return SaveFile(_root.Id, FileName, string.Empty, CreatorId, Created, inputStream);
		}

		/// <summary>
		/// Saves the file.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="FileName">Name of the file.</param>
		/// <param name="CreatorId">The creator id.</param>
		/// <param name="Created">The created.</param>
		/// <param name="inputStream">The input stream.</param>
		/// <returns></returns>
		public FileInfo SaveFile(DirectoryInfo parent, string FileName, int CreatorId, DateTime Created, Stream inputStream)
		{
			return SaveFile(parent.Id, FileName, string.Empty, CreatorId, Created, inputStream);
		}

		/// <summary>
		/// Saves the file.
		/// </summary>
		/// <param name="parentFolderId">The parent folder id.</param>
		/// <param name="FileName">Name of the file.</param>
		/// <param name="Description">The description.</param>
		/// <param name="CreatorId">The creator id.</param>
		/// <param name="Created">The created.</param>
		/// <param name="inputStream">The input stream.</param>
		/// <returns></returns>
		public FileInfo SaveFile(int parentFolderId, string fileName, string description, int creatorId, DateTime created, Stream inputStream)
		{
			fileName = GetCleanupFileName(fileName);

			using (Mediachase.IBN.Database.DbTransaction tran = Mediachase.IBN.Database.DbTransaction.Begin())
			{
				FileInfo previousFile = GetFile(parentFolderId, fileName);
				FileInfo info = CreateFile(parentFolderId, fileName, description, creatorId, DateTime.UtcNow);

				byte[] tmpBuffer = new byte[BufferSize];

				using (Stream outputStream = FileOpenWrite(info))
				{
					int count;
					do
					{
						count = inputStream.Read(tmpBuffer, 0, tmpBuffer.Length);
						outputStream.Write(tmpBuffer, 0, count);
					}
					while (count > 0);

					outputStream.Flush();
				}

				DBFile.RecalculateFileSize(info.FileBinaryId);

				// OZ: [2007-09-25] Update Parent Foldet Modified, ModifierId
				ModifyDirectory(parentFolderId, creatorId, info.Created);
				//

				string containerKey = this.OwnerContainer.Key;
				string[] parts = containerKey.Split('_');
				if (parts.Length == 2)
				{
					bool sendAlert = true;
					SystemEventTypes eventType;
					int objectId = int.Parse(parts[1]);
					int relObjectId = info.Id;

					switch (parts[0])
					{
						case "DocumentId":
							if (previousFile != null)
								eventType = SystemEventTypes.Document_Updated_FileList_FileUpdated;
							else
								eventType = SystemEventTypes.Document_Updated_FileList_FileAdded;
							break;
						case "DocumentVers":
							eventType = SystemEventTypes.Document_Updated_VersionList_VersionAdded;
							relObjectId = -1;
							break;
						case "EventId":
							if (previousFile != null)
								eventType = SystemEventTypes.CalendarEntry_Updated_FileList_FileUpdated;
							else
								eventType = SystemEventTypes.CalendarEntry_Updated_FileList_FileAdded;
							break;
						case "IncidentId":
							if (previousFile != null)
								eventType = SystemEventTypes.Issue_Updated_FileList_FileUpdated;
							else
								eventType = SystemEventTypes.Issue_Updated_FileList_FileAdded;
							break;
						case "ProjectId":
							if (previousFile != null)
								eventType = SystemEventTypes.Project_Updated_FileList_FileUpdated;
							else
								eventType = SystemEventTypes.Project_Updated_FileList_FileAdded;
							break;
						case "TaskId":
							if (previousFile != null)
								eventType = SystemEventTypes.Task_Updated_FileList_FileUpdated;
							else
								eventType = SystemEventTypes.Task_Updated_FileList_FileAdded;
							break;
						case "ToDoId":
							if (previousFile != null)
								eventType = SystemEventTypes.Todo_Updated_FileList_FileUpdated;
							else
								eventType = SystemEventTypes.Todo_Updated_FileList_FileAdded;
							break;
						default:
							sendAlert = false;
							eventType = SystemEventTypes.Document_Created;
							break;
					}

					if (sendAlert)
						SystemEvents.AddSystemEvents(eventType, objectId, relObjectId);
				}

				tran.Commit();

				return info;
			}
		}
		#endregion

		#region -- LoadFile --
		public static void LightLoadFile(int FileId, Stream outputStream)
		{
			FileInfo info = null;

			using (IDataReader reader = DBFile.GetById(CurrentTimeZoneId, FileId))
			{
				if (reader.Read())
					info = new FileInfo(reader);
			}
			if (info != null)
			{
				LightLoadFile(info, outputStream);
			}
		}

		public static void LightLoadFile(FileInfo info, Stream outputStream)
		{
			using (Mediachase.IBN.Database.DbTransaction tran = Mediachase.IBN.Database.DbTransaction.Begin())
			{
				DBFile.FileBinaryModifyCounter(info.FileBinaryId);

				byte[] tmpBuffer = new byte[BufferSize];

				using (Stream inputStream = DBFile.OpenRead(info.FileBinaryId))
				{
					int count;
					do
					{
						count = inputStream.Read(tmpBuffer, 0, tmpBuffer.Length);
						outputStream.Write(tmpBuffer, 0, count);
					}
					while (count > 0);

					outputStream.Flush();
				}
				tran.Commit();
			}
		}

		/// <summary>
		/// Loads the file.
		/// </summary>
		/// <param name="FileId">The file id.</param>
		/// <param name="outputStream">The output stream.</param>
		public void LoadFile(int FileId, Stream outputStream)
		{
			FileInfo info = null;

			using (IDataReader reader = DBFile.GetById(CurrentTimeZoneId, FileId))
			{
				if (reader.Read())
					info = new FileInfo(this, reader);
			}
			if (info != null)
			{
				LoadFile(info, outputStream);
			}
		}

		/// <summary>
		/// Loads the file.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="outputStream">The output stream.</param>
		public void LoadFile(FileInfo info, Stream outputStream)
		{
			using (Mediachase.IBN.Database.DbTransaction tran = Mediachase.IBN.Database.DbTransaction.Begin())
			{
				DBFile.FileBinaryModifyCounter(info.FileBinaryId);

				byte[] tmpBuffer = new byte[BufferSize];

				using (Stream inputStream = FileOpenRead(info))
				{
					int count;
					do
					{
						count = inputStream.Read(tmpBuffer, 0, tmpBuffer.Length);
						outputStream.Write(tmpBuffer, 0, count);
					}
					while (count > 0);

					outputStream.Flush();
				}
				tran.Commit();
			}
		}

		/// <summary>
		/// Loads the file.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="outputStream">The output stream.</param>
		public void LoadFile(FileHistoryInfo info, Stream outputStream)
		{
			using (Mediachase.IBN.Database.DbTransaction tran = Mediachase.IBN.Database.DbTransaction.Begin())
			{
				DBFile.FileBinaryModifyCounter(info.FileBinaryId);

				byte[] tmpBuffer = new byte[BufferSize];

				using (Stream inputStream = FileOpenRead(info))
				{
					int count;
					do
					{
						count = inputStream.Read(tmpBuffer, 0, tmpBuffer.Length);
						outputStream.Write(tmpBuffer, 0, count);
					}
					while (count > 0);

					outputStream.Flush();
				}
				tran.Commit();
			}
		}
		#endregion


		#region -- CreateFile --

		/// <summary>
		/// Creates the file.
		/// </summary>
		/// <param name="FileName">Name of the file.</param>
		/// <returns></returns>
		public FileInfo CreateFile(string FileName)
		{
			return CreateFile(_root.Id, FileName, string.Empty, this.CurrentUserId, DateTime.UtcNow);
		}

		/// <summary>
		/// Creates the file.
		/// </summary>
		/// <param name="parentDirectory">The parent directory.</param>
		/// <param name="FileName">Name of the file.</param>
		/// <returns></returns>
		public FileInfo CreateFile(DirectoryInfo parentDirectory, string FileName)
		{
			return CreateFile(parentDirectory.Id, FileName, string.Empty, this.CurrentUserId, DateTime.UtcNow);
		}

		/// <summary>
		/// Creates the file.
		/// </summary>
		/// <param name="parentDirectoryId">The parent directory id.</param>
		/// <param name="FileName">Name of the file.</param>
		/// <returns></returns>
		public FileInfo CreateFile(int parentDirectoryId, string FileName)
		{
			return CreateFile(parentDirectoryId, FileName, string.Empty, this.CurrentUserId, DateTime.UtcNow);
		}

		/// <summary>
		/// Creates the file.
		/// </summary>
		/// <param name="parentDirectory">The parent directory.</param>
		/// <param name="FileName">Name of the file.</param>
		/// <param name="CreatorId">The creator id.</param>
		/// <param name="Created">The created.</param>
		/// <returns></returns>
		public FileInfo CreateFile(DirectoryInfo parentDirectory, string FileName, int CreatorId, DateTime Created)
		{
			return CreateFile(parentDirectory.Id, FileName, string.Empty, CreatorId, Created);
		}

		/// <summary>
		/// Creates the file.
		/// </summary>
		/// <param name="parentDirectoryId">The parent directory id.</param>
		/// <param name="FileName">Name of the file.</param>
		/// <param name="creatorId">The creator id.</param>
		/// <param name="created">The created.</param>
		/// <param name="description">The description.</param>
		/// <returns></returns>
		public FileInfo CreateFile(int parentDirectoryId, string fileName, string description, int creatorId, DateTime created)
		{
			int ContentTypeId = DSFile.GetContentTypeByExtension(Path.GetExtension(fileName));

			using (IDataReader reader = DBFile.Create(CurrentTimeZoneId, fileName, parentDirectoryId, creatorId, created, description))
			{
				if (reader.Read())
					return new FileInfo(this, reader);
				else
					return null;
			}
		}

		#endregion

		#region -- CreateDirectory --

		/// <summary>
		/// Creates the directory.
		/// </summary>
		/// <param name="DirectoryName">Name of the directory.</param>
		/// <returns></returns>
		public DirectoryInfo CreateDirectory(string DirectoryName)
		{
			return CreateDirectory(_root.Id, DirectoryName, this.CurrentUserId, DateTime.UtcNow);
		}

		/// <summary>
		/// Creates the directory.
		/// </summary>
		/// <param name="parentDirectory">The parent directory.</param>
		/// <param name="DirectoryName">Name of the directory.</param>
		/// <returns></returns>
		public DirectoryInfo CreateDirectory(DirectoryInfo parentDirectory, string DirectoryName)
		{
			return CreateDirectory(parentDirectory.Id, DirectoryName, this.CurrentUserId, DateTime.UtcNow);
		}

		/// <summary>
		/// Creates the directory.
		/// </summary>
		/// <param name="parentDirectoryId">The parent directory id.</param>
		/// <param name="DirectoryName">Name of the directory.</param>
		/// <returns></returns>
		public DirectoryInfo CreateDirectory(int parentDirectoryId, string DirectoryName)
		{
			return CreateDirectory(parentDirectoryId, DirectoryName, this.CurrentUserId, DateTime.UtcNow);
		}

		/// <summary>
		/// Creates the directory.
		/// </summary>
		/// <param name="parentDirectory">The parent directory.</param>
		/// <param name="DirectoryName">Name of the directory.</param>
		/// <param name="CreatorId">The creator id.</param>
		/// <param name="Created">The created.</param>
		/// <returns></returns>
		public DirectoryInfo CreateDirectory(DirectoryInfo parentDirectory, string DirectoryName, int CreatorId, DateTime Created)
		{
			return CreateDirectory(parentDirectory.Id, DirectoryName, CreatorId, Created);
		}

		/// <summary>
		/// Creates the directory.
		/// </summary>
		/// <param name="parentDirectoryId">The parent directory id.</param>
		/// <param name="DirectoryName">Name of the directory.</param>
		/// <param name="CreatorId">The creator id.</param>
		/// <param name="Created">The created.</param>
		/// <returns></returns>
		public DirectoryInfo CreateDirectory(int parentDirectoryId, string DirectoryName, int CreatorId, DateTime Created)
		{
			using (IDataReader reader = DBDirectory.Create(CurrentTimeZoneId, _ownerContainer.Key, DirectoryName, parentDirectoryId, CreatorId, Created))
			{
				if (reader.Read())
					return new DirectoryInfo(this, reader);
				else
					return null;
			}
		}

		#endregion

		#region -- FileOpenRead --
		/// <summary>
		/// Files the open read.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <returns></returns>
		public Stream FileOpenRead(FileHistoryInfo info)
		{
			return DBFile.OpenRead(info.FileBinaryId);
		}

		/// <summary>
		/// Files the open read.
		/// </summary>
		/// <param name="FileId">The file id.</param>
		/// <returns></returns>
		public Stream FileOpenRead(int FileId)
		{
			FileInfo info = null;
			using (IDataReader reader = DBFile.GetById(CurrentTimeZoneId, FileId))
			{
				if (reader.Read())
					info = new FileInfo(this, reader);
				else
					return null;
			}
			return FileOpenRead(info);
		}

		/// <summary>
		/// Files the open read.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <returns></returns>
		public Stream FileOpenRead(FileInfo info)
		{
			return DBFile.OpenRead(info.FileBinaryId);
		}
		#endregion

		#region -- FileOpenWrite --
		/// <summary>
		/// Files the open write.
		/// </summary>
		/// <param name="FileId">The file id.</param>
		/// <returns></returns>
		public Stream FileOpenWrite(int FileId)
		{
			FileInfo info = null;
			using (IDataReader reader = DBFile.GetById(CurrentTimeZoneId, FileId))
			{
				if (reader.Read())
					info = new FileInfo(this, reader);
				else
					return null;
			}
			return FileOpenWrite(info);
		}

		/// <summary>
		/// Files the open write.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <returns></returns>
		public Stream FileOpenWrite(FileInfo info)
		{
			ModifyFile(info);

			info.FileBinaryId = DBFile.CreateBinary(
				info.Id,
				DSFile.GetContentTypeByFileName(info.Name));

			return DBFile.OpenWrite(info.FileBinaryId);
		}
		#endregion

		#region -- RenameFile --
		/// <summary>
		/// Renames the file.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="Name">The name.</param>
		/// <returns></returns>
		public FileInfo RenameFile(FileInfo info, string Name)
		{
			return RenameFile(info.Id, Name);
		}

		public FileInfo RenameFile(int FileId, string Name)
		{
			return RenameFile(FileId, Name, null);
		}

		/// <summary>
		/// Renames the file.
		/// </summary>
		/// <param name="FileId">The file id.</param>
		/// <param name="Name">The name.</param>
		/// <returns></returns>
		public FileInfo RenameFile(int FileId, string Name, string description)
		{
			FileInfo retVal = null;

			if (description != null)
				UpdateDescription(FileId, description);

			retVal = GetFile(FileId);
			if (retVal.Name != Name)
			{
				Name = GetCleanupFileName(Name);
				using (IDataReader reader = DBFile.Rename(CurrentTimeZoneId, FileId, Name, this.CurrentUserId, DateTime.UtcNow))
				{
					if (reader.Read())
						retVal = new FileInfo(this, reader);
				}
			}

			return retVal;
		}

		public static void UpdateDescription(int FileId, string Description)
		{
			DBFile.UpdateDescription(FileId, Description);
		}
		#endregion

		#region -- RenameDirectory --
		/// <summary>
		/// Renames the directory.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="Name">The name.</param>
		/// <returns></returns>
		public DirectoryInfo RenameDirectory(DirectoryInfo info, string Name)
		{
			return RenameDirectory(info.Id, Name);
		}

		/// <summary>
		/// Renames the directory.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <param name="Name">The name.</param>
		/// <returns></returns>
		public DirectoryInfo RenameDirectory(int DirectoryId, string Name)
		{
			using (IDataReader reader = DBDirectory.Rename(CurrentTimeZoneId, DirectoryId, Name, this.CurrentUserId, DateTime.UtcNow))
			{
				if (reader.Read())
					return new DirectoryInfo(this, reader);
				else
					return null;
			}
		}
		#endregion

		#region -- ModifyFile --
		/// <summary>
		/// Modifies the file.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <returns></returns>
		public FileInfo ModifyFile(FileInfo info)
		{
			return ModifyFile(info.Id, this.CurrentUserId, DateTime.UtcNow);
		}

		/// <summary>
		/// Modifies the file.
		/// </summary>
		/// <param name="FileId">The file id.</param>
		/// <returns></returns>
		public FileInfo ModifyFile(int FileId)
		{
			return ModifyFile(FileId, this.CurrentUserId, DateTime.UtcNow);
		}

		/// <summary>
		/// Modifies the file.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="UserId">The user id.</param>
		/// <param name="modified">The modified.</param>
		/// <returns></returns>
		public FileInfo ModifyFile(FileInfo info, int UserId, DateTime modified)
		{
			return ModifyFile(info.Id, UserId, modified);
		}

		/// <summary>
		/// Modifies the file.
		/// </summary>
		/// <param name="FileId">The file id.</param>
		/// <param name="UserId">The user id.</param>
		/// <param name="modified">The modified.</param>
		/// <returns></returns>
		public FileInfo ModifyFile(int FileId, int UserId, DateTime modified)
		{
			using (IDataReader reader = DBFile.Modify(CurrentTimeZoneId, FileId, UserId, modified))
			{
				if (reader.Read())
					return new FileInfo(this, reader);
				else
					return null;
			}
		}
		#endregion

		#region -- ModifyDirectory --
		/// <summary>
		/// Modifies the directory.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="UserId">The user id.</param>
		/// <param name="modified">The modified.</param>
		/// <returns></returns>
		public DirectoryInfo ModifyDirectory(DirectoryInfo info, int UserId, DateTime modified)
		{
			return ModifyDirectory(info.Id, UserId, modified);
		}

		/// <summary>
		/// Modifies the directory.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <param name="UserId">The user id.</param>
		/// <param name="modified">The modified.</param>
		/// <returns></returns>
		public DirectoryInfo ModifyDirectory(int DirectoryId, int UserId, DateTime modified)
		{
			using (IDataReader reader = DBDirectory.Modify(CurrentTimeZoneId, DirectoryId, UserId, modified))
			{
				if (reader.Read())
					return new DirectoryInfo(this, reader);
				else
					return null;
			}
		}
		#endregion

		#region -- GetDirectories --
		/// <summary>
		/// Gets the directories.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <returns></returns>
		public DirectoryInfo[] GetDirectories(DirectoryInfo parent)
		{
			return GetDirectories(parent.Id);
		}

		/// <summary>
		/// Gets the directories.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <returns></returns>
		public DirectoryInfo[] GetDirectories(int DirectoryId)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DBDirectory.GetChildDirectoriesByParent(CurrentTimeZoneId, DirectoryId, _ownerContainer.Key))
			{
				while (reader.Read())
				{
					list.Add(new DirectoryInfo(this, reader));
				}
			}
			return (DirectoryInfo[])list.ToArray(typeof(DirectoryInfo));
		}
		#endregion

		#region -- SearchFiles --
		// ContainerKey, FileId, [Name], DirectoryId, FileBinaryId, CreatorId, Created, ModifierId, Modified, Length, ContentTypeString, AllowHistory
		/// <summary>
		/// Searches the files.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <param name="ContainerKey">The container key.</param>
		/// <param name="DirectoryId">The directory id.</param>
		/// <param name="Deep">if set to <c>true</c> [deep].</param>
		/// <param name="Keyword">The keyword.</param>
		/// <param name="ContentTypeId">The content type id.</param>
		/// <param name="modifiedFrom">The modified from.</param>
		/// <param name="modifiedTo">The modified to.</param>
		/// <param name="lengthFrom">The length from.</param>
		/// <param name="lengthTo">The length to.</param>
		/// <returns></returns>
		public static FileInfo[] SearchFiles(int UserId, string ContainerKey,
			int DirectoryId, bool Deep,
			string Keyword,
			int ContentTypeId,
			DateTime modifiedFrom,
			DateTime modifiedTo,
			int lengthFrom,
			int lengthTo)
		{
			ArrayList retVal = new ArrayList();

			bool checkCanRead = false;

			try
			{
				using (IDataReader reader = DBFile.Search(CurrentTimeZoneId, UserId,
						  ContainerKey,
						  DirectoryId <= 0 ? null : (object)DirectoryId,
						  Deep,
						  Keyword,
						  ContentTypeId <= 0 ? null : (object)ContentTypeId,
						  modifiedFrom == DateTime.MinValue ? null : (object)modifiedFrom,
						  modifiedTo == DateTime.MinValue ? null : (object)modifiedTo,
						  lengthFrom <= 0 ? null : (object)lengthFrom,
						  lengthTo <= 0 ? null : (object)lengthTo))
				{
					while (reader.Read())
					{
						FileInfo fileInfo = new FileInfo(reader);

						// OZ: 2008-08-18 - Fix Visible Incident Or Document Files
						if (!checkCanRead)
						{
							checkCanRead = fileInfo.ContainerKey.StartsWith("ForumNodeId_") ||
								fileInfo.ContainerKey.StartsWith("DocumentVers_");
						}
						//

						retVal.Add(fileInfo);
					}
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				// OZ: Ignore Execution of a full-text operation failed. A clause of the query contained only ignored words. 
				if (ex.Number != 7619)
					throw;
			}

			// OZ: 2008-08-18 - Fix Visible Incident Or Document Files
			if (checkCanRead)
				FixIncidentOrDocument(retVal);
			//

			return (FileInfo[])retVal.ToArray(typeof(FileInfo));
		}

		#endregion


		#region -- FilesSearch --
		// ContainerKey, FileId, [Name], DirectoryId, FileBinaryId, CreatorId, Created, ModifierId, Modified, Length, ContentTypeString, AllowHistory
		/// <summary>
		/// Searches the files.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <param name="ProjectId">The project id.</param>
		/// <param name="ObjectTypeId">The object type id.</param>
		/// <param name="Keyword">The keyword.</param>
		/// <param name="ContentTypeId">The content type id.</param>
		/// <param name="modifiedFrom">The modified from.</param>
		/// <param name="modifiedTo">The modified to.</param>
		/// <param name="lengthFrom">The length from.</param>
		/// <param name="lengthTo">The length to.</param>
		/// <returns></returns>
		public static FileInfo[] SearchFiles(int UserId,
			int ProjectId,
			int ObjectTypeId,
			string Keyword,
			int ContentTypeId,
			DateTime modifiedFrom,
			DateTime modifiedTo,
			int lengthFrom,
			int lengthTo)
		{
			ArrayList retVal = new ArrayList();

			bool checkCanRead = false;

			try
			{
				using (IDataReader reader = DBFileStorage.Search(UserId,
				  ProjectId <= 0 ? null : (object)ProjectId,
				  ObjectTypeId <= 0 ? null : (object)ObjectTypeId,
				  null,
				  Keyword,
				  ContentTypeId <= 0 ? null : (object)ContentTypeId,
				  modifiedFrom == DateTime.MinValue ? null : (object)modifiedFrom,
				  modifiedTo == DateTime.MinValue ? null : (object)modifiedTo,
				  lengthFrom <= 0 ? null : (object)lengthFrom,
				  lengthTo <= 0 ? null : (object)lengthTo))
				{
					while (reader.Read())
					{
						FileInfo fileInfo = new FileInfo(reader);

						// OZ: 2008-08-18 - Fix Visible Incident Or Document Files
						if (!checkCanRead)
						{
							checkCanRead = fileInfo.ContainerKey.StartsWith("ForumNodeId_") || 
								fileInfo.ContainerKey.StartsWith("DocumentVers_");
						}
						//


						retVal.Add(fileInfo);
					}
				}

			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				// OZ: Ignore // Ignore Execution of a full-text operation failed. A clause of the query contained only ignored words. 
				if (ex.Number != 7619)
					throw;
			}

			// OZ: 2008-08-18 - Fix Visible Incident Or Document Files
			if (checkCanRead)
				FixIncidentOrDocument(retVal);
			//

			return (FileInfo[])retVal.ToArray(typeof(FileInfo));
		}

		/// <summary>
		/// Fixes the incident or document.
		/// </summary>
		/// <param name="retVal">The ret val.</param>
		private static void FixIncidentOrDocument(ArrayList retVal)
		{
			for (int index = retVal.Count - 1; index >= 0; index--)
			{
				FileInfo fileInfo = (FileInfo)retVal[index];

				if (!FixCanRead(fileInfo))
					retVal.RemoveAt(index);
			}
		}

		/// <summary>
		/// Fixes the can read.
		/// </summary>
		/// <param name="fileInfo">The file info.</param>
		/// <returns></returns>
		private static bool FixCanRead(FileInfo fileInfo)
		{
			if (fileInfo.ContainerKey.StartsWith("ForumNodeId_"))
			{
				// Extract forumNodeId
				int forumNodeId = int.Parse(fileInfo.ContainerKey.Split('_')[1]);

				// Find incidentId by ForumNodeId
				string forumContainerKey = ForumThreadNodeInfo.GetOwnerContainerKey(forumNodeId);

				if (forumContainerKey == null)
					return false;

				int incidentId = int.Parse(forumContainerKey.Split('_')[1]);

				// Check Security
				return Incident.CanRead(incidentId);
			}
			else if (fileInfo.ContainerKey.StartsWith("DocumentVers_"))
			{
				// Extract documentVersionId
				int documentId = int.Parse(fileInfo.ContainerKey.Split('_')[1]);

				// Check Security
				return Document.CanRead(documentId);
			}

			return true;
		}
		#endregion

		#region -- GetFiles --
		/// <summary>
		/// Gets the files.
		/// </summary>
		/// <returns></returns>
		public FileInfo[] GetFiles()
		{
			return GetFiles(this.Root.Id);
		}

		/// <summary>
		/// Gets the files.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <returns></returns>
		public FileInfo[] GetFiles(DirectoryInfo parent)
		{
			return GetFiles(parent.Id);
		}

		/// <summary>
		/// Gets the files.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <returns></returns>
		public FileInfo[] GetFiles(int DirectoryId)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DBDirectory.GetChildFilesByDirectoryId(CurrentTimeZoneId, DirectoryId, _ownerContainer.Key))
			{
				while (reader.Read())
				{
					list.Add(new FileInfo(this, reader));
				}
			}
			return (FileInfo[])list.ToArray(typeof(FileInfo));
		}


		/// <summary>
		/// Gets the files.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="Deep">if set to <c>true</c> [deep].</param>
		/// <param name="Keyword">The keyword.</param>
		/// <param name="ContentTypeId">The content type id.</param>
		/// <param name="modifiedFrom">The modified from.</param>
		/// <param name="modifiedTo">The modified to.</param>
		/// <param name="lengthFrom">The length from.</param>
		/// <param name="lengthTo">The length to.</param>
		/// <returns></returns>
		public FileInfo[] GetFiles(DirectoryInfo parent, bool Deep,
			string Keyword,
			int ContentTypeId,
			DateTime modifiedFrom,
			DateTime modifiedTo,
			int lengthFrom,
			int lengthTo)
		{
			return GetFiles(parent.Id, Deep, Keyword, ContentTypeId, modifiedFrom, modifiedTo, lengthFrom, lengthTo);
		}

		/// <summary>
		/// Gets the files.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <param name="Deep">if set to <c>true</c> [deep].</param>
		/// <param name="Keyword">The keyword.</param>
		/// <param name="ContentTypeId">The content type id.</param>
		/// <param name="modifiedFrom">The modified from.</param>
		/// <param name="modifiedTo">The modified to.</param>
		/// <param name="lengthFrom">The length from.</param>
		/// <param name="lengthTo">The length to.</param>
		/// <returns></returns>
		public FileInfo[] GetFiles(int DirectoryId, bool Deep,
			string Keyword,
			int ContentTypeId,
			DateTime modifiedFrom,
			DateTime modifiedTo,
			int lengthFrom,
			int lengthTo)
		{
			return FileStorage.SearchFiles(this.CurrentUserId,
					   _ownerContainer.Key,
					   DirectoryId, Deep,
					   Keyword,
					   ContentTypeId,
					   modifiedFrom,
					   modifiedTo,
					   lengthFrom,
					   lengthTo);
		}

		#endregion

		#region -- GetFile --
		/// <summary>
		/// Gets the file.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="Name">The name.</param>
		/// <returns></returns>
		public FileInfo GetFile(DirectoryInfo info, string Name)
		{
			return GetFile(info.Id, Name);
		}

		/// <summary>
		/// Gets the file.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <param name="Name">The name.</param>
		/// <returns></returns>
		public FileInfo GetFile(int DirectoryId, string Name)
		{
			using (IDataReader reader = DBFile.GetByName(CurrentTimeZoneId, Name, DirectoryId, _ownerContainer.Key))
			{
				if (reader.Read())
					return new FileInfo(this, reader);
				else
					return null;
			}
		}

		/// <summary>
		/// Gets the file.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <param name="Action">The action.</param>
		/// <param name="FileId">The file id.</param>
		/// <returns></returns>
		public static FileInfo GetFile(int UserId, string Action, int FileId)
		{
			FileInfo retVal = null;

			using (IDataReader reader = DBFile.GetById(CurrentTimeZoneId, FileId))
			{
				if (reader.Read())
				{
					retVal = new FileInfo(reader);
				}
			}

			if (!FileStorage.CanUserRunAction(UserId, retVal.ContainerKey, retVal.ParentDirectoryId, Action))
			{
				throw new AccessDeniedException();
			}

			return retVal;
		}

		/// <summary>
		/// Gets the file.
		/// </summary>
		/// <param name="FileId">The file id.</param>
		/// <returns></returns>
		public FileInfo GetFile(int FileId)
		{
			using (IDataReader reader = DBFile.GetById(CurrentTimeZoneId, FileId))
			{
				if (reader.Read())
					return new FileInfo(this, reader);
				else
					return null;
			}
		}
		#endregion

		#region -- GetDirectory --
		/// <summary>
		/// Gets the directory.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <returns></returns>
		public DirectoryInfo GetDirectory(int DirectoryId)
		{
			using (IDataReader reader = DBDirectory.GetById(CurrentTimeZoneId, DirectoryId))
			{
				if (reader.Read())
				{
					// OZ: Check ContainerKey
					if(((string)reader["ContainerKey"]) == this._ownerContainer.Key)
					{
						return new DirectoryInfo(this, reader);
					}
				}
			}

			return null;
		}

		internal static DirectoryInfo InnerGetDirectory(int DirectoryId)
		{
			using (IDataReader reader = DBDirectory.GetById(CurrentTimeZoneId, DirectoryId))
			{
				if (reader.Read())
					return new DirectoryInfo(null, reader);
				else
					return null;
			}
		}

		/// <summary>
		/// Gets the directory.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		public DirectoryInfo GetDirectory(int DirectoryId, string path)
		{
			return GetDirectory(DirectoryId, path, false);
		}

		/// <summary>
		/// Gets the directory.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		public DirectoryInfo GetDirectory(DirectoryInfo info, string path)
		{
			return GetDirectory(info, path, false);
		}

		/// <summary>
		/// Gets the directory.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <param name="path">The path.</param>
		/// <param name="bCreateIfNotExists">if set to <c>true</c> [b create if not exists].</param>
		/// <returns></returns>
		public DirectoryInfo GetDirectory(int DirectoryId, string path, bool bCreateIfNotExists)
		{
			return GetDirectory(GetDirectory(DirectoryId), path, bCreateIfNotExists);
		}

		/// <summary>
		/// Gets the directory.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="path">The path.</param>
		/// <param name="bCreateIfNotExists">if set to <c>true</c> [b create if not exists].</param>
		/// <returns></returns>
		public DirectoryInfo GetDirectory(DirectoryInfo info, string path, bool bCreateIfNotExists)
		{
			if (path.IndexOf('\\') != -1)
			{
				info = GetDirectory(info, path.Substring(0, path.LastIndexOf('\\')), bCreateIfNotExists);
				path = path.Substring(path.LastIndexOf('\\') + 1);
			}
			if (info == null)
				return null;

			using (IDataReader reader = DBDirectory.GetByName(CurrentTimeZoneId, path, info.Id, _ownerContainer.Key))
			{
				if (reader.Read())
				{
					return new DirectoryInfo(this, reader);
				}
			}
			return bCreateIfNotExists ? CreateDirectory(info, path) : null;
		}
		#endregion

		#region -- GetParentDirectory --
		/// <summary>
		/// Gets the parent directory.
		/// </summary>
		/// <param name="dirInfo">The dir info.</param>
		/// <returns></returns>
		public DirectoryInfo GetParentDirectory(DirectoryInfo dirInfo)
		{
			return GetParentDirectory(dirInfo.Id);
		}

		/// <summary>
		/// Gets the parent directory.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <returns></returns>
		public DirectoryInfo GetParentDirectory(int DirectoryId)
		{
			using (IDataReader reader = DBDirectory.GetParentById(CurrentTimeZoneId, DirectoryId))
			{
				if (reader.Read())
					return new DirectoryInfo(this, reader);
				else
					return null;
			}
		}
		#endregion

		#region -- FileExist --
		/// <summary>
		/// Files the exist.
		/// </summary>
		/// <param name="fileInfo">The file info.</param>
		/// <param name="parentInfo">The parent info.</param>
		/// <returns></returns>
		public bool FileExist(FileInfo fileInfo, DirectoryInfo parentInfo)
		{
			return FileExist(fileInfo.Name, parentInfo.ParentDirectoryId);
		}

		/// <summary>
		/// Files the exist.
		/// </summary>
		/// <param name="Name">The name.</param>
		/// <param name="DirectoryId">The directory id.</param>
		/// <returns></returns>
		public bool FileExist(string Name, int DirectoryId)
		{
			using (IDataReader reader = DBFile.GetByName(CurrentTimeZoneId, Name, DirectoryId, _ownerContainer.Key))
			{
				return reader.Read();
			}
		}
		#endregion

		#region -- DirectoryExist --
		/// <summary>
		/// Directories the exist.
		/// </summary>
		/// <param name="dirInfo">The dir info.</param>
		/// <param name="parentInfo">The parent info.</param>
		/// <returns></returns>
		public bool DirectoryExist(DirectoryInfo dirInfo, DirectoryInfo parentInfo)
		{
			return DirectoryExist(dirInfo.Name, parentInfo.Id);
		}

		/// <summary>
		/// Directories the exist.
		/// </summary>
		/// <param name="Name">The name.</param>
		/// <param name="ParentDirectoryId">The parent directory id.</param>
		/// <returns></returns>
		public bool DirectoryExist(string Name, int ParentDirectoryId)
		{
			using (IDataReader reader = DBDirectory.GetByName(CurrentTimeZoneId, Name, ParentDirectoryId, _ownerContainer.Key))
			{
				return reader.Read();
			}
		}
		#endregion

		#region -- MoveFile --
		/// <summary>
		/// Moves the file.
		/// </summary>
		/// <param name="fileInfo">The file info.</param>
		/// <param name="newDirectoryInfo">The new directory info.</param>
		/// <returns></returns>
		public FileInfo MoveFile(FileInfo fileInfo, DirectoryInfo newDirectoryInfo)
		{
			return MoveFile(fileInfo.Id, newDirectoryInfo.Id);
		}

		/// <summary>
		/// Moves the file.
		/// </summary>
		/// <param name="fileInfo">The file info.</param>
		/// <param name="newDirectoryInfo">The new directory info.</param>
		/// <param name="Modifier">The modifier.</param>
		/// <param name="Modified">The modified.</param>
		/// <returns></returns>
		public FileInfo MoveFile(FileInfo fileInfo, DirectoryInfo newDirectoryInfo, int Modifier, DateTime Modified)
		{
			return MoveFile(fileInfo.Id, newDirectoryInfo.Id, Modifier, Modified);
		}

		/// <summary>
		/// Moves the file.
		/// </summary>
		/// <param name="FileId">The file id.</param>
		/// <param name="NewDirectoryId">The new directory id.</param>
		/// <returns></returns>
		public FileInfo MoveFile(int FileId, int NewDirectoryId)
		{
			return MoveFile(FileId, NewDirectoryId, this.CurrentUserId, DateTime.UtcNow);
		}


		/// <summary>
		/// Moves the file.
		/// </summary>
		/// <param name="FileId">The file id.</param>
		/// <param name="NewDirectoryId">The new directory id.</param>
		/// <param name="Modifier">The modifier.</param>
		/// <param name="Modified">The modified.</param>
		/// <returns></returns>
		public FileInfo MoveFile(int FileId, int NewDirectoryId, int Modifier, DateTime Modified)
		{
			ModifyFile(FileId, Modifier, Modified);

			using (IDataReader reader = DBFile.Move(CurrentTimeZoneId, FileId, NewDirectoryId))
			{
				if (reader.Read())
					return new FileInfo(this, reader);
				else
					return null;
			}
		}

		#endregion

		#region -- CopyFile --
		/// <summary>
		/// Copies the file.
		/// </summary>
		/// <param name="fileInfo">The file info.</param>
		/// <param name="newDirectoryInfo">The new directory info.</param>
		/// <returns></returns>
		public FileInfo CopyFile(FileInfo fileInfo, DirectoryInfo newDirectoryInfo)
		{
			return CopyFile(fileInfo.Id, newDirectoryInfo.Id, false);
		}

		/// <summary>
		/// Copies the file.
		/// </summary>
		/// <param name="fileInfo">The file info.</param>
		/// <param name="newDirectoryInfo">The new directory info.</param>
		/// <param name="OverwriteExisting">if set to <c>true</c> [overwrite existing].</param>
		/// <returns></returns>
		public FileInfo CopyFile(FileInfo fileInfo, DirectoryInfo newDirectoryInfo, bool OverwriteExisting)
		{
			return CopyFile(fileInfo.Id, newDirectoryInfo.Id, OverwriteExisting);
		}

		/// <summary>
		/// Copies the file.
		/// </summary>
		/// <param name="fileInfo">The file info.</param>
		/// <param name="newDirectoryInfo">The new directory info.</param>
		/// <param name="Modifier">The modifier.</param>
		/// <param name="Modified">The modified.</param>
		/// <returns></returns>
		public FileInfo CopyFile(FileInfo fileInfo, DirectoryInfo newDirectoryInfo, int Modifier, DateTime Modified)
		{
			return CopyFile(fileInfo.Id, newDirectoryInfo.Id, Modifier, Modified, false);
		}

		/// <summary>
		/// Copies the file.
		/// </summary>
		/// <param name="FileId">The file id.</param>
		/// <param name="NewDirectoryId">The new directory id.</param>
		/// <returns></returns>
		public FileInfo CopyFile(int FileId, int NewDirectoryId)
		{
			return CopyFile(FileId, NewDirectoryId, this.CurrentUserId, DateTime.UtcNow, false);
		}

		/// <summary>
		/// Copies the file.
		/// </summary>
		/// <param name="FileId">The file id.</param>
		/// <param name="NewDirectoryId">The new directory id.</param>
		/// <param name="OverwriteExisting">if set to <c>true</c> [overwrite existing].</param>
		/// <returns></returns>
		public FileInfo CopyFile(int FileId, int NewDirectoryId, bool OverwriteExisting)
		{
			return CopyFile(FileId, NewDirectoryId, this.CurrentUserId, DateTime.UtcNow, OverwriteExisting);
		}


		/// <summary>
		/// Copies the file.
		/// </summary>
		/// <param name="FileId">The file id.</param>
		/// <param name="NewDirectoryId">The new directory id.</param>
		/// <param name="Modifier">The modifier.</param>
		/// <param name="Modified">The modified.</param>
		/// <returns></returns>
		public FileInfo CopyFile(int FileId, int NewDirectoryId, int Modifier, DateTime Modified)
		{
			return CopyFile(FileId, NewDirectoryId, Modifier, Modified, false);
		}

		/// <summary>
		/// Copies the file.
		/// </summary>
		/// <param name="FileId">The file id.</param>
		/// <param name="NewDirectoryId">The new directory id.</param>
		/// <param name="Modifier">The modifier.</param>
		/// <param name="Modified">The modified.</param>
		/// <param name="OverwriteExisting">if set to <c>true</c> [overwrite existing].</param>
		/// <returns></returns>
		public FileInfo CopyFile(int FileId, int NewDirectoryId, int Modifier, DateTime Modified, bool OverwriteExisting)
		{
			int NewFileId = DBFile.Copy(FileId, NewDirectoryId, OverwriteExisting);
			ModifyFile(NewFileId, Modifier, Modified);
			return this.GetFile(FileId);
		}
		#endregion

		#region -- DirectoryMove --
		/// <summary>
		/// Moves the directory.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="NewParentDirectoryInfo">The new parent directory info.</param>
		/// <returns></returns>
		public DirectoryInfo MoveDirectory(DirectoryInfo info, DirectoryInfo NewParentDirectoryInfo)
		{
			return MoveDirectory(info.Id, NewParentDirectoryInfo.Id);
		}

		/// <summary>
		/// Moves the directory.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="NewParentDirectoryInfo">The new parent directory info.</param>
		/// <param name="Modifier">The modifier.</param>
		/// <param name="Modified">The modified.</param>
		/// <returns></returns>
		public DirectoryInfo MoveDirectory(DirectoryInfo info, DirectoryInfo NewParentDirectoryInfo, int Modifier, DateTime Modified)
		{
			return MoveDirectory(info.Id, NewParentDirectoryInfo.Id, Modifier, Modified);
		}

		/// <summary>
		/// Moves the directory.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <param name="NewParentDirectoryId">The new parent directory id.</param>
		/// <returns></returns>
		public DirectoryInfo MoveDirectory(int DirectoryId, int NewParentDirectoryId)
		{
			return MoveDirectory(DirectoryId, NewParentDirectoryId, this.CurrentUserId, DateTime.UtcNow);
		}

		/// <summary>
		/// Moves the directory.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <param name="NewParentDirectoryId">The new parent directory id.</param>
		/// <param name="Modifier">The modifier.</param>
		/// <param name="Modified">The modified.</param>
		/// <returns></returns>
		public DirectoryInfo MoveDirectory(int DirectoryId, int NewParentDirectoryId, int Modifier, DateTime Modified)
		{
			if (DirectoryId == NewParentDirectoryId)
				throw new ArgumentException("DirectoryId == NewParentDirectoryId");

			ModifyDirectory(DirectoryId, Modifier, Modified);

			using (IDataReader reader = DBDirectory.Move(CurrentTimeZoneId, DirectoryId, NewParentDirectoryId))
			{
				if (reader.Read())
					return new DirectoryInfo(this, reader);
				else
					return null;
			}
		}
		#endregion

		#region -- CanUserRunAction --
		/// <summary>
		/// Determines whether this instance [can user run action] the specified user id.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <param name="ContainerKey">The container key.</param>
		/// <param name="DirectoryId">The directory id.</param>
		/// <param name="Action">The action.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user run action] the specified user id; otherwise, <c>false</c>.
		/// </returns>
		public static bool CanUserRunAction(int UserId, string ContainerKey, int DirectoryId, string Action)
		{
			if(ContainerKey==null)
				throw new ArgumentNullException("ContainerKey");
			if (Action == null)
				throw new ArgumentNullException("Action");

			// [2008-01-29] OR: Security fix
			if (Security.IsUserInGroup(UserId, InternalSecureGroups.Administrator))
				return true;

			string key = string.Format(CultureInfo.InvariantCulture, 
				"FileStorage_CanUserRunAction_{0}_{1}_{2}_{3}",
				UserId, ContainerKey, DirectoryId, Action);

			// [2008-06-27] Cache Result to HttpContext
			if (HttpContext.Current != null && HttpContext.Current.Items.Contains(key))
				return (bool)HttpContext.Current.Items[key];

			// [2009-06-06] Document ReadOnly
			if (ContainerKey.StartsWith("DocumentId_"))
			{
				int documentId = int.Parse(ContainerKey.Substring("DocumentId_".Length));

				bool isReadOnly = Document.IsReadOnly(documentId);

				if (isReadOnly)
				{
					if (Action == "Read")
					{
						if (HttpContext.Current != null)
							HttpContext.Current.Items.Add(key, true);

						return true;
					}
					else
					{
						if (HttpContext.Current != null)
							HttpContext.Current.Items.Add(key, false);

						return false;
					}
				}
			}
			//

			// Default Code
			bool retVal = DBFileStorage.CanUserRunAction(UserId, ContainerKey, DirectoryId, Action);

			if (HttpContext.Current != null)
				HttpContext.Current.Items.Add(key, retVal);

			return retVal;
		}

		/// <summary>
		/// Determines whether this instance [can user run action] the specified user id.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <param name="info">The info.</param>
		/// <param name="Action">The action.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user run action] the specified user id; otherwise, <c>false</c>.
		/// </returns>
		public bool CanUserRunAction(int UserId, DirectoryInfo info, string Action)
		{
			return CanUserRunAction(UserId, info.Id, Action);
		}

		/// <summary>
		/// Determines whether this instance [can user run action] the specified user id.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <param name="DirectoryId">The directory id.</param>
		/// <param name="Action">The action.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user run action] the specified user id; otherwise, <c>false</c>.
		/// </returns>
		public bool CanUserRunAction(int UserId, int DirectoryId, string Action)
		{
			// [2008-01-29] OR: Security fix
			//return Security.IsUserInGroup(InternalSecureGroups.Administrator)
			//	|| DBFileStorage.CanUserRunAction(UserId, _ownerContainer.Key, DirectoryId, Action);
			return FileStorage.CanUserRunAction(UserId, _ownerContainer.Key, DirectoryId, Action);
		}

		/// <summary>
		/// Determines whether this instance [can user read] the specified user id.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <param name="ContainerKey">The container key.</param>
		/// <param name="DirectoryId">The directory id.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user read] the specified user id; otherwise, <c>false</c>.
		/// </returns>
		public static bool CanUserRead(int UserId, string ContainerKey, int DirectoryId)
		{
			return CanUserRunAction(UserId, ContainerKey, DirectoryId, "Read");
		}

		/// <summary>
		/// Determines whether this instance [can user read] the specified user id.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <param name="info">The info.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user read] the specified user id; otherwise, <c>false</c>.
		/// </returns>
		public bool CanUserRead(int UserId, DirectoryInfo info)
		{
			return CanUserRead(UserId, info.Id);
		}

		/// <summary>
		/// Determines whether this instance [can user read] the specified user id.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <param name="DirectoryId">The directory id.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user read] the specified user id; otherwise, <c>false</c>.
		/// </returns>
		public bool CanUserRead(int UserId, int DirectoryId)
		{
			return CanUserRunAction(UserId, DirectoryId, "Read");
		}

		/// <summary>
		/// Determines whether this instance [can user write] the specified user id.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <param name="ContainerKey">The container key.</param>
		/// <param name="DirectoryId">The directory id.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user write] the specified user id; otherwise, <c>false</c>.
		/// </returns>
		public static bool CanUserWrite(int UserId, string ContainerKey, int DirectoryId)
		{
			return CanUserRunAction(UserId, ContainerKey, DirectoryId, "Write");
		}

		/// <summary>
		/// Determines whether this instance [can user write] the specified user id.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <param name="info">The info.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user write] the specified user id; otherwise, <c>false</c>.
		/// </returns>
		public bool CanUserWrite(int UserId, DirectoryInfo info)
		{
			return CanUserWrite(UserId, info.Id);
		}

		/// <summary>
		/// Determines whether this instance [can user write] the specified user id.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <param name="DirectoryId">The directory id.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user write] the specified user id; otherwise, <c>false</c>.
		/// </returns>
		public bool CanUserWrite(int UserId, int DirectoryId)
		{
			return CanUserRunAction(UserId, DirectoryId, "Write");
		}

		/// <summary>
		/// Determines whether this instance [can user admin] the specified user id.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <param name="ContainerKey">The container key.</param>
		/// <param name="DirectoryId">The directory id.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user admin] the specified user id; otherwise, <c>false</c>.
		/// </returns>
		public static bool CanUserAdmin(int UserId, string ContainerKey, int DirectoryId)
		{
			return CanUserRunAction(UserId, ContainerKey, DirectoryId, "Admin");
		}

		/// <summary>
		/// Determines whether this instance [can user admin] the specified user id.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <param name="info">The info.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user admin] the specified user id; otherwise, <c>false</c>.
		/// </returns>
		public bool CanUserAdmin(int UserId, DirectoryInfo info)
		{
			return CanUserAdmin(UserId, info.Id);
		}

		/// <summary>
		/// Determines whether this instance [can user admin] the specified user id.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <param name="DirectoryId">The directory id.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user admin] the specified user id; otherwise, <c>false</c>.
		/// </returns>
		public bool CanUserAdmin(int UserId, int DirectoryId)
		{
			return CanUserRunAction(UserId, DirectoryId, "Admin");
		}
		#endregion

		#region -- Current User CanUserRunAction --
		/// <summary>
		/// Determines whether this instance [can user run action] the specified info.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="Action">The action.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user run action] the specified info; otherwise, <c>false</c>.
		/// </returns>
		public bool CanUserRunAction(DirectoryInfo info, string Action)
		{
			return CanUserRunAction(this.CurrentUserId, info.Id, Action);
		}

		/// <summary>
		/// Determines whether this instance [can user run action] the specified directory id.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <param name="Action">The action.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user run action] the specified directory id; otherwise, <c>false</c>.
		/// </returns>
		public bool CanUserRunAction(int DirectoryId, string Action)
		{
			// [2008-01-29] OR: Security fix
			//return Security.IsUserInGroup(InternalSecureGroups.Administrator)
			//    || DBFileStorage.CanUserRunAction(this.CurrentUserId, _ownerContainer.Key, DirectoryId, Action);

			return FileStorage.CanUserRunAction(this.CurrentUserId, _ownerContainer.Key, DirectoryId, Action);
		}

		/// <summary>
		/// Determines whether this instance [can user read] the specified info.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user read] the specified info; otherwise, <c>false</c>.
		/// </returns>
		public bool CanUserRead(DirectoryInfo info)
		{
			return CanUserRead(this.CurrentUserId, info.Id);
		}

		/// <summary>
		/// Determines whether this instance [can user read] the specified directory id.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user read] the specified directory id; otherwise, <c>false</c>.
		/// </returns>
		public bool CanUserRead(int DirectoryId)
		{
			return CanUserRunAction(this.CurrentUserId, DirectoryId, "Read");
		}

		/// <summary>
		/// Determines whether this instance [can user write] the specified info.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user write] the specified info; otherwise, <c>false</c>.
		/// </returns>
		public bool CanUserWrite(DirectoryInfo info)
		{
			return CanUserWrite(this.CurrentUserId, info.Id);
		}

		/// <summary>
		/// Determines whether this instance [can user write] the specified directory id.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user write] the specified directory id; otherwise, <c>false</c>.
		/// </returns>
		public bool CanUserWrite(int DirectoryId)
		{
			return CanUserRunAction(this.CurrentUserId, DirectoryId, "Write");
		}

		/// <summary>
		/// Determines whether this instance [can user admin] the specified info.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user admin] the specified info; otherwise, <c>false</c>.
		/// </returns>
		public bool CanUserAdmin(DirectoryInfo info)
		{
			return CanUserAdmin(this.CurrentUserId, info.Id);
		}

		/// <summary>
		/// Determines whether this instance [can user admin] the specified directory id.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can user admin] the specified directory id; otherwise, <c>false</c>.
		/// </returns>
		public bool CanUserAdmin(int DirectoryId)
		{
			return CanUserRunAction(this.CurrentUserId, DirectoryId, "Admin");
		}
		#endregion

		#region -- AllowFileHistory --
		/// <summary>
		/// Allows the file history.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="AllowHistory">if set to <c>true</c> [allow history].</param>
		/// <returns></returns>
		public FileInfo AllowFileHistory(FileInfo info, bool AllowHistory)
		{
			return AllowFileHistory(info.Id, AllowHistory);
		}

		/// <summary>
		/// Allows the file history.
		/// </summary>
		/// <param name="FileId">The file id.</param>
		/// <param name="AllowHistory">if set to <c>true</c> [allow history].</param>
		/// <returns></returns>
		public FileInfo AllowFileHistory(int FileId, bool AllowHistory)
		{
			using (IDataReader reader = DBFile.AllowHistory(CurrentTimeZoneId, FileId, AllowHistory))
			{
				if (reader.Read())
					return new FileInfo(this, reader);
				else
					return null;
			}
		}
		#endregion

		#region -- DeleteFile --
		/// <summary>
		/// Deletes the file.
		/// </summary>
		/// <param name="fileInfo">The file info.</param>
		public void DeleteFile(FileInfo fileInfo)
		{
			DeleteFile(fileInfo.Id);
		}

		/// <summary>
		/// Deletes the file.
		/// </summary>
		/// <param name="FileId">The file id.</param>
		public void DeleteFile(int FileId)
		{
			using (Mediachase.IBN.Database.DbTransaction tran = Mediachase.IBN.Database.DbTransaction.Begin())
			{
				string containerKey = this.OwnerContainer.Key;
				string[] parts = containerKey.Split('_');
				if (parts.Length == 2)
				{
					try
					{
						int objectId = int.Parse(parts[1]);
						if (parts[0] == "ProjectId")
							SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_FileList_FileDeleted, objectId, FileId);
						else if (parts[0] == "IncidentId")
							SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_FileList_FileDeleted, objectId, FileId);
						else if (parts[0] == "TaskId")
							SystemEvents.AddSystemEvents(SystemEventTypes.Task_Updated_FileList_FileDeleted, objectId, FileId);
						else if (parts[0] == "DocumentId")
							SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_FileList_FileDeleted, objectId, FileId);
						else if (parts[0] == "ToDoId")
							SystemEvents.AddSystemEvents(SystemEventTypes.Todo_Updated_FileList_FileDeleted, objectId, FileId);
						else if (parts[0] == "EventId")
							SystemEvents.AddSystemEvents(SystemEventTypes.CalendarEntry_Updated_FileList_FileDeleted, objectId, FileId);
					}
					catch
					{
					}
				}

				DBFile.Delete(FileId);

				tran.Commit();
			}
		}

		#endregion

		#region -- DeleteFolder --
		/// <summary>
		/// Deletes the folder.
		/// </summary>
		/// <param name="dirInfo">The dir info.</param>
		public void DeleteFolder(DirectoryInfo dirInfo)
		{
			DeleteFolder(dirInfo.Id);
		}

		/// <summary>
		/// Deletes the folder.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		public void DeleteFolder(int DirectoryId)
		{
			DBDirectory.Delete(DirectoryId);
		}

		/// <summary>
		/// Deletes the folder.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		internal static void InnerDeleteFolder(int DirectoryId)
		{
			DBDirectory.Delete(DirectoryId);
		}
		#endregion

		#region -- DeleteAll
		/// <summary>
		/// Deletes all.
		/// </summary>
		public void DeleteAll()
		{
			// Clear User Roles
			ForeignContainerKey.Delete(this.OwnerContainer.Key);
			UserRole.DeleteAll(this.OwnerContainer.Key);
			this.DeleteFolder(this.Root);
		}
		#endregion

		private static string GetCleanupFileName(string fileName)
		{
			string retVal = fileName;
			if (!string.IsNullOrEmpty(retVal))
			{
				//---------------------------------
				// OR: [2007-08-28] double dots fix
				while (retVal.IndexOf("..") > 0)
					retVal = retVal.Replace("..", ".");
				//---------------------------------
			}
			return retVal;
		}

	}
}
