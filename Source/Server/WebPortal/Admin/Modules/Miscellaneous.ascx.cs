using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn;

namespace Mediachase.UI.Web.Admin.Modules
{
	public partial class Miscellaneous : System.Web.UI.UserControl
	{
		private UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		public string keyOperationMode = "TimeTrackingMode";
		public string devMode = "dev";

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				BindDataAudit();
				SaveButton.Attributes.Add("onclick", "DisableButtons(this);");
			}

			lblDisableRss.Click += new EventHandler(lblDisableRss_Click);
			lblEnableRss.Click += new EventHandler(lblEnableRss_Click);
			RssHeader.Title = CHelper.GetResFileString("{IbnFramework.Admin:RssHeader}");

			SavedLabel.Visible = false;
			BindSecHeader();
			SaveButton.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			AuditIbnClientLogin.Text = String.Format(GetGlobalResourceObject("IbnFramework.Admin", "AuditIbnClientLogin").ToString(), IbnConst.ProductFamilyShort);
		}

		#region lblEnableRss_Click
		/// <summary>
		/// Handles the Click event of the lblEnableRss control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void lblEnableRss_Click(object sender, EventArgs e)
		{
			PortalConfig.IsListRssEnabled = true;
		} 
		#endregion

		#region lblDisableRss_Click
		/// <summary>
		/// Handles the Click event of the lblDisableRss control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void lblDisableRss_Click(object sender, EventArgs e)
		{
			PortalConfig.IsListRssEnabled = false;
		} 
		#endregion

		#region BindSecHeader
		private void BindSecHeader()
		{
			secHeader.Title = LocRM.GetString("Miscellaneous");
			secHeader.AddLink(
				CHelper.GetIconText(LocRM.GetString("tCommonSettings"), ResolveClientUrl("~/Layouts/Images/cancel.gif")),
				"~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin3");
		}
		#endregion

		#region BindDataAudit
		private void BindDataAudit()
		{
			AuditWebLogin.Checked = PortalConfig.AuditWebLogin;

			AuditIbnClientLogin.Checked = PortalConfig.AuditIbnClientLogin;
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			bool adminMode = true;
			if (pc[keyOperationMode] != null && pc[keyOperationMode] == devMode)
				adminMode = false;

			if (adminMode)
			{
				DevModeRow.Visible = false;
				AdminModeRow.Visible = true;
			}
			else
			{
				DevModeRow.Visible = true;
				AdminModeRow.Visible = false;
			}

			if (PortalConfig.IsListRssEnabled)
			{
				RssEnableRow.Visible = false;
				RssDisableRow.Visible = true;
			}
			else
			{
				RssEnableRow.Visible = true;
				RssDisableRow.Visible = false;
			}

			if (PortalConfig.ProjectViewControl.ToLowerInvariant() == "~/Apps/ProjectManagement/Modules/ViewProject.ascx".ToLowerInvariant())
			{
				ProjectWithTabsRow.Visible = false;
				ProjectWithLeftMenuRow.Visible = true;
			}
			else
			{
				ProjectWithTabsRow.Visible = true;
				ProjectWithLeftMenuRow.Visible = false;
			}
		} 
		#endregion

		#region ButtonSwitchToAdmin_Click
		protected void ButtonSwitchToAdmin_Click(object sender, EventArgs e)
		{
			pc.Remove(keyOperationMode);
		}
		#endregion

		#region ButtonSwitchToDev_Click
		protected void ButtonSwitchToDev_Click(object sender, EventArgs e)
		{
			pc[keyOperationMode] = devMode;
		}
		#endregion

		#region SaveButton_ServerClick
		protected void SaveButton_ServerClick(object sender, System.EventArgs e)
		{
			PortalConfig.AuditWebLogin = AuditWebLogin.Checked;

			PortalConfig.AuditIbnClientLogin = AuditIbnClientLogin.Checked;

			SavedLabel.Visible = true;
		}
		#endregion

		#region SwitchToProjectWithLeftMenuButton_Click
		protected void SwitchToProjectWithLeftMenuButton_Click(object sender, EventArgs e)
		{
			PortalConfig.ProjectViewControl = "~/Apps/ProjectManagement/Modules/ViewProject.ascx";
		} 
		#endregion

		#region SwitchToProjectWithTabsButton_Click
		protected void SwitchToProjectWithTabsButton_Click(object sender, EventArgs e)
		{
			PortalConfig.ProjectViewControl = string.Empty;
		} 
		#endregion
	}
}