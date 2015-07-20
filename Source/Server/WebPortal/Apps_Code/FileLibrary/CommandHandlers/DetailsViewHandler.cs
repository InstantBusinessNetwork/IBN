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
	public class DetailsViewHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
			_pc["fs_ViewStyle"] = "DetailsView";
			((System.Web.UI.Control)(Sender)).Page.Response.Redirect(((System.Web.UI.Control)(Sender)).Page.Request.RawUrl);
		}

		#endregion
	}
}
