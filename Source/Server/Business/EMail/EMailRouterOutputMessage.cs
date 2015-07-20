using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

using Mediachase.IBN;
using Mediachase.Ibn;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Database;
using Mediachase.IBN.Database.EMail;
using Mediachase.Schedule;
using Mediachase.Net.Mail;


namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for EMailRouterOutputMessage.
	/// </summary>
	public static class EMailRouterOutputMessage
	{
		/// <summary>
		/// Gets the E mail creation date.
		/// </summary>
		/// <returns></returns>
		private static string GetEMailCreationDate()
		{
			DateTime userTime = DateTime.UtcNow;

			//Fri, 13 Oct 2006 13:16:31 +0800 
			//Wed, 21 Feb 2007 00:04:40 -0600
			return userTime.ToString("r");
		}

		/// <summary>
		/// Sends the auto reply.
		/// </summary>
		/// <param name="IncidentId">The incident id.</param>
		/// <param name="From">From.</param>
		/// <param name="To">To.</param>
		internal static void SendAutoReply(int IncidentId, string From, string To)
		{
			IncidentBox incidentBox = IncidentBox.Load( Incident.GetIncidentBox(IncidentId));

			if(!incidentBox.Document.EMailRouterBlock.SendAutoReply)
				return;

			Alerts2.Message msg = Alerts2.GetMessage(incidentBox.Document.GeneralBlock.AutoReplyEMailSubjectTemplate,
				incidentBox.Document.GeneralBlock.AutoReplyEMailBodyTemplate,
				SystemEventTypes.Issue_Created,
				IncidentId, null,
				-1,
				Security.CurrentUser.UserID);

			string subject = msg.Subject, body = msg.Body;

			MemoryStream emlMessage = new MemoryStream();
			byte[] tmpBuffer = null;
			
			#region Fill Pop3 Message Stream
			// Create Pop3 Message Headers
			StringBuilder sbHeaders = new StringBuilder();

			sbHeaders.AppendFormat("Date: {0}", GetEMailCreationDate()).Append("\r\n");
			sbHeaders.Append("From: empty@empty.com").Append("\r\n");
			sbHeaders.Append("To: empty@empty.com").Append("\r\n");
			sbHeaders.AppendFormat("Subject: {0}", Rfc822HeaderCollection.Encode2AsciiString(subject)).Append("\r\n");
			sbHeaders.Append("MIME-Version: 1.0").Append("\r\n");
			sbHeaders.Append("Content-Type: multipart/mixed; boundary=\"----------7E143249668A83E\"").Append("\r\n");
			sbHeaders.Append("\r\n");

			tmpBuffer = Encoding.ASCII.GetBytes(sbHeaders.ToString());
			emlMessage.Write(tmpBuffer,0, tmpBuffer.Length);

			// Create Pop3 Message Entry
			StringBuilder sbMessage = new StringBuilder();

			sbMessage.Append("------------7E143249668A83E").Append("\r\n");

			// IF MESSAGE IS PLAIN TEXT
			//sbMessage.Append("Content-Type: text/plain; charset=utf-8").Append("\r\n");

			// IF MESSAGE IS HTML TEXT
			sbMessage.Append("Content-Type: text/html; charset=utf-8").Append("\r\n");

			sbMessage.Append("Content-Transfer-Encoding: base64").Append("\r\n");
			sbMessage.Append("\r\n");

			// OZ Fixed 500 5.3.3 Line too long (in reply to end of DATA command)
			sbMessage.Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(body), Base64FormattingOptions.InsertLineBreaks)).Append("\r\n");

			tmpBuffer = Encoding.ASCII.GetBytes(sbMessage.ToString());
			emlMessage.Write(tmpBuffer,0, tmpBuffer.Length);

			// Add Final Line
			tmpBuffer = Encoding.ASCII.GetBytes("------------7E143249668A83E--\r\n\r\n");
			emlMessage.Write(tmpBuffer,0, tmpBuffer.Length);

			#endregion

			Pop3Message InMsg = new Pop3Message(emlMessage);

			OutputMessageCreator outputAutoReply = new OutputMessageCreator(InMsg, IncidentId, From, From);
			outputAutoReply.AddRecipient(To);

			int emailBoxId = EMail.EMailRouterOutputMessage.FindEMailRouterPublicId(IncidentId);

			foreach(OutputMessage outputMsg in outputAutoReply.Create())
			{
				try
				{
					SmtpClientUtility.SendMessage(OutgoingEmailServiceType.HelpDeskEmailBox, emailBoxId, outputMsg.MailFrom, outputMsg.RcptTo, outputMsg.Subject, outputMsg.Data);
				}
				catch(Exception ex)
				{
					System.Diagnostics.Trace.WriteLine(ex);
					Log.WriteError(ex.ToString());
				}
			}

		}

		/// <summary>
		/// Sends the on close auto reply.
		/// </summary>
		/// <param name="IncidentId">The incident id.</param>
		/// <param name="To">To.</param>
		/// <param name="From">From.</param>
		public static void SendAutoIncidentClosed(int IncidentId, string From, params string[] ToList)
		{
			IncidentBox incidentBox = IncidentBox.Load(Incident.GetIncidentBox(IncidentId));

			if (!incidentBox.Document.EMailRouterBlock.SendAutoIncidentClosed)
				return;

			Alerts2.Message msg = Alerts2.GetMessage(incidentBox.Document.GeneralBlock.OnCloseAutoReplyEMailSubjectTemplate,
				incidentBox.Document.GeneralBlock.OnCloseAutoReplyEMailBodyTemplate,
				SystemEventTypes.Issue_Created,
				IncidentId, null,
				-1,
				Security.CurrentUser.UserID);

			string subject = msg.Subject;
			string body = msg.Body;

			MemoryStream emlMessage = new MemoryStream();
			byte[] tmpBuffer = null;

			#region Fill Pop3 Message Stream
			// Create Pop3 Message Headers
			StringBuilder sbHeaders = new StringBuilder();

			sbHeaders.AppendFormat("Date: {0}", GetEMailCreationDate()).Append("\r\n");
			sbHeaders.Append("From: empty@empty.com").Append("\r\n");
			sbHeaders.Append("To: empty@empty.com").Append("\r\n");
			sbHeaders.AppendFormat("Subject: {0}", Rfc822HeaderCollection.Encode2AsciiString(subject)).Append("\r\n");
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

			// OZ Fixed 500 5.3.3 Line too long (in reply to end of DATA command)
			sbMessage.Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(body), Base64FormattingOptions.InsertLineBreaks)).Append("\r\n");

			tmpBuffer = Encoding.ASCII.GetBytes(sbMessage.ToString());
			emlMessage.Write(tmpBuffer, 0, tmpBuffer.Length);

			// Add Final Line
			tmpBuffer = Encoding.ASCII.GetBytes("------------7E143249668A83E--\r\n\r\n");
			emlMessage.Write(tmpBuffer, 0, tmpBuffer.Length);

			#endregion

			Pop3Message InMsg = new Pop3Message(emlMessage);

			OutputMessageCreator outputAutoReply = new OutputMessageCreator(InMsg, IncidentId, From, From);

			foreach (string to in ToList)
			{
				outputAutoReply.AddRecipient(to);
			}

			int emailBoxId = EMail.EMailRouterOutputMessage.FindEMailRouterPublicId(IncidentId);

			foreach (OutputMessage outputMsg in outputAutoReply.Create())
			{
				try
				{
					SmtpClientUtility.SendMessage(OutgoingEmailServiceType.HelpDeskEmailBox, emailBoxId, outputMsg.MailFrom, outputMsg.RcptTo, outputMsg.Subject, outputMsg.Data);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.WriteLine(ex);
					Log.WriteError(ex.ToString());
				}
			}

		}


        /// <summary>
        /// Sends the outgoing.
        /// </summary>
        /// <param name="IncidentId">The incident id.</param>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        //public static ArrayList SendOutgoing(int IncidentId, EMailOutgoingMessage email)
        //{
        //    ArrayList retVal = new ArrayList();

        //    // TODO: Not implemented yet

        //    return retVal;
        //}


        /// <summary>
        /// Sends the outgoing.
        /// </summary>
        /// <param name="IncidentId">The incident id.</param>
        /// <returns></returns>
