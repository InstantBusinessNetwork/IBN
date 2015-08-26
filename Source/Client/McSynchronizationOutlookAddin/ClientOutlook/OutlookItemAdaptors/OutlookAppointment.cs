using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OutlookAddin.OutlookItemAdaptors
{
	public class OutlookAppointment : OutlookItem
	{
		private Outlook._AppointmentItem _oAppItem;

		public OutlookAppointment(OutlookListener outlookListener, Outlook._AppointmentItem oAppItem)
			: base(outlookListener)
		{
			_oAppItem = oAppItem;
		}
		public override object InnerOutlookObject
		{
			get { return _oAppItem; }
		}

		#region Properties
		public bool IsRecurring
		{
			get
			{
				return GetProp<bool>(_oAppItem, "IsRecurring");
			}
			set
			{
				SetProp(_oAppItem, "IsRecurring", value);
			}
		}
		public string Categories
		{
			get
			{
				return GetProp<string>(_oAppItem, "Categories");
			}
			set
			{
				SetProp(_oAppItem, "Categories", value);
			}
		}


		public Outlook.OlImportance Importance
		{
			get
			{
				return GetProp<Outlook.OlImportance>(_oAppItem, "Importance");
			}
			set
			{
				SetProp(_oAppItem, "Importance", value);
			}
		}

		public Outlook.OlSensitivity Sensitivity
		{
			get
			{
				return GetProp<Outlook.OlSensitivity>(_oAppItem, "Sensitivity");
			}
			set
			{
				SetProp(_oAppItem, "Sensitivity", value);
			}
		}

		public string Subject
		{
			get
			{
				return GetProp<string>(_oAppItem, "Subject");
			}
			set
			{
				SetProp(_oAppItem, "Subject", value);
			}
		}

		public string Location
		{
			get
			{
				return GetProp<string>(_oAppItem, "Location");
			}
			set
			{
				SetProp(_oAppItem, "Location", value);
			}
		}

		public string Body
		{
			get
			{
				return GetProp<string>(_oAppItem, "Body");
			}
			set
			{
				SetProp(_oAppItem, "Body", value);
			}
		}

		public DateTime Start
		{
			get
			{
				return GetProp<DateTime>(_oAppItem, "Start");
			}
			set
			{
				SetProp(_oAppItem, "Start", value);
			}
		}

		public DateTime End
		{
			get
			{
				return GetProp<DateTime>(_oAppItem, "End");
			}
			set
			{
				SetProp(_oAppItem, "End", value);
			}
		}

		public DateTime LastModificationTime
		{
			get
			{
				return GetProp<DateTime>(_oAppItem, "LastModificationTime");
			}
			set
			{
				SetProp(_oAppItem, "LastModificationTime", value);
			}
		}
		public string EntryID
		{
			get
			{
				return GetProp<string>(_oAppItem, "EntryID");
			}

		}
		public IEnumerable<OutlookRecipient> Recipients
		{
			get
			{
				return base._outlookListener.GetAppointmentRecipients(_oAppItem);
			}
		}
		#endregion


		#region Methods
		public void Save()
		{
			base._outlookListener.AppointmentSave(_oAppItem);
		}

		public void Delete()
		{
			base._outlookListener.AppointmentDelete(_oAppItem);

		}

		/// <summary>
		/// Clears the recurrence pattern.
		/// </summary>
		public void ClearRecurrencePattern()
		{
			base._outlookListener.AppointmentClearRecurrencePattern(_oAppItem);
		}

		/// <summary>
		/// Removes the recipient.
		/// </summary>
		/// <param name="index">The index.</param>
		public void RemoveRecipient(int index)
		{
			base._outlookListener.AppointmentRemoveRecipient(_oAppItem, index);
		}
		/// <summary>
		/// Adds the recipient.
		/// </summary>
		/// <param name="recipientName">Name of the recipient.</param>
		/// <returns></returns>
		public OutlookRecipient AddRecipient(string recipientName)
		{
			return base._outlookListener.AddAppointmentRecipient(_oAppItem, recipientName);
		}
		/// <summary>
		/// Gets the recurrence pattern.
		/// </summary>
		/// <returns></returns>
		public OutlookRecurrencePattern GetRecurrencePattern()
		{
			return base._outlookListener.GetRecurrencePattern(_oAppItem) as OutlookRecurrencePattern;
		}
		#endregion
	}
}
