using System;
using System.Data;
using System.IO;
using System.Text;
using System.Collections;
using Mediachase.IBN.Database;
using Mediachase.IBN.Database.EMail;
using Mediachase.Net.Mail;
using System.Text.RegularExpressions;
using Mediachase.IBN.Business.ControlSystem;
using System.Collections.Specialized;
using System.Collections.Generic;
using Mediachase.IBN.Business.EMail;

namespace Mediachase.IBN.Business
{
    public static class EMailClient
    {
        public const string DefaultMode = "";
        public const string IssueMode = "issue";
		public const string SmtpTestMode = "smtptest";

		/// <summary>
		/// Determines whether [is alert sender email] [the specified email].
		/// </summary>
		/// <param name="email">The email.</param>
		/// <returns>
		/// 	<c>true</c> if [is alert sender email] [the specified email]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsAlertSenderEmail(string email)
		{
			if (string.IsNullOrEmpty(email))
				return false;

			return email == Alerts2.AlertSenderEmail;
		}

        /// <summary>
        /// Gets the default recipint list.
        /// </summary>
        /// <param name="Mode">The mode.</param>
        /// <param name="Params">The params.</param>
        /// <returns></returns>
        public static string[] GetDefaultRecipientList(string Mode, NameValueCollection Params)
        {
            List<string> retVal = new List<string>();

            Mode = Mode.ToLower();

            switch (Mode)
            {
                case EMailClient.IssueMode:
                    if (Params["IssueId"] == null)
                        throw new ArgumentNullException("Params[\"IssueId\"]");

                    int issueId = int.Parse(Params["IssueId"]);

                    int incidentBoxId = Incident.GetIncidentBox(issueId);

                    IncidentBoxDocument incBoxDoc = IncidentBoxDocument.Load(incidentBoxId);

                    // InternalEmailRecipients
                    if (incBoxDoc.EMailRouterBlock.AllowEMailRouting)
                    {
                        foreach (int userId in Incident.GetInternalEmailRecipients(issueId))
                        {
                            UserLight user = UserLight.Load(userId);
                            if (!retVal.Contains(user.Email))
                                retVal.Add(user.Email);

                        }
                    }

                    // External Emalil
                    foreach (EMailIssueExternalRecipient exRecipient in EMailIssueExternalRecipient.List(issueId))
                    {
                        if (!retVal.Contains(exRecipient.EMail))
                            retVal.Add(exRecipient.EMail);
                    }

                    break;
                default:
                    break;
            }

            return retVal.ToArray();
        }

        #region Send
        public static void SendMessage(string[] To, string Subject, string Body, Mediachase.IBN.Business.ControlSystem.DirectoryInfo Attachments)
        {
            SendMessage(To, Subject, Body, Attachments, string.Empty, new NameValueCollection());
        }

