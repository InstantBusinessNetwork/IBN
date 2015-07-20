using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Configuration
{
	public interface ILanguageInfo
	{
		int LanguageId { get; }
		string Locale { get; }
		string FriendlyName { get; }
	}
}
