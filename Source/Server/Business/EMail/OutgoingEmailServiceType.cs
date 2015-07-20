using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Identifies outgoing email service.
	/// </summary>
	public enum OutgoingEmailServiceType
	{
		/// <summary>
		/// 
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// 
		/// </summary>
		AlertService = 1, // Global Service

		/// <summary>
		/// 
		/// </summary>
		SendFile = 2, // Global Service

		/// <summary>
		/// 
		/// </summary>
		HelpDeskEmailBox = 3, // + Pop3BoxId
	}
}
