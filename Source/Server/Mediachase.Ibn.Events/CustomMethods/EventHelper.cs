using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Events.McCalendar.Components;
using Mediachase.iCal.DataTypes;
using Mediachase.iCal.Components;
using Mediachase.Ibn.Events.McCalendar;
using Mediachase.Ibn.Events.Request;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Events.McCalendar.DataTypes;


namespace Mediachase.Ibn.Events.CustomMethods
{
	/// <summary>
	/// Реализует обработчики запросов на модификацию CalendarEvent, CalendarEventResource, CalendarEventRecurrence
	/// и основную логику работы с рекурсивными событиями
	/// </summary>
	public static class EventHelper
	{

		public const string FORCE_BASE_PARAM = "ForceBase";


		private static string[] AlwaysExcludedEntityProperty = { CalendarEventEntity.FieldCalendarEventExceptionId,
												   CalendarEventEntity.FieldIsRecurring,
												   CalendarEventEntity.FieldDeletedException };
		//private static string[] ExcludedEntityPropertyIfPrimaryKeyChanged = { CalendarEventEntity.FieldStart,
		//                                                                      CalendarEventEntity.FieldEnd };

		#region Prepare Load event

		/// <summary>
		/// LoadEvent обработчик 
		/// Позволяет загрузить как виртуальное так и реальное событие по его PrimaryKeyId
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns>CalendarEventEntity</returns>
		internal static EntityObject PrerpareLoadEventRequest(LoadRequest request)
		{
			VirtualEventId vEventId = (VirtualEventId)request.Target.PrimaryKeyId.Value;
			McEvent mcEvent = LoadCalEvent(vEventId);
			//Если это виртуальное событие то необходимо посчитать дату начала и конца из ReccurenceId
			if (vEventId.IsRecurrence && mcEvent.RecurrenseState != eRecurrenceState.Exception)
			{
				iCalDateTime reccurencyDt = RecurrenceId2iCalDateTime(vEventId.RecurrenceId, mcEvent.DTStart);
				mcEvent.DTStart = reccurencyDt;
				mcEvent.DTEnd = mcEvent.DTStart + mcEvent.Duration;
				//set recurrence_id for virtual events
				mcEvent.Recurrence_ID = reccurencyDt;
				//Return virtual id
				mcEvent.UID = vEventId.ToString();
			}
			EntityObjectHierarchy hierarchy = ConstructEntityHierarchy(mcEvent);
			return hierarchy.InnerEntity;
		}
		#endregion

		#region Prepare Update Event
		/// <summary>
		/// UpdateEvent обработчик
		/// </summary>
		/// <remarks>
		/// - Может разбивть серию рекурсивных событий на будущую и прошлую (серию) или на одиночные события
		///	- Реализует удаление exception при возврате exception на прежнее место в серии
		///	- Изменение длительности для серии
		/// </remarks>
		/// <param name="request">The request.</param>
		internal static void PrepareUpdateEventRequest(UpdateRequest request)
		{
			EntityObject eventEntity = request.Target;
			if (eventEntity == null || eventEntity.MetaClassName != CalendarEventEntity.ClassName)
				throw new ArgumentException("invalid target name");

			bool exceptionCreated;
			bool needUpdate = true;

			VirtualEventId vEventId = (VirtualEventId)eventEntity.PrimaryKeyId;
			Dictionary<PrimaryKeyId, PrimaryKeyId> primaryKeyMap = PrepareChangeData(vEventId, out exceptionCreated);
			//Предполагаем что vEeventId это серия
			PrimaryKeyId origEventId = (PrimaryKeyId)vEventId;
			if (vEventId.IsRecurrence)
			{
				//Берем виртуальный ID как реальный
				origEventId = PrimaryKeyId.Parse(vEventId.ToString());
			}
			//Получаем id (при создании exception)нового eventa в противно случаи id будет прежний
			PrimaryKeyId newEventId = FindDestiantionId(primaryKeyMap, origEventId);
			McEvent mcEvent = LoadCalEvent(newEventId);
			//При апдейте для серии
			if (mcEvent.RecurrenseState == eRecurrenceState.Master)
			{
				DateTime newDtStart = (DateTime)eventEntity[CalendarEventEntity.FieldStart];
				DateTime newDtEnd = (DateTime)eventEntity[CalendarEventEntity.FieldEnd];
			
				//Если Год/Месяц/День не определены значит меняется длительность
				if(newDtStart.Year == DateTime.MinValue.Year && newDtEnd.Year == DateTime.MinValue.Year)
				{
					//При изменении длительности события
					//необходимо для update использовать поле начала родительсокого события 
					//(у виртуального события оно отличается от родительского)
					TimeSpan newEventDuration = newDtEnd.TimeOfDay - newDtStart.TimeOfDay;
					if (mcEvent.Duration != newEventDuration)
					{
						newDtStart = DateTime.SpecifyKind(mcEvent.DTStart.Value, DateTimeKind.Local);
						//Так как операция сложения приводит к вызову Copy в типе iCalDateTime и вызова конструктора iCalDateTime, а там 
						//идет принудительная установка бита UTC в DateTime. То след необходимо явно присваивать бит Local.
						newDtEnd = DateTime.SpecifyKind(mcEvent.DTStart.Value + newEventDuration, DateTimeKind.Local);
					}
				}

				//Удалить рекурсию если она имела органичение Until и время события стало больше
				if (mcEvent.RPattern != null && mcEvent.RecurrenceCount == int.MinValue
					&& newDtStart >= mcEvent.RecurrenceUntil.Value)
				{
					
					CalendarEventRecurrenceEntity recurrenceEntity = new CalendarEventRecurrenceEntity(mcEvent.RPattern.MetaObjectId.Value);
					recurrenceEntity.EventId = mcEvent.EventPrimaryKeyId.Value;
					//Удаляем рекурсию
					DeleteRequest deleteRequest = (DeleteRequest)ConstructRequest<DeleteRequest>(recurrenceEntity, false);
					BusinessManager.Execute(deleteRequest);
				}

				 eventEntity[CalendarEventEntity.FieldStart] = newDtStart;
				 eventEntity[CalendarEventEntity.FieldEnd] = newDtEnd;
			} //Если редактируемое событие exception
			else if(mcEvent.RecurrenseState == eRecurrenceState.Exception)
			{
				//Реализация фичи: Когда событие являющееся exception и отличающееся от родительского только датой начала,
				//опять получает дату начала равную первоначальному значению, то этот exception должен быть удален

				//Если модифицирован exception кроме случая когда он только создан
				if (!exceptionCreated)
				{
					McEvent exceptionEvent = mcEvent;
					//Загружаем родительский event
					McEvent masterSeriesEvent = LoadCalEvent(exceptionEvent.CalendarEventExceptionId.Value);

					Dictionary<string, string[]> usedPropInComp = new Dictionary<string, string[]>();
					usedPropInComp.Add(CalendarEventEntity.ClassName, CalendarEventEntity.ComparedProperties);
					//Исключаем из набора полей участвующих в сравнении CalendarEventResource, поле EventId
					usedPropInComp.Add(CalendarEventResourceEntity.ClassName,
									   Array.FindAll<string>(CalendarEventResourceEntity.ComparedProperties,
															 delegate(string propName)
															 {
																 return propName != CalendarEventResourceEntity.FieldEventId;
															 }));
					EntityObjectHierarchy minHierarchy = ConstructEntityHierarchy(exceptionEvent);
					EntityObjectHierarchy maxHierarchy = ConstructEntityHierarchy(masterSeriesEvent);
					if (maxHierarchy.Childrens.Count < minHierarchy.Childrens.Count)
					{
						EntityObjectHierarchy tmp = maxHierarchy;
						maxHierarchy = minHierarchy;
						minHierarchy = tmp;
					}
					EntityHierarchyComparer entityHierarchyComp = new EntityHierarchyComparer(maxHierarchy, usedPropInComp);

					TimeSpan exEventDuration = (DateTime)eventEntity[CalendarEventEntity.FieldEnd] - (DateTime)eventEntity[CalendarEventEntity.FieldStart];
					//Если они идентичны по определенным полям и имеют одинаковую длительность
					if (masterSeriesEvent.Duration.Value == exEventDuration
						&& entityHierarchyComp.CompareHierarchy(minHierarchy))
					{
						//Если новая дата начала exception event совпадает с первоначальной расчетной от ReccurenceId и даты начала родительского события
						//т.е возвращается на свое место
						if ((DateTime)eventEntity[CalendarEventEntity.FieldStart] == RecurrenceId2iCalDateTime(((VirtualEventId)newEventId).RecurrenceId,
																												masterSeriesEvent.DTStart).Value)
						{
							//то удаляем этот exception
							DeleteEvent(exceptionEvent, true);
							needUpdate = false;
						}
					}
				}
			}
			//exclude not outer updatability properties
			SanityProcessEntityProperty(eventEntity);

			if (needUpdate)
			{
				eventEntity.PrimaryKeyId = newEventId;
				UpdateRequest updateRequest = (UpdateRequest)ConstructRequest<UpdateRequest>(eventEntity, true);
				BusinessManager.Execute(updateRequest);
			}

		}
		#endregion

