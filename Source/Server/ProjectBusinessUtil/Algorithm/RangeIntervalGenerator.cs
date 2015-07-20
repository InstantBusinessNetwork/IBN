using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Time;

namespace ProjectBusinessUtil.Algorithm
{
    /// <summary>
    /// Represents .
    /// </summary>
    public class RangeIntervalGenerator : IntervalGenerator<Interval>
    {
        #region Const
        #endregion

        #region Fields
        private RangeIntervalIterator _iterator;
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeIntervalGenerator"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="calendarStepUnit">The calendar step unit.</param>
        public RangeIntervalGenerator(long start, long end, int calendarStepAmount)
        {
            _iterator = new RangeIntervalIterator(start, end, calendarStepAmount);
        }

        public RangeIntervalGenerator(long start, long end)
        {
            _iterator = new RangeIntervalIterator(start, end);
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion

        public override IEnumerator<Interval> GetEnumerator()
        {
            return _iterator;
        }
    }
}
