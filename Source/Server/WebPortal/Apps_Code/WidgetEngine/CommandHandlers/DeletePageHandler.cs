using System;
using System.Collections.Generic;
using System.Web;

using Mediachase.Ibn.Business.Customization;
using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.WidgetEngine.CommandHandlers
{
	public class DeletePageHandler : ICommand
	{
		#region ICommand Members
		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				if (cp.CommandArguments.ContainsKey(CustomPageEntity.FieldUid))
				{
					CustomPageManager.DeleteCustomPage(new Guid(cp.CommandArguments[CustomPageEntity.FieldUid]));
				}
			}
		}
		#endregion
	}
}
