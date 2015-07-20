using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Data;
using System.Globalization;
using Mediachase.Ibn.Assignments;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Apps.BusinessProcess.Modules
{
	public partial class AssignmentEdit : System.Web.UI.UserControl
	{
		#region AssignmentId
		protected PrimaryKeyId AssignmentId
		{
			get
			{
				PrimaryKeyId retval = PrimaryKeyId.Empty;
				if (!String.IsNullOrEmpty(Request.QueryString["ObjectId"]))
					retval = PrimaryKeyId.Parse(Request.QueryString["ObjectId"]);

				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindInfo();
				BindData();
			}
		}

		#region BindInfo
		private void BindInfo()
		{
			SaveButton.CustomImage = ResolveUrl("~/layouts/images/saveitem.gif");
			SaveButton.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Save").ToString();
			CancelButton.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Cancel").ToString();

			if (!String.IsNullOrEmpty(Request["closeFramePopup"]))
				CancelButton.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{;}}", Request["closeFramePopup"]));
			else
				CancelButton.Attributes.Add("onclick", "window.close();");
		}
		#endregion

		#region BindData
		private void BindData()
		{
			AssignmentEntity assignment = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, AssignmentId);
			if (assignment != null)
			{
				SubjectText.Text = assignment.Subject;
				if (assignment.PlanFinishDate.HasValue)
					DueDatePicker.SelectedDate = assignment.PlanFinishDate.Value;
			}
		}
		#endregion

		#region SaveButton_ServerClick
		protected void SaveButton_ServerClick(object sender, EventArgs e)
		{
			AssignmentEntity assignment = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, AssignmentId);
			if (assignment != null)
			{
				assignment.Subject = SubjectText.Text;
				if (DueDatePicker.SelectedDate != DateTime.MinValue)
					assignment.PlanFinishDate = DueDatePicker.SelectedDate;
				else
					assignment.PlanFinishDate = null;
				BusinessManager.Update(assignment);
			}

			// Close popup
			if (!String.IsNullOrEmpty(Request["closeFramePopup"]))
			{
				CommandParameters cp = new CommandParameters();
				if (Request["ReturnCommand"] != null)
					cp.CommandName = Request["ReturnCommand"];
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, string.Empty, true);
			}
			else
			{
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					"<script language='javascript'>" +
					"try {window.opener.location.href=window.opener.location.href;}" +
					"catch (e){} window.close();</script>");
			}
		} 
		#endregion
	}
}