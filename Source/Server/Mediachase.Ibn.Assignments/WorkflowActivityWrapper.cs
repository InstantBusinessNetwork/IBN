using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using System.Workflow.Activities;
using System.Workflow.ComponentModel;

using Mediachase.Ibn.Assignments.Schemas;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Microsoft.Win32;

namespace Mediachase.Ibn.Assignments
{
	public static class WorkflowActivityWrapper
	{
		#region ActivityStatus
		public enum ActivityStatus
		{
			// Summary:
			//     Represents the status when an activity is being initialized.
			/// <summary>
			/// 
			/// </summary>
			Initialized = 0,
			//
			// Summary:
			//     Represents the status when an activity is executing.
			/// <summary>
			/// 
			/// </summary>
			Executing = 1,
			//
			// Summary:
			//     Represents the status when an activity is in the process of being canceled.
			/// <summary>
			/// 
			/// </summary>
			Canceling = 2,
			//
			// Summary:
			//     Represents the status when an activity is closed.
			/// <summary>
			/// 
			/// </summary>
			Closed = 3,
			//
			// Summary:
			//     Represents the status when an activity is compensating.
			/// <summary>
			/// 
			/// </summary>
			Compensating = 4,
			//
			// Summary:
			//     Represents the status when an activity is faulting.
			/// <summary>
			/// 
			/// </summary>
			Faulting = 5,
		} 
		#endregion

		#region GetActivityMaster
		/// <summary>
		/// Gets the activity master.
		/// </summary>
		/// <param name="schemaMaster">The schema master.</param>
		/// <param name="activityRoot">The activity root.</param>
		/// <param name="activityQualifiedName">Name of the activity qualified.</param>
		/// <returns></returns>
		public static ActivityMaster GetActivityMaster(SchemaMaster schemaMaster, object activityRoot, string activityQualifiedName)
		{
			Activity root = activityRoot as Activity;
			if (root == null)
				return null;

			Activity element = root.GetActivityByName(activityQualifiedName);
			if (element == null)
				return null;

			ActivityMaster activityMaster = schemaMaster.GetActivityMaster(element.GetType());

			if (activityMaster == null)
				return null;

			return activityMaster;
		} 
		#endregion

		#region GetActivityByName
		/// <summary>
		/// Gets the name of the activity by.
		/// </summary>
		/// <param name="activityRoot">The activity root.</param>
		/// <param name="activityQualifiedName">Name of the activity qualified.</param>
		/// <returns></returns>
		public static object GetActivityByName(object activityRoot, string activityQualifiedName)
		{
			Activity root = activityRoot as Activity;
			if (root == null)
				return null;

			Activity element = root.GetActivityByName(activityQualifiedName);
			if (element == null)
				return null;

			return element;
		} 
		#endregion

		#region AddActivity
		/// <summary>
		/// Adds the activity.
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		/// <param name="parentActivity">The parent activity.</param>
		/// <param name="childActivity">The child activity.</param>
		public static void AddActivity(object parentActivity, object childActivity)
		{
			CompositeActivity parent = parentActivity as CompositeActivity;
			if (parent == null)
				throw new ArgumentException("The parent activity should be CompositeActivity.", "parentActivity");

			Activity child = childActivity as Activity;
			if (child == null)
				throw new ArgumentException("The child activity should be Activity.", "childActivity");

			string strChildActivity = McWorkflowSerializer.GetString<Activity>(child);

			parent.Activities.Add(child);

			// OZ Addon Alpply Modification to Workflow Instance
			//if (instanceId != Guid.Empty)
			//{
			//    try
			//    {
			//        //Guid instanceId = Guid.NewGuid();
			//        WorkflowInstance instance = GlobalWorkflowRuntime.WorkflowRuntime.GetWorkflow(instanceId);

			//        Activity root = instance.GetWorkflowDefinition();

			//        WorkflowChanges wc = new WorkflowChanges(root);

			//        CompositeActivity tmpParent = (CompositeActivity)wc.TransientWorkflow.GetActivityByName(parent.QualifiedName);
			//        //Activity tmpChild = child.Clone();

			//        Activity tmpChild = McWorkflowSerializer.GetObject<Activity>(strChildActivity);
			//        tmpParent.Activities.Add(tmpChild);

			//        instance.ApplyWorkflowChanges(wc);
			//    }
			//    catch (Exception ex)
			//    {
			//        System.Diagnostics.Trace.WriteLine(ex.ToString());
			//    }
			//}
			// End

		} 
		#endregion

