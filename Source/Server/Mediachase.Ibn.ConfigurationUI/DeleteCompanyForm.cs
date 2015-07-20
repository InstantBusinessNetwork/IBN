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
	public partial class DeleteCompanyForm : Form
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
		public string CompanyId { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether [delete database].
		/// </summary>
		/// <value><c>true</c> if [delete database]; otherwise, <c>false</c>.</value>
		public bool DeleteDatabase { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ChangeDomainForm"/> class.
		/// </summary>
		/// <param name="serverConfigurator">The server configurator.</param>
		/// <param name="companyId">The company id.</param>
		/// <param name="domainName">Name of the domain.</param>
		public DeleteCompanyForm(IConfigurator serverConfigurator, string companyId, string domainName)
		{
			InitializeComponent();

#if RADIUS
			this.Icon = SnapInResources.portal_RS;
#endif 

			this.Configurator = serverConfigurator;
			this.CompanyId = companyId;
			this.labelWarningMsg.Text = string.Format(SnapInResources.CompanyScopeNode_Action_Delete_WarningMessage, domainName);
		}

		/// <summary>
		/// Handles the Click event of the buttonOk control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonOk_Click(object sender, EventArgs e)
		{
			//this.Configurator.DeleteCompany(this.CompanyId, checkBoxDeleteDatabase.Checked);
			this.DeleteDatabase = checkBoxDeleteDatabase.Checked;

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{

		}

	}
}
