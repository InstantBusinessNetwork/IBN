using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Calendar
{
    /// <summary>
    /// Represents working calendar.
    /// </summary>
    public class WorkCalendar : WorkCalendarBase
    {
        #region Const
        #endregion

        #region Fields
        WorkWeek _workingWeek = new WorkWeek();
        List<WorkDay> _dayException = new List<WorkDay>();
        WorkCalendar _baseCalendar = null;
        #endregion

        #region .Ctor
        public WorkCalendar()
        {
        }
        #endregion

        #region Properties
        public WorkCalendar BaseCalendar
        {
            get { return _baseCalendar; }
            set { _baseCalendar = value; }
        }
	
        /// <summary>
        /// Gets the working week.
        /// </summary>
        /// <value>The working week.</value>
        public override WorkWeek WorkingWeek
        {
            get { return _workingWeek; }
        }

        /// <summary>
        /// Gets the day exception.
        /// </summary>
        /// <value>The day exception.</value>
        public override List<WorkDay> DayException
        {
            get { return _dayException; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adjusts the in calendar.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="useSooner">if set to <c>true</c> [use sooner].</param>
        /// <returns></returns>
        public override long AdjustInCalendar(long date, bool useSooner)
        {
            long retVal = 0;
            if (useSooner)
            {
                long backOne = AddDuration(date, CalendarHelper.MilisPerMinute() * -1, useSooner);
                retVal = AddDuration(backOne, CalendarHelper.MilisPerMinute(), useSooner);
            }
            else
            {
                long aheadOne = AddDuration(date, CalendarHelper.MilisPerMinute(), useSooner);
                retVal = AddDuration(date, CalendarHelper.MilisPerMinute() * -1, useSooner);
            }

            return retVal;
        }

        /// <summary>
        /// Adds the duration.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="useSooner">if set to <c>true</c> [use sooner].</param>
        /// <returns></returns>
        public override long AddDuration(long date, long duration, bool useSooner)
        {
            long retVal = 0;
            bool reverse = duration < 0;
            duration = duration < 0 ? duration * -1 : duration;
            foreach (WorkDay workDay in new WorkDayCollection(this, date, reverse))
            {
                long time = 0;
                //1. First day process
                if (date > workDay.Start)
                {
                    time = date - workDay.Start;
                    long workAt = workDay.WorkingHours.CalcWorkAtTime(time);
                    //in same day
                    duration -= workAt;
                    if (duration >= 0)
                        continue;
                }

                if (duration >= 0)
                {
                    //3. Whole day process
                    duration -= workDay.WorkingHours.GetDuration();
                }

                //2. Last day process
                if (duration < 0)
                {
                    //if (reverse)
                    //    time = workDay.WorkingHours.CalcWorkAtTime(duration);
                    //else
                    time = workDay.WorkingHours.CalcTimeRemainingWork(duration);

                    retVal = workDay.Start + time;
                    break;
                }

            }

            return retVal;
        }
        /// <summary>
        /// Subtracts the duration.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="useSooner">if set to <c>true</c> [use sooner].</param>
        /// <returns></returns>
        public override long SubstractDates(long laterDate, long earlierDate, bool elapsed)
        {
            // if later is before earlier swap the dates.  The value of swap is tested later and sign is reversed if it is used
            long swap = 0;
            long duration = 0;

            if (laterDate < earlierDate)
            {
                swap = earlierDate;
                earlierDate = laterDate;
                laterDate = swap;
            }
            if (elapsed == true)
                return laterDate - earlierDate;

            foreach (WorkDay workDay in new WorkDayCollection(this, earlierDate, false))
            {
                long time = 0;
                //1. First day process
                if (earlierDate > workDay.Start)
                {
                    time = earlierDate - workDay.Start;
                    duration += workDay.WorkingHours.CalcWorkAtTime(time);
                    //if earlier and later in same day
                    if (workDay.End >= laterDate)
                    {
                        time = laterDate - workDay.Start;
                        duration -= workDay.WorkingHours.CalcWorkAtTime(time);
                        break;
                    }
               
                    continue;
                }

                //3. Whole day process
                duration += workDay.WorkingHours.GetDuration();

                //2. Last day process
                if (workDay.End >= laterDate)
                {
                    time = laterDate - workDay.Start;
                    duration -= workDay.WorkingHours.CalcWorkAtTime(time);
                    break;
                }
            }

            return duration;
        }
        #endregion


    }
}
