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
	public class DeleteSelectedOrgHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string[] selectedElements = EntityGrid.GetCheckedCollection(((CommandManager)Sender).Page, cp.CommandArguments["GridId"]);
				string deleteType = cp.CommandArguments["DeleteType"];	// 0: org only; 1: org & contacts

				if (selectedElements != null)
				{
					string className = OrganizationEntity.GetAssignedMetaClassName();
					int errorCount = 0;

					foreach (string elem in selectedElements)
					{
						string id = elem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0];
						PrimaryKeyId key = PrimaryKeyId.Parse(id);

						try
						{
							DeleteRequest request = new DeleteRequest(className, key);
							request.Parameters.Add(OrganizationRequestParameters.Delete_RelatedContactAction, (deleteType == "0") ? RelatedContactAction.Detach : RelatedContactAction.Delete);
							BusinessManager.Execute(request);
						}
						catch (Exception ex)
						{
							CHelper.GenerateErrorReport(ex);
							errorCount++;
						}
					}


					if (errorCount > 0)
						((CommandManager)Sender).InfoMessage = CHelper.GetResFileString("{IbnFramework.Common:NotAllSelectedItemsWereProcessed}");
				}
			}
		}

		#endregion
	}
}
