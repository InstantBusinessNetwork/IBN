using System;
using System.Diagnostics;
using System.Windows.Forms;

using Mediachase.Ibn.Configuration;
using Mediachase.Ibn.ConfigurationUI.Updates;
using Microsoft.ManagementConsole;
using Microsoft.ManagementConsole.Advanced;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents Server Form View.
	/// </summary>
	public class ServerFormView: FormView
	{
		#region Const
		#endregion

		#region Properties
		protected SyncAction EditSqlServerSettingsAction { get; private set; }
		protected SyncAction CreateAspAction { get; private set; }
		protected SyncAction DeleteAspAction { get; private set; }

		protected SyncAction UpdateCommonComponentsAction { get; private set; }
		protected SyncAction UpdateServerAction { get; private set; }

		protected SyncAction UpdateCheckAction { get; private set; }

		public ServerFeaturesControl ServerFeaturesControl { get; set; }
		#endregion


		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ServerFormView"/> class.
		/// </summary>
		public ServerFormView()
		{
			this.EditSqlServerSettingsAction = new SyncAction(SnapInResources.ServerFormView_Action_EditSqlServerSettings_Name,
				SnapInResources.ServerFormView_Action_EditSqlServerSettings_Description, 12);

			this.CreateAspAction = new SyncAction(SnapInResources.ServerFormView_Action_AspCreate_Name,
				SnapInResources.ServerFormView_Action_AspCreate_Description, 23);

			this.DeleteAspAction = new SyncAction(SnapInResources.ServerFormView_Action_AspDelete_Name,
				SnapInResources.ServerFormView_Action_AspDelete_Description, 24);

			this.UpdateCommonComponentsAction = new SyncAction(SnapInResources.ServerFormView_Action_UpdateCommonComponents_Name,
				SnapInResources.ServerFormView_Action_UpdateCommonComponents_Description, 7);

			this.UpdateServerAction = new SyncAction(SnapInResources.ServerFormView_Action_UpdateServer_Name,
				SnapInResources.ServerFormView_Action_UpdateServer_Description, 7);

			this.UpdateCheckAction = new SyncAction(SnapInResources.UpgradesScopeNode_Action_UpdateCheck_Name,
			SnapInResources.UpgradesScopeNode_Action_UpdateCheck_Description, 6);

#if RADIUS
			this.UpdateCheckAction.Enabled = false;
			this.UpdateCheckAction.ImageIndex = 30;
#endif
		}
		#endregion

		#region Methods
		/// <summary>
		/// Allows snap-in code to perform custom view initialization.
		/// </summary>
		/// <param name="status">The status object to update.</param>
		protected override void OnInitialize(AsyncStatus status)
		{
			// handle any basic stuff
			base.OnInitialize(status);

			this.ServerFeaturesControl = (ServerFeaturesControl)this.Control;
			((ServerScopeNode)this.ScopeNode).ServerFormView = this;

			this.ActionsPaneItems.Add(this.EditSqlServerSettingsAction);

			if (this.GetConfigurator().CanCreateAspSite())
			{
				this.ActionsPaneItems.Add(this.CreateAspAction);
			}

			if (this.GetConfigurator().CanDeleteAspSite())
			{
				this.ActionsPaneItems.Add(this.DeleteAspAction);
			}

			this.ActionsPaneItems.Add(new ActionSeparator());

			this.ActionsPaneItems.Add(this.UpdateServerAction);
			this.ActionsPaneItems.Add(this.UpdateCommonComponentsAction);
			this.ActionsPaneItems.Add(new ActionSeparator());
			this.ActionsPaneItems.Add(this.UpdateCheckAction); 
		}

		/// <summary>
		/// Handles the execution of a selection-independent (view-global) action that runs synchronous to MMC.
		/// </summary>
		/// <param name="action">The executed action.</param>
		/// <param name="status">The object that holds the status information.</param>
		protected override void OnSyncAction(SyncAction action, SyncStatus status)
		{
			base.OnSyncAction(action, status);

			if (action == this.EditSqlServerSettingsAction)
			{
				try
				{
					OnEditSqlServerSettingsAction(status);
					this.ServerFeaturesControl.ReloadData();
				}
				catch (Exception ex)
				{
					ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
					this.SnapIn.Console.ShowDialog(exForm);
				}
			}
			else if (action == this.CreateAspAction)
			{
				try
				{
					CreateAspSite(this.GetConfigurator(), this.SnapIn.Console, status);

					this.ServerFeaturesControl.ReloadData();
				}
				catch (Exception ex)
				{
					ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
					this.SnapIn.Console.ShowDialog(exForm);
				}
			}
			else if (action == this.DeleteAspAction)
			{
				try
				{
					OnDeleteAspAction(status);
					this.ServerFeaturesControl.ReloadData();
				}
				catch (Exception ex)
				{
					ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
					this.SnapIn.Console.ShowDialog(exForm);
				}
			}
			else if (action == UpdateCommonComponentsAction)
			{
				try
				{
					OnUpdateCommonComponentsAction(status);
					this.ServerFeaturesControl.ReloadData();
				}
				catch (Exception ex)
				{
					ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
					this.SnapIn.Console.ShowDialog(exForm);
				}
			}
			else if (action == UpdateServerAction)
			{
				try
				{
					OnUpdateServerAction(status);
					this.ServerFeaturesControl.ReloadData();
				}
				catch (Exception ex)
				{
					ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
					this.SnapIn.Console.ShowDialog(exForm);
				}
			}
			else if (action == UpdateCheckAction)
			{
				try
				{
					OnUpdateCheckAction(status);
					this.ServerFeaturesControl.ReloadData();
				}
				catch (Exception ex)
				{
					ThreadExceptionDialog exForm = new ThreadExceptionDialog(ex);
					this.SnapIn.Console.ShowDialog(exForm);
				}
			}
		}

		private void OnUpdateCheckAction(SyncStatus status)
		{
			UpdateCheckForm form = new UpdateCheckForm(this.GetConfigurator());
			this.SnapIn.Console.ShowDialog(form);
		}

		private void OnUpdateServerAction(SyncStatus status)
		{
			UpdateServerForm form = new UpdateServerForm(GetConfigurator());

			if (this.SnapIn.Console.ShowDialog(form) == DialogResult.OK)
			{
				Process currentProcess = Process.GetCurrentProcess();
				ProcessStartInfo processStart = GetConfigurator().BuildUpdateCommandForServer(form.GetUpdateId(), currentProcess.Id);
				Process.Start(processStart);
				currentProcess.CloseMainWindow();
			}
		}

		private void OnUpdateCommonComponentsAction(SyncStatus status)
		{
			// Check New Updates
			int codeVersion = this.GetConfigurator().CommonVersion;

			int[] availableUpdates = this.GetConfigurator().ListUpdates();

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
				UpdateCommonComponentsForm form = new UpdateCommonComponentsForm(GetConfigurator());

				if (this.SnapIn.Console.ShowDialog(form) == DialogResult.OK)
				{
					Process currentProcess = Process.GetCurrentProcess();
					ProcessStartInfo processStart = GetConfigurator().BuildUpdateCommandForCommonComponents(form.GetUpdateId(), currentProcess.Id);
					Process.Start(processStart);
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
		}

		private void OnDeleteAspAction(SyncStatus status)
		{
			DeleteAspForm form = new DeleteAspForm();

			if (this.SnapIn.Console.ShowDialog(form) == DialogResult.OK)
			{
				status.ReportProgress(0, 0, SnapInResources.ServerFormView_Action_AspDelete_Progress);

				this.GetConfigurator().DeleteAspSite(form.DeleteDatabase);
				//Thread.Sleep(10000);
			}
		}

		/// <summary>
		/// Gets the configurator.
		/// </summary>
		/// <returns></returns>
		protected IConfigurator GetConfigurator()
		{
			return this.ServerFeaturesControl.GetConfigurator();
		}

		internal static void CreateAspSite(IConfigurator configurator, Microsoft.ManagementConsole.Advanced.Console console, SyncStatus status)
		{
			// Check Sql Settings
			ISqlServerSettings sqlSettings = configurator.SqlSettings;
			bool sqlSettingsOK = !string.IsNullOrEmpty(sqlSettings.Server);

			if (!sqlSettingsOK)
			{
				// TODO: Show Warning
				EditSqlServerSettingsForm editSqlSettinnsForm = new EditSqlServerSettingsForm(configurator);

				if (console.ShowDialog(editSqlSettinnsForm) == DialogResult.OK)
				{
					configurator.ChangeSqlServerSettings(
					editSqlSettinnsForm.SqlServerName,
					editSqlSettinnsForm.UseWindowsAuth ? AuthenticationType.Windows : AuthenticationType.SqlServer,
					editSqlSettinnsForm.SqlServerUser,
					editSqlSettinnsForm.SqlServerPassword,
					editSqlSettinnsForm.IbnUserName,
					editSqlSettinnsForm.IbnUserPassword);

					sqlSettingsOK = true;
				}
			}

			if (sqlSettingsOK)
			{
				CreateAspForm createAspForm = new CreateAspForm(configurator);

				if (console.ShowDialog(createAspForm) == DialogResult.OK)
				{
					if (status != null)
						status.ReportProgress(0, 0, SnapInResources.ServerFormView_Action_AspCreate_Progress);

					configurator.CreateAspSite(createAspForm.textBoxHost.Text
						, createAspForm.IisIPAddress
						, int.Parse(createAspForm.textBoxIisPort.Text)
						, createAspForm.IisPool
						);
				}
			}
		}

		/// <summary>
		/// Called when [edit SQL server settings action].
		/// </summary>
		/// <param name="status">The status.</param>
		private void OnEditSqlServerSettingsAction(SyncStatus status)
		{
			this.ServerFeaturesControl.ShowEditSqlServerSettingsForm();
		}

		#endregion

		
	}
}
