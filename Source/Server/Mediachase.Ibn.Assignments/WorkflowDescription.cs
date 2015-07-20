using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Assignments.Schemas;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents workflow description.
	/// </summary>
	public class WorkflowDescription
	{
		#region Const
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the instance id.
		/// </summary>
		/// <value>The instance id.</value>
		public Guid? InstanceId { get; set; }

		/// <summary>
		/// Gets or sets the transient workflow.
		/// </summary>
		/// <value>The transient workflow.</value>
		public object TransientWorkflow { get; protected set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the schema master.
		/// </summary>
		/// <value>The schema master.</value>
		public SchemaMaster SchemaMaster { get; set; }

		/// <summary>
		/// Gets or sets the type of the plan finish time.
		/// </summary>
		/// <value>The type of the plan finish time.</value>
		public TimeType PlanFinishTimeType { get; set; }

		/// <summary>
		/// Gets or sets the duration of the plan.
		/// </summary>
		/// <value>The duration of the plan.</value>
		public int? PlanDuration { get; set; }

		/// <summary>
		/// Gets or sets the plan finish date.
		/// </summary>
		/// <value>The plan finish date.</value>
		public DateTime? PlanFinishDate { get; set; }

		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="WorkflowDescription"/> class.
		/// </summary>
		public WorkflowDescription(string name, SchemaMaster schemaMaster, object transientWorkflow)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (schemaMaster == null)
				throw new ArgumentNullException("schemaMaster");
			if (transientWorkflow == null)
				throw new ArgumentNullException("transientWorkflow");

			this.Name = name;
			this.SchemaMaster = schemaMaster;
			this.TransientWorkflow = transientWorkflow;
			this.PlanFinishTimeType = TimeType.NotSet;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkflowDescription"/> class.
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		/// <param name="name">The name.</param>
		/// <param name="schemaMaster">The schema master.</param>
		/// <param name="transientWorkflow">The transient workflow.</param>
		public WorkflowDescription(Guid instanceId, string name, SchemaMaster schemaMaster, object transientWorkflow)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (schemaMaster == null)
				throw new ArgumentNullException("schemaMaster");
			if (transientWorkflow == null)
				throw new ArgumentNullException("transientWorkflow");

			this.InstanceId = instanceId;
			this.Name = name;
			this.SchemaMaster = schemaMaster;
			this.TransientWorkflow = transientWorkflow;
			this.PlanFinishTimeType = TimeType.NotSet;
		}
		#endregion

		#region Methods
		#endregion

		
	}
}
