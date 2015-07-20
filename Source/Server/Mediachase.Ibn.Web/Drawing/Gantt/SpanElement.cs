using System;
using System.Drawing;
using System.Xml.XPath;

namespace Mediachase.Ibn.Web.Drawing.Gantt
{
	public class SpanElement : GanttElement
	{
		public const string ElementName = "span";

		internal SpanElement(GanttView view, string id, string type, string tag)
			: base(view, ElementName)
		{
			AddAttribute(IdName, id);
			AddAttribute(TypeName, type);
			AddAttribute(TagName, tag);
		}

		internal SpanElement(GanttView view, XPathNavigator node)
			: base(view, node)
		{
		}

		public override RectangleF CalculateRectangle(DrawingContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			return new RectangleF(context.Left, context.CurrentTop, context.Width, context.ItemHeight);
		}

		public override void RenderChildren(DrawingContext context)
		{
			float y = context.CurrentTop;

			foreach (Element child in this.Children)
			{
				child.Render(context);

				context.CurrentTop = y;
			}
		}
	}
}
