using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Time;
using ProjectBusinessUtil.Calendar;

namespace ProjectBusinessUtil.Assignment.Contour
{
    /// <summary>
    /// Represents ContourBucket iterator, use for iteration on defined work contour.
    /// </summary>
    public class ContourBucketIntervalIterator : Interval, IEnumerator<Interval> 
    {
        #region Const
        #endregion

        #region Fields
        private WorkCalendarBase _workCalendar;
        private AbstractContour _contourBucket;
        private Interval _current;
        private int _currentBucket = -1;
        private long _assignmentDuration;
        
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="ContourBucketIntervalIterator "/> class.
        /// </summary>
        public ContourBucketIntervalIterator(WorkCalendarBase workCalendar, AbstractContour contourBucket, long assignmentStart,
                                            long assignmentDuration)
        {
            _workCalendar = workCalendar;
            _contourBucket = contourBucket;
            _assignmentDuration = assignmentDuration;

            this.Start = this.End = assignmentStart;
            _current = new Interval(assignmentStart, assignmentStart);
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion



        #region IEnumerator<Interval> Members

        public Interval Current
        {
            get 
             {
                return _current; 
             }
        }

        /// <summary>
        /// Gets the current bucket.
        /// </summary>
        /// <value>The current bucket.</value>
        public AbstractContourBucket CurrentBucket
        {
            get
            {
                if (_currentBucket >= 0 && _currentBucket < _contourBucket.ContourBuckets.Count)
                    return _contourBucket.ContourBuckets[_currentBucket];

                return null;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region IEnumerator Members
        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        public bool MoveNext()
        {
            _currentBucket++;
            //Is all bucket out
            if (_currentBucket == _contourBucket.ContourBuckets.Count)
                return false;

            _current.Start = _workCalendar.AddDuration(_current.End, 0, false);
            long bucketDuration = _contourBucket.ContourBuckets[_currentBucket].GetBucketDuration(_assignmentDuration);
            _current.End = _workCalendar.AddDuration(_current.Start, bucketDuration, true);
            

            return true;            
        }

        public void Reset()
        {
            _currentBucket = 0;
            _current.Start = _current.End = this.Start;
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }
}
