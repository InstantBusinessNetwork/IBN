using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.ConfigurationUI.Updates
{
	/// <summary>
	/// Provides data for the download progress event args event.
	/// </summary>
	public class DownloadProgressEventArgs: EventArgs
	{

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="DownloadProgressEventArgs"/> class.
		/// </summary>
		public DownloadProgressEventArgs(long contentLength, long receivedBytes)
		{
			this.ContentLength = contentLength;
			this.ReceivedBytes = receivedBytes;

			if(this.ContentLength!=0)
				this.Progress = (int)((this.ReceivedBytes * 100) / this.ContentLength);
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the length of the content.
		/// </summary>
		/// <value>The length of the content.</value>
		public long ContentLength { get; protected set; }

		/// <summary>
		/// Gets or sets the received bytes.
		/// </summary>
		/// <value>The received bytes.</value>
		public long ReceivedBytes { get; protected set; }

		/// <summary>
		/// Gets or sets the progress.
		/// </summary>
		/// <value>The progress.</value>
		public int Progress { get; protected set; }
		#endregion
	}

}
