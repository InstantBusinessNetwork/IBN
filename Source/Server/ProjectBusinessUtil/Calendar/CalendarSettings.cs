using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Calendar
{
    /// <summary>
    /// Represents calendar settings.
    /// </summary>
    public class CalendarSettings
    {
        #region Const
        public const byte CalendarIntervals = 8;
        public const byte DayInWeek = 7;
        //0 - is not working day; 1 - is working day
        // 0 - Sunday 1- is monday ... 6 - is saturday
        public const byte WeekDayMask = 0x3E; //0111110
       

        #endregion

        #region Fields
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarSettings"/> class.
        /// </summary>
        public CalendarSettings()
        {
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion
    }
}
