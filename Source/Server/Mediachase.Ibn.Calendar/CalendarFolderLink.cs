using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Calendar
{
    /// <summary>
    /// Represents: Calendar folder link support. 
    /// </summary>
    partial class CalendarFolderLink
    {
        static CalendarFolderLink()
        {
            CalendarFolderManager.CalendarDeleting += DeleteLinkDirectly;
        }



        /// <summary>
        /// Deletes the link directly.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        internal static void DeleteLinkDirectly(object sender, EventArgs args)
        {
            Calendar calendar = sender as Calendar;
            if(calendar != null)
            {
       
                CalendarFolderLink[] links = 
                            CalendarFolderLink.List(new FilterElement("CalendarId",
                                                                       FilterElementType.Equal,
                                                                       calendar.PrimaryKeyId.Value));
                foreach(CalendarFolderLink link in links)
                {
                    link.Delete();
                }

             }
        }
    }
}
