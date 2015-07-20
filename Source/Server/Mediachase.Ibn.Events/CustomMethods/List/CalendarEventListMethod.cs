using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Events.McCalendar.Components;
using Mediachase.iCal.Components;
using Mediachase.iCal.DataTypes;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Events.McCalendar;
using Mediachase.Ibn.Events.Request;
using Mediachase.Ibn.Events.Response;
using Mediachase.Ibn.Data.Services;

namespace Mediachase.Ibn.Events.CustomMethods.List
{
	/// <summary>
	/// CustomMethod позволяет получить список событий по определнным критериям
	/// </summary>
	public class CalendarEventListMethod
	{
		public const string METHOD_NAME = "ListEvent";

		/// <summary>
		/// Lists the event.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <remarks>
		///  - Удаляет FilterElement с source (DTStart, DTEnd)в запросе и формирует на основании их предикат
		///    позволяющий производить выборку по данным критериям из произвольной коллекции
		///   
		/// </remarks>
		/// <returns></returns>
		public static ListResponse ListEvent(CalendarEventListRequest request)
		{
			
			List<CalendarEventEntity> retVal = new List<CalendarEventEntity>();

			TZID dtStartTZID = String.IsNullOrEmpty(request.DTStartTimeZoneId) ? null : new TZID(request.DTStartTimeZoneId);
			TZID dtEndTZID = String.IsNullOrEmpty(request.DTEndTimeZoneId) ? null : new TZID(request.DTEndTimeZoneId);

			//Заполняем FIlterElementCollection элементами исходного массива фильтров
			FilterElementCollection filterColl = new FilterElementCollection();
			foreach (FilterElement filterEl in request.Filters)
			{
				filterColl.Add(filterEl);
			}
			AndBlockFilterElementPredicate<DateTime> andBlockDTStartFilterPredicate = 
																new AndBlockFilterElementPredicate<DateTime>();
			AndBlockFilterElementPredicate<DateTime> andBlockDTEndFilterPredicate = 
																new AndBlockFilterElementPredicate<DateTime>();

			ConstructPredicate(andBlockDTStartFilterPredicate, filterColl, CalendarEventEntity.FieldStart);
			ConstructPredicate(andBlockDTEndFilterPredicate, filterColl, CalendarEventEntity.FieldEnd);

			//Получаем все FilterElement которые связаны с датой и формируем на основе их дерево выражений
			foreach (FilterElement filterEl in FindFilterElementBySource(new string[] {CalendarEventEntity.FieldStart,
																						 CalendarEventEntity.FieldEnd},
																		 request.Filters, true))
			{
				//Рекурсивно удаляем найденный filterElement из исходного набора
				DeepRemoveFilter(filterColl, filterEl);
			}
			//добавляем дополнительные критерии 
			AddExtraCriteria(filterColl);
			//Получаем список эвентов
			retVal.AddRange(ListEvents(filterColl, request));
			
			//Применяем критерии отбора по датам начала и завершения взятые из первоначального запроса, и преобразованные 
			//для отбора из коллекции EntityObjects
			retVal.RemoveAll(delegate(CalendarEventEntity eventEntity) { return !andBlockDTStartFilterPredicate.Evaluate(eventEntity.Start); });
			retVal.RemoveAll(delegate(CalendarEventEntity eventEntity) { return !andBlockDTEndFilterPredicate.Evaluate(eventEntity.End); });

			retVal.Sort();

			return new ListResponse(retVal.ToArray());

		}

		/// <summary>
		/// Возвращает список CalendarEventResourceEtity в которых есть ресурс c заданным principalId
		/// </summary>
		/// <param name="principalId">The principal id.</param>
		/// <returns></returns>
		private static List<CalendarEventResourceEntity> GetListCalendarResourceByPrincipalId(int principalId)
		{
			List<CalendarEventResourceEntity> retVal = new List<CalendarEventResourceEntity>();
			//Конструируем стандартный запрос LIST для ресурсов
			ListRequest moListRequest = (ListRequest)EventHelper.ConstructRequest<ListRequest>(new CalendarEventResourceEntity(), true);
			moListRequest.Filters = new FilterElement[] { new FilterElement(CalendarEventResourceEntity.FieldPrincipalId, 
																			FilterElementType.Equal, principalId) };
			//выполняем стандартный запрос
			ListResponse listResp = (ListResponse)BusinessManager.Execute(moListRequest);
			if (listResp != null && listResp.EntityObjects != null)
			{

				retVal.AddRange(Array.ConvertAll<EntityObject, CalendarEventResourceEntity>(listResp.EntityObjects,
																							delegate(EntityObject entity) { return entity as CalendarEventResourceEntity; }));
			}

			return retVal;
		}

