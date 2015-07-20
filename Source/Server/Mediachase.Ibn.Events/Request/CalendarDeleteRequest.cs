using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Events.Request
{
	public class CalendarDeleteRequest : CalendarRequest
	{
		
		public CalendarDeleteRequest()
			: base(RequestMethod.Delete, null)
		{
		}


		public CalendarDeleteRequest(EntityObject target)
			: base(RequestMethod.Delete, target)
		{
		}

		public CalendarDeleteRequest(EntityObject target, string recurrenceId)
			: base(RequestMethod.Delete, target, recurrenceId)
		{
			RecurrenceId = recurrenceId;
		}

	}
}
