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
	public class UndoLeftMenuItemHandler : ICommand
	{
		#region ICommand Members
		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string fullId = cp.CommandArguments["primaryKeyId"];

				HttpRequest request = HttpContext.Current.Request;

				// ProfileId
				int? profileId = null;
				if (String.Compare(request["ClassName"], CustomizationProfileEntity.ClassName, true) == 0
					&& !String.IsNullOrEmpty(request["ObjectId"]))
				{
					profileId = int.Parse(request["ObjectId"]);
				}

				// UserId
				int? userId = null;
				if (String.Compare(request["ClassName"], "Principal", true) == 0
					&& !String.IsNullOrEmpty(request["ObjectId"]))
				{
					userId = int.Parse(request["ObjectId"]);
				}

				NavigationManager.UndoModifyNavigationItem(fullId, (PrimaryKeyId?)profileId, (PrimaryKeyId?)userId);

				CHelper.RequireBindGrid();
			}
		}
		#endregion
	}
}
