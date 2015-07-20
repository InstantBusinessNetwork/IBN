using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.iCal.DataTypes;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Events.McCalendar.Components;

namespace Mediachase.Ibn.Events.McCalendar.Serialization.Entity
{
	public static class iCal2EntityMapping
	{
		private static Dictionary<Type, List<Dictionary<string, MappingResult>>> _mappingDef = 
										new Dictionary<Type,List<Dictionary<string,MappingResult>>>();

		public class MappingResult
		{
			public String _name;
			public Type _valueType;
			
			public MappingResult(string name, Type valueType)
			{
				_name = name;
				_valueType = valueType;
			}

			public string Name
			{
				get
				{
					return _name;
				}
			}
			
			public Type ValueType
			{
				get
				{
					return _valueType;
				}
			}

			
		}

		static iCal2EntityMapping()
		{
			
			//rfc2445 properties 

			//
			//CalendarEventEntity
			//

			//CATEGORIES Property
			RegisterMapProp<CalendarEventEntity>(CalendarEventEntity.FieldCategories, new MappingResult("CATEGORIES", typeof(string)));
			//DTStart Property
			RegisterMapProp<CalendarEventEntity>(CalendarEventEntity.FieldStart, new MappingResult("DTSTART", typeof(DateTime)));
			//TZID Parameter
			RegisterMapProp<CalendarEventEntity>(CalendarEventEntity.FieldStartTimeZoneOffset, new MappingResult("DTSTART.TZID", typeof(string)));
			//DTEnd Property
			RegisterMapProp<CalendarEventEntity>(CalendarEventEntity.FieldEnd, new MappingResult("DTEND", typeof(DateTime)));
			//TZID Parameter
			RegisterMapProp<CalendarEventEntity>(CalendarEventEntity.FieldEndTimeZoneOffset, new MappingResult("DTEND.TZID", typeof(string)));
			//PRIORITY
			RegisterMapProp<CalendarEventEntity>(CalendarEventEntity.FieldImportance, new MappingResult("PRIORITY", typeof(int)));
			//CLASS
			RegisterMapProp<CalendarEventEntity>(CalendarEventEntity.FieldSensitivy, new MappingResult("CLASS", typeof(int)));
			//SUMMARY
			RegisterMapProp<CalendarEventEntity>(CalendarEventEntity.FieldSubject, new MappingResult("SUMMARY", typeof(string)));
			//LOCATION
			RegisterMapProp<CalendarEventEntity>(CalendarEventEntity.FieldLocation, new MappingResult("LOCATION", typeof(string)));
			//DESCRIPTION
			RegisterMapProp<CalendarEventEntity>(CalendarEventEntity.FieldBody, new MappingResult("DESCRIPTION", typeof(string)));
			
			//UID
			RegisterMapProp<CalendarEventEntity>("PrimaryKeyId", new MappingResult("UID", typeof(PrimaryKeyId)));
			//Recurrence-ID
			RegisterMapProp<CalendarEventEntity>(CalendarEventEntity.FieldRecurrenceId, new MappingResult("RECURRENCE-ID", typeof(string)));

			//X-Properties 
			//STATUS
			RegisterMapProp<CalendarEventEntity>(CalendarEventEntity.FieldMeetingStatus, new MappingResult(McEvent.MEETING_STATUS_PROP, typeof(int)));
			//DELETED EXCEPTION
			RegisterMapProp<CalendarEventEntity>(CalendarEventEntity.FieldDeletedException, new MappingResult(McEvent.DELETED_EXCEPTION_PROP, typeof(bool)));
			//CALENDAR EVENT EXCEPTION ID
			RegisterMapProp<CalendarEventEntity>(CalendarEventEntity.FieldCalendarEventExceptionId, new MappingResult(McEvent.EXCEPTION_ID_PROP, typeof(PrimaryKeyId)));
			//PROJECT_ID
			RegisterMapProp<CalendarEventEntity>(CalendarEventEntity.FieldProjectId, new MappingResult(McEvent.PROJECT_ID_PROP, typeof(PrimaryKeyId)));
			
			//Custom properties
			//RegisterMapProp<CalendarEventEntity>(CalendarEventEntity.FieldAllDayEvent, new MappingResult("X-MC-ALLDAY", typeof(bool)));
			//RegisterMapProp<CalendarEventEntity>(CalendarEventEntity.FieldBusyStatus, new MappingResult("X-MC-BUSYSTATUS", typeof(bool)));


			//
			//CalendarEventRecurrenceEntity
			//
			RegisterMapProp<CalendarEventRecurrenceEntity>(CalendarEventRecurrenceEntity.FieldDayOfMonth, new MappingResult("ByMonthDay", typeof(int)));
			RegisterMapProp<CalendarEventRecurrenceEntity>(CalendarEventRecurrenceEntity.FieldDayOfWeekMask, new MappingResult("ByDay", typeof(int)));
			RegisterMapProp<CalendarEventRecurrenceEntity>(CalendarEventRecurrenceEntity.FieldInstance, new MappingResult("ByDay.Num", typeof(int)));
			RegisterMapProp<CalendarEventRecurrenceEntity>(CalendarEventRecurrenceEntity.FieldInterval, new MappingResult("Interval", typeof(int)));
			RegisterMapProp<CalendarEventRecurrenceEntity>(CalendarEventRecurrenceEntity.FieldMonthOfYear, new MappingResult("ByMonth", typeof(int)));
			RegisterMapProp<CalendarEventRecurrenceEntity>(CalendarEventRecurrenceEntity.FieldOccurrences, new MappingResult("Count", typeof(int)));
			RegisterMapProp<CalendarEventRecurrenceEntity>(CalendarEventRecurrenceEntity.FieldPatternEndDate, new MappingResult("Until", typeof(DateTime)));
			RegisterMapProp<CalendarEventRecurrenceEntity>(CalendarEventRecurrenceEntity.FieldRecurrenceType, new MappingResult("Frequency", typeof(int)));
			RegisterMapProp<CalendarEventRecurrenceEntity>(CalendarEventRecurrenceEntity.FieldEventId, new MappingResult("X-" + CalendarEventRecurrenceEntity.FieldEventId, typeof(PrimaryKeyId)));
			RegisterMapProp<CalendarEventRecurrenceEntity>(CalendarEventRecurrenceEntity.FieldTitle, new MappingResult("X-" + CalendarEventRecurrenceEntity.FieldTitle, typeof(string)));
			//
			//CalendarEventResourceEntity
			//
			RegisterMapProp<CalendarEventResourceEntity>(CalendarEventResourceEntity.FieldEmail, new MappingResult("Authority", typeof(string)));
			RegisterMapProp<CalendarEventResourceEntity>(CalendarEventResourceEntity.FieldContactId, new MappingResult("X-" + CalendarEventResourceEntity.FieldContactId, typeof(PrimaryKeyId)));
			RegisterMapProp<CalendarEventResourceEntity>(CalendarEventResourceEntity.FieldOrganizationId, new MappingResult("X-" + CalendarEventResourceEntity.FieldOrganizationId, typeof(PrimaryKeyId)));
			RegisterMapProp<CalendarEventResourceEntity>(CalendarEventResourceEntity.FieldPrincipalId, new MappingResult("X-"+ CalendarEventResourceEntity.FieldPrincipalId, typeof(PrimaryKeyId)));
			RegisterMapProp<CalendarEventResourceEntity>(CalendarEventResourceEntity.FieldResourceEventOrganizator, new MappingResult("X-" + CalendarEventResourceEntity.FieldResourceEventOrganizator, typeof(bool)));
			RegisterMapProp<CalendarEventResourceEntity>(CalendarEventResourceEntity.FieldStatus, new MappingResult("X-" + CalendarEventResourceEntity.FieldStatus, typeof(eResourceStatus)));
			RegisterMapProp<CalendarEventResourceEntity>(CalendarEventResourceEntity.FieldName, new MappingResult("X-" + CalendarEventResourceEntity.FieldName, typeof(string)));

		}

