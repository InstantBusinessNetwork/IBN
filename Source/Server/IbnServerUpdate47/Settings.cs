using System;
using System.Globalization;
using System.IO;

using Mediachase.Ibn;
using Mediachase.Ibn.Configuration;


namespace IbnServerUpdate
{
	internal sealed class Settings
	{
		const string constIbnSqlUser = "IBN_SQL_USER";

		static string _LogFile;
		static long _SiteId;
		static int _Version;
		static string _IbnSqlUser;

		static string _InstallDir;
		static string _InstallDirWeb;
		static string _SqlServer;
		static string _SqlDatabase;
		static string _SqlUser;
		static string _SqlPassword;
		static string _UpdatesDir;
		static string _BackupDir;
		static int _MaxVersion;
		private static bool _PauseUpdate;

		public static string LogFile { get { return _LogFile; } }
		public static int Version { get { return _Version; } }
		public static long SiteId { get { return _SiteId; } }

		public static string InstallDir{ get{ return _InstallDir; } }
		public static string InstallDirWeb{ get{ return _InstallDirWeb; } }
		public static string SqlServer{ get{ return _SqlServer; } }
		public static string SqlDatabase{ get{ return _SqlDatabase; } }
		public static string SqlUser{ get{ return _SqlUser; } }
		public static string SqlPassword{ get{ return _SqlPassword; } }
		public static string UpdatesDir{ get{ return _UpdatesDir; } }
		public static string BackupDir{ get{ return _BackupDir; } }
		public static bool PauseUpdate { get { return _PauseUpdate; } }

		private Settings()
		{
		}

		public static void SetMaxVersion(int value)
		{
			if (value > _MaxVersion)
			{
				_MaxVersion = value;
			}
		}

		public static void SaveVersion()
		{
			RegistrySettings.WriteString(Configurator.ConstCommonVersion, _MaxVersion.ToString(CultureInfo.InvariantCulture));
		}

		#region Init
		public static bool Init(string[] args)
		{
			ReadArgsFromSystem();
			ParseCommandLine(args);

			AppendSlash(ref _InstallDir);
			AppendSlash(ref _InstallDirWeb);
			AppendSlash(ref _BackupDir);
			AppendSlash(ref _UpdatesDir);

			if(_InstallDirWeb == null)
				_InstallDirWeb = _InstallDir + "Web\\";
			if(_LogFile == null)
				_LogFile = "update.log";
			if (_IbnSqlUser == null)
				_IbnSqlUser = "IBN";

			RegistrySettings.WriteString(constIbnSqlUser, _IbnSqlUser);

			return (_InstallDir != null && _SqlServer != null
				&& _SqlUser != null && _SqlDatabase != null
				&& _SiteId > 0);
		}
		#endregion

		#region AppendSlash
		static void AppendSlash(ref string path)
		{
			if (!string.IsNullOrEmpty(path) && path[path.Length - 1] != Path.DirectorySeparatorChar)
				path += Path.DirectorySeparatorChar;
		}
		#endregion

		#region ReadArgsFromSystem
		static void ReadArgsFromSystem()
		{
			_InstallDir = RegRead("INSTALLDIR");
			_InstallDirWeb = RegRead("INSTALLDIR_WEB");
			_SqlServer = RegRead("MC_SQL_SERVER");
			_SqlDatabase = RegRead("MC_SQL_DATABASE");
			_SqlUser = RegRead("MC_SQL_USER");
			_SqlPassword = RegRead("MC_SQL_PASSWORD");
			_IbnSqlUser = RegRead(constIbnSqlUser);
			_SiteId = long.Parse(RegRead("MC_WEB_SERVER_NUMBER"), CultureInfo.InvariantCulture);

			_Version = RegistrySettings.ReadInt32(Configurator.ConstCommonVersion);
			_MaxVersion = _Version;

			_BackupDir = Environment.GetEnvironmentVariable("TEMP");
			AppendSlash(ref _BackupDir);
			_BackupDir += string.Format(CultureInfo.InvariantCulture, "Ibn{0}UpdateBackup\\", IbnConst.VersionMajorMinor);

			_UpdatesDir = AppDomain.CurrentDomain.BaseDirectory;

			_PauseUpdate = !string.IsNullOrEmpty(RegRead("PauseUpdate"));
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
							case "ID":
								_InstallDir = optVal;
								break;
							case "S":
								_SqlServer = optVal;
								break;
							case "DB":
								_SqlDatabase = optVal;
								break;
							case "U":
								_SqlUser = optVal;
								break;
							case "P":
								_SqlPassword = optVal;
								break;
							case "L":
								_LogFile = optVal;
								break;
							case "UD":
								_UpdatesDir = optVal;
								break;
							case "BD":
								_BackupDir = optVal;
								break;
							case "NU":
								_SiteId = long.Parse(optVal, CultureInfo.InvariantCulture);
								break;
						}
					}
				}
			}
		}
		#endregion

		#region RegRead(...)
		public static string RegRead(string argName)
		{
			string ret = RegistrySettings.ReadString(argName);
			if (ret != null && ret.Length == 0)
				ret = null;
			return ret;
		}
		#endregion
	}
}
