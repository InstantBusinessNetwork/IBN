using System;
using System.Collections;
using System.Data;

using Security = Mediachase.IBN.Business.Security;
using System.Collections.Generic;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.SpreadSheet;
using Mediachase.IBN.Database.SpreadSheet;
using System.IO;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Services;

namespace Mediachase.Ibn.Apps.ClioSoft
{
	public static class BusReport
	{
		#region GenerateProjectTaskBusinessScoreReport
		/// <summary>
		/// Generates the project task business score report.
		/// </summary>
		/// <param name="projectIdList">The project id list.</param>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <param name="completion">The completion.</param>
		/// <param name="completionFrom">The completion from.</param>
		/// <param name="completionTo">The completion to.</param>
		/// <returns></returns>
		public static ReportResult GenerateProjectTaskBusinessScoreReport(List<int> projectIdList,
			DateTime from, DateTime to, int completion, DateTime completionFromDate, DateTime completionToDate)
		{
			if (projectIdList == null)
				throw new ArgumentNullException("projectIdList");
				// test only;
				//projectIdList = new List<int>(new int[] { 624, 544, 665, 680, 686, 687, 688, 689, 681, 691, 781, 633, 809, 803, 801, 838, 407, 852, 849, 853, 841, 856, 819, 848 });

			TimeZone tz = Security.CurrentUser.CurrentTimeZone;
			completionFromDate = tz.ToUniversalTime(completionFromDate);
			completionToDate = tz.ToUniversalTime(completionToDate);
					
			ReportResult result = new ReportResult();

			foreach (int projectId in projectIdList)
			{
				// Get Project Title
				string projectTitleName = Project.GetProjectTitle(projectId);

				ReportResultItem projectItem = new ReportResultItem(ObjectTypes.Project, projectId, projectTitleName);

				#region Load Task List
				// Get Task(Id,Name) List by projectId and fill projectItem.Items
				using (IDataReader reader = DbReport.GetTasks(projectId, completion, completionFromDate, completionToDate))
				{
					while (reader.Read())
					{
						int taskId = (int)reader["TaskId"];
						string taskTitle = (string)reader["Title"];

						ReportResultItem taskItem = new ReportResultItem(ObjectTypes.Task, taskId, taskTitle);

						// Add taskItem to projectItem
						projectItem.ChildItems.Add(taskItem);
					}
				}
				#endregion

				#region Create SpreadSheetDocument and SpreadSheetView
				// Create a new Spread Sheet Document
				SpreadSheetDocument ssDocument = new SpreadSheetDocument(SpreadSheetDocumentType.Total);

				// Load Template
				ssDocument.Template.Load(Path.Combine(ProjectSpreadSheet.TemplateDirectory, "ProfitAndLossStatementSimple_RU.xml"));

				ProjectSpreadSheetRow[] projectSpreadSheets = ProjectSpreadSheetRow.List(projectId);
				ProjectSpreadSheetRow projectSpreadSheet = projectSpreadSheets[0];

				// Add user rows
				if (projectSpreadSheet.UserRows != string.Empty)
				{
					try
					{
						ssDocument.Template.LoadXml(projectSpreadSheet.UserRows);
					}
					catch (Exception ex)
					{
						System.Diagnostics.Trace.WriteLine(ex);
					}
				}

				SpreadSheetView ssView = new SpreadSheetView(ssDocument);
				#endregion

				// Load Business Score
				//BusinessScore[] businessScoreList = BusinessScore.List();
				List<ReportResultItem> taskWithoutFinancesRemoveIt = new List<ReportResultItem>();

				#region Calcualte Business Score
				// Calcualte Business Score
				foreach (ReportResultItem taskItem in projectItem.ChildItems)
				{
					// Cleare Spread Sheet Document
					ssView.Document.DeleteAllCells();

					// Get Actual Finances for current task + task/todo
					ActualFinances[] finances = ActualFinances.List(taskItem.ObjectId, taskItem.ObjectType, from, to);

					if (finances.Length > 0)
					{
						// Append Actual Finances to SpreadSheetDocument
						foreach (ActualFinances finance in finances)
						{
							string columnId = SpreadSheetView.GetColumnByDate(ssView.Document.DocumentType, finance.Date);
							string rowId = finance.RowId;

							Cell cell = ssView.Document.GetCell(columnId, rowId);
							if (cell == null)
							{
								cell = ssView.Document.AddCell(columnId, rowId, CellType.Common, 0);
							}

							cell.Value += finance.Value;
						}

						// Add Business Scope ReportResultItem and Calculate Business Scope total.
						// TODO: Localize ReportResultItem.Name
						taskItem.ChildItems.Add(new ReportResultItem(ObjectTypes.UNDEFINED, -1, "Доходы"/*"Revenues"*/));
						taskItem.ChildItems.Add(new ReportResultItem(ObjectTypes.UNDEFINED, -1, "Расходы"/*Expenses"*/));
						taskItem.ChildItems.Add(new ReportResultItem(ObjectTypes.UNDEFINED, -1, "Чистый доход"/*"NetIncome"*/));

						taskItem.ChildItems[0].Total = ssView["TT", "Revenues"];
						taskItem.ChildItems[1].Total = ssView["TT", "Expenses"];
						taskItem.ChildItems[2].Total = ssView["TT", "NetIncome"];

						// Copy NetIncome to total
						taskItem.Total = taskItem.ChildItems[2].Total;
					}
					else
					{
						taskWithoutFinancesRemoveIt.Add(taskItem);
					}
				} 

				// Remove Empty Tasks
				foreach (ReportResultItem item in taskWithoutFinancesRemoveIt)
				{
					projectItem.ChildItems.Remove(item);
				}
				#endregion

				projectItem.Total = CalculateTotal(projectItem.ChildItems);

				// Add projectItem to result
				result.Items.Add(projectItem);
			}

			// Calculate Report Total
			result.Total = CalculateTotal(result.Items);

			// Final return result
			return result;
		}
		#endregion

