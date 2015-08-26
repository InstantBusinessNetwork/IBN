using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Sync.Core.Common;
using System.Windows.Forms;

namespace OutlookAddin.OutlookItemAdaptors
{
	public class OutlookItemFactory : AbstractFactory, IFactoryMethod<OutlookItem>
									  
	{
		private OutlookListener _listener;

		public OutlookItemFactory(OutlookListener listener)
		{
			_listener = listener;
		}
		#region IFactoryMethod<OutlookItem> Members

		public OutlookItem Create(object obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			OutlookItem retVal = null;

			Outlook._AppointmentItem appItem = obj as Outlook._AppointmentItem;
			if(obj is Outlook._AppointmentItem)
			{
				retVal = new OutlookAppointment(_listener, (Outlook._AppointmentItem)obj);
			}
			else if (obj is Outlook.RecurrencePattern)
			{
				retVal = new OutlookRecurrencePattern(_listener, (Outlook.RecurrencePattern)obj);
			}
			else if(obj is Outlook.Exception)
			{
				retVal = new OutlookException(_listener, (Outlook.Exception)obj);
			}
			else if(obj is Outlook.Recipient)
			{
				retVal = new OutlookRecipient(_listener, (Outlook.Recipient)obj);
			}
			else if (obj is Outlook.MAPIFolder)
			{
				retVal = new OutlookFolder(_listener, (Outlook.MAPIFolder)obj);
			}

			return retVal;
		}

		#endregion
	}
}
