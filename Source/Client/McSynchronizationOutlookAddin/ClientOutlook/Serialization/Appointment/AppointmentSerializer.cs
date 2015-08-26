using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Sync.Core;
using Mediachase.Sync.Core.TransferDataType;
using OutlookAddin.OutlookItemAdaptors;

namespace Mediachase.ClientOutlook.SyncTransferedData.Appointment
{
	public class AppointmentSerializer : ITransferDataSerializable
	{
		private OutlookAppointment  _appItem;

		public AppointmentSerializer(OutlookAppointment appItem)
		{
			_appItem = appItem;
		}

		public OutlookAppointment InnerAppointment
		{
			get
			{
				return _appItem;
			}
		}

		#region IEntitySerializable Members

		public virtual SyncTransferData Serialize()
		{
			AppointmentTransferData appointment = new AppointmentTransferData();
			//Calculated field
			if (InnerAppointment.IsRecurring)
			{
				appointment.IsRecurring = true;
			}
			//Copy exist iCal type property to entity object
			//CATEGORIES
			if (InnerAppointment.Categories != null)
			{
				appointment.Categories = InnerAppointment.Categories;
			}
			//PRIORITY
			//TODO: check int value
			switch (InnerAppointment.Importance)
			{
				case Outlook.OlImportance.olImportanceLow:
					appointment.Importance = (int)eImportance.Low;
					break;
				case Outlook.OlImportance.olImportanceNormal:
					appointment.Importance = (int)eImportance.Normal;
					break;
				case Outlook.OlImportance.olImportanceHigh:
					appointment.Importance = (int)eImportance.High;
					break;
			}

			//CLASS
			//TDOD: check int value
			switch (InnerAppointment.Sensitivity)
			{
				case Outlook.OlSensitivity.olConfidential:
					appointment.Sensitivy = (int)eSesitivity.Confidential;
					break;
				case Outlook.OlSensitivity.olNormal:
					appointment.Sensitivy = (int)eSesitivity.Normal;
					break;
				case Outlook.OlSensitivity.olPersonal:
					appointment.Sensitivy = (int)eSesitivity.Personal;
					break;
				case Outlook.OlSensitivity.olPrivate:
					appointment.Sensitivy = (int)eSesitivity.Private;
					break;
			}
			//SUMMARY
			appointment.Subject = InnerAppointment.Subject;

			//LOCATION
			appointment.Location = InnerAppointment.Location;

			//DESCRIPTION
			appointment.Body = InnerAppointment.Body;

			//Type properties serialized always in local time
			//DTSTART
			appointment.Start = InnerAppointment.Start;
			//DTEND
			appointment.End = InnerAppointment.End;

			//URI
			appointment.Uri = InnerAppointment.EntryID;
			appointment.LastModified = (ulong)InnerAppointment.LastModificationTime.Ticks;
			//Last modified

			return appointment;
		}


		public object Deserialize(SyncTransferData data)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			AppointmentTransferData appData = data as AppointmentTransferData;
			if (appData == null)
				throw new NullReferenceException("appData");

			bool isException = appData.RecurrenceId != DateTime.MinValue;

			object propVal = data.Properties[AppointmentTransferData.FieldImportance];
			if(propVal != null)
			{
				eImportance importance = (eImportance)(int)propVal;
				switch (importance)
				{
					case eImportance.Low:
						InnerAppointment.Importance = Outlook.OlImportance.olImportanceLow;
						break;
					case eImportance.Normal:
						InnerAppointment.Importance = Outlook.OlImportance.olImportanceNormal;
						break;
					case eImportance.High:
						InnerAppointment.Importance = Outlook.OlImportance.olImportanceHigh;
						break;
				}
				
			}

			InnerAppointment.Start = (DateTime)data.Properties[AppointmentTransferData.FieldStart];
			InnerAppointment.End = (DateTime)data.Properties[AppointmentTransferData.FieldEnd];
			InnerAppointment.Subject = (string)data.Properties[AppointmentTransferData.FieldSubject];
			InnerAppointment.Location = (string)data.Properties[AppointmentTransferData.FieldLocation];
			InnerAppointment.Body = (string)data.Properties[AppointmentTransferData.FieldBody];

			//case "DTSTART.TZID":
			//case "DTEND.TZID":

			//Для exception данный свойства модифицировать нельзя
			if (!isException)
			{
				InnerAppointment.Categories = (string)data.Properties[AppointmentTransferData.FieldCategories];

				propVal = data.Properties[AppointmentTransferData.FieldSensitivy];
				if (propVal != null)
				{
					eSesitivity sensitivity = (eSesitivity)(int)data.Properties[AppointmentTransferData.FieldSensitivy];
					switch (sensitivity)
					{
						case eSesitivity.Confidential:
							InnerAppointment.Sensitivity = Outlook.OlSensitivity.olConfidential;
							break;
						case eSesitivity.Normal:
							InnerAppointment.Sensitivity = Outlook.OlSensitivity.olNormal;
							break;
						case eSesitivity.Personal:
							InnerAppointment.Sensitivity = Outlook.OlSensitivity.olPersonal;
							break;
						case eSesitivity.Private:
							InnerAppointment.Sensitivity = Outlook.OlSensitivity.olPrivate;
							break;
					}
				}

			}
			return InnerAppointment;
		}


		#endregion
	}
}
