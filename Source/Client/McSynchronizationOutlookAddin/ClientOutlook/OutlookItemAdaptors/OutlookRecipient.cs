using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutlookAddin.OutlookItemAdaptors
{
	public class OutlookRecipient : OutlookItem
	{
		private Outlook.Recipient _oRecipient;

		public OutlookRecipient(OutlookListener listener, Outlook.Recipient oRecipient)
			: base(listener)
		{
			_oRecipient = oRecipient;
		}

		#region Properties
		public string Address
		{
			get { return GetProp<string>(_oRecipient, "Address"); }
		}

		#endregion
		#region Overrides
		public override object InnerOutlookObject
		{
			get { return _oRecipient; }
		}
		#endregion
	}
}
