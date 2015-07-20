using System.Collections;
using System.Collections.Specialized;
using Mediachase.IBN.Database.EMail;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for AttachmentInfo.
	/// </summary>
	public class AttachmentInfo
	{
		string _fileName = null;
		string _contentType = null;
		long _size = 0;
		private NameValueCollection _headers = new NameValueCollection();

		/// <summary>
		/// Initializes a new instance of the <see cref="AttachmentInfo"/> class.
		/// </summary>
		/// <param name="FileName">Name of the file.</param>
		/// <param name="ContentType">Type of the content.</param>
		public AttachmentInfo(string FileName, string ContentType, long Size)
		{
			_fileName = FileName;
			_contentType = ContentType;
			_size = Size;
		}

		/// <summary>
		/// Gets the headers.
		/// </summary>
		/// <value>The headers.</value>
		public NameValueCollection Headers
		{
			get 
			{
				return _headers;
			}
		}

		/// <summary>
		/// Gets the name of the file.
		/// </summary>
		/// <value>The name of the file.</value>
		public string FileName
		{
			get 
			{
				return _fileName;
			}
		}

		/// <summary>
		/// Gets the type of the content.
		/// </summary>
		/// <value>The type of the content.</value>
		public string ContentType
		{
			get 
			{
				return _contentType;
			}
		}

		/// <summary>
		/// Gets the size.
		/// </summary>
		/// <value>The size.</value>
		public long Size
		{
			get 
			{
				return _size;
			}
		}
	}
}
