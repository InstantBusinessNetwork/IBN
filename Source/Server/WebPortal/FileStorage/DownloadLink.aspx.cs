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
using System.Reflection;
using System.Resources;

namespace Mediachase.UI.Web.FileStorage
{
	public partial class DownloadLink : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strFileLibrary", Assembly.GetExecutingAssembly());
			pT.Title = LocRM.GetString("tDownloadLinkTitle");
		}
	}
}
