using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Functor;
using ProjectBusinessUtil.Time;
using ProjectBusinessUtil.Assignment.Contour;

namespace ProjectBusinessUtil.Assignment.Functor
{
    /// <summary>
    /// Represents functor calculate date from work using by working interval bucket.
    /// </summary>
    public class DateAtWorkFunctor : AssignmentBaseFunctor<long>
    {
        #region Const
        #endregion

        #region Fields
        AssignmentBaseFunctor<double> _assignmentFunctor;
        private double _prevSum = 0;
        private double _workValue = 0;
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DateAtWorkFunctor"/> class.
        /// </summary>
        public DateAtWorkFunctor(AssignmentBaseFunctor<double> assignmentFunctor, double workValue)
            : base (assignmentFunctor.Assignment, assignmentFunctor.CountourGenerator)
        {
            _assignmentFunctor = assignmentFunctor;
            _workValue = workValue;
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
        }


        /// <summary>
        /// Executes the specified param.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <returns></returns>
        public override object Execute(Interval param)
        {
            Interval interval = param;
            //Execute aggregated functor
            _assignmentFunctor.Execute(param);

            double sum = _assignmentFunctor.Value;
            if (this.Value == 0 && sum >= _workValue)
            {
                if (_workValue == 0.0 || interval.Start == interval.End)
                { // take care of degenerate case
                    this.Value = param.Start;
                }
                double workIntervalDuration = sum - _prevSum;
                double fractionOfDuration = (workIntervalDuration - (sum - _workValue)) / workIntervalDuration;
                long duration = WorkingCalendar.SubstractDates(interval.End, interval.Start, false);
                this.Value = WorkingCalendar.AddDuration(interval.Start, (long)(duration * fractionOfDuration), true);
            }

            _prevSum = sum;
            return this;
        }
    }
}

