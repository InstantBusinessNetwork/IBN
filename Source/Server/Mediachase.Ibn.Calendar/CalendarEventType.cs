using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Calendar
{
    /// <summary>
    /// Represents: Calendar event types.
    /// </summary>
    public enum CalendarEventType
    {
        /// <summary>
        /// Undefined type
        /// </summary>
        Undef = 0x0,
        /// <summary>
        /// Is primary event in calendar
        /// </summary>
        Primary,
        /// <summary>
        /// is event contain in calendar as link
        /// </summary>
        Link,
        /// <summary>
        /// is virtual event (recurring)
        /// </summary>
        Recurring
    }
}
