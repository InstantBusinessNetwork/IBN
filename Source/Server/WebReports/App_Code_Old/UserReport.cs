using System;
using System.Collections;
using System.Data;
//using Mediachase.IBN.Business;

using DBCommon = Mediachase.IBN.Database.DBCommon;
using Security = Mediachase.IBN.Business.Security;

//using Mediachase.IBN.Database;

namespace Mediachase.IBN.DefaultUserReports
{
	#region Enum ActivityReportType
	public enum ActivityReportType
	{
		Messages,
		FilesExchanged,
		IMLogins,
		PortalLogins,
		CalendarEntries,
		ActiveProjects,
		FilesPublished,
		FileVersionsPublished,
		NewIssues,
		NewTasks,
		NewToDos
	}
	#endregion

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

	/// <summary>
	/// Summary description for UserReport.
	/// </summary>
	public class UserReport
	{
		private const int TimeInterval = 25;

		#region GetCountIMMessages
		public static int GetCountIMMessages(bool getInternal)
		{
			int DefaultBias = UserReport.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBUserReport.GetCountIMMessages(DefaultStartDate, DefaultEndDate, getInternal, DefaultBias);
		}
		#endregion

		#region GetQuickSnapshotReport
		public static IDataReader GetQuickSnapshotReport(DateTime StartDate, DateTime EndDate, int CreatorId)
		{
			StartDate = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, StartDate);
			EndDate = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, EndDate);

			return DBUserReport.GetQuickSnapshotReport(StartDate, EndDate, CreatorId);
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
					string sDisplayName = "";
					using (IDataReader _group = UserReport.GetIMGroup(user.IMGroupId))
					{
						if (_group.Read())
							sDisplayName = _group["IMGroupName"].ToString();
					}
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
					using (IDataReader _obj = UserReport.GetUserInfoByOriginalId(user_id))
					{
						if (_obj.Read())
							user_id = (int)_obj["UserId"];
						else
							continue;
					}

					int DefaultBias = UserReport.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
					switch (type)
					{
						case ActivityReportType.Messages:
							using (IDataReader _obj = DBUserReport.GetUserStats(user_id, fromDate, toDate, DefaultBias))
							{
								if (_obj.Read())
									user.Count = (int)_obj["MsgSent"];
							}
							break;
						case ActivityReportType.FilesExchanged:
							using (IDataReader _obj = DBUserReport.GetUserStats(user_id, fromDate, toDate, DefaultBias))
							{
								if (_obj.Read())
									user.Count = (int)_obj["FilesSent"];
							}
							break;
						case ActivityReportType.IMLogins:
							using (IDataReader _obj = DBUserReport.GetUserStats(user_id, fromDate, toDate, DefaultBias))
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
						case ActivityReportType.FilesPublished:
							using (IDataReader _obj = GetQuickSnapshotReport(fromDate, toDate, user_id))
							{
								if (_obj.Read())
									user.Count = (int)_obj["NewAssetsCount"];
							}
							break;
						case ActivityReportType.FileVersionsPublished:
							using (IDataReader _obj = GetQuickSnapshotReport(fromDate, toDate, user_id))
							{
								if (_obj.Read())
									user.Count = (int)_obj["NewAssetVersionsCount"];
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

		#region GetUsersByCompany
		public static IDataReader GetUsersByCompany()
		{
			return DBUserReport.GetUsersByCompany();
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
			return DBUserReport.GetMostActiveGroupsByPortalLoginsReport(FromDate, ToDate);
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
			return DBUserReport.GetMostActiveGroupsByProjectReport(FromDate, ToDate);
		}
		#endregion

		#region GetMostActiveGroupsByIncidentReport
		/// <summary>
		/// DisplayName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetMostActiveGroupsByIncidentReport(DateTime FromDate, DateTime ToDate, int TopCount)
		{
			return BuildActivityTable(GetMostActiveGroupsByIncidentReport(FromDate, ToDate), TopCount);
		}

		public static IDataReader GetMostActiveGroupsByIncidentReport(DateTime FromDate, DateTime ToDate)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			FromDate = DBCommon.GetUTCDate(TimeZoneId, FromDate);
			ToDate = DBCommon.GetUTCDate(TimeZoneId, ToDate).AddDays(1);
			return DBUserReport.GetMostActiveGroupsByIncidentReport(FromDate, ToDate);
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
			return DBUserReport.GetMostActiveGroupsByEventReport(FromDate, ToDate);
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
			return DBUserReport.GetMostActiveGroupsByTaskReport(FromDate, ToDate);
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
			return DBUserReport.GetMostActiveGroupsByToDoReport(FromDate, ToDate);
		}
		#endregion

		#region GetMostActiveGroupsByAssetReport
		/// <summary>
		/// DisplayName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetMostActiveGroupsByAssetReport(DateTime FromDate, DateTime ToDate, int TopCount)
		{
			return BuildActivityTable(GetMostActiveGroupsByAssetReport(FromDate, ToDate), TopCount);
		}

		public static IDataReader GetMostActiveGroupsByAssetReport(DateTime FromDate, DateTime ToDate)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			FromDate = DBCommon.GetUTCDate(TimeZoneId, FromDate);
			ToDate = DBCommon.GetUTCDate(TimeZoneId, ToDate).AddDays(1);
			return DBUserReport.GetMostActiveGroupsByAssetReport(FromDate, ToDate);
		}
		#endregion

