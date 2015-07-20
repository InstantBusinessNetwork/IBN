using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Mediachase.Ibn.Configuration;

namespace Mediachase.Ibn.ConfigurationUI
{
	public partial class CreateCompanyForm : Form
	{
		private bool _emailHasChanged;
		private bool _automaticChanging;

		/// <summary>
		/// Gets or sets the color of the invalid background.
		/// </summary>
		/// <value>The color of the invalid back.</value>
		protected Color InvalidBackColor { get; private set; }

		/// <summary>
		/// Gets or sets the color of the valid background.
		/// </summary>
		/// <value>The color of the valid back.</value>
		protected Color ValidBackColor { get; private set; }

		/// <summary>
		/// Gets the portal pool.
		/// </summary>
		/// <value>The portal pool.</value>
		public string IisPool
		{
			get
			{
				return comboBoxIisPool.SelectedIndex == 0 ?
					string.Empty :
					(string)comboBoxIisPool.SelectedItem;
			}
		}

		/// <summary>
		/// Gets the IP address for web site.
		/// </summary>
		/// <value>The IP address.</value>
		public string IisIPAddress
		{
			get
			{
				return comboBoxIisIPAddress.SelectedIndex == 0 ?
					string.Empty :
					(string)comboBoxIisIPAddress.SelectedItem;
			}
		}

