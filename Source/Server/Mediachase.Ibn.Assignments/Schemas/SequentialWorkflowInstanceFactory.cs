using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Assignments.Schemas;
using System.Workflow.Activities;

namespace Mediachase.Ibn.Assignments.Schemas
{
	/// <summary>
	/// Represents sequential workflow instance factory.
	/// </summary>
	public class SequentialWorkflowInstanceFactory : ObjectInstanceFactory
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="SequentialWorkflowInstanceFactory"/> class.
		/// </summary>
		public SequentialWorkflowInstanceFactory()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <returns></returns>
		public override object CreateInstance()
		{
			return new SequentialWorkflowActivity("SequentialWorkflowActivity_Root");
		}
		#endregion
	}
}
