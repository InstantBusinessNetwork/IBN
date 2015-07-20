using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Database;
using Mediachase.Ibn.Data.Sql;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Business.Messages
{
	/// <summary>
	/// Represents ibn client message delivery provider.
	/// </summary>
	public class IbnClientMessageDeliveryProvider: MessageDeliveryProvider
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="IbnClientMessageDeliveryProvider"/> class.
		/// </summary>
		public IbnClientMessageDeliveryProvider()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		/// <summary>
		/// Invokes this instance.
		/// </summary>
		public override void Invoke()
		{
			// Remove Expired
			RemoveExpiredMessages();

			//int maxDeliveryAttempts = 100; // TODO: Read From Config

			EntityObject[] elements = BusinessManager.List(OutgoingMessageQueueEntity.ClassName,
				OutgoingMessageUtil.CreateIbnClientMessageDeliveryProviderFilters(),
				new SortingElement[]
					{
						SortingElement.Ascending(OutgoingMessageQueueEntity.FieldCreated)
					});

			foreach (OutgoingMessageQueueEntity element in elements)
			{
				try
				{
					// Load Ibn Message
					IbnClientMessageEntity message = (IbnClientMessageEntity)BusinessManager.Load(IbnClientMessageEntity.ClassName, element.IbnClientMessageId.Value);

					// Send
					int toOriginalId = DBUser.GetOriginalId(message.ToId);
					int fromOriginalId = DBUser.GetOriginalId(message.FromId);

					IMHelper.SendMessage(toOriginalId, fromOriginalId, message.HtmlBody);

					element.Error = string.Empty;
					element.DeliveryAttempts++;
					element.IsDelivered = true;
				}
				catch (Exception ex)
				{
					element.Error = ex.Message;
					element.DeliveryAttempts++;

					// TODO: Save Complete Error Stack || Complete Delivery Log
				}

				BusinessManager.Update(element);
			}

			//
		}

		private void RemoveExpiredMessages()
		{
			int expiration = PortalConfig.MdsDeleteOlderMoreThan;

			if (expiration > 0)
			{
				SqlScript sqlScript = new SqlScript();

				sqlScript.AppendLine("DELETE FROM [dbo].[cls_IbnClientMessage] WHERE Created < @ExpirationDate");

				sqlScript.AddParameter("@ExpirationDate", DateTime.UtcNow.AddMinutes(-expiration));

				sqlScript.Execute(); 
			}
		}
		#endregion

	}
}
