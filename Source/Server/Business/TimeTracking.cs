using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.IBN.Database;
//using Mediachase.IbnNext;
using Mediachase.Ibn.Data.Services;

using Mediachase.IbnNext.TimeTracking;
using Mediachase.IBN.Business.SpreadSheet;
//using Mediachase.IbnNext.Activity;
using Mediachase.Ibn.Data.Meta;
//using Mediachase.IbnNext.Events;

namespace Mediachase.IBN.Business
{
	public class TimeTracking
	{
		#region GetListTimeTrackingItemsForAdd
		/// <summary>
		///  ObjectId, ObjectTypeId, ObjectName, BlockTypeInstanceId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListTimeTrackingItemsForAdd(int blockTypeInstanceId, DateTime startDate, int userId)
		{
			return DbTimeTracking.GetListTimeTrackingItemsForAdd(blockTypeInstanceId, startDate, userId);
		}
		public static DataTable GetListTimeTrackingItemsForAdd_DataTable(int blockTypeInstanceId, DateTime startDate, int userId)
		{
			return DbTimeTracking.GetListTimeTrackingItemsForAdd_DataTable(blockTypeInstanceId, startDate, userId);
		}
		#endregion

		#region GetWeekStart
		public static DateTime GetWeekStart(DateTime dt)
		{
			dt = dt.Date;
			int dow = (int)dt.DayOfWeek;
			int fdow = PortalConfig.PortalFirstDayOfWeek;
			fdow = (fdow == 7) ? 0 : fdow;

			int diff = dow - fdow;
			DateTime result;
			if (diff < 0)
				result = dt.AddDays(-(7 + diff));
			else
				result = dt.AddDays(-diff);

			if (result.Year < dt.Year)
				result = new DateTime(dt.Year, 1, 1);

			return result;
		}
		#endregion

		#region GetRealWeekStart
		public static DateTime GetRealWeekStart(DateTime dt)
		{
			dt = dt.Date;
			int dow = (int)dt.DayOfWeek;
			int fdow = PortalConfig.PortalFirstDayOfWeek;
			fdow = (fdow == 7) ? 0 : fdow;

			int diff = dow - fdow;
			DateTime result;
			if (diff < 0)
				result = dt.AddDays(-(7 + diff));
			else
				result = dt.AddDays(-diff);

			return result;
		}
		#endregion

		#region GetWeekEnd
		public static DateTime GetWeekEnd(DateTime dt)
		{
			int dow = (int)dt.DayOfWeek;
			int fdow = PortalConfig.PortalFirstDayOfWeek;
			fdow = (fdow == 7) ? 0 : fdow;

			int diff = dow - fdow;
			DateTime result;
			if (diff < 0)
				result = dt.AddDays(-(7 + diff));
			else
				result = dt.AddDays(-diff);
			result = result.AddDays(6);

			if (result.Year > dt.Year)
				result = new DateTime(dt.Year, 12, 31);

			return result;
		}
		#endregion

		#region GetRealWeekEnd
		public static DateTime GetRealWeekEnd(DateTime dt)
		{
			int dow = (int)dt.DayOfWeek;
			int fdow = PortalConfig.PortalFirstDayOfWeek;
			fdow = (fdow == 7) ? 0 : fdow;

			int diff = dow - fdow;
			DateTime result;
			if (diff < 0)
				result = dt.AddDays(-(7 + diff));
			else
				result = dt.AddDays(-diff);
			result = result.AddDays(6);
			return result;
		}
		#endregion

		#region GetDayNum
		public static int GetDayNum(DateTime dt)
		{
			DateTime RealWeekStart = GetRealWeekStart(dt);
			TimeSpan ts = dt.Subtract(RealWeekStart);
			return ts.Days + 1;
		}
		#endregion

		#region CanUpdate
		public static bool CanUpdate(DateTime startDate, int projectId)
		{
			bool retval = false;

			if (Configuration.TimeTrackingModule)
			{
				startDate = GetWeekStart(startDate);

				// O.R. [2008-07-25]
				TimeTrackingBlockTypeInstance inst = null;
				using (SkipSecurityCheckScope scope = Mediachase.Ibn.Data.Services.Security.SkipSecurityCheck())
				{
					inst = TimeTrackingManager.GetBlockTypeInstanceByProject(projectId);
				}
				if (inst != null)
				{
					TimeTrackingBlock[] blocks = TimeTrackingBlock.List(
						FilterElement.EqualElement("OwnerId", Security.CurrentUser.UserID),
						FilterElement.EqualElement("BlockTypeInstanceId", inst.PrimaryKeyId.Value),
						FilterElement.EqualElement("StartDate", startDate)
						);
					if (blocks.Length > 0)	// Block exists
					{
						TimeTrackingBlock block = blocks[0];
						SecurityService ss = block.GetService<SecurityService>();
						retval = ss.CheckUserRight(TimeTrackingManager.Right_Write);
					}
					else	// Block doesn't exist
					{
						SecurityService ss = inst.GetService<SecurityService>();
						retval = ss.CheckUserRight(TimeTrackingManager.Right_AddMyTTBlock) || ss.CheckUserRight(TimeTrackingManager.Right_AddAnyTTBlock);
					}
				}
			}

			return retval;
		}
		#endregion

