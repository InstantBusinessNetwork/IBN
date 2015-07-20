using System;
using System.Collections;

namespace Mediachase.SQLQueryCreator
{
	/// <summary>
	/// Summary description for FieldCollection.
	/// </summary>
	public class FieldCollection: CollectionBase 	
	{
		public FieldCollection()
		{
		}

		public void Add(QField field)
		{
			//TODO: Add Check Unique field.
			this.List.Add(field);
		}

		public QField this[int Index]
		{
			get
			{
				return (QField)this.List[Index];
			}
		}

		public QField this[string Name]
		{
			get
			{
				foreach(QField field in this.List)
				{
					if(string.Compare(field.Name,Name,true)==0)
						return field;
				}

				return null;
			}
		}

		public int IndexOf(QField	field)
		{
			for(int Index = 0;Index<this.List.Count;Index++)
			{
				if(field==(QField)this.List[Index])
				{
					return Index;
				}
			}
			return -1;
		}
	}
}
