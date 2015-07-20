using System;
using System.Data;
using System.Collections;
using Mediachase.IBN.Database;
using System.Xml;

using Mediachase.Ibn;


namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for IBNPrj2XML.
	/// </summary>
	public class IBNPrj2XML
	{
		[ThreadStatic]
		private static int GlobalUID;

		[ThreadStatic]
		private static Hashtable htTasks,htResources;

		private const string DateTimeFormatString = "yyyy-MM-ddTHH:mm:ss";

		private static int GetNewUID()
		{
			return ++GlobalUID;
		}

		static IBNPrj2XML()
		{
			GlobalUID=0;
			htResources = new Hashtable();
			htTasks = new Hashtable();
		}

	#region IBN2XML
		public static XmlDocument IBN2XML(int project_id)
		{
			if(!License.MsProjectIntegration)
				throw new FeatureNotAvailable();

			if ( htResources != null )
				htResources.Clear();
			else
				htResources = new Hashtable();
			if ( htTasks != null )
				htTasks.Clear();
			else
				htTasks = new Hashtable();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlProjectTemplate);

			int _id;
			int CalendarUID = GetNewUID();
			string CalendarName = "";
			int iCalendarId = 0;
			using(IDataReader reader = Project.GetProject(project_id))
			{
				if(reader.Read())
				{
					iCalendarId = (int)reader["CalendarId"];
					doc.SelectSingleNode("Project/UID").InnerText = GetNewUID().ToString();
					doc.SelectSingleNode("Project/Name").InnerText = reader["Title"].ToString() + ".xml";
					doc.SelectSingleNode("Project/Title").InnerText = reader["Title"].ToString();
					if(reader["Description"]!=System.DBNull.Value)
						doc.SelectSingleNode("Project/Subject").InnerText = reader["Description"].ToString();
					doc.SelectSingleNode("Project/Category").InnerText = reader["TypeName"].ToString();

					_id = (int)reader["ManagerId"];
					using(IDataReader reader_User = User.GetUserInfo(_id))
					{
						if(reader_User.Read())
							doc.SelectSingleNode("Project/Manager").InnerText = reader_User["FirstName"].ToString()+" "+reader_User["LastName"].ToString();
					}

					_id = (int)reader["CreatorId"];
					using(IDataReader reader_User = User.GetUserInfo(_id))
					{
						if(reader_User.Read())
							doc.SelectSingleNode("Project/Author").InnerText = reader_User["FirstName"].ToString()+" "+reader_User["LastName"].ToString();
					}

					doc.SelectSingleNode("Project/CreationDate").InnerText = XmlConvert.ToString((DateTime)reader["CreationDate"], DateTimeFormatString);
//					doc.SelectSingleNode("Project/LastSaved").InnerText = XmlConvert.ToString((DateTime)reader["LastSavedDate"], DateTimeFormatString);
					doc.SelectSingleNode("Project/StartDate").InnerText = XmlConvert.ToString((DateTime)reader["TargetStartDate"], DateTimeFormatString);
					if (reader["TargetFinishDate"]!=DBNull.Value)
					{
						doc.SelectSingleNode("Project/FinishDate").InnerText = XmlConvert.ToString((DateTime)reader["TargetFinishDate"], DateTimeFormatString);
					}				
					doc.SelectSingleNode("Project/CalendarUID").InnerText = CalendarUID.ToString();
					doc.SelectSingleNode("Project/WeekStartDay").InnerText = 1.ToString();
					//
					doc.SelectSingleNode("Project/DefaultTaskType").InnerText = 1.ToString();
					doc.SelectSingleNode("Project/ProjectExternallyEdited").InnerText = 0.ToString();
					//doc.SelectSingleNode("Project/ScheduleFromStart").InnerText = 1.ToString();
					//doc.SelectSingleNode("Project/FYStartDate").InnerText = 1.ToString();
					//doc.SelectSingleNode("Project/DefaultFixedCostAccrual").InnerText = 1.ToString();
					//doc.SelectSingleNode("Project/SpreadPercentComplete").InnerText = 1.ToString();
					//
					CalendarName = reader["CalendarName"].ToString();
				}
			}
			//convertCalendars(doc, project_id, CalendarUID, CalendarName);
			Cal2NewZoneXML(doc, CalendarUID, iCalendarId, Security.CurrentUser.TimeZoneId);
			convertTeam(doc,project_id, CalendarUID);
			convertTasks(doc,project_id,CalendarUID);
			convertPredecessors(doc,project_id);
			XmlAttribute xmlAtr = doc.CreateAttribute("xmlns");
			xmlAtr.Value = @"http://schemas.microsoft.com/project";
			doc.SelectSingleNode("Project").Attributes.Append(xmlAtr);
			return doc;
		}
	#endregion

	#region Convert

		#region Calendars
		private static void convertCalendars(XmlDocument xmlPrjDoc, int project_id, int CalendarUID, string CalendarName)
		{
			XmlDocumentFragment xmlCalendar = xmlPrjDoc.CreateDocumentFragment();
			xmlCalendar.InnerXml = xmlMsCalendarTemplate;

			xmlCalendar.SelectSingleNode("Calendar/UID").InnerText = CalendarUID.ToString();
			xmlCalendar.SelectSingleNode("Calendar/Name").InnerText = CalendarName;
			xmlCalendar.SelectSingleNode("Calendar/IsBaseCalendar").InnerText = 1.ToString();
			xmlCalendar.SelectSingleNode("Calendar/BaseCalendarUID").InnerText = (-1).ToString();
			
			XmlNode calendarNode = xmlCalendar.SelectSingleNode("Calendar");
			XmlNode newCalenderNode = calendarNode.CloneNode(true);
			XmlNode calendarsNode = xmlPrjDoc.SelectSingleNode("Project/Calendars");
			calendarsNode.AppendChild(newCalenderNode);

			XmlElement elem = xmlPrjDoc.CreateElement("WeekDays");
			string s="";
			for(int i=1;i<=7;i++)
			{
				if(i<6) 
					s+= "<WeekDay>" +
						"<DayType>"+i.ToString()+"</DayType>" +
						"<DayWorking>1</DayWorking>" +
						"<WorkingTimes>"+
						"<WorkingTime>"+
						"<FromTime>08:00:00</FromTime>"+
						"<ToTime>12:00:00</ToTime>"+
						"</WorkingTime>"+
						"<WorkingTime>"+
						"<FromTime>13:00:00</FromTime>"+
						"<ToTime>17:00:00</ToTime>"+
						"</WorkingTime>"+
						"</WorkingTimes>"+
						"</WeekDay>";
				else
					s+= "<WeekDay>" +
						"<DayType>"+i.ToString()+"</DayType>" +
						"<DayWorking>0</DayWorking>" +
						"</WeekDay>";
			}
			elem.InnerXml=s;
			xmlPrjDoc.SelectSingleNode(String.Format("Project/Calendars/Calendar[{0}]", 1)).AppendChild(elem);
		}
		#endregion

		#region Cal2NewZoneXML
		public static void Cal2NewZoneXML(XmlDocument xmlPrjDoc, int CalendarUID, int calendar_id, int newZoneId)
		{
			XmlDocumentFragment xmlCalendar = xmlPrjDoc.CreateDocumentFragment();
			xmlCalendar.InnerXml = xmlCalendarTemplate;
			
			int curOffset=0;
			int newOffset=User.GetTimeZoneBias(newZoneId);

			using(IDataReader reader = Calendar.GetCalendar(calendar_id))
			{
				if(reader.Read())
				{
					int curZoneId = (int)reader["TimeZoneId"];
					//curOffset = (int)reader["TimeOffset"];
					curOffset=User.GetTimeZoneBias(curZoneId);
					xmlCalendar.SelectSingleNode("Calendar/UID").InnerText = CalendarUID.ToString();
					if(reader["CalendarName"]!=DBNull.Value)
						xmlCalendar.SelectSingleNode("Calendar/Name").InnerText = reader["CalendarName"].ToString();
					xmlCalendar.SelectSingleNode("Calendar/IsBaseCalendar").InnerText = 1.ToString();
					xmlCalendar.SelectSingleNode("Calendar/BaseCalendarUID").InnerText = (-1).ToString();
				}
			}

			#region Weekdays
			for(byte i=1;i<=7;i++)
			{
				using(IDataReader reader = DBCalendar.GetListWeekdayHours(calendar_id, i))
				{
					while(reader.Read())
					{
						int fromTime = (int)reader["FromTime"]+curOffset-newOffset;
						int toTime = (int)reader["ToTime"];
						if(toTime==0)
							toTime = 1440;
						toTime += curOffset-newOffset;
					
						if(fromTime<0 && toTime<=0)	//полностью во вчерашний день
						{
							WorkWeekDay(xmlPrjDoc,xmlCalendar, DiffDay(i), fromTime+1440, toTime+1440);
						}
						if(fromTime<0 && toTime>0)	//частично во вчерашний день, частично в сегодняшний
						{
							WorkWeekDay(xmlPrjDoc,xmlCalendar, DiffDay(i), fromTime+1440, 1440);
					
							WorkWeekDay(xmlPrjDoc,xmlCalendar, i, 0, toTime);
						}
						if(fromTime>=0 && fromTime<1440 && toTime>0 && toTime<=1440)	//полностью в сегодняшний
						{
							WorkWeekDay(xmlPrjDoc,xmlCalendar, i, fromTime, toTime);
						}
						if(fromTime>0 && fromTime<1440 && toTime>1440)	//частично в сегодняшнем дне, частично в завтрашнем
						{
							WorkWeekDay(xmlPrjDoc,xmlCalendar, i, fromTime, 1440);

							WorkWeekDay(xmlPrjDoc,xmlCalendar, AddDay(i), 0, toTime-1440);
						}
						if(fromTime>=1440 && toTime>1440)	//полностью в завтрашнем дне
						{
							WorkWeekDay(xmlPrjDoc,xmlCalendar, AddDay(i), fromTime-1440, toTime-1440);
						}
					}
				}
			}
			#endregion

			#region Exceptions
			using(IDataReader reader = DBCalendar.GetListExceptions(calendar_id))
			{
				while(reader.Read())
				{
					int iExceptionId = (int)reader["ExceptionId"];
					DateTime fromDate = (DateTime)reader["FromDate"];
					DateTime toDate = (DateTime)reader["ToDate"];
					using(IDataReader _hoursExcept = DBCalendar.GetListExceptionHours(iExceptionId))
					{
						int fromTime=0;
						int toTime=0;
						if (_hoursExcept.Read())
						{
							fromTime = (int)_hoursExcept["FromTime"] + curOffset - newOffset;
							toTime = (int)_hoursExcept["ToTime"] + curOffset - newOffset;	
							WorkingException(xmlPrjDoc,xmlCalendar, fromDate, toDate, fromTime, toTime);
							if (_hoursExcept.Read())
							{
								fromTime = (int)_hoursExcept["FromTime"] + curOffset - newOffset;
								toTime = (int)_hoursExcept["ToTime"] + curOffset - newOffset;	
								WorkingException(xmlPrjDoc,xmlCalendar, fromDate, toDate, fromTime, toTime);
								if (_hoursExcept.Read())
								{
									fromTime = (int)_hoursExcept["FromTime"] + curOffset - newOffset;
									toTime = (int)_hoursExcept["ToTime"] + curOffset - newOffset;
									WorkingException(xmlPrjDoc,xmlCalendar, fromDate, toDate, fromTime, toTime);
									if (_hoursExcept.Read())
									{
										fromTime = (int)_hoursExcept["FromTime"] + curOffset - newOffset;
										toTime = (int)_hoursExcept["ToTime"] + curOffset - newOffset;	
										WorkingException(xmlPrjDoc,xmlCalendar, fromDate, toDate, fromTime, toTime);
										if (_hoursExcept.Read())
										{
											fromTime = (int)_hoursExcept["FromTime"] + curOffset - newOffset;
											toTime = (int)_hoursExcept["ToTime"] + curOffset - newOffset;
											WorkingException(xmlPrjDoc,xmlCalendar, fromDate, toDate, fromTime, toTime);
										}
									}
								}
							}
						}
						else
						{
							XmlElement elem = xmlPrjDoc.CreateElement("WeekDay");
							string s= "<DayType>0</DayType>"+
								"<DayWorking>0</DayWorking>"+
								"<TimePeriod>"+
								"<FromDate>"+XmlConvert.ToString(fromDate.AddHours(curOffset-newOffset), DateTimeFormatString)+"</FromDate>"+
								"<ToDate>"+XmlConvert.ToString(toDate.AddSeconds((curOffset-newOffset+24)*3600-1), DateTimeFormatString)+"</ToDate>"+
								"</TimePeriod>";
							elem.InnerXml=s;
							xmlCalendar.SelectSingleNode("Calendar/WeekDays").AppendChild(elem);	

					#region Comments
							/*fromTime = curOffset*60 - newOffset*60;
							toTime = 1440 + curOffset*60 - newOffset*60;
							if(fromTime<0)
							{
								string s= "<DayType>0</DayType>"+
									"<DayWorking>"+0.ToString()+"</DayWorking>"+
									"<TimePeriod>"+
									"<FromDate>"+XmlConvert.ToString(fromDate.AddDays(-1), DateTimeFormatString)+"</FromDate>"+
									"<ToDate>"+XmlConvert.ToString(toDate.AddDays(-1), DateTimeFormatString)+"</ToDate>"+
									"</TimePeriod>"+
									"<WorkingTimes>"+
									"<WorkingTime>"+
									"<FromTime>"+GetTimeString(fromTime+1440)+"</FromTime>"+
									"<ToTime>"+GetTimeString(1440)+"</ToTime>"+
									"</WorkingTime>"+
									"</WorkingTimes>";
								elem.InnerXml=s;
								xmlCalendar.SelectSingleNode("Calendar/WeekDays").AppendChild(elem);	

								XmlElement elem2 = xmlCalendar.CreateElement("WeekDay");
								string s2= "<DayType>0</DayType>"+
									"<DayWorking>"+0.ToString()+"</DayWorking>"+
									"<TimePeriod>"+
									"<FromDate>"+XmlConvert.ToString(fromDate, DateTimeFormatString)+"</FromDate>"+
									"<ToDate>"+XmlConvert.ToString(toDate, DateTimeFormatString)+"</ToDate>"+
									"</TimePeriod>"+
									"<WorkingTimes>"+
									"<WorkingTime>"+
									"<FromTime>"+GetTimeString(0)+"</FromTime>"+
									"<ToTime>"+GetTimeString(toTime)+"</ToTime>"+
									"</WorkingTime>"+
									"</WorkingTimes>";
								elem2.InnerXml=s2;
								xmlCalendar.SelectSingleNode("Calendar/WeekDays").AppendChild(elem2);
							}
							if(toTime>1440)
							{
								XmlElement elem = xmlCalendar.CreateElement("WeekDay");
								string s= "<DayType>0</DayType>"+
									"<DayWorking>"+0.ToString()+"</DayWorking>"+
									"<TimePeriod>"+
									"<FromDate>"+XmlConvert.ToString(fromDate, DateTimeFormatString)+"</FromDate>"+
									"<ToDate>"+XmlConvert.ToString(toDate, DateTimeFormatString)+"</ToDate>"+
									"</TimePeriod>"+
									"<WorkingTimes>"+
									"<WorkingTime>"+
									"<FromTime>"+GetTimeString(fromTime)+"</FromTime>"+
									"<ToTime>"+GetTimeString(1440)+"</ToTime>"+
									"</WorkingTime>"+
									"</WorkingTimes>";
								elem.InnerXml=s;
								xmlCalendar.SelectSingleNode("Calendar/WeekDays").AppendChild(elem);
						
								XmlElement elem2 = xmlCalendar.CreateElement("WeekDay");
								string s2= "<DayType>0</DayType>"+
									"<DayWorking>"+0.ToString()+"</DayWorking>"+
									"<TimePeriod>"+
									"<FromDate>"+XmlConvert.ToString(fromDate.AddDays(1), DateTimeFormatString)+"</FromDate>"+
									"<ToDate>"+XmlConvert.ToString(toDate.AddDays(1), DateTimeFormatString)+"</ToDate>"+
									"</TimePeriod>"+
									"<WorkingTimes>"+
									"<WorkingTime>"+
									"<FromTime>"+GetTimeString(0)+"</FromTime>"+
									"<ToTime>"+GetTimeString(toTime-1440)+"</ToTime>"+
									"</WorkingTime>"+
									"</WorkingTimes>";
								elem2.InnerXml=s2;
								xmlCalendar.SelectSingleNode("Calendar/WeekDays").AppendChild(elem2);
							}*/
					#endregion

						}
					}
				}
			}
			#endregion

			XmlNode calendarNode = xmlCalendar.SelectSingleNode("Calendar");
			XmlNode newCalendarNode = calendarNode.CloneNode(true);
			XmlNode allcalendarNode = xmlPrjDoc.SelectSingleNode("Project/Calendars");
			allcalendarNode.AppendChild(newCalendarNode);
		}
		#endregion

		#region Tasks
		private static void convertTasks(XmlDocument xmlPrjDoc, int project_id, int CalendarUID)
		{
			using(IDataReader TaskList = Task.GetListTasksByProject(project_id))
			{
				while(TaskList.Read())
				{
					XmlDocumentFragment xmlTask = xmlPrjDoc.CreateDocumentFragment();
					xmlTask.InnerXml = xmlMsTaskTemplate;
					int iTaskId = (int)TaskList["TaskId"];
					int newuid = GetNewUID();
					String TaskDuration="";
					DateTime dtS=DateTime.Now;
					DateTime dtF=DateTime.Now;
					bool HasAssign = false;
					using(IDataReader reader = Task.GetListResources(iTaskId))
					{
						if(reader.Read())
						{
							HasAssign = true;
						}
					}

					bool fl_Summary = false;
					int iComplType = 0;
					int iPerc=0;
					int duration = 0;
					
					using(IDataReader reader = Task.GetTask(iTaskId))
					{
						if(reader.Read())
						{
							iComplType = (int)reader["CompletionTypeId"];
							xmlTask.SelectSingleNode("Task/UID").InnerText = newuid.ToString();
							xmlTask.SelectSingleNode("Task/ID").InnerText = ((int)reader["TaskNum"]).ToString();
							xmlTask.SelectSingleNode("Task/Name").InnerText = reader["Title"].ToString();
							xmlTask.SelectSingleNode("Task/Type").InnerText = 1.ToString();
							xmlTask.SelectSingleNode("Task/IsNull").InnerText = 0.ToString();
							xmlTask.SelectSingleNode("Task/CreateDate").InnerText = XmlConvert.ToString((DateTime)reader["CreationDate"], DateTimeFormatString);
							xmlTask.SelectSingleNode("Task/WBS").InnerText = reader["OutlineNumber"].ToString();
							xmlTask.SelectSingleNode("Task/WBSLevel").InnerText = ((int)reader["OutlineLevel"]).ToString();
							xmlTask.SelectSingleNode("Task/OutlineNumber").InnerText = reader["OutlineNumber"].ToString();
							xmlTask.SelectSingleNode("Task/OutlineLevel").InnerText = ((int)reader["OutlineLevel"]).ToString();
							xmlTask.SelectSingleNode("Task/Priority").InnerText = ((int)reader["PriorityId"]).ToString();
							dtS = (DateTime)reader["StartDate"];
							dtF = (DateTime)reader["FinishDate"];
							xmlTask.SelectSingleNode("Task/Start").InnerText = XmlConvert.ToString(dtS, DateTimeFormatString);
							xmlTask.SelectSingleNode("Task/Finish").InnerText = XmlConvert.ToString(dtF, DateTimeFormatString);
							int IsMilestone = 0;
							if((bool)reader["IsMilestone"]) IsMilestone = 1;
							int IsSummary = 0;
							if((bool)reader["IsSummary"]) IsSummary = 1;
							if(IsSummary==1 || IsMilestone==1) fl_Summary = true;
							xmlTask.SelectSingleNode("Task/Milestone").InnerText = IsMilestone.ToString();
							xmlTask.SelectSingleNode("Task/Summary").InnerText = IsSummary.ToString();
							xmlTask.SelectSingleNode("Task/ActualStart").InnerText = XmlConvert.ToString(dtS, DateTimeFormatString);
							if(reader["ActualFinishDate"]!=System.DBNull.Value)
								xmlTask.SelectSingleNode("Task/ActualFinish").InnerText = XmlConvert.ToString((DateTime)reader["ActualFinishDate"], DateTimeFormatString);
							else
							{
								// Oleg Zhuk: System.Xml.Schema.XmlSchemaException: The 'http://schemas.microsoft.com/project:ActualFinish' element has an invalid value according to its data type. An error occurred at file:///C:/Documents and Settings/bug/My Documents/IBN Received Files/проект+71.xml(1, 2700).
								xmlTask.SelectSingleNode("Task").RemoveChild(xmlTask.SelectSingleNode("Task/ActualFinish"));
								// End Oleg Zhuk [7/30/2004]
							}

							xmlTask.SelectSingleNode("Task/FixedCostAccrual").InnerText = 3.ToString();
							xmlTask.SelectSingleNode("Task/ConstraintType").InnerText = 2.ToString();//((int)reader["ConstraintTypeId"]).ToString();
							xmlTask.SelectSingleNode("Task/CalendarUID").InnerText = CalendarUID.ToString();
							if(reader["ConstraintDate"]!=System.DBNull.Value)
								xmlTask.SelectSingleNode("Task/ConstraintDate").InnerText = XmlConvert.ToString((DateTime)reader["ConstraintDate"], DateTimeFormatString);
							if(reader["Description"]!=System.DBNull.Value)
								xmlTask.SelectSingleNode("Task/Notes").InnerText = reader["Description"].ToString();
							xmlTask.SelectSingleNode("Task/IgnoreResourceCalendar").InnerText = 1.ToString();
							xmlTask.SelectSingleNode("Task/EffortDriven").InnerText = 0.ToString();
							duration = (int)reader["Duration"];
							iPerc = (int)reader["PercentCompleted"];
							xmlTask.SelectSingleNode("Task/PercentComplete").InnerText = iPerc.ToString();
							if(!HasAssign)
							{
								TaskDuration = string.Format("PT{0}H{1}M{2}S", duration/60,duration-60*(duration/60),0);					
								xmlTask.SelectSingleNode("Task/Duration").InnerText = TaskDuration;
											
								int actduration = duration*iPerc/100;
								String ActTaskDuration = string.Format("PT{0}H{1}M{2}S", actduration/60,actduration-60*(actduration/60),0);
								xmlTask.SelectSingleNode("Task/ActualDuration").InnerText =ActTaskDuration;
								int remDuration = duration - actduration;
								String RemTaskDuration = string.Format("PT{0}H{1}M{2}S", remDuration/60,remDuration-60*(remDuration/60),0);
								xmlTask.SelectSingleNode("Task/RemainingDuration").InnerText =RemTaskDuration;
							}
							else
							{
								// Oleg Zhuk: System.Xml.Schema.XmlSchemaException: The 'http://schemas.microsoft.com/project:Duration' element has an invalid value according to its data type. An error occurred at file:///C:/Documents and Settings/bug/My Documents/IBN Received Files/проект+71.xml(1, 2505). [7/30/2004]
								xmlTask.SelectSingleNode("Task").RemoveChild(xmlTask.SelectSingleNode("Task/Duration"));
								// Oleg Zhuk: System.Xml.Schema.XmlSchemaException: The 'http://schemas.microsoft.com/project:ActualDuration' element has an invalid value according to its data type. An error occurred at file:///C:/Documents and Settings/bug/My Documents/IBN Received Files/проект+71.xml(1, 2700).
								xmlTask.SelectSingleNode("Task").RemoveChild(xmlTask.SelectSingleNode("Task/ActualDuration"));
								// Oleg Zhuk: System.Xml.Schema.XmlSchemaException: The 'http://schemas.microsoft.com/project:RemainingDuration' element has an invalid value according to its data type. An error occurred at file:///C:/Documents and Settings/bug/My Documents/IBN Received Files/проект+71.xml(1, 2700). [7/30/2004]
								xmlTask.SelectSingleNode("Task").RemoveChild(xmlTask.SelectSingleNode("Task/RemainingDuration"));
								// End Oleg Zhuk [7/30/2004]
							}
						}
					}
					if(!fl_Summary)
					{
						using(IDataReader reader = Task.GetListResources(iTaskId))
						{
							while (reader.Read())
							{
								int iUserId = (int)reader["UserId"];
								int rPerc = (int)reader["PercentCompleted"];
								if(iComplType==2) rPerc=iPerc;
								ConvertAssignments(xmlPrjDoc, project_id, iUserId, newuid, dtS, dtF, duration, rPerc);
							}
						}
					}				
					XmlNode taskNode = xmlTask.SelectSingleNode("Task");
					XmlNode newTaskNode = taskNode.CloneNode(true);
					XmlNode tasksNode = xmlPrjDoc.SelectSingleNode("Project/Tasks");
					tasksNode.AppendChild(newTaskNode);
					htTasks.Add(iTaskId,newuid);
				}
			}
		}
		#endregion

		#region Team
		private static void convertTeam(XmlDocument xmlPrjDoc, int project_id, int CalendarUID)
		{
			using(IDataReader Team = Project.GetListTeamMembers(project_id))
			{
				int id = 1;
				while(Team.Read())
				{
					XmlDocumentFragment xmlTeam = xmlPrjDoc.CreateDocumentFragment();
					xmlTeam.InnerXml = xmlMsResourceTemplate;
					int newuid = GetNewUID();
					xmlTeam.SelectSingleNode("Resource/UID").InnerText = newuid.ToString();
					xmlTeam.SelectSingleNode("Resource/ID").InnerText = (id++).ToString();	
					int iUserId=(int)Team["UserId"];
					using(IDataReader reader = User.GetUserInfo(iUserId))
					{
						if(reader.Read())
						{
							xmlTeam.SelectSingleNode("Resource/Name").InnerText = reader["LastName"].ToString()+" "+reader["FirstName"].ToString();
							xmlTeam.SelectSingleNode("Resource/EmailAddress").InnerText = reader["Email"].ToString();
						}
					}
					xmlTeam.SelectSingleNode("Resource/Type").InnerText = 1.ToString();
					xmlTeam.SelectSingleNode("Resource/IsNull").InnerText = 0.ToString();
					if(Team["Code"]!=System.DBNull.Value)
						xmlTeam.SelectSingleNode("Resource/Code").InnerText = Team["Code"].ToString();
					if(Team["Rate"]!=System.DBNull.Value)
						xmlTeam.SelectSingleNode("Resource/StandardRate").InnerText = ((decimal)Team["Rate"]).ToString();
					xmlTeam.SelectSingleNode("Resource/CalendarUID").InnerText = CalendarUID.ToString();
					XmlNode teamNode = xmlTeam.SelectSingleNode("Resource");
					XmlNode newTeamNode = teamNode.CloneNode(true);
					XmlNode allteamNode = xmlPrjDoc.SelectSingleNode("Project/Resources");
					allteamNode.AppendChild(newTeamNode);
					htResources.Add(iUserId,newuid);
				}
			}
		}
		#endregion

		#region Assignments
		private static void ConvertAssignments(XmlDocument xmlPrjDoc, int project_id, int user_id, int taskuid, DateTime StartDate, DateTime FinishDate, int TaskDuration, int rPercCompl)
		{	
		
			XmlDocumentFragment xmlAssignment = xmlPrjDoc.CreateDocumentFragment();
			xmlAssignment.InnerXml = xmlMsAssignmentTemplate;

			try
			{
				xmlAssignment.SelectSingleNode("Assignment/ResourceUID").InnerText = htResources[user_id].ToString();
			}
			catch
			{
				return;
			}
			xmlAssignment.SelectSingleNode("Assignment/UID").InnerText = GetNewUID().ToString();
			xmlAssignment.SelectSingleNode("Assignment/TaskUID").InnerText = taskuid.ToString();	
			
			xmlAssignment.SelectSingleNode("Assignment/ActualStart").InnerText = XmlConvert.ToString(StartDate, DateTimeFormatString);
			xmlAssignment.SelectSingleNode("Assignment/PercentWorkComplete").InnerText = rPercCompl.ToString();
			String sTaskDuration = string.Format("PT{0}H{1}M{2}S", TaskDuration/60,TaskDuration-60*(TaskDuration/60),0);	
			xmlAssignment.SelectSingleNode("Assignment/Work").InnerText = sTaskDuration;
			xmlAssignment.SelectSingleNode("Assignment/RegularWork").InnerText = sTaskDuration;
			//xmlAssignment.SelectSingleNode("Assignment/ActualFinish").InnerText = XmlConvert.ToString(FinishDate, DateTimeFormatString);
			int actduration = TaskDuration*rPercCompl/100;
			String ActTaskDuration = string.Format("PT{0}H{1}M{2}S", actduration/60,actduration-60*(actduration/60),0);
			xmlAssignment.SelectSingleNode("Assignment/ActualWork").InnerText = ActTaskDuration;				
			int remDuration = TaskDuration-actduration;
			String RemTaskDuration = string.Format("PT{0}H{1}M{2}S", remDuration/60,remDuration-60*(remDuration/60),0);
			xmlAssignment.SelectSingleNode("Assignment/RemainingWork").InnerText = RemTaskDuration;
						
			XmlNode assignmentNode = xmlAssignment.SelectSingleNode("Assignment");
			XmlNode newAssignmentNode = assignmentNode.CloneNode(true);
			XmlNode assignmentsNode = xmlPrjDoc.SelectSingleNode("Project/Assignments");		
			assignmentsNode.AppendChild(newAssignmentNode);
		}
		#endregion

		#region Predesessors
		private static void convertPredecessors(XmlDocument xmlPrjDoc, int project_id)
		{			
			int taskcount = 0;
			using(IDataReader PTasks = Task.GetListTasksByProject(project_id))
			{
				while(PTasks.Read())
				{
					taskcount++;
					int iTaskId = (int)PTasks["TaskId"];
					using(IDataReader Preds = Task.GetListPredecessorsDetails(iTaskId))
					{
						while(Preds.Read())
						{
							int iPredId = (int)Preds["TaskId"];
							int lag = (int)Preds["Lag"]*10;
							//String TaskLag = string.Format("PT{0}H{1}M{2}S", lag/60,lag-60*(lag/60),0);					
							XmlElement elem = xmlPrjDoc.CreateElement("PredecessorLink");							
							elem.InnerXml = "<PredecessorUID>" + htTasks[iPredId].ToString() + "</PredecessorUID>" +
								"<Type>1</Type>" +
								//"<CrossProject>0</CrossProject>" +
								//"<CrossProjectName />" +
								"<LinkLag>"+lag.ToString()+"</LinkLag>" +
								"<LagFormat>7</LagFormat>";
							xmlPrjDoc.SelectSingleNode(String.Format("Project/Tasks/Task[{0}]", taskcount)).AppendChild(elem);
						}
					}
				}
			}
		}
		#endregion

	#endregion

	#region Work

		private static void WorkingException(XmlDocument xmlPrjDoc, XmlDocumentFragment doc, DateTime fromDate, DateTime toDate, int fromTime, int toTime)
		{
			if(fromTime<0 && toTime<=0)	//полностью во вчерашний день
			{
				WorkExceptionDay(xmlPrjDoc,doc, fromDate.AddDays(-1), toDate.AddDays(-1), fromTime+1440, toTime+1440);
			}
			if(fromTime<0 && toTime>0)	//частично во вчерашний день, частично в сегодняшний
			{
				WorkExceptionDay(xmlPrjDoc,doc, fromDate.AddDays(-1),toDate.AddDays(-1), fromTime+1440, 1440);

				WorkExceptionDay(xmlPrjDoc,doc, fromDate, toDate, 0, toTime);
			}
			if(fromTime>=0 && fromTime<1440 && toTime>0 && toTime<=1440)	//полностью в сегодняшний
			{
				WorkExceptionDay(xmlPrjDoc,doc, fromDate, toDate, fromTime, toTime);
			}
			if(fromTime>0 && fromTime<1440 && toTime>1440)	//частично в сегодняшнем дне, частично в завтрашнем
			{
				WorkExceptionDay(xmlPrjDoc,doc, fromDate, toDate, fromTime, 1440);
				
				WorkExceptionDay(xmlPrjDoc,doc, fromDate.AddDays(1), toDate.AddDays(1), 0, toTime-1440);
			}
			if(fromTime>=1440 && toTime>1440)	//полностью в завтрашнем дне
			{
				WorkExceptionDay(xmlPrjDoc,doc, fromDate.AddDays(1), toDate.AddDays(1), fromTime-1440, toTime-1440);
			}
		}

		private static void WorkWeekDay(XmlDocument xmlPrjDoc, XmlDocumentFragment doc, int i, int fromTime, int toTime)
		{
			XmlElement elem = xmlPrjDoc.CreateElement("WorkingTime");
			CheckWorkTimes(xmlPrjDoc,doc, AddDay((byte)i));
			string s= "<FromTime>"+GetTimeString(fromTime)+"</FromTime>"+
				"<ToTime>"+GetTimeString(toTime)+"</ToTime>";
			elem.InnerXml=s;
			doc.SelectSingleNode(String.Format("Calendar/WeekDays/WeekDay[{0}]/WorkingTimes", AddDay((byte)i))).AppendChild(elem);
			doc.SelectSingleNode(String.Format("Calendar/WeekDays/WeekDay[{0}]/DayWorking", AddDay((byte)i))).InnerText=1.ToString();
		}

		private static void WorkExceptionDay(XmlDocument xmlPrjDoc, XmlDocumentFragment doc, DateTime fromDate, DateTime toDate, int fromTime, int toTime)
		{
			XmlElement elem = xmlPrjDoc.CreateElement("WeekDay");
			string s= "<DayType>0</DayType>"+
				"<DayWorking>1</DayWorking>"+
				"<TimePeriod>"+
				"<FromDate>"+XmlConvert.ToString(fromDate, DateTimeFormatString)+"</FromDate>"+
				"<ToDate>"+XmlConvert.ToString(toDate.AddSeconds(23*3600+59*60+59), DateTimeFormatString)+"</ToDate>"+
				"</TimePeriod>"+
				"<WorkingTimes>"+
				"<WorkingTime>"+
				"<FromTime>"+GetTimeString(fromTime)+"</FromTime>"+
				"<ToTime>"+GetTimeString(toTime)+"</ToTime>"+
				"</WorkingTime>"+
				"</WorkingTimes>";
			elem.InnerXml=s;
			doc.SelectSingleNode("Calendar/WeekDays").AppendChild(elem);	
		}

		private static byte DiffDay(byte i)
		{
			if(i==1)
				return 7;
			else 
				return (byte)(i-1);
		}

		private static byte AddDay(byte i)
		{
			if(i==7)
				return 1;
			else
				return (byte)(i+1);
		}

		private static string GetTimeString(int TimeMin)
		{
			int _h=TimeMin/60;
			int _m=TimeMin - _h*60;
			//If end time is 0, it is treated as midnight the next day
			//13.11.2008 et: lost 1 minute fix
			if (TimeMin == 1440)
			{
				return "00:00:00";
			}
			//if (TimeMin==1440)
			//    return "23:59:59";
			string s="";
			if(_h<10 && _m<10)
				s="0"+_h+":0"+_m+":00";
			if(_h<10 && _m>=10)
				s="0"+_h+":"+_m+":00";
			if(_h>=10 && _m<10)
				s=_h+":0"+_m+":00";
			if(_h>=10 && _m>=10)
				s=_h+":"+_m+":00";
			return s;
		}

		private static void CheckWorkTimes(XmlDocument xmlPrjDoc, XmlDocumentFragment doc, int i)
		{
			if(doc.SelectSingleNode(String.Format("Calendar/WeekDays/WeekDay[{0}]/WorkingTimes", i))==null)
			{
				XmlElement WTNode = xmlPrjDoc.CreateElement("WorkingTimes");
				doc.SelectSingleNode(String.Format("Calendar/WeekDays/WeekDay[{0}]", i)).AppendChild(WTNode);
			}
		}

	#endregion

	#region Templates

		#region xmlProjectTemplate
		private const string xmlProjectTemplate = 
			"<?xml version=\"1.0\" encoding=\"utf-8\" ?>"+
			"<Project>" +
			"<UID/>" +
			"<Name />" +		
			"<Title />" +	
			"<Subject />" +	
			"<Category />" +
			//			"<Company />" +
			"<Manager />" +
			"<Author />" +
			"<CreationDate />" +
			//			"<Revision />" +
			"<LastSaved />" +
			"<ScheduleFromStart />" +
			"<StartDate />" +
			"<FinishDate />" +

			// Oleg Zhuk: Comment line fix XmlSchemaException [7/30/2004]
			//"<FYStartDate />" +
			// End Oleg Zhuk Comment [7/30/2004]

			//			"<CriticalSlackLimit />" +
			//			"<CurrencyDigits />" +
			//			"<CurrencySymbol />" +
			//			"<CurrencySymbolPosition />" +
			"<CalendarUID />" +
			//			"<DefaultStartTime />" +
			//			"<DefaultFinishTime />" +
			//			"<MinutesPerDay />" +
			//			"<MinutesPerWeek />" +
			//			"<DaysPerMonth />" +
			"<DefaultTaskType />" +
			//			"<DefaultFixedCostAccrual />" +
			//			"<DefaultStandardRate />" +
			//			"<DefaultOvertimeRate />" +
			//			"<DurationFormat />" +
			//			"<WorkFormat />" +
			//			"<EditableActualCosts />" +
			//			"<HonorConstraints />" +
			//			"<EarnedValueMethod />" +
			//			"<InsertedProjectsLikeSummary />" +
			//			"<MultipleCriticalPaths />" +
			//			"<NewTasksEffortDriven />" +
			//			"<NewTasksEstimated />" +
			//			"<SplitsInProgressTasks />" +
			//			"<SpreadActualCost />" +
			//			"<SpreadPercentComplete />" +
			//			"<TaskUpdatesResource />" +
			//			"<FiscalYearStart />" +
			"<WeekStartDay />" +
			//			"<MoveCompletedEndsBack />" +
			//			"<MoveRemainingStartsBack />" +
			//			"<MoveRemainingStartsForward />" +
			//			"<MoveCompletedEndsForward />" +
			//			"<BaselineForEarnedValue />" +
			//			"<AutoAddNewResourcesAndTasks />" +
			//			"<StatusDate />" +
			//			"<CurrentDate />" +
			//			"<MicrosoftProjectServerURL />" +
			//			"<Autolink />" +
			//			"<NewTaskStartDate />" +
			//			"<DefaultTaskEVMethod />" +
			"<ProjectExternallyEdited />" +
			//			"<OutlineCodes></OutlineCodes>" +
			//			"<WBSMasks></WBSMasks>" +
			//			"<ExtendedAttributes></ExtendedAttributes>" +
			"<Calendars></Calendars>" +
			"<Tasks></Tasks>" +
			"<Resources></Resources>" +
			"<Assignments></Assignments>" +
			"</Project>";
		#endregion

		#region xmlMsCalendarTemplate
		private const string xmlMsCalendarTemplate=
			"<Calendar>"+
			"<UID />"+
			"<Name />"+
			"<IsBaseCalendar />"+
			"<BaseCalendarUID />"+
			//			"<WeekDays>"+
			//				"<WeekDay>"+
			//					"<DayType />"+
			//					"<DayWorking />"+
			//					"<TimePeriod>"+
			//						"<FromDate />"+
			//						"<ToDate />"+
			//					"</TimePeriod>"+
			//					"<WorkingTimes>"+
			//						"<WorkingTime>"+
			//							"<FromTime />"+
			//							"<ToTime />"+
			//						"</WorkingTime>"+
			//					"</WorkingTimes>"+
			//				"</Weekday>"+
			//			"</Weekdays>"+
			"</Calendar>";
		#endregion

		#region xmlMsTaskTemplate
		private const string xmlMsTaskTemplate = 
			"<Task>" +
			"<UID />" +
			"<ID />" +
			"<Name />" +
			"<Type />" +
			"<IsNull />" +
			"<CreateDate />" +
			//			"<Contact />" +
			"<WBS />" +
			"<WBSLevel />" +
			"<OutlineNumber />" +
			"<OutlineLevel />" +
			"<Priority />" +
			"<Start />" +
			"<Finish />" +
			"<Duration />" +
			//			"<DurationFormat />" +
			//			"<Work />" +
			//			"<Stop />" +
			//			"<Resume />" +
			//			"<ResumeValid />" +
			"<EffortDriven />" +
			//			"<Recurring />" +
			//			"<OverAllocated />" +
			//			"<Estimated />" +
			"<Milestone />" +
			"<Summary />" + 
			//			"<Critical />" +
			//			"<IsSubproject />" +
			//			"<IsSubprojectReadOnly />" +
			//			"<SubprojectName />" +
			//			"<ExternalTask />" +
			//			"<ExternalTaskProject />" +
			//			"<EarlyStart />" +
			//			"<EarlyFinish />" +
			//			"<LateStart />" +
			//			"<LateFinish />" +
			//			"<StartVariance />" +
			//			"<FinishVariance />" +
			//			"<WorkVariance />" +
			//			"<FreeSlack />" +
			//			"<TotalSlack />" +
			//			"<FixedCost />" +
			"<FixedCostAccrual />" +
			"<PercentComplete />" +
			//			"<PercentWorkComplete />" +
			//			"<Cost />" +
			//			"<OvertimeCost />" +
			//			"<OvertimeWork />" +
			"<ActualStart />" +
			"<ActualFinish />" +
			"<ActualDuration />" +
			//			"<ActualCost />" +
			//			"<ActualOvertimeCost />" +
			//			"<ActualWork />" +
			//			"<ActualOvertimeWork />" +
			//			"<RegularWork />" +
			"<RemainingDuration />" +
			//			"<RemainingCost />" +
			//			"<RemainingWork />" +
			//			"<RemainingOvertimeCost />" +
			//			"<RemainingOvertimeWork />" +
			//			"<ACWP />" + 
			//			"<CV />" +
			"<ConstraintType />" +
			"<CalendarUID />" +
			"<ConstraintDate />" +
			//			"<Deadline />" +
			//			"<LevelAssignments />" +
			//			"<LevelingCanSplit />" +
			//			"<LevelingDelay />" +
			//			"<PreLeveledStart />" +
			//			"<PreLeveledFinish />" +
			//			"<Hyperlink />" +
			//			"<HyperlinkAddress />" +
			//			"<HyperlinkSubAddress />" +
			"<IgnoreResourceCalendar />" +
			"<Notes />" +
			//			"<HideBar />" +
			//			"<Rollup />" +
			//			"<BCWS />" +
			//			"<BCWP />" +
			//			"<PhysicalPercentComplete />" +
			//			"<EarnedValueMethod />" +
			//"<PredecessorLink>" +
			//"</PredecessorLink>" +
			//			"<ExtendedAttribute>" +
			/*"<UID />" +
										"<FieldID />" +
										"<Value />" +
										"<ValueID />" +
										"<DurationFormat />" +*/
			//			"</ExtendedAttribute>" + 
			//			"<Baseline>" +
			/*"<TimephasedData />" +
										"<Number />" +
										"<Interim />" +
										"<Start />" +
										"<Finish />" +
										"<Duration />" +
										"<DurationFormat />" +
										"<EstimatedDuration />" +
										"<Work />" +
										"<Cost />" +
										"<BCWS />" +
										"<BCWP />" +*/
			//			"</Baseline>" +
			//			"<OutlineCode>" +
			/*"<UID />" +
										"<FieldID />" +
										"<ValueID />" +*/
			//			"</OutlineCode>" +
			//			"<TimephasedData />" +
			"</Task>";
		#endregion

		#region xmlMsResourceTemplate
		private const string xmlMsResourceTemplate = 
			"<Resource>" +
			"<UID />" +
			"<ID />" +
			"<Name />" +
			"<Type />"+
			"<IsNull />"+
			//			"<Initials />"+
			//			"<Phonetics />"+
			//			"<NTAccount />"+
			//			"<MaterialLabel />"+
			"<Code />"+
			//			"<Group />"+
			//			"<WorkGroup />"+
			"<EmailAddress />"+
			//			"<Hyperlink />"+
			//			"<HyperlinkAddress />"+
			//			"<HyperlinkSubAddress />"+
			//			"<MaxUnits />"+
			//			"<PeakUnits />"+
			//			"<OverAllocated />"+
			//			"<AvailableFrom />"+
			//			"<AvailableTo />"+
			//			"<Start />"+
			//			"<Finish />"+
			//			"<CanLevel />"+
			//			"<AccrueAt />"+
			//			"<Work />"+
			//			"<RegularWork />"+
			//			"<OvertimeWork />"+
			//			"<ActualWork />"+
			//			"<RemainingWork />"+
			//			"<ActualOvertimeWork />"+
			//			"<RemainingOvertimeWork />"+
			//			"<PercentWorkComplete />"+
			"<StandardRate />"+
			//			"<StandardRateFormat />"+
			//			"<Cost />"+
			//			"<OvertimeRate />"+
			//			"<OvertimeRateFormat />"+
			//			"<OvertimeCost />"+
			//			"<CostPerUse />"+
			//			"<ActualCost />"+
			//			"<ActualOvertimeCost />"+
			//			"<RemainingCost />"+
			//			"<RemainingOvertimeCost />"+
			//			"<WorkVariance />"+
			//			"<CostVariance />"+
			//			"<SV />"+
			//			"<CV />"+
			//			"<ACWP />"+
			"<CalendarUID />"+
			//			"<Notes />"+
			//			"<BCWS />"+
			//			"<BCWP />"+
			//			"<IsGeneric />"+
			//			"<IsInactive />"+
			//			"<ExtendedAttribute>"+
			/*"<UID />"+
										"<FieldID />"+
										"<Value />"+
										"<ValueID />"+
										"<DurationFormat />"+*/ 
			//			"</ExtendedAttribute>"+
			//			"<Baseline>"+
			/*"<Number />"+
										"<Work />"+
										"<Cost />"+
										"<BCWS />"+
										"<BCWP />"+ */
			//			"</Baseline>"+
			//			"<OutlineCode>"+
			/*"<UID />"+
										"<FieldID />"+
										"<ValueID />"+ */
			//			"</OutlineCode>" +
			//			"<AvailabilityPeriods>" +
			/*"<AvailabilityPeriod>"+
											"<AvailableFrom />"+
											"<AvailableTo />"+
											"<AvailableUnits />"+
										"</AvailabilityPeriod>"+*/
			//			"</AvailabilityPeriods>"+
			//			"<Rates>"+
			//			"<Rate>"+
			/*"<RatesFrom />"+
											"<RatesTo />"+
											"<RateTable />"+
											"<StandardRate />"+
											"<StandardRateFormat />"+
											"<OvertimeRate />"+
											"<OvertimeRateFormat />"+
											"<CostPerUse />"+ */
			//			"</Rate>"+
			//			"</Rates>"+
			//			"<TimephasedData />"+
			"</Resource>";
		#endregion

		#region xmlMsAssignmentTemplate
		private const string xmlMsAssignmentTemplate = 
			"<Assignment>" + 
			"<UID />" + 
			"<TaskUID />" + 
			"<ResourceUID />" + 
			"<PercentWorkComplete />" + 
			//			"<ActualCost />" + 
			//			"<ActualFinish />" + 
			//			"<ActualOvertimeCost />" + 
			//			"<ActualOvertimeWork />" + 
			"<ActualStart />" + 
			"<ActualWork />" + 
			//			"<ACWP />" + 
			//			"<Confirmed />" + 
			//			"<Cost />" + 
			//			"<CostRateTable />" + 
			//			"<CostVariance />" + 
			//			"<CV />" + 
			//			"<Delay />" + 
			//			"<Finish />" + 
			//			"<FinishVariance />" + 
			//			"<Hyperlink />" + 
			//			"<HyperlinkAddress />" + 
			//			"<HyperlinkSubAddress />" + 
			//			"<WorkVariance />" + 
			//			"<HasFixedRateUnits />" + 
			//			"<FixedMaterial />" + 
			//			"<LevelingDelay />" + 
			//			"<LinkedFields />" + 
			//			"<Milestone />" + 
			//			"<Notes />" + 
			//			"<Overallocated />" + 
			//			"<OvertimeCost />" + 
			//			"<OvertimeWork />" + 
			"<RegularWork />" + 
			//			"<RemainingCost />" + 
			//			"<RemainingOvertimeCost />" + 
			//			"<RemainingOvertimeWork />" + 
			"<RemainingWork />" + 
			//			"<ResponsePending />" + 
			//			"<Start />" + 
			//			"<Stop />" + 
			//			"<Resume />" + 
			//			"<StartVariance />" + 
			//			"<Units />" + 
			//			"<UpdateNeeded />" + 
			//			"<VAC />" + 
			"<Work />" + 
			//			"<WorkContour />" + 
			//			"<BCWS />" + 
			//			"<BCWP />" + 
			//			"<ExtendedAttribute>" + 
			/*"<UID />" + 
										"<FieldID />" + 
										"<Value />" + 
										"<ValueID />" + 
										"<DurationFormat />" + */
			//			"</ExtendedAttribute>" +
			//			"<Baseline>" + 
			/*"<TimephasedData />" + 
										"<Number />" + 
										"<Start />" + 
										"<Finish />" + 
										"<Work />" + 
										"<Cost />" + 
										"<BCWS />" + 
										"<BCWP />" + */
			//			"</Baseline>" + 
			//			"<TimephasedDataType />" + 
			"</Assignment>" ;
		#endregion

		#region xmlCalendarTemplate
		private const string xmlCalendarTemplate=
			"<Calendar>"+
			"<UID />"+
			"<Name />"+
			"<IsBaseCalendar />"+
			"<BaseCalendarUID />"+
			"<WeekDays>"+
			"<WeekDay>"+
			"<DayType>1</DayType>"+
			"<DayWorking>0</DayWorking>"+
		//	"<WorkingTimes>"+
		//	"</WorkingTimes>"+
			"</WeekDay>"+
			"<WeekDay>"+
			"<DayType>2</DayType>"+
			"<DayWorking>0</DayWorking>"+
		//	"<WorkingTimes>"+
		//	"</WorkingTimes>"+
			"</WeekDay>"+
			"<WeekDay>"+
			"<DayType>3</DayType>"+
			"<DayWorking>0</DayWorking>"+
		//	"<WorkingTimes>"+
		//	"</WorkingTimes>"+
			"</WeekDay>"+
			"<WeekDay>"+
			"<DayType>4</DayType>"+
			"<DayWorking>0</DayWorking>"+
		//	"<WorkingTimes>"+
		//	"</WorkingTimes>"+
			"</WeekDay>"+
			"<WeekDay>"+
			"<DayType>5</DayType>"+
			"<DayWorking>0</DayWorking>"+
		//	"<WorkingTimes>"+
		//	"</WorkingTimes>"+
			"</WeekDay>"+
			"<WeekDay>"+
			"<DayType>6</DayType>"+
			"<DayWorking>0</DayWorking>"+
		//	"<WorkingTimes>"+
		//	"</WorkingTimes>"+
			"</WeekDay>"+
			"<WeekDay>"+
			"<DayType>7</DayType>"+
			"<DayWorking>0</DayWorking>"+
		//	"<WorkingTimes>"+
		//	"</WorkingTimes>"+
			"</WeekDay>"+
			"</WeekDays>"+
			"</Calendar>";
			#endregion


	#endregion

	}
}
