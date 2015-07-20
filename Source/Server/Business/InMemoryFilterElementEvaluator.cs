using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Ibn.Data.Meta.Management;
using System.Text.RegularExpressions;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Represents in memory filter element evaluator.
	/// </summary>
	public static class InMemoryFilterElementEvaluator
	{
		#region Eval
		/// <summary>
		/// Evals the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="elements">The elements.</param>
		/// <returns></returns>
		public static bool Eval(EntityObject source, FilterElement[] elements)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (elements == null)
				return true;

			foreach (FilterElement element in elements)
			{
				if (!Eval(source, element))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Evals the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="element">The element.</param>
		/// <returns></returns>
		public static bool Eval(EntityObject source, FilterElement element)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (element == null)
				throw new ArgumentNullException("element");

			switch (element.Type)
			{
				case FilterElementType.Abstract:
					throw new NotSupportedException();

				case FilterElementType.Exists:
					throw new NotSupportedException();
				case FilterElementType.NotExists:
					throw new NotSupportedException();
				case FilterElementType.In:
					throw new NotSupportedException();
				case FilterElementType.NotIn:
					throw new NotSupportedException();

				case FilterElementType.AndBlock:
					{
						foreach (FilterElement childElement in element.ChildElements)
						{
							if (!Eval(source, childElement))
								return false;
						}
						return true;
					}
				case FilterElementType.OrBlock:
					{
						foreach (FilterElement childElement in element.ChildElements)
						{
							if (Eval(source, childElement))
								return true;
						}
						return false;
					}
				case FilterElementType.Between:
					{
						object propertyValue = GetPropertyValue(source, element.Source);

						object start = element.ChildElements[0].Value;
						object end = element.ChildElements[1].Value;

						int result1 = Compare(start, propertyValue);
						int result2 = Compare(propertyValue, end);

						return result1 <= 0 && result2<= 0;
					}
				case FilterElementType.Contains:
					{
						string propertyValue = (string)GetPropertyValue(source, element.Source);
						string mask = (string)element.Value;

						if (propertyValue == null || mask==null)
							return false;

						return propertyValue.IndexOf(mask, StringComparison.OrdinalIgnoreCase)!=-1;
					}
				case FilterElementType.NotContains:
					{
						string propertyValue = (string)GetPropertyValue(source, element.Source);
						string mask = (string)element.Value;

						if (propertyValue == null || mask == null)
							return false;

						return !(propertyValue.IndexOf(mask, StringComparison.OrdinalIgnoreCase) != -1);
					}
				case FilterElementType.Custom:
					throw new NotSupportedException();
				case FilterElementType.EndsWith:
					{
						string propertyValue = (string)GetPropertyValue(source, element.Source);
						string mask = (string)element.Value;

						if (propertyValue == null || mask == null)
							return false;

						return propertyValue.EndsWith(mask, StringComparison.OrdinalIgnoreCase);
					}
				case FilterElementType.NotEndsWith:
					{
						string propertyValue = (string)GetPropertyValue(source, element.Source);
						string mask = (string)element.Value;

						if (propertyValue == null || mask == null)
							return false;

						return !propertyValue.EndsWith(mask, StringComparison.OrdinalIgnoreCase);
					}
				case FilterElementType.StartsWith:
					{
						string propertyValue = (string)GetPropertyValue(source, element.Source);
						string mask = (string)element.Value;

						if (propertyValue == null || mask == null)
							return false;

						return propertyValue.StartsWith(mask, StringComparison.OrdinalIgnoreCase);
					}
				case FilterElementType.NotStartsWith:
					{
						string propertyValue = (string)GetPropertyValue(source, element.Source);
						string mask = (string)element.Value;

						if (propertyValue == null || mask == null)
							return false;

						return !propertyValue.StartsWith(mask, StringComparison.OrdinalIgnoreCase);
					}

				case FilterElementType.Equal:
					{
						object propertyValue = GetPropertyValue(source, element.Source);
						object srcValue = element.Value;

						return Compare(propertyValue, srcValue) == 0;
					}
				case FilterElementType.NotEqual:
					{
						object propertyValue = GetPropertyValue(source, element.Source);
						object srcValue = element.Value;

						return Compare(propertyValue, srcValue) != 0;
					}
				case FilterElementType.Greater:
					{
						object propertyValue = GetPropertyValue(source, element.Source);
						object srcValue = element.Value;

						return Compare(propertyValue, srcValue) > 0;
					}
				case FilterElementType.GreaterOrEqual:
					{
						object propertyValue = GetPropertyValue(source, element.Source);
						object srcValue = element.Value;

						return Compare(propertyValue, srcValue) >= 0;
					}
				case FilterElementType.IsNotNull:
					{
						object propertyValue = GetPropertyValue(source, element.Source);
						return propertyValue!=null;
					}
				case FilterElementType.IsNull:
					{
						object propertyValue = GetPropertyValue(source, element.Source);
						return propertyValue == null;
					}
				case FilterElementType.Less:
					{
						object propertyValue = GetPropertyValue(source, element.Source);
						object srcValue = element.Value;

						return Compare(propertyValue, srcValue) < 0;
					}
				case FilterElementType.LessOrEqual:
					{
						object propertyValue = GetPropertyValue(source, element.Source);
						object srcValue = element.Value;

						return Compare(propertyValue, srcValue) <= 0;
					}
				case FilterElementType.Like:
					{
						string propertyValue = (string)GetPropertyValue(source, element.Source);
						string mask = (string)element.Value;

						if (propertyValue == null || mask == null)
							return false;

						// TODO: Support % Only
						mask = mask.Replace('%', '*');

						return WildcardUtil.PatternMatch(propertyValue, mask);
					}
				case FilterElementType.NotLike:
					{
						string propertyValue = (string)GetPropertyValue(source, element.Source);
						string mask = (string)element.Value;

						if (propertyValue == null || mask == null)
							return false;

						// TODO: Support % Only
						mask = mask.Replace('%', '*');

						return !WildcardUtil.PatternMatch(propertyValue, mask);
					}
			}

			throw new NotSupportedException();
		}

		/// <summary>
		/// Compares the specified x and y. Value Less than zero X instance is less than Y. (X &lt; Y)
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns>	A 32-bit signed integer that indicates the relative order of the objects
		/// being compared. The return value has these meanings: 
		/// Value Meaning Less than zero X instance is less than Y. (X &lt; Y)
		/// Zero X is equal to Y. (X == Y)
		/// Greater than zero X is greater than Y. (X &gt; Y) </returns> 
		private static int Compare(object x, object y)
		{
			if (x == null && x == y)
				return 0;

			if (x == null)
				return -1;
			if (y == null)
				return 1;

			if (x is string && y is string)
			{
				return string.Compare((string)x, (string)y, StringComparison.OrdinalIgnoreCase);
			}

			return ((IComparable)x).CompareTo(y);
		}

		/// <summary>
		/// Gets the property value.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		private static object GetPropertyValue(EntityObject obj, string source)
		{
			if (source.IndexOf('[') != -1)
			{
				StringBuilder retVal = new StringBuilder();

				// Process Complex Filter
				foreach(Match match in Regex.Matches(source, @"\[(?<source>[^\]]+)\]"))
				{
					object objValue = GetPropertyValue(obj, match.Groups["source"].Value);
					if (objValue != null)
					{
						if (objValue is DateTime)
						{
							retVal.Append(objValue);
							retVal.AppendFormat(@" {0:MM\/dd\/yyyy}", (DateTime)objValue);
							retVal.AppendFormat(@" {0:dd.MM.yyyy}", (DateTime)objValue);
						}
						else
						{
							retVal.Append(objValue);
						}

						retVal.Append(' ');

					}
				}

				return retVal.ToString();
			}

			return obj[source];
		}
		#endregion
	}
}
