using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel;

namespace Mediachase.Ibn.AssignmentsUI.Modules.Primitives
{
	interface IWorkflowPrimitive
	{
		Activity DataItem { get; set; }
	}
}
