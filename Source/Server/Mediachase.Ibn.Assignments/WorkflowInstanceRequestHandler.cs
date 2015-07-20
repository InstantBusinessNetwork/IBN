using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;
using System.Workflow.Activities;
using System.IO;
using System.Xml;
using Mediachase.Ibn.Data.Sql;
using System.Data;

namespace Mediachase.Ibn.Assignments
{
	public class WorkflowInstanceRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="IbnClientMessageRequestHandler"/> class.
		/// </summary>
		public WorkflowInstanceRequestHandler()
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
			if (metaClassName == WorkflowInstanceEntity.ClassName)
			{
				WorkflowInstanceEntity retVal = new WorkflowInstanceEntity();
				retVal.PrimaryKeyId = primaryKeyId;
				return retVal;
			}

			return base.CreateEntityObject(metaClassName, primaryKeyId);
		}
		#endregion

		/// <summary>
		/// Initializes the entity.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void InitializeEntity(BusinessContext context)
		{
			base.InitializeEntity(context);

			((WorkflowInstanceEntity)((InitializeEntityResponse)context.Response).EntityObject).Xaml = McWorkflowSerializer.GetString(new SequentialWorkflowActivity());
		}

		private void UpdateTimeStatus(BusinessContext context)
		{
			WorkflowInstanceEntity entity = context.Request.Target as WorkflowInstanceEntity;

			if (entity != null)
			{
				entity.TimeStatus = null;

				if (entity.State == (int)BusinessProcessState.Active)
				{
					DateTime dateTimeNow = DataContext.Current.CurrentUserTimeZone.ToLocalTime(DateTime.UtcNow);

					if (entity.PlanFinishDate.HasValue && entity.PlanFinishDate.Value < dateTimeNow)
					{
						entity.TimeStatus = (int)WorkflowInstanceTimeStatus.OverDue;
					}
				}
				//else if (entity.State == (int)BusinessProcessState.Pending)
				//{
				//    if (entity.PlanStartDate.HasValue && entity.PlanFinishDate.Value < DateTime.Now)
				//    {
				//        entity.TimeStatus = (int)WorkflowInstanceTimeStatus.OverStart;
				//    }
				//}
			}
		}

		#region Create
		/// <summary>
		/// Pres the create inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreCreateInsideTransaction(BusinessContext context)
		{
			base.PreCreateInsideTransaction(context);

			UpdateTimeStatus(context);
		}
		#endregion

		#region Upate
		/// <summary>
		/// Pres the update inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreUpdateInsideTransaction(BusinessContext context)
		{
			base.PreUpdateInsideTransaction(context);

			UpdateTimeStatus(context);
		}
		#endregion

		#region Delete
		protected override void PreDeleteInsideTransaction(BusinessContext context)
		{
			// Delete assignments
			foreach (AssignmentEntity entity in 
				BusinessManager.List(AssignmentEntity.ClassName, 
					new FilterElement[] { 
						FilterElement.EqualElement(AssignmentEntity.FieldWorkflowInstanceId, context.GetTargetPrimaryKeyId().Value) 
					}))
			{
				BusinessManager.Delete(entity);
			}

			// Delete history
			SqlHelper.ExecuteNonQuery(SqlContext.Current,
				System.Data.CommandType.StoredProcedure,
				"HistoryEntityDelete",
				SqlHelper.SqlParameter("@ClassName", SqlDbType.NVarChar, 250, WorkflowInstanceEntity.ClassName),
				SqlHelper.SqlParameter("@ObjectId", SqlDbType.VarChar, 36, context.GetTargetPrimaryKeyId().Value.ToString()));

			base.PreDeleteInsideTransaction(context);
		}
		#endregion

		#region CopyEntityObjectToMetaObject
		protected override void CopyEntityObjectToMetaObject(EntityObject target, Mediachase.Ibn.Data.Meta.MetaObject metaObject)
		{
			base.CopyEntityObjectToMetaObject(target, metaObject);

			// OZ 2009-06-04 Duration Fix
			TimeType timeType = (TimeType)(int)metaObject[WorkflowInstanceEntity.FieldPlanFinishTimeType];

			if (timeType == TimeType.Duration && 
				metaObject[WorkflowInstanceEntity.FieldPlanDuration] == null)
			{
				metaObject[WorkflowInstanceEntity.FieldPlanFinishTimeType] = (int)TimeType.NotSet;
				timeType = TimeType.NotSet;
			}
			else if (timeType == TimeType.DateTime && 
				metaObject[WorkflowInstanceEntity.FieldPlanFinishDate] == null)
			{
				metaObject[WorkflowInstanceEntity.FieldPlanFinishTimeType] = (int)TimeType.NotSet;
				timeType = TimeType.NotSet;
			}

			// Recalculate Plan Date
			if (timeType == TimeType.NotSet)
			{
				metaObject[WorkflowInstanceEntity.FieldPlanDuration] = null;
				metaObject[WorkflowInstanceEntity.FieldPlanFinishDate] = null;
			}
			else if (timeType == TimeType.Duration)
			{
				DateTime? actualStartDate = (DateTime?)metaObject[WorkflowInstanceEntity.FieldActualStartDate];
				int duration = (int)metaObject[WorkflowInstanceEntity.FieldPlanDuration];

				if (actualStartDate.HasValue)
				{
					metaObject[WorkflowInstanceEntity.FieldPlanFinishDate] = actualStartDate.Value.AddMinutes(duration);
				}
				else
				{
					metaObject[WorkflowInstanceEntity.FieldPlanFinishDate] = null;
				}
			}
			else if (timeType == TimeType.DateTime)
			{
				DateTime? actualStartDate = (DateTime?)metaObject[WorkflowInstanceEntity.FieldActualStartDate];
				DateTime planFinishDate = (DateTime)metaObject[WorkflowInstanceEntity.FieldPlanFinishDate];

				if (actualStartDate.HasValue)
				{
					metaObject[WorkflowInstanceEntity.FieldPlanDuration] = (int)(planFinishDate - actualStartDate.Value).TotalMinutes;
				}
				else
				{
					metaObject[WorkflowInstanceEntity.FieldPlanDuration] = null;
				}
			}
			//

		}
		#endregion

		#region Custom
		protected override void CustomMethod(BusinessContext context)
		{
			switch (context.GetMethod())
			{
				case WorkflowInstanceMethod.Start:
					Start(context);
					break;
				case WorkflowInstanceMethod.Resume:
					Resume(context);
					break;
				case WorkflowInstanceMethod.Suspend:
					Suspend(context);
					break;
				case WorkflowInstanceMethod.Terminate:
					Terminate(context);
					break;
			}
		}

		/// <summary>
		/// Starts the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void Start(BusinessContext context)
		{
			// Load WorkflowInstanceEntity
			WorkflowInstanceEntity wf = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, context.GetTargetPrimaryKeyId().Value);

			// Check Wf State
			if (wf.State != (int)BusinessProcessState.Pending)
				throw new InvalidOperationException();

			WorkflowInstance instance = CreateWorkflow((Guid)wf.PrimaryKeyId.Value, wf.Xaml);
			instance.Start();
			GlobalWorkflowRuntime.RunWorkflow(instance);

			//wf.State = (int)BusinessProcessState.Active;
			//BusinessManager.Update(wf);
		}

		/// <summary>
		/// Creates the workflow.
		/// </summary>
		/// <param name="worflowDef">The worflow def.</param>
		/// <returns></returns>
		private static WorkflowInstance CreateWorkflow(Guid instanceId, string worflowDefinition)
		{
			using (StringReader workflowDefReader = new StringReader(worflowDefinition))
			{
				using (XmlReader workflowDefXmlReader = XmlReader.Create(workflowDefReader))
				{
					WorkflowInstance instance = GlobalWorkflowRuntime.WorkflowRuntime.CreateWorkflow(workflowDefXmlReader, null, null, instanceId);
					return instance;
				}
			}
		}

		/// <summary>
		/// Resumes the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void Resume(BusinessContext context)
		{
			// Load WorkflowInstanceEntity
			WorkflowInstanceEntity wf = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, context.GetTargetPrimaryKeyId().Value);

			// Check Wf State
			if (wf.State != (int)BusinessProcessState.Suspended)
				throw new InvalidOperationException();

			WorkflowInstance instance = GlobalWorkflowRuntime.WorkflowRuntime.GetWorkflow((Guid)wf.PrimaryKeyId.Value);
			instance.Resume();
			GlobalWorkflowRuntime.RunWorkflow(instance);

			//wf.State = (int)BusinessProcessState.Active;
			//BusinessManager.Update(wf);
		}

		/// <summary>
		/// Suspends the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void Suspend(BusinessContext context)
		{
			//string strPrimaryKeyId = "28ad9324-85a5-4dd2-96a2-ace25bd14be4";

			//BusinessManager.Execute<CompleteAssignmentRequest, Response>(new CompleteAssignmentRequest(PrimaryKeyId.Parse(strPrimaryKeyId)));

			// Load WorkflowInstanceEntity
			WorkflowInstanceEntity wf = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, context.GetTargetPrimaryKeyId().Value);

			// Check Wf State
			if (wf.State != (int)BusinessProcessState.Active)
				throw new InvalidOperationException();

			WorkflowInstance instance = GlobalWorkflowRuntime.WorkflowRuntime.GetWorkflow((Guid)wf.PrimaryKeyId.Value);
			instance.Suspend(string.Empty);
			GlobalWorkflowRuntime.RunWorkflow(instance);

			//wf.State = (int)BusinessProcessState.Suspended;
			//BusinessManager.Update(wf);

			// Susupend All Active Assignments
		}

		/// <summary>
		/// Terminates the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void Terminate(BusinessContext context)
		{
			// Load WorkflowInstanceEntity
			WorkflowInstanceEntity wf = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, context.GetTargetPrimaryKeyId().Value);

			// Check Wf State
			if (wf.State != (int)BusinessProcessState.Active)
				throw new InvalidOperationException();

			WorkflowInstance instance = GlobalWorkflowRuntime.WorkflowRuntime.GetWorkflow((Guid)wf.PrimaryKeyId.Value);
			instance.Terminate("Cancel by user");
			GlobalWorkflowRuntime.RunWorkflow(instance);

			//wf.State = (int)BusinessProcessState.Completed;
			//wf.ExecutionResult = (int)BusinessProcessExecutionResult.Canceled;

			//BusinessManager.Update(wf);
		}
		#endregion

		#endregion
	}
}
