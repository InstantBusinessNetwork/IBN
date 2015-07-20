using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace Mediachase.Ibn.Web.Drawing.Gantt
{
	public class Style : IComparable<Style>
	{
		public const string Inherit = "inherit";

		private string[] _propertyNames;
		private string[] _defaultValues;
		private List<string> _notInheritableNames;

		public int Order { get; private set; }
		public StyleSelector Selector { get; private set; }
		public IDictionary<string, string> Properties { get; private set; }

		protected Style(string[] propertyNames, string[] defaultValues, string[] notInheritableNames)
		{
			_propertyNames = propertyNames;
			_defaultValues = defaultValues;
			_notInheritableNames = new List<string>(notInheritableNames);

			Properties = new Dictionary<string, string>(propertyNames.Length);
			foreach (string name in propertyNames)
				Properties[name] = null;
		}

		public void Load(XPathNavigator node, string namespaceId, int order)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			Order = order;
			Selector = new StyleSelector(node.GetAttribute("selector", namespaceId));

			foreach (string name in _propertyNames)
			{
				XPathNavigator child = node.SelectSingleNode(name);
				if (child != null)
					Properties[name] = child.Value;
			}
		}

		/// <summary>
		/// Copies properties from given style.
		/// Returns False if there is any undefined property.
		/// </summary>
		/// <param name="style"></param>
		/// <returns></returns>
		public bool CopyFrom(Style style, bool parentStyle)
		{
			bool ret = true;
			foreach (string name in _propertyNames)
			{
				string value = Properties[name];
				if (!parentStyle && value == null || parentStyle && (value == Inherit || value == null && !_notInheritableNames.Contains(name)))
					value = style.Properties[name];
				if (value != null)
				{
					Properties[name] = value;
					if (value == Inherit)
						ret = false;
				}
				else
					ret = false;
			}
			return ret;
		}

		public void ApplyDefaultValues()
		{
			int i = 0;
			foreach(string name in _propertyNames)
			{
				if (Properties[name] == null)
					Properties[name] = _defaultValues[i];
				i++;
			}
		}

		public virtual void CalculateValues()
		{
		}

		public override int GetHashCode()
		{
			return Selector.GetHashCode();
		}

		#region IComparable<Style> Members

		public int CompareTo(Style other)
		{
			return Compare(this, other);
		}

		#endregion

		public override bool Equals(object obj)
		{
			return (Compare(this, obj as Style) == 0);
		}

		public static bool operator ==(Style style1, Style style2)
		{
			return (Compare(style1, style2) == 0);
		}
		public static bool operator !=(Style style1, Style style2)
		{
			return (Compare(style1, style2) != 0);
		}
		public static bool operator <(Style style1, Style style2)
		{
			return (Compare(style1, style2) < 0);
		}
		public static bool operator >(Style style1, Style style2)
		{
			return (Compare(style1, style2) > 0);
		}


		private static int Compare(Style style1, Style style2)
		{
			int result;

			object object1 = style1 as object;
			object object2 = style2 as object;

			if (object1 == null)
			{
				if (object2 == null)
					result = 0;
				else
					result = -1;
			}
			else
			{
				if (object2 == null)
					result = 1;
				else
				{
					result = style1.Selector.IdAttributesCount.CompareTo(style2.Selector.IdAttributesCount);
					if (result == 0)
						result = style1.Selector.OtherAttributesCount.CompareTo(style2.Selector.OtherAttributesCount);
					if (result == 0)
						result = style1.Selector.ElementNamesCount.CompareTo(style2.Selector.ElementNamesCount);
					if (result == 0)
						result = style1.Order.CompareTo(style2.Order);
				}
			}

			return result;
		}
	}
}
