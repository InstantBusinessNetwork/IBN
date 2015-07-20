using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ProjectBusinessUtil.Algorithm
{
    /// <summary>
    /// Represents basic date interval generator.
    /// </summary>
    public abstract class IntervalGenerator<T> : IEnumerable<T>
    {
        #region Const
        #endregion

        #region Fields
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalGenerator<T> : IEnumerable<T>"/> class.
        /// </summary>
        public IntervalGenerator()
        {
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion



        #region IEnumerable<T> Members

        abstract public IEnumerator<T> GetEnumerator();

        #endregion



        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
