using System;
using Mediachase.Net.Mail;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for NewPop3MessageEventArgs.
	/// </summary>
	public class NewPop3MessageEventArgs: EventArgs
	{
		Pop3Message _message;

		/// <summary>
		/// Initializes a new instance of the <see cref="NewPop3MessageEventArgs"/> class.
		/// </summary>
		/// <param name="Message">The message.</param>
		public NewPop3MessageEventArgs(Pop3Message Message)
		{
			_message = Message;
		}

		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <value>The message.</value>
		public Pop3Message Message
		{
			get 
			{
				return _message;
			}
		}

	}
}
