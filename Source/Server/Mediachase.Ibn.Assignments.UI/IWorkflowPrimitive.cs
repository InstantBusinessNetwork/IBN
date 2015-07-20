using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel;

namespace Mediachase.Ibn.Assignments.UI
{
	public interface IWorkflowPrimitive
	{
		IbnActivity DataItem { get; set; }
	}
}
