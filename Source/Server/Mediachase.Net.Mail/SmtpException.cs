using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Mediachase.Net.Mail
{
	[Serializable]
	public class SmtpException : Exception
	{
		public SmtpException()
			: base()
		{
		}

		public SmtpException(string message)
			: base(message)
		{
		}

		public SmtpException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected SmtpException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
