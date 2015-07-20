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
	using cal = Mediachase.IBN.Business.Calendar;

	/// <summary>
	///		Summary description for ExceptionsEditor.
	/// </summary>
	public partial class ExceptionsEditor : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strExceptionsEditor", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		#region CalendarID
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
					return -1;
				}
			}
		}
		#endregion

		#region UserCalendarId
		private int UserCalendarId
		{
			get
			{
				try
				{
					return int.Parse(Request["UserCalendarId"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region ExceptionID
		private int ExceptionID
		{
			get
			{
				try
				{
					return int.Parse(Request["ExceptionID"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			BindToolbar();
			DataBind();
			if (!IsPostBack)
				if (ExceptionID > 0)
					BindSavedValues();
				else
				{
					Radiobutton1.Checked = true;
					tcdisable();
				}
		}

		#region BindSavedValues
		private void BindSavedValues()
		{
			if (CalendarID > 0)	//project calendar
			{
				using (IDataReader rdr = cal.GetException(ExceptionID))
				{
					if (rdr.Read())
					{
						dc1.SelectedDate = (DateTime)rdr["FromDate"];
						dc2.SelectedDate = (DateTime)rdr["ToDate"];
					}
				}

				using (IDataReader rdr = cal.GetListExceptionHours(ExceptionID))
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
			else  //user calendar
			{
				using (IDataReader rdr = cal.GetUserException(ExceptionID))
				{
					if (rdr.Read())
					{
						dc1.SelectedDate = (DateTime)rdr["FromDate"];
						dc2.SelectedDate = (DateTime)rdr["ToDate"];
					}
				}

				using (IDataReader rdr = cal.GetListUserCalendarExceptionHours(ExceptionID))
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
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{

			secHeader.Title = LocRM.GetString("tTitle");

			secHeader.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/SaveItem.gif") + "'/> " + LocRM.GetString("Save"), "javascript:document.forms[0]." + btnSave.ClientID + ".click()");
			secHeader.AddSeparator();
			if (CalendarID > 0)
				secHeader.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/Cancel.gif") + "'/> " + LocRM.GetString("Cancel"),
					ResolveUrl("~/Admin/CalendarDetails.aspx?CalendarID=" + CalendarID));
			else
				secHeader.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/Cancel.gif") + "'/> " + LocRM.GetString("Cancel"),
					ResolveUrl("~/Directory/UserView.aspx?UserID=" + Security.CurrentUser.UserID));

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
			this.cv1.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.cv1_Validate);
			this.CustomValidator1.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.CustomValidator1_ServerValidate);
		}
		#endregion

		protected void Button1_Click(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			if (Radiobutton1.Checked && ExceptionID > 0)
			{
				if (CalendarID > 0)			//project calendar
					cal.UpdateException(ExceptionID, dc1.SelectedDate, dc2.SelectedDate, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1);
				else if (UserCalendarId > 0)	//user Calendar
					cal.UpdateUserException(ExceptionID, dc1.SelectedDate, dc2.SelectedDate, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1);
			}
			else if (Radiobutton1.Checked && ExceptionID < 0)
			{
				if (CalendarID > 0)
					cal.CreateException(CalendarID, dc1.SelectedDate, dc2.SelectedDate, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1);
				else if (UserCalendarId > 0)
					cal.CreateUserException(UserCalendarId, dc1.SelectedDate, dc2.SelectedDate, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1);
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

				if (ExceptionID > 0)
				{
					if (CalendarID > 0)
						cal.UpdateException(ExceptionID, dc1.SelectedDate, dc2.SelectedDate, f1, t1, f2, t2, f3, t3, f4, t4, f5, t5);
					else if (UserCalendarId > 0)
						cal.UpdateUserException(ExceptionID, dc1.SelectedDate, dc2.SelectedDate, f1, t1, f2, t2, f3, t3, f4, t4, f5, t5);
				}
				else
				{
					if (CalendarID > 0)
						cal.CreateException(CalendarID, dc1.SelectedDate, dc2.SelectedDate, f1, t1, f2, t2, f3, t3, f4, t4, f5, t5);
					else if (UserCalendarId > 0)
						cal.CreateUserException(UserCalendarId, dc1.SelectedDate, dc2.SelectedDate, f1, t1, f2, t2, f3, t3, f4, t4, f5, t5);
				}


			}
			if (CalendarID > 0)
				Response.Redirect("~/Admin/CalendarDetails.aspx?CalendarID=" + CalendarID);
			else
				Response.Redirect("~/Directory/UserView.aspx?UserID=" + Security.CurrentUser.UserID, true);

		}

		#region tcenable
		private void tcenable()
		{
			tc1.Enable();
			tc2.Enable();
			tc3.Enable();
			tc4.Enable();
			tc5.Enable();
		}
		#endregion

		#region tcdisable
		private void tcdisable()
		{
			tc1.Disable();
			tc2.Disable();
			tc3.Disable();
			tc4.Disable();
			tc5.Disable();
		}
		#endregion


		#region rb_ChangeWorkDay
		protected void rb_ChangeWorkDay(object sender, System.EventArgs e)
		{
			if (rbWorking.Checked == true)
				tcenable();
			else
				tcdisable();
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

		#region cv1_Validate
		private void cv1_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			if (dc1.SelectedDate <= dc2.SelectedDate)
				args.IsValid = true;
			else
				args.IsValid = false;
		}
		#endregion

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
	}
}
