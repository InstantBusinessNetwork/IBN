using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Business.Documents;
using Mediachase.IBN.Business.WebDAV.Common;
using Mediachase.Ibn.Clients;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Web.UI;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.UI.Web.WebServices;
using Mediachase.Ibn.Events;
using System.Threading;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business.WebDAV;

namespace Mediachase.UI.Web.Util
{
	/// <summary>
	/// Summary description for CommonHelper.
	/// </summary>
	public class CommonHelper
	{
		protected readonly static int timeOutInterval = 1000;
		public const string ChartPath = "~/Layouts/images/Charts/";
		private const string userStatusKey = "userstatus";
		private const string userStatusAndPositionKey = "userstatusposition";
		private const string userNameKey = "username";
		private const string userNameAndPositionKey = "usernameposition";

		public static string ProductFamilyShort { get { return Mediachase.Ibn.IbnConst.ProductFamilyShort; } }

		#region GetWeekStartByDate
		public static DateTime GetWeekStartByDate(DateTime start)
		{
			start = start.Date;
			int dow = (int)start.DayOfWeek;
			int fdow = PortalConfig.PortalFirstDayOfWeek;
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

		#region GetRealWeekStartByDate
		public static DateTime GetRealWeekStartByDate(DateTime start)
		{
			int dow = (int)start.DayOfWeek;
			int fdow = PortalConfig.PortalFirstDayOfWeek;
			fdow = (fdow == 7) ? 0 : fdow;

			int diff = dow - fdow;
			DateTime result;
			if (diff < 0)
				result = start.AddDays(-(7 + diff));
			else
				result = start.AddDays(-diff);

			return result;
		}
		#endregion

		#region GetWeekEndByDate
		public static DateTime GetWeekEndByDate(DateTime start)
		{
			int dow = (int)start.DayOfWeek;
			int fdow = PortalConfig.PortalFirstDayOfWeek;
			fdow = (fdow == 7) ? 0 : fdow;

			int diff = dow - fdow;
			DateTime result;
			if (diff < 0)
				result = start.AddDays(-(7 + diff));
			else
				result = start.AddDays(-diff);
			result = result.AddDays(6);
			if (result.Year > start.Year)
				return new DateTime(start.Year, 12, 31);
			else
				return result;
		}
		#endregion

		#region GetRealWeekEndByDate
		public static DateTime GetRealWeekEndByDate(DateTime start)
		{
			int dow = (int)start.DayOfWeek;
			int fdow = PortalConfig.PortalFirstDayOfWeek;
			fdow = (fdow == 7) ? 0 : fdow;

			int diff = dow - fdow;
			DateTime result;
			if (diff < 0)
				result = start.AddDays(-(7 + diff));
			else
				result = start.AddDays(-diff);
			result = result.AddDays(6);
			return result;
		}
		#endregion

		#region EndWithSlash
		public static string EndWithSlash(string s)
		{
			string ret = s;
			if (!ret.EndsWith("/") && !ret.EndsWith("\\"))
				ret += "/";
			return ret;
		}
		#endregion

		#region CloseIt
		public static void CloseIt(HttpResponse response)
		{
			response.Clear();
			response.Write("<script type=\"text/javascript\">");
			response.Write("window.close();");
			response.Write("</script>");
			response.End();
		}
		#endregion

		#region CloseItAndRefresh
		public static void CloseItAndRefresh(HttpResponse response)
		{
			response.Clear();
			response.Write("<script type=\"text/javascript\">");
			response.Write("try {window.opener.document.forms[0].submit();} catch (e) {}");
			response.Write("window.close()");
			response.Write("</script>");
			response.End();
		}

		public static void CloseItAndRefresh(HttpResponse response, string refreshButton)
		{
			response.Clear();
			response.Write("<script language=\"javascript\" type=\"text/javascript\">");
			response.Write(string.Format(CultureInfo.InvariantCulture, "try {{window.opener.{0};}} catch (e) {{}}", refreshButton));
			response.Write(string.Format(CultureInfo.InvariantCulture, "setTimeout('window.close()', {0});", timeOutInterval));
			response.Write("</script>");
			response.End();
		}
		#endregion

		#region GetCloseRefreshString
		public static string GetCloseRefreshString(string RefreshButton)
		{
			string retVal = String.Format("try {{window.opener.{0};}} catch (e) {{}}", RefreshButton);
			retVal += String.Format("setTimeout('window.close()', {0}); return false;", timeOutInterval);
			return retVal;
		}
		#endregion

		#region GetTable
		public static System.Data.DataTable GetTable(System.Data.IDataReader _reader)
		{

			System.Data.DataTable _table = _reader.GetSchemaTable();
			System.Data.DataTable _dt = new System.Data.DataTable();
			System.Data.DataColumn _dc;
			System.Data.DataRow _row;
			System.Collections.ArrayList _al = new System.Collections.ArrayList();

			for (int i = 0; i < _table.Rows.Count; i++)
			{

				_dc = new System.Data.DataColumn();

				if (!_dt.Columns.Contains(_table.Rows[i]["ColumnName"].ToString()))
				{

					_dc.ColumnName = _table.Rows[i]["ColumnName"].ToString();
					_dc.Unique = Convert.ToBoolean(_table.Rows[i]["IsUnique"]);
					_dc.AllowDBNull = Convert.ToBoolean(_table.Rows[i]["AllowDBNull"]);
					_dc.ReadOnly = Convert.ToBoolean(_table.Rows[i]["IsReadOnly"]);
					_dc.DataType = (Type)_table.Rows[i]["DataType"];
					_al.Add(_dc.ColumnName);
					_dt.Columns.Add(_dc);
				}
			}

			while (_reader.Read())
			{
				_row = _dt.NewRow();
				for (int i = 0; i < _al.Count; i++)
				{
					_row[((System.String)_al[i])] = _reader[(System.String)_al[i]];
				}
				_dt.Rows.Add(_row);
			}
			_reader.Close();
			return _dt;
		}
		#endregion

		#region ByteSizeToStr
		public static string ByteSizeToStr(int size)
		{
			string sReturn;

			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(CommonHelper).Assembly);

			if (size < 1024)
				sReturn = String.Format("{0} " + LocRM.GetString("tbytes"), size);
			else if (size < 1000 * 1024)
				sReturn = String.Format("{0,0:f} " + LocRM.GetString("tKBytes"), size / 1024.0);
			else
				sReturn = String.Format("{0,0:f} " + LocRM.GetString("tMBytes"), size / (1024.0 * 1024));

			return sReturn;
		}
		#endregion

		#region GetIncidentTitle
		public static string GetIncidentTitle(string title, int incidentId, bool isOverdue, int stateId, string stateName)
		{
			return GetIncidentTitle(title, incidentId, isOverdue, stateId, stateName, false);
		}

		public static string GetIncidentTitle(string title, int incidentId, bool isOverdue, int stateId, string stateName, bool isBoldText)
		{
			string img = "incident_closed.gif";
			if (isOverdue)
			{
				img = "incident_overdue.gif";
				stateName = String.Format(CultureInfo.InvariantCulture,
					"{0}, {1}",
					stateName,
					HttpContext.GetGlobalResourceObject("IbnFramework.Incident", "Overdue"));
			}
			else if (stateId == (int)ObjectStates.Upcoming)
			{
				img = "incident_new.gif";
			}
			else if (stateId == (int)ObjectStates.Active || stateId == (int)ObjectStates.ReOpen || stateId == (int)ObjectStates.OnCheck)
			{
				img = "incident_active.gif";
			}

			if (!Security.CurrentUser.IsExternal)
				return String.Format(CultureInfo.InvariantCulture,
					"<a href='{0}?IncidentId={1}'><img alt='{3}' src='{2}' title='{3}'/> {4}</a>", 
					GetAbsolutePath("/Incidents/IncidentView.aspx"),
					incidentId,
					GetAbsolutePath("/Layouts/Images/icons/" + img),
					stateName,
					((isBoldText) ? ("<b>" + title + "</b>") : title));
			else
				return String.Format(CultureInfo.InvariantCulture,
					"<img alt='{1}' src='{0}' title='{1}'/> {2}",
					GetAbsolutePath("/Layouts/Images/icons/" + img),
					stateName,
					((isBoldText) ? ("<b>" + title + "</b>") : title));
		}

		public static string GetIncidentTitle(Page pageInstance, string title, int incidentId, bool isOverdue, int stateId, string stateName)
		{
			return GetIncidentTitle(pageInstance, title, incidentId, isOverdue, stateId, stateName, false);
		}

		public static string GetIncidentTitle(Page pageInstance, string title, int incidentId, bool isOverdue, int stateId, string stateName, bool isBoldText)
		{
			string img = "incident_closed.gif";
			if (isOverdue)
			{
				img = "incident_overdue.gif";
				stateName = String.Format(CultureInfo.InvariantCulture,
					"{0}, {1}",
					stateName,
					HttpContext.GetGlobalResourceObject("IbnFramework.Incident", "Overdue"));
			}
			else if (stateId == (int)ObjectStates.Upcoming)
			{
				img = "incident_new.gif";
			}
			else if (stateId == (int)ObjectStates.Active || stateId == (int)ObjectStates.ReOpen || stateId == (int)ObjectStates.OnCheck)
			{
				img = "incident_active.gif";
			}
			if (!Security.CurrentUser.IsExternal)
				return String.Format(CultureInfo.InvariantCulture,
					"<a href='{0}?IncidentId={1}'><img alt='{4}' src='{2}/{3}' title='{4}'/> {5}</a>",
					pageInstance.ResolveClientUrl("~/Incidents/IncidentView.aspx"),
					incidentId, 
					pageInstance.ResolveClientUrl("~/layouts/images/icons"),
					img, 
					stateName, 
					((isBoldText) ? ("<b>" + title + "</b>") : title));
			else return String.Format(CultureInfo.InvariantCulture,
				"<img alt='{2}' src='{0}/{1}' title='{2}'/> {3}",
				pageInstance.ResolveClientUrl("~/Layouts/Images/icons"),
				img, 
				stateName, 
				((isBoldText) ? ("<b>" + title + "</b>") : title));
		}
		#endregion

		#region GetIncidentTitle
		public static string GetIncidentTitle(int incidentId)
		{
			string title = "";
			string stateName = "";
			int stateId = 1;
			bool isOverdue = false;

			string ret = string.Empty;
			using (IDataReader rdr = Incident.GetIncident(incidentId, false))
			{
				if (rdr.Read())
				{
					title = (string)rdr["Title"];
					stateName = (string)rdr["StateName"];
					stateId = (int)rdr["StateId"];
					isOverdue = (bool)rdr["IsOverdue"];
				}
			}

			return GetIncidentTitle(title, incidentId, isOverdue, stateId, stateName);
		}
		#endregion

		#region GetIncidentTitle
		public static string GetIncidentTitle(int IncidentID, string Title)
		{
			if (Incident.CanRead(IncidentID))
				return GetIncidentTitle(IncidentID);
			else
				return String.Format(CultureInfo.InvariantCulture,
					"<img alt='' src='{0}'> {1}",
					GetAbsolutePath("/Layouts/Images/icons/incident.gif"),
					Title);
		}
		#endregion

		#region GetIncidentTitleWL
		public static string GetIncidentTitleWL(int incidentId)
		{
			string ret = string.Empty;
			using (IDataReader rdr = Incident.GetIncident(incidentId, false))
			{
				if (rdr.Read())
				{
					string img = "incident_closed.gif";
					string title = (string)rdr["Title"];
					string stateName = (string)rdr["StateName"];
					int stateId = (int)rdr["StateId"];
					if ((bool)rdr["IsOverdue"])
						img = "incident_overdue.gif";
					else if (stateId == (int)ObjectStates.Upcoming)
						img = "incident_new.gif";
					else if (stateId == (int)ObjectStates.Active || stateId == (int)ObjectStates.ReOpen || stateId == (int)ObjectStates.OnCheck)
						img = "incident_active.gif";
					ret = String.Format(CultureInfo.InvariantCulture,
						"<img alt='{2}' src='{0}/{1}' title='{2}'/> {3}",
						GetAbsolutePath("/Layouts/Images/icons"),
						img,
						stateName,
						title);
				}
			}
			return ret;
		}

		public static string GetIncidentTitleWL(string incidentTitle, int stateId, bool isOverdue)
		{
			string ret = string.Empty;
			string img = "incident_closed.gif";
			if (isOverdue)
				img = "incident_overdue.gif";
			else if (stateId == (int)ObjectStates.Upcoming)
				img = "incident_new.gif";
			else if (stateId == (int)ObjectStates.Active || stateId == (int)ObjectStates.ReOpen || stateId == (int)ObjectStates.OnCheck)
				img = "incident_active.gif";
			ret = String.Format(CultureInfo.InvariantCulture,
				"<img alt='' src='{0}/{1}'/> {2}",
				GetAbsolutePath("/Layouts/Images/icons"),
				img, 
				incidentTitle);
			return ret;
		}
		#endregion

		#region GetEventTitle
		public static string GetEventTitle(int EventID, int stateId, string Title)
		{
			string img = "event.gif";
			switch (stateId)
			{
				case (int)ObjectStates.Active:
					img = "event_started.gif";
					break;
				case (int)ObjectStates.Completed:
					img = "event_completed.gif";
					break;
			}

			return String.Format(
				@"<a href='{0}?EventId={1}'><img alt='' src='{2}'> {3}</a>", 
				GetAbsolutePath("/Events/EventView.aspx"),
				EventID,
				GetAbsolutePath("/Layouts/Images/icons/" + img),
				Title);
		}
		#endregion

		#region GetEventTitle
		public static string GetEventTitle(int EventID, int stateId, string Title, int SharedId)
		{
			string _SharedId = String.Empty;
			if (SharedId > 0)
				_SharedId = "&SharedId=" + SharedId;

			string img = "event.gif";
			switch (stateId)
			{
				case (int)ObjectStates.Active:
					img = "event_started.gif";
					break;
				case (int)ObjectStates.Completed:
					img = "event_completed.gif";
					break;
			}

			return String.Format(CultureInfo.InvariantCulture,
				@"<a href='{0}?EventId={1}{2}'><img alt='' src='{3}'> {4}</a>", 
				GetAbsolutePath("/Events/EventView.aspx"),
				EventID, 
				_SharedId,
				GetAbsolutePath("/Layouts/Images/icons/" + img),
				Title);
		}
		#endregion

		#region GetTaskToDoLink
		public static string GetTaskToDoLink(int ID, int IsToDo)
		{
			string ret = string.Empty;
			int stateId = (int)ObjectStates.Upcoming;
			string Title = "";
			using (IDataReader rdr = (IsToDo != 1) ? Task.GetTask(ID, false) : Mediachase.IBN.Business.ToDo.GetToDo(ID, false))
			{
				if (rdr.Read())
				{
					stateId = (int)rdr["StateId"];
					Title = rdr["Title"].ToString();
				}
			}
			return GetTaskToDoLink(ID, IsToDo, Title, stateId, "");
		}

		public static string GetTaskToDoLink(int ID, int IsToDo, string Title, int stateId)
		{
			return GetTaskToDoLink(ID, IsToDo, Title, stateId, "");
		}

		public static string GetTaskToDoLink(int ID, int IsToDo, string Title, int stateId, string Target)
		{
			string img = (IsToDo == 1) ? "task_started.gif" : "task1_started.gif";
			if (stateId == (int)ObjectStates.Active)
				img = (IsToDo == 1) ? "task.gif" : "task1.gif";
			else if (stateId == (int)ObjectStates.Overdue)
				img = (IsToDo == 1) ? "task_overdue.gif" : "task1_overdue.gif";
			else if (stateId == (int)ObjectStates.Suspended)
				img = (IsToDo == 1) ? "task_suspensed.gif" : "task1_suspensed.gif";
			else if (stateId == (int)ObjectStates.Completed)
				img = (IsToDo == 1) ? "task_completed.gif" : "task1_completed.gif";

			string link = "";
			if (IsToDo == 1)
				link = GetAbsolutePath("/ToDo/ToDoView.aspx?ToDoID=" + ID);
			else
				link = GetAbsolutePath("/Tasks/TaskView.aspx?TaskID=" + ID);

			if (Target != "")
				Target = String.Format(" target='{0}'", Target);
			
			return String.Format("<a href='{0}'{3}><img alt='' src='{2}'/> {1} (#{4})</a>", link, Title, GetAbsolutePath("/Layouts/Images/icons/" + img), Target, ID);
		}

		public static string GetToDoLink(int ID, string Title, int stateId)
		{
			string img = "task_started.gif";
			if (stateId == (int)ObjectStates.Active)
				img = "task.gif";
			else if (stateId == (int)ObjectStates.Overdue)
				img = "task_overdue.gif";
			else if (stateId == (int)ObjectStates.Suspended)
				img = "task_suspensed.gif";
			else if (stateId == (int)ObjectStates.Completed)
				img = "task_completed.gif";

			string link = GetAbsolutePath("/ToDo/ToDoView.aspx?ToDoID=" + ID);

			return String.Format("<a href='{0}'><img alt='' src='{2}'/> {1}</a>", link, Title, GetAbsolutePath("/Layouts/Images/icons/" + img));
		}
		#endregion

		#region GetTaskToDoLink
		public static string GetTaskToDoLink(int ID, int IsToDo, string Title)
		{
			string realimg = (IsToDo == 1) ? "task.gif" : "task1.gif";
			string img = "<img alt='' src='" + GetAbsolutePath("/Layouts/Images/icons/" + realimg) + "'/> ";
			if (IsToDo == 1)
			{
				if (Mediachase.IBN.Business.ToDo.CanRead(ID))
					return GetTaskToDoLink(ID, IsToDo);
				else
					return String.Concat(img, Title);
			}
			else
			{
				if (Mediachase.IBN.Business.Task.CanRead(ID))
					return GetTaskToDoLink(ID, IsToDo);
				else
					return String.Concat(img, Title);
			}
		}
		#endregion

