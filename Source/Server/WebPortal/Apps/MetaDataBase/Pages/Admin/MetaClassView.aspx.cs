using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Pages.Admin
{
	public partial class MetaClassView : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string title = "";
			if (Request.QueryString["class"] != null)
			{
				string className = Request.QueryString["class"];
				MetaClass mc = MetaDataWrapper.GetMetaClassByName(className);

				if (mc.IsCard)
					title = "CardCustomization";
				else if (mc.IsBridge)
					title = "BridgeCustomization";
				else
					title = "InfoCustomization";
			}
			pT.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", title).ToString();

			Mediachase.IBN.Business.UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
			if (pc["TimeTrackingMode"] != null && pc["TimeTrackingMode"] == "dev")
				pT.SetControlProperties("ShowSystemInfo", true);
			else
				pT.SetControlProperties("ShowSystemInfo", false);
		}
	}
}
