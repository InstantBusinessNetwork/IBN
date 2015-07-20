using System;
using System.Web;
using System.Collections;
using System.Collections.Specialized;
using Mediachase.Net.Mail;
using System.Text;

namespace Mediachase.IBN.Business.EMail
{
	internal class OutputMessage
	{
		public string MailFrom;
		public string RcptTo;
		public string Subject;
		public byte[]	Data;
	}

	/// <summary>
	/// Summary description for OutputMessageCreator.
	/// </summary>
	internal class OutputMessageCreator
	{
		private Pop3Message _sourceMsg = null;
		private ArrayList _recipientList = new ArrayList();
		private string _subject = null;
		private string _from = null;
		private string _mailFrom = null;
		private int _incidentId;

		private NameValueCollection	_additionalHeaders = new NameValueCollection();

		private Hashtable _createdToEmailList = new Hashtable();

		private static string GetAbsolutePath(string xs_Path)
		{
			return (Configuration.PortalLink + xs_Path);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OutputMessageCreator"/> class.
		/// </summary>
		/// <param name="Msg">The MSG.</param>
		/// <param name="IncidentId">The incident id.</param>
		/// <param name="From">From.</param>
		public OutputMessageCreator(Pop3Message Msg, int IncidentId, string MailFrom, string From)
		{
			_sourceMsg = Msg;
			_from = From;
			_mailFrom = MailFrom;
			_incidentId = IncidentId;
		}

		/// <summary>
		/// Adds the recipient.
		/// </summary>
		/// <param name="Email">The email.</param>
		public void AddRecipient(string Email)
		{
			_recipientList.Add(Email);
		}

		/// <summary>
		/// Adds the recipient.
		/// </summary>
		/// <param name="IbnUserId">The ibn user id.</param>
		public void AddRecipient(int IbnUserId)
		{
			_recipientList.Add(IbnUserId);
		}

		/// <summary>
		/// Gets the additional headers.
		/// </summary>
		/// <value>The additional headers.</value>
		public NameValueCollection AdditionalHeaders
		{
			get 
			{
				return _additionalHeaders;
			}
		}

		/// <summary>
		/// Gets or sets from.
		/// </summary>
		/// <value>From.</value>
		public string From
		{
			get 
			{
				return _from;
			}
			set
			{
				_from = value;
			}
		}

		/// <summary>
		/// Gets or sets the mail from.
		/// </summary>
		/// <value>The mail from.</value>
		public string MailFrom
		{
			get 
			{
				return _mailFrom;
			}
			set
			{
				_mailFrom = value;
			}
		}
		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>The subject.</value>
		public string Subject
		{
			get 
			{
				return _subject;
			}
			set
			{
				_subject = value;
			}
		}

		public void AddIgnoreRecipient(string RecipientEmail)
		{
			_createdToEmailList.Add(RecipientEmail, null);
		}

		/// <summary>
		/// Creates this instance.
		/// </summary>
		/// <returns></returns>
		public ArrayList	Create()
		{
			ArrayList retVal = new ArrayList();
			//_createdToEmailList.Clear();

			foreach(object Recipient in _recipientList)
			{
				if(Recipient is string)
				{
					if(!_createdToEmailList.ContainsKey(Recipient))
					{
						_createdToEmailList.Add(Recipient,null);

						retVal.Add(CreateSingle((string)Recipient));
					}
				}
				else if (Recipient is int)
				{
					UserLight user = UserLight.Load((int)Recipient);

					if(user.Email!=null && user.Email!=string.Empty && 
						!_createdToEmailList.ContainsKey(user.Email))
					{
						_createdToEmailList.Add(user.Email,null);

						NameValueCollection headers = new NameValueCollection();

                        if (_incidentId > 0)
                        {
                            Guid UserTicket = IncidentUserTicket.CreateAndReturnUID(user.UserID, _incidentId);

                            headers.Add("X-IBN-USERTICKET", UserTicket.ToString());
                            headers.Add("X-IBN-WEBSERVER", GetAbsolutePath("/WebServices/IncidentInfo.asmx"));
                        }

						retVal.Add(CreateSingle(user.Email, headers));
					}
				}
			}

			return retVal;
		}

		/// <summary>
		/// Determines whether the specified value is ASCII.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="permitCROrLF">if set to <c>true</c> [permit CR or LF].</param>
		/// <returns>
		/// 	<c>true</c> if the specified value is ASCII; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsAscii(string value, bool permitCROrLF)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			foreach (char ch1 in value)
			{
				if (ch1 > '\x007f')
				{
					return false;
				}
				if (!permitCROrLF && ((ch1 == '\r') || (ch1 == '\n')))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Creates the single.
		/// </summary>
		/// <param name="ToEMail">To E mail.</param>
		/// <returns></returns>
		protected OutputMessage CreateSingle(string ToEMail)
		{
			return CreateSingle(ToEMail, new NameValueCollection());
		}

		/// <summary>
		/// Creates the single.
		/// </summary>
		/// <param name="EMail">The E mail.</param>
		/// <returns></returns>
		protected OutputMessage CreateSingle(string ToEMail, NameValueCollection headers)
		{
			Pop3MessageModifier md = new Pop3MessageModifier(EMailMessage.GetPop3MessageBytes(_sourceMsg));

			// Step 1. Change Headers
			md.Headers["From"] = this.From;
			md.Headers["To"] = ToEMail;

			// Step 2. Remove Headers
			md.Headers.Remove("cc");
			md.Headers.Remove("bcc");
			md.Headers.Remove("Sender");
			md.Headers.Remove("Reply-To");
			md.Headers.Remove("Reply-To");

			// OZ 2009-01-22 Exchange 2007 Problem
			md.Headers.Remove("Thread-Topic");
			md.Headers.Remove("Thread-Index");

			// OZ 2009-02-05 Gmail Problem
			md.Headers.Remove("Message-ID");

			if(this.Subject!=null)
			{
				md.Headers["Subject"] = this.Subject;
			}

			// Remove all incoming command
			foreach(string Key in md.Headers.AllKeys)
			{
				string UKey = Key.ToUpper();
				if(UKey.StartsWith("X-IBN-"))
					md.Headers.Remove(Key);
			}

			foreach(string Key in headers.AllKeys)
			{
				md.Headers.Remove(Key);
				md.Headers.Add(Key, headers[Key]);
			}

			foreach(string Key in this.AdditionalHeaders.AllKeys)
			{
				md.Headers.Remove(Key);
				md.Headers.Add(Key, this.AdditionalHeaders[Key]);
			}

			OutputMessage retVal = new OutputMessage();

			retVal.Data = md.GetBuffer();
			retVal.MailFrom = this.MailFrom;
			retVal.RcptTo = ToEMail;
			retVal.Subject = md.Headers["Subject"];

			return retVal;
		}
	}
}