		#region GetToDoStatusWL
		public static string GetToDoStatusWL(string title, int stateId, Page page)
		{
			string img = "task_started.gif";
			if (stateId == (int)ObjectStates.Active)
				img = "task.gif";
			else if (stateId == (int)ObjectStates.Overdue)
				img = "task_overdue.gif";
			else if (stateId == (int)ObjectStates.Suspended)
				img = "task_suspensed.gif";
			else if (stateId == (int)ObjectStates.Completed)
				img = "task_completed.gif";

			return String.Format(CultureInfo.InvariantCulture,
				"<img src='{0}' alt='' width='16px' height='16px' /> {1}",
				page.ResolveClientUrl("~/Layouts/Images/icons/" + img),
				title);
		}
		#endregion

		#region GetProjectStatus
		public static string GetProjectStatus(int ProjectID)
		{
			string ret = string.Empty;
			try
			{
				using (IDataReader rdr = Project.GetProject(ProjectID))
				{
					if (rdr.Read())
					{
						string img = "project_active.gif";
						Project.ProjectStatus ps = (Project.ProjectStatus)rdr["StatusId"];
						if (ps == Project.ProjectStatus.Pending || ps == Project.ProjectStatus.OnHold)
							img = "project_pending.gif";
						if (ps == Project.ProjectStatus.AtRisk)
							img = "project_atrisk.gif";
						if (ps == Project.ProjectStatus.Completed || ps == Project.ProjectStatus.Cancelled)
							img = "project_completed.gif";
						string title = (string)rdr["Title"];
						string link_img = GetAbsolutePath(String.Format("/Layouts/Images/icons/{0}", img));
						string link_prj = GetAbsolutePath("/Projects/ProjectView.aspx");
						ret = String.Format(
							@"<a href='{0}?ProjectId={1}'><img alt='' src='{2}' title='{3}'/> {3}</a>",
							link_prj, ProjectID, link_img, title);
					}
				}
			}
			catch
			{
				ret = String.Format("{0}", Task.GetProjectTitle(ProjectID));
			}
			return ret;
		}
		#endregion

		#region GetProjectStatusWithId
		public static string GetProjectStatusWithId(int ProjectID)
		{
			string ret = string.Empty;
			try
			{
				using (IDataReader rdr = Project.GetProject(ProjectID))
				{
					if (rdr.Read())
					{
						string img = "project_active.gif";
						Project.ProjectStatus ps = (Project.ProjectStatus)rdr["StatusId"];
						if (ps == Project.ProjectStatus.Pending || ps == Project.ProjectStatus.OnHold)
							img = "project_pending.gif";
						if (ps == Project.ProjectStatus.AtRisk)
							img = "project_atrisk.gif";
						if (ps == Project.ProjectStatus.Completed || ps == Project.ProjectStatus.Cancelled)
							img = "project_completed.gif";
						string title = (string)rdr["Title"] + CHelper.GetProjectNumPostfix(ProjectID, (string)rdr["ProjectCode"]);
						ret = String.Format(CultureInfo.InvariantCulture,
							@"<a href='{0}?ProjectId={1}'><img alt='' src='{2}'/> {3}</a>",
							GetAbsolutePath("/Projects/ProjectView.aspx"),
							ProjectID, 
							GetAbsolutePath("/Layouts/Images/icons/" + img),
							title);
					}
				}
			}
			catch
			{
				ret = Task.GetProjectTitle(ProjectID);
			}
			return ret;
		}
		#endregion

		#region GetProjectStatusWL
		public static string GetProjectStatusWL(int ProjectID)
		{
			string ret = string.Empty;
			try
			{
				using (IDataReader rdr = Project.GetProject(ProjectID))
				{
					if (rdr.Read())
					{
						string img = "project_active.gif";
						Project.ProjectStatus ps = (Project.ProjectStatus)rdr["StatusId"];
						if (ps == Project.ProjectStatus.Pending || ps == Project.ProjectStatus.OnHold)
							img = "project_pending.gif";
						if (ps == Project.ProjectStatus.AtRisk)
							img = "project_atrisk.gif";
						if (ps == Project.ProjectStatus.Completed || ps == Project.ProjectStatus.Cancelled)
							img = "project_completed.gif";
						string title = rdr["Title"].ToString();
						ret = String.Format(
							@"<img alt='' src='{3}/{1}' title='{2}'/> {0}",
							title, img, "", GetAbsolutePath("/Layouts/Images/icons"));
					}
				}
			}
			catch
			{
				ret = String.Format("{0}", Task.GetProjectTitle(ProjectID));
			}
			return ret;
		}
		#endregion

		#region GetProjectStatus
		public static string GetProjectStatus(int ProjectID, string Title)
		{
			if (Project.CanRead(ProjectID))
				return GetProjectStatus(ProjectID);
			else return
					 String.Format(CultureInfo.InvariantCulture,
					 "<img src='{0}'> {1}",
					 GetAbsolutePath("/Layouts/Images/icons/project.gif"),
					 Title);
		}
		#endregion

		#region GetProjectStatus
		public static string GetProjectStatus(int ProjectID, string Title, int StatusId)
		{
			string img = "project_active.gif";

			if (StatusId == (int)Project.ProjectStatus.Pending || StatusId == (int)Project.ProjectStatus.OnHold)
				img = "project_pending.gif";
			if (StatusId == (int)Project.ProjectStatus.AtRisk)
				img = "project_atrisk.gif";
			if (StatusId == (int)Project.ProjectStatus.Completed || StatusId == (int)Project.ProjectStatus.Cancelled)
				img = "project_completed.gif";
			return String.Format(CultureInfo.InvariantCulture,
				@"<a href='{0}?ProjectId={1}'><img alt='' src='{2}'/> {3}</a>",
				GetAbsolutePath("/Projects/ProjectView.aspx"),
				ProjectID, 
				GetAbsolutePath("/Layouts/Images/icons/" + img),
				Title);
		}

		public static string GetProjectStatus(Page pageInstance, int ProjectID, string Title, int StatusId)
		{
			string img = "project_active.gif";

			if (StatusId == (int)Project.ProjectStatus.Pending || StatusId == (int)Project.ProjectStatus.OnHold)
				img = "project_pending.gif";
			if (StatusId == (int)Project.ProjectStatus.AtRisk)
				img = "project_atrisk.gif";
			if (StatusId == (int)Project.ProjectStatus.Completed || StatusId == (int)Project.ProjectStatus.Cancelled)
				img = "project_completed.gif";
			return String.Format(
				@"<a href='{4}?ProjectId={0}'><img alt='' src='{5}/{2}' title='{3}'/> {1}</a>",
				ProjectID, Title, img, "", 
				pageInstance.ResolveClientUrl("~/Projects/ProjectView.aspx"),
				pageInstance.ResolveClientUrl("~/Layouts/Images/icons"));
		}
		#endregion

		#region GetProjectStatusWL
		public static string GetProjectStatusWL(string Title, int StatusId)
		{
			string img = "project_active.gif";

			if (StatusId == (int)Project.ProjectStatus.Pending || StatusId == (int)Project.ProjectStatus.OnHold)
				img = "project_pending.gif";
			if (StatusId == (int)Project.ProjectStatus.AtRisk)
				img = "project_atrisk.gif";
			if (StatusId == (int)Project.ProjectStatus.Completed || StatusId == (int)Project.ProjectStatus.Cancelled)
				img = "project_completed.gif";
			return String.Format(
				@"<img alt='' src='{2}/{1}'/> {0}</a>", 
					Title, img, GetAbsolutePath("/Layouts/Images/icons"));
		}
		#endregion


		#region public static string GetGroupPureName(int groupId)
		public static string GetGroupPureName(int groupId)
		{
			string result = string.Empty;

			using (IDataReader reader = SecureGroup.GetGroup(groupId))
			{
				if (reader.Read())
					result = GetResFileString(reader["GroupName"].ToString());
			}

			return result;
		}
		#endregion

		#region public static string GetGroupLink(int groupId)
		public static string GetGroupLink(int groupId)
		{
			string result = string.Empty;

			string groupName = GetGroupPureName(groupId);
			if (!string.IsNullOrEmpty(groupName))
				result = GetGroupLink(groupId, groupName);

			return result;
		}
		#endregion
		#region public static string GetGroupLink(int groupId, string groupTitle)
		public static string GetGroupLink(int groupId, string groupTitle)
		{
			if (Security.CurrentUser.IsExternal)
				return GetGroupLinkUL(groupId, groupTitle);
			else
				return BuildGroupTitle(groupId, groupTitle, true);
		}
		#endregion

		#region public static string GetGroupLinkUL(int groupId)
		public static string GetGroupLinkUL(int groupId)
		{
			string groupName = GetGroupPureName(groupId);
			return GetGroupLinkUL(groupId, groupName);
		}
		#endregion
		#region public static string GetGroupLinkUL(int groupId, string groupTitle)
		public static string GetGroupLinkUL(int groupId, string groupTitle)
		{
			return BuildGroupTitle(groupId, groupTitle, false);
		}
		#endregion

		#region private static string GetGroupImage(int groupId)
		private static string GetGroupImage(int groupId)
		{
			if (groupId == (int)InternalSecureGroups.Administrator
				|| groupId == (int)InternalSecureGroups.PowerProjectManager
				|| groupId == (int)InternalSecureGroups.ProjectManager
				|| groupId == (int)InternalSecureGroups.HelpDeskManager
				|| groupId == (int)InternalSecureGroups.ExecutiveManager
				|| groupId == (int)InternalSecureGroups.Everyone
				|| groupId == (int)InternalSecureGroups.Roles
				|| groupId == (int)InternalSecureGroups.TimeManager)
				return "Admins.gif";
			else if (SecureGroup.IsPartner(groupId) || groupId == (int)InternalSecureGroups.Partner)
				return "Partners.gif";
			else
				return "Regular.gif";
		}
		#endregion
		#region private static string BuildGroupTitle(int groupId, string groupTitle, bool addLink)
		private static string BuildGroupTitle(int groupId, string groupTitle, bool addLink)
		{
			string path = HttpRuntime.AppDomainAppVirtualPath;
			if (path == "/")
				path = "";

			StringBuilder builder = new StringBuilder();
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.ConformanceLevel = ConformanceLevel.Fragment;

			using (XmlWriter writer = XmlWriter.Create(builder, settings))
			{
				if (addLink)
				{
					writer.WriteStartElement("a");
					writer.WriteAttributeString("href", string.Concat(path, "/Directory/Directory.aspx?Tab=0&SGroupID=", groupId));
				}

				writer.WriteStartElement("span");
				writer.WriteStartElement("img");
				writer.WriteAttributeString("alt", string.Empty);
				writer.WriteAttributeString("src", string.Concat(path, "/Layouts/Images/icons/", GetGroupImage(groupId)));
				writer.WriteEndElement(); // img
				writer.WriteEndElement(); // span
				writer.WriteString(" ");
				writer.WriteStartElement("span");
				writer.WriteString(groupTitle);
				writer.WriteEndElement(); // span

				if (addLink)
					writer.WriteEndElement(); // a
			}

			return builder.ToString();
		}
		#endregion


		#region GetUserStatusPureName
		public static string GetUserStatusPureName(int UserID)
		{
			string RetVal = "";

			Hashtable UserNameList = new Hashtable();

			HttpContext context = HttpContext.Current;
			if (context != null && context.Items.Contains(userNameKey))
				UserNameList = (Hashtable)context.Items[userNameKey];

			if (UserNameList.ContainsKey(UserID))
			{
				RetVal = UserNameList[UserID].ToString();
			}
			else
			{
				using (IDataReader reader = User.GetUserInfo(UserID, false))
				{
					if (reader.Read())
						RetVal = (string)reader["LastName"] + " " + (string)reader["FirstName"];
				}
				if (RetVal == "")	// Group
					RetVal = GetGroupPureName(UserID);
				if (RetVal == String.Empty)
					RetVal = Mediachase.Ibn.Web.UI.CHelper.GetUserName(UserID);

				UserNameList.Add(UserID, RetVal);
				context.Items.Remove(userNameKey);
				context.Items.Add(userNameKey, UserNameList);
			}

			return RetVal;
		}
		#endregion
		#region GetUserStatusAndPositionPureName
		public static string GetUserStatusAndPositionPureName(int UserID)
		{
			string RetVal = "";

			Hashtable UserNameList = new Hashtable();

			HttpContext context = HttpContext.Current;
			if (context != null && context.Items.Contains(userNameAndPositionKey))
				UserNameList = (Hashtable)context.Items[userNameAndPositionKey];

			if (UserNameList.ContainsKey(UserID))
				RetVal = UserNameList[UserID].ToString();
			else
			{
				using (IDataReader reader = User.GetUserInfo(UserID, false))
				{
					if (reader.Read())
						RetVal = (string)reader["LastName"] + " " + (string)reader["FirstName"];
				}
				if (RetVal == "")	// Group
					RetVal = GetGroupPureName(UserID);
				if (RetVal == String.Empty)
					RetVal = Mediachase.Ibn.Web.UI.CHelper.GetUserName(UserID);

				using (IDataReader reader = User.GetUserProfile(UserID, false))
				{
					if (reader.Read())
					{
						if (reader["position"] != DBNull.Value)
						{
							string position = reader["position"].ToString();
							if (position != string.Empty)
								RetVal = string.Concat(RetVal, ", ", position);
						}
					}
				}

				UserNameList.Add(UserID, RetVal);
				context.Items.Remove(userNameAndPositionKey);
				context.Items.Add(userNameAndPositionKey, UserNameList);
			}

			return RetVal;
		}
		#endregion

		#region public static string GetUserStatus(int userId)
		public static string GetUserStatus(int userId)
		{
			string result = string.Empty;

			if (userId != 0)
			{
				Hashtable cache = new Hashtable();
				HttpContext context = HttpContext.Current;
				if (context != null && context.Items.Contains(userStatusKey))
					cache = (Hashtable)context.Items[userStatusKey];

				if (cache.ContainsKey(userId))
					result = cache[userId].ToString();
				else
				{
					StringBuilder builder = new StringBuilder();
					XmlWriterSettings settings = new XmlWriterSettings();
					settings.ConformanceLevel = ConformanceLevel.Fragment;

					using (XmlWriter writer = XmlWriter.Create(builder, settings))
					{
						ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(CommonHelper).Assembly);

						if (userId == -1) // system
						{
							writer.WriteStartElement("span");
							writer.WriteAttributeString("class", "user-system");
							writer.WriteString(LocRM.GetString("SystemUser"));
							writer.WriteEndElement(); // span
						}
						else
						{
							string path = HttpRuntime.AppDomainAppVirtualPath;
							if (path == "/")
								path = "";

							bool isCurrentUserExternal = true;
							try
							{
								isCurrentUserExternal = Security.CurrentUser.IsExternal;
							}
							catch { }

							using (IDataReader reader = User.GetUserInfo(userId, false))
							{
								if (reader.Read())
								{
									string userName = (string)reader["LastName"] + " " + (string)reader["FirstName"];

									bool isExternal = (bool)reader["IsExternal"];
									bool isPending = (bool)reader["IsPending"];

									if (Mediachase.IBN.Business.PortalConfig.UseIM && !isCurrentUserExternal)
									{
										if (!isExternal && !isPending)
										{
											if (reader["OriginalId"] != DBNull.Value)
											{
												int imId = (int)reader["OriginalId"];
												int statusId = Mediachase.IBN.Business.User.GetUserStatus(imId);
												Mediachase.UI.Web.WebServices.UserStatus userStatus = (Mediachase.UI.Web.WebServices.UserStatus)statusId;
												writer.WriteStartElement("a");
												writer.WriteAttributeString("href", string.Concat("ibnto:", (string)reader["Login"], "@", Mediachase.IBN.Business.Configuration.Domain));
												writer.WriteAttributeString("title", LocRM.GetString("SendIBNMessage"));
												writer.WriteStartElement("img");
												writer.WriteAttributeString("alt", string.Empty);
												writer.WriteAttributeString("src", string.Concat(path, "/Layouts/Images/status/status_", userStatus.ToString(), ".gif"));
												writer.WriteEndElement(); // img
												writer.WriteEndElement(); // a
											}
										}
										else if (isExternal)
										{
											writer.WriteStartElement("span");
											writer.WriteStartElement("img");
											writer.WriteAttributeString("alt", string.Empty);
											writer.WriteAttributeString("src", string.Concat(path, "/Layouts/Images/status/status_external.gif"));
											writer.WriteAttributeString("title", LocRM.GetString("ExternalUser"));
											writer.WriteEndElement(); // img
											writer.WriteEndElement(); // span
										}
										else if (isPending)
										{
											writer.WriteStartElement("span");
											writer.WriteStartElement("img");
											writer.WriteAttributeString("alt", string.Empty);
											writer.WriteAttributeString("src", string.Concat(path, "/Layouts/Images/status/status_pending.gif"));
											writer.WriteAttributeString("title", LocRM.GetString("PendingUser"));
											writer.WriteEndElement(); // img
											writer.WriteEndElement(); // span
										}
										writer.WriteString(" ");
									}

									writer.WriteStartElement("a");
									writer.WriteAttributeString("href", string.Concat("mailto:", (string)reader["Email"]));
									writer.WriteAttributeString("title", LocRM.GetString("SendEmail"));
									writer.WriteStartElement("img");
									writer.WriteAttributeString("alt", string.Empty);
									writer.WriteAttributeString("src", string.Concat(path, "/Layouts/Images/Mail.gif"));
									writer.WriteEndElement(); // img
									writer.WriteEndElement(); // a
									writer.WriteString(" ");

									if (!isCurrentUserExternal)
									{
										writer.WriteStartElement("a");
										writer.WriteAttributeString("href", string.Concat(path, "/Directory/UserView.aspx?UserID=", userId.ToString(CultureInfo.InvariantCulture)));
										writer.WriteAttributeString("title", LocRM.GetString("ViewUserProfile"));
									}

									writer.WriteStartElement("span");
									if (!(bool)reader["IsActive"])
										writer.WriteAttributeString("class", "user-disabled");
									writer.WriteString(userName);
									writer.WriteEndElement(); // span

									if (!isCurrentUserExternal)
										writer.WriteEndElement(); // a
								}
								else // Group
								{
									result = GetGroupLink(userId);
									if (string.IsNullOrEmpty(result))
									{
										writer.WriteStartElement("span");
										writer.WriteAttributeString("class", "user-unknown");
										writer.WriteString(Mediachase.Ibn.Web.UI.CHelper.GetUserName(userId));
										writer.WriteEndElement(); // span
									}
								}
							}
						}
					}

					if (string.IsNullOrEmpty(result))
						result = builder.ToString();

					if (userId != -1) // system
					{
						cache.Add(userId, result);
						context.Items.Remove(userStatusKey);
						context.Items.Add(userStatusKey, cache);
					}
				}
			}

