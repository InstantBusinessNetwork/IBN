using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Sync.Core;
using Mediachase.Sync.Core.TransferDataType;
using OutlookAddin.OutlookItemAdaptors;

namespace Mediachase.ClientOutlook.SyncTransferedData.Appointment
{
	public class RecipientSerializer : ITransferDataSerializable
	{
		private OutlookRecipient _recipient;

		public RecipientSerializer()
		{
		}

		public RecipientSerializer(OutlookRecipient recipient)
		{
			_recipient = recipient;
		}

		public OutlookRecipient Recipient
		{
			get
			{
				return _recipient;
			}
		}
		#region IEntitySerializable Members

		public SyncTransferData Serialize()
		{
			RecipientTransferData retVal = new RecipientTransferData();
			retVal.Email = Recipient.Address;
		
			return retVal;
		}

		public object Deserialize(SyncTransferData data)
		{
			RecipientTransferData recipTransfData = data as RecipientTransferData;
			string retVal = null;
			if (recipTransfData != null)
			{
				if (!string.IsNullOrEmpty(recipTransfData.Email))
				{
					retVal = recipTransfData.Email;
				}
				else
				{
					retVal = recipTransfData.Name;
				}
			}
			return retVal;
		}

		#endregion
	}
}
