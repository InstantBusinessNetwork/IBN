using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Mediachase.Schedule.Service
{
	[Serializable]
	public class ScheduleServiceException : Exception
	{
		public ScheduleServiceException()
			: base()
		{
		}

		public ScheduleServiceException(string message)
			: base(message)
		{
		}

		public ScheduleServiceException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected ScheduleServiceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
