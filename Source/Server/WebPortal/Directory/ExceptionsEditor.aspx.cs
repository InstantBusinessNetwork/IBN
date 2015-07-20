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

namespace Mediachase.UI.Web.Directory
{
	public partial class ExceptionsEditor : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ApplyLocalization();
		}

		private void ApplyLocalization()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strPageTitles", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
			if (Request["ExceptionId"] != null)
				pT.Title = LocRM.GetString("tExceptionsEditor");
			else
				pT.Title = LocRM.GetString("tNewException");
		}
	}
}
