using System;
using System.Collections;
using System.Data;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.UI.Web.Util;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls
{
	public partial class TimeTrackingInfo : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.TimeTracking.Resources.strTimeTrackingInfo", typeof(TimeTrackingInfo).Assembly);

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

		#region ProjectId
		protected int ProjectId
		{
			get
			{
				return CommonHelper.GetRequestInteger(Request, "ProjectID", 0);
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
				else if (ProjectId > 0)
					retval = ProjectId;

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
				else if (ProjectId > 0)
					retval = ObjectTypes.Project;

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
			BindData();
			BindToolbar();
		}

		#region BindToolbar
		private void BindToolbar()
		{
			if (ProjectId > 0)
				blockHeaderMain.AddText(LocRM.GetString("Timesheet"));
			else
				blockHeaderMain.AddText(LocRM.GetString("MyTimesheet"));
		}
		#endregion

		#region BindData
		private void BindData()
		{
			if (ProjectId > 0)
			{
				using (IDataReader reader = Project.GetProject(ProjectId))
				{
					if (reader.Read())
					{
						labelTaskTime.Text = CommonHelper.GetHours((int)reader["TaskTime"]);
						labelSpentTime.Text = CommonHelper.GetHours((int)reader["TotalMinutes"]);
					}
				}
			}
			else
			{
				int taskTime = GetTaskTime();
				labelTaskTime.Text = CommonHelper.GetHours(taskTime);

				labelSpentTime.Text = CommonHelper.GetHours(TimeTrackingManager.GetTotalHoursByObject(ObjectId, ObjectTypeId));
			}

			if (TaskId > 0)
				tdTaskTime.Visible = PortalConfig.CommonTaskAllowViewTaskTimeField;
			if (IncidentId > 0)
				tdTaskTime.Visible = PortalConfig.CommonIncidentAllowViewTaskTimeField;
			if (DocumentId > 0)
				tdTaskTime.Visible = PortalConfig.CommonDocumentAllowViewTaskTimeField;
			if (ToDoId > 0)
				tdTaskTime.Visible = PortalConfig.CommonToDoAllowViewTaskTimeField;
			if (ProjectId > 0)
				tdTaskTime.Visible = PortalConfig.GeneralAllowTaskTimeField;
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
	}
}