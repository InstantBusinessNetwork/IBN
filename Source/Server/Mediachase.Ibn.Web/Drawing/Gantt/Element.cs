using System.Collections.Generic;
using System.Xml.XPath;

namespace Mediachase.Ibn.Web.Drawing.Gantt
{
	public class Element
	{
		private Element _parent;
		private string _name;
		private Dictionary<string, AttributeInfo> _attributes;
		private List<Element> _children;
		private Style _style;
		private float _width;
		private float _height;
		private bool _skipRendering;

		public Element Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public IDictionary<string, AttributeInfo> Attributes
		{
			get { return _attributes; }
		}

		public IList<Element> Children
		{
			get { return _children; }
		}

		public Style Style
		{
			get { return _style; }
			set { _style = value; }
		}

		public float Width
		{
			get { return _width; }
			set { _width = value; }
		}

		public float Height
		{
			get { return _height; }
			set { _height = value; }
		}

		public bool SkipRendering
		{
			get { return _skipRendering; }
			set { _skipRendering = value; }
		}

		public Element(string name)
		{
			_children = new List<Element>();
			_attributes = new Dictionary<string, AttributeInfo>();
			_name = name;
		}

		internal Element(XPathNavigator node)
			: this(node.Name)
		{
			XPathNavigator navigator = node.CreateNavigator();
			if (navigator.MoveToFirstAttribute())
			{
				do
				{
					AddAttribute(navigator.Name, navigator.Value);
				} while (navigator.MoveToNextAttribute());
			}
		}

		public void AddAttribute(string name, string value)
		{
			if(!string.IsNullOrEmpty(name) && value != null)
				_attributes.Add(name, new AttributeInfo(name, value));
		}

		internal void AddChild(Element element)
		{
			_children.Add(element);
			element.Parent = this;
		}

		public void Render(DrawingContext context)
		{
			RenderElement(context);
			RenderChildren(context);
		}

		public virtual void RenderElement(DrawingContext context)
		{
		}

		public virtual void RenderChildren(DrawingContext context)
		{
			foreach (Element child in _children)
				child.Render(context);
		}

		public virtual void StartNewPage(DrawingContext context)
		{
		}

		public void Save(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement(_name);

			foreach (AttributeInfo ai in _attributes.Values)
				writer.WriteAttributeString(ai.Name, ai.Value);

			foreach (Element child in _children)
				child.Save(writer);

			writer.WriteEndElement();
		}
	}
}
