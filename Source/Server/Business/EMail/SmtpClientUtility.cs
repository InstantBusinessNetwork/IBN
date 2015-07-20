using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Mediachase.IBN.Database;
using Mediachase.IBN.Database.ControlSystem;
using Mediachase.IBN.Database.EMail;
using Mediachase.Net.Mail;
using Mediachase.Ibn.Business.Messages;
using Mediachase.Ibn.Core.Business;


namespace Mediachase.IBN.Business.EMail
{
	public enum SmtpSettingsResult
	{
		None = 0,
		ServerName = 1,
		Connection = 2,
		Authentication = 4,
		AllOk = ServerName | Connection | Authentication
	}

	internal static class SmtpClientUtility
	{
		#region Utility
		/// <summary>
		/// Gets the SMTP reply command.
		/// </summary>
		/// <param name="Response">The response.</param>
		/// <returns></returns>
		private static int GetSmtpReplyCommand(string Response)
		{
			return int.Parse(Response.Substring(0, 3));
		}

		/// <summary>
		/// Streams the read.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="Buffer">The buffer.</param>
		/// <returns></returns>
		private static int StreamRead(Stream stream, byte[] Buffer)
		{
			return stream.Read(Buffer, 0, Buffer.Length);
		}

		/// <summary>
		/// Streams the read line.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="Buffer">The buffer.</param>
		/// <returns></returns>
		private static string StreamReadLine(Stream stream)
		{
			StringBuilder retVal = new StringBuilder(256);

			string newText = string.Empty;
			int realReceived = -1;

			byte[] Buffer = new byte[1];

			do
			{
				realReceived = StreamRead(stream, Buffer);
				if (realReceived != 1)
					break;

				newText = Encoding.Default.GetString(Buffer, 0, realReceived);
				retVal.Append(newText);
			}
			while (newText != "\n");

			return retVal.ToString();
		}

		/// <summary>
		/// Streams the read string.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="Buffer">The buffer.</param>
		/// <returns></returns>
		private static string StreamReadString(Stream stream)
		{
			StringBuilder retVal = new StringBuilder(1024);

			string newLine = string.Empty;
			do
			{
				newLine = StreamReadLine(stream);
				retVal.Append(newLine);
			}
			while (newLine.Length > 4 && newLine[3] == '-'); // Multiline response

			return retVal.ToString();
		}


		/// <summary>
		/// Streams the write.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="Format">The format.</param>
		/// <param name="args">The args.</param>
		private static void StreamWrite(Stream stream, string Format, params object[] args)
		{
			StreamWrite(stream, string.Format(Format, args));
		}

		/// <summary>
		/// Streams the write.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="Data">The data.</param>
		private static void StreamWrite(Stream stream, string Data)
		{
			StreamWrite(stream, System.Text.Encoding.Default.GetBytes(Data));
		}

		/// <summary>
		/// Streams the write.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="buffer">The buffer.</param>
		private static void StreamWrite(Stream stream, byte[] buffer)
		{
			//stream.Write(byffer, 0, byffer.Length);
			StreamWrite(stream, buffer, 1024 * 64);
		}

		/// <summary>
		/// Streams the write.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="buffer">The buffer.</param>
		/// <param name="maxChunkLength">Length of the max chunk.</param>
		private static void StreamWrite(Stream stream, byte[] buffer, int maxChunkLength)
		{
			int offset = 0;
			int notSentLength = buffer.Length;

			while (notSentLength > 0)
			{
				int count = System.Math.Min(notSentLength, maxChunkLength);

				stream.Write(buffer, offset, count);

				offset += count;
				notSentLength -= count;
			}
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
			if (sslPolicyErrors == SslPolicyErrors.None)
				return true;

			System.Diagnostics.Trace.WriteLine(string.Format("Certificate error: {0}", sslPolicyErrors));

			// Do not allow this client to communicate with unauthenticated servers.
			// return true;
			return true;
		}
		#endregion

