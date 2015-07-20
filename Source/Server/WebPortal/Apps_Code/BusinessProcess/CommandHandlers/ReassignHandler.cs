using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Assignments;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.CommandHandlers
{
	public class ReassignHandler : ICommand
	{
		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				if (cp.CommandArguments["SelectedValue"] == null)
					throw new ArgumentException("SelectedValue is null for ReassignHandler");
				if (cp.CommandArguments["ObjectId"] == null)
					throw new ArgumentException("ObjectId is null for ReassignHandler");

				int userId = int.Parse(cp.CommandArguments["SelectedValue"]);
				PrimaryKeyId assignmentId = PrimaryKeyId.Parse(cp.CommandArguments["ObjectId"]);

				AssignmentEntity assignment = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, assignmentId);
				assignment.UserId = userId;
				BusinessManager.Update(assignment);

				CHelper.AddToContext("RebindAssignments", true);
			}
		}
	}
}
