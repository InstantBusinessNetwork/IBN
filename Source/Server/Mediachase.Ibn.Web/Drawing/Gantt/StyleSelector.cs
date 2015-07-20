using System.Collections.Generic;

namespace Mediachase.Ibn.Web.Drawing.Gantt
{
	public class StyleSelector
	{
		private List<SimpleSelector> _simpleSelectors;
		private int _idAttributesCount;
		private int _otherAttributesCount;
		private int _elementNamesCount;

		public int IdAttributesCount
		{
			get { return _idAttributesCount; }
		}

		public int OtherAttributesCount
		{
			get { return _otherAttributesCount; }
		}

		public int ElementNamesCount
		{
			get { return _elementNamesCount; }
		}

		public StyleSelector(string selector)
		{
			_simpleSelectors = new List<SimpleSelector>();

			foreach (string simpleSelectorString in selector.Split(' '))
			{
				SimpleSelector simpleSelector = new SimpleSelector(simpleSelectorString);
				_simpleSelectors.Add(simpleSelector);

				_idAttributesCount += simpleSelector.IdAttributesCount;
				_otherAttributesCount += simpleSelector.OtherAttributesCount;
				_elementNamesCount += simpleSelector.ElementNamesCount;
			}
		}

		public bool Matches(Element element)
		{
			bool ret = false;

			Element currentElement = element;
			for (int i = _simpleSelectors.Count - 1; i >= 0; )
			{
				SimpleSelector currentSelector = _simpleSelectors[i];
				ret = currentSelector.Matches(currentElement);
				if (ret)
				{
					currentElement = currentElement.Parent;
					i--;
				}
				else
				{
					if (i == _simpleSelectors.Count - 1 || currentElement.Parent == null)
						break;
					else
						currentElement = currentElement.Parent;
				}
			}

			return ret;
		}
	}
}
