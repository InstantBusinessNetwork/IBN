using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Time;

namespace ProjectBusinessUtil.Algorithm
{
    /// <summary>
    /// Represents .
    /// </summary>
    public class GroupingIntervalGenerator : IntervalGenerator<Interval>
    {
        #region Const
        #endregion

        #region Fields
        private GroupingIntervalIterator _iterator;
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupingIntervalGenerator"/> class.
        /// </summary>
        public GroupingIntervalGenerator(long start, long end, long calendarStepAmount,
                                         IEnumerable<Interval> subIntervalGenerator)
        {
            _iterator = new GroupingIntervalIterator(start, end, calendarStepAmount, subIntervalGenerator);
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        public override IEnumerator<Interval> GetEnumerator()
        {
            return _iterator;
        }

        #endregion
    }
}
