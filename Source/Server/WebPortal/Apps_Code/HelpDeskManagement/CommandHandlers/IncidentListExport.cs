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

namespace Mediachase.Ibn.Web.UI.HelpDeskManagement.CommandHandlers
{
	public class IncidentListExport : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string type = cp.CommandArguments["Type"];
				string variant = cp.CommandArguments["Variant"];
				((System.Web.UI.Control)(Sender)).Page.Response.Redirect("~/Apps/HelpDeskManagement/Pages/IncidentListNew.aspx?Export=" + type + "&variant=" + variant);
			}
		}

		#endregion
	}
}
