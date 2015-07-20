using System;

namespace Mediachase.Ibn
{
	internal class Constants
	{
		internal const string Name = "OLE DB Service";
		internal const string ServiceName = "McOleDBService" + IbnConst.VersionMajorMinor;
		internal const string DisplayName = IbnConst.ProductFamilyShort + " " + IbnConst.VersionMajorDotMinor + " " + Name;

		private Constants()
		{
		}
	}
}
