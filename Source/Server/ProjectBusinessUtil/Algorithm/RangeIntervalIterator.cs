using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Time;
using ProjectBusinessUtil.Calendar;

namespace ProjectBusinessUtil.Algorithm
{
    /// <summary>
    /// Represents calendar range interval iterator.
    /// </summary>
    public class RangeIntervalIterator : Interval, IEnumerator<Interval>
    {
        #region Const
        #endregion

        #region Fields
        Interval _current;
        long _stepAmount = 0;
        protected bool _finish = false;
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeIntervalIterator"/> class.
        /// </summary>
        public RangeIntervalIterator(long start, long end, long stepAmount)
            : base(start, end)
        {
            _stepAmount = stepAmount;
            _current = new Interval(start, start);
        }
        public RangeIntervalIterator(long start, long end)
            : this(start, end, CalendarHelper.MilisPerDay())
        {
        }
        #endregion

        #region Properties
        protected long StepAmount
        {
            get { return _stepAmount; }
            set { _stepAmount = value; }
        }
        #endregion

        #region Methods
        #endregion



        #region IEnumerator<Interval> Members

        public virtual Interval Current
        {
            get { return _current; }
        }

        /// <summary>
        /// Gets the next.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <returns></returns>
        protected bool GetNext(Interval interval, ref Interval nextInterval)
        {
            
            nextInterval = new Interval(interval.Start, interval.End);
            nextInterval.Start = nextInterval.End;
            nextInterval.End += _stepAmount;
            if (nextInterval.End > this.End)
            {
                nextInterval.End = this.End;
                return false;
            }

            return true;
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion

        #region IEnumerator Members

        public virtual bool MoveNext()
        {
            if (_finish == true)
                return false;
            
            _finish = GetNext(_current, ref _current);
            return true;
        }

        public virtual void Reset()
        {
            _current = null;
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
