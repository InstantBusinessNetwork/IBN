using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Assignments;
using Mediachase.IBN.Business.ControlSystem;

namespace Mediachase.IBN.Business.Assignments
{
	/// <summary>
	/// Represents reset business process owner read only plugin.
	/// </summary>
	public class ResetBusinessProcessOwnerReadOnlyPlugin: IPlugin
	{
		#region Const
		#endregion

		#region Properties
		public BusinessContext Context { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ResetBusinessProcessOwnerReadOnlyPlugin"/> class.
		/// </summary>
		public ResetBusinessProcessOwnerReadOnlyPlugin()
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

			if (entityObject.OwnerDocumentId.HasValue)
			{
				Document.SetReadOnly(
					entityObject.OwnerDocumentId.Value,
					false);
			}

			// TODO: Add New Owner Here
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
