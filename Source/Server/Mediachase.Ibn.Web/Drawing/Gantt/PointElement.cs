using System;
using System.Drawing;
using System.Globalization;
using System.Xml.XPath;

namespace Mediachase.Ibn.Web.Drawing.Gantt
{
	public class PointElement : GanttElement
	{
		public const string ElementName = "point";
		public const string ValueName = "value";

		private DateTime _value;
		private float _startX;

		public DateTime Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public override float StartX
		{
			get { return _startX; }
		}

		internal PointElement(GanttView view, DateTime value, string id, string type, string tag)
			: base(view, ElementName)
		{
			_value = value;

			AddAttribute(IdName, id);
			AddAttribute(TypeName, type);
			AddAttribute(TagName, tag);

			AddAttribute(ValueName, value.ToString(CultureInfo.InvariantCulture));
		}

		internal PointElement(GanttView view, XPathNavigator node)
			: base(view, node)
		{
			_value = DateTime.Parse(Attributes[ValueName].Value, CultureInfo.InvariantCulture);
		}

		public override RectangleF CalculateRectangle(DrawingContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			GanttStyle style = this.Style as GanttStyle;

			_startX = context.DateToPixel(_value);
			float x = _startX;

			float width = style.Width;
			float height = style.Height;

			// Make odd size
			if ((width % 2) == 0)
				width--;
			if ((height % 2) == 0)
				height--;

			float minSize = Math.Min(style.Width, style.Height);

			switch (style.ShapeStyle)
			{
				case ShapeStyle.Circle:
					width = height = minSize;
					x -= (width - 1) / 2;
					break;
				case ShapeStyle.LeftTriangle:
					height = minSize;
					width = (minSize + 1) / 2;
					x += 1 - width;
					break;
				case ShapeStyle.RightTriangle:
					height = minSize;
					width = (minSize + 1) / 2;
					break;
				default:
					x -= (width - 1) / 2;
					break;
			}

			float y = context.CurrentTop + (context.ItemHeight - height) / 2;

			return new RectangleF(x, y, width, height);
		}

		public override void RenderGanttElement(DrawingContext context, RectangleF rectangle, Brush backgroundBrush, Pen borderPen)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			float halfWidth = (rectangle.Width - 1) / 2;
			float halfHeight = (rectangle.Height - 1) / 2;

			float centerX = rectangle.Left + halfWidth;
			float centerY = rectangle.Top + halfHeight;

			GanttStyle style = this.Style as GanttStyle;
			PointF[] points = null;
			RectangleF ellipse = RectangleF.Empty;

			switch (style.ShapeStyle)
			{
				case ShapeStyle.Circle:
				case ShapeStyle.Ellipse:
					ellipse = rectangle;
					ellipse.Width--;
					ellipse.Height--;
					break;
				case ShapeStyle.LeftTriangle:
					points = new PointF[]
					{
						new PointF(rectangle.Right-1, rectangle.Top),
						new PointF(rectangle.Right-1, rectangle.Bottom-1),
						new PointF(rectangle.Left, centerY),
					};
					break;
				case ShapeStyle.RightTriangle:
					points = new PointF[]
					{
						new PointF(rectangle.Left, rectangle.Top),
						new PointF(rectangle.Left, rectangle.Bottom-1),
						new PointF(rectangle.Right-1, centerY),
					};
					break;
				case ShapeStyle.Rhombus:
					points = new PointF[]
					{
						new PointF(centerX, rectangle.Top),
						new PointF(rectangle.Right-1, centerY),
						new PointF(centerX, rectangle.Bottom-1),
						new PointF(rectangle.Left, centerY),
					};
					break;
				case ShapeStyle.PentagonDown:
					points = new PointF[]
					{
						new PointF(rectangle.Left, rectangle.Top),
						new PointF(rectangle.Right-1, rectangle.Top),
						new PointF(rectangle.Right-1, centerY),
						new PointF(centerX, rectangle.Bottom-1),
						new PointF(rectangle.Left, centerY),
					};
					break;
				case ShapeStyle.PentagonUp:
					points = new PointF[]
					{
						new PointF(centerX, rectangle.Top),
						new PointF(rectangle.Right-1, centerY),
						new PointF(rectangle.Right-1, rectangle.Bottom-1),
						new PointF(rectangle.Left, rectangle.Bottom-1),
						new PointF(rectangle.Left, centerY),
					};
					break;
				case ShapeStyle.PentagonRight:
					points = new PointF[]
					{
						new PointF(rectangle.Right - 1, centerY),
						new PointF(centerX, rectangle.Bottom - 1),
						new PointF(rectangle.Left, rectangle.Bottom-1),
						new PointF(rectangle.Left, rectangle.Top),
						new PointF(centerX, rectangle.Top),
					};
					break;
				case ShapeStyle.PentagonLeft:
					points = new PointF[]
					{
						new PointF(rectangle.Left, centerY),
						new PointF(centerX, rectangle.Top),
						new PointF(rectangle.Right - 1, rectangle.Top),
						new PointF(rectangle.Right - 1, rectangle.Bottom - 1),
						new PointF(centerX, rectangle.Bottom - 1),
					};
					break;
				default:
					points = new PointF[]
					{
						new PointF(rectangle.Left, rectangle.Top),
						new PointF(rectangle.Right, rectangle.Top),
						new PointF(rectangle.Right, rectangle.Bottom),
						new PointF(rectangle.Left, rectangle.Bottom),
					};
					break;
			}

			if (ellipse != RectangleF.Empty)
			{
				if (backgroundBrush != null)
					context.Graphics.FillEllipse(backgroundBrush, ellipse);
				if (borderPen != null)
					context.Graphics.DrawEllipse(borderPen, ellipse);
			}

			if (points != null)
			{
				if (backgroundBrush != null)
					context.Graphics.FillPolygon(backgroundBrush, points);
				if (borderPen != null)
					context.Graphics.DrawPolygon(borderPen, points);
			}
		}
	}
}
