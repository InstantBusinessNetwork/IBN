using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

using Mediachase.Ibn.Converter;

namespace IbnConverterGui
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonConvert;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabPageSettings;
		private System.Windows.Forms.TabPage tabPageLog;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textBoxLog;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Button buttonCopySettings;
		private System.Windows.Forms.Button buttonClose;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.Windows.Forms.TextBox textSourceServer;
		private System.Windows.Forms.ComboBox comboSourceDatabase;
		private System.Windows.Forms.ComboBox comboSourceCompany;

		private System.Windows.Forms.TextBox textTargetServer;
		private System.Windows.Forms.ComboBox comboTargetDatabase;

		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox textBoxSourceSqlPassword;
		private System.Windows.Forms.TextBox textBoxSourceSqlUser;
		private System.Windows.Forms.GroupBox groupTarget;
		private System.Windows.Forms.GroupBox groupSource;
		private System.Windows.Forms.TextBox textBoxTargetSqlPassword;
		private System.Windows.Forms.TextBox textBoxTargetSqlUser;
		private System.Windows.Forms.CheckBox checkBoxTargetUseTrustedConnection;
		private System.Windows.Forms.CheckBox checkBoxSourceUseTrustedConnection;
		
		private IbnConverter _converter;
		private bool m_InProgress;
		private bool m_Cancel;
		private System.Windows.Forms.Panel panel1;

		private bool m_SourceDatabaseListPopulated;
		private bool m_TargetDatabaseListPopulated;
		private bool m_SourceCompanyListPopulated;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_converter  = new IbnConverter(
				int.Parse(ConfigurationManager.AppSettings["SqlCommandTimeout"]),
				int.Parse(ConfigurationManager.AppSettings["BinaryBufferSize"]));

			_converter.Progress += new EventHandler<ConverterEventArgs>(this.OnProgress);
			_converter.Warning += new EventHandler<ConverterEventArgs>(this.OnWarning);
			_converter.Completed += new EventHandler<ConverterEventArgs>(this.OnCompleted);
		}

		public delegate void OnDtsErrorMethod(string eventSource, int errorCode, string source, string description);
		public delegate void LogWriteLineMethod(string message);
		public delegate void ConversionCompletedMethod(string message, MessageBoxIcon icon);

		//private IAsyncResult _conversionAsyncResult = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.buttonConvert = new System.Windows.Forms.Button();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageSettings = new System.Windows.Forms.TabPage();
			this.groupTarget = new System.Windows.Forms.GroupBox();
			this.textTargetServer = new System.Windows.Forms.TextBox();
			this.textBoxTargetSqlPassword = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textBoxTargetSqlUser = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.checkBoxTargetUseTrustedConnection = new System.Windows.Forms.CheckBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.comboTargetDatabase = new System.Windows.Forms.ComboBox();
			this.groupSource = new System.Windows.Forms.GroupBox();
			this.textSourceServer = new System.Windows.Forms.TextBox();
			this.comboSourceDatabase = new System.Windows.Forms.ComboBox();
			this.buttonCopySettings = new System.Windows.Forms.Button();
			this.comboSourceCompany = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textBoxSourceSqlPassword = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxSourceSqlUser = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkBoxSourceUseTrustedConnection = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPageLog = new System.Windows.Forms.TabPage();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.textBoxLog = new System.Windows.Forms.TextBox();
			this.buttonClose = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tabControl.SuspendLayout();
			this.tabPageSettings.SuspendLayout();
			this.groupTarget.SuspendLayout();
			this.groupSource.SuspendLayout();
			this.tabPageLog.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonConvert
			// 
			this.buttonConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonConvert.Location = new System.Drawing.Point(416, 8);
			this.buttonConvert.Name = "buttonConvert";
			this.buttonConvert.Size = new System.Drawing.Size(75, 23);
			this.buttonConvert.TabIndex = 0;
			this.buttonConvert.Text = "Convert";
			this.buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tabPageSettings);
			this.tabControl.Controls.Add(this.tabPageLog);
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(576, 326);
			this.tabControl.TabIndex = 0;
			// 
			// tabPageSettings
			// 
			this.tabPageSettings.Controls.Add(this.groupTarget);
			this.tabPageSettings.Controls.Add(this.groupSource);
			this.tabPageSettings.Location = new System.Drawing.Point(4, 22);
			this.tabPageSettings.Name = "tabPageSettings";
			this.tabPageSettings.Size = new System.Drawing.Size(568, 300);
			this.tabPageSettings.TabIndex = 0;
			this.tabPageSettings.Text = "Settings";
			// 
			// groupTarget
			// 
			this.groupTarget.Controls.Add(this.textTargetServer);
			this.groupTarget.Controls.Add(this.textBoxTargetSqlPassword);
			this.groupTarget.Controls.Add(this.label7);
			this.groupTarget.Controls.Add(this.textBoxTargetSqlUser);
			this.groupTarget.Controls.Add(this.label8);
			this.groupTarget.Controls.Add(this.checkBoxTargetUseTrustedConnection);
			this.groupTarget.Controls.Add(this.label9);
			this.groupTarget.Controls.Add(this.label10);
			this.groupTarget.Controls.Add(this.comboTargetDatabase);
			this.groupTarget.Location = new System.Drawing.Point(288, 0);
			this.groupTarget.Name = "groupTarget";
			this.groupTarget.Size = new System.Drawing.Size(272, 280);
			this.groupTarget.TabIndex = 1;
			this.groupTarget.TabStop = false;
			this.groupTarget.Text = "IBN 4.7 Target Company ";
			// 
			// textTargetServer
			// 
			this.textTargetServer.Location = new System.Drawing.Point(8, 40);
			this.textTargetServer.Name = "textTargetServer";
			this.textTargetServer.Size = new System.Drawing.Size(256, 20);
			this.textTargetServer.TabIndex = 0;
			this.textTargetServer.Text = "(local)";
			this.textTargetServer.TextChanged += new System.EventHandler(this.textTargetServer_TextChanged);
			// 
			// textBoxTargetSqlPassword
			// 
			this.textBoxTargetSqlPassword.Location = new System.Drawing.Point(8, 152);
			this.textBoxTargetSqlPassword.Name = "textBoxTargetSqlPassword";
			this.textBoxTargetSqlPassword.PasswordChar = '*';
			this.textBoxTargetSqlPassword.Size = new System.Drawing.Size(256, 20);
			this.textBoxTargetSqlPassword.TabIndex = 3;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 136);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(248, 16);
			this.label7.TabIndex = 18;
			this.label7.Text = "Sql Password:";
			// 
			// textBoxTargetSqlUser
			// 
			this.textBoxTargetSqlUser.Location = new System.Drawing.Point(8, 112);
			this.textBoxTargetSqlUser.Name = "textBoxTargetSqlUser";
			this.textBoxTargetSqlUser.Size = new System.Drawing.Size(256, 20);
			this.textBoxTargetSqlUser.TabIndex = 2;
			this.textBoxTargetSqlUser.Text = "sa";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(8, 96);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(248, 16);
			this.label8.TabIndex = 16;
			this.label8.Text = "Sql User:";
			// 
			// checkBoxTargetUseTrustedConnection
			// 
			this.checkBoxTargetUseTrustedConnection.Checked = true;
			this.checkBoxTargetUseTrustedConnection.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxTargetUseTrustedConnection.Location = new System.Drawing.Point(8, 72);
			this.checkBoxTargetUseTrustedConnection.Name = "checkBoxTargetUseTrustedConnection";
			this.checkBoxTargetUseTrustedConnection.Size = new System.Drawing.Size(256, 16);
			this.checkBoxTargetUseTrustedConnection.TabIndex = 1;
			this.checkBoxTargetUseTrustedConnection.Text = "Use Trusted Connection";
			this.checkBoxTargetUseTrustedConnection.CheckedChanged += new System.EventHandler(this.checkBoxTargetUseTrustedConnection_CheckedChanged);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(8, 184);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(256, 16);
			this.label9.TabIndex = 13;
			this.label9.Text = "IBN Portal Database:";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(8, 24);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(100, 16);
			this.label10.TabIndex = 11;
			this.label10.Text = "Sql Server:";
			// 
			// comboTargetDatabase
			// 
			this.comboTargetDatabase.Items.AddRange(new object[] {
            "IBN47"});
			this.comboTargetDatabase.Location = new System.Drawing.Point(8, 200);
			this.comboTargetDatabase.Name = "comboTargetDatabase";
			this.comboTargetDatabase.Size = new System.Drawing.Size(256, 21);
			this.comboTargetDatabase.Sorted = true;
			this.comboTargetDatabase.TabIndex = 4;
			this.comboTargetDatabase.DropDown += new System.EventHandler(this.comboTargetDatabase_DropDown);
			// 
			// groupSource
			// 
			this.groupSource.Controls.Add(this.textSourceServer);
			this.groupSource.Controls.Add(this.comboSourceDatabase);
			this.groupSource.Controls.Add(this.buttonCopySettings);
			this.groupSource.Controls.Add(this.comboSourceCompany);
			this.groupSource.Controls.Add(this.label5);
			this.groupSource.Controls.Add(this.textBoxSourceSqlPassword);
			this.groupSource.Controls.Add(this.label4);
			this.groupSource.Controls.Add(this.textBoxSourceSqlUser);
			this.groupSource.Controls.Add(this.label3);
			this.groupSource.Controls.Add(this.checkBoxSourceUseTrustedConnection);
			this.groupSource.Controls.Add(this.label2);
			this.groupSource.Controls.Add(this.label1);
			this.groupSource.Location = new System.Drawing.Point(8, 0);
			this.groupSource.Name = "groupSource";
			this.groupSource.Size = new System.Drawing.Size(272, 280);
			this.groupSource.TabIndex = 0;
			this.groupSource.TabStop = false;
			this.groupSource.Text = "IBN 4.5 Source Company ";
			// 
			// textSourceServer
			// 
			this.textSourceServer.Location = new System.Drawing.Point(8, 40);
			this.textSourceServer.Name = "textSourceServer";
			this.textSourceServer.Size = new System.Drawing.Size(256, 20);
			this.textSourceServer.TabIndex = 0;
			this.textSourceServer.Text = "(local)";
			this.textSourceServer.TextChanged += new System.EventHandler(this.textSourceServer_TextChanged);
			// 
			// comboSourceDatabase
			// 
			this.comboSourceDatabase.Items.AddRange(new object[] {
            "IBN45"});
			this.comboSourceDatabase.Location = new System.Drawing.Point(8, 200);
			this.comboSourceDatabase.Name = "comboSourceDatabase";
			this.comboSourceDatabase.Size = new System.Drawing.Size(256, 21);
			this.comboSourceDatabase.Sorted = true;
			this.comboSourceDatabase.TabIndex = 4;
			this.comboSourceDatabase.SelectedIndexChanged += new System.EventHandler(this.comboSourceDatabase_SelectedIndexChanged);
			this.comboSourceDatabase.DropDown += new System.EventHandler(this.comboSourceDatabase_DropDown);
			// 
			// buttonCopySettings
			// 
			this.buttonCopySettings.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonCopySettings.Location = new System.Drawing.Point(253, 8);
			this.buttonCopySettings.Name = "buttonCopySettings";
			this.buttonCopySettings.Size = new System.Drawing.Size(16, 18);
			this.buttonCopySettings.TabIndex = 10;
			this.buttonCopySettings.Text = ">";
			this.buttonCopySettings.Click += new System.EventHandler(this.buttonCopySettings_Click);
			// 
			// comboSourceCompany
			// 
			this.comboSourceCompany.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSourceCompany.Location = new System.Drawing.Point(8, 248);
			this.comboSourceCompany.Name = "comboSourceCompany";
			this.comboSourceCompany.Size = new System.Drawing.Size(256, 21);
			this.comboSourceCompany.Sorted = true;
			this.comboSourceCompany.TabIndex = 5;
			this.comboSourceCompany.DropDown += new System.EventHandler(this.comboSourceCompany_DropDown);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 232);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(256, 16);
			this.label5.TabIndex = 9;
			this.label5.Text = "IBN Company:";
			// 
			// textBoxSourceSqlPassword
			// 
			this.textBoxSourceSqlPassword.Location = new System.Drawing.Point(8, 152);
			this.textBoxSourceSqlPassword.Name = "textBoxSourceSqlPassword";
			this.textBoxSourceSqlPassword.PasswordChar = '*';
			this.textBoxSourceSqlPassword.Size = new System.Drawing.Size(256, 20);
			this.textBoxSourceSqlPassword.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 136);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(248, 16);
			this.label4.TabIndex = 7;
			this.label4.Text = "Sql Password:";
			// 
			// textBoxSourceSqlUser
			// 
			this.textBoxSourceSqlUser.Location = new System.Drawing.Point(8, 112);
			this.textBoxSourceSqlUser.Name = "textBoxSourceSqlUser";
			this.textBoxSourceSqlUser.Size = new System.Drawing.Size(256, 20);
			this.textBoxSourceSqlUser.TabIndex = 2;
			this.textBoxSourceSqlUser.Text = "sa";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 96);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(248, 16);
			this.label3.TabIndex = 5;
			this.label3.Text = "Sql User:";
			// 
			// checkBoxSourceUseTrustedConnection
			// 
			this.checkBoxSourceUseTrustedConnection.Checked = true;
			this.checkBoxSourceUseTrustedConnection.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxSourceUseTrustedConnection.Location = new System.Drawing.Point(8, 72);
			this.checkBoxSourceUseTrustedConnection.Name = "checkBoxSourceUseTrustedConnection";
			this.checkBoxSourceUseTrustedConnection.Size = new System.Drawing.Size(256, 16);
			this.checkBoxSourceUseTrustedConnection.TabIndex = 1;
			this.checkBoxSourceUseTrustedConnection.Text = "Use Trusted Connection";
			this.checkBoxSourceUseTrustedConnection.CheckedChanged += new System.EventHandler(this.checkBoxSourceUseTrustedConnection_CheckedChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 184);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(248, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "IBN Main Database:";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Sql Server:";
			// 
			// tabPageLog
			// 
			this.tabPageLog.Controls.Add(this.progressBar);
			this.tabPageLog.Controls.Add(this.textBoxLog);
			this.tabPageLog.Location = new System.Drawing.Point(4, 22);
			this.tabPageLog.Name = "tabPageLog";
			this.tabPageLog.Size = new System.Drawing.Size(568, 300);
			this.tabPageLog.TabIndex = 1;
			this.tabPageLog.Text = "Log";
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(4, 293);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(560, 8);
			this.progressBar.TabIndex = 1;
			this.progressBar.Visible = false;
			// 
			// textBoxLog
			// 
			this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxLog.Location = new System.Drawing.Point(0, 0);
			this.textBoxLog.MaxLength = 0;
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ReadOnly = true;
			this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxLog.Size = new System.Drawing.Size(568, 300);
			this.textBoxLog.TabIndex = 0;
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.Location = new System.Drawing.Point(496, 8);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 23);
			this.buttonClose.TabIndex = 1;
			this.buttonClose.Text = "Cancel";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.buttonConvert);
			this.panel1.Controls.Add(this.buttonClose);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 325);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(576, 40);
			this.panel1.TabIndex = 2;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(576, 365);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.tabControl);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(582, 399);
			this.Name = "MainForm";
			this.Text = "IBN Server 4.5 to 4.7 Converter";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.tabControl.ResumeLayout(false);
			this.tabPageSettings.ResumeLayout(false);
			this.groupTarget.ResumeLayout(false);
			this.groupTarget.PerformLayout();
			this.groupSource.ResumeLayout(false);
			this.groupSource.PerformLayout();
			this.tabPageLog.ResumeLayout(false);
			this.tabPageLog.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}

		private void MainForm_Load(object sender, System.EventArgs e)
		{
			UpdateUI(true);
		}

		#region GetConnectionString()
		private string GetConnectionString(bool source, bool defaultDatabase)
		{
			bool trustedConnection = source ? checkBoxSourceUseTrustedConnection.Checked : checkBoxTargetUseTrustedConnection.Checked;
			string server = source ? textSourceServer.Text : textTargetServer.Text;
			string initialCatalog = defaultDatabase ? string.Empty : (source ? comboSourceDatabase.Text : comboTargetDatabase.Text);
			string user = source ? textBoxSourceSqlUser.Text : textBoxTargetSqlUser.Text;
			string password = source ? textBoxSourceSqlPassword.Text : textBoxTargetSqlPassword.Text;

			if(trustedConnection)
				return string.Format("Data source={0};Initial catalog={1};Integrated Security=SSPI", server, initialCatalog);
			else
				return string.Format("Data source={0};Initial catalog={1};User ID={2};Password={3}", server, initialCatalog, user, password);
		}
		#endregion
		#region GetSourcePortalConnectionString()
		private string GetSourcePortalConnectionString()
		{
			string initialCatalog = ((CompanyInfo)((ComboboxWraper)comboSourceCompany.SelectedItem).Value).Database;

			if(checkBoxSourceUseTrustedConnection.Checked)
				return string.Format("Data source={0};Initial catalog={1};Integrated Security=SSPI", textSourceServer.Text, initialCatalog);
			else
				return string.Format("Data source={0};Initial catalog={1};User ID={2};Password={3}", textSourceServer.Text, initialCatalog, textBoxSourceSqlUser.Text, textBoxSourceSqlPassword.Text);
		}
		#endregion

		public void ConversionCompleted(string message)//, MessageBoxIcon icon)
		{
			LogMessage("------------------------------------------------------------------------------------------------------------------------------------------------------", false);
			LogMessage(message);

			m_InProgress = false;
			LockUI(true);
		}

		#region GetDatabases()
		private void GetDatabases(bool source, ComboBox combo, ref bool populated)
		{
			this.Cursor = Cursors.WaitCursor;
			try
			{
				if(!populated)
				{
					combo.Items.Clear();

					foreach (string db in IbnConverter.GetDatabases(GetConnectionString(source, true)))
						combo.Items.Add(db);

					populated = true;
				}
			}
			catch(SqlException ex)
			{
				MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch{}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}
		#endregion
		#region GetCompanies()
		private void GetCompanies(bool source, ComboBox combo, ref bool populated)
		{
			this.Cursor = Cursors.WaitCursor;
			try
			{
				if(!populated)
				{
					combo.Items.Clear();

					foreach(CompanyInfo ci in IbnConverter.GetCompanies(GetConnectionString(source, false)))
						combo.Items.Add(new ComboboxWraper(string.Format("{0} ({1})", ci.Domain, ci.Database), ci));

					populated = true;
				}
			}
			catch(SqlException ex)
			{
				MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}
		#endregion
		#region GetCalendars()
		private void GetCalendars(bool source, ComboBox combo, ref bool populated)
		{
			this.Cursor = Cursors.WaitCursor;
			try
			{
				if(!populated)
				{
					combo.Items.Clear();

					if(comboSourceCompany.SelectedItem != null)
					{
						NameValueCollection items = IbnConverter.GetCalendars(GetSourcePortalConnectionString());
						foreach(string key in items.AllKeys)
							combo.Items.Add(new ComboboxWraper(items[key], int.Parse(key)));

						populated = true;
					}
				}
			}
			catch(SqlException ex)
			{
				MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}
		#endregion
		#region GetManagers()
		private void GetManagers(bool source, ComboBox combo, ref bool populated)
		{
			this.Cursor = Cursors.WaitCursor;
			try
			{
				if(!populated)
				{
					combo.Items.Clear();

					if(comboSourceCompany.SelectedItem != null)
					{
						NameValueCollection items = IbnConverter.GetManagers(GetSourcePortalConnectionString());
						foreach(string key in items.AllKeys)
							combo.Items.Add(new ComboboxWraper(items[key], int.Parse(key)));

						populated = true;
					}
				}
			}
			catch(SqlException ex)
			{
				MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}
		#endregion
		#region UpdateUI()
		private void UpdateUI(bool init)
		{
			UpdateUI(init, groupSource, true, _converter.RequiredSourceVersion, textSourceServer, checkBoxSourceUseTrustedConnection, textBoxSourceSqlUser, textBoxSourceSqlPassword, comboSourceDatabase, comboSourceCompany);
			UpdateUI(init, groupTarget, false, _converter.RequiredTargetVersion, textTargetServer, checkBoxTargetUseTrustedConnection, textBoxTargetSqlUser, textBoxTargetSqlPassword, comboTargetDatabase, null);
			LockUI(true);
		}

		private void UpdateUI(bool init, GroupBox group, bool isSource, Version version, TextBox textServer, CheckBox checkTrusted, TextBox textUser, TextBox textPassword, ComboBox comboDatabase, ComboBox comboCompany)
		{
			group.Text = string.Format(CultureInfo.InvariantCulture, "IBN {0} {1} Company", version, isSource ? "Source" : "Target");

			textUser.Enabled = !checkTrusted.Checked;
			textPassword.Enabled = !checkTrusted.Checked;

			if(init && comboDatabase.Items.Count > 0)
				comboDatabase.SelectedIndex = 0;

			if (init && comboCompany != null && comboCompany.Items.Count > 0)
				comboCompany.SelectedIndex = 0;
		}
		#endregion
		#region LockUI()
		public void LockUI(bool enable)
		{
			buttonConvert.Enabled = enable;
			progressBar.Enabled = enable;
			buttonCopySettings.Enabled = enable;

			checkBoxSourceUseTrustedConnection.Enabled = enable;
			checkBoxTargetUseTrustedConnection.Enabled = enable;
 

			if(!checkBoxSourceUseTrustedConnection.Checked)
			{
				textBoxSourceSqlUser.Enabled = enable;
				textBoxSourceSqlPassword.Enabled = enable;
			}
 
			if(!checkBoxTargetUseTrustedConnection.Checked)
			{
				textBoxTargetSqlPassword.Enabled = enable;
				textBoxTargetSqlUser.Enabled = enable;
			}
 

			comboSourceDatabase.Enabled = enable;
			comboTargetDatabase.Enabled = enable;

			comboSourceCompany.Enabled = enable;

			textSourceServer.Enabled = enable;
			textTargetServer.Enabled = enable;

			buttonClose.Text = m_InProgress ? "Cancel" : "Close";
		}
		#endregion
		#region LogMessage()
		public void LogMessage(string message)
		{
			LogMessage(message, true);
		}
		public void LogMessage(string message, bool writeTime)
		{
			StringBuilder sb = new StringBuilder(textBoxLog.Text);

			if(writeTime)
			{
				sb.Append(DateTime.Now.ToString("T"));
				sb.Append(" ");
			}
			sb.Append(message);
			sb.Append("\r\n");

			textBoxLog.Text = sb.ToString();

			textBoxLog.Select(textBoxLog.Text.Length, 0);
			textBoxLog.ScrollToCaret();
		}
		#endregion
		#region ResetCombo()
		static void ResetCombo(ComboBox combo)
		{
			if(combo.Items.Count > 0)
				combo.SelectedIndex = 0;
			else
				combo.SelectedIndex = -1;
		}
		#endregion
		#region ValidateCombo()
		bool ValidateCombo(ComboBox combo, string validatorMessage)
		{
			bool ok = (combo.Items.Count > 0 && combo.SelectedItem != null);
			if(!ok)
				MessageBox.Show(validatorMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
			return ok;
		}
		#endregion

		#region * Converter event handlers *
		private void OnProgress(object sender, ConverterEventArgs e)
		{
			if (e != null)
			{
				e.Cancel = m_Cancel;
				LogMessage(e.Message);
			}
		}

		private void OnWarning(object sender, ConverterEventArgs e)
		{
			if (e != null)
			{
				e.Cancel = m_Cancel;
				LogMessage("*** WARNING ***" + e.Message);

				if (!m_Cancel)
				{
					DialogResult result = MessageBox.Show(e.Message + "\r\nContinue conversion?", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
					if (result == DialogResult.Cancel)
					{
						e.Cancel = m_Cancel = true;
						LogMessage("Canceling (Please wait) ...");
					}
				}
			}
		}

		private void OnCompleted(object sender, ConverterEventArgs e)
		{
			string message = "Conversion completed successfully.";
			try
			{
				_converter.EndConvert();
			}
			catch(IbnConverterException ex)
			{
				message = ex.Message;
			}
			catch(Exception ex)
			{
				message = "Conversion failed.\r\n" + ex.ToString().Replace("\n", "\r\n");
			}
			ConversionCompleted(message);
		}
		#endregion

		private void checkBoxSourceUseTrustedConnection_CheckedChanged(object sender, System.EventArgs e)
		{
			textBoxSourceSqlUser.Enabled = !checkBoxSourceUseTrustedConnection.Checked;
			textBoxSourceSqlPassword.Enabled = !checkBoxSourceUseTrustedConnection.Checked;
		}

		private void checkBoxTargetUseTrustedConnection_CheckedChanged(object sender, System.EventArgs e)
		{
			textBoxTargetSqlUser.Enabled = !checkBoxTargetUseTrustedConnection.Checked;
			textBoxTargetSqlPassword.Enabled = !checkBoxTargetUseTrustedConnection.Checked;
		}

		private void buttonCopySettings_Click(object sender, System.EventArgs e)
		{
			textTargetServer.Text = textSourceServer.Text;
			textBoxTargetSqlUser.Text = textBoxSourceSqlUser.Text;
			textBoxTargetSqlPassword.Text = textBoxSourceSqlPassword.Text;
			checkBoxTargetUseTrustedConnection.Checked = checkBoxSourceUseTrustedConnection.Checked;
		}

		private void buttonClose_Click(object sender, System.EventArgs e)
		{
			if(!m_InProgress)
				this.Close();
			else
			{
				DialogResult result = MessageBox.Show("Do you really want to cancel current operation.", this.Text, MessageBoxButtons.YesNo);
				if(result == DialogResult.Yes)
				{
					m_Cancel = true;
					LogMessage("Canceling (Please wait) ...");
				}
			}
		}

		private void buttonConvert_Click(object sender, System.EventArgs e)
		{
			if(!m_InProgress)
			{
				if(!ValidateCombo(comboSourceCompany, "Please select source company."))
					return;
				if(!ValidateCombo(comboTargetDatabase, "Please select target database."))
					return;

				DialogResult result = MessageBox.Show("All data in the target company will be deleted.\r\n\r\nDo you really want to continue with conversion?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
				if(result == DialogResult.Yes)
				{
					textBoxLog.Text = string.Empty;

					LogMessage("------------------------------------------------------------------------------------------------------------------------------------------------------", false);
					LogMessage(string.Format("Source: {0} at {1}.{2}", comboSourceCompany.Text, textSourceServer.Text, comboSourceDatabase.Text), false);
					LogMessage(string.Format("Target: {0}.{1}", textTargetServer.Text, comboTargetDatabase.Text), false);
					LogMessage("------------------------------------------------------------------------------------------------------------------------------------------------------", false);
					LogMessage("Conversion started.");

					tabControl.SelectedTab = tabPageLog ;

					_converter.SourceConnectionString = GetConnectionString(true, false);
					_converter.TargetConnectionString = GetConnectionString(false, false);

					m_Cancel = false;

					_converter.BeginConvert(
						((CompanyInfo)((ComboboxWraper)comboSourceCompany.SelectedItem).Value).Id
						, 0
						);

					m_InProgress = true;

					LockUI(false);
				}
			}
		}

		private void comboSourceDatabase_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			m_SourceCompanyListPopulated = false;
			comboSourceCompany.SelectedIndex = -1;
		}

		private void comboSourceDatabase_DropDown(object sender, System.EventArgs e)
		{
			GetDatabases(true, comboSourceDatabase, ref m_SourceDatabaseListPopulated);
		}

		private void comboTargetDatabase_DropDown(object sender, System.EventArgs e)
		{
			GetDatabases(false, comboTargetDatabase, ref m_TargetDatabaseListPopulated);
		}

		private void comboSourceCompany_DropDown(object sender, System.EventArgs e)
		{
			GetCompanies(true, comboSourceCompany, ref m_SourceCompanyListPopulated);
		}

		private void textSourceServer_TextChanged(object sender, EventArgs e)
		{
			m_SourceDatabaseListPopulated = false;
			m_SourceCompanyListPopulated = false;
		}

		private void textTargetServer_TextChanged(object sender, EventArgs e)
		{
			m_TargetDatabaseListPopulated = false;
		}
	}
}
