namespace OutlookAddin.OutlookUI
{
	partial class FormSyncItem
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSyncItem));
			this.panel1 = new System.Windows.Forms.Panel();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add(this.statusStrip1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(292, 367);
			this.panel1.TabIndex = 2;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Location = new System.Drawing.Point(0, 345);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(292, 22);
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "appointment_ok.png");
			this.imageList1.Images.SetKeyName(1, "appointment.png");
			this.imageList1.Images.SetKeyName(2, "appointment_sync.png");
			this.imageList1.Images.SetKeyName(3, "appointment_sync_1.png");
			this.imageList1.Images.SetKeyName(4, "appointment_sync_2.png");
			this.imageList1.Images.SetKeyName(5, "appointment_sync_3.png");
			this.imageList1.Images.SetKeyName(6, "appointment_conflict.png");
			this.imageList1.Images.SetKeyName(7, "appointment_canceled.png");
			this.imageList1.Images.SetKeyName(8, "appointment_failed.png");
			this.imageList1.Images.SetKeyName(9, "task_sync_ok.png");
			this.imageList1.Images.SetKeyName(10, "task.png");
			this.imageList1.Images.SetKeyName(11, "task_sync.png");
			this.imageList1.Images.SetKeyName(12, "task_sync_1.png");
			this.imageList1.Images.SetKeyName(13, "task_sync_2.png");
			this.imageList1.Images.SetKeyName(14, "task_sync_3.png");
			this.imageList1.Images.SetKeyName(15, "task_sync_canceled.png");
			this.imageList1.Images.SetKeyName(16, "task_sync_conflict.png");
			this.imageList1.Images.SetKeyName(17, "task_sync_failed.png");
			this.imageList1.Images.SetKeyName(18, "contact_ok.png");
			this.imageList1.Images.SetKeyName(19, "contact.png");
			this.imageList1.Images.SetKeyName(20, "contact_sync.png");
			this.imageList1.Images.SetKeyName(21, "contact_sync_1.png");
			this.imageList1.Images.SetKeyName(22, "contact_sync_2.png");
			this.imageList1.Images.SetKeyName(23, "contact_sync_3.png");
			this.imageList1.Images.SetKeyName(24, "contact_canceled.png");
			this.imageList1.Images.SetKeyName(25, "contact_conflict.png");
			this.imageList1.Images.SetKeyName(26, "contact_failed.png");
			this.imageList1.Images.SetKeyName(27, "note.png");
			// 
			// FormSyncItem
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 367);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.Name = "FormSyncItem";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SyncItemForm";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ImageList imageList1;
	}
}