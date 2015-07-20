using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Events.Request
{
	public class CalendarUpdateRequest : CalendarRequest
	{

		public CalendarUpdateRequest()
			: base(RequestMethod.Update, null)
		{
		}

		public CalendarUpdateRequest(EntityObject target)
			: base(RequestMethod.Update, target)
		{
		}

		public CalendarUpdateRequest(EntityObject target, string recurrenceId)
			: base(RequestMethod.Update, target, recurrenceId)
		{
		}
	}
}
