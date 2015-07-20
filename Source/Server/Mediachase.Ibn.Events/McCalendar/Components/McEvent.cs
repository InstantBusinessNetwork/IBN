using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.iCal.Components;
using Mediachase.iCal.DataTypes;
using Mediachase.Ibn.Data;
using Mediachase.iCal.Serialization;
using Mediachase.Ibn.Events.McCalendar.Serialization.Entity;
using System.Collections;
using Mediachase.Ibn.Events.McCalendar.DataTypes;

namespace Mediachase.Ibn.Events.McCalendar.Components
{
	/// <summary>
	/// Адаптирует iCal.Event для работы с Entity объектами.
	/// <remarks> 
	/// - Может сериализоваться в/из дерева связанных EntityObject (EntityObjectHierarchy) в/из RFC 2445 iCalendar
	/// - Содержит дополнительный набор сериализуемых свойств (X- свойства)
	/// - Адаптирует базовую функиональность рекурсивного события под конкретный требования и ограничения (only 1 RRULE, RRULE должен быть ограничен )
	/// </remarks>
	/// 
	/// </summary>
	public class McEvent : Event
	{

		public const string EXCEPTION_ID_PROP = "X-" + CalendarEventEntity.FieldCalendarEventExceptionId;
		public const string DELETED_EXCEPTION_PROP = "X-" + CalendarEventEntity.FieldDeletedException;
		public const string MEETING_STATUS_PROP = "X-" + CalendarEventEntity.FieldMeetingStatus;
		public const string PROJECT_ID_PROP = "X-" + CalendarEventEntity.FieldProjectId;

		private List<McEvent> _recurrenceException = new List<McEvent>();


		public McEvent()
			: base(null)
		{
		}
		public McEvent(iCalObject parent)
			: base(parent)
		{
		}

		public bool IsReccuring
		{
			get
			{
				return RRule != null;
			}
		}

		/// <summary>
		/// Устанавливает дату ограничения рекурсии
		/// </summary>
		/// <remarks>
		/// Если у текущего паттерна рекурсии будет тип ограничения COUNT то он будет заменен на UNTIL
		/// </remarks>
		/// <param name="until">The until.</param>
		public iCalDateTime RecurrenceUntil
		{
			get
			{
				iCalDateTime retVal = null;
				if (RRule != null && RRule.Length != 0)
				{
					retVal = RRule[0].Until;
				}
				return retVal;
			}

			set
			{
				base.Until = value;
				//Fix TimeKind is local
				base.Until.IsUniversalTime = false;
				base.Until = DateTime.SpecifyKind(base.Until.Value, DateTimeKind.Local);
				foreach (RecurrencePattern pattern in RRule)
				{
					CountToUntilReplace(DTStart, base.Until, pattern);
				}
				
			}
		}

		public McRecurrencePattern RPattern
		{
			get
			{
				McRecurrencePattern retVal = null;
				if (RRule != null && RRule.Length != 0)
				{
					retVal = RRule[0] as McRecurrencePattern;
				}
				return retVal;
			}
		}
		public int RecurrenceCount
		{
			get
			{
				int retVal = int.MinValue;
				if(RRule != null && RRule.Length != 0)
				{
					retVal = RRule[0].Count;
				}
				return retVal;
			}
			set
			{
				if (RRule != null && RRule.Length != 0)
				{
					RRule[0].Count = value == 0 ? int.MinValue : value;
				}
			}
		}

		public PrimaryKeyId? EventPrimaryKeyId
		{
			get
			{
				PrimaryKeyId? retVal = null;
				try
				{
					retVal = PrimaryKeyId.Parse(UID);
				}
				catch
				{
				}

				return retVal;

			}
			set
			{
				if (value != null)
				{
					UID = value.Value.ToString();
				}
			}
		}
		/// <summary>
		/// Вычисляет текущее состояние рекурсии для события
		/// </summary>
		/// <value>The state of the recurrense.</value>
		public virtual eRecurrenceState RecurrenseState
		{
			get
			{
				eRecurrenceState retVal = eRecurrenceState.NotRecurring;

				if (Properties.ContainsKey(DELETED_EXCEPTION_PROP) 
					&& Convert.ToBoolean(((Property)Properties[DELETED_EXCEPTION_PROP]).Value))
				{
					retVal = eRecurrenceState.DeletedException;
				}
				else if (CalendarEventExceptionId != null)
				{
					retVal = eRecurrenceState.Exception;
				} 
				else if (IsReccuring)
				{
					retVal = eRecurrenceState.Master;
				}

				return retVal;
			}
		}

		/// <summary>
		/// PrimaryKeyId родительского события (для которого это события является exception)
		/// </summary>
		/// <value>The calendar event exception id.</value>
		public PrimaryKeyId? CalendarEventExceptionId
		{
			get
			{
				
				PrimaryKeyId? retVal = null;
				if (Properties.ContainsKey(EXCEPTION_ID_PROP))
				{
					Property property = (Property)Properties[EXCEPTION_ID_PROP];
					retVal = PrimaryKeyId.Parse(property.Value);
				}

				return retVal;
			}

			set
			{
				Property property = null;
				if (!Properties.ContainsKey(EXCEPTION_ID_PROP))
				{
					property = new Property(this, EXCEPTION_ID_PROP);
					//Properties.Add(property.Name, property);
				}

				property = (Property)Properties[EXCEPTION_ID_PROP];
				property.Value = value.HasValue ? value.Value.ToString() : null;
			}
		}

