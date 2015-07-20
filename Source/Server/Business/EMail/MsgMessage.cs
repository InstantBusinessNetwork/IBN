using System;
using System.IO;
using System.Text;
using Mediachase.Net.Mail;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for MsgMessage.
	/// </summary>
	public class MsgMessage
	{
		public static void OpenEml(int IncidentId, int EMailMessageId, Stream outputStream)
		{
			Pop3Message message = EMailMessage.GetPop3Message(EMailMessageId);
			IncidentBox incidentBox = IncidentBox.Load(Incident.GetIncidentBox(IncidentId)); 

			EMailRouterPop3Box emailBox = EMailRouterPop3Box.ListInternal();

			string SenderName = EMailMessage.GetSenderName(message);

			string FromEmail = SenderName==string.Empty?emailBox.EMailAddress:
				string.Format("\"{0}\" <{1}>",
				SenderName,
				emailBox.EMailAddress);

			OutputMessageCreator output = new OutputMessageCreator(message, IncidentId, emailBox.EMailAddress, FromEmail);
			output.AddRecipient(Security.CurrentUser.UserID);

			string Subject = (message.Subject==null?string.Empty:message.Subject);

			if(Subject==string.Empty)
			{
				// OZ: Maybe:  Set Default Inicdent Title if subject is empty
				//Subject = Incident.GetIncidentTitle(IncidentId);
			}

			if(TicketUidUtil.LoadFromString(Subject)==string.Empty)
				output.Subject = string.Format("[{0}] {1}",TicketUidUtil.Create(incidentBox.IdentifierMask,IncidentId), Subject);


			OutputMessage outputMsg = (OutputMessage)output.Create()[0];
			outputStream.Write(outputMsg.Data, 0, outputMsg.Data.Length);

			outputStream.Flush();
		}

		public static void Open(int IncidentId, int EMailMessageId, Stream outputStream)
		{
			System.Reflection.Assembly asm = typeof(MsgMessage).Assembly;

			Pop3Message message = EMailMessage.GetPop3Message(EMailMessageId);
			IncidentBox incidentBox = IncidentBox.Load(Incident.GetIncidentBox(IncidentId)); 

			EMailRouterPop3Box box = EMailRouterPop3Box.ListInternal();

			string receiver = Security.CurrentUser.Email;//EMailMessage.GetSenderEmail(message);
			string sender = box.EMailAddress;

			using (Stream stream = asm.GetManifestResourceStream("Mediachase.IBN.Business.Resources.template.msg"))
			{
				using (MemoryStream memStream = new MemoryStream())
				{
					using (MsgHelper helper = new MsgHelper(stream))
					{
						helper.SetSenderEmail(sender);

						//string SenderName = EMailMessage.GetSenderName(message);
						//helper.SetSenderName(SenderName==string.Empty?sender:SenderName);
						helper.SetSenderName(sender);

						string IncidentTitle = Incident.GetIncidentTitle(IncidentId);
						string IncidentTicket = TicketUidUtil.Create(incidentBox.IdentifierMask,IncidentId);

						string Subject = (message.Subject==null?string.Empty:message.Subject);

						if(Subject==string.Empty)
						{
							// OZ: Maybe:  Set Default Inicdent Title if subject is empty
							//Subject = Incident.GetIncidentTitle(IncidentId);
						}

						if(TicketUidUtil.LoadFromString(Subject)==string.Empty)
							Subject = string.Format("[{0}] {1}",TicketUidUtil.Create(incidentBox.IdentifierMask,IncidentId), Subject);

						helper.SetReceiverName(Security.CurrentUser.DisplayName);
						helper.SetDisplayTo(receiver);
						helper.SetReceiverEmail(receiver);
						helper.SetCreationTimes(DateTime.UtcNow);

                        helper.SetSubject(Subject);
                        helper.SetBody(message.BodyText);
                        if (message.BodyHtml == null || message.BodyHtml.Trim() == String.Empty)
                        {
                            helper.SetHtmlBody(message.BodyText);
                        }
                        else helper.SetHtmlBody(message.BodyHtml);

						helper.Commit();

						helper.createMSG(memStream);
						memStream.Flush();
						memStream.Seek(0,SeekOrigin.Begin);

						const int bufLen = 1024; 
						byte[] buffer = new byte[bufLen]; 
						int bytesRead; 
						while ((bytesRead = memStream.Read(buffer, 0, bufLen)) > 0) 
						{ 
							outputStream.Write(buffer, 0, bytesRead);
						} 
						outputStream.Flush();
					}
				}
			}
		}
	}
}
