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

namespace Mediachase.Ibn.Web.UI.Shell.CommandHandlers
{
	public class TestPortal : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;

			if (Mediachase.IBN.Business.Configuration.Domain.ToLower().IndexOf("localhost")>=0 ||
				String.Compare(Mediachase.IBN.Business.Configuration.Domain, "ibn47.mediachase.net", true) == 0 ||
				String.Compare(Mediachase.IBN.Business.Configuration.Domain, "ibn.mediachase.ru", true) == 0)
			{
				retval = true;
			}

			return retval;
		}

		#endregion
	}
}
