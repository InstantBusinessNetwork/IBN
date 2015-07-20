using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;

namespace Mediachase.Net.Mail
{
	/// <summary>
	/// Allows applications to send e-mail by using the Simple Mail Transfer Protocol (SMTP). 
	/// </summary>
	/// <remarks>
	/// http://tools.ietf.org/html/rfc2821
	/// </remarks>
	public class SmtpClient
	{
		#region Fields

		private string _host = "localhost";
		private int _port = 25;
		private int _receiveTimeout = 30000; // 30 seconds
		private SecureConnectionType _secureConnectionType = SecureConnectionType.None;
		private bool _authenticate;
		private string _user;
		private string _password;

		#endregion

		#region Properties

		public string Host
		{
			get { return _host; }
			set { _host = value; }
		}

		public int Port
		{
			get { return _port; }
			set { _port = value; }
		}

		public int ReceiveTimeout
		{
			get { return _receiveTimeout; }
			set { _receiveTimeout = value; }
		}

		public SecureConnectionType SecureConnectionType
		{
			get { return _secureConnectionType; }
			set { _secureConnectionType = value; }
		}

		public bool Authenticate
		{
			get { return _authenticate; }
			set { _authenticate = value; }
		}

		public string User
		{
			get { return _user; }
			set { _user = value; }
		}

		public string Password
		{
			get { return _password; }
			set { _password = value; }
		}

		#endregion

