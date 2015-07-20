using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business.ReportSystem
{
	/// <summary>
	/// Represent report access entry.
	/// </summary>
	[Serializable]
	public class ReportAce
	{
		private int _id;

		private bool _allow;
		private int? _principalId;
		private string _role;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportAce"/> class.
		/// </summary>
		/// <param name="role">The role.</param>
		/// <param name="action">The action.</param>
		/// <param name="allow">if set to <c>true</c> [allow].</param>
		public ReportAce(string role, bool allow):this(0,role,null,allow)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportAce"/> class.
		/// </summary>
		/// <param name="principalId">The principal id.</param>
		/// <param name="action">The action.</param>
		/// <param name="allow">if set to <c>true</c> [allow].</param>
		public ReportAce(int principalId,bool allow):this(0,null,principalId,allow)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportAce"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="role">The role.</param>
		/// <param name="principalId">The principal id.</param>
		/// <param name="action">The action.</param>
		/// <param name="allow">if set to <c>true</c> [allow].</param>
		/// <param name="isInternal">if set to <c>true</c> [is internal].</param>
		internal ReportAce(int id, string role,int? principalId, bool allow)
		{
			_id = id;
			_role = role;
			_principalId = principalId;
			_allow = allow;
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
		public int? PrincipalId
		{
			get
			{
				return _principalId;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="ReportAce"/> is allow.
		/// </summary>
		/// <value><c>true</c> if allow; otherwise, <c>false</c>.</value>
		public bool Allow
		{
			get
			{
				return _allow;
			}
		}
	}
}