		#region Prepare Create Event
		/// <summary>
		/// CreateEvent обработчик
		/// </summary>
		/// <remarks>
		///  - Если у entity указан PrimaryKeyId то он и будет ключем в базе
		///  - Генерирует новый PrimaryKeyId который служит pk для Event-a
		///  </remarks>
		/// <param name="request">The request.</param>
		/// <returns></returns>
		internal static CreateResponse CreateEventRequestHandle(CreateRequest request)
		{
			EntityObject eventEntity = request.Target;
			if (eventEntity == null || eventEntity.MetaClassName != CalendarEventEntity.ClassName)
				throw new ArgumentException("invalid target name");

			PrimaryKeyId eventId = (PrimaryKeyId)VirtualEventId.CreateInstance();
			if (eventEntity.PrimaryKeyId != null)
			{
				eventId = eventEntity.PrimaryKeyId.Value;
			}
			eventEntity["CalendarEventId"] = eventId;

			CreateRequest createRequest = (CreateRequest)ConstructRequest<CreateRequest>(eventEntity, true);
			return (CreateResponse)BusinessManager.Execute(createRequest);
		}
		#endregion

		#region Delete event
		/// <summary>
		/// DeleteEvent обработчик
		/// </summary>
		/// <remarks>
		/// - Удаляет событие и все связанные с ним объекты
		/// - Может разбивать серию рекурсивных событий на будущую и прошлую (серию) или на одиночные события
		/// - Маркирует но не удаляет exception
		/// </remarks>
		/// <param name="request">The request.</param>
		internal static void DeleteEventRequestHandle(DeleteRequest request)
		{
			bool needDelete = true;
			bool exceptionCreated;
			EntityObject eventEntity = request.Target;
			if (eventEntity == null || eventEntity.MetaClassName != CalendarEventEntity.ClassName)
				throw new ArgumentException("invalid target name");

			VirtualEventId vEventId = (VirtualEventId)eventEntity.PrimaryKeyId;
			Dictionary<PrimaryKeyId, PrimaryKeyId> primaryKeyMap = PrepareChangeData(vEventId, out exceptionCreated);
			PrimaryKeyId origEventId = (PrimaryKeyId)vEventId;
			if (vEventId.IsRecurrence)
			{
				origEventId = PrimaryKeyId.Parse(vEventId.ToString());
			}
			McEvent mcEvent = LoadCalEvent(FindDestiantionId(primaryKeyMap, origEventId));

			//1. Для серии с Exceptions удалить всю серию и удалить каскадно все Exceptions
			if (mcEvent.IsReccuring)
			{
				if (mcEvent.RecurrenceException != null)
				{
					foreach (McEvent exception in mcEvent.RecurrenceException)
					{
						BusinessManager.Delete(new CalendarEventEntity(exception.EventPrimaryKeyId.Value));
					}
				}
			}
			else if (mcEvent.RecurrenseState == eRecurrenceState.Exception)
			{
				//Маркируем как удаленное
				needDelete = false;
				mcEvent.MarkAsDeletedException();
				UpdateEvent(mcEvent, true);
			}

			if (needDelete)
			{
				DeleteEvent(mcEvent, true);
			}

		}
		#endregion

