//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------

namespace Mediachase.Web.UI.WebControls
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Collections;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Text.RegularExpressions;
	using Mediachase.Web.UI.WebControls.Util;

    /// <summary>
    ///  Utility class with various useful static functions.
    /// </summary>
    public class Helper
    {
        private static System.Resources.ResourceManager _ResourceManager = null;

        public static System.Resources.ResourceManager GetResourceManager()
        {
            if (_ResourceManager == null)
            {
                Type ourType = typeof(Helper);
                _ResourceManager = new System.Resources.ResourceManager(ourType.Namespace, ourType.Module.Assembly);
            }

            return _ResourceManager;
        }

        public static string GetStringResource(string name)
        {
			string retval = string.Empty;
			try
			{
				retval = (string)GetResourceManager().GetObject(name);
			}
			catch
			{
			}

			return retval;
        }

        /// <summary>
        ///  Converts a FontUnit to a size for the HTML FONT tag
        /// </summary>
        public static string ConvertToHtmlFontSize(FontUnit fu)
        {
            if ((int)(fu.Type) > 3)
            {
                return ((int)(fu.Type)-3).ToString();
            }

            if (fu.Type == FontSize.AsUnit) {
                if (fu.Unit.Type == UnitType.Point) {
                    if (fu.Unit.Value <= 8)
                        return "1";
                    else if (fu.Unit.Value <= 10)
                        return "2";
                    else if (fu.Unit.Value <= 12)
                        return "3";
                    else if (fu.Unit.Value <= 14)
                        return "4";
                    else if (fu.Unit.Value <= 18)
                        return "5";
                    else if (fu.Unit.Value <= 24)
                        return "6";
                    else
                        return "7";
                }
            }

            return null;
        }

        /// <summary>
        ///  Searches for an object's parents for a Form object
        /// </summary>
        public static Control FindForm(Control child)
        {
            Control parent = child;

            while (parent != null)
            {
                if (parent is HtmlForm)
                    break;

                parent = parent.Parent;
            }

            return parent;
        }

        /// <summary>
        /// Given a string that contains a number, extracts the substring containing the number.
        /// Returns only the first number.
        /// Example: "5px" returns "5"
        /// </summary>
        /// <param name="str">The string containing the number.</param>
        /// <returns>The extracted number or String.Empty.</returns>
        public static string ExtractNumberString(string str)
        {
            string[] strings = ExtractNumberStrings(str);
            if (strings.Length > 0)
            {
                return strings[0];
            }

            return String.Empty;
        }

        /// <summary>
        /// Extracts all numbers from the string.
        /// </summary>
        /// <param name="str">The string containing numbers.</param>
        /// <returns>An array of the numbers as strings.</returns>
        public static string[] ExtractNumberStrings(string str)
        {
            if (str == null)
            {
                return new string[0];
            }

            // Match the digits
            MatchCollection matches = Regex.Matches(str, "(\\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // Move to a string array
            string[] strings = new string[matches.Count];
            int index = 0;

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    strings[index] = match.Value;
                }
                else
                {
                    strings[index] = String.Empty;
                }

                index++;
            }

            return strings;
        }

        /// <summary>
        ///  Converts a color string to a hex value string ("Green" -> "#000800")
        /// </summary>
        public static string ColorToHexString(string color)
        {
            if (color[0] == '#')
            {
                return color;
            }

            Color c = ColorTranslator.FromHtml(color);
            return ColorToHexString(c);
        }

        /// <summary>
        ///  Converts a Color to a hex value string (Color.Green -> "#000800")
        /// </summary>
        public static string ColorToHexString(Color c)
        {
            string r = Convert.ToString(c.R, 16);
            if (r.Length < 2)
                r = "0" + r;
            string g = Convert.ToString(c.G, 16);
            if (g.Length < 2)
                g = "0" + g;
            string b = Convert.ToString(c.B, 16);
            if (b.Length < 2)
                b = "0" + b;

            string str = "#" + r + g + b;
            return str;
        }

		/*
		/// <summary>
		/// Returns localized week number
		/// </summary>
		/// <param name="dayOfWeek"></param>
		/// <returns></returns>
		internal static int LocalizedDayOfWeek(System.DayOfWeek dayOfWeek)
		{
			// Old way with auto detect based on localization
			 int day = dayOfWeek.GetHashCode() - System.Globalization.DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek.GetHashCode();
			// New settings based on setting selected for control
			// int day = dayOfWeek.GetHashCode() - System.Globalization.DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek.GetHashCode();
			
			if(day<0)
				day+=7;

			return day;
		}
		*/

		/// <summary>
		/// Returns localized week number
		/// </summary>
		/// <param name="dayOfWeek"></param>
		/// <returns></returns>
		public static int LocalizedDayOfWeek(System.DayOfWeek dayOfWeek, CalendarDayOfWeek firstDayOfWeek)
		{
			// New settings based on setting selected for control
			int day = dayOfWeek.GetHashCode() - GetDayOfWeek(firstDayOfWeek).GetHashCode();
			
			if(day<0)
				day+=7;

			return day;
		}

		public static bool IsWeekend(DateTime now)
		{
			DayOfWeek dayOfWeek = System.Globalization.DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek;
			if(dayOfWeek == DayOfWeek.Monday || dayOfWeek == DayOfWeek.Sunday) // West
			{
				if(now.Date.DayOfWeek == DayOfWeek.Sunday || now.Date.DayOfWeek == DayOfWeek.Saturday)
				{
					return true;
				}
			}
			else // Arabic
			{
				if(now.Date.DayOfWeek == DayOfWeek.Thursday || now.Date.DayOfWeek == DayOfWeek.Friday)
				{
					return true;
				}
			}

			return false;
		}


		/// <summary>
		/// Gets end date for task view
		/// </summary>
		/// <param name="start"></param>
		/// <returns></returns>
		public static DateTime GetTaskEndDate(DateTime start)
		{
			DateTime end = start.AddMonths(1).Date;
			//if(end.Date.Day < 7)
			//	end = end.Date.AddDays(8 - end.Date.Day);

			return end;
		}

		public static int GetDaySpan(DateTime start, DateTime end)
		{
			TimeSpan span = end.Date - start.Date;
			int days = (int)span.TotalDays;
			//if(start.Day < end.Day && days > 0)
			//	days++;
			return days;
		}
		
		/// <summary>
		/// Returns date formatted for current hour half, 
		/// can be 0, 30 and 59 minutes.
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime GetHourDate(DateTime date)
		{
			int minutes = 0;
			int seconds = 0;
			if(date.Minute>0)
			{
				minutes = 30;
				seconds = 0;
			}
			if(date.Minute>30)
			{
				minutes = 59;
				seconds = 59;
			}
			return new DateTime(date.Year, date.Month, date.Day, date.Hour, minutes, seconds);
		}

		
		/// <summary>
		/// Used by day view renderer
		/// </summary>
		/// <param name="date"></param>
		/// <returns>0 or 30 min decrement datetime</returns>
		public static DateTime GetHourStartDate(DateTime date)
		{
			int minutes = 0;
			int seconds = 0;

			if(date.Minute>=30)
				minutes = 30;

			return new DateTime(date.Year, date.Month, date.Day, date.Hour, minutes, seconds);
		}

		/*
		/// <summary>
		/// Returns date formatted for current hour half, 
		/// can be 0 and 30 minutes. Used for comparing with cycle date
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		internal static DateTime GetHourCycleDate(DateTime date)
		{
			int minutes = 0;
			int seconds = 0;
			if(date.Minute>30)
			{
				minutes = 30;
			}
			return new DateTime(date.Year, date.Month, date.Day, date.Hour, minutes, seconds);
		}
		*/

		public static int GetHourSpan(DateTime now, CalendarItem item)
		{
			TimeSpan spanTime = Helper.GetHourDate(item.EndDate) - Helper.GetHourStartDate(item.StartDate);
			int colSpan = (int)System.Math.Ceiling(spanTime.TotalMinutes/60*2);

			// detect if events spans from previous week
			if(item.StartDate.Date<now.Date)
			{
				// Calculate span from current date
				TimeSpan spanTime2 = Helper.GetHourDate(item.EndDate) - now;
				colSpan = (int)Math.Ceiling(spanTime2.TotalHours*2);
			}

			// detect if event spans to next week
			if(colSpan>(47 - now.Hour.GetHashCode()*2))
			{
				colSpan = (47 - now.Hour*2)+1;
			}

			return colSpan;
		}

		/*
		/// <summary>
		/// Will determine if current event is all day event. 
		/// </summary>
		/// <param name="item"></param>
		/// <param name="now"></param>
		/// <returns></returns>
		internal static bool IsAllDayEvent(CalendarItem item, DateTime now)
		{
			//if((now.Date > item.StartDate || (now.Date >= item.StartDate && item.StartDate.Hour == 0)) && now.Date <= item.EndDate)
			if(now.Date > item.StartDate.Date && now.Date < item.EndDate.Date)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Determines if event is withing current span
		/// </summary>
		/// <param name="span"></param>
		/// <param name="item"></param>
		/// <param name="now"></param>
		/// <returns></returns>
		internal static bool IsWithinSpanEvent(MatrixSpan span, CalendarItem item, DateTime now)
		{
			switch(span)
			{
				case MatrixSpan.WeekHourSpan:
					if(!IsAllDayEvent(item, now) && (item.StartDate.Date == item.EndDate.Date && GetHourStartDate(item.StartDate) <= now && GetHourDate(item.EndDate) >= now))
						return true;
					break;
				case MatrixSpan.HourSpan:
					// Don't add up all day events
					if(!IsAllDayEvent(item, now) && (GetHourStartDate(item.StartDate) <= now && GetHourDate(item.EndDate) >= now))
						return true;
					break;
				case MatrixSpan.WeekDaySpan:
					if(item.EndDate.Date >= now.Date && item.StartDate.Date <= now.Date && item.StartDate.Date != item.EndDate.Date)
						return true;
					break;
				case MatrixSpan.DaySpan:
					if(item.EndDate.Date >= now.Date && item.StartDate.Date <= now.Date)
						return true;
					break;
				case MatrixSpan.MonthSpan:
					if(item.EndDate >= now && item.StartDate <= now)
						return true;
					break;
			}
			return false;
		}

		/// <summary>
		/// Determines if current event belongs to the span
		/// </summary>
		/// <param name="index"></param>
		/// <param name="span"></param>
		/// <param name="item"></param>
		/// <param name="now"></param>
		/// <returns></returns>
		internal static bool IsCurrentSpanEvent(int index, MatrixSpan span, CalendarItem item, DateTime now)
		{
			switch(span)
			{
				case MatrixSpan.WeekHourSpan:
					if(!IsAllDayEvent(item, now) && item.EndDate.Date == item.StartDate.Date && ((index==0 && item.EndDate >= now && item.StartDate <= now) || GetHourCycleDate(item.StartDate) == now))
						return true;
					break;
				case MatrixSpan.HourSpan:
					// Don't render items spanning from previous days, we use all day section for that
					//if(!IsAllDayEvent(item, now) && (GetHourCycleDate(item.StartDate) == now || (item.StartDate now == item.EndDate.Date))
					//if(!IsAllDayEvent(item, now) && (GetHourStartDate(item.StartDate) <= now && GetHourDate(item.EndDate) >= now))
					if(!IsAllDayEvent(item, now) && (index==0 && item.EndDate >= now && item.StartDate <= now) || GetHourCycleDate(item.StartDate) == now)
						return true;
					break;
				case MatrixSpan.WeekDaySpan: // only events spanning multiple days
					if(((item.StartDate.Date == now) || (index==0 && item.EndDate.Date >= now && item.StartDate.Date <= now)) && item.StartDate.Date != item.EndDate.Date)
						return true;
					break;
				case MatrixSpan.DaySpan:
					if((item.StartDate.Date == now) || (index==0 && item.EndDate.Date >= now && item.StartDate.Date <= now))
						return true;
					break;
				case MatrixSpan.MonthSpan:
					if((item.StartDate == now) || (index==0 && item.EndDate >= now && item.StartDate <= now))
						return true;
					break;
			}

			return false;
		}
		*/


		/// <summary>
		/// Increments Date
		/// </summary>
		/// <param name="span"></param>
		/// <param name="now"></param>
		/// <returns></returns>
		public static DateTime IncrementDate(MatrixSpan span, DateTime now)
		{
			switch(span)
			{
				case MatrixSpan.WeekHourSpan:
				case MatrixSpan.HourSpan:
					return now.AddMinutes(30);
					//return now.AddHours(1);
				case MatrixSpan.WeekDaySpan:
				case MatrixSpan.DaySpan:
					return now.AddDays(1);
				case MatrixSpan.MonthSpan:
					return now.AddMonths(1);
			}
			return now;
		}

		/// <summary>
		/// Increments Date
		/// </summary>
		/// <param name="span"></param>
		/// <param name="now"></param>
		/// <returns></returns>
		public static DateTime IncrementDate(TimescaleTopSpan span, DateTime now, CalendarDayOfWeek firstDayOfWeek)
		{
			switch(span)
			{
				case TimescaleTopSpan.Week:
					return now.AddDays(7).AddDays(-(LocalizedDayOfWeek(now.AddDays(7).DayOfWeek, firstDayOfWeek)));
				case TimescaleTopSpan.Month:
					return now.AddMonths(1).AddDays(-now.AddMonths(1).Day+1);
			}
			return now;
		}

		public static DateTime GetMonthStartDate(DateTime now, DateTime MonthStartDate, CalendarDayOfWeek firstDayOfWeek)
		{
			DateTime monthStart = DateTime.MinValue;
			if(MonthStartDate>DateTime.MinValue)
				monthStart = MonthStartDate;
			else
				monthStart = new DateTime(now.Year, now.Month, 1);
			return monthStart.AddDays(-LocalizedDayOfWeek(monthStart.DayOfWeek, firstDayOfWeek));
		}

		/// <summary>
		/// Add link attributes to td tag
		/// </summary>
		/// <param name="writer"></param>
		public static void AddLinkAttributes(HtmlTextWriter writer, string link, DateTime cycleDate)
		{
			// Render linked hour if format is specified
			if(link.Length > 0)
			{
				string href = "javascript:" + String.Format(link, cycleDate);
				writer.AddAttribute(HtmlTextWriterAttribute.Onclick, href);
				writer.AddStyleAttribute("cursor", "hand");
			}
		}

		/// <summary>
		/// Add context link attributes to td tag
		/// </summary>
		/// <param name="writer"></param>
		public static void AddOnContextMenuAttribute(HtmlTextWriter writer, string link, DateTime cycleDate)
		{
			// Render linked hour if format is specified
			if(link.Length > 0)
			{
				string href = "return " + String.Format(link, cycleDate);
				writer.AddAttribute("oncontextmenu", href);
			}
		}


		/// <summary>
		/// Add link attributes to td tag
		/// </summary>
		/// <param name="writer"></param>
		public static void AddMultiOwnerLinkAttributes(HtmlTextWriter writer, string link, string link2, DateTime cycleDate, string owner)
		{
			// Render linked hour if format is specified
			if(link.Length > 0)
			{
				string href = "javascript:" + String.Format(link, cycleDate, owner);
				writer.AddAttribute(HtmlTextWriterAttribute.Onclick, href);
				writer.AddStyleAttribute("cursor", "hand");
			}
			else if(link2.Length > 0)
			{
				string href = "javascript:" + String.Format(link, cycleDate, owner);
				writer.AddAttribute(HtmlTextWriterAttribute.Onclick, href);
				writer.AddStyleAttribute("cursor", "hand");
			}
		}

		/// <summary>
		/// Add owner attributes to td tag
		/// </summary>
		/// <param name="writer"></param>
		public static void AddOwnerAttributes(HtmlTextWriter writer, string link, DateTime cycleDate, string owner)
		{
			// Render linked owner if format is specified
			if(link.Length > 0)
			{
				string href = "javascript:" + String.Format(link, cycleDate, owner);
				writer.AddAttribute(HtmlTextWriterAttribute.Onclick, href);
				writer.AddStyleAttribute("cursor", "hand");
			}
		}
		/// <summary>
		/// Add context link ownewr attributes to td tag
		/// </summary>
		/// <param name="writer"></param>
		public static void AddOnContextMenuOwnerAttribute(HtmlTextWriter writer, string link, DateTime cycleDate, string owner)
		{
			// Render linked hour if format is specified
			if(link.Length > 0)
			{
				string href = "return " + String.Format(link, cycleDate, owner);
				writer.AddAttribute("oncontextmenu", href);
			}
		}

		/// <summary>
		/// Return true if date is weekend
		/// </summary>
		/// <param name="now"></param>
		/// <param name="workWeek">work week</param>
		/// <returns></returns>
		public static bool IsWeekend(DateTime now, CalendarDayOfWeek workWeek)
		{
			if((Helper.GetCalDayOfWeek(now.DayOfWeek) & workWeek)==0)
				return true;
			else
				return false;
		}

		public static CalendarDayOfWeek GetCalDayOfWeek(DayOfWeek weekDay)
		{
			switch(weekDay)
			{
				case DayOfWeek.Sunday:
					return CalendarDayOfWeek.Sunday;
				case DayOfWeek.Monday:
					return CalendarDayOfWeek.Monday;
				case DayOfWeek.Tuesday:
					return CalendarDayOfWeek.Tuesday;
				case DayOfWeek.Wednesday:
					return CalendarDayOfWeek.Wednesday;
				case DayOfWeek.Thursday:
					return CalendarDayOfWeek.Thursday;
				case DayOfWeek.Friday:
					return CalendarDayOfWeek.Friday;
				case DayOfWeek.Saturday:
					return CalendarDayOfWeek.Saturday;
				default:
					return CalendarDayOfWeek.Sunday;
			}
		}

		public static DayOfWeek GetDayOfWeek(CalendarDayOfWeek calDayOfWeek)
		{
			switch(calDayOfWeek)
			{
				case CalendarDayOfWeek.Sunday:
					return DayOfWeek.Sunday;
				case CalendarDayOfWeek.Monday:
					return DayOfWeek.Monday;
				case CalendarDayOfWeek.Tuesday:
					return DayOfWeek.Tuesday;
				case CalendarDayOfWeek.Wednesday:
					return DayOfWeek.Wednesday;
				case CalendarDayOfWeek.Thursday:
					return DayOfWeek.Thursday;
				case CalendarDayOfWeek.Friday:
					return DayOfWeek.Friday;
				case CalendarDayOfWeek.Saturday:
					return DayOfWeek.Saturday;
				default:
					return DayOfWeek.Sunday;
			}
		}
   }
}