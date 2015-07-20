using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Time;

namespace ProjectBusinessUtil.Algorithm
{
    /// <summary>
    /// Represents .
    /// </summary>
    public class WhereInRangePredicate : Interval
    {
        #region Const
        #endregion

        #region Fields
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="WhereInRangePredicate"/> class.
        /// </summary>
        public WhereInRangePredicate(long start, long end)
            : base(start, end)
        {
        }
        #endregion

        #region Properties
        #endregion

        #region Methods

        /// <summary>
        /// Evaluates the specified interval.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <returns></returns>
        public bool Evaluate(Interval interval)
        {
            return (interval.Start >= this.Start && interval.End <= this.End);
        }

        #endregion


    }
}
