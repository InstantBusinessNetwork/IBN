using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Business.Customization;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Database;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Administration.CommandHandlers
{
	public class ShowItemHandler : ICommand
	{
		#region ICommand Members
		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string fullId = cp.CommandArguments["primaryKeyId"];

				PrimaryKeyId profileId = PrimaryKeyId.Parse(HttpContext.Current.Request["ObjectId"]);

				NavigationManager.ShowCustomizationItem(fullId, CustomizationStructureType.NavigationMenu, profileId);

				CHelper.RequireBindGrid();
			}
		}
		#endregion
	}
}