		#region ResetObjectId
		// Resets ObjectId and ObjectTypeId in cls_TimeSheetEntry
		public static void ResetObjectId(int objectTypeId, int objectId)
		{
			DbTimeTracking.ResetObjectId(objectTypeId, objectId);
		}
		#endregion

		#region GetListTimeTrackingItemsForFinances
		/// <summary>
		///  TimeTrackingEntryId, UserId, ObjectTypeId, ObjectId, Title, 
		///  Day1, Day2, Day3, Day4, Day5, Day6, Day7, Total, TotalApproved, Rate, Cost, TaskTime
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListTimeTrackingItemsForFinances(int blockTypeInstanceId, DateTime startDate, int userId)
		{
			return DbTimeTracking.GetListTimeTrackingItemsForFinances(blockTypeInstanceId, startDate, userId);
		}
		public static DataTable GetListTimeTrackingItemsForFinances_DataTable(int blockTypeInstanceId, DateTime startDate, int userId)
		{
			return DbTimeTracking.GetListTimeTrackingItemsForFinances_DataTable(blockTypeInstanceId, startDate, userId);
		}
		#endregion

		#region RegisterFinances
		public static void RegisterFinances(TimeTrackingBlock block, string rowId, DateTime regDate)
		{
			if (!Configuration.TimeTrackingModule)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			if ((bool)block.Properties["AreFinancesRegistered"].Value)
				throw new NotSupportedException("Finances are already registered.");

			if (!TimeTrackingBlock.CheckUserRight(block, TimeTrackingManager.Right_RegFinances))
				throw new Mediachase.Ibn.AccessDeniedException();

			///  TimeTrackingEntryId, UserId, ObjectTypeId, ObjectId, Title, 
			///  Day1, Day2, Day3, Day4, Day5, Day6, Day7, Total, TotalApproved, Rate, Cost
			DataTable dt = GetListTimeTrackingItemsForFinances_DataTable(block.BlockTypeInstanceId, block.StartDate, block.OwnerId);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				// O.R. [2008-07-29]: We don't need to check the "Write" right for Entry and Block
				using (SkipSecurityCheckScope scope = Mediachase.Ibn.Data.Services.Security.SkipSecurityCheck())
				{
					foreach (DataRow row in dt.Rows)
					{
						// O.R. [2008-07-28]: Rate and TotalApproved may contain null values, so we need do assign them
						TimeTrackingEntry entry = MetaObjectActivator.CreateInstance<TimeTrackingEntry>(TimeTrackingEntry.GetAssignedMetaClass(), (int)row["TimeTrackingEntryId"]);
						entry.Properties["Rate"].Value = row["Rate"];
						entry.Properties["TotalApproved"].Value = row["TotalApproved"];
						entry.Save();

						double cost = (double)row["Cost"];
						if (cost == 0d)
							continue;

						// O.R. [2008-07-24] Now we use ProjectId 
						/*					ObjectTypes objectType = ObjectTypes.Timesheet;
											int objectId = (int)row["TimeTrackingEntryId"];
						 */
						ObjectTypes objectType = ObjectTypes.Project;
						int objectId = block.ProjectId.Value;

						if (row["ObjectTypeId"] != DBNull.Value && row["ObjectId"] != DBNull.Value)
						{
							objectType = (ObjectTypes)row["ObjectTypeId"];
							objectId = (int)row["ObjectId"];
						}

						// row["TotalApproved"] alwas has not null value
						ActualFinances.Create(objectId, objectType, regDate, rowId, cost, row["Title"].ToString(), block.PrimaryKeyId.Value, (double)row["TotalApproved"], (int)block.OwnerId);
					}

					block.Properties["AreFinancesRegistered"].Value = true;
					block.Save();
				}

				// Recalculate TotalMinutes and TotalApproved
				RecalculateProjectAndObjects(block);

				tran.Commit();
			}
		}
		#endregion

