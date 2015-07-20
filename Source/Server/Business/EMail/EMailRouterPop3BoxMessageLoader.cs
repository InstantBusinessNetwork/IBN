using System;
using Mediachase.Net.Mail;
using System.Net;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for EMailRouterPop3BoxMessageLoader.
	/// </summary>
	public class EMailRouterPop3BoxMessageLoader
	{
		private EMailRouterPop3Box _pop3Box = null;

		public EMailRouterPop3BoxMessageLoader(EMailRouterPop3Box pop3Box)
		{
			_pop3Box = pop3Box;
		}

		public EMailRouterPop3Box Owner
		{
			get 
			{
				return _pop3Box;
			}
		}

		public event ExceptionAbortEventHandler Error;
		public event EventHandler				NewMessage;

		#region Error Event
		private bool RaiseErrorEvent(Exception ex)
		{
			ExceptionAbortEventArgs args = new ExceptionAbortEventArgs(ex);
			RaiseErrorEvent(args);
			return args.Abort;
		}

		/// <summary>
		/// Raises the error event.
		/// </summary>
		/// <param name="args">The <see cref="T:Mediachase.EMailRouter.ExceptionAbortEventArgs"/> instance containing the event data.</param>
		private void RaiseErrorEvent(ExceptionAbortEventArgs args)
		{
			OnErrorEvent(args);
		}

		/// <summary>
		/// Raises the error event event.
		/// </summary>
		/// <param name="args">The <see cref="T:Mediachase.EMailRouter.ExceptionAbortEventArgs"/> instance containing the event data.</param>
		protected virtual void OnErrorEvent(ExceptionAbortEventArgs args)
		{
			if (this.Error != null)
			{
				this.Error(this, args);
			}
		}
		#endregion

		#region NewMessage Event
		/// <summary>
		/// Raises the error event.
		/// </summary>
		/// <param name="args">The <see cref="T:Mediachase.EMailRouter.ExceptionAbortEventArgs"/> instance containing the event data.</param>
		private void RaiseNewMessageEvent(Pop3Message message)
		{
			OnNewMessageEvent(new NewPop3MessageEventArgs(message));
		}

		/// <summary>
		/// Raises the error event event.
		/// </summary>
		/// <param name="args">The <see cref="T:Mediachase.EMailRouter.ExceptionAbortEventArgs"/> instance containing the event data.</param>
		protected virtual void OnNewMessageEvent(NewPop3MessageEventArgs args)
		{
			if (this.NewMessage != null)
			{
				this.NewMessage(this, args);
			}
		}
		#endregion

		public void ReceiveMessages()
		{
			try
			{
				Pop3Connection connection = new Pop3Connection();

				//IPHostEntry hostInfo = Dns.GetHostEntry(Owner.Server);
				IPAddress[] addresses = Dns.GetHostAddresses(Owner.Server);

				IPEndPoint pop3ServerEndPoint = new IPEndPoint(addresses[0], Owner.Port);

                if (this.Owner.SecureConnectionType == Pop3SecureConnectionType.Ssl)
                {
                    connection.OpenSsl(pop3ServerEndPoint, Owner.Server);
                }
                else
                {
                    connection.Open(pop3ServerEndPoint);
                }

                if (this.Owner.SecureConnectionType == Pop3SecureConnectionType.Tls)
                {
                    connection.Stls(Owner.Server);
                }

				connection.User(Owner.Login);
				connection.Pass(Owner.Pass);

				Pop3Stat stat = connection.Stat();

				EMailRouterPop3Box.UpdateStatistic(this.Owner.EMailRouterPop3BoxId,
					true, 
					string.Empty, stat.MessageCout);


				if(stat.MessageCout > 0)
				{
					// Step 3. Request uidl.
					Pop3UIDInfoList uidList = connection.Uidl();

					foreach (Pop3UIDInfo uidItem in uidList)
					{
						try
						{
							// Step 4. Request message.
							Pop3Message msg = connection.Retr(uidItem.ID);

							// Step 5. Process message.
							RaiseNewMessageEvent(msg);

							// Step 6. Delete message from server.
							connection.Dele(uidItem.ID);
						}
						catch (Exception  ex)
						{
							if(RaiseErrorEvent(ex))
								throw;
						}
					}
				}

				connection.Quit();
			}
			catch(Exception ex)
			{
				if(RaiseErrorEvent(ex))
					throw;
			}
		}
	}
}
