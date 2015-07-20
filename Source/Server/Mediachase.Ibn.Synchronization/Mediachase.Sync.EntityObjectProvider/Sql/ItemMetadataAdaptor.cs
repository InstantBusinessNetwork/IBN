using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Synchronization.MetadataStorage;
using Microsoft.Synchronization;
using Mediachase.Ibn.Data;

namespace Mediachase.Sync.EntityObjectProvider.Sql
{
	public class ItemMetadataAdaptor : ItemMetadata
	{
		private SynchronizationMetadataRow _metadataRow;

		private ItemMetadataAdaptor(SynchronizationMetadataRow row)
		{
			_metadataRow = row;
		}

		public void Save()
		{
			_metadataRow.Update();
		}

		public void Delete()
		{
			_metadataRow.Delete();
		}

		public static ItemMetadataAdaptor CreateInstance(SyncId globalId, SyncId replicaId)
		{
			SynchronizationMetadataRow row = new SynchronizationMetadataRow();
			ItemMetadataAdaptor retVal = new ItemMetadataAdaptor(row);
			row.Id = Guid.NewGuid();
			row.UniqueId = globalId.GetSyncGlobalId().UniqueId;
			row.Prefix = (long)globalId.GetSyncGlobalId().Prefix;
			row.ReplicaId = replicaId.GetGuidId();
			return retVal;
		}
		public static ItemMetadataAdaptor CreateInstance(ItemMetadata itemMetadata)
		{
			ItemMetadataAdaptor retVal = itemMetadata as ItemMetadataAdaptor;
			
			//Создаем новый объект и копируем туда свойства
			if (retVal == null)
			{
				retVal = new ItemMetadataAdaptor(new SynchronizationMetadataRow());

				SyncGlobalId syncGlobalId = itemMetadata.GlobalId.GetSyncGlobalId();
				retVal._metadataRow.UniqueId = syncGlobalId.UniqueId;
				retVal._metadataRow.Prefix = Convert.ToInt64(syncGlobalId.Prefix);
				retVal._metadataRow.IsDeleted = itemMetadata.IsDeleted;

				retVal.ChangeVersion = itemMetadata.ChangeVersion;
				retVal.CreationVersion = itemMetadata.CreationVersion;
			}

			return retVal;
		}

		/// <summary>
		/// Parses the sync version  to SyncVersion object example "1.2" "1212.321".
		/// </summary>
		/// <param name="version">The version.</param>
		/// <returns></returns>
		private static SyncVersion String2SyncVersion(string version)
		{
			SyncVersion retVal = SyncVersion.UnknownVersion;
			if (!string.IsNullOrEmpty(version))
			{
				string []parts = version.Split('.');
				try
				{
					retVal = new SyncVersion(Convert.ToUInt32(parts[0]), Convert.ToUInt64(parts[1]));
				}
				catch
				{
					throw new ArgumentException("invalid version");
				}
			}
			return retVal;
		}

		/// <summary>
		/// SyncVersion to string.
		/// </summary>
		/// <param name="version">The version.</param>
		/// <returns></returns>
		private static string SyncVersion2String(SyncVersion version)
		{
			string retVal = null;
			if (version != null)
			{
				retVal = version.ReplicaKey.ToString() + "." + version.TickCount;
			}

			return retVal;
		}

		public Guid? Uri
		{
			get
			{
				Guid? retVal = null;
				if (_metadataRow != null)
				{
					retVal = _metadataRow.Uri;
				}
				return retVal;
			}
			set
			{
				_metadataRow.Uri = value;
			}
		}

		public long? Timestamp
		{
			get
			{
				return _metadataRow.Timestamp;
			}

			set
			{
				_metadataRow.Timestamp = value;
			}
		}

		public static IEnumerable<ItemMetadataAdaptor> GetAllMetadataItems(SyncId replicaId)
		{

			FilterElement filterEl = new FilterElement(SynchronizationMetadataRow.ColumnReplicaId, FilterElementType.Equal,
													   replicaId.GetGuidId());

			return SynchronizationMetadataRow.List(filterEl).Select(x => new ItemMetadataAdaptor(x));
		}

		public static ItemMetadataAdaptor FindMetadataItemById(SyncId replicaId, SyncId globalId)
		{
			ItemMetadataAdaptor retVal = null;
			FilterElement filterEl = new AndBlockFilterElement();
			filterEl.ChildElements.Add(new FilterElement(SynchronizationMetadataRow.ColumnReplicaId, FilterElementType.Equal,
														 replicaId.GetGuidId()));
			filterEl.ChildElements.Add(new FilterElement(SynchronizationMetadataRow.ColumnUniqueId, FilterElementType.Equal,
														 globalId.GetSyncGlobalId().UniqueId));
			filterEl.ChildElements.Add(new FilterElement(SynchronizationMetadataRow.ColumnPrefix, FilterElementType.Equal,
														(long)globalId.GetSyncGlobalId().Prefix));

			SynchronizationMetadataRow[] itemRows = SynchronizationMetadataRow.List(filterEl);
			if (itemRows.Length != 0)
			{
				retVal = new ItemMetadataAdaptor(itemRows[0]);
			}

			return retVal;
		}

