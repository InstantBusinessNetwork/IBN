using System.Web;
using System.Web.UI;
using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Resources;
using System.Text.RegularExpressions;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.IBN.DefaultUserReports;
using System.Globalization;

//using Mediachase.IBN.Business;	// !!


namespace Mediachase.IBN.DefaultUserReports
{
	#region Class UserDateTime
	public class UserDateTime
	{
		private const string sUserNow = "usernow";

		public static DateTime Now
		{
			get
			{
				return UserNow();
			}

		}

		public static DateTime Today
		{
			get
			{
				return UserToday();
			}
		}

		private static DateTime UserNow()
		{
			DateTime usernow = DateTime.UtcNow;
			HttpContext context = HttpContext.Current;

			if (context.Items.Contains(sUserNow))
			{
				usernow = (DateTime)context.Items[sUserNow];
			}
			else
			{
				try
				{
					usernow = UserReport.GetLocalDate(usernow);
					context.Items.Add(sUserNow, usernow);
				}
				catch
				{
				}
			}
			return usernow;
		}

		private static DateTime UserToday()
		{
			DateTime today = UserNow();
			return new DateTime(today.Year, today.Month, today.Day);
		}
	}
	#endregion
}

namespace Mediachase.UI.Web.UserReports.Util
{
	/// <summary>
	/// Summary description for CommonHelper.
	/// </summary>
	public class CommonHelper
	{
		public const string ChartPath = "~/Layouts/images/Charts/";
		private const string sUserStatus = "userstatus";

		#region GetWeekStartByDate
		public static DateTime GetWeekStartByDate(DateTime start)
		{
			start = start.Date;
			int dow = (int)start.DayOfWeek;
			int fdow = Mediachase.IBN.Business.PortalConfig.PortalFirstDayOfWeek;
			fdow = (fdow == 7) ? 0 : fdow;

			int diff = dow - fdow;
			DateTime result;
			if (diff < 0)
				result = start.AddDays(-(7 + diff));
			else
				result = start.AddDays(-diff);

			if (result.Year < start.Year)
				result = new DateTime(start.Year, 1, 1);

			return result;
		}
		#endregion

		#region GetGroupLink(int GroupID,string GroupTitle)
		public static string GetGroupLink(int GroupID,string GroupTitle)
		{
			/*
			if (Security.CurrentUser.IsExternal)
				return GetGroupLinkUL(GroupID, GroupTitle);
			*/
			string path = HttpRuntime.AppDomainAppVirtualPath;
			if (path == "/") path = "";
			/*
			if(GroupID == (int)InternalSecureGroups.Administrator
				|| GroupID == (int)InternalSecureGroups.PowerProjectManager
				|| GroupID == (int)InternalSecureGroups.ProjectManager
				|| GroupID == (int)InternalSecureGroups.HelpDeskManager
				|| GroupID == (int)InternalSecureGroups.ExecutiveManager
				|| GroupID == (int)InternalSecureGroups.Everyone
				|| GroupID == (int)InternalSecureGroups.Roles
				|| GroupID == (int)InternalSecureGroups.Intranet)
				return String.Format("<a href='" + path +"/Directory/Directory.aspx?Tab=0&SGroupID={0}'><img src='../layouts/images/icons/admins.GIF' border=0 align='absmiddle'>&nbsp;{1}</a>", GroupID.ToString(), GroupTitle);
			else if (SecureGroup.IsPartner(GroupID) || GroupID==(int)InternalSecureGroups.Partner)
				return String.Format("<a href='" + path +"/Directory/Directory.aspx?Tab=0&SGroupID={0}'><img src='../layouts/images/icons/partners.GIF' border=0 align='absmiddle'>&nbsp;{1}</a>", GroupID.ToString(), GroupTitle);
			else
			*/
			return String.Format("<a href='" + path + "/Directory/Directory.aspx?Tab=0&SGroupID={0}'><img alt='' src='" + GetAbsolutePath("/layouts/images/icons/regular.GIF") + "' border='0' align='absmiddle'/>&nbsp;{1}</a>", GroupID.ToString(), GroupTitle);
		}

