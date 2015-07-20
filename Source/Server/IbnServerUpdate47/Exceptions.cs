using System;
using System.Runtime.Serialization;

namespace IbnServerUpdate
{
	[Serializable]
	public class IbnUpdateException : Exception
	{
		public IbnUpdateException()
			: base()
		{
		}

		public IbnUpdateException(string message)
			: base(message)
		{
		}

		public IbnUpdateException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected IbnUpdateException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
