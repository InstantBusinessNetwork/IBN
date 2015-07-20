using System;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Calendar.Modules
{
	/// <summary>
	///		Summary description for CalendarEntry.
	/// </summary>
	public partial class CalendarEntry : System.Web.UI.UserControl
	{
		protected Mediachase.UI.Web.Modules.Separator1 sep1;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strThisWeek", typeof(CalendarEntry).Assembly);

		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				hdnDate.Value = UserDateTime.UserToday.ToShortDateString();
				msCal.SelectedDate = UserDateTime.UserToday;
			}
			msCal.FirstDayOfWeek = (FirstDayOfWeek)PortalConfig.PortalFirstDayOfWeek;
			msCal.UseAccessibleHeader = false;
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			hdnDate.Value = msCal.SelectedDate.ToShortDateString();
			if (hdnDate.Value != "")
				BindWeek();
			BindToolbar();
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("Week");

			secHeader.AddLink(
				String.Format(CultureInfo.InvariantCulture, "<img alt='' src='{0}' border='0' width='16' height='16' align='absmiddle'/>", ResolveClientUrl("~/Layouts/Images/b4.gif")),
				String.Format(CultureInfo.InvariantCulture, "javascript:{0}", Page.ClientScript.GetPostBackEventReference(lbHide, ""))
				);
		}
		#endregion

		#region BindWeek
		private void BindWeek()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("DayTitle");
			dt.Columns.Add("EventList");

			string tab = "MyTab";

			//DateTime day = DateTime.Parse(hdnDate.Value);
			DateTime day = msCal.SelectedDate;

			//DVS: Fix bug with postback at workspace
			if (day == DateTime.MinValue)
				day = DateTime.Now;

			for (int i = 0; i < 7; i++)
			{
				DataRow dr = dt.NewRow();

				if (day != UserDateTime.UserToday)
				{
					dr["DayTitle"] = String.Format(CultureInfo.InvariantCulture,
						"<a href='{0}?{1}=DailyCalendar&amp;Date={2}' style='padding-left:5px;'>{3} - {4}</a>",
						ResolveClientUrl("~/calendar/default.aspx"),
						tab,
						Server.UrlEncode(day.ToShortDateString()),
						day.ToString("dddd"),
						day.ToShortDateString());
				}
				else
				{
					dr["DayTitle"] = String.Format(CultureInfo.InvariantCulture,
						"<b><a href='{0}?{1}=DailyCalendar&amp;Date={2}' style='padding-left:5px;'>{3} - {4}</a></b>",
						ResolveClientUrl("~/calendar/default.aspx"),
						tab,
						Server.UrlEncode(day.ToShortDateString()),
						LocRM.GetString("Today"),
						day.ToShortDateString());
				}

				DataTable dtday = Mediachase.IBN.Business.CalendarView.GetListEvents(day, 0);
				if (dtday.Rows.Count > 0)
					dr["EventList"] = CreateDayList(dtday, day);
				else
				{
					dr["EventList"] = String.Format(CultureInfo.InvariantCulture,
						"<a href='{0}?Start={1}' style='padding-left:15px;'>{2}</a>",
						ResolveClientUrl("~/Events/EventEdit.aspx"),
						Server.UrlEncode(day.ToShortDateString()),
						LocRM.GetString("NoEvents"));
				}
				if (!((day.DayOfWeek == DayOfWeek.Sunday || day.DayOfWeek == DayOfWeek.Saturday) && dtday.Rows.Count == 0))
					dt.Rows.Add(dr);
				day = day.AddDays(1);
			}
			dlWeek.DataSource = dt.DefaultView;
			dlWeek.DataBind();
		}
		#endregion

		#region CreateDayList
		private string CreateDayList(DataTable dt, DateTime day)
		{
			/*
					table.Columns.Add("ID", typeof(int));
					table.Columns.Add("Type", typeof(int));
					table.Columns.Add("Title", typeof(string));
					table.Columns.Add("Description", typeof(string));
					table.Columns.Add("StartDate", typeof(DateTime));
					table.Columns.Add("FinishDate", typeof(DateTime));
					table.Columns.Add("ShowStartDate", typeof(bool));
					table.Columns.Add("ShowFinishDate", typeof(bool));
					table.Columns.Add("Overdue", typeof(bool));
					
			///  ID, Type, Title, Description, StartDate, FinishDate, ShowStartDate, ShowFinishDate, StateId
			*/

			StringBuilder st = new StringBuilder();
			st.Append(@"<table border='0' cellpadding='4' cellspacing='3' width='100%' class='ibn-propertysheet'>");
			DateTime now = UserDateTime.UserNow;
			int i = 0;
			//Milestones
			DataView dv = dt.DefaultView;
			dv.Sort = "StartDate";
			if (Configuration.ProjectManagementEnabled)
			{
				foreach (DataRowView dr in dv)
				{
					int type = (int)dr["Type"];

					if (type == (int)CalendarView.CalendarFilter.MileStone)
					{
						if ((int)dr["StateId"] == (int)ObjectStates.Completed || (int)dr["StateId"] == (int)ObjectStates.Suspended)
							continue;

						i++;
						DateTime sd = (DateTime)dr["StartDate"];
						DateTime fd = (DateTime)dr["FinishDate"];
						int id = (int)dr["ID"];
						string title = (string)dr["Title"];

						string color = "gray";

						st.Append(@"<tr><td width='8' valign='top'>");
						st.Append(@"&nbsp;</td>");

						st.Append(@"<td width='140' valign='top'>");
						st.Append("<span style='color:" + color + "'>" + LocRM.GetString("Milestone") + "</span></td>");
						st.Append(String.Format("<td valign='top'><a href='{0}?TaskID={1}'>{2}</a></td></tr>", ResolveClientUrl("~/Tasks/TaskView.aspx"), id, title));
					}
				}
			}

			//All Day
			foreach (DataRowView dr in dv)
			{
				DateTime sd = (DateTime)dr["StartDate"];
				DateTime fd = (DateTime)dr["FinishDate"];
				int type = (int)dr["Type"];

				if (sd < day && fd >= day.AddDays(1) && type != (int)CalendarView.CalendarFilter.Task)
				{
					i++;
					int id = (int)dr["ID"];
					string title = (string)dr["Title"];

					string color = "green";
					if (sd < now && fd < now)
						color = "gray";

					st.Append(@"<tr><td width='8' valign='top>'");
					if (sd <= now && fd >= now && day.DayOfWeek == now.DayOfWeek)
						st.Append(String.Format(CultureInfo.InvariantCulture, "<img alt='' src='{0}'/></td>", ResolveClientUrl("~/Layouts/images/bulet.gif")));
					else
						st.Append(@"&nbsp;</td>");

					st.Append(@"<td width='140' valign='top'>");
					st.Append("<span style='color:" + color + "'>" + LocRM.GetString("AllDayEvent") + "</span></td>");
					st.Append(String.Format(CultureInfo.InvariantCulture,
						"<td valign='top'><a href='{0}?EventID={1}'>{2}</a></td></tr>",
						ResolveClientUrl("~/Events/EventView.aspx"),
						id,
						title));
				}
			}

			// none all day //
			foreach (DataRowView dr in dv)
			{
				DateTime sd = (DateTime)dr["StartDate"];
				DateTime fd = (DateTime)dr["FinishDate"];
				int type = (int)dr["Type"];

				if (!(sd < day && fd >= day.AddDays(1)) && (type != (int)CalendarView.CalendarFilter.MileStone))
				{
					i++;
					int id = (int)dr["ID"];
					string title = (string)dr["Title"];

					st.Append(@"<tr><td width='8' valign='top'>");
					if (sd <= now && fd >= now && day.DayOfWeek == now.DayOfWeek)
						st.Append(String.Format(CultureInfo.InvariantCulture, "<img alt='' src='{0}'/></td>", ResolveClientUrl("~/Layouts/images/bulet.gif")));
					else
						st.Append(@"&nbsp;</td>");

					st.Append(@"<td width='140' valign='top'>");
					string color = "black";
					if (sd < now && fd < now) color = "gray";
					if (sd < day && fd >= day.AddDays(1))
						st.Append("<span style='color:green'>" + LocRM.GetString("AllDayEvent") + "</span></td>");
					else if (sd < day)
						st.Append("<span style='color:" + color + "'>" + LocRM.GetString("Till") + " " +
							fd.ToShortTimeString() + "</span></td>");
					else if (fd >= day.AddDays(1))
						st.Append("<span style='color:" + color + "'>" + LocRM.GetString("From") + " " +
							sd.ToShortTimeString() + "</span></td>");
					else
						st.Append("<span style='color:" + color + "'>" + sd.ToShortTimeString() + " - " +
							fd.ToShortTimeString() + "</span></td>");

					st.Append(String.Format(CultureInfo.InvariantCulture,
						"<td valign='top'><a href='{0}?EventID={1}'>{2}</a></td></tr>",
						ResolveClientUrl("~/Events/EventView.aspx"),
						id,
						title));
				}
			}
			st.Append("</table>");
			if (i > 0)
				return st.ToString();
			else
			{
				return String.Format(CultureInfo.InvariantCulture,
					"<a href='{0}?Start={1}' style='padding:15px'>{2}</a>",
					ResolveClientUrl("~/Events/EventEdit.aspx"),
					Server.UrlEncode(day.ToShortDateString()),
					LocRM.GetString("NoEvents"));
			}
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			lbHide.Click += new EventHandler(lbHide_Click);
		}
		#endregion

		private void lbHide_Click(object sender, EventArgs e)
		{
			//CommonHelper.HideWorkspaceControl("10");
			Response.Redirect("~/Workspace/default.aspx?BTab=Workspace");
		}
	}
}
