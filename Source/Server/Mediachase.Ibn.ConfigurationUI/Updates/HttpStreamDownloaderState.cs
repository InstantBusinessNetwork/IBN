using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.ConfigurationUI.Updates
{
	/// <summary>
	/// 
	/// </summary>
	public enum HttpStreamDownloaderState
	{
		/// <summary>
		/// 
		/// </summary>
		None = 0,

		/// <summary>
		/// 
		/// </summary>
		ConnectingToServer = 1,

		/// <summary>
		/// 
		/// </summary>
		Downloading = 2,

		/// <summary>
		/// 
		/// </summary>
		Completed = 3,

		/// <summary>
		/// 
		/// </summary>
		Failed = 4,
	}
}
