namespace Mediachase.Ibn.ConfigurationUI
{
	partial class UpdateCompanyListForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateCompanyListForm));
			this.wizard = new Mediachase.Ibn.ConfigurationUI.Wizard.Wizard();
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
			this.wizardPage3Suceess = new Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage();
			this.tableLayoutPanelUpdateLog = new System.Windows.Forms.TableLayoutPanel();
			this.pictureBoxUpdateResult = new System.Windows.Forms.PictureBox();
			this.labelFinalResult = new System.Windows.Forms.Label();
			this.headerPage3 = new Mediachase.Ibn.ConfigurationUI.Wizard.Header();
			this.wizardPage2Update = new Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage();
			this.label5 = new System.Windows.Forms.Label();
			this.textBoxPage2Log = new System.Windows.Forms.TextBox();
			this.headerPage2 = new Mediachase.Ibn.ConfigurationUI.Wizard.Header();
			this.wizard.SuspendLayout();
			this.wizardPage1Info.SuspendLayout();
			this.panelPage1CommonFileUpdateWarning.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxPage1Warning)).BeginInit();
			this.wizardPage3Suceess.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxUpdateResult)).BeginInit();
			this.wizardPage2Update.SuspendLayout();
			this.SuspendLayout();
			// 
			// wizard
			// 
			this.wizard.AccessibleDescription = null;
			this.wizard.AccessibleName = null;
			resources.ApplyResources(this.wizard, "wizard");
			this.wizard.BackgroundImage = null;
			this.wizard.Controls.Add(this.wizardPage1Info);
			this.wizard.Controls.Add(this.wizardPage3Suceess);
			this.wizard.Controls.Add(this.wizardPage2Update);
			this.wizard.Name = "wizard";
			this.wizard.Pages.AddRange(new Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage[] {
            this.wizardPage1Info,
            this.wizardPage2Update,
            this.wizardPage3Suceess});
			this.wizard.CloseFromCancel += new System.ComponentModel.CancelEventHandler(this.wizard_CloseFromCancel);
			// 
			// wizardPage1Info
			// 
			this.wizardPage1Info.AccessibleDescription = null;
			this.wizardPage1Info.AccessibleName = null;
			resources.ApplyResources(this.wizardPage1Info, "wizardPage1Info");
			this.wizardPage1Info.BackgroundImage = null;
			this.wizardPage1Info.Controls.Add(this.labelNextToStartUpdate);
			this.wizardPage1Info.Controls.Add(this.comboBoxAvailableUpdates);
			this.wizardPage1Info.Controls.Add(this.label2);
			this.wizardPage1Info.Controls.Add(this.label1);
			this.wizardPage1Info.Controls.Add(this.label3);
			this.wizardPage1Info.Controls.Add(this.headerPage1);
			this.wizardPage1Info.Controls.Add(this.panelPage1CommonFileUpdateWarning);
			this.wizardPage1Info.Font = null;
			this.wizardPage1Info.IsFinishPage = false;
			this.wizardPage1Info.Name = "wizardPage1Info";
			this.wizardPage1Info.CloseFromNext += new Mediachase.Ibn.ConfigurationUI.Wizard.PageEventHandler(this.wizardPage1Info_CloseFromNext);
			// 
			// labelNextToStartUpdate
			// 
			this.labelNextToStartUpdate.AccessibleDescription = null;
			this.labelNextToStartUpdate.AccessibleName = null;
			resources.ApplyResources(this.labelNextToStartUpdate, "labelNextToStartUpdate");
			this.labelNextToStartUpdate.Font = null;
			this.labelNextToStartUpdate.Name = "labelNextToStartUpdate";
			// 
			// comboBoxAvailableUpdates
			// 
			this.comboBoxAvailableUpdates.AccessibleDescription = null;
			this.comboBoxAvailableUpdates.AccessibleName = null;
			resources.ApplyResources(this.comboBoxAvailableUpdates, "comboBoxAvailableUpdates");
			this.comboBoxAvailableUpdates.BackgroundImage = null;
			this.comboBoxAvailableUpdates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxAvailableUpdates.Font = null;
			this.comboBoxAvailableUpdates.FormattingEnabled = true;
			this.comboBoxAvailableUpdates.Name = "comboBoxAvailableUpdates";
			this.comboBoxAvailableUpdates.SelectedIndexChanged += new System.EventHandler(this.comboBoxAvailableUpdates_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AccessibleDescription = null;
			this.label2.AccessibleName = null;
			resources.ApplyResources(this.label2, "label2");
			this.label2.Font = null;
			this.label2.Name = "label2";
			// 
			// label1
			// 
			this.label1.AccessibleDescription = null;
			this.label1.AccessibleName = null;
			resources.ApplyResources(this.label1, "label1");
			this.label1.Font = null;
			this.label1.Name = "label1";
			// 
			// label3
			// 
			this.label3.AccessibleDescription = null;
			this.label3.AccessibleName = null;
			resources.ApplyResources(this.label3, "label3");
			this.label3.Font = null;
			this.label3.Name = "label3";
			// 
			// headerPage1
			// 
			this.headerPage1.AccessibleDescription = null;
			this.headerPage1.AccessibleName = null;
			resources.ApplyResources(this.headerPage1, "headerPage1");
			this.headerPage1.BackColor = System.Drawing.SystemColors.Control;
			this.headerPage1.CausesValidation = false;
			this.headerPage1.Font = null;
			this.headerPage1.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.ScopeNodeImage_Upgrade;
			this.headerPage1.Name = "headerPage1";
			// 
			// panelPage1CommonFileUpdateWarning
			// 
			this.panelPage1CommonFileUpdateWarning.AccessibleDescription = null;
			this.panelPage1CommonFileUpdateWarning.AccessibleName = null;
			resources.ApplyResources(this.panelPage1CommonFileUpdateWarning, "panelPage1CommonFileUpdateWarning");
			this.panelPage1CommonFileUpdateWarning.BackColor = System.Drawing.SystemColors.Info;
			this.panelPage1CommonFileUpdateWarning.BackgroundImage = null;
			this.panelPage1CommonFileUpdateWarning.Controls.Add(this.label4);
			this.panelPage1CommonFileUpdateWarning.Controls.Add(this.pictureBoxPage1Warning);
			this.panelPage1CommonFileUpdateWarning.Font = null;
			this.panelPage1CommonFileUpdateWarning.Name = "panelPage1CommonFileUpdateWarning";
			// 
			// label4
			// 
			this.label4.AccessibleDescription = null;
			this.label4.AccessibleName = null;
			resources.ApplyResources(this.label4, "label4");
			this.label4.Font = null;
			this.label4.Name = "label4";
			// 
			// pictureBoxPage1Warning
			// 
			this.pictureBoxPage1Warning.AccessibleDescription = null;
			this.pictureBoxPage1Warning.AccessibleName = null;
			resources.ApplyResources(this.pictureBoxPage1Warning, "pictureBoxPage1Warning");
			this.pictureBoxPage1Warning.BackgroundImage = null;
			this.pictureBoxPage1Warning.Font = null;
			this.pictureBoxPage1Warning.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.Messagebox_Warning;
			this.pictureBoxPage1Warning.ImageLocation = null;
			this.pictureBoxPage1Warning.Name = "pictureBoxPage1Warning";
			this.pictureBoxPage1Warning.TabStop = false;
			// 
			// wizardPage3Suceess
			// 
			this.wizardPage3Suceess.AccessibleDescription = null;
			this.wizardPage3Suceess.AccessibleName = null;
			resources.ApplyResources(this.wizardPage3Suceess, "wizardPage3Suceess");
			this.wizardPage3Suceess.BackgroundImage = null;
			this.wizardPage3Suceess.Controls.Add(this.tableLayoutPanelUpdateLog);
			this.wizardPage3Suceess.Controls.Add(this.pictureBoxUpdateResult);
			this.wizardPage3Suceess.Controls.Add(this.labelFinalResult);
			this.wizardPage3Suceess.Controls.Add(this.headerPage3);
			this.wizardPage3Suceess.Font = null;
			this.wizardPage3Suceess.IsFinishPage = false;
			this.wizardPage3Suceess.Name = "wizardPage3Suceess";
			// 
			// tableLayoutPanelUpdateLog
			// 
			this.tableLayoutPanelUpdateLog.AccessibleDescription = null;
			this.tableLayoutPanelUpdateLog.AccessibleName = null;
			resources.ApplyResources(this.tableLayoutPanelUpdateLog, "tableLayoutPanelUpdateLog");
			this.tableLayoutPanelUpdateLog.BackColor = System.Drawing.SystemColors.Window;
			this.tableLayoutPanelUpdateLog.BackgroundImage = null;
			this.tableLayoutPanelUpdateLog.Font = null;
			this.tableLayoutPanelUpdateLog.Name = "tableLayoutPanelUpdateLog";
			// 
			// pictureBoxUpdateResult
			// 
			this.pictureBoxUpdateResult.AccessibleDescription = null;
			this.pictureBoxUpdateResult.AccessibleName = null;
			resources.ApplyResources(this.pictureBoxUpdateResult, "pictureBoxUpdateResult");
			this.pictureBoxUpdateResult.BackgroundImage = null;
			this.pictureBoxUpdateResult.Font = null;
			this.pictureBoxUpdateResult.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.CompanyUpdate_Result_Success;
			this.pictureBoxUpdateResult.ImageLocation = null;
			this.pictureBoxUpdateResult.Name = "pictureBoxUpdateResult";
			this.pictureBoxUpdateResult.TabStop = false;
			// 
			// labelFinalResult
			// 
			this.labelFinalResult.AccessibleDescription = null;
			this.labelFinalResult.AccessibleName = null;
			resources.ApplyResources(this.labelFinalResult, "labelFinalResult");
			this.labelFinalResult.Font = null;
			this.labelFinalResult.Name = "labelFinalResult";
			// 
			// headerPage3
			// 
			this.headerPage3.AccessibleDescription = null;
			this.headerPage3.AccessibleName = null;
			resources.ApplyResources(this.headerPage3, "headerPage3");
			this.headerPage3.BackColor = System.Drawing.SystemColors.Control;
			this.headerPage3.CausesValidation = false;
			this.headerPage3.Font = null;
			this.headerPage3.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.ScopeNodeImage_Upgrade;
			this.headerPage3.Name = "headerPage3";
			// 
			// wizardPage2Update
			// 
			this.wizardPage2Update.AccessibleDescription = null;
			this.wizardPage2Update.AccessibleName = null;
			resources.ApplyResources(this.wizardPage2Update, "wizardPage2Update");
			this.wizardPage2Update.BackgroundImage = null;
			this.wizardPage2Update.Controls.Add(this.label5);
			this.wizardPage2Update.Controls.Add(this.textBoxPage2Log);
			this.wizardPage2Update.Controls.Add(this.headerPage2);
			this.wizardPage2Update.Font = null;
			this.wizardPage2Update.IsFinishPage = false;
			this.wizardPage2Update.Name = "wizardPage2Update";
			this.wizardPage2Update.ShowFromNext += new System.EventHandler(this.wizardPage2Update_ShowFromNext);
			// 
			// label5
			// 
			this.label5.AccessibleDescription = null;
			this.label5.AccessibleName = null;
			resources.ApplyResources(this.label5, "label5");
			this.label5.Font = null;
			this.label5.Name = "label5";
			// 
			// textBoxPage2Log
			// 
			this.textBoxPage2Log.AccessibleDescription = null;
			this.textBoxPage2Log.AccessibleName = null;
			resources.ApplyResources(this.textBoxPage2Log, "textBoxPage2Log");
			this.textBoxPage2Log.BackColor = System.Drawing.SystemColors.Info;
			this.textBoxPage2Log.BackgroundImage = null;
			this.textBoxPage2Log.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxPage2Log.Font = null;
			this.textBoxPage2Log.Name = "textBoxPage2Log";
			this.textBoxPage2Log.ReadOnly = true;
			// 
			// headerPage2
			// 
			this.headerPage2.AccessibleDescription = null;
			this.headerPage2.AccessibleName = null;
			resources.ApplyResources(this.headerPage2, "headerPage2");
			this.headerPage2.BackColor = System.Drawing.SystemColors.Control;
			this.headerPage2.CausesValidation = false;
			this.headerPage2.Font = null;
			this.headerPage2.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.ScopeNodeImage_Upgrade;
			this.headerPage2.Name = "headerPage2";
			// 
			// UpdateCompanyListForm
			// 
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = null;
			this.Controls.Add(this.wizard);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UpdateCompanyListForm";
			this.ShowInTaskbar = false;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UpdateCompanyForm_FormClosing);
			this.wizard.ResumeLayout(false);
			this.wizardPage1Info.ResumeLayout(false);
			this.wizardPage1Info.PerformLayout();
			this.panelPage1CommonFileUpdateWarning.ResumeLayout(false);
			this.panelPage1CommonFileUpdateWarning.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxPage1Warning)).EndInit();
			this.wizardPage3Suceess.ResumeLayout(false);
			this.wizardPage3Suceess.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxUpdateResult)).EndInit();
			this.wizardPage2Update.ResumeLayout(false);
			this.wizardPage2Update.PerformLayout();
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
		private System.Windows.Forms.Label labelFinalResult;
		private System.Windows.Forms.PictureBox pictureBoxUpdateResult;
		private System.Windows.Forms.Label labelNextToStartUpdate;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelUpdateLog;

	}
}