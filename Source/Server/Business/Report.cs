using System;
using System.Data;
using System.Collections;
using Mediachase.IBN.Database;
using System.Xml;
using Mediachase.IBN.Business.Reports;
using Mediachase.SQLQueryCreator;

namespace Mediachase.IBN.Business
{

	#region Class ActivityInfo
	public class ActivityInfo : IComparable
	{
		public string DisplayName;
		public int Count;

		public ActivityInfo() { }
		public ActivityInfo(string displayName)
		{
			DisplayName = displayName;
		}

		public void AddInfo(ActivityInfo info)
		{
			if (info != null)
			{
				Count += info.Count;
			}
		}

		#region Implementation of IComparable
		public int CompareTo(object obj)
		{
			if (obj == null)
				return 1;
			else
			{
				int n = -1 * this.Count.CompareTo(((ActivityInfo)obj).Count);
				if (n == 0)
					n = this.DisplayName.CompareTo(((ActivityInfo)obj).DisplayName);
				return n;
			}
		}
		#endregion
	}
	#endregion

	#region Class UserActivityInfo
	public class UserActivityInfo : ActivityInfo
	{
		public int IMGroupId;

		public UserActivityInfo() { }
		public UserActivityInfo(string displayName, int iIMGroupid)
			: base(displayName)
		{
			IMGroupId = iIMGroupid;
		}
	}
	#endregion

	#region Enum ActivityReportType
	public enum ActivityReportType
	{
		Messages,
		FilesExchanged,
		IMLogins,
		PortalLogins,
		CalendarEntries,
		ActiveProjects,
		//		FilesPublished,
		//		FileVersionsPublished,
		NewIssues,
		NewTasks,
		NewToDos
	}
	#endregion

	#region Class HourAndCount
	public class HourAndCount
	{
		public int Hour;
		public int Count;

		public HourAndCount() { }
		public HourAndCount(int hour)
		{
			Hour = hour;
			Count = 0;
		}
	}
	#endregion

	#region Class DayOfWeekAndCount
	public class DayOfWeekAndCount
	{
		public String Day;
		public int Count;

		public DayOfWeekAndCount() { }
		public DayOfWeekAndCount(String day)
		{
			string _s = day.Substring(0, 1);
			Day = _s.ToUpper();
			_s = day.Substring(1);
			Day += _s;
			Count = 0;
		}
	}
	#endregion

	public class Report
	{
		private const int TimeInterval = 25;

		#region ObjectType
		[Flags]
		public enum ObjectType
		{
			Task = 0x01,
			ToDo = 0x02,
			Appointment = 0x04,
			Event = 0x08,
			Meeting = 0x10,
			IncidentToDo = 0x20,
			DocumentToDo = 0x40,
			TaskToDo = 0x80
		};
		#endregion

		#region GetSecureGroupsActivityTable
		/// <summary>
		/// Returns fields: DisplayName, Count.
		/// </summary>
		public static DataTable GetSecureGroupsActivityTable(DateTime fromDate, DateTime toDate, ActivityReportType type, int topCount)
		{
			return BuildActivityTable(GetSecureGroupsActivity(fromDate, toDate, type, topCount));
		}
		#endregion

		#region GetGroupsActivityTable
		/// <summary>
		/// Returns fields: DisplayName, Count.
		/// </summary>
		public static DataTable GetGroupsActivityTable(DateTime fromDate, DateTime toDate, ActivityReportType type, int topCount)
		{
			return BuildActivityTable(GetGroupsActivity(fromDate, toDate, type, topCount));
		}
		#endregion

		#region GetUsersActivityTable
		/// <summary>
		/// Returns fields: DisplayName, Count.
		/// </summary>
		public static DataTable GetUsersActivityTable(DateTime fromDate, DateTime toDate, ActivityReportType type, int topCount)
		{
			return BuildActivityTable(GetUsersActivity(fromDate, toDate, type, topCount));
		}

		#endregion

		#region BuildActivityTable
		/// <summary>
		/// Returns fields: DisplayName, Count.
		/// </summary>
		public static DataTable BuildActivityTable(ArrayList list)
		{
			DataTable table;
			DataRow row;

			table = new DataTable();
			table.Columns.Add("DisplayName", typeof(string));
			table.Columns.Add("Count", typeof(int));

			foreach (ActivityInfo entry in list)
			{
				row = table.NewRow();

				row["DisplayName"] = entry.DisplayName;
				row["Count"] = entry.Count;

				table.Rows.Add(row);
			}
			return table;
		}
		#endregion

		#region GetGroupsActivity
		/// <summary>
		/// Returns ArrayList of ActivityInfo objects.
		/// </summary>
		public static ArrayList GetGroupsActivity(DateTime fromDate, DateTime toDate, ActivityReportType type, int topCount)
		{
			ArrayList list = new ArrayList();
			Hashtable hash = new Hashtable();
			ActivityInfo group;

			foreach (UserActivityInfo user in GetUsersActivity(fromDate, toDate, type, 0))
			{
				group = (ActivityInfo)hash[user.IMGroupId];
				if (group == null)
				{
					string sDisplayName = IMGroup.GetIMGroupName(user.IMGroupId, string.Empty);
					group = new ActivityInfo(sDisplayName);
					hash[user.IMGroupId] = group;
					list.Add(group);
				}
				group.AddInfo(user);
			}

			// Leave only topCount items
			if (topCount > 0)
			{
				list.Sort();
				if (list.Count > topCount)
					list.RemoveRange(topCount, list.Count - topCount);
			}

			// Remove items with zero counter
			for (int i = 0; i < list.Count; )
			{
				if (((ActivityInfo)list[i]).Count == 0)
					list.RemoveAt(i);
				else
					i++;
			}

			return list;
		}
		#endregion

		#region GetUsersActivity
		/// <summary>
		/// Returns ArrayList of UserActivityInfo objects.
		/// </summary>
		public static ArrayList GetUsersActivity(DateTime fromDate, DateTime toDate, ActivityReportType type, int topCount)
		{
			ArrayList list = new ArrayList();
			UserActivityInfo user;

			using (IDataReader reader = GetUsersByCompany())
			{
				while (reader.Read())
				{
					string sDisplayName = reader["DisplayName"].ToString();
					int iIMGroupId = (int)reader["IMGroupId"];
					int user_id = (int)reader["UserId"];
					user = new UserActivityInfo(sDisplayName, iIMGroupId);
					list.Add(user);
					using (IDataReader _obj = User.GetUserInfoByOriginalId(user_id))
					{
						if (_obj.Read())
							user_id = (int)_obj["UserId"];
						else
							continue;
					}

					int DefaultBias = User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
					switch (type)
					{
						case ActivityReportType.Messages:
							using (IDataReader _obj = DBReport.GetUserStats(user_id, fromDate, toDate, DefaultBias))
							{
								if (_obj.Read())
									user.Count = (int)_obj["MsgSent"];
							}
							break;
						case ActivityReportType.FilesExchanged:
							using (IDataReader _obj = DBReport.GetUserStats(user_id, fromDate, toDate, DefaultBias))
							{
								if (_obj.Read())
									user.Count = (int)_obj["FilesSent"];
							}
							break;
						case ActivityReportType.IMLogins:
							using (IDataReader _obj = DBReport.GetUserStats(user_id, fromDate, toDate, DefaultBias))
							{
								if (_obj.Read())
									user.Count = (int)_obj["LoginsClient"];
							}
							break;
						case ActivityReportType.PortalLogins:
							using (IDataReader _obj = GetQuickSnapshotReport(fromDate, toDate, user_id))
							{
								if (_obj.Read())
									user.Count = (int)_obj["PortalLogins"];
							}
							break;
						case ActivityReportType.CalendarEntries:
							using (IDataReader _obj = GetQuickSnapshotReport(fromDate, toDate, user_id))
							{
								if (_obj.Read())
									user.Count = (int)_obj["NewEventsCount"];
							}
							break;
						case ActivityReportType.ActiveProjects:
							using (IDataReader _obj = GetQuickSnapshotReport(fromDate, toDate, user_id))
							{
								if (_obj.Read())
									user.Count = (int)_obj["NewProjectsCount"];
							}
							break;
						case ActivityReportType.NewIssues:
							using (IDataReader _obj = GetQuickSnapshotReport(fromDate, toDate, user_id))
							{
								if (_obj.Read())
									user.Count = (int)_obj["NewIncidentsCount"];
							}
							break;
						case ActivityReportType.NewTasks:
							using (IDataReader _obj = GetQuickSnapshotReport(fromDate, toDate, user_id))
							{
								if (_obj.Read())
									user.Count = (int)_obj["NewTasksCount"];
							}
							break;
						case ActivityReportType.NewToDos:
							using (IDataReader _obj = GetQuickSnapshotReport(fromDate, toDate, user_id))
							{
								if (_obj.Read())
									user.Count = (int)_obj["NewToDosCount"];
							}
							break;
						default:
							break;
					}
				}
			}

			// Leave only topCount items
			if (topCount > 0)
			{
				list.Sort();
				if (list.Count > topCount)
					list.RemoveRange(topCount, list.Count - topCount);
			}

			// Remove items with zero counter
			for (int i = 0; i < list.Count; )
			{
				if (((ActivityInfo)list[i]).Count == 0)
					list.RemoveAt(i);
				else
					i++;
			}

			return list;
		}
		#endregion

		#region GetSecureGroupsActivity
		/// <summary>
		/// Returns ArrayList of ActivityInfo objects.
		/// </summary>
		public static ArrayList GetSecureGroupsActivity(DateTime fromDate, DateTime toDate, ActivityReportType type, int topCount)
		{
			ArrayList list = new ArrayList();
			ActivityInfo group;

			using (IDataReader reader = SecureGroup.GetListGroups())
			{
				while (reader.Read())
				{
					string sDisplayName = Common.GetWebResourceString(reader["GroupName"].ToString());
					int iGroupId = (int)reader["GroupId"];
					group = new ActivityInfo(sDisplayName);
					using (IDataReader _obj = GetSecGroupStats(iGroupId, fromDate, toDate, false))
					{
						if (_obj.Read())
						{
							switch (type)
							{
								case ActivityReportType.PortalLogins:
									group.Count = (int)_obj["PortalLogins"];
									break;
								case ActivityReportType.CalendarEntries:
									group.Count = (int)_obj["NewEventsCount"];
									break;
								case ActivityReportType.ActiveProjects:
									group.Count = (int)_obj["NewProjectsCount"];
									break;
								/*								case ActivityReportType.FilesPublished:
																	group.Count=(int)_obj["NewAssetsCount"];
																	break;
																case ActivityReportType.FileVersionsPublished:
																	group.Count=(int)_obj["NewAssetVersionsCount"];
																	break;
								*/
								case ActivityReportType.NewIssues:
									group.Count = (int)_obj["NewIncidentsCount"];
									break;
								case ActivityReportType.NewTasks:
									group.Count = (int)_obj["NewTasksCount"];
									break;
								case ActivityReportType.NewToDos:
									group.Count = (int)_obj["NewToDosCount"];
									break;
								default:
									break;
							}
						}
					}
					list.Add(group);
				}
			}

			// Leave only topCount items
			if (topCount > 0)
			{
				list.Sort();
				if (list.Count > topCount)
					list.RemoveRange(topCount, list.Count - topCount);
			}

			// Remove items with zero counter
			for (int i = 0; i < list.Count; )
			{
				if (((ActivityInfo)list[i]).Count == 0)
					list.RemoveAt(i);
				else
					i++;
			}

			return list;
		}
		#endregion

