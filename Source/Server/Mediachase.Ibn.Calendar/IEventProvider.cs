using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Calendar
{
    /// <summary>
    /// Represents: Calendar event provider interface.
    /// </summary>
    public interface IEventProvider
    {
        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        /// <returns></returns>
        CalendarEventType GetEventType();

        /// <summary>
        /// Initializes the specified event storage.
        /// </summary>
        /// <param name="eventStorage">The event storage.</param>
        void Initialize(MetaObject eventStorage);
        /// <summary>
        /// Creates the event.
        /// </summary>
        /// <returns></returns>
        CalendarEventInfo CreateEvent(ulong eventInfo);

        /// <summary>
        /// Deletes the event.
        /// </summary>
        /// <param name="eventId">The event id.</param>
        void DeleteEvent(ulong eventId);

        /// <summary>
        /// Gets the event info.
        /// </summary>
        /// <param name="eventId">The event id.</param>
        /// <returns></returns>
        CalendarEventInfo GetEventInfo(ulong eventId);

        /// <summary>
        /// Gets the event info collection.
        /// </summary>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns></returns>
        EventInfoCollections GetEventInfoCollection(DateTime dtStart, DateTime dtEnd);

    }
}
