namespace Mediachase.Ibn.ConfigurationUI
{
	partial class DeleteAspForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeleteAspForm));
			this.panelHSplit = new System.Windows.Forms.Panel();
			this.mainPanel = new System.Windows.Forms.Panel();
			this.checkBoxDeleteDatabase = new System.Windows.Forms.CheckBox();
			this.groupBoxCompany = new System.Windows.Forms.GroupBox();
			this.labelWarningMsg = new System.Windows.Forms.Label();
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.mainPanel.SuspendLayout();
			this.groupBoxCompany.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelHSplit
			// 
			this.panelHSplit.AccessibleDescription = null;
			this.panelHSplit.AccessibleName = null;
			resources.ApplyResources(this.panelHSplit, "panelHSplit");
			this.panelHSplit.BackColor = System.Drawing.Color.Black;
			this.panelHSplit.BackgroundImage = null;
			this.panelHSplit.Font = null;
			this.panelHSplit.Name = "panelHSplit";
			// 
			// mainPanel
			// 
			this.mainPanel.AccessibleDescription = null;
			this.mainPanel.AccessibleName = null;
			resources.ApplyResources(this.mainPanel, "mainPanel");
			this.mainPanel.BackColor = System.Drawing.SystemColors.Window;
			this.mainPanel.BackgroundImage = null;
			this.mainPanel.Controls.Add(this.checkBoxDeleteDatabase);
			this.mainPanel.Controls.Add(this.groupBoxCompany);
			this.mainPanel.Font = null;
			this.mainPanel.Name = "mainPanel";
			// 
			// checkBoxDeleteDatabase
			// 
			this.checkBoxDeleteDatabase.AccessibleDescription = null;
			this.checkBoxDeleteDatabase.AccessibleName = null;
			resources.ApplyResources(this.checkBoxDeleteDatabase, "checkBoxDeleteDatabase");
			this.checkBoxDeleteDatabase.BackgroundImage = null;
			this.checkBoxDeleteDatabase.Checked = true;
			this.checkBoxDeleteDatabase.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxDeleteDatabase.ForeColor = System.Drawing.SystemColors.ControlText;
			this.checkBoxDeleteDatabase.Name = "checkBoxDeleteDatabase";
			this.checkBoxDeleteDatabase.UseVisualStyleBackColor = true;
			// 
			// groupBoxCompany
			// 
			this.groupBoxCompany.AccessibleDescription = null;
			this.groupBoxCompany.AccessibleName = null;
			resources.ApplyResources(this.groupBoxCompany, "groupBoxCompany");
			this.groupBoxCompany.BackgroundImage = null;
			this.groupBoxCompany.Controls.Add(this.labelWarningMsg);
			this.groupBoxCompany.ForeColor = System.Drawing.Color.Red;
			this.groupBoxCompany.Name = "groupBoxCompany";
			this.groupBoxCompany.TabStop = false;
			// 
			// labelWarningMsg
			// 
			this.labelWarningMsg.AccessibleDescription = null;
			this.labelWarningMsg.AccessibleName = null;
			resources.ApplyResources(this.labelWarningMsg, "labelWarningMsg");
			this.labelWarningMsg.AutoEllipsis = true;
			this.labelWarningMsg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.labelWarningMsg.Name = "labelWarningMsg";
			// 
			// buttonOk
			// 
			this.buttonOk.AccessibleDescription = null;
			this.buttonOk.AccessibleName = null;
			resources.ApplyResources(this.buttonOk, "buttonOk");
			this.buttonOk.BackgroundImage = null;
			this.buttonOk.Font = null;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.AccessibleDescription = null;
			this.buttonCancel.AccessibleName = null;
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.BackgroundImage = null;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Font = null;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// DeleteAspForm
			// 
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = null;
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.panelHSplit);
			this.Controls.Add(this.mainPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DeleteAspForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.mainPanel.ResumeLayout(false);
			this.mainPanel.PerformLayout();
			this.groupBoxCompany.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelHSplit;
		private System.Windows.Forms.Panel mainPanel;
		private System.Windows.Forms.GroupBox groupBoxCompany;
		private System.Windows.Forms.Label labelWarningMsg;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.CheckBox checkBoxDeleteDatabase;
	}
}