//        public static ArrayList SendOutgoing(int IncidentId, int ThreadNodeId)
//        {
//            // 2007-02-15: OZ: New Messagew Addon
//            Issue2.SetNewMessage(IncidentId, false);
//            //

//            ArrayList retVal = new ArrayList();

//            IncidentBox incidentBox = IncidentBox.Load( Incident.GetIncidentBox(IncidentId));

//            bool AllowEMailRouting = true;

//            EMailRouterIncidentBoxBlock settings = IncidentBoxDocument.Load(incidentBox.IncidentBoxId).EMailRouterBlock;
//            if(!settings.AllowEMailRouting)
//                AllowEMailRouting = false;

//            EMailRouterPop3Box internalPop3Box = EMailRouterPop3Box.ListInternal();
//            if(internalPop3Box==null)
//                AllowEMailRouting = false;

//            EMailRouterPop3Box pop3Box = internalPop3Box;

//            BaseIbnContainer foContainer = BaseIbnContainer.Create("FileLibrary",string.Format("IncidentId_{0}",IncidentId));
//            ForumStorage forumStorage = (ForumStorage)foContainer.LoadControl("ForumStorage");

//            ForumThreadNodeInfo node = forumStorage.GetForumThreadNode(ThreadNodeId);

//            BaseIbnContainer fsContainer = BaseIbnContainer.Create("FileLibrary",string.Format("ForumNodeId_{0}",node.Id));
//            FileStorage fileStorage = (FileStorage)fsContainer.LoadControl("FileStorage");

