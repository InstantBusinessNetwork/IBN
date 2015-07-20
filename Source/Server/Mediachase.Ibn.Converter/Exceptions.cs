using System;
using System.Runtime.Serialization;

namespace Mediachase.Ibn.Converter
{
	[Serializable]
	public class IbnConverterException : Exception
	{
		public IbnConverterException()
			: base()
		{
		}

		public IbnConverterException(string message)
			: base(message)
		{
		}

		public IbnConverterException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected IbnConverterException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