		// Alex ??
		public static string GetGroupLinkUL(int GroupID,string GroupTitle)
		{
			/*
			if(GroupID == (int)InternalSecureGroups.Administrator
				|| GroupID == (int)InternalSecureGroups.PowerProjectManager
				|| GroupID == (int)InternalSecureGroups.ProjectManager
				|| GroupID == (int)InternalSecureGroups.HelpDeskManager
				|| GroupID == (int)InternalSecureGroups.ExecutiveManager
				|| GroupID == (int)InternalSecureGroups.Everyone
				|| GroupID == (int)InternalSecureGroups.Roles
				|| GroupID == (int)InternalSecureGroups.Intranet)
				return String.Format("<img src='../layouts/images/icons/admins.GIF' border=0 align='absmiddle'>&nbsp;{1}", GroupID.ToString(), GroupTitle);
			else if (SecureGroup.IsPartner(GroupID) || GroupID==(int)InternalSecureGroups.Partner)
				return String.Format("<img src='../layouts/images/icons/partners.GIF' border=0 align='absmiddle'>&nbsp;{1}", GroupID.ToString(), GroupTitle);
			else
			*/
			return String.Format("<img alt='' src='" + GetAbsolutePath("/layouts/images/icons/regular.GIF") + "' border='0' align='absmiddle'/>&nbsp;{1}", GroupID.ToString(), GroupTitle);

		}

		public static string GetGroupLink(int GroupID)
		{
			string GroupName = "";
			using(IDataReader reader = UserReport.GetGroup(GroupID))
			{
				if (reader.Read())
					GroupName = (string)reader["GroupName"];
			}
			return GetGroupLink(GroupID, GroupName);
		}
/*
		public static string GetGroupLinkUL(int GroupID)
		{
			string GroupName = "";
			using(IDataReader reader = UserReport.GetGroup(GroupID))
			{
				if (reader.Read())
					GroupName = (string)reader["GroupName"];
			}
			return GetGroupLinkUL(GroupID, GroupName);
		}

		public static string GetGroupPureName(int GroupID)
		{
			string GroupName = "";
			using(IDataReader reader = UserReport.GetGroup(GroupID))
			{
				if (reader.Read())
					GroupName = (string)reader["GroupName"];
			}
			return GroupName;
		}
*/		
		#endregion

		#region GetUserStatus
		public static string GetUserStatus(int UserID)
		{
			string RetVal = "";
			Hashtable	UserStatusList = new Hashtable();

			HttpContext context = HttpContext.Current;
			if(context != null && context.Items.Contains(sUserStatus))
				UserStatusList = (Hashtable)context.Items[sUserStatus];

			if (UserStatusList.ContainsKey(UserID))
			{
				RetVal = UserStatusList[UserID].ToString();
			}
			else
			{
        ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.GlobalModules.Resources.strTemplate", typeof(CommonHelper).Assembly);
			
				string UserName = "";
				string path = HttpRuntime.AppDomainAppVirtualPath;
				if (path == "/") path = "";
				string Domain = Mediachase.IBN.Business.Configuration.Domain;
			
				using(IDataReader reader = UserReport.GetUserInfo(UserID,false))
				{
					if (reader.Read())
					{
						bool IsActive = (bool)reader["IsActive"];
						bool IsExternal = (bool)reader["IsExternal"];
						bool IsPending = (bool)reader["IsPending"];
						bool IsImInternal = true;
						try
						{
							IsImInternal = Mediachase.IBN.Business.Security.CurrentUser.IsExternal;
						}
						catch {}
						
						UserName = (string)reader["FirstName"] + " " + (string)reader["LastName"];
						if(!IsActive)
							UserName = "<font color='#707070'>" + UserName + "</font>";
						string UserIMStatus = "";

						if (!IsExternal && !IsPending)
						{
							if (reader["OriginalId"] != DBNull.Value)
							{
								int OriginalId = (int)reader["OriginalId"];
								UserIMStatus = @"<a href='ibnto:" + (string)reader["Login"] + "@" + Domain + "' title='" + LocRM.GetString("SendIBNMessage") + "'><img alt='' src='../WebServices/UserStatusImage.aspx?UserID=" + OriginalId + "' border='0' align='absmiddle'/></a>&nbsp;";
							}
						}
						else if (IsExternal) UserIMStatus = "<img alt='' src='../layouts/images/status/status_external.gif' border='0' align='absmiddle' title='" + LocRM.GetString("ExternalUser") + "'/>&nbsp;";
						else if (IsPending) UserIMStatus = "<img alt='' src='../layouts/images/status/status_pending.gif' border='0' align='absmiddle' title='" + LocRM.GetString("PendingUser") + "'/>&nbsp;";

						if (IsImInternal)
							RetVal = String.Format(
								"<a href='mailto:{2}' title='" + LocRM.GetString("SendEmail") + "'>" +
								"<img alt='' src='../layouts/images/mail.GIF' border='0' align='absmiddle'/></a>&nbsp;" +
								"{0}", UserName,UserID,reader["Email"]);
						else
							RetVal = String.Format(UserIMStatus + "<a href='mailto:{2}' title='" +
								LocRM.GetString("SendEmail")+"'>" +
								"<img alt='' src='../layouts/images/mail.GIF' border='0' align='absmiddle'/></a>&nbsp;" +
								"<a href='" + path + "/Directory/UserView.aspx?UserID={1}' title='" + LocRM.GetString("ViewUserProfile") + "'>{0}</a>", UserName, UserID, reader["Email"]);
						UserStatusList.Add(UserID, RetVal);
						context.Items.Remove(sUserStatus);
						context.Items.Add(sUserStatus, UserStatusList);
					}
				}

				if (RetVal == String.Empty)
				{
					RetVal = GetGroupLink(UserID);
					UserStatusList.Add(UserID, RetVal);
					context.Items.Remove(sUserStatus);
					context.Items.Add(sUserStatus, UserStatusList);
				}
			}

			return RetVal;
		}
		#endregion

