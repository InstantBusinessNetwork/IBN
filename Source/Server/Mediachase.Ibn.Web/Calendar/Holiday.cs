using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Mediachase.Web.UI.WebControls
{
	public enum HolidayType
	{
		DayOfWeek	=	0,
		Date =	1
	}
	/// <summary>
	/// Summary description for Holiday.
	/// </summary>
	[ParseChildren(false)]
	[ToolboxItem(false)]	
	public class Holiday : Control, INamingContainer
	{
		private HolidayType	_type;
		private DayOfWeek _dayOfWeek;
		private DateTime _date;
		private string _name;

		public Holiday(DayOfWeek dayOfWeek, string name)
		{
			_type = HolidayType.DayOfWeek;
			_dayOfWeek = dayOfWeek;
			_name = name;
		}

		public Holiday(DateTime	date, string name)
		{
			_type = HolidayType.Date;
			_date = date;
			_name = name;
		}

		public HolidayType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public DateTime	HolidayDate
		{
			get
			{
				return _date;
			}
			set
			{
				_date = value;
			}
		}

		public DayOfWeek	HolidayDayOfWeek
		{
			get
			{
				return _dayOfWeek;
			}
			set
			{
				_dayOfWeek = value;
			}
		}

		public void Change(DayOfWeek	dayOfWeek)
		{
			_type = HolidayType.DayOfWeek;
			_dayOfWeek = dayOfWeek;
		}

		public void Change(DateTime		date)
		{
			_type = HolidayType.Date;
			_date = date;
		}

		public bool IsHoliday(DateTime date)
		{
			switch(this.Type) 
			{
			case HolidayType.Date:
				if(date.Year==this.HolidayDate.Year&&
					date.Month==this.HolidayDate.Month&&
					date.Day==this.HolidayDate.Day)
					return true;
				break;
			case HolidayType.DayOfWeek:
				if(date.DayOfWeek==this.HolidayDayOfWeek)
					return true;
				break;
			}
			return false;
		}
	}
}
