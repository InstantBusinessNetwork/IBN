namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Reflection;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.Ibn;
	using Mediachase.IBN.Business;
	

	/// <summary>
	///		Summary description for IntervalsEditor.
	/// </summary>
	public partial class IntervalsEditor : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM;


		private int CalendarID
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

		private int DayID
		{
			get
			{
				try
				{
					return int.Parse(Request["DayID"]);
				}
				catch
				{
					throw new AccessDeniedException();
				}
			}
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
				BindSavedValues();
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strIntervalsEditor", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			BindToolbar();
			DataBind();
		}

		private void BindSavedValues()
		{
			using (IDataReader rdr = Mediachase.IBN.Business.Calendar.GetListWeekdayHours(CalendarID, (byte)DayID))
			{
				if (rdr.Read())
				{
					tc1.SetDate(ParseMin((int)rdr["FromTime"]), ParseMin((int)rdr["ToTime"]));
					if (rdr.Read())
					{
						tc2.SetDate(ParseMin((int)rdr["FromTime"]), ParseMin((int)rdr["ToTime"]));
						if (rdr.Read())
						{
							tc3.SetDate(ParseMin((int)rdr["FromTime"]), ParseMin((int)rdr["ToTime"]));
							if (rdr.Read())
							{
								tc4.SetDate(ParseMin((int)rdr["FromTime"]), ParseMin((int)rdr["ToTime"]));
								if (rdr.Read())
									tc5.SetDate(ParseMin((int)rdr["FromTime"]), ParseMin((int)rdr["ToTime"]));
							}
						}
					}
					tcenable();
					rbWorking.Checked = true;
					Radiobutton1.Checked = false;

				}
				else
				{
					tcdisable();
					rbWorking.Checked = false;
					Radiobutton1.Checked = true;
				}
			}
		}

		private void BindToolbar()
		{
			string day = GetDateName(DayID);
			secHeader.Title = "<span style='text-transform: capitalize;'>" + day + "</span>";
			secHeader.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/SaveItem.gif") + "'/> " + LocRM.GetString("Save"), "javascript:document.forms[0]." + btnSave.ClientID + ".click()");
			secHeader.AddSeparator();
			secHeader.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/Cancel.gif") + "'/> " + LocRM.GetString("Cancel"), 
				ResolveUrl("~/Admin/CalendarDetails.aspx?CalendarID=" + CalendarID));

		}

		private string GetDateName(int num)
		{
			if (num == 7) num = 0;
			return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((DayOfWeek)(num));
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
			this.cvNoOneWorkingInterval.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.cvNoOne_Validate);
			this.CustomValidator1.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.CustomValidator1_ServerValidate);

		}
		#endregion

		protected void Button1_Click(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			if (Radiobutton1.Checked)
				try
				{
					Mediachase.IBN.Business.Calendar.UpdateWeekdayHours(CalendarID, (byte)DayID, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1);
					Response.Redirect("~/Admin/CalendarDetails.aspx?CalendarID=" + CalendarID);
				}
				catch
				{
					cvNoOneWorkingInterval.IsValid = false;
				}

			else
			{
				int f1 = -1, t1 = -1, f2 = -1, t2 = -1, f3 = -1, t3 = -1, f4 = -1, t4 = -1, f5 = -1, t5 = -1;
				if (tc1.IsActive)
				{
					f1 = tc1.getFrom;
					t1 = tc1.getTo;
				}
				if (tc2.IsActive)
				{
					f2 = tc2.getFrom;
					t2 = tc2.getTo;
				}
				if (tc3.IsActive)
				{
					f3 = tc3.getFrom;
					t3 = tc3.getTo;
				}
				if (tc4.IsActive)
				{
					f4 = tc4.getFrom;
					t4 = tc4.getTo;
				}
				if (tc5.IsActive)
				{
					f5 = tc5.getFrom;
					t5 = tc5.getTo;
				}

				try
				{
					Mediachase.IBN.Business.Calendar.UpdateWeekdayHours(CalendarID, (byte)DayID, f1, t1, f2, t2, f3, t3, f4, t4, f5, t5);
					Response.Redirect("~/Admin/CalendarDetails.aspx?CalendarID=" + CalendarID);
				}
				catch
				{
					cvNoOneWorkingInterval.IsValid = false;
				}
			}
		}

		private void tcenable()
		{
			tc1.Enable();
			tc2.Enable();
			tc3.Enable();
			tc4.Enable();
			tc5.Enable();
		}

		private void tcdisable()
		{
			tc1.Disable();
			tc2.Disable();
			tc3.Disable();
			tc4.Disable();
			tc5.Disable();
		}


		protected void rb_ChangeWorkDay(object sender, System.EventArgs e)
		{
			if (rbWorking.Checked == true)
				tcenable();
			else
				tcdisable();
		}

		private DateTime ParseMin(int minutes)
		{
			TimeSpan ts = TimeSpan.FromMinutes(minutes);
			int hrs = (int)ts.TotalHours;
			int min = ts.Minutes;

			return new DateTime(1990, 1, 1, hrs, min, 0);
		}

		#region CustomValidator1_ServerValidate
		private void CustomValidator1_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			if (rbWorking.Checked)
			{
				Interval[] interval = new Interval[5];
				int index = 0;
				bool fields_fall = false;
				if (tc1.IsActive)
				{
					interval[index] = new Interval();
					try
					{
						interval[index].FromDate = tc1.getFrom;
						interval[index].ToDate = tc1.getTo;
					}
					catch
					{
						fields_fall = true;
					}
					index++;
				}
				if (tc2.IsActive)
				{
					interval[index] = new Interval();
					try
					{
						interval[index].FromDate = tc2.getFrom;
						interval[index].ToDate = tc2.getTo;
					}
					catch
					{
						fields_fall = true;
					}
					index++;
				}
				if (tc3.IsActive)
				{
					interval[index] = new Interval();
					try
					{
						interval[index].FromDate = tc3.getFrom;
						interval[index].ToDate = tc3.getTo;
					}
					catch
					{
						fields_fall = true;
					}
					index++;
				}
				if (tc4.IsActive)
				{
					interval[index] = new Interval();
					try
					{
						interval[index].FromDate = tc4.getFrom;
						interval[index].ToDate = tc4.getTo;
					}
					catch
					{
						fields_fall = true;
					}
					index++;
				}
				if (tc5.IsActive)
				{
					interval[index] = new Interval();
					try
					{
						interval[index].FromDate = tc5.getFrom;
						interval[index].ToDate = tc5.getTo;
					}
					catch
					{
						fields_fall = true;
					}
					index++;
				}

				if (fields_fall)
				{
					args.IsValid = false;
					return;
				}

				if (index > 0)
				{
					for (int i = 1; i < index; i++)
					{
						if (interval[i - 1].ToDate > interval[i].FromDate)
						{
							args.IsValid = false;
							return;

						}
					}
					args.IsValid = true;
				}
				else
					args.IsValid = true;
			}
			else
			{
				args.IsValid = true;
			}
		}
		#endregion

		private void cvNoOne_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{

		}

		private void btnSave_ServerClick(object sender, System.EventArgs e)
		{

		}
	}

	internal class Interval
	{
		public int FromDate, ToDate;
	}

}
