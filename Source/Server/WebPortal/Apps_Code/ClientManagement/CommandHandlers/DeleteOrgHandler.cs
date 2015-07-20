using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Apps.MetaUI.Grid;
using Mediachase.Ibn.Clients;

namespace Mediachase.Ibn.Web.UI.ClientManagement.CommandHandlers
{
	public class DeleteOrgHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				PrimaryKeyId key = PrimaryKeyId.Parse(cp.CommandArguments["ObjectId"]);
				string deleteType = cp.CommandArguments["DeleteType"];	// 0: org only; 1: org & contacts

				int errorCount = 0;
				string className = OrganizationEntity.GetAssignedMetaClassName();
				DeleteRequest request = new DeleteRequest(className, key);
				request.Parameters.Add(OrganizationRequestParameters.Delete_RelatedContactAction, (deleteType == "0") ? RelatedContactAction.Detach : RelatedContactAction.Delete);

				try
				{
					BusinessManager.Execute(request);
				}
				catch (Exception ex)
				{
					CHelper.GenerateErrorReport(ex);
					errorCount++;
				}

				if (errorCount > 0)
					((CommandManager)Sender).InfoMessage = CHelper.GetResFileString("{IbnFramework.Common:ActionWasNotProcessed}");
				else
					((Control)Sender).Page.Response.Redirect(CHelper.GetLinkEntityList(className));
			}
		}

		#endregion
	}
}
