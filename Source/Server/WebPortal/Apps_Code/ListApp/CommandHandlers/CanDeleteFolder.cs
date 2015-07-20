using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Lists;

namespace Mediachase.Ibn.Web.UI.ListApp.CommandHandlers
{
	public class CanDeleteFolder : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			bool retval = true;
			object id = CHelper.GetFromContext("ListFolderId");
			if (id != null)
			{
				int iid = -1;
				int.TryParse(id.ToString(), out iid);
				if (iid == -1)
					retval = false;
				else if (iid > 0)
				{
					if (iid == (int)ListManager.GetPublicRoot().PrimaryKeyId.Value)
						return false;
					if (iid == (int)ListManager.GetPrivateRoot(Mediachase.IBN.Business.Security.CurrentUser.UserID).PrimaryKeyId.Value)
						return false;
					ListFolder fld = new ListFolder(iid);
					if (!fld.ParentId.HasValue && fld.FolderType == ListFolderType.Project)
						return false;
					if (!ListManager.CanDeleteFolder(iid))
						return false;
				}
			}
			return retval;
		}

		#endregion
	}
}
