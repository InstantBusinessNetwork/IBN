using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using ProjectBusinessUtil.Time;

namespace ProjectBusinessUtil.Calendar
{
    /// <summary>
    /// Represents calendar work day iteration.
    /// </summary>
    internal class WorkDayIterator : IEnumerator<WorkDay>
    {
        #region Const
        #endregion

        #region Fields
        WorkCalendar _calendar;
        WorkDay _current;
        long _startPoint;
        int _step = 1;
        bool _reverse = false;
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarIterator"/> class.
        /// </summary>
        public WorkDayIterator(WorkCalendar calendar, long startDate, bool reverse)
        {
            _calendar = calendar;
            _reverse = reverse;
            _step = reverse ? -1 : 1;
            _startPoint = startDate;
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        /// <summary>
        /// Sets the curent.
        /// </summary>
        /// <param name="date">The date.</param>
        private void SetCurent(long date)
        {
            long intervalStart = CalendarHelper.DayOf(date);
            Interval interval = new Interval(intervalStart, intervalStart + GetStepAmount());

            if (GetIfExistException(interval, out _current) == false)
            {
                //First get workDay working resource calendar 
                _current = _calendar.WorkingWeek.WeekDays[CalendarHelper.DayOfWeek(intervalStart)];
                //If resource calendar not set and calendar have base calendar get work day from him
                if (_current == null)
                {
                    if (_calendar.BaseCalendar != null)
                    {
                        _current = (WorkDay)_calendar.BaseCalendar.WorkingWeek.WeekDays[CalendarHelper.DayOfWeek(intervalStart)].Clone();
                    }
                    else
                    {
                        throw new Exception("Invalid work day");
                    }
                }

                _current.Start = interval.Start;
                _current.End = interval.End;
            }
        }

        /// <summary>
        /// Gets if exist exception.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <param name="outException">The out exception.</param>
        /// <returns></returns>
        private bool GetIfExistException(Interval interval, out WorkDay outException)
        {
            bool retVal = false;
            outException = null;
            List<WorkDay> joinedException = _calendar.DayException;
            if (_calendar.BaseCalendar != null)
            {
                joinedException = JoinException(_calendar.BaseCalendar.DayException, _calendar.DayException);
            }

            foreach(WorkDay exception in joinedException)
            {
                if(Interval.IntersectInRange(exception, interval))
                {
                    outException = (WorkDay)exception.Clone();
                    outException.Start = interval.Start;
                    outException.End = interval.End;
                    retVal = true;
                    break;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Joins the exception.
        /// </summary>
        /// <param name="baseException">The base exception.</param>
        /// <param name="derivedException">The derived exception.</param>
        /// <returns></returns>
        private static List<WorkDay> JoinException(List<WorkDay> baseException, List<WorkDay> derivedException)
        {

            List<WorkDay> retVal = new List<WorkDay>();
            if ((derivedException.Count == 0) && ((derivedException.Count == 0)))
                return retVal;
            else if ((derivedException.Count == 0) && (baseException.Count != 0))
                return baseException;
            else if ((baseException.Count == 0) && (derivedException.Count != 0))
                return derivedException;
            else if (baseException == derivedException)
                return baseException;

            List<WorkDay> baseList = new List<WorkDay>((WorkDay[])baseException.ToArray().Clone());
            
            foreach (WorkDay derivedItem in derivedException)
            {
                WorkDay toRemove = null;
                foreach(WorkDay baseItem in baseList)
                {
                    if (Interval.IntersectInRange(derivedItem, baseItem))
                    {
                        toRemove = baseItem;
                        break;
                    }
                }

                retVal.Add((WorkDay)derivedItem.Clone());

                if (toRemove != null)
                    baseList.Remove(toRemove);
            }

            retVal.AddRange(baseList);

            return retVal;
        }
        /// <summary>
        /// Gets the step amount.
        /// </summary>
        /// <returns></returns>
        private long GetStepAmount()
        {
            return _step * CalendarHelper.MilisPerDay();
        }
        #endregion


        #region IEnumerator<long> Members

        public WorkDay Current
        {
            get { return _current; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region IEnumerator Members

        public bool MoveNext()
        {
            long nextDay = _startPoint;
            //first call
            if(_current != null)
            {
                nextDay = _current.Start + GetStepAmount();
            }

            SetCurent(nextDay);
            return true;
        }

        public void Reset()
        {
            SetCurent(_startPoint);
        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }
}
