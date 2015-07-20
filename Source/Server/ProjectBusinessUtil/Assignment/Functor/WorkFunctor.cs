using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Time;
using ProjectBusinessUtil.Assignment.Contour;
using System.Xml;
using ProjectBusinessUtil.Calendar;

namespace ProjectBusinessUtil.Assignment.Functor
{
    /// <summary>
    /// Represents functor calculated work from defined interval and CountourBucket.
    /// </summary>
    public class WorkFunctor : OverTimeFunctor
    {
        #region Const
        #endregion

        #region Fields
#if DEBUG
        XmlDocument _doc;
#endif
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="MyClass"/> class.
        /// </summary>
        public WorkFunctor(Assignment assignment, ContourBucketIntervalGenerator contourGenerator,
                               double overTimeUnits)
            : base(assignment, contourGenerator, overTimeUnits)
        {
        }
        #endregion

        #region Properties
        #endregion

        #region Methods

#if DEBUG
        /// <summary>
        /// Debugs the interval.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="value">The value.</param>
        private void DebugInterval(Interval interval, long duration, long value)
        {
            if (_doc == null)
            {
                _doc = new XmlDocument();
                _doc.AppendChild(_doc.CreateElement("ss"));
            }

            XmlElement element = (XmlElement)_doc.DocumentElement.AppendChild(_doc.CreateElement("WorkInterval"));

            XmlNode node = element.AppendChild(_doc.CreateElement("Start"));
            node.InnerText = new DateTime(CalendarHelper.Milis2Tick(interval.Start)).ToString("yyyy-MM-ddTHH:mm:ss");
            node = element.AppendChild(_doc.CreateElement("Finish"));
            node.InnerText = new DateTime(CalendarHelper.Milis2Tick(interval.End)).ToString("yyyy-MM-ddTHH:mm:ss");
            node = element.AppendChild(_doc.CreateElement("Value"));
            node.InnerText = String.Format("PT{0}H{1}M{2}S", value / CalendarHelper.MilisPerHour(),
                                                             (value % CalendarHelper.MilisPerHour()) / CalendarHelper.MilisPerMinute(),
                                                             (value % CalendarHelper.MilisPerMinute()) / (CalendarHelper.MilisPerMinute() / 60));
        }
#endif
        #endregion
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Executes the specified param.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <returns></returns>
        public override object Execute(Interval param)
        {
            AbstractContourBucket bucket = base.CountourGenerator.CurrentBucket;
            Interval interval = param;
            if (bucket != null)
            {
                double bucketDuration = WorkingCalendar.SubstractDates(interval.End, interval.Start, false);

                double workValue = bucket.GetEffectiveUnits(base.AssignmentUnits) * bucketDuration;
                _regularValue += workValue;
                _overTimeValue += _overTimeUnits * bucketDuration;
                this.Value = _regularValue + _overTimeValue;
#if DEBUG
                DebugInterval(interval, (long)bucketDuration, (long)workValue);
#endif
            }

            return this;
        }
    }
}