		#region CalculateTotal
		/// <summary>
		/// Calculates the total.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <returns></returns>
		private static double CalculateTotal(List<ReportResultItem> items)
		{
			double total = 0;

			foreach (ReportResultItem item in items)
			{
				total += item.Total;
			}

			return total;
		}
		#endregion

		#region GenerateBusinessScoreProjectTaskReport
		/// <summary>
		/// Generates the business score project task report.
		/// </summary>
		/// <param name="projectIdList">The project id list.</param>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <param name="completion">The completion.</param>
		/// <param name="completionFrom">The completion from.</param>
		/// <param name="completionTo">The completion to.</param>
		/// <returns></returns>
		public static ReportResult GenerateBusinessScoreProjectTaskReport(List<int> projectIdList,
			DateTime from, DateTime to, int completion, DateTime completionFromDate, DateTime completionToDate)
		{
			if (projectIdList == null)
				throw new ArgumentNullException("projectIdList");
			// test only;
			//projectIdList = new List<int>(new int[] { 624, 544, 665, 680, 686, 687, 688, 689, 681, 691, 781, 633, 809, 803, 801, 838, 407, 852, 849, 853, 841, 856, 819, 848 });

			TimeZone tz = Security.CurrentUser.CurrentTimeZone;
			completionFromDate = tz.ToUniversalTime(completionFromDate);
			completionToDate = tz.ToUniversalTime(completionToDate);

			ReportResult result = new ReportResult();

			ReportResultItem revenuesTotal = new ReportResultItem(ObjectTypes.UNDEFINED, -1, "Доходы"/*"Revenues"*/);
			ReportResultItem expensesTotal = new ReportResultItem(ObjectTypes.UNDEFINED, -1, "Расходы"/*Expenses"*/);
			ReportResultItem netIncomeTotal = new ReportResultItem(ObjectTypes.UNDEFINED, -1, "Чистый доход"/*"NetIncome"*/);

			result.Items.Add(revenuesTotal);
			result.Items.Add(expensesTotal);
			result.Items.Add(netIncomeTotal);

			foreach (int projectId in projectIdList)
			{
				// Get Project Title
				string projectTitleName = Project.GetProjectTitle(projectId);

				ReportResultItem revenuesProjectItem = new ReportResultItem(ObjectTypes.Project, projectId, projectTitleName);
				ReportResultItem expensesProjectItem = new ReportResultItem(ObjectTypes.Project, projectId, projectTitleName);
				ReportResultItem netIncomeProjectItem = new ReportResultItem(ObjectTypes.Project, projectId, projectTitleName);

				#region Load Task List
				// Get Task(Id,Name) List by projectId and fill projectItem.Items
				using (IDataReader reader = DbReport.GetTasks(projectId, completion, completionFromDate, completionToDate))
				{
					while (reader.Read())
					{
						int taskId = (int)reader["TaskId"];
						string taskTitle = (string)reader["Title"];

						ReportResultItem revenuesTaskItem = new ReportResultItem(ObjectTypes.Task, taskId, taskTitle);
						ReportResultItem expensesTaskItem = new ReportResultItem(ObjectTypes.Task, taskId, taskTitle);
						ReportResultItem netIncomeTaskItem = new ReportResultItem(ObjectTypes.Task, taskId, taskTitle);

						// Add taskItem to projectItem
						revenuesProjectItem.ChildItems.Add(revenuesTaskItem);
						expensesProjectItem.ChildItems.Add(expensesTaskItem);
						netIncomeProjectItem.ChildItems.Add(netIncomeTaskItem);
					}
				} 
				#endregion

				#region Create SpreadSheetDocument and SpreadSheetView
				// Create a new Spread Sheet Document
				SpreadSheetDocument ssDocument = new SpreadSheetDocument(SpreadSheetDocumentType.Total);

				// Load Template
				ssDocument.Template.Load(Path.Combine(ProjectSpreadSheet.TemplateDirectory, "ProfitAndLossStatementSimple_RU.xml"));

				ProjectSpreadSheetRow[] projectSpreadSheets = ProjectSpreadSheetRow.List(projectId);
				ProjectSpreadSheetRow projectSpreadSheet = projectSpreadSheets[0];

				// Add user rows
				if (projectSpreadSheet.UserRows != string.Empty)
				{
					try
					{
						ssDocument.Template.LoadXml(projectSpreadSheet.UserRows);
					}
					catch (Exception ex)
					{
						System.Diagnostics.Trace.WriteLine(ex);
					}
				}

				SpreadSheetView ssView = new SpreadSheetView(ssDocument);
				#endregion

				// Load Business Score
				//BusinessScore[] businessScoreList = BusinessScore.List();

				#region Calcualte Business Score
				List<int> taskWithoutFinancesRemoveIt = new List<int>();

				// Calcualte Business Score
				for (int index = 0; index < revenuesProjectItem.ChildItems.Count; index++)
				{
					int taskId = revenuesProjectItem.ChildItems[index].ObjectId;

					// Cleare Spread Sheet Document
					ssView.Document.DeleteAllCells();

					// Get Actual Finances for current task + task/todo
					ActualFinances[] finances = ActualFinances.List(taskId, ObjectTypes.Task, from, to);

					if (finances.Length > 0)
					{
						// Append Actual Finances to SpreadSheetDocument
						foreach (ActualFinances finance in finances)
						{
							string columnId = SpreadSheetView.GetColumnByDate(ssView.Document.DocumentType, finance.Date);
							string rowId = finance.RowId;

							Cell cell = ssView.Document.GetCell(columnId, rowId);
							if (cell == null)
							{
								cell = ssView.Document.AddCell(columnId, rowId, CellType.Common, 0);
							}

							cell.Value += finance.Value;
						}

						revenuesProjectItem.ChildItems[index].Total = ssView["TT", "Revenues"];
						expensesProjectItem.ChildItems[index].Total = ssView["TT", "Expenses"];
						netIncomeProjectItem.ChildItems[index].Total = ssView["TT", "NetIncome"];
					}
					else
					{
						taskWithoutFinancesRemoveIt.Add(index);
					}
				}

				taskWithoutFinancesRemoveIt.Reverse();
				foreach (int removeItemIndex in taskWithoutFinancesRemoveIt)
				{
					revenuesProjectItem.ChildItems.RemoveAt(removeItemIndex);
					expensesProjectItem.ChildItems.RemoveAt(removeItemIndex);
					netIncomeProjectItem.ChildItems.RemoveAt(removeItemIndex);
				}
				#endregion

				// Calculate Project total
				revenuesProjectItem.Total = CalculateTotal(revenuesProjectItem.ChildItems);
				expensesProjectItem.Total = CalculateTotal(expensesProjectItem.ChildItems);
				netIncomeProjectItem.Total = CalculateTotal(netIncomeProjectItem.ChildItems);

				// Add projectItem to Business Score
				revenuesTotal.ChildItems.Add(revenuesProjectItem);
				expensesTotal.ChildItems.Add(expensesProjectItem);
				netIncomeTotal.ChildItems.Add(netIncomeProjectItem);
			}

			revenuesTotal.Total = CalculateTotal(revenuesTotal.ChildItems);
			expensesTotal.Total = CalculateTotal(expensesTotal.ChildItems);
			netIncomeTotal.Total = CalculateTotal(netIncomeTotal.ChildItems);

			// Calculate Report Total
			result.Total = CalculateTotal(result.Items);

			// Final return result
			return result;
		}
		#endregion

