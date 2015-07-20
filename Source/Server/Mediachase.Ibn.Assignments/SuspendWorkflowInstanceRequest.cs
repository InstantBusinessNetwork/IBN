using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents suspend workflow instance request.
	/// </summary>
	public class SuspendWorkflowInstanceRequest: Request
	{
		#region Const	
		#endregion 

		#region Properties	
		#endregion 

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="SuspendWorkflowInstanceRequest"/> class.
		/// </summary>
		public SuspendWorkflowInstanceRequest():base(WorkflowInstanceMethod.Suspend, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SuspendWorkflowInstanceRequest"/> class.
		/// </summary>
		/// <param name="workflowInstanceId">The workflow instance id.</param>
		public SuspendWorkflowInstanceRequest(PrimaryKeyId workflowInstanceId)
			: base(WorkflowInstanceMethod.Suspend, new WorkflowInstanceEntity(workflowInstanceId))
		{
		}
		#endregion
	
		#region Methods
		#endregion 
	}
}
