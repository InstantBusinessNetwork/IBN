using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Net.Mail;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data.Sql;
using Mediachase.IBN.Business.EMail;

namespace Mediachase.Ibn.Business.Messages
{
	/// <summary>
	/// Represents email delivery provider.
	/// </summary>
	public class EmailDeliveryProvider : MessageDeliveryProvider
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="EmailDeliveryProvider"/> class.
		/// </summary>
		public EmailDeliveryProvider()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		public override void Invoke()
		{
			// TODO: Remove Expired
			RemoveExpiredMessages();

			//int maxDeliveryAttempts = 100; // TODO: Read From Config
			int maxMessagesPerConnection = 10;

			// Prepare temporary collection
			List<MailMessage> outputMessages = new List<MailMessage>();
			List<OutgoingMessageQueueEntity> queueEntity = new List<OutgoingMessageQueueEntity>();

			// Load Outgoing Message Queue Entity
			EntityObject[] elements = BusinessManager.List(OutgoingMessageQueueEntity.ClassName,
				OutgoingMessageUtil.CreateEmailDeliveryProviderFilters(),
				new SortingElement[]
					{
						SortingElement.Ascending(OutgoingMessageQueueEntity.FieldSource),
						SortingElement.Ascending(OutgoingMessageQueueEntity.FieldCreated)
					});


			// Run Message Delivery Process
			for (int elementIndex = 0; elementIndex < elements.Length; elementIndex++)
			{
				// Read Element From Outgoing Message Queue 
				OutgoingMessageQueueEntity element = (OutgoingMessageQueueEntity)elements[elementIndex];

				// Load Email Message
				EmailEntity emailEntity = (EmailEntity)BusinessManager.Load(EmailEntity.ClassName, element.EmailId.Value);

				// Create Output Mail Message
				MailMessage outputMessage = CopyEmailEntityToMailMessage(emailEntity);

				// Add Output message to Output queue
				outputMessages.Add(outputMessage);
				queueEntity.Add(element);

				string currentSource = element.Source;
				string nextElementSource = (elementIndex < (elements.Length - 1)) ?
					((OutgoingMessageQueueEntity)elements[elementIndex+1]).Source :
					string.Empty;

				if (outputMessages.Count < maxMessagesPerConnection && 
					currentSource==nextElementSource)
					continue;

				// Send Output queue
				try
				{
					// OZ [2010-03-03]: Check that SmtpServer is configured. Write To Outgoing log
					if (SmtpBox.TotalCount() == 0)
						throw new SmtpNotConfiguredException();

					// Initialize Smtp Client
					SmtpClient smtpClient = Mediachase.IBN.Business.EMail.SmtpClientUtility.CreateSmtpClient(element.Source);

					// Send
					smtpClient.Send(outputMessages.ToArray());

					// Process result
					ProcessSendResult(outputMessages, queueEntity);
				}
				catch (Exception ex)
				{
					ProcessException(outputMessages, queueEntity, ex);
				}

				// Clear Output queue
				outputMessages.Clear();
				queueEntity.Clear();
			}

			//
		}

		private void RemoveExpiredMessages()
		{
			int expiration = PortalConfig.MdsDeleteOlderMoreThan;

			if (expiration > 0)
			{
				SqlScript sqlScript = new SqlScript();

				sqlScript.AppendLine("DELETE FROM [dbo].[cls_Email] WHERE Created < @ExpirationDate");

				sqlScript.AddParameter("@ExpirationDate", DateTime.UtcNow.AddMinutes(-expiration));

				sqlScript.Execute();
			}
		}

		/// <summary>
		/// Processes the exception.
		/// </summary>
		/// <param name="queueEntity">The queue entity.</param>
		/// <param name="ex">The ex.</param>
		private void ProcessException(List<MailMessage> outputMessages, List<OutgoingMessageQueueEntity> queueEntity, Exception ex)
		{
			for (int index = 0; index < outputMessages.Count; index++)
			{
				MailMessage message = outputMessages[index];
				OutgoingMessageQueueEntity entity = queueEntity[index];

				if (message.SentDate != DateTime.MinValue)
				{
					entity.Error = string.Empty;
					entity.DeliveryAttempts++;
					entity.IsDelivered = true;
				}
				else
				{
					entity.Error = ex.Message;
					entity.DeliveryAttempts++;
				}

				BusinessManager.Update(entity);
			}

			//foreach (OutgoingMessageQueueEntity entity in queueEntity)
			//{
			//    entity.Error = ex.Message;
			//    entity.DeliveryAttempts++;

			//    // TODO: Save Complete Error Stack || Complete Delivery Log

			//    BusinessManager.Update(entity);
			//}
		}

		/// <summary>
		/// Processes the send result.
		/// </summary>
		/// <param name="outputMessages">The output messages.</param>
		/// <param name="queueEntity">The queue entity.</param>
		private void ProcessSendResult(List<MailMessage> outputMessages, List<OutgoingMessageQueueEntity> queueEntity)
		{
			for (int index = 0; index < outputMessages.Count; index++)
			{
				MailMessage message = outputMessages[index];
				OutgoingMessageQueueEntity entity = queueEntity[index];

				if (message.SentDate!=DateTime.MinValue)
				{
					// Mark Message As Delivered
					entity.Error = string.Empty;
					entity.DeliveryAttempts++;
					entity.IsDelivered = true;
				}
				else
				{
					entity.Error = message.ErrorMessage;
					entity.DeliveryAttempts++;
				}

				BusinessManager.Update(entity);
			}
		}

		/// <summary>
		/// Copies the email entity to mail message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		private MailMessage CopyEmailEntityToMailMessage(EmailEntity message)
		{
			MailMessage outputMessage = new MailMessage();

			outputMessage.Subject = message.Subject;
			outputMessage.From = EmailUtil.CreateMailAddress(message.From);
			outputMessage.To.Add(EmailUtil.CreateMailAddress(message.To));

			if (!string.IsNullOrEmpty(message.MessageContext))
			{
				outputMessage.MessageContent = message.MessageContext;
			}
			else
			{
				outputMessage.IsBodyHtml = true;
				outputMessage.Body = message.HtmlBody;
			}
			return outputMessage;
		}

		#endregion
	}
}