//            string IncidentTitle = Incident.GetIncidentTitle(IncidentId);
//            string IncidentTicket = TicketUidUtil.Create(incidentBox.IdentifierMask,IncidentId);
			
//            string subject;

//            if(incidentBox.Document.GeneralBlock.AllowOutgoingEmailFormat)
//            {
//                StringBuilder sb = new StringBuilder(incidentBox.Document.GeneralBlock.OutgoingEmailFormatSubject,4096);

//                sb.Replace("[=Title=]", IncidentTitle);
//                sb.Replace("[=Ticket=]", IncidentTicket);
//                sb.Replace("[=Text=]", node.Text);
//                sb.Replace("[=FirstName=]", Security.CurrentUser.FirstName);
//                sb.Replace("[=LastName=]", Security.CurrentUser.LastName);

//                subject = sb.ToString();
//            }
//            else
//            {
//                subject = string.Format("RE: [{0}] {1}", 
//                    IncidentTicket,
//                    IncidentTitle);
//            }

//            // Create Pop3 Message Stream
//            MemoryStream emlMessage = new MemoryStream();
//            byte[] tmpBuffer = null;
			
//            #region Fill Pop3 Message Stream
//            // Create Pop3 Message Headers
//            StringBuilder sbHeaders = new StringBuilder();

//            sbHeaders.AppendFormat("Date: {0}", GetEMailCreationDate()).Append("\r\n");
//            sbHeaders.Append("From: empty@empty.com").Append("\r\n");
//            sbHeaders.Append("To: empty@empty.com").Append("\r\n");
//            sbHeaders.AppendFormat("Subject: {0}", Rfc822HeaderCollection.Encode2AsciiString(subject)).Append("\r\n");
//            sbHeaders.Append("MIME-Version: 1.0").Append("\r\n");
//            sbHeaders.Append("Content-Type: multipart/mixed; boundary=\"----------7E143249668A83E\"").Append("\r\n");
//            sbHeaders.Append("\r\n");

//            tmpBuffer = Encoding.ASCII.GetBytes(sbHeaders.ToString());
//            emlMessage.Write(tmpBuffer,0, tmpBuffer.Length);

//            // Create Pop3 Message Entry
//            StringBuilder sbMessage = new StringBuilder();

//            sbMessage.Append("------------7E143249668A83E").Append("\r\n");

