namespace Mediachase.Ibn.ConfigurationUI
{
	partial class CompanyUpdateLogControl
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
			this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.labelCompanyId = new System.Windows.Forms.Label();
			this.linkDetails = new System.Windows.Forms.LinkLabel();
			this.labelCompanyName = new System.Windows.Forms.Label();
			this.tableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel
			// 
			this.tableLayoutPanel.ColumnCount = 3;
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.41994F));
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 68.58006F));
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 66F));
			this.tableLayoutPanel.Controls.Add(this.labelCompanyId, 1, 0);
			this.tableLayoutPanel.Controls.Add(this.linkDetails, 2, 0);
			this.tableLayoutPanel.Controls.Add(this.labelCompanyName, 0, 0);
			this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 1;
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.Size = new System.Drawing.Size(398, 24);
			this.tableLayoutPanel.TabIndex = 0;
			// 
			// labelCompanyId
			// 
			this.labelCompanyId.AutoSize = true;
			this.labelCompanyId.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelCompanyId.Location = new System.Drawing.Point(107, 0);
			this.labelCompanyId.Name = "labelCompanyId";
			this.labelCompanyId.Size = new System.Drawing.Size(221, 24);
			this.labelCompanyId.TabIndex = 2;
			this.labelCompanyId.Text = "A067FD67-5D0F-41ab-8559-98744AD1C449";
			this.labelCompanyId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// linkDetails
			// 
			this.linkDetails.AutoSize = true;
			this.linkDetails.Dock = System.Windows.Forms.DockStyle.Fill;
			this.linkDetails.Location = new System.Drawing.Point(334, 0);
			this.linkDetails.Name = "linkDetails";
			this.linkDetails.Size = new System.Drawing.Size(61, 24);
			this.linkDetails.TabIndex = 0;
			this.linkDetails.TabStop = true;
			this.linkDetails.Text = "Details";
			this.linkDetails.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.linkDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDetails_LinkClicked);
			// 
			// labelCompanyName
			// 
			this.labelCompanyName.AutoEllipsis = true;
			this.labelCompanyName.AutoSize = true;
			this.labelCompanyName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelCompanyName.Location = new System.Drawing.Point(3, 0);
			this.labelCompanyName.Name = "labelCompanyName";
			this.labelCompanyName.Size = new System.Drawing.Size(98, 24);
			this.labelCompanyName.TabIndex = 1;
			this.labelCompanyName.Text = "CompanyName";
			this.labelCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// CompanyUpdateLogControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.tableLayoutPanel);
			this.Name = "CompanyUpdateLogControl";
			this.Size = new System.Drawing.Size(398, 24);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private System.Windows.Forms.LinkLabel linkDetails;
		private System.Windows.Forms.Label labelCompanyId;
		private System.Windows.Forms.Label labelCompanyName;
	}
}
