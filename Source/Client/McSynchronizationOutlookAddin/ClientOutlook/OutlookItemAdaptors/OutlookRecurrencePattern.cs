using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutlookAddin.OutlookItemAdaptors
{
	public class OutlookRecurrencePattern : OutlookItem
	{
		private Outlook.RecurrencePattern _oRecurrPattern;

		public OutlookRecurrencePattern(OutlookListener outlookListener, Outlook.RecurrencePattern oRecurrPattern)
			:base(outlookListener)
		{
			_oRecurrPattern = oRecurrPattern;
		}

		#region Properties
		public Outlook.OlRecurrenceType RecurrenceType
		{
			get
			{
				return base.GetProp<Outlook.OlRecurrenceType>(_oRecurrPattern, "RecurrenceType");
			}
			set
			{
				base.SetProp(_oRecurrPattern, "RecurrenceType", value);
			}
		}

		public Outlook.OlDaysOfWeek DayOfWeekMask
		{
			get
			{
				return base.GetProp<Outlook.OlDaysOfWeek>(_oRecurrPattern, "DayOfWeekMask");
			}
			set
			{
				base.SetProp(_oRecurrPattern, "DayOfWeekMask", value);
			}
		}

		public int MonthOfYear
		{
			get
			{
				return base.GetProp<int>(_oRecurrPattern, "MonthOfYear");
			}
			set
			{
				base.SetProp(_oRecurrPattern, "MonthOfYear", value);
			}
		}

		public int DayOfMonth
		{
			get
			{
				return base.GetProp<int>(_oRecurrPattern, "DayOfMonth");
			}
			set
			{
				base.SetProp(_oRecurrPattern, "DayOfMonth", value);
			}
		}
		public int Instance
		{
			get
			{
				return base.GetProp<int>(_oRecurrPattern, "Instance");
			}
			set
			{
				base.SetProp(_oRecurrPattern, "Instance", value);
			}
		}
		public int Interval
		{
			get
			{
				return base.GetProp<int>(_oRecurrPattern, "Interval");
			}
			set
			{
				base.SetProp(_oRecurrPattern, "Interval", value);
			}
		}
		public int Occurrences
		{
			get
			{
				return base.GetProp<int>(_oRecurrPattern, "Occurrences");
			}
			set
			{
				base.SetProp(_oRecurrPattern, "Occurrences", value);
			}
		}
		public DateTime PatternEndDate
		{
			get
			{
				return base.GetProp<DateTime>(_oRecurrPattern, "PatternEndDate");
			}
			set
			{
				base.SetProp(_oRecurrPattern, "PatternEndDate", value);
			}
		}

		public IEnumerable<OutlookException> Exceptions
		{
			get
			{
				return base._outlookListener.GetRecurrenceExceptions(_oRecurrPattern);
			}
		}

		#endregion

		#region Methods
		public OutlookAppointment GetOccurrence(DateTime recurrenceId)
		{
			return base._outlookListener.GetRecurrenceOccurence(_oRecurrPattern, recurrenceId);
		}
		#endregion
		#region Overrides
		public override object InnerOutlookObject
		{
			get { return _oRecurrPattern; }
		}
		#endregion
	}
}
