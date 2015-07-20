namespace Mediachase.Ibn.ConfigurationUI
{
	partial class LogoControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogoControl));
			this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBoxLogo
			// 
			this.pictureBoxLogo.BackColor = System.Drawing.Color.Transparent;
#if RADIUS
			this.pictureBoxLogo.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.ServerFormViewImage_LogoRS;
#else
			this.pictureBoxLogo.Image = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.ServerFormViewImage_Logo;
#endif

			resources.ApplyResources(this.pictureBoxLogo, "pictureBoxLogo");
			this.pictureBoxLogo.Name = "pictureBoxLogo";
			this.pictureBoxLogo.TabStop = false;
			// 
			// LogoControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.BackgroundImage = global::Mediachase.Ibn.ConfigurationUI.SnapInResources.ServerFormViewImage_LogoFon;
			this.Controls.Add(this.pictureBoxLogo);
			this.DoubleBuffered = true;
			this.Name = "LogoControl";
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBoxLogo;
	}
}
