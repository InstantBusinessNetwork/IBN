using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Assignment.Contour
{
    /// <summary>
    /// Represents Personal work and cost contour.
    /// </summary>
    public class PersonalContour : AbstractContour
    {
        #region Const
        #endregion

        #region Fields
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonalContour"/> class.
        /// </summary>
        public PersonalContour(ContourTypes contourType, AbstractContourBucket[] buckets)
            : base(contourType, buckets)
        {
        }
        public PersonalContour()
            : base()
        {
        }

        #endregion

        #region Properties
        #endregion

        #region Methods
        /// <summary>
        /// Gets all buckets before a duration, cutting a bucket in half if needed.  
        /// Will return empty list if at start. If fraction has specified then get half bucket if durationAt
        /// intersect with exist bucket
        /// </summary>
        /// <param name="durationAt">The duration at.</param>
        /// <returns></returns>
        private PersonalContourBucket[] GetBucketsBeforeDuration(long durationAt)
        {
            List<PersonalContourBucket> retVal = new List<PersonalContourBucket>();
            long currentDuration = 0;
            PersonalContourBucket personalBucket = null;
            
            if (durationAt != 0)
            {
                foreach (AbstractContourBucket bucket in base.ContourBuckets)
                {
                    personalBucket = bucket as PersonalContourBucket;
                    if (personalBucket != null)
                    {
                        currentDuration = personalBucket.ElapsedDuration;
                        if (currentDuration >= durationAt)
                        {
                            retVal.Add(personalBucket);
                            break;
                        }
                        retVal.Add(personalBucket);
                    }
                }
            }
            return retVal.ToArray();
        }

        /// <summary>
        /// Gets all buckets after a duration, cutting a bucket in half if needed.  
        /// Will return empty list if at end
        /// </summary>
        /// <param name="durationAt">The duration at.</param>
        /// <returns></returns>
        private PersonalContourBucket[] GetBucketAfterDuration(long durationAt)
        {
            List<PersonalContourBucket> retVal = new List<PersonalContourBucket>();
            long currentDuration = 0;
            PersonalContourBucket personalBucket = null;
            foreach (AbstractContourBucket bucket in base.ContourBuckets)
            {
                personalBucket = bucket as PersonalContourBucket;
                if (personalBucket != null)
                {
                    currentDuration = personalBucket.ElapsedDuration;
                    if (currentDuration > durationAt)
                    {
                        retVal.Add(personalBucket);
                    }
                }
            }

            return retVal.ToArray();
        }
        #endregion

        /// <summary>
        /// Set the duration of the personal contour. This implies either truncating the bucket array or changing the last bucket
        /// to accommodate the new duration. Note that only the last bucket will be modified.  The number of buckets will never increase.
        /// Since the buckets themselves are immutable, the last element will most likely be replaced.
        /// </summary>
        /// <param name="newDuration">The new duration.</param>
        /// <param name="actualDuration">The actual duration.</param>
        /// <returns></returns>
        public override AbstractContour AdjustDuration(long newDuration, long actualDuration)
        {
            PersonalContour newContour = new PersonalContour();
            foreach (AbstractContourBucket bucket in base.ContourBuckets)
            {
                PersonalContourBucket personalBucket = (PersonalContourBucket)bucket;
                newContour.ContourBuckets.Add(personalBucket);
                newDuration -= personalBucket.Duration;
                if (newDuration <= 0)
                {
                    // adding a negative value
                    personalBucket.AdjustDuration(newDuration);
                    newDuration = 0;
                    //AbstractContour result = newContour.makePacked(); // pack so as to get rid of any trailing empty buckets
                }
            }
            // extend last bucket to account for duration
            ((PersonalContourBucket)newContour.ContourBuckets[newContour.ContourBuckets.Count - 1]).AdjustDuration(newDuration);

            this.ContourBuckets.Clear();
            this.ContourBuckets.AddRange(newContour.ContourBuckets);
            return this;
        }

        /// <summary>
        /// Adjusts the units.
        /// </summary>
        /// <param name="multiplier">The multiplier.</param>
        /// <param name="startingFrom">The starting from.</param>
        /// <returns></returns>
        public override AbstractContour AdjustUnits(double multiplier, long startingFrom)
        {
            PersonalContour newContour = new PersonalContour();
            newContour.ContourBuckets.AddRange(GetBucketsBeforeDuration(startingFrom));
            foreach (PersonalContourBucket remainingBucket in GetBucketAfterDuration(startingFrom))
            {
                newContour.ContourBuckets.Add(remainingBucket.AdjustUnits(multiplier));
            }
            this.ContourBuckets.Clear();
            this.ContourBuckets.AddRange(newContour.ContourBuckets);
            return this;
        }

        /// <summary>
        /// Adjusts the work.
        /// </summary>
        /// <param name="multiplier">The multiplier.</param>
        /// <param name="actualDuration">The actual duration.</param>
        /// <returns></returns>
        public override AbstractContour AdjustWork(double multiplier, long actualDuration)
        {
            PersonalContour newContour = new PersonalContour();
            newContour.ContourBuckets.AddRange(GetBucketsBeforeDuration(actualDuration));
            foreach (PersonalContourBucket remainingBucket in GetBucketAfterDuration(actualDuration))
            {
                newContour.ContourBuckets.Add(remainingBucket.AdjustWork(multiplier));
            }
            this.ContourBuckets.Clear();
            this.ContourBuckets.AddRange(newContour.ContourBuckets);
            return this;
        }

        /// <summary>
        /// Inserts the interval into bucket sequence
        /// </summary>
        /// <param name="startDuration">The start duration.</param>
        /// <param name="endDuration">The end duration.</param>
        /// <param name="units">The units.</param>
        /// <returns></returns>
        public PersonalContour InsertInterval(long startDuration, long endDuration, double units)
        {
            if (startDuration != endDuration)
            {
                List<PersonalContourBucket> buckets = new List<PersonalContourBucket>();
                buckets.AddRange(GetBucketsBeforeDuration(startDuration));
                buckets.Add(new PersonalContourBucket(endDuration - startDuration, endDuration, units));
                buckets.AddRange(GetBucketAfterDuration(endDuration));
                this.ContourBuckets.Clear();
                this.ContourBuckets.AddRange(buckets.ToArray());
            }

            return this;
        }

    }
}
