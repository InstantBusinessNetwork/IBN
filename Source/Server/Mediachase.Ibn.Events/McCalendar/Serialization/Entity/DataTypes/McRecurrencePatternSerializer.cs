using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using System.Reflection;
using Mediachase.iCal.Components;
using Mediachase.iCal.DataTypes;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Events.McCalendar.DataTypes;


namespace Mediachase.Ibn.Events.McCalendar.Serialization.Entity.DataTypes
{
	public class McRecurrencePatternSerializer : IEntitySerializable
	{
		private RecurrencePattern _rPattern;

		public McRecurrencePatternSerializer(RecurrencePattern rPattern)
		{
			_rPattern = rPattern;
		}

		public RecurrencePattern RPattern
		{
			get
			{
				return _rPattern;
			}
		}
		#region IEntitySerializable Members

		public object Deserialize<T>(EntityObject entity)
		{
			//Primary key
			if (entity.PrimaryKeyId != null)
			{
				McRecurrencePattern mcRecurrencePattern = RPattern as McRecurrencePattern;
				if (mcRecurrencePattern != null)
				{
					mcRecurrencePattern.MetaObjectId = entity.PrimaryKeyId;
				}
			}

			foreach (EntityObjectProperty entityProp in entity.Properties)
			{
				iCal2EntityMapping.MappingResult mapRes = iCal2EntityMapping.EntityProp2iCalProp<T>(entityProp.Name);
				if (entityProp.Value == null || mapRes == null)
					continue;

				if(mapRes.Name.StartsWith("X-"))
				{
					RPattern.AddParameter(mapRes.Name, entityProp.Value.ToString());
					continue;
				}

				switch (mapRes.Name)
				{
					case "ByMonthDay":
						int monthDay = (int)entityProp.Value;
						if (monthDay != 0)
						{
							RPattern.ByMonthDay.Add(monthDay);
						}
						break;
					case "ByDay":
						if (((int)entityProp.Value & (byte)eBitDayOfWeek.Sunday) == (int)eBitDayOfWeek.Sunday)
							RPattern.ByDay.Add(new DaySpecifier(DayOfWeek.Sunday));
						if (((int)entityProp.Value & (byte)eBitDayOfWeek.Monday) == (int)eBitDayOfWeek.Monday)
							RPattern.ByDay.Add(new DaySpecifier(DayOfWeek.Monday));
						if (((int)entityProp.Value & (byte)eBitDayOfWeek.Tuesday) == (int)eBitDayOfWeek.Tuesday)
							RPattern.ByDay.Add(new DaySpecifier(DayOfWeek.Tuesday));
						if (((int)entityProp.Value & (byte)eBitDayOfWeek.Wednesday) == (int)eBitDayOfWeek.Wednesday)
							RPattern.ByDay.Add(new DaySpecifier(DayOfWeek.Wednesday));
						if (((int)entityProp.Value & (byte)eBitDayOfWeek.Thursday) == (byte)eBitDayOfWeek.Thursday)
							RPattern.ByDay.Add(new DaySpecifier(DayOfWeek.Thursday));
						if (((int)entityProp.Value & (byte)eBitDayOfWeek.Friday) == (byte)eBitDayOfWeek.Friday)
							RPattern.ByDay.Add(new DaySpecifier(DayOfWeek.Friday));
						if (((int)entityProp.Value & (byte)eBitDayOfWeek.Saturday) == (byte)eBitDayOfWeek.Saturday)
							RPattern.ByDay.Add(new DaySpecifier(DayOfWeek.Saturday));
						break;
					case "ByDay.Num":
						if (RPattern.ByDay.Count != 0)
						{
							int propVal = (int)entityProp.Value;
							if ((eInstanceType)propVal == eInstanceType.InstanceLast)
							{								
								propVal = -1;
							}

							RPattern.ByDay[0].Num = propVal;
						}
						break;
					case "Interval":
						RPattern.Interval = (int)entityProp.Value == 0 ? 1 : (int)entityProp.Value;
						break;
					case "ByMonth":
						int month = (int)entityProp.Value;
						if (month != 0)
						{
							RPattern.ByMonth.Add((int)entityProp.Value);
						}
						break;
					case "Count":
						//Fix bug in iCal RRULE.Count must be int.minValue if is 0. Else UNTIL not worked
						int rruleCount = (int)entityProp.Value;
						RPattern.Count = rruleCount == 0 ? int.MinValue : rruleCount; 
						break;
					case "Frequency":
						if ((eRecurrenceType)entityProp.Value == eRecurrenceType.RecursDaily)
							RPattern.Frequency = FrequencyType.Daily;
						else if((eRecurrenceType)entityProp.Value == eRecurrenceType.RecursWeekly)
							RPattern.Frequency = FrequencyType.Weekly;
						else if ((eRecurrenceType)entityProp.Value == eRecurrenceType.RecursMonthly)
							RPattern.Frequency = FrequencyType.Monthly;
						else if ((eRecurrenceType)entityProp.Value == eRecurrenceType.RecursYearly)
							RPattern.Frequency = FrequencyType.Yearly;
						else if ((eRecurrenceType)entityProp.Value == eRecurrenceType.RecursMonthNth)
							RPattern.Frequency = FrequencyType.Monthly;
						else if ((eRecurrenceType)entityProp.Value == eRecurrenceType.RecursYearNth)
							RPattern.Frequency = FrequencyType.Yearly;
						break;
					case "Until":
						RPattern.Until = new iCalDateTime((DateTime)entityProp.Value);
						RPattern.Until.IsUniversalTime = true;
						break;
				}

			}
			return RPattern;
		}


