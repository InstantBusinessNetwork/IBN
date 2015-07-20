using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Apps.ClioSoft.Modules
{
	public partial class PickerControl : System.Web.UI.UserControl
	{
		#region event: OnValueChange
		public delegate void ValueChanged(Object sender, EventArgs e);

		public event ValueChanged ValueChange;

		protected virtual void OnValueChange(Object sender, EventArgs e)
		{
			if (ValueChange != null)
			{
				ValueChange(this, e);
			}
		}
		#endregion

		#region prop: ShowImageButton
		public bool ShowImageButton
		{
			get
			{
				if (ViewState[this.ID + "_ShowImageButton"] == null)
					return true;
				return Convert.ToBoolean(ViewState[this.ID + "_ShowImageButton"].ToString());
			}
			set
			{
				ViewState[this.ID + "_ShowImageButton"] = value;
			}
		}
		#endregion

		public enum SelectedModeType
		{
			Day,
			Week,
			WorkWeek,
			Month,
			Year
		}

		#region prop: ShowWeekNumber
		/// <summary>
		/// Gets or sets a value indicating whether [show week number].
		/// </summary>
		/// <value><c>true</c> if [show week number]; otherwise, <c>false</c>.</value>
		public bool ShowWeekNumber
		{
			get
			{
				if (ViewState[this.ID + "_ShowWeekNumber"] == null)
					return false;

				return Convert.ToBoolean(ViewState[this.ID + "_ShowWeekNumber"].ToString(), CultureInfo.InvariantCulture);
			}
			set
			{
				ViewState[this.ID + "_ShowWeekNumber"] = value;
			}
		}
		#endregion

		#region prop: SelectedMode
		public SelectedModeType SelectedMode
		{
			get
			{
				if (ViewState["_selectedMode"] != null)
					return (SelectedModeType)ViewState["_selectedMode"];
				else
					return SelectedModeType.Day;
			}
			set
			{
				ViewState["_selectedMode"] = value;
				calendarButtonExtender.SelectedMode = value.ToString();
			}
		}
		#endregion

		#region prop: SelectedDate
		public DateTime SelectedDate
		{
			get
			{
				string sDate = hfUpdateCalendar.Value.Split(new char[] { '^', '^' })[0];// txtDate.Text;

				hfUpdateCalendar.Value = string.Empty;
				//IE FIX
				sDate = sDate.Replace("UTC", string.Empty);

				if (sDate == string.Empty)
				{
					//if (this.ShowWeekNumber && txtDate.Text.Contains(":"))
					//    sDate = txtDate.Text.Substring(3);
					//else
					sDate = txtDate.Text;
				}

				DateTime retVal = DateTime.MinValue;
				if (!String.IsNullOrEmpty(sDate))
				{
					if (sDate.IndexOf("-") >= 0)
						sDate = sDate.Substring(0, sDate.IndexOf("-"));
					retVal = DateTime.Parse(sDate, CultureInfo.CurrentCulture.DateTimeFormat);
				}
				if (SelectedMode == SelectedModeType.Week)
				{
					if (!String.IsNullOrEmpty(txtDate.Text))
					{
						txtDate.Text = CommonHelper.GetWeekStartByDate(retVal).ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat) + " - " + CommonHelper.GetWeekStartByDate(retVal).AddDays(6).ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat);

						if (this.ShowWeekNumber)
							txtDate.Text = String.Format("{0} (#{1})", txtDate.Text, Iso8601WeekNumber.GetWeekNumber(retVal));
					}
					return CommonHelper.GetWeekStartByDate(retVal);
				}
				else if (SelectedMode == SelectedModeType.WorkWeek)
				{
					DateTime sd = retVal;
					DateTime ed = retVal;
					if ((int)FirstDayOfWeek > 0 || ((int)FirstDayOfWeek == 0 && (int)sd.DayOfWeek > 0))
					{
						while ((int)sd.DayOfWeek != 1)
						{
							sd = sd.AddDays(-1);
						}
					}
					else
					{
						while ((int)sd.DayOfWeek != 1)
						{
							sd = sd.AddDays(1);
						}
					}
					ed = sd.AddDays(4);
					if (!String.IsNullOrEmpty(txtDate.Text))
						txtDate.Text = sd.ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat) + " - " + ed.ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat);
					return retVal;
				}
				else if (SelectedMode == SelectedModeType.Month)
				{
					DateTime sd = retVal;
					DateTime ed = retVal;
					GetMonthViewLimits(retVal, out sd, out ed);
					if (!String.IsNullOrEmpty(txtDate.Text))
						txtDate.Text = sd.ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat) + " - " + ed.ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat);
					return retVal;
				}
				else if (SelectedMode == SelectedModeType.Year)
				{
					DateTime sd = new DateTime(retVal.Year, 1, 1);
					DateTime ed = new DateTime(retVal.Year, 12, 31);
					if (!String.IsNullOrEmpty(txtDate.Text))
						txtDate.Text = sd.ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat) + " - " + ed.ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat);
					return retVal;
				}
				else
				{
					if (!String.IsNullOrEmpty(txtDate.Text))
						txtDate.Text = retVal.ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat);
					if (!ShowTime)
						return retVal;
					//TIME
					#region Time
					string str = txtTime.Text;
					int idouble = str.IndexOf(":");
					if (idouble < 0)
						return retVal;
					string sH = str.Substring(0, idouble);
					string sM = str.Substring(idouble + 1, 2);
					string sAM = str.Substring(idouble + 3);
					int h = int.Parse(sH);
					int m = int.Parse(sM);
					if (!String.IsNullOrEmpty(sAM))
					{
						if (sAM.ToLower().Equals("am"))
						{
							if (h == 12)
								h = 0;
						}
						if (sAM.ToLower().Equals("pm"))
						{
							if (h < 12)
								h += 12;
						}
					}
					retVal = retVal.AddHours(h);
					retVal = retVal.AddMinutes(m);
					#endregion

					return retVal;
				}
			}
			set
			{
				DateTime retVal = value;
				if (retVal == DateTime.MinValue)
				{
					txtDate.Text = "";
					txtTime.Text = "";
					return;
				}
				if (SelectedMode == SelectedModeType.Week)
				{
					retVal = CommonHelper.GetWeekStartByDate(value);
					txtDate.Text = retVal.ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat) + " - " + retVal.AddDays(6).ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat);
					//txtDate.Text = string.Format("{0} ({1})", txtDate.Text, Iso8601WeekNumber.GetWeekNumber(this.SelectedDate));
					//TODO Iso8601WeekNumber.GetWeekNumber(this.SelectedDate);
				}
				else if (SelectedMode == SelectedModeType.WorkWeek)
				{
					DateTime sd = retVal;
					DateTime ed = retVal;
					if ((int)FirstDayOfWeek > 0 || ((int)FirstDayOfWeek == 0 && (int)sd.DayOfWeek > 0))
					{
						while ((int)sd.DayOfWeek != 1)
						{
							sd = sd.AddDays(-1);
						}
					}
					else
					{
						while ((int)sd.DayOfWeek != 1)
						{
							sd = sd.AddDays(1);
						}
					}
					ed = sd.AddDays(4);
					txtDate.Text = sd.ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat) + " - " + ed.ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat);
				}
				else if (SelectedMode == SelectedModeType.Month)
				{
					DateTime sd = retVal;
					DateTime ed = retVal;
					GetMonthViewLimits(retVal, out sd, out ed);
					txtDate.Text = sd.ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat) + " - " + ed.ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat);
				}
				else if (SelectedMode == SelectedModeType.Year)
				{
					DateTime sd = new DateTime(retVal.Year, 1, 1);
					DateTime ed = new DateTime(retVal.Year, 12, 31);
					txtDate.Text = sd.ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat) + " - " + ed.ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat);
				}
				else
				{
					txtDate.Text = retVal.ToString(this.DateFormat, CultureInfo.CurrentCulture.DateTimeFormat);
					txtTime.Text = retVal.ToString(this.TimeFormat, CultureInfo.CurrentCulture.DateTimeFormat);
				}
			}
		}
		#endregion

		#region prop: FirstDayOfWeek
		public FirstDayOfWeek FirstDayOfWeek
		{
			get
			{
				if (ViewState["_firstDayOfWeek"] != null)
					return (FirstDayOfWeek)ViewState["_firstDayOfWeek"];
				else
					return (FirstDayOfWeek)PortalConfig.PortalFirstDayOfWeek;
			}
			set
			{
				ViewState["_firstDayOfWeek"] = value;
				calendarButtonExtender.FirstDayOfWeek = value;
			}
		}
		#endregion

		#region prop: AutoPostBack
		public bool AutoPostBack
		{
			get
			{
				if (ViewState["_autoPostBack"] != null)
					return (bool)ViewState["_autoPostBack"];
				else
					return false;
			}
			set
			{
				ViewState["_autoPostBack"] = value;
				if (value)
					calendarButtonExtender.UpdateElementId = hfUpdateCalendar.ClientID;
			}
		}
		#endregion

		#region prop: DateFormat
		public string DateFormat
		{
			get
			{
				if (ViewState[this.ID + "_DateFormat"] != null)
					return ViewState[this.ID + "_DateFormat"].ToString();
				else
					return "d";
			}
			set
			{
				ViewState[this.ID + "_DateFormat"] = value;
			}
		}
		#endregion

		#region prop: TimeFormat
		public string TimeFormat
		{
			get
			{
				if (ViewState[this.ID + "_TimeFormat"] != null)
					return ViewState[this.ID + "_TimeFormat"].ToString();
				else
					return "t";
			}
			set
			{
				ViewState[this.ID + "_TimeFormat"] = value;
			}
		}
		#endregion

		#region prop: ShowTime
		public bool ShowTime
		{
			get
			{
				if (ViewState[this.ID + "_ShowTime"] != null)
					return (bool)ViewState[this.ID + "_ShowTime"];
				else
					return false;
			}
			set
			{
				ViewState[this.ID + "_ShowTime"] = value;
			}
		}
		#endregion

		#region prop: DateWidth
		public Unit DateWidth
		{
			get
			{
				if (ViewState[this.ID + "_DateWidth"] != null)
				{
					return (Unit)ViewState[this.ID + "_DateWidth"];
				}
				else
				{
					if (this.ShowWeekNumber)
						return Unit.Pixel(250);
					else
						return Unit.Pixel(180);
				}
			}
			set
			{
				ViewState[this.ID + "_DateWidth"] = value;
			}
		}
		#endregion

		#region prop: TimeWidth
		public Unit TimeWidth
		{
			get
			{
				if (ViewState[this.ID + "_TimeWidth"] != null)
					return (Unit)ViewState[this.ID + "_TimeWidth"];
				else
					return Unit.Pixel(90);
			}
			set
			{
				ViewState[this.ID + "_TimeWidth"] = value;
			}
		}
		#endregion

		#region prop: DateCssClass
		public string DateCssClass
		{
			get
			{
				if (ViewState[this.ID + "_DateCssClass"] != null)
					return ViewState[this.ID + "_DateCssClass"].ToString();
				else
					return String.Empty;
			}
			set
			{
				ViewState[this.ID + "_DateCssClass"] = value;
			}
		}
		#endregion

		#region prop: TimeCssClass
		public string TimeCssClass
		{
			get
			{
				if (ViewState[this.ID + "_TimeCssClass"] != null)
					return ViewState[this.ID + "_TimeCssClass"].ToString();
				else
					return String.Empty;
			}
			set
			{
				ViewState[this.ID + "_TimeCssClass"] = value;
			}
		}
		#endregion

		#region prop: ButtonImageUrl
		public string ButtonImageUrl
		{
			get
			{
				if (ViewState[this.ID + "_ButtonImageUrl"] != null)
					return ViewState[this.ID + "_ButtonImageUrl"].ToString();
				else
					return "~/Layouts/Images/mcCalendar.gif";
			}
			set
			{
				ViewState[this.ID + "_ButtonImageUrl"] = value;
			}
		}
		#endregion

		#region prop: ResetImageUrl
		public string ResetImageUrl
		{
			get
			{
				if (ViewState[this.ID + "_ResetImageUrl"] != null)
					return ViewState[this.ID + "_ResetImageUrl"].ToString();
				else
					return "~/Layouts/Images/reset17.gif";
			}
			set
			{
				ViewState[this.ID + "_ResetImageUrl"] = value;
			}
		}
		#endregion

		#region prop: ReadOnly
		public bool ReadOnly
		{
			get
			{
				if (ViewState[this.ID + "_ReadOnly"] != null)
					return (bool)ViewState[this.ID + "_ReadOnly"];
				else
					return false;
			}
			set
			{
				ViewState[this.ID + "_ReadOnly"] = value;
			}
		}
		#endregion

		#region prop: Enabled
		public bool Enabled
		{
			get
			{
				if (ViewState[this.ID + "_Enabled"] != null)
					return (bool)ViewState[this.ID + "_Enabled"];
				else
					return true;
			}
			set
			{
				ViewState[this.ID + "_Enabled"] = value;
				txtDate.Enabled = value;
				txtTime.Enabled = value;
				tpExt.Enabled = value;
			}
		}
		#endregion

		#region prop: TimeOnly
		public bool TimeOnly
		{
			get
			{
				if (ViewState[this.ID + "_TimeOnly"] != null)
					return (bool)ViewState[this.ID + "_TimeOnly"];
				else
					return false;
			}
			set
			{
				ViewState[this.ID + "_TimeOnly"] = value;
			}
		}
		#endregion

		#region prop: DateIsRequired
		public bool DateIsRequired
		{
			get
			{
				if (ViewState[this.ID + "_DateIsRequired"] != null)
					return (bool)ViewState[this.ID + "_DateIsRequired"];
				else
					return false;
			}
			set
			{
				ViewState[this.ID + "_DateIsRequired"] = value;
			}
		}
		#endregion

		#region prop: DefaultTimeString
		public string DefaultTimeString
		{
			get
			{
				if (ViewState[this.ID + "_DefaultTimeString"] != null)
					return ViewState[this.ID + "_DefaultTimeString"].ToString();
				else
					return String.Empty;
			}
			set
			{
				ViewState[this.ID + "_DefaultTimeString"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (TimeOnly)
			{
				ShowTime = true;
				tdDate.Visible = false;
				imgCell.Visible = false;
				tdSeparator.Visible = false;
			}
			rfDate.Enabled = DateIsRequired;

			tpExt.CalendarId = calendarButtonExtender.ClientID;
			tpExt.IsAM = !String.IsNullOrEmpty(CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator);
			if (!String.IsNullOrEmpty(DefaultTimeString))
				tpExt.DefaultTimeValue = DefaultTimeString;

			if (!String.IsNullOrEmpty(DateCssClass))
				txtDate.CssClass = DateCssClass;
			if (!String.IsNullOrEmpty(TimeCssClass))
				txtTime.CssClass = TimeCssClass;
			if (ReadOnly)
			{
				txtDate.ReadOnly = true;
				txtTime.ReadOnly = true;
			}

			//Enabled
			txtDate.Enabled = Enabled;
			txtTime.Enabled = Enabled;
			tpExt.Enabled = Enabled;

			img1.ImageUrl = ResolveUrl(ButtonImageUrl);
			if (!this.ShowImageButton)
			{
				imgCell.Style.Add("display", "none");
				calendarButtonExtender.PopupButtonID = "txtDate";
			}
			else
			{
				imgCell.Style.Add("display", "");
				calendarButtonExtender.PopupButtonID = "img1";
			}

			if (this.SelectedMode != SelectedModeType.Day || !ShowTime)
				tdTime.Visible = false;

			if (AutoPostBack)
			{
				calendarButtonExtender.UpdateElementId = hfUpdateCalendar.ClientID;
				hfUpdateCalendar.ValueChanged += new EventHandler(hfUpdateCalendar_ValueChanged);
			}
			calendarButtonExtender.FirstDayOfWeek = (FirstDayOfWeek)PortalConfig.PortalFirstDayOfWeek;

			if (this.DateFormat != string.Empty)
				calendarButtonExtender.Format = this.DateFormat; //dd MMM yyyy

			//DV TODO: Plohoj metod  nado razbiratsya pochemu ne vosstanavlvaetsya sam
			if (String.IsNullOrEmpty(txtDate.Text))
			{
				if (this.Request.Form[this.txtDate.UniqueID] != null)
					txtDate.Text = this.Request.Form[this.txtDate.UniqueID];
			}
			////////////////////////////////////////////////////////////////////

			txtDate.Width = DateWidth;
			txtTime.Width = TimeWidth;

			this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString("N"),
				String.Format("<link rel='Stylesheet' href='{0}' type='text/css' />",
					ResolveClientUrl("~/styles/IbnFramework/calendar.css")));

			this.Page.LoadComplete += new EventHandler(Page_LoadComplete);
		}

		void Page_LoadComplete(object sender, EventArgs e)
		{
			if (TryLoadHiddenField() != string.Empty)
			{
				hfUpdateCalendar.Value = TryLoadHiddenField();

				if (hfUpdateCalendar.Value != string.Empty)
					this.OnValueChange(sender, e);
			}
		}

		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		//DV TODO: Plohoj metod nado razbiratsya pochemu ne vosstanavlvaetsya sam
		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		string TryLoadHiddenField()
		{
			if (hfUpdateCalendar.Value == string.Empty && this.Request.Form[this.hfUpdateCalendar.UniqueID] != null)
			{
				return this.Request.Form[this.hfUpdateCalendar.UniqueID];
			}
			else
			{
				return string.Empty;
			}
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
		}

		protected void hfUpdateCalendar_ValueChanged(object sender, EventArgs e)
		{
			//this.OnValueChange(sender, e);
		}

		protected void GetMonthViewLimits(DateTime selectedDate, out DateTime startDate, out DateTime endDate)
		{
			startDate = selectedDate.Date;
			endDate = selectedDate.Date;
			while (startDate.Day > 1)
			{
				startDate = startDate.AddDays(-1);
			}
			int sdow = PortalConfig.PortalFirstDayOfWeek;
			while ((int)startDate.DayOfWeek != sdow)
			{
				startDate = startDate.AddDays(-1);
			}
			endDate = startDate.AddDays(41);
		}
	}
}