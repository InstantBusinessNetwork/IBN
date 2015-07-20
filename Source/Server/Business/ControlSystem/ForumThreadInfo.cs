using System;
using System.Data;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Summary description for ForumThreadInfo.
	/// </summary>
	public class ForumThreadInfo
	{
		private int _id = 0;
		private int _ownerForumId = 0;
		private string _name = string.Empty;
		private DateTime _created = DateTime.MinValue;

		public ForumThreadInfo()
		{
		}

		public ForumThreadInfo(IDataReader reader)
		{
			_id = (int)reader["ThreadId"];
			_ownerForumId = (int)reader["ForumId"];
			_created = (DateTime)reader["Created"];
			_name = (string)reader["Name"];
		}

		public int Id
		{
			get
			{
				return _id;
			}
		}

		public int OwnerForumId
		{
			get
			{
				return _ownerForumId;
			}
		}

		public string Name
		{
			get
			{
				return _name;
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
