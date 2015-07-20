using System;
using System.Text.RegularExpressions;

namespace Mediachase.IBN.Business.EMail
{
	public class TicketUidUtil
	{
		public static string Create(string pointKey, int threadId)
		{
			string smallUID = string.Format("{0}-{1:D6}", pointKey, threadId);
			byte[] tmpBuffer = System.Text.Encoding.UTF8.GetBytes(smallUID);
			return string.Format("{0}-{1:D5}", smallUID, Crc16.Calculate(tmpBuffer, 0, tmpBuffer.Length));
		}

		public static int GetThreadId(string str)
		{
			string regExPattern = @"(?<UIDKey>\w+)\s*-\s*(?<UIDThreadId>\d{6,})\s*-\s*(?<UIDCrc16>\d{5})";
			Regex regEx = new Regex(regExPattern, RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
			Match match = regEx.Match(str);
			if (match.Success)
			{
				string UIDKey = match.Groups["UIDKey"].Value;
				string UIDThreadId = match.Groups["UIDThreadId"].Value;
				string UIDCrc16 = match.Groups["UIDCrc16"].Value;

				string retVal = string.Format("{0}-{1}-{2}", UIDKey, UIDThreadId, UIDCrc16);
				string smallUID = string.Format("{0}-{1}", UIDKey, UIDThreadId);

				byte[] tmpBuffer = System.Text.Encoding.UTF8.GetBytes(smallUID);
				if (Crc16.Calculate(tmpBuffer, 0, tmpBuffer.Length).ToString("D5") == UIDCrc16)
				{
					try
					{
						return int.Parse(UIDThreadId);
					}
					catch
					{
					}
				}
				else
				{
					// Wrong UID
				}
			}

			return -1;
		}

		public static string LoadFromString(string str)
		{
			string regExPattern = @"(?<UIDKey>\w+)\s*-\s*(?<UIDThreadId>\d{6,})\s*-\s*(?<UIDCrc16>\d{5})";
			Regex regEx = new Regex(regExPattern, RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
			Match match = regEx.Match(str);
			if (match.Success)
			{
				string UIDKey = match.Groups["UIDKey"].Value;
				string UIDThreadId = match.Groups["UIDThreadId"].Value;
				string UIDCrc16 = match.Groups["UIDCrc16"].Value;

				string retVal = string.Format("{0}-{1}-{2}", UIDKey, UIDThreadId, UIDCrc16);
				string smallUID = string.Format("{0}-{1}", UIDKey, UIDThreadId);

				byte[] tmpBuffer = System.Text.Encoding.UTF8.GetBytes(smallUID);
				if (Crc16.Calculate(tmpBuffer, 0, tmpBuffer.Length).ToString("D5") == UIDCrc16)
				{
					return retVal;
				}
				else
				{
					// Wrong UID
				}
			}

			return string.Empty;

		}
	}

}
