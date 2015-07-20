using System;
using System.Collections.Generic;
using System.Text;

using Mediachase.Database;
using System.Data;

namespace Mediachase.Ibn.Configuration
{
	internal class ConfigurationParameter : IConfigurationParameter
	{
		public string Name { get; internal set; }
		public string Value { get; internal set; }
		public string Type { get; internal set; }

		internal ConfigurationParameter()
		{
		}

		internal ConfigurationParameter(string name, string value)
		{
			Name = name;
			Value = value;
		}

		internal ConfigurationParameter(string name, string value, string type)
			: this()
		{
			Name = name;
			Value = value;
			Type = type;
		}
	}
}
