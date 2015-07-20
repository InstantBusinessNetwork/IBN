using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents block activity.
	/// </summary>
	[Designer(typeof(ParallelActivityDesigner))]
	public partial class BlockActivity : CompositeActivity, IActivityEventListener<ActivityExecutionStatusChangedEventArgs>
	{
		#region DependencyProperty Static Fields
		private static DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(BlockActivityType), typeof(BlockActivity), new PropertyMetadata(BlockActivityType.All, DependencyPropertyOptions.Metadata));
		private static DependencyProperty IsExecutingProperty = DependencyProperty.Register("IsExecuting", typeof(bool), typeof(BlockActivity), new PropertyMetadata(false));
		#endregion

		#region Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="BlockActivity"/> class.
		/// </summary>
		public BlockActivity()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BlockActivity"/> class.
		/// </summary>
		/// <param name="name">The name for the instance.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="name"/> is a null reference (Nothing in Visual Basic).</exception>
		public BlockActivity(string name):base(name)
		{
			InitializeComponent();
		} 
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		[DefaultValue(BlockActivityType.All)]
		public BlockActivityType Type
		{
			get
			{
				return (BlockActivityType)GetValue(TypeProperty);
			}
			set
			{
				base.SetValue(TypeProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is executing.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is executing; otherwise, <c>false</c>.
		/// </value>
		private bool IsExecuting
		{
			get
			{
				return (bool)base.GetValue(IsExecutingProperty);
			}
			set
			{
				base.SetValue(IsExecutingProperty, value);
			}
		}
		#endregion

		/// <summary>
		/// Called by the workflow runtime to cancel execution of an activity that is currently executing.
		/// </summary>
		/// <param name="executionContext">The <see cref="T:System.Workflow.ComponentModel.ActivityExecutionContext"/> containing the instance to cancel.</param>
		/// <returns>
		/// The status at the end of the operation, which determines whether the activity remains in the canceling state, or transitions to the closed state.
		/// </returns>
		protected override ActivityExecutionStatus Cancel(ActivityExecutionContext executionContext)
		{
			if (executionContext == null)
				throw new ArgumentNullException("executionContext");

			bool flag = true;

			for (int i = 0; i < base.EnabledActivities.Count; i++)
			{
				Activity activity = base.EnabledActivities[i];
				if (activity.ExecutionStatus == ActivityExecutionStatus.Executing)
				{
					executionContext.CancelActivity(activity);
					flag = false;
				}
				else if ((activity.ExecutionStatus == ActivityExecutionStatus.Canceling) || (activity.ExecutionStatus == ActivityExecutionStatus.Faulting))
				{
					flag = false;
				}
			}

			if (!flag)
			{
				return ActivityExecutionStatus.Canceling;
			}

			return ActivityExecutionStatus.Closed;
		}


		protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
		{
			if (executionContext == null)
				throw new ArgumentNullException("executionContext");

			this.IsExecuting = true;

			ExecuteActivities(executionContext);

			if (base.EnabledActivities.Count != 0)
				return ActivityExecutionStatus.Executing;

			return ActivityExecutionStatus.Closed;
		}

		/// <summary>
		/// Executes any activities.
		/// </summary>
		/// <param name="executionContext">The execution context.</param>
		private void ExecuteActivities(ActivityExecutionContext executionContext)
		{
			for (int i = 0; i < base.EnabledActivities.Count; i++)
			{
				Activity activity = base.EnabledActivities[i];
				activity.RegisterForStatusChange(Activity.ClosedEvent, this);
				executionContext.ExecuteActivity(activity);
			}
		}


		/// <summary>
		/// Processes the closed even for any activities.
		/// </summary>
		/// <param name="executionContext">The execution context.</param>
		/// <param name="closedActivity">The closed activity.</param>
		/// <returns></returns>
		private ActivityExecutionStatus ProcessClosedEvenForAnyActivities(ActivityExecutionContext executionContext, Activity closedActivity)
		{
			for (int i = 0; i < base.EnabledActivities.Count; i++)
			{
				Activity activity = base.EnabledActivities[i];

				if (base.EnabledActivities[i].ExecutionStatus == ActivityExecutionStatus.Executing)
				{
					executionContext.CancelActivity(activity);
				}
			}

			return ActivityExecutionStatus.Closed;
		}

		/// <summary>
		/// Executes all activities.
		/// </summary>
		/// <param name="executionContext">The execution context.</param>
		private ActivityExecutionStatus ProcessClosedEvenForAllActivities(ActivityExecutionContext executionContext, Activity closedActivity)
		{
			for (int i = 0; i < base.EnabledActivities.Count; i++)
			{
				Activity activity = base.EnabledActivities[i];

				if (activity.ExecutionStatus == ActivityExecutionStatus.Executing)
				{
					return ActivityExecutionStatus.Executing;
				}
			}

			return ActivityExecutionStatus.Closed;
		}

		#region IActivityEventListener<ActivityExecutionStatusChangedEventArgs> Members

		void IActivityEventListener<ActivityExecutionStatusChangedEventArgs>.OnEvent(object sender, ActivityExecutionStatusChangedEventArgs e)
		{
			if (sender == null)
				throw new ArgumentNullException("sender");
			if (e == null)
				throw new ArgumentNullException("e");

			ActivityExecutionContext context = sender as ActivityExecutionContext;
			if (context == null)
				throw new ArgumentException("Sender must be ActivityExecutionContext", "sender");

			BlockActivity activity = context.Activity as BlockActivity;

			e.Activity.UnregisterForStatusChange(Activity.ClosedEvent, this);

			ActivityExecutionStatus result = ActivityExecutionStatus.Initialized;

			switch (this.Type)
			{
				case BlockActivityType.All:
					result = ProcessClosedEvenForAllActivities(context, e.Activity);
					break;
				case BlockActivityType.Any:
					result = ProcessClosedEvenForAnyActivities(context, e.Activity);
					break;
				default:
					throw new ArgumentOutOfRangeException("Unknown Type");
			}

			if (result == ActivityExecutionStatus.Closed)
			{
				context.CloseActivity();
			}
		}

		#endregion
	}
}
