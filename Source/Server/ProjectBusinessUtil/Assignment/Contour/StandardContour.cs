using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Assignment.Contour
{
    /// <summary>
    /// Represents standart contour bucket.
    /// </summary>
    public class StandardContour : AbstractContour
    {
        #region Const
        #endregion

        #region Fields
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="StandardContour"/> class.
        /// </summary>
        public StandardContour(ContourTypes contourType, AbstractContourBucket[] buckets)
            : base(contourType, buckets)
        {
        }

        public StandardContour()
            : base()
        {
        }

        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion

        public override AbstractContour AdjustDuration(long newDuration, long actualDuration)
        {
            return this;
        }

        public override AbstractContour AdjustUnits(double multiplier, long startingFrom)
        {
            return this;
        }

        public override AbstractContour AdjustWork(double multiplier, long actualDuration)
        {
            return this;
        }
    }
}
