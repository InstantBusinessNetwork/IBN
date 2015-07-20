using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Functor;
using ProjectBusinessUtil.Time;
using ProjectBusinessUtil.Assignment.Functor;
using ProjectBusinessUtil.Calendar;

namespace ProjectBusinessUtil.Services
{
    /// <summary>
    /// Represents functor uses for creation MsProject TimePhase .
    /// </summary>
    public class TimePhaseDataGetter : CalculationFunctor<Interval>, IHasValue<IEnumerable<TimePhasedDataType>>
    {
        #region Const
        #endregion

        #region Fields
        private TimePhasedDataType.TimePhaseType _type;
        private AssignmentBaseFunctor<double> _functor;

        TimePhasedDataType.TimePhaseUnit _maxGroupUnit;
        TimePhasedDataType.TimePhaseUnit _minGroupUnit;
        List<TimePhasedDataType> _value;
        TimePhasedDataType _groupingPhase = null;
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="TimePhaseDataGetter"/> class.
        /// </summary>
        public TimePhaseDataGetter(TimePhasedDataType.TimePhaseType type, TimePhasedDataType.TimePhaseUnit maxGroupUnit,
                                   TimePhasedDataType.TimePhaseUnit minGroupUnit, AssignmentBaseFunctor<double> functor)
        {
            _type = type;
            _functor = functor;
            _minGroupUnit = minGroupUnit;
            _maxGroupUnit = maxGroupUnit;
            _value = new List<TimePhasedDataType>();
        }
        #endregion

        #region Properties
        
        #endregion

        #region Methods
        /// <summary>
        /// Decrements the unit.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns></returns>
        private static TimePhasedDataType.TimePhaseUnit DecrementUnit(TimePhasedDataType.TimePhaseUnit unit)
        {

            return (TimePhasedDataType.TimePhaseUnit)((int)unit - 1);
        }
        /// <summary>
        /// Increments the unit.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns></returns>
        private static TimePhasedDataType.TimePhaseUnit IncrementUnit(TimePhasedDataType.TimePhaseUnit unit)
        {

            return (TimePhasedDataType.TimePhaseUnit)((int)unit + 1);
        }

        /// <summary>
        /// Unit2durations the specified unit.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns></returns>
        private static long Unit2duration(TimePhasedDataType.TimePhaseUnit unit)
        {
            long retVal = 0;
            switch (unit)
            {
                case TimePhasedDataType.TimePhaseUnit.Hours:
                    retVal = CalendarHelper.MilisPerHour();
                    break;
                case TimePhasedDataType.TimePhaseUnit.Minute:
                    retVal = CalendarHelper.MilisPerMinute();
                    break;
                case TimePhasedDataType.TimePhaseUnit.Day:
                    retVal = CalendarHelper.MilisPerDay();
                    break;
                case TimePhasedDataType.TimePhaseUnit.Week:
                    retVal = CalendarHelper.MilisPerWeek();
                    break;
            }

            return retVal;
        }
        /// <summary>
        /// Duration2s the unit.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns></returns>
        private static TimePhasedDataType.TimePhaseUnit Duration2Unit(long duration)
        {
            TimePhasedDataType.TimePhaseUnit retVal = TimePhasedDataType.TimePhaseUnit.Minute;

            if (duration / CalendarHelper.MilisPerYear() > 0)
                retVal = TimePhasedDataType.TimePhaseUnit.Year;
            else if (duration / CalendarHelper.MilisPerMonth() > 0)
                retVal = TimePhasedDataType.TimePhaseUnit.Year;
            else if (duration / CalendarHelper.MilisPerWeek() > 0)
                retVal = TimePhasedDataType.TimePhaseUnit.Mounth;
            else if (duration / CalendarHelper.MilisPerDay() > 0)
                retVal = TimePhasedDataType.TimePhaseUnit.Week;
            else if (duration / CalendarHelper.MilisPerHour() > 0)
                retVal = TimePhasedDataType.TimePhaseUnit.Day;
            else if (duration / CalendarHelper.MilisPerMinute() > 0)
                retVal = TimePhasedDataType.TimePhaseUnit.Hours;

            return retVal;
        }

