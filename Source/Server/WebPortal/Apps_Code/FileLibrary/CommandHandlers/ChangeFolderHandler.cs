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
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.FileLibrary.CommandHandlers
{
	public class ChangeFolderHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string containerKey = cp.CommandArguments["ContainerKey"];
				string folderId = cp.CommandArguments["FolderId"];
				UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
				_pc["fs_FolderId_" + containerKey] = folderId;
				
				CHelper.RequireBindGrid();
			}
		}

		#endregion
	}
}
