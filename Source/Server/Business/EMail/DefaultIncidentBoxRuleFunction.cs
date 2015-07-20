using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for DefaultIncidentBoxRuleFunction.
	/// </summary>
	public class DefaultIncidentBoxRuleFunction
	{
		public DefaultIncidentBoxRuleFunction()
		{
		}

		/// <summary>
		/// Determines whether the specified data source has keyword.
		/// </summary>
		/// <param name="DataSource">The data source.</param>
		/// <param name="KeywordPrefix">The keyword prefix.</param>
		/// <param name="KeywordValue">The keyword value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified data source has keyword; otherwise, <c>false</c>.
		/// </returns>
		public bool HasKeyword(string DataSource, string KeywordPrefix, string KeywordValue)
		{
			return Regex.IsMatch(DataSource, string.Format(@"{0}\s*:(\s|\w)*{1}", KeywordPrefix, KeywordValue));
		}
	}
}
