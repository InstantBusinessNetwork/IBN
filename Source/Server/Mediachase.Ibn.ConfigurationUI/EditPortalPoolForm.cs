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
	public partial class EditPortalPoolForm : Form
	{
		/// <summary>
		/// Gets or sets the configurator.
		/// </summary>
		/// <value>The configurator.</value>
		protected IConfigurator Configurator { get; private set; }

		/// <summary>
		/// Gets or sets the name of the current pool.
		/// </summary>
		/// <value>The name of the current pool.</value>
		protected string CurrentPool { get; private set; }

		/// <summary>
		/// Gets the portal pool.
		/// </summary>
		/// <value>The portal pool.</value>
		public string PortalPool
		{
			get
			{
				return (comboBoxIisPool.SelectedIndex == 0 ? null : (string)comboBoxIisPool.SelectedItem);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditPortalPoolForm"/> class.
		/// </summary>
		/// <param name="serverConfigurator">The server configurator.</param>
		public EditPortalPoolForm(IConfigurator serverConfigurator, string currentPool)
		{
			InitializeComponent();

#if RADIUS
			this.Icon = SnapInResources.portal_RS;
#endif 

			this.CurrentPool = currentPool;
			this.Configurator = serverConfigurator;

			// Read SQL Server Settings Here
			InitializeIisBlock();
		}

		/// <summary>
		/// Initializes the IIS block.
		/// </summary>
		private void InitializeIisBlock()
		{
			comboBoxIisPool.Items.Add(SnapInResources.CreateCompanyForm_Create_New_Pool);

			// Load Pool Elements from this.Configurator
			// Select current pool name
			int currentPoolIndex = 0;
			foreach (string poolName in this.Configurator.ListApplicationPools())
			{
				int i = comboBoxIisPool.Items.Add(poolName);
				if (poolName == this.CurrentPool)
					currentPoolIndex = i;
			}

			comboBoxIisPool.SelectedIndex = currentPoolIndex;
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

	}
}
