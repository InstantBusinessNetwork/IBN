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
	public class DeleteItemHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string sid = cp.CommandArguments["primaryKeyId"];

				string[] elem = sid.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
				//type, id, _containerName, _containerKey
				if (elem.Length != 4)
					return;

				int id = -1;
				int.TryParse(elem[0], out id);

				FileStorage fs = FileStorage.Create(elem[2], elem[3]);
				if (id == 1)
				{
					DirectoryInfo di = fs.GetDirectory(int.Parse(elem[1]));
					fs.DeleteFolder(di);
				}
				if (id == 2)
				{
					FileInfo fi = fs.GetFile(int.Parse(elem[1]));
					fs.DeleteFile(fi);
				}

				CHelper.RequireBindGrid();
			}
		}

		#endregion
	}
}
