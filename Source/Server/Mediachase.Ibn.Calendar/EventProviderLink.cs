using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Calendar
{
    /// <summary>
    /// Represents: Calendar event link source provider.
    /// </summary>
    internal class EventProviderLink : IEventProvider
    {
        private static CalendarEventType _eventType = CalendarEventType.Link;
      
        private int _calendarId = -1;

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
            Calendar calendar = eventStorage as Calendar;

            if (calendar == null)
                throw new ArgumentException("event storage");

            //Subscribe on delete primary event, for cleanup link
            CalendarEvent.CalEventDeleteEvent += PrimaryEventDeleteHandler;
            _calendarId = calendar.PrimaryKeyId.Value;

        }

        /// <summary>
        /// Creates the event.
        /// </summary>
        /// <param name="eventInfo"></param>
        /// <returns></returns>
        public CalendarEventInfo CreateEvent(ulong eventId)
        {
            CalendarEventInfo retVal = null;

            //Is create link only
            if (eventId == 0)
                return retVal;

            if (_calendarId != 0)
            {                    
                 try
                {
                    int eventKey = (int)EventProviderHelper.GetEventKey(eventId);

                    CalendarEvent calEvent = new CalendarEvent(eventKey);

                    retVal = new CalendarEventInfo(this, calEvent);
                   
                    CalendarEventLink eventLink = new CalendarEventLink();
                    eventLink.CalendarId = _calendarId;
                    eventLink.EventId = (int)EventProviderHelper.GetEventKey(eventId);
                    eventLink.Save();

                    //Set event key
                    retVal.SetEventId(EventProviderHelper.MakeEventId(GetEventType(),
																	  (ulong)(int)eventLink.PrimaryKeyId.Value));

                }
                catch(ObjectNotFoundException)
                {

                }
                                
            }
       
           return retVal;
        }

        /// <summary>
        /// Deletes the event.
        /// </summary>
        /// <param name="eventInfo"></param>
        public void DeleteEvent(ulong eventId)
        {
            if (EventProviderHelper.CheckEventBelong(GetEventType(), eventId))
            {
                if (_calendarId == -1)
                    throw new NullReferenceException("calendarId");
               
                try
                {
                    int linkKey = (int)EventProviderHelper.GetEventKey(eventId);

                    CalendarEventLink eventLink = new CalendarEventLink(linkKey);

                    eventLink.Delete();

                }
                catch(ObjectNotFoundException)
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

            if (EventProviderHelper.CheckEventBelong(GetEventType(), eventId))
            {
                try
                {
                    int linkKey = (int)EventProviderHelper.GetEventKey(eventId);

                    CalendarEventLink eventLink = new CalendarEventLink(linkKey);
                    CalendarEvent calEvent = new CalendarEvent(eventLink.EventId);

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
        public EventInfoCollections GetEventInfoCollection(DateTime dtStart, 
                                                           DateTime dtEnd)
        {
            EventInfoCollections retVal = new EventInfoCollections();

            if (_calendarId == -1)
                throw new NullReferenceException("calendarId");

            CalendarEventLink[] eventLinks = CalendarEventLink.List(new FilterElement("CalendarId",
                                                          FilterElementType.Equal, _calendarId));

            foreach (CalendarEventLink eventLink in eventLinks)
            {
                try
                {
                    CalendarEvent calEvent = new CalendarEvent(eventLink.EventId);

                    if ((calEvent.DtStart >= dtStart) && (calEvent.DtEnd <= dtEnd))
                    {
                        CalendarEventInfo eventInfo = new CalendarEventInfo(this, calEvent);
                        eventInfo.SetEventId(EventProviderHelper.MakeEventId(GetEventType(),
																			 (ulong)(int)eventLink.PrimaryKeyId.Value));
                        retVal.Add(eventInfo);
                    }
                }
                catch(ObjectNotFoundException)
                {
                    throw;
                }
              
            }

            return retVal;
        }

        #endregion

        /// <summary>
        /// Events the delete handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Mediachase.Ibn.Calendar.CalendarEventArgs"/> instance containing the event data.</param>
        private void PrimaryEventDeleteHandler(object sender, CalendarEventArgs args)
        {
            CalendarEventLink[] eventLinks = CalendarEventLink.List(new FilterElement("EventId", FilterElementType.Equal, 
                                                                            args.CalEvent.PrimaryKeyId.Value));
            foreach(CalendarEventLink eventLink in eventLinks)
            {
                eventLink.Delete();
            }
           
        }
      
    }
}
