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
using System.Web;
using System.Globalization;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for EMailMessage.
	/// </summary>
	public class EMailMessage
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EMailMessage"/> class.
		/// </summary>
		private EMailMessage()
		{
		}

		/// <summary>
		/// Gets the POP3 message.
		/// </summary>
		/// <param name="EmlMessageBuffer">The eml message buffer.</param>
		/// <returns></returns>
		public static Pop3Message GetPop3Message(byte[] EmlMessageBuffer)
		{
			MemoryStream memStream = new MemoryStream(EmlMessageBuffer.Length);
			memStream.Write(EmlMessageBuffer, 0, EmlMessageBuffer.Length);
			memStream.Position = 0;

			return new Pop3Message(memStream);
		}

		/// <summary>
		/// Gets the POP3 message.
		/// </summary>
		/// <param name="EMailMessageId">The E mail message id.</param>
		/// <returns></returns>
		public static Pop3Message GetPop3Message(int EMailMessageId)
		{
			EMailMessageRow row = new EMailMessageRow(EMailMessageId);
			return GetPop3Message(row.EmlMessage);
		}

		/// <summary>
		/// Gets the POP3 message bytes.
		/// </summary>
		/// <param name="msg">The MSG.</param>
		/// <returns></returns>
		internal static byte[] GetPop3MessageBytes(Pop3Message msg)
		{
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
			{
				byte[] tmpBuffer = new byte[64 * 1024];

				msg.InputStream.Position = 0;

				int RealRead = 0;
				while ((RealRead = msg.InputStream.Read(tmpBuffer, 0, 64 * 1024)) != 0)
				{
					ms.Write(tmpBuffer, 0, RealRead);
				}
				ms.Flush();
				ms.Capacity = (int)ms.Length;
				return ms.GetBuffer();
			}
		}

		/// <summary>
		/// Extracts the first email.
		/// </summary>
		/// <param name="addresses">The addresses.</param>
		/// <returns></returns>
		private static string ExtractFirstEmail(string addresses)
		{
			if (addresses == null)
				return string.Empty;

			Match match = Regex.Match(addresses, "((?<phrase>[^:]+):)?(?<mailbox>(((?<show_name>[^<]*)<)?(?<email>(?<localpart>[^@]+)@(?<domain>[^>]+))(>)?));?",
				RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

			if (match.Success &&
				match.Groups["email"] != null &&
				match.Groups["email"].Captures.Count > 0)
			{
				return match.Groups["email"].Captures[0].Value;
			}

			return addresses;
		}

		/// <summary>
		/// Extracts the name of the first show.
		/// </summary>
		/// <param name="addresses">The addresses.</param>
		/// <returns></returns>
		private static string ExtractFirstShowName(string addresses)
		{
			if (string.IsNullOrEmpty(addresses))
				return string.Empty;

			Match match = Regex.Match(addresses,
				"((?<phrase>[^:]+):)?(?<mailbox>(((?<show_name>[^<]*)<)?(?<email>(?<localpart>[^@]+)@(?<domain>[^>]+))(>)?));?",
				RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

			if (match.Success &&
				match.Groups["show_name"] != null &&
				match.Groups["show_name"].Captures.Count > 0)
			{
				string showName = match.Groups["show_name"].Captures[0].Value;
				if (showName != null)
				{
					showName = showName.Trim(' ', '"');
					if (showName != string.Empty)
						return showName;
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// Gets the sender email.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		internal static string GetSenderEmail(Pop3Message message)
		{
			if (message.Headers["Reply-To"] != null)
				return EMailMessage.ExtractFirstEmail(message.Headers["Reply-To"]);

			//if (message.Headers["Sender"] != null)
			//    return EMailMessage.ExtractFirstEmail(message.Headers["Sender"]);
			return EMailMessage.ExtractFirstEmail(message.Headers["From"]);
		}

		/// <summary>
		/// Gets the name of the sender.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		internal static string GetSenderName(Pop3Message message)
		{
			if (message.Headers["Reply-To"] != null)
				return EMailMessage.ExtractFirstEmail(message.Headers["Reply-To"]);

			//if (message.Headers["Sender"] != null)
			//    return EMailMessage.ExtractFirstShowName(message.Headers["Sender"]);

			return EMailMessage.ExtractFirstShowName(message.Headers["From"]);
		}

		/// <summary>
		/// Creates the specified E mail router POP3 box id.
		/// </summary>
		/// <param name="EMailRouterPop3BoxId">The E mail router POP3 box id.</param>
		/// <param name="messageStream">The message stream.</param>
		/// <returns></returns>
		public static int CreateFromStream(int EMailRouterPop3BoxId, Stream messageStream)
		{
			if (messageStream == null)
				throw new ArgumentNullException("messageStream");

			using (System.IO.StreamReader st = new System.IO.StreamReader(messageStream))
			{
				string strmsg = st.ReadToEnd();

				byte[] buffer = System.Text.Encoding.Default.GetBytes(strmsg);
				System.IO.MemoryStream memStream = new System.IO.MemoryStream(buffer, 0, 
					buffer.Length, true, true);

				return Create(EMailRouterPop3BoxId, new Pop3Message(memStream));
			}
		}

		/// <summary>
		/// Creates the specified message.
		/// </summary>
		/// <param name="EMailRouterPop3BoxId">The E mail router POP3 box id.</param>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		public static int Create(int EMailRouterPop3BoxId, Pop3Message message)
		{
			if (message == null)
				throw new ArgumentNullException("message");

			EMailMessageRow newRow = new EMailMessageRow();

			newRow.EMailRouterPop3BoxId = EMailRouterPop3BoxId;
			//newRow.Created = DateTime.UtcNow;
			newRow.From = EMailMessage.GetSenderEmail(message);

			//if (message.Sender != null)
			//    newRow.From = EMailMessage.ExtractFirstEmail(message.Sender.Email);
			//else
			//    newRow.From = EMailMessage.ExtractFirstEmail(message.From.Email);

			newRow.To = EMailMessage.ExtractFirstEmail(message.To);

			newRow.Subject = message.Subject == null ? string.Empty : message.Subject;

			newRow.EmlMessage = GetPop3MessageBytes(message);

			newRow.Update();

			return newRow.PrimaryKeyId;
		}

		/// <summary>
		/// Approves the pending.
		/// </summary>
		/// <param name="EMailMessageIdList">The E mail message id list.</param>
		public static void ApprovePending(ArrayList EMailMessageIdList)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				foreach (int EMailMessageId in EMailMessageIdList)
				{
					ApprovePending(EMailMessageId);
				}

				tran.Commit();
			}
		}

		/// <summary>
		/// Approves the pending.
		/// </summary>
		public static void ApprovePending()
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				foreach (int EMailMessageId in EMailMessage.ListPendigEMailMessageIds())
				{
					ApprovePending(EMailMessageId);
				}

				tran.Commit();
			}
		}

		/// <summary>
		/// Finds the incident by ticket.
		/// </summary>
		/// <param name="Ticket">The ticket.</param>
		/// <returns></returns>
		private static int FindIncidentByTicket(string Ticket)
		{
			int IncidentId = TicketUidUtil.GetThreadId(Ticket);

			using (IDataReader reader = DBIncident.GetIncident(IncidentId, 0, 1))
			{
				if (reader.Read())
					return IncidentId;
			}
			return -1;
		}



		/// <summary>
		/// Approves the pending.
		/// </summary>
		/// <param name="EMailMessageId">The E mail message id.</param>
		public static void ApprovePending(int EMailMessageId)
		{
			// Stop Double Approve
			if (!PendingEMailMessageRow.Contains(EMailMessageId))
				return;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				// Remove from pending
				PendingEMailMessageRow.DeleteByEMailMessageId(EMailMessageId);

				// Load Pop3 Message
				EMailMessageInfo msgInfo = EMailMessageInfo.Load(EMailMessageId);
				EMailRouterPop3Box emailBox = EMailRouterPop3Box.Load(msgInfo.EMailRouterPop3BoxId);

				Pop3Message msg = EMailMessage.GetPop3Message(EMailMessageId);

				// Add User to White Lits
				string SenderEmail = EMailMessage.GetSenderEmail(msg);
				//if(!WhiteListItem.Contains(SenderEmail))

				if (PortalConfig.UseAntiSpamFilter && PortalConfig.AutoFillWhiteList)
					WhiteListItem.Create(SenderEmail);

				string TicketUID = TicketUidUtil.LoadFromString(msg.Subject == null ? string.Empty : msg.Subject);

				if (TicketUID == string.Empty)
				{
					int IncidentId = CreateNewIncident(EMailMessageId, emailBox, msg);

					// Automaticaly by Incident.CreateFromEmail
					//EMailRouterOutputMessage.Send(IncidentId, emailBox, msg);
				}
				else
				{
					// Assign By Ticket
					int IncidentId = FindIncidentByTicket(TicketUID);

					if (IncidentId != -1)
					{
						int creatorId, issueBoxId;
						using (IDataReader reader = Incident.GetIncident(IncidentId))
						{
							reader.Read();

							creatorId = (int)reader["CreatorId"];
							issueBoxId = (int)reader["IncidentBoxId"];
						}

						int stateId, managerId, responsibleId;
						bool isResposibleGroup;
						ArrayList users = new ArrayList();
						Issue2.GetIssueBoxSettings(issueBoxId, out stateId, out managerId, out responsibleId, out isResposibleGroup, users);

						UserLight prevUser = LogOnCreator(creatorId, msg);

						int ThreadNodeId = AddToIncidentMessage(emailBox.IsInternal, IncidentId, EMailMessageId, msg);

						if (ProcessXIbnHeaders(IncidentId, ThreadNodeId, msg))
						{
							ArrayList excludeUsers = EMailRouterOutputMessage.Send(IncidentId, emailBox, msg);

							// O.R. [2008-09-09]: Exclude inactive users
							if (responsibleId > 0 && User.GetUserActivity(responsibleId) != User.UserActivity.Active)
								responsibleId = -1;
							ArrayList activeUsers = new ArrayList();
							foreach (int userId in users)
							{
								if (User.GetUserActivity(userId) == User.UserActivity.Active)
									activeUsers.Add(userId);
							}
							//

							Issue2.SendAlertsForNewIssue(IncidentId, managerId, responsibleId, activeUsers, excludeUsers);
						}
					}
					else
					{
						IncidentId = CreateNewIncident(EMailMessageId, emailBox, msg);

						// Automaticaly by Incident.CreateFromEmail
						//EMailRouterOutputMessage.Send(IncidentId, emailBox, msg);
					}
				}
				tran.Commit();
			}
		}

		/// <summary>
		/// Copies to incident.
		/// </summary>
		/// <param name="emailMessageIdList">The email message id list.</param>
		/// <param name="incidentId">The incident id.</param>
		public static void CopyToIncident(ArrayList emailMessageIdList, int incidentId)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				foreach (int emailMessageId in emailMessageIdList)
				{
					CopyToIncident(emailMessageId, incidentId);
				}

				tran.Commit();
			}
		}

		/// <summary>
		/// Copies to incident.
		/// </summary>
		/// <param name="emailMessageId">The email message id.</param>
		/// <param name="incidentId">The incident id.</param>
		public static void CopyToIncident(int emailMessageId, int incidentId)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				// Remove from pending
				PendingEMailMessageRow.DeleteByEMailMessageId(emailMessageId);

				// Load Pop3 Message
				EMailMessageInfo msgInfo = EMailMessageInfo.Load(emailMessageId);
				EMailRouterPop3Box emailBox = EMailRouterPop3Box.Load(msgInfo.EMailRouterPop3BoxId);

				Pop3Message msg = EMailMessage.GetPop3Message(emailMessageId);

				// Add User to White Lits
				string SenderEmail = EMailMessage.GetSenderEmail(msg);
				//if(!WhiteListItem.Contains(SenderEmail))

				if (PortalConfig.UseAntiSpamFilter && PortalConfig.AutoFillWhiteList)
					WhiteListItem.Create(SenderEmail);


					int creatorId, issueBoxId;
					using (IDataReader reader = Incident.GetIncident(incidentId))
					{
						reader.Read();

						creatorId = (int)reader["CreatorId"];
						issueBoxId = (int)reader["IncidentBoxId"];
					}

					int stateId, managerId, responsibleId;
					bool isResposibleGroup;
					ArrayList users = new ArrayList();
					Issue2.GetIssueBoxSettings(issueBoxId, out stateId, out managerId, out responsibleId, out isResposibleGroup, users);

					UserLight prevUser = LogOnCreator(creatorId, msg);

					int ThreadNodeId = AddToIncidentMessage(emailBox.IsInternal, incidentId, emailMessageId, msg);

					if (ProcessXIbnHeaders(incidentId, ThreadNodeId, msg))
					{
						ArrayList excludeUsers = EMailRouterOutputMessage.Send(incidentId, emailBox, msg);

						// O.R. [2008-09-09]: Exclude inactive users
						if (responsibleId > 0 && User.GetUserActivity(responsibleId) != User.UserActivity.Active)
							responsibleId = -1;
						ArrayList activeUsers = new ArrayList();
						foreach (int userId in users)
						{
							if (User.GetUserActivity(userId) == User.UserActivity.Active)
								activeUsers.Add(userId);
						}
						//

						Issue2.SendAlertsForNewIssue(incidentId, managerId, responsibleId, activeUsers, excludeUsers);
					}

				tran.Commit();
			}
		}

		/// <summary>
		/// Deletes the specified E mail message id.
		/// </summary>
		/// <param name="EMailMessageId">The E mail message id.</param>
		public static void Delete(int EMailMessageId)
		{
			ArrayList list = new ArrayList();
			list.Add(EMailMessageId);

			Delete(list);
		}

		/// <summary>
		/// Deletes the specified E mail message id list.
		/// </summary>
		/// <param name="EMailMessageIdList">The E mail message id list.</param>
		public static void Delete(ArrayList EMailMessageIdList)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				foreach (int EMailMessageId in EMailMessageIdList)
				{
					if (PortalConfig.UseAntiSpamFilter && PortalConfig.AutoFillBlackList)
					{
						string senderEmail = EMailMessageRow.GetSenderEmail(EMailMessageId);

						if (!string.IsNullOrEmpty(senderEmail) && !BlackListItem.Contains(senderEmail))
							BlackListItem.Create(senderEmail);
					}

					EMailMessageRow.Delete(EMailMessageId);
				}

				tran.Commit();
			}
		}

		/// <summary>
		/// Marks as peinding message.
		/// </summary>
		/// <param name="EMailMessageId">The E mail message id.</param>
		public static void MarkAsPendingMessage(int EMailMessageId)
		{
			PendingEMailMessageRow pendigRow = new PendingEMailMessageRow();
			pendigRow.EMailMessageId = EMailMessageId;
			pendigRow.Update();
		}

		/// <summary>
		/// Marks as peinding message.
		/// </summary>
		/// <param name="EMailMessageId">The E mail message id.</param>
		//		public static void MarkAsManualAssign(int EMailMessageId)
		//		{
		//			ManualAssignEMailMessageRow row = new ManualAssignEMailMessageRow();
		//			row.EMailMessageId = EMailMessageId;
		//			row.Update();
		//		}

		/// <summary>
		/// Lists the pendig E mail message ids.
		/// </summary>
		/// <returns></returns>
		public static int[] ListPendigEMailMessageIds()
		{
			ArrayList retVal = new ArrayList();

			foreach (PendingEMailMessageRow row in PendingEMailMessageRow.List())
			{
				retVal.Add(row.EMailMessageId);
			}

			return (int[])retVal.ToArray(typeof(int));
		}


		/// <summary>
		/// Gets the owner incident id.
		/// </summary>
		/// <param name="EMailMessageId">The E mail message id.</param>
		/// <returns></returns>
		public static int GetOwnerIncidentId(int EMailMessageId)
		{
			return EMailMessageRow.GetOwnerIncidentId(EMailMessageId);
		}

		/// <summary>
		/// Gets the E mail router POP3 box id by issue id.
		/// </summary>
		/// <param name="IncidentId">The incident id.</param>
		/// <returns></returns>
		public static int GetEMailRouterPop3BoxIdByIssueId(int IncidentId)
		{
			return EMailMessageRow.GetEMailRouterPop3BoxIdByIssueId(IncidentId);
		}

		//		public static string[] GetExternalSendersByIncidentId(int IncidentId)
		//		{
		//			return EMailMessageRow.GetExternalSendersByIncidentId(IncidentId);
		//		}

		/// <summary>
		/// Logs the on creator.
		/// </summary>
		/// <param name="DefaultCreatorId">The default creator id.</param>
		/// <param name="msg">The MSG.</param>
		/// <returns></returns>
		internal static UserLight LogOnCreator(int DefaultCreatorId, Pop3Message msg)
		{
			if (Security.CurrentUser != null)
				return Security.CurrentUser;

			int creatorId = DefaultCreatorId;

			int emailUser = DBUser.GetUserByEmail(EMailMessage.GetSenderEmail(msg), false);

			if (emailUser > 0)
			{
				// OZ: 2007-01-26: Fix problem: Mediachase.IBN.Business.AccessDeniedException: Access denied.
				//   at Mediachase.IBN.Business.Incident.CreateFromEmail
				using (IDataReader reader = DBUser.GetUserInfo(emailUser))
				{
					if (reader.Read())
					{
						if (!(bool)reader["IsActive"])
						{
							emailUser = -1;
						}
					}
					else
					{
						emailUser = -1;
					}
				}
				//
			}

			if (emailUser > 0)
				creatorId = emailUser;

			UserLight creator = UserLight.Load(creatorId);
			return Security.SetCurrentUser(creator);
		}

		/// <summary>
		/// Creates the new incident.
		/// </summary>
		/// <param name="EMailMessageId">The E mail message id.</param>
		/// <param name="emailBox">The email box.</param>
		/// <param name="msg">The MSG.</param>
		/// <returns></returns>
		public static int CreateNewIncident(int EMailMessageId, EMailRouterPop3Box emailBox, Pop3Message msg)
		{
			IEMailIncidentMapping mappingHandler = EMailIncidentMappingHandler.LoadHandler(emailBox.Settings.SelectedHandlerId);

			// Step. Mapping
			IncidentInfo incidentInfo = mappingHandler.Create(emailBox, msg);

			// Set Creator
			UserLight prevUser = LogOnCreator(incidentInfo.CreatorId, msg);

			incidentInfo.CreatorId = Security.CurrentUser.UserID;

			// Step. Evaluate IncidentBox
			IncidentBox incidentBox = null;

			if (incidentInfo.IncidentBoxId != -1)
				incidentBox = IncidentBox.Load(incidentInfo.IncidentBoxId);
			else
				incidentBox = IncidentBoxRule.Evaluate(incidentInfo);

			// Step. Create Incident
			int IncidentId = Incident.CreateFromEmail(incidentInfo.Title,
				incidentInfo.Description,
				incidentInfo.ProjectId,
				incidentInfo.TypeId,
				incidentInfo.PriorityId,
				incidentInfo.SeverityId,
				incidentBox.Document.GeneralBlock.TaskTime,
				incidentBox.Document.GeneralBlock.ExpectedDuration,
				incidentBox.Document.GeneralBlock.ExpectedResponseTime,
				incidentBox.Document.GeneralBlock.ExpectedAssignTime,
				incidentInfo.GeneralCategories,
				incidentInfo.IncidentCategories,
				incidentBox.IncidentBoxId,
				EMailMessageId,
				incidentInfo.OrgUid,
				incidentInfo.ContactUid);

			return IncidentId;
		}

		/// <summary>
		/// Adds to incident message.
		/// </summary>
		/// <param name="IsInternal">if set to <c>true</c> [is internal].</param>
		/// <param name="IncidentId">The incident id.</param>
		/// <param name="EMailMessageId">The E mail message id.</param>
		/// <returns></returns>
		public static int AddToIncidentMessage(bool IsInternal, int IncidentId, int EMailMessageId)
		{
			Pop3Message message = EMailMessage.GetPop3Message(EMailMessageId);
			return AddToIncidentMessage(IsInternal, IncidentId, EMailMessageId, message);
		}

		/// <summary>
		/// Adds to incident message.
		/// </summary>
		/// <param name="IsInternal">if set to <c>true</c> [is internal].</param>
		/// <param name="IncidentId">The incident id.</param>
		/// <param name="EMailMessageId">The E mail message id.</param>
		/// <param name="msg">The MSG.</param>
		/// <returns></returns>
		public static int AddToIncidentMessage(bool IsInternal, int IncidentId, int EMailMessageId, Pop3Message msg)
		{
			IncidentBox incidentBox = IncidentBox.Load(Incident.GetIncidentBox(IncidentId));

			if (IsInternal)
			{
				// 2007-02-15: OZ: New Messagew Addon
				Issue2.SetNewMessage(IncidentId, false);
				//

				// TODO: If Responsible is Group, set current user as responsible

				return AddInternalEMail2Incident(IncidentId, EMailMessageId, EMailMessage.GetSenderName(msg), EMailMessage.GetSenderEmail(msg));
			}
			else
			{
				// 2007-02-15: OZ: New Messagew Addon
				Issue2.SetNewMessage(IncidentId, true);
				//

				return AddExternalEMail2Incident(IncidentId, EMailMessageId, EMailMessage.GetSenderName(msg), EMailMessage.GetSenderEmail(msg));
			}

		}


		/// <summary>
		/// Adds the external E mail to incident.
		/// </summary>
		/// <param name="IncidentId">The incident id.</param>
		/// <param name="EMailMessageId">The E mail message id.</param>
		/// <param name="message">The message.</param>
		private static int AddExternalEMail2Incident(int IncidentId, int EMailMessageId, string SenderName, string SenderEmail)
		{
			ForumThreadNodeInfo info = null;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				BaseIbnContainer FOcontainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", IncidentId));
				ForumStorage forumStorage = (ForumStorage)FOcontainer.LoadControl("ForumStorage");

				EMailMessageInfo mi = EMailMessageInfo.Load(EMailMessageId);

				info = forumStorage.CreateForumThreadNode(EMailMessageInfo.ExtractTextFromHtml(mi.HtmlBody),
					DateTime.UtcNow,
					Security.UserID,
					EMailMessageId,
					(int)ForumStorage.NodeContentType.EMail);

				// 2006-12-12 OZ: Register EMail External Recepient
				if (!EMailIssueExternalRecipient.Contains(IncidentId, SenderEmail))
					EMailIssueExternalRecipient.Create(IncidentId, SenderEmail);

				ForumThreadNodeSettingCollection settings = new ForumThreadNodeSettingCollection(info.Id);
				settings.Add(ForumThreadNodeSetting.Incoming, "1");

				if (HttpContext.Current != null && HttpContext.Current.Items.Contains(ForumThreadNodeSetting.AllRecipients))
					settings.Add(ForumThreadNodeSetting.AllRecipients, HttpContext.Current.Items[ForumThreadNodeSetting.AllRecipients].ToString());

				// O.R. [2008-09-19]: Recalculate Expected Dates
				DBIncident.RecalculateExpectedResponseDate(IncidentId);
				DBIncident.RecalculateExpectedAssignDate(IncidentId);

				tran.Commit();
			}

			return info.Id;
		}

		/// <summary>
		/// Adds the internal E mail2 incident.
		/// </summary>
		/// <param name="IncidentId">The incident id.</param>
		/// <param name="EMailMessageId">The E mail message id.</param>
		/// <param name="SenderName">Name of the sender.</param>
		/// <param name="SenderEmail">The sender email.</param>
		/// <returns></returns>
		private static int AddInternalEMail2Incident(int IncidentId, int EMailMessageId, string SenderName, string SenderEmail)
		{
			ForumThreadNodeInfo info = null;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				BaseIbnContainer FOcontainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", IncidentId));
				ForumStorage forumStorage = (ForumStorage)FOcontainer.LoadControl("ForumStorage");

				int SenderUserID = Security.UserID;
				EMailMessageInfo mi = EMailMessageInfo.Load(EMailMessageId);

				info = forumStorage.CreateForumThreadNode(EMailMessageInfo.ExtractTextFromHtml(mi.HtmlBody),
					DateTime.UtcNow,
					Security.UserID,
					EMailMessageId,
					(int)ForumStorage.NodeContentType.EMail);

				ForumThreadNodeSettingCollection settings = new ForumThreadNodeSettingCollection(info.Id);
				settings.Add(ForumThreadNodeSetting.Outgoing, "1");

				if (HttpContext.Current != null && HttpContext.Current.Items.Contains(ForumThreadNodeSetting.AllRecipients))
					settings.Add(ForumThreadNodeSetting.AllRecipients, HttpContext.Current.Items[ForumThreadNodeSetting.AllRecipients].ToString());

				tran.Commit();
			}

			return info.Id;
		}

		/// <summary>
		/// Processes the X ibn headers.
		/// </summary>
		/// <param name="IncidentId">The incident id.</param>
		/// <param name="ThreadNodeId">The thread node id.</param>
		/// <param name="Msg">The MSG.</param>
		/// <returns></returns>
		public static bool ProcessXIbnHeaders(int IncidentId, int ThreadNodeId, Pop3Message Msg)
		{
			bool bRetVal = true;

			ForumStorage.NodeType nodeType = ForumStorage.NodeType.Internal;

			ForumThreadNodeSettingCollection settings = new ForumThreadNodeSettingCollection(ThreadNodeId);

			if (settings.Contains(ForumThreadNodeSetting.Incoming))
				nodeType = ForumStorage.NodeType.Incoming;
			else if (settings.Contains(ForumThreadNodeSetting.Outgoing))
				nodeType = ForumStorage.NodeType.Outgoing;

			bool bSetIncidentState = false;
			ObjectStates cmdObjectStates = ObjectStates.Active;
			int newResponsibleId = 0;

			foreach (string HeaderName in Msg.Headers.AllKeys)
			{
				string UpHeaderName = HeaderName.ToUpper();

				if (UpHeaderName.StartsWith("X-IBN-"))
				{
					switch (UpHeaderName)
					{
						case "X-IBN-STATE":
							if (Msg.Headers[HeaderName].Trim() != "0")
							{
								cmdObjectStates = (ObjectStates)Enum.Parse(typeof(ObjectStates), Msg.Headers[HeaderName]);

								if (cmdObjectStates >= ObjectStates.Upcoming &&
									 cmdObjectStates <= ObjectStates.OnCheck)
								{
									bSetIncidentState = true;
								}
								else
								{
									// OZ: Wrong Object State
									cmdObjectStates = ObjectStates.Active;
								}
							}
							break;
						case "X-IBN-PRIVATEMSG":
							if (Msg.Headers[HeaderName].Trim() != "0")
							{
								XIbnHeaderCommand.SetPrivateStatus(ThreadNodeId);
								nodeType = ForumStorage.NodeType.Internal;
								bRetVal = false;
							}
							break;
						case "X-IBN-RESOLUTION":
							if (Msg.Headers[HeaderName].Trim() != "0")
							{
								XIbnHeaderCommand.SetResolutionStatus(ThreadNodeId);
							}
							break;
						case "X-IBN-WORKAROUND":
							if (Msg.Headers[HeaderName].Trim() != "0")
							{
								XIbnHeaderCommand.SetWorkaroundStatus(ThreadNodeId);
							}
							break;
						case "X-IBN-RESPONSIBLE":
							if (Msg.Headers[HeaderName].Trim() != "0")
							{
								newResponsibleId = int.Parse(Msg.Headers[HeaderName]);
							}
							break;
						default:
							XIbnHeaderCommand.Unknown(IncidentId, Msg.Headers[HeaderName]);
							break;
					}
				}
			}

			// Set Default IBN Object State
			if (!bSetIncidentState)
			{
				if (nodeType == ForumStorage.NodeType.Incoming)
				{
					IncidentBoxDocument document = IncidentBoxDocument.Load(Incident.GetIncidentBox(IncidentId));

					switch (document.EMailRouterBlock.IncomingEMailAction)
					{
						case ExternalEMailActionType.None:
							break;
						case ExternalEMailActionType.SetReOpenState:
							XIbnHeaderCommand.SetIncidentState(nodeType, document, IncidentId, ThreadNodeId, ObjectStates.ReOpen);
							break;
					}
				}
				else if (nodeType == ForumStorage.NodeType.Outgoing)
				{
					IncidentBoxDocument document = IncidentBoxDocument.Load(Incident.GetIncidentBox(IncidentId));

					switch (document.EMailRouterBlock.OutgoingEMailAction)
					{
						case InternalEMailActionType.None:
							break;
						case InternalEMailActionType.SendToCheck:
							XIbnHeaderCommand.SetIncidentState(nodeType, document, IncidentId, ThreadNodeId, ObjectStates.OnCheck);
							break;
						case InternalEMailActionType.SetCompletedState:
							XIbnHeaderCommand.SetIncidentState(nodeType, document, IncidentId, ThreadNodeId, ObjectStates.Completed);
							break;
						case InternalEMailActionType.SetSuspendState:
							XIbnHeaderCommand.SetIncidentState(nodeType, document, IncidentId, ThreadNodeId, ObjectStates.Suspended);
							break;
					}
				}
			}
			else
			{
				IncidentBoxDocument document = IncidentBoxDocument.Load(Incident.GetIncidentBox(IncidentId));
				XIbnHeaderCommand.SetIncidentState(nodeType, document, IncidentId, ThreadNodeId, cmdObjectStates);
			}


			// OZ: 2008-11-24 ChangeResponsible, -1 = NotSet, -2 = Group
			if (newResponsibleId != 0)
			{
				XIbnHeaderCommand.ChangeResponsible(IncidentId, newResponsibleId);
			}


			return bRetVal;
		}

		/// <summary>
		/// Gets the outgoing email format body preview.
		/// </summary>
		/// <param name="IncidentId">The incident id.</param>
		/// <returns></returns>
		public static string GetOutgoingEmailFormatBodyPreview(int issueId)
		{
			IncidentBox incidentBox = IncidentBox.Load(Incident.GetIncidentBox(issueId));

			string issueTitle = Incident.GetIncidentTitle(issueId);
			string issueLink = Alerts2.MakeIssueLink(issueId);
			string issueTicket = TicketUidUtil.Create(incidentBox.IdentifierMask, issueId);

			StringBuilder sb = new StringBuilder(incidentBox.Document.GeneralBlock.OutgoingEmailFormatBody, 4096);

			sb.Replace("[=Title=]", issueTitle);
			sb.Replace("[=Ticket=]", issueTicket);
			//sb.Replace("[=Text=]", node.Text);

			string linkStart = string.Format(CultureInfo.InvariantCulture, "<a href=\"{0}\">", HttpUtility.HtmlAttributeEncode(issueLink));
			sb.Replace("[=Link=]", linkStart);
			sb.Replace("[=/Link=]", "</a>");

			UserLight currentUser = Security.CurrentUser;
			if (currentUser != null)
			{
				sb.Replace("[=FirstName=]", currentUser.FirstName);
				sb.Replace("[=LastName=]", currentUser.LastName);
				sb.Replace("[=Email=]", currentUser.Email);

				using (IDataReader reader = User.GetUserProfile(currentUser.UserID))
				{
					if (reader.Read())
					{
						sb.Replace("[=Phone=]", reader["Phone"].ToString());
						sb.Replace("[=Fax=]", reader["Fax"].ToString());
						sb.Replace("[=Mobile=]", reader["Mobile"].ToString());
						sb.Replace("[=Position=]", reader["Position"].ToString());
						sb.Replace("[=Department=]", reader["Department"].ToString());
						sb.Replace("[=Company=]", reader["Company"].ToString());
						sb.Replace("[=Location=]", reader["Location"].ToString());
					}
				}
			}

			return sb.ToString();
		}

		#region GetPendingMessages
		// [2008-01-10] OR: Pending Incident List Optimization
		/// <summary>
		/// </summary>
		/// <returns>Columns: PendingMessageId, Created, From, To, Subject</returns>
		public static DataTable GetPendingMessages()
		{
			return EMailMessageRow.GetPendingMessages(Security.CurrentUser.TimeZoneId);
		}
		#endregion
	}
}
