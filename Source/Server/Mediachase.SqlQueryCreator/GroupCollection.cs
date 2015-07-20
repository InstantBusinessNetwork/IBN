using System;
using System.Collections;

namespace Mediachase.SQLQueryCreator
{
	/// <summary>
	/// Summary description for GroupCollection.
	/// </summary>
	public class GroupCollection: CollectionBase
	{
		public GroupCollection()
		{
		}

		public void Add(QField field)
		{
			//TODO: Add Check Unique field.
			this.List.Add(field);
		}
	}
}
