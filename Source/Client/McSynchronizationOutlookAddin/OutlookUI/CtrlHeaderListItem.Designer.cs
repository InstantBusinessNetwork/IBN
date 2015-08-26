namespace OutlookAddin.OutlookUI
{
	partial class CtrlHeaderListItem
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
			this.pnlHeader = new CtrlCustomPanel();
			this.lblCaption = new System.Windows.Forms.Label();
			this.pbLogo = new System.Windows.Forms.PictureBox();
			this.pnlHeader.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
			this.SuspendLayout();
			// 
			// pnlHeader
			// 
			this.pnlHeader.Controls.Add(this.lblCaption);
			this.pnlHeader.Controls.Add(this.pbLogo);
			this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlHeader.Location = new System.Drawing.Point(0, 0);
			this.pnlHeader.Name = "pnlHeader";
			this.pnlHeader.Size = new System.Drawing.Size(220, 48);
			this.pnlHeader.TabIndex = 0;
			// 
			// lblCaption
			// 
			this.lblCaption.AutoSize = true;
			this.lblCaption.BackColor = System.Drawing.Color.Transparent;
			this.lblCaption.Location = new System.Drawing.Point(66, 15);
			this.lblCaption.Name = "lblCaption";
			this.lblCaption.Size = new System.Drawing.Size(35, 13);
			this.lblCaption.TabIndex = 2;
			this.lblCaption.Text = "label1";
			// 
			// pbLogo
			// 
			this.pbLogo.BackColor = System.Drawing.Color.Transparent;
			this.pbLogo.Location = new System.Drawing.Point(7, 6);
			this.pbLogo.Name = "pbLogo";
			this.pbLogo.Size = new System.Drawing.Size(32, 32);
			this.pbLogo.TabIndex = 1;
			this.pbLogo.TabStop = false;
			// 
			// CtrlHeaderListItem
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlHeader);
			this.Name = "CtrlHeaderListItem";
			this.Size = new System.Drawing.Size(220, 48);
			this.pnlHeader.ResumeLayout(false);
			this.pnlHeader.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private CtrlCustomPanel pnlHeader;
		private System.Windows.Forms.PictureBox pbLogo;
		private System.Windows.Forms.Label lblCaption;
	}
}
