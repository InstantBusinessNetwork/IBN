using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Assignments.Workflow
{
	public static class WorkflowRuntime
	{
		/// <summary>
		/// Starts the workflow.
		/// </summary>
		/// <param name="worflowDef">The worflow def.</param>
		/// <returns></returns>
		public static Guid StartWorkflow(WorkflowDefinitionEntity worflowDef)
		{
			WorkflowSchema schema = McXmlSerializer.GetObject<WorkflowSchema>(worflowDef.Xaml);

			return schema.InstanceId;
		}

		/// <summary>
		/// Modifies the workflow.
		/// </summary>
		/// <param name="workflow">The workflow.</param>
		/// <param name="modification">The modification.</param>
		public static void ModifyWorkflow(WorkflowSchema workflow, WorkflowSchema modification)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Runs the workflow.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public static void ProcessWorkflowRules(AssignmentEntity entity)
		{
			throw new NotImplementedException();
		}
	}
}
