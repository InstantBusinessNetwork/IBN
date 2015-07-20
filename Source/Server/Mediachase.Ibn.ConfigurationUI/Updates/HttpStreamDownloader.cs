using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace Mediachase.Ibn.ConfigurationUI.Updates
{
	/// <summary>
	/// Represents HTTP stream downloader.
	/// </summary>
	[DefaultEvent("Completed")]
	public partial class HttpStreamDownloader : Component
	{
		public const int ReadBufferSize = 48 * 1024;

		private byte[] innerBuffer = new byte[ReadBufferSize];

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpStreamDownloader"/> class.
		/// </summary>
		public HttpStreamDownloader()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpStreamDownloader"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		public HttpStreamDownloader(IContainer container)
		{
			container.Add(this);

			InitializeComponent();
		}

		#region Events
		public event EventHandler ConnectingToServer;
		public event EventHandler<DownloadProgressEventArgs> Downloading;

		public event EventHandler Completed;
		public event EventHandler Failed;
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the request URI.
		/// </summary>
		/// <value>The request URI.</value>
		[Category("Appearance")]
		[Localizable(true)]
		public Uri RequestUri { get; set; }

		/// <summary>
		/// Gets or sets the output file path.
		/// </summary>
		/// <value>The output file path.</value>
		[Category("Appearance")]
		[Localizable(true)]
		public string OutputFilePath { get; set; }

		/// <summary>
		/// Gets or sets the response stream.
		/// </summary>
		/// <value>The response stream.</value>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Stream ResponseStream { get; set; }

		/// <summary>
		/// Gets or sets the state.
		/// </summary>
		/// <value>The state.</value>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public HttpStreamDownloaderState State { get; protected set; }

		/// <summary>
		/// Gets or sets the error.
		/// </summary>
		/// <value>The error.</value>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Exception Error { get; protected set; }

		/// <summary>
		/// Gets the length of the content.
		/// </summary>
		/// <value>The length of the content.</value>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public long ContentLength 
		{ 
			get 
			{
				if (this.Response != null)
					return this.Response.ContentLength;

				return -1;
			}
		}

		protected HttpWebRequest Request { get; set; }
		protected HttpWebResponse Response { get; set; }
		protected Stream SourceResponseStream { get; set; }

		#endregion

		#region Methods
		/// <summary>
		/// Starts this instance.
		/// </summary>
		public void BeginDownload()
		{
			if(this.State!=HttpStreamDownloaderState.None && this.State!=HttpStreamDownloaderState.Failed &&
				this.State!=HttpStreamDownloaderState.Completed)
			{
				throw new ArgumentException("Wrong State" + this.State.ToString());
			}

			this.State = HttpStreamDownloaderState.None;

			this.Response = null;
			this.SourceResponseStream = null;

			this.Request = (HttpWebRequest)HttpWebRequest.Create(this.RequestUri);
			this.Request.MaximumAutomaticRedirections = 10;
			this.Request.Timeout = 60000;
			this.Request.ReadWriteTimeout = 60000;

			RaiseConnectingToServer();

			this.State = HttpStreamDownloaderState.ConnectingToServer;

			this.Request.BeginGetResponse(OnGetResponseEnd, null);
		}

		/// <summary>
		/// Aborts this instance.
		/// </summary>
		public void Abort()
		{
			try
			{
				if (this.Request != null)
					this.Request.Abort();
			}
			catch
			{
			}

			try
			{
				if(this.Response!=null)
					this.Response.Close();
			}
			catch
			{
			}

			this.State = HttpStreamDownloaderState.Failed;
		}
		#endregion

		#region Async Callbacks
		/// <summary>
		/// Called when [get response end].
		/// </summary>
		/// <param name="ar">The ar.</param>
		private void OnGetResponseEnd(IAsyncResult ar)
		{
			try
			{
				this.Response = (HttpWebResponse)this.Request.EndGetResponse(ar);

				// Create Source Stream
				this.SourceResponseStream = this.Response.GetResponseStream();

				// Create Destination Stream
				if (!string.IsNullOrEmpty(this.OutputFilePath))
				{
					this.ResponseStream = new FileStream(this.OutputFilePath, FileMode.CreateNew);
				}

				BeginRead();
			}
			catch (Exception ex)
			{
				this.Error = ex;
				this.State = HttpStreamDownloaderState.Failed;

				RaiseFailed();
			}
		}

		/// <summary>
		/// Begins the read.
		/// </summary>
		/// <param name="stream">The stream.</param>
		private void BeginRead()
		{
			try
			{
				this.State = HttpStreamDownloaderState.Downloading;

				IAsyncResult result = this.SourceResponseStream.BeginRead(innerBuffer, 0, ReadBufferSize, OnReadEnd, null);
			}
			catch (Exception ex)
			{
				this.Error = ex;
				this.State = HttpStreamDownloaderState.Failed;

				RaiseFailed();
			}
		}

		/// <summary>
		/// Called when [read end].
		/// </summary>
		/// <param name="ar">The ar.</param>
		private void OnReadEnd(IAsyncResult ar)
		{
			try
			{
				int realRead = this.SourceResponseStream.EndRead(ar);

				// Save buffer
				if (this.ResponseStream != null)
				{
					this.ResponseStream.Write(innerBuffer, 0, realRead);
				}

				RaiseDownloading(this.ContentLength, this.ResponseStream.Length);

				// Read next chunk
				if (realRead > 0)
				{
					BeginRead();
				}
				else
				{
					this.SourceResponseStream.Close();
					this.SourceResponseStream = null;

					// Check That 

					RaiseCompleted();
				}
			}
			catch (Exception ex)
			{
				this.Error = ex;
				this.State = HttpStreamDownloaderState.Failed;

				RaiseFailed();
			}
		}
		#endregion

		#region Event Methods
		protected void InnerSafeEventInvoke(Delegate handler, params object[] args)
		{
			try
			{
				if (handler != null)
				{
					foreach (Delegate delegateItem in handler.GetInvocationList())
					{
						Control control = delegateItem.Target as Control;
						if (control != null && control.InvokeRequired)
							control.Invoke(delegateItem, args);
						else
							delegateItem.DynamicInvoke(args);
					}
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex, "HttpStreamDownloader");
			}
		}

		/// <summary>
		/// Raises the connecting to server.
		/// </summary>
		private void RaiseConnectingToServer()
		{
			OnConnectingToServer(this, EventArgs.Empty);
		}

		/// <summary>
		/// Called when [connecting to server].
		/// </summary>
		protected virtual void OnConnectingToServer(object sender, EventArgs args)
		{
			InnerSafeEventInvoke(ConnectingToServer,sender, args);
		}


		/// <summary>
		/// Raises the downloading.
		/// </summary>
		private void RaiseDownloading(long contentLength, long receivedBytes)
		{
			DownloadProgressEventArgs args = new DownloadProgressEventArgs(contentLength, receivedBytes);

			OnDownloading(this, args);
		}

		/// <summary>
		/// Called when [downloading].
		/// </summary>
		protected virtual void OnDownloading(object sender, DownloadProgressEventArgs args)
		{
			InnerSafeEventInvoke(Downloading,sender, args);
		}

		/// <summary>
		/// Raises the completed.
		/// </summary>
		private void RaiseCompleted()
		{
			this.State = HttpStreamDownloaderState.Completed;

			if(!string.IsNullOrEmpty(this.OutputFilePath))
			{
				this.ResponseStream.Flush();
				this.ResponseStream.Close();
				this.ResponseStream = null;
			}

			OnCompleted(this, EventArgs.Empty);
		}

		/// <summary>
		/// Called when [completed].
		/// </summary>
		protected virtual void OnCompleted(object sender, EventArgs args)
		{
			InnerSafeEventInvoke(this.Completed, sender, args);
		}

		/// <summary>
		/// Raises the failed.
		/// </summary>
		private void RaiseFailed()
		{
			this.State = HttpStreamDownloaderState.Failed;

			if (!string.IsNullOrEmpty(this.OutputFilePath))
			{
				try
				{
					this.ResponseStream.Flush();
					this.ResponseStream.Close();
					this.ResponseStream = null;
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex, "HttpStreamDownloader");
				}

				try
				{
					if (this.Response != null)
						this.Response.Close();
				}
				catch (Exception ex)
				{
					
					Trace.WriteLine(ex, "HttpStreamDownloader");
				}
			}

			if (this.Failed.Target is Control && ((Control)this.Failed.Target).InvokeRequired)
				((Control)this.Failed.Target).Invoke(new EventHandler(OnFailed), this, EventArgs.Empty);
			else
				OnFailed(this, EventArgs.Empty);
		}

		/// <summary>
		/// Called when [failed].
		/// </summary>
		protected virtual void OnFailed(object sender, EventArgs args)
		{
			if (this.Failed != null)
				this.Failed(sender, args);
		}
		#endregion
	}
}
