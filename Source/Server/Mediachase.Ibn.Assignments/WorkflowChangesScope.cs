using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel;
using System.Workflow.Runtime;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using System.Diagnostics;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents workflow changes scope.
	/// </summary>
	public class WorkflowChangesScope : IDisposable
	{
		#region Const
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the instance id.
		/// </summary>
		/// <value>The instance id.</value>
		public Guid InstanceId { get; protected set; }

		/// <summary>
		/// Gets or sets the transient workflow.
		/// </summary>
		/// <value>The transient workflow.</value>
		//public CompositeActivity TransientWorkflow { get; protected set; }
		public object TransientWorkflow { get; protected set; }

		/// <summary>
		/// Gets or sets the inner workflow changes.
		/// </summary>
		/// <value>The inner workflow changes.</value>
		private WorkflowChanges InnerWorkflowChanges { get; set; }

		/// <summary>
		/// Gets or sets the workflow entity.
		/// </summary>
		/// <value>The workflow entity.</value>
		public EntityObject WorkflowEntity { get; protected set; }

		/// <summary>
		/// Gets the schema id.
		/// </summary>
		/// <value>The schema id.</value>
		public Guid WorkflowSchemaId 
		{
			get
			{
				return (Guid)this.WorkflowEntity[WorkflowInstanceEntity.FieldSchemaId];
			}
		}

		/// <summary>
		/// Gets the name of the workflow.
		/// </summary>
		/// <value>The name of the workflow.</value>
		public string WorkflowName
		{
			get
			{
				return (string)this.WorkflowEntity[WorkflowInstanceEntity.FieldName];
			}
		}
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="WorkflowChangesScope"/> class.
		/// </summary>
		/// <param name="entity">The entity.</param>
		protected WorkflowChangesScope(WorkflowInstanceEntity entity)
		{
			this.InstanceId = entity.PrimaryKeyId.HasValue ? (Guid)entity.PrimaryKeyId.Value : Guid.Empty;
			this.WorkflowEntity = entity;

			if (this.InstanceId != Guid.Empty && TryLoadFromMsWorkflow())
			{
				// TODO: Custom Login here
			}
			else
			{
				this.TransientWorkflow = McWorkflowSerializer.GetObject<CompositeActivity>((string)this.WorkflowEntity[WorkflowInstanceEntity.FieldXaml]);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkflowChangesScope"/> class.
		/// </summary>
		/// <param name="entity">The entity.</param>
		protected WorkflowChangesScope(WorkflowDefinitionEntity entity)
		{
			this.InstanceId = entity.PrimaryKeyId.HasValue? (Guid)entity.PrimaryKeyId.Value: Guid.Empty;
			this.WorkflowEntity = entity;

			this.TransientWorkflow = McWorkflowSerializer.GetObject<CompositeActivity>((string)this.WorkflowEntity[WorkflowInstanceEntity.FieldXaml]);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkflowChangesScope"/> class.
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		protected WorkflowChangesScope(Guid instanceId)
		{
			this.InstanceId = instanceId;

			// Step 1. Try Load From MS Workflow
			if (TryLoadFromMsWorkflow())
			{
				// TODO: Custom Login here
			}
			// Step 2. Try Load From WorkflowInstanceEntity
			else if (TryLoadFromWorkflowInstanceEntity())
			{
				// TODO: Custom Login here
			}
			// Step 3. Try Load From WorkflowDefinitionEntity
			else if (TryLoadFromWorkflowDefinitionEntity())
			{
				// TODO: Custom Login here
			}
			else
				// Step 4. Throw Exception Wrong instanceId
				throw new ArgumentException("Wrong workflow instance id.", "instanceId");
		}

		/// <summary>
		/// Tries the load from workflow definition entity.
		/// </summary>
		/// <returns></returns>
		private bool TryLoadFromWorkflowDefinitionEntity()
		{
			try
			{
				this.WorkflowEntity = (WorkflowDefinitionEntity)BusinessManager.Load(WorkflowDefinitionEntity.ClassName,
					new PrimaryKeyId(this.InstanceId));

				this.TransientWorkflow = McWorkflowSerializer.GetObject<CompositeActivity>((string)this.WorkflowEntity[WorkflowInstanceEntity.FieldXaml]);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.ToString(), "Mediachase.Ibn.Assignments::TryLoadFromWorkflowInstanceEntity");
				return false;
			}

			return true;
		}

		/// <summary>
		/// Tries the load from workflow instance entity.
		/// </summary>
		/// <returns></returns>
		private bool TryLoadFromWorkflowInstanceEntity()
		{
			try
			{
				this.WorkflowEntity = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName,
					new PrimaryKeyId(this.InstanceId));

				this.TransientWorkflow = McWorkflowSerializer.GetObject<CompositeActivity>((string)this.WorkflowEntity[WorkflowInstanceEntity.FieldXaml]);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.ToString(), "Mediachase.Ibn.Assignments::TryLoadFromWorkflowInstanceEntity");
				return false;
			}

			return true;
		}

		/// <summary>
		/// Tries the load from ms workflow.
		/// </summary>
		/// <returns></returns>
		private bool TryLoadFromMsWorkflow()
		{
			try
			{
				WorkflowInstance instance = GlobalWorkflowRuntime.WorkflowRuntime.GetWorkflow(this.InstanceId);
				Activity root = instance.GetWorkflowDefinition();

				if(this.WorkflowEntity == null)
					this.WorkflowEntity = BusinessManager.Load(WorkflowInstanceEntity.ClassName, new PrimaryKeyId(this.InstanceId));

				this.InnerWorkflowChanges = new WorkflowChanges(root);
				this.TransientWorkflow = this.InnerWorkflowChanges.TransientWorkflow;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.ToString(), "Mediachase.Ibn.Assignments::TryLoadFromMsWorkflow");
				return false;
			}

			return true;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates the specified instance id.
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		/// <returns></returns>
		public static WorkflowChangesScope Create(Guid instanceId)
		{
			WorkflowChangesScope retVal = new WorkflowChangesScope(instanceId);

			return retVal;
		}

		/// <summary>
		/// Creates the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		public static WorkflowChangesScope Create(WorkflowInstanceEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			WorkflowChangesScope retVal = new WorkflowChangesScope(entity);

			return retVal;
		}

		/// <summary>
		/// Creates the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		public static WorkflowChangesScope Create(WorkflowDefinitionEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			WorkflowChangesScope retVal = new WorkflowChangesScope(entity);

			return retVal;
		}

		/// <summary>
		/// Applies the workflow changes.
		/// </summary>
		public void ApplyWorkflowChanges()
		{
			if (this.InnerWorkflowChanges != null)
			{
				// Load WorkflowInstance
				WorkflowInstance instance = GlobalWorkflowRuntime.WorkflowRuntime.GetWorkflow(this.InstanceId);

				// ApplyWorkflowChanges to WorkflowInstance
				instance.ApplyWorkflowChanges(this.InnerWorkflowChanges);

				// ApplyWorkflowChanges to  WorkflowInstanceEntity
				this.WorkflowEntity[WorkflowInstanceEntity.FieldXaml] = McWorkflowSerializer.GetString(this.InnerWorkflowChanges.TransientWorkflow);

				BusinessManager.Update(this.WorkflowEntity);
			}
			else if (this.WorkflowEntity != null)
			{
				// ApplyWorkflowChanges to  WorkflowInstanceEntity
				this.WorkflowEntity[WorkflowInstanceEntity.FieldXaml] = McWorkflowSerializer.GetString(this.TransientWorkflow);

				if (this.WorkflowEntity.PrimaryKeyId.HasValue)
					BusinessManager.Update(this.WorkflowEntity);
				else
					this.InstanceId = (Guid)BusinessManager.Create(this.WorkflowEntity);
			}
			else
				throw new InvalidOperationException("The workflow has already been saved.");

			Dispose(true);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="bIsManage"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool bIsManage)
		{
			if (bIsManage)
			{
				this.InnerWorkflowChanges = null;
				this.WorkflowEntity = null;

				this.InstanceId = Guid.Empty;
				this.TransientWorkflow = null;
			}
		}
		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
