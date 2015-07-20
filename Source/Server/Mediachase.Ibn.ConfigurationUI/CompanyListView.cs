using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ManagementConsole;
using System.Threading;
using Mediachase.Ibn.Configuration;
using System.Windows.Forms;
using System.Diagnostics;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents company list view.
	/// </summary>
	public class CompanyListView : MmcListView
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="CompanyListView"/> class.
		/// </summary>
		public CompanyListView()
		{
		}
		#endregion

		#region Properties
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
		/// Gets or sets the upgrade.
		/// </summary>
		/// <value>The upgrade.</value>
		protected SyncAction UpgradeAction { get; private set; }

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
		/// Gets or sets the change portal pool.
		/// </summary>
		/// <value>The change portal pool.</value>
		protected SyncAction ChangePortalPool { get; private set; }

		#endregion

		#region Methods
		/// <summary>
		/// Allows snap-in code to perform custom view initialization.
		/// </summary>
		/// <param name="status">The status object to update.</param>
		protected override void OnInitialize(AsyncStatus status)
		{
			base.OnInitialize(status);

			// Create a set of columns for use in the list view
			// Define the default column title
			this.Columns[0].Title = SnapInResources.CompanyListView_Column_Name;
			this.Columns[0].SetWidth(200);

			// Add detail column
			//this.Columns.Add(new MmcListViewColumn(SnapInResources.CompanyListView_Column_Created, 100));
			this.Columns.Add(new MmcListViewColumn(SnapInResources.CompanyListView_Column_Status, 75));
			this.Columns.Add(new MmcListViewColumn(SnapInResources.CompanyListView_Column_Ssl, 50));
			this.Columns.Add(new MmcListViewColumn(SnapInResources.CompanyListView_Column_PrimaryDomain, 150));
			this.Columns.Add(new MmcListViewColumn(SnapInResources.CompanyListView_Column_Port, 50));
			this.Columns.Add(new MmcListViewColumn(SnapInResources.CompanyListView_Column_Build, 50));

			this.Columns.Add(new MmcListViewColumn(SnapInResources.CompanyListView_Column_DatabaseName, 150));
			//this.Columns.Add(new MmcListViewColumn(SnapInResources.CompanyListView_Column_DatabaseVersion, 50));

			this.Columns.Add(new MmcListViewColumn(SnapInResources.CompanyListView_Column_CodePath, 150));
			this.Columns.Add(new MmcListViewColumn(SnapInResources.CompanyListView_Column_SiteId, 100));
			this.Columns.Add(new MmcListViewColumn(SnapInResources.CompanyListView_Column_IMPool, 150));
			this.Columns.Add(new MmcListViewColumn(SnapInResources.CompanyListView_Column_PortalPool, 150));

			this.Columns.Add(new MmcListViewColumn(SnapInResources.CompanyListView_Column_SchedulerServiceStatus, 75));

			this.Columns.Add(new MmcListViewColumn(SnapInResources.CompanyListView_Column_DefaultLanguage, 75));

			// Set to show all columns
			this.Mode = MmcListViewMode.Report;  // default (set for clarity)

			// Set to show refresh as an option
			//this.SelectionData.EnabledStandardVerbs = StandardVerbs.Refresh;

			// Multi Company Actions
			this.StartAction = new SyncAction(SnapInResources.CompanyScopeNode_Action_Start_Name,
				SnapInResources.CompanyScopeNode_Action_Start_Description, 10);
			this.StopAction = new SyncAction(SnapInResources.CompanyScopeNode_Action_Stop_Name,
				SnapInResources.CompanyScopeNode_Action_Stop_Description, 11);
			this.UpgradeAction = new SyncAction(SnapInResources.CompanyScopeNode_Action_Upgrade_Name,
							SnapInResources.CompanyScopeNode_Action_Upgrade_Description, 6);

			this.EnableSchedulerServiceAction = new SyncAction(SnapInResources.CompanyScopeNode_Action_EnableSchedulerService_Name,
				SnapInResources.CompanyScopeNode_Action_EnableSchedulerService_Description, 27);
			this.DisableSchedulerServiceAction = new SyncAction(SnapInResources.CompanyScopeNode_Action_DisableSchedulerService_Name,
				SnapInResources.CompanyScopeNode_Action_DisableSchedulerService_Description, 28);

			this.ChangePortalPool = new SyncAction(SnapInResources.CompanyScopeNode_Action_ChangePortalPool_Name,
				SnapInResources.CompanyScopeNode_Action_ChangePortalPool_Description, 29);
		}

		protected override void OnSelectionChanged(SyncStatus status)
		{
			if (this.SelectedNodes.Count == 0)
			{
				// No items are selected; clear selection data and associated actions.
				SelectionData.Clear();
				SelectionData.ActionsPaneItems.Clear();
			}
			else
			{
				List<CompanyScopeNode> selectedCompanyIds = new List<CompanyScopeNode>();

				bool activeCompanyEnable = false;
				bool inactiveCompanyEnable = false;

				bool activeSchedulerEnable = false;
				bool inactiveSchedulerEnable = false;

				foreach (CompanyScopeNode node in this.SelectedNodes)
				{
					if (node.IsActive)
						activeCompanyEnable = true;
					else
						inactiveCompanyEnable = true;

					if (node.IsEnableShedulerService)
						activeSchedulerEnable = true;
					else
						inactiveSchedulerEnable = true;

					selectedCompanyIds.Add(node);
				}

				if (status != null)
				{
					SelectionData.Update(selectedCompanyIds.ToArray(), this.SelectedNodes.Count > 1, null, null);
				}

				this.SelectionData.ActionsPaneItems.Clear();

				this.SelectionData.ActionsPaneItems.Add(this.StartAction);
				this.SelectionData.ActionsPaneItems.Add(this.StopAction);

				this.SelectionData.ActionsPaneItems.Add(new ActionSeparator());

				this.SelectionData.ActionsPaneItems.Add(this.ChangePortalPool);
				this.SelectionData.ActionsPaneItems.Add(this.UpgradeAction);

				this.SelectionData.ActionsPaneItems.Add(new ActionSeparator());

				// Enable Disable Company List
				if (inactiveCompanyEnable)
				{
					this.StartAction.Enabled = true;
					this.StartAction.ImageIndex = 10;
				}
				else
				{
					this.StartAction.Enabled = false;
					this.StartAction.ImageIndex = 13;
				}

				if (activeCompanyEnable)
				{
					this.StopAction.Enabled = true;
					this.StopAction.ImageIndex = 11;
				}
				else
				{
					this.StopAction.Enabled = false;
					this.StopAction.ImageIndex = 14;
				}

				// Scheduler Service
				if (inactiveSchedulerEnable)
				{
					this.SelectionData.ActionsPaneItems.Add(this.EnableSchedulerServiceAction);

				}
				if (activeSchedulerEnable)
				{
					this.SelectionData.ActionsPaneItems.Add(this.DisableSchedulerServiceAction);
				}

				//this.UpgradeAction.Enabled = false;
			}
		}

		protected override void OnRefresh(AsyncStatus status)
		{
			// TODO: Not implemented yet
		}

		protected override void OnSyncSelectionAction(SyncAction action, SyncStatus status)
		{
			status.CanCancel = true;

			string newPortalPoolName = string.Empty;
			int versionId = 0;

			// Request Information
			if (action == this.ChangePortalPool)
			{
				EditPortalPoolForm form = new EditPortalPoolForm(GetConfigurator(), SnapInResources.CompanyScopeNode_Action_ChangePortalPool_Select);

				if (this.SnapIn.Console.ShowDialog(form) != DialogResult.OK || 
					string.IsNullOrEmpty(form.PortalPool))
					return;

				newPortalPoolName = form.PortalPool;

			}
			else if (action == this.UpgradeAction)
			{
				CompanyScopeNode[] upgradeCompanyItems = (CompanyScopeNode[])this.SelectionData.SelectionObject;

				UpdateCompanyListForm form = new UpdateCompanyListForm(GetConfigurator(), upgradeCompanyItems);

				if (DialogResult.Retry == this.SnapIn.Console.ShowDialog(form))
				{
					Process currentProcess = Process.GetCurrentProcess();

					// Start a new Process
					ProcessStartInfo start = GetConfigurator().BuildUpdateCommandForCommonComponents(form.GetUpdateId(), currentProcess.Id);

					// Run Update Common Components
					Process.Start(start);

					// Close current process
					currentProcess.CloseMainWindow();
				}

				return;
			}

			// Read Selected nodes
			CompanyScopeNode[] companyItems = (CompanyScopeNode[])this.SelectionData.SelectionObject;

			int workProcessed = 0;
			int totalWork = companyItems.Length;

			// ReportProgress 0, Len
			status.ReportProgress(workProcessed, totalWork, string.Empty);

			foreach (CompanyScopeNode company in companyItems)
			{
				string companyId = (string)company.Tag;
				string companyName = company.DisplayName;

				if (action == this.StartAction)
				{
					// ReportProgress Item, Len
					status.ReportProgress(workProcessed, totalWork,
						string.Format(SnapInResources.CompanyScopeNode_Action_Start_Progress, companyName));

					OnStartCompany(companyId);

					company.IsActive = true;
				}
				else if (action == this.StopAction)
				{
					// ReportProgress Item, Len
					status.ReportProgress(workProcessed, totalWork,
						string.Format(SnapInResources.CompanyScopeNode_Action_Stop_Progress, companyName));

					OnStopCompany(companyId);

					company.IsActive = false;
				}
				else if (action == this.EnableSchedulerServiceAction)
				{
					status.ReportProgress(workProcessed, totalWork,
						string.Format(SnapInResources.CompanyScopeNode_Action_EnableSchedulerService_Progress, companyName));

					OnEnableSchedulerService(companyId);

					company.IsEnableShedulerService = true;
				}
				else if (action == this.DisableSchedulerServiceAction)
				{
					status.ReportProgress(workProcessed, totalWork,
						string.Format(SnapInResources.CompanyScopeNode_Action_DisableSchedulerService_Progress, companyName));

					OnDisableSchedulerService(companyId);

					company.IsEnableShedulerService = false;
				}
				else if (action == this.ChangePortalPool)
				{
					status.ReportProgress(workProcessed, totalWork,
						string.Format(SnapInResources.CompanyScopeNode_Action_ChangePortalPool_Progress, companyName));

					company.SubItemDisplayNames[9] = OnChangePortalPool(companyId, newPortalPoolName);
				}
				else if (action == this.UpgradeAction)
				{
					status.ReportProgress(workProcessed, totalWork,
						string.Format(SnapInResources.CompanyScopeNode_Action_Upgrade_Progress, companyName));

					OnUpgradeAction(companyId, versionId);
				}

				company.RefreshCompanyStatus();

				// Process Cancel
				if (status.IsCancelSignaled)
					break;

				// Increase Progress
				workProcessed++;
			}

			// ReportProgress Len, Len
			status.ReportProgress(totalWork, totalWork, string.Empty);

			// Refresh Selection Button
			OnSelectionChanged(null);
		}

		private void OnUpgradeAction(string companyId, int versionId)
		{
			// TODO: Not Implemented yet
		}

		private string OnChangePortalPool(string companyId, string newPortalPool)
		{
			//Thread.Sleep(10000);
			IConfigurator configurator = GetConfigurator();
			return configurator.ChangeCompanyApplicationPool(companyId, newPortalPool);
		}

		private void OnDisableSchedulerService(string companyId)
		{
			IConfigurator configurator = GetConfigurator();

			configurator.EnableScheduleService(companyId, false);
		}

		/// <summary>
		/// Gets the configurator.
		/// </summary>
		/// <returns></returns>
		private IConfigurator GetConfigurator()
		{
			return ((CompaniesScopeNode)this.ScopeNode).Configurator;
		}

		/// <summary>
		/// Called when [enable scheduler service].
		/// </summary>
		/// <param name="companyId">The company id.</param>
		private void OnEnableSchedulerService(string companyId)
		{
			IConfigurator configurator = GetConfigurator();

			configurator.EnableScheduleService(companyId, true);
		}

		/// <summary>
		/// Stops the company.
		/// </summary>
		/// <param name="companyId">The company id.</param>
		private void OnStopCompany(string companyId)
		{
			IConfigurator configurator = GetConfigurator();

			configurator.ActivateCompany(companyId, false, true);
		}

		/// <summary>
		/// Starts the company.
		/// </summary>
		/// <param name="companyId">The company id.</param>
		private void OnStartCompany(string companyId)
		{
			IConfigurator configurator = GetConfigurator();

			configurator.ActivateCompany(companyId, true, true);
		}

		#endregion
	}
}
