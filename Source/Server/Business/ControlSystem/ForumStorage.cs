using System;
using System.Data;
using System.Collections;
using Mediachase.IBN.Database.ControlSystem;
using System.IO;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Summary description for ForumStorage.
	/// </summary>
	public class ForumStorage: IIbnControl
	{
		//private string				_containerKey = string.Empty;
		private IbnControlInfo		_info = null;
		private IIbnContainer		_ownerContainer = null;
		private const int			BufferSize = 655360;

		public enum NodeType
		{
			Internal = 0,
			Incoming = 1,
			Outgoing = 2
		}

		public enum NodeContentType
		{
			/*ExternalQuestion = 1,
			ExternalMessage = 2,
			PrivateMessage = 10,
			PublicMessage = 20,
			PublicResolution = 21,
			PublicWorkaround = 22,*/

			// OZ: Real Values
			Text = 0,
			TextWithFiles = 1,
			EMail = 2,
			PhoneCall = 3,
			Report = 4,
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ForumStorage"/> class.
		/// </summary>
		public ForumStorage()
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
			//_containerKey = owner.Key;
			_info =  controlInfo;
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get
			{
				return "ForumStorage";
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
				return new string[]{};
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
		/// Gets the parameters.
		/// </summary>
		/// <value>The parameters.</value>
		public System.Collections.Specialized.NameValueCollection Parameters
		{
			get
			{
				return _info.Parameters;
			}
		}

		/// <summary>
		/// Gets the base actions.
		/// </summary>
		/// <param name="Action">The action.</param>
		/// <returns></returns>
		public string[] GetBaseActions(string Action)
		{
			return new string[]{};
		}

		/// <summary>
		/// Gets the derived actions.
		/// </summary>
		/// <param name="Action">The action.</param>
		/// <returns></returns>
		public string[] GetDerivedActions(string Action)
		{
			return new string[]{};
		}

		#endregion

		#region -- GetForums --
		/// <summary>
		/// Gets the forums.
		/// </summary>
		/// <returns></returns>
		public ForumInfo[] GetForums()
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DBForum.GetForumsByContainerKey(_ownerContainer.Key))
			{
				while (reader.Read())
				{
					list.Add(new ForumInfo(reader));
				}
			}
			return (ForumInfo[])list.ToArray(typeof(ForumInfo));
		}
		#endregion

		#region -- GetForumThread --
		/// <summary>
		/// Gets the forum threads.
		/// </summary>
		/// <param name="forum">The forum.</param>
		/// <returns></returns>
		public ForumThreadInfo[] GetForumThreads(ForumInfo forum)
		{
			return GetForumThreads(forum.Id);
		}

		/// <summary>
		/// Gets the forum threads.
		/// </summary>
		/// <param name="ForumId">The forum id.</param>
		/// <returns></returns>
		public ForumThreadInfo[] GetForumThreads(int ForumId)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DBForum.GetForumThreadsByForumId(ForumId))
			{
				while (reader.Read())
				{
					list.Add(new ForumThreadInfo(reader));
				}
			}
			return (ForumThreadInfo[])list.ToArray(typeof(ForumThreadInfo));
		}

		/// <summary>
		/// Gets the forum threads.
		/// </summary>
		/// <returns></returns>
		public ForumThreadInfo[] GetForumThreads()
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DBForum.GetForumThreadsByContainerKey(_ownerContainer.Key))
			{
				while (reader.Read())
				{
					list.Add(new ForumThreadInfo(reader));
				}
			}
			return (ForumThreadInfo[])list.ToArray(typeof(ForumThreadInfo));
		}
		#endregion

		#region CurrentUserId
		protected int CurrentUserId
		{
			get
			{
				if(Security.CurrentUser!=null)
					return Security.CurrentUser.UserID;

				return -1;
			}
		}
		#endregion

		#region CurrentUserTimeZoneId
		protected int CurrentUserTimeZoneId
		{
			get
			{
				if (Security.CurrentUser != null)
					return Security.CurrentUser.TimeZoneId;

				return User.GetTimeZoneByBias(-TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours);
			}
		}
		#endregion

		#region -- GetForumThreadNodes --
		/// <summary>
		/// Gets the forum thread nodes.
		/// </summary>
		/// <param name="thread">The thread.</param>
		/// <returns></returns>
		public ForumThreadNodeInfo[] GetForumThreadNodes(ForumThreadInfo thread)
		{
			return GetForumThreadNodes(thread.Id);
		}

		/// <summary>
		/// Gets the forum thread nodes.
		/// </summary>
		/// <param name="forum">The forum.</param>
		/// <returns></returns>
		public ForumThreadNodeInfo[] GetForumThreadNodes(ForumInfo forum)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DBForum.GetForumThreadNodesByForumId(this.CurrentUserTimeZoneId, forum.Id))
			{
				while (reader.Read())
				{
					list.Add(new ForumThreadNodeInfo(reader));
				}
			}
			return (ForumThreadNodeInfo[])list.ToArray(typeof(ForumThreadNodeInfo));
		}

		/// <summary>
		/// Gets the forum thread nodes.
		/// </summary>
		/// <param name="ThreadId">The thread id.</param>
		/// <returns></returns>
		public ForumThreadNodeInfo[] GetForumThreadNodes(int ThreadId)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DBForum.GetForumThreadNodesByThreadId(this.CurrentUserTimeZoneId, ThreadId))
			{
				while (reader.Read())
				{
					list.Add(new ForumThreadNodeInfo(reader));
				}
			}
			return (ForumThreadNodeInfo[])list.ToArray(typeof(ForumThreadNodeInfo));
		}

		/// <summary>
		/// Gets the forum thread nodes.
		/// </summary>
		/// <returns></returns>
		public ForumThreadNodeInfo[] GetForumThreadNodes()
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DBForum.GetForumThreadNodesByContainerKey(this.CurrentUserTimeZoneId, _ownerContainer.Key))
			{
				while (reader.Read())
				{
					list.Add(new ForumThreadNodeInfo(reader));
				}
			}
			return (ForumThreadNodeInfo[])list.ToArray(typeof(ForumThreadNodeInfo));
		}
		#endregion

		#region -- GetForum --
		public ForumInfo GetForum(int ForumId)
		{
			using (IDataReader reader = DBForum.GetForum(ForumId))
			{
				if (reader.Read())
				{
					return new ForumInfo(reader);
				}
			}

			return null;
		}
		#endregion

		#region -- CreateForum --
		public ForumInfo CreateForum(string Name)
		{
			return CreateForum(Name, string.Empty, DateTime.UtcNow);
		}

		public ForumInfo CreateForum(string Name, string Description)
		{
			return CreateForum(Name, Description, DateTime.UtcNow);
		}

		public ForumInfo CreateForum(string Name, string Description, DateTime Created)
		{
			int retVal = DBForum.CreateForum(_ownerContainer.Key, Name, Description, Created);
			return GetForum(retVal);
		}
		#endregion

		#region -- GetForumThread --
		public ForumThreadInfo GetForumThread(int ThreadId)
		{
			using (IDataReader reader = DBForum.GetForumThread(ThreadId))
			{
				if (reader.Read())
				{
					return new ForumThreadInfo(reader);
				}
			}

			return null;
		}
		#endregion

		#region -- CreateForumThread --
		public ForumThreadInfo CreateForumThread(ForumInfo forum,string Name)
		{
			return CreateForumThread(forum.Id, Name, DateTime.UtcNow);
		}

		public ForumThreadInfo CreateForumThread(int ForumId,string Name)
		{
			return CreateForumThread(ForumId, Name, DateTime.UtcNow);
		}

		public ForumThreadInfo CreateForumThread(ForumInfo forum, string Name, DateTime Created)
		{
			return CreateForumThread(forum.Id,Name,Created);
		}

		public ForumThreadInfo CreateForumThread(int ForumId, string Name, DateTime Created)
		{
			int retVal = DBForum.CreateForumThread(ForumId,Name, Created);
			return GetForumThread(retVal);
		}

		public ForumThreadInfo CreateForumThread(string Name, DateTime Created)
		{
			int retVal = DBForum.CreateSimpleForumThread(_ownerContainer.Key,Name, Created);
			return GetForumThread(retVal);
		}

		public ForumThreadInfo CreateForumThread(string Name)
		{
			int retVal = DBForum.CreateSimpleForumThread(_ownerContainer.Key,Name, DateTime.UtcNow);
			return GetForumThread(retVal);
		}
		#endregion

		#region -- GetForumThreadNode --
		public ForumThreadNodeInfo GetForumThreadNode(int NodeId)
		{
			using (IDataReader reader = DBForum.GetForumThreadNode(this.CurrentUserTimeZoneId, NodeId))
			{
				if (reader.Read())
				{
					return new ForumThreadNodeInfo(reader);
				}
			}

			return null;
		}
		#endregion

		#region -- CreateForumThreadNode --
		public ForumThreadNodeInfo CreateForumThreadNode(ForumThreadInfo thread, string Text, int CreatorId, int nodeType)
		{
			return CreateForumThreadNode(thread.Id, Text, DateTime.UtcNow, CreatorId, nodeType);
		}

		public ForumThreadNodeInfo CreateForumThreadNode(ForumThreadInfo thread, string Text, string CreatorName, string CreatorEmail, int nodeType)
		{
			return CreateForumThreadNode(thread.Id, Text, DateTime.UtcNow, CreatorName, CreatorEmail, nodeType);
		}

		public ForumThreadNodeInfo CreateForumThreadNode(int ThreadId, string Text, int CreatorId, int nodeType)
		{
			return CreateForumThreadNode(ThreadId, Text, DateTime.UtcNow, CreatorId, nodeType);
		}

		public ForumThreadNodeInfo CreateForumThreadNode(int ThreadId, string Text, string CreatorName, string CreatorEmail, int nodeType)
		{
			return CreateForumThreadNode(ThreadId, Text, DateTime.UtcNow, CreatorName, CreatorEmail, nodeType);
		}

		public ForumThreadNodeInfo CreateForumThreadNode(int ThreadId, string Text, DateTime Created, string CreatorName, string CreatorEmail, int nodeType)
		{
			int retVal = DBForum.CreateForumThreadNode(ThreadId, Text, Created, null, CreatorName, CreatorEmail, null, nodeType);
			return GetForumThreadNode(retVal);
		}

		public ForumThreadNodeInfo CreateForumThreadNode(int ThreadId, string Text, DateTime Created, string CreatorName, string CreatorEmail, int EMailMessageId, int nodeType)
		{
			int retVal = DBForum.CreateForumThreadNode(ThreadId, Text, Created, null, CreatorName, CreatorEmail, EMailMessageId, nodeType);
			return GetForumThreadNode(retVal);
		}

		public ForumThreadNodeInfo CreateForumThreadNode(int ThreadId, string Text, DateTime Created, int CreatorId, int nodeType)
		{
			int retVal = DBForum.CreateForumThreadNode(ThreadId, Text, Created, CreatorId, null, null, null, nodeType);
			return GetForumThreadNode(retVal);
		}

		public ForumThreadNodeInfo CreateForumThreadNode(int ThreadId, string Text, DateTime Created, int CreatorId, int EMailMessageId, int nodeType)
		{
			int retVal = DBForum.CreateForumThreadNode(ThreadId, Text, Created, CreatorId, null, null, EMailMessageId, nodeType);
			return GetForumThreadNode(retVal);
		}

		public ForumThreadNodeInfo CreateForumThreadNode(string Text, int CreatorId, int nodeType)
		{
			return CreateForumThreadNode(Text, DateTime.UtcNow, CreatorId, nodeType);
		}

		public ForumThreadNodeInfo CreateForumThreadNode(string Text, string CreatorName, string CreatorEmail, int nodeType)
		{
			return CreateForumThreadNode(Text, DateTime.UtcNow, CreatorName, CreatorEmail, nodeType);
		}

		public ForumThreadNodeInfo CreateForumThreadNode(string Text, DateTime Created, string CreatorName, string CreatorEmail, int nodeType)
		{
			int retVal = DBForum.CreateSimpleForumThreadNode(_ownerContainer.Key, Text, Created, null, CreatorName, CreatorEmail, null, nodeType);
			return GetForumThreadNode(retVal);
		}

		public ForumThreadNodeInfo CreateForumThreadNode(string Text, DateTime Created, string CreatorName, string CreatorEmail, int EMailMessageId, int nodeType)
		{
			int retVal = DBForum.CreateSimpleForumThreadNode(_ownerContainer.Key, Text, Created, null, CreatorName, CreatorEmail, EMailMessageId, nodeType);
			return GetForumThreadNode(retVal);
		}

		public ForumThreadNodeInfo CreateForumThreadNode(string Text, DateTime Created, int CreatorId, int nodeType)
		{
			int retVal = DBForum.CreateSimpleForumThreadNode(_ownerContainer.Key, Text, Created, CreatorId, null, null, null, nodeType);
			return GetForumThreadNode(retVal);
		}

		public ForumThreadNodeInfo CreateForumThreadNode(string Text, DateTime Created, int CreatorId, int EMailMessageId, int nodeType)
		{
			int retVal = DBForum.CreateSimpleForumThreadNode(_ownerContainer.Key, Text, Created, CreatorId, null, null, EMailMessageId, nodeType);
			return GetForumThreadNode(retVal);
		}
		#endregion


		#region -- RenameForum --
		/// <summary>
		/// Renames the forum.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="newName">The new name.</param>
		/// <param name="newDescription">The new description.</param>
		/// <returns></returns>
		public ForumInfo RenameForum(ForumInfo info, string newName, string newDescription)
		{
			return RenameForum(info.Id, newName, newDescription);
		}

		/// <summary>
		/// Renames the forum.
		/// </summary>
		/// <param name="ForumId">The forum id.</param>
		/// <param name="newName">The new name.</param>
		/// <param name="newDescription">The new description.</param>
		/// <returns></returns>
		public ForumInfo RenameForum(int ForumId, string newName, string newDescription)
		{
			DBForum.RenameForum(ForumId, newName, newDescription);
			return this.GetForum(ForumId);
		}
		#endregion

		#region -- RenameForumThread --
		/// <summary>
		/// Renames the forum thread.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="newName">The new name.</param>
		/// <returns></returns>
		public ForumThreadInfo RenameForumThread(ForumThreadInfo info, string newName)
		{
			return RenameForumThread(info.Id, newName);
		}

		/// <summary>
		/// Renames the forum thread.
		/// </summary>
		/// <param name="ThreadId">The thread id.</param>
		/// <param name="newName">The new name.</param>
		/// <returns></returns>
		public ForumThreadInfo RenameForumThread(int ThreadId, string newName)
		{
			DBForum.RenameForumThread(ThreadId, newName);
			return this.GetForumThread(ThreadId);
		}
		#endregion


		#region -- SetThreadNodeText --
		/// <summary>
		/// Sets the thread node text.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="Text">The text.</param>
		/// <returns></returns>
		public ForumThreadNodeInfo SetThreadNodeText(ForumThreadNodeInfo info, string Text)
		{
			return SetThreadNodeText(info.Id, Text);
		}

		/// <summary>
		/// Sets the thread node text.
		/// </summary>
		/// <param name="NodeId">The node id.</param>
		/// <param name="Text">The text.</param>
		/// <returns></returns>
		public ForumThreadNodeInfo SetThreadNodeText(int NodeId, string Text)
		{
			DBForum.SetThreadNodeText(NodeId, Text);
			return this.GetForumThreadNode(NodeId);
		}
		#endregion

		#region -- DeleteForum --
		/// <summary>
		/// Deletes the forum.
		/// </summary>
		public void DeleteForum()
		{
			DBForum.DeleteForumByContainerKey(_ownerContainer.Key);
		}

		/// <summary>
		/// Deletes the forum.
		/// </summary>
		/// <param name="info">The info.</param>
		public void DeleteForum(ForumInfo info)
		{
			DeleteForum(info.Id);
		}

		/// <summary>
		/// Deletes the forum.
		/// </summary>
		/// <param name="ForumId">The forum id.</param>
		public void DeleteForum(int ForumId)
		{
			DBForum.DeleteForumById(ForumId);
		}

		#endregion

		#region -- DeleteForumThread --
		/// <summary>
		/// Deletes the forum thread.
		/// </summary>
		/// <param name="info">The info.</param>
		public void DeleteForumThread(ForumInfo info)
		{
			DBForum.DeleteForumThreadByForumId(info.Id);
		}

		/// <summary>
		/// Deletes the forum thread.
		/// </summary>
		/// <param name="info">The info.</param>
		public void DeleteForumThread(ForumThreadInfo info)
		{
			DeleteForumThread(info.Id);
		}

		/// <summary>
		/// Deletes the forum thread.
		/// </summary>
		/// <param name="ThreadId">The thread id.</param>
		public void DeleteForumThread(int ThreadId)
		{
			DBForum.DeleteForumThreadById(ThreadId);
		}

		#endregion

		#region -- DeleteForumThread --
		/// <summary>
		/// Deletes the forum thread node.
		/// </summary>
		/// <param name="info">The info.</param>
		public void DeleteForumThreadNode(ForumThreadInfo info)
		{
			DBForum.DeleteForumThreadNodeByThreadId(info.Id);
		}

		/// <summary>
		/// Deletes the forum thread node.
		/// </summary>
		/// <param name="info">The info.</param>
		public void DeleteForumThreadNode(ForumThreadNodeInfo info)
		{
			DeleteForumThreadNode(info.Id);
		}

		/// <summary>
		/// Deletes the forum thread node.
		/// </summary>
		/// <param name="NodeId">The node id.</param>
		public void DeleteForumThreadNode(int NodeId)
		{
			DBForum.DeleteForumThreadNodeById(NodeId);
		}

		#endregion

		#region -- Delete --
		/// <summary>
		/// Deletes this instance.
		/// </summary>
		public void Delete()
		{
			DBForum.DeleteForumsByContainerKey(this._ownerContainer.Key);
		}
		#endregion


	}
}
