using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Configuration
{
	public interface IAspInfo
	{
		string Scheme { get; }
		string Host { get; }
		string Port { get; }
		string Database { get; }
		long SiteId { get; }
		string ApplicationPool { get; }
		bool IsApplicationPoolCreated { get; }
	}
}
