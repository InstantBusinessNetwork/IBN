using System;

namespace Mediachase.Ibn
{
	internal class IbnConst
	{
		internal const string MajorVersion = "4";
		internal const string MinorVersion = "7";
		internal const string BuildNumber = "68";
		internal const string VersionMajorDotMinor = MajorVersion + "." + MinorVersion;
		internal const string VersionMajorMinor = MajorVersion + MinorVersion;
		internal const string FullVersion = VersionMajorDotMinor + "." + BuildNumber;
		internal const string AssemblyVersion = FullVersion + ".0";
		internal const string NeutralResourcesLanguage = "en";

#if RADIUS
		internal const string CompanyName = "Radius-Soft";
		internal const string AssemblyCompany = CompanyName;
		internal const string ProductFamily = "MagRul";
		internal const string ProductFamilyShort = ProductFamily;
		internal const string ProductGuid = "{297C8BB0-44B6-43F9-B353-7E36E88A08C4}";
		//internal const string ProductGuid = "{6FDD3C43-89F5-457E-B7CB-8B2A25450B29}";
#else
		internal const string CompanyName = "Mediachase";
		internal const string AssemblyCompany = CompanyName + " LTD";
		internal const string ProductFamily = "Instant Business Network";
		internal const string ProductFamilyShort = "IBN";
		internal const string ProductGuid = "{297C8BB0-44B6-43F9-B353-7E36E88A08C4}";
#endif

		internal const string ProductName = ProductFamilyShort + " Server " + VersionMajorDotMinor;
		internal const string FullProductName = CompanyName + " " + ProductName + "." + BuildNumber;
		internal const string AssemblyCopyright = "© 2011 " + AssemblyCompany + ". All rights reserved.";
		internal const string InstallKey = CompanyName + @"\" + ProductFamily + @"\" + VersionMajorDotMinor + @"\Server\Install";
		internal const string EventLogName = ProductFamilyShort + " " + VersionMajorDotMinor + " Server";

		private IbnConst()
		{
		}
	}
}
