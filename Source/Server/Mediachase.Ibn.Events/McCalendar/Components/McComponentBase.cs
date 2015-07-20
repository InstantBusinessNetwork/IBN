using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.iCal.Components;

namespace Mediachase.Ibn.Events.McCalendar.Components
{
	public class McComponentBase : ComponentBase
	{
		#region Constructors

		public McComponentBase() : base() { }
		public McComponentBase(iCalObject parent) : base(parent) { }
		public McComponentBase(iCalObject parent, string name) : base(parent, name) { }

		#endregion

		#region Static Public Methods

		static new public ComponentBase Create(iCalObject parent, string name)
		{
			switch (name.ToUpper())
			{
				//case ALARM: return new Alarm(parent);
				case EVENT: return new McEvent(parent);
				//case FREEBUSY: return new FreeBusy(parent);
				//case JOURNAL: return new Journal(parent);
				//case TIMEZONE: return new iCalTimeZone(parent);
				//case TODO: return new Todo(parent);
				case DAYLIGHT:
				case STANDARD:
					return new TimeZoneInfo(name.ToUpper(), parent);
				default: return new McComponentBase(parent, name);
			}
		}
		#endregion

	}
}
