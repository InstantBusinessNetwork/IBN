using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Assignments;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Core;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.Modules
{
	public partial class AssignmentsInfo : MCDataBoundControl
	{
		private const int indentSize = 50;

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region DataBind
		public override void DataBind()
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("AssignmentId", typeof(string));
			dt.Columns.Add("Subject", typeof(string));
			dt.Columns.Add("User", typeof(string));
			dt.Columns.Add("State", typeof(string));
			dt.Columns.Add("Result", typeof(int));
			dt.Columns.Add("FinishDate", typeof(string));
			dt.Columns.Add("Comment", typeof(string));
			dt.Columns.Add("Indent", typeof(int));
			dt.Columns.Add("ClosedBy", typeof(string));

			WorkflowInstanceEntity wfEntity = (WorkflowInstanceEntity)DataItem;

			// Filter: 
			//	1: WorkflowInstanceId = wfEntity.PrimaryKeyId, 
			//	2: ParentAssignmentId IS NULL (other elements we'll get via the recursion)
			FilterElementCollection fec = new FilterElementCollection();
			fec.Add(FilterElement.EqualElement(AssignmentEntity.FieldWorkflowInstanceId, wfEntity.PrimaryKeyId.Value));
			fec.Add(FilterElement.IsNullElement(AssignmentEntity.FieldParentAssignmentId));

			// Sorting
			SortingElementCollection sec = new SortingElementCollection();
			sec.Add(new SortingElement(AssignmentEntity.FieldCreated, SortingElementType.Asc));

			EntityObject[] assignments = BusinessManager.List(AssignmentEntity.ClassName, fec.ToArray(), sec.ToArray());

			ProcessCollection(dt, assignments, wfEntity, 0);

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
		private void ProcessCollection(DataTable dt, EntityObject[] assignments, WorkflowInstanceEntity wfEntity, int level)
		{
			level++;

			foreach (AssignmentEntity assignment in assignments)
			{
				DataRow row = dt.NewRow();
				row["AssignmentId"] = assignment.PrimaryKeyId.ToString();
				row["Subject"] = assignment.Subject;
				if (assignment.UserId.HasValue)
					row["User"] = CommonHelper.GetUserStatus(assignment.UserId.Value);
				row["State"] = CHelper.GetResFileString(MetaEnum.GetFriendlyName(MetaDataWrapper.GetEnumByName("AssignmentState"), assignment.State));
				if (assignment.ExecutionResult.HasValue)
					row["Result"] = assignment.ExecutionResult.Value;
				if (assignment.ActualFinishDate.HasValue)
					row["FinishDate"] = String.Concat(assignment.ActualFinishDate.Value.ToShortDateString(), " ", assignment.ActualFinishDate.Value.ToShortTimeString());
				row["Comment"] = CHelper.ParseText(assignment.Comment, true, true, false);
				row["Indent"] = (level - 1) * indentSize;
				if (assignment.ClosedBy.HasValue)
				{
					row["ClosedBy"] = CommonHelper.GetUserStatus(assignment.ClosedBy.Value);
				}
				dt.Rows.Add(row);

				// Filter: 
				//	1: WorkflowInstanceId = wfEntity.PrimaryKeyId, 
				//	2: ParentAssignmentId = assignment.PrimaryKeyId
				FilterElementCollection fec = new FilterElementCollection();
				fec.Add(FilterElement.EqualElement(AssignmentEntity.FieldWorkflowInstanceId, wfEntity.PrimaryKeyId.Value));
				fec.Add(FilterElement.EqualElement(AssignmentEntity.FieldParentAssignmentId, assignment.PrimaryKeyId.Value));

				// Sorting
				SortingElementCollection sec = new SortingElementCollection();
				sec.Add(new SortingElement(AssignmentEntity.FieldCreated, SortingElementType.Asc));

				EntityObject[] children = BusinessManager.List(AssignmentEntity.ClassName, fec.ToArray(), sec.ToArray());
				
				ProcessCollection(dt, children, wfEntity, level);
			}
		}
		#endregion

		#region GetSubject
		protected string GetSubject(string subject)
		{
			if (String.IsNullOrEmpty(subject))
				subject = GetGlobalResourceObject("IbnFramework.BusinessProcess", "NoSubject").ToString();

			return subject;
		} 
		#endregion

		#region GetResult
		protected string GetResult(object resultObj)
		{
			string retval = string.Empty;

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

			return retval;
		} 
		#endregion
	}
}
