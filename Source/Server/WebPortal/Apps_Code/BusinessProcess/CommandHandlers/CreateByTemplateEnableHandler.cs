using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Assignments;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Util;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.CommandHandlers
{
	public class CreateByTemplateEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;

			if (HttpContext.Current.Items.Contains("OwnerTypeId") && HttpContext.Current.Items.Contains("OwnerId"))
			{
				int ownerTypeId = (int)HttpContext.Current.Items["OwnerTypeId"];
				int ownerId = (int)HttpContext.Current.Items["OwnerId"];

				bool canUpdate = false;
				if (ownerTypeId == (int)ObjectTypes.Document)
					canUpdate = Document.CanUpdate(ownerId);
				else if (ownerTypeId == (int)ObjectTypes.Task)
					canUpdate = Task.CanUpdate(ownerId);
				else if (ownerTypeId == (int)ObjectTypes.ToDo)
					canUpdate = ToDo.CanUpdate(ownerId);
				else if (ownerTypeId == (int)ObjectTypes.Issue)
					canUpdate = Incident.CanUpdate(ownerId);

				if (canUpdate)
				{
					int projectId = CommonHelper.GetProjectIdByObjectIdObjectType(ownerId, ownerTypeId);

					FilterElementCollection filters = new FilterElementCollection();
					filters.Add(FilterElement.EqualElement(WorkflowDefinitionEntity.FieldSupportedIbnObjectTypes, ownerTypeId));
					if (projectId > 0)
					{
						// O.R. [2010-02-03]: Allow to select non-project templates for a project
						OrBlockFilterElement orBlock = new OrBlockFilterElement();
						orBlock.ChildElements.Add(FilterElement.EqualElement(WorkflowDefinitionEntity.FieldProjectId, projectId));
						orBlock.ChildElements.Add(FilterElement.IsNullElement(WorkflowDefinitionEntity.FieldProjectId));
						filters.Add(orBlock);
					}
					else
					{
						filters.Add(FilterElement.IsNullElement(WorkflowDefinitionEntity.FieldProjectId));
					}

					EntityObject[] items = BusinessManager.List(WorkflowDefinitionEntity.ClassName, filters.ToArray());
					if (items != null && items.Length > 0)
						retval = true;
				}
			}

			return retval;
		}
		#endregion
	}
}
