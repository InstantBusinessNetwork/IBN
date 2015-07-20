using System.Globalization;

namespace Mediachase.Ibn.Configuration
{
	internal class SqlServerSettings : ISqlServerSettings
	{
		public string Server { get; internal set; }
		public AuthenticationType Authentication { get; internal set; }
		public string AdminUser { get; internal set; }
		public string AdminPassword { get; internal set; }
		public string PortalUser { get; internal set; }
		public string PortalPassword { get; internal set; }
		public string AdminConnectionString { get; internal set; }

		internal SqlServerSettings(string server, AuthenticationType authentication, string adminUser, string adminPassword, string ibnUser, string ibnPassword)
		{
			Server = server;
			Authentication = authentication;
			AdminUser = adminUser;
			AdminPassword = adminPassword;
			PortalUser = ibnUser;
			PortalPassword = ibnPassword;

			AdminConnectionString = BuildConnectionString(authentication, server, "master", adminUser, adminPassword);
		}

		#region internal static string BuildConnectionString(AuthenticationType authentication, string server, string database, string user, string password)
		internal static string BuildConnectionString(AuthenticationType authentication, string server, string database, string user, string password)
		{
			string format;

			if (authentication == AuthenticationType.SqlServer)
				format = "Data source={0};Initial catalog={1};User Id={2};Password={3}";
			else
				format = "Data source={0};Initial catalog={1};Integrated Security=SSPI";

			return string.Format(CultureInfo.InvariantCulture, format,
				server, database, user, password);
		}
		#endregion
	}
}
