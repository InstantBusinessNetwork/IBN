using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Mediachase.Net.Mail
{
	/// <summary>
	/// Represents attachment collection.
	/// </summary>
	public class AttachmentCollection : Collection<Attachment>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AttachmentCollection"/> class.
		/// </summary>
		public AttachmentCollection()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AttachmentCollection"/> class.
		/// </summary>
		/// <param name="list">The list that is wrapped by the new collection.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="list"/> is null.</exception>
		public AttachmentCollection(IList<Attachment> list):base(list)
		{

		}
	}
}
