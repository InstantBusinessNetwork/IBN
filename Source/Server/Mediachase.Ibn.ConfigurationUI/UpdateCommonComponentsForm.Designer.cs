namespace Mediachase.Ibn.ConfigurationUI
{
	partial class UpdateCommonComponentsForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateCommonComponentsForm));
			this.wizard = new Mediachase.Ibn.ConfigurationUI.Wizard.Wizard();
			this.wizardPage1Info = new Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage();
			this.label7 = new System.Windows.Forms.Label();
			this.comboBoxAvailableUpdates = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.headerPage1 = new Mediachase.Ibn.ConfigurationUI.Wizard.Header();
			this.panelPage1CommonFileUpdateWarning = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this.pictureBoxPage1Warning = new System.Windows.Forms.PictureBox();
			this.wizard.SuspendLayout();
			this.wizardPage1Info.SuspendLayout();
			this.panelPage1CommonFileUpdateWarning.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxPage1Warning)).BeginInit();
			this.SuspendLayout();
			// 
			// wizard
			// 
			this.wizard.AccessibleDescription = null;
			this.wizard.AccessibleName = null;
			resources.ApplyResources(this.wizard, "wizard");
			this.wizard.BackgroundImage = null;
			this.wizard.Controls.Add(this.wizardPage1Info);
			this.wizard.Name = "wizard";
			this.wizard.Pages.AddRange(new Mediachase.Ibn.ConfigurationUI.Wizard.WizardPage[] {
            this.wizardPage1Info});
			// 
			// wizardPage1Info
			// 
			this.wizardPage1Info.AccessibleDescription = null;
			this.wizardPage1Info.AccessibleName = null;
			resources.ApplyResources(this.wizardPage1Info, "wizardPage1Info");
			this.wizardPage1Info.BackgroundImage = null;
			this.wizardPage1Info.Controls.Add(this.label7);
			this.wizardPage1Info.Controls.Add(this.comboBoxAvailableUpdates);
			this.wizardPage1Info.Controls.Add(this.label2);
			this.wizardPage1Info.Controls.Add(this.label1);
			this.wizardPage1Info.Controls.Add(this.label3);
			this.wizardPage1Info.Controls.Add(this.headerPage1);
			this.wizardPage1Info.Controls.Add(this.panelPage1CommonFileUpdateWarning);
			this.wizardPage1Info.Font = null;
			this.wizardPage1Info.IsFinishPage = false;
			this.wizardPage1Info.Name = "wizardPage1Info";
			// 
			// label7
			// 
			this.label7.AccessibleDescription = null;
			this.label7.AccessibleName = null;
			resources.ApplyResources(this.label7, "label7");
			this.label7.Font = null;
			this.label7.Name = "label7";
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
			// UpdateCommonComponentsForm
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
			this.Name = "UpdateCommonComponentsForm";
			this.ShowInTaskbar = false;
			this.wizard.ResumeLayout(false);
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
		private System.Windows.Forms.Label label7;

	}
}