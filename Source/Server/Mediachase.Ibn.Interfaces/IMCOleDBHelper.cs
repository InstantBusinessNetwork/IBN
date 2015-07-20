using System;
using System.Data;

namespace Mediachase.Ibn.Service
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public interface IMCOleDBHelper
	{
		DataSet ConvertExcelToDataSet(string fileName);
	}
}
