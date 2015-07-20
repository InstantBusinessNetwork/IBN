using System;
using System.Collections;

namespace Mediachase.MetaDataPlus.Configurator
{
	/// <summary>
	/// Summary description for MetaTypeCollection.
	/// </summary>
	public class MetaTypeCollection : ReadOnlyCollectionBase
	{
		internal MetaTypeCollection()
		{
		}

		internal void Add(MetaType newItem)
		{
			this.InnerList.Add(newItem);
		}

		internal void Delete(int mateTypeId)
		{
			foreach (MetaType item in this)
			{
				if (item.Id == mateTypeId)
				{
					this.InnerList.Remove(item);
					break;
				}
			}
		}

		internal void Clear()
		{
			this.InnerList.Clear();
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public MetaType this[int index]
		{
			get
			{
				return (MetaType)this.InnerList[index];
			}
		}

		public MetaType this[string name]
		{
			get
			{
				foreach (MetaType item in this)
				{
					if (string.Compare(item.Name, name, true) == 0)
						return item;
				}
				return null;
			}
		}
	}
}
