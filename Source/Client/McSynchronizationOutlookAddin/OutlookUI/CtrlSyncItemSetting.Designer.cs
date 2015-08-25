namespace OutlookAddin.OutlookUI
{
	partial class CtrlSyncItemSetting
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
			this.components = new System.ComponentModel.Container();
			this.panel1 = new System.Windows.Forms.Panel();
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.btnFillRemoteTargets = new System.Windows.Forms.Button();
			this.cbRemoteSyncTarget = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.chkboxIncludeSubfolder = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnChangeFolder = new System.Windows.Forms.Button();
			this.tbSyncFolder = new System.Windows.Forms.TextBox();
			this.lblSyncFolder = new System.Windows.Forms.Label();
			this.cbConflictPolicy = new System.Windows.Forms.ComboBox();
			this.lblConflictResolution = new System.Windows.Forms.Label();
			this.cbDirection = new System.Windows.Forms.ComboBox();
			this.lblSyncDirection = new System.Windows.Forms.Label();
			this.pnlTop = new System.Windows.Forms.Panel();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
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
			this.panel1.Size = new System.Drawing.Size(249, 225);
			this.panel1.TabIndex = 0;
			// 
			// pnlBottom
			// 
			this.pnlBottom.Controls.Add(this.btnFillRemoteTargets);
			this.pnlBottom.Controls.Add(this.cbRemoteSyncTarget);
			this.pnlBottom.Controls.Add(this.label2);
			this.pnlBottom.Controls.Add(this.chkboxIncludeSubfolder);
			this.pnlBottom.Controls.Add(this.label1);
			this.pnlBottom.Controls.Add(this.btnChangeFolder);
			this.pnlBottom.Controls.Add(this.tbSyncFolder);
			this.pnlBottom.Controls.Add(this.lblSyncFolder);
			this.pnlBottom.Controls.Add(this.cbConflictPolicy);
			this.pnlBottom.Controls.Add(this.lblConflictResolution);
			this.pnlBottom.Controls.Add(this.cbDirection);
			this.pnlBottom.Controls.Add(this.lblSyncDirection);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlBottom.Location = new System.Drawing.Point(0, 48);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(249, 177);
			this.pnlBottom.TabIndex = 1;
			// 
			// btnFillRemoteTargets
			// 
			this.btnFillRemoteTargets.Location = new System.Drawing.Point(203, 146);
			this.btnFillRemoteTargets.Name = "btnFillRemoteTargets";
			this.btnFillRemoteTargets.Size = new System.Drawing.Size(35, 23);
			this.btnFillRemoteTargets.TabIndex = 11;
			this.btnFillRemoteTargets.Text = "Fill";
			this.btnFillRemoteTargets.UseVisualStyleBackColor = true;
			this.btnFillRemoteTargets.Click += new System.EventHandler(this.btnFillRemoteTargets_Click);
			// 
			// cbRemoteSyncTarget
			// 
			this.cbRemoteSyncTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbRemoteSyncTarget.FormattingEnabled = true;
			this.cbRemoteSyncTarget.Location = new System.Drawing.Point(21, 146);
			this.cbRemoteSyncTarget.Name = "cbRemoteSyncTarget";
			this.cbRemoteSyncTarget.Size = new System.Drawing.Size(176, 21);
			this.cbRemoteSyncTarget.TabIndex = 10;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Location = new System.Drawing.Point(18, 130);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(105, 13);
			this.label2.TabIndex = 9;
			this.label2.Text = "Remote Sync Target";
			// 
			// chkboxIncludeSubfolder
			// 
			this.chkboxIncludeSubfolder.AutoSize = true;
			this.chkboxIncludeSubfolder.Location = new System.Drawing.Point(80, 104);
			this.chkboxIncludeSubfolder.Name = "chkboxIncludeSubfolder";
			this.chkboxIncludeSubfolder.Size = new System.Drawing.Size(15, 14);
			this.chkboxIncludeSubfolder.TabIndex = 8;
			this.chkboxIncludeSubfolder.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Location = new System.Drawing.Point(18, 104);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Recursive";
			// 
			// btnChangeFolder
			// 
			this.btnChangeFolder.Location = new System.Drawing.Point(203, 74);
			this.btnChangeFolder.Name = "btnChangeFolder";
			this.btnChangeFolder.Size = new System.Drawing.Size(26, 23);
			this.btnChangeFolder.TabIndex = 6;
			this.btnChangeFolder.Text = "...";
			this.btnChangeFolder.UseVisualStyleBackColor = true;
			this.btnChangeFolder.Click += new System.EventHandler(this.btnChangeFolder_Click);
			// 
			// tbSyncFolder
			// 
			this.tbSyncFolder.Location = new System.Drawing.Point(21, 77);
			this.tbSyncFolder.Name = "tbSyncFolder";
			this.tbSyncFolder.ReadOnly = true;
			this.tbSyncFolder.Size = new System.Drawing.Size(176, 20);
			this.tbSyncFolder.TabIndex = 5;
			// 
			// lblSyncFolder
			// 
			this.lblSyncFolder.AutoSize = true;
			this.lblSyncFolder.BackColor = System.Drawing.Color.Transparent;
			this.lblSyncFolder.Location = new System.Drawing.Point(16, 61);
			this.lblSyncFolder.Name = "lblSyncFolder";
			this.lblSyncFolder.Size = new System.Drawing.Size(92, 13);
			this.lblSyncFolder.TabIndex = 4;
			this.lblSyncFolder.Text = "Local Sync Folder";
			// 
			// cbConflictPolicy
			// 
			this.cbConflictPolicy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbConflictPolicy.FormattingEnabled = true;
			this.cbConflictPolicy.Items.AddRange(new object[] {
            "Local  wins",
            "Remote wins",
            "Merge",
            "Cancel"});
			this.cbConflictPolicy.Location = new System.Drawing.Point(80, 33);
			this.cbConflictPolicy.Name = "cbConflictPolicy";
			this.cbConflictPolicy.Size = new System.Drawing.Size(149, 21);
			this.cbConflictPolicy.TabIndex = 3;
			// 
			// lblConflictResolution
			// 
			this.lblConflictResolution.AutoSize = true;
			this.lblConflictResolution.BackColor = System.Drawing.Color.Transparent;
			this.lblConflictResolution.Location = new System.Drawing.Point(16, 33);
			this.lblConflictResolution.Name = "lblConflictResolution";
			this.lblConflictResolution.Size = new System.Drawing.Size(42, 13);
			this.lblConflictResolution.TabIndex = 2;
			this.lblConflictResolution.Text = "Conflict";
			// 
			// cbDirection
			// 
			this.cbDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbDirection.FormattingEnabled = true;
			this.cbDirection.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cbDirection.Items.AddRange(new object[] {
            "Two-way",
            "Server -> Outlook",
            "Outlook -> Server"});
			this.cbDirection.Location = new System.Drawing.Point(80, 5);
			this.cbDirection.Name = "cbDirection";
			this.cbDirection.Size = new System.Drawing.Size(149, 21);
			this.cbDirection.TabIndex = 1;
			// 
			// lblSyncDirection
			// 
			this.lblSyncDirection.AutoSize = true;
			this.lblSyncDirection.BackColor = System.Drawing.Color.Transparent;
			this.lblSyncDirection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.lblSyncDirection.Location = new System.Drawing.Point(16, 8);
			this.lblSyncDirection.Name = "lblSyncDirection";
			this.lblSyncDirection.Size = new System.Drawing.Size(49, 13);
			this.lblSyncDirection.TabIndex = 0;
			this.lblSyncDirection.Text = "Direction";
			// 
			// pnlTop
			// 
			this.pnlTop.BackColor = System.Drawing.Color.Transparent;
			this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlTop.Location = new System.Drawing.Point(0, 0);
			this.pnlTop.Name = "pnlTop";
			this.pnlTop.Size = new System.Drawing.Size(249, 48);
			this.pnlTop.TabIndex = 0;
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// CtrlSyncItemSetting
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Name = "CtrlSyncItemSetting";
			this.Size = new System.Drawing.Size(249, 225);
			this.panel1.ResumeLayout(false);
			this.pnlBottom.ResumeLayout(false);
			this.pnlBottom.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel pnlTop;
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Label lblConflictResolution;
		private System.Windows.Forms.ComboBox cbDirection;
		private System.Windows.Forms.Label lblSyncDirection;
		private System.Windows.Forms.Label lblSyncFolder;
		private System.Windows.Forms.Button btnChangeFolder;
		private System.Windows.Forms.TextBox tbSyncFolder;
		private System.Windows.Forms.CheckBox chkboxIncludeSubfolder;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnFillRemoteTargets;
		private System.Windows.Forms.Label label2;
		public System.Windows.Forms.ComboBox cbConflictPolicy;
		private System.Windows.Forms.ComboBox cbRemoteSyncTarget;
		private System.Windows.Forms.ImageList imageList1;
	}
}
