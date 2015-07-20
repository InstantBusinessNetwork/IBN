using System;
using System.Runtime.Serialization;

namespace Mediachase.Ibn.Converter
{
	[Serializable]
	public class ClientsConverterException : Exception
	{
		public ClientsConverterException()
			: base()
		{
		}

		public ClientsConverterException(string message)
			: base(message)
		{
		}

		public ClientsConverterException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected ClientsConverterException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
