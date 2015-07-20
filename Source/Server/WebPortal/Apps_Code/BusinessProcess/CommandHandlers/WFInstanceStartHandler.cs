using System;
using System.Data;
using System.Configuration;
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
	public class WFInstanceStartHandler : ICommand
	{
		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				if (cp.CommandArguments["primaryKeyId"] == null)
					throw new ArgumentException("PrimaryKeyId is null for WFInstanceStartHandler");

				PrimaryKeyId pk = PrimaryKeyId.Parse(cp.CommandArguments["primaryKeyId"]);
				WorkflowInstanceEntity entity = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, pk);
				if (entity.PlanFinishDate.HasValue && entity.PlanFinishDate.Value <= Mediachase.IBN.Business.UserDateTime.UserNow)
				{
					CommandManager cm = Sender as CommandManager;
					cm.InfoMessage = CHelper.GetResFileString("{IbnFramework.BusinessProcess:StartInstanceWarning}");
				}
				else
					BusinessManager.Execute(new StartWorkflowInstanceRequest(pk));
			}
		}
	}
}
