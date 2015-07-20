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

namespace Mediachase.UI.Web.Common
{
	public partial class SelectIncident : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ApplyLocalization();
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strPageTitles", Assembly.GetExecutingAssembly());
			pT.Title = LocRM.GetString("SelectIssue");
		}
		#endregion

		public Mediachase.UI.Web.Modules.DialogTemplate pT
		{
			get { return Migrated_pT; }
			//set { Migrated_pT = value; }
		}

	}
}