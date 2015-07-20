using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Assignments.Workflow
{
	/// <summary>
	/// Represents create assignment activity.
	/// </summary>
	public class CreateAssignmentActivity: ActivityElement
	{
		#region Const
		#endregion

		#region Properties
		/// <summary>
		/// Gets a value indicating whether this instance can have children.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance can have children; otherwise, <c>false</c>.
		/// </value>
		public override bool CanHaveChildren { get { return false; } }

		/// <summary>
		/// Gets or sets the prototype id.
		/// </summary>
		/// <value>The prototype id.</value>
		public string PrototypeId { get; set; }

		/// <summary>
		/// Gets or sets the user id.
		/// </summary>
		/// <value>The user id.</value>
		public int? UserId { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateAssignmentActivity"/> class.
		/// </summary>
		public CreateAssignmentActivity()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateAssignmentActivity"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public CreateAssignmentActivity(string name)
		{
			this.Name = name;
		}
		#endregion

		#region Methods
		#endregion
	} 
}
