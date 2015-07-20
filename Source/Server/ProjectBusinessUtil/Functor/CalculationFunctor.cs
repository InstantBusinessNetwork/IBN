using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Functor
{
    /// <summary>
    /// Represents calculation functor uses in queries.
    /// </summary>
    public abstract class CalculationFunctor<T> : IFunctor<T>
    {
        #region Const
        #endregion

        #region Fields
        private bool _cumulative;
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="CalculationFunctor<T> : IFunctor<T>"/> class.
        /// </summary>
        public CalculationFunctor()
        {
        }
        #endregion

        #region Properties
        public bool IsCumulative
        {
            get { return _cumulative; }
            protected set { _cumulative = value; }
        }
        #endregion

        #region Methods
        abstract public void Initialize();
        #endregion

        #region IFunctor<T> Members

        abstract public object Execute(T param);
        
        #endregion
    }
}
