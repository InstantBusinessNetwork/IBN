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
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.FileLibrary.CommandHandlers
{
	public class RightAdminHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			if (Mediachase.IBN.Business.Security.CurrentUser.IsExternal)
				return false;

			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string containerKey = CHelper.GetFromContext(Mediachase.Ibn.Web.UI.FileLibrary.Modules.FileStorage._containerKeyKey).ToString();
				string containerName = CHelper.GetFromContext(Mediachase.Ibn.Web.UI.FileLibrary.Modules.FileStorage._containerNameKey).ToString();

				UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
				int folderId = int.Parse(_pc["fs_FolderId_" + containerKey]);
				FileStorage fs = FileStorage.Create(containerName, containerKey);
				if (fs.CanUserAdmin(folderId))
					return true;
			}
			return false;
		}

		#endregion
	}
}
