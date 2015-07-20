using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Calendar
{
    /// <summary>
    /// Represents: Mediator pattern implementation, translate all command to 
    /// each provider.
    /// </summary>
    internal class EventProviderMediator : IEventProvider
    {
        private static CalendarEventType _eventType = CalendarEventType.Undef;
        private List<IEventProvider> _eventProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProviderDeMultiplecsor"/> class.
        /// </summary>
        internal EventProviderMediator()
        {
            _eventProviders = new List<IEventProvider>();
            //Register event providers
            _eventProviders.Add(new EventProviderPrimary());
            _eventProviders.Add(new EventProviderLink());
            _eventProviders.Add(new EventProviderReccurence());

        }

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
        /// Initializes the specified calendar.
        /// </summary>
        /// <param name="calendar">The calendar.</param>
        public void Initialize(MetaObject obj)
        {
            Calendar calendar = obj as Calendar;

            if (obj == null)
                throw new ArgumentException("calendar");

            foreach (IEventProvider provider in _eventProviders)
            {
                provider.Initialize(calendar);
            }
        }

        /// <summary>
        /// Creates the event.
        /// </summary>
        /// <returns></returns>
        public CalendarEventInfo CreateEvent(ulong eventId)
        {
            CalendarEventInfo retVal = null;

            foreach (IEventProvider provider in _eventProviders)
            {
                retVal = provider.CreateEvent(eventId);
                if (retVal != null)
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Deletes the event.
        /// </summary>
        /// <param name="eventInfo"></param>
        public void DeleteEvent(ulong eventId)
        {
            foreach (IEventProvider provider in _eventProviders)
            {
                provider.DeleteEvent(eventId);
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

            foreach (IEventProvider provider in _eventProviders)
            {
                retVal = provider.GetEventInfo(eventId);
                if (retVal != null)
                    break;
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

            foreach (IEventProvider provider in _eventProviders)
            {
                retVal.AddRange(provider.GetEventInfoCollection(dtStart, dtEnd));
            }

            return retVal;
        }

        #endregion
    }
}
