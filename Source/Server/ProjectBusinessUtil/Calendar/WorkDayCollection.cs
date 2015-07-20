using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Calendar
{
    /// <summary>
    /// Represents working day collection on calendar.
    /// </summary>
    public class WorkDayCollection : IEnumerable<WorkDay>
    {

        #region Const
        #endregion

        #region Fields
        WorkDayIterator _iter;

        #endregion

        #region .Ctor
        public WorkDayCollection(WorkCalendar calendar, long startDate, bool reverse)
        {
            _iter = new WorkDayIterator(calendar, startDate, reverse);
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion


        #region IEnumerable<WorkDay> Members

        public IEnumerator<WorkDay> GetEnumerator()
        {
            return _iter;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
