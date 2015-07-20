using System;
using System.DirectoryServices;
using System.Globalization;

using Mediachase.Ibn;
using System.IO;

namespace IbnServer
{
	internal sealed class Settings
	{
		public static string LogFile { get; private set; }
		public static string Action { get; private set; }
		public static string InstallDir { get; private set; }
		public static string InstallDirWeb { get; private set; }
		public static bool UpdateRegistry { get; private set; }
		public static bool RegisterAspNet { get; private set; }
		public static bool DeleteCompany { get; private set; }

		private Settings()
		{
		}

		public static bool Init(string[] args)
		{
			InstallDir = RegistrySettings.ReadString("INSTALLDIR");
			DeleteCompany = true;

			ParseCommandLine(args);

			if (!string.IsNullOrEmpty(InstallDir))
				InstallDir = InstallDir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

			InstallDirWeb = Path.Combine(InstallDir, @"Code\_Source\Web");

			bool result = false;

			switch (Action)
			{
				case "INSTALL":
				case "REMOVE":
					result = (InstallDir != null);
					break;
			}

			return result;
		}

		#region static void AppendSlash(ref string path)
		static void AppendSlash(ref string path)
		{
			if (!string.IsNullOrEmpty(path) && path[path.Length - 1] != Path.DirectorySeparatorChar)
				path += Path.DirectorySeparatorChar;
		}
		#endregion

		static void ParseCommandLine(string[] args)
		{
			int i;
			string optName, optVal;
			string quotes = "\"";

			foreach (string arg in args)
			{
				if (arg.Length >= 1 && arg[0] == '-' || arg[0] == '/')
				{
					i = arg.IndexOf(":", 1, StringComparison.OrdinalIgnoreCase);
					if (i > 1)
					{
						optName = arg.Substring(1, i - 1).ToUpperInvariant();
						optVal = arg.Substring(i + 1);
						if (optVal.Length >= 2 && optVal.StartsWith(quotes, StringComparison.OrdinalIgnoreCase) && optVal.EndsWith(quotes, StringComparison.OrdinalIgnoreCase))
							optVal = optVal.Substring(1, optVal.Length - 2);
						if (optVal.Length == 0)
							optVal = null;

						switch (optName)
						{
							case "ACT":
								Action = optVal.ToUpperInvariant();
								break;
							case "DC":
								DeleteCompany = bool.Parse(optVal);
								break;
							case "ID":
								InstallDir = optVal;
								break;
							case "L":
								LogFile = optVal;
								break;
							case "RA":
								RegisterAspNet = bool.Parse(optVal);
								break;
							case "REG":
								UpdateRegistry = (optVal == "true");
								break;
						}
					}
				}
			}
		}
	}
}
