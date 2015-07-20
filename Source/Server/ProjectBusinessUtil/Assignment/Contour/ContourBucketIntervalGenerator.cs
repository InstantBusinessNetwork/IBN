using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Time;
using ProjectBusinessUtil.Algorithm;
using ProjectBusinessUtil.Calendar;

namespace ProjectBusinessUtil.Assignment.Contour
{
    /// <summary>
    /// Represents Interval generator based on bucket contour.
    /// </summary>
    public class ContourBucketIntervalGenerator : IntervalGenerator<Interval>
    {
        #region Const
        #endregion

        #region Fields
        ContourBucketIntervalIterator _iterator;
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="ContourBucketIntervalGenerator"/> class.
        /// </summary>
        public ContourBucketIntervalGenerator(WorkCalendarBase workCalendar, AbstractContour contourBucket, long assignmentStart,
                                            long assignmentDuration)
        {
            _iterator = new ContourBucketIntervalIterator(workCalendar, contourBucket, assignmentStart, assignmentDuration);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the current bucket.
        /// </summary>
        /// <value>The current bucket.</value>
        public AbstractContourBucket CurrentBucket 
        {
            get { return _iterator.CurrentBucket; }
        }
        #endregion

        #region Methods
        #endregion

        #region IEnumerator
        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<Interval> GetEnumerator()
        {
            return _iterator;
        } 
        #endregion
    }
}