		#region CheckSettigs
		/// <summary>
		/// Gets the host address I PV4.
		/// </summary>
		/// <param name="host">The host.</param>
		/// <returns></returns>
		private static IPAddress GetHostAddressIPv4(string host)
		{
			IPAddress[] items = Dns.GetHostAddresses(host);

			foreach (IPAddress item in items)
			{
				if (item.AddressFamily == AddressFamily.InterNetwork)
					return item;
			}

			if (items.Length > 0)
				return items[0];

			return null;
		}

		[Obsolete]
		internal static SmtpSettingsResult CheckSettings(string SmtpServerHost, int Port,
			SecureConnectionType SecureConnection,
			bool Authenticate,
			string User, string Password)
		{
			SmtpSettingsResult result = SmtpSettingsResult.None;

			if (SmtpServerHost == string.Empty)
				SmtpServerHost = "localhost";

			try
			{
				// Progressive Method
				//byte[] inBuffer = new byte[10 * 1024];
				//int realReceived = -1;
				string ResponseString = string.Empty;

				// Step 1. Open Socket Connection
				using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
				{
					#region Init Socket
					//IPHostEntry entry = Dns.GetHostEntry(SmtpServerHost);
					IPAddress address = GetHostAddressIPv4(SmtpServerHost);

					result |= SmtpSettingsResult.ServerName;

					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 30000);
					socket.Connect(new IPEndPoint(address, Port));
					#endregion

					Stream smptStream = new NetworkStream(socket);

					if (SecureConnection == SecureConnectionType.Ssl)
					{
						SslStream sslStream = new SslStream(smptStream, false,
							new RemoteCertificateValidationCallback(ValidateServerCertificate), null);

						sslStream.AuthenticateAsClient(SmtpServerHost);

						smptStream = sslStream;
					}

					#region Welcome
					// Step 2. Read Welcome
					//realReceivet = socket.Receive(inByffer);
					//realReceived = StreamRead(smptStream,inByffer);

					//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
					ResponseString = StreamReadString(smptStream);

					System.Diagnostics.Trace.WriteLine(ResponseString);
					switch (GetSmtpReplyCommand(ResponseString))
					{
						case 220:
							// Success
							break;
						default:
							throw new SmtpClientException(ResponseString);
					}
					#endregion

					#region HELO
					// Step 3. Send EHLO command
					IPAddress[] ipList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

					if (ipList.Length > 0)
					{
						//socket.Send(System.Text.Encoding.Default.GetBytes(string.Format("EHLO [{0}]\r\n", ipList[0].ToString())));
						StreamWrite(smptStream, "EHLO [{0}]\r\n", ipList[0].ToString());
					}
					else
					{
						//socket.Send(System.Text.Encoding.Default.GetBytes("EHLO\r\n"));
						StreamWrite(smptStream, "EHLO\r\n");
					}

					// Step 4. Read Response
					//realReceivet = socket.Receive(inByffer);
					//realReceived = StreamRead(smptStream, inByffer);

					//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
					ResponseString = StreamReadString(smptStream);

					System.Diagnostics.Trace.WriteLine(ResponseString);
					switch (GetSmtpReplyCommand(ResponseString))
					{
						case 250:
							// Success
							break;
						default:
							throw new SmtpClientException(ResponseString);
					}
					#endregion

					if (SecureConnection == SecureConnectionType.Tls)
					{
						#region STARTTLS
						// Step. Send STARTTLS command
						StreamWrite(smptStream, "STARTTLS\r\n");
						//realReceived = StreamRead(smptStream, inByffer);

						// Step. Read Response
						//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
						ResponseString = StreamReadString(smptStream);
						System.Diagnostics.Trace.WriteLine(ResponseString);

						switch (GetSmtpReplyCommand(ResponseString))
						{
							case 220:
								// Success
								break;
							default:
								throw new SmtpClientException(ResponseString);
						}

						SslStream tlsSmtpStream = new SslStream(smptStream, false,
								new RemoteCertificateValidationCallback(ValidateServerCertificate),
								null);

						tlsSmtpStream.AuthenticateAsClient(SmtpServerHost);

						smptStream = tlsSmtpStream;

						#endregion
					}

					result |= SmtpSettingsResult.Connection;

					if (Authenticate)
					{
						#region AUTH LOGIN
						//socket.Send(System.Text.Encoding.Default.GetBytes("AUTH LOGIN\r\n"));
						StreamWrite(smptStream, "AUTH LOGIN\r\n");

						// Step 4. Read Response
						//realReceivet = socket.Receive(inByffer);
						//realReceived = StreamRead(smptStream, inByffer);

						//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
						ResponseString = StreamReadString(smptStream);
						System.Diagnostics.Trace.WriteLine(ResponseString);
						switch (GetSmtpReplyCommand(ResponseString))
						{
							case 334:
								// Success
								break;
							default:
								throw new SmtpClientException(ResponseString);
						}

						// Send User Name
						string UserNameB64 = Convert.ToBase64String(Encoding.Default.GetBytes(User)) + "\r\n";

						//socket.Send(System.Text.Encoding.Default.GetBytes(UserNameB64));
						StreamWrite(smptStream, UserNameB64);

						// Step 4. Read Response
						//realReceivet = socket.Receive(inByffer);
						//realReceived = StreamRead(smptStream, inByffer);

						//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
						ResponseString = StreamReadString(smptStream);
						System.Diagnostics.Trace.WriteLine(ResponseString);
						switch (GetSmtpReplyCommand(ResponseString))
						{
							case 334:
								// Success
								break;
							default:
								throw new SmtpClientException(ResponseString);
						}

						// Send Password
						string PassB64 = Convert.ToBase64String(Encoding.Default.GetBytes(Password)) + "\r\n";

						//socket.Send(System.Text.Encoding.Default.GetBytes(PassB64));
						StreamWrite(smptStream, PassB64);

						// Step 4. Read Response
						//realReceivet = socket.Receive(inByffer);
						//realReceived = StreamRead(smptStream, inByffer);

						//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
						ResponseString = StreamReadString(smptStream);
						System.Diagnostics.Trace.WriteLine(ResponseString);
						switch (GetSmtpReplyCommand(ResponseString))
						{
							case 235:
								// Success
								break;
							default:
								throw new SmtpClientException(ResponseString);
						}

						#endregion
					}

					result |= SmtpSettingsResult.Authentication;

					#region QUIT
					// Step 14. Send QUIT command
					//socket.Send(System.Text.Encoding.Default.GetBytes("QUIT\r\n"));
					StreamWrite(smptStream, "QUIT\r\n");

					// Step 15. Close Connection
					//realReceivet = socket.Receive(inByffer);
					//realReceived = StreamRead(smptStream, inByffer);

					//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
					ResponseString = StreamReadString(smptStream);
					System.Diagnostics.Trace.WriteLine(ResponseString);
					switch (GetSmtpReplyCommand(ResponseString))
					{
						case 221:
							// Success
							break;
						default:
							throw new SmtpClientException(ResponseString);
					}
					#endregion

					socket.Close();
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex, "SmtpClient.CheckSetting");
			}

