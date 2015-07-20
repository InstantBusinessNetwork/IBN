using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Mediachase.Net.Mail
{
	public class MailMessage
	{
		#region Fields

		private MailAddress _from;
		private MailAddressCollection _to = new MailAddressCollection();
		private MailAddressCollection _cc = new MailAddressCollection();
		private MailAddressCollection _bcc = new MailAddressCollection();
		private AttachmentCollection _attachments = new AttachmentCollection();
		private string _subject;
		private string _body;
		private bool _isBodyHtml;
		private string _messageContent;
		private string _errorMessage;
		private DateTime _errorDate;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets from.
		/// </summary>
		/// <value>From.</value>
		public MailAddress From
		{
			get { return _from; }
			set { _from = value; }
		}

		/// <summary>
		/// Gets to.
		/// </summary>
		/// <value>To.</value>
		public MailAddressCollection To
		{
			get { return _to; }
		}

		/// <summary>
		/// Gets the CC.
		/// </summary>
		/// <value>The CC.</value>
		public MailAddressCollection CC
		{
			get { return _cc; }
		}

		/// <summary>
		/// Gets the BCC.
		/// </summary>
		/// <value>The BCC.</value>
		public MailAddressCollection Bcc
		{
			get { return _bcc; }
		}

		/// <summary>
		/// Gets the attachments.
		/// </summary>
		/// <value>The attachments.</value>
		public AttachmentCollection Attachments
		{
			get { return _attachments; }
		}

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>The subject.</value>
		public string Subject
		{
			get { return _subject; }
			set { _subject = value; }
		}

		/// <summary>
		/// Gets or sets the body.
		/// </summary>
		/// <value>The body.</value>
		public string Body
		{
			get { return _body; }
			set { _body = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is body HTML.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is body HTML; otherwise, <c>false</c>.
		/// </value>
		public bool IsBodyHtml
		{
			get { return _isBodyHtml; }
			set { _isBodyHtml = value; }
		}

		/// <summary>
		/// Gets or sets the content of the message.
		/// </summary>
		/// <value>The content of the message.</value>
		public string MessageContent
		{
			get { return _messageContent; }
			set { _messageContent = value; }
		}

		/// <summary>
		/// Gets or sets the error message.
		/// </summary>
		/// <value>The error message.</value>
		public string ErrorMessage
		{
			get { return _errorMessage; }
			set { _errorMessage = value; }
		}

		/// <summary>
		/// Gets or sets the error date.
		/// </summary>
		/// <value>The error date.</value>
		public DateTime ErrorDate
		{
			get { return _errorDate; }
			set { _errorDate = value; }
		}

		/// <summary>
		/// Gets or sets the sent date.
		/// </summary>
		/// <value>The sent date.</value>
		public DateTime SentDate
		{
			get;
			set;
		}

		#endregion

		/// <summary>
		/// Creates the content of the message.
		/// </summary>
		public void CreateMessageContent()
		{
			string boundary = "---=_Next_Part_" + Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);

			StringBuilder message = new StringBuilder();

			// Add headers
			WriteLine(message, "Date: ", DateTime.UtcNow.ToString("r", CultureInfo.InvariantCulture));
			WriteLine(message, "From: ", CreateAddressHeaderValue(_from));
			if (_to.Count > 0)
				WriteLine(message, "To: ", CreateAddressListHeaderValue(_to));
			if (_cc.Count > 0)
				WriteLine(message, "Cc: ", CreateAddressListHeaderValue(_cc));
			WriteLine(message, "Subject: ", EncodeHeaderValue(_subject));
			WriteLine(message, "MIME-Version: 1.0");
			WriteLine(message, "Content-Type: multipart/mixed; boundary=\"", boundary, "\"");
			WriteLine(message);

			// Add body
			WriteLine(message, "--", boundary);
			WriteLine(message, "Content-Type: text/", _isBodyHtml ? "html" : "plain", "; charset=utf-8");
			WriteLine(message, "Content-Transfer-Encoding: base64");
			WriteLine(message);
			WriteLine(message, Convert.ToBase64String(Encoding.UTF8.GetBytes(_body), Base64FormattingOptions.InsertLineBreaks));

			// Add attachments
			foreach (Attachment attachment in _attachments)
			{
				string name = EncodeHeaderValue(attachment.Name);

				WriteLine(message, "--", boundary);
				WriteLine(message, "Content-Type: ", attachment.ContentType, "; name=\"", name, "\"");
				WriteLine(message, "Content-Transfer-Encoding: base64");
				WriteLine(message, "Content-Disposition: attachment; filename=\"", name, "\"");
				WriteLine(message);
				WriteLine(message, Convert.ToBase64String(attachment.GetData(), Base64FormattingOptions.InsertLineBreaks));
				WriteLine(message);
			}

			// Add final lines
			WriteLine(message, "--", boundary, "--");
			WriteLine(message);

			_messageContent = message.ToString();
		}

		/// <summary>
		/// Encodes the header value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static string EncodeHeaderValue(string value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			StringBuilder builder = new StringBuilder(value.Length * 2);

			int nonAsciiCharacterIndex = -1;

			for (int index = 0; index < value.Length; index++)
			{
				char character = value[index];

				if (character > '\x007f')
				{
					if (nonAsciiCharacterIndex == -1)
						nonAsciiCharacterIndex = index;
				}
				else
				{
					if (nonAsciiCharacterIndex != -1)
					{
						if (character == '\x0020')
							continue;

						AppendEncodedSubstring(builder, value, nonAsciiCharacterIndex, index);
						nonAsciiCharacterIndex = -1;
					}

					builder.Append(character);
				}
			}

			if (nonAsciiCharacterIndex != -1)
			{
				AppendEncodedSubstring(builder, value, nonAsciiCharacterIndex, value.Length);
			}

			return builder.ToString();
		}


		/// <summary>
		/// Appends the encoded substring.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="value">The value.</param>
		/// <param name="firstCharacterIndex">First index of the character.</param>
		/// <param name="currentCharacterIndex">Index of the current character.</param>
		private static void AppendEncodedSubstring(StringBuilder builder, string value, int firstCharacterIndex, int currentCharacterIndex)
		{
			string substring = value.Substring(firstCharacterIndex, currentCharacterIndex - firstCharacterIndex);
			byte[] bytes = Encoding.UTF8.GetBytes(substring);
			string strBase64 = Convert.ToBase64String(bytes);
			builder.Append("=?utf-8?B?");
			builder.Append(strBase64);
			builder.Append("?=");
		}

		/// <summary>
		/// Writes the line.
		/// </summary>
		/// <param name="stringBuilder">The string builder.</param>
		/// <param name="parts">The parts.</param>
		private static void WriteLine(StringBuilder stringBuilder, params string[] parts)
		{
			foreach (string part in parts)
			{
				stringBuilder.Append(part);
			}
			stringBuilder.Append("\r\n");
		}

		/// <summary>
		/// Creates the address list header value.
		/// </summary>
		/// <param name="addresses">The addresses.</param>
		/// <returns></returns>
		private static string CreateAddressListHeaderValue(MailAddressCollection addresses)
		{
			StringBuilder sb = new StringBuilder();

			foreach (MailAddress address in addresses)
			{
				if (sb.Length > 0)
				{
					sb.Append(",\r\n ");
				}
				sb.Append(CreateAddressHeaderValue(address));
			}

			return sb.ToString();
		}

		/// <summary>
		/// Creates the address header value.
		/// </summary>
		/// <param name="address">The address.</param>
		/// <returns></returns>
		private static string CreateAddressHeaderValue(MailAddress address)
		{
			StringBuilder builder = new StringBuilder();

			if (!string.IsNullOrEmpty(address.DisplayName))
			{
				builder.Append("\"");
				builder.Append(EncodeHeaderValue(address.DisplayName.Replace("\"", "\\\"")));
				builder.Append("\" ");
			}

			builder.Append("<");
			builder.Append(address.Address);
			builder.Append(">");

			return builder.ToString();
		}
	}
}
