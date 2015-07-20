using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Assignments;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Utility class to update WorkflowInstance and  Assignment Time Status.
	/// </summary>
	public static class BusinessProcessSheduler
	{
		/// <summary>
		/// Gets the auto complete default comment.
		/// </summary>
		/// <returns></returns>
		private static string GetAutoCompleteDefaultComment()
		{
			return "Closed Automatically by Workflow";
		}

		/// <summary>
		/// Processes this instance.
		/// </summary>
		public static void Process()
		{
			// Step 1. Process WorkflowInstanceEntity
			// Step 1-1. Set Overdue
			foreach (WorkflowInstanceEntity entity in BusinessManager.List(WorkflowInstanceEntity.ClassName,
				new FilterElement[] { 
					FilterElement.EqualElement(WorkflowInstanceEntity.FieldState, (int)BusinessProcessState.Active),
					FilterElement.IsNullElement(WorkflowInstanceEntity.FieldTimeStatus),
					new FilterElement(WorkflowInstanceEntity.FieldPlanFinishDate, FilterElementType.LessOrEqual, DateTime.UtcNow)
				}))
			{
				BusinessManager.Update(entity);
			}

			// Step 1-2. Reset Overdue
			foreach (WorkflowInstanceEntity entity in BusinessManager.List(WorkflowInstanceEntity.ClassName,
				new FilterElement[] { 
					FilterElement.EqualElement(WorkflowInstanceEntity.FieldState, (int)BusinessProcessState.Active),
					FilterElement.EqualElement(WorkflowInstanceEntity.FieldTimeStatus, (int)WorkflowInstanceTimeStatus.OverDue),
					new FilterElement(WorkflowInstanceEntity.FieldPlanFinishDate, FilterElementType.Greater, DateTime.UtcNow)
				}))
			{
				BusinessManager.Update(entity);
			}

			// Step 2. Process AssignmentEntity
			// Step 2-1. Set Overdue
			foreach (AssignmentEntity entity in BusinessManager.List(AssignmentEntity.ClassName,
				new FilterElement[] { 
					FilterElement.EqualElement(AssignmentEntity.FieldState, (int)AssignmentState.Active),
					FilterElement.IsNullElement(AssignmentEntity.FieldTimeStatus),
					new FilterElement(AssignmentEntity.FieldPlanFinishDate, FilterElementType.LessOrEqual, DateTime.UtcNow)
				}))
			{
				// Process Auto Complete
				if (entity.WorkflowInstanceId.HasValue)
				{
					WorkflowInstanceEntity wfInstance = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, entity.WorkflowInstanceId.Value);

					switch (WorkflowParameters.GetAssignmentOverdueAction(wfInstance))
					{
						case AssignmentOverdueAction.NoAction:
							BusinessManager.Update(entity);
							break;
						case AssignmentOverdueAction.AutoCompleteWithDecline:
							CloseAssignmentRequest declineRequet = new CloseAssignmentRequest(entity.PrimaryKeyId.Value, (int)AssignmentExecutionResult.Declined);

							declineRequet.Comment = WorkflowParameters.GetAutoCompleteComment(wfInstance);
							if(string.IsNullOrEmpty(declineRequet.Comment))
								declineRequet.Comment = GetAutoCompleteDefaultComment();

							BusinessManager.Execute(declineRequet);
							break;
						case AssignmentOverdueAction.AutoCompleteWithAccept:
							CloseAssignmentRequest acceptRequet = new CloseAssignmentRequest(entity.PrimaryKeyId.Value, (int)AssignmentExecutionResult.Accepted);

							acceptRequet.Comment = WorkflowParameters.GetAutoCompleteComment(wfInstance);
							if (string.IsNullOrEmpty(acceptRequet.Comment))
								acceptRequet.Comment = GetAutoCompleteDefaultComment();

							BusinessManager.Execute(acceptRequet);
							break;
					}
				}
				else
				{
					BusinessManager.Update(entity);
				}
			}

			// Step 2-1. Reset Overdue
			foreach (AssignmentEntity entity in BusinessManager.List(AssignmentEntity.ClassName,
				new FilterElement[] { 
					FilterElement.EqualElement(AssignmentEntity.FieldState, (int)AssignmentState.Active),
					FilterElement.EqualElement(AssignmentEntity.FieldTimeStatus, (int)AssignmentTimeStatus.OverDue),
					new FilterElement(AssignmentEntity.FieldPlanFinishDate, FilterElementType.Greater, DateTime.UtcNow)
				}))
			{
				BusinessManager.Update(entity);
			}
		}
	}
}
