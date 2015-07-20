using System;
using System.Collections.Generic;
using System.Web;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.CommandHandlers
{
	public class NeedToBindGridHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			CHelper.RequireBindGrid();
		}

		#endregion
	}
}
