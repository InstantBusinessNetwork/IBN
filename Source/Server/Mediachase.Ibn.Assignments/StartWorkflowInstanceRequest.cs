using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents start workflow instance request.
	/// </summary>
	public class StartWorkflowInstanceRequest: Request
	{
		#region Const	
		#endregion 

		#region Properties	
		#endregion 

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="StartWorkflowInstanceRequest"/> class.
		/// </summary>
		public StartWorkflowInstanceRequest():base(WorkflowInstanceMethod.Start, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StartWorkflowInstanceRequest"/> class.
		/// </summary>
		/// <param name="workflowInstanceId">The workflow instance id.</param>
		public StartWorkflowInstanceRequest(PrimaryKeyId workflowInstanceId)
			: base(WorkflowInstanceMethod.Start, new WorkflowInstanceEntity(workflowInstanceId))
		{
		}
		#endregion
	
		#region Methods
		#endregion 
	}
}
