using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.Modules
{
	public partial class WorkflowListByObject : System.Web.UI.UserControl
	{
		#region PlaceName
		protected string PlaceName
		{
			get
			{
				string retval = String.Empty;
				if (!String.IsNullOrEmpty(Request.QueryString["DocumentId"]))
					retval = "DocumentView";
				else if (!String.IsNullOrEmpty(Request.QueryString["TaskId"]))
					retval = "TaskView";
				else if (!String.IsNullOrEmpty(Request.QueryString["ToDoId"]))
					retval = "ToDoView";
				else if (!String.IsNullOrEmpty(Request.QueryString["IncidentId"]))
					retval = "IssueView";
				else if (!String.IsNullOrEmpty(Request.QueryString["EventId"]))
					retval = "CalendarEntryView";
				else if (!String.IsNullOrEmpty(Request.QueryString["ProjectId"]))
					retval = "ProjectView";
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			PrepareContext();

			if (!IsPostBack)
			{
				MainGrid.DoPadding = true;
				MainGrid.ShowCheckBoxes = true;
				MainGrid.ClassName = "WorkflowInstance";
				MainGrid.PlaceName = PlaceName;
				MainGrid.ViewName = "WorkflowInstancesByObject";
				MainGrid.ProfileName = "WorkflowInstancesByDocument";

				MainGrid.DataBind();
			}
		}

		#region PrepareContext
		// We use the Context items in the following file:
		// ~/Apps/BusinessProcess/Config/ListViewUI/WorkflowInstance.WorkflowInstancesByObject.xml
		// and in the CreateByTemplateEnableHandler
		private void PrepareContext()
		{
			string ownerName = String.Empty;
			int ownerId = -1;
			int ownerTypeId = (int)ObjectTypes.UNDEFINED;
			if (!String.IsNullOrEmpty(Request.QueryString["DocumentId"]))
			{
				ownerName = "OwnerDocumentId";
				ownerTypeId = (int)ObjectTypes.Document;
				ownerId = int.Parse(Request.QueryString["DocumentId"]);
			}
			else if (!String.IsNullOrEmpty(Request.QueryString["TaskId"]))
			{
				ownerName = "OwnerTaskId";
				ownerTypeId = (int)ObjectTypes.Task;
				ownerId = int.Parse(Request.QueryString["TaskId"]);
			}
			else if (!String.IsNullOrEmpty(Request.QueryString["ToDoId"]))
			{
				ownerName = "OwnerToDoId";
				ownerTypeId = (int)ObjectTypes.ToDo;
				ownerId = int.Parse(Request.QueryString["ToDoId"]);
			}
			else if (!String.IsNullOrEmpty(Request.QueryString["IncidentId"]))
			{
				ownerName = "OwnerIncidentId";
				ownerTypeId = (int)ObjectTypes.Issue;
				ownerId = int.Parse(Request.QueryString["IncidentId"]);
			}
			else if (!String.IsNullOrEmpty(Request.QueryString["EventId"]))
			{
				ownerName = "OwnerEventId";
				ownerTypeId = (int)ObjectTypes.CalendarEntry;
				ownerId = int.Parse(Request.QueryString["EventId"]);
			}
			else if (!String.IsNullOrEmpty(Request.QueryString["ProjectId"]))
			{
				ownerName = "OwnerProjectId";
				ownerTypeId = (int)ObjectTypes.Project;
				ownerId = int.Parse(Request.QueryString["ProjectId"]);
			}
			CHelper.AddToContext("OwnerName", ownerName);
			CHelper.AddToContext("OwnerTypeId", ownerTypeId);
			CHelper.AddToContext("OwnerId", ownerId);
		}
		#endregion
	}
}