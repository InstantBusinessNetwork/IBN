using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Events.McCalendar.Common;
using Mediachase.Ibn.Data;
using System.Globalization;

namespace Mediachase.Ibn.Events.CustomMethods.List
{
	public class FilterElementPredicateFactory<T> : AbstractFactory, IFactoryMethod<FilterElementPredicate<T>> where T : IComparable<T>
	{
		#region IFactoryMethod<Func<T1,T2,TResult>> Members

		FilterElementPredicate<T> IFactoryMethod<FilterElementPredicate<T>>.Create(object obj)
		{
			FilterElementPredicate<T> retVal = null;
			FilterElement filterEl = obj as FilterElement;
			if (filterEl != null)
			{
				if (filterEl.Type == FilterElementType.AndBlock)
				{
					retVal = new AndBlockFilterElementPredicate<T>();
					foreach (FilterElement childFilterEl in filterEl.ChildElements)
					{
						retVal.AppendChildPredicate(childFilterEl);
					}
				}
				if (filterEl.Type == FilterElementType.OrBlock)
				{
					retVal = new OrBlockFilterElementPredicate<T>();
					foreach (FilterElement childFilterEl in filterEl.ChildElements)
					{
						retVal.AppendChildPredicate(childFilterEl);
					}
				}
				else
				{
					IComparable<T> filterValue = TryConvertFilterValue(filterEl.Value) as IComparable<T>;
					if (filterValue != null)
					{
						Func<T, bool> functor = null;
						switch (filterEl.Type)
						{
							case FilterElementType.Equal:
								functor = delegate(T obj1) { return filterValue.CompareTo(obj1) == 0; };
								break;
							case FilterElementType.NotEqual:
								functor = delegate(T obj1) { return filterValue.CompareTo(obj1) != 0; };
								break;
							case FilterElementType.Less:
								functor = delegate(T obj1) { return filterValue.CompareTo(obj1) > 0; };
								break;
							case FilterElementType.LessOrEqual:
								functor = delegate(T obj1) { return filterValue.CompareTo(obj1) >= 0; };
								break;
							case FilterElementType.Greater:
								functor = delegate(T obj1) { return filterValue.CompareTo(obj1) < 0; };
								break;
							case FilterElementType.GreaterOrEqual:
								functor = delegate(T obj1) { return filterValue.CompareTo(obj1) <= 0; };
								break;
						}
						if (functor != null)
						{
							retVal = new FilterElementPredicate<T>(filterEl, functor);
						}
					}
				}
			}

			return retVal;
		}

		private object TryConvertFilterValue(object filterValue)
		{
			object retVal = filterValue;
			if (filterValue.GetType() != typeof(T))
			{
				
				IConvertible convertibleValue = filterValue as IConvertible;
				if (convertibleValue != null)
				{
					switch (convertibleValue.GetTypeCode())
					{
						case TypeCode.DateTime:
							retVal = Convert.ToDateTime(convertibleValue);
							break;
						case TypeCode.Int32:
							retVal = Convert.ToInt32(convertibleValue);
							break;
						//TODO: Impl other types
					}
				}
				else
				{
					throw new FormatException("invalid object typ or undefinede");
				}
			}

			return retVal;
		}

		#endregion

	}

}
