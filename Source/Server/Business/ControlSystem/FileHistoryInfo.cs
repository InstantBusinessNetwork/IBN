using System;
using System.Data;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Summary description for FileHistoryInfo.
	/// </summary>
	public class FileHistoryInfo
	{
		private FileStorage _ownerFileStorage;

		private int _Id;
		private int	_fileId;
		private string	_name;
		private int	_length;
		private int _filebinaryId;
		private string _contentType;
		private int _contentTypeId;

		private int _modifierId;
		private DateTime _modified;

		public int Id
		{
			get{return _Id;}
		}

		public int FileId
		{
			get { return _fileId; }
		}

		public string Name
		{
			get{return _name;}
		}

		public int Length
		{
			get{return _length;}
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

		public int ModifierId
		{
			get{return _modifierId;}
		}

		public DateTime Modified
		{
			get{return _modified;}
		}

		public FileHistoryInfo(FileStorage ownerFileStorage, IDataReader reader)
		{
			_ownerFileStorage = ownerFileStorage;

			_Id = (int)reader["Id"];
			_fileId = (int)reader["FileId"];
			_name = (string)reader["Name"];
			_modifierId = (int)reader["ModifierId"];
			_modified = (DateTime)reader["Modified"];
			_filebinaryId = (reader["FileBinaryId"]==DBNull.Value) ? 0 : (int)reader["FileBinaryId"];
			_length = (reader["Length"]==DBNull.Value) ? 0 : (int)reader["Length"];
			_contentType = (reader["ContentTypeString"]==DBNull.Value ? String.Empty : (string)reader["ContentTypeString"]);
			_contentTypeId = (reader["ContentTypeId"]==DBNull.Value ? 0 : (int)reader["ContentTypeId"]);
		}
	}
}
