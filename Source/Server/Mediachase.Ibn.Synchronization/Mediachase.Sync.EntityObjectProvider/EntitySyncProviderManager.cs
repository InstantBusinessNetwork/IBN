using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Sync.Core.Common;
using Microsoft.Synchronization;
using System.Security.Principal;
using Mediachase.Sync.Core.TransferDataType;
using Mediachase.Sync.Core;
using Mediachase.Ibn.Events;

namespace Mediachase.Sync.EntityObjectProvider
{
	public class EntitySyncProviderManager
	{

		private static object _lockObject = new object();
		private static Dictionary<string, GenericRemoteSyncProvider<EntityObjectHierarchy>> _activeSyncPorviders =
														new Dictionary<string, GenericRemoteSyncProvider<EntityObjectHierarchy>>();



		/// <summary>
		/// Gets the provider.
		/// </summary>
		/// <param name="principal">The principal.</param>
		/// <param name="syncProviderType">Type of the sync provider.</param>
		/// <returns></returns>
		public static GenericRemoteSyncProvider<EntityObjectHierarchy> GetProvider(int principalId, eSyncProviderType syncProviderType)
		{
			GenericRemoteSyncProvider<EntityObjectHierarchy> retVal;
			string key = GetUniqueProviderKey(principalId, syncProviderType);
			lock (_lockObject)
			{
				if (!_activeSyncPorviders.TryGetValue(key, out retVal))
				{
					switch (syncProviderType)
					{
						case eSyncProviderType.Appointment:
							retVal = CalendarEventSyncProvider.CreateInstance(principalId);
							break;
					}

					if (retVal != null)
					{
						_activeSyncPorviders.Add(key, retVal);
					}
				}

			}

			return retVal;
		}

		private static string GetUniqueProviderKey(int principalId, eSyncProviderType providerType)
		{
			return principalId.ToString() + providerType.ToString();
		}

	}
}
