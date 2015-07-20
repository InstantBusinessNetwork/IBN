using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Calendar
{
    /// <summary>
    /// Represents working week.
    /// </summary>
    public class WorkWeek
    {
        #region Const
        #endregion

        #region Fields
        WorkDay[] _workDays = 
                    new WorkDay[CalendarSettings.DayInWeek];
        byte _weekDaysMask =
                        CalendarSettings.WeekDayMask;
        #endregion

        #region .Ctor
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the week day mask.
        /// </summary>
        /// <value>The week day mask.</value>
        public byte WeekDayMask
        {
            get
            { return _weekDaysMask; }
            set
            { _weekDaysMask = value; }
        }
        /// <summary>
        /// Gets the week days.
        /// </summary>
        /// <value>The week days.</value>
        public WorkDay[] WeekDays
        {
            get { return _workDays; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <returns></returns>
        public long GetDuration()
        {
            long retVal = 0;
            foreach(WorkDay workDay in WeekDays)
            {
                if (workDay != null)
                    retVal += workDay.GetDuration();
            }

            return retVal;
        }

        /// <summary>
        /// Sets the week days.
        /// </summary>
        /// <param name="day">The day.</param>
        public void SetWeekDays(WorkDay day)
        {
            for(int i = 0; i< CalendarSettings.DayInWeek; i++)
            {
              if(IsWeekEnd(i) == false)
              {
                  WeekDays[i] = day;
              }
            }
        }

        /// <summary>
        /// Sets the week end days.
        /// </summary>
        /// <param name="day">The day.</param>
        public void SetWeekEndDays(WorkDay day)
        {
            for (int i = 0; i < CalendarSettings.DayInWeek; i++)
            {
                if (IsWeekEnd(i) == true)
                {
                    WeekDays[i] = day;
                }
            }
        }

        /// <summary>
        /// Isweeks the end.
        /// </summary>
        /// <param name="weekDayNum">The week day num.</param>
        /// <returns></returns>
        private bool IsWeekEnd(int weekDayNum)
        {
            return (((WeekDayMask >> weekDayNum) & 0x01) == 0);
        }

        
        #endregion

    }
}
