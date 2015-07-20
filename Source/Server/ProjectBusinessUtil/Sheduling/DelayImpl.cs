using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Sheduling
{
    /// <summary>
    /// Represents .
    /// </summary>
    public class DelayImpl : IDelayable
    {
        #region Const
        #endregion

        #region Fields
        private long _delay;
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DelayImpl"/> class.
        /// </summary>
        public DelayImpl(long delay)
        {
            _delay = delay;
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion

        #region IDelayable Members

        public long CalcTotalDelay()
        {
            return Delay;
        }

        public long Delay
        {
            get
            {
                return _delay;
            }
            set
            {
                _delay = value;
            }
        }

        #endregion
    }
}
