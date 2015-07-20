using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.Schedule;
using Mediachase.Ibn;
using Mediachase.IBN;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Database;
using Mediachase.IBN.Database.EMail;
using Mediachase.Net.Mail;
using Mediachase.Ibn.Data;


namespace Mediachase.IBN.Business.EMail
{
	public class EMailRouterScheduleClientInvoker
	{
		public static void Invoke()
		{
			Mediachase.IBN.Business.EMail.EMailRouterScheduleClient client = new Mediachase.IBN.Business.EMail.EMailRouterScheduleClient();
			client.Invoke("");
		}
	}

	/// <summary>
	/// Summary description for EMailRouterScheduleClient.
	/// </summary>
	public class EMailRouterScheduleClient : MarshalByRefObject, IScheduleClient
	{
		static EMailRouterScheduleClient()
		{
			// O.R. [2007-08-22]: Ensure transaction subscription
			DatabaseTransactionBridge.Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EMailRouterScheduleClient"/> class.
		/// </summary>
		public EMailRouterScheduleClient()
		{
		}

		[ThreadStatic]
		private static EMailMessageLogSetting EmailLogSettings;

		#region IScheduleClient Members

		/// <summary>
		/// Invoke2s the specified application name.
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		public void Invoke(string applicationName)
		{
			DbHelper2.Init();

			string[] list = Company.GetApplicationIdsForScheduleService();

			foreach (string appId in list)
			{
				try
				{
					//System.Diagnostics.Trace.WriteLine("Invoke2: " + appId);
					GlobalContext.Current = new GlobalContext(System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, @"..\Apps"));
					//Configuration.Init2(appId);
					DataContext.Current = new DataContext(Mediachase.IBN.Database.DbContext.Current.PortalConnectionString);

					SingleCompanyInvoke();
				}
				catch (Exception e)
				{
					string msg = string.Format("Invoke() failed.\nAPP_ID: {0}\nException: {1}", appId, e.ToString());
					Log.WriteError(msg);
				}
				finally
				{
				}
			}
		}

		public void SingleCompanyInvoke()
		{
			if (!License.HelpDesk)
				return;

			// OZ: [2007-01-29] EMailMessageLog
			EmailLogSettings = EMailMessageLogSetting.Current;
			if (EmailLogSettings.IsActive)
				EMailMessageLog.CleanUp(EmailLogSettings.Period);
			else
				EmailLogSettings = null;
			// 

			// Process External Box
			foreach (EMailRouterPop3Box pop3Box in EMailRouterPop3Box.ListExternal())
			{
				if (pop3Box.Activity.IsActive)
				{
					EMailRouterPop3BoxMessageLoader loader = new EMailRouterPop3BoxMessageLoader(pop3Box);

					loader.Error += new ExceptionAbortEventHandler(OnMessageLoaderError);
					loader.NewMessage += new EventHandler(OnNewMessage);


					loader.ReceiveMessages();
				}
			}

			// Process Internal Box
			EMailRouterPop3Box internalPop3Box = EMailRouterPop3Box.ListInternal();
			if (internalPop3Box != null && internalPop3Box.Activity.IsActive)
			{
				EMailRouterPop3BoxMessageLoader loader = new EMailRouterPop3BoxMessageLoader(internalPop3Box);

				loader.Error += new ExceptionAbortEventHandler(OnMessageLoaderError);
				loader.NewMessage += new EventHandler(OnNewMessage);

				loader.ReceiveMessages();
			}

			//Process send email from SMTP queue
			//try
			//{
			//    SmtpClientUtility.ProcessSendMessages();
			//}
			//catch (Exception)
			//{
			//    // TODO:
			//    //PortalConfig.SmtpSettings.IsChecked = false;
			//    throw;
			//}
		}

		#endregion

		private void OnMessageLoaderError(object sender, ExceptionAbortEventArgs args)
		{
			if (args.Exception is Pop3Exception ||
				args.Exception is System.Net.Sockets.SocketException ||
				args.Exception is System.IO.IOException)
			{
				EMailRouterPop3BoxMessageLoader loader = (EMailRouterPop3BoxMessageLoader)sender;
				EMailRouterPop3Box.UpdateStatistic(loader.Owner.EMailRouterPop3BoxId, false,
					args.Exception.Message, 0);
			}
			else
			{
				Log.WriteError(args.Exception.ToString());
			}
		}

		private int RegisterEmail(int EMailRouterPop3BoxId, Pop3Message msg)
		{
			return EMailMessage.Create(EMailRouterPop3BoxId, msg);
		}

		private int FindIncidentByTicket(string Ticket)
		{
			int IncidentId = TicketUidUtil.GetThreadId(Ticket);

			using (IDataReader reader = DBIncident.GetIncident(IncidentId, 0, 1))
			{
				if (reader.Read())
					return IncidentId;
			}
			return -1;
		}

