using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Sync.Core;

namespace Mediachase.ClientOutlook.SyncTransferedData
{
	public interface ITransferDataSerializable
	{
		SyncTransferData Serialize();
		object Deserialize(SyncTransferData data);
	}
}
