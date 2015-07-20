using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents assignment auto complete plugin.
	/// </summary>
	public class AssignmentAutoCompletePlugin: IPlugin 
	{
		#region Const
		#endregion

		#region Properties
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="AutoCompletePlugin"/> class.
		/// </summary>
		public AssignmentAutoCompletePlugin()
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Executes the specified contex.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void Execute(BusinessContext context)
		{
			if ((context.GetMethod() == RequestMethod.Update || 
				context.GetMethod() == RequestMethod.Create) &&
				context.GetTargetPrimaryKeyId().HasValue)
			{
				if (context.Request.Target.Properties.GetValue<bool>(AssignmentEntity.FieldAutoComplete, false))
				{
					decimal planWork = context.Request.Target.Properties.GetValue<decimal>(AssignmentEntity.FieldPlanWork, decimal.MaxValue);
					decimal actualWork = context.Request.Target.Properties.GetValue<decimal>(AssignmentEntity.FieldActualWork, decimal.MinValue);

					if (planWork == actualWork)
					{
						// Invoke Auto Complete
						BusinessManager.Execute(new CloseAssignmentRequest(context.GetTargetPrimaryKeyId().Value, (int)AssignmentExecutionResult.Accepted));
					}
				}
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
			this.Execute(context);
		}

		#endregion
	}
}
