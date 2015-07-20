using System;
using System.Collections.Generic;
using System.Web;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.ListApp.CommandHandlers
{
	public class CanReadList : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				if (cp.CommandArguments != null && cp.CommandArguments.ContainsKey(CommandParameters.CommandParametersKey))
				{
					string param = cp.CommandArguments[CommandParameters.CommandParametersKey];

					int id = -1;
					int.TryParse(param, out id);

					if (id > 0)
						retval = ListInfoBus.CanRead(id);
				}
			}
			return retval;
		}

		#endregion
	}
}
