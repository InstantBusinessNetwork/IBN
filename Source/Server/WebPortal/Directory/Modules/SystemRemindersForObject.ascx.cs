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
	using Mediachase.Ibn.Web.Interfaces;
	using System.Reflection;

	/// <summary>
	///		Summary description for SystemRemindersForObject.
	/// </summary>
	public partial class SystemRemindersForObject : System.Web.UI.UserControl, IPageTemplateTitle
	{
		#region HTML Variables
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMessageTemplates", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(SystemRemindersForObject).Assembly);

		#region ObjectType
		private ObjectTypes objectType = ObjectTypes.UNDEFINED;
		public ObjectTypes ObjectType
		{
			get
			{
				return objectType;
			}
			set
			{
				objectType = value;
			}
		}
		#endregion

		#region ObjectId
		private int objectId = -1;
		public int ObjectId
		{
			get
			{
				return objectId;
			}
			set
			{
				objectId = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			LoadRequestVariables();

			if (!IsPostBack)
			{
				BindDefaultVisibility();
				ApplyLocalization();
				BindDefaultValues();
			}
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
			ibEditStart.Click += new ImageClickEventHandler(ibEditStart_Click);
			ibEditFinish.Click += new ImageClickEventHandler(ibEditFinish_Click);
			ibRestoreStart.Click += new ImageClickEventHandler(ibRestoreStart_Click);
			ibRestoreFinish.Click += new ImageClickEventHandler(ibRestoreFinish_Click);
			ibSaveStart.Click += new ImageClickEventHandler(ibSaveStart_Click);
			ibSaveFinish.Click += new ImageClickEventHandler(ibSaveFinish_Click);
			ibCancelStart.Click += new ImageClickEventHandler(ibCancel_Click);
			ibCancelFinish.Click += new ImageClickEventHandler(ibCancel_Click);
		}
		#endregion

		#region LoadRequestVariables
		private void LoadRequestVariables()
		{
			if (Request.QueryString["ObjectTypeId"] != null)
			{
				try
				{
					ObjectType = (ObjectTypes)int.Parse(Request.QueryString["ObjectTypeId"]);
				}
				catch
				{
				}
			}

			if (Request.QueryString["ObjectId"] != null)
			{
				try
				{
					ObjectId = int.Parse(Request.QueryString["ObjectId"]);
				}
				catch
				{
				}
			}
		}
		#endregion

		#region BindDefaultVisibility
		private void BindDefaultVisibility()
		{
			lblStart.Visible = true;
			ddStart.Visible = false;
			ibEditStart.Visible = true;
			ibRestoreStart.Visible = false;
			ibSaveStart.Visible = false;
			ibCancelStart.Visible = false;

			lblFinish.Visible = true;
			ddFinish.Visible = false;
			ibEditFinish.Visible = true;
			ibRestoreFinish.Visible = false;
			ibSaveFinish.Visible = false;
			ibCancelFinish.Visible = false;
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			ibEditStart.AlternateText = ibEditFinish.AlternateText = LocRM.GetString("Edit");
			ibRestoreStart.AlternateText = ibRestoreFinish.AlternateText = LocRM.GetString("RestoreDefaults");
			ibSaveStart.AlternateText = ibSaveFinish.AlternateText = LocRM.GetString("Save");
			ibCancelStart.AlternateText = ibCancelFinish.AlternateText = LocRM.GetString("Cancel");

			btnClose.Text = LocRM.GetString("Close");

			string link = "";
			if (ObjectType == ObjectTypes.Project)
				link =
					"try {\r\n" +
					"	window.opener.top.frames['right'].location.href='../Projects/ProjectView.aspx?ProjectId=" + ObjectId + "';\r\n" +
					"} catch (e){}\r\n" +
					"window.close();\r\n" +
					"return false;";
			else if (ObjectType == ObjectTypes.Task)
				link =
					"try {\r\n" +
					"	window.opener.top.frames['right'].location.href='../Tasks/TaskView.aspx?TaskId=" + ObjectId + "';\r\n" +
					"} catch (e){}\r\n" +
					"window.close();\r\n" +
					"return false;";
			else if (ObjectType == ObjectTypes.ToDo)
				link =
					"try {\r\n" +
					"	window.opener.top.frames['right'].location.href='../ToDo/ToDoView.aspx?TodoId=" + ObjectId + "';\r\n" +
					"} catch (e){}\r\n" +
					"window.close();\r\n" +
					"return false;";
			else if (ObjectType == ObjectTypes.CalendarEntry)
				link =
					"try {\r\n" +
					"	window.opener.top.frames['right'].location.href='../Events/EventView.aspx?EventId=" + ObjectId + "';\r\n" +
					"} catch (e){}\r\n" +
					"window.close();\r\n" +
					"return false;";

			btnClose.Attributes.Add("onclick", link);
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
			if (ObjectType == ObjectTypes.Project)
			{
				BindRow(DateTypes.Project_StartDate, ibRestoreStart, lblStart);
				BindRow(DateTypes.Project_FinishDate, ibRestoreFinish, lblFinish);
			}
			else if (ObjectType == ObjectTypes.Task)
			{
				BindRow(DateTypes.Task_StartDate, ibRestoreStart, lblStart);
				BindRow(DateTypes.Task_FinishDate, ibRestoreFinish, lblFinish);
			}
			else if (ObjectType == ObjectTypes.ToDo)
			{
				BindRow(DateTypes.Todo_StartDate, ibRestoreStart, lblStart);
				BindRow(DateTypes.Todo_FinishDate, ibRestoreFinish, lblFinish);
			}
			else if (ObjectType == ObjectTypes.CalendarEntry)
			{
				BindRow(DateTypes.CalendarEntry_StartDate, ibRestoreStart, lblStart);
				BindRow(DateTypes.CalendarEntry_FinishDate, ibRestoreFinish, lblFinish);
			}
		}
		#endregion

		#region BindRow
		private void BindRow(DateTypes DateType, ImageButton ibRestore, Label lblInterval)
		{
			int Lag = -1;
			bool IsActive = false;
			int SubscriptionTypeId = (int)SubscriptionTypes.Global;

			using (IDataReader reader = Schedule.GetReminderSubscriptionPersonalForObject(DateType, ObjectId))
			{
				// SubscriptionId, DateTypeId, UserId, ObjectId, Lag, IsActive, HookId, SubscriptionType 
				if (reader.Read())
				{
					Lag = (int)reader["Lag"];
					IsActive = (bool)reader["IsActive"];
					SubscriptionTypeId = (int)reader["SubscriptionType"];
				}
			}

			if (SubscriptionTypeId == (int)SubscriptionTypes.PersonalForObject)
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

			using (IDataReader reader = Schedule.GetReminderSubscriptionPersonalForObject(DateType, ObjectId))
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

		#region ibEditStart_Click
		private void ibEditStart_Click(object sender, ImageClickEventArgs e)
		{
			lblStart.Visible = false;
			ddStart.Visible = true;
			ibEditStart.Visible = false;
			ibRestoreStart.Visible = false;
			ibSaveStart.Visible = true;
			ibCancelStart.Visible = true;
			if (ddStart.Items.Count <= 0)
				FillDropDown(ddStart);

			if (ObjectType == ObjectTypes.Project)
				SelectValue(ddStart, DateTypes.Project_StartDate);
			else if (ObjectType == ObjectTypes.Task)
				SelectValue(ddStart, DateTypes.Task_StartDate);
			else if (ObjectType == ObjectTypes.ToDo)
				SelectValue(ddStart, DateTypes.Todo_StartDate);
			else if (ObjectType == ObjectTypes.CalendarEntry)
				SelectValue(ddStart, DateTypes.CalendarEntry_StartDate);
		}
		#endregion

		#region ibEditFinish_Click
		private void ibEditFinish_Click(object sender, ImageClickEventArgs e)
		{
			lblFinish.Visible = false;
			ddFinish.Visible = true;
			ibEditFinish.Visible = false;
			ibRestoreFinish.Visible = false;
			ibSaveFinish.Visible = true;
			ibCancelFinish.Visible = true;
			if (ddFinish.Items.Count <= 0)
				FillDropDown(ddFinish);

			if (ObjectType == ObjectTypes.Project)
				SelectValue(ddFinish, DateTypes.Project_FinishDate);
			else if (ObjectType == ObjectTypes.Task)
				SelectValue(ddFinish, DateTypes.Task_FinishDate);
			else if (ObjectType == ObjectTypes.ToDo)
				SelectValue(ddFinish, DateTypes.Todo_FinishDate);
			else if (ObjectType == ObjectTypes.CalendarEntry)
				SelectValue(ddFinish, DateTypes.CalendarEntry_FinishDate);
		}
		#endregion

		#region ibRestoreStart_Click
		private void ibRestoreStart_Click(object sender, ImageClickEventArgs e)
		{
			if (ObjectType == ObjectTypes.Project)
				Schedule.DeleteReminderSubscriptionPersonalForObject(DateTypes.Project_StartDate, ObjectId);
			else if (ObjectType == ObjectTypes.Task)
				Schedule.DeleteReminderSubscriptionPersonalForObject(DateTypes.Task_StartDate, ObjectId);
			else if (ObjectType == ObjectTypes.ToDo)
				Schedule.DeleteReminderSubscriptionPersonalForObject(DateTypes.Todo_StartDate, ObjectId);
			else if (ObjectType == ObjectTypes.CalendarEntry)
				Schedule.DeleteReminderSubscriptionPersonalForObject(DateTypes.CalendarEntry_StartDate, ObjectId);

			BindDefaultValues();
		}
		#endregion

		#region ibRestoreFinish_Click
		private void ibRestoreFinish_Click(object sender, ImageClickEventArgs e)
		{
			if (ObjectType == ObjectTypes.Project)
				Schedule.DeleteReminderSubscriptionPersonalForObject(DateTypes.Project_FinishDate, ObjectId);
			else if (ObjectType == ObjectTypes.Task)
				Schedule.DeleteReminderSubscriptionPersonalForObject(DateTypes.Task_FinishDate, ObjectId);
			else if (ObjectType == ObjectTypes.ToDo)
				Schedule.DeleteReminderSubscriptionPersonalForObject(DateTypes.Todo_FinishDate, ObjectId);
			else if (ObjectType == ObjectTypes.CalendarEntry)
				Schedule.DeleteReminderSubscriptionPersonalForObject(DateTypes.CalendarEntry_FinishDate, ObjectId);

			BindDefaultValues();
		}
		#endregion

		#region ibSaveStart_Click
		private void ibSaveStart_Click(object sender, ImageClickEventArgs e)
		{
			int Lag = int.Parse(ddStart.SelectedValue);

			if (ObjectType == ObjectTypes.Project)
				Schedule.UpdateReminderSubscriptionPersonalForObject(DateTypes.Project_StartDate, ObjectId, Lag > 0 ? Lag : 0, Lag > 0);
			else if (ObjectType == ObjectTypes.Task)
				Schedule.UpdateReminderSubscriptionPersonalForObject(DateTypes.Task_StartDate, ObjectId, Lag > 0 ? Lag : 0, Lag > 0);
			else if (ObjectType == ObjectTypes.ToDo)
				Schedule.UpdateReminderSubscriptionPersonalForObject(DateTypes.Todo_StartDate, ObjectId, Lag > 0 ? Lag : 0, Lag > 0);
			else if (ObjectType == ObjectTypes.CalendarEntry)
				Schedule.UpdateReminderSubscriptionPersonalForObject(DateTypes.CalendarEntry_StartDate, ObjectId, Lag > 0 ? Lag : 0, Lag > 0);

			BindDefaultVisibility();
			BindDefaultValues();
		}
		#endregion

		#region ibSaveFinish_Click
		private void ibSaveFinish_Click(object sender, ImageClickEventArgs e)
		{
			int Lag = int.Parse(ddFinish.SelectedValue);

			if (ObjectType == ObjectTypes.Project)
				Schedule.UpdateReminderSubscriptionPersonalForObject(DateTypes.Project_FinishDate, ObjectId, Lag > 0 ? Lag : 0, Lag > 0);
			else if (ObjectType == ObjectTypes.Task)
				Schedule.UpdateReminderSubscriptionPersonalForObject(DateTypes.Task_FinishDate, ObjectId, Lag > 0 ? Lag : 0, Lag > 0);
			else if (ObjectType == ObjectTypes.ToDo)
				Schedule.UpdateReminderSubscriptionPersonalForObject(DateTypes.Todo_FinishDate, ObjectId, Lag > 0 ? Lag : 0, Lag > 0);
			else if (ObjectType == ObjectTypes.CalendarEntry)
				Schedule.UpdateReminderSubscriptionPersonalForObject(DateTypes.CalendarEntry_FinishDate, ObjectId, Lag > 0 ? Lag : 0, Lag > 0);

			BindDefaultVisibility();
			BindDefaultValues();
		}
		#endregion

		#region ibCancel_Click
		private void ibCancel_Click(object sender, ImageClickEventArgs e)
		{
			BindDefaultVisibility();
			BindDefaultValues();
		}
		#endregion

		#region IPageTemplateTitle Members
		public string Modify(string oldValue)
		{
			return LocRM2.GetString("tReminderNotifications");
		}
		#endregion
	}
}