		#region GetAssignmentProperties
		/// <summary>
		/// Gets the assignment properties.
		/// </summary>
		/// <param name="activity">The activity.</param>
		/// <returns></returns>
		public static PropertyValueCollection GetAssignmentProperties(object activity)
		{
			CreateAssignmentAndWaitResultActivity root = activity as CreateAssignmentAndWaitResultActivity;
			if (root == null)
				return null;

			return root.AssignmentProperties;
		}

		/// <summary>
		/// Gets the assignment properties.
		/// </summary>
		/// <param name="workflowInstanceId">The workflow instance id.</param>
		/// <param name="activityName">Name of the activity.</param>
		/// <returns></returns>
		public static PropertyValueCollection GetAssignmentProperties(Guid workflowInstanceId, string activityName)
		{
			using (WorkflowChangesScope scope = WorkflowChangesScope.Create(workflowInstanceId))
			{
				return GetAssignmentProperties(GetActivityByName(scope.TransientWorkflow,activityName));
			}

		} 
		#endregion

		#region GetBlockActivityType
		/// <summary>
		/// Gets the type of the block activity.
		/// </summary>
		/// <param name="activity">The activity.</param>
		/// <returns></returns>
		public static BlockActivityType GetBlockActivityType(object activity)
		{
			BlockActivity block = activity as BlockActivity;
			if (block == null)
				throw new ArgumentException("The activity should be CompositeActivity.", "activity");

			return block.Type;
		} 
		#endregion

		#region SetBlockActivityType
		/// <summary>
		/// Sets the type of the block activity.
		/// </summary>
		/// <param name="activity">The activity.</param>
		/// <param name="type">The type.</param>
		public static void SetBlockActivityType(object activity, BlockActivityType type)
		{
			BlockActivity block = activity as BlockActivity;
			if (block == null)
				throw new ArgumentException("The activity should be CompositeActivity.", "activity");

			block.Type = type;
		}
		#endregion

		#region GetActivityList
		/// <summary>
		/// Gets the activity list.
		/// </summary>
		/// <param name="activityRoot">The activity root.</param>
		/// <returns></returns>
		public static DataTable GetActivityList(WorkflowDescription wfDescription, object activityRoot)
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Id", typeof(string));
			dt.Columns.Add("Number", typeof(int));
			dt.Columns.Add("Subject", typeof(string));
			dt.Columns.Add("User", typeof(int));
			dt.Columns.Add("DueDate", typeof(DateTime));
			dt.Columns.Add("State", typeof(ActivityStatus));
			dt.Columns.Add("IsBlock", typeof(bool));
			dt.Columns.Add("Level", typeof(int));

			ProcessActivity(wfDescription, (Activity)activityRoot, dt, 0);

			return dt;
		}

