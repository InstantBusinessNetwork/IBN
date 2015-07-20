namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Text;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Globalization;

	/// <summary>
	///		Summary description for WorkspaceThisWeek.
	/// </summary>
	public partial class WorkspaceThisWeek : System.Web.UI.UserControl
	{
		protected Mediachase.UI.Web.Modules.Separator1 sep1;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strThisWeek", typeof(WorkspaceThisWeek).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;


		private int ProjectID
		{
			get
			{
				try
				{
					return int.Parse(Request["ProjectID"]);
				}
				catch
				{
					return -1;
				}
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbar();
			BindWeek();
		}

		private void BindWeek()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("DayTitle");
			dt.Columns.Add("EventList");

			DateTime day = UserDateTime.UserToday;

			if (pc["ProjectUSetup_ThisWeekDaysBefore"] != null)
				day = day.AddDays(-int.Parse(pc["ProjectUSetup_ThisWeekDaysBefore"]));


			for (int i = 0; i < 7; i++)
			{
				DataRow dr = dt.NewRow();

				string daylink = String.Format(CultureInfo.InvariantCulture,
					"<a href='{0}?ProjectID={1}&amp;Tab=Calendar&amp;SubTab=DailyCalendar&amp;Date={2}' style='padding:5px'>{3} - {4}</a>",
					ResolveClientUrl("~/projects/projectview.aspx"),
					ProjectID,
					Server.UrlEncode(day.ToShortDateString()),
					day.ToString("dddd"),
					day.ToShortDateString());

				if (day.DayOfWeek != UserDateTime.UserNow.DayOfWeek)
					dr["DayTitle"] = daylink;
				else
					dr["DayTitle"] = "<b>" + daylink + "</b>";

				DataTable dtday = CalendarView.GetListEvents(day, ProjectID);
				if (dtday.Rows.Count > 0)
					dr["EventList"] = CreateDayList(dtday, day);
				else
					dr["EventList"] = String.Format(CultureInfo.InvariantCulture,
						"<a href='{0}?ProjectID={1}&amp;Start={2}' style='padding:15px'>{3}</a>",
						ResolveClientUrl("~/Events/EventEdit.aspx"),
						ProjectID,
						Server.UrlEncode(day.ToShortDateString()),
						LocRM.GetString("NoEvents"));
				if (!((day.DayOfWeek == DayOfWeek.Sunday || day.DayOfWeek == DayOfWeek.Saturday) && dtday.Rows.Count == 0))
					dt.Rows.Add(dr);
				day = day.AddDays(1);
			}
			dlWeek.DataSource = dt.DefaultView;
			dlWeek.DataBind();
		}

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
			*/

			int i = 0;
			StringBuilder st = new StringBuilder();
			st.Append(@"<table border='0' cellpadding='4' cellspacing='3' width='100%' class='ibn-propertysheet'>");
			DateTime now = UserDateTime.UserNow;
			//Milestones
			foreach (DataRow dr in dt.Rows)
			{
				DateTime sd = (DateTime)dr["StartDate"];
				DateTime fd = (DateTime)dr["FinishDate"];
				int id = (int)dr["ID"];
				string title = (string)dr["Title"];
				int type = (int)dr["Type"];

				if (type == (int)CalendarView.CalendarFilter.MileStone)
				{
					if ((int)dr["StateId"] == (int)ObjectStates.Completed || (int)dr["StateId"] == (int)ObjectStates.Suspended)
						continue;

					i++;

					string color = "gray";

					st.Append(@"<tr><td width='8' valign='top'>");
					st.Append(@"&nbsp;</td>");

					st.Append(@"<td width='140' valign='top'>");
					st.Append("<span style='color:" + color + "'>" + LocRM.GetString("Milestone") + "</span></td>");
					st.Append(String.Format(CultureInfo.InvariantCulture,
						"<td valign='top'><a href='{0}?TaskID={1}'>{2}</a></td></tr>",
						ResolveClientUrl("~/Tasks/TaskView.aspx"),
						id, 
						title));
				}
			}
			//All Day
			foreach (DataRow dr in dt.Rows)
			{
				DateTime sd = (DateTime)dr["StartDate"];
				DateTime fd = (DateTime)dr["FinishDate"];
				int id = (int)dr["ID"];
				string title = (string)dr["Title"];
				int type = (int)dr["Type"];

				if (sd < day && fd >= day.AddDays(1) && type != (int)CalendarView.CalendarFilter.MileStone)
				{
					i++;

					string color = "green";
					if (sd < now && fd < now)
						color = "gray";

					st.Append(@"<tr><td width='8' valign='top'>");
					if (sd <= now && fd >= now && day.DayOfWeek == now.DayOfWeek)
						st.Append(String.Format(CultureInfo.InvariantCulture,
							@"<img alt='' src='{0}'/></td>",
							ResolveClientUrl("~/Layouts/images/bulet.gif")));
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
			foreach (DataRow dr in dt.Rows)
			{
				DateTime sd = (DateTime)dr["StartDate"];
				DateTime fd = (DateTime)dr["FinishDate"];
				int id = (int)dr["ID"];
				string title = (string)dr["Title"];
				int type = (int)dr["Type"];


				if (!(sd < day && fd >= day.AddDays(1)) && type != (int)CalendarView.CalendarFilter.MileStone)
				{
					i++;

					st.Append(@"<tr><td width='8' valign='top'>");
					if (sd <= now && fd >= now && day.DayOfWeek == now.DayOfWeek)
						st.Append(String.Concat("<img alt='' src='", ResolveClientUrl("~/Layouts/images/bulet.gif"), "'/></td>"));
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
					"<a href='{0}?ProjectID={1}&amp;Start={2}' style='padding:15px'>{3}</a>",
					ResolveClientUrl("~/Events/EventEdit.aspx"),
					ProjectID,
					Server.UrlEncode(day.ToShortDateString()),
					LocRM.GetString("NoEvents"));
			}
		}

		private void BindToolbar()
		{
			tbHeader.Title = LocRM.GetString("tbTitle");
		}

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

		}
		#endregion
	}
}
