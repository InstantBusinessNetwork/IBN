using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

using Mediachase.IBN.Database;


namespace Mediachase.IBN.Business
{
	public class Gantt
	{
		private Gantt()
		{
		}

		#region GetData()
		public static string GetData(int projectId)
		{
			StringBuilder sb = new StringBuilder();
			XmlTextWriter w = new XmlTextWriter(new StringWriter(sb));

			w.WriteStartElement("Gantt");
			if(Project.IsWebGanttChartEnabled())
			{
				ArrayList tasks = new ArrayList();

				int calendarId;
				using(IDataReader reader = Project.GetProject(projectId))
				{
					reader.Read();
					calendarId = DBCommon.NullToInt32(reader["CalendarId"]);
				}

				// Save project calendar
				w.WriteStartElement("Calendar");
				// Save workdays
				w.WriteStartElement("Workdays");
				using(IDataReader reader = Calendar.GetListWeekdayHours(calendarId))
				{
					while(reader.Read())
					{
						w.WriteStartElement("Workday");

						w.WriteAttributeString("DayOfWeek", reader["DayOfWeek"].ToString());
						w.WriteAttributeString("FromTime", reader["FromTime"].ToString());
						w.WriteAttributeString("ToTime", reader["ToTime"].ToString());
						
						w.WriteEndElement(); // Workday
					}
				}
				w.WriteEndElement(); // Workdays
				// Save exceptions
				w.WriteStartElement("Exceptions");
				using(IDataReader reader = Calendar.GetListExceptionHoursByCalendar(calendarId))
				{
					while(reader.Read())
					{
						w.WriteStartElement("Exception");

						w.WriteAttributeString("FromDate", reader["FromDate"].ToString());
						w.WriteAttributeString("ToDate", reader["ToDate"].ToString());
						w.WriteAttributeString("FromTime", reader["FromTime"].ToString());
						w.WriteAttributeString("ToTime", reader["ToTime"].ToString());
						
						w.WriteEndElement(); // Exception
					}
				}
				w.WriteEndElement(); // Exceptions
				w.WriteEndElement(); // Calendar


				w.WriteStartElement("Tasks");
				DataTable dt = DBTask.GetListTasksByProjectCollapsedDataTable(projectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId);
				for(int i=0; i<dt.Rows.Count; i++)
				{
					DataRow dr = dt.Rows[i];

					//if (dr.RowState == DataRowState.Deleted) // Skip subtasks of a collapsed task.
					//	continue;

					w.WriteStartElement("Task");

					int taskId = (int)dr["TaskId"];
					if(!tasks.Contains(taskId))
						tasks.Add(taskId);

					w.WriteElementString("TaskId", taskId.ToString());
					w.WriteElementString("TaskNum", ((int)dr["TaskNum"]).ToString());
					w.WriteElementString("Title", (string)dr["Title"]);
					w.WriteElementString("OutlineNumber", (string)dr["OutlineNumber"]);
					w.WriteElementString("OutlineLevel", ((int)dr["OutlineLevel"]).ToString());
					w.WriteElementString("Collapsed", (((int)dr["IsCollapsed"]) != 0).ToString());
					w.WriteElementString("Milestone", ((bool)dr["IsMilestone"]).ToString());
					w.WriteElementString("Summary", ((bool)dr["IsSummary"]).ToString());
					w.WriteElementString("Start", ((DateTime)dr["StartDate"]).ToString("s"));
					w.WriteElementString("Finish", ((DateTime)dr["FinishDate"]).ToString("s"));
					w.WriteElementString("Completed", ((bool)dr["IsCompleted"]).ToString());
					w.WriteElementString("Progress", ((int)dr["PercentCompleted"]).ToString());
					w.WriteElementString("ProgressLocked", (((int)dr["CompletionTypeId"]) != (int)CompletionType.Any).ToString());

					w.WriteEndElement(); // Task
				}
				w.WriteEndElement(); // Tasks

				w.WriteStartElement("Links");
				using(IDataReader reader = Project.GetListTaskLinksByProject(projectId))
				{
					while(reader.Read())
					{
						int predId = (int)reader["PredId"];
						int succId = (int)reader["SuccId"];
						if(tasks.Contains(predId) && tasks.Contains(succId))
						{
							w.WriteStartElement("Link");

							w.WriteElementString("PredId", predId.ToString());
							w.WriteElementString("SuccId", succId.ToString());

							w.WriteEndElement(); // Link
						}
					}
				}
				w.WriteEndElement(); // Links
			}
			w.WriteEndElement(); // Gantt
			return sb.ToString();
		}
		#endregion
	}
}
