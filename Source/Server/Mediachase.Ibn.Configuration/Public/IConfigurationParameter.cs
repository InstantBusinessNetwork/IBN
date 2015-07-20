using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Configuration
{
	public interface IConfigurationParameter
	{
		string Name { get; }
		string Value { get; }
		string Type { get; }
	}
}
