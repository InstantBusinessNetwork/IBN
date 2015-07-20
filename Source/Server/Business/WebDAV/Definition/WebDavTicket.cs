using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Net.Wdom;
using System.Text.RegularExpressions;

namespace Mediachase.IBN.Business.WebDAV.Definition
{
    /// <summary>
    /// Represent web dav ticket used in url access to element
    /// <remarks>
    ///  /(public_part [handler_type])/(crypt part [sessionId]:[virtual_absolute_path])/(public part [file name])
    /// </remarks>
    /// </summary>
    public class WebDavTicket
    {
        private ePluginToken _handlerType = ePluginToken.webdav;
        private Guid _sessionId;
        private WebDavAbsolutePath _absPath;
		//Обрамляет sessionId в тикете
		private const char SessionIdDelimiter = '%';
		private static Dictionary<char, char> Base64ToUrlReplacementPattern = new Dictionary<char, char>();
		private static Dictionary<char, char> UrlToBase64ReplacementPattern = new Dictionary<char, char>();
		//// ASP.NET returns 400 Bad Request IF url includes: * & % .. # ' +
		private static string[] IllegalUrlFileNameChars = {"%", "&", "*", ":", "#", "'", "+" };

		static WebDavTicket()
		{
			//RFC3548 Url safe
			RegisterReplacement('/', '-');
			RegisterReplacement('+', '_');

		}

        private WebDavTicket()
        {
        }

        private WebDavTicket(ePluginToken handler, Guid sessionId, WebDavAbsolutePath absPath)
        {
            _handlerType = handler;
            _sessionId = sessionId;
            _absPath = absPath;
        }


		#region Properties
		public bool IsCollection
		{
			get
			{
				return string.IsNullOrEmpty(AbsolutePath.FileName);
			}
		}

		public ePluginToken PluginToken
		{
			get
			{
				return _handlerType;
			}
			set
			{
				_handlerType = value;
			}


		}

		public Guid SessionId
		{
			get { return _sessionId; }
			set { _sessionId = value; }
		}


		public WebDavAbsolutePath AbsolutePath
		{
			get { return _absPath; }
			set { _absPath = value; }
		}

		#endregion

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="elementStorageType">Type of the element storage.</param>
        /// <param name="elmentId">The elment id.</param>
        /// <param name="sessionId">The session id.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        //public static WebDavTicket CreateInstance(ePluginToken handler, ObjectTypes elementStorageType, 
        //                    int elmentId, Guid sessionId, string fileName)
        //{
        //    return new WebDavTicket(handler, elementStorageType, elmentId, sessionId, fileName);
        //}

        public static WebDavTicket CreateInstance(ePluginToken handler, Guid sessionId, WebDavAbsolutePath absPath)
        {
            return new WebDavTicket(handler, sessionId, absPath);
        }

        /// <summary>
        /// Parses the specified SRC ticket.
        /// </summary>
        /// <remarks>Format specified: P - plugin token A - absolute path with session id F - file name</remarks>
        /// <param name="srcTicket">The SRC ticket.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static WebDavTicket Parse(string srcTicket, string format)
        {
            return ParseByFormat(srcTicket, format);
        }

        /// <summary>
        /// Parses the specified SRC ticket.
        /// </summary>
        /// <param name="srcTicket">The SRC ticket.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static WebDavTicket Parse(string srcTicket)
        {
            WebDavTicket retVal = null;
            try
            {
                retVal = ParseByFormat(srcTicket, "/P/A/F");
            }
            catch (System.FormatException)
            {
            }
            if (retVal == null)
            {
                retVal = ParseByFormat(srcTicket, "A");
            }

            return retVal;
        }

        private static WebDavTicket ParseByFormat(string srcTicket, string format)
        {
            WebDavTicket retVal = new WebDavTicket();

            int index = 0;
            StringBuilder regExp = new StringBuilder();
            regExp.Append(@"^");
            foreach (char patternChar in format.ToCharArray())
            {
				char nextChar = (format.Length - index) > 1 ? format[index + 1] : '/';
                switch (patternChar)
                {
                    case 'P':
                        regExp.Append(String.Format("(?<PLUGIN_TOKEN>[^{0}]+)", nextChar));
                        break;
                    case 'A':
                        regExp.Append(String.Format("(?<ABSOLUTE_PATH>[^{0}]+)", nextChar));
                        break;
                    case 'F':
                        regExp.Append(@"(?<FILE_NAME>[^\\/:*?\""<>|\r\n]*)");
                        break;

                    default:
                        regExp.Append(patternChar);
                        break;
                }

                index++;
            }

            try
            {
                Regex regex = new Regex(regExp.ToString());
                Match match = regex.Match(srcTicket.Trim());
                string fileName = null;

                string capture = match.Groups["PLUGIN_TOKEN"].Value;
                if (!string.IsNullOrEmpty(capture))
                {
                    retVal.PluginToken = (ePluginToken)Enum.Parse(typeof(ePluginToken), capture);
                }

                capture = match.Groups["FILE_NAME"].Value;
                if (!string.IsNullOrEmpty(capture))
                {
                    fileName = capture;
                }

                capture = match.Groups["ABSOLUTE_PATH"].Value;
				Guid sessionId;
				WebDavAbsolutePath absPath;
				ParseAbsPathTicket(capture, out sessionId, out absPath);

				retVal.SessionId = sessionId;
				retVal.AbsolutePath = absPath;
                retVal.AbsolutePath.FileName = fileName;
            }
			catch (FormatException e)
			{
				throw new FormatException(e.Message);
			}
            catch (System.Exception)
            {
                throw new FormatException("invalid ticket");
            }

