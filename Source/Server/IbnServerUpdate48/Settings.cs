using System;
using System.Globalization;
using System.IO;

using Mediachase.Ibn;
using Mediachase.Ibn.Configuration;

namespace Update
{
	internal sealed class Settings
	{
		internal static string InstallDir { get; private set; }
		internal static int CommonVersion { get; private set; }
		internal static string UpdateDir { get; private set; }
		internal static string BackupDir { get; private set; }

		internal static string DemoHosts { get; private set; }
		internal static string DemoBackupDir { get; private set; }

		internal static string Target { get; private set; }
		internal static bool ShowLog { get; private set; }
		internal static string Id { get; private set; }
		internal static string LogFile { get; private set; }
		internal static int ProcessId { get; private set; }

		private Settings()
		{
		}

		public static void UpdateCommonVersion(int version)
		{
			CommonVersion = version;
			RegistrySettings.WriteString(Configurator.ConstCommonVersion, version.ToString(CultureInfo.InvariantCulture));
		}

		#region Init
		public static bool Init(string[] args)
		{
			ShowLog = true;

			ReadArgsFromSystem();
			ParseCommandLine(args);

			if(LogFile == null)
				LogFile = "update.log";

			return (InstallDir != null);
		}
		#endregion

		#region ReadArgsFromSystem
		static void ReadArgsFromSystem()
		{
			InstallDir = RegRead("INSTALLDIR");
			CommonVersion = RegistrySettings.ReadInt32(Configurator.ConstCommonVersion);

			BackupDir = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), string.Concat(IbnConst.ProductFamilyShort, IbnConst.VersionMajorMinor, "UpdateBackup"));
			UpdateDir = AppDomain.CurrentDomain.BaseDirectory;

			DemoBackupDir = RegRead("DemoBackupDir");
			DemoHosts = RegRead("DemoHosts");
		}
		#endregion

		#region ParseCommandLine
		static void ParseCommandLine(string[] args)
		{
			int i;
			string optName, optVal;
			char quotes = '"';

			foreach(string arg in args)
			{
				if(arg.Length >= 1 && arg[0] == '-' || arg[0] == '/')
				{
					i = arg.IndexOf(":", 1, StringComparison.OrdinalIgnoreCase);
					if(i > 1)
					{
						optName = arg.Substring(1, i-1).ToUpperInvariant();
						optVal = arg.Substring(i+1);
						if (optVal.Length >= 2 && optVal[0] == quotes && optVal[optVal.Length - 1] == quotes)
							optVal = optVal.Substring(1, optVal.Length - 2);
						if(optVal.Length == 0)
							optVal = null;

						switch(optName)
						{
							case "TARGET":
								Target = optVal.ToUpperInvariant();
								break;
							case "SHOWLOG":
								ShowLog = bool.Parse(optVal);
								break;
							case "ID":
								Id = optVal;
								break;
							case "PROCESSID":
								ProcessId = int.Parse(optVal, CultureInfo.InvariantCulture);
								break;

							case "L":
								LogFile = optVal;
								break;
							case "UD":
								UpdateDir = optVal;
								break;
							case "BD":
								BackupDir = optVal;
								break;
						}
					}
				}
			}
		}
		#endregion

		#region public static string RegRead(string name)
		public static string RegRead(string name)
		{
			string result = RegistrySettings.ReadString(name);

			if (result != null && result.Length == 0)
				result = null;

			return result;
		}
		#endregion
	}
}