		private static void RegisterMapProp<T>(string entityName, MappingResult iCalName)
		{
			List<Dictionary<string, MappingResult>> map;
			if(!_mappingDef.TryGetValue(typeof(T), out map))
			{
				map = new List<Dictionary<string, MappingResult>>();
				map.Add(new Dictionary<string, MappingResult>());
				map.Add(new Dictionary<string, MappingResult>());
				_mappingDef.Add(typeof(T), map);
			}
			Dictionary<string, MappingResult> entity2iCal = map[0];
			Dictionary<string, MappingResult> iCal2Entity = map[1];

			entity2iCal.Add(entityName, iCalName);
			iCal2Entity.Add(iCalName.Name, new MappingResult(entityName, iCalName.ValueType));
		}

		public static MappingResult iCalProp2EntityProp<T>(string propName)
		{
			return FindProp(_mappingDef[typeof(T)][1], propName);
		}

		public static MappingResult EntityProp2iCalProp<T>(string propName)
		{
			return FindProp(_mappingDef[typeof(T)][0], propName);
		}

		private static MappingResult FindProp(Dictionary<string, MappingResult> map, string propName)
		{
			MappingResult retVal = null;
			foreach (string name in map.Keys)
			{
				if(name.Equals(propName, StringComparison.InvariantCultureIgnoreCase))
				{
					retVal = map[name];
					break;
				}
			}
			
			return retVal;
		}

	}
}
