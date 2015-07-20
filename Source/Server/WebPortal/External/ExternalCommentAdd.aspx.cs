using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;
using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.External
{
	/// <summary>
	/// Summary description for CommentAdd1.
	/// </summary>
	/// 
	public partial class ExternalCommentAdd : System.Web.UI.Page
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			if (!Security.CurrentUser.IsExternal)
				Response.Redirect("../logoff.aspx");
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strPageTitles", typeof(ExternalCommentAdd).Assembly);
			pT.Title = LocRM.GetString("tCommentAdd");
		}
		#endregion
	
	}
}
