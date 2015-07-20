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
using System.Threading;
using System.Xml;

namespace Mediachase.Ibn.ConfigurationUI.Updates
{
	/// <summary>
	/// Represents update company form.
	/// </summary>
	public partial class UpdateCheckForm : Form
	{
		/// <summary>
		/// Gets or sets the configurator.
		/// </summary>
		/// <value>The configurator.</value>
		protected IConfigurator Configurator { get; private set; }

		protected string UpdateUrl { get; set; }
		protected string UpdateMoreInfoUrl { get; set; }

		public Process UnpackProcess { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateCheckForm"/> class.
		/// </summary>
		/// <param name="serverConfigurator">The server configurator.</param>
		public UpdateCheckForm(IConfigurator configurator)
		{
			InitializeComponent();

#if RADIUS
			this.Icon = SnapInResources.portal_RS;
#endif 
			this.Configurator = configurator;
		}

		private bool CheckNewUpdateFromFile(string filePath)
		{
			XmlDocument rssDoc = new XmlDocument();

			rssDoc.Load(filePath);

			int[] updates = this.Configurator.ListUpdates();

			string maxUpdateBuild = string.Empty;

			if (updates.Length > 0)
				maxUpdateBuild = updates[updates.Length - 1].ToString();

			string categoryName = "Server";

//#if DEBUG
//            categoryName = "ClientTools";
//#endif

			string itemXPath = string.Format("rss/channel/item[guid>'{0}' and category='{1}']",
				maxUpdateBuild, categoryName);

			foreach (XmlNode xmlNode in rssDoc.SelectNodes(itemXPath))
			{
				XmlNode title = xmlNode.SelectSingleNode("title");
				XmlNode comment = xmlNode.SelectSingleNode("comment");
				XmlNode link = xmlNode.SelectSingleNode("link");

				if (title == null || link==null)
					continue;

				labelRssUpdateTitle.Text = title.InnerText;
				this.UpdateUrl = link.InnerText;

				if (comment == null)
				{
					linkLabelViewUpdateMoreInfo.Visible = false;
				}
				else
				{
					this.UpdateMoreInfoUrl = comment.InnerText;
				}


				return true;
			}

			return false;
		}

		#region rssDownloader Events
		private void rssDownloader_ConnectingToServer(object sender, EventArgs e)
		{
			labelUpdateCheckProgressText.Text = SnapInResources.UpdateCheckForm_Status_ConnectingToServer;
		}

		private void rssDownloader_Completed(object sender, EventArgs e)
		{
			labelUpdateCheckProgressText.Text = SnapInResources.UpdateCheckForm_Status_Completed;

			try
			{
				if (CheckNewUpdateFromFile(rssDownloader.OutputFilePath))
				{
					wizard.ActivatePage(wizardPage3UpdateAvailable);
					wizard.NextButtonText = SnapInResources.Wizard_DownloadButton;
					wizard.NextEnabled = true;
					wizard.BackEnabled = false;
				}
				else
				{
					wizard.ActivatePage(wizardPage2NoUpdateFound);
					wizard.NextButtonText = SnapInResources.Wizard_FinishButton;
					wizard.NextEnabled = true;
					wizard.BackEnabled = false;
					wizard.CancelEnabled = false;
				}
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}

		private void rssDownloader_Downloading(object sender, DownloadProgressEventArgs e)
		{
			labelUpdateCheckProgressText.Text = SnapInResources.UpdateCheckForm_Status_Downloading;// "Downloading ...";

			progressBarRssDownload.Value = e.Progress;
		}

		private void rssDownloader_Failed(object sender, EventArgs e)
		{
			labelUpdateCheckProgressText.Text = SnapInResources.UpdateCheckForm_Status_Failed;

			ShowError(rssDownloader.Error);
		}
		
		#endregion

		private void UpdateCheckForm_Load(object sender, EventArgs e)
		{
			StartUpdateCheckProcess();
		}

		private void StartUpdateCheckProcess()
		{
			this.wizard.NextEnabled = false;
			this.wizard.BackEnabled = false;
			progressBarRssDownload.Value = 0;

			try
			{
				rssDownloader.OutputFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + ".xml");
				string rssUri = "http://update.mediachase.ru/Public/GetRss.aspx";

				string edition = GetEdition();
				if (!string.IsNullOrEmpty(edition))
					rssUri += "?edition=" + System.Web.HttpUtility.UrlEncode(edition);

				rssDownloader.RequestUri = new Uri(rssUri);
				rssDownloader.BeginDownload();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
				ShowError(ex);
			}
		}

		private string GetEdition()
		{
			string result = null;

			foreach(IConfigurationParameter param in this.Configurator.ListServerProperties())
			{
				if(param.Name=="ServerEdition")
					result = param.Value;
			}

			return result;
		}

		private void wizardPage1Checking_ShowFromNext(object sender, EventArgs e)
		{

		}

		private void wizardPage2NoUpdateFound_CloseFromNext(object sender, Mediachase.Ibn.ConfigurationUI.Wizard.PageEventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void wizardPage4DownloadingUpdate_ShowFromNext(object sender, EventArgs e)
		{
			this.wizard.NextEnabled = false;
			this.wizard.BackEnabled = false;
			progressBarUpdateDownloading.Value = 0;

			try
			{
				updateDownloader.RequestUri = new Uri(this.UpdateUrl);
				updateDownloader.OutputFilePath = Path.Combine(Path.GetTempPath(),
					Path.GetTempFileName() + ".exe");

				updateDownloader.BeginDownload();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
				ShowError(ex);
			}
		}

		#region updateDownloader events
		/// <summary>
		/// Handles the Completed event of the updateDownloader control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void updateDownloader_Completed(object sender, EventArgs e)
		{
			try
			{
				labelDownloadingProgressText.Text = SnapInResources.UpdateCheckForm_Status_UnpackUpdate;

				this.UnpackProcess = new Process();

				this.UnpackProcess.StartInfo.FileName = updateDownloader.OutputFilePath;
				this.UnpackProcess.EnableRaisingEvents = true;

				this.UnpackProcess.Exited += new EventHandler(unpackProcess_Exited);

				this.UnpackProcess.Start();
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}

		/// <summary>
		/// Handles the Exited event of the unpackProcess control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void unpackProcess_Exited(object sender, EventArgs e)
		{
			try
			{
				if (this.UnpackProcess.ExitCode != 0)
				{
					// TODO: Check Exit Code
				}

				labelDownloadingProgressText.Text = SnapInResources.UpdateCheckForm_Status_Completed;

				wizard.ActivatePage(wizardPage5Finish);

				wizard.NextButtonText = SnapInResources.Wizard_FinishButton;
				wizard.NextEnabled = true;
				wizard.BackEnabled = false;
				wizard.CancelEnabled = false;
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}

			this.UnpackProcess = null;
		}

		private void updateDownloader_ConnectingToServer(object sender, EventArgs e)
		{
			labelDownloadingProgressText.Text = SnapInResources.UpdateCheckForm_Status_ConnectingToServer;
		}

		private void updateDownloader_Downloading(object sender, DownloadProgressEventArgs e)
		{
			labelDownloadingProgressText.Text = SnapInResources.UpdateCheckForm_Status_Downloading;// "Downloading ...";

			progressBarUpdateDownloading.Value = e.Progress;
		}

		private void updateDownloader_Failed(object sender, EventArgs e)
		{
			labelDownloadingProgressText.Text = SnapInResources.UpdateCheckForm_Status_Failed;// "Downloading ...";

			ShowError(updateDownloader.Error);
		} 
		#endregion

		/// <summary>
		/// Shows the error.
		/// </summary>
		/// <param name="ex">The ex.</param>
		protected void ShowError(Exception ex)
		{
			textBoxErrorMessage.Text = string.Format(SnapInResources.UpdateCheckFrom_ErrorMessage_TextFormat,
				ex.Message, ex.ToString());

			wizard.ActivatePage(wizardPageFailed);
			wizard.BackEnabled = false;
			wizard.NextEnabled = true;
			wizard.CancelEnabled = true;
		}

		/// <summary>
		/// Handles the LinkClicked event of the linkLabelViewUpdateMoreInfo control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
		private void linkLabelViewUpdateMoreInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Help.ShowHelp(null, this.UpdateMoreInfoUrl);
		}

		private void wizardPageFailed_CloseFromNext(object sender, Mediachase.Ibn.ConfigurationUI.Wizard.PageEventArgs e)
		{
			this.wizard.ActivatePage(wizardPage1Checking);

			StartUpdateCheckProcess();
		}

		private void UpdateCheckForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.UnpackProcess != null)
			{
				e.Cancel = true;

				return;
			}

			if (this.wizard.Page == wizardPage1Checking ||
				this.wizard.Page == wizardPage4DownloadingUpdate)
			{
				if (MessageBox.Show(SnapInResources.SoftwareUpdateMsg_CancelUpdate,
					SnapInResources.SoftwareUpdate_Caption,
					MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
				{
					e.Cancel = true;
				}
				else
				{
					rssDownloader.Abort();
					updateDownloader.Abort();
				}
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (msg.WParam.ToInt32() == (int)Keys.Escape)
			{
				this.DialogResult = DialogResult.Cancel;
				this.Close();
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
