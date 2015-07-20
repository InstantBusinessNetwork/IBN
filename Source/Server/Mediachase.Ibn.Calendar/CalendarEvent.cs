using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Calendar
{
    /// <summary>
    /// Represent: Calendar Event instance. 
    /// </summary>
    partial class CalendarEvent
    {
        #region Events
           /// <summary>
           /// Raise if Calendar event create
           /// </summary>
            static public event CalendarEventHandler CalEventCreateEvent;

            /// <summary>
            /// Raise if Calendar event delete
            /// </summary>
            static public event CalendarEventHandler CalEventDeleteEvent;


            /// <summary>
            /// Raises the create event.
            /// </summary>
            static public void RaiseCreateEvent(CalendarEvent calEvent)
            {
                CalendarEventArgs args = new CalendarEventArgs();
                args.CalEvent = calEvent;
                OnEventCreate(args);
            }

            /// <summary>
            /// Raises the delete event.
            /// </summary>
            static public void RaiseDeleteEvent(CalendarEvent calEvent)
            {
                CalendarEventArgs args = new CalendarEventArgs();
                args.CalEvent = calEvent;
                OnEventDelete(args);
            }

            /// <summary>
            /// Raises the <see cref="E:EventCreate"/> event.
            /// </summary>
            /// <param name="args">The <see cref="Mediachase.Ibn.Calendar.CalendarEventArgs"/> instance containing the event data.</param>
            static private void OnEventCreate(CalendarEventArgs args)
            {
                CalendarEventHandler temp = CalEventCreateEvent;

                if (temp != null)
                    CalEventCreateEvent(null, args);
            }

            /// <summary>
            /// Raises the <see cref="E:EventDelete"/> event.
            /// </summary>
            /// <param name="args">The <see cref="Mediachase.Ibn.Calendar.CalendarEventArgs"/> instance containing the event data.</param>
            static private void OnEventDelete(CalendarEventArgs args)
            {
                CalendarEventHandler temp = CalEventDeleteEvent;

                if (temp != null)
                    CalEventDeleteEvent(null, args);
            }
        #endregion
              
    }
}