		private static void ProcessActivity(WorkflowDescription wfDescription, Activity activity, DataTable dt, int level)
		{
			DataRow row = dt.NewRow();
			row["Id"] = activity.QualifiedName;
			row["Number"] = dt.Rows.Count + 1;

			if (activity is SequentialWorkflowActivity)
			{
				row["Subject"] = wfDescription.Name;
				row["IsBlock"] = true;
			}
			else if (activity is StateMachineWorkflowActivity)
			{
				row["Subject"] = wfDescription.Name;
				row["IsBlock"] = true;
			}
			else if (activity is BlockActivity)
			{
				if (((BlockActivity)activity).Type == BlockActivityType.All)
					row["Subject"] = "{IbnFramework.BusinessProcess:BlockActivityTypeAll}";
				else
					row["Subject"] = "{IbnFramework.BusinessProcess:BlockActivityTypeAny}";

				row["IsBlock"] = true;
			}
			else if (activity is CreateAssignmentAndWaitResultActivity)
			{
				PropertyValueCollection properties = ((CreateAssignmentAndWaitResultActivity)activity).AssignmentProperties;
				row["Subject"] = properties[AssignmentEntity.FieldSubject].ToString();
				row["User"] = (int)properties[AssignmentEntity.FieldUserId];
				if (properties[AssignmentEntity.FieldPlanFinishDate] != null)
					row["DueDate"] = (DateTime)properties[AssignmentEntity.FieldPlanFinishDate];
				row["IsBlock"] = false;
			}
			if (row["Subject"] == DBNull.Value)
				row["Subject"] = "{IbnFramework.BusinessProcess:NoSubject}";

			row["State"] = GetActivityStatus(wfDescription, activity);
			row["Level"] = level;
			dt.Rows.Add(row);

			level++;
			if (activity is SequentialWorkflowActivity)
			{
				SequentialWorkflowActivity block = (SequentialWorkflowActivity)activity;

				foreach (Activity child in block.Activities)
					ProcessActivity(wfDescription, child, dt, level);
			}
			else if (activity is StateMachineWorkflowActivity)
			{
				StateMachineWorkflowActivity block = (StateMachineWorkflowActivity)activity;

				foreach (Activity child in block.Activities)
					ProcessActivity(wfDescription, child, dt, level);
			}
			else if (activity is BlockActivity)
			{
				BlockActivity block = (BlockActivity)activity;

				foreach (Activity child in block.Activities)
					ProcessActivity(wfDescription, child, dt, level);
			}
		}
		#endregion

		#region GetChildActivityList
		public static DataTable GetChildActivityList(WorkflowDescription wfDescription, object activity)
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Id", typeof(string));
			dt.Columns.Add("Number", typeof(int));
			dt.Columns.Add("Subject", typeof(string));
			dt.Columns.Add("User", typeof(int));
			dt.Columns.Add("DueDate", typeof(DateTime));
			dt.Columns.Add("State", typeof(ActivityStatus));
			dt.Columns.Add("IsBlock", typeof(bool));
			dt.Columns.Add("ReadOnlyLibraryAccess", typeof(bool));

			if (activity is SequentialWorkflowActivity)
			{
				SequentialWorkflowActivity block = (SequentialWorkflowActivity)activity;

				foreach (Activity child in block.Activities)
					ProcessChildActivity(wfDescription, child, dt);
			}
			else if (activity is StateMachineWorkflowActivity)
			{
				StateMachineWorkflowActivity block = (StateMachineWorkflowActivity)activity;

				foreach (Activity child in block.Activities)
					ProcessChildActivity(wfDescription, child, dt);
			}
			else if (activity is BlockActivity)
			{
				BlockActivity block = (BlockActivity)activity;

				foreach (Activity child in block.Activities)
					ProcessChildActivity(wfDescription, child, dt);
			}

