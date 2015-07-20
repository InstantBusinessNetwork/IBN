using System;
using System.Collections.Generic;
using System.Web;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.CommandHandlers
{
	public class PrinterVersionHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			CHelper.AddToContext(CHelper.PrinterVersionKey, true);
		}

		#endregion
	}
}
