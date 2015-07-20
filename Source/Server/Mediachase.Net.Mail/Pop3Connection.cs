using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Mediachase.Net.Mail
{
    /// <summary>
    /// Summary description for Pop3Connection.
    /// </summary>
    public class Pop3Connection
    {
        private char[] _Delimiter = (" \r\n").ToCharArray();

        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private Stream _stream = null;

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        virtual protected Socket Connection
        {
            get
            {
                return _socket;
            }
        }

        /// <summary>
        /// Gets the network stream.
        /// </summary>
        /// <value>The network stream.</value>
        virtual protected Stream NetworkStream
        {
            get
            {
                return _stream;
            }
            set
            {
                _stream = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pop3Connection"/> class.
        /// </summary>
        public Pop3Connection()
        {
        }

        /// <summary>
        /// Opens the SSL.
        /// </summary>
        /// <param name="HostName">Name of the host.</param>
        /// <param name="Port">The port.</param>
        public void OpenSsl(string HostName, int Port)
        {
            IPHostEntry hostInfo = Dns.GetHostEntry(HostName);
            IPAddress ipAddress = hostInfo.AddressList[0];

            IPEndPoint pop3ServerEndPoint = new IPEndPoint(ipAddress, Port);

            OpenSsl(pop3ServerEndPoint, HostName);
        }

        /// <summary>
        /// Opens the SSL.
        /// </summary>
        /// <param name="serverIP">The server IP.</param>
        /// <param name="targetHost">The target host.</param>
        public void OpenSsl(IPEndPoint serverIP ,string targetHost)
        {
            this.Connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 30000);
            this.Connection.Connect(serverIP);
            this.NetworkStream = new NetworkStream(this.Connection, false);

            SslStream sslStream = new SslStream(this.NetworkStream, false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                null);
            sslStream.AuthenticateAsClient(targetHost);

            this.NetworkStream = sslStream;


            string welcomeMsg = this.ReadOKResponse();
        }

        /// <summary>
        /// Opens the specified host name.
        /// </summary>
        /// <param name="HostName">Name of the host.</param>
        /// <param name="Port">The port.</param>
        public void Open(string HostName, int Port)
        {
            IPHostEntry hostInfo = Dns.GetHostEntry(HostName);
            IPAddress ipAddress = hostInfo.AddressList[0];

            IPEndPoint pop3ServerEndPoint = new IPEndPoint(ipAddress, Port);

            Open(pop3ServerEndPoint);
        }

        /// <summary>
        /// Opens the specified server IP.
        /// </summary>
        /// <param name="serverIP">The server IP.</param>
        public void Open(IPEndPoint serverIP)
        {
            this.Connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 30000);
            this.Connection.Connect(serverIP);
            this.NetworkStream = new NetworkStream(this.Connection, false);

            string welcomeMsg = this.ReadOKResponse();
        }

        /// <summary>
        /// Send user command, response should indicate that a password is required. 
        /// </summary>
        /// <param name="Name"></param>
        public void User(string Name)
        {
            this.SendCommand(string.Format("USER {0}\r\n", Name));
            string response = this.ReadOKResponse();
        }

        /// <summary>
        /// Send password, response includes message count and mailbox size. Note: the mailbox on the server is locked until quit() is called. 
        /// </summary>
        /// <param name="Password"></param>
        public void Pass(string Password)
        {
            this.SendCommand(string.Format("PASS {0}\r\n", Password));
            string response = this.ReadOKResponse();
        }

        /// <summary>
        /// Signoff: commit changes, unlock mailbox, drop connection
        /// </summary>
        public void Quit()
        {
            this.SendCommand("QUIT\r\n");
            string response = this.ReadOKResponse();

            this.Connection.Close();
        }

        /// <summary>
        /// Get mailbox status. The result is a tuple of 2 integers: (message count, mailbox size). 
        /// </summary>
        public Pop3Stat Stat()
        {
            //	CLIENT: STAT
            //	SERVER: +ОК 2 320

            this.SendCommand("STAT\r\n");
            string statResponse = this.ReadOKResponse();

            string[] NumArray = statResponse.Split(_Delimiter);
            if (NumArray.Length != 2)
                throw new Pop3ServerIncorectAnswerException();

            Pop3Stat answer = null;

            try
            {
                answer = new Pop3Stat(Int32.Parse(NumArray[0]), Int32.Parse(NumArray[1]));
            }
            catch (Exception ex)
            {
                throw new Pop3ServerIncorectAnswerException("Incorect response format.", ex);
            }

            return answer;
        }

        /// <summary>
        /// Request message list, result is in the form (response, ['mesg_num octets', ...]). If which is set, it is the message to list. 
        /// </summary>
        public Pop3MessageInfo List(int MessageID)
        {
            //			CLIENT: LIST 2
            //			SERVER: +ОК 2 200 ...
            //			CLIENT: LIST 3
            //			SERVER: -ERR no such message, only 2 messages in maildrop
            this.SendCommand(string.Format("LIST {0}\r\n", MessageID));

            string listResponse = this.ReadOKResponse();

            string[] NumArray = listResponse.Split(_Delimiter);
            if (NumArray.Length != 2)
                throw new Pop3ServerIncorectAnswerException();

            Pop3MessageInfo message = null;

            try
            {
                message = new Pop3MessageInfo(Int32.Parse(NumArray[0]), null, Int32.Parse(NumArray[1]));
            }
            catch (Exception ex)
            {
                throw new Pop3ServerIncorectAnswerException("Incorect response format.", ex);
            }

            return message;
        }

        /// <summary>
        /// Request message list, result is in the form (response, ['mesg_num octets', ...]). If which is set, it is the message to list. 
        /// </summary>
        public Pop3MessageInfoList List()
        {
            //			CLIENT: LIST
            //			SERVER: +ОК 2 messages (320 octets)
            //			SERVER: 1 120
            //			SERVER: 2 200
            //			SERVER: . ...
            this.SendCommand("LIST\r\n");
            string statResponse = this.ReadOKResponse();

            Pop3MessageInfoList list = new Pop3MessageInfoList();

            while (true)
            {
                string responseLine = ReadLine();

                if (responseLine.StartsWith("."))
                    break;

                string[] NumArray = responseLine.Split(_Delimiter);
                if (NumArray.Length != 2)
                    throw new Pop3ServerIncorectAnswerException();

                Pop3MessageInfo message = null;

                try
                {
                    message = new Pop3MessageInfo(Int32.Parse(NumArray[0]), null, Int32.Parse(NumArray[1]));
                }
                catch (Exception ex)
                {
                    throw new Pop3ServerIncorectAnswerException("Incorect response format.", ex);
                }

                list.Add(message);
            }

            return list;
        }

        /// <summary>
        /// Retrieve whole message number which, and set its seen flag. Result is in form (response, ['line', ...], octets). 
        /// </summary>
        /// <param name="MessageId"></param>
        public Pop3Message Retr(int MessageId)
        {
            this.SendCommand(string.Format("RETR {0}\r\n", MessageId));
            string statResponse = this.ReadOKResponse();

            // Begin Read Binary Data (For more information about message see RFC822) [1/22/2004]
            MemoryStream EMailBinaryData = ReadMessageResponse();


            // Parse Message [1/22/2004]
            return new Pop3Message(EMailBinaryData);
        }

        /// <summary>
        /// Flag message number which for deletion. On most servers deletions are not actually performed until QUIT (the major exception is Eudora QPOP, which deliberately violates the RFCs by doing pending deletes on any disconnect). 
        /// </summary>
        /// <param name="MessageId"></param>
        public void Dele(int MessageId)
        {
            this.SendCommand(string.Format("DELE {0}\r\n", MessageId));
            string statResponse = this.ReadOKResponse();
        }


        /// <summary>
        /// Do nothing. Might be used as a keep-alive. 
        /// </summary>
        public void Noop()
        {
            this.SendCommand("NOOP\r\n");
            string statResponse = this.ReadOKResponse();
        }


        /// <summary>
        /// Сервер возвращает наибольший номер сообщения из тех, к которым ранее уже обращались
        /// </summary>
        /// <param name="Index"></param>
        public void Last(out int Index)
        {
            Index = 0;
        }

        /// <summary>
        /// Remove any deletion marks for the mailbox. 
        /// </summary>
        public void Rset()
        {
            this.SendCommand("RSET\r\n");
            string statResponse = this.ReadOKResponse();
        }

        /// <summary>
        /// Uidls the specified list.
        /// </summary>
        /// <param name="List">The list.</param>
        public void Uidl(Pop3MessageInfoList List)
        {
            Pop3UIDInfoList pop3UIDLInfoList = this.Uidl();

            for (int iIndex = 0; iIndex < pop3UIDLInfoList.Count; iIndex++)
            {
                List[iIndex].UID = pop3UIDLInfoList[iIndex].UID;
            }
        }

        /// <summary>
        /// Uidls this instance.
        /// </summary>
        /// <returns></returns>
        public Pop3UIDInfoList Uidl()
        {
            this.SendCommand("UIDL\r\n");
            string uidlResponse = this.ReadOKResponse();

            Pop3UIDInfoList list = new Pop3UIDInfoList();

            while (true)
            {
                string responseLine = ReadLine();

                if (responseLine.StartsWith("."))
                    break;

                string[] NumArray = responseLine.Split(_Delimiter);
                if (NumArray.Length != 2)
                    throw new Pop3ServerIncorectAnswerException();

                Pop3UIDInfo message = null;

                try
                {
                    message = new Pop3UIDInfo(Int32.Parse(NumArray[0]), NumArray[1]);
                }
                catch (Exception ex)
                {
                    throw new Pop3ServerIncorectAnswerException("Incorect response format.", ex);
                }

                list.Add(message);
            }
            return list;
        }

        /// <summary>
        /// Uidls the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Uidl(Pop3MessageInfo message)
        {
            message.UID = this.Uidl(message.ID).UID;
        }


        /// <summary>
        /// Uidls the specified message ID.
        /// </summary>
        /// <param name="MessageID">The message ID.</param>
        /// <returns></returns>
        public Pop3UIDInfo Uidl(int MessageID)
        {
            this.SendCommand(string.Format("UIDL {0}\r\n", MessageID));
            string uidlResponse = this.ReadOKResponse();

            string[] NumArray = uidlResponse.Split(_Delimiter);
            if (NumArray.Length != 2)
                throw new Pop3ServerIncorectAnswerException();

            return new Pop3UIDInfo(MessageID, NumArray[1]);
        }

        /// <summary>
        /// Use the more secure APOP authentication to log into the POP3 server. 
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Secret"></param>
        public void Apop(string User, string Secret)
        {
            //TODO: need implementation
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use RPOP authentication (similar to UNIX r-commands) to log into POP3 server. 
        /// </summary>
        /// <param name="User"></param>
        public void Rpop(string User)
        {
            //TODO: need implementation
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the message header plus howmuch lines of the message after the header of message number which. Result is in form (response, ['line', ...], octets). 
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Size"></param>
        public Pop3Message Top(int Index, int Size)
        {
            this.SendCommand(string.Format("TOP {0} {1}\r\n", Index, Size));
            string statResponse = this.ReadOKResponse();

            // Begin Read Binary Data (For more information about message see RFC822) [1/22/2004]
            MemoryStream EMailBinaryData = ReadMessageResponse();

            // Parse Message [1/22/2004]
            return new Pop3Message(EMailBinaryData);
        }

        /// <summary>
        /// Returns Capability list
        /// </summary>
        /// <returns></returns>
        public string[] Capa()
        {
            List<string> retVal = new List<string>();

            //CLIENT: 
            //CAPA
            //SERVER:
            //+OK Capability list follows
            //TOP
            //USER
            //LOGIN-DELAY 120
            //PIPELINING
            //EXPIRE NEVER
            //UIDL
            //IMPLEMENTATION Mail.Ru
            //.

            this.SendCommand("CAPA\r\n");
            string statResponse = this.ReadOKResponse();

            while (true)
            {
                string responseLine = ReadLine();

                if (responseLine.StartsWith("."))
                    break;

                retVal.Add(responseLine);
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Starts TLS connection.
        /// </summary>
        /// <param name="targetHost">The target host.</param>
        public void Stls(string targetHost)
        {
            this.SendCommand("STLS\r\n");
            string statResponse = this.ReadOKResponse();

            SslStream tlsStream = new SslStream(this.NetworkStream, false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                null);

            tlsStream.AuthenticateAsClient(targetHost);

            this.NetworkStream = tlsStream;
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

        #region Inner Data Methods
        protected virtual byte[] ReadByte()
        {
            int iRealRead;
            return ReadBytes(1, out iRealRead);
        }

        protected virtual byte[] ReadBytes(int BufferSize, out int iRealRead)
        {
            byte[] retArray = new byte[BufferSize];

            //iRealRead = this.Connection.Receive(retArray);
            iRealRead = this.NetworkStream.Read(retArray, 0, BufferSize);

            if (iRealRead == 0)
                return null;

            return retArray;
        }

        protected virtual string ReadLine()
        {
            byte[] bs = null;
            LineParser linePaser = new LineParser();

            ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();

            while ((bs = ReadByte()) != null)
            {
                linePaser.Write(aSCIIEncoding.GetChars(bs));

                string strNextLine = linePaser.ReadLine();
                if (strNextLine != null)
                {
                    return strNextLine;
                }
            }

            throw new Pop3ServerIncorectAnswerException();
        }

        protected virtual string ReadOKResponse()
        {
            string strNextLine = ReadLine();

            System.Diagnostics.Debug.WriteLine(string.Format("SERVER:\r\n {0}", strNextLine));

            if (strNextLine.StartsWith("+OK"))
            {
                if (strNextLine.Length == 3)
                    return "";
                else
                    return strNextLine.Substring(4);
            }

            throw new Pop3ServerReturnErrorException(strNextLine);
        }

        protected virtual MemoryStream ReadMessageResponse()
        {
            MemoryStream memoryStream = new MemoryStream();

            byte[] bs = null;
            byte[] bsTmpNextLine = null;
            int iRealRead;

            string strPrevLine = string.Empty;

            while ((bs = ReadBytes(4096, out iRealRead)) != null)
            {
                if (bsTmpNextLine != null)
                {
                    MemoryStream tmpMemory = new MemoryStream();
                    tmpMemory.Write(bsTmpNextLine, 0, bsTmpNextLine.Length);
                    tmpMemory.Write(bs, 0, iRealRead);
                    bs = tmpMemory.GetBuffer();
                    iRealRead = (int)tmpMemory.Length;
                    bsTmpNextLine = null;
                }
                StreamLineReader lineReader = new StreamLineReader(bs, 0, iRealRead);

                byte[] bsNextLine = null;

                System.Diagnostics.Debug.WriteLine("-----------------------------------------------");

                while ((bsNextLine = lineReader.ReadLine()) != null)
                {
                    if (lineReader.CRLFWasFound)
                    {
                        string strLine = Encoding.ASCII.GetString(bsNextLine);

                        if (strLine.StartsWith(".") && strLine.Length == 1)
                        {
                            // we are found end.
                            memoryStream.Flush();
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            return memoryStream;
                        }
                        System.Diagnostics.Debug.WriteLine(strLine);

                        memoryStream.Write(bsNextLine, 0, bsNextLine.Length);
                        memoryStream.WriteByte((byte)'\r');
                        memoryStream.WriteByte((byte)'\n');

                        strPrevLine = strLine;
                    }
                    else
                    {
                        bsTmpNextLine = bsNextLine;
                    }

                }
            }

            throw new Pop3ServerIncorectAnswerException();
        }

        protected virtual void SendCommand(string cmdText)
        {
            System.Diagnostics.Debug.Write(string.Format("CLIENT:\r\n {0}", cmdText));
            ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();

            char[] tmpCharArray = cmdText.ToCharArray();
            byte[] tmpByteArray = new byte[tmpCharArray.Length];

            aSCIIEncoding.GetBytes(tmpCharArray, 0, tmpCharArray.Length, tmpByteArray, 0);

            //int iSentBytes = this.Connection.Send(tmpByteArray);
            this.NetworkStream.Write(tmpByteArray, 0, tmpByteArray.Length);
            //if(iSentBytes!=tmpByteArray.Length)
            //    throw new Pop3SendDataErrorException(string.Format("Socket sent only {0} bytes to server (buffer size {1} bytes).",iSentBytes,tmpByteArray.Length));

        }

        #endregion
    }

}
