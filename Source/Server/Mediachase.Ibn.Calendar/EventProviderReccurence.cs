using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Calendar
{
    /// <summary>
    /// Represents: Calendar event virtual (recurrence) source provider.
    /// </summary>
    internal class EventProviderReccurence : IEventProvider
    {
        private static CalendarEventType _eventType = CalendarEventType.Recurring;
        
        private int _calendarId = -1;

        #region IEventProvider Members

        /// <summary>
        /// Gets the provider mask.
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
            return null;
           
        }

        /// <summary>
        /// Deletes the event.
        /// </summary>
        /// <remarks>Add except date in recurrence for this event</remarks>
        /// <param name="eventInfo"></param>
        public void DeleteEvent(ulong eventId)
        {
            if (EventProviderHelper.CheckEventBelong(GetEventType(), eventId))
            {
                DateTime recurDate;
                CalendarEvent calEvent;

                ulong eventKey = EventProviderHelper.GetEventKey(eventId);

                if (DecryptId(eventKey, out recurDate, out calEvent))
                {
                    CalendarEventRecurrence[] eventRecurrs = CalendarEventRecurrence.List(new FilterElement("EventId",
                                                                 FilterElementType.Equal, calEvent.PrimaryKeyId.Value));
                                        
                    foreach (CalendarEventRecurrence eventRecurr in eventRecurrs)
                    {
                        // Add exception date
                        if(recurDate <= eventRecurr.DtEnd && 
                           recurDate >= eventRecurr.DtStart)
                        {
                            eventRecurr.Exdate = eventRecurr.Exdate + ";" + recurDate.ToString();
                            eventRecurr.Save();
                            break;
                        }
                        
                    }
                    
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
                DateTime recurDate;
                CalendarEvent calEvent;

                ulong eventKey = EventProviderHelper.GetEventKey(eventId);

                if(DecryptId(eventKey ,out recurDate, out calEvent))
                {
                    retVal = MakeEventInfo(recurDate, calEvent);
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

            //querying non virtual events
            if (dtStart == DateTime.MinValue && dtEnd == DateTime.MaxValue)
                return retVal;

            CalendarEvent[] calEvents = CalendarEvent.List(new FilterElement("PrimaryCalendarId",
                                                     FilterElementType.Equal, _calendarId));

            foreach(CalendarEvent calEvent in calEvents)
            {
                CalendarEventRecurrence[] eventRecurrs = CalendarEventRecurrence.List(new FilterElement("EventId",
                                                                 FilterElementType.Equal, calEvent.PrimaryKeyId.Value));

                List<DateTime> eventRecurrResult = new List<DateTime>();

                foreach(CalendarEventRecurrence eventRecurr in eventRecurrs)
                {
                 
                    List<DateTime> recurrList = GetRecurrence(eventRecurr.Rrule, 
                                                              eventRecurr.Exrule, 
                                                              eventRecurr.Exdate,
                                                              calEvent.DtStart, dtEnd);

                    //skip recurrence equals beginning owner calendar event
                    recurrList.Remove(calEvent.DtStart);

                   eventRecurrResult = (JoinList(eventRecurrResult, recurrList, false));
                }

               //Begin create virtual events
                foreach(DateTime eventDate in eventRecurrResult)
                {
                    retVal.Add(MakeEventInfo(eventDate, calEvent));
                }
            }

            return retVal;
        }

        #endregion

        /// <summary>
        /// Makes the event info.
        /// </summary>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="calEvent">The cal event.</param>
        /// <returns></returns>
        private CalendarEventInfo MakeEventInfo(DateTime dtStart, CalendarEvent calEvent)
        {
            CalendarEventInfo retVal = new CalendarEventInfo();

            dtStart = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day,
                                   calEvent.DtStart.Hour, calEvent.DtStart.Minute,
                                   calEvent.DtStart.Second);

            ulong eventId = EncryptId(calEvent, dtStart);

            //TODO: Use Clone
            CalendarEvent virtualEvent = (CalendarEvent)calEvent.Clone();
          
            TimeSpan eventDuration = calEvent.DtEnd - calEvent.DtStart;
            virtualEvent.DtStart = dtStart;
            virtualEvent.DtEnd = virtualEvent.DtStart + eventDuration; 

            retVal.EventProvider = this;
            retVal.CalendarEvent = virtualEvent;

            retVal.SetEventId(EventProviderHelper.MakeEventId(GetEventType(), eventId));

            return retVal;
        }

        /// <summary>
        /// Hashes the function.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="eventKey">The event key.</param>
        /// <returns></returns>
        private ulong EncryptId(CalendarEvent calEvent, DateTime recurrDate)
        {
            TimeSpan offset = recurrDate - calEvent.DtStart;
            //Is mus be overflow !!!
            ulong passedDays = (ulong)offset.TotalDays;

			return ((passedDays << 32) | (ulong)(int)calEvent.PrimaryKeyId.Value);
        }

        /// <summary>
        /// Decrypts the id.
        /// </summary>
        /// <param name="ecryptId">The ecrypt id.</param>
        /// <returns></returns>
        private bool DecryptId(ulong encryptId, out DateTime recurrDate, 
                              out CalendarEvent calEvent)
        {
            int passedDays = (int)(encryptId >> 32);
            int eventKey = (int)(encryptId & int.MaxValue);
            try
            {
                calEvent = new CalendarEvent(eventKey);
                recurrDate = calEvent.DtStart.AddDays((Double)passedDays);
            }         
            catch(ObjectNotFoundException)
            {
                recurrDate = DateTime.MinValue;
                calEvent = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the recurrence.
        /// </summary>
        /// <param name="rRule">The r rule.</param>
        /// <param name="exRule">The ex rule.</param>
        /// <param name="exDate">The ex date.</param>
        /// <returns></returns>
        List<DateTime> GetRecurrence(String rRule, String exRule, String exDate, 
                                     DateTime dtStart, DateTime dtEnd)
        {
            List<DateTime> retVal = new List<DateTime>();
            if (!String.IsNullOrEmpty(rRule))
            {
                List<DateTime> recurr = ReccurenceEngine.GetReccurence(rRule, dtStart,
                                                                       dtEnd);
                List<DateTime> exDates = String2DateTimeList(exDate);
                if (!String.IsNullOrEmpty(exRule))
                {
                   exDates.AddRange(ReccurenceEngine.GetReccurence(exRule, dtStart, dtEnd));
                }
                //Exclude date from recurrence list
                retVal = JoinList(recurr, exDates, true);
            }

            return retVal;
            
        }
        /// <summary>
        /// Joins the list.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="mode">if set to <c>true</c> [mode].</param>
        /// <remarks> Left join </remarks>
        /// <returns></returns>
        List<DateTime> JoinList(List<DateTime> left, List<DateTime> right, 
                                    bool exclude)
        {
            List<DateTime> retVal = new List<DateTime>();
            List<DateTime> dummy;

            if(right.Count > left.Count)
            {
                dummy = left;
                left = right;
                right = dummy;
            }
            
            foreach(DateTime leftIter in left)
            {
                bool foundMatch = false; 
                foundMatch = exclude ? foundMatch : !foundMatch;

                foreach(DateTime rightIter in right)
                {
                    foundMatch = (rightIter == leftIter);
                    foundMatch = exclude ? !foundMatch : foundMatch;

                    if(foundMatch)
                        break;
                }

                if(foundMatch)
                   retVal.Add(leftIter);
            }

            return retVal;
        }
        /// <summary>
        /// String2s the date time list.
        /// </summary>
        /// <param name="dateTimeList">The date time list.</param>
        /// <returns></returns>
        private List<DateTime> String2DateTimeList(String dateTimeList)
        {
      
            List<DateTime> retVal = new List<DateTime>();

            if (String.IsNullOrEmpty(dateTimeList))
                return retVal;

            String[] dates = dateTimeList.Split(new Char[] { ';' });
            foreach(String date in dates)
            {
                try
                {
                    retVal.Add(DateTime.Parse(date));
                }
                catch(FormatException)
                {
                }
            }
            return retVal;
        }

        /// <summary>
        /// Events the delete handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Mediachase.Ibn.Calendar.CalendarEventArgs"/> instance containing the event data.</param>
        private void PrimaryEventDeleteHandler(object sender, CalendarEventArgs args)
        {
       
            CalendarEventRecurrence[] eventRecurrs = 
                            CalendarEventRecurrence.List(new FilterElement("EventId",
                                                    FilterElementType.Equal, 
                                                    args.CalEvent.PrimaryKeyId.Value));
                                                          
            foreach (CalendarEventRecurrence eventRecurr in eventRecurrs)
            {
                eventRecurr.Delete();
            }

        }
    }
}
