using System;
using System.Data;
using System.IO;

namespace Mediachase.MetaDataPlus.Import
{
	/// <summary>
	/// Summary description for IIncomingDataParser.
	/// </summary>
	public interface IIncomingDataParser
	{
		string Name { get; }
		string Description { get; }

		DataSet Parse(string fileName, Stream stream);

		bool CanParse(string fileName, Stream stream);
	}
}
