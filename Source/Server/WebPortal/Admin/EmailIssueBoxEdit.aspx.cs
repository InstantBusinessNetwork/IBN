using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Resources;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business.EMail;
using System.Reflection;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Admin
{
	/// <summary>
	/// Summary description for EmailIssueBoxEdit.
	/// </summary>
	public partial class EmailIssueBoxEdit : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected string sTitle = "";

		#region IssBoxId
		public int IssBoxId
		{
			get
			{
				if (Request["IssBoxId"] != null)
					return int.Parse(Request["IssBoxId"]);
				else return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ibn.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/dialog.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			sTitle = (IssBoxId > 0) ? LocRM.GetString("tIssueBoxEdit") : LocRM.GetString("tIssueBoxNew");
			ApplyLocalization();
			lblDuplicate.Visible = false;
			if (!this.IsPostBack)
				BindData();
			BindToolbar();

			imbSave.CustomImage = this.ResolveUrl("~/Layouts/Images/saveitem.gif");
			imbCancel.CustomImage = this.ResolveUrl("~/Layouts/Images/cancel.gif");
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			lbIsDefault.Text = LocRM.GetString("tDefaultBox");
			lbName.Text = LocRM.GetString("tName");
			lbMask.Text = LocRM.GetString("tIdentMask");
			imbSave.Text = LocRM.GetString("tSave");
			imbCancel.Text = LocRM.GetString("tCancel");
		}
		#endregion

		#region BindData
		private void BindData()
		{
			if (IssBoxId > 0)
			{
				IncidentBox ib = IncidentBox.Load(IssBoxId);
				if (ib != null)
				{
					cbIsDefault.Checked = ib.IsDefault;
					tbName.Text = ib.Name;
					tbMask.Text = ib.IdentifierMask;
				}
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = sTitle;
			secHeader.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tClose"), "javascript:window.close();");
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.imbSave.ServerClick += new EventHandler(imbSave_ServerClick);
		}
		#endregion

		private void imbSave_ServerClick(object sender, EventArgs e)
		{
			int iIssBoxId = -1;
			try
			{
				if (IssBoxId > 0)
				{
					IncidentBox ib = IncidentBox.Load(IssBoxId);
					if (ib != null)
					{
						ib.IsDefault = cbIsDefault.Checked;
						ib.Name = tbName.Text.Trim();
						ib.IdentifierMask = tbMask.Text.Trim();
						IncidentBox.Update(ib);
					}
				}
				else
				{
					iIssBoxId = IncidentBox.Create(tbName.Text.Trim(), tbMask.Text.Trim(), cbIsDefault.Checked);
				}
				if (iIssBoxId > 0)
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
						"try {window.opener.location.href='" + ResolveUrl("~/Admin/EMailIssueBoxView.aspx") + "?IssBoxId=" + iIssBoxId + "';}" +
						"catch (e){} window.close();", true);
				else
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
						"try {window.opener.location.href=window.opener.location.href;}" +
						"catch (e){} window.close();", true);
			}
			catch (IncidentBoxDuplicateNameException)
			{
				lblDuplicate.Text = LocRM.GetString("tDuplicateName");
				lblDuplicate.Visible = true;
			}
			catch (IncidentBoxDuplicateIdentifierMaskException)
			{
				lblDuplicate.Text = LocRM.GetString("tDuplicateMask");
				lblDuplicate.Visible = true;
			}

		}
	}
}
