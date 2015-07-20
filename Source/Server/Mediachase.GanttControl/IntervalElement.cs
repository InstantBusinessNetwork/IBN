using System;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;
using System.Text;
using System.Xml.XPath;

namespace Mediachase.Gantt
{
	public class IntervalElement : GanttElement
	{
		public const string ElementName = "interval";
		public const string StartName = "start";
		public const string FinishName = "finish";

		private DateTime _start;
		private DateTime _finish;

		public DateTime Start
		{
			get { return _start; }
			//set { _start = value; }
		}

		public DateTime Finish
		{
			get { return _finish; }
			//set { _finish = value; }
		}

		internal IntervalElement(GanttView view, DateTime start, DateTime finish, string id, string type, string tag)
			: base(view, ElementName)
		{
			_start = start;
			_finish = finish;

			AddAttribute(IdName, id);
			AddAttribute(TypeName, type);
			AddAttribute(TagName, tag);

			AddAttribute(StartName, start.ToString(CultureInfo.InvariantCulture));
			AddAttribute(FinishName, finish.ToString(CultureInfo.InvariantCulture));
		}

		internal IntervalElement(GanttView view, XPathNavigator node)
			: base(view, node)
		{
			_start = DateTime.Parse(Attributes[StartName].Value, CultureInfo.InvariantCulture);
			_finish = DateTime.Parse(Attributes[FinishName].Value, CultureInfo.InvariantCulture);
		}

		public override RectangleF CalculateRectangle(DrawingContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			GanttStyle style = this.Style as GanttStyle;

			float height = style.Height;
			float x = context.DateToPixel(_start);
			float x2 = context.DateToPixel(_finish);
			float width = x2 - x;
			float y = context.CurrentTop + (context.ItemHeight - height) / 2;

			return new RectangleF(x, y, width, height);
		}

		public override void RenderGanttElement(DrawingContext context, RectangleF rectangle, Brush backgroundBrush, Pen borderPen)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			float height = rectangle.Height;

			// Make even size
			if ((height % 2) != 0)
				height++;

			float halfHeight = rectangle.Height / 2;

			GanttStyle style = this.Style as GanttStyle;
			PointF[] points = null;

			switch (style.ShapeStyle)
			{
				case ShapeStyle.Height50PercentUp:
					points = new PointF[]
					{
						new PointF(rectangle.Left, rectangle.Top),
						new PointF(rectangle.Right-1, rectangle.Top),
						new PointF(rectangle.Right-1, rectangle.Top + halfHeight),
						new PointF(rectangle.Left, rectangle.Top + halfHeight),
					};
					break;
				case ShapeStyle.Height50PercentDown:
					points = new PointF[]
					{
						new PointF(rectangle.Left, rectangle.Bottom - halfHeight),
						new PointF(rectangle.Right-1, rectangle.Bottom - halfHeight),
						new PointF(rectangle.Right-1, rectangle.Bottom-1),
						new PointF(rectangle.Left, rectangle.Bottom-1),
					};
					break;
				default:
					points = new PointF[]
					{
						new PointF(rectangle.Left, rectangle.Top),
						new PointF(rectangle.Right-1, rectangle.Top),
						new PointF(rectangle.Right-1, rectangle.Bottom-1),
						new PointF(rectangle.Left, rectangle.Bottom-1),
					};
					break;
			}

			if (points != null)
			{
				if (backgroundBrush != null)
					context.Graphics.FillPolygon(backgroundBrush, points);

				if (borderPen != null)
				{
					if (style.ShapeStyle == ShapeStyle.Interval)
					{
						context.Graphics.DrawLine(borderPen, rectangle.Left, rectangle.Top, rectangle.Left, rectangle.Bottom - 1);
						context.Graphics.DrawLine(borderPen, rectangle.Right - 1, rectangle.Top, rectangle.Right - 1, rectangle.Bottom - 1);
					}
					else
					{
						context.Graphics.DrawPolygon(borderPen, points);
					}
				}
			}
		}
	}
}
