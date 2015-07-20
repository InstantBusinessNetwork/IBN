using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Assignment.Contour
{
    /// <summary>
    /// Represents abstract base class for work and cost bucket.
    /// </summary>
    public abstract class AbstractContourBucket
    {
        #region Const
        #endregion

        #region Fields
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractContourBucket"/> class.
        /// </summary>
        public AbstractContourBucket()
        {
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        /// <summary>
        /// Gets the duration of the bucket.
        /// </summary>
        /// <param name="assignmentDuration">Duration of the assignment.</param>
        /// <returns></returns>
        abstract public long GetBucketDuration(long assignmentDuration);
        /// <summary>
        /// Gets the effective units.
        /// </summary>
        /// <param name="assignmentUnits">The assignment units.</param>
        /// <returns></returns>
        abstract public double GetEffectiveUnits(double assignmentUnits);

        #endregion

        
    }
}
