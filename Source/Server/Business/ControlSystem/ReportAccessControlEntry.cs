using System;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Represents an Report Access Control Entry.
	/// </summary>
	[Serializable]
	public class ReportAccessControlEntry
	{
		private int _id;

		private bool _allow;
		private string _action;
		private int _principalId;
		private string _role;

		private bool _isInternal;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportAccessControlEntry"/> class.
		/// </summary>
		/// <param name="role">The role.</param>
		/// <param name="action">The action.</param>
		/// <param name="allow">if set to <c>true</c> [allow].</param>
		public ReportAccessControlEntry(string role,string action, bool allow):this(0,role,0,action,allow,false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportAccessControlEntry"/> class.
		/// </summary>
		/// <param name="principalId">The principal id.</param>
		/// <param name="action">The action.</param>
		/// <param name="allow">if set to <c>true</c> [allow].</param>
		public ReportAccessControlEntry(int principalId,string action, bool allow):this(0,null,principalId,action,allow,false)
		{
		}

		/// <summary>
		/// Internals the create.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="role">The role.</param>
		/// <param name="principalId">The principal id.</param>
		/// <param name="action">The action.</param>
		/// <param name="allow">if set to <c>true</c> [allow].</param>
		/// <param name="isInternal">if set to <c>true</c> [is internal].</param>
		/// <returns></returns>
		internal static ReportAccessControlEntry InternalCreate(int id, string role,int principalId,string action, bool allow, bool isInternal)
		{
			return new ReportAccessControlEntry(id, role,principalId,action, allow, isInternal);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportAccessControlEntry"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="role">The role.</param>
		/// <param name="principalId">The principal id.</param>
		/// <param name="action">The action.</param>
		/// <param name="allow">if set to <c>true</c> [allow].</param>
		/// <param name="isInternal">if set to <c>true</c> [is internal].</param>
		protected ReportAccessControlEntry(int id, string role,int principalId,string action, bool allow, bool isInternal)
		{
			_id = id;
			_role = role;
			_principalId = principalId;
			_action = action;
			_allow = allow;
			_isInternal = isInternal;
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
		/// Gets a value indicating whether this <see cref="ReportAccessControlEntry"/> is allow.
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
	}
}
