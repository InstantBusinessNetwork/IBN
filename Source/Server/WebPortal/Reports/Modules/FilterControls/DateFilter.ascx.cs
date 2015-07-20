using System;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.Interfaces;


namespace Mediachase.UI.Web.Reports.Modules
{
	/// <summary>
	///		Summary description for DateFilter.
	/// </summary>
	public partial class DateFilter : System.Web.UI.UserControl, IFilterControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strXMLReport", typeof(DateFilter).Assembly);
		protected void Page_Load(object sender, System.EventArgs e)
		{
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			if (ddPeriod.Value == "9")
				tableDate.Style.Add("display", "block");
			else
				tableDate.Style.Add("display", "none");
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
		}
		#endregion

		#region Implementation of IFilterControl
		public object Value
		{
			set
			{
				if (value != null)
				{
					DateFilterValue dtValue = (DateFilterValue)value;
					if (dtValue.TypeValue == "9")
					{
						if (DateTime.Parse(dtValue.FirstValue) != DateTime.MinValue)
						{
							//fromDate.Text = DateTime.Parse(dtValue.FirstValue).ToShortDateString();
							dtcFromDate.SelectedDate = DateTime.Parse(dtValue.FirstValue);
						}
						else
						{
							//fromDate.Text = "";
						}
						if (DateTime.Parse(dtValue.SecondValue) < DateTime.MaxValue.Date)
							//toDate.Text = DateTime.Parse(dtValue.SecondValue).ToShortDateString();
							dtcToDate.SelectedDate = DateTime.Parse(dtValue.SecondValue);
						else
							//toDate.Text = "";
							tableDate.Style.Add("display", "block");
					}
					else
						tableDate.Style.Add("display", "none");
					ddPeriod.Value = dtValue.TypeValue;
				}
				else
				{
					ddPeriod.Value = "0";
					tableDate.Style.Add("display", "none");
				}
			}
			get
			{
				DateTime _Start = DateTime.MinValue;
				DateTime _Finish = DateTime.MaxValue;
				//GetDates(ddPeriod.Value, out _Start, out _Finish, fromDate.Text, toDate.Text);
				GetDates(ddPeriod.Value, out _Start, out _Finish, dtcFromDate.SelectedDate.ToShortDateString(), dtcToDate.SelectedDate.ToShortDateString());
				object retval = null;
				if (ddPeriod.Value == "9" && _Start != DateTime.MinValue && _Finish != DateTime.MaxValue)
				{
					retval = new DateFilterValue(ddPeriod.Value,
						_Start.ToString("yyyy-MM-ddTHH:mm:ss"), _Finish.ToString("yyyy-MM-ddTHH:mm:ss"));
				}
				else if (ddPeriod.Value != "0" && ddPeriod.Value != "9")
				{
					retval = new DateFilterValue(ddPeriod.Value, "", "");
				}
				return retval;
			}
		}

		public string FilterTitle
		{
			set
			{
				lblTitle.Text = value;
			}
			get
			{
				return lblTitle.Text;
			}
		}

		private string filterField = "";
		public string FilterField
		{
			set
			{
				filterField = value;
			}
			get
			{
				return filterField;
			}
		}

		public string FilterType
		{
			get
			{
				return "DateTime";
			}
		}

		public void InitControl(object reader)
		{
			ddPeriod.Items.Clear();
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tAny"), "0"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tToday"), "1"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tYesterday"), "2"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tThisWeek"), "3"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tLastWeek"), "4"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tThisMonth"), "5"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tLastMonth"), "6"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tThisYear"), "7"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tLastYear"), "8"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tCustom"), "9"));
			//fromDate.Text = UserDateTime.UserToday.AddMonths(-1).ToShortDateString();
			dtcFromDate.SelectedDate = UserDateTime.UserToday.AddMonths(-1);
			//toDate.Text = UserDateTime.UserNow.ToShortDateString();
			dtcToDate.SelectedDate = UserDateTime.UserNow;
			tableDate.Style.Add("display", "none");
		}
		#endregion

		#region GetDates
		private void GetDates(string _value, out DateTime _start, out DateTime _finish, string fromCustom, string toCustom)
		{
			switch (_value)
			{
				case "1":	//Today
					_start = UserDateTime.UserToday;
					_finish = UserDateTime.UserToday.AddDays(1);
					break;
				case "2":	//Yesterday
					_start = UserDateTime.UserToday.AddDays(-1);
					_finish = UserDateTime.UserToday;
					break;
				case "3":	//ThisWeek
					_start = UserDateTime.UserToday.AddDays(1 - (int)DateTime.Now.DayOfWeek);
					_finish = UserDateTime.UserToday.AddDays(1);
					break;
				case "4":	//LastWeek
					_start = UserDateTime.UserToday.AddDays(1 - (int)DateTime.Now.DayOfWeek - 7);
					_finish = UserDateTime.UserToday.AddDays(1 - (int)DateTime.Now.DayOfWeek);
					break;
				case "5":	//ThisMonth
					_start = UserDateTime.UserToday.AddDays(1 - DateTime.Now.Day);
					_finish = UserDateTime.UserToday.AddDays(1);
					break;
				case "6":	//LastMonth
					_start = UserDateTime.UserToday.AddDays(1 - DateTime.Now.Day).AddMonths(-1);
					_finish = UserDateTime.UserToday.AddDays(1 - DateTime.Now.Day);
					break;
				case "7":	//ThisYear
					_start = UserDateTime.UserToday.AddDays(1 - DateTime.Now.DayOfYear);
					_finish = UserDateTime.UserToday.AddDays(1);
					break;
				case "8":	//LastYear
					_start = UserDateTime.UserToday.AddDays(1 - DateTime.Now.DayOfYear).AddYears(-1);
					_finish = UserDateTime.UserToday.AddDays(1 - DateTime.Now.DayOfYear);
					break;
				case "9":	//Custom
					try
					{
						_start = DateTime.Parse(fromCustom);
					}
					catch
					{
						_start = DateTime.MinValue;
					}
					try
					{
						_finish = DateTime.Parse(toCustom);
					}
					catch
					{
						_finish = DateTime.MaxValue;
					}
					break;
				default:
					_start = DateTime.MinValue;
					_finish = DateTime.MaxValue;
					break;
			}
		}
		#endregion

		//===========================================================================
		// This public property was added by conversion wizard to allow the access of a protected, autogenerated member variable tdTitle.
		//===========================================================================
		public System.Web.UI.HtmlControls.HtmlTableCell tdTitle
		{
			get { return Migrated_tdTitle; }
			//set { Migrated_tdTitle = value; }
		}
	}
}
