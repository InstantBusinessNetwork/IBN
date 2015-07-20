namespace Mediachase.UI.Web.Events.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using Mediachase.UI.Web.Util;
	 

	/// <summary>
	///		Summary description for Recurrence.
	/// </summary>
	public partial  class Recurrence : System.Web.UI.UserControl
	{
		public int EndAfter;

    public ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strRecEditor", typeof(Recurrence).Assembly);

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
					throw new Exception("Invalid Event ID!");
				}
			}
		}
		#endregion

		#region SharedID
		private int SharedID
		{
			get 
			{
				return CommonHelper.GetRequestInteger(Request, "SharedId", -1);
			}
		}
		#endregion

		#region Weekdays
		public byte Pattern;
		public byte SubPattern;
		public byte Frequency;
		private byte bWeekdays;
		public string Weekdays
		{
			get
			{
				string str = "";
				foreach (CalendarEntry.BitDayOfWeek day in Enum.GetValues(typeof(CalendarEntry.BitDayOfWeek)))
					if(((bWeekdays & (byte)day) == (byte)day) && (day != CalendarEntry.BitDayOfWeek.Unknown) && (day != CalendarEntry.BitDayOfWeek.Alldays) && (day != CalendarEntry.BitDayOfWeek.Weekdays))
					{
						str += System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(CalendarEntry.GetDayOfWeekByBit(day)).ToLower();
						str += ", ";
					}			
				if(str.Length > 2)
					str = str.Substring(0,str.Length-2);
				return str;
			}
		}
		#endregion

		#region WeekNumber
		public byte MonthDay;
		private byte bWeekNumber;
		public string WeekNumber
		{
			get
			{
				switch(bWeekNumber)
				{
					case 1:
						return LocRM.GetString("First").ToLower();
					case 2:
						return LocRM.GetString("Second").ToLower();
					case 3:
						return LocRM.GetString("Third").ToLower();
					case 4:
						return LocRM.GetString("Fourth").ToLower();
					case 8:
						return LocRM.GetString("Last").ToLower();
					default:
						return "";
				}
			}
		}
		#endregion

		#region MonthName
		private byte MonthNumber;
		public string MonthName
		{
			get
			{
				if(MonthNumber>0)
					return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(MonthNumber);
				return "";
			}
		}
		#endregion
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.Visible = true;
		}
		
		#region BindToolbar()
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("Recurrence"));
			if (CalendarEntry.CanUpdate(EventId))
			{
				String slink = String.Empty;
				if (SharedID>0)
					slink="&SharedId=" + SharedID;

				secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/recurrence.gif'/> " + LocRM.GetString("Modify"),"../Events/RecEditor.aspx?EventId=" + EventId + slink);
				if (ShowRecurrence.Visible)
				{
					secHeader.AddRightLink("<img alt='' src='../Layouts/Images/delete.gif'/> " + LocRM.GetString("Delete"),"javascript: document.forms[0]." + btnDelete.ClientID + ".click()");
					btnDelete.Attributes.Add("onclick","return confirm('"+ LocRM.GetString("Warning") +"');");
				}
			}
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			using (IDataReader reader = CalendarEntry.GetRecurrence(EventId))
			{
				if(reader.Read())
				{
					lblNoRecurrence.Visible = false;
					ShowRecurrence.Visible = true;
					DateTime dt = DateTime.MinValue.AddMinutes((int)reader["StartTime"]);
					txtStartTime.Text = dt.ToShortTimeString();
					dt = DateTime.MinValue.AddMinutes((int)reader["EndTime"]);
					txtEndTime.Text = dt.ToShortTimeString();
					Pattern = (byte)reader["Pattern"];
					SubPattern = (byte)reader["SubPattern"];
					Frequency = (byte)reader["Frequency"];
					bWeekdays = (byte)reader["Weekdays"];
					MonthDay = (byte)reader["DayOfMonth"];
					bWeekNumber = (byte)reader["WeekNumber"];
					MonthNumber = (byte)reader["MonthNumber"];
					EndAfter = (int)reader["EndAfter"];
					Label lblControl = (Label)this.FindControl("txtInfo" + Pattern + SubPattern);
					if(lblControl != null)
						lblControl.Visible = true;
					if(EndAfter != 0)
					{
						lblEnd.Text = LocRM.GetString("Endafter") + ":";
						txtEnd.Text = EndAfter.ToString() + " " + LocRM.GetString("occurrences");
					}
					else 
					{
						using (IDataReader rdr = CalendarEntry.GetEvent(EventId))
						{
							if (rdr.Read())
							{
								lblEnd.Text = LocRM.GetString("Endby") + ":";
								txtEnd.Text = ((DateTime)rdr["FinishDate"]).ToShortDateString();
							}
						}
					}
					if(reader["TimeZoneId"] != DBNull.Value)
					{	
	
						TimeSpan Offset = new TimeSpan(0,-User.GetTimeZoneBias((int)reader["TimeZoneId"]),0);
						txtTimeZone.Text = "GMT ";
						if(Offset.TotalMinutes != 0)
						{
							string str = "";
							str += (Offset.TotalMinutes > 0) ? "+" : "-";
							if(Math.Abs(Offset.Hours)<10)
								str += "0";
							str += Math.Abs(Offset.Hours).ToString() + ":";
							if(Math.Abs(Offset.Minutes)<10)
								str += "0";
							str += Math.Abs(Offset.Minutes).ToString();
							txtTimeZone.Text += str;
						}
					}
				}
				else
				{
					lblNoRecurrence.Text = "<br>&nbsp; " + LocRM.GetString("NotSet") + "<br>&nbsp;";
					lblNoRecurrence.Visible = true;
					ShowRecurrence.Visible = false;
				}
			}
			DataBind();
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
		}
		#endregion

		#region btnDelete_Click
		protected void btnDelete_Click(object sender, System.EventArgs e)
		{
			CalendarEntry.DeleteRecurrence(EventId);

			String slink = String.Empty;
			if (SharedID>0)
				slink="&SharedId=" + SharedID;

			Response.Redirect(String.Format("../Events/EventView.aspx?EventId={0}{1}&Tab=General", EventId, slink), true);
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindValues();
			BindToolbar();
		}
		#endregion

//===========================================================================
// This public property was added by conversion wizard to allow the access of a protected, autogenerated member variable secHeader.
//===========================================================================
    public Mediachase.UI.Web.Modules.BlockHeaderLightWithMenu secHeader {
        get { return Migrated_secHeader; }
        //set { Migrated_secHeader = value; }
    }
	}
}
