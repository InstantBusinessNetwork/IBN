using System;
using System.Globalization;
using System.Windows.Forms;

using Microsoft.ManagementConsole;

using Mediachase.Ibn.Configuration;
using System.Net;

namespace Mediachase.Ibn.ConfigurationUI
{
	public partial class ServerFeaturesControl : UserControl, IFormViewControl
	{
		public ServerFormView ParentFormView { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ServerFeaturesControl"/> class.
		/// </summary>
		public ServerFeaturesControl()
		{
			InitializeComponent();
			this.Dock = DockStyle.Fill;
		}

		#region IFormViewControl Members

		/// <summary>
		/// Uses the associated Windows Forms view to initialize the control that implements the interface.
		/// </summary>
		/// <param name="view">The associated <see cref="T:Microsoft.ManagementConsole.FormView"></see> value.</param>
		void IFormViewControl.Initialize(FormView view)
		{
			this.ParentFormView = (ServerFormView)view;

			LoadDataFromConfigurator();
		}

		/// <summary>
		/// Loads the data from configurator.
		/// </summary>
		public void LoadDataFromConfigurator()
		{
			InitGeneralInfoBlock();
			InitSqlServerSettingsBlock();
			InitLicenseInfoBlock();
			InitAspBlock();
		}


		#endregion

		/// <summary>
		/// Gets the configurator.
		/// </summary>
		/// <returns></returns>
		public IConfigurator GetConfigurator()
		{
			return ((ServerScopeNode)this.ParentFormView.ScopeNode).Configurator;
		}

		/// <summary>
		/// Handles the Click event of the buttonEditSqlServerSettings control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonEditSqlServerSettings_Click(object sender, EventArgs e)
		{
			try
			{
				ShowEditSqlServerSettingsForm();
			}
			catch (Exception ex)
			{
				ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
				this.ParentFormView.SnapIn.Console.ShowDialog(exForm);
			}
		}


		private void InitGeneralInfoBlock()
		{
			textBoxServerName.Text = Dns.GetHostName().ToUpperInvariant();
			textBoxTotalCompanies.Text = this.GetConfigurator().ListCompanies(false).Length.ToString();
			textBoxCommonComponentVersion.Text = this.GetConfigurator().CommonVersion.ToString();
		}


		private void InitLicenseInfoBlock()
		{
			try
			{
				foreach (IConfigurationParameter param in this.GetConfigurator().ListLicenseProperties())
				{
					if (param.Name == "LicenseName")
					{
						textBoxLicenseName.Text = param.Value;
					}
					else if (param.Name == "ExpirationDate")
					{
						if (string.IsNullOrEmpty(param.Value))
							textBoxLicenseExpirationDate.Text = SnapInResources.License_Expiration_Unlimited;
						else
							textBoxLicenseExpirationDate.Text = DateTime.Parse(param.Value, CultureInfo.InvariantCulture).ToShortDateString();
					}
				}
			}
			catch (LicenseExpiredException)
			{
				textBoxLicenseName.Text = SnapInResources.License_Expired;
				textBoxLicenseExpirationDate.Text = string.Format(SnapInResources.License_Expired_Text, Mediachase.Ibn.IbnConst.FullProductName);
			}
		}

		/// <summary>
		/// Inits the SQL server settings block.
		/// </summary>
		private void InitSqlServerSettingsBlock()
		{
			IConfigurator configurator = GetConfigurator();
			ISqlServerSettings settings = configurator.SqlSettings;

			//textBoxAdministratorSettings.Text = string.Format(CultureInfo.CurrentCulture,
			//    SnapInResources.ServerFeaturesControl_AdministratorSettings_Format,
			//    settings.Server,
			//    settings.Authentication == AuthenticationType.Windows ? SnapInResources.Authentication_Windows_Name : SnapInResources.Authentication_SqlServer_Name,
			//    settings.AdminUser);
			textBoxSqlServer.Text = settings.Server;
			textBoxSqlAuthentication.Text = settings.Authentication == AuthenticationType.Windows ? SnapInResources.Authentication_Windows_Name : SnapInResources.Authentication_SqlServer_Name;
			textBoxSqlAdminUser.Text = settings.AdminUser;

			textBoxSqlAccountForIbnPortal.Text = settings.PortalUser;
		}

		private void InitAspBlock()
		{
			IConfigurator configurator = GetConfigurator();

			groupBoxAsp.Visible = configurator.CanCreateAspSite() || configurator.CanDeleteAspSite();

			if (configurator.CanCreateAspSite())
			{
				buttonInstallAsp.Visible = true;
				buttonUninstallAsp.Visible = false;
			}
			else if (configurator.CanDeleteAspSite())
			{
				buttonInstallAsp.Visible = false;
				buttonUninstallAsp.Visible = true;
			}

			IAspInfo aspInfo = configurator.GetAspInfo();
			if (aspInfo != null)
			{
				// Fill Asp Information
				int port = (string.IsNullOrEmpty(aspInfo.Port) ? -1 : int.Parse(aspInfo.Port, CultureInfo.InvariantCulture));
				linkLabelAspUrl.Text = new UriBuilder(aspInfo.Scheme, aspInfo.Host, port).ToString();
				buttonAspConfigure.Enabled = true;

				textBoxAspSiteId.Text = aspInfo.SiteId.ToString();
				textBoxAspDatabase.Text = aspInfo.Database;
			}
			else
			{
				linkLabelAspUrl.Text = string.Empty;
				buttonAspConfigure.Enabled = false;

				textBoxAspSiteId.Text = string.Empty;
				textBoxAspDatabase.Text = string.Empty;

			}
		}

		/// <summary>
		/// Shows the edit SQL server settings form.
		/// </summary>
		public void ShowEditSqlServerSettingsForm()
		{
			EditSqlServerSettingsForm form = new EditSqlServerSettingsForm(GetConfigurator());

			if (this.ParentFormView.SnapIn.Console.ShowDialog(form) == DialogResult.OK)
			{
				GetConfigurator().ChangeSqlServerSettings(
					form.SqlServerName,
					form.UseWindowsAuth ? AuthenticationType.Windows : AuthenticationType.SqlServer,
					form.SqlServerUser,
					form.SqlServerPassword,
					form.IbnUserName,
					form.IbnUserPassword);

				InitSqlServerSettingsBlock();
			}
		}

		/// <summary>
		/// Handles the Click event of the buttonUninstallAsp control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonUninstallAsp_Click(object sender, EventArgs e)
		{
			DeleteAspForm form = new DeleteAspForm();

			if (this.ParentFormView.SnapIn.Console.ShowDialog(form) == DialogResult.OK)
			{
				//status.ReportProgress(0, 0, SnapInResources.ServerFormView_Action_AspDelete_Progress);

				this.GetConfigurator().DeleteAspSite(form.DeleteDatabase);
				//Thread.Sleep(10000);

				InitAspBlock();
			}
		}

		/// <summary>
		/// Handles the Click event of the buttonInstallAsp control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonInstallAsp_Click(object sender, EventArgs e)
		{
			ServerFormView.CreateAspSite(this.GetConfigurator(), this.ParentFormView.SnapIn.Console, null);
			InitAspBlock();
		}

		internal void ReloadData()
		{
			InitSqlServerSettingsBlock();
			InitAspBlock();
		}

		private void buttonAspConfigure_Click(object sender, EventArgs e)
		{
			EditAspSettingsForm form = new EditAspSettingsForm(this.GetConfigurator());

			if (this.ParentFormView.SnapIn.Console.ShowDialog(form) == DialogResult.OK)
			{
				this.GetConfigurator().ChangeAspApplicationPool(form.ApplicationPool);
				this.GetConfigurator().ChangeAspAddress(form.AspSchema, form.AspHost, form.AspPort);

				InitAspBlock();
			}
		}

		private void linkLabelAspUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Help.ShowHelp(null, linkLabelAspUrl.Text);
		}
	}
}
