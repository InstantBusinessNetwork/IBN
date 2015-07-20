using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.Remoting; 

using Mediachase.Ibn;


namespace Mediachase.Ibn.Service
{
	public class OleDBService : System.ServiceProcess.ServiceBase
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		//private System.ComponentModel.Container components;

		public OleDBService()
		{
			InitializeComponent();
		}

		// The main entry point for the process
		static void Main()
		{
			System.ServiceProcess.ServiceBase[] ServicesToRun;
	
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new OleDBService() };

			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		private void InitializeComponent()
		{
			this.ServiceName = Constants.ServiceName;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				//if (components != null) 
				//{
				//    components.Dispose();
				//}
			}
			base.Dispose( disposing );
		}

		protected override void OnStart(string[] args)
		{
			RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, false);
		}
 
		protected override void OnStop()
		{
		}
	}
}
