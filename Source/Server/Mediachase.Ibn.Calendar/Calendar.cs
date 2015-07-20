using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Calendar
{
    /// <summary>
    /// Represents: Calendar instance. Implement event management functional.
    /// </summary>
    partial class Calendar
    {
        private EventProviderMediator _providers;
     
        /// <summary>
        /// Adds the event.
        /// </summary>
        public CalendarEventInfo AddEvent()
        {
             return AddEvent(0);
        
        }
        /// <summary>
        /// Adds the event.
        /// </summary>
        public CalendarEventInfo AddEvent(ulong eventId)
        {
            CheckProviderInitialize();
            return _providers.CreateEvent(eventId);
        }

        /// <summary>
        /// Adds the event.
        /// </summary>
        /// <param name="eventInfoCollection">The event info collection.</param>
        /// <returns></returns>
        public EventInfoCollections AddEvent(EventInfoCollections eventInfoCollection)
        {
            EventInfoCollections retVal = new EventInfoCollections();

            foreach(CalendarEventInfo eventInfo in eventInfoCollection)
            {
              retVal.Add( AddEvent(eventInfo.EventId) );
            }

            return retVal;
        }

        /// <summary>
        /// Gets the event info.
        /// </summary>
        /// <param name="eventId">The event id.</param>
        /// <returns></returns>
        public CalendarEventInfo GetEvent(ulong eventId)
        {
            CheckProviderInitialize();
            CalendarEventInfo retVal = new CalendarEventInfo();
            retVal = _providers.GetEventInfo(eventId);

            return retVal;

        }

        /// <summary>
        /// Gets the event info.
        /// </summary>
        /// <param name="dtStart">The dtStart.</param>
        /// <param name="dtEnd">The dtEnd.</param>
        /// <returns></returns>
        public EventInfoCollections GetEventList(DateTime dtStart, DateTime dtEnd)
        {
           CheckProviderInitialize();
           EventInfoCollections retVal =  
                    _providers.GetEventInfoCollection(dtStart, dtEnd);
           return retVal;

        }

        /// <summary>
        /// Removes the event.
        /// </summary>
        /// <param name="eventId">The event id.</param>
        public void DeleteEvent(ulong eventId)
        {
           _providers.DeleteEvent(eventId);
        }


        /// <summary>
        /// Removes the event.
        /// </summary>
        /// <param name="dtStart">The dtStart.</param>
        /// <param name="dtEnd">The dtEnd.</param>
        public void DeleteEvent(DateTime dtStart, DateTime dtEnd)
        {
            EventInfoCollections eventInfos = GetEventList(dtStart, dtEnd);
            DeleteEvent(eventInfos);
        }

        /// <summary>
        /// Removes the event.
        /// </summary>
        /// <param name="eventInfoList">The event info list.</param>
        public void DeleteEvent(EventInfoCollections eventInfoList)
        {
            CheckProviderInitialize();
            if (eventInfoList != null)
            {
                foreach (CalendarEventInfo eventInfo in eventInfoList)
                {
                    _providers.DeleteEvent(eventInfo.EventId);
                }
            }
        }

        /// <summary>
        /// Checks the provider initialize.
        /// </summary>
        private void CheckProviderInitialize()
        {
            if(_providers == null)
            {
                _providers = new EventProviderMediator();
                _providers.Initialize(this);
            }
        }

        /// <summary>
        /// Called after a delete operation.
        /// </summary>
        protected override void  OnDeleting()
        {
           EventInfoCollections events =  GetEventList(DateTime.MinValue, DateTime.MaxValue);

           DeleteEvent(events);

           base.OnDeleting();
        } 
      
    }
}
