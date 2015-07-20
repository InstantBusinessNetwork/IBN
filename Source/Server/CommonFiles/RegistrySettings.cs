using System;
using System.Globalization;
using System.IO;
using System.Security;

using Microsoft.Win32;


namespace Mediachase.Ibn
{
	internal sealed class RegistrySettings
	{
		public static bool IsWin64 { get { return IntPtr.Size == 8; } }

		#region public static string BuildKeyName()
		public static string BuildKeyName()
		{
			return BuildKeyName(IbnConst.InstallKey);
		}
		#endregion
		#region public static string BuildKeyName(string relativeKey)
		public static string BuildKeyName(string relativeKey)
		{
			return string.Concat(@"SOFTWARE\", IsWin64 ? @"Wow6432Node\" : string.Empty, relativeKey);
		}
		#endregion

		#region public static string ReadString(string name)
		public static string ReadString(string name)
		{
			return ReadString(IbnConst.InstallKey, name);
		}
		#endregion
		#region public static string ReadString(string relativeKey, string name)
		public static string ReadString(string relativeKey, string name)
		{
			string ret = null;

			try
			{
				RegistryKey rk = Registry.LocalMachine.OpenSubKey(BuildKeyName(relativeKey));
				if (rk != null)
				{
					ret = (string)rk.GetValue(name);
					rk.Close();
				}
			}
			catch (SecurityException)
			{
			}
			catch (IOException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}

			return ret;
		}
		#endregion

		#region public static int ReadInt32(string name)
		public static int ReadInt32(string name)
		{
			int ret = 0;
			string s = ReadString(name);
			if (!string.IsNullOrEmpty(s))
				ret = int.Parse(s, CultureInfo.InvariantCulture);
			return ret;
		}
		#endregion

		#region public static long ReadInt64(string name, long defaultValue)
		public static long ReadInt64(string name, long defaultValue)
		{
			long ret = defaultValue;
			string s = ReadString(name);
			if (!string.IsNullOrEmpty(s))
				ret = long.Parse(s, CultureInfo.InvariantCulture);
			return ret;
		}
		#endregion

		#region public static bool ReadBoolean(string name, bool defaultValue)
		public static bool ReadBoolean(string name, bool defaultValue)
		{
			bool ret = defaultValue;
			string s = ReadString(name);
			if (!string.IsNullOrEmpty(s))
				ret = bool.Parse(s);
			return ret;
		}
		#endregion

		#region public static void WriteString(string name, string value)
		public static void WriteString(string name, string value)
		{
			WriteString(IbnConst.InstallKey, name, value);
		}
		#endregion
		#region public static void WriteString(string relativeKey, string name, string value)
		public static void WriteString(string relativeKey, string name, string value)
		{
			RegistryKey rk = Registry.LocalMachine.OpenSubKey(BuildKeyName(relativeKey), true);
			if (rk != null)
			{
				if (value != null)
					rk.SetValue(name, value);
				else
					rk.DeleteValue(name);

				rk.Close();
			}
		}
		#endregion

		private RegistrySettings()
		{
		}
	}
}