			return dt;
		}

		private static void ProcessChildActivity(WorkflowDescription wfDescription, Activity activity, DataTable dt)
		{
			DataRow row = dt.NewRow();
			row["Id"] = activity.QualifiedName;
			row["Number"] = dt.Rows.Count + 1;
			row["ReadOnlyLibraryAccess"] = false;

			if (activity is SequentialWorkflowActivity)
			{
				row["Subject"] = wfDescription.Name;
				row["IsBlock"] = true;
			}
			else if (activity is StateMachineWorkflowActivity)
			{
				row["Subject"] = wfDescription.Name;
				row["IsBlock"] = true;
			}
			else if (activity is BlockActivity)
			{
				if (((BlockActivity)activity).Type == BlockActivityType.All)
					row["Subject"] = "{IbnFramework.BusinessProcess:BlockActivityTypeAll}";
				else
					row["Subject"] = "{IbnFramework.BusinessProcess:BlockActivityTypeAny}";

				row["IsBlock"] = true;
			}
			else if (activity is CreateAssignmentAndWaitResultActivity)
			{
				PropertyValueCollection properties = ((CreateAssignmentAndWaitResultActivity)activity).AssignmentProperties;
				row["Subject"] = properties[AssignmentEntity.FieldSubject].ToString();
				row["User"] = (int)properties[AssignmentEntity.FieldUserId];
				if (properties[AssignmentEntity.FieldPlanFinishDate] != null)
					row["DueDate"] = (DateTime)properties[AssignmentEntity.FieldPlanFinishDate];
				if (properties["ReadOnlyLibraryAccess"] != null)
					row["ReadOnlyLibraryAccess"] = (bool)properties["ReadOnlyLibraryAccess"];
					
				row["IsBlock"] = false;
			}
			if (row["Subject"] == DBNull.Value)
				row["Subject"] = "{IbnFramework.BusinessProcess:NoSubject}";

			row["State"] = GetActivityStatus(wfDescription, activity);
			dt.Rows.Add(row);
		}
		#endregion

		#region GetActivityStatus
		/// <summary>
		/// Gets the activity status.
		/// </summary>
		/// <param name="workflowInstance">The workflow instance.</param>
		/// <param name="activity">The activity.</param>
		/// <returns></returns>
		private static ActivityStatus GetActivityStatus(WorkflowDescription wfDescription, Activity activity)
		{
			if(!wfDescription.InstanceId.HasValue)
				return ActivityStatus.Initialized;

			if (IsWorkflowInstanceClosed(wfDescription.InstanceId.Value))
				return ActivityStatus.Closed;

			if (activity is BlockActivity)
			{
				if (((BlockActivity)activity).Activities.Count > 0)
				{
					ActivityStatus retVal = ActivityStatus.Initialized;

					foreach (Activity child in ((BlockActivity)activity).Activities)
					{
						ActivityStatus childStatus = GetActivityStatus(wfDescription, child);

						if (childStatus == ActivityStatus.Executing)
							return ActivityStatus.Executing;

						if (childStatus == ActivityStatus.Closed)
							retVal = ActivityStatus.Closed;
					}

					return retVal;
				}
				else
				{
					int index = activity.Parent.Activities.IndexOf(activity);

					if ((index > 0 && 
						GetActivityStatus(wfDescription, activity.Parent.Activities[index - 1]) == ActivityStatus.Closed 
						)||
					   ((index + 1) < activity.Parent.Activities.Count && 
					   GetActivityStatus(wfDescription, activity.Parent.Activities[index + 1]) == ActivityStatus.Closed))
						return ActivityStatus.Closed;

					return ActivityStatus.Initialized;
				}
			}
			else
			{
				EntityObject[] assignments = BusinessManager.List(AssignmentEntity.ClassName,
					new FilterElement[] { FilterElement.EqualElement(AssignmentEntity.FieldWorkflowInstanceId, (PrimaryKeyId)wfDescription.InstanceId) });

				foreach (AssignmentEntity assg in assignments)
				{
					if (assg.WorkflowActivityName == activity.Name)
					{
						return assg.ExecutionResult.HasValue ? ActivityStatus.Closed : ActivityStatus.Executing;
					}
				}
			}

			//try
			//{
			//    WorkflowInstance innerInstance = GlobalWorkflowRuntime.WorkflowRuntime.GetWorkflow((Guid)workflowInstance.PrimaryKeyId.Value);
			//    return (ActivityStatus)(int)innerInstance.GetWorkflowDefinition().GetActivityByName(activity.QualifiedName).ExecutionStatus;
			//}
			//catch (Exception ex)
			//{
			//    Trace.WriteLine(ex, "WfActivityWrapper");
			//}

			return ActivityStatus.Initialized;
		}

		/// <summary>
		/// Determines whether [is workflow instance closed] [the specified instance id].
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		/// <returns>
		/// 	<c>true</c> if [is workflow instance closed] [the specified instance id]; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsWorkflowInstanceClosed(Guid instanceId)
		{
			// Use HttpContext Optimization
			string key = "IsWorkflowInstanceClosed_" + instanceId.ToString("N", CultureInfo.InvariantCulture);

			if (HttpContext.Current.Items.Contains(key))
				return (bool)HttpContext.Current.Items[key];

			bool bRetVal = BusinessManager.List(WorkflowInstanceEntity.ClassName,
				new FilterElement[] {
					FilterElement.EqualElement("PrimaryKeyId", instanceId),
					FilterElement.IsNotNullElement(WorkflowInstanceEntity.FieldExecutionResult),
				}).Length > 0;

			HttpContext.Current.Items[key] = bRetVal;

			return bRetVal;
		} 
		#endregion

		#region RemoveActivity
		/// <summary>
		/// Removes the activity.
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		/// <param name="parentActivity">The parent activity.</param>
		/// <param name="activityQualifiedName">Name of the activity qualified.</param>
		public static void RemoveActivity(object parentActivity, string activityQualifiedName)
		{
			CompositeActivity parent = parentActivity as CompositeActivity;
			if (parent == null)
				throw new ArgumentException("The parent activity should be CompositeActivity.", "parentActivity");

			Activity activity = parent.GetActivityByName(activityQualifiedName);
			if (activity != null)
				parent.Activities.Remove(activity);

			// OZ Addon Alpply Modification to Workflow Instance
			//if (instanceId != Guid.Empty)
			//{
			//    try
			//    {
			//        //Guid instanceId = Guid.NewGuid();
			//        WorkflowInstance instance = GlobalWorkflowRuntime.WorkflowRuntime.GetWorkflow(instanceId);

			//        Activity root = instance.GetWorkflowDefinition();

			//        WorkflowChanges wc = new WorkflowChanges(root);

			//        CompositeActivity tmpParent = (CompositeActivity)wc.TransientWorkflow.GetActivityByName(parent.QualifiedName);
			//        Activity tmpChild = wc.TransientWorkflow.GetActivityByName(activity.QualifiedName); ;

			//        tmpParent.Activities.Remove(tmpChild);

			//        instance.ApplyWorkflowChanges(wc);
			//    }
			//    catch (Exception ex)
			//    {
			//        System.Diagnostics.Trace.WriteLine(ex.ToString());
			//    }
			//}
			// End
		}
		#endregion

		#region GetActivityUserList
		/// <summary>
		/// Gets the activity user list.
		/// </summary>
		/// <param name="parentActivity">The parent activity.</param>
		/// <returns></returns>
		public static int[] GetActivityUserList(object parentActivity)
		{
			List<int> retVal = new List<int>();

			CompositeActivity compositeActivity = parentActivity as CompositeActivity;

			if (compositeActivity != null)
			{
				foreach (Activity activity in compositeActivity.Activities)
				{
					CreateAssignmentAndWaitResultActivity cr = activity as CreateAssignmentAndWaitResultActivity;

					if (cr != null && cr.AssignmentProperties.Contains(AssignmentEntity.FieldUserId))
					{
						int? userId = cr.AssignmentProperties[AssignmentEntity.FieldUserId] as int?;

						if (userId.HasValue && !retVal.Contains(userId.Value))
						{
							retVal.Add(userId.Value);
						}
					}

					if (activity is CompositeActivity)
					{
						foreach (int userId in GetActivityUserList(activity))
						{
							if (!retVal.Contains(userId))
							{
								retVal.Add(userId);
							}
						}
					}
				}
			}

			return retVal.ToArray();
		}
		#endregion

		#region GetWorkflowInfo
		/// <summary>
		/// Gets the business process info.
		/// </summary>
		/// <param name="workflowId">The workflow id.</param>
		/// <returns></returns>
		public static BusinessProcessInfo GetBusinessProcessInfo(Guid workflowId)
		{
			BusinessProcessInfo retVal = new BusinessProcessInfo();

			using (WorkflowChangesScope scope = WorkflowChangesScope.Create(workflowId))
			{
				// Step 1. Fill BusinessProcessInfo
				retVal.PrimaryKeyId = (PrimaryKeyId)workflowId;
				retVal.Name = scope.WorkflowEntity.Properties.GetValue<string>(WorkflowInstanceEntity.FieldName);

				if(scope.WorkflowEntity.Properties.Contains(WorkflowInstanceEntity.FieldState))
					retVal.State = (BusinessProcessState)scope.WorkflowEntity.Properties.GetValue<int>(WorkflowInstanceEntity.FieldState);
				else
					retVal.State = BusinessProcessState.Pending;

				retVal.PlanFinishDate = scope.WorkflowEntity.Properties.GetValue<DateTime?>(WorkflowInstanceEntity.FieldPlanFinishDate);

				retVal.PlanDuration = scope.WorkflowEntity.Properties.GetValue<int?>(WorkflowInstanceEntity.FieldPlanDuration);

				if(scope.WorkflowEntity.Properties.Contains(WorkflowInstanceEntity.FieldActualStartDate))
					retVal.ActualStartDate = scope.WorkflowEntity.Properties.GetValue<DateTime?>(WorkflowInstanceEntity.FieldActualStartDate);

				if (scope.WorkflowEntity.Properties.Contains(WorkflowInstanceEntity.FieldActualFinishDate))
					retVal.ActualFinishDate = scope.WorkflowEntity.Properties.GetValue<DateTime?>(WorkflowInstanceEntity.FieldActualFinishDate);

				if (scope.WorkflowEntity.Properties.Contains(WorkflowInstanceEntity.FieldExecutionResult))
					retVal.ExecutionResult = (BusinessProcessExecutionResult?)scope.WorkflowEntity.Properties.GetValue<int?>(WorkflowInstanceEntity.FieldExecutionResult);

				// Step 2. Fill ActivityInfo Recursion
				CompositeActivity activity = scope.TransientWorkflow as CompositeActivity;
				if (activity != null)
				{
					EntityObject[] assignments = BusinessManager.List(AssignmentEntity.ClassName, new FilterElement[] { FilterElement.EqualElement(AssignmentEntity.FieldWorkflowInstanceId, (PrimaryKeyId)workflowId) });

					CreateBusinessProcessActivityInfo(retVal, scope, assignments, activity, retVal.Activities);

					// Fill Plan Finish Date
					if(retVal.PlanFinishDate.HasValue && retVal.State == BusinessProcessState.Active)
						FillPlanFinishDate(retVal.Activities);
				}
			}

			return retVal;
		}

		private static void FillPlanFinishDate(ActivityInfoCollection activities)
		{
			double avgDurations = double.MinValue;
			DateTime dateTimeNow = DataContext.Current.CurrentUserTimeZone.ToLocalTime(DateTime.UtcNow);

			for(int index = 0; index<activities.Count; index++)
			{
				ActivityInfo activity = activities[index];

				if (activity.State == AssignmentState.Pending && activity.PlanFinishDate == null)
				{
					if (activity.Parent != null)
					{
						activity.PlanStartDate = activity.Parent.PlanStartDate;
						activity.PlanFinishDate = activity.Parent.PlanFinishDate;
					}
					else
					{
						activity.PlanStartDate = activities[index - 1].PlanFinishDate;

						int elementWidth = activity.Type== ActivityType.Assignment?
							(activity.Activities.Count == 0 ? 1 : activity.Activities.Count) : (activity.Activities.Count);

						if (avgDurations == double.MinValue)
						{
							if (elementWidth == 0)
							{
								avgDurations = 0;
							}
							else
							{
								int belowCount = activity.GetActivitiesBelowCount();
								avgDurations = (activity.BusinessProcess.PlanFinishDate.Value - activity.PlanStartDate.Value).TotalMinutes / (belowCount + elementWidth);
							}
						}

						activity.PlanFinishDate = activity.PlanStartDate.Value.AddMinutes(elementWidth*avgDurations);
					}

					if (activity.PlanStartDate.HasValue &&
						activity.PlanStartDate.Value < dateTimeNow)
						activity.TimeStatus = AssignmentTimeStatus.OverStart;

					if (activity.PlanFinishDate.HasValue &&
						activity.PlanFinishDate.Value < dateTimeNow)
						activity.TimeStatus = AssignmentTimeStatus.OverDue;
				}

				if (activity.Activities.Count > 0)
					FillPlanFinishDate(activity.Activities);
			}
		}

		/// <summary>
		/// Creates the business process activity info.
		/// </summary>
		/// <param name="businessProcess">The business process.</param>
		/// <param name="scope">The scope.</param>
		/// <param name="parentActivity">The parent activity.</param>
		/// <param name="activityInfoItems">The activity info items.</param>
		private static void CreateBusinessProcessActivityInfo(BusinessProcessInfo businessProcess, WorkflowChangesScope scope, EntityObject[] assignments, CompositeActivity parentActivity, ActivityInfoCollection activityInfoItems)
		{
			foreach (Activity activity in parentActivity.Activities)
			{
				if (activity is CreateAssignmentAndWaitResultActivity)
				{
					CreateAssignmentAndWaitResultActivity asgActivity = (CreateAssignmentAndWaitResultActivity)activity;
					PropertyValueCollection properties = asgActivity.AssignmentProperties;

					ActivityInfo activityInfo = new ActivityInfo();
					activityInfoItems.Add(activityInfo);

					activityInfo.Name = activity.Name;
					activityInfo.Type = ActivityType.Assignment;

					AssignmentEntity assignment = FindAssignmentByActivityName(assignments, activity.Name);

					if (assignment != null)
					{
						activityInfo.State = (AssignmentState)assignment.State;
						activityInfo.Subject = assignment.Subject;

						activityInfo.TimeStatus = (AssignmentTimeStatus?)assignment.TimeStatus;

						activityInfo.PlanStartDate = assignment.ActualStartDate;
						activityInfo.PlanFinishDate = assignment.PlanFinishDate;

						activityInfo.ActualStartDate = assignment.ActualStartDate;
						activityInfo.ActualFinishDate = assignment.ActualFinishDate;
						activityInfo.Comment = assignment.Comment;

						activityInfo.UserId = assignment.UserId;

						activityInfo.ClosedBy = assignment.ClosedBy;

						activityInfo.ExecutionResult = (AssignmentExecutionResult?)assignment.ExecutionResult;
					}
					else
					{
						activityInfo.State = businessProcess.State == BusinessProcessState.Closed?
							AssignmentState.Closed: 
							AssignmentState.Pending;

						if (asgActivity.AssignmentProperties.Contains(AssignmentEntity.FieldSubject))
							activityInfo.Subject = (string)asgActivity.AssignmentProperties[AssignmentEntity.FieldSubject];

						if (asgActivity.AssignmentProperties.Contains(AssignmentEntity.FieldUserId))
							activityInfo.UserId = (int?)asgActivity.AssignmentProperties[AssignmentEntity.FieldUserId];

						// Resolve Plan Time
						//if (activityInfo.State == AssignmentState.Pending && 
						//    businessProcess.State == BusinessProcessState.Active &&
						//    businessProcess.PlanFinishDate.HasValue)
						//{
						//    //CalculatePlanFinishDate(businessProcess, parentActivity, activity, activityInfo);
						//}

						//if (activityInfo.PlanStartDate.HasValue && 
						//    activityInfo.PlanStartDate.Value < DateTime.Now)
						//    activityInfo.TimeStatus = AssignmentTimeStatus.OverStart;

						//if (activityInfo.PlanFinishDate.HasValue &&
						//    activityInfo.PlanFinishDate.Value < DateTime.Now)
						//    activityInfo.TimeStatus = AssignmentTimeStatus.OverDue;
					}


					// 2008-07-07 Added Assignment Properties
					activityInfo.AssignmentProperties.AddRange(asgActivity.AssignmentProperties);
					//
				}
				else if (activity is BlockActivity)
				{
					// Create Block Info
					ActivityInfo blockInfo = new ActivityInfo();
					activityInfoItems.Add(blockInfo);

					blockInfo.Name = activity.Name;
					blockInfo.Type = ActivityType.Block;

					blockInfo.State = businessProcess.State == BusinessProcessState.Closed ?
							AssignmentState.Closed :
							AssignmentState.Pending;

					CreateBusinessProcessActivityInfo(businessProcess, scope, assignments, (BlockActivity)activity, blockInfo.Activities);

					if (blockInfo.State == AssignmentState.Pending)
					{
						if (blockInfo.Activities.Count > 0)
						{
							foreach (ActivityInfo childActivitiInfo in blockInfo.Activities)
							{
								if (childActivitiInfo.State == AssignmentState.Active)
								{
									blockInfo.State = AssignmentState.Active;
									break;
								}
								else if (childActivitiInfo.State == AssignmentState.Closed)
								{
									blockInfo.State = AssignmentState.Closed;
								}
							}
						}
						else
						{
							int blockIndex = activityInfoItems.IndexOf(blockInfo);

							if(blockIndex==0)
								blockInfo.State = AssignmentState.Closed;
							else
								blockInfo.State = activityInfoItems[blockIndex-1].State==AssignmentState.Active? AssignmentState.Pending:
									activityInfoItems[blockIndex - 1].State;
						}
					}

					if (blockInfo.Activities.Count > 0 && 
						(blockInfo.State == AssignmentState.Active || 
						blockInfo.State == AssignmentState.Closed))
					{
						blockInfo.PlanStartDate = blockInfo.Activities[0].PlanStartDate;
						blockInfo.PlanFinishDate = blockInfo.Activities[0].PlanFinishDate;
					}
				}
			}
		}

		//private static void CalculatePlanFinishDate(BusinessProcessInfo businessProcess, CompositeActivity parentActivity, Activity activity, ActivityInfo activityInfo)
		//{
		//    if (businessProcess.PlanFinishDate < DateTime.Now)
		//    {
		//        //activityInfo.PlanStartDate = businessProcess.PlanFinishDate;
		//        activityInfo.PlanFinishDate = businessProcess.PlanFinishDate;
		//    }
		//    else
		//    {
		//        // Find Post Assignment Count
		//        int postAssignmentCount = 0;

		//        CompositeActivity parent = parentActivity;
		//        Activity childActivity = activity;

		//        if (parent is BlockActivity)
		//        {
		//            childActivity = parent;
		//            parent = parent.Parent;
		//        }

		//        int activityIndex = parent.Activities.IndexOf(childActivity);

		//        for (int index = activityIndex + 1; index < parent.Activities.Count; index++)
		//        {
		//            if (parent.Activities[index] is CreateAssignmentAndWaitResultActivity ||
		//                (parent.Activities[index] is BlockActivity) && ((BlockActivity)parent.Activities[index]).Activities.Count > 0)
		//            {
		//                postAssignmentCount++;
		//            }
		//        }

		//        // Find Previous ActivityInfo
		//        DateTime now = businessProcess.ActualStartDate.Value;

		//        if (postAssignmentCount == 0)
		//        {
		//            activityInfo.PlanFinishDate = businessProcess.PlanFinishDate;
		//        }
		//        else
		//        {
		//            double avgDurations = (businessProcess.PlanFinishDate.Value - DateTime.Now).TotalMinutes / (postAssignmentCount);
		//            activityInfo.PlanFinishDate = DateTime.Now.AddMinutes(avgDurations);
		//        }
		//    }
		//}

		/// <summary>
		/// Finds the name of the assignment by activity.
		/// </summary>
		/// <param name="assignments">The assignments.</param>
		/// <param name="activityName">Name of the activity.</param>
		/// <returns></returns>
		private static AssignmentEntity FindAssignmentByActivityName(EntityObject[] assignments, string activityName)
		{
			foreach (AssignmentEntity assignment in assignments)
			{
				if (assignment.WorkflowActivityName == activityName)
					return assignment;
			}

			return null;
		}
		#endregion

		#region IsFramework35Installed
		/// <summary>
		/// Determines whether .NET Framework 3.5 is installed.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if .NET Framework 3.5 is installed; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsFramework35Installed()
		{
			bool result = false;

			const string keyName = "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v3.5";
			const string valueName = "Install";

			using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName))
			{
				if (key != null)
				{
					if (key.GetValueKind(valueName) == RegistryValueKind.DWord)
					{
						object value = key.GetValue(valueName);
						if (value != null)
							result = ((int)value == 1);
					}
				}
			}

			return result;
		} 
		#endregion
	}
}
