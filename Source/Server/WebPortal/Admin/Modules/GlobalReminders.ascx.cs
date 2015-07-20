namespace Mediachase.UI.Web.Admin.Modules
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
	using System.Reflection;

	/// <summary>
	///		Summary description for GlobalReminders.
	/// </summary>
	public partial class GlobalReminders : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMessageTemplates", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(GlobalReminders).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strEventEdit", typeof(GlobalReminders).Assembly);
		protected ResourceManager LocRM4 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				BindDefaultValues();
				BindValues();
			}

			secHeader.Title = LocRM2.GetString("tReminderNotifications");
			secHeader.AddLink("<img alt='' src='" + this.Page.ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM4.GetString("tRoutingWorkflow"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin4"));
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			btnCancel.CustomImage = this.Page.ResolveUrl("~/layouts/images/deny.gif");
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
			btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
			btnCancel.ServerClick += new EventHandler(btnCancel_ServerClick);
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			btnSave.Text = LocRM3.GetString("tbsave_save");
			btnCancel.Text = LocRM3.GetString("tbsave_cancel");

			if (Configuration.ProjectManagementEnabled)
			{
				FillDropDown(ddProjectStart);
				FillDropDown(ddProjectFinish);
				FillDropDown(ddTaskStart);
				FillDropDown(ddTaskFinish);
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
			FillDropDown(ddToDoStart);
			FillDropDown(ddToDoFinish);
			FillDropDown(ddCalendarEntryStart);
			FillDropDown(ddCalendarEntryFinish);
			FillDropDown(ddAssignmentFinish);

			if (!Configuration.WorkflowModule)
			{
				AssignmentRow1.Visible = false;
				AssignmentRow2.Visible = false;
			}
		}
		#endregion

		#region FillDropDown
		private void FillDropDown(DropDownList ddl)
		{
			ddl.Items.Add(new ListItem(LocRM3.GetString("tDonotremind"), "-1"));
			ddl.Items.Add(new ListItem(LocRM3.GetString("t5Min"), "5"));
			ddl.Items.Add(new ListItem(LocRM3.GetString("t15Min"), "15"));
			ddl.Items.Add(new ListItem(LocRM3.GetString("t30Min"), "30"));
			ddl.Items.Add(new ListItem(LocRM3.GetString("t1Hour"), "60"));
			ddl.Items.Add(new ListItem(LocRM3.GetString("t4Hour"), "240"));
			ddl.Items.Add(new ListItem(LocRM3.GetString("t1Day"), "1440"));
			ddl.Items.Add(new ListItem(LocRM3.GetString("t2Day"), "2880"));
			ddl.Items.Add(new ListItem(LocRM3.GetString("t3Day"), "4320"));
			ddl.Items.Add(new ListItem(LocRM3.GetString("t1Week"), "10080"));
			ddl.Items.Add(new ListItem(LocRM3.GetString("t2Week"), "20160"));
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			SelectValue(ddProjectStart, DateTypes.Project_StartDate);
			SelectValue(ddProjectFinish, DateTypes.Project_FinishDate);
			SelectValue(ddTaskStart, DateTypes.Task_StartDate);
			SelectValue(ddTaskFinish, DateTypes.Task_FinishDate);
			SelectValue(ddToDoStart, DateTypes.Todo_StartDate);
			SelectValue(ddToDoFinish, DateTypes.Todo_FinishDate);
			SelectValue(ddCalendarEntryStart, DateTypes.CalendarEntry_StartDate);
			SelectValue(ddCalendarEntryFinish, DateTypes.CalendarEntry_FinishDate);

			if (Configuration.WorkflowModule)
				SelectValue(ddAssignmentFinish, DateTypes.AssignmentFinishDate);
		}
		#endregion

		#region SelectValue
		private void SelectValue(DropDownList ddl, DateTypes DateType)
		{
			ddl.ClearSelection();

			using (IDataReader reader = Schedule.GetReminderSubscriptionGlobal(DateType))
			{
				///  SubscriptionId, DateTypeId, UserId, ObjectId, Lag, IsActive, HookId 
				if (reader.Read())
				{
					if ((bool)reader["IsActive"])
						Util.CommonHelper.SafeSelect(ddl, reader["Lag"].ToString());
				}
			}
		}
		#endregion

		#region btnSave_ServerClick
		private void btnSave_ServerClick(object sender, EventArgs e)
		{
			Schedule.UpdateReminderSubscriptionGlobal(
				int.Parse(ddProjectStart.SelectedValue), int.Parse(ddProjectFinish.SelectedValue),
				int.Parse(ddCalendarEntryStart.SelectedValue), int.Parse(ddCalendarEntryFinish.SelectedValue),
				int.Parse(ddTaskStart.SelectedValue), int.Parse(ddTaskFinish.SelectedValue),
				int.Parse(ddToDoStart.SelectedValue), int.Parse(ddToDoFinish.SelectedValue),
				int.Parse(ddAssignmentFinish.SelectedValue));

			Response.Redirect("~/Admin/GlobalReminders.aspx", true);
		}
		#endregion

		#region btnCancel_ServerClick
		private void btnCancel_ServerClick(object sender, EventArgs e)
		{
			Response.Redirect("~/Admin/GlobalReminders.aspx", true);
		}
		#endregion
	}
}
