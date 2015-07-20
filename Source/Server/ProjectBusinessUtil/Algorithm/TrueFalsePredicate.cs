using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Algorithm
{
    /// <summary>
    /// Represents .
    /// </summary>
    public class TrueFalsePredicate<T>
    {
        #region Const
        #endregion

        #region Fields
        private bool _result;
        #endregion

        #region .Ctor
        public TrueFalsePredicate(bool result)
        {
            _result = result;
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        public bool Evaluate(T value)
        {
            return _result;
        }
        #endregion


    }
}