		#region UnRegisterFinances
		public static void UnRegisterFinances(TimeTrackingBlock block)
		{
			if (!Configuration.TimeTrackingModule)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			if (!(bool)block.Properties["AreFinancesRegistered"].Value)
				throw new NotSupportedException("Finances are not registered.");

			if (!TimeTrackingBlock.CheckUserRight(block, TimeTrackingManager.Right_UnRegFinances))
				throw new Mediachase.Ibn.AccessDeniedException();


			using (DbTransaction tran = DbTransaction.Begin())
			{
				ActualFinances.DeleteByBlockId(block.PrimaryKeyId.Value);

				// O.R. [2008-07-29]: We don't need to check the "Write" right for Block
				using (SkipSecurityCheckScope scope = Mediachase.Ibn.Data.Services.Security.SkipSecurityCheck())
				{
					block.Properties["AreFinancesRegistered"].Value = false;
					block.Save();
 				}

				// Recalculate TotalMinutes and TotalApproved
				RecalculateProjectAndObjects(block);

				tran.Commit();
			}
		}
		#endregion

		#region RecalculateProjectTaskTime
		/// <summary>
		/// This method calls if the TaskTime value of Event, Task, ToDo, Incident or Document was changed.
		/// It also calls after deleting the Event, Task, ToDo, Incident or Document.
		/// </summary>
		/// <param name="projectId"></param>
		public static void RecalculateProjectTaskTime(int projectId)
		{
			DbTimeTracking.RecalculateProjectTaskTime(projectId);
		}
		#endregion

		#region RecalculateObjectAndProject
		/// <summary>
		/// Recalculates the object TotalMinutes and TotalApproved.
		/// </summary>
		/// <param name="objectId">The object id.</param>
		/// <param name="objectTypeId">The object type id.</param>
		/// <param name="projectId">The project id.</param>
		private static void RecalculateObjectAndProject(int? objectId, int? objectTypeId, int projectId)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				if (objectId.HasValue && objectTypeId.HasValue)
				{
					DbTimeTracking.RecalculateObject(objectId.Value, objectTypeId.Value, projectId);
				}

				if (projectId > 0)
					DbTimeTracking.RecalculateProject(projectId);

