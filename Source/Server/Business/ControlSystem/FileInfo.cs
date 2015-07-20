using System;
using System.Collections;
using System.Data;
using Mediachase.IBN.Database.ControlSystem;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Summary description for FileInfo.
	/// </summary>
	public class FileInfo
	{
		private FileStorage _ownerFileStorage = null;
		private string _containerKey = null;

		private int	_fileId;
		private string	_name;
		private int	_length;
		private int _filebinaryId;
		private string _contentType;
		private int _contentTypeId;
		private bool _allowHistory;

		private int _downloadCount = 0;

		private int _creatorId;
		private DateTime _created;
		private int _modifierId;
		private DateTime _modified;

		private int _parentDirectoryId;
		private DirectoryInfo _parentDirectory = null;

		private string _description = string.Empty;

		protected int CurrentTimeZoneId
		{
			get
			{
				if(Security.CurrentUser!=null)
					return Security.CurrentUser.TimeZoneId;
				return 0;
			}
		}

		public FileInfo(FileStorage ownerFileStorage, IDataReader reader):this(reader)
		{
			_ownerFileStorage = ownerFileStorage;
			if(ownerFileStorage!=null && ownerFileStorage.OwnerContainer!=null)
				_containerKey = ownerFileStorage.OwnerContainer.Key;
		}

		public FileInfo(IDataReader reader)
		{
			//_ownerFileStorage = ownerFileStorage;
			try
			{
					_containerKey = (string)reader["ContainerKey"];
			}
			catch
			{
			}

			_fileId = (int)reader["FileId"];
			_name = (string)reader["Name"];
			_parentDirectoryId = (int)reader["DirectoryId"];

			_creatorId = (int)SqlHelper.DBNull2Null(reader["CreatorId"],0);
			_created = (DateTime)reader["Created"];
			_modifierId = (int)SqlHelper.DBNull2Null(reader["ModifierId"],0);
			_modified = (DateTime)reader["Modified"];

			_filebinaryId = (reader["FileBinaryId"]==DBNull.Value) ? 0 : (int)reader["FileBinaryId"];
			_length = (reader["Length"]==DBNull.Value) ? 0 : (int)reader["Length"];
			_contentType = (reader["ContentTypeString"]==DBNull.Value ? String.Empty : (string)reader["ContentTypeString"]);
			_contentTypeId = (reader["ContentTypeId"]==DBNull.Value ? 0 : (int)reader["ContentTypeId"]);
			_allowHistory = (bool)reader["AllowHistory"];

			_description = (string)reader["Description"];

		}

		public string ContainerKey
		{
			get
			{
				return _containerKey;
			}
		}

		public int Id
		{
			get{return _fileId;}
		}

		public string Name
		{
			get{return _name;}
		}

		public int Length
		{
			get{return _length;}
		}

		public int CreatorId
		{
			get{return _creatorId;}
		}

		public DateTime Created
		{
			get{return _created;}
		}

		public int ModifierId
		{
			get{return _modifierId;}
		}

		public DateTime Modified
		{
			get{return _modified;}
		}

		public int ParentDirectoryId
		{
			get{return _parentDirectoryId;}
		}

		public DirectoryInfo ParentDirectory
		{
			get
			{
				if (_parentDirectory == null)
				{
					if(_ownerFileStorage!=null)
						_parentDirectory = _ownerFileStorage.GetDirectory(_parentDirectoryId);
					else
					{
						_parentDirectory = FileStorage.InnerGetDirectory(_parentDirectoryId);
					}
				}

				return _parentDirectory;
			}
		}

		public int FileBinaryId
		{
			get{return _filebinaryId;}
			set{ _filebinaryId = value; }	
		}

		public string FileBinaryContentType
		{
			get {return _contentType; }
		}

		public int FileBinaryContentTypeId
		{
			get { return _contentTypeId; }
		}

		public bool AllowHistory
		{
			get { return _allowHistory; }
		}

		public int DownloadCount
		{
			get
			{
				if (_filebinaryId != 0 && _downloadCount == 0)
					_downloadCount = DBFile.GetDownloadCount(_filebinaryId);
				
				return _downloadCount;
			}
		}

		public FileHistoryInfo[] GetHistory()
		{
			ArrayList list = new ArrayList();
			using (IDataReader reader = DBFile.GetHistory(CurrentTimeZoneId, _fileId))
			{
				while (reader.Read())
				{
					list.Add(new FileHistoryInfo(_ownerFileStorage, reader));
				}
			}
			return (FileHistoryInfo[])list.ToArray(typeof(FileHistoryInfo));
		}

		public string Description
		{
			get { return _description; }
		}
	
	}
}
