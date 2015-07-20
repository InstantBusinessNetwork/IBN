namespace Mediachase.Ibn.ConfigurationUI
{
	partial class EditDefaultAddressForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditDefaultAddressForm));
			this.panelHSplit = new System.Windows.Forms.Panel();
			this.mainPanel = new System.Windows.Forms.Panel();
			this.groupBoxCompany = new System.Windows.Forms.GroupBox();
			this.textBoxIisPort = new System.Windows.Forms.TextBox();
			this.labelPort = new System.Windows.Forms.Label();
			this.comboBoxSchema = new System.Windows.Forms.ComboBox();
			this.labelScheme = new System.Windows.Forms.Label();
			this.labelHost = new System.Windows.Forms.Label();
			this.textHostName = new System.Windows.Forms.TextBox();
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.mainPanel.SuspendLayout();
			this.groupBoxCompany.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// panelHSplit
			// 
			this.panelHSplit.BackColor = System.Drawing.Color.Black;
			resources.ApplyResources(this.panelHSplit, "panelHSplit");
			this.panelHSplit.Name = "panelHSplit";
			// 
			// mainPanel
			// 
			this.mainPanel.BackColor = System.Drawing.SystemColors.Window;
			this.mainPanel.Controls.Add(this.groupBoxCompany);
			resources.ApplyResources(this.mainPanel, "mainPanel");
			this.mainPanel.Name = "mainPanel";
			// 
			// groupBoxCompany
			// 
			this.groupBoxCompany.Controls.Add(this.textBoxIisPort);
			this.groupBoxCompany.Controls.Add(this.labelPort);
			this.groupBoxCompany.Controls.Add(this.comboBoxSchema);
			this.groupBoxCompany.Controls.Add(this.labelScheme);
			this.groupBoxCompany.Controls.Add(this.labelHost);
			this.groupBoxCompany.Controls.Add(this.textHostName);
			resources.ApplyResources(this.groupBoxCompany, "groupBoxCompany");
			this.groupBoxCompany.ForeColor = System.Drawing.Color.DarkBlue;
			this.groupBoxCompany.Name = "groupBoxCompany";
			this.groupBoxCompany.TabStop = false;
			this.groupBoxCompany.UseCompatibleTextRendering = true;
			// 
			// textBoxIisPort
			// 
			resources.ApplyResources(this.textBoxIisPort, "textBoxIisPort");
			this.textBoxIisPort.Name = "textBoxIisPort";
			this.textBoxIisPort.Validated += new System.EventHandler(this.textBoxIisPort_Validated);
			// 
			// labelPort
			// 
			resources.ApplyResources(this.labelPort, "labelPort");
			this.labelPort.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labelPort.Name = "labelPort";
			// 
			// comboBoxSchema
			// 
			this.comboBoxSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.comboBoxSchema, "comboBoxSchema");
			this.comboBoxSchema.FormattingEnabled = true;
			this.comboBoxSchema.Items.AddRange(new object[] {
            resources.GetString("comboBoxSchema.Items"),
            resources.GetString("comboBoxSchema.Items1")});
			this.comboBoxSchema.Name = "comboBoxSchema";
			// 
			// labelScheme
			// 
			this.labelScheme.AllowDrop = true;
			resources.ApplyResources(this.labelScheme, "labelScheme");
			this.labelScheme.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labelScheme.Name = "labelScheme";
			// 
			// labelHost
			// 
			resources.ApplyResources(this.labelHost, "labelHost");
			this.labelHost.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labelHost.Name = "labelHost";
			// 
			// textHostName
			// 
			this.textHostName.AcceptsTab = true;
			resources.ApplyResources(this.textHostName, "textHostName");
			this.textHostName.Name = "textHostName";
			this.textHostName.Validated += new System.EventHandler(this.textBoxNewDomainName_Validated);
			// 
			// buttonOk
			// 
			resources.ApplyResources(this.buttonOk, "buttonOk");
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// errorProvider
			// 
			this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
			this.errorProvider.ContainerControl = this;
			// 
			// EditDefaultAddressForm
			// 
			this.AcceptButton = this.buttonOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.panelHSplit);
			this.Controls.Add(this.mainPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EditDefaultAddressForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.mainPanel.ResumeLayout(false);
			this.groupBoxCompany.ResumeLayout(false);
			this.groupBoxCompany.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelHSplit;
		private System.Windows.Forms.Panel mainPanel;
		private System.Windows.Forms.GroupBox groupBoxCompany;
		private System.Windows.Forms.Label labelHost;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.Label labelScheme;
		private System.Windows.Forms.Label labelPort;
		private System.Windows.Forms.TextBox textBoxIisPort;
		private System.Windows.Forms.TextBox textHostName;
		private System.Windows.Forms.ComboBox comboBoxSchema;
	}
}