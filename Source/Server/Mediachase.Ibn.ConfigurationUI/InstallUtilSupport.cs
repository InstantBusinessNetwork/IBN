using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ManagementConsole;
using System.ComponentModel;
using System.Diagnostics;

namespace Mediachase.Ibn.ConfigurationUI
{
	/// <summary>
	/// Implements MMC SnapIn installer.
	/// </summary>
	[RunInstaller(true)]
	public sealed class InstallUtilSupport : SnapInInstaller
	{
		/// <summary>
		/// Writes settings into the registry. This method overrides the virtual Install method for <see cref="T:System.Configuration.Install.Installer"></see> class. For each snap-in that is defined in the loaded assembly, reflection is used to get registration information such as their node types, their snap-in identities, and other information. These settings are then written to the registry. If a rollback is required, previous settings are stored in the supplied                                                                                                                  <see cref="T:System.Collections.IDictionary"></see> value.
		/// </summary>
		/// <param name="stateSaver">An                                                                                                                                                                  <see cref="T:System.Collections.IDictionary"></see> value used to save information that is needed to perform a rollback or uninstall operation.</param>
		public override void Install(System.Collections.IDictionary stateSaver)
		{
			base.Install(stateSaver);

			// Run mmcperf.exe
			try
			{
				RunMmcPerf();
			}
			catch
			{
			}
		}

		/// <summary>
		/// Runs the MMC perf.
		/// </summary>
		private static void RunMmcPerf()
		{
			ProcessStartInfo info = new ProcessStartInfo("mmcperf.exe");

			info.WindowStyle = ProcessWindowStyle.Hidden;

			using (Process process = Process.Start(info))
			{
				process.WaitForExit();
			}
		}
	}
}
