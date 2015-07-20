using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Gantt
{
	public class SimpleSelector
	{
		private string _elementName;
		private List<AttributeSelector> _attributeSelectors;

		public int IdAttributesCount
		{
			get { return string.IsNullOrEmpty(_elementName) ? 0 : 0; }
		}

		public int OtherAttributesCount
		{
			get { return _attributeSelectors.Count; }
		}

		public int ElementNamesCount
		{
			get { return string.IsNullOrEmpty(_elementName) ? 0 : 1; }
		}

		public SimpleSelector(string selector)
		{
			if(selector == null)
				throw new ArgumentNullException("selector");

			_attributeSelectors = new List<AttributeSelector>();

			int length = selector.Length;
			int position = 0;

			while (position < length)
			{
				int i = selector.IndexOf('[', position);
				if (i < 0)
				{
					if(_elementName == null)
						_elementName = selector.Substring(position);
					position = length;
				}
				else
				{
					if (_elementName == null)
						_elementName = selector.Substring(position, i);

					int j = selector.IndexOf(']', i);
					if (j < 0)
					{
						position = length;
					}
					else
					{
						AttributeSelector attributeSelector = new AttributeSelector(selector.Substring(i + 1, j - i - 1));
						_attributeSelectors.Add(attributeSelector);
						position = j + 1;
					}
				}
			}

			if (_elementName == "*")
				_elementName = string.Empty;
		}

		public bool Matches(Element element)
		{
			if (element == null)
				throw new ArgumentNullException("element");

			bool ret = false;

			if (string.IsNullOrEmpty(_elementName) || _elementName == element.Name)
			{
				ret = true;
				foreach (AttributeSelector attributeSelector in _attributeSelectors)
				{
					if (!attributeSelector.Matches(element))
					{
						ret = false;
						break;
					}
				}
			}

			return ret;
		}
	}
}
