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
	public class ShowSelectedItemsHandler: ICommand
	{
		#region ICommand Members
		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string[] selectedItems = MCGrid.GetCheckedCollection(((CommandManager)Sender).Page, cp.CommandArguments["GridId"]);

				HttpRequest request = HttpContext.Current.Request;

				int? profileId = null;
				if (String.Compare(request["ClassName"], CustomizationProfileEntity.ClassName, true) == 0 && !String.IsNullOrEmpty(request["ObjectId"]))
					profileId = int.Parse(request["ObjectId"]);

				int? principalId = null;
				if (String.Compare(request["ClassName"], "Principal", true) == 0 && !String.IsNullOrEmpty(request["ObjectId"]))
					principalId = int.Parse(request["ObjectId"]);

				using (DbTransaction tran = DbTransaction.Begin())
				{
					foreach (string fullId in selectedItems)
					{
						NavigationManager.ShowCustomizationItem(fullId, CustomizationStructureType.NavigationMenu, profileId, principalId);
					}
					tran.Commit();
				}

				if (selectedItems.Length > 0)
					CHelper.RequireBindGrid();
			}
		}
		#endregion
	}
}
