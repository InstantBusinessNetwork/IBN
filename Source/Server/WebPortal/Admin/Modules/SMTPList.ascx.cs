using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using System.Reflection;
using Mediachase.IBN.Business.EMail;

namespace Mediachase.UI.Web.Admin.Modules
{
	public partial class SMTPList : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, EventArgs e)
		{
			this.dgSets.DeleteCommand += new DataGridCommandEventHandler(dg_DeleteCommand);
			if (!Page.IsPostBack)
				BindDG();
			BindToolbar();
		}

		#region BindDG
		private void BindDG()
		{
			dgSets.Columns[1].HeaderText = LocRM.GetString("tTitle");
			dgSets.Columns[2].HeaderText = LocRM.GetString("tServer");
			dgSets.Columns[3].HeaderText = LocRM.GetString("tPort");
			dgSets.Columns[4].HeaderText = LocRM.GetString("tUser");
			dgSets.Columns[5].HeaderText = LocRM.GetString("tDefaultBox");

			dgSets.DataSource = SmtpBox.List();
			dgSets.DataBind();

			foreach (DataGridItem dgi in dgSets.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarningSmtpBox") + "')");
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tSMTPList");

			secHeader.AddLink(String.Format("<img alt='' src='{1}'/>&nbsp;{0}",
					LocRM.GetString("tSmtpListSettings"),
					Page.ResolveUrl("~/Layouts/Images/customize.gif")),
					String.Format("javascript:ShowWizard('{0}',280,100)", Page.ResolveUrl("~/Admin/SmtpListSettings.aspx")));

			secHeader.AddLink("<img alt='' src='" + Page.ResolveUrl("~/Layouts/Images/newitem.gif") + "'/> " + LocRM.GetString("tSmtpBoxNew"), ResolveUrl("~/Admin/SMTPSettings.aspx"));
			secHeader.AddLink("<img alt='' src='" + Page.ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tCommonSettings"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin3"));
		}
		#endregion

		#region Delete
		private void dg_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int sid = int.Parse(e.Item.Cells[0].Text);
			SmtpBox.Delete(sid);
			Response.Redirect("~/Admin/SMTPList.aspx", true);
		}
		#endregion

	}
}