		#region GetMostActiveGroupsByAssetVersionReport
		/// <summary>
		/// DisplayName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetMostActiveGroupsByAssetVersionReport(DateTime FromDate, DateTime ToDate, int TopCount)
		{
			return BuildActivityTable(GetMostActiveGroupsByAssetVersionReport(FromDate, ToDate), TopCount);
		}

		public static IDataReader GetMostActiveGroupsByAssetVersionReport(DateTime FromDate, DateTime ToDate)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			FromDate = DBCommon.GetUTCDate(TimeZoneId, FromDate);
			ToDate = DBCommon.GetUTCDate(TimeZoneId, ToDate).AddDays(1);
			return DBUserReport.GetMostActiveGroupsByAssetVersionReport(FromDate, ToDate);
		}
		#endregion

		#region GetCountChatMessages
		public static int GetCountChatMessages(DateTime StartDate, DateTime EndDate)
		{
			int DefaultBias = UserReport.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBUserReport.GetCountChatMessages(StartDate, EndDate, DefaultBias);
		}
		#endregion

		#region GetCountChatMessages
		public static int GetCountChatMessages()
		{
			int DefaultBias = UserReport.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBUserReport.GetCountChatMessages(DefaultStartDate, DefaultEndDate, DefaultBias);
		}
		#endregion

		#region GetFilesTransferred
		public static int GetFilesTransferred(DateTime StartDate, DateTime EndDate)
		{
			int DefaultBias = UserReport.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBUserReport.GetFilesTransferred(StartDate, EndDate, DefaultBias);
		}
		#endregion

		#region GetFilesTransferred
		public static int GetFilesTransferred()
		{
			int DefaultBias = UserReport.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBUserReport.GetFilesTransferred(DefaultStartDate, DefaultEndDate, DefaultBias);
		}
		#endregion

		#region GetAuthenticatedUsers
		public static int GetAuthenticatedUsers(DateTime StartDate, DateTime EndDate)
		{
			int DefaultBias = UserReport.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBUserReport.GetAuthenticatedUsers(StartDate, EndDate, DefaultBias);
		}
		#endregion

		#region GetCountIMMessages
		public static int GetCountIMMessages(DateTime StartDate, DateTime EndDate, bool getInternal)
		{
			int DefaultBias = UserReport.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBUserReport.GetCountIMMessages(StartDate, EndDate, getInternal, DefaultBias);
		}
		#endregion

		#region GetTop10Users
		public static DataView GetTop10Users(DateTime StartDate, DateTime EndDate)
		{
			int DefaultBias = UserReport.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			DataTable dt = new DataTable("Top10writers");
			dt.Columns.Add("Place");
			dt.Columns.Add("Group");
			dt.Columns.Add("Name");
			dt.Columns.Add("TotalMessages");

			using (IDataReader reader = DBUserReport.GetTop10Users(StartDate, EndDate, DefaultBias))
			{
				int place = 1;
				while (reader.Read())
				{
					int iUserId = reader.GetInt32(0);
					string sName = "";
					int iIMGroupId = 0;
					using (IDataReader _obj = UserReport.GetUserInfoByOriginalId(iUserId))
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
						using (IDataReader _obj = UserReport.GetIMGroup(iIMGroupId))
						{
							if (_obj.Read())
								dr["Group"] = _obj["IMGroupName"].ToString();
						}
					}
					dt.Rows.Add(dr);
					place++;
				}
			}

			return dt.DefaultView;
		}
		#endregion

		#region GetListUsers
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IsActive, IMGroupId
		/// </summary>
		public static IDataReader GetListUsers(int group_id)
		{
			return DBUserReport.GetListUsers(group_id);
		}
		#endregion

		#region GetListIMGroup
		/// <summary>
		/// Reader returns fields:
		///		IMGroupId, IMGroupName, color, logo_version, is_partner
		/// </summary>
		public static IDataReader GetListIMGroup()
		{
			return DBUserReport.GetIMGroup(0, false);
		}
		#endregion

		#region GetListIMSessionsByUserAndDate
		/// <summary>
		/// Reader contains columns:
		///		User_Id, SessionBegin, SessionEnd
		/// </summary>
		public static IDataReader GetListIMSessionsByUserAndDate(int UserId, DateTime StartDate, DateTime EndDate)
		{
			int DefaultBias = UserReport.GetCurrentBias();
			return DBUserReport.GetListIMSessionsByUserAndDate(UserId, StartDate, EndDate, DefaultBias);
		}
		#endregion

		#region GetSecureGroup
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, IMGroupId
		/// </summary>
		public static IDataReader GetGroup(int group_id)
		{
			return DBUserReport.GetGroup(group_id);
		}
		#endregion

		#region GetParentGroup
		public static int GetParentGroup(int group_id)
		{
			return DBUserReport.GetParentGroup(group_id);
		}
		#endregion

		#region GetListChildGroups
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, HasChildren
		/// </summary>
		public static IDataReader GetListChildGroups(int group_id)
		{
			return DBUserReport.GetListChildGroups(group_id);
		}
		#endregion

		#region GetListSecureGroup
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName
		/// </summary>
		public static IDataReader GetListSecureGroup(int user_id)
		{
			return DBUserReport.GetListSecureGroupExplicit(user_id);
		}
		#endregion

		#region GetListActiveUsersInGroup
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IMGroupId, CreatedBy
		/// </summary>
		public static IDataReader GetListActiveUsersInGroup(int group_id)
		{
			return GetListActiveUsersInGroup(group_id, true);
		}

		public static IDataReader GetListActiveUsersInGroup(int group_id, bool check_security)
		{
			return DBUserReport.GetListActiveUsersInGroup(group_id);
		}
		#endregion

		#region GetUserInfo
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IsActive, IMGroupId, CreatedBy, Comments, IsExternal, IsPending, OriginalId
		/// </summary>
		public static IDataReader GetUserInfo(int user_id)
		{
			return GetUserInfo(user_id, true);
		}
		#endregion

		#region GetUserInfo
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IsActive, IMGroupId, CreatedBy, Comments, IsExternal, IsPending, OriginalId
		/// </summary>
		public static IDataReader GetUserInfo(int user_id, bool check_rights)
		{
			return DBUserReport.GetUserInfo(user_id);
		}
		#endregion

		#region GetSecureGroupStats
		/// <summary>
		/// ActiveAccounts, InactiveAccounts, NewProjectsCount, NewEventsCount, 
		/// NewIncidentsCount, NewToDosCount, NewTasksCount, PortalLogins, ReOpenIncidentsCount
		/// </summary>
		private static IDataReader GetSecGroupStats(int GroupID, DateTime fromDate, DateTime toDate, bool ShowAll)
		{
			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			fromDate = DBCommon.GetUTCDate(TimeZoneId, fromDate);
			toDate = DBCommon.GetUTCDate(TimeZoneId, toDate);

			return DBUserReport.GetSecGroupStats(GroupID, fromDate, toDate, ShowAll);
		}
		#endregion

		#region GetSecureGroupStats
		/// <summary>
		/// ActiveAccounts, InactiveAccounts, NewProjectsCount, NewEventsCount, NewAssetsCount
		/// NewAssetVersionsCount, NewIncidentsCount, NewToDosCount, NewTasksCount, ReOpenIncidentsCount
		/// </summary>
		public static IDataReader GetSecGroupStats(int GroupID, DateTime fromDate, DateTime toDate)
		{
			return GetSecGroupStats(GroupID, fromDate, toDate, true);
		}
		#endregion

		#region GetUserStats
		/// <summary>
		/// LoginsClient, MsgSent, MsgReceived, ConfCreated,
		/// ConfMsgSent, FilesSent, FSBytes, FilesReceived, FRBytes
		/// </summary>
		public static DataTable GetUserStats(int UserID, DateTime fromDate, DateTime toDate)
		{
			DataTable table = new DataTable();
			DataRow row;

			table.Columns.Add("LoginsClient", typeof(int));
			table.Columns.Add("MsgSent", typeof(int));
			table.Columns.Add("MsgReceived", typeof(int));
			table.Columns.Add("ConfCreated", typeof(int));
			table.Columns.Add("ConfMsgSent", typeof(int));
			table.Columns.Add("FilesSent", typeof(int));
			table.Columns.Add("FSBytes", typeof(long));
			table.Columns.Add("FilesReceived", typeof(int));
			table.Columns.Add("FRBytes", typeof(long));

			row = table.NewRow();

			int DefaultBias = UserReport.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			using (IDataReader reader = DBUserReport.GetUserStats(UserID, fromDate, toDate, DefaultBias))
			{
				if (reader != null)
				{
					if (reader.Read())
					{
						if (reader["LoginsClient"] != DBNull.Value)
							row["LoginsClient"] = (int)reader["LoginsClient"];
						else
							row["LoginsClient"] = 0;
						if (reader["MsgSent"] != DBNull.Value)
							row["MsgSent"] = (int)reader["MsgSent"];
						else
							row["MsgSent"] = 0;
						if (reader["MsgReceived"] != DBNull.Value)
							row["MsgReceived"] = (int)reader["MsgReceived"];
						else
							row["MsgReceived"] = 0;
						if (reader["ConfCreated"] != DBNull.Value)
							row["ConfCreated"] = (int)reader["ConfCreated"];
						else
							row["ConfCreated"] = 0;
						if (reader["ConfMsgSent"] != DBNull.Value)
							row["ConfMsgSent"] = (int)reader["ConfMsgSent"];
						else
							row["ConfMsgSent"] = 0;
						if (reader["FilesSent"] != DBNull.Value)
							row["FilesSent"] = (int)reader["FilesSent"];
						else
							row["FilesSent"] = 0;
						if (reader["FSBytes"] != DBNull.Value)
							row["FSBytes"] = (long)reader["FSBytes"];
						else
							row["FSBytes"] = 0;
						if (reader["FilesReceived"] != DBNull.Value)
							row["FilesReceived"] = (int)reader["FilesReceived"];
						else
							row["FilesReceived"] = 0;
						if (reader["FRBytes"] != DBNull.Value)
							row["FRBytes"] = (long)reader["FRBytes"];
						else
							row["FRBytes"] = 0;
					}
				}
			}

			table.Rows.Add(row);
			return table;
		}
		#endregion

		#region GetLocalDate
		public static DateTime GetLocalDate(DateTime UTCDate)
		{
			return GetLocalDate(Security.CurrentUser.TimeZoneId, UTCDate);
		}

		public static DateTime GetLocalDate(int TimeZoneId, DateTime UTCDate)
		{
			return DBCommon.GetLocalDate(TimeZoneId, UTCDate);
		}
		#endregion

		#region GetIMGroup
		/// <summary>
		/// Reader returns fields:
		///		IMGroupId, IMGroupName, color, logo_version, is_partner
		/// </summary>
		public static IDataReader GetIMGroup(int group_id)
		{
			return DBUserReport.GetIMGroup(group_id, true);
		}
		#endregion

		#region GetChats
		public static IDataReader GetChats(int iUserId)
		{
			return DBUserReport.GetChats(iUserId);
		}
		#endregion

		#region SearchChat
		public static IDataReader SearchChat(int iUserId, int iChatId, DateTime dtFrom, DateTime dtTo,
			string sKeyWord, int iOrder)
		{
			int DefaultBias = UserReport.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			return DBUserReport.SearchChat(iUserId, iChatId, dtFrom, dtTo, sKeyWord, iOrder, DefaultBias);
		}
		#endregion

		#region GetTimeZoneBias
		public static int GetTimeZoneBias(int TimeZoneId)
		{
			return DBCommon.GetTimeZoneBias(TimeZoneId);
		}
		#endregion

		#region GetListAllUsersInGroup
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IMGroupId, CreatedBy, Activity, IsExternal
		/// </summary>
		public static IDataReader GetListAllUsersInGroup(int group_id)
		{
			return DBUserReport.GetListAllUsersInGroup(group_id);
		}
		#endregion

		#region GetListGroups
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName
		/// </summary>
		public static IDataReader GetListGroups(bool IsPartner)
		{
			if (!IsPartner/*Security.IsUserInGroup(InternalSecureGroups.Partner)*/)
			{
				return DBUserReport.GetListGroups();
			}
			else
			{
				int PartnerGroupId = DBUserReport.GetGroupForPartnerUser(Security.CurrentUser.UserID);

				return DBUserReport.GetListGroupsByPartner(PartnerGroupId, true, true);
			}
		}
		#endregion

		#region GetListGroupsAsTree
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName, Level
		/// </summary>
		public static IDataReader GetListGroupsAsTree(bool IsPartner)
		{
			if (!IsPartner)
				return DBUserReport.GetListGroupsAsTree();
			else
			{
				int PartnerGroupId = DBUserReport.GetGroupForPartnerUser(Security.CurrentUser.UserID);
				return DBUserReport.GetListGroupsByPartner(PartnerGroupId, true, true);
			}
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
			using (IDataReader rdr = UserReport.GetListAllUsersInGroup(userGroupId))
			{
				while (rdr.Read())
				{
					if (rdr["OriginalId"] != DBNull.Value)
						UserList.Add((int)rdr["OriginalId"]);
				}
			}

			using (IDataReader rdr = UserReport.GetListAllUsersInGroup(contUserGroupId))
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

			int DefaultBias = UserReport.GetTimeZoneBias(Security.CurrentUser.TimeZoneId);
			using (IDataReader reader = DBUserReport.SearchHistory(fromDate, toDate,
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

		#region GetGroupForPartnerUser
		public static int GetGroupForPartnerUser(int UserId)
		{
			return DBUserReport.GetGroupForPartnerUser(UserId);
		}
		#endregion

		#region GetListAvailableGroups
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName
		/// </summary>
		public static IDataReader GetListAvailableGroups(bool IsPartner)
		{
			if (!IsPartner/*Security.IsUserInGroup(InternalSecureGroups.Partner)*/)
				return DBUserReport.GetListAvailableGroups();
			else
				return DBUserReport.GetListAvailableGroupsForPartner(Security.CurrentUser.UserID);
		}
		#endregion

		#region Get*AlertsHistory

		public static DataView GetAlertsHistory(int userId, DateTime startDate, DateTime endDate, bool displayLastEventsFirst)
		{
			if (startDate == DateTime.MinValue)
				startDate = DefaultStartDate;
			if (endDate == DateTime.MinValue)
				endDate = DefaultEndDate;

			DataTable dt = null;
			using (IDataReader reader = DBUserReport.Message_GetByUserIdAndTimeFilter(userId, startDate, endDate, Security.CurrentUser.TimeZoneId))
			{
				dt = new DataTable();
				dt.Columns.Add(new DataColumn("send_time", System.Type.GetType("System.DateTime")));
				dt.Columns.Add(new DataColumn("mess_text", System.Type.GetType("System.String")));
				while (reader.Read())
				{
					DataRow dr = dt.NewRow();
					dr["send_time"] = (DateTime)reader["Sent"];
					dr["mess_text"] = reader["Body"].ToString();
					dt.Rows.Add(dr);
				}
			}
			DataView dv = dt.DefaultView;
			if (displayLastEventsFirst)
				dv.Sort = "send_time DESC";
			return dv;//Alert.GetAlertsHistory(UserId, StartDate, EndDate, DateTime.MinValue, "", DisplayLastEventsFirst);
		}
		public static DataView GetBroadcastAlertsHistory(int UserId)
		{
			return null;//Alert.GetAlertsHistory(UserId, DateTime.MinValue, DateTime.MinValue, DateTime.UtcNow, AlertEventType.General_BroadcastAlert.ToString(), true);
		}
		#endregion

		#region GetUserInfoByOriginalId
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, Activity, IMGroupId
		/// </summary>
		public static IDataReader GetUserInfoByOriginalId(int OriginalId)
		{
			return DBUserReport.GetUserInfoByOriginalId(OriginalId);
		}
		#endregion

		#region GetCurrentBias
		public static int GetCurrentBias()
		{
			return GetCurrentBias(Security.CurrentUser.TimeZoneId);
		}
		public static int GetCurrentBias(int TimeZoneId)
		{
			DateTime UTCDate = DateTime.UtcNow;
			DateTime LocalDate = DBCommon.GetLocalDate(TimeZoneId, UTCDate);
			int retval = UTCDate.Hour * 60 + UTCDate.Minute - LocalDate.Hour * 60 - LocalDate.Minute;
			if (UTCDate.Date > LocalDate.Date)
				retval = retval + 24 * 60;
			else if (UTCDate.Date < LocalDate.Date)
				retval = retval - 24 * 60;
			return retval;
		}
		#endregion

		#region GetMenuInAlerts
		public static bool GetMenuInAlerts(int UserId)
		{
			return DBUserReport.GetMenuInAlerts(UserId);
		}
		#endregion

		#region internal GetDates(string range, out DateTime start, out DateTime finish, string fromCustom, string toCustom)
		internal static void GetDates(string range, out DateTime start, out DateTime finish, string fromCustom, string toCustom)
		{
			switch (range)
			{
				case "1": //Today
					start = UserDateTime.Today;
					finish = UserDateTime.Now;
					break;
				case "2": //Yesterday
					finish = UserDateTime.Today;
					start = finish.AddDays(-1);
					break;
				case "3": //ThisWeek
					start = UserDateTime.Today.AddDays(1 - (int)DateTime.Now.DayOfWeek);
					finish = UserDateTime.Now;
					break;
				case "4": //LastWeek
					finish = UserDateTime.Today.AddDays(1 - (int)DateTime.Now.DayOfWeek);
					start = finish.AddDays(-7);
					break;
				case "5": //ThisMonth
					start = UserDateTime.Today.AddDays(1 - DateTime.Now.Day);
					finish = UserDateTime.Now;
					break;
				case "6": //LastMonth
					finish = UserDateTime.Today.AddDays(1 - DateTime.Now.Day);
					start = finish.AddMonths(-1);
					break;
				case "7": //ThisYear
					start = UserDateTime.Today.AddDays(1 - DateTime.Now.DayOfYear);
					finish = UserDateTime.Now;
					break;
				case "8": //LastYear
					finish = UserDateTime.Today.AddDays(1 - DateTime.Now.DayOfYear);
					start = finish.AddYears(-1);
					break;
				case "9": //Custom
					if (!DateTime.TryParse(fromCustom, out start))
						start = DefaultStartDate;
					if (!DateTime.TryParse(toCustom, out finish))
						finish = DefaultEndDate;
					break;
				default:
					start = DefaultStartDate;
					finish = DefaultEndDate;
					break;
			}
		}
		#endregion
		#region internal DefaultStartDate
		internal static DateTime DefaultStartDate
		{
			get
			{
				return DateTime.Now.AddYears(-TimeInterval);
			}
		}
		#endregion
		#region internal DefaultEndDate
		internal static DateTime DefaultEndDate
		{
			get
			{
				return DateTime.Now.AddYears(TimeInterval);
			}
		}
		#endregion
	}
}
