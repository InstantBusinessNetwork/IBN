namespace Mediachase.Ibn.ConfigurationUI
{
	partial class CreateAspForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateAspForm));
			this.mainPanel = new System.Windows.Forms.Panel();
			this.groupBoxCompany = new System.Windows.Forms.GroupBox();
			this.comboBoxIisPool = new System.Windows.Forms.ComboBox();
			this.labelIisPool = new System.Windows.Forms.Label();
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
			this.mainPanel.AccessibleDescription = null;
			this.mainPanel.AccessibleName = null;
			resources.ApplyResources(this.mainPanel, "mainPanel");
			this.mainPanel.BackColor = System.Drawing.SystemColors.Window;
			this.mainPanel.BackgroundImage = null;
			this.mainPanel.Controls.Add(this.groupBoxCompany);
			this.errorProvider.SetError(this.mainPanel, resources.GetString("mainPanel.Error"));
			this.mainPanel.Font = null;
			this.errorProvider.SetIconAlignment(this.mainPanel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mainPanel.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.mainPanel, ((int)(resources.GetObject("mainPanel.IconPadding"))));
			this.mainPanel.Name = "mainPanel";
			// 
			// groupBoxCompany
			// 
			this.groupBoxCompany.AccessibleDescription = null;
			this.groupBoxCompany.AccessibleName = null;
			resources.ApplyResources(this.groupBoxCompany, "groupBoxCompany");
			this.groupBoxCompany.BackgroundImage = null;
			this.groupBoxCompany.Controls.Add(this.comboBoxIisPool);
			this.groupBoxCompany.Controls.Add(this.labelIisPool);
			this.groupBoxCompany.Controls.Add(this.textBoxIisPort);
			this.groupBoxCompany.Controls.Add(this.labelHost);
			this.groupBoxCompany.Controls.Add(this.labelIisPort);
			this.groupBoxCompany.Controls.Add(this.textBoxHost);
			this.groupBoxCompany.Controls.Add(this.comboBoxIisIPAddress);
			this.groupBoxCompany.Controls.Add(this.labelIisIPAddress);
			this.errorProvider.SetError(this.groupBoxCompany, resources.GetString("groupBoxCompany.Error"));
			this.groupBoxCompany.ForeColor = System.Drawing.Color.DarkBlue;
			this.errorProvider.SetIconAlignment(this.groupBoxCompany, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBoxCompany.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.groupBoxCompany, ((int)(resources.GetObject("groupBoxCompany.IconPadding"))));
			this.groupBoxCompany.Name = "groupBoxCompany";
			this.groupBoxCompany.TabStop = false;
			// 
			// comboBoxIisPool
			// 
			this.comboBoxIisPool.AccessibleDescription = null;
			this.comboBoxIisPool.AccessibleName = null;
			resources.ApplyResources(this.comboBoxIisPool, "comboBoxIisPool");
			this.comboBoxIisPool.BackgroundImage = null;
			this.comboBoxIisPool.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.errorProvider.SetError(this.comboBoxIisPool, resources.GetString("comboBoxIisPool.Error"));
			this.comboBoxIisPool.FormattingEnabled = true;
			this.errorProvider.SetIconAlignment(this.comboBoxIisPool, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("comboBoxIisPool.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.comboBoxIisPool, ((int)(resources.GetObject("comboBoxIisPool.IconPadding"))));
			this.comboBoxIisPool.Name = "comboBoxIisPool";
			// 
			// labelIisPool
			// 
			this.labelIisPool.AccessibleDescription = null;
			this.labelIisPool.AccessibleName = null;
			resources.ApplyResources(this.labelIisPool, "labelIisPool");
			this.errorProvider.SetError(this.labelIisPool, resources.GetString("labelIisPool.Error"));
			this.labelIisPool.ForeColor = System.Drawing.SystemColors.ControlText;
			this.errorProvider.SetIconAlignment(this.labelIisPool, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelIisPool.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.labelIisPool, ((int)(resources.GetObject("labelIisPool.IconPadding"))));
			this.labelIisPool.Name = "labelIisPool";
			// 
			// textBoxIisPort
			// 
			this.textBoxIisPort.AccessibleDescription = null;
			this.textBoxIisPort.AccessibleName = null;
			resources.ApplyResources(this.textBoxIisPort, "textBoxIisPort");
			this.textBoxIisPort.BackgroundImage = null;
			this.errorProvider.SetError(this.textBoxIisPort, resources.GetString("textBoxIisPort.Error"));
			this.errorProvider.SetIconAlignment(this.textBoxIisPort, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxIisPort.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.textBoxIisPort, ((int)(resources.GetObject("textBoxIisPort.IconPadding"))));
			this.textBoxIisPort.Name = "textBoxIisPort";
			this.textBoxIisPort.Validated += new System.EventHandler(this.textBoxIisPort_Validated);
			// 
			// labelHost
			// 
			this.labelHost.AccessibleDescription = null;
			this.labelHost.AccessibleName = null;
			resources.ApplyResources(this.labelHost, "labelHost");
			this.errorProvider.SetError(this.labelHost, resources.GetString("labelHost.Error"));
			this.labelHost.ForeColor = System.Drawing.SystemColors.ControlText;
			this.errorProvider.SetIconAlignment(this.labelHost, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelHost.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.labelHost, ((int)(resources.GetObject("labelHost.IconPadding"))));
			this.labelHost.Name = "labelHost";
			// 
			// labelIisPort
			// 
			this.labelIisPort.AccessibleDescription = null;
			this.labelIisPort.AccessibleName = null;
			resources.ApplyResources(this.labelIisPort, "labelIisPort");
			this.errorProvider.SetError(this.labelIisPort, resources.GetString("labelIisPort.Error"));
			this.labelIisPort.ForeColor = System.Drawing.SystemColors.ControlText;
			this.errorProvider.SetIconAlignment(this.labelIisPort, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelIisPort.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.labelIisPort, ((int)(resources.GetObject("labelIisPort.IconPadding"))));
			this.labelIisPort.Name = "labelIisPort";
			// 
			// textBoxHost
			// 
			this.textBoxHost.AcceptsReturn = true;
			this.textBoxHost.AccessibleDescription = null;
			this.textBoxHost.AccessibleName = null;
			resources.ApplyResources(this.textBoxHost, "textBoxHost");
			this.textBoxHost.BackgroundImage = null;
			this.errorProvider.SetError(this.textBoxHost, resources.GetString("textBoxHost.Error"));
			this.errorProvider.SetIconAlignment(this.textBoxHost, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxHost.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.textBoxHost, ((int)(resources.GetObject("textBoxHost.IconPadding"))));
			this.textBoxHost.Name = "textBoxHost";
			this.textBoxHost.TextChanged += new System.EventHandler(this.textBoxHost_TextChanged);
			this.textBoxHost.Validated += new System.EventHandler(this.textBoxDomainName_Validated);
			// 
			// comboBoxIisIPAddress
			// 
			this.comboBoxIisIPAddress.AccessibleDescription = null;
			this.comboBoxIisIPAddress.AccessibleName = null;
			resources.ApplyResources(this.comboBoxIisIPAddress, "comboBoxIisIPAddress");
			this.comboBoxIisIPAddress.BackgroundImage = null;
			this.comboBoxIisIPAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.errorProvider.SetError(this.comboBoxIisIPAddress, resources.GetString("comboBoxIisIPAddress.Error"));
			this.comboBoxIisIPAddress.FormattingEnabled = true;
			this.errorProvider.SetIconAlignment(this.comboBoxIisIPAddress, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("comboBoxIisIPAddress.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.comboBoxIisIPAddress, ((int)(resources.GetObject("comboBoxIisIPAddress.IconPadding"))));
			this.comboBoxIisIPAddress.Name = "comboBoxIisIPAddress";
			// 
			// labelIisIPAddress
			// 
			this.labelIisIPAddress.AccessibleDescription = null;
			this.labelIisIPAddress.AccessibleName = null;
			resources.ApplyResources(this.labelIisIPAddress, "labelIisIPAddress");
			this.errorProvider.SetError(this.labelIisIPAddress, resources.GetString("labelIisIPAddress.Error"));
			this.labelIisIPAddress.ForeColor = System.Drawing.SystemColors.ControlText;
			this.errorProvider.SetIconAlignment(this.labelIisIPAddress, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelIisIPAddress.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.labelIisIPAddress, ((int)(resources.GetObject("labelIisIPAddress.IconPadding"))));
			this.labelIisIPAddress.Name = "labelIisIPAddress";
			// 
			// buttonCancel
			// 
			this.buttonCancel.AccessibleDescription = null;
			this.buttonCancel.AccessibleName = null;
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.BackgroundImage = null;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.errorProvider.SetError(this.buttonCancel, resources.GetString("buttonCancel.Error"));
			this.buttonCancel.Font = null;
			this.errorProvider.SetIconAlignment(this.buttonCancel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("buttonCancel.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.buttonCancel, ((int)(resources.GetObject("buttonCancel.IconPadding"))));
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOk
			// 
			this.buttonOk.AccessibleDescription = null;
			this.buttonOk.AccessibleName = null;
			resources.ApplyResources(this.buttonOk, "buttonOk");
			this.buttonOk.BackgroundImage = null;
			this.errorProvider.SetError(this.buttonOk, resources.GetString("buttonOk.Error"));
			this.buttonOk.Font = null;
			this.errorProvider.SetIconAlignment(this.buttonOk, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("buttonOk.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.buttonOk, ((int)(resources.GetObject("buttonOk.IconPadding"))));
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// panelHSplit
			// 
			this.panelHSplit.AccessibleDescription = null;
			this.panelHSplit.AccessibleName = null;
			resources.ApplyResources(this.panelHSplit, "panelHSplit");
			this.panelHSplit.BackColor = System.Drawing.Color.Black;
			this.panelHSplit.BackgroundImage = null;
			this.errorProvider.SetError(this.panelHSplit, resources.GetString("panelHSplit.Error"));
			this.panelHSplit.Font = null;
			this.errorProvider.SetIconAlignment(this.panelHSplit, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("panelHSplit.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.panelHSplit, ((int)(resources.GetObject("panelHSplit.IconPadding"))));
			this.panelHSplit.Name = "panelHSplit";
			// 
			// errorProvider
			// 
			this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
			this.errorProvider.ContainerControl = this;
			resources.ApplyResources(this.errorProvider, "errorProvider");
			// 
			// CreateAspForm
			// 
			this.AcceptButton = this.buttonOk;
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = null;
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.panelHSplit);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.mainPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CreateAspForm";
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
		internal System.Windows.Forms.TextBox textBoxHost;
		internal System.Windows.Forms.TextBox textBoxIisPort;
		private System.Windows.Forms.ComboBox comboBoxIisIPAddress;
		private System.Windows.Forms.ComboBox comboBoxIisPool;
		private System.Windows.Forms.Label labelIisPool;
	}
}