using System;
using System.Drawing;
using System.Globalization;
using System.Xml.XPath;

namespace Mediachase.Ibn.Web.Drawing.Gantt
{
	public class DataElement : GanttElement
	{
		public const string ElementName = "data";
		public const string StartDateName = "startDate";

		private DateTime? _startDate;

		public DateTime? StartDate
		{
			get { return _startDate; }
		}

		internal DataElement(GanttView view, DateTime? startDate)
			: base(view, ElementName)
		{
			if (startDate != null)
			{
				_startDate = startDate.Value;
				AddAttribute(StartDateName, startDate.Value.ToString(CultureInfo.InvariantCulture));
			}
		}

		internal DataElement(GanttView view, XPathNavigator node)
			: base(view, node)
		{
			if (Attributes.ContainsKey(StartDateName))
				_startDate = DateTime.Parse(Attributes[StartDateName].Value, CultureInfo.InvariantCulture);
		}

		public override void RenderChildren(DrawingContext context)
		{
			foreach (Element child in this.Children)
			{
				child.Render(context);

				context.CurrentTop += child.Height;
				context.FreeHeight -= child.Height;
			}
		}

		public override void StartNewPage(DrawingContext context)
		{
			foreach (Element child in this.Children)
			{
				if (child is HeadElement)
				{
					child.Render(context);
					context.CurrentTop += child.Height;
				}
			}
		}

		public override RectangleF CalculateRectangle(DrawingContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			return new RectangleF(context.Left, context.Top, context.Width, context.Height);
		}
	}
}