		/// <summary>
		/// Коллекция всех exception данного события
		/// </summary>
		/// <value>The recurrence exception.</value>
		public virtual McEvent[] RecurrenceException
		{
			get
			{
				return _recurrenceException.ToArray();
			}

		}

		public override iCalDateTime Recurrence_ID
		{
			get
			{
				return base.Recurrence_ID;
			}
			set
			{
				base.Recurrence_ID = value;
				//Fix TimeKind is local
				base.Recurrence_ID.IsUniversalTime = false;
				base.Recurrence_ID.Value = DateTime.SpecifyKind(base.Recurrence_ID.Value, DateTimeKind.Local);
			}
		}

		public override iCalDateTime DTStart
		{
			get
			{
				return base.DTStart;
			}
			set
			{
				base.DTStart = value;
				//Fix TimeKind is local
				base.DTStart.IsUniversalTime = false;
				base.DTStart.Value = DateTime.SpecifyKind(base.DTStart.Value, DateTimeKind.Local);
			}
		}

		public override iCalDateTime DTEnd
		{
			get
			{
				return base.DTEnd;
			}
			set
			{
				base.DTEnd = value;
				//Fix TimeKind is local
				base.DTEnd.IsUniversalTime = false;
				base.DTEnd.Value = DateTime.SpecifyKind(base.DTEnd.Value, DateTimeKind.Local);
			}
		}
		/// <summary>
		/// Gets the recurrence series end date.
		/// </summary>
		/// <value>The recurrence series end date.</value>
		/// <remarks>Вычисляет дату звершения серии, основываясь на том что рекурсия не может быть бесконечной
		/// и ограничена либо свойством UNTIL либо COUNT</remarks>
		public iCalDateTime RecurrenceSeriesEndDate
		{
			get
			{
				iCalDateTime retVal = DTEnd;
				if (IsReccuring)
				{
					RecurrencePattern rpattern = RRule[0];
					if (rpattern.Until != null)
					{
						retVal = rpattern.Until;
					}
					else
					{
						int occurCount = 0;
						int dayOffset = 0;
						while (occurCount < rpattern.Count)
						{
							occurCount = base.GetOccurrences(DTStart, DTEnd.AddDays(++dayOffset)).Count;
						}
						retVal = DTEnd.AddDays(dayOffset);
					}
				}
				//fix bug iCal iCalDateTime always UTC
				retVal.IsUniversalTime = false;
				retVal.Value = DateTime.SpecifyKind(retVal.Value, DateTimeKind.Local);
				return retVal;
			}
		}

		/// <summary>
		/// Добавляет расширенное свойство определяющее, что событие удалено
		/// </summary>
		public void MarkAsDeletedException()
		{
			string flagValue = true.ToString();

			if (!this.Properties.ContainsKey(DELETED_EXCEPTION_PROP))
			{
				AddProperty(DELETED_EXCEPTION_PROP, flagValue);
			}
			else
			{
				((Property)Properties[DELETED_EXCEPTION_PROP]).Value = flagValue;
			}
		}
		/// <summary>
		/// Clears the recurrence.
		/// </summary>
		public virtual void ClearRecurrence()
		{
			ClearEvaluation();
			_recurrenceException.Clear();
			RRule = null;
			RDate = null;
			ExDate = null;
			ExRule = null;
		}

		/// <summary>
		/// Gets the occurrences.
		/// </summary>
		/// <param name="startTime">The start time.</param>
		/// <param name="endTime">The end time.</param>
		/// <remarks> Вычисляет все экземпляры рекурсивного события в соотв с паттерном рекурсии. С учетом exception event.
		/// </remarks>
		/// <returns></returns>
		public override List<Occurrence> GetOccurrences(iCalDateTime startTime, iCalDateTime endTime)
		{
			//Очищаем кеш повторений
			ClearEvaluation();

			List<Occurrence> retVal = base.GetOccurrences(startTime, endTime);
		
			if(RecurrenceException != null)
			{
				foreach (McEvent exception in RecurrenceException)
				{
					if (exception.Recurrence_ID != null)
					{
						//Start = RecurrenceId, Duration = Parent event duration
						Occurrence exceptionOccur = new Occurrence(null, new Period(exception.Recurrence_ID, this.Duration));
						//remove exception from orig occurrences list
						if (retVal.Contains(exceptionOccur))
						{
							retVal.Remove(exceptionOccur);
						}
					}
				}
			}
			return retVal;
		}

	
		/// <summary>
		/// Counts to until replace. For  recurrence pattern
		/// </summary>
		/// <param name="dtStart">The dt start.</param>
		/// <param name="until">The until.</param>
		/// <param name="rpattern">The rpattern.</param>
		private static void CountToUntilReplace(iCalDateTime dtStart, iCalDateTime until, RecurrencePattern pattern)
		{
			//COUNT is specified
			if (pattern.Count > 0)
			{
				List<iCalDateTime> occurs = pattern.Evaluate(dtStart, dtStart, until);
				if (occurs.Count != 0 && occurs[occurs.Count - 1] >= until)
				{
					//Remove COUNT
					pattern.Count = int.MinValue;
					pattern.Until = until;
				}
			}
			else
			{
				pattern.Until = until;
			}
			
		}


