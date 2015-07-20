using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;

using Mediachase.IBN.Business;
using Mediachase.IBN.Business.EMail;
using Mediachase.IBN.Business.Pop3;
using Mediachase.UI.Web.Util;
using System.Reflection;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Admin
{
	/// <summary>
	/// Summary description for EMailPop3BoxEdit.
	/// </summary>
	public partial class EMailPop3BoxEdit : System.Web.UI.Page
	{
		protected CheckBox cbIsInternal;
		protected HtmlTableRow trIsInternal;
		protected string Password = "";

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected string sTitle = "";

		#region BoxId
		public int BoxId
		{
			get
			{
				if (Request["BoxId"] != null)
					return int.Parse(Request["BoxId"]);
				else return -1;
			}
		}
		#endregion

		#region IsInternal
		public bool IsInternal
		{
			get
			{
				if (Request["IsInternal"] != null)
					return true;
				else return false;
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

			sTitle = (BoxId > 0) ? LocRM.GetString("tPop3BoxEdit") : LocRM.GetString("tPop3BoxNew");
			ApplyLocalization();
			if (!this.IsPostBack)
				BindData();
			if ((lbSettingsValid.Visible && lbSettingsValid.ForeColor == Color.Red) || (!lbSettingsValid.Visible && ViewState["Pop3Box_Password"] == null))
			{
				rfvPassword.Enabled = rfvConfirmPassword.Enabled = cvConfirmPassword.Enabled = true;
			}
			//else 
			//{
			//	rfvPassword.Enabled = rfvConfirmPassword.Enabled = cvConfirmPassword.Enabled = false;
			//}

			BindToolbar();

			imbSave.CustomImage = this.ResolveUrl("~/Layouts/Images/saveitem.gif");
			imbCancel.CustomImage = this.ResolveUrl("~/Layouts/Images/cancel.gif");
		}

		#region BindData
		private void BindData()
		{
			ddSmtpBoxes.DataSource = SmtpBox.List();
			ddSmtpBoxes.DataTextField = "Name";
			ddSmtpBoxes.DataValueField = "SmtpBoxId";
			ddSmtpBoxes.DataBind();
			ddSmtpBoxes.Items.Insert(0, new ListItem(LocRM.GetString("tDefaultValue"), "-1"));
			if (BoxId > 0)
			{
				EMailRouterPop3Box box = EMailRouterPop3Box.Load(BoxId);
				if (box != null)
				{
					txtName.Text = box.Name;
					tbInternalEmail.Text = box.EMailAddress;
					tbServer.Text = box.Server;
					tbPort.Text = box.Port.ToString();
					tbLogin.Text = box.Login;
					//tbPassword.Text = box.Pass;
					ViewState["Pop3Box_Password"] = box.Pass;
					ListItem li = null;
					switch (box.SecureConnectionType)
					{
						case Pop3SecureConnectionType.Ssl:
							li = rblSecureConnection.Items.FindByValue("SSL");
							if (li != null) li.Selected = true;
							break;
						case Pop3SecureConnectionType.Tls:
							li = rblSecureConnection.Items.FindByValue("TLS");
							if (li != null) li.Selected = true;
							break;
						default:
							li = rblSecureConnection.Items.FindByValue("Never");
							if (li != null) li.Selected = true;
							break;
					}
					SmtpBox smtp = OutgoingEmailServiceConfig.FindSmtpBox(OutgoingEmailServiceType.HelpDeskEmailBox, BoxId, false);
					if (smtp != null)
						CommonHelper.SafeSelect(ddSmtpBoxes, smtp.SmtpBoxId.ToString());
				}
				trConfirmPassword.Visible = false;
				rfvPassword.Enabled = false;
			}
			else
			{
				trConfirmPassword.Visible = true;
				rfvPassword.Enabled = true;
			}
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{

			imbSave.Text = LocRM.GetString("tSave");
			imbCancel.Text = LocRM.GetString("tCancel");

			ltInternalEmail.Text = LocRM.GetString("tInternalEmail");
			ltServer.Text = LocRM.GetString("tServer");
			ltPort.Text = LocRM.GetString("tPort");
			ltLogin.Text = LocRM.GetString("tLogin");
			ltPassword.Text = LocRM.GetString("tPassword");
			ltConfirmPassword.Text = LocRM.GetString("tConfirmPassword");
			btCheckSettings.Text = LocRM.GetString("tCheck");
			foreach (ListItem li in rblSecureConnection.Items)
			{
				switch (li.Value)
				{
					case "Never":
						li.Text = LocRM.GetString("tNo");
						break;
					case "SSL":
						li.Text = "SSL";
						break;
					case "TLS":
						li.Text = "TLS";
						break;
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
			this.btCheckSettings.Click += new EventHandler(btCheckSettings_Click);
		}
		#endregion

		#region Save Event
		private void imbSave_ServerClick(object sender, EventArgs e)
		{

			if (BoxId > 0)
			{
				EMailRouterPop3Box box = EMailRouterPop3Box.Load(BoxId);
				if (box != null)
				{
					box.Name = txtName.Text;
					box.Server = tbServer.Text.Trim();
					box.Port = int.Parse(tbPort.Text.Trim());
					box.Login = tbLogin.Text;
					box.Pass = tbPassword.Text != string.Empty ? tbPassword.Text : ViewState["Pop3Box_Password"].ToString();
					box.EMailAddress = tbInternalEmail.Text;
					switch (rblSecureConnection.SelectedValue)
					{
						case "Never":
							box.SecureConnectionType = Pop3SecureConnectionType.None;
							break;
						case "SSL":
							box.SecureConnectionType = Pop3SecureConnectionType.Ssl;
							break;
						case "TLS":
							box.SecureConnectionType = Pop3SecureConnectionType.Tls;
							break;
					}
					EMailRouterPop3Box.Update(box);

					if (int.Parse(ddSmtpBoxes.SelectedValue) > 0)
						OutgoingEmailServiceConfig.AssignWithSmtpBox(OutgoingEmailServiceType.HelpDeskEmailBox, BoxId, int.Parse(ddSmtpBoxes.SelectedValue));
					else
						OutgoingEmailServiceConfig.AssignWithDefaultSmtpBox(OutgoingEmailServiceType.HelpDeskEmailBox, BoxId);
				}
			}
			else
			{
				Pop3SecureConnectionType sct = Pop3SecureConnectionType.None;
				switch (rblSecureConnection.SelectedValue)
				{
					case "Never":
						sct = Pop3SecureConnectionType.None;
						break;
					case "SSL":
						sct = Pop3SecureConnectionType.Ssl;
						break;
					case "TLS":
						sct = Pop3SecureConnectionType.Tls;
						break;
				}
				if (IsInternal)
				{
					int boxId = EMailRouterPop3Box.CreateInternal(txtName.Text, tbInternalEmail.Text.Trim(), tbServer.Text.Trim(), int.Parse(tbPort.Text.Trim()), tbLogin.Text, tbPassword.Text != string.Empty ? tbPassword.Text : ViewState["Pop3Box_Password"].ToString(), sct);
					if (int.Parse(ddSmtpBoxes.SelectedValue) > 0)
						OutgoingEmailServiceConfig.AssignWithSmtpBox(OutgoingEmailServiceType.HelpDeskEmailBox, boxId, int.Parse(ddSmtpBoxes.SelectedValue));
					else
						OutgoingEmailServiceConfig.AssignWithDefaultSmtpBox(OutgoingEmailServiceType.HelpDeskEmailBox, boxId);
				}
				else
				{
					int boxId = EMailRouterPop3Box.CreateExternal(txtName.Text, tbInternalEmail.Text, tbServer.Text.Trim(), int.Parse(tbPort.Text.Trim()), tbLogin.Text, tbPassword.Text != string.Empty ? tbPassword.Text : ViewState["Pop3Box_Password"].ToString(), sct);
					if (int.Parse(ddSmtpBoxes.SelectedValue) > 0)
						OutgoingEmailServiceConfig.AssignWithSmtpBox(OutgoingEmailServiceType.HelpDeskEmailBox, boxId, int.Parse(ddSmtpBoxes.SelectedValue));
					else
						OutgoingEmailServiceConfig.AssignWithDefaultSmtpBox(OutgoingEmailServiceType.HelpDeskEmailBox, boxId);
				}
			}

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					  "try {window.opener.location.href=window.opener.location.href;}" +
					  "catch (e){} window.close();", true);
		}
		#endregion

		#region Check Settings
		private void btCheckSettings_Click(object sender, EventArgs e)
		{
			if (tbPassword.Text != string.Empty)
				ViewState["Pop3Box_Password"] = tbPassword.Text;

			Pop3SecureConnectionType sct = Pop3SecureConnectionType.None;
			switch (rblSecureConnection.SelectedValue)
			{
				case "Never":
					sct = Pop3SecureConnectionType.None;
					break;
				case "SSL":
					sct = Pop3SecureConnectionType.Ssl;
					break;
				case "TLS":
					sct = Pop3SecureConnectionType.Tls;
					break;
			}

			Pop3SettingsResult sok = EMailRouterPop3Box.CheckSettings(tbServer.Text.Trim(), int.Parse(tbPort.Text.Trim()), tbLogin.Text.Trim(),
				tbPassword.Text != string.Empty ? tbPassword.Text : ViewState["Pop3Box_Password"].ToString(),
				sct);

			lbSettingsValid.Visible = true;
			if (sok == Pop3SettingsResult.AllOk)
			{
				lbSettingsValid.ForeColor = Color.Blue;
				lbSettingsValid.Text = LocRM.GetString("tOK");
				rfvPassword.Enabled = rfvConfirmPassword.Enabled = cvConfirmPassword.Enabled = false;
			}
			else
			{
				lbSettingsValid.ForeColor = Color.Red;
				lbSettingsValid.Text = LocRM.GetString("tError");
				rfvPassword.Enabled = rfvConfirmPassword.Enabled = cvConfirmPassword.Enabled = true;
				if (sok == Pop3SettingsResult.None)
				{
					rfvServer.IsValid = rfvPort.IsValid = false;
				}
				else
					if (sok == Pop3SettingsResult.ServerName)
					{
						rfvLogin.IsValid = false;
					}
					else
						if (sok == (Pop3SettingsResult.ServerName | Pop3SettingsResult.Pop3User))
						{
							rfvPassword.IsValid = false;
						}

			}

		}
		#endregion
	}
}
