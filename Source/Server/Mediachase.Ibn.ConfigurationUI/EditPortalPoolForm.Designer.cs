namespace Mediachase.Ibn.ConfigurationUI
{
	partial class EditPortalPoolForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditPortalPoolForm));
			this.mainPanel = new System.Windows.Forms.Panel();
			this.groupBoxIisSettings = new System.Windows.Forms.GroupBox();
			this.comboBoxIisPool = new System.Windows.Forms.ComboBox();
			this.labelIisPool = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.panelHSplit = new System.Windows.Forms.Panel();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.mainPanel.SuspendLayout();
			this.groupBoxIisSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// mainPanel
			// 
			this.mainPanel.BackColor = System.Drawing.SystemColors.Window;
			this.mainPanel.Controls.Add(this.groupBoxIisSettings);
			resources.ApplyResources(this.mainPanel, "mainPanel");
			this.mainPanel.Name = "mainPanel";
			// 
			// groupBoxIisSettings
			// 
			this.groupBoxIisSettings.Controls.Add(this.comboBoxIisPool);
			this.groupBoxIisSettings.Controls.Add(this.labelIisPool);
			resources.ApplyResources(this.groupBoxIisSettings, "groupBoxIisSettings");
			this.groupBoxIisSettings.ForeColor = System.Drawing.Color.DarkBlue;
			this.groupBoxIisSettings.Name = "groupBoxIisSettings";
			this.groupBoxIisSettings.TabStop = false;
			// 
			// comboBoxIisPool
			// 
			this.comboBoxIisPool.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.comboBoxIisPool, "comboBoxIisPool");
			this.comboBoxIisPool.FormattingEnabled = true;
			this.comboBoxIisPool.Name = "comboBoxIisPool";
			// 
			// labelIisPool
			// 
			resources.ApplyResources(this.labelIisPool, "labelIisPool");
			this.labelIisPool.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labelIisPool.Name = "labelIisPool";
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOk
			// 
			resources.ApplyResources(this.buttonOk, "buttonOk");
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// panelHSplit
			// 
			this.panelHSplit.BackColor = System.Drawing.Color.Black;
			resources.ApplyResources(this.panelHSplit, "panelHSplit");
			this.panelHSplit.Name = "panelHSplit";
			// 
			// errorProvider
			// 
			this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
			this.errorProvider.ContainerControl = this;
			// 
			// EditPortalPoolForm
			// 
			this.AcceptButton = this.buttonOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.panelHSplit);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.mainPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EditPortalPoolForm";
			this.ShowInTaskbar = false;
			this.mainPanel.ResumeLayout(false);
			this.groupBoxIisSettings.ResumeLayout(false);
			this.groupBoxIisSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel mainPanel;
		private System.Windows.Forms.GroupBox groupBoxIisSettings;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Panel panelHSplit;
		private System.Windows.Forms.ErrorProvider errorProvider;
		internal System.Windows.Forms.ComboBox comboBoxIisPool;
		private System.Windows.Forms.Label labelIisPool;
	}
}