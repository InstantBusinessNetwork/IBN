using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Time;
using ProjectBusinessUtil.Functor;
using ProjectBusinessUtil.Calendar;
using ProjectBusinessUtil.Assignment.Contour;

namespace ProjectBusinessUtil.Assignment.Functor
{
    /// <summary>
    /// Represents .
    /// </summary>
    public abstract class AssignmentBaseFunctor<T> : CalculationFunctor<Interval>, IHasValue<T>
    {
        #region Const
        #endregion

        #region Fields
        private Assignment _assignment;
        private ContourBucketIntervalGenerator _contourIntervalGenerator;
        private T _value;
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="AssignmentFunctor"/> class.
        /// </summary>
        public AssignmentBaseFunctor(Assignment assignment, ContourBucketIntervalGenerator contourGenerator)
        {
            _assignment = assignment;
            _contourIntervalGenerator = contourGenerator;
        }
        #endregion

        #region Properties
        public Assignment Assignment
        {
            get { return _assignment; }
        }
        public double AssignmentUnits
        {
            get { return _assignment.Units; }
        }
        public long AssignmentDuration
        {
            get { return _assignment.Duration; }
        }
        public WorkCalendarBase WorkingCalendar
        {
            get { return _assignment.WorkingCalendar; }
        }
        public ContourBucketIntervalGenerator CountourGenerator
        {
            get { return _contourIntervalGenerator; }
        }
        #endregion
        

        #region Methods
        #endregion

        #region IHasValue<T2> Members

        public virtual T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        #endregion
    }
}
