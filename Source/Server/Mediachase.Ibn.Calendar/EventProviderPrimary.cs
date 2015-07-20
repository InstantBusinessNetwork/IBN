using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Calendar
{
    /// <summary>
    /// Represents: Calendar event primary source provider.
    /// </summary>
    internal class EventProviderPrimary : IEventProvider
    {
        private static CalendarEventType _eventType = CalendarEventType.Primary;
        private Calendar _calendar;

        #region IEventProvider Members
        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        /// <returns></returns>
        public CalendarEventType GetEventType()
        {
            return _eventType;
        }
        /// <summary>
        /// Initializes the specified event storage.
        /// </summary>
        /// <param name="eventStorage">The event storage.</param>
        public void Initialize(MetaObject eventStorage)
        {
            _calendar = eventStorage as Calendar;

            if (_calendar == null)
                throw new ArgumentException("event storage");
        }

        /// <summary>
        /// Creates the event.
        /// </summary>
        /// <param name="eventInfo"></param>
        /// <returns></returns>
        public CalendarEventInfo CreateEvent(ulong eventId)
        {
            //Is create primary event only
            if (eventId != 0)
                return null;

            CalendarEventInfo retVal = null;

            if (_calendar != null)
            {
                retVal = new CalendarEventInfo();
                CalendarEvent calEvent = new CalendarEvent();

                calEvent.PrimaryCalendarId = _calendar.PrimaryKeyId.Value;

                //Raise event
                CalendarEvent.RaiseCreateEvent(calEvent);

                calEvent.Save();

                retVal.CalendarEvent = calEvent;
                retVal.EventProvider = this;
                retVal.SetEventId(EventProviderHelper.MakeEventId(GetEventType(), 
                                                                  (ulong)(int)calEvent.PrimaryKeyId.Value));
            }

            return retVal;
        }

        /// <summary>
        /// Deletes the event.
        /// </summary>
        /// <param name="eventId">The event id.</param>
        public void DeleteEvent(ulong eventId)
        {
            if (EventProviderHelper.CheckEventBelong(GetEventType(), eventId))
            {               
                if (_calendar == null)
                    throw new NullReferenceException("calendar");
                try
                {
                    int eventKey = (int)EventProviderHelper.GetEventKey(eventId);
                    CalendarEvent calEvent = new CalendarEvent(eventKey);

                    //Raise event
                    CalendarEvent.RaiseDeleteEvent(calEvent);
                    calEvent.Delete();
                }
                catch (ObjectNotFoundException)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets the event info.
        /// </summary>
        /// <param name="eventId">The event id.</param>
        /// <returns></returns>
        public CalendarEventInfo GetEventInfo(ulong eventId)
        {
            CalendarEventInfo retVal = null;

            if(EventProviderHelper.CheckEventBelong(GetEventType(), eventId))
            {
                int eventKey = (int)EventProviderHelper.GetEventKey(eventId);
                try
                {
                    CalendarEvent calEvent = new CalendarEvent(eventKey);
                    retVal = new CalendarEventInfo(this, calEvent);
                    retVal.SetEventId(eventId);
                }
                catch (ObjectNotFoundException)
                {
                    throw;
                }

            }

            return retVal;
        }

        /// <summary>
        /// Gets the event info collection.
        /// </summary>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns></returns>
        public EventInfoCollections GetEventInfoCollection(DateTime dtStart, DateTime dtEnd)
        {
            EventInfoCollections retVal = new EventInfoCollections();

            if (_calendar == null)
                throw new NullReferenceException("calendar");

            CalendarEvent[] calEvents = CalendarEvent.List(new FilterElement("PrimaryCalendarId",
                                                                 FilterElementType.Equal, _calendar.PrimaryKeyId.Value));
            foreach(CalendarEvent calEvent in calEvents)
            {
                if ((calEvent.DtStart >= dtStart) && (calEvent.DtEnd <= dtEnd))
                {
                    CalendarEventInfo eventInfo = new CalendarEventInfo(this, calEvent);
                    eventInfo.SetEventId(EventProviderHelper.MakeEventId(GetEventType(),
                                                                        (ulong)(int)calEvent.PrimaryKeyId.Value));
                    retVal.Add(eventInfo);
                }
            }

            return retVal;
        }

        #endregion
    }
}
