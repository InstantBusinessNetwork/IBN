using System;
using System.Runtime.Serialization;

namespace Update
{
	[Serializable]
	public class UpdateException : Exception
	{
		public UpdateException()
			: base()
		{
		}

		public UpdateException(string message)
			: base(message)
		{
		}

		public UpdateException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected UpdateException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
