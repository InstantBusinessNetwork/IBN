using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Synchronization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Mediachase.Ibn.Data;

namespace Mediachase.Sync.EntityObjectProvider.Sql
{
	/// <summary>
	/// Представляет метаданные реплики синхронизации
	/// </summary>
	public class ReplicaMetadataAdaptor
	{
		private SynchronizationReplicaRow _replicaRow;
		private SyncKnowledge _currentKnowledge;
		private ForgottenKnowledge _forgottenKnowledge;
		private ReplicaKeyMap _replicaKeyMap;

		private object _lockObject = new object();

		private ReplicaMetadataAdaptor(SynchronizationReplicaRow replicaRow)
		{
			_replicaRow = replicaRow;
		}


		/// <summary>
		/// Открывает хранилище метаданных реплики по ее replicaId.
		/// </summary>
		/// <param name="replicaId">The replica id.</param>
		/// <returns></returns>
		public static ReplicaMetadataAdaptor CreateInstance(SyncId replicaId)
		{
			ReplicaMetadataAdaptor retVal = null;
			SynchronizationReplicaRow row = null;
			row = new SynchronizationReplicaRow(replicaId.GetGuidId());
			retVal = new ReplicaMetadataAdaptor(row);

			return retVal;
		}

		/// <summary>
		/// Создает новое хранилище метаданных реплики
		/// </summary>
		/// <param name="replicaId">The replica id.</param>
		/// <param name="idFormats">The id formats.</param>
		/// <returns></returns>
		public static ReplicaMetadataAdaptor CreateInstance(SyncId replicaId, SyncIdFormatGroup idFormats)
		{
			SynchronizationReplicaRow row = new SynchronizationReplicaRow();
			row.TickCount = 1;
			ReplicaMetadataAdaptor retVal = new ReplicaMetadataAdaptor(row);
			retVal._replicaRow.PrimaryKeyId = replicaId.GetGuidId();
			retVal.ReplicaKeyMap = new ReplicaKeyMap(idFormats, replicaId);
			retVal.CurrentKnowledge = new SyncKnowledge(idFormats, retVal.ReplicaKeyMap, (ulong)retVal.TickCount);
			retVal.ForgottenKnowledge = new ForgottenKnowledge(idFormats, retVal.CurrentKnowledge);
			return retVal;
			
		}


		/// <summary>
		/// Открывает хранилище метаданных по principalId
		/// </summary>
		/// <param name="pricipalId">The pricipal id.</param>
		/// <returns></returns>
		public static ReplicaMetadataAdaptor CreateInstance(int pricipalId)
		{
			if (pricipalId == -1)
			{
				throw new ArgumentException("principalId");
			}
			ReplicaMetadataAdaptor retVal = null;
			FilterElement filterEl = new FilterElement(SynchronizationReplicaRow.ColumnPrincipalId, FilterElementType.Equal, pricipalId);
			foreach (SynchronizationReplicaRow replicaRow in SynchronizationReplicaRow.List(filterEl))
			{
				retVal = new ReplicaMetadataAdaptor(replicaRow);
				break;
			}

			return retVal;
		}
		//public static EntityReplicaMetadata CreateInstance(int PrincipalId)
		//{
		//    EntityReplicaMetadata retVal = null;
		//    FilterElement filterEl = new FilterElement(SynchronizationReplicaRow.ColumnPrincipalId,
		//                                               FilterElementType.Equal, PrincipalId);
		//    foreach (SynchronizationReplicaRow row in SynchronizationReplicaRow.List(filterEl))
		//    {
		//        retVal = new EntityReplicaMetadata(row);
		//        break;
		//    }

		//    return retVal;
			
		//}
		private void Save(bool tickCountOnly)
		{
			if (!tickCountOnly)
			{
				if (_replicaRow == null)
				{
					throw new Exception("replica not initialized");
				}

				if (_currentKnowledge != null)
				{
					_replicaRow.CurrentKnowledge = SerializeObject<SyncKnowledge>(_currentKnowledge);
				}

				if (_forgottenKnowledge != null)
				{
					_replicaRow.ForgottenKnowledge = SerializeObject<ForgottenKnowledge>(_forgottenKnowledge);
				}

				if (_replicaKeyMap != null)
				{
					_replicaRow.ReplicaKeyMap = SerializeObject<ReplicaKeyMap>(_replicaKeyMap);
				}
			}

			_replicaRow.Update();

		}
		public void Save()
		{
			Save(false);
		}

