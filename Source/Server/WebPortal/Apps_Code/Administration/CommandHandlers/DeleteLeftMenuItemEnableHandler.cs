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
	public class DeleteLeftMenuItemEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				if (!cp.CommandArguments.ContainsKey("added"))
					throw new ArgumentException("added is null for DeleteLeftMenuItemEnableHandler");

				if (!cp.CommandArguments.ContainsKey("addedLayer"))
					throw new ArgumentException("addedLayer is null for UpdateLeftMenuItemEnableHandler");

				bool added = bool.Parse(cp.CommandArguments["added"]);
				string addedLayer = cp.CommandArguments["addedLayer"];

				HttpRequest request = HttpContext.Current.Request;

				if (String.Compare(request["ClassName"], "Principal", true) == 0 && !String.IsNullOrEmpty(request["ObjectId"]))
				{
					// user layer
					retval = added && addedLayer.Contains(NavigationManager.CustomizationLayerUser);
				}
				else if (String.Compare(request["ClassName"], CustomizationProfileEntity.ClassName, true) == 0 && !String.IsNullOrEmpty(request["ObjectId"]))
				{
					// profile layer
					retval = added && addedLayer.Contains(NavigationManager.CustomizationLayerProfile);
				}
				else
				{
					// global layer
					retval = added && addedLayer.Contains(NavigationManager.CustomizationLayerGlobal);
				}
			}
			return retval;
		}
		#endregion
	}
}
