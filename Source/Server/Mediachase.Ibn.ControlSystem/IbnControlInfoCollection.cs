using System;
using System.Collections;
using System.Collections.Specialized;

namespace Mediachase.Ibn.ControlSystem
{
	/// <summary>
	/// Summary description for IbnControlInfoCollection.
	/// </summary>
	public class IbnControlInfoCollection: DictionaryBase
	{
		public IbnControlInfoCollection()
		{
		}

		public void Add(IbnControlInfo item)
		{
			this.InnerHashtable.Add(item.Name,item);
		}

		public IbnControlInfo this[string name]
		{
			get
			{
				return (IbnControlInfo)this.InnerHashtable[name];
			}
		}
	}
}
