using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Calendar;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace ProjectBusinessUtil.XmlSerialization
{
    /// <summary>
    /// Represents : Serializable calendar type supported MsProject MSPDI format.
    /// </summary>
    public class SerializableCalendar : WorkCalendar
    {
        #region Const
        #endregion

        #region Fields
        CalendarType _MSPDIcalendar;
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableCalendar"/> class.
        /// </summary>
        public SerializableCalendar(string xmlCalendar)
        {
            Initialize(xmlCalendar);
        }
        #endregion

        #region Properties
        protected CalendarType InnerCalendar
        {
            get { return _MSPDIcalendar; }
        }
       
        #endregion

        #region Methods

        /// <summary>
        /// Loads the calendar.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private void Initialize(string xmlCalendar)
        {
            XmlSerializer xmlsz = new XmlSerializer(typeof(CalendarType));
            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xmlCalendar)))
            {
                CalendarType calendarType = (CalendarType)xmlsz.Deserialize(ms);
                _MSPDIcalendar = calendarType;

                //Nothing todo
                if (calendarType.WeekDays == null)
                    return;

                foreach (CalendarWeekDay weekDay in calendarType.WeekDays)
                {
                    WorkDay workDay = new WorkDay();

                    int intervalNum = 0;
                    if (weekDay.DayWorking == true)
                    {
                        foreach (CalendarWeekDayWorkingTimesWorkingTime workingTimes in weekDay.WorkingTimes.Items)
                        {
                            workDay.WorkingHours.AddInterval(intervalNum++, CalendarHelper.Tick2Milis(workingTimes.FromTime.Ticks),
                                                  CalendarHelper.Tick2Milis(workingTimes.ToTime.Ticks));
                        }
                    }

                    if (weekDay.DayType == CalendarWeekDayDayType.Exception)
                    {
                        workDay.Start = CalendarHelper.Tick2Milis(weekDay.TimePeriod.FromDate.Ticks);
                        workDay.End = CalendarHelper.Tick2Milis(weekDay.TimePeriod.ToDate.Ticks);
                        this.DayException.Add(workDay);
                        continue;
                    }
                    //Set week work day
                    this.WorkingWeek.WeekDays[(int)weekDay.DayType - 1] = workDay;
                }

            }
        }
        #endregion
    }
}
