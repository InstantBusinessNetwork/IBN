using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Mediachase.Ibn.Web.Drawing.Gantt
{
	public class GanttView
	{
		public const string StyleNamespaceId = "";

		private Dictionary<string, Element> _map;
		private DataElement _data;
		private HeadElement _head;
		private BodyElement _body;
		private CalendarElement _calendar;
		private List<AxisElement> _axes;
		private List<Style> _styles;
		private DateTime _startDate = DateTime.Now;
		private IFormatProvider _provider;

		private float _headerItemHeight;
		private float _itemHeight;

		// public

		public IFormatProvider Provider
		{
			get { return _provider; }
			set { _provider = value; }
		}

		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				_startDate = value;
				foreach (AxisElement axis in _axes)
					axis.ViewStartDate = value;
			}
		}

		#region public .ctor()
		public GanttView(float headerItemHeight, float itemHeight)
		{
			_map = new Dictionary<string, Element>();
			_styles = new List<Style>();
			_axes = new List<AxisElement>(2);
			_headerItemHeight = headerItemHeight;
			_itemHeight = itemHeight;
		}
		#endregion

		#region public LoadDataFromFile(...)
		public void LoadDataFromFile(string filePath)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filePath);
			LoadData(doc);
		}
		#endregion
		#region public LoadData(...)
		public void LoadData(IXPathNavigable data)
		{
			_map.Clear();
			_axes.Clear();

			LoadElement(data.CreateNavigator().SelectSingleNode("data"));
		}
		#endregion

		#region public LoadStyleSheetFromFile(...)
		public void LoadStyleSheetFromFile(string filePath)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filePath);
			LoadStyleSheet(doc);
		}
		#endregion
		#region public LoadStyleSheet(...)
		public void LoadStyleSheet(IXPathNavigable styles)
		{
			_styles.Clear();
			int ruleOrder = 0;
			foreach (XPathNavigator node in styles.CreateNavigator().SelectSingleNode("style").SelectChildren(XPathNodeType.Element))
			{
				switch (node.Name)
				{
					case "rule":
						GanttStyle style = new GanttStyle();
						style.Load(node, StyleNamespaceId, ruleOrder);
						_styles.Add(style);
						ruleOrder++;
						break;
				}
			}
			_styles.Sort();
		}
		#endregion

		#region public void ApplyStyleSheet()
		public void ApplyStyleSheet()
		{
			ApplyStyle(_data);
			CalculateAxesWidth();
		}
		#endregion

		#region public int GetPortionX(DateTime date, int portionWidth)
		public int GetPortionX(DateTime date, int portionWidth)
		{
			DrawingContext context = new DrawingContext(_startDate, 0, 0, portionWidth, 0, 0, 0);

			CalculateAxesWidth();
			_axes[_axes.Count - 1].SetEndDate(context);

			float x = context.DateToPixel(date);
			return Convert.ToInt32(Math.Floor(x / portionWidth));
		}
		#endregion

		#region public RenderPortion(...)
		public void RenderPortion(Point portionOffset, Size portionSize, int itemsPerPage, int pageNumber, ImageFormat format, Stream output)
		{
			using (Bitmap bitmap = new Bitmap(portionSize.Width, portionSize.Height))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					float x = portionSize.Width * portionOffset.X;
					float y;
					if (portionOffset.Y < 0) // Render head
						y = 0;
					else
						y = portionSize.Height * portionOffset.Y + pageNumber * _itemHeight * itemsPerPage + _head.CalculateHeight(_headerItemHeight);

					using (Matrix matrix = new Matrix())
					{
						matrix.Translate(-x, -y);
						graphics.Transform = matrix;
					}

					DrawingContext context = new DrawingContext(_startDate, x, y, portionSize.Width, portionSize.Height, itemsPerPage, pageNumber);
					context.Graphics = graphics;
					context.Provider = _provider;
					context.HeaderItemHeight = _headerItemHeight;
					context.ItemHeight = _itemHeight;

					_data.Render(context);
				}
				bitmap.Save(output, format);
			}
		}
		#endregion

		#region public Create*Element(...)
		public DataElement CreateDataElement(DateTime? startDate)
		{
			DataElement element = new DataElement(this, startDate);
			_data = element;
			if (_data.StartDate != null)
				_startDate = _data.StartDate.Value;
			AddElement(element, null);

			CreateHeadElement();
			CreateBodyElement();
			CreateCalendarElement();

			return element;
		}

		public AxisElement CreateAxisElement(ScaleLevel scale, int interval, string format, DayOfWeek? firstDay)
		{
			AxisElement element = new AxisElement(this, scale, interval, format, firstDay);
			element.ViewStartDate = _startDate;
			_axes.Add(element);
			AddElement(element, _head);
			return element;
		}

		public DayElement CreateDayElement(DayOfWeek dayOfWeek, bool isHoliday)
		{
			DayElement element = new DayElement(this, dayOfWeek, isHoliday);
			AddElement(element, _calendar);
			return element;
		}

		public DayElement CreateDateElement(DateTime date, bool isHoliday)
		{
			DayElement element = new DayElement(this, date, isHoliday);
			AddElement(element, _calendar);
			return element;
		}

		public SpanElement CreateSpanElement(string id, string type, string tag)
		{
			SpanElement element = new SpanElement(this, id, type, tag);
			AddElement(element, _body);
			return element;
		}

		public IntervalElement CreateIntervalElement(Element parent, DateTime start, DateTime finish, string id, string type, string tag)
		{
			IntervalElement element = new IntervalElement(this, start, finish, id, type, tag);
			AddElement(element, parent);
			return element;
		}

		public PointElement CreatePointElement(Element parent, DateTime value, string id, string type, string tag)
		{
			PointElement element = new PointElement(this, value, id, type, tag);
			AddElement(element, parent);
			return element;
		}

		public RelationElement CreateRelationElement(string id, string type, string tag, string originId, string targetId)
		{
			RelationElement element = new RelationElement(this, id, type, tag, originId, targetId);
			element.Origin = _map[element.OriginId];
			element.Target = _map[element.TargetId];
			AddElement(element, _body);
			return element;
		}
		#endregion

		#region public void GenerateDataXml(Encoding encoding, Stream output)
		public void GenerateDataXml(Encoding encoding, Stream output)
		{
			XmlTextWriter w = new XmlTextWriter(output, encoding);
			w.Formatting = Formatting.Indented;
			w.IndentChar = '	';
			w.Indentation = 1;
			w.WriteStartDocument();

			_data.Save(w);

			w.Flush();
		}
		#endregion

		public float CalculateHeadHeight()
		{
			return _head.CalculateHeight(_headerItemHeight);
		}


		// private
		private HeadElement CreateHeadElement()
		{
			HeadElement element = new HeadElement(this);
			_head = element;
			AddElement(element, _data);
			return element;
		}

		private BodyElement CreateBodyElement()
		{
			BodyElement element = new BodyElement(this);
			_body = element;
			AddElement(element, _data);
			return element;
		}

		private CalendarElement CreateCalendarElement()
		{
			CalendarElement element = new CalendarElement(this);
			_calendar = element;
			AddElement(element, _body);
			return element;
		}

		private void AddElement(Element element, Element parent)
		{
			if (element != null)
			{
				AddElementToMap(element);
				if (parent != null)
				{
					parent.AddChild(element);
				}
			}
		}

		#region private ApplyStyle(...)
		private void ApplyStyle(Element element)
		{
			List<Style> styles = new List<Style>();
			foreach (Style style in _styles)
			{
				if (style.Selector.Matches(element))
					styles.Add(style);
			}

			GanttStyle computedStyle = new GanttStyle();

			bool allPropertiesAreDefined = false;
			for (int i = styles.Count - 1; i >= 0; i--)
			{
				allPropertiesAreDefined = computedStyle.CopyFrom(styles[i], false);
				if (allPropertiesAreDefined)
					break;
			}

			if(!allPropertiesAreDefined && element.Parent != null)
				allPropertiesAreDefined = computedStyle.CopyFrom(element.Parent.Style, true);

			if (!allPropertiesAreDefined)
				computedStyle.ApplyDefaultValues();

			computedStyle.CalculateValues();
			element.Style = computedStyle;

			foreach (Element child in element.Children)
			{
				ApplyStyle(child);
			}
		}
		#endregion

		#region private LoadElement(...)
		private Element LoadElement(XPathNavigator node)
		{
			Element element = null;

			switch (node.Name)
			{
				case DataElement.ElementName:
					_data = new DataElement(this, node);
					if(_data.StartDate != null)
						_startDate = _data.StartDate.Value;
					element = _data;
					break;
				case HeadElement.ElementName:
					_head = new HeadElement(this, node);
					element = _head;
					break;
				case BodyElement.ElementName:
					_body = new BodyElement(this, node);
					element = _body;
					break;
				case CalendarElement.ElementName:
					_calendar = new CalendarElement(this, node);
					element = _calendar;
					break;
				case DayElement.ElementName:
					element = new DayElement(this, node);
					break;
				case AxisElement.ElementName:
					AxisElement axis = new AxisElement(this, node);
					axis.ViewStartDate = _startDate;
					_axes.Add(axis);
					element = axis;
					break;
				case SpanElement.ElementName:
					element = new SpanElement(this, node);
					break;
				case IntervalElement.ElementName:
					element = new IntervalElement(this, node);
					break;
				case PointElement.ElementName:
					element = new PointElement(this, node);
					break;
				case RelationElement.ElementName:
					RelationElement relation = new RelationElement(this, node);
					relation.Origin = _map[relation.OriginId];
					relation.Target = _map[relation.TargetId];
					element = relation;
					break;
				default:
					element = new GanttElement(this, node);
					break;
			}

			if (element != null)
			{
				AddElementToMap(element);

				if (node.HasChildren)
				{
					foreach (XPathNavigator childNode in node.SelectChildren(XPathNodeType.Element))
					{
						Element childElement = LoadElement(childNode);
						if (childElement != null)
							element.AddChild(childElement);
					}
				}
			}

			return element;
		}
		#endregion

		private void AddElementToMap(Element element)
		{
			AttributeInfo id;
			if (element.Attributes.TryGetValue("id", out id) && id != null && !string.IsNullOrEmpty(id.Value))
				_map.Add(id.Value, element);
		}

		private void CalculateAxesWidth()
		{
			AxisElement axis = null;
			for (int i = _axes.Count - 1; i >= 0; i--)
			{
				_axes[i].CalculateWidth(axis);
				axis = _axes[i];
			}
		}
	}
}
