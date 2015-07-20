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
	/// <summary>
	/// Represents update server form.
	/// </summary>
	public partial class UpdateServerForm : Form
	{
		/// <summary>
		/// 
		/// </summary>
		public delegate void UpdateCompanyHandler(int versionId);
		/// <summary>
		/// 
		/// </summary>
		public delegate void UpdateCompanyResultHandler(int errorCode);
		/// <summary>
		/// 
		/// </summary>
		public delegate void AddLogRecordHandler(string line);

		/// <summary>
		/// Gets or sets the configurator.
		/// </summary>
		/// <value>The configurator.</value>
		protected IConfigurator Configurator { get; private set; }


		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateServerForm"/> class.
		/// </summary>
		/// <param name="serverConfigurator">The server configurator.</param>
		public UpdateServerForm(IConfigurator configurator)
		{
			InitializeComponent();

#if RADIUS
			this.Icon = SnapInResources.portal_RS;
#endif 

			this.Configurator = configurator;
			InitializePage1();
		}

		/// <summary>
		/// Initializes the page1.
		/// </summary>
		private void InitializePage1()
		{
			int[] availableUpdates = this.Configurator.ListUpdates();

			foreach (int availableUpdate in availableUpdates)
			{
				int index = comboBoxAvailableUpdates.Items.Add(availableUpdate);
			}

			comboBoxAvailableUpdates.SelectedIndex = comboBoxAvailableUpdates.Items.Count - 1;

			this.wizard.FinishButtonText = SnapInResources.UpdateServerForm_InstallButton_Text;
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the comboBoxAvailableUpdates control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void comboBoxAvailableUpdates_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		/// <summary>
		/// Gets the update id.
		/// </summary>
		/// <returns></returns>
		public int GetUpdateId()
		{
			return (int)comboBoxAvailableUpdates.SelectedItem;
		}

		private void wizardPage1Info_CloseFromNext(object sender, Mediachase.Ibn.ConfigurationUI.Wizard.PageEventArgs e)
		{

		}

		private void wizard_CloseFromCancel(object sender, CancelEventArgs e)
		{

		}

		private void wizardPage1Info_ShowFromNext(object sender, EventArgs e)
		{

		}
	}
}
