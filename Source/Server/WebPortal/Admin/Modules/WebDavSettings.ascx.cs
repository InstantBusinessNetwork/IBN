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
using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Admin.Modules
{
	public partial class WebDavSettings : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, EventArgs e)
		{
			btnSave.CustomImage = ResolveUrl("~/layouts/Images/SaveItem.gif");
			btnCancel.CustomImage = ResolveUrl("~/layouts/Images/cancel.gif");
			btnCancel.IsDecline = true;
			btnSave.Text = LocRM2.GetString("Save");
			btnCancel.Text = LocRM2.GetString("Cancel");
			btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
			btnCancel.ServerClick += new EventHandler(btnCancel_ServerClick);

			BindToolbar();

			if (!Page.IsPostBack)
			{
				txtLifeTime.Text = PortalConfig.WebDavSessionLifeTime;
			}
		}

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tWebDavSettings");
		}
		#endregion

		void btnCancel_ServerClick(object sender, EventArgs e)
		{
			Response.Redirect("~/Admin/SearchSettings.aspx");
		}

		void btnSave_ServerClick(object sender, EventArgs e)
		{
			string sValue = "60";
			if (!String.IsNullOrEmpty(txtLifeTime.Text))
				sValue = txtLifeTime.Text;
			PortalConfig.WebDavSessionLifeTime = sValue;
			Response.Redirect("~/Admin/SearchSettings.aspx");
		}
	}
}