        /// <summary>
        /// Groups the by work create interval breaking not work interval.
        /// </summary>
        /// <param name="groupPhase">The group phase.</param>
        /// <returns></returns>
        private TimePhasedDataType[] GroupByWork(TimePhasedDataType[] groupPhase)
        {
            List<TimePhasedDataType> retVal = new List<TimePhasedDataType>();
            TimePhasedDataType group = null;
            TimePhasedDataType prevPhase = null;
           
            foreach(TimePhasedDataType phase in groupPhase)
            {
                if (group == null || (phase.Value == 0 && prevPhase.Value != 0) 
                    || (phase.Value != 0 && prevPhase.Value == 0))
                {
                    group = new TimePhasedDataType();
                    group.Start = phase.Start;
                    group.Finish = phase.Finish;
                    group.Value = phase.Value;
                    retVal.Add(group);
                    prevPhase = phase;
                    continue;
                }

                group.Finish = phase.Finish;
                group.Value += phase.Value;

                prevPhase = phase;
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Incrementals the group.
        /// </summary>
        /// <param name="phase">The phase.</param>
        /// <param name="groupUnit">The group unit.</param>
        /// <returns></returns>
        private TimePhasedDataType[] IncrementalGroup(TimePhasedDataType phase, TimePhasedDataType.TimePhaseUnit groupUnit)
        {
            List<TimePhasedDataType> retVal = new List<TimePhasedDataType>();

            long intervalDuration = phase.Finish - phase.Start;
            long wholePartCount = intervalDuration / Unit2duration(groupUnit);
            long partialPart = intervalDuration - wholePartCount * Unit2duration(groupUnit);
            if (wholePartCount > 0)
            {
                for (int i = 0; i < wholePartCount; i++)
                {
                    TimePhasedDataType newPhase = new TimePhasedDataType();
                    long start = phase.Start + i * Unit2duration(groupUnit);
                    newPhase.Start = start;
                    newPhase.Finish = start + Unit2duration(groupUnit);
                    newPhase.Unit = groupUnit;
                    newPhase.Value = (long)((double)Unit2duration(groupUnit) / intervalDuration * phase.Value);
                    retVal.Add(newPhase);
                }
            }

            if (partialPart != 0 && groupUnit != _minGroupUnit)
            {
                TimePhasedDataType newPhase = new TimePhasedDataType();
                newPhase.Start = phase.Finish - partialPart;
                newPhase.Finish = phase.Finish;
                newPhase.Value = (long)((double)partialPart / intervalDuration * phase.Value);
                retVal.AddRange(IncrementalGroup(newPhase, DecrementUnit(groupUnit)));
            }

            return retVal.ToArray();
        }

        #endregion



        #region IFunctor<Interval> Members

        /// <summary>
        /// Executes the specified param.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <returns></returns>
        public override object Execute(Interval param)
        {
            double workPerInterval = _functor.Value;
            //Call aggregated work functor 
            _functor.Execute(param);
            workPerInterval = _functor.Value - workPerInterval;

            //first call or breake interval
            _groupingPhase = new TimePhasedDataType();
            _groupingPhase.Start = param.Start;
            _groupingPhase.Finish = param.End;
            _groupingPhase.Value = (long)workPerInterval;
            _value.Add(_groupingPhase);
           
            return this;
        }

        #endregion

        public override void Initialize()
        {
            
        }

        #region IHasValue<IEnumerable<TimePhasedDataType>> Members

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public IEnumerable<TimePhasedDataType> Value
        {
            get
            {
                foreach (TimePhasedDataType value in GroupByWork(_value.ToArray()))
                {
                    foreach (TimePhasedDataType phase in IncrementalGroup(value, _maxGroupUnit))
                    {
                        phase.Type =(int)_type;
                        yield return phase;
                    }
                }
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }
}
