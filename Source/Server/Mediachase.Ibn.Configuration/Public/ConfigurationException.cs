using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Mediachase.Ibn.Configuration
{
	[Serializable]
	public class ConfigurationException : Exception
	{
		public ConfigurationException()
			: base()
		{
		}

		public ConfigurationException(string message)
			: base(message)
		{
		}

		public ConfigurationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected ConfigurationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