		private void OnNewMessage(object sender, EventArgs e)
		{
			if (e is NewPop3MessageEventArgs && sender is EMailRouterPop3BoxMessageLoader)
			{
				EMailRouterPop3Box internalPop3Box = EMailRouterPop3Box.ListInternal();
				if (internalPop3Box != null && !internalPop3Box.Activity.IsActive)
					internalPop3Box = null;

				UserLight prevUser = Security.CurrentUser;

				Alerts2.SendToCurrentUser = true;

				try
				{
					using (DbTransaction tran = DbTransaction.Begin())
					{
						EMailRouterPop3BoxMessageLoader loader = (EMailRouterPop3BoxMessageLoader)sender;

						Pop3Message msg = ((NewPop3MessageEventArgs)e).Message;

						string TicketUID = TicketUidUtil.LoadFromString(msg.Subject == null ? string.Empty : msg.Subject);

						EMailMessageAntiSpamRuleRusult result = EMailMessageAntiSpamRuleRusult.Deny;

						if (loader.Owner.IsInternal)
						{
							int UserId = DBUser.GetUserByEmail(EMailMessage.GetSenderEmail(msg), false);
							if (TicketUID != string.Empty && UserId > 0)
								result = EMailMessageAntiSpamRuleRusult.Accept;
						}
						else
						{
							if (PortalConfig.UseAntiSpamFilter)
								result = EMailMessageAntiSpamRule.Check(loader.Owner, msg);
							else
								result = EMailMessageAntiSpamRuleRusult.Accept;
						}

						int EMailMessageId = -1;

						// OZ: [2007-01-29] EMailMessageLog
						if (EmailLogSettings != null)
						{
							string from = string.Empty;

							if (msg.Headers["Reply-To"] != null)
							{
								from = msg.Headers["Reply-To"];
							}
							//else if (msg.Headers["Sender"] != null)
							//{
							//    from = msg.Headers["Sender"];
							//}
							else if (msg.Headers["From"] != null)
							{
								from = msg.Headers["From"];
							}

							EMailMessageLog.Add(from,
								msg.Headers["To"] != null ? msg.Headers["To"] : string.Empty,
								msg.Headers["Subject"] != null ? msg.Headers["Subject"] : string.Empty,
								loader.Owner.EMailRouterPop3BoxId,
								result);
						}
						//

						switch (result)
						{
							case EMailMessageAntiSpamRuleRusult.Pending:
								// Register Email Message
								EMailMessageId = RegisterEmail(loader.Owner.EMailRouterPop3BoxId, msg);
								// Add to pending email table
								EMailMessage.MarkAsPendingMessage(EMailMessageId);
								break;
							case EMailMessageAntiSpamRuleRusult.Accept:
								// Register Email Message
								EMailMessageId = RegisterEmail(loader.Owner.EMailRouterPop3BoxId, msg);


								if (TicketUID == string.Empty)
								{
									int IncidentId = EMailMessage.CreateNewIncident(EMailMessageId, loader.Owner, msg);
								}
								else
								{
									// Assign By Ticket
									int IncidentId = FindIncidentByTicket(TicketUID);

									if (IncidentId != -1)
									{
										int creatorId, issueBoxId;
										using (IDataReader reader = DBIncident.GetIncident(IncidentId, 0, 1))
										{
											reader.Read();

											creatorId = (int)reader["CreatorId"];
											issueBoxId = (int)reader["IncidentBoxId"];
										}

										int stateId, managerId, responsibleId;
										bool isResposibleGroup;
										ArrayList users = new ArrayList();

										Issue2.GetIssueBoxSettings(issueBoxId, out stateId, out managerId, out responsibleId, out isResposibleGroup, users);

										EMailMessage.LogOnCreator(creatorId, msg);

										int ThreadNodeId = EMailMessage.AddToIncidentMessage(loader.Owner.IsInternal, IncidentId, EMailMessageId, msg);

										if (EMailMessage.ProcessXIbnHeaders(IncidentId, ThreadNodeId, msg))
										{
											try
											{
												ArrayList excludeUsers = EMailRouterOutputMessage.Send(IncidentId, loader.Owner, msg);
												SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Forum_MessageAdded, IncidentId, -1, excludeUsers);
											}
											catch (Exception ex)
											{
												System.Diagnostics.Trace.WriteLine(ex);
												Log.WriteError(ex.ToString());
											}
										}
									}
									else
									{
										// Add to pending email table
										if (!loader.Owner.IsInternal)
											EMailMessage.MarkAsPendingMessage(EMailMessageId);
									}
								}
								break;
							case EMailMessageAntiSpamRuleRusult.Deny:
								// Do nothing
								break;
						}

						tran.Commit();
					}
				}
				finally
				{
					Alerts2.SendToCurrentUser = false;

					if (Security.CurrentUser != prevUser)
						Security.SetCurrentUser(prevUser);
				}
			}
		}

	}
}
