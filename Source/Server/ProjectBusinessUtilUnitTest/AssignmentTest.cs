using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectBusinessUtil.Assignment;
using ProjectBusinessUtil.Task;
using ProjectBusinessUtil.Calendar;
using ProjectBusinessUtil.Assignment.Contour;
using ProjectBusinessUtil.Services;
using System.Xml;
using System.Text.RegularExpressions;
using System.Globalization;
using ProjectBusinessUtil.XmlSerialization;
using System.Xml.Serialization;

namespace Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class AssignmentTest
    {
        public AssignmentTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
        [TestMethod]
        public void CalendarTest()
        {
            WorkCalendarFactory calFactory = new WorkCalendarFactory();
            WorkCalendarBase calendar = calFactory.Create<DefaultCalendar>(null);
            long start = CalendarHelper.Tick2Milis(new DateTime(2008, 01, 01).Ticks);
            start += CalendarHelper.MilisPerHour();
            long duration = CalendarHelper.MilisPerHour() * 1;
            long result = calendar.AddDuration(start, duration, false);
            duration = calendar.SubstractDates(result, start, false);
        }

        [TestMethod]
        public void SetActualWorkTest()
        {
            WorkCalendarFactory calFactory = new WorkCalendarFactory();
            ContourFactory factory = new ContourFactory();
            AbstractContour contourBucket = factory.Create<StandardContour>(ContourTypes.FrontLoaded);
            WorkCalendarBase calendar = calFactory.Create<DefaultCalendar>(null);
            Task task = new Task();
            task.Start = CalendarHelper.Tick2Milis(new DateTime(2008, 01, 01).Ticks);
            Assignment assignment = new Assignment(task, calendar, contourBucket, 1.0, 0);
            long duration = CalendarHelper.MilisPerHour() * 13 + CalendarHelper.MilisPerMinute() * 20;
            assignment.Duration = duration;

            long work = assignment.GetWork(assignment.Start, assignment.End);

            assignment.SetActualWork(CalendarHelper.MilisPerMinute() * 48);
        }

        static private object GetXmlValue(XmlNode node, Type valType,
                                  bool optional, object defaultVal)
        {
            object retVal = defaultVal;

            if (node == null)
            {
                if (optional)
                    return retVal;

                throw new ArgumentException("must presents");
            }

            if (valType == typeof(TimeSpan))
            {

                Regex pattern = new Regex(@"^PT(?<H>[0-9]+)H(?<M>[0-9]+)"
                                          + @"M(?<S>(?:[0-9]*\.)?[0-9]+)S");
                Match match = pattern.Match(node.InnerText);

                if (match.Success)
                {
                    int hours = Int32.Parse(match.Groups["H"].Value);
                    int min = Int32.Parse(match.Groups["M"].Value);
                    double second = Double.Parse(match.Groups["S"].Value, CultureInfo.InvariantCulture);
                    int sec = (Int32)second;
                    int milis = (Int32)((second - sec) * 1000);
                    retVal = new TimeSpan(0, hours, min, sec, milis);
                }

            }
            else if (valType == typeof(DateTime))
            {
                retVal = XmlConvert.ToDateTime(node.InnerText,
                                               XmlDateTimeSerializationMode.Utc);
            }
            else if (valType == typeof(String))
            {
                retVal = node.InnerText;
            }
            else if (valType == typeof(Int32))
            {
                retVal = XmlConvert.ToInt32(node.InnerText);
            }
            else if (valType == typeof(Boolean))
            {
                retVal = XmlConvert.ToBoolean(node.InnerText);
            }
            else if (valType == typeof(double))
            {
                retVal = XmlConvert.ToDouble(node.InnerText);
            }
            else
            {
                throw new ArgumentException("unsupported type");
            }

            return retVal;
        }

        [TestMethod]
        public void TimePhaseTest()
        {
            string fileName = @"C:\Projects\MsProjectSynchronization\MsProject\ExceptionOverlapped.xml";
            long assignmentStart = CalendarHelper.Tick2Milis(new DateTime(2008, 8, 4, 9, 0,0).Ticks);
            long assignmentFinish = CalendarHelper.Tick2Milis(new DateTime(2008, 8, 19, 3, 0,0).Ticks); 
            int calendarUID = 3;
            int baseCalendarUID = 1;
            int assignmentID = 2;
            double units = 1.0;
            long duration = 24 *  CalendarHelper.MilisPerHour();
            double percentWorkComplete = 0.58;

            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement("ss"));

            WorkCalendarFactory calFactory = new WorkCalendarFactory();
            ContourFactory factory = new ContourFactory();
            AbstractContour contourBucket = factory.Create<StandardContour>(ContourTypes.Flat);

            WorkCalendar calendarBase =(WorkCalendar) LoadCalendar(fileName, baseCalendarUID);
            WorkCalendar calendar = (WorkCalendar)LoadCalendar(fileName, calendarUID);
            calendar.BaseCalendar = calendarBase;
            
            Task task = new Task();
            task.Start = assignmentStart;
            
            Assignment assignment = new Assignment(task, calendar, contourBucket, units, 0);
            //long duration = CalendarHelper.MilisPerHour() * 40; //+ CalendarHelper.MilisPerMinute() * 20;
            assignment.End = assignmentFinish;

            List<TimePhasedDataType> timePhasedList = LoadTimePhase(fileName, assignmentID);
            contourBucket =  factory.Create<PersonalContour>(new KeyValuePair<Assignment, TimePhasedDataType[]>(assignment, timePhasedList.ToArray()));
            XmlDocument debugdoc = DebugInterval((PersonalContour)contourBucket, assignment);

            assignment = new Assignment(task, calendar, contourBucket, units, 0);
            assignment.End = assignmentFinish;

            if (assignment.Duration != duration )
                throw new Exception("bad duration");
        
            long work = assignment.GetWork(assignment.Start, assignment.End);
            assignment.SetActualWork(work * percentWorkComplete);
            long actualWork = assignment.GetWork(assignment.Start, assignment.Stop);
          

            
            foreach(TimePhasedDataType timePhase in TimePhasedService.GetTimePhaseData(assignment, TimePhasedDataType.TimePhaseType.AssignmentRemainingWork))
            {
                XmlElement element = doc.CreateElement("TimePhaseData");
                XmlNode node = element.AppendChild(doc.CreateElement("Type"));
                node.InnerText = timePhase.Type.ToString();
                node = element.AppendChild(doc.CreateElement("UID"));
                node.InnerText = timePhase.UID.ToString();
                node = element.AppendChild(doc.CreateElement("Start"));
                node.InnerText = new DateTime(CalendarHelper.Milis2Tick(timePhase.Start)).ToString("yyyy-MM-ddTHH:mm:ss");
                node = element.AppendChild(doc.CreateElement("Finish"));
                node.InnerText = new DateTime(CalendarHelper.Milis2Tick(timePhase.Finish)).ToString("yyyy-MM-ddTHH:mm:ss");
                node = element.AppendChild(doc.CreateElement("Unit"));
                node.InnerText = timePhase.Unit.ToString();
                node = element.AppendChild(doc.CreateElement("Value"));
                node.InnerText = String.Format("PT{0}H{1}M{2}S", timePhase.Value / CalendarHelper.MilisPerHour(),
                                                                 (timePhase.Value % CalendarHelper.MilisPerHour()) / CalendarHelper.MilisPerMinute(),
                                                                 (timePhase.Value % CalendarHelper.MilisPerMinute()) / (CalendarHelper.MilisPerMinute() / 60));
                doc.DocumentElement.AppendChild(element);
            }
        }

        [TestMethod]
        public void CalendarSerializeTest()
        {
          
            string fileName = @"c:\Projects\MsProjectSynchronization\MsProject\Project1.xml";
            SerializableCalendar cal = (SerializableCalendar)LoadCalendar(fileName, 4);
          
        }


        private XmlDocument DebugInterval(PersonalContour contour, Assignment assignment)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement("ss"));
            long startDuration = 0;
            foreach (PersonalContourBucket bucket in contour.ContourBuckets)
            {
                XmlElement element = doc.CreateElement("TimePhaseData");
                XmlNode node = element.AppendChild(doc.CreateElement("Start"));
                node.InnerText = new DateTime(CalendarHelper.Milis2Tick(assignment.WorkingCalendar.AddDuration(assignment.Start, startDuration, false))).ToString("yyyy-MM-ddTHH:mm:ss");
                node = element.AppendChild(doc.CreateElement("Finish"));
                node.InnerText = new DateTime(CalendarHelper.Milis2Tick(assignment.WorkingCalendar.AddDuration(assignment.Start, bucket.ElapsedDuration, false))).ToString("yyyy-MM-ddTHH:mm:ss");
                node = element.AppendChild(doc.CreateElement("Value"));
                node.InnerText = String.Format("PT{0}H{1}M{2}S", bucket.Duration / CalendarHelper.MilisPerHour(),
                                                                 (bucket.Duration % CalendarHelper.MilisPerHour()) / CalendarHelper.MilisPerMinute(),
                                                                 (bucket.Duration % CalendarHelper.MilisPerMinute()) / (CalendarHelper.MilisPerMinute() / 60));
                doc.DocumentElement.AppendChild(element);
                startDuration = bucket.ElapsedDuration;
            }
            return doc;
        }

        private  List<TimePhasedDataType> LoadTimePhase(string file, int assignmentId)
        {
            List<TimePhasedDataType> timePhasedList = new List<TimePhasedDataType>();
            XmlDocument srcDoc = new XmlDocument();
            //srcDoc.Load(@"C:\Projects\MsProjectSynchronization\MsProject\50unitFrontLoad10.xml");
            srcDoc.Load(file);

            XmlNamespaceManager nsXmlMmngr =
                         new XmlNamespaceManager(srcDoc.NameTable);
            nsXmlMmngr.AddNamespace("ns", @"http://schemas.microsoft.com/project");
            string assignXPath = string.Format("ns:Assignments/ns:Assignment[ns:UID = '{0}']", assignmentId);
            XmlNode assignNode =
                             srcDoc.DocumentElement.SelectSingleNode(assignXPath, nsXmlMmngr);

            string timePhaseXPath = "ns:TimephasedData";
            XmlNodeList timePhaseNodes =
                            assignNode.SelectNodes(timePhaseXPath, nsXmlMmngr);

            foreach (XmlNode timePhaseNode in timePhaseNodes)
            {
                TimePhasedDataType newTimePhase = new TimePhasedDataType();
                newTimePhase.UID = (Int32)GetXmlValue(timePhaseNode["UID"], typeof(Int32),
                                            true, -1);
                newTimePhase.Unit = (TimePhasedDataType.TimePhaseUnit)GetXmlValue(timePhaseNode["Unit"], typeof(Int32),
                                            true, 2);

                newTimePhase.Type = (Int32)GetXmlValue(timePhaseNode["Type"], typeof(Int32),
                                                true, -1);
                newTimePhase.Start = CalendarHelper.Tick2Milis(((DateTime)GetXmlValue(timePhaseNode["Start"], typeof(DateTime),
                                                            true, DateTime.MinValue)).Ticks);
                newTimePhase.Finish = CalendarHelper.Tick2Milis(((DateTime)GetXmlValue(timePhaseNode["Finish"], typeof(DateTime),
                                                            true, DateTime.MinValue)).Ticks);
                newTimePhase.Value = CalendarHelper.Tick2Milis(((TimeSpan)GetXmlValue(timePhaseNode["Value"], typeof(TimeSpan),
                                                            true, TimeSpan.Zero)).Ticks);
                timePhasedList.Add(newTimePhase);
            }

            return timePhasedList;
        }

        
        private WorkCalendarBase LoadCalendar(string file, int calUID)
        {
            WorkCalendar retVal = null;
            XmlDocument srcDoc = new XmlDocument();
            srcDoc.Load(file);
            XmlNamespaceManager nsXmlMmngr =
                     new XmlNamespaceManager(srcDoc.NameTable);
            nsXmlMmngr.AddNamespace("ns", @"http://schemas.microsoft.com/project");

            string calXPath = String.Format("ns:Calendars/ns:Calendar[ns:UID = '{0}']", calUID);
            XmlNode calendarNode = srcDoc.DocumentElement.SelectSingleNode(calXPath, nsXmlMmngr);
            retVal = new SerializableCalendar(calendarNode.OuterXml);

            return retVal;

        }
    }
}
