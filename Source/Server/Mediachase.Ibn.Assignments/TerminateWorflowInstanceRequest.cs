using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents terminate workflow instance request.
	/// </summary>
	public class TerminateWorkflowInstanceRequest : Request
	{
		#region Const
		#endregion

		#region Properties
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="TerminateWorkflowInstanceRequest"/> class.
		/// </summary>
		public TerminateWorkflowInstanceRequest()
			: base(WorkflowInstanceMethod.Terminate, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TerminateWorkflowInstanceRequest"/> class.
		/// </summary>
		/// <param name="workflowInstanceId">The workflow instance id.</param>
		public TerminateWorkflowInstanceRequest(PrimaryKeyId workflowInstanceId)
			: base(WorkflowInstanceMethod.Terminate, new WorkflowInstanceEntity(workflowInstanceId))
		{
		}
		#endregion

		#region Methods
		#endregion
	}
}