			return result;
		}
		#endregion

		#region public static string GetUserStatus(int userId, string color)
		public static string GetUserStatus(int userId, string color)
		{
			string result = string.Empty;

			StringBuilder builder = new StringBuilder();
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.ConformanceLevel = ConformanceLevel.Fragment;

			using (XmlWriter writer = XmlWriter.Create(builder, settings))
			{
				ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(CommonHelper).Assembly);

				string path = HttpRuntime.AppDomainAppVirtualPath;
				if (path == "/")
					path = "";

				bool isCurrentUserExternal = true;
				try
				{
					isCurrentUserExternal = Security.CurrentUser.IsExternal;
				}
				catch { }

				using (IDataReader reader = User.GetUserInfo(userId, false))
				{
					if (reader.Read())
					{
						string userName = (string)reader["LastName"] + " " + (string)reader["FirstName"];

						bool isExternal = (bool)reader["IsExternal"];
						bool isPending = (bool)reader["IsPending"];

						if (Mediachase.IBN.Business.PortalConfig.UseIM && !isCurrentUserExternal)
						{
							if (!isExternal && !isPending)
							{
								if (reader["OriginalId"] != DBNull.Value)
								{
									int imId = (int)reader["OriginalId"];
									int statusId = Mediachase.IBN.Business.User.GetUserStatus(imId);
									Mediachase.UI.Web.WebServices.UserStatus userStatus = (Mediachase.UI.Web.WebServices.UserStatus)statusId;

									writer.WriteStartElement("a");
									writer.WriteAttributeString("href", string.Concat("ibnto:", (string)reader["Login"], "@", Mediachase.IBN.Business.Configuration.Domain));
									writer.WriteAttributeString("title", LocRM.GetString("SendIBNMessage"));
									writer.WriteStartElement("img");
									writer.WriteAttributeString("alt", string.Empty);
									writer.WriteAttributeString("src", string.Concat(path, "/Layouts/Images/status/status_", userStatus.ToString(), ".gif"));
									writer.WriteEndElement(); // img
									writer.WriteEndElement(); // a
								}
							}
							else if (isExternal)
							{
								writer.WriteStartElement("span");
								writer.WriteStartElement("img");
								writer.WriteAttributeString("alt", string.Empty);
								writer.WriteAttributeString("src", string.Concat(path, "/Layouts/Images/status/status_external.gif"));
								writer.WriteAttributeString("title", LocRM.GetString("ExternalUser"));
								writer.WriteEndElement(); // img
								writer.WriteEndElement(); // span
							}
							else if (isPending)
							{
								writer.WriteStartElement("span");
								writer.WriteStartElement("img");
								writer.WriteAttributeString("alt", string.Empty);
								writer.WriteAttributeString("src", string.Concat(path, "/Layouts/Images/status/status_pending.gif"));
								writer.WriteAttributeString("title", LocRM.GetString("PendingUser"));
								writer.WriteEndElement(); // img
								writer.WriteEndElement(); // span
							}
							writer.WriteString(" ");
						}

						writer.WriteStartElement("a");
						writer.WriteAttributeString("href", string.Concat("mailto:", (string)reader["Email"]));
						writer.WriteAttributeString("title", LocRM.GetString("SendEmail"));
						writer.WriteStartElement("img");
						writer.WriteAttributeString("alt", string.Empty);
						writer.WriteAttributeString("src", string.Concat(path, "/Layouts/Images/Mail.gif"));
						writer.WriteEndElement(); // img
						writer.WriteEndElement(); // a
						writer.WriteString(" ");

						if (!isCurrentUserExternal)
						{
							writer.WriteStartElement("a");
							writer.WriteAttributeString("href", string.Concat(path, "/Directory/UserView.aspx?UserID=", userId.ToString(CultureInfo.InvariantCulture)));
							writer.WriteAttributeString("title", LocRM.GetString("ViewUserProfile"));
						}

						writer.WriteStartElement("span");
						if(!string.IsNullOrEmpty(color))
							writer.WriteAttributeString("style", "color: " + color);
						else if (!(bool)reader["IsActive"])
							writer.WriteAttributeString("class", "user-disabled");
						writer.WriteString(userName);
						writer.WriteEndElement(); // span

						if (!isCurrentUserExternal)
							writer.WriteEndElement(); // a
					}
					else // Group
					{
						result = GetGroupLink(userId);
						if (string.IsNullOrEmpty(result))
						{
							writer.WriteStartElement("span");
							writer.WriteAttributeString("class", "user-unknown");
							writer.WriteString(Mediachase.Ibn.Web.UI.CHelper.GetUserName(userId));
							writer.WriteEndElement(); // span
						}
					}
				}
			}

			if (string.IsNullOrEmpty(result))
				result = builder.ToString();

