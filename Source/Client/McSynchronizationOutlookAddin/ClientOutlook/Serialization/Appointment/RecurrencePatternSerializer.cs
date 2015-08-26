using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Sync.Core;
using Mediachase.Sync.Core.TransferDataType;
using OutlookAddin.OutlookItemAdaptors;

namespace Mediachase.ClientOutlook.SyncTransferedData.Appointment
{
	public class RecurrencePatternSerializer : ITransferDataSerializable
	{
		private OutlookRecurrencePattern _rPattern;
		private static Dictionary<Outlook.OlRecurrenceType, IEnumerable<string>> _recurTypeFieldMap = 
										new Dictionary<Outlook.OlRecurrenceType, IEnumerable<string>>();

		static RecurrencePatternSerializer()
		{
			//olRecursDaily Interval, NoEndDate, Occurrences, PatternEndDate
			_recurTypeFieldMap.Add(Outlook.OlRecurrenceType.olRecursDaily, new string[] {
																						  RecurrencePatternTransferData.FieldInterval,
																						  RecurrencePatternTransferData.FieldNoEndDate,
																						  RecurrencePatternTransferData.FieldOccurrences,
																						  RecurrencePatternTransferData.FieldPatternEndDate
																						});
			//olRecursWeekly DayOfWeekMask, Interval, NoEndDate, Occurrences, PatternEndDate
			_recurTypeFieldMap.Add(Outlook.OlRecurrenceType.olRecursWeekly, new string[] {
																							RecurrencePatternTransferData.FieldDayOfWeekMask,
																							RecurrencePatternTransferData.FieldInterval,
																							RecurrencePatternTransferData.FieldNoEndDate,
																							RecurrencePatternTransferData.FieldOccurrences,
																							RecurrencePatternTransferData.FieldPatternEndDate
																						});
			//olRecursMonthly DayOfMonth, Interval, NoEndDate, Occurrences,  PatternEndDate
			_recurTypeFieldMap.Add(Outlook.OlRecurrenceType.olRecursMonthly, new string[] {
																							RecurrencePatternTransferData.FieldDayOfMonth,
																							RecurrencePatternTransferData.FieldInterval,
																							RecurrencePatternTransferData.FieldNoEndDate,
																							RecurrencePatternTransferData.FieldOccurrences,
																							RecurrencePatternTransferData.FieldPatternEndDate
																							});
			//olRecursMonthNth DayOfWeekMask, Interval, Instance, NoEndDate, Occurrences, PatternEndDate
			_recurTypeFieldMap.Add(Outlook.OlRecurrenceType.olRecursMonthNth, new string[] {
																							RecurrencePatternTransferData.FieldDayOfWeekMask,
																							RecurrencePatternTransferData.FieldInterval,
																							RecurrencePatternTransferData.FieldInstance,
																							RecurrencePatternTransferData.FieldNoEndDate,
																							RecurrencePatternTransferData.FieldOccurrences,
																							RecurrencePatternTransferData.FieldPatternEndDate
																							});
			//olRecursYearly DayOfMonth, Interval, MonthOfYear, NoEndDate, Occurrences, PatternEndDate
			_recurTypeFieldMap.Add(Outlook.OlRecurrenceType.olRecursYearly, new string[] {
																							RecurrencePatternTransferData.FieldDayOfMonth,
																							RecurrencePatternTransferData.FieldMonthOfYear,
																							RecurrencePatternTransferData.FieldInterval,
																							RecurrencePatternTransferData.FieldNoEndDate,
																							RecurrencePatternTransferData.FieldOccurrences,
																							RecurrencePatternTransferData.FieldPatternEndDate
																						});

			//olRecursYearNth DayOfWeekMask, Interval, Instance, NoEndDate, Occurrences, PatternEndDate 
			_recurTypeFieldMap.Add(Outlook.OlRecurrenceType.olRecursYearNth, new string[] {
																							RecurrencePatternTransferData.FieldDayOfWeekMask,
																							RecurrencePatternTransferData.FieldInstance,
																							RecurrencePatternTransferData.FieldInterval,
																							RecurrencePatternTransferData.FieldNoEndDate,
																							RecurrencePatternTransferData.FieldOccurrences,
																							RecurrencePatternTransferData.FieldPatternEndDate
																						});
		}

		public RecurrencePatternSerializer(OutlookRecurrencePattern rPattern)
		{
			_rPattern = rPattern;
		}

		public OutlookRecurrencePattern RPattern
		{
			get
			{
				return _rPattern;
			}
		}
		#region IEntitySerializable Members
		/// <summary>
		/// Преобразует TranferData в объект Outlook RecurrencePattern
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public object Deserialize(SyncTransferData data)
		{
			TransferProperty2OutlookProperty(RecurrencePatternTransferData.FieldRecurrenceType, data);
			IEnumerable<string> recurProp;
			if(!_recurTypeFieldMap.TryGetValue(RPattern.RecurrenceType, out recurProp))
			{
				throw new ArgumentException("Invalid recurrence pattern");
			}

			foreach (string property in recurProp)
			{
				TransferProperty2OutlookProperty(property, data);
			}

			return RPattern;
		}