		#region GenerateUserReport
		/// <summary>
		/// Generates the user report.
		/// </summary>
		/// <param name="userIdList">The user id list.</param>
		/// <param name="projectIdList">The project id list.</param>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <param name="completion">The completion.</param>
		/// <param name="completionFrom">The completion from.</param>
		/// <param name="completionTo">The completion to.</param>
		/// <returns></returns>
		public static ReportResult GenerateUserReport(List<int> userIdList,
			List<int> projectIdList,
			DateTime from, DateTime to,
			int completion, DateTime completionFromDate, DateTime completionToDate)
		{
			if (userIdList == null)
				throw new ArgumentNullException("userIdList");
			if (projectIdList == null)
				throw new ArgumentNullException("projectIdList");

			TimeZone tz = Security.CurrentUser.CurrentTimeZone;
			completionFromDate = tz.ToUniversalTime(completionFromDate);
			completionToDate = tz.ToUniversalTime(completionToDate);

			ReportResult result = new ReportResult();

			Dictionary<int, int> stateMachineFinalStates = new Dictionary<int, int>();

			// Foreach userId in userIdList
			foreach (int userId in userIdList)
			{
				string userName = User.GetUserName(userId);

				UserReportResultItem userItem = new UserReportResultItem(ObjectTypes.User, userId, userName);

				// Foreach projectId in projectIdList
				foreach (int projectId in projectIdList)
				{
					Dictionary<int, int> taskIdIndexDic = new Dictionary<int, int>();

					// TODO: Check userId has TTBlock (projectId, userId = owner, from, to) And AreFinancesRegistered is True
					if (TimeTrackingBlock.GetTotalCount(FilterElement.EqualElement("OwnerId", userId),
						FilterElement.EqualElement("ProjectId", projectId),
						new IntervalFilterElement("StartDate", from, to),
						FilterElement.EqualElement("AreFinancesRegistered", true)) == 0)
						continue;

					string projectTitle = Project.GetProjectTitle(projectId);

					UserReportResultItem projectItem = new UserReportResultItem(ObjectTypes.Project, projectId, projectTitle);

					using (IDataReader reader = DbReport.GetUserReportInfo(from, to, userId, projectId, completion, completionFromDate, completionToDate))
					{
						// Enum TTEntries (ObjectType is equal to Task And ToDo (append to owner task)) and return
						// - FinalApproved If State is Final State  = Day1 + ... + Day7
						// - TotalApproved
						// - Cost = TotalApproved * Rate
						// 

						while (reader.Read())
						{
							int taskId = (int)reader["TaskId"];
							string taskName = (string)reader["TaskName"];
							int mc_StateMachineId = (int)reader["mc_StateMachineId"];
							int mc_StateId = (int)reader["mc_StateId"];
							DateTime startDate = (DateTime)reader["StartDate"];
							double day1 = (double)reader["Day1"];
							double day2 = (double)reader["Day2"];
							double day3 = (double)reader["Day3"];
							double day4 = (double)reader["Day4"];
							double day5 = (double)reader["Day5"];
							double day6 = (double)reader["Day6"];
							double day7 = (double)reader["Day7"];
							double totalApproved = (double)reader["TotalApproved"];
							decimal rate = (decimal)reader["Rate"];

							bool isCurrentStateFinal = false;

							#region Check Final State Cache
							if (!stateMachineFinalStates.ContainsKey(mc_StateMachineId))
							{
								int finalStateId = StateMachineManager.GetFinalStateId(DataContext.Current.GetMetaClass("TimeTrackingBlock"), mc_StateMachineId);
								stateMachineFinalStates.Add(mc_StateMachineId, finalStateId);
							}
							#endregion

							isCurrentStateFinal = (stateMachineFinalStates[mc_StateMachineId] == mc_StateId);

							UserReportResultItem taskItem;

							if (taskIdIndexDic.ContainsKey(taskId))
								taskItem = (UserReportResultItem)projectItem.ChildItems[taskIdIndexDic[taskId]];
							else
							{
								taskItem = new UserReportResultItem(ObjectTypes.Task, taskId, taskName);
								projectItem.ChildItems.Add(taskItem);

								taskIdIndexDic.Add(taskId, projectItem.ChildItems.Count - 1);
							}

							if (isCurrentStateFinal)
								taskItem.ApprovedByManager += (day1 + day2 + day3 + day4 + day5 + day6 + day7);
							else
								taskItem.ApprovedByManager += 0;

							taskItem.Total += totalApproved;
							taskItem.Cost += totalApproved * (double)rate / 60;
						}
					}

					// Calculate Total projectItem
					CalculateTotalForUserReportResultItem(projectItem);

					userItem.ChildItems.Add(projectItem);
				}

				// Calculate Total userItem
				CalculateTotalForUserReportResultItem(userItem);

				result.Items.Add(userItem);
			}

			return result;
		}

