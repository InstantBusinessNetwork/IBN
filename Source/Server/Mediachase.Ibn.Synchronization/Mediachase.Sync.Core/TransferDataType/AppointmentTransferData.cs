using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Sync.Core.TransferDataType
{
	[Serializable]
	public class AppointmentTransferData : SyncTransferData
	{
		public const string DataName = "Appointment";

		public const string FieldAllDayEvent = "AllDayEvent";
		public const string FieldBody = "Body";
		public const string FieldBusyStatus = "BusyStatus";
		public const string FieldCalendarEventException = "CalendarEventException";
		public const string FieldCalendarEventExceptionId = "CalendarEventExceptionId";
		public const string FieldCategories = "Categories";
		public const string FieldCreated = "Created";
		public const string FieldCreatorId = "CreatorId";
		public const string FieldDeletedException = "DeletedException";
		public const string FieldDuration = "Duration";
		public const string FieldEnd = "End";
		public const string FieldEndTimeZoneOffset = "EndTimeZoneOffset";
		public const string FieldImportance = "Importance";
		public const string FieldIsRecurring = "IsRecurring";
		public const string FieldLocation = "Location";
		public const string FieldMeetingStatus = "MeetingStatus";
		public const string FieldModified = "Modified";
		public const string FieldModifierId = "ModifierId";
		public const string FieldProject = "Project";
		public const string FieldProjectId = "ProjectId";
		public const string FieldSensitivy = "Sensitivy";
		public const string FieldSequence = "Sequence";
		public const string FieldStart = "Start";
		public const string FieldStartTimeZoneOffset = "StartTimeZoneOffset";
		public const string FieldSubject = "Subject";

		public const string FieldRecurrenceId = "RecurrenceId";

		public AppointmentTransferData()
			: base(DataName)
		{
			InitializeProperties();
		}

		protected void InitializeProperties()
		{
			base.Properties.Add(FieldAllDayEvent, null);
			base.Properties.Add(FieldBody, null);
			base.Properties.Add(FieldBusyStatus, null);
			base.Properties.Add(FieldCalendarEventException, null);
			base.Properties.Add(FieldCategories, null);
			base.Properties.Add(FieldCreated, null);
			base.Properties.Add(FieldDeletedException, null);
			base.Properties.Add(FieldDuration, null);
			base.Properties.Add(FieldEnd, null);
			base.Properties.Add(FieldEndTimeZoneOffset, null);
			base.Properties.Add(FieldImportance, null);
			base.Properties.Add(FieldIsRecurring, null);
			base.Properties.Add(FieldLocation, null);
			base.Properties.Add(FieldMeetingStatus, null);
			base.Properties.Add(FieldProject, null);
			base.Properties.Add(FieldSensitivy, null);
			base.Properties.Add(FieldSequence, null);
			base.Properties.Add(FieldStart, null);
			base.Properties.Add(FieldStartTimeZoneOffset, null);
			base.Properties.Add(FieldSubject, null);

			base.Properties.Add(FieldRecurrenceId, null);
		
		}

		#region Named Properties
		public DateTime RecurrenceId
		{
			get
			{
				return (System.DateTime)base.Properties[FieldRecurrenceId];
			}

			set
			{
				base.Properties[FieldRecurrenceId] = value;
			}
		}
		public System.Boolean AllDayEvent
		{
			get
			{
				return (System.Boolean)base.Properties[FieldAllDayEvent];
			}

			set
			{
				base.Properties[FieldAllDayEvent] = value;
			}

		}

		public System.String Body
		{
			get
			{
				return (System.String)base.Properties[FieldBody];
			}

			set
			{
				base.Properties[FieldBody] = value;
			}

		}

		public System.Boolean BusyStatus
		{
			get
			{
				return (System.Boolean)base.Properties[FieldBusyStatus];
			}

			set
			{
				base.Properties[FieldBusyStatus] = value;
			}

		}

		public System.String CalendarEventException
		{
			get
			{
				return (System.String)base.Properties[FieldCalendarEventException];
			}

		}

		public System.String Categories
		{
			get
			{
				return (System.String)base.Properties[FieldCategories];
			}

			set
			{
				base.Properties[FieldCategories] = value;
			}

		}

		public System.DateTime Created
		{
			get
			{
				return (System.DateTime)base.Properties[FieldCreated];
			}

			set
			{
				base.Properties[FieldCreated] = value;
			}

		}

		public System.Int32 CreatorId
		{
			get
			{
				return (System.Int32)base.Properties[FieldCreatorId];
			}

			set
			{
				base.Properties[FieldCreatorId] = value;
			}

		}

		public System.Boolean DeletedException
		{
			get
			{
				return (System.Boolean)base.Properties[FieldDeletedException];
			}

			set
			{
				base.Properties[FieldDeletedException] = value;
			}

		}

		public Nullable<System.Int32> Duration
		{
			get
			{
				return (Nullable<System.Int32>)base.Properties[FieldDuration];
			}

			set
			{
				base.Properties[FieldDuration] = value;
			}

		}

		public System.DateTime End
		{
			get
			{
				return (System.DateTime)base.Properties[FieldEnd];
			}

			set
			{
				base.Properties[FieldEnd] = value;
			}

		}

		public Nullable<System.Int32> EndTimeZoneOffset
		{
			get
			{
				return (Nullable<System.Int32>)base.Properties[FieldEndTimeZoneOffset];
			}

			set
			{
				base.Properties[FieldEndTimeZoneOffset] = value;
			}

		}

		public Nullable<System.Int32> Importance
		{
			get
			{
				return (Nullable<System.Int32>)base.Properties[FieldImportance];
			}

			set
			{
				base.Properties[FieldImportance] = value;
			}

		}

		public System.Boolean IsRecurring
		{
			get
			{
				return (System.Boolean)base.Properties[FieldIsRecurring];
			}

			set
			{
				base.Properties[FieldIsRecurring] = value;
			}

		}

		public System.String Location
		{
			get
			{
				return (System.String)base.Properties[FieldLocation];
			}

			set
			{
				base.Properties[FieldLocation] = value;
			}

		}

		public System.Int32 MeetingStatus
		{
			get
			{
				return (System.Int32)base.Properties[FieldMeetingStatus];
			}

			set
			{
				base.Properties[FieldMeetingStatus] = value;
			}

		}

		public System.DateTime Modified
		{
			get
			{
				return (System.DateTime)base.Properties[FieldModified];
			}

			set
			{
				base.Properties[FieldModified] = value;
			}

		}

		public System.Int32 ModifierId
		{
			get
			{
				return (System.Int32)base.Properties[FieldModifierId];
			}

			set
			{
				base.Properties[FieldModifierId] = value;
			}

		}

		public System.String Project
		{
			get
			{
				return (System.String)base.Properties[FieldProject];
			}

		}

	
		public System.Int32 Sensitivy
		{
			get
			{
				return (System.Int32)base.Properties[FieldSensitivy];
			}

			set
			{
				base.Properties[FieldSensitivy] = value;
			}

		}

		public System.Int32 Sequence
		{
			get
			{
				return (System.Int32)base.Properties[FieldSequence];
			}

			set
			{
				base.Properties[FieldSequence] = value;
			}

		}

		public System.DateTime Start
		{
			get
			{
				return (System.DateTime)base.Properties[FieldStart];
			}

			set
			{
				base.Properties[FieldStart] = value;
			}

		}

		public Nullable<System.Int32> StartTimeZoneOffset
		{
			get
			{
				return (Nullable<System.Int32>)base.Properties[FieldStartTimeZoneOffset];
			}

			set
			{
				base.Properties[FieldStartTimeZoneOffset] = value;
			}

		}

		public System.String Subject
		{
			get
			{
				return (System.String)base.Properties[FieldSubject];
			}

			set
			{
				base.Properties[FieldSubject] = value;
			}

		}

		#endregion

	}
}
