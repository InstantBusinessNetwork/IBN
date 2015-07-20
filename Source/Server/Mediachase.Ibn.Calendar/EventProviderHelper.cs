using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Calendar
{
    /// <summary>
    /// Represents: Encrypt and decrypt Calendar event,
    /// with calendar event provider type.
    /// schema  providerId [addi_nfo] event primary key
    ///         +-2byte---+--30byte--+--------32byte----+
    /// Unsigned long (64 byte)
    /// </summary>
    internal static class EventProviderHelper
    {
        static public byte ProviderMaskSize; 
        static public ulong ProviderMask; 

        static EventProviderHelper()
        {
            ProviderMaskSize = 2;
            ProviderMask = ulong.MaxValue >> ProviderMaskSize;
        }

        /// <summary>
        /// Gets the event key.
        /// </summary>
        /// <param name="eventId">The event id.</param>
        /// <returns></returns>
        static public ulong GetEventKey(ulong eventId)
        {
            return eventId & ProviderMask;
        }

        /// <summary>
        /// Makes the event id.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="eventId">The event id.</param>
        /// <returns></returns>
        static public ulong MakeEventId(CalendarEventType eventType, ulong eventId)
        {
            return MakeEventId((ulong)eventType, eventId);
        }
        /// <summary>
        /// Gets the event id.
        /// </summary>
        /// <param name="providerMask">The provider mask.</param>
        /// <param name="eventId">The event id.</param>
        /// <returns></returns>
        static public ulong MakeEventId(ulong providerId, ulong eventId)
        {
            if ((eventId & ~ProviderMask) != 0)
                throw new OverflowException("ProviderMask");

            ulong retVal = eventId | (providerId << (64 - ProviderMaskSize));

            return retVal;
        }
        /// <summary>
        /// Checks the event beulong.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="eventId">The event id.</param>
        /// <returns></returns>
        static public bool CheckEventBelong(CalendarEventType eventType, ulong eventId)
        {
            return CheckEventBelong((ulong)eventType, eventId);
        }
        /// <summary>
        /// Checks the event beulong.
        /// </summary>
        /// <param name="providerMask">The provider mask.</param>
        /// <param name="eventId">The event id.</param>
        /// <returns></returns>
        static public bool CheckEventBelong(ulong providerId, ulong eventId)
        {
            return ((eventId  >> (64 - ProviderMaskSize)) == providerId);

     
        }
    }
}