		#region Prepare Update recurrence
		/// <summary>
		/// UpdateRecurrence обработчик
		/// </summary>
		/// <param name="request">The request.</param>
		/// <remarks>
		/// - Обновляет паттерн рекурсии
		/// - Может разбивть серию рекурсивных событий на будущую и прошлую (серию) 
		/// </remarks>
		/// <returns></returns>
		internal static PrimaryKeyId PrepareUpdateRecurrenceRequest(UpdateRequest request)
		{
			EntityObject recurrenceEntity = request.Target;

			if (recurrenceEntity == null || recurrenceEntity.MetaClassName != CalendarEventRecurrenceEntity.ClassName)
				throw new ArgumentException("invalid target name");

			PrimaryKeyId recurrenceId = recurrenceEntity.PrimaryKeyId.Value;
			PrimaryKeyId eventId = recurrenceEntity.Properties.GetValue<PrimaryKeyId>(CalendarEventRecurrenceEntity.FieldEventId);

			//Удаляем рекурсию
			DeleteRequest deleteRequest = (DeleteRequest)ConstructRequest<DeleteRequest>(recurrenceEntity, false);
			BusinessManager.Execute(deleteRequest);

			//Создаем новую рекурсию
			CreateRequest createRequest = (CreateRequest)ConstructRequest<CreateRequest>(recurrenceEntity, false);
			CreateResponse createResponse = (CreateResponse)BusinessManager.Execute(createRequest);

			SanityProcessEntityProperty(recurrenceEntity);
			//exclude not outer updatability properties
			//SanityProcessEntityProperty(recurrenceEntity, false);

			return createResponse.PrimaryKeyId;
		}
		#endregion

		#region Delete recurrence
		/// <summary>
		/// DeleteRecurrence обработчик
		/// </summary>
		/// <param name="request">The request.</param>
		/// <remarks>
		///  - Может разбивть серию рекурсивных событий на будущую и прошлую (серию)
		///  - Удаляет паттерн рекурсии
		///  - Может удалить все exception данной серии
		/// </remarks>
		/// <returns></returns>
		internal static void DeleteRecurrenceRequestHandle(DeleteRequest request)
		{
			bool needDelete = true;
			bool exceptionCreated;
			EntityObject recurrenceEntity = request.Target;
			if (recurrenceEntity == null || recurrenceEntity.MetaClassName != CalendarEventRecurrenceEntity.ClassName)
				throw new ArgumentException("invalid target name");

			PrimaryKeyId recurrenceId = recurrenceEntity.PrimaryKeyId.Value;
			PrimaryKeyId eventId = recurrenceEntity.Properties.GetValue<PrimaryKeyId>(CalendarEventRecurrenceEntity.FieldEventId);

			Dictionary<PrimaryKeyId, PrimaryKeyId> primaryKeyMap = PrepareChangeData((VirtualEventId)eventId, out exceptionCreated);
			McEvent mcEvent = LoadCalEvent(FindDestiantionId(primaryKeyMap, eventId));
			recurrenceId = FindDestiantionId(primaryKeyMap, recurrenceId);

			//	1. Для серии с определенными EXCEPTIONS удалить заданную рекурсию и каскадно удалить
			// все события являющиеся EXCEPTIONS для данной серии
			if (mcEvent.IsReccuring)
			{
				////Meняем статус
				//mcEvent.RecurrenseState = eRecurrenceState.NotRecurring;
				//UpdateEvent(mcEvent, true);
				if (mcEvent.RecurrenceException != null)
				{
					foreach (McEvent exception in mcEvent.RecurrenceException)
					{
						//Рекурсивно удаляем все Exception  с использованием базового хендлера(true в формировании запроса)
						DeleteEvent(exception, true);
						//DeleteRequest deleteRequest = (DeleteRequest)ConstructRequest<DeleteRequest>(new CalendarEventEntity(exception.EventPrimaryKeyId.Value), false);
						//BusinessManager.Execute(deleteRequest);
					}
				}
			}

			if (needDelete)
			{
				recurrenceEntity.PrimaryKeyId = recurrenceId;
				DeleteRequest deleteRequest = (DeleteRequest)ConstructRequest<DeleteRequest>(recurrenceEntity, true);
				BusinessManager.Execute(deleteRequest);
			}
		}
		#endregion

		#region Create Recurrence
		/// <summary>
		/// CreateRecurrence обработчик
		/// </summary>
		/// <param name="request">The request.</param>
		/// <remarks>
		/// - Создает паттерн рекурсии события
		/// </remarks>
		/// <returns></returns>
		internal static CreateResponse CreateRecurrenceRequestHandle(CreateRequest request)
		{
			EntityObject recurrenceEntity = request.Target;
			if (recurrenceEntity == null || recurrenceEntity.MetaClassName != CalendarEventRecurrenceEntity.ClassName)
				throw new ArgumentException("invalid target name");

			VirtualEventId vEventId = (VirtualEventId)recurrenceEntity.Properties.GetValue<PrimaryKeyId>(CalendarEventRecurrenceEntity.FieldEventId);
			if (vEventId.IsRecurrence)
				throw new ArgumentException("recurrence not added to recurrence occur event");

			SanityProcessEntityProperty(recurrenceEntity);
			CreateRequest createRequest = (CreateRequest)ConstructRequest<CreateRequest>(recurrenceEntity, true);
			return (CreateResponse)BusinessManager.Execute(createRequest);
		}
		#endregion

		#region Prepare Update Resource
		/// <summary>
		/// UpdateResource обработчик
		/// </summary>
		/// <param name="request">The request.</param>
		/// <remarks>
		/// - Может разбивать серию рекурсивных событий на будущую и прошлую (серию) или на одиночные события
		/// - Обновляет ресурс события
		/// </remarks>
		/// <returns></returns>
		internal static PrimaryKeyId PrepareUpdateResourceRequest(UpdateRequest request)
		{
			EntityObject resourceEntity = request.Target;
			bool exceptionCreated;
			if (resourceEntity == null || resourceEntity.MetaClassName != CalendarEventResourceEntity.ClassName)
				throw new ArgumentException("invalid target name");

			VirtualEventId vEventId = (VirtualEventId)resourceEntity.Properties.GetValue<PrimaryKeyId>(CalendarEventResourceEntity.FieldEventId);
			PrimaryKeyId resourceId = resourceEntity.PrimaryKeyId.Value;
			Dictionary<PrimaryKeyId, PrimaryKeyId> primaryKeyMap = PrepareChangeData(vEventId, out exceptionCreated);
			PrimaryKeyId newResourceId = FindDestiantionId(primaryKeyMap, resourceId);

			return newResourceId;
		}
		#endregion

