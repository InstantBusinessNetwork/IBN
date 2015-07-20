using System;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Represents an Access Control Entry.
	/// </summary>
	[Serializable]
	public class AccessControlEntry
	{
		private int _id;
		private bool _isInherited;

		private bool _allow;
		private string _action;
		private int _principalId;
		private string _role;

		private bool _isInternal;

		/// <summary>
		/// Initializes a new instance of the <see cref="AccessControlEntry"/> class.
		/// </summary>
		/// <param name="role">The role.</param>
		/// <param name="action">The action.</param>
		/// <param name="allow">if set to <c>true</c> [allow].</param>
		public AccessControlEntry(string role, string action, bool allow)
			: this(0, false, role, 0, action, allow, false, Guid.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AccessControlEntry"/> class.
		/// </summary>
		/// <param name="role">The role.</param>
		/// <param name="action">The action.</param>
		/// <param name="allow">if set to <c>true</c> [allow].</param>
		/// <param name="ownerKey">The owner key.</param>
		public AccessControlEntry(string role, string action, bool allow, Guid ownerKey)
			: this(0, false, role, 0, action, allow, false, ownerKey)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AccessControlEntry"/> class.
		/// </summary>
		/// <param name="principalId">The principal id.</param>
		/// <param name="action">The action.</param>
		/// <param name="allow">if set to <c>true</c> [allow].</param>
		public AccessControlEntry(int principalId, string action, bool allow)
			: this(0, false, null, principalId, action, allow, false, Guid.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AccessControlEntry"/> class.
		/// </summary>
		/// <param name="principalId">The principal id.</param>
		/// <param name="action">The action.</param>
		/// <param name="allow">if set to <c>true</c> [allow].</param>
		/// <param name="ownerKey">The owner key.</param>
		public AccessControlEntry(int principalId, string action, bool allow, Guid ownerKey)
			: this(0, false, null, principalId, action, allow, false, ownerKey)
		{
		}

		/// <summary>
		/// Internals the create.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="isInherited">if set to <c>true</c> [is inherited].</param>
		/// <param name="role">The role.</param>
		/// <param name="principalId">The principal id.</param>
		/// <param name="action">The action.</param>
		/// <param name="allow">if set to <c>true</c> [allow].</param>
		/// <param name="isInternal">if set to <c>true</c> [is internal].</param>
		/// <returns></returns>
		internal static AccessControlEntry InternalCreate(int id, bool isInherited, string role,int principalId,string action, bool allow, bool isInternal, Guid ownerKey)
		{
			return new AccessControlEntry(id, isInherited, role,principalId,action, allow, isInternal, ownerKey);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AccessControlEntry"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="isInherited">if set to <c>true</c> [is inherited].</param>
		/// <param name="role">The role.</param>
		/// <param name="principalId">The principal id.</param>
		/// <param name="action">The action.</param>
		/// <param name="allow">if set to <c>true</c> [allow].</param>
		/// <param name="isInternal">if set to <c>true</c> [is internal].</param>
		protected AccessControlEntry(int id, bool isInherited, string role,int principalId,string action, bool allow, bool isInternal, Guid ownerKey)
		{
			_id = id;
			_isInherited = isInherited;
			_role = role;
			_principalId = principalId;
			_action = action;
			_allow = allow;
			_isInternal = isInternal;
			this.OwnerKey = ownerKey;
		}

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		public int Id
		{
			get
			{
				return _id;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is iherited.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is iherited; otherwise, <c>false</c>.
		/// </value>
		public bool IsIherited
		{
			get
			{
				return _isInherited;
			}
		}

		/// <summary>
		/// Sets the is inherited.
		/// </summary>
		/// <param name="isInherited">if set to <c>true</c> [is inherited].</param>
		internal void SetIsInherited(bool isInherited)
		{
			_isInherited = isInherited;
		}

		/// <summary>
		/// Gets a value indicating whether this instance is role ace.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is role ace; otherwise, <c>false</c>.
		/// </value>
		public bool IsRoleAce
		{
			get
			{
				return _role!=null;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is principal ace.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is principal ace; otherwise, <c>false</c>.
		/// </value>
		public bool IsPrincipalAce
		{
			get
			{
				return !this.IsRoleAce;
			}
		}

		/// <summary>
		/// Gets the role.
		/// </summary>
		/// <value>The role.</value>
		public string Role
		{
			get
			{
				return _role;
			}
		}

		/// <summary>
		/// Gets the principal id.
		/// </summary>
		/// <value>The principal id.</value>
		public int PrincipalId
		{
			get
			{
				return _principalId;
			}
		}

		/// <summary>
		/// Gets the action.
		/// </summary>
		/// <value>The action.</value>
		public string Action
		{
			get
			{
				return _action;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="AccessControlEntry"/> is allow.
		/// </summary>
		/// <value><c>true</c> if allow; otherwise, <c>false</c>.</value>
		public bool Allow
		{
			get
			{
				return _allow;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is internal.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is internal; otherwise, <c>false</c>.
		/// </value>
		public bool IsInternal
		{
			get
			{
				return _isInternal;
			}
		}

		/// <summary>
		/// Gets or sets the owner key.
		/// </summary>
		/// <value>The owner key.</value>
		public Guid OwnerKey { get; private set; }
	}
}
