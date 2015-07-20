using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Microsoft.Synchronization;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Events;
using Mediachase.Ibn.Events.Request;
using Mediachase.Ibn.Events.CustomMethods.UpdateResources;
using Mediachase.Ibn.Events.CustomMethods.ChangeTracking;
using Mediachase.Ibn.Data.Services;
using Microsoft.Synchronization.MetadataStorage;
using System.Security.Principal;

namespace Mediachase.Sync.EntityObjectProvider
{
	/// <summary>
	/// Регистрирует все изменения CalendarEvent в хранилище метаданных
	/// </summary>
	public class CalendarEventChangeTracker : IPlugin
	{
		private CalendarEventSyncProvider _provider;

		/// <summary>
		/// Initializes a new instance of the <see cref="CalendarEventChangeTracker"/> class.
		/// </summary>
		public CalendarEventChangeTracker()
		{
		}

		private CalendarEventSyncProvider Provider
		{
			get
			{
				if (_provider == null)
				{

					_provider = EntitySyncProviderManager.GetProvider(Security.CurrentUserId,
										 Mediachase.Sync.Core.eSyncProviderType.Appointment) as CalendarEventSyncProvider;
				}

				return _provider;
			}
		}


		#region Metadata tracking implementation
		private SyncVersion GetNewSyncVersion()
		{
			return new SyncVersion(0, Provider.ReplicaMetadata.GetNextTickCount());
		}

		private ItemMetadata CreateMetadataItem(PrimaryKeyId? eventPk)
		{
			if (eventPk == null)
			{
				throw new ArgumentNullException("eventPk");
			}
			SyncVersion newVersion = GetNewSyncVersion();
			//New item, must have been created since that last time the metadata was updated.
			//Create the item metadata required for sync (giving it a SyncID and a version, defined to be a DWORD and a ULONGLONG
			//For creates, simply provide the relative replica ID (0) and the tick count for the provider (ever increasing)
			SyncGlobalId globalId = new SyncGlobalId(0, Guid.NewGuid());
			ItemMetadata retVal = Provider.ReplicaMetadata.CreateItemMetadata(new SyncId(globalId), newVersion);
			retVal.ChangeVersion = newVersion;
			//Set Uri as pk event
			retVal.SetCustomField(SyncReplicaMetadata.URI_COLUMNNAME, (Guid)eventPk.Value);
			Provider.ReplicaMetadata.SaveItemMetadata(retVal);

			return retVal;
		}

		private void ChangeVersionMetadataItem(PrimaryKeyId? eventPk)
		{
			if (eventPk == null)
			{
				throw new ArgumentNullException("eventPk");
			}
			ItemMetadata item = Provider.ReplicaMetadata.FindItemMetadataByUniqueIndexedField(SyncReplicaMetadata.URI_COLUMNNAME,
																							  eventPk);
			//Если метаданные для данного элемента отсутсвуют то создаем их
			//Это может произойти когда реплика была создана при уже существующих calendar events.
			if (item == null)
			{
				CreateMetadataItem(eventPk);
			}
			else
			{
				SyncVersion newVersion = GetNewSyncVersion();
				item.ChangeVersion = newVersion;
				Provider.ReplicaMetadata.SaveItemMetadata(item);
			}
		}

		private void MarkAsTombstoneMetadataItem(PrimaryKeyId? eventPk)
		{
			if (eventPk == null)
			{
				throw new ArgumentNullException("eventPk");
			}
			SyncVersion newVersion = GetNewSyncVersion();
			ItemMetadata item = Provider.ReplicaMetadata.FindItemMetadataByUniqueIndexedField(SyncReplicaMetadata.URI_COLUMNNAME,
																							  eventPk);
			//Если метаданные для данного элемента отсутсвуют то нет необходимости создавать их и помечать как удаленные
			if (item != null)
			{
				item.MarkAsDeleted(newVersion);
				Provider.ReplicaMetadata.SaveItemMetadata(item);
			}
		}
		#endregion

		#region IPlugin Members
		/// <summary>
		/// Регестрирует все изменения над событиями в хранилище метаданных
		/// Данный plugin должен быть подписан на все post события производимые над всеми метаклассами событий
		/// В хранилище метаданных отслеживаться будет только события (в случаи наличия рекрсии только базовое зобытие рекрсии). 
		/// Например: 1. Если мы изменяем ресурсы события то в хранилище метаданных изменится версия связанного с данным ресурсом события.
		///	2. Если мы создаем exception то обновиться метаданные только базовго события, а новые метаданные для exception созданы не будут.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Execute(BusinessContext context)
		{
			bool needTracking = false;

			//Отмлеживаем только во время отсутствия прцесса синхронизации
			if (!CalendarEventSyncProvider.SyncSessionInProcess(Provider.ReplicaId))
			{
				//Регистрируем только вызовы BusinessObjectRequestHandler, все вызовы  перехватываемые  обработчиком
				//отслеживать не нужно. 
				bool origRequestInvoked = context.Request.Parameters.GetValue<bool>(Mediachase.Ibn.Events.CustomMethods.EventHelper.FORCE_BASE_PARAM, false);
				if (origRequestInvoked)
				{
					//перечисление методов
					switch (context.GetMethod())
					{
						case RequestMethod.Create:
						case RequestMethod.Delete:
						case RequestMethod.Update:
						case CalendarResourcesUpdateMethod.METHOD_NAME:
						case ChangeTrackingMethod.METHOD_NAME:
							needTracking = true;
							break;

					}
				}
			}

			if (needTracking)
			{
				PrimaryKeyId? eventId = null;
				//Пытаемся получить event pk из запроса или ответа
				if (context.GetTargetMetaClassName() == CalendarEventEntity.ClassName)
				{
					eventId = context.Request.Target.PrimaryKeyId;
					if (eventId == null)
					{
						eventId = ((CreateResponse)context.Response).PrimaryKeyId;
					}
				}
				else if (context.GetTargetMetaClassName() == CalendarEventRecurrenceEntity.ClassName)
				{
					eventId = (PrimaryKeyId?)context.Request.Target[CalendarEventRecurrenceEntity.FieldEventId];
				}
				else if (context.GetTargetMetaClassName() == CalendarEventResourceEntity.ClassName)
				{
					eventId = (PrimaryKeyId?)context.Request.Target[CalendarEventResourceEntity.FieldEventId];
				}

				VirtualEventId vEventId = (VirtualEventId)eventId.Value;
				eventId = vEventId.RealEventId;
				//для события (виртуального или exception) необходимо произвести обновление только метаданных 
				//родительского события
				if (vEventId.IsRecurrence)
				{
					ChangeVersionMetadataItem(eventId);
				}
				else
				{
					//только для методов касающихся события выполняются операции создания и удаления метаданных
					if (context.GetTargetMetaClassName() == CalendarEventEntity.ClassName &&
						context.Request.Method == RequestMethod.Create)
					{
						CreateMetadataItem(eventId.Value);
					}
					else if (context.GetTargetMetaClassName() == CalendarEventEntity.ClassName &&
							 context.Request.Method == RequestMethod.Delete)
					{
						MarkAsTombstoneMetadataItem(eventId);
					}
					else
					{
						ChangeVersionMetadataItem(eventId);
					}
				}

			}
		}

		#endregion
	}
}
