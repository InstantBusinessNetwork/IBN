using System;
using System.Data;

namespace Mediachase.Ibn.ControlSystem
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

		public DirectoryInfo(FileStorage ownerFileStorage, IDataReader reader)
		{
			_ownerFileStorage = ownerFileStorage;

			_id = (int)reader["DirectoryId"];
			_name = (string)reader["Name"];
			
			_creatorId = (int)SqlHelper.DBNull2Null(reader["CreatorId"], 0);
			_created = (DateTime)reader["Created"];
			_modifierId = (int)SqlHelper.DBNull2Null(reader["ModifierId"], 0);
			_modified = (DateTime)reader["Modified"];

			_parentDirectoryId = (int)SqlHelper.DBNull2Null(reader["ParentDirectoryId"], -1);
		}

		public int Id
		{
			get{return _id;}
		}

		public string Name
		{
			get{return _name;}
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

		public DirectoryInfo Parent
		{
			get
			{
				if(_parentDirectory == null)
				{
					if(_ownerFileStorage != null)
						_parentDirectory = _ownerFileStorage.GetDirectory(_parentDirectoryId);
					else
						_parentDirectory = FileStorage.InnerGetDirectory(_parentDirectoryId);
				}

				return _parentDirectory;
			}
		}

		public int ParentDirectoryId
		{
			get{return _parentDirectoryId;}
		}
	}
}
