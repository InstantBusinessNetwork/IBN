using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using cal = Mediachase.IBN.Business.Calendar;
using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.Directory.Modules
{
	public partial class UserCalendar : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strCalendarDetails", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected int UserCalendarId = -1;

		#region UserId
		private int UserId
		{
			get
			{
				if (Request["UserID"] != null)
					return int.Parse(Request["UserID"]);
				else
					return Mediachase.IBN.Business.Security.CurrentUser.UserID;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				ApplyLocalization();
				BindCalendars();
			}

			#region Visibility Calendar Blocks
			using (IDataReader reader = cal.GetUserCalendarByUser(UserId))
			{
				if (reader.Read())
				{
					firstEnter.Visible = false;
					tdCalendar.Visible = true;
					currentCalendar.Visible = true;
					newCalendar.Visible = false;
					trInformation.Visible = true;
					UserCalendarId = (int)reader["UserCalendarId"];
					int calendarId = (int)reader["CalendarId"];
					CommonHelper.SafeSelect(ddCalendar, calendarId.ToString());
					BindValues(calendarId);
				}
				else
				{
					if (!Page.IsPostBack)
					{
						firstEnter.Visible = true;
						tdCalendar.Visible = false;
					}
					else
					{
						firstEnter.Visible = false;
						tdCalendar.Visible = true;
						newCalendar.Visible = true;
						currentCalendar.Visible = false;
					}
					trInformation.Visible = false;
				}
			} 
			#endregion

			if(!Page.IsPostBack)
				BindToolbar();
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			secCalendar.Title = LocRM.GetString("UserCalendar");
			secCalendarDays.Title = LocRM.GetString("WorkDays");
			secCalendarExcep.Title = LocRM.GetString("CalendarExceptions");
			secUserExcep.Title = LocRM.GetString("UserExceptions");
			
			lbNewCalendar.Text = LocRM.GetString("CreateUserCalendar");

			ibSave.ToolTip = LocRM.GetString("Save");
			ibCancel.ToolTip = LocRM.GetString("Cancel");
			ibEdit.ToolTip = LocRM.GetString("Edit");
		} 
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secUserExcep.AddRightLink(
				String.Format("<img alt='' src='{0}'/> {1}",
					Page.ResolveUrl("~/Layouts/Images/newitem.gif"), LocRM.GetString("NewException")),
				String.Format("{0}?UserCalendarId={1}",
					Page.ResolveUrl("~/Directory/ExceptionsEditor.aspx"), UserCalendarId));
		} 
		#endregion

		#region BindCalendars
		private void BindCalendars()
		{
			DataTable dt = cal.GetListCalendarsDataTable();
			DataView dv = dt.DefaultView;
			dv.RowFilter = "IsNull(ProjectId,-1) = -1";
			ddCalendar.DataSource = dv;
			ddCalendar.DataTextField = "CalendarName";
			ddCalendar.DataValueField = "CalendarId";
			ddCalendar.DataBind();
		} 
		#endregion

		#region BindValues
		private void BindValues(int calendarId)
		{
			using (IDataReader reader = cal.GetCalendar(calendarId))
			{
				if (reader.Read())
				{
					lblCalendar.Text = reader["CalendarName"].ToString();
					BindDG(calendarId);
					BindDGEX(calendarId);
					BindDGUserEX();
				}
			}
		} 
		#endregion

		#region BindDG
		private void BindDG(int CalendarID)
		{
			dgCalendar.Columns[1].HeaderText = LocRM.GetString("WeekDays");
			dgCalendar.Columns[2].HeaderText = LocRM.GetString("WorkTimeIntervals");

			DateTimeFormatInfo dtf = CultureInfo.CurrentCulture.DateTimeFormat;

			DataTable dt = new DataTable();
			dt.Columns.Add("DayOfWeek", typeof(int));
			dt.Columns.Add("DayTitle");
			dt.Columns.Add("Intervals");

			int fdw = (int)dtf.FirstDayOfWeek;
			for (int i = 1; i <= 7; i++)
			{
				DataRow dr = dt.NewRow();
				if (fdw == 7) fdw = 0;

				dr["DayOfWeek"] = FromUsaToRus(fdw);
				dr["DayTitle"] = dtf.GetDayName((DayOfWeek)fdw) + ":";

				string intervals = "";
				using (IDataReader rdr = cal.GetListWeekdayHours(CalendarID, (byte)(FromUsaToRus(fdw))))
				{
					while (rdr.Read())
					{
						if (intervals != "")
							intervals += "<br/>";
						int _fromtime = (int)rdr["FromTime"];
						int _totime = (int)rdr["ToTime"];

						DateTime fromtime = ParseMin(_fromtime);
						DateTime totime = ParseMin(_totime);

						intervals += fromtime.ToShortTimeString() + " - " + totime.ToShortTimeString();
					}
				}

				if (intervals == "")
					intervals = LocRM.GetString("Empty");


				dr["Intervals"] = intervals;

				fdw++;

				dt.Rows.Add(dr);
			}
			dgCalendar.DataSource = dt.DefaultView;
			dgCalendar.DataBind();
		}
		#endregion

		#region BindDGEX
		private void BindDGEX(int CalendarID)
		{
			dgCalendarEx.Columns[1].HeaderText = LocRM.GetString("Exceptions");
			dgCalendarEx.Columns[2].HeaderText = LocRM.GetString("WorkTimeIntervals");

			DateTimeFormatInfo dtf = CultureInfo.CurrentCulture.DateTimeFormat;

			DataTable dt = new DataTable();
			dt.Columns.Add("ExceptionId", typeof(int));
			dt.Columns.Add("ExceptionInterval");
			dt.Columns.Add("Intervals");

			using (IDataReader rdr = cal.GetListExceptions(CalendarID))
			{
				while (rdr.Read())
				{
					DataRow dr = dt.NewRow();
					dr["ExceptionId"] = rdr["ExceptionId"];
					dr["ExceptionInterval"] = ((DateTime)rdr["FromDate"]).ToShortDateString() + " - " + ((DateTime)rdr["ToDate"]).ToShortDateString();

					string intervals = "";
					using (IDataReader rdrEX = cal.GetListExceptionHours((int)rdr["ExceptionId"]))
					{
						while (rdrEX.Read())
						{
							if (intervals != "")
								intervals += "<br/>";
							int _fromtime = (int)rdrEX["FromTime"];
							int _totime = (int)rdrEX["ToTime"];

							DateTime fromtime = ParseMin(_fromtime);
							DateTime totime = ParseMin(_totime);

							intervals += fromtime.ToShortTimeString() + " - " + totime.ToShortTimeString();
						}
					}
					if (intervals == "")
						intervals = LocRM.GetString("Empty");

					dr["Intervals"] = intervals;
					dt.Rows.Add(dr);
				}
			}
			dgCalendarEx.DataSource = dt.DefaultView;
			dgCalendarEx.DataBind();
		}
		#endregion

		#region BindDGUserEX
		private void BindDGUserEX()
		{
			dgUserCalendarEx.Columns[1].HeaderText = LocRM.GetString("Exceptions");
			dgUserCalendarEx.Columns[2].HeaderText = LocRM.GetString("WorkTimeIntervals");

			DateTimeFormatInfo dtf = CultureInfo.CurrentCulture.DateTimeFormat;

			DataTable dt = new DataTable();
			dt.Columns.Add("ExceptionId", typeof(int));
			dt.Columns.Add("ExceptionInterval");
			dt.Columns.Add("Intervals");

			using (IDataReader rdr = cal.GetListUserCalendarExceptions(UserCalendarId))
			{
				while (rdr.Read())
				{
					DataRow dr = dt.NewRow();
					dr["ExceptionId"] = rdr["ExceptionId"];
					dr["ExceptionInterval"] = ((DateTime)rdr["FromDate"]).ToShortDateString() + " - " + ((DateTime)rdr["ToDate"]).ToShortDateString();

					string intervals = "";
					using (IDataReader rdrEX = cal.GetListUserCalendarExceptionHours((int)rdr["ExceptionId"]))
					{
						while (rdrEX.Read())
						{
							if (intervals != "")
								intervals += "<br/>";
							int _fromtime = (int)rdrEX["FromTime"];
							int _totime = (int)rdrEX["ToTime"];

							DateTime fromtime = ParseMin(_fromtime);
							DateTime totime = ParseMin(_totime);

							intervals += fromtime.ToShortTimeString() + " - " + totime.ToShortTimeString();
						}
					}
					if (intervals == "")
						intervals = LocRM.GetString("Empty");

					dr["Intervals"] = intervals;
					dt.Rows.Add(dr);
				}
			}
			dgUserCalendarEx.DataSource = dt.DefaultView;
			dgUserCalendarEx.DataBind();

			foreach (DataGridItem dgi in dgUserCalendarEx.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("WarningEX") + "')");
			}
		}
		#endregion

		#region Visibility Events
		protected void lbNewCalendar_Click(object sender, EventArgs e)
		{
		}

		protected void ibEdit_Click(object sender, ImageClickEventArgs e)
		{
			firstEnter.Visible = false;
			tdCalendar.Visible = true;
			newCalendar.Visible = true;
			currentCalendar.Visible = false;
		}

		protected void ibCancel_Click(object sender, ImageClickEventArgs e)
		{
			if (!trInformation.Visible)
			{
				firstEnter.Visible = true;
				tdCalendar.Visible = false;
			}
		} 
		#endregion

		#region Save UserCalendar
		protected void ibSave_Click(object sender, ImageClickEventArgs e)
		{
			cal.AddUserCalendar(int.Parse(ddCalendar.SelectedValue), UserId);
			this.Page.Response.Redirect(this.Page.Request.RawUrl, true);
		} 
		#endregion

		#region Delete Exception
		protected void dgUserCalendarEx_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandArgument != DBNull.Value)
			{
				int exId = int.Parse(e.CommandArgument.ToString());
				cal.DeleteUserException(exId);
				BindDGUserEX();
			}
		} 
		#endregion


		#region FromUsaToRus
		private int FromUsaToRus(int day)
		{
			if (day == 0)
				return 7;
			return day++;
		}
		#endregion

		#region ParseMin
		private DateTime ParseMin(int minutes)
		{
			TimeSpan ts = TimeSpan.FromMinutes(minutes);
			int hrs = (int)ts.TotalHours;
			int min = ts.Minutes;

			return new DateTime(1990, 1, 1, hrs, min, 0);
		}
		#endregion
	}
}