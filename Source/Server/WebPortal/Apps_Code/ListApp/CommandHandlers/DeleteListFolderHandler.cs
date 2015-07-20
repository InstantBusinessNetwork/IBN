using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Lists;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.ListApp.CommandHandlers
{
	public class DeleteListFolderHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				object id = CHelper.GetFromContext("ListFolderId");
				if (id != null)
				{
					int iid = -1;
					int.TryParse(id.ToString(), out iid);

					ListFolder folder = new ListFolder(iid);
					UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
					if(folder.ParentId.HasValue)
						_pc["ListInfo_FolderId_EntityList_GroupItemKey"] = folder.ParentId.Value.ToString();

					ListManager.DeleteFolder(iid);

					((Control)Sender).Page.Response.Redirect("~/Apps/ListApp/Pages/ListAppList.aspx");
				}
			}
		}

		#endregion
	}
}
