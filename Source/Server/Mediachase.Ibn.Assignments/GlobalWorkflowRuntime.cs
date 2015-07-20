using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;
using System.Xml;
using System.IO;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core.Business;
using System.Diagnostics;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Activities;
using System.ComponentModel.Design;
using System.Workflow.ComponentModel;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents global workflow runtime.
	/// </summary>
	/// <remarks>
	/// WorkflowOwnershipException	
	/// Message: WorkflowRuntime не является владельцем этого потока работ. Время владения WorkflowRuntime истекло или владельцем потока работ является другая среда WorkflowRuntime. 
	/// 
	/// Solution:
	/// UPDATE InstanceState SET ownerId = NULL, ownedUntil= NULL
	/// WHERE ownerId IS NOT NULL AND ownedUntil IS NOT NULL
	/// 
	/// More Info:
	/// http://social.msdn.microsoft.com/forums/en-US/windowsworkflowfoundation/thread/b79cef9d-2af0-4155-ace7-63790e4dd86f/
	/// </remarks>
	public static class GlobalWorkflowRuntime
	{
        public static WorkflowRuntime WorkflowRuntime { get; private set; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public static void StartRuntime(string connectionString)
        {
			if (WorkflowRuntime == null)
			{
				lock (typeof(GlobalWorkflowRuntime))
				{
					if (WorkflowRuntime == null)
					{
						ResetWorkflowOwnership(connectionString);

						WorkflowRuntime = new WorkflowRuntime();
						InstallWorkflowServices(connectionString);

						WorkflowRuntime.WorkflowStarted += new EventHandler<WorkflowEventArgs>(WorkflowRuntime_WorkflowStarted);
						WorkflowRuntime.WorkflowCompleted += new EventHandler<WorkflowCompletedEventArgs>(WorkflowRuntime_WorkflowCompleted);
						WorkflowRuntime.WorkflowTerminated += new EventHandler<WorkflowTerminatedEventArgs>(WorkflowRuntime_WorkflowTerminated);
						WorkflowRuntime.WorkflowSuspended += new EventHandler<WorkflowSuspendedEventArgs>(WorkflowRuntime_WorkflowSuspended);
						WorkflowRuntime.WorkflowResumed += new EventHandler<WorkflowEventArgs>(WorkflowRuntime_WorkflowResumed);

						WorkflowRuntime.StartRuntime();
					}
				}
			}
        }

		/// <summary>
		/// Resets the workflow ownership.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		private static void ResetWorkflowOwnership(string connectionString)
		{
			string cleatLocksSqlScript = @"UPDATE  InstanceState SET ownerID=null, ownedUntil=null, unlocked=1 WHERE ownerID IS NOT NULL";

			Mediachase.Ibn.Data.Sql.SqlHelper.ExecuteNonQuery(connectionString,
				System.Data.CommandType.Text,
				cleatLocksSqlScript);
		}

		#region Workflow Event Callbacks
		/// <summary>
		/// Handles the WorkflowStarted event of the WorkflowRuntime control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Workflow.Runtime.WorkflowEventArgs"/> instance containing the event data.</param>
		static void WorkflowRuntime_WorkflowStarted(object sender, WorkflowEventArgs e)
		{
			DateTime dateTimeNow = DataContext.Current.CurrentUserTimeZone.ToLocalTime(DateTime.UtcNow);

			WorkflowInstanceEntity wf = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, (PrimaryKeyId)e.WorkflowInstance.InstanceId);
			wf.ActualStartDate = dateTimeNow;
			wf.State = (int)BusinessProcessState.Active;

			BusinessManager.Update(wf);
		}

		/// <summary>
		/// Handles the WorkflowCompleted event of the WorkflowRuntime control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Workflow.Runtime.WorkflowCompletedEventArgs"/> instance containing the event data.</param>
		static void WorkflowRuntime_WorkflowCompleted(object sender, WorkflowCompletedEventArgs e)
		{
			Trace.WriteLine("WorkflowCompleted", "GlobalWorkflowRuntime");

			DateTime dateTimeNow = DataContext.Current.CurrentUserTimeZone.ToLocalTime(DateTime.UtcNow);

			WorkflowInstanceEntity wf = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, (PrimaryKeyId)e.WorkflowInstance.InstanceId);
			wf.State = (int)BusinessProcessState.Closed;
			wf.ExecutionResult = (int)BusinessProcessExecutionResult.Accepted; // Calculate Result
			wf.ActualFinishDate = dateTimeNow;

			BusinessManager.Update(wf);

			// Cancel All Assignments
			CancelAllAssignments(wf);
		}


		static void WorkflowRuntime_WorkflowResumed(object sender, WorkflowEventArgs e)
		{
			Trace.WriteLine("WorkflowResume", "GlobalWorkflowRuntime");

			WorkflowInstanceEntity wf = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, (PrimaryKeyId)e.WorkflowInstance.InstanceId);
			wf.State = (int)BusinessProcessState.Active;
			BusinessManager.Update(wf);

			// Resume All Suspended Assignments
			ActivateAllSuspendedAssignments(wf);

		}

		static void WorkflowRuntime_WorkflowSuspended(object sender, WorkflowSuspendedEventArgs e)
		{
			Trace.WriteLine("WorkflowSuspended", "GlobalWorkflowRuntime");

			WorkflowInstanceEntity wf = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, (PrimaryKeyId)e.WorkflowInstance.InstanceId);
			wf.State = (int)BusinessProcessState.Suspended;
			BusinessManager.Update(wf);

			// Suspend All Active Assignments
			SuspendAllActiveAssignments(wf);
		}

		static void WorkflowRuntime_WorkflowTerminated(object sender, WorkflowTerminatedEventArgs e)
		{
			Trace.WriteLine("WorkflowTerminated", "GlobalWorkflowRuntime");

			WorkflowInstanceEntity wf = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, (PrimaryKeyId)e.WorkflowInstance.InstanceId);

			DateTime dateTimeNow = DataContext.Current.CurrentUserTimeZone.ToLocalTime(DateTime.UtcNow);

			if (e.Exception is CustomWorkflowTerminateException)
			{
				wf.ExecutionResult = (int)((CustomWorkflowTerminateException)e.Exception).ExecutionResult;
				wf.ActualFinishDate = dateTimeNow;
				wf.State = (int)BusinessProcessState.Closed;
			}
			else
			{
				wf.State = (int)BusinessProcessState.Closed;
				wf.ActualFinishDate = dateTimeNow;
				wf.ExecutionResult = (int)BusinessProcessExecutionResult.Canceled; // Calculate Result
			}

			// TODO: Save Exception 
			//e.Exception 

			BusinessManager.Update(wf);

			// Cancel All Active Assignments
			CancelAllAssignments(wf);
		}

		#endregion

		#region Assignments Methods
		/// <summary>
		/// Cancels all assignments.
		/// </summary>
		/// <param name="wf">The wf.</param>
		private static void CancelAllAssignments(WorkflowInstanceEntity wf)
		{
			foreach (AssignmentEntity assignment in BusinessManager.List(AssignmentEntity.ClassName, new FilterElement[]
							{
								FilterElement.NotEqualElement(AssignmentEntity.FieldState, (int)AssignmentState.Closed),
								FilterElement.EqualElement(AssignmentEntity.FieldWorkflowInstanceId, wf.PrimaryKeyId.Value)
							}))
			{
				assignment.State = (int)AssignmentState.Closed;
				assignment.ExecutionResult = (int)AssignmentExecutionResult.Canceled;

				BusinessManager.Update(assignment);
			}
		}

		/// <summary>
		/// Activates all suspended assignments.
		/// </summary>
		/// <param name="wf">The wf.</param>
		private static void ActivateAllSuspendedAssignments(WorkflowInstanceEntity wf)
		{
			foreach (AssignmentEntity assignment in BusinessManager.List(AssignmentEntity.ClassName, new FilterElement[]
							{
								FilterElement.EqualElement(AssignmentEntity.FieldState, (int)AssignmentState.Suspended),
								FilterElement.EqualElement(AssignmentEntity.FieldWorkflowInstanceId, wf.PrimaryKeyId.Value)
							}))
			{
				assignment.State = (int)AssignmentState.Active;

				BusinessManager.Update(assignment);
			}
		}

		/// <summary>
		/// Suspends all active assignments.
		/// </summary>
		/// <param name="wf">The wf.</param>
		private static void SuspendAllActiveAssignments(WorkflowInstanceEntity wf)
		{
			foreach (AssignmentEntity assignment in BusinessManager.List(AssignmentEntity.ClassName, new FilterElement[]
							{
								FilterElement.EqualElement(AssignmentEntity.FieldState, (int)AssignmentState.Active),
								FilterElement.EqualElement(AssignmentEntity.FieldWorkflowInstanceId, wf.PrimaryKeyId.Value)
							}))
			{
				assignment.State = (int)AssignmentState.Suspended;

				BusinessManager.Update(assignment);
			}
		}
		#endregion

        /// <summary>
        /// Installs the workflow services.
        /// </summary>
		private static void InstallWorkflowServices(string connectionString)
        {
            // WorkFlow Work In ASP.NET Thread
            WorkflowRuntime.AddService(new ManualWorkflowSchedulerService());

            // SQL Persistence Service
			if (!string.IsNullOrEmpty(connectionString))
			{
				SqlWorkflowPersistenceService sqlSvc = new SqlWorkflowPersistenceService(
					connectionString,
					true,
					TimeSpan.MaxValue, // TimeSpan.FromSeconds(90)
					TimeSpan.FromSeconds(120));

				WorkflowRuntime.AddService(sqlSvc);
			}
        }


        /// <summary>
        /// Stops this instance.
        /// </summary>
        public static void StopRuntime()
        {
            if (WorkflowRuntime != null)
            {
                WorkflowRuntime.StopRuntime();

				WorkflowRuntime.WorkflowCompleted -= new EventHandler<WorkflowCompletedEventArgs>(WorkflowRuntime_WorkflowCompleted);
				WorkflowRuntime.WorkflowTerminated -= new EventHandler<WorkflowTerminatedEventArgs>(WorkflowRuntime_WorkflowTerminated);
				WorkflowRuntime.WorkflowSuspended -= new EventHandler<WorkflowSuspendedEventArgs>(WorkflowRuntime_WorkflowSuspended);
				WorkflowRuntime.WorkflowResumed -= new EventHandler<WorkflowEventArgs>(WorkflowRuntime_WorkflowResumed);


                WorkflowRuntime.Dispose();
                WorkflowRuntime = null;
            }
        }


		/// <summary>
		/// Runs the workflow.
		/// </summary>
		/// <param name="instance">The instance.</param>
		public static void RunWorkflow(WorkflowInstance instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			ManualWorkflowSchedulerService service = WorkflowRuntime.GetService<ManualWorkflowSchedulerService>();
			if (service != null)
				service.RunWorkflow(instance.InstanceId);
		}

		#region CreateWorkflowView
		static void TestWorkflowView()
		{
			GlobalWorkflowRuntime.StartRuntime("Data source=S2;Initial catalog=ibn48portal;User ID=dev;Password=");
			DataContext.Current = new DataContext("Data source=S2;Initial catalog=ibn48portal;User ID=dev;Password=");

			//WorkflowInstanceEntity entity = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, PrimaryKeyId.Parse("23ec347d-0529-43d4-ab34-d916f754b0df"));

			#region xaml string
			string xaml = @"<?xml version=""1.0"" encoding=""utf-16""?><SequentialWorkflowActivity " +
@"x:Name=""SequentialWorkflowActivity_Root"" " +
@"xmlns:ns0=""clr-namespace:Mediachase.Ibn.Assignments;Assembly=Mediachase.Ibn.Assignments, " +
@"Version=4.7.54.0, Culture=neutral, PublicKeyToken=null"" " +
@"xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" " +
@"xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/workflow"">" + Environment.NewLine +
@"	<ns0:CreateAssignmentAndWaitResultActivity RequestProperties=""{x:Null}"" " +
@"x:Name=""createAssignmentAndWait_ce11db253f434983b25f7a7f5b056145"">" + Environment.NewLine +
@"		<ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"			<ns0:PropertyValueCollection>" + Environment.NewLine +
@"				<ns0:PropertyValue Name=""Subject"">" + Environment.NewLine +
@"					<ns0:PropertyValue.Value>" + Environment.NewLine +
@"						<ns1:String xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">OZ OverDue Test 3</ns1:String>" + Environment.NewLine +
@"					</ns0:PropertyValue.Value>" + Environment.NewLine +
@"				</ns0:PropertyValue>" + Environment.NewLine +
@"				<ns0:PropertyValue Name=""UserId"">" + Environment.NewLine +
@"					<ns0:PropertyValue.Value>" + Environment.NewLine +
@"						<ns1:Int32 xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">65</ns1:Int32>" + Environment.NewLine +
@"					</ns0:PropertyValue.Value>" + Environment.NewLine +
@"				</ns0:PropertyValue>" + Environment.NewLine +
@"			</ns0:PropertyValueCollection>" + Environment.NewLine +
@"		</ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"	</ns0:CreateAssignmentAndWaitResultActivity>" + Environment.NewLine +
@"	<ns0:CreateAssignmentAndWaitResultActivity RequestProperties=""{x:Null}"" " +
@"x:Name=""createAssignmentAndWait_0c22583fd14344d9a82d43cc674ab0d7"">" + Environment.NewLine +
@"		<ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"			<ns0:PropertyValueCollection>" + Environment.NewLine +
@"				<ns0:PropertyValue Name=""Subject"">" + Environment.NewLine +
@"					<ns0:PropertyValue.Value>" + Environment.NewLine +
@"						<ns1:String xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">OZ OverDue Test 3</ns1:String>" + Environment.NewLine +
@"					</ns0:PropertyValue.Value>" + Environment.NewLine +
@"				</ns0:PropertyValue>" + Environment.NewLine +
@"				<ns0:PropertyValue Name=""UserId"">" + Environment.NewLine +
@"					<ns0:PropertyValue.Value>" + Environment.NewLine +
@"						<ns1:Int32 xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">65</ns1:Int32>" + Environment.NewLine +
@"					</ns0:PropertyValue.Value>" + Environment.NewLine +
@"				</ns0:PropertyValue>" + Environment.NewLine +
@"			</ns0:PropertyValueCollection>" + Environment.NewLine +
@"		</ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"	</ns0:CreateAssignmentAndWaitResultActivity>" + Environment.NewLine +
@"	<ns0:BlockActivity x:Name=""blockActivity_8ef17c90ba4a4a09bd92d5dc14b3e779"">" + Environment.NewLine +
@"		<ns0:CreateAssignmentAndWaitResultActivity RequestProperties=""{x:Null}"" " +
@"x:Name=""createAssignmentAndWait_0f4cb38833a643fabad5fafb88d76657"">" + Environment.NewLine +
@"			<ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"				<ns0:PropertyValueCollection>" + Environment.NewLine +
@"					<ns0:PropertyValue Name=""Subject"">" + Environment.NewLine +
@"						<ns0:PropertyValue.Value>" + Environment.NewLine +
@"							<ns1:String xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">OZ OverDue Test 3</ns1:String>" + Environment.NewLine +
@"						</ns0:PropertyValue.Value>" + Environment.NewLine +
@"					</ns0:PropertyValue>" + Environment.NewLine +
@"					<ns0:PropertyValue Name=""UserId"">" + Environment.NewLine +
@"						<ns0:PropertyValue.Value>" + Environment.NewLine +
@"							<ns1:Int32 xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">65</ns1:Int32>" + Environment.NewLine +
@"						</ns0:PropertyValue.Value>" + Environment.NewLine +
@"					</ns0:PropertyValue>" + Environment.NewLine +
@"				</ns0:PropertyValueCollection>" + Environment.NewLine +
@"			</ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"		</ns0:CreateAssignmentAndWaitResultActivity>" + Environment.NewLine +
@"		<ns0:CreateAssignmentAndWaitResultActivity RequestProperties=""{x:Null}"" " +
@"x:Name=""createAssignmentAndWait_bf381aae62aa4732ba5328140e3b8a09"">" + Environment.NewLine +
@"			<ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"				<ns0:PropertyValueCollection>" + Environment.NewLine +
@"					<ns0:PropertyValue Name=""Subject"">" + Environment.NewLine +
@"						<ns0:PropertyValue.Value>" + Environment.NewLine +
@"							<ns1:String xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">OZ OverDue Test 3</ns1:String>" + Environment.NewLine +
@"						</ns0:PropertyValue.Value>" + Environment.NewLine +
@"					</ns0:PropertyValue>" + Environment.NewLine +
@"					<ns0:PropertyValue Name=""UserId"">" + Environment.NewLine +
@"						<ns0:PropertyValue.Value>" + Environment.NewLine +
@"							<ns1:Int32 xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">65</ns1:Int32>" + Environment.NewLine +
@"						</ns0:PropertyValue.Value>" + Environment.NewLine +
@"					</ns0:PropertyValue>" + Environment.NewLine +
@"				</ns0:PropertyValueCollection>" + Environment.NewLine +
@"			</ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"		</ns0:CreateAssignmentAndWaitResultActivity>" + Environment.NewLine +
@"	</ns0:BlockActivity>" + Environment.NewLine +
@"	<ns0:BlockActivity x:Name=""blockActivity_a6c544f45e3b4b4facba5193b307291e"">" + Environment.NewLine +
@"		<ns0:CreateAssignmentAndWaitResultActivity RequestProperties=""{x:Null}"" " +
@"x:Name=""createAssignmentAndWait_7f8498d01af740eda769250ddaa8912b"">" + Environment.NewLine +
@"			<ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"				<ns0:PropertyValueCollection>" + Environment.NewLine +
@"					<ns0:PropertyValue Name=""Subject"">" + Environment.NewLine +
@"						<ns0:PropertyValue.Value>" + Environment.NewLine +
@"							<ns1:String xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">OZ OverDue Test 3</ns1:String>" + Environment.NewLine +
@"						</ns0:PropertyValue.Value>" + Environment.NewLine +
@"					</ns0:PropertyValue>" + Environment.NewLine +
@"					<ns0:PropertyValue Name=""UserId"">" + Environment.NewLine +
@"						<ns0:PropertyValue.Value>" + Environment.NewLine +
@"							<ns1:Int32 xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">65</ns1:Int32>" + Environment.NewLine +
@"						</ns0:PropertyValue.Value>" + Environment.NewLine +
@"					</ns0:PropertyValue>" + Environment.NewLine +
@"				</ns0:PropertyValueCollection>" + Environment.NewLine +
@"			</ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"		</ns0:CreateAssignmentAndWaitResultActivity>" + Environment.NewLine +
@"		<ns0:CreateAssignmentAndWaitResultActivity RequestProperties=""{x:Null}"" " +
@"x:Name=""createAssignmentAndWait_97f018bfd3fb47cdac6c569394d2856e"">" + Environment.NewLine +
@"			<ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"				<ns0:PropertyValueCollection>" + Environment.NewLine +
@"					<ns0:PropertyValue Name=""Subject"">" + Environment.NewLine +
@"						<ns0:PropertyValue.Value>" + Environment.NewLine +
@"							<ns1:String xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">OZ OverDue Test 3</ns1:String>" + Environment.NewLine +
@"						</ns0:PropertyValue.Value>" + Environment.NewLine +
@"					</ns0:PropertyValue>" + Environment.NewLine +
@"					<ns0:PropertyValue Name=""UserId"">" + Environment.NewLine +
@"						<ns0:PropertyValue.Value>" + Environment.NewLine +
@"							<ns1:Int32 xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">65</ns1:Int32>" + Environment.NewLine +
@"						</ns0:PropertyValue.Value>" + Environment.NewLine +
@"					</ns0:PropertyValue>" + Environment.NewLine +
@"				</ns0:PropertyValueCollection>" + Environment.NewLine +
@"			</ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"		</ns0:CreateAssignmentAndWaitResultActivity>" + Environment.NewLine +
@"		<ns0:CreateAssignmentAndWaitResultActivity RequestProperties=""{x:Null}"" " +
@"x:Name=""createAssignmentAndWait_96cf746ba7724deca0a1e3b42e8a6623"">" + Environment.NewLine +
@"			<ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"				<ns0:PropertyValueCollection>" + Environment.NewLine +
@"					<ns0:PropertyValue Name=""Subject"">" + Environment.NewLine +
@"						<ns0:PropertyValue.Value>" + Environment.NewLine +
@"							<ns1:String xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">OZ OverDue Test 3</ns1:String>" + Environment.NewLine +
@"						</ns0:PropertyValue.Value>" + Environment.NewLine +
@"					</ns0:PropertyValue>" + Environment.NewLine +
@"					<ns0:PropertyValue Name=""UserId"">" + Environment.NewLine +
@"						<ns0:PropertyValue.Value>" + Environment.NewLine +
@"							<ns1:Int32 xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">65</ns1:Int32>" + Environment.NewLine +
@"						</ns0:PropertyValue.Value>" + Environment.NewLine +
@"					</ns0:PropertyValue>" + Environment.NewLine +
@"				</ns0:PropertyValueCollection>" + Environment.NewLine +
@"			</ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"		</ns0:CreateAssignmentAndWaitResultActivity>" + Environment.NewLine +
@"		<ns0:CreateAssignmentAndWaitResultActivity RequestProperties=""{x:Null}"" " +
@"x:Name=""createAssignmentAndWait_a05219765fec40d9bb50fb4f463110cc"">" + Environment.NewLine +
@"			<ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"				<ns0:PropertyValueCollection>" + Environment.NewLine +
@"					<ns0:PropertyValue Name=""Subject"">" + Environment.NewLine +
@"						<ns0:PropertyValue.Value>" + Environment.NewLine +
@"							<ns1:String xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">OZ OverDue Test 3</ns1:String>" + Environment.NewLine +
@"						</ns0:PropertyValue.Value>" + Environment.NewLine +
@"					</ns0:PropertyValue>" + Environment.NewLine +
@"					<ns0:PropertyValue Name=""UserId"">" + Environment.NewLine +
@"						<ns0:PropertyValue.Value>" + Environment.NewLine +
@"							<ns1:Int32 xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">65</ns1:Int32>" + Environment.NewLine +
@"						</ns0:PropertyValue.Value>" + Environment.NewLine +
@"					</ns0:PropertyValue>" + Environment.NewLine +
@"				</ns0:PropertyValueCollection>" + Environment.NewLine +
@"			</ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"		</ns0:CreateAssignmentAndWaitResultActivity>" + Environment.NewLine +
@"		<ns0:CreateAssignmentAndWaitResultActivity RequestProperties=""{x:Null}"" " +
@"x:Name=""createAssignmentAndWait_88c79524e0ec44a5985caf02b1735400"">" + Environment.NewLine +
@"			<ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"				<ns0:PropertyValueCollection>" + Environment.NewLine +
@"					<ns0:PropertyValue Name=""Subject"">" + Environment.NewLine +
@"						<ns0:PropertyValue.Value>" + Environment.NewLine +
@"							<ns1:String xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">OZ OverDue Test 3</ns1:String>" + Environment.NewLine +
@"						</ns0:PropertyValue.Value>" + Environment.NewLine +
@"					</ns0:PropertyValue>" + Environment.NewLine +
@"					<ns0:PropertyValue Name=""UserId"">" + Environment.NewLine +
@"						<ns0:PropertyValue.Value>" + Environment.NewLine +
@"							<ns1:Int32 xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">65</ns1:Int32>" + Environment.NewLine +
@"						</ns0:PropertyValue.Value>" + Environment.NewLine +
@"					</ns0:PropertyValue>" + Environment.NewLine +
@"				</ns0:PropertyValueCollection>" + Environment.NewLine +
@"			</ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"		</ns0:CreateAssignmentAndWaitResultActivity>" + Environment.NewLine +
@"		<ns0:CreateAssignmentAndWaitResultActivity RequestProperties=""{x:Null}"" " +
@"x:Name=""createAssignmentAndWait_7e3ed79e3a8e4c02bedb6d780f61b416"">" + Environment.NewLine +
@"			<ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"				<ns0:PropertyValueCollection>" + Environment.NewLine +
@"					<ns0:PropertyValue Name=""Subject"">" + Environment.NewLine +
@"						<ns0:PropertyValue.Value>" + Environment.NewLine +
@"							<ns1:String xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">OZ OverDue Test 3</ns1:String>" + Environment.NewLine +
@"						</ns0:PropertyValue.Value>" + Environment.NewLine +
@"					</ns0:PropertyValue>" + Environment.NewLine +
@"					<ns0:PropertyValue Name=""UserId"">" + Environment.NewLine +
@"						<ns0:PropertyValue.Value>" + Environment.NewLine +
@"							<ns1:Int32 xmlns:ns1=""clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, " +
@"Culture=neutral, PublicKeyToken=b77a5c561934e089"">65</ns1:Int32>" + Environment.NewLine +
@"						</ns0:PropertyValue.Value>" + Environment.NewLine +
@"					</ns0:PropertyValue>" + Environment.NewLine +
@"				</ns0:PropertyValueCollection>" + Environment.NewLine +
@"			</ns0:CreateAssignmentAndWaitResultActivity.AssignmentProperties>" + Environment.NewLine +
@"		</ns0:CreateAssignmentAndWaitResultActivity>" + Environment.NewLine +
@"	</ns0:BlockActivity>" + Environment.NewLine +
@"</SequentialWorkflowActivity>"; 
			#endregion

			Activity wf = McWorkflowSerializer.GetObject<Activity>(xaml);

			WorkflowView view = CreateWorkflowView(wf);

			view.SaveWorkflowImage(@"C:\1.png", System.Drawing.Imaging.ImageFormat.Png);
		}

		/// <summary>
		/// Creates the workflow view.
		/// </summary>
		/// <param name="xaml">The xaml.</param>
		/// <returns></returns>
		public static WorkflowView CreateWorkflowView(string xaml)
		{
			if (xaml == null)
				throw new ArgumentNullException("xaml");

			return CreateWorkflowView(McWorkflowSerializer.GetObject<Activity>(xaml));
		} 

		/// <summary>
		/// Creates the workflow view.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		public static WorkflowView CreateWorkflowView(WorkflowInstanceEntity instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			return CreateWorkflowView(McWorkflowSerializer.GetObject<Activity>(instance.Xaml));
		} 

		/// <summary>
		/// Creates the workflow view.
		/// </summary>
		/// <param name="workflow">The workflow.</param>
		/// <returns></returns>
		public static WorkflowView CreateWorkflowView(Activity workflow)
		{
			if (workflow == null)
				throw new ArgumentNullException("workflow");

			WorkflowLoader loader = new WorkflowLoader(workflow);

			DesignSurface surface = new DesignSurface();
			surface.BeginLoad(loader);

			WorkflowView retVal = new WorkflowView(surface);

			return retVal;
		} 
		#endregion
	}
}
