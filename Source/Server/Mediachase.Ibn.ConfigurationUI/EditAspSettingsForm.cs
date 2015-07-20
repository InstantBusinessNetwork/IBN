using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Mediachase.Ibn.Configuration;
using System.Diagnostics;
using System.Data.SqlClient;

namespace Mediachase.Ibn.ConfigurationUI
{
	public partial class EditAspSettingsForm : Form
	{
		/// <summary>
		/// Gets or sets the configurator.
		/// </summary>
		/// <value>The configurator.</value>
		protected IConfigurator Configurator { get; private set; }

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
		/// Gets the portal pool.
		/// </summary>
		/// <value>The portal pool.</value>
		public string ApplicationPool
		{
			get
			{
				return (comboBoxIisPool.SelectedIndex == 0 ? string.Empty : (string)comboBoxIisPool.SelectedItem);
			}
		}

		/// <summary>
		/// Gets the ASP schema.
		/// </summary>
		/// <value>The ASP schema.</value>
		public string AspSchema
		{
			get
			{
				return comboBoxSchema.SelectedIndex == 0 ? "http" : "https";
			}
		}

		/// <summary>
		/// Gets the ASP schema.
		/// </summary>
		/// <value>The ASP schema.</value>
		public string AspHost
		{
			get
			{
				return textHostName.Text;
			}
		}

		/// <summary>
		/// Gets the ASP port.
		/// </summary>
		/// <value>The ASP port.</value>
		public string AspPort
		{
			get
			{
				return textBoxIisPort.Text;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditAspSettingsForm"/> class.
		/// </summary>
		/// <param name="serverConfigurator">The server configurator.</param>
		public EditAspSettingsForm(IConfigurator serverConfigurator)
		{
			InitializeComponent();

#if RADIUS
			this.Icon = SnapInResources.portal_RS;
#endif 


			this.Configurator = serverConfigurator;

			//this.EnableForeColor = Color.FromKnownColor(System.Drawing.KnownColor.ControlText);
			//this.DisableForeColor = Color.FromKnownColor(System.Drawing.KnownColor.InactiveCaptionText);

			this.ValidBackColor = Color.FromKnownColor(System.Drawing.KnownColor.Window);
			this.InvalidBackColor = Color.FromArgb(255, 192, 192);


			// Read SQL Server Settings Here
			InitAspAddressBlock();
			InitIISPoolBlock();
		}

		private void InitAspAddressBlock()
		{
			IAspInfo aspInfo = this.Configurator.GetAspInfo();

			if (aspInfo != null)
			{
				comboBoxSchema.SelectedIndex = aspInfo.Scheme == "https" ? 1 : 0;
				textHostName.Text = aspInfo.Host;
				textBoxIisPort.Text = aspInfo.Port;
			}
		}

		private void InitIISPoolBlock()
		{
			IAspInfo aspInfo = this.Configurator.GetAspInfo();

			if (aspInfo!=null)
			{
				comboBoxIisPool.Items.Add(SnapInResources.CreateCompanyForm_Create_New_Pool);

				// Enum Available pools
				int selectedIndex = 0;
				foreach (string poolName in this.Configurator.ListApplicationPools())
				{
					int index = comboBoxIisPool.Items.Add(poolName);
					if (poolName == aspInfo.ApplicationPool)
						selectedIndex = index;
				}

				comboBoxIisPool.SelectedIndex = selectedIndex;
			}
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
				// TODO: Fixed Items Here

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

		private void textHostName_Validated(object sender, EventArgs e)
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
	}
}
