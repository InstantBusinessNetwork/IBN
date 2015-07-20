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
using System.IO;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Represents update company form.
	/// </summary>
	public partial class UpdateCompanyForm : Form
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

		protected ICompanyInfo CompanyInfo { get; private set; }

		public bool UpdateCommonFile { get; set; }

		protected Process UpdateProcess { get; set; }

		protected string UpdateLogFileName { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateCompanyForm"/> class.
		/// </summary>
		/// <param name="serverConfigurator">The server configurator.</param>
		public UpdateCompanyForm(IConfigurator configurator, ICompanyInfo companyInfo)
		{
			InitializeComponent();

#if RADIUS
			this.Icon = SnapInResources.portal_RS;
#endif 

			this.Configurator = configurator;
			this.CompanyInfo = companyInfo;

			InitializePage1();
		}

		/// <summary>
		/// Initializes the page1.
		/// </summary>
		private void InitializePage1()
		{
			int codeVersion = this.CompanyInfo.CodeVersion;
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
				this.labelNextToStartUpdate.Visible = false;
				this.wizard.NextButtonText = SnapInResources.UpdateServerForm_InstallButton_Text;
			}
			else
			{
				this.UpdateCommonFile = false;
				this.panelPage1CommonFileUpdateWarning.Visible = false;
				this.labelNextToStartUpdate.Visible = true;
				this.wizard.NextButtonText = SnapInResources.UpdateServerForm_InstallButton_Text;
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
			asyncRead.BeginInvoke(GetUpdateId(), OnReadFromStandardOutputEnd, asyncRead);
		}

		/// <summary>
		/// Called when [read from standard output].
		/// </summary>
		/// <param name="process">The process.</param>
		protected void OnUpdateCompany(int versionId)
		{
			// Start Async Update progress
			ProcessStartInfo start = this.Configurator.BuildUpdateCommandForCompany(versionId, this.CompanyInfo.Id);

			try
			{
				//ProcessStartInfo start = new ProcessStartInfo(@"E:\Projects\tfs.mediachase.ru\Instant Business Network\4.8\Server\Mediachase.Ibn.ConfigurationUI.TestConsole\bin\Debug\Mediachase.Ibn.ConfigurationUI.TestConsole.exe", "/s");
				//ProcessStartInfo start = new ProcessStartInfo(@"E:\Projects\tfs.mediachase.ru\Instant Business Network\4.8\Server\Mediachase.Ibn.ConfigurationUI.TestConsole\bin\Debug\Mediachase.Ibn.ConfigurationUI.TestConsole.exe", "/e");

				start.RedirectStandardOutput = true;
				start.UseShellExecute = false;
				start.CreateNoWindow = true;

				this.UpdateProcess = Process.Start(start);

				string strUpdateLine;

				while ((strUpdateLine = this.UpdateProcess.StandardOutput.ReadLine()) != null)
				{
					AddLogRecord(strUpdateLine);
				}
			}
			catch (Exception ex)
			{
				AddLogRecord("========================================================");
				AddLogRecord(ex.ToString());
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
		protected void OnReadFromStandardOutputEnd(IAsyncResult ar)
		{
			((UpdateCompanyHandler)ar.AsyncState).EndInvoke(ar);

			if (this.UpdateProcess != null)
			{
				this.UpdateProcess.WaitForExit();

				int result = UpdateProcess.ExitCode;

				try
				{
					if (result != 0)
					{
						// Error
						this.Invoke(new UpdateCompanyResultHandler(OnCompanyUpdateError), result);
					}
					else
					{
						// All Is Ok
						this.Invoke(new UpdateCompanyResultHandler(OnCompanyUpdatedSuccessfully), result);
					}
				}
				catch (ObjectDisposedException)
				{
				}

				this.UpdateProcess = null;
			}
		}

		protected void OnCompanyUpdatedSuccessfully(int errorCode)
		{
			this.wizard.ActivatePage(wizardPage3Suceess);

			this.wizard.NextEnabled = true;
			this.wizard.BackEnabled = false;
			this.wizard.CancelEnabled = false;
		}

		protected void OnCompanyUpdateError(int errorCode)
		{
			this.wizard.NextEnabled = false;
			this.wizard.BackEnabled = true;

			MessageBox.Show(SnapInResources.SoftwareUpdateMsg_UpdateFailed,
			   SnapInResources.SoftwareUpdate_Caption,
			   MessageBoxButtons.OK, MessageBoxIcon.Error);
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

		private void linkViewLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (string.IsNullOrEmpty(UpdateLogFileName))
			{
				// Save Log to Temporary File Storage and
				UpdateLogFileName = Path.GetTempFileName() + ".txt";

				using (StreamWriter writer = new StreamWriter(UpdateLogFileName))
				{
					writer.Write(this.textBoxPage2Log.Text);
					writer.Flush();
					writer.Close();
				}
			}

			Process.Start(UpdateLogFileName);
		}
	}
}
