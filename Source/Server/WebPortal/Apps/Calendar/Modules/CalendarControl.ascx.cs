using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.AjaxCalendar;
using System.Resources;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.Calendar.Modules
{
	public partial class CalendarControl : System.Web.UI.UserControl
	{

		#region prop: ViewMode
		/// <summary>
		/// Gets the view mode.
		/// </summary>
		/// <value>The view mode.</value>
		public string ViewMode
		{
			get
			{
				if (Request["ViewMode"] != null)
				{
					_pc["AjaxCal_Mode"] = Request["ViewMode"];
					_pc["AjaxCal_Type"] = null;
					return Request["ViewMode"];
				}
				if (_pc["AjaxCal_Mode"] == null)
					_pc["AjaxCal_Mode"] = ViewModeType.WorkWeek.ToString();
				return _pc["AjaxCal_Mode"];
			}
		} 
		#endregion

		#region prop: ViewType
		/// <summary>
		/// Gets the type of the view.
		/// </summary>
		/// <value>The type of the view.</value>
		public string ViewType
		{
			get
			{
				if (Request["ViewType"] != null)
				{
					_pc["AjaxCal_Type"] = Request["ViewType"];
					return Request["ViewType"];
				}
				if (_pc["AjaxCal_Type"] != null)
					return _pc["AjaxCal_Type"];

				ViewModeType mode = (ViewModeType)Enum.Parse(typeof(ViewModeType), this.ViewMode);

				switch (mode)
				{
					case ViewModeType.Day:
					case ViewModeType.Week:
					case ViewModeType.WorkWeek:
						return CalendarViewType.MultiDay.ToString();
					default:
						return null;
				}
			}
		} 
		#endregion

		protected string sCreate = String.Empty;
		protected string sUpdate = String.Empty;
		protected string sUpdateConfirm = String.Empty;
		protected string sDelete = String.Empty;
		protected string sEdit = String.Empty;

		private UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

		protected void Page_Init(object sender, EventArgs e)
		{
			this.imbNext.Click += new ImageClickEventHandler(imbNext_Click);
			this.imbPrev.Click += new ImageClickEventHandler(imbPrev_Click);
			this.lbToday.Click += new EventHandler(lbToday_Click);
			this.CalendarCtrl.SelectedViewChange += new EventHandler<CalendarViewSelectEventArgs>(CalendarCtrl_SelectedViewChange);
			this.dtcWeek.ValueChange += new PickerControl.ValueChanged(dtcWeek_ValueChange);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				InitLabels();
				UpdateTodayButton();
			}
			
			CalendarCtrl.MultiDayView.ItemTemplate = String.Format("<div style=\"background-color: #2952a3; height:1px; font-size:1px; line-height:1px; margin:0px 2px;\"></div><div style=\"background-color: #2952a3; height:1px; font-size:1px; line-height:1px; margin:0px 1px; \"></div><div style=\"background-color:#668CD9; border-left: solid 1px #2952a3; border-right: solid 1px #2952a3; height:100%;\"><table border=\"0\" cellpadding=\"2\" width=\"100%\" cellspacing=\"0\" style=\"background-color:rgb(41, 82, 163); width:100%; font-weight:bold; font-size:11px; color:#ffffff; table-layout:fixed; overflow:hidden; font-family:Verdana,Sans-serif;\"><tr mcc_action=\"move\"><td style=\"cursor:move; overflow:hidden;\" unselectable=\"on\"><span id=\"sHour\"></span>:<span id=\"sMinute\"></span></td></tr></table><div id=\"title\" style=\"cursor:default; color:#ffffff; font-family:Verdana,Sans-serif; font-size:11px;\" unselectable=\"on\"></div><div id=\"resizer\" mcc_action=\"resize\" style=\"font-size:5px; line-height:5px; position:absolute; bottom: 5px; height:5px; width:100%; z-index:11; cursor:s-resize;\"><table width=\"100%\" height=\"100%\"><tr><td align=\"center\" style=\"color:#ffffff\">————–</td></tr></table></div></div><div style=\"position:absolute; bottom:0px; width:100%;\"><div style=\"background-color: #2952a3; height:1px; font-size:1px; line-height:1px; border-left: 1px solid #2952a3; border-right: 1px solid #2952a3;\"></div></div><div mcc_action=\"delete\" style=\"position:absolute; z-index:20;right:2px;top:2px;cursor:default;\"><img src=\"{0}\"/></div>", ResolveUrl("~/Images/IbnFramework/closeTab.gif"));
			CalendarCtrl.MonthView.ItemTemplate = String.Format("<div style=\"background-color:#668cd9; margin-left:1px; margin-right:1px; line-height:1px; font-size:1px; height:1px; cursor:default;\" unselectable=\"on\"></div><div style=\"background-color:#668cd9; padding-top:1px; height:100%; padding-bottom:1px; text-align:left; color:#fff;\" unselectable=\"on\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr style=\"font-family:verdana, sans-serif; font-size:11px;\"><td unselectable=\"on\" id=\"lb\">&nbsp;(</td><td unselectable=\"on\" style=\"cursor:default;\"><span id=\"sTime\" mcc_action=\"move\" style=\"cursor:move;\"></span></td><td unselectable=\"on\" id=\"rb\">)&nbsp;</td><td id=\"title\" style=\"text-align:left;\"></td></tr></table><div mcc_action=\"delete\" style=\"position:absolute; z-index:20;right:2px;top:2px;cursor:default;\"><img src=\"{0}\"/></div></div>", ResolveUrl("~/Images/IbnFramework/closeTab.gif"));
			CalendarCtrl.YearView.ItemTemplate = String.Format("<div style=\"background-color:#668cd9; margin-left:1px; margin-right:1px; line-height:1px; font-size:1px; height:1px; cursor:default;\" unselectable=\"on\"></div><div style=\"background-color:#668cd9; padding-top:1px; height:100%; padding-bottom:1px; text-align:left; color:#fff;\" unselectable=\"on\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr style=\"font-family:verdana, sans-serif; font-size:11px;\"><td unselectable=\"on\" id=\"lb\">&nbsp;(</td><td unselectable=\"on\" style=\"cursor:default;\"><span id=\"sTime\"></span></td><td unselectable=\"on\" id=\"rb\">)&nbsp;</td><td id=\"title\" style=\"text-align:left;\"></td></tr></table><div mcc_action=\"delete\" style=\"position:absolute; z-index:20;right:2px;top:2px;cursor:default;\"><img src=\"{0}\"/></div></div>", ResolveUrl("~/Images/IbnFramework/closeTab.gif"));
			CalendarCtrl.MultiDayView.ItemMapping = "[{\"id\":\"title\",\"property\":\"innerHTML\",\"value\":\"Title\"},{\"id\":\"sHour\",\"property\":\"innerHTML\",\"value\":\"StartDate.getHours()<10? '0' +item.StartDate.getHours():item.StartDate.getHours()\"},{\"id\":\"sMinute\",\"property\":\"innerHTML\",\"value\":\"StartDate.getMinutes()<10 ? '0'+item.StartDate.getMinutes():item.StartDate.getMinutes()\"}]";
			
			CalendarCtrl.MultiDayView.EventBarItemTemplate = String.Format("<div style=\"background-color:#668cd9; margin-left:1px; margin-right:1px; line-height:1px; font-size:1px; height:1px; cursor:default;\" unselectable=\"on\"></div><div style=\"background-color:#668cd9; padding-top:1px; height:100%; padding-bottom:1px; text-align:left; color:#fff;\" unselectable=\"on\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr style=\"font-family:verdana, sans-serif; font-size:11px;\"><td unselectable=\"on\" id=\"lb\">&nbsp;(</td><td unselectable=\"on\" style=\"cursor:default;\"><span id=\"sTime\" mcc_action=\"move\" style=\"cursor:move;\"></span></td><td unselectable=\"on\" id=\"rb\">)&nbsp;</td><td id=\"title\" style=\"text-align:left;\"></td></tr></table><div mcc_action=\"delete\" style=\"position:absolute; z-index:20;right:2px;top:2px;cursor:default;\"><img src=\"{0}\"/></div></div>", ResolveUrl("~/Images/IbnFramework/closeTab.gif"));
			CalendarCtrl.MultiDayView.EventBarItemMapping = "[{\"id\":\"title\",\"property\":\"innerHTML\",\"value\":\"Title\"}, {\"id\":\"lb\",\"property\":\"style.display\", \"value\":\"IsAllDay == true ? 'none' : ''\"}, {\"id\":\"rb\",\"property\":\"style.display\", \"value\":\"IsAllDay == true ? 'none' : ''\"},{\"id\":\"sTime\",\"property\":\"innerHTML\", \"value\":\"IsAllDay == false ? ((item.StartDate.getHours()<10?'0'+item.StartDate.getHours():item.StartDate.getHours())+':'+(item.StartDate.getMinutes()<10?'0'+item.StartDate.getMinutes():item.StartDate.getMinutes())):''\"}]";
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			#region Register Commands
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = new CommandParameters("MC_Art_NewEvent");
			cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
			cp.AddCommandArgument("EventStartDate", "%%START%%");
			cp.AddCommandArgument("EventEndDate", "%%END%%");
			string cmd = cm.AddCommand("", "", "Calendar", cp);
			sCreate = cmd.Replace("\"", "&quot;");

			cp = new CommandParameters("MC_Art_UpdateEvent");
			cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
			cp.AddCommandArgument("Uid", "%%UID%%");
			cmd = cm.AddCommand("", "", "Calendar", cp);
			sUpdate = cmd.Replace("\"", "&quot;");

			cp = new CommandParameters("MC_Art_UpdateEventConfirm");
			cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
			cp.AddCommandArgument("Uid", "%%UID%%");
			cp.AddCommandArgument("args", "%%args%%");
			cmd = cm.AddCommand("", "", "Calendar", cp);
			sUpdateConfirm = cmd.Replace("\"", "&quot;");

			cp = new CommandParameters("MC_Art_DeleteEvent");
			cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
			cp.AddCommandArgument("Uid", "%%UID%%");
			cmd = cm.AddCommand("", "", "Calendar", cp);
			sDelete = cmd.Replace("\"", "&quot;");

			cp = new CommandParameters("MC_Calendar_EditEvent");
			cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
			cp.AddCommandArgument("Uid", "%%UID%%");
			cmd = cm.AddCommand("", "", "Calendar", cp);
			sEdit = cmd.Replace("\"", "&quot;"); 
			#endregion

			dtcWeek.SelectedDate = CalendarCtrl.SelectedDate;			
			this.CalendarCtrl.ViewMode = (ViewModeType)Enum.Parse(typeof(ViewModeType), this.ViewMode);

			if (this.ViewType != null)
				this.CalendarCtrl.ViewType = (CalendarViewType)Enum.Parse(typeof(CalendarViewType), this.ViewType);

			this.CalendarCtrl.Refresh();
		}
		
		protected void InitLabels()
		{
			if (_pc["AjaxCal_SelDate"] != null)
			{
				CalendarCtrl.SelectedDate = DateTime.Parse(_pc["AjaxCal_SelDate"], CultureInfo.InvariantCulture);
			}
			dtcWeek.SelectedDate = CalendarCtrl.SelectedDate;
		}

		protected void UpdateTodayButton()
		{
			DateTime now = DateTime.Now.Date;
			DateTime s = CalendarCtrl.ViewStartDate.Date;
			DateTime e = CalendarCtrl.ViewEndDate.Date;
			if (now >= s && now <= e)
				lbToday.Enabled = false;
			else
				lbToday.Enabled = true;
		}

		void dtcWeek_ValueChange(object sender, EventArgs e)
		{
			CalendarCtrl.SelectedDate = dtcWeek.SelectedDate;
			_pc["AjaxCal_SelDate"] = CalendarCtrl.SelectedDate.ToString(CultureInfo.InvariantCulture);
			CalendarCtrl.Refresh();
			UpdateTodayButton();
		}

		#region Prev_Next_Today
		void lbToday_Click(object sender, EventArgs e)
		{
			this.CalendarCtrl.SelectedDate = UserDateTime.UserNow;
			_pc["AjaxCal_SelDate"] = CalendarCtrl.SelectedDate.ToString(CultureInfo.InvariantCulture);
			this.CalendarCtrl.Refresh();
			UpdateTodayButton();
		}

		void imbPrev_Click(object sender, ImageClickEventArgs e)
		{
			switch (CalendarCtrl.ViewType)
			{
				case CalendarViewType.MultiDay:
					if (CalendarCtrl.ViewMode == ViewModeType.Day)
						CalendarCtrl.SelectedDate = CalendarCtrl.SelectedDate.AddDays(-1);
					if (CalendarCtrl.ViewMode == ViewModeType.WorkWeek ||
						CalendarCtrl.ViewMode == ViewModeType.Week)
						CalendarCtrl.SelectedDate = CalendarCtrl.SelectedDate.AddDays(-7);
					break;
				case CalendarViewType.Month:
					CalendarCtrl.SelectedDate = CalendarCtrl.SelectedDate.AddMonths(-1);
					break;
				case CalendarViewType.Year:
					CalendarCtrl.SelectedDate = CalendarCtrl.SelectedDate.AddYears(-1);
					break;
				default:
					break;
			}
			_pc["AjaxCal_SelDate"] = CalendarCtrl.SelectedDate.ToString(CultureInfo.InvariantCulture);
			CalendarCtrl.Refresh();
			UpdateTodayButton();
		}

		void imbNext_Click(object sender, ImageClickEventArgs e)
		{
			switch (CalendarCtrl.ViewType)
			{
				case CalendarViewType.MultiDay:
					if (CalendarCtrl.ViewMode == ViewModeType.Day)
						CalendarCtrl.SelectedDate = CalendarCtrl.SelectedDate.AddDays(1);
					if (CalendarCtrl.ViewMode == ViewModeType.WorkWeek ||
						CalendarCtrl.ViewMode == ViewModeType.Week)
						CalendarCtrl.SelectedDate = CalendarCtrl.SelectedDate.AddDays(7);
					break;
				case CalendarViewType.Month:
					CalendarCtrl.SelectedDate = CalendarCtrl.SelectedDate.AddMonths(1);
					break;
				case CalendarViewType.Year:
					CalendarCtrl.SelectedDate = CalendarCtrl.SelectedDate.AddYears(1);
					break;
				default:
					break;
			}
			_pc["AjaxCal_SelDate"] = CalendarCtrl.SelectedDate.ToString(CultureInfo.InvariantCulture);
			CalendarCtrl.Refresh();
			UpdateTodayButton();
		} 
		#endregion

		void lbYear_Click(object sender, EventArgs e)
		{
			this.CalendarCtrl.ViewType = CalendarViewType.Year;
			this.CalendarCtrl.Refresh();
			imbNext.Attributes.Add("title", GetGlobalResourceObject("IbnFramework.Calendar", "NextYear").ToString());
			imbPrev.Attributes.Add("title", GetGlobalResourceObject("IbnFramework.Calendar", "PreviousYear").ToString());
			UpdateTodayButton();
			this.dtcWeek.SelectedMode = PickerControl.SelectedModeType.Year;
		}

		void CalendarCtrl_SelectedViewChange(object sender, CalendarViewSelectEventArgs e)
		{
			_pc["AjaxCal_SelDate"] = e.NewDate.ToString(CultureInfo.InvariantCulture);
			switch (e.NewViewType)
			{
				case CalendarViewType.Year:
					_pc["AjaxCal_Type"] = CalendarViewType.Year.ToString();
					break;
				case CalendarViewType.Month:
					_pc["AjaxCal_Type"] = CalendarViewType.Month.ToString();
					break;
				case CalendarViewType.MultiDay:
					_pc["AjaxCal_Type"] = null;
					_pc["AjaxCal_Mode"] = ViewModeType.Day.ToString();
					break;
				default:
					break;
			}
			Response.Redirect("~/Apps/Calendar/Pages/Calendar.aspx", true);
		}
	}
}