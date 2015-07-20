using System;
using System.Collections;
using System.Collections.Specialized;

namespace Mediachase.IBN.Business.EMail
{
	internal delegate string[] HeaderParser(string value);
	/// <summary>
	/// 
	/// </summary>
	class HeaderInfo
	{
		private bool _restricted;
		private HeaderParser _parser;
		private string _name;
		private bool _multivalue;

		internal HeaderParser Parser
		{
			get { return _parser; }
		}

		internal string Name
		{
			get { return _name; }
		}

		internal bool Multivalue
		{
			get { return _multivalue; }
		}

		internal HeaderInfo(string name, bool restricted, bool multivalue, HeaderParser parser)
		{
			_name = name;
			_restricted = restricted;
			_parser = parser;
			_multivalue = multivalue;
		}
	}

	/// <summary>
	/// Summary description for HeaderInfoTable.
	/// </summary>
	class HeaderInfoTable
	{
		private static Hashtable HeaderHashTable;
		private static HeaderParser SingleValueParser = new HeaderParser(ParseSingleValue);
		private static HeaderParser MultivalueParser = new HeaderParser(ParseMultivalue);
		private static HeaderInfo UnknownHeaderInfo = new HeaderInfo(String.Empty, false, false, SingleValueParser);
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
			return new string[] { value };
		}

		private static string[] ParseMultivalue(string value)
		{
			StringCollection stringCollection = new StringCollection();
			bool flag = false;
			int i = 0;
			char[] chs = new char[(uint)value.Length];
			for (int j = 0; j < value.Length; j++)
			{
				if (value[j] == '\"')
				{
					flag = (flag == false);
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
				new HeaderInfo("Return-path", true, false, SingleValueParser), 
				new HeaderInfo("Received", true, false, SingleValueParser), 
				//; original mail
				new HeaderInfo("From", false, true, MultivalueParser), 
				new HeaderInfo("Sender", false, false, SingleValueParser), 
				new HeaderInfo("Reply-To", false, true, MultivalueParser), 
				//; forwarded
				new HeaderInfo("Resent-Sender", false, false, SingleValueParser), 
				new HeaderInfo("Resent-From", false, true, MultivalueParser), 
				new HeaderInfo("Resent-Reply-To", false, true, MultivalueParser), 
				// ; Creation time [1/23/2004]
				// ; Original [1/23/2004]
				new HeaderInfo("Date", false, false, SingleValueParser),
				new HeaderInfo("Resent-Date", false, false, SingleValueParser),
				// ;  address required [1/23/2004]
				new HeaderInfo("To", false, true, MultivalueParser),
				new HeaderInfo("Resent-To", false, false, SingleValueParser),
				new HeaderInfo("cc", false, false, SingleValueParser),
				new HeaderInfo("Resent-cc", false, false, SingleValueParser),
				new HeaderInfo("bcc", false, false, SingleValueParser),
				new HeaderInfo("Resent-bcc", false, false, SingleValueParser),
				//;  others optional
				new HeaderInfo("Message-ID", false, false, SingleValueParser),
				new HeaderInfo("Resent-Message-ID", false, false, SingleValueParser),
				new HeaderInfo("In-Reply-To", false, false, SingleValueParser),
				new HeaderInfo("References", false, false, SingleValueParser),
				new HeaderInfo("Keywords", false, false, SingleValueParser),
				new HeaderInfo("Subject", false, false, SingleValueParser),
				new HeaderInfo("Comments", false, false, SingleValueParser),
				new HeaderInfo("Encrypted", false, false, SingleValueParser)
			};

			HeaderHashTable = new Hashtable((int)headerInfos1.Length * 2);
			for (int i = 0; i < (int)headerInfos1.Length; i++)
			{
				HeaderHashTable[headerInfos1[i].Name] = headerInfos1[i];
			}
			headerInfos1 = null;
			return true;
		}
	}
}
