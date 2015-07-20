using System;
using System.Runtime.Serialization;

namespace Mediachase.Ibn.Converter
{
	[Serializable]
	public class MetadataConverterException : Exception
	{
		public MetadataConverterException()
			: base()
		{
		}

		public MetadataConverterException(string message)
			: base(message)
		{
		}

		public MetadataConverterException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected MetadataConverterException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
