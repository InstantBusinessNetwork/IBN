using System;
using System.Data;
using System.Collections;
using System.Xml;

using Mediachase.Ibn;
using Mediachase.IBN.Database;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for CalendarView.
	/// </summary>
	public class CalendarView
	{
		#region CALENDAR_ENTRY_TYPE
		private static ObjectTypes CALENDAR_ENTRY_TYPE
		{
			get {return ObjectTypes.CalendarEntry;}
		}
		#endregion

		#region CalendarFilter
		[Flags]
		public enum CalendarFilter
		{
			Task	= 0x01,
			ToDo	= 0x02,
			Appointment = 0x04,
			Event = 0x08,
			Meeting = 0x10,
			MileStone = 0x20
		};
		#endregion

		#region GetListPeopleForCalendar
		/// <summary>
		/// Reader returns fields:
		///  UserId, FirstName, LastName, Email, Login, Level
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListPeopleForCalendar()
		{
			return DBCommon.GetListSharingByProUser(Security.CurrentUser.UserID);
		}

		/// <summary>
		/// Reader returns fields:
		///  UserId, FirstName, LastName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListPeopleForCalendar(int ProjectId)
		{
			return DBProject.GetListTeamMemberNames(ProjectId);
		}
		#endregion

		#region GetListProjects
		/// <summary>
		/// Reader returns fields:
		///		ProjectId, Title
		/// </summary>
		public static IDataReader GetListProjects()
		{
			return DBProject.GetListProjectsSimple(Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListCalendarEntriesByUser
		/// <summary>
		///  ID, Type, Title, Description, StartDate, FinishDate, ShowStartDate, ShowFinishDate, 
		///  StateId, PriorityId, CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListCalendarEntriesByUser(
			DateTime from_date, DateTime to_date, 
			bool get_assigned, bool get_managed, bool get_created,
			int entry_type, int resource_id)
		{
			return GetListCalendarEntries(from_date, to_date, get_assigned, get_managed, get_created,
				entry_type, resource_id, 0);
		}

		public static DataTable GetListCalendarEntriesByUser(
			DateTime from_date, DateTime to_date, 
			bool get_assigned, bool get_managed, bool get_created,
			int entry_type, int resource_id, bool for_calendar)
		{
			return GetListCalendarEntries(from_date, to_date, get_assigned, get_managed, get_created,
				entry_type, resource_id, 0, for_calendar);
		}
		#endregion

		#region GetListCalendarEntriesByProject
		/// <summary>
		///  ID, Type, Title, Description, StartDate, FinishDate, ShowStartDate, ShowFinishDate, 
		///  StateId, PriorityId, CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListCalendarEntriesByProject(
			DateTime from_date, DateTime to_date, 
			bool get_assigned, bool get_managed, bool get_created,
			int entry_type, int project_id)
		{
			return GetListCalendarEntries(from_date, to_date, get_assigned, get_managed, get_created,
				entry_type, Security.CurrentUser.UserID, project_id);
		}

		public static DataTable GetListCalendarEntriesByProject(
			DateTime from_date, DateTime to_date, 
			bool get_assigned, bool get_managed, bool get_created,
			int entry_type, int project_id, bool for_calendar)
		{
			return GetListCalendarEntries(from_date, to_date, get_assigned, get_managed, get_created,
				entry_type, Security.CurrentUser.UserID, project_id, for_calendar);
		}
		#endregion

		#region GetListCalendarEntries
		/// <summary>
		///  ID, Type, Title, Description, StartDate, FinishDate, ShowStartDate, ShowFinishDate, 
		///  StateId, PriorityId, CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListCalendarEntries(
			DateTime from_date, DateTime to_date, 
			bool get_assigned, bool get_managed, bool get_created,
			int entry_type, int resource_id, int project_id)
		{
			return GetListCalendarEntries(from_date, to_date, get_assigned, get_managed, get_created, entry_type, resource_id, project_id, "", PrimaryKeyId.Empty, PrimaryKeyId.Empty);
		}

		public static DataTable GetListCalendarEntries(
			DateTime from_date, DateTime to_date, 
			bool get_assigned, bool get_managed, bool get_created, 
			int entry_type, int resource_id, int project_id, bool for_calendar)
		{
			return GetListCalendarEntries(from_date, to_date, get_assigned, get_managed, get_created, entry_type, resource_id, project_id, "", for_calendar, PrimaryKeyId.Empty, PrimaryKeyId.Empty);
		}

		public static DataTable GetListCalendarEntries(
			DateTime from_date, DateTime to_date, 
			bool get_assigned, bool get_managed, bool get_created, 
			int entry_type, int resource_id, int project_id, string Keyword,
			PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			return GetListCalendarEntries(from_date, to_date, get_assigned, get_managed, get_created, entry_type, resource_id, project_id, Keyword, false, contactUid, orgUid);
		}

		public static DataTable GetListCalendarEntries(
			DateTime from_date, DateTime to_date, 
			bool get_assigned, bool get_managed, bool get_created, 
			int entry_type, int resource_id, int project_id, string Keyword, bool for_calendar, 
			PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			if (project_id <= 0 && resource_id == 0)
				throw new AccessDeniedException();

			DateTime user_from_date = from_date;
			DateTime user_to_date = to_date;

			from_date = from_date.Date;
			to_date = to_date.Date;

			DataRow row;
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			from_date = DBCommon.GetUTCDate(TimeZoneId, from_date);	// to UTC
			to_date = DBCommon.GetUTCDate(TimeZoneId, to_date);			// to UTC

			DateTime UserDate = DBCommon.GetLocalDate(TimeZoneId, DateTime.UtcNow);		// current User's datetime
			DateTime UserStartDate = UserDate.Date;
			DateTime UserFinishDate = UserDate.Date.AddDays(1);

			DataTable table = new DataTable();
			table.Columns.Add("ID", typeof(int));
			table.Columns.Add("Type", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("Description", typeof(string));
			table.Columns.Add("StartDate", typeof(DateTime));
			table.Columns.Add("FinishDate", typeof(DateTime));
			table.Columns.Add("ShowStartDate", typeof(bool));
			table.Columns.Add("ShowFinishDate", typeof(bool));
			table.Columns.Add("StateId", typeof(int));
			table.Columns.Add("PriorityId", typeof(int));
			table.Columns.Add("CanEdit", typeof(int));	
			table.Columns.Add("CanDelete", typeof(int));	

			// Tasks
			if ((entry_type & (int)CalendarFilter.Task) == (int)CalendarFilter.Task)
			{
				using(IDataReader reader = DBTask.GetListTasksByFilter(project_id, Security.CurrentUser.UserID, resource_id, TimeZoneId, from_date, to_date, get_assigned, get_managed, get_created, Keyword))
				{
					while (reader.Read())
					{
						row = table.NewRow();
						row["ID"] = (int)reader["TaskId"];
						row["Type"] = (int)CalendarFilter.Task;
						row["Title"] = reader["Title"].ToString();
						row["Description"] = reader["Description"].ToString();
						row["StartDate"] = (DateTime)reader["StartDate"];
						row["FinishDate"] = (DateTime)reader["FinishDate"];
						row["ShowStartDate"] = true;
						row["ShowFinishDate"] = true;
						row["StateId"] = (int)reader["StateId"];
						row["PriorityId"] = (int)reader["PriorityId"];
						row["CanEdit"] = reader["CanEdit"];
						row["CanDelete"] = reader["CanDelete"];

						table.Rows.Add(row);
					}
				}
			}

			// ToDo
			if ((entry_type & (int)CalendarFilter.ToDo) == (int)CalendarFilter.ToDo)
			{
				using(IDataReader reader = DBToDo.GetListToDoByFilter(project_id, Security.CurrentUser.UserID, Security.CurrentUser.LanguageId, resource_id, TimeZoneId, from_date, to_date, get_assigned, get_managed, get_created, Keyword, null, null))
				{
					while (reader.Read())
					{
						if (for_calendar && reader["FinishDate"] == DBNull.Value && (bool)reader["IsCompleted"])
							continue;

						row = table.NewRow();

						row["ID"] = (int)reader["ToDoId"];
						row["Type"] = (int)CalendarFilter.ToDo;
						row["Title"] = reader["Title"].ToString();
						row["Description"] = reader["Description"].ToString();
						row["StateId"] = (int)reader["StateId"];
						row["PriorityId"] = (int)reader["PriorityId"];

						if (reader["StartDate"] != DBNull.Value)
						{
							row["StartDate"] = (DateTime)reader["StartDate"];
							row["ShowStartDate"] = true;
						}
						else
						{
							row["StartDate"] = (DateTime)reader["CreationDate"];
							row["ShowStartDate"] = false;
						}

						if (reader["FinishDate"] != DBNull.Value)
						{
							row["FinishDate"] = (DateTime)reader["FinishDate"];
							row["ShowFinishDate"] = true;
						}
						else
						{
							if((DateTime)row["StartDate"]>UserFinishDate)
							{
								row["FinishDate"] = ((DateTime)row["StartDate"]).AddDays(1);
								row["ShowFinishDate"] = false;
							}
							else
							{
								row["FinishDate"] = UserFinishDate;
								row["ShowFinishDate"] = false;
							}
						}

						// in Calendar for null start and finish we'll show only current day
						if (for_calendar && reader["StartDate"] == DBNull.Value && reader["FinishDate"] == DBNull.Value)
						{
							row["StartDate"] = UserStartDate;
						}

						// Fix: (2007-11-21). If ToDo has empty StartDate and not empty FinishDate and CreationDate > FinishDate 
						if (row["StartDate"] != DBNull.Value && row["FinishDate"] != DBNull.Value && (DateTime)row["StartDate"] > (DateTime)row["FinishDate"])
							row["StartDate"] = row["FinishDate"];

						row["CanEdit"] = reader["CanEdit"];
						row["CanDelete"] = reader["CanDelete"];

						table.Rows.Add(row);
					}
				}
			}

			// Event
			if ((entry_type & (int)CalendarFilter.Appointment) == (int)CalendarFilter.Appointment
				|| (entry_type & (int)CalendarFilter.Event) == (int)CalendarFilter.Event
				|| (entry_type & (int)CalendarFilter.Meeting) == (int)CalendarFilter.Meeting)
			{
				using(IDataReader reader = DBEvent.GetListEventsByFilter(project_id, Security.CurrentUser.UserID, resource_id, TimeZoneId, from_date, to_date, get_assigned, get_managed, get_created, Keyword, 
					contactUid == PrimaryKeyId.Empty ? null : (object)contactUid,
					orgUid == PrimaryKeyId.Empty ? null : (object)orgUid))
				{
					while (reader.Read())
					{
						int EventType = (int)reader["TypeId"];
						if (EventType == (int)CalendarEntry.EventType.Appointment 
							&& (entry_type & (int)CalendarFilter.Appointment) != (int)CalendarFilter.Appointment)
							continue;
						if (EventType == (int)CalendarEntry.EventType.Event 
							&& (entry_type & (int)CalendarFilter.Event) != (int)CalendarFilter.Event)
							continue;
						if (EventType == (int)CalendarEntry.EventType.Meeting 
							&& (entry_type & (int)CalendarFilter.Meeting) != (int)CalendarFilter.Meeting)
							continue;

						int EventId = (int)reader["EventId"];
						string Title = reader["Title"].ToString();
						string Description = reader["Description"].ToString();
						DateTime StartDate = (DateTime)reader["StartDate"];
						DateTime FinishDate = (DateTime)reader["FinishDate"];
						int HasRecurrence = (int)reader["HasRecurrence"];
						int EventTypeId;
						if (EventType == (int)CalendarEntry.EventType.Meeting)
							EventTypeId = (int)CalendarFilter.Meeting;
						else if (EventType == (int)CalendarEntry.EventType.Event)
							EventTypeId = (int)CalendarFilter.Event;
						else
							EventTypeId = (int)CalendarFilter.Appointment;

						if (HasRecurrence == 0)
						{
							row = table.NewRow();

							row["ID"] = EventId;
							row["Type"] = EventTypeId;
							row["Title"] = Title;
							row["Description"] = Description;
							row["StartDate"] = StartDate;
							row["FinishDate"] = FinishDate;
							row["ShowStartDate"] = true;
							row["ShowFinishDate"] = true;
							row["StateId"] = (int)reader["StateId"];
							row["PriorityId"] = (int)reader["PriorityId"];
							row["CanEdit"] = reader["CanEdit"];
							row["CanDelete"] = reader["CanDelete"];

							table.Rows.Add(row);
						}
						else	// Recurrence
						{
							int StartTime;
							int EndTime;
							CalendarEntry.Recurrence recurrence;
							using(IDataReader r_reader = DBCommon.GetRecurrence((int)ObjectTypes.CalendarEntry, EventId))
							{
								r_reader.Read();
								recurrence = new CalendarEntry.Recurrence(
									(byte)r_reader["Pattern"],
									(byte)r_reader["SubPattern"],
									(byte)r_reader["Frequency"],
									(byte)r_reader["Weekdays"],
									(byte)r_reader["DayOfMonth"],
									(byte)r_reader["WeekNumber"],
									(byte)r_reader["MonthNumber"],
									(int)r_reader["EndAfter"],
									StartDate, 
									FinishDate,
									(int)r_reader["TimeZoneId"]);
								StartTime = (int)r_reader["StartTime"];
								EndTime = (int)r_reader["EndTime"];
							}

							// Get new StartDate and FinishDate for recurrence TimeZone (not UserTimeOffset)
							DateTime eventStartDate = DateTime.UtcNow;
							using(IDataReader r_reader = DBEvent.GetEventDates(EventId, recurrence.TimeZoneId))
							{
								r_reader.Read();
								recurrence.StartDate = ((DateTime)r_reader["StartDate"]).Date;
								recurrence.FinishDate = ((DateTime)r_reader["FinishDate"]).Date;
								eventStartDate = (DateTime)r_reader["StartDate"];
							}
							eventStartDate = DBCommon.GetUTCDate(recurrence.TimeZoneId, eventStartDate);

							// from_date, to_date - in UTC
							ArrayList dates = CalendarEntry.GetRecurDates(from_date, to_date, StartTime, eventStartDate, recurrence);
							foreach (DateTime dt in dates)	// Dates in UTC (но в предположении, что событие начинается в 00:00. Поэтому надо еще добавить StartTime)
							{
								DateTime UserDt = DBCommon.GetLocalDate(TimeZoneId, dt);	// from UTC to User's time
								DateTime _StartDate = UserDt.AddMinutes(StartTime);
								DateTime _FinishDate = UserDt.AddMinutes(EndTime);

								// O.R. Если по каким-то причинам дата не попадает в нужный диапазон, пропускаем её
								if (_FinishDate < user_from_date || _StartDate > user_to_date.Date.AddDays(1))
									continue;

								row = table.NewRow();

								row["ID"] = EventId;
								row["Type"] = EventTypeId;
								row["Title"] = Title;
								row["Description"] = Description;
								row["StartDate"] = _StartDate;
								row["FinishDate"] = _FinishDate;
								row["ShowStartDate"] = true;
								row["ShowFinishDate"] = true;

								if (_StartDate > UserDate)
									row["StateId"] = 1;	// upcoming
								else if (_FinishDate < UserDate)
									row["StateId"] = 5;	// completed
								else
									row["StateId"] = 2;	// active

								row["PriorityId"] = (int)reader["PriorityId"];
								row["CanEdit"] = reader["CanEdit"];
								row["CanDelete"] = reader["CanDelete"];

								table.Rows.Add(row);
							}
						}
					}
				}
			}

			// MileStone
			if ((entry_type & (int)CalendarFilter.MileStone) == (int)CalendarFilter.MileStone)
			{
				using(IDataReader reader = DBTask.GetListMilestones(project_id, Security.CurrentUser.UserID, resource_id, TimeZoneId, from_date, to_date, Keyword))
				{
					while (reader.Read())
					{
						row = table.NewRow();
						row["ID"] = (int)reader["TaskId"];
						row["Type"] = (int)CalendarFilter.MileStone;
						row["Title"] = reader["Title"].ToString();
						row["Description"] = reader["Description"].ToString();
						row["StartDate"] = (DateTime)reader["StartDate"];
						row["FinishDate"] = (DateTime)reader["FinishDate"];
						row["ShowStartDate"] = true;
						row["ShowFinishDate"] = true;
						row["StateId"] = (int)reader["StateId"];
						row["PriorityId"] = (int)reader["PriorityId"];
						row["CanEdit"] = reader["CanEdit"];
						row["CanDelete"] = reader["CanDelete"];

						table.Rows.Add(row);
					}
				}
			}

			return table;
		}
		#endregion

		#region GetListCalendarEntriesXML
		public static void GetListCalendarEntriesXML(XmlDocument xmlDocument, DateTime dateFrom, DateTime dateTo, bool loadToDo, bool loadTask, bool loadEvent)
		{
			XmlNode taskRoot = xmlDocument.SelectSingleNode("ibnCalendar/tasks");
			XmlNode todoRoot = xmlDocument.SelectSingleNode("ibnCalendar/todos");
			XmlNode	eventRoot= xmlDocument.SelectSingleNode("ibnCalendar/events");

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			dateFrom = DBCommon.GetUTCDate(TimeZoneId, dateFrom.Date);	// to UTC
			dateTo = DBCommon.GetUTCDate(TimeZoneId, dateTo.Date);		// to UTC

			if (loadTask) 
			{
				using(IDataReader reader = DBTask.GetListTasksByFilter(0, Security.CurrentUser.UserID, 
						Security.CurrentUser.UserID, TimeZoneId, dateFrom, dateTo, true, false, false, ""))
				{
					XmlDocumentFragment fragmentTaskItem = xmlDocument.CreateDocumentFragment();
					fragmentTaskItem.InnerXml = "<task><id/><title/><description/><priority/><startDate/><dueDate/><percentComplete/></task>";

					while (reader.Read())
					{
						string Id =  reader["TaskId"].ToString();
						string Title =  reader["Title"].ToString();
						string Description = reader["Description"].ToString();
						string Priority = reader["PriorityId"].ToString();
						string StartDate = reader["StartDate"]==DBNull.Value?string.Empty:((DateTime)reader["StartDate"]).ToString("s");
						string FinishDate = reader["FinishDate"]==DBNull.Value?string.Empty:((DateTime)reader["FinishDate"]).ToString("s");
						string PercentCompleted = reader["PercentCompleted"].ToString();

						XmlNode	currentItemNode = fragmentTaskItem.SelectSingleNode("task");

						currentItemNode.SelectSingleNode("id").InnerText =Id; 
						currentItemNode.SelectSingleNode("title").InnerText =Title; 
						currentItemNode.SelectSingleNode("description").InnerText =Description; 
						currentItemNode.SelectSingleNode("priority").InnerText =Priority; 
						currentItemNode.SelectSingleNode("percentComplete").InnerText =PercentCompleted; 
						currentItemNode.SelectSingleNode("startDate").InnerText =StartDate; 
						currentItemNode.SelectSingleNode("dueDate").InnerText =FinishDate; 

						taskRoot.AppendChild(currentItemNode.CloneNode(true));
					}
				}
			}
			if (loadToDo) 
			{
				using(IDataReader reader = DBToDo.GetListToDoByFilter(0, Security.CurrentUser.UserID, Security.CurrentUser.LanguageId,
						  Security.CurrentUser.UserID, TimeZoneId, dateFrom, dateTo, true, false, false, "", null, null))
				{
					XmlDocumentFragment fragmentToDoItem = xmlDocument.CreateDocumentFragment();
					fragmentToDoItem.InnerXml = "<todo><id/><title/><description/><priority/><startDate/><dueDate/><percentComplete/></todo>";

					while (reader.Read())
					{
						if (reader["FinishDate"] == DBNull.Value && (bool)reader["IsCompleted"])
							continue;

						string Id =  reader["ToDoId"].ToString();
						string Title =  reader["Title"].ToString();
						string Description = reader["Description"].ToString();
						string Priority = reader["PriorityId"].ToString();
						string StartDate = reader["StartDate"]==DBNull.Value?string.Empty:((DateTime)reader["StartDate"]).ToString("s");
						string FinishDate = reader["FinishDate"]==DBNull.Value?string.Empty:((DateTime)reader["FinishDate"]).ToString("s");
						string PercentCompleted = reader["PercentCompleted"].ToString();

						XmlNode	currentItemNode = fragmentToDoItem.SelectSingleNode("todo");

						currentItemNode.SelectSingleNode("id").InnerText =Id; 
						currentItemNode.SelectSingleNode("title").InnerText =Title; 
						currentItemNode.SelectSingleNode("description").InnerText =Description; 
						currentItemNode.SelectSingleNode("priority").InnerText =Priority; 
						currentItemNode.SelectSingleNode("percentComplete").InnerText =PercentCompleted; 
						currentItemNode.SelectSingleNode("startDate").InnerText =StartDate; 
						currentItemNode.SelectSingleNode("dueDate").InnerText =FinishDate; 

						todoRoot.AppendChild(currentItemNode.CloneNode(true));
					}
				}
			}
			if (loadEvent) 
			{
				using(IDataReader reader = DBEvent.GetListEventsByFilter(0, Security.CurrentUser.UserID,
						  Security.CurrentUser.UserID, TimeZoneId, dateFrom, dateTo, true, false, false, "",  null, null))
				{
					while (reader.Read()) 
					{
						XmlDocumentFragment	EventItem = xmlDocument.CreateDocumentFragment();
						EventItem.InnerXml = "<event><id/><title/><description/><location/><startDate/><finishDate/><priority/><reminderInterval/><resources/></event>";

						XmlDocumentFragment	ResourceItem = xmlDocument.CreateDocumentFragment();
						ResourceItem.InnerXml = "<resource><login/><type/><name/><address/></resource>";

						int			EventId = (int)reader["EventId"];
						int			ManagerId = (int)reader["ManagerId"];
						bool		HasRecurrence = ((int)reader["HasRecurrence"] != 0);
						ArrayList	RecurDatesList =  null;
						ArrayList	addedUserId	=	new ArrayList();
						int			StartTime	=	0;
						int			EndTime		=	0;

						CalendarEntry.Recurrence	recurrence = new CalendarEntry.Recurrence(0,0,0,0,0,0,0,0,DateTime.Now,DateTime.Now,0);
						if (HasRecurrence) 
						{
							using(IDataReader r_reader = DBCommon.GetRecurrence((int)CALENDAR_ENTRY_TYPE, EventId))
							{
								r_reader.Read();
								recurrence = new CalendarEntry.Recurrence(
									(byte)r_reader["Pattern"],
									(byte)r_reader["SubPattern"],
									(byte)r_reader["Frequency"],
									(byte)r_reader["Weekdays"],
									(byte)r_reader["DayOfMonth"],
									(byte)r_reader["WeekNumber"],
									(byte)r_reader["MonthNumber"],
									(int)r_reader["EndAfter"],
									dateFrom, 
									dateTo,
									(int)r_reader["TimeZoneId"]);

								StartTime = (int)r_reader["StartTime"];
								EndTime = (int)r_reader["EndTime"];
							}
							// Get new StartDate and FinishDate for recurrence TimeZone (not UserTimeOffset)
							DateTime eventStartDate = DateTime.UtcNow;
							using(IDataReader r_reader = DBEvent.GetEventDates(EventId, recurrence.TimeZoneId))
							{
								r_reader.Read();
								recurrence.StartDate = ((DateTime)r_reader["StartDate"]).Date;
								recurrence.FinishDate = ((DateTime)r_reader["FinishDate"]).Date;
								eventStartDate = (DateTime)r_reader["StartDate"];
							}
							eventStartDate = DBCommon.GetUTCDate(recurrence.TimeZoneId, eventStartDate);

							RecurDatesList = CalendarEntry.GetRecurDates(dateFrom, dateTo, StartTime, eventStartDate, recurrence);

							if (RecurDatesList.Count == 0) continue;
						}

						EventItem.SelectSingleNode("event/id").InnerText = ((int)reader["EventId"]).ToString();
						EventItem.SelectSingleNode("event/title").InnerText = (string)reader["Title"];
						EventItem.SelectSingleNode("event/description").InnerText = (string)reader["Description"];
						EventItem.SelectSingleNode("event/location").InnerText = (string)reader["Location"];
						EventItem.SelectSingleNode("event/startDate").InnerText = ((DateTime)reader["StartDate"]).ToString("s");
						EventItem.SelectSingleNode("event/finishDate").InnerText = ((DateTime)reader["FinishDate"]).ToString("s");
						EventItem.SelectSingleNode("event/priority").InnerText = ((int)reader["PriorityId"]).ToString();
						EventItem.SelectSingleNode("event/reminderInterval").InnerText = ((int)reader["Interval"]).ToString();

						XmlNode	resourceRoot	=	EventItem.SelectSingleNode("event/resources");

						using(IDataReader r_reader = User.GetUserInfo(ManagerId))
						{
							if(r_reader.Read())
							{
								ResourceItem.SelectSingleNode("resource/login").InnerText = (string)r_reader["Login"];
								ResourceItem.SelectSingleNode("resource/type").InnerText = "manager";
								ResourceItem.SelectSingleNode("resource/name").InnerText = string.Format("{0} {1}",r_reader["FirstName"],r_reader["LastName"]);
								ResourceItem.SelectSingleNode("resource/address").InnerText = (string)r_reader["Email"];

								resourceRoot.AppendChild(ResourceItem.CloneNode(true));
								addedUserId.Add(ManagerId);
							}
						}

						using(IDataReader r_reader = CalendarEntry.GetListResources(EventId))
						{
							while(r_reader.Read())
							{
								int PrincipalId = (int)r_reader["PrincipalId"];
								bool IsGroup = (bool)r_reader["IsGroup"];
								bool MustBeConfirmed = (bool)r_reader["MustBeConfirmed"];
								bool ResponsePending = (bool)r_reader["ResponsePending"];
								bool IsConfirmed= (bool)r_reader["IsConfirmed"];

								if(IsGroup)
								{
									using(IDataReader g_reader = SecureGroup.GetGroup(PrincipalId))
									{
										if(g_reader.Read())
										{
											ResourceItem.SelectSingleNode("resource/login").InnerText = "";
											ResourceItem.SelectSingleNode("resource/type").InnerText = "group";
											ResourceItem.SelectSingleNode("resource/name").InnerText = Common.GetWebResourceString(g_reader["GroupName"].ToString());
											ResourceItem.SelectSingleNode("resource/address").InnerText = "";

											resourceRoot.AppendChild(ResourceItem.CloneNode(true));
										}
									}
								}
								else if(!MustBeConfirmed||
									MustBeConfirmed&&ResponsePending||
									MustBeConfirmed&&!ResponsePending&&IsConfirmed)
								{
									if(!addedUserId.Contains(PrincipalId))
									{
										using(IDataReader u_reader = User.GetUserInfo(PrincipalId))
										{
											if(u_reader.Read())
											{
												ResourceItem.SelectSingleNode("resource/login").InnerText = (string)u_reader["Login"];
												ResourceItem.SelectSingleNode("resource/type").InnerText = MustBeConfirmed?"optional":"required";
												ResourceItem.SelectSingleNode("resource/name").InnerText = string.Format("{0} {1}",u_reader["FirstName"],u_reader["LastName"]);
												ResourceItem.SelectSingleNode("resource/address").InnerText = (string)u_reader["Email"];

												resourceRoot.AppendChild(ResourceItem.CloneNode(true));
												addedUserId.Add(ManagerId);
											}
										}
									}
								}
							}
						}

						if (HasRecurrence)	// recurrence exists
						{
							XmlDocumentFragment	RecurrenceItem = xmlDocument.CreateDocumentFragment();
							RecurrenceItem.InnerXml = "<recurrence><recurrenceType></recurrenceType><interval/><dayOfWeek/><dayOfMonth/><monthOfYear/><instance/><startTime/><endTime/><occurrences/></recurrence>";

							// Common Information [2/9/2005]
							RecurrenceItem.SelectSingleNode("recurrence/recurrenceType").InnerText = "";
							RecurrenceItem.SelectSingleNode("recurrence/startTime").InnerText =  string.Format("{0:d2}:{1:d2}:00",StartTime/60,StartTime%60);
							RecurrenceItem.SelectSingleNode("recurrence/endTime").InnerText = string.Format("{0:d2}:{1:d2}:00",EndTime/60,EndTime%60);
							RecurrenceItem.SelectSingleNode("recurrence/occurrences").InnerText = recurrence.EndAfter.ToString();

							//								RecurrenceItem.SelectSingleNode("recurrence/interval").InnerText = rr_reader["Frequency"].ToString();
							//								RecurrenceItem.SelectSingleNode("recurrence/dayOfWeek").InnerText = rr_reader["Weekdays"].ToString();
							//								RecurrenceItem.SelectSingleNode("recurrence/dayOfMonth").InnerText = rr_reader["DayOfMonth"].ToString();
							//								RecurrenceItem.SelectSingleNode("recurrence/monthOfYear").InnerText = rr_reader["MonthNumber"].ToString();
							//								RecurrenceItem.SelectSingleNode("recurrence/instance").InnerText = rr_reader["WeekNumber"].ToString(); // ??? MonthNumber

							// 1-Daily 2-Weekly 3-Monthly 4-Yearly
							switch((int)recurrence.Pattern)
							{
								case 1:
									if(recurrence.SubPattern==CalendarEntry.RecurSubPattern.SubPattern1)
									{
										RecurrenceItem.SelectSingleNode("recurrence/recurrenceType").InnerText = "daily";
										RecurrenceItem.SelectSingleNode("recurrence/interval").InnerText = recurrence.Frequency.ToString();
										RecurrenceItem.SelectSingleNode("recurrence/dayOfWeek").InnerText = "0";
									}
									else // SubPattern 2
									{
										if(recurrence.Frequency==1)
										{
											RecurrenceItem.SelectSingleNode("recurrence/recurrenceType").InnerText = "weekly";
											RecurrenceItem.SelectSingleNode("recurrence/interval").InnerText = "1";// [2/11/2005] recurrence.Frequency.ToString();
											RecurrenceItem.SelectSingleNode("recurrence/dayOfWeek").InnerText = ((int)CalendarEntry.BitDayOfWeek.Weekdays).ToString();
										}
										else
										{
											//HasRecurrence = false;
											// Unpack Recurring calendar Item [2/11/2005]
											foreach(DateTime dt  in RecurDatesList)
											{
												DateTime UserDt = DBCommon.GetLocalDate(Security.CurrentUser.TimeZoneId, dt);	// from UTC to User's time
												DateTime _StartDate = UserDt.AddMinutes(StartTime);
												DateTime _FinishDate = UserDt.AddMinutes(EndTime);

												XmlNode xmlRecEvent = EventItem.CloneNode(true);

												xmlRecEvent.SelectSingleNode("event/startDate").InnerText = _StartDate.ToString("s");
												xmlRecEvent.SelectSingleNode("event/finishDate").InnerText = _FinishDate.ToString("s");

												eventRoot.AppendChild(xmlRecEvent);
											}

											continue;
										}
									}
									break;
								case 2:
									RecurrenceItem.SelectSingleNode("recurrence/recurrenceType").InnerText = "weekly";
									RecurrenceItem.SelectSingleNode("recurrence/interval").InnerText = recurrence.Frequency.ToString();
									RecurrenceItem.SelectSingleNode("recurrence/dayOfWeek").InnerText = recurrence.WeekDays.ToString();
									break;
								case 3:
									if(recurrence.SubPattern==CalendarEntry.RecurSubPattern.SubPattern1)
									{
										RecurrenceItem.SelectSingleNode("recurrence/recurrenceType").InnerText = "monthly";
										RecurrenceItem.SelectSingleNode("recurrence/interval").InnerText = recurrence.Frequency.ToString();
										RecurrenceItem.SelectSingleNode("recurrence/dayOfMonth").InnerText = recurrence.DayOfMonth.ToString();

									}
									else // SubPattern 2
									{
										RecurrenceItem.SelectSingleNode("recurrence/recurrenceType").InnerText = "monthNth";
										RecurrenceItem.SelectSingleNode("recurrence/interval").InnerText = recurrence.Frequency.ToString();
										RecurrenceItem.SelectSingleNode("recurrence/dayOfWeek").InnerText = recurrence.WeekDays.ToString();
										RecurrenceItem.SelectSingleNode("recurrence/instance").InnerText = recurrence.RecurWeekNumber.ToString();
									}
									break;
								case 4:
									if(recurrence.SubPattern==CalendarEntry.RecurSubPattern.SubPattern1)
									{
										RecurrenceItem.SelectSingleNode("recurrence/recurrenceType").InnerText = "yearly";
										RecurrenceItem.SelectSingleNode("recurrence/dayOfMonth").InnerText = recurrence.DayOfMonth.ToString();
										RecurrenceItem.SelectSingleNode("recurrence/monthOfYear").InnerText = recurrence.MonthNumber.ToString();
									}
									else // SubPattern 2
									{
										RecurrenceItem.SelectSingleNode("recurrence/recurrenceType").InnerText = "yearNth";
										RecurrenceItem.SelectSingleNode("recurrence/monthOfYear").InnerText =  recurrence.MonthNumber.ToString();
										RecurrenceItem.SelectSingleNode("recurrence/dayOfWeek").InnerText = recurrence.WeekDays.ToString();
										RecurrenceItem.SelectSingleNode("recurrence/instance").InnerText = recurrence.RecurWeekNumber.ToString();

									}
									break;
							}


							//
							EventItem.SelectSingleNode("event").AppendChild(RecurrenceItem);
						}
						
						eventRoot.AppendChild(EventItem);
					}
				}
			}
		}
		#endregion

		#region GetListEvents
		/// <summary>
		///  ID, Type, Title, Description, StartDate, FinishDate, ShowStartDate, ShowFinishDate, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEvents(DateTime dt, int ProjectId)
		{
			return GetListCalendarEntries(dt, dt, true, true, false,
				(int)(CalendarFilter.Event | CalendarFilter.Meeting | CalendarFilter.Appointment | CalendarFilter.MileStone), Security.CurrentUser.UserID, ProjectId);
		}
		#endregion

		#region GetListEventsByUser
		/// <summary>
		///  ID, Type, Title, Description, StartDate, FinishDate, ShowStartDate, ShowFinishDate, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEventsByUser(DateTime dt, int resource_id)
		{
			return GetListCalendarEntries(dt, dt, true, true, false,
				(int)(CalendarFilter.Event | CalendarFilter.Meeting | CalendarFilter.Appointment), resource_id, 0);
		}
		#endregion
		
		#region GetSharingLevel
		public static int GetSharingLevel(int UserId)
		{
			return DBUser.GetSharingLevel(UserId, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListScheduledUsers
		/// <summary>
		/// Reader returns fields:
		///  UserId, FirstName, LastName, Email, Login
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListScheduledUsers()
		{
			return DBUser.GetListScheduledUsers(Security.CurrentUser.UserID);
		}

		public static IDataReader GetListScheduledUsers(int EventId)
		{
			if (EventId > 0)
				return DBEvent.GetListUsersForEvent(EventId);
			else
				return GetListScheduledUsers();
		}
		#endregion

		#region GetListScheduledUsersDataTable
		/// <summary>
		/// DataTable returns fields:
		///  UserId, FirstName, LastName, Email, Login
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListScheduledUsersDataTable()
		{
			return DBUser.GetListScheduledUsersDataTable(Security.CurrentUser.UserID);
		}

		public static DataTable GetListScheduledUsersDataTable(int EventId)
		{
			if (EventId > 0)
				return DBEvent.GetListUsersForEventDataTable(EventId);
			else
				return GetListScheduledUsersDataTable();
		}
		#endregion

		#region GetListScheduling
		/// <summary>
		///  UserId, StartDate, EndDate
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListScheduling(DateTime from_date, DateTime to_date, int entry_type)
		{
			return GetListScheduling(from_date, to_date, entry_type, -1);
		}

		/// <summary>
		///  UserId, StartDate, EndDate
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListScheduling(DateTime from_date, DateTime to_date, int entry_type, int event_id)
		{
			DateTime user_from_date = from_date;
			DateTime user_to_date = to_date;

			DataRow row;

			int user_id = Security.CurrentUser.UserID;
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			from_date = DBCommon.GetUTCDate(TimeZoneId, from_date);	// to UTC
			to_date = DBCommon.GetUTCDate(TimeZoneId, to_date);			// to UTC

			DataTable table = new DataTable();
			table.Columns.Add("UserId", typeof(int));
			table.Columns.Add("StartDate", typeof(DateTime));
			table.Columns.Add("FinishDate", typeof(DateTime));

			// Tasks
			if ((entry_type & (int)CalendarFilter.Task) == (int)CalendarFilter.Task)
			{
				using(IDataReader reader = (event_id <= 0) ? DBTask.GetListTasksForScheduling(user_id, TimeZoneId, from_date, to_date) : DBTask.GetListTasksForEventScheduling(event_id, TimeZoneId, from_date, to_date))
				{
					while (reader.Read())
					{
						row = table.NewRow();

						row["UserId"] = (int)reader["UserId"];
						row["StartDate"] = (DateTime)reader["StartDate"];
						row["FinishDate"] = (DateTime)reader["FinishDate"];

						table.Rows.Add(row);
					}
				}
			}

			// ToDo
			if ((entry_type & (int)CalendarFilter.ToDo) == (int)CalendarFilter.ToDo)
			{
				using(IDataReader reader = (event_id <= 0) ? DBToDo.GetListToDoForScheduling(user_id, TimeZoneId, from_date, to_date) : DBToDo.GetListToDoForEventScheduling(event_id, TimeZoneId, from_date, to_date))
				{
					while (reader.Read())
					{
						row = table.NewRow();

						row["UserId"] = (int)reader["UserId"];
						row["StartDate"] = (DateTime)reader["StartDate"];
						row["FinishDate"] = (DateTime)reader["FinishDate"];

						table.Rows.Add(row);
					}
				}
			}

			// Event
			if ((entry_type & (int)CalendarFilter.Appointment) == (int)CalendarFilter.Appointment
				|| (entry_type & (int)CalendarFilter.Event) == (int)CalendarFilter.Event
				|| (entry_type & (int)CalendarFilter.Meeting) == (int)CalendarFilter.Meeting)
			{
				using(IDataReader reader = (event_id <= 0) ? DBEvent.GetListEventsForScheduling(user_id, TimeZoneId, from_date, to_date) : DBEvent.GetListEventsForEventScheduling(event_id, TimeZoneId, from_date, to_date))
				{
					while (reader.Read())
					{
						int EventType = (int)reader["TypeId"];
						if (EventType == (int)CalendarEntry.EventType.Appointment 
							&& (entry_type & (int)CalendarFilter.Appointment) != (int)CalendarFilter.Appointment)
							continue;
						if (EventType == (int)CalendarEntry.EventType.Event 
							&& (entry_type & (int)CalendarFilter.Event) != (int)CalendarFilter.Event)
							continue;
						if (EventType == (int)CalendarEntry.EventType.Meeting 
							&& (entry_type & (int)CalendarFilter.Meeting) != (int)CalendarFilter.Meeting)
							continue;

						int EventId = (int)reader["EventId"];
						DateTime StartDate = (DateTime)reader["StartDate"];
						DateTime FinishDate = (DateTime)reader["FinishDate"];
						int HasRecurrence = (int)reader["HasRecurrence"];

						if (HasRecurrence == 0)
						{
							row = table.NewRow();

							row["UserId"] = (int)reader["UserId"];
							row["StartDate"] = (DateTime)reader["StartDate"];
							row["FinishDate"] = (DateTime)reader["FinishDate"];

							table.Rows.Add(row);
						}
						else	// Recurrence
						{
							int StartTime;
							int EndTime;
							CalendarEntry.Recurrence recurrence;
							using(IDataReader r_reader = DBCommon.GetRecurrence((int)ObjectTypes.CalendarEntry, EventId))
							{
								r_reader.Read();
								recurrence = new CalendarEntry.Recurrence(
									(byte)r_reader["Pattern"],
									(byte)r_reader["SubPattern"],
									(byte)r_reader["Frequency"],
									(byte)r_reader["Weekdays"],
									(byte)r_reader["DayOfMonth"],
									(byte)r_reader["WeekNumber"],
									(byte)r_reader["MonthNumber"],
									(int)r_reader["EndAfter"],
									StartDate, 
									FinishDate,
									(int)r_reader["TimeZoneId"]);
								StartTime = (int)r_reader["StartTime"];
								EndTime = (int)r_reader["EndTime"];
							}

							// Get new StartDate and FinishDate for recurrence TimeZone (not UserTimeOffset)
							DateTime eventStartDate = DateTime.UtcNow;
							using(IDataReader r_reader = DBEvent.GetEventDates(EventId, recurrence.TimeZoneId))
							{
								r_reader.Read();
								recurrence.StartDate = ((DateTime)r_reader["StartDate"]).Date;
								recurrence.FinishDate = ((DateTime)r_reader["FinishDate"]).Date;
								eventStartDate = (DateTime)r_reader["StartDate"];
							}
							eventStartDate = DBCommon.GetUTCDate(recurrence.TimeZoneId, eventStartDate);

							// from_date, to_date - in UTC
							ArrayList dates = CalendarEntry.GetRecurDates(from_date, to_date, StartTime, eventStartDate, recurrence);
							foreach (DateTime dt in dates)	// Dates in UTC
							{
								DateTime UserDt = DBCommon.GetLocalDate(TimeZoneId, dt);	// from UTC to User's time

								// O.R. Если по каким-то причинам дата не попадает в нужный диапазон, пропускаем её
								if (UserDt.AddMinutes(EndTime) < user_from_date || UserDt.AddMinutes(StartTime) > user_to_date.Date.AddDays(1))
									continue;

								row = table.NewRow();

								row["UserId"] = (int)reader["UserId"];
								row["StartDate"] = UserDt.AddMinutes(StartTime);
								row["FinishDate"] = UserDt.AddMinutes(EndTime);

								table.Rows.Add(row);
							}
						}
					}
				}
			}

			DataTable ret_table = new DataTable();
			ret_table.Columns.Add("UserId", typeof(int));
			ret_table.Columns.Add("StartDate", typeof(DateTime));
			ret_table.Columns.Add("FinishDate", typeof(DateTime));

			DataView dv = table.DefaultView;
			dv.Sort = "UserId, StartDate";

			int SavedUserId = 0;
			DateTime SavedStart = DateTime.MinValue;
			DateTime SavedFinish = DateTime.MinValue;
			for(int i = 0; i < dv.Count; i++)
			{
				int UserId = (int)dv[i]["UserId"];
				DateTime Start = (DateTime)dv[i]["StartDate"];
				DateTime Finish = (DateTime)dv[i]["FinishDate"];

				if (SavedUserId != UserId)	// if user changed
				{
					if (SavedUserId != 0)	// if not first row, then user changed indeed - add info
					{	
						row = ret_table.NewRow();

						row["UserId"] = SavedUserId;
						row["StartDate"] = SavedStart;
						row["FinishDate"] = SavedFinish;

						ret_table.Rows.Add(row);
					}
					SavedUserId = UserId;
					SavedStart = Start;
					SavedFinish = Finish;
				}
				else	// the same user
				{
					if (Start <= SavedFinish)		// the same interval
					{
						SavedFinish = (Finish > SavedFinish)?Finish:SavedFinish;
					}
					else	// add the new interval
					{
						row = ret_table.NewRow();
						row["UserId"] = SavedUserId;
						row["StartDate"] = SavedStart;
						row["FinishDate"] = SavedFinish;
						ret_table.Rows.Add(row);

						SavedStart = Start;
						SavedFinish = Finish;
					}
				}
			}
			if (dv.Count > 0)		// add the last row
			{
				row = ret_table.NewRow();

				row["UserId"] = SavedUserId;
				row["StartDate"] = SavedStart;
				row["FinishDate"] = SavedFinish;

				ret_table.Rows.Add(row);
			}

			return ret_table;
		}
		#endregion

		#region AddScheduledUsers
		public static void AddScheduledUsers(DataTable dt)
		{
			int CurrentUserId = Security.CurrentUser.UserID;

			ArrayList NewUsers = new ArrayList();
			ArrayList DeletedUsers = new ArrayList();
			foreach (DataRow dr in dt.Rows)
			{
				NewUsers.Add((int)dr["UserId"]);
			}

			using(IDataReader reader = DBUser.GetListScheduledUsers(CurrentUserId))
			{
				while(reader.Read())
				{
					int UserId = (int)reader["UserId"];
					if (NewUsers.Contains(UserId))
						NewUsers.Remove(UserId);
					else
						DeletedUsers.Add(UserId);
				}
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				// Remove Users
				foreach(int UserId in DeletedUsers)
					DBUser.DeleteScheduledUser(CurrentUserId, UserId);

				// Add new Users
				foreach (int UserId in NewUsers)
					DBUser.AddScheduledUser(CurrentUserId, UserId);

				tran.Commit();
			}
		}
		#endregion
	}
}
