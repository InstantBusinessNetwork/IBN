using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Xml.XPath;

namespace Mediachase.Ibn.Web.Drawing.Gantt
{
	public class AxisElement : GanttElement
	{
		public const string ElementName = "axis";
		public const string ScaleName = "scale";
		public const string IntervalName = "interval";
		public const string FormatName = "format";
		public const string FirstDayName = "firstDay";
		public const string TitleTypeName = "titleType";
		public const string WidthName = "width";

		private ScaleLevel _scale;
		private int _interval;
		private string _format;
		private DateTime _startDate;
		private DateTime _viewStartDate;
		private DayOfWeek _firstDay;
		private TitleType _titleType = TitleType.Start;

		public TitleType TitleType
		{
			get
			{
				return _titleType;
			}
			set
			{
				_titleType = value;
				if(Attributes.ContainsKey(TitleTypeName))
					Attributes[TitleTypeName].Value = value.ToString();
				else
					Attributes.Add(TitleTypeName, new AttributeInfo(TitleTypeName, value.ToString()));
			}
		}

		public string Format
		{
			get
			{
				return _format;
			}
			set
			{
				_format = value;
				Attributes[FormatName].Value = value;

			}
		}

		public TimeSpan Interval
		{
			get
			{
				switch (_scale)
				{
					case ScaleLevel.Hour:
						return new TimeSpan(0, _interval, 0, 0);
					case ScaleLevel.Day:
						return new TimeSpan(_interval, 0, 0, 0);
					case ScaleLevel.Week:
						return new TimeSpan(_interval * 7, 0, 0, 0);
					default:
						throw new NotSupportedException(_scale.ToString());
				}
			}
		}

		public DateTime StartDate
		{
			get { return _startDate; }
		}

		public DateTime ViewStartDate
		{
			get { return _viewStartDate; }
			set
			{
				_viewStartDate = value;
				_startDate = value;
				switch (_scale)
				{
					case ScaleLevel.Hour:
						_startDate = new DateTime(_startDate.Year, _startDate.Month, _startDate.Day, _startDate.Hour, 0, 0);
						break;
					case ScaleLevel.Day:
						_startDate = _startDate.Date;
						break;
					case ScaleLevel.Week:
						while (_startDate.DayOfWeek != _firstDay)
						{
							_startDate = _startDate.AddDays(-1);
						}
						_startDate = _startDate.Date;
						break;
				}
			}
		}

		internal AxisElement(GanttView view, ScaleLevel scale, int interval, string format, DayOfWeek? firstDay)
			: base(view, ElementName)
		{
			_scale = scale;
			_interval = interval;
			_format = format;
			if (firstDay != null)
				_firstDay = firstDay.Value;

			AddAttribute(ScaleName, scale.ToString());
			AddAttribute(IntervalName, interval.ToString(CultureInfo.InvariantCulture));
			AddAttribute(FormatName, format);
			if (firstDay != null)
				AddAttribute(FirstDayName, firstDay.Value.ToString());
		}

		internal AxisElement(GanttView view, XPathNavigator node)
			: base(view, node)
		{
			_scale = (ScaleLevel)Enum.Parse(typeof(ScaleLevel), Attributes[ScaleName].Value);
			_interval = int.Parse(Attributes[IntervalName].Value, CultureInfo.InvariantCulture);
			_format = Attributes[FormatName].Value;
			if(_scale == ScaleLevel.Week)
				_firstDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), Attributes[FirstDayName].Value);

			if (Attributes.ContainsKey(TitleTypeName))
			{
				string titleTypeValue = Attributes[TitleTypeName].Value;
				if (!string.IsNullOrEmpty(titleTypeValue))
				{
					_titleType = (TitleType)Enum.Parse(typeof(TitleType), titleTypeValue);
				}
			}

