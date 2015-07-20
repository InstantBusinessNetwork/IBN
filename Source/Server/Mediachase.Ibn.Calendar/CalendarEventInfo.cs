using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Calendar
{
    /// <summary>
    /// Represents: Calendar event wrapper. Extent calendar event property.
    /// </summary>
    public class CalendarEventInfo
    {
        private IEventProvider _eventProvider;
        private CalendarEvent _event;
        private ulong _eventId = 0;

        internal CalendarEventInfo()
        {
        }
        internal CalendarEventInfo(IEventProvider eventProvider, CalendarEvent calEvent)
        {
            EventProvider = eventProvider;
            CalendarEvent = calEvent;
         }

        /// <summary>
        /// Sets the event id.
        /// </summary>
        /// <param name="eventId">The event id.</param>
        internal void SetEventId(ulong eventId)
        {
            _eventId = eventId;
        }

        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        /// <value>The type of the event.</value>
        public CalendarEventType EventType
        {
            get
            {
                return _eventProvider.GetEventType();
            }
        }
        /// <summary>
        /// Gets the event id.
        /// </summary>
        /// <value>The event id.</value>
        public ulong EventId
        {
             get 
             {
                 if(_eventId == 0)
                     throw new ArgumentException("eventId");
                 
                 return _eventId;
                 
             }
        }
	
        /// <summary>
        /// Gets the calendar event.
        /// </summary>
        /// <value>The calendar event.</value>
        public CalendarEvent CalendarEvent
        {
            get { return _event; }

            set 
             { 
                  _event = value;
             }
        }

        /// <summary>
        /// Gets the event provider.
        /// </summary>
        /// <value>The event provider.</value>
        internal IEventProvider EventProvider
        {
            get { return _eventProvider; }
            set { _eventProvider = value; }
        }
    }
}
