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
	public class WFInstanceTerminateHandler : ICommand
	{
		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				if (cp.CommandArguments["primaryKeyId"] == null)
					throw new ArgumentException("PrimaryKeyId is null for WFInstanceTerminateHandler");

				PrimaryKeyId pk = PrimaryKeyId.Parse(cp.CommandArguments["primaryKeyId"]);

				BusinessManager.Execute(new TerminateWorkflowInstanceRequest(pk));
			}
		}
	}
}
