namespace Mediachase.Ibn.ConfigurationUI
{
	partial class EditSqlServerSettingsForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditSqlServerSettingsForm));
			this.mainPanel = new System.Windows.Forms.Panel();
			this.groupBoxConnection = new System.Windows.Forms.GroupBox();
			this.linkLabelCheckConnection = new System.Windows.Forms.LinkLabel();
			this.comboBoxAuthentication = new System.Windows.Forms.ComboBox();
			this.labelAuthentication = new System.Windows.Forms.Label();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.labelPassword = new System.Windows.Forms.Label();
			this.textBoxUser = new System.Windows.Forms.TextBox();
			this.labelUser = new System.Windows.Forms.Label();
			this.comboBoxSqlServer = new System.Windows.Forms.ComboBox();
			this.labelSqlServerName = new System.Windows.Forms.Label();
			this.groupBoxIbnLogin = new System.Windows.Forms.GroupBox();
			this.textBoxIbnUser = new System.Windows.Forms.TextBox();
			this.labelIbnUser = new System.Windows.Forms.Label();
			this.textBoxAutoPassword = new System.Windows.Forms.TextBox();
			this.labelIbnUserPasswordRe = new System.Windows.Forms.Label();
			this.textBoxIbnUserPasswordRe = new System.Windows.Forms.TextBox();
			this.labelIbnUserPassword = new System.Windows.Forms.Label();
			this.textBoxIbnUserPassword = new System.Windows.Forms.TextBox();
			this.radioButtonEnterPassManually = new System.Windows.Forms.RadioButton();
			this.radioButtonGenerateRandomPass = new System.Windows.Forms.RadioButton();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.panelHSplit = new System.Windows.Forms.Panel();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.mainPanel.SuspendLayout();
			this.groupBoxConnection.SuspendLayout();
			this.groupBoxIbnLogin.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// mainPanel
			// 
			this.mainPanel.BackColor = System.Drawing.SystemColors.Window;
			this.mainPanel.Controls.Add(this.groupBoxConnection);
			this.mainPanel.Controls.Add(this.groupBoxIbnLogin);
			resources.ApplyResources(this.mainPanel, "mainPanel");
			this.mainPanel.Name = "mainPanel";
			// 
			// groupBoxConnection
			// 
			this.groupBoxConnection.Controls.Add(this.linkLabelCheckConnection);
			this.groupBoxConnection.Controls.Add(this.comboBoxAuthentication);
			this.groupBoxConnection.Controls.Add(this.labelAuthentication);
			this.groupBoxConnection.Controls.Add(this.textBoxPassword);
			this.groupBoxConnection.Controls.Add(this.labelPassword);
			this.groupBoxConnection.Controls.Add(this.textBoxUser);
			this.groupBoxConnection.Controls.Add(this.labelUser);
			this.groupBoxConnection.Controls.Add(this.comboBoxSqlServer);
			this.groupBoxConnection.Controls.Add(this.labelSqlServerName);
			resources.ApplyResources(this.groupBoxConnection, "groupBoxConnection");
			this.groupBoxConnection.ForeColor = System.Drawing.Color.DarkBlue;
			this.groupBoxConnection.Name = "groupBoxConnection";
			this.groupBoxConnection.TabStop = false;
			// 
			// linkLabelCheckConnection
			// 
			resources.ApplyResources(this.linkLabelCheckConnection, "linkLabelCheckConnection");
			this.linkLabelCheckConnection.Name = "linkLabelCheckConnection";
			this.linkLabelCheckConnection.TabStop = true;
			this.linkLabelCheckConnection.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelCheckConnection_LinkClicked);
			// 
			// comboBoxAuthentication
			// 
			this.comboBoxAuthentication.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.comboBoxAuthentication, "comboBoxAuthentication");
			this.comboBoxAuthentication.FormattingEnabled = true;
			this.comboBoxAuthentication.Items.AddRange(new object[] {
            resources.GetString("comboBoxAuthentication.Items"),
            resources.GetString("comboBoxAuthentication.Items1")});
			this.comboBoxAuthentication.Name = "comboBoxAuthentication";
			this.comboBoxAuthentication.SelectedIndexChanged += new System.EventHandler(this.comboBoxAuthentication_SelectedIndexChanged);
			// 
			// labelAuthentication
			// 
			resources.ApplyResources(this.labelAuthentication, "labelAuthentication");
			this.labelAuthentication.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labelAuthentication.Name = "labelAuthentication";
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxPassword, "textBoxPassword");
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.UseSystemPasswordChar = true;
			// 
			// labelPassword
			// 
			resources.ApplyResources(this.labelPassword, "labelPassword");
			this.labelPassword.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labelPassword.Name = "labelPassword";
			// 
			// textBoxUser
			// 
			this.textBoxUser.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxUser, "textBoxUser");
			this.textBoxUser.Name = "textBoxUser";
			// 
			// labelUser
			// 
			resources.ApplyResources(this.labelUser, "labelUser");
			this.labelUser.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labelUser.Name = "labelUser";
			// 
			// comboBoxSqlServer
			// 
			resources.ApplyResources(this.comboBoxSqlServer, "comboBoxSqlServer");
			this.comboBoxSqlServer.FormattingEnabled = true;
			this.comboBoxSqlServer.Name = "comboBoxSqlServer";
			this.comboBoxSqlServer.DropDown += new System.EventHandler(this.comboBoxSqlServer_DropDown);
			// 
			// labelSqlServerName
			// 
			resources.ApplyResources(this.labelSqlServerName, "labelSqlServerName");
			this.labelSqlServerName.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labelSqlServerName.Name = "labelSqlServerName";
			// 
			// groupBoxIbnLogin
			// 
			this.groupBoxIbnLogin.Controls.Add(this.textBoxIbnUser);
			this.groupBoxIbnLogin.Controls.Add(this.labelIbnUser);
			this.groupBoxIbnLogin.Controls.Add(this.textBoxAutoPassword);
			this.groupBoxIbnLogin.Controls.Add(this.labelIbnUserPasswordRe);
			this.groupBoxIbnLogin.Controls.Add(this.textBoxIbnUserPasswordRe);
			this.groupBoxIbnLogin.Controls.Add(this.labelIbnUserPassword);
			this.groupBoxIbnLogin.Controls.Add(this.textBoxIbnUserPassword);
			this.groupBoxIbnLogin.Controls.Add(this.radioButtonEnterPassManually);
			this.groupBoxIbnLogin.Controls.Add(this.radioButtonGenerateRandomPass);
			resources.ApplyResources(this.groupBoxIbnLogin, "groupBoxIbnLogin");
			this.groupBoxIbnLogin.ForeColor = System.Drawing.Color.DarkBlue;
			this.groupBoxIbnLogin.Name = "groupBoxIbnLogin";
			this.groupBoxIbnLogin.TabStop = false;
			// 
			// textBoxIbnUser
			// 
			this.textBoxIbnUser.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxIbnUser, "textBoxIbnUser");
			this.textBoxIbnUser.Name = "textBoxIbnUser";
			this.textBoxIbnUser.Validated += new System.EventHandler(this.textBoxIbnUser_Validated);
			// 
			// labelIbnUser
			// 
			resources.ApplyResources(this.labelIbnUser, "labelIbnUser");
			this.labelIbnUser.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labelIbnUser.Name = "labelIbnUser";
			// 
			// textBoxAutoPassword
			// 
			this.textBoxAutoPassword.AcceptsReturn = true;
			this.textBoxAutoPassword.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxAutoPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.textBoxAutoPassword, "textBoxAutoPassword");
			this.textBoxAutoPassword.Name = "textBoxAutoPassword";
			this.textBoxAutoPassword.ReadOnly = true;
			// 
			// labelIbnUserPasswordRe
			// 
			resources.ApplyResources(this.labelIbnUserPasswordRe, "labelIbnUserPasswordRe");
			this.labelIbnUserPasswordRe.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labelIbnUserPasswordRe.Name = "labelIbnUserPasswordRe";
			// 
			// textBoxIbnUserPasswordRe
			// 
			this.textBoxIbnUserPasswordRe.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxIbnUserPasswordRe, "textBoxIbnUserPasswordRe");
			this.textBoxIbnUserPasswordRe.Name = "textBoxIbnUserPasswordRe";
			this.textBoxIbnUserPasswordRe.UseSystemPasswordChar = true;
			this.textBoxIbnUserPasswordRe.Validated += new System.EventHandler(this.textBoxIbnUserPasswordRe_Validated);
			// 
			// labelIbnUserPassword
			// 
			resources.ApplyResources(this.labelIbnUserPassword, "labelIbnUserPassword");
			this.labelIbnUserPassword.ForeColor = System.Drawing.SystemColors.ControlText;
			this.labelIbnUserPassword.Name = "labelIbnUserPassword";
			// 
			// textBoxIbnUserPassword
			// 
			this.textBoxIbnUserPassword.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxIbnUserPassword, "textBoxIbnUserPassword");
			this.textBoxIbnUserPassword.Name = "textBoxIbnUserPassword";
			this.textBoxIbnUserPassword.UseSystemPasswordChar = true;
			this.textBoxIbnUserPassword.Validated += new System.EventHandler(this.textBoxIbnUserPassword_Validated);
			// 
			// radioButtonEnterPassManually
			// 
			resources.ApplyResources(this.radioButtonEnterPassManually, "radioButtonEnterPassManually");
			this.radioButtonEnterPassManually.Checked = true;
			this.radioButtonEnterPassManually.ForeColor = System.Drawing.SystemColors.ControlText;
			this.radioButtonEnterPassManually.Name = "radioButtonEnterPassManually";
			this.radioButtonEnterPassManually.TabStop = true;
			this.radioButtonEnterPassManually.UseVisualStyleBackColor = true;
			this.radioButtonEnterPassManually.CheckedChanged += new System.EventHandler(this.radioButtonEnterPassManually_CheckedChanged);
			// 
			// radioButtonGenerateRandomPass
			// 
			resources.ApplyResources(this.radioButtonGenerateRandomPass, "radioButtonGenerateRandomPass");
			this.radioButtonGenerateRandomPass.ForeColor = System.Drawing.SystemColors.ControlText;
			this.radioButtonGenerateRandomPass.Name = "radioButtonGenerateRandomPass";
			this.radioButtonGenerateRandomPass.UseVisualStyleBackColor = true;
			this.radioButtonGenerateRandomPass.CheckedChanged += new System.EventHandler(this.radioButtonGenerateRandomPass_CheckedChanged);
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
			// EditSqlServerSettingsForm
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
			this.Name = "EditSqlServerSettingsForm";
			this.ShowInTaskbar = false;
			this.mainPanel.ResumeLayout(false);
			this.groupBoxConnection.ResumeLayout(false);
			this.groupBoxConnection.PerformLayout();
			this.groupBoxIbnLogin.ResumeLayout(false);
			this.groupBoxIbnLogin.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel mainPanel;
		private System.Windows.Forms.GroupBox groupBoxConnection;
		private System.Windows.Forms.GroupBox groupBoxIbnLogin;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Panel panelHSplit;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.ComboBox comboBoxSqlServer;
		private System.Windows.Forms.Label labelSqlServerName;
		internal System.Windows.Forms.TextBox textBoxPassword;
		private System.Windows.Forms.Label labelPassword;
		internal System.Windows.Forms.TextBox textBoxUser;
		private System.Windows.Forms.Label labelUser;
		private System.Windows.Forms.RadioButton radioButtonGenerateRandomPass;
		private System.Windows.Forms.RadioButton radioButtonEnterPassManually;
		private System.Windows.Forms.Label labelIbnUserPasswordRe;
		internal System.Windows.Forms.TextBox textBoxIbnUserPasswordRe;
		private System.Windows.Forms.Label labelIbnUserPassword;
		internal System.Windows.Forms.TextBox textBoxIbnUserPassword;
		private System.Windows.Forms.ComboBox comboBoxAuthentication;
		private System.Windows.Forms.Label labelAuthentication;
		internal System.Windows.Forms.TextBox textBoxAutoPassword;
		private System.Windows.Forms.LinkLabel linkLabelCheckConnection;
		internal System.Windows.Forms.TextBox textBoxIbnUser;
		private System.Windows.Forms.Label labelIbnUser;
	}
}