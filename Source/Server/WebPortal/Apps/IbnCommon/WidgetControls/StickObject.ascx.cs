using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Assignments;
using Mediachase.IBN.Business;
using IbnCalendar = Mediachase.IBN.Business.Calendar;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Data;

namespace Mediachase.UI.Web.Shell.Modules
{
	public partial class StickObject : System.Web.UI.UserControl
	{
		#region ObjectId
		public int ObjectId
		{
			get
			{
				int retval = -1;
				if (!String.IsNullOrEmpty(Request.QueryString["ObjectId"]))
					retval = int.Parse(Request.QueryString["ObjectId"]);

				return retval;
			}
		}
		#endregion

		#region ObjectTypeId
		public int ObjectTypeId 
		{
			get
			{
				int retval = (int)ObjectTypes.UNDEFINED;
				if (!String.IsNullOrEmpty(Request.QueryString["ObjectTypeId"]))
					retval = int.Parse(Request.QueryString["ObjectTypeId"]);

				return retval;
			}
		}
		#endregion

		#region UserId
		public int UserId
		{
			get
			{
				int retval = Security.CurrentUser.UserID;
				if (!String.IsNullOrEmpty(Request.QueryString["UserId"]))
					retval = int.Parse(Request.QueryString["UserId"]);

				return retval;
			}
		}
		#endregion

		#region AssignmentId
		public Guid? AssignmentId
		{
			get
			{
				Guid? retval = null;
				if (!String.IsNullOrEmpty(Request.QueryString["AssignmentId"]))
					retval = new Guid(Request.QueryString["AssignmentId"]);

				return retval;
			}
		}
		#endregion

		#region CommandName
		public string CommandName 
		{
			get
			{
				return Request.QueryString["CommandName"];
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
				BindData();
		}

		#region BindData
		private void BindData()
		{
			// Positions
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("PositionId", typeof(int)));
			dt.Columns.Add(new DataColumn("PositionName", typeof(string)));

			bool objectIsSticked = IbnCalendar.CheckStickedObject(UserId, ObjectId, ObjectTypeId, AssignmentId);
			int stickedCount = IbnCalendar.GetStickedObjectsCount(UserId);
			if (!objectIsSticked)
				stickedCount++;

			for (int i = 1; i <= stickedCount; i++)
			{
				DataRow row = dt.NewRow();
				row["PositionId"] = i;
				row["PositionName"] = String.Format(CultureInfo.InvariantCulture, 
					"{0} {1}", GetGlobalResourceObject("IbnFramework.Calendar", "AtPositionN").ToString(), i);
				dt.Rows.Add(row);
			}
			if (objectIsSticked)
			{
				DataRow row = dt.NewRow();
				row["PositionId"] = -1;
				row["PositionName"] = String.Concat("[ ", GetGlobalResourceObject("IbnFramework.Calendar", "Unpin").ToString(), " ]");
				dt.Rows.Add(row);
			}

			PositionList.DataSource = dt;
			PositionList.DataBind();

			// Priorities
			PriorityList.DataSource = Project.GetListPriorities();
			PriorityList.DataBind();

			// strings
			ObjectLabel.Text = CommonHelper.GetObjectTitle(ObjectTypeId, ObjectId);
		}
		#endregion

		#region PositionList_ItemCommand
		protected void PositionList_ItemCommand(object source, DataListCommandEventArgs e)
		{
			int positionId = int.Parse(e.CommandArgument.ToString());

			if (positionId > 0)
				IbnCalendar.AddStickedObject(UserId, ObjectId, ObjectTypeId, AssignmentId, positionId);
			else
				IbnCalendar.DeleteStickedObject(UserId, ObjectId, ObjectTypeId, AssignmentId);

			CommandParameters cp = new CommandParameters(CommandName);
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
		#endregion

		#region PriorityList_ItemCommand
		protected void PriorityList_ItemCommand(object source, DataListCommandEventArgs e)
		{
			int priorityId = int.Parse(e.CommandArgument.ToString());

			if (ObjectTypeId == (int)ObjectTypes.Task)
				Task2.UpdatePriority(ObjectId, priorityId);
			else if (ObjectTypeId == (int)ObjectTypes.ToDo)
				ToDo2.UpdatePriority(ObjectId, priorityId);
			else if (ObjectTypeId == (int)ObjectTypes.Issue)
				Issue2.UpdatePriority(ObjectId, priorityId);
			else if (ObjectTypeId == (int)ObjectTypes.Document)
			{
				if (AssignmentId.HasValue)
				{
					AssignmentEntity assignment = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, (PrimaryKeyId)AssignmentId.Value);
					if (priorityId == (int)Priority.Low)
						assignment.Priority = (int)AssignmentPriority.Low;
					else if (priorityId == (int)Priority.Normal)
						assignment.Priority = (int)AssignmentPriority.Normal;
					else if (priorityId == (int)Priority.High)
						assignment.Priority = (int)AssignmentPriority.High;
					else if (priorityId == (int)Priority.VeryHigh)
						assignment.Priority = (int)AssignmentPriority.VeryHigh;
					else
						assignment.Priority = (int)AssignmentPriority.Urgent;

					BusinessManager.Update(assignment);
				}
			}

			CommandParameters cp = new CommandParameters(CommandName);
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString(), true);
		}
		#endregion
	}
}