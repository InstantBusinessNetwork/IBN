using System;
using System.Data;
using Mediachase.IBN.Database.ControlSystem;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Summary description for ForumThreadNodeInfo.
	/// </summary>
	public class ForumThreadNodeInfo
	{
		private int _id = 0;
		private int _ownerThreadId = 0;
		private string _text = string.Empty;
		private DateTime _created = DateTime.MinValue;
		private int _creatorId = 0;
		private bool _isCreatorIbnUser = false;
		private string _creatorName = null;
		private string _creatorEmail = null;
		private int _eMailMessageId = 0;
		private ForumStorage.NodeContentType _nodeContentType = ForumStorage.NodeContentType.Text;

		public ForumThreadNodeInfo()
		{
		}

		#region ForumThreadNodeInfo
		public ForumThreadNodeInfo(IDataReader reader)
		{
			_id = (int)reader["NodeId"];
			_ownerThreadId = (int)reader["ThreadId"];
			_text = (string)reader["Text"];
			_created = (DateTime)reader["Created"];
			_creatorId = (int)SqlHelper.DBNull2Null(reader["CreatorId"],0);
			_isCreatorIbnUser = (reader["CreatorId"]!=DBNull.Value);
			_creatorName= (string)SqlHelper.DBNull2Null(reader["CreatorName"]);
			_creatorEmail= (string)SqlHelper.DBNull2Null(reader["CreatorEmail"]);
			_eMailMessageId = (int)SqlHelper.DBNull2Null(reader["EMailMessageId"],0);
			_nodeContentType = (ForumStorage.NodeContentType)reader["NodeType"];
		}
		#endregion

		public static ForumThreadNodeInfo Load(int NodeId)
		{
			ForumThreadNodeInfo retVal = null;

			using(IDataReader reader = DBForum.GetForumThreadNode(Security.CurrentUser.TimeZoneId,NodeId) )
			{
				if(reader.Read())
				{
					retVal = new ForumThreadNodeInfo(reader);
				}
			}

			return retVal;
		}

        /// <summary>
        /// Gets the owner container key.
        /// </summary>
        /// <param name="NodeId">The node id.</param>
        /// <returns></returns>
        public static string GetOwnerContainerKey(int NodeId)
        {
            using (IDataReader reader = DBForum.GetOwnerContainerKey(NodeId))
            {
                if (reader.Read())
                {
                    return (string)reader["ContainerKey"];
                }
            }

            return null;
        }

		#region Properties
		public int Id
		{
			get
			{
				return _id;
			}
		}

		public int OwnerThreadId
		{
			get
			{
				return _ownerThreadId;
			}
		}

		public DateTime Created
		{
			get
			{
				return _created;
			}
		}

		public string Text
		{
			get
			{
				return _text;
			}
		}

		public bool IsCreatorIbnUser
		{
			get
			{
				return _isCreatorIbnUser;
			}
		}

		public int CreatorId
		{
			get
			{
				return _creatorId;
			}
		}

		public string CreatorName
		{
			get
			{
				return _creatorName;
			}
		}

		public string CreatorEmail
		{
			get
			{
				return _creatorEmail;
			}
		}

		public int EMailMessageId
		{
			get
			{
				return _eMailMessageId;
			}
		}

		public ForumStorage.NodeContentType ContentType
		{
			get
			{
				return _nodeContentType;
			}
		}
		#endregion
	}
}
