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

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.CommandHandlers
{
	public class EntityListExport : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string type = cp.CommandArguments["Type"];
				string variant = cp.CommandArguments["Variant"];
				string className = cp.CommandArguments["ClassName"];
				string ids = String.Empty;
				if (String.Compare(variant, "3", true) == 0)	//SelectedItems
				{
					string[] selectedElements = EntityGrid.GetCheckedCollection(((CommandManager)Sender).Page, cp.CommandArguments["GridId"]);
					ids = String.Join(";", selectedElements);
				}
				((System.Web.UI.Control)(Sender)).Page.Response.Redirect(((System.Web.UI.Control)(Sender)).Page.ResolveUrl("~/Apps/MetaUIEntity/Pages/EntityList.aspx") + "?ClassName=" + className + "&Export=" + type + "&variant=" + variant + "&ids=" + ids);
			}
		}

		#endregion
	}
}
