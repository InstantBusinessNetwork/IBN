namespace Mediachase.Ibn.ConfigurationUI
{
	partial class CreateCompanyForDatabaseForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateCompanyForDatabaseForm));
			this.mainPanel = new System.Windows.Forms.Panel();
			this.groupBoxCompany = new System.Windows.Forms.GroupBox();
			this.comboBoxSqlDatabase = new System.Windows.Forms.ComboBox();
			this.labelSqlDatabaseName = new System.Windows.Forms.Label();
			this.comboBoxIisPool = new System.Windows.Forms.ComboBox();
			this.labelIisPool = new System.Windows.Forms.Label();
			this.checkBoxIsActive = new System.Windows.Forms.CheckBox();
			this.textBoxIisPort = new System.Windows.Forms.TextBox();
			this.labelHost = new System.Windows.Forms.Label();
			this.labelIisPort = new System.Windows.Forms.Label();
			this.textBoxHost = new System.Windows.Forms.TextBox();
			this.comboBoxIisIPAddress = new System.Windows.Forms.ComboBox();
			this.labelIisIPAddress = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.panelHSplit = new System.Windows.Forms.Panel();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.mainPanel.SuspendLayout();
			this.groupBoxCompany.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// mainPanel
			// 
			resources.ApplyResources(this.mainPanel, "mainPanel");
			this.mainPanel.BackColor = System.Drawing.SystemColors.Window;
			this.mainPanel.Controls.Add(this.groupBoxCompany);
			this.mainPanel.Name = "mainPanel";
			// 
			// groupBoxCompany
			// 
			this.groupBoxCompany.Controls.Add(this.comboBoxSqlDatabase);
			this.groupBoxCompany.Controls.Add(this.labelSqlDatabaseName);
			this.groupBoxCompany.Controls.Add(this.comboBoxIisPool);
			this.groupBoxCompany.Controls.Add(this.labelIisPool);
			this.groupBoxCompany.Controls.Add(this.checkBoxIsActive);
			this.groupBoxCompany.Controls.Add(this.textBoxIisPort);
			this.groupBoxCompany.Controls.Add(this.labelHost);
			this.groupBoxCompany.Controls.Add(this.labelIisPort);
			this.groupBoxCompany.Controls.Add(this.textBoxHost);
			this.groupBoxCompany.Controls.Add(this.comboBoxIisIPAddress);
			this.groupBoxCompany.Controls.Add(this.labelIisIPAddress);
			resources.ApplyResources(this.groupBoxCompany, "groupBoxCompany");
			this.groupBoxCompany.ForeColor = System.Drawing.Color.DarkBlue;
			this.groupBoxCompany.Name = "groupBoxCompany";
			this.groupBoxCompany.TabStop = false;
			// 
			// comboBoxSqlDatabase
			// 
			this.comboBoxSqlDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.comboBoxSqlDatabase, "comboBoxSqlDatabase");
			this.comboBoxSqlDatabase.FormattingEnabled = true;
			this.comboBoxSqlDatabase.Name = "comboBoxSqlDatabase";
			this.comboBoxSqlDatabase.Validating += new System.ComponentModel.CancelEventHandler(this.comboBoxSqlDatabase_Validating);
			// 
			// labelSqlDatabaseName
			// 
			resources.ApplyResources(this.labelSqlDatabaseName, "labelSqlDatabaseName");
			this.labelSqlDatabaseName.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labelSqlDatabaseName.Name = "labelSqlDatabaseName";
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
			// checkBoxIsActive
			// 
			resources.ApplyResources(this.checkBoxIsActive, "checkBoxIsActive");
			this.checkBoxIsActive.Checked = true;
			this.checkBoxIsActive.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxIsActive.ForeColor = System.Drawing.SystemColors.ControlText;
			this.checkBoxIsActive.Name = "checkBoxIsActive";
			this.checkBoxIsActive.UseVisualStyleBackColor = true;
			// 
			// textBoxIisPort
			// 
			resources.ApplyResources(this.textBoxIisPort, "textBoxIisPort");
			this.textBoxIisPort.Name = "textBoxIisPort";
			this.textBoxIisPort.Validated += new System.EventHandler(this.textBoxIisPort_Validated);
			// 
			// labelHost
			// 
			resources.ApplyResources(this.labelHost, "labelHost");
			this.labelHost.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labelHost.Name = "labelHost";
			// 
			// labelIisPort
			// 
			resources.ApplyResources(this.labelIisPort, "labelIisPort");
			this.labelIisPort.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labelIisPort.Name = "labelIisPort";
			// 
			// textBoxHost
			// 
			this.textBoxHost.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxHost, "textBoxHost");
			this.textBoxHost.Name = "textBoxHost";
			this.textBoxHost.Validated += new System.EventHandler(this.textBoxDomainName_Validated);
			// 
			// comboBoxIisIPAddress
			// 
			this.comboBoxIisIPAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.comboBoxIisIPAddress, "comboBoxIisIPAddress");
			this.comboBoxIisIPAddress.FormattingEnabled = true;
			this.comboBoxIisIPAddress.Name = "comboBoxIisIPAddress";
			// 
			// labelIisIPAddress
			// 
			resources.ApplyResources(this.labelIisIPAddress, "labelIisIPAddress");
			this.labelIisIPAddress.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labelIisIPAddress.Name = "labelIisIPAddress";
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
			// CreateCompanyForDatabaseForm
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
			this.Name = "CreateCompanyForDatabaseForm";
			this.ShowInTaskbar = false;
			this.mainPanel.ResumeLayout(false);
			this.groupBoxCompany.ResumeLayout(false);
			this.groupBoxCompany.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel mainPanel;
		private System.Windows.Forms.Label labelIisIPAddress;
		private System.Windows.Forms.Label labelHost;
		private System.Windows.Forms.Label labelIisPort;
		private System.Windows.Forms.GroupBox groupBoxCompany;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Panel panelHSplit;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.ComboBox comboBoxIisIPAddress;
		private System.Windows.Forms.ComboBox comboBoxIisPool;
		private System.Windows.Forms.Label labelIisPool;
		internal System.Windows.Forms.TextBox textBoxHost;
		internal System.Windows.Forms.TextBox textBoxIisPort;
		internal System.Windows.Forms.CheckBox checkBoxIsActive;
		private System.Windows.Forms.Label labelSqlDatabaseName;
		internal System.Windows.Forms.ComboBox comboBoxSqlDatabase;
	}
}