		#region Delete Resource
		/// <summary>
		/// DeleteResource обработчик
		/// Deletes the resource request handle.
		/// </summary>
		/// <remarks>
		/// - Может разбивать серию рекурсивных событий на будущую и прошлую (серию) или на одиночные события
		/// - Удаляет ресурс события
		/// </remarks>
		/// <param name="request">The request.</param>
		internal static void DeleteResourceRequestHandle(DeleteRequest request)
		{
			EntityObject resourceEntity = request.Target;
			bool exceptionCreated;
			if (resourceEntity == null || resourceEntity.MetaClassName != CalendarEventResourceEntity.ClassName)
				throw new ArgumentException("invalid target name");

			//Нельзя удалять организатора события
			CalendarEventResourceEntity resource = (CalendarEventResourceEntity)BusinessManager.Load(request.Target.MetaClassName,
																									 request.Target.PrimaryKeyId.Value);
			if (!resource.ResourceEventOrganizator)
			{
				VirtualEventId vEventId = (VirtualEventId)resourceEntity.Properties.GetValue<PrimaryKeyId>(CalendarEventResourceEntity.FieldEventId);
				PrimaryKeyId resourceId = resourceEntity.PrimaryKeyId.Value;
				Dictionary<PrimaryKeyId, PrimaryKeyId> primaryKeyMap = PrepareChangeData(vEventId, out exceptionCreated);
				//подменяем pk на новый
				resourceEntity.PrimaryKeyId = FindDestiantionId(primaryKeyMap, resourceId);
				DeleteRequest deleteRequest = (DeleteRequest)ConstructRequest<DeleteRequest>(resourceEntity, true);
				BusinessManager.Execute(deleteRequest);
			}
		}
		#endregion

		#region Create Resource
		/// <summary>
		/// CreateResource обработчик
		/// </summary>
		/// <remarks>
		/// - Может разбивать серию рекурсивных событий на будущую и прошлую (серию) или на одиночные события
		/// - Добавляет к событию новый ресурс
		/// </remarks>
		/// <param name="request">The request.</param>
		/// <returns></returns>
		internal static CreateResponse CreateResourceRequestHandle(CreateRequest request)
		{
			EntityObject resourceEntity = request.Target;
			bool exceptionCreated;
			if (resourceEntity == null || resourceEntity.MetaClassName != CalendarEventResourceEntity.ClassName)
				throw new ArgumentException("invalid target name");

			VirtualEventId vEventId = (VirtualEventId)resourceEntity.Properties.GetValue<PrimaryKeyId>(CalendarEventResourceEntity.FieldEventId);
			Dictionary<PrimaryKeyId, PrimaryKeyId> primaryKeyMap = PrepareChangeData(vEventId, out exceptionCreated);
			//Заменям event id на новый если в процессе создания ресурса был создан новый event
			PrimaryKeyId origEventId = (PrimaryKeyId)vEventId;
			if (vEventId.IsRecurrence)
			{
				origEventId = PrimaryKeyId.Parse(vEventId.ToString());
			}
			PrimaryKeyId newEventId = FindDestiantionId(primaryKeyMap, origEventId);
			resourceEntity[CalendarEventResourceEntity.FieldEventId] = newEventId;

			//Call orig handler
			CreateRequest createRequest = (CreateRequest)ConstructRequest<CreateRequest>(resourceEntity, true);
			return (CreateResponse)BusinessManager.Execute(createRequest);
		}

		#endregion

		#region Split recurrence event series methods

		/// <summary>
		/// Может разбивать серию рекурсивных событий на одиночные события(exception)
		/// </summary>
		/// <param name="vEventId">The v event id.</param>
		/// <param name="exceptionCreated">if set to <c>true</c> [exception created].</param>
		/// <returns>Маппинг старых pk на новые pk</returns>
		internal static Dictionary<PrimaryKeyId, PrimaryKeyId> PrepareChangeData(VirtualEventId vEventId, out bool exceptionCreated)
		{
			exceptionCreated = false;
			//resourceId mapping old<->new
			Dictionary<PrimaryKeyId, PrimaryKeyId> retVal = new Dictionary<PrimaryKeyId, PrimaryKeyId>();
			McEvent mcEvent = LoadCalEvent(vEventId);
			if (mcEvent != null && mcEvent.IsReccuring)
			{
				//Редактируем серию
				if (vEventId.IsRecurrence)
				{
					//Пытаемся разбить рекурсию (создать exception)
					exceptionCreated = TryCreateException(mcEvent, vEventId, retVal);
				}
				else
				{
					//Необходимо найти последний exception с рекурсией (exception с рекурсией будет только когда имело место 
					//разбиение по прошедшим событиям)
					//Данная функциональность необходима для редактирования и разбивки серии через прошедшую (не редактируемую)
					//Редактировать как серию но указать событие которое является частью серии в прошлом, будет найдена последняя 
					//exception серия
					McEvent mcEventLastEx = FindLastExceptionSeries(mcEvent);
					PrimaryKeyId pkLastEx = mcEventLastEx.EventPrimaryKeyId.Value;
					PrimaryKeyId pkEvent = mcEvent.EventPrimaryKeyId.Value;
					//Необходимо найти соответвующие элементы нового собыитя (рекурсию и ресурсы) и создать связь их id
					if (pkLastEx != pkEvent)
					{
						//retVal.Add(pkEvent, pkLastEx);
						EntityObjectHierarchy exceptionHierarhy = ConstructEntityHierarchy(mcEventLastEx);
						EntityObjectHierarchy masterHierarhy = ConstructEntityHierarchy(mcEvent);
						//Добавляем связь eventId на event exception id
						retVal.Add(pkEvent, pkLastEx);
						foreach (EntityObjectHierarchy childMaster in masterHierarhy.Childrens)
						{
							foreach (EntityObjectHierarchy childException in exceptionHierarhy.Childrens)
							{
								//Рекурсия может быть только одна одна
								if (childMaster.InnerEntity.MetaClassName == CalendarEventRecurrenceEntity.ClassName
									&& childException.InnerEntity.MetaClassName == childMaster.InnerEntity.MetaClassName)
								{
									retVal.Add(childMaster.InnerEntity.PrimaryKeyId.Value, childException.InnerEntity.PrimaryKeyId.Value);
									break;
								}


								if (childMaster.InnerEntity.MetaClassName == CalendarEventResourceEntity.ClassName
									&& childMaster.InnerEntity.MetaClassName == childException.InnerEntity.MetaClassName)
								{
									//ресурсы ищем путем сравнения по уникальным полям
									EntityComparer entityComp = new EntityComparer(childMaster.InnerEntity, CalendarEventResourceEntity.ComparedProperties);
									if (entityComp.CompareEntity(childException.InnerEntity))
									{
										retVal.Add(childMaster.InnerEntity.PrimaryKeyId.Value, childException.InnerEntity.PrimaryKeyId.Value);
									}

								}
							}

						}
					}
					//Пытаемся разбить серию по прошедшему веремени
					//Отключаем механизм разбивки серии по прошлому
					//exceptionCreated = TrySplitSeries(mcEventLastEx, retVal);
				}
			}
			return retVal;
		}

