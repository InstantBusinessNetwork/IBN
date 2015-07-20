using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules
{
	public partial class _default : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Mediachase.IBN.Business.Configuration.TimeTrackingCustomization)
				throw new LicenseRestrictionException();
		}
	}
}