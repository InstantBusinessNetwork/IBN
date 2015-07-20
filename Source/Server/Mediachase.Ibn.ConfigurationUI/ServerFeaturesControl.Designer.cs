namespace Mediachase.Ibn.ConfigurationUI
{
	partial class ServerFeaturesControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerFeaturesControl));
			this.groupBoxSqlSettings = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanelSqlServerSettings = new System.Windows.Forms.TableLayoutPanel();
			this.textBoxSqlAdminUser = new System.Windows.Forms.TextBox();
			this.textBoxSqlServer = new System.Windows.Forms.TextBox();
			this.labelSqlAdminUser = new System.Windows.Forms.Label();
			this.labelSqlAuthentication = new System.Windows.Forms.Label();
			this.textBoxSqlAuthentication = new System.Windows.Forms.TextBox();
			this.textBoxSqlAccountForIbnPortal = new System.Windows.Forms.TextBox();
			this.labelSqlAccountForIbnPortal = new System.Windows.Forms.Label();
			this.labelSqlServer = new System.Windows.Forms.Label();
			this.buttonEditSqlServerSettings = new System.Windows.Forms.Button();
			this.groupBoxServerSettings = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.textBoxServerName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxIPAddress = new System.Windows.Forms.TextBox();
			this.labelServerName = new System.Windows.Forms.Label();
			this.textBoxTotalCompanies = new System.Windows.Forms.TextBox();
			this.labelTotalCompanies = new System.Windows.Forms.Label();
			this.textBoxCommonComponentVersion = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBoxLicense = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.textBoxLicenseName = new System.Windows.Forms.TextBox();
			this.labelLicenseExpirationDate = new System.Windows.Forms.Label();
			this.textBoxLicenseExpirationDate = new System.Windows.Forms.TextBox();
			this.labelLicenseName = new System.Windows.Forms.Label();
			this.groupBoxAsp = new System.Windows.Forms.GroupBox();
			this.buttonAspConfigure = new System.Windows.Forms.Button();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.textBoxAspDatabase = new System.Windows.Forms.TextBox();
			this.labelAspSiteId = new System.Windows.Forms.Label();
			this.textBoxAspSiteId = new System.Windows.Forms.TextBox();
			this.labelAspDatabase = new System.Windows.Forms.Label();
			this.linkLabelAspUrl = new System.Windows.Forms.LinkLabel();
			this.labelAspUrl = new System.Windows.Forms.Label();
			this.buttonUninstallAsp = new System.Windows.Forms.Button();
			this.buttonInstallAsp = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.logoControl1 = new Mediachase.Ibn.ConfigurationUI.LogoControl();
			this.groupBoxSqlSettings.SuspendLayout();
			this.tableLayoutPanelSqlServerSettings.SuspendLayout();
			this.groupBoxServerSettings.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBoxLicense.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.groupBoxAsp.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBoxSqlSettings
			// 
			this.groupBoxSqlSettings.Controls.Add(this.tableLayoutPanelSqlServerSettings);
			this.groupBoxSqlSettings.Controls.Add(this.buttonEditSqlServerSettings);
			resources.ApplyResources(this.groupBoxSqlSettings, "groupBoxSqlSettings");
			this.groupBoxSqlSettings.Name = "groupBoxSqlSettings";
			this.groupBoxSqlSettings.TabStop = false;
			// 
			// tableLayoutPanelSqlServerSettings
			// 
			resources.ApplyResources(this.tableLayoutPanelSqlServerSettings, "tableLayoutPanelSqlServerSettings");
			this.tableLayoutPanelSqlServerSettings.Controls.Add(this.textBoxSqlAdminUser, 1, 2);
			this.tableLayoutPanelSqlServerSettings.Controls.Add(this.textBoxSqlServer, 1, 0);
			this.tableLayoutPanelSqlServerSettings.Controls.Add(this.labelSqlAdminUser, 0, 2);
			this.tableLayoutPanelSqlServerSettings.Controls.Add(this.labelSqlAuthentication, 0, 1);
			this.tableLayoutPanelSqlServerSettings.Controls.Add(this.textBoxSqlAuthentication, 1, 1);
			this.tableLayoutPanelSqlServerSettings.Controls.Add(this.textBoxSqlAccountForIbnPortal, 1, 3);
			this.tableLayoutPanelSqlServerSettings.Controls.Add(this.labelSqlAccountForIbnPortal, 0, 3);
			this.tableLayoutPanelSqlServerSettings.Controls.Add(this.labelSqlServer, 0, 0);
			this.tableLayoutPanelSqlServerSettings.Name = "tableLayoutPanelSqlServerSettings";
			// 
			// textBoxSqlAdminUser
			// 
			this.textBoxSqlAdminUser.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxSqlAdminUser, "textBoxSqlAdminUser");
			this.textBoxSqlAdminUser.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxSqlAdminUser.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxSqlAdminUser.Name = "textBoxSqlAdminUser";
			this.textBoxSqlAdminUser.ReadOnly = true;
			// 
			// textBoxSqlServer
			// 
			this.textBoxSqlServer.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxSqlServer, "textBoxSqlServer");
			this.textBoxSqlServer.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxSqlServer.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxSqlServer.Name = "textBoxSqlServer";
			this.textBoxSqlServer.ReadOnly = true;
			// 
			// labelSqlAdminUser
			// 
			resources.ApplyResources(this.labelSqlAdminUser, "labelSqlAdminUser");
			this.labelSqlAdminUser.Name = "labelSqlAdminUser";
			// 
			// labelSqlAuthentication
			// 
			resources.ApplyResources(this.labelSqlAuthentication, "labelSqlAuthentication");
			this.labelSqlAuthentication.Name = "labelSqlAuthentication";
			// 
			// textBoxSqlAuthentication
			// 
			this.textBoxSqlAuthentication.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxSqlAuthentication, "textBoxSqlAuthentication");
			this.textBoxSqlAuthentication.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxSqlAuthentication.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxSqlAuthentication.Name = "textBoxSqlAuthentication";
			this.textBoxSqlAuthentication.ReadOnly = true;
			// 
			// textBoxSqlAccountForIbnPortal
			// 
			this.textBoxSqlAccountForIbnPortal.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxSqlAccountForIbnPortal, "textBoxSqlAccountForIbnPortal");
			this.textBoxSqlAccountForIbnPortal.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxSqlAccountForIbnPortal.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxSqlAccountForIbnPortal.Name = "textBoxSqlAccountForIbnPortal";
			this.textBoxSqlAccountForIbnPortal.ReadOnly = true;
			// 
			// labelSqlAccountForIbnPortal
			// 
			resources.ApplyResources(this.labelSqlAccountForIbnPortal, "labelSqlAccountForIbnPortal");
			this.labelSqlAccountForIbnPortal.Name = "labelSqlAccountForIbnPortal";
			// 
			// labelSqlServer
			// 
			resources.ApplyResources(this.labelSqlServer, "labelSqlServer");
			this.labelSqlServer.Name = "labelSqlServer";
			// 
			// buttonEditSqlServerSettings
			// 
			resources.ApplyResources(this.buttonEditSqlServerSettings, "buttonEditSqlServerSettings");
			this.buttonEditSqlServerSettings.BackColor = System.Drawing.SystemColors.Control;
			this.buttonEditSqlServerSettings.Name = "buttonEditSqlServerSettings";
			this.buttonEditSqlServerSettings.UseVisualStyleBackColor = false;
			this.buttonEditSqlServerSettings.Click += new System.EventHandler(this.buttonEditSqlServerSettings_Click);
			// 
			// groupBoxServerSettings
			// 
			this.groupBoxServerSettings.Controls.Add(this.tableLayoutPanel1);
			resources.ApplyResources(this.groupBoxServerSettings, "groupBoxServerSettings");
			this.groupBoxServerSettings.Name = "groupBoxServerSettings";
			this.groupBoxServerSettings.TabStop = false;
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.textBoxServerName, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.textBoxIPAddress, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.labelServerName, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.textBoxTotalCompanies, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.labelTotalCompanies, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.textBoxCommonComponentVersion, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// textBoxServerName
			// 
			this.textBoxServerName.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxServerName, "textBoxServerName");
			this.textBoxServerName.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxServerName.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxServerName.Name = "textBoxServerName";
			this.textBoxServerName.ReadOnly = true;
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// textBoxIPAddress
			// 
			resources.ApplyResources(this.textBoxIPAddress, "textBoxIPAddress");
			this.textBoxIPAddress.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxIPAddress.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxIPAddress.Name = "textBoxIPAddress";
			this.textBoxIPAddress.ReadOnly = true;
			// 
			// labelServerName
			// 
			resources.ApplyResources(this.labelServerName, "labelServerName");
			this.labelServerName.Name = "labelServerName";
			// 
			// textBoxTotalCompanies
			// 
			this.textBoxTotalCompanies.AcceptsReturn = true;
			this.textBoxTotalCompanies.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxTotalCompanies.BorderStyle = System.Windows.Forms.BorderStyle.None;
			resources.ApplyResources(this.textBoxTotalCompanies, "textBoxTotalCompanies");
			this.textBoxTotalCompanies.Name = "textBoxTotalCompanies";
			this.textBoxTotalCompanies.ReadOnly = true;
			// 
			// labelTotalCompanies
			// 
			resources.ApplyResources(this.labelTotalCompanies, "labelTotalCompanies");
			this.labelTotalCompanies.Name = "labelTotalCompanies";
			// 
			// textBoxCommonComponentVersion
			// 
			this.textBoxCommonComponentVersion.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxCommonComponentVersion, "textBoxCommonComponentVersion");
			this.textBoxCommonComponentVersion.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxCommonComponentVersion.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxCommonComponentVersion.Name = "textBoxCommonComponentVersion";
			this.textBoxCommonComponentVersion.ReadOnly = true;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// groupBoxLicense
			// 
			this.groupBoxLicense.Controls.Add(this.tableLayoutPanel2);
			resources.ApplyResources(this.groupBoxLicense, "groupBoxLicense");
			this.groupBoxLicense.Name = "groupBoxLicense";
			this.groupBoxLicense.TabStop = false;
			// 
			// tableLayoutPanel2
			// 
			resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
			this.tableLayoutPanel2.Controls.Add(this.textBoxLicenseName, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.labelLicenseExpirationDate, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.textBoxLicenseExpirationDate, 1, 1);
			this.tableLayoutPanel2.Controls.Add(this.labelLicenseName, 0, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			// 
			// textBoxLicenseName
			// 
			this.textBoxLicenseName.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxLicenseName, "textBoxLicenseName");
			this.textBoxLicenseName.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxLicenseName.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxLicenseName.Name = "textBoxLicenseName";
			this.textBoxLicenseName.ReadOnly = true;
			// 
			// labelLicenseExpirationDate
			// 
			resources.ApplyResources(this.labelLicenseExpirationDate, "labelLicenseExpirationDate");
			this.labelLicenseExpirationDate.Name = "labelLicenseExpirationDate";
			// 
			// textBoxLicenseExpirationDate
			// 
			this.textBoxLicenseExpirationDate.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxLicenseExpirationDate, "textBoxLicenseExpirationDate");
			this.textBoxLicenseExpirationDate.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxLicenseExpirationDate.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxLicenseExpirationDate.Name = "textBoxLicenseExpirationDate";
			this.textBoxLicenseExpirationDate.ReadOnly = true;
			// 
			// labelLicenseName
			// 
			resources.ApplyResources(this.labelLicenseName, "labelLicenseName");
			this.labelLicenseName.Name = "labelLicenseName";
			// 
			// groupBoxAsp
			// 
			this.groupBoxAsp.Controls.Add(this.buttonAspConfigure);
			this.groupBoxAsp.Controls.Add(this.tableLayoutPanel3);
			this.groupBoxAsp.Controls.Add(this.linkLabelAspUrl);
			this.groupBoxAsp.Controls.Add(this.labelAspUrl);
			this.groupBoxAsp.Controls.Add(this.buttonUninstallAsp);
			this.groupBoxAsp.Controls.Add(this.buttonInstallAsp);
			this.groupBoxAsp.Controls.Add(this.pictureBox1);
			resources.ApplyResources(this.groupBoxAsp, "groupBoxAsp");
			this.groupBoxAsp.Name = "groupBoxAsp";
			this.groupBoxAsp.TabStop = false;
			// 
			// buttonAspConfigure
			// 
			resources.ApplyResources(this.buttonAspConfigure, "buttonAspConfigure");
			this.buttonAspConfigure.BackColor = System.Drawing.SystemColors.Control;
			this.buttonAspConfigure.Name = "buttonAspConfigure";
			this.buttonAspConfigure.UseVisualStyleBackColor = false;
			this.buttonAspConfigure.Click += new System.EventHandler(this.buttonAspConfigure_Click);
			// 
			// tableLayoutPanel3
			// 
			resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
			this.tableLayoutPanel3.Controls.Add(this.textBoxAspDatabase, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this.labelAspSiteId, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this.textBoxAspSiteId, 1, 1);
			this.tableLayoutPanel3.Controls.Add(this.labelAspDatabase, 0, 0);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			// 
			// textBoxAspDatabase
			// 
			this.textBoxAspDatabase.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxAspDatabase, "textBoxAspDatabase");
			this.textBoxAspDatabase.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxAspDatabase.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxAspDatabase.Name = "textBoxAspDatabase";
			this.textBoxAspDatabase.ReadOnly = true;
			// 
			// labelAspSiteId
			// 
			resources.ApplyResources(this.labelAspSiteId, "labelAspSiteId");
			this.labelAspSiteId.Name = "labelAspSiteId";
			// 
			// textBoxAspSiteId
			// 
			this.textBoxAspSiteId.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxAspSiteId, "textBoxAspSiteId");
			this.textBoxAspSiteId.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxAspSiteId.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxAspSiteId.Name = "textBoxAspSiteId";
			this.textBoxAspSiteId.ReadOnly = true;
			// 
			// labelAspDatabase
			// 
			this.labelAspDatabase.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			resources.ApplyResources(this.labelAspDatabase, "labelAspDatabase");
			this.labelAspDatabase.Name = "labelAspDatabase";
			// 
			// linkLabelAspUrl
			// 
			resources.ApplyResources(this.linkLabelAspUrl, "linkLabelAspUrl");
			this.linkLabelAspUrl.Name = "linkLabelAspUrl";
			this.linkLabelAspUrl.TabStop = true;
			this.linkLabelAspUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelAspUrl_LinkClicked);
			// 
			// labelAspUrl
			// 
			resources.ApplyResources(this.labelAspUrl, "labelAspUrl");
			this.labelAspUrl.Name = "labelAspUrl";
			// 
			// buttonUninstallAsp
			// 
			resources.ApplyResources(this.buttonUninstallAsp, "buttonUninstallAsp");
			this.buttonUninstallAsp.BackColor = System.Drawing.SystemColors.Control;
			this.buttonUninstallAsp.Name = "buttonUninstallAsp";
			this.buttonUninstallAsp.UseVisualStyleBackColor = false;
			this.buttonUninstallAsp.Click += new System.EventHandler(this.buttonUninstallAsp_Click);
			// 
			// buttonInstallAsp
			// 
			resources.ApplyResources(this.buttonInstallAsp, "buttonInstallAsp");
			this.buttonInstallAsp.BackColor = System.Drawing.SystemColors.Control;
			this.buttonInstallAsp.Name = "buttonInstallAsp";
			this.buttonInstallAsp.UseVisualStyleBackColor = false;
			this.buttonInstallAsp.Click += new System.EventHandler(this.buttonInstallAsp_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.Asp;
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// logoControl1
			// 
			this.logoControl1.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.logoControl1, "logoControl1");
			this.logoControl1.Name = "logoControl1";
			// 
			// ServerFeaturesControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.groupBoxAsp);
			this.Controls.Add(this.groupBoxSqlSettings);
			this.Controls.Add(this.groupBoxLicense);
			this.Controls.Add(this.groupBoxServerSettings);
			this.Controls.Add(this.logoControl1);
			this.DoubleBuffered = true;
			this.Name = "ServerFeaturesControl";
			this.groupBoxSqlSettings.ResumeLayout(false);
			this.tableLayoutPanelSqlServerSettings.ResumeLayout(false);
			this.tableLayoutPanelSqlServerSettings.PerformLayout();
			this.groupBoxServerSettings.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.groupBoxLicense.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.groupBoxAsp.ResumeLayout(false);
			this.groupBoxAsp.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxSqlSettings;
		private System.Windows.Forms.Button buttonEditSqlServerSettings;
		private System.Windows.Forms.GroupBox groupBoxServerSettings;
		private System.Windows.Forms.Label labelSqlAccountForIbnPortal;
		private System.Windows.Forms.TextBox textBoxSqlAccountForIbnPortal;
		private LogoControl logoControl1;
		private System.Windows.Forms.TextBox textBoxSqlAdminUser;
		private System.Windows.Forms.Label labelSqlAdminUser;
		private System.Windows.Forms.TextBox textBoxSqlAuthentication;
		private System.Windows.Forms.Label labelSqlAuthentication;
		private System.Windows.Forms.TextBox textBoxSqlServer;
		private System.Windows.Forms.Label labelSqlServer;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSqlServerSettings;
		private System.Windows.Forms.GroupBox groupBoxLicense;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TextBox textBoxServerName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxIPAddress;
		private System.Windows.Forms.Label labelServerName;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.TextBox textBoxLicenseName;
		private System.Windows.Forms.Label labelLicenseExpirationDate;
		private System.Windows.Forms.TextBox textBoxLicenseExpirationDate;
		private System.Windows.Forms.Label labelLicenseName;
		private System.Windows.Forms.TextBox textBoxTotalCompanies;
		private System.Windows.Forms.Label labelTotalCompanies;
		private System.Windows.Forms.GroupBox groupBoxAsp;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button buttonInstallAsp;
		private System.Windows.Forms.Button buttonUninstallAsp;
		private System.Windows.Forms.LinkLabel linkLabelAspUrl;
		private System.Windows.Forms.Label labelAspUrl;
		private System.Windows.Forms.Button buttonAspConfigure;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.TextBox textBoxAspDatabase;
		private System.Windows.Forms.Label labelAspSiteId;
		private System.Windows.Forms.TextBox textBoxAspSiteId;
		private System.Windows.Forms.Label labelAspDatabase;
		private System.Windows.Forms.TextBox textBoxCommonComponentVersion;
		private System.Windows.Forms.Label label1;
	}
}
