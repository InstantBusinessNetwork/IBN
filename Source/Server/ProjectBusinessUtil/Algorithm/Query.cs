using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Functor;
using System.Collections;

namespace ProjectBusinessUtil.Algorithm
{
    /// <summary>
    /// Represents functional query behavior.
    /// </summary>
    public class Query<T>
    {
        #region Const
        #endregion

        #region Fields
        IEnumerable<T> _fromGenerator;
        Predicate<T> _wherePredicate;
        IFunctor<T> _selectStatement;
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        public Query()
        {
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        /// <summary>
        /// Selects the specified select statement.
        /// </summary>
        /// <param name="selectStatement">The select statement.</param>
        /// <returns></returns>
        public Query<T> Select(IFunctor<T> selectStatement)
        {
            _selectStatement = selectStatement;
            return this;
        }

        /// <summary>
        /// Froms the specified from generator.
        /// </summary>
        /// <param name="fromGenerator">From generator.</param>
        /// <returns></returns>
        public Query<T> From(IEnumerable<T> fromGenerator)
        {
            _fromGenerator = fromGenerator;
            return this;
        }

        /// <summary>
        /// Wheres the specified where predicate.
        /// </summary>
        /// <param name="wherePredicate">The where predicate.</param>
        /// <returns></returns>
        public Query<T> Where(Predicate<T> wherePredicate)
        {
            _wherePredicate = wherePredicate;
            return this;

        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        public void Execute()
        {
            foreach(T item in _fromGenerator)
            {
                if(_wherePredicate(item) == true)
                {
                   _selectStatement.Execute(item); 
                }
            }
        }
        #endregion

        
    }
}
