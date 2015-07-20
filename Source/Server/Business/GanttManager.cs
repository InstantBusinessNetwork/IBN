using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

using Mediachase.Ibn;
using Mediachase.Ibn.Web.Drawing.Gantt;
using Mediachase.IBN.Business.SpreadSheet;

namespace Mediachase.IBN.Business
{
	public enum GanttItem
	{
		PointStart,
		PointFinish,
		PointActualStart,
		PointActualFinish,
		Interval,
		IntervalActual,
		IntervalProgress,
		IntervalSummary,
		IntervalProject,
		PointMilestone,
		PointMilestoneBasePlanRight,
		PointMilestoneBasePlanLeft,
	}

	#region public class GanttObject
	public class GanttObject
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public bool IsProject { get; set; }
		public bool IsCollapsed { get; set; }
		public bool IsCompleted { get; set; }
		public int MilestonesCount { get; set; }
		public DateTime Start { get; set; }
		public DateTime Finish { get; set; }
	}
	#endregion

	public sealed class GanttManager
	{
		public const int HeaderItemHeight = 13;
		public const int HeaderHeight = HeaderItemHeight * 2 + 3;
		public const int ItemHeight = 19;
		public const int PortionWidth = 336;
		public const int PortionHeight = 336;

		public static int GetPortionX(int projectId, DateTime date)
		{
			GanttView gantt = CreateGanttView(projectId, 0, 0);
			return gantt.GetPortionX(date, PortionWidth);
		}

		#region public GetLegendItems(bool isTaskView, bool includeBasePlan)
		public static GanttItem[] GetLegendItems(bool isAnalysisChart, bool includeBasePlan)
		{
			List<GanttItem> items = new List<GanttItem>();

			if (!isAnalysisChart)
			{
				items.Add(GanttItem.PointActualStart);
				items.Add(GanttItem.Interval);
				items.Add(GanttItem.IntervalActual);
				items.Add(GanttItem.IntervalProgress);
				items.Add(GanttItem.IntervalSummary);
			}
			else
			{
				items.Add(GanttItem.IntervalProject);
			}

			items.Add(GanttItem.PointMilestone);

			if (includeBasePlan)
			{
				items.Add(GanttItem.PointMilestoneBasePlanRight);
				items.Add(GanttItem.PointMilestoneBasePlanLeft);
			}

			return items.ToArray();
		}
		#endregion

		#region public GetAnalysisDataTable(int originalPlanSlotId, int basePlanSlotId)
		public static DataTable GetAnalysisDataTable(int originalPlanSlotId, int basePlanSlotId)
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;

			dt.Columns.Add("ProjectId", typeof(int));
			dt.Columns.Add("TaskId", typeof(int));
			dt.Columns.Add("Title", typeof(string));
			dt.Columns.Add("IsCollapsed", typeof(bool));
			dt.Columns.Add("MilestonesCount", typeof(int));

			foreach (GanttObject item in GetAnalysisObjects(originalPlanSlotId, basePlanSlotId))
			{
				DataRow dr = dt.NewRow();

				dr["ProjectId"] = item.IsProject ? item.Id : -1;
				dr["TaskId"] = item.IsProject ? -1 : item.Id;
				dr["Title"] = item.Title;
				dr["IsCollapsed"] = item.IsCollapsed;
				dr["MilestonesCount"] = item.IsProject ? item.MilestonesCount : -1;

				dt.Rows.Add(dr);
			}

			return dt;
		}
		#endregion

		#region public GetAnalysisObjects(int originalPlanSlotId, int basePlanSlotId)
		public static IList<GanttObject> GetAnalysisObjects(int originalPlanSlotId, int basePlanSlotId)
		{
			List<GanttObject> ret = new List<GanttObject>();

			IFormatProvider provider = CultureInfo.InvariantCulture;

			List<int> projects = new List<int>();
			UserLightPropertyCollection pc = Security.CurrentUser.Properties;
			switch (pc["Report_ProjectListType"])
			{
				case "Custom":
					string projectList = pc["Report_ProjectListData"];
					if (!string.IsNullOrEmpty(projectList))
					{
						foreach (string id in projectList.Split(';'))
						{
							int projectId;
							if (int.TryParse(id.Trim(), out projectId))
							{
								projects.Add(projectId);
							}
						}
					}
					break;

				case "Portfolio":
					string portfolioId = pc["Report_ProjectListData"];
					DataTable dt = Project.GetListProjectGroupedByPortfolio(int.Parse(portfolioId, provider), 0, 0);
					foreach (DataRow row in dt.Rows)
					{
						int projectId = (int)row["ProjectId"];
						if (projectId > 0)
							projects.Add(projectId);
					}
					break;

				default: // All projects
					using (IDataReader reader = Project.GetListProjects())
					{
						while (reader.Read())
						{
							projects.Add((int)reader["ProjectId"]);
						}
					}
					break;
			}

			List<int> collapsedProjects = new List<int>();
			string collapsedProjectList = pc["Report_CollapsedProjectsList"];
			if (!string.IsNullOrEmpty(collapsedProjectList))
			{
				foreach (string id in collapsedProjectList.Split(';'))
				{
					int projectId;
					if(int.TryParse(id.Trim(), out projectId))
					{
						if (!collapsedProjects.Contains(projectId))
						{
							collapsedProjects.Add(projectId);
						}
					}
				}
			}

			foreach (int projectId in projects)
			{
				GanttObject project = new GanttObject();

				project.Id = projectId;
				project.IsProject = true;
				project.IsCollapsed = collapsedProjects.Contains(projectId);
				project.MilestonesCount = Task.TasksGetMilestonesCount(projectId);

				// O.R. [2009-03-30]: if current user is PM, then he can got AccessDeniedException for some projects
				try
				{
					using (IDataReader reader = Project.GetProject(projectId))
					{
						if (reader.Read())
						{
							project.Title = reader["Title"].ToString();
							project.Start = (DateTime)reader["TargetStartDate"];
							project.Finish = (DateTime)reader["TargetFinishDate"];
							project.IsCompleted = !(bool)reader["IsActive"];
						}
					}
				}
				catch (AccessDeniedException)
				{
					continue;
				}

				ret.Add(project);

				// Load milestones
				Dictionary<int, DateTime> originalPlan = null;
				if (originalPlanSlotId > 0)
					originalPlan = ProjectSpreadSheet.GetTaskHash(projectId, originalPlanSlotId);

				Dictionary<int, DateTime> basePlan = null;
				if (basePlanSlotId > 0)
					basePlan = ProjectSpreadSheet.GetTaskHash(projectId, basePlanSlotId);

				int milestonesCount = 0;
				using (IDataReader reader = Task.GetListTasksByProject(projectId))
				{
					while (reader.Read())
					{
						if ((bool)reader["IsMilestone"])
						{
							GanttObject milestone = new GanttObject();

							milestone.Id = (int)reader["TaskId"];
							milestone.IsProject = false;
							milestone.IsCompleted = (bool)reader["IsCompleted"];
							milestone.Title = reader["Title"].ToString();

							if (originalPlan != null)
							{
								if (originalPlan.ContainsKey(milestone.Id))
									milestone.Start = originalPlan[milestone.Id];
							}
							else
								milestone.Start = (DateTime)reader["StartDate"];

							milestone.Finish = milestone.Start;

							if (basePlan != null && basePlan.ContainsKey(milestone.Id))
								milestone.Finish = basePlan[milestone.Id];

							if (milestone.Start != DateTime.MinValue)
							{
								milestonesCount++;
								if (!project.IsCollapsed)
									ret.Add(milestone);
							}
						}
					}
				}

				project.MilestonesCount = milestonesCount;
			}

			return ret;
		}
		#endregion


		#region public static byte[] RenderLegendItem(string styleFilePath, GanttItem item, bool completed)
		public static byte[] RenderLegendItem(string styleFilePath, GanttItem item, bool completed)
		{
			byte[] ret = null;

			DateTime startDate = DateTime.Now.Date;
			GanttView gantt = CreateGanttView(startDate, DayOfWeek.Monday, 0, ItemHeight);

			Element spanElement = gantt.CreateSpanElement(null, null, null);

			DateTime pointDate = startDate.AddHours(36);
			DateTime intervalStart = startDate.AddHours(12);
			DateTime intervalFinish = intervalStart.AddDays(2);

			string tag = completed ? "Completed" : "";

			switch (item)
			{
				case GanttItem.PointStart:
					gantt.CreatePointElement(spanElement, pointDate, null, "Start", tag);
					break;
				case GanttItem.PointFinish:
					gantt.CreatePointElement(spanElement, pointDate, null, "Finish", tag);
					break;
				case GanttItem.PointActualStart:
					gantt.CreatePointElement(spanElement, pointDate, null, "ActualStart", tag);
					break;
				case GanttItem.PointActualFinish:
					gantt.CreatePointElement(spanElement, pointDate, null, "ActualFinish", tag);
					break;
				case GanttItem.Interval:
					gantt.CreateIntervalElement(spanElement, intervalStart, intervalFinish, null, null, tag);
					break;
				case GanttItem.IntervalActual:
					gantt.CreateIntervalElement(spanElement, intervalStart, intervalFinish, null, "Actual", tag);
					break;
				case GanttItem.IntervalProgress:
					gantt.CreateIntervalElement(spanElement, intervalStart, intervalFinish, null, "Progress", tag);
					break;
				case GanttItem.IntervalSummary:
				case GanttItem.IntervalProject:
					Element interval = gantt.CreateIntervalElement(spanElement, intervalStart, intervalFinish, null, "Summary", tag);
					gantt.CreatePointElement(interval, intervalStart, null, "SummaryStart", tag);
					gantt.CreatePointElement(interval, intervalFinish, null, "SummaryFinish", tag);
					break;
				case GanttItem.PointMilestone:
					AddMilestone(gantt, spanElement, pointDate, null, null, tag);
					break;
				case GanttItem.PointMilestoneBasePlanRight:
					AddMilestone(gantt, spanElement, intervalStart, intervalFinish, null, tag);
					break;
				case GanttItem.PointMilestoneBasePlanLeft:
					AddMilestone(gantt, spanElement, intervalFinish, intervalStart, null, tag);
					break;
			}

			#region Render

			using (MemoryStream stream = new MemoryStream())
			{
				gantt.LoadStyleSheetFromFile(styleFilePath);
				gantt.ApplyStyleSheet();

				gantt.RenderPortion(new Point(0, 0), new Size(24 * 3, Convert.ToInt32(ItemHeight)), 0, 0, ImageFormat.Png, stream);

				ret = stream.ToArray();
			}

			#endregion

			return ret;
		}
		#endregion

		#region public static byte[] RenderAnalysisChart(int originalPlanSlotId, int basePlanSlotId, bool generateDataXml, string styleFilePath, int portionX, int portionY, int itemsPerPage, int pageNumber)
		public static byte[] RenderAnalysisChart(int originalPlanSlotId, int basePlanSlotId, bool generateDataXml, string styleFilePath, int portionX, int portionY, int itemsPerPage, int pageNumber)
		{
			byte[] result = null;

			IList<GanttObject> items = GetAnalysisObjects(originalPlanSlotId, basePlanSlotId);
			if (items.Count > 0)
			{
				GanttView gantt = CreateGanttView(items[0].Id, HeaderItemHeight, ItemHeight);

				if (portionY >= 0)
				{
					#region Add data

					foreach (GanttObject item in items)
					{
						Element spanElement = gantt.CreateSpanElement(null, null, null);

						string id = string.Format(CultureInfo.InvariantCulture, "{0}{1}", item.IsProject ? "Project" : "Task", item.Id);
						string tag = item.IsCompleted ? "Completed" : null;

						if (item.IsProject)
						{
							AddInterval2(gantt, spanElement, item.Start, item.Finish, id, "Summary", tag);
						}
						else
						{
							AddMilestone(gantt, spanElement, item.Start, item.Finish, id, tag);
						}
					}

					#endregion
				}

				result = Render(gantt, generateDataXml, styleFilePath, portionX, portionY, PortionWidth, PortionHeight, itemsPerPage, pageNumber);
			}

			return result;
		}
		#endregion

		#region public static byte[] RenderChart(int projectId, int basePlanSlotId, bool generateDataXml, string styleFilePath, int portionX, int portionY, int itemsPerPage, int pageNumber)
		public static byte[] RenderChart(int projectId, int basePlanSlotId, bool generateDataXml, string styleFilePath, int portionX, int portionY, int itemsPerPage, int pageNumber)
		{
			GanttView gantt = CreateGanttView(projectId, HeaderItemHeight, ItemHeight);

			if (portionY >= 0)
			{
				#region Add data

				#region Load calendar

				int calendarId = -1;
				using (IDataReader reader = Project.GetProject(projectId))
				{
					if (reader.Read())
					{
						calendarId = (int)reader["CalendarId"];
					}
				}
				if (calendarId > 0)
				{
					// Load rules by day of week
					ArrayList days = new ArrayList();
					for (byte i = 1; i <= 7; i++)
						days.Add(i);
					using (IDataReader reader = Mediachase.IBN.Business.Calendar.GetListWeekdayHours(calendarId))
					{
						while (reader.Read())
						{
							byte b = (byte)reader["DayOfWeek"];
							days.Remove(b);
						}
					}
					foreach (byte b in days)
					{
						gantt.CreateDayElement(ConvertDayOfWeek(b), true);
					}

					// Load exceptions
					using (IDataReader reader = Mediachase.IBN.Business.Calendar.GetListExceptionHoursByCalendar(calendarId))
					{
						while (reader.Read())
						{
							DateTime fromDate = (DateTime)reader["FromDate"];
							DateTime toDate = (DateTime)reader["ToDate"];
							bool holiday = (reader["FromTime"] == DBNull.Value && reader["ToTime"] == DBNull.Value);
							for (DateTime date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
								gantt.CreateDateElement(date, holiday);
						}
					}
				}

				#endregion

				#region Load base plan

				Dictionary<int, DateTime> basePlanTaskHash = null;
				if (basePlanSlotId > 0)
					basePlanTaskHash = SpreadSheet.ProjectSpreadSheet.GetTaskHash(projectId, basePlanSlotId);

				#endregion

				List<string> ids = new List<string>();

				#region Load tasks

				DataTable dt = Task.GetListTasksByProjectCollapsedDataTable(projectId);
				dt.AcceptChanges();

				for (int i = 0; i < dt.Rows.Count; i++)
				{
					DataRow dr = dt.Rows[i];

					if (dr.RowState != DataRowState.Deleted)
					{
						Element spanElement = gantt.CreateSpanElement(null, null, null);

						int taskId = (int)dr["TaskId"];
						string id = taskId.ToString(CultureInfo.InvariantCulture);
						ids.Add(id);
						DateTime start = (DateTime)dr["StartDate"];
						bool isMilestone = (bool)dr["IsMilestone"];
						bool isSummary = (bool)dr["IsSummary"];
						bool isCompleted = (bool)dr["IsCompleted"];
						string type = isSummary ? "Summary" : null;
						string tag = isCompleted ? "Completed" : null;

						if (isMilestone)
						{
							DateTime? basePlanDate = null;

							if (basePlanTaskHash != null && basePlanTaskHash.ContainsKey(taskId))
								basePlanDate = basePlanTaskHash[taskId];

							AddMilestone(gantt, spanElement, start, basePlanDate, id, tag);
						}
						else
						{
							AddInterval(dr, gantt, spanElement, "StartDate", "FinishDate", id, type, tag);

							if (!isSummary)
							{
								AddInterval(dr, gantt, spanElement, "ActualStartDate", "ActualFinishDate", null, "Actual", tag);

								int progress = (int)dr["PercentCompleted"];
								if (progress > 0)
								{
									DateTime finish = (DateTime)dr["FinishDate"];
									TimeSpan interval = finish - start;
									finish = start + new TimeSpan(interval.Ticks * progress / 100);
									AddInterval2(gantt, spanElement, start, finish, null, "Progress", tag);
								}
							}
						}
					}
				}

				#endregion

				#region Load relations

				using (IDataReader reader = Project.GetListTaskLinksByProject(projectId))
				{
					while (reader.Read())
					{
						//string linkId = reader["LinkId"].ToString();
						string succId = reader["SuccId"].ToString();
						string predId = reader["PredId"].ToString();

						if (ids.Contains(succId) && ids.Contains(predId))
							gantt.CreateRelationElement(null, null, null, predId, succId);
					}
				}

				#endregion

				#endregion
			}

			return Render(gantt, generateDataXml, styleFilePath, portionX, portionY, PortionWidth, PortionHeight, itemsPerPage, pageNumber);
		}
		#endregion


		#region internal static byte[] Render(GanttView ganttView, bool generateDataXml, string styleFilePath, int portionX, int portionY, int portionWidth, int portionHeight, int itemsPerPage, int pageNumber)
		internal static byte[] Render(GanttView ganttView, bool generateDataXml, string styleFilePath, int portionX, int portionY, int portionWidth, int portionHeight, int itemsPerPage, int pageNumber)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				if (generateDataXml)
				{
					ganttView.GenerateDataXml(Encoding.UTF8, stream);
				}
				else
				{
					ganttView.LoadStyleSheetFromFile(styleFilePath);
					ganttView.ApplyStyleSheet();

					if (portionY < 0)
						portionHeight = Convert.ToInt32(ganttView.CalculateHeadHeight());

					if (portionHeight > 0 && portionWidth > 0)
						ganttView.RenderPortion(new Point(portionX, portionY), new Size(portionWidth, portionHeight), itemsPerPage, pageNumber, ImageFormat.Png, stream);
				}
				return stream.ToArray();
			}
		}
		#endregion


		private GanttManager()
		{
		}

		#region private CreateGanttView(int projectId, float headerItemHeight, float itemHeight)
		private static GanttView CreateGanttView(int projectId, float headerItemHeight, float itemHeight)
		{
			return CreateGanttView((Task.GetMinStartDate(projectId)).Date, ConvertDayOfWeek(PortalConfig.PortalFirstDayOfWeek), headerItemHeight, itemHeight);
		}
		#endregion
		#region private CreateGanttView(DateTime startDate, float headerItemHeight, float itemHeight)
		private static GanttView CreateGanttView(DateTime startDate, DayOfWeek firstDayOfWeek, float headerItemHeight, float itemHeight)
		{
			GanttView gantt = new GanttView(headerItemHeight, itemHeight);

			gantt.CreateDataElement(startDate);
			gantt.CreateAxisElement(ScaleLevel.Week, 1, "dd MMMM yyyy", firstDayOfWeek);
			gantt.CreateAxisElement(ScaleLevel.Day, 1, "dd", null);

			return gantt;
		}
		#endregion

		#region private AddMilestone(GanttView gantt, Element parent, DateTime date, DateTime? basePlanDate, string id, string tag)
		private static void AddMilestone(GanttView gantt, Element parent, DateTime date, DateTime? basePlanDate, string id, string tag)
		{
			string type = "Milestone";

			gantt.CreatePointElement(parent, date, id, type, tag);

			if (basePlanDate != null && basePlanDate.Value != date)
			{
				string direction = basePlanDate.Value > date ? "Right" : "Left";

				PointElement small = gantt.CreatePointElement(parent, date, null, type, "BasePlanSmall");
				small.AddAttribute("direction", direction);

				PointElement large = gantt.CreatePointElement(parent, basePlanDate.Value, null, type, "BasePlanLarge");
				large.AddAttribute("direction", direction);
			}
		}
		#endregion

		#region private AddInterval(DataRow dr, GanttView gantt, Element parent, string startName, string finishName, string id, string type, string tag)
		private static Element AddInterval(DataRow dr, GanttView gantt, Element parent, string startName, string finishName, string id, string type, string tag)
		{
			return AddInterval2(gantt, parent, GetDate(dr, startName), GetDate(dr, finishName), id, type, tag);
		}
		#endregion

		#region private AddInterval2(GanttView gantt, Element parent, DateTime? start, DateTime? finish, string id, string type, string tag)
		private static Element AddInterval2(GanttView gantt, Element parent, DateTime? start, DateTime? finish, string id, string type, string tag)
		{
			Element child = null;

			if (start != null && finish != null)
			{
				child = gantt.CreateIntervalElement(parent, start.Value, finish.Value, id, type, tag);
				if (type == "Summary")
				{
					gantt.CreatePointElement(child, start.Value, null, type + "Start", tag);
					gantt.CreatePointElement(child, finish.Value, null, type + "Finish", tag);
				}
			}
			else
			{
				if (start != null)
				{
					child = gantt.CreatePointElement(parent, start.Value, id, type + "Start", tag);
				}
				if (finish != null)
				{
					child = gantt.CreatePointElement(parent, finish.Value, id, type + "Finish", tag);
				}
			}

			return child;
		}
		#endregion

		#region private GetDate(DataRow dr, string name)
		private static DateTime? GetDate(DataRow dr, string name)
		{
			DateTime? ret = null;

			object value = dr[name];
			if (value != null && value != DBNull.Value)
				ret = (DateTime)value;

			return ret;
		}
		#endregion

		#region private ConvertDayOfWeek(int dayOfWeek)
		private static DayOfWeek ConvertDayOfWeek(int dayOfWeek)
		{
			return dayOfWeek == 7 ? DayOfWeek.Sunday : (DayOfWeek)dayOfWeek;
		}
		#endregion
	}
}
