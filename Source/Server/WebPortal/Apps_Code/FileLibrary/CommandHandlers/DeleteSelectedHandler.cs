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
using System.Globalization;
using Mediachase.IBN.Business.ControlSystem;

namespace Mediachase.Ibn.Web.UI.FileLibrary.CommandHandlers
{
	public class DeleteSelectedHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				string[] checkedElems = MCGrid.GetCheckedCollection(((CommandManager)Sender).Page, cp.CommandArguments["GridId"]);

				foreach (string elem in checkedElems)
				{
					string[] elemMas = elem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
					//type, id, _containerName, _containerKey
					if (elemMas.Length != 4)
						continue;

					int id = Convert.ToInt32(elemMas[1], CultureInfo.InvariantCulture);
					FileStorage fs = FileStorage.Create(elemMas[2], elemMas[3]);
					if (elemMas[0] == "1")
					{
						fs.DeleteFolder(id);
					}
					else if (elemMas[0] == "2")
					{
						fs.DeleteFile(id);
					}
				}

				CHelper.RequireBindGrid();
			}
		}

		#endregion
	}
}
