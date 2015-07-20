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

namespace Mediachase.Ibn.Web.UI.FileLibrary.CommandHandlers
{
	public class DeleteItemEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			if (Mediachase.IBN.Business.Security.CurrentUser.IsExternal)
				return false;

			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string sid = cp.CommandArguments["primaryKeyId"];

				string[] elem = sid.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
				//type, id, _containerName, _containerKey
				if (elem.Length != 4)
					return false;

				int id = -1;
				int.TryParse(elem[0], out id);

				FileStorage fs = FileStorage.Create(elem[2], elem[3]);
				if (id == 1)
				{
					DirectoryInfo di = fs.GetDirectory(int.Parse(elem[1]));
					if (fs.CanUserWrite(di.Parent))
						return true;
				}
				if (id == 2)
				{
					FileInfo fi = fs.GetFile(int.Parse(elem[1]));
					if (fs.CanUserWrite(fi.ParentDirectory))
						return true;
				}
			}
			return false;
		}

		#endregion
	}
}