			if (Attributes.ContainsKey(WidthName))
			{
				string widthValue = Attributes[WidthName].Value;
				if (!string.IsNullOrEmpty(widthValue))
				{
					this.Width = int.Parse(widthValue, NumberStyles.Integer, CultureInfo.InvariantCulture);
				}
			}
		}

		public void CalculateWidth(AxisElement nextAxis)
		{
			if (this.Width <= 0)
			{
				if (nextAxis != null)
				{
					this.Width = Convert.ToSingle(nextAxis.Width * this.Interval.TotalMinutes / nextAxis.Interval.TotalMinutes);
				}
				else
				{
					// TODO: Calculate cell width.
					switch (_scale)
					{
						case ScaleLevel.Hour:
							this.Width = 60;
							break;
						case ScaleLevel.Day:
							this.Width = 24;
							break;
					}
				}
			}
		}

		public DateTime GetCellStartDate(int index)
		{
			TimeSpan interval = new TimeSpan(index * this.Interval.Ticks);
			return _startDate + interval;
		}

		public override void RenderElement(DrawingContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			GanttStyle style = this.Style as GanttStyle;
			this.Height = context.HeaderItemHeight;
			float visibleRight = context.Left + context.Width;

			int leftIndex = GetIndexByPoint(context.Left);
			int rightIndex = GetIndexByPoint(visibleRight);

			TimeSpan axisInterval = new TimeSpan();
			for (int cellIndex = leftIndex; cellIndex <= rightIndex; cellIndex++)
			{
				TimeCell cell = GetCell(cellIndex);
				RectangleF rectangle = new RectangleF(cell.Left, context.CurrentTop, cell.Width, this.Height);

				if (rectangle.Left >= context.Left && rectangle.Right <= visibleRight)
					axisInterval += cell.Interval;
				else if (rectangle.Left < context.Left && rectangle.Right <= visibleRight)
					axisInterval += cell.GetTimeInterval(rectangle.Right - context.Left);
				else if (rectangle.Left >= context.Left && rectangle.Right > visibleRight)
					axisInterval += cell.GetTimeInterval(visibleRight - rectangle.Left);

				using (SolidBrush backgroundBrush = new SolidBrush(style.BackgroundColor))
				{
					context.Graphics.FillRectangle(backgroundBrush, rectangle);
				}

				StringBuilder text = new StringBuilder();

				switch (_titleType)
				{
					case TitleType.Start:
						text.Append(GetDateText(cell.StartDate, _format, context.Provider));
						break;
					case TitleType.StartEnd:
						text.Append(GetDateText(cell.StartDate, _format, context.Provider));
						text.Append(" - ");
						text.Append(GetDateText(cell.StartDate.AddTicks(cell.Interval.Ticks - 1), _format, context.Provider));
						break;
				}

				if (text.Length > 0)
				{
					if (_scale == ScaleLevel.Week)
					{
						string weekNumber = string.Format(CultureInfo.InvariantCulture, " (#{0})", Iso8601WeekNumber.GetWeekNumber(cell.StartDate));
						text.Append(weekNumber);
					}

					using (SolidBrush textBrush = new SolidBrush(style.ForegroundColor))
					{
						// TODO: Load font style from style sheet.
						Font font = new Font("Arial", 8, FontStyle.Regular);

						StringFormat stringFormat = new StringFormat();
						stringFormat.Alignment = StringAlignment.Center;
						stringFormat.LineAlignment = StringAlignment.Center;

						context.Graphics.DrawString(text.ToString(), font, textBrush, rectangle, stringFormat);
					}
				}

				rectangle.Width--;
				rectangle.Height--;
				using (Pen borderPen = new Pen(style.BorderColor, style.BorderWidth))
				{
					context.Graphics.DrawLine(borderPen, rectangle.Left, rectangle.Top, rectangle.Left, rectangle.Bottom);
				}
			}

			context.EndDate = _viewStartDate + axisInterval;
		}

		private static string GetDateText(DateTime date, string format, IFormatProvider provider)
		{
			return date.ToString(format, provider);
		}

		internal void SetEndDate(DrawingContext context)
		{
			float visibleRight = context.Left + context.Width;
			int leftIndex = GetIndexByPoint(context.Left);
			int rightIndex = GetIndexByPoint(visibleRight);

			TimeSpan axisInterval = new TimeSpan();
			for (int cellIndex = leftIndex; cellIndex <= rightIndex; cellIndex++)
			{
				TimeCell cell = GetCell(cellIndex);
				RectangleF rectangle = new RectangleF(cell.Left, context.CurrentTop, cell.Width, this.Height);

				if (rectangle.Left >= context.Left && rectangle.Right <= visibleRight)
					axisInterval += cell.Interval;
				else if (rectangle.Left < context.Left && rectangle.Right <= visibleRight)
					axisInterval += cell.GetTimeInterval(rectangle.Right - context.Left);
				else if (rectangle.Left >= context.Left && rectangle.Right > visibleRight)
					axisInterval += cell.GetTimeInterval(visibleRight - rectangle.Left);
			}

			context.EndDate = _viewStartDate + axisInterval;
		}

		private int GetIndexByPoint(float left)
		{
			int index = 0;

			TimeCell cell = GetCell(index);

			// Step 1. First approximation
			index = Convert.ToInt32(left / cell.Width);

			// Step 2. Calibration
			bool next = false;
			bool previous = false;

			while (true)
			{
				if (next && previous)
					throw new GanttControlException(string.Format(CultureInfo.CurrentUICulture, "CalibrateIndex deadlock. Index = '{0}', Point X = '{1}'", index, left));

				cell = GetCell(index);

				if ((cell.Left + cell.Width) < left)
				{
					index++;
					next = true;
					continue;
				}

				if (cell.Left > left)
				{
					index--;
					previous = true;
					continue;
				}

				break;
			}

			return index;
		}

		private TimeCell GetCell(int index)
		{
			DateTime startDate = GetCellStartDate(index);
			TimeSpan interval = this.Interval;
			float width = this.Width;
			float x = DrawingContext.DateToPixel(startDate, _viewStartDate, interval, width);

			return new TimeCell(index, startDate, interval, x, width, this);
		}
	}
}
