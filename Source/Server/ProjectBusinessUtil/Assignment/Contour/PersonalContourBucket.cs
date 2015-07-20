using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Assignment.Contour
{
    /// <summary>
    /// Represents  An allocation bucket represents the finest grained detail for an assignment.  The amount of work is determined
    /// by a value of effort during a duration.
    /// I do not store the absolute time value of the start/end because the allocation bucket can be shifted.
    /// The basic formula Work = Units * Duration applies..
    /// </summary>
    public class PersonalContourBucket : AbstractContourBucket
    {
        #region Const
        #endregion

        #region Fields
        private long _duration = 0;
        private long _elapsedDuration = 0;
        private double _units = 1.0;
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="MyClass"/> class.
        /// </summary>
        public PersonalContourBucket(long duration, double units)
        {
            _duration = duration;
            _units = units;
        }
        public PersonalContourBucket(long duration, long elapsedDuration, double units)
        {
            _duration = duration;
            _elapsedDuration = elapsedDuration;
            _units = units;
        }
        #endregion

        #region Properties
        public long Duration
        {
            get { return _duration; }
        }
        public long ElapsedDuration
        {
            get { return _elapsedDuration; }
            set { _elapsedDuration = value; }
        }
        public double Units
        {
            get { return _units; }
        }
	
        #endregion

        #region Methods

        public PersonalContourBucket AdjustUnits(double multiplier)
        {
            _units *= multiplier;
            return this;
        }

        public PersonalContourBucket AdjustWork(double multiplier)
        {
            if (multiplier == 0)
                throw new ArgumentNullException("multiplier cannot be 0");

            _duration =(long) (_duration * multiplier);
            _units /= multiplier;
            return this;
        }

        public PersonalContourBucket AdjustDuration(long offset)
        {
            _duration = _duration + offset;
            return this;
        }

        #endregion

        #region AbstractContourBucket
        /// <summary>
        /// Gets the duration of the bucket.
        /// </summary>
        /// <param name="assignmentDuration">Duration of the assignment.</param>
        /// <returns></returns>
        public override long GetBucketDuration(long assignmentDuration)
        {
            return _duration;
        }

        /// <summary>
        /// Gets the effective units.
        /// </summary>
        /// <param name="assignmentUnits">The assignment units.</param>
        /// <returns></returns>
        public override double GetEffectiveUnits(double assignmentUnits)
        {
            return  _units;
        } 
        #endregion
    }
}
