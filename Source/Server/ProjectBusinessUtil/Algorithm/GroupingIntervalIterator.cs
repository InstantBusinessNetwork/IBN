using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Time;

namespace ProjectBusinessUtil.Algorithm
{
    /// <summary>
    /// Represents grouping iteration on two interval generator.
    /// </summary>
    public class GroupingIntervalIterator : RangeIntervalIterator
    {
        #region Const
        #endregion

        #region Fields
        private IEnumerable<Interval> _subIntervalGenerator;
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupingIntervalIterator"/> class.
        /// </summary>
        public GroupingIntervalIterator(long start, long end, long calendarStepAmount,
                                         IEnumerable<Interval> subIntervalGenerator)
            : base(start, end, calendarStepAmount)
        {
            _subIntervalGenerator = subIntervalGenerator;
            //Initialize aggregated enumerator
        }
        #endregion

        #region Properties
        #endregion

        #region Methods

       
        public override Interval Current
        {
            get
            {
                return base.Current;
            }
        }

        /// <summary>
        /// Moves the next.
        /// Moves time interval to step amount and move iterator aggregated collection
        /// Генерирует интервалы задданой длительности на заданном отрезке, и паралельно призводит итерацию
        /// наложенного генератора интервалов в соответсвии с совпадениями генерируемых интервалов с наложенными
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            if (base._finish == true)
                return false;

            Interval subInterval = _subIntervalGenerator.GetEnumerator().Current;
            Interval groupNextInterval = new Interval();
            
            if (Current.Start == subInterval.End || Current.End == subInterval.End)
            {
                if (_subIntervalGenerator.GetEnumerator().MoveNext() == false)
                    return false;
                subInterval = _subIntervalGenerator.GetEnumerator().Current;
            }

            base.GetNext(this.Current, ref groupNextInterval);
            Current.Start = groupNextInterval.Start;
			Current.End = Math.Min(groupNextInterval.End, subInterval.End);

            if (Current.End == base.End)
                base._finish = true;

            return true;
        }
        /// <summary>
        /// Resets this instance.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _subIntervalGenerator.GetEnumerator().Reset();
        }
        #endregion


    }
}
