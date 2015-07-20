using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;

using Mediachase.Ibn;


namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for ProjectInstaller.
	/// </summary>
	[RunInstaller(true)]
	public class ProjectInstaller : System.Configuration.Install.Installer
	{
		private System.Diagnostics.EventLogInstaller eventLogInstaller1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
//		private System.ComponentModel.Container components = null;

		public ProjectInstaller()
		{
			// This call is required by the Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitComponent call
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
	}
}