			return result;
		}
		#endregion

		/// <summary>
		/// Sends the message.
		/// </summary>
		/// <param name="mailFrom">The mail from.</param>
		/// <param name="rcptTo">The RCPT to.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="emlData">The eml data.</param>
		internal static void SendMessage(OutgoingEmailServiceType serviceType, int? serviceKey, string mailFrom, string rcptTo, string subject, byte[] emlData)
		{
			//EmailMessageSmtpQueueRow newOutMsg = new EmailMessageSmtpQueueRow();
			//newOutMsg.MailFrom = MailFrom;
			//newOutMsg.RcptTo = RcptTo;
			//newOutMsg.Subject = Subject;
			//newOutMsg.Created = DateTime.UtcNow;
			//newOutMsg.ErrorMsg = string.Empty;

			//newOutMsg.Update();
			//using (Stream outStream = newOutMsg.GetEmlData(DbContext.Current.Transaction, SqlBlobAccess.Write))
			//{
			//    outStream.Write(EmlData, 0, EmlData.Length);
			//    outStream.Flush();
			//}
			//tran.Commit();
			EmailEntity email = BusinessManager.InitializeEntity<EmailEntity>(EmailEntity.ClassName);

			email.From = mailFrom;
			email.To = rcptTo;
			email.Subject = subject;
			email.MessageContext = System.Text.Encoding.Default.GetString(emlData);

			email.HtmlBody = string.Empty;

			// Create Message
			CreateRequest request = new CreateRequest(email);
			request.Parameters.Add(OutgoingMessageQueuePlugin.AddToQueue, true);
			request.Parameters.Add(OutgoingMessageQueuePlugin.SourceName, 
				string.Format("{0}{1}", 
					serviceType, 
					serviceKey.HasValue?(":"+serviceKey.Value.ToString()):""));

			BusinessManager.Execute(request);
		}

