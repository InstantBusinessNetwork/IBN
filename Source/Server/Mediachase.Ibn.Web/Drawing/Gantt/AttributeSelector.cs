using System;

namespace Mediachase.Ibn.Web.Drawing.Gantt
{
	public class AttributeSelector
	{
		private string _attributeName;
		private string _attributeValue;

		public AttributeSelector(string selector)
		{
			if (selector == null)
				throw new ArgumentNullException("selector");

			int k = selector.IndexOf('=');
			if (k > 0)
			{
				_attributeName = selector.Substring(0, k);
				_attributeValue = selector.Substring(k + 1);
			}
			else
			{
				_attributeName = selector;
			}
		}

		public bool Matches(Element element)
		{
			if (element == null)
				throw new ArgumentNullException("element");

			bool ret = false;

			AttributeInfo ai;
			if (element.Attributes.TryGetValue(_attributeName, out ai))
			{
				if (_attributeValue == null || _attributeValue == ai.Value)
					ret = true;
			}

			return ret;
		}
	}
}
