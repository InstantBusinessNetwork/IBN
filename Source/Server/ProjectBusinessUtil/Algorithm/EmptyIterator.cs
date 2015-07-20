using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Time;

namespace ProjectBusinessUtil.Algorithm
{
    /// <summary>
    /// Represents empty iterator.
    /// </summary>
    public class EmptyIterator<T> : IEnumerator<T>
    {
        #region Const
        #endregion

        #region Fields
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyIntervalIterator"/> class.
        /// </summary>
        public EmptyIterator()
        {
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion

        #region IEnumerator<T> Members

        public T Current
        {
            get { return default(T); }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { return null; }
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
            
        }

        #endregion
    }
}