        public static void SendMessage(string[] To, string Subject, string Body, Mediachase.IBN.Business.ControlSystem.DirectoryInfo Attachments, string Mode, NameValueCollection Params)
        {
            // Cleanup Temporary files
            DbEMailTempFile.CleanUp();

            #region Validate Arguments
            if (To == null)
                throw new ArgumentNullException("To");

            if (Subject == null)
                throw new ArgumentNullException("Subject");

            if (Body == null)
                throw new ArgumentNullException("Body");

            //if (To.Length == 0)
            //    throw new ArgumentOutOfRangeException("To", "Email recipient list is empty.");

            if (Mode == null)
                Mode = string.Empty;

			if (Params == null)
				Params = new NameValueCollection();
            #endregion

			string FromEmail = string.Empty;

			switch (Mode)
			{
				case EMailClient.IssueMode:
				case EMailClient.SmtpTestMode:
					FromEmail = Alerts2.AlertSenderEmail;
					break;
				default:
					FromEmail = Security.CurrentUser.Email;
					break;
			}

			string FullFromEmail = string.Format("\"{0} {1}\" <{2}>",
				Security.CurrentUser.LastName,
				Security.CurrentUser.FirstName,
				FromEmail);

            using (DbTransaction tran = DbTransaction.Begin())
            {
                EMailMessageLogSetting EmailLogSettings = EMailMessageLogSetting.Current;
                if (EmailLogSettings.IsActive)
                    EMailMessageLog.CleanUp(EmailLogSettings.Period);
                else
                    EmailLogSettings = null;

                Mode = Mode.ToLower();

                #region Pre-format incoming arguments
                switch (Mode)
                {
                    case EMailClient.IssueMode:
                        if (Params["IssueId"] == null)
                            throw new ArgumentNullException("Params[\"IssueId\"]");

                        int IssueId = int.Parse(Params["IssueId"]);

                        // TODO: Validate Subject & Ticket
                        if (TicketUidUtil.LoadFromString(Subject) == string.Empty)
                        {
                            IncidentBox incidentBox = IncidentBox.Load(Incident.GetIncidentBox(IssueId));

                            string IncidentTicket = Incident.GetIdentifier(IssueId);

                            if (incidentBox.Document.GeneralBlock.AllowOutgoingEmailFormat)
                            {
                                StringBuilder sb = new StringBuilder(incidentBox.Document.GeneralBlock.OutgoingEmailFormatSubject, 4096);

                                sb.Replace("[=Title=]", Subject);
                                sb.Replace("[=Ticket=]", IncidentTicket);
                                //sb.Replace("[=Text=]", Body);
                                sb.Replace("[=FirstName=]", Security.CurrentUser.FirstName);
                                sb.Replace("[=LastName=]", Security.CurrentUser.LastName);

                                Subject = sb.ToString();
                            }
                            else
                            {
                                Subject = string.Format("RE: [{0}] {1}",
                                    IncidentTicket,
                                    Subject);
                            }

                        }
                        break;
                    default:
                        break;
                }
                
                #endregion

                Pop3Message msg = Create(FullFromEmail, To, Subject, Body, Attachments);

                switch (Mode)
                {
                    case EMailClient.IssueMode:
                        #region Issue
                        int IssueId = int.Parse(Params["IssueId"]);

                        IncidentBox incidentBox = IncidentBox.Load(Incident.GetIncidentBox(IssueId));

                        bool AllowEMailRouting = true;

                        EMailRouterIncidentBoxBlock settings = IncidentBoxDocument.Load(incidentBox.IncidentBoxId).EMailRouterBlock;
                        if (!settings.AllowEMailRouting)
                            AllowEMailRouting = false;

                        EMailRouterPop3Box internalPop3Box = EMailRouterPop3Box.ListInternal();
                        if (internalPop3Box == null)
                            AllowEMailRouting = false;

                        // Register Email Message
                        // OZ: [2007--05-25] Fix Problem Object reference not set to an instance of an object If (internalPop3Box == NULL)
                        int EMailMessageId = EMailMessage.Create(internalPop3Box!=null?
                            internalPop3Box.EMailRouterPop3BoxId : EMailRouterOutputMessage.FindEMailRouterPublicId(IssueId), 
                            msg);

                        // Register Forume Node
                        int ThreadNodeId = EMailMessage.AddToIncidentMessage(true, IssueId, EMailMessageId, msg);

                        // Send Message

                        if (AllowEMailRouting)
                        {
                            ArrayList excludedUsers = EMailRouterOutputMessage.Send(IssueId, internalPop3Box, msg, To);
                            SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Forum_MessageAdded, IssueId, -1, excludedUsers);
                        }
                        else
                        {
                            FromEmail = EMailRouterOutputMessage.FindEMailRouterPublicEmail(IssueId);
                            FullFromEmail = string.Format("\"{0} {1}\" <{2}>",
								Security.CurrentUser.LastName,
                                Security.CurrentUser.FirstName,
                                FromEmail);

                            // Create OutputMessageCreator
                            OutputMessageCreator issueOutput = new OutputMessageCreator(msg,
                                -1,
                                FromEmail,
                                FullFromEmail);

                            // Fill Recipent
                            foreach (string ToItem in To)
                            {
                                issueOutput.AddRecipient(ToItem);
                            }

                            foreach (EMailIssueExternalRecipient exRecipient in EMailIssueExternalRecipient.List(IssueId))
                            {
                                issueOutput.AddRecipient(exRecipient.EMail);
                            }

							int emailBoxId = EMail.EMailRouterOutputMessage.FindEMailRouterPublicId(IssueId);

                            //Send Smtp Message
                            foreach (OutputMessage outputMsg in issueOutput.Create())
                            {
								SmtpClientUtility.SendMessage(OutgoingEmailServiceType.HelpDeskEmailBox, emailBoxId, outputMsg.MailFrom, outputMsg.RcptTo, outputMsg.Subject, outputMsg.Data);
                            }

                            ArrayList excludedUsers = new ArrayList();

                            foreach (string ToItem in To)
                            {
                                int emailUserId = DBUser.GetUserByEmail(ToItem, false);
                                if (emailUserId > 0)
                                    excludedUsers.Add(emailUserId);
                            }

                            SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Forum_MessageAdded, IssueId, -1, excludedUsers);
                        }
                        #endregion                        
                        break;
					case EMailClient.SmtpTestMode:
						throw new NotImplementedException();
						//OutputMessageCreator smtpTestOutput = new OutputMessageCreator(msg,
						//    -1,
						//    FromEmail,
						//    FullFromEmail);

						//// Fill Recipent
						//foreach (string ToItem in To)
						//{
						//    smtpTestOutput.AddRecipient(ToItem);
						//}

						////Send Smtp Message
						//foreach (OutputMessage outputMsg in smtpTestOutput.Create())
						//{
						//    //SmtpClientUtility.DirectSendMessage(outputMsg.MailFrom, outputMsg.RcptTo, outputMsg.Subject, outputMsg.Data);
						//    //SmtpBox.SendTestEmail(
						//}
						//break;
                    default:
                        #region Default
                        // Create OutputMessageCreator
                        OutputMessageCreator defaultOutput = new OutputMessageCreator(msg,
                            -1,
                            FromEmail,
                            FullFromEmail);

                        // Fill Recipent
                        foreach (string ToItem in To)
                        {
                            defaultOutput.AddRecipient(ToItem);
                        }

                        //Send Smtp Message
                        foreach (OutputMessage outputMsg in defaultOutput.Create())
                        {
                            SmtpClientUtility.SendMessage(OutgoingEmailServiceType.SendFile, null, outputMsg.MailFrom, outputMsg.RcptTo, outputMsg.Subject, outputMsg.Data);
                        }
                    
                        #endregion                        
                        break;
                }

