namespace Mediachase.UI.Web.Calendar.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Reflection;
	using System.Resources;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using Mediachase.UI.Web.Util;
	using Mediachase.Web.UI.WebControls;

	/// <summary>
	///		Summary description for CalendarView2.
	/// </summary>
	public partial class CalendarViewMy : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Calendar.Resources.strCalendar", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(CalendarViewMy).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Calendar.Resources.strCalendarView", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM4 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(CalendarViewMy).Assembly);

		IFormatProvider culture = CultureInfo.InvariantCulture;

		#region PersonId
		private int PersonId
		{
			get
			{
				if (Request["PersonId"] != null)
					return int.Parse(Request["PersonId"].ToString());
				else
					return -1;
			}
		}
		#endregion

		private string HeaderText = "";

		UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		#region BTab
		private string BTab
		{
			get
			{
				if (pc["Calendar1_CurrentTab"] != "SharedCalendars" && Request["Tab"] != "SharedCalendars")
				{
					return Request["MyTab"];
				}
				else
					return Request["BTab"];

			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnApply.Text = LocRM.GetString("Apply");
			cbChkAll.Text = "&nbsp;" + LocRM2.GetString("tObjects");
			BindToolbar();
			BindTabs();
			BindCalendar();
		}

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			BindInfoView();
			bool _chk = true;
			foreach (ListItem liItem in cblType.Items)
				if (!liItem.Selected)
				{
					_chk = false;
					break;
				}
			cbChkAll.Checked = _chk;
			lbShowFilter.Text = "<img align='absmiddle' border='0' title='" + LocRM.GetString("tShowFilter") + "' src='../Layouts/Images/scrolldown_hover.GIF' />";
			lbHideFilter.Text = "<img align='absmiddle' border='0' title='" + LocRM.GetString("tHideFilter") + "' src='../Layouts/Images/scrollup_hover.GIF' />";
			trFilter.Visible = (pc["Cal_ShowFilter"] != null && bool.Parse(pc["Cal_ShowFilter"]));
			trFilterView.Visible = !trFilter.Visible;

			secHeader.Title = HeaderText;

			CalendarPre_Render();
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			if (pc["Calendar1_CurrentTab"] != "SharedCalendars" && Request["Tab"] != "SharedCalendars")
			{
				HeaderText = LocRM.GetString("tMyCalendar");
			}
			else
				HeaderText = LocRM.GetString("tSharedCalendars");
		}
		#endregion

		#region BindTabs
		private void BindTabs()
		{
			string PropertyName = "CalendarView2_CurrentTab";

			if (pc["Calendar1_CurrentTab"] != "SharedCalendars" && Request["Tab"] != "SharedCalendars")
			{
			}
			else
				PropertyName = "CalendarView2Shared_CurrentTab";


			if (BTab != null && (BTab == "DailyCalendar" || BTab == "WeeklyCalendar" || BTab == "YearlyCalendar" || BTab == "MonthlyCalendar" || BTab == "WorkWeeklyCalendar"))
				pc[PropertyName] = BTab;
			else if (pc[PropertyName] == null)
				pc[PropertyName] = "MonthlyCalendar";

			if (pc[PropertyName] == "WorkWeeklyCalendar")
				HeaderText += " - " + LocRM.GetString("tWWView");
			else if (pc[PropertyName] == "DailyCalendar")
				HeaderText += " - " + LocRM.GetString("tDayView");
			else if (pc[PropertyName] == "WeeklyCalendar")
				HeaderText += " - " + LocRM.GetString("tWeekView");
			else if (pc[PropertyName] == "MonthlyCalendar")
				HeaderText += " - " + LocRM.GetString("tMonthView");
			else if (pc[PropertyName] == "YearlyCalendar")
				HeaderText += " - " + LocRM.GetString("tYearView");
		}
		#endregion

		#region BindCalendar
		private void BindCalendar()
		{
			if (pc["Calendar1_CurrentTab"] != "SharedCalendars")
				tdPerson.Visible = false;
			else
				tdPerson.Visible = true;

			if (!IsPostBack)
			{
				BindDefaultValues();
				BindSavedData();
			}
			else
			{
				CalendarCtrl.ViewType = (CalendarViewType)ViewState["CalendarCurrentView"];
				CalendarCtrl.SelectedDate = (DateTime)ViewState["CalendarSelectedDate"];
			}
		} 
		#endregion

		#region CalendarPre_Render
		private void CalendarPre_Render()
		{
			BindToolbar2();
			BindClendarControl();

			ViewState["CalendarCurrentView"] = CalendarCtrl.ViewType;
			ViewState["CalendarSelectedDate"] = CalendarCtrl.SelectedDate;
		} 
		#endregion

		#region BindToolbar2
		private void BindToolbar2()
		{
			if (!(pc["Calendar1_CurrentTab"] == "SharedCalendars") || Mediachase.IBN.Business.CalendarView.GetSharingLevel(int.Parse(ddlPerson.SelectedItem.Value)) == 1)
			{
				string SharedId = String.Empty;
				if (pc["Calendar1_CurrentTab"] == "SharedCalendars" && ViewState["SharedId"] != null)
					SharedId = "?SharedId=" + ViewState["SharedId"];

				ComponentArt.Web.UI.MenuItem subItem;

				#region Create Calendar Entry
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.LookId = "TopItemLook";
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/event_create.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = String.Format("../Events/EventEdit.aspx{0}", SharedId);
				subItem.Text = LocRM3.GetString("AddEvent");
				secHeader.ActionsMenu.Items.Add(subItem);
				#endregion

				#region Create ToDo
				if (pc["Calendar1_CurrentTab"] != "SharedCalendars")
				{
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.LookId = "TopItemLook";
					subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task_create.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.NavigateUrl = String.Format("../ToDo/ToDoEdit.aspx{0}", SharedId);
					subItem.Text = LocRM3.GetString("AddToDo");
					secHeader.ActionsMenu.Items.Add(subItem);
				}
				#endregion
			}
		}
		#endregion

		#region BindClendarControl
		private void BindClendarControl()
		{
			if (!IsPostBack)
				BindViewType();
			else
				SelectRightTab(CalendarCtrl.ViewType);
			BindLabelHeader();

			int mask = 0;
			foreach (ListItem li in cblType.Items)
				if (li.Selected)
					mask = mask | int.Parse(li.Value);

			int UserID = 0;
			if (pc["Calendar1_CurrentTab"] != "SharedCalendars")
				UserID = Security.CurrentUser.UserID;
			else
				UserID = int.Parse((string)ViewState["SharedId"]);

			DataTable dt = Mediachase.IBN.Business.CalendarView.GetListCalendarEntriesByUser(
				CalendarCtrl.DisplayStartDate, CalendarCtrl.DisplayEndDate, 
				true, false, false, mask, UserID, true);

			int fdow = (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
			fdow = (int)Math.Pow(2, fdow);
			CalendarCtrl.FirstDayOfWeek = (CalendarDayOfWeek)fdow;
			CalendarCtrl.Items.Clear();
			CalendarCtrl.DataSource = dt.DefaultView;
			CalendarCtrl.DataBind();
			CalendarCtrl.DayLinkFormat = "../events/eventedit.aspx?start={0:g}";
		}
		#endregion

		#region BindViewType
		private void BindViewType()
		{
			string PropertyName = "CalendarView2_CurrentTab";

			if (pc["Calendar1_CurrentTab"] == "SharedCalendars" || Request["Tab"] == "SharedCalendars")
				PropertyName = "CalendarView2Shared_CurrentTab";

			switch (pc[PropertyName])
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
				case "WorkWeeklyCalendar":
					CalendarCtrl.ViewType = CalendarViewType.WorkWeekView;
					break;
			}
		}
		#endregion

		#region SelectRightTab
		private void SelectRightTab(Mediachase.Web.UI.WebControls.CalendarViewType e)
		{
			string PropertyName = "CalendarView2_CurrentTab";
			if (pc["Calendar1_CurrentTab"] == "SharedCalendars" || Request["Tab"] == "SharedCalendars")
				PropertyName = "CalendarView2Shared_CurrentTab";

			switch (e)
			{
				case CalendarViewType.DayView:
					pc[PropertyName] = "DailyCalendar";
					break;
				case CalendarViewType.MonthView:
					pc[PropertyName] = "MonthlyCalendar";
					break;
				case CalendarViewType.WeekView2:
					pc[PropertyName] = "WeeklyCalendar";
					break;
				case CalendarViewType.WorkWeekView:
					pc[PropertyName] = "WorkWeeklyCalendar";
					break;
				case CalendarViewType.YearView:
					pc[PropertyName] = "YearlyCalendar";
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
					lblDec.Text = li + LocRM3.GetString("PD");
					lblInc.Text = LocRM3.GetString("ND") + ri;
					break;
				case CalendarViewType.WeekView2:
					lblMonth.Text = CalendarCtrl.SelectedDate.ToString("MMMM, yyyy");
					lblDec.Text = li + LocRM3.GetString("PW");
					lblInc.Text = LocRM3.GetString("NW") + ri;
					break;
				case CalendarViewType.WorkWeekView:
					lblMonth.Text = CalendarCtrl.SelectedDate.ToString("MMMM, yyyy");
					lblDec.Text = li + LocRM3.GetString("PW");
					lblInc.Text = LocRM3.GetString("NW") + ri;
					break;
				case CalendarViewType.MonthView:
					lblMonth.Text = CalendarCtrl.SelectedDate.ToString("MMMM, yyyy");
					lblDec.Text = li + LocRM3.GetString("PM");
					lblInc.Text = LocRM3.GetString("NM") + ri;
					break;
				case CalendarViewType.YearView:
					lblMonth.Text = CalendarCtrl.SelectedDate.ToString("yyyy");
					lblDec.Text = li + LocRM3.GetString("PY");
					lblInc.Text = LocRM3.GetString("NY") + ri;
					break;
				default:
					break;
			}
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			cblType.Items.Add(new ListItem(LocRM3.GetString("Event"), ((int)Mediachase.IBN.Business.CalendarView.CalendarFilter.Event).ToString()));
			cblType.Items.Add(new ListItem(LocRM3.GetString("Meeting"), ((int)Mediachase.IBN.Business.CalendarView.CalendarFilter.Meeting).ToString()));
			cblType.Items.Add(new ListItem(LocRM3.GetString("Appointment"), ((int)Mediachase.IBN.Business.CalendarView.CalendarFilter.Appointment).ToString()));
			if (Configuration.ProjectManagementEnabled)
				cblType.Items.Add(new ListItem(LocRM3.GetString("Task"), ((int)Mediachase.IBN.Business.CalendarView.CalendarFilter.Task).ToString()));
			cblType.Items.Add(new ListItem(LocRM3.GetString("ToDo"), ((int)Mediachase.IBN.Business.CalendarView.CalendarFilter.ToDo).ToString()));

			foreach (ListItem li in cblType.Items)
				li.Selected = true;

			using (IDataReader rdr = Mediachase.IBN.Business.CalendarView.GetListPeopleForCalendar())
			{
				while (rdr.Read())
				{
					ddlPerson.Items.Add(new ListItem((string)rdr["LastName"] + " " + (string)rdr["FirstName"], rdr["UserId"].ToString()));
				}
			}
			if (PersonId > 0)
			{
				pc["CWrapper_People"] = PersonId.ToString();
			}
			if (Request["Date"] != null)
				CalendarCtrl.SelectedDate = DateTime.Parse(Server.UrlDecode(Request["Date"]));

			try
			{
				string[] timeParts;

				timeParts = PortalConfig.WorkTimeStart.Split(':');
				CalendarCtrl.DayStartHour = int.Parse(timeParts[0]);

				timeParts = PortalConfig.WorkTimeFinish.Split(':');
				CalendarCtrl.DayEndHour = int.Parse(timeParts[0]);
			}
			catch
			{
			}
		}
		#endregion

		#region BindSavedData
		private void BindSavedData()
		{
			if (pc["CWrapper_Type"] != null)
			{
				int evt = int.Parse(pc["CWrapper_Type"]);
				cblType.ClearSelection();
				foreach (ListItem li in cblType.Items)
					if ((evt & int.Parse(li.Value)) != 0)
						li.Selected = true;
			}

			if (pc["CWrapper_People"] != null)
			{
				ddlPerson.ClearSelection();
				CommonHelper.SafeSelect(ddlPerson, pc["CWrapper_People"]);
			}

			if (ddlPerson.SelectedItem != null)
				ViewState["SharedId"] = ddlPerson.SelectedItem.Value;
		}
		#endregion

		#region BindInfoView
		private void BindInfoView()
		{
			tblFilterInfoSet.Rows.Clear();

			//Person
			if (pc["Calendar1_CurrentTab"] == "SharedCalendars")
			{
				ListItem li = ddlPerson.SelectedItem;
				if (li != null && li.Value != "0")
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("Person")), li.Text);
			}

			// Objects
			string Objs = "";
			foreach (ListItem liItem in cblType.Items)
			{
				if (liItem.Selected)
				{
					if (Objs != "")
						Objs += ", ";
					Objs += liItem.Text;
				}
			}
			if (Objs != "")
				AddRow(String.Format("{0}:&nbsp; ", LocRM2.GetString("tObjects")), Objs);
		}

		private void AddRow(string filterName, string filterValue)
		{
			HtmlTableRow tr = new HtmlTableRow();
			HtmlTableCell td1 = new HtmlTableCell();
			td1.Align = "Right";
			td1.Width = "120px";
			td1.InnerHtml = filterName;
			HtmlTableCell td2 = new HtmlTableCell();
			td2.InnerHtml = filterValue;
			tr.Cells.Add(td1);
			tr.Cells.Add(td2);
			tblFilterInfoSet.Rows.Add(tr);
		}
		#endregion

		#region SaveValues
		private void SaveValues()
		{
			int mask = 0;
			foreach (ListItem li in cblType.Items)
				if (li.Selected)
					mask = mask | int.Parse(li.Value);

			pc["CWrapper_Type"] = mask.ToString();

			if (ddlPerson.SelectedItem != null)
			{
				pc["CWrapper_People"] = ddlPerson.SelectedItem.Value;
				ViewState["SharedId"] = ddlPerson.SelectedItem.Value;
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
			this.lbShowFilter.Click += new EventHandler(lbShowFilter_Click);
			this.lbHideFilter.Click += new EventHandler(lbShowFilter_Click);
			this.CalendarCtrl.SelectedViewChange += new Mediachase.Web.UI.WebControls.SelectEventHandler(this.CalendarCtrl_SelectedViewChange);
			this.btnApply.Click += new System.EventHandler(this.btnApplyFilter_Click);
		}
		#endregion

		#region CalendarCtrl_SelectedViewChange
		private void CalendarCtrl_SelectedViewChange(object sender, Mediachase.Web.UI.WebControls.CalendarViewSelectEventArgs e)
		{

			SelectRightTab(e.NewViewType);

			BindLabelHeader();
			BindClendarControl();
		}
		#endregion

		#region btnApplyFilter_Click
		private void btnApplyFilter_Click(object sender, System.EventArgs e)
		{
			SaveValues();
			BindClendarControl();
		}
		#endregion

		#region GetUrl
		protected string GetUrl(int ID, Mediachase.IBN.Business.CalendarView.CalendarFilter Type)
		{
			string SharedId = "";
			if (pc["Calendar1_CurrentTab"] == "SharedCalendars" && ViewState["SharedId"] != null)
				SharedId = "&SharedId=" + ViewState["SharedId"];

			switch (Type)
			{
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.Appointment:
					return "../Events/EventView.aspx?EventID=" + ID + SharedId;
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.Event:
					return "../Events/EventView.aspx?EventID=" + ID + SharedId;
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.Task:
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.MileStone:
					return "../Tasks/TaskView.aspx?TaskID=" + ID + SharedId;
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.Meeting:
					return "../Events/EventView.aspx?EventID=" + ID + SharedId;
				case Mediachase.IBN.Business.CalendarView.CalendarFilter.ToDo:
					return "../ToDo/ToDoView.aspx?ToDoID=" + ID + SharedId;
			}
			return "";
		}
		#endregion

		#region lbShowFilter_Click
		private void lbShowFilter_Click(object sender, EventArgs e)
		{
			if (pc["Cal_ShowFilter"] == null || !bool.Parse(pc["Cal_ShowFilter"]))
				pc["Cal_ShowFilter"] = "True";
			else
				pc["Cal_ShowFilter"] = "False";
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
				case CalendarViewType.WorkWeekView:
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
				case CalendarViewType.WorkWeekView:
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
	}
}
