namespace OutlookAddin.OutlookUI
{
	partial class CtrlSyncItem
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
			this.panel1 = new CtrlCustomPanel();
			this.pbSyncStatus = new System.Windows.Forms.PictureBox();
			this.pbSyncItemLogo = new System.Windows.Forms.PictureBox();
			this.panel2 = new CtrlCustomPanel();
			this.lblSyncStats = new System.Windows.Forms.Label();
			this.progressBarSync = new System.Windows.Forms.ProgressBar();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbSyncStatus)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbSyncItemLogo)).BeginInit();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.Transparent;
			this.panel1.Controls.Add(this.pbSyncStatus);
			this.panel1.Controls.Add(this.pbSyncItemLogo);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(255, 47);
			this.panel1.TabIndex = 0;
			// 
			// pbSyncStatus
			// 
			this.pbSyncStatus.Location = new System.Drawing.Point(202, 6);
			this.pbSyncStatus.Name = "pbSyncStatus";
			this.pbSyncStatus.Size = new System.Drawing.Size(43, 38);
			this.pbSyncStatus.TabIndex = 5;
			this.pbSyncStatus.TabStop = false;
			// 
			// pbSyncItemLogo
			// 
			this.pbSyncItemLogo.Location = new System.Drawing.Point(8, 6);
			this.pbSyncItemLogo.Name = "pbSyncItemLogo";
			this.pbSyncItemLogo.Size = new System.Drawing.Size(43, 38);
			this.pbSyncItemLogo.TabIndex = 4;
			this.pbSyncItemLogo.TabStop = false;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.lblSyncStats);
			this.panel2.Controls.Add(this.progressBarSync);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 47);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(255, 46);
			this.panel2.TabIndex = 1;
			// 
			// lblSyncStats
			// 
			this.lblSyncStats.AutoSize = true;
			this.lblSyncStats.Location = new System.Drawing.Point(101, 30);
			this.lblSyncStats.Name = "lblSyncStats";
			this.lblSyncStats.Size = new System.Drawing.Size(35, 13);
			this.lblSyncStats.TabIndex = 1;
			this.lblSyncStats.Text = "label1";
			// 
			// progressBarSync
			// 
			this.progressBarSync.Location = new System.Drawing.Point(13, 6);
			this.progressBarSync.Name = "progressBarSync";
			this.progressBarSync.Size = new System.Drawing.Size(227, 23);
			this.progressBarSync.TabIndex = 0;
			// 
			// CtrlSyncItem
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "CtrlSyncItem";
			this.Size = new System.Drawing.Size(255, 93);
			this.MouseLeave += new System.EventHandler(this.CtrlSyncItem_MouseLeave);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.CtrlSyncItem_Paint);
			this.MouseEnter += new System.EventHandler(this.CtrlSyncItem_MouseEnter);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pbSyncStatus)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbSyncItemLogo)).EndInit();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.PictureBox pbSyncItemLogo;
		private System.Windows.Forms.PictureBox pbSyncStatus;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label lblSyncStats;
		private System.Windows.Forms.ProgressBar progressBarSync;
	}
}
