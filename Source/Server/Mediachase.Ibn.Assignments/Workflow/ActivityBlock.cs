using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Assignments.Workflow
{
	/// <summary>
	/// Represents base activity block.
	/// </summary>
	[Serializable]
	public class ActivityBlock : ActivityElement
	{
		#region Const
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ActivityBlock"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public ActivityBlock(string name)
		{
			this.Name = name;
			this.CompletionType = ActivityBlockCompletionType.All;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActivityBlock"/> class.
		/// </summary>
		public ActivityBlock()
		{
			this.CompletionType = ActivityBlockCompletionType.All;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public ActivityBlockType Type { get; set; }

		/// <summary>
		/// Gets or sets the type of the completion.
		/// </summary>
		/// <value>The type of the completion.</value>
		public ActivityBlockCompletionType CompletionType { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [allow control].
		/// </summary>
		/// <value><c>true</c> if [allow control]; otherwise, <c>false</c>.</value>
		public CreateAssignmentActivity ControllerActivity { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [allow control].
		/// </summary>
		/// <value><c>true</c> if [allow control]; otherwise, <c>false</c>.</value>
		public bool AllowControl { get; set; }

		/// <summary>
		/// Gets a value indicating whether this instance can have children.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance can have children; otherwise, <c>false</c>.
		/// </value>
		public override bool CanHaveChildren { get { return true; } }
		#endregion

		#region Methods
		#endregion

		
	}
}
