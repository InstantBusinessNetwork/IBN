using System;
using System.Windows.Forms;

using Microsoft.ManagementConsole;
using Microsoft.ManagementConsole.Advanced;

using Mediachase.Ibn.Configuration;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents companies scope node.
	/// </summary>
	public class CompaniesScopeNode : ConfigurationScopeNode
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="CompaniesScopeNode"/> class.
		/// </summary>
		public CompaniesScopeNode(IConfigurator configurator)
		{
			this.Configurator = configurator;

			// Init View Params and Tag
			this.DataElementType = DataElementType.Companies;
			this.DisplayName = SnapInResources.ScopeNodeDispayName_Companies;
			this.Tag = string.Empty;
			this.ImageIndex = 3;
			this.SelectedImageIndex = 3;

			// Init Right Panel
			// Create a message view for the root node.
			MmcListViewDescription lvd = new MmcListViewDescription();
			lvd.DisplayName = SnapInResources.ScopeNodeDispayName_Companies;
			lvd.ViewType = typeof(CompanyListView);
			//lvd.Options = MmcListViewOptions.SingleSelect;

			this.ViewDescriptions.Add(lvd);
			this.ViewDescriptions.DefaultIndex = 0;

			// Init Actions
			this.EnabledStandardVerbs = StandardVerbs.Refresh;

			this.CreateNewCompany = new SyncAction(SnapInResources.CompaniesScopeNode_Action_Create_Name, 
				SnapInResources.CompaniesScopeNode_Action_Create_Description, 8);
			this.CreateNewCompanyForDatabase = new SyncAction(SnapInResources.CompaniesScopeNode_Action_CreateForDatabase_Name,
				SnapInResources.CompaniesScopeNode_Action_CreateForDatabase_Description, 31);


			this.ActionsPaneItems.Add(this.CreateNewCompany);
			this.ActionsPaneItems.Add(this.CreateNewCompanyForDatabase);

			// Read Copmpanies From Config
			Refresh();

		}

		private void LoadCompanies()
		{
			foreach (ICompanyInfo companyInfo in GetCompanyElements())
			{
				CompanyScopeNode companyScopeNode = new CompanyScopeNode(companyInfo);
				this.Children.Add(companyScopeNode);

				// Domain Alias
#if DEBUG
				ScopeNode domainAliasesScopeNode = new ScopeNode();
				companyScopeNode.Children.Add(domainAliasesScopeNode);

				DataElement domainAliases = new DataElement(DataElementType.DomainAliases,
					string.Empty,
					SnapInResources.ScopeNodeDispayName_DomainAliases);

				domainAliasesScopeNode.DisplayName = domainAliases.DisplayName;
				domainAliasesScopeNode.Tag = domainAliases;
				domainAliasesScopeNode.ImageIndex = 5;
				domainAliasesScopeNode.SelectedImageIndex = 5; 
#endif
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the configurator.
		/// </summary>
		/// <value>The configurator.</value>
		public IConfigurator Configurator { get; private set; }

		/// <summary>
		/// Gets or sets the create new company.
		/// </summary>
		/// <value>The create new company.</value>
		protected SyncAction CreateNewCompany { get; private set; }

		/// <summary>
		/// Gets or sets the create new company for database.
		/// </summary>
		/// <value>The create new company for database.</value>
		protected SyncAction CreateNewCompanyForDatabase { get; private set; }
		#endregion

		#region Methods

		/// <summary>
		/// Gets the company name list.
		/// </summary>
		/// <param name="serverConfigurator">The server configurator.</param>
		/// <returns></returns>
		private ICompanyInfo[] GetCompanyElements()
		{
			return this.Configurator.ListCompanies(false);
		}

		/// <summary>
		/// Refreshes this instance.
		/// </summary>
		internal void Refresh()
		{
			this.Children.Clear();

			LoadCompanies();
			//LoadCompanies();
			//LoadCompanies();
		}

		/// <summary>
		/// Called when the standard verb <see cref="F:Microsoft.ManagementConsole.StandardVerbs.Refresh"></see> is triggered.
		/// </summary>
		/// <param name="status">The object  that holds the status information.</param>
		protected override void OnRefresh(AsyncStatus status)
		{
			this.Refresh();
		}
	
		/// <summary>
		/// Called when an action is triggered for the node. Derived classes should override this method to provide application-specific handling of the action.
		/// </summary>
		/// <param name="action">The action that has been triggered.</param>
		/// <param name="status">The status object.</param>
		protected override void OnSyncAction(SyncAction action, SyncStatus status)
		{
			if (action == this.CreateNewCompany)
			{
				try
				{
					OnCreateNewCompany(status);
				}
				catch (LicenseExpiredException)
				{
					MessageBoxParameters msgBox = new MessageBoxParameters();
					msgBox.Icon = MessageBoxIcon.Error;
					msgBox.Caption = SnapInResources.License_Expired;
					msgBox.Text = string.Format(SnapInResources.License_Expired_Text, Mediachase.Ibn.IbnConst.FullProductName);
					msgBox.Buttons = MessageBoxButtons.OK;

					this.SnapIn.Console.ShowDialog(msgBox);
				}
				catch (Exception ex)
				{
					ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
					this.SnapIn.Console.ShowDialog(exForm);
				}
			}
			else if (action == this.CreateNewCompanyForDatabase)
			{
				try
				{
					OnCreateNewCompanyForDatabase(status);
				}
				catch (LicenseExpiredException)
				{
					MessageBoxParameters msgBox = new MessageBoxParameters();
					msgBox.Icon = MessageBoxIcon.Error;
					msgBox.Caption = SnapInResources.License_Expired;
					msgBox.Text = string.Format(SnapInResources.License_Expired_Text, Mediachase.Ibn.IbnConst.FullProductName);
					msgBox.Buttons = MessageBoxButtons.OK;

					this.SnapIn.Console.ShowDialog(msgBox);
				}
				catch (Exception ex)
				{
					ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
					this.SnapIn.Console.ShowDialog(exForm);
				}
			}
		}

		/// <summary>
		/// Called when [create new company].
		/// </summary>
		/// <param name="status">The status.</param>
		private void OnCreateNewCompany(SyncStatus status)
		{
			// Check Sql Settings
			ISqlServerSettings sqlSettings = this.Configurator.SqlSettings;

			if (string.IsNullOrEmpty(sqlSettings.Server))
			{
				// TODO: Show Warning
				EditSqlServerSettingsForm editSqlSettinnsForm = new EditSqlServerSettingsForm(this.Configurator);

				if (this.SnapIn.Console.ShowDialog(editSqlSettinnsForm) == DialogResult.OK)
				{
					this.Configurator.ChangeSqlServerSettings(
					editSqlSettinnsForm.SqlServerName,
					editSqlSettinnsForm.UseWindowsAuth ? AuthenticationType.Windows : AuthenticationType.SqlServer,
					editSqlSettinnsForm.SqlServerUser,
					editSqlSettinnsForm.SqlServerPassword,
					editSqlSettinnsForm.IbnUserName,
					editSqlSettinnsForm.IbnUserPassword);
				}
				else
					return;
			}

			CreateCompanyForm createCompanyForm = new CreateCompanyForm(this.Configurator);

			if (this.SnapIn.Console.ShowDialog(createCompanyForm) == DialogResult.OK)
			{
				status.ReportProgress(0, 0, string.Format(SnapInResources.CompanyScopeNode_Action_Create_Progress, createCompanyForm.textBoxHost.Text));

				string newCompanyId = string.Empty;

				//Thread.Sleep(10000);

				newCompanyId = this.Configurator.CreateCompany(createCompanyForm.textBoxCompanyName.Text,
						createCompanyForm.textBoxHost.Text,
						((ILanguageInfo)createCompanyForm.comboBoxDefaultLanguage.SelectedItem).Locale,
						createCompanyForm.checkBoxIsActive.Checked,
						createCompanyForm.IisIPAddress,
						int.Parse(createCompanyForm.textBoxIisPort.Text),
						createCompanyForm.IisPool,
						createCompanyForm.textBoxAdminAccountName.Text,
						createCompanyForm.textBoxAdminPassword.Text,
						createCompanyForm.textBoxAdminFirstName.Text,
						createCompanyForm.textBoxAdminLastName.Text,
						createCompanyForm.textBoxAdminEmail.Text);

				// Refresh Company List
				Refresh();

				// TODO: Navigate browser to form.NewCompanyId Url
			}
		}

		/// <summary>
		/// Called when [create new company for database].
		/// </summary>
		/// <param name="status">The status.</param>
		private void OnCreateNewCompanyForDatabase(SyncStatus status)
		{
			// Check Sql Settings
			ISqlServerSettings sqlSettings = this.Configurator.SqlSettings;

			if (string.IsNullOrEmpty(sqlSettings.Server))
			{
				// TODO: Show Warning
				EditSqlServerSettingsForm editSqlSettinnsForm = new EditSqlServerSettingsForm(this.Configurator);

				if (this.SnapIn.Console.ShowDialog(editSqlSettinnsForm) == DialogResult.OK)
				{
					this.Configurator.ChangeSqlServerSettings(
					editSqlSettinnsForm.SqlServerName,
					editSqlSettinnsForm.UseWindowsAuth ? AuthenticationType.Windows : AuthenticationType.SqlServer,
					editSqlSettinnsForm.SqlServerUser,
					editSqlSettinnsForm.SqlServerPassword,
					editSqlSettinnsForm.IbnUserName,
					editSqlSettinnsForm.IbnUserPassword);
				}
				else
					return;
			}

			CreateCompanyForDatabaseForm createCompanyForDatabaseForm = new CreateCompanyForDatabaseForm(this.Configurator);

			if (this.SnapIn.Console.ShowDialog(createCompanyForDatabaseForm) == DialogResult.OK)
			{
				status.ReportProgress(0, 0, string.Format(SnapInResources.CompanyScopeNode_Action_CreateForDatabase_Progress, createCompanyForDatabaseForm.textBoxHost.Text));

				string newCompanyId = string.Empty;

				//Thread.Sleep(10000);

				newCompanyId = this.Configurator.CreateCompanyForDatabase(
					createCompanyForDatabaseForm.comboBoxSqlDatabase.Text,
					DateTime.UtcNow,
					createCompanyForDatabaseForm.checkBoxIsActive.Checked,
					createCompanyForDatabaseForm.textBoxHost.Text,
					createCompanyForDatabaseForm.IisIPAddress,
					int.Parse(createCompanyForDatabaseForm.textBoxIisPort.Text),
					createCompanyForDatabaseForm.IisPool,
					true);

				// Refresh Company List
				Refresh();

				// TODO: Navigate browser to form.NewCompanyId Url
			}
		}
		#endregion

		
	}
}
