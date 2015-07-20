using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mediachase.Ibn.Configuration;
using System.Text.RegularExpressions;

namespace Mediachase.Ibn.ConfigurationUI
{
	public partial class EditDefaultAddressForm : Form
	{
		/// <summary>
		/// Gets or sets the configurator.
		/// </summary>
		/// <value>The configurator.</value>
		protected IConfigurator Configurator { get; private set; }

		/// <summary>
		/// Gets or sets the company id.
		/// </summary>
		/// <value>The company id.</value>
		protected string CompanyId { get; private set; }

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
		/// Gets or sets the new host.
		/// </summary>
		/// <value>The new host.</value>
		public string NewHost { get; private set; }

		/// <summary>
		/// Gets or sets the new port.
		/// </summary>
		/// <value>The new port.</value>
		public string NewPort { get; private set; }

		/// <summary>
		/// Gets or sets the new schema.
		/// </summary>
		/// <value>The new schema.</value>
		public string NewSchema { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ChangeDomainForm"/> class.
		/// </summary>
		/// <param name="configurator">The configurator.</param>
		public EditDefaultAddressForm(IConfigurator serverConfigurator, string companyId)
		{
			InitializeComponent();

#if RADIUS
			this.Icon = SnapInResources.portal_RS;
#endif 


			this.Configurator = serverConfigurator;
			this.CompanyId = companyId;

			this.ValidBackColor = Color.FromKnownColor(System.Drawing.KnownColor.Window);
			this.InvalidBackColor = Color.FromArgb(255, 192, 192);

			ICompanyInfo companyInfo = this.Configurator.GetCompanyInfo(companyId);

			comboBoxSchema.SelectedIndex = companyInfo.Scheme == "https" ? 1 : 0;
			textHostName.Text = companyInfo.Host;
			textBoxIisPort.Text = companyInfo.Port;
			
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
				this.NewSchema = comboBoxSchema.SelectedIndex == 0 ? "http" : "https";
				this.NewHost = this.textHostName.Text;
				this.NewPort = this.textBoxIisPort.Text;
				
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{

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

		private void textBoxNewDomainName_Validated(object sender, EventArgs e)
		{
			if (Regex.IsMatch(textHostName.Text, @"^[A-Z0-9-.]+$", RegexOptions.Singleline | RegexOptions.IgnoreCase))
			{
				errorProvider.SetError(textHostName, string.Empty);
				textHostName.BackColor = this.ValidBackColor;
			}
			else
			{
				if (string.IsNullOrEmpty(textHostName.Text))
					errorProvider.SetError(textHostName, SnapInResources.ErrorProviderMsg_RequiredField);
				else
					errorProvider.SetError(textHostName, SnapInResources.ErrorProviderMsg_InvalidDomainName);

				textHostName.BackColor = this.InvalidBackColor;
			}
		}

		private void textBoxIisPort_Validated(object sender, EventArgs e)
		{
			int port;
			if (string.IsNullOrEmpty(textBoxIisPort.Text) || 
				int.TryParse(textBoxIisPort.Text, out port))
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
	}
}