                if(Attachments!=null)
                    FileStorage.InnerDeleteFolder(Attachments.Id);

                tran.Commit();
            }
        }


        #endregion

        #region Create Pop3Message
        internal static Pop3Message Create(string From, string[] To, string Subject, string Body, Mediachase.IBN.Business.ControlSystem.DirectoryInfo Attachments)
        {
            MemoryStream emlMessage = new MemoryStream();
            byte[] tmpBuffer = null;

            string ToEmail = string.Join(", ", To);

            #region Fill Pop3 Message Stream
            // Create Pop3 Message Headers
            StringBuilder sbHeaders = new StringBuilder();

            sbHeaders.AppendFormat("Date: {0}", DateTime.UtcNow.ToString("r")).Append("\r\n");

            sbHeaders.AppendFormat("From: {0}", Rfc822HeaderCollection.Encode2AsciiString(From)).Append("\r\n");
            sbHeaders.AppendFormat("To: {0}", Rfc822HeaderCollection.Encode2AsciiString(ToEmail)).Append("\r\n");

            sbHeaders.AppendFormat("Subject: {0}", Rfc822HeaderCollection.Encode2AsciiString(Subject)).Append("\r\n");
            sbHeaders.Append("MIME-Version: 1.0").Append("\r\n");
            sbHeaders.Append("Content-Type: multipart/mixed; boundary=\"----------7E143249668A83E\"").Append("\r\n");
            sbHeaders.Append("\r\n");

            tmpBuffer = Encoding.ASCII.GetBytes(sbHeaders.ToString());
            emlMessage.Write(tmpBuffer, 0, tmpBuffer.Length);

            // Create Pop3 Message Entry
            StringBuilder sbMessage = new StringBuilder();

            sbMessage.Append("------------7E143249668A83E").Append("\r\n");

            // IF MESSAGE IS PLAIN TEXT
            //sbMessage.Append("Content-Type: text/plain; charset=utf-8").Append("\r\n");

            // IF MESSAGE IS HTML TEXT
            sbMessage.Append("Content-Type: text/html; charset=utf-8").Append("\r\n");

            sbMessage.Append("Content-Transfer-Encoding: base64").Append("\r\n");
            sbMessage.Append("\r\n");

            string FullMessage = Body;

            sbMessage.Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(FullMessage),Base64FormattingOptions.InsertLineBreaks)).Append("\r\n");

            tmpBuffer = Encoding.ASCII.GetBytes(sbMessage.ToString());
            emlMessage.Write(tmpBuffer, 0, tmpBuffer.Length);

            if (Attachments != null)
            {
                Hashtable contentTypeHash = new Hashtable();

                using (IDataReader reader = ContentType.GetListContentTypes())
                {
                    while (reader.Read())
                    {
                        contentTypeHash.Add(((string)reader["Extension"]).ToLower(), (string)reader["ContentTypeString"]);
                    }
                }

                // Add Pop3 Message Attachements
                foreach (Mediachase.IBN.Business.ControlSystem.FileInfo fileInfo in Attachments.GetFiles())
                {
                    StringBuilder sbFile = new StringBuilder();

                    sbFile.Append("------------7E143249668A83E").Append("\r\n");
                    sbFile.AppendFormat("Content-Type: {0}; name=\"{1}\"", fileInfo.FileBinaryContentType, Rfc822HeaderCollection.Encode2AsciiString(fileInfo.Name)).Append("\r\n");
                    sbFile.Append("Content-Transfer-Encoding: base64").Append("\r\n");
                    sbFile.AppendFormat("Content-Disposition: attachment; filename=\"{0}\"", Rfc822HeaderCollection.Encode2AsciiString(fileInfo.Name)).Append("\r\n");
                    sbFile.Append("\r\n");

                    using (MemoryStream fs = new MemoryStream())
                    {
                        FileStorage.LightLoadFile(fileInfo, fs);
                        fs.Capacity = (int)fs.Length;

                        sbFile.Append(Convert.ToBase64String(fs.GetBuffer(), Base64FormattingOptions.InsertLineBreaks));
                    }

                    sbFile.Append("\r\n");

                    tmpBuffer = Encoding.ASCII.GetBytes(sbFile.ToString());
                    emlMessage.Write(tmpBuffer, 0, tmpBuffer.Length);
                }
            }

            // Add Final Line
            tmpBuffer = Encoding.ASCII.GetBytes("------------7E143249668A83E--\r\n\r\n");
            emlMessage.Write(tmpBuffer, 0, tmpBuffer.Length);

            #endregion

            return new Pop3Message(emlMessage);
        }
        #endregion
    }
}
