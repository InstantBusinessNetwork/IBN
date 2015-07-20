using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Calendar
{
    /// <summary>
    /// Represents calendar time helper.
    /// </summary>
    public static class CalendarHelper
    {
        #region Const
        #endregion

        #region Fields
        #endregion
      
        #region Properties
        #endregion

        #region Methods
        public static long Milis2Tick(long milis)
        {
            return milis * TimeSpan.TicksPerMillisecond;
        }
        public static long Tick2Milis(long tick)
        {
            return tick / TimeSpan.TicksPerMillisecond;
        }

        public static long MilisPerMinute()
        {
            return Tick2Milis(TimeSpan.TicksPerMinute);
        }
        public static long MilisPerHour()
        {
            return Tick2Milis(TimeSpan.TicksPerHour);
        }
        public static long MilisPerDay()
        {
            return Tick2Milis(TimeSpan.TicksPerDay);
        }
        public static long MilisPerWeek()
        {
            return MilisPerDay() * 7;
        }
        public static long MilisPerMonth()
        {
            return MilisPerDay() * 31;
        }
        public static long MilisPerYear()
        {
            return MilisPerDay() * 366;
        }
        /// <summary>
        /// Days the of.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static long DayOf(long date)
        {
            DateTime dateHelper = new DateTime(Milis2Tick(date), DateTimeKind.Unspecified);
            return Tick2Milis(new DateTime(dateHelper.Year, dateHelper.Month, dateHelper.Day).Ticks);
        }
        /// <summary>
        /// Times the of.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static long TimeOf(long date)
        {
            return date - DayOf(date);
        }
        /// <summary>
        /// Days the of week.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static int DayOfWeek(long date)
        {
            return (int)(new DateTime(Milis2Tick(date), DateTimeKind.Unspecified).DayOfWeek);
        }
     
        /// <summary>
        /// Tick2s the hour.
        /// </summary>
        /// <param name="tick">The tick.</param>
        /// <returns></returns>
        public static int HourFromMilis(long milis)
        {
            return new TimeSpan(Milis2Tick(milis)).Hours;
        }
        /// <summary>
        /// Tick2s the minute.
        /// </summary>
        /// <param name="tick">The tick.</param>
        /// <returns></returns>
        public static int MinuteFromMilis(long milis)
        {
            return new TimeSpan(Milis2Tick(milis)).Minutes;
        }
        #endregion
    }
}