		#region .ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="SmtpClient"/> class.
		/// </summary>
		public SmtpClient()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SmtpClient"/> class.
		/// </summary>
		/// <param name="host">The host.</param>
		public SmtpClient(string host)
		{
			_host = host;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SmtpClient"/> class.
		/// </summary>
		/// <param name="host">The host.</param>
		/// <param name="port">The port.</param>
		public SmtpClient(string host, int port)
		{
			_host = host;
			_port = port;
		}

		#endregion

		/// <summary>
		/// Sends the specified messages.
		/// </summary>
		/// <param name="messages">The messages.</param>
		public void Send(params MailMessage[] messages)
		{
			// Open socket
			using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
			{
				#region Init Socket
				IPAddress address = GetHostAddressIPv4(_host);

				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _receiveTimeout);
				socket.Connect(new IPEndPoint(address, _port));
				#endregion

				using (NetworkStream networkStream = new NetworkStream(socket))
				{
					Stream smtpStream = networkStream;

					if (_secureConnectionType == SecureConnectionType.Ssl)
					{
						SslStream sslStream = new SslStream(networkStream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
						sslStream.AuthenticateAsClient(_host);
						smtpStream = sslStream;
					}

					#region Welcome
					// Read welcome message
					ReadAndValidateResponse(smtpStream, 220);
					#endregion

					#region EHLO
					// Send EHLO command
					IPAddress currentIPv4 = GetCurrentAddressIPv4();

					string command = "EHLO";
					if (currentIPv4!=null)
						command += string.Format(CultureInfo.InvariantCulture, " [{0}]", currentIPv4);

					WriteCommand(smtpStream, command);
					ReadAndValidateResponse(smtpStream, 250);
					#endregion

					if (_secureConnectionType == SecureConnectionType.Tls)
					{
						#region STARTTLS
						// Send STARTTLS command
						WriteCommand(smtpStream, "STARTTLS");
						ReadAndValidateResponse(smtpStream, 220);

						SslStream tlsStream = new SslStream(smtpStream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
						tlsStream.AuthenticateAsClient(_host);

						smtpStream = tlsStream;
						#endregion
					}

					if (_authenticate)
					{
						#region AUTH LOGIN
						WriteCommand(smtpStream, "AUTH LOGIN");
						ReadAndValidateResponse(smtpStream, 334);

						// Send user name
						WriteCommand(smtpStream, Convert.ToBase64String(Encoding.ASCII.GetBytes(_user)));
						ReadAndValidateResponse(smtpStream, 334);

						// Send password
						WriteCommand(smtpStream, Convert.ToBase64String(Encoding.ASCII.GetBytes(_password)));
						ReadAndValidateResponse(smtpStream, 235);
						#endregion
					}

					foreach (MailMessage message in messages)
					{
						try
						{
							// Create Message Content
							if (string.IsNullOrEmpty(message.MessageContent))
							{
								message.CreateMessageContent();
							}

							#region MAIL FROM
							// Send MAIL FROM command
							WriteCommand(smtpStream, "MAIL FROM:<", message.From.Address, ">");
							ReadAndValidateResponse(smtpStream, 250);
							#endregion

							#region RCPT TO
							MailAddressCollection recipients = new MailAddressCollection();
							foreach (MailAddress mailAddress in message.To)
								recipients.Add(mailAddress);
							foreach (MailAddress mailAddress in message.CC)
								recipients.Add(mailAddress);
							foreach (MailAddress mailAddress in message.Bcc)
								recipients.Add(mailAddress);

							// Send RCPT TO command
							foreach (MailAddress mailAddress in recipients)
							{
								WriteCommand(smtpStream, "RCPT TO:<", mailAddress.Address, ">");
								ReadAndValidateResponse(smtpStream, 250, 251);
							}
							#endregion

							#region DATA
							// Send DATA command
							WriteCommand(smtpStream, "DATA");
							ReadAndValidateResponse(smtpStream, 354);

							// Send message
							WriteCommand(smtpStream, message.MessageContent);
							WriteCommand(smtpStream, ".");
							ReadAndValidateResponse(smtpStream, 250);
							#endregion

							message.SentDate = DateTime.UtcNow;
						}
						catch (SmtpException ex)
						{
							message.ErrorDate = DateTime.UtcNow;
							message.ErrorMessage = ex.Message;

							#region RSET
							// Send RSET command
							try
							{
								WriteCommand(smtpStream, "RSET");
								ReadAndValidateResponse(smtpStream, 250);
							}
							catch (Exception inex)
							{
								Trace.WriteLine(inex, "SmtpClient");
							}
							#endregion
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the current address IPv4.
		/// </summary>
		/// <returns></returns>
		private IPAddress GetCurrentAddressIPv4()
		{
			IPAddress[] items = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

			foreach (IPAddress item in items)
			{
				if (item.AddressFamily == AddressFamily.InterNetwork)
					return item;
			}

			return null;
		}

		/// <summary>
		/// Gets the host address IPv4.
		/// </summary>
		/// <param name="host">The host.</param>
		/// <returns></returns>
		private IPAddress GetHostAddressIPv4(string host)
		{
			IPAddress[] items = Dns.GetHostAddresses(host);

			foreach (IPAddress item in items)
			{
				if (item.AddressFamily == AddressFamily.InterNetwork)
					return item;
			}

			return null;
		}



		/// <summary>
		/// Validates the server certificate.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="certificate">The certificate.</param>
		/// <param name="chain">The chain.</param>
		/// <param name="sslPolicyErrors">The SSL policy errors.</param>
		/// <returns></returns>
		private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors != SslPolicyErrors.None)
			{
				Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Certificate error: {0}", sslPolicyErrors));
				// Do not allow this client to communicate with unauthenticated servers.
				// return false;
			}

			return true;
		}

		/// <summary>
		/// Reads the and validate response.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="validResponseCodes">The valid response codes.</param>
		private static void ReadAndValidateResponse(Stream stream, params int[] validResponseCodes)
		{
			string response = StreamReadString(stream);
			Trace.WriteLine(response);
			int responseCode = GetResponseCode(response);

			bool valid = false;

			foreach (int validResponseCode in validResponseCodes)
			{
				if (responseCode == validResponseCode)
				{
					valid = true;
					break;
				}
			}

			if (!valid)
				throw new SmtpException(response);
		}

		/// <summary>
		/// Streams the read string.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns></returns>
		private static string StreamReadString(Stream stream)
		{
			StringBuilder sb = new StringBuilder(1024);

			string newLine = string.Empty;
			do
			{
				newLine = StreamReadLine(stream);
				sb.Append(newLine);
			}
			while (newLine.Length > 4 && newLine[3] == '-'); // Multiline response

			return sb.ToString();
		}

		/// <summary>
		/// Streams the read line.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns></returns>
		private static string StreamReadLine(Stream stream)
		{
			StringBuilder sb = new StringBuilder(256);

			string newText = string.Empty;
			int realReceived = -1;

			byte[] Buffer = new byte[1];

			do
			{
				realReceived = StreamRead(stream, Buffer);
				if (realReceived != 1)
					break;

				newText = Encoding.ASCII.GetString(Buffer, 0, realReceived);
				sb.Append(newText);
			}
			while (newText != "\n");

			return sb.ToString();
		}

		/// <summary>
		/// Streams the read.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="buffer">The buffer.</param>
		/// <returns></returns>
		private static int StreamRead(Stream stream, byte[] buffer)
		{
			return stream.Read(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Gets the response code.
		/// </summary>
		/// <param name="response">The response.</param>
		/// <returns></returns>
		private static int GetResponseCode(string response)
		{
			if (string.IsNullOrEmpty(response))
				return -1;

			try
			{
				return int.Parse(response.Substring(0, 3), CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex, "SmtpClient");
				return -1;
			}
		}

		/// <summary>
		/// Writes the command.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="parts">The parts.</param>
		private static void WriteCommand(Stream stream, params string[] parts)
		{
			StringBuilder builder = new StringBuilder();
			foreach (string part in parts)
				builder.Append(part);
			builder.Append("\r\n");
			WriteBytes(stream, Encoding.Default.GetBytes(builder.ToString()), 1024 * 64);
		}

		/// <summary>
		/// Writes the bytes.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="data">The data.</param>
		/// <param name="maxChunkLength">Length of the max chunk.</param>
		private static void WriteBytes(Stream stream, byte[] data, int maxChunkLength)
		{
			int offset = 0;
			int notSentLength = data.Length;

			while (notSentLength > 0)
			{
				int count = Math.Min(notSentLength, maxChunkLength);

				stream.Write(data, offset, count);

				offset += count;
				notSentLength -= count;
			}
		}
	}
}
