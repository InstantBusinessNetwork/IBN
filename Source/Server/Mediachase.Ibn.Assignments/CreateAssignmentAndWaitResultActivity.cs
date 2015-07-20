using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using System.Collections.Generic;
using System.IO;

namespace Mediachase.Ibn.Assignments
{
	public partial class CreateAssignmentAndWaitResultActivity : Activity, IActivityEventListener<QueueEventArgs>
    {
		#region DependencyProperty Static Fields
		private static DependencyProperty AssignmentPropertiesProperty = DependencyProperty.Register("AssignmentProperties", typeof(PropertyValueCollection), typeof(CreateAssignmentAndWaitResultActivity), new PropertyMetadata(null, DependencyPropertyOptions.Metadata));

		private static DependencyProperty UseRequestProperty = DependencyProperty.Register("UseRequest", typeof(bool), typeof(CreateAssignmentAndWaitResultActivity), new PropertyMetadata(false, DependencyPropertyOptions.Metadata));

		private static DependencyProperty RequestPropertiesProperty = DependencyProperty.Register("RequestProperties", typeof(PropertyValueCollection), typeof(CreateAssignmentAndWaitResultActivity), new PropertyMetadata(null, DependencyPropertyOptions.Metadata));

		private static DependencyProperty RequestedUsersProperty = DependencyProperty.Register("RequestedUsers", typeof(string), typeof(CreateAssignmentAndWaitResultActivity), new PropertyMetadata(null, DependencyPropertyOptions.Metadata));
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateAssignmentAndWaitResultActivity"/> class.
		/// </summary>
		/// <param name="name">The name to associate with this instance.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="Identifier"/> is a null reference (Nothing in Visual Basic).</exception>
		public CreateAssignmentAndWaitResultActivity(string name):base(name)
		{
			InitializeComponent();
		} 

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateAssignmentAndWaitResultActivity"/> class.
		/// </summary>
		public CreateAssignmentAndWaitResultActivity()
		{
			InitializeComponent();
		} 
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the assignment properties.
		/// </summary>
		/// <value>The assignment properties.</value>
		public PropertyValueCollection AssignmentProperties
		{
			get
			{
				return (PropertyValueCollection)base.GetValue(AssignmentPropertiesProperty);
			}
			set 
			{
				base.SetValue(AssignmentPropertiesProperty, value); 
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [use request].
		/// </summary>
		/// <value><c>true</c> if [use request]; otherwise, <c>false</c>.</value>
		[DefaultValue(false)]
		public bool UseRequest
		{
			get
			{
				return (bool)base.GetValue(UseRequestProperty);
			}
			set { base.SetValue(UseRequestProperty, value); }
		}

		/// <summary>
		/// Gets or sets the request properties.
		/// </summary>
		/// <value>The request properties.</value>
		public PropertyValueCollection RequestProperties
		{
			get
			{
				return (PropertyValueCollection)base.GetValue(RequestPropertiesProperty);
			}
			set
			{
				base.SetValue(RequestPropertiesProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the requested users.
		/// </summary>
		/// <value>The requested users.</value>
		public object RequestedUsers
		{
			get
			{
				return base.GetValue(RequestedUsersProperty);
			}
			set { base.SetValue(RequestedUsersProperty, value); }
		}
		#endregion
 
		#region Initialize
		/// <summary>
		/// Initializes all appropriate child activities of this instance and the specified <see cref="T:System.IServiceProvider"/>.
		/// </summary>
		/// <param name="provider">The specified <see cref="T:System.IServiceProvider"/>.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="provider"/> is a null reference (Nothing in Visual Basic).</exception>
		protected override void Initialize(IServiceProvider provider)
		{
			//this.QueueName = this.Name;

			//WorkflowQueuingService svc =
			//    provider.GetService(typeof(WorkflowQueuingService)) as WorkflowQueuingService;

			//if (this.Name != null)
			//{
			//    GetQueueInternal(svc);
			//}

			base.Initialize(provider);
		}
		#endregion

		#region Queue Utility Methods
		/// <summary>
		/// Gets the queue.
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		protected WorkflowQueue GetQueue(ActivityExecutionContext ctx)
		{
			WorkflowQueuingService svc = ctx.GetService<WorkflowQueuingService>();

			if (!svc.Exists(this.Name))
			{
				return svc.CreateWorkflowQueue(this.Name, false);
			}

			return svc.GetWorkflowQueue(this.Name);
		}

		/// <summary>
		/// Subscribes to item available.
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		protected void SubscribeToQueueItemAvailable(ActivityExecutionContext ctx)
		{
			WorkflowQueue queue = GetQueue(ctx);
			queue.RegisterForQueueItemAvailable(this);
		}

		/// <summary>
		/// Unsubscribes to item available.
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		protected void UnsubscribeFromQueueItemAvailable(ActivityExecutionContext ctx)
		{
			WorkflowQueue queue = GetQueue(ctx);
			queue.UnregisterForQueueItemAvailable(this);

			queue.QueuingService.DeleteWorkflowQueue(this.Name);
		} 
		#endregion

		#region Execute
		/// <summary>
		/// Executes the activity.
		/// </summary>
		/// <param name="executionContext">The execution context of the activity.</param>
		/// <returns>
		/// The <see cref="T:System.Workflow.ComponentModel.ActivityExecutionStatus"/> of the activity after executing the activity.
		/// </returns>
		protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
		{
			// Step 2. Create Assignment
			CreateAssignment();

			// Step 4. Create Assignment
			SubscribeToQueueItemAvailable(executionContext);

			return ActivityExecutionStatus.Executing;
		}
		#endregion

		#region CreateAssignment
		/// <summary>
		/// Creates the assignment.
		/// </summary>
		private void CreateAssignment()
		{
			AssignmentEntity assignment = BusinessManager.InitializeEntity<AssignmentEntity>(AssignmentEntity.ClassName);

			WorkflowInstanceEntity ownerWfInstance = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, (PrimaryKeyId)this.WorkflowInstanceId);

			// Step 2-1. Set Assignment Properties
			InitAssignmentFields(ownerWfInstance, assignment, this.AssignmentProperties);

			// Step 2-2 Initialize Plan Finish Date
			InitializePlanFinishDate(ownerWfInstance, assignment);

			assignment.Type = "Assignment";

			assignment.PrimaryKeyId = BusinessManager.Create(assignment);

			BusinessManager.Execute(new ActivateAssignmentRequest(assignment));

			// Step 3. Create Assignment Request
			if (this.UseRequest)
			{
				foreach (int userId in ResolveRequestUserIdByProperties())
				{
					AssignmentEntity request = BusinessManager.InitializeEntity<AssignmentEntity>(AssignmentEntity.ClassName);

					InitAssignmentFields(ownerWfInstance, request, this.RequestProperties);

					request.Type = "AssignmentRequest";
					request.UserId = userId;
					request.ParentAssignmentId = assignment.PrimaryKeyId;

					// Create
					request.PrimaryKeyId = BusinessManager.Create(request);

					// Activate
					BusinessManager.Execute(new ActivateAssignmentRequest(request));
				}
			}
		}
		#endregion

		#region Activity Initialization Methods
		private void InitializePlanFinishDate(WorkflowInstanceEntity ownerWfInstance, AssignmentEntity assignment)
		{
			if (ownerWfInstance.PlanFinishTimeType == (int)TimeType.NotSet || 
				assignment.PlanFinishDate!=null)
				return;

			DateTime dateTimeNow = DataContext.Current.CurrentUserTimeZone.ToLocalTime(DateTime.UtcNow);

			if (dateTimeNow >= ownerWfInstance.PlanFinishDate.Value)
			{
				assignment.PlanFinishDate = ownerWfInstance.PlanFinishDate;
				return;
			}

			int currentAssignment = CalculateCurrentAssignmentCount();
			int postAssignmentCount = CalculatePostAssignmentCount();

			if (postAssignmentCount == 0)
			{
				assignment.PlanFinishDate = ownerWfInstance.PlanFinishDate;
			}
			else
			{
				double avgDurations = (ownerWfInstance.PlanFinishDate.Value - dateTimeNow).TotalMinutes / (postAssignmentCount + currentAssignment);

				assignment.PlanFinishDate = dateTimeNow.AddMinutes(avgDurations * currentAssignment);
			}
		}

		/// <summary>
		/// Calculates the current assignment count.
		/// </summary>
		/// <returns></returns>
		private int CalculateCurrentAssignmentCount()
		{
			CompositeActivity parent = this.Parent;

			if (parent is BlockActivity)
			{
				return ((BlockActivity)parent).Activities.Count;
			}

			return 1;
		}

		/// <summary>
		/// Calculates the post assignment count.
		/// </summary>
		/// <returns></returns>
		private int CalculatePostAssignmentCount()
		{
			int postAssignmentCount = 0;

			CompositeActivity parent = this.Parent;
			Activity childActivity = this;

			if (parent is BlockActivity)
			{
				childActivity = parent;
				parent = parent.Parent;
			}

			int activityIndex = parent.Activities.IndexOf(childActivity);

			for (int index = activityIndex + 1; index < parent.Activities.Count; index++)
			{
				if (parent.Activities[index] is CreateAssignmentAndWaitResultActivity)
				{
					postAssignmentCount++;
				}
				else if (parent.Activities[index] is BlockActivity)
				{
					postAssignmentCount += ((BlockActivity)parent.Activities[index]).Activities.Count;
				}
			}
			return postAssignmentCount;
		} 
		#endregion

		#region IActivityEventListener<QueueEventArgs> Members

		/// <summary>
		/// Defines the processing procedure when the subscribed-to event occurs.
		/// </summary>
		/// <param name="sender">The object that raised the event.</param>
		/// <param name="e">The previously typed event arguments.</param>
		void IActivityEventListener<QueueEventArgs>.OnEvent(object sender, QueueEventArgs e)
		{
			ActivityExecutionContext ctx = sender as ActivityExecutionContext;

			if (ctx != null && 
				this.ExecutionStatus == ActivityExecutionStatus.Executing)
			{

				try
				{
					ActivityExecutionStatus status = ProcessQueueElement(ctx);
					if (status == ActivityExecutionStatus.Closed)
					{
						UnsubscribeFromQueueItemAvailable(ctx);
						ctx.CloseActivity();
					}
					else if (status == ActivityExecutionStatus.Canceling)
					{
						UnsubscribeFromQueueItemAvailable(ctx);
						ctx.CloseActivity();
						
						throw new CustomWorkflowTerminateException(BusinessProcessExecutionResult.Declined);
					}
				}
				finally
				{
				}
			}
		}

		#endregion

		/// <summary>
		/// Resolves the request user id by properties.
		/// </summary>
		/// <param name="properties">The properties.</param>
		/// <returns></returns>
		private List<int> ResolveRequestUserIdByProperties()
		{
			object value = this.RequestedUsers;

			if (value is DynamicValue)
				value = ((DynamicValue)value).EvaluateValue(this);

			List<int> retVal = new List<int>();

			if (value is int)
			{
				retVal.Add((int)value);
			}
			else if (value is IEnumerable)
			{
				foreach (int item in (IEnumerable)value)
				{
					retVal.Add(item);
				}
			}


			return retVal;
		}

		/// <summary>
		/// Inits the assignment fields.
		/// </summary>
		/// <param name="assignment">The assignment.</param>
		/// <param name="strPrototypeId">The STR prototype id.</param>
        private void InitAssignmentFields(WorkflowInstanceEntity ownerWfInstance,  AssignmentEntity assignment, PropertyValueCollection properties)
        {
			// Init Workflow Properties
            assignment.WorkflowInstanceId = (PrimaryKeyId)this.WorkflowInstanceId;
            assignment.WorkflowActivityName = this.Name;

			// Copy Owner From
			assignment.OwnerDocumentId = ownerWfInstance.OwnerDocumentId;

			if (properties != null)
			{
				foreach (PropertyValue prop in properties)
				{
					if(prop.IsDynamicValue)
						assignment[prop.Name] = ((DynamicValue)prop.Value).EvaluateValue(this);
					else
						assignment[prop.Name] = prop.Value;
				}
			}
        }

        /// <summary>
        /// Queues the item available.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
		protected ActivityExecutionStatus ProcessQueueElement(ActivityExecutionContext ctx)
        {
            WorkflowQueue queue = GetQueue(ctx);

			AssignmentEntity assignment = queue.Dequeue() as AssignmentEntity;

			if (assignment != null)
			{
				return CheckAssignmentCompletion(assignment);
			}

			return ActivityExecutionStatus.Executing;
        }

		/// <summary>
		/// Checks the assignment states.
		/// </summary>
		/// <param name="assignment">The assignment.</param>
		/// <returns></returns>
		private ActivityExecutionStatus CheckAssignmentCompletion(AssignmentEntity assignment)
		{
			if (assignment.State == (int)AssignmentState.Closed)
			{
				if (AssignmentIsRequest(assignment))
				{
					if (assignment.ExecutionResult == (int)AssignmentExecutionResult.Accepted)
					{
						// TODO: Close all assignment request
						foreach (AssignmentEntity request in BusinessManager.List(AssignmentEntity.ClassName,
							new FilterElement[]
							{
								FilterElement.EqualElement(AssignmentEntity.FieldParentAssignmentId, assignment.ParentAssignmentId.Value)
							}))
						{
							BusinessManager.Execute(new CloseAssignmentRequest(request.PrimaryKeyId.Value, (int)AssignmentExecutionResult.Canceled));
						}

						// Update User In Parent Assignment
						AssignmentEntity parentAssignment = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, assignment.ParentAssignmentId.Value);
						parentAssignment.UserId = assignment.UserId;

						BusinessManager.Update(parentAssignment);
					}
				}
				else
				{
					if (assignment.ExecutionResult == (int)AssignmentExecutionResult.Declined)
					{
						return ActivityExecutionStatus.Canceling;
					}

					return ActivityExecutionStatus.Closed;
				}
			}

			return ActivityExecutionStatus.Executing;
		}

		/// <summary>
		/// Assignments the is request.
		/// </summary>
		/// <param name="assignment">The assignment.</param>
		/// <returns></returns>
		private bool AssignmentIsRequest(AssignmentEntity assignment)
		{
			if (this.UseRequest && assignment.ParentAssignmentId.HasValue)
			{
				return true;
			}

			return false;
		}



	}
}
