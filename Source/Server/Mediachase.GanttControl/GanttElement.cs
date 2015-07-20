using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Xml.XPath;

namespace Mediachase.Gantt
{
	public class GanttElement : Element
	{
		public const string IdName = "id";
		public const string TypeName = "type";
		public const string TagName = "tag";

		private GanttView _view;
		private RectangleF _rectangle;

		public GanttView View
		{
			get { return _view; }
			set { _view = value; }
		}

		public RectangleF Rectangle
		{
			get { return _rectangle; }
		}

		public virtual float StartX
		{
			get { return _rectangle.Left; }
		}

		internal GanttElement(GanttView view, string name)
			: base(name)
		{
			_view = view;
		}

		internal GanttElement(GanttView view, XPathNavigator node)
			: base(node)
		{
			_view = view;
		}

		public override void RenderElement(DrawingContext context)
		{
			GanttStyle style = this.Style as GanttStyle;
			if (style.ShapeStyle != ShapeStyle.None)
			{
				_rectangle = CalculateRectangleAndSetSize(context);

				using (Brush backgroundBrush = CreateBackgroundBrush(style))
				using (Pen borderPen = CreateBorderPen(style))
				{
					RenderGanttElement(context, _rectangle, backgroundBrush, borderPen);
				}
			}
		}

		public RectangleF CalculateRectangleAndSetSize(DrawingContext context)
		{
			RectangleF rectangle = CalculateRectangle(context);

			this.Width = rectangle.Width;
			this.Height = rectangle.Height;

			return rectangle;
		}

		public virtual RectangleF CalculateRectangle(DrawingContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			GanttStyle style = this.Style as GanttStyle;

			float width = this.Width;
			if (width <= 0)
				width = style.Width;

			float height = this.Height;
			if (height <= 0)
				height = style.Height;

			return new RectangleF(context.Left, context.CurrentTop, width, height);
		}

		public virtual void RenderGanttElement(DrawingContext context, RectangleF rectangle, Brush backgroundBrush, Pen borderPen)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			if (backgroundBrush != null)
				context.Graphics.FillRectangle(backgroundBrush, rectangle);
			rectangle.Width--;
			rectangle.Height--;
			if (borderPen != null)
				context.Graphics.DrawRectangle(borderPen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		protected static Brush CreateBackgroundBrush(GanttStyle style)
		{
			if (style == null)
				throw new ArgumentNullException("style");

			switch (style.FillStyle)
			{
				case FillStyle.Hatch:
					return new HatchBrush(style.HatchStyle, style.ForegroundColor, style.BackgroundColor);
				case FillStyle.Solid:
					return new SolidBrush(style.BackgroundColor);
				default:
					return null;
			}
		}

		protected static Pen CreateBorderPen(GanttStyle style)
		{
			if (style == null)
				throw new ArgumentNullException("style");

			switch (style.BorderStyle)
			{
				case BorderStyle.Solid:
					return new Pen(style.BorderColor, style.BorderWidth);
				default:
					return null;
			}
		}
	}
}