            return retVal;
        }

		private static int IndexOfCharInByteArray(byte[] array, char pattern, int startIndex)
		{
			int retVal = -1;
			for(int index = startIndex; index < array.Length; index += 2)
			{
				char examined = BitConverter.ToChar(array, index);
				if (examined == pattern)
				{
					retVal = index; 
					break;
				}
			}
			return retVal;
		}

		/// <summary>
		/// Разбирает зашифрованную часть WebDav тикета. использует сивол разделителя для определения части session и absPath
		/// Разбор ведется на бинарном уровне.
		/// </summary>
		private static void ParseAbsPathTicket(string ticket, out Guid sessionId, out WebDavAbsolutePath absPath)
		{
			sessionId = Guid.Empty;
			absPath = null;

			byte[] ticketArr = DecryptToken(ticket); 
			byte[] absPathArr = new byte[ticketArr.Length];
			int offset;
			sessionId = GetSessionIdFromTicketArray(ticketArr, out offset);

			Array.Copy(ticketArr, offset, absPathArr, 0, ticketArr.Length - offset);
			absPath = WebDavAbsolutePath.Parse(absPathArr);
		}

		private static Guid GetSessionIdFromTicketArray(byte[] ticketArr, out int offset)
		{
			Guid retVal = Guid.Empty;
			byte[] sessionIdArr = Guid.Empty.ToByteArray();
			int attemptCount = 0;
			offset = IndexOfCharInByteArray(ticketArr, SessionIdDelimiter, 2);
			while (offset != 2 && offset != (sessionIdArr.Length + 2))
			{
				offset = IndexOfCharInByteArray(ticketArr, SessionIdDelimiter, offset + 2);
				if (attemptCount++ > ticketArr.Length / 2)
				{
					break;
				}
			}
			//Guid length
			if (offset == (sessionIdArr.Length + 2))
			{
				Array.Copy(ticketArr, 2, sessionIdArr, 0, offset - 2);
				retVal = new Guid(sessionIdArr);
			}
			offset += 2;
			return retVal;
		}

        private static byte[] DecryptToken(string str)
        {
            return  Convert.FromBase64String(Base64SafeReplace(UrlToBase64ReplacementPattern, str));
        }

        private static string CryptToken(byte[] byteArr)
        {
			//В Windows приложениях использующих компонент IE, ссылки заканчивающиеся на ==, распознаются компонентом 
			//без завершающего ==. Так как часть WebDav тикета AbsolutePath  кодируется кодировакой BASE64 (rfc3548) то
			//если количество битов в исходном наборе не кратно 6 c остатком 4 то в конце кодированной последоватеольности 
			//добавляется два символа ==, если не кратен 6 c остатком 2 то в конце добавляется один символ =.
			//Для решения данной проблемы, необходимо выровнять исходную последоватеотьность до кратной 6.
			int offset = (byteArr.Length * 8) % 6 == 0 ? 0 : (byteArr.Length * 8) % 6;
			byte[] byteArrNormalized = new byte[byteArr.Length + offset];
			Array.Copy(byteArr, byteArrNormalized, byteArr.Length);
			return Base64SafeReplace(Base64ToUrlReplacementPattern, Convert.ToBase64String(byteArrNormalized));
        }
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return ToString("/P/A/F");
        }


        /// <summary>
        /// Toes the string.
        /// </summary>
        /// <remarks>Format specified: P - plugin token A - absolutepath with session id F - file name</remarks>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public string ToString(string format)
        {
            StringBuilder retVal = new StringBuilder();
            foreach (char patternChar in format.ToCharArray())
            {
                switch (patternChar)
                {
                    case 'P':
                        retVal.Append(PluginToken.ToString());
                        break;
                    case 'A':
                        List<byte> token = new List<byte>();
						token.AddRange(BitConverter.GetBytes(SessionIdDelimiter));
                        //If SessionId is empty then no save him
                        if (SessionId != Guid.Empty)
                        {
                            token.AddRange(SessionId.ToByteArray());
                        }
						token.AddRange(BitConverter.GetBytes(SessionIdDelimiter));
                        //absolute path
                        token.AddRange(AbsolutePath.GetByteArray());
                        retVal.Append(CryptToken(token.ToArray()));
                        break;
                    case 'F':
						retVal.Append(FileNameReplaceInvalidChars(AbsolutePath.FileName));
                        break;

                    default:
                        retVal.Append(patternChar);
                        break;
                }
            }

            return retVal.ToString();
        }

		/// <summary>
		/// Replace invalid char in file name
		/// http://support.microsoft.com/default.aspx?scid=kb;EN-US;826437
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns></returns>
		private static string FileNameReplaceInvalidChars(string fileName)
		{
			String retVal = fileName;

			if (!string.IsNullOrEmpty(retVal))
			{
				foreach (string illegalChar in IllegalUrlFileNameChars)
				{
					retVal = retVal.Replace(illegalChar, string.Empty);
				}
				//replace .. to .
				while(retVal.Contains(".."))
				{
					retVal = retVal.Replace("..", ".");
				}
			}
			return retVal;
		}

		private static void RegisterReplacement(char toReplace, char fromReplace)
		{
			Base64ToUrlReplacementPattern.Add(toReplace, fromReplace);
			UrlToBase64ReplacementPattern.Add(fromReplace, toReplace);
		}

		private static string Base64SafeReplace(Dictionary<char, char> replacementPattern, string base64str)
		{
			String retVal = base64str;
			foreach (char key in replacementPattern.Keys)
			{
				retVal = retVal.Replace(key, replacementPattern[key]);
			}

			return retVal;
		}

	
	
    }
}
