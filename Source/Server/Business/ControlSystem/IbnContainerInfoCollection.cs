using System;
using System.Collections;
using System.Collections.Specialized;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Summary description for IbnContainerInfoCollection.
	/// </summary>
	public class IbnContainerInfoCollection: DictionaryBase
	{
		public IbnContainerInfoCollection()
		{
		}

		public void Add(IbnContainerInfo item)
		{
			this.InnerHashtable.Add(item.Name,item);
		}

		public IbnContainerInfo this[string name]
		{
			get
			{
				return (IbnContainerInfo)this.InnerHashtable[name];
			}
		}
	}
}