//            // IF MESSAGE IS PLAIN TEXT
//            //sbMessage.Append("Content-Type: text/plain; charset=utf-8").Append("\r\n");

//            // IF MESSAGE IS HTML TEXT
//            sbMessage.Append("Content-Type: text/html; charset=utf-8").Append("\r\n");

//            sbMessage.Append("Content-Transfer-Encoding: base64").Append("\r\n");
//            sbMessage.Append("\r\n");

//            string FullMessage;

////			if(incidentBox.Document.GeneralBlock.AllowOutgoingEmailFormat)
////			{
////				StringBuilder sb = new StringBuilder(incidentBox.Document.GeneralBlock.OutgoingEmailFormatBody,4096);
////
////				sb.Replace("[=Title=]", IncidentTitle);
////				sb.Replace("[=Ticket=]", IncidentTicket);
////				sb.Replace("[=Text=]", node.Text);
////				sb.Replace("[=FirstName=]", Security.CurrentUser.FirstName);
////				sb.Replace("[=LastName=]", Security.CurrentUser.LastName);
////
////				FullMessage = sb.ToString();
////			}
////			else
////			{
//                FullMessage = node.Text;
////			}

//            sbMessage.Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(FullMessage))).Append("\r\n");

//            tmpBuffer = Encoding.ASCII.GetBytes(sbMessage.ToString());
//            emlMessage.Write(tmpBuffer,0, tmpBuffer.Length);

//            Hashtable contentTypeHash = new Hashtable();

//            using(IDataReader reader = ContentType.GetListContentTypes())
//            {
//                while(reader.Read())
//                {
//                    contentTypeHash.Add(((string)reader["Extension"]).ToLower(), (string)reader["ContentTypeString"]);
//                }
//            }


//            // Add Pop3 Message Attachements
//            foreach(Mediachase.IBN.Business.ControlSystem.FileInfo  fileInfo in fileStorage.GetFiles())
//            {
//                StringBuilder sbFile = new StringBuilder();

//                sbFile.Append("------------7E143249668A83E").Append("\r\n");
//                sbFile.AppendFormat("Content-Type: {0}; name=\"{1}\"", fileInfo.FileBinaryContentType, Rfc822HeaderCollection.Encode2AsciiString(fileInfo.Name)).Append("\r\n");
//                sbFile.Append("Content-Transfer-Encoding: base64").Append("\r\n");
//                sbFile.AppendFormat("Content-Disposition: attachment; filename=\"{0}\"", Rfc822HeaderCollection.Encode2AsciiString(fileInfo.Name)).Append("\r\n");
//                sbFile.Append("\r\n");

//                using(MemoryStream fs = new MemoryStream())
//                {
//                    FileStorage.LightLoadFile(fileInfo, fs);
//                    fs.Capacity = (int)fs.Length;

//                    sbFile.Append(Convert.ToBase64String(fs.GetBuffer()));
//                }

//                sbFile.Append("\r\n");

//                tmpBuffer = Encoding.ASCII.GetBytes(sbFile.ToString());
//                emlMessage.Write(tmpBuffer,0, tmpBuffer.Length);
//            }

//            // Add Final Line
//            tmpBuffer = Encoding.ASCII.GetBytes("------------7E143249668A83E--\r\n\r\n");
//            emlMessage.Write(tmpBuffer,0, tmpBuffer.Length);

//            #endregion

//            Pop3Message InMsg = new Pop3Message(emlMessage);

//            // Send Email
//            #region Internal -> Internal Info

//            // Internal -> Internal Info
//            if(AllowEMailRouting)
//            {
//                // 2007-02-12 OZ: "FN LN" <email@domain.com>
//                string FromEmail = string.Format("\"{0} {1}\" <{2}>",
//                    Security.CurrentUser.FirstName, 
//                    Security.CurrentUser.LastName,
//                    pop3Box.EMailAddress);

//                OutputMessageCreator output2Iternal = new OutputMessageCreator(InMsg, IncidentId, pop3Box.EMailAddress, FromEmail);
	
