using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Mediachase.Net.Mail
{
	/// <summary>
	/// Represents mail address.
	/// </summary>
	public class MailAddress
	{
		private string _address;
		private string _displayName;

		/// <param name="address">A Mail address in the format of local-part@domain without angle brackets.</param>
		public MailAddress(string address)
			: this(address, null)
		{
		}

		/// <param name="address">A Mail address in the format of local-part@domain without angle brackets.</param>
		/// <param name="displayName">A display name without surrounding quotes.</param>
		public MailAddress(string address, string displayName)
		{
			if (address == null)
				throw new ArgumentNullException("address");

			_address = address;
			_displayName = displayName;
		}

		/// <summary>
		/// Gets the address.
		/// </summary>
		/// <value>The address.</value>
		public string Address
		{
			get { return _address; }
		}

		/// <summary>
		/// Gets the display name.
		/// </summary>
		/// <value>The display name.</value>
		public string DisplayName
		{
			get { return _displayName; }
		}

		public static MailAddress Parse(string rawAddress)
		{
			Match adressMatch = Regex.Match(rawAddress.Trim(), "((?<show_name>[^<]*)<)?(?<email>(?<localpart>[^@]+)@(?<domain>[^>]+))(>)?");

			return new MailAddress(adressMatch.Groups["localpart"].Value + "@" +  adressMatch.Groups["domain"].Value, adressMatch.Groups["show_name"].Value);
		}

	}
}
