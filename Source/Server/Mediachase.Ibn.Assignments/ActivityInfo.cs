using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents activity info.
	/// </summary>
	[Serializable]
	public class ActivityInfo
	{
		#region Const
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the parent.
		/// </summary>
		/// <value>The parent.</value>
		[XmlIgnore]
		public ActivityInfo Parent { get; set; }

		private BusinessProcessInfo _businessProcess;
		/// <summary>
		/// Gets or sets the business process.
		/// </summary>
		/// <value>The business process.</value>
		[XmlIgnore]
		public BusinessProcessInfo BusinessProcess 
		{
			get
			{
				return _businessProcess;
			}
			set
			{
				_businessProcess = value;

				foreach (ActivityInfo info in this.Activities)
				{
					info.BusinessProcess = this.BusinessProcess;
				}
			}
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public ActivityType Type { get; set; }

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>The subject.</value>
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the comment.
		/// </summary>
		/// <value>The comment.</value>
		public string Comment { get; set; }

		/// <summary>
		/// Gets or sets the user id.
		/// </summary>
		/// <value>The user id.</value>
		public int? UserId { get; set; }

		/// <summary>
		/// Gets or sets the state.
		/// </summary>
		/// <value>The state.</value>
		public AssignmentState State { get; set; }

		/// <summary>
		/// Gets or sets the time status.
		/// </summary>
		/// <value>The time status.</value>
		public AssignmentTimeStatus? TimeStatus { get; set; }

		/// <summary>
		/// Gets or sets the plan start date.
		/// </summary>
		/// <value>The plan start date.</value>
		public DateTime? PlanStartDate { get; set; }

		/// <summary>
		/// Gets or sets the plan finish date.
		/// </summary>
		/// <value>The plan finish date.</value>
		public DateTime? PlanFinishDate { get; set; }

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
		public ActivityInfoCollection Activities { get; set; }

		/// <summary>
		/// Gets or sets the closed by.
		/// </summary>
		/// <value>The closed by.</value>
		public int? ClosedBy { get; set; }

		/// <summary>
		/// Gets or sets the execution result.
		/// </summary>
		/// <value>The execution result.</value>
		public AssignmentExecutionResult? ExecutionResult { get; set; }

		/// <summary>
		/// Gets or sets the assignment properties.
		/// </summary>
		/// <value>The assignment properties.</value>
		public PropertyValueCollection AssignmentProperties { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ActivityInfo"/> class.
		/// </summary>
		public ActivityInfo()
		{
			this.Activities = new ActivityInfoCollection(this);
			this.AssignmentProperties = new PropertyValueCollection();
		}
		#endregion

		#region Methods
		#endregion

		/// <summary>
		/// Gets the activities below count.
		/// </summary>
		/// <returns></returns>
		internal int GetActivitiesBelowCount()
		{
			int currentIndex = this.BusinessProcess.Activities.IndexOf(this);

			if (currentIndex == -1)
				throw new ArgumentException("currentIndex == -1");
			
			int retVal = 0;

			for (int index = currentIndex + 1; index < this.BusinessProcess.Activities.Count; index++)
			{
				ActivityInfo item = this.BusinessProcess.Activities[index];
				retVal += item.Activities.Count == 0 ? 1 : item.Activities.Count;
			}

			return retVal;
		}
	}
}
