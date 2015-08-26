using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutlookAddin.OutlookItemAdaptors
{
	public class OutlookException : OutlookItem
	{
		private Outlook.Exception _oException;

		public OutlookException(OutlookListener listener, Outlook.Exception oException)
			: base(listener)
		{
			_oException = oException;
		}
		#region Properties
		public bool Deleted
		{
			get
			{
				return base.GetProp<bool>(_oException, "Deleted");
			}
		}
		public DateTime OriginalDate
		{
			get
			{
				return base.GetProp<DateTime>(_oException, "OriginalDate");
			}
		}
		public OutlookAppointment AppointmentItem
		{
			get
			{
				return base.GetProp<OutlookAppointment>(_oException, "AppointmentItem");
			}
		}
		#endregion

		public override object InnerOutlookObject
		{
			get { return _oException; }
		}
	}
}
