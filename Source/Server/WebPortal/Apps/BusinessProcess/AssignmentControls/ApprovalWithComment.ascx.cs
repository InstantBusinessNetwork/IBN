using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.WebControls;

using Mediachase.Ibn.Assignments;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using System.Globalization;
using Mediachase.IBN.Business.ControlSystem;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.AssignmentControls
{
	public partial class ApprovalWithComment : MCDataBoundControl, IItemCommand
	{
		#region AssignmentId
		protected PrimaryKeyId? AssignmentId
		{
			set
			{
				ViewState["AssignmentId"] = value;
			}
			get
			{
				PrimaryKeyId? retval = null;
				if (ViewState["AssignmentId"] != null)
					retval = (PrimaryKeyId)ViewState["AssignmentId"];
				return retval;
			}
		}
		#endregion

		#region ContainerKey, ContainerName
		protected string ContainerKey
		{
			get
			{
				if (Request["ProjectId"] != null)
					return "ProjectId_" + Request["ProjectId"];
				else if (Request["IncidentId"] != null)
					return "IncidentId_" + Request["IncidentId"];
				else if (Request["TaskId"] != null)
					return "TaskId_" + Request["TaskId"];
				else if (Request["DocumentId"] != null)
					return "DocumentId_" + Request["DocumentId"];
				else if (Request["EventId"] != null)
					return "EventId_" + Request["EventId"];
				else if (Request["ToDoId"] != null)
				{
					using (IDataReader reader = Mediachase.IBN.Business.ToDo.GetToDo(int.Parse(Request["ToDoId"]), false))
					{
						if (reader.Read())
						{
							if (reader["TaskId"] != DBNull.Value)
								return "TaskId_" + reader["TaskId"].ToString();
							else if (reader["DocumentId"] != DBNull.Value)
								return "DocumentId_" + reader["DocumentId"].ToString();
							else
								return "ToDoId_" + Request["ToDoId"];
						}
						else
							return "ToDoId_" + Request["ToDoId"];
					}
				}
				else
					return "Workspace";
			}
		}

		protected string ContainerName
		{
			get
			{
				return "FileLibrary";
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region DataBind
		public override void DataBind()
		{
			AssignmentEntity entity = (AssignmentEntity)this.DataItem;
			AssignmentId = entity.PrimaryKeyId;

			if (!String.IsNullOrEmpty(entity.Subject))
				SubjectLabel.Text = entity.Subject;
			else
				SubjectLabel.Text = GetGlobalResourceObject("IbnFramework.BusinessProcess", "NoSubject").ToString();

			if (entity.PlanFinishDate.HasValue)
			{
				DueDateLabel.Text = String.Concat(entity.PlanFinishDate.Value.ToShortDateString(), " ", entity.PlanFinishDate.Value.ToShortTimeString());
				if (entity.TimeStatus == (int)AssignmentTimeStatus.OverDue)
					DueDateLabel.CssClass = "ibn-error";

				DueDateRow.Visible = true;
			}
			else
			{
				DueDateRow.Visible = false;
			}
		}
		#endregion

		#region ApproveButton_ServerClick
		protected void ApproveButton_ServerClick(object sender, EventArgs e)
		{
			AssignmentEntity assignment = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, AssignmentId.Value);
			assignment.ExecutionResult = (int)AssignmentExecutionResult.Accepted;

			// files
			string filesText = string.Empty;
			FileStorage fs = FileStorage.Create(ContainerName, ContainerKey);
			FileInfo[] files = fs.GetFiles();
			if (files != null && files.Length > 0)
			{
				foreach (FileInfo file in files)
				{
					if (!String.IsNullOrEmpty(filesText))
						filesText += ", ";
					filesText += file.Name;
				}

				filesText = String.Concat("<i>Файлы: ", filesText, "</i>");
			}

			if (!String.IsNullOrEmpty(filesText))
				assignment.Comment = String.Format(CultureInfo.InvariantCulture,
					"{0}\r\n{1}",
					filesText, CommentText.Text);
			else
				assignment.Comment = CommentText.Text;

			BusinessManager.Execute(new CloseAssignmentRequest(assignment));

			// Notify parent control
			ItemCommand(sender, e);
		}
		#endregion

		#region DenyButton_ServerClick
		protected void DenyButton_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			AssignmentEntity assignment = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, AssignmentId.Value);
			assignment.ExecutionResult = (int)AssignmentExecutionResult.Declined;
			assignment.Comment = CommentText.Text;
			BusinessManager.Execute(new CloseAssignmentRequest(assignment));

			// Notify parent control
			ItemCommand(sender, e);
		}
		#endregion

		#region IItemCommand Members
		public event ItemCommandEventHandler ItemCommand;
		#endregion
	}
}