//                // Exclude a message sender
//                output2Iternal.AddIgnoreRecipient(EMailMessage.GetSenderEmail(InMsg));
	
//                // Load InternalUser
//                foreach(int UserId in GetInternalUsersByIncidentId(IncidentId))
//                {
//                    output2Iternal.AddRecipient(UserId);
//                    retVal.Add(UserId);
//                }
	
//                foreach(OutputMessage outputMsg in output2Iternal.Create())
//                {
//                    try
//                    {
//                        SmtpClient.SendMessage(outputMsg.MailFrom, outputMsg.RcptTo, outputMsg.Subject, outputMsg.Data);
//                    }
//                    catch(Exception ex)
//                    {
//                        System.Diagnostics.Trace.WriteLine(ex);
//                        Log.WriteError(ex.ToString());
//                    }
//                }
//            }
//            #endregion

//            #region Internal -> Extrenal
//            // Internal -> Extrenal
//            string fromEMailAddress =  FindEMailRouterPublicEmail(IncidentId);

//            if(fromEMailAddress!=string.Empty)
//            {
//                //EMailRouterPop3Box externalPop3Box = EMailRouterPop3Box.Load(realEMailBoxId);

//                OutputMessageCreator output2External = new OutputMessageCreator(InMsg, IncidentId, fromEMailAddress, fromEMailAddress);

//                // Load External Senders
//                //foreach(string exRecipient in EMailMessage.GetExternalSendersByIncidentId(IncidentId))

//                // 2006-12-12 OZ: Load External Senders
//                foreach(EMailIssueExternalRecipient exRecipient in EMailIssueExternalRecipient.List(IncidentId))
//                {
//                    output2External.AddRecipient(exRecipient.EMail);
//                }

//                foreach(OutputMessage outputMsg in output2External.Create())
//                {
//                    try
//                    {
//                        SmtpClient.SendMessage(outputMsg.MailFrom, outputMsg.RcptTo, outputMsg.Subject, outputMsg.Data);
//                    }
//                    catch(Exception ex)
//                    {
//                        System.Diagnostics.Trace.WriteLine(ex);
//                        Log.WriteError(ex.ToString());
//                    }
//                }

//            }
//            #endregion

