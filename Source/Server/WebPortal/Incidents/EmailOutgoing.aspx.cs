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
using System.Resources;
using System.Reflection;

using Mediachase.IBN.Business;
using Mediachase.Ibn;

namespace Mediachase.UI.Web.Incidents
{
	public partial class EmailOutgoing : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Administrator))
				throw new AccessDeniedException();
			ApplyLocalization();
		}

		private void ApplyLocalization()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
			pT.Title = LocRM.GetString("tEmailOutgoing");
		}
	}
}