		/// <summary>
		/// Serializes this instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public EntityObjectHierarchy Serialize<T>()
		{
			CalendarEventRecurrenceEntity recurrenceEntity = new CalendarEventRecurrenceEntity();
			recurrenceEntity.DayOfMonth = RPattern.ByMonthDay.Count != 0 ? RPattern.ByMonthDay[0] : 0;
			eBitDayOfWeek dayOfWeekMask = eBitDayOfWeek.Unknown;
			eInstanceType instance  = eInstanceType.InstanceFirst;

			//Primary key
			McRecurrencePattern mcRecurrencePattern = RPattern as McRecurrencePattern;
			if(mcRecurrencePattern != null)
			{
				recurrenceEntity.PrimaryKeyId = mcRecurrencePattern.MetaObjectId;
			}
			//Copy RPATTERN Parameters to entity property
			foreach (string paramName in RPattern.Parameters.Keys)
			{
				iCal2EntityMapping.MappingResult mapRes = iCal2EntityMapping.iCalProp2EntityProp<T>(paramName);
				if (mapRes != null && RPattern.Parameters.ContainsKey(paramName))
				{
					foreach (string paramValue in ((Parameter)RPattern.Parameters[paramName]).Values)
					{
						recurrenceEntity[mapRes.Name] = EntityPropConverter.ToEntityProperty(mapRes.ValueType, paramValue);
					}
				
				}
			}
			
			foreach (DaySpecifier daySpec in RPattern.ByDay)
			{
				if (daySpec.Num != int.MinValue)
				{
					instance = (eInstanceType)daySpec.Num;
				}
				switch (daySpec.DayOfWeek)
				{
					case DayOfWeek.Sunday:
						dayOfWeekMask |= eBitDayOfWeek.Sunday;
						break;
					case DayOfWeek.Friday:
						dayOfWeekMask |= eBitDayOfWeek.Friday;
						break;
					case DayOfWeek.Monday:
						dayOfWeekMask |= eBitDayOfWeek.Monday;
						break;
					case DayOfWeek.Saturday:
						dayOfWeekMask |= eBitDayOfWeek.Saturday;
						break;
					case DayOfWeek.Thursday:
						dayOfWeekMask |= eBitDayOfWeek.Thursday;
						break;
					case DayOfWeek.Tuesday:
						dayOfWeekMask |= eBitDayOfWeek.Tuesday;
						break;
					case DayOfWeek.Wednesday:
						dayOfWeekMask |= eBitDayOfWeek.Wednesday;
						break;
				}
			}
			recurrenceEntity.DayOfWeekMask = (int)dayOfWeekMask;
			recurrenceEntity.Instance = (int)instance;
			recurrenceEntity.Interval = RPattern.Interval;

			eRecurrenceType recType = eRecurrenceType.RecursDaily;

			if (RPattern.Frequency == FrequencyType.Daily)
				recType = eRecurrenceType.RecursDaily;
			else if (RPattern.Frequency == FrequencyType.Weekly)
				recType = eRecurrenceType.RecursWeekly;
			else if (RPattern.Frequency == FrequencyType.Yearly)
				recType = eRecurrenceType.RecursYearly;
			//TODO: MOTH YEAR NTLR
			recurrenceEntity.RecurrenceType = (int)recType;
			recurrenceEntity.MonthOfYear = RPattern.ByMonth.Count != 0 ? RPattern.ByMonth[0] : 0;
			if(RPattern.Until != null)
			{
				recurrenceEntity.PatternEndDate = DateTime.SpecifyKind(RPattern.Until.Value, DateTimeKind.Local);
			}
			recurrenceEntity.Occurrences = RPattern.Count == int.MinValue ? 0 : RPattern.Count;

			return new EntityObjectHierarchy(recurrenceEntity);

		}
		#endregion
	}
}
