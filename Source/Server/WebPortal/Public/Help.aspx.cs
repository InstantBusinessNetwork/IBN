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

namespace Mediachase.UI.Web.Public
{
	public partial class help : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ApplyLocalization();
		}
		private void ApplyLocalization()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Public.Resources.strContactUs", typeof(help).Assembly);
			pT.Title = LocRM.GetString("HelpSupport");
		}
	}
}
