using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Mediachase.Ibn.Configuration;

namespace Mediachase.Ibn.ConfigurationUI
{
	public partial class EditSqlServerSettingsForm : Form
	{
		/// <summary>
		/// Gets or sets the configurator.
		/// </summary>
		/// <value>The configurator.</value>
		protected IConfigurator Configurator { get; private set; }


		/// <summary>
		/// Gets or sets the color of the invalid back.
		/// </summary>
		/// <value>The color of the invalid back.</value>
		//protected Color EnableForeColor { get; private set; }

		/// <summary>
		/// Gets or sets the color of the valid back.
		/// </summary>
		/// <value>The color of the valid back.</value>
		//protected Color DisableForeColor { get; private set; }

		/// <summary>
		/// Gets or sets the color of the invalid back.
		/// </summary>
		/// <value>The color of the invalid back.</value>
		protected Color InvalidBackColor { get; private set; }

		/// <summary>
		/// Gets or sets the color of the valid back.
		/// </summary>
		/// <value>The color of the valid back.</value>
		protected Color ValidBackColor { get; private set; }


		/// <summary>
		/// Gets a value indicating whether [use windows auth].
		/// </summary>
		/// <value><c>true</c> if [use windows auth]; otherwise, <c>false</c>.</value>
		public bool UseWindowsAuth
		{
			get
			{
				return comboBoxAuthentication.SelectedIndex == 0;
			}
		}

		/// <summary>
		/// Gets the name of the SQL server.
		/// </summary>
		/// <value>The name of the SQL server.</value>
		public string SqlServerName
		{
			get
			{
				return comboBoxSqlServer.Text;
			}
		}

		/// <summary>
		/// Gets the SQL server user.
		/// </summary>
		/// <value>The SQL server user.</value>
		public string SqlServerUser
		{
			get
			{
				if (UseWindowsAuth)
					return string.Empty;

				return textBoxUser.Text;
			}
		}

		/// <summary>
		/// Gets the SQL server password.
		/// </summary>
		/// <value>The SQL server password.</value>
		public string SqlServerPassword
		{
			get
			{
				if (UseWindowsAuth)
					return string.Empty;

				return textBoxPassword.Text;
			}
		}

		/// <summary>
		/// Gets the name of the ibn user.
		/// </summary>
		/// <value>The name of the ibn user.</value>
		public string IbnUserName
		{
			get
			{
				return textBoxIbnUser.Text;
			}
		}

