namespace Mediachase.Ibn.Configuration
{
	public interface ISqlServerSettings
	{
		string Server { get; }
		AuthenticationType Authentication { get; }
		string AdminUser { get; }
		string AdminPassword { get; }
		string PortalUser { get; }
		string PortalPassword { get; }
		string AdminConnectionString { get; }
	}
}
