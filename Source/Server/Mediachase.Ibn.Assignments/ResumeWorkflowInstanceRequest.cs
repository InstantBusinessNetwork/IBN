using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents resume workflow instance request.
	/// </summary>
	public class ResumeWorkflowInstanceRequest : Request
	{
		#region Const
		#endregion

		#region Properties
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="SuspendWorkflowInstanceRequest"/> class.
		/// </summary>
		public ResumeWorkflowInstanceRequest()
			: base(WorkflowInstanceMethod.Resume, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SuspendWorkflowInstanceRequest"/> class.
		/// </summary>
		/// <param name="workflowInstanceId">The workflow instance id.</param>
		public ResumeWorkflowInstanceRequest(PrimaryKeyId workflowInstanceId)
			: base(WorkflowInstanceMethod.Resume, new WorkflowInstanceEntity(workflowInstanceId))
		{
		}
		#endregion

		#region Methods
		#endregion
	}
}
