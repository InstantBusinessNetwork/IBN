namespace Mediachase.Ibn.ConfigurationUI
{
	partial class UpdateInfoControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateInfoControl));
			this.groupBoxUpdateInfo = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.textBoxVersion = new System.Windows.Forms.TextBox();
			this.labelReleaseDate = new System.Windows.Forms.Label();
			this.textBoxReleaseDate = new System.Windows.Forms.TextBox();
			this.labelVersion = new System.Windows.Forms.Label();
			this.textBoxRestrictions = new System.Windows.Forms.TextBox();
			this.labelRestrictions = new System.Windows.Forms.Label();
			this.groupBoxInfo = new System.Windows.Forms.GroupBox();
			this.webBrowserUpdateInfo = new System.Windows.Forms.WebBrowser();
			this.groupBoxUpdateInfo.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBoxInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxUpdateInfo
			// 
			this.groupBoxUpdateInfo.Controls.Add(this.tableLayoutPanel1);
			resources.ApplyResources(this.groupBoxUpdateInfo, "groupBoxUpdateInfo");
			this.groupBoxUpdateInfo.Name = "groupBoxUpdateInfo";
			this.groupBoxUpdateInfo.TabStop = false;
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.textBoxVersion, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.labelReleaseDate, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.textBoxReleaseDate, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.labelVersion, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.textBoxRestrictions, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.labelRestrictions, 0, 2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// textBoxVersion
			// 
			this.textBoxVersion.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxVersion, "textBoxVersion");
			this.textBoxVersion.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxVersion.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxVersion.Name = "textBoxVersion";
			this.textBoxVersion.ReadOnly = true;
			// 
			// labelReleaseDate
			// 
			resources.ApplyResources(this.labelReleaseDate, "labelReleaseDate");
			this.labelReleaseDate.Name = "labelReleaseDate";
			// 
			// textBoxReleaseDate
			// 
			this.textBoxReleaseDate.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxReleaseDate, "textBoxReleaseDate");
			this.textBoxReleaseDate.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxReleaseDate.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxReleaseDate.Name = "textBoxReleaseDate";
			this.textBoxReleaseDate.ReadOnly = true;
			// 
			// labelVersion
			// 
			resources.ApplyResources(this.labelVersion, "labelVersion");
			this.labelVersion.Name = "labelVersion";
			// 
			// textBoxRestrictions
			// 
			this.textBoxRestrictions.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxRestrictions, "textBoxRestrictions");
			this.textBoxRestrictions.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxRestrictions.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxRestrictions.Name = "textBoxRestrictions";
			this.textBoxRestrictions.ReadOnly = true;
			// 
			// labelRestrictions
			// 
			resources.ApplyResources(this.labelRestrictions, "labelRestrictions");
			this.labelRestrictions.Name = "labelRestrictions";
			// 
			// groupBoxInfo
			// 
			this.groupBoxInfo.Controls.Add(this.webBrowserUpdateInfo);
			resources.ApplyResources(this.groupBoxInfo, "groupBoxInfo");
			this.groupBoxInfo.Name = "groupBoxInfo";
			this.groupBoxInfo.TabStop = false;
			// 
			// webBrowserUpdateInfo
			// 
			this.webBrowserUpdateInfo.AllowNavigation = false;
			this.webBrowserUpdateInfo.AllowWebBrowserDrop = false;
			resources.ApplyResources(this.webBrowserUpdateInfo, "webBrowserUpdateInfo");
			this.webBrowserUpdateInfo.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserUpdateInfo.Name = "webBrowserUpdateInfo";
			// 
			// UpdateInfoControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.groupBoxInfo);
			this.Controls.Add(this.groupBoxUpdateInfo);
			this.DoubleBuffered = true;
			this.Name = "UpdateInfoControl";
			this.groupBoxUpdateInfo.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.groupBoxInfo.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxUpdateInfo;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TextBox textBoxVersion;
		private System.Windows.Forms.Label labelReleaseDate;
		private System.Windows.Forms.TextBox textBoxReleaseDate;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.TextBox textBoxRestrictions;
		private System.Windows.Forms.Label labelRestrictions;
		private System.Windows.Forms.GroupBox groupBoxInfo;
		private System.Windows.Forms.WebBrowser webBrowserUpdateInfo;
	}
}
