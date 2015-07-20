using System;
using System.Drawing;
using System.Globalization;
using System.Xml.XPath;

namespace Mediachase.Ibn.Web.Drawing.Gantt
{
	public class DayElement : GanttElement
	{
		public const string ElementName = "day";
		public const string DayOfWeekName = "dayOfWeek";
		public const string DateName = "date";
		public const string HolidayName = "holiday";

		private DayOfWeek? _dayOfWeek;
		private DateTime? _date;
		private bool _isHoliday;

		public bool IsHoliday
		{
			get { return _isHoliday; }
			set { _isHoliday = value; }
		}

		internal DayElement(GanttView view, DayOfWeek dayOfWeek, bool isHoliday)
			: base(view, ElementName)
		{
			_dayOfWeek = dayOfWeek;
			_isHoliday = isHoliday;

			AddAttribute(DayOfWeekName, dayOfWeek.ToString());
			AddAttribute(HolidayName, isHoliday.ToString(CultureInfo.InvariantCulture));
		}

		internal DayElement(GanttView view, DateTime date, bool isHoliday)
			: base(view, ElementName)
		{
			_date = date;
			_isHoliday = isHoliday;

			AddAttribute(DateName, date.ToString(CultureInfo.InvariantCulture));
			AddAttribute(HolidayName, isHoliday.ToString(CultureInfo.InvariantCulture));
		}

		internal DayElement(GanttView view, XPathNavigator node)
			: base(view, node)
		{
			if (Attributes.ContainsKey(DayOfWeekName))
				_dayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), Attributes[DayOfWeekName].Value);
			if (Attributes.ContainsKey(DateName))
				_date = DateTime.Parse(Attributes[DateName].Value, CultureInfo.InvariantCulture);
			_isHoliday = bool.Parse(Attributes[HolidayName].Value);
		}

		public override void RenderElement(DrawingContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			DateTime leftDate = context.PixelToDate(context.Left).Date;
			DateTime rightDate = context.PixelToDate(context.Left + context.Width).Date;
			bool isTopPortion = context.CurrentTop > context.Top;
			float top = isTopPortion ? context.CurrentTop : context.Top;
			float width = context.TimeSpanToPixel(new TimeSpan(1, 0, 0, 0));
			float height = isTopPortion ? context.FreeHeight : context.Height;

			DateTime currentDate = leftDate;
			while (currentDate <= rightDate)
			{
				bool render = false;

				if (_dayOfWeek != null)
				{
					if (currentDate.DayOfWeek == _dayOfWeek.Value)
						render = true;
				}

				if (_date != null)
				{
					if (currentDate == _date.Value)
						render = true;
				}

				if (render)
				{
					GanttStyle style = this.Style as GanttStyle;
					RectangleF rectangle = new RectangleF(context.DateToPixel(currentDate), top, width, height);

					using (Brush backgroundBrush = CreateBackgroundBrush(style))
					using (Pen borderPen = CreateBorderPen(style))
					{
						RenderGanttElement(context, rectangle, backgroundBrush, borderPen);
					}
				}

				currentDate = currentDate.AddDays(1);
			}
		}
	}
}