		private static bool TrySplitSeries(McEvent mcEvent, Dictionary<PrimaryKeyId, PrimaryKeyId> primaryKeyChangeMap)
		{
			//Series are spited
			bool retVal = false;

			if (mcEvent == null)
				throw new ArgumentNullException("mcEvent");

			//Split event on two part one in past other in future
			McEvent newEvent = null;
			iCalDateTime recurrenceDt = new iCalDateTime(DateTime.UtcNow);
			//if event recurrence range greater that now then split recurrence and create recurrence exception
			if (mcEvent.IsReccuring)
			{
				List<Occurrence> occurrEarlyNow = mcEvent.GetOccurrences(mcEvent.DTStart, recurrenceDt);
				if (occurrEarlyNow.Count != 0)
				{
					Period lastEarlyNowOccurence = occurrEarlyNow[occurrEarlyNow.Count - 1].Period;
					recurrenceDt = lastEarlyNowOccurence.StartTime;
					// occur1 occur2 NOW occur3
					//Need find occur3 
					//Другого подхода не нашел кроме как прибавить один год к последнему occurrence перед NOW и взять первое occurrences из
					//полученной серии
					switch (mcEvent.RRule[0].Frequency)
					{
						case FrequencyType.Daily:
							recurrenceDt = recurrenceDt.AddDays(7);
							break;
						case FrequencyType.Weekly:
							recurrenceDt = recurrenceDt.AddMonths(1);
							break;
						case FrequencyType.Monthly:
							recurrenceDt = recurrenceDt.AddYears(1);
							break;
						case FrequencyType.Yearly:
							recurrenceDt = recurrenceDt.AddYears(10);
							break;
					}
					List<Occurrence> occurAfterNowList = mcEvent.GetOccurrences(lastEarlyNowOccurence.StartTime, recurrenceDt);
					if (occurAfterNowList.Count > 1)
					{
						retVal = true;
						Occurrence occurFirstAfterNow = occurAfterNowList[1];
						newEvent = (McEvent)mcEvent.Copy();
						newEvent.DTStart = occurFirstAfterNow.Period.StartTime;
						newEvent.DTEnd = occurFirstAfterNow.Period.EndTime;
						//newEvent.RecurrenseState = eRecurrenceState.Exception;
						newEvent.CalendarEventExceptionId = mcEvent.EventPrimaryKeyId;
						//Устанавливаем предел рекурсии для нового события только если он был как COUNT
						//в случае UNTIL он будет прежний
						if (newEvent.RecurrenceCount != int.MinValue)
						{
							newEvent.RecurrenceCount -= occurrEarlyNow.Count;
						}
						//Устанавливаем предел рекурсии для события в прошлом
						//Причем если у него используется предел COUNT то он будет изменен на соотв UNTIL
						//дата until будет равна дате начала exception - 1 минута, так как GetOccurence использует 
						//дату Until включительно
						mcEvent.RecurrenceUntil = newEvent.DTStart - new TimeSpan(TimeSpan.TicksPerMinute);
						UpdateEvent(mcEvent, true);

						newEvent = CreateEvent(newEvent, primaryKeyChangeMap);

						//Переназначаем все exception базовой серии на новую серию
						foreach (McEvent exception in mcEvent.RecurrenceException)
						{
							if (exception.RecurrenseState == eRecurrenceState.Exception)
							{
								exception.CalendarEventExceptionId = newEvent.EventPrimaryKeyId;
								UpdateEvent(exception, true);
							}
						}
						//Вычисляем новый recurrenceId
						//if (virtualEventId.IsRecurrence)
						//{
						//    virtualEventId.RecurrenceId = CalculateNewRecurrenceId(virtualEventId.RecurrenceId, mcEvent.DTStart, newEvent.DTStart);
						//}
						//Восстанавливаем virtual id с новыми значением  recurrence id
						//virtualEventId = VirtualEventId.CreateInstance(newEventPk, virtualEventId.RecurrenceId);
					}
					//else
					//{
					//    //нельзя модифицировать события которые уже прошли
					//    throw new Exception("Read only (event series in past)");
					//}
				}
			}

			return retVal;
		}

		private static bool TryCreateException(McEvent mcEvent, VirtualEventId virtualEventId,
											  Dictionary<PrimaryKeyId, PrimaryKeyId> primaryKeyChangeMap)
		{
			bool retVal = false;
			if (mcEvent == null)
				throw new ArgumentNullException("mcEvent");
			//If specified occurrence recurrence then create exception
			if (virtualEventId.IsRecurrence)
			{
				retVal = true;
				//Вычисляем дату occurrence относительно базового события
				iCalDateTime reccurencyDt = RecurrenceId2iCalDateTime(virtualEventId.RecurrenceId, mcEvent.DTStart);
				McEvent newEvent = (McEvent)mcEvent.Copy();
				newEvent.ClearRecurrence();
				newEvent.DTStart = reccurencyDt;
				newEvent.DTEnd = newEvent.DTStart + mcEvent.Duration;
				//newEvent.RecurrenseState = eRecurrenceState.Exception;
				newEvent.CalendarEventExceptionId = (PrimaryKeyId)virtualEventId;
				newEvent.UID = virtualEventId.ToString();
				newEvent = CreateEvent(newEvent, false, primaryKeyChangeMap);
			}

			return retVal;
		}

		#endregion
		#region recurrence id methods
		public static PrimaryKeyId CreateExceptionId(PrimaryKeyId eventId, DateTime baseStart, DateTime recurrenceId)
		{
			VirtualEventId retVal = (VirtualEventId)eventId;
			retVal.RecurrenceId = iCalDateTime2Recurrence(baseStart, recurrenceId);
			return PrimaryKeyId.Parse(retVal.ToString());
		}
		/// <summary>
		/// Вычисляет RecurrenceId по двум датам
		/// </summary>
		/// <param name="recurrenceId">The recurrence id.</param>
		/// <param name="dtStartOld">The dt start old.</param>
		/// <param name="dtStartNew">The dt start new.</param>
		/// <returns></returns>
		internal static Int16 CalculateNewRecurrenceId(Int16 recurrenceId, iCalDateTime dtStartOld, iCalDateTime dtStartNew)
		{
			Int16 retVal = recurrenceId;

			if (dtStartNew <= dtStartOld)
				throw new ArgumentException("invalid base dates");

			TimeSpan delta = dtStartNew - dtStartOld;
			retVal = (Int16)(recurrenceId - (Int16)delta.TotalDays);
			if (retVal < 0)
				throw new Exception("invalid recurrence id");

			return retVal;
		}

