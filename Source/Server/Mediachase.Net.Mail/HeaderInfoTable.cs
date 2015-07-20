using System;
using System.Collections;
using System.Collections.Specialized;

namespace Mediachase.Net.Mail
{
	internal delegate string[] HeaderParser(string value);
	/// <summary>
	/// 
	/// </summary>
	internal class HeaderInfo
	{
		internal bool IsRestricted;

		internal HeaderParser Parser;

		internal string HeaderName;

		internal bool AllowMultiValues;

		internal HeaderInfo(string name, bool restricted, bool multi, HeaderParser p)
		{
			HeaderName = name;
			IsRestricted = restricted;
			Parser = p;
			AllowMultiValues = multi;
		}
	}

	/// <summary>
	/// Summary description for HeaderInfoTable.
	/// </summary>
	internal class HeaderInfoTable
	{
		private static Hashtable HeaderHashTable;

		private static HeaderParser SingleParser = new HeaderParser(ParseSingleValue);

		private static HeaderParser MultiParser = new HeaderParser(ParseMultiValue);

		private static HeaderInfo UnknownHeaderInfo = new HeaderInfo(String.Empty, false, false, SingleParser);

		private static bool m_Initialized = Initialize();


		internal HeaderInfo this[string name]
		{
			get
			{
				HeaderInfo headerInfo = (HeaderInfo)HeaderHashTable[name];
				if (headerInfo == null)
				{
					return UnknownHeaderInfo;
				}
				else
				{
					return headerInfo;
				}
			}
		}

		private static string[] ParseSingleValue(string value)
		{
			return new string[]{value};
		}

		private static string[] ParseMultiValue(string value)
		{
			StringCollection stringCollection = new StringCollection();
			bool flag = false;
			int i = 0;
			char[] chs = new char[(uint)value.Length];
			for (int j = 0; j < value.Length; j++)
			{
				if (value[j] == '\"')
				{
					flag = (flag== false);
					continue;
				}

				if (value[j] == ',' && !flag)
				{
					string str = new String(chs, 0, i);
					stringCollection.Add(str.Trim());
					i = 0;
				}
				else
				{
					chs[i++] = value[j];
				}
			}
			if (i != 0)
			{
				string str = new String(chs, 0, i);
				stringCollection.Add(str.Trim());
			}
			string[] strs = new string[(uint)stringCollection.Count];
			stringCollection.CopyTo(strs, 0);
			return strs;
		}

		private static bool Initialize()
		{
			HeaderInfo[] headerInfos1 = new HeaderInfo[]
			{	
				// Pop3 Headers
				//;  author id & one
				// ; net traversals
				new HeaderInfo("Return-path", true, false, SingleParser), 
				new HeaderInfo("Received", true, false, SingleParser), 
				//; original mail
				new HeaderInfo("From", false, true, MultiParser), 
				new HeaderInfo("Sender", false, false, SingleParser), 
				new HeaderInfo("Reply-To", false, true, MultiParser), 
				//; forwarded
				new HeaderInfo("Resent-Sender", false, false, SingleParser), 
				new HeaderInfo("Resent-From", false, true, MultiParser), 
				new HeaderInfo("Resent-Reply-To", false, true, MultiParser), 
				// ; Creation time [1/23/2004]
				// ; Original [1/23/2004]
				new HeaderInfo("Date", false, false, SingleParser),
				new HeaderInfo("Resent-Date", false, false, SingleParser),
				// ;  address required [1/23/2004]
				new HeaderInfo("To", false, true, MultiParser),
				new HeaderInfo("Resent-To", false, false, SingleParser),
				new HeaderInfo("cc", false, false, SingleParser),
				new HeaderInfo("Resent-cc", false, false, SingleParser),
				new HeaderInfo("bcc", false, false, SingleParser),
				new HeaderInfo("Resent-bcc", false, false, SingleParser),
				//;  others optional
				new HeaderInfo("Message-ID", false, false, SingleParser),
				new HeaderInfo("Resent-Message-ID", false, false, SingleParser),
				new HeaderInfo("In-Reply-To", false, false, SingleParser),
				new HeaderInfo("References", false, false, SingleParser),
				new HeaderInfo("Keywords", false, false, SingleParser),
				new HeaderInfo("Subject", false, false, SingleParser),
				new HeaderInfo("Comments", false, false, SingleParser),
				new HeaderInfo("Encrypted", false, false, SingleParser)
			};

			HeaderHashTable = new Hashtable((int)headerInfos1.Length * 2);
			for (int i = 0; i < (int)headerInfos1.Length; i++)
			{
				HeaderHashTable[headerInfos1[i].HeaderName] = headerInfos1[i];
			}
			headerInfos1 = null;
			return true;
		}
	}
}
