using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Mediachase.Net.Mail
{
	/// <summary>
	/// Represents mail address collection.
	/// </summary>
	public class MailAddressCollection : Collection<MailAddress>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MailAddressCollection"/> class.
		/// </summary>
		public MailAddressCollection()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MailAddressCollection"/> class.
		/// </summary>
		/// <param name="list">The list that is wrapped by the new collection.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="list"/> is null.</exception>
		public MailAddressCollection(IList<MailAddress> list)
			: base(list)
		{
		}
	}
}
