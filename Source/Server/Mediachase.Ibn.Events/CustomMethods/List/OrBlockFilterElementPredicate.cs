using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Events.CustomMethods.List
{
	/// <summary>
	/// Представляет коллекцию предикатов объединенных условием OR
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class OrBlockFilterElementPredicate<T> : FilterElementPredicate<T> where T : IComparable<T>
	{
		public OrBlockFilterElementPredicate()
		{
		}

		public override bool Evaluate(T obj)
		{
			bool retVal = false;

			foreach (FilterElementPredicate<T> childPredicate in ChildPredicates)
			{
				if (childPredicate.Evaluate(obj))
				{
					retVal = true;
					break;
				}
			}

			return retVal;
		}


	}
}
