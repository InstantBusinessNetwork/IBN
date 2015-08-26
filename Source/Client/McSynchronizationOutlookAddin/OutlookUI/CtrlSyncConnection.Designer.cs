namespace OutlookAddin.OutlookUI
{
	partial class CtrlSyncConnection
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.tbPass = new System.Windows.Forms.TextBox();
			this.tbUser = new System.Windows.Forms.TextBox();
			this.tbServer = new System.Windows.Forms.TextBox();
			this.lblPassword = new System.Windows.Forms.Label();
			this.lblUser = new System.Windows.Forms.Label();
			this.lblServer = new System.Windows.Forms.Label();
			this.pnlTop = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.pnlBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.pnlBottom);
			this.panel1.Controls.Add(this.pnlTop);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(223, 147);
			this.panel1.TabIndex = 0;
			// 
			// pnlBottom
			// 
			this.pnlBottom.Controls.Add(this.tbPass);
			this.pnlBottom.Controls.Add(this.tbUser);
			this.pnlBottom.Controls.Add(this.tbServer);
			this.pnlBottom.Controls.Add(this.lblPassword);
			this.pnlBottom.Controls.Add(this.lblUser);
			this.pnlBottom.Controls.Add(this.lblServer);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlBottom.Location = new System.Drawing.Point(0, 48);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(223, 99);
			this.pnlBottom.TabIndex = 2;
			this.pnlBottom.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlBottom_Paint);
			// 
			// tbPass
			// 
			this.tbPass.Location = new System.Drawing.Point(70, 69);
			this.tbPass.Name = "tbPass";
			this.tbPass.Size = new System.Drawing.Size(100, 20);
			this.tbPass.TabIndex = 5;
			// 
			// tbUser
			// 
			this.tbUser.Location = new System.Drawing.Point(70, 41);
			this.tbUser.Name = "tbUser";
			this.tbUser.Size = new System.Drawing.Size(100, 20);
			this.tbUser.TabIndex = 4;
			// 
			// tbServer
			// 
			this.tbServer.Location = new System.Drawing.Point(70, 11);
			this.tbServer.Name = "tbServer";
			this.tbServer.Size = new System.Drawing.Size(133, 20);
			this.tbServer.TabIndex = 3;
			// 
			// lblPassword
			// 
			this.lblPassword.AutoSize = true;
			this.lblPassword.BackColor = System.Drawing.Color.Transparent;
			this.lblPassword.Location = new System.Drawing.Point(16, 71);
			this.lblPassword.Name = "lblPassword";
			this.lblPassword.Size = new System.Drawing.Size(53, 13);
			this.lblPassword.TabIndex = 2;
			this.lblPassword.Text = "Password";
			// 
			// lblUser
			// 
			this.lblUser.AutoSize = true;
			this.lblUser.BackColor = System.Drawing.Color.Transparent;
			this.lblUser.Location = new System.Drawing.Point(16, 43);
			this.lblUser.Name = "lblUser";
			this.lblUser.Size = new System.Drawing.Size(29, 13);
			this.lblUser.TabIndex = 1;
			this.lblUser.Text = "User";
			// 
			// lblServer
			// 
			this.lblServer.AutoSize = true;
			this.lblServer.BackColor = System.Drawing.Color.Transparent;
			this.lblServer.Location = new System.Drawing.Point(16, 14);
			this.lblServer.Name = "lblServer";
			this.lblServer.Size = new System.Drawing.Size(38, 13);
			this.lblServer.TabIndex = 0;
			this.lblServer.Text = "Server";
			// 
			// pnlTop
			// 
			this.pnlTop.BackColor = System.Drawing.Color.Transparent;
			this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlTop.Location = new System.Drawing.Point(0, 0);
			this.pnlTop.Name = "pnlTop";
			this.pnlTop.Size = new System.Drawing.Size(223, 48);
			this.pnlTop.TabIndex = 1;
			// 
			// CtrlSyncConnection
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Name = "CtrlSyncConnection";
			this.Size = new System.Drawing.Size(223, 147);
			this.panel1.ResumeLayout(false);
			this.pnlBottom.ResumeLayout(false);
			this.pnlBottom.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel pnlTop;
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.TextBox tbPass;
		private System.Windows.Forms.TextBox tbUser;
		private System.Windows.Forms.TextBox tbServer;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.Label lblUser;
		private System.Windows.Forms.Label lblServer;
	}
}
