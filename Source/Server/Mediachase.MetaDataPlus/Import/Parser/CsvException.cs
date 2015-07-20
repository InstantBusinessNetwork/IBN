using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Mediachase.MetaDataPlus.Import.Parser
{
	[Serializable]
	public class CsvException : Exception
	{
		public CsvException()
			: base()
		{
		}

		public CsvException(string message)
			: base(message)
		{
		}

		public CsvException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected CsvException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
