using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Time;

namespace ProjectBusinessUtil.Calendar
{
    /// <summary>
    /// Represents one working time interval
    /// </summary>
    public class WorkRange : Interval
    {
        private bool _overTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="overtime">if set to <c>true</c> [overtime].</param>
        public WorkRange(long start, long end, bool overtime)
            : base(start, end)
        {
            _overTime = overtime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public WorkRange(long start, long end)
            : this(start, end, false)
        {
        }

        #region Properties
        public bool OverTime
        {
            get { return _overTime; }
            set { _overTime = value; }
        } 
        #endregion

        #region Methods
        /// <summary>
        /// Gets the duration of the work.
        /// </summary>
        /// <returns></returns>
        long GetDuration()
        {
            return this.GetDuration();
        }
        /// <summary>
        /// Overlapses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        bool Overlaps(WorkRange other)
        {
            if (other == null)
                return false;

            return ((this.Start > other.Start && this.End > other.End) 
                    || (other.Start > this.Start && other.End > this.End)) == false;
        }
        #endregion
    }
}