		#region GetDayOfWeekReportTable
		/// <summary>
		/// Returns fields: Title, Count.
		/// </summary>
		public static DataTable GetDayOfWeekReportTable(DateTime fromDate, DateTime toDate, DateTime StartWeekDay)
		{
			DataTable table = new DataTable();
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("Count", typeof(int));

			DataRow row;
			foreach (DayOfWeekAndCount entry in GetDayOfWeekReport(fromDate, toDate, StartWeekDay))
			{
				row = table.NewRow();

				row["Title"] = entry.Day.ToString();
				row["Count"] = entry.Count;

				table.Rows.Add(row);
			}
			return table;
		}
		#endregion

		#region GetHourReportTable
		/// <summary>
		/// Returns fields: Title, Count.
		/// </summary>
		public static DataTable GetHourReportTable(DateTime fromDate, DateTime toDate)
		{
			DataTable table = new DataTable();
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("Count", typeof(int));

			DataRow row;
			foreach (HourAndCount entry in GetHourReport(fromDate, toDate))
			{
				row = table.NewRow();

				row["Title"] = entry.Hour.ToString();
				row["Count"] = entry.Count;

				table.Rows.Add(row);
			}
			return table;
		}
		#endregion

		#region GetDayOfWeekReport
		/// <summary>
		/// Returns ArrayList of DayOfWeekAndCount objects.
		/// </summary>
		public static ArrayList GetDayOfWeekReport(DateTime fromDate, DateTime toDate, DateTime StartWeekDay)
		{
			ArrayList list = new ArrayList();

			for (int i = 0; i < 7; i++)
			{
				//DayOfWeek dow=(DayOfWeek)Enum.Parse(typeof(DayOfWeek), i.ToString());
				list.Add(new DayOfWeekAndCount(StartWeekDay.ToString("dddd")));
				StartWeekDay = StartWeekDay.AddDays(1);
			}

			int DefaultBias = User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			using (IDataReader reader = DBReport.GetSessions(fromDate, toDate, DefaultBias))
			{
				if (reader != null)
				{
					while (reader.Read())
					{
						DateTime dt = new DateTime(1970, 1, 1).AddSeconds(reader.GetInt32(0));
						int j = (int)dt.DayOfWeek;
						if (Security.CurrentUser.Culture.IndexOf("ru") >= 0)
						{
							if (j > 0) j--;
							else j = 6;
						}
						((DayOfWeekAndCount)list[j]).Count++;
					}
				}
			}

			return list;
		}
		#endregion

		#region GetHourReport
		/// <summary>
		/// Returns ArrayList of HourAndCount objects.
		/// </summary>
		public static ArrayList GetHourReport(DateTime fromDate, DateTime toDate)
		{
			ArrayList list = new ArrayList();

			for (int i = 0; i < 24; i++)
				list.Add(new HourAndCount(i));

			int DefaultBias = User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			using (IDataReader reader = DBReport.GetSessions(fromDate, toDate, DefaultBias))
			{
				if (reader != null)
				{
					while (reader.Read())
					{
						DateTime dt = new DateTime(1970, 1, 1).AddSeconds(reader.GetInt32(0));
						((HourAndCount)list[dt.Hour]).Count++;
					}
				}
			}

			return list;
		}
		#endregion

		#region SearchHistory
		public static DataTable SearchHistory(DateTime fromDate, DateTime toDate,
												int userID, int userGroupId,
												int contUserID, int contUserGroupId,
												int msgType, string keyword, bool displayLastEventsFirst)
		{
			ArrayList UserList = new ArrayList();
			ArrayList ContactList = new ArrayList();
			using (IDataReader rdr = SecureGroup.GetListAllUsersInGroup(userGroupId))
			{
				while (rdr.Read())
				{
					if (rdr["OriginalId"] != DBNull.Value)
						UserList.Add((int)rdr["OriginalId"]);
				}
			}

			using (IDataReader rdr = SecureGroup.GetListAllUsersInGroup(contUserGroupId))
			{
				while (rdr.Read())
				{
					if (rdr["OriginalId"] != DBNull.Value)
						ContactList.Add((int)rdr["OriginalId"]);
				}
			}

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("FromFirst", typeof(string)));
			dt.Columns.Add(new DataColumn("FromLast", typeof(string)));
			dt.Columns.Add(new DataColumn("ToFirst", typeof(string)));
			dt.Columns.Add(new DataColumn("ToLast", typeof(string)));
			dt.Columns.Add(new DataColumn("mess_text", typeof(string)));
			dt.Columns.Add(new DataColumn("send_time", typeof(DateTime)));
			DataRow dr;

			int DefaultBias = User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			using (IDataReader reader = DBReport.SearchHistory(fromDate, toDate,
					  userID, contUserID,
					  msgType, keyword, displayLastEventsFirst,
					  DefaultBias))
			{
				while (reader.Read())
				{
					dr = dt.NewRow();
					int FromId = (int)reader["FromID"];
					int ToId = (int)reader["ToID"];
					if (msgType == 1)	//in + out
					{
						if ((!UserList.Contains(FromId) || !ContactList.Contains(ToId)) &&
							(!UserList.Contains(ToId) || !ContactList.Contains(FromId))) continue;
					}
					else if (msgType == 2) //in
					{
						if (!UserList.Contains(ToId) || !ContactList.Contains(FromId)) continue;
					}
					else //out
					{
						if (!UserList.Contains(FromId) || !ContactList.Contains(ToId)) continue;
					}
					dr["FromFirst"] = reader["FromFirst"].ToString();
					dr["FromLast"] = reader["FromLast"].ToString();
					dr["ToFirst"] = reader["ToFirst"].ToString();
					dr["ToLast"] = reader["ToLast"].ToString();
					dr["mess_text"] = reader["mess_text"].ToString();
					dr["send_time"] = (DateTime)reader["send_time"];
					dt.Rows.Add(dr);
				}
			}

			return dt;
		}
		#endregion

		#region GetChats
		public static IDataReader GetChats(int iUserId)
		{
			return DBReport.GetChats(iUserId);
		}
		#endregion

		#region GetGroupsCount
		public static int GetGroupsCount()
		{
			return DBReport.GetGroupsCount();
		}
		#endregion

		#region GetUsersByCompany
		public static IDataReader GetUsersByCompany()
		{
			return DBReport.GetUsersByCompany();
		}
		#endregion

		#region GetUsersCount
		public static int GetUsersCount()
		{
			return DBReport.GetUsersCount();
		}
		#endregion

		#region GetActiveUsersCount
		public static int GetActiveUsersCount()
		{
			return DBReport.GetActiveUsersCount();
		}
		#endregion

		#region GetInActiveUsersCount
		public static int GetInActiveUsersCount()
		{
			return DBReport.GetInActiveUsersCount();
		}
		#endregion

		#region GetCountIMMessages
		public static int GetCountIMMessages(DateTime StartDate, DateTime EndDate, bool getInternal)
		{
			int DefaultBias = User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBReport.GetCountIMMessages(StartDate, EndDate, getInternal, DefaultBias);
		}
		#endregion

		#region GetCountIMMessages
		public static int GetCountIMMessages(bool getInternal)
		{
			int DefaultBias = User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBReport.GetCountIMMessages(Report.DefaultStartDate, Report.DefaultEndDate, getInternal, DefaultBias);
		}
		#endregion

		#region GetCountChatMessages
		public static int GetCountChatMessages(DateTime StartDate, DateTime EndDate)
		{
			int DefaultBias = User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBReport.GetCountChatMessages(StartDate, EndDate, DefaultBias);
		}
		#endregion

		#region GetCountChatMessages
		public static int GetCountChatMessages()
		{
			int DefaultBias = User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBReport.GetCountChatMessages(Report.DefaultStartDate, Report.DefaultEndDate, DefaultBias);
		}
		#endregion

		#region GetFilesTransferred
		public static int GetFilesTransferred(DateTime StartDate, DateTime EndDate)
		{
			int DefaultBias = User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBReport.GetFilesTransferred(StartDate, EndDate, DefaultBias);
		}
		#endregion

		#region GetFilesTransferred
		public static int GetFilesTransferred()
		{
			int DefaultBias = User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBReport.GetFilesTransferred(Report.DefaultStartDate, Report.DefaultEndDate, DefaultBias);
		}
		#endregion

		/*#region GetBytesTransferred
		public static int GetBytesTransferred(DateTime StartDate,DateTime EndDate)
		{
			int DefaultBias = User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBReport.GetBytesTransferred(StartDate,EndDate, Configuration.CompanyId, DefaultBias);
		}
		#endregion

		#region GetBytesTransferred
		public static int GetBytesTransferred()
		{
			int DefaultBias = User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBReport.GetBytesTransferred(DateTime.Now,DateTime.Now, Configuration.CompanyId, DefaultBias);
		}
		#endregion*/

		#region GetAuthenticatedUsers
		public static int GetAuthenticatedUsers(DateTime StartDate, DateTime EndDate)
		{
			int DefaultBias = User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBReport.GetAuthenticatedUsers(StartDate, EndDate, DefaultBias);
		}
		#endregion

		#region GetConferencesCreated
		public static int GetConferencesCreated(DateTime StartDate)
		{
			int DefaultBias = User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBReport.GetConferencesCreated(StartDate, DefaultBias);
		}
		#endregion

		#region GetTop10Users
		public static DataView GetTop10Users(DateTime StartDate, DateTime EndDate)
		{
			int DefaultBias = User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			DataTable dt = new DataTable("Top10writers");
			dt.Columns.Add("Place");
			dt.Columns.Add("Group");
			dt.Columns.Add("Name");
			dt.Columns.Add("TotalMessages");

			using (IDataReader reader = DBReport.GetTop10Users(StartDate, EndDate, DefaultBias))
			{
				int place = 1;
				while (reader.Read())
				{
					int iUserId = reader.GetInt32(0);
					string sName = "";
					int iIMGroupId = 0;
					using (IDataReader _obj = User.GetUserInfoByOriginalId(iUserId))
					{
						if (_obj.Read())
						{
							sName = _obj["FirstName"].ToString() + " " + _obj["LastName"].ToString();
							iIMGroupId = (int)_obj["IMGroupId"];
						}
						else
							continue;
					}
					DataRow dr = dt.NewRow();
					dr["Place"] = place.ToString();
					dr["TotalMessages"] = reader.GetInt32(1).ToString();
					dr["Name"] = sName;
					if (iIMGroupId > 0)
					{
						string groupName = IMGroup.GetIMGroupName(iIMGroupId, null);
						if(groupName != null)
							dr["Group"] = groupName;
					}
					dt.Rows.Add(dr);
					place++;
				}
			}

			return dt.DefaultView;
		}
		#endregion

		#region GetGroupStats
		public static DataTable GetGroupStats(int IMGroupId, DateTime fromDate, DateTime toDate)
		{
			int DefaultBias = User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);

			DataTable table = new DataTable();
			table.Columns.Add("AccActive", typeof(int));
			table.Columns.Add("AccInactive", typeof(int));
			table.Columns.Add("MsgIM", typeof(int));
			table.Columns.Add("MsgConf", typeof(int));
			table.Columns.Add("TransfFiles", typeof(int));
			table.Columns.Add("TransfBytes", typeof(int));
			table.Columns.Add("Confs", typeof(int));

