using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Time;

namespace ProjectBusinessUtil.Calendar
{
    /// <summary>
    /// Represents working day.
    /// </summary>
    public class WorkDay : Interval, ICloneable
    {
        #region Const
        #endregion

        #region Fields
        private WorkHours _workHours = new WorkHours();
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkDay"/> class.
        /// </summary>
        /// <param name="date">The date.</param>
        public WorkDay(long date)
            : base(date, date)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkDay"/> class.
        /// </summary>
        public WorkDay()
            :base (0,0)
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the working hours.
        /// </summary>
        /// <value>The working hours.</value>
        public WorkHours WorkingHours
        {
            get { return _workHours; }
            set { _workHours = value; }
        }
	
        #endregion

        #region Methods
        public long GetDuration()
        {
            return WorkingHours.GetDuration();
        }
        /// <summary>
        /// Determines whether this instance is working.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is working; otherwise, <c>false</c>.
        /// </returns>
        public bool IsWorking()
        {
            return WorkingHours.GetDuration() != 0;
        }

        #endregion


        #region ICloneable Members

        public object Clone()
        {
            WorkDay retVal = new WorkDay();
            retVal.Start = this.Start;
            retVal.End = this.End;
            retVal.WorkingHours = (WorkHours)this.WorkingHours.Clone();
            return retVal;
        }

        #endregion
    }
}
