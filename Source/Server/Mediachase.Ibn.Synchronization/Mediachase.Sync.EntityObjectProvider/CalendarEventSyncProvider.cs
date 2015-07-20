using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Sync.Core;
using Microsoft.Synchronization;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Events;
using Mediachase.Ibn.Events.CustomMethods.ChangeTracking;
using Mediachase.Ibn.Events.CustomMethods.UpdateResources;
using Microsoft.Synchronization.MetadataStorage;
using Mediachase.Sync.EntityObjectProvider.Sql;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Events.McCalendar.Components;
using Mediachase.Ibn.Events.CustomMethods;
using Mediachase.Sync.Core.TransferDataType;
using System.Security.Principal;

namespace Mediachase.Sync.EntityObjectProvider
{
	/// <summary>
	/// Предоставляет доступ к елементам событий во время синхронизации, и отслеживает изменения событий с последующей
	/// регистрацией информации о харакетере изменения 
	/// </summary>
	public class CalendarEventSyncProvider : GenericRemoteSyncProvider<EntityObjectHierarchy>
	{
		
		private static Object _lock = new Object();
		private static List<SyncId> _syncPool = new List<SyncId>();

		SyncId _replicaId = null;
	
		private CalendarEventSyncProvider(SyncId replicaId)
		{
			_replicaId = replicaId;
		}

		/// <summary>
		/// Возвращает признак того что указанная реплика находится в стадии синхронизации
		/// </summary>
		/// <param name="replicaId">The replica id.</param>
		/// <returns></returns>
		public static bool SyncSessionInProcess(SyncId replicaId)
		{
			return _syncPool.Contains(replicaId);
		}

		/// <summary>
		/// Создает экземпляр провайдера для заданного пользователя
		/// </summary>
		/// <param name="principalId">The principal id.</param>
		/// <returns></returns>
		public static CalendarEventSyncProvider CreateInstance(int principalId)
		{
			CalendarEventSyncProvider retVal = null;
			lock (_lock)
			{
				SyncId replicaId = SyncMetadataStore.FindReplicaIdByPrincipalId(principalId);
				if (replicaId == null)
				{
					//генерируем новый replica id
					replicaId = new SyncId(Guid.NewGuid());
					//Создаем новое хранилище метаданных реплики
					SyncMetadataStore.CreateStore(replicaId, principalId);
				}

				retVal = new CalendarEventSyncProvider(replicaId);
				retVal.InitializeMetaDataStore();
			}
			return retVal;
		}

		public SyncId ReplicaId
		{
			get
			{
				return _replicaId;
			}
		}

		public ReplicaMetadata ReplicaMetadata
		{
			get
			{
				return _metaData;
			}
		}

		public override Microsoft.Synchronization.SyncIdFormatGroup IdFormats
		{

			get
			{
				return SyncMetadataStore.StaticIdFormats;
			}
		}

		protected override void BeginTransaction()
		{
			if (_metaDataStore == null)
			{
				throw new ArgumentNullException("metadata store not initialized");
			}

			SyncMetadataStore metaDataStore = _metaDataStore as SyncMetadataStore;
			metaDataStore.BeginTransaction();
		}

		protected override void CommitTransaction()
		{
			if (_metaDataStore == null)
			{
				throw new ArgumentNullException("metadata store not initialized");
			}

			SyncMetadataStore metaDataStore = _metaDataStore as SyncMetadataStore;
			metaDataStore.CommitTransaction();
		}

		public override void BeginSession(SyncProviderPosition position, SyncSessionContext syncSessionContext)
		{
			
			lock (_lock)
			{
				_syncPool.Add(ReplicaId);
			}

			base.BeginSession(position, syncSessionContext);

		}

		public override void EndSession(SyncSessionContext syncSessionContext)
		{
			lock (_lock)
			{
				_syncPool.Remove(ReplicaId);
			}

			base.EndSession(syncSessionContext);
		}

