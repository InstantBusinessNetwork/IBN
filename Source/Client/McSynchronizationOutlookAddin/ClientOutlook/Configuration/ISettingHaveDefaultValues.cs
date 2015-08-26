using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutlookAddin.ClientOutlook.Configuration
{
	public interface ISettingHaveDefaultValues
	{
		IDictionary<string, object> DefaultValues { get; }
	}
}
