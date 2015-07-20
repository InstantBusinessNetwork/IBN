namespace Mediachase.Ibn.Configuration
{
	partial class ProjectInstaller
	{
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
			this.Installers.AddRange(new System.Configuration.Install.Installer[] { this.eventLogInstaller1 });
		}

		#endregion

		private System.Diagnostics.EventLogInstaller eventLogInstaller1;
	}
}