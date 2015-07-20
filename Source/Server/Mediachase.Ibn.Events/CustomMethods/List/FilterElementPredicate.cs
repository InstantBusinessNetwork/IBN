using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;
using System.Collections;

namespace Mediachase.Ibn.Events.CustomMethods.List
{

	internal delegate TResult Func<T, TResult>(T arg);

	/// <summary>
	/// Представляет оббертку над предикатом сравнения значения value FilterElement - a произвольного типа
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class FilterElementPredicate<T> where T: IComparable<T>
	{

		private List<FilterElementPredicate<T>> _childPredicates = new List<FilterElementPredicate<T>>();
		private Func<T, bool> _predicate = null;
		private FilterElement _innerFilterElement = null;

		protected List<FilterElementPredicate<T>> ChildPredicates
		{
			get
			{
				return _childPredicates;
			}
		}
		protected FilterElementPredicate()
		{
		}

		public FilterElement InnerFilterEl
		{
			get
			{
				return _innerFilterElement;
			}
		}

		public FilterElementPredicate(FilterElement filterEl, Func<T, bool> predicate)
		{
			_innerFilterElement = filterEl;
			_predicate = predicate;
		}

		public virtual bool Evaluate(T obj)
		{
			if (_predicate == null)
				throw new NullReferenceException("predicate not defined");

			return _predicate(obj);
		}

		public FilterElementPredicate<T> AppendChildPredicate(FilterElement filterEl)
		{
			FilterElementPredicateFactory<T> factory = new FilterElementPredicateFactory<T>();
			FilterElementPredicate<T> childPredicate = factory.Create<FilterElementPredicate<T>>(filterEl);
			if (childPredicate != null)
			{
				ChildPredicates.Add(childPredicate);
			}

			return childPredicate;
		}
	}
}