//            return retVal;
//        }

		public static string FindEMailRouterPublicEmail(int IncidentId)
		{
			int realEMailBoxId = EMailMessage.GetEMailRouterPop3BoxIdByIssueId(IncidentId);
			if(realEMailBoxId==-1)
			{
				EMailRouterPop3Box[] emailBoxList = EMailRouterPop3Box.ListExternal();

				// OZ 2009-01-20 Find EmailBox mapped with IncidentBox
				int incidentBoxId = Incident.GetIncidentBox(IncidentId);

				foreach (EMailRouterPop3Box emailBox in emailBoxList)
				{
					if (emailBox.Settings.DefaultEMailIncidentMappingBlock.IncidentBoxId == incidentBoxId)
					{
						return emailBox.EMailAddress;
					}
				}
				//

				// try to find active
				foreach(EMailRouterPop3Box emailBox in emailBoxList)
				{
					if(emailBox.Activity.IsActive)
					{
						return emailBox.EMailAddress;
					}
				}

				if (emailBoxList.Length > 0)
					return emailBoxList[0].EMailAddress;

                if (Security.CurrentUser != null)
                    return Security.CurrentUser.Email;

                return Alerts2.AlertSenderEmail;
			}
			else
			{
				EMailRouterPop3Box emailBox = EMailRouterPop3Box.Load(realEMailBoxId);
				return emailBox.EMailAddress;
			}
		}

		/// <summary>
		/// Finds the E mail router public id.
		/// </summary>
		/// <param name="IncidentId">The incident id.</param>
		/// <returns></returns>
        public static int FindEMailRouterPublicId(int IncidentId)
        {
            int realEMailBoxId = EMailMessage.GetEMailRouterPop3BoxIdByIssueId(IncidentId);
            if (realEMailBoxId == -1)
            {
                EMailRouterPop3Box[] emailBoxList = EMailRouterPop3Box.ListExternal();

				// OZ 2009-01-20 Find EmailBox mapped with IncidentBox
				int incidentBoxId = Incident.GetIncidentBox(IncidentId);

				foreach (EMailRouterPop3Box emailBox in emailBoxList)
				{
					if (emailBox.Settings.DefaultEMailIncidentMappingBlock.IncidentBoxId == incidentBoxId)
						return emailBox.EMailRouterPop3BoxId;
				}
				//

                // try to find firt active
                foreach (EMailRouterPop3Box emailBox in emailBoxList)
                {
                    if (emailBox.Activity.IsActive)
                    {
                        return emailBox.EMailRouterPop3BoxId;
                    }
                }
				//

				if (emailBoxList.Length > 0)
					return emailBoxList[0].EMailRouterPop3BoxId;

				return -1;
            }
            else
            {
                return realEMailBoxId;
            }
        }


		public static ArrayList Send(int IncidentId, int EMailMessageId)
		{
			EMailMessageRow row = new EMailMessageRow(EMailMessageId);

			return Send(IncidentId, EMailRouterPop3Box.Load(row.EMailRouterPop3BoxId), EMailMessage.GetPop3Message(row.EmlMessage));
		}

        public static ArrayList Send(int IncidentId, EMailRouterPop3Box pop3Box, Pop3Message InMsg)
        {
            return Send(IncidentId, pop3Box, InMsg, null);
        }

		public static ArrayList Send(int IncidentId, EMailRouterPop3Box pop3Box, Pop3Message InMsg, string[] RecipientEmails)
		{
			ArrayList retVal = new ArrayList();

			IncidentBox incidentBox = IncidentBox.Load( Incident.GetIncidentBox(IncidentId));

			EMailRouterIncidentBoxBlock settings = IncidentBoxDocument.Load(incidentBox.IncidentBoxId).EMailRouterBlock;

			if(pop3Box.IsInternal)
			{
				#region Internal -> Extrenal
				// Internal -> Internal Info

				// 2009-06-16 OZ: Add Chech send emails to internal users
				if (settings.AllowEMailRouting)
				{
					// 2007-02-12 OZ: "FN LN" <email@domain.com>
					string FromEmail = string.Format("\"{0} {1}\" <{2}>",
						Security.CurrentUser.LastName,
						Security.CurrentUser.FirstName,
						pop3Box.EMailAddress);

					OutputMessageCreator output2Iternal = new OutputMessageCreator(InMsg, IncidentId, pop3Box.EMailAddress, FromEmail);

					// Exclude a message sender
					//output2Iternal.AddIgnoreRecipient(EMailMessage.GetSenderEmail(InMsg));

					// Load InternalUser
					foreach (int UserId in GetInternalUsersByIncidentId(IncidentId))
					{
						output2Iternal.AddRecipient(UserId);
						retVal.Add(UserId);
					}

					// Load InformationRecipientList
					//				foreach(int infoRecipient in IncidentBoxDocument.Load(incidentBox.IncidentBoxId).EMailRouterBlock.InformationRecipientList)
					//				{
					//					output2Iternal.AddRecipient(infoRecipient);
					//				}

					foreach (OutputMessage outputMsg in output2Iternal.Create())
					{
						//try
						//{
						SmtpClientUtility.SendMessage(OutgoingEmailServiceType.HelpDeskEmailBox, pop3Box.EMailRouterPop3BoxId, outputMsg.MailFrom, outputMsg.RcptTo, outputMsg.Subject, outputMsg.Data);
						//}
						//catch(Exception ex)
						//{
						//    System.Diagnostics.Trace.WriteLine(ex);
						//    Log.WriteError(ex.ToString());
						//}
					}
				}

				// Internal -> Extrenal
				string publicEmail = FindEMailRouterPublicEmail(IncidentId);

                if (publicEmail!=string.Empty)
				{
					//EMailRouterPop3Box externalPop3Box = EMailRouterPop3Box.Load(realEMailBoxId);

                    OutputMessageCreator output2External = new OutputMessageCreator(InMsg, IncidentId, publicEmail, publicEmail);

					// Load External Senders
//					foreach(string exRecipient in EMailMessage.GetExternalSendersByIncidentId(IncidentId))
//					{
//						output2External.AddRecipient(exRecipient);
//					}

					// Load External Senders
					foreach (EMailIssueExternalRecipient exRecipient in EMailIssueExternalRecipient.List(IncidentId))
					{
						output2External.AddRecipient(exRecipient.EMail);
					}

                    // 2007-03-12 RecipientEmails Addon
                    if (RecipientEmails != null)
                    {
                        foreach (string exItem in RecipientEmails)
                        {
                            int emailUserId = DBUser.GetUserByEmail(exItem, false);
                            if (emailUserId > 0)
                            {
                                if (!retVal.Contains(emailUserId))
                                {
                                    output2External.AddRecipient(exItem);
                                    retVal.Add(emailUserId);
                                }
                            }
                            else
                                output2External.AddRecipient(exItem);
                        }
                    }
                    //

					int emailBoxId = EMail.EMailRouterOutputMessage.FindEMailRouterPublicId(IncidentId);

					foreach(OutputMessage outputMsg in output2External.Create())
					{
                        //try
                        //{
						SmtpClientUtility.SendMessage(OutgoingEmailServiceType.HelpDeskEmailBox, emailBoxId, outputMsg.MailFrom, outputMsg.RcptTo, outputMsg.Subject, outputMsg.Data);
                        //}
                        //catch(Exception ex)
                        //{
                        //    System.Diagnostics.Trace.WriteLine(ex);
                        //    Log.WriteError(ex.ToString());
                        //}
					}
				}
				#endregion
			}
			else
			{
				if (!settings.AllowEMailRouting)
					return retVal;

				EMailRouterPop3Box internalPop3Box = EMailRouterPop3Box.ListInternal();
				if (internalPop3Box == null)
					return retVal;

				#region // External -> Internal
				// External -> Internal

				string SenderName = EMailMessage.GetSenderName(InMsg);
				string FromEmail = SenderName==string.Empty?internalPop3Box.EMailAddress:
					string.Format("\"{0}\" <{1}>",
					SenderName,
					internalPop3Box.EMailAddress);

				OutputMessageCreator output = new OutputMessageCreator(InMsg, IncidentId, internalPop3Box.EMailAddress, FromEmail);

				string Subject = (InMsg.Subject==null?string.Empty:InMsg.Subject);

				if(Subject==string.Empty)
				{
					// OZ: Maybe:  Set Default Inicdent Title if subject is empty
					//Subject = Incident.GetIncidentTitle(IncidentId);
				}

				if(TicketUidUtil.LoadFromString(Subject)==string.Empty)
					output.Subject = string.Format("[{0}] {1}",TicketUidUtil.Create(incidentBox.IdentifierMask,IncidentId), Subject);

				foreach(int UserId in GetInternalUsersByIncidentId(IncidentId))
				{
					output.AddRecipient(UserId);
					retVal.Add(UserId);
				}

//				IncidentBoxDocument incidentDoc = IncidentBoxDocument.Load(incidentBox.IncidentBoxId);
//
//				foreach(int infoRecipient in incidentDoc.EMailRouterBlock.InformationRecipientList)
//				{
//					output.AddRecipient(infoRecipient);
//				}
				
				foreach(OutputMessage outputMsg in output.Create())
				{
                    //try
                    //{
					SmtpClientUtility.SendMessage(OutgoingEmailServiceType.HelpDeskEmailBox, internalPop3Box.EMailRouterPop3BoxId, outputMsg.MailFrom, outputMsg.RcptTo, outputMsg.Subject, outputMsg.Data);
                    //}
                    //catch(Exception ex)
                    //{
                    //    System.Diagnostics.Trace.WriteLine(ex);
                    //    Log.WriteError(ex.ToString());
                    //}
				}
				#endregion
			}
			
			return retVal;
		}

		private static ArrayList GetInternalUsersByIncidentId(int IncidentId)
		{
			return Incident.GetInternalEmailRecipients(IncidentId);
		}
	}
}
