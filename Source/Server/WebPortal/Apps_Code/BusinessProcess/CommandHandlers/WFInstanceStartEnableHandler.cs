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


namespace Mediachase.Ibn.Web.UI.BusinessProcess.CommandHandlers
{
	public class WFInstanceStartEnableHandler : ICommandEnableHandler
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

				if (canUpdate && Element is CommandParameters)
				{
					CommandParameters cp = (CommandParameters)Element;

					if (cp.CommandArguments["primaryKeyId"] == null)
						throw new ArgumentException("PrimaryKeyId is null for WFInstanceStartEnableHandler");

					PrimaryKeyId pk = PrimaryKeyId.Parse(cp.CommandArguments["primaryKeyId"]);
					WorkflowInstanceEntity entity = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, pk);
					retval = (entity.State == (int)BusinessProcessState.Pending);
				}
			}

			return retval;
		}
		#endregion
	}
}
