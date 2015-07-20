using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Calendar
{
    public class CalendarEventArgs
    {
        private CalendarEvent _event;
        
        public CalendarEvent CalEvent
        {
            get { return _event; }
            set { _event = value; }
        }
	
    }
}
