using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Mediachase.Ibn.Events
{
	[DebuggerDisplay("{PrimaryKeyId} - {Subject} ({Start} - {End})")]
	public partial class CalendarEventEntity : IComparable<CalendarEventEntity>
	{
		#region Util
		public const string FieldRecurrenceId = "RecurrenceId";
		public static string[] ComparedProperties = { 
											   CalendarEventEntity.FieldBody, 
					                           CalendarEventEntity.FieldBusyStatus,
					                           CalendarEventEntity.FieldCategories,
					                           CalendarEventEntity.FieldEndTimeZoneOffset,
					                           CalendarEventEntity.FieldStartTimeZoneOffset,
					                           CalendarEventEntity.FieldImportance,
					                           CalendarEventEntity.FieldLocation,
					                           CalendarEventEntity.FieldMeetingStatus,
											   CalendarEventEntity.FieldSensitivy,
											   CalendarEventEntity.FieldSubject
											 };
		#endregion

		public System.DateTime RecurrenceId
		{
			get
			{
				return base.Properties.GetValue<DateTime>(FieldRecurrenceId, DateTime.MinValue);
			}

			set
			{

				if (base.Properties.Contains(FieldRecurrenceId))
				{
					base.Properties[FieldRecurrenceId].Value = value;
				}
				else
				{
					base.Properties.Add(FieldRecurrenceId, value);
				}
			}

		}

		#region IComparable<CalendarEventEntity> Members

		public int CompareTo(CalendarEventEntity other)
		{
			if (Start != null && other.Start != null)
			{
				if (Start == other.Start)
					return 0;
				else if (Start < other.Start)
					return -1;
				else if (Start > other.Start)
					return 1;
			}

			return 0;
		}

		#endregion
	}
}