			DataRow row = table.NewRow();
			using (IDataReader reader = DBReport.GetGroupStats(IMGroupId, fromDate, toDate, DefaultBias))
			{
				if (reader != null)
				{
					if (reader.Read())
					{
						row["AccActive"] = (int)reader["AccActive"];
						row["AccInactive"] = (int)reader["AccInactive"];
						row["MsgIM"] = (int)reader["MsgIM"];
						row["MsgConf"] = (int)reader["MsgConf"];
						row["TransfFiles"] = (int)reader["TransfFiles"];
						row["TransfBytes"] = (int)reader["TransfBytes"];
						row["Confs"] = (int)reader["Confs"];
					}
				}
			}
			table.Rows.Add(row);
			return table;
		}
		#endregion

		#region GetListTasksForUser
		/// <summary>
		/// Reader contains fields:
		///  TaskId, Title, ProjectTitle, ManagerName, StartDate, FinishDate, PercentCompleted, 
		///  IsCompleted, StateId
		/// </summary>
		/// <param name="ProjectId"></param>
		/// <returns></returns>
		public static IDataReader GetListAllTasksForUser()
		{
			int user_id = Security.CurrentUser.UserID;
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			return DBReport.GetListAllTasksForUser(user_id, 0, TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListAllToDoForUserByProject
		/// <summary>
		/// DataTable contains fields:
		///  ToDoId, Title, Description, PriorityId, PriorityName, StartDate, FinishDate, PercentCompleted
		/// </summary>
		/// <param name="ProjectId"></param>
		/// <returns></returns>
		public static IDataReader GetListAllToDoForUserByProject(int user_id, int project_id)
		{
			//int user_id = Security.CurrentUser.UserID;
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			return DBReport.GetListAllToDoForUserByProject(project_id, user_id, TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region SearchChat
		public static IDataReader SearchChat(int iUserId, int iChatId, DateTime dtFrom, DateTime dtTo,
			string sKeyWord, int iOrder)
		{
			int DefaultBias = User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBReport.SearchChat(iUserId, iChatId, dtFrom, dtTo, sKeyWord, iOrder, DefaultBias);
		}
		#endregion

		#region GetEvent
		public static IDataReader GetEvent(int event_id)
		{
			return DBEvent.GetEvent(event_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetQuickSnapshotReport
		public static IDataReader GetQuickSnapshotReport(DateTime StartDate, DateTime EndDate, int CreatorId)
		{
			StartDate = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, StartDate);
			EndDate = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, EndDate);
			return DBReport.GetQuickSnapshotReport(StartDate, EndDate, CreatorId);
		}
		#endregion

		#region GetSecureGroupStats
		/// <summary>
		/// ActiveAccounts, InactiveAccounts, NewProjectsCount, NewEventsCount, 
		/// NewIncidentsCount, NewToDosCount, NewTasksCount, PortalLogins
		/// </summary>
		private static IDataReader GetSecGroupStats(int GroupID, DateTime fromDate, DateTime toDate, bool ShowAll)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			fromDate = DBCommon.GetUTCDate(TimeZoneId, fromDate);
			toDate = DBCommon.GetUTCDate(TimeZoneId, toDate);

			return DBReport.GetSecGroupStats(GroupID, fromDate, toDate, ShowAll);
		}
		#endregion

		#region GetSecureGroupStats
		/// <summary>
		/// ActiveAccounts, InactiveAccounts, NewProjectsCount, NewEventsCount, 
		/// NewIncidentsCount, NewToDosCount, NewTasksCount.
		/// </summary>
		public static IDataReader GetSecGroupStats(int GroupID, DateTime fromDate, DateTime toDate)
		{
			return GetSecGroupStats(GroupID, fromDate, toDate, true);
		}
		#endregion

		#region GetListAllTasksForUserByProject
		/// <summary>
		/// DataTable contains fields:
		///  TaskId, Title, Description, PriorityId, PriorityName, StartDate, FinishDate, PercentCompleted
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListAllTasksForUserByProject(int user_id, int project_id)
		{
			return DBReport.GetListAllTasksForUser(user_id, project_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListAllEventsForUserByProject
		/// <summary>
		/// DataTable contains fields:
		///  EventId, Title, Description, PriorityId, PriorityName, TypeId, StartDate, FinishDate, 
		///  StateId, HasRecurrence
		/// </summary>
		/// <param name="ProjectId"></param>
		/// <returns></returns>
		public static IDataReader GetListAllEventsForUserByProject(int user_id, int project_id)
		{
			return DBReport.GetListAllEventsForUserByProject(project_id, user_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListStatusReportElementsByUser
		/// <summary>
		/// ObjectId, ObjectTypeId, Title, ProjectId, ProjectTitle, ActualFinishDate, 
		/// IsCompleted, ManagerId, ManagerName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListStatusReportElementsByUser(DateTime FromDate,
			DateTime ToDate, bool IncludeEvents, bool IncludeActiveTasks, ArrayList Categories)
		{
			FromDate = FromDate.Date;
			ToDate = ToDate.Date;

			int UserId = Security.CurrentUser.UserID;
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			FromDate = DBCommon.GetUTCDate(TimeZoneId, FromDate);
			ToDate = DBCommon.GetUTCDate(TimeZoneId, ToDate);

			bool CheckCategories = (Categories == null || Categories.Count <= 0) ? false : true;

			DataRow row;
			DataTable table = new DataTable();
			table.Columns.Add("ObjectId", typeof(int));
			table.Columns.Add("ObjectTypeId", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("ProjectId", typeof(int));
			table.Columns.Add("ProjectTitle", typeof(string));
			table.Columns.Add("ActualFinishDate", typeof(DateTime));
			table.Columns.Add("IsCompleted", typeof(bool));
			table.Columns.Add("ManagerId", typeof(int));
			table.Columns.Add("ManagerName", typeof(string));

			// ActiveTasks
			if (IncludeActiveTasks)
			{
				using (IDataReader reader = DBToDo.GetListActiveToDoAndTasksByUser(FromDate, ToDate, UserId, TimeZoneId))
				{
					while (reader.Read())
					{
						row = table.NewRow();
						int ObjectId = (int)reader["ObjectId"];
						row["ObjectId"] = ObjectId;
						if ((int)reader["IsToDo"] == 1)
						{
							row["ObjectTypeId"] = (int)ObjectType.ToDo;
							if ((int)reader["IsIncidentTodo"] == 1)
								row["ObjectTypeId"] = (int)ObjectType.IncidentToDo;
							if ((int)reader["IsDocumentTodo"] == 1)
								row["ObjectTypeId"] = (int)ObjectType.DocumentToDo;
							if ((int)reader["IsTaskTodo"] == 1)
								row["ObjectTypeId"] = (int)ObjectType.TaskToDo;
						}
						else
						{
							row["ObjectTypeId"] = (int)ObjectType.Task;
						}
						int ObjectTypeId = (int)row["ObjectTypeId"];

						row["Title"] = reader["Title"];
						row["ProjectId"] = reader["ProjectId"];
						row["ProjectTitle"] = reader["ProjectTitle"];
						row["ActualFinishDate"] = reader["ActualFinishDate"];
						row["IsCompleted"] = reader["IsCompleted"];

						if (reader["ManagerId"] != DBNull.Value)
							row["ManagerId"] = reader["ManagerId"];
						else
							row["ManagerId"] = -1;

						if (reader["ManagerName"] != DBNull.Value)
							row["ManagerName"] = reader["ManagerName"];
						else
							row["ManagerName"] = "";

						// Check for duplicate
						DataRow[] rows = table.Select("ObjectId = " + ObjectId + " AND ObjectTypeId = " + ObjectTypeId);
						if (rows.Length <= 0)
						{
							if (!CheckCategories || Categories.Contains(reader["CategoryId"]))
								table.Rows.Add(row);
						}
					}
				}
			}

			// Events
			if (IncludeEvents)
			{
				int mask = (int)CalendarView.CalendarFilter.Appointment
					| (int)CalendarView.CalendarFilter.Event
					| (int)CalendarView.CalendarFilter.Meeting;
				DataTable eventsTable = CalendarView.GetListCalendarEntriesByUser(
					FromDate, ToDate, true, true, false, mask, UserId);
				foreach (DataRow dr in eventsTable.Rows)
				{
					row = table.NewRow();

					int ObjectId = (int)dr["ID"];
					int ObjectTypeId = (int)dr["Type"];
					row["ObjectId"] = ObjectId;
					row["ObjectTypeId"] = ObjectTypeId;
					row["Title"] = dr["Title"];
					row["ActualFinishDate"] = dr["FinishDate"];
					if ((int)dr["StateId"] == (int)ObjectStates.Suspended || (int)dr["StateId"] == (int)ObjectStates.Completed)
						row["IsCompleted"] = true;
					else
						row["IsCompleted"] = false;

					// Check for duplicate
					DataRow[] rows = table.Select("ObjectId = " + ObjectId + " AND ObjectTypeId = " + ObjectTypeId);
					if (rows.Length <= 0)
					{
						int ProjectId = DBEvent.GetProject(ObjectId);
						if (ProjectId > 0)
						{
							row["ProjectId"] = ProjectId;
							row["ProjectTitle"] = DBProject.GetTitle(ProjectId);
						}
						else
						{
							row["ProjectId"] = DBNull.Value;
							row["ProjectTitle"] = DBNull.Value;
						}

						if (!CheckCategories || CheckEventCategories(ObjectId, Categories))
							table.Rows.Add(row);
					}
				}
			}

			return table;
		}
		#endregion

		#region GetListStatusReportElementsByProject
		/// <summary>
		/// ObjectId, ObjectTypeId, Title, ActualFinishDate, IsCompleted
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListStatusReportElementsByProject(int ProjectId, DateTime FromDate,
			DateTime ToDate, bool IncludeEvents, bool IncludeActiveTasks, ArrayList Categories)
		{
			FromDate = FromDate.Date;
			ToDate = ToDate.Date;

			int UserId = Security.CurrentUser.UserID;
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			FromDate = DBCommon.GetUTCDate(TimeZoneId, FromDate);
			ToDate = DBCommon.GetUTCDate(TimeZoneId, ToDate);

			bool CheckCategories = (Categories == null || Categories.Count <= 0) ? false : true;

			DataRow row;
			DataTable table = new DataTable();
			table.Columns.Add("ObjectId", typeof(int));
			table.Columns.Add("ObjectTypeId", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("ActualFinishDate", typeof(DateTime));
			table.Columns.Add("IsCompleted", typeof(bool));

			// ActiveTasks
			if (IncludeActiveTasks)
			{
				using (IDataReader reader = DBToDo.GetListActiveToDoAndTasksByProject(FromDate, ToDate, ProjectId, TimeZoneId))
				{
					while (reader.Read())
					{
						row = table.NewRow();
						int ObjectId = (int)reader["ObjectId"];
						row["ObjectId"] = ObjectId;
						if ((int)reader["IsToDo"] == 1)
						{
							row["ObjectTypeId"] = (int)ObjectType.ToDo;
							if ((int)reader["IsIncidentTodo"] == 1)
								row["ObjectTypeId"] = (int)ObjectType.IncidentToDo;
							if ((int)reader["IsDocumentTodo"] == 1)
								row["ObjectTypeId"] = (int)ObjectType.DocumentToDo;
							if ((int)reader["IsTaskTodo"] == 1)
								row["ObjectTypeId"] = (int)ObjectType.TaskToDo;
						}
						else
						{
							row["ObjectTypeId"] = (int)ObjectType.Task;
						}
						int ObjectTypeId = (int)row["ObjectTypeId"];

						row["Title"] = reader["Title"];
						row["ActualFinishDate"] = reader["ActualFinishDate"];
						row["IsCompleted"] = reader["IsCompleted"];

						// Check for duplicate
						DataRow[] rows = table.Select("ObjectId = " + ObjectId + " AND ObjectTypeId = " + ObjectTypeId);
						if (rows.Length <= 0)
						{
							if (!CheckCategories || Categories.Contains(reader["CategoryId"]))
								table.Rows.Add(row);
						}
					}
				}
			}

			// Events
			if (IncludeEvents)
			{
				int mask = (int)CalendarView.CalendarFilter.Appointment
					| (int)CalendarView.CalendarFilter.Event
					| (int)CalendarView.CalendarFilter.Meeting;
				DataTable eventsTable = CalendarView.GetListCalendarEntries(
					FromDate, ToDate, true, true, false, mask, 0, ProjectId);
				foreach (DataRow dr in eventsTable.Rows)
				{
					row = table.NewRow();

					int ObjectId = (int)dr["ID"];
					int ObjectTypeId = (int)dr["Type"];
					row["ObjectId"] = ObjectId;
					row["ObjectTypeId"] = ObjectTypeId;
					row["Title"] = dr["Title"];
					row["ActualFinishDate"] = dr["FinishDate"];
					if ((int)dr["StateId"] == (int)ObjectStates.Suspended || (int)dr["StateId"] == (int)ObjectStates.Completed)
						row["IsCompleted"] = true;
					else
						row["IsCompleted"] = false;

					// Check for duplicate
					DataRow[] rows = table.Select("ObjectId = " + ObjectId + " AND ObjectTypeId = " + ObjectTypeId);
					if (rows.Length <= 0)
					{
						if (!CheckCategories || CheckEventCategories(ObjectId, Categories))
							table.Rows.Add(row);
					}
				}
			}

			return table;
		}
		#endregion

		#region CheckEventCategories
		private static bool CheckEventCategories(int EventId, ArrayList Categories)
		{
			bool retval = false;
			using (IDataReader reader = CalendarEntry.GetListCategories(EventId))
			{
				while (reader.Read())
				{
					if (Categories.Contains((int)reader["CategoryId"]))
					{
						retval = true;
						break;
					}
				}
			}
			return retval;
		}
		#endregion

		#region AddTemplateInfo
		private static void AddTemplateInfo(XmlDocument doc, IBNReportTemplate repTemp)
		{
			XmlDocument xmlTemplateDoc = repTemp.CreateXMLTemplate();

			XmlNode xmlRoot = xmlTemplateDoc.SelectSingleNode("IBNReportTemplate");

			XmlNode xmlNode_Root = doc.ImportNode(xmlRoot, true);
			doc.SelectSingleNode("Report").AppendChild(xmlNode_Root);
		}
		#endregion

		/*#region AddGroupInfo
		private static void AddGroupInfo(XmlDocument doc, IBNReportTemplate repTemp)
		{
			XmlDocument	xmlTemplateDoc = repTemp.CreateXMLTemplate();

			XmlNode xmlGroups  = xmlTemplateDoc.SelectSingleNode("IBNReportTemplate/Groups");

			XmlNode xmlNodeG  = doc.ImportNode(xmlGroups,true);
			doc.SelectSingleNode("Report").AppendChild(xmlNodeG);
		}	
		#endregion*/

		#region GetIncidentXMLReport
		public static XmlDocument GetIncidentXMLReport(IBNReportTemplate repTemp, string _lang, string _timeoffset)
		{
			QIncident objI = new QIncident();

			QMaker qMaker = new QMaker(objI);
			qMaker.Language = _lang;
			qMaker.TimeOffset = _timeoffset;

			#region Fields
			foreach (FieldInfo fi in repTemp.Fields)
			{
				qMaker.Fields.Add(objI.Fields[fi.Name]);
			}
			#endregion

			#region GroupFields
			foreach (FieldInfo fi1 in repTemp.Groups)
			{
				qMaker.Groups.Add(objI.Fields[fi1.Name]);
			}
			#endregion

			#region Filters
			bool PrjFilterExist = false;
			foreach (FilterInfo fti in repTemp.Filters)
			{
				if (fti.FieldName == "IncProjectId")
					PrjFilterExist = true;
				QField qField = objI.Fields[fti.FieldName];
				QDictionary qDic = objI.GetDictionary(qField);
				if (qDic != null)
				{
					if (fti.Values.Count > 0)
					{
						string[] _str = new string[fti.Values.Count];
						int i = 0;
						foreach (string s in fti.Values)
							_str[i++] = s;
						qMaker.Filters.Add(new SimpleFilterCondition(objI.Fields[qDic.FieldId.Name], _str, SimpleFilterType.Equal));
					}
				}
				else
				{
					switch (qField.DataType)
					{
						case DbType.Decimal:
						case DbType.Int32:
						case DbType.Time:
							if (fti.Values.Count > 0)
							{
								switch (fti.Values[0])
								{
									case "0":
										qMaker.Filters.Add(new SimpleFilterCondition(objI.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Equal));
										break;
									case "1":
										qMaker.Filters.Add(new SimpleFilterCondition(objI.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Great));
										break;
									case "2":
										qMaker.Filters.Add(new SimpleFilterCondition(objI.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Less));
										break;
									case "3":
										qMaker.Filters.Add(new IntervalFilterCondition(objI.Fields[fti.FieldName], fti.Values[1].ToString(), fti.Values[2].ToString()));
										break;
								}
							}
							break;
						case DbType.DateTime:
						case DbType.Date:
							if (fti.Values.Count > 0)
							{
								DateTime _Start = DateTime.MinValue;
								DateTime _Finish = DateTime.MaxValue;
								string sVal = fti.Values[0];
								SetDates(sVal, out _Start, out _Finish);
								if (sVal != "9")
								{
									qMaker.Filters.Add(new IntervalFilterCondition(objI.Fields[fti.FieldName], "CONVERT(datetime,'" + _Start.ToString("yyyy-MM-dd") + "', 120)", "CONVERT(datetime,'" + _Finish.ToString("yyyy-MM-dd") + "', 120)"));
								}
								else if (DateTime.Parse(fti.Values[1]) == DateTime.MinValue)
								{
									qMaker.Filters.Add(new SimpleFilterCondition(objI.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[2]).ToString("yyyy-MM-dd") + "', 120)", SimpleFilterType.LessOrEqual));
								}
								else if (DateTime.Parse(fti.Values[2]) >= DateTime.MaxValue.Date)
								{
									qMaker.Filters.Add(new SimpleFilterCondition(objI.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[1]).ToString("yyyy-MM-dd") + "', 120)", SimpleFilterType.GreatOrEqual));
								}
								else
								{
									qMaker.Filters.Add(new IntervalFilterCondition(objI.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[1]).ToString("yyyy-MM-dd") + "', 120)", "CONVERT(datetime,'" + DateTime.Parse(fti.Values[2]).ToString("yyyy-MM-dd") + "', 120)"));
								}
							}
							break;
						case DbType.String:
							if (fti.Values.Count > 0)
							{
								qMaker.Filters.Add(new SimpleFilterCondition(objI.Fields[fti.FieldName], GetApString(fti.Values[0]), SimpleFilterType.Like));
							}
							break;
					}
				}
			}
			if (!PrjFilterExist &&
					!(Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) || Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager)))
			{
				ArrayList alPrj = new ArrayList();
				using (IDataReader reader = Project.GetListProjects())
				{
					while (reader.Read())
						alPrj.Add(((int)reader["ProjectId"]).ToString());
				}
				alPrj.Add("NULL");
				string[] _str = new string[alPrj.Count];
				int i = 0;
				foreach (string s in alPrj)
					_str[i++] = s;
				qMaker.Filters.Add(new SimpleFilterCondition(objI.Fields["IncProjectId"], _str, SimpleFilterType.Equal));
			}
			#endregion

			string sql = qMaker.Create();
			XmlDocument doc = DBReport.GetXMLReport(sql);
			QMetaLoader.AddHeaderInformation(doc, qMaker);
			//QMetaLoader.AddSortInformation(doc, repTemp);
			//QMetaLoader.AddFilterInformation(doc, repTemp);
			AddTemplateInfo(doc, repTemp);
			ProcessResourceValues(doc);
			return doc;
		}
		#endregion

		#region GetProjectXMLReport
		public static XmlDocument GetProjectXMLReport(IBNReportTemplate repTemp, string _lang, string _timeoffset)
		{
			QProject objP = new QProject();
			QMaker qMaker = new QMaker(objP);
			qMaker.Language = _lang;
			qMaker.TimeOffset = _timeoffset;

			#region Fields
			foreach (FieldInfo fi in repTemp.Fields)
			{
				qMaker.Fields.Add(objP.Fields[fi.Name]);
			}
			#endregion

			#region GroupFields
			foreach (FieldInfo fi1 in repTemp.Groups)
			{
				qMaker.Groups.Add(objP.Fields[fi1.Name]);
			}
			#endregion

			#region Filters
			if (!Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) &&
				!Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager))
			{
				qMaker.PrevSqlQuery.Add(String.Format("DECLARE @ProjectIdTable TABLE(Id INT) " +
					"INSERT INTO @ProjectIdTable " +
					"SELECT ProjectId FROM PROJECT_SECURITY_ALL" +
					" WHERE PrincipalId = {0} AND (IsTeamMember = 1 OR IsSponsor = 1 OR IsStakeHolder = 1 OR IsManager = 1 OR IsExecutiveManager = 1)",
					Security.CurrentUser.UserID));
				qMaker.Filters.Add(new SimpleFilterCondition(qMaker.OwnerObject.Fields["PrjId"], "(SELECT Id FROM @ProjectIdTable)", SimpleFilterType.In));
			}
			foreach (FilterInfo fti in repTemp.Filters)
			{
				QField qField = objP.Fields[fti.FieldName];
				QDictionary qDic = objP.GetDictionary(qField);
				if (qDic != null)
				{
					if (fti.Values.Count > 0)
					{
						string[] _str = new string[fti.Values.Count];
						int i = 0;
						foreach (string s in fti.Values)
							_str[i++] = s;
						qMaker.Filters.Add(new SimpleFilterCondition(objP.Fields[qDic.FieldId.Name], _str, SimpleFilterType.Equal));
					}
				}
				else
				{
					switch (qField.DataType)
					{
						case DbType.Decimal:
						case DbType.Int32:
						case DbType.Time:
							if (fti.Values.Count > 0)
							{
								switch (fti.Values[0])
								{
									case "0":
										qMaker.Filters.Add(new SimpleFilterCondition(objP.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Equal));
										break;
									case "1":
										qMaker.Filters.Add(new SimpleFilterCondition(objP.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Great));
										break;
									case "2":
										qMaker.Filters.Add(new SimpleFilterCondition(objP.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Less));
										break;
									case "3":
										qMaker.Filters.Add(new IntervalFilterCondition(objP.Fields[fti.FieldName], fti.Values[1].ToString(), fti.Values[2].ToString()));
										break;
								}
							}
							break;
						case DbType.DateTime:
						case DbType.Date:
							if (fti.Values.Count > 0)
							{
								DateTime _Start = DateTime.MinValue;
								DateTime _Finish = DateTime.MaxValue;
								string sVal = fti.Values[0];
								SetDates(sVal, out _Start, out _Finish);
								if (sVal != "9")
								{
									qMaker.Filters.Add(new IntervalFilterCondition(objP.Fields[fti.FieldName], "CONVERT(datetime,'" + _Start.ToString("yyyy-MM-dd") + "',120)", "CONVERT(datetime,'" + _Finish.ToString("yyyy-MM-dd") + "',120)"));
								}
								else if (DateTime.Parse(fti.Values[1]) == DateTime.MinValue)
								{
									qMaker.Filters.Add(new SimpleFilterCondition(objP.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[2]).ToString("yyyy-MM-dd") + "',120)", SimpleFilterType.LessOrEqual));
								}
								else if (DateTime.Parse(fti.Values[2]) >= DateTime.MaxValue.Date)
								{
									qMaker.Filters.Add(new SimpleFilterCondition(objP.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[1]).ToString("yyyy-MM-dd") + "',120)", SimpleFilterType.GreatOrEqual));
								}
								else
								{
									qMaker.Filters.Add(new IntervalFilterCondition(objP.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[1]).ToString("yyyy-MM-dd") + "',120)", "CONVERT(datetime,'" + DateTime.Parse(fti.Values[2]).ToString("yyyy-MM-dd") + "',120)"));
								}
							}
							break;
						case DbType.String:
							if (fti.Values.Count > 0)
							{
								qMaker.Filters.Add(new SimpleFilterCondition(objP.Fields[fti.FieldName], GetApString(fti.Values[0]), SimpleFilterType.Like));
							}
							break;
					}
				}
			}
			#endregion

			string sql = qMaker.Create();
			XmlDocument doc = DBReport.GetXMLReport(sql);
			QMetaLoader.AddHeaderInformation(doc, qMaker);
			//QMetaLoader.AddSortInformation(doc, repTemp);
			//QMetaLoader.AddFilterInformation(doc, repTemp);
			AddTemplateInfo(doc, repTemp);
			ProcessResourceValues(doc);
			return doc;
		}
		#endregion

		#region GetToDoXMLReport
		public static XmlDocument GetToDoXMLReport(IBNReportTemplate repTemp, string _lang, string _timeoffset)
		{
			QToDo objT = new QToDo();
			QMaker qMaker = new QMaker(objT);
			qMaker.Language = _lang;
			qMaker.TimeOffset = _timeoffset;

			#region Fields
			foreach (FieldInfo fi in repTemp.Fields)
			{
				qMaker.Fields.Add(objT.Fields[fi.Name]);
			}
			#endregion

			#region GroupFields
			foreach (FieldInfo fi1 in repTemp.Groups)
			{
				qMaker.Groups.Add(objT.Fields[fi1.Name]);
			}
			#endregion

			#region Filters
			if (!Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) &&
				!Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager))
			{
				qMaker.PrevSqlQuery.Add(String.Format(
					"DECLARE @ToDoIdTable TABLE(Id INT) " +
					"INSERT INTO @ToDoIdTable " +
					" SELECT DISTINCT TD.ToDoId As Id FROM TODO AS TD " +
					" WHERE TD.ToDoId IN (SELECT ToDoId FROM TODO_SECURITY_ALL WHERE PrincipalId = {0} AND (IsManager = 1 OR IsResource = 1))" +
					"  OR TD.ProjectId IN (SELECT ProjectId FROM PROJECT_SECURITY_ALL WHERE PrincipalId = {0} AND (IsTeamMember = 1 OR IsSponsor = 1 OR IsStakeHolder = 1 OR IsManager = 1 OR IsExecutiveManager = 1))",
					Security.CurrentUser.UserID));
				qMaker.Filters.Add(new SimpleFilterCondition(qMaker.OwnerObject.Fields["ToDoId"], "(SELECT Id FROM @ToDoIdTable)", SimpleFilterType.In));
			}
			foreach (FilterInfo fti in repTemp.Filters)
			{
				QField qField = objT.Fields[fti.FieldName];
				QDictionary qDic = objT.GetDictionary(qField);
				if (qDic != null)
				{
					if (fti.Values.Count > 0)
					{
						string[] _str = new string[fti.Values.Count];
						int i = 0;
						foreach (string s in fti.Values)
							_str[i++] = s;
						qMaker.Filters.Add(new SimpleFilterCondition(objT.Fields[qDic.FieldId.Name], _str, SimpleFilterType.Equal));
					}
				}
				else
				{
					switch (qField.DataType)
					{
						case DbType.Decimal:
						case DbType.Int32:
						case DbType.Time:
							if (fti.Values.Count > 0)
							{
								switch (fti.Values[0])
								{
									case "0":
										qMaker.Filters.Add(new SimpleFilterCondition(objT.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Equal));
										break;
									case "1":
										qMaker.Filters.Add(new SimpleFilterCondition(objT.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Great));
										break;
									case "2":
										qMaker.Filters.Add(new SimpleFilterCondition(objT.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Less));
										break;
									case "3":
										qMaker.Filters.Add(new IntervalFilterCondition(objT.Fields[fti.FieldName], fti.Values[1].ToString(), fti.Values[2].ToString()));
										break;
								}
							}
							break;
						case DbType.DateTime:
						case DbType.Date:
							if (fti.Values.Count > 0)
							{
								DateTime _Start = DateTime.MinValue;
								DateTime _Finish = DateTime.MaxValue;
								string sVal = fti.Values[0];
								SetDates(sVal, out _Start, out _Finish);
								if (sVal != "9")
								{
									qMaker.Filters.Add(new IntervalFilterCondition(objT.Fields[fti.FieldName], "CONVERT(datetime,'" + _Start.ToString("yyyy-MM-dd") + "',120)", "CONVERT(datetime,'" + _Finish.ToString("yyyy-MM-dd") + "',120)"));
								}
								else if (DateTime.Parse(fti.Values[1]) == DateTime.MinValue)
								{
									qMaker.Filters.Add(new SimpleFilterCondition(objT.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[2]).ToString("yyyy-MM-dd") + "',120)", SimpleFilterType.LessOrEqual));
								}
								else if (DateTime.Parse(fti.Values[2]) >= DateTime.MaxValue.Date)
								{
									qMaker.Filters.Add(new SimpleFilterCondition(objT.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[1]).ToString("yyyy-MM-dd") + "',120)", SimpleFilterType.GreatOrEqual));
								}
								else
								{
									qMaker.Filters.Add(new IntervalFilterCondition(objT.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[1]).ToString("yyyy-MM-dd") + "',120)", "CONVERT(datetime,'" + DateTime.Parse(fti.Values[2]).ToString("yyyy-MM-dd") + "',120)"));
								}
							}
							break;
						case DbType.String:
							if (fti.Values.Count > 0)
							{
								qMaker.Filters.Add(new SimpleFilterCondition(objT.Fields[fti.FieldName], GetApString(fti.Values[0]), SimpleFilterType.Like));
							}
							break;
					}
				}
			}
			#endregion

			string sql = qMaker.Create();
			XmlDocument doc = DBReport.GetXMLReport(sql);
			QMetaLoader.AddHeaderInformation(doc, qMaker);
			//QMetaLoader.AddSortInformation(doc, repTemp);
			//QMetaLoader.AddFilterInformation(doc, repTemp);
			AddTemplateInfo(doc, repTemp);
			ProcessResourceValues(doc);
			return doc;
		}
		#endregion

		#region GetTaskXMLReport
		public static XmlDocument GetTaskXMLReport(IBNReportTemplate repTemp, string _lang, string _timeoffset)
		{
			QTask objT = new QTask();
			QMaker qMaker = new QMaker(objT);
			qMaker.Language = _lang;
			qMaker.TimeOffset = _timeoffset;

			#region Fields
			foreach (FieldInfo fi in repTemp.Fields)
			{
				qMaker.Fields.Add(objT.Fields[fi.Name]);
			}
			#endregion

			#region GroupFields
			foreach (FieldInfo fi1 in repTemp.Groups)
			{
				qMaker.Groups.Add(objT.Fields[fi1.Name]);
			}
			#endregion

			#region Filters
			if (!Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) &&
				!Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager))
			{
				qMaker.PrevSqlQuery.Add(String.Format("DECLARE @TaskIdTable TABLE(Id INT) " +
					"INSERT INTO @TaskIdTable " +
					"SELECT DISTINCT TS.TaskId As Id FROM TASKS AS TS " +
					"WHERE TS.TaskId IN (SELECT TaskId FROM TASK_SECURITY WHERE PrincipalId = {0} AND (IsManager = 1 OR IsResource = 1))" +
					"  OR TS.ProjectId IN (SELECT ProjectId FROM PROJECT_SECURITY_ALL WHERE PrincipalId = {0} AND (IsTeamMember = 1 OR IsSponsor = 1 OR IsStakeHolder = 1 OR IsManager = 1 OR IsExecutiveManager = 1))", 
					Security.CurrentUser.UserID));
				qMaker.Filters.Add(new SimpleFilterCondition(qMaker.OwnerObject.Fields["ToDoId"], "(SELECT Id FROM @TaskIdTable)", SimpleFilterType.In));
			}
			foreach (FilterInfo fti in repTemp.Filters)
			{
				QField qField = objT.Fields[fti.FieldName];
				QDictionary qDic = objT.GetDictionary(qField);
				if (qDic != null)
				{
					if (fti.Values.Count > 0)
					{
						string[] _str = new string[fti.Values.Count];
						int i = 0;
						foreach (string s in fti.Values)
							_str[i++] = s;
						qMaker.Filters.Add(new SimpleFilterCondition(objT.Fields[qDic.FieldId.Name], _str, SimpleFilterType.Equal));
					}
				}
				else
				{
					switch (qField.DataType)
					{
						case DbType.Decimal:
						case DbType.Int32:
						case DbType.Time:
							if (fti.Values.Count > 0)
							{
								switch (fti.Values[0])
								{
									case "0":
										qMaker.Filters.Add(new SimpleFilterCondition(objT.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Equal));
										break;
									case "1":
										qMaker.Filters.Add(new SimpleFilterCondition(objT.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Great));
										break;
									case "2":
										qMaker.Filters.Add(new SimpleFilterCondition(objT.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Less));
										break;
									case "3":
										qMaker.Filters.Add(new IntervalFilterCondition(objT.Fields[fti.FieldName], fti.Values[1].ToString(), fti.Values[2].ToString()));
										break;
								}
							}
							break;
						case DbType.DateTime:
						case DbType.Date:
							if (fti.Values.Count > 0)
							{
								DateTime _Start = DateTime.MinValue;
								DateTime _Finish = DateTime.MaxValue;
								string sVal = fti.Values[0];
								SetDates(sVal, out _Start, out _Finish);
								if (sVal != "9")
								{
									qMaker.Filters.Add(new IntervalFilterCondition(objT.Fields[fti.FieldName], "CONVERT(datetime,'" + _Start.ToString("yyyy-MM-dd") + "',120)", "CONVERT(datetime,'" + _Finish.ToString("yyyy-MM-dd") + "',120)"));
								}
								else if (DateTime.Parse(fti.Values[1]) == DateTime.MinValue)
								{
									qMaker.Filters.Add(new SimpleFilterCondition(objT.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[2]).ToString("yyyy-MM-dd") + "',120)", SimpleFilterType.LessOrEqual));
								}
								else if (DateTime.Parse(fti.Values[2]) >= DateTime.MaxValue.Date)
								{
									qMaker.Filters.Add(new SimpleFilterCondition(objT.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[1]).ToString("yyyy-MM-dd") + "',120)", SimpleFilterType.GreatOrEqual));
								}
								else
								{
									qMaker.Filters.Add(new IntervalFilterCondition(objT.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[1]).ToString("yyyy-MM-dd") + "',120)", "CONVERT(datetime,'" + DateTime.Parse(fti.Values[2]).ToString("yyyy-MM-dd") + "',120)"));
								}
							}
							break;
						case DbType.String:
							if (fti.Values.Count > 0)
							{
								qMaker.Filters.Add(new SimpleFilterCondition(objT.Fields[fti.FieldName], GetApString(fti.Values[0]), SimpleFilterType.Like));
							}
							break;
					}
				}
			}
			#endregion

			string sql = qMaker.Create();
			XmlDocument doc = DBReport.GetXMLReport(sql);
			QMetaLoader.AddHeaderInformation(doc, qMaker);
			//QMetaLoader.AddSortInformation(doc, repTemp);
			//QMetaLoader.AddFilterInformation(doc, repTemp);
			AddTemplateInfo(doc, repTemp);
			ProcessResourceValues(doc);
			return doc;
		}
		#endregion

		#region GetCalendarEntryXMLReport
		public static XmlDocument GetCalendarEntryXMLReport(IBNReportTemplate repTemp, string _lang, string _timeoffset)
		{
			QCalendarEntries objC = new QCalendarEntries();
			QMaker qMaker = new QMaker(objC);
			qMaker.Language = _lang;
			qMaker.TimeOffset = _timeoffset;

			#region Fields
			foreach (FieldInfo fi in repTemp.Fields)
			{
				qMaker.Fields.Add(objC.Fields[fi.Name]);
			}
			#endregion

			#region GroupFields
			foreach (FieldInfo fi1 in repTemp.Groups)
			{
				qMaker.Groups.Add(objC.Fields[fi1.Name]);
			}
			#endregion

			#region Filters
			if (!Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) &&
				!Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager))
			{
				qMaker.PrevSqlQuery.Add(String.Format("DECLARE @EventIdTable TABLE(Id INT) " +
					"INSERT INTO @EventIdTable " +
					"SELECT DISTINCT EV.EventId As Id FROM EVENTS AS EV " +
					"WHERE (EV.EventId IN (SELECT EventId FROM EVENT_SECURITY_ALL S WHERE PrincipalId ={0} AND (IsResource = 1 OR IsManager = 1)))" +
					"OR (EV.ProjectId IN (SELECT ProjectId FROM PROJECT_SECURITY_ALL WHERE PrincipalId = {0} AND (IsTeamMember = 1 OR IsSponsor = 1 OR IsStakeHolder = 1 OR IsManager = 1 OR IsExecutiveManager = 1)))", Security.CurrentUser.UserID));
				qMaker.Filters.Add(new SimpleFilterCondition(qMaker.OwnerObject.Fields["EventId"], "(SELECT Id FROM @EventIdTable)", SimpleFilterType.In));
			}

			foreach (FilterInfo fti in repTemp.Filters)
			{
				QField qField = objC.Fields[fti.FieldName];
				QDictionary qDic = objC.GetDictionary(qField);
				if (qDic != null)
				{
					if (fti.Values.Count > 0)
					{
						string[] _str = new string[fti.Values.Count];
						int i = 0;
						foreach (string s in fti.Values)
							_str[i++] = s;
						qMaker.Filters.Add(new SimpleFilterCondition(objC.Fields[qDic.FieldId.Name], _str, SimpleFilterType.Equal));
					}
				}
				else
				{
					switch (qField.DataType)
					{
						case DbType.Decimal:
						case DbType.Int32:
						case DbType.Time:
							if (fti.Values.Count > 0)
							{
								switch (fti.Values[0])
								{
									case "0":
										qMaker.Filters.Add(new SimpleFilterCondition(objC.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Equal));
										break;
									case "1":
										qMaker.Filters.Add(new SimpleFilterCondition(objC.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Great));
										break;
									case "2":
										qMaker.Filters.Add(new SimpleFilterCondition(objC.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Less));
										break;
									case "3":
										qMaker.Filters.Add(new IntervalFilterCondition(objC.Fields[fti.FieldName], fti.Values[1].ToString(), fti.Values[2].ToString()));
										break;
								}
							}
							break;
						case DbType.DateTime:
						case DbType.Date:
							if (fti.Values.Count > 0)
							{
								DateTime _Start = DateTime.MinValue;
								DateTime _Finish = DateTime.MaxValue;
								string sVal = fti.Values[0];
								SetDates(sVal, out _Start, out _Finish);
								if (sVal != "9")
								{
									qMaker.Filters.Add(new IntervalFilterCondition(objC.Fields[fti.FieldName], "CONVERT(datetime,'" + _Start.ToString("yyyy-MM-dd") + "',120)", "CONVERT(datetime,'" + _Finish.ToString("yyyy-MM-dd") + "',120)"));
								}
								else if (DateTime.Parse(fti.Values[1]) == DateTime.MinValue)
								{
									qMaker.Filters.Add(new SimpleFilterCondition(objC.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[2]).ToString("yyyy-MM-dd") + "',120)", SimpleFilterType.LessOrEqual));
								}
								else if (DateTime.Parse(fti.Values[2]) >= DateTime.MaxValue.Date)
								{
									qMaker.Filters.Add(new SimpleFilterCondition(objC.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[1]).ToString("yyyy-MM-dd") + "',120)", SimpleFilterType.GreatOrEqual));
								}
								else
								{
									qMaker.Filters.Add(new IntervalFilterCondition(objC.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[1]).ToString("yyyy-MM-dd") + "',120)", "CONVERT(datetime,'" + DateTime.Parse(fti.Values[2]).ToString("yyyy-MM-dd") + "',120)"));
								}
							}
							break;
						case DbType.String:
							if (fti.Values.Count > 0)
							{
								qMaker.Filters.Add(new SimpleFilterCondition(objC.Fields[fti.FieldName], GetApString(fti.Values[0]), SimpleFilterType.Like));
							}
							break;
					}
				}
			}
			#endregion

			string sql = qMaker.Create();
			XmlDocument doc = DBReport.GetXMLReport(sql);
			QMetaLoader.AddHeaderInformation(doc, qMaker);
			//QMetaLoader.AddSortInformation(doc, repTemp);
			//QMetaLoader.AddFilterInformation(doc, repTemp);
			AddTemplateInfo(doc, repTemp);
			ProcessResourceValues(doc);
			return doc;
		}
		#endregion

		#region GetDocumentXMLReport
		public static XmlDocument GetDocumentXMLReport(IBNReportTemplate repTemp, string _lang, string _timeoffset)
		{
			QDocument objD = new QDocument();
			QMaker qMaker = new QMaker(objD);
			qMaker.Language = _lang;
			qMaker.TimeOffset = _timeoffset;

			#region Fields
			foreach (FieldInfo fi in repTemp.Fields)
			{
				qMaker.Fields.Add(objD.Fields[fi.Name]);
			}
			#endregion

			#region GroupFields
			foreach (FieldInfo fi1 in repTemp.Groups)
			{
				qMaker.Groups.Add(objD.Fields[fi1.Name]);
			}
			#endregion

			#region Filters
			if (!Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) &&
				!Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager))
			{
				qMaker.PrevSqlQuery.Add(String.Format("DECLARE @DocumentIdTable TABLE(Id INT) " +
					"INSERT INTO @DocumentIdTable " +
					"SELECT DISTINCT TD.DocumentId As Id FROM DOCUMENTS AS TD " +
					"WHERE TD.DocumentId IN (SELECT DocumentId FROM DOCUMENT_SECURITY_ALL WHERE PrincipalId = {0} AND (IsManager = 1 OR IsResource = 1)) " +
					" OR (TD.ProjectId IN (SELECT ProjectId FROM PROJECT_SECURITY_ALL WHERE PrincipalId = {0} AND (IsTeamMember = 1 OR IsSponsor = 1 OR IsStakeHolder = 1 OR IsManager = 1 OR IsExecutiveManager = 1)))", 
					Security.CurrentUser.UserID));
				qMaker.Filters.Add(new SimpleFilterCondition(qMaker.OwnerObject.Fields["DocId"], "(SELECT Id FROM @DocumentIdTable)", SimpleFilterType.In));
			}

			foreach (FilterInfo fti in repTemp.Filters)
			{
				QField qField = objD.Fields[fti.FieldName];
				QDictionary qDic = objD.GetDictionary(qField);
				if (qDic != null)
				{
					if (fti.Values.Count > 0)
					{
						string[] _str = new string[fti.Values.Count];
						int i = 0;
						foreach (string s in fti.Values)
							_str[i++] = s;
						qMaker.Filters.Add(new SimpleFilterCondition(objD.Fields[qDic.FieldId.Name], _str, SimpleFilterType.Equal));
					}
				}
				else
				{
					switch (qField.DataType)
					{
						case DbType.Decimal:
						case DbType.Int32:
						case DbType.Time:
							if (fti.Values.Count > 0)
							{
								switch (fti.Values[0])
								{
									case "0":
										qMaker.Filters.Add(new SimpleFilterCondition(objD.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Equal));
										break;
									case "1":
										qMaker.Filters.Add(new SimpleFilterCondition(objD.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Great));
										break;
									case "2":
										qMaker.Filters.Add(new SimpleFilterCondition(objD.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Less));
										break;
									case "3":
										qMaker.Filters.Add(new IntervalFilterCondition(objD.Fields[fti.FieldName], fti.Values[1].ToString(), fti.Values[2].ToString()));
										break;
								}
							}
							break;
						case DbType.DateTime:
						case DbType.Date:
							if (fti.Values.Count > 0)
							{
								DateTime _Start = DateTime.MinValue;
								DateTime _Finish = DateTime.MaxValue;
								string sVal = fti.Values[0];
								SetDates(sVal, out _Start, out _Finish);
								if (sVal != "9")
								{
									qMaker.Filters.Add(new IntervalFilterCondition(objD.Fields[fti.FieldName], "CONVERT(datetime,'" + _Start.ToString("yyyy-MM-dd") + "',120)", "CONVERT(datetime,'" + _Finish.ToString("yyyy-MM-dd") + "',120)"));
								}
								else if (DateTime.Parse(fti.Values[1]) == DateTime.MinValue)
								{
									qMaker.Filters.Add(new SimpleFilterCondition(objD.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[2]).ToString("yyyy-MM-dd") + "',120)", SimpleFilterType.LessOrEqual));
								}
								else if (DateTime.Parse(fti.Values[2]) >= DateTime.MaxValue.Date)
								{
									qMaker.Filters.Add(new SimpleFilterCondition(objD.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[1]).ToString("yyyy-MM-dd") + "',120)", SimpleFilterType.GreatOrEqual));
								}
								else
								{
									qMaker.Filters.Add(new IntervalFilterCondition(objD.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[1]).ToString("yyyy-MM-dd") + "',120)", "CONVERT(datetime,'" + DateTime.Parse(fti.Values[2]).ToString("yyyy-MM-dd") + "',120)"));
								}
							}
							break;
						case DbType.String:
							if (fti.Values.Count > 0)
							{
								qMaker.Filters.Add(new SimpleFilterCondition(objD.Fields[fti.FieldName], GetApString(fti.Values[0]), SimpleFilterType.Like));
							}
							break;
					}
				}
			}
			#endregion

			string sql = qMaker.Create();
			XmlDocument doc = DBReport.GetXMLReport(sql);
			QMetaLoader.AddHeaderInformation(doc, qMaker);
			//QMetaLoader.AddSortInformation(doc, repTemp);
			//QMetaLoader.AddFilterInformation(doc, repTemp);
			AddTemplateInfo(doc, repTemp);
			ProcessResourceValues(doc);
			return doc;
		}
		#endregion

		#region GetUsersXMLReport
		public static XmlDocument GetUsersXMLReport(IBNReportTemplate repTemp, string _lang, string _timeoffset)
		{
			QDirectory objD = new QDirectory();
			QMaker qMaker = new QMaker(objD);
			qMaker.Language = _lang;
			qMaker.TimeOffset = _timeoffset;

			#region Fields
			foreach (FieldInfo fi in repTemp.Fields)
			{
				qMaker.Fields.Add(objD.Fields[fi.Name]);
			}
			#endregion

			#region GroupFields
			foreach (FieldInfo fi1 in repTemp.Groups)
			{
				qMaker.Groups.Add(objD.Fields[fi1.Name]);
			}
			#endregion

			#region Filters
			foreach (FilterInfo fti in repTemp.Filters)
			{
				QField qField = objD.Fields[fti.FieldName];
				QDictionary qDic = objD.GetDictionary(qField);
				if (qDic != null)
				{
					if (fti.Values.Count > 0)
					{
						string[] _str = new string[fti.Values.Count];
						int i = 0;
						foreach (string s in fti.Values)
							_str[i++] = s;
						qMaker.Filters.Add(new SimpleFilterCondition(objD.Fields[qDic.FieldId.Name], _str, SimpleFilterType.Equal));
					}
				}
				else
				{
					switch (qField.DataType)
					{
						case DbType.Decimal:
						case DbType.Int32:
						case DbType.Time:
							if (fti.Values.Count > 0)
							{
								switch (fti.Values[0])
								{
									case "0":
										qMaker.Filters.Add(new SimpleFilterCondition(objD.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Equal));
										break;
									case "1":
										qMaker.Filters.Add(new SimpleFilterCondition(objD.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Great));
										break;
									case "2":
										qMaker.Filters.Add(new SimpleFilterCondition(objD.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Less));
										break;
									case "3":
										qMaker.Filters.Add(new IntervalFilterCondition(objD.Fields[fti.FieldName], fti.Values[1].ToString(), fti.Values[2].ToString()));
										break;
								}
							}
							break;
						case DbType.DateTime:
						case DbType.Date:
							if (fti.Values.Count > 0)
							{
								DateTime _Start = DateTime.MinValue;
								DateTime _Finish = DateTime.MaxValue;
								string sVal = fti.Values[0];
								SetDates(sVal, out _Start, out _Finish);
								if (sVal != "9")
								{
									qMaker.Filters.Add(new IntervalFilterCondition(objD.Fields[fti.FieldName], "CONVERT(datetime,'" + _Start.ToString("yyyy-MM-dd") + "',120)", "CONVERT(datetime,'" + _Finish.ToString("yyyy-MM-dd") + "',120)"));
								}
								else if (DateTime.Parse(fti.Values[1]) == DateTime.MinValue)
								{
									qMaker.Filters.Add(new SimpleFilterCondition(objD.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[2]).ToString("yyyy-MM-dd") + "',120)", SimpleFilterType.LessOrEqual));
								}
								else if (DateTime.Parse(fti.Values[2]) >= DateTime.MaxValue.Date)
								{
									qMaker.Filters.Add(new SimpleFilterCondition(objD.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[1]).ToString("yyyy-MM-dd") + "',120)", SimpleFilterType.GreatOrEqual));
								}
								else
								{
									qMaker.Filters.Add(new IntervalFilterCondition(objD.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[1]).ToString("yyyy-MM-dd") + "',120)", "CONVERT(datetime,'" + DateTime.Parse(fti.Values[2]).ToString("yyyy-MM-dd") + "',120)"));
								}
							}
							break;
						case DbType.String:
							if (fti.Values.Count > 0)
							{
								qMaker.Filters.Add(new SimpleFilterCondition(objD.Fields[fti.FieldName], GetApString(fti.Values[0]), SimpleFilterType.Like));
							}
							break;
					}
				}
			}
			#endregion

			string sql = qMaker.Create();
			XmlDocument doc = DBReport.GetXMLReport(sql);
			QMetaLoader.AddHeaderInformation(doc, qMaker);
			//QMetaLoader.AddSortInformation(doc, repTemp);
			//QMetaLoader.AddFilterInformation(doc, repTemp);
			AddTemplateInfo(doc, repTemp);
			ProcessResourceValues(doc);
			return doc;
		}
		#endregion

		#region GetPortfolioXMLReport
		public static XmlDocument GetPortfolioXMLReport(IBNReportTemplate repTemp, string _lang, string _timeoffset)
		{
			QPortfolio objP = new QPortfolio();

			QMaker qMaker = new QMaker(objP);
			qMaker.Language = _lang;
			qMaker.TimeOffset = _timeoffset;

			#region Fields
			foreach (FieldInfo fi in repTemp.Fields)
			{
				qMaker.Fields.Add(objP.Fields[fi.Name]);
			}
			#endregion

			#region GroupFields
			foreach (FieldInfo fi1 in repTemp.Groups)
			{
				qMaker.Groups.Add(objP.Fields[fi1.Name]);
			}
			#endregion

			#region Filters
			foreach (FilterInfo fti in repTemp.Filters)
			{
				QField qField = objP.Fields[fti.FieldName];
				QDictionary qDic = objP.GetDictionary(qField);
				if (qDic != null)
				{
					if (fti.Values.Count > 0)
					{
						string[] _str = new string[fti.Values.Count];
						int i = 0;
						foreach (string s in fti.Values)
							_str[i++] = s;
						qMaker.Filters.Add(new SimpleFilterCondition(objP.Fields[qDic.FieldId.Name], _str, SimpleFilterType.Equal));
					}
				}
				else
				{
					switch (qField.DataType)
					{
						case DbType.Decimal:
						case DbType.Int32:
						case DbType.Time:
							if (fti.Values.Count > 0)
							{
								switch (fti.Values[0])
								{
									case "0":
										qMaker.Filters.Add(new SimpleFilterCondition(objP.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Equal));
										break;
									case "1":
										qMaker.Filters.Add(new SimpleFilterCondition(objP.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Great));
										break;
									case "2":
										qMaker.Filters.Add(new SimpleFilterCondition(objP.Fields[fti.FieldName], fti.Values[1].ToString(), SimpleFilterType.Less));
										break;
									case "3":
										qMaker.Filters.Add(new IntervalFilterCondition(objP.Fields[fti.FieldName], fti.Values[1].ToString(), fti.Values[2].ToString()));
										break;
								}
							}
							break;
						case DbType.DateTime:
						case DbType.Date:
							if (fti.Values.Count > 0)
							{
								DateTime _Start = DateTime.MinValue;
								DateTime _Finish = DateTime.MaxValue;
								string sVal = fti.Values[0];
								SetDates(sVal, out _Start, out _Finish);
								if (sVal != "9")
								{
									qMaker.Filters.Add(new IntervalFilterCondition(objP.Fields[fti.FieldName], "CONVERT(datetime,'" + _Start.ToString("yyyy-MM-dd") + "',120)", "CONVERT(datetime,'" + _Finish.ToString("yyyy-MM-dd") + "',120)"));
								}
								else if (DateTime.Parse(fti.Values[1]) == DateTime.MinValue)
								{
									qMaker.Filters.Add(new SimpleFilterCondition(objP.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[2]).ToString("yyyy-MM-dd") + "',120)", SimpleFilterType.LessOrEqual));
								}
								else if (DateTime.Parse(fti.Values[2]) >= DateTime.MaxValue.Date)
								{
									qMaker.Filters.Add(new SimpleFilterCondition(objP.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[1]).ToString("yyyy-MM-dd") + "',120)", SimpleFilterType.GreatOrEqual));
								}
								else
								{
									qMaker.Filters.Add(new IntervalFilterCondition(objP.Fields[fti.FieldName], "CONVERT(datetime,'" + DateTime.Parse(fti.Values[1]).ToString("yyyy-MM-dd") + "',120)", "CONVERT(datetime,'" + DateTime.Parse(fti.Values[2]).ToString("yyyy-MM-dd") + "',120)"));
								}
							}
							break;
						case DbType.String:
							if (fti.Values.Count > 0)
							{
								qMaker.Filters.Add(new SimpleFilterCondition(objP.Fields[fti.FieldName], GetApString(fti.Values[0]), SimpleFilterType.Like));
							}
							break;
					}
				}
			}
			#endregion

			string sql = qMaker.Create();
			XmlDocument doc = DBReport.GetXMLReport(sql);
			QMetaLoader.AddHeaderInformation(doc, qMaker);
			AddTemplateInfo(doc, repTemp);
			return doc;
		}
		#endregion

		#region GetApString
		private static string GetApString(string _string)
		{
			string retval = "N'%";
			while (_string.Length > 0)
			{
				int i = _string.IndexOf("'");
				if (i >= 0)
				{
					retval += _string.Substring(0, i) + "'+ CHAR(39) + N'";
					_string = _string.Remove(0, i + 1);
				}
				else
				{
					retval += _string + "%'";
					break;
				}
			}

			return retval;
		}
		#endregion

		#region SetDates
		private static void SetDates(string _value, out DateTime _start, out DateTime _finish)
		{
			switch (_value)
			{
				case "1":	//Today
					_start = DateTime.Today;
					_finish = DateTime.Today.AddDays(1);
					break;
				case "2":	//Yesterday
					_start = DateTime.Today.AddDays(-1);
					_finish = DateTime.Today;
					break;
				case "3":	//ThisWeek
					_start = DateTime.Today.AddDays(1 - (int)DateTime.Now.DayOfWeek);
					_finish = DateTime.Today.AddDays(1);
					break;
				case "4":	//LastWeek
					_start = DateTime.Today.AddDays(1 - (int)DateTime.Now.DayOfWeek - 7);
					_finish = DateTime.Today.AddDays(1 - (int)DateTime.Now.DayOfWeek);
					break;
				case "5":	//ThisMonth
					_start = DateTime.Today.AddDays(1 - DateTime.Now.Day);
					_finish = DateTime.Today.AddDays(1);
					break;
				case "6":	//LastMonth
					_start = DateTime.Today.AddDays(1 - DateTime.Now.Day).AddMonths(-1);
					_finish = DateTime.Today.AddDays(1 - DateTime.Now.Day);
					break;
				case "7":	//ThisYear
					_start = DateTime.Today.AddDays(1 - DateTime.Now.DayOfYear);
					_finish = DateTime.Today.AddDays(1);
					break;
				case "8":	//LastYear
					_start = DateTime.Today.AddDays(1 - DateTime.Now.DayOfYear).AddYears(-1);
					_finish = DateTime.Today.AddDays(1 - DateTime.Now.DayOfYear);
					break;
				default:
					_start = DateTime.MinValue;
					_finish = DateTime.MaxValue;
					break;
			}
		}
		#endregion

		#region Get*AlertsHistory
		public static DataView GetAlertsHistory(int UserId, DateTime StartDate, DateTime EndDate, bool DisplayLastEventsFirst)
		{
			return null;//Alert.GetAlertsHistory(UserId, StartDate, EndDate, DateTime.MinValue, "", DisplayLastEventsFirst);
		}
		public static DataView GetBroadcastAlertsHistory(int UserId)
		{
			return null;//Alert.GetAlertsHistory(UserId, DateTime.MinValue, DateTime.MinValue, DateTime.UtcNow, AlertEventType.General_BroadcastAlert.ToString(), true);
		}
		#endregion

		#region GetQuickUsageStatsReport
		/// <summary>
		/// ActiveProjects, CalendarEntries, FilesPublished, TasksCompleted, IssuesResolved
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetQuickUsageStatsReport()
		{
			return DBReport.GetQuickUsageStatsReport(Security.CurrentUser.UserID);
		}
		#endregion

		#region GetQuickUsageStatsReport2
		/// <summary>
		/// MessSent, MessReceived
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetQuickUsageStatsReport2()
		{
			int OriginalUserId = DBUser.GetOriginalId(Security.CurrentUser.UserID);
			return DBReport.GetQuickUsageStatsReport2(OriginalUserId);
		}
		#endregion

		#region GetToDoAndTaskTrackingReport
		/// <summary>
		/// Total, Completed, PastDue, Active, Upcoming
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetToDoAndTaskTrackingReport(int ProjectId)
		{
			return DBReport.GetToDoAndTaskTrackingReport(ProjectId);
		}
		#endregion

		#region GetProjectStatisticReport
		/// <summary>
		/// Discussions, Events, Tasks, ToDo, Incidents, Resources
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetProjectStatisticReport(int ProjectId)
		{
			return DBReport.GetProjectStatisticReport(ProjectId);
		}
		#endregion

		#region GetProjectTeamBreakdownReport
		/// <summary>
		/// ProjectId, UserId, FirstName, LastName, Code, Rate, Minutes
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetProjectTeamBreakdownReport(int ProjectId)
		{
			return DBReport.GetProjectTeamBreakdownReport(ProjectId);
		}
		#endregion

		#region GetProjectTeamBreakdownReportDataTable
		/// <summary>
		/// ProjectId, UserId, FirstName, LastName, Code, Rate, Minutes, TotalMinutes
		/// </summary>
		/// <returns></returns>
		public static DataTable GetProjectTeamBreakdownReportDataTable(int ProjectId)
		{
			return DBReport.GetProjectTeamBreakdownReportDataTable(ProjectId);
		}
		#endregion

		#region GetProjectStatisticByUserReport
		/// <summary>
		/// IncidentsCreated, IncidentsModified, IncidentsClosed, 
		/// Events, Discussions
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetProjectStatisticByUserReport(int ProjectId, int UserId)
		{
			return DBReport.GetProjectStatisticByUserReport(ProjectId, UserId);
		}
		#endregion

		#region GetHelpDeskIssuesSnapshotReport
		/// <summary>
		/// HDM, TotalIssues, ProjectIssues, GeneralIssues
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetHelpDeskIssuesSnapshotReport()
		{
			return DBReport.GetHelpDeskIssuesSnapshotReport();
		}
		#endregion

		#region GetUsageStats
		/// <summary>
		/// IMLogins, MessSent, MessReceived, IMGroups
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetUsageStats()
		{
			return DBReport.GetUsageStats();
		}
		#endregion

		#region BuildActivityTable
		/// <summary>
		/// Returns fields: DisplayName, Count.
		/// </summary>
		public static DataTable BuildActivityTable(IDataReader reader, int TopCount)
		{
			DataTable table;
			DataRow row;

			table = new DataTable();
			table.Columns.Add("DisplayName", typeof(string));
			table.Columns.Add("Count", typeof(int));

			int Counter = 0;

			try
			{
				while (reader.Read())
				{
					Counter++;
					row = table.NewRow();

					row["DisplayName"] = reader["DisplayName"];
					row["Count"] = reader["Count"];

					table.Rows.Add(row);

					if (Counter >= TopCount)
						break;
				}
			}
			finally
			{
				reader.Close();
			}
			return table;
		}
		#endregion

		#region GetMostActiveGroupsByPortalLoginsReport
		/// <summary>
		/// DisplayName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetMostActiveGroupsByPortalLoginsReport(DateTime FromDate, DateTime ToDate, int TopCount)
		{
			return BuildActivityTable(GetMostActiveGroupsByPortalLoginsReport(FromDate, ToDate), TopCount);
		}

		public static IDataReader GetMostActiveGroupsByPortalLoginsReport(DateTime FromDate, DateTime ToDate)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			FromDate = DBCommon.GetUTCDate(TimeZoneId, FromDate);
			ToDate = DBCommon.GetUTCDate(TimeZoneId, ToDate).AddDays(1);
			return DBReport.GetMostActiveGroupsByPortalLoginsReport(FromDate, ToDate);
		}
		#endregion

		#region GetMostActiveGroupsByProjectReport
		/// <summary>
		/// DisplayName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetMostActiveGroupsByProjectReport(DateTime FromDate, DateTime ToDate, int TopCount)
		{
			return BuildActivityTable(GetMostActiveGroupsByProjectReport(FromDate, ToDate), TopCount);
		}

		public static IDataReader GetMostActiveGroupsByProjectReport(DateTime FromDate, DateTime ToDate)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			FromDate = DBCommon.GetUTCDate(TimeZoneId, FromDate);
			ToDate = DBCommon.GetUTCDate(TimeZoneId, ToDate).AddDays(1);
			return DBReport.GetMostActiveGroupsByProjectReport(FromDate, ToDate);
		}
		#endregion

		#region GetMostActiveGroupsByEventReport
		/// <summary>
		/// DisplayName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetMostActiveGroupsByEventReport(DateTime FromDate, DateTime ToDate, int TopCount)
		{
			return BuildActivityTable(GetMostActiveGroupsByEventReport(FromDate, ToDate), TopCount);
		}

		public static IDataReader GetMostActiveGroupsByEventReport(DateTime FromDate, DateTime ToDate)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			FromDate = DBCommon.GetUTCDate(TimeZoneId, FromDate);
			ToDate = DBCommon.GetUTCDate(TimeZoneId, ToDate).AddDays(1);
			return DBReport.GetMostActiveGroupsByEventReport(FromDate, ToDate);
		}
		#endregion

		#region GetMostActiveGroupsByTaskReport
		/// <summary>
		/// DisplayName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetMostActiveGroupsByTaskReport(DateTime FromDate, DateTime ToDate, int TopCount)
		{
			return BuildActivityTable(GetMostActiveGroupsByTaskReport(FromDate, ToDate), TopCount);
		}

		public static IDataReader GetMostActiveGroupsByTaskReport(DateTime FromDate, DateTime ToDate)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			FromDate = DBCommon.GetUTCDate(TimeZoneId, FromDate);
			ToDate = DBCommon.GetUTCDate(TimeZoneId, ToDate).AddDays(1);
			return DBReport.GetMostActiveGroupsByTaskReport(FromDate, ToDate);
		}
		#endregion

		#region GetMostActiveGroupsByToDoReport
		/// <summary>
		/// DisplayName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetMostActiveGroupsByToDoReport(DateTime FromDate, DateTime ToDate, int TopCount)
		{
			return BuildActivityTable(GetMostActiveGroupsByToDoReport(FromDate, ToDate), TopCount);
		}

		public static IDataReader GetMostActiveGroupsByToDoReport(DateTime FromDate, DateTime ToDate)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			FromDate = DBCommon.GetUTCDate(TimeZoneId, FromDate);
			ToDate = DBCommon.GetUTCDate(TimeZoneId, ToDate).AddDays(1);
			return DBReport.GetMostActiveGroupsByToDoReport(FromDate, ToDate);
		}
		#endregion

		#region PortalLogin
		/// <summary>
		/// </summary>
		/// <returns></returns>
		public static void LogPortalLogin(int UserId, string IP)
		{
			DBReport.LogPortalLogin(UserId, IP);
		}
		#endregion

		#region GetListReportsCreators
		/// <summary>
		/// Reader contains fields:
		/// UserId, FirstName, LastName, FullName, FullName2
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListReportsCreators()
		{
			return DBReport.GetListReportsCreators();
		}
		#endregion

		#region GetListReportsModifiers
		/// <summary>
		/// Reader contains fields:
		/// UserId, FirstName, LastName, FullName, FullName2
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListReportsModifiers()
		{
			return DBReport.GetListReportsModifiers();
		}
		#endregion

		#region CreateReportTemplate
		public static int CreateReportTemplate(string Name, string RepTemplate, bool IsGlobal, bool IsTemporary)
		{
			try
			{
				DBReport.DeleteTemporaryTemplates();
			}
			catch
			{
			}
			return DBReport.CreateReportTemplate(Name, Security.CurrentUser.UserID,
				DateTime.UtcNow, RepTemplate, IsGlobal, IsTemporary);
		}
		#endregion

		#region UpdateReportTemplate
		public static void UpdateReportTemplate(int TemplateId, string Name, string RepTemplate, bool IsGlobal, bool IsTemporary)
		{
			DBReport.UpdateReportTemplate(TemplateId, Name, Security.CurrentUser.UserID,
				DateTime.UtcNow, RepTemplate, IsGlobal, IsTemporary);
		}
		#endregion

		#region CreateReportByTemplate
		public static int CreateReportByTemplate(int ReportId, string RepXML)
		{
			try
			{
				DBReport.DeleteTemporaryTemplates();
			}
			catch
			{
			}
			return DBReport.CreateReportByTemplate(ReportId, Security.CurrentUser.UserID,
				DateTime.UtcNow, System.Text.Encoding.UTF8.GetBytes(RepXML));
		}
		#endregion

		#region GetReport
		/// <summary>
		/// DataReader contains fields:
		///  TemplateId, ReportCreated, ReportCreator, Name, TemplateCreated, TemplateModified, TemplateCreatorId, TemplateModifierId, TemplateXML
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetReport(int ReportItemId)
		{
			return DBReport.GetReport(ReportItemId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetReportBinaryData
		public static IDataReader GetReportBinaryData(int ReportItemId)
		{
			return DBReport.GetReportBinaryData(ReportItemId);
		}
		#endregion

		#region GetReportTemplatesByFilter
		/// <summary>
		/// DataReader contains fields:
		///  TemplateId, TemplateName, TemplateCreated, TemplateModified, TemplateCreatorId, TemplateModifierId, TemplateXML
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetReportTemplatesByFilter(int CreatorId,
			DateTime StartDate, DateTime FinishDate, int ModifierId)
		{
			return DBReport.GetReportTemplatesByFilter(CreatorId, Security.CurrentUser.TimeZoneId,
				StartDate, FinishDate, ModifierId, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetReportTemplatesByFilterDataTable
		/// <summary>
		/// DataReader contains fields:
		///  TemplateId, TemplateName, TemplateCreated, TemplateModified, TemplateCreatorId, TemplateModifierId, TemplateXML
		/// </summary>
		/// <returns></returns>
		public static DataTable GetReportTemplatesByFilterDataTable(int CreatorId,
			DateTime StartDate, DateTime FinishDate, int ModifierId)
		{
			return DBReport.GetReportTemplatesByFilterDataTable(CreatorId,
				Security.CurrentUser.TimeZoneId, StartDate, FinishDate, ModifierId, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetReportTemplate
		/// <summary></summary>
		/// <returns>
		/// DataReader contains fields:
		///  TemplateId, Name, TemplateCreated, TemplateModified, TemplateCreatorId, TemplateModifierId, TemplateXML
		/// </returns>
		public static IDataReader GetReportTemplate(int ReportTemplateId)
		{
			return DBReport.GetReportTemplate(ReportTemplateId, Security.CurrentUser.TimeZoneId);
		}

		public static IDataReader GetReportTemplate()
		{
			return DBReport.GetReportTemplate(0, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetReportsByTemplateId
		/// <summary></summary>
		/// <returns>
		/// DataReader contains fields:
		///  ReportItemId, ReportId, CreationDate, CreatorId
		/// </returns>
		public static DataTable GetReportsByTemplateId(int TemplateId)
		{
			return DBReport.GetReportsByTemplateId(TemplateId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetQDictionary
		public static IDataReader GetQDictionary(string query)
		{
			return DBReport.GetQDictionary(query);
		}
		#endregion

		#region DeleteReportTemplate
		/// <summary>
		/// </summary>
		/// <returns></returns>
		public static void DeleteReportTemplate(int TemplateId)
		{
			DBReport.DeleteReportTemplate(TemplateId);
		}
		#endregion

		#region DeleteReportItem
		/// <summary>
		/// </summary>
		/// <returns></returns>
		public static void DeleteReportItem(int ReportItemId)
		{
			DBReport.DeleteReportItem(ReportItemId);
		}
		#endregion

		#region GetOnlineUsersCount
		public static int GetOnlineUsersCount()
		{
			return DBReport.GetOnlineUsersCount();
		}
		#endregion

		#region GetUserLoginsClient
		/// <summary>
		/// </summary>
		public static int GetUserLoginsClient(int UserID)
		{
			return DBReport.GetUserLoginsClient(UserID);
		}
		#endregion

		#region GetStatusLog
		/// <summary>
		/// Gets the Status Log
		/// </summary>
		/// <param name="imGroupId"></param>
		/// <param name="userId"></param>
		/// <param name="fromDate"></param>
		/// <param name="toDate"></param>
		/// <returns>user_id, dt, status, first_name, last_name</returns>
		public static IDataReader GetStatusLog(int imGroupId, int userId, DateTime fromDate, DateTime toDate)
		{
			return DBReport.GetStatusLog(imGroupId, userId, fromDate, toDate, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region internal DefaultStartDate
		public static DateTime DefaultStartDate
		{
			get
			{
				return DateTime.Now.AddYears(-TimeInterval);
			}
		}
		#endregion
		#region internal DefaultEndDate
		public static DateTime DefaultEndDate
		{
			get
			{
				return DateTime.Now.AddYears(TimeInterval);
			}
		}
		#endregion

		#region ProcessResourceValues
		private static void ProcessResourceValues(XmlDocument doc)
		{
			XmlNodeList nodes;

			// Values
			nodes = doc.SelectNodes("//Field/Values/Value");
			foreach (XmlNode node in nodes)
			{
				if (node.HasChildNodes && node.FirstChild.NodeType == XmlNodeType.Text)
				{
					node.FirstChild.Value = Common.GetWebResourceString(node.FirstChild.Value);
				}
			}

			// Groups
			nodes = doc.SelectNodes("//Groups/Group/@Name");
			foreach (XmlNode node in nodes)
			{
				node.Value = Common.GetWebResourceString(node.FirstChild.Value);
			}
		}
		#endregion
	}
}
