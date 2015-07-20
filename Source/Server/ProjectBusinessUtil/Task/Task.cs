using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Task
{
    /// <summary>
    /// Represents .
    /// </summary>
    public class Task
    {
        #region Const
        #endregion

        #region Fields
        long _start;

        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="Task"/> class.
        /// </summary>
        public Task()
        {
        }
        #endregion

        #region Properties
        public long Start
        {
            get { return _start; }
            set { _start = value; }
        }
	
        #endregion

        #region Methods
        #endregion
    }
}
