using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Configuration
{
	public interface ICompanyInfo
	{
		string Id { get; }
		bool IsActive { get; }
		string Scheme { get; }
		string Host { get; }
		string Port { get; }
		string Database { get; }
		string CodePath { get; }
		int CodeVersion { get; }
		long SiteId { get; }
		string IMPool { get; }
		string PortalPool { get; }
		bool IsPortalPoolCreated { get; }
		DateTime Created { get; }
		bool IsScheduleServiceEnabled { get; }

		ILanguageInfo DefaultLanguage { get; }
		int DatabaseSize { get; }
		int DatabaseState { get; }
		int DatabaseVersion { get; }
		int InternalUsersCount { get; }
		int ExternalUsersCount { get; }
	}
}