		/// <summary>
		/// Calculates the total for user report result item.
		/// </summary>
		/// <param name="parentItem">The parent item.</param>
		private static void CalculateTotalForUserReportResultItem(UserReportResultItem parentItem)
		{
			parentItem.Total = 0;
			parentItem.Cost = 0;
			parentItem.ApprovedByManager = 0;

			foreach (UserReportResultItem item in parentItem.ChildItems)
			{
				parentItem.Total += item.Total;
				parentItem.Cost += item.Cost;
				parentItem.ApprovedByManager += item.ApprovedByManager;
			}
		}
		#endregion

		#region GetProjectManagers
		/// <summary>
		/// Gets the project managers.
		/// </summary>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <returns>ManagerId, ManagerName</returns>
		public static IDataReader GetProjectManagers(DateTime fromDate, DateTime toDate)
		{
			return DbReport.GetProjectManagers(fromDate, toDate);
		}
		#endregion

		#region GetClients
		/// <summary>
		/// Gets the clients.
		/// </summary>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <param name="managerList">The manager list.</param>
		/// <returns>ClientId, ClientName</returns>
		public static IDataReader GetClients(DateTime fromDate, DateTime toDate, string[] managerList)
		{
			string managers = String.Join(",", managerList);
			return DbReport.GetClients(fromDate, toDate, managers);
		}
		#endregion

