namespace Mediachase.Ibn.ConfigurationUI.Updates
{
	partial class UpdateCheckForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateCheckForm));
			this.wizard = new Mediachase.Ibn.ConfigurationUI.Wizard.Wizard();
			this.wizardPage1Checking = new Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage();
			this.pictureBoxCheckProgress = new System.Windows.Forms.PictureBox();
			this.labelUpdateCheckProgressText = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.progressBarRssDownload = new System.Windows.Forms.ProgressBar();
			this.headerPage1 = new Mediachase.Ibn.ConfigurationUI.Wizard.Header();
			this.wizardPage5Finish = new Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage();
			this.label8 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.header1 = new Mediachase.Ibn.ConfigurationUI.Wizard.Header();
			this.wizardPage4DownloadingUpdate = new Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage();
			this.pictureBoxDownloadProgress = new System.Windows.Forms.PictureBox();
			this.labelDownloadingProgressText = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.progressBarUpdateDownloading = new System.Windows.Forms.ProgressBar();
			this.headerPage4 = new Mediachase.Ibn.ConfigurationUI.Wizard.Header();
			this.wizardPage3UpdateAvailable = new Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage();
			this.linkLabelViewUpdateMoreInfo = new System.Windows.Forms.LinkLabel();
			this.label7 = new System.Windows.Forms.Label();
			this.labelRssUpdateTitle = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.headerPag3 = new Mediachase.Ibn.ConfigurationUI.Wizard.Header();
			this.wizardPageFailed = new Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage();
			this.textBoxErrorMessage = new System.Windows.Forms.TextBox();
			this.headerPageFailed = new Mediachase.Ibn.ConfigurationUI.Wizard.Header();
			this.wizardPage2NoUpdateFound = new Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.headerPage2 = new Mediachase.Ibn.ConfigurationUI.Wizard.Header();
			this.rssDownloader = new Mediachase.Ibn.ConfigurationUI.Updates.HttpStreamDownloader(this.components);
			this.updateDownloader = new Mediachase.Ibn.ConfigurationUI.Updates.HttpStreamDownloader(this.components);
			this.wizard.SuspendLayout();
			this.wizardPage1Checking.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheckProgress)).BeginInit();
			this.wizardPage5Finish.SuspendLayout();
			this.wizardPage4DownloadingUpdate.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxDownloadProgress)).BeginInit();
			this.wizardPage3UpdateAvailable.SuspendLayout();
			this.wizardPageFailed.SuspendLayout();
			this.wizardPage2NoUpdateFound.SuspendLayout();
			this.SuspendLayout();
			// 
			// wizard
			// 
			this.wizard.Controls.Add(this.wizardPage1Checking);
			this.wizard.Controls.Add(this.wizardPage5Finish);
			this.wizard.Controls.Add(this.wizardPage4DownloadingUpdate);
			this.wizard.Controls.Add(this.wizardPage3UpdateAvailable);
			this.wizard.Controls.Add(this.wizardPageFailed);
			this.wizard.Controls.Add(this.wizardPage2NoUpdateFound);
			resources.ApplyResources(this.wizard, "wizard");
			this.wizard.Name = "wizard";
			this.wizard.Pages.AddRange(new Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage[] {
            this.wizardPage1Checking,
            this.wizardPage2NoUpdateFound,
            this.wizardPageFailed,
            this.wizardPage3UpdateAvailable,
            this.wizardPage4DownloadingUpdate,
            this.wizardPage5Finish});
			// 
			// wizardPage1Checking
			// 
			this.wizardPage1Checking.Controls.Add(this.pictureBoxCheckProgress);
			this.wizardPage1Checking.Controls.Add(this.labelUpdateCheckProgressText);
			this.wizardPage1Checking.Controls.Add(this.label1);
			this.wizardPage1Checking.Controls.Add(this.progressBarRssDownload);
			this.wizardPage1Checking.Controls.Add(this.headerPage1);
			resources.ApplyResources(this.wizardPage1Checking, "wizardPage1Checking");
			this.wizardPage1Checking.IsFinishPage = false;
			this.wizardPage1Checking.Name = "wizardPage1Checking";
			this.wizardPage1Checking.ShowFromNext += new System.EventHandler(this.wizardPage1Checking_ShowFromNext);
			// 
			// pictureBoxCheckProgress
			// 
			this.pictureBoxCheckProgress.BackColor = System.Drawing.Color.Transparent;
			this.pictureBoxCheckProgress.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.UpdateCheckForm_Progress;
			resources.ApplyResources(this.pictureBoxCheckProgress, "pictureBoxCheckProgress");
			this.pictureBoxCheckProgress.Name = "pictureBoxCheckProgress";
			this.pictureBoxCheckProgress.TabStop = false;
			// 
			// labelUpdateCheckProgressText
			// 
			resources.ApplyResources(this.labelUpdateCheckProgressText, "labelUpdateCheckProgressText");
			this.labelUpdateCheckProgressText.Name = "labelUpdateCheckProgressText";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// progressBarRssDownload
			// 
			resources.ApplyResources(this.progressBarRssDownload, "progressBarRssDownload");
			this.progressBarRssDownload.Name = "progressBarRssDownload";
			// 
			// headerPage1
			// 
			this.headerPage1.BackColor = System.Drawing.SystemColors.Control;
			this.headerPage1.CausesValidation = false;
			resources.ApplyResources(this.headerPage1, "headerPage1");
			this.headerPage1.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.ScopeNodeImage_Upgrades;
			this.headerPage1.Name = "headerPage1";
			// 
			// wizardPage5Finish
			// 
			this.wizardPage5Finish.Controls.Add(this.label8);
			this.wizardPage5Finish.Controls.Add(this.label3);
			this.wizardPage5Finish.Controls.Add(this.header1);
			resources.ApplyResources(this.wizardPage5Finish, "wizardPage5Finish");
			this.wizardPage5Finish.IsFinishPage = false;
			this.wizardPage5Finish.Name = "wizardPage5Finish";
			// 
			// label8
			// 
			resources.ApplyResources(this.label8, "label8");
			this.label8.Name = "label8";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// header1
			// 
			this.header1.BackColor = System.Drawing.SystemColors.Control;
			this.header1.CausesValidation = false;
			resources.ApplyResources(this.header1, "header1");
			this.header1.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.ScopeNodeImage_Upgrades;
			this.header1.Name = "header1";
			// 
			// wizardPage4DownloadingUpdate
			// 
			this.wizardPage4DownloadingUpdate.Controls.Add(this.pictureBoxDownloadProgress);
			this.wizardPage4DownloadingUpdate.Controls.Add(this.labelDownloadingProgressText);
			this.wizardPage4DownloadingUpdate.Controls.Add(this.label6);
			this.wizardPage4DownloadingUpdate.Controls.Add(this.progressBarUpdateDownloading);
			this.wizardPage4DownloadingUpdate.Controls.Add(this.headerPage4);
			resources.ApplyResources(this.wizardPage4DownloadingUpdate, "wizardPage4DownloadingUpdate");
			this.wizardPage4DownloadingUpdate.IsFinishPage = false;
			this.wizardPage4DownloadingUpdate.Name = "wizardPage4DownloadingUpdate";
			this.wizardPage4DownloadingUpdate.ShowFromNext += new System.EventHandler(this.wizardPage4DownloadingUpdate_ShowFromNext);
			// 
			// pictureBoxDownloadProgress
			// 
			this.pictureBoxDownloadProgress.BackColor = System.Drawing.Color.Transparent;
			this.pictureBoxDownloadProgress.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.UpdateCheckForm_Progress;
			resources.ApplyResources(this.pictureBoxDownloadProgress, "pictureBoxDownloadProgress");
			this.pictureBoxDownloadProgress.Name = "pictureBoxDownloadProgress";
			this.pictureBoxDownloadProgress.TabStop = false;
			// 
			// labelDownloadingProgressText
			// 
			resources.ApplyResources(this.labelDownloadingProgressText, "labelDownloadingProgressText");
			this.labelDownloadingProgressText.Name = "labelDownloadingProgressText";
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// progressBarUpdateDownloading
			// 
			resources.ApplyResources(this.progressBarUpdateDownloading, "progressBarUpdateDownloading");
			this.progressBarUpdateDownloading.Name = "progressBarUpdateDownloading";
			// 
			// headerPage4
			// 
			this.headerPage4.BackColor = System.Drawing.SystemColors.Control;
			this.headerPage4.CausesValidation = false;
			resources.ApplyResources(this.headerPage4, "headerPage4");
			this.headerPage4.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.ScopeNodeImage_Upgrades;
			this.headerPage4.Name = "headerPage4";
			// 
			// wizardPage3UpdateAvailable
			// 
			this.wizardPage3UpdateAvailable.Controls.Add(this.linkLabelViewUpdateMoreInfo);
			this.wizardPage3UpdateAvailable.Controls.Add(this.label7);
			this.wizardPage3UpdateAvailable.Controls.Add(this.labelRssUpdateTitle);
			this.wizardPage3UpdateAvailable.Controls.Add(this.label5);
			this.wizardPage3UpdateAvailable.Controls.Add(this.headerPag3);
			resources.ApplyResources(this.wizardPage3UpdateAvailable, "wizardPage3UpdateAvailable");
			this.wizardPage3UpdateAvailable.IsFinishPage = false;
			this.wizardPage3UpdateAvailable.Name = "wizardPage3UpdateAvailable";
			// 
			// linkLabelViewUpdateMoreInfo
			// 
			resources.ApplyResources(this.linkLabelViewUpdateMoreInfo, "linkLabelViewUpdateMoreInfo");
			this.linkLabelViewUpdateMoreInfo.Name = "linkLabelViewUpdateMoreInfo";
			this.linkLabelViewUpdateMoreInfo.TabStop = true;
			this.linkLabelViewUpdateMoreInfo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelViewUpdateMoreInfo_LinkClicked);
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
			// 
			// labelRssUpdateTitle
			// 
			resources.ApplyResources(this.labelRssUpdateTitle, "labelRssUpdateTitle");
			this.labelRssUpdateTitle.Name = "labelRssUpdateTitle";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// headerPag3
			// 
			this.headerPag3.BackColor = System.Drawing.SystemColors.Control;
			this.headerPag3.CausesValidation = false;
			resources.ApplyResources(this.headerPag3, "headerPag3");
			this.headerPag3.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.ScopeNodeImage_Upgrades;
			this.headerPag3.Name = "headerPag3";
			// 
			// wizardPageFailed
			// 
			this.wizardPageFailed.Controls.Add(this.textBoxErrorMessage);
			this.wizardPageFailed.Controls.Add(this.headerPageFailed);
			resources.ApplyResources(this.wizardPageFailed, "wizardPageFailed");
			this.wizardPageFailed.IsFinishPage = false;
			this.wizardPageFailed.Name = "wizardPageFailed";
			this.wizardPageFailed.CloseFromNext += new Mediachase.Ibn.ConfigurationUI.Wizard.PageEventHandler(this.wizardPageFailed_CloseFromNext);
			// 
			// textBoxErrorMessage
			// 
			this.textBoxErrorMessage.BackColor = System.Drawing.SystemColors.Info;
			this.textBoxErrorMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxErrorMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			resources.ApplyResources(this.textBoxErrorMessage, "textBoxErrorMessage");
			this.textBoxErrorMessage.Name = "textBoxErrorMessage";
			// 
			// headerPageFailed
			// 
			this.headerPageFailed.BackColor = System.Drawing.SystemColors.Control;
			this.headerPageFailed.CausesValidation = false;
			resources.ApplyResources(this.headerPageFailed, "headerPageFailed");
			this.headerPageFailed.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.UpdateCheckForm_Error;
			this.headerPageFailed.Name = "headerPageFailed";
			// 
			// wizardPage2NoUpdateFound
			// 
			this.wizardPage2NoUpdateFound.Controls.Add(this.label4);
			this.wizardPage2NoUpdateFound.Controls.Add(this.label2);
			this.wizardPage2NoUpdateFound.Controls.Add(this.headerPage2);
			resources.ApplyResources(this.wizardPage2NoUpdateFound, "wizardPage2NoUpdateFound");
			this.wizardPage2NoUpdateFound.IsFinishPage = false;
			this.wizardPage2NoUpdateFound.Name = "wizardPage2NoUpdateFound";
			this.wizardPage2NoUpdateFound.CloseFromNext += new Mediachase.Ibn.ConfigurationUI.Wizard.PageEventHandler(this.wizardPage2NoUpdateFound_CloseFromNext);
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// headerPage2
			// 
			this.headerPage2.BackColor = System.Drawing.SystemColors.Control;
			this.headerPage2.CausesValidation = false;
			resources.ApplyResources(this.headerPage2, "headerPage2");
			this.headerPage2.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.ScopeNodeImage_Upgrades;
			this.headerPage2.Name = "headerPage2";
			// 
			// rssDownloader
			// 
			this.rssDownloader.OutputFilePath = null;
			resources.ApplyResources(this.rssDownloader, "rssDownloader");
			this.rssDownloader.Downloading += new System.EventHandler<Mediachase.Ibn.ConfigurationUI.Updates.DownloadProgressEventArgs>(this.rssDownloader_Downloading);
			this.rssDownloader.Failed += new System.EventHandler(this.rssDownloader_Failed);
			this.rssDownloader.Completed += new System.EventHandler(this.rssDownloader_Completed);
			this.rssDownloader.ConnectingToServer += new System.EventHandler(this.rssDownloader_ConnectingToServer);
			// 
			// updateDownloader
			// 
			this.updateDownloader.OutputFilePath = null;
			this.updateDownloader.RequestUri = null;
			this.updateDownloader.Downloading += new System.EventHandler<Mediachase.Ibn.ConfigurationUI.Updates.DownloadProgressEventArgs>(this.updateDownloader_Downloading);
			this.updateDownloader.Failed += new System.EventHandler(this.updateDownloader_Failed);
			this.updateDownloader.Completed += new System.EventHandler(this.updateDownloader_Completed);
			this.updateDownloader.ConnectingToServer += new System.EventHandler(this.updateDownloader_ConnectingToServer);
			// 
			// UpdateCheckForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.wizard);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UpdateCheckForm";
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.UpdateCheckForm_Load);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UpdateCheckForm_FormClosing);
			this.wizard.ResumeLayout(false);
			this.wizardPage1Checking.ResumeLayout(false);
			this.wizardPage1Checking.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheckProgress)).EndInit();
			this.wizardPage5Finish.ResumeLayout(false);
			this.wizardPage5Finish.PerformLayout();
			this.wizardPage4DownloadingUpdate.ResumeLayout(false);
			this.wizardPage4DownloadingUpdate.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxDownloadProgress)).EndInit();
			this.wizardPage3UpdateAvailable.ResumeLayout(false);
			this.wizardPage3UpdateAvailable.PerformLayout();
			this.wizardPageFailed.ResumeLayout(false);
			this.wizardPageFailed.PerformLayout();
			this.wizardPage2NoUpdateFound.ResumeLayout(false);
			this.wizardPage2NoUpdateFound.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private Mediachase.Ibn.ConfigurationUI.Wizard.Wizard wizard;
		private Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage wizardPage1Checking;
		private Mediachase.Ibn.ConfigurationUI.Wizard.Header headerPage1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ProgressBar progressBarRssDownload;
		private Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage wizardPage2NoUpdateFound;
		private System.Windows.Forms.Label label2;
		private Mediachase.Ibn.ConfigurationUI.Wizard.Header headerPage2;
		private Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage wizardPage3UpdateAvailable;
		private Mediachase.Ibn.ConfigurationUI.Wizard.Header headerPag3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.LinkLabel linkLabelViewUpdateMoreInfo;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label labelRssUpdateTitle;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label labelUpdateCheckProgressText;
		private Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage wizardPage4DownloadingUpdate;
		private System.Windows.Forms.Label labelDownloadingProgressText;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ProgressBar progressBarUpdateDownloading;
		private Mediachase.Ibn.ConfigurationUI.Wizard.Header headerPage4;
		private Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage wizardPage5Finish;
		private System.Windows.Forms.Label label3;
		private Mediachase.Ibn.ConfigurationUI.Wizard.Header header1;
		private System.Windows.Forms.Label label8;
		private HttpStreamDownloader rssDownloader;
		private HttpStreamDownloader updateDownloader;
		private Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage wizardPageFailed;
		private System.Windows.Forms.TextBox textBoxErrorMessage;
		private Mediachase.Ibn.ConfigurationUI.Wizard.Header headerPageFailed;
		private System.Windows.Forms.PictureBox pictureBoxCheckProgress;
		private System.Windows.Forms.PictureBox pictureBoxDownloadProgress;

	}
}