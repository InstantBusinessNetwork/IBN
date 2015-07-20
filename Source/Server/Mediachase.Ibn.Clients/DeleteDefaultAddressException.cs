using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Clients
{
	[global::System.Serializable]
	public class DeleteDefaultAddressException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteDefaultAddressException"/> class.
		/// </summary>
		public DeleteDefaultAddressException() : base("Cannot delete default address.") { }
		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteDefaultAddressException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public DeleteDefaultAddressException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteDefaultAddressException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="inner">The inner.</param>
		public DeleteDefaultAddressException(string message, Exception inner) : base(message, inner) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteDefaultAddressException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
		/// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
		protected DeleteDefaultAddressException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

}
