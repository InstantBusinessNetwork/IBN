using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Sync.Core;
using Microsoft.Synchronization;
using OutlookAddin.ClientOutlook.Configuration;

namespace Mediachase.ClientOutlook.Configuration
{
	public class syncAppointmentSetting : syncProviderSetting, ISettingHaveDefaultValues
	{
		#region ISettingHaveDefaultValues Members

		IDictionary<string, object> ISettingHaveDefaultValues.DefaultValues
		{
			get
			{
				Dictionary<string, object> retVal = new Dictionary<string, object>();

				SyncId replicaId = new SyncId(Guid.NewGuid());
				retVal.Add("replicaStoreFileName",
							ApplicationConfig.GenerateReplicaStoreFileName(Outlook.OlItemType.olAppointmentItem));
				retVal.Add("replicaId", replicaId.ToString());
				retVal.Add("syncDirection", (int)SyncDirectionOrder.Download);
				retVal.Add("syncConflictResolution", (int)ConflictResolutionPolicy.DestinationWins);
				retVal.Add("recursive", true);

				return retVal;

			}

		}

		#endregion
	}
}
