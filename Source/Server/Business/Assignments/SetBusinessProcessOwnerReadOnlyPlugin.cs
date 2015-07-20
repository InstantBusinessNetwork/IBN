using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Assignments;
using Mediachase.IBN.Business.ControlSystem;

namespace Mediachase.IBN.Business.Assignments
{
	/// <summary>
	/// Represents sync owner read only plugin.
	/// </summary>
	public class SetBusinessProcessOwnerReadOnlyPlugin: IPlugin
	{
		#region Const
		#endregion

		#region Properties
		public BusinessContext Context { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="SetBusinessProcessOwnerReadOnlyPlugin"/> class.
		/// </summary>
		public SetBusinessProcessOwnerReadOnlyPlugin()
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Executes the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public virtual void Execute(BusinessContext context)
		{
			WorkflowInstanceEntity entityObject = (WorkflowInstanceEntity)BusinessManager.Load(context.GetTargetMetaClassName(), context.GetTargetPrimaryKeyId().Value);

			if (entityObject.State == (int)BusinessProcessState.Closed &&
				entityObject.ExecutionResult == (int)BusinessProcessExecutionResult.Accepted)
			{
				if (entityObject.OwnerDocumentId.HasValue)
				{
					Document.SetReadOnly(
						entityObject.OwnerDocumentId.Value,
						WorkflowParameters.GetOwnerReadOnly(entityObject));
				}

				// TODO: Add New Owner Here
			}
		}
		#endregion

		#region IPlugin Members

		/// <summary>
		/// Executes the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		void IPlugin.Execute(BusinessContext context)
		{
			this.Context = context;

			this.Execute(context);
		}

		#endregion
	}
}
