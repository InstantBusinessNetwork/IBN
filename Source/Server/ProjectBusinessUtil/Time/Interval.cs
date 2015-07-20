using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Calendar;

namespace ProjectBusinessUtil.Time
{
    /// <summary>
    /// Represent time distributed interval
    /// </summary>
    public class Interval
    {
        private long _start;
        private long _end;

        /// <summary>
        /// Initializes a new instance of the <see cref="Interval"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public Interval(long start, long end)
        {
            _start = start;
            _end = end;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Interval"/> class.
        /// </summary>
        public Interval()
        {
            _start = long.MaxValue;
            _end = long.MinValue;
        }
        #region Properties
        /// <summary>
        /// Gets or sets the begin.
        /// </summary>
        /// <value>The begin.</value>
        public long Start
        {
            get { return _start; }
            set { _start = value; }
        }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>The end.</value>
        public long End
        {
            get { return _end; }
            set { _end = value; }
        } 
        #endregion


        #region Methods
        /// <summary>
        /// Ins the range.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <param name="dst">The DST.</param>
        /// <returns></returns>
        public static bool WholeInRange(Interval checkInterval, Interval inInterval)
        {
            return (inInterval.End > checkInterval.End && inInterval.Start < checkInterval.Start);
            //dst.End > src.Start && dst.Start < src.End
        }

        /// <summary>
        /// Partials the in range.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <param name="dst">The DST.</param>
        /// <returns></returns>
        public static bool PartialInRange(Interval checkInterval, Interval inInterval)
        {
            return (inInterval.End > checkInterval.Start && inInterval.Start < checkInterval.End);
        }

        /// <summary>
        /// Intersects the in range.
        /// </summary>
        /// <param name="checkInterval">The check interval.</param>
        /// <param name="inInterval">The in interval.</param>
        /// <returns></returns>
        public static bool IntersectInRange(Interval checkInterval, Interval inInterval)
        {
            return (WholeInRange(checkInterval, inInterval) || PartialInRange(checkInterval, inInterval));
        }
        /// <summary>
        /// Gets the duration of the elapsed.
        /// </summary>
        /// <returns></returns>
        public long GetDuration()
        {
            return End - Start;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", new DateTime(CalendarHelper.Milis2Tick(this.Start)), 
                                              new DateTime(CalendarHelper.Milis2Tick(this.End)));
        }
        #endregion
	
	
    }
}