		/// <summary>
		/// Creates the SMTP client.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		public static SmtpClient CreateSmtpClient(string source)
		{
			Mediachase.IBN.Business.EMail.OutgoingEmailServiceType serviceType = Mediachase.IBN.Business.EMail.OutgoingEmailServiceType.Unknown;
			int? serviceKey = null;

			if (source.Contains(":"))
			{
				string[] items = source.Split(':');
				serviceType = (Mediachase.IBN.Business.EMail.OutgoingEmailServiceType)Enum.Parse(typeof(Mediachase.IBN.Business.EMail.OutgoingEmailServiceType), items[0]);
				serviceKey = int.Parse(items[1]);
			}
			else
			{
				serviceType = (Mediachase.IBN.Business.EMail.OutgoingEmailServiceType)Enum.Parse(typeof(Mediachase.IBN.Business.EMail.OutgoingEmailServiceType), source);
			}

			Mediachase.IBN.Business.EMail.SmtpBox smtpBox = Mediachase.IBN.Business.EMail.OutgoingEmailServiceConfig.FindSmtpBox(serviceType, serviceKey);

			return CreateSmtpClient(smtpBox);
		}

		internal static SmtpClient CreateSmtpClient(Mediachase.IBN.Business.EMail.SmtpBox smtpBox)
		{
			string smtpServerHost = smtpBox.Server;
			if (smtpServerHost == string.Empty)
				smtpServerHost = "localhost";

			SmtpClient smtpClient = new SmtpClient(smtpServerHost, smtpBox.Port);

			smtpClient.Authenticate = smtpBox.Authenticate;
			smtpClient.User = smtpBox.User;
			smtpClient.Password = smtpBox.Password;

			// OZ 2009-05-08 Added Receive Timeou
			smtpClient.ReceiveTimeout = PortalConfig.SmtpRequestTimeout*1000; // Default value 30 (sec)
			//

			smtpClient.SecureConnectionType = smtpBox.SecureConnection;

			return smtpClient;
		}



		//public static void DirectSendMessage(string MailFrom, string RcptTo, string Subject, byte[] EmlData)
		//{
		//    //SmtpBox box = SmtpBox.GetDefault();
		//    //SmtpClient smtpClient = new SmtpClient();

		//    // Test Email

		//    //using (DbTransaction tran = DbTransaction.Begin())
		//    //{
		//    //    EmailMessageSmtpQueueRow newOutMsg = new EmailMessageSmtpQueueRow();
		//    //    newOutMsg.MailFrom = MailFrom;
		//    //    newOutMsg.RcptTo = RcptTo;
		//    //    newOutMsg.Subject = Subject;
		//    //    newOutMsg.Created = DateTime.UtcNow;
		//    //    newOutMsg.ErrorMsg = string.Empty;

		//    //    newOutMsg.Update();

		//    //    using (Stream outStream = newOutMsg.GetEmlData(DbContext.Current.Transaction, SqlBlobAccess.Write))
		//    //    {
		//    //        outStream.Write(EmlData, 0, EmlData.Length);
		//    //        outStream.Flush();
		//    //    }

		//    //    ProcessSendMessages();

