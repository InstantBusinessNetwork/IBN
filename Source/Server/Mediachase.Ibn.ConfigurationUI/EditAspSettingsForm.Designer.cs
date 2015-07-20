namespace Mediachase.Ibn.ConfigurationUI
{
	partial class EditAspSettingsForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditAspSettingsForm));
			this.mainPanel = new System.Windows.Forms.Panel();
			this.groupBoxAddress = new System.Windows.Forms.GroupBox();
			this.textBoxIisPort = new System.Windows.Forms.TextBox();
			this.labelPort = new System.Windows.Forms.Label();
			this.comboBoxSchema = new System.Windows.Forms.ComboBox();
			this.labelScheme = new System.Windows.Forms.Label();
			this.labelHost = new System.Windows.Forms.Label();
			this.textHostName = new System.Windows.Forms.TextBox();
			this.groupBoxIisApplicationPool = new System.Windows.Forms.GroupBox();
			this.comboBoxIisPool = new System.Windows.Forms.ComboBox();
			this.labelIisPool = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.panelHSplit = new System.Windows.Forms.Panel();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.mainPanel.SuspendLayout();
			this.groupBoxAddress.SuspendLayout();
			this.groupBoxIisApplicationPool.SuspendLayout();
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
			this.mainPanel.Controls.Add(this.groupBoxAddress);
			this.mainPanel.Controls.Add(this.groupBoxIisApplicationPool);
			this.errorProvider.SetError(this.mainPanel, resources.GetString("mainPanel.Error"));
			this.mainPanel.Font = null;
			this.errorProvider.SetIconAlignment(this.mainPanel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mainPanel.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.mainPanel, ((int)(resources.GetObject("mainPanel.IconPadding"))));
			this.mainPanel.Name = "mainPanel";
			// 
			// groupBoxAddress
			// 
			this.groupBoxAddress.AccessibleDescription = null;
			this.groupBoxAddress.AccessibleName = null;
			resources.ApplyResources(this.groupBoxAddress, "groupBoxAddress");
			this.groupBoxAddress.BackgroundImage = null;
			this.groupBoxAddress.Controls.Add(this.textBoxIisPort);
			this.groupBoxAddress.Controls.Add(this.labelPort);
			this.groupBoxAddress.Controls.Add(this.comboBoxSchema);
			this.groupBoxAddress.Controls.Add(this.labelScheme);
			this.groupBoxAddress.Controls.Add(this.labelHost);
			this.groupBoxAddress.Controls.Add(this.textHostName);
			this.errorProvider.SetError(this.groupBoxAddress, resources.GetString("groupBoxAddress.Error"));
			this.groupBoxAddress.ForeColor = System.Drawing.Color.DarkBlue;
			this.errorProvider.SetIconAlignment(this.groupBoxAddress, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBoxAddress.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.groupBoxAddress, ((int)(resources.GetObject("groupBoxAddress.IconPadding"))));
			this.groupBoxAddress.Name = "groupBoxAddress";
			this.groupBoxAddress.TabStop = false;
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
			// labelPort
			// 
			this.labelPort.AccessibleDescription = null;
			this.labelPort.AccessibleName = null;
			resources.ApplyResources(this.labelPort, "labelPort");
			this.errorProvider.SetError(this.labelPort, resources.GetString("labelPort.Error"));
			this.labelPort.ForeColor = System.Drawing.SystemColors.ControlText;
			this.errorProvider.SetIconAlignment(this.labelPort, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelPort.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.labelPort, ((int)(resources.GetObject("labelPort.IconPadding"))));
			this.labelPort.Name = "labelPort";
			// 
			// comboBoxSchema
			// 
			this.comboBoxSchema.AccessibleDescription = null;
			this.comboBoxSchema.AccessibleName = null;
			resources.ApplyResources(this.comboBoxSchema, "comboBoxSchema");
			this.comboBoxSchema.BackgroundImage = null;
			this.comboBoxSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.errorProvider.SetError(this.comboBoxSchema, resources.GetString("comboBoxSchema.Error"));
			this.comboBoxSchema.FormattingEnabled = true;
			this.errorProvider.SetIconAlignment(this.comboBoxSchema, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("comboBoxSchema.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.comboBoxSchema, ((int)(resources.GetObject("comboBoxSchema.IconPadding"))));
			this.comboBoxSchema.Items.AddRange(new object[] {
            resources.GetString("comboBoxSchema.Items"),
            resources.GetString("comboBoxSchema.Items1")});
			this.comboBoxSchema.Name = "comboBoxSchema";
			// 
			// labelScheme
			// 
			this.labelScheme.AccessibleDescription = null;
			this.labelScheme.AccessibleName = null;
			this.labelScheme.AllowDrop = true;
			resources.ApplyResources(this.labelScheme, "labelScheme");
			this.errorProvider.SetError(this.labelScheme, resources.GetString("labelScheme.Error"));
			this.labelScheme.ForeColor = System.Drawing.SystemColors.ControlText;
			this.errorProvider.SetIconAlignment(this.labelScheme, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelScheme.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.labelScheme, ((int)(resources.GetObject("labelScheme.IconPadding"))));
			this.labelScheme.Name = "labelScheme";
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
			// textHostName
			// 
			this.textHostName.AcceptsTab = true;
			this.textHostName.AccessibleDescription = null;
			this.textHostName.AccessibleName = null;
			resources.ApplyResources(this.textHostName, "textHostName");
			this.textHostName.BackgroundImage = null;
			this.errorProvider.SetError(this.textHostName, resources.GetString("textHostName.Error"));
			this.errorProvider.SetIconAlignment(this.textHostName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textHostName.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.textHostName, ((int)(resources.GetObject("textHostName.IconPadding"))));
			this.textHostName.Name = "textHostName";
			this.textHostName.Validated += new System.EventHandler(this.textHostName_Validated);
			// 
			// groupBoxIisApplicationPool
			// 
			this.groupBoxIisApplicationPool.AccessibleDescription = null;
			this.groupBoxIisApplicationPool.AccessibleName = null;
			resources.ApplyResources(this.groupBoxIisApplicationPool, "groupBoxIisApplicationPool");
			this.groupBoxIisApplicationPool.BackgroundImage = null;
			this.groupBoxIisApplicationPool.Controls.Add(this.comboBoxIisPool);
			this.groupBoxIisApplicationPool.Controls.Add(this.labelIisPool);
			this.errorProvider.SetError(this.groupBoxIisApplicationPool, resources.GetString("groupBoxIisApplicationPool.Error"));
			this.groupBoxIisApplicationPool.ForeColor = System.Drawing.Color.DarkBlue;
			this.errorProvider.SetIconAlignment(this.groupBoxIisApplicationPool, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBoxIisApplicationPool.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.groupBoxIisApplicationPool, ((int)(resources.GetObject("groupBoxIisApplicationPool.IconPadding"))));
			this.groupBoxIisApplicationPool.Name = "groupBoxIisApplicationPool";
			this.groupBoxIisApplicationPool.TabStop = false;
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
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
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
			// EditAspSettingsForm
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
			this.Name = "EditAspSettingsForm";
			this.ShowInTaskbar = false;
			this.mainPanel.ResumeLayout(false);
			this.groupBoxAddress.ResumeLayout(false);
			this.groupBoxAddress.PerformLayout();
			this.groupBoxIisApplicationPool.ResumeLayout(false);
			this.groupBoxIisApplicationPool.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel mainPanel;
		private System.Windows.Forms.GroupBox groupBoxAddress;
		private System.Windows.Forms.GroupBox groupBoxIisApplicationPool;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Panel panelHSplit;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.TextBox textBoxIisPort;
		private System.Windows.Forms.Label labelPort;
		private System.Windows.Forms.ComboBox comboBoxSchema;
		private System.Windows.Forms.Label labelScheme;
		private System.Windows.Forms.Label labelHost;
		private System.Windows.Forms.TextBox textHostName;
		internal System.Windows.Forms.ComboBox comboBoxIisPool;
		private System.Windows.Forms.Label labelIisPool;
	}
}