		/// <summary>
		/// Transfers the property2 outlook property.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="data">The data.</param>
		private void TransferProperty2OutlookProperty(string propertyName, SyncTransferData data)
		{
			object propValue = data.Properties[propertyName];
			if (propValue == null)
				return;

			if (propertyName == RecurrencePatternTransferData.FieldRecurrenceType)
			{
				eRecurrenceType recType = (eRecurrenceType)(int)propValue;
				switch (recType)
				{
					case eRecurrenceType.RecursDaily:
						RPattern.RecurrenceType = Outlook.OlRecurrenceType.olRecursDaily;
						break;
					case eRecurrenceType.RecursMonthly:
						RPattern.RecurrenceType = Outlook.OlRecurrenceType.olRecursMonthly;
						break;
					case eRecurrenceType.RecursMonthNth:
						RPattern.RecurrenceType = Outlook.OlRecurrenceType.olRecursMonthNth;
						break;
					case eRecurrenceType.RecursWeekly:
						RPattern.RecurrenceType = Outlook.OlRecurrenceType.olRecursWeekly;
						break;
					case eRecurrenceType.RecursYearly:
						RPattern.RecurrenceType = Outlook.OlRecurrenceType.olRecursYearly;
						break;
					case eRecurrenceType.RecursYearNth:
						RPattern.RecurrenceType = Outlook.OlRecurrenceType.olRecursYearNth;
						break;
				}
			} 
			else if (propertyName == RecurrencePatternTransferData.FieldDayOfWeekMask)
			{
				eBitDayOfWeek dayOfWeekMask = (eBitDayOfWeek)(int)propValue;
				RPattern.DayOfWeekMask = (Outlook.OlDaysOfWeek)(int)dayOfWeekMask;
			}
			else if (propertyName == RecurrencePatternTransferData.FieldPatternEndDate)
			{
				RPattern.PatternEndDate = (DateTime)propValue;
			}
			else if (propertyName == RecurrencePatternTransferData.FieldDayOfMonth)
			{
				RPattern.DayOfMonth = (int)propValue;
			}
			else if (propertyName == RecurrencePatternTransferData.FieldInstance)
			{
				RPattern.Instance = (int)propValue;
			}
			else if (propertyName == RecurrencePatternTransferData.FieldInterval)
			{
				RPattern.Interval = (int)propValue;
			}
			else if (propertyName == RecurrencePatternTransferData.FieldMonthOfYear)
			{
				RPattern.MonthOfYear = (int)propValue;
			}
			else if (propertyName == RecurrencePatternTransferData.FieldOccurrences)
			{
				RPattern.Occurrences = (int)propValue;
			}
		}
		/// <summary>
		/// Serializes this instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public SyncTransferData Serialize()
		{

			RecurrencePatternTransferData retVal = new RecurrencePatternTransferData();
			switch (RPattern.RecurrenceType)
			{
				case Outlook.OlRecurrenceType.olRecursDaily:
					retVal.RecurrenceType = (int)eRecurrenceType.RecursDaily;
					break;
				case Outlook.OlRecurrenceType.olRecursMonthly:
					retVal.RecurrenceType = (int)eRecurrenceType.RecursMonthly;
					break;
				case Outlook.OlRecurrenceType.olRecursMonthNth:
					retVal.RecurrenceType = (int)eRecurrenceType.RecursMonthNth;
					break;
				case Outlook.OlRecurrenceType.olRecursWeekly:
					retVal.RecurrenceType = (int)eRecurrenceType.RecursWeekly;
					break;
				case Outlook.OlRecurrenceType.olRecursYearly:
					retVal.RecurrenceType = (int)eRecurrenceType.RecursYearly;
					break;
				case Outlook.OlRecurrenceType.olRecursYearNth:
					retVal.RecurrenceType = (int)eRecurrenceType.RecursYearNth;
					break;
			}
			
			retVal.DayOfWeekMask = (int)RPattern.DayOfWeekMask;

			retVal.MonthOfYear = RPattern.MonthOfYear;
			retVal.DayOfMonth = RPattern.DayOfMonth;
			if (RPattern.Instance != 0)
			{
				retVal.Instance = RPattern.Instance;
			}
			retVal.Interval = RPattern.Interval;

			retVal.Occurrences = RPattern.Occurrences;
			if (retVal.Occurrences == 0)
			{
				retVal.PatternEndDate = RPattern.PatternEndDate;
			}

			return retVal;
		}
		#endregion
	}
}
