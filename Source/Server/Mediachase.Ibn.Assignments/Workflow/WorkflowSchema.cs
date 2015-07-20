using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Assignments.Workflow
{
	/// <summary>
	/// Represents workflow schema.
	/// </summary>
	public class WorkflowSchema
	{
		#region Const
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the instance id.
		/// </summary>
		/// <value>The instance id.</value>
		public Guid InstanceId { get; set; }

		/// <summary>
		/// Gets or sets the activity.
		/// </summary>
		/// <value>The activity.</value>
		public ActivityElement Activity { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="WorkflowSchema"/> class.
		/// </summary>
		public WorkflowSchema()
		{
			this.InstanceId = Guid.NewGuid();
		}
		#endregion

		#region Methods
		#endregion

		
	} 
}
