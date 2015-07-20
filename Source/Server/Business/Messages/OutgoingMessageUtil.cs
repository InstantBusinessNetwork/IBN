using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Business.Messages;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Business.Messages
{
	/// <summary>
	/// Represents outgoing message utility.
	/// </summary>
	internal static class OutgoingMessageUtil
	{
		/// <summary>
		/// Creates the email delivery provider filters.
		/// </summary>
		/// <returns></returns>
		internal static FilterElement[] CreateEmailDeliveryProviderFilters()
		{
			List<FilterElement> retVal = new List<FilterElement>();

			retVal.Add(FilterElement.IsNotNullElement(OutgoingMessageQueueEntity.FieldEmailId));
			retVal.Add(FilterElement.EqualElement(OutgoingMessageQueueEntity.FieldIsDelivered, false));

			retVal.Add(new FilterElement("", FilterElementType.Custom,
				"[t01].[Modified] < (DATEADD(mi,-(CASE WHEN [t01].[DeliveryAttempts]>5 THEN 30 ELSE [t01].[DeliveryAttempts]*5 END),getutcdate()))"));

			// Check Delivery Constrains Max Delivery Attempts
			if (PortalConfig.MdsMaxDeliveryAttempts > 0)
			{
				retVal.Add(new FilterElement(OutgoingMessageQueueEntity.FieldDeliveryAttempts,
					FilterElementType.LessOrEqual,
					PortalConfig.MdsMaxDeliveryAttempts));
			}

			// Check Delivery Constrains Delivery Timeout
			if (PortalConfig.MdsDeliveryTimeout > 0)
			{
				retVal.Add(new FilterElement(OutgoingMessageQueueEntity.FieldCreated,
					FilterElementType.GreaterOrEqual,
					DateTime.UtcNow.AddMinutes(-1 * PortalConfig.MdsDeliveryTimeout)));
			}

			return retVal.ToArray();
		}

		/// <summary>
		/// Creates the email delivery provider filters.
		/// </summary>
		/// <returns></returns>
		internal static FilterElement[] CreateIbnClientMessageDeliveryProviderFilters()
		{
			List<FilterElement> retVal = new List<FilterElement>();

			retVal.Add(FilterElement.IsNotNullElement(OutgoingMessageQueueEntity.FieldIbnClientMessageId));
			retVal.Add(FilterElement.EqualElement(OutgoingMessageQueueEntity.FieldIsDelivered, false));

			retVal.Add(new FilterElement("", FilterElementType.Custom,
				"[t01].[Modified] < (DATEADD(mi,-(CASE WHEN [t01].[DeliveryAttempts]>5 THEN 30 ELSE [t01].[DeliveryAttempts]*5 END),getutcdate()))"));

			// Check Delivery Constrains Max Delivery Attempts
			if (PortalConfig.MdsMaxDeliveryAttempts > 0)
			{
				retVal.Add(new FilterElement(OutgoingMessageQueueEntity.FieldDeliveryAttempts,
					FilterElementType.LessOrEqual,
					PortalConfig.MdsMaxDeliveryAttempts));
			}

			// Check Delivery Constrains Delivery Timeout
			if (PortalConfig.MdsDeliveryTimeout > 0)
			{
				retVal.Add(new FilterElement(OutgoingMessageQueueEntity.FieldCreated,
					FilterElementType.GreaterOrEqual,
					DateTime.UtcNow.AddMinutes(-1 * PortalConfig.MdsDeliveryTimeout)));
			}

			return retVal.ToArray();
		}
	}
}