		//    //    tran.Commit();
		//    //}
		//}

		/*public static void ProcessSendMessages()
		{
			SmtpBox smtpBox = SmtpBox.GetDefault();

			// TODO:
			// SmtpBox smtpBox = OutgoingEmailServiceConfig.FindSmtpBox(OutgoingEmailServiceType.ExternalPop3IssueBox, incidentBoxId)

			string SmtpServerHost = smtpBox.Server;
			if (SmtpServerHost == string.Empty)
				SmtpServerHost = "localhost";

			int atOnceMsgCount = 0;//undef process all messages
			int processMsgCount = 0;
			int maxAttempCount = 100;
			int bufferSize = 10240;

			EmailMessageSmtpQueueRow[] outMsgs = EmailMessageSmtpQueueRow.List();
			//nothing to send
			if (outMsgs.Length == 0)
				return;

			// Progressive Method
			//byte[] inBuffer = new byte[10 * 1024];
			//int realReceived = -1;
			string ResponseString = string.Empty;

			// Step 1. Open Socket Connection
			using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
			{
				#region Init Socket
				//IPHostEntry entry = Dns.GetHostEntry(SmtpServerHost);
				IPAddress[] addresses = Dns.GetHostAddresses(SmtpServerHost);

				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 30000);
				socket.Connect(new IPEndPoint(addresses[0], smtpBox.Port));
				#endregion

				Stream smptStream = new NetworkStream(socket);

				if (smtpBox.SecureConnection == SecureConnectionType.Ssl)
				{
					SslStream sslStream = new SslStream(smptStream, false,
						new RemoteCertificateValidationCallback(ValidateServerCertificate), null);

					sslStream.AuthenticateAsClient(SmtpServerHost);

					smptStream = sslStream;
				}

				#region Welcome
				// Step 2. Read Welcome
				//realReceivet = socket.Receive(inByffer);
				//realReceived = StreamRead(smptStream,inByffer);

				//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
				ResponseString = StreamReadString(smptStream);

				System.Diagnostics.Trace.WriteLine(ResponseString);
				switch (GetSmtpReplyCommand(ResponseString))
				{
					case 220:
						// Success
						break;
					default:
						throw new SmtpClientException(ResponseString);
				}
				#endregion

				#region HELO
				// Step 3. Send EHLO command
				IPAddress[] ipList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

				if (ipList.Length > 0)
				{
					//socket.Send(System.Text.Encoding.Default.GetBytes(string.Format("EHLO [{0}]\r\n", ipList[0].ToString())));
					StreamWrite(smptStream, "EHLO [{0}]\r\n", ipList[0].ToString());
				}
				else
				{
					//socket.Send(System.Text.Encoding.Default.GetBytes("EHLO\r\n"));
					StreamWrite(smptStream, "EHLO\r\n");
				}

				// Step 4. Read Response
				//realReceivet = socket.Receive(inByffer);
				//realReceived = StreamRead(smptStream, inByffer);

				//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
				ResponseString = StreamReadString(smptStream);

				System.Diagnostics.Trace.WriteLine(ResponseString);
				switch (GetSmtpReplyCommand(ResponseString))
				{
					case 250:
						// Success
						break;
					default:
						throw new SmtpClientException(ResponseString);
				}
				#endregion

				if (smtpBox.SecureConnection == SecureConnectionType.Tls)
				{
					#region STARTTLS
					// Step. Send STARTTLS command
					StreamWrite(smptStream, "STARTTLS\r\n");
					//realReceived = StreamRead(smptStream, inByffer);

					// Step. Read Response
					//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
					ResponseString = StreamReadString(smptStream);
					System.Diagnostics.Trace.WriteLine(ResponseString);

					switch (GetSmtpReplyCommand(ResponseString))
					{
						case 220:
							// Success
							break;
						default:
							throw new SmtpClientException(ResponseString);
					}

					SslStream tlsSmtpStream = new SslStream(smptStream, false,
							new RemoteCertificateValidationCallback(ValidateServerCertificate),
							null);

					tlsSmtpStream.AuthenticateAsClient(SmtpServerHost);

					smptStream = tlsSmtpStream;

					#endregion
				}

				if (smtpBox.Authenticate)
				{
					#region AUTH LOGIN
					//socket.Send(System.Text.Encoding.Default.GetBytes("AUTH LOGIN\r\n"));
					StreamWrite(smptStream, "AUTH LOGIN\r\n");

					// Step 4. Read Response
					//realReceivet = socket.Receive(inByffer);
					//realReceived = StreamRead(smptStream, inByffer);

					//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
					ResponseString = StreamReadString(smptStream);
					System.Diagnostics.Trace.WriteLine(ResponseString);
					switch (GetSmtpReplyCommand(ResponseString))
					{
						case 334:
							// Success
							break;
						default:
							throw new SmtpClientException(ResponseString);
					}

					// Send User Name
					string UserNameB64 = Convert.ToBase64String(Encoding.Default.GetBytes(smtpBox.User)) + "\r\n";

					//socket.Send(System.Text.Encoding.Default.GetBytes(UserNameB64));
					StreamWrite(smptStream, UserNameB64);

					// Step 4. Read Response
					//realReceivet = socket.Receive(inByffer);
					//realReceived = StreamRead(smptStream, inByffer);

					//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
					ResponseString = StreamReadString(smptStream);
					System.Diagnostics.Trace.WriteLine(ResponseString);
					switch (GetSmtpReplyCommand(ResponseString))
					{
						case 334:
							// Success
							break;
						default:
							throw new SmtpClientException(ResponseString);
					}

					// Send Password
					string PassB64 = Convert.ToBase64String(Encoding.Default.GetBytes(smtpBox.Password)) + "\r\n";

					//socket.Send(System.Text.Encoding.Default.GetBytes(PassB64));
					StreamWrite(smptStream, PassB64);

					// Step 4. Read Response
					//realReceivet = socket.Receive(inByffer);
					//realReceived = StreamRead(smptStream, inByffer);

					//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
					ResponseString = StreamReadString(smptStream);
					System.Diagnostics.Trace.WriteLine(ResponseString);
					switch (GetSmtpReplyCommand(ResponseString))
					{
						case 235:
							// Success
							break;
						default:
							throw new SmtpClientException(ResponseString);
					}

					#endregion
				}

				foreach (EmailMessageSmtpQueueRow msg in outMsgs)
				{
					if (msg.Generation == maxAttempCount)
						continue;

					if ((atOnceMsgCount != 0) &&
						(processMsgCount > atOnceMsgCount))
						break;

					using (DbTransaction tran = DbTransaction.Begin())
					{
						string MailFrom = msg.MailFrom;
						string RcptTo = msg.RcptTo;
						string Subject = msg.Subject;

						MemoryStream memStream = new MemoryStream();
						byte[] tmpBuffer = new byte[bufferSize];

						using (Stream inputStream = msg.GetEmlData(DbContext.Current.Transaction, SqlBlobAccess.Read))
						{
							int count = 0;
							do
							{
								count = inputStream.Read(tmpBuffer, 0, bufferSize);
								memStream.Write(tmpBuffer, 0, count);
							}
							while (count > 0);
						}

						memStream.Capacity = (int)memStream.Length;
						byte[] EmlData = memStream.GetBuffer();

						try
						{
							#region MAIL
							// Step 5. Send MAIL FROM command
							//socket.Send(System.Text.Encoding.Default.GetBytes(string.Format("MAIL FROM:<{0}>\r\n", MailFrom)));
							StreamWrite(smptStream, "MAIL FROM:<{0}>\r\n", MailFrom);

							// Step 6. Read Response
							//realReceivet = socket.Receive(inByffer);
							//realReceived = StreamRead(smptStream, inByffer);

							//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
							ResponseString = StreamReadString(smptStream);
							System.Diagnostics.Trace.WriteLine(ResponseString);
							switch (GetSmtpReplyCommand(ResponseString))
							{
								case 250:
									// Success
									break;
								default:
									throw new SmtpClientException(ResponseString);
							}
							#endregion

							#region RCPT
							// Step 7. Send RCPT TO command
							//socket.Send(System.Text.Encoding.Default.GetBytes(string.Format("RCPT TO:<{0}>\r\n", RcptTo)));
							StreamWrite(smptStream, "RCPT TO:<{0}>\r\n", RcptTo);

							// Step 8. Read Response
							//realReceivet = socket.Receive(inByffer);
							//realReceived = StreamRead(smptStream, inByffer);

							//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
							ResponseString = StreamReadString(smptStream);
							System.Diagnostics.Trace.WriteLine(ResponseString);
							switch (GetSmtpReplyCommand(ResponseString))
							{
								case 250:
								case 251:
									// Success
									break;
								default:
									throw new SmtpClientException(ResponseString);
							}
							#endregion

							#region DATA
							// Step 9. Send DATA command
							//socket.Send(System.Text.Encoding.Default.GetBytes("DATA\r\n"));
							StreamWrite(smptStream, "DATA\r\n");

							// Step 10. Read Response
							//realReceivet = socket.Receive(inByffer);
							//realReceived = StreamRead(smptStream, inByffer);

							//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
							ResponseString = StreamReadString(smptStream);
							System.Diagnostics.Trace.WriteLine(ResponseString);
							switch (GetSmtpReplyCommand(ResponseString))
							{
								case 354:
									// Success
									break;
								default:
									throw new SmtpClientException(ResponseString);
							}

							// Step 11. Send EMAIL 
							//socket.Send(EmlData);
							StreamWrite(smptStream, EmlData);

							// Step 12. Send .
							//socket.Send(System.Text.Encoding.Default.GetBytes(".\r\n"));
							StreamWrite(smptStream, ".\r\n");

							// Step 13. Read Response
							//realReceivet = socket.Receive(inByffer);
							//realReceived = StreamRead(smptStream, inByffer);

							//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
							ResponseString = StreamReadString(smptStream);
							System.Diagnostics.Trace.WriteLine(ResponseString);
							switch (GetSmtpReplyCommand(ResponseString))
							{
								case 250:
									// Success
									break;
								default:
									throw new SmtpClientException(ResponseString);
							}
							#endregion

							// OZ: [2007-01-30] EMailMessageLog
							if (EMailMessageLogSetting.Current.IsActive)
							{
								EMailMessageLog.Add(MailFrom,
									RcptTo,
									Subject);
							}

							processMsgCount++;
							//remove message from queue
							msg.Delete();
						}
						catch (SmtpClientException e)
						{
							//inc generation
							msg.Generation = (msg.Generation < maxAttempCount)
												? msg.Generation + 1
												: msg.Generation;
							msg.ErrorMsg = e.Message;
							msg.Update();

							#region RSET
							// Step 14. Send RSET command
							StreamWrite(smptStream, "RSET\r\n");
							ResponseString = StreamReadString(smptStream);
							System.Diagnostics.Trace.WriteLine(ResponseString);
							switch (GetSmtpReplyCommand(ResponseString))
							{
								case 250:
									// Success
									break;
								default:
									throw new SmtpClientException(ResponseString);
							}
							#endregion
						}

						tran.Commit();
					}

				}

				#region QUIT
				// Step 14. Send QUIT command
				//socket.Send(System.Text.Encoding.Default.GetBytes("QUIT\r\n"));
				StreamWrite(smptStream, "QUIT\r\n");

				// Step 15. Close Connection
				//realReceivet = socket.Receive(inByffer);
				//realReceived = StreamRead(smptStream, inByffer);

				//ResponseString = Encoding.Default.GetString(inByffer, 0, realReceived);
				ResponseString = StreamReadString(smptStream);
				System.Diagnostics.Trace.WriteLine(ResponseString);
				switch (GetSmtpReplyCommand(ResponseString))
				{
					case 221:
						// Success
						break;
					default:
						throw new SmtpClientException(ResponseString);
				}
				#endregion

				socket.Close();
			}

		}*/


	}
}
