using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Mediachase.Schedule.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class UrlInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UrlInfo"/> class.
		/// </summary>
		/// <param name="url">The URL.</param>
		public UrlInfo(string url)
		{
			this.Url = url;
			this.Credential = null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UrlInfo"/> class.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="connectionString">The connection string.</param>
		public UrlInfo(string url, string connectionString)
		{
			this.Url = url;
			this.ConnectionString = connectionString;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UrlInfo"/> class.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="credential">The credential.</param>
		public UrlInfo(string url, NetworkCredential credential)
		{
			this.Url = url;
			this.Credential = credential;
		}

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>The URL.</value>
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the credential.
		/// </summary>
		/// <value>The credential.</value>
		public NetworkCredential Credential { get; set; }

		/// <summary>
		/// Gets or sets the connection string.
		/// </summary>
		/// <value>The connection string.</value>
		public string ConnectionString { get; set; }
	}
}
