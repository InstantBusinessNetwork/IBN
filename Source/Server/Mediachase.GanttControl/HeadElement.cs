using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Xml.XPath;

namespace Mediachase.Gantt
{
	public class HeadElement : GanttElement
	{
		public const string ElementName = "head";

		private RectangleF _rectangle;

		internal HeadElement(GanttView view)
			: base(view, ElementName)
		{
		}

		internal HeadElement(GanttView view, XPathNavigator node)
			: base(view, node)
		{
		}

		public override RectangleF CalculateRectangle(DrawingContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			if (context.HeaderItemHeight > 0)
				return new RectangleF(context.Left, context.CurrentTop, context.Width, CalculateHeight(context.HeaderItemHeight));
			else
				return RectangleF.Empty;
		}

		public override void RenderGanttElement(DrawingContext context, RectangleF rectangle, Brush backgroundBrush, Pen borderPen)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			GanttStyle style = this.Style as GanttStyle;

			if (backgroundBrush != null)
				context.Graphics.FillRectangle(backgroundBrush, rectangle);

			if (borderPen != null)
			{
				int n = this.Children.Count + 1;
				for (int i = 0; i < n; i++)
				{
					float y = rectangle.Top + i * (context.HeaderItemHeight + style.BorderWidth);
					context.Graphics.DrawLine(borderPen, rectangle.Left, y, rectangle.Right, y);
				}
			}

			_rectangle = rectangle;
		}

		public override void RenderChildren(DrawingContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			GanttStyle style = this.Style as GanttStyle;
			float y = context.CurrentTop;

			foreach (Element child in this.Children)
			{
				context.CurrentTop += style.BorderWidth;

				child.Render(context);

				context.CurrentTop += child.Height;
			}

			context.CurrentTop = y;

			context.Graphics.SetClip(_rectangle, CombineMode.Exclude);
		}

		public float CalculateHeight(float headerItemHeight)
		{
			GanttStyle style = this.Style as GanttStyle;
			return headerItemHeight * this.Children.Count + style.BorderWidth * (this.Children.Count + 1);
		}
	}
}
