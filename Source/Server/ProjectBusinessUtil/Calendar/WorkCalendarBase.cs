using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Calendar
{
    public abstract class WorkCalendarBase 
    {
        abstract public List<WorkDay> DayException { get; }
        abstract public WorkWeek WorkingWeek { get; }

        abstract public long AdjustInCalendar(long date, bool useSooner);
        abstract public long AddDuration(long date, long duration, bool useSooner);
        abstract public long SubstractDates(long laterDate, long earlierDate, bool useSooner);
    }
}