		public void AddReccurenceException(McEvent eventException)
		{
			if (_recurrenceException.Contains(eventException))
				return;

			_recurrenceException.Add(eventException);

		}


		/// <summary>
		/// Получает дерево связвнных entityObject из объектной модели эвента
		/// <remarks>
		/// Структура EntityObjectHierarchy События 
		///		<event>
		///			<recurrencePattern>
		///			</recurrencePattern>
		///			<resource>
		///			</resource>
		///			<exception>
		///				<resource>
		///				</resource>
		///			</exception>
		///		</event>
		/// </remarks>
		/// </summary>
		/// <returns></returns>
		public EntityObjectHierarchy GetEntityObjectHierarchy()
		{
			EntityObjectHierarchy retVal = null;
			McSerializerFactory factory = new McSerializerFactory();
			IEntitySerializable serializer = factory.Create<IEntitySerializable>(this);
			retVal = serializer.Serialize<CalendarEventEntity>();
			if (RRule != null)
			{
				foreach (RecurrencePattern rpattern in RRule)
				{
					serializer = factory.Create<IEntitySerializable>(rpattern);
					retVal.Childrens.Add(serializer.Serialize<CalendarEventRecurrenceEntity>());
				}
			}

			if (Organizer != null)
			{

				serializer = factory.Create<IEntitySerializable>(Organizer);
				if (serializer != null)
					retVal.Childrens.Add(serializer.Serialize<CalendarEventResourceEntity>());
			}
			if (Attendee != null)
			{
				foreach (Cal_Address attendee in Attendee)
				{
					serializer = factory.Create<IEntitySerializable>(attendee);
					if (serializer != null)
						retVal.Childrens.Add(serializer.Serialize<CalendarEventResourceEntity>());

				}
			}
			//exceptions
			if (RecurrenceException != null)
			{
				foreach (McEvent exception in RecurrenceException)
				{
					retVal.Childrens.Add(exception.GetEntityObjectHierarchy());
				}
			}

			return retVal;

		}


		/// <summary>
		/// Получает объект типа McEvent из дерева связанных EntityObject (EntityObjectHierarchy)
		/// </summary>
		/// <param name="entityHierarchy">The entity hierarchy.</param>
		/// <returns></returns>
		public static McEvent GetMcEventFromEntityObjectHierarchy(EntityObjectHierarchy entityHierarchy)
		{
			McEvent retVal = new McEvent();
			McSerializerFactory factory = new McSerializerFactory();
			IEntitySerializable serializer = factory.Create<IEntitySerializable>(retVal);
			retVal = (McEvent)serializer.Deserialize<CalendarEventEntity>(entityHierarchy.InnerEntity);
			foreach (EntityObjectHierarchy child in entityHierarchy.Childrens)
			{
				if (child.InnerEntity.MetaClassName == CalendarEventRecurrenceEntity.ClassName)
				{
					McRecurrencePattern rPattern = new McRecurrencePattern();
					retVal.AddChild(rPattern);
					serializer = factory.Create<IEntitySerializable>(rPattern);
					if (serializer != null)
					{
						rPattern = (McRecurrencePattern)serializer.Deserialize<CalendarEventRecurrenceEntity>(child.InnerEntity);
						retVal.RRule = new RecurrencePattern[] { rPattern };
					}
				}
				else if (child.InnerEntity.MetaClassName == CalendarEventResourceEntity.ClassName)
				{
					McCalAddress resource = new McCalAddress();
					retVal.AddChild(resource);
					serializer = factory.Create<IEntitySerializable>(resource);
					if(serializer != null)
					{
						resource = (McCalAddress)serializer.Deserialize<CalendarEventResourceEntity>(child.InnerEntity);
						retVal.AddChild(resource);
						if(((CalendarEventResourceEntity)child.InnerEntity).ResourceEventOrganizator)
						{
							retVal.Organizer = resource;
						}
						else
						{
							
							List<Cal_Address> oldVals = new List<Cal_Address>();
							if (retVal.Attendee != null)
							{
								oldVals.AddRange(retVal.Attendee);
							}
							oldVals.Add(resource);
							retVal.Attendee = oldVals.ToArray();
						}
					}
				}
				else if (child.InnerEntity.MetaClassName == CalendarEventEntity.ClassName)
				{
					McEvent exception = GetMcEventFromEntityObjectHierarchy(child);
					if (exception != null)
					{
						retVal.AddReccurenceException(exception);
					}
				}
			}

			return retVal;
			
		}

	}
}
