using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Assignments;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.CommandHandlers
{
	public class BackToOwnerObjectHandler : ICommand
	{
		public void Invoke(object Sender, object Element)
		{
			NameValueCollection qs = ((Control)Sender).Page.Request.QueryString;
			if (!String.IsNullOrEmpty(qs["ObjectId"]))
			{
				WorkflowInstanceEntity entity = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, PrimaryKeyId.Parse(qs["ObjectId"]));
				if (entity != null)
				{
					if (entity.OwnerDocumentId.HasValue)
						((Control)Sender).Page.Response.Redirect(CHelper.GetLinkObjectViewByOwnerName("ownerdocumentid", entity.OwnerDocumentId.ToString()));
				}
			}

			if (!String.IsNullOrEmpty(qs["Id"]))
			{
				WorkflowInstanceEntity entity = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, PrimaryKeyId.Parse(qs["Id"]));
				if (entity != null)
				{
					if (entity.OwnerDocumentId.HasValue)
						((Control)Sender).Page.Response.Redirect(CHelper.GetLinkObjectViewByOwnerName("ownerdocumentid", entity.OwnerDocumentId.ToString()));
				}
			}

		}
	}
}
