using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Assignment.Contour;
using ProjectBusinessUtil.Time;

namespace ProjectBusinessUtil.Assignment.Functor
{
    /// <summary>
    /// Represents .
    /// </summary>
    public class OverTimeFunctor : AssignmentBaseFunctor<double>
    {
        #region Const
        #endregion

        #region Fields
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="OverTimeFunctor"/> class.
        /// </summary>
        public OverTimeFunctor(Assignment assignment, ContourBucketIntervalGenerator contourGenerator,
                               double overTimeUnits)
            : base(assignment, contourGenerator)
        {
            _overTimeUnits = overTimeUnits;
        }
        #endregion

        #region Properties
        protected double _overTimeUnits;
        protected double _regularValue = 0.0;
        protected double _overTimeValue = 0.0;
        #endregion

        #region Methods
        #endregion

        public override object Execute(Interval param)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Initialize()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