		/// <summary>
		/// Вычисляет дату на основании recurrenceId и базовой даты 
		/// </summary>
		/// <param name="recurrenceId">The recurrence id.</param>
		/// <param name="dtStart">The dt start.</param>
		/// <returns></returns>
		internal static iCalDateTime RecurrenceId2iCalDateTime(Int16 recurrenceId, iCalDateTime dtStart)
		{
			iCalDateTime retVal = dtStart.AddDays(recurrenceId);
			//Fix Bug in iCal TimeKind is always UTC
			retVal.IsUniversalTime = false;
			retVal.Value = DateTime.SpecifyKind(retVal.Value, DateTimeKind.Local);
			return retVal;
		}

		/// <summary>
		/// Вычисляет RecurrenceId из двуж дат (базовой даты серии и даты повторения серии)
		/// </summary>
		/// <param name="dtStart">The dt start.</param>
		/// <param name="occur">The occur.</param>
		/// <returns></returns>
		internal static Int16 iCalDateTime2Recurrence(iCalDateTime dtStart, iCalDateTime occur)
		{
			TimeSpan delta = occur - dtStart;
			return (Int16)delta.TotalDays;
		}
		internal static Int16 iCalDateTime2Recurrence(DateTime dtStart, DateTime occur)
		{
			return iCalDateTime2Recurrence(new iCalDateTime(dtStart), new iCalDateTime(occur));
		}
		#endregion

		/// <summary>
		/// Присваивает связанные поля всх дочерних элементов entityHierarchy pk родительского элемента 
		/// </summary>
		/// <param name="entityHierarchy">The entity hierarchy.</param>
		public static void NormailzeReferences(EntityObjectHierarchy entityHierarchy)
		{
			CalendarEventEntity eventEntity = entityHierarchy.InnerEntity as CalendarEventEntity;
			if (eventEntity == null)
				throw new Exception("Invalid hierarchy");

			foreach (EntityObjectHierarchy child in entityHierarchy.Childrens)
			{
				string entityType = child.InnerEntity.MetaClassName;
				if (entityType == CalendarEventRecurrenceEntity.ClassName)
				{
					((CalendarEventRecurrenceEntity)child.InnerEntity).EventId = eventEntity.PrimaryKeyId.Value;
				}
				else if (entityType == CalendarEventResourceEntity.ClassName)
				{
					((CalendarEventResourceEntity)child.InnerEntity).EventId = eventEntity.PrimaryKeyId.Value;
				}
				else if (entityType == CalendarEventEntity.ClassName)
				{
					((CalendarEventEntity)child.InnerEntity).CalendarEventExceptionId = eventEntity.PrimaryKeyId.Value;
				}
			}
		}

		/// <summary>
		/// Преобразует объект типа McEvent в EntityObject
		/// </summary>
		/// <param name="mcEvent">The mc event.</param>
		/// <returns></returns>
		internal static EntityObjectHierarchy ConstructEntityHierarchy(McEvent mcEvent)
		{
			if (mcEvent == null)
				throw new NullReferenceException("mcEvent");

			EntityObjectHierarchy retVal = null;
			retVal = mcEvent.GetEntityObjectHierarchy();
			NormailzeReferences(retVal);
			return retVal;
		}


		#region McEvent methods
		/// <summary>
		/// Загружает все exception события с заданным pk
		/// </summary>
		/// <param name="eventId">The event id.</param>
		/// <returns></returns>
		private static IEnumerable<McEvent> LoadCalEventException(PrimaryKeyId eventId)
		{
			CalendarEventEntity eventTarget = new CalendarEventEntity();
			ListRequest listRequest = (ListRequest)EventHelper.ConstructRequest<ListRequest>(eventTarget, true);
			listRequest.Filters = new FilterElement[] { new FilterElement(CalendarEventEntity.FieldCalendarEventExceptionId,
																						FilterElementType.Equal, eventId) };
			ListResponse listResponse = (ListResponse)BusinessManager.Execute(listRequest);
			foreach (EntityObject entity in listResponse.EntityObjects)
			{
				yield return LoadCalEvent(entity.PrimaryKeyId.Value);
			}

		}

		/// <summary>
		/// Возвращает иерархию элементов события по его ключу .
		/// </summary>
		/// <param name="eventId">The event id.</param>
		/// <returns></returns>
		public static EntityObjectHierarchy LoadEventEntityHierarchy(PrimaryKeyId eventId)
		{
			VirtualEventId vEventId = (VirtualEventId)eventId;
			return ConstructEntityHierarchy(LoadCalEvent(vEventId));
		}

		/// <summary>
		/// Получает объект типа McEvent из EntityObject по заданному pk
		/// </summary>
		/// <param name="vEventId">The v event id.</param>
		/// <returns></returns>
		internal static McEvent LoadCalEvent(VirtualEventId vEventId)
		{
			McEvent retVal = null;
			//Try first load with virtual event id
			PrimaryKeyId eventId = PrimaryKeyId.Parse(vEventId.ToString());
			retVal = LoadCalEvent(eventId);
			if (retVal == null)
			{
				retVal = LoadCalEvent((PrimaryKeyId)vEventId);
			}
			return retVal;
		}

