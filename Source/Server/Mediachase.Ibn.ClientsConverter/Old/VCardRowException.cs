using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Mediachase.Ibn.Database.VCard
{
	[Serializable]
	public class VCardRowException : Exception
	{
		public VCardRowException()
			: base()
		{
		}

		public VCardRowException(string message)
			: base(message)
		{
		}

		public VCardRowException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected VCardRowException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}


		public VCardRowException(string className, string fieldName, int fieldValue)
			: this(string.Format(CultureInfo.InvariantCulture, "Record with {0}.{1}={2} not found.", className, fieldName, fieldValue))
		{
		}
	}
}
