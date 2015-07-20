using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Business.Customization;

namespace Mediachase.Ibn.Web.UI.Administration.CommandHandlers
{
	public class UndoLeftMenuItemEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				if (!cp.CommandArguments.ContainsKey("changed"))
					throw new ArgumentException("changed is null for UndoLeftMenuItemEnableHandler");

				if (!cp.CommandArguments.ContainsKey("changedLayer"))
					throw new ArgumentException("changedLayer is null for UndoLeftMenuItemEnableHandler");

				bool changed = bool.Parse(cp.CommandArguments["changed"]);
				string changedLayer = cp.CommandArguments["changedLayer"];

				HttpRequest request = HttpContext.Current.Request;

				if (String.Compare(request["ClassName"], "Principal", true) == 0 && !String.IsNullOrEmpty(request["ObjectId"]))
				{
					// user layer
					retval = changed && changedLayer.Contains(NavigationManager.CustomizationLayerUser);
				}
				else if (String.Compare(request["ClassName"], CustomizationProfileEntity.ClassName, true) == 0 && !String.IsNullOrEmpty(request["ObjectId"]))
				{
					// profile layer
					retval = changed && changedLayer.Contains(NavigationManager.CustomizationLayerProfile);
				}
				else
				{
					// global layer
					retval = changed && changedLayer.Contains(NavigationManager.CustomizationLayerGlobal);
				}
			}
			return retval;
		}
		#endregion
	}
}
