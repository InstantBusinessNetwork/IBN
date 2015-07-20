using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data.Services;

namespace Mediachase.Ibn.Calendar
{
    /// <summary>
    /// Represents: Calendar folder support and calendar tree storage.
    /// </summary>
    partial class CalendarFolder
    {
        /// <summary>
        /// Gets the tree service.
        /// </summary>
        /// <returns></returns>
        public TreeService GetTreeService()
        {
            return this.GetService<TreeService>();
        }

    }
}
