namespace Mediachase.Ibn.ConfigurationUI
{
	partial class CreateCompanyForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateCompanyForm));
			this.mainPanel = new System.Windows.Forms.Panel();
			this.groupBoxCompany = new System.Windows.Forms.GroupBox();
			this.comboBoxIisPool = new System.Windows.Forms.ComboBox();
			this.labelIisPool = new System.Windows.Forms.Label();
			this.checkBoxIsActive = new System.Windows.Forms.CheckBox();
			this.comboBoxDefaultLanguage = new System.Windows.Forms.ComboBox();
			this.labelDefaultLanguage = new System.Windows.Forms.Label();
			this.labelCompanyName = new System.Windows.Forms.Label();
			this.textBoxCompanyName = new System.Windows.Forms.TextBox();
			this.textBoxIisPort = new System.Windows.Forms.TextBox();
			this.labelHost = new System.Windows.Forms.Label();
			this.labelIisPort = new System.Windows.Forms.Label();
			this.textBoxHost = new System.Windows.Forms.TextBox();
			this.comboBoxIisIPAddress = new System.Windows.Forms.ComboBox();
			this.labelIisIPAddress = new System.Windows.Forms.Label();
			this.groupBoxAdministrator = new System.Windows.Forms.GroupBox();
			this.labelAdminEmail = new System.Windows.Forms.Label();
			this.textBoxAdminEmail = new System.Windows.Forms.TextBox();
			this.labelAdminPasswordRe = new System.Windows.Forms.Label();
			this.textBoxAdminPasswordRe = new System.Windows.Forms.TextBox();
			this.labelAdminPassword = new System.Windows.Forms.Label();
			this.textBoxAdminPassword = new System.Windows.Forms.TextBox();
			this.labelAdminLastName = new System.Windows.Forms.Label();
			this.textBoxAdminLastName = new System.Windows.Forms.TextBox();
			this.labelAdminFirstName = new System.Windows.Forms.Label();
			this.textBoxAdminFirstName = new System.Windows.Forms.TextBox();
			this.labelAdminAccountName = new System.Windows.Forms.Label();
			this.textBoxAdminAccountName = new System.Windows.Forms.TextBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.panelHSplit = new System.Windows.Forms.Panel();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.mainPanel.SuspendLayout();
			this.groupBoxCompany.SuspendLayout();
			this.groupBoxAdministrator.SuspendLayout();
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
			this.mainPanel.Controls.Add(this.groupBoxAdministrator);
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
			this.groupBoxCompany.Controls.Add(this.checkBoxIsActive);
			this.groupBoxCompany.Controls.Add(this.comboBoxDefaultLanguage);
			this.groupBoxCompany.Controls.Add(this.labelDefaultLanguage);
			this.groupBoxCompany.Controls.Add(this.labelCompanyName);
			this.groupBoxCompany.Controls.Add(this.textBoxCompanyName);
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
			// checkBoxIsActive
			// 
			this.checkBoxIsActive.AccessibleDescription = null;
			this.checkBoxIsActive.AccessibleName = null;
			resources.ApplyResources(this.checkBoxIsActive, "checkBoxIsActive");
			this.checkBoxIsActive.BackgroundImage = null;
			this.checkBoxIsActive.Checked = true;
			this.checkBoxIsActive.CheckState = System.Windows.Forms.CheckState.Checked;
			this.errorProvider.SetError(this.checkBoxIsActive, resources.GetString("checkBoxIsActive.Error"));
			this.checkBoxIsActive.ForeColor = System.Drawing.SystemColors.ControlText;
			this.errorProvider.SetIconAlignment(this.checkBoxIsActive, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("checkBoxIsActive.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.checkBoxIsActive, ((int)(resources.GetObject("checkBoxIsActive.IconPadding"))));
			this.checkBoxIsActive.Name = "checkBoxIsActive";
			this.checkBoxIsActive.UseVisualStyleBackColor = true;
			// 
			// comboBoxDefaultLanguage
			// 
			this.comboBoxDefaultLanguage.AccessibleDescription = null;
			this.comboBoxDefaultLanguage.AccessibleName = null;
			resources.ApplyResources(this.comboBoxDefaultLanguage, "comboBoxDefaultLanguage");
			this.comboBoxDefaultLanguage.BackgroundImage = null;
			this.comboBoxDefaultLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.errorProvider.SetError(this.comboBoxDefaultLanguage, resources.GetString("comboBoxDefaultLanguage.Error"));
			this.comboBoxDefaultLanguage.FormattingEnabled = true;
			this.errorProvider.SetIconAlignment(this.comboBoxDefaultLanguage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("comboBoxDefaultLanguage.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.comboBoxDefaultLanguage, ((int)(resources.GetObject("comboBoxDefaultLanguage.IconPadding"))));
			this.comboBoxDefaultLanguage.Name = "comboBoxDefaultLanguage";
			// 
			// labelDefaultLanguage
			// 
			this.labelDefaultLanguage.AccessibleDescription = null;
			this.labelDefaultLanguage.AccessibleName = null;
			resources.ApplyResources(this.labelDefaultLanguage, "labelDefaultLanguage");
			this.errorProvider.SetError(this.labelDefaultLanguage, resources.GetString("labelDefaultLanguage.Error"));
			this.labelDefaultLanguage.ForeColor = System.Drawing.SystemColors.ControlText;
			this.errorProvider.SetIconAlignment(this.labelDefaultLanguage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelDefaultLanguage.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.labelDefaultLanguage, ((int)(resources.GetObject("labelDefaultLanguage.IconPadding"))));
			this.labelDefaultLanguage.Name = "labelDefaultLanguage";
			// 
			// labelCompanyName
			// 
			this.labelCompanyName.AccessibleDescription = null;
			this.labelCompanyName.AccessibleName = null;
			resources.ApplyResources(this.labelCompanyName, "labelCompanyName");
			this.errorProvider.SetError(this.labelCompanyName, resources.GetString("labelCompanyName.Error"));
			this.labelCompanyName.ForeColor = System.Drawing.SystemColors.ControlText;
			this.errorProvider.SetIconAlignment(this.labelCompanyName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelCompanyName.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.labelCompanyName, ((int)(resources.GetObject("labelCompanyName.IconPadding"))));
			this.labelCompanyName.Name = "labelCompanyName";
			// 
			// textBoxCompanyName
			// 
			this.textBoxCompanyName.AccessibleDescription = null;
			this.textBoxCompanyName.AccessibleName = null;
			resources.ApplyResources(this.textBoxCompanyName, "textBoxCompanyName");
			this.textBoxCompanyName.BackgroundImage = null;
			this.errorProvider.SetError(this.textBoxCompanyName, resources.GetString("textBoxCompanyName.Error"));
			this.errorProvider.SetIconAlignment(this.textBoxCompanyName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxCompanyName.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.textBoxCompanyName, ((int)(resources.GetObject("textBoxCompanyName.IconPadding"))));
			this.textBoxCompanyName.Name = "textBoxCompanyName";
			this.textBoxCompanyName.Validated += new System.EventHandler(this.textBoxCompanyName_Validated);
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
			// groupBoxAdministrator
			// 
			this.groupBoxAdministrator.AccessibleDescription = null;
			this.groupBoxAdministrator.AccessibleName = null;
			resources.ApplyResources(this.groupBoxAdministrator, "groupBoxAdministrator");
			this.groupBoxAdministrator.BackgroundImage = null;
			this.groupBoxAdministrator.Controls.Add(this.labelAdminEmail);
			this.groupBoxAdministrator.Controls.Add(this.textBoxAdminEmail);
			this.groupBoxAdministrator.Controls.Add(this.labelAdminPasswordRe);
			this.groupBoxAdministrator.Controls.Add(this.textBoxAdminPasswordRe);
			this.groupBoxAdministrator.Controls.Add(this.labelAdminPassword);
			this.groupBoxAdministrator.Controls.Add(this.textBoxAdminPassword);
			this.groupBoxAdministrator.Controls.Add(this.labelAdminLastName);
			this.groupBoxAdministrator.Controls.Add(this.textBoxAdminLastName);
			this.groupBoxAdministrator.Controls.Add(this.labelAdminFirstName);
			this.groupBoxAdministrator.Controls.Add(this.textBoxAdminFirstName);
			this.groupBoxAdministrator.Controls.Add(this.labelAdminAccountName);
			this.groupBoxAdministrator.Controls.Add(this.textBoxAdminAccountName);
			this.errorProvider.SetError(this.groupBoxAdministrator, resources.GetString("groupBoxAdministrator.Error"));
			this.groupBoxAdministrator.ForeColor = System.Drawing.Color.DarkBlue;
			this.errorProvider.SetIconAlignment(this.groupBoxAdministrator, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBoxAdministrator.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.groupBoxAdministrator, ((int)(resources.GetObject("groupBoxAdministrator.IconPadding"))));
			this.groupBoxAdministrator.Name = "groupBoxAdministrator";
			this.groupBoxAdministrator.TabStop = false;
			// 
			// labelAdminEmail
			// 
			this.labelAdminEmail.AccessibleDescription = null;
			this.labelAdminEmail.AccessibleName = null;
			resources.ApplyResources(this.labelAdminEmail, "labelAdminEmail");
			this.errorProvider.SetError(this.labelAdminEmail, resources.GetString("labelAdminEmail.Error"));
			this.labelAdminEmail.ForeColor = System.Drawing.SystemColors.ControlText;
			this.errorProvider.SetIconAlignment(this.labelAdminEmail, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelAdminEmail.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.labelAdminEmail, ((int)(resources.GetObject("labelAdminEmail.IconPadding"))));
			this.labelAdminEmail.Name = "labelAdminEmail";
			// 
			// textBoxAdminEmail
			// 
			this.textBoxAdminEmail.AccessibleDescription = null;
			this.textBoxAdminEmail.AccessibleName = null;
			resources.ApplyResources(this.textBoxAdminEmail, "textBoxAdminEmail");
			this.textBoxAdminEmail.BackgroundImage = null;
			this.errorProvider.SetError(this.textBoxAdminEmail, resources.GetString("textBoxAdminEmail.Error"));
			this.errorProvider.SetIconAlignment(this.textBoxAdminEmail, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxAdminEmail.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.textBoxAdminEmail, ((int)(resources.GetObject("textBoxAdminEmail.IconPadding"))));
			this.textBoxAdminEmail.Name = "textBoxAdminEmail";
			this.textBoxAdminEmail.TextChanged += new System.EventHandler(this.textBoxAdminEmail_TextChanged);
			this.textBoxAdminEmail.Validated += new System.EventHandler(this.textBoxAdminEmail_Validated);
			// 
			// labelAdminPasswordRe
			// 
			this.labelAdminPasswordRe.AccessibleDescription = null;
			this.labelAdminPasswordRe.AccessibleName = null;
			resources.ApplyResources(this.labelAdminPasswordRe, "labelAdminPasswordRe");
			this.errorProvider.SetError(this.labelAdminPasswordRe, resources.GetString("labelAdminPasswordRe.Error"));
			this.labelAdminPasswordRe.ForeColor = System.Drawing.SystemColors.ControlText;
			this.errorProvider.SetIconAlignment(this.labelAdminPasswordRe, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelAdminPasswordRe.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.labelAdminPasswordRe, ((int)(resources.GetObject("labelAdminPasswordRe.IconPadding"))));
			this.labelAdminPasswordRe.Name = "labelAdminPasswordRe";
			// 
			// textBoxAdminPasswordRe
			// 
			this.textBoxAdminPasswordRe.AcceptsReturn = true;
			this.textBoxAdminPasswordRe.AccessibleDescription = null;
			this.textBoxAdminPasswordRe.AccessibleName = null;
			resources.ApplyResources(this.textBoxAdminPasswordRe, "textBoxAdminPasswordRe");
			this.textBoxAdminPasswordRe.BackgroundImage = null;
			this.errorProvider.SetError(this.textBoxAdminPasswordRe, resources.GetString("textBoxAdminPasswordRe.Error"));
			this.errorProvider.SetIconAlignment(this.textBoxAdminPasswordRe, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxAdminPasswordRe.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.textBoxAdminPasswordRe, ((int)(resources.GetObject("textBoxAdminPasswordRe.IconPadding"))));
			this.textBoxAdminPasswordRe.Name = "textBoxAdminPasswordRe";
			this.textBoxAdminPasswordRe.UseSystemPasswordChar = true;
			this.textBoxAdminPasswordRe.Validated += new System.EventHandler(this.textBoxAdminPasswordRe_Validated);
			// 
			// labelAdminPassword
			// 
			this.labelAdminPassword.AccessibleDescription = null;
			this.labelAdminPassword.AccessibleName = null;
			resources.ApplyResources(this.labelAdminPassword, "labelAdminPassword");
			this.errorProvider.SetError(this.labelAdminPassword, resources.GetString("labelAdminPassword.Error"));
			this.labelAdminPassword.ForeColor = System.Drawing.SystemColors.ControlText;
			this.errorProvider.SetIconAlignment(this.labelAdminPassword, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelAdminPassword.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.labelAdminPassword, ((int)(resources.GetObject("labelAdminPassword.IconPadding"))));
			this.labelAdminPassword.Name = "labelAdminPassword";
			// 
			// textBoxAdminPassword
			// 
			this.textBoxAdminPassword.AcceptsReturn = true;
			this.textBoxAdminPassword.AccessibleDescription = null;
			this.textBoxAdminPassword.AccessibleName = null;
			resources.ApplyResources(this.textBoxAdminPassword, "textBoxAdminPassword");
			this.textBoxAdminPassword.BackgroundImage = null;
			this.errorProvider.SetError(this.textBoxAdminPassword, resources.GetString("textBoxAdminPassword.Error"));
			this.errorProvider.SetIconAlignment(this.textBoxAdminPassword, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxAdminPassword.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.textBoxAdminPassword, ((int)(resources.GetObject("textBoxAdminPassword.IconPadding"))));
			this.textBoxAdminPassword.Name = "textBoxAdminPassword";
			this.textBoxAdminPassword.UseSystemPasswordChar = true;
			this.textBoxAdminPassword.Validated += new System.EventHandler(this.textBoxAdminPassword_Validated);
			// 
			// labelAdminLastName
			// 
			this.labelAdminLastName.AccessibleDescription = null;
			this.labelAdminLastName.AccessibleName = null;
			resources.ApplyResources(this.labelAdminLastName, "labelAdminLastName");
			this.errorProvider.SetError(this.labelAdminLastName, resources.GetString("labelAdminLastName.Error"));
			this.labelAdminLastName.ForeColor = System.Drawing.SystemColors.ControlText;
			this.errorProvider.SetIconAlignment(this.labelAdminLastName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelAdminLastName.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.labelAdminLastName, ((int)(resources.GetObject("labelAdminLastName.IconPadding"))));
			this.labelAdminLastName.Name = "labelAdminLastName";
			// 
			// textBoxAdminLastName
			// 
			this.textBoxAdminLastName.AcceptsReturn = true;
			this.textBoxAdminLastName.AccessibleDescription = null;
			this.textBoxAdminLastName.AccessibleName = null;
			resources.ApplyResources(this.textBoxAdminLastName, "textBoxAdminLastName");
			this.textBoxAdminLastName.BackgroundImage = null;
			this.errorProvider.SetError(this.textBoxAdminLastName, resources.GetString("textBoxAdminLastName.Error"));
			this.errorProvider.SetIconAlignment(this.textBoxAdminLastName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxAdminLastName.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.textBoxAdminLastName, ((int)(resources.GetObject("textBoxAdminLastName.IconPadding"))));
			this.textBoxAdminLastName.Name = "textBoxAdminLastName";
			this.textBoxAdminLastName.Validated += new System.EventHandler(this.textBoxAdminLastName_Validated);
			// 
			// labelAdminFirstName
			// 
			this.labelAdminFirstName.AccessibleDescription = null;
			this.labelAdminFirstName.AccessibleName = null;
			resources.ApplyResources(this.labelAdminFirstName, "labelAdminFirstName");
			this.errorProvider.SetError(this.labelAdminFirstName, resources.GetString("labelAdminFirstName.Error"));
			this.labelAdminFirstName.ForeColor = System.Drawing.SystemColors.ControlText;
			this.errorProvider.SetIconAlignment(this.labelAdminFirstName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelAdminFirstName.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.labelAdminFirstName, ((int)(resources.GetObject("labelAdminFirstName.IconPadding"))));
			this.labelAdminFirstName.Name = "labelAdminFirstName";
			// 
			// textBoxAdminFirstName
			// 
			this.textBoxAdminFirstName.AcceptsReturn = true;
			this.textBoxAdminFirstName.AccessibleDescription = null;
			this.textBoxAdminFirstName.AccessibleName = null;
			resources.ApplyResources(this.textBoxAdminFirstName, "textBoxAdminFirstName");
			this.textBoxAdminFirstName.BackgroundImage = null;
			this.errorProvider.SetError(this.textBoxAdminFirstName, resources.GetString("textBoxAdminFirstName.Error"));
			this.errorProvider.SetIconAlignment(this.textBoxAdminFirstName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxAdminFirstName.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.textBoxAdminFirstName, ((int)(resources.GetObject("textBoxAdminFirstName.IconPadding"))));
			this.textBoxAdminFirstName.Name = "textBoxAdminFirstName";
			this.textBoxAdminFirstName.Validated += new System.EventHandler(this.textBoxAdminFirstName_Validated);
			// 
			// labelAdminAccountName
			// 
			this.labelAdminAccountName.AccessibleDescription = null;
			this.labelAdminAccountName.AccessibleName = null;
			resources.ApplyResources(this.labelAdminAccountName, "labelAdminAccountName");
			this.errorProvider.SetError(this.labelAdminAccountName, resources.GetString("labelAdminAccountName.Error"));
			this.labelAdminAccountName.ForeColor = System.Drawing.SystemColors.ControlText;
			this.errorProvider.SetIconAlignment(this.labelAdminAccountName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelAdminAccountName.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.labelAdminAccountName, ((int)(resources.GetObject("labelAdminAccountName.IconPadding"))));
			this.labelAdminAccountName.Name = "labelAdminAccountName";
			// 
			// textBoxAdminAccountName
			// 
			this.textBoxAdminAccountName.AcceptsReturn = true;
			this.textBoxAdminAccountName.AccessibleDescription = null;
			this.textBoxAdminAccountName.AccessibleName = null;
			resources.ApplyResources(this.textBoxAdminAccountName, "textBoxAdminAccountName");
			this.textBoxAdminAccountName.BackgroundImage = null;
			this.errorProvider.SetError(this.textBoxAdminAccountName, resources.GetString("textBoxAdminAccountName.Error"));
			this.errorProvider.SetIconAlignment(this.textBoxAdminAccountName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxAdminAccountName.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.textBoxAdminAccountName, ((int)(resources.GetObject("textBoxAdminAccountName.IconPadding"))));
			this.textBoxAdminAccountName.Name = "textBoxAdminAccountName";
			this.textBoxAdminAccountName.TextChanged += new System.EventHandler(this.textBoxAdminAccountName_TextChanged);
			this.textBoxAdminAccountName.Validated += new System.EventHandler(this.textBoxAdminAccountName_Validated);
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
			// CreateCompanyForm
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
			this.Name = "CreateCompanyForm";
			this.ShowInTaskbar = false;
			this.mainPanel.ResumeLayout(false);
			this.groupBoxCompany.ResumeLayout(false);
			this.groupBoxCompany.PerformLayout();
			this.groupBoxAdministrator.ResumeLayout(false);
			this.groupBoxAdministrator.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel mainPanel;
		private System.Windows.Forms.Label labelIisIPAddress;
		private System.Windows.Forms.Label labelHost;
		private System.Windows.Forms.Label labelCompanyName;
		private System.Windows.Forms.Label labelIisPort;
		private System.Windows.Forms.Label labelAdminAccountName;
		private System.Windows.Forms.GroupBox groupBoxCompany;
		private System.Windows.Forms.GroupBox groupBoxAdministrator;
		private System.Windows.Forms.Label labelAdminFirstName;
		private System.Windows.Forms.Label labelAdminPasswordRe;
		private System.Windows.Forms.Label labelAdminPassword;
		private System.Windows.Forms.Label labelAdminLastName;
		private System.Windows.Forms.Label labelAdminEmail;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Panel panelHSplit;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.Label labelDefaultLanguage;
		private System.Windows.Forms.ComboBox comboBoxIisIPAddress;
		private System.Windows.Forms.ComboBox comboBoxIisPool;
		private System.Windows.Forms.Label labelIisPool;
		internal System.Windows.Forms.TextBox textBoxHost;
		internal System.Windows.Forms.TextBox textBoxCompanyName;
		internal System.Windows.Forms.TextBox textBoxIisPort;
		internal System.Windows.Forms.TextBox textBoxAdminAccountName;
		internal System.Windows.Forms.TextBox textBoxAdminFirstName;
		internal System.Windows.Forms.TextBox textBoxAdminPasswordRe;
		internal System.Windows.Forms.TextBox textBoxAdminPassword;
		internal System.Windows.Forms.TextBox textBoxAdminLastName;
		internal System.Windows.Forms.TextBox textBoxAdminEmail;
		internal System.Windows.Forms.ComboBox comboBoxDefaultLanguage;
		internal System.Windows.Forms.CheckBox checkBoxIsActive;
	}
}