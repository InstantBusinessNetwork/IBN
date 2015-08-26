namespace OutlookAddin
{
	partial class FormSyncOptions
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSyncOptions));
			this.panelLeft = new System.Windows.Forms.Panel();
			this.panelRight = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.btCancel = new System.Windows.Forms.Button();
			this.btApply = new System.Windows.Forms.Button();
			this.imageList32 = new System.Windows.Forms.ImageList(this.components);
			this.panel1 = new System.Windows.Forms.Panel();
			this.imageList48 = new System.Windows.Forms.ImageList(this.components);
			this.panelRight.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelLeft
			// 
			this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.panelLeft.Location = new System.Drawing.Point(0, 0);
			this.panelLeft.Name = "panelLeft";
			this.panelLeft.Padding = new System.Windows.Forms.Padding(3);
			this.panelLeft.Size = new System.Drawing.Size(73, 348);
			this.panelLeft.TabIndex = 0;
			// 
			// panelRight
			// 
			this.panelRight.Controls.Add(this.panel3);
			this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelRight.Location = new System.Drawing.Point(73, 0);
			this.panelRight.Name = "panelRight";
			this.panelRight.Size = new System.Drawing.Size(295, 348);
			this.panelRight.TabIndex = 1;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.btCancel);
			this.panel3.Controls.Add(this.btApply);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel3.Location = new System.Drawing.Point(0, 304);
			this.panel3.Margin = new System.Windows.Forms.Padding(0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(295, 44);
			this.panel3.TabIndex = 0;
			// 
			// btCancel
			// 
			this.btCancel.Location = new System.Drawing.Point(136, 9);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 1;
			this.btCancel.Text = "Cancel";
			this.btCancel.UseVisualStyleBackColor = true;
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// btApply
			// 
			this.btApply.Location = new System.Drawing.Point(217, 9);
			this.btApply.Name = "btApply";
			this.btApply.Size = new System.Drawing.Size(75, 23);
			this.btApply.TabIndex = 0;
			this.btApply.Text = "Apply";
			this.btApply.UseVisualStyleBackColor = true;
			this.btApply.Click += new System.EventHandler(this.btApply_Click);
			// 
			// imageList32
			// 
			this.imageList32.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList32.ImageStream")));
			this.imageList32.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList32.Images.SetKeyName(0, "setting_menu_item_task32.png");
			this.imageList32.Images.SetKeyName(1, "setting_menu_item_appointment32.png");
			this.imageList32.Images.SetKeyName(2, "setting_menu_item_contact32.png");
			this.imageList32.Images.SetKeyName(3, "setting_menu_item_note32.png");
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.panelRight);
			this.panel1.Controls.Add(this.panelLeft);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(368, 348);
			this.panel1.TabIndex = 1;
			// 
			// imageList48
			// 
			this.imageList48.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList48.ImageStream")));
			this.imageList48.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList48.Images.SetKeyName(0, "setting_menu_connection.png");
			this.imageList48.Images.SetKeyName(1, "setting_menu_syncItems.png");
			// 
			// FormSyncOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(368, 348);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.Name = "FormSyncOptions";
			this.ShowInTaskbar = false;
			this.Text = "SyncOptionsForm";
			this.TopMost = true;
			this.panelRight.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelLeft;
		private System.Windows.Forms.Panel panelRight;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btApply;
		private System.Windows.Forms.ImageList imageList32;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ImageList imageList48;
	}
}