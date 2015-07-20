using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Assignments;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core;
using Mediachase.IBN.Business.Assignments;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.Modules
{
	public partial class WorkFlowInstanceView : System.Web.UI.UserControl
	{
		#region ObjectId
		protected Guid ObjectId
		{
			get
			{
				Guid retval = PrimaryKeyId.Empty;
				if (!String.IsNullOrEmpty(Request.QueryString["ObjectId"]))
					retval = new Guid(Request.QueryString["ObjectId"]);

				return retval;
			}
		}
		#endregion

		#region OwnerName
		protected string OwnerName
		{
			get
			{
				string retval = string.Empty;
				if (!String.IsNullOrEmpty(Request.QueryString["OwnerName"]))
					retval = Request.QueryString["OwnerName"];

				return retval;
			}
		}
		#endregion

		#region OwnerId
		protected int OwnerId
		{
			get
			{
				int retval = -1;
				if (!String.IsNullOrEmpty(Request.QueryString["OwnerId"]))
					retval = int.Parse(Request.QueryString["OwnerId"]);

				return retval;
			}
		}
		#endregion

		private const int indentSize = 30;

		BusinessProcessInfo info = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			info = WorkflowActivityWrapper.GetBusinessProcessInfo(ObjectId);

			if (!IsPostBack)
			{
				BindData();
				BindActivities();
			}

			BindToolbar();
		}

		#region BindToolbar
		private void BindToolbar()
		{
			MainHeader.Title = GetGlobalResourceObject("IbnFramework.BusinessProcess", "WorkflowInfo").ToString();

			string link = CHelper.GetLinkObjectViewByOwnerName(OwnerName, OwnerId.ToString());
			string text = CHelper.GetIconText(GetGlobalResourceObject("IbnFramework.Common", "Back").ToString(), ResolveClientUrl("~/Images/IbnFramework/cancel.GIF"));
			if (!String.IsNullOrEmpty(link))
				MainHeader.AddLink(text, link);
		}
		#endregion

		#region BindData
		private void BindData()
		{
			string containerKey = string.Empty;

			// WF Name
			WorkFlowLabel.Text = info.Name;

			// WF State
			if (info.State == BusinessProcessState.Active)
				StateLabel.Text = GetGlobalResourceObject("IbnFramework.BusinessProcess", "WFStateActive").ToString();
			else if (info.State == BusinessProcessState.Pending)
				StateLabel.Text = GetGlobalResourceObject("IbnFramework.BusinessProcess", "WFStatePending").ToString();
			else if (info.State == BusinessProcessState.Suspended)
				StateLabel.Text = GetGlobalResourceObject("IbnFramework.BusinessProcess", "WFStateSuspended").ToString();
			else
				StateLabel.Text = GetGlobalResourceObject("IbnFramework.BusinessProcess", "WFStateCompleted").ToString();

			// Owner Type
			if (String.Compare(OwnerName, WorkflowInstanceEntity.FieldOwnerDocumentId, true) == 0)
			{
				OwnerLiteral.Text = GetGlobalResourceObject("IbnFramework.Admin", "ObjectType_Document").ToString();
				OwnerLink.Text = CommonHelper.GetObjectTitle((int)Mediachase.IBN.Business.ObjectTypes.Document, OwnerId);

				containerKey = String.Concat("DocumentId_", OwnerId.ToString());
			}
			OwnerLink.NavigateUrl = CHelper.GetLinkObjectViewByOwnerName(OwnerName, OwnerId.ToString());

			// Current Date
			CurrentDateLiteral.Text = GetGlobalResourceObject("IbnFramework.BusinessProcess", "CurrentDate").ToString();
			DateTime userDate = Mediachase.IBN.Business.User.GetLocalDate(DateTime.UtcNow);
			CurrentDateLabel.Text = String.Concat(userDate.ToLongDateString(), " ", userDate.ToShortTimeString());

			// Files
			string containerName = "FileLibrary";
			string filesText = string.Empty;
			FileStorage fs = FileStorage.Create(containerName, containerKey);
			FileInfo[] files = fs.GetFiles();
			if (files != null && files.Length > 0)
			{
				foreach (FileInfo file in files)
				{
					if (!String.IsNullOrEmpty(filesText))
						filesText += ", ";
					filesText += file.Name;
				}
			}
			FilesLabel.Text = filesText;
		} 
		#endregion

		#region BindActivities
		private void BindActivities()
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Subject", typeof(string));
			dt.Columns.Add("User", typeof(string));
			dt.Columns.Add("State", typeof(int));
			dt.Columns.Add("Result", typeof(int));
			dt.Columns.Add("FinishDate", typeof(string));
			dt.Columns.Add("Comment", typeof(string));
			dt.Columns.Add("Indent", typeof(int));
			dt.Columns.Add("ClosedBy", typeof(string));
			dt.Columns.Add("PlanFinishDate", typeof(string));
			dt.Columns.Add("ReadOnly", typeof(bool));

			ProcessCollection(dt, info.Activities, 0);

			AssignmentList.DataSource = dt;
			AssignmentList.DataBind();

			if (dt.Rows.Count > 0)
			{
				AssignmentList.Visible = true;
				NoAssignmentsLabel.Visible = false;
			}
			else
			{
				AssignmentList.Visible = false;
				NoAssignmentsLabel.Visible = true;
				NoAssignmentsLabel.Text = GetGlobalResourceObject("IbnFramework.BusinessProcess", "NoAssignments").ToString();
			}
		} 
		#endregion

		#region ProcessCollection
		private void ProcessCollection(DataTable dt, ActivityInfoCollection activities, int level)
		{
			level++;

			foreach (Mediachase.Ibn.Assignments.ActivityInfo activityInfo in activities)
			{
				DataRow row = dt.NewRow();

				if (activityInfo.Type == ActivityType.Block)
					row["Subject"] = GetGlobalResourceObject("IbnFramework.BusinessProcess", "ParallelBlock").ToString();
				else
					row["Subject"] = (activityInfo.Subject != null) ? activityInfo.Subject : String.Empty;

				if (activityInfo.UserId.HasValue)
					row["User"] = CommonHelper.GetUserStatus(activityInfo.UserId.Value);
				row["State"] = (int)activityInfo.State;

				if (activityInfo.ExecutionResult.HasValue)
					row["Result"] = activityInfo.ExecutionResult.Value;
				if (activityInfo.ActualFinishDate.HasValue)
					row["FinishDate"] = String.Concat(activityInfo.ActualFinishDate.Value.ToShortDateString(), " ", activityInfo.ActualFinishDate.Value.ToShortTimeString());
				row["Comment"] = CHelper.ParseText(activityInfo.Comment, true, true, true);
				row["Indent"] = (level - 1) * indentSize;
				if (activityInfo.ClosedBy.HasValue)
				{
					row["ClosedBy"] = CommonHelper.GetUserStatus(activityInfo.ClosedBy.Value);
				}
				if (activityInfo.PlanFinishDate.HasValue)
					row["PlanFinishDate"] = String.Concat(activityInfo.PlanFinishDate.Value.ToShortDateString(), " ", activityInfo.PlanFinishDate.Value.ToShortTimeString());

				row["ReadOnly"] = false;
				PropertyValueCollection properties = activityInfo.AssignmentProperties;
				if (properties.Contains(AssignmentCustomProperty.ReadOnlyLibraryAccess))
				{
					bool? readOnly = properties[AssignmentCustomProperty.ReadOnlyLibraryAccess] as bool?;
					if (readOnly.HasValue)
						row["ReadOnly"] = readOnly.Value;
				}

				dt.Rows.Add(row);

				ProcessCollection(dt, activityInfo.Activities, level);
			}
		}
		#endregion

		#region GetSubject
		protected string GetSubject(string subject, bool readOnly)
		{
			string retval = subject;
			if (String.IsNullOrEmpty(subject))
				retval = GetGlobalResourceObject("IbnFramework.BusinessProcess", "NoSubject").ToString();

			retval = String.Concat("<span class=\"assignmentSubject\">", retval, "</span>");
			if (readOnly)
				retval += String.Concat(" <span class=\"readonly\">[", GetGlobalResourceObject("IbnFramework.BusinessProcess", "ReadOnly").ToString(), "]</span>");

			return retval;
		}
		#endregion

		#region GetResult
		protected string GetResult(object stateObj, object resultObj)
		{
			string retval = string.Empty;

			if (stateObj != null && stateObj != DBNull.Value)
			{
				int state = (int)stateObj;

				if (state == (int)AssignmentState.Closed)
				{
					if (resultObj != null && resultObj != DBNull.Value)
					{
						int result = (int)resultObj;

						retval = CHelper.GetResFileString(MetaEnum.GetFriendlyName(MetaDataWrapper.GetEnumByName("AssignmentExecutionResult"), result));
						if (result == (int)AssignmentExecutionResult.Accepted)
						{
							retval = String.Concat("<span class=\"resultAccepted\">", retval, "</span>");
						}
						else if (result == (int)AssignmentExecutionResult.Declined)
						{
							retval = String.Concat("<span class=\"resultDeclined\">", retval, "</span>");
						}
						else if (result == (int)AssignmentExecutionResult.Canceled)
						{
							retval = String.Concat("<span class=\"resultCanceled\">", retval, "</span>");
						}
					}
				}
				else
				{
					retval = CHelper.GetResFileString(MetaEnum.GetFriendlyName(MetaDataWrapper.GetEnumByName("AssignmentState"), state));
				}
			}
			return retval;
		}
		#endregion
	}
}