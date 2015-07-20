using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Configuration
{
	public interface IUpdateInfo
	{
		int Version { get; }
		DateTime ReleaseDate { get; }
		bool UpdatesCommonComponents { get; }
		bool RequiresCommonComponentsUpdate { get; }
		string Changes { get; }
		string Warnings { get; }
	}
}
