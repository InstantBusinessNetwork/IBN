using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Sql;
using System.Data;
using System.Diagnostics;

namespace Mediachase.Ibn.Assignments
{
    public class AssignmentRequestHandler : BusinessObjectRequestHandler
	{
        #region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="IbnClientMessageRequestHandler"/> class.
		/// </summary>
        public AssignmentRequestHandler()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods

		/// <summary>
		/// Updates the time status.
		/// </summary>
		/// <param name="context">The context.</param>
		private void UpdateTimeStatus(BusinessContext context)
		{
			AssignmentEntity entity = context.Request.Target as AssignmentEntity;

			if (entity != null)
			{
				entity.TimeStatus = null;

				DateTime dateTimeNow = DataContext.Current.CurrentUserTimeZone.ToLocalTime(DateTime.UtcNow);

				if (entity.State == (int)AssignmentState.Active)
				{
					if (entity.PlanFinishDate.HasValue && entity.PlanFinishDate.Value < dateTimeNow)
					{
						entity.TimeStatus = (int)AssignmentTimeStatus.OverDue;
					}
				}
				else if (entity.State == (int)AssignmentState.Pending)
				{
					if (entity.PlanStartDate.HasValue && entity.PlanFinishDate.Value < dateTimeNow)
					{
						entity.TimeStatus = (int)AssignmentTimeStatus.OverStart;
					}
				}
			}
		}

		#region CreateEntityObject
		/// <summary>
		/// Creates the entity object.
		/// </summary>
		/// <param name="metaClassName">Name of the meta class.</param>
		/// <param name="primaryKeyId">The primary key id.</param>
		/// <returns></returns>
		protected override EntityObject CreateEntityObject(string metaClassName, PrimaryKeyId? primaryKeyId)
		{
            if (metaClassName == AssignmentEntity.ClassName)
			{
                AssignmentEntity retVal = new AssignmentEntity();
				retVal.PrimaryKeyId = primaryKeyId;
				return retVal;
			}

			return base.CreateEntityObject(metaClassName, primaryKeyId);
		}
		#endregion

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

		#region Update
		protected override void PreUpdateInsideTransaction(BusinessContext context)
		{
			base.PreUpdateInsideTransaction(context);

			UpdateTimeStatus(context);
		}		
		#endregion

		#region Delete
		/// <summary>
		/// Pres the delete inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreDeleteInsideTransaction(BusinessContext context)
		{
			base.PreDeleteInsideTransaction(context);

			SqlHelper.ExecuteNonQuery(SqlContext.Current, System.Data.CommandType.StoredProcedure, 
				"bus_cls_AssignmentDelete", 
				SqlHelper.SqlParameter("@AssignmentId", SqlDbType.UniqueIdentifier, (Guid)context.GetTargetPrimaryKeyId().Value));
		}
		#endregion

        #region Custom Methods
        protected override void CustomMethod(BusinessContext context)
        {
            switch (context.GetMethod())
            {
                case AssignmentRequestMethod.Close:
					CloseAssignment(context);
                    break;
				case AssignmentRequestMethod.Activate:
					ActivateAssignment(context);
					break;
				case AssignmentRequestMethod.Resume:
					ResumeAssignment(context);
					break;
				case AssignmentRequestMethod.Suspend:
					SuspendAssignment(context);
					break;
				case AssignmentRequestMethod.AssignUser:
					AssignUser(context);
					break;
				case AssignmentRequestMethod.Reactivate:
					Reactivate(context);
					break;
			}
        }
        #endregion

		#region AssignUser
		private void AssignUser(BusinessContext context)
		{
			throw new NotImplementedException();			
		}
		#endregion

		#region Activate
		private void Reactivate(BusinessContext context)
		{
			// Load Assignment
			MetaObject metaObject = MetaObjectActivator.CreateInstance(AssignmentEntity.ClassName, context.GetTargetPrimaryKeyId().Value);

			if (((int)metaObject[AssignmentEntity.FieldState]) != (int)AssignmentState.Closed)
				throw new InvalidOperationException();

			// Load Meta Object
			metaObject[AssignmentEntity.FieldState] = (int)AssignmentState.Active;
			metaObject[AssignmentEntity.FieldActualFinishDate] = null;
			metaObject[AssignmentEntity.FieldActualWork] = null;

			// Save Meta Object
			metaObject.Save();

			//if (entity.WorkflowInstanceId.HasValue)
			//{
			//    // Run Workflow
			//    RunWorkflow(entity);
			//}

			context.SetResponse(new Response());
		}
        #endregion

