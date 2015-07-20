using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Calendar
{
    /// <summary>
    /// Represents: Calendar wrapper. Extend standard calendar functionality.
    /// </summary>
    public class CalendarInfo
    {
        private readonly Calendar _calendar;
        private readonly CalendarFolderLink _calendarLink;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarInfo"/> class.
        /// </summary>
        /// <param name="calendar">The calendar.</param>
        /// <param name="link">The link.</param>
        public CalendarInfo(Calendar calendar, CalendarFolderLink link)
        {
            _calendarLink = link;
            _calendar = calendar;

        }

        /// <summary>
        /// Determines whether this instance is link.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is link; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLink()
        {
            return _calendarLink != null;
        }

        /// <summary>
        /// Gets the calendar.
        /// </summary>
        /// <value>The calendar.</value>
        public Calendar Calendar
        {
            get { return _calendar; }
            
        }

        /// <summary>
        /// Gets the calendar link.
        /// </summary>
        /// <value>The calendar link.</value>
        public CalendarFolderLink CalendarLink
        {
            get { return _calendarLink; }
            
        }
	
	
    }
}
