using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Business.Directory;

namespace Mediachase.Ibn.Web.UI.IbnDirectory.CommandHandlers
{
	public class MoveOrgUnitHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				PrimaryKeyId targetId = PrimaryKeyId.Parse(cp.CommandArguments["TreeServiceTargetObjectId"]);
				PrimaryKeyId newId = PrimaryKeyId.Parse(cp.CommandArguments["SelectedValue"].Split(new string[] {"::"}, StringSplitOptions.RemoveEmptyEntries)[0]);
				if(targetId != PrimaryKeyId.Empty && newId != PrimaryKeyId.Empty && newId != targetId)
				{
					DirectoryOrganizationalUnitEntity obj = (DirectoryOrganizationalUnitEntity)BusinessManager.Load(DirectoryOrganizationalUnitEntity.ClassName, targetId);
					MoveTreeNodeRequest req = new MoveTreeNodeRequest(obj);
					req.NewParent = newId;
					BusinessManager.Execute(req);
				}
				CHelper.RequireBindGrid();
			}
		}

		#endregion
	}
}
