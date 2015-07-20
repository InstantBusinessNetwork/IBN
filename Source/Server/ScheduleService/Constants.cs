using System;

namespace Mediachase.Ibn
{
	internal class Constants
	{
		internal const string Name = "Schedule Service";
		internal const string ServiceName = "ScheduleService" + IbnConst.VersionMajorMinor;
		internal const string DisplayName = IbnConst.ProductFamilyShort + " " + IbnConst.VersionMajorDotMinor + " " + Name;

		private Constants()
		{
		}
	}
}
