using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.Remoting;
using System.Xml;
using System.Threading;

using Mediachase.Ibn;
using System.IO;
using System.Configuration;


namespace Mediachase.Schedule.Service
{
	public class ScheduleService : System.ServiceProcess.ServiceBase
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		//private System.ComponentModel.Container _components;
		private System.IO.FileSystemWatcher _fileSystemWatcher;
		private InvocationTimer _invocationTimer;
		private System.Threading.Timer _threadingTimer;

		private SchedulerServiceClient _schedulerServiceClient = new SchedulerServiceClient();

		public ScheduleService()
		{


			// This call is required by the Windows.Forms Component Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitComponent call
		}

		// The main entry point for the process
		static void Main()
		{
			System.ServiceProcess.ServiceBase[] ServicesToRun;

			// More than one user Service may run within the same process. To add
			// another service to this process, change the following line to
			// create a second service object. For example,
			//
			//   ServicesToRun = New System.ServiceProcess.ServiceBase[] {new Service1(), new MySecondUserService()};
			//
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new ScheduleService() };

			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._fileSystemWatcher = new System.IO.FileSystemWatcher();
			((System.ComponentModel.ISupportInitialize)(this._fileSystemWatcher)).BeginInit();
			// 
			// m_fileSystemWatcher
			// 
			this._fileSystemWatcher.EnableRaisingEvents = false;
			this._fileSystemWatcher.NotifyFilter = System.IO.NotifyFilters.LastWrite;
			this._fileSystemWatcher.Path = AppDomain.CurrentDomain.BaseDirectory;
			this._fileSystemWatcher.Changed += new System.IO.FileSystemEventHandler(this.OnIbnConfigChanged);
			((System.ComponentModel.ISupportInitialize)(this._fileSystemWatcher)).EndInit();

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				//if (_components != null)
				//{
				//    _components.Dispose();
				//}
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			lock (typeof(ScheduleService))
			{
				try
				{
					_invocationTimer = new InvocationTimer();


					string strInterval = ConfigurationManager.AppSettings["Interval"];
					if (strInterval != null)
					{
						_invocationTimer.Interval = int.Parse(strInterval);
					}
					else
					{
						_invocationTimer.Interval = 10;
					}

					_threadingTimer = new System.Threading.Timer(new System.Threading.TimerCallback(m_timer_Elapsed), null, Timeout.Infinite, Timeout.Infinite);

					//RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, false);

					// Initialize WebConfig Reset
					this._fileSystemWatcher.Path = SchedulerServiceClient.InstallDir;
					this._fileSystemWatcher.Filter = "*.config";
					this._fileSystemWatcher.EnableRaisingEvents = true;

					SetTimer();
				}
				catch (Exception ex)
				{
					Log.WriteEntry("OnStart(): Exception: " + ex.ToString(), EventLogEntryType.Error);
				}
			}
		}

		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			System.Diagnostics.Trace.WriteLine("OnStop");

			try
			{
				if (_threadingTimer != null)
				{
					_threadingTimer.Dispose();
				}
			}
			catch (Exception ex)
			{
				Log.WriteEntry("OnStop(): Exception: " + ex.ToString(), EventLogEntryType.Error);
			}
		}

		/// <summary>
		/// Sets the timer.
		/// </summary>
		private void SetTimer()
		{
			_threadingTimer.Change(0, _invocationTimer != null ? _invocationTimer.Interval * 1000 : 60000);
		}

		/// <summary>
		/// M_timer_s the elapsed.
		/// </summary>
		/// <param name="state">The state.</param>
		private void m_timer_Elapsed(object state)
		{
			lock (typeof(ScheduleService))
			{
				Invoke();
			}
		}

		/// <summary>
		/// Invokes this instance.
		/// </summary>
		private void Invoke()
		{
			System.Diagnostics.Trace.WriteLine("ScheduleService.Invoke");

			lock (this)
			{
				try
				{
					_schedulerServiceClient.Push();
				}
				catch (Exception ex)
				{
					Log.WriteEntry("SchedulerServiceClient::Push. Exception: " + ex.ToString(), EventLogEntryType.Error);
				}
			}
		}

		/// <summary>
		/// Called when [ibn config changed].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.IO.FileSystemEventArgs"/> instance containing the event data.</param>
		private void OnIbnConfigChanged(object sender, System.IO.FileSystemEventArgs e)
		{
			try
			{
				if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["WriteDebugInfo"]))
				{
					Log.WriteEntry("Reset Configuration. Config File Changed", EventLogEntryType.Information);
				}

				_schedulerServiceClient.RefreshWebServiceList();
			}
			catch (Exception ex)
			{
				Log.WriteEntry("ResetWebServiceList. Exception: " + ex.ToString(), EventLogEntryType.Error);
			}
		}
	}
}
