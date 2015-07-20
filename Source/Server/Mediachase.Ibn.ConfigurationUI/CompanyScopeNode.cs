using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Microsoft.ManagementConsole;

using Mediachase.Ibn.Configuration;
using System.Diagnostics;
using Microsoft.ManagementConsole.Advanced;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents .
	/// </summary>
	public class CompanyScopeNode : ConfigurationScopeNode
	{
		#region Const
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets a value indicating whether this instance is active.
		/// </summary>
		/// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is enable sheduler service.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is enable sheduler service; otherwise, <c>false</c>.
		/// </value>
		public bool IsEnableShedulerService { get; set; }

		/// <summary>
		/// Gets or sets the application pool.
		/// </summary>
		/// <value>The application pool.</value>
		public string ApplicationPool { get; set; }

		/// <summary>
		/// Gets or sets the company info.
		/// </summary>
		/// <value>The company info.</value>
		protected ICompanyInfo CompanyInfo { get; private set; }

		/// <summary>
		/// Gets or sets the start.
		/// </summary>
		/// <value>The start.</value>
		protected SyncAction StartAction { get; private set; }
		/// <summary>
		/// Gets or sets the stop.
		/// </summary>
		/// <value>The stop.</value>
		protected SyncAction StopAction { get; private set; }
		/// <summary>
		/// Gets or sets the change domain.
		/// </summary>
		/// <value>The change domain.</value>
		protected SyncAction ChangeDomainAction { get; private set; }
		/// <summary>
		/// Gets or sets the upgrade.
		/// </summary>
		/// <value>The upgrade.</value>
		protected SyncAction UpgradeAction { get; private set; }

		/// <summary>
		/// Gets or sets the browse action.
		/// </summary>
		/// <value>The browse action.</value>
		protected SyncAction BrowseAction { get; private set; }

		/// <summary>
		/// Gets or sets the start.
		/// </summary>
		/// <value>The start.</value>
		protected SyncAction EnableSchedulerServiceAction { get; private set; }
		/// <summary>
		/// Gets or sets the stop.
		/// </summary>
		/// <value>The stop.</value>
		protected SyncAction DisableSchedulerServiceAction { get; private set; }

		/// <summary>
		/// Gets or sets the stop.
		/// </summary>
		/// <value>The stop.</value>
		protected SyncAction ChangePortalPool { get; private set; }

		internal CompanySettingsListView CompanySettingsListView { get; set; }
		#endregion


		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="CompanyScopeNode"/> class.
		/// </summary>
		public CompanyScopeNode(ICompanyInfo companyInfo)
		{
			this.CompanyInfo = companyInfo;

			// Init Data
			LoadCompanyInfo();

			// Init Actions
			this.DataElementType = DataElementType.Company;
			this.EnabledStandardVerbs = StandardVerbs.Delete| StandardVerbs.Refresh;

			this.StartAction = new SyncAction(SnapInResources.CompanyScopeNode_Action_Start_Name,
				SnapInResources.CompanyScopeNode_Action_Start_Description, 10);
			this.StopAction = new SyncAction(SnapInResources.CompanyScopeNode_Action_Stop_Name,
				SnapInResources.CompanyScopeNode_Action_Stop_Description, 11);
			this.ChangeDomainAction = new SyncAction(SnapInResources.CompanyScopeNode_Action_UpdateAddress_Name,
				SnapInResources.CompanyScopeNode_Action_ChangeDomain_Description, 12);
			this.UpgradeAction = new SyncAction(SnapInResources.CompanyScopeNode_Action_Upgrade_Name,
				SnapInResources.CompanyScopeNode_Action_Upgrade_Description, 6);

			this.BrowseAction = new SyncAction(SnapInResources.CompanyScopeNode_Action_Browse_Name,
				SnapInResources.CompanyScopeNode_Action_Browse_Description, 16);

			this.EnableSchedulerServiceAction = new SyncAction(SnapInResources.CompanyScopeNode_Action_EnableSchedulerService_Name,
				SnapInResources.CompanyScopeNode_Action_EnableSchedulerService_Description, 27);
			this.DisableSchedulerServiceAction = new SyncAction(SnapInResources.CompanyScopeNode_Action_DisableSchedulerService_Name,
				SnapInResources.CompanyScopeNode_Action_DisableSchedulerService_Description, 28);

			this.ChangePortalPool = new SyncAction(SnapInResources.CompanyScopeNode_Action_ChangePortalPool_Name,
				SnapInResources.CompanyScopeNode_Action_ChangePortalPool_Description, 29);

			this.ActionsPaneItems.Add(this.BrowseAction);
			this.ActionsPaneItems.Add(new ActionSeparator());
			this.ActionsPaneItems.Add(this.StartAction);
			this.ActionsPaneItems.Add(this.StopAction);
			this.ActionsPaneItems.Add(new ActionSeparator());
			this.ActionsPaneItems.Add(this.ChangeDomainAction);
			this.ActionsPaneItems.Add(this.ChangePortalPool);
			this.ActionsPaneItems.Add(this.UpgradeAction);
			this.ActionsPaneItems.Add(new ActionSeparator());

			// Not Implemented yet
			//this.UpgradeAction.Enabled = false;

			// Init view
			MmcListViewDescription lvd = new MmcListViewDescription();
			lvd.DisplayName = SnapInResources.CompanySettingsListView_DisplayName;
			lvd.ViewType = typeof(CompanySettingsListView);
			lvd.Options = MmcListViewOptions.ExcludeScopeNodes;

			// Attach the view to the root node
			this.ViewDescriptions.Add(lvd);
			this.ViewDescriptions.DefaultIndex = 0;

			// disable actions + Images
			RefreshCompanyStatus();
		}

		private void LoadCompanyInfo()
		{
			this.DisplayName = this.CompanyInfo.Host;
			this.Tag = this.CompanyInfo.Id;
			this.IsActive = this.CompanyInfo.IsActive;
			this.IsEnableShedulerService = this.CompanyInfo.IsScheduleServiceEnabled;
			this.ApplicationPool = this.CompanyInfo.PortalPool;

			// Load Company SubItemDisplayNames 
			//this.SubItemDisplayNames.Add(""); // Created
			this.SubItemDisplayNames.Add(this.CompanyInfo.IsActive ? SnapInResources.Company_ActiveStatusName :
				SnapInResources.Company_InactiveStatusName); // Status
			this.SubItemDisplayNames.Add(this.CompanyInfo.Scheme); // http or https
			this.SubItemDisplayNames.Add(this.CompanyInfo.Host); // Host
			this.SubItemDisplayNames.Add(this.CompanyInfo.Port); // Port
			this.SubItemDisplayNames.Add(this.CompanyInfo.CodeVersion.ToString(CultureInfo.CurrentUICulture)); // Build
			this.SubItemDisplayNames.Add(this.CompanyInfo.Database);// DatabaseName
			//this.SubItemDisplayNames.Add(this.CompanyInfo.DatabaseVersion.ToString(CultureInfo.CurrentUICulture));// DatabaseVersion
			this.SubItemDisplayNames.Add(this.CompanyInfo.CodePath);// CodePath
			this.SubItemDisplayNames.Add(this.CompanyInfo.SiteId.ToString());// SiteId
			this.SubItemDisplayNames.Add(this.CompanyInfo.IMPool);// IMPool
			this.SubItemDisplayNames.Add(this.CompanyInfo.PortalPool);// PortalPool
			this.SubItemDisplayNames.Add(this.CompanyInfo.IsScheduleServiceEnabled?SnapInResources.SchedulerService_EnableStatusName:
				SnapInResources.SchedulerService_DisableStatusName);// IsScheduleServiceEnabled

			string defaultLanguage = string.Empty;
			if (this.CompanyInfo.DefaultLanguage != null && this.CompanyInfo.DefaultLanguage.FriendlyName != null)
				defaultLanguage = this.CompanyInfo.DefaultLanguage.FriendlyName;
			this.SubItemDisplayNames.Add(defaultLanguage);// Default Language

		}

		public void RefreshCompanyStatus()
		{
			if(this.ActionsPaneItems.Contains(this.EnableSchedulerServiceAction))
				this.ActionsPaneItems.Remove(this.EnableSchedulerServiceAction);
			if (this.ActionsPaneItems.Contains(this.DisableSchedulerServiceAction))
				this.ActionsPaneItems.Remove(this.DisableSchedulerServiceAction);


			if (this.IsEnableShedulerService)
			{
				this.SubItemDisplayNames[10] = SnapInResources.SchedulerService_EnableStatusName;
				this.ActionsPaneItems.Add(this.DisableSchedulerServiceAction);
			}
			else
			{
				this.SubItemDisplayNames[10] = SnapInResources.SchedulerService_DisableStatusName;
				this.ActionsPaneItems.Add(this.EnableSchedulerServiceAction);
			}

			if (this.IsActive)
			{
				this.SubItemDisplayNames[0] = SnapInResources.Company_ActiveStatusName;
				this.ImageIndex = this.IsEnableShedulerService?4:25;
				this.SelectedImageIndex = this.IsEnableShedulerService ? 4 : 25;
				
				this.StartAction.ImageIndex = 13;
				this.StartAction.Enabled = false;
				
				this.StopAction.ImageIndex = 11;
				this.StopAction.Enabled = true;
			}
			else
			{
				this.SubItemDisplayNames[0] = SnapInResources.Company_InactiveStatusName;
				this.ImageIndex = this.IsEnableShedulerService ? 15:26;
				this.SelectedImageIndex = this.IsEnableShedulerService ? 15 : 26; ;

				this.StartAction.ImageIndex = 10;
				this.StartAction.Enabled = true;
				
				this.StopAction.ImageIndex = 14;
				this.StopAction.Enabled = false;
			}

			if (CompanySettingsListView != null)
				CompanySettingsListView.Refresh();
		}
		#endregion


		#region Methods

		/// <summary>
		/// Gets the company configurator.
		/// </summary>
		/// <returns></returns>
		public IConfigurator GetCompanyConfigurator()
		{
			return ((ServerScopeNode)this.Parent.Parent).Configurator;
		}

		/// <summary>
		/// Called when the <see cref="F:Microsoft.ManagementConsole.StandardVerbs.Delete"></see> standard verb is triggered.
		/// </summary>
		/// <param name="status">An object that holds the status information.</param>
		protected override void OnDelete(SyncStatus status)
		{
			string companyId = this.CompanyInfo.Id;
			string domainName = this.DisplayName;

			DeleteCompanyForm form = new DeleteCompanyForm(GetCompanyConfigurator(), companyId, domainName);

			if (this.SnapIn.Console.ShowDialog(form) == DialogResult.OK)
			{
				status.ReportProgress(0, 0, string.Format(SnapInResources.CompanyScopeNode_Action_Delete_Progress, domainName));

				//Thread.Sleep(10000);
				GetCompanyConfigurator().DeleteCompany(companyId, form.DeleteDatabase);

				// Refresh ServerFormView
				ServerScopeNode serverScopeNode = ((ServerScopeNode)this.Parent.Parent);

				if (serverScopeNode.ServerFormView != null && serverScopeNode.ServerFormView.ServerFeaturesControl != null)
				{
					serverScopeNode.ServerFormView.ServerFeaturesControl.LoadDataFromConfigurator();
				}

				// Remove Current Element From Tree
				this.Parent.Children.Remove(this);
			}
		}

		protected override void OnRefresh(AsyncStatus status)
		{
			base.OnRefresh(status);

			try
			{
				// Refresh Company Info
				this.CompanyInfo = this.GetCompanyConfigurator().GetCompanyInfo((string)this.Tag);
				this.SubItemDisplayNames.Clear();

				LoadCompanyInfo();

				RefreshCompanyStatus();

				// Refresh CompanySettingsListView
				if (CompanySettingsListView != null)
					CompanySettingsListView.Refresh();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex, "CompanyScopeNode.Refresh");
			}
			
		}

		/// <summary>
		/// Called when an action is triggered for the node. Derived classes should override this method to provide application-specific handling of the action.
		/// </summary>
		/// <param name="action">The action that has been triggered.</param>
		/// <param name="status">The status object.</param>
		protected override void OnSyncAction(SyncAction action, SyncStatus status)
		{
			if (action == this.StartAction)
			{
				try
				{
					OmStartCompany(status);
				}
				catch (Exception ex)
				{
					ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
					this.SnapIn.Console.ShowDialog(exForm);
				}
			}
			else if (action == this.StopAction)
			{
				try
				{
					OnStopCompany(status);
				}
				catch (Exception ex)
				{
					ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
					this.SnapIn.Console.ShowDialog(exForm);
				}
			}
			else if (action == this.ChangeDomainAction)
			{
				try
				{
					OnChangeDomain(status);
				}
				catch (Exception ex)
				{
					ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
					this.SnapIn.Console.ShowDialog(exForm);
				}
			}
			else if (action == this.UpgradeAction)
			{
				try
				{
					OnUpgrade(status);
				}
				catch (Exception ex)
				{
					ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
					this.SnapIn.Console.ShowDialog(exForm);
				}
			}
			else if (action == this.BrowseAction)
			{
				try
				{
					OnBrowse(status);
				}
				catch (Exception ex)
				{
					ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
					this.SnapIn.Console.ShowDialog(exForm);
				}
			}
			else if (action == EnableSchedulerServiceAction)
			{
				try
				{
					OnEnableSchedulerService(status);
				}
				catch (Exception ex)
				{
					ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
					this.SnapIn.Console.ShowDialog(exForm);
				}
			}
			else if (action == DisableSchedulerServiceAction)
			{
				try
				{
					OnDisableSchedulerService(status);
				}
				catch (Exception ex)
				{
					ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
					this.SnapIn.Console.ShowDialog(exForm);
				}
			}
			else if (action == ChangePortalPool)
			{
				try
				{
					OnChangePortalPool(status);
				}
				catch (Exception ex)
				{
					ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
					this.SnapIn.Console.ShowDialog(exForm);
				}
			}
		}

		private void OnUpgrade(SyncStatus status)
		{
			IConfigurator configurator = GetCompanyConfigurator();

			// Check New Updates
			int codeVersion = this.CompanyInfo.CodeVersion;
			int[] availableUpdates = configurator.ListUpdates();

			bool bDetectNewUpdate = false;

			foreach (int availableUpdate in availableUpdates)
			{
				if (codeVersion < availableUpdate)
				{
					bDetectNewUpdate = true;
					break;
				}
			}

			if (bDetectNewUpdate)
			{
				// Show Select New Update
				UpdateCompanyForm form = new UpdateCompanyForm(configurator, this.CompanyInfo);

				if (DialogResult.Retry == this.SnapIn.Console.ShowDialog(form))
				{
					Process currentProcess = Process.GetCurrentProcess();

					// Start a new Process
					ProcessStartInfo start = configurator.BuildUpdateCommandForCommonComponents(form.GetUpdateId(), currentProcess.Id);

					// Run Update Common Components
					Process.Start(start);

					// Close current process
					currentProcess.CloseMainWindow();
				}
			}
			else
			{
				MessageBoxParameters msgBox = new MessageBoxParameters();
				msgBox.Icon = MessageBoxIcon.Information;
				msgBox.Caption = SnapInResources.SoftwareUpdate_Caption;
				msgBox.Buttons = MessageBoxButtons.OK;
				msgBox.Text = SnapInResources.SoftwareUpdateMsg_UnableFind;

				this.SnapIn.Console.ShowDialog(msgBox);
			}

			// TODO: Check Common Components Update
			//Process.GetCurrentProcess().CloseMainWindow();
		}

		private void OnChangePortalPool(SyncStatus status)
		{
			IConfigurator configurator = GetCompanyConfigurator();

			EditPortalPoolForm form = new EditPortalPoolForm(configurator, this.ApplicationPool);

			if (this.SnapIn.Console.ShowDialog(form) == DialogResult.OK)
			{
				status.ReportProgress(0, 0,
					string.Format(SnapInResources.CompanyScopeNode_Action_ChangePortalPool_Progress, this.CompanyInfo.Host));

				this.ApplicationPool = configurator.ChangeCompanyApplicationPool(this.CompanyInfo.Id, form.PortalPool);
				this.SubItemDisplayNames[9] = this.ApplicationPool;

				RefreshCompanyStatus();
			}
		}

		private void OnEnableSchedulerService(SyncStatus status)
		{
			GetCompanyConfigurator().EnableScheduleService(this.CompanyInfo.Id, true);

			this.IsEnableShedulerService = true;

			RefreshCompanyStatus();
		}

		private void OnDisableSchedulerService(SyncStatus status)
		{
			GetCompanyConfigurator().EnableScheduleService(this.CompanyInfo.Id, false);

			this.IsEnableShedulerService = false;

			RefreshCompanyStatus();
		}

		/// <summary>
		/// Called when [browse].
		/// </summary>
		/// <param name="status">The status.</param>
		private void OnBrowse(SyncStatus status)
		{
			int port = (string.IsNullOrEmpty(CompanyInfo.Port) ? -1 : int.Parse(CompanyInfo.Port, CultureInfo.InvariantCulture));
			Help.ShowHelp(null, new UriBuilder(CompanyInfo.Scheme, CompanyInfo.Host, port).ToString());
		}

		/// <summary>
		/// Called when [change domain].
		/// </summary>
		/// <param name="status">The status.</param>
		private void OnChangeDomain(SyncStatus status)
		{
			string companyId = this.CompanyInfo.Id;
			

			EditDefaultAddressForm form = new EditDefaultAddressForm(GetCompanyConfigurator(), companyId);

			if (this.SnapIn.Console.ShowDialog(form) == DialogResult.OK)
			{
				status.ReportProgress(0, 0, SnapInResources.CompanyScopeNode_Action_UpdateAddress_Progress);

				//Thread.Sleep(10000);
				GetCompanyConfigurator().ChangeCompanyAddress(companyId, form.NewSchema, form.NewHost, form.NewPort, true);

				// Refresh Element
				this.DisplayName = form.NewHost;

				this.SubItemDisplayNames[1] = form.NewSchema;
				this.SubItemDisplayNames[2] = form.NewHost;
				this.SubItemDisplayNames[3] = form.NewPort;
			}
		}

		/// <summary>
		/// Called when [stop company].
		/// </summary>
		/// <param name="status">The status.</param>
		public void OnStopCompany(SyncStatus status)
		{
			status.ReportProgress(0, 0, string.Format(SnapInResources.CompanyScopeNode_Action_Stop_Progress, this.DisplayName));

			//Thread.Sleep(10000);
			GetCompanyConfigurator().ActivateCompany(this.CompanyInfo.Id, false, true);

			this.IsActive = false;
			RefreshCompanyStatus();

		}

		/// <summary>
		/// Oms the start company.
		/// </summary>
		/// <param name="status">The status.</param>
		public void OmStartCompany(SyncStatus status)
		{
			status.ReportProgress(0, 0, string.Format(SnapInResources.CompanyScopeNode_Action_Start_Progress, this.DisplayName));

			//Thread.Sleep(10000);
			GetCompanyConfigurator().ActivateCompany(this.CompanyInfo.Id, true, true);

			this.IsActive = true;
			RefreshCompanyStatus();
		}

		#endregion

		
	}
}
