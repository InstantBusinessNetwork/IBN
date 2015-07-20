using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Events.McCalendar.Components;
using Mediachase.Ibn.Core.Business;
using Mediachase.iCal.DataTypes;
using System.Reflection;
using Mediachase.iCal.Components;
using Mediachase.iCal.Serialization;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Events.McCalendar.Serialization.Entity.DataTypes
{
	public class McEventSerializer : IEntitySerializable
	{
		private McEvent _event;

		public McEventSerializer(McEvent mcEvent)
		{
			_event = mcEvent;
		}

		public McEvent InnerEvent
		{
			get
			{
				return _event;
			}
		}

		virtual protected IEnumerable<object> FieldsAndProperties
		{
			get
			{
				foreach (PropertyInfo pi in InnerEvent.GetType().GetProperties())
					if (pi.GetCustomAttributes(typeof(SerializedAttribute), true).Length > 0)
					{
						yield return pi;
					}
				foreach (Property property in InnerEvent.Properties)
				{
					yield return property;
				}
			}
		}
	

		private object ReflectioniCalPropValue(object itemInfo, out string itemName)
		{
			object retVal = null;

			PropertyInfo prop = null;
			Type itemType = null;
			itemName = null;
			object[] itemAttrs = null;
			prop = (PropertyInfo)itemInfo;
			itemType = prop.PropertyType;
			itemName = prop.Name;

			// Get attributes that are attached to each item
			itemAttrs =  prop.GetCustomAttributes(true);
			// Get the item's value
			retVal =  prop.GetValue(InnerEvent, null);

			// Retrieve custom attributes for this field/property
			if (retVal is iCalDataType)
			{
				((iCalDataType)retVal).Attributes = itemAttrs;
			}

			return retVal;
		}

		#region IEntitySerializable Members

		public virtual EntityObjectHierarchy Serialize<T>()
		{
			CalendarEventEntity eventEntity = new CalendarEventEntity();
			//Primary key serialize
			if (InnerEvent.UID != null)
			{
				eventEntity.PrimaryKeyId = PrimaryKeyId.Parse(InnerEvent.UID.ToString());
			}
			//Calculated field
			if(InnerEvent.IsReccuring)
			{
				eventEntity.IsRecurring = true;
			}
			//not standart iCal Property serialize to entity property
			foreach (Property property in InnerEvent.Properties.Values)
			{
				iCal2EntityMapping.MappingResult mapRes = iCal2EntityMapping.iCalProp2EntityProp<T>(property.Name);
				if (mapRes != null)
				{
					eventEntity[mapRes.Name] = EntityPropConverter.ToEntityProperty(mapRes.ValueType, property.Value);
				}
			}
			//Copy exist iCal type property to entity object
			//CATEGORIES
			if (InnerEvent.Categories != null)
			{
				foreach (TextCollection category in InnerEvent.Categories)
				{
					eventEntity.Categories = (eventEntity.Categories + "," + category.ToString()).TrimStart(',');
				}
			}
			//PRIORITY
			if (InnerEvent.Priority != null)
			{
				eventEntity.Importance = InnerEvent.Priority;
			}
			//CLASS
			eventEntity.Sensitivy = (int)eSesitivity.Normal;
			if (InnerEvent.Class != null)
			{
				eventEntity.Sensitivy = Convert.ToInt32(InnerEvent.Class.ToString());
			}
			//SUMMARY
			if (InnerEvent.Summary != null)
			{
				eventEntity.Subject = InnerEvent.Summary;
			}
			//LOCATION
			if (InnerEvent.Location != null)
			{
				eventEntity.Location = InnerEvent.Location;
			}
			//DESCRIPTION
			if (InnerEvent.Description != null)
			{
				eventEntity.Body = InnerEvent.Description;
			}
			
			//Type properties serialized always in local time
			//DTSTART
			eventEntity.Start = InnerEvent.DTStart.Value;
			//DTEND
			eventEntity.End = InnerEvent.DTEnd.Value;

			//RECURRENCE ID
			if (InnerEvent.Recurrence_ID != null)
			{
				eventEntity.RecurrenceId = InnerEvent.Recurrence_ID.Value;
			}

			return new EntityObjectHierarchy(eventEntity);
		}


		public object Deserialize<T>(Mediachase.Ibn.Core.Business.EntityObject entity)
		{
			if(entity.PrimaryKeyId != null)
			{
				InnerEvent.UID = new Text(entity.PrimaryKeyId.Value.ToString());
			}
			foreach (EntityObjectProperty entityProp in entity.Properties)
			{
				iCal2EntityMapping.MappingResult mapRes = iCal2EntityMapping.EntityProp2iCalProp<T>(entityProp.Name);
				//Пропускаем пустые свойства
				if (entityProp.Value == null || mapRes == null)
					continue;

				if (mapRes.Name.StartsWith("X-"))
				{
					InnerEvent.AddProperty(mapRes.Name, entityProp.Value.ToString());
					continue;
				}

				switch (mapRes.Name)
				{
					case "CATEGORIES":
						InnerEvent.Categories = new TextCollection[] { new TextCollection(entityProp.Value.ToString()) };
						break;

					case "DTSTART":
						InnerEvent.DTStart = (DateTime)entityProp.Value;
						break;

					case "DTEND":
						InnerEvent.DTEnd = (DateTime)entityProp.Value;
						break;

					case "DTSTART.TZID":
					case "DTEND.TZID":
						if (entityProp.Name == CalendarEventEntity.FieldEndTimeZoneOffset)
						{
							InnerEvent.DTEnd.AddParameter(mapRes.Name, entityProp.Value.ToString());
						}
						else
						{
							InnerEvent.DTStart.AddParameter(mapRes.Name, entityProp.Value.ToString());
						}
						break;
					case "PRIORITY":
						InnerEvent.Priority = new Integer((int)entityProp.Value);
						break;

					case "CLASS":
						InnerEvent.Class = new Text(entityProp.Value.ToString());
						break;
					case "SUMMARY":
						InnerEvent.Summary = new Text(entityProp.Value.ToString());
						break;
					case "LOCATION":
						InnerEvent.Location = new Text(entityProp.Value.ToString());
						break;
					case "DESCRIPTION":
						InnerEvent.Description = new Text(entityProp.Value.ToString());
						break;
					case "RECURRENCE-ID":
						InnerEvent.Recurrence_ID = new iCalDateTime(entityProp.Value.ToString());
						break;
				}

			}
			return InnerEvent;
		}
		#endregion
	}
}