		#region Activate
		/// <summary>
		/// Activates the assignment.
		/// </summary>
		/// <param name="context">The context.</param>
		private void ActivateAssignment(BusinessContext context)
		{
			// Load Assignment
			MetaObject metaObject = MetaObjectActivator.CreateInstance(AssignmentEntity.ClassName, context.GetTargetPrimaryKeyId().Value);

			if (((int)metaObject[AssignmentEntity.FieldState]) != (int)AssignmentState.Pending)
				throw new InvalidOperationException();

			// Load Meta Object
			DateTime dateTimeNow = DataContext.Current.CurrentUserTimeZone.ToLocalTime(DateTime.UtcNow);
			
			metaObject[AssignmentEntity.FieldState] = (int)AssignmentState.Active;
			metaObject[AssignmentEntity.FieldActualStartDate] = dateTimeNow;

			// Save Meta Object
			metaObject.Save();

			//if (entity.WorkflowInstanceId.HasValue)
			//{
			//    // Run Workflow
			//    RunWorkflow(entity);
			//}

			context.SetResponse(new Response());
		}
		#endregion

		#region Resume
		/// <summary>
		/// Resumes the assignment.
		/// </summary>
		/// <param name="context">The context.</param>
		private void ResumeAssignment(BusinessContext context)
		{
			// Load Assignment
			MetaObject metaObject = MetaObjectActivator.CreateInstance(AssignmentEntity.ClassName, context.GetTargetPrimaryKeyId().Value);

			if (((int)metaObject[AssignmentEntity.FieldState]) != (int)AssignmentState.Suspended)
				throw new InvalidOperationException();

			// Load Meta Object
			metaObject[AssignmentEntity.FieldState] = (int)AssignmentState.Active;

			// Save Meta Object
			metaObject.Save();

			if (metaObject[AssignmentEntity.FieldWorkflowInstanceId] != null)
			{
				// Run Workflow
				AssignmentEntity entity = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, context.GetTargetPrimaryKeyId().Value);
				RunWorkflow(entity);
			}


			context.SetResponse(new Response());
		}
		#endregion

		#region Resume
		/// <summary>
		/// Suspends the assignment.
		/// </summary>
		/// <param name="context">The context.</param>
		private void SuspendAssignment(BusinessContext context)
		{
			// Load Assignment
			MetaObject metaObject = MetaObjectActivator.CreateInstance(AssignmentEntity.ClassName, context.GetTargetPrimaryKeyId().Value);

			if (((int)metaObject[AssignmentEntity.FieldState]) != (int)AssignmentState.Active)
				throw new InvalidOperationException();

			// Load Meta Object
			
			metaObject[AssignmentEntity.FieldState] = (int)AssignmentState.Suspended;

			// Save Meta Object
			metaObject.Save();

			if (metaObject[AssignmentEntity.FieldWorkflowInstanceId] != null)
			{
				// Run Workflow
				AssignmentEntity entity = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, context.GetTargetPrimaryKeyId().Value);
				RunWorkflow(entity);
			}


			context.SetResponse(new Response());
		}
		#endregion

        #region Complete
		/// <summary>
		/// Closes the assignment.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void CloseAssignment(BusinessContext context)
        {
			CloseAssignmentRequest request = (CloseAssignmentRequest)context.Request;

			// Load Assignment
			MetaObject metaObject = MetaObjectActivator.CreateInstance(AssignmentEntity.ClassName, context.GetTargetPrimaryKeyId().Value);

			if (((int)metaObject[AssignmentEntity.FieldState]) != (int)AssignmentState.Active)
				throw new InvalidOperationException();

			DateTime dateTimeNow = DataContext.Current.CurrentUserTimeZone.ToLocalTime(DateTime.UtcNow);

			// Update Meta Object Fields
			metaObject[AssignmentEntity.FieldClosedBy] = (int)DataContext.Current.CurrentUserId;
			metaObject[AssignmentEntity.FieldState] = (int)AssignmentState.Closed;
			metaObject[AssignmentEntity.FieldExecutionResult] = request.ExecutionResult;
			metaObject[AssignmentEntity.FieldComment] = request.Comment;
			metaObject[AssignmentEntity.FieldActualFinishDate] = dateTimeNow;

			// Save Meta Object
			metaObject.Save();

			if (metaObject[AssignmentEntity.FieldWorkflowInstanceId]!=null)
			{
				// Run Workflow
				AssignmentEntity entity = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, context.GetTargetPrimaryKeyId().Value);
				RunWorkflow(entity);
			}

			context.SetResponse(new Response());
        }

		/// <summary>
		/// Runs the workflow.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="executionResult">The execution result.</param>
		private static void RunWorkflow(AssignmentEntity entity)
		{
			Guid wfInstanceId = (Guid)entity.WorkflowInstanceId;
			string queueName = entity.WorkflowActivityName;

			WorkflowInstance instance = GlobalWorkflowRuntime.WorkflowRuntime.GetWorkflow(wfInstanceId);

			instance.EnqueueItem(queueName, entity, null, null);

			GlobalWorkflowRuntime.RunWorkflow(instance);
		}      
        #endregion


		#endregion
	}
}
