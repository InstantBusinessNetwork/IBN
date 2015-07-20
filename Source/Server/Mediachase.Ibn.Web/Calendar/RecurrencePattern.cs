using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Reflection;
using Mediachase.Web.UI.WebControls.Util;

namespace Mediachase.Web.UI.WebControls
{
	/// <summary>
	/// Summary description for rencePattern.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class RecurrencePattern : Component
	{
		private PatternRecurrence _Pattern = PatternRecurrence.Daily;
		private DateTime _StartTime = DateTime.Now;
		private DateTime _EndTime = DateTime.Now;
		private RecurrenceEndType _EndType;
		private RecurrencePatternType _PatternType;
		private int _Frequency;
		private int _Count;
		private BitDayOfWeek _WeekDays;
		private int _DayOfMonth;
		private int _Month;
		private WeekNumber _WeekNumber;
		private DayOfWeek _DayOfWeek;

		/// <summary>
		/// Pattern
		/// </summary>
		[
		Category("Pattern"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemPattern"),
		]
		public PatternRecurrence Pattern
		{
			get 
			{ 
				return _Pattern;
			}
			set { _Pattern = value; }
		}

		/// <summary>
		/// Pattern
		/// </summary>
		[
		Category("Pattern"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemPatternStartTime"),
		]
		public DateTime PatternStartTime
		{
			get 
			{ 
				return _StartTime;
			}
			set { _StartTime = value; }
		}
		
		/// <summary>
		/// Pattern
		/// </summary>
		[
		Category("Pattern"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemPatternEndTime"),
		]
		public DateTime PatternEndTime
		{
			get 
			{ 
				return _EndTime;
			}
			set { _EndTime = value; }
		}

		/// <summary>
		/// Pattern
		/// </summary>
		[
		Category("Pattern"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemPatternEndType"),
		]
		public RecurrenceEndType EndType
		{
			get { return _EndType; }
			set { _EndType = value; }
		}

		/// <summary>
		/// Pattern
		/// </summary>
		[
		Category("Pattern"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemPatternType"),
		]
		public RecurrencePatternType PatternType
		{
			get { return _PatternType; }
			set { _PatternType = value; }
		}

		/// <summary>
		/// Count
		/// </summary>
		[
		Category("Pattern"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemPatternCount"),
		]
		public int Count
		{
			get { return _Count; }
			set { _Count = value; }
		}

		/// <summary>
		/// Pattern
		/// </summary>
		[
		Category("Pattern"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemPatternFrequency"),
		]
		public int Frequency
		{
			get { return _Frequency; }
			set { _Frequency = value; }
		}
		
		/// <summary>
		/// Pattern
		/// </summary>
		[
		Category("Pattern"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemPatternWeekDays"),
		]
		public BitDayOfWeek WeekDays
		{
			get { return _WeekDays; }
			set { _WeekDays = value; }
		}
		
		/// <summary>
		/// Pattern
		/// </summary>
		[
		Category("Pattern"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemPatternDayOfMonth"),
		]
		public int DayOfMonth
		{
			get { return _DayOfMonth; }
			set { _DayOfMonth = value; }
		}
		
		/// <summary>
		/// Pattern
		/// </summary>
		[
		Category("Pattern"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemPatternMonth"),
		]
		public int Month
		{
			get { return _Month; }
			set { _Month = value; }
		}

		/// <summary>
		/// Pattern
		/// </summary>
		[
		Category("Pattern"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemPatternWeekNumber"),
		]
		public WeekNumber WeekNumber
		{
			get { return _WeekNumber; }
			set { _WeekNumber = value; }
		}

		/// <summary>
		/// Pattern
		/// </summary>
		[
		Category("Pattern"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemPatternDayOfWeek"),
		]
		public DayOfWeek DayOfWeek
		{
			get { return _DayOfWeek; }
			set { _DayOfWeek = value; }
		}

		public RecurrencePattern()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public override string ToString()
		{
			return "Calculated recurrance";
		}
	}

	public enum PatternRecurrence
	{
		Daily,
		Weekly,
		Monthly,
		Yearly
	};

	public enum RecurrencePatternType
	{
		Type1,
		Type2
	};

	public enum RecurrenceEndType
	{
		NoEnd,
		Counter,
		EndDate
	};

	[FlagsAttribute]
	public enum WeekNumber
	{
		None    = 0,
		First   = 1,
		Second  = 2,
		Third   = 3,
		Fourth  = 4,
		Fifth  =  5,
		Last    = 8
	};

	[FlagsAttribute]
	public enum BitDayOfWeek
	{
		None      = 0x00,
		Monday    = 0x01,
		Tuesday   = 0x02,
		Wednesday = 0x04,
		Thursday  = 0x08,
		Friday    = 0x10,
		Saturday  = 0x20,
		Sunday    = 0x40
	}

	public enum MonthOfYear
	{
		January,
		February,
		March,
		April,
		May,
		June,
		July,
		August,
		September,
		October,
		November,
		December
	}

	public enum Disposition
	{
		First,
		Last
	}
}