		#region GetProjectGroups
		/// <summary>
		/// Gets the project groups.
		/// </summary>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <param name="managerList">The manager list.</param>
		/// <param name="clientList">The client list.</param>
		/// <returns>ProjectGroupId, ProjectGroupName</returns>
		public static IDataReader GetProjectGroups(DateTime fromDate, DateTime toDate, string[] managerList, string[] clientList)
		{
			string managers = String.Join(",", managerList);
			string clients = String.Join(",", clientList);

			return DbReport.GetProjectGroups(fromDate, toDate, managers, clients);
		}
		#endregion

		#region GetProjects
		/// <summary>
		/// Gets the projects.
		/// </summary>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <param name="managerList">The manager list.</param>
		/// <param name="clientList">The client list.</param>
		/// <param name="projectGroupList">The project group list.</param>
		/// <param name="completion">The completion.</param>
		/// <param name="completionFromDate">The completion from date.</param>
		/// <param name="completionToDate">The completion to date.</param>
		/// <returns></returns>
		public static IDataReader GetProjects(DateTime fromDate, DateTime toDate, string[] managerList, string[] clientList, string[] projectGroupList, int completion, DateTime completionFromDate, DateTime completionToDate)
		{
			TimeZone tz = Security.CurrentUser.CurrentTimeZone;
			completionFromDate = tz.ToUniversalTime(completionFromDate);
			completionToDate = tz.ToUniversalTime(completionToDate);

			string managers = String.Join(",", managerList);
			string clients = String.Join(",", clientList);
			string projectGroups = String.Join(",", projectGroupList);

			return DbReport.GetProjects(fromDate, toDate, managers, clients, projectGroups, completion, completionFromDate, completionToDate);
		}
		#endregion