			return result;
		}
		#endregion

		#region public static string GetUserStatusUL(int userId)
		public static string GetUserStatusUL(int userId)
		{
			string result = string.Empty;

			StringBuilder builder = new StringBuilder();
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.ConformanceLevel = ConformanceLevel.Fragment;

			using (XmlWriter writer = XmlWriter.Create(builder, settings))
			{
				ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(CommonHelper).Assembly);

				string path = HttpRuntime.AppDomainAppVirtualPath;
				if (path == "/")
					path = "";

				using (IDataReader reader = User.GetUserInfo(userId, false))
				{
					if (reader.Read())
					{
						string userName = (string)reader["LastName"] + " " + (string)reader["FirstName"];

						bool isExternal = (bool)reader["IsExternal"];
						bool isPending = (bool)reader["IsPending"];

						if (Mediachase.IBN.Business.PortalConfig.UseIM)
						{
							if (!isExternal && !isPending)
							{
								if (reader["OriginalId"] != DBNull.Value)
								{
									writer.WriteStartElement("a");
									writer.WriteAttributeString("href", string.Concat("ibnto:", (string)reader["Login"], "@", Mediachase.IBN.Business.Configuration.Domain));
									writer.WriteAttributeString("title", LocRM.GetString("SendIBNMessage"));
									writer.WriteStartElement("img");
									writer.WriteAttributeString("alt", string.Empty);
									writer.WriteAttributeString("src", string.Concat(path, "/WebServices/UserStatusImage.aspx?UserID=", reader["OriginalId"].ToString()));
									writer.WriteEndElement(); // img
									writer.WriteEndElement(); // a
								}
							}
							else if (isExternal)
							{
								writer.WriteStartElement("span");
								writer.WriteStartElement("img");
								writer.WriteAttributeString("alt", string.Empty);
								writer.WriteAttributeString("src", string.Concat(path, "/Layouts/Images/status/status_external.gif"));
								writer.WriteAttributeString("title", LocRM.GetString("ExternalUser"));
								writer.WriteEndElement(); // img
								writer.WriteEndElement(); // span
							}
							else if (isPending)
							{
								writer.WriteStartElement("span");
								writer.WriteStartElement("img");
								writer.WriteAttributeString("alt", string.Empty);
								writer.WriteAttributeString("src", string.Concat(path, "/Layouts/Images/status/status_pending.gif"));
								writer.WriteAttributeString("title", LocRM.GetString("PendingUser"));
								writer.WriteEndElement(); // img
								writer.WriteEndElement(); // span
							}
							writer.WriteString(" ");
						}

						writer.WriteStartElement("a");
						writer.WriteAttributeString("href", string.Concat("mailto:", (string)reader["Email"]));
						writer.WriteAttributeString("title", LocRM.GetString("SendEmail"));
						writer.WriteStartElement("img");
						writer.WriteAttributeString("alt", string.Empty);
						writer.WriteAttributeString("src", string.Concat(path, "/Layouts/Images/Mail.gif"));
						writer.WriteEndElement(); // img
						writer.WriteEndElement(); // a
						writer.WriteString(" ");

						writer.WriteStartElement("span");
						if (!(bool)reader["IsActive"])
							writer.WriteAttributeString("class", "user-disabled");
						writer.WriteString(userName);
						writer.WriteEndElement(); // span
					}
					else //Group
					{
						result = GetGroupLinkUL(userId);
						if (string.IsNullOrEmpty(result))
						{
							writer.WriteStartElement("span");
							writer.WriteAttributeString("class", "user-unknown");
							writer.WriteString(Mediachase.Ibn.Web.UI.CHelper.GetUserName(userId));
							writer.WriteEndElement(); // span
						}
					}
				}
			}

			if (string.IsNullOrEmpty(result))
				result = builder.ToString();

			return result;
		}
		#endregion


		#region GetLocaleUIName
		public static string GetLocaleUIName()
		{
			return System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
		}
		#endregion

		#region GetMetaField
		public static string GetMetaField(int FieldId, string link)
		{
			string ret = string.Empty;
			ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
			string fNameSpace = MetaDataWrapper.GetMetaFieldNameSpace(FieldId);
			string img = "UserField.gif";
			string _alt = LocRM.GetString("tUserField");
			if (fNameSpace.StartsWith(MetaDataWrapper.SystemRoot))
			{
				img = "SystemField.gif";
				_alt = LocRM.GetString("tSystemField");
			}
			else if (fNameSpace.StartsWith(MetaDataWrapper.UserRoot))
			{
				img = "AdminField.gif";
				_alt = LocRM.GetString("tGlobalField");
			}
			string title = MetaDataWrapper.GetMetaFieldFriendlyName(FieldId);
			ret = String.Format(CultureInfo.InvariantCulture,
				@"<a href='{0}' title='{1}'><img alt='' src='{2}'/> {3}</a>", 
				link,
				_alt,
				GetAbsolutePath("/Layouts/Images/icons/" + img),
				title);
			return ret;
		}
		#endregion

		#region GetMetaFieldProperty
		public static string GetMetaFieldProperty(Page page, int FieldId)
		{
			if (page == null)
				throw new ArgumentNullException("page");

			string ret = string.Empty;
			ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
			string fNameSpace = MetaDataWrapper.GetMetaFieldNameSpace(FieldId);
			string img = "UserField.gif";
			string _alt = LocRM.GetString("tUserField");
			if (fNameSpace.StartsWith(MetaDataWrapper.SystemRoot))
			{
				img = "SystemField.gif";
				_alt = LocRM.GetString("tSystemField");
			}
			else if (fNameSpace.StartsWith(MetaDataWrapper.UserRoot))
			{
				img = "AdminField.gif";
				_alt = LocRM.GetString("tGlobalField");
			}
			string title = MetaDataWrapper.GetMetaFieldFriendlyName(FieldId);
			ret = String.Format(CultureInfo.InvariantCulture,
				@"<img src='{0}' alt='{1}' title='{1}'/> {2}",
				page.ResolveUrl("~/Layouts/Images/icons/" + img), 
				_alt,
				title);
			return ret;
		}
		#endregion

		#region SafeSelect
		public static void SafeSelect(ListControl ddl, string val)
		{
			ListItem li = ddl.Items.FindByValue(val);
			if (li != null)
			{
				ddl.ClearSelection();
				li.Selected = true;
			}
		}
		#endregion

		#region SafeMultipleSelect
		public static void SafeMultipleSelect(ListControl ddl, string val)
		{
			ListItem li = ddl.Items.FindByValue(val);
			if (li != null)
				li.Selected = true;
		}
		#endregion

		#region ExportExcel
		public static void ExportExcel(Control ctrl, string filename, HttpResponse _Response)
		{
			ExportExcel(ctrl, filename, _Response, "\\@");
		}

		public static void ExportExcel(Control ctrl, string filename, HttpResponse _Response, string numberFormat)
		{
			HttpResponse Response = HttpContext.Current.Response;
			Response.Clear();
			Response.Charset = "utf-8";

			Response.ContentType = "application/vnd.ms-excel";
			Response.AddHeader("content-disposition", String.Format("attachment; filename=\"{0}\"", filename));
			System.IO.StringWriter stringWrite = new System.IO.StringWriter();
			System.Web.UI.HtmlTextWriter htmlWrite = new System.Web.UI.HtmlTextWriter(stringWrite);
			ctrl.RenderControl(htmlWrite);
			//Incident (#8069) (SUPPORT-008069-42308)
			//Sometimes not all data load to output stream
			Thread.Sleep(500);
			htmlWrite.Flush();

			string Header = "<html xmlns:x=\"urn:schemas-microsoft-com:office:excel\"><head><meta http-equiv=\"Content-Type\" content=\"application/octet-stream; charset=utf-8\"></head><body><style type='text/css'>.TdTextClass{mso-number-format:" + numberFormat + ";} .SectionTable td{mso-number-format:" + numberFormat + ";}</style>";
			string Footer = "</body></html>";
			Response.Write(String.Concat(Header, stringWrite.ToString(), Footer));
			Response.End();
		}
		#endregion

		#region SaveXML(DataTable dt, string filename)
		public static void SaveXML(DataTable dt, string filename)
		{
			if (dt != null)
			{
				HttpResponse Response = HttpContext.Current.Response;
				Response.Clear();
				Response.ContentType = "text/xml";
				Response.AddHeader("content-disposition", String.Format("attachment; filename={0}", filename));

				DataSet ds = null;
				if (dt.DataSet != null)
					ds = dt.DataSet;
				else
				{
					ds = new DataSet();
					ds.Tables.Add(dt);
				}

				StringWriter s = new StringWriter();
				ds.WriteXml(s, XmlWriteMode.WriteSchema);
				Response.Write(s.ToString());
				Response.End();
			}
		}
		#endregion

		#region EmailLink(string email)
		public static string EmailLink(string email)
		{
			return String.Format("<a href='mailto:{0}'>{0}</a>", email);
		}

		public static string EmailLink(string email, string text)
		{
			return String.Format("<a href='mailto:{0}'>{1}</a>", email, text);
		}
		#endregion

		#region GetEmailLink
		public static string GetEmailLink(string sEmail, string sText)
		{
			string RetVal = "";

			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(CommonHelper).Assembly);

			RetVal = String.Format(
				"<a href='mailto:{0}' title='{3}'><img alt='' src='{2}'/></a> {1}",
				sEmail, sText, GetAbsolutePath("/Layouts/Images/mail.GIF"),
				LocRM.GetString("SendEmail"));

			return RetVal;
		}
		#endregion

		#region Link(string link)
		public static string Link(string link)
		{
			return String.Format("<a href='{0}'>{0}</a>", link);
		}

		public static string Link(string link, string text)
		{
			return String.Format("<a href='{0}'>{1}</a>", link, text);
		}

		public static string LinkToBlank(string link)
		{
			return String.Format("<a target='_blank' href='{0}'>{0}</a>", link);
		}

		public static string LinkToBlank(string link, string text)
		{
			return String.Format("<a target='_blank' href='{0}'>{1}</a>", link, text);
		}
		#endregion

		#region GetStringInterval(TimeSpan ts)
		public static string GetStringInterval(TimeSpan ts)
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(CommonHelper).Assembly);
			StringBuilder sb = new StringBuilder();

			if (ts.Days > 0)
				sb.Append(ts.Days + " " + LocRM.GetString("Days") + " ");
			if (ts.Days > 0 || ts.Hours > 0)
				sb.Append(ts.Hours + " " + LocRM.GetString("Hours") + " ");
			sb.Append(" " + ts.Minutes + " " + LocRM.GetString("Minutes"));
			return sb.ToString();
		}
		#endregion

		#region GetStringTimeZoneOffset
		public static string GetStringTimeZoneOffset(int TimeZoneId)
		{
			TimeSpan Offset = new TimeSpan(0, -User.GetTimeZoneBias(TimeZoneId), 0);
			if (Offset.TotalMinutes == 0)
				return "GMT";
			string str = "GMT ";
			str += (Offset.TotalMinutes > 0) ? "+" : "-";
			if (Math.Abs(Offset.Hours) < 10)
				str += "0";
			str += Math.Abs(Offset.Hours).ToString() + ":";
			if (Math.Abs(Offset.Minutes) < 10)
				str += "0";
			str += Math.Abs(Offset.Minutes).ToString();
			return str;
		}
		#endregion

		#region parsetext(string text, bool allow)
		public static string parsetext(string text, bool allow)
		{
			return parsetext(text, allow, false);
		}

		public static string parsetext(string text, bool allow, bool parseLong)
		{
			StringBuilder sb = new StringBuilder(text);
			sb.Replace("  ", " &nbsp;");
			if (!allow)
			{
				sb.Replace("<", "&lt;");
				sb.Replace(">", "&gt;");
				sb.Replace("\"", "&quot;");
			}
			StringReader sr = new StringReader(sb.ToString());
			StringWriter sw = new StringWriter();
			while (sr.Peek() > -1)
			{
				string temp = sr.ReadLine();
				if (parseLong && temp.IndexOf(" ") < 0 && temp.IndexOf("?") < 0 && temp.Length > 70)
				{
					string sAllow = "";
					while (temp.Length > 70)
					{
						sAllow += temp.Substring(0, 70) + " ";
						temp = temp.Substring(70);
					}
					sAllow += temp;
					temp = sAllow;
				}
				sw.Write(temp);
				if (!temp.EndsWith(">"))
					sw.Write("<br />");
			}
			string retval = sw.GetStringBuilder().ToString();
			if (retval.EndsWith("<br />"))
				retval = retval.Substring(0, retval.Length - 6);
			return retval;
		}

		public static string parsetext_br(string text, bool allow)
		{
			StringBuilder sb = new StringBuilder(text);
			sb.Replace("  ", " &nbsp;");
			if (allow)
			{
				sb.Replace("<", "&lt;");
				sb.Replace(">", "&gt;");
				sb.Replace("\"", "&quot;");
			}
			StringReader sr = new StringReader(sb.ToString());
			StringWriter sw = new StringWriter();
			while (sr.Peek() > -1)
			{
				string temp = sr.ReadLine();
				sw.Write(temp + "<br />");
			}
			return sw.GetStringBuilder().ToString();
		}

		public static string parsetext_br(string text)
		{
			return parsetext_br(text, true);
		}
		#endregion

		#region ParseText
		public static string ParseText(string text)
		{
			return ParseText(text, false, false, false);
		}

		public static string ParseText(string text, bool preserveWhiteSpace, bool preserveLineBreaks, bool preserveHtmlTags)
		{
			StringBuilder sb = new StringBuilder(text);
			if (preserveWhiteSpace)
				sb.Replace("  ", " &nbsp;");
			if (!preserveHtmlTags)
			{
				sb.Replace("<", "&lt;");
				sb.Replace(">", "&gt;");
				sb.Replace("\"", "&quot;");
			}
			string resultString = sb.ToString();
			if (preserveLineBreaks)
			{
				StringReader sr = new StringReader(resultString);
				StringWriter sw = new StringWriter();
				while (sr.Peek() > -1)
				{
					string temp = sr.ReadLine();
					sw.Write(temp + "<br />");
				}
				resultString = sw.GetStringBuilder().ToString();
			}
			return resultString;
		}
		#endregion

		#region ValidateXMLWithMsProjectSchema
		static public bool ValidateXMLWithMsProjectSchema(XmlReader xmlReader)
		{
			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(xmlReader);

				XmlNamespaceManager mngr = new XmlNamespaceManager(doc.NameTable);
				mngr.AddNamespace("ns", "http://schemas.microsoft.com/project");

				if (doc.DocumentElement.Name != "Project")
				{
					throw new ArgumentException();
				}
			}
			catch (XmlException)
			{
				// Incorect Document [5/7/2004]
				return false;
			}
			catch (XmlSchemaException ex)
			{
				System.Diagnostics.Trace.WriteLine(ex);
				//Returns detailed information about the schema exception.
				return false;
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}
		#endregion

		#region GetHours
		public static string GetHours(int iTotalMinutes)
		{
			int iMinutes = 0;
			int iHours = Math.DivRem(iTotalMinutes, 60, out iMinutes);
			return String.Format("{0}:{1}{2}", iHours, (iMinutes < 10) ? "0" : "", iMinutes);
		}
		#endregion

		#region GetMinutes
		public static int GetMinutes(string Hours)
		{
			if (Hours == "")
				Hours = "0:00";
			string[] parts = Hours.Split(':');
			int Minutes = 0;
			Minutes = int.Parse(parts[0]) * 60;
			if (parts.Length > 1)
				Minutes += int.Parse(parts[1]);
			return Minutes;
		}
		#endregion

		#region BindMetaTypesItemCollections
		public static void BindMetaTypesItemCollections(ListItemCollection items, bool IsAllItems)
		{
			ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Lists.Resources.strLists", typeof(CommonHelper).Assembly);
			items.Clear();
			if (IsAllItems)
				items.Add(new ListItem(LocRM2.GetString("tAllTypes"), "-1"));
			MetaTypeCollection coll = MetaType.GetMetaDataMetaTypes();
			Hashtable htTypes = new Hashtable();
			htTypes.Clear();
			foreach (MetaType type in coll)
			{
				if (!(type.MetaDataType == MetaDataType.DictionaryMultivalue
					|| type.MetaDataType == MetaDataType.DictionarySingleValue
					|| type.MetaDataType == MetaDataType.EnumMultivalue
					|| type.MetaDataType == MetaDataType.EnumSingleValue
					|| type.MetaDataType == MetaDataType.StringDictionary
					|| type.MetaDataType == MetaDataType.MetaObject))
					htTypes.Add(type.Id.ToString(), type.FriendlyName);
			}
			items.Add(new ListItem(htTypes["31"].ToString(), "31"));
			htTypes.Remove("31");
			items.Add(new ListItem(htTypes["32"].ToString(), "32"));
			htTypes.Remove("32");
			items.Add(new ListItem(htTypes["26"].ToString(), "26"));
			htTypes.Remove("26");
			items.Add(new ListItem(MetaType.Load(MetaDataType.Money).FriendlyName, "9"));
			items.Add(new ListItem(MetaType.Load(MetaDataType.Float).FriendlyName, "6"));
			items.Add(new ListItem(htTypes["28"].ToString(), "28"));
			htTypes.Remove("28");
			items.Add(new ListItem(MetaType.Load(MetaDataType.DateTime).FriendlyName, "4"));

			items.Add(new ListItem(LocRM.GetString("Dictionary"), "0"));

			items.Add(new ListItem(htTypes["27"].ToString(), "27"));
			htTypes.Remove("27");
			items.Add(new ListItem(htTypes["30"].ToString(), "30"));
			htTypes.Remove("30");
			items.Add(new ListItem(htTypes["33"].ToString(), "33"));
			htTypes.Remove("33");
			items.Add(new ListItem(htTypes["29"].ToString(), "29"));
			htTypes.Remove("29");
			items.Add(new ListItem(htTypes["39"].ToString(), "39"));
			htTypes.Remove("39");
			items.Add(new ListItem(htTypes["40"].ToString(), "40"));
			htTypes.Remove("40");

			foreach (string s in htTypes.Keys)
				items.Add(new ListItem(htTypes[s].ToString(), s));
		}
		#endregion

		#region GetAbsolutePath
		private static string GetAbsolutePath(string xs_Path)
		{
			string UrlScheme = System.Configuration.ConfigurationManager.AppSettings["UrlScheme"];

			StringBuilder builder = new StringBuilder();
			if (UrlScheme != null)
				builder.Append(UrlScheme);
			else
				builder.Append(HttpContext.Current.Request.Url.Scheme);
			builder.Append("://");

			// Oleg Rylin: Fixing the problem with non-default port [6/20/2006]
			builder.Append(HttpContext.Current.Request.Url.Authority);

			builder.Append(HttpContext.Current.Request.ApplicationPath);
			if(!builder.ToString().EndsWith("/"))
			{
				builder.Append("/");
			}

			if (xs_Path != string.Empty)
			{
				if (xs_Path[0] == '/')
					xs_Path = xs_Path.Substring(1, xs_Path.Length - 1);
				builder.Append(xs_Path);
			}
			return builder.ToString().Replace("\\\\", "\\");
			 

			// [2008-02-28] Oleg Rylin: An attempt of refusal from full path 
			//return string.Format(CultureInfo.InvariantCulture,
			//    "{0}{1}{2}", 
			//    HttpContext.Current.Request.ApplicationPath, 
			//    (xs_Path[0] == '/') ? "" : "/",
			//    xs_Path);
		}
		#endregion

		#region GetRequestInteger
		public static int GetRequestInteger(HttpRequest request, string varName, int defaultValue)
		{
			int ret = defaultValue;
			string val = request[varName];
			if (val != null && val.Length > 0)
			{
				try
				{
					ret = int.Parse(val);
				}
				catch { }
			}
			return ret;
		}
		#endregion

		#region ReloadTopFrame
		public static void ReloadTopFrame(string CheckStr, string RightFrameUrl, HttpResponse Response)
		{
			if (!Security.CurrentUser.IsExternal)
			{
				CheckStr = CheckStr.ToLower();

				Response.Clear();
				Response.Write(
				  "<script language=JavaScript>" +
				  "var oFSRight = window.top.document.getElementById(\"fs3\");" +
				  "if (oFSRight && oFSRight.rows && oFSRight.rows.substr(0, 1) != \"0\")" +
				  "{" +
				  "	var oTopRight = window.top.document.getElementById(\"topright\");" +
				  "	if (oTopRight && oTopRight.src && oTopRight.src.toLowerCase().indexOf(\"" + CheckStr + "\") > 0)" +
				  "	{" +
				  "		var s = oTopRight.src;" +
				  "		oTopRight.src = \"\";" +
				  "		oTopRight.src = s;" +
				  "	}" +
				  "}" +
				  "var oRight = window.top.right;" +
				  "if (oRight && oRight.location)" +
				  "	oRight.location.href = \"" + RightFrameUrl + "\"" +
				  "</script>"
				  );
				Response.End();
			}
			else
			{
				Response.Redirect(RightFrameUrl);
			}
		}
		#endregion

		#region ReloadOpenerFrame
		public static void ReloadOpenerFrame(string CheckStr, string RightFrameUrl, HttpResponse Response)
		{
			CheckStr = CheckStr.ToLower();

			Response.Clear();
			Response.Write(
				"<script language=JavaScript>" +
				"var oFSRight = window.opener.top.document.getElementById(\"fs3\");" +
				"if (oFSRight && oFSRight.rows && oFSRight.rows.substr(0, 1) != \"0\")" +
				"{" +
				"	var oTopRight = window.opener.top.document.getElementById(\"topright\");" +
				"	if (oTopRight && oTopRight.src && oTopRight.src.toLowerCase().indexOf(\"" + CheckStr + "\") > 0)" +
				"	{" +
				"		var s = oTopRight.src;" +
				"		oTopRight.src = \"\";" +
				"		oTopRight.src = s;" +
				"	}" +
				"}" +
				"var oRight = window.opener.top.right;" +
				"if (oRight && oRight.location)" +
				"	oRight.location.href = \"" + RightFrameUrl + "\";" +
				"window.close();" +
				"</script>"
				);
			Response.End();
		}
		#endregion

		#region GetObjectTypeName
		public static string GetObjectTypeName(int objectTypeId, string className)
		{
			string retval = "";
			if (objectTypeId > 0)
				retval = GetObjectTypeName(objectTypeId);
			else if (className.ToLower() == OrganizationEntity.GetAssignedMetaClassName().ToLower())
				retval = HttpContext.GetGlobalResourceObject("IbnFramework.Client", "Organization").ToString();
			else if (className.ToLower() == ContactEntity.GetAssignedMetaClassName().ToLower())
				retval = HttpContext.GetGlobalResourceObject("IbnFramework.Client", "Contact").ToString();
			else
			{
				Mediachase.Ibn.Data.Meta.Management.MetaClass mc = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(className);
				retval = GetResFileString(mc.FriendlyName);
			}

			return retval;
		}

		public static string GetObjectTypeName(int ObjectTypeId)
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strNotExistingId", typeof(CommonHelper).Assembly);

			string retval = "";
			switch (ObjectTypeId)
			{
				case (int)ObjectTypes.User:
					retval = LocRM.GetString("User");
					break;
				case (int)ObjectTypes.Project:
					retval = LocRM.GetString("Project");
					break;
				case (int)ObjectTypes.Task:
					retval = LocRM.GetString("Task");
					break;
				case (int)ObjectTypes.ToDo:
					retval = LocRM.GetString("ToDo");
					break;
				case (int)ObjectTypes.CalendarEntry:
					retval = LocRM.GetString("Event");
					break;
				case (int)ObjectTypes.Issue:
					retval = LocRM.GetString("Incident");
					break;
				case (int)ObjectTypes.Document:
					retval = LocRM.GetString("Document");
					break;
				case (int)ObjectTypes.List:
					retval = LocRM.GetString("List");
					break;
				case (int)ObjectTypes.IssueRequest:
					retval = LocRM.GetString("IssueRequest");
					break;
				case (int)ObjectTypes.KnowledgeBase:
					retval = LocRM.GetString("Article");
					break;
				case (int)ObjectTypes.Assignment:
					retval = LocRM.GetString("Assignment");
					break;
			}

			if (retval.Length > 0)
				retval = retval.Substring(0, 1).ToUpper() + retval.Substring(1);

			return retval;
		}
		#endregion

		#region public static string GetHtmlLink(string link, string text)
		public static string GetHtmlLink(string link, string text)
		{
			string retVal = string.Empty;

			if (!string.IsNullOrEmpty(link) && !string.IsNullOrEmpty(text))
			{
				retVal = string.Format("<a href=\"{0}\">{1}</a>", HttpUtility.HtmlEncode(link), HttpUtility.HtmlEncode(text));
			}

			return retVal;
		}
		#endregion

		#region GetObjectLink
		public static string GetObjectLink(int objectTypeId, int objectId)
		{
			string linkFormat;
			switch (objectTypeId)
			{
				case (int)ObjectTypes.CalendarEntry:
					linkFormat = "/Events/EventView.aspx?EventId={0}";
					break;
				case (int)ObjectTypes.Document:
					linkFormat = "/Documents/DocumentView.aspx?DocumentId={0}";
					break;
				case (int)ObjectTypes.Issue:
					linkFormat = "/Incidents/IncidentView.aspx?IncidentId={0}";
					break;
				case (int)ObjectTypes.IssueRequest:
					linkFormat = "/Incidents/MailRequestView.aspx?RequestId={0}";
					break;
				case (int)ObjectTypes.KnowledgeBase:
					linkFormat = "/Incidents/ArticleView.aspx?ArticleId={0}";
					break;
				case (int)ObjectTypes.List:
					linkFormat = "/Apps/MetaUIEntity/Pages/EntityList.aspx?ClassName=List_{0}";
					break;
				case (int)ObjectTypes.Project:
					linkFormat = "/Projects/ProjectView.aspx?ProjectId={0}";
					break;
				case (int)ObjectTypes.Task:
					linkFormat = "/Tasks/TaskView.aspx?TaskId={0}";
					break;
				case (int)ObjectTypes.ToDo:
					linkFormat = "/ToDo/ToDoView.aspx?ToDoId={0}";
					break;
				case (int)ObjectTypes.User:
					linkFormat = "/Directory/UserView.aspx?UserId={0}";
					break;
				default:
					linkFormat = null;
					break;
			}

			string retVal = string.Empty;

			if (!string.IsNullOrEmpty(linkFormat))
			{
				retVal = string.Format(GetAbsolutePath(linkFormat), objectId);
			}

			return retVal;
		}

		public static string GetObjectLink(int objectTypeId, int objectId, Page page)
		{
			string retVal = string.Empty;
			switch (objectTypeId)
			{
				case (int)ObjectTypes.CalendarEntry:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?EventId={1}",
						page.ResolveClientUrl("~/Events/EventView.aspx"),
						objectId);
					break;
				case (int)ObjectTypes.Document:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?DocumentId={1}",
						page.ResolveClientUrl("~/Documents/DocumentView.aspx"),
						objectId);
					break;
				case (int)ObjectTypes.Issue:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?IncidentId={1}",
						page.ResolveClientUrl("~/Incidents/IncidentView.aspx"),
						objectId);
					break;
				case (int)ObjectTypes.IssueRequest:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?RequestId={1}",
						page.ResolveClientUrl("~/Incidents/MailRequestView.aspx"),
						objectId);
					break;
				case (int)ObjectTypes.KnowledgeBase:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?ArticleId={1}",
						page.ResolveClientUrl("~/Incidents/ArticleView.aspx"),
						objectId);
					break;
				case (int)ObjectTypes.List:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?ClassName=List_{1}",
						page.ResolveClientUrl("~/Apps/MetaUIEntity/Pages/EntityList.aspx"),
						objectId);
					break;
				case (int)ObjectTypes.Project:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?ProjectId={1}",
						page.ResolveClientUrl("~/Projects/ProjectView.aspx"),
						objectId);
					break;
				case (int)ObjectTypes.Task:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?TaskId={1}",
						page.ResolveClientUrl("~/Tasks/TaskView.aspx"),
						objectId);
					break;
				case (int)ObjectTypes.ToDo:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?ToDoId={1}",
						page.ResolveClientUrl("~/ToDo/ToDoView.aspx"),
						objectId);
					break;
				case (int)ObjectTypes.User:
					retVal = String.Format(
						CultureInfo.InvariantCulture,
						"{0}?UserId={1}",
						page.ResolveClientUrl("~/Directory/UserView.aspx"),
						objectId);
					break;
			}
			return retVal;
		}
		#endregion
		#region GetObjectTitle
		public static string GetObjectTitle(int objectTypeId, int objectId)
		{
			string retVal;
			switch (objectTypeId)
			{
				case (int)ObjectTypes.CalendarEntry:
					retVal = CalendarEntry.GetEventTitle(objectId);
					break;
				case (int)ObjectTypes.Document:
					retVal = Document.GetTitle(objectId);
					break;
				case (int)ObjectTypes.Issue:
					retVal = Incident.GetIncidentTitle(objectId);
					break;
				case (int)ObjectTypes.IssueBox:
					retVal = Mediachase.IBN.Business.EMail.IncidentBox.Load(objectId).Name;
					break;
				case (int)ObjectTypes.IssueRequest:
					retVal = IssueRequest.GetSubject(objectId);
					break;
				case (int)ObjectTypes.KnowledgeBase:
					retVal = Mediachase.IBN.Business.Common.GetArticleTitle(objectId);
					break;
				case (int)ObjectTypes.List:
					retVal = ListManager.GetListTitle(objectId);
					break;
				case (int)ObjectTypes.Project:
					retVal = Project.GetProjectTitle(objectId);
					break;
				case (int)ObjectTypes.ProjectGroup:
					retVal = ProjectGroup.GetProjectGroupTitle(objectId);
					break;
				case (int)ObjectTypes.Task:
					retVal = Task.GetTaskTitle(objectId);
					break;
				case (int)ObjectTypes.ToDo:
					retVal = Mediachase.IBN.Business.ToDo.GetToDoTitle(objectId);
					break;
				case (int)ObjectTypes.User:
					retVal = User.GetUserName(objectId);
					break;
				case (int)ObjectTypes.UserGroup:
					retVal = GetResFileString(SecureGroup.GetGroupName(objectId));
					break;
				default:
					retVal = string.Empty;
					break;
			}

			return retVal;
		}
		#endregion
		#region GetObjectLinkAndTitle
		public static string GetObjectLinkAndTitle(int objectTypeId, int objectId)
		{
			string link = GetObjectLink(objectTypeId, objectId);
			string title = GetObjectTitle(objectTypeId, objectId);

			return GetHtmlLink(link, title);
		}

		public static string GetObjectLinkAndTitle(int objectTypeId, int objectId, Page page)
		{
			string link = GetObjectLink(objectTypeId, objectId, page);
			string title = GetObjectTitle(objectTypeId, objectId);

			return GetHtmlLink(link, title);
		}
		#endregion
		#region GetObjectHTMLTitle
		public static string GetObjectHTMLTitle(int ObjectTypeId, int ObjectId)
		{
			string retval = "";
			switch (ObjectTypeId)
			{
				case (int)ObjectTypes.User:
					retval = GetUserStatusUL(ObjectId);
					break;
				case (int)ObjectTypes.UserGroup:
					retval = GetGroupLinkUL(ObjectId);
					break;
				case (int)ObjectTypes.Project:
					retval = GetProjectStatusWL(ObjectId);
					break;
				case (int)ObjectTypes.Task:
					retval = Task.GetTaskTitle(ObjectId);
					break;
				case (int)ObjectTypes.ToDo:
					retval = Mediachase.IBN.Business.ToDo.GetToDoTitle(ObjectId);
					break;
				case (int)ObjectTypes.CalendarEntry:
					retval = CalendarEntry.GetEventTitle(ObjectId);
					break;
				case (int)ObjectTypes.Issue:
					retval = GetIncidentTitleWL(ObjectId);
					break;
				case (int)ObjectTypes.IssueRequest:
					retval = IssueRequest.GetSubject(ObjectId);
					break;
				case (int)ObjectTypes.ProjectGroup:
					retval = ProjectGroup.GetProjectGroupTitle(ObjectId);
					break;
				case (int)ObjectTypes.Document:
					retval = Document.GetTitle(ObjectId);
					break;
				case (int)ObjectTypes.List:
					retval = ListManager.GetListTitle(ObjectId);
					break;
				case (int)ObjectTypes.IssueBox:
					retval = Mediachase.IBN.Business.EMail.IncidentBox.Load(ObjectId).Name;
					break;
				case (int)ObjectTypes.KnowledgeBase:
					retval = Mediachase.IBN.Business.Common.GetArticleTitle(ObjectId);
					break;
			}
			return retval;
		}
		#endregion

		#region GetEntityTitle
		public static string GetEntityTitle(int objectTypeId, PrimaryKeyId objectId)
		{
			string retVal = String.Empty;
			EntityObject entity;
			try
			{
				switch (objectTypeId)
				{
					case (int)ObjectTypes.Contact:
						entity = BusinessManager.Load(ContactEntity.GetAssignedMetaClassName(), objectId);
						if (entity != null)
							retVal = ((ContactEntity)entity).FullName;
						break;
					case (int)ObjectTypes.Organization:
						entity = BusinessManager.Load(OrganizationEntity.GetAssignedMetaClassName(), objectId);
						if (entity != null)
							retVal = ((OrganizationEntity)entity).Name;
						break;
					case (int)ObjectTypes.Document:
						entity = BusinessManager.Load(DocumentEntity.GetAssignedMetaClassName(), objectId);
						if (entity != null)
							retVal = ((DocumentEntity)entity).Title;
						break;
				}
			}
			catch { }
			return retVal;
		}
		#endregion

		#region GetEntityTitle
		public static string GetEntityTitle(string className, PrimaryKeyId objectId)
		{
			string retVal = String.Empty;
			EntityObject entity;
			if (className == ContactEntity.GetAssignedMetaClassName())
			{
				return GetEntityTitle((int)ObjectTypes.Contact, objectId);
			}
			else if (className == OrganizationEntity.GetAssignedMetaClassName())
			{
				return GetEntityTitle((int)ObjectTypes.Organization, objectId);
			}
			else if(className == DocumentEntity.GetAssignedMetaClassName())
			{
				return GetEntityTitle((int)ObjectTypes.Document, objectId);
			}
			else
			{
				Mediachase.Ibn.Data.Meta.Management.MetaClass mc = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(className);
				entity = BusinessManager.Load(className, objectId);
				if (entity != null)
					retVal = CHelper.GetResFileString(entity.Properties[mc.TitleFieldName].Value.ToString());
			}

			return retVal;
		}
		#endregion

		#region GetPriorityIcon
		public static string GetPriorityIcon(int PID, string Name)
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strResAsts", typeof(CommonHelper).Assembly);
			string color = "PriorityNormal.gif";
			if (PID < 100) color = "PriorityLow.gif";
			if (PID > 500 && PID < 800) color = "PriorityHigh.gif";
			if (PID >= 800 && PID < 1000) color = "PriorityVeryHigh.gif";
			if (PID >= 1000) color = "PriorityUrgent.gif";
			Name = LocRM.GetString("Priority") + Name;
			return String.Format(CultureInfo.InvariantCulture,
				"<img src='{0}' alt='{1}' title='{1}'/>",
				GetAbsolutePath("/Layouts/Images/icons/" + color),
				Name);
		}

		public static string GetPriorityIcon(int priorityId, Page page)
		{
			string icon;
			string tooltip;
			if (priorityId < 500)
			{
				icon = "PriorityLow.gif";
				tooltip = HttpContext.GetGlobalResourceObject("IbnFramework.Common", "PriorityLow").ToString();
			}
			else if (priorityId < 750)
			{
				icon = "PriorityNormal.gif";
				tooltip = HttpContext.GetGlobalResourceObject("IbnFramework.Common", "PriorityNormal").ToString();
			}
			else if (priorityId < 900)
			{
				icon = "PriorityHigh.gif";
				tooltip = HttpContext.GetGlobalResourceObject("IbnFramework.Common", "PriorityHigh").ToString();
			}
			else if (priorityId < 1000)
			{
				icon = "PriorityVeryHigh.gif";
				tooltip = HttpContext.GetGlobalResourceObject("IbnFramework.Common", "PriorityVeryHigh").ToString();
			}
			else
			{
				icon = "PriorityUrgent.gif";
				tooltip = HttpContext.GetGlobalResourceObject("IbnFramework.Common", "PriorityUrgent").ToString();
			}

			return String.Format(CultureInfo.InvariantCulture,
				"<img src='{0}' alt='{1}' title='{1}'/>",
				page.ResolveClientUrl("~/Layouts/Images/icons/" + icon), 
				tooltip);
		}
		#endregion

		#region GetIntervalString
		public static string GetIntervalString(int IntervalValue)
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strEventEdit", typeof(CommonHelper).Assembly);
			string retVal = LocRM.GetString("tDonotremind");
			switch (IntervalValue)
			{
				case 5:
					retVal = LocRM.GetString("t5Min");
					break;
				case 15:
					retVal = LocRM.GetString("t15Min");
					break;
				case 30:
					retVal = LocRM.GetString("t30Min");
					break;
				case 60:
					retVal = LocRM.GetString("t1Hour");
					break;
				case 240:
					retVal = LocRM.GetString("t4Hour");
					break;
				case 1440:
					retVal = LocRM.GetString("t1Day");
					break;
				case 2880:
					retVal = LocRM.GetString("t2Day");
					break;
				case 4320:
					retVal = LocRM.GetString("t3Day");
					break;
				case 10080:
					retVal = LocRM.GetString("t1Week");
					break;
				case 20160:
					retVal = LocRM.GetString("t2Week");
					break;
			}
			return retVal;
		}
		#endregion

		#region GetDocumentTitle
		public static string GetDocumentTitle(string Title, int DocumentId, bool IsCompleted, int ReasonId)
		{
			string img = "document_active.gif";
			switch (ReasonId)
			{
				case 1:
				case 2:
					img = "document_completed.gif";
					break;
				case 3:
				case 4:
					img = "document_suspensed.gif";
					break;
			}
			if (!Security.CurrentUser.IsExternal)
				return String.Format(CultureInfo.InvariantCulture,
					@"<a href='{0}?DocumentId={1}'><img alt='' src='{2}'/> {3}</a>", 
					GetAbsolutePath("/Documents/DocumentView.aspx"),
					DocumentId,
					GetAbsolutePath("/Layouts/Images/icons/" + img),
					Title);
			else return String.Format(CultureInfo.InvariantCulture,
					@"<img alt='' src='{0}'/> {1}",
					GetAbsolutePath("/Layouts/Images/icons/" + img),
					Title);
		}
		#endregion

		#region GetPriorityColor
		public static Color GetPriorityColor(int PriorityId)
		{
			Color retval = Color.FromArgb(0, 0, 0);

			if (PriorityId == (int)Priority.Low)
				retval = Color.FromArgb(128, 128, 128);
			else if (PriorityId == (int)Priority.Normal)
				retval = Color.Black;
			else if (PriorityId == (int)Priority.High)
				retval = Color.FromArgb(86, 0, 0);
			else if (PriorityId == (int)Priority.VeryHigh)
				retval = Color.FromArgb(170, 0, 0);
			else if (PriorityId == (int)Priority.Urgent)
				retval = Color.FromArgb(255, 0, 0);

			return retval;
		}
		#endregion

		#region GetProjectIdByObjectIdObjectType
		public static int GetProjectIdByObjectIdObjectType(int ObjectId, int ObjectTypeId)
		{
			switch (ObjectTypeId)
			{
				case (int)ObjectTypes.Project: return ObjectId;
				case (int)ObjectTypes.Task: return Task.GetProject(ObjectId);
				case (int)ObjectTypes.ToDo: return Mediachase.IBN.Business.ToDo.GetProject(ObjectId);
				case (int)ObjectTypes.CalendarEntry: return Mediachase.IBN.Business.CalendarEntry.GetProject(ObjectId);
				case (int)ObjectTypes.Issue: return Incident.GetProject(ObjectId);
				case (int)ObjectTypes.Document: return Document.GetProject(ObjectId);
				default: return -1;
			}
		}
		#endregion

		#region GetStateColor
		public static Color GetStateColor(int stateId)
		{
			Color retval = Color.FromArgb(0, 0, 0);

			if (stateId == (int)ObjectStates.Upcoming)
				retval = Color.FromArgb(0, 0, 0);
			else if (stateId == (int)ObjectStates.Active || stateId == (int)ObjectStates.ReOpen)
				retval = Color.FromArgb(0, 128, 0);
			else if (stateId == (int)ObjectStates.Overdue)
				retval = Color.FromArgb(255, 0, 0);
			else if (stateId == (int)ObjectStates.OnCheck)
				retval = Color.FromArgb(128, 0, 0);
			else
				retval = Color.FromArgb(128, 128, 128);

			return retval;
		}
		#endregion

		#region GetStateColorString
		public static string GetStateColorString(int stateId)
		{
			string retval = "#000000";

			if (stateId == (int)ObjectStates.Upcoming)
				retval = "#000000";
			else if (stateId == (int)ObjectStates.Active || stateId == (int)ObjectStates.ReOpen)
				retval = "#008000";
			else if (stateId == (int)ObjectStates.Overdue)
				retval = "#ff0000";
			else if (stateId == (int)ObjectStates.OnCheck)
				retval = "#800000";
			else
				retval = "#808080";

			return retval;
		}
		#endregion

		#region CountOf
		public static int CountOf(string str, string sub)
		{
			int retVal = 0;
			while (str.IndexOf(sub) >= 0)
			{
				retVal++;
				str = str.Substring(str.IndexOf(sub) + sub.Length);
			}
			return retVal;
		}
		#endregion

		#region GetClientLink
		public static string GetClientLink(Page pageInstance, object OrgUid, object ContactUid, object ClientName)
		{
			bool canOrgEdit = true;
			bool canClientEdit = true;

			ContactEntity contactEntity = null;
			if (ContactUid != DBNull.Value && PrimaryKeyId.Parse(ContactUid.ToString()) != PrimaryKeyId.Empty)
			{
				try
				{
					contactEntity = (ContactEntity)BusinessManager.Load(ContactEntity.GetAssignedMetaClassName(), PrimaryKeyId.Parse(ContactUid.ToString()));
				}
				catch
				{
					ResourceManager LocRM1 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(CommonHelper).Assembly);
					return LocRM1.GetString("ClientNotSet");
				}
			}

			if (Security.IsUserInGroup(InternalSecureGroups.Partner))
			{
				canOrgEdit = false;
				canClientEdit = false;
				int groupId = User.GetGroupForPartnerUser(Security.CurrentUser.UserID);
				PrimaryKeyId partnerOrgUid = PrimaryKeyId.Empty;
				PrimaryKeyId partnerContactUid = PrimaryKeyId.Empty;
				using (IDataReader reader = SecureGroup.GetGroup(groupId))
				{
					if (reader.Read())
					{
						if (reader["OrgUid"] != DBNull.Value)
							partnerOrgUid = PrimaryKeyId.Parse(reader["OrgUid"].ToString());

						if (reader["ContactUid"] != DBNull.Value)
							partnerContactUid = PrimaryKeyId.Parse(reader["ContactUid"].ToString());
					}
				}
				if (OrgUid != DBNull.Value && PrimaryKeyId.Parse(OrgUid.ToString()) != PrimaryKeyId.Empty)
				{
					if (partnerOrgUid == PrimaryKeyId.Parse(OrgUid.ToString()))
						canOrgEdit = true;
				}
				if (ContactUid != DBNull.Value && PrimaryKeyId.Parse(ContactUid.ToString()) != PrimaryKeyId.Empty)
				{
					if (partnerContactUid != PrimaryKeyId.Empty 
						&& contactEntity.OrganizationId.HasValue
						&& partnerContactUid == contactEntity.OrganizationId.Value)
					{
						canOrgEdit = true;
						canClientEdit = true;
					}
					if (partnerContactUid != PrimaryKeyId.Empty && partnerContactUid == PrimaryKeyId.Parse(ContactUid.ToString()))
					{
						canOrgEdit = false;
						canClientEdit = true;
					}
				}
			}

			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(CommonHelper).Assembly);
			string retval = "";
			if (OrgUid != DBNull.Value && PrimaryKeyId.Parse(OrgUid.ToString()) != PrimaryKeyId.Empty)
			{
				if (canOrgEdit && !Security.CurrentUser.IsExternal)
					retval = String.Format(CultureInfo.InvariantCulture,
						"<a href='{0}'>{1}</a>", 
						pageInstance.ResolveClientUrl(CHelper.GetLinkEntityView(OrganizationEntity.GetAssignedMetaClassName(), OrgUid.ToString())),
						ClientName.ToString());
				else
					retval = ClientName.ToString();
			}
			else if (ContactUid != DBNull.Value && PrimaryKeyId.Parse(ContactUid.ToString()) != PrimaryKeyId.Empty)
			{
				if (!String.IsNullOrEmpty(contactEntity.Organization))
				{
					if (!Security.CurrentUser.IsExternal)
					{
						if (canOrgEdit && canClientEdit)
							retval = String.Format(CultureInfo.InvariantCulture,
								"<a href='{0}'>{1}</a>&nbsp;&nbsp;(&nbsp;<a href='{2}'>{3}</a>&nbsp;)",
								pageInstance.ResolveClientUrl(CHelper.GetLinkEntityView(ContactEntity.GetAssignedMetaClassName(), ContactUid.ToString())),
								ClientName.ToString(),
								pageInstance.ResolveClientUrl(CHelper.GetLinkEntityView(OrganizationEntity.GetAssignedMetaClassName(), contactEntity.OrganizationId.ToString())),
								contactEntity.Organization);
						else if (canOrgEdit && !canClientEdit)
							retval = String.Format(CultureInfo.InvariantCulture,
								"{0}&nbsp;&nbsp;(&nbsp;<a href='{1}'>{2}</a>&nbsp;)", 
								ClientName.ToString(), 
								pageInstance.ResolveClientUrl(CHelper.GetLinkEntityView(OrganizationEntity.GetAssignedMetaClassName(), contactEntity.OrganizationId.ToString())),
								contactEntity.Organization);
						else if (!canOrgEdit && canClientEdit)
							retval = String.Format(CultureInfo.InvariantCulture,
								"<a href='{0}'>{1}</a>&nbsp;&nbsp;(&nbsp;{2}&nbsp;)",
								pageInstance.ResolveClientUrl(CHelper.GetLinkEntityView(ContactEntity.GetAssignedMetaClassName(), ContactUid.ToString())),
								ClientName.ToString(),
								contactEntity.Organization);
						else
							retval = String.Format(CultureInfo.InvariantCulture,
								"{0}&nbsp;&nbsp;(&nbsp;{1}&nbsp;)",
								ClientName.ToString(), 
								contactEntity.Organization);
					}
					else
						retval = String.Format(CultureInfo.InvariantCulture,
							"{0}&nbsp;&nbsp;(&nbsp;{1}&nbsp;)",
							ClientName.ToString(),
							contactEntity.Organization);
				}
				else
				{
					if (canClientEdit && !Security.CurrentUser.IsExternal)
						retval = String.Format(CultureInfo.InvariantCulture,
							"<a href='{0}'>{1}</a>",
							pageInstance.ResolveClientUrl(CHelper.GetLinkEntityView(ContactEntity.GetAssignedMetaClassName(), ContactUid.ToString())),
							ClientName.ToString());
					else
						retval = ClientName.ToString();
				}
			}
			else
				retval = LocRM.GetString("ClientNotSet");
			return retval;
		}

		public static string GetClientLink(Page pageInstance, PrimaryKeyId OrgUid, PrimaryKeyId ContactUid, string ClientName)
		{
			bool canOrgEdit = true;
			bool canClientEdit = true;

			ContactEntity contactEntity = null;
			if (ContactUid != PrimaryKeyId.Empty)
			{
				try
				{
					contactEntity = (ContactEntity)BusinessManager.Load(ContactEntity.GetAssignedMetaClassName(), ContactUid);
				}
				catch
				{
					return String.Empty;
				}
			}

			if (Security.IsUserInGroup(InternalSecureGroups.Partner))
			{
				canOrgEdit = false;
				canClientEdit = false;
				int groupId = User.GetGroupForPartnerUser(Security.CurrentUser.UserID);
				PrimaryKeyId partnerOrgUid = PrimaryKeyId.Empty;
				PrimaryKeyId partnerContactUid = PrimaryKeyId.Empty;
				using (IDataReader reader = SecureGroup.GetGroup(groupId))
				{
					if (reader.Read())
					{
						if (reader["OrgUid"] != DBNull.Value)
							partnerOrgUid = PrimaryKeyId.Parse(reader["OrgUid"].ToString());

						if (reader["ContactUid"] != DBNull.Value)
							partnerContactUid = PrimaryKeyId.Parse(reader["ContactUid"].ToString());
					}
				}
				if (OrgUid != PrimaryKeyId.Empty)
				{
					if (partnerOrgUid == OrgUid)
						canOrgEdit = true;
				}
				else if (ContactUid != PrimaryKeyId.Empty)
				{
					if (partnerOrgUid != PrimaryKeyId.Empty 
						&& contactEntity.OrganizationId.HasValue
						&& partnerOrgUid == contactEntity.OrganizationId.Value)
					{
						canOrgEdit = true;
						canClientEdit = true;
					}
					if (partnerContactUid != PrimaryKeyId.Empty && partnerContactUid == ContactUid)
					{
						canOrgEdit = false;
						canClientEdit = true;
					}
				}
			}

			string retval = "";
			if (OrgUid != PrimaryKeyId.Empty)
			{
				if (canOrgEdit && !Security.CurrentUser.IsExternal)
					retval = String.Format(CultureInfo.InvariantCulture,
						"<a href='{0}'>{1}</a>", 
						pageInstance.ResolveClientUrl(CHelper.GetLinkEntityView(OrganizationEntity.GetAssignedMetaClassName(), OrgUid.ToString())), 
						ClientName);
				else
					retval = ClientName;
			}
			else if (ContactUid != PrimaryKeyId.Empty)
			{
				if (!String.IsNullOrEmpty(contactEntity.Organization))
				{
					if (!Security.CurrentUser.IsExternal)
					{
						if (canOrgEdit && canClientEdit)
							retval = String.Format(CultureInfo.InvariantCulture,
								"<a href='{0}'>{1}</a>&nbsp;&nbsp;(&nbsp;<a href='{2}'>{3}</a>&nbsp;)",
								pageInstance.ResolveClientUrl(CHelper.GetLinkEntityView(ContactEntity.GetAssignedMetaClassName(), ContactUid.ToString())), 
								ClientName,
								pageInstance.ResolveClientUrl(CHelper.GetLinkEntityView(OrganizationEntity.GetAssignedMetaClassName(), contactEntity.OrganizationId.ToString())),
								contactEntity.Organization);
						else if (canOrgEdit && !canClientEdit)
							retval = String.Format(CultureInfo.InvariantCulture,
								"{0}&nbsp;&nbsp;(&nbsp;<a href='{1}'>{2}</a>&nbsp;)", 
								ClientName,
								pageInstance.ResolveClientUrl(CHelper.GetLinkEntityView(OrganizationEntity.GetAssignedMetaClassName(), contactEntity.OrganizationId.ToString())),
								contactEntity.Organization);
						else if (!canOrgEdit && canClientEdit)
							retval = String.Format(CultureInfo.InvariantCulture,
								"<a href='{0}'>{1}</a>&nbsp;&nbsp;(&nbsp;{2}&nbsp;)",
								pageInstance.ResolveClientUrl(CHelper.GetLinkEntityView(ContactEntity.GetAssignedMetaClassName(), ContactUid.ToString())), 
								ClientName,
								contactEntity.Organization);
						else
							retval = String.Format(CultureInfo.InvariantCulture,
								"{0}&nbsp;&nbsp;(&nbsp;{1}&nbsp;)", 
								ClientName, 
								contactEntity.Organization);
					}
					else
						retval = String.Format(CultureInfo.InvariantCulture,
							"{0}&nbsp;&nbsp;(&nbsp;{1}&nbsp;)",
							ClientName,
							contactEntity.Organization);
				}
				else
				{
					if (canClientEdit && !Security.CurrentUser.IsExternal)
						retval = String.Format(CultureInfo.InvariantCulture,
							"<a href='{0}'>{1}</a>",
							pageInstance.ResolveClientUrl(CHelper.GetLinkEntityView(ContactEntity.GetAssignedMetaClassName(), ContactUid.ToString())), 
							ClientName);
					else
						retval = ClientName;
				}
			}
			return retval;
		}
		#endregion

		#region GetOrganizationLink
		public static string GetOrganizationLink(Page pageInstance, PrimaryKeyId OrgUid, string OrgName)
		{
			bool canEdit = true;
			if (Security.IsUserInGroup(InternalSecureGroups.Partner))
			{
				int groupId = User.GetGroupForPartnerUser(Security.CurrentUser.UserID);
				using (IDataReader reader = SecureGroup.GetGroup(groupId))
				{
					if (reader.Read())
					{
						if (reader["OrgUid"] == DBNull.Value || PrimaryKeyId.Parse(reader["OrgUid"].ToString()) != OrgUid)
							canEdit = false;
					}
				}
			}
			if (canEdit && !Security.CurrentUser.IsExternal)
				return String.Format(CultureInfo.InvariantCulture,
					"<a href='{0}'>{1}</a>", 
					pageInstance.ResolveClientUrl(CHelper.GetLinkEntityView(OrganizationEntity.GetAssignedMetaClassName(), OrgUid.ToString())),
					OrgName);
			else
				return OrgName;
		}
		#endregion

		#region GetContactLink
		public static string GetContactLink(Page pageInstance, PrimaryKeyId ContactUid, string ClientName)
		{
			bool canEdit = true;
			if (Security.IsUserInGroup(InternalSecureGroups.Partner))
			{
				int groupId = User.GetGroupForPartnerUser(Security.CurrentUser.UserID);
				using (IDataReader reader = SecureGroup.GetGroup(groupId))
				{
					if (reader.Read())
					{
						if (reader["OrgUid"] != DBNull.Value)
						{
							PrimaryKeyId partnerOrgUid = PrimaryKeyId.Parse(reader["OrgUid"].ToString());
							ContactEntity contactEntity = (ContactEntity)BusinessManager.Load(ContactEntity.GetAssignedMetaClassName(), ContactUid);
							if (!contactEntity.OrganizationId.HasValue || contactEntity.OrganizationId.Value != partnerOrgUid)
								canEdit = false;
						}
						else if (reader["ContactUid"] == DBNull.Value || PrimaryKeyId.Parse(reader["ContactUid"].ToString()) != ContactUid)
							canEdit = false;
					}
				}
			}
			if (canEdit && !Security.CurrentUser.IsExternal)
				return String.Format(CultureInfo.InvariantCulture,
					"<a href='{0}'>{1}</a>", 
					pageInstance.ResolveClientUrl(CHelper.GetLinkEntityView(ContactEntity.GetAssignedMetaClassName(), ContactUid.ToString())),
					ClientName);
			else
				return ClientName;
		}
		#endregion

		#region GetShortFileName
		public static string GetShortFileName(string sName, int iMax)
		{
			string sFullName = sName;
			string sExt = "";
			if (sName.LastIndexOf(".") >= 0)
			{
				sFullName = sName.Substring(0, sName.LastIndexOf("."));
				sExt = sName.Substring(sName.LastIndexOf("."));
			}
			if (sFullName.Length > iMax - sExt.Length && iMax - 2 - sExt.Length > 0)
				sFullName = sFullName.Substring(0, iMax - 2 - sExt.Length) + "..";
			return sFullName + sExt;
		}
		#endregion

		#region GetLinkText
		public static string GetLinkText(Mediachase.IBN.Business.ControlSystem.FileStorage fs, int fileId)
		{
			string sLink = "";

			using (System.IO.MemoryStream memStream = new System.IO.MemoryStream())
			{
				fs.LoadFile(fileId, memStream);
				memStream.Position = 0;
				System.IO.StreamReader reader = new System.IO.StreamReader(memStream, Encoding.Unicode);
				string data = reader.ReadLine();
				while (data != null)
				{
					if (data.IndexOf("URL=") >= 0)
					{
						sLink = data.Substring(data.IndexOf("URL=") + 4);
						break;
					}
					data = reader.ReadLine();
				}
			}
			if (sLink != "")
			{
				if (sLink.StartsWith("\\\\"))
					sLink = "file:///" + sLink;
				else if (sLink.IndexOf("://") < 0)
					sLink = "http://" + sLink;
			}
			return sLink;
		}

		public static string GetLinkText(Mediachase.IBN.Business.ControlSystem.FileStorage fs, Mediachase.IBN.Business.ControlSystem.FileInfo fi)
		{
			string sLink = "";

			using (System.IO.MemoryStream memStream = new System.IO.MemoryStream())
			{
				fs.LoadFile(fi, memStream);
				memStream.Position = 0;
				System.IO.StreamReader reader = new System.IO.StreamReader(memStream, Encoding.Unicode);
				string data = reader.ReadLine();
				while (data != null)
				{
					if (data.IndexOf("URL=") >= 0)
					{
						sLink = data.Substring(data.IndexOf("URL=") + 4);
						break;
					}
					data = reader.ReadLine();
				}
			}
			if (sLink != "")
			{
				if (sLink.StartsWith("\\\\"))
					sLink = "file:///" + sLink;
				else if (sLink.IndexOf("://") < 0)
					sLink = "http://" + sLink;
			}
			return sLink;
		}

		public static string GetLinkText(Mediachase.IBN.Business.ControlSystem.FileStorage fs, Mediachase.IBN.Business.ControlSystem.FileHistoryInfo fi)
		{
			string sLink = "";

			using (System.IO.MemoryStream memStream = new System.IO.MemoryStream())
			{
				fs.LoadFile(fi, memStream);
				memStream.Position = 0;
				System.IO.StreamReader reader = new System.IO.StreamReader(memStream, Encoding.Unicode);
				string data = reader.ReadLine();
				while (data != null)
				{
					if (data.IndexOf("URL=") >= 0)
					{
						sLink = data.Substring(data.IndexOf("URL=") + 4);
						break;
					}
					data = reader.ReadLine();
				}
			}
			if (sLink != "")
			{
				if (sLink.IndexOf("://") < 0 && !sLink.StartsWith("\\\\"))
					sLink = "http://" + sLink;
			}
			return sLink;
		}
		#endregion

		#region GetParentContainer
		public static void GetParentContainer(string CKey, Mediachase.IBN.Business.ControlSystem.DirectoryInfo di, out string ParentName, out string ParentLink)
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strLibraryList", typeof(CommonHelper).Assembly);
			int FolderId = di.Id;
			string FolderName = (di.ParentDirectoryId > 0) ? di.Name : LocRM.GetString("tRoot");
			int iLength = (CKey.LastIndexOf("_") >= 0) ? CKey.LastIndexOf("_") : CKey.Length;
			string CKey_Start = CKey.Substring(0, iLength);
			string CKey_End = (CKey.Length > iLength) ? CKey.Substring(iLength + 1) : "";

			int iObjectId = -1;
			try
			{
				iObjectId = int.Parse(CKey_End);
			}
			catch { }

			switch (CKey_Start)
			{
				case "ProjectId":
					ParentName = String.Format("{0} ({1})", Task.GetProjectTitle(iObjectId), LocRM.GetString("tProject"));
					if (Project.CanRead(iObjectId))
						ParentLink = String.Format(CultureInfo.InvariantCulture,
							"<a href='{0}?ProjectId={1}&Tab={2}&FolderId={3}&SubTab=0'>{4}</a>", 
							GetAbsolutePath("/Projects/ProjectView.aspx"),
							iObjectId, 
							"FileLibrary", 
							FolderId, 
							ParentName);
					else
						ParentLink = ParentName;
					break;
				case "IncidentId":
					ParentName = String.Format("{0} ({1})", Incident.GetIncidentTitle(iObjectId), LocRM.GetString("tIssue"));
					if (Incident.CanRead(iObjectId))
						ParentLink = String.Format(CultureInfo.InvariantCulture,
							"<a href='{0}?IncidentId={1}&Tab={2}'>{3}</a>", 
							GetAbsolutePath("/Incidents/IncidentView.aspx"),
							iObjectId, 
							"Forum", 
							ParentName);
					else
						ParentLink = ParentName;
					break;
				case "ForumNodeId":
					CKey = ForumThreadNodeInfo.GetOwnerContainerKey(iObjectId);
					if (CKey != null)
					{
						iLength = (CKey.LastIndexOf("_") >= 0) ? CKey.LastIndexOf("_") : CKey.Length;
						CKey_Start = CKey.Substring(0, iLength);
						CKey_End = (CKey.Length > iLength) ? CKey.Substring(iLength + 1) : "";

						ParentName = String.Format("{0} ({1})", Incident.GetIncidentTitle(int.Parse(CKey_End)), LocRM.GetString("tIssue"));
						if (Incident.CanRead(int.Parse(CKey_End)))
							ParentLink = String.Format(CultureInfo.InvariantCulture,
								"<a href='{0}?IncidentId={1}&Tab={2}'>{3}</a>", 
								GetAbsolutePath("/Incidents/IncidentView.aspx"),
								CKey_End, 
								"Forum", 
								ParentName);
						else
							ParentLink = ParentName;
					}
					else
					{
						ParentName = LocRM.GetString("tIssue");
						ParentLink = ParentName;
					}
					break;
				case "TaskId":
					ParentName = String.Format("{0} ({1})", Task.GetTaskTitle(iObjectId), LocRM.GetString("tTask"));
					if (Task.CanRead(iObjectId))
						ParentLink = String.Format(CultureInfo.InvariantCulture,
							"<a href='{0}?TaskId={1}&Tab={2}&FolderId={3}&SubTab=0'>{4}</a>", 
							GetAbsolutePath("/Tasks/TaskView.aspx"),
							iObjectId, 
							"FileLibrary", 
							FolderId, 
							ParentName);
					else
						ParentLink = ParentName;
					break;
				case "DocumentId":
					ParentName = String.Format("{0} ({1})", Document.GetTitle(iObjectId), LocRM.GetString("tDocument"));
					if (Document.CanRead(iObjectId))
						ParentLink = String.Format(CultureInfo.InvariantCulture,
							"<a href='{0}?DocumentId={1}&Tab={2}&FolderId={3}&SubTab=0'>{4}</a>", 
							GetAbsolutePath("/Documents/DocumentView.aspx"),
							iObjectId, 
							"FileLibrary", 
							FolderId, 
							ParentName);
					else
						ParentLink = ParentName;
					break;
				case "DocumentVers":
					ParentName = String.Format("{0} ({1})", Document.GetTitle(iObjectId), LocRM.GetString("tDocument"));
					if (Document.CanRead(iObjectId))
						ParentLink = String.Format(CultureInfo.InvariantCulture,
							"<a href='{0}?DocumentId={1}&Tab={2}&SubTab=0'>{3}</a>", 
							GetAbsolutePath("/Documents/DocumentView.aspx"),
							iObjectId, 
							"FileLibrary", 
							ParentName);
					else
						ParentLink = ParentName;
					break;
				case "ToDoId":
					ParentName = String.Format("{0} ({1})", Mediachase.IBN.Business.ToDo.GetToDoTitle(iObjectId), LocRM.GetString("tToDo"));
					if (Mediachase.IBN.Business.ToDo.CanRead(iObjectId))
						ParentLink = String.Format(CultureInfo.InvariantCulture,
							"<a href='{0}?ToDoId={1}&Tab={2}&FolderId={3}&SubTab=0'>{4}</a>", 
							GetAbsolutePath("/ToDo/ToDoView.aspx"),
							iObjectId, 
							"FileLibrary", 
							FolderId, 
							ParentName);
					else
						ParentLink = ParentName;
					break;
				case "EventId":
					ParentName = String.Format("{0} ({1})", CalendarEntry.GetEventTitle(iObjectId), LocRM.GetString("tEvent"));
					if (CalendarEntry.CanRead(iObjectId))
						ParentLink = String.Format(CultureInfo.InvariantCulture,
							"<a href='{0}?EventId={1}&Tab={2}&FolderId={3}&SubTab=0'>{4}</a>", 
							GetAbsolutePath("/Events/EventView.aspx"),
							iObjectId, 
							"FileLibrary", 
							FolderId, 
							ParentName);
					else
						ParentLink = ParentName;
					break;
				case "Workspace":
					ParentName = String.Format("{0} ({1})", FolderName, LocRM.GetString("tGlobal"));
					ParentLink = String.Format(CultureInfo.InvariantCulture,
						"<a href='{0}?Tab=0&FolderId={1}'>{2}</a>", 
						GetAbsolutePath("/FileStorage/default.aspx"),
						FolderId, 
						ParentName);
					break;
				case "ArticleId":
					ParentName = String.Format("{0} ({1})", Mediachase.IBN.Business.Common.GetArticleTitle(iObjectId), LocRM.GetString("tKnowledgeBase"));
					ParentLink = String.Format(CultureInfo.InvariantCulture,
						"<a href='{0}?ArticleId={1}'>{2}</a>", 
						GetAbsolutePath("/Incidents/ArticleView.aspx"),
						iObjectId, 
						ParentName);
					break;
				default:
					ParentName = "";
					ParentLink = "";
					break;
			}
		}
		#endregion

		#region Help Strings
		public static string FormatBytes(long bytes)
		{
			const double ONE_KB = 1024;
			const double ONE_MB = ONE_KB * 1024;
			const double ONE_GB = ONE_MB * 1024;
			const double ONE_TB = ONE_GB * 1024;
			const double ONE_PB = ONE_TB * 1024;
			const double ONE_EB = ONE_PB * 1024;
			const double ONE_ZB = ONE_EB * 1024;
			const double ONE_YB = ONE_ZB * 1024;

			if ((double)bytes <= 999)
				return bytes.ToString() + " bytes";
			else if ((double)bytes <= ONE_KB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_KB) + " KB";
			else if ((double)bytes <= ONE_MB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_MB) + " MB";
			else if ((double)bytes <= ONE_GB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_GB) + " GB";
			else if ((double)bytes <= ONE_TB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_TB) + " TB";
			else if ((double)bytes <= ONE_PB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_PB) + " PB";
			else if ((double)bytes <= ONE_EB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_EB) + " EB";
			else if ((double)bytes <= ONE_ZB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_ZB) + " ZB";
			else
				return ThreeNonZeroDigits((double)bytes / ONE_YB) + " YB";
		}

		public static string ThreeNonZeroDigits(double value)
		{
			if (value >= 100)
				return ((int)value).ToString();
			else if (value >= 10)
				return value.ToString("0.0");
			else
				return value.ToString("0.00");
		}
		#endregion

		#region GetControlByServerId
		public static Control GetControlByServerId(Page _page, string serverId)
		{
			Control _control = null;

			Control c = _page.FindControl(serverId);
			if (c != null)
				_control = c;
			else
				_control = GetControlFromCollection(_page.Controls, serverId);

			return _control;
		}

		private static Control GetControlFromCollection(ControlCollection _coll, string serverId)
		{
			Control retVal = null;
			foreach (Control c in _coll)
			{
				Control need = c.FindControl(serverId);
				if (need != null)
				{
					retVal = need;
					break;
				}
				else
				{
					retVal = GetControlFromCollection(c.Controls, serverId);
					if (retVal != null)
						break;
				}
			}
			return retVal;
		}
		#endregion

		#region GetIncidentStatusName
		public static string GetIncidentStatusName(int stateId)
		{
			string retVal = "";
			using (IDataReader reader = Incident.GetIncidentState(stateId))
			{
				if (reader.Read())
					retVal = reader["StateName"].ToString();
			}
			return retVal;
		}
		#endregion

		#region GetAbsoluteDownloadFilePath
		public static string GetAbsoluteDownloadFilePath(int FileId, string FileName, string ContainerName, string ContainerKey)
		{
			string retVal = "";
            if (Security.CurrentUser.IsExternal)
                retVal = String.Format("{0}?Id={1}&CName={2}&CKey={3}", GetAbsolutePath("/External/DownloadFile.aspx"), FileId, ContainerName, ContainerKey);
            else
                retVal = WebDavUrlBuilder.GetFileStorageWebDavUrl( FileId, FileName, true);
				//retVal = GetAbsolutePath(Mediachase.IBN.Business.ControlSystem.WebDavFileUserTicket.GetDownloadPath(FileId, FileName));
			return retVal;
		}

		public static string GetAbsolutePublicDownloadFilePath(int FileId, string FileName, string ContainerName, string ContainerKey)
		{
			string retVal = "";
			if (Security.CurrentUser.IsExternal)
				retVal = String.Format("{0}?Id={1}&CName={2}&CKey={3}", GetAbsolutePath("/External/DownloadFile.aspx"), FileId, ContainerName, ContainerKey);
			else
				retVal = WebDavUrlBuilder.GetFileStorageWebDavUrl(FileId, FileName, false);
			//retVal = GetAbsolutePath(Mediachase.IBN.Business.ControlSystem.WebDavFileUserTicket.GetDownloadPath(FileId, FileName));
			return retVal;
		}

		public static string GetAbsoluteDownloadFilePath(int FileId, string FileName, int HistoryId, string ContainerName, string ContainerKey)
		{
            //TODO: implement file history
			string retVal = "";
			if (Security.CurrentUser.IsExternal)
				retVal = String.Format("{0}?Id={1}&CName={2}&CKey={3}&VId={4}", GetAbsolutePath("/External/DownloadFile.aspx"), FileId, ContainerName, ContainerKey, HistoryId);
			else
                retVal = WebDavUrlBuilder.GetFileStorageWebDavUrl(FileId, HistoryId, FileName, true);
				//retVal = GetAbsolutePath(Mediachase.IBN.Business.ControlSystem.WebDavFileUserTicket.GetDownloadPath(FileId, FileName, HistoryId));
			return retVal;
		}
		#endregion

		#region GetResString
		public static string GetResString(string ResFileName, string ResKey)
		{
			try
			{
				return HttpContext.GetGlobalResourceObject(ResFileName, ResKey).ToString();
			}
			catch
			{
				return ResKey;
			}
		}

		public static string GetResString(string ResKey)
		{
			return GetResString("IbnFramework.Global", ResKey);
		}

		public static string GetResFileString(string ResFileKey)//for {file:key} values
		{
			string sTemp = ResFileKey;
			if (sTemp.StartsWith("{") && sTemp.EndsWith("}"))
			{
				sTemp = sTemp.Substring(1, sTemp.Length - 2);
				string fileName = "IbnFramework.Global";
				if (sTemp.IndexOf(":") >= 0)
				{
					fileName = sTemp.Substring(0, sTemp.IndexOf(":"));
					sTemp = sTemp.Substring(sTemp.IndexOf(":") + 1);
				}
				try
				{
					return HttpContext.GetGlobalResourceObject(fileName, sTemp).ToString();
				}
				catch
				{
				}

				try
				{
					return HttpContext.GetGlobalResourceObject("IbnNext." + fileName, sTemp).ToString();
				}
				catch
				{
					return ResFileKey;
				}
			}
			else
				return ResFileKey;
		}
		#endregion

		#region GetMetaTypeName
		internal static string GetMetaTypeName(Mediachase.Ibn.Data.Meta.Management.MetaField field)
		{
			string typeName = "";
			if (field.IsMultivalueEnum)
				typeName = "EnumMultivalue";
			else if (field.IsEnum)
				typeName = "Enum";
			else
				typeName = field.GetMetaType().Name;
			return typeName;
		}
		#endregion

		#region OpenInNewWindow
		public static bool OpenInNewWindow(string ContentType)
		{
			bool retval = false;
			if (ContentType.ToLower().IndexOf("htm") >= 0 ||
				ContentType.ToLower().IndexOf("message") >= 0 ||
				ContentType.ToLower().IndexOf("url") >= 0 ||
				ContentType.ToLower().IndexOf("image") >= 0
			  )
				retval = true;
			return retval;
		}
		#endregion

		#region AddToContext
		public static void AddToContext(string key, object value)
		{
			if (HttpContext.Current.Items.Contains(key))
				HttpContext.Current.Items[key] = value;
			else
				HttpContext.Current.Items.Add(key, value);
		}
		#endregion

		#region GetFromContext
		public static object GetFromContext(string key)
		{
			if (HttpContext.Current.Items.Contains(key))
				return HttpContext.Current.Items[key];
			else
				return null;
		}
		#endregion

		#region RemoveFromContext
		public static void RemoveFromContext(string key)
		{
			if (HttpContext.Current.Items.Contains(key))
				HttpContext.Current.Items.Remove(key);
		}
		#endregion

		#region UpdateParentPanel
		public static void UpdateParentPanel(Control startPoint)
		{
			Control c = startPoint;
			do
			{
				c = c.Parent;
				if (c is UpdatePanel)
				{
					((UpdatePanel)c).Update();
					break;
				}
			} while (c != startPoint.Page);
		}
		#endregion

		#region UpdatePanelUpdate
		public static bool UpdatePanelUpdate(Page _page, string upName)
		{
			UpdatePanel up = null;
			up = GetUpdatePanelFromCollection(_page.Controls, upName);
			if (up != null)
			{
				up.Update();
				return true;
			}
			return false;
		}

		private static UpdatePanel GetUpdatePanelFromCollection(ControlCollection coll, string upName)
		{
			UpdatePanel retVal = null;
			foreach (Control c in coll)
			{
				if (c is UpdatePanel && c.ID == upName)
				{
					retVal = (UpdatePanel)c;
					break;

				}
				else
				{
					retVal = GetUpdatePanelFromCollection(c.Controls, upName);
					if (retVal != null)
						break;
				}
			}
			return retVal;
		}
		#endregion

		#region UpdateAllParentPanels
		public static void UpdateAllParentPanels(Control startPoint)
		{
			Control c = startPoint;
			do
			{
				c = c.Parent;
				if (c is UpdatePanel)
				{
					((UpdatePanel)c).Update();
				}
			} while (c != startPoint.Page);
		}
		#endregion

		#region CheckCardField
		public static bool CheckCardField(Mediachase.Ibn.Data.Meta.Management.MetaClass _class, Mediachase.Ibn.Data.Meta.Management.MetaField cardField)
		{
			string CardPKeyName = string.Format("{0}Id", cardField.Owner.Name);
			string CardRefKeyName = string.Format("{0}Id", _class.Name);
			return (cardField.Name != CardRefKeyName &&
					cardField.Name != CardPKeyName &&
					!(cardField.GetOriginalMetaType().McDataType == Mediachase.Ibn.Data.Meta.Management.McDataType.ReferencedField &&
					cardField.Attributes.GetValue<string>(Mediachase.Ibn.Data.Meta.Management.McDataTypeAttribute.ReferencedFieldMetaClassName) == _class.Name)
					);
		}
		#endregion

		#region GetAllMetaFields
		public static List<Mediachase.Ibn.Data.Meta.Management.MetaField> GetAllMetaFields(Mediachase.Ibn.Data.Meta.Management.MetaView View)
		{
			List<Mediachase.Ibn.Data.Meta.Management.MetaField> retVal = new List<Mediachase.Ibn.Data.Meta.Management.MetaField>();
			foreach (Mediachase.Ibn.Data.Meta.Management.MetaField field in View.MetaClass.Fields)
			{
				retVal.Add(field);
			}

			if (View.Card != null)
			{
				Mediachase.Ibn.Data.Meta.Management.MetaClass mcCard = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(View.Card.Name);
				foreach (Mediachase.Ibn.Data.Meta.Management.MetaField field in mcCard.Fields)
				{
					if (CommonHelper.CheckCardField(View.MetaClass, field))
						retVal.Add(field);
				}
			}

			return retVal;
		}
		#endregion

		#region GetUserName
		/// <summary>
		/// IbnNext 
		/// </summary>
		/// <param name="UserID"></param>
		/// <returns></returns>
		public static string GetUserName(int UserID)
		{
			try
			{
				Principal pc = new Principal(UserID);
				return pc.Name;
			}
			catch { }

			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(CommonHelper).Assembly);
			return LocRM.GetString("tUnknownUser");
		}
		#endregion

		#region CreateDefaultUserPreference
		public static void CreateDefaultUserPreference(Mediachase.Ibn.Data.Meta.Management.MetaView View)
		{
			Mediachase.Ibn.Core.McMetaViewPreference pref = new Mediachase.Ibn.Core.McMetaViewPreference();
			pref.MetaView = View;

			foreach (Mediachase.Ibn.Data.Meta.Management.MetaField field in View.MetaClass.Fields)
			{
				pref.SetAttribute(field.Name, Mediachase.Ibn.Core.McMetaViewPreference.AttrWidth, 150);
			}

			pref.Attributes.Set("MarginTop", 10);
			pref.Attributes.Set("MarginLeft", 10);
			pref.Attributes.Set("MarginRight", 10);
			pref.Attributes.Set("MarginBottom", 10);
			Mediachase.Ibn.Core.UserMetaViewPreference.Save(Security.CurrentUser.UserID, pref);
		}
		#endregion

		#region CreateDefaultUserPreferenceTimeTracking
		public static void CreateDefaultUserPreferenceTimeTracking(Mediachase.Ibn.Data.Meta.Management.MetaView View)
		{
			Mediachase.Ibn.Core.McMetaViewPreference pref = new Mediachase.Ibn.Core.McMetaViewPreference();
			pref.MetaView = View;

			pref.ShowAllMetaField();

			foreach (Mediachase.Ibn.Data.Meta.Management.MetaField field in pref.GetVisibleMetaField())
			{
                if (!field.Name.Contains("Day") && !field.Name.Contains("Title") && !field.Name.Contains("StateFriendlyName"))
				{
					pref.HideMetaField(field.Name);
				}
				else
				{
                    //if (field.Name == "Title")
                    //{
                    //    pref.SetAttribute<int>(field.Name, Mediachase.Ibn.Core.McMetaViewPreference.AttrIndex, 0);
                    //}
					if (field.Name == "DayT")
					{
						pref.SetAttribute<int>(field.Name, Mediachase.Ibn.Core.McMetaViewPreference.AttrIndex, 8);
					}
					else
					{
						if (field.Name.Contains("Day"))
						{
							pref.SetAttribute<int>(field.Name, Mediachase.Ibn.Core.McMetaViewPreference.AttrIndex, Convert.ToInt32(field.Name.Replace("Day", "").Trim()));
						}
						else
						{
							pref.SetAttribute<int>(field.Name, Mediachase.Ibn.Core.McMetaViewPreference.AttrIndex, 0);
						}
					}
				}
			}

			foreach (Mediachase.Ibn.Data.Meta.Management.MetaField field in View.MetaClass.Fields)
			{
				if (field.Name.Contains("Day"))
					pref.SetAttribute(field.Name, Mediachase.Ibn.Core.McMetaViewPreference.AttrWidth, 37);
				else
					pref.SetAttribute(field.Name, Mediachase.Ibn.Core.McMetaViewPreference.AttrWidth, 300);
			}

            pref.SetAttribute<int>("StateFriendlyName", Mediachase.Ibn.Core.McMetaViewPreference.AttrIndex, 9);
			pref.Attributes.Set("MarginTop", 10);
			pref.Attributes.Set("MarginLeft", 10);
			pref.Attributes.Set("MarginRight", 10);
			pref.Attributes.Set("MarginBottom", 150);
            pref.Attributes.Set("PageSize", -1);
			Mediachase.Ibn.Core.UserMetaViewPreference.SaveDefault(pref);
		}
		#endregion

		#region CreateDefaultReportPreferenceTimeTracking
		public static Mediachase.Ibn.Core.McMetaViewPreference CreateDefaultReportPreferenceTimeTracking(Mediachase.Ibn.Data.Meta.Management.MetaView View)
		{
			Mediachase.Ibn.Core.McMetaViewPreference pref = new Mediachase.Ibn.Core.McMetaViewPreference();
			pref.MetaView = View;

			pref.ShowAllMetaField();

			foreach (Mediachase.Ibn.Data.Meta.Management.MetaField field in pref.GetVisibleMetaField())
			{
				if (!field.Name.Contains("Day") && !field.Name.Contains("Title") && !field.Name.Contains("StateFriendlyName"))
				{
					pref.HideMetaField(field.Name);
				}
				else
				{
					//if (field.Name == "Title")
					//{
					//    pref.SetAttribute<int>(field.Name, Mediachase.Ibn.Core.McMetaViewPreference.AttrIndex, 0);
					//}
					if (field.Name == "DayT")
					{
						pref.SetAttribute<int>(field.Name, Mediachase.Ibn.Core.McMetaViewPreference.AttrIndex, 8);
					}
					else
					{
						if (field.Name.Contains("Day"))
						{
							pref.SetAttribute<int>(field.Name, Mediachase.Ibn.Core.McMetaViewPreference.AttrIndex, Convert.ToInt32(field.Name.Replace("Day", "").Trim()));
						}
						else
						{
							pref.SetAttribute<int>(field.Name, Mediachase.Ibn.Core.McMetaViewPreference.AttrIndex, 0);
						}
					}
				}
			}

			foreach (Mediachase.Ibn.Data.Meta.Management.MetaField field in View.MetaClass.Fields)
			{
				if (field.Name.Contains("Day"))
					pref.SetAttribute(field.Name, Mediachase.Ibn.Core.McMetaViewPreference.AttrWidth, 37);
				else
					pref.SetAttribute(field.Name, Mediachase.Ibn.Core.McMetaViewPreference.AttrWidth, 300);
			}

			pref.SetAttribute<int>("StateFriendlyName", Mediachase.Ibn.Core.McMetaViewPreference.AttrIndex, 9);
			pref.Attributes.Set("MarginTop", 10);
			pref.Attributes.Set("MarginLeft", 10);
			pref.Attributes.Set("MarginRight", 10);
			pref.Attributes.Set("MarginBottom", 150);
			pref.Attributes.Set("PageSize", -1);

			Mediachase.Ibn.Core.MetaViewGroupUtil.CollapseAll(MetaViewGroupByType.Secondary, pref);

			return pref;
		}
		#endregion

		#region GetMetaViewPreference
		public static Mediachase.Ibn.Core.McMetaViewPreference GetMetaViewPreference(MetaView CurrentView)
		{
			Mediachase.Ibn.Core.McMetaViewPreference pref = Mediachase.Ibn.Core.UserMetaViewPreference.Load(CurrentView, Mediachase.IBN.Business.Security.CurrentUser.UserID);

			if (pref == null || pref.Attributes.Count == 0)
			{
				CreateDefaultUserPreference(CurrentView);
				pref = Mediachase.Ibn.Core.UserMetaViewPreference.Load(CurrentView, Mediachase.IBN.Business.Security.CurrentUser.UserID);
			}

			return pref;
		}
		#endregion

		//*****IbnNext***************

		#region EventDefaultTemp
		public static string GetEventResourceString(MetaObject eventObject)
		{
			string retVal = GetResFileString(eventObject.Properties["EventTitle"].Value.ToString());
			//{event:...}
			MatchCollection coll = Regex.Matches(retVal, "{event:(?<EventProp>[^}]*)}");
			foreach (Match match in coll)
			{
				string sArg = match.Groups["EventProp"].Value;
				retVal = retVal.Replace(match.ToString(), GetResFileString(eventObject.Properties[sArg].Value.ToString()));
			}
			//{args:...}
			if (eventObject.Properties["ArgumentType"].Value != null &&
				eventObject.Properties["ArgumentData"].Value != null)
			{
				string argumentType = eventObject.Properties["ArgumentType"].Value.ToString();
				string argumentData = eventObject.Properties["ArgumentData"].Value.ToString();
				MatchCollection argscoll = Regex.Matches(retVal, "{args:(?<EventArg>[^}]*)}");
				if (argscoll.Count > 0)
				{
					Type objType = Mediachase.Ibn.Data.AssemblyUtil.LoadType(argumentType);
					object obj = McXmlSerializer.GetObject(objType, argumentData);
					if (obj != null)
					{
						foreach (Match match in argscoll)
						{
							string p_name = match.Groups["EventArg"].Value;
							PropertyInfo pinfo = objType.GetProperty(p_name);
							if (pinfo != null)
							{
								string sTemp = pinfo.GetValue(obj, null).ToString();

								retVal = retVal.Replace(match.ToString(), GetResFileString(sTemp));
							}
						}
					}
				}
			}
			return retVal;
		}
		#endregion

		#region GetPermissionIconPath
		public static string GetPermissionIconPath(int rightValue)
		{
			return GetPermissionIconPath(rightValue, false);
		}

		public static string GetPermissionIconPath(int rightValue, bool isInhereted)
		{
			string path;

			path = GetAbsolutePath("/Images/IbnFramework/Blank.gif");
			if (rightValue == (int)Mediachase.Ibn.Data.Services.Security.Rights.Allow)
				path = GetAbsolutePath("/Images/IbnFramework/Shield-Green-Tick.png");
			else if (rightValue == (int)Mediachase.Ibn.Data.Services.Security.Rights.Forbid)
				path = GetAbsolutePath("/Images/IbnFramework/Shield-Red-Cross.png");

			return path;
		}
		#endregion

		#region GetPermissionImage
		public static string GetPermissionImage(int rightValue)
		{
			return GetPermissionImage(rightValue, false);
		}

		public static string GetPermissionImage(int rightValue, bool isInhereted)
		{
			string toolTip;
			if (rightValue == (int)Mediachase.Ibn.Data.Services.Security.Rights.Allow)
				toolTip = HttpContext.GetGlobalResourceObject("IbnFramework.Security", "PermissionAllow").ToString();
			else if (rightValue == (int)Mediachase.Ibn.Data.Services.Security.Rights.Forbid)
				toolTip = HttpContext.GetGlobalResourceObject("IbnFramework.Security", "PermissionForbid").ToString();
			else
				toolTip = HttpContext.GetGlobalResourceObject("IbnFramework.Security", "PermissionNone").ToString();

			return String.Format(CultureInfo.InvariantCulture,
				"<img src='{0}' alt='{1}'/>",
				GetPermissionIconPath(rightValue, isInhereted),
				toolTip);
		}
		#endregion
		//*****End IbnNext***********

		#region GetObjectLinkWithIcon
		public static string GetObjectLinkWithIcon(int objectTypeId, int objectId, string objectName, int stateId, bool isOverdue, Page page)
		{
			string link = GetObjectLink(objectTypeId, objectId, page);

			string icon = GetObjectIcon(objectTypeId, objectId, stateId, isOverdue, page);

			return string.Format("<a href=\"{0}\">{1} {2}</a>", link, icon, objectName);
		}
		#endregion

		#region GetObjectIcon
		public static string GetObjectIcon(int objectTypeId, int objectId, int stateId, bool isOverdue, Page page)
		{
			string icon = String.Empty;

			if (objectTypeId == (int)ObjectTypes.ToDo)
			{
				icon = "task.gif";
				if (stateId == (int)ObjectStates.Overdue || isOverdue)
					icon = "task_overdue.gif";
				else if (stateId == (int)ObjectStates.Active)
					icon = "task_started.gif";
				else if (stateId == (int)ObjectStates.Suspended)
					icon = "task_suspensed.gif";
				else if (stateId == (int)ObjectStates.Completed)
					icon = "task_completed.gif";
			}
			else if (objectTypeId == (int)ObjectTypes.Task)
			{
				icon = "task1.gif";
				if (stateId == (int)ObjectStates.Overdue || isOverdue)
					icon = "task1_overdue.gif";
				else if (stateId == (int)ObjectStates.Active)
					icon = "task1_started.gif";
				else if (stateId == (int)ObjectStates.Suspended)
					icon = "task1_suspensed.gif";
				else if (stateId == (int)ObjectStates.Completed)
					icon = "task1_completed.gif";
			}
			else if (objectTypeId == (int)ObjectTypes.Issue)
			{
				icon = "incident_closed.gif";
				if (isOverdue)
					icon = "incident_overdue.gif";
				else if (stateId == (int)ObjectStates.Upcoming)
					icon = "incident_new.gif";
				else if (stateId == (int)ObjectStates.Active || stateId == (int)ObjectStates.ReOpen || stateId == (int)ObjectStates.OnCheck)
					icon = "incident_active.gif";
			}
			else if (objectTypeId == (int)ObjectTypes.CalendarEntry)
			{
				icon = "event.gif";
				if (stateId == (int)ObjectStates.Active)
					icon = "event_started.gif";
				else if (stateId == (int)ObjectStates.Completed)
					icon = "event_completed.gif";
			}
			else if (objectTypeId == (int)ObjectTypes.Document)
			{
				icon = "document_active.gif";
				if (isOverdue)
					icon = "document_overdue.gif";
				if (stateId == (int)ObjectStates.Completed)
					icon = "document_completed.gif";
				else if (stateId == (int)ObjectStates.Suspended)
					icon = "document_suspensed.gif";
			}

			if (icon != string.Empty)
			{
				icon = String.Format(CultureInfo.InvariantCulture, "~/Layouts/Images/icons/{0}", icon);
				icon = page.ResolveUrl(icon);

				icon = String.Format(CultureInfo.InvariantCulture, "<img alt='' src='{0}'/>", icon);
			}
			return icon;
		}
		#endregion

		#region GetHtmlFileTitle
		public static string GetHtmlFileTitle(string fileName)
		{
			string retVal = fileName;
			retVal = retVal.Replace("\\", "_").Replace("/", "_").Replace("*", "_").Replace("?", "_").Replace("<", "_").Replace(">", "_").Replace(":", "_").Replace("\"", "_");
			return retVal;
		}
		#endregion

		#region GetObjectTypeByClassName
		public static ObjectTypes GetObjectTypeByClassName(string className)
		{
			className = className.ToLowerInvariant();
			ObjectTypes retVal = ObjectTypes.UNDEFINED;

			if (className == ContactEntity.GetAssignedMetaClassName().ToLowerInvariant())
				retVal = ObjectTypes.Contact;
			else if (className == OrganizationEntity.GetAssignedMetaClassName().ToLowerInvariant())
				retVal = ObjectTypes.Organization;
			else if (className == DocumentEntity.GetAssignedMetaClassName().ToLowerInvariant())
				retVal = ObjectTypes.Document;
			else if (className == CalendarEventEntity.ClassName.ToLowerInvariant())
				retVal = ObjectTypes.CalendarEvent;

			return retVal;
		}
		#endregion

		#region GetClassNameByObjectType
		public static string GetClassNameByObjectType(ObjectTypes objectType)
		{
			return GetClassNameByObjectType((int)objectType);
		}

		public static string GetClassNameByObjectType(int objectTypeId)
		{
			string retVal = String.Empty;

			if (objectTypeId == (int)ObjectTypes.Contact)
				retVal = ContactEntity.GetAssignedMetaClassName();
			else if (objectTypeId == (int)ObjectTypes.Organization)
				retVal = OrganizationEntity.GetAssignedMetaClassName();
			else if (objectTypeId == (int)ObjectTypes.Document)
				retVal = DocumentEntity.GetAssignedMetaClassName();
			else if (objectTypeId == (int)ObjectTypes.CalendarEvent)
				retVal = CalendarEventEntity.ClassName;

			return retVal;
		}
		#endregion

		#region GetLockerText
		public static string GetLockerText(string sLink)
		{
			string retVal = String.Empty;
			UserLight locker = WebDavSessionManager.GetFileLockedUserId(sLink);
			if (locker != null)
			{
				ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strLibraryAdvanced", Assembly.GetExecutingAssembly());
				retVal = String.Format("<span class='IconAndText' style='color:#858585;font-size:7pt;'><img src='{0}'/> {2} {1}</span>",
					GetAbsolutePath("/Layouts/Images/lock.png"),
					GetUserStatusPureName(locker.UserID),
					LocRM.GetString("tEditedBy"));
			}
			return retVal;
		} 
		#endregion

		#region CanViewOrganization
		public static bool CanViewOrganization(PrimaryKeyId orgUid)
		{
			bool retVal = true;

			if (Security.IsUserInGroup(InternalSecureGroups.Partner))
			{
				retVal = false;

				// get the partner groupId
				int groupId = User.GetGroupForPartnerUser(Security.CurrentUser.UserID);

				// get the OrgUid for the partner group
				PrimaryKeyId partnerOrgUid = PrimaryKeyId.Empty;
				using (IDataReader reader = SecureGroup.GetGroup(groupId))
				{
					if (reader.Read())
					{
						if (reader["OrgUid"] != DBNull.Value)
							partnerOrgUid = PrimaryKeyId.Parse(reader["OrgUid"].ToString());
					}
				}

				if (partnerOrgUid == orgUid)
				{
					retVal = true;
				}
			}

			return retVal;
		}
		#endregion

		#region CanViewContact
		public static bool CanViewContact(PrimaryKeyId contactUid)
		{
			bool retVal = true;

			if (Security.IsUserInGroup(InternalSecureGroups.Partner))
			{
				retVal = false;

				// get the partner groupId
				int groupId = User.GetGroupForPartnerUser(Security.CurrentUser.UserID);

				// get the OrgUid/ContactUid for the partner group
				PrimaryKeyId partnerContactUid = PrimaryKeyId.Empty;
				PrimaryKeyId partnerOrgUid = PrimaryKeyId.Empty;
				using (IDataReader reader = SecureGroup.GetGroup(groupId))
				{
					if (reader.Read())
					{
						if (reader["ContactUid"] != DBNull.Value)
							partnerContactUid = PrimaryKeyId.Parse(reader["ContactUid"].ToString());
						if (reader["OrgUid"] != DBNull.Value)
							partnerOrgUid = PrimaryKeyId.Parse(reader["OrgUid"].ToString());
					}
				}

				if (partnerContactUid == contactUid)	// the same contact
				{
					retVal = true;
				}
				else if (partnerOrgUid != PrimaryKeyId.Empty)
				{
					// if partner group has OrgUid, 
					// then check, that contactUid is contact in the same organization
					ContactEntity contactEntity = (ContactEntity)BusinessManager.Load(ContactEntity.GetAssignedMetaClassName(), contactUid);

					if (contactEntity != null 
						&& contactEntity.OrganizationId.HasValue 
						&& partnerOrgUid == contactEntity.OrganizationId.Value)
					{
						retVal = true;
					}
				}
			}
			return retVal;
		}
		#endregion

		#region public static void RegisterScrollScript(Page page, string pageXClientId, string pageYClientId)
		public static void RegisterScrollScript(Page page, string pageXClientId, string pageYClientId)
		{
			StringBuilder builder = new StringBuilder();

			builder.AppendLine("	//<![CDATA[");
			builder.AppendLine("	function ScrollIt() {");
			builder.AppendLine("		window.scrollTo(document.forms[0]." + pageXClientId + ".value, document.forms[0]." + pageYClientId + ".value);");
			builder.AppendLine("	}");
			builder.AppendLine("	function setcoords() {");
			builder.AppendLine("		var myPageX;");
			builder.AppendLine("		var myPageY;");
			builder.AppendLine("		if (document.all) {");
			builder.AppendLine("			myPageX = document.body.scrollLeft;");
			builder.AppendLine("			myPageY = document.body.scrollTop;");
			builder.AppendLine("		}");
			builder.AppendLine("		else {");
			builder.AppendLine("			myPageX = window.pageXOffset;");
			builder.AppendLine("			myPageY = window.pageYOffset;");
			builder.AppendLine("		}");
			builder.AppendLine("		document.forms[0]." + pageXClientId + ".value = myPageX;");
			builder.AppendLine("		document.forms[0]." + pageYClientId + ".value = myPageY;");
			builder.AppendLine("	}");
			builder.AppendLine("	//]]>");

			UtilHelper.RegisterScriptBlock(page, builder.ToString(), true);
		}
		#endregion

		#region public static void RegisterLocationScript(Page page, string title)
		public static void RegisterLocationScript(Page page, string title)
		{
			string pageUrl = page.ResolveUrl("~/Apps/Shell/Pages/default.aspx");

			StringBuilder builder = new StringBuilder();

			builder.AppendLine("	//<![CDATA[");
			builder.AppendLine("	if (parent == window) {");
			builder.AppendLine("		if (location.replace)");
			builder.AppendLine("			location.replace('" + pageUrl + "#right=' + escapeWithAmp(location.href));");
			builder.AppendLine("		else");
			builder.AppendLine("			location.href = '" + pageUrl + "#right=' + escapeWithAmp(location.href);");
			builder.AppendLine("	}");
			builder.AppendLine("	else {");
			builder.AppendLine("		if (parent && parent.document) {");
			builder.AppendLine("			var td = parent.document.getElementById(\"onetidPageTitle\");");
			builder.AppendLine("			if (td)");
			builder.AppendLine("				td.innerHTML = self.document.title;");
			builder.AppendLine("		}");
			builder.AppendLine("		top.document.title = self.document.title + '" + title + "';");
			builder.AppendLine("	}");
			builder.AppendLine("	function escapeWithAmp(str) {");
			builder.AppendLine("		var re = /&/gi;");
			builder.AppendLine("		var ampEncoded = \"%26\";");
			builder.AppendLine("		return escape(str).replace(re, ampEncoded);");
			builder.AppendLine("	}");
			builder.AppendLine("	//]]>");

			UtilHelper.RegisterScriptBlock(page, builder.ToString(), true);
		}
		#endregion

		#region public static void SafeRegisterStyle(Page page, string link)
		public static void SafeRegisterStyle(Page page, string link)
		{
			if (page == null)
				throw new ArgumentNullException("page");

			try
			{
				UtilHelper.RegisterCssStyleSheet(page, link);
			}
			catch
			{
				if (!page.ClientScript.IsClientScriptBlockRegistered(link))
				{
					string url = UtilHelper.ResolveUrl(page, link);
					page.ClientScript.RegisterClientScriptBlock(page.GetType(), link, "<link type='text/css' rel='stylesheet' href='" + url + "' />");
				}
			}
		}
		#endregion

		public static string GetCompanyLogoUrl(Page page)
		{
			if (page == null)
				throw new ArgumentNullException("page");

			return page.ResolveUrl(string.Concat("~/Common/CompanyLogo.aspx?v=", PortalConfig.PortalCompanyLogoVersion));
		}
	}
}
