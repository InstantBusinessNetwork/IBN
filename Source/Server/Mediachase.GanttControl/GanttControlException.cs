using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Mediachase.Gantt
{
	[Serializable]
	public class GanttControlException : Exception
	{
		public GanttControlException()
			: base()
		{
		}

		public GanttControlException(string message)
			: base(message)
		{
		}

		public GanttControlException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected GanttControlException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