		#region ExportExcel
		public static void ExportExcel(Control ctrl,string filename,HttpResponse _Response)
		{
			HttpResponse Response = HttpContext.Current.Response;
			Response.Clear();
			Response.Charset = "utf-8";
			Response.AddHeader("Content-Type","application/octet-stream");
			Response.AddHeader("content-disposition", String.Format("attachment; filename={0}", filename));
			System.IO.StringWriter stringWrite =  new System.IO.StringWriter();
			System.Web.UI.HtmlTextWriter htmlWrite  = new System.Web.UI.HtmlTextWriter(stringWrite);
			ctrl.RenderControl(htmlWrite);
			string Header = "<html><head><meta http-equiv=\"Content-Type\" content=\"application/octet-stream; charset=utf-8\"></head><body>";
			string Footer = "</body></html>";
			Response.Write(String.Concat(Header,stringWrite.ToString(),Footer));
			Response.End();
		}
		#endregion

		#region GetStringInterval(TimeSpan ts)
		public static string GetStringInterval(TimeSpan ts)
		{
      ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.GlobalModules.Resources.strTemplate", typeof(CommonHelper).Assembly);
			StringBuilder sb = new StringBuilder();

			if (ts.Days > 0)
				sb.Append(ts.Days + " " + LocRM.GetString("Days") + " ");
			if (ts.Days > 0 ||  ts.Hours > 0)
				sb.Append(ts.Hours + " " + LocRM.GetString("Hours")+ " ");
			sb.Append(" " + ts.Minutes + " " + LocRM.GetString("Minutes"));
			return sb.ToString();  
		}
		#endregion

		#region GetAbsolutePath
		public static string GetAbsolutePath(string xs_Path)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(HttpContext.Current.Request.Url.Scheme);
			builder.Append("://");
			builder.Append(HttpContext.Current.Request.Url.Host);
			builder.Append(HttpContext.Current.Request.ApplicationPath);
			builder.Append("/");
			if(xs_Path != string.Empty)
			{
				if(xs_Path[0] == '/')
					xs_Path = xs_Path.Substring(1, xs_Path.Length - 1);
				builder.Append(xs_Path);
			}
			return builder.ToString();
		}
		#endregion

		#region GetResFileString
		public static string GetResFileString(string fileAndKey)//for {file:key} values
		{
			return Mediachase.IBN.Business.Common.GetWebResourceString(fileAndKey, CultureInfo.CurrentUICulture);
		}
		#endregion
	}
}