		/// <summary>
		/// Gets or sets the configurator.
		/// </summary>
		/// <value>The configurator.</value>
		protected IConfigurator Configurator { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateCompanyForm"/> class.
		/// </summary>
		/// <param name="serverConfigurator">The server configurator.</param>
		public CreateCompanyForm(IConfigurator serverConfigurator)
		{
			InitializeComponent();

#if RADIUS
			this.Icon = SnapInResources.portal_RS;
#endif 

			this.Configurator = serverConfigurator;

			this.ValidBackColor = Color.FromKnownColor(System.Drawing.KnownColor.Window);
			this.InvalidBackColor = Color.FromArgb(255, 192, 192);

			InitializeIisBlock();
			InitializeAdminBlock();
		}

		/// <summary>
		/// Initializes the IIS block.
		/// </summary>
		/// <param name="serverConfigurator">The server configurator.</param>
		private void InitializeIisBlock()
		{
			int defaultLanguageIndex = 0;
			ILanguageInfo[] languages = this.Configurator.ListLanguages();
			if (languages.Length > 1)
			{
				for (int i = 0; i < languages.Length; i++)
				{
					if (languages[i].Locale == "ru-RU")
					{
						IConfigurationParameter[] serverProperties = this.Configurator.ListServerProperties();
						foreach (IConfigurationParameter parameter in serverProperties)
						{
							if (parameter.Name == "ServerEdition" && (parameter.Value == "RU" || parameter.Value == "RS"))
							{
								defaultLanguageIndex = i;
								break;
							}
						}
						break;
					}
				}
			}

			comboBoxDefaultLanguage.DisplayMember = "FriendlyName";
			comboBoxDefaultLanguage.DataSource = languages;
			this.comboBoxDefaultLanguage.SelectedIndex = defaultLanguageIndex;

			textBoxHost.Text = this.Configurator.HostName.ToLowerInvariant();

			comboBoxIisIPAddress.Items.Add(SnapInResources.IPAddress_All_Unassigned);
			// Load IP addresses from configurator
			foreach (string item in this.Configurator.ListIPAddresses())
				comboBoxIisIPAddress.Items.Add(item);
			comboBoxIisIPAddress.SelectedIndex = 0;

			comboBoxIisPool.Items.Add(SnapInResources.CreateCompanyForm_Create_New_Pool);
			// Load pools from configurator
			foreach (string item in this.Configurator.ListApplicationPools())
				comboBoxIisPool.Items.Add(item);
			comboBoxIisPool.SelectedIndex = 0;
		}


		/// <summary>
		/// Initializes the admin block.
		/// </summary>
		private void InitializeAdminBlock()
		{
			textBoxAdminLastName.Text = IbnConst.ProductFamilyShort;
		}

		/// <summary>
		/// Handles the Click event of the buttonOk control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonOk_Click(object sender, EventArgs e)
		{
			if (CheckErrorProvider(this.Controls))
			{
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		/// <summary>
		/// Checks the error provider.
		/// </summary>
		/// <param name="controls">The controls.</param>
		/// <returns></returns>
		private bool CheckErrorProvider(Control.ControlCollection controls)
		{
			this.ValidateChildren();

			foreach (Control control in controls)
			{
				if (!string.IsNullOrEmpty(errorProvider.GetError(control)))
					return false;

				if (control.HasChildren && !CheckErrorProvider(control.Controls))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Handles the Click event of the buttonCancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonCancel_Click(object sender, EventArgs e)
		{

		}

		#region Company Section Validators
		/// <summary>
		/// Handles the Validated event of the textBoxCompanyName control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void textBoxCompanyName_Validated(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(textBoxCompanyName.Text))
			{
				errorProvider.SetError(textBoxCompanyName, SnapInResources.ErrorProviderMsg_RequiredField);
				textBoxCompanyName.BackColor = this.InvalidBackColor;
			}
			else
			{
				errorProvider.SetError(textBoxCompanyName, string.Empty);
				textBoxCompanyName.BackColor = this.ValidBackColor;
			}
		}

		/// <summary>
		/// Handles the Validated event of the textBoxDomainName control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void textBoxDomainName_Validated(object sender, EventArgs e)
		{
			if (Regex.IsMatch(textBoxHost.Text, @"^[A-Z0-9-.]+$", RegexOptions.Singleline | RegexOptions.IgnoreCase))
			{
				errorProvider.SetError(textBoxHost, string.Empty);
				textBoxHost.BackColor = this.ValidBackColor;
			}
			else
			{
				if (string.IsNullOrEmpty(textBoxHost.Text))
					errorProvider.SetError(textBoxHost, SnapInResources.ErrorProviderMsg_RequiredField);
				else
					errorProvider.SetError(textBoxHost, SnapInResources.ErrorProviderMsg_InvalidDomainName);

				textBoxHost.BackColor = this.InvalidBackColor;
			}
		}

		/// <summary>
		/// Handles the Validated event of the textBoxIisPort control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void textBoxIisPort_Validated(object sender, EventArgs e)
		{
			int port;
			if (int.TryParse(textBoxIisPort.Text, out port))
			{
				errorProvider.SetError(textBoxIisPort, string.Empty);
				textBoxIisPort.BackColor = this.ValidBackColor;
			}
			else
			{
				errorProvider.SetError(textBoxIisPort, SnapInResources.ErrorProviderMsg_InvalidIisPort);
				textBoxIisPort.BackColor = this.InvalidBackColor;
			}
		}
		
		#endregion

		#region Admin Section Validators
		/// <summary>
		/// Handles the Validated event of the textBoxAdminAccountName control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void textBoxAdminAccountName_Validated(object sender, EventArgs e)
		{
			if (Regex.IsMatch(textBoxAdminAccountName.Text, @"^[A-Z0-9]+$", RegexOptions.Singleline | RegexOptions.IgnoreCase))
			{
				errorProvider.SetError(textBoxAdminAccountName, string.Empty);
				textBoxAdminAccountName.BackColor = this.ValidBackColor;
			}
			else
			{
				if (string.IsNullOrEmpty(textBoxAdminAccountName.Text))
					errorProvider.SetError(textBoxAdminAccountName, SnapInResources.ErrorProviderMsg_RequiredField);
				else
					errorProvider.SetError(textBoxAdminAccountName, SnapInResources.ErrorProviderMsg_InvalidAdminLogin);

				textBoxAdminAccountName.BackColor = this.InvalidBackColor;
			}
		}

		/// <summary>
		/// Handles the Validated event of the textBoxAdminFirstName control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void textBoxAdminFirstName_Validated(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(textBoxAdminFirstName.Text))
			{
				errorProvider.SetError(textBoxAdminFirstName, SnapInResources.ErrorProviderMsg_RequiredField);
				textBoxAdminFirstName.BackColor = this.InvalidBackColor;
			}
			else
			{
				errorProvider.SetError(textBoxAdminFirstName, string.Empty);
				textBoxAdminFirstName.BackColor = this.ValidBackColor;
			}
		}

		/// <summary>
		/// Handles the Validated event of the textBoxAdminLastName control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void textBoxAdminLastName_Validated(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(textBoxAdminLastName.Text))
			{
				errorProvider.SetError(textBoxAdminLastName, SnapInResources.ErrorProviderMsg_RequiredField);
				textBoxAdminLastName.BackColor = this.InvalidBackColor;
			}
			else
			{
				errorProvider.SetError(textBoxAdminLastName, string.Empty);
				textBoxAdminLastName.BackColor = this.ValidBackColor;
			}
		}

		/// <summary>
		/// Handles the Validated event of the textBoxAdminEmail control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void textBoxAdminEmail_Validated(object sender, EventArgs e)
		{
			if (Regex.IsMatch(textBoxAdminEmail.Text, @"^[A-Z0-9._%+-]+@[A-Z0-9.-]", RegexOptions.Singleline | RegexOptions.IgnoreCase))
			{
				errorProvider.SetError(textBoxAdminEmail, string.Empty);
				textBoxAdminEmail.BackColor = this.ValidBackColor;
			}
			else
			{
				if (string.IsNullOrEmpty(textBoxAdminEmail.Text))
					errorProvider.SetError(textBoxAdminEmail, SnapInResources.ErrorProviderMsg_RequiredField);
				else
					errorProvider.SetError(textBoxAdminEmail, SnapInResources.ErrorProviderMsg_InvalidAdminEmail);

				textBoxAdminEmail.BackColor = this.InvalidBackColor;
			}
		}

		/// <summary>
		/// Handles the Validated event of the textBoxAdminPassword control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void textBoxAdminPassword_Validated(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(textBoxAdminPassword.Text))
			{
				errorProvider.SetError(textBoxAdminPassword, SnapInResources.ErrorProviderMsg_RequiredField);
				textBoxAdminPassword.BackColor = this.InvalidBackColor;
			}
			else
			{
				errorProvider.SetError(textBoxAdminPassword, string.Empty);
				textBoxAdminPassword.BackColor = this.ValidBackColor;
			}
		}

		/// <summary>
		/// Handles the Validated event of the textBoxAdminPasswordRe control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void textBoxAdminPasswordRe_Validated(object sender, EventArgs e)
		{
			if (textBoxAdminPasswordRe.Text != textBoxAdminPassword.Text)
			{
				errorProvider.SetError(textBoxAdminPasswordRe, SnapInResources.ErrorProviderMsg_PassDontMatch);
				textBoxAdminPasswordRe.BackColor = this.InvalidBackColor;
			}
			else
			{
				errorProvider.SetError(textBoxAdminPasswordRe, string.Empty);
				textBoxAdminPasswordRe.BackColor = this.ValidBackColor;
			}
		} 
		#endregion

		private void textBoxHost_TextChanged(object sender, EventArgs e)
		{
			GenerateAdminEmail();
		}

		private void textBoxAdminAccountName_TextChanged(object sender, EventArgs e)
		{
			GenerateAdminEmail();
		}

		private void textBoxAdminEmail_TextChanged(object sender, EventArgs e)
		{
			if (!_automaticChanging)
				_emailHasChanged = true;
		}

		private void GenerateAdminEmail()
		{
			if (!_emailHasChanged)
			{
				_automaticChanging = true;
				textBoxAdminEmail.Text = textBoxAdminAccountName.Text + "@" + textBoxHost.Text;
				_automaticChanging = false;
			}
		}
	}
}