		/// <summary>
		/// Возвращает список событий как виртуальных так и реальных.
		/// </summary>
		/// <param name="filterColl">The filter coll.</param>
		/// <param name="request">The request.</param>
		/// <returns></returns>
		private static IEnumerable<CalendarEventEntity> ListEvents(FilterElementCollection filterColl, CalendarEventListRequest request)
		{
			McEvent mcEvent = null;
			//Конструируем стандартный запрос LIST без критериев содержащих дату начала и конца события
			ListRequest moListRequest = (ListRequest)EventHelper.ConstructRequest<ListRequest>(request.Target, true);
			moListRequest.Filters = filterColl.ToArray();
			moListRequest.Sorting = request.Sorting;
			//выполняем стандартный запрос
			ListResponse listResp = (ListResponse)BusinessManager.Execute(moListRequest);

			if (listResp != null && listResp.EntityObjects != null)
			{
				//Пробегаем по реальным events
				foreach (CalendarEventEntity entity in listResp.EntityObjects)
				{
					mcEvent = EventHelper.LoadCalEvent(entity.PrimaryKeyId.Value);
					if (mcEvent.IsReccuring)
					{
						iCalDateTime dtReccurBase = mcEvent.DTStart;
						//Пробегаем по виртуальным events
						foreach (Occurrence occur in mcEvent.GetOccurrences(mcEvent.DTStart, mcEvent.RecurrenceSeriesEndDate))
						{
							//Создаем виртуальные события
							VirtualEventId vEventId = (VirtualEventId)entity.PrimaryKeyId;
							vEventId.RecurrenceId = EventHelper.iCalDateTime2Recurrence(dtReccurBase, occur.Period.StartTime);
							//Set virtual id
							mcEvent.UID = vEventId.ToString();
							//Set recurrence-ID
							mcEvent.Recurrence_ID = occur.Period.StartTime;
							mcEvent.DTStart = occur.Period.StartTime;
							mcEvent.DTEnd = occur.Period.EndTime;
							EntityObjectHierarchy recurrenceEntity = EventHelper.ConstructEntityHierarchy(mcEvent);
							CalendarEventEntity occurEventEntity = recurrenceEntity.InnerEntity as CalendarEventEntity;
							if (occurEventEntity != null)
							{
								yield return occurEventEntity;
							}
						}
					}
					else
					{
						yield return entity;
					}
				}
			}
		}

		/// <summary>
		/// Добавляет дополнительные критерии выборки событий.
		/// </summary>
		/// <param name="filterColl">The filter coll.</param>
		private static void AddExtraCriteria(FilterElementCollection filterColl)
		{
			//int currentUserId = Security.CurrentUserId;
			////Выбираем только те события в которых текущий пользователь участник или организатор
			//FilterElement eventResFilter = new FilterElement("PrimaryKeyId", FilterElementType.Equal, Guid.Empty);
			//List<CalendarEventResourceEntity> listResource = GetListCalendarResourceByPrincipalId(currentUserId);
			//if(listResource.Count != 0)
			//{
			//    //Формируем in диапазон возможных event pk (те события в которых есть данный ресурс)
			//    List<PrimaryKeyId> pkInList = new List<PrimaryKeyId>();
			//    foreach(CalendarEventResourceEntity resourceEntity in listResource)
			//    {
			//        pkInList.Add(resourceEntity.EventId);
			//    }
			//    eventResFilter = new FilterElement("PrimaryKeyId", FilterElementType.In, pkInList.ToArray());
			//}

			//filterColl.Add(eventResFilter);
			//не выводить эвенты со статусом DeletedException
			filterColl.Add(new FilterElement(CalendarEventEntity.FieldDeletedException,
											FilterElementType.Equal, false));
		}

		private static void DeepRemoveFilter(FilterElementCollection filterColl, FilterElement toRemoveFilter)
		{
			if (filterColl.Contains(toRemoveFilter))
			{
				filterColl.Remove(toRemoveFilter);
			}
			foreach (FilterElement filterEl in filterColl)
			{
				if (filterEl.HasChildElements)
				{
					DeepRemoveFilter(filterEl.ChildElements, toRemoveFilter); 
				}
			}
		}

		private static void ConstructPredicate(FilterElementPredicate<DateTime> predicate, FilterElementCollection filterColl, string sourceName)
		{
			foreach(FilterElement filterEl in filterColl)
			{
				if (filterEl.HasChildElements)
				{
					//Создаем блок
					predicate = predicate.AppendChildPredicate(filterEl);
					//рекурсивно обрабатываем дочерние фильтры
					ConstructPredicate(predicate, filterEl.ChildElements, sourceName);
				}

				if (filterEl.Source == sourceName)
				{
					predicate.AppendChildPredicate(filterEl);
				}
			}
			
		}

		private static IEnumerable<FilterElement> FindFilterElementBySource(string[] source, FilterElement[] filters, bool recursive)
		{
			foreach (FilterElement filter in filters)
			{
				//Рекурсивно ищем в дочерних элементах
				if (recursive && filter.HasChildElements)
				{
					foreach (FilterElement childFilter in FindFilterElementBySource(source, filter.ChildElements.ToArray(), recursive))
					{
						yield return childFilter;
					}
				}

				if (Array.Find<string>(source, delegate(string other) { return filter.Source == other; }) != null)
				{
					yield return filter;
				}
			}
		}
	}
}
