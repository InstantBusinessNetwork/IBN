using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Assignment.Contour
{
    /// <summary>
    /// Represents standard MsProject contour work distribution.
    /// </summary>
    public  class StandardContourBucket : AbstractContourBucket
    {
        #region Const
        #endregion

        #region Fields
        double _fractionOfDuration = 1.0;
        double _units = 1.0;
        #endregion

        #region .Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardContourBucket"/> class.
        /// </summary>
        public StandardContourBucket(double units, double fractionOfDuration)
        {
            _fractionOfDuration = fractionOfDuration;
            _units = units;
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion


        #region AbstractContourBucket
        /// <summary>
        /// Gets the duration of the bucket.
        /// </summary>
        /// <param name="assignmentDuration">Duration of the assignment.</param>
        /// <returns></returns>
        public override long GetBucketDuration(long assignmentDuration)
        {
            return (long)(assignmentDuration * _fractionOfDuration);
        }

        /// <summary>
        /// Gets the effective units.
        /// </summary>
        /// <param name="assignmentUnits">The assignment units.</param>
        /// <returns></returns>
        public override double GetEffectiveUnits(double assignmentUnits)
        {
            return _units * assignmentUnits;
        } 
        #endregion
    }
}