		public SyncId ReplicaId
		{
			get { return new SyncId(_replicaRow.ReplicaId); }
			set { _replicaRow.ReplicaId = value.GetGuidId(); }
		}

		public uint TickCount
		{
			get
			{
				return (uint)_replicaRow.TickCount;
			}
		}

		/// <summary>
		/// Атомарно увеличивает счетчик
		/// </summary>
		/// <returns></returns>
		public uint GetNextTickCount()
		{
			lock (_lockObject)
			{
				_replicaRow.TickCount += 1;
				//Update only tick count
				Save(true);
			}
			return (uint)_replicaRow.TickCount;
		}
		

		public SyncKnowledge CurrentKnowledge 
		{
			get
			{
				if (_currentKnowledge == null)
				{
					_currentKnowledge = DeserializeObject<SyncKnowledge>(_replicaRow.CurrentKnowledge);
					if (_currentKnowledge.ReplicaId != ReplicaId)
					{
						throw new Exception("Replica id of loaded knowledge doesn't match replica id ");
					}
				}
				return _currentKnowledge;
			}
			set 
			{
				_currentKnowledge = value;
			}
		}

		public ForgottenKnowledge ForgottenKnowledge 
		{
			get 
			{
				if (_forgottenKnowledge == null)
				{
					_forgottenKnowledge =  DeserializeObject<ForgottenKnowledge>(_replicaRow.ForgottenKnowledge);
					if (_forgottenKnowledge.ReplicaId != ReplicaId)
					{
						throw new Exception("Replica id of loaded knowledge doesn't match replica id ");
					}
				}
				return _forgottenKnowledge;
			}

			set 
			{
				_forgottenKnowledge = value; 
			}
		}

		public ReplicaKeyMap ReplicaKeyMap 
		{
			get 
			{
				if (_replicaKeyMap == null)
				{
					_replicaKeyMap = DeserializeObject<ReplicaKeyMap>(_replicaRow.ReplicaKeyMap);
				}
				return _replicaKeyMap;
			}
			set 
			{ 
				_replicaKeyMap = value; 
			} 
		}

		public DateTime? LastDeletedItemsCleanupTime
		{
			get { return _replicaRow.LastDeletedItemsCleanupTime; }
			set { _replicaRow.LastDeletedItemsCleanupTime = value; }
		}

		public int? PrincipalId
		{
			get { return _replicaRow.PrincipalId; }
			set { _replicaRow.PrincipalId = value; }
		}

		public IEnumerable<ItemMetadataAdaptor> EntityMetadataItems
		{
			get
			{
				return ItemMetadataAdaptor.GetAllMetadataItems(ReplicaId);
			}
		}

		public ItemMetadataAdaptor FindItemMetadataById(SyncId globalId)
		{
			return ItemMetadataAdaptor.FindMetadataItemById(ReplicaId, globalId);
		}

		private static string SerializeObject<T>(T obj)
		{
			MemoryStream memStream = new MemoryStream();
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(memStream, obj);

			return Binary2String(memStream.ToArray());
		}

		private static T DeserializeObject<T>(string strData)
		{
			if (String.IsNullOrEmpty(strData))
			{
				throw new ArgumentException("string data");
			}

			MemoryStream memStream = new MemoryStream(String2Binary(strData));
			BinaryFormatter bf = new BinaryFormatter();
			
            T retVal = (T) bf.Deserialize(memStream);

			return retVal;
		}

		private static byte[] String2Binary(string strData)
		{
			return Convert.FromBase64String(strData);
		}

		private static string Binary2String(byte[] data)
		{
			return Convert.ToBase64String(data, Base64FormattingOptions.None);
		}
	}
}
