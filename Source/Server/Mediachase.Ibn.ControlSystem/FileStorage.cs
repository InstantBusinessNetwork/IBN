using System;
using System.Collections.Specialized;
using System.Data;

namespace Mediachase.Ibn.ControlSystem
{
	public class FileStorage : IIbnControl
	{
		private IIbnContainer		_ownerContainer = null;
		private IbnControlInfo		_info = null;
		private DirectoryInfo		_root = null;

		public FileStorage()
		{
		}

		protected int CurrentUserId
		{
			get
			{
				return -1;
			}
		}

		public IbnControlInfo Info
		{
			get
			{
				return _info;
			}
		}

		public DirectoryInfo Root
		{
			get
			{
				return _root;
			}
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

			_info =  controlInfo;

			using (IDataReader reader = DBDirectory.GetRoot(owner.Key))
			{
				if (reader.Read())
					_root = new DirectoryInfo(this, reader);
			}

			if (_root==null)
			{
				using (IDataReader reader = DBDirectory.CreateRoot(owner.Key, "root", this.CurrentUserId, DateTime.Now))
				{
					if (reader.Read())
					{
						_root = new DirectoryInfo(this, reader);
					}
				}
					
				AccessControlList rootAcl = AccessControlList.GetACL(_root.Id);

				foreach(AccessControlEntry ace in _info.DefaultAccessControlList.GetACL(_ownerContainer.Key))
					rootAcl.Add(ace);

				if(rootAcl.Count>0)
					AccessControlList.SetACL(this, rootAcl);
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
				return new string[] {"Read", "Write", "Admin"};
			}
		}

		/// <summary>
		/// Gets the base actions.
		/// </summary>
		/// <param name="Action">The action.</param>
		/// <returns></returns>
		public string[] GetBaseActions(string Action)
		{
			switch(Action.ToLower())
			{
				case "read":
					return new string[]{};
				case "write":
					return new string[]{"Read"};
				case "admin":
					return new string[]{"Read", "Write"};
			}

			return new string[]{};
		}

		/// <summary>
		/// Gets the derived actions.
		/// </summary>
		/// <param name="Action">The action.</param>
		/// <returns></returns>
		public string[] GetDerivedActions(string Action)
		{
			switch(Action.ToLower())
			{
				case "read":
					return new string[]{"Write","Admin"};
				case "write":
					return new string[]{"Admin"};
				case "admin":
					return new string[]{};
			}

			return new string[]{};
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

		#region -- GetDirectory --
		/// <summary>
		/// Gets the directory.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <returns></returns>
		public DirectoryInfo GetDirectory(int DirectoryId)
		{
			using (IDataReader reader = DBDirectory.GetById(DirectoryId))
			{
				if(reader.Read())
					return new DirectoryInfo(this, reader);
				else
					return null;
			}
		}

		internal static DirectoryInfo InnerGetDirectory(int DirectoryId)
		{
			using (IDataReader reader = DBDirectory.GetById(DirectoryId))
			{
				if(reader.Read())
					return new DirectoryInfo(null, reader);
				else
					return null;
			}
		}
		#endregion
	}
}
