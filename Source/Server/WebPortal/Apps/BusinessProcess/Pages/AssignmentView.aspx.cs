using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Assignments;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.Pages
{
	public partial class AssignmentView : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Guid assignmentId = new Guid(Request.QueryString["id"]);
			AssignmentEntity entity = null;

			try
			{
				entity = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, (PrimaryKeyId)assignmentId);
			}
			catch
			{
			}

			if (entity != null && entity.OwnerDocumentId.HasValue)
				Response.Redirect(CHelper.GetAssignmentLink(assignmentId.ToString(), (int)ObjectTypes.Document, entity.OwnerDocumentId.Value));
			else
				Response.Redirect("~/Common/NotExistingId.aspx?AssignmentId=" + assignmentId.ToString());
		}
	}
}
