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

namespace Mediachase.Ibn.Web.UI.TimeTracking.Pages.Public
{
	public partial class ListTimeTrackingNew : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			pT.Title = CHelper.GetResFileString("{IbnFramework.Global:_mc_TimeManagement}");
			LoadScripts();
		}

		#region LoadScripts
		void LoadScripts()
		{
			//ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("~/Scripts/IbnFramework/ext-base.js"));
			//ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("~/Scripts/IbnFramework/ext-all.js"));
			ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("~/Scripts/IbnFramework/CSJSRequestObject.js"));
			//ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("~/Scripts/IbnFramework/ToolbarCA.js"));
			ScriptManager.GetCurrent(this.Page).ScriptMode = ScriptMode.Release;

			ScriptManager.GetCurrent(this.Page).EnablePageMethods = true;
		}
		#endregion
	}
}