		/// <summary>
		/// Gets the ibn user password.
		/// </summary>
		/// <value>The ibn user password.</value>
		public string IbnUserPassword
		{
			get
			{
				if (radioButtonGenerateRandomPass.Checked)
					return textBoxAutoPassword.Text;

				return textBoxIbnUserPassword.Text;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditSqlServerSettingsForm"/> class.
		/// </summary>
		/// <param name="serverConfigurator">The server configurator.</param>
		public EditSqlServerSettingsForm(IConfigurator serverConfigurator)
		{
			InitializeComponent();

#if RADIUS
			this.Icon = SnapInResources.portal_RS;
#endif 


			this.Configurator = serverConfigurator;

			//this.EnableForeColor = Color.FromKnownColor(System.Drawing.KnownColor.ControlText);
			//this.DisableForeColor = Color.FromKnownColor(System.Drawing.KnownColor.InactiveCaptionText);

			this.ValidBackColor = Color.FromKnownColor(System.Drawing.KnownColor.Window);
			this.InvalidBackColor = Color.FromArgb(255, 192, 192);


			// Read SQL Server Settings Here
			LoadSqlServerSettings();

			UpdateSqlServerConfigBlock();
			UpdateIbnUserConfigBlock();
		}

		private void LoadSqlServerSettings()
		{
			ISqlServerSettings settings = this.Configurator.SqlSettings;

			// Init SqlServerConfigBlock

			if (string.IsNullOrEmpty(settings.Server))
				comboBoxSqlServer.Text = "(local)";
			else
				comboBoxSqlServer.Text = settings.Server;

			if (settings.Authentication == AuthenticationType.Windows)
				comboBoxAuthentication.SelectedIndex = 0;
			else
				comboBoxAuthentication.SelectedIndex = 1;

			if (string.IsNullOrEmpty(settings.AdminUser))
				textBoxUser.Text = "sa";
			else
				textBoxUser.Text = settings.AdminUser;

			textBoxPassword.Text = settings.AdminPassword;
			

			// IbnUserConfigBlock
			radioButtonEnterPassManually.Checked = true;

			if (string.IsNullOrEmpty(settings.PortalUser))
				textBoxIbnUser.Text = IbnConst.ProductFamilyShort;
			else
				textBoxIbnUser.Text = settings.PortalUser;

			textBoxIbnUserPassword.Text = settings.PortalPassword;
			textBoxIbnUserPasswordRe.Text = settings.PortalPassword;
		}

		/// <summary>
		/// Handles the Click event of the buttonOk control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonOk_Click(object sender, EventArgs e)
		{
			if (CheckErrorProvider(this.Controls))
			{
				// TODO: Fixed Items Here

				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		/// <summary>
		/// Checks the error provider.
		/// </summary>
		/// <param name="controls">The controls.</param>
		/// <returns></returns>
		private bool CheckErrorProvider(Control.ControlCollection controls)
		{
			this.ValidateChildren();

			foreach (Control control in controls)
			{
				if (!string.IsNullOrEmpty(errorProvider.GetError(control)))
					return false;

				if (control.HasChildren && !CheckErrorProvider(control.Controls))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Handles the Click event of the buttonCancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonCancel_Click(object sender, EventArgs e)
		{

		}

		/// <summary>
		/// Handles the DropDown event of the comboBoxSqlServer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void comboBoxSqlServer_DropDown(object sender, EventArgs e)
		{
			comboBoxSqlServer.Items.Clear();

			Application.UseWaitCursor = true;

			try
			{
				SQLDMO.ApplicationClass app = new SQLDMO.ApplicationClass();
				SQLDMO.NameList servers = app.ListAvailableSQLServers();

				for (int i = 1; i <= servers.Count; i++)
				{
					comboBoxSqlServer.Items.Add(servers.Item(i));
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex, "Mediachase.Ibn.ConfigurationUI.SQLDMO");
			}

			Application.UseWaitCursor = false;

		}

		/// <summary>
		/// Updates the SQL server config block.
		/// </summary>
		private void UpdateSqlServerConfigBlock()
		{
			// Sql Server Authentication
			if (!this.UseWindowsAuth)
			{
				textBoxUser.Enabled = true;
				textBoxPassword.Enabled = true;

				//textBoxUser.ForeColor = this.EnableForeColor;
				//textBoxPassword.ForeColor = this.EnableForeColor;

				labelUser.Enabled = true;
				labelPassword.Enabled = true;
				//labelUser.ForeColor = this.EnableForeColor;
				//labelPassword.ForeColor = this.EnableForeColor;
			}
			// Windows Authentication
			else
			{
				textBoxUser.Enabled = false;
				textBoxPassword.Enabled = false;

				//textBoxUser.ForeColor = this.DisableForeColor;
				//textBoxPassword.ForeColor = this.DisableForeColor;
				//labelUser.ForeColor = this.DisableForeColor;
				//labelPassword.ForeColor = this.DisableForeColor;
				labelUser.Enabled = false;
				labelPassword.Enabled = false;
			}

			CheckErrorProvider(this.Controls);
		}

		/// <summary>
		/// Handles the CheckedChanged event of the radioButtonEnterPassManually control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void radioButtonEnterPassManually_CheckedChanged(object sender, EventArgs e)
		{
			UpdateIbnUserConfigBlock();
		}

		/// <summary>
		/// Handles the CheckedChanged event of the radioButtonGenerateRandomPass control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void radioButtonGenerateRandomPass_CheckedChanged(object sender, EventArgs e)
		{
			UpdateIbnUserConfigBlock();
		}

		/// <summary>
		/// Updates the ibn user config block.
		/// </summary>
		private void UpdateIbnUserConfigBlock()
		{
			if (!radioButtonGenerateRandomPass.Checked)
			{
				textBoxAutoPassword.Text = string.Empty;

				textBoxIbnUserPassword.Enabled = true;
				textBoxIbnUserPasswordRe.Enabled = true;

				labelIbnUserPassword.Enabled = true;
				labelIbnUserPasswordRe.Enabled = true;
			}
			else
			{
				textBoxAutoPassword.Text = Guid.NewGuid().ToString();

				textBoxIbnUserPassword.Enabled = false;
				textBoxIbnUserPasswordRe.Enabled = false;

				labelIbnUserPassword.Enabled = false;
				labelIbnUserPasswordRe.Enabled = false;
			}

			CheckErrorProvider(this.Controls);
		}

		private void comboBoxAuthentication_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateSqlServerConfigBlock();
		}

		private void linkLabelCheckConnection_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Application.UseWaitCursor = true;

			try
			{
				using (IDataReader reader = ExecuteReader(this.GetConnectionString(), "sp_databases"))
				{
				}

				//errorProvider.SetError(comboBoxSqlServer, string.Empty);
				MessageBox.Show(SnapInResources.EditSqlServerSettingsForm_CheckCompletedMsgBox_Text, 
					SnapInResources.EditSqlServerSettingsForm_CheckCompletedMsgBox_Caption, 
					MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (SqlException ex)
			{
				//errorProvider.SetError(comboBoxSqlServer, ex.Message);
				MessageBox.Show(ex.Message, 
					SnapInResources.EditSqlServerSettingsForm_SqlConnectionErrorMsgBox_Caption, 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (Exception ex)
			{
				//errorProvider.SetError(comboBoxSqlServer, ex.Message);
				MessageBox.Show(ex.ToString(), 
					SnapInResources.EditSqlServerSettingsForm_GlobalErrorMsgBox_Caption, 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			

			Application.UseWaitCursor = false;

		}

		/// <summary>
		/// Gets the connection string.
		/// </summary>
		/// <returns></returns>
		private string GetConnectionString()
		{
			if (this.UseWindowsAuth)
				return string.Format("Data source={0};Initial catalog={1};Integrated Security=SSPI", 
					this.SqlServerName, 
					"master");

			return string.Format("Data source={0};Initial catalog={1};User ID={2};Password={3}", 
				this.SqlServerName, 
				"master", 
				this.SqlServerUser, this.SqlServerPassword);
		}

		/// <summary>
		/// Executes the reader.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		/// <param name="commandText">The command text.</param>
		/// <returns></returns>
		private static SqlDataReader ExecuteReader(string connectionString, string commandText)
		{
			//create & open a SqlConnection
			SqlConnection cn = new SqlConnection(connectionString);
			cn.Open();

			try
			{
				//call the private overload that takes an internally owned connection in place of the connection string
				SqlCommand cmd = cn.CreateCommand();

				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = commandText;

				return cmd.ExecuteReader(CommandBehavior.CloseConnection);
			}
			catch
			{
				//if we fail to return the SqlDatReader, we need to close the connection ourselves
				cn.Close();
				throw;
			}
		}

		/// <summary>
		/// Handles the Validated event of the textBoxIbnUserPassword control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void textBoxIbnUserPassword_Validated(object sender, EventArgs e)
		{
			if (radioButtonEnterPassManually.Checked)
			{
				if (string.IsNullOrEmpty(textBoxIbnUserPassword.Text))
				{
					errorProvider.SetError(textBoxIbnUserPassword, SnapInResources.ErrorProviderMsg_RequiredField);
					textBoxIbnUserPassword.BackColor = this.InvalidBackColor;
				}
				else
				{
					errorProvider.SetError(textBoxIbnUserPassword, string.Empty);
					textBoxIbnUserPassword.BackColor = this.ValidBackColor;
				}
			}
			else
			{
				errorProvider.SetError(textBoxIbnUserPassword, string.Empty);
				textBoxIbnUserPassword.BackColor = this.ValidBackColor;
			}
		}

		/// <summary>
		/// Handles the Validated event of the textBoxIbnUserPasswordRe control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void textBoxIbnUserPasswordRe_Validated(object sender, EventArgs e)
		{
			if (radioButtonEnterPassManually.Checked)
			{
				if (textBoxIbnUserPasswordRe.Text != textBoxIbnUserPassword.Text)
				{
					errorProvider.SetError(textBoxIbnUserPasswordRe, SnapInResources.ErrorProviderMsg_PassDontMatch);
					textBoxIbnUserPasswordRe.BackColor = this.InvalidBackColor;
				}
				else
				{
					errorProvider.SetError(textBoxIbnUserPasswordRe, string.Empty);
					textBoxIbnUserPasswordRe.BackColor = this.ValidBackColor;
				}
			}
			else
			{
				errorProvider.SetError(textBoxIbnUserPasswordRe, string.Empty);
				textBoxIbnUserPasswordRe.BackColor = this.ValidBackColor;
			}
		}

		/// <summary>
		/// Handles the Validated event of the textBoxIbnUser control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void textBoxIbnUser_Validated(object sender, EventArgs e)
		{
			if (radioButtonEnterPassManually.Checked)
			{
				if (string.IsNullOrEmpty(textBoxIbnUser.Text))
				{
					errorProvider.SetError(textBoxIbnUser, SnapInResources.ErrorProviderMsg_RequiredField);
					textBoxIbnUser.BackColor = this.InvalidBackColor;
				}
				else
				{
					errorProvider.SetError(textBoxIbnUser, string.Empty);
					textBoxIbnUser.BackColor = this.ValidBackColor;
				}
			}
		}



		#region Company Section Validators
		#endregion

	}
}
