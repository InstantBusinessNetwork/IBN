using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;
using System.Workflow.Activities;

namespace Mediachase.Ibn.Assignments
{
	public class WorkflowDefinitionRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="IbnClientMessageRequestHandler"/> class.
		/// </summary>
		public WorkflowDefinitionRequestHandler()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods

		#region CreateEntityObject
		/// <summary>
		/// Creates the entity object.
		/// </summary>
		/// <param name="metaClassName">Name of the meta class.</param>
		/// <param name="primaryKeyId">The primary key id.</param>
		/// <returns></returns>
		protected override EntityObject CreateEntityObject(string metaClassName, PrimaryKeyId? primaryKeyId)
		{
			if (metaClassName == WorkflowDefinitionEntity.ClassName)
			{
				WorkflowDefinitionEntity retVal = new WorkflowDefinitionEntity();
				retVal.PrimaryKeyId = primaryKeyId;
				return retVal;
			}

			return base.CreateEntityObject(metaClassName, primaryKeyId);
		}
		#endregion

		protected override void InitializeEntity(BusinessContext context)
		{
			base.InitializeEntity(context);

			((WorkflowDefinitionEntity)((InitializeEntityResponse)context.Response).EntityObject).Xaml = McWorkflowSerializer.GetString(new SequentialWorkflowActivity());
		}

		#region Create
		protected override void PreCreateInsideTransaction(BusinessContext context)
		{
			base.PreCreateInsideTransaction(context);
		}
		#endregion


		#endregion
	}
}
