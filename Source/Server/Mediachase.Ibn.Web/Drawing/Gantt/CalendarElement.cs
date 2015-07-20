using System;
using System.Drawing;
using System.Xml.XPath;

namespace Mediachase.Ibn.Web.Drawing.Gantt
{
	public class CalendarElement : GanttElement
	{
		public const string ElementName = "calendar";

		internal CalendarElement(GanttView view)
			: base(view, ElementName)
		{
		}

		internal CalendarElement(GanttView view, XPathNavigator node)
			: base(view, node)
		{
		}

		public override RectangleF CalculateRectangle(DrawingContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			bool isTopPortion = context.CurrentTop > context.Top;
			return new RectangleF(context.Left, isTopPortion ? context.CurrentTop : context.Top, context.Width, isTopPortion ? context.FreeHeight : context.Height);
		}

		public override void RenderChildren(DrawingContext context)
		{
			foreach (Element child in this.Children)
				child.Render(context);
		}
	}
}
