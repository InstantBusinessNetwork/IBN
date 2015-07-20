using System;
using System.Data;
using System.Collections;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;
using Mediachase.IbnNext.TimeTracking;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls
{
	public partial class TimeTrackingInfoWithAdd : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.TimeTracking.Resources.strTimeTrackingInfo", typeof(TimeTrackingInfoWithAdd).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strQuickTracking", typeof(TimeTrackingInfoWithAdd).Assembly);

		#region TaskId
		protected int TaskId
		{
			get
			{
				return CommonHelper.GetRequestInteger(Request, "TaskID", 0);
			}
		}
		#endregion

		#region IncidentId
		protected int IncidentId
		{
			get
			{
				return CommonHelper.GetRequestInteger(Request, "IncidentID", 0);
			}
		}
		#endregion

		#region DocumentId
		protected int DocumentId
		{
			get
			{
				return CommonHelper.GetRequestInteger(Request, "DocumentID", 0);
			}
		}
		#endregion

		#region EventId
		protected int EventId
		{
			get
			{
				return CommonHelper.GetRequestInteger(Request, "EventID", 0);
			}
		}
		#endregion

		#region ToDoId
		protected int ToDoId
		{
			get
			{
				return CommonHelper.GetRequestInteger(Request, "ToDoID", 0);
			}
		}
		#endregion

		#region ObjectId
		protected int ObjectId
		{
			get
			{
				int retval = -1;

				if (TaskId > 0)
					retval = TaskId;
				else if (IncidentId > 0)
					retval = IncidentId;
				else if (DocumentId > 0)
					retval = DocumentId;
				else if (EventId > 0)
					retval = EventId;
				else if (ToDoId > 0)
					retval = ToDoId;

				return retval;
			}
		}
		#endregion

		#region ObjectTypeId
		protected int ObjectTypeId
		{
			get
			{
				ObjectTypes retval = ObjectTypes.UNDEFINED;

				if (TaskId > 0)
					retval = ObjectTypes.Task;
				else if (IncidentId > 0)
					retval = ObjectTypes.Issue;
				else if (DocumentId > 0)
					retval = ObjectTypes.Document;
				else if (EventId > 0)
					retval = ObjectTypes.CalendarEntry;
				else if (ToDoId > 0)
					retval = ObjectTypes.ToDo;

				return (int)retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Configuration.TimeTrackingModule)
			{
				this.Visible = false;
				return;
			}
			if (!IsPostBack)
			{
				BindToolbar();
				BindData();

				dtc.SelectedDate = UserDateTime.UserToday;
			}

			UpdateButton.Text = LocRM.GetString("Add");
			UpdateButton.Attributes.Add("onclick", "DisableButtons(this);");
		}

		#region BindToolbar
		private void BindToolbar()
		{
			MainBlockHeader.Title = LocRM.GetString("MyTimesheet");
		}
		#endregion

		#region BindData
		private void BindData()
		{
			int taskTime = GetTaskTime();
			TaskTimeLabel.Text = CommonHelper.GetHours(taskTime);

			SpentTimeLabel.Text = CommonHelper.GetHours(TimeTrackingManager.GetTotalHoursByObject(ObjectId, ObjectTypeId));

			TimesheetHours.Value = DateTime.MinValue;

			if (TaskId > 0)
				tdTaskTime.Visible = PortalConfig.CommonTaskAllowViewTaskTimeField;
			if (IncidentId > 0)
				tdTaskTime.Visible = PortalConfig.CommonIncidentAllowViewTaskTimeField;
			if (DocumentId > 0)
				tdTaskTime.Visible = PortalConfig.CommonDocumentAllowViewTaskTimeField;
			if (ToDoId > 0)
				tdTaskTime.Visible = PortalConfig.CommonToDoAllowViewTaskTimeField;
		}
		#endregion

		#region GetTaskTime
		private int GetTaskTime()
		{
			int minutes = 0;
			if (TaskId > 0)
				using (IDataReader reader = Task.GetTask(TaskId, false))
				{
					if (reader.Read())
						minutes = (int)reader["TaskTime"];
				}
			if (IncidentId > 0)
				using (IDataReader reader = Incident.GetIncident(IncidentId, false))
				{
					if (reader.Read())
						minutes = (int)reader["TaskTime"];
				}
			if (DocumentId > 0)
				using (IDataReader reader = Document.GetDocument(DocumentId, false))
				{
					if (reader.Read())
						minutes = (int)reader["TaskTime"];
				}
			if (EventId > 0)
				using (IDataReader reader = CalendarEntry.GetEvent(EventId, false))
				{
					if (reader.Read())
						minutes = (int)reader["TaskTime"];
				}
			if (ToDoId > 0)
				using (IDataReader reader = Mediachase.IBN.Business.ToDo.GetToDo(ToDoId, false))
				{
					if (reader.Read())
						minutes = (int)reader["TaskTime"];
				}
			return minutes;
		}
		#endregion

		#region UpdateButton_Click
		protected void UpdateButton_Click(object sender, System.EventArgs e)
		{
			string hours = String.Format("{0:H:mm}", TimesheetHours.Value);
			if (hours == "")
				hours = "0:00";
			string[] parts = hours.Split(':');
			int minutes = 0;
			minutes = int.Parse(parts[0]) * 60;
			if (parts.Length > 1)
				minutes += int.Parse(parts[1]);

			if (minutes > 0)
			{
				DateTime dt = dtc.SelectedDate;
				int projectId = CommonHelper.GetProjectIdByObjectIdObjectType(ObjectId, ObjectTypeId);

				if (!Mediachase.IBN.Business.TimeTracking.CanUpdate(dt, projectId))
				{
					TimesheetHoursValidator.ErrorMessage = LocRM2.GetString("tWrongDate");
					TimesheetHoursValidator.IsValid = false;
					return;
				}

				string title = CommonHelper.GetObjectTitle(ObjectTypeId, ObjectId);

				try
				{
					TimeTrackingManager.AddHoursForEntryByObject(
						ObjectId,
						ObjectTypeId,
						title,
						projectId,
						Mediachase.IBN.Business.Security.UserID,
						Mediachase.IBN.Business.TimeTracking.GetWeekStart(dt),
						Mediachase.IBN.Business.TimeTracking.GetDayNum(dt),
						minutes,
						true);
				}
				catch (AccessDeniedException)
				{
					TimesheetHoursValidator.ErrorMessage = LocRM2.GetString("tWrongDate");
					TimesheetHoursValidator.IsValid = false;
					return;
				}

				BindData();
			}
		}
		#endregion
	}
}