using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;
using Mediachase.Web.UI.WebControls;

namespace Mediachase.UI.Web.Projects.Modules
{
	/// <summary>
	///		Summary description for ProjectCalendarWrapper2.
	/// </summary>
	public partial class ProjectCalendarWrapper2 : System.Web.UI.UserControl
	{
		private UserLightPropertyCollection pc;

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Calendar.Resources.strCalendarView", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Calendar.Resources.strCalendar", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		IFormatProvider culture = CultureInfo.InvariantCulture;

		private string pcKey = "ProjectCalendar_CurrentTab";

		#region ProjectID
		protected int ProjectID
		{
			get
			{
				try
				{
					return int.Parse(Request["ProjectID"]);
				}
				catch
				{
					throw new Exception("ProjectID is Reqired");
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			pc = Security.CurrentUser.Properties;

			if (!IsPostBack)
			{
				BindDefaultValues();
				BindSavedData();
				BindClendarControl();
			}
			btnApply.Text = LocRM.GetString("Apply");
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

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.CalendarCtrl.ItemCreated += new Mediachase.Web.UI.WebControls.CalendarItemEventHandler(this.CalendarCtrl_ItemCreated);
			this.CalendarCtrl.SelectedViewChange += new Mediachase.Web.UI.WebControls.SelectEventHandler(this.CalendarCtrl_SelectedViewChange);
			this.btnApply.Click += new EventHandler(btnApply_Click);
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			cblType.Items.Add(new ListItem(LocRM.GetString("Event"), ((int)Mediachase.IBN.Business.CalendarView.CalendarFilter.Event).ToString()));
			cblType.Items.Add(new ListItem(LocRM.GetString("Meeting"), ((int)Mediachase.IBN.Business.CalendarView.CalendarFilter.Meeting).ToString()));
			cblType.Items.Add(new ListItem(LocRM.GetString("Appointment"), ((int)Mediachase.IBN.Business.CalendarView.CalendarFilter.Appointment).ToString()));
			cblType.Items.Add(new ListItem(LocRM.GetString("Task"), ((int)Mediachase.IBN.Business.CalendarView.CalendarFilter.Task).ToString()));
			cblType.Items.Add(new ListItem(LocRM.GetString("ToDo"), ((int)Mediachase.IBN.Business.CalendarView.CalendarFilter.ToDo).ToString()));

			foreach (ListItem li in cblType.Items)
				li.Selected = true;

			ddlPerson.Items.Clear();
			ddlPerson.Items.Add(new ListItem(LocRM.GetString("AllTeamMembers"), "0"));
			using (IDataReader rdr = Mediachase.IBN.Business.CalendarView.GetListPeopleForCalendar(ProjectID))
			{
				if (rdr != null)
				{
					while (rdr.Read())
						ddlPerson.Items.Add(new ListItem((string)rdr["LastName"] + " " + (string)rdr["FirstName"], rdr["UserId"].ToString()));
				}
			}

			if (Request["Date"] != null)
				CalendarCtrl.SelectedDate = DateTime.Parse(Server.UrlDecode(Request["Date"]));

		}
		#endregion

		#region BindSavedData
		private void BindSavedData()
		{
			if (pc["ProjectCWrapper_Type"] != null)
			{
				int evt = int.Parse(pc["ProjectCWrapper_Type"]);
				cblType.ClearSelection();
				foreach (ListItem li in cblType.Items)
					if ((evt & int.Parse(li.Value)) != 0)
						li.Selected = true;
					else
						li.Selected = false;
			}

			if (pc["ProjectCWrapper_People"] != null)
			{
				ddlPerson.ClearSelection();
				CommonHelper.SafeSelect(ddlPerson, pc["ProjectCWrapper_People"]);
			}
		}
		#endregion

		#region SaveValues
		private void SaveValues()
		{
			int mask = 0;
			foreach (ListItem li in cblType.Items)
				if (li.Selected)
					mask = mask | int.Parse(li.Value);

			pc["ProjectCWrapper_Type"] = mask.ToString();

			if (ddlPerson.SelectedItem != null)
			{
				pc["ProjectCWrapper_People"] = ddlPerson.SelectedItem.Value;
			}
		}
		#endregion

		#region BindClendarControl
		private void BindClendarControl()
		{
			if (!IsPostBack)
				BindViewType();
			BindLabelHeader();

			int mask = 0;
			foreach (ListItem li in cblType.Items)
				if (li.Selected)
					mask = mask | int.Parse(li.Value);

			DataTable dt;

			dt = Mediachase.IBN.Business.CalendarView.GetListCalendarEntries(CalendarCtrl.DisplayStartDate, CalendarCtrl.DisplayEndDate, true, false, false, mask, int.Parse(ddlPerson.SelectedValue), ProjectID, true);

			int fdow = (int)System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
			fdow = (int)Math.Pow(2, fdow);
			CalendarCtrl.FirstDayOfWeek = (CalendarDayOfWeek)fdow;

			CalendarCtrl.Items.Clear();
			CalendarCtrl.DataSource = dt.DefaultView;
			CalendarCtrl.DataBind();
			CalendarCtrl.DayLinkFormat = "../events/eventedit.aspx?ProjectId=" + ProjectID + "&start={0:g}";

			pc["ProjectCWrapper_StartDate"] = CalendarCtrl.SelectedDate.ToString("d", culture);
		}
		#endregion

		#region BindViewType
		private void BindViewType()
		{
			switch (pc[pcKey])
			{
				case "DailyCalendar":
					CalendarCtrl.ViewType = CalendarViewType.DayView;
					break;
				case "MonthlyCalendar":
					CalendarCtrl.ViewType = CalendarViewType.MonthView;
					break;
				case "YearlyCalendar":
					CalendarCtrl.ViewType = CalendarViewType.YearView;
					break;
				case "WeeklyCalendar":
					CalendarCtrl.ViewType = CalendarViewType.WeekView2;
					break;
			}
		}
		#endregion

		#region BindLabelHeader
		private void BindLabelHeader()
		{
			string li = String.Empty;
			string ri = String.Empty;
			switch (CalendarCtrl.ViewType)
			{
				case CalendarViewType.DayView:
					lblMonth.Text = CalendarCtrl.SelectedDate.ToString("dd MMMM, yyyy");
					lblDec.Text = li + LocRM.GetString("PD");
					lblInc.Text = LocRM.GetString("ND") + ri;
					break;
				case CalendarViewType.WeekView2:
					lblMonth.Text = CalendarCtrl.SelectedDate.ToString("MMMM, yyyy");
					lblDec.Text = li + LocRM.GetString("PW");
					lblInc.Text = LocRM.GetString("NW") + ri;
					break;
				case CalendarViewType.MonthView:
					lblMonth.Text = CalendarCtrl.SelectedDate.ToString("MMMM, yyyy");
					lblDec.Text = li + LocRM.GetString("PM");
					lblInc.Text = LocRM.GetString("NM") + ri;
					break;
				case CalendarViewType.YearView:
					lblMonth.Text = CalendarCtrl.SelectedDate.ToString("yyyy");
					lblDec.Text = li + LocRM.GetString("PY");
					lblInc.Text = LocRM.GetString("NY") + ri;
					break;
				default:
					break;
			}
		}
		#endregion

		#region GetUrl
		protected string GetUrl(int ID, Mediachase.IBN.Business.CalendarView.CalendarFilter Type)
		{
			switch (Type)
			{
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.Appointment:
					return "../Events/EventView.aspx?EventID=" + ID;
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.Event:
					return "../Events/EventView.aspx?EventID=" + ID;
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.Task:
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.MileStone:
					return "../Tasks/TaskView.aspx?TaskID=" + ID;
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.Meeting:
					return "../Events/EventView.aspx?EventID=" + ID;
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.ToDo:
					return "../ToDo/ToDoView.aspx?ToDoID=" + ID;
			}
			return "";
		}
		#endregion

		#region CalendarCtrl_ItemCreated
		private void CalendarCtrl_ItemCreated(object sender, Mediachase.Web.UI.WebControls.CalendarItemEventArgs e)
		{
			CalendarItem _ci = e.Item;
			int ID = (int)DataBinder.Eval(e.Item.DataItem, "ID");
			HtmlAnchor lnk = (HtmlAnchor)e.Item.FindControl("lnk");
			switch ((Mediachase.IBN.Business.CalendarView.CalendarFilter)DataBinder.Eval(e.Item.DataItem, "Type"))
			{
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.Appointment:
					//					_ci.LabelColor = Color.LightGoldenrodYellow;
					break;
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.Event:
					//					_ci.LabelColor = Color.LightCoral;
					break;
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.Meeting:
					//					_ci.LabelColor = Color.LightSalmon;
					break;
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.Task:
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.MileStone:
					//					_ci.LabelColor = Color.LightSeaGreen;
					break;
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.ToDo:
					//					_ci.LabelColor = Color.LightSteelBlue;
					break;
			}
		}
		#endregion

		#region DecDateButton_Click
		protected void DecDateButton_Click(object sender, System.EventArgs e)
		{
			switch (CalendarCtrl.ViewType)
			{
				case CalendarViewType.DayView:
					CalendarCtrl.SelectedDate = CalendarCtrl.SelectedDate.AddDays(-1);
					break;
				case CalendarViewType.WeekView2:
					CalendarCtrl.SelectedDate = CalendarCtrl.SelectedDate.AddDays(-7);
					break;
				case CalendarViewType.MonthView:
					CalendarCtrl.SelectedDate = CalendarCtrl.SelectedDate.AddMonths(-1);
					break;
				case CalendarViewType.YearView:
					CalendarCtrl.SelectedDate = CalendarCtrl.SelectedDate.AddYears(-1);
					break;
				default:
					break;
			}
			BindLabelHeader();
			BindClendarControl();
		}
		#endregion

		#region IncDateButton_Click
		protected void IncDateButton_Click(object sender, System.EventArgs e)
		{
			switch (CalendarCtrl.ViewType)
			{
				case CalendarViewType.DayView:
					CalendarCtrl.SelectedDate = CalendarCtrl.SelectedDate.AddDays(+1);
					break;
				case CalendarViewType.WeekView2:
					CalendarCtrl.SelectedDate = CalendarCtrl.SelectedDate.AddDays(+7);
					break;
				case CalendarViewType.MonthView:
					CalendarCtrl.SelectedDate = CalendarCtrl.SelectedDate.AddMonths(+1);
					break;
				case CalendarViewType.YearView:
					CalendarCtrl.SelectedDate = CalendarCtrl.SelectedDate.AddYears(+1);
					break;
				default:
					break;
			}
			BindLabelHeader();
			BindClendarControl();
		}
		#endregion

		#region CalendarCtrl_SelectedViewChange
		private void CalendarCtrl_SelectedViewChange(object sender, Mediachase.Web.UI.WebControls.CalendarViewSelectEventArgs e)
		{
			BindLabelHeader();
			BindClendarControl();
		}
		#endregion

		#region btnApply_Click
		private void btnApply_Click(object sender, EventArgs e)
		{
			SaveValues();
			BindClendarControl();
		}
		#endregion
	}
}
