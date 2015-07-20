using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.XPath;

namespace Mediachase.Ibn.Configuration
{
	/// <summary>
	/// Represents Safe XmlDocument.
	/// </summary>
	internal sealed class SafeXmlDocument : IDisposable
	{
		#region Properties

		public string FilePath { get; private set; }
		private FileStream InnerFileStream { get; set; }

		#endregion

		#region .Ctor

		private SafeXmlDocument()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SafeXmlDocument"/> class.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="access">The access.</param>
		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
		internal SafeXmlDocument(string filePath, FileAccess access) :
			this(filePath, access, -1)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SafeXmlDocument"/> class.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="access">The access.</param>
		/// <param name="timeout">The timeout.</param>
		//[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
		//internal SafeIbnConfig(string filePath, FileAccess access, TimeSpan timeout) :
		//    this(filePath, access, (int)timeout.TotalMilliseconds)
		//{
		//}

		/// <summary>
		/// Initializes a new instance of the <see cref="SafeXmlDocument"/> class.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="access">The access.</param>
		/// <param name="millisecondsTimeout">The milliseconds timeout.</param>
		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
		internal SafeXmlDocument(string filePath, FileAccess access, int millisecondsTimeout)
		{
			// Save Path
			this.FilePath = filePath;
			//this.FileAccess = access;

			DateTime timeoutTime = DateTime.Now.AddMilliseconds(millisecondsTimeout);

			while (true)
			{
				try
				{
					this.InnerFileStream = File.Open(this.FilePath,
								FileMode.Open,
								access,
								access == FileAccess.Read ? FileShare.Read : FileShare.None);
				}
				catch (IOException ex)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					Trace.WriteLine(ex.Message, "SafeXmlDocument");

					// 32 -> The process cannot access the file because it is being used by another process
					if (lastWin32Error == 32)
					{
						// Wait Here And Try Againg
						Thread.Sleep(2);
						continue;
					}
				}

				// Check Timeout
				if (millisecondsTimeout > 0 && DateTime.Now > timeoutTime)
					throw new TimeoutException();

				break;
			}
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="SafeXmlDocument"/> is reclaimed by garbage collection.
		/// </summary>
		~SafeXmlDocument()
		{
			// Finalizer calls Dispose(false)
			Dispose(false);
		}

		#endregion

		#region Methods
		/// <summary>
		/// Loads the xml document.
		/// </summary>
		/// <returns></returns>
		internal XmlDocument LoadDocument()
		{
			if (this.InnerFileStream == null)
				throw new ObjectDisposedException("InnerFileStream");

			// Move to first position
			this.InnerFileStream.Seek(0, SeekOrigin.Begin);

			// Load Xml Document
			XmlDocument retVal = new XmlDocument();
			retVal.Load(this.InnerFileStream);
			return retVal;
		}

		/// <summary>
		/// Saves the xml document.
		/// </summary>
		/// <param name="xmlDocument">The XML document.</param>
		internal void SaveDocument(XmlDocument xmlDocument)
		{
			if (xmlDocument == null)
				throw new ArgumentNullException("xmlDocument");

			if (this.InnerFileStream == null)
				throw new ObjectDisposedException("InnerFileStream");

			// Move to first position
			this.InnerFileStream.Seek(0, SeekOrigin.Begin);

			// Save Xml Document
			xmlDocument.Save(this.InnerFileStream);
			this.InnerFileStream.SetLength(this.InnerFileStream.Position);
		}

		/// <summary>
		/// Releases this instance.
		/// </summary>
		//internal void Release()
		//{
		//    Dispose(true);
		//}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				// free managed resources
				if (this.InnerFileStream != null)
				{
					this.InnerFileStream.Flush();
					this.InnerFileStream.Close();
					this.InnerFileStream = null;
				}
			}

			// free native resources if there are any.
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