		/// <summary>
		/// Loads the cal event.
		/// </summary>
		/// <param name="eventId">The event id.</param>
		/// <returns></returns>
		internal static McEvent LoadCalEvent(PrimaryKeyId eventId)
		{
			McEvent retVal = null;
			EntityObjectHierarchy entityHierarhy = null;
			//1. Get event
			CalendarEventEntity eventTarget = new CalendarEventEntity(eventId);
			LoadRequest loadRequest = (LoadRequest)ConstructRequest<LoadRequest>(eventTarget, true);
			LoadResponse response = new LoadResponse();
			try
			{
				response = (LoadResponse)BusinessManager.Execute(loadRequest);
			}
			catch (System.Exception)
			{

			}
			if (response.EntityObject != null)
			{
				List<EntityObject> listResponse = new List<EntityObject>();
				//2. Get recursion pattern
				CalendarEventRecurrenceEntity reccurencyTarget = new CalendarEventRecurrenceEntity();
				ListRequest recurrencyRequest = (ListRequest)ConstructRequest<ListRequest>(reccurencyTarget, true);
				recurrencyRequest.Filters = new FilterElement[] {new FilterElement(CalendarEventRecurrenceEntity.FieldEventId,
																FilterElementType.Equal, eventId)};

				//3. get resources
				CalendarEventResourceEntity resourceTarget = new CalendarEventResourceEntity();
				ListRequest resourceRequest = (ListRequest)ConstructRequest<ListRequest>(resourceTarget, true);
				resourceRequest.Filters = new FilterElement[]{new FilterElement(CalendarEventResourceEntity.FieldEventId,
																Mediachase.Ibn.Data.FilterElementType.Equal, eventId)};

				listResponse.AddRange(((ListResponse)BusinessManager.Execute(recurrencyRequest)).EntityObjects);
				listResponse.AddRange(((ListResponse)BusinessManager.Execute(resourceRequest)).EntityObjects);

				//4. Construct entityObjectHierarhy
				entityHierarhy = new EntityObjectHierarchy(response.EntityObject);
				foreach (EntityObject childEntity in listResponse)
				{
					entityHierarhy.Childrens.Add(new EntityObjectHierarchy(childEntity));
				}
				retVal = McEvent.GetMcEventFromEntityObjectHierarchy(entityHierarhy);

				//Load exceptions
				if (retVal.IsReccuring && retVal.RecurrenseState == eRecurrenceState.Master)
				{
					foreach (McEvent eventException in LoadCalEventException(eventId))
					{
						//Set recurrence_id for exceptions
						VirtualEventId vEventId = (VirtualEventId)eventException.EventPrimaryKeyId.Value;
						eventException.Recurrence_ID = RecurrenceId2iCalDateTime(vEventId.RecurrenceId, retVal.DTStart);
						retVal.AddReccurenceException(eventException);
					}

				}
			}

			return retVal;

		}

		private static McEvent CreateEvent(McEvent mcEvent, Dictionary<PrimaryKeyId, PrimaryKeyId> primaryKeyMap)
		{
			return CreateEvent(mcEvent, true, primaryKeyMap);
		}

		/// <summary>
		/// Создает и сохраняет CalendarEventEntity и все связанные с ним объекты (CalendarEventResource, CalendarEventrecurrence)
		/// из объекта типа McEvent
		/// </summary>
		/// <param name="mcEvent">The mc event.</param>
		/// <param name="generateEventId">if set to <c>true</c> [generate event id].</param>
		/// <param name="primaryKeyMap">The primary key map.</param>
		/// <returns></returns>
		private static McEvent CreateEvent(McEvent mcEvent, bool generateEventId, Dictionary<PrimaryKeyId, PrimaryKeyId> primaryKeyMap)
		{
			if (mcEvent == null)
				throw new ArgumentNullException("mcEvent");

			EntityObjectHierarchy entityHierarchy = ConstructEntityHierarchy(mcEvent);
			if (entityHierarchy != null)
			{
				CalendarEventEntity eventEntity = entityHierarchy.InnerEntity as CalendarEventEntity;
				if (eventEntity != null)
				{
					PrimaryKeyId? oldEntityPk = eventEntity.PrimaryKeyId;
					if (generateEventId)
					{
						//generate new pk
						eventEntity.PrimaryKeyId = (PrimaryKeyId)VirtualEventId.CreateInstance();
						NormailzeReferences(entityHierarchy);
						BusinessManager.Create(eventEntity);
						if (oldEntityPk != null)
						{
							primaryKeyMap.Add(oldEntityPk.Value, eventEntity.PrimaryKeyId.Value);
						}
					}
					else
					{
						BusinessManager.Create(eventEntity);
					}

					//Удаляем из набора entity организатора так как он создается при создании event
					//CalendarEventCreateHandler в методе PostCreate
					RemoveOrganizatorFromEntityHierarchy(entityHierarchy);

					foreach (EntityObjectHierarchy child in entityHierarchy.Childrens)
					{
						oldEntityPk = child.InnerEntity.PrimaryKeyId;
						//Reset PrimaryKey allow generate new
						//child.InnerEntity.PrimaryKeyId = (PrimaryKeyId)Guid.NewGuid();
						child.InnerEntity.PrimaryKeyId = BusinessManager.Create(child.InnerEntity);
						if (oldEntityPk != null)
						{
							primaryKeyMap.Add(oldEntityPk.Value, child.InnerEntity.PrimaryKeyId.Value);
						}
					}
				}
				//				UpdateFromHierarchy(entityHierarchy, true);
			}

			return McEvent.GetMcEventFromEntityObjectHierarchy(entityHierarchy);
		}

		/// <summary>
		/// Обновляет событие из объекта типа McEvent
		/// </summary>
		/// <param name="mcEvent">The mc event.</param>
		/// <param name="forceBase">if set to <c>true</c> [force base].</param>
		private static void UpdateEvent(McEvent mcEvent, bool forceBase)
		{
			if (mcEvent == null)
				throw new ArgumentNullException("mcEvent");

			EntityObjectHierarchy entityHierarchy = ConstructEntityHierarchy(mcEvent);
			if (entityHierarchy != null)
			{
				UpdateFromHierarchy(entityHierarchy, forceBase);
			}
		}

		/// <summary>
		/// Удаляет событие 
		/// </summary>
		/// <param name="mcEvent">The mc event.</param>
		/// <param name="forceBase">if set to <c>true</c> [force base].</param>
		private static void DeleteEvent(McEvent mcEvent, bool forceBase)
		{
			if (mcEvent == null)
				throw new ArgumentNullException("mcEvent");

			//First recursive delete all exceptions
			foreach (McEvent exception in mcEvent.RecurrenceException)
			{
				DeleteEvent(exception, forceBase);
			}
			EntityObjectHierarchy entityHierarchy = ConstructEntityHierarchy(mcEvent);
			DeleteFromHierarchy(entityHierarchy, forceBase);
		}
		#endregion

		#region EntityHierarchy methods
		private static void DeleteFromHierarchy(EntityObjectHierarchy entityHierarchy, bool forceBase)
		{
			CalendarEventEntity eventEntity = entityHierarchy.InnerEntity as CalendarEventEntity;
			DeleteRequest deleteRequest = null;
			if (eventEntity != null)
			{
				//delete first child referenced elements
				foreach (EntityObjectHierarchy child in entityHierarchy.Childrens)
				{
					deleteRequest = (DeleteRequest)ConstructRequest<DeleteRequest>(child.InnerEntity, forceBase);
					BusinessManager.Execute(deleteRequest);
				}
				//delete event
				deleteRequest = (DeleteRequest)ConstructRequest<DeleteRequest>(eventEntity, forceBase);
				BusinessManager.Execute(deleteRequest);
			}
		}

