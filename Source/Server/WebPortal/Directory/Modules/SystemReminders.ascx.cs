namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
	using System.Resources;
	using System.Globalization;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Reflection;

	/// <summary>
	///		Summary description for SystemReminders.
	/// </summary>
	public partial class SystemReminders : System.Web.UI.UserControl
	{
		#region HTML Variables
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMessageTemplates", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(SystemReminders).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				ApplyLocalization();
				BindDefaultValues();
			}

			secHeader.AddText(LocRM2.GetString("tReminderNotifications"));
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
			ibEditProjectStart.Click +=new ImageClickEventHandler(ibEdit_Click);
			ibEditProjectFinish.Click +=new ImageClickEventHandler(ibEdit_Click);
			ibEditTaskStart.Click +=new ImageClickEventHandler(ibEdit_Click);
			ibEditTaskFinish.Click +=new ImageClickEventHandler(ibEdit_Click);
			ibEditToDoStart.Click +=new ImageClickEventHandler(ibEdit_Click);
			ibEditToDoFinish.Click +=new ImageClickEventHandler(ibEdit_Click);
			ibEditCalendarEntryStart.Click +=new ImageClickEventHandler(ibEdit_Click);
			ibEditCalendarEntryFinish.Click +=new ImageClickEventHandler(ibEdit_Click);
			ibEditAssignmentFinish.Click += new ImageClickEventHandler(ibEdit_Click);

			ibRestoreProjectStart.Click +=new ImageClickEventHandler(ibRestore_Click);
			ibRestoreProjectFinish.Click +=new ImageClickEventHandler(ibRestore_Click);
			ibRestoreTaskStart.Click +=new ImageClickEventHandler(ibRestore_Click);
			ibRestoreTaskFinish.Click +=new ImageClickEventHandler(ibRestore_Click);
			ibRestoreToDoStart.Click +=new ImageClickEventHandler(ibRestore_Click);
			ibRestoreToDoFinish.Click +=new ImageClickEventHandler(ibRestore_Click);
			ibRestoreCalendarEntryStart.Click +=new ImageClickEventHandler(ibRestore_Click);
			ibRestoreCalendarEntryFinish.Click +=new ImageClickEventHandler(ibRestore_Click);
			ibRestoreAssignmentFinish.Click += new ImageClickEventHandler(ibRestore_Click);

			ibSaveProjectStart.Click +=new ImageClickEventHandler(ibSave_Click);
			ibSaveProjectFinish.Click +=new ImageClickEventHandler(ibSave_Click);
			ibSaveTaskStart.Click +=new ImageClickEventHandler(ibSave_Click);
			ibSaveTaskFinish.Click +=new ImageClickEventHandler(ibSave_Click);
			ibSaveToDoStart.Click +=new ImageClickEventHandler(ibSave_Click);
			ibSaveToDoFinish.Click +=new ImageClickEventHandler(ibSave_Click);
			ibSaveCalendarEntryStart.Click +=new ImageClickEventHandler(ibSave_Click);
			ibSaveCalendarEntryFinish.Click +=new ImageClickEventHandler(ibSave_Click);
			ibSaveAssignmentFinish.Click += new ImageClickEventHandler(ibSave_Click);

			ibCancelProjectStart.Click +=new ImageClickEventHandler(ibCancel_Click);
			ibCancelProjectFinish.Click +=new ImageClickEventHandler(ibCancel_Click);
			ibCancelTaskStart.Click +=new ImageClickEventHandler(ibCancel_Click);
			ibCancelTaskFinish.Click +=new ImageClickEventHandler(ibCancel_Click);
			ibCancelToDoStart.Click +=new ImageClickEventHandler(ibCancel_Click);
			ibCancelToDoFinish.Click +=new ImageClickEventHandler(ibCancel_Click);
			ibCancelCalendarEntryStart.Click +=new ImageClickEventHandler(ibCancel_Click);
			ibCancelCalendarEntryFinish.Click +=new ImageClickEventHandler(ibCancel_Click);
			ibCancelAssignmentFinish.Click += new ImageClickEventHandler(ibCancel_Click);
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			ibEditProjectStart.AlternateText = ibEditProjectFinish.AlternateText = 
				ibEditTaskStart.AlternateText = ibEditTaskFinish.AlternateText = 
				ibEditToDoStart.AlternateText = ibEditToDoFinish.AlternateText = 
				ibEditCalendarEntryStart.AlternateText = ibEditCalendarEntryFinish.AlternateText = 
				ibEditAssignmentFinish.AlternateText =
				LocRM.GetString("Edit");

			ibRestoreProjectStart.AlternateText = ibRestoreProjectFinish.AlternateText = 
				ibRestoreTaskStart.AlternateText = ibRestoreTaskFinish.AlternateText = 
				ibRestoreToDoStart.AlternateText = ibRestoreToDoFinish.AlternateText = 
				ibRestoreCalendarEntryStart.AlternateText = ibRestoreCalendarEntryFinish.AlternateText =
				ibRestoreAssignmentFinish.AlternateText = 
				LocRM.GetString("RestoreDefaults");

			ibSaveProjectStart.AlternateText = ibSaveProjectFinish.AlternateText = 
				ibSaveTaskStart.AlternateText = ibSaveTaskFinish.AlternateText = 
				ibSaveToDoStart.AlternateText = ibSaveToDoFinish.AlternateText = 
				ibSaveCalendarEntryStart.AlternateText = ibSaveCalendarEntryFinish.AlternateText =
				ibSaveAssignmentFinish.AlternateText = 
				LocRM.GetString("Save");

			ibCancelProjectStart.AlternateText = ibCancelProjectFinish.AlternateText = 
				ibCancelTaskStart.AlternateText = ibCancelTaskFinish.AlternateText = 
				ibCancelToDoStart.AlternateText = ibCancelToDoFinish.AlternateText = 
				ibCancelCalendarEntryStart.AlternateText = ibCancelCalendarEntryFinish.AlternateText =
				ibCancelAssignmentFinish.AlternateText = 
				LocRM.GetString("Cancel");
		}
		#endregion

		#region FillDropDown
		private void FillDropDown(DropDownList ddl)
		{
			ddl.Items.Add(new ListItem(CommonHelper.GetIntervalString(-1), "-1"));
			ddl.Items.Add(new ListItem(CommonHelper.GetIntervalString(5), "5"));
			ddl.Items.Add(new ListItem(CommonHelper.GetIntervalString(15), "15"));
			ddl.Items.Add(new ListItem(CommonHelper.GetIntervalString(30), "30"));
			ddl.Items.Add(new ListItem(CommonHelper.GetIntervalString(60), "60"));
			ddl.Items.Add(new ListItem(CommonHelper.GetIntervalString(240), "240"));
			ddl.Items.Add(new ListItem(CommonHelper.GetIntervalString(1440), "1440"));
			ddl.Items.Add(new ListItem(CommonHelper.GetIntervalString(2880), "2880"));
			ddl.Items.Add(new ListItem(CommonHelper.GetIntervalString(4320), "4320"));
			ddl.Items.Add(new ListItem(CommonHelper.GetIntervalString(10080), "10080"));
			ddl.Items.Add(new ListItem(CommonHelper.GetIntervalString(20160), "20160"));
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			if(Configuration.ProjectManagementEnabled)
			{
				BindRow(DateTypes.Project_StartDate, ibRestoreProjectStart, lblProjectStart);
				BindRow(DateTypes.Project_FinishDate, ibRestoreProjectFinish, lblProjectFinish);
				BindRow(DateTypes.Task_StartDate, ibRestoreTaskStart, lblTaskStart);
				BindRow(DateTypes.Task_FinishDate, ibRestoreTaskFinish, lblTaskFinish);
			}
			else
			{
				trPrj1.Visible = false;
				trPrj2.Visible = false;
				trPrj3.Visible = false;
				trTask1.Visible = false;
				trTask2.Visible = false;
				trTask3.Visible = false;
			}
			BindRow(DateTypes.Todo_StartDate, ibRestoreToDoStart, lblToDoStart);
			BindRow(DateTypes.Todo_FinishDate, ibRestoreToDoFinish, lblToDoFinish);
			BindRow(DateTypes.CalendarEntry_StartDate, ibRestoreCalendarEntryStart, lblCalendarEntryStart);
			BindRow(DateTypes.CalendarEntry_FinishDate, ibRestoreCalendarEntryFinish, lblCalendarEntryFinish);

			if (Configuration.WorkflowModule)
			{
				BindRow(DateTypes.AssignmentFinishDate, ibRestoreAssignmentFinish, lblAssignmentFinish);
			}
			else
			{
				AssignmentRow1.Visible = false;
				AssignmentRow2.Visible = false;
			}
		}
		#endregion

		#region BindRow
		private void BindRow(DateTypes DateType, ImageButton ibRestore, Label lblInterval)
		{
			int Lag = -1;
			bool IsActive = false;
			int SubscriptionTypeId = (int)SubscriptionTypes.Global;

			using (IDataReader reader = Schedule.GetReminderSubscriptionPersonal(DateType))
			{
				// SubscriptionId, DateTypeId, UserId, ObjectId, ObjectUid, Lag, IsActive, HookId, SubscriptionType 
				if (reader.Read())
				{
					Lag = (int)reader["Lag"];
					IsActive = (bool)reader["IsActive"];
					SubscriptionTypeId = (int)reader["SubscriptionType"];
				}
			}

			if (SubscriptionTypeId == (int)SubscriptionTypes.Personal)
				ibRestore.Visible = true;
			if (!IsActive)
				Lag = -1;
			lblInterval.Text = CommonHelper.GetIntervalString(Lag);
		}
		#endregion

		#region SelectValue
		private void SelectValue(DropDownList ddl, DateTypes DateType)
		{
			ddl.ClearSelection();

			using (IDataReader reader = Schedule.GetReminderSubscriptionPersonal(DateType))
			{
				///  SubscriptionId, DateTypeId, UserId, ObjectId, ObjectUid, Lag, IsActive, HookId 
				if (reader.Read())
				{
					if ((bool)reader["IsActive"])
						Util.CommonHelper.SafeSelect(ddl, reader["Lag"].ToString());
				}
			}

		}
		#endregion

		#region ibEdit_Click
		private void ibEdit_Click(object sender, ImageClickEventArgs e)
		{
			ImageButton ib = (ImageButton)sender;
			if (ib.ID == "ibEditProjectStart")
			{
				lblProjectStart.Visible = false;
				ddProjectStart.Visible = true;
				ibEditProjectStart.Visible = false;
				ibRestoreProjectStart.Visible = false;
				ibSaveProjectStart.Visible = true;
				ibCancelProjectStart.Visible = true;
				if (ddProjectStart.Items.Count <= 0)
					FillDropDown(ddProjectStart);
				SelectValue(ddProjectStart, DateTypes.Project_StartDate);
			}
			else if (ib.ID == "ibEditProjectFinish")
			{
				lblProjectFinish.Visible = false;
				ddProjectFinish.Visible = true;
				ibEditProjectFinish.Visible = false;
				ibRestoreProjectFinish.Visible = false;
				ibSaveProjectFinish.Visible = true;
				ibCancelProjectFinish.Visible = true;
				if (ddProjectFinish.Items.Count <= 0)
					FillDropDown(ddProjectFinish);
				SelectValue(ddProjectFinish, DateTypes.Project_FinishDate);
			}
			else if (ib.ID == "ibEditTaskStart")
			{
				lblTaskStart.Visible = false;
				ddTaskStart.Visible = true;
				ibEditTaskStart.Visible = false;
				ibRestoreTaskStart.Visible = false;
				ibSaveTaskStart.Visible = true;
				ibCancelTaskStart.Visible = true;
				if (ddTaskStart.Items.Count <= 0)
					FillDropDown(ddTaskStart);
				SelectValue(ddTaskStart, DateTypes.Task_StartDate);
			}
			else if (ib.ID == "ibEditTaskFinish")
			{
				lblTaskFinish.Visible = false;
				ddTaskFinish.Visible = true;
				ibEditTaskFinish.Visible = false;
				ibRestoreTaskFinish.Visible = false;
				ibSaveTaskFinish.Visible = true;
				ibCancelTaskFinish.Visible = true;
				if (ddTaskFinish.Items.Count <= 0)
					FillDropDown(ddTaskFinish);
				SelectValue(ddTaskFinish, DateTypes.Task_FinishDate);
			}
			else if (ib.ID == "ibEditToDoStart")
			{
				lblToDoStart.Visible = false;
				ddToDoStart.Visible = true;
				ibEditToDoStart.Visible = false;
				ibRestoreToDoStart.Visible = false;
				ibSaveToDoStart.Visible = true;
				ibCancelToDoStart.Visible = true;
				if (ddToDoStart.Items.Count <= 0)
					FillDropDown(ddToDoStart);
				SelectValue(ddToDoStart, DateTypes.Todo_StartDate);
			}
			else if (ib.ID == "ibEditToDoFinish")
			{
				lblToDoFinish.Visible = false;
				ddToDoFinish.Visible = true;
				ibEditToDoFinish.Visible = false;
				ibRestoreToDoFinish.Visible = false;
				ibSaveToDoFinish.Visible = true;
				ibCancelToDoFinish.Visible = true;
				if (ddToDoFinish.Items.Count <= 0)
					FillDropDown(ddToDoFinish);
				SelectValue(ddToDoFinish, DateTypes.Todo_FinishDate);
			}
			else if (ib.ID == "ibEditCalendarEntryStart")
			{
				lblCalendarEntryStart.Visible = false;
				ddCalendarEntryStart.Visible = true;
				ibEditCalendarEntryStart.Visible = false;
				ibRestoreCalendarEntryStart.Visible = false;
				ibSaveCalendarEntryStart.Visible = true;
				ibCancelCalendarEntryStart.Visible = true;
				if (ddCalendarEntryStart.Items.Count <= 0)
					FillDropDown(ddCalendarEntryStart);
				SelectValue(ddCalendarEntryStart, DateTypes.CalendarEntry_StartDate);
			}
			else if (ib.ID == "ibEditCalendarEntryFinish")
			{
				lblCalendarEntryFinish.Visible = false;
				ddCalendarEntryFinish.Visible = true;
				ibEditCalendarEntryFinish.Visible = false;
				ibRestoreCalendarEntryFinish.Visible = false;
				ibSaveCalendarEntryFinish.Visible = true;
				ibCancelCalendarEntryFinish.Visible = true;
				if (ddCalendarEntryFinish.Items.Count <= 0)
					FillDropDown(ddCalendarEntryFinish);
				SelectValue(ddCalendarEntryFinish, DateTypes.CalendarEntry_FinishDate);
			}
			else if (ib.ID == "ibEditAssignmentFinish")
			{
				lblAssignmentFinish.Visible = false;
				ddAssignmentFinish.Visible = true;
				ibEditAssignmentFinish.Visible = false;
				ibRestoreAssignmentFinish.Visible = false;
				ibSaveAssignmentFinish.Visible = true;
				ibCancelAssignmentFinish.Visible = true;
				if (ddAssignmentFinish.Items.Count <= 0)
					FillDropDown(ddAssignmentFinish);
				SelectValue(ddAssignmentFinish, DateTypes.AssignmentFinishDate);
			}
		}
		#endregion

		#region ibRestore_Click
		private void ibRestore_Click(object sender, ImageClickEventArgs e)
		{
			ImageButton ib = (ImageButton)sender;
			if (ib.ID == "ibRestoreProjectStart")
				Schedule.DeleteReminderSubscriptionPersonal(DateTypes.Project_StartDate);
			else if (ib.ID == "ibRestoreProjectFinish")
				Schedule.DeleteReminderSubscriptionPersonal(DateTypes.Project_FinishDate);
			else if (ib.ID == "ibRestoreTaskStart")
				Schedule.DeleteReminderSubscriptionPersonal(DateTypes.Task_StartDate);
			else if (ib.ID == "ibRestoreTaskFinish")
				Schedule.DeleteReminderSubscriptionPersonal(DateTypes.Task_FinishDate);
			else if (ib.ID == "ibRestoreToDoStart")
				Schedule.DeleteReminderSubscriptionPersonal(DateTypes.Todo_StartDate);
			else if (ib.ID == "ibRestoreToDoFinish")
				Schedule.DeleteReminderSubscriptionPersonal(DateTypes.Todo_FinishDate);
			else if (ib.ID == "ibRestoreCalendarEntryStart")
				Schedule.DeleteReminderSubscriptionPersonal(DateTypes.CalendarEntry_StartDate);
			else if (ib.ID == "ibRestoreCalendarEntryFinish")
				Schedule.DeleteReminderSubscriptionPersonal(DateTypes.CalendarEntry_FinishDate);
			else if (ib.ID == "ibRestoreAssignmentFinish")
				Schedule.DeleteReminderSubscriptionPersonal(DateTypes.AssignmentFinishDate);

			string url = String.Concat("~/Directory/UserView.aspx?UserId=", Security.CurrentUser.UserID.ToString());
			Response.Redirect(url, true);
		}
		#endregion

		#region ibSave_Click
		private void ibSave_Click(object sender, ImageClickEventArgs e)
		{
			ImageButton ib = (ImageButton)sender;
			if (ib.ID == "ibSaveProjectStart")
			{
				int Lag = int.Parse(ddProjectStart.SelectedValue);
				Schedule.UpdateReminderSubscriptionPersonal(DateTypes.Project_StartDate, Lag>0?Lag:0, Lag>0);
			}
			else if (ib.ID == "ibSaveProjectFinish")
			{
				int Lag = int.Parse(ddProjectFinish.SelectedValue);
				Schedule.UpdateReminderSubscriptionPersonal(DateTypes.Project_FinishDate, Lag>0?Lag:0, Lag>0);
			}
			else if (ib.ID == "ibSaveTaskStart")
			{
				int Lag = int.Parse(ddTaskStart.SelectedValue);
				Schedule.UpdateReminderSubscriptionPersonal(DateTypes.Task_StartDate, Lag>0?Lag:0, Lag>0);
			}
			else if (ib.ID == "ibSaveTaskFinish")
			{
				int Lag = int.Parse(ddTaskFinish.SelectedValue);
				Schedule.UpdateReminderSubscriptionPersonal(DateTypes.Task_FinishDate, Lag>0?Lag:0, Lag>0);
			}
			else if (ib.ID == "ibSaveToDoStart")
			{
				int Lag = int.Parse(ddToDoStart.SelectedValue);
				Schedule.UpdateReminderSubscriptionPersonal(DateTypes.Todo_StartDate, Lag>0?Lag:0, Lag>0);
			}
			else if (ib.ID == "ibSaveToDoFinish")
			{
				int Lag = int.Parse(ddToDoFinish.SelectedValue);
				Schedule.UpdateReminderSubscriptionPersonal(DateTypes.Todo_FinishDate, Lag>0?Lag:0, Lag>0);
			}
			else if (ib.ID == "ibSaveCalendarEntryStart")
			{
				int Lag = int.Parse(ddCalendarEntryStart.SelectedValue);
				Schedule.UpdateReminderSubscriptionPersonal(DateTypes.CalendarEntry_StartDate, Lag>0?Lag:0, Lag>0);
			}
			else if (ib.ID == "ibSaveCalendarEntryFinish")
			{
				int Lag = int.Parse(ddCalendarEntryFinish.SelectedValue);
				Schedule.UpdateReminderSubscriptionPersonal(DateTypes.CalendarEntry_FinishDate, Lag>0?Lag:0, Lag>0);
			}
			else if (ib.ID == "ibSaveAssignmentFinish")
			{
				int Lag = int.Parse(ddAssignmentFinish.SelectedValue);
				Schedule.UpdateReminderSubscriptionPersonal(DateTypes.AssignmentFinishDate, Lag > 0 ? Lag : 0, Lag > 0);
			}

			string url = String.Concat("~/Directory/UserView.aspx?UserId=", Security.CurrentUser.UserID.ToString());
			Response.Redirect(url, true);
		}
		#endregion

		#region ibCancel_Click
		private void ibCancel_Click(object sender, ImageClickEventArgs e)
		{
			string url = String.Concat("~/Directory/UserView.aspx?UserId=", Security.CurrentUser.UserID.ToString());
			Response.Redirect(url, true);
		}
		#endregion
	}
}
