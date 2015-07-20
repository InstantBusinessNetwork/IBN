using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Sync.Core.TransferDataType
{
	[Serializable]
	public class RecurrencePatternTransferData : SyncTransferData
	{
		public const string DataName = "RecurrencePattern";

		public const string FieldDayOfMonth = "DayOfMonth";
		public const string FieldDayOfWeekMask = "DayOfWeekMask";
		public const string FieldEvent = "Event";
		public const string FieldInstance = "Instance";
		public const string FieldInterval = "Interval";
		public const string FieldMonthOfYear = "MonthOfYear";
		public const string FieldNoEndDate = "NoEndDate";
		public const string FieldOccurrences = "Occurrences";
		public const string FieldPatternEndDate = "PatternEndDate";
		public const string FieldRecurrenceType = "RecurrenceType";
		public const string FieldTitle = "Title";

		public RecurrencePatternTransferData()
			: base(DataName)
		{
			InitializeProperties();
		}

		protected void InitializeProperties()
		{
			base.Properties.Add("DayOfMonth", null);
			base.Properties.Add("DayOfWeekMask", null);
			base.Properties.Add("Event", null);
			base.Properties.Add("Instance", null);
			base.Properties.Add("Interval", null);
			base.Properties.Add("MonthOfYear", null);
			base.Properties.Add("NoEndDate", null);
			base.Properties.Add("Occurrences", null);
			base.Properties.Add("PatternEndDate", null);
			base.Properties.Add("RecurrenceType", null);
			base.Properties.Add("Title", null);

		}

	
		#region Named Properties

		public Nullable<System.Int32> DayOfMonth
		{
			get
			{
				return (Nullable<System.Int32>)base.Properties["DayOfMonth"];
			}

			set
			{
				base.Properties["DayOfMonth"] = value;
			}

		}

		public Nullable<System.Int32> DayOfWeekMask
		{
			get
			{
				return (Nullable<System.Int32>)base.Properties["DayOfWeekMask"];
			}

			set
			{
				base.Properties["DayOfWeekMask"] = value;
			}

		}

		public System.String Event
		{
			get
			{
				return (System.String)base.Properties["Event"];
			}

		}

		public Nullable<System.Int32> Instance
		{
			get
			{
				return (Nullable<System.Int32>)base.Properties["Instance"];
			}

			set
			{
				base.Properties["Instance"] = value;
			}

		}

		public Nullable<System.Int32> Interval
		{
			get
			{
				return (Nullable<System.Int32>)base.Properties["Interval"];
			}

			set
			{
				base.Properties["Interval"] = value;
			}

		}

		public Nullable<System.Int32> MonthOfYear
		{
			get
			{
				return (Nullable<System.Int32>)base.Properties["MonthOfYear"];
			}

			set
			{
				base.Properties["MonthOfYear"] = value;
			}

		}

		public Nullable<System.Boolean> NoEndDate
		{
			get
			{
				return (Nullable<System.Boolean>)base.Properties["NoEndDate"];
			}

			set
			{
				base.Properties["NoEndDate"] = value;
			}

		}

		public Nullable<System.Int32> Occurrences
		{
			get
			{
				return (Nullable<System.Int32>)base.Properties["Occurrences"];
			}

			set
			{
				base.Properties["Occurrences"] = value;
			}

		}

		public Nullable<System.DateTime> PatternEndDate
		{
			get
			{
				return (Nullable<System.DateTime>)base.Properties["PatternEndDate"];
			}

			set
			{
				base.Properties["PatternEndDate"] = value;
			}

		}

		public System.Int32 RecurrenceType
		{
			get
			{
				return (System.Int32)base.Properties["RecurrenceType"];
			}

			set
			{
				base.Properties["RecurrenceType"] = value;
			}

		}

		public System.String Title
		{
			get
			{
				return (System.String)base.Properties["Title"];
			}

			set
			{
				base.Properties["Title"] = value;
			}

		}

		#endregion
	}
}
