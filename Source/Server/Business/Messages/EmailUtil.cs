using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Mediachase.Net.Mail;

namespace Mediachase.Ibn.Business.Messages
{
	/// <summary>
	/// Represents EmailUtil.
	/// </summary>
	public static class EmailUtil
	{
		#region Methods
		/// <summary>
		/// Extracts the first email.
		/// </summary>
		/// <param name="addresses">The addresses.</param>
		/// <returns></returns>
		public static string ExtractFirstEmail(string addresses)
		{
			if (addresses == null)
				return string.Empty;

			Match match = Regex.Match(addresses, "((?<phrase>[^:]+):)?(?<mailbox>(((?<show_name>[^<]*)<)?(?<email>(?<localpart>[^@]+)@(?<domain>[^>]+))(>)?));?",
				RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

			if (match.Success &&
				match.Groups["email"] != null &&
				match.Groups["email"].Captures.Count > 0)
			{
				return match.Groups["email"].Captures[0].Value;
			}

			return addresses;
		}

		/// <summary>
		/// Extracts the name of the first show.
		/// </summary>
		/// <param name="addresses">The addresses.</param>
		/// <returns></returns>
		public static string ExtractFirstShowName(string addresses)
		{
			if (string.IsNullOrEmpty(addresses))
				return string.Empty;

			Match match = Regex.Match(addresses,
				"((?<phrase>[^:]+):)?(?<mailbox>(((?<show_name>[^<]*)<)?(?<email>(?<localpart>[^@]+)@(?<domain>[^>]+))(>)?));?",
				RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

			if (match.Success &&
				match.Groups["show_name"] != null &&
				match.Groups["show_name"].Captures.Count > 0)
			{
				string showName = match.Groups["show_name"].Captures[0].Value;
				if (showName != null)
				{
					showName = showName.Trim(' ', '"');
					if (showName != string.Empty)
						return showName;
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// Creates the mail address.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <returns></returns>
		public static MailAddress CreateMailAddress(string email)
		{
			return new MailAddress(EmailUtil.ExtractFirstEmail(email), EmailUtil.ExtractFirstShowName(email));
		}

		#endregion
	}
}
