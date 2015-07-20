using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Events.Request
{
	public abstract class CalendarRequest : Mediachase.Ibn.Core.Business.Request
	{
		public const string RECURRENCEID_PROP = "RecurrenceId";


		public CalendarRequest(string method, EntityObject target)
			: base(method, target)
		{
		}

		public CalendarRequest(string method, EntityObject target, string recurrenceId)
			: this(method, target)
		{
			RecurrenceId = recurrenceId;
		}

		public string RecurrenceId
		{
			get
			{
				return base.Parameters.GetValue<String>(RECURRENCEID_PROP, null);

			}

			set
			{
				base.Parameters.Add(RECURRENCEID_PROP, value);
			}
		}
	}
}
