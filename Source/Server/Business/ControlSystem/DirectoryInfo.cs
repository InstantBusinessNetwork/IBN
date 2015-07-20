using System;
using System.Data;
using Mediachase.IBN.Database.ControlSystem;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Summary description for DirectoryInfo.
	/// </summary>
	public class DirectoryInfo
	{
		private FileStorage _ownerFileStorage;

		private int _id;
		private string _name;

		private int _creatorId;
		private DateTime _created;
		private int _modifierId;
		private DateTime _modified;

		private int _parentDirectoryId;
		private DirectoryInfo _parentDirectory = null;

        /// <summary>
        /// Gets the current time zone id.
        /// </summary>
        /// <value>The current time zone id.</value>
		protected int CurrentTimeZoneId
		{
			get
			{
				if(Security.CurrentUser!=null)
					return Security.CurrentUser.TimeZoneId;
				return 0;
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DirectoryInfo"/> class.
        /// </summary>
        /// <param name="ownerFileStorage">The owner file storage.</param>
        /// <param name="reader">The reader.</param>
		public DirectoryInfo(FileStorage ownerFileStorage, IDataReader reader)
		{
			_ownerFileStorage = ownerFileStorage;

			_id = (int)reader["DirectoryId"];
			_name = (string)reader["Name"];

			_creatorId = (int)SqlHelper.DBNull2Null(reader["CreatorId"],0);
			_created = (DateTime)SqlHelper.DBNull2Null(reader["Created"], DateTime.Now);
			_modifierId = (int)SqlHelper.DBNull2Null(reader["ModifierId"],0);
			_modified = (DateTime)SqlHelper.DBNull2Null(reader["Modified"], DateTime.Now);

			_parentDirectoryId = (int)SqlHelper.DBNull2Null(reader["ParentDirectoryId"],-1);
		}

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
		public int Id
		{
			get{return _id;}
		}

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
		public string Name
		{
			get{return _name;}
		}

        /// <summary>
        /// Gets the creator id.
        /// </summary>
        /// <value>The creator id.</value>
		public int CreatorId
		{
			get{return _creatorId;}
		}

        /// <summary>
        /// Gets the created.
        /// </summary>
        /// <value>The created.</value>
		public DateTime Created
		{
			get{return _created;}
		}

        /// <summary>
        /// Gets the modifier id.
        /// </summary>
        /// <value>The modifier id.</value>
		public int ModifierId
		{
			get{return _modifierId;}
		}

        /// <summary>
        /// Gets the modified.
        /// </summary>
        /// <value>The modified.</value>
		public DateTime Modified
		{
			get{return _modified;}
		}

        /// <summary>
        /// Gets the owner file storage.
        /// </summary>
        /// <value>The owner file storage.</value>
        public FileStorage OwnerFileStorage
        {
            get { return _ownerFileStorage; }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
		public DirectoryInfo Parent
		{
			get
			{
				if (_parentDirectory == null)
				{
					if(_ownerFileStorage!=null)
						_parentDirectory = _ownerFileStorage.GetDirectory(_parentDirectoryId);
					else
						_parentDirectory = FileStorage.InnerGetDirectory(_parentDirectoryId);
				}

				return _parentDirectory;
			}
		}

        /// <summary>
        /// Gets the parent directory id.
        /// </summary>
        /// <value>The parent directory id.</value>
		public int ParentDirectoryId
		{
			get{return _parentDirectoryId;}
		}

        /// <summary>
        /// Gets the container key.
        /// </summary>
        /// <param name="DirectoryId">The directory id.</param>
        /// <returns></returns>
		public static string GetContainerKey(int DirectoryId)
		{
			return DBDirectory.GetContainerKey(DirectoryId);
		}

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <returns></returns>
        public FileInfo[] GetFiles()
        {
            return this.OwnerFileStorage.GetFiles(this);
        }
    }
}
