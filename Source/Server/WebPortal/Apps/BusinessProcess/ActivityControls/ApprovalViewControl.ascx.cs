using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Assignments;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.ActivityControls
{
	public partial class ApprovalViewControl : MCDataBoundControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region DataBind
		public override void DataBind()
		{
			if (DataItem != null)
			{
				PropertyValueCollection properties = WorkflowActivityWrapper.GetAssignmentProperties(DataItem);

				object prop;

				// User
				prop = properties[AssignmentEntity.FieldUserId];
				if (prop != null)
				{
					UserLight user = UserLight.Load((int)prop);
					UserLabel.Text = user.DisplayName;
				}

				// Subject
				prop = properties[AssignmentEntity.FieldSubject];
				if (prop != null)
					SubjectLabel.Text = prop.ToString();
			}
		}
		#endregion
	}
}