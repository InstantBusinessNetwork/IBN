using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace IbnServer
{
	[Serializable]
	public class IbnSetupException : Exception
	{
		public IbnSetupException()
			: base()
		{
		}

		public IbnSetupException(string message)
			: base(message)
		{
		}

		public IbnSetupException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected IbnSetupException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
