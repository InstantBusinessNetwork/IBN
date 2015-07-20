using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Business.Directory
{
	/// <summary>
	/// Represents MoveTreeNode request.
	/// </summary>
	public class MoveTreeNodeRequest: Request
	{
		#region Const
		#endregion

		#region Fields
		private PrimaryKeyId _newParent;
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="MoveTreeNodeRequest"/> class.
		/// </summary>
		/// <param name="target">The target.</param>
		public MoveTreeNodeRequest(DirectoryOrganizationalUnitEntity target): 
			base(DirectoryOrganizationalUnitMethod.Move, target)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MoveTreeNodeRequest"/> class.
		/// </summary>
		public MoveTreeNodeRequest():
			base(DirectoryOrganizationalUnitMethod.Move, new DirectoryOrganizationalUnitEntity())
		{
		}
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the new parent.
		/// </summary>
		/// <value>The new parent.</value>
		public PrimaryKeyId NewParent
		{
			get { return _newParent; }
			set { _newParent = value; }
		}
	
		#endregion

		#region Methods
		#endregion
	}
}
