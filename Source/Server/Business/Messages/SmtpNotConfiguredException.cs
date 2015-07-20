using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Business.Messages
{
	/// <summary>
	/// Represents SMTP not configured exception.
	/// </summary>
	public class SmtpNotConfiguredException: Exception
	{
		#region Const
		#endregion

		#region Properties
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="SmtpNotConfiguredException"/> class.
		/// </summary>
		public SmtpNotConfiguredException()
			: base("Cannot find an outgoing email server settings. Go to Administration/Common Settings/Outgoing email servers (Smtp) and create Smtp Box.")
		{
		}
		#endregion

		#region Methods
		#endregion
	}
}
