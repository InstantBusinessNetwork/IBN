using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Web.UI.BusinessProcess
{
	public interface IItemCommand
	{
		event ItemCommandEventHandler ItemCommand;
	}

	public delegate void ItemCommandEventHandler(object sender, System.EventArgs e);
}
