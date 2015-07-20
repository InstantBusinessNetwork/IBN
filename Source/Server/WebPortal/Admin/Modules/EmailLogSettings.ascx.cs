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

using Mediachase.IBN.Business.EMail;
using Mediachase.UI.Web.Util;
using System.Reflection;

namespace Mediachase.UI.Web.Admin.Modules
{
	public partial class EmailLogSettings : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
				BindLogSettings();

			UpdateLogSettingsButton.Text = LocRM.GetString("tSave");
			CancelButton.Text = LocRM.GetString("tCancel");
			UpdateLogSettingsButton.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			CancelButton.CustomImage = this.Page.ResolveUrl("~/layouts/images/cancel.gif");
		}

		#region BindLogSettings
		private void BindLogSettings()
		{
			EMailMessageLogSetting cur = EMailMessageLogSetting.Current;
			if (cur != null)
			{
				tbLogPeriod.Text = cur.Period.ToString();
			}
		}
		#endregion

		#region UpdateLogSettingsButton_ServerClick
		protected void UpdateLogSettingsButton_ServerClick(object sender, EventArgs e)
		{
			EMailMessageLogSetting cur = EMailMessageLogSetting.Current;
			try
			{
				if (int.Parse(tbLogPeriod.Text) > 0)
					cur.Period = int.Parse(tbLogPeriod.Text);
			}
			catch
			{
				cur.Period = 7;
			}
			EMailMessageLogSetting.Update(cur);
			CommonHelper.CloseIt(Response);
		}
		#endregion
	}
}