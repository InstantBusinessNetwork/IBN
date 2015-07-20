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
using Mediachase.Ibn.Core;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.Pages
{
	public partial class EntityView : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(Request["ClassName"]))
			{
				pT.Title = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(Request["ClassName"]).FriendlyName);
			}
		}
	}
}
