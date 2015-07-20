using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Mediachase.Ibn
{
	[Serializable]
	public class IMHelperException : Exception
	{
		public IMHelperException()
			: base()
		{
		}

		public IMHelperException(string message)
			: base(message)
		{
		}

		public IMHelperException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected IMHelperException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
