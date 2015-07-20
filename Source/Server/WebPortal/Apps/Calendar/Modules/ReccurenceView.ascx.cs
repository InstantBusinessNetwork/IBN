using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Resources;
using System.Reflection;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Events;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Web.UI.Calendar.Modules
{
	public partial class ReccurenceView : MCDataBoundControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strRecEditor", Assembly.GetExecutingAssembly());

		#region EventId
		private PrimaryKeyId EventId
		{
			get
			{
				try
				{
					return PrimaryKeyId.Parse(Request["ObjectId"]);
				}
				catch
				{
					throw new Exception("Invalid ObjectId!");
				}
			}
		}
		#endregion

		#region Weekdays
		public byte Pattern;
		public byte Frequency;
		private byte bWeekdays;
		public int EndAfter;
		public string Weekdays
		{
			get
			{
				string str = "";
				foreach (eBitDayOfWeek day in Enum.GetValues(typeof(eBitDayOfWeek)))
					if (((bWeekdays & (byte)day) == (byte)day) && (day != eBitDayOfWeek.Unknown) && (day != eBitDayOfWeek.Alldays) && (day != eBitDayOfWeek.Weekdays))
					{
						str += System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(GetDayOfWeekByBit(day)).ToLower();
						str += ", ";
					}
				if (str.Length > 2)
					str = str.Substring(0, str.Length - 2);
				return str;
			}
		}
		#endregion

		#region WeekNumber
		public byte MonthDay;
		private byte bWeekNumber;
		public string WeekNumber
		{
			get
			{
				switch (bWeekNumber)
				{
					case 1:
						return LocRM.GetString("First");
					case 2:
						return LocRM.GetString("Second");
					case 3:
						return LocRM.GetString("Third");
					case 4:
						return LocRM.GetString("Fourth");
					case 5:
						return LocRM.GetString("Last");
					default:
						return "";
				}
			}
		}
		#endregion

		#region MonthName
		private byte MonthNumber;
		public string MonthName
		{
			get
			{
				if (MonthNumber > 0)
					return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(MonthNumber);
				return "";
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
		}

		public override void DataBind()
		{
			BindValues();
			BindToolBar();
			base.DataBind();
		}

		#region BindValues
		private void BindValues()
		{
			CalendarEventEntity ceo = (CalendarEventEntity)DataItem;//BusinessManager.Load(CalendarEventEntity.ClassName, EventId) as CalendarEventEntity;
			if (ceo != null)
			{
				FilterElementCollection fec = new FilterElementCollection();
				FilterElement fe = FilterElement.EqualElement("EventId", EventId);
				fec.Add(fe);
				EntityObject[] list = BusinessManager.List(CalendarEventRecurrenceEntity.ClassName, fec.ToArray());
				if (list.Length > 0)
				{
					CalendarEventRecurrenceEntity cero = (CalendarEventRecurrenceEntity)list[0];

					lblNoRecurrence.Visible = false;
					ShowRecurrence.Visible = true;
					txtStartTime.Text = ceo.Start.ToShortTimeString();
					txtEndTime.Text = ceo.End.ToShortTimeString();
					Pattern = (byte)cero.RecurrenceType;
					if (cero.RecurrenceType == (int)eRecurrenceType.RecursWeekly &&
							cero.DayOfWeekMask.HasValue &&
							cero.DayOfWeekMask.Value == (int)eBitDayOfWeek.Weekdays)
						Pattern = (byte)eRecurrenceType.RecursDaily;

					if (cero.Interval.HasValue)
						Frequency = (byte)cero.Interval.Value;
					if (cero.DayOfWeekMask.HasValue)
						bWeekdays = (byte)cero.DayOfWeekMask;
					if (cero.DayOfMonth.HasValue)
						MonthDay = (byte)cero.DayOfMonth.Value;
					if (cero.Instance.HasValue)
						bWeekNumber = (byte)cero.Instance.Value;
					if (cero.MonthOfYear.HasValue)
						MonthNumber = (byte)cero.MonthOfYear.Value;

					if (cero.Occurrences.HasValue)
						EndAfter = cero.Occurrences.Value;

					int pattern = Pattern;
					if (Pattern == 1)
						pattern = (cero.DayOfWeekMask.HasValue &&
							 cero.DayOfWeekMask.Value == (int)eBitDayOfWeek.Weekdays) ? 12 : 11;

					Label lblControl = (Label)this.FindControl("txtInfo" + pattern);
					if (lblControl != null)
						lblControl.Visible = true;
					if (EndAfter != 0)
					{
						lblEnd.Text = LocRM.GetString("Endafter") + ":";
						txtEnd.Text = EndAfter.ToString() + " " + LocRM.GetString("occurrences");
					}
					else if (cero.PatternEndDate.HasValue)
					{
						lblEnd.Text = LocRM.GetString("Endby") + ":";
						txtEnd.Text = cero.PatternEndDate.Value.ToShortDateString();
					}
				}
				else
				{
					lblNoRecurrence.Text = "<br>&nbsp; " + LocRM.GetString("NotSet") + "<br>&nbsp;";
					lblNoRecurrence.Visible = true;
					ShowRecurrence.Visible = false;
				}
			}
		}
		#endregion

		#region BindToolBar
		private void BindToolBar()
		{
			secHeader.Title = LocRM.GetString("Recurrence");

			CommandParameters cp = new CommandParameters("CEvent_NewReccurence");
			string cmd = CommandManager.GetCurrent(this.Page).AddCommand("CalendarEvent", "CalendarEvent", "CalendarEventView", cp);
			cmd = cmd.Replace("\"", "&quot;");

			secHeader.AddRightLink(String.Format("<img alt='' src='{0}'/> {1}",
									ResolveClientUrl("~/Layouts/Images/icons/recurrence.gif"), LocRM.GetString("Modify")),
				String.Format("javascript:{{{0}}}", cmd));

			if (ShowRecurrence.Visible)
			{
				secHeader.AddRightLink(String.Format("<img alt='' src='{0}'/> {1}",
											ResolveClientUrl("~/Layouts/Images/delete.gif"), LocRM.GetString("Delete")),
					"javascript: document.getElementById('" + btnDelete.ClientID + "').click()");
				btnDelete.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "');");
			}
		}
		#endregion

		#region btnDelete_Click
		protected void btnDelete_Click(object sender, System.EventArgs e)
		{
			CalendarEventEntity ceo = BusinessManager.Load(CalendarEventEntity.ClassName, EventId) as CalendarEventEntity;
			if (ceo != null)
			{
				FilterElementCollection fec = new FilterElementCollection();
				PrimaryKeyId realKey = ((VirtualEventId)EventId).RealEventId;
				FilterElement fe = FilterElement.EqualElement("EventId", realKey);
				fec.Add(fe);
				EntityObject[] list = BusinessManager.List(CalendarEventRecurrenceEntity.ClassName, fec.ToArray());
				if (list.Length > 0)
				{
					CalendarEventRecurrenceEntity cero = (CalendarEventRecurrenceEntity)list[0];
					BusinessManager.Delete(cero);

					Response.Redirect("~/Apps/MetaUIEntity/Pages/EntityView.aspx?ClassName=CalendarEvent&ObjectId=" + realKey.ToString());
				}
			}
		}
		#endregion

		public override bool CheckVisibility(object dataItem)
		{
			if (dataItem != null)
			{
				CalendarEventEntity ceo = (CalendarEventEntity)dataItem;
				if (ceo.CalendarEventExceptionId.HasValue)
					return false;
			}
			return base.CheckVisibility(dataItem);
		}


		#region GetDayOfWeekByBit
		public static DayOfWeek GetDayOfWeekByBit(eBitDayOfWeek bitDayOfWeek)
		{
			switch (bitDayOfWeek)
			{
				case eBitDayOfWeek.Monday:
					return DayOfWeek.Monday;
				case eBitDayOfWeek.Tuesday:
					return DayOfWeek.Tuesday;
				case eBitDayOfWeek.Wednesday:
					return DayOfWeek.Wednesday;
				case eBitDayOfWeek.Thursday:
					return DayOfWeek.Thursday;
				case eBitDayOfWeek.Friday:
					return DayOfWeek.Friday;
				case eBitDayOfWeek.Saturday:
					return DayOfWeek.Saturday;
				case eBitDayOfWeek.Sunday:
					return DayOfWeek.Sunday;
			}
			throw new Exception("Illegal day of week.");
		}
		#endregion
	}
}