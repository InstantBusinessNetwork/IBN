using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.XPath;

namespace Mediachase.Gantt
{
	public class BodyElement : GanttElement
	{
		public const string ElementName = "body";

		internal BodyElement(GanttView view)
			: base(view, ElementName)
		{
		}

		internal BodyElement(GanttView view, XPathNavigator node)
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
			int i = 0;
			int p = 0;
			foreach(Element child in this.Children)
			{
				bool isCalendarElement = child is CalendarElement;

				//if (!isCalendarElement && context.ItemsPerPage > 0 && i == 0 && p > 0 && p <= context.PageNumber)
				//    Parent.StartNewPage(context);

				child.Render(context);

				if (!isCalendarElement)
				{
					context.CurrentTop += context.ItemHeight;
					i++;
					if (i == context.ItemsPerPage)
					{
						i = 0;
						p++;
					}
				}
			}
		}
	}
}
