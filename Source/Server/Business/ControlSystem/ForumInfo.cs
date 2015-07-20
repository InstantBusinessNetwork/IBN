using System;
using System.Data;
using Mediachase.IBN.Database.ControlSystem;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Summary description for ForumInfo.
	/// </summary>
	public class ForumInfo
	{
		private int _id = 0;
		private DateTime _created = DateTime.MinValue;
		private string _name = string.Empty;
		private string _description = string.Empty;
	
		public ForumInfo()
		{
		}

		public ForumInfo(IDataReader reader)
		{
			_id = (int)reader["ForumId"];
			_created = (DateTime)reader["Created"];
			_name = (string)reader["Name"];
			_description = (string)SqlHelper. DBNull2Null(reader["Description"],string.Empty);
		}

		public int Id
		{
			get
			{
				return _id;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public string Description
		{
			get
			{
				return _description;
			}
		}

		public DateTime Created
		{
			get
			{
				return _created;
			}
		}

	}
}
