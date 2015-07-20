using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Mediachase.Ibn.Calendar
{                 
    static internal class ReccurenceEngine
    {
        private delegate List<DateTime> RruleFrequency(RruleParam param);

        enum Frequence
        {
            Daily,
            Weekly,
            Monthly,
            Yearly
        }

        private class RruleParam
        {
            public List<DateTime> GetRecurringDates()
            {
                if (freqHandler == null)
                    throw new ArgumentNullException("freqHandler");

                return freqHandler(this);

            }

            public RruleFrequency freqHandler = null;

            public DateTime DTStart;
            public DateTime DTEnd;

            public DayOfWeek WkST = DayOfWeek.Monday;

            public int Interval = 1;
            public int Count = 0;
            public int[] Setpos = null; //смещение в совпадениях

            public DayOfWeek[] ByDay;
            public int[] byMonthDay;
            public int[] byYearDay;
            public int[] byMonth;
            public int[] byWeekno;

        };

        static public List<DateTime> GetReccurence(String rrule, DateTime dtStart, 
                                                   DateTime dtEnd)
        {
            //DAILY
            //"FREQ=DAILY; INTERVAL=2;";
            //"FREQ=DAILY;"
            //RRULE:FREQ=DAILY;INTERVAL=10;COUNT=3;

            //WEEKLY
            //FREQ=WEEKLY;COUNT=10
            //FREQ=WEEKLY;
            //FREQ=WEEKLY;INTERVAL=2
            //FREQ=WEEKLY;COUNT=10;BYDAY=TU,TH
            //FREQ=WEEKLY;INTERVAL=2;COUNT=8;WKST=SU;BYDAY=TU,TH

            //MONTHLY
            //FREQ=MONTHLY;COUNT=10;BYDAY=FR;SETPOS=1
            //FREQ=MONTHLY;INTERVAL=2;COUNT=10;BYDAY=SU;SETPOS=1,-1 первое и последнее воскресенье месяца
            //FREQ=MONTHLY;COUNT=6;BYDAY=MO;SETPOS=-2
            //FREQ=MONTHLY;BYMONTHDAY=-3
            //FREQ=MONTHLY;COUNT=10;BYMONTHDAY=2,15
            //FREQ=MONTHLY;COUNT=10;BYMONTHDAY=1,-1
            //FREQ=MONTHLY;INTERVAL=18;COUNT=10;BYMONTHDAY=10,11,12,13,14,15
            //FREQ=MONTHLY;INTERVAL=2;BYDAY=TU
            //FREQ=MONTHLY;BYDAY=FR;BYMONTHDAY=13
            //FREQ=MONTHLY;COUNT=3;BYDAY=TU,WE,TH;BYSETPOS=3
            //FREQ=MONTHLY;BYDAY=MO,TU,WE,TH,FR;BYSETPOS=-2 - пердпослений рабочий день месяца

            //FREQ=MONTHLY;BYDAY=MO;BYSETPOS=-1 - последний понедельник месяца
            //FREQ=MONTHLY;COUNT=9;BYDAY=1MO, 3MO, 5MO; - каждый 2-й понедельник месяца
            //FREQ=MONTHLY;COUNT=9;BYDAY=MO;SETPOS=1,3,5 - Каждый второй понедельник месяцв

            //YEARLY
            //FREQ=YEARLY;BYMONTH=1;BYDAY=SU,MO,TU,WE,TH,FR,SA
            //FREQ=YEARLY;COUNT=10;BYYEARDAY=1,-1
            //FREQ=YEARLY;COUNT=10;BYMONTH=6,7
            //FREQ=YEARLY;INTERVAL=3;COUNT=10;BYYEARDAY=1,100,200
            //FREQ=YEARLY;COUNT=10;BYYEARDAY=366, -366
            //FREQ=YEARLY;BYDAY=MO;SETPOS=20
            //FREQ=YEARLY;BYWEEKNO=20;BYDAY=MO
            //FREQ=YEARLY;BYMONTH=3;BYDAY=TH
            //FREQ=YEARLY;INTERVAL=1;BYMONTH=1;BYDAY=TU;BYMONTHDAY=1,2,3,4,5,6;WEEKNO=1;BYYEARDAY=3

            //String rrule = "FREQ=YEARLY;BYDAY=MO;SETPOS=52";
            RruleParam param = RruleParse(rrule);
            param.DTStart = dtStart;
            param.DTEnd = dtEnd;
            return  param.GetRecurringDates();
        }


        static private List<DateTime> Yearly(RruleParam param)
        {
            return Yearly(param.Count, param.Interval, param.Setpos, param.ByDay, param.WkST,
                          param.byMonthDay, param.byYearDay, param.byMonth, param.byWeekno,
                          param.DTStart, param.DTEnd);
        }

        static private List<DateTime> Yearly(int count, int interval, int[] setPos, DayOfWeek[] byDay,
                                            DayOfWeek wkST, int[] byMonthDay, int[] byYearDay,
                                            int[] byMonth, int[] byWeekno, DateTime dtStart,
                                            DateTime dtFinish)
        {
            List<DateTime> retVal = new List<DateTime>();
            DateTime curDate = dtStart;
            //Invariant
            if ((byMonthDay == null) && (byDay == null) && (byYearDay == null))
                byMonthDay = new int[] { dtStart.Day };

            while (curDate < dtFinish)
            {
                List<DateTime> matchSet = new List<DateTime>();
                List<DateTime> monthlyMatchSet = null;
                List<DateTime> yearDayMatchSet = null;

                DateTime firstDayInYear = new DateTime(curDate.Year, 1, 1);
                DateTime lastDayInYear = new DateTime(curDate.Year, 12, 31);
                int daysInYear = DateTime.DaysInMonth(curDate.Year, 2) > 28 ? 366 : 365;
                //Calculate week num in current year
                List<DateTime> weekNumMatchSet = WeekFast(1, wkST, firstDayInYear,
                                                          lastDayInYear);
                //Week number one of the calendar year is the
                //first week which contains at least four (4) days in that calendar
                //year.
                if (weekNumMatchSet[0].Day > 4)
                {
                    weekNumMatchSet.Add(firstDayInYear);
                    weekNumMatchSet.Sort();
                }

                //YearDay match list
                if (byYearDay != null)
                {
                    yearDayMatchSet = new List<DateTime>();
                    foreach (int yearDay in byYearDay)
                    {
                        if (yearDay < -366 || yearDay > 366 || yearDay == 0)
                            throw new ArgumentException("BYYEARDAY");

                        if (Math.Abs(yearDay) > daysInYear)
                            continue;

                        DateTime toAdd = yearDay < 0 ? lastDayInYear : firstDayInYear;

                        if (toAdd >= dtStart && toAdd <= dtFinish)
                        {
                            //inclusive
                            yearDayMatchSet.Add(toAdd.AddDays(yearDay < 0 ?
                                                              yearDay + 1 :
                                                              yearDay - 1));
                        }
                    }
                }

                //Monthly match list
                if (byMonth != null)
                {
                    monthlyMatchSet = new List<DateTime>();
                    foreach (int month in byMonth)
                    {
                        DateTime firstdayInMonth = new DateTime(curDate.Year, month, 1);
                        DateTime lastDayInMonth = new DateTime(curDate.Year, month, DateTime.DaysInMonth(curDate.Year, month));
                        monthlyMatchSet.AddRange(Monthly(0, 1, null, byMonthDay, byDay,
                                                  firstdayInMonth, lastDayInMonth));
                    }
                }
                else if ((byDay != null) || (byMonthDay != null))
                {
                    monthlyMatchSet = new List<DateTime>();
                    monthlyMatchSet.AddRange(Monthly(0, 1, null, byMonthDay, byDay,
                                                     firstDayInYear, lastDayInYear));
                }


                //join set
                Stack<List<DateTime>> toJoin = new Stack<List<DateTime>>();
                toJoin.Push(monthlyMatchSet);
                toJoin.Push(yearDayMatchSet);
                matchSet = BeginJoin(toJoin);

                if (byWeekno != null)
                {
                    List<DateTime> tmpList = new List<DateTime>();
                    foreach (int weekNo in byWeekno)
                    {
                        if (weekNo > weekNumMatchSet.Count || weekNo == 0)
                            throw new System.ArgumentException("BYWEEKNO");

                        DateTime weekBegin = weekNumMatchSet[weekNo - 1];
                        foreach (DateTime matchDay in matchSet)
                        {
                            if (matchDay >= weekBegin && matchDay < weekBegin.AddDays(7))
                                tmpList.Add(matchDay);
                        }
                    }

                    matchSet = tmpList;
                }

                //SETPOS 
                matchSet = SetPosApply(matchSet, setPos);
                foreach (DateTime toAdd in matchSet)
                {
                    if (toAdd >= dtStart && toAdd <= dtFinish)
                        retVal.Add(toAdd);
                }

                curDate = new DateTime(curDate.Year, 1, 1);
                curDate = curDate.AddYears(interval);
            }

            if (retVal.Count > 0)
            {
                retVal.Sort();
                retVal = CountApply(retVal, count);
            }

            return retVal;
        }



        static private List<DateTime> Monthly(RruleParam param)
        {
            return Monthly(param.Count, param.Interval, param.Setpos, param.byMonthDay, param.ByDay,
                           param.DTStart, param.DTEnd);
        }

        static private List<DateTime> Monthly(int count, int interval, int[] setPos, int[] byMonthDay,
                                             DayOfWeek[] byDay, DateTime dtStart,
                                             DateTime dtFinish)
        {
            List<DateTime> retVal = new List<DateTime>();
            DateTime curDate = dtStart;
            //Invariant
            if ((byMonthDay == null) && (byDay == null))
                byMonthDay = new int[] { dtStart.Day };

            while (curDate < dtFinish)
            {
                List<DateTime> matchSet = new List<DateTime>();
                List<DateTime> weekDayMatchSet = null;
                List<DateTime> monthDayMatchSet = null;

                int dayInMonth = DateTime.DaysInMonth(curDate.Year, curDate.Month);
                //Get weekDays
                if (byDay != null)
                {
                    weekDayMatchSet = new List<DateTime>();
                    DateTime startDate = new DateTime(curDate.Year, curDate.Month, 1);
                    DateTime finishDate = new DateTime(curDate.Year, curDate.Month,
                                                       dayInMonth);
                    foreach (DayOfWeek weekDay in byDay)
                    {
                        weekDayMatchSet.AddRange(WeekFast(1, weekDay,
                                                         startDate, finishDate));
                    }
                }
                //Get month days
                if (byMonthDay != null)
                {
                    monthDayMatchSet = new List<DateTime>();
                    foreach (int monthDay in byMonthDay)
                    {

                        if ((monthDay > dayInMonth) || (monthDay == 0))
                            continue;

                        DateTime toAdd = new DateTime(curDate.Year, curDate.Month,
                                                      monthDay < 0 ? dayInMonth + monthDay + 1
                                                                     : monthDay);
                        monthDayMatchSet.Add(toAdd);
                        //if (toAdd >= dtStart && toAdd <= dtFinish)


                    }
                }
                //Join set
                Stack<List<DateTime>> toJoin = new Stack<List<DateTime>>();
                toJoin.Push(weekDayMatchSet);
                toJoin.Push(monthDayMatchSet);
                matchSet = BeginJoin(toJoin);

                //SETPOS 
                matchSet = SetPosApply(matchSet, setPos);
                foreach (DateTime toAdd in matchSet)
                {
                    if (toAdd >= dtStart && toAdd <= dtFinish)
                        retVal.Add(toAdd);
                }

                curDate = new DateTime(curDate.Year, curDate.Month, 1);
                curDate = curDate.AddMonths(interval);

            }

            if (retVal.Count > 0)
            {
                retVal.Sort();
                retVal = CountApply(retVal, count);
            }

            return retVal;
        }

        static private List<DateTime> Weekly(RruleParam param)
        {
            return Weekly(param.Count, param.Interval, param.ByDay,
                          param.DTStart, param.DTEnd);
        }

        //FREQ=WEEKLY;INTERVAL=2;WKST=SU;BYDAY=MO,WE,FR
        static private List<DateTime> Weekly(int count, int interval, DayOfWeek[] byDay,
                                            DateTime dtStart, DateTime dtFinish)
        {
            List<DateTime> retVal = new List<DateTime>();
            //Invariant
            if (byDay == null)
                byDay = new DayOfWeek[] { dtStart.DayOfWeek };

            foreach (DayOfWeek weekDay in byDay)
            {
                retVal.AddRange(WeekFast(interval, weekDay, dtStart, dtFinish));
            }

            if (retVal.Count > 0)
            {
                retVal.Sort();
                retVal = CountApply(retVal, count);
            }
            return retVal;
        }

        static private List<DateTime> Daily(RruleParam param)
        {
            return Daily(param.Count, param.Interval, param.DTStart, param.DTEnd);
        }

        static private List<DateTime> Daily(int count, int interval, DateTime dtStart, DateTime dtFinish)
        {
            List<DateTime> retVal = new List<DateTime>();
            DateTime curDate = dtStart;

            if (interval <= 0)
                throw new ArgumentException("INTERVAL");

            while (curDate < dtFinish)
            {
                if ((count > 0) && (retVal.Count == count))
                    break;

                retVal.Add(curDate);

                curDate = curDate.AddDays(interval);
            }

            return retVal;
        }

        //Util methods
        static private RruleParam RruleParse(string rrule)
        {
            RruleParam retVal = new RruleParam();
            StringBuilder regExpBld = new StringBuilder();

            regExpBld.Append("(?:FREQ=(?<FREQ>[^;]+)|");
            regExpBld.Append("COUNT=(?<COUNT>[^;]+)|");
            regExpBld.Append("INTERVAL=(?<INTERVAL>[^;]+)|");
            regExpBld.Append("WKST=(?<WKST>[^;]+)|");
            regExpBld.Append("SETPOS=(?<SETPOS>[^;]+)|");
            regExpBld.Append("BYMONTH=(?<BYMONTH>[^;]+)|");
            regExpBld.Append("INTERVAL=(?<INTERVAL>[^;]+)|");
            regExpBld.Append("BYDAY=(?<BYDAY>[^;]+)|");
            regExpBld.Append("BYMONTHDAY=(?<BYMONTHDAY>[^;]+)|");
            regExpBld.Append("BYWEEKNO=(?<BYWEEKNO>[^;]+)|");
            regExpBld.Append("BYYEARDAY=(?<BYYEARDAY>[^;]+))");

            Regex rruleParser = new Regex(regExpBld.ToString());
            MatchCollection matches = rruleParser.Matches(rrule);

            foreach (Match match in matches)
            {
                string capture = match.Groups["FREQ"].Value;
                if (!String.IsNullOrEmpty(capture))
                {
                    if (capture.Equals("DAILY", StringComparison.InvariantCultureIgnoreCase))
                        retVal.freqHandler = Daily;
                    else if (capture.Equals("WEEKLY", StringComparison.InvariantCultureIgnoreCase))
                        retVal.freqHandler = Weekly;
                    else if (capture.Equals("MONTHLY", StringComparison.InvariantCultureIgnoreCase))
                        retVal.freqHandler = Monthly;
                    else if (capture.Equals("YEARLY", StringComparison.InvariantCultureIgnoreCase))
                        retVal.freqHandler = Yearly;
                    else
                        throw new ArgumentException("FREQ");
                }

                capture = match.Groups["COUNT"].Value;
                if (!String.IsNullOrEmpty(capture))
                    retVal.Count = Convert.ToInt32(capture);

                capture = match.Groups["INTERVAL"].Value;
                if (!String.IsNullOrEmpty(capture))
                    retVal.Interval = Convert.ToInt32(capture);

                capture = match.Groups["SETPOS"].Value;
                if (!String.IsNullOrEmpty(capture))
                    retVal.Setpos = ConvertParam<int>(capture);

                capture = match.Groups["WKST"].Value;
                if (!String.IsNullOrEmpty(capture))
                    retVal.WkST = ConvertParam<DayOfWeek>(capture)[0];

                capture = match.Groups["BYMONTH"].Value;
                if (!String.IsNullOrEmpty(capture))
                    retVal.byMonth = ConvertParam<int>(capture);

                capture = match.Groups["BYDAY"].Value;
                if (!String.IsNullOrEmpty(capture))
                    retVal.ByDay = ConvertParam<DayOfWeek>(capture);

                capture = match.Groups["BYMONTHDAY"].Value;
                if (!String.IsNullOrEmpty(capture))
                    retVal.byMonthDay = ConvertParam<int>(capture);

                capture = match.Groups["BYWEEKNO"].Value;
                if (!String.IsNullOrEmpty(capture))
                    retVal.byWeekno = ConvertParam<int>(capture);

                capture = match.Groups["BYYEARDAY"].Value;
                if (!String.IsNullOrEmpty(capture))
                    retVal.byYearDay = ConvertParam<int>(capture);

            }

            return retVal;
        }

        static private T[] ConvertParam<T>(string str)
        {
            List<T> retVal = new List<T>();
            string[] values = str.Split(new Char[] { ',' });
            foreach (string value in values)
            {
                retVal.Add(ConvertVal<T>(value.Trim()));
            }

            return retVal.ToArray();
        }

        static private T ConvertVal<T>(string str)
        {
            T retVal = default(T);
            if (typeof(T) == typeof(Int32))
                retVal = (T)String2Int(str);
            else if (typeof(T) == typeof(DayOfWeek))
                retVal = (T)String2DayOfWeek(str);

            return retVal;
        }

        static private object String2Int(string str)
        {
            return Convert.ToInt32(str);
        }

        static private object String2DayOfWeek(string str)
        {

            DayOfWeek retVal;
            if (str.Equals("MO", StringComparison.InvariantCultureIgnoreCase))
                retVal = DayOfWeek.Monday;
            else if (str.Equals("TU", StringComparison.InvariantCultureIgnoreCase))
                retVal = DayOfWeek.Tuesday;
            else if (str.Equals("WE", StringComparison.InvariantCultureIgnoreCase))
                retVal = DayOfWeek.Wednesday;
            else if (str.Equals("TH", StringComparison.InvariantCultureIgnoreCase))
                retVal = DayOfWeek.Thursday;
            else if (str.Equals("FR", StringComparison.InvariantCultureIgnoreCase))
                retVal = DayOfWeek.Friday;
            else if (str.Equals("SA", StringComparison.InvariantCultureIgnoreCase))
                retVal = DayOfWeek.Saturday;
            else if (str.Equals("SU", StringComparison.InvariantCultureIgnoreCase))
                retVal = DayOfWeek.Sunday;
            else
                throw new ArgumentException("BYDAY");

            return retVal;
        }

        static private List<DateTime> WeekFast(int interval, DayOfWeek byDay,
                                               DateTime dtStart, DateTime dtFinish)
        {
            List<DateTime> retVal = new List<DateTime>();
            DayOfWeek dwStart = dtStart.DayOfWeek;
            int repeatDay = GetWeekDayOffset(dwStart, byDay);
            while (dtStart.AddDays(repeatDay) <= dtFinish)
            {
                retVal.Add(dtStart.AddDays(repeatDay));
                repeatDay += 7 * interval;
            }

            return retVal;
        }
        static private int GetWeekDayOffset(DayOfWeek fromWD, DayOfWeek toWD)
        {
            int retVal = toWD >= fromWD ? (int)toWD - (int)fromWD : 7 - (int)fromWD + (int)toWD;
            return retVal;
        }

        static private List<DateTime> CountApply(List<DateTime> matchSet, int count)
        {
            if ((matchSet.Count > 0) && (count > 0))
            {
                count = count > matchSet.Count ? matchSet.Count : count;
                matchSet.RemoveRange(count, matchSet.Count - count);
            }
            return matchSet;
        }

        static private List<DateTime> SetPosApply(List<DateTime> matchSet,
                                                int[] setPos)
        {
            if (setPos == null)
                return matchSet;

            List<DateTime> retVal = new List<DateTime>();
            matchSet.Sort();

            foreach (int pos in setPos)
            {
                if (Math.Abs(pos) > matchSet.Count)
                    continue;

                if (matchSet.Count != 0)
                {
                    if (pos < 0)
                    {
                        matchSet.Reverse();
                    }
                    retVal.Add(matchSet[Math.Abs(pos) - 1]);
                }
            }

            return retVal;
        }
        static private List<DateTime> BeginJoin(Stack<List<DateTime>> compItems)
        {
            if (compItems.Count == 0)
                return null;

            List<DateTime> retVal = compItems.Pop();

            while (compItems.Count != 0)
            {
                retVal = JoinPair(retVal, compItems.Pop());
            }

            return retVal;
        }

        static private List<DateTime> JoinPair(List<DateTime> left, List<DateTime> right)
        {
            if ((right == null) && ((left == null)))
                return new List<DateTime>();
            else if ((right == null) && (left != null))
                return left;
            else if ((left == null) && (right != null))
                return right;
            else if (left == right)
                return left;

            List<DateTime> retVal = new List<DateTime>();
            foreach (DateTime leftItem in left)
            {
                if (right.Contains(leftItem))
                {
                    retVal.Add(leftItem);
                    right.Remove(leftItem);
                }
            }

            return retVal;
        }
    }
}
