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
using Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules;

namespace Mediachase.Ibn.Web.UI.HelpDeskManagement.CommandHandlers
{
	public class NewViewCreatedHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string uid = cp.CommandArguments["ViewUid"];
				UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
				pc[IncidentListNew.IssueListViewNameKey] = uid;
				((System.Web.UI.Control)(Sender)).Page.Response.Redirect(((System.Web.UI.Control)(Sender)).Page.Request.RawUrl);
			}
			
		}

		#endregion
	}
}
