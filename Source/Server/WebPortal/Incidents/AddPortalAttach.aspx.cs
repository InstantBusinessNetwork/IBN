using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Resources;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Mediachase.UI.Web.Incidents
{
	public partial class AddPortalAttach : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strAddDoc", typeof(AddPortalAttach).Assembly);

		protected void Page_Load(object sender, EventArgs e)
		{
			pT.Title = LocRM.GetString("tAddIBNFiles");
		}

	}
}