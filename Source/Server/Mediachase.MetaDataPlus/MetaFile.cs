using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

using Mediachase.MetaDataPlus.Common;
using Mediachase.MetaDataPlus.Configurator;


namespace Mediachase.MetaDataPlus
{
	/// <summary>
	/// Summary description for MetaFile.
	/// </summary>
	public class MetaFile
	{
		private int _id = -1;

		private string _fileName = string.Empty;
		private string _contentType = string.Empty;
		private byte[] _data;
		private DateTime _creationTime = DateTime.Now;
		private DateTime _lastWriteTime = DateTime.Now;
		private DateTime _lastReadTime = DateTime.Now;

		private bool _HasChanges = true;

		public MetaFile()
		{
		}

		public MetaFile(string name, byte[] buffer)
		{
			this.Name = name;
			this.Buffer = buffer;
		}

		public MetaFile(string name, string contentType, byte[] buffer)
		{
			this.Name = name;
			this.Buffer = buffer;
			this.ContentType = contentType;
		}

		internal MetaFile(SqlDataReader reader)
		{
			_id = (int)reader["MetaKey"];
			_fileName = (string)(SqlHelper.DBNull2Null(reader["FileName"], string.Empty));
			_contentType = (string)(SqlHelper.DBNull2Null(reader["ContentType"], string.Empty));

			if (MetaDataContext.Current.MetaFileDataStorageType == MetaFileDataStorageType.DataBase)
				_data = (byte[])(SqlHelper.DBNull2Null(reader["Data"]));
			else
			{
				using (FileStream stream = File.OpenRead(Path.Combine(MetaDataContext.Current.LocalDiskStorage, string.Format("{0}.mdpdata", _id))))
				{
					_data = new byte[stream.Length];
					stream.Read(_data, 0, (int)stream.Length);
				}
			}

			_creationTime = (DateTime)reader["CreationTime"];
			_lastWriteTime = (DateTime)reader["LastWriteTime"];
			_lastReadTime = (DateTime)reader["LastReadTime"];

			this.HasChanges = false;
		}

		internal void Delete()
		{
			if (MetaDataContext.Current.MetaFileDataStorageType == MetaFileDataStorageType.LocalDisk)
			{
				File.Delete(Path.Combine(MetaDataContext.Current.LocalDiskStorage, string.Format("{0}.mdpdata", _id)));
			}
		}

		#region Common Properties
		internal void SetId(int id)
		{
			_id = id;
		}

        public int Id
        {
            get
            {
                return _id;
            }
        }

		internal bool HasChanges
		{
			get
			{
				return _HasChanges;
			}
			set
			{
				_HasChanges = value;
			}
		}

		public string Name
		{
			get
			{
				return _fileName;
			}
			set
			{
				HasChanges = true;
				_fileName = value;
			}
		}

		public string ContentType
		{
			get
			{
				return _contentType;
			}
			set
			{
				HasChanges = true;
				_contentType = value;
			}
		}

		public int Size
		{
			get
			{
				return _data == null ? 0 : _data.Length;
			}
		}

		public byte[] Buffer
		{
			get
			{
				return _data;
			}
			set
			{
				HasChanges = true;
				_data = value;
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return _creationTime;
			}
			set
			{
				HasChanges = true;
				_creationTime = value;
			}
		}

		public DateTime LastWriteTime
		{
			get
			{
				return _lastWriteTime;
			}
			set
			{
				HasChanges = true;
				_lastWriteTime = value;
			}
		}


		public DateTime LastReadTime
		{
			get
			{
				return _lastReadTime;
			}
			set
			{
				HasChanges = true;
				_lastReadTime = value;
			}
		}

		#endregion
	}
}
