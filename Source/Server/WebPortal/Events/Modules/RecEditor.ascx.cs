namespace Mediachase.UI.Web.Events.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Collections;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;

	/// <summary>
	///		Summary description for RecEditor.
	/// </summary>
	public partial class RecEditor : System.Web.UI.UserControl
	{
		#region Html Vars
		protected CustomValidator cvStartEndDate;
		protected RequiredFieldValidator rfDayReq, rfv;
		protected RangeValidator rfDayNZ, rv;
		#endregion

		public ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strRecEditor", typeof(RecEditor).Assembly);

		#region EventId
		private int EventId
		{
			get
			{
				try
				{
					return int.Parse(Request["EventId"]);
				}
				catch
				{
					throw new Exception("Not Valid ID");
				}
			}
		}
		#endregion

		#region SharedID
		private int SharedID
		{
			get
			{
				try
				{
					return int.Parse(Request["SharedId"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region StartTime
		private int TimeStart
		{
			get
			{
				return dtcTimeStart.SelectedDate.Hour * 60 + dtcTimeStart.SelectedDate.Minute;
			}
			set
			{
				dtcTimeStart.SelectedDate = DateTime.MinValue.AddMinutes(value);
			}
		}
		#endregion

		#region EndTime
		private int TimeEnd
		{
			get
			{
				return dtcTimeEnd.SelectedDate.Hour * 60 + dtcTimeEnd.SelectedDate.Minute;
			}
			set
			{
				dtcTimeEnd.SelectedDate = DateTime.MinValue.AddMinutes(value);
			}
		}
		#endregion

		#region Pattern
		public byte Pattern
		{
			get
			{
				for (byte i = 1; i <= 4; i++)
				{
					HtmlInputRadioButton rb = (HtmlInputRadioButton)this.FindControl("rbRecType" + i);
					if (rb != null)
						if (rb.Checked)
							return i;
				}
				return 0;
			}
			set
			{
				for (byte i = 1; i <= 4; i++)
				{
					HtmlInputRadioButton rb = (HtmlInputRadioButton)this.FindControl("rbRecType" + i);
					if (rb != null)
						rb.Checked = false;
				}
				HtmlInputRadioButton rbControl = (HtmlInputRadioButton)this.FindControl("rbRecType" + value);
				if (rbControl != null)
					rbControl.Checked = true;
			}
		}
		#endregion

		#region SubPattern
		private byte SubPattern
		{
			get
			{
				for (int i = 1; i <= 4; i++)
				{
					HtmlInputRadioButton rbControl = (HtmlInputRadioButton)this.FindControl("rbRecType" + i);
					if ((rbControl != null) && (rbControl.Checked))
					{
						for (byte j = 1; j <= 2; j++)
						{
							HtmlInputRadioButton rbSubControl = (HtmlInputRadioButton)this.FindControl("rbRecType" + i + j);
							if ((rbSubControl != null) && (rbSubControl.Checked))
								return j;
						}
						return 1;
					}
				}
				return 0;
			}
			set
			{
				if ((Pattern != 0) && (value != 0))
				{

					for (int i = 1; i <= 2; i++)
					{
						HtmlInputRadioButton rb = (HtmlInputRadioButton)this.FindControl("rbRecType" + Pattern + i);
						if (rb != null)
							rb.Checked = false;
					}
					HtmlInputRadioButton rbControl = (HtmlInputRadioButton)this.FindControl("rbRecType" + Pattern + value);
					if (rbControl != null)
						rbControl.Checked = true;
				}
			}
		}
		#endregion

		#region Frequency
		private byte Frequency
		{
			get
			{
				TextBox tbControl = (TextBox)this.FindControl("tbFreq" + Pattern);
				if (tbControl == null)
					tbControl = (TextBox)this.FindControl("tbFreq" + Pattern + SubPattern);
				if ((tbControl != null) && (tbControl.Text != ""))
					return byte.Parse(tbControl.Text);
				return 0;
			}
			set
			{
				TextBox tbControl = (TextBox)this.FindControl("tbFreq" + Pattern);
				if (tbControl == null)
					tbControl = (TextBox)this.FindControl("tbFreq" + Pattern + SubPattern);
				if (tbControl != null)
					tbControl.Text = value.ToString();
			}
		}
		#endregion

		#region Weekdays
		private byte Weekdays
		{
			get
			{
				Control cControl = this.FindControl("Weekday" + Pattern);
				if (cControl == null)
					cControl = this.FindControl("Weekday" + Pattern + SubPattern);
				if (cControl != null)
				{
					if (cControl.GetType().Name == "HtmlGenericControl")
					{
						byte retval = 0;
						foreach (CalendarEntry.BitDayOfWeek day in Enum.GetValues(typeof(CalendarEntry.BitDayOfWeek)))
						{
							string num = ((int)day).ToString();
							string chbName = "day" + num;
							HtmlInputCheckBox chbControl = (HtmlInputCheckBox)cControl.FindControl(chbName);
							if ((chbControl != null) && (chbControl.Checked))
								retval += (byte)day;
						}
						return retval;
					}
					else
						if (cControl.GetType().Name == "DropDownList")
							return byte.Parse(((DropDownList)cControl).SelectedItem.Value);
				}
				return 0;
			}
			set
			{

				Control cControl = this.FindControl("Weekday" + Pattern);
				if (cControl == null)
					cControl = this.FindControl("Weekday" + Pattern + SubPattern);
				if (cControl != null)
				{
					if (cControl.GetType().Name == "HtmlGenericControl")
					{
						foreach (CalendarEntry.BitDayOfWeek day in Enum.GetValues(typeof(CalendarEntry.BitDayOfWeek)))
						{
							// O.R. [2008-11-25]: Reset checked elements if they are not in value
							string num = ((int)day).ToString();
							string chbName = "day" + num;
							HtmlInputCheckBox chbControl = (HtmlInputCheckBox)cControl.FindControl(chbName);

							if (chbControl != null)
							{
								if (((value & (byte)day) == (byte)day) && (day != CalendarEntry.BitDayOfWeek.Unknown))
									chbControl.Checked = true;
								else
									chbControl.Checked = false;
							}
						}
					}
					else if (cControl.GetType().Name == "DropDownList")
					{
						DropDownList ddlWeekdays = (DropDownList)cControl;
						ddlWeekdays.ClearSelection();
						ListItem lItem = ddlWeekdays.Items.FindByValue(value.ToString());
						if (lItem != null)
							lItem.Selected = true;
					}
				}
			}
		}
		#endregion

		#region MonthDay
		private byte MonthDay
		{
			get
			{
				TextBox tbControl = (TextBox)this.FindControl("tbMonthDay" + Pattern);
				if (tbControl == null)
					tbControl = (TextBox)this.FindControl("tbMonthDay" + Pattern + SubPattern);
				if (tbControl != null)
					if (tbControl.Text != "")
						return byte.Parse(tbControl.Text);
				return 0;
			}
			set
			{
				TextBox tbControl = (TextBox)this.FindControl("tbMonthDay" + Pattern);
				if (tbControl == null)
					tbControl = (TextBox)this.FindControl("tbMonthDay" + Pattern + SubPattern);
				if (tbControl != null)
					tbControl.Text = value.ToString();
			}
		}

		#endregion

		#region WeekNumber
		private byte WeekNumber
		{
			get
			{
				DropDownList ddlControl = (DropDownList)this.FindControl("WeekNumber" + Pattern + SubPattern);
				if (ddlControl != null)
					return byte.Parse(ddlControl.SelectedItem.Value);
				return 0;
			}
			set
			{
				DropDownList ddlControl = (DropDownList)this.FindControl("WeekNumber" + Pattern + SubPattern);
				if (ddlControl != null)
				{
					ListItem lItem = ddlControl.Items.FindByValue(value.ToString());
					if (lItem != null)
					{
						ddlControl.ClearSelection();
						lItem.Selected = true;
					}
				}
			}
		}
		#endregion

		#region MonthNumber
		private byte MonthNumber
		{
			get
			{
				DropDownList ddlControl = (DropDownList)this.FindControl("MonthNumber" + Pattern + SubPattern);
				if (ddlControl != null)
					return byte.Parse(ddlControl.SelectedItem.Value);
				return 0;
			}
			set
			{
				DropDownList ddlControl = (DropDownList)this.FindControl("MonthNumber" + Pattern + SubPattern);
				if (ddlControl != null)
				{
					ListItem lItem = ddlControl.Items.FindByValue(value.ToString());
					if (lItem != null)
					{
						ddlControl.ClearSelection();
						lItem.Selected = true;
					}
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			BindToolbar();
			// Put user code to initialize the page here
			if (!Page.IsPostBack)
				BindValues();
		}

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tbEdit");
			secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("tbsave_gotolist"), "../Calendar/");
			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");

			Legend1.InnerText = LocRM.GetString("TimeSection");
			Legend2.InnerText = LocRM.GetString("PatternSection");
			Legend3.InnerText = LocRM.GetString("RangeSection");
		}
		#endregion

		#region SetDefaultValues
		private void SetDefaulValues()
		{
			tbEndAfter.Text = "1";
			rbEndBy.Checked = false;
			rbEndAfter.Checked = true;

			using (IDataReader reader = CalendarEntry.GetEventDates(EventId))
			{
				if (reader.Read())
				{
					dtcDateStart.SelectedDate = (DateTime)reader["StartDate"];
					dtcDateEnd.SelectedDate = (DateTime)reader["FinishDate"];
					dtcTimeStart.SelectedDate = (DateTime)reader["StartDate"];
					dtcTimeEnd.SelectedDate = (DateTime)reader["FinishDate"];
				}
				else
				{
					dtcTimeStart.SelectedDate = UserDateTime.UserNow;
					dtcTimeEnd.SelectedDate = UserDateTime.UserNow.AddHours(1);
				}
			}

			for (byte i = 4; i >= 1; i--)
			{
				Pattern = i;
				for (byte j = 2; j >= 1; j--)
				{
					SubPattern = j;
					Frequency = 5;
					Weekdays = (byte)CalendarEntry.GetBitDayOfWeek(UserDateTime.UserNow.DayOfWeek);
					MonthDay = (byte)UserDateTime.UserNow.Day;
					WeekNumber = (byte)(UserDateTime.UserNow.Day / 7 + 1);
					MonthNumber = (byte)UserDateTime.UserNow.Month;
				}
			}

		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			lstTimeZone.DataSource = User.GetListTimeZone();
			lstTimeZone.DataTextField = "DisplayName";
			lstTimeZone.DataValueField = "TimeZoneId";
			lstTimeZone.DataBind();
			string sTimeZoneId = Security.CurrentUser.TimeZoneId.ToString();
			ListItem[] liArray = new ListItem[5];
			liArray[0] = new ListItem(LocRM.GetString("First"), "1");
			liArray[1] = new ListItem(LocRM.GetString("Second"), "2");
			liArray[2] = new ListItem(LocRM.GetString("Third"), "3");
			liArray[3] = new ListItem(LocRM.GetString("Fourth"), "4");
			liArray[4] = new ListItem(LocRM.GetString("Last"), "8");
			WeekNumber32.Items.AddRange(liArray);
			WeekNumber42.Items.AddRange(liArray);

			liArray = new ListItem[7];
			int k = 0;
			foreach (System.DayOfWeek day in Enum.GetValues(typeof(System.DayOfWeek)))
			{
				liArray[k] = new ListItem(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)day], ((int)CalendarEntry.GetBitDayOfWeek(day)).ToString());
				k++;
			}

			Weekday32.Items.AddRange(liArray);
			Weekday42.Items.AddRange(liArray);

			liArray = new ListItem[12];
			for (int i = 1; i <= 12; i++)
				liArray[i - 1] = new ListItem(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i), i.ToString());
			MonthNumber41.Items.AddRange(liArray);
			MonthNumber42.Items.AddRange(liArray);
			SetDefaulValues();
			if (EventId != 0)
			{
				using (IDataReader reader = CalendarEntry.GetRecurrence(EventId))
				{
					///		RecurrenceId, ObjectTypeId, ObjectId, StartTime, 
					///		EndTime, Pattern, SubPattern, Frequency, Weekdays, 
					///		DayOfMonth, WeekNumber, MonthNumber, EndAfter 
					if (reader.Read())
					{
						TimeStart = (int)reader["StartTime"];
						TimeEnd = (int)reader["EndTime"];
						Pattern = (byte)reader["Pattern"];
						SubPattern = (byte)reader["SubPattern"];
						Frequency = (byte)reader["Frequency"];
						Weekdays = (byte)reader["Weekdays"];
						MonthDay = (byte)reader["DayOfMonth"];
						WeekNumber = (byte)reader["WeekNumber"];
						MonthNumber = (byte)reader["MonthNumber"];
						int iEndAfter = (int)reader["EndAfter"];
						if (iEndAfter != 0)
						{
							rbEndBy.Checked = false;
							rbEndAfter.Checked = true;
							tbEndAfter.Text = iEndAfter.ToString();
						}
						else
						{
							rbEndAfter.Checked = false;
							rbEndBy.Checked = true;
						}
						if (reader["TimeZoneId"] != DBNull.Value)
							sTimeZoneId = reader["TimeZoneId"].ToString();
					}
				}
			}
			ListItem lItem = lstTimeZone.Items.FindByValue(sTimeZoneId);
			if (lItem != null)
			{
				lstTimeZone.ClearSelection();
				lItem.Selected = true;
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
			this.cvDates.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.cvStartEndDate_Validate);
			this.cvWeek.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.cvWeek_Validate);
			this.cvDate2.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.cvDate2_Validate);

		}
		#endregion

		#region btnSave_ServerClick
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			SwitchValidators();

			Page.Validate();
			if (!Page.IsValid) return;

			if (EventId != 0)
			{
				int iEndAfter = 0;
				if (rbEndAfter.Checked)
					iEndAfter = int.Parse(tbEndAfter.Text);
				// TODO: вместо Security.CurrentUser.TimeOffset использовать значение, выбранное из списка!
				CalendarEntry.AddRecurrence(EventId, TimeStart, TimeEnd, Pattern,
					SubPattern, Frequency, Weekdays, MonthDay,
					WeekNumber, MonthNumber, iEndAfter, dtcDateStart.SelectedDate,
					dtcDateEnd.SelectedDate, int.Parse(lstTimeZone.SelectedItem.Value));
				String slink = String.Empty;
				if (SharedID > 0) slink = "&SharedId=" + SharedID;

				Response.Redirect("../Events/EventView.aspx?EventId=" + EventId + slink);
			}
		}
		#endregion

		#region SwitchValidators
		private void SwitchValidators()
		{
			switch (Pattern)
			{
				case 1:
					rfvDay.Enabled = true;
					rvDay.Enabled = true;

					rfvWeek.Enabled = false;
					rvWeek.Enabled = false;
					cvWeek.Enabled = false;

					rvMonth1.Enabled = false;
					rfvMonth1.Enabled = false;
					rvMonth2.Enabled = false;
					rfvMonth2.Enabled = false;
					rvMonth3.Enabled = false;
					rfvMonth3.Enabled = false;

					rvYear.Enabled = false;
					rfvYear.Enabled = false;

					break;
				case 2:
					rfvDay.Enabled = false;
					rvDay.Enabled = false;

					rfvWeek.Enabled = true;
					rvWeek.Enabled = true;
					cvWeek.Enabled = true;

					rvMonth1.Enabled = false;
					rfvMonth1.Enabled = false;
					rvMonth2.Enabled = false;
					rfvMonth2.Enabled = false;
					rvMonth3.Enabled = false;
					rfvMonth3.Enabled = false;

					rvYear.Enabled = false;
					rfvYear.Enabled = false;
					break;
				case 3:
					rfvDay.Enabled = false;
					rvDay.Enabled = false;

					rfvWeek.Enabled = false;
					rvWeek.Enabled = false;
					cvWeek.Enabled = false;

					if (rbRecType31.Checked)
					{
						rvMonth1.Enabled = true;
						rfvMonth1.Enabled = true;

						rvMonth2.Enabled = true;
						rfvMonth2.Enabled = true;

						rvMonth3.Enabled = false;
						rfvMonth3.Enabled = false;
					}
					else
					{
						rvMonth1.Enabled = false;
						rfvMonth1.Enabled = false;

						rvMonth2.Enabled = false;
						rfvMonth2.Enabled = false;

						rvMonth3.Enabled = true;
						rfvMonth3.Enabled = true;
					}

					rvYear.Enabled = false;
					rfvYear.Enabled = false;

					break;
				case 4:
					rfvDay.Enabled = false;
					rvDay.Enabled = false;

					rfvWeek.Enabled = false;
					rvWeek.Enabled = false;
					cvWeek.Enabled = false;

					rvMonth1.Enabled = false;
					rfvMonth1.Enabled = false;
					rvMonth2.Enabled = false;
					rfvMonth2.Enabled = false;
					rvMonth3.Enabled = false;
					rfvMonth3.Enabled = false;

					if (rbRecType41.Checked)
					{
						rvYear.Enabled = true;
						rfvYear.Enabled = true;
					}
					else
					{
						rvYear.Enabled = false;
						rfvYear.Enabled = false;
					}
					break;
			}

			if (rbEndAfter.Checked)
			{
				rvDate1.Enabled = true;
				rfvDate1.Enabled = true;
				cvDate2.Enabled = false;
			}
			else
			{
				rvDate1.Enabled = false;
				rfvDate1.Enabled = false;
				cvDate2.Enabled = true;
			}
		}
		#endregion

		#region btnCancel_ServerClick
		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			String slink = String.Empty;
			if (SharedID > 0) slink = "&SharedId=" + SharedID;

			Response.Redirect("../Events/EventView.aspx?EventId=" + EventId + slink);
		}
		#endregion

		#region cvStartEndDate_Validate
		private void cvStartEndDate_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			if (TimeStart > TimeEnd) args.IsValid = false;
		}
		#endregion

		#region cvWeek_Validate
		private void cvWeek_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			if (Weekdays == 0) args.IsValid = false;
		}
		#endregion

		#region cvDate2_Validate
		private void cvDate2_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			if (dtcDateStart.SelectedDate > dtcDateEnd.SelectedDate)
				args.IsValid = false;
		}
		#endregion

		//===========================================================================
		// This public property was added by conversion wizard to allow the access of a protected, autogenerated member variable secHeader.
		//===========================================================================
		public Mediachase.UI.Web.Modules.BlockHeader secHeader
		{
			get { return Migrated_secHeader; }
			//set { Migrated_secHeader = value; }
		}
	}
}
