using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Events.CustomMethods.List
{
	/// <summary>
	/// Представляет коллекцию предикатов объединенных условием AND
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class AndBlockFilterElementPredicate<T> : FilterElementPredicate<T> where T : IComparable<T>
	{
		public AndBlockFilterElementPredicate()
		{
		}

		public override bool Evaluate(T obj)
		{
			bool retVal = true;

			foreach (FilterElementPredicate<T> childPredicate in ChildPredicates)
			{
				if (!childPredicate.Evaluate(obj))
				{
					retVal = false;
					break;
				}
			}

			return retVal;
		}

		
	}
}
