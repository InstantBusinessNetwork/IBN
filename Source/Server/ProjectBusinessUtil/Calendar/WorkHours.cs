using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Calendar
{
    /// <summary>
    /// Represents working hours.
    /// </summary>
    public class WorkHours : ICloneable
    {
        #region Const
        #endregion

        #region Fields
        WorkRange[] _workRanges = new WorkRange[CalendarSettings.CalendarIntervals];
        #endregion

        #region .Ctor
        #endregion

        #region Properties

        /// <summary>
        /// Gets the work ranges.
        /// </summary>
        /// <value>The work ranges.</value>
        public WorkRange[] WorkRanges
        {
            get { return _workRanges; }
        }
	    #endregion

        #region Methods
        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <returns></returns>
        public long GetDuration()
        {
            long retVal = 0;
            foreach(WorkRange range in _workRanges)
            {
                if(range != null)
                {
                    retVal += range.GetDuration();
                }
                
            }

            return retVal;
        }


        /// <summary>
        /// Calcs the work at time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        public long CalcWorkAtTime(long time)
        {
            long retVal = 0;
            
            //Before
            if (time < 0)
                return GetDuration() - CalcWorkAtTime(time * -1);
            if (time > 0)
            {
                //After
                for (int i = 0; i < WorkRanges.Length; i++)
                {
                    WorkRange range = WorkRanges[i];
                    if (range != null)
                    {
                        if (range.End > time)
                            retVal += (range.End - Math.Max(time, range.Start));
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Calcs the time remaining work.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns></returns>
        public long CalcTimeRemainingWork(long duration)
        {
            long retVal = 0;
            long work = 0;
            duration = duration < 0 ? duration * -1 : duration;

            for (int i = WorkRanges.Length - 1; i >= 0; i--)
            {
                WorkRange range = WorkRanges[i];
                if (range != null)
                {
                    work += range.GetDuration();
                    if (work > duration)
                    {
                        retVal = range.Start + (work - duration);
                        //if (retVal == range.Start && i != 0 && WorkRanges[i - 1] != null)
                        //{
                           
                        // //   retVal = CalcTimeAtWork(duration);
                        //}
                        break;
                    }
                    else if(work == duration)
                    {
                        //retVal = CalcTimeAtWork(range.Start + (work - duration));
                        retVal = range.Start;
                        break;
                    }
                }
            }

            return retVal;
        }
        /// <summary>
        /// Calcs the time at work.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns></returns>
        public long CalcTimeAtWork(long duration)
        {
            long retVal = 0;
            long work = 0;
            duration = duration < 0 ? duration * -1 : duration;

            for (int i = 0; i < WorkRanges.Length; i++ )
            {
                WorkRange range = WorkRanges[i];
                if (range != null)
                {
                    work += range.GetDuration();
                    if (work > duration)
                    {
                        retVal = range.End - (work - duration);
                        break;
                    }
                    else if(work == duration)
                    {
                        retVal = range.End;
                        break;
                    }
                }
            }

            return retVal;
        }
        /// <summary>
        /// Sets the interval.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public void AddInterval(int number, long start, long end)
        {
            //If end time is 0, it is treated as midnight the next day
           if(CalendarHelper.HourFromMilis(end) == 0 && CalendarHelper.MinuteFromMilis(end) == 0)
           {
               end = CalendarHelper.MilisPerHour() * 24 + end;
           }

           WorkRanges[number] = new WorkRange(start, end, false);
        }
        #endregion



        #region ICloneable Members

        public object Clone()
        {
            WorkHours retVal = new WorkHours();
            for(int i = 0; i < WorkRanges.Length; i++)
            {
                if(WorkRanges[i] != null)
                {
                    WorkRange range = new WorkRange(WorkRanges[i].Start, WorkRanges[i].End, WorkRanges[i].OverTime);

                    retVal.WorkRanges[i] = range;
                }
            }

            return retVal;
        }

        #endregion
    }
}
