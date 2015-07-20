namespace Mediachase.Ibn.ConfigurationUI
{
	partial class UpdateCompanyForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateCompanyForm));
			this.wizard = new Mediachase.Ibn.ConfigurationUI.Wizard.Wizard();
			this.wizardPage2Update = new Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage();
			this.label5 = new System.Windows.Forms.Label();
			this.textBoxPage2Log = new System.Windows.Forms.TextBox();
			this.headerPage2 = new Mediachase.Ibn.ConfigurationUI.Wizard.Header();
			this.wizardPage3Suceess = new Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage();
			this.linkViewLog = new System.Windows.Forms.LinkLabel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label6 = new System.Windows.Forms.Label();
			this.headerPage3 = new Mediachase.Ibn.ConfigurationUI.Wizard.Header();
			this.wizardPage1Info = new Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage();
			this.labelNextToStartUpdate = new System.Windows.Forms.Label();
			this.comboBoxAvailableUpdates = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.headerPage1 = new Mediachase.Ibn.ConfigurationUI.Wizard.Header();
			this.panelPage1CommonFileUpdateWarning = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this.pictureBoxPage1Warning = new System.Windows.Forms.PictureBox();
			this.wizard.SuspendLayout();
			this.wizardPage2Update.SuspendLayout();
			this.wizardPage3Suceess.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.wizardPage1Info.SuspendLayout();
			this.panelPage1CommonFileUpdateWarning.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxPage1Warning)).BeginInit();
			this.SuspendLayout();
			// 
			// wizard
			// 
			this.wizard.Controls.Add(this.wizardPage1Info);
			this.wizard.Controls.Add(this.wizardPage3Suceess);
			this.wizard.Controls.Add(this.wizardPage2Update);
			resources.ApplyResources(this.wizard, "wizard");
			this.wizard.Name = "wizard";
			this.wizard.Pages.AddRange(new Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage[] {
            this.wizardPage1Info,
            this.wizardPage2Update,
            this.wizardPage3Suceess});
			this.wizard.CloseFromCancel += new System.ComponentModel.CancelEventHandler(this.wizard_CloseFromCancel);
			// 
			// wizardPage2Update
			// 
			this.wizardPage2Update.Controls.Add(this.label5);
			this.wizardPage2Update.Controls.Add(this.textBoxPage2Log);
			this.wizardPage2Update.Controls.Add(this.headerPage2);
			resources.ApplyResources(this.wizardPage2Update, "wizardPage2Update");
			this.wizardPage2Update.IsFinishPage = false;
			this.wizardPage2Update.Name = "wizardPage2Update";
			this.wizardPage2Update.ShowFromNext += new System.EventHandler(this.wizardPage2Update_ShowFromNext);
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// textBoxPage2Log
			// 
			this.textBoxPage2Log.BackColor = System.Drawing.SystemColors.Info;
			this.textBoxPage2Log.BorderStyle = System.Windows.Forms.BorderStyle.None;
			resources.ApplyResources(this.textBoxPage2Log, "textBoxPage2Log");
			this.textBoxPage2Log.Name = "textBoxPage2Log";
			this.textBoxPage2Log.ReadOnly = true;
			// 
			// headerPage2
			// 
			this.headerPage2.BackColor = System.Drawing.SystemColors.Control;
			this.headerPage2.CausesValidation = false;
			resources.ApplyResources(this.headerPage2, "headerPage2");
			this.headerPage2.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.ScopeNodeImage_Upgrade;
			this.headerPage2.Name = "headerPage2";
			// 
			// wizardPage3Suceess
			// 
			this.wizardPage3Suceess.Controls.Add(this.linkViewLog);
			this.wizardPage3Suceess.Controls.Add(this.pictureBox1);
			this.wizardPage3Suceess.Controls.Add(this.label6);
			this.wizardPage3Suceess.Controls.Add(this.headerPage3);
			resources.ApplyResources(this.wizardPage3Suceess, "wizardPage3Suceess");
			this.wizardPage3Suceess.IsFinishPage = false;
			this.wizardPage3Suceess.Name = "wizardPage3Suceess";
			// 
			// linkViewLog
			// 
			resources.ApplyResources(this.linkViewLog, "linkViewLog");
			this.linkViewLog.Name = "linkViewLog";
			this.linkViewLog.TabStop = true;
			this.linkViewLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkViewLog_LinkClicked);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.CompanyUpdate_Result_Success;
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// headerPage3
			// 
			this.headerPage3.BackColor = System.Drawing.SystemColors.Control;
			this.headerPage3.CausesValidation = false;
			resources.ApplyResources(this.headerPage3, "headerPage3");
			this.headerPage3.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.ScopeNodeImage_Upgrade;
			this.headerPage3.Name = "headerPage3";
			// 
			// wizardPage1Info
			// 
			this.wizardPage1Info.Controls.Add(this.labelNextToStartUpdate);
			this.wizardPage1Info.Controls.Add(this.comboBoxAvailableUpdates);
			this.wizardPage1Info.Controls.Add(this.label2);
			this.wizardPage1Info.Controls.Add(this.label1);
			this.wizardPage1Info.Controls.Add(this.label3);
			this.wizardPage1Info.Controls.Add(this.headerPage1);
			this.wizardPage1Info.Controls.Add(this.panelPage1CommonFileUpdateWarning);
			resources.ApplyResources(this.wizardPage1Info, "wizardPage1Info");
			this.wizardPage1Info.IsFinishPage = false;
			this.wizardPage1Info.Name = "wizardPage1Info";
			this.wizardPage1Info.CloseFromNext += new Mediachase.Ibn.ConfigurationUI.Wizard.PageEventHandler(this.wizardPage1Info_CloseFromNext);
			// 
			// labelNextToStartUpdate
			// 
			resources.ApplyResources(this.labelNextToStartUpdate, "labelNextToStartUpdate");
			this.labelNextToStartUpdate.Name = "labelNextToStartUpdate";
			// 
			// comboBoxAvailableUpdates
			// 
			this.comboBoxAvailableUpdates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxAvailableUpdates.FormattingEnabled = true;
			resources.ApplyResources(this.comboBoxAvailableUpdates, "comboBoxAvailableUpdates");
			this.comboBoxAvailableUpdates.Name = "comboBoxAvailableUpdates";
			this.comboBoxAvailableUpdates.SelectedIndexChanged += new System.EventHandler(this.comboBoxAvailableUpdates_SelectedIndexChanged);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// headerPage1
			// 
			this.headerPage1.BackColor = System.Drawing.SystemColors.Control;
			this.headerPage1.CausesValidation = false;
			resources.ApplyResources(this.headerPage1, "headerPage1");
			this.headerPage1.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.ScopeNodeImage_Upgrade;
			this.headerPage1.Name = "headerPage1";
			// 
			// panelPage1CommonFileUpdateWarning
			// 
			this.panelPage1CommonFileUpdateWarning.BackColor = System.Drawing.SystemColors.Info;
			this.panelPage1CommonFileUpdateWarning.Controls.Add(this.label4);
			this.panelPage1CommonFileUpdateWarning.Controls.Add(this.pictureBoxPage1Warning);
			resources.ApplyResources(this.panelPage1CommonFileUpdateWarning, "panelPage1CommonFileUpdateWarning");
			this.panelPage1CommonFileUpdateWarning.Name = "panelPage1CommonFileUpdateWarning";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// pictureBoxPage1Warning
			// 
			this.pictureBoxPage1Warning.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.Messagebox_Warning;
			resources.ApplyResources(this.pictureBoxPage1Warning, "pictureBoxPage1Warning");
			this.pictureBoxPage1Warning.Name = "pictureBoxPage1Warning";
			this.pictureBoxPage1Warning.TabStop = false;
			// 
			// UpdateCompanyForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.wizard);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UpdateCompanyForm";
			this.ShowInTaskbar = false;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UpdateCompanyForm_FormClosing);
			this.wizard.ResumeLayout(false);
			this.wizardPage2Update.ResumeLayout(false);
			this.wizardPage2Update.PerformLayout();
			this.wizardPage3Suceess.ResumeLayout(false);
			this.wizardPage3Suceess.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.wizardPage1Info.ResumeLayout(false);
			this.wizardPage1Info.PerformLayout();
			this.panelPage1CommonFileUpdateWarning.ResumeLayout(false);
			this.panelPage1CommonFileUpdateWarning.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxPage1Warning)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Mediachase.Ibn.ConfigurationUI.Wizard.Wizard wizard;
		private Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage wizardPage1Info;
		private Mediachase.Ibn.ConfigurationUI.Wizard.Header headerPage1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBoxAvailableUpdates;
		private System.Windows.Forms.Panel panelPage1CommonFileUpdateWarning;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.PictureBox pictureBoxPage1Warning;
		private Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage wizardPage2Update;
		private Mediachase.Ibn.ConfigurationUI.Wizard.Header headerPage2;
		private System.Windows.Forms.TextBox textBoxPage2Log;
		private System.Windows.Forms.Label label5;
		private Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage wizardPage3Suceess;
		private Mediachase.Ibn.ConfigurationUI.Wizard.Header headerPage3;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label labelNextToStartUpdate;
		private System.Windows.Forms.LinkLabel linkViewLog;

	}
}