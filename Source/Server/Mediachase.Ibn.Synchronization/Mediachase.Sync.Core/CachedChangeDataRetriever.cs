using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Synchronization;
using Mediachase.Sync.Core;

namespace Mediachase.Sync.Core
{
	[Serializable]
	public class CachedChangeDataRetriever : IChangeDataRetriever
	{
		private SyncIdFormatGroup _idFormats;
		private Dictionary<SyncId, SyncTransferData> _cachedData;
		public CachedChangeDataRetriever()
		{
		}

		public CachedChangeDataRetriever(
			IChangeDataRetriever changeDataRetriever,
			ChangeBatchBase sourceChanges)
		{
			_idFormats = changeDataRetriever.IdFormats;
			_cachedData = new Dictionary<SyncId, SyncTransferData>();

			// Look at each change in the source batch
			foreach (ItemChange itemChange in sourceChanges)
			{
				if (itemChange.ChangeKind != ChangeKind.Deleted)
				{
					// This is not delete, so there is some data associated
					// with this change.

					// Create a UserLoadChangeContext to retriever this data.
					UserLoadChangeContext loadChangeContext = new UserLoadChangeContext(
						_idFormats,
						itemChange);

					// Retrieve the data (we know that our provider uses data of type ItemData.
					SyncTransferData itemData = changeDataRetriever.LoadChangeData(loadChangeContext) as SyncTransferData;

					// Cache it
					_cachedData.Add(itemChange.ItemId, itemData);
				}
			}
		}

		public SyncIdFormatGroup IdFormats
		{
			get
			{
				return _idFormats;
			}
		}

		public object LoadChangeData(LoadChangeContext loadChangeContext)
		{
			return _cachedData[loadChangeContext.ItemChange.ItemId];
		}
	}
}
