using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Mediachase.Ibn.Database
{
	internal class DBCommon
	{
		private DBCommon()
		{
		}

		public static string ReplaceSqlWildcard(string sourceString)
		{
			return Regex.Replace(sourceString, @"(\[|%|_)", "[$0]", RegexOptions.IgnoreCase);
		}
	}
}
