using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.Administration.Modules
{
	public partial class FieldsVisibility : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				BindSavedValues();
				BindToolbar();
			}
			ApllyLocalization();
		}

		private void ApllyLocalization()
		{
			allowClient.Text = " " + GetGlobalResourceObject("IbnFramework.Admin", "generalAllowClient").ToString();
			allowGenCats.Text = " " + GetGlobalResourceObject("IbnFramework.Admin", "generalAllowGeneralCategories").ToString();
			allowPriority.Text = " " + GetGlobalResourceObject("IbnFramework.Admin", "generalAllowPriority").ToString();
			allowTaskTime.Text = " " + GetGlobalResourceObject("IbnFramework.Admin", "generalAllowTaskTime").ToString();
			btnSave.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Save").ToString();
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			btnCancel.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Cancel").ToString();
			lblGoToObjectSettings.Text = String.Format("<a href='{1}'><span style='color:red;'>{0}</span></a>", 
				GetGlobalResourceObject("IbnFramework.Admin", "GoToObjectsSettings").ToString(),
				this.Page.ResolveClientUrl("~/Apps/Administration/Pages/ObjectFieldsVisibility.aspx"));
		}

		private void BindSavedValues()
		{
			allowClient.Checked = PortalConfig.GeneralAllowClientField;
			allowGenCats.Checked = PortalConfig.GeneralAllowGeneralCategoriesField;
			allowPriority.Checked = PortalConfig.GeneralAllowPriorityField;
			allowTaskTime.Checked = PortalConfig.GeneralAllowTaskTimeField;
		}

		private void BindToolbar()
		{
			BlockHeaderMain.Title = GetGlobalResourceObject("IbnFramework.Admin", "GeneralFieldsVisibility").ToString();
			BlockHeaderMain.AddLink(String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle' /> {1}",
					this.Page.ResolveClientUrl("~/Layouts/Images/cancel.gif"), GetGlobalResourceObject("IbnShell.Navigation", "tInfoAppearance").ToString()),
				this.Page.ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin1"));
		}

		protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin1");
		}

		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			if (PortalConfig.GeneralAllowClientField != allowClient.Checked)
				PortalConfig.GeneralAllowClientField = allowClient.Checked;
			if (PortalConfig.GeneralAllowGeneralCategoriesField != allowGenCats.Checked)
				PortalConfig.GeneralAllowGeneralCategoriesField = allowGenCats.Checked;
			if (PortalConfig.GeneralAllowPriorityField != allowPriority.Checked)
				PortalConfig.GeneralAllowPriorityField = allowPriority.Checked;
			if (PortalConfig.GeneralAllowTaskTimeField != allowTaskTime.Checked)
				PortalConfig.GeneralAllowTaskTimeField = allowTaskTime.Checked;

			Response.Redirect("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin1");
		}
	}
}