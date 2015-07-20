using System;
using System.Net;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Mediachase.Net.Mail
{
	public enum DataParseStatus
	{
		NeedMoreData = 0,

		ContinueParsing = 1,

		Done = 2,

		Invalid = 3,

	}
	/// <summary>
	/// Summary description for Rfc822HeaderCollection.
	/// </summary>
	public class Rfc822HeaderCollection: NameValueCollection
	{
		private readonly static HeaderInfoTable HInfo = new HeaderInfoTable();

		public static string CheckBadChars(string name, bool isHeaderValue)
		{
			if (name == null || name.Length == 0)
			{
				if (!isHeaderValue)
				{
					throw new ArgumentException("value");
				}
				else
				{
					return String.Empty;
				}
			}
			if (isHeaderValue)
			{
				name = name.Trim();
				bool flag = false;
				CharEnumerator charEnumerator = name.GetEnumerator();
				while (charEnumerator.MoveNext())
				{
					char ch = charEnumerator.Current;
					if (ch == '\u007f' || ch < ' ' && ch != '\t' && ch != '\r' && ch != '\n')
					{
						throw new ArgumentException("value");
					}
					if (flag)
					{
						if (ch != ' ' && ch != '\t')
						{
							throw new ArgumentException("value");
						}
						flag = false;
					}
					else if (ch == '\n')
					{
						flag = true;
					}
				}
			}
			else
			{
				if (name.IndexOfAny(ValidationHelper.InvalidParamChars) != -1)
				{
					throw new ArgumentException("name");
				}
				if (ContainsNonAsciiChars(name))
				{
					throw new ArgumentException("name");
				}
			}
			return name;
		}

		public static bool IsValidToken(string token)
		{
			if (token.Length > 0 && token.IndexOfAny(ValidationHelper.InvalidParamChars) == -1)
			{
				return ContainsNonAsciiChars(token) == false;
			}
			else
			{
				return false;
			}
		}

		public static bool ContainsNonAsciiChars(string token)
		{
			for (int i = 0; i < token.Length; i++)
			{
				if (token[i] < ' ' || token[i] > '~')
				{
					return true;
				}
			}
			return false;
		}

		public override void Add(string name, string value)
		{
			name = CheckBadChars(name, false);
//			ThrowOnRestrictedHeader(name);
			value = CheckBadChars(value, true);
			base.Add(name, value);
		}

		public override string[] GetValues(string header)
		{
			HeaderInfo headerInfo = HInfo[header];
			string[] strs1 = base.GetValues(header);
			if (headerInfo == null || strs1 == null || !headerInfo.AllowMultiValues)
			{
				return strs1;
			}
			ArrayList arrayList = null;
			for (int i = 0; i < (int)strs1.Length; i++)
			{
				string[] strs2 = headerInfo.Parser(strs1[i]);
				if (arrayList != null)
				{
					arrayList.AddRange(strs2);
				}
				else if ((int)strs2.Length > 1)
				{
					arrayList = new ArrayList(strs1);
					arrayList.RemoveRange(i, (int)strs1.Length - i);
					arrayList.AddRange(strs2);
				}
			}
			if (arrayList == null)
			{
				return strs1;
			}
			string[] strs3 = new string[(uint)arrayList.Count];
			arrayList.CopyTo(strs3);
			return strs3;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(30 * base.Count);
			for (int i = 0; i < base.Count; i++)
			{
				string strKey = base.GetKey(i);
				string strValue = base.Get(i);
				stringBuilder.Append(strKey).Append(": ");
				stringBuilder.Append(strValue).Append("\r\n");
			}
			stringBuilder.Append("\r\n");
			return stringBuilder.ToString();
		}

		public static string Encode2AsciiString(string value)
		{
			return Encode2AsciiString(value, false);
		}

		public static string Encode2AsciiString(string value, bool encodeQuotes)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			StringBuilder sbRetVal = new StringBuilder(value.Length * 2);

			int nonAsciiCharIndex = -1;

			for (int index = 0; index < value.Length; index++)
			{
				char ch = value[index];

				if (ch > '\x007f' || ch == '\x0022' /*"*/ && encodeQuotes)
				{
					if (nonAsciiCharIndex == -1)
						nonAsciiCharIndex = index;
				}
				else
				{
					if (nonAsciiCharIndex != -1)
					{
						if (ch == '\x0020')
							continue;

						string strNonAscii = value.Substring(nonAsciiCharIndex, index - nonAsciiCharIndex);
						byte[] utf8Buffer = Encoding.UTF8.GetBytes(strNonAscii);
						string strBase64 = Convert.ToBase64String(utf8Buffer);
						sbRetVal.AppendFormat("=?utf-8?B?{0}?=", strBase64);
						nonAsciiCharIndex = -1;
					}

					sbRetVal.Append(ch);
				}
			}

			if (nonAsciiCharIndex != -1)
			{
				string strNonAscii = value.Substring(nonAsciiCharIndex, value.Length - nonAsciiCharIndex);
				byte[] utf8Buffer = Encoding.UTF8.GetBytes(strNonAscii);
				string strBase64 = Convert.ToBase64String(utf8Buffer);
				sbRetVal.AppendFormat("=?utf-8?B?{0}?=", strBase64);
				nonAsciiCharIndex = -1;
			}

			return sbRetVal.ToString();
		}

		public string GetAsciiString()
		{
			StringBuilder stringBuilder = new StringBuilder(30 * base.Count);
			for (int i = 0; i < base.Count; i++)
			{
				string strHeader = base.GetKey(i);
				string strValue = Encode2AsciiString(base.Get(i));

				stringBuilder.Append(strHeader).Append(": ");
				stringBuilder.Append(strValue).Append("\r\n");
			}
			stringBuilder.Append("\r\n");
			return stringBuilder.ToString();
		}

		public byte[] ToByteArray()
		{
			string str = GetAsciiString();
			return Encoding.UTF8.GetBytes(str);
		}

		public Rfc822HeaderCollection()
		{
		}

		public DataParseStatus ParseHeaders(byte[] buffer, int size, ref int unparsed)
		{
			int headerKeyBegin = -1;
			int hederKeyEnd = -1;
			int finalCLRNCount = -1;
			int currIndex = unparsed;

			Encoding defaultEncoding = Encoding.Default;

			do
			{
				string strHeader = String.Empty;
				
				bool flag1 = false;

				bool isHeaderKeyUTF8 = false;
				bool isHeaderValueUTF8 = false;

				string strHeaderValue = null;

				if (base.Count == 0)
				{
					while(currIndex < size && (buffer[currIndex] == 32 /*[space]*/ 
						|| buffer[currIndex] == 9))
						currIndex++;

					if (currIndex == size)
					{
						return DataParseStatus.NeedMoreData;
					}
				}

				headerKeyBegin = currIndex;
				for (; currIndex < size && buffer[currIndex] != 58/*:*/ && buffer[currIndex] != 10; currIndex++)
				{
					if (buffer[currIndex] > 32 /*[space]*/)
					{
						hederKeyEnd = currIndex;
						//isHeaderKeyUTF8 = !isHeaderKeyUTF8 ? (buffer[currIndex] > 127) : true;
					}
				}

				if (currIndex == size)
				{
					return DataParseStatus.NeedMoreData;
				}

				while (true)
				{
					string strHeaderValueTmp = String.Empty;
					int headerValueBegin = -1;
					int headerValueEnd = -1;

					for (finalCLRNCount = (base.Count != 0 || hederKeyEnd >= 0) ? 0 : 1; currIndex < size && finalCLRNCount < 2 && (buffer[currIndex] <= 32 /*[space]*/ || buffer[currIndex] == 58); currIndex++)
					{
						if (buffer[currIndex] == 10)
						{
							finalCLRNCount++;
							flag1 = (currIndex + 1 < size) ? ((buffer[currIndex + 1] != 32 /*[space]*/) ? (buffer[currIndex + 1] == 9) : true) : false;
						}
					}

					if (finalCLRNCount != 2 && (finalCLRNCount != 1 || flag1))
					{
						if (currIndex == size)
						{
							return DataParseStatus.NeedMoreData;
						}

						headerValueBegin = currIndex;
						for (; currIndex < size && buffer[currIndex] != 10; currIndex++)
						{
							if (buffer[currIndex] > 32 /*[space]*/)
							{
								headerValueEnd = currIndex;
								//isHeaderValueUTF8 = !isHeaderValueUTF8 ? (buffer[currIndex] > 127) : true;
							}
						}

						if (currIndex == size)
						{
							return DataParseStatus.NeedMoreData;
						}

						for (finalCLRNCount = 0; currIndex < size && finalCLRNCount < 2 && (buffer[currIndex] == 13 || buffer[currIndex] == 10); currIndex++)
						{
							if (buffer[currIndex] == 10)
							{
								finalCLRNCount++;
							}
						}

						if (currIndex == size && finalCLRNCount < 2)
						{
							return DataParseStatus.NeedMoreData;
						}
					}

					if (headerValueBegin >= 0 && headerValueBegin > hederKeyEnd && headerValueEnd >= headerValueBegin)
					{
						if (isHeaderValueUTF8)
						{
							strHeaderValueTmp = Encoding.UTF8.GetString(buffer, headerValueBegin, headerValueEnd - headerValueBegin + 1);
						}
						else
						{
							strHeaderValueTmp = defaultEncoding.GetString(buffer, headerValueBegin, headerValueEnd - headerValueBegin + 1);
						}
					}
					strHeaderValue = (strHeaderValue != null) ? String.Concat(strHeaderValue, " ", strHeaderValueTmp) : strHeaderValueTmp;
					if (currIndex >= size || 
						finalCLRNCount != 1 || 
						buffer[currIndex] != 32 /*[space]*/ && buffer[currIndex] != 9)
					{
						break;
					}
					currIndex++;
				}

				if (headerKeyBegin >= 0 && hederKeyEnd >= headerKeyBegin)
				{
					if (isHeaderKeyUTF8)
					{
						strHeader = Encoding.UTF8.GetString(buffer, headerKeyBegin, hederKeyEnd - headerKeyBegin + 1);
					}
					else
					{
						strHeader = Encoding.ASCII.GetString(buffer, headerKeyBegin, hederKeyEnd - headerKeyBegin + 1);
					}
				}
				if (strHeader.Length > 0)
				{
					string strValue = DeocodeHeaderValue(strHeaderValue);
					base.Add(strHeader, strValue);

					if (strHeader == "Content-Type")
					{
						string strContentType;

						Hashtable parametrs;
						MimeEntry.ParseHeader(strValue, out strContentType, out parametrs);

						string strCharset = (string)parametrs["charset"];

						if (!string.IsNullOrEmpty(strCharset))
						{
							try
							{
								defaultEncoding = Encoding.GetEncoding(strCharset);
							}
							catch
							{
								defaultEncoding = Encoding.Default;
							}
						}
					}
				}
				unparsed = currIndex;
			}
			while (finalCLRNCount != 2);

			//this.IsReadOnly = true;

			return DataParseStatus.Done;
		}

		public static string DeocodeHeaderValue(string headerValue)
		{
			// Cool Encoding [2/13/2004]
			MatchCollection matchList = Regex.Matches(headerValue, @"(?<encodedword>=\?(?<charset>[^?]+)\?(?<encoding>[^?]+)\?(?<encodedtext>[^?]+)\?=)");

			if(matchList.Count>0)
			{
				// Fix: White space between adjacent 'encoded-word's is not displayed
				// [2007-07-03] TODO (=?ISO-8859-1?Q?a?= =?ISO-8859-1?Q?b?=)   ->  (ab)
				// [2007-07-03] TODO (=?ISO-8859-1?Q?a?=  =?ISO-8859-1?Q?b?=)   ->  (ab)
				// [2007-07-03] TODO (=?ISO-8859-1?Q?a?=\r\n  =?ISO-8859-1?Q?b?=)   ->  (ab)
				headerValue = Regex.Replace(headerValue, @"\?=[ ]+=\?", @"?==?");

				System.Text.StringBuilder  strBilder = new System.Text.StringBuilder(headerValue);

				foreach(Match matchItem in matchList)
				{
					string strEncodedWord = matchItem.Groups["encodedword"].Value;

					string strCharset = matchItem.Groups["charset"].Value;
					string strEncoding = matchItem.Groups["encoding"].Value;
					string strEncodedText = matchItem.Groups["encodedtext"].Value;

					
					if (matchItem.Groups["spaceceprator"].Success)
					{
						strBilder.Replace(strEncodedWord + matchItem.Groups["spaceceprator"].Value, strEncodedWord);
					}

					System.Text.Encoding encoder = System.Text.Encoding.GetEncoding(strCharset);

					string DecodedString = null;

					if(strEncoding.ToUpper() == "Q")
					{
						// FIX: The 8-bit hexadecimal value 20 (e.g., ISO-8859-1 SPACE) may be
						// represented as "_" (underscore, ASCII 95.).  (This character may
						// not pass through some internetwork mail gateways, but its use
						// will greatly enhance readability of "Q" encoded data with mail
						// readers that do not support this encoding.) 

						DecodedString = MimeEntry.QDecode(encoder,strEncodedText.Replace('_',' '));
					}
					else if(strEncoding.ToUpper() == "B")
					{
						int base64FixCount = 0;

						while(true)
						{
							try
							{
								byte[] FromBase64String = Convert.FromBase64String(strEncodedText);
								DecodedString = encoder.GetString(FromBase64String);
								break;
							}
							catch(System.FormatException)
							{
								// Remove not supported chars
								if (base64FixCount == 0)
									strEncodedText = Regex.Replace(strEncodedText, "[^a-zA-Z0-9+/=]+", string.Empty);
								else if (base64FixCount == 1)
									strEncodedText += "=";
								else
									strEncodedText = strEncodedText.Substring(0, strEncodedText.Length - 1);

								if (strEncodedText.Length == 0 || base64FixCount == 25) // Max 25 Attempts to fix chars
								{
									DecodedString = null;
									break;
								}

								base64FixCount++;
							}
						}
					}	
			
					if(DecodedString!=null)
						strBilder.Replace(strEncodedWord, DecodedString);
				}

				return strBilder.ToString();
			}

//			if(headerValue.StartsWith("=?"))
//			{
//				string[] parts = headerValue.Substring(2).Split(new char[]{'?'});
//				
//				string encoderName	= parts[0];
//				string type			= parts[1];
//				string datax		= parts[2];
//
//				System.Text.Encoding encoder = System.Text.Encoding.GetEncoding(encoderName);
//				if(type.ToUpper() == "Q")
//				{
//					return MimeEntry.QDecode(encoder,datax);
//				}
//				else if(type.ToUpper() == "B")
//				{
//					return encoder.GetString(Convert.FromBase64String(datax));
//				}				
//			}

			return headerValue;
		}
	}
}