				tran.Commit();
			}
		}
		#endregion

		#region RecalculateProjectAndObjects
		/// <summary>
		/// Recalculates the project and objects TotalMinutes and TotalApproved.
		/// </summary>
		/// <param name="block">The block.</param>
		private static void RecalculateProjectAndObjects(TimeTrackingBlock block)
		{
			int projectId = GetProjectIdByTimeTrackingBlock(block);

			if (projectId > 0)
			{
				TimeTrackingEntry[] entryList = TimeTrackingEntry.List(
					FilterElement.EqualElement("ParentBlockId", block.PrimaryKeyId.Value),
					FilterElement.IsNotNullElement("ObjectId"),
					FilterElement.IsNotNullElement("ObjectTypeId"));
				foreach (TimeTrackingEntry entry in entryList)
				{
					DbTimeTracking.RecalculateObject((int)entry.Properties["ObjectId"].Value, (int)entry.Properties["ObjectTypeId"].Value, block.ProjectId.Value);
				}

				DbTimeTracking.RecalculateProject(projectId);
			}
		}
		#endregion

		#region GetCurrentAndPrevStateIdByTimeTrackingBlock
		private static void GetCurrentAndPrevStateIdByTimeTrackingBlock(TimeTrackingBlock block, out int curStateId, out int prevStateId)
		{
			curStateId = -1;
			prevStateId = -1;

			StateMachineService stateMachine = block.GetService<StateMachineService>();
			if (stateMachine == null || stateMachine.StateMachine.States.Count == 0 || stateMachine.CurrentState == null)
				return;

			EventService eventService = block.GetService<EventService>();
			if (eventService != null)
			{
				// Detects that state is changed, find moFromStateName, toStateName
				MetaObject[] events = eventService.LoadEvents(TriggerContext.Current.TransactionId);
				StateMachineEventServiceArgs args = StateMachine.GetStateChangedEventArgs(events);
				if (args != null)
				{
					curStateId = StateMachineManager.GetState(TimeTrackingBlock.GetAssignedMetaClass(), args.CurrentState.Name).PrimaryKeyId.Value;
					prevStateId = StateMachineManager.GetState(TimeTrackingBlock.GetAssignedMetaClass(), args.PrevState.Name).PrimaryKeyId.Value;
				}
			}
		}
		#endregion

		#region GetFinalStateIdByTimeTrackingBlock
		private static int GetFinalStateIdByTimeTrackingBlock(TimeTrackingBlock block)
		{
			StateMachineService stateMachine = block.GetService<StateMachineService>();
			if (stateMachine == null || stateMachine.StateMachine.States.Count == 0)
				return -1;

			State finalState = stateMachine.StateMachine.States[stateMachine.StateMachine.States.Count - 1];

			MetaObject stateObject = StateMachineManager.GetState(TimeTrackingBlock.GetAssignedMetaClass(), finalState.Name);
			return stateObject.PrimaryKeyId.Value;
		}
		#endregion

		#region GetProjectIdByTimeTrackingBlock
		private static int GetProjectIdByTimeTrackingBlock(TimeTrackingBlock block)
		{
			return block.Properties["ProjectId"].Value != null ? (int)(PrimaryKeyId)block.Properties["ProjectId"].Value : -1;
			//TimeTrackingBlockTypeInstance instance = new TimeTrackingBlockTypeInstance(block.BlockTypeInstanceId);
			//return base.Properties["ProjectId"].Value != null ? (int)base.Properties["ProjectId"].Value : -1;
		}
		#endregion

		#region TriggerAction
		/// <summary>
		/// Recalculates the project TotalMinutes and TotalApproved.
		/// </summary>
		[TriggerAction("RecalculateProject", "Recalculates project fields by tracking block")]
		public static void RecalculateProject()
		{
			if (TriggerContext.Current == null)
				throw new ArgumentException("TriggerContext.Current == null");

			TimeTrackingBlock block = TriggerContext.Current.Parameter.MetaObject as TimeTrackingBlock;
			if (block == null)
				return;

			RecalculateProjectAndObjects(block);
		}

		/// <summary>
		/// Recalculates the object TotalMinutes and TotalApproved..
		/// </summary>
		[TriggerAction("RecalculateObject", "Recalculates object fields by tracking entry")]
		public static void RecalculateObject()
		{
			if (TriggerContext.Current == null)
				throw new ArgumentException("TriggerContext.Current == null");

			TimeTrackingEntry entry = TriggerContext.Current.Parameter.MetaObject as TimeTrackingEntry;
			if (entry == null)
				return;

			TimeTrackingBlock block = MetaObjectActivator.CreateInstance<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(), entry.ParentBlockId);

			int projectId = GetProjectIdByTimeTrackingBlock(block);

			//TimeTracking.RecalculateObject(int? objectId, int? objectTypeId, int projectId)
			//надо вызывать при создании TimeTrackingEntry (с хотя бы одним ненулевым Day1, Day2..., Day7)
			//при изменении TimeTrackingEntry (когда изменился хотя бы один Day1, Day2..., Day7) и 
			// при удалении TimeTrackingEntry (с хотя бы одним ненулевым Day1, Day2..., Day7)

			// Comment conition because can n't detects that all DayX set to zero
			if (TriggerContext.Current.Parameter.State == MetaObjectState.Created && (entry.Day1 != 0 || entry.Day2 != 0 || entry.Day3 != 0 ||
				entry.Day4 != 0 || entry.Day5 != 0 || entry.Day6 != 0 || entry.Day7 != 0) ||
				TriggerContext.Current.Parameter.State != MetaObjectState.Created)
			{
				RecalculateObjectAndProject(entry.Properties["ObjectId"].GetValue<int?>(),
					entry.Properties["ObjectTypeId"].GetValue<int?>(),
					projectId);
			}
		}

		public static void RegisterTriggers()
		{
			TriggerManager.AddTrigger(TimeTrackingBlock.GetAssignedMetaClass(), 
				new Trigger("RecalculateProjectByTimeTrackingBlock",
				"Recalculates project fields by time tracking block", false,false, true, string.Empty, "RecalculateProject"));

			TriggerManager.AddTrigger(TimeTrackingEntry.GetAssignedMetaClass(),
				new Trigger("RecalculateObjectByTimeTrackingEntry",
				"Recalculates project fields by time tracking block", true, true, true, string.Empty, "RecalculateObject"));
		}
		#endregion

		#region GetWeekItemsForUser
		/// <summary>
		/// Gets the week items for current user.
		/// </summary>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <returns></returns>
		public static WeekItemInfo[] GetWeekItemsForCurrentUser(DateTime from, DateTime to)
		{
			return GetWeekItemsForUser(from, to, Security.CurrentUser.UserID);
		}

		/// <summary>
		/// Gets the time tracking block min start date.
		/// </summary>
		/// <returns></returns>
		public static DateTime GetTimeTrackingBlockMinStartDate()
		{
			return GetTimeTrackingBlockMinStartDate(Security.CurrentUser.UserID);
		}

		/// <summary>
		/// Gets the min start date.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public static DateTime GetTimeTrackingBlockMinStartDate(int userId)
		{
			TimeTrackingBlock[] userBlocks = TimeTrackingBlock.List(
				new FilterElementCollection(FilterElement.EqualElement("OwnerId", userId)),
				new SortingElementCollection(SortingElement.Ascending("StartDate")), 0, 1);

			if (userBlocks.Length > 0)
				return userBlocks[0].StartDate;

			return DateTime.Now.AddDays(-210);
		}

		/// <summary>
		/// Gets the week items for user.
		/// </summary>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public static WeekItemInfo[] GetWeekItemsForUser(DateTime from, DateTime to, int userId)
		{
			List<WeekItemInfo> retVal = new List<WeekItemInfo>();

			// Load User TT blocks
			TimeTrackingBlock[] userBlocks = TimeTrackingBlock.List(FilterElement.EqualElement("OwnerId", userId),
				new IntervalFilterElement("StartDate", from, to));

			// Sort Block By Start Date
			Array.Sort<TimeTrackingBlock>(userBlocks, CompareTimeTrackingBlockByStartDate);

			// Create Week Item Info list
			DateTime currentWeekStart = GetRealWeekStart(from);

			// TODO: Current
			while (currentWeekStart < to)
			{
				// Calculate aggregated status of all blocks
				WeekItemStatus status = CalculateWeekStatusByBlocks(currentWeekStart, userBlocks);

				WeekItemInfo item = new WeekItemInfo(currentWeekStart, 
					Iso8601WeekNumber.GetWeekNumber(currentWeekStart.AddDays(3)),
					status);

				CalculateDayTotal(currentWeekStart, item, userBlocks);

				retVal.Add(item);

				// Go to next week
				currentWeekStart = currentWeekStart.AddDays(7);
			}

			return retVal.ToArray();
		}

		/// <summary>
		/// Compares the time tracking block by start date.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns></returns>
		private static int CompareTimeTrackingBlockByStartDate(TimeTrackingBlock x, TimeTrackingBlock y)
		{
			if (x == null)
				throw new ArgumentNullException("x");
			if (y == null)
				throw new ArgumentNullException("y");

			return x.StartDate.CompareTo(y.StartDate);
		}

		/// <summary>
		/// Calculates the week status by blocks.
		/// </summary>
		/// <param name="currentWeekStart">The current week start.</param>
		/// <param name="userBlocks">The user blocks.</param>
		/// <returns></returns>
		private static WeekItemStatus CalculateWeekStatusByBlocks(DateTime currentWeekStart, TimeTrackingBlock[] userBlocks)
		{
			DateTime currentWeekEnd = currentWeekStart.AddDays(7);

			WeekItemStatus status = WeekItemStatus.NotCompleted;

			foreach (TimeTrackingBlock block in userBlocks)
			{
				// Process if block.StartDate IN [currentWeekStart, currentWeekStart)
				if (block.StartDate < currentWeekStart)
					continue;
				if (block.StartDate >= currentWeekEnd)
					break;

				if ((bool)block["IsRejected"] && block.mc_State == "Initial")
				{
					// At least one block rejected
					status = WeekItemStatus.Rejected;
					break;
				}

				switch (block.mc_State)
				{
					case "Initial": // Initial - At least one block not sent to approve
						if (status == WeekItemStatus.NotCompleted || 
							(int)status > (int)WeekItemStatus.InProcess)
							status = WeekItemStatus.InProcess;
						break;
					case "Final": // Final - All blocks approved
						if (status == WeekItemStatus.NotCompleted)
							status = WeekItemStatus.Approved;
						break;
					default:
						if (status == WeekItemStatus.NotCompleted ||
							(int)status > (int)WeekItemStatus.Submitted)
							status = WeekItemStatus.Submitted;
						break;
				}
			}

			return status;
		}

		private static void CalculateDayTotal(DateTime currentWeekStart, WeekItemInfo item, TimeTrackingBlock[] userBlocks)
		{
			DateTime currentWeekEnd = currentWeekStart.AddDays(7);

			foreach (TimeTrackingBlock block in userBlocks)
			{
				// Process if block.StartDate IN [currentWeekStart, currentWeekStart)
				if (block.StartDate < currentWeekStart)
					continue;
				if (block.StartDate >= currentWeekEnd)
					continue;

				item.Day1 += block.Day1;
				item.Day2 += block.Day2;
				item.Day3 += block.Day3;
				item.Day4 += block.Day4;
				item.Day5 += block.Day5;
				item.Day6 += block.Day6;
				item.Day7 += block.Day7;
			}
		}
		#endregion
	}
}