		private static void UpdateFromHierarchy(EntityObjectHierarchy entityHierarchy, bool forceBase)
		{
			//Create event first
			CalendarEventEntity eventEntity = entityHierarchy.InnerEntity as CalendarEventEntity;
			if (eventEntity != null)
			{
				UpdateRequest updateRequest = (UpdateRequest)ConstructRequest<UpdateRequest>(eventEntity, forceBase);
				BusinessManager.Execute(updateRequest);
				foreach (EntityObjectHierarchy child in entityHierarchy.Childrens)
				{
					updateRequest = (UpdateRequest)ConstructRequest<UpdateRequest>(child.InnerEntity, forceBase);
					BusinessManager.Execute(updateRequest);
				}
			}
		}

		#endregion

		#region Helpers methods
		internal static PrimaryKeyId FindDestiantionId(Dictionary<PrimaryKeyId, PrimaryKeyId> linkedMap, PrimaryKeyId origPk)
		{
			PrimaryKeyId retVal = origPk;
			if (linkedMap != null)
			{
				while (linkedMap.ContainsKey(retVal))
				{
					retVal = linkedMap[retVal];
				}
			}

			return retVal;
		}

		/// <summary>
		/// Фильтрует набор свойств EntityObject в соотв заданными условиями
		/// </summary>
		/// <param name="entity">The entity.</param>
		private static void SanityProcessEntityProperty(EntityObject entity)
		{
			////Если во время update произошло созлания исключения с рекурсией, то из оригинального запроса удаляются
			////свойства даты так как дата будет установлена в процессе разбивки
			//if (exceptionSeriesCreated)
			//{
			//    ExcludeEntityProperty(entity, ExcludedEntityPropertyIfPrimaryKeyChanged);
			//}
			ExcludeEntityProperty(entity, AlwaysExcludedEntityProperty);

			if (entity.MetaClassName == CalendarEventRecurrenceEntity.ClassName)
			{
				//Рекурсия должна быть ограничена только одним типом ограничения UNTIL или COUNT
				int count = (int)entity[CalendarEventRecurrenceEntity.FieldOccurrences];
				DateTime? until = (DateTime?)entity[CalendarEventRecurrenceEntity.FieldPatternEndDate];

				if (count != 0 && until != null)
				{
					throw new ArgumentException("UNTIL and COUNT unable both use");
				}
			}
		}

		private static void ExcludeEntityProperty(EntityObject entity, string[] exProp)
		{
			foreach (string propertyName in exProp)
			{
				if (entity.Properties.Contains(propertyName))
				{
					EntityObjectProperty entityObjProp = entity.Properties[propertyName];
					entity.Properties.Remove(entityObjProp);
				}
			}
		}

		/// <summary>
		/// Позволяет создать стандартный запрос или запрос обрабатываемый расшириными обработчиками.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity">The entity.</param>
		/// <param name="forceBase">if set to <c>true</c> [force base].</param>
		/// <returns></returns>
		public static Mediachase.Ibn.Core.Business.Request ConstructRequest<T>(EntityObject entity, bool forceBase) where T : new()
		{
			Mediachase.Ibn.Core.Business.Request retVal = new T() as Mediachase.Ibn.Core.Business.Request;
			if (retVal != null)
			{
				retVal.Target = entity;
				retVal.Parameters.Add(FORCE_BASE_PARAM, forceBase);
			}

			return retVal;
		}

		/// <summary>
		/// Finds the last exception series.
		/// </summary>
		/// <param name="mcEvent">The mc event.</param>
		/// <returns></returns>
		private static McEvent FindLastExceptionSeries(McEvent mcEvent)
		{
			McEvent retVal = mcEvent;
			foreach (McEvent exception in mcEvent.RecurrenceException)
			{
				if (exception.IsReccuring && exception.RecurrenceException != null)
				{
					retVal = FindLastExceptionSeries(exception);
					break;
				}
			}
			return retVal;
		}


		/// <summary>
		/// Позволяет рекурсивно найти фильтр в дереве фильтров
		/// </summary>
		/// <param name="filters">The filters.</param>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		internal static object RecursiveFindFilterElementValue(FilterElement[] filters, string source)
		{
			object retVal = null;
			foreach (FilterElement filterEl in filters)
			{
				if (filterEl.ChildElements != null)
				{
					retVal = RecursiveFindFilterElementValue(filterEl.ChildElements.ToArray(), source);
				}
				else if (filterEl.Source == source)
				{
					retVal = filterEl.Value;

					if (filterEl.ValueIsTemplate)
					{
						retVal = TemplateResolver.ResolveAll(filterEl.Value.ToString());
					}
				}

				if (retVal != null)
					break;
			}

			return retVal;
		}

		/// <summary>
		/// Позволяет рекурсивно заменить фильтр в дереве фильтров
		/// </summary>
		/// <param name="filters">The filters.</param>
		/// <param name="source">The source.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		internal static FilterElement[] RecursiveReplaceFilterElementValue(FilterElement[] filters, string source, object value)
		{
			List<FilterElement> retVal = new List<FilterElement>();

			foreach (FilterElement filterEl in filters)
			{
				FilterElement filter = filterEl;
				if (filterEl.ChildElements != null)
				{
					FilterElement[] childFilterEl = RecursiveReplaceFilterElementValue(filterEl.ChildElements.ToArray(),
																						source, value);
					filterEl.ChildElements.Clear();
					filterEl.ChildElements.AddRange(childFilterEl);
				}

				if (filterEl.Source == source)
				{
					filter = new FilterElement(source, filterEl.Type, value);
				}
				retVal.Add(filter);
			}

			return retVal.ToArray();
		}

		private static void RemoveOrganizatorFromEntityHierarchy(EntityObjectHierarchy entityHierarhy)
		{
			entityHierarhy.Childrens.RemoveAll(delegate(EntityObjectHierarchy entity)
			{
				return entity.InnerEntity.MetaClassName == CalendarEventResourceEntity.ClassName
				&& (bool)entity.InnerEntity[CalendarEventResourceEntity.FieldResourceEventOrganizator];
			});
		}
		#endregion
	}
}
