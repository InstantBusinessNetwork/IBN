using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Web.UI.MetaUIEntity.Modules;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.CommandHandlers
{
	public class ProfileEditedHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string uid = cp.CommandArguments["ViewUid"];
				string className = cp.CommandArguments["ClassName"];
				UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
				EntityList.SetProfileName(pc, className, uid);
				((System.Web.UI.Control)(Sender)).Page.Response.Redirect(((System.Web.UI.Control)(Sender)).Page.Request.RawUrl);
			}
		}

		#endregion
	}
}
