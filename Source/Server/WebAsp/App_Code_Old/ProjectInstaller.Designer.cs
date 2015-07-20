namespace Mediachase.Ibn.WebAsp.App_Code_Old
{
	partial class ProjectInstaller
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
			this.eventLogInstaller1 = new System.Diagnostics.EventLogInstaller();
			// 
			// eventLogInstaller1
			// 
			this.eventLogInstaller1.Log = Log.Name;
			this.eventLogInstaller1.Source = Log.Source;
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
																					  this.eventLogInstaller1});

		}

		#endregion

		private System.Diagnostics.EventLogInstaller eventLogInstaller1;
	}
}