		#region GetUsers
		/// <summary>
		/// Gets the users.
		/// </summary>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <returns>UserId, UserName</returns>
		public static IDataReader GetUsers(DateTime fromDate, DateTime toDate)
		{
			return DbReport.GetUsers(fromDate, toDate);
		}
		#endregion

		#region GetClientsByUser
		/// <summary>
		/// Gets the clients by user.
		/// </summary>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <param name="userList">The user list.</param>
		/// <returns>ClientId, ClientName</returns>
		public static IDataReader GetClientsByUser(DateTime fromDate, DateTime toDate, string[] userList)
		{
			string users = String.Join(",", userList);
			return DbReport.GetClientsByUser(fromDate, toDate, users);
		}
		#endregion

		#region GetProjectGroupsByUser
		/// <summary>
		/// Gets the project groups by user.
		/// </summary>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <param name="userList">The user list.</param>
		/// <param name="clientList">The client list.</param>
		/// <returns>ProjectGroupId, ProjectGroupName</returns>
		public static IDataReader GetProjectGroupsByUser(DateTime fromDate, DateTime toDate, string[] userList, string[] clientList)
		{
			string users = String.Join(",", userList);
			string clients = String.Join(",", clientList);

			return DbReport.GetProjectGroupsByUser(fromDate, toDate, users, clients);
		}
		#endregion

		#region GetProjectsByUser
		/// <summary>
		/// Gets the projects by user.
		/// </summary>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <param name="userList">The user list.</param>
		/// <param name="clientList">The client list.</param>
		/// <param name="projectGroupList">The project group list.</param>
		/// <param name="completion">The completion.</param>
		/// <param name="completionFromDate">The completion from date.</param>
		/// <param name="completionToDate">The completion to date.</param>
		/// <returns></returns>
		public static IDataReader GetProjectsByUser(DateTime fromDate, DateTime toDate, string[] userList, string[] clientList, string[] projectGroupList, int completion, DateTime completionFromDate, DateTime completionToDate)
		{
			TimeZone tz = Security.CurrentUser.CurrentTimeZone;
			completionFromDate = tz.ToUniversalTime(completionFromDate);
			completionToDate = tz.ToUniversalTime(completionToDate);

			string users = String.Join(",", userList);
			string clients = String.Join(",", clientList);
			string projectGroups = String.Join(",", projectGroupList);

			return DbReport.GetProjectsByUser(fromDate, toDate, users, clients, projectGroups, completion, completionFromDate, completionToDate);
		}
		#endregion
	}
}
