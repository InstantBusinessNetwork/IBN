using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;
using System.Reflection;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Events;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Calendar.Modules
{
	public partial class ReccurenceEdit : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strRecEditor", Assembly.GetExecutingAssembly());

		#region EventId
		private PrimaryKeyId EventId
		{
			get
			{
				try
				{
					return PrimaryKeyId.Parse(Request["EventId"]);
				}
				catch
				{
					throw new Exception("Not Valid ID");
				}
			}
		}
		#endregion

		#region Pattern
		public byte Pattern
		{
			get
			{
				for (byte i = 1; i <= 6; i++)
				{
					HtmlInputRadioButton rb = (HtmlInputRadioButton)this.FindControl("rbRecType" + i);
					if (rb != null)
						if (rb.Checked)
						{
							if (i == 3)
							{
								for (byte j = 3; j <= 4; j++)
								{
									HtmlInputRadioButton rbSubControl = (HtmlInputRadioButton)this.FindControl(String.Format("rbRecType{0}1", j));
									if ((rbSubControl != null) && (rbSubControl.Checked))
										return j;
								}
							}
							if (i == 5)
							{
								for (byte j = 5; j <= 6; j++)
								{
									HtmlInputRadioButton rbSubControl = (HtmlInputRadioButton)this.FindControl(String.Format("rbRecType{0}1", j));
									if ((rbSubControl != null) && (rbSubControl.Checked))
										return j;
								}
							}
							return i;
						}
				}
				return 0;
			}
			set
			{
				for (byte i = 1; i <= 6; i++)
				{
					HtmlInputRadioButton rb = (HtmlInputRadioButton)this.FindControl("rbRecType" + i);
					if (rb != null)
						rb.Checked = false;
				}

				HtmlInputRadioButton rbControl = (HtmlInputRadioButton)this.FindControl("rbRecType" + value);
				if (rbControl != null)
					rbControl.Checked = true;
				else
				{
					rbControl = (HtmlInputRadioButton)this.FindControl("rbRecType" + (value - 1).ToString());
					if (rbControl != null)
						rbControl.Checked = true;
				}

				if (value == 3 || value == 4)
				{
					for (byte j = 3; j <= 4; j++)
					{
						rbControl = (HtmlInputRadioButton)this.FindControl(String.Format("rbRecType{0}1", j));
						if (rbControl != null)
							rbControl.Checked = j == value;
					}
				}

				if (value == 5 || value == 6)
				{
					for (byte j = 5; j <= 6; j++)
					{
						rbControl = (HtmlInputRadioButton)this.FindControl(String.Format("rbRecType{0}1", j));
						if (rbControl != null)
							rbControl.Checked = j == value;
					}
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
					tbControl = (TextBox)this.FindControl("tbFreq" + Pattern);
				if ((tbControl != null) && (tbControl.Text != ""))
					return byte.Parse(tbControl.Text);
				return 0;
			}
			set
			{
				TextBox tbControl = (TextBox)this.FindControl("tbFreq" + Pattern);
				if (tbControl == null)
					tbControl = (TextBox)this.FindControl("tbFreq" + Pattern);
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
					cControl = this.FindControl("Weekday" + Pattern);
				if (cControl != null)
				{
					if (cControl.GetType().Name == "HtmlGenericControl")
					{
						byte retval = 0;
						foreach (eBitDayOfWeek day in Enum.GetValues(typeof(eBitDayOfWeek)))
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
							return byte.Parse(((DropDownList)cControl).SelectedValue);
				}
				return 0;
			}
			set
			{

				Control cControl = this.FindControl("Weekday" + Pattern);
				if (cControl == null)
					cControl = this.FindControl("Weekday" + Pattern);
				if (cControl != null)
				{
					if (cControl.GetType().Name == "HtmlGenericControl")
					{
						foreach (eBitDayOfWeek day in Enum.GetValues(typeof(eBitDayOfWeek)))
						{
							// O.R. [2008-11-25]: Reset checked elements if they are not in value
							string num = ((int)day).ToString();
							string chbName = "day" + num;
							HtmlInputCheckBox chbControl = (HtmlInputCheckBox)cControl.FindControl(chbName);

							if (chbControl != null)
							{
								if (((value & (byte)day) == (byte)day) && (day != eBitDayOfWeek.Unknown))
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
				if (tbControl != null)
					if (!String.IsNullOrEmpty(tbControl.Text))
						return byte.Parse(tbControl.Text);
				return 0;
			}
			set
			{
				TextBox tbControl = (TextBox)this.FindControl("tbMonthDay" + Pattern);
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
				DropDownList ddlControl = (DropDownList)this.FindControl("WeekNumber" + Pattern);
				if (ddlControl != null)
					return byte.Parse(ddlControl.SelectedValue);
				return 0;
			}
			set
			{
				DropDownList ddlControl = (DropDownList)this.FindControl("WeekNumber" + Pattern);
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
				DropDownList ddlControl = (DropDownList)this.FindControl("MonthNumber" + Pattern);
				if (ddlControl != null)
					return byte.Parse(ddlControl.SelectedValue);
				return 0;
			}
			set
			{
				DropDownList ddlControl = (DropDownList)this.FindControl("MonthNumber" + Pattern);
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

		protected void Page_Load(object sender, EventArgs e)
		{
			ApplyLocalization();
			if (!Page.IsPostBack)
				BindValues();
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			secHeader.Title = LocRM.GetString("PatternSection");
			secHeader2.Title = LocRM.GetString("RangeSection");

			btnSave.Text = GetGlobalResourceObject("IbnFramework.Common", "btnOK").ToString();
			btnSave.CustomImage = this.Page.ResolveUrl("~/Layouts/images/saveitem.gif");
			btnCancel.Text = GetGlobalResourceObject("IbnFramework.Common", "btnCancel").ToString();
			btnCancel.CustomImage = this.Page.ResolveUrl("~/Layouts/images/cancel.gif");
			btnCancel.Attributes.Add("onclick", CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true));
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			BindLists();

			SetDefaulValues();

			CalendarEventEntity ceo = BusinessManager.Load(CalendarEventEntity.ClassName, EventId) as CalendarEventEntity;
			if (ceo != null)
			{
				dtcDateStart.SelectedDate = ceo.Start;
				dtcDateEnd.SelectedDate = ceo.End;

				FilterElementCollection fec = new FilterElementCollection();
				FilterElement fe = FilterElement.EqualElement("EventId", ((VirtualEventId)EventId).RealEventId);
				fec.Add(fe);
				EntityObject[] list = BusinessManager.List(CalendarEventRecurrenceEntity.ClassName, fec.ToArray());
				if (list.Length > 0)
				{
					CalendarEventRecurrenceEntity cero = (CalendarEventRecurrenceEntity)list[0];

					int pattern = cero.RecurrenceType;
					if (cero.RecurrenceType == (int)eRecurrenceType.RecursWeekly &&
							cero.DayOfWeekMask.HasValue &&
							cero.DayOfWeekMask.Value == (int)eBitDayOfWeek.Weekdays)
						pattern = 1;

					Pattern = (byte)pattern;

					if (cero.Interval.HasValue)
						Frequency = (byte)cero.Interval.Value;
					if (cero.DayOfWeekMask.HasValue)
						Weekdays = (byte)cero.DayOfWeekMask;
					if (cero.DayOfMonth.HasValue)
						MonthDay = (byte)cero.DayOfMonth.Value;
					if (cero.Instance.HasValue)
						WeekNumber = (byte)cero.Instance.Value;
					if (cero.MonthOfYear.HasValue)
						MonthNumber = (byte)cero.MonthOfYear.Value;

					if (cero.PatternEndDate.HasValue)
					{
						rbEndBy.Checked = true;
						rbEndAfter.Checked = false;
						dtcDateEnd.SelectedDate = cero.PatternEndDate.Value.Date;
					}
					else if (cero.Occurrences.HasValue && cero.Occurrences.Value > 0)
					{
						rbEndAfter.Checked = true;
						tbEndAfter.Text = cero.Occurrences.Value.ToString();
					}
				}
			}
		}
		#endregion

		#region BindLists
		private void BindLists()
		{
			ListItem[] liArray = new ListItem[5];
			liArray[0] = new ListItem(LocRM.GetString("First"), ((int)eInstanceType.InstanceFirst).ToString());
			liArray[1] = new ListItem(LocRM.GetString("Second"), ((int)eInstanceType.InstanceSecond).ToString());
			liArray[2] = new ListItem(LocRM.GetString("Third"), ((int)eInstanceType.InstanceThird).ToString());
			liArray[3] = new ListItem(LocRM.GetString("Fourth"), ((int)eInstanceType.InstanceFour).ToString());
			liArray[4] = new ListItem(LocRM.GetString("Last"), ((int)eInstanceType.InstanceLast).ToString());
			WeekNumber4.Items.AddRange(liArray);
			WeekNumber6.Items.AddRange(liArray);

			liArray = new ListItem[7];
			int k = 0;
			foreach (System.DayOfWeek day in Enum.GetValues(typeof(System.DayOfWeek)))
			{
				liArray[k] = new ListItem(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)day], ((int)GetBitDayOfWeek(day)).ToString());
				k++;
			}

			Weekday4.Items.AddRange(liArray);
			Weekday6.Items.AddRange(liArray);

			liArray = new ListItem[12];
			for (int i = 1; i <= 12; i++)
				liArray[i - 1] = new ListItem(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i), i.ToString());
			MonthNumber5.Items.AddRange(liArray);
			MonthNumber6.Items.AddRange(liArray);
		}
		#endregion

		#region SetDefaultValues
		private void SetDefaulValues()
		{
			tbEndAfter.Text = "2";
			rbEndBy.Checked = false;
			rbEndAfter.Checked = true;

			for (byte i = 6; i >= 1; i--)
			{
				Pattern = i;
				Frequency = 1;
				Weekdays = (byte)GetBitDayOfWeek(Mediachase.IBN.Business.UserDateTime.UserNow.DayOfWeek);
				MonthDay = (byte)Mediachase.IBN.Business.UserDateTime.UserNow.Day;
				WeekNumber = (byte)(Mediachase.IBN.Business.UserDateTime.UserNow.Day / 7 + 1);
				MonthNumber = (byte)Mediachase.IBN.Business.UserDateTime.UserNow.Month;
			}
		}
		#endregion

		#region btnSave_ServerClick
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			CalendarEventEntity ceo = BusinessManager.Load(CalendarEventEntity.ClassName, EventId) as CalendarEventEntity;
			if (ceo != null)
			{
				CalendarEventRecurrenceEntity cero;
				bool create = false;

				FilterElementCollection fec = new FilterElementCollection();
				FilterElement fe = FilterElement.EqualElement("EventId", ((VirtualEventId)EventId).RealEventId);
				fec.Add(fe);
				EntityObject[] list = BusinessManager.List(CalendarEventRecurrenceEntity.ClassName, fec.ToArray());
				if (list.Length > 0)
					cero = (CalendarEventRecurrenceEntity)list[0];
				else
				{
					cero = BusinessManager.InitializeEntity<CalendarEventRecurrenceEntity>(CalendarEventRecurrenceEntity.ClassName);
					create = true;
				}

				cero.EventId = ((VirtualEventId)EventId).RealEventId;
				cero.RecurrenceType = (int)Pattern;
				switch (Pattern)
				{
					case 1:	//RecursDaily
						if (rbRecType11.Checked)
							cero.Interval = (int)Frequency;
						else
						{
							cero.RecurrenceType = (int)eRecurrenceType.RecursWeekly;
							cero.DayOfWeekMask = (int)eBitDayOfWeek.Weekdays;
						}
						break;
					case 2:	//RecursWeekly
						cero.DayOfWeekMask = (int)Weekdays;
						cero.Interval = (int)Frequency;
						break;
					case 3:	//RecursMonthly
						cero.DayOfMonth = (int)MonthDay;
						cero.Interval = (int)Frequency;
						break;
					case 4:	//RecursMonthNth
						cero.Instance = (int)WeekNumber;
						cero.DayOfWeekMask = (int)Weekdays;
						cero.Interval = (int)Frequency;
						break;
					case 5:	//RecursYearly
						cero.MonthOfYear = (int)MonthNumber;
						cero.DayOfMonth = (int)MonthDay;
						break;
					case 6:	//RecursYearNth
						cero.Instance = (int)WeekNumber;
						cero.DayOfWeekMask = (int)Weekdays;
						cero.MonthOfYear = (int)MonthNumber;
						break;
					default:
						break;
				}

				cero.Title = ceo.Subject;
				if (rbEndBy.Checked)
				{
					cero.PatternEndDate = dtcDateEnd.SelectedDate.AddDays(1).AddSeconds(-1);
					cero.Occurrences = 0;
				}
				else
				{
					cero.Occurrences = int.Parse(tbEndAfter.Text);
					cero.PatternEndDate = null;
				}

				if (create)
					BusinessManager.Create(cero);
				else
					BusinessManager.Update(cero);

				CommandParameters cp = new CommandParameters("CEvent_ReccurenceAdded");
				CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString(), true);
			}
		}
		#endregion


		#region GetBitDayOfWeek
		public static eBitDayOfWeek GetBitDayOfWeek(DayOfWeek day)
		{
			switch (day)
			{
				case DayOfWeek.Monday:
					return eBitDayOfWeek.Monday;
				case DayOfWeek.Tuesday:
					return eBitDayOfWeek.Tuesday;
				case DayOfWeek.Wednesday:
					return eBitDayOfWeek.Wednesday;
				case DayOfWeek.Thursday:
					return eBitDayOfWeek.Thursday;
				case DayOfWeek.Friday:
					return eBitDayOfWeek.Friday;
				case DayOfWeek.Saturday:
					return eBitDayOfWeek.Saturday;
				case DayOfWeek.Sunday:
					return eBitDayOfWeek.Sunday;
			}
			return eBitDayOfWeek.Unknown;
		}
		#endregion
	}
}