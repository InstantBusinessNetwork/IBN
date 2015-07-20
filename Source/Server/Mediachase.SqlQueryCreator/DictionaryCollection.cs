using System;
using System.Collections;

namespace Mediachase.SQLQueryCreator
{
	/// <summary>
	/// Summary description for DictionaryCollection.
	/// </summary>
	public class DictionaryCollection: CollectionBase 	
	{
		public DictionaryCollection()
		{
		}

		public void Add(QDictionary dic)
		{
			//TODO: Add Check Unique dic.
			this.List.Add(dic);
		}

		public QDictionary this[QField	FieldValue]
		{
			get
			{
				foreach(QDictionary dic in this.List)
				{
					if(dic.FieldValue==FieldValue)
						return dic;
				}

				return null;
			}
		}

		public QDictionary this[int Index]
		{
			get
			{
				return (QDictionary)this.List[Index];
			}
		}

		public int IndexOf(QDictionary	dic)
		{
			for(int Index = 0;Index<this.List.Count;Index++)
			{
				if(dic==(QDictionary)this.List[Index])
				{
					return Index;
				}
			}
			return -1;
		}
	}
}
