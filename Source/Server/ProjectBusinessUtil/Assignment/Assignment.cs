using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Calendar;
using ProjectBusinessUtil.Assignment.Contour;
using ProjectBusinessUtil.Sheduling;
using ProjectBusinessUtil.Time;
using ProjectBusinessUtil.Algorithm;
using ProjectBusinessUtil.Assignment.Functor;
using ProjectBusinessUtil.Functor;
using System.Xml;

namespace ProjectBusinessUtil.Assignment
{
    /// <summary>
    /// Represents .
    /// </summary>
    public class Assignment
    {
        #region Const
        #endregion

        #region Fields
        Task.Task _task;
        AbstractContour _contourBucket;
        WorkCalendarBase _workingCalendar;
        IDelayable _delay;

        long _duration;
        double _units;
        double _percentComplete;
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="Assignment"/> class.
        /// </summary>
        public Assignment(Task.Task task, WorkCalendarBase workCalendar, AbstractContour contourBucket, double units, long delay)
        {
            _task = task;
            _contourBucket = contourBucket;
            _units = units;
            _workingCalendar = workCalendar;
            _delay = new DelayImpl(delay);
            
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public long Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        /// <summary>
        /// Gets the working calendar.
        /// </summary>
        /// <value>The working calendar.</value>
        public WorkCalendarBase WorkingCalendar
        {
            get { return _workingCalendar; }
        }

        /// <summary>
        /// Gets the current contour.
        /// </summary>
        /// <value>The current contour.</value>
        public AbstractContour CurrentContour
        {
            get { return _contourBucket; }
        }
        /// <summary>
        /// Gets the units.
        /// </summary>
        /// <value>The units.</value>
        public double Units
        {
            get { return _units; }
        }

        /// <summary>
        /// Gets or sets the percent complete.
        /// </summary>
        /// <value>The percent complete.</value>
        public double PercentComplete
        {
            get { return _percentComplete; }
            set { 
                    _percentComplete = value > 1.0 ? 1.0 : value; 
                }
        }

        /// <summary>
        /// Gets or sets the delay.
        /// </summary>
        /// <value>The delay.</value>
        public long Delay
        {
            get { return _delay.CalcTotalDelay(); }
            set 
               {
                   _delay.Delay = value; 
               }
        }

        /// <summary>
        /// Gets the actual start.
        /// </summary>
        /// <value>The actual start.</value>
        public long ActualStart
        {
            get { return Start; }
        }

        /// <summary>
        /// Gets or sets the Assignment start date.
        /// </summary>
        /// <value>The start.</value>
        public long Start
        {
            get
            {
                long start = WorkingCalendar.AddDuration(GetTaskStart(), 0, false);
                if (Delay > 0)
                    start = WorkingCalendar.AddDuration(start, Delay, false); // use later time

                return start;
            }
            set
            {
                if (value > GetTaskStart())
                {
                    Delay = WorkingCalendar.SubstractDates(value, GetTaskStart(), false);
                }
            }
        }

        /// <summary>
        /// Gets or sets the stop.
        /// </summary>
        /// <value>The stop.</value>
        public long Stop
        {
            get
            {
                return WorkingCalendar.AddDuration(Start, ActualDuration, true); // use earlier date
            }

            set
            {
                if (value < Start) // if before start ignore
                    return;

                long actualDuration = WorkingCalendar.SubstractDates(value, Start, false);
                if (Duration != 0)
                    PercentComplete = (((double)actualDuration) / Duration);
            }
        }

        /// <summary>
        /// Gets the end.
        /// </summary>
        /// <value>The end.</value>
        public long End
        {
            get 
              {
				return WorkingCalendar.AddDuration(Start, Duration, true);
		      }
              set
              {
                  Duration = WorkingCalendar.SubstractDates(Start, value , false);
              }
        }


        /// <summary>
        /// Gets or sets the duration of the remaining.
        /// </summary>
        /// <value>The duration of the remaining.</value>
        public long RemainingDuration
        {
            get
            {
                return (long)(Duration * (1 - PercentComplete));
            }

            set
            {
                PercentComplete = (1 - ((double)(RemainingDuration / Duration)));
            }
        }

        /// <summary>
        /// Gets or sets the actual duration of assignment calculated and set only percentCompleted.
        /// </summary>
        /// <value>The actual duration.</value>
        public long ActualDuration
        {
            get
            {
                return (long)(Duration * PercentComplete);
            }

            set
            {
                if (value > 0)
                      PercentComplete = ((double)value / Duration);
            }
        }

        #endregion


        #region Methods
        /// <summary>
        /// Gets the work.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        public long GetWork(long start, long end)
        {
            long retVal = 0;
            if(IsInRange(start, end))
            {
                Query<Interval> query = new Query<Interval>();
                AssignmentBaseFunctor<double> workFunctor = GetWorkFunctor();
                WhereInRangePredicate whereInRange = new WhereInRangePredicate(start, end);
                GroupingIntervalGenerator groupGen = new GroupingIntervalGenerator(start, end, CalendarHelper.MilisPerHour(),
                                                                                    workFunctor.CountourGenerator);
                query.Select(workFunctor).From(groupGen).Where(whereInRange.Evaluate).Execute();
                retVal = (long)workFunctor.Value;
               // XmlDocument debugDoc = ((WorkFunctor)workFunctor).DebugDocument;
                
            }

            return retVal;
        }

        /// <summary>
        /// Sets the actual work.
        /// </summary>
        /// <param name="actualWork">The actual work.</param>
        public void SetActualWork(double actualWork)
        {
            if (actualWork == 0)
            {
                PercentComplete = 0;
                return;
            }
            //find stop date by work value
            Query<Interval> query = new Query<Interval>();
            AssignmentBaseFunctor<double> workFunctor = GetWorkFunctor();
            TrueFalsePredicate<Interval> alwaysTrue = new TrueFalsePredicate<Interval>(true);
            DateAtWorkFunctor dateAtWork = new DateAtWorkFunctor(workFunctor, actualWork);
            query.Select(dateAtWork).From(workFunctor.CountourGenerator).Where(alwaysTrue.Evaluate).Execute();

            this.Stop = dateAtWork.Value;
        }

        /// <summary>
        /// Determines whether [is in range] [the specified start].
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>
        /// 	<c>true</c> if [is in range] [the specified start]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsInRange(long start, long end)
        {
		   return (end > Start && start < End);
        }
        /// <summary>
        /// Gets the work functor.
        /// </summary>
        /// <returns></returns>
        public AssignmentBaseFunctor<double> GetWorkFunctor()
        {
            ContourBucketIntervalGenerator intervalGenerator = new ContourBucketIntervalGenerator(WorkingCalendar, CurrentContour,
                                                                                                  Start, Duration);
            WorkFunctor workFunctor = new WorkFunctor(this, intervalGenerator, 0);
            return workFunctor;
        }

        private long GetTaskStart()
        {
            return _task.Start;
        }

        
        #endregion


    }
}
