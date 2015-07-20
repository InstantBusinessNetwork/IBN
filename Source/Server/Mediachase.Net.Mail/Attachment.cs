using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mediachase.Net.Mail
{
	/// <summary>
	/// Represents attachment.
	/// </summary>
	public class Attachment : IDisposable
	{
		private string _contentType;
		private string _name;
		private MemoryStream _data = new MemoryStream();

		/// <summary>
		/// Gets or sets the type of the content.
		/// </summary>
		/// <value>The type of the content.</value>
		public string ContentType
		{
			get { return _contentType; }
			set { _contentType = value; }
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Attachment"/> class.
		/// </summary>
		/// <param name="contentType">Type of the content.</param>
		/// <param name="filePath">The file path.</param>
		public Attachment(string contentType, string filePath)
		{
			if (contentType == null)
				throw new ArgumentNullException("contentType");
			if (filePath == null)
				throw new ArgumentNullException("filePath");

			_contentType = contentType;
			_name = Path.GetFileName(filePath);
			using (Stream data = File.OpenRead(filePath))
			{
				LoadData(data);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Attachment"/> class.
		/// </summary>
		/// <param name="contentType">Type of the content.</param>
		/// <param name="name">The name.</param>
		/// <param name="data">The data.</param>
		public Attachment(string contentType, string name, Stream data)
		{
			if (contentType == null)
				throw new ArgumentNullException("contentType");
			if (name == null)
				throw new ArgumentNullException("name");
			if (data == null)
				throw new ArgumentNullException("data");

			_contentType = contentType;
			_name = name;
			LoadData(data);
		}

		#region IDisposable Members
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="freeManagedResources"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool freeManagedResources)
		{
			if (freeManagedResources)
			{
				// free managed resources
				_data.Dispose();
			}

			// free native resources if there are any.
		}

		/// <summary>
		/// Gets the data.
		/// </summary>
		/// <returns></returns>
		internal byte[] GetData()
		{
			return _data.GetBuffer();
		}

		/// <summary>
		/// Loads the data.
		/// </summary>
		/// <param name="stream">The stream.</param>
		private void LoadData(Stream stream)
		{
			byte[] buffer = new byte[100 * 1024];

			int count;
			do
			{
				count = stream.Read(buffer, 0, buffer.Length);
				_data.Write(buffer, 0, count);
			}
			while (count > 0);

			_data.Flush();
			_data.Capacity = (int)_data.Length;
		}
	}
}
