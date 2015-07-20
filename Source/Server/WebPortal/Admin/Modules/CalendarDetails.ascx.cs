namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.IBN.Business;
	using Mediachase.IBN;
	using Mediachase.Ibn;
	using System.Reflection;

	/// <summary>
	///		Summary description for CalendarDetails.
	/// </summary>
	public partial class CalendarDetails : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strCalendarDetails", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected bool isMSProject = false;

		#region CalendarID
		protected int CalendarID
		{
			get
			{
				try
				{
					return int.Parse(Request["CalendarID"]);
				}
				catch
				{
					throw new AccessDeniedException();
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbar();
			if (!IsPostBack)
			{
				BindDG();
				BindDGEX();
			}
		}

		#region BindDG
		private void BindDG()
		{
			dgCalendar.Columns[1].HeaderText = LocRM.GetString("WeekDays");
			dgCalendar.Columns[2].HeaderText = LocRM.GetString("WorkTimeIntervals");

			DateTimeFormatInfo dtf = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat;

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
				using (IDataReader rdr =
						   Mediachase.IBN.Business.Calendar.GetListWeekdayHours(CalendarID, (byte)(FromUsaToRus(fdw))))
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

		#region FromUsaToRus
		private int FromUsaToRus(int day)
		{
			if (day == 0)
				return 7;
			return day++;
		}
		#endregion

		#region BindDGEX
		private void BindDGEX()
		{
			dgCalendarEx.Columns[1].HeaderText = LocRM.GetString("Exceptions");
			dgCalendarEx.Columns[2].HeaderText = LocRM.GetString("WorkTimeIntervals");

			DateTimeFormatInfo dtf = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat;

			DataTable dt = new DataTable();
			dt.Columns.Add("ExceptionId", typeof(int));
			dt.Columns.Add("ExceptionInterval");
			dt.Columns.Add("Intervals");

			using (IDataReader rdr = Mediachase.IBN.Business.Calendar.GetListExceptions(CalendarID))
			{
				while (rdr.Read())
				{
					DataRow dr = dt.NewRow();
					dr["ExceptionId"] = rdr["ExceptionId"];
					dr["ExceptionInterval"] = ((DateTime)rdr["FromDate"]).ToShortDateString() + " - " + ((DateTime)rdr["ToDate"]).ToShortDateString();

					string intervals = "";
					using (IDataReader rdrEX = Mediachase.IBN.Business.Calendar.GetListExceptionHours((int)rdr["ExceptionId"]))
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

			foreach (DataGridItem dgi in dgCalendarEx.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("WarningEX") + "')");
			}
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

		#region BindToolbar
		private void BindToolbar()
		{
			int projectId = -1;
			using (IDataReader reader = Mediachase.IBN.Business.Calendar.GetCalendar(CalendarID))
			{
				if (reader.Read())
				{
					secHeader.Title = reader["CalendarName"].ToString();

					if (reader["ProjectId"] != DBNull.Value)
						projectId = (int)reader["ProjectId"];
				}
			}
			if (projectId > 0)
				isMSProject = Project.GetIsMSProject(projectId);

			secHeader.AddLink(String.Format("<img alt='' src='{0}'/> {1}",
				Page.ResolveUrl("~/Layouts/Images/cancel.gif"), LocRM.GetString("Back")),
				Page.ResolveUrl("~/Admin/CalendarList.aspx"));

			secHeader1.Title = LocRM.GetString("Exceptions");
			if (!isMSProject)
				secHeader1.AddLink(String.Format("<img alt='' src='{0}'/> {1}",
					Page.ResolveUrl("~/Layouts/Images/newitem.gif"), LocRM.GetString("NewException")),
					Page.ResolveUrl("~/Admin/ExceptionsEditor.aspx?CalendarID=" + CalendarID));
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

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgCalendarEx.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgEX_delete);

		}
		#endregion

		#region dgEX_delete
		private void dgEX_delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int ExceptionID = int.Parse(e.Item.Cells[0].Text);
			Mediachase.IBN.Business.Calendar.DeleteException(ExceptionID);
			BindDGEX();
		}
		#endregion
	}
}
