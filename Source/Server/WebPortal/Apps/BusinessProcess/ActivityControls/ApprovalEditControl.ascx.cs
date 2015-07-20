using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Assignments;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.ActivityControls
{
	public partial class ApprovalEditControl : MCDataSavedControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region DataBind
		public override void DataBind()
		{
			UserList.DataSource = User.GetListActive();
			UserList.DataBind();

			if (DataItem != null)
			{
				PropertyValueCollection properties = WorkflowActivityWrapper.GetAssignmentProperties(DataItem);

				object prop;

				// User
				prop = properties[AssignmentEntity.FieldUserId];
				if (prop != null)
					CHelper.SafeSelect(UserList, prop.ToString());

				// Subject
				prop = properties[AssignmentEntity.FieldSubject];
				if (prop != null)
					SubjectText.Text = prop.ToString();
			}
		}
		#endregion

		#region Save
		public override object Save(object dataItem)
		{
			PropertyValueCollection properties = WorkflowActivityWrapper.GetAssignmentProperties(dataItem);
			properties[AssignmentEntity.FieldSubject] = SubjectText.Text.Trim();
			properties[AssignmentEntity.FieldUserId] = int.Parse(UserList.SelectedValue);

			return base.Save(dataItem);
		}
		#endregion
	}
}