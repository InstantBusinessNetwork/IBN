using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Assignment.Contour;
using ProjectBusinessUtil.Time;
using ProjectBusinessUtil.Algorithm;
using ProjectBusinessUtil.Calendar;
using ProjectBusinessUtil.Assignment.Functor;

namespace ProjectBusinessUtil.Services
{
    /// <summary>
    /// Represents Service for get MsProject like TimePhased intervals.
    /// </summary>
    public static class TimePhasedService
    {
        #region Const
        #endregion

        #region Fields
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="TimePhasedService"/> class.
        /// </summary>
        #endregion

        #region Properties
        #endregion

        #region Methods
        /// <summary>
        /// Gets the contour.
        /// </summary>
        /// <param name="timePhases">The time phases.</param>
        /// <returns></returns>
        public static AbstractContour GetContour(TimePhasedDataType[] timePhases)
        {
            ContourFactory factory = new ContourFactory();
            return factory.Create<PersonalContour>(timePhases);
        }

        /// <summary>
        /// Gets the time phase data.
        /// </summary>
        /// <param name="assignment">The assignment.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IEnumerable<TimePhasedDataType> GetTimePhaseData(Assignment.Assignment assignment, TimePhasedDataType.TimePhaseType type)
        {
			if(assignment == null)
				throw new ArgumentException("assignment");

			//определяем величину дискретизации для генератора интервалов.
			//Если ее определить слишком маленькую то будет много TimePhases елементов
			//Для плоского распределения трудозатрат использовать ДЕНЬ для нелинейных функций использовать ЧАС
			long groupIntervalValue = assignment.CurrentContour.ContourType == ContourTypes.Flat ? CalendarHelper.MilisPerDay() : CalendarHelper.MilisPerHour();

            long start = type == TimePhasedDataType.TimePhaseType.AssignmentActualWork ? assignment.Start : assignment.Stop;
            long stop = type == TimePhasedDataType.TimePhaseType.AssignmentActualWork ? assignment.Stop : assignment.End;
            Query<Interval> query = new Query<Interval>();
            AssignmentBaseFunctor<double> workFunctor = assignment.GetWorkFunctor();
			
            GroupingIntervalGenerator groupGen = new GroupingIntervalGenerator(start, stop, groupIntervalValue, 
                                                                              workFunctor.CountourGenerator);
            TimePhaseDataGetter timePhaseGetter = new TimePhaseDataGetter(type, TimePhasedDataType.TimePhaseUnit.Day,
                                                                          TimePhasedDataType.TimePhaseUnit.Minute, workFunctor);
            WhereInRangePredicate whereInRange = new WhereInRangePredicate(start, stop);

            query.Select(timePhaseGetter).From(groupGen).Where(whereInRange.Evaluate).Execute();

            return timePhaseGetter.Value;
        }

        #endregion


    }
}
