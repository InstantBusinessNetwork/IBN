using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents business process info.
	/// </summary>
	[Serializable]
	public class BusinessProcessInfo
	{
		#region Const
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the primary key id.
		/// </summary>
		/// <value>The primary key id.</value>
		public PrimaryKeyId PrimaryKeyId { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the state.
		/// </summary>
		/// <value>The state.</value>
		public BusinessProcessState State { get; set; }

		/// <summary>
		/// Gets or sets the execution result.
		/// </summary>
		/// <value>The execution result.</value>
		public BusinessProcessExecutionResult? ExecutionResult { get; set; }

		/// <summary>
		/// Gets or sets the plan finish date.
		/// </summary>
		/// <value>The plan finish date.</value>
		public DateTime? PlanFinishDate { get; set; }

		/// <summary>
		/// Gets or sets the duration of the plan.
		/// </summary>
		/// <value>The duration of the plan.</value>
		public int? PlanDuration { get; set; }

		/// <summary>
		/// Gets or sets the actual start.
		/// </summary>
		/// <value>The actual start.</value>
		public DateTime? ActualStartDate { get; set; }

		/// <summary>
		/// Gets or sets the actual finish.
		/// </summary>
		/// <value>The actual finish.</value>
		public DateTime? ActualFinishDate { get; set; }

		/// <summary>
		/// Gets or sets the activities.
		/// </summary>
		/// <value>The activities.</value>
		public ActivityInfoCollection Activities { get;  set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="BusinessProcessInfo"/> class.
		/// </summary>
		public BusinessProcessInfo()
		{
			this.Activities = new ActivityInfoCollection(this);
		}
		#endregion

		#region Methods
		#endregion
	}
}
