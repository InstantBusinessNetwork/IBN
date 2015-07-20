using System;

namespace Mediachase.Ibn
{
	internal class IbnConst
	{
		internal const string Version1 = "4.6";
		internal const string Version2 = "46";
		internal const string ProductName1 = "IBN " + Version1;
		internal const string ProductName = ProductName1 + " Server";
		internal const string AssemblyVersion = Version1 + ".0.*";
		internal const string EventLogName = ProductName;
		internal const string ProductGuid = "{6E00C76F-15BB-41CB-A393-00DB7288E0BD}"; // TODO: change GUID before release.
		internal const string ServerKey = @"Mediachase\Instant Business Network\" + Version1 + @"\Server";
		internal const string InstallKey = ServerKey + @"\Install";

		private IbnConst()
		{
		}
	}
}
