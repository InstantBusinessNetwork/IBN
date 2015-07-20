using System;
using System.Resources;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Configuration;
using Mediachase.Net.Mail;

namespace Mediachase.Ibn.WebAsp.Modules
{
	public partial class SettingsEdit : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Label lblParentDomain;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebAsp.App_GlobalResources.Resources.Sites", typeof(SettingsEdit).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbar();

			if (!IsPostBack)
			{
				ApplyLocalization();
				BindInfo();
			}
		}

		#region BindToolbar
		private void BindToolbar()
		{
			secH.Title = LocRM.GetString("tbTitle3");
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			lgdSets.InnerText = LocRM.GetString("tTrialSets");
			lblMaxHDD.Text = LocRM.GetString("MaxHDD");
			lblPeriod.Text = LocRM.GetString("Period");
			lblAllowedUsers.Text = LocRM.GetString("AllowedInternalUsers");
			lblAllowedExternalUsers.Text = LocRM.GetString("AllowedExternalUsers");

			legendDns.InnerText = LocRM.GetString("DnsSettings");
			lblDnsParentDomain.Text = LocRM.GetString("DnsParentDomain");

			legendIis.InnerText = LocRM.GetString("IisSettings");
			lblIisIpAddress.Text = LocRM.GetString("IisIpAddress");
			lblIisPort.Text = LocRM.GetString("IisPort");

			lgdNotif.InnerText = LocRM.GetString("NotifSets");
			lblEMailFrom.Text = LocRM.GetString("EMailFrom");
			lblOperatorEmail.Text = LocRM.GetString("OperatorEmail");

			lgdGeneral.InnerText = LocRM.GetString("GeneralSettings");
			chkAutoDeactivateExpired.Text = LocRM.GetString("AutoDeactivateExpired");
			chkAutoDeleteOutdated.Text = LocRM.GetString("AutoDeleteOutdated");
			lblOutdatePeriod.Text = LocRM.GetString("OutdatePeriod");

			chkSendSpam.Text = LocRM.GetString("SendSpam");

			chkSendSpamOneDayAfter.Text = LocRM.GetString("SendSpamOneDayAfter");
			lblOneDayAfterPeriod.Text = LocRM.GetString("OneDayAfterPeriod");

			chkSendSpamOneWeekAfter.Text = LocRM.GetString("SendSpamOneWeekAfter");
			lblOneWeekAfterPeriod.Text = LocRM.GetString("OneWeekAfterPeriod");

			chkSendSpamOneWeekBefore.Text = LocRM.GetString("SendSpamOneWeekBefore");
			lblOneWeekBeforePeriod.Text = LocRM.GetString("OneWeekBeforePeriod");

			chkSendSpamOneDayBefore.Text = LocRM.GetString("SendSpamOneDayBefore");
			lblOneDayBeforePeriod.Text = LocRM.GetString("OneDayBeforePeriod");

			lgdTariff.InnerText = LocRM.GetString("TariffSettings");
			UseTariffsCheckBox.Text = LocRM.GetString("UseTariffs");
			AutoDeactivateUnpaidCheckBox.Text = LocRM.GetString("AutoDeactivateUnpaid");

			SendBillableSpamCheckBox.Text = LocRM.GetString("SendBillableSpam");
			SendBillableSpam7dayCheckBox.Text = LocRM.GetString("SendBillableSpam7day");
			SendBillableSpam3dayCheckBox.Text = LocRM.GetString("SendBillableSpam3day");
			SendBillableSpam1dayCheckBox.Text = LocRM.GetString("SendBillableSpam1day");
			SendBillableSpamNegativeBalanceCheckBox.Text = LocRM.GetString("SendBillableSpamNegativeBalance");

			SmtpSettingsLegend.InnerText = LocRM.GetString("SmtpSettings");
			SmtpAuthenticateValue.Text = LocRM.GetString("SmtpAuthenticate");

			btnSave.Text = LocRM.GetString("Save");
		}
		#endregion

		#region BindInfo
		private void BindInfo()
		{
			ListItem li;

			SmtpSecureConnectionList.Items.Add(new ListItem(SecureConnectionType.None.ToString(), ((int)SecureConnectionType.None).ToString()));
			SmtpSecureConnectionList.Items.Add(new ListItem(SecureConnectionType.Ssl.ToString(), ((int)SecureConnectionType.Ssl).ToString()));
			SmtpSecureConnectionList.Items.Add(new ListItem(SecureConnectionType.Tls.ToString(), ((int)SecureConnectionType.Tls).ToString()));

			DefaultTrialPoolList.Items.Add(new ListItem(LocRM.GetString("NewPool"), ""));
			DefaultBillablePoolList.Items.Add(new ListItem(LocRM.GetString("NewPool"), ""));
			foreach (string poolName in Configurator.Create().ListApplicationPools())
			{
				DefaultTrialPoolList.Items.Add(poolName);
				DefaultBillablePoolList.Items.Add(poolName);
			}

			// DefaultTariff
			DefaultTariff.DataTextField = "tariffName";
			DefaultTariff.DataValueField = "tariffId";
			DefaultTariff.DataSource = Tariff.GetTariff(0, 0);
			DefaultTariff.DataBind();
			//

			AspSettings settings = AspSettings.Load();

			txtDnsParentDomain.Text = settings.DnsParentDomain;

			txtIisIpAddress.Text = settings.IisIpAddress;
			txtIisPort.Text = settings.IisPort.ToString();

			txtMaxHDD.Text = settings.MaxHDD.ToString();
			txtPeriod.Text = settings.TrialPeriod.ToString();
			txtMaxUsers.Text = settings.MaxUsers.ToString();
			txtEMailFrom.Text = settings.EmailFrom;
			txtOperatorEmail.Text = settings.OperatorEmail;
			tbExternalUsers.Text = settings.MaxExternalUsers.ToString();
			chkAutoDeactivateExpired.Checked = settings.AutoDeactivateExpired;
			chkAutoDeleteOutdated.Checked = settings.AutoDeleteOutdated;
			txtOutdatePeriod.Text = settings.OutdatePeriod.ToString();

			chkSendSpam.Checked = settings.SendSpam;

			chkSendSpamOneDayAfter.Checked = settings.SendSpamOneDayAfter;
			txtOneDayAfterPeriod.Text = settings.OneDayAfterPeriod.ToString();

			chkSendSpamOneWeekAfter.Checked = settings.SendSpamOneWeekAfter;
			txtOneWeekAfterPeriod.Text = settings.OneWeekAfterPeriod.ToString();

			chkSendSpamOneWeekBefore.Checked = settings.SendSpamOneWeekBefore;
			txtOneWeekBeforePeriod.Text = settings.OneWeekBeforePeriod.ToString();

			chkSendSpamOneDayBefore.Checked = settings.SendSpamOneDayBefore;
			txtOneDayBeforePeriod.Text = settings.OneDayBeforePeriod.ToString();

			SendBillableSpamCheckBox.Checked = settings.SendBillableSpam;
			SendBillableSpam7dayCheckBox.Checked = settings.SendBillableSpam7day;
			SendBillableSpam3dayCheckBox.Checked = settings.SendBillableSpam3day;
			SendBillableSpam1dayCheckBox.Checked = settings.SendBillableSpam1day;
			SendBillableSpamNegativeBalanceCheckBox.Checked = settings.SendBillableSpamNegativeBalance;

			UseTariffsCheckBox.Checked = settings.UseTariffs;
			AutoDeactivateUnpaidCheckBox.Checked = settings.AutoDeactivateUnpaid;

			SmtpServerValue.Text = settings.SmtpServer;
			SmtpPortValue.Text = settings.SmtpPort.ToString();
			li = SmtpSecureConnectionList.Items.FindByValue(((int)settings.SmtpSecureConnection).ToString());
			if (li != null)
				li.Selected = true;
			SmtpAuthenticateValue.Checked = settings.SmtpAuthenticate;
			SmtpUserValue.Text = settings.SmtpUser;
			SmtpPasswordValue.Text = settings.SmtpPassword;

			li = DefaultBillablePoolList.Items.FindByValue(settings.DefaultBillablePool);
			if (li != null)
				li.Selected = true;

			li = DefaultTrialPoolList.Items.FindByValue(settings.DefaultTrialPool);
			if (li != null)
				li.Selected = true;

			li = DefaultTariff.Items.FindByValue(settings.DefaultTariff.ToString());
			if (li != null)
				li.Selected = true;
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		#region btnSave_Click
		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			AspSettings settings = AspSettings.Load();

			settings.DnsParentDomain = txtDnsParentDomain.Text;

			settings.IisIpAddress = txtIisIpAddress.Text;
			settings.IisPort = int.Parse(txtIisPort.Text);

			settings.MaxHDD = Int32.Parse(txtMaxHDD.Text);
			settings.TrialPeriod = Int32.Parse(txtPeriod.Text);
			settings.MaxUsers = Int32.Parse(txtMaxUsers.Text);
			settings.EmailFrom = txtEMailFrom.Text;
			settings.OperatorEmail = txtOperatorEmail.Text;
			settings.MaxExternalUsers = int.Parse(tbExternalUsers.Text);

			settings.AutoDeactivateExpired = chkAutoDeactivateExpired.Checked;
			settings.AutoDeleteOutdated = chkAutoDeleteOutdated.Checked;
			settings.OutdatePeriod = int.Parse(txtOutdatePeriod.Text);

			settings.SendSpam = chkSendSpam.Checked;

			settings.SendSpamOneDayAfter = chkSendSpamOneDayAfter.Checked && chkSendSpam.Checked;
			settings.OneDayAfterPeriod = int.Parse(txtOneDayAfterPeriod.Text);

			settings.SendSpamOneWeekAfter = chkSendSpamOneWeekAfter.Checked && chkSendSpam.Checked;
			settings.OneWeekAfterPeriod = int.Parse(txtOneWeekAfterPeriod.Text);

			settings.SendSpamOneWeekBefore = chkSendSpamOneWeekBefore.Checked && chkSendSpam.Checked;
			settings.OneWeekBeforePeriod = int.Parse(txtOneWeekBeforePeriod.Text);

			settings.SendSpamOneDayBefore = chkSendSpamOneDayBefore.Checked && chkSendSpam.Checked;
			settings.OneDayBeforePeriod = int.Parse(txtOneDayBeforePeriod.Text);

			settings.UseTariffs = UseTariffsCheckBox.Checked;
			settings.AutoDeactivateUnpaid = AutoDeactivateUnpaidCheckBox.Checked && UseTariffsCheckBox.Checked;

			settings.SendBillableSpam = SendBillableSpamCheckBox.Checked;
			settings.SendBillableSpam7day = SendBillableSpam7dayCheckBox.Checked && SendBillableSpamCheckBox.Checked;
			settings.SendBillableSpam3day = SendBillableSpam3dayCheckBox.Checked && SendBillableSpamCheckBox.Checked;
			settings.SendBillableSpam1day = SendBillableSpam1dayCheckBox.Checked && SendBillableSpamCheckBox.Checked;
			settings.SendBillableSpamNegativeBalance = SendBillableSpamNegativeBalanceCheckBox.Checked && SendBillableSpamCheckBox.Checked;

			settings.SmtpServer = SmtpServerValue.Text;
			settings.SmtpPort = int.Parse(SmtpPortValue.Text);
			settings.SmtpSecureConnection = (SecureConnectionType)int.Parse(SmtpSecureConnectionList.SelectedValue);
			settings.SmtpAuthenticate = SmtpAuthenticateValue.Checked;
			settings.SmtpUser = SmtpUserValue.Text;
			settings.SmtpPassword = SmtpPasswordValue.Text;

			settings.DefaultTrialPool = DefaultTrialPoolList.SelectedValue;
			settings.DefaultBillablePool = DefaultBillablePoolList.SelectedValue;

			if (DefaultTariff.Items.Count > 0)
				settings.DefaultTariff = int.Parse(DefaultTariff.SelectedValue);
			else
				settings.DefaultTariff = -1;

			settings.Save();
			Response.Redirect("settings.aspx");
		}
		#endregion

		#region GetByteArray
		protected byte[] GetByteArray(Mediachase.FileUploader.Web.UI.McHtmlInputFile fControl)
		{
			byte[] MyFile = null;
			if (fControl.PostedFile != null && fControl.PostedFile.ContentLength > 0)
			{
				MyFile = new byte[fControl.PostedFile.ContentLength];
				fControl.PostedFile.InputStream.Read(MyFile, 0, fControl.PostedFile.ContentLength);
			}
			return MyFile;
		}
		#endregion
	}
}
