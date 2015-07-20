using System;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

//using Mediachase.Alert.Service;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.EMail;
using Mediachase.Net.Mail;
using System.Reflection;


namespace Mediachase.UI.Web.Admin.Modules
{
	/// <summary>
	///		Summary description for SMTPSettings.
	/// </summary>
	public partial class SMTPSettings : System.Web.UI.UserControl
	{
		#region _boxId
		private int _boxId
		{
			get
			{
				if (Request["BoxId"] != null)
					return int.Parse(Request["BoxId"]);
				return -1;
			}
		} 
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.imbSave.ServerClick += new EventHandler(imbSave_ServerClick);
			this.lbCheck.Click += new EventHandler(lbCheck_Click);
			this.btCheckSettings.Click += new EventHandler(btCheckSettings_Click);

			ApplyLocalization();
			BindToolBar();
			if (!Page.IsPostBack)
			{
				ViewState["IsInChecking"] = false.ToString();
				BindTypes();
				if(_boxId > 0)
					BindValues();
			}
			if (_boxId > 0)
			{
				SmtpBox box = SmtpBox.Load(_boxId);
				if (box.Checked)
				{
					ViewState["IsInChecking"] = false.ToString();
					lblMessage.Text = LocRM.GetString("tSetsAreCorrect");
					lbCheck.ForeColor = Color.Green;
					lbCheck.Visible = true;
				}
				else
				{
					lblMessage.Text = LocRM.GetString("tNeedToCheck");
					lbCheck.ForeColor = Color.Red;
					lbCheck.Visible = true;
				}
			}
			else
			{
				divCheck.Visible = false;
				rbSecurity.SelectedIndex = 0;
				txtPort.Text = "25";
			}
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script type=\"text/javascript\">SetVisible('" + cbAuthenticate.ClientID + "');</script>");
		} 
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			imbSave.Text = LocRM.GetString("tSave");
			imbSave.CustomImage = ResolveUrl("~/layouts/images/saveitem.gif");
			
			btCheckSettings.Text = LocRM.GetString("tCheck");
			
			cbAuthenticate.Text = "&nbsp;" + LocRM.GetString("tAuthenticate");

			lbCheck.Text = LocRM.GetString("tCheckSMTP");

			cbIsDefault.Text = "&nbsp;" + LocRM.GetString("tDefaultBox");
 		}
		#endregion

		#region BindToolBar
		private void BindToolBar()
		{
			secHeader.Title = LocRM.GetString("tSMTPSettings");
			secHeader.AddLink("<img alt='' src='" + ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tSMTPList"), ResolveUrl("~/Admin/SMTPList.aspx"));
		}
		#endregion

		#region BindTypes
		private void BindTypes()
		{
			rbSecurity.Items.Clear();
			rbSecurity.Items.Add(new ListItem(LocRM.GetString("tUseNone"), "0"));
			rbSecurity.Items.Add(new ListItem(LocRM.GetString("tUseSsl3"), "1"));
			rbSecurity.Items.Add(new ListItem(LocRM.GetString("tUseTls"), "2"));
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			SmtpBox box = SmtpBox.Load(_boxId);
			txtTitle.Text = box.Name;
			txtServer.Text = box.Server;
			txtPort.Text = box.Port.ToString();
			cbIsDefault.Checked = box.IsDefault;

			switch (box.SecureConnection)
			{
				case SecureConnectionType.None:
					rbSecurity.SelectedValue = "0";
					break;
				case SecureConnectionType.Ssl:
					rbSecurity.SelectedValue = "1";
					break;
				case SecureConnectionType.Tls:
					rbSecurity.SelectedValue = "2";
					break;
				default:
					break;
			}

			cbAuthenticate.Checked = box.Authenticate;
			txtUser.Text = box.User;
			ViewState["Password"] = box.Password;
		}
		#endregion

		#region Save Box
		private void imbSave_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;
			SmtpBox box;
			if (_boxId > 0)
				box = SmtpBox.Load(_boxId);
			else
				box = SmtpBox.Initialize();
			box.Name = txtTitle.Text;
			box.Server = txtServer.Text;
			box.Port = int.Parse(txtPort.Text);
			switch (rbSecurity.SelectedValue)
			{
				case "0":
					box.SecureConnection = SecureConnectionType.None;
					break;
				case "1":
					box.SecureConnection = SecureConnectionType.Ssl;
					break;
				case "2":
					box.SecureConnection = SecureConnectionType.Tls;
					break;
				default:
					break;
			}

			box.Authenticate = cbAuthenticate.Checked;
			if (cbAuthenticate.Checked)
			{
				box.User = txtUser.Text;
				if (txtPassword.Text.Length > 0 || txtUser.Text.Length == 0)
					box.Password = txtPassword.Text;
			}
			//PortalConfig.SmtpSettings.IsChecked = false;
			//PortalConfig.SmtpSettings.CheckUid = Guid.NewGuid();

			if (_boxId > 0)
				SmtpBox.Update(box);
			else
				SmtpBox.Create(box);

			if (cbIsDefault.Checked)
				SmtpBox.SetDefault(box.SmtpBoxId);

			Response.Redirect("~/Admin/SMTPList.aspx", true);
		}
		#endregion

		private void lbCheck_Click(object sender, EventArgs e)
		{
			if (_boxId > 0)
			{
				string message = String.Empty;
				try
				{
					SmtpBox.SendTestEmail(_boxId, LocRM.GetString("tLetterSubj"), LocRM.GetString("tLetterBody"),
						Mediachase.Ibn.Web.UI.CHelper.GetFullPath("/Public/CheckSMTPSettings.aspx"));
				}
				catch (Exception ex)
				{
					message = ex.Message;
					message = message.Replace("\r\n", "<br/>");
				}

				lbCheck.Visible = false;
				if (String.IsNullOrEmpty(message))
				{
					lblMessage.Text = String.Format(LocRM.GetString("tGuidWasSent"), Security.CurrentUser.Email);
				}
				else
				{
					lblMessage.Text = LocRM.GetString("tGuidWasNotSent") + "<br/><br/>" + message;
				}
			}
		}

		#region Check Settings
		private void btCheckSettings_Click(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			SecureConnectionType type = SecureConnectionType.None;
			switch (rbSecurity.SelectedValue)
			{
				case "0":
					type = SecureConnectionType.None;
					break;
				case "1":
					type = SecureConnectionType.Ssl;
					break;
				case "2":
					type = SecureConnectionType.Tls;
					break;
				default:
					break;
			}

			string password = txtPassword.Text.Trim();
			if (String.IsNullOrEmpty(password) && ViewState["Password"] != null)
				password = ViewState["Password"].ToString();

			SmtpSettingsResult result = SmtpBox.CheckSettings(txtServer.Text, int.Parse(txtPort.Text), type, cbAuthenticate.Checked, txtUser.Text, password);

			lbSettingsValid.Visible = true;
			if (result == SmtpSettingsResult.AllOk)
			{
				lbSettingsValid.ForeColor = Color.Blue;
				lbSettingsValid.Text = LocRM.GetString("tOK");
			}
			else
			{
				lbSettingsValid.ForeColor = Color.Red;
				lbSettingsValid.Text = LocRM.GetString("tError");
			}

		}
		#endregion

	}
}