		protected override void InitializeMetaDataStore()
		{
			if (_metaDataStore == null || _metaData == null)
			{
				// Values for adding a custom field to the metadata store
				List<FieldSchema> fields = new List<FieldSchema>();
				_metaDataStore = SyncMetadataStore.OpenStore(ReplicaId);
				_metaData = _metaDataStore.GetReplicaMetadata(IdFormats, ReplicaId);
			}
		}

		protected override void CloseMetaDataStore()
		{
			if (_metaDataStore != null)
			{
				((SyncMetadataStore)_metaDataStore).Dispose();
			}
			_metaDataStore = null;
		}

		protected override void UpdateMetadataStoreWithLocalChanges()
		{
			//Set RemainingWorkSessionEstimate to all item count in metadata
			//Получем количество всех элементов контекста синхронизации для данной реплики. 
			//это не правильно, нужно получить количество всех элементов которые будут участвовать в данной сесси синхронизации
			//для этого нужно в первом выхове метода GetChangeBatch получить все элементы которые будут участвовать в синхронизации
			//в не зависимости от размера changeBatch, а в последующих вызовах отдавать прциями из кеша. 
			//TODO: Реализовать выше-перчсисленное
			uint dummy;
			_metaData.GetItemCount(out base.RemainingSessionWorkEstimate, out dummy);
		}

		protected override void ReadDataStore()
		{
			//Nothing todo
		}

		protected override void SaveDataStore()
		{
			//Nothing todo
		}

		protected override uint SyncBatchSize
		{
			get { return 50; }
		}

		/// <summary>
		/// Создает событие. Логика регистрация в метаданных происходит в базовом классе
		/// </summary>
		/// <param name="change">The change.</param>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		protected override void CreateDataItem(ItemChange change, ItemMetadata item, EntityObjectHierarchy data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			EntityObject eventEntity = data.InnerEntity;
			//Дата начала события
			DateTime eventDtStart = (DateTime)eventEntity[CalendarEventEntity.FieldStart];
			//В начале создаем событие  без регистрации в мета данных события
			PrimaryKeyId? eventId = eventEntity.PrimaryKeyId = BusinessManager.Create(eventEntity);

			//Устанавливаем отношение родительского элемента с дочерними
			EventHelper.NormailzeReferences(data);
			foreach (EntityObjectHierarchy child in data.Childrens)
			{
				CalendarEventEntity exceptionEvent = child.InnerEntity as CalendarEventEntity;
				if (exceptionEvent != null)
				{
					//Формируем id exception в соответвии принятым форматом
					DateTime recurrenceId = exceptionEvent.RecurrenceId;
					exceptionEvent.PrimaryKeyId = EventHelper.CreateExceptionId(eventId.Value, eventDtStart, recurrenceId);
				}

				//Вызываем создание дочерних элементов
				BusinessManager.Create(child.InnerEntity);

				//создаем дочерние элементы exception
				if (exceptionEvent != null)
				{
					foreach(EntityObjectHierarchy exceptionChild in child.Childrens)
					{
						CalendarEventResourceEntity exceptionResource = exceptionChild.InnerEntity as CalendarEventResourceEntity;
						if (exceptionResource != null)
						{ 
							//устанавливаем связь ресурса с event (exception)
							exceptionResource.EventId = exceptionEvent.PrimaryKeyId.Value;
							BusinessManager.Create(exceptionChild.InnerEntity);
						}
					}
				}
			}

			item.SetCustomField(SyncReplicaMetadata.URI_COLUMNNAME, (Guid)eventId);
		}

