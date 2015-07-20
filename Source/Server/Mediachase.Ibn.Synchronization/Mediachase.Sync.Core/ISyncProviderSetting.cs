using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Synchronization;

namespace Mediachase.Sync.Core
{
	public interface ISyncProviderSetting
	{
		ConflictResolutionPolicy ConflictResolutionPolicySetting { get; }
		SyncDirectionOrder SyncDirectionOrderSetting { get; }
	}
}
