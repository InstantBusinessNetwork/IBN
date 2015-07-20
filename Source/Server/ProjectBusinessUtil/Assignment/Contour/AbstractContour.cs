using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Assignment.Contour
{
    /// <summary>
    /// Represents abstract base class for work and cost contours
    /// </summary>
    public abstract class AbstractContour
    {
        #region Const
        #endregion

        #region Fields
        protected List<AbstractContourBucket> _contourBuckets;
        protected ContourTypes _contourType = ContourTypes.Contoured;
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractContour"/> class.
        /// </summary>
        public AbstractContour(ContourTypes contourType, AbstractContourBucket[] buckets)
        {
            _contourType = contourType;
            _contourBuckets = new List<AbstractContourBucket>(buckets);
        }

        public AbstractContour()
            : this(ContourTypes.Flat, new AbstractContourBucket[] { })
        {

        }
        #endregion

        #region Properties
        public List<AbstractContourBucket> ContourBuckets
        {
            get { return _contourBuckets; }
        }

		public ContourTypes ContourType
		{
			get { return _contourType; }
		}
        #endregion

        #region Methods
        abstract public AbstractContour AdjustDuration(long newDuration, long actualDuration);
        abstract public AbstractContour AdjustUnits(double multiplier, long startingFrom);
        abstract public AbstractContour AdjustWork(double multiplier, long actualDuration);


        /// <summary>
        /// Calcs the duration of the sum bucket.
        /// </summary>
        /// <param name="assignmentDuration">Duration of the assignment.</param>
        /// <returns></returns>
        public long CalcSumBucketDuration(long assignmentDuration)
        {
            long retVal = 0;
            foreach(AbstractContourBucket bucket in ContourBuckets)
            {
                retVal += bucket.GetBucketDuration(assignmentDuration);
            }

            return retVal;
        }

        #endregion

        
    }
}
