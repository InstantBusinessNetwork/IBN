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
	public partial class CreateAspForm : Form
	{
		/// <summary>
		/// Gets or sets the configurator.
		/// </summary>
		/// <value>The configurator.</value>
		protected IConfigurator Configurator { get; set; }

		/// <summary>
		/// Gets or sets the color of the invalid back.
		/// </summary>
		/// <value>The color of the invalid back.</value>
		protected Color InvalidBackColor { get; private set; }

		/// <summary>
		/// Gets or sets the color of the valid back.
		/// </summary>
		/// <value>The color of the valid back.</value>
		protected Color ValidBackColor { get; private set; }

		/// <summary>
		/// Gets the application pool for web site.
		/// </summary>
		/// <value>The application pool.</value>
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
		/// Initializes a new instance of the <see cref="CreateAspForm"/> class.
		/// </summary>
		/// <param name="serverConfigurator">The server configurator.</param>
		public CreateAspForm(IConfigurator configurator)
		{
			InitializeComponent();

#if RADIUS
			this.Icon = SnapInResources.portal_RS;
#endif 

			this.Configurator = configurator;

			this.ValidBackColor = Color.FromKnownColor(System.Drawing.KnownColor.Window);
			this.InvalidBackColor = Color.FromArgb(255, 192, 192);

			InitalizeAspBlock();
		}

		/// <summary>
		/// Initalizes the ASP block.
		/// </summary>
		private void InitalizeAspBlock()
		{
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

		private void textBoxHost_TextChanged(object sender, EventArgs e)
		{

		}
	}
}
