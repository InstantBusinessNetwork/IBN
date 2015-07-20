using System;
using System.Data;
using System.IO;

namespace Mediachase.MetaDataPlus.Import.Parser
{
	/// <summary>
	/// Summary description for XmlIncomingDataParser.
	/// </summary>
	public class XmlIncomingDataParser : IIncomingDataParser
	{
		public string Name
		{
			get
			{
				return "XmlIncomingDataParser";
			}
		}

		public string Description
		{
			get
			{
				return "Mediachase.MetaDataPlus.Import.Parser.XmlIncomingDataParser";
			}
		}

		public XmlIncomingDataParser()
		{
		}

		public virtual DataSet Parse(string fileName, Stream stream)
		{
			DataSet ds = new DataSet();

			if (stream != null)
			{
				ds.ReadXml(stream);
			}
			else ds.ReadXml(fileName);

			return ds;
		}

		public bool CanParse(string fileName, Stream stream)
		{
			try
			{
				Parse(fileName, stream);

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
