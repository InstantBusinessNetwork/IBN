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

namespace Mediachase.Ibn.Web.UI.ProjectManagement.CommandHandlers
{
	public class RedirectHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			((System.Web.UI.Control)(Sender)).Page.Response.Redirect(((System.Web.UI.Control)(Sender)).Page.Request.RawUrl, true);
		}

		#endregion
	}
}