		public static IEnumerable<ItemMetadata> FindMetadataItemByCustomField(string customField, object value)
		{
			Type valueType = value.GetType();

			FilterElement filterEl = new FilterElement(customField, FilterElementType.Equal, value);

			foreach (SynchronizationMetadataRow itemRow in SynchronizationMetadataRow.List(filterEl))
			{
				yield return new ItemMetadataAdaptor(itemRow);
			}

		}
		#region ItemMetadata
		public override Microsoft.Synchronization.SyncVersion ChangeVersion
		{
			get
			{
				SyncVersion retVal = null;
				if (_metadataRow != null)
				{
					retVal = String2SyncVersion(_metadataRow.CurrentVersion);
				}
				return retVal;
			}
			set
			{
				_metadataRow.CurrentVersion = SyncVersion2String(value);
			}
		}

		public override Microsoft.Synchronization.SyncVersion CreationVersion
		{
			get
			{
				SyncVersion retVal = null;
				if (_metadataRow != null)
				{
					retVal = String2SyncVersion(_metadataRow.CreationVersion);
				}
				return retVal;
			}
			set
			{
				_metadataRow.CreationVersion = SyncVersion2String(value);
			}
		}


		public override Microsoft.Synchronization.SyncId GlobalId
		{
			get
			{
				SyncId retVal = null;
				if (_metadataRow != null)
				{
					SyncGlobalId globalId = new SyncGlobalId(Convert.ToUInt64(_metadataRow.Prefix), _metadataRow.UniqueId);
					retVal = new SyncId(globalId);
				}

				return retVal;
			}
		}

		public override bool IsDeleted
		{
			get { return _metadataRow.IsDeleted; }
		}

		public override void MarkAsDeleted(Microsoft.Synchronization.SyncVersion changeVersion)
		{
			_metadataRow.IsDeleted = true;
			this.ChangeVersion = changeVersion;
		}

		public override void ResurrectDeletedItem()
		{
			_metadataRow.IsDeleted = false;
		}

		public override void SetCustomField(string fieldName, Guid value)
		{
			SetCustomField<Guid>(fieldName, value);
		}

		public override void SetCustomField(string fieldName, ulong value)
		{
			SetCustomField<long>(fieldName, (long)value);
		}

		private void SetCustomField<T>(string fieldName, T value)
		{
			switch (fieldName)
			{
				case SynchronizationMetadataRow.ColumnUri:
					this.Uri = value as Guid?;
					break;
				case SynchronizationMetadataRow.ColumnTimestamp:
					this.Timestamp = value as long?;
					break;
			}
		}

		private T GetCustomFiels<T>(string fieldName, T defaultValue)
		{
			T retVal = defaultValue;
			switch (fieldName)
			{
				case SynchronizationMetadataRow.ColumnUri:
					retVal = (T)(object)_metadataRow.Uri;
					break;
				case SynchronizationMetadataRow.ColumnTimestamp:
					retVal = (T)(object)_metadataRow.Timestamp;
					break;
			}

			return retVal;
		}

		public override Guid? GetGuidField(string fieldName)
		{
			return GetCustomFiels<Guid?>(fieldName, null);
		}

		public override ulong? GetUInt64Field(string fieldName)
		{
			return GetCustomFiels<ulong?>(fieldName, null);
		} 

		#region NotImplement
		public override void SetChangeUnitVersion(Microsoft.Synchronization.SyncId changeUnitId, Microsoft.Synchronization.SyncVersion changeUnitVersion)
		{
			throw new NotImplementedException();
		}

		

		

		public override void SetCustomField(string fieldName, uint value)
		{
			throw new NotImplementedException();
		}

		public override void SetCustomField(string fieldName, ushort value)
		{
			throw new NotImplementedException();
		}

		public override void SetCustomField(string fieldName, byte value)
		{
			throw new NotImplementedException();
		}

		public override void SetCustomField(string fieldName, string value)
		{
			throw new NotImplementedException();
		}

		public override void SetCustomField(string fieldName, byte[] value)
		{
			throw new NotImplementedException();
		}

		public override byte? GetByteField(string fieldName)
		{
			throw new NotImplementedException();
		}

		public override byte[] GetBytesField(string fieldName)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<ChangeUnitMetadata> GetChangeUnitEnumerator()
		{
			throw new NotImplementedException();
		}

		public override Microsoft.Synchronization.SyncVersion GetChangeUnitVersion(Microsoft.Synchronization.SyncId changeUnitId)
		{
			throw new NotImplementedException();
		}

		public override string GetStringField(string fieldName)
		{
			throw new NotImplementedException();
		}

		public override ushort? GetUInt16Field(string fieldName)
		{
			throw new NotImplementedException();
		}

		public override uint? GetUInt32Field(string fieldName)
		{
			throw new NotImplementedException();
		}

		
		#endregion

		#endregion



		public override SyncId GetMergeWinnerId()
		{
			throw new NotImplementedException();
		}

		public override void SetMergeWinnerIdOnDeletedItem(SyncId winnerId)
		{
			throw new NotImplementedException();
		}
	}
}
