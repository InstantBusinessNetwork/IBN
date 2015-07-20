using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Sync.Core
{
	[Serializable]
	public class SyncTransferData
	{	
		public string SyncDataName { get; set; }
		public ulong LastModified { get; set; }
		public string Uri { get; set; }

		public List<SyncTransferData> Childrens { get; private set; }
		public Dictionary<string, object> Properties { get; private set; }

		public SyncTransferData(string dataName)
		{
			SyncDataName = dataName;
			Properties = new Dictionary<string, object>();
			Childrens = new List<SyncTransferData>();
		}

		public T GetPropertyValue<T>(string name, T defaultValue)
		{
			object retVal;
			if (!Properties.TryGetValue(name, out retVal))
			{
				retVal = defaultValue;
			}
			return (T)retVal;
		}

		public override string ToString()
		{
			return SyncDataName;
		}

	}
}
