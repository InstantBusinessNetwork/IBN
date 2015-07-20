using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using Mediachase.Ibn.Web.Drawing.Gantt;

namespace Mediachase.IBN.Business
{
	public class ResourceChart
	{
		public const int HeaderItemHeight = 13;
		public const int HeaderHeight = HeaderItemHeight * 2 + 3;
		public const int ItemHeight = 19;
		public const int PortionWidth = 504;

		#region public static string[] GetLegendItems()
		public static string[] GetLegendItems()
		{
			List<string> items = new List<string>();

			items.Add(Calendar.TimeType.CalendarEntry.ToString());
			items.Add(Calendar.TimeType.WorkPinnedUp.ToString());
			items.Add(Calendar.TimeType.WorkUrgent.ToString());
			items.Add(Calendar.TimeType.WorkVeryHigh.ToString());
			items.Add(Calendar.TimeType.WorkHigh.ToString());
			items.Add(Calendar.TimeType.WorkNormal.ToString());
			items.Add(Calendar.TimeType.WorkLow.ToString());
			items.Add(Calendar.TimeType.Off.ToString());
			items.Add(Calendar.TimeType.Past.ToString());
			items.Add("Highlight");

			return items.ToArray();
		}
		#endregion

		#region public static byte[] RenderLegendItem(string styleFilePath, string type)
		public static byte[] RenderLegendItem(string styleFilePath, string type)
		{
			byte[] ret = null;

			DateTime startDate = DateTime.Now.Date;
			GanttView gantt = CreateGanttView(startDate, false, DayOfWeek.Monday, 0, ItemHeight);

			Element spanElement = gantt.CreateSpanElement(null, null, null);

			DateTime intervalStart = startDate.AddHours(12);
			DateTime intervalFinish = intervalStart.AddDays(2);

			gantt.CreateIntervalElement(spanElement, intervalStart, intervalFinish, null, type, null);

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

		#region public static byte[] Render(DateTime startDate, bool vastScale, int[] users, ObjectTypes[] objectTypes, bool generateDataXml, string styleFilePath, int portionX, int portionY, int itemsPerPage, int pageNumber)
		public static byte[] Render(bool vastScale, 
			DateTime curDate, 
			DateTime startDate,  
			int[] users, 
			ObjectTypes[] objectTypes, 
			List<KeyValuePair<int, int>> highlightedItems,
			bool generateDataXml, 
			string styleFilePath, 
			int portionX, 
			int portionY, 
			int itemsPerPage, 
			int pageNumber)
		{
			startDate = startDate.AddDays((vastScale ? 7 : 21) * portionX);
			DateTime finishDate = startDate.AddDays(vastScale ? 7 : 21);
			portionX = 0;

			GanttView gantt = CreateGanttView(startDate, vastScale, ConvertDayOfWeek(PortalConfig.PortalFirstDayOfWeek), HeaderItemHeight, ItemHeight);

			if (portionY >= 0)
			{
				#region Add data
				foreach (int userId in users)
				{
					Element spanElement = gantt.CreateSpanElement(null, null, null);

					DataTable table = Calendar.GetResourceUtilization(userId, curDate, startDate, finishDate, new ArrayList(objectTypes), highlightedItems, true, true, true, false);
					table.AcceptChanges();

					for (int i = 0; i < table.Rows.Count; i++)
					{
						DataRow row = table.Rows[i];

						if (row.RowState != DataRowState.Deleted)
						{
							DateTime intervalStart = (DateTime)row["Start"];
							DateTime intervalFinish = ((DateTime)row["Finish"]);
							Calendar.TimeType timeType = (Calendar.TimeType)row["Type"];
							bool highlight = (bool)row["Highlight"];

							gantt.CreateIntervalElement(spanElement, intervalStart, intervalFinish, null, timeType.ToString(), null);
							if (highlight)
								gantt.CreateIntervalElement(spanElement, intervalStart, intervalFinish, null, "Highlight", null);
						}
					}
				}

				#endregion
			}

			return GanttManager.Render(gantt, generateDataXml, styleFilePath, portionX, portionY, PortionWidth, users.Length * ItemHeight, itemsPerPage, pageNumber);
		}
		#endregion


		#region private static GanttView CreateGanttView(DateTime startDate, bool vastScale, DayOfWeek firstDayOfWeek, float headerItemHeight, float itemHeight)
		private static GanttView CreateGanttView(DateTime startDate, bool vastScale, DayOfWeek firstDayOfWeek, float headerItemHeight, float itemHeight)
		{
			GanttView gantt = new GanttView(headerItemHeight, itemHeight);

			gantt.CreateDataElement(startDate);

			AxisElement topAxis = gantt.CreateAxisElement(ScaleLevel.Week, 1, string.Empty, firstDayOfWeek);
			AxisElement bottomAxis = gantt.CreateAxisElement(ScaleLevel.Day, 1, string.Empty, null);

			if (vastScale)
			{
				topAxis.TitleType = TitleType.StartEnd;
				topAxis.Format = "d MMMM yyyy";

				bottomAxis.TitleType = TitleType.Start;
				bottomAxis.Format = "ddd, d MMM";
				bottomAxis.Width = 72;
			}
			else
			{
				topAxis.TitleType = TitleType.Start;
				topAxis.Format = "d MMMM yyyy";

				bottomAxis.TitleType = TitleType.Start;
				bottomAxis.Format = "dd";
				bottomAxis.Width = 24;
			}

			return gantt;
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
