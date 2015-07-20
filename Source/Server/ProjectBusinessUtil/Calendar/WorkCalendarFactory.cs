using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Common;

namespace ProjectBusinessUtil.Calendar
{
    public class WorkCalendarFactory : AbstractFactory, IFactoryMethod<WorkCalendar>, IFactoryMethod<DefaultCalendar>
    {

        #region IFactoryMethod<WorkCalendar> Members

        WorkCalendar IFactoryMethod<WorkCalendar>.Create(object obj)
        {
            return new WorkCalendar();
        }

        #endregion

        #region IFactoryMethod<DefaultCalendar> Members

        DefaultCalendar IFactoryMethod<DefaultCalendar>.Create(object obj)
        {
            DefaultCalendar retVal = new DefaultCalendar();
            WorkDay workingDay = new WorkDay();
            WorkDay noWorkingDay = new WorkDay();
            workingDay.WorkingHours.AddInterval(0, CalendarHelper.MilisPerHour() * 9, CalendarHelper.MilisPerHour() * 13);
            workingDay.WorkingHours.AddInterval(1, CalendarHelper.MilisPerHour()* 14, CalendarHelper.MilisPerHour() * 18);
            retVal.WorkingWeek.SetWeekDays(workingDay);
            retVal.WorkingWeek.SetWeekEndDays(noWorkingDay);
            return retVal;
        }

        #endregion
    }
}