		/// <summary>
		/// Обновляет событие новыми значениями.Логика регистрация в метаданных происходит в базовом классе
		/// </summary>
		/// <param name="change">The change.</param>
		/// <param name="item">The item.</param>
		/// <param name="data">The data.</param>
		/// <returns>Набор пользовательских полей для записи в метаданные</returns>
		protected override void UpdateDataItem(ItemChange change, ItemMetadata item, 
																		   EntityObjectHierarchy data)
		{
			//TODO: Заменить удаление + создание на update
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			EntityObject eventEntity = data.InnerEntity;
			//Получаем pk события из метаданных события
			PrimaryKeyId? eventId = eventEntity.PrimaryKeyId = (PrimaryKeyId)item.GetGuidField(SyncReplicaMetadata.URI_COLUMNNAME);

			//Удаляем событие и все связанные с ним элементы
			BusinessManager.Delete(eventEntity);
			//Заново создаем событие и все связанные с ним элементы, id event остается прежним
			CreateDataItem(change, item, data);

			item.SetCustomField(SyncReplicaMetadata.URI_COLUMNNAME, (Guid)eventId);
		}

		/// <summary>
		/// Удаляет событие
		/// </summary>
		/// <param name="change">The change.</param>
		/// <param name="item">The item.</param>
		/// <param name="data">The data.</param>
		/// <returns>признак успешного удаления</returns>
		protected override bool DeleteDataItem(ItemChange change, ItemMetadata item, EntityObjectHierarchy data)
		{
			bool retVal = false;
			PrimaryKeyId? eventId = (PrimaryKeyId)item.GetGuidField(SyncReplicaMetadata.URI_COLUMNNAME);
			if (eventId != null)
			{
				//Вызываем удаление события без регистрации в мета данных события
				BusinessManager.Delete(new CalendarEventEntity(eventId.Value));
				retVal = true;
			}

			return retVal;
		}

		/// <summary>
		/// Возвращает иерархию события по его SyncId в метаданных
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		protected override EntityObjectHierarchy GetDataItem(ItemMetadata item)
		{
			EntityObjectHierarchy retVal = null;

			PrimaryKeyId? eventId = (PrimaryKeyId)item.GetGuidField(SyncReplicaMetadata.URI_COLUMNNAME);
			if (eventId != null)
			{
				retVal = EventHelper.LoadEventEntityHierarchy(eventId.Value);
			}

			return retVal;
		}

		
		/// <summary>
		/// Converts to transfer data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		protected override object ConvertToTransferData(EntityObjectHierarchy data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.InnerEntity.MetaClassName != CalendarEventEntity.ClassName)
			{
				throw new FormatException("event must be parrent element in hierarchy");
			}
			TransferDataConverFactory converFactory = new TransferDataConverFactory();

			SyncTransferData retVal = converFactory.Create<SyncTransferData>(data);
			
			return retVal;
		}

		/// <summary>
		/// Converts from transfer data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		protected override EntityObjectHierarchy ConvertFromTransferData(object data)
		{
			
			SyncTransferData transferData = data as SyncTransferData;
			if(transferData == null)
			{
				throw new NotSupportedException("transfer data not supported");
			}

			EntityObjectHierarchy retVal = null;
			TransferDataConverFactory converFactory = new TransferDataConverFactory();

			if (transferData.SyncDataName != AppointmentTransferData.DataName)
			{
				throw new FormatException("invalid structire transfer data");
			}

			retVal = converFactory.Create<EntityObjectHierarchy>(transferData);
			
			return retVal;
		}


		/// <summary>
		/// Saves the item metadata.
		/// </summary>
		/// <param name="item">The item.</param>
		protected override void SaveItemMetadata(ItemMetadata item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			item.SetCustomField(SyncReplicaMetadata.TIMESTAMP_COLUMNNAME, (ulong)DateTime.UtcNow.Ticks);
			_metaData.SaveItemMetadata(item);
		}

		#region Not implemented
		protected override EntityObjectHierarchy MergeDataItem(EntityObjectHierarchy dataItem, 
															   EntityObjectHierarchy otherDataItem)
		{
			throw new NotImplementedException();
		}
		#endregion

	
	}
}
