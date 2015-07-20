using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.Pages
{
	public partial class EntityList : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request["ClassName"] != null)
			{
				MetaClass mc = MetaDataWrapper.GetMetaClassByName(Request["ClassName"]);
				if(mc != null)
					pT.Title = CHelper.GetResFileString(mc.PluralName);
			}
		}
	}
}
