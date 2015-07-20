using System;
using System.Collections;

namespace Mediachase.SQLQueryCreator
{
	/// <summary>
	/// Summary description for FilterCollection.
	/// </summary>
	public class FilterCollection: CollectionBase
	{
		public FilterCollection()
		{
		}

		public void Add(FilterCondition condition)
		{
			this.List.Add(condition);
		}
	}
}
