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
	/// Represents update company form.
	/// </summary>
	public partial class UpdateCompanyListForm : Form
	{
		/// <summary>
		/// 
		/// </summary>
		public delegate void UpdateCompanyHandler();
		/// <summary>
		/// 
		/// </summary>
		public delegate void UpdateCompanyResultHandler();
		/// <summary>
		/// 
		/// </summary>
		public delegate void AddLogRecordHandler(string line);

		/// <summary>
		/// Gets or sets the configurator.
		/// </summary>
		/// <value>The configurator.</value>
		protected IConfigurator Configurator { get; private set; }

		protected CompanyScopeNode[] Companies { get; private set; }

		public bool UpdateCommonFile { get; set; }

		protected Process UpdateProcess { get; set; }
		protected bool CancelUpdate { get; set; }

		protected List<CompanyScopeNode> FailedCompanies { get; set; }

		protected List<string> FailedCompaniesLogs { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateCompanyListForm"/> class.
		/// </summary>
		/// <param name="serverConfigurator">The server configurator.</param>
		public UpdateCompanyListForm(IConfigurator configurator, CompanyScopeNode[] companies)
		{
			InitializeComponent();

#if RADIUS
			this.Icon = SnapInResources.portal_RS;
#endif 

			this.Configurator = configurator;
			this.Companies = companies;
			this.CancelUpdate = false;

			this.FailedCompanies = new List<CompanyScopeNode>();
			this.FailedCompaniesLogs = new List<string>();

			InitializePage1();
		}

		/// <summary>
		/// Initializes the page1.
		/// </summary>
		private void InitializePage1()
		{
			int codeVersion = -1;
			int[] availableUpdates = this.Configurator.ListUpdates();

			foreach (int availableUpdate in availableUpdates)
			{
				if (availableUpdate > codeVersion)
				{
					int index = comboBoxAvailableUpdates.Items.Add(availableUpdate);
				}
			}

			comboBoxAvailableUpdates.SelectedIndex = comboBoxAvailableUpdates.Items.Count - 1;
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the comboBoxAvailableUpdates control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void comboBoxAvailableUpdates_SelectedIndexChanged(object sender, EventArgs e)
		{
			int updateId = GetUpdateId();

			if (this.Configurator.CheckIfCommonComponentsUpdateIsRequired(updateId))
			{
				this.UpdateCommonFile = true;
				this.panelPage1CommonFileUpdateWarning.Visible = true;
				this.wizard.NextButtonText = SnapInResources.UpdateServerForm_InstallButton_Text;
				this.labelNextToStartUpdate.Visible = false;
			}
			else
			{
				this.UpdateCommonFile = false;
				this.panelPage1CommonFileUpdateWarning.Visible = false;
				this.wizard.NextButtonText = SnapInResources.UpdateServerForm_InstallButton_Text;
				this.labelNextToStartUpdate.Visible = true;
			}
		}

		/// <summary>
		/// Gets the update id.
		/// </summary>
		/// <returns></returns>
		public int GetUpdateId()
		{
			return (int)comboBoxAvailableUpdates.SelectedItem;
		}

		/// <summary>
		/// Handles the CloseFromNext event of the wizardPage1Info control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="Mediachase.Ibn.ConfigurationUI.Wizard.PageEventArgs"/> instance containing the event data.</param>
		private void wizardPage1Info_CloseFromNext(object sender, Mediachase.Ibn.ConfigurationUI.Wizard.PageEventArgs e)
		{
			if (this.UpdateCommonFile)
			{
				this.DialogResult = DialogResult.Retry;
				this.Close();
			}
		}

		private void wizardPage2Update_ShowFromNext(object sender, EventArgs e)
		{
			this.wizard.NextEnabled = false;
			this.wizard.BackEnabled = false;

			UpdateCompanyHandler asyncRead = new UpdateCompanyHandler(OnUpdateCompany);
			asyncRead.BeginInvoke(OnUpdateCompanyEnd, asyncRead);
		}

		/// <summary>
		/// Called when [read from standard output].
		/// </summary>
		/// <param name="process">The process.</param>
		protected void OnUpdateCompany()
		{
			int index = 1;

			foreach (CompanyScopeNode company in this.Companies)
			{
				if (this.CancelUpdate)
					break;

				string companyId = (string)company.Tag;
				string companyName = company.DisplayName;

				try
				{
					// Start Async Update progress
					ProcessStartInfo start = this.Configurator.BuildUpdateCommandForCompany(GetUpdateId(), companyId);

					AddLogRecord(string.Format(SnapInResources.UpdateCompanyListForm_Start_Text,
						index, this.Companies.Length,
						string.Format(SnapInResources.CompanyScopeNode_Action_Upgrade_Progress, companyName)));

					// Test consoles
					//ProcessStartInfo start = new ProcessStartInfo(@"E:\Projects\tfs.mediachase.ru\Instant Business Network\4.8\Server\Mediachase.Ibn.ConfigurationUI.TestConsole\bin\Debug\Mediachase.Ibn.ConfigurationUI.TestConsole.exe", "/s");
					//ProcessStartInfo start = new ProcessStartInfo(@"E:\Projects\tfs.mediachase.ru\Instant Business Network\4.8\Server\Mediachase.Ibn.ConfigurationUI.TestConsole\bin\Debug\Mediachase.Ibn.ConfigurationUI.TestConsole.exe", "/e");

					start.RedirectStandardOutput = true;
					start.UseShellExecute = false;
					start.CreateNoWindow = true;

					this.UpdateProcess = Process.Start(start);

					StringBuilder sbLog = new StringBuilder();
					string strUpdateLine;

					while ((strUpdateLine = this.UpdateProcess.StandardOutput.ReadLine()) != null)
					{
						// TODO:
						sbLog.AppendLine(strUpdateLine);
					}

					if (this.UpdateProcess != null)
					{
						this.UpdateProcess.WaitForExit();

						if (this.UpdateProcess.ExitCode == 0)
						{
							// Success
							AddLogRecord(SnapInResources.UpdateCompanyListForm_OKResult_Text);
						}
						else
						{
							// Error
							AddLogRecord(string.Format(SnapInResources.UpdateCompanyListForm_ErrorResult_Text, this.UpdateProcess.ExitCode));

							this.FailedCompanies.Add(company);
							this.FailedCompaniesLogs.Add(sbLog.ToString());
						}
					}
				}
				catch (ObjectDisposedException)
				{
				}
				catch (Exception ex)
				{
					AddLogRecord("========================================================");
					AddLogRecord(ex.ToString());
				}

				index++;
			}
		}

		private void AddLogRecord(string strUpdateLine)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new AddLogRecordHandler(AddLogRecord), strUpdateLine);
				return;
			}

			this.textBoxPage2Log.SuspendLayout();

			this.textBoxPage2Log.Text += (strUpdateLine + Environment.NewLine);
			this.textBoxPage2Log.SelectionStart = this.textBoxPage2Log.Text.Length;
			this.textBoxPage2Log.ScrollToCaret();
			this.textBoxPage2Log.Update();

			this.textBoxPage2Log.ResumeLayout();
		}


		/// <summary>
		/// Called when [read from standard output end].
		/// </summary>
		/// <param name="ar">The ar.</param>
		protected void OnUpdateCompanyEnd(IAsyncResult ar)
		{
			((UpdateCompanyHandler)ar.AsyncState).EndInvoke(ar);

			this.Invoke(new UpdateCompanyResultHandler(OnCompanieUpdated));
		}

		protected void OnCompanieUpdated()
		{
			// Build wizardPage3Suceess Page 
			//for (int i = 0; i < 10; i++)
			{
				if (this.FailedCompanies.Count != 0)
				{
					pictureBoxUpdateResult.Image = SnapInResources.CompanyUpdate_Result_Warning;
					labelFinalResult.Text = SnapInResources.UpdateCompanyListForm_FinalWarning_Text;
					//listBoxFailedCompany.Visible = true;

					for(int index = 0; index<this.FailedCompanies.Count; index++ )
					{
						CompanyScopeNode company = this.FailedCompanies[index];

						string companyId = (string)company.Tag;
						string companyName = company.DisplayName;

						CompanyUpdateLogControl control = new CompanyUpdateLogControl();
						control.Init(companyName, companyId, this.FailedCompaniesLogs[index]);

						tableLayoutPanelUpdateLog.Controls.Add(control);
						//listBoxFailedCompany.Items.Add(string.Format("{0} ({1})", companyName, companyId));
					}
				} 
			}

			this.wizard.ActivatePage(wizardPage3Suceess);

			this.wizard.BackEnabled = false;
			this.wizard.NextEnabled = true;
			this.wizard.CancelEnabled = false;
		}

		private void wizard_CloseFromCancel(object sender, CancelEventArgs e)
		{
			Process updateProcess = this.UpdateProcess;

			if (updateProcess != null && MessageBox.Show(SnapInResources.SoftwareUpdateMsg_CancelUpdate, 
				SnapInResources.SoftwareUpdate_Caption, 
				MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
			{
				e.Cancel = true;
			}
		}

		private void UpdateCompanyForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				this.CancelUpdate = true;

				Process updateProcess = this.UpdateProcess;
				this.UpdateProcess = null;

				if (updateProcess != null)
				{
					updateProcess.Kill();
				}
			}
			catch 
			{
			}
		}